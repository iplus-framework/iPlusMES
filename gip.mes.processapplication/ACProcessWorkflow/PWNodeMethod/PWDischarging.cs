using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System.Xml;

namespace gip.mes.processapplication
{
    /// <summary>
    /// Class that is responsible for transporting materials from one point to another. 
    /// These may be either raw materials, intermediates or end products. 
    /// The workflow node can also be linked to an intermediate product in a material workflow. 
    /// PWDischarging is used for fully automatic production. 
    /// It calls the PAFDischarging process function asynchronously to delegate the real-time critical tasks to a PLC controller. 
    /// Manufactured, received or relocated quantities are posted via warehouse management (ACFacilityManager). 
    /// It can work with different data contexts (production and picking orders or delivery notes).
    /// </summary>
    /// <seealso cref="gip.core.autocomponent.PWNodeProcessMethod" />
    /// <seealso cref="gip.core.autocomponent.IPWNodeDeliverMaterial" />
    /// <seealso cref="gip.core.autocomponent.IACMyConfigCache" />
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PWDischarging'}de{'PWDischarging'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public partial class PWDischarging : PWNodeProcessMethod, IPWNodeDeliverMaterial
    {
        public const string PWClassName = "PWDischarging";

        #region c´tors
        static PWDischarging()
        {
            ACMethod method;
            method = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("SkipIfNoComp", typeof(bool), false, Global.ParamOption.Required));
            paramTranslation.Add("SkipIfNoComp", "en{'Skip if no components dosed'}de{'Überspringe wenn keine Komponente dosiert'}");
            method.ParameterValueList.Add(new ACValue("LimitToMaxCapOfDest", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("LimitToMaxCapOfDest", "en{'Limit filling of destination to available space'}de{'Limitiere Zielbefüllung auf rechnerischen Restinhalt'}");
            method.ParameterValueList.Add(new ACValue("PrePostQOnDest", typeof(double), 0.0, Global.ParamOption.Optional));
            paramTranslation.Add("PrePostQOnDest", "en{'Pre posting quantity to destination at start'}de{'Vorbuchungsmenge auf Ziel bei Start'}");
            method.ParameterValueList.Add(new ACValue("NoPostingOnRelocation", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("NoPostingOnRelocation", "en{'No posting at relocation'}de{'Keine Buchung bei Umlagerung'}");
            method.ParameterValueList.Add(new ACValue("KeepPlannedDestOnEmptying", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("KeepPlannedDestOnEmptying", "en{'Keep planned destination on emptying mode'}de{'Geplantes Ziel im Entleerungsmodus beibehalten'}");
            method.ParameterValueList.Add(new ACValue("SkipPredCount", typeof(short), 0, Global.ParamOption.Optional));
            paramTranslation.Add("SkipPredCount", "en{'Search limit predecessors (- outside of group)'}de{'Begrenzung Vorgängersuche (- ausserhalb von Gruppe)'}");
            method.ParameterValueList.Add(new ACValue("PostingBehaviour", typeof(PostingBehaviourEnum), PostingBehaviourEnum.NotSet, Global.ParamOption.Optional));
            paramTranslation.Add("PostingBehaviour", "en{'Posting behaviour'}de{'Buchungsverhalten'}");
            method.ParameterValueList.Add(new ACValue("IgnorePredecessors", typeof(string), null, Global.ParamOption.Optional));
            paramTranslation.Add("IgnorePredecessors", "en{'Ignore predecessor groups'}de{'Ignoriere Vorgangsgruppen'}");
            method.ParameterValueList.Add(new ACValue("LossCorrectionFactor", typeof(double?), null, Global.ParamOption.Optional));
            paramTranslation.Add("LossCorrectionFactor", "en{'Loss correction factor 0 = bill of material factor, n=%'}de{'Verlustfaktor 0 = Rezept Faktor, n=%'}");

            var wrapper = new ACMethodWrapper(method, "en{'Configuration'}de{'Konfiguration'}", typeof(PWDischarging), paramTranslation, null);
            ACMethod.RegisterVirtualMethod(typeof(PWDischarging), ACStateConst.SMStarting, wrapper);
            RegisterExecuteHandler(typeof(PWDischarging), HandleExecuteACMethod_PWDischarging);
        }

        public PWDischarging(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            using (ACMonitor.Lock(_20015_LockValue))
            {
                _CacheModuleDestinations = null;
                _LastTargets = null;
                _NoTargetWait = null;
                _NoTargetLongWait = null;
                _CheckIfAutomaticTargetChangePossible = null;
                _IsWaitingOnTarget = false;
                _CurrentDischargingRoute = null;
            }

            CurrentDisEntityID.ValueT = Guid.Empty;

            return base.ACDeInit(deleteACClassTask);
        }

        public override void Recycle(IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {
            using (ACMonitor.Lock(_20015_LockValue))
            {
                _CacheModuleDestinations = null;
                _LastTargets = null;
                _NoTargetWait = null;
                _NoTargetLongWait = null;
                _CheckIfAutomaticTargetChangePossible = null;
                _IsWaitingOnTarget = false;
                _CurrentDischargingRoute = null;
            }

            CurrentDisEntityID.ValueT = Guid.Empty;

            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }
        #endregion

        #region Enums and Const

        public enum StartDisResult
        {
            WaitForCallback = 0,
            CycleWait = 1,
            FastCycleWait = 2,
            CancelDischarging = 3
        }

        #endregion

        #region Properties
        public T ParentPWMethod<T>() where T : PWMethodVBBase
        {
            if (ParentRootWFNode == null)
                return null;
            return ParentRootWFNode as T;
        }

        public bool IsProduction
        {
            get
            {
                return ParentPWMethod<PWMethodProduction>() != null;
            }
        }

        public bool IsIntake
        {
            get
            {
                return ParentPWMethod<PWMethodIntake>() != null;
            }
        }

        public bool IsLoading
        {
            get
            {
                return ParentPWMethod<PWMethodLoading>() != null;
            }
        }

        public bool IsRelocation
        {
            get
            {
                return ParentPWMethod<PWMethodRelocation>() != null;
            }
        }

        public bool IsTransport
        {
            get
            {
                return ParentPWMethod<PWMethodTransportBase>() != null;
            }
        }

        public virtual bool IsLastDischargingNode
        {
            get
            {
                return false;
            }
        }


        protected FacilityManager ACFacilityManager
        {
            get
            {
                PWMethodVBBase pwMethodTransport = ParentPWMethod<PWMethodVBBase>();
                return pwMethodTransport != null ? pwMethodTransport.ACFacilityManager : null;
            }
        }


        public PAFDischarging CurrentExecutingFunction
        {
            get
            {
                return GetCurrentExecutingFunction<PAFDischarging>();
            }
        }

        private Route _CurrentDischargingRoute;
        [ACPropertyInfo(true, 9999, "", "", "", false)]
        public Route CurrentDischargingRoute
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _CurrentDischargingRoute;
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _CurrentDischargingRoute = value;
                }
                OnPropertyChanged("CurrentDischargingRoute");
            }
        }

        private DateTime? _NoTargetWait;
        protected DateTime? NoTargetWait
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _NoTargetWait;
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _NoTargetWait = value;
                }
            }
        }


        private bool? _CheckIfAutomaticTargetChangePossible = null;
        protected bool? CheckIfAutomaticTargetChangePossible
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _CheckIfAutomaticTargetChangePossible;
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _CheckIfAutomaticTargetChangePossible = value;
                }
            }
        }

        private bool _IsWaitingOnTarget;
        public bool IsWaitingOnTarget
        {
            get
            {
                bool isWaitingOnTarget;

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    isWaitingOnTarget = _IsWaitingOnTarget;
                }

                return (NoTargetWait.HasValue || isWaitingOnTarget) && IsSubscribedToWorkCycle;
            }
            protected set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _IsWaitingOnTarget = value;
                }
            }
        }

        /// <summary>
        /// If Discharging is to a Processmodule, then the Routing-Results for possible destination-modules is cached here.
        /// _LastTargets is null
        /// </summary>
        private Guid[] _CacheModuleDestinations = null;
        protected Guid[] CacheModuleDestinations
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_CacheModuleDestinations == null)
                        return null;
                    return _CacheModuleDestinations.ToArray();
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _CacheModuleDestinations = value;
                }
            }
        }

        /// <summary>
        /// If Discharging is to a Facility/Silo, then the last determined Targets are cached here
        /// _CacheModuleDestinations is null
        /// </summary>
        private Guid[] _LastTargets = null;
        protected Guid[] LastTargets
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_LastTargets == null)
                        return null;
                    return _LastTargets.ToArray();
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _LastTargets = value;
                }
            }
        }


        private DateTime? _NoTargetLongWait;
        protected DateTime? NoTargetLongWait
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _NoTargetLongWait;
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _NoTargetLongWait = value;
                }
            }
        }

        protected bool SkipIfNoComp
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("SkipIfNoComp");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }


        protected short SkipPredCount
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("SkipPredCount");
                    if (acValue != null)
                    {
                        return acValue.ParamAsInt16;
                    }
                }
                return 0;
            }
        }

        protected string IgnorePredecessors
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("IgnorePredecessors");
                    if (acValue != null)
                    {
                        return acValue.ParamAsString;
                    }
                }
                return null;
            }
        }

        protected bool LimitToMaxCapOfDest
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("LimitToMaxCapOfDest");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        protected bool KeepPlannedDestOnEmptying
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("KeepPlannedDestOnEmptying");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        protected double PrePostQOnDest
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("PrePostQOnDest");
                    if (acValue != null)
                    {
                        return acValue.ParamAsDouble;
                    }
                }
                return 0.0;
            }
        }

        protected bool NoPostingOnRelocation
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("NoPostingOnRelocation");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        public PostingBehaviourEnum PostingBehaviour
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("PostingBehaviour");
                    if (acValue != null)
                    {
                        return (PostingBehaviourEnum)acValue.ParamAsInt16;
                    }
                }
                return PostingBehaviourEnum.NotSet;
            }
        }

        [ACPropertyBindingSource(9999, "", "", "", false, true)]
        public IACContainerTNet<Guid> CurrentDisEntityID { get; set; }

        public override bool IsSkippable
        {
            get
            {
                return false;
            }
        }

        protected double? LossCorrectionFactor
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("LossCorrectionFactor");
                    if (acValue != null && acValue.Value != null)
                    {
                        return acValue.ParamAsDouble;
                    }
                }
                return null;
            }
        }

        #endregion

        #region Methods

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "EnterExtraDisTargetDest":
                    EnterExtraDisTargetDest();
                    return true;
                case Const.IsEnabledPrefix + "EnterExtraDisTargetDest":
                    result = IsEnabledEnterExtraDisTargetDest();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PWDischarging(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWNodeProcessMethod(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
#endregion


#region ACState

        public override void SMIdle()
        {
            CacheModuleDestinations = null;
            LastTargets = null;
            base.SMIdle();
        }

        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            var pwGroup = ParentPWGroup;
            if (!CheckParentGroupAndHandleSkipMode())
                return;

            if (   ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMBatchCancelled)
                || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMBatchCancelled))
            {
                // Falls durch tiefere Callstacks der Status schon weitergeschaltet worden ist, dann schalte Status nicht weiter
                if (CurrentACState == ACStateEnum.SMStarting)
                    CurrentACState = ACStateEnum.SMCompleted;
                return;
            }
            if (SkipIfNoComp)
            {
                bool hasRunSomeDosings = false;
                List<IPWNodeReceiveMaterial> previousDosings = PWDosing.FindPreviousDosingsInPWGroup<IPWNodeReceiveMaterial>(this, SkipPredCount == 0 ? 40 : Math.Abs(SkipPredCount), SkipPredCount >= 0, IgnorePredecessors);
                if (previousDosings != null)
                    hasRunSomeDosings = previousDosings.Where(c => c.HasRunSomeDosings).Any();
                if (!hasRunSomeDosings)
                {
                    if (CurrentACState == ACStateEnum.SMStarting)
                        CurrentACState = ACStateEnum.SMCompleted;
                    return;
                }
            }

            core.datamodel.ACClassMethod refPAACClassMethod = RefACClassMethodOfContentWF;
            if (refPAACClassMethod != null)
            {
                PAProcessModule module = null;
                if (ParentPWGroup.NeedsAProcessModule && (ACOperationMode == ACOperationModes.Live || ACOperationMode == ACOperationModes.Simulation))
                    module = ParentPWGroup.AccessedProcessModule;
                // Testmode
                else
                    module = ParentPWGroup.ProcessModuleForTestmode;

                if (module == null)
                {
                    // TODO: Meldung: Programmfehler, darf nicht vorkommen
                    return;
                }

                if (NoTargetWait.HasValue && DateTime.Now < NoTargetWait.Value) // Zyklisches Warten um zyklische Datenbankabfragen zu minimieren
                    return;
                NoTargetWait = null;
                CheckIfAutomaticTargetChangePossible = null;
                StartDisResult result = StartDischarging(module);
                if (result == StartDisResult.CycleWait || result == StartDisResult.FastCycleWait)
                {
                    if (result == StartDisResult.CycleWait)
                        NoTargetWait = DateTime.Now + TimeSpan.FromSeconds(10);
                    IsWaitingOnTarget = true;
                    SubscribeToProjectWorkCycle();
                    return;
                }
                else if (result == StartDisResult.WaitForCallback)
                {
                    IsWaitingOnTarget = false;
                    NoTargetWait = null;
                    // Falls durch tiefere Callstacks der Status schon weitergeschaltet worden ist, dann schalte Status nicht weiter
                    if (CurrentACState == ACStateEnum.SMStarting)
                        CurrentACState = ACStateEnum.SMRunning;
                    return;
                }
                else
                {
                    IsWaitingOnTarget = false;
                    NoTargetWait = null;
                    // Falls durch tiefere Callstacks der Status schon weitergeschaltet worden ist, dann schalte Status nicht weiter
                    if (CurrentACState == ACStateEnum.SMStarting)
                        CurrentACState = ACStateEnum.SMCompleted;
                    return;
                }
            }

            // Falls module.AddTask synchron ausgeführt wurde, dann ist der Status schon weiter
            if (CurrentACState == ACStateEnum.SMStarting)
            {
                CurrentACState = ACStateEnum.SMRunning;
                //PostExecute(PABaseState.SMStarting);
            }
        }

        public override void SMRunning()
        {
            if (NoTargetWait.HasValue && DateTime.Now < NoTargetWait.Value) // Zyklisches Warten um zyklische Datenbankabfragen zu minimieren
                return;

            // TODO: Check Vollmelder => Automatischer Zielwschsel
            PAFDischarging discharging = CurrentExecutingFunction;

            // Falls Dosierung zur Zeit aktiv ist, dann gibt es auch einen Eintrag in der TaskSubscriptionListe
            if (discharging != null)
            {
                SubscribeToProjectWorkCycle();
                if (discharging.CurrentACState != ACStateEnum.SMIdle)
                {
                    StartDisResult result = OnHandleStateCheckFullSilo(discharging);
                    if (result == StartDisResult.CycleWait)
                    {
                        NoTargetWait = DateTime.Now + TimeSpan.FromSeconds(10);
                        return;
                    }
                }
            }
            NoTargetWait = null;
            IsWaitingOnTarget = false;
        }

        public virtual StartDisResult StartDischarging(PAProcessModule module)
        {
            // TODO Handle Methods for In- / Out- and Relocation-Methods
            if (!IsProduction && !IsIntake && !IsRelocation)
                return StartDisResult.CancelDischarging;

            // Falls Produktionsauftrag
            if (IsProduction)
            {
                return StartDischargingProd(module);
            }
            else if (IsIntake)
            {
                return StartDischargingIntake(module);
            }
            else if (IsRelocation)
            {
                return StartDischargingRelocation(module);
            }
            else if (IsLoading)
            {
                return StartDischargingLoading(module);
            }
            return StartDisResult.CancelDischarging;
        }

        /// <summary>
        /// 2.1 Zyklische Aufrufmethode bei Zielvollmeldung
        /// </summary>
        public virtual StartDisResult OnHandleStateCheckFullSilo(PAFDischarging discharging)
        {
            // TODO: Handle Methods Out- and Relocation-Methods
            if (!IsProduction && !IsIntake && !IsRelocation && !IsLoading)
                return StartDisResult.WaitForCallback;

            if (!IsConditionForDestinationChange(discharging))
            {
                NoTargetWait = null;
                return StartDisResult.WaitForCallback;
            }
            else if (!discharging.FaultAckDestinationFull.ValueT && CheckIfAutomaticTargetChangePossible.HasValue)
            {
                return StartDisResult.CycleWait;
            }
            else if (discharging.FaultAckDestinationFull.ValueT && CheckIfAutomaticTargetChangePossible.HasValue)
            {
                CheckIfAutomaticTargetChangePossible = null;
                discharging.FaultAckDestinationFull.ValueT = false;
            }

            // Wenn Ziel-Voll-Meldung kommt aber der Schritt breits im Leerfahren ist dann keinen Zielwechsel mehr durchführen
            if (!CanChangeDestOnStopping && discharging.CurrentACState == ACStateEnum.SMStopping)
            {
                discharging.AcknowledgeAlarms();
                return StartDisResult.WaitForCallback;
            }

            // If tragetSilo is null, then dicharging is to Processmodule and not to Silo
            PAProcessModule targetModule = TargetPAModule(null);
            if (targetModule == null)
            {
                return StartDisResult.WaitForCallback;
            }

            // Falls Produktionsauftrag
            if (IsProduction)
            {
                return OnHandleStateCheckFullSiloProd(discharging, targetModule);
            }
            else if (IsIntake && ParentPWGroup != null)
            {
                return OnHandleStateCheckFullSiloIntake(discharging, targetModule, ParentPWGroup.AccessedProcessModule);
            }
            else if (IsRelocation && ParentPWGroup != null)
            {
                return OnHandleStateCheckFullSiloRelocation(discharging, targetModule, ParentPWGroup.AccessedProcessModule);
            }
            else if (IsLoading && ParentPWGroup != null)
            {
                return OnHandleStateCheckFullSiloLoading(discharging, targetModule, ParentPWGroup.AccessedProcessModule);
            }
            return StartDisResult.WaitForCallback;
        }

        public virtual bool IsConditionForDestinationChange(PAFDischarging discharging)
        {
            return discharging.StateDestinationFull.ValueT != PANotifyState.Off;
        }

        protected virtual bool CanChangeDestOnStopping
        {
            get
            {
                return false;
            }
        }

        public override void SMCompleted()
        {
            if (ParentPWGroup != null)
            {
                IPAMCont cont = ParentPWGroup.AccessedProcessModule as IPAMCont;
                if (cont != null)
                    cont.ResetFillVolume();
            }

            base.SMCompleted();
        }

        #endregion


        #region Callback
        public override void TaskCallback(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject)
        {
            _InCallback = true;
            try
            {
                if (e != null)
                {
                    IACTask taskEntry = wrapObject as IACTask;
                    ACMethodEventArgs eM = e as ACMethodEventArgs;
                    _CurrentMethodEventArgs = eM;
                    if (taskEntry.State == PointProcessingState.Deleted && CurrentACState != ACStateEnum.SMIdle)
                    {
                        CheckIfAutomaticTargetChangePossible = null;
                        ACMethod acMethod = e.ParentACMethod;
                        if (acMethod == null)
                            acMethod = taskEntry.ACMethod;
                        if (ParentPWGroup == null)
                        {
                            Messages.LogError(this.GetACUrl(), "TaskCallback()", "ParentPWGroup is null");
                            return;
                        }
                        PAProcessFunction discharging = ParentPWGroup.GetExecutingFunction<PAProcessFunction>(taskEntry.RequestID);
                        CheckIfAutomaticTargetChangePossible = null;
                        if (discharging != null)
                        {
                            double actualWeight = 0;
                            var acValue = e.GetACValue("ActualQuantity");
                            if (acValue != null)
                                actualWeight = acValue.ParamAsDouble;
                            //short simulationWeight = (short)acMethod["Source"];

                            if (OnTaskCallbackCanExecutePostings(sender, e, wrapObject, discharging, acMethod))
                            {
                                using (var dbIPlus = new Database())
                                using (var dbApp = new DatabaseApp(dbIPlus))
                                {
                                    ProdOrderPartslistPos currentBatchPos = null;
                                    if (IsProduction)
                                    {
                                        currentBatchPos = ParentPWMethod<PWMethodProduction>().CurrentProdOrderPartslistPos.FromAppContext<ProdOrderPartslistPos>(dbApp);
                                        // Wenn kein Istwert von der Funktion zurückgekommen, dann berechne Zugangsmenge über die Summe der dosierten Mengen
                                        // Minus der bereits zugebuchten Menge (falls zyklische Zugagnsbuchungen im Hintergrund erfolgten)
                                        OnTaskCallbackCheckQuantity(eM, taskEntry, acMethod, dbApp, dbIPlus, currentBatchPos, ref actualWeight);

                                        bool exceptionHandled = false;
                                        var routeItem = CurrentDischargingDest(dbIPlus, true);
                                        PAProcessModule targetModule = TargetPAModule(dbIPlus); // If Discharging is to Processmodule, then targetSilo ist null
                                        if (routeItem != null && targetModule != null)
                                        {
                                            try
                                            {
                                                DoInwardBooking(actualWeight, dbApp, routeItem, null, currentBatchPos, e, true);
                                            }
                                            catch (Exception ex)
                                            {
                                                Messages.LogException(this.GetACUrl(), "TaskCallback(20)", ex);
                                                ProcessAlarm.ValueT = PANotifyState.AlarmOrFault;
                                                OnNewAlarmOccurred(ProcessAlarm, new Msg(ex.Message, this, eMsgLevel.Error, PWClassName, "TaskCallback", 1020), true);
                                                discharging.FunctionError.ValueT = PANotifyState.AlarmOrFault;
                                                discharging.OnNewAlarmOccurred(discharging.FunctionError, new Msg(ex.Message, discharging, eMsgLevel.Error, nameof(PAFDischarging), "TaskCallback", 1020), true);
                                                exceptionHandled = true;
                                            }
                                            finally
                                            {
                                                routeItem.Detach();
                                            }
                                        }

                                        if (ParentPWGroup != null)
                                        {
                                            List<IPWNodeReceiveMaterial> previousDosings = PWDosing.FindPreviousDosingsInPWGroup<IPWNodeReceiveMaterial>(this);
                                            if (previousDosings != null)
                                            {
                                                foreach (var pwDosing in previousDosings)
                                                {
                                                    if (   (((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMDisThenNextComp) && IsProduction)
                                                        || (((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMInterDischarging) && IsTransport)
                                                        )
                                                        pwDosing.ResetDosingsAfterInterDischarging(dbApp);
                                                    else
                                                        pwDosing.SetDosingsCompletedAfterDischarging(dbApp);
                                                }
                                            }
                                        }
                                        if (dbApp.IsChanged)
                                        {
                                            if (exceptionHandled)
                                                dbApp.ACUndoChanges();
                                            else
                                                dbApp.ACSaveChanges();
                                        }
                                    }
                                    else if (IsTransport)
                                    {
                                        if (this.IsSimulationOn && actualWeight <= 0.000001)
                                        {
                                            ACValue acValueTargetQ = acMethod.ParameterValueList.GetACValue("TargetQuantity");
                                            if (acValueTargetQ != null)
                                                actualWeight = acValueTargetQ.ParamAsDouble;
                                        }

                                        var routeItem = CurrentDischargingDest(dbIPlus, true);
                                        PAProcessModule targetModule = TargetPAModule(dbIPlus); // If Discharging is to Processmodule, then targetSilo ist null
                                        if (routeItem != null && targetModule != null)
                                        {
                                            try
                                            {
                                                if (IsIntake)
                                                {
                                                    var pwMethod = ParentPWMethod<PWMethodIntake>();
                                                    Picking picking = null;
                                                    DeliveryNotePos notePos = null;
                                                    FacilityBooking fBooking = null;
                                                    if (pwMethod.CurrentPicking != null)
                                                    {
                                                        picking = pwMethod.CurrentPicking.FromAppContext<Picking>(dbApp);
                                                        PickingPos pickingPos = pwMethod.CurrentPickingPos != null ? pwMethod.CurrentPickingPos.FromAppContext<PickingPos>(dbApp) : null;
                                                        if (picking != null)
                                                            DoInwardBooking(actualWeight, dbApp, routeItem, picking, pickingPos, e, true);
                                                    }
                                                    else if (pwMethod.CurrentDeliveryNotePos != null)
                                                    {
                                                        notePos = pwMethod.CurrentDeliveryNotePos.FromAppContext<DeliveryNotePos>(dbApp);
                                                        if (notePos != null)
                                                            DoInwardBooking(actualWeight, dbApp, routeItem, notePos, e, true);
                                                    }
                                                    else if (pwMethod.CurrentFacilityBooking != null)
                                                    {
                                                        fBooking = pwMethod.CurrentFacilityBooking.FromAppContext<FacilityBooking>(dbApp);
                                                        if (fBooking != null)
                                                            DoInwardBooking(actualWeight, dbApp, routeItem, fBooking, e, true);
                                                    }
                                                }
                                                else if (IsRelocation)
                                                {
                                                    var pwMethod = ParentPWMethod<PWMethodRelocation>();
                                                    Picking picking = null;
                                                    FacilityBooking fBooking = null;
                                                    if (pwMethod.CurrentPicking != null)
                                                    {
                                                        picking = pwMethod.CurrentPicking.FromAppContext<Picking>(dbApp);
                                                        PickingPos pickingPos = pwMethod.CurrentPickingPos != null ? pwMethod.CurrentPickingPos.FromAppContext<PickingPos>(dbApp) : null;
                                                        if (picking != null)
                                                        {
                                                            if (this.IsSimulationOn && actualWeight <= 0.000001 && pickingPos != null)
                                                                actualWeight = pickingPos.TargetQuantityUOM;
                                                            DoInwardBooking(actualWeight, dbApp, routeItem, picking, pickingPos, e, true);
                                                        }
                                                    }
                                                    else if (pwMethod.CurrentFacilityBooking != null)
                                                    {
                                                        fBooking = pwMethod.CurrentFacilityBooking.FromAppContext<FacilityBooking>(dbApp);
                                                        if (fBooking != null)
                                                            DoInwardBooking(actualWeight, dbApp, routeItem, fBooking, e, true);
                                                    }
                                                }
                                                else if (IsLoading)
                                                {
                                                    var pwMethod = ParentPWMethod<PWMethodLoading>();
                                                    Picking picking = null;
                                                    DeliveryNotePos notePos = null;
                                                    if (pwMethod.CurrentPicking != null)
                                                    {
                                                        picking = pwMethod.CurrentPicking.FromAppContext<Picking>(dbApp);
                                                        PickingPos pickingPos = pwMethod.CurrentPickingPos != null ? pwMethod.CurrentPickingPos.FromAppContext<PickingPos>(dbApp) : null;
                                                        if (picking != null)
                                                            DoOutwardBooking(actualWeight, dbApp, routeItem, picking, pickingPos, e, true);
                                                    }
                                                    else if (pwMethod.CurrentDeliveryNotePos != null)
                                                    {
                                                        notePos = pwMethod.CurrentDeliveryNotePos.FromAppContext<DeliveryNotePos>(dbApp);
                                                        if (notePos != null)
                                                            DoOutwardBooking(actualWeight, dbApp, routeItem, notePos, e, true);
                                                    }
                                                }
                                            }
                                            finally
                                            {
                                                routeItem.Detach();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        UnSubscribeToProjectWorkCycle();
                        _LastCallbackResult = e;
                        CurrentACState = ACStateEnum.SMCompleted;
                    }
                    else if (PWPointRunning != null && eM != null && eM.ResultState == Global.ACMethodResultState.InProcess && taskEntry.State == PointProcessingState.Accepted)
                    {
                        PAProcessModule module = sender.ParentACComponent as PAProcessModule;
                        if (module != null)
                        {
                            PAProcessFunction function = module.GetExecutingFunction<PAProcessFunction>(eM.ACRequestID);
                            if (function != null)
                            {
                                if (function.CurrentACState == ACStateEnum.SMRunning)
                                {
                                    ACEventArgs eventArgs = ACEventArgs.GetVirtualEventArgs("PWPointRunning", VirtualEventArgs);
                                    eventArgs.GetACValue("TimeInfo").Value = RecalcTimeInfo();
                                    PWPointRunning.Raise(eventArgs);
                                }
                            }
                        }
                    }
                    else if (taskEntry.State == PointProcessingState.Rejected)
                    {
                        //ACMethodEventArgs eMethodEventArgs = e as ACMethodEventArgs;
                        //if (eMethodEventArgs != null && eMethodEventArgs.ResultState == Global.ACMethodResultState.Failed)
                        //{
                        //}
                    }
                }
            }
            finally
            {
                _InCallback = false;
            }
        }

        protected virtual bool OnTaskCallbackCanExecutePostings(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject, PAProcessFunction discharging, ACMethod acMethod)
        {
            return true;
        }
        #endregion


        #region Planned Silos
        public virtual FacilityReservation GetNextFreeDestination(IList<FacilityReservation> plannedSilos, ProdOrderPartslistPos pPos, 
            bool changeReservationStateIfFull, FacilityReservation ignoreFullSilo = null, PAFDischarging discharging = null)
        {
            if (plannedSilos == null || !plannedSilos.Any())
                return null;
            foreach (FacilityReservation plannedSilo in plannedSilos.Where(c => c.ReservationState == GlobalApp.ReservationState.Active))
            {
                if (CheckPlannedDestinationSilo(plannedSilo, pPos, changeReservationStateIfFull, ignoreFullSilo))
                    return plannedSilo;
            }
            foreach (FacilityReservation plannedSilo in plannedSilos.Where(c => c.ReservationState == GlobalApp.ReservationState.New))
            {
                if (CheckPlannedDestinationSilo(plannedSilo, pPos, changeReservationStateIfFull, ignoreFullSilo))
                    return plannedSilo;
            }
            foreach (FacilityReservation plannedSilo in plannedSilos.Where(c => c.ReservationState == GlobalApp.ReservationState.Finished))
            {
                if (CheckPlannedDestinationSilo(plannedSilo, pPos, changeReservationStateIfFull, ignoreFullSilo))
                {
                    plannedSilo.ReservationState = GlobalApp.ReservationState.New;
                    return plannedSilo;
                }
            }
            return null;
        }

        public static FacilityReservation GetNextFreeDestination(ACComponent invoker, IList<FacilityReservation> plannedSilos, ProdOrderPartslistPos pPos, 
            bool changeReservationStateIfFull, FacilityReservation ignoreFullSilo = null)
        {
            if (plannedSilos == null || !plannedSilos.Any())
                return null;
            foreach (FacilityReservation plannedSilo in plannedSilos.Where(c => c.ReservationState == GlobalApp.ReservationState.Active))
            {
                if (CheckPlannedDestinationSilo(invoker, plannedSilo, pPos, changeReservationStateIfFull, ignoreFullSilo))
                    return plannedSilo;
            }
            foreach (FacilityReservation plannedSilo in plannedSilos.Where(c => c.ReservationState == GlobalApp.ReservationState.New))
            {
                if (CheckPlannedDestinationSilo(invoker, plannedSilo, pPos, changeReservationStateIfFull, ignoreFullSilo))
                    return plannedSilo;
            }
            foreach (FacilityReservation plannedSilo in plannedSilos.Where(c => c.ReservationState == GlobalApp.ReservationState.Finished))
            {
                if (CheckPlannedDestinationSilo(invoker, plannedSilo, pPos, changeReservationStateIfFull, ignoreFullSilo))
                {
                    plannedSilo.ReservationState = GlobalApp.ReservationState.New;
                    return plannedSilo;
                }
            }
            return null;
        }

        protected virtual bool CheckPlannedDestinationSilo(FacilityReservation plannedSilo, ProdOrderPartslistPos batchPos, bool changeReservationStateIfFull, FacilityReservation ignoreFullSilo = null, int additionalSearchFlags = 0)
        {            
            if (plannedSilo == null || (ignoreFullSilo != null && ignoreFullSilo == plannedSilo))
                return false;
            if (plannedSilo.Facility != null
                && plannedSilo.Facility.InwardEnabled)
            {
                if (plannedSilo.Facility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                {
                    // Entweder ist das Silo überhaupt nicht mit einem MAterial belegt
                    // Oder wenn es mit einem Material belegt ist überpüfe ob Material bzw. Produktionsmaterialnummern übereinstimmen
                    // Falls es sich um ein Zwischneprodukt handelt dann überprüfe auch die Rezeptnummer
                    if (   !plannedSilo.Facility.MaterialID.HasValue
                        || (   (    (batchPos.BookingMaterial.ProductionMaterialID.HasValue && plannedSilo.Facility.MaterialID == batchPos.BookingMaterial.ProductionMaterialID)
                                 || (!batchPos.BookingMaterial.ProductionMaterialID.HasValue && plannedSilo.Facility.MaterialID == batchPos.BookingMaterial.MaterialID))
                             && (batchPos.IsFinalMixure
                                 || (   !batchPos.IsFinalMixure
                                     &&  (    !plannedSilo.Facility.PartslistID.HasValue 
                                           || batchPos.ProdOrderPartslist.PartslistID == plannedSilo.Facility.PartslistID)
                                    )
                                ) 
                           )
                       )
                    {
                        // Prüfe ob rechnerisch die Charge reinpassen würde
                        if (plannedSilo.Facility.CurrentFacilityStock != null
                            && (plannedSilo.Facility.MaxWeightCapacity > 1)
                            && (batchPos.TargetQuantityUOM + plannedSilo.Facility.CurrentFacilityStock.StockQuantity > (plannedSilo.Facility.MaxWeightCapacity /*+ batchPos.TargetQuantity*/)))
                        {
                            if (changeReservationStateIfFull)
                                Messages.LogDebug(this.GetACUrl(), "PWDischarging.CheckPlannedDestinationSilo(1)", String.Format("Silo {0} würde rechnerisch überfüllt werden", plannedSilo.Facility.FacilityNo));
                            plannedSilo.ReservationState = GlobalApp.ReservationState.Finished;
                            return false;
                        }
                        else
                            return true;
                    }
                }
                // Auf Lagerplätze ohne Belegung darf immer entleert werden
                else
                    return true;
            }
            // Wenn kein Bezug zu einem Lagerplatz hergstellt ist und MAterial nicht gebucht werdne soll, dann darf entleert werden
            else if (plannedSilo.Facility == null 
                && batchPos.BookingMaterial.MDFacilityManagementType != null 
                && batchPos.BookingMaterial.MDFacilityManagementType.FacilityManagementType == MDFacilityManagementType.FacilityManagementTypes.NoFacility)
            {
                return true;
            }
            return false;
        }

        public static bool CheckPlannedDestinationSilo(ACComponent invoker, FacilityReservation plannedSilo, ProdOrderPartslistPos batchPos, bool changeReservationStateIfFull, FacilityReservation ignoreFullSilo = null)
        {
            if (plannedSilo == null)
                return false;
            if (plannedSilo.Facility != null
                && plannedSilo.Facility.InwardEnabled)
            {
                if (plannedSilo.Facility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                {
                    // Entweder ist das Silo überhaupt nicht mit einem MAterial belegt
                    // Oder wenn es mit einem Material belegt ist überpüfe ob Material bzw. Produktionsmaterialnummern übereinstimmen
                    // Falls es sich um ein Zwischneprodukt handelt dann überprüfe auch die Rezeptnummer
                    if (!plannedSilo.Facility.MaterialID.HasValue
                        || (((batchPos.BookingMaterial.ProductionMaterialID.HasValue && plannedSilo.Facility.MaterialID == batchPos.BookingMaterial.ProductionMaterialID)
                                 || (!batchPos.BookingMaterial.ProductionMaterialID.HasValue && plannedSilo.Facility.MaterialID == batchPos.BookingMaterial.MaterialID))
                             && (batchPos.IsFinalMixure
                                 || (!batchPos.IsFinalMixure
                                     && (!plannedSilo.Facility.PartslistID.HasValue
                                           || batchPos.ProdOrderPartslist.PartslistID == plannedSilo.Facility.PartslistID)
                                    )
                                )
                           )
                       )
                    {
                        // Prüfe ob rechnerisch die Charge reinpassen würde
                        if (plannedSilo.Facility.CurrentFacilityStock != null
                            && (plannedSilo.Facility.MaxWeightCapacity > 1)
                            && (batchPos.TargetQuantityUOM + plannedSilo.Facility.CurrentFacilityStock.StockQuantity > (plannedSilo.Facility.MaxWeightCapacity /*+ batchPos.TargetQuantity*/)))
                        {
                            if (changeReservationStateIfFull)
                                invoker.Messages.LogDebug(invoker.GetACUrl(), "PWDischarging.CheckPlannedDestinationSilo(1)", String.Format("Silo {0} würde rechnerisch überfüllt werden", plannedSilo.Facility.FacilityNo));
                            plannedSilo.ReservationState = GlobalApp.ReservationState.Finished;
                            return false;
                        }
                        else
                            return true;
                    }
                }
                // Auf Lagerplätze ohne Belegung darf immer entleert werden
                else
                    return true;
            }
            // Wenn kein Bezug zu einem Lagerplatz hergstellt ist und MAterial nicht gebucht werdne soll, dann darf entleert werden
            else if (plannedSilo.Facility == null
                && batchPos.BookingMaterial.MDFacilityManagementType != null
                && batchPos.BookingMaterial.MDFacilityManagementType.FacilityManagementType == MDFacilityManagementType.FacilityManagementTypes.NoFacility)
            {
                return true;
            }
            return false;
        }
#endregion


#region Routing
        public static MsgWithDetails DetermineDischargingRoute(Database db, ACComponent acCompFrom, ACComponent acCompTo, 
            out Route route, int searchDepth, 
            Func<gip.core.datamodel.ACClass, gip.core.datamodel.ACClassProperty, Route, bool> deSelector, 
            string deSelectionRuleID, object[] deSelParams = null)
        {
            route = null;
            ACComponent routingService = null;
            PAClassAlarmingBase routeableComp = acCompFrom as PAClassAlarmingBase;
            if (routeableComp != null)
                routingService = routeableComp.RoutingService;

            Guid acClassIDCompTo = acCompTo.ComponentClass.ACClassID;
            RoutingResult rResult = ACRoutingService.SelectRoutes(routingService, db, routingService != null && routingService.IsProxy,
                                 acCompFrom, acCompTo, RouteDirections.Forwards, deSelectionRuleID, deSelParams != null ? deSelParams : new object[] { },
                                 (c, p, r) => c.ACClassID == acClassIDCompTo,
                                 deSelector,
                                 0, true, true, false, false, searchDepth);
            if (rResult.Routes == null || !rResult.Routes.Any())
                return new MsgWithDetails { Source = acCompFrom.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "DetermineDischargingRoute(1)", Message = "No route found" };

            route = rResult.Routes.FirstOrDefault();
            return null;
        }


        protected void DetermineDischargingRoute(Database db, ACComponent acCompFrom, ACComponent acCompTo,
            int searchDepth,
            Func<gip.core.datamodel.ACClass, gip.core.datamodel.ACClassProperty, Route, bool> deSelector, 
            string deSelectionRuleID, object[] deSelParams = null, Route predefinedRoute = null)
        {
            Route route = null;
            if (predefinedRoute != null)
            {
                if (!predefinedRoute.AreACUrlInfosSet)
                    predefinedRoute.AttachTo(db);
                var item = predefinedRoute.GetRouteTarget();
                if (item != null && item.TargetACComponent == acCompTo)
                    route = predefinedRoute;
            }
            if (route == null)
                PWDischarging.DetermineDischargingRoute(db, acCompFrom, acCompTo, out route, searchDepth, deSelector, deSelectionRuleID, deSelParams);
            if (route != null)
                route.Detach(true);
            CurrentDischargingRoute = route;
        }

        public virtual RouteItem CurrentDischargingDest(Database db, bool leaveAttached = false)
        {
            if (CurrentDischargingRoute == null)
                return null;
            RouteItem item = CurrentDischargingRoute.GetRouteTarget();
            //RouteItem item = CurrentDischargingRoute.LastOrDefault();
            if (item == null)
                return null;
            if (!item.AreACUrlInfosSet && db != null && db != core.datamodel.Database.GlobalDatabase)
            {
                item.AttachTo(db);
                item.Detach(true);
            }
            RouteItem clone = item.Clone() as RouteItem;
            if (db != null && leaveAttached)
                clone.AttachTo(db);
            return clone;
        }

        private ACRef<PAMSilo> _CurrentCachedDestinationSilo = null;
        public PAMSilo CurrentCachedDestinationSilo(Database db = null, bool leaveAttached = false)
        {
            if (CurrentDischargingRoute == null)
            {
                _CurrentCachedDestinationSilo = null;
                return null;
            }
            RouteItem lastItem = CurrentDischargingRoute.GetRouteTarget();
            //var lastItem = CurrentDischargingRoute.LastOrDefault();
            if (lastItem == null)
            {
                _CurrentCachedDestinationSilo = null;
                return null;
            }
            if (_CurrentCachedDestinationSilo != null && _CurrentCachedDestinationSilo.ValueT != null)
            {
                if (_CurrentCachedDestinationSilo.ValueT.ComponentClass.ACClassID == lastItem.TargetGuid)
                    return _CurrentCachedDestinationSilo.ValueT;
                _CurrentCachedDestinationSilo.Detach();
                _CurrentCachedDestinationSilo = null;
            }

            RouteItem clone = lastItem.Clone() as RouteItem;
            IACComponent component = clone.TargetACComponent;
            if (component == null && db != null && db != core.datamodel.Database.GlobalDatabase)
            {
                clone.AttachTo(db);
                component = clone.TargetACComponent;
                if (!leaveAttached)
                    clone.Detach(true);
            }

            PAMSilo targetSilo = component as PAMSilo;
            if (targetSilo == null)
                return null;
            _CurrentCachedDestinationSilo = new ACRef<PAMSilo>(targetSilo, this);
            return _CurrentCachedDestinationSilo.ValueT;
        }


        public PAProcessModule TargetPAModule(Database db)
        {
            RouteItem item = CurrentDischargingDest(db, false);
            if (item == null)
                return null;
            PAProcessModule target = item.TargetACComponent as PAProcessModule;
            if (target != null)
                return target;
            if (!item.IsAttached)
                item.AttachTo(db == null ? this.Root.Database : db);
            item.Detach();
            return item.TargetACComponent as PAProcessModule;
        }

#endregion


#region Alarmhandling
        public override void AcknowledgeAlarms()
        {
            NoTargetWait = null;
            CheckIfAutomaticTargetChangePossible = null;
            base.AcknowledgeAlarms();
        }
        #endregion


        #region Misc
        private gip.core.datamodel.ACProgramLog _NewAddedProgramLog = null;
        protected override void OnNewProgramLogAddedToQueue(ACMethod acMethod, gip.core.datamodel.ACProgramLog currentProgramLog)
        {
            if (_NewAddedProgramLog == null
                && CurrentDisEntityID != null
                && CurrentDisEntityID.ValueT != Guid.Empty 
                && IsTransport)
            {
                _NewAddedProgramLog = currentProgramLog;
                ACClassTaskQueue.TaskQueue.ObjectContext.ACChangesExecuted += ACClassTaskQueue_ChangesSaved;
            }
        }

        void ACClassTaskQueue_ChangesSaved(object sender, ACChangesEventArgs e)
        {
            ACClassTaskQueue.TaskQueue.ObjectContext.ACChangesExecuted -= ACClassTaskQueue_ChangesSaved;
            if (_NewAddedProgramLog != null)
            {
                if (CurrentDisEntityID.ValueT != Guid.Empty)
                {
                    Guid currentDisEntityID = CurrentDisEntityID.ValueT;
                    gip.core.datamodel.ACProgramLog newAddedProgramLog = _NewAddedProgramLog;
                    short propertyToSet = 0;
                    if (IsIntake)
                    {
                        if (ParentPWMethod<PWMethodIntake>().CurrentPicking != null)
                            propertyToSet = 1;
                        else if (ParentPWMethod<PWMethodIntake>().CurrentDeliveryNotePos != null)
                            propertyToSet = 2;
                    }
                    else if (IsRelocation)
                    {
                        if (ParentPWMethod<PWMethodRelocation>().CurrentPicking != null)
                            propertyToSet = 1;
                        else if (ParentPWMethod<PWMethodRelocation>().CurrentFacilityBooking != null)
                            propertyToSet = 3;
                    }
                    else if (IsLoading)
                    {
                        if (ParentPWMethod<PWMethodLoading>().CurrentPicking != null)
                            propertyToSet = 1;
                        else if (ParentPWMethod<PWMethodLoading>().CurrentDeliveryNotePos != null)
                            propertyToSet = 2;
                    }

                    this.ApplicationManager.ApplicationQueue.Add(() =>
                    //System.Threading.ThreadPool.QueueUserWorkItem((object state) =>
                    {
                        using (DatabaseApp dbApp = new DatabaseApp())
                        {
                            OrderLog orderLog = OrderLog.NewACObject(dbApp, newAddedProgramLog);
                            PickingPos currentDisEntity = dbApp.PickingPos.FirstOrDefault(c=>c.PickingPosID == currentDisEntityID);
                            if (propertyToSet == 1)
                            {
                                currentDisEntity.ACClassTaskID = this.ContentTask.ACClassTaskID;
                                orderLog.PickingPosID = currentDisEntityID;
                            }
                            else if (propertyToSet == 2)
                                orderLog.DeliveryNotePosID = currentDisEntityID;
                            else if (propertyToSet == 3)
                                orderLog.FacilityBookingID = currentDisEntityID;
                            dbApp.OrderLog.AddObject(orderLog);
                            dbApp.ACSaveChanges();
                        }
                        _NewAddedProgramLog = null;
                    });
                }
                else
                    _NewAddedProgramLog = null;
            }
            else
                _NewAddedProgramLog = null;
        }

        public override PAOrderInfo GetPAOrderInfo()
        {
            if (CurrentACState == ACStateEnum.SMIdle || CurrentACState == ACStateEnum.SMBreakPoint)
                return null;
            PAOrderInfo info = null;
            if (CurrentDisEntityID != null && CurrentDisEntityID.ValueT != Guid.Empty)
            {
                info = new PAOrderInfo();
                if (IsIntake)
                {
                    if (ParentPWMethod<PWMethodIntake>().CurrentPicking != null)
                        info.Add(PickingPos.ClassName, CurrentDisEntityID.ValueT);
                    else if (ParentPWMethod<PWMethodIntake>().CurrentDeliveryNotePos != null)
                        info.Add(DeliveryNotePos.ClassName, CurrentDisEntityID.ValueT);
                }
                else if (IsRelocation)
                {
                    if (ParentPWMethod<PWMethodRelocation>().CurrentPicking != null)
                        info.Add(PickingPos.ClassName, CurrentDisEntityID.ValueT);
                    else if (ParentPWMethod<PWMethodRelocation>().CurrentFacilityBooking != null)
                        info.Add(FacilityBooking.ClassName, CurrentDisEntityID.ValueT);
                }
                else if (IsLoading)
                {
                    if (ParentPWMethod<PWMethodLoading>().CurrentPicking != null)
                        info.Add(PickingPos.ClassName, CurrentDisEntityID.ValueT);
                    else if (ParentPWMethod<PWMethodLoading>().CurrentDeliveryNotePos != null)
                        info.Add(DeliveryNotePos.ClassName, CurrentDisEntityID.ValueT);
                }
            }
            else if (ParentPWGroup != null)
            {
                return ParentPWGroup.GetPAOrderInfo();
            }
            return null;
        }
        #endregion


        #region User-Interaction-Methods

        //#region Cancel Component
        //[ACMethodInteraction("", "en{'Cancel Discharging'}de{'Entleerung nicht mehr durchführen'}", 800, true)]
        //public virtual void CancelDischarging()
        //{
        //    if (!IsEnabledCancelDischarging())
        //        return;
        //    AcknowledgeAlarms();
        //    NoTargetFoundForDischarging.ValueT = 2;
        //}

        //public virtual bool IsEnabledCancelDischarging()
        //{
        //    return NoTargetFoundForDischarging.ValueT == 1;
        //}

        //public static bool AskCancelDischarging(IACComponent acComponent)
        //{
        //    if (acComponent == null)
        //        return false;
        //    return acComponent.Messages.Question(acComponent, "Question50021", Global.MsgResult.Yes) == Global.MsgResult.Yes;
        //}

        //#endregion

        #region Reset
        public override void Reset()
        {
            if (CurrentDischargingRoute != null)
            {
                _CurrentDischargingRoute = null;
            }

            ClearMyConfiguration();
            NoTargetWait = null;
            IsWaitingOnTarget = false;
            CheckIfAutomaticTargetChangePossible = null;
            UnSubscribeToProjectWorkCycle();

            base.Reset();
        }

        public override bool IsEnabledReset()
        {
            if (!(this._InCallback && CurrentACState == ACStateEnum.SMCompleted) 
                && this.TaskSubscriptionPoint.ConnectionList.Any())
            {
                foreach (var entry in this.TaskSubscriptionPoint.ConnectionList)
                {
                    var ACComponent = entry.ValueT;
                    if (!ACComponent.IsProxy)
                    {
                        foreach (var child in ACComponent.ACComponentChilds)
                        {
                            PAProcessFunction function = child as PAProcessFunction;
                            if (function != null && function.CurrentACState != ACStateEnum.SMIdle)
                                return false;
                        }
                    }
                }
            }
            return base.IsEnabledReset();
        }

        #endregion

        #region Extra-Target
        [ACMethodInteraction("", "en{'Enter extra destination'}de{'Sonderziel eingeben'}", 400, true)]
        public virtual void EnterExtraDisTargetDest()
        {
        }

        public virtual bool IsEnabledEnterExtraDisTargetDest()
        {
            PWGroupVB pwGroupVB = ParentPWGroup as PWGroupVB;
            if (pwGroupVB == null || RootPW == null)
                return false;
            string acUrlExtraDisDest;
            return    (   ((ACSubStateEnum)pwGroupVB.CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode)
                        || ((ACSubStateEnum)pwGroupVB.CurrentACSubState).HasFlag(ACSubStateEnum.SMDisThenNextComp)
                        || ((ACSubStateEnum)pwGroupVB.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode)
                        || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode)
                        || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMDisThenNextComp)
                        || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode))
                    && IsAlarmActive(ProcessAlarm) != null
                    && (!pwGroupVB.IsExtraDisTargetEntered(out acUrlExtraDisDest) || pwGroupVB.ExtraDisTargetComp == null);
        }

        public static bool AskUserEnterExtraDisTargetDest(IACComponent acComponent)
        {
            if (acComponent == null)
                return false;
            PWMethodVBBase.EnterExtraDisTargetDest(acComponent.ParentACComponent);
            return true;
        }
        #endregion


        #endregion

        #region Planning and Testing
        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList)
        {
            base.DumpPropertyList(doc, xmlACPropertyList);

            XmlElement xmlChild = xmlACPropertyList["LastTargets"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("LastTargets");
                if (xmlChild != null)
                {
                    var lastTargets = LastTargets;
                    if (lastTargets == null || !lastTargets.Any())
                        xmlChild.InnerText = "null";
                    else
                    {
                        int i = 0;
                        StringBuilder sb = new StringBuilder();
                        foreach (var element in lastTargets)
                        {
                            sb.AppendFormat("{0}:{1}|", i, element.ToString());
                        }
                        xmlChild.InnerText = sb.ToString();
                    }
                }
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["CacheModuleDestinations"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("CacheModuleDestinations");
                if (xmlChild != null)
                {
                    var cachedModDest = CacheModuleDestinations;
                    if (cachedModDest == null || !cachedModDest.Any())
                        xmlChild.InnerText = "null";
                    else
                    {
                        int i = 0;
                        StringBuilder sb = new StringBuilder();
                        foreach (var element in cachedModDest)
                        {
                            sb.AppendFormat("{0}:{1}|", i, element.ToString());
                        }
                        xmlChild.InnerText = sb.ToString();
                    }
                }
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["NoTargetWait"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("NoTargetWait");
                if (xmlChild != null)
                    xmlChild.InnerText = NoTargetWait.HasValue ? NoTargetWait.Value.ToString() : "null";
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["NoTargetLongWait"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("NoTargetLongWait");
                if (xmlChild != null)
                    xmlChild.InnerText = NoTargetLongWait.HasValue ? NoTargetLongWait.Value.ToString() : "null";
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["CheckIfAutomaticTargetChangePossible"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("CheckIfAutomaticTargetChangePossible");
                if (xmlChild != null)
                    xmlChild.InnerText = CheckIfAutomaticTargetChangePossible.HasValue ? CheckIfAutomaticTargetChangePossible.Value.ToString() : "null";
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["IsWaitingOnTarget"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("IsWaitingOnTarget");
                if (xmlChild != null)
                    xmlChild.InnerText = IsWaitingOnTarget.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["SkipIfNoComp"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("SkipIfNoComp");
                if (xmlChild != null)
                    xmlChild.InnerText = SkipIfNoComp.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["SkipPredCount"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("SkipPredCount");
                if (xmlChild != null)
                    xmlChild.InnerText = SkipPredCount.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["LimitToMaxCapOfDest"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("LimitToMaxCapOfDest");
                if (xmlChild != null)
                    xmlChild.InnerText = LimitToMaxCapOfDest.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["PrePostQOnDest"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("PrePostQOnDest");
                if (xmlChild != null)
                    xmlChild.InnerText = PrePostQOnDest.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["NoPostingOnRelocation"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("NoPostingOnRelocation");
                if (xmlChild != null)
                    xmlChild.InnerText = NoPostingOnRelocation.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }
        }
        #endregion

        #endregion
    }
}
