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
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Scheduler for Picking orders'}de{'Zeitplaner für Kommissionieraufträge'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.Required, false, false)]
    public class PAPickingScheduler : PAWorkflowSchedulerBase
    {
        #region c'tors
        public PAPickingScheduler(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool baseACInit = base.ACInit(startChildMode);

            _PickingManager = ACPickingManager.ACRefToServiceInstance(this);
            if (_PickingManager == null)
                throw new Exception("PickingManager not configured");
            return baseACInit;
        }

        public override bool ACPostInit()
        {
            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {

            ACPickingManager.DetachACRefFromServiceInstance(this, _PickingManager);
            _PickingManager = null;
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties

        #region Properties -> Manager

        protected ACRef<ACPickingManager> _PickingManager = null;
        public ACPickingManager PickingManager
        {
            get
            {
                if (_PickingManager == null)
                    return null;
                return _PickingManager.ValueT;
            }
        }

        #endregion        

        #endregion

        #region Precompiled Queries
        protected static readonly Func<DatabaseApp, Guid, Guid, IQueryable<Picking>> s_cQry_ReadyPickingsForPWNode =
        CompiledQuery.Compile<DatabaseApp, Guid, Guid, IQueryable<Picking>>(
                (ctx, mdSchedulingGroupID, appDefManagerID) => ctx.Picking
                                    .Where(c => c.ACClassMethodID.HasValue
                                                && c.PickingStateIndex == (short)PickingStateEnum.WFReadyToStart
                                                && c.PickingPos_Picking.Any(x => x.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad)
                                                && c.VBiACClassWFID.HasValue
                                                && c.VBiACClassWF.ACClassMethod.ACClassID == appDefManagerID
                                                && c.VBiACClassWF.MDSchedulingGroupWF_VBiACClassWF.Any(x => x.MDSchedulingGroupID == mdSchedulingGroupID))
                                    .OrderBy(c => c.ScheduledOrder ?? 0)
            );


        #endregion

        #region Methods

        #region Processing schedules

        protected override void OnProcessActivatedSchedule(DatabaseApp dbApp, PAScheduleForPWNode scheduleForPWNode, BatchPlanStartModeEnum startMode, Guid[] acclassWfIds, IEnumerable<PWNodeProcessWorkflowVB> instancesOfThisSchedule)
        {
            List<Picking> startablePickings = new List<Picking>();

            if (startMode == BatchPlanStartModeEnum.AutoTime)
            {
                IQueryable<Picking> readyPickings = s_cQry_ReadyPickingsForPWNode(dbApp, scheduleForPWNode.MDSchedulingGroupID, AppDefManagerID);
                DateTime dateTimeIfNull = DateTime.Now.AddDays(1);
                startablePickings = readyPickings.Where(c => (c.ScheduledStartDate ?? dateTimeIfNull) <= DateTime.Now).ToArray().Distinct().ToList();
            }
            else // if (startMode == AutoSequential || startMode == AutoTimeAndSequential || BatchPlanStartModeEnum.SemiAutomatic)
            {
                bool canStartMultiple = scheduleForPWNode.MDSchedulingGroup?.MDKey[0] == '+';
                if (!canStartMultiple && instancesOfThisSchedule.Any())
                {
                    // If there are any active nodes, that are not completed, than next Pickings must wait
                    if (instancesOfThisSchedule.Where(c => (c.CurrentACState >= ACStateEnum.SMRunning && c.CurrentACState < ACStateEnum.SMCompleted)
                                                            || (c.CurrentACState == ACStateEnum.SMStarting && c.IterationCount.ValueT <= 0 && !c.SkipWaitingNodes))
                                                .Any())
                        return;
                }
                IQueryable<Picking> readyPickings = s_cQry_ReadyPickingsForPWNode(dbApp, scheduleForPWNode.MDSchedulingGroupID, AppDefManagerID);
                if (!readyPickings.Any())
                    return;
                Picking nextPicking = null;
                nextPicking = readyPickings.FirstOrDefault();
                // If Scheduled date not reached, then wait
                if (startMode == BatchPlanStartModeEnum.AutoTimeAndSequential
                    && !((nextPicking.ScheduledStartDate ?? DateTime.Now) <= DateTime.Now)
                    )
                    return;
                if (nextPicking == null)
                    return;
                startablePickings.Add(nextPicking);
            }

            foreach (Picking startablePicking in startablePickings)
            {
                PWNodeProcessWorkflowVB activeInstanceForPicking = instancesOfThisSchedule.Where(c => c.CurrentPicking != null
                                                                                                    && c.CurrentPicking.PickingID == startablePicking.PickingID)
                                                                                            .FirstOrDefault();
                //if (!startablePicking.IsValidated)
                //{
                //    if (activeInstanceForPicking == null)
                //    {
                //        // MEssage for user
                //        continue;
                //    }
                //}
                Msg saveMessage = null;
                // If Workflownode already running for this production order, then only activate Picking
                // because PWNodeProcessWorkflowVB will start it itself.
                if (activeInstanceForPicking != null)
                {
                    startablePicking.PickingState = PickingStateEnum.WFActive;
                    saveMessage = dbApp.ACSaveChanges();
                    if (saveMessage == null)
                    {
                        if (activeInstanceForPicking.CurrentACState == ACStateEnum.SMStopping)
                        {
                            activeInstanceForPicking.Resume();
                        }
                        else if (activeInstanceForPicking.CurrentACState == ACStateEnum.SMStarting
                                && activeInstanceForPicking.IsInPlanningWaitNotElapsed)
                        {
                            activeInstanceForPicking.ResetPlanningWait();
                        }
                    }
                }
                // New workflow must be instantiated
                else
                {
                    Guid acclassMethodID = startablePicking.ACClassMethodID.Value;
                    core.datamodel.ACClassMethod aCClassMethod = null;
                    using (ACMonitor.Lock(dbApp.ContextIPlus.QueryLock_1X000))
                    {
                        aCClassMethod = dbApp.ContextIPlus.ACClassMethod.Where(c => c.ACClassMethodID == acclassMethodID).FirstOrDefault();
                    }
                    saveMessage = PickingManager.StartPicking(this.ApplicationManager, dbApp, aCClassMethod, startablePicking.VBiACClassWF, startablePicking, false);
                }

                if (saveMessage != null)
                {
                    // Message / Alarm
                }
            }
        }
        #endregion

        #region Client-Methods
        #endregion

        #region Static
        #endregion

        #region Alarm & HandleExecuteACMethod

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            //result = null;
            //switch (acMethodName)
            //{
            //    case MN_UpdateScheduleFromClient:
            //        if (acParameter.Length == 1)
            //            result = UpdateScheduleFromClient(acParameter[0] as PAScheduleForPWNode);
            //        return true;
            //    case nameof(ResetAllSchedules):
            //        ResetAllSchedules();
            //        return true;
            //}
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        #endregion


    }


}
