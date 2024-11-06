// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using System.ComponentModel;
using gip.core.processapplication;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PADosingAbortReason'}de{'PADosingAbortReason'}", Global.ACKinds.TACEnum)]
    public enum PADosingAbortReason : short
    {
        NotSet = 0,

        /// <summary>
        /// Dosing is aborted, Current component with the remaining quantity should not be dosed again
        /// </summary>
        CompCancelled = 1,

        /// <summary>
        /// Source is empty => Abort dosing, scale down other components and set batch plan to completed, End Batchplan
        /// Quelle ist leer => Dosiere nicht erneut, Skaliere restliche Komponenten und beende Batchplan
        /// </summary>
        EmptySourceEndBatchplan = 2,

        /// <summary>
        /// EmptySource => Abort dosing. Book Silo empty. Try again rest-dosing with next silo;   
        /// Quelle ist leer => Breche Dosierung ab. Buche Silo leer und starte Restdosierung  mit nächstem Silo
        /// </summary>
        EmptySourceNextSource = 3,

        /// <summary>
        /// EmptySource => Abort dosing. Book Silo empty. Adjust other components. Wait with next Batch
        /// Quelle ist leer => Breche Dosierung ab. Buche Silo leer. Passe restliche Komponenten an und warte mit dem Start vom nächsten Batch bis Komponente wieder verfügbar
        /// </summary>
        EmptySourceAbortAdjustOtherAndWait = 4,

        /// <summary>
        /// Machine malfunction => Abort dosing. Don't post Silo empty. Try again rest-dosing with next silo; 
        /// Technisches problem => Breche Dosierung ab. Buche Silo NICHT leer und starte Restdosierung  mit nächstem Silo
        /// </summary>
        MachineMalfunction = 5,

        /// <summary>
        /// Source is empty and extra target exists  => Abort dosing and activate Discharging to extra target, then start dosing all components again
        /// Materialmangel steht an und es gibt eine Sonderentleermöglichkeit => Breche Dosierung ab entleere in Sonderziel und dosiere alle Komponenten von vorne
        /// </summary>
        EndDosingThenDisThenNextComp = 6,

        /// <summary>
        /// Tolerance error and extra target exists  => Abort dosing and activate Discharging to extra target, then start dosing all components again
        /// Toleranzfehler steht an und es gibt eine Sonderentleermöglichkeit => Breche Dosierung ab entleere in Sonderziel und dosiere alle Komponenten von vorne
        /// </summary>
        EndTolErrorDosingThenDisThenNextComp = 7,

        /// <summary>
        /// Source is empty and discharging to extra target doesn't exist => Abort dosing and switch to emptying mode; 
        /// Materialmangel steht an und es gibt keine Sonderentleermöglichkeit => Breche Dosierung ab und fahre Anlage leer
        /// </summary>
        EndDosingThenDisThenEnd = 8,

        /// <summary>A
        /// Tolerance error and discharging to extra target doesn't exist => Abort dosing and switch to emptying mode; 
        /// Toleranzfehler steht an und es gibt keine Sonderentleermöglichkeit => Breche Dosierung ab und fahre Anlage leer
        /// </summary>
        EndTolErrorDosingThenDisThenEnd = 9,
    }


    public enum PADosingLastBatchEnum : short
    {
        None = 0,
        LastBatch = 1,
        LastComponent = 2,
        LastBatchAndComponent = 3
    }


    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Dosing'}de{'Dosieren'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, PWDosing.PWClassName, true)]
    public class PAFDosing : PAProcessFunction, IPAFuncReceiveMaterial, IPAFuncScaleConfig
    {
        #region Properties

        #region Tolerance State
        [ACPropertyBindingTarget(635, "Read from PLC", "en{'State of Tolerance'}de{'Status Toleranz'}", "", false, false, RemotePropID = 19)]
        public IACContainerTNet<PANotifyState> StateTolerance { get; set; }
        public void OnSetStateTolerance(IACPropertyNetValueEvent valueEvent)
        {
            PANotifyState newSensorState = (valueEvent as ACPropertyValueEvent<PANotifyState>).Value;
            if (FaultAckTolerance.ValueT && newSensorState != PANotifyState.AlarmOrFault)
                FaultAckTolerance.ValueT = false;
            if (newSensorState != StateTolerance.ValueT)
            {
                if (newSensorState == PANotifyState.AlarmOrFault)
                    _StateToleranceAlarmChanged = PAAlarmChangeState.NewAlarmOccurred;
                else if (newSensorState == PANotifyState.Off)
                    _StateToleranceAlarmChanged = PAAlarmChangeState.AlarmDisappeared;
            }
        }

        private PAAlarmChangeState _StateToleranceAlarmChanged = PAAlarmChangeState.NoChange;
        void StateTolerance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_StateToleranceAlarmChanged != PAAlarmChangeState.NoChange && e.PropertyName == Const.ValueT)
            {
                OnAlarmDisappeared(StateTolerance);
                if (_StateToleranceAlarmChanged == PAAlarmChangeState.NewAlarmOccurred)
                    OnNewAlarmOccurred(StateTolerance);
                else
                    OnAlarmDisappeared(StateTolerance);
                _StateToleranceAlarmChanged = PAAlarmChangeState.NoChange;
            }
        }
        [ACPropertyBindingTarget(654, "Write to PLC", "en{'Fault acknowledge Tolerance'}de{'Toleranzquittung'}", "", true, false, RemotePropID = 20)]
        public IACContainerTNet<bool> FaultAckTolerance { get; set; }
        #endregion

        #region LackOfMaterial State
        protected bool _LackOfMaterialForced = false;
        public bool IsLackOfMaterialForced
        {
            get
            {
                return _LackOfMaterialForced;
            }
        }

        [ACPropertyBindingTarget(636, "Read from PLC", "en{'State of lack of material'}de{'Status Materialmangel'}", "", false, false, RemotePropID = 21)]
        public IACContainerTNet<PANotifyState> StateLackOfMaterial { get; set; }
        public void OnSetStateLackOfMaterial(IACPropertyNetValueEvent valueEvent)
        {
            PANotifyState newSensorState = (valueEvent as ACPropertyValueEvent<PANotifyState>).Value;
            if (FaultAckLackOfMaterial.ValueT && newSensorState != PANotifyState.AlarmOrFault)
                FaultAckLackOfMaterial.ValueT = false;
            if (newSensorState != StateLackOfMaterial.ValueT)
            {
                if (newSensorState == PANotifyState.AlarmOrFault)
                    _StateLackOfMaterialAlarmChanged = PAAlarmChangeState.NewAlarmOccurred;
                else if (newSensorState == PANotifyState.Off)
                    _StateLackOfMaterialAlarmChanged = PAAlarmChangeState.AlarmDisappeared;
            }
        }

        private PAAlarmChangeState _StateLackOfMaterialAlarmChanged = PAAlarmChangeState.NoChange;
        void StateLackOfMaterial_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_StateLackOfMaterialAlarmChanged != PAAlarmChangeState.NoChange && e.PropertyName == Const.ValueT)
            {
                OnAlarmDisappeared(StateLackOfMaterial);
                if (_StateLackOfMaterialAlarmChanged == PAAlarmChangeState.NewAlarmOccurred)
                    OnNewAlarmOccurred(StateLackOfMaterial);
                else
                    OnAlarmDisappeared(StateLackOfMaterial);
                _StateLackOfMaterialAlarmChanged = PAAlarmChangeState.NoChange;
            }
        }
        [ACPropertyBindingTarget(655, "Write to PLC", "en{'Fault acknowledge lack of material'}de{'Materialmangelquittung'}", "", true, false, RemotePropID = 22)]
        public IACContainerTNet<bool> FaultAckLackOfMaterial { get; set; }

        [ACPropertyBindingSource(656, "", "en{'Dosing abortion reason'}de{'Grund des Dosierabbruchs'}", "", false, true)]
        public IACContainerTNet<PADosingAbortReason> DosingAbortReason { get; set; }

        protected string _ExtraDisTargetDest = null;
        [ACPropertyInfo(false, 9999)]
        public string ExtraDisTargetDest
        {
            get
            {
                return _ExtraDisTargetDest;
            }
            set
            {
                _ExtraDisTargetDest = value;
                OnPropertyChanged("ExtraDisTargetDest");
            }
        }

        #endregion

        #region DosingTime State
        [ACPropertyBindingTarget(637, "Read from PLC", "en{'State of dosingtime-fault'}de{'Status Dosierzeitfehler'}", "", false, false, RemotePropID = 23)]
        public IACContainerTNet<PANotifyState> StateDosingTime { get; set; }
        public void OnSetStateDosingTime(IACPropertyNetValueEvent valueEvent)
        {
            PANotifyState newSensorState = (valueEvent as ACPropertyValueEvent<PANotifyState>).Value;
            if (FaultAckDosingTime.ValueT && newSensorState != PANotifyState.AlarmOrFault)
                FaultAckDosingTime.ValueT = false;
            if (newSensorState != StateDosingTime.ValueT)
            {
                if (newSensorState == PANotifyState.AlarmOrFault)
                    _StateDosingTimeAlarmChanged = PAAlarmChangeState.NewAlarmOccurred;
                else if (newSensorState == PANotifyState.Off)
                    _StateDosingTimeAlarmChanged = PAAlarmChangeState.AlarmDisappeared;
            }
        }

        private PAAlarmChangeState _StateDosingTimeAlarmChanged = PAAlarmChangeState.NoChange;
        void StateDosingTime_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_StateDosingTimeAlarmChanged != PAAlarmChangeState.NoChange && e.PropertyName == Const.ValueT)
            {
                OnAlarmDisappeared(StateDosingTime);
                if (_StateDosingTimeAlarmChanged == PAAlarmChangeState.NewAlarmOccurred)
                    OnNewAlarmOccurred(StateDosingTime);
                else
                    OnAlarmDisappeared(StateDosingTime);
                _StateDosingTimeAlarmChanged = PAAlarmChangeState.NoChange;
            }
        }
        [ACPropertyBindingTarget(656, "Write to PLC", "en{'Fault acknowledge dosingtime-fault'}de{'Dosierzeitfehlerquittung'}", "", true, false, RemotePropID = 24)]
        public IACContainerTNet<bool> FaultAckDosingTime { get; set; }
        #endregion

        #region Result Validation
        private bool _QuantityIsZero = false;
        public bool QuantityIsZero
        {
            get
            {
                return _QuantityIsZero;
            }
        }

        private bool? _AckZeroQ_WasDosed = null;
        public bool? AckZeroQ_WasDosed
        {
            get
            {
                return _AckZeroQ_WasDosed;
            }
        }
        #endregion

        #region Misc

        public virtual PAEScaleBase CurrentScaleForWeighing
        {
            get
            {
                PAEScaleBase scale = ScaleMappingHelper.AssignedScales.FirstOrDefault();
                if (scale != null)
                    return scale;
                var queryScales = ParentACComponent.FindChildComponents<PAEScaleBase>(c => c is PAEScaleBase);
                if (!queryScales.Any())
                    return null;
                if (queryScales.Count == 1)
                    return queryScales.FirstOrDefault();
                var charLast = this.ACIdentifier.Last();
                var foundScale = queryScales.Where(c => c.ACIdentifier.Last() == charLast).FirstOrDefault();
                if (foundScale != null)
                    return foundScale;
                return queryScales.FirstOrDefault();
            }
        }

        private PAScaleMappingHelper<PAEScaleBase> _ScaleMappingHelper;
		public PAScaleMappingHelper<PAEScaleBase> ScaleMappingHelper
		{
			get
			{
                using (ACMonitor.Lock(_20015_LockValue))
                {
					if (_ScaleMappingHelper == null)
						_ScaleMappingHelper = new PAScaleMappingHelper<PAEScaleBase>(this, this);
                }
				return _ScaleMappingHelper;
			}
		}
		
		public const string VMethodName_Dosing = "Dosing";

        public PAMSilo CurrentDosingSilo
        {
            get
            {
                try
                {
                    if (IsTransportActive
                        && CurrentACMethod != null
                        && CurrentACMethod.ValueT != null)
                    {
                        return GetSourceSiloFromMethod(CurrentACMethod.ValueT);
                    }
                }
                catch (Exception e)
                {
                    if (Messages != null)
                        Messages.LogException(this.GetACUrl(), "CurrentDosingSilo", e);
                }
                return null;
            }
        }
        #endregion

        #endregion

        #region Config
        protected ACPropertyConfigValue<string> _FuncScaleConfig;
        [ACPropertyConfig("en{'Assigned Scales'}de{'Zugeordnete Waagen'}")]
        public string FuncScaleConfig
        {
            get
            {
                return _FuncScaleConfig.ValueT;
            }
        }

        protected ACPropertyConfigValue<bool> _StopOnSourceChange;
        [ACPropertyConfig("en{'Stop-Comand when changing source'}de{'Stop-Kommando bei Quellenwechsel'}")]
        public bool StopOnSourceChange
        {
            get
            {
                return _StopOnSourceChange.ValueT;
            }
        }

        protected ACPropertyConfigValue<short> _AutoAbortActionOnBlockedSilo;
        [ACPropertyConfig("en{'Automatic abort action on blocked source'}de{'Automatische Abbruchsaktion bei gesperrter Quelle'}")]
        public short AutoAbortActionOnBlockedSilo
        {
            get
            {
                return _AutoAbortActionOnBlockedSilo.ValueT;
            }
        }

        public PADosingAbortReason AutoAbortActionEnumOnBlockedSilo
        {
            get
            {
                try
                {
                    PADosingAbortReason reason = (PADosingAbortReason) AutoAbortActionOnBlockedSilo;
                    return reason;
                }
                catch
                {
                    return PADosingAbortReason.NotSet;
                }
            }
        }
        #endregion

        #region Constructors 

        static PAFDosing()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFDosing), ACStateConst.TMStart, CreateVirtualMethod(VMethodName_Dosing, "en{'Dosing'}de{'Dosieren'}", typeof(PWDosing)));
            RegisterExecuteHandler(typeof(PAFDosing), HandleExecuteACMethod_PAFDosing);
        }

        public PAFDosing(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _FuncScaleConfig = new ACPropertyConfigValue<string>(this, PAScaleMappingHelper<IACComponent>.FuncScaleConfigName, "");
            _StopOnSourceChange = new ACPropertyConfigValue<bool>(this, "StopOnSourceChange", false);
            _AutoAbortActionOnBlockedSilo = new ACPropertyConfigValue<short>(this, "AutoAbortActionOnBlockedSilo", 0);
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            StateTolerance.PropertyChanged += StateTolerance_PropertyChanged;
            StateLackOfMaterial.PropertyChanged += StateLackOfMaterial_PropertyChanged;
            StateDosingTime.PropertyChanged += StateDosingTime_PropertyChanged;
            _ = FuncScaleConfig;
            _ = StopOnSourceChange;
            _ = AutoAbortActionOnBlockedSilo;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            StateTolerance.PropertyChanged -= StateTolerance_PropertyChanged;
            StateLackOfMaterial.PropertyChanged -= StateLackOfMaterial_PropertyChanged;
            StateDosingTime.PropertyChanged -= StateDosingTime_PropertyChanged;
            using (ACMonitor.Lock(_20015_LockValue))
            {
                if (_ScaleMappingHelper != null)
                {
                    _ScaleMappingHelper.DetachAndRemove();
                    _ScaleMappingHelper = null;
                }
            }

            return base.ACDeInit(deleteACClassTask);
        }

        public override bool ACPostInit()
        {
            bool succ = base.ACPostInit();

            try
            {
                if (succ
                    && IsTransportActive
                    && CurrentACMethod != null
                    && CurrentACMethod.ValueT != null)
                {
                    PAMSilo pamSilo = GetSourceSiloFromMethod(CurrentACMethod.ValueT);
                    if (pamSilo != null)
                        pamSilo.SubscribeTransportFunction(this);
                }
            }
            catch (Exception e)
            {
                if (Messages != null)
                    Messages.LogException(this.GetACUrl(), "ACPostInit", e);
            }
            return succ;
        }

        #endregion

        #region Public

        #region State-Methods

        [ACMethodAsync("Process", "en{'Start'}de{'Start'}", (short)MISort.Start, false)]
        public override ACMethodEventArgs Start(ACMethod acMethod)
        {
            if (_LackOfMaterialForced)
                StateLackOfMaterial.ValueT = PANotifyState.Off;
            _LackOfMaterialForced = false;
            return base.Start(acMethod);
        }

        public override void SMIdle()
        {
            base.SMIdle();
            if (CurrentACState == ACStateEnum.SMIdle)
            {
                if (_LackOfMaterialForced)
                    StateLackOfMaterial.ValueT = PANotifyState.Off;
                _LackOfMaterialForced = false;
            }
        }

        public override void SMStarting()
        {
            ExtraDisTargetDest = null;
            DosingAbortReason.ValueT = PADosingAbortReason.NotSet;
            _AckZeroQ_WasDosed = null;
            _QuantityIsZero = false;
            base.SMStarting();
        }

        public override void Resume()
        {
            PWDosing invokingDosNode = null;
            if (this.CurrentTask != null && CurrentACMethod.ValueT != null)
                invokingDosNode = this.CurrentTask.ValueT as PWDosing;

            if (invokingDosNode != null)
            {
                Msg msg = invokingDosNode.CanResumeDosing();
                if (msg != null)
                {
                    if (FunctionError.ValueT == PANotifyState.Off)
                        Messages.LogException(this.GetACUrl(), "Resume(1)", msg.Message);
                    FunctionError.ValueT = PANotifyState.AlarmOrFault;
                    OnNewAlarmOccurred(FunctionError, new Msg(msg.Message, this, eMsgLevel.Exception, "PAFDosing", "Resume", 1110), true);
                    return;
                }
            }

            base.Resume();
        }

        public override void Abort()
        {
            if (!IsEnabledAbort())
                return;
            if (DosingAbortReason.ValueT == PADosingAbortReason.NotSet)
                DosingAbortReason.ValueT = PADosingAbortReason.CompCancelled;
            Messages.LogDebug(this.GetACUrl(), "Abort(DosingAbortReason)", DosingAbortReason.ValueT.ToString());
            base.Abort();
        }

        public override void Stopp()
        {
            if (!IsEnabledStopp())
                return;
            if (DosingAbortReason.ValueT == PADosingAbortReason.NotSet)
                DosingAbortReason.ValueT = PADosingAbortReason.CompCancelled;
            Messages.LogDebug(this.GetACUrl(), "Stopp(DosingAbortReason)", DosingAbortReason.ValueT.ToString());
            base.Stopp();
        }

        #endregion


        #region Interactionmethods on StateLackOfMaterial

        #region Book silo empty then change source
        /// <summary>
        /// EmptySource => Abort dosing. Post Silo empty. Try again rest-dosing with next silo;   
        /// Quelle ist leer => Breche Dosierung ab. Buche Silo leer und starte Restdosierung  mit nächstem Silo
        /// </summary>
        [ACMethodInteraction("", "en{'Source/Silo/Cell/Tank is empty => Change source'}de{'Quelle/Silo/Zelle/Tank ist leer => Quellenwechsel'}", 800, true)]
        public virtual void SetAbortReasonEmpty()
        {
            if (!IsEnabledSetAbortReasonEmpty())
                return;
            SetAbortReasonEmptyForced();
        }

        public virtual void SetAbortReasonEmptyForced()
        {
            DosingAbortReason.ValueT = PADosingAbortReason.EmptySourceNextSource;
            OnSourceChangeStoppOrAbort();
            AcknowledgeAlarms();
        }

        public virtual bool IsEnabledSetAbortReasonEmpty()
        {
            if (   (this.StateLackOfMaterial.ValueT == PANotifyState.Off && this.Malfunction.ValueT == PANotifyState.Off)
                || (CurrentACState != ACStateEnum.SMRunning && CurrentACState != ACStateEnum.SMPaused && CurrentACState != ACStateEnum.SMHeld))
                return false;
            return true; // Erlaubt nocheinmal zu setzen falls Komando nicht an SPS angekommen ist
            //if (DosingAbortReason.ValueT == PADosingAbortReason.NotSet)
            //    return true;
            //return false;
        }

        public static bool AskUserSetAbortReasonEmpty(IACComponent acComponent)
        {
            return ValidateRemainingStock(acComponent);
        }

        private static bool ValidateRemainingStock(IACComponent acComponent)
        {
            if (acComponent == null)
                return false;
            DosingRestInfo restInfo = acComponent.ExecuteMethod("GetDosingRestInfo", null) as DosingRestInfo;
            if (restInfo == null)
                return true;
            if (restInfo.IsZeroTolSet && !restInfo.InZeroTolerance)
            {
                // "Question50067": Es wurden {0:F3} kg aus der Quelle {1} dosiert, die zur Zeit einen Bestand von {2:F3} kg hat. Damit bleibt ein Rest von {3:F3} kg übrig, der größer ist als die eingestellte Leertoleranz von {4:F3} kg. Möchten Sie die Quelle tatsächlich leer buchen?
                // "Question50067": {0:F3} kg were dosed from source {1}, which currently has a stock of {2:F3} kg. This leaves a residue of {3:F3} kg, which is greater than the set empty tolerance of {4:F3} kg. Do you really want to post the source empty?
                Global.MsgResult questionRes = acComponent.Messages.Question(acComponent, "Question50067", Global.MsgResult.Yes, false, restInfo.DosedQuantity, restInfo.FacilityName, restInfo.Stock, restInfo.RemainingStock, restInfo.ZeroTol);
                if (questionRes == Global.MsgResult.Yes)
                    return true;
                return false;
            }
            return true;
        }

        [ACMethodInfo("", "en{'Remaining stock in silo'}de{'Restinhalt im Silo'}", 9999)]
        public DosingRestInfo GetDosingRestInfo()
        {
            var acMethod = CurrentACMethod.ValueT;
            if (acMethod == null)
                return null;

            PAMSilo currentSource = GetSourceSiloFromMethod(acMethod);
            if (currentSource == null)
                return null;

            DosingRestInfo restInfo = new DosingRestInfo(currentSource, this, null, IsSourceMarkedAsEmpty);
            return restInfo;
        }

        public bool IsSourceMarkedAsEmpty
        {
            get
            {
                return DosingAbortReason.ValueT == PADosingAbortReason.EmptySourceNextSource
                    || DosingAbortReason.ValueT == PADosingAbortReason.EmptySourceEndBatchplan
                    || DosingAbortReason.ValueT == PADosingAbortReason.EndDosingThenDisThenEnd
                    || DosingAbortReason.ValueT == PADosingAbortReason.EndDosingThenDisThenNextComp
                    || DosingAbortReason.ValueT == PADosingAbortReason.EmptySourceAbortAdjustOtherAndWait;
            }
        }

        #endregion

        #region Change source, don't book silo empty
        /// <summary>
        /// Machine malfunction => Abort dosing. Don't book Silo empty. Try again rest-dosing with next silo; 
        /// Technisches problem => Breche Dosierung ab. Buche Silo NICHT leer und starte Restdosierung  mit nächstem Silo
        /// </summary>
        [ACMethodInteraction("", "en{'Machine malfuncton => change source'}de{'Anlagenstörung => Quellenwechsel'}", 802, true)]
        public virtual void SetAbortReasonMalfunction()
        {
            if (!IsEnabledSetAbortReasonMalfunction())
                return;
            SetAbortReasonMalfunctionForced();
        }

        public virtual void SetAbortReasonMalfunctionForced()
        {
            DosingAbortReason.ValueT = PADosingAbortReason.MachineMalfunction;
            OnSourceChangeStoppOrAbort();
            AcknowledgeAlarms();
        }

        public virtual bool IsEnabledSetAbortReasonMalfunction()
        {
            if (   (this.StateLackOfMaterial.ValueT == PANotifyState.Off && this.Malfunction.ValueT == PANotifyState.Off)
                || (CurrentACState != ACStateEnum.SMRunning && CurrentACState != ACStateEnum.SMPaused && CurrentACState != ACStateEnum.SMHeld))
                return false;
            return true; // Erlaubt nocheinmal zu setzen falls Komando nicht an SPS angekommen ist
            //if (DosingAbortReason.ValueT == PADosingAbortReason.NotSet)
            //    return true;
            //return false;
        }
        #endregion

        #region Cancel dosing, Discharge, dose next component
        /// <summary>
        /// Lack of Material and extra target exists  => Abort dosing and activate Discharging to extra target, then start dosing all components again
        /// Materialmangel steht an und es gibt eine Sonderentleermöglichkeit => Breche Dosierung ab entleere in Sonderziel und dosiere alle Komponenten von vorne
        /// </summary>
        [ACMethodInteraction("", "en{'Cancel dosing>alt. discharging>dose comp. again'}de{'Dosierabbruch>Sonderentleerung>Komp. erneut dosieren'}", 803, true)]
        public virtual void EndDosDisNextComp()
        {
            if (!IsEnabledEndDosDisNextComp())
                return;
            EndDosDisNextCompForced();
        }

        public virtual void EndDosDisNextCompForced()
        {
            DosingAbortReason.ValueT = PADosingAbortReason.EndDosingThenDisThenNextComp;
            OnSourceChangeStoppOrAbort();
            AcknowledgeAlarms();
        }

        public virtual bool IsEnabledEndDosDisNextComp()
        {
            if (   (this.StateLackOfMaterial.ValueT == PANotifyState.Off && this.Malfunction.ValueT == PANotifyState.Off)
                || (CurrentACState != ACStateEnum.SMRunning && CurrentACState != ACStateEnum.SMPaused && CurrentACState != ACStateEnum.SMHeld))
                return false;
            return true; // Erlaubt nocheinmal zu setzen falls Komando nicht an SPS angekommen ist
            //if (DosingAbortReason.ValueT == PADosingAbortReason.NotSet)
            //    return true;
            //return false;
        }

        public static bool AskUserEndDosDisNextComp(IACComponent acComponent)
        {
            if (acComponent == null)
                return false;
            if (!ValidateRemainingStock(acComponent))
                return false;
            // "Question50022": Do you want do cancel the current Dosing, then discharge to a alternate target and then dose the next component?
            Global.MsgResult questionRes = acComponent.Messages.Question(acComponent, "Question50022", Global.MsgResult.Yes);
            if (questionRes == Global.MsgResult.Yes)
            {
                // "Question50033" Please enter the Facility-No. or the address (ACUrl) of the alternative Target if you want to reject the batch. If you wan't to transport it to the originally planned target leave it blank.?
                PWMethodVBBase.EnterExtraDisTargetDest(acComponent, "Question50034");
                return true;
            }
            return false;
        }
        #endregion

        #region Cancel dosing, Discharge, End Batch
        /// <summary>
        /// Source is empty and discharging to extra target doesn't exist => Abort dosing and switch to emptying mode; 
        /// Materialmangel steht an und es gibt keine Sonderentleermöglichkeit => Breche Dosierung ab und fahre Anlage leer
        /// </summary>
        [ACMethodInteraction("", "en{'Cancel dosing>Emptying mode>ending'}de{'Dosierabbruch>Leerfahren>Ende'}", 803, true)]
        public virtual void EndDosDisEnd()
        {
            if (!IsEnabledEndDosDisEnd())
                return;
            EndDosDisEndForced();
        }

        public virtual void EndDosDisEndForced()
        {
            DosingAbortReason.ValueT = PADosingAbortReason.EndDosingThenDisThenEnd;
            OnSourceChangeStoppOrAbort();
            AcknowledgeAlarms();
        }

        public virtual bool IsEnabledEndDosDisEnd()
        {
            if (   (this.StateLackOfMaterial.ValueT == PANotifyState.Off && this.Malfunction.ValueT == PANotifyState.Off)
                || (CurrentACState != ACStateEnum.SMRunning && CurrentACState != ACStateEnum.SMPaused && CurrentACState != ACStateEnum.SMHeld))
                return false;
            return true; // Erlaubt nocheinmal zu setzen falls Komando nicht an SPS angekommen ist
            //if (DosingAbortReason.ValueT == PADosingAbortReason.NotSet)
            //    return true;
            //return false;
        }

        public static bool AskUserEndDosDisEnd(IACComponent acComponent)
        {
            if (acComponent == null)
                return false;
            if (!ValidateRemainingStock(acComponent))
                return false;
            // "Question50021": Do you want do cancel the current Dosing, then discharge to a alternate target and then cancel the batch?
            Global.MsgResult questionRes = acComponent.Messages.Question(acComponent, "Question50021", Global.MsgResult.Yes);
            if (questionRes == Global.MsgResult.Yes)
            {
                // "Question50033" Please enter the Facility-No. or the address (ACUrl) of the alternative Target if you want to reject the batch. If you wan't to transport it to the originally planned target leave it blank.?
                PWMethodVBBase.EnterExtraDisTargetDest(acComponent, "Question50034");
                return true;
            }
            return false;
        }
        #endregion

        #region Cancel dosing, Scale remaining Components, End Batch and End order
        /// <summary>
        /// Source is empty => Abort dosing, scale down other components and set batch plan to completed, End Batchplan
        /// Quelle ist leer => Dosiere nicht erneut, Skaliere restliche Komponenten und beende Batchplan
        /// </summary>
        [ACMethodInteraction("", "en{'Cancel dosing>Adapt remaining comp.>End order'}de{'Dosierabbruch>Restl.Komp.Anpassen>Auftragsende'}", 803, true)]
        public virtual void EndDosEndOrder()
        {
            if (!IsEnabledEndDosEndOrder())
                return;
            EndDosEndOrderForced();
        }

        public virtual void EndDosEndOrderForced()
        {
            DosingAbortReason.ValueT = PADosingAbortReason.EmptySourceEndBatchplan;
            OnSourceChangeStoppOrAbort();
            AcknowledgeAlarms();
        }

        public virtual bool IsEnabledEndDosEndOrder()
        {
            if (   (this.StateLackOfMaterial.ValueT == PANotifyState.Off && this.Malfunction.ValueT == PANotifyState.Off)
                || (CurrentACState != ACStateEnum.SMRunning && CurrentACState != ACStateEnum.SMPaused && CurrentACState != ACStateEnum.SMHeld))
                return false;
            return true; // Erlaubt nocheinmal zu setzen falls Komando nicht an SPS angekommen ist
            //if (DosingAbortReason.ValueT == PADosingAbortReason.NotSet)
            //    return true;
            //return false;
        }

        public static bool AskUserEndDosEndOrder(IACComponent acComponent)
        {
            return ValidateRemainingStock(acComponent);
            //if (acComponent == null)
            //    return false;
            //// "Question50021": Do you want do cancel the current Dosing, then discharge to a alternate target and then cancel the batch?
            //Global.MsgResult questionRes = acComponent.Messages.Question(acComponent, "Question50021", Global.MsgResult.Yes);
            //if (questionRes == Global.MsgResult.Yes)
            //{
            //    // "Question50033" Please enter the Facility-No. or the address (ACUrl) of the alternative Target if you want to reject the batch. If you wan't to transport it to the originally planned target leave it blank.?
            //    PWMethodVBBase.EnterExtraDisTargetDest(acComponent, "Question50034");
            //    return true;
            //}
            //return false;
        }
        #endregion

        #region Cancel dosing, Scale remaining Components, End Batch and wait
        /// <summary>
        /// EmptySource => Abort dosing. Book Silo empty. Adjust other components. Wait with next Batch
        /// Quelle ist leer => Breche Dosierung ab. Buche Silo leer. Passe restliche Komponenten an und warte mit dem Start vom nächsten Batch bis Komponente wieder verfügbar
        /// </summary>
        [ACMethodInteraction("", "en{'Cancel dosing>Adapt remaining comp.>Wait next batch'}de{'Dosierabbruch>Restl.Komp.Anpassen>Warte mit nächstem Batch'}", 804, true)]
        public virtual void EndDosAdjustRestWait()
        {
            if (!IsEnabledEndDosAdjustRestWait())
                return;
            EndDosAdjustRestWaitForced();
        }

        public virtual void EndDosAdjustRestWaitForced()
        {
            DosingAbortReason.ValueT = PADosingAbortReason.EmptySourceAbortAdjustOtherAndWait;
            OnSourceChangeStoppOrAbort();
            AcknowledgeAlarms();
        }

        public virtual bool IsEnabledEndDosAdjustRestWait()
        {
            if ((this.StateLackOfMaterial.ValueT == PANotifyState.Off && this.Malfunction.ValueT == PANotifyState.Off)
                || (CurrentACState != ACStateEnum.SMRunning && CurrentACState != ACStateEnum.SMPaused && CurrentACState != ACStateEnum.SMHeld))
                return false;
            return true; // Erlaubt nocheinmal zu setzen falls Komando nicht an SPS angekommen ist
            //if (DosingAbortReason.ValueT == PADosingAbortReason.NotSet)
            //    return true;
            //return false;
        }

        public static bool AskUserEndDosAdjustRestWait(IACComponent acComponent)
        {
            if (acComponent == null)
                return false;
            if (!ValidateRemainingStock(acComponent))
                return false;

            // "Question50021": Do you want do cancel the current Dosing, then discharge to a alternate target and then cancel the batch?
            Global.MsgResult questionRes = acComponent.Messages.Question(acComponent, "Question50021", Global.MsgResult.Yes);
            if (questionRes == Global.MsgResult.Yes)
            {
                // "Question50033" Please enter the Facility-No. or the address (ACUrl) of the alternative Target if you want to reject the batch. If you wan't to transport it to the originally planned target leave it blank.?
                PWMethodVBBase.EnterExtraDisTargetDest(acComponent, "Question50034");
                return true;
            }
            return false;
        }
        #endregion

        #region Force set lack of material
        [ACMethodInteraction("", "en{'Set lack of material'}de{'Materialmangel setzen'}", 820, true)]
        public virtual void ForceSetLackOfMaterial()
        {
            // Enabled, for calling from background
            //if (!IsEnabledForceSetLackOfMaterial())
            //    return;
            this.StateLackOfMaterial.ValueT = PANotifyState.AlarmOrFault;
            _LackOfMaterialForced = true;
            string user = "Service";
            if (Root.CurrentInvokingUser != null)
                user = Root.CurrentInvokingUser.VBUserName;
            Messages.LogDebug(this.GetACUrl(), "ForceSetLackOfMaterial()", user);
        }

        public virtual bool IsEnabledForceSetLackOfMaterial()
        {
            return CurrentACState == ACStateEnum.SMPaused && this.StateLackOfMaterial.ValueT == PANotifyState.Off;
        }
        #endregion

        #region Resert Abort Reason
        [ACMethodInteraction("", "en{'Reset Abortreason'}de{'Abbruchgrund zurücksetzen'}", 805, true)]
        public virtual void ResetAbortReason()
        {
            if (!IsEnabledResetAbortReason())
                return;
            DosingAbortReason.ValueT = PADosingAbortReason.NotSet;
            if (_LackOfMaterialForced && StateLackOfMaterial.ValueT != PANotifyState.Off)
            {
                StateLackOfMaterial.ValueT = PANotifyState.Off;
                AcknowledgeAlarms();
            }
        }

        public virtual bool IsEnabledResetAbortReason()
        {
            return DosingAbortReason != null && DosingAbortReason.ValueT != PADosingAbortReason.NotSet;
        }
        #endregion

        #endregion


        #region Interactionmethods on StateTolerance

        #region Cancel dosing, Discharge, dose next component
        /// <summary>
        /// Tolerance error and extra target exists  => Abort dosing and activate Discharging to extra target, then start dosing all components again
        /// Toleranzfehler steht an und es gibt eine Sonderentleermöglichkeit => Breche Dosierung ab entleere in Sonderziel und dosiere alle Komponenten von vorne
        /// </summary>
        [ACMethodInteraction("", "en{'Cancel dosing>alt. discharging>dose comp. again'}de{'Dosierabbruch>Sonderentleerung>Komp. erneut dosieren'}", 805, true)]
        public virtual void EndDosDisNextCompOnTol()
        {
            if (!IsEnabledEndDosDisNextCompOnTol())
                return;
            EndDosDisNextCompOnTolForced();
        }

        public virtual void EndDosDisNextCompOnTolForced()
        {
            DosingAbortReason.ValueT = PADosingAbortReason.EndTolErrorDosingThenDisThenNextComp;
            Abort();
        }

        public virtual bool IsEnabledEndDosDisNextCompOnTol()
        {
            if (this.StateTolerance.ValueT == PANotifyState.Off
                || (CurrentACState != ACStateEnum.SMRunning && CurrentACState != ACStateEnum.SMPaused && CurrentACState != ACStateEnum.SMHeld))
                return false;
            return true; // Erlaubt nocheinmal zu setzen falls Komando nicht an SPS angekommen ist
            //if (DosingAbortReason.ValueT == PADosingAbortReason.NotSet)
            //    return true;
            //return false;
        }

        public static bool AskUserEndDosDisNextCompOnTol(IACComponent acComponent)
        {
            if (acComponent == null)
                return false;
            // "Question50022": Do you want do cancel the current Dosing, then discharge to a alternate target and then dose the next component?
            Global.MsgResult questionRes = acComponent.Messages.Question(acComponent, "Question50022", Global.MsgResult.Yes);
            if (questionRes == Global.MsgResult.Yes)
            {
                // "Question50033" Please enter the Facility-No. or the address (ACUrl) of the alternative Target if you want to reject the batch. If you wan't to transport it to the originally planned target leave it blank.?
                PWMethodVBBase.EnterExtraDisTargetDest(acComponent, "Question50034");
                return true;
            }
            return false;
        }
        #endregion

        #region Cancel dosing, Discharge, End Batch
        /// <summary>
        /// Tolerance error and discharging to extra target doesn't exist => Abort dosing and switch to emptying mode; 
        /// Toleranzfehler steht an und es gibt keine Sonderentleermöglichkeit => Breche Dosierung ab und fahre Anlage leer
        /// </summary>
        [ACMethodInteraction("", "en{'Acknowledge tolerance>Emptying mode>ending'}de{'Toleranzquittung>Leerfahren>Ende'}", 804, true)]
        public virtual void EndDosDisEndOnTol()
        {
            if (!IsEnabledEndDosDisEndOnTol())
                return;
            EndDosDisEndOnTolForced();
        }

        public virtual void EndDosDisEndOnTolForced()
        {
            DosingAbortReason.ValueT = PADosingAbortReason.EndTolErrorDosingThenDisThenEnd;
            Abort();
            AcknowledgeAlarms();
        }

        public virtual bool IsEnabledEndDosDisEndOnTol()
        {
            if (this.StateTolerance.ValueT == PANotifyState.Off
                || (CurrentACState != ACStateEnum.SMRunning && CurrentACState != ACStateEnum.SMPaused && CurrentACState != ACStateEnum.SMHeld))
                return false;
            return true; // Erlaubt nocheinmal zu setzen falls Komando nicht an SPS angekommen ist
            //if (DosingAbortReason.ValueT == PADosingAbortReason.NotSet)
            //    return true;
            //return false;
        }

        public static bool AskUserEndDosDisEndOnTol(IACComponent acComponent)
        {
            if (acComponent == null)
                return false;

            ACComponent _this = acComponent as ACComponent;

            // "Question50021", Do you want do cancel the current Dosing, then discharge to a alternate target and then cancel the batch?
            Global.MsgResult questionRes = acComponent.Messages.Question(acComponent, "Question50021", Global.MsgResult.Yes);
            if (questionRes == Global.MsgResult.Yes)
            {
                // "Question50033" Please enter the Facility-No. or the address (ACUrl) of the alternative Target if you want to reject the batch. If you wan't to transport it to the originally planned target leave it blank.?
                PWMethodVBBase.EnterExtraDisTargetDest(acComponent, "Question50034");

                return true;
            }
            return false;
        }
        #endregion

        #endregion


        #region Interactionmethods on Result-Validation
        [ACMethodInteraction("", "en{'Accept zero quantity and take target quantity'}de{'Akzeptiere Null-Menge und übernehme Sollwert'}", 809, true)]
        public virtual void ZeroQAccept()
        {
            if (!IsEnabledZeroQAccept())
                return;
            _AckZeroQ_WasDosed = true;
            AcknowledgeAlarms();
        }

        public virtual bool IsEnabledZeroQAccept()
        {
            return QuantityIsZero && !AckZeroQ_WasDosed.HasValue;
        }


        [ACMethodInteraction("", "en{'Not dosed => Dose again'}de{'Nicht dosiert => Dosiere erneut'}", 810, true)]
        public virtual void ZeroQNotAccept()
        {
            if (!IsEnabledZeroQNotAccept())
                return;
            _AckZeroQ_WasDosed = false;
             AcknowledgeAlarms();
        }

        public virtual bool IsEnabledZeroQNotAccept()
        {
            return QuantityIsZero && !AckZeroQ_WasDosed.HasValue;
        }
        #endregion

        #region Misc
        public virtual void OnSiloStateChanged(PAMSilo silo, bool outwardEnabled)
        {
            if (IsDosingActiveFromSilo(silo))
            {
                this.ACStateConverter.OnProjSpecFunctionEvent(this, "OnSiloStateChanged", silo, outwardEnabled);
                if (!outwardEnabled && DosingAbortReason.ValueT == PADosingAbortReason.NotSet)
                {
                    PADosingAbortReason action = AutoAbortActionEnumOnBlockedSilo;
                    if (action == PADosingAbortReason.CompCancelled)
                        Abort();
                    else if (action == PADosingAbortReason.EmptySourceEndBatchplan)
                        EndDosEndOrderForced();
                    else if (action == PADosingAbortReason.EmptySourceNextSource)
                        SetAbortReasonEmptyForced();
                    else if (action == PADosingAbortReason.EmptySourceAbortAdjustOtherAndWait)
                        EndDosAdjustRestWaitForced();
                    else if (action == PADosingAbortReason.MachineMalfunction)
                        SetAbortReasonMalfunctionForced();
                    else if (action == PADosingAbortReason.EndDosingThenDisThenNextComp)
                        EndDosDisNextCompForced();
                    else if (action == PADosingAbortReason.EndTolErrorDosingThenDisThenNextComp)
                        EndDosDisNextCompOnTolForced();
                    else if (action == PADosingAbortReason.EndDosingThenDisThenEnd)
                        EndDosDisEndForced();
                    else if (action == PADosingAbortReason.EndTolErrorDosingThenDisThenEnd)
                        EndDosDisEndOnTolForced();
                }
                else if (outwardEnabled && DosingAbortReason.ValueT != PADosingAbortReason.NotSet && DosingAbortReason.ValueT == AutoAbortActionEnumOnBlockedSilo)
                {
                    ResetAbortReason();
                }
            }
        }

        public bool IsTransportActive
        {
            get
            {
                return CurrentACState >= ACStateEnum.SMRunning
                    && CurrentACState < ACStateEnum.SMResetting;
            }
        }

        public virtual bool IsDosingActiveFromSilo(PAMSilo silo)
        {
            if (!IsTransportActive)
                return false;
            var acMethod = CurrentACMethod.ValueT;
            if (acMethod == null || this.ACStateConverter == null)
                return false;

            PAMSilo currentSource = GetSourceSiloFromMethod(acMethod);
            return currentSource != null && silo == currentSource;

            //var valueSource = acMethod.ParameterValueList.GetACValue("Source");
            //if (valueSource == null)
            //    return false;
            //int currentSource = 0;
            //if (valueSource.Value is Int16)
            //    currentSource = valueSource.ParamAsInt16;
            //else if (valueSource.Value is UInt16)
            //    currentSource = valueSource.ParamAsUInt16;
            //if (currentSource <= 0)
            //    return false;
            //return silo.RouteItemIDAsNum == currentSource;
        }

        protected virtual void OnSourceChangeStoppOrAbort()
        {
            if (StopOnSourceChange)
                Stopp();
            else
                Abort();
        }
        #endregion

        #endregion

        #region override abstract methods
        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(SetAbortReasonEmpty):
                    SetAbortReasonEmpty();
                    return true;
                case nameof(SetAbortReasonMalfunction):
                    SetAbortReasonMalfunction();
                    return true;
                case nameof(EndDosDisNextComp):
                    EndDosDisNextComp();
                    return true;
                case nameof(EndDosDisEnd):
                    EndDosDisEnd();
                    return true;
                case nameof(EndDosEndOrder):
                    EndDosEndOrder();
                    return true;
                case nameof(EndDosDisNextCompOnTol):
                    EndDosDisNextCompOnTol();
                    return true;
                case nameof(EndDosDisEndOnTol):
                    EndDosDisEndOnTol();
                    return true;
                case nameof(EndDosAdjustRestWait):
                    EndDosAdjustRestWait();
                    return true;
                case nameof(ZeroQAccept):
                    ZeroQAccept();
                    return true;
                case nameof(ZeroQNotAccept):
                    ZeroQNotAccept();
                    return true;
                case nameof(ForceSetLackOfMaterial):
                    ForceSetLackOfMaterial();
                    return true;
                case nameof(InheritParamsFromConfig):
                    InheritParamsFromConfig(acParameter[0] as ACMethod, acParameter[1] as ACMethod, (bool)acParameter[2]);
                    return true;
                case nameof(SetDefaultACMethodValues):
                    SetDefaultACMethodValues(acParameter[0] as ACMethod);
                    return true;
                case nameof(IsEnabledSetAbortReasonEmpty):
                    result = IsEnabledSetAbortReasonEmpty();
                    return true;
                case nameof(IsEnabledSetAbortReasonMalfunction):
                    result = IsEnabledSetAbortReasonMalfunction();
                    return true;
                case nameof(IsEnabledEndDosDisNextComp):
                    result = IsEnabledEndDosDisNextComp();
                    return true;
                case nameof(IsEnabledEndDosDisEnd):
                    result = IsEnabledEndDosDisEnd();
                    return true;
                case nameof(IsEnabledEndDosEndOrder):
                    result = IsEnabledEndDosEndOrder();
                    return true;
                case nameof(IsEnabledEndDosDisNextCompOnTol):
                    result = IsEnabledEndDosDisNextCompOnTol();
                    return true;
                case nameof(IsEnabledEndDosDisEndOnTol):
                    result = IsEnabledEndDosDisEndOnTol();
                    return true;
                case nameof(IsEnabledEndDosAdjustRestWait):
                    result = IsEnabledEndDosAdjustRestWait();
                    return true;
                case nameof(IsEnabledZeroQAccept):
                    result = IsEnabledZeroQAccept();
                    return true;
                case nameof(IsEnabledZeroQNotAccept):
                    result = IsEnabledZeroQNotAccept();
                    return true;
                case nameof(IsEnabledForceSetLackOfMaterial):
                    result = IsEnabledForceSetLackOfMaterial();
                    return true;
                case nameof(GetDosingRestInfo):
                    result = GetDosingRestInfo();
                    return true;
                case nameof(ResetAbortReason):
                    ResetAbortReason();
                    return true;
                case nameof(IsEnabledResetAbortReason):
                    result = IsEnabledResetAbortReason();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PAFDosing(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(AskUserEndDosDisEnd):
                    result = AskUserEndDosDisEnd(acComponent);
                    return true;
                case nameof(AskUserEndDosEndOrder):
                    result = AskUserEndDosEndOrder(acComponent);
                    return true;
                case nameof(AskUserEndDosDisNextCompOnTol):
                    result = AskUserEndDosDisNextCompOnTol(acComponent);
                    return true;
                case nameof(AskUserEndDosDisEndOnTol):
                    result = AskUserEndDosDisEndOnTol(acComponent);
                    return true;
                case nameof(AskUserEndDosDisNextComp):
                    result = AskUserEndDosDisNextComp(acComponent);
                    return true;
                case nameof(AskUserEndDosAdjustRestWait):
                    result = AskUserEndDosAdjustRestWait(acComponent);
                    return true;
                case nameof(AskUserSetAbortReasonEmpty):
                    result = AskUserSetAbortReasonEmpty(acComponent);
                    return true;
            }
            return HandleExecuteACMethod_PAProcessFunction(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        #endregion


        protected override MsgWithDetails CompleteACMethodOnSMStarting(ACMethod acMethod, ACMethod previousParams)
        {
            ACValue value = acMethod.ParameterValueList.GetACValue(nameof(Route));
            if (value == null)
            {
                MsgWithDetails msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "CompleteACMethodOnSMStarting(1)", Message = "Route is empty." };
                return msg;
            }
            Route route = value.ValueT<Route>();
            RouteItem sourceRouteItem = route.FirstOrDefault();
            if (sourceRouteItem == null)
            {
                MsgWithDetails msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "CompleteACMethodOnSMStarting(2)", Message = "Last RouteItem is null." };
                return msg;
            }

            ACValue valuePrev = null;
            Route prevR = null;
            if (previousParams != null && IsSimulationOn)
            {
                valuePrev = previousParams.ParameterValueList.GetACValue(nameof(Route));
                if (valuePrev != null)
                    prevR = valuePrev.ValueT<Route>();
            }

            using (var db = new Database())
            {
                try
                {
                    route?.AttachTo(db);
                    prevR?.AttachTo(db);
                    
                    MsgWithDetails msg = GetACMethodFromConfig(db, route, acMethod);
                    if (msg != null)
                        return msg;

                    if (IsSimulationOn)
                    {
                        if (prevR != null && acMethod != previousParams)
                            PAEControlModuleBase.ActivateRouteOnSimulation(prevR, true);
                        PAEControlModuleBase.ActivateRouteOnSimulation(route, false);
                    }
                }
                catch (Exception e)
                {
                    Messages.LogException(this.GetACUrl(), "CompleteACMethodOnSMStarting()", e);
                    MsgWithDetails msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "CompleteACMethodOnSMStarting(3)", Message = e.Message };
                    return msg;
                }
                finally
                {
                     route?.Detach(true);
                     prevR?.Detach(true);
                    //sourceRouteItem.DetachEntitesFromDbContext();
                }
            }

            var targetQ = acMethod.ParameterValueList.GetACValue("TargetQuantity");
            var swtValue = acMethod.ParameterValueList.GetACValue("SWTWeight");
            if (targetQ != null && (swtValue == null || swtValue.ParamAsDouble < 0.0001))
            {
                double targetQuantity = targetQ.ParamAsDouble;
                var scale = ParentACComponent as IPAMContScale;
                if (scale != null)
                {
                    double? remainingWeight = null;
                    if (scale.RemainingWeightCapacity.HasValue)
                        remainingWeight = scale.RemainingWeightCapacity.Value;
                    else if ((scale as PAProcessModuleVB).MaxWeightCapacity.ValueT > 0.00000001)
                        remainingWeight = (scale as PAProcessModuleVB).MaxWeightCapacity.ValueT;

                    if (remainingWeight.HasValue 
                        && (targetQuantity > (remainingWeight.Value * 1.01))
                        && Math.Abs(remainingWeight.Value) > 0.000001) // Toleranz 1%
                    {
                        MsgWithDetails msg = new MsgWithDetails()
                                        {
                                            Source = this.GetACUrl(),
                                            MessageLevel = eMsgLevel.Error,
                                            ACIdentifier = "CompleteACMethodOnSMStarting(4)",
                                            Message = String.Format("TargetQuantity of {0} kg exceeds the remaining scale capacity!", targetQuantity)
                                        };
                        return msg;
                    }
                }
            }

            return null;
        }

        protected override CompleteResult AnalyzeACMethodResult(ACMethod acMethod, out MsgWithDetails msg, CompleteResult completeResult)
        {
            PAMSilo pamSilo = GetSourceSiloFromMethod(acMethod);
            if (pamSilo != null)
                pamSilo.UnSubscribeTransportFunction(this);

            if (acMethod == null)
            {
                msg = null;
                return completeResult;
            }

            if (completeResult != CompleteResult.Succeeded)
            {
                msg = null;
                return completeResult;
            }

            // If Dosing is done without scale and no result from PLC,
            // then calculate quantity from difference between last posted quantity
            // and measured weight
            double actualQuantity = (double)acMethod.ResultValueList["ActualQuantity"];
            if (   actualQuantity < 0.0000001 
                && CurrentScaleForWeighing == null)
            {
                PAMSilo dosingSilo = CurrentDosingSilo;
                if (dosingSilo != null)
                {
                    DosingRestInfo restInfo = new DosingRestInfo(dosingSilo, this, null, IsSourceMarkedAsEmpty);
                    if (Math.Abs(restInfo.DosedQuantity) > Double.Epsilon)
                    {
                        actualQuantity = restInfo.DosedQuantity;
                        acMethod.ResultValueList["ActualQuantity"] = actualQuantity;
                    }
                }
            }
            
            if (this.IsSimulationOn)
            {
                if (actualQuantity < 0.0000001)
                {
                    actualQuantity = (double)acMethod.ParameterValueList["TargetQuantity"];
                    if (DosingAbortReason.ValueT != PADosingAbortReason.NotSet)
                    {
                        if (CurrentScaleForWeighing != null)
                        {
                            if (CurrentScaleForWeighing.ActualWeight.ValueT > 0.0001 && CurrentScaleForWeighing.ActualWeight.ValueT < actualQuantity)
                                actualQuantity = CurrentScaleForWeighing.ActualWeight.ValueT;
                            else
                                actualQuantity = actualQuantity * 0.5;
                        }
                        else
                            actualQuantity = actualQuantity * 0.5;
                    }
                    acMethod.ResultValueList["ActualQuantity"] = actualQuantity;
                }
            }
            else if (actualQuantity < 0.0000001)
            {
                _QuantityIsZero = true;
                if (_AckZeroQ_WasDosed.HasValue)
                {
                    if (_AckZeroQ_WasDosed.Value)
                        acMethod.ResultValueList["ActualQuantity"] = (double)acMethod.ParameterValueList["TargetQuantity"];
                }
                else
                {
                    if (DosingAbortReason.ValueT == PADosingAbortReason.NotSet)
                    {
                        // Actual quantity is zero. Please verify if component was dosed.
                        msg = new MsgWithDetails() { Message = Root.Environment.TranslateMessage(this, "Warning50017"), ACIdentifier = "ZeroQuantity" };
                        completeResult = CompleteResult.FailedAndWait;
                        return completeResult;
                    }
                }
            }
            else
            {
                _AckZeroQ_WasDosed = null;
                _QuantityIsZero = false;
            }

            var container = ParentACComponent as IPAMCont;
            if (container != null)
            {
                var densityACValue = acMethod.ParameterValueList.GetACValue("Density");
                if (densityACValue != null && Material.ValidateDensity(densityACValue.ParamAsDouble))
                {
                    actualQuantity = (double)acMethod.ResultValueList["ActualQuantity"];
                    double newVolume = (actualQuantity * 1000) / densityACValue.ParamAsDouble;
                    IACPropertyNetTarget fillVolumeProp = container.FillVolume as IACPropertyNetTarget;
                    if (fillVolumeProp != null && fillVolumeProp.Source == null)
                        container.FillVolume.ValueT += newVolume;
                }
            }


            ACValue value = acMethod.ParameterValueList.GetACValue(nameof(Route));
            if (value == null)
            {
                msg = null;
                return completeResult;
            }
            Route route = value.ValueT<Route>();

            using (var db = new Database())
            {
                try
                {
                    route.AttachTo(db);

                    if (IsSimulationOn)
                        PAEControlModuleBase.ActivateRouteOnSimulation(route, true);

                    if (route == null || route.Count < 1)
                    {
                        msg = null;
                        return completeResult;
                    }
                    RouteItem targetRouteItem = route.LastOrDefault();
                    if (targetRouteItem.Target.ACKind == Global.ACKinds.TPAProcessFunction)
                    {
                        if (route.Count < 2)
                        {
                            msg = null;
                            return completeResult;
                        }
                        targetRouteItem = route[route.Count - 2];
                    }
                    RouteItem sourceRouteItem = route.FirstOrDefault();

                    //sourceRouteItem.DetachEntitesFromDbContext();
                    gip.core.datamodel.ACClass thisACClass = ComponentClass.FromIPlusContext<gip.core.datamodel.ACClass>(db);
                    gip.core.datamodel.ACClassPropertyRelation logicalRelation = db.ACClassPropertyRelation
                        .Where(c => c.SourceACClassID == sourceRouteItem.Source.ACClassID
                            && c.SourceACClassPropertyID == sourceRouteItem.SourceProperty.ACClassPropertyID
                            && c.TargetACClassID == targetRouteItem.Target.ACClassID
                            && c.TargetACClassPropertyID == targetRouteItem.TargetProperty.ACClassPropertyID)
                        .FirstOrDefault();
                    if (logicalRelation == null)
                    {
                        msg = null;
                        return completeResult;
                    }
                    gip.core.datamodel.ACClassConfig config = logicalRelation.ACClassConfig_ACClassPropertyRelation.FirstOrDefault();
                    if (config == null)
                    {
                        msg = null;
                        return completeResult;
                    }
                    ACMethod storedACMethod = config.Value as ACMethod;
                    if (storedACMethod == null)
                    {
                        msg = null;
                        return completeResult;
                    }

                    if (acMethod.ResultValueList.GetACValue("Afterflow") != null)
                    {
                        storedACMethod.ResultValueList["Afterflow"] = acMethod.ResultValueList["Afterflow"];
                        storedACMethod.ParameterValueList["Afterflow"] = acMethod.ResultValueList["Afterflow"];
                    }

                    config.Value = storedACMethod;
                    if (config.ACProperties != null)
                        config.ACProperties.Serialize();
                    db.ACSaveChanges();
                }
                catch (Exception e)
                {
                    msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "ACMethodResultCompleted(1)", Message = e.Message };
                    completeResult = CompleteResult.FailedAndWait;
                    return completeResult;
                }
                finally
                {
                    route.Detach(true);
                }
            }
            msg = null;
            return completeResult;
        }

        protected virtual void OnSetRouteItemData(ACMethod acMethod, PAMSilo silo, RouteItem routeItem, bool isConfigInitialization)
        {
            if (silo != null)
                silo.SubscribeTransportFunction(this);
        }


        protected override void OnChangingCurrentACMethod(ACMethod currentACMethod, ACMethod newACMethod)
        {
            base.OnChangingCurrentACMethod(currentACMethod, newACMethod);

            ACValue value = currentACMethod.ParameterValueList.GetACValue(nameof(Route));
            if (value != null)
            {
                Route originalR = value.ValueT<Route>();
                if (originalR != null && !originalR.AreACUrlInfosSet)
                {
                    using (var db = new Database())
                    {
                        try
                        {
                            originalR.AttachTo(db); // Global context
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            originalR.Detach(true);
                        }
                    }
                }
            }

            bool unsubscribe = true;
            if (IsMethodChangedFromClient 
                && newACMethod != null 
                && currentACMethod != null)
            {
                unsubscribe = false;
                ACValue acValueNew = newACMethod.ParameterValueList.GetACValue("Source");
                ACValue acValueOld = currentACMethod.ParameterValueList.GetACValue("Source");
                if (acValueNew != null && acValueOld != null)
                {
                    if (acValueNew.Value is Int16)
                    {
                        if (acValueNew.ParamAsInt16 > 0
                            && acValueNew.ParamAsInt16 != acValueOld.ParamAsInt16)
                            unsubscribe = true;
                    }
                    else if (acValueNew.Value is UInt16)
                    {
                        if (acValueNew.ParamAsUInt16 > 0
                            && acValueNew.ParamAsUInt16 != acValueOld.ParamAsUInt16)
                            unsubscribe = true;
                    }
                }
            }

            // If Source will be changed from PWDosing, than previous silo must be unsubscribed
            if (unsubscribe)
            {
                PAMSilo pamSilo = GetSourceSiloFromMethod(currentACMethod);
                if (pamSilo != null)
                    pamSilo.UnSubscribeTransportFunction(this);
            }
        }


        public override void InitializeRouteAndConfig(Database dbIPlus)
        {
            gip.core.datamodel.ACClass thisACClass = this.ComponentClass;
            gip.core.datamodel.ACClass parentACClass = ParentACComponent.ComponentClass;
            try
            {
                ACRoutingParameters routingParameters = new ACRoutingParameters()
                {
                    Database = dbIPlus,
                    Direction = RouteDirections.Backwards,
                    DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && c.ACClassID == parentACClass.ACClassID,
                    DBIncludeInternalConnections = true,
                    AutoDetachFromDBContext = false
                };

                var parentModule = ACRoutingService.DbSelectRoutesFromPoint(thisACClass, this.PAPointMatIn1.PropertyInfo, routingParameters).FirstOrDefault();
                var sourcePoint = parentModule?.FirstOrDefault()?.SourceACPoint?.PropertyInfo;
                if (sourcePoint == null)
                    return;

                routingParameters.DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && c.ACClassID != parentACClass.ACClassID;

                var routes = ACRoutingService.DbSelectRoutesFromPoint(parentACClass, sourcePoint, routingParameters);
                if (routes != null && routes.Any())
                {
                    string virtMethodName = VMethodName_Dosing;
                    IReadOnlyList<ACMethodWrapper> virtualMethods = ACMethod.GetVirtualMethodInfos(this.GetType(), ACStateConst.TMStart);
                    if (virtualMethods != null && virtualMethods.Any())
                        virtMethodName = virtualMethods.FirstOrDefault().Method.ACIdentifier;
                    virtMethodName = OnGetVMethodNameForRouteInitialization(virtMethodName);

                    foreach (Route route in routes)
                    {
                        ACMethod acMethod = ACUrlACTypeSignature("!" + virtMethodName);
                        GetACMethodFromConfig(dbIPlus, route, acMethod, true);
                    }
                }
            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), "InitializeRouteAndConfig(0)", e.Message);
            }
        }


        protected MsgWithDetails GetACMethodFromConfig(Database db, Route route, ACMethod acMethod, bool isConfigInitialization = false)
        {
            if (route == null || route.Count < 1)
                return new MsgWithDetails();
            if (IsMethodChangedFromClient)
                return null;
            RouteItem targetRouteItem = route.LastOrDefault();
            if (targetRouteItem.Target.ACKind == Global.ACKinds.TPAProcessFunction)
            {
                if (route.Count < 2)
                    return new MsgWithDetails();
                targetRouteItem = route[route.Count - 2];
            }
            RouteItem sourceRouteItem = route.FirstOrDefault();

            ACValue valueSource = acMethod.ParameterValueList.GetACValue("Source");
            if (valueSource != null && sourceRouteItem != null)
            {
                bool setSource = false;
                if (valueSource.Value is Int16)
                {
                    setSource = valueSource.ParamAsInt16 <= 0;
                }
                else if (valueSource.Value is UInt16)
                {
                    setSource = valueSource.ParamAsUInt16 <= 0;
                }

                PAMSilo pamSilo = sourceRouteItem.SourceACComponent as PAMSilo;
                if (setSource || isConfigInitialization)
                {
                    if (pamSilo != null)
                    {
                        if (valueSource.Value is Int16)
                            valueSource.Value = Convert.ToInt16(pamSilo.RouteItemIDAsNum);
                        else if (valueSource.Value is UInt16)
                            valueSource.Value = Convert.ToUInt16(pamSilo.RouteItemIDAsNum);
                    }
                }
                OnSetRouteItemData(acMethod, pamSilo, sourceRouteItem, isConfigInitialization);
            }

            List<MaterialConfig> materialConfigList = null;
            gip.core.datamodel.ACClass thisACClass = ComponentClass.FromIPlusContext<gip.core.datamodel.ACClass>(db);
            gip.core.datamodel.ACClassConfig config = null;
            gip.core.datamodel.ACClassPropertyRelation logicalRelation = db.ACClassPropertyRelation
                .Where(c => c.SourceACClassID == sourceRouteItem.Source.ACClassID
                            && c.SourceACClassPropertyID == sourceRouteItem.SourceProperty.ACClassPropertyID
                            && c.TargetACClassID == targetRouteItem.Target.ACClassID
                            && c.TargetACClassPropertyID == targetRouteItem.TargetProperty.ACClassPropertyID)
                .FirstOrDefault();
            if (logicalRelation == null)
            {
                logicalRelation = gip.core.datamodel.ACClassPropertyRelation.NewACObject(db, null);
                logicalRelation.SourceACClass = sourceRouteItem.Source;
                logicalRelation.SourceACClassProperty = sourceRouteItem.SourceProperty;
                logicalRelation.TargetACClass = targetRouteItem.Target;
                logicalRelation.TargetACClassProperty = targetRouteItem.TargetProperty;
                logicalRelation.ConnectionType = Global.ConnectionTypes.DynamicConnection;
            }
            else
            {
                config = logicalRelation.ACClassConfig_ACClassPropertyRelation.FirstOrDefault();
                if (!isConfigInitialization)
                {
                    PAMSilo pamSilo = sourceRouteItem.SourceACComponent as PAMSilo;
                    if (pamSilo != null)
                    {
                        if (pamSilo.Facility != null && pamSilo.Facility.ValueT != null && pamSilo.Facility.ValueT.ValueT != null)
                        {
                            Guid? materialID = pamSilo.Facility.ValueT.ValueT.MaterialID;
                            if (materialID.HasValue && materialID != Guid.Empty)
                            {
                                Guid acClassIdOfParent = ParentACComponent.ComponentClass.ACClassID;
                                using (var dbApp = new DatabaseApp())
                                {
                                    // 1. Hole Material-Konfiguration spezielle für diesen Weg
                                    materialConfigList = dbApp.MaterialConfig.Where(c => c.VBiACClassPropertyRelationID == logicalRelation.ACClassPropertyRelationID && c.MaterialID == materialID.Value).AsNoTracking().ToList();
                                    var wayIndependent = dbApp.MaterialConfig.Where(c => c.MaterialID == materialID.Value && c.VBiACClassID == acClassIdOfParent).AsNoTracking();
                                    foreach (var matConfigIndepedent in wayIndependent)
                                    {
                                        if (!materialConfigList.Where(c => c.LocalConfigACUrl == matConfigIndepedent.LocalConfigACUrl).Any())
                                            materialConfigList.Add(matConfigIndepedent);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            ACMethod storedACMethod = null;
            if (config == null)
            {
                config = thisACClass.NewACConfig(null, db.GetACType(typeof(ACMethod))) as gip.core.datamodel.ACClassConfig;
                config.KeyACUrl = logicalRelation.GetKey();
                config.ACClassPropertyRelation = logicalRelation;
            }
            else
                storedACMethod = config.Value as ACMethod;

            bool isNewDefaultedMethod = false;
            bool differentVirtualMethod = false;
            if (storedACMethod == null || storedACMethod.ACIdentifier != acMethod.ACIdentifier)
            {
                if (storedACMethod != null && storedACMethod.ACIdentifier != acMethod.ACIdentifier)
                {
                    differentVirtualMethod = true;
                    var clonedMethod = acMethod.Clone() as ACMethod;
                    clonedMethod.CopyParamValuesFrom(storedACMethod);
                    storedACMethod = clonedMethod;
                }
                else
                {
                    isNewDefaultedMethod = true;
                    storedACMethod = acMethod.Clone() as ACMethod;
                    ACUrlCommand("!SetDefaultACMethodValues", storedACMethod);
                }
            }
            // Überschreibe Parameter mit materialabhängigen Einstellungen
            if (!isConfigInitialization
                && config.EntityState != EntityState.Added
                && materialConfigList != null
                && materialConfigList.Any())
            {
                foreach (var matConfig in materialConfigList)
                {
                    ACValue acValue = acMethod.ParameterValueList.Where(c => c.ACIdentifier == matConfig.LocalConfigACUrl).FirstOrDefault();
                    if (acValue != null/* && acValue.HasDefaultValue*/)
                        acValue.Value = matConfig.Value;
                    if (storedACMethod != null)
                    {
                        acValue = storedACMethod.ParameterValueList.Where(c => c.ACIdentifier == matConfig.LocalConfigACUrl).FirstOrDefault();
                        if (acValue != null/* && acValue.HasDefaultValue*/)
                            acValue.Value = matConfig.Value;
                    }
                }
            }
            if (!isNewDefaultedMethod)
                ACUrlCommand("!InheritParamsFromConfig", acMethod, storedACMethod, isConfigInitialization);
            if (config.EntityState == EntityState.Added || isNewDefaultedMethod)
                config.Value = storedACMethod;
            else if (isConfigInitialization)
            {
                if (differentVirtualMethod)
                    config.Value = storedACMethod;
                else
                    config.Value = acMethod;
            }
            if (config.EntityState == EntityState.Added || logicalRelation.EntityState == EntityState.Added || isNewDefaultedMethod || isConfigInitialization || differentVirtualMethod)
            {
                MsgWithDetails msg = db.ACSaveChanges();
                if (msg != null)
                    return msg;
            }
            return null;
        }


        public override void SMAborting()
        {
            base.SMAborting();
            if (DosingAbortReason.ValueT == PADosingAbortReason.NotSet)
                DosingAbortReason.ValueT = PADosingAbortReason.CompCancelled;
        }

        public override void SMStopping()
        {
            base.SMStopping();
            if (DosingAbortReason.ValueT == PADosingAbortReason.NotSet)
                DosingAbortReason.ValueT = PADosingAbortReason.CompCancelled;
        }
        #endregion

        #region Simulation
        protected double? _SimulationScaleIncrement = null;
        protected override bool CyclicWaitIfSimulationOn()
        {
            if (ACOperationMode != ACOperationModes.Live || !IsSimulationOn)
            {
                _SimulationScaleIncrement = null;
                return base.CyclicWaitIfSimulationOn();
            }

            //var scale = ParentACComponent as IPAMContScale;
            IACCommSession session = Session as IACCommSession;
            PAEScaleGravimetric scale = this.CurrentScaleForWeighing as PAEScaleGravimetric;
            if (scale != null
                && !IsManualSimulation 
                && CurrentACMethod != null && CurrentACMethod.ValueT != null 
                && (session == null || !session.IsConnected.ValueT))
            {
                var targetQ = CurrentACMethod.ValueT.ParameterValueList.GetACValue("TargetQuantity");
                if (targetQ != null)
                {
                    SubscribeToProjectWorkCycle();
                    double targetQuantity = targetQ.ParamAsDouble;
                    if (scale.IsTareRequestAcknowledged)
                    {
                        scale.DesiredWeight.ValueT = targetQuantity;
                        scale.TareScale(true, true);
                    }
                    else if (scale.ActualWeight.ValueT >= targetQuantity)
                    {
                        scale.TareScale(false, true);
                        UnSubscribeToProjectWorkCycle();
                        _SimulationWait = 0;
                        _SimulationScaleIncrement = null;
                        scale.DesiredWeight.ValueT = 0.0;
                        return false;
                    }
                    if (!_SimulationScaleIncrement.HasValue)
                    {
                        Random random = new Random();
                        PWDosing invokingDosNode = null;
                        if (this.CurrentTask != null && CurrentACMethod.ValueT != null)
                            invokingDosNode = this.CurrentTask.ValueT as PWDosing;
                        int rndMinValue = 5;
                        int rndMaxValue = 20;
                        if (invokingDosNode != null && invokingDosNode.IterationCount.ValueT > 0)
                        {
                            rndMinValue = invokingDosNode.IterationCount.ValueT * 5;
                            rndMaxValue = invokingDosNode.IterationCount.ValueT * 20;
                        }

                        int steps = random.Next(rndMinValue, rndMaxValue);
                        _SimulationScaleIncrement = targetQuantity / steps;
                    }
                    scale.SimulateWeight(_SimulationScaleIncrement.Value);
                    return true;
                }
            }
            _SimulationScaleIncrement = null;
            return base.CyclicWaitIfSimulationOn();
        }
        #endregion

        #region Private
        public static PAMSilo GetSourceSiloFromMethod(ACMethod acMethod)
        {
            if (acMethod == null)
                return null;
            ACValue value = acMethod.ParameterValueList.GetACValue(nameof(Route));
            if (value == null)
                return null;
            Route route = value.ValueT<Route>();
            if (route == null)
                return null;
            RouteItem sourceRouteItem = route.FirstOrDefault();
            if (sourceRouteItem == null)
                return null;
            PAMSilo pamSilo = sourceRouteItem.SourceACComponent as PAMSilo;
            if (pamSilo != null)
                return pamSilo;
            if (sourceRouteItem.IsAttached)
                return null;
            using (var db = new Database())
            {
                try
                {
                    sourceRouteItem.AttachTo(db);
                    pamSilo = sourceRouteItem.SourceACComponent as PAMSilo;
                }
                finally
                {
                    if (sourceRouteItem != null)
                        sourceRouteItem.Detach();
                }
            }
            return pamSilo;
        }

        public static ACMethodWrapper CreateVirtualMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);

            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue(Material.ClassName, typeof(string), null, Global.ParamOption.Optional));
            paramTranslation.Add(Material.ClassName, "en{'Material'}de{'Material'}");
            method.ParameterValueList.Add(new ACValue("PLPosRelation", typeof(Guid), null, Global.ParamOption.Optional));
            paramTranslation.Add("PLPosRelation", "en{'Order position'}de{'Auftragsposition'}");
            method.ParameterValueList.Add(new ACValue(nameof(Route), typeof(Route), null, Global.ParamOption.Required));
            paramTranslation.Add(nameof(Route), "en{'Route'}de{'Route'}");
            method.ParameterValueList.Add(new ACValue("Source", typeof(Int16), 0, Global.ParamOption.Required));
            paramTranslation.Add("Source", "en{'Source'}de{'Quelle'}");
            method.ParameterValueList.Add(new ACValue("Destination", typeof(Int16), 0, Global.ParamOption.Optional));
            paramTranslation.Add("Destination", "en{'Destination'}de{'Ziel'}");
            method.ParameterValueList.Add(new ACValue("TargetQuantity", typeof(Double), (Double)0.0, Global.ParamOption.Required));
            paramTranslation.Add("TargetQuantity", "en{'Target Quantity'}de{'Sollmenge'}");
            //Method.ParameterValueList.Add(new ACValue("TargetWeight", typeof(Double), 0, Global.ParamOption.Optional));
            method.ParameterValueList.Add(new ACValue("FlowRate1", typeof(Int16), (Int16)0, Global.ParamOption.Optional));
            paramTranslation.Add("FlowRate1", "en{'Flow Rate 1'}de{'Dosierstufe 1'}");
            method.ParameterValueList.Add(new ACValue("FlowRate2", typeof(Int16), (Int16)0, Global.ParamOption.Optional));
            paramTranslation.Add("FlowRate2", "en{'Flow Rate 2'}de{'Dosierstufe 2'}");
            method.ParameterValueList.Add(new ACValue("FlowRate3", typeof(Int16), (Int16)0, Global.ParamOption.Optional));
            paramTranslation.Add("FlowRate3", "en{'Flow Rate 3'}de{'Dosierstufe 3'}");
            method.ParameterValueList.Add(new ACValue("FlowRate4", typeof(Int16), (Int16)0, Global.ParamOption.Optional));
            paramTranslation.Add("FlowRate4", "en{'Flow Rate 4'}de{'Dosierstufe 4'}");
            method.ParameterValueList.Add(new ACValue("FlowRate5", typeof(Int16), (Int16)0, Global.ParamOption.Optional));
            paramTranslation.Add("FlowRate5", "en{'Flow Rate 5'}de{'Dosierstufe 5'}");
            method.ParameterValueList.Add(new ACValue("FlowRate6", typeof(Int16), (Int16)0, Global.ParamOption.Optional));
            paramTranslation.Add("FlowRate6", "en{'Flow Rate 6'}de{'Dosierstufe 6'}");
            method.ParameterValueList.Add(new ACValue("FlowRate7", typeof(Int16), (Int16)0, Global.ParamOption.Optional));
            paramTranslation.Add("FlowRate7", "en{'Flow Rate 7'}de{'Dosierstufe 7'}");
            method.ParameterValueList.Add(new ACValue("FlowRate8", typeof(Int16), (Int16)0, Global.ParamOption.Optional));
            paramTranslation.Add("FlowRate8", "en{'Flow Rate 8'}de{'Dosierstufe 8'}");
            method.ParameterValueList.Add(new ACValue("FlowRate9", typeof(Int16), (Int16)0, Global.ParamOption.Optional));
            paramTranslation.Add("FlowRate9", "en{'Flow Rate 9'}de{'Dosierstufe 9'}");
            method.ParameterValueList.Add(new ACValue("FlowSwitching1", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("FlowSwitching1", "en{'Switching point 1'}de{'Umschaltpunkt 1'}");
            method.ParameterValueList.Add(new ACValue("FlowSwitching2", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("FlowSwitching2", "en{'Switching point 2'}de{'Umschaltpunkt 2'}");
            method.ParameterValueList.Add(new ACValue("FlowSwitching3", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("FlowSwitching3", "en{'Switching point 3'}de{'Umschaltpunkt 3'}");
            method.ParameterValueList.Add(new ACValue("FlowSwitching4", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("FlowSwitching4", "en{'Switching point 4'}de{'Umschaltpunkt 4'}");
            method.ParameterValueList.Add(new ACValue("FlowSwitching5", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("FlowSwitching5", "en{'Switching point 5'}de{'Umschaltpunkt 5'}");
            method.ParameterValueList.Add(new ACValue("FlowSwitching6", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("FlowSwitching6", "en{'Switching point 6'}de{'Umschaltpunkt 6'}");
            method.ParameterValueList.Add(new ACValue("FlowSwitching7", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("FlowSwitching7", "en{'Switching point 7'}de{'Umschaltpunkt 7'}");
            method.ParameterValueList.Add(new ACValue("FlowSwitching8", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("FlowSwitching8", "en{'Switching point 8'}de{'Umschaltpunkt 8'}");
            method.ParameterValueList.Add(new ACValue("FlowSwitching9", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("FlowSwitching9", "en{'Switching point 9'}de{'Umschaltpunkt 9'}");
            method.ParameterValueList.Add(new ACValue("RoughFineSwitching", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("RoughFineSwitching", "en{'Switching point Rough/Fine'}de{'Umschaltpunkt Grob/Fein'}");
            method.ParameterValueList.Add(new ACValue("FlowRateRough", typeof(Int16), (Int16)0, Global.ParamOption.Optional));
            paramTranslation.Add("FlowRateRough", "en{'FlowRateRough'}de{'Dosierleistung Grobstufe'}");
            method.ParameterValueList.Add(new ACValue("FlowRateFine", typeof(Int16), (Int16)0, Global.ParamOption.Optional));
            paramTranslation.Add("FlowRateFine", "en{'FlowRateFine'}de{'Dosierleistung Feinstufe'}");
            method.ParameterValueList.Add(new ACValue("TolerancePlus", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("TolerancePlus", "en{'Tolerance + [+=kg/-=%]'}de{'Toleranz + [+=kg/-=%]'}");
            method.ParameterValueList.Add(new ACValue("ToleranceMinus", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("ToleranceMinus", "en{'Tolerance - [+=kg/-=%]'}de{'Toleranz - [+=kg/-=%]'}");
            method.ParameterValueList.Add(new ACValue("Afterflow", typeof(Double), (Double)0.0, Global.ParamOption.Required));
            paramTranslation.Add("Afterflow", "en{'Afterflow'}de{'Nachlauf'}");
            method.ParameterValueList.Add(new ACValue("AfterflowMAX", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("AfterflowMAX", "en{'Afterflow Max.'}de{'Nachlauf Max.'}");
            method.ParameterValueList.Add(new ACValue("LackOfMatCheckWeight", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("LackOfMatCheckWeight", "en{'Lack of material checkweight'}de{'Materialmangel Prüfgewicht'}");
            method.ParameterValueList.Add(new ACValue("LackOfMatCheckTime", typeof(TimeSpan), TimeSpan.Zero, Global.ParamOption.Optional));
            paramTranslation.Add("LackOfMatCheckTime", "en{'Lack of material checktime'}de{'Materialmangel Prüfzeit'}");
            method.ParameterValueList.Add(new ACValue("PulsationTime", typeof(TimeSpan), TimeSpan.Zero, Global.ParamOption.Optional));
            paramTranslation.Add("PulsationTime", "en{'Pulsation time'}de{'Pulszeit'}");
            method.ParameterValueList.Add(new ACValue("PulsationPauseTime", typeof(TimeSpan), TimeSpan.Zero, Global.ParamOption.Optional));
            paramTranslation.Add("PulsationPauseTime", "en{'Pulsation pause time'}de{'Puls-Pause-Zeit'}");
            method.ParameterValueList.Add(new ACValue("AdjustmentTime", typeof(TimeSpan), TimeSpan.Zero, Global.ParamOption.Optional)); // Nachdosierimpulszeit in ms
            paramTranslation.Add("AdjustmentTime", "en{'Adjustment time'}de{'Nachdosierimpulszeit'}");
            method.ParameterValueList.Add(new ACValue("CalmingTime", typeof(TimeSpan), TimeSpan.Zero, Global.ParamOption.Optional)); // Berúhigungszeit in ms
            paramTranslation.Add("CalmingTime", "en{'Calming time'}de{'Beruhigungszeit'}");
            method.ParameterValueList.Add(new ACValue("StandstillTime", typeof(TimeSpan), TimeSpan.Zero, Global.ParamOption.Optional)); // ms
            paramTranslation.Add("StandstillTime", "en{'Standstill time'}de{'Stillstandszeit'}");
            method.ParameterValueList.Add(new ACValue("StandstillWeight", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("StandstillWeight", "en{'Standstill weight'}de{'Stillstandsgewicht'}");
            method.ParameterValueList.Add(new ACValue("Temperature", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("Temperature", "en{'Temperature'}de{'Temperatur'}");
            method.ParameterValueList.Add(new ACValue(PWMethodVBBase.IsLastBatchParamName, typeof(Int16), (Int16)0, Global.ParamOption.Optional));
            paramTranslation.Add(PWMethodVBBase.IsLastBatchParamName, "en{'Last batch'}de{'Letzter Batch'}"); // 1=Last Batch, 2=Last component, 3=Last Batch & Last component
            method.ParameterValueList.Add(new ACValue("SprayNozzle", typeof(Int16), (Int16)0, Global.ParamOption.Optional));
            paramTranslation.Add("SprayNozzle", "en{'Spray nozzle'}de{'Sprühdüsen'}");
            method.ParameterValueList.Add(new ACValue("ImpulsePerLiter", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("ImpulsePerLiter", "en{'Impulse per liter'}de{'Impuls pro Liter'}");
            method.ParameterValueList.Add(new ACValue("Density", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("Density", "en{'Density [g/dm³]'}de{'Dichte [g/dm³]'}");
            method.ParameterValueList.Add(new ACValue("MaxDosingTime", typeof(TimeSpan), TimeSpan.Zero, Global.ParamOption.Optional)); // ms
            paramTranslation.Add("MaxDosingTime", "en{'Max. Dosing time'}de{'Maximale Dosierzeit'}");
            method.ParameterValueList.Add(new ACValue("AdjustmentFlowRate", typeof(Int16), (Int16)0, Global.ParamOption.Optional));
            paramTranslation.Add("AdjustmentFlowRate", "en{'Adjustment FlowRate'}de{'Dosierstufe in Nachdosierung'}");
            method.ParameterValueList.Add(new ACValue("EndlessDosing", typeof(bool), (bool)false, Global.ParamOption.Optional));
            paramTranslation.Add("EndlessDosing", "en{'Endless dosing'}de{'Endlose Dosierung'}");
            method.ParameterValueList.Add(new ACValue("SWTWeight", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("SWTWeight", "en{'SWT tip weight'}de{'SWT Kippgewicht'}");

            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            method.ResultValueList.Add(new ACValue("ActualQuantity", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            resultTranslation.Add("ActualQuantity", "en{'Actual quantity'}de{'Istgewicht'}");
            method.ResultValueList.Add(new ACValue("ScaleTotalWeight", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            resultTranslation.Add("ScaleTotalWeight", "en{'Scale total weight'}de{'Gesamtgewicht Waage'}");
            method.ResultValueList.Add(new ACValue("Afterflow", typeof(Double), (Double)(-1.0), Global.ParamOption.Optional));
            resultTranslation.Add("Afterflow", "en{'Afterflow'}de{'Nachlauf'}");
            method.ResultValueList.Add(new ACValue("DosingTime", typeof(TimeSpan), TimeSpan.Zero, Global.ParamOption.Optional));
            resultTranslation.Add("DosingTime", "en{'Dosingtime'}de{'Dosierzeit'}");
            method.ResultValueList.Add(new ACValue("Temperature", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            resultTranslation.Add("Temperature", "en{'Temperature'}de{'Temperatur'}");
            method.ResultValueList.Add(new ACValue("GaugeCode", typeof(string), "", Global.ParamOption.Optional));
            resultTranslation.Add("GaugeCode", "en{'Gauge code/Alibi-No.'}de{'Wägeid/Alibi-No.'}");
            method.ResultValueList.Add(new ACValue("GaugeCodeStart", typeof(string), "", Global.ParamOption.Optional));
            resultTranslation.Add("GaugeCodeStart", "en{'Gauge code start/Alibi-No.'}de{'Wägeid Start/Alibi-No.'}");
            method.ResultValueList.Add(new ACValue("GaugeCodeEnd", typeof(string), "", Global.ParamOption.Optional));
            resultTranslation.Add("GaugeCodeEnd", "en{'Gauge code end/Alibi-No.'}de{'Wägeid Ende/Alibi-No.'}");
            method.ResultValueList.Add(new ACValue("FlowRate", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            resultTranslation.Add("FlowRate", "en{'Flow rate'}de{'Durchflussrate'}");
            method.ResultValueList.Add(new ACValue("ImpulseCounter", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            resultTranslation.Add("ImpulseCounter", "en{'Impulse counter'}de{'Impulszähler'}");

            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }

        public override void AcknowledgeAlarms()
        {
            if (!IsEnabledAcknowledgeAlarms())
                return;
            OnHandleAcknowlegdeDosingAlarms();
            base.AcknowledgeAlarms();
        }

        public override bool IsEnabledAcknowledgeAlarms()
        {
            if (((StateTolerance.ValueT == PANotifyState.AlarmOrFault) && (!FaultAckTolerance.ValueT))
                || ((StateLackOfMaterial.ValueT == PANotifyState.AlarmOrFault) && (!FaultAckLackOfMaterial.ValueT))
                || ((StateDosingTime.ValueT == PANotifyState.AlarmOrFault) && (!FaultAckDosingTime.ValueT)))
                return true;
            return base.IsEnabledAcknowledgeAlarms();
        }

        protected virtual void OnHandleAcknowlegdeDosingAlarms()
        {
            if (StateTolerance.ValueT != PANotifyState.Off)
                FaultAckTolerance.ValueT = true;
            if (StateLackOfMaterial.ValueT != PANotifyState.Off)
            {
                if (_LackOfMaterialForced == true)
                {
                    StateLackOfMaterial.ValueT = PANotifyState.Off;
                    _LackOfMaterialForced = false;
                }
                else
                    FaultAckLackOfMaterial.ValueT = true;
            }
            if (StateDosingTime.ValueT != PANotifyState.Off)
                FaultAckDosingTime.ValueT = true;
        }

        [ACMethodInfo("Function", "en{'Default dosing paramters'}de{'Standard Dosierparameter'}", 9999)]
        public virtual void SetDefaultACMethodValues(ACMethod newACMethod)
        {
            newACMethod["FlowRate9"] = (Int16)100;
            newACMethod["FlowRate8"] = (Int16)90;
            newACMethod["FlowRate7"] = (Int16)80;
            newACMethod["FlowRate6"] = (Int16)70;
            newACMethod["FlowRate5"] = (Int16)60;
            newACMethod["FlowRate4"] = (Int16)50;
            newACMethod["FlowRate3"] = (Int16)40;
            newACMethod["FlowRate2"] = (Int16)30;
            newACMethod["FlowRate1"] = (Int16)20;
            newACMethod["FlowSwitching9"] = (Double)500;
            newACMethod["FlowSwitching8"] = (Double)250;
            newACMethod["FlowSwitching7"] = (Double)125;
            newACMethod["FlowSwitching6"] = (Double)75;
            newACMethod["FlowSwitching5"] = (Double)50;
            newACMethod["FlowSwitching4"] = (Double)25;
            newACMethod["FlowSwitching3"] = (Double)10;
            newACMethod["FlowSwitching2"] = (Double)5;
            newACMethod["FlowSwitching1"] = (Double)2.5;
            newACMethod["RoughFineSwitching"] = (Double)10;
            newACMethod["FlowRateRough"] = (Int16)100;
            newACMethod["FlowRateFine"] = (Int16)10;
            newACMethod["TolerancePlus"] = (Double)1;
            newACMethod["ToleranceMinus"] = (Double)1;
            newACMethod["Afterflow"] = (Double)0;
            newACMethod["AfterflowMAX"] = (Double)10;
            newACMethod["LackOfMatCheckWeight"] = (Double)2;
            newACMethod["LackOfMatCheckTime"] = TimeSpan.FromSeconds(10);
            newACMethod["PulsationTime"] = TimeSpan.FromSeconds(2);
            newACMethod["PulsationPauseTime"] = TimeSpan.FromSeconds(1);
            newACMethod["AdjustmentTime"] = TimeSpan.FromSeconds(1);
            newACMethod["CalmingTime"] = TimeSpan.FromSeconds(3);
            newACMethod["StandstillTime"] = TimeSpan.FromSeconds(1);
            newACMethod["StandstillWeight"] = (Double)1;
            newACMethod["Temperature"] = (Double)0;
            newACMethod[PWMethodVBBase.IsLastBatchParamName] = (Int16)0;
            newACMethod["SprayNozzle"] = (Int16)0;
            newACMethod["ImpulsePerLiter"] = (Double)1;
            newACMethod["Density"] = (Double)1000;
        }

        [ACMethodInfo("Function", "en{'Inherit params from config'}de{'Übernehme Dosierparameter aus Konfiguration'}", 9999)]
        public virtual void InheritParamsFromConfig(ACMethod newACMethod, ACMethod configACMethod, bool isConfigInitialization)
        {
            if (isConfigInitialization)
            {
                object valueSource = null;
                ACValue acValue = newACMethod.ParameterValueList.GetACValue("Source");
                if (acValue != null)
                    valueSource = acValue.Value;

                newACMethod.ParameterValueList.CopyValues(configACMethod.ParameterValueList);

                try
                {
                    if (acValue != null)
                        newACMethod.ParameterValueList["Source"] = valueSource;
                    newACMethod.ParameterValueList[Material.ClassName] = "";
                    newACMethod.ParameterValueList["PLPosRelation"] = Guid.Empty;
                    newACMethod.ParameterValueList[nameof(Route)] = null;
                    newACMethod.ParameterValueList["TargetQuantity"] = (double)0.0;
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    Messages.LogException("PAFDosing", "InheritParamsFromConfig", msg);
                }

                //newACMethod["FlowRate1"] = configACMethod.ParameterValueList.GetInt16("FlowRate1");
                //newACMethod["FlowRate2"] = configACMethod.ParameterValueList.GetInt16("FlowRate2");
                //newACMethod["FlowRate3"] = configACMethod.ParameterValueList.GetInt16("FlowRate3");
                //newACMethod["FlowRate4"] = configACMethod.ParameterValueList.GetInt16("FlowRate4");
                //newACMethod["FlowRate5"] = configACMethod.ParameterValueList.GetInt16("FlowRate5");
                //newACMethod["FlowRate6"] = configACMethod.ParameterValueList.GetInt16("FlowRate6");
                //newACMethod["FlowRate7"] = configACMethod.ParameterValueList.GetInt16("FlowRate7");
                //newACMethod["FlowRate8"] = configACMethod.ParameterValueList.GetInt16("FlowRate8");
                //newACMethod["FlowRate9"] = configACMethod.ParameterValueList.GetInt16("FlowRate9");
                //newACMethod["FlowSwitching1"] = configACMethod.ParameterValueList.GetDouble("FlowSwitching1");
                //newACMethod["FlowSwitching2"] = configACMethod.ParameterValueList.GetDouble("FlowSwitching2");
                //newACMethod["FlowSwitching3"] = configACMethod.ParameterValueList.GetDouble("FlowSwitching3");
                //newACMethod["FlowSwitching4"] = configACMethod.ParameterValueList.GetDouble("FlowSwitching4");
                //newACMethod["FlowSwitching5"] = configACMethod.ParameterValueList.GetDouble("FlowSwitching5");
                //newACMethod["FlowSwitching6"] = configACMethod.ParameterValueList.GetDouble("FlowSwitching6");
                //newACMethod["FlowSwitching7"] = configACMethod.ParameterValueList.GetDouble("FlowSwitching7");
                //newACMethod["FlowSwitching8"] = configACMethod.ParameterValueList.GetDouble("FlowSwitching8");
                //newACMethod["FlowSwitching9"] = configACMethod.ParameterValueList.GetDouble("FlowSwitching9");
                //newACMethod["RoughFineSwitching"] = configACMethod.ParameterValueList.GetDouble("RoughFineSwitching");
                //newACMethod["FlowRateRough"] = configACMethod.ParameterValueList.GetInt16("FlowRateRough");
                //newACMethod["FlowRateFine"] = configACMethod.ParameterValueList.GetInt16("FlowRateFine");
                //newACMethod["TolerancePlus"] = configACMethod.ParameterValueList.GetDouble("TolerancePlus");
                //newACMethod["ToleranceMinus"] = configACMethod.ParameterValueList.GetDouble("ToleranceMinus");
                //newACMethod["Afterflow"] = configACMethod.ResultValueList.GetDouble("Afterflow");
                //newACMethod["AfterflowMAX"] = configACMethod.ParameterValueList.GetDouble("AfterflowMAX");
                //newACMethod["LackOfMatCheckWeight"] = configACMethod.ParameterValueList.GetDouble("LackOfMatCheckWeight");
                //newACMethod["LackOfMatCheckTime"] = configACMethod.ParameterValueList.GetTimeSpan("LackOfMatCheckTime");
                //newACMethod["PulsationTime"] = configACMethod.ParameterValueList.GetTimeSpan("PulsationTime");
                //newACMethod["PulsationPauseTime"] = configACMethod.ParameterValueList.GetTimeSpan("PulsationPauseTime");
                //newACMethod["AdjustmentTime"] = configACMethod.ParameterValueList.GetTimeSpan("AdjustmentTime");
                //newACMethod["CalmingTime"] = configACMethod.ParameterValueList.GetTimeSpan("CalmingTime");
                //newACMethod["StandstillTime"] = configACMethod.ParameterValueList.GetTimeSpan("StandstillTime");
                //newACMethod["StandstillWeight"] = configACMethod.ParameterValueList.GetDouble("StandstillWeight");
                //newACMethod["Temperature"] = configACMethod.ParameterValueList.GetDouble("Temperature");
                //newACMethod[PWMethodVBBase.IsLastBatchParamName] = configACMethod.ParameterValueList.GetInt16("LastBatch");
                //newACMethod["SprayNozzle"] = configACMethod.ParameterValueList.GetInt16("SprayNozzle");
                //newACMethod["ImpulsePerLiter"] = configACMethod.ParameterValueList.GetDouble("ImpulsePerLiter");
                //newACMethod["Density"] = configACMethod.ParameterValueList.GetDouble("Density");
            }
            else
            {
                if (newACMethod.ParameterValueList.GetInt16("FlowRate1") <= 0)
                    newACMethod["FlowRate1"] = configACMethod.ParameterValueList.GetInt16("FlowRate1");
                if (newACMethod.ParameterValueList.GetInt16("FlowRate2") <= 0)
                    newACMethod["FlowRate2"] = configACMethod.ParameterValueList.GetInt16("FlowRate2");
                if (newACMethod.ParameterValueList.GetInt16("FlowRate3") <= 0)
                    newACMethod["FlowRate3"] = configACMethod.ParameterValueList.GetInt16("FlowRate3");
                if (newACMethod.ParameterValueList.GetInt16("FlowRate4") <= 0)
                    newACMethod["FlowRate4"] = configACMethod.ParameterValueList.GetInt16("FlowRate4");
                if (newACMethod.ParameterValueList.GetInt16("FlowRate5") <= 0)
                    newACMethod["FlowRate5"] = configACMethod.ParameterValueList.GetInt16("FlowRate5");
                if (newACMethod.ParameterValueList.GetInt16("FlowRate6") <= 0)
                    newACMethod["FlowRate6"] = configACMethod.ParameterValueList.GetInt16("FlowRate6");
                if (newACMethod.ParameterValueList.GetInt16("FlowRate7") <= 0)
                    newACMethod["FlowRate7"] = configACMethod.ParameterValueList.GetInt16("FlowRate7");
                if (newACMethod.ParameterValueList.GetInt16("FlowRate8") <= 0)
                    newACMethod["FlowRate8"] = configACMethod.ParameterValueList.GetInt16("FlowRate8");
                if (newACMethod.ParameterValueList.GetInt16("FlowRate9") <= 0)
                    newACMethod["FlowRate9"] = configACMethod.ParameterValueList.GetInt16("FlowRate9");
                if (newACMethod.ParameterValueList.GetDouble("FlowSwitching1") <= 0.000001)
                    newACMethod["FlowSwitching1"] = configACMethod.ParameterValueList.GetDouble("FlowSwitching1");
                if (newACMethod.ParameterValueList.GetDouble("FlowSwitching2") <= 0.000001)
                    newACMethod["FlowSwitching2"] = configACMethod.ParameterValueList.GetDouble("FlowSwitching2");
                if (newACMethod.ParameterValueList.GetDouble("FlowSwitching3") <= 0.000001)
                    newACMethod["FlowSwitching3"] = configACMethod.ParameterValueList.GetDouble("FlowSwitching3");
                if (newACMethod.ParameterValueList.GetDouble("FlowSwitching4") <= 0.000001)
                    newACMethod["FlowSwitching4"] = configACMethod.ParameterValueList.GetDouble("FlowSwitching4");
                if (newACMethod.ParameterValueList.GetDouble("FlowSwitching5") <= 0.000001)
                    newACMethod["FlowSwitching5"] = configACMethod.ParameterValueList.GetDouble("FlowSwitching5");
                if (newACMethod.ParameterValueList.GetDouble("FlowSwitching6") <= 0.000001)
                    newACMethod["FlowSwitching6"] = configACMethod.ParameterValueList.GetDouble("FlowSwitching6");
                if (newACMethod.ParameterValueList.GetDouble("FlowSwitching7") <= 0.000001)
                    newACMethod["FlowSwitching7"] = configACMethod.ParameterValueList.GetDouble("FlowSwitching7");
                if (newACMethod.ParameterValueList.GetDouble("FlowSwitching8") <= 0.000001)
                    newACMethod["FlowSwitching8"] = configACMethod.ParameterValueList.GetDouble("FlowSwitching8");
                if (newACMethod.ParameterValueList.GetDouble("FlowSwitching9") <= 0.000001)
                    newACMethod["FlowSwitching9"] = configACMethod.ParameterValueList.GetDouble("FlowSwitching9");
                if (newACMethod.ParameterValueList.GetDouble("RoughFineSwitching") <= 0.000001)
                    newACMethod["RoughFineSwitching"] = configACMethod.ParameterValueList.GetDouble("RoughFineSwitching");
                if (newACMethod.ParameterValueList.GetInt16("FlowRateRough") <= 0)
                    newACMethod["FlowRateRough"] = configACMethod.ParameterValueList.GetInt16("FlowRateRough");
                if (newACMethod.ParameterValueList.GetInt16("FlowRateFine") <= 0)
                    newACMethod["FlowRateFine"] = configACMethod.ParameterValueList.GetInt16("FlowRateFine");

                double targetQ = newACMethod.ParameterValueList.GetDouble("TargetQuantity");
                double tolPlus = newACMethod.ParameterValueList.GetDouble("TolerancePlus");
                if (Math.Abs(tolPlus) <= Double.Epsilon)
                    tolPlus = configACMethod.ParameterValueList.GetDouble("TolerancePlus");
                tolPlus = RecalcAbsoluteTolerance(tolPlus, targetQ);
                newACMethod["TolerancePlus"] = tolPlus;

                double tolMinus = newACMethod.ParameterValueList.GetDouble("ToleranceMinus");
                if (Math.Abs(tolMinus) <= Double.Epsilon)
                    tolMinus = configACMethod.ParameterValueList.GetDouble("ToleranceMinus");
                tolMinus = RecalcAbsoluteTolerance(tolMinus, targetQ);
                newACMethod["ToleranceMinus"] = tolMinus;

                if (newACMethod.ParameterValueList.GetDouble("Afterflow") <= 0)
                {
                    double afterflowTarget = configACMethod.ParameterValueList.GetDouble("Afterflow");
                    double afterflowLastResult = configACMethod.ResultValueList.GetDouble("Afterflow");
                    // Falls negativ, dann forcisert Benutzer den neuen Nachlaufwert
                    if (afterflowTarget <= -0.000001)
                    {
                        afterflowTarget = Math.Abs(afterflowTarget);
                        newACMethod["Afterflow"] = afterflowTarget;
                    }
                    else if (afterflowLastResult >= -0.000001)
                    {
                        afterflowLastResult = Math.Abs(afterflowLastResult);
                        newACMethod["Afterflow"] = afterflowLastResult;
                    }
                    else
                        newACMethod["Afterflow"] = afterflowTarget;
                }
                if (newACMethod.ParameterValueList.GetDouble("AfterflowMAX") <= 0)
                    newACMethod["AfterflowMAX"] = configACMethod.ParameterValueList.GetDouble("AfterflowMAX");
                if (newACMethod.ParameterValueList.GetDouble("LackOfMatCheckWeight") <= 0)
                    newACMethod["LackOfMatCheckWeight"] = configACMethod.ParameterValueList.GetDouble("LackOfMatCheckWeight");
                if (newACMethod.ParameterValueList.GetTimeSpan("LackOfMatCheckTime") == TimeSpan.Zero)
                    newACMethod["LackOfMatCheckTime"] = configACMethod.ParameterValueList.GetTimeSpan("LackOfMatCheckTime");
                if (newACMethod.ParameterValueList.GetTimeSpan("PulsationTime") == TimeSpan.Zero)
                    newACMethod["PulsationTime"] = configACMethod.ParameterValueList.GetTimeSpan("PulsationTime");
                if (newACMethod.ParameterValueList.GetTimeSpan("PulsationPauseTime") == TimeSpan.Zero)
                    newACMethod["PulsationPauseTime"] = configACMethod.ParameterValueList.GetTimeSpan("PulsationPauseTime");
                if (newACMethod.ParameterValueList.GetTimeSpan("AdjustmentTime") == TimeSpan.Zero)
                    newACMethod["AdjustmentTime"] = configACMethod.ParameterValueList.GetTimeSpan("AdjustmentTime");
                if (newACMethod.ParameterValueList.GetTimeSpan("CalmingTime") == TimeSpan.Zero)
                    newACMethod["CalmingTime"] = configACMethod.ParameterValueList.GetTimeSpan("CalmingTime");
                if (newACMethod.ParameterValueList.GetTimeSpan("StandstillTime") == TimeSpan.Zero)
                    newACMethod["StandstillTime"] = configACMethod.ParameterValueList.GetTimeSpan("StandstillTime");
                if (newACMethod.ParameterValueList.GetDouble("StandstillWeight") <= 0)
                    newACMethod["StandstillWeight"] = configACMethod.ParameterValueList.GetDouble("StandstillWeight");
                if (newACMethod.ParameterValueList.GetDouble("Temperature") <= 0)
                    newACMethod["Temperature"] = configACMethod.ParameterValueList.GetDouble("Temperature");
                if (newACMethod.ParameterValueList.GetInt16(PWMethodVBBase.IsLastBatchParamName) <= 0)
                    newACMethod[PWMethodVBBase.IsLastBatchParamName] = configACMethod.ParameterValueList.GetInt16(PWMethodVBBase.IsLastBatchParamName);
                if (newACMethod.ParameterValueList.GetInt16("SprayNozzle") <= 0)
                    newACMethod["SprayNozzle"] = configACMethod.ParameterValueList.GetInt16("SprayNozzle");
                if (newACMethod.ParameterValueList.GetDouble("ImpulsePerLiter") <= 0)
                    newACMethod["ImpulsePerLiter"] = configACMethod.ParameterValueList.GetDouble("ImpulsePerLiter");
                if (newACMethod.ParameterValueList.GetDouble("Density") <= 0)
                    newACMethod["Density"] = configACMethod.ParameterValueList.GetDouble("Density");
                if (newACMethod.ParameterValueList.GetTimeSpan("MaxDosingTime") == TimeSpan.Zero)
                    newACMethod["MaxDosingTime"] = configACMethod.ParameterValueList.GetTimeSpan("MaxDosingTime");
                if (newACMethod.ParameterValueList.GetInt16("AdjustmentFlowRate") <= 0)
                    newACMethod["AdjustmentFlowRate"] = configACMethod.ParameterValueList.GetInt16("AdjustmentFlowRate");
            }
        }

        public static double RecalcAbsoluteTolerance(double tolValue, double targetQ, double? forceDefaultPerc = 0.05)
        {
            // Falls Toleranz negativ, dann ist die Toleranz in % angegeben
            if (tolValue < -0.0000001)
            {
                if (Math.Abs(targetQ) > Double.Epsilon)
                    tolValue = targetQ * tolValue * -0.01;
                else
                    tolValue = 0.001;
            }
            else if (Math.Abs(tolValue) <= Double.Epsilon && forceDefaultPerc.HasValue)
            {
                if (Math.Abs(targetQ) > Double.Epsilon)
                    tolValue = targetQ * forceDefaultPerc.Value;
                else
                    tolValue = 0.001;
            }
            return tolValue;
        }
        #endregion

    }

}
