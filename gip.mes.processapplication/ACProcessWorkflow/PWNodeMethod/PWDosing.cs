using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.core.processapplication;
using System.Xml;

namespace gip.mes.processapplication
{
    /// <summary>
    /// Class that is responsible for processing input-materials that are associated with an intermediate product. 
    /// The intermediate prduct, in turn, is linked through the material workflow to one or more workflow nodes that are from this PWDosing class. 
    /// PWDosing is used for fully automatic production. 
    /// It calls the PAFDosing process function asynchronously to delegate the real-time critical tasks to a PLC controller.
    /// Consumed quantities are posted by warehouse management (ACFacilityManager).
    /// It can work with different data contexts (production and picking orders).
    /// </summary>
    /// <seealso cref="gip.core.autocomponent.PWNodeProcessMethod" />
    /// <seealso cref="gip.core.autocomponent.IPWNodeReceiveMaterialRouteable" />
    /// <seealso cref="gip.core.autocomponent.IACMyConfigCache" />
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Workflowclass Dosing'}de{'Workflowklasse Dosieren'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public partial class PWDosing : PWNodeProcessMethod, IPWNodeReceiveMaterialRouteable
    {
        public const string PWClassName = "PWDosing";

        #region c´tors
        static PWDosing()
        {
            ACMethod method;
            method = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("SkipComponents", typeof(bool), false, Global.ParamOption.Required));
            paramTranslation.Add("SkipComponents", "en{'Skip not dosable components'}de{'Überspringe nicht dosierbare Komponenten'}");
            method.ParameterValueList.Add(new ACValue("ComponentsSeqFrom", typeof(Int32), 0, Global.ParamOption.Optional));
            paramTranslation.Add("ComponentsSeqFrom", "en{'Components from Seq.-No.'}de{'Komponenten VON Seq.-Nr.'}");
            method.ParameterValueList.Add(new ACValue("ComponentsSeqTo", typeof(Int32), 0, Global.ParamOption.Optional));
            paramTranslation.Add("ComponentsSeqTo", "en{'Components to Seq.-No.'}de{'Komponenten BIS Seq.-Nr.'}");
            method.ParameterValueList.Add(new ACValue("ScaleOtherComp", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("ScaleOtherComp", "en{'Scale other components after Dosing'}de{'Restliche Komponenten anpassen'}");
            method.ParameterValueList.Add(new ACValue("ManuallyChangeSource", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("ManuallyChangeSource", "en{'Manually change source'}de{'Manueller Quellenwechsel'}");
            method.ParameterValueList.Add(new ACValue("MinDosQuantity", typeof(double), 0.0, Global.ParamOption.Optional));
            paramTranslation.Add("MinDosQuantity", "en{'Minimum dosing quantity'}de{'Minimale Dosiermenge'}");
            method.ParameterValueList.Add(new ACValue("OldestSilo", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("OldestSilo", "en{'Dosing from oldest Silo only'}de{'Nur aus ältestem Silo dosieren'}");
            method.ParameterValueList.Add(new ACValue("AutoChangeScale", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("AutoChangeScale", "en{'Automatically change scale'}de{'Automatischer Waagenwechsel'}");
            method.ParameterValueList.Add(new ACValue("CheckScaleEmpty", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("CheckScaleEmpty", "en{'Check if scale empty'}de{'Prüfung Waage leer'}");
            method.ParameterValueList.Add(new ACValue("BookTargetQIfZero", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("BookTargetQIfZero", "en{'Post target quantity when actual = 0'}de{'Sollmenge buchen wenn Istgewicht = 0'}");
            method.ParameterValueList.Add(new ACValue("DoseFromFillingSilo", typeof(bool?), null, Global.ParamOption.Optional));
            paramTranslation.Add("DoseFromFillingSilo", "en{'Dose from silo that is filling'}de{'Dosiere aus Silo das befüllt wird'}");
            method.ParameterValueList.Add(new ACValue("FacilityNoSort", typeof(string), null, Global.ParamOption.Optional));
            paramTranslation.Add("FacilityNoSort", "en{'Priorization order container number'}de{'Priorisierungsreihenfolge Silonummer'}");
            var wrapper = new ACMethodWrapper(method, "en{'Configuration'}de{'Konfiguration'}", typeof(PWDosing), paramTranslation, null);
            ACMethod.RegisterVirtualMethod(typeof(PWDosing), ACStateConst.SMStarting, wrapper);
            RegisterExecuteHandler(typeof(PWDosing), HandleExecuteACMethod_PWDosing);
        }


        public PWDosing(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            return base.ACPostInit();
        }

        public override bool ACPreDeInit(bool deleteACClassTask = false)
        {
            bool result = base.ACPreDeInit(deleteACClassTask);
            if (deleteACClassTask)
                this.TaskSubscriptionPoint.UnSubscribe();
            return result;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            NoSourceFoundForDosing.ValueT = 0;

            using (ACMonitor.Lock(_20015_LockValue))
            {
                _CurrentDosingRoute = null;
                _NoSourceWait = null;
                _NextCheckIfPWDosingsFinished = null;
                _CurrentParallelPWDosings = null;
                _LastQueriedDosingSilo = null;
                _DosingFuncResultState = PADosingAbortReason.NotSet;
                _CachedEmptySiloHandlingOption = null;
                _MaxWeightAlarmSet = false;
                _EmptyScaleAlarm = EmptyScaleAlarmState.None;
                _RepeatDosingForPicking = false;
            }

            return base.ACDeInit(deleteACClassTask);
        }

        public override void Recycle(IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {
            using (ACMonitor.Lock(_20015_LockValue))
            {
                _CurrentDosingRoute = null;
                _NoSourceWait = null;
                _NextCheckIfPWDosingsFinished = null;
                _CurrentParallelPWDosings = null;
                _LastQueriedDosingSilo = null;
                _DosingFuncResultState = PADosingAbortReason.NotSet;
                _CachedEmptySiloHandlingOption = null;
                _MaxWeightAlarmSet = false;
                _EmptyScaleAlarm = EmptyScaleAlarmState.None;
                _RepeatDosingForPicking = false;
            }
            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }
        #endregion

        #region Properties

        #region Common
        [ACPropertyBindingSource(9999, "", "", "", false, true)]
        public IACContainerTNet<Guid> CurrentDosingPos { get; set; }

        /// <summary>
        /// 0 = Not set; 
        /// 1 = No silo found, wait for User reponse; 
        /// 2 = User has cancelled dosing; 
        /// </summary>
        [ACPropertyBindingSource(9999, "", "en{'Source/Silo/Cell/Tank not found'}de{'Keine Quelle/Silo/Zelle/Tank gefunden'}", "", false, false)]
        public IACContainerTNet<short> NoSourceFoundForDosing { get; set; }

        private DateTime? _NoSourceWait;
        protected DateTime? NoSourceWait
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _NoSourceWait;
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _NoSourceWait = value;
                }
            }
        }

        private EmptySiloHandlingOptions? _CachedEmptySiloHandlingOption = null;
        protected  EmptySiloHandlingOptions? CachedEmptySiloHandlingOption
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _CachedEmptySiloHandlingOption;
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _CachedEmptySiloHandlingOption = value;
                }
            }
        }


        private bool _MaxWeightAlarmSet;
        public bool MaxWeightAlarmSet
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _MaxWeightAlarmSet;
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _MaxWeightAlarmSet = value;
                }
            }
        }


        protected enum EmptyScaleAlarmState
        {
            None = 0,
            Alarm = 1,
            Acknowledged = 2
        }

        private EmptyScaleAlarmState _EmptyScaleAlarm;
        protected EmptyScaleAlarmState EmptyScaleAlarm
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _EmptyScaleAlarm;
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _EmptyScaleAlarm = value;
                }
            }
        }


        private Route _CurrentDosingRoute;
        [ACPropertyInfo(true, 9999, "", "", "", false)]
        public Route CurrentDosingRoute
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _CurrentDosingRoute;
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _CurrentDosingRoute = value;
                }
                OnPropertyChanged("CurrentDosingRoute");
            }
        }

        public bool HasAnyMaterialToProcess
        {
            get
            {
                if (IsProduction)
                    return HasAnyMaterialToProcessProd;

                return true;
            }
        }

        public PAFDosing CurrentExcutingFunction
        {
            get
            {
                ACPointAsyncRMISubscrWrap<ACComponent> taskEntry = null;

                using (ACMonitor.Lock(TaskSubscriptionPoint.LockLocalStorage_20033))
                {
                    taskEntry = this.TaskSubscriptionPoint.ConnectionList.FirstOrDefault();
                }
                // Falls Dosierung zur Zeit aktiv ist, dann gibt es auch einen Eintrag in der TaskSubscriptionListe
                if (taskEntry != null)
                {
                    return ParentPWGroup.GetExecutingFunction<PAFDosing>(taskEntry.RequestID);
                }
                return null;
            }
        }

        private PAMSilo _LastQueriedDosingSilo;
        protected virtual PAMSilo LastQueriedDosingSilo
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _LastQueriedDosingSilo;
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _LastQueriedDosingSilo = value;
                }
                OnPropertyChanged("LastQueriedDosingSilo");
            }
        }

        private PADosingAbortReason _DosingFuncResultState = PADosingAbortReason.NotSet;
        protected PADosingAbortReason DosingFuncResultState
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _DosingFuncResultState;
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _DosingFuncResultState = value;
                }
            }
        }
        /// <summary>
        /// Liste die angibt ob es noch weitere parallele Schritte gibt, welche die noch die letzten Dosierungen für das gleiche Zwischenmaterial durchführen
        /// </summary>
        private List<PWDosing> _CurrentParallelPWDosings = null;
        protected List<PWDosing> CurrentParallelPWDosings
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_CurrentParallelPWDosings == null)
                        return null;
                    return _CurrentParallelPWDosings.ToList();
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _CurrentParallelPWDosings = value;
                }
            }
        }


        private DateTime? _NextCheckIfPWDosingsFinished = null;
        protected DateTime? NextCheckIfPWDosingsFinished
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _NextCheckIfPWDosingsFinished;
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _NextCheckIfPWDosingsFinished = value;
                }
            }
        }

        public bool HasRunSomeDosings
        {
            get
            {
                var previousLogs = PreviousProgramLogs;
                return previousLogs != null && previousLogs.Any();
            }
        }

        public int CountRunDosings
        {
            get
            {
                var previousLogs = PreviousProgramLogs;
                return previousLogs != null ? previousLogs.Count() : 0;
            }
        }

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

        #endregion

        #region PWMethodBase
        public PWMethodVBBase ParentPWMethodVBBase
        {
            get
            {
                return ParentRootWFNode as PWMethodVBBase;
            }
        }

        protected FacilityManager ACFacilityManager
        {
            get
            {
                if (ParentPWMethodVBBase == null)
                    return null;
                return ParentPWMethodVBBase.ACFacilityManager as FacilityManager;
            }
        }

        #endregion

        #region Configuration

        public override void ClearMyConfiguration()
        {
            base.ClearMyConfiguration();
            using (ACMonitor.Lock(_20015_LockValue))
            {
                _ExcludedSilos = null;
            }
        }

        public bool ComponentsSkippable
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("SkipComponents");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        public bool ScaleOtherComp
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("ScaleOtherComp");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        public bool ManuallyChangeSource
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("ManuallyChangeSource");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        public bool CheckScaleEmpty
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("CheckScaleEmpty");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        public bool BookTargetQIfZero
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("BookTargetQIfZero");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        public bool? DoseFromFillingSilo
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("DoseFromFillingSilo");
                    if (acValue != null)
                        return acValue.ValueT<bool?>();
                }
                return null;
            }
        }

        public bool OldestSilo
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("OldestSilo");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        public string FacilityNoSort
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("FacilityNoSort");
                    if (acValue != null)
                    {
                        return acValue.ParamAsString;
                    }
                }
                return null;
            }
        }
        
        public bool AutoChangeScale
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("AutoChangeScale");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// A endless dosing can be achieved by setting the MinDosQuantity to a large negative value.
        /// If the MinDosQuantity is positive and the remaining Quantity for dosing is smaller than this value, then the dosing will be skipped
        /// </summary>
        public double MinDosQuantity
        {
            get
            {
                double? minDosQuantity = null;
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("MinDosQuantity");
                    if (acValue != null)
                    {
                        minDosQuantity = acValue.ParamAsDouble;
                    }
                }
                if (!minDosQuantity.HasValue && this.ParentPWGroup != null)
                {
                    PAMHopperscale pamScale = this.ParentPWGroup.AccessedProcessModule as PAMHopperscale;
                    if (pamScale != null)
                    {
                        if (pamScale.MinDosingWeight.HasValue)
                            minDosQuantity = pamScale.MinDosingWeight.Value;
                    }
                }

                if (!minDosQuantity.HasValue)
                    minDosQuantity = 0.000001;
                return minDosQuantity.Value;
            }
        }


        public Int32 ComponentsSeqFrom
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("ComponentsSeqFrom");
                    if (acValue != null)
                    {
                        return acValue.ParamAsInt32;
                    }
                }
                return 0;
            }
        }

        public Int32 ComponentsSeqTo
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("ComponentsSeqTo");
                    if (acValue != null)
                    {
                        return acValue.ParamAsInt32;
                    }
                }
                return 0;
            }
        }

        public RuleValueList ExcludedSilosRule
        {
            get
            {
                RuleValueList ruleValueList = null;
                ConfigManagerIPlus serviceInstance = ConfigManagerIPlus.GetServiceInstance(this);
                ruleValueList = serviceInstance.GetRuleValueList(MandatoryConfigStores, PreValueACUrl, ConfigACUrl + @"\Rules\" + ACClassWFRuleTypes.Excluded_process_modules.ToString());
                return ruleValueList;
            }
        }

        IEnumerable<gip.core.datamodel.ACClass> _ExcludedSilos;
        public IEnumerable<gip.core.datamodel.ACClass> ExcludedSilos
        {
            get
            {
                if (_ExcludedSilos != null)
                    return _ExcludedSilos;
                RuleValueList ruleValueList = ExcludedSilosRule;
                if (ruleValueList == null || ruleValueList.Items == null || !ruleValueList.Items.Any())
                {
                    _ExcludedSilos = new gip.core.datamodel.ACClass[] { };
                    return _ExcludedSilos;
                }

                using (ACMonitor.Lock(gip.core.datamodel.Database.GlobalDatabase.QueryLock_1X000))
                {
                    _ExcludedSilos = ruleValueList.GetSelectedClasses(ACClassWFRuleTypes.Excluded_process_modules, gip.core.datamodel.Database.GlobalDatabase);
                }
                if (_ExcludedSilos == null)
                    _ExcludedSilos = new gip.core.datamodel.ACClass[] { };
                return _ExcludedSilos;
            }
        }

        public PAFDosing CurrentExecutingFunction
        {
            get
            {
                IEnumerable<ACPointAsyncRMISubscrWrap<ACComponent>> taskEntries = null;

                using (ACMonitor.Lock(TaskSubscriptionPoint.LockLocalStorage_20033))
                {
                    taskEntries = this.TaskSubscriptionPoint.ConnectionList.ToArray();
                }
                // Falls Dosierung zur Zeit aktiv ist, dann gibt es auch einen Eintrag in der TaskSubscriptionListe
                if (taskEntries != null && taskEntries.Any())
                {
                    foreach (var entry in taskEntries)
                    {
                        PAFDosing dosing = ParentPWGroup.GetExecutingFunction<PAFDosing>(entry.RequestID);
                        if (dosing != null)
                            return dosing;
                    }
                }
                return null;
            }
        }
#endregion

#endregion


#region Methods

#region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "CancelCurrentComponent":
                    CancelCurrentComponent();
                    return true;
                case Const.IsEnabledPrefix + "CancelCurrentComponent":
                    result = IsEnabledCancelCurrentComponent();
                    return true;
                case "AckNotEmptyScale":
                    AckNotEmptyScale();
                    return true;
                case Const.IsEnabledPrefix + "AckNotEmptyScale":
                    result = IsEnabledAckNotEmptyScale();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PWDosing(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWNodeProcessMethod(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
#endregion


#region ACState
        public override void SMIdle()
        {
            base.SMIdle();
            CurrentDosingPos.ValueT = Guid.Empty;
            NextCheckIfPWDosingsFinished = null;
            CurrentParallelPWDosings = null;
            MaxWeightAlarmSet = false;
            EmptyScaleAlarm = EmptyScaleAlarmState.None;
            RepeatDosingForPicking = false;
        }

        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            var pwGroup = ParentPWGroup;
            if (pwGroup == null) // Is null when Service-Application is shutting down
            {
                if (this.InitState == ACInitState.Initialized)
                    Messages.LogError(this.GetACUrl(), "SMStarting()", "ParentPWGroup is null");
                return;
            }

            if (   ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMBatchCancelled)
                || ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode)
                || ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMDisThenNextComp)
                || ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMInterDischarging)
                || ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode)
                || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMBatchCancelled)
                || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode)
                || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode))
            {
                UnSubscribeToProjectWorkCycle();
                // Falls durch tiefere Callstacks der Status schon weitergeschaltet worden ist, dann schalte Status nicht weiter
                if (CurrentACState == ACStateEnum.SMStarting)
                    CurrentACState = ACStateEnum.SMCompleted;
                return;
            }

            if (Root == null || !Root.Initialized)
            {
                SubscribeToProjectWorkCycle();
                return;
            }

            core.datamodel.ACClassMethod refPAACClassMethod = null;
            if (this.ContentACClassWF != null)
            {

                using (ACMonitor.Lock(this.ContextLockForACClassWF))
                {
                    refPAACClassMethod = this.ContentACClassWF.RefPAACClassMethod;
                }
            }

            if (pwGroup != null
                && this.ContentACClassWF != null
                && refPAACClassMethod != null)
            {
                PAProcessModule module = null;
                if (ParentPWGroup.NeedsAProcessModule && (ACOperationMode == ACOperationModes.Live || ACOperationMode == ACOperationModes.Simulation))
                    module = ParentPWGroup.AccessedProcessModule;
                // Testmode
                else
                    module = ParentPWGroup.ProcessModuleForTestmode;

                if (module == null)
                {
                    //Error50372: The workflow group has not occupied a process module.
                    // Die Workflowgruppe hat kein Prozessmodul belegt.
                    Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "SMStarting(10)", 1000, "Error50372");
                    ActivateProcessAlarmWithLog(msg, false);
                    SubscribeToProjectWorkCycle();
                    return;
                }

                if (NoSourceFoundForDosing.ValueT == 1)
                {
                    if (!IsSubscribedToWorkCycle)
                        SubscribeToProjectWorkCycle();
                    if (!NoSourceWait.HasValue)
                    {
                        NoSourceWait = DateTime.Now + TimeSpan.FromSeconds(10);
                        return;
                    }
                    else if (DateTime.Now < NoSourceWait.Value)
                        return;
                }

                if (CheckScaleEmpty && EmptyScaleAlarm != EmptyScaleAlarmState.Acknowledged)
                {
                    PAMHopperscale hopScale = module as PAMHopperscale;
                    if (hopScale != null && !hopScale.IsScaleEmpty)
                    {
                        EmptyScaleAlarm = EmptyScaleAlarmState.Alarm;
                        //Error50294: Scale is not empty. Dosing coudn't be started!
                        Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "SMStarting", 1010, "Error50294");
                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), "SMStarting(2)", msg.Message);
                        OnNewAlarmOccurred(ProcessAlarm, msg, true);

                        SubscribeToProjectWorkCycle();
                        return;
                    }
                }


                NoSourceWait = null;
                CachedEmptySiloHandlingOption = null;
                StartNextCompResult result = StartNextCompResult.Done;
                if (IsProduction)
                    result = StartNextProdComponent(module);
                else if (IsTransport)
                    result = StartNextPickingPos(module);
                if (result == StartNextCompResult.CycleWait)
                {
                    SubscribeToProjectWorkCycle();
                    return;
                }
                else if (result == StartNextCompResult.NextCompStarted || result == StartNextCompResult.WaitForNextEvent)
                {
                    // Falls durch Wiederholschleife der Workfloknoten gestartet worden ist, dann setze den Substatus zurück, damit nur einmal dosiert wird.
                    RepeatDosingForPicking = false;

                    // Falls durch tiefere Callstacks der Status schon weitergeschaltet worden ist, dann schalte Status nicht weiter
                    if (CurrentACState == ACStateEnum.SMStarting)
                        CurrentACState = ACStateEnum.SMRunning;
                    return;
                }
                else
                {
                    // Falls durch Wiederholschleife der Workfloknoten gestartet worden ist, dann setze den Substatus zurück, damit nur einmal dosiert wird.
                    RepeatDosingForPicking = false;

                    UnSubscribeToProjectWorkCycle();
                    // Falls durch tiefere Callstacks der Status schon weitergeschaltet worden ist, dann schalte Status nicht weiter
                    if (CurrentACState == ACStateEnum.SMStarting && !_RepeatAfterCompleted)
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
            PAFDosing dosing = CurrentExecutingFunction;
            // Falls Dosierung zur Zeit aktiv ist, dann gibt es auch einen Eintrag in der TaskSubscriptionListe
            if (dosing != null)
            {
                SubscribeToProjectWorkCycle();
                if (!Root.Initialized)
                    return;
                if (dosing.CurrentACState != ACStateEnum.SMIdle)
                {
                    OnHandleStateCheckEmptySilo(dosing);
                }
            }
            // Sonst ist kein Dosierauftrag auf PAFDosing aktiv
            else
            {
                var pwGroup = ParentPWGroup;
                if (pwGroup == null) // Is null when Service-Application is shutting down
                {
                    if (this.InitState == ACInitState.Initialized)
                        Messages.LogError(this.GetACUrl(), "SMRunning()", "ParentPWGroup is null");
                    return;
                }

                if (((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMBatchCancelled)
                    || ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode)
                    || ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMDisThenNextComp)
                    || ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMInterDischarging)
                    || ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode)
                    || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMBatchCancelled)
                    || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode)
                    || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode))
                {
                    // Falls durch tiefere Callstacks der Status schon weitergeschaltet worden ist, dann schalte Status nicht weiter
                    if (CurrentACState == ACStateEnum.SMRunning)
                        CurrentACState = ACStateEnum.SMCompleted;
                    return;
                }

                if (NoSourceFoundForDosing.ValueT == 1)
                {
                    if (!NoSourceWait.HasValue)
                    {
                        NoSourceWait = DateTime.Now + TimeSpan.FromSeconds(10);
                        return;
                    }
                    else if (DateTime.Now < NoSourceWait.Value)
                        return;
                }

                PAProcessModule module = pwGroup.AccessedProcessModule;
                if (module != null)
                {
                    if (!Root.Initialized)
                    {
                        SubscribeToProjectWorkCycle();
                        return;
                    }

                    NoSourceWait = null;
                    CachedEmptySiloHandlingOption = null;
                    StartNextCompResult result = StartNextCompResult.Done;
                    if (IsProduction)
                        result = StartNextProdComponent(module);
                    else if (IsTransport)
                        result = StartNextPickingPos(module);
                    if (result == StartNextCompResult.CycleWait)
                    {
                        SubscribeToProjectWorkCycle();
                        return;
                    }
                    else if (result == StartNextCompResult.NextCompStarted || result == StartNextCompResult.WaitForNextEvent)
                    {
                        // Falls durch Wiederholschleife der Workfloknoten gestartet worden ist, dann setze den Substatus zurück, damit nur einmal dosiert wird.
                        RepeatDosingForPicking = false;
                        SubscribeToProjectWorkCycle();
                        //UnSubscribeToProjectWorkCycle();
                        //CurrentACState = PABaseState.SMRunning;
                        return;
                    }
                    else
                    {
                        // Falls durch Wiederholschleife der Workfloknoten gestartet worden ist, dann setze den Substatus zurück, damit nur einmal dosiert wird.
                        RepeatDosingForPicking = false;
                        UnSubscribeToProjectWorkCycle();
                        // Falls durch tiefere Callstacks der Status schon weitergeschaltet worden ist, dann schalte Status nicht weiter
                        if (CurrentACState == ACStateEnum.SMRunning)
                            CurrentACState = ACStateEnum.SMCompleted;
                        return;
                    }
                }
            }
        }

        protected gip.core.datamodel.ACProgramLog _NewAddedProgramLog = null;
        protected override void OnNewProgramLogAddedToQueue(ACMethod acMethod, gip.core.datamodel.ACProgramLog currentProgramLog)
        {
            if (_NewAddedProgramLog == null)
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
                if (CurrentDosingPos.ValueT != Guid.Empty)
                {
                    this.ApplicationManager.ApplicationQueue.Add(() =>
                    //ThreadPool.QueueUserWorkItem((object state) =>
                    {
                        using (DatabaseApp dbApp = new DatabaseApp())
                        {
                            OrderLog orderLog = OrderLog.NewACObject(dbApp, _NewAddedProgramLog);
                            if (IsProduction)
                                orderLog.ProdOrderPartslistPosRelationID = CurrentDosingPos.ValueT;
                            if (IsTransport)
                                orderLog.PickingPosID = CurrentDosingPos.ValueT;
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

        public virtual bool HasDosedComponents(out double sumQuantity)
        {
            using (var dbApp = new DatabaseApp())
            {
                return ManageDosingState(ManageDosingStatesMode.QueryDosedComponents, dbApp, out sumQuantity);
            }
        }

        public virtual bool HasOpenDosings(out double sumQuantity)
        {
            using (var dbApp = new DatabaseApp())
            {
                return ManageDosingState(ManageDosingStatesMode.QueryOpenDosings, dbApp, out sumQuantity);
            }
        }

        public virtual bool HasAnyDosings(out double sumQuantity)
        {
            using (var dbApp = new DatabaseApp())
            {
                return ManageDosingState(ManageDosingStatesMode.QueryHasAnyDosings, dbApp, out sumQuantity);
            }
        }

        public virtual bool ResetDosingsAfterInterDischarging(DatabaseApp dbApp)
        {
            double sumQuantity;
            return ManageDosingState(ManageDosingStatesMode.ResetDosings, dbApp, out sumQuantity);
        }

        public virtual bool SetDosingsCompletedAfterDischarging(DatabaseApp dbApp)
        {
            double sumQuantity;
            return ManageDosingState(ManageDosingStatesMode.SetDosingsCompleted, dbApp, out sumQuantity);
        }


        public class ScaleBoundaries
        {
            public ScaleBoundaries(IPAMContScale scale)
            {
                RemainingWeightCapacity = scale.RemainingWeightCapacity;
                MinDosingWeight = scale.MinDosingWeight;
                MaxWeightCapacity = scale.MaxWeightCapacity.ValueT;
                RemainingVolumeCapacity = scale.RemainingVolumeCapacity;
                MaxVolumeCapacity = scale.MaxVolumeCapacity.ValueT;
            }

            public double? RemainingWeightCapacity
            {
                get;
                set;
            }

            public double? MinDosingWeight
            {
                get;
                set;
            }

            public double MaxWeightCapacity
            {
                get;
                set;
            }

            public double? RemainingVolumeCapacity
            {
                get;
                set;
            }

            public double MaxVolumeCapacity
            {
                get;
                set;
            }
        }

        public virtual ScaleBoundaries OnGetScaleBoundariesForDosing(IPAMContScale scale)
        {
            return new ScaleBoundaries(scale);
        }

        protected enum ManageDosingStatesMode
        {
            QueryOpenDosings = 0,
            QueryHasAnyDosings = 1,
            SetDosingsCompleted = 2,
            ResetDosings = 3,
            QueryDosedComponents = 4
        }

        private bool ManageDosingState(ManageDosingStatesMode mode, DatabaseApp dbApp, out double sumQuantity)
        {
            sumQuantity = 0.0;
            if (IsProduction)
                return ManageDosingStateProd(mode, dbApp, out sumQuantity);
            else if (IsTransport)
                return ManageDosingStatePicking(mode, dbApp, out sumQuantity);
            return false;
        }

        /// <summary>
        /// 2.1 Zyklische Aufrufmethode bei Materialmangel in HandleState
        /// </summary>
        public virtual void OnHandleStateCheckEmptySilo(PAFDosing dosing)
        {
            //double actualQuantity = 0;
            //PAEScaleBase scale = dosing.CurrentScaleForWeighing;
            //if (scale != null)
            //{
            //    actualQuantity = scale.ActualWeight.ValueT;
            //    if (!IsSimulationOn)
            //    {
            //        PAEScaleTotalizing totalizingScale = scale as PAEScaleTotalizing;
            //        if (totalizingScale != null)
            //            actualQuantity = totalizingScale.TotalActualWeight.ValueT;
            //    }
            //}

            if (!ManuallyChangeSource
                && dosing.StateLackOfMaterial.ValueT != PANotifyState.Off
                && dosing.CurrentACState == ACStateEnum.SMRunning
                && dosing.DosingAbortReason.ValueT == PADosingAbortReason.NotSet)
            {
                PAMSilo silo = CurrentDosingSilo(null);
                if (silo == null)
                    return;

                DosingRestInfo restInfo = new DosingRestInfo(silo, dosing, 10);

                //if (silo.MatSensorEmtpy == null
                //    || (silo.MatSensorEmtpy != null && silo.MatSensorEmtpy.SensorState.ValueT != PANotifyState.Off))
                {
                    //silo.RefreshFacility();
                    //double zeroTolerance = 10;
                    //if (silo.Facility.ValueT != null && silo.Facility.ValueT.ValueT != null)
                    //    zeroTolerance = silo.Facility.ValueT.ValueT.Tolerance;
                    //if (zeroTolerance <= 0.1)
                    //    zeroTolerance = 10;

                    // Überprüfe Rechnerischen Restbestand des Silos
                    //double rest = silo.FillLevel.ValueT - actualQuantity;
                    //if (rest < zeroTolerance)
                    if (restInfo.InZeroTolerance)
                    {
                        // Falls Methode true zurückgibt
                        EmptySiloHandlingOptions handlingOptions = HandleAbortReasonOnEmptySilo(silo);
                        if (handlingOptions.HasFlag(EmptySiloHandlingOptions.OtherSilosAvailable))
                        {
                            dosing.SetAbortReasonEmpty();
                        }
                        else if (handlingOptions.HasFlag(EmptySiloHandlingOptions.NoSilosAvailable) && IsTransport)
                        {
                            dosing.EndDosEndOrder();
                        }
                        else if (handlingOptions.HasFlag(EmptySiloHandlingOptions.NoSilosAvailable))
                        {
                            // Warning50005: No Silo/Tank/Container found for component {0}
                            Msg msg = new Msg(this, eMsgLevel.Warning, PWClassName, "OnHandleStateCheckEmptySilo(100)", 100, "Warning50005",
                                            silo.MaterialName != null ? silo.MaterialName.ValueT : "");

                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        }
                    }
                    else
                    {
                        // Warning50030:  Lack of Material: Stock in Silo / Tank / Container {0} is to high for automatic switching to another Silo / Tank / Container
                        Msg msg = new Msg(this, eMsgLevel.Warning, PWClassName, "OnHandleStateCheckEmptySilo(101)", 101, "Warning50030",
                                        silo.ACIdentifier);

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    }
                }
            }
        }

        [Flags]
        public enum EmptySiloHandlingOptions
        {
            NotSet = 0x0,

            NoSilosAvailable = 0x01,

            /// <summary>
            /// Andere Silos sind verfügbar
            /// </summary>
            OtherSilosAvailable = 0x02,

            /// <summary>
            /// Falls OtherSilosAvailable gesetzt, Anzeige dass Silos auf diesem Prozessmodul verfügbar sind
            /// </summary>
            AvailabeOnThisModule = 0x04,

            /// <summary>
            /// Falls OtherSilosAvailable gesetzt, Anzeige dass Silos auf anderem Prozessmodul verfügbar sind und ein Waagenwchsel gemacht wird
            /// </summary>
            AvailableOnOtherModule = 0x08,

            /// <summary>
            /// Es gibt alternative Silos, jedoch ist das Material noch nicht verarbeitbar (OtherSilosAvailable is nicht gesetzt)
            /// </summary>
            AvailableButMaterialUnfinished = 0x10
        }

        /// <summary>
        /// 3.1 Hilfsmethode, die von Basisklasse aufgerufen wird um bei Materialmangel nachzufragen ob
        /// aktuelle Komponente automatisch abgebrochen und leergebucht werden darf
        /// </summary>
        /// <param name="silo"></param>
        /// <returns>True bedeutet, das Silo leergebucht werden darf und es evtl .aternative Silos gibt für einen Wechsel. 
        /// False bedeutet, dass kein automatisches Handling möglich ist und der Bediener oder die Klassenableitung darauf reagieren muss </returns>
        protected virtual EmptySiloHandlingOptions HandleAbortReasonOnEmptySilo(PAMSilo silo)
        {
            if (IsProduction)
                return HandleAbortReasonOnEmptySiloProd(silo);
            else if (IsTransport)
                return HandleAbortReasonOnEmptySiloPicking(silo);
            return EmptySiloHandlingOptions.NoSilosAvailable;
        }

        public virtual Msg CanResumeDosing()
        {
            if (IsProduction)
                return CanResumeDosingProd();
            else if (IsTransport)
                return CanResumeDosingPicking();
            return null;
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
                        PAFDosing dosing = null;
                        var pwGroup = ParentPWGroup;
                        if (pwGroup != null)
                            dosing = ParentPWGroup.GetExecutingFunction<PAFDosing>(taskEntry.RequestID);
                        else
                            Messages.LogError(this.GetACUrl(), "TaskCallback()", "ParentPWGroup is null");
                        CachedEmptySiloHandlingOption = null;
                        if (dosing != null)
                        {
                            DosingFuncResultState = dosing.DosingAbortReason.ValueT;
                            if (DosingFuncResultState == PADosingAbortReason.EndDosingThenDisThenEnd
                                || DosingFuncResultState == PADosingAbortReason.EndTolErrorDosingThenDisThenEnd)
                            {
                                ParentPWGroup.CurrentACSubState = (uint)ACSubStateEnum.SMEmptyingMode;
                                PWMethodVBBase rootPW = RootPW as PWMethodVBBase;
                                if (rootPW != null)
                                {
                                    PWGroupVB pwGroupVB = pwGroup as PWGroupVB;
                                    // Immer Sonderentleerziel in die Gruppe eintragen, weil evtl. andere Gruppen woanderst hin entleeren sollten
                                    // Falls der Anwender ein geminsames Ziel haben möchte, dann muss er manuell das Sonderentleerziel in den Root-Knoten ds Workfows eintragen.
                                    if (pwGroupVB != null)
                                        pwGroupVB.ExtraDisTargetDest = dosing.ExtraDisTargetDest;
                                    else
                                        rootPW.ExtraDisTargetDest = dosing.ExtraDisTargetDest;
                                    rootPW.SwitchToEmptyingMode();
                                }
                                else
                                    RootPW.CurrentACSubState = (uint)ACSubStateEnum.SMEmptyingMode;
                            }
                            else if (DosingFuncResultState == PADosingAbortReason.EndDosingThenDisThenNextComp
                                || DosingFuncResultState == PADosingAbortReason.EndTolErrorDosingThenDisThenNextComp)
                            {
                                ParentPWGroup.CurrentACSubState = (uint)ACSubStateEnum.SMDisThenNextComp;
                                PWMethodVBBase rootPW = RootPW as PWMethodVBBase;
                                if (rootPW != null)
                                {
                                    PWGroupVB pwGroupVB = pwGroup as PWGroupVB;
                                    // Immer Sonderentleerziel in die Gruppe eintragen, weil evtl. andere Gruppen woanderst hin entleeren sollten
                                    // Falls der Anwender ein geminsames Ziel haben möchte, dann muss er manuell das Sonderentleerziel in den Root-Knoten ds Workfows eintragen.
                                    if (pwGroupVB != null)
                                        pwGroupVB.ExtraDisTargetDest = dosing.ExtraDisTargetDest;
                                    else
                                        rootPW.ExtraDisTargetDest = dosing.ExtraDisTargetDest;
                                }
                            }
                            else if (DosingFuncResultState == PADosingAbortReason.EmptySourceEndBatchplan)
                            {
                                ParentPWGroup.CurrentACSubState = (uint)ACSubStateEnum.SMLastBatchEndOrder;
                                if (IsProduction)
                                    ParentPWMethod<PWMethodProduction>().EndBatchPlan();
                                else if (IsTransport)
                                    ParentPWMethod<PWMethodTransportBase>().EndPicking();
                                else
                                    RootPW.CurrentACSubState = (uint)ACSubStateEnum.SMLastBatchEndOrder;
                            }
                        }
                        DoDosingBooking(sender, e, wrapObject, DosingFuncResultState, dosing);
                        if (CurrentACMethod.ValueT != null)
                        {
                            RecalcTimeInfo();
                            FinishProgramLog(CurrentACMethod.ValueT);
                        }
                        _LastCallbackResult = e;
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
#endregion


#region Booking
        /// <summary>
        /// 4.1 Hauptbuchungsmethode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="wrapObject"></param>
        /// <param name="dis2SpecialDest"></param>
        public virtual Msg DoDosingBooking(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject, 
                                            PADosingAbortReason dosingFuncResultState, PAFDosing dosing, 
                                            string dis2SpecialDest = null, bool reEvaluatePosState = true,
                                            double? actualQuantity = null, double? tolerancePlus = null, double? toleranceMinus = null, double? targetQuantity = null)
        {
            bool isEndlessDosing = MinDosQuantity <= -0.00001;
            if (e != null)
            {
                IACTask taskEntry = wrapObject as IACTask;
                if (taskEntry.State == PointProcessingState.Deleted /*&& taskEntry.InProcess*/)
                {
                    ACMethod acMethod = e.ParentACMethod;
                    if (acMethod == null)
                        acMethod = taskEntry.ACMethod;
                    if (!actualQuantity.HasValue)
                        actualQuantity = (double)e["ActualQuantity"];
                    if (!tolerancePlus.HasValue)
                        tolerancePlus = (double)acMethod["TolerancePlus"];
                    if (!toleranceMinus.HasValue)
                        toleranceMinus = (double)acMethod["ToleranceMinus"];
                    targetQuantity = (double)acMethod["TargetQuantity"];
                    //var acValue = acMethod.ParameterValueList.GetACValue("EndlessDosing");
                    //if (acValue != null)
                    //    isEndlessDosing = acValue.ParamAsBoolean;
                }
            }

            Msg msg = null;
            if (actualQuantity.HasValue && tolerancePlus.HasValue && toleranceMinus.HasValue && targetQuantity.HasValue)
            {
                if (Math.Abs(actualQuantity.Value - 0) <= Double.Epsilon && BookTargetQIfZero)
                    actualQuantity = targetQuantity;
                bool thisDosingIsInTol = false;
                if (reEvaluatePosState 
                    && (actualQuantity >= (targetQuantity - toleranceMinus))
                    && !isEndlessDosing)
                    thisDosingIsInTol = true;

                //Regex rgx = new Regex("\\((.*?)\\)");
                //string loggerInstance = rgx.Replace(this.GetACUrl(), "");
                //PerformanceEvent perfEvent = null;
                if (IsProduction)
                    msg = DoDosingBookingProd(sender, e, wrapObject, dosingFuncResultState, dosing, dis2SpecialDest, reEvaluatePosState, actualQuantity, tolerancePlus, toleranceMinus, targetQuantity, isEndlessDosing, thisDosingIsInTol);
                else if (IsTransport)
                    msg = DoDosingBookingPicking(sender, e, wrapObject, dosingFuncResultState, dosing, dis2SpecialDest, reEvaluatePosState, actualQuantity, tolerancePlus, toleranceMinus, targetQuantity, isEndlessDosing, thisDosingIsInTol);
            }
            return msg;
        }



#endregion


#region Routing

        public static void RemoveFacility(Guid? ignoreFacilityID, IList<Facility> possibleSilos)
        {
            if (possibleSilos == null || !possibleSilos.Any())
                return;
            if (ignoreFacilityID.HasValue)
            {
                Facility facilityToRemove = possibleSilos.Where(c => c.FacilityID == ignoreFacilityID.Value).FirstOrDefault();
                if (facilityToRemove != null)
                {
                    possibleSilos.Remove(facilityToRemove);
                }
            }
        }


        public enum RouteQueryPurpose
        {
            StartDosing = 0,
            HandleEmptySilo = 1
        }

        public class RouteQueryParams
        {
            #region c'tors
            public RouteQueryParams(RouteQueryPurpose purpose)
            {
                _Purpose = purpose;
            }

            public RouteQueryParams(RouteQueryPurpose purpose, 
                ACPartslistManager.SearchMode searchMode,
                DateTime? filterTimeOlderThan,
                Guid? ignoreFacilityID,
                IEnumerable<gip.core.datamodel.ACClass> exclusionList)
            {
                _Purpose = purpose;
                SearchMode = searchMode;
                FilterTimeOlderThan = filterTimeOlderThan;
                IgnoreFacilityID = ignoreFacilityID;
                ExclusionList = exclusionList;
            }
            #endregion

            #region Parameter
            private RouteQueryPurpose _Purpose;
            public RouteQueryPurpose Purpose
            {
                get
                {
                    return _Purpose;
                }
            }

            public ACPartslistManager.SearchMode SearchMode { get; set; }
            public DateTime? FilterTimeOlderThan { get; set; }
            public Guid? IgnoreFacilityID { get; set; }
            public IEnumerable<gip.core.datamodel.ACClass> ExclusionList { get; set; }
            #endregion

            #region Result
            public EmptySiloHandlingOptions SuggestedOptionResult { get; set; }
            #endregion
        }

        public virtual IEnumerable<Route> GetRoutes(ProdOrderPartslistPosRelation relation,
                                                DatabaseApp dbApp, Database dbIPlus,
                                                RouteQueryParams queryParams,
                                                out IList<Facility> possibleSilos)
        {
            if (ParentPWGroup == null || ParentPWGroup.AccessedProcessModule == null || PartslistManager == null)
            {
                throw new NullReferenceException("AccessedProcessModule is null");
            }

            // FacilityNoSort

            core.datamodel.ACClass accessAClass = ParentPWGroup.AccessedProcessModule.ComponentClass;
            IEnumerable<Route> routes = PartslistManager.GetRoutes(relation, dbApp, dbIPlus,
                                        accessAClass,
                                        queryParams.SearchMode,
                                        queryParams.FilterTimeOlderThan,
                                        out possibleSilos,
                                        queryParams.IgnoreFacilityID,
                                        queryParams.ExclusionList);
            if (possibleSilos != null && possibleSilos.Any())
                possibleSilos = ApplyPriorizationRules(possibleSilos);
            return routes;
        }

        public virtual IEnumerable<Route> GetRoutes(PickingPos pickingPos,
                                                    DatabaseApp dbApp, Database dbIPlus,
                                                    RouteQueryParams queryParams,
                                                    out IList<Facility> possibleSilos)
        {
            if (ParentPWGroup == null || ParentPWGroup.AccessedProcessModule == null || PickingManager == null)
            {
                throw new NullReferenceException("AccessedProcessModule is null");
            }

            core.datamodel.ACClass accessAClass = ParentPWGroup.AccessedProcessModule.ComponentClass;
            IEnumerable<Route> routes = PickingManager.GetRoutes(pickingPos, dbApp, dbIPlus,
                                        accessAClass,
                                        queryParams.SearchMode,
                                        queryParams.FilterTimeOlderThan,
                                        out possibleSilos,
                                        queryParams.IgnoreFacilityID,
                                        queryParams.ExclusionList);
            if (possibleSilos != null && possibleSilos.Any())
                possibleSilos = ApplyPriorizationRules(possibleSilos);
            return routes;
        }

        protected virtual IList<Facility> ApplyPriorizationRules(IList<Facility> possibleSilos)
        {
            if (String.IsNullOrEmpty(FacilityNoSort))
                return possibleSilos;
            string[] facilitySortRules = FacilityNoSort.Split(';');
            if (facilitySortRules == null || !facilitySortRules.Any())
                return possibleSilos;
            List<Facility> priorizedList = new List<Facility>();
            List<Facility> posterizedList = new List<Facility>();
            int insertIndex = 0;
            foreach (string entry in facilitySortRules)
            {
                string rule = entry.Trim();
                if (string.IsNullOrEmpty(rule))
                    continue;
                Facility facility;
                if (rule[0] == '!')
                {
                    rule = rule.Substring(1);
                    if (String.IsNullOrEmpty(rule))
                        continue;
                    facility = possibleSilos.Where(c => c.FacilityNo == rule).FirstOrDefault();
                    if (facility != null)
                    {
                        possibleSilos.Remove(facility);
                        possibleSilos.Add(facility);
                    }
                }
                else
                {
                    facility = possibleSilos.Where(c => c.FacilityNo == rule).FirstOrDefault();
                    if (facility != null)
                    {
                        possibleSilos.Remove(facility);
                        possibleSilos.Insert(insertIndex, facility);
                        insertIndex++;
                    }
                }
            }
            return possibleSilos;
        }

        public virtual RouteItem CurrentDosingSource(Database db)
        {
            if (CurrentDosingRoute == null)
                return null;
            RouteItem item = CurrentDosingRoute.FirstOrDefault();
            if (item == null)
                return null;
            RouteItem clone = item.Clone() as RouteItem;
            if (db != null)
                clone.AttachTo(db);
            return clone;
        }

        public PAMSilo CurrentDosingSilo(Database db)
        {
            RouteItem item = CurrentDosingSource(null);
            if (item == null)
            {
                LastQueriedDosingSilo = null;
                return null;
            }
            PAMSilo source = item.SourceACComponent as PAMSilo;
            if (source != null)
                return source;

            item = CurrentDosingSource(db);
            if (item == null)
            {
                LastQueriedDosingSilo = null;
                return null;
            }
            source = item.SourceACComponent as PAMSilo;
            if (source == null && db == null && !item.IsAttached)
                item.AttachTo(this.Root.Database);
            item.Detach();
            LastQueriedDosingSilo = item.SourceACComponent as PAMSilo;
            return LastQueriedDosingSilo;
        }
        
#endregion


#region Misc
        public override PAOrderInfo GetPAOrderInfo()
        {
            if (CurrentACState == ACStateEnum.SMIdle || CurrentACState == ACStateEnum.SMBreakPoint)
                return null;

            PAOrderInfo info = null;
            if (CurrentDosingPos != null && CurrentDosingPos.ValueT != Guid.Empty)
            {
                info = new PAOrderInfo();
                if (IsProduction)
                    info.Add(ProdOrderPartslistPosRelation.ClassName, CurrentDosingPos.ValueT);
                else if (IsTransport)
                    info.Add(PickingPos.ClassName, CurrentDosingPos.ValueT);
            }
            return info;
        }

        /// <summary>
        /// Finds all Dosing-Nodes inside a PWGroup or a Loop. 
        /// If a Predecessor is a PWNodeOr and the one of the Inputs comes from a PWDosingLoop then the search will also be stopped
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchStartsFromNode"></param>
        /// <param name="stopSearchAtType">If null, then search stops at PWNodeOr</param>
        /// <returns></returns>
        public static List<T> FindPreviousDosingsInPWGroup<T>(PWBase searchStartsFromNode) where T : PWDosing
        {
            if (searchStartsFromNode == null)
                return new List<T>();
            return searchStartsFromNode.FindPredecessors<T>(true,
                                                    c => c is T,
                                                    c => c is PWNodeOr && (c as PWNodeOr).PWPointIn.ConnectionList.Where(d => d.ValueT is PWDosingLoop).Any(),
                                                    40);
        }

        internal virtual void OnDosingLoopDecision(PWDosingLoop dosingloop, bool willRepeatDosing)
        {
            if (willRepeatDosing && IsTransport && ParentPWGroup != null)
            {
                RepeatDosingForPicking = true;
            }
        }
        #endregion



        #region Alarmhandling
        public override void AcknowledgeAlarms()
        {
            CachedEmptySiloHandlingOption = null;
            NoSourceFoundForDosing.ValueT = 0;
            base.AcknowledgeAlarms();
        }
#endregion


#region User-Interaction-Methods

        #region Cancel Component
        [ACMethodInteraction("", "en{'Dont dose current component'}de{'Aktuelle Komponente nicht mehr dosieren'}", 800, true)]
        public virtual void CancelCurrentComponent()
        {
            if (!IsEnabledCancelCurrentComponent())
                return;
            AcknowledgeAlarms();
            NoSourceFoundForDosing.ValueT = 2;
        }

        public virtual bool IsEnabledCancelCurrentComponent()
        {
            return NoSourceFoundForDosing.ValueT == 1;
        }
        #endregion

        #region Acknowledge Empty Scale
        [ACMethodInteraction("", "en{'Nevertheless, dose when the scale is not empty'}de{'Trotzdem bei nicht leerer Waage dosieren'}", 801, true)]
        public virtual void AckNotEmptyScale()
        {
            if (!IsEnabledAckNotEmptyScale())
                return;
            AcknowledgeAlarms();
            EmptyScaleAlarm = EmptyScaleAlarmState.Acknowledged;
        }

        public virtual bool IsEnabledAckNotEmptyScale()
        {
            return EmptyScaleAlarm == EmptyScaleAlarmState.Alarm;
        }
        #endregion


        #region Reset
        public override void Reset()
        {
            if (CurrentDosingRoute != null)
            {
                CurrentDosingRoute.Detach();
                CurrentDosingRoute = null;
            }
            this.TaskSubscriptionPoint.UnSubscribe();

            ClearMyConfiguration();
            NoSourceWait = null;
            NoSourceFoundForDosing.ValueT = 0;
            CachedEmptySiloHandlingOption = null;
            UnSubscribeToProjectWorkCycle();

            base.Reset();
        }

        public override bool IsEnabledReset()
        {
            //if (this.TaskSubscriptionPoint.ConnectionList.Any())
                //return false;
            return base.IsEnabledReset();
        }

        #endregion


        #endregion


        #region Planning and Testing
        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList)
        {
            base.DumpPropertyList(doc, xmlACPropertyList);

            XmlElement xmlChild = xmlACPropertyList["NoSourceWait"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("NoSourceWait");
                if (xmlChild != null)
                    xmlChild.InnerText = NoSourceWait.HasValue ? NoSourceWait.Value.ToString() : "null";
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["NextCheckIfPWDosingsFinished"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("NextCheckIfPWDosingsFinished");
                if (xmlChild != null)
                    xmlChild.InnerText = NextCheckIfPWDosingsFinished.HasValue ? NextCheckIfPWDosingsFinished.Value.ToString() : "null";
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["CurrentParallelPWDosings"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("CurrentParallelPWDosings");
                if (xmlChild != null)
                {
                    var parallelDosings = CurrentParallelPWDosings;
                    if (parallelDosings == null || !parallelDosings.Any())
                        xmlChild.InnerText = "null";
                    else
                    {
                        int i = 0;
                        StringBuilder sb = new StringBuilder();
                        foreach (var element in parallelDosings)
                        {
                            sb.AppendFormat("{0}:{1}|", i, element.GetACUrl());
                        }
                        xmlChild.InnerText = sb.ToString();
                    }
                }
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["LastQueriedDosingSilo"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("LastQueriedDosingSilo");
                if (xmlChild != null)
                    xmlChild.InnerText = LastQueriedDosingSilo != null ? LastQueriedDosingSilo.GetACUrl() : "null";
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["DosingFuncResultState"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("DosingFuncResultState");
                if (xmlChild != null)
                    xmlChild.InnerText = DosingFuncResultState.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["CachedEmptySiloHandlingOption"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("CachedEmptySiloHandlingOption");
                if (xmlChild != null)
                    xmlChild.InnerText = CachedEmptySiloHandlingOption.HasValue? CachedEmptySiloHandlingOption.ToString() : "null";
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["MaxWeightAlarmSet"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("MaxWeightAlarmSet");
                if (xmlChild != null)
                    xmlChild.InnerText = MaxWeightAlarmSet.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["RepeatDosingForPicking"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("RepeatDosingForPicking");
                if (xmlChild != null)
                    xmlChild.InnerText = RepeatDosingForPicking.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["EmptyScaleAlarm"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("EmptyScaleAlarm");
                if (xmlChild != null)
                    xmlChild.InnerText = EmptyScaleAlarm.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["ComponentsSkippable"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("ComponentsSkippable");
                if (xmlChild != null)
                    xmlChild.InnerText = ComponentsSkippable.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["ScaleOtherComp"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("ScaleOtherComp");
                if (xmlChild != null)
                    xmlChild.InnerText = ScaleOtherComp.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["ManuallyChangeSource"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("ManuallyChangeSource");
                if (xmlChild != null)
                    xmlChild.InnerText = ManuallyChangeSource.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["OldestSilo"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("OldestSilo");
                if (xmlChild != null)
                    xmlChild.InnerText = OldestSilo.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["AutoChangeScale"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("AutoChangeScale");
                if (xmlChild != null)
                    xmlChild.InnerText = AutoChangeScale.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["MinDosQuantity"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("MinDosQuantity");
                if (xmlChild != null)
                    xmlChild.InnerText = MinDosQuantity.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["ComponentsSeqFrom"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("ComponentsSeqFrom");
                if (xmlChild != null)
                    xmlChild.InnerText = ComponentsSeqFrom.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["ComponentsSeqTo"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("ComponentsSeqTo");
                if (xmlChild != null)
                    xmlChild.InnerText = ComponentsSeqTo.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["CheckScaleEmpty"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("CheckScaleEmpty");
                if (xmlChild != null)
                    xmlChild.InnerText = CheckScaleEmpty.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["BookTargetQIfZero"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("BookTargetQIfZero");
                if (xmlChild != null)
                    xmlChild.InnerText = BookTargetQIfZero.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["DoseFromFillingSilo"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("DoseFromFillingSilo");
                if (xmlChild != null)
                    xmlChild.InnerText = DoseFromFillingSilo.HasValue ? DoseFromFillingSilo.ToString() : "null";
                xmlACPropertyList.AppendChild(xmlChild);
            }


            xmlChild = xmlACPropertyList["FacilityNoSort"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("FacilityNoSort");
                if (xmlChild != null)
                    xmlChild.InnerText = FacilityNoSort;
                xmlACPropertyList.AppendChild(xmlChild);
            }
        }
        #endregion

        #endregion
    }
}
