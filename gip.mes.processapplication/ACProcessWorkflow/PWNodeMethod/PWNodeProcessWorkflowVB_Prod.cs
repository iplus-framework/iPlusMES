using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using System.Data.Objects;
using gip.mes.facility;

namespace gip.mes.processapplication
{
    public partial class PWNodeProcessWorkflowVB
    {
        #region Properties
        #region Manager
        protected ACProdOrderManager ProdOrderManager
        {
            get
            {
                PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                return pwMethodProduction != null ? pwMethodProduction.ProdOrderManager : null;
            }
        }

        protected ACPartslistManager PartslistManager
        {
            get
            {
                PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                return pwMethodProduction != null ? pwMethodProduction.PartslistManager : null;
            }
        }

        protected mes.facility.ACMatReqManager MatReqManager
        {
            get
            {
                PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                return pwMethodProduction != null ? pwMethodProduction.MatReqManager : null;
            }
        }
        #endregion  

        #region Common Properties
        private gip.mes.datamodel.ProdOrderPartslist _CurrentProdOrderPartslist = null;
        public gip.mes.datamodel.ProdOrderPartslist CurrentProdOrderPartslist
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_CurrentProdOrderPartslist != null)
                        return _CurrentProdOrderPartslist;
                }
                LoadVBEntities();

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _CurrentProdOrderPartslist;
                }
            }
        }

        private gip.mes.datamodel.MaterialWFConnection _MaterialWFConnection = null;
        public gip.mes.datamodel.MaterialWFConnection MaterialWFConnection
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))

                {
                    if (_MaterialWFConnection != null)
                        return _MaterialWFConnection;
                }
                LoadVBEntities();

                using (ACMonitor.Lock(_20015_LockValue))

                {
                    return _MaterialWFConnection;
                }
            }
        }

        private List<BatchPlanningTime> _BatchPlanningTimes = null;
        protected List<BatchPlanningTime> BatchPlanningTimes
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_BatchPlanningTimes != null)
                        return _BatchPlanningTimes;
                }
                LoadVBEntities();

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _BatchPlanningTimes;
                }
            }
        }

        #endregion

        #endregion

        #region Precompiled Queries
        protected static readonly Func<DatabaseApp, Guid, Guid, mes.datamodel.ACClassWF, IQueryable<MaterialWFConnection>> s_cQry_IsSubBatchCreation =
        CompiledQuery.Compile<DatabaseApp, Guid, Guid, mes.datamodel.ACClassWF, IQueryable<MaterialWFConnection>>(
            (ctx, materialID, materialWFACClassMethodID, contentACClassWFVB) => ctx.MaterialWFConnection
                                                                    .Where(c => c.MaterialID == materialID
                                                                           && c.MaterialWFACClassMethodID == materialWFACClassMethodID
                                                                           && c.ACClassWF.ACClassMethodID == contentACClassWFVB.RefPAACClassMethodID
                                                                           && c.ACClassWFID != contentACClassWFVB.ACClassWFID
                                                                            && (c.ACClassWF.PWACClass.ACIdentifier == PWNodeProcessWorkflow.PWClassName
                                                                                || c.ACClassWF.PWACClass.BasedOnACClassID.HasValue
                                                                                    && (c.ACClassWF.PWACClass.ACClass1_BasedOnACClass.ACIdentifier == PWNodeProcessWorkflow.PWClassName
                                                                                        || c.ACClassWF.PWACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue && c.ACClassWF.PWACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == PWNodeProcessWorkflow.PWClassName)))
        );
        #endregion

        #region Methods

        #region Helper methods

        private List<ProdOrderBatchPlan> LoadUncompletedBatchPlans(ProdOrderPartslist currentProdOrderPartslist, gip.mes.datamodel.ACClassWF contentACClassWFVB)
        {
            if (currentProdOrderPartslist == null || contentACClassWFVB == null)
                return new List<ProdOrderBatchPlan>();
            var uncompletedBatchPlans = contentACClassWFVB.ProdOrderBatchPlan_VBiACClassWF.Where(c => c.ProdOrderPartslistID == currentProdOrderPartslist.ProdOrderPartslistID
                                                                    && c.PlanStateIndex <= (short)GlobalApp.BatchPlanState.Paused)
                                                                    .OrderByDescending(c => c.PlanStateIndex)
                                                                    .ThenBy(c => c.PlannedStartDate)
                                                                    .ToList();
            return uncompletedBatchPlans;
        }

        private void ReCreateBatchPlanningTimes(List<ProdOrderBatchPlan> uncompletedBatchPlans)
        {
            if (uncompletedBatchPlans != null && uncompletedBatchPlans.Any())
            {
                var batchPlanningTimes = uncompletedBatchPlans
                                        .Where(c => c.PlanState >= GlobalApp.BatchPlanState.AutoStart && c.PlanState <= GlobalApp.BatchPlanState.InProcess)
                                        .OrderByDescending(c => c.PlanStateIndex)
                                        .ThenBy(c => c.PlannedStartDate)
                                        .Select(c => new BatchPlanningTime()
                                        {
                                            ProdOrderBatchPlanID = c.ProdOrderBatchPlanID,
                                            BatchPlanState = c.PlanState,
                                            PlannedTime = c.PlannedStartDate
                                        })
                                        .ToList();

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _BatchPlanningTimes = batchPlanningTimes;
                    if (_BatchPlanningTimes.Any())
                    {
                        _LastPriorityTime = _BatchPlanningTimes.FirstOrDefault().PlannedTime;
                    }
                }
            }
            else
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _BatchPlanningTimes = new List<BatchPlanningTime>();
                }
            }
        }

        private int _NoBatchPlanFoundCounter = 0;
        public int NoBatchPlanFoundCounter
        {
            get
            {
                return _NoBatchPlanFoundCounter;
            }
        }

        private const string MN_ReadAndStartNextBatch = "ReadAndStartNextBatch";
        protected virtual StartNextBatchResult ReadAndStartNextBatch(out ProdOrderBatchPlan batchPlanEntry, out ProdOrderBatch nextBatch, out ProdOrderPartslistPos newChildPosForBatch, out Guid? startNextBatchAtProjectID, out bool isLastBatch)
        {
            batchPlanEntry = null;
            nextBatch = null;
            newChildPosForBatch = null;
            startNextBatchAtProjectID = null;
            isLastBatch = false;
            Msg msg = null;
            if (ACProgramVB == null || ContentACClassWF == null || ProdOrderManager == null || PartslistManager == null)
            {
                //Error50146:ACProgramVB is null or ContentACClassWF or ProdOrderManager or PartslistManager is null.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, MN_ReadAndStartNextBatch, 1000, "Error50146");
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartNextBatchResult.CycleWait;
            }
            if (!IsProduction)
            {
                //Error50147:Root-Node must be a PWMethodProduction.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, MN_ReadAndStartNextBatch, 1010, "Error50147");
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartNextBatchResult.CycleWait;
            }
            if (CurrentProdOrderPartslist == null)
            {
                // Error00122: No Bill of material assigned to Workflow
                msg = new Msg(this, eMsgLevel.Error, PWClassName, MN_ReadAndStartNextBatch, 1020, "Error00122");
                OnNewAlarmOccurred(ProcessAlarm, msg, true);

                return StartNextBatchResult.CycleWait;
            }

            //var vbDump = Root.SRootDump;
            //Regex rgx = new Regex("\\((.*?)\\)");
            //string loggerInstance = rgx.Replace(this.GetACUrl(), "");
            //PerformanceEvent perfEvent = null;

            //perfEvent = vbDump != null ? vbDump.PerfLogger.Start(loggerInstance, 200) : null;
            bool canBeStarted = CheckIfNextBatchCanBeStarted();
            //if (perfEvent != null)
            //    vbDump.PerfLogger.Stop(loggerInstance, 200, perfEvent);

            if (!canBeStarted)
            {
                return StartNextBatchResult.WaitForNextEvent;
            }

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                var currentProdOrderPartslist = CurrentProdOrderPartslist.FromAppContext<ProdOrderPartslist>(dbApp);
                var contentACClassWFVB = ContentACClassWF.FromAppContext<gip.mes.datamodel.ACClassWF>(dbApp);

                var uncompletedBatchPlans = LoadUncompletedBatchPlans(currentProdOrderPartslist, contentACClassWFVB);
                ReCreateBatchPlanningTimes(uncompletedBatchPlans);

                var startableBatchPlans = uncompletedBatchPlans.Where(c =>     c.PlanState >= GlobalApp.BatchPlanState.AutoStart 
                                                                            && c.PlanState <= GlobalApp.BatchPlanState.InProcess)
                                                                    .OrderByDescending(c => c.PlanStateIndex)
                                                                    .ThenBy(c => c.PlannedStartDate);
                var activeBatchPlan = startableBatchPlans.FirstOrDefault();
                if (activeBatchPlan == null)
                {
                    if (this.IterationCount.ValueT > 0)
                    {
                        _NoBatchPlanFoundCounter = 0;
                        return StartNextBatchResult.Done;
                    }
                    else
                    {
                        ParallelNodeStats stats = GetParallelNodeStats();
                        // If there are no active nodes any more and no other node has a breakpoint set,
                        // an all other nodes are waiting like this one
                        // then complete this node
                        if (_NoBatchPlanFoundCounter > 0
                            && stats.AreOtherParallelNodesCompletable)
                            //&& stats.ActiveParallelNodesCount <= 0
                            //&& (stats.WaitingParallelNodesCount == stats.CountParallelNodes - stats.IdleParallelNodesCount))
                        {
                            _NoBatchPlanFoundCounter = 0;
                            return StartNextBatchResult.Done;
                        }
                        // As far as there are other active nodes, remain in SMStarting-State because somebody maybe wants to reactivate this node
                        // => return StartNextBatchResult.CycleWait
                        else
                        {
                            _NoBatchPlanFoundCounter++;
                            return StartNextBatchResult.CycleWait;
                        }
                    }
                }
                else if (!activeBatchPlan.IsValid)
                {
                    if (this.IterationCount.ValueT > 0 && activeBatchPlan.PlanMode == BatchPlanMode.UseTotalSize && activeBatchPlan.TotalSize <= 0.00001)
                    {
                        activeBatchPlan.PlanState = GlobalApp.BatchPlanState.Completed;
                        _NoBatchPlanFoundCounter = 0;
                        dbApp.ACSaveChanges();
                        return StartNextBatchResult.Done;
                    }
                    // Error00125: Batchplan-Values are invalid
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, MN_ReadAndStartNextBatch, 1040, "Error00125");
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    return StartNextBatchResult.CycleWait;
                }

                MaterialWFConnection matWFConnection = null;
                if (activeBatchPlan != null && activeBatchPlan.MaterialWFACClassMethodID.HasValue)
                {
                    matWFConnection = contentACClassWFVB.MaterialWFConnection_ACClassWF
                        .Where(c => c.MaterialWFACClassMethodID == activeBatchPlan.MaterialWFACClassMethodID.Value)
                        .FirstOrDefault();
                }
                else
                {
                    PartslistACClassMethod plMethod = currentProdOrderPartslist.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
                    if (plMethod != null)
                    {
                        matWFConnection = contentACClassWFVB.MaterialWFConnection_ACClassWF
                            .Where(c => c.MaterialWFACClassMethodID == plMethod.MaterialWFACClassMethodID)
                            .FirstOrDefault();
                    }
                    else
                    {
                        matWFConnection = contentACClassWFVB.MaterialWFConnection_ACClassWF
                            .Where(c => c.MaterialWFACClassMethod.MaterialWFID == currentProdOrderPartslist.Partslist.MaterialWFID)
                            .FirstOrDefault();
                    }
                }

                if (matWFConnection == null)
                {
                    // Error00124: No relation defined between Workflownode and intermediate material in Materialworkflow
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, MN_ReadAndStartNextBatch, 1050, "Error00124");
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    return StartNextBatchResult.CycleWait;
                }

                ProdOrderPartslistPos intermediatePosition = currentProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                    .Where(c => c.MaterialID.HasValue && c.MaterialID == matWFConnection.MaterialID
                        && c.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern
                        && !c.ParentProdOrderPartslistPosID.HasValue).FirstOrDefault();
                // TODO: Falls zwei Batch-Start-Schritte parallel, weil zwei verscheidene Unterrezepte gestartet werden sollen z.B. Normalablauf danach Leerfahrrezept
                // Logik: "Bin ich der Schritt der dieses Unterrezept starten muss"
                if (intermediatePosition == null)
                {
                    //Error50149:ProdOrderPartslistPos not found for intermediate Material.
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, MN_ReadAndStartNextBatch, 1060, "Error50149");
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    return StartNextBatchResult.CycleWait;
                }

                // Deactivated 31.07.2021, because veryfication is done before at row with "if (activeBatchPlan == null)"
                //if (!startableBatchPlans.Any())
                //{
                //    // Error00123: No batchplan found for this intermediate material
                //    msg = new Msg(this, eMsgLevel.Error, PWClassName, MN_ReadAndStartNextBatch, 1070, "Error00123");
                //    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                //    return StartNextBatchResult.CycleWait;
                //}

                // Schaue nach aktiven Batch-Planungseinträgen
                int nextBatchNo = 0;
                ProdOrderBatch prevBatch = null;
                double tolerance = intermediatePosition.TargetQuantityUOM * ProdOrderManager.TolRemainingCallQ * 0.01;
                bool totalSizeReached;

                //perfEvent = vbDump != null ? vbDump.PerfLogger.Start(loggerInstance, 202) : null;
                NextBatchState nbResult = ManageStateAndGetNextBatchFromPlanEntry(intermediatePosition, currentProdOrderPartslist, dbApp, uncompletedBatchPlans, null, false, null, false,
                                                out batchPlanEntry, out prevBatch, out nextBatchNo, out isLastBatch, out totalSizeReached, tolerance);
                //if (perfEvent != null)
                //    vbDump.PerfLogger.Stop(loggerInstance, 202, perfEvent);

                // Falls Status eines BatchPlans auf ERledigt gesetzt wurde, dann erneut aktualisiseren
                ReCreateBatchPlanningTimes(uncompletedBatchPlans);

                if (prevBatch != null)
                {
                    Guid batchPlanID = Guid.Empty;
                    if (batchPlanEntry != null)
                        batchPlanID = batchPlanEntry.ProdOrderBatchPlanID;
                    // Sonst letzter Batch von letztem Planeintrag
                    else if (prevBatch.ProdOrderBatchPlanID.HasValue)
                        batchPlanID = prevBatch.ProdOrderBatchPlanID.Value;

                    if (batchPlanID != Guid.Empty)
                    {
                        var lastProdOrderPartslistPosBatch = prevBatch.ProdOrderPartslistPos_ProdOrderBatch
                                                                            .Where(c => c.ParentProdOrderPartslistPosID == intermediatePosition.ProdOrderPartslistPosID)
                                                                            .FirstOrDefault();
                        if (lastProdOrderPartslistPosBatch != null)
                        {
                            // Wenn zuletzt nach der BAtcherzeugnung der Server abgestürtzt ist und neu hochgefahren worden ist, dann ist der Start des Sub-Workflows nicht erfolgreich gewesen
                            if (!lastProdOrderPartslistPosBatch.ACClassTaskID.HasValue
                                && prevBatch.MDProdOrderState.ProdOrderState < MDProdOrderState.ProdOrderStates.InProduction)
                            {

                                //perfEvent = vbDump != null ? vbDump.PerfLogger.Start(loggerInstance, 203) : null;
                                bool isStarted = CheckIfBatchIsStartedInSubWf(prevBatch.ProdOrderBatchID);
                                //if (perfEvent != null)
                                //    vbDump.PerfLogger.Stop(loggerInstance, 203, perfEvent);

                                if (!isStarted)
                                {
                                    nextBatch = prevBatch;
                                    newChildPosForBatch = lastProdOrderPartslistPosBatch;
                                    startNextBatchAtProjectID = null;
                                    return StartNextBatchResult.StartNextBatch;
                                }
                            }
                        }
                    }
                }

                if (nbResult == NextBatchState.CompletedNoNewEntry)
                {
                    if (AlarmOnCompleted)
                    {
                        //Error50203: Subtasks Order {0}, Recipe {1}, Position {2} are completed.
                        string message = Root.Environment.TranslateMessage(this, "Error50203",
                                                        intermediatePosition.ProdOrderPartslist.ProdOrder.ProgramNo,
                                                        intermediatePosition.ProdOrderPartslist.Partslist.PartslistNo,
                                                        intermediatePosition.BookingMaterial.MaterialName1);
                        msg = new Msg(message, this, eMsgLevel.Error, PWClassName, MN_ReadAndStartNextBatch, 1080);
                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogDebug(this.GetACUrl(), "MN_ReadAndStartNextBatch", msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    }

                    if (   (totalSizeReached && EndProdOrderPartslistMode == EndPListMode.OrderSizeReached)
                        || (!uncompletedBatchPlans.Where(c => c.PlanState < GlobalApp.BatchPlanState.Completed).Any() && EndProdOrderPartslistMode == EndPListMode.AllBatchPlansCompleted))
                    {
                        // Complete this ProdorderPartslist
                        MDProdOrderState finishedState = DatabaseApp.s_cQry_GetMDProdOrderState(dbApp, MDProdOrderState.ProdOrderStates.ProdFinished).FirstOrDefault();
                        if (finishedState != null)
                            intermediatePosition.ProdOrderPartslist.MDProdOrderState = finishedState;

                        // Complete following ProdorderPartslist
                        if (StartNextStage == StartNextStageMode.CompleteOnLastBatch)
                        {
                            List<ProdOrderBatchPlan> batchPlans = ProdOrderManager.GetBatchplansOfNextStages(dbApp, intermediatePosition.ProdOrderPartslist);
                            bool canCompletePOLists = !batchPlans.Any(c => c.PlanState > GlobalApp.BatchPlanState.Created && c.PlanState < GlobalApp.BatchPlanState.Completed);
                            batchPlans.Where(c => c.PlanState == GlobalApp.BatchPlanState.Created).ToList().ForEach(c => c.PlanState = GlobalApp.BatchPlanState.Cancelled);
                            if (canCompletePOLists)
                            {
                                if (finishedState != null)
                                    ProdOrderManager.GetNextStages(dbApp, intermediatePosition.ProdOrderPartslist).ForEach(c => c.MDProdOrderState = finishedState);
                            }
                        }

                        // Complete whole ProdOrder
                        IEnumerable<ProdOrderPartslist> allProdOrderPartslists = dbApp.ProdOrderPartslist
                                                                                .Include(c => c.MDProdOrderState)
                                                                                .Include(c => c.ProdOrder)
                                                                                .Where(c => c.ProdOrderID == intermediatePosition.ProdOrderPartslist.ProdOrderID).ToArray();
                        if (   allProdOrderPartslists != null
                            && !allProdOrderPartslists.Where(c => c.MDProdOrderState.ProdOrderState < MDProdOrderState.ProdOrderStates.ProdFinished).Any())
                        {
                            intermediatePosition.ProdOrderPartslist.ProdOrder.MDProdOrderState = finishedState;
                        }
                    }

                    if (dbApp.IsChanged)
                        dbApp.ACSaveChanges();

                    OnBatchplanCompleted(dbApp, intermediatePosition, batchPlanEntry);
                }
                else if (    nbResult == NextBatchState.NoPlanEntryFound 
                          && uncompletedBatchPlans.Where(c => c.PlanState == GlobalApp.BatchPlanState.Created || c.PlanState == GlobalApp.BatchPlanState.Paused).Any())
                {
                    //// Error00123: No batchplan found for this intermediate material
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, MN_ReadAndStartNextBatch, 1090, "Error00123");
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    return StartNextBatchResult.CycleWait;
                }
                // When PartialQuantity in a Batchplan was reached but the total batch count is not reached: 
                else if (nbResult == NextBatchState.UncompletedButPartialQuantityReached)
                {
                    // No Alarm or Warning!
                }

                if (batchPlanEntry == null)
                    return StartNextBatchResult.Done;

                IList<FacilityReservation> plannedSilos = batchPlanEntry.FacilityReservation_ProdOrderBatchPlan.ToArray();
                if (plannedSilos.Any())
                {
                    var plannedSilo = plannedSilos.FirstOrDefault();
                    if (plannedSilo.VBiACClassID != null)
                    {
                        var queryProject = dbApp.ACClass.Where(c => c.ACClassID == plannedSilo.VBiACClassID).Select(c => c.ACProjectID);
                        if (queryProject.Any())
                            startNextBatchAtProjectID = queryProject.FirstOrDefault();
                    }
                }

                if (ProdOrderManager != null && PartslistManager != null)
                {
                    try
                    {
                        using (var dbIPlus = new Database())
                        {
                            bool validationSuccess = true;
                            var mandantoryConfigStores = MandatoryConfigStores;
                            if (!ValidateExpectedConfigStores())
                            {
                                validationSuccess = false;
                                return StartNextBatchResult.CycleWait;
                            }
                            MsgWithDetails msg2 = ProdOrderManager.ValidateStart(dbApp, dbIPlus, this,
                                                                        currentProdOrderPartslist, mandantoryConfigStores,
                                                                        PARole.ValidationBehaviour.Strict, PartslistManager, MatReqManager);
                            if (msg2 != null)
                            {
                                if (!String.IsNullOrEmpty(msg2.InnerMessage))
                                    OnNewAlarmOccurred(ProcessAlarm, new Msg(msg2.InnerMessage, this, eMsgLevel.Error, PWClassName, MN_ReadAndStartNextBatch, 1200), true);
                                if (!msg2.IsSucceded())
                                {
                                    validationSuccess = false;
                                    return StartNextBatchResult.CycleWait;
                                }
                            }
                            if (validationSuccess)
                            {
                                if (ProdOrderManager.SetBatchPlanValidated(dbApp, currentProdOrderPartslist) > 0)
                                    dbApp.ACSaveChanges();
                            }
                        }
                    }
                    catch (Exception ec)
                    {
                        Messages.LogException("PWNodeProcessWorkflowVB_Prod", "ReadAndStartNextBatch", ec);
                    }
                }

                double batchCreateSize = GetBatchSizeForNextBatch(dbApp, intermediatePosition, currentProdOrderPartslist, batchPlanEntry, prevBatch, nextBatchNo, GetBatchSizePhase.BeforeCreateBatch);

                List<ACProdOrderManager.BatchCreateEntry> batchCreatePlan = new List<ACProdOrderManager.BatchCreateEntry>();
                batchCreatePlan.Add(new ACProdOrderManager.BatchCreateEntry() { BatchSeqNo = nextBatchNo, Size = batchCreateSize });
                List<object> resultNewEntities = new List<object>();
                msg = ProdOrderManager.BatchCreate(dbApp, intermediatePosition, batchCreatePlan, resultNewEntities, BatchSizeLoss, null); // Toleranz ist max. ein Batch mehr
                if (msg != null)
                {
                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        Messages.LogException(this.GetACUrl(), "ReadAndStartNextBatch(3)", msg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    dbApp.ACUndoChanges();
                    return StartNextBatchResult.CycleWait;
                }
                else
                {
                    batchPlanEntry.BatchActualCount++;
                    if (batchPlanEntry.DiffPartialCount.HasValue)
                        batchPlanEntry.PartialActualCount = batchPlanEntry.PartialActualCount.HasValue ? batchPlanEntry.PartialActualCount + 1 : 1;
                    batchPlanEntry.PlanState = GlobalApp.BatchPlanState.InProcess;
                    if (intermediatePosition.MDProdOrderPartslistPosState.ProdOrderPartslistPosState == MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.AutoStart)
                        intermediatePosition.MDProdOrderPartslistPosState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.InProcess).FirstOrDefault();
                    newChildPosForBatch = resultNewEntities.Where(c => c is ProdOrderPartslistPos).FirstOrDefault() as ProdOrderPartslistPos;

                    FacilityLot facilityLot = null;
                    if (newChildPosForBatch != null && newChildPosForBatch.BookingMaterial.IsLotManaged)
                    {
                        facilityLot = newChildPosForBatch.FacilityLot;
                        if (facilityLot == null)
                        {
                            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(FacilityLot), FacilityLot.NoColumnName, FacilityLot.FormatNewNo, this);
                            facilityLot = FacilityLot.NewACObject(dbApp, null, secondaryKey);
                            newChildPosForBatch.FacilityLot = facilityLot;
                        }
                    }

                    nextBatch = resultNewEntities.Where(c => c is ProdOrderBatch).FirstOrDefault() as ProdOrderBatch;
                    if (newChildPosForBatch != null && nextBatch != null)
                    {
                        newChildPosForBatch.MDProdOrderPartslistPosState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.InProcess)
                                .FirstOrDefault();

                        //nextBatch.SetDestination(roasterGroup.GetACUrl());
                        nextBatch.ProdOrderBatchPlan = batchPlanEntry;
                        //activeRoastingChildPos.CopySelectedModulesFromParent(activeRoastingPos);

                        // Create Child-Positions for other intermediates which belongs to the same batch
                        var iPositionsBelongsToSameBatch = currentProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                            .Where(c => c.MaterialID.HasValue
                                && c.Material.MaterialWFConnection_Material
                                        .Where(d => d.MaterialWFACClassMethodID == matWFConnection.MaterialWFACClassMethodID
                                                && d.ACClassWF.ACClassMethodID == contentACClassWFVB.RefPAACClassMethodID).Any()
                                && c.ProdOrderPartslistPosID != intermediatePosition.ProdOrderPartslistPosID
                                && c.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern
                                && !c.ParentProdOrderPartslistPosID.HasValue)
                            .ToArray();
                        foreach (ProdOrderPartslistPos intermediatePosOfSubWF in iPositionsBelongsToSameBatch)
                        {
                            // Finde heraus ob diese Zwischenmaterial mit einem anderen PWNodeProcessWorkflow verbunden ist als dieser
                            // Falls ja, dann erzeugt dieser selbst weitere unterbatche. Dann darf dieser PWNodeProcessWorkflow keinen Batch für dieses Zwischenmaterial erzeugen
                            var queryBatchCreation = s_cQry_IsSubBatchCreation(dbApp, intermediatePosOfSubWF.MaterialID.Value, matWFConnection.MaterialWFACClassMethodID, contentACClassWFVB);
                            if (!queryBatchCreation.Any())
                            {
                                resultNewEntities = new List<object>();
                                msg = ProdOrderManager.BatchCreate(dbApp, intermediatePosOfSubWF, nextBatch, newChildPosForBatch.BatchFraction, nextBatch.BatchSeqNo, resultNewEntities); // Toleranz ist max. ein Batch mehr
                                if (msg != null)
                                {
                                    Messages.LogError(this.GetACUrl(), "ReadAndStartNextBatch(4)", msg.InnerMessage);
                                    OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Exception, PWClassName, MN_ReadAndStartNextBatch, 1220), true);
                                    dbApp.ACUndoChanges();
                                    return StartNextBatchResult.CycleWait;
                                }
                                if (facilityLot != null)
                                {
                                    foreach (var newChildBatchEntities in resultNewEntities.Where(c => c is ProdOrderPartslistPos).Select(c => c as ProdOrderPartslistPos))
                                    {
                                        newChildBatchEntities.FacilityLot = facilityLot;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Messages.LogError(this.GetACUrl(), "ReadAndStartNextBatch(5)", "No Batch created");
                        OnNewAlarmOccurred(ProcessAlarm, new Msg("No Batch created", this, eMsgLevel.Exception, PWClassName, MN_ReadAndStartNextBatch, 1230), true);
                        dbApp.ACUndoChanges();
                        return StartNextBatchResult.CycleWait;
                    }
                }
                CurrentProdOrderBatchPlanID = batchPlanEntry.ProdOrderBatchPlanID;
                if (batchPlanEntry.BatchActualCount == 1)
                {
                    newChildPosForBatch.ProdOrderPartslist.StartDate = DateTime.Now;
                }

                MsgWithDetails saveMsg = dbApp.ACSaveChanges();
                if (saveMsg != null)
                {
                    Messages.LogError (this.GetACUrl(), "ReadAndStartNextBatch(5)", saveMsg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, new Msg(saveMsg.InnerMessage, this, eMsgLevel.Exception, PWClassName, MN_ReadAndStartNextBatch, 1210), true);
                    dbApp.ACUndoChanges();
                    return StartNextBatchResult.CycleWait;
                }


                return StartNextBatchResult.StartNextBatch;
            }

            //queryBatchPlans.Where();
        }

        protected bool WillReadAndStartNextBatchCompleteNode_Prod()
        {
            return false;
        }

        protected virtual void OnBatchplanCompleted(DatabaseApp dbApp, ProdOrderPartslistPos intermediatePosition, ProdOrderBatchPlan lastBatchPlanEntry)
        {
        }

        public Guid? CurrentProdOrderBatchPlanID { get; set; }


        /// <summary>
        /// ManageStateAndGetNextBatchFromPlanEntry
        /// </summary>
        /// <param name="prodOrderPartslistPos">Root Position of intermediate Pos</param>
        /// <param name="currentProdOrderPartslist"></param>
        /// <param name="dbApp"></param>
        /// <param name="batchSizePlanList"></param>
        /// <param name="batchSize"></param>
        /// <param name="adaptToRestQuantity"></param>
        /// <param name="acClassOfMachine"></param>
        /// <param name="ignoreModuleSelection"></param>
        /// <param name="batchPlanEntry"></param>
        /// <param name="prevBatch"></param>
        /// <param name="nextBatchNo"></param>
        /// <param name="isLastBatch"></param>
        /// <param name="toleranceRemainingQuantity"></param>
        /// <returns></returns>
        public virtual NextBatchState ManageStateAndGetNextBatchFromPlanEntry(ProdOrderPartslistPos prodOrderPartslistPos, ProdOrderPartslist currentProdOrderPartslist, DatabaseApp dbApp,
                                                                        IEnumerable<ProdOrderBatchPlan> batchSizePlanList,
                                                                        Nullable<double> batchSize, bool adaptToRestQuantity,
                                                                        gip.core.datamodel.ACClass acClassOfMachine, bool ignoreModuleSelection,
                                                                        out ProdOrderBatchPlan batchPlanEntry, out ProdOrderBatch prevBatch, out int nextBatchNo, out bool isLastBatch, out bool totalSizeReached,
                                                                        double? toleranceRemainingQuantity = null)
        {
            batchPlanEntry = null;
            prevBatch = null;
            nextBatchNo = 0;
            isLastBatch = false;
            totalSizeReached = false;
            if (batchSizePlanList != null && batchSizePlanList.Any())
            {
                var batchPlanForMachine = batchSizePlanList.Where(c => c.PlanState == GlobalApp.BatchPlanState.AutoStart
                                                                    || c.PlanState == GlobalApp.BatchPlanState.InProcess)
                                                           .OrderByDescending(c => c.PlanStateIndex)
                                                           .ThenBy(c => c.PlannedStartDate);
                if (batchPlanForMachine.Any())
                    batchPlanEntry = batchPlanForMachine.FirstOrDefault();
                else
                    return NextBatchState.NoPlanEntryFound;

                if (batchPlanEntry == null)
                    return NextBatchState.NoPlanEntryFound;

                // Ermittle zuletzt gestarteten Batch
                prevBatch = prodOrderPartslistPos.ProdOrderPartslistPos_ParentProdOrderPartslistPos
                    .Where(c => c.ProdOrderBatch != null)
                    .Select(c => c.ProdOrderBatch).Distinct().OrderByDescending(c => c.BatchSeqNo)
                    .FirstOrDefault();
                nextBatchNo = prevBatch == null ? 1 : prevBatch.BatchSeqNo + 1;

                if (!batchSize.HasValue)
                {
                    batchSize = GetBatchSizeForNextBatch(dbApp, prodOrderPartslistPos, currentProdOrderPartslist, batchPlanEntry, prevBatch, nextBatchNo, GetBatchSizePhase.ManageState);
                }
                // Überprüfe ob nochmal ein Batch gestratet werden muss
                bool createOneBatchMore = false;
                if (batchPlanEntry.PlanMode == BatchPlanMode.UseFromTo)
                {
                    if (nextBatchNo <= batchPlanEntry.BatchNoTo)
                    {
                        createOneBatchMore = true;
                        if (nextBatchNo == batchPlanEntry.BatchNoTo)
                            isLastBatch = true;
                    }
                }
                else if (batchPlanEntry.PlanMode == BatchPlanMode.UseBatchCount)
                {
                    if (batchPlanEntry.BatchActualCount < batchPlanEntry.BatchTargetCount)
                    {
                        if (!batchPlanEntry.DiffPartialCount.HasValue
                            || batchPlanEntry.DiffPartialCount > 0)
                        {
                            // Somebody has reset the PartialTargetCount-Property during the partial production => don't create a batch
                            if (batchPlanEntry.PartialActualCount.HasValue && !batchPlanEntry.DiffPartialCount.HasValue)
                                createOneBatchMore = false;
                            else
                                createOneBatchMore = true;
                        }
                        if (batchPlanEntry.BatchActualCount + 1 >= batchPlanEntry.BatchTargetCount)
                            isLastBatch = true;
                    }
                    else if (batchPlanEntry.BatchActualCount >= batchPlanEntry.BatchTargetCount)
                        isLastBatch = true;
                }
                else //if (batchPlanEntry.PlanMode == BatchPlanMode.UseTotalSize)
                {
                    if ((!adaptToRestQuantity && batchPlanEntry.RemainingQuantity + batchSize.Value > batchSize.Value)
                        || (adaptToRestQuantity && batchPlanEntry.RemainingQuantity > 1))
                    {
                        createOneBatchMore = true;
                        double newRemainingQ = batchPlanEntry.RemainingQuantity - batchSize.Value;
                        isLastBatch = !((!adaptToRestQuantity && newRemainingQ + batchSize.Value > batchSize.Value)
                                        || (adaptToRestQuantity && newRemainingQ > 1));
                    }
                }

                if (toleranceRemainingQuantity.HasValue && prodOrderPartslistPos.RemainingCallQuantity < 0)
                    totalSizeReached = (prodOrderPartslistPos.RemainingCallQuantity * -1) > toleranceRemainingQuantity;
                else
                    totalSizeReached = prodOrderPartslistPos.RemainingCallQuantity + batchSize.Value < batchSize.Value;
                //if (adaptToRestQuantity && (batchPlanEntry.PlanMode == BatchPlanMode.UseTotalSize))
                //totalSizeReached = prodOrderPartslistPos.RemainingCallQuantity < 1;
                // Überprüfe ob die Gesamtmenge erreicht ist. Sie darf um max. einen Batch darf größer sein

                if (!createOneBatchMore || totalSizeReached)
                {
                    NextBatchState nextBatchState = NextBatchState.CompletedNoNewEntry;
                    if (batchPlanEntry.PlanMode != BatchPlanMode.UseBatchCount
                        || isLastBatch 
                        || totalSizeReached)
                    {
                        if ((isLastBatch
                                && (!batchPlanEntry.DiffPartialCount.HasValue
                                    || (batchPlanEntry.DiffPartialCount <= 0 && batchPlanEntry.BatchActualCount >= batchPlanEntry.BatchTargetCount)))
                            || totalSizeReached
                            || batchPlanEntry.PlanMode != BatchPlanMode.UseBatchCount)
                            batchPlanEntry.PlanState = GlobalApp.BatchPlanState.Completed;
                        else if ((batchPlanEntry.DiffPartialCount.HasValue && batchPlanEntry.DiffPartialCount <= 0)
                            || (!batchPlanEntry.DiffPartialCount.HasValue && batchPlanEntry.PartialActualCount.HasValue))
                        {
                            nextBatchState = NextBatchState.UncompletedButPartialQuantityReached;
                            batchPlanEntry.PlanState = GlobalApp.BatchPlanState.Paused;
                        }
                        batchPlanEntry.PartialTargetCount = null;
                        batchPlanEntry.PartialActualCount = null;
                    }
                    else if (  (batchPlanEntry.DiffPartialCount.HasValue && batchPlanEntry.DiffPartialCount <= 0)
                            || (!batchPlanEntry.DiffPartialCount.HasValue && batchPlanEntry.PartialActualCount.HasValue))
                    {
                        nextBatchState = NextBatchState.UncompletedButPartialQuantityReached;
                        batchPlanEntry.PlanState = GlobalApp.BatchPlanState.Paused;
                        batchPlanEntry.PartialTargetCount = null;
                        batchPlanEntry.PartialActualCount = null;
                    }
                    if (!batchSizePlanList.Where(c => c.PlanState == GlobalApp.BatchPlanState.AutoStart
                                                   || c.PlanState == GlobalApp.BatchPlanState.InProcess).Any())
                    {
                        if (totalSizeReached)
                        {
                            prodOrderPartslistPos.MDProdOrderPartslistPosState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed).FirstOrDefault();
                            // prodOrderPartslistPos.TopParentPartslistPos.RecalcActualQuantity();
                            prodOrderPartslistPos.ProdOrderPartslist.RecalcActualQuantitySP(dbApp);
                            //if (EndPList == 
                        }
                        else
                            prodOrderPartslistPos.MDProdOrderPartslistPosState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.PartialCompleted).FirstOrDefault();
                    }
                    else if (totalSizeReached)
                    {
                        prodOrderPartslistPos.MDProdOrderPartslistPosState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed).FirstOrDefault();
                        //prodOrderPartslistPos.TopParentPartslistPos.RecalcActualQuantity();
                        prodOrderPartslistPos.ProdOrderPartslist.RecalcActualQuantitySP(dbApp);
                    }

                    dbApp.ACSaveChanges();
                    batchPlanEntry = null;
                    return nextBatchState;
                }
            }
            else
                return NextBatchState.NoPlanEntryFound;
            return batchPlanEntry != null ? NextBatchState.EntryFound : NextBatchState.NoPlanEntryFound;
        }

        public enum GetBatchSizePhase
        {
            ManageState = 0,
            BeforeCreateBatch = 1
        }

        public virtual double GetBatchSizeForNextBatch(DatabaseApp dbApp, ProdOrderPartslistPos intermediatePosition, ProdOrderPartslist currentProdOrderPartslist,
                                                        ProdOrderBatchPlan batchPlanEntry, ProdOrderBatch lastBatch, int nextBatchNo, GetBatchSizePhase phase)
        {
            bool batchwiseCreation = batchPlanEntry.PlanMode == BatchPlanMode.UseBatchCount || batchPlanEntry.PlanMode == BatchPlanMode.UseFromTo;
            double batchCreateSize = batchwiseCreation ? batchPlanEntry.BatchSize : batchPlanEntry.TotalSize;

            if (phase == GetBatchSizePhase.BeforeCreateBatch)
            {
                // Convert to quantity measured in units of intermediate-Material 
                MDUnit mdUnitPartslist = currentProdOrderPartslist.Partslist.Material.BaseMDUnit;
                MDUnit mdUnitIntermediate = intermediatePosition.MDUnit;
                if (mdUnitIntermediate == null)
                    mdUnitIntermediate = intermediatePosition.Material.BaseMDUnit;
                if (mdUnitPartslist != null && mdUnitIntermediate != null && mdUnitPartslist != mdUnitIntermediate)
                {
                    try
                    {
                        batchCreateSize = intermediatePosition.Material.ConvertToBaseQuantity(batchCreateSize, mdUnitPartslist);
                    }
                    catch (Exception ec)
                    {
                        Messages.LogException("PWNodeProcessWorkflowVB_Prod", "GetBatchSizeForNextBatch", ec);
                    }
                }
            }
            return batchCreateSize;
        }
        #endregion


        #region ACState
        //protected ProdOrderBatchPlan _BatchPlanToStart = null;
        //protected ProdOrderBatch _NextBatch = null;
        //protected ProdOrderPartslistPos _NewChildPosForBatch = null;
        //protected Guid? _StartNextBatchAtProjectID = null;
        protected Guid? _NewChildPosForBatch_ProdOrderPartslistPosID;
        protected Guid? NewChildPosForBatch_ProdOrderPartslistPosID
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _NewChildPosForBatch_ProdOrderPartslistPosID;
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _NewChildPosForBatch_ProdOrderPartslistPosID = value;
                }
            }
        }

#endregion

#region Interaction
        public void ReloadBatchPlans()
        {
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                var currentProdOrderPartslist = CurrentProdOrderPartslist.FromAppContext<ProdOrderPartslist>(dbApp);
                var contentACClassWFVB = ContentACClassWF.FromAppContext<gip.mes.datamodel.ACClassWF>(dbApp);

                var uncompletedBatchPlans = LoadUncompletedBatchPlans(currentProdOrderPartslist, contentACClassWFVB);
                ReCreateBatchPlanningTimes(uncompletedBatchPlans);
            }

            using (ACMonitor.Lock(_20015_LockValue))
            {
                    _PlanningWait = null;
            }
        }
#endregion
#endregion
    }
}
