using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using System.Data.Objects;
using System.Data.Common.CommandTrees;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Scheduler for Production orders'}de{'Zeitplaner für Produktionsaufträge'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.Required, false, false)]
    public class PABatchPlanScheduler : PAWorkflowSchedulerBase
    {
        #region c'tors
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
            return baseACInit;
        }

        public override bool ACPostInit()
        {
            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {

            ACProdOrderManager.DetachACRefFromServiceInstance(this, _ProdOrderManager);
            _ProdOrderManager = null;
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties

        #region Properties -> Manager

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

        protected override void OnProcessActivatedSchedule(DatabaseApp dbApp, PAScheduleForPWNode scheduleForPWNode, BatchPlanStartModeEnum startMode, Guid[] acclassWfIds, IEnumerable<PWNodeProcessWorkflowVB> instancesOfThisSchedule)
        {
            List<ProdOrderBatchPlan> startableBatchPlans = new List<ProdOrderBatchPlan>();

            if (startMode == BatchPlanStartModeEnum.AutoTime)
            {
                IQueryable<ProdOrderBatchPlan> readyBatchPlans = s_cQry_ReadyBatchPlansForPWNode(dbApp, scheduleForPWNode.MDSchedulingGroupID, AppDefManagerID);
                DateTime dateTimeIfNull = DateTime.Now.AddDays(1);
                startableBatchPlans = readyBatchPlans.Where(c => (c.ScheduledStartDate ?? dateTimeIfNull) <= DateTime.Now).ToList();
            }
            else
            {
                bool canStartMultiple = scheduleForPWNode.MDSchedulingGroup?.MDKey[0] == '+';
                if (!canStartMultiple && instancesOfThisSchedule.Any())
                {
                    // If there are any active nodes, that are not completed, than next Batchplans must wait
                    if (instancesOfThisSchedule.Where(c => (c.CurrentACState >= ACStateEnum.SMRunning && c.CurrentACState < ACStateEnum.SMCompleted)
                                                            || (c.CurrentACState == ACStateEnum.SMStarting && c.IterationCount.ValueT <= 0 && !c.SkipWaitingNodes))
                                                .Any())
                        return;
                }
                IQueryable<ProdOrderBatchPlan> readyBatchPlans = s_cQry_ReadyBatchPlansForPWNode(dbApp, scheduleForPWNode.MDSchedulingGroupID, AppDefManagerID);
                if (!readyBatchPlans.Any())
                    return;
                ProdOrderBatchPlan nextBatchPlan = null;
                if (startMode == BatchPlanStartModeEnum.SemiAutomatic)
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
                            PWNodeProcessWorkflowVB activeInstanceForBatchplan = instancesOfThisSchedule.Where(c => c.CurrentProdOrderPartslist != null
                                                                                                                && c.CurrentProdOrderPartslist.ProdOrderPartslistID == nextBatchPlan.ProdOrderPartslistID)
                                                                                                        .FirstOrDefault();
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
                    if (startMode == BatchPlanStartModeEnum.AutoTimeAndSequential
                        && !((nextBatchPlan.ScheduledStartDate ?? DateTime.Now) <= DateTime.Now)
                        )
                        return;
                }
                if (nextBatchPlan == null)
                    return;
                startableBatchPlans.Add(nextBatchPlan);
            }

            foreach (ProdOrderBatchPlan startableBatchPlan in startableBatchPlans)
            {
                PWNodeProcessWorkflowVB activeInstanceForBatchplan = instancesOfThisSchedule.Where(c => c.CurrentProdOrderPartslist != null
                                                                                                    && c.CurrentProdOrderPartslist.ProdOrderPartslistID == startableBatchPlan.ProdOrderPartslistID)
                                                                                            .FirstOrDefault();
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
                        else if (activeInstanceForBatchplan.CurrentACState == ACStateEnum.SMStarting
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
