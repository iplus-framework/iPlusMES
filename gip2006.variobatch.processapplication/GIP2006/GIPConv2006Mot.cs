using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.ComponentModel;
using System.Runtime.Serialization;
using gip.core.processapplication;
using gip.mes.processapplication;

namespace gip2006.variobatch.processapplication
{
    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'BitAccess GIPConv2006MotMaskRes'}de{'Bitzugriff GIPConv2006MotMaskRes'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(100, "Bit00", "en{'Stopped (Bit00/3.0)'}de{'Steht  (Bit00/3.0)'}")]
    [ACPropertyEntity(101, "Bit01", "en{'Is triggered (Bit01/3.1)'}de{'Angesteuert (Bit01/3.1)'}")]
    [ACPropertyEntity(102, "Bit02", "en{'Running (Bit02/3.2)'}de{'Laufmeldung (Bit02/3.2)'}")]
    [ACPropertyEntity(103, "Bit03", "en{'Fast (Bit03/3.3)'}de{'Schnell (Bit03/3.3)'}")]
    [ACPropertyEntity(104, "Bit04", "en{'Direction 1 (Bit04/3.4)'}de{'Richtung 1 (Bit04/3.4)'}")]
    [ACPropertyEntity(119, "Bit19", "en{'Jam sensor fault (Bit19/1.3)'}de{'Staumelder (Bit19/1.3)'}")]
    [ACPropertyEntity(122, "Bit22", "en{'VFD fault (Bit22/1.6)'}de{'Störung FU, Softstarter (Bit22/1.6)'}")]
    [ACPropertyEntity(124, "Bit24", "en{'Thermistor fault (Bit24/0.0)'}de{'Kaltleiter, PTC (Bit24/0.0)'}")]
    [ACPropertyEntity(125, "Bit25", "en{'Rotation fontrol fault (Bit25/0.1)'}de{'Drehzahlwächter (Bit25/0.1)'}")]
    [ACPropertyEntity(126, "Bit26", "en{'Misalignment top right (Bit26/0.2)'}de{'Schieflaufwächter oben rechts (Bit26/0.2)'}")]
    [ACPropertyEntity(127, "Bit27", "en{'Misalignment top left (Bit27/0.3)'}de{'Schieflaufwächter oben links (Bit27/0.3)'}")]
    [ACPropertyEntity(128, "Bit28", "en{'Misalignment bottom right (Bit28/0.4)'}de{'Schieflaufwächter unten rechts (Bit28/0.4)'}")]
    [ACPropertyEntity(129, "Bit29", "en{'Misalignment bottom left (Bit29/0.5)'}de{'Schieflaufwächter unten links (Bit29/0.5)'}")]
    [ACPropertyEntity(131, "Bit31", "en{'ContactorFault (Bit31/0.7)'}de{'Motorschutz (Bit31/0.7)'}")]
    public class GIPConv2006MotMaskRes : GIPConv2006BaseMaskRes
    {
        #region c'tors
        public GIPConv2006MotMaskRes()
        {
        }

        public GIPConv2006MotMaskRes(IACType acValueType)
            : base(acValueType)
        {
        }
        #endregion

        #region Customized Bits

        public bool Bit00_Stopped
        {
            get { return Bit00; }
            set { Bit00 = value; }
        }

        public bool Bit01_Triggered
        {
            get { return Bit01; }
            set { Bit01 = value; }
        }

        public bool Bit02_Run
        {
            get { return Bit02; }
            set { Bit02 = value; }
        }

        public bool Bit03_Fast
        {
            get { return Bit03; }
            set { Bit03 = value; }
        }

        public bool Bit04_Direction1
        {
            get { return Bit04; }
            set { Bit04 = value; }
        }

        public bool Bit19_JamSensorFault
        {
            get { return Bit19; }
            set { Bit19 = value; }
        }

        public bool Bit22_VFDFault
        {
            get { return Bit22; }
            set { Bit22 = value; }
        }

        public bool Bit24_ThermistorFault
        {
            get { return Bit24; }
            set { Bit24 = value; }
        }

        public bool Bit25_RotCtrlFault
        {
            get { return Bit25; }
            set { Bit25 = value; }
        }

        public bool Bit26_MisTRFault
        {
            get { return Bit26; }
            set { Bit26 = value; }
        }

        public bool Bit27_MisTLFault
        {
            get { return Bit27; }
            set { Bit27 = value; }
        }

        public bool Bit28_MisBRFault
        {
            get { return Bit28; }
            set { Bit28 = value; }
        }

        public bool Bit29_MisBLFault
        {
            get { return Bit29; }
            set { Bit29 = value; }
        }

        public bool Bit31_ContactorFault
        {
            get { return Bit31; }
            set { Bit31 = value; }
        }
        #endregion
    }

    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'BitAccess GIPConv2006MotMaskReq'}de{'Bitzugriff GIPConv2006MotMaskReq'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(100, "Bit00", "en{'Stop (Bit00)'}de{'Stopp (Bit00)'}")]
    [ACPropertyEntity(102, "Bit02", "en{'Start (Bit02)'}de{'Start (Bit02)'}")]
    [ACPropertyEntity(103, "Bit03", "en{'Slow (Bit03)'}de{'Langsam (Bit03)'}")]
    [ACPropertyEntity(104, "Bit04", "en{'Fast (Bit04)'}de{'Schnell (Bit04)'}")]
    [ACPropertyEntity(108, "Bit08", "en{'Left (Bit08)'}de{'Linkslauf (Bit08)'}")]
    [ACPropertyEntity(109, "Bit09", "en{'Right (Bit09)'}de{'Rechtslauf (Bit09)'}")]
    public class GIPConv2006MotMaskReq : GIPConv2006BaseMaskReq
    {
        #region c'tors
        public GIPConv2006MotMaskReq()
        {
        }

        public GIPConv2006MotMaskReq(IACType acValueType)
            : base(acValueType)
        {
        }
        #endregion

        #region Customized Bits

        public bool Bit00_Stop
        {
            get { return Bit00; }
            set { Bit00 = value; }
        }

        public bool Bit02_Start
        {
            get { return Bit02; }
            set { Bit02 = value; }
        }

        public bool Bit03_Slow
        {
            get { return Bit03; }
            set { Bit03 = value; }
        }

        public bool Bit04_Fast
        {
            get { return Bit04; }
            set { Bit04 = value; }
        }
        public bool Bit08_Left
        {
            get { return Bit08; }
            set { Bit08 = value; }
        }

        public bool Bit09_Right
        {
            get { return Bit09; }
            set { Bit09 = value; }
        }

        #endregion
    }


    /// <summary>
    /// Baseclass for converting State and Types between Standard-Model-Components and DataAccess-/Vendor Model
    /// </summary>
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'GIPConv2006Mot'}de{'GIPConv2006Mot'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.NotStorable, false, false)]
    public class GIPConv2006Mot : GIPConv2006Base
    {
        #region c'tors
        public GIPConv2006Mot(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        public override bool ACPostInit()
        {
            if (ParentACComponent is PAEEMotorBase)
            {
                PAEEMotorBase motor = ParentACComponent as PAEEMotorBase;
                if (motor != null)
                {
                    PAEContactor contactor = motor.Contactor;
                    if (contactor != null)
                    {
                        ContactorSensorState = contactor.SensorState;
                        if (ContactorSensorState != null)
                            (ContactorSensorState as IACPropertyNetServer).ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
                        FaultAckContactor = contactor.FaultACK;
                        if (FaultAckContactor != null)
                            (FaultAckContactor as IACPropertyNetServer).ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
                    }

                    PAEThermistor thermistor = motor.Thermistor;
                    if (thermistor != null)
                    {
                        ThermistorSensorState = thermistor.SensorState;
                        if (ThermistorSensorState != null)
                            (ThermistorSensorState as IACPropertyNetServer).ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
                        FaultAckThermistor = thermistor.FaultACK;
                        if (FaultAckThermistor != null)
                            (FaultAckThermistor as IACPropertyNetServer).ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
                    }

                    //PAEAmmeter Ammeter = Motor.Ammeter;
                    //if (Ammeter != null)
                    //{
                    //}
                }
                if (motor.ParentACComponent is PAETransport)
                {
                    PAETransport transport = motor.ParentACComponent as PAETransport;
                    if (transport != null)
                    {
                        PAERotationControl rotCtrl = transport.RotationControl;
                        if (rotCtrl != null)
                        {
                            RotCtrlSensorState = rotCtrl.SensorState;
                            if (RotCtrlSensorState != null)
                                (RotCtrlSensorState as IACPropertyNetServer).ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
                            FaultAckRotCtrl = rotCtrl.FaultACK;
                            if (FaultAckRotCtrl != null)
                                (FaultAckRotCtrl as IACPropertyNetServer).ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
                        }

                        foreach (PAEJamSensor jamSensor in transport.JamSensors)
                        {
                            JamSensorState = jamSensor.SensorState;
                            if (JamSensorState != null)
                                (JamSensorState as IACPropertyNetServer).ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
                            FaultAckJam = jamSensor.FaultACK;
                            if (FaultAckJam != null)
                                (FaultAckJam as IACPropertyNetServer).ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
                            break;
                        }

                        if (transport is PAEElevator)
                        {
                            PAEElevator elevator = transport as PAEElevator;
                            foreach (PAEMisalignment misSensor in elevator.MisalignmentSensors)
                            {
                                if (misSensor.ACIdentifier.EndsWith("TL") || misSensor.ACIdentifier.EndsWith("LT"))
                                {
                                    MisTLSensorState = misSensor.SensorState;
                                    if (MisTLSensorState != null)
                                        (MisTLSensorState as IACPropertyNetServer).ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
                                    FaultAckMisTR = misSensor.FaultACK;
                                    if (FaultAckMisTR != null)
                                        (FaultAckMisTR as IACPropertyNetServer).ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
                                }
                                else if (misSensor.ACIdentifier.EndsWith("TR") || misSensor.ACIdentifier.EndsWith("RT"))
                                {
                                    MisTRSensorState = misSensor.SensorState;
                                    if (MisTRSensorState != null)
                                        (MisTRSensorState as IACPropertyNetServer).ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
                                    FaultAckMisTR = misSensor.FaultACK;
                                    if (FaultAckMisTR != null)
                                        (FaultAckMisTR as IACPropertyNetServer).ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
                                }
                                else if (misSensor.ACIdentifier.EndsWith("BL") || misSensor.ACIdentifier.EndsWith("LB"))
                                {
                                    MisBLSensorState = misSensor.SensorState;
                                    if (MisBLSensorState != null)
                                        (MisBLSensorState as IACPropertyNetServer).ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
                                    FaultAckMisTR = misSensor.FaultACK;
                                    if (FaultAckMisTR != null)
                                        (FaultAckMisTR as IACPropertyNetServer).ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
                                }
                                else if (misSensor.ACIdentifier.EndsWith("BR") || misSensor.ACIdentifier.EndsWith("RB"))
                                {
                                    MisBRSensorState = misSensor.SensorState;
                                    if (MisBRSensorState != null)
                                        (MisBRSensorState as IACPropertyNetServer).ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
                                    FaultAckMisTR = misSensor.FaultACK;
                                    if (FaultAckMisTR != null)
                                        (FaultAckMisTR as IACPropertyNetServer).ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
                                }
                            }
                        }
                    }
                }
            }
            return base.ACPostInit();
        }

        protected override void BindMyProperties()
        {
            if (_PropertiesBound)
                return;
            base.BindMyProperties();
            if (ParentACComponent is PAETurnHeadDist)
            {
                if (_ReqPosition != null && _ReqPosition.Source == null && AggrNo != null && AggrNo.ValueT > 0 && Session != null)
                {
                    IACPropertyNetTarget newTarget = null;
                    if (BindProperty("AGG_HSOLL", _ReqPosition, String.Format("A{0:000}", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                    {
                        ReqPosition = newTarget as IACContainerTNet<Int16>;
                        newTarget.ForceBroadcast = true;
                    }
                }
                if (_Position != null && _Position.Source == null && AggrNo != null && AggrNo.ValueT > 0 && Session != null)
                {
                    IACPropertyNetTarget newTarget = null;
                    if (BindProperty("AGG_IST", _Position, String.Format("A{0:000}", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                    {
                        Position = newTarget as IACContainerTNet<Int16>;
                    }
                }
                if (_DesiredPosition != null && _DesiredPosition.Source == null && AggrNo != null && AggrNo.ValueT > 0 && Session != null)
                {
                    IACPropertyNetTarget newTarget = null;
                    if (BindProperty("AGG_SOLL", _DesiredPosition, String.Format("A{0:000}", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                    {
                        DesiredPosition = newTarget as IACContainerTNet<Int16>;
                    }
                }
            }
            else //if (ParentACComponent is PAEEMotorFreqCtrl)
            {
                if (_ReqSpeed != null && _ReqSpeed.Source == null && AggrNo != null && AggrNo.ValueT > 0 && Session != null)
                {
                    IACPropertyNetTarget newTarget = null;
                    if (BindProperty("AGG_HSOLL", _ReqSpeed, String.Format("A{0:000}", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                    {
                        ReqSpeed = newTarget as IACContainerTNet<Double>;
                        newTarget.ForceBroadcast = true;
                    }
                }
                if (_Speed != null && _Speed.Source == null && AggrNo != null && AggrNo.ValueT > 0 && Session != null)
                {
                    IACPropertyNetTarget newTarget = null;
                    if (BindProperty("AGG_IST", _Speed, String.Format("A{0:000}", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                    {
                        Speed = newTarget as IACContainerTNet<Double>;
                    }
                }
                if (_DesiredSpeed != null && _DesiredSpeed.Source == null && AggrNo != null && AggrNo.ValueT > 0 && Session != null)
                {
                    IACPropertyNetTarget newTarget = null;
                    if (BindProperty("AGG_SOLL", _DesiredSpeed, String.Format("A{0:000}", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                    {
                        DesiredSpeed = newTarget as IACContainerTNet<Double>;
                    }
                }
            }
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (ContactorSensorState != null)
                (ContactorSensorState as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            ContactorSensorState = null;
            
            if (FaultAckContactor != null)
                (FaultAckContactor as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            FaultAckContactor = null;
            
            if (ThermistorSensorState != null)
                (ThermistorSensorState as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            ThermistorSensorState = null;
            
            if (FaultAckThermistor != null)
                (FaultAckThermistor as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            FaultAckThermistor = null;
            
            if (RotCtrlSensorState != null)
                (RotCtrlSensorState as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            RotCtrlSensorState = null;
            
            if (FaultAckRotCtrl != null)
                (FaultAckRotCtrl as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            FaultAckRotCtrl = null;

            if (MisTRSensorState != null)
                (MisTRSensorState as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            MisTRSensorState = null;

            if (FaultAckMisTR != null)
                (FaultAckMisTR as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            FaultAckMisTR = null;

            if (MisTLSensorState != null)
                (MisTLSensorState as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            MisTLSensorState = null;

            if (FaultAckMisTL != null)
                (FaultAckMisTL as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            FaultAckMisTL = null;

            if (MisBLSensorState != null)
                (MisBLSensorState as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            MisBLSensorState = null;

            if (FaultAckMisBL != null)
                (FaultAckMisBL as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            FaultAckMisBL = null;

            if (MisBRSensorState != null)
                (MisBRSensorState as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            MisBRSensorState = null;

            if (FaultAckMisBR != null)
                (FaultAckMisBR as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            FaultAckMisBR = null;

            if (JamSensorState != null)
                (JamSensorState as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            JamSensorState = null;

            if (FaultAckJam != null)
                (FaultAckJam as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            FaultAckJam = null;

            if (RunState != null)
                (RunState as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            RunState = null;

            if (DirectionLeft != null)
                (DirectionLeft as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            DirectionLeft = null;

            if (SpeedFast != null)
                (SpeedFast as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            SpeedFast = null;

            if (MaintenanceSwitch != null)
                (MaintenanceSwitch as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            MaintenanceSwitch = null;

            if (ReqDirectionLeft != null)
                (ReqDirectionLeft as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            ReqDirectionLeft = null;

            if (ReqSpeedFast != null)
                (ReqSpeedFast as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            ReqSpeedFast = null;

            if (ReqRunState != null)
                (ReqRunState as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            ReqRunState = null;

            if (VFDState != null)
                (VFDState as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            VFDState = null;

            if (FaultAckVFD != null)
                (FaultAckVFD as IACPropertyNetServer).ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
            FaultAckVFD = null;

            return base.ACDeInit(deleteACClassTask);
        }
        #endregion

        #region Properties

        #region Read-Values from PLC
        [ACPropertyBindingTarget(240, "Read from PLC", "en{'Response'}de{'Rückmeldung'}", "", false, false)]
        public IACContainerTNet<GIPConv2006MotMaskRes> Response { get; set; }
        protected override IACPropertyNetTarget _Response 
        {
            get
            {
                 return (IACPropertyNetTarget) this.Response;
            }
        }

        protected override GIPConv2006BaseMaskRes _ResponseValue
        {
            get { return Response.ValueT; }
        }

        protected GIPConv2006MotMaskRes _UnboundedResponse = new GIPConv2006MotMaskRes();


        #region Motor
        public IACContainerTNet<Boolean> RunState { get; set; }
        public IACContainerTNet<Boolean> DirectionLeft { get; set; }
        public IACContainerTNet<Boolean> SpeedFast { get; set; }
        public IACContainerTNet<Boolean> MaintenanceSwitch { get; set; }

        public IACContainerTNet<Double> ReqSpeed { get; set; }
        public IACPropertyNetTarget _ReqSpeed
        {
            get
            {
                return (IACPropertyNetTarget)this.ReqSpeed;
            }
        }

        public IACContainerTNet<Double> Speed { get; set; }
        public IACPropertyNetTarget _Speed
        {
            get
            {
                return (IACPropertyNetTarget)this.Speed;
            }
        }

        public IACContainerTNet<Double> DesiredSpeed { get; set; }
        public IACPropertyNetTarget _DesiredSpeed
        {
            get
            {
                return (IACPropertyNetTarget)this.DesiredSpeed;
            }
        }


        public IACContainerTNet<Int16> ReqPosition { get; set; }
        public IACPropertyNetTarget _ReqPosition
        {
            get
            {
                return (IACPropertyNetTarget)this.ReqPosition;
            }
        }

        public IACContainerTNet<Int16> Position { get; set; }
        public IACPropertyNetTarget _Position
        {
            get
            {
                return (IACPropertyNetTarget)this.Position;
            }
        }

        public IACContainerTNet<Int16> DesiredPosition { get; set; }
        public IACPropertyNetTarget _DesiredPosition
        {
            get
            {
                return (IACPropertyNetTarget)this.DesiredPosition;
            }
        }

        #endregion

        #region Sensors
        public IACContainerTNet<PANotifyState> ContactorSensorState { get; set; }
        public IACContainerTNet<PANotifyState> ThermistorSensorState { get; set; }
        public IACContainerTNet<PANotifyState> RotCtrlSensorState { get; set; }
        public IACContainerTNet<PANotifyState> MisTRSensorState { get; set; }
        public IACContainerTNet<PANotifyState> MisTLSensorState { get; set; }
        public IACContainerTNet<PANotifyState> MisBRSensorState { get; set; }
        public IACContainerTNet<PANotifyState> MisBLSensorState { get; set; }
        public IACContainerTNet<PANotifyState> JamSensorState { get; set; }
        public IACContainerTNet<PANotifyState> VFDState { get; set; }
        #endregion

        #endregion

        #region Write-Value to PLC
        [ACPropertyBindingTarget(250, "Read from PLC", "en{'Request'}de{'Kommando'}", "", false, false)]
        public IACContainerTNet<GIPConv2006MotMaskReq> Request { get; set; }
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


        #region Motor
        public IACContainerTNet<Boolean> ReqDirectionLeft { get; set; }
        public IACContainerTNet<Boolean> ReqSpeedFast { get; set; }
        public IACContainerTNet<Boolean> ReqRunState { get; set; }
        #endregion


        #region Sensors
        public IACContainerTNet<bool> FaultAckContactor { get; set; }
        public IACContainerTNet<bool> FaultAckThermistor { get; set; }
        public IACContainerTNet<bool> FaultAckRotCtrl { get; set; }
        public IACContainerTNet<bool> FaultAckMisTR { get; set; }
        public IACContainerTNet<bool> FaultAckMisTL { get; set; }
        public IACContainerTNet<bool> FaultAckMisBR { get; set; }
        public IACContainerTNet<bool> FaultAckMisBL { get; set; }
        public IACContainerTNet<bool> FaultAckJam { get; set; }
        public IACContainerTNet<bool> FaultAckVFD { get; set; }
        #endregion

        #endregion

        #endregion

        #region overridden methods
        protected override bool OnParentServerPropertyFound(IACPropertyNetServer parentProperty)
        {
            switch (parentProperty.ACIdentifier)
            {
                case "RunState":
                    RunState = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "DirectionLeft":
                    DirectionLeft = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "SpeedFast":
                    SpeedFast = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "MaintenanceSwitch":
                    MaintenanceSwitch = parentProperty as IACContainerTNet<Boolean>;
                    return true;
                case "ReqDirectionLeft":
                    ReqDirectionLeft = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "ReqSpeedFast":
                    ReqSpeedFast = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "ReqRunState":
                    ReqRunState = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "VFDState":
                case "StarterState":
                    VFDState = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                case "VFDStateACK":
                case "StarterStateACK":
                    FaultAckVFD = parentProperty as IACContainerTNet<bool>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                case "ReqSpeed":
                    ReqSpeed = parentProperty as IACContainerTNet<Double>;
                    parentProperty.ForceBroadcast = true;
                    return false;
                case "Speed":
                    Speed = parentProperty as IACContainerTNet<Double>;
                    return false;
                case "DesiredSpeed":
                    DesiredSpeed = parentProperty as IACContainerTNet<Double>;
                    return false;
                case "ReqPosition":
                    ReqPosition = parentProperty as IACContainerTNet<Int16>;
                    parentProperty.ForceBroadcast = true;
                    return false;
                case "Position":
                    Position = parentProperty as IACContainerTNet<Int16>;
                    return false;
                case "DesiredPosition":
                    DesiredPosition = parentProperty as IACContainerTNet<Int16>;
                    return false;
                default:
                    break;
            }

            return base.OnParentServerPropertyFound(parentProperty);
        }

        protected override void ModelProperty_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if (sender == FaultStateACK)
            {
                if (FaultStateACK.ValueT == true)
                {
                    _LockResend_ReqRunState = true;
                    if (ReqRunState.ValueT)
                        ReqRunState.ValueT = false;
                    _LockResend_ReqRunState = false;
                    _RequestValue.Bit15_FaultAck = FaultStateACK.ValueT;
                }
                return;
            }
            else if (sender == ReqRunState)
            {
                if (!_LockResend_ReqRunState)
                {
                    if (ReqRunState.ValueT)
                    {
                        Request.ValueT.Bit00_Stop = false;
                        Request.ValueT.Bit02_Start = true;
                    }
                    else
                    {
                        Request.ValueT.Bit00_Stop = true;
                        Request.ValueT.Bit02_Start = false;
                    }
                }
                return;
            }
            else if (sender == ReqDirectionLeft)
            {
                if (!_LockResend_ReqRunState)
                {
                    if (ReqDirectionLeft.ValueT)
                    {
                        Request.ValueT.Bit09_Right = false;
                        Request.ValueT.Bit08_Left = true;
                    }
                    else
                    {
                        Request.ValueT.Bit09_Right = true;
                        Request.ValueT.Bit08_Left = false;
                    }
                }
                return;
            }
            else if (sender == ReqSpeedFast)
            {
                if (!_LockResend_ReqRunState)
                {
                    if (ReqSpeedFast.ValueT)
                    {
                        Request.ValueT.Bit04_Fast = true;
                        Request.ValueT.Bit03_Slow = false;
                    }
                    else
                    {
                        Request.ValueT.Bit03_Slow = true;
                        Request.ValueT.Bit04_Fast = false;
                    }
                }
                return;
            }
            else if (sender == ReqOperatingMode)
            {
                if ((ReqOperatingMode.ValueT == Global.OperatingMode.Manual) || (ReqOperatingMode.ValueT == Global.OperatingMode.Maintenance))
                {
                    Request.ValueT.Bit00_Stop = true;
                    Request.ValueT.Bit02_Start = false;
                    Request.ValueT.Bit06_Manual = true;
                }
                else
                {
                    Request.ValueT.Bit06_Manual = false;
                }
                return;
            }
            else if (  (sender == FaultAckContactor)
                    || (sender == FaultAckThermistor)
                    || (sender == FaultAckRotCtrl)
                    || (sender == FaultAckMisTR)
                    || (sender == FaultAckMisTL)
                    || (sender == FaultAckMisBR)
                    || (sender == FaultAckMisBL)
                    || (sender == FaultAckJam)
                    || (sender == FaultAckVFD)
                )
            {
                bool quitValue = false;
                if (sender == FaultAckContactor)
                    quitValue = FaultAckContactor.ValueT;
                else if (sender == FaultAckThermistor)
                    quitValue = FaultAckThermistor.ValueT;
                else if (sender == FaultAckRotCtrl)
                    quitValue = FaultAckRotCtrl.ValueT;
                else if (sender == FaultAckMisTR)
                    quitValue = FaultAckMisTR.ValueT;
                else if (sender == FaultAckMisTL)
                    quitValue = FaultAckMisTL.ValueT;
                else if (sender == FaultAckMisBR)
                    quitValue = FaultAckMisBR.ValueT;
                else if (sender == FaultAckMisBL)
                    quitValue = FaultAckMisBL.ValueT;
                else if (sender == FaultAckJam)
                    quitValue = FaultAckJam.ValueT;
                else if (sender == FaultAckVFD)
                    quitValue = FaultAckVFD.ValueT;

                Request.ValueT.Bit15_FaultAck = quitValue;
                return;
            }
            base.ModelProperty_ValueUpdatedOnReceival(sender, e, phase);
        }
        #endregion

        #region Converter-Logic
        private bool _LockResend_ReqRunState = false;
        protected override void Response_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            base.Response_PropertyChanged(sender, e, phase);
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                _LockResend_ReqRunState = true;
                if (RunState != null)
                {
                    bool changed = RunState.ValueT != Response.ValueT.Bit02_Run;
                    RunState.ValueT = Response.ValueT.Bit02_Run;
                    if (changed)
                    {
                        PAEControlModuleBase controlModule = ParentACComponent as PAEControlModuleBase;
                        if (controlModule != null)
                        {
                            if (RunState.ValueT)
                                controlModule.TurnOnInstant.ValueT = DateTime.Now;
                            else
                                controlModule.TurnOffInstant.ValueT = DateTime.Now;
                        }
                    }
                }
                if (SpeedFast != null) SpeedFast.ValueT = !Response.ValueT.Bit03_Fast;
                if (DirectionLeft != null) DirectionLeft.ValueT = Response.ValueT.Bit04_Direction1;
                if (IsTriggered != null) IsTriggered.ValueT = Response.ValueT.Bit01_Triggered;
                //if (ReqRunState != null)
                //{
                //    if (!ReqRunState.ValueT)
                //    {
                //        if (RunState.ValueT || Response.ValueT.Bit01_Triggered)
                //            ReqRunState.ValueT = true;
                //    }
                //    else
                //    {
                //        if (!RunState.ValueT && !Response.ValueT.Bit01_Triggered)
                //            ReqRunState.ValueT = false;
                //    }
                //}
                bool hasNewUnboundedAlarm = false;
                if (JamSensorState != null)
                {
                    if (Response.ValueT.Bit19_JamSensorFault)
                        JamSensorState.ValueT = PANotifyState.AlarmOrFault;
                    else
                        JamSensorState.ValueT = PANotifyState.Off;
                }
                else
                {
                    if (_UnboundedResponse.Bit19_JamSensorFault != Response.ValueT.Bit19_JamSensorFault)
                    {
                        _UnboundedResponse.Bit19_JamSensorFault = Response.ValueT.Bit19_JamSensorFault;
                        if (Response.ValueT.Bit19_JamSensorFault)
                            hasNewUnboundedAlarm = true;
                    }
                }

                if (ThermistorSensorState != null)
                {
                    if (Response.ValueT.Bit24_ThermistorFault)
                        ThermistorSensorState.ValueT = PANotifyState.AlarmOrFault;
                    else
                        ThermistorSensorState.ValueT = PANotifyState.Off;
                }
                else
                {
                    if (_UnboundedResponse.Bit24_ThermistorFault != Response.ValueT.Bit24_ThermistorFault)
                    {
                        _UnboundedResponse.Bit24_ThermistorFault = Response.ValueT.Bit24_ThermistorFault;
                        if (Response.ValueT.Bit24_ThermistorFault)
                            hasNewUnboundedAlarm = true;
                    }
                }

                if (RotCtrlSensorState != null)
                {
                    if (Response.ValueT.Bit25_RotCtrlFault)
                        RotCtrlSensorState.ValueT = PANotifyState.AlarmOrFault;
                    else
                        RotCtrlSensorState.ValueT = PANotifyState.Off;
                }
                else
                {
                    if (_UnboundedResponse.Bit25_RotCtrlFault != Response.ValueT.Bit25_RotCtrlFault)
                    {
                        _UnboundedResponse.Bit25_RotCtrlFault = Response.ValueT.Bit25_RotCtrlFault;
                        if (Response.ValueT.Bit25_RotCtrlFault)
                            hasNewUnboundedAlarm = true;
                    }
                }


                if (MisTRSensorState != null)
                {
                    if (Response.ValueT.Bit26_MisTRFault)
                        MisTRSensorState.ValueT = PANotifyState.AlarmOrFault;
                    else
                        MisTRSensorState.ValueT = PANotifyState.Off;
                }
                else
                {
                    if (_UnboundedResponse.Bit26_MisTRFault != Response.ValueT.Bit26_MisTRFault)
                    {
                        _UnboundedResponse.Bit26_MisTRFault = Response.ValueT.Bit26_MisTRFault;
                        if (Response.ValueT.Bit26_MisTRFault)
                            hasNewUnboundedAlarm = true;
                    }
                }


                if (MisTLSensorState != null)
                {
                    if (Response.ValueT.Bit27_MisTLFault)
                        MisTLSensorState.ValueT = PANotifyState.AlarmOrFault;
                    else
                        MisTLSensorState.ValueT = PANotifyState.Off;
                }
                else
                {
                    if (_UnboundedResponse.Bit27_MisTLFault != Response.ValueT.Bit27_MisTLFault)
                    {
                        _UnboundedResponse.Bit27_MisTLFault = Response.ValueT.Bit27_MisTLFault;
                        if (Response.ValueT.Bit27_MisTLFault)
                            hasNewUnboundedAlarm = true;
                    }
                }


                if (MisBRSensorState != null)
                {
                    if (Response.ValueT.Bit28_MisBRFault)
                        MisBRSensorState.ValueT = PANotifyState.AlarmOrFault;
                    else
                        MisBRSensorState.ValueT = PANotifyState.Off;
                }
                else
                {
                    if (_UnboundedResponse.Bit28_MisBRFault != Response.ValueT.Bit28_MisBRFault)
                    {
                        _UnboundedResponse.Bit28_MisBRFault = Response.ValueT.Bit28_MisBRFault;
                        if (Response.ValueT.Bit28_MisBRFault)
                            hasNewUnboundedAlarm = true;
                    }
                }

                if (MisBLSensorState != null)
                {
                    if (Response.ValueT.Bit29_MisBLFault)
                        MisBLSensorState.ValueT = PANotifyState.AlarmOrFault;
                    else
                        MisBLSensorState.ValueT = PANotifyState.Off;
                }
                else
                {
                    if (_UnboundedResponse.Bit29_MisBLFault != Response.ValueT.Bit29_MisBLFault)
                    {
                        _UnboundedResponse.Bit29_MisBLFault = Response.ValueT.Bit29_MisBLFault;
                        if (Response.ValueT.Bit29_MisBLFault)
                            hasNewUnboundedAlarm = true;
                    }
                }


                if (ContactorSensorState != null)
                {
                    if (Response.ValueT.Bit31_ContactorFault)
                        ContactorSensorState.ValueT = PANotifyState.AlarmOrFault;
                    else
                        ContactorSensorState.ValueT = PANotifyState.Off;
                }
                else
                {
                    if (_UnboundedResponse.Bit31_ContactorFault != Response.ValueT.Bit31_ContactorFault)
                    {
                        _UnboundedResponse.Bit31_ContactorFault = Response.ValueT.Bit31_ContactorFault;
                        if (Response.ValueT.Bit31_ContactorFault)
                            hasNewUnboundedAlarm = true;
                    }
                }


                if (VFDState != null)
                {
                    if (Response.ValueT.Bit22_VFDFault)
                        VFDState.ValueT = PANotifyState.AlarmOrFault;
                    else
                        VFDState.ValueT = PANotifyState.Off;
                }
                else
                {
                    if (_UnboundedResponse.Bit22_VFDFault != Response.ValueT.Bit22_VFDFault)
                    {
                        _UnboundedResponse.Bit22_VFDFault = Response.ValueT.Bit22_VFDFault;
                        if (Response.ValueT.Bit22_VFDFault)
                            hasNewUnboundedAlarm = true;
                    }
                }

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
                        if (_UnboundedResponse.Bit19_JamSensorFault)
                            OnNewAlarmOccurred(UnboundAlarm, Response.ValueT.GetBitNameFromMap("Bit19"), true);
                        if (_UnboundedResponse.Bit24_ThermistorFault)
                            OnNewAlarmOccurred(UnboundAlarm, Response.ValueT.GetBitNameFromMap("Bit24"), true);
                        if (_UnboundedResponse.Bit25_RotCtrlFault)
                            OnNewAlarmOccurred(UnboundAlarm, Response.ValueT.GetBitNameFromMap("Bit25"), true);
                        if (_UnboundedResponse.Bit26_MisTRFault)
                            OnNewAlarmOccurred(UnboundAlarm, Response.ValueT.GetBitNameFromMap("Bit26"), true);
                        if (_UnboundedResponse.Bit27_MisTLFault)
                            OnNewAlarmOccurred(UnboundAlarm, Response.ValueT.GetBitNameFromMap("Bit27"), true);
                        if (_UnboundedResponse.Bit28_MisBRFault)
                            OnNewAlarmOccurred(UnboundAlarm, Response.ValueT.GetBitNameFromMap("Bit28"), true);
                        if (_UnboundedResponse.Bit29_MisBLFault)
                            OnNewAlarmOccurred(UnboundAlarm, Response.ValueT.GetBitNameFromMap("Bit29"), true);
                        if (_UnboundedResponse.Bit31_ContactorFault)
                            OnNewAlarmOccurred(UnboundAlarm, Response.ValueT.GetBitNameFromMap("Bit31"), true);
                        if (_UnboundedResponse.Bit22_VFDFault)
                            OnNewAlarmOccurred(UnboundAlarm, Response.ValueT.GetBitNameFromMap("Bit22"), true);
                        if (_UnboundedResponse.Bit20_FaultExtern)
                            OnNewAlarmOccurred(UnboundAlarm, Response.ValueT.GetBitNameFromMap("Bit20"), true);
                        if (_UnboundedResponse.Bit30_FaultResponse)
                            OnNewAlarmOccurred(UnboundAlarm, Response.ValueT.GetBitNameFromMap("Bit30"), true);
                        EndAlarmTrans();
                    }
                }
                else
                {
                    bool isOneAlarmActive = _UnboundedResponse.Bit19_JamSensorFault
                                            || _UnboundedResponse.Bit24_ThermistorFault
                                            || _UnboundedResponse.Bit25_RotCtrlFault
                                            || _UnboundedResponse.Bit26_MisTRFault
                                            || _UnboundedResponse.Bit27_MisTLFault
                                            || _UnboundedResponse.Bit28_MisBRFault
                                            || _UnboundedResponse.Bit29_MisBLFault
                                            || _UnboundedResponse.Bit31_ContactorFault
                                            || _UnboundedResponse.Bit22_VFDFault
                                            || _UnboundedResponse.Bit20_FaultExtern
                                            || _UnboundedResponse.Bit30_FaultResponse;

                    if (!isOneAlarmActive && this.UnboundAlarm.ValueT != PANotifyState.Off)
                    {
                        this.UnboundAlarm.ValueT = PANotifyState.Off;
                        AcknowledgeAlarms();
                    }
                }

                _LockResend_ReqRunState = false;
            }
        }

        public override void OnSetRequest(IACPropertyNetValueEvent valueEvent)
        {
            base.OnSetRequest(valueEvent);
            if (valueEvent.Sender == EventRaiser.Source)
            {
                // Nicht zurücklesen, da SPS die gesetzten Werte selbst zurücksetzt
                //if (ReqRunState != null) ReqRunState.ValueT = Request.ValueT.Bit02;
            }
        }

        protected override void Request_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            base.Request_PropertyChanged(sender, e, phase);
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {
                _LockResend_ReqRunState = true;
                SynchronizeRequest(true);
                _LockResend_ReqRunState = false;
            }
        }

        private void SynchronizeRequest(bool reqChanged = false)
        {
            //if (!Response.ValueT.Bit01_Triggered && !reqChanged)
            //return;
            if (!reqChanged)
                return;
            //if (!ReqRunState.ValueT && Request.ValueT.Bit02_Start /*&& !Request.ValueT.Bit00_Stop*/)
            //{
            //    ReqRunState.ValueT = true;
            //}
            if (ReqRunState != null && ReqRunState.ValueT && (Response.ValueT.Bit02_Run || Request.ValueT.Bit00_Stop))
            {
                ReqRunState.ValueT = false;
            }
            //if ((ReqDirectionLeft != null) && !ReqDirectionLeft.ValueT && Request.ValueT.Bit08_Left /*&& !Request.ValueT.Bit09_Right*/)
            //{
            //    ReqDirectionLeft.ValueT = true;
            //}
            if ((ReqDirectionLeft != null) && ReqDirectionLeft.ValueT && /*Request.ValueT.Bit08_Left &&*/ Request.ValueT.Bit09_Right)
            {
                ReqDirectionLeft.ValueT = false;
            }
            if ((ReqSpeedFast != null) && ReqSpeedFast.ValueT && !Request.ValueT.Bit04_Fast && Request.ValueT.Bit03_Slow)
            {
                ReqSpeedFast.ValueT = false;
            }
            //else if ((ReqSpeedFast != null) && !ReqSpeedFast.ValueT && Request.ValueT.Bit04_Fast && !Request.ValueT.Bit03_Slow)
            //{
            //    ReqSpeedFast.ValueT = true;
            //}
        }

        #endregion

    }
}
