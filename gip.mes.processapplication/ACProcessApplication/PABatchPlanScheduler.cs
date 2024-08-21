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
            _RMIPoint = new ACPointAsyncRMI(this, nameof(RMIPoint), 1);
            _RMIPoint.SetMethod = OnSetInvocationPoint;
        }

        static PABatchPlanScheduler()
        {
            ACMethod.RegisterVirtualMethod(typeof(PABatchPlanScheduler), "RunRoutesCalculation", CreateVirtualRunRouteCalculationMethod("RunRouteCalculation", "en{'Route calculation'}de{'Route calculation'}", null));
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

        #region Propreties => RMI

        ACPointAsyncRMI _RMIPoint;
        [ACPropertyAsyncMethodPoint(9999)]
        public ACPointAsyncRMI RMIPoint
        {
            get
            {
                return _RMIPoint;
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

        #region Methods => Route calculation

        [ACMethodAsync("Send", "en{'RunRoutesCalculation'}de{'RunRoutesCalculation'}", 201, false)]
        public ACMethodEventArgs RunRoutesCalculation(ACMethod acMethod)
        {
            ACPointAsyncRMIWrap<ACComponent> currentAsyncRMI = RMIPoint.CurrentAsyncRMI;

            ApplicationManager.ApplicationQueue.Add(() => RoutesCalculation(currentAsyncRMI, acMethod));

            return new ACMethodEventArgs(acMethod, Global.ACMethodResultState.InProcess);
        }

        public void RoutesCalculation(ACPointAsyncRMIWrap<ACComponent> currentAsyncRMI, ACMethod acMethod)
        {
            RoutesCalculation();

            if (currentAsyncRMI != null && !currentAsyncRMI.CallbackIsPending)
            {
                // 2. Fill out the result parameters
                ACMethodEventArgs result = new ACMethodEventArgs(acMethod, Global.ACMethodResultState.Succeeded);
                result.GetACValue("CalculationFinished").Value = true;

                // 3. Invoke callback method of the invoker. 
                // If client has requested the asynchronous invocation was via network the callback will be done on the remote side at the client
                RMIPoint.InvokeCallbackDelegate(result);
            }
        }

        public void RoutesCalculation()
        {
            using (Database db = new core.datamodel.Database())
            using (DatabaseApp dbApp = new DatabaseApp(db))
            {
                var batchPlans = ProdOrderManager.GetProductionLinieBatchPlansWithPWNode(dbApp, GlobalApp.BatchPlanState.Created, GlobalApp.BatchPlanState.Paused, null, null, null, null, null, null, null);

                MsgWithDetails msg = new MsgWithDetails();
                List<FacilityReservationRoutes> routesResult = new List<FacilityReservationRoutes>();

                ConfigManagerIPlus configManager = ConfigManagerIPlus.GetServiceInstance(this);

                foreach (ProdOrderBatchPlan batchPlan in batchPlans)
                {
                    List<IACConfigStore> listOfSelectedStores = new List<IACConfigStore>() { batchPlan.ProdOrderPartslist, batchPlan.ProdOrderPartslist.Partslist, batchPlan.ProdOrderPartslist.Partslist.MaterialWF, 
                                                                                             batchPlan.IplusVBiACClassWF.ACClassMethod};
                    var result = ProdOrderManager.CalculatePossibleRoutes(dbApp, db, batchPlan, listOfSelectedStores, configManager, msg);
                    if (result != null && result.Any())
                    {
                        routesResult.AddRange(result);
                    }
                }
            }
        }

        #endregion

        #region RegisterVirtualMethods

        protected static ACMethodWrapper CreateVirtualRunRouteCalculationMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);

            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("RouteCalculation", typeof(bool), false, Global.ParamOption.Required));
            paramTranslation.Add("RouteCalculation", "en{'RouteCalculation'}de{'RouteCalculation'}");


            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            method.ResultValueList.Add(new ACValue("CalculationFinished", typeof(bool), true, Global.ParamOption.Required));
            resultTranslation.Add("CalculationFinished", "en{'CalculationFinished'}de{'CalculationFinished'}");

            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }

        #endregion

        #region Methods => RMI

        public bool OnSetInvocationPoint(IACPointNetBase point)
        {
            var query = ReferencePoint.ConnectionList.Where(c => c is IACContainerRef);
            // This delegate in invioked when the RMI-Point has got a new entry

            // VARIANT A:
            // DeQueueInvocationList() handles all new entries by calling ExampleMethodAsync() for each new entry
            RMIPoint.DeQueueInvocationList();

            //// VARIANT B:
            //// If you want to handle it on another way, then implement you own logic. 
            //foreach (var newEntry in RMIPoint.ConnectionList.Where(c => c.State == PointProcessingState.NewEntry))
            //{
            //    // Call ActivateAsyncRMI if you want to handle this entry. (ActivateAsyncRMI knows that your ExampleMethodAsync has to be invoked)
            //    RMIPoint.ActivateAsyncRMI(newEntry, true);
            //    // Attention: If you don't invoke all new entries, than you have to handle the other remaining entries in another cyclic thread
            //    // otherwise the requester will never get back a result!
            //}

            return true;
        }

        #endregion

        #endregion


    }
}
