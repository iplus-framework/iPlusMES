// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.core.autocomponent;
using System.Collections.ObjectModel;
using gip.mes.processapplication;
using gip.bso.masterdata;
using System.IO;
using System.Data;
using gip.core.media;
using System.Threading.Tasks;

namespace gip.bso.manufacturing
{
    /// <summary>
    /// Business object or App for managing laboratory orders in the MES (manufacturing execution system).
    /// It extends the <see cref="BSOLabOrder"/> base app and adds MES-specific features: Excel export of selected
    /// laboratory orders and weighing statistics (sample weighing charts) for laboratory order positions with a
    /// sample weighing tag.
    /// To search for records, enter the search string in the SearchWord property (inherited from BSOLabOrder).
    /// The database result is copied to the LabOrderList property.
    /// Then call the NavigateFirst() method to set CurrentLabOrder with the first record in the list.
    /// CurrentLabOrder is used to display and edit the currently selected record; the positions of the lab order are
    /// in the LabOrderPosList property and the currently selected position in the CurrentLabOrderPos property.
    /// Property changes should always be made to CurrentLabOrder / CurrentLabOrderPos and when all field values have been
    /// changed, the Save() method should be called to save the changes in the database before navigating to the next
    /// record or creating a new record.
    /// New laboratory orders are created with the inherited dialog workflow (New Lab Order, then Create) from a
    /// laboratory order template; they are usually generated automatically by a workflow node or another module.
    /// The Load() method updates the current objects with fresh database data if another user has made changes in the background.
    /// Special workflows of this MES extension:
    /// - ExportToExcel(): exports all selected lab orders (IsSelected in LabOrderList, with a production order position)
    ///   to an Excel file chosen via ExportFilePath. Runs in the background worker; progress is shown in a progress dialog
    ///   and result messages are collected in MsgList/CurrentMsg.
    /// - FilterLabOrderSelectAll selects/deselects all lab orders in LabOrderList for the export.
    /// - When CurrentLabOrderPos changes, the weighing statistics are rebuilt: PiStats/SamplePiStatsList hold the recorded
    ///   sample values of the position and SamplePiMaxList/SamplePiMinList/SamplePiSetPointList the corresponding
    ///   max/min/setpoint series (only for positions whose MDLabTag is a sample weighing tag).
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOLabOrderMES&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Laboratory Order MES'}de{'Laborauftrag MES'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + LabOrder.ClassName,
        Description = @"Business object or App for managing laboratory orders in the MES (manufacturing execution system).
        It extends the BSOLabOrder base app and adds MES-specific features: Excel export of selected laboratory orders and weighing statistics (sample weighing charts) for laboratory order positions with a sample weighing tag.
        To search for records, enter the search string in the SearchWord property (inherited from BSOLabOrder).
        The database result is copied to the LabOrderList property.
        Then call the NavigateFirst() method to set CurrentLabOrder with the first record in the list.
        CurrentLabOrder is used to display and edit the currently selected record; the positions of the lab order are in the LabOrderPosList property and the currently selected position in the CurrentLabOrderPos property.
        Property changes should always be made to CurrentLabOrder / CurrentLabOrderPos and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
        New laboratory orders are created with the inherited dialog workflow (New Lab Order, then Create) from a laboratory order template; they are usually generated automatically by a workflow node or another module.
        The Load() method updates the current objects with fresh database data if another user has made changes in the background.
        Special workflows of this MES extension:
        - ExportToExcel(): exports all selected lab orders (IsSelected in LabOrderList, with a production order position) to an Excel file chosen via ExportFilePath. Runs in the background worker; progress is shown in a progress dialog and result messages are collected in MsgList/CurrentMsg.
        - FilterLabOrderSelectAll selects/deselects all lab orders in LabOrderList for the export.
        - When CurrentLabOrderPos changes, the weighing statistics are rebuilt: PiStats/SamplePiStatsList hold the recorded sample values of the position and SamplePiMaxList/SamplePiMinList/SamplePiSetPointList the corresponding max/min/setpoint series (only for positions whose MDLabTag is a sample weighing tag).
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOLabOrderMES&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOLabOrderMES : BSOLabOrder
    {

        #region const

        public const string BGWorkerMehtod_DoExportExcel = @"DoExportExcel";

        #endregion

        #region c´tors

        public BSOLabOrderMES(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            this.PropertyChanged += BSOLabOrderMES_PropertyChanged;
            //Filter();
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            this.PropertyChanged -= BSOLabOrderMES_PropertyChanged;

            this._CurrentSamplePiStats = null;
            this._SelectedSamplePiStats = null;
            return await base.ACDeInit(deleteACClassTask);
        }

        void BSOLabOrderMES_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentLabOrderPos))
            {
                ReportChangedSampleWeighing();
            }
        }

        public override LabOrder CurrentLabOrder
        {
            get => base.CurrentLabOrder;
            set
            {
                base.CurrentLabOrder = value;
            }
        }

        #endregion

        #region Properties

        string _ExportFilePath;

        [ACPropertyInfo(500, "ExportFilePath", "en{'Export File Path'}de{'Export File Path'}", Description = @"Path of the Excel file (.xlsx) to which the selected laboratory orders are exported. Set via the file dialog opened by ExportToExcel().")]
        public string ExportFilePath
        {
            get
            {
                return _ExportFilePath;
            }
            set
            {
                if (_ExportFilePath != value)
                {
                    _ExportFilePath = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private bool _FilterLabOrderSelectAll;
        [ACPropertyInfo(999, "", ConstApp.SelectAll, Description = @"Select all checkbox: sets or clears the IsSelected flag of all laboratory orders in LabOrderList, e.g. to select all orders for the Excel export.")]
        public bool FilterLabOrderSelectAll
        {
            get
            {
                return _FilterLabOrderSelectAll;
            }
            set
            {
                if (_FilterLabOrderSelectAll != value)
                {
                    _FilterLabOrderSelectAll = value;
                    if (LabOrderList != null && LabOrderList.Any())
                    {
                        LabOrderList.ToList().ForEach(c => c.IsSelected = value);
                        OnPropertyChanged(nameof(LabOrderList));
                    }
                    OnPropertyChanged(nameof(FilterLabOrderSelectAll));
                }
            }
        }

        #endregion

        #region Methods

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                //case "NewSamplePiStats":
                //    NewSamplePiStats();
                //    return true;
                //case "IsEnabledNewSamplePiStats":
                //    result = IsEnabledNewSamplePiStats();
                //    return true;
                //case "DeleteSamplePiStats":
                //    DeleteSamplePiStats();
                //    return true;
                //case "IsEnabledDeleteSamplePiStats":
                //    result = IsEnabledDeleteSamplePiStats();
                //    return true;
                default:
                    break;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }
        
        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(ExportToExcel):
                    return new string[] { nameof(LabOrderList), nameof(BackgroundWorker) };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }


        [ACMethodInfo("ExportToExcel", "en{'Export to Excel'}de{'Zum Excel exportieren'}", 701, Description = @"Exports all selected laboratory orders (IsSelected, with a production order position) to the Excel file chosen via ExportFilePath. Opens a file dialog, then runs the export in the background worker with a progress dialog.")]
        public async Task ExportToExcel()
        {

            if (!IsEnabledExportToExcel())
            {
                return;
            }

            ACMediaController mediaController = ACMediaController.GetServiceInstance(this);
            string exportFilePath = await mediaController.OpenFileDialog(
                false,
                ExportFilePath,
                false,
                ".xlsx",
                new Dictionary<string, string>()
                {
                    {
                        "Excel Files",
                        "*.xlsx, *.csv, *.xls"
                    }
                });

            if (exportFilePath != null && Directory.Exists(Path.GetDirectoryName(exportFilePath)))
            {
                ExportFilePath = exportFilePath;
                BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoExportExcel);
                await ShowDialogAsync(this, DesignNameProgressBar);
            }
        }

        public bool IsEnabledExportToExcel()
        {
            return
                !BackgroundWorker.IsBusy
                && LabOrderList != null
                && LabOrderList.Any(c => c.IsSelected)
                && LabOrderList.Any(c => c.IsSelected && c.ProdOrderPartslistPos != null);
        }

        private void DoExportToExcel(string exportFilePath, LabOrder[] selectedLabOrders)
        {
            using (Database database = new core.datamodel.Database())
            {
                LabOrderToExcel.DoLabOrderToExcel(database, exportFilePath, selectedLabOrders);
            }
        }

        #endregion 

        #region Properties


        #region Properties -> Messages

        Msg _CurrentMsg;

        [ACPropertyCurrent(528, "Message", "en{'Message'}de{'Meldung'}", Description = @"Currently displayed message of a background operation (e.g. export canceled or failed).")]
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

        [ACPropertyList(529, "Message", "en{'Messagelist'}de{'Meldungsliste'}", Description = @"Messages collected during background operations (e.g. export errors); cleared when a new background operation completes.")]
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

        #endregion

        #region BackgroundWorker

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
                case BGWorkerMehtod_DoExportExcel:
                    LabOrder[] selectedLabOrders = LabOrderList.Where(c => c.IsSelected && c.ProdOrderPartslistPos != null).ToArray();
                    DoExportToExcel(ExportFilePath, selectedLabOrders);
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
        }

        #endregion

        #region Managers

        #endregion

        #region GrainSize

        SamplePiValue _SelectedSamplePiStats;
        [ACPropertySelected(200, "SamplePiStats", Description = @"Selected sample value in the weighing statistics chart of the current laboratory order position.")]
        public SamplePiValue SelectedSamplePiStats
        {
            get
            {
                return _SelectedSamplePiStats;
            }
            set
            {
                _SelectedSamplePiStats = value;
                OnPropertyChanged(nameof(SelectedSamplePiStats));
            }
        }

        SamplePiValue _CurrentSamplePiStats;
        /// <summary>
        /// Gets or sets the current facility.
        /// </summary>
        /// <value>The current facility.</value>
        [ACPropertyCurrent(201, "SamplePiStats", Description = @"Currently displayed sample value of the weighing statistics of the current laboratory order position.")]
        public SamplePiValue CurrentSamplePiStats
        {
            get
            {
                return _CurrentSamplePiStats;
            }
            set
            {
                _CurrentSamplePiStats = value;
                OnPropertyChanged(nameof(CurrentSamplePiStats));
            }
        }

        [ACPropertyList(202, "SamplePiStats", Description = @"Recorded sample weighing values of the current laboratory order position (only for positions whose lab tag is a sample weighing tag).")]
        public IList<SamplePiValue> SamplePiStatsList
        {
            get
            {
                if (_PiStats == null)
                    return new List<SamplePiValue>();
                return _PiStats.Values;
            }
        }

        public SamplePiStats _PiStats;
        [ACPropertyInfo(203, "", "en{'Weighing statistics'}de{'Wäge-Statistiken'}", Description = @"Weighing statistics of the current laboratory order position: sample values with tolerances and setpoint, rebuilt whenever CurrentLabOrderPos changes.")]
        public SamplePiStats PiStats
        {
            get
            {
                return _PiStats;
            }
        }

        protected void ReportChangedSampleWeighing()
        {
            RebuildSamplePiStatsStats();
            OnPropertyChanged(nameof(PiStats));
            OnPropertyChanged(nameof(SamplePiStatsList));
            OnPropertyChanged(nameof(SamplePiMaxList));
            OnPropertyChanged(nameof(SamplePiMinList));
            OnPropertyChanged(nameof(SamplePiSetPointList));
        }

        protected void RebuildSamplePiStatsStats()
        {
            _PiStats = null;
            _SamplePiMaxList = null;
            _SamplePiMinList = null;
            _SamplePiSetPointList = null;

            if (CurrentLabOrderPos == null || CurrentLabOrderPos.MDLabTag == null || CurrentLabOrderPos.MDLabTag.MDKey != PWSampleWeighing.C_LabOrderPosTagKey)
                return;

            try
            {
                _PiStats = CurrentLabOrderPos[PWSamplePiLightBox.C_LabOrderExtFieldStats] as SamplePiStats;
                if (_PiStats != null)
                {
                    if ((_PiStats.TolPlus <= double.Epsilon || _PiStats.TolMinus <= double.Epsilon || _PiStats.SetPoint <= double.Epsilon)
                    && (CurrentLabOrderPos.ReferenceValue.HasValue && CurrentLabOrderPos.ValueMax.HasValue && CurrentLabOrderPos.ValueMin.HasValue))
                        _PiStats.SetToleranceAndRecalc(CurrentLabOrderPos.ReferenceValue.Value, CurrentLabOrderPos.ValueMax.Value - CurrentLabOrderPos.ReferenceValue.Value, CurrentLabOrderPos.ReferenceValue.Value - CurrentLabOrderPos.ValueMin.Value, true);
                    if (_PiStats.Values.Any())
                    {
                        _SamplePiMaxList = _PiStats.Values.Select(c => new SamplePiValue() { DTStamp = c.DTStamp, Value = CurrentLabOrderPos.ValueMax.HasValue ? CurrentLabOrderPos.ValueMax.Value : 0.0 }).ToArray();
                        _SamplePiMinList = _PiStats.Values.Select(c => new SamplePiValue() { DTStamp = c.DTStamp, Value = CurrentLabOrderPos.ValueMin.HasValue ? CurrentLabOrderPos.ValueMin.Value : 0.0 }).ToArray();
                        _SamplePiSetPointList = _PiStats.Values.Select(c => new SamplePiValue() { DTStamp = c.DTStamp, Value = CurrentLabOrderPos.ReferenceValue.HasValue ? CurrentLabOrderPos.ReferenceValue.Value : 0.0 }).ToArray();
                    }
                }
            }
            catch (Exception e)
            {
                Messages.ExceptionAsync(this, e.Message, true);
            }
            return;
        }


        protected IList<SamplePiValue> _SamplePiMaxList;
        [ACPropertyList(203, "SamplePiMaxStats", Description = @"Upper tolerance limit series (ValueMax of the position) for the weighing statistics chart.")]
        public IList<SamplePiValue> SamplePiMaxList
        {
            get
            {
                return _SamplePiMaxList;
            }
        }


        protected IList<SamplePiValue> _SamplePiMinList;
        [ACPropertyList(204, "SamplePiMinStats", Description = @"Lower tolerance limit series (ValueMin of the position) for the weighing statistics chart.")]
        public IList<SamplePiValue> SamplePiMinList
        {
            get
            {
                return _SamplePiMinList;
            }
        }


        protected IList<SamplePiValue> _SamplePiSetPointList;
        [ACPropertyList(205, "SamplePiSetPointStats", Description = @"Setpoint series (ReferenceValue of the position) for the weighing statistics chart.")]
        public IList<SamplePiValue> SamplePiSetPointList
        {
            get
            {
                return _SamplePiSetPointList;
            }
        }

        #endregion
    }

}
