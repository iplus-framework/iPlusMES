// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.ComponentModel;
using System.Runtime.Serialization;
using gip.core.processapplication;

namespace gip2006.variobatch.processapplication
{
    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'BitAccess GIPConv2006ConfigMUX'}de{'BitzugriffGIPConv2006ConfigMUX'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(108, "Bit08", "en{'Save(Bit08)'}de{'Save(Bit08)'}")]
    [ACPropertyEntity(109, "Bit09", "en{'Reset(Bit09)'}de{'Reset(Bit09)'}")]
    [ACPropertyEntity(111, "Bit11", "en{'Read(Bit11)'}de{'Read(Bit11)'}")]
    [ACPropertyEntity(112, "Bit12", "en{'Default(Bit12)'}de{'Default(Bit12)'}")]
    public class GIPConv2006MUXCmd : BitAccessForInt16
    {
        #region c'tors
        public GIPConv2006MUXCmd()
        {
        }

        public GIPConv2006MUXCmd(IACType acValueType)
            : base(acValueType)
        {
        }

        public bool Bit08_Save
        {
            get { return Bit08; }
            set { Bit08 = value; }
        }

        public bool Bit09_Reset
        {
            get { return Bit09; }
            set { Bit09 = value; }
        }

        public bool Bit11_Read
        {
            get { return Bit11; }
            set { Bit11 = value; }
        }

        public bool Bit12_Default
        {
            get { return Bit12; }
            set { Bit12 = value; }
        }

        #endregion
    }

    public interface IGIPConvComp4MUX : IACComponent
    {
        IACContainerTNet<Int16> AggrNo { get; set; }

        void SelectControlModule();
        void ResetStatistic();
    }

    /// <summary>
    /// Baseclass for multiplexing Configuration-Parameter
    /// </summary>
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'GIPConv2006ConfigMUX'}de{'GIPConv2006ConfigMUX'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.NotStorable, false, true)]
    public class GIPConv2006ConfigMUX : ACComponent
    {
        #region c'tors
        public GIPConv2006ConfigMUX(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _ReadStatistics = new ACPropertyConfigValue<bool>(this, "ReadStatistics", false);
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            if (TurnOnDelayPLC != null)
                (TurnOnDelayPLC as IACPropertyNetServer).ValueUpdatedOnReceival += PLCProperty_ValueUpdatedOnReceival;
            if (TurnOffDelayPLC != null)
                (TurnOffDelayPLC as IACPropertyNetServer).ValueUpdatedOnReceival += PLCProperty_ValueUpdatedOnReceival;
            if (FaultDelayTimePLC != null)
                (FaultDelayTimePLC as IACPropertyNetServer).ValueUpdatedOnReceival += PLCProperty_ValueUpdatedOnReceival;
            if (FaultDelayTimeAllPLC != null)
                (FaultDelayTimeAllPLC as IACPropertyNetServer).ValueUpdatedOnReceival += PLCProperty_ValueUpdatedOnReceival;
            if (MUXCommandPLC != null)
                (MUXCommandPLC as IACPropertyNetServer).ValueUpdatedOnReceival += PLCProperty_ValueUpdatedOnReceival;
            if (OperatingTimePLC != null)
                (OperatingTimePLC as IACPropertyNetServer).ValueUpdatedOnReceival += PLCProperty_ValueUpdatedOnReceival;
            if (SwitchingFrequencyPLC != null)
                (SwitchingFrequencyPLC as IACPropertyNetServer).ValueUpdatedOnReceival += PLCProperty_ValueUpdatedOnReceival;
            if (TotalAlarmsPLC != null)
                (TotalAlarmsPLC as IACPropertyNetServer).ValueUpdatedOnReceival += PLCProperty_ValueUpdatedOnReceival;
            if (OnTimePLC != null)
                (OnTimePLC as IACPropertyNetServer).ValueUpdatedOnReceival += PLCProperty_ValueUpdatedOnReceival;

            _ = ReadStatistics;
            return true;
        }

        public override bool ACPostInit()
        {
            return base.ACPostInit();
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            if (TurnOnDelayPLC != null)
                (TurnOnDelayPLC as IACPropertyNetServer).ValueUpdatedOnReceival -= PLCProperty_ValueUpdatedOnReceival;
            if (TurnOffDelayPLC != null)
                (TurnOffDelayPLC as IACPropertyNetServer).ValueUpdatedOnReceival -= PLCProperty_ValueUpdatedOnReceival;
            if (FaultDelayTimePLC != null)
                (FaultDelayTimePLC as IACPropertyNetServer).ValueUpdatedOnReceival -= PLCProperty_ValueUpdatedOnReceival;
            if (FaultDelayTimeAllPLC != null)
                (FaultDelayTimeAllPLC as IACPropertyNetServer).ValueUpdatedOnReceival -= PLCProperty_ValueUpdatedOnReceival;
            if (MUXCommandPLC != null)
                (MUXCommandPLC as IACPropertyNetServer).ValueUpdatedOnReceival -= PLCProperty_ValueUpdatedOnReceival;
            if (OperatingTimePLC != null)
                (OperatingTimePLC as IACPropertyNetServer).ValueUpdatedOnReceival -= PLCProperty_ValueUpdatedOnReceival;
            if (SwitchingFrequencyPLC != null)
                (SwitchingFrequencyPLC as IACPropertyNetServer).ValueUpdatedOnReceival -= PLCProperty_ValueUpdatedOnReceival;
            if (TotalAlarmsPLC != null)
                (TotalAlarmsPLC as IACPropertyNetServer).ValueUpdatedOnReceival -= PLCProperty_ValueUpdatedOnReceival;
            if (OnTimePLC != null)
                (OnTimePLC as IACPropertyNetServer).ValueUpdatedOnReceival -= PLCProperty_ValueUpdatedOnReceival;

            if (_SelectedModule != null)
            {
                _SelectedModule.ObjectAttached -= _SelectedModule_ObjectAttached;
                _SelectedModule.ObjectDetaching -= _SelectedModule_ObjectDetaching;
                _SelectedModule.Detach();
                _SelectedModule = null;
            }

            return await base.ACDeInit(deleteACClassTask);
        }

        private ACPropertyConfigValue<bool> _ReadStatistics;
        [ACPropertyConfig("ReadStatistics")]
        public bool ReadStatistics
        {
            get
            {
                return _ReadStatistics.ValueT;
            }
            set
            {
                _ReadStatistics.ValueT = value;
            }
        }

        #endregion

        #region Properties

        #region Internal
        private ACRef<ACComponent> _SelectedModule = null;
        public ACComponent SelectedModule
        {
            get
            {
                if (_SelectedModule == null)
                    return null;
                return _SelectedModule.ValueT;
            }
            set
            {
                bool objectSwapped = true;
                if (_SelectedModule != null)
                {
                    if (_SelectedModule.ValueT != value)
                    {
                        _SelectedModule.Detach();
                        _SelectedModule = null;
                        _CurrentAggNo = 0;
                    }
                    else
                        objectSwapped = false;
                }
                if ((value != null) && objectSwapped)
                {
                    _SelectedModule = new ACRef<ACComponent>(value, this);
                    _SelectedModule.ObjectAttached += _SelectedModule_ObjectAttached;
                    _SelectedModule.ObjectDetaching += _SelectedModule_ObjectDetaching;
                }
                if ((_SelectedModule != null) && objectSwapped)
                {
                    FindAndMapPropertiesForMUX();
                }
                OnPropertyChanged("SelectedModule");
            }
        }

        private Int16 _CurrentAggNo = 0;
        #endregion

        #region Mapped Properties to Multiplex
        public IACContainerTNet<TimeSpan> TurnOnDelay { get; set; }
        private bool _LockResend_TurnOnDelay = false;
        public IACContainerTNet<TimeSpan> TurnOffDelay { get; set; }
        private bool _LockResend_TurnOffDelay = false;
        public IACContainerTNet<TimeSpan> FaultDelayTime { get; set; }
        private bool _LockResend_FaultDelayTime = false;
        public IACContainerTNet<TimeSpan> FaultDelayTimeAll { get; set; }
        //private bool _LockResend_FaultDelayTimeAll = false;

        public IACContainerTNet<TimeSpan> OperatingTime { get; set; }
        public IACContainerTNet<Int32> SwitchingFrequency { get; set; }
        public IACContainerTNet<Int32> TotalAlarms { get; set; }
        public IACContainerTNet<DateTime> TurnOnInstant { get; set; }
        public IACContainerTNet<DateTime> TurnOffInstant { get; set; }
        public IACContainerTNet<TimeSpan> OnTime { get; set; }

        #endregion

        #region Binding Properties
        [ACPropertyBindingTarget(240, "Read from PLC", "en{'TurnOnDelayPLC'}de{'TurnOnDelayPLC'}", "", false, false)]
        public IACContainerTNet<Int16> TurnOnDelayPLC { get; set; }

        private void UpdateTurnOnDelay()
        {
            if (TurnOnDelay != null)
            {
                _LockResend_TurnOnDelay = true;
                TurnOnDelay.ValueT = TimeSpan.FromSeconds(TurnOnDelayPLC.ValueT * 0.1);
                //TurnOnDelay.ValueT = new TimeSpan(0, 0, TurnOnDelayPLC.ValueT);
                _LockResend_TurnOnDelay = false;
            }
        }


        [ACPropertyBindingTarget(241, "Read from PLC", "en{'TurnOffDelayPLC'}de{'TurnOffDelayPLC'}", "", false, false)]
        public IACContainerTNet<Int16> TurnOffDelayPLC { get; set; }

        private void UpdateTurnOffDelay()
        {
            if (TurnOffDelay != null)
            {
                _LockResend_TurnOffDelay = true;
                try
                {
                    // Conversion from 1/10 seconds
                    TurnOffDelay.ValueT = TimeSpan.FromSeconds(TurnOffDelayPLC.ValueT * 0.1);
                    //TurnOffDelay.ValueT = new TimeSpan(0, 0, TurnOffDelayPLC.ValueT);
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    if (e.InnerException != null && e.InnerException.Message != null)
                        msg += " Inner:" + e.InnerException.Message;

                    Messages.LogException("GIPConv2006MUX", "UpdateTurnOffDelay", msg);
                }
                _LockResend_TurnOffDelay = false;
            }
        }

        [ACPropertyBindingTarget(242, "Read from PLC", "en{'FaultDelayTimePLC'}de{'Störzeit Aggregat'}", "", false, false)]
        public IACContainerTNet<Int16> FaultDelayTimePLC { get; set; }

        private void UpdateFaultDelayTime()
        {
            if (FaultDelayTime != null)
            {
                _LockResend_FaultDelayTime = true;
                try
                {
                    // Conversion from 1/10 seconds
                    FaultDelayTime.ValueT = new TimeSpan(0, 0, FaultDelayTimePLC.ValueT);
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    if (e.InnerException != null && e.InnerException.Message != null)
                        msg += " Inner:" + e.InnerException.Message;

                    Messages.LogException("GIPConv2006MUX", "UpdateFaultDelayTime", msg);
                }
                _LockResend_FaultDelayTime = false;
            }
        }

        [ACPropertyBindingTarget(243, "Read from PLC", "en{'FaultDelayTimeGeneralPLC'}de{'Störzeit Allgemein'}", "", false, false)]
        public IACContainerTNet<Int16> FaultDelayTimeAllPLC { get; set; }

        private void UpdateFaultDelayTimeAll()
        {
            if (FaultDelayTimeAll != null)
            {
                //_LockResend_FaultDelayTimeAll = true;
                FaultDelayTimeAll.ValueT = new TimeSpan(0, 0, FaultDelayTimeAllPLC.ValueT);
                //_LockResend_FaultDelayTimeAll = false;
            }
        }

        [ACPropertyBindingTarget(244, "Read from PLC", "en{'AggrNoPLC'}de{'Aggregatnummer'}", "", false, false)]
        public IACContainerTNet<Int16> AggrNoPLC { get; set; }

        [ACPropertyBindingTarget(240, "Read from PLC", "en{'MUX-Command (AGG_DIAG'}de{'MUX-Command (AGG_DIAG)'}", "", false, false)]
        public IACContainerTNet<GIPConv2006MUXCmd> MUXCommandPLC { get; set; }

        [ACPropertyBindingTarget(245, "Statistics", "en{'Operating time'}de{'Betriebsdauer'}", "", false, false)]
        public IACContainerTNet<Int32> OperatingTimePLC { get; set; }

        private void UpdateOperatingTime()
        {
            if (OperatingTime != null && ReadStatistics)
            {
                OperatingTime.ValueT = new TimeSpan(OperatingTimePLC.ValueT, 0, 0);
            }
        }


        [ACPropertyBindingTarget(246, "Statistics", "en{'Switching frequency'}de{'Schalthäufigkeit'}", "", false, false)]
        public IACContainerTNet<Int32> SwitchingFrequencyPLC { get; set; }

        private void UpdateSwitchingFrequency()
        {
            if (SwitchingFrequency != null && ReadStatistics)
            {
                SwitchingFrequency.ValueT = SwitchingFrequencyPLC.ValueT;
            }
        }

        [ACPropertyBindingTarget(247, "Statistics", "en{'Total alarms'}de{'Anzahl Störungen'}", "", false, false)]
        public IACContainerTNet<Int32> TotalAlarmsPLC { get; set; }

        private void UpdateTotalAlarms()
        {
            if (TotalAlarms != null && ReadStatistics)
            {
                TotalAlarms.ValueT = TotalAlarmsPLC.ValueT;
            }
        }

        [ACPropertyBindingTarget(248, "Statistics", "en{'on-time'}de{'Einschaltdauer'}", "", false, false)]
        public IACContainerTNet<Int32> OnTimePLC { get; set; }

        private void UpdateOnTime()
        {
            if (OnTime != null && ReadStatistics)
            {
                OnTime.ValueT = new TimeSpan(0, 0, OnTimePLC.ValueT);
            }
        }

        protected virtual void PLCProperty_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                switch (e.PropertyName)
                {
                    case "TurnOnDelayPLC":
                        UpdateTurnOnDelay();
                        break;
                    case "TurnOffDelayPLC":
                        UpdateTurnOffDelay();
                        break;
                    case "FaultDelayTimePLC":
                        UpdateFaultDelayTime();
                        break;
                    case "FaultDelayTimeAllPLC":
                        UpdateFaultDelayTimeAll();
                        break;
                    case "OperatingTimePLC":
                        UpdateOperatingTime();
                        break;
                    case "SwitchingFrequencyPLC":
                        UpdateSwitchingFrequency();
                        break;
                    case "TotalAlarmsPLC":
                        UpdateTotalAlarms();
                        break;
                    case "OnTimePLC":
                        UpdateOnTime();
                        break;
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Private
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "Reset":
                    Reset();
                    return true;
                case "SelectControlModule":
                    SelectControlModule(acParameter[0] as string);
                    return true;
                case Const.IsEnabledPrefix + "Reset":
                    result = IsEnabledReset();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        private void _SelectedModule_ObjectAttached(object sender, EventArgs e)
        {
            FindAndMapPropertiesForMUX();
        }

        private void _SelectedModule_ObjectDetaching(object sender, EventArgs e)
        {
            UnMapPropertiesForMUX();
        }

        private void FindAndMapPropertiesForMUX()
        {
            if (SelectedModule == null)
            {
                _CurrentAggNo = 0;
                return;
            }
            IGIPConvComp4MUX converter = SelectedModule.FindChildComponents<IGIPConvComp4MUX>(c => c is IGIPConvComp4MUX, null, 1).FirstOrDefault();
            if (converter == null)
            {
                _CurrentAggNo = 0;
                return;
            }
            _CurrentAggNo = converter.AggrNo.ValueT;
            if (_CurrentAggNo <= 0)
                return;

            IACPropertyNetServer[] query = null;

            using (ACMonitor.Lock(LockMemberList_20020))
            {
                query = SelectedModule.ACMemberList.Where(c => c is IACPropertyNetServer).Select(c => c as IACPropertyNetServer).ToArray();
            }
            if (query != null && query.Any())
            {
                foreach (IACPropertyNetServer targetProperty in query)
                {
                    if (OnParentTargetPropertyFound(targetProperty))
                        targetProperty.ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
                }
            }
            AggrNoPLC.ValueT = _CurrentAggNo;
            MUXCommandPLC.ValueT.Bit11_Read = true; // Lesebefehl: Übetrage werte aus InstanzDB -> DB830
        }

        private void UnMapPropertiesForMUX()
        {
            if (SelectedModule == null)
                return;
            if (_CurrentAggNo <= 0)
                return;
            IACPropertyNetTarget[] query = null;

            using (ACMonitor.Lock(LockMemberList_20020))
            {
                query = SelectedModule.ACMemberList.Where(c => c is IACPropertyNetTarget).Select(c => c as IACPropertyNetTarget).ToArray();
            }
            if (query != null && query.Any())
            {
                foreach (IACPropertyNetTarget targetProperty in query)
                {
                    targetProperty.ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
                }
            }

            TurnOnDelay = null;
            TurnOffDelay = null;
            FaultDelayTime = null;
            FaultDelayTimeAll = null;

            OperatingTime = null;
            SwitchingFrequency = null;
            TotalAlarms = null;
            TurnOnInstant = null;
            TurnOffInstant = null;
            OnTime = null;
        }
#endregion

#region Public
        [ACMethodInfo("", "en{'Select control module to multiplex'}de{'Auswahl des zu multiplexenden Control-Modul'}", 201, false, Global.ACKinds.MSMethodPrePost)]
        public void SelectControlModule(string acURL)
        {
            if (!PreExecute("SelectControlModule"))
                return;
            if (String.IsNullOrEmpty(acURL))
            {
                SelectedModule = null;
            }
            else
            {
                SelectedModule = Root.ACUrlCommand(acURL) as ACComponent;
            }
            PostExecute("SelectControlModule");
        }

        [ACMethodInteraction("", "en{'Reset'}de{'Reset'}", 200, true, "", Global.ACKinds.MSMethodPrePost)]
        public virtual void Reset()
        {
            if (!IsEnabledReset())
                return;
            if (!PreExecute("Reset"))
                return;
            MUXCommandPLC.ValueT.Bit09_Reset = true;
            PostExecute("Reset");
        }

        public virtual bool IsEnabledReset()
        {
            if ((SelectedModule == null) || (_CurrentAggNo <= 0))
                return false;
            return true;
        }

#endregion

#region Protected
        protected virtual bool OnParentTargetPropertyFound(IACPropertyNetServer parentProperty)
        {
            switch (parentProperty.ACIdentifier)
            {
                case "TurnOnDelay":
                case "DelayTimeSignalOn":
                    TurnOnDelay = parentProperty as IACContainerTNet<TimeSpan>;
                    UpdateTurnOnDelay();
                    return true;
                case "TurnOffDelay":
                case "DelayTimeSignalOff":
                    TurnOffDelay = parentProperty as IACContainerTNet<TimeSpan>;
                    UpdateTurnOffDelay();
                    return true;
                case "FaultDelayTime":
                    FaultDelayTime = parentProperty as IACContainerTNet<TimeSpan>;
                    UpdateFaultDelayTime();
                    return true;
                case "FaultDelayTimeAll":
                    FaultDelayTimeAll = parentProperty as IACContainerTNet<TimeSpan>;
                    UpdateFaultDelayTimeAll();
                    return true;
                case "OperatingTime":
                    OperatingTime = parentProperty as IACContainerTNet<TimeSpan>;
                    UpdateOperatingTime();
                    return true;
                case "SwitchingFrequency":
                    SwitchingFrequency = parentProperty as IACContainerTNet<Int32>;
                    UpdateSwitchingFrequency();
                    return true;
                case "TotalAlarms":
                    TotalAlarms = parentProperty as IACContainerTNet<Int32>;
                    UpdateTotalAlarms();
                    return true;
                case "TurnOnInstant":
                    TurnOnInstant = parentProperty as IACContainerTNet<DateTime>;
                    return true;
                case "TurnOffInstant":
                    TurnOffInstant = parentProperty as IACContainerTNet<DateTime>;
                    return true;
                case "OnTime":
                    OnTime = parentProperty as IACContainerTNet<TimeSpan>;
                    UpdateOnTime();
                    return true;
            }
            return false;
        }

        protected virtual void ModelProperty_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if (_SelectedModule == null)
                return;
            try
            {
                if (sender == TurnOnDelay)
                {
                    if (!_LockResend_TurnOnDelay)
                    {
                        try
                        {
                            // Conversion to 1/10 seconds
                            TurnOnDelayPLC.ValueT = System.Convert.ToInt16(TurnOnDelay.ValueT.TotalMilliseconds * 0.01);
                            MUXCommandPLC.ValueT.Bit08_Save = true;
                        }
                        catch (Exception ec)
                        {
                            string msg = ec.Message;
                            if (ec.InnerException != null && ec.InnerException.Message != null)
                                msg += " Inner:" + ec.InnerException.Message;

                            Messages.LogException("GIPConv2006MUX", "ModelProperty_ValueUpdatedOnReceival", msg);
                        }
                    }
                    return;
                }
                else if (sender == TurnOffDelay)
                {
                    if (!_LockResend_TurnOffDelay)
                    {
                        try
                        {
                            // Conversion to 1/10 seconds
                            TurnOffDelayPLC.ValueT = System.Convert.ToInt16(TurnOffDelay.ValueT.TotalMilliseconds * 0.01);
                            MUXCommandPLC.ValueT.Bit08_Save = true;
                        }
                        catch (Exception ec)
                        {
                            string msg = ec.Message;
                            if (ec.InnerException != null && ec.InnerException.Message != null)
                                msg += " Inner:" + ec.InnerException.Message;

                            Messages.LogException("GIPConv2006MUX", "ModelProperty_ValueUpdatedOnReceival(10)", msg);
                        }
                    }
                    return;
                }
                else if (sender == FaultDelayTime)
                {
                    if (!_LockResend_FaultDelayTime)
                    {
                        FaultDelayTimePLC.ValueT = System.Convert.ToInt16(FaultDelayTime.ValueT.TotalSeconds);
                        MUXCommandPLC.ValueT.Bit12_Default = true;
                        MUXCommandPLC.ValueT.Bit08_Save = true;
                    }
                    return;
                }
                else if (sender == FaultDelayTimeAll)
                {
                    if (!_LockResend_TurnOffDelay)
                    {
                        FaultDelayTimeAllPLC.ValueT = System.Convert.ToInt16(FaultDelayTimeAll.ValueT.TotalSeconds);
                        MUXCommandPLC.ValueT.Bit08_Save = true;
                    }
                    return;
                }
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("GIPConv2006MUX", "ModelProperty_ValueUpdatedOnReceival(20)", msg);
            }
            return;
        }
#endregion
#endregion
    }
}
