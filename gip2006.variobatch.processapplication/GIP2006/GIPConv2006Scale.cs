using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.ComponentModel;
using System.Runtime.Serialization;
using gip.core.processapplication;

namespace gip2006.variobatch.processapplication
{
    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'BitAccess GIPConv2006ScaleMaskRes'}de{'Bitzugriff GIPConv2006ScaleMaskRes'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(100, "Bit00", "en{'Empty(Bit00)'}de{'Empty(Bit00)'}")]
    [ACPropertyEntity(101, "Bit01", "en{'Rough(Bit01)'}de{'Rough(Bit01)'}")]
    [ACPropertyEntity(102, "Bit02", "en{'Fine(Bit02)'}de{'Fine(Bit02)'}")]
    [ACPropertyEntity(103, "Bit03", "en{'Full(Bit03)'}de{'Full(Bit03)'}")]
    [ACPropertyEntity(104, "Bit04", "en{'Flow(Bit04)'}de{'Flow(Bit04)'}")]
    [ACPropertyEntity(105, "Bit05", "en{'Dosingtime(Bit06)'}de{'Dosingtime(Bit05)'}")]
    [ACPropertyEntity(106, "Bit06", "en{'Manual(Bit06)'}de{'Manual(Bit06)'}")]
    [ACPropertyEntity(108, "Bit08", "en{'LackOfMaterial(Bit08)'}de{'LackOfMaterial(Bit08)'}")]
    [ACPropertyEntity(109, "Bit09", "en{'Presignal(Bit09)'}de{'Presignal(Bit09)'}")]
    [ACPropertyEntity(110, "Bit10", "en{'Discharging(Bit10)'}de{'Discharging(Bit10)'}")]
    [ACPropertyEntity(111, "Bit11", "en{'Dosing(Bit11)'}de{'Dosing(Bit11)'}")]
    [ACPropertyEntity(112, "Bit12", "en{'Tared(Bit12)'}de{'Tared(Bit12)'}")]
    [ACPropertyEntity(113, "Bit13", "en{'Tolerance(Bit13)'}de{'Tolerance(Bit13)'}")]
    [ACPropertyEntity(114, "Bit14", "en{'AllocatedByWay(Bit14)'}de{'AllocatedByWay(Bit14)'}")]
    [ACPropertyEntity(115, "Bit15", "en{'Fault(Bit15)'}de{'Fault(Bit15)'}")]
    public class GIPConv2006ScaleMaskRes : BitAccessForInt16
    {
        #region c'tors
        public GIPConv2006ScaleMaskRes()
        {
        }

        public GIPConv2006ScaleMaskRes(IACType acValueType)
            : base(acValueType)
        {
        }
        #endregion

        #region Customized Bits

        public bool Bit00_Empty
        {
            get { return Bit00; }
            set { Bit00 = value; }
        }

        public bool Bit01_Rough
        {
            get { return Bit01; }
            set { Bit01 = value; }
        }

        public bool Bit02_Fine
        {
            get { return Bit02; }
            set { Bit02 = value; }
        }

        public bool Bit03_Full
        {
            get { return Bit03; }
            set { Bit03 = value; }
        }

        public bool Bit04_Flow
        {
            get { return Bit04; }
            set { Bit04 = value; }
        }

        public bool Bit05_Dosingtime
        {
            get { return Bit05; }
            set { Bit05 = value; }
        }

        public bool Bit06_Manual
        {
            get { return Bit06; }
            set { Bit06 = value; }
        }

        public bool Bit08_LackOfMaterial
        {
            get { return Bit08; }
            set { Bit08 = value; }
        }

        public bool Bit09_Presignal
        {
            get { return Bit09; }
            set { Bit09 = value; }
        }

        public bool Bit10_Discharging
        {
            get { return Bit10; }
            set { Bit10 = value; }
        }

        public bool Bit11_Dosing
        {
            get { return Bit11; }
            set { Bit11 = value; }
        }

        public bool Bit12_Tared
        {
            get { return Bit12; }
            set { Bit12 = value; }
        }

        public bool Bit13_Tolerance
        {
            get { return Bit13; }
            set { Bit13 = value; }
        }

        public bool Bit14_AllocatedByWay
        {
            get { return Bit14; }
            set { Bit14 = value; }
        }

        public bool Bit15_Fault
        {
            get { return Bit15; }
            set { Bit15 = value; }
        }
        #endregion
    }

    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'BitAccess GIPConv2006ScaleMaskReq'}de{'Bitzugriff GIPConv2006ScaleMaskReq'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(106, "Bit06", "en{'ResetZero(Bit06)'}de{'ResetZero(Bit06)'}")]
    [ACPropertyEntity(107, "Bit07", "en{'SetZero(Bit07)'}de{'SetZero(Bit07)'}")]
    [ACPropertyEntity(115, "Bit15", "en{'FaultAck(Bit15)'}de{'FaultAck(Bit15)'}")]
    public class GIPConv2006ScaleMaskReq : BitAccessForInt16
    {
        #region c'tors
        public GIPConv2006ScaleMaskReq()
        {
        }

        public GIPConv2006ScaleMaskReq(IACType acValueType)
            : base(acValueType)
        {
        }
        #endregion

        #region Customized Bits

        public bool Bit06_ResetZero
        {
            get { return Bit06; }
            set { Bit06 = value; }
        }

        public bool Bit07_SetZero
        {
            get { return Bit07; }
            set { Bit07 = value; }
        }

        public bool Bit15_FaultAck
        {
            get { return Bit15; }
            set { Bit15 = value; }
        }

        #endregion
    }


    /// <summary>
    /// Baseclass for converting State and Types between Standard-Model-Components and DataAccess-/Vendor Model
    /// </summary>
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'GIPConv2006Scale'}de{'GIPConv2006Scale'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.NotStorable, false, false)]
    public class GIPConv2006Scale : PAStateConverterBase
    {
        #region c'tors
        public GIPConv2006Scale(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _TareInternal = new ACPropertyConfigValue<bool>(this, "TareInternal", false);
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            _Response.ValueUpdatedOnReceival += Response_PropertyChanged;
            _Request.ValueUpdatedOnReceival += Request_PropertyChanged;
            bool tare = _TareInternal.ValueT;
            return true;
        }

        public override bool ACPostInit()
        {
            bool result = base.ACPostInit();

            BindMyProperties();

            return result;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            _Response.ValueUpdatedOnReceival -= Response_PropertyChanged;
            _Request.ValueUpdatedOnReceival -= Request_PropertyChanged;
            return base.ACDeInit(deleteACClassTask);
        }

        protected bool _PropertiesBound = false;
        protected void BindMyProperties()
        {
            if (_PropertiesBound)
                return;
            _PropertiesBound = true;

            if (_Response.Source == null && AggrNo != null && AggrNo.ValueT > 0 && Session != null)
            {
                IACPropertyNetTarget newTarget = null;
                IACPropertyNetTarget oldTarget = _Response;
                if (BindProperty("Waagen_Stat", _Response, String.Format("W{0:000}_RM", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                {
                    OnReponsePropertyReplaced(oldTarget, newTarget);
                }
            }
            if (_Request.Source == null && AggrNo != null && AggrNo.ValueT > 0 && Session != null)
            {
                IACPropertyNetTarget newTarget = null;
                IACPropertyNetTarget oldTarget = _Request;
                if (BindProperty("Waagen_Komm", _Request, String.Format("W{0:000}_KM", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                {
                    OnRequestPropertyReplaced(oldTarget, newTarget);
                }
            }

            if (ParentACComponent != null && AggrNo != null && AggrNo.ValueT > 0 && Session != null)
            {
                PAEScaleBase controlModule = FindParentComponent<PAEScaleBase>(c => c is PAEScaleBase);
                if (controlModule != null)
                {
                    PAEScaleGravimetric scaleGravimetric = controlModule as PAEScaleGravimetric;
                    if (controlModule.DesiredWeight != null && (controlModule.DesiredWeight as IACPropertyNetTarget).Source == null)
                    {
                        IACPropertyNetTarget newTarget = null;
                        if (BindProperty("Waagen_Stat", controlModule.DesiredWeight as IACPropertyNetTarget, String.Format("W{0:000}_SOLL", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                        {
                        }
                    }
                    if (TareInternal)
                    {
                        if (scaleGravimetric != null && scaleGravimetric.ActualWeightExternal != null && (scaleGravimetric.ActualWeightExternal as IACPropertyNetTarget).Source == null)
                        {
                            IACPropertyNetTarget newTarget = null;
                            if (BindProperty("Waagen_Stat", scaleGravimetric.ActualWeightExternal as IACPropertyNetTarget, String.Format("W{0:000}_IST", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                            {
                            }
                        }
                    }
                    else
                    {
                        if (controlModule.ActualWeight != null && (controlModule.ActualWeight as IACPropertyNetTarget).Source == null)
                        {
                            IACPropertyNetTarget newTarget = null;
                            if (BindProperty("Waagen_Stat", controlModule.ActualWeight as IACPropertyNetTarget, String.Format("W{0:000}_IST", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                            {
                            }
                        }

                        if (scaleGravimetric != null && scaleGravimetric.TareCounterReq != null && (scaleGravimetric.TareCounterReq as IACPropertyNetTarget).Source == null)
                        {
                            IACPropertyNetTarget newTarget = null;
                            if (BindProperty("Waagen_Stat", scaleGravimetric.TareCounterReq as IACPropertyNetTarget, String.Format("W{0:000}_TCOUNTSOLL", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                            {
                            }
                        }

                        if (scaleGravimetric != null && scaleGravimetric.TareCounterRes != null && (scaleGravimetric.TareCounterRes as IACPropertyNetTarget).Source == null)
                        {
                            IACPropertyNetTarget newTarget = null;
                            if (BindProperty("Waagen_Stat", scaleGravimetric.TareCounterRes as IACPropertyNetTarget, String.Format("W{0:000}_TCOUNTIST", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                            {
                            }
                        }
                    }
                    if (controlModule.ActualValue != null && (controlModule.ActualValue as IACPropertyNetTarget).Source == null)
                    {
                        IACPropertyNetTarget newTarget = null;
                        if (BindProperty("Waagen_Stat", controlModule.ActualValue as IACPropertyNetTarget, String.Format("W{0:000}_BRUTTO", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                        {
                        }
                    }

                    PAEScaleTotalizing totalizingScale = controlModule as PAEScaleTotalizing;
                    if (totalizingScale != null)
                    {
                        if (totalizingScale.TotalActualWeight != null && (totalizingScale.TotalActualWeight as IACPropertyNetTarget).Source == null)
                        {
                            IACPropertyNetTarget newTarget = null;
                            if (BindProperty("Waagen_Stat", totalizingScale.TotalActualWeight as IACPropertyNetTarget, String.Format("W{0:000}_GESIST", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                            {
                            }
                        }
                        if (totalizingScale.TotalDesiredWeight != null && (totalizingScale.TotalDesiredWeight as IACPropertyNetTarget).Source == null)
                        {
                            IACPropertyNetTarget newTarget = null;
                            if (BindProperty("Waagen_Stat", totalizingScale.TotalDesiredWeight as IACPropertyNetTarget, String.Format("W{0:000}_GESSOLL", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                            {
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Properties

        #region Configuration

        private ACPropertyConfigValue<bool> _TareInternal;
        [ACPropertyConfig("en{'Tare internal'}de{'Internes Tarieren'}")]
        public bool TareInternal
        {
            get
            {
                PAEScaleGravimetric gravimetricScale = ParentACComponent as PAEScaleGravimetric;
                if (gravimetricScale != null && gravimetricScale.TareInternal)
                    return gravimetricScale.TareInternal;
                return _TareInternal.ValueT;
            }
            set
            {
                _TareInternal.ValueT = value;
            }
        }

        [ACPropertyBindingSource(9999, "", "en{'Aggregate number'}de{'Aggregatnummer'}", "", false, false)]
        public IACContainerTNet<Int16> AggrNo { get; set; }

        #endregion


        #region Read-Values from PLC
        public IACContainerTNet<Byte> AllocatedByWay { get; set; }

        public IACContainerTNet<PANotifyState> StateUL1 { get; set; }
        public IACContainerTNet<PANotifyState> StateUL2 { get; set; }
        public IACContainerTNet<PANotifyState> StateLL1 { get; set; }
        public IACContainerTNet<PANotifyState> StateLL2 { get; set; }

        public IACContainerTNet<PANotifyState> StateScale { get; set; }
        public IACContainerTNet<PANotifyState> StateTolerance { get; set; }
        public IACContainerTNet<PANotifyState> StateLackOfMaterial { get; set; }
        public IACContainerTNet<PANotifyState> StateDosingTime { get; set; }
        public IACContainerTNet<Boolean> IsDosing { get; set; }
        public IACContainerTNet<Boolean> IsRough { get; set; }
        public IACContainerTNet<Boolean> IsFine { get; set; }

        public IACContainerTNet<Boolean> IsDischarging { get; set; }
        public IACContainerTNet<Boolean> IsTared { get; set; }
        public IACContainerTNet<Boolean> ZeroSet { get; set; }
        public IACContainerTNet<Boolean> IsManualDosing { get; set; }
        public IACContainerTNet<Int32> TareCounterRes { get; set; }

        public IACContainerTNet<Boolean> IsPresignal { get; set; }
        public IACContainerTNet<Boolean> IsFlow { get; set; }


        [ACPropertyBindingTarget(240, "Read from PLC", "en{'Response'}de{'Rückmeldung'}", "", false, false)]
        public IACContainerTNet<GIPConv2006ScaleMaskRes> Response { get; set; }
        protected virtual IACPropertyNetTarget _Response
        {
            get
            {
                return (IACPropertyNetTarget)this.Response;
            }
        }
        protected virtual void OnReponsePropertyReplaced(IACPropertyNetTarget oldTarget, IACPropertyNetTarget newTarget)
        {
            if (oldTarget != null)
                oldTarget.ValueUpdatedOnReceival -= Response_PropertyChanged;
            if (newTarget != null)
                newTarget.ValueUpdatedOnReceival += Response_PropertyChanged;
        }
        protected virtual GIPConv2006ScaleMaskRes _ResponseValue
        {
            get { return Response.ValueT; }
        }
        #endregion

        #region Write-Values to PLC
        public IACContainerTNet<bool> FaultAckUL1 { get; set; }
        public IACContainerTNet<bool> FaultAckUL2 { get; set; }
        public IACContainerTNet<bool> FaultAckLL1 { get; set; }
        public IACContainerTNet<bool> FaultAckLL2 { get; set; }

        public IACContainerTNet<bool> FaultAckScale { get; set; }
        public IACContainerTNet<bool> FaultAckTolerance { get; set; }
        public IACContainerTNet<bool> FaultAckLackOfMaterial { get; set; }
        public IACContainerTNet<bool> FaultAckDosingTime { get; set; }

        public IACContainerTNet<Boolean> ReqZeroSet { get; set; }
        public IACContainerTNet<Boolean> ReqZeroReset { get; set; }
        public IACContainerTNet<Boolean> ReqIsTared { get; set; }
        public IACContainerTNet<Int32> TareCounterReq { get; set; }


        [ACPropertyBindingTarget(250, "Read from PLC", "en{'Request'}de{'Kommando'}", "", false, false)]
        public IACContainerTNet<GIPConv2006ScaleMaskReq> Request { get; set; }

        protected virtual IACPropertyNetTarget _Request
        {
            get
            {
                return (IACPropertyNetTarget)Request;
            }
        }
        protected virtual void OnRequestPropertyReplaced(IACPropertyNetTarget oldTarget, IACPropertyNetTarget newTarget)
        {
            if (oldTarget != null)
                oldTarget.ValueUpdatedOnReceival -= Request_PropertyChanged;
            if (newTarget != null)
                newTarget.ValueUpdatedOnReceival += Request_PropertyChanged;
        }

        protected virtual GIPConv2006ScaleMaskReq _RequestValue
        {
            get { return Request.ValueT; }
        }
        #endregion

        #endregion

        #region overridden methods
        protected override bool OnParentServerPropertyFound(IACPropertyNetServer parentProperty)
        {
            switch (parentProperty.ACIdentifier)
            {
                case "AllocatedByWay":
                    AllocatedByWay = parentProperty as IACContainerTNet<Byte>;
                    return true;
                case "StateUL1":
                    StateUL1 = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                case "StateUL2":
                    StateUL2 = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                case "StateLL1":
                    StateLL1 = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                case "StateLL2":
                    StateLL2 = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                case "StateScale":
                    StateScale = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                case "StateTolerance":
                    StateTolerance = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                case "StateLackOfMaterial":
                    StateLackOfMaterial = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                case "StateDosingTime":
                    StateDosingTime = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                case "IsDosing":
                    IsDosing = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "IsRough":
                    IsRough = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "IsFine":
                    IsFine = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "IsDischarging":
                    IsDischarging = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "IsTared":
                    IsTared = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "IsManualDosing":
                    IsManualDosing = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "ZeroSet":
                    ZeroSet = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "IsPresignal":
                    IsPresignal = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "IsFlow":
                    IsFlow = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "TareCounterRes":
                    TareCounterRes = parentProperty as IACContainerTNet<Int32>;
                    return true;
                case "FaultAckUL1":
                    FaultAckUL1 = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "FaultAckUL2":
                    FaultAckUL2 = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "FaultAckLL1":
                    FaultAckLL1 = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "FaultAckLL2":
                    FaultAckLL2 = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "FaultAckScale":
                    FaultAckScale = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "FaultAckTolerance":
                    FaultAckTolerance = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "FaultAckLackOfMaterial":
                    FaultAckLackOfMaterial = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "FaultAckDosingTime":
                    FaultAckDosingTime = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "ReqZeroSet":
                    ReqZeroSet = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "ReqZeroReset":
                    ReqZeroReset = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "ReqIsTared":
                    ReqIsTared = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "TareCounterReq":
                    TareCounterReq = parentProperty as IACContainerTNet<Int32>;
                    parentProperty.ForceBroadcast = true;
                    return true;
            }
            return false;
        }

        protected override void ModelProperty_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if (!_LockResend_ReqRunState)
            {
                if (sender == FaultAckScale)
                {
                    _RequestValue.Bit15_FaultAck = FaultAckScale.ValueT;
                    return;
                }
                else if (sender == ReqZeroSet)
                {
                    _RequestValue.Bit07_SetZero = ReqZeroSet.ValueT;
                }
                else if (sender == ReqZeroReset)
                {
                    _RequestValue.Bit06_ResetZero = ReqZeroReset.ValueT;
                }
                else if (sender == ReqIsTared && !TareInternal)
                {
                    // This bit currently doesn't exist in GIP-PLC
                    //_RequestValue.Bit05_ReqIsTared = ReqIsTared.ValueT;
                }
            }
        }
        #endregion

        #region Converter-Logic
        protected virtual void Response_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                if (AllocatedByWay != null) AllocatedByWay.ValueT = System.Convert.ToByte(_ResponseValue.Bit14_AllocatedByWay);
                if (StateUL2 != null)
                {
                    if (_ResponseValue.Bit03_Full)
                        StateUL2.ValueT = PANotifyState.AlarmOrFault;
                    else
                        StateUL2.ValueT = PANotifyState.Off;
                }
                if (StateLL2 != null)
                {
                    if (_ResponseValue.Bit00_Empty)
                        StateLL2.ValueT = PANotifyState.InfoOrActive;
                    else
                        StateLL2.ValueT = PANotifyState.Off;
                }
                if (StateScale != null)
                {
                    if (_ResponseValue.Bit15_Fault)
                        StateScale.ValueT = PANotifyState.AlarmOrFault;
                    else
                        StateScale.ValueT = PANotifyState.Off;
                }
                if (StateTolerance != null)
                {
                    if (_ResponseValue.Bit13_Tolerance)
                        StateTolerance.ValueT = PANotifyState.AlarmOrFault;
                    else
                        StateTolerance.ValueT = PANotifyState.Off;
                }
                if (StateLackOfMaterial != null)
                {
                    if (_ResponseValue.Bit08_LackOfMaterial)
                        StateLackOfMaterial.ValueT = PANotifyState.AlarmOrFault;
                    else
                        StateLackOfMaterial.ValueT = PANotifyState.Off;
                }
                if (StateDosingTime != null)
                {
                    if (_ResponseValue.Bit05_Dosingtime)
                        StateDosingTime.ValueT = PANotifyState.AlarmOrFault;
                    else
                        StateDosingTime.ValueT = PANotifyState.Off;
                }
                if (IsDosing != null) IsDosing.ValueT = _ResponseValue.Bit11_Dosing;
                if (IsRough != null) IsRough.ValueT = _ResponseValue.Bit01_Rough;
                if (IsFine != null) IsFine.ValueT = _ResponseValue.Bit02_Fine;

                if (IsDischarging != null) IsDischarging.ValueT = _ResponseValue.Bit10_Discharging;
                if (IsTared != null && !TareInternal) IsTared.ValueT = _ResponseValue.Bit12_Tared;
                if (IsManualDosing != null) IsManualDosing.ValueT = _ResponseValue.Bit06_Manual;
                if (IsPresignal != null) IsPresignal.ValueT = _ResponseValue.Bit09_Presignal;
                if (IsFlow != null) IsFlow.ValueT = _ResponseValue.Bit04_Flow;
            }
        }

        private bool _LockResend_ReqRunState = false;
        protected virtual void Request_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                _LockResend_ReqRunState = true;
                if (ReqZeroSet != null && ReqZeroReset != null && ZeroSet != null)
                { 
                    ReqZeroSet.ValueT = _RequestValue.Bit07_SetZero;
                    ReqZeroReset.ValueT = _RequestValue.Bit06_ResetZero;
                    if (ReqZeroSet.ValueT && !ReqZeroReset.ValueT)
                        ZeroSet.ValueT = true;
                    else
                        ZeroSet.ValueT = false;
                    // This bit currently doesn't exist in GIP-PLC
                    //ReqIsTared.ValueT = _RequestValue.Bit05_ReqIsTared;
                }
                _LockResend_ReqRunState = false;
            }
        }
        #endregion
    }
}
