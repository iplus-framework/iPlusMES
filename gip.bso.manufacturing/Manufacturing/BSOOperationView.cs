// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.autocomponent;
using VD = gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;
using ClosedXML.Excel;
using gip.core.media;
using System.Threading.Tasks;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'BSOOperationView'}de{'BSOOperationView'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]

    public class BSOOperationView : ACBSOvb
    {

        #region ctor's

        public BSOOperationView(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool baseACInit = base.ACInit(startChildMode);
            FilterStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            FilterEndTime = FilterStartTime.Value.AddMonths(1).AddMinutes(-1);
            SelectedTableMode = TableModeList.FirstOrDefault();
            return baseACInit;
        }

        #endregion

        #region Properties

        #region Properties -> Filter

        private DateTime? _FilterStartTime;
        [ACPropertyInfo(100, "FilterStartTime", Const.From)]
        public DateTime? FilterStartTime
        {
            get
            {
                return _FilterStartTime;
            }
            set
            {
                if (_FilterStartTime != value)
                {
                    _FilterStartTime = value;
                    OnPropertyChanged(nameof(FilterStartTime));
                }
            }
        }

        private DateTime? _FilterEndTime;
        [ACPropertyInfo(101, "FilterEndTime", Const.To)]
        public DateTime? FilterEndTime
        {
            get
            {
                return _FilterEndTime;
            }
            set
            {
                if (_FilterEndTime != value)
                {
                    _FilterEndTime = value;
                    OnPropertyChanged(nameof(FilterEndTime));
                }
            }
        }


        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilePath;
        [ACPropertyInfo(999, "FilePath", "en{'Export file'}de{'Exportdatei'}")]
        public string FilePath
        {
            get
            {
                return _FilePath;
            }
            set
            {
                if (_FilePath != value)
                {
                    _FilePath = value;
                    OnPropertyChanged();
                }
            }
        }

        private ACValueItem _SelectedTableMode;
        [ACPropertySelected(410, "TableMode")]
        public ACValueItem SelectedTableMode
        {
            get { return _SelectedTableMode; }
            set
            {
                _SelectedTableMode = value;
                OnPropertyChanged();
            }
        }

        private ACValueItemList _TableModeList;
        [ACPropertyList(411, "TableMode")]
        public ACValueItemList TableModeList
        {
            get
            {
                if (_TableModeList == null)
                {
                    _TableModeList = new ACValueItemList("");
                    _TableModeList.Add(new ACValueItem("en{'Operation log and params in one worksheet (true starschema)'}de{'Betriebsprotokoll und Parameter in einem Arbeitsblatt (echtes Starschema)'}", (short)1, null));
                    _TableModeList.Add(new ACValueItem("en{'Operation log and params in each worksheet'}de{'Betriebsprotokoll und Parameter in jedem Arbeitsblatt'}", (short)2, null));
                }
                return _TableModeList;
            }
        }



        #endregion

        #endregion


        #region ACMethod

        /// <summary>
        /// Exports the folder.
        /// </summary>
        [ACMethodInfo("SetFilePath", "en{'...'}de{'...'}", 200, false, false, true)]
        public void SetFilePath()
        {
            if (!IsEnabledSetFilePath())
                return;

            ACMediaController mediaController = ACMediaController.GetServiceInstance(this);
            string filePath = mediaController.OpenFileDialog(
                false,
                FilePath,
                false,
                ".xlsx",
                new Dictionary<string, string>()
                {
                    {
                        "Excel Files",
                        "*.xlsx, *.csv, *.xls"
                    }
                });

            if(filePath != null && Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                FilePath = filePath;
            }
            //using (var dialog = new CommonOpenFileDialog())
            //{
            //    dialog.IsFolderPicker = false;
            //    dialog.Filters.Clear();
            //    dialog.Filters.Add(new CommonFileDialogFilter("Excel (*.xlsx)", ".xlsx"));
            //    dialog.DefaultExtension = ".xlsx";
            //    if (!string.IsNullOrEmpty(FilePath))
            //    {
            //        dialog.DefaultDirectory = Path.GetDirectoryName(FilePath);
            //        dialog.DefaultFileName = Path.GetFileName(FilePath);
            //    }
            //    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            //    {
            //        if (!string.IsNullOrEmpty(dialog.FileName) && Directory.Exists(Path.GetDirectoryName(dialog.FileName)))
            //        {
            //            FilePath = dialog.FileName;
            //        }
            //    }
            //}
        }

        public bool IsEnabledSetFilePath()
        {
            return true;
        }



        /// <summary>
        /// Source Property: Export
        /// </summary>
        [ACMethodInfo("Export", "en{'Export'}de{'Exportieren'}", 201)]
        public async Task Export()
        {
            if (!IsEnabledExport())
                return;
            BackgroundWorker.RunWorkerAsync(nameof(DoExportOperationLogBI));
            await ShowDialogAsync(this, DesignNameProgressBar);
        }

        public bool IsEnabledExport()
        {
            return
                FilterStartTime != null
                && FilterEndTime != null
                && !string.IsNullOrEmpty(FilePath)
                && Directory.Exists(Path.GetDirectoryName(FilePath))
                && SelectedTableMode != null;
        }

        #endregion

        #region Message

        public void SendMessage(object result)
        {
            Msg msg = result as Msg;
            if (msg != null)
            {
                SendMessage(msg);
            }
        }

        /// <summary>
        /// The _ current MSG
        /// </summary>
        Msg _CurrentMsg;
        /// <summary>
        /// Gets or sets the current MSG.
        /// </summary>
        /// <value>The current MSG.</value>
        [ACPropertyCurrent(528, "Message", "en{'Message'}de{'Meldung'}")]
        public Msg CurrentMsg
        {
            get
            {
                return _CurrentMsg;
            }
            set
            {
                _CurrentMsg = value;
                OnPropertyChanged(nameof(CurrentMsg));
            }
        }

        private ObservableCollection<Msg> msgList;
        /// <summary>
        /// Gets the MSG list.
        /// </summary>
        /// <value>The MSG list.</value>
        [ACPropertyList(529, "Message", "en{'Messagelist'}de{'Meldungsliste'}")]
        public ObservableCollection<Msg> MsgList
        {
            get
            {
                if (msgList == null)
                    msgList = new ObservableCollection<Msg>();
                return msgList;
            }
        }

        public void SendMessage(Msg msg)
        {
            MsgList.Add(msg);
            OnPropertyChanged(nameof(MsgList));
        }

        public void ClearMessages()
        {
            MsgList.Clear();
            OnPropertyChanged(nameof(MsgList));
        }

        #endregion

        #region Excel export => Data types

        Type stringType = typeof(string);
        Type shortType = typeof(short);
        Type intType = typeof(int);
        Type doubleType = typeof(double);
        Type decimalType = typeof(decimal);
        Type floatType = typeof(float);

        #endregion

        #region BackgroundWorker

        /// <summary>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        public override void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            base.BgWorkerDoWork(sender, e);
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = e.Argument.ToString();

            worker.ProgressInfo.OnlyTotalProgress = true;
            worker.ProgressInfo.AddSubTask(command, 0, 9);
            string message = Translator.GetTranslation("en{'Running {0}...'}de{'{0} läuft...'}");
            worker.ProgressInfo.ReportProgress(command, 0, string.Format(message, command));

            string updateName = Root.Environment.User.Initials;

            switch (command)
            {
                case nameof(DoExportOperationLogBI):
                    DoExportOperationLogBI(DatabaseApp, FilePath, FilterStartTime.Value, FilterEndTime.Value, ((short)SelectedTableMode.Value) == 1);
                    break;

            }
        }

        public override void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            base.BgWorkerCompleted(sender, e);
            CloseWindow(this, DesignNameProgressBar);
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = worker.EventArgs.Argument.ToString();
            ClearMessages();
            if (e.Cancelled)
            {
                SendMessage(new Msg() { MessageLevel = eMsgLevel.Info, Message = string.Format(@"Operation {0} canceled by user!", command) });
            }
            if (e.Error != null)
            {
                SendMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = string.Format(@"Error by doing {0}! Message:{1}", command, e.Error.Message) });
            }
            else
            {

            }
        }

        #endregion

        #region BackgroundWorker -> DoMehtods

        #region BackgroundWorker -> DoMehtods -> GetOperationLogView

        private void DoExportOperationLogBI(VD.DatabaseApp databaseApp, string fileName, DateTime startTime, DateTime endTime, bool isOneTable)
        {
            List<VD.OperationLog> operationLogs =
                databaseApp
                .OperationLog
                .Where(c => c.InsertDate >= startTime && c.InsertDate < endTime)
                .ToList();


            List<OperationLogView> operationLogViews = GetOperationLogView(operationLogs);

            DataTable[] dataTables = GetDataTables(isOneTable);
            FillDataTables(operationLogViews, dataTables);
            WriteExcel(fileName, dataTables);
        }

        private List<OperationLogView> GetOperationLogView(List<VD.OperationLog> operationLogs)
        {
            List<OperationLogView> operationLogViews = new List<OperationLogView>();
            List<VD.OperationLog> filteredOperationLogs = 
                operationLogs
                .Where(c=>c.XMLValue != null && c.Operation == (short)VD.OperationLogEnum.UnregisterEntityOnScan)
                .ToList();
            foreach (VD.OperationLog operationLog in filteredOperationLogs)
            {
                OperationLogView wp = new OperationLogView();

                wp.OperationLogID = operationLog.OperationLogID;
                wp.UserName = operationLog.UpdateName;

                FillMaterialAndOrderData(operationLog, wp);
                FillTimeData(operationLogs, operationLog, wp);
                FillMachineData(operationLog, wp);
                FillParamsData(operationLog, wp);

                operationLogViews.Add(wp);
            }
            return operationLogViews;
        }


        private void FillMaterialAndOrderData(VD.OperationLog operationLog, OperationLogView wp)
        {
            wp.MaterialNo = operationLog.FacilityCharge?.Partslist?.Material?.MaterialNo;
            wp.MaterialName = operationLog.FacilityCharge?.Partslist?.Material?.MaterialName1;
            wp.ProgramNo = operationLog.FacilityCharge?.ProdOrderProgramNo;
            wp.FacilityLotNo = operationLog.FacilityCharge?.FacilityLot?.LotNo;
            wp.SplitNo = operationLog.FacilityCharge?.SplitNo ?? 0;
        }

        private void FillTimeData(List<VD.OperationLog> operationLogs, VD.OperationLog operationLog, OperationLogView wp)
        {
            VD.OperationLog initialOperationLog =
                    operationLogs
                    .Where(c =>
                                c.ACProgramLogID == operationLog.ACProgramLogID
                                && c.FacilityChargeID == operationLog.FacilityChargeID
                                && c.OperationLogID != operationLog.OperationLogID
                                && c.InsertDate < operationLog.InsertDate
                                && c.Operation == (short)VD.OperationLogEnum.RegisterEntityOnScan)
                    .OrderByDescending(c => c.InsertDate)
                    .FirstOrDefault();

            wp.StartTime = operationLog.InsertDate;
            if (initialOperationLog != null)
            {
                wp.StartTime = initialOperationLog.InsertDate;
            }
            wp.EndTime = operationLog.InsertDate;
        }

        private void FillMachineData(VD.OperationLog operationLog, OperationLogView wp)
        {
            wp.ACCaptionInstance = Translator.GetTranslation(operationLog.RefACClass.ACCaptionTranslation);
            wp.ACUrlInstance = operationLog.RefACClass.ACURLComponentCached;

            VD.ACClass basedOnClass = operationLog.RefACClass.ACClass1_BasedOnACClass;
            if (basedOnClass != null)
            {
                wp.ACCaptionTypeModel = Translator.GetTranslation(basedOnClass.ACCaptionTranslation);
                wp.ACUrlTypeModel = basedOnClass.ACURLCached;
            }
        }

        private void FillParamsData(VD.OperationLog operationLog, OperationLogView wp)
        {
            ACMethod method = gip.core.datamodel.ACClassMethod.DeserializeACMethod(operationLog.XMLValue);
            wp.MethodeName = method.ACCaption;

            foreach (ACValue aCValue in method.ParameterValueList)
            {
                OperationLogParam pr = GetOperationLogParam(aCValue, operationLog.OperationLogID, true, false);
                wp.Params.Add(pr);
            }

            foreach (ACValue aCValue in method.ResultValueList)
            {
                OperationLogParam pr = GetOperationLogParam(aCValue,operationLog.OperationLogID, false, true);
                wp.Params.Add(pr);
            }
        }

        private OperationLogParam GetOperationLogParam(ACValue aCValue, Guid operationLogID, bool isParam, bool isResult)
        {
            OperationLogParam pr = new OperationLogParam();
            pr.OperationLogID = operationLogID;
            pr.ParameterName = aCValue.ACCaption;
            pr.IsParam = isParam;
            pr.IsResult = isResult;
            if (aCValue.Value != null)
            {
                if (typeof(string).Name == aCValue.DataTypeName)
                {
                    pr.ValueStr = aCValue.Value.ToString();
                }
                else if (typeof(double).Name == aCValue.DataTypeName)
                {
                    pr.ValueDouble = aCValue.ValueT<double>();
                }
                else if (typeof(int).Name == aCValue.DataTypeName)
                {
                    pr.ValueInt = aCValue.ValueT<int>();
                }
            }
            return pr;
        }

        #endregion

        #region BackgroundWorker -> DoMehtods -> Prepare Data Tables

        private DataTable[] GetDataTables(bool isOneTable)
        {
            List<DataTable> dataTables = new List<DataTable>();

            DataTable operationLogTable = new DataTable(nameof(OperationLogView));
            dataTables.Add(operationLogTable);

            if (!isOneTable)
            {
                operationLogTable.Columns.Add(nameof(OperationLogView.OperationLogID), typeof(Guid));
            }
            operationLogTable.Columns.Add(nameof(OperationLogView.MaterialNo), typeof(string));
            operationLogTable.Columns.Add(nameof(OperationLogView.MaterialName), typeof(string));
            operationLogTable.Columns.Add(nameof(OperationLogView.ProgramNo), typeof(string));
            operationLogTable.Columns.Add(nameof(OperationLogView.FacilityLotNo), typeof(string));
            operationLogTable.Columns.Add(nameof(OperationLogView.SplitNo), typeof(int));
            operationLogTable.Columns.Add(nameof(OperationLogView.ACCaptionInstance), typeof(string));
            operationLogTable.Columns.Add(nameof(OperationLogView.ACUrlInstance), typeof(string));
            operationLogTable.Columns.Add(nameof(OperationLogView.ACCaptionTypeModel), typeof(string));
            operationLogTable.Columns.Add(nameof(OperationLogView.ACUrlTypeModel), typeof(string));
            operationLogTable.Columns.Add(nameof(OperationLogView.MethodeName), typeof(string));
            operationLogTable.Columns.Add(nameof(OperationLogView.StartTime), typeof(DateTime));
            operationLogTable.Columns.Add(nameof(OperationLogView.EndTime), typeof(DateTime));

            if (isOneTable)
            {
                operationLogTable.Columns.Add(nameof(OperationLogView.ParameterName), typeof(string));
                operationLogTable.Columns.Add(nameof(OperationLogView.ValueStr), typeof(string));
                operationLogTable.Columns.Add(nameof(OperationLogView.ValueDouble), typeof(double));
            }
            else
            {
                DataTable operationLogParamTable = new DataTable(nameof(OperationLogParam));
                dataTables.Add(operationLogParamTable);

                operationLogParamTable.Columns.Add(nameof(OperationLogView.OperationLogID), typeof(Guid));
                operationLogParamTable.Columns.Add(nameof(OperationLogView.ParameterName), typeof(string));
                operationLogParamTable.Columns.Add(nameof(OperationLogView.ValueStr), typeof(string));
                operationLogParamTable.Columns.Add(nameof(OperationLogView.ValueDouble), typeof(double));
            }

            return dataTables.ToArray();
        }

        private void FillDataTables(List<OperationLogView> operationLogViews, DataTable[] dataTables)
        {
            DataTable operationLogTable = dataTables.FirstOrDefault(c => c.TableName == nameof(OperationLogView));
            DataTable operationLogParamTable = dataTables.FirstOrDefault(c => c.TableName == nameof(OperationLogParam));
            foreach (OperationLogView operationLogView in operationLogViews)
            {
                if (operationLogParamTable != null)
                {
                    DataRow operationLogRow = GetBasicOperationLogRow(operationLogTable, operationLogView);
                    operationLogTable.Rows.Add(operationLogRow);
                    operationLogRow[nameof(OperationLogView.OperationLogID)] = operationLogView.OperationLogID;
                    foreach (OperationLogParam operationLogParam in operationLogView.Params)
                    {
                        DataRow operationLogParamRow = GetOperationLogParamRow(operationLogParamTable, operationLogParam);
                        operationLogParamTable.Rows.Add(operationLogParamRow);
                    }
                }
                else
                {
                    foreach (OperationLogParam operationLogParam in operationLogView.Params)
                    {
                        DataRow operationLogRow = GetBasicOperationLogRow(operationLogTable, operationLogView);
                        WriteInOperationLogRowParam(operationLogRow, operationLogParam);
                        operationLogTable.Rows.Add(operationLogRow);
                    }
                }
            }
        }

        public DataRow GetBasicOperationLogRow(DataTable operationLogTable, OperationLogView operationLogView)
        {
            DataRow operationLogRow = operationLogTable.NewRow();
            operationLogRow[nameof(OperationLogView.MaterialNo)] = operationLogView.MaterialNo;
            operationLogRow[nameof(OperationLogView.MaterialName)] = operationLogView.MaterialName;
            operationLogRow[nameof(OperationLogView.FacilityLotNo)] = operationLogView.FacilityLotNo;
            operationLogRow[nameof(OperationLogView.ProgramNo)] = operationLogView.ProgramNo;
            operationLogRow[nameof(OperationLogView.SplitNo)] = operationLogView.SplitNo;
            operationLogRow[nameof(OperationLogView.ACCaptionInstance)] = operationLogView.ACCaptionInstance;
            operationLogRow[nameof(OperationLogView.ACUrlInstance)] = operationLogView.ACUrlInstance;
            operationLogRow[nameof(OperationLogView.ACCaptionTypeModel)] = operationLogView.ACCaptionTypeModel;
            operationLogRow[nameof(OperationLogView.ACUrlTypeModel)] = operationLogView.ACUrlTypeModel;
            operationLogRow[nameof(OperationLogView.MethodeName)] = operationLogView.MethodeName;
            operationLogRow[nameof(OperationLogView.StartTime)] = operationLogView.StartTime;
            operationLogRow[nameof(OperationLogView.EndTime)] = operationLogView.EndTime;
            return operationLogRow;
        }

        private void WriteInOperationLogRowParam(DataRow dataRow, OperationLogParam operationLogParam)
        {
            dataRow[nameof(OperationLogView.ParameterName)] = operationLogParam.ParameterName;
            if (operationLogParam.ValueStr != null)
            {
                dataRow[nameof(OperationLogView.ValueStr)] = operationLogParam.ValueStr;
            }
            if (operationLogParam.ValueDouble != null)
            {
                dataRow[nameof(OperationLogView.ValueDouble)] = operationLogParam.ValueDouble;
            }
        }

        public DataRow GetOperationLogParamRow(DataTable operationLogParamTable, OperationLogParam operationLogParam)
        {
            DataRow operationLogParamRow = operationLogParamTable.NewRow();
            operationLogParamRow[nameof(OperationLogView.OperationLogID)] = operationLogParam.OperationLogID;
            WriteInOperationLogRowParam(operationLogParamRow, operationLogParam);
            return operationLogParamRow;
        }

        #endregion

        #region BackgroundWorker -> DoMehtods -> Excel
        private void WriteExcel(string fileName, DataTable[] dataTables)
        {
            XLWorkbook workBook = new XLWorkbook();

            foreach (DataTable dataTable in dataTables)
            {
                AddWorkSheet(workBook, dataTable);
            }

            workBook.SaveAs(fileName);
        }

        private void AddWorkSheet(XLWorkbook workBook, DataTable dataTable)
        {
            IXLWorksheet workSheet = workBook.Worksheets.Add(dataTable.TableName);

            int colNr = 1;
            foreach (DataColumn col in dataTable.Columns)
            {
                workSheet.Cell(1, colNr).Value = col.Caption;
                workSheet.Cell(1, colNr).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                workSheet.Cell(1, colNr).Style.Border.RightBorderColor = XLColor.Gray;
                colNr++;
            }

            int rowNr = 2;
            foreach (DataRow row in dataTable.Rows)
            {
                BackgroundWorker.ProgressInfo.ReportProgress("", BackgroundWorker.ProgressInfo.TotalProgress.ProgressCurrent + 1, "");
                for (int i = 0; i < row.ItemArray.Count(); i++)
                {
                    string value = row[i].ToString();
                    if (value != null)
                    {
                        DataColumn col = dataTable.Columns[i];

                        short shortValue;
                        int intValue;
                        float floatValue;
                        double doubleValue;
                        decimal decimalValue;

                        IXLCell cell = workSheet.Cell(rowNr, i + 1);
                        if (col.DataType == stringType)
                        {
                            cell.SetValue(value);
                            continue;
                        }

                        else if (col.DataType == shortType && short.TryParse(value, out shortValue))
                        {
                            cell.SetValue(shortValue);
                            continue;
                        }

                        else if (col.DataType == intType && int.TryParse(value, out intValue))
                        {
                            cell.SetValue(intValue);
                            continue;
                        }

                        else if (col.DataType == floatType && float.TryParse(value, out floatValue))
                        {
                            cell.SetValue(floatValue);
                            continue;
                        }

                        else if (col.DataType == doubleType && double.TryParse(value, out doubleValue))
                        {
                            cell.SetValue(doubleValue);
                            continue;
                        }

                        else if (col.DataType == decimalType && decimal.TryParse(value, out decimalValue))
                        {
                            cell.SetValue(decimalValue);
                            continue;
                        }

                        else if (col.DataType.IsEnum && !string.IsNullOrEmpty(value))
                        {
                            try
                            {
                                cell.SetValue(Enum.Parse(col.DataType, value).ToString());
                                continue;
                            }
                            catch (Exception e)
                            {
                                Messages.LogException(this.GetACUrl(), "Export(0)", e);
                            }
                        }

                        cell.SetValue(value);
                    }
                }

                rowNr++;
            }

            workSheet.Columns().AdjustToContents();
            workSheet.SheetView.FreezeRows(1);
            workSheet.SheetView.FreezeColumns(1);
        }

        #endregion

        #endregion
    }


}