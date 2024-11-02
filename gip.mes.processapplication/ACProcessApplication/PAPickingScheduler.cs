// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Scheduler for Picking orders'}de{'Zeitplaner für Kommissionieraufträge'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.Required, false, false)]
    public class PAPickingScheduler : PAWorkflowSchedulerBase
    {
        #region c'tors
        public PAPickingScheduler(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _RMIPoint = new ACPointAsyncRMI(this, nameof(RMIPoint), 1);
            _RMIPoint.SetMethod = OnSetInvocationPoint;
        }

        static PAPickingScheduler()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAPickingScheduler), "RunRoutesCalculation", CreateVirtualRunRouteCalculationMethod("RunRouteCalculation", "en{'Route calculation'}de{'Route calculation'}", null));
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
        protected static readonly Func<DatabaseApp, Guid, Guid, IEnumerable<Picking>> s_cQry_ReadyPickingsForPWNode =
        EF.CompileQuery<DatabaseApp, Guid, Guid, IEnumerable<Picking>>(
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
                IEnumerable<Picking> readyPickings = s_cQry_ReadyPickingsForPWNode(dbApp, scheduleForPWNode.MDSchedulingGroupID, AppDefManagerID);
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
                IEnumerable<Picking> readyPickings = s_cQry_ReadyPickingsForPWNode(dbApp, scheduleForPWNode.MDSchedulingGroupID, AppDefManagerID);
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

        #region Methods => Routes

        private DateTime _LastRunPossibleRoutesCheck = DateTime.Now;

        [ACMethodAsync("Send", "en{'RunRoutesCalculation'}de{'RunRoutesCalculation'}", 201, false)]
        public ACMethodEventArgs RunRoutesCalculation(ACMethod acMethod)
        {
            ACPointAsyncRMIWrap<ACComponent> currentAsyncRMI = RMIPoint.CurrentAsyncRMI;

            ApplicationManager.ApplicationQueue.Add(() => RoutesCalculation(currentAsyncRMI, acMethod));

            return new ACMethodEventArgs(acMethod, Global.ACMethodResultState.InProcess);
        }

        private void RoutesCalculation(ACPointAsyncRMIWrap<ACComponent> currentAsyncRMI, ACMethod acMethod)
        {
            TimeSpan ts = DateTime.Now - _LastRunPossibleRoutesCheck;
            if (ts.TotalSeconds < 15)
                return;

            RoutesCalculation();

            PABatchPlanScheduler batchScheduler = ParentACComponent.FindChildComponents<PABatchPlanScheduler>(c => c is PABatchPlanScheduler).FirstOrDefault() as PABatchPlanScheduler;
            if (batchScheduler != null)
                batchScheduler.RoutesCalculation();

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
                var pickings = PickingManager.GetScheduledPickings(dbApp, PickingStateEnum.WaitOnManualClosing, PickingStateEnum.InProcess, null, null, null, null).ToArray();

                MsgWithDetails msg = new MsgWithDetails();
                foreach (Picking picking in pickings)
                {
                    List<IACConfigStore> listOfSelectedStores = new List<IACConfigStore>() { picking };
                    PickingManager.CalculatePossibleRoutes(dbApp, db, picking, listOfSelectedStores, msg);
                }

                if (msg.MsgDetailsCount > 0)
                {
                    OnNewAlarmOccurred(IsSchedulingAlarm, msg);
                }    
            }
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

        #endregion
    }
}
