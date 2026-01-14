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

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Laboratory Order MES'}de{'Laborauftrag MES'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + LabOrder.ClassName)]
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

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            this.PropertyChanged -= BSOLabOrderMES_PropertyChanged;

            this._CurrentSamplePiStats = null;
            this._SelectedSamplePiStats = null;
            return base.ACDeInit(deleteACClassTask);
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

        [ACPropertyInfo(500, "ExportFilePath", "en{'Export File Path'}de{'Export File Path'}")]
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
        [ACPropertyInfo(999, "", ConstApp.SelectAll)]
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

        [ACMethodInfo("ExportToExcel", "en{'Export to Excel'}de{'Zum Excel exportieren'}", 701)]
        public void ExportToExcel()
        {

            if (!IsEnabledExportToExcel())
            {
                return;
            }

            ACMediaController mediaController = ACMediaController.GetServiceInstance(this);
            string exportFilePath = mediaController.OpenFileDialog(
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
                ShowDialog(this, DesignNameProgressBar);
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
        [ACPropertySelected(200, "SamplePiStats")]
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
        [ACPropertyCurrent(201, "SamplePiStats")]
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

        [ACPropertyList(202, "SamplePiStats")]
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
        [ACPropertyInfo(203, "", "en{'Weighing statistics'}de{'Wäge-Statistiken'}")]
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
                Messages.Exception(this, e.Message, true);
            }
            return;
        }


        protected IList<SamplePiValue> _SamplePiMaxList;
        [ACPropertyList(203, "SamplePiMaxStats")]
        public IList<SamplePiValue> SamplePiMaxList
        {
            get
            {
                return _SamplePiMaxList;
            }
        }


        protected IList<SamplePiValue> _SamplePiMinList;
        [ACPropertyList(204, "SamplePiMinStats")]
        public IList<SamplePiValue> SamplePiMinList
        {
            get
            {
                return _SamplePiMinList;
            }
        }


        protected IList<SamplePiValue> _SamplePiSetPointList;
        [ACPropertyList(205, "SamplePiSetPointStats")]
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
