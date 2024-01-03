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
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Base class for Workflow-Scheduler'}de{'Basisklasse für Worfklow-Scheduler'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.Required, false, false)]
    public abstract class PAWorkflowSchedulerBase : PAJobScheduler
    {
        #region c'tors
        public PAWorkflowSchedulerBase(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _GroupIndexFrom = new ACPropertyConfigValue<short>(this, nameof(GroupIndexFrom), 0);
            _GroupIndexTo = new ACPropertyConfigValue<short>(this, nameof(GroupIndexTo), 4999);
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool baseACInit = base.ACInit(startChildMode);
            _ = GroupIndexFrom;
            _ = GroupIndexTo;

            using (ACMonitor.Lock(_20015_LockValue))
            {
                _DelegateQueue = new ACDelegateQueue(this.GetACUrl());
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
            return base.ACDeInit(deleteACClassTask);
        }

        private void InitScheduleListForPWNodes()
        {
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                if (SchedulesForPWNodes.ValueT == null)
                {
                    PAScheduleForPWNodeList newScheduleList = CreateScheduleListForPWNodes(this, databaseApp, null, GroupIndexFrom, GroupIndexTo);
                    using (ACMonitor.Lock(_20015_LockValue))
                    {
                        foreach (var item in newScheduleList)
                        {
                            item.StartMode = BatchPlanStartModeEnum.Off;
                        }
                    }
                    SchedulesForPWNodes.ValueT = newScheduleList;
                }
                else
                {
                    PAScheduleForPWNodeList additionalElements = CreateScheduleListForPWNodes(this, databaseApp, SchedulesForPWNodes.ValueT.ToArray(), GroupIndexFrom, GroupIndexTo);
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

        #region Config
        protected ACPropertyConfigValue<short> _GroupIndexFrom;
        [ACPropertyConfig("en{'Filter MDSchedulingGroupIndex FROM'}de{'Filter MDSchedulingGroupIndex VON'}")]
        public short GroupIndexFrom
        {
            get
            {
                return _GroupIndexFrom.ValueT;
            }
        }

        protected ACPropertyConfigValue<short> _GroupIndexTo;
        [ACPropertyConfig("en{'Filter MDSchedulingGroupIndex TO'}de{'Filter MDSchedulingGroupIndex BIS'}")]
        public short GroupIndexTo
        {
            get
            {
                return _GroupIndexTo.ValueT;
            }
        }
        #endregion

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
                activatedSchedules = activatedSchedules.Where(c => c.StartMode != vd.BatchPlanStartModeEnum.Off).ToArray();
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
                        BatchPlanStartModeEnum startMode;
                        using (ACMonitor.Lock(_20015_LockValue))
                        {
                            startMode = scheduleForPWNode.StartMode;
                        }
                        Guid[] acclassWfIds = dbApp.MDSchedulingGroupWF.Where(c => c.MDSchedulingGroupID == scheduleForPWNode.MDSchedulingGroupID).Select(c => c.VBiACClassWFID).Distinct().ToArray();
                        IEnumerable<PWNodeProcessWorkflowVB> instancesOfThisSchedule = queryLoadedInstances != null
                            ? queryLoadedInstances.Where(c => acclassWfIds.Contains(c.ContentACClassWF.ACClassWFID))
                            : new PWNodeProcessWorkflowVB[] { };

                        OnProcessActivatedSchedule(dbApp, scheduleForPWNode, startMode, acclassWfIds, instancesOfThisSchedule);
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
                IsSchedulingAlarm.ValueT = PANotifyState.AlarmOrFault;
                Msg msg = new Msg(e.Message, this, eMsgLevel.Exception, "PAWFNodeSchedulerBase", "ProcessActivatedSchedules", 100);
                if (IsAlarmActive(IsSchedulingAlarm, e.Message) == null)
                    Messages.LogException(this.GetACUrl(), "ProcessActivatedSchedules(100)", e, true);
                OnNewAlarmOccurred(IsSchedulingAlarm, msg, true);
            }
        }

        protected abstract void OnProcessActivatedSchedule(DatabaseApp dbApp, PAScheduleForPWNode scheduleForPWNode, BatchPlanStartModeEnum startMode, Guid[] acclassWfIds, IEnumerable<PWNodeProcessWorkflowVB> instancesOfThisSchedule);

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
                msg = new Msg("UpdateScheduleFromClient_errConfigNotLoaded", this, eMsgLevel.Error, nameof(PAWorkflowSchedulerBase), "UpdateScheduleFromClient()", 10);
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
                msg = new Msg("SchedulesForPWNodes is null", this, eMsgLevel.Error, nameof(PAWorkflowSchedulerBase), "UpdateScheduleFromClient()", 20);
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
                msg = new Msg("Item not found", this, eMsgLevel.Error, nameof(PAWorkflowSchedulerBase), "UpdateScheduleFromClient()", 20);
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
                    if (item.StartMode != changedScheduleNode.StartMode)
                    {
                        string schedulingGroupName = item.MDSchedulingGroup != null ? item.MDSchedulingGroup.MDSchedulingGroupName : item.MDSchedulingGroupID.ToString();
                        string modeChangeMessage = $"Startmode changed from {item.StartMode} to {changedScheduleNode.StartMode} at {schedulingGroupName} by {changedScheduleNode.UpdateName}";
                        Messages.LogDebug(
                            this.GetACUrl(),
                            "UpdateScheduleFromClient(10)", modeChangeMessage);
                        item.StartMode = changedScheduleNode.StartMode;
                    }
                    item.UpdateName = changedScheduleNode.UpdateName;
                    item.UpdateTime = DateTime.Now;
                }
                // Force Broadcast
                (SchedulesForPWNodes as IACPropertyNetServer).ChangeValueServer(scheduleList, true);

                if (item.StartMode != BatchPlanStartModeEnum.Off)
                    DelegateQueue.Add(() => { ProcessActivatedSchedules(new PAScheduleForPWNode[] { item }); });
            }
            return null;
        }


        [ACMethodInteraction("", "en{'Reset schedules'}de{'Zeitpläne zurücksetzen'}", 700, true, "")]
        public void ResetAllSchedules()
        {
            if (!Root.Environment.User.IsSuperuser)
                return;
            SchedulesForPWNodes.ValueT = null;
            InitScheduleListForPWNodes();
        }
        #endregion

        #region Static

        public static PAScheduleForPWNodeList CreateScheduleListForPWNodes(ACComponent invoker, DatabaseApp databaseApp, IEnumerable<PAScheduleForPWNode> ignoreACClassWFs, short mdSchedulingGroupIndexFrom, short mdSchedulingGroupIndexTo)
        {
            List<MDSchedulingGroup> allPWNodes =
                databaseApp
                .MDSchedulingGroup
                .Where(c => c.MDSchedulingGroupIndex >= mdSchedulingGroupIndexFrom && c.MDSchedulingGroupIndex <= mdSchedulingGroupIndexTo)
                .AsEnumerable()
                .OrderBy(c => c.ACCaption)
                .ToList();
            if (ignoreACClassWFs != null && ignoreACClassWFs.Any())
            {
                foreach (var node in ignoreACClassWFs)
                {
                    node.MDSchedulingGroup = allPWNodes.Where(c => c.MDSchedulingGroupID == node.MDSchedulingGroupID).FirstOrDefault();
                }
                var ignoreACClassWFIds = ignoreACClassWFs.Select(c => c.MDSchedulingGroupID).ToArray();
                allPWNodes = allPWNodes.Where(c => !ignoreACClassWFIds.Contains(c.MDSchedulingGroupID)).ToList();
            }
            PAScheduleForPWNodeList list = new PAScheduleForPWNodeList(allPWNodes.Select(c => new PAScheduleForPWNode()
            {
                MDSchedulingGroupID = c.MDSchedulingGroupID,
                MDSchedulingGroup = c,
                StartMode = vd.BatchPlanStartModeEnum.Off,
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
                case nameof(ResetAllSchedules):
                    ResetAllSchedules();
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
                IsSchedulingAlarm.ValueT = PANotifyState.Off;
                OnAlarmDisappeared(IsSchedulingAlarm);
            }
            base.AcknowledgeAlarms();
        }

        #endregion

        #endregion


    }


}
