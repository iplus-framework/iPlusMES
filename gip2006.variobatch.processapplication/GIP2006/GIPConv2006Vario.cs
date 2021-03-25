using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.ComponentModel;
using System.Runtime.Serialization;
using gip.core.communication;
using gip.mes.processapplication;

namespace gip2006.variobatch.processapplication
{
    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'Variobatch 2006 stateword'}de{'Variobatch 2006 Statuswort'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(100, "Bit08", "en{'Bit  8: Active'}de{'Bit  8: Aktiv'}")]
    [ACPropertyEntity(101, "Bit09", "en{'Bit  9: Paused'}de{'Bit  9: Unterbrochen'}")]
    [ACPropertyEntity(102, "Bit10", "en{'Bit 10: Acknowledged'}de{'Bit 10: Quittungsbestätigung'}")]
    [ACPropertyEntity(103, "Bit11", "en{'Bit 11: Breaked'}de{'Bit 11: Abgebrochen'}")]
    [ACPropertyEntity(104, "Bit12", "en{'Bit 12: Switch interlock'}de{'Bit 12: Einschaltverriegelt'}")]
    [ACPropertyEntity(105, "Bit13", "en{'Bit 13: Operation interlock'}de{'Bit 13: Betriebsverriegelt'}")]
    [ACPropertyEntity(106, "Bit14", "en{'Bit 14: Malfunction'}de{'Bit 14: Störung'}")]
    [ACPropertyEntity(107, "Bit15", "en{'Bit 15: Completed'}de{'Bit 15: Fertig'}")]
    [ACPropertyEntity(108, "Bit00", "en{'Bit  0: Tolerance error'}de{'Bit  0: Toleranzfehler'}")]
    [ACPropertyEntity(109, "Bit01", "en{'Bit  1: Dosingtime error'}de{'Bit  1: Dosierzeitfehler'}")]
    [ACPropertyEntity(110, "Bit02", "en{'Bit  2: Servicemode'}de{'Bit  2: Servicebetrieb'}")]
    [ACPropertyEntity(111, "Bit03", "en{'Bit  3: Way finishing / Source switchable'}de{'Bit  3: Weg läuft ab / Wechselbereit'}")]
    [ACPropertyEntity(112, "Bit04", "en{'Bit  4: Way active'}de{'Bit  4: Weg läuft'}")]
    [ACPropertyEntity(113, "Bit05", "en{'Bit  5: Target value out of range'}de{'Bit  5: Sollwert zu gross'}")]
    [ACPropertyEntity(114, "Bit06", "en{'Bit  6: Destination full'}de{'Bit  6: Ziel voll'}")]
    [ACPropertyEntity(115, "Bit07", "en{'Bit  7: No dosing level'}de{'Bit  7: Keine Dosierstufe angewaehlt'}")]
    public class GIPConv2006VarioMaskRes : BitAccessForInt16
    {
        #region c'tors
        public GIPConv2006VarioMaskRes()
        {
        }

        public GIPConv2006VarioMaskRes(IACType acValueType)
            : base(acValueType)
        {
        }
        #endregion

        #region Customized Bits

        public bool Bit00_ToleranceError
        {
            get { return Bit00; }
            set { Bit00 = value; }
        }

        public bool Bit01_DosingtimeError
        {
            get { return Bit01; }
            set { Bit01 = value; }
        }

        public bool Bit02_ServiceMode
        {
            get { return Bit02; }
            set { Bit02 = value; }
        }

        public bool Bit03_SourceSwitchable
        {
            get { return Bit03; }
            set { Bit03 = value; }
        }

        public bool Bit04_WayActive
        {
            get { return Bit04; }
            set { Bit04 = value; }
        }

        public bool Bit05_TargetValueOutOfRange
        {
            get { return Bit05; }
            set { Bit05 = value; }
        }

        public bool Bit06_DestinationFull
        {
            get { return Bit06; }
            set { Bit06 = value; }
        }
        public bool Bit07_NoDosingLevel
        {
            get { return Bit07; }
            set { Bit07 = value; }
        }

        public bool Bit08_Active
        {
            get { return Bit08; }
            set { Bit08 = value; }
        }

        public bool Bit09_Paused
        {
            get { return Bit09; }
            set { Bit09 = value; }
        }

        public bool Bit10_Acknowledged
        {
            get { return Bit10; }
            set { Bit10 = value; }
        }
        
        public bool Bit11_Breaked
        {
            get { return Bit11; }
            set { Bit11 = value; }
        }

        public bool Bit12_SwitchInterlock
        {
            get { return Bit12; }
            set { Bit12 = value; }
        }

        public bool Bit13_OperationInterlock
        {
            get { return Bit13; }
            set { Bit13 = value; }
        }

        public bool Bit14_Malfunction
        {
            get { return Bit14; }
            set { Bit14 = value; }
        }

        public bool Bit15_Completed
        {
            get { return Bit15; }
            set { Bit15 = value; }
        }
        #endregion
    }

    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'Variobatch 2006 commandword'}de{'Variobatch 2006 Kommandowort'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACPropertyEntity(100, "Bit08", "en{'Bit  8: Start'}de{'Bit  8: Start'}")]
    [ACPropertyEntity(101, "Bit09", "en{'Bit  9: Pause'}de{'Bit  9: Unterbrechen'}")]
    [ACPropertyEntity(102, "Bit10", "en{'Bit 10: Continue'}de{'Bit 10: Fortsetzen'}")]
    [ACPropertyEntity(103, "Bit11", "en{'Bit 11: Break'}de{'Bit 11: Abbrechen'}")]
    [ACPropertyEntity(104, "Bit12", "en{'Bit 12: Stop'}de{'Bit 12: Stop'}")]
    [ACPropertyEntity(105, "Bit13", "en{'Bit 13: ---'}de{'Bit 13: ---'}")]
    [ACPropertyEntity(106, "Bit14", "en{'Bit 14: Acknowledge'}de{'Bit 14: Quittieren'}")]
    [ACPropertyEntity(107, "Bit15", "en{'Bit 15: ---'}de{'Bit 15: ---'}")]
    [ACPropertyEntity(108, "Bit00", "en{'Bit  0: Tolerance acknowledge'}de{'Bit  0: Toleranz Quittieren'}")]
    [ACPropertyEntity(109, "Bit01", "en{'Bit  1: Dosingtime acknowledge'}de{'Bit  1: Dosierzeit Quittieren'}")]
    [ACPropertyEntity(110, "Bit02", "en{'Bit  2: ---'}de{'Bit  2: ---'}")]
    [ACPropertyEntity(111, "Bit03", "en{'Bit  3: Signal startable'}de{'Bit  3: Signal Startbar'}")]
    [ACPropertyEntity(112, "Bit04", "en{'Bit  4: Prepare Source-Switching'}de{'Bit  4: Einleitung Quellenwechsel'}")]
    [ACPropertyEntity(113, "Bit05", "en{'Bit  5: Destination changed'}de{'Bit  5: Ziel gewechselt'}")]
    [ACPropertyEntity(114, "Bit06", "en{'Bit  6: End step'}de{'Bit  6: Schritt Beenden'}")]
    [ACPropertyEntity(115, "Bit07", "en{'Bit  7: ---'}de{'Bit  7: ---'}")]
    public class GIPConv2006VarioMaskReq : BitAccessForInt16
    {
        #region c'tors
        public GIPConv2006VarioMaskReq()
        {
        }

        public GIPConv2006VarioMaskReq(IACType acValueType)
            : base(acValueType)
        {
        }
        #endregion

        #region Customized Bits

        public bool Bit00_ToleranceAck
        {
            get { return Bit00; }
            set { Bit00 = value; }
        }

        public bool Bit01_DosingtimeAck
        {
            get { return Bit01; }
            set { Bit01 = value; }
        }

        public bool Bit03_SignalStartable
        {
            get { return Bit03; }
            set { Bit03 = value; }
        }

        public bool Bit04_PrepareSourceSwitching
        {
            get { return Bit04; }
            set { Bit04 = value; }
        }

        public bool Bit05_DestinationChanged
        {
            get { return Bit05; }
            set { Bit05 = value; }
        }

        public bool Bit06_EndStep
        {
            get { return Bit06; }
            set { Bit06 = value; }
        }

        public bool Bit08_Start
        {
            get { return Bit08; }
            set { Bit08 = value; }
        }

        public bool Bit09_Pause
        {
            get { return Bit09; }
            set { Bit09 = value; }
        }

        public bool Bit10_Continue
        {
            get { return Bit10; }
            set { Bit10 = value; }
        }

        public bool Bit11_Break
        {
            get { return Bit11; }
            set { Bit11 = value; }
        }

        public bool Bit12_Stop
        {
            get { return Bit12; }
            set { Bit12 = value; }
        }

        public bool Bit14_Acknowledge
        {
            get { return Bit14; }
            set { Bit14 = value; }
        }
        #endregion
    }


    /// <summary>
    /// Baseclass for converting State and Types between Standard-Model-Components and DataAccess-/Vendor Model
    /// </summary>
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'GIPConv2006 Vario'}de{'GIPConv2006 Vario'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.NotStorable, false, false)]
    public class GIPConv2006Vario : PAFuncStateConvBase
    {
        #region c'tors
        public GIPConv2006Vario(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _StateDBOffset = new ACPropertyConfigValue<UInt16>(this, "StateDBOffset", 0);
            _StateDBNo = new ACPropertyConfigValue<UInt16>(this, "StateDBNo", 0);
            _CmdDBOffset = new ACPropertyConfigValue<UInt16>(this, "CmdDBOffset", 0);
            _CmdDBNo = new ACPropertyConfigValue<UInt16>(this, "CmdDBNo", 0);

        }

        private static bool? _IsConverterDeactivated = null;
        public static bool IsConverterDeactivated
        {
            get
            {
                if (_IsConverterDeactivated.HasValue)
                    return _IsConverterDeactivated.Value;
                _IsConverterDeactivated = false;
                try
                {
                    ProcessConfiguration processConfig = (ProcessConfiguration)CommandLineHelper.ConfigCurrentDir.GetSection("Process/ProcessConfiguration");
                    if (processConfig != null)
                    {
                        if (processConfig.DeactivateProcessConverter)
                        {
                            _IsConverterDeactivated = processConfig.DeactivateProcessConverter;
                        }
                    }
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    if (gip.core.datamodel.Database.Root != null && gip.core.datamodel.Database.Root.Messages != null && 
                                                                          gip.core.datamodel.Database.Root.InitState == ACInitState.Initialized)
                        gip.core.datamodel.Database.Root.Messages.LogException("GIPConv2006Vario", "IsConverterDeactivated", msg);
                }
                return _IsConverterDeactivated.Value;
            }
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            if (!IsConverterDeactivated)
            {
                _Response.ValueUpdatedOnReceival += Response_PropertyChanged;
                _Request.ValueUpdatedOnReceival += Request_PropertyChanged;
            }

            return true;
        }

        public override bool ACPostInit()
        {
            // This calls are needed to add entries in Variobatch-Konfiguration
            UInt16 dbNo = CmdDBNo;
            UInt16 dbOffset = CmdDBOffset;
            dbNo = StateDBNo;
            dbOffset = StateDBOffset;

            bool result = base.ACPostInit();
            BindMyProperties();
            return result;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (!IsConverterDeactivated)
            {
                _Response.ValueUpdatedOnReceival -= Response_PropertyChanged;
                _Request.ValueUpdatedOnReceival -= Request_PropertyChanged;
            }

            return base.ACDeInit(deleteACClassTask);
        }

        private bool _PropertiesBound = false;
        private void BindMyProperties()
        {
            if (_PropertiesBound)
                return;
            if (!IsConverterDeactivated)
            {
                if (_Response.Source == null && AggrNo != null && AggrNo.ValueT > 0 && Session != null)
                {
                    IACPropertyNetTarget newTarget = null;
                    IACPropertyNetTarget oldTarget = _Response;
                    if (BindProperty("VB_STAT", _Response, String.Format("ST{0:000}", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                    {
                        OnReponsePropertyReplaced(oldTarget, newTarget);
                    }
                }
                if (_Request.Source == null && AggrNo != null && AggrNo.ValueT > 0 && Session != null)
                {
                    IACPropertyNetTarget newTarget = null;
                    IACPropertyNetTarget oldTarget = _Request;
                    if (BindProperty("VB_KOMM", _Request, String.Format("CMD{0:000}", AggrNo.ValueT), out newTarget).HasFlag(PropBindingBindingResult.TargetPropReplaced))
                    {
                        OnRequestPropertyReplaced(oldTarget, newTarget);
                    }
                }
            }
            _PropertiesBound = true;
        }
        #endregion

        #region Configuration

        [ACPropertyBindingSource(9999, "", "en{'Aggregate number'}de{'Aggregatnummer'}", "", false, false)]
        public IACContainerTNet<Int16> AggrNo { get; set; }

        [ACPropertyInfo(true, 500)]
        public short ConverterBehaviour
        {
            get;set;
        }

        #endregion

        #region Properties

        bool _AcceptStateFromPLC = false;
        [Flags]
        enum ConvInitState
        {
            None = 0x0,
            RequestInitialized = 0x1,
            ResponseInitialized = 0x2,
            ACStateRestored = 0x4,
        }

        ConvInitState _InitState = ConvInitState.None;

        public bool IsACStateRestored
        {
            get
            {
                return (_InitState & ConvInitState.ACStateRestored) == ConvInitState.ACStateRestored;
            }
        }

        public override bool IsReadyForSending
        {
            get
            {
                if (!this.Root.Initialized)
                    return false;
                if (IsSimulationOn)
                    return true;
                bool isReadyForSending = false; 
                if (this.Session != null)
                {
                    isReadyForSending = true;
                    ACSession acSession = this.Session as ACSession;
                    if (acSession != null && !acSession.IsReadyForWriting)
                        isReadyForSending = false;
                    else if (acSession == null && !(bool)this.Session.ACUrlCommand("IsReadyForWriting"))
                        isReadyForSending = false;
                }
                if (isReadyForSending && !IsACStateRestored)
                    isReadyForSending = false;
                return isReadyForSending;
            }
        }

        public override bool IsReadyForReading
        {
            get
            {
                if (IsSimulationOn)
                    return true;
                bool isReadyForReading = false;
                if (this.Session != null)
                {
                    isReadyForReading = true;
                    ACSession acSession = this.Session as ACSession;
                    if (acSession != null && !acSession.IsReadyForWriting)
                        isReadyForReading = false;
                    else if (acSession == null && !(bool)this.Session.ACUrlCommand("IsReadyForWriting"))
                        isReadyForReading = false;
                }
                if (isReadyForReading && !IsACStateRestored)
                    isReadyForReading = false;
                return isReadyForReading;
            }
        }

        #region Configuration

        private ACPropertyConfigValue<UInt16> _CmdDBNo;
        [ACPropertyConfig("en{'Command Datablocknumber'}de{'Kommando DB-Nummer'}")]
        public UInt16 CmdDBNo
        {
            get
            {
                UInt16 resultValue = _CmdDBNo.ValueT;
                if (resultValue == 0 && AggrNo != null && AggrNo.ValueT > 0)
                    resultValue = Convert.ToUInt16(1000 + AggrNo.ValueT);
                return resultValue;
            }
            set
            {
                _CmdDBNo.ValueT = value;
            }
        }

        private ACPropertyConfigValue<UInt16> _CmdDBOffset;
        [ACPropertyConfig("en{'Startaddress in command-db'}de{'Startadresse Kommando-DB'}")]
        public UInt16 CmdDBOffset
        {
            get
            {
                UInt16 resultValue = _CmdDBOffset.ValueT;
                if (resultValue == 0 && AggrNo != null && AggrNo.ValueT > 0)
                    resultValue = 200;
                return resultValue;
            }
            set
            {
                _CmdDBOffset.ValueT = value;
            }
        }


        private ACPropertyConfigValue<UInt16> _StateDBNo;
        [ACPropertyConfig("en{'State Datablocknumber'}de{'Status DB-Nummer'}")]
        public UInt16 StateDBNo
        {
            get
            {
                UInt16 resultValue = _StateDBNo.ValueT;
                if (resultValue == 0 && AggrNo != null && AggrNo.ValueT > 0)
                    resultValue = Convert.ToUInt16(1000 + AggrNo.ValueT);
                return resultValue;
            }
            set
            {
                _StateDBNo.ValueT = value;
            }
        }

        private ACPropertyConfigValue<UInt16> _StateDBOffset;
        [ACPropertyConfig("en{'Startaddress in state-db'}de{'Startadresse Status-DB'}")]
        public UInt16 StateDBOffset
        {
            get
            {
                UInt16 resultValue = _StateDBOffset.ValueT;
                if (resultValue == 0 && AggrNo != null && AggrNo.ValueT > 0)
                    resultValue = 100;
                return resultValue;
            }
            set
            {
                _StateDBOffset.ValueT = value;
            }
        }

        #endregion

        #region Read-Values from PLC
        [ACPropertyBindingTarget(240, "Read from PLC", "en{'Response'}de{'Rückmeldung'}", "", false, false)]
        public IACContainerTNet<GIPConv2006VarioMaskRes> Response { get; set; }
        protected IACPropertyNetTarget _Response
        {
            get
            {
                return (IACPropertyNetTarget)this.Response;
            }
        }

        protected virtual void OnReponsePropertyReplaced(IACPropertyNetTarget oldTarget, IACPropertyNetTarget newTarget)
        {
            if (!IsConverterDeactivated)
            {
                if (oldTarget != null)
                    (oldTarget as IACPropertyNetServer).ValueUpdatedOnReceival -= Response_PropertyChanged;
                if (newTarget != null)
                    (newTarget as IACPropertyNetServer).ValueUpdatedOnReceival += Response_PropertyChanged;
            }
        }
        
        protected GIPConv2006VarioMaskRes _ResponseValue
        {
            get { return Response.ValueT; }
        }

        public IACContainerTNet<PANotifyState> Malfunction { get; set; }
        public IACContainerTNet<PANotifyState> StateTolerance { get; set; }
        public IACContainerTNet<PANotifyState> StateLackOfMaterial { get; set; }
        public IACContainerTNet<PANotifyState> StateDosingTime { get; set; }
        public IACContainerTNet<PANotifyState> StateDestinationFull { get; set; }

        #endregion

        #region Write-Value to PLC
        [ACPropertyBindingTarget(250, "Read from PLC", "en{'Request'}de{'Kommando'}", "", false, false)]
        public IACContainerTNet<GIPConv2006VarioMaskReq> Request { get; set; }
        protected virtual IACPropertyNetTarget _Request
        {
            get
            {
                return (IACPropertyNetTarget)Request;
            }
        }
        protected virtual void OnRequestPropertyReplaced(IACPropertyNetTarget oldTarget, IACPropertyNetTarget newTarget)
        {
            if (!IsConverterDeactivated)
            {
                if (oldTarget != null)
                    oldTarget.ValueUpdatedOnReceival -= Request_PropertyChanged;
                if (newTarget != null)
                    newTarget.ValueUpdatedOnReceival += Request_PropertyChanged;
            }
        }
        protected virtual GIPConv2006VarioMaskReq _RequestValue
        {
            get { return Request.ValueT; }
        }

        public IACContainerTNet<bool> AckMalfunction { get; set; }
        public IACContainerTNet<bool> FaultAckTolerance { get; set; }
        public IACContainerTNet<bool> FaultAckLackOfMaterial { get; set; }
        public IACContainerTNet<bool> FaultAckDosingTime { get; set; }
        public IACContainerTNet<bool> FaultAckDestinationFull { get; set; }

        #endregion

        #endregion

        #region overridden methods
        protected override bool OnParentServerPropertyFound(IACPropertyNetServer parentProperty)
        {
            switch (parentProperty.ACIdentifier)
            {
                case "Malfunction":
                    Malfunction = parentProperty as IACContainerTNet<PANotifyState>;
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
                case "StateDestinationFull":
                    StateDestinationFull = parentProperty as IACContainerTNet<PANotifyState>;
                    return true;
                case "AckMalfunction":
                    AckMalfunction = parentProperty as IACContainerTNet<Boolean>;
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
                case "FaultAckDestinationFull":
                    FaultAckDestinationFull = parentProperty as IACContainerTNet<Boolean>;
                    parentProperty.ForceBroadcast = true;
                    return true;
                default:
                    break;
            }

            return base.OnParentServerPropertyFound(parentProperty);
        }

        public override ACStateEnum GetNextACState(PAProcessFunction sender, string transitionMethod = "")
        {
            ACStateEnum nextState = GetNextACState(false, transitionMethod);
            // Pausieren während des Stoppens:
            if (nextState == ACStateEnum.SMStopping 
                && transitionMethod == ACStateConst.TMPause 
                && ACState.ValueT == ACStateEnum.SMStopping
                && !_RequestValue.Bit09_Pause)
            {
                _RequestValue.Bit09_Pause = true;
                nextState = ACStateEnum.SMStopping;
            }
            else if (nextState == ACStateEnum.SMStopping
                && transitionMethod == ACStateConst.TMResume
                && ACState.ValueT == ACStateEnum.SMStopping
                && _RequestValue.Bit09_Pause
                && _ResponseValue.Bit09_Paused)
            {
                _RequestValue.Bit09_Pause = false;
                _RequestValue.Bit10_Continue = true;
                nextState = ACStateEnum.SMStopping;
            }
            return nextState;
        }

        protected virtual ACStateEnum GetNextACState(bool invokedFromResponseChangeEvent, string transitionMethod = "")
        {
            // TODO: Dieser Zustände machen erst bei Rückmeldung von der SPS weiter:
            // SMStarting, SMRunning, SMResetting, SMPausing, SMResuming, 
            // SMHolding, SMRestarting, SMAborting, SMStopping,
            ACStateEnum defaultNextState = PAFuncStateConvBase.GetDefaultNextACState(ACState.ValueT, transitionMethod);
            if (!IsSimulationOn)
            {
                // Falls aufgerufen von von PACProcessFunction
                if (!invokedFromResponseChangeEvent && String.IsNullOrEmpty(transitionMethod) && !IsReadyForSending)
                    return ACState.ValueT;

                ACStateEnum stateInPLC = TranslateToACStateFromBits();
                // transitionMethod is Empty in following cases:
                // 1. Invoked from cyclic method in PAProcessFunction which is not a transition (e.G. SMAborting, SMStopping....) 
                // 2. Invoked from SyncACStateFromPLC() when Response in PLC changed
                if (String.IsNullOrEmpty(transitionMethod))
                {
                    // PLC switched to the next state according to the standard State-Machine
                    if (defaultNextState == stateInPLC)
                    {
                        if (invokedFromResponseChangeEvent)
                            ResetRequestBitsForACState(stateInPLC);
                        return defaultNextState;
                    }
                    // PLC has not switched to the next state
                    else
                    {
                        // Out of sync. The current state in PLC is not in sync with current state in iPlus
                        if (ACState.ValueT != stateInPLC
                            && !(ACState.ValueT == ACStateEnum.SMStarting && stateInPLC == ACStateEnum.SMIdle))
                        {
                            if (invokedFromResponseChangeEvent
                                || _AcceptStateFromPLC)
                            {
                                ResetRequestBitsForACState(stateInPLC);
                                _AcceptStateFromPLC = false;
                                return stateInPLC;
                            }
                            else
                            {
                                ConversionAlarm.ValueT = PANotifyState.AlarmOrFault;

                                //Error:50172: The current state -{0}- in PLC is not in sync with current state -{1}- in iPlus
                                Msg msg = new Msg(this, eMsgLevel.Error, "", "GIPConv2006Vario.GetNextACState()", 1000, "Error50172", stateInPLC, ACState.ValueT);

                                Messages.LogException(this.GetACUrl(), "GIPConv2006Vario.GetNextACState()", msg.Message);
                                OnNewAlarmOccurred(ConversionAlarm, msg, true);

                                // TODO: Sicherstellen, dass beide PLC-Variablen eine aktuellen Stand haben
                                return ACState.ValueT;
                            }
                        }
                        else
                        {
                            if (invokedFromResponseChangeEvent)
                                ResetRequestBitsForACState(stateInPLC);
                            return ACState.ValueT; //return defaultNextState;
                        }
                    }
                }
                // Else state-change is in iPlus (Transition)
                else
                {
                    // Out of sync. The current state in PLC is not in sync with current state in iPlus
                    if (ACState.ValueT != stateInPLC
                        && !(   (ACState.ValueT == ACStateEnum.SMStarting && stateInPLC == ACStateEnum.SMIdle)
                             || (ACState.ValueT == ACStateEnum.SMRunning && stateInPLC == ACStateEnum.SMStopping && defaultNextState == ACStateEnum.SMAborting)
                             || (ACState.ValueT == ACStateEnum.SMCompleted && stateInPLC == ACStateEnum.SMResetting && defaultNextState == ACStateEnum.SMResetting))
                        )
                    {
                        if ((invokedFromResponseChangeEvent && !Root.Initialized) 
                            || _AcceptStateFromPLC)
                        {
                            _AcceptStateFromPLC = false;
                            return stateInPLC;
                        }
                        else
                        {
                            ConversionAlarm.ValueT = PANotifyState.AlarmOrFault;

                            //Error:50172: The current state -{0}- in PLC is not in sync with current state -{1}- in iPlus
                            Msg msg = new Msg(this, eMsgLevel.Error, "", "GIPConv2006Vario.GetNextACState()", 1010, "Error50172", stateInPLC, ACState.ValueT);

                            Messages.LogException(this.GetACUrl(), "GIPConv2006Vario.GetNextACState()", msg.Message);
                            OnNewAlarmOccurred(ConversionAlarm, msg, true);
                            return ACState.ValueT;
                        }
                    }
                    else
                    {
                        SetRequestForACState(defaultNextState);
                        stateInPLC = TranslateToACStateFromBits();
                        if (stateInPLC == defaultNextState)
                            return stateInPLC;
                        else if (stateInPLC == ACStateEnum.SMIdle && defaultNextState == ACStateEnum.SMResetting && transitionMethod == ACStateConst.TMReset)
                            return ACStateEnum.SMIdle;
                        return ACState.ValueT;
                    }
                }
            }

            return defaultNextState;
        }

        public override bool IsEnabledTransition(PAProcessFunction sender, string transitionMethod)
        {
            if (!IsSimulationOn)
            {
                if (!IsReadyForSending)
                {
                    PAProcessFunction processFunction = ParentACComponent as PAProcessFunction;
                    if (processFunction != null 
                        && !processFunction.IsSubscribedToWorkCycle
                        && transitionMethod == ACStateConst.TMReset
                        && (ACState.ValueT == ACStateEnum.SMCompleted || ACState.ValueT == ACStateEnum.SMAborted || ACState.ValueT == ACStateEnum.SMStopped))
                    {
                        processFunction.SubscribeToProjectWorkCycle();
                    }
                    // Auch wenn keine TCP_Verbindung zur SPS da ist, der Auftrag darf asynchron gestartet werden, 
                    // damit nicht ständig der Aufrufer neue Einträge in die Taskliste macht und somit unnötige neue ACClassTaskValue-Einträge angelegt und anschliessend wieder gelöscht werden
                    else if (processFunction != null
                        && transitionMethod == ACStateConst.TMStart
                        && ACState.ValueT == ACStateEnum.SMIdle)
                    {
                        return PAFuncStateConvBase.IsEnabledTransitionDefault(ACState.ValueT, transitionMethod, sender);
                    }
                    return false;
                }
            }

            // Holding wird in gip 2006 nicht unterstützt
            if (transitionMethod == ACStateConst.TMHold || transitionMethod == ACStateConst.TMRestart)
                return false;
            if (transitionMethod == ACStateConst.TMStart && ConverterBehaviour == 1)
            {
                ACStateEnum acStateInPLC = TranslateToACStateFromBits();
                if (ConverterBehaviour == 1 && ACState.ValueT == ACStateEnum.SMResetting && acStateInPLC == ACStateEnum.SMResetting)
                {
                    if (_ResponseValue.Bit11_Breaked)
                        _ResponseValue.Bit11_Breaked = false;
                }
            }
            if (transitionMethod == ACStateConst.TMRun)
            {
                if (!IsSimulationOn)
                {
                    // Nach dem Hochfahren überprüfen ob bereits ein Start an die SPS gesendet worden ist => keine ReSendACMethod mehr aufrufen
                    // indem UnSubscribeToProjectWorkCycle() aufgerufen wird
                    ACStateEnum acStateInPLC = TranslateToACStateFromBits();
                    if (acStateInPLC == ACStateEnum.SMStarting)
                    {
                        PAProcessFunction processFunction = ParentACComponent as PAProcessFunction;
                        if (processFunction != null && processFunction.CurrentACState == ACStateEnum.SMStarting)
                        {
                            processFunction.UnSubscribeToProjectWorkCycle();
                            return false;
                        }
                    }
                    else if (ConverterBehaviour == 1 && ACState.ValueT == ACStateEnum.SMStarting && acStateInPLC == ACStateEnum.SMResetting)
                    {
                        if (_ResponseValue.Bit11_Breaked)
                            _ResponseValue.Bit11_Breaked = false;
                        acStateInPLC = ACStateEnum.SMIdle;
                    }
                    if (!_ResponseValue.Bit13_OperationInterlock || !_ResponseValue.Bit12_SwitchInterlock || acStateInPLC != ACStateEnum.SMIdle)
                    {
                        // TODO Alarm
                        return false;
                    }
                }
            }
            // Sonderfall gip gegenüber S88: Pausieren während des Stoppens
            else if (transitionMethod == ACStateConst.TMPause)
            {
                return ACState.ValueT == ACStateEnum.SMRunning || ACState.ValueT == ACStateEnum.SMStopping;
            }
            // Sonderfall gip gegenüber S88: Fortsetzen während des Stoppens
            else if (transitionMethod == ACStateConst.TMResume)
            {
                return ACState.ValueT == ACStateEnum.SMPaused || _ResponseValue.Bit09_Paused;
            }
            return PAFuncStateConvBase.IsEnabledTransitionDefault(ACState.ValueT, transitionMethod, sender);
        }

        public override MsgWithDetails SendACMethod(PAProcessFunction sender, ACMethod acMethod)
        {
            if (Session == null)
            {
                MsgWithDetails msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "SendACMethod()", Message = "Session is null" };
                return msg;
            }
            if (CmdDBNo <= 0)
            {
                MsgWithDetails msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "SendACMethod()", Message = "CmdDBNo is zero" };
                return msg;
            }
            if (!IsReadyForSending)
            {
                MsgWithDetails msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "SendACMethod()", Message = "Session is not ready for writing" };
                return msg;
            }
            bool sended = false;
            ACChildInstanceInfo childInfo = new ACChildInstanceInfo(ParentACComponent);
            object result = this.Session.ACUrlCommand("!SendObject", acMethod, CmdDBNo, CmdDBOffset, childInfo);
            if (result != null)
                sended = (bool)result;

            if (!sended)
            {
                MsgWithDetails msg = new MsgWithDetails()
                {
                    Source = this.GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "SendACMethod()",
                    Message = string.Format("ACMethod was not sended. Method name: {0}. Please check if connection is established.", acMethod == null ? "-" : acMethod.ACIdentifier)
                };
                return msg;
            }
            else if (IsSimulationOn && _ResponseValue != null && _ResponseValue.Bit00_ToleranceError)
                _ResponseValue.Bit00_ToleranceError = false;
            return null;
        }

        public override PAProcessFunction.CompleteResult ReceiveACMethodResult(PAProcessFunction sender, ACMethod acMethod, out MsgWithDetails msg)
        {
            if (Session == null)
            {
                msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "ReceiveACMethodResult()", Message = "Session is null" };
                return IsSimulationOn ? PAProcessFunction.CompleteResult.Succeeded : PAProcessFunction.CompleteResult.FailedAndWait;
            }
            if (StateDBNo <= 0 && !IsSimulationOn)
            {
                msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "ReceiveACMethodResult()", Message = "StateDBNo is zero" };
                return PAProcessFunction.CompleteResult.FailedAndWait;
            }
            if (!IsReadyForReading)
            {
                msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "SendACMethod()", Message = "Session is not ready for writing" };
                return IsSimulationOn ? PAProcessFunction.CompleteResult.Succeeded : PAProcessFunction.CompleteResult.FailedAndWait;
            }

            if (acMethod == null)
            {
                msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "SendACMethod()", Message = "acMethod is null" };
                return PAProcessFunction.CompleteResult.FailedAndWait;
            }

            ACChildInstanceInfo childInfo = new ACChildInstanceInfo(ParentACComponent);
            object[] miscParams = new object[] { childInfo, false };

            ACMethod result = this.Session.ACUrlCommand("!ReadObject", acMethod, StateDBNo, StateDBOffset, miscParams) as ACMethod;
            if (result == null)
            {
                msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "ReceiveACMethodResult()", Message = "ACMethod-Result was not received. Please check if connection is established." };
                return IsSimulationOn ? PAProcessFunction.CompleteResult.Succeeded : PAProcessFunction.CompleteResult.FailedAndWait;
            }
            // If acMethod was serilized over network, we don't receive the original object. Therefore the results mus be copied
            if (result != acMethod)
                acMethod.ResultValueList = result.ResultValueList;
            msg = null;
            return PAProcessFunction.CompleteResult.Succeeded;
        }

        //protected override void Response_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        //{
        //    if (phase == ACPropertyChangedPhase.AfterBroadcast)
        //        return;
        //    base.Response_PropertyChanged(sender, e, phase);
        //    if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
        //    {
        //        if (IsTriggered != null) IsTriggered.ValueT = Response.ValueT.Bit01_Triggered;
        //    }
        //}
        protected virtual void Response_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource)
                && (e.ValueEvent.InvokerInfo != null || this.Root.Initialized))
            {
                _InitState |= ConvInitState.ResponseInitialized;

                bool changed = false;
                if (Malfunction != null)
                {
                    if (_ResponseValue.Bit14_Malfunction)
                        Malfunction.ValueT = PANotifyState.AlarmOrFault;
                    else
                    {
                        if (_RequestValue.Bit14_Acknowledge)
                        { 
                            _RequestValue.Bit14_Acknowledge = false;
                            changed = true;
                        }
                        Malfunction.ValueT = PANotifyState.Off;
                    }
                }
                if (StateTolerance != null)
                {
                    if (_ResponseValue.Bit00_ToleranceError)
                        StateTolerance.ValueT = PANotifyState.AlarmOrFault;
                    else
                    {
                        if (_RequestValue.Bit00_ToleranceAck)
                        { 
                            _RequestValue.Bit00_ToleranceAck = false;
                            changed = true;
                        }
                        StateTolerance.ValueT = PANotifyState.Off;
                    }
                }
                if (StateLackOfMaterial != null)
                {
                    if (_ResponseValue.Bit01_DosingtimeError)
                        StateLackOfMaterial.ValueT = PANotifyState.AlarmOrFault;
                    else
                    {
                        if (_RequestValue.Bit01_DosingtimeAck)
                        { 
                            _RequestValue.Bit01_DosingtimeAck = false;
                            changed = true;
                        }
                        StateLackOfMaterial.ValueT = PANotifyState.Off;
                    }
                }
                if (StateDestinationFull != null)
                {
                    if (_ResponseValue.Bit06_DestinationFull)
                        StateDestinationFull.ValueT = PANotifyState.AlarmOrFault;
                    else
                        StateDestinationFull.ValueT = PANotifyState.Off;
                }

                if (_RequestValue.Bit04_PrepareSourceSwitching && _ResponseValue.Bit03_SourceSwitchable)
                {
                    PAFDosing dosing = ParentACComponent as PAFDosing;
                    if (dosing != null 
                        && (dosing.CurrentACState == ACStateEnum.SMRunning || dosing.CurrentACState == ACStateEnum.SMPaused))
                    {
                        var appManager = dosing.ApplicationManager;
                        if (appManager != null && appManager.ApplicationQueue != null)
                        {
                            appManager.ApplicationQueue.Add(() =>
                            {
                                dosing.SetAbortReasonMalfunctionForced();
                            });
                        }
                    }
                }

                if (changed)
                {
                    try
                    {
                        Messages.LogDebug(this.GetACUrl(), "Response_PropertyChanged(_RequestValue)", String.Format("0x{0:x}", _RequestValue.ValueT));
                        if (LoggingEnabled)
                            Messages.LogDebug(this.GetACUrl(), "Response_PropertyChanged(_RequestValue)", _RequestValue.DumpBitAccessMapWithValues());
                    }
                    catch (Exception ec)
                    {
                        string msg = ec.Message;
                        if (ec.InnerException != null && ec.InnerException.Message != null)
                            msg += " Inner:" + ec.InnerException.Message;

                        Messages.LogException("GIPConv2006Vario", "Response_PropertyChanged", msg);
                    }
                }

                if (!((_InitState & ConvInitState.RequestInitialized) == ConvInitState.RequestInitialized))
                    return;
                _InitState |= ConvInitState.ACStateRestored;
                if (LoggingEnabled)
                {
                    Messages.LogDebug(this.GetACUrl(), "Response_PropertyChanged(_ResponseValue)", String.Format("0x{0:x}", _ResponseValue.ValueT));
                    Messages.LogDebug(this.GetACUrl(), "Response_PropertyChanged(_ResponseValue)", _ResponseValue.DumpBitAccessMapWithValues());
                }
                SyncACStateFromPLC(1,sender,e,phase);
            }
        }

        private void SyncACStateFromPLC(short callerID, object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            ACStateEnum newACState = TranslateToACStateFromBits();
            ACStateEnum suggestedNextACState = GetNextACState(true);
            if (ACState.ValueT != ACStateEnum.SMStopping
                && ACState.ValueT != ACStateEnum.SMStopped
                && ACState.ValueT != ACStateEnum.SMCompleted
                && ACState.ValueT != ACStateEnum.SMResetting
                && ACState.ValueT != ACStateEnum.SMAborting
                && ACState.ValueT != ACStateEnum.SMAborted
                && newACState == ACStateEnum.SMResetting)
                return;
            if (ACState.ValueT != suggestedNextACState && newACState == suggestedNextACState)
            {
                //if (newACState == PABaseState.SMIdle && (ACState.ValueT == PABaseState.SMStarting || ACState.ValueT == PABaseState.SMRunning))
                //{
                //    System.Diagnostics.Debugger.Break();
                //}
                if (LoggingEnabled)
                    Messages.LogDebug(this.GetACUrl(), "SyncACStateFromPLC(0)", String.Format("Changing ACState to: {0}", suggestedNextACState));

                (ACState as ACPropertyNetServerBase<ACStateEnum>).ChangeValueServer(ACState.ForceBroadcast, e.ValueEvent.InvokerInfo, suggestedNextACState);
                //ACState.ValueT = suggestedNextACState;
            }
            // Falls kein Reset ausgelöst, dann forciere erneut um den Schritt zu beenden (Passiert meist beim Hochfahren)
            else if (newACState == suggestedNextACState
                    && ACState.ValueT == suggestedNextACState
                    && (ACState.ValueT == ACStateEnum.SMCompleted || ACState.ValueT == ACStateEnum.SMAborted || ACState.ValueT == ACStateEnum.SMStopped))
            {
                var appManager = this.ApplicationManager;
                if (appManager != null && appManager.ApplicationQueue != null)
                {
                    appManager.ApplicationQueue.Add(() =>
                    {
                        if (newACState == suggestedNextACState
                        && ACState.ValueT == suggestedNextACState
                        && (ACState.ValueT == ACStateEnum.SMCompleted || ACState.ValueT == ACStateEnum.SMAborted || ACState.ValueT == ACStateEnum.SMStopped))
                        {
                            PAProcessFunction function = ParentACComponent as PAProcessFunction;
                            if (function != null)
                            {
                                if (ACState.ValueT == ACStateEnum.SMCompleted)
                                    function.SMCompleted();
                                else if (ACState.ValueT == ACStateEnum.SMAborted)
                                    function.SMAborted();
                                else if (ACState.ValueT == ACStateEnum.SMStopped)
                                    function.SMStopped();
                            }
                        }
                    });
                }
            }
        }

        //private bool _LockResend_ReqRunState = false;
        protected virtual void Request_PropertyChanged(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource)
                && (e.ValueEvent.InvokerInfo != null || this.Root.Initialized))
            {
                //_LockResend_ReqRunState = true;
                _InitState |= ConvInitState.RequestInitialized;
                if (!((_InitState & ConvInitState.ResponseInitialized) == ConvInitState.ResponseInitialized))
                    return;
                if (!((_InitState & ConvInitState.ACStateRestored) == ConvInitState.ACStateRestored))
                {
                    _InitState |= ConvInitState.ACStateRestored;
                    if (LoggingEnabled)
                    {
                        Messages.LogDebug(this.GetACUrl(), "Request_PropertyChanged(_RequestValue)", String.Format("0x{0:x}", _RequestValue.ValueT));
                        Messages.LogDebug(this.GetACUrl(), "Request_PropertyChanged(_RequestValue)", _RequestValue.DumpBitAccessMapWithValues());
                    }
                    SyncACStateFromPLC(2, sender, e, phase);
                }

                //AckMalfunction.ValueT = _RequestValue.Bit14_Acknowledge;
                //FaultAckTolerance.ValueT = _RequestValue.Bit00_ToleranceAck;
                //FaultAckLackOfMaterial.ValueT = _RequestValue.Bit01_DosingtimeAck;

                //ReqZeroSet.ValueT = _RequestValue.Bit07_SetZero;
                //_LockResend_ReqRunState = false;
            }
        }

        protected override void ModelProperty_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (sender == ACState)
            {
                if (phase == ACPropertyChangedPhase.BeforeBroadcast || ACState.InRestorePhase)
                    return;
                //ACPropertyValueEvent<String> valueEventT = e.ValueEvent as ACPropertyValueEvent<String>;
                //if (valueEventT.Sender == EventRaiser.Target)
                //    return;
                //SetRequestForACState(ACState.ValueT);
            }

            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            bool requestChanged = false;
            if (sender == AckMalfunction)
            {
                if (AckMalfunction.ValueT == true)
                {
                    _RequestValue.Bit14_Acknowledge = AckMalfunction.ValueT;
                    requestChanged = true;
                }
            }
            else if (sender == FaultAckDosingTime)
            {
                if (FaultAckDosingTime.ValueT == true)
                { 
                    _RequestValue.Bit01_DosingtimeAck = FaultAckDosingTime.ValueT;
                    requestChanged = true;
                }
            }
            else if (sender == FaultAckLackOfMaterial)
            {
                if (FaultAckLackOfMaterial.ValueT == true)
                { 
                    _RequestValue.Bit01_DosingtimeAck = FaultAckLackOfMaterial.ValueT;
                    requestChanged = true;
                }
            }
            else if (sender == FaultAckTolerance)
            {
                if (FaultAckTolerance.ValueT == true)
                { 
                    _RequestValue.Bit00_ToleranceAck = FaultAckTolerance.ValueT;
                    requestChanged = true;
                }
            }
            else if (sender == FaultAckDestinationFull)
            {
                if (FaultAckDestinationFull.ValueT == true)
                { 
                    _RequestValue.Bit14_Acknowledge = FaultAckDestinationFull.ValueT;
                    requestChanged = true;
                }
            }

            if (requestChanged && LoggingEnabled)
            {
                Messages.LogDebug(this.GetACUrl(), "ModelProperty_ValueUpdatedOnReceival(_RequestValue)", String.Format("0x{0:x}", _RequestValue.ValueT));
                Messages.LogDebug(this.GetACUrl(), "ModelProperty_ValueUpdatedOnReceival(_RequestValue)", _RequestValue.DumpBitAccessMapWithValues());
            }
        }

        protected void SetRequestForACState(ACStateEnum nextACState)
        {
            bool changed = false;
            switch (nextACState)
            {
                case ACStateEnum.SMRunning:
                    if (!_RequestValue.Bit08_Start)
                    {
                        _RequestValue.Bit08_Start = true;
                        PAFMixing mixing = ParentACComponent as PAFMixing;
                        if (mixing != null && mixing.CurrentACMethod != null && mixing.CurrentACMethod.ValueT != null)
                        {
                            var acValue = mixing.CurrentACMethod.ValueT.ParameterValueList.GetACValue("SwitchOff");
                            if (acValue != null && acValue.ParamAsBoolean)
                                _RequestValue.Bit12_Stop = true;
                        }
                        changed = true;
                    }
                    break;
                
                case ACStateEnum.SMPausing:
                case ACStateEnum.SMHolding:
                    if (!_RequestValue.Bit09_Pause)
                        changed = true;
                    _RequestValue.Bit09_Pause = true;

                    if (_RequestValue.Bit10_Continue)
                        changed = true;
                    _RequestValue.Bit10_Continue = false;

                    // Zusätzlicher Trigger um sicher zu sein, dass Kommando auch abgesendet wird, falls der Kommandostatus in der SPS untershciedlich sein sollte
                    if (!changed)
                        _RequestValue.Bit02 = !_RequestValue.Bit02;
                    break;
                
                case ACStateEnum.SMResuming:
                case ACStateEnum.SMRestarting:
                    if (!_RequestValue.Bit10_Continue)
                        changed = true;
                    _RequestValue.Bit10_Continue = true;
                    
                    if (_RequestValue.Bit09_Pause)
                        changed = true;
                    _RequestValue.Bit09_Pause = false;

                    // Zusätzlicher Trigger um sicher zu sein, dass Kommando auch abgesendet wird, falls der Kommandostatus in der SPS untershciedlich sein sollte
                    if (!changed)
                        _RequestValue.Bit02 = !_RequestValue.Bit02;
                    break;
                
                case ACStateEnum.SMAborting:
                    if (!_RequestValue.Bit11_Break)
                        changed = true;
                    if (ConverterBehaviour == 1 && !_RequestValue.Bit09_Pause)
                        _RequestValue.Bit09_Pause = true;
                    _RequestValue.Bit11_Break = true;
                    _RequestValue.Bit04_PrepareSourceSwitching = false;

                    // Zusätzlicher Trigger um sicher zu sein, dass Kommando auch abgesendet wird, falls der Kommandostatus in der SPS untershciedlich sein sollte
                    if (!changed)
                        _RequestValue.Bit02 = !_RequestValue.Bit02;
                    break;
                
                case ACStateEnum.SMStopping:
                    if (!_RequestValue.Bit12_Stop)
                        changed = true;
                    _RequestValue.Bit12_Stop = true;
                    _RequestValue.Bit04_PrepareSourceSwitching = false;

                    // Zusätzlicher Trigger um sicher zu sein, dass Kommando auch abgesendet wird, falls der Kommandostatus in der SPS untershciedlich sein sollte
                    if (!changed)
                        _RequestValue.Bit02 = !_RequestValue.Bit02;
                    break;
                
                case ACStateEnum.SMResetting:
                case ACStateEnum.SMIdle:
                    if (_RequestValue.Bit08_Start)
                        changed = true;
                    _RequestValue.Bit08_Start = false;
                    
                    if (_RequestValue.Bit12_Stop)
                        changed = true;
                    _RequestValue.Bit12_Stop = false;
                    _RequestValue.Bit04_PrepareSourceSwitching = false;

                    if (_RequestValue.Bit11_Break)
                        changed = true;
                    _RequestValue.Bit11_Break = false;
                    
                    if (_RequestValue.Bit09_Pause)
                        changed = true;
                    _RequestValue.Bit09_Pause = false;
                    
                    if (_RequestValue.Bit10_Continue)
                        changed = true;
                    _RequestValue.Bit10_Continue = false;

                    // Zusätzlicher Trigger um sicher zu sein, dass Kommando auch abgesendet wird, falls der Kommandostatus in der SPS untershciedlich sein sollte
                    if (!changed)
                        _RequestValue.Bit02 = !_RequestValue.Bit02;
                    break;
            }
            if (changed)
            {
                try
                {
                    Messages.LogDebug(this.GetACUrl(), "SetRequestForACState(_RequestValue)", String.Format("0x{0:x}", _RequestValue.ValueT));
                    if (LoggingEnabled)
                        Messages.LogDebug(this.GetACUrl(), "SetRequestForACState(_RequestValue)", _RequestValue.DumpBitAccessMapWithValues());
                    if (_Request.CurrentChangeValueRequest != null)
                    {
                        IACPropertyNetValueEvent prevRequest = _Request.CurrentChangeValueRequest;
                        _RequestValue.Bit02 = !_RequestValue.Bit02;
                        Messages.LogDebug(this.GetACUrl(), "SetRequestForACState(1)", String.Format("CurrentChangeValueRequest 0x{0:x} not cleared. New Request started with 0x{1:x}", prevRequest.ChangedValue, _RequestValue.ValueT));
                    }
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    Messages.LogException("GIPConv2006Vario", "SetRequestForACState", msg);
                }
            }
        }

        protected void ResetRequestBitsForACState(ACStateEnum currentACState)
        {
            bool changed = false;
            switch (currentACState)
            {
                case ACStateEnum.SMRunning:
                case ACStateEnum.SMStopping:
                    if (_RequestValue.Bit10_Continue && !_ResponseValue.Bit09_Paused)
                    {
                        _RequestValue.Bit10_Continue = false;
                        changed = true;
                    }
                    if (_RequestValue.Bit09_Pause && !_ResponseValue.Bit09_Paused)
                    {
                        _RequestValue.Bit09_Pause = false;
                        changed = true;
                    }
                    break;
                //case PABaseState.SMPaused:
                //    if (_RequestValue.Bit09_Pause && _ResponseValue.Bit09_Paused)
                //        _RequestValue.Bit09_Pause = false;
                //    break;
                case ACStateEnum.SMIdle:
                    if (_RequestValue.Bit08_Start)
                        changed = true;
                    _RequestValue.Bit08_Start = false;

                    if (_RequestValue.Bit12_Stop)
                        changed = true;
                    _RequestValue.Bit12_Stop = false;

                    if (_RequestValue.Bit11_Break)
                        changed = true;
                    _RequestValue.Bit11_Break = false;

                    if (_RequestValue.Bit09_Pause)
                        changed = true;
                    _RequestValue.Bit09_Pause = false;

                    if (_RequestValue.Bit10_Continue)
                        changed = true;
                    _RequestValue.Bit10_Continue = false;

                    // Zusätzlicher Trigger um sicher zu sein, dass Kommando auch abgesendet wird, falls der Kommandostatus in der SPS untershciedlich sein sollte
                    if (!changed)
                        _RequestValue.Bit02 = !_RequestValue.Bit02;

                    break;
            }
            if (changed)
            {
                try
                {
                    Messages.LogDebug(this.GetACUrl(), "ResetRequestBitsForACState(_RequestValue)", String.Format("0x{0:x}", _RequestValue.ValueT));
                    if (LoggingEnabled)
                        Messages.LogDebug(this.GetACUrl(), "ResetRequestBitsForACState(_RequestValue)", _RequestValue.DumpBitAccessMapWithValues());
                    if (_Request.CurrentChangeValueRequest != null)
                    {
                        IACPropertyNetValueEvent prevRequest = _Request.CurrentChangeValueRequest;
                        _RequestValue.Bit02 = !_RequestValue.Bit02;
                        Messages.LogDebug(this.GetACUrl(), "ResetRequestBitsForACState(1)", String.Format("CurrentChangeValueRequest 0x{0:x} not cleared. New Request started with 0x{1:x}", prevRequest.ChangedValue, _RequestValue.ValueT));
                    }
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    Messages.LogException("GIPConv2006Vario", "ResetRequestBitsForACState", msg);
                }
            }

        }


        protected ACStateEnum TranslateToACStateFromBits()
        {
            ACStateEnum translatedState = ACStateEnum.SMIdle;
            // When Function is completed or aborted, than Bit08_Active is set to false before Bit08_Start is false.
            if (ConverterBehaviour == 1)
            {
                if (!_RequestValue.Bit08_Start && !_ResponseValue.Bit08_Active)
                {
                    if (_ResponseValue.Bit11_Breaked || _ResponseValue.Bit15_Completed)
                        translatedState = ACStateEnum.SMResetting;
                    else
                         translatedState = ACStateEnum.SMIdle;
                }
                else if (_RequestValue.Bit08_Start 
                    && !_ResponseValue.Bit08_Active 
                    && (ACState.ValueT == ACStateEnum.SMStarting || ACState.ValueT == ACStateEnum.SMIdle))
                {
                    translatedState = ACStateEnum.SMStarting;
                }
                else if (       _RequestValue.Bit08_Start 
                            && (     _ResponseValue.Bit08_Active
                                || (!_ResponseValue.Bit08_Active && ACState.ValueT != ACStateEnum.SMStarting && ACState.ValueT != ACStateEnum.SMIdle))
                        )
                {
                    translatedState = ACStateEnum.SMRunning;

                    if (_RequestValue.Bit12_Stop && !_ResponseValue.Bit15_Completed)
                        translatedState = ACStateEnum.SMStopping;
                    else if (_RequestValue.Bit12_Stop && _ResponseValue.Bit15_Completed)
                        translatedState = ACStateEnum.SMStopped;

                    if (translatedState != ACStateEnum.SMStopped)
                    {
                        if (_RequestValue.Bit11_Break && !_ResponseValue.Bit11_Breaked)
                            translatedState = ACStateEnum.SMAborting;
                        else if (_ResponseValue.Bit11_Breaked)
                            translatedState = ACStateEnum.SMAborted;

                        if (translatedState != ACStateEnum.SMAborted)
                        {
                            if (_ResponseValue.Bit15_Completed)
                                translatedState = ACStateEnum.SMCompleted;
                            if (translatedState == ACStateEnum.SMRunning)
                            {
                                if (_ResponseValue.Bit08_Active)
                                {
                                    if (_RequestValue.Bit09_Pause && !_ResponseValue.Bit09_Paused)
                                        translatedState = ACStateEnum.SMPausing;
                                    else if (_RequestValue.Bit10_Continue && _ResponseValue.Bit09_Paused)
                                        translatedState = ACStateEnum.SMResuming;
                                    else if (_ResponseValue.Bit09_Paused)
                                        translatedState = ACStateEnum.SMPaused;
                                }
                                else
                                {
                                    translatedState = ACStateEnum.SMCompleted;
                                }
                            }
                        }
                    }

                    else if (_ResponseValue.Bit15_Completed)
                        translatedState = ACStateEnum.SMCompleted;
                }
                else if (!_RequestValue.Bit08_Start && _ResponseValue.Bit08_Active)
                {
                    translatedState = ACStateEnum.SMResetting;
                }
            }
            // Standard: Bit08_Active will be set to false, when Bit08_Start is false
            else
            {
                if (!_RequestValue.Bit08_Start && !_ResponseValue.Bit08_Active)
                {
                    translatedState = ACStateEnum.SMIdle;
                }
                else if (_RequestValue.Bit08_Start && !_ResponseValue.Bit08_Active)
                {
                    translatedState = ACStateEnum.SMStarting;
                }
                else if (_RequestValue.Bit08_Start && _ResponseValue.Bit08_Active)
                {
                    translatedState = ACStateEnum.SMRunning;

                    if (_RequestValue.Bit12_Stop && !_ResponseValue.Bit15_Completed)
                        translatedState = ACStateEnum.SMStopping;
                    else if (_RequestValue.Bit12_Stop && _ResponseValue.Bit15_Completed)
                        translatedState = ACStateEnum.SMStopped;

                    if (translatedState != ACStateEnum.SMStopped)
                    {
                        if (_RequestValue.Bit11_Break && !_ResponseValue.Bit11_Breaked)
                            translatedState = ACStateEnum.SMAborting;
                        else if (_ResponseValue.Bit11_Breaked)
                            translatedState = ACStateEnum.SMAborted;

                        if (translatedState != ACStateEnum.SMAborted)
                        {
                            if (_ResponseValue.Bit15_Completed)
                                translatedState = ACStateEnum.SMCompleted;
                            if (translatedState == ACStateEnum.SMRunning)
                            {
                                if (_RequestValue.Bit09_Pause && !_ResponseValue.Bit09_Paused)
                                    translatedState = ACStateEnum.SMPausing;
                                else if (_RequestValue.Bit10_Continue && _ResponseValue.Bit09_Paused)
                                    translatedState = ACStateEnum.SMResuming;
                                else if (_ResponseValue.Bit09_Paused)
                                    translatedState = ACStateEnum.SMPaused;
                            }
                        }
                    }


                    //if (_RequestValue.Bit09_Pause && !_ResponseValue.Bit09_Paused)
                    //    translatedState = PABaseState.SMPausing;
                    //else if (_RequestValue.Bit10_Continue && _ResponseValue.Bit09_Paused)
                    //    translatedState = PABaseState.SMResuming;
                    //else if (_ResponseValue.Bit09_Paused)
                    //    translatedState = PABaseState.SMPaused;

                    //if (_RequestValue.Bit12_Stop && !_ResponseValue.Bit15_Completed)
                    //    translatedState = PABaseState.SMStopping;
                    //else if (_RequestValue.Bit12_Stop && _ResponseValue.Bit15_Completed)
                    //    translatedState = PABaseState.SMStopped;

                    //if (_RequestValue.Bit11_Break && !_ResponseValue.Bit11_Breaked)
                    //    translatedState = PABaseState.SMAborting;
                    //else if (_ResponseValue.Bit11_Breaked)
                    //    translatedState = PABaseState.SMAborted;

                    else if (_ResponseValue.Bit15_Completed)
                        translatedState = ACStateEnum.SMCompleted;
                }
                else if (!_RequestValue.Bit08_Start && _ResponseValue.Bit08_Active)
                {
                    translatedState = ACStateEnum.SMResetting;
                }
            }
            return translatedState;
        }

        public override void AcknowledgeAlarms()
        {
            if (!IsEnabledAcknowledgeAlarms())
                return;
            if (this.ConversionAlarm.ValueT == PANotifyState.AlarmOrFault)
            {
                base.AcknowledgeAlarms();
                _AcceptStateFromPLC = true;
                ConversionAlarm.ValueT = PANotifyState.Off;
                OnAlarmDisappeared(ConversionAlarm);
            }
        }

        public override void OnProjSpecFunctionEvent(PAProcessFunction sender, string eventName, params object[] projSpecParams)
        {
            if (sender is PAFDosing && eventName == "OnSiloStateChanged" && projSpecParams.Count() >= 2)
            {
                PAMSilo silo = projSpecParams[0] as PAMSilo;
                bool outwardEnabled = (bool)projSpecParams[1];
                if (_RequestValue != null)
                {
                    _RequestValue.Bit04_PrepareSourceSwitching = !outwardEnabled;
                    if (LoggingEnabled)
                    {
                        Messages.LogDebug(this.GetACUrl(), "OnProjSpecFunctionEvent(_RequestValue)", String.Format("0x{0:x}", _RequestValue.ValueT));
                        Messages.LogDebug(this.GetACUrl(), "OnProjSpecFunctionEvent(_RequestValue)", _RequestValue.DumpBitAccessMapWithValues());
                    }
                }
            }
            //else if (sender is PAFDischarging && eventName == "OnSiloStateChanged" && projSpecParams.Count() >= 2)
            //{
            //    PAMSilo silo = projSpecParams[0] as PAMSilo;
            //    bool outwardEnabled = (bool)projSpecParams[1];
            //    if (_RequestValue != null)
            //        _RequestValue.Bit04_PrepareSourceSwitching = !outwardEnabled;
            //}
        }

        #endregion
    }

}
