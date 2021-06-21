using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System.Threading;

namespace gip.mes.processapplication
{
    /// <summary>
    /// Workflow-Root für Produktion
    /// </summary>
    [ACClassConstructorInfo(
    new object[] 
        { 
            new object[] {gip.core.datamodel.ACProgram.ClassName, Global.ParamOption.Required, typeof(Guid)},
            new object[] {gip.core.datamodel.ACProgramLog.ClassName, Global.ParamOption.Optional, typeof(Guid)},
            new object[] {ProdOrderPartslistPos.ClassName, Global.ParamOption.Required, typeof(Guid)},
            new object[] {PWMethodVBBase.IsLastBatchParamName, Global.ParamOption.Optional, typeof(Int16) }
       }
    )]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Production'}de{'Produktion'}", Global.ACKinds.TPWMethod, Global.ACStorableTypes.Optional, true, true, "", "PWBSOMethod/CurrentACProgram", 10)]
    public class PWMethodProduction : PWMethodVBBase
    {
        new public const string PWClassName = "PWMethodProduction";

        #region c´tors

        static PWMethodProduction()
        {
            RegisterExecuteHandler(typeof(PWMethodProduction), HandleExecuteACMethod_PWMethodProduction);
        }

        public PWMethodProduction(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            _ProdOrderManager = ACProdOrderManager.ACRefToServiceInstance(this);
            if (_ProdOrderManager == null)
                throw new Exception("ProdOrderManager not configured");
            _PartslistManager = ACPartslistManager.ACRefToServiceInstance(this);
            if (_PartslistManager == null)
                throw new Exception("PartslistManager not configured");
            _MatReqManager = facility.ACMatReqManager.ACRefToServiceInstance(this);
            if (_MatReqManager == null)
                throw new Exception("MatReqManager is not configured");
            _LabOrderManager = ACLabOrderManager.ACRefToServiceInstance(this);
            if(_LabOrderManager == null)
                throw new Exception("LabOrderManager is not configured");

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            ACProdOrderManager.DetachACRefFromServiceInstance(this, _ProdOrderManager);
            _ProdOrderManager = null;
            ACPartslistManager.DetachACRefFromServiceInstance(this, _PartslistManager);
            _PartslistManager = null;
            facility.ACMatReqManager.DetachACRefFromServiceInstance(this, _MatReqManager);
            _MatReqManager = null;
            ACLabOrderManager.DetachACRefFromServiceInstance(this, _LabOrderManager);
            _LabOrderManager = null;
            UnregisterFromCachedDestinations();

            using (ACMonitor.Lock(_20015_LockValue))
            {
                //_ACProgramVB = null;
                _CurrentProdOrderBatch = null;
                _CurrentProdOrderPartslistPos = null;
                _NewAddedProgramLog = null;
            }


            if (!base.ACDeInit(deleteACClassTask))
                return false;

            return true;
        }

        public override void Recycle(IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {
            using (ACMonitor.Lock(_20015_LockValue))
            {
                //_ACProgramVB = null;
                _CurrentProdOrderBatch = null;
                _CurrentProdOrderPartslistPos = null;
                _NewAddedProgramLog = null;
            }

            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }
        #endregion

        #region Properties

        #region Properties -> WF selected

        protected ACRef<ACProdOrderManager> _ProdOrderManager = null;
        public ACProdOrderManager ProdOrderManager
        {
            get
            {
                if (_ProdOrderManager == null)
                    return null;
                return _ProdOrderManager.ValueT;
            }
        }

        protected ACRef<ACPartslistManager> _PartslistManager = null;
        public ACPartslistManager PartslistManager
        {
            get
            {
                if (_PartslistManager == null)
                    return null;
                return _PartslistManager.ValueT;
            }
        }

        protected ACRef<facility.ACMatReqManager> _MatReqManager = null;
        public facility.ACMatReqManager MatReqManager
        {
            get
            {
                if (_MatReqManager == null)
                    return null;
                return _MatReqManager.ValueT;
            }
        }

        protected ACRef<ACLabOrderManager> _LabOrderManager = null;
        public ACLabOrderManager LabOrderManager
        {
            get
            {
                if (_LabOrderManager == null)
                    return null;
                return _LabOrderManager.ValueT;
            }
        }


        private gip.mes.datamodel.ProdOrderPartslistPos _CurrentProdOrderPartslistPos = null;
        /// <summary>
        /// CurrentProdOrderPartslistPos could be a Child-Pos which is a Batch. If this node is an Instance on planning level, then the Position is the root intermediate Pos.
        /// </summary>
        public gip.mes.datamodel.ProdOrderPartslistPos CurrentProdOrderPartslistPos
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_CurrentProdOrderPartslistPos != null)
                        return _CurrentProdOrderPartslistPos;
                }
                LoadVBEntities();

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _CurrentProdOrderPartslistPos;
                }
            }
        }

        private gip.mes.datamodel.ProdOrderBatch _CurrentProdOrderBatch = null;
        public gip.mes.datamodel.ProdOrderBatch CurrentProdOrderBatch
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_CurrentProdOrderBatch != null)
                        return _CurrentProdOrderBatch;
                }
                LoadVBEntities();

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _CurrentProdOrderBatch;
                }
            }
        }

        protected bool _IsStartingMethodProduction = false;
        protected bool IsStartingMethodProduction
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _IsStartingMethodProduction;
                }
            }
            private set
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _IsStartingMethodProduction = value;
                }
            }
        }

        //private MaterialWFACClassMethod _ConfigStageMatWF = null;
        //private Partslist _ConfigStagePartslist = null;
        //private MaterialWF _ConfigMaterialWF = null;
        //private ProdOrderPartslist _ConfigStageProdPartslist = null;

        #endregion


        //#region IACConfigMethodHierarchy

        //public override List<gip.core.datamodel.ACClassMethod> ACConfigMethodHierarchy
        //{
        //    get
        //    {
        //        List<gip.core.datamodel.ACClassMethod> methods = new List<gip.core.datamodel.ACClassMethod>();
        //        IACComponentPWNode invoker = null;
        //        if (this.ContentACClassWF != null)
        //        {
        //            methods.Add(ContentACClassWF.ACClassMethod);
        //            if (RootPW != null && CurrentTask != null && CurrentTask.ValueT != null)
        //            {
        //                invoker = CurrentTask.ValueT as IACComponentPWNode;
        //                if (invoker != null && invoker.ContentACClassWF != null)
        //                    methods.Add(invoker.ContentACClassWF.ACClassMethod);
        //            }
        //        }
        //        int i = 1;
        //        foreach (gip.core.datamodel.ACClassMethod method in methods)
        //        {
        //            method.Priority = i;
        //            i++;
        //        }
        //        return methods;
        //    }
        //}

        //#endregion

        #region IACConfigStoreSelection
        protected override void OnRebuildMandatoryConfigStoresCache(IACComponentPWNode invoker, List<IACConfigStore> mandatoryConfigStores, bool recalcExpectedConfigStoresCount)
        {
            base.OnRebuildMandatoryConfigStoresCache(invoker, mandatoryConfigStores, recalcExpectedConfigStoresCount);
            Guid acClassMethodID = invoker != null ? invoker.ContentACClassWF.ACClassMethodID : ContentACClassWF.ACClassMethodID;
            if (CurrentProdOrderPartslistPos != null)
            {
                ConfigManagerIPlus serviceInstance = ConfigManagerIPlus.GetServiceInstance(this);

                string errorMessage = null;
                int expectedConfigStoresCount = 0;
                List<IACConfigStore> prodPartslistOfflineList = (serviceInstance as ConfigManagerIPlusMES).GetProductionPartslistConfigStoreOfflineList(ContentTask.ACClassTaskID, acClassMethodID, out expectedConfigStoresCount, out errorMessage);
                if (prodPartslistOfflineList != null)
                    mandatoryConfigStores.AddRange(prodPartslistOfflineList);
                else
                {
                    ProcessAlarm.ValueT = PANotifyState.AlarmOrFault;
                    if (String.IsNullOrEmpty(errorMessage))
                        errorMessage = "";
                    errorMessage = errorMessage + " Configuration could not be loaded. Workflownodes will run with wrong parameters. If you acknowledge the alarm, the workflow will continue otherwise reset the Workflow manually!";
                    OnNewAlarmOccurred(ProcessAlarm, new Msg(errorMessage, this, eMsgLevel.Error, PWClassName, "MandatoryConfigStores", 1000), true);
                    Messages.LogError(this.GetACUrl(), "OnRebuildMandatoryConfigStoresCache(10)", errorMessage);
                }
                if (recalcExpectedConfigStoresCount)
                {
                    using (ACMonitor.Lock(_20015_LockStoreList))
                    {
                        _ExpectedConfigStoresCount += expectedConfigStoresCount;
                    }
                }
            }
            else if (recalcExpectedConfigStoresCount)
            {
                Messages.LogError(this.GetACUrl(), "OnRebuildMandatoryConfigStoresCache(20)", "CurrentProdOrderPartslistPos is null => ConfigStore-Validation will fail!");
                Messages.LogError(this.GetACUrl(), "OnRebuildMandatoryConfigStoresCache(10)", System.Environment.StackTrace);
                using (ACMonitor.Lock(_20015_LockStoreList))
                {
                    // Minimum is ProdOrderPartslistConfig and PartslistConfig:
                    _ExpectedConfigStoresCount += 2;
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        #region overrides
        protected override bool OnWorkflowUnloading(int retryUnloadCountDown)
        {
            try
            {
                if (CurrentProdOrderPartslistPos != null)
                {
                    using (DatabaseApp dbApp = new DatabaseApp())
                    {
                        var pos = CurrentProdOrderPartslistPos.FromAppContext<ProdOrderPartslistPos>(dbApp);
                        if (pos != null)
                        {
                            pos.ProdOrderPartslist.EndDate = DateTime.Now;
                            pos.ACClassTaskID = null;

                            if (pos.ParentProdOrderPartslistPosID.HasValue)
                            {
                                pos.MDProdOrderPartslistPosState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed)
                                        .FirstOrDefault();
                                if (pos.ProdOrderBatch != null)
                                {
                                    pos.ProdOrderBatch.MDProdOrderState = DatabaseApp.s_cQry_GetMDProdOrderState(dbApp, MDProdOrderState.ProdOrderStates.ProdFinished)
                                        .FirstOrDefault();
                                }
                            }

                            MsgWithDetails saveMsg = dbApp.ACSaveChanges();
                            if (saveMsg != null)
                            {
                                Messages.LogError(this.GetACUrl(), "OnWorkflowUnloading(0)", saveMsg.InnerMessage);
                                return false;
                            }
                            else
                                return true;
                        }
                        else
                        {
                            Messages.LogError(this.GetACUrl(), "OnWorkflowUnloading(1)", "pos is null");
                            return false;
                        }
                    }
                }
                else if (this.ContentTask != null)
                {
                    using (DatabaseApp dbApp = new DatabaseApp())
                    {
                        var vbTask = this.ContentTask.FromAppContext<gip.mes.datamodel.ACClassTask>(dbApp);
                        if (vbTask != null)
                        {
                            foreach (var pos in vbTask.ProdOrderPartslistPos_ACClassTask)
                            {
                                pos.ACClassTaskID = null;
                            }
                        }
                        MsgWithDetails saveMsg = dbApp.ACSaveChanges();
                        if (saveMsg != null)
                        {
                            Messages.LogError(this.GetACUrl(), "OnWorkflowUnloading(2)", saveMsg.InnerMessage);
                            return false;
                        }
                        else
                            return true;
                    }
                }
            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), "OnWorkflowUnloading(3)", e);
                return false;
            }
            return true;
        }

        public override PAOrderInfo GetPAOrderInfo()
        {
            PAOrderInfo orderInfo = base.GetPAOrderInfo();

            if (CurrentProdOrderBatch == null || CurrentProdOrderPartslistPos == null)
                return orderInfo;

            if (orderInfo == null)
                orderInfo = new PAOrderInfo();

            orderInfo.Add(ProdOrderBatch.ClassName, CurrentProdOrderBatch.ProdOrderBatchID);
            orderInfo.Add(ProdOrderPartslistPos.ClassName, CurrentProdOrderPartslistPos.ProdOrderPartslistPosID);

            return orderInfo;
        }

        #endregion

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "OnProdOrderStarted":
                    OnProdOrderStarted(acParameter[0] as ProdOrderPartslistPos);
                    return true;
                case "IsProdOrderBatchStarted":
                    IsProdOrderBatchStarted((Guid)acParameter[0]);
                    return true;
                case "EndBatchPlan":
                    EndBatchPlan();
                    return true;
                case "ReloadBatchPlans":
                    ReloadBatchPlans();
                    return true;
                case "ReloadBPAndResume":
                    ReloadBPAndResume();
                    return true;
                case Const.IsEnabledPrefix + "EndBatchPlan":
                    result = IsEnabledEndBatchPlan();
                    return true;
                case Const.IsEnabledPrefix + "ReloadBatchPlans":
                    result = IsEnabledReloadBatchPlans();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PWMethodProduction(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case Const.AskUserPrefix + "EndBatchPlan":
                    result = AskUserEndBatchPlan(acComponent);
                    return true;
            }
            return HandleExecuteACMethod_PWMethodVBBase(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

#endregion

#region ACState-Methods
        public override ACMethodEventArgs Start(ACMethod acMethod)
        {
            if (!IsEnabledStart(acMethod))
                return CreateNewMethodEventArgs(acMethod, Global.ACMethodResultState.Failed);

            try
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    EnteringStartMethod();
                    ProdOrderBatch currentProdOrderBatch = null;
                    ProdOrderPartslistPos currentProdOrderPartslistPos = null;
                    Guid prodOrderPartslistPosID = (Guid)acMethod[ProdOrderPartslistPos.ClassName];
                    if (prodOrderPartslistPosID != Guid.Empty)
                        currentProdOrderPartslistPos = dbApp.ProdOrderPartslistPos.Where(c => c.ProdOrderPartslistPosID == prodOrderPartslistPosID).FirstOrDefault();
                    if (currentProdOrderPartslistPos != null)
                    {
                        currentProdOrderPartslistPos.ACClassTaskID = this.ContentTask.ACClassTaskID;
                        if (currentProdOrderPartslistPos.ProdOrderBatch != null)
                            currentProdOrderPartslistPos.ProdOrderBatch.MDProdOrderState = DatabaseApp.s_cQry_GetMDProdOrderState(dbApp, MDProdOrderState.ProdOrderStates.InProduction).FirstOrDefault();
                    }
                    var msg = dbApp.ACSaveChangesWithRetry();
                    if (msg != null)
                    {
                        ProcessAlarm.ValueT = PANotifyState.AlarmOrFault;
                        OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.InnerMessage, this, eMsgLevel.Error, PWClassName, "Start", 1000), true);
                        Messages.LogError(this.GetACUrl(), "Start(0)", msg.InnerMessage);
                        return CreateNewMethodEventArgs(acMethod, Global.ACMethodResultState.Failed);
                    }

                    if (currentProdOrderPartslistPos != null)
                        currentProdOrderBatch = currentProdOrderPartslistPos.ProdOrderBatch;
                    using (ACMonitor.Lock(_20015_LockValue))
                    {
                        _CurrentProdOrderBatch = currentProdOrderBatch;
                        _CurrentProdOrderPartslistPos = currentProdOrderPartslistPos;
                    }

                    OnEnterStart(acMethod);
                    ACUrlCommand("!OnProdOrderStarted", currentProdOrderPartslistPos);

                    if (currentProdOrderBatch != null)
                        dbApp.Detach(currentProdOrderBatch);
                    if (currentProdOrderPartslistPos != null)
                        dbApp.Detach(currentProdOrderPartslistPos);
                    LeavingStartMethod();
                    OnLeaveStart(acMethod);
                }
            }
            catch (Exception e)
            {
                //Exception50000: {0}\n{1}\n{2}
                Msg msg = new Msg(this, eMsgLevel.Exception, PWClassName, "Start", 1000, "Exception50000",
                                    e.Message, e.InnerException != null ? e.InnerException.Message : "", e.StackTrace);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                Messages.LogException(this.GetACUrl(), "Start(0)", msg.Message);
                throw e;
            }
            finally
            {
                ResetStartingProcessFunction();
            }
            return CreateNewMethodEventArgs(acMethod, Global.ACMethodResultState.InProcess);
        }

        protected override bool CanRunWorkflow()
        {
            bool canRunWF = false;

            using (ACMonitor.Lock(_20015_LockValue))
            {
                canRunWF = _CurrentProdOrderBatch != null || _CurrentProdOrderPartslistPos != null;
            }
            if (!canRunWF)
                return false;

            return ProcessAlarm.ValueT == PANotifyState.Off;
        }


        public override void AcknowledgeAlarms()
        {
            if (!IsEnabledAcknowledgeAlarms())
                return;
            ProcessAlarm.ValueT = PANotifyState.Off;
            base.AcknowledgeAlarms();
        }


        public override bool IsEnabledAcknowledgeAlarms()
        {
            if (ProcessAlarm.ValueT == PANotifyState.AlarmOrFault)
                return true;
            return base.IsEnabledAcknowledgeAlarms();
        }


        [ACMethodInfo("Function", "en{'OnProdOrderStarted'}de{'OnProdOrderStarted'}", 9999)]
        public virtual void OnProdOrderStarted(ProdOrderPartslistPos currentProdOrderPartslistPos)
        {
        }

        public override bool IsEnabledReStart()
        {
            bool isEnabled = base.IsEnabledReStart();
            if (!isEnabled)
                return false;
            return CurrentProdOrderPartslistPos != null;
        }

#endregion

#region Helper Methods
        protected virtual void LoadVBEntities()
        {
            var rootPW = RootPW;
            if (rootPW == null)
                return;
            if (this.ContentTask.EntityState == System.Data.EntityState.Added)
            {
                Messages.LogError(this.GetACUrl(), "LoadVBEntities(10)", "EntityState of ContentTask is Added and not saved to the database. The call of LoadVBEntities is too early!");
                Messages.LogError(this.GetACUrl(), "LoadVBEntities(11)", System.Environment.StackTrace);
                return;
            }
            if (IsStartingProcessFunction)
            {
                Messages.LogError(this.GetACUrl(), "LoadVBEntities(20)", "IsStartingProcessFunction is true. The call of LoadVBEntities is too early!");
                Messages.LogError(this.GetACUrl(), "LoadVBEntities(21)", System.Environment.StackTrace);
                return;
            }
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                var contentTaskVB = this.ContentTask.FromAppContext<gip.mes.datamodel.ACClassTask>(dbApp);
                if (contentTaskVB != null)
                {
                    ProdOrderBatch currentProdOrderBatch = null;
                    ProdOrderPartslistPos currentProdOrderPartslistPos = null;

                    currentProdOrderPartslistPos = contentTaskVB.ProdOrderPartslistPos_ACClassTask.FirstOrDefault();
                    if (currentProdOrderPartslistPos != null)
                    {
                        currentProdOrderBatch = currentProdOrderPartslistPos.ProdOrderBatch;
                        if (currentProdOrderBatch != null)
                            dbApp.Detach(currentProdOrderBatch);
                        dbApp.Detach(currentProdOrderPartslistPos);
                    }

                    using (ACMonitor.Lock(_20015_LockValue))
                    {
                        _CurrentProdOrderPartslistPos = currentProdOrderPartslistPos;
                        _CurrentProdOrderBatch = currentProdOrderBatch;
                    }
                }
            }
        }

        private gip.core.datamodel.ACProgramLog _NewAddedProgramLog = null;
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
                ProdOrderPartslistPos currentProdOrderPartslistPos = null;

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    currentProdOrderPartslistPos = _CurrentProdOrderPartslistPos;
                }

                if (currentProdOrderPartslistPos != null)
                {
                    this.ApplicationManager.ApplicationQueue.Add(() =>
                    //ThreadPool.QueueUserWorkItem((object state) =>
                    {
                        using (DatabaseApp dbApp = new DatabaseApp())
                        {
                            OrderLog orderLog = OrderLog.NewACObject(dbApp, _NewAddedProgramLog);
                            orderLog.ProdOrderPartslistPosID = currentProdOrderPartslistPos.ProdOrderPartslistPosID;
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

        [ACMethodInfo("Query", "en{'IsProdOrderBatchStarted'}de{'IsProdOrderBatchStarted'}", 9999)]
        public bool IsProdOrderBatchStarted(Guid prodOrderBatchID)
        {
            if (CurrentProdOrderBatch == null)
                return false;
            return CurrentProdOrderBatch.ProdOrderBatchID == prodOrderBatchID;
        }


        private class BatchPlanTargets
        {
            public Guid[] _LastTargets = null;
            public DateTime? _NextTargetQuery = null;
            public List<int> _PWMethods = new List<int>();
        }

        private static Dictionary<Guid, BatchPlanTargets> _BatchPlanTargetCache = new Dictionary<Guid, BatchPlanTargets>();
        private static object _TargetCacheLock = new object();

        private void UnregisterFromCachedDestinations()
        {
            try
            {
                ProdOrderBatch currentProdOrderBatch = null;

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    currentProdOrderBatch = _CurrentProdOrderBatch;
                }

                if (currentProdOrderBatch != null && currentProdOrderBatch.ProdOrderBatchPlanID.HasValue)
                {
                    Guid batchPlanID = currentProdOrderBatch.ProdOrderBatchPlanID.Value;
                    lock (_TargetCacheLock)
                    {
                        BatchPlanTargets targetCache = null;
                        if (_BatchPlanTargetCache.TryGetValue(batchPlanID, out targetCache))
                        {
                            int myHashCode = this.GetHashCode();
                            if (targetCache._PWMethods.Contains(myHashCode))
                                targetCache._PWMethods.Remove(myHashCode);
                            if (targetCache._PWMethods.Count <= 0)
                                _BatchPlanTargetCache.Remove(batchPlanID);
                        }
                    }
                }
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("PWMethodProduction", "UnregisterFromCachedDestinations", msg);
            }
        }

        public virtual Guid[] GetCachedDestinations(bool refreshCache, out short errorCode, out string errorMsg)
        {
            errorMsg = null;
            try
            {
                if (CurrentProdOrderBatch != null
                    && CurrentProdOrderBatch.ProdOrderBatchPlanID.HasValue)
                {
                    BatchPlanTargets targetCache = null;
                    Guid batchPlanID = CurrentProdOrderBatch.ProdOrderBatchPlanID.Value;
                    lock (_TargetCacheLock)
                    {
                        if (!_BatchPlanTargetCache.TryGetValue(batchPlanID, out targetCache))
                        {
                            targetCache = new BatchPlanTargets() { _NextTargetQuery = DateTime.Now.AddSeconds(20) };
                            _BatchPlanTargetCache.Add(batchPlanID, targetCache);
                        }
                        int myHashCode = this.GetHashCode();
                        if (!targetCache._PWMethods.Contains(myHashCode))
                            targetCache._PWMethods.Add(myHashCode);
                    }

                    if (targetCache._NextTargetQuery == null || DateTime.Now > targetCache._NextTargetQuery.Value || targetCache._LastTargets == null || refreshCache)
                    {
                        using (var dbApp = new DatabaseApp())
                        {
                            ProdOrderPartslistPos currentBatchPos = CurrentProdOrderPartslistPos.FromAppContext<ProdOrderPartslistPos>(dbApp);
                            var batchPlan = dbApp.ProdOrderBatchPlan.Where(c => c.ProdOrderBatchPlanID == batchPlanID).FirstOrDefault();
                            if (batchPlan == null)
                            {
                                errorCode = 1;
                                return null;
                            }
                            IList<FacilityReservation> plannedSilos = batchPlan.FacilityReservation_ProdOrderBatchPlan.OrderBy(c => c.Sequence).ToArray();
                            if (plannedSilos == null || !plannedSilos.Any())
                            {
                                errorCode = 2;
                                return null;
                            }

                            PWDischarging lastDischargingNode = null;
                            var disNodes = FindChildComponents<PWDischarging>(c => c is PWDischarging);
                            if (disNodes != null && disNodes.Any())
                            {
                                lastDischargingNode = disNodes.Where(c => c.IsLastDischargingNode).FirstOrDefault();
                                if (lastDischargingNode == null)
                                {
                                    foreach (PWDischarging disNode in disNodes)
                                    {
                                        MaterialWFConnection connectionToDischarging = null;
                                        if (disNode.IsLastDisNode(dbApp, batchPlan, out connectionToDischarging))
                                        {
                                            lastDischargingNode = disNode;
                                            break;
                                        }
                                    }
                                }
                                if (lastDischargingNode == null)
                                {
                                    var endNode = FindChildComponents<PWNodeEnd>(c => c is PWNodeEnd, null, 1).FirstOrDefault();
                                    if (endNode != null)
                                    {
                                        var sourceGroups = endNode.PWPointIn.ConnectionList
                                                        .Where(c => c.ValueT is PWGroup)
                                                        .Select(c => c.ValueT as PWGroup).ToList();
                                        foreach (var pwGroup in sourceGroups)
                                        {
                                            lastDischargingNode = pwGroup.FindChildComponents<PWDischarging>(c => c is PWDischarging).FirstOrDefault();
                                            if (lastDischargingNode != null)
                                                break;
                                        }
                                    }
                                }
                                if (lastDischargingNode == null)
                                    lastDischargingNode = disNodes.FirstOrDefault();
                            }

                            FacilityReservation facilityReservation = null;
                            if (lastDischargingNode != null)
                            {
                                facilityReservation = lastDischargingNode.GetNextFreeDestination(plannedSilos, currentBatchPos, false);
                            }
                            else
                            {
                                facilityReservation = PWDischarging.GetNextFreeDestination(this, plannedSilos, currentBatchPos, false);
                            }

                            if (facilityReservation != null)
                            {
                                targetCache._LastTargets = new Guid[] { facilityReservation.VBiACClassID.Value };
                            }
                            else
                            {
                                targetCache._LastTargets = plannedSilos.OrderBy(c => c.VBiACClassID.Value).Select(c => c.VBiACClassID.Value).ToArray();
                            }
                        }
                        // Alle 20 Sekunden abfragen um Datenbankbelastung zu reduzieren
                        targetCache._NextTargetQuery = DateTime.Now.AddSeconds(20);
                    }
                    errorCode = 0;
                    return targetCache._LastTargets;
                }
                else if (ParentTaskExecComp != null && CurrentTask != null)
                {
                    PWNodeProcessWorkflow invoker = CurrentTask.ValueT as PWNodeProcessWorkflow;
                    if (invoker != null)
                    {
                        var result = invoker.GetCachedDestinations(refreshCache, out errorCode, out errorMsg);
                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                errorMsg = e.Message;
                errorCode = -1;
                return null;
            }
            errorMsg = "No targets found";
            errorCode = -1;
            return null;
        }

        #endregion

        #region User Interaction Methods
        [ACMethodInteraction("", "en{'End current batch plan/Order'}de{'Beende aktuellen Batchplan/Auftrag'}", 296, true)]
        public virtual void EndBatchPlan()
        {
            if (!IsEnabledEndBatchPlan())
                return;
            if (CurrentProdOrderBatch != null)
            {
                using (var dbApp = new DatabaseApp())
                {
                    ProdOrderBatch batch = CurrentProdOrderBatch.FromAppContext<ProdOrderBatch>(dbApp);
                    if (batch.ProdOrderBatchPlan != null)
                    {
                        if (batch.ProdOrderBatchPlan.PlanMode == GlobalApp.BatchPlanMode.UseFromTo)
                        {
                            batch.ProdOrderBatchPlan.BatchNoTo = batch.BatchSeqNo;
                        }
                        else if (batch.ProdOrderBatchPlan.PlanMode == GlobalApp.BatchPlanMode.UseBatchCount)
                        {
                            batch.ProdOrderBatchPlan.BatchTargetCount = batch.ProdOrderBatchPlan.BatchActualCount;
                        }
                        else //if (batchPlanEntry.PlanMode == GlobalApp.BatchPlanMode.UseTotalSize)
                        {
                            batch.ProdOrderBatchPlan.TotalSize = batch.ProdOrderBatchPlan.ActualQuantity;
                        }
                        batch.ProdOrderBatchPlan.PlanState = GlobalApp.BatchPlanState.Completed;
                    }
                    dbApp.ACSaveChanges();
                }
                ReloadBatchPlans();
            }
            CurrentACSubState = (uint) ACSubStateEnum.SMLastBatchEndOrder;
        }

        public virtual bool IsEnabledEndBatchPlan()
        {
            return !((ACSubStateEnum)CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrder);
        }

        public static bool AskUserEndBatchPlan(IACComponent acComponent)
        {
            if (acComponent == null)
                return false;
            // TODO Übersetzung:
            return acComponent.Messages.Question(acComponent, "Question50021", Global.MsgResult.Yes) == Global.MsgResult.Yes;
        }


        [ACMethodInteraction("", "en{'Reload batch plans'}de{'Batchpläne nachladen'}", 298, true)]
        public virtual void ReloadBatchPlans()
        {
            if (!IsEnabledReloadBatchPlans())
                return;
            FindChildComponents<PWNodeProcessWorkflowVB>(c => c is PWNodeProcessWorkflowVB).ForEach(c => c.ReloadBatchPlans());
        }

        public virtual bool IsEnabledReloadBatchPlans()
        {
            return true;
        }

        public const string ReloadBPAndResumeACIdentifier = "ReloadBPAndResume";
        [ACMethodInfo("", "en{'Reload batch plans'}de{'Batchpläne nachladen'}", 299, false)]
        public virtual void ReloadBPAndResume()
        {
            var query = FindChildComponents<PWNodeProcessWorkflowVB>(c => c is PWNodeProcessWorkflowVB);
            query.ForEach(c => c.ReloadBatchPlans());
            var stoppingNodes = query.Where(c => c.CurrentACState == ACStateEnum.SMStopping);
            if (stoppingNodes.Any())
            {
                stoppingNodes.ToList().ForEach(c => c.Resume());
            }
        }


#endregion

#region Planning and Testing
        protected override TimeSpan GetPlannedDuration()
        {
            return base.GetPlannedDuration();
        }

        protected override DateTime GetPlannedStartTime()
        {
            return base.GetPlannedStartTime();
        }
#endregion
#endregion

    }
}
