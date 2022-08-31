using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.core.autocomponent;
using gip.core.manager;
using System.Collections.ObjectModel;
using gip.mes.facility;
using gip.mes.processapplication;
using System.Data.Objects;
using gip.bso.masterdata;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Laboratory Order MES'}de{'Laborauftrag MES'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + LabOrder.ClassName)]
    public class BSOLabOrderMES : BSOLabOrder
    {
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
                if (   (_PiStats.TolPlus <= double.Epsilon || _PiStats.TolMinus <= double.Epsilon || _PiStats.SetPoint <= double.Epsilon)
                    && (CurrentLabOrderPos.ReferenceValue.HasValue && CurrentLabOrderPos.ValueMax.HasValue && CurrentLabOrderPos.ValueMin.HasValue))
                    _PiStats.SetToleranceAndRecalc(CurrentLabOrderPos.ReferenceValue.Value, CurrentLabOrderPos.ValueMax.Value - CurrentLabOrderPos.ReferenceValue.Value, CurrentLabOrderPos.ReferenceValue.Value - CurrentLabOrderPos.ValueMin.Value, true);
                if (_PiStats != null && _PiStats.Values.Any())
                {
                    _SamplePiMaxList = _PiStats.Values.Select(c => new SamplePiValue() { DTStamp = c.DTStamp, Value = CurrentLabOrderPos.ValueMax.HasValue ? CurrentLabOrderPos.ValueMax.Value : 0.0 }).ToArray();
                    _SamplePiMinList = _PiStats.Values.Select(c => new SamplePiValue() { DTStamp = c.DTStamp, Value = CurrentLabOrderPos.ValueMin.HasValue ? CurrentLabOrderPos.ValueMin.Value : 0.0 }).ToArray();
                    _SamplePiSetPointList = _PiStats.Values.Select(c => new SamplePiValue() { DTStamp = c.DTStamp, Value = CurrentLabOrderPos.ReferenceValue.HasValue ? CurrentLabOrderPos.ReferenceValue.Value : 0.0 }).ToArray();
                }
            }
            catch (Exception e)
            {
                Messages.Exception(this, e.Message, true);
            }
            return;
        }


        IList<SamplePiValue> _SamplePiMaxList;
        [ACPropertyList(203, "SamplePiMaxStats")]
        public IList<SamplePiValue> SamplePiMaxList
        {
            get
            {
                return _SamplePiMaxList;
            }
        }


        IList<SamplePiValue> _SamplePiMinList;
        [ACPropertyList(204, "SamplePiMinStats")]
        public IList<SamplePiValue> SamplePiMinList
        {
            get
            {
                return _SamplePiMinList;
            }
        }


        IList<SamplePiValue> _SamplePiSetPointList;
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
