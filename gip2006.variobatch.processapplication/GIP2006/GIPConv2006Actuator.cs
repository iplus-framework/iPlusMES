using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using gip.mes.processapplication;
using gip.core.processapplication;

namespace gip2006.variobatch.processapplication
{
    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'BitAccess GIPConv2006ActuatorMaskRes'}de{'Bitzugriff GIPConv2006ActuatorMaskRes'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(100, "Bit00", "en{'Closed, Position 1 (Bit00/3.0)'}de{'Zu, Position 1  (Bit00/3.0)'}")]
    [ACPropertyEntity(101, "Bit01", "en{'Triggered (Bit01/3.1)'}de{'Angesteuert (Bit01/3.1)'}")]
    [ACPropertyEntity(102, "Bit02", "en{'Open, Position 2 (Bit02/3.2)'}de{'Offen, Position 2 Offen(Bit02/3.2)'}")]
    [ACPropertyEntity(103, "Bit03", "en{'Mid (Bit03/3.3)'}de{'Mitte (Bit03/3.3)'}")]
    [ACPropertyEntity(104, "Bit04", "en{'Half open (Bit04/3.4)'}de{'Halb Offen (Bit04/3.4)'}")]
    [ACPropertyEntity(108, "Bit08", "en{'Allocated, Position 2 (Bit08/2.0)'}de{'Belegt, Position 2 (Bit08/2.0)'}")]
    [ACPropertyEntity(109, "Bit09", "en{'Allocated, Position 1 (Bit09/2.1)'}de{'Belegt, Position 1 (Bit09/2.1)'}")]
    [ACPropertyEntity(112, "Bit12", "en{'Endposition 1 (Bit12/2.4)'}de{'Endlage 1 (Bit12/2.4)'}")]
    [ACPropertyEntity(113, "Bit13", "en{'Endposition 2 (Bit13/2.5)'}de{'Endlage 2 (Bit13/2.5)'}")]
    public class GIPConv2006ActuatorMaskRes : GIPConv2006BaseMaskRes
    {
        #region c'tors
        public GIPConv2006ActuatorMaskRes()
        {
        }

        public GIPConv2006ActuatorMaskRes(IACType acValueType)
            : base(acValueType)
        {
        }
        #endregion

        #region Customized Bits

        public bool Bit00_Pos1Closed
        {
            get { return Bit00; }
            set { Bit00 = value; }
        }

        public bool Bit01_Triggered
        {
            get { return Bit01; }
            set { Bit01 = value; }
        }

        public bool Bit02_Pos2Open
        {
            get { return Bit02; }
            set { Bit02 = value; }
        }

        public bool Bit03_Mid
        {
            get { return Bit03; }
            set { Bit03 = value; }
        }

        public bool Bit04_HalfOpen
        {
            get { return Bit04; }
            set { Bit04 = value; }
        }

        public bool Bit08_AllocatedPos2
        {
            get { return Bit08; }
            set { Bit08 = value; }
        }

        public bool Bit09_AllocatedPos1
        {
            get { return Bit09; }
            set { Bit09 = value; }
        }

        public bool Bit12_EndPos1
        {
            get { return Bit12; }
            set { Bit12 = value; }
        }

        public bool Bit13_EndPos2
        {
            get { return Bit13; }
            set { Bit13 = value; }
        }
        #endregion
    }

    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'BitAccess GIPConv2006ActuatorMaskReq'}de{'Bitzugriff GIPConv2006ActuatorMaskReq'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(100, "Bit00", "en{'Close, Position 1 (Bit00)'}de{'Schliessen, Position 1 (Bit00)'}")]
    [ACPropertyEntity(101, "Bit01", "en{'Stop (Bit01)'}de{'Stopp (Bit01)'}")]
    [ACPropertyEntity(102, "Bit02", "en{'Open, Position 2 (Bit02)'}de{'Öffnen, Position 2 (Bit02)'}")]
    [ACPropertyEntity(103, "Bit03", "en{'Mid, Posiiton 3 (Bit03)'}de{'Mitte, Position 3 (Bit03)'}")]
    public class GIPConv2006ActuatorMaskReq : GIPConv2006BaseMaskReq
    {
        #region c'tors
        public GIPConv2006ActuatorMaskReq()
        {
        }

        public GIPConv2006ActuatorMaskReq(IACType acValueType)
            : base(acValueType)
        {
        }
        #endregion

        #region Customized Bits

        public bool Bit00_Pos1Close
        {
            get { return Bit00; }
            set { Bit00 = value; }
        }

        public bool Bit01_Stop
        {
            get { return Bit01; }
            set { Bit01 = value; }
        }

        public bool Bit02_Pos2Open
        {
            get { return Bit02; }
            set { Bit02 = value; }
        }

        public bool Bit03_Pos3Mid
        {
            get { return Bit03; }
            set { Bit03 = value; }
        }
        #endregion
    }


    /// <summary>
    /// Baseclass for converting State and Types between Standard-Model-Components and DataAccess-/Vendor Model
    /// </summary>
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'GIPConv2006Actuator'}de{'GIPConv2006Actuator'}", Global.ACKinds.TACAbstractClass, Global.ACStorableTypes.NotStorable, false, false)]
    public abstract class GIPConv2006Actuator : GIPConv2006Base
    {
        #region c'tors
        public GIPConv2006Actuator(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _ConvBehaviour = new ACPropertyConfigValue<ushort>(this, "ConvBehaviour", 0);
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            _ = ConvBehaviour;
            return true;
        }

        public override bool ACPostInit()
        {
            return base.ACPostInit();
        }

        protected override void BindMyProperties()
        {
            if (_PropertiesBound)
                return;
            base.BindMyProperties();
            if (ParentACComponent is PAEActuator1way_Analog)
            {
                if (_ReqOpeningWidth != null && _ReqOpeningWidth.Source == null && AggrNo != null && AggrNo.ValueT > 0 && Session != null)
                {
                    IACPropertyNetTarget newTarget = null;
                    if (BindProperty("AGG_HSOLL", _ReqOpeningWidth, String.Format("A{0:000}", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                    {
                        if (ReqOpeningWidth != null)
                            (ReqOpeningWidth as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
                        ReqOpeningWidth = newTarget as IACContainerTNet<Double>;
                        newTarget.ForceBroadcast = true;
                        if (ReqOpeningWidth != null)
                            (ReqOpeningWidth as IACPropertyNetServer).ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
                    }
                }
                if (_OpeningWidth != null && _OpeningWidth.Source == null && AggrNo != null && AggrNo.ValueT > 0 && Session != null)
                {
                    IACPropertyNetTarget newTarget = null;
                    if (BindProperty("AGG_IST", _OpeningWidth, String.Format("A{0:000}", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                    {
                        if (OpeningWidth != null)
                            (OpeningWidth as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
                        OpeningWidth = newTarget as IACContainerTNet<Double>;
                        if (OpeningWidth != null)
                            (OpeningWidth as IACPropertyNetServer).ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
                    }
                }
                if (_DesiredWidth != null && _DesiredWidth.Source == null && AggrNo != null && AggrNo.ValueT > 0 && Session != null)
                {
                    IACPropertyNetTarget newTarget = null;
                    if (BindProperty("AGG_SOLL", _DesiredWidth, String.Format("A{0:000}", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                    {
                        if (DesiredWidth != null)
                            (DesiredWidth as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
                        DesiredWidth = newTarget as IACContainerTNet<Double>;
                        if (DesiredWidth != null)
                            (DesiredWidth as IACPropertyNetServer).ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
                    }
                }
            }
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (Pos1 != null)
                (Pos1 as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            Pos1 = null;

            if (Pos2 != null)
                (Pos2 as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            Pos2 = null;

            if (Pos3 != null)
                (Pos3 as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            Pos3 = null;

            if (OpeningWidth != null)
                (OpeningWidth as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            OpeningWidth = null;

            if (DesiredWidth != null)
                (DesiredWidth as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            DesiredWidth = null;

            if (StopAct != null)
                (StopAct as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            StopAct = null;

            if (ReqPos1 != null)
                (ReqPos1 as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            ReqPos1 = null;

            if (ReqPos2 != null)
                (ReqPos2 as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            ReqPos2 = null;

            if (ReqPos3 != null)
                (ReqPos3 as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            ReqPos3 = null;

            if (ReqOpeningWidth != null)
                (ReqOpeningWidth as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            ReqOpeningWidth = null;

            if (ReqStopAct != null)
                (ReqStopAct as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            ReqStopAct = null;

            return base.ACDeInit(deleteACClassTask);
        }
        #endregion

        #region Properties

        #region Configuration
        private ACPropertyConfigValue<ushort> _ConvBehaviour;
        [ACPropertyConfig("en{'Converter Behaviour'}de{'Konvertierer Verhalten'}")]
        public ushort ConvBehaviour
        {
            get
            {
                return _ConvBehaviour.ValueT;
            }
            set
            {
                _ConvBehaviour.ValueT = value;
            }
        }

        #endregion

        #region Read-Values from PLC
        [ACPropertyBindingTarget(240, "Read from PLC", "en{'Response'}de{'Rückmeldung'}", "", false, false)]
        public IACContainerTNet<GIPConv2006ActuatorMaskRes> Response { get; set; }
        protected override IACPropertyNetTarget _Response
        {
            get
            {
                return (IACPropertyNetTarget)this.Response;
            }
        }

        protected override GIPConv2006BaseMaskRes _ResponseValue
        {
            get { return Response.ValueT; }
        }

        protected GIPConv2006ActuatorMaskRes _UnboundedResponse = new GIPConv2006ActuatorMaskRes();

        public IACContainerTNet<Boolean> Pos1 { get; set; }
        public IACContainerTNet<Boolean> Pos2 { get; set; }
        public IACContainerTNet<Boolean> Pos3 { get; set; }

        public IACContainerTNet<Double> OpeningWidth { get; set; }
        public IACPropertyNetTarget _OpeningWidth
        {
            get
            {
                return (IACPropertyNetTarget)this.OpeningWidth;
            }
        }

        public IACContainerTNet<Double> DesiredWidth { get; set; }
        public IACPropertyNetTarget _DesiredWidth
        {
            get
            {
                return (IACPropertyNetTarget)this.DesiredWidth;
            }
        }

        public IACContainerTNet<Boolean> StopAct { get; set; }

        #endregion

        #region Write-Value to PLC
        [ACPropertyBindingTarget(250, "Read from PLC", "en{'Request'}de{'Kommando'}", "", false, false)]
        public IACContainerTNet<GIPConv2006ActuatorMaskReq> Request { get; set; }
        protected override IACPropertyNetTarget _Request
        {
            get
            {
                return (IACPropertyNetTarget)Request;
            }
        }

        protected override GIPConv2006BaseMaskReq _RequestValue
        {
            get { return Request.ValueT; }
        }


        public IACContainerTNet<Boolean> ReqPos1 { get; set; }
        public IACContainerTNet<Boolean> ReqPos2 { get; set; }
        public IACContainerTNet<Boolean> ReqPos3 { get; set; }
        public IACContainerTNet<Double> ReqOpeningWidth { get; set; }
        public IACPropertyNetTarget _ReqOpeningWidth
        {
            get
            {
                return (IACPropertyNetTarget)ReqOpeningWidth;
            }
        }
        public IACContainerTNet<Boolean> ReqStopAct { get; set; }


        #endregion

        #endregion

        #region overridden methods
        protected override void Response_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            base.Response_PropertyChanged(sender, e, phase);
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                if (IsTriggered != null) IsTriggered.ValueT = Response.ValueT.Bit01_Triggered;
                bool hasNewUnboundedAlarm = false;

                if (_UnboundedResponse.Bit20_FaultExtern != Response.ValueT.Bit20_FaultExtern)
                {
                    _UnboundedResponse.Bit20_FaultExtern = Response.ValueT.Bit20_FaultExtern;
                    if (Response.ValueT.Bit20_FaultExtern)
                        hasNewUnboundedAlarm = true;
                }


                if (_UnboundedResponse.Bit30_FaultResponse != Response.ValueT.Bit30_FaultResponse)
                {
                    _UnboundedResponse.Bit30_FaultResponse = Response.ValueT.Bit30_FaultResponse;
                    if (Response.ValueT.Bit30_FaultResponse)
                        hasNewUnboundedAlarm = true;
                }

                if (hasNewUnboundedAlarm)
                {
                    var bitAccessMap = Response.ValueT.BitAccessMap;
                    if (bitAccessMap != null)
                    {
                        this.UnboundAlarm.ValueT = PANotifyState.AlarmOrFault;
                        BeginAlarmTrans();
                        if (_UnboundedResponse.Bit20_FaultExtern)
                            OnNewAlarmOccurred(UnboundAlarm, Response.ValueT.GetBitNameFromMap("Bit20"), true);
                        if (_UnboundedResponse.Bit30_FaultResponse)
                            OnNewAlarmOccurred(UnboundAlarm, Response.ValueT.GetBitNameFromMap("Bit30"), true);
                        EndAlarmTrans();
                    }
                }
                else
                {
                    bool isOneAlarmActive = _UnboundedResponse.Bit20_FaultExtern
                                            || _UnboundedResponse.Bit30_FaultResponse;

                    if (!isOneAlarmActive && this.UnboundAlarm.ValueT != PANotifyState.Off)
                    {
                        this.UnboundAlarm.ValueT = PANotifyState.Off;
                        AcknowledgeAlarms();
                    }
                }
            }
        }

        protected override bool OnParentServerPropertyFound(IACPropertyNetServer parentProperty)
        {
            switch (parentProperty.ACIdentifier)
            {
                case "Pos1":
                case "Pos1Open":
                    Pos1 = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "Pos2":
                    Pos2 = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "Pos3":
                    Pos3 = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "OpeningWidth":
                    OpeningWidth = parentProperty as IACContainerTNet<Double>;
                    return true;
                case "DesiredWidth":
                    DesiredWidth = parentProperty as IACContainerTNet<Double>;
                    return true;
                case "StopAct":
                    StopAct = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "ReqPos1":
                case "ReqPos1Open":
                    ReqPos1 = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "ReqPos2":
                    ReqPos2 = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "ReqPos3":
                    ReqPos3 = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "ReqOpeningWidth":
                    ReqOpeningWidth = parentProperty as IACContainerTNet<Double>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "ReqStopAct":
                    ReqStopAct = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                default:
                    break;
            }

            return base.OnParentServerPropertyFound(parentProperty);
        }
        #endregion
    }

}
