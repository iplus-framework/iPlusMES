using gip.core.autocomponent;
using gip.core.datamodel;
using vd = gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using System.Data.Objects;
using System.Data.Common.CommandTrees;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'PABatchPlanScheduler'}de{'PABatchPlanScheduler'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.Required, false, false)]
    public class PABatchPlanScheduler : PAJobScheduler
    {
        #region c'tors
        public static string ClassName = @"PABatchPlanScheduler";

        public PABatchPlanScheduler(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool baseACInit = base.ACInit(startChildMode);

            _ProdOrderManager = ACProdOrderManager.ACRefToServiceInstance(this);
            if (_ProdOrderManager == null)
                throw new Exception("ProdOrderManager not configured");

            using (ACMonitor.Lock(_20015_LockValue))
            {
                _DelegateQueue = new ACDelegateQueue(ACIdentifier);
            }
            _DelegateQueue.StartWorkerThread();

            return baseACInit;
        }

        public override bool ACPostInit()
        {
            InitScheduleListForPWNodes();
            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            _DelegateQueue.StopWorkerThread();
            using (ACMonitor.Lock(_20015_LockValue))
            {
                _DelegateQueue = null;
            }

            ACProdOrderManager.DetachACRefFromServiceInstance(this, _ProdOrderManager);
            _ProdOrderManager = null;
            return base.ACDeInit(deleteACClassTask);
        }

        private void InitScheduleListForPWNodes()
        {
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                if (SchedulesForPWNodes.ValueT == null)
                {
                    PAScheduleForPWNodeList newScheduleList = CreateScheduleListForPWNodes(this, databaseApp, null);
                    using (ACMonitor.Lock(_20015_LockValue))
                    {
                        foreach (var item in newScheduleList)
                        {
                            item.StartMode = GlobalApp.BatchPlanStartModeEnum.Off;
                        }
                    }
                    SchedulesForPWNodes.ValueT = newScheduleList;
                }
                else
                {
                    PAScheduleForPWNodeList additionalElements = CreateScheduleListForPWNodes(this, databaseApp, SchedulesForPWNodes.ValueT.Select(c => c.MDSchedulingGroupID).ToArray());
                    if (additionalElements.Any())
                    {
                        PAScheduleForPWNodeList scheduleList = SchedulesForPWNodes.ValueT;
                        foreach (PAScheduleForPWNode newElement in additionalElements)
                            scheduleList.Add(newElement);
                        // Force Broadcast
                        (SchedulesForPWNodes as IACPropertyNetServer).ChangeValueServer(scheduleList, true);
                    }
                }
            }
        }

        #endregion

        #region Properties

        #region Properties -> Manager

        private Guid? _AppDefManagerID;
        protected Guid AppDefManagerID
        {
            get
            {
                if (_AppDefManagerID.HasValue)
                    return _AppDefManagerID.Value;
                _AppDefManagerID = this.ApplicationManager.ComponentClass.BasedOnACClassID;
                return _AppDefManagerID.Value;
            }
        }

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

        #endregion

        #region Properties -> Queue

        private ACDelegateQueue _DelegateQueue = null;
        public ACDelegateQueue DelegateQueue
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _DelegateQueue;
                }
            }
        }

        #endregion

        #region Properties -> ACPropertyBindingSource

        [ACPropertyBindingSource(730, "Error", "en{'Scheduling-Alarm'}de{'Zeitplanungs-Alarm'}", "", false, false)]
        public IACContainerTNet<PANotifyState> IsSchedulingAlarm { get; set; }

        public const string PN_SchedulesForPWNodes = "SchedulesForPWNodes";
        [ACPropertyBindingSource(731, "StartModeConfiguration", "en{'Schedules for WF-Batch-Manager'}de{'Zeitpläne für WF-Batch-Manager'}", "", true, true)]
        public IACContainerTNet<PAScheduleForPWNodeList> SchedulesForPWNodes { get; set; }

        #endregion

        #endregion

        #region Precompiled Queries
        protected static readonly Func<DatabaseApp, Guid, Guid, IQueryable<ProdOrderBatchPlan>> s_cQry_ReadyBatchPlansForPWNode =
        CompiledQuery.Compile<DatabaseApp, Guid, Guid, IQueryable<ProdOrderBatchPlan>>(
                (ctx, mdSchedulingGroupID, appDefManagerID) => ctx.ProdOrderBatchPlan
                                    .Where(c => c.ProdOrderPartslist.MDProdOrderState.MDProdOrderStateIndex <= (short)MDProdOrderState.ProdOrderStates.ProdFinished
                                                && c.ProdOrderPartslist.ProdOrder.MDProdOrderState.MDProdOrderStateIndex <= (short)MDProdOrderState.ProdOrderStates.ProdFinished
                                                && c.VBiACClassWF.MDSchedulingGroupWF_VBiACClassWF.Any(x => x.MDSchedulingGroupID == mdSchedulingGroupID)
                                                && c.PlanStateIndex == (short)GlobalApp.BatchPlanState.ReadyToStart
                                                && c.VBiACClassWF.ACClassMethod.ACClassID == appDefManagerID)
                                    .OrderBy(c => c.ScheduledOrder ?? 0)
            );


        #endregion

        #region Methods

        #region Processing schedules

        protected override void RunJob(DateTime now, DateTime lastRun, DateTime nextRun)
        {
            if (SchedulesForPWNodes == null || SchedulesForPWNodes.ValueT == null)
                return;
            PAScheduleForPWNode[] activatedSchedules = SchedulesForPWNodes.ValueT.ToArray();
            if (!activatedSchedules.Any())
                return;

            using (ACMonitor.Lock(_20015_LockValue))
            {
                activatedSchedules = activatedSchedules.Where(c => c.StartMode != vd.GlobalApp.BatchPlanStartModeEnum.Off).ToArray();
            }
            if (!activatedSchedules.Any())
                return;

            // Prevent unnecessary cyclic processing in a short time distance by comparing the last processing time
            DateTime dtNow = DateTime.Now;
            using (ACMonitor.Lock(_20015_LockValue))
            {
                if (!activatedSchedules
                    .Where(c => !c.LastProcessingTime.HasValue
                               || (dtNow - c.LastProcessingTime.Value).TotalSeconds > 20)
                    .Any())
                    return;
            }

            // Process activated schedules
            DelegateQueue.Add(() => { ProcessActivatedSchedules(activatedSchedules); });
        }

        protected void ProcessActivatedSchedules(PAScheduleForPWNode[] activatedSchedules)
        {
            try
            {
                using (Database db = new Database())
                using (DatabaseApp dbApp = new DatabaseApp(db))
                {
                    var queryLoadedInstances = this.ApplicationManager.ACCompTypeDict.GetComponentsOfType<PWNodeProcessWorkflowVB>(true);
                    if (queryLoadedInstances != null && queryLoadedInstances.Any())
                        queryLoadedInstances = queryLoadedInstances.Where(c => c.InitState == ACInitState.Initialized);
                    foreach (PAScheduleForPWNode scheduleForPWNode in activatedSchedules)
                    {
                        GlobalApp.BatchPlanStartModeEnum startMode;
                        using (ACMonitor.Lock(_20015_LockValue))
                        {
                            startMode = scheduleForPWNode.StartMode;
                        }
                        Guid[] acclassWfIds = dbApp.MDSchedulingGroupWF.Where(c => c.MDSchedulingGroupID == scheduleForPWNode.MDSchedulingGroupID).Select(c => c.VBiACClassWFID).Distinct().ToArray();
                        IEnumerable<PWNodeProcessWorkflowVB> instancesOfThisSchedule = queryLoadedInstances != null
                            ? queryLoadedInstances.Where(c => acclassWfIds.Contains(c.ContentACClassWF.ACClassWFID))
                            : new PWNodeProcessWorkflowVB[] { };
                        List<ProdOrderBatchPlan> startableBatchPlans = new List<ProdOrderBatchPlan>();

                        if (startMode == GlobalApp.BatchPlanStartModeEnum.AutoTime)
                        {
                            IQueryable<ProdOrderBatchPlan> readyBatchPlans = s_cQry_ReadyBatchPlansForPWNode(dbApp, scheduleForPWNode.MDSchedulingGroupID, AppDefManagerID);
                            startableBatchPlans = readyBatchPlans.Where(c => (c.ScheduledStartDate != null ? (c.ScheduledStartDate ?? DateTime.Now) : (c.CalculatedStartDate ?? DateTime.Now)) <= DateTime.Now).ToList();
                        }
                        else
                        {
                            if (instancesOfThisSchedule.Any())
                            {
                                // If there are any active nodes, that are not completed, than next Batchplans must wait
                                if (instancesOfThisSchedule.Where(c =>    (c.CurrentACState >= ACStateEnum.SMRunning && c.CurrentACState < ACStateEnum.SMCompleted)
                                                                       || (c.CurrentACState == ACStateEnum.SMStarting && c.IterationCount.ValueT <= 0 && !c.SkipWaitingNodes))
                                                            .Any())
                                    continue;
                            }
                            IQueryable<ProdOrderBatchPlan> readyBatchPlans = s_cQry_ReadyBatchPlansForPWNode(dbApp, scheduleForPWNode.MDSchedulingGroupID, AppDefManagerID);
                            if (!readyBatchPlans.Any())
                                continue;
                            ProdOrderBatchPlan nextBatchPlan = null;
                            if (startMode == GlobalApp.BatchPlanStartModeEnum.SemiAutomatic)
                            {
                                nextBatchPlan = readyBatchPlans.Where(c => c.PartialTargetCount.HasValue
                                                                            && c.PartialTargetCount > 0
                                                                            && (!c.PartialActualCount.HasValue || c.PartialActualCount == 0)).FirstOrDefault();
                                if (nextBatchPlan == null)
                                {
                                    nextBatchPlan = readyBatchPlans.Where(c => c.PartialTargetCount.HasValue
                                                                            && c.PartialTargetCount > 0
                                                                            && (c.PartialActualCount.HasValue || c.PartialTargetCount == c.PartialTargetCount)).FirstOrDefault();
                                    if (nextBatchPlan != null)
                                    {
                                        PWNodeProcessWorkflowVB activeInstanceForBatchplan = instancesOfThisSchedule.Where(c => c.CurrentProdOrderPartslist.ProdOrderPartslistID == nextBatchPlan.ProdOrderPartslistID).FirstOrDefault();
                                        if (activeInstanceForBatchplan == null)
                                        {
                                            if (nextBatchPlan.BatchActualCount >= nextBatchPlan.BatchTargetCount)
                                                nextBatchPlan.PlanState = GlobalApp.BatchPlanState.Completed;
                                            else
                                                nextBatchPlan.PlanState = GlobalApp.BatchPlanState.Paused;
                                            nextBatchPlan.PartialTargetCount = null;
                                            nextBatchPlan.PartialActualCount = null;
                                        }
                                        nextBatchPlan = null;
                                        dbApp.ACSaveChanges();
                                        scheduleForPWNode.IncrementRefreshCounter();
                                        // Force Broadcast
                                        (SchedulesForPWNodes as IACPropertyNetServer).ChangeValueServer(SchedulesForPWNodes.ValueT, true);
                                    }
                                }
                            }
                            else // if (startMode == AutoSequential || startMode == AutoTimeAndSequential)
                            {
                                nextBatchPlan = readyBatchPlans.FirstOrDefault();
                                // If Scheduled date not reached, then wait
                                if (startMode == GlobalApp.BatchPlanStartModeEnum.AutoTimeAndSequential
                                    && !((nextBatchPlan.ScheduledStartDate != null ? (nextBatchPlan.ScheduledStartDate ?? DateTime.Now) : (nextBatchPlan.CalculatedStartDate ?? DateTime.Now)) <= DateTime.Now)
                                    )
                                    continue;
                            }
                            if (nextBatchPlan == null)
                                continue;
                            startableBatchPlans.Add(nextBatchPlan);
                        }

                        foreach (ProdOrderBatchPlan startableBatchPlan in startableBatchPlans)
                        {
                            PWNodeProcessWorkflowVB activeInstanceForBatchplan = instancesOfThisSchedule.Where(c => c.CurrentProdOrderPartslist.ProdOrderPartslistID == startableBatchPlan.ProdOrderPartslistID).FirstOrDefault();
                            //if (!startableBatchPlan.IsValidated)
                            //{
                            //    if (activeInstanceForBatchplan == null)
                            //    {
                            //        // MEssage for user
                            //        continue;
                            //    }
                            //}
                            Msg saveMessage = null;
                            // If Workflownode already running for this production order, then only activate Batchplan
                            // because PWNodeProcessWorkflowVB will start it itself.
                            if (activeInstanceForBatchplan != null)
                            {
                                startableBatchPlan.PlanState = GlobalApp.BatchPlanState.AutoStart;
                                saveMessage = dbApp.ACSaveChanges();
                                if (saveMessage == null)
                                {
                                    if (activeInstanceForBatchplan.CurrentACState == ACStateEnum.SMStopping)
                                    {
                                        activeInstanceForBatchplan.Resume();
                                    }
                                    else if (  activeInstanceForBatchplan.CurrentACState == ACStateEnum.SMStarting
                                            && activeInstanceForBatchplan.IsInPlanningWaitNotElapsed)
                                    {
                                        activeInstanceForBatchplan.ResetPlanningWait();
                                    }
                                }
                            }
                            // New workflow must be instantiated
                            else
                            {
                                Guid acclassMethodID = startableBatchPlan.MaterialWFACClassMethod.ACClassMethodID;
                                core.datamodel.ACClassMethod aCClassMethod = null;
                                using (ACMonitor.Lock(dbApp.ContextIPlus.QueryLock_1X000))
                                {
                                    aCClassMethod = dbApp.ContextIPlus.ACClassMethod.Where(c => c.ACClassMethodID == acclassMethodID).FirstOrDefault();
                                }
                                startableBatchPlan.PlanState = GlobalApp.BatchPlanState.AutoStart;
                                saveMessage = dbApp.ACSaveChanges();
                                if (saveMessage == null)
                                    saveMessage = ProdOrderManager.StartBatchPlan(this.ApplicationManager, dbApp, aCClassMethod, startableBatchPlan.VBiACClassWF, startableBatchPlan, false);
                            }

                            if (saveMessage != null)
                            {
                                // Message / Alarm
                            }
                        }
                    }

                    DateTime dtNow = DateTime.Now;
                    using (ACMonitor.Lock(_20015_LockValue))
                    {
                        foreach (PAScheduleForPWNode scheduleForPWNode in activatedSchedules)
                        {
                            scheduleForPWNode.LastProcessingTime = dtNow;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
                if (e.InnerException != null && e.InnerException.Message != null)
                    message += " Inner:" + e.InnerException.Message;
                IsSchedulingAlarm.ValueT = PANotifyState.AlarmOrFault;
                Msg msg = new Msg(message, this, eMsgLevel.Exception, "PABatchPlanScheduler", "ProcessActivatedSchedules", 100);
                if (IsAlarmActive(IsSchedulingAlarm, message) == null)
                    Messages.LogException(this.GetACUrl(), "ProcessActivatedSchedules(100)", e);
                OnNewAlarmOccurred(IsSchedulingAlarm, msg, true);
            }
        }

        internal void OnACStateChangedOfPWNode(PWNodeProcessWorkflowVB pwNode)
        {
            if (pwNode == null || pwNode.ContentACClassWF == null)
                return;
            Guid acClassWFID = pwNode.ContentACClassWF.ACClassWFID;
            ACStateEnum changedACState = pwNode.CurrentACState;
            if (changedACState == ACStateEnum.SMIdle
                || changedACState == ACStateEnum.SMPaused
                || changedACState == ACStateEnum.SMRunning
                || changedACState == ACStateEnum.SMStopping)
            {
                if (SchedulesForPWNodes == null || SchedulesForPWNodes.ValueT == null || pwNode.MDSchedulingGroup == null)
                    return;
                PAScheduleForPWNode scheduleForPWNode = SchedulesForPWNodes.ValueT.Where(c => c.MDSchedulingGroupID == pwNode.MDSchedulingGroup.MDSchedulingGroupID).FirstOrDefault();
                if (scheduleForPWNode == null)
                    return;
                scheduleForPWNode.IncrementRefreshCounter();
                // Force Broadcast
                (SchedulesForPWNodes as IACPropertyNetServer).ChangeValueServer(SchedulesForPWNodes.ValueT, true);

                // TODO: Refresh, wenn Ist-Batchzaehler sich erhöht hat
                // TODO: Bei Idle nicht nochmals senden wenn zuvor Stopping gesendet wurde
                if (changedACState == ACStateEnum.SMStopping)
                {
                    DelegateQueue.Add(() => { ProcessActivatedSchedules(new PAScheduleForPWNode[] { scheduleForPWNode }); });
                }
            }
        }
        #endregion

        #region Client-Methods

        public const string MN_UpdateScheduleFromClient = "UpdateScheduleFromClient";
        [ACMethodInfo("UpdateScheduleFromClient", "en{'UpdateScheduleFromClient'}de{'UpdateScheduleFromClient'}", 9999)]
        public Msg UpdateScheduleFromClient(PAScheduleForPWNode changedScheduleNode)
        {
            Msg msg = null;
            if (SchedulesForPWNodes == null || SchedulesForPWNodes.ValueT == null)
            {
                msg = new Msg("UpdateScheduleFromClient_errConfigNotLoaded", this, eMsgLevel.Error, PABatchPlanScheduler.ClassName, "UpdateScheduleFromClient()", 10);
                if (IsAlarmActive(SchedulesForPWNodes, msg.Message) == null)
                    Messages.LogMessageMsg(msg);
                OnNewAlarmOccurred(SchedulesForPWNodes, msg);
                return msg;
            }

            PAScheduleForPWNodeList scheduleList = SchedulesForPWNodes.ValueT;
            if (scheduleList == null)
            {
                InitScheduleListForPWNodes();
                scheduleList = SchedulesForPWNodes.ValueT;
            }
            if (scheduleList == null)
            {
                msg = new Msg("SchedulesForPWNodes is null", this, eMsgLevel.Error, PABatchPlanScheduler.ClassName, "UpdateScheduleFromClient()", 20);
                if (IsAlarmActive(SchedulesForPWNodes, msg.Message) == null)
                    Messages.LogMessageMsg(msg);
                OnNewAlarmOccurred(SchedulesForPWNodes, msg);
                return msg;
            }
            PAScheduleForPWNode item = scheduleList.FirstOrDefault(c => c.MDSchedulingGroupID == changedScheduleNode.MDSchedulingGroupID);
            if (item == null)
            {
                InitScheduleListForPWNodes();
                scheduleList = SchedulesForPWNodes.ValueT;
                item = scheduleList.FirstOrDefault(c => c.MDSchedulingGroupID == changedScheduleNode.MDSchedulingGroupID);
            }
            if (item == null)
            {
                msg = new Msg("Item not found", this, eMsgLevel.Error, PABatchPlanScheduler.ClassName, "UpdateScheduleFromClient()", 20);
                if (IsAlarmActive(SchedulesForPWNodes, msg.Message) == null)
                    Messages.LogMessageMsg(msg);
                OnNewAlarmOccurred(SchedulesForPWNodes, msg);
                return msg;
            }
            if (item.StartMode != changedScheduleNode.StartMode
                || item.RefreshCounter != changedScheduleNode.RefreshCounter)
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    item.UpdateAndMaintainRefreshCounter(changedScheduleNode);
                    item.StartMode = changedScheduleNode.StartMode;
                    item.UpdateName = changedScheduleNode.UpdateName;
                    item.UpdateTime = DateTime.Now;
                }
                // Force Broadcast
                (SchedulesForPWNodes as IACPropertyNetServer).ChangeValueServer(scheduleList, true);

                if (item.StartMode != GlobalApp.BatchPlanStartModeEnum.Off)
                    DelegateQueue.Add(() => { ProcessActivatedSchedules(new PAScheduleForPWNode[] { item }); });
            }
            return null;
        }

        #endregion

        #region Static

        public static PAScheduleForPWNodeList CreateScheduleListForPWNodes(ACComponent invoker, DatabaseApp databaseApp, Guid[] ignoreACClassWFIds)
        {
            List<MDSchedulingGroup> allPWNodes =
                databaseApp
                .MDSchedulingGroup
                .AsEnumerable()
                .OrderBy(c => c.ACCaption)
                .ToList();
            if (ignoreACClassWFIds != null && ignoreACClassWFIds.Any())
                allPWNodes = allPWNodes.Where(c => !ignoreACClassWFIds.Contains(c.MDSchedulingGroupID)).ToList();
            PAScheduleForPWNodeList list = new PAScheduleForPWNodeList(allPWNodes.Select(c => new PAScheduleForPWNode()
            {
                MDSchedulingGroupID = c.MDSchedulingGroupID,
                MDSchedulingGroup = c,
                StartMode = vd.GlobalApp.BatchPlanStartModeEnum.Off,
                UpdateName = invoker.Root.Environment.User.Initials,
                UpdateTime = DateTime.Now
            }));
            return list;
        }

        

        #endregion

        #region Alarm & HandleExecuteACMethod

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case MN_UpdateScheduleFromClient:
                    if (acParameter.Length == 1)
                        result = UpdateScheduleFromClient(acParameter[0] as PAScheduleForPWNode);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override void AcknowledgeAlarms()
        {
            if (!IsEnabledAcknowledgeAlarms())
                return;
            if (IsSchedulingAlarm.ValueT == PANotifyState.AlarmOrFault)
            {
                base.AcknowledgeAlarms();
                IsSchedulingAlarm.ValueT = PANotifyState.Off;
                OnAlarmDisappeared(IsSchedulingAlarm);
            }
        }

        #endregion

        #endregion

        
    }


}
