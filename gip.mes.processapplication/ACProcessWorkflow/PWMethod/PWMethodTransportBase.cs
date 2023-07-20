using System;
using System.Linq;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System.Collections.Generic;

namespace gip.mes.processapplication
{
    [ACClassConstructorInfo(
    new object[]
        {
            new object[] {gip.core.datamodel.ACProgram.ClassName, Global.ParamOption.Required, typeof(Guid)},
            new object[] {gip.core.datamodel.ACProgramLog.ClassName, Global.ParamOption.Optional, typeof(Guid)},
            new object[] {PWProcessFunction.C_InvocationCount, Global.ParamOption.Optional, typeof(int)},
            new object[] {Picking.ClassName, Global.ParamOption.Optional, typeof(Guid)},
            new object[] {DeliveryNotePos.ClassName, Global.ParamOption.Optional, typeof(Guid)},
            new object[] {FacilityBooking.ClassName, Global.ParamOption.Optional, typeof(Guid)},
            new object[] {PickingPos.ClassName, Global.ParamOption.Optional, typeof(Guid) },
            new object[] {PWMethodVBBase.IsLastBatchParamName, Global.ParamOption.Optional, typeof(Int16) }
        }
    )]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PWMethodTransportBase'}de{'PWMethodTransportBase'}", Global.ACKinds.TPWMethod, Global.ACStorableTypes.Optional, true, true, "", "PWMethodTransportBase", 20)]
    public abstract class PWMethodTransportBase : PWMethodVBBase 
    {
        new public const string PWClassName = "PWMethodTransportBase";

        #region c´tors
        static PWMethodTransportBase()
        {
            RegisterExecuteHandler(typeof(PWMethodTransportBase), HandleExecuteACMethod_PWMethodTransportBase);
        }

        public PWMethodTransportBase(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            _PickingManager = ACPickingManager.ACRefToServiceInstance(this);
            if (_PickingManager == null)
                throw new Exception("PickingManager not configured");

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            ACPickingManager.DetachACRefFromServiceInstance(this, _PickingManager);
            _PickingManager = null;
            UnregisterFromCachedDestinations();


            using (ACMonitor.Lock(_20015_LockValue))
            {
                _CurrentDeliveryNotePos = null;
                _CurrentPicking = null;
                _CurrentPickingPos = null;
                _CurrentFacilityBooking = null;
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
                _CurrentDeliveryNotePos = null;
                _CurrentPicking = null;
                _CurrentPickingPos = null;
                _CurrentFacilityBooking = null;
                _NewAddedProgramLog = null;
            }
            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }
        #endregion

        #region Properties
        protected gip.mes.datamodel.Picking _CurrentPicking = null;
        /// <summary>
        /// CurrentProdOrderPartslistPos could be a Child-Pos which is a Batch. If this node is an Instance on planning level, then the Position is the root intermediate Pos.
        /// </summary>
        public gip.mes.datamodel.Picking CurrentPicking
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_CurrentPicking != null)
                        return _CurrentPicking;
                }
                LoadVBEntities();

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _CurrentPicking;
                }
            }
        }

        protected gip.mes.datamodel.PickingPos _CurrentPickingPos = null;
        public gip.mes.datamodel.PickingPos CurrentPickingPos
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_CurrentPickingPos != null)
                        return _CurrentPickingPos;
                }
                LoadVBEntities();

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _CurrentPickingPos;
                }
            }
        }

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

        protected gip.mes.datamodel.DeliveryNotePos _CurrentDeliveryNotePos = null;
        public gip.mes.datamodel.DeliveryNotePos CurrentDeliveryNotePos
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_CurrentDeliveryNotePos != null)
                        return _CurrentDeliveryNotePos;
                }
                LoadVBEntities();

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _CurrentDeliveryNotePos;
                }
            }
        }

        protected gip.mes.datamodel.FacilityBooking _CurrentFacilityBooking = null;
        public gip.mes.datamodel.FacilityBooking CurrentFacilityBooking
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_CurrentFacilityBooking != null)
                        return _CurrentFacilityBooking;
                }
                LoadVBEntities();

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _CurrentFacilityBooking;
                }
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
                case nameof(EndPicking):
                    EndPicking();
                    return true;
                case nameof(IsEnabledEndPicking):
                    result = IsEnabledEndPicking();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PWMethodTransportBase(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(AskUserEndPicking):
                    result = AskUserEndPicking(acComponent);
                    return true;
            }
            return HandleExecuteACMethod_PWMethodVBBase(out result, acComponent, acMethodName, acClassMethod, acParameter);
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

        #region Order

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
            using (ACMonitor.Lock(_20015_LockValue))
            {
                if (_CurrentPicking != null)
                    return;
            }
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                ACMethod acMethod = CurrentACMethod.ValueT;
                if (acMethod != null)
                {
                    var entity = PWDischarging.GetTransportEntityFromACMethod(dbApp, acMethod);
                    if (entity != null)
                    {
                        Picking currentPicking = entity as Picking;
                        PickingPos currentPickingPos = null;
                        DeliveryNotePos currentDeliveryNotePos = null;
                        if (currentPicking != null)
                        {
                            currentPickingPos = currentPicking.PickingPos_Picking.FirstOrDefault();
                            dbApp.Detach(currentPicking);
                            if (currentPickingPos != null)
                                dbApp.Detach(currentPickingPos);
                        }

                        currentDeliveryNotePos = entity as DeliveryNotePos;
                        if (currentDeliveryNotePos != null)
                            dbApp.Detach(currentDeliveryNotePos);

                        FacilityBooking currentFacilityBooking = entity as FacilityBooking;
                        if (currentFacilityBooking != null)
                            dbApp.Detach(currentFacilityBooking);

                        using (ACMonitor.Lock(_20015_LockValue))
                        {
                            _CurrentPicking = currentPicking;
                            _CurrentPickingPos = currentPickingPos;
                            _CurrentDeliveryNotePos = currentDeliveryNotePos;
                            _CurrentFacilityBooking = currentFacilityBooking;
                        }
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

        protected virtual void ACClassTaskQueue_ChangesSaved(object sender, ACChangesEventArgs e)
        {
            ACClassTaskQueue.TaskQueue.ObjectContext.ACChangesExecuted -= ACClassTaskQueue_ChangesSaved;
            if (_NewAddedProgramLog != null)
            {
                var currentPickingPos = CurrentPickingPos;
                gip.core.datamodel.ACProgramLog newAddedProgramLog = _NewAddedProgramLog;
                if (currentPickingPos != null)
                {
                    this.ApplicationManager.ApplicationQueue.Add(() =>
                    //ThreadPool.QueueUserWorkItem((object state) =>
                    {
                        using (DatabaseApp dbApp = new DatabaseApp())
                        {
                            OrderLog orderLog = OrderLog.NewACObject(dbApp, newAddedProgramLog);
                            orderLog.PickingPosID = currentPickingPos.PickingPosID;
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
        #endregion

        #region Overrides

        protected override void OnRebuildMandatoryConfigStoresCache(IACComponentPWNode invoker, List<IACConfigStore> mandatoryConfigStores, bool recalcExpectedConfigStoresCount)
        {
            base.OnRebuildMandatoryConfigStoresCache(invoker, mandatoryConfigStores, recalcExpectedConfigStoresCount);

            Guid acClassMethodID = invoker != null ? invoker.ContentACClassWF.ACClassMethodID : ContentACClassWF.ACClassMethodID;
            if (CurrentPicking != null)
            {
                ConfigManagerIPlus serviceInstance = ConfigManagerIPlus.GetServiceInstance(this);

                string errorMessage = null;
                int expectedConfigStoresCount = 0;

                List<IACConfigStore> pickingOfflineList = (serviceInstance as ConfigManagerIPlusMES).GetPickingConfigStoreOfflineList(ContentTask.ACClassTaskID, CurrentPicking.PickingID,
                                                                                                                                      out expectedConfigStoresCount, out errorMessage);                if (pickingOfflineList != null)
                {
                    mandatoryConfigStores.AddRange(pickingOfflineList);
                }
                else
                {
                    ProcessAlarm.ValueT = PANotifyState.AlarmOrFault;
                    if (String.IsNullOrEmpty(errorMessage))
                        errorMessage = "";
                    errorMessage = errorMessage + " Configuration could not be loaded. Workflownodes will run with wrong parameters. If you acknowledge the alarm, the workflow will continue otherwise reset the Workflow manually!";
                    OnNewAlarmOccurred(ProcessAlarm, new Msg(errorMessage, this, eMsgLevel.Error, PWClassName, "MandatoryConfigStores", 1000), true);
                    Messages.LogError(this.GetACUrl(), "Start(0)", errorMessage);
                }
                if (recalcExpectedConfigStoresCount)
                {
                    using (ACMonitor.Lock(_20015_LockStoreList))
                    {
                        _ExpectedConfigStoresCount += expectedConfigStoresCount;
                    }
                }
            }
        }

        #endregion


        #region User-Interaction
        [ACMethodInteraction("", "en{'End current picking order'}de{'Beende aktuellen Kommissionierauftrag'}", 296, true)]
        public virtual void EndPicking()
        {
            if (!IsEnabledEndPicking())
                return;
            if (CurrentPicking != null)
            {
                using (var dbApp = new DatabaseApp())
                {
                    Picking picking = CurrentPicking.FromAppContext<Picking>(dbApp);
                    if (picking != null)
                    {
                        foreach (PickingPos pickingPos in picking.PickingPos_Picking)
                        {
                            if (pickingPos.MDDelivPosLoadState != null && pickingPos.MDDelivPosLoadState.DelivPosLoadState != MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck)
                                pickingPos.MDDelivPosLoadState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();
                        }
                    }
                    dbApp.ACSaveChanges();
                }
            }
            CurrentACSubState = (int) ACSubStateEnum.SMLastBatchEndOrder;
        }

        public virtual bool IsEnabledEndPicking()
        {
            return !((ACSubStateEnum)CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrder) && CurrentPicking != null;
        }

        public static bool AskUserEndPicking(IACComponent acComponent)
        {
            if (acComponent == null)
                return false;
            // TODO Übersetzung:
            return acComponent.Messages.Question(acComponent, "Question50021", Global.MsgResult.Yes) == Global.MsgResult.Yes;
        }
        #endregion


        #region Caching
        //private class TransportTargets
        //{
        //    public Guid[] _LastTargets = null;
        //    public DateTime? _NextTargetQuery = null;
        //    public List<int> _PWMethods = new List<int>();
        //}

        //private static Dictionary<Guid, TransportTargets> _TransportTargetCache = new Dictionary<Guid, TransportTargets>();
        //private static object _TargetCacheLock = new object();

        private void UnregisterFromCachedDestinations()
        {
            try
            {
                //if (_CurrentProdOrderBatch != null && _CurrentProdOrderBatch.ProdOrderBatchPlanID.HasValue)
                //{
                //    Guid batchPlanID = _CurrentProdOrderBatch.ProdOrderBatchPlanID.Value;
                //    lock (_TargetCacheLock)
                //    {
                //        TransportTargets targetCache = null;
                //        if (_TransportTargetCache.TryGetValue(batchPlanID, out targetCache))
                //        {
                //            int myHashCode = this.GetHashCode();
                //            if (targetCache._PWMethods.Contains(myHashCode))
                //                targetCache._PWMethods.Remove(myHashCode);
                //            if (targetCache._PWMethods.Count <= 0)
                //                _TransportTargetCache.Remove(batchPlanID);
                //        }
                //    }
                //}
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("PWMethodTransportBase", "UnregisterFromCachedDestinations", msg);
            }
        }

        //public Guid[] GetCachedDestinations(bool refreshCache, out short errorCode, out string errorMsg)
        //{
        //    errorMsg = null;
        //    try
        //    {
        //        TransportTargets targetCache = null;
        //        Guid batchPlanID = CurrentProdOrderBatch.ProdOrderBatchPlanID.Value;
        //        lock (_TargetCacheLock)
        //        {
        //            if (!_TransportTargetCache.TryGetValue(batchPlanID, out targetCache))
        //            {
        //                targetCache = new TransportTargets() { _NextTargetQuery = DateTime.Now.AddSeconds(20) };
        //                _TransportTargetCache.Add(batchPlanID, targetCache);
        //            }
        //            int myHashCode = this.GetHashCode();
        //            if (!targetCache._PWMethods.Contains(myHashCode))
        //                targetCache._PWMethods.Add(myHashCode);
        //        }

        //        if (targetCache._NextTargetQuery == null || DateTime.Now > targetCache._NextTargetQuery.Value || targetCache._LastTargets == null || refreshCache)
        //        {
        //            using (var dbApp = new DatabaseApp())
        //            {
        //                ProdOrderPartslistPos currentBatchPos = CurrentProdOrderPartslistPos.FromAppContext<ProdOrderPartslistPos>(dbApp);
        //                var batchPlan = dbApp.ProdOrderBatchPlan.Where(c => c.ProdOrderBatchPlanID == batchPlanID).FirstOrDefault();
        //                if (batchPlan == null)
        //                {
        //                    errorCode = 1;
        //                    return null;
        //                }
        //                IList<FacilityReservation> plannedSilos = batchPlan.FacilityReservation_ProdOrderBatchPlan.OrderBy(c => c.Sequence).ToArray();
        //                if (plannedSilos == null || !plannedSilos.Any())
        //                {
        //                    errorCode = 2;
        //                    return null;
        //                }

        //                PWDischarging lastDischargingNode = null;
        //                var disNodes = FindChildComponents<PWDischarging>(c => c is PWDischarging);
        //                if (disNodes != null && disNodes.Any())
        //                {
        //                    lastDischargingNode = disNodes.Where(c => c.IsLastDischargingNode).FirstOrDefault();
        //                    if (lastDischargingNode == null)
        //                    {
        //                        foreach (PWDischarging disNode in disNodes)
        //                        {
        //                            MaterialWFConnection connectionToDischarging = null;
        //                            if (disNode.IsLastDisNode(dbApp, batchPlan, out connectionToDischarging))
        //                            {
        //                                lastDischargingNode = disNode;
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    if (lastDischargingNode == null)
        //                    {
        //                        var endNode = FindChildComponents<PWNodeEnd>(c => c is PWNodeEnd, 1).FirstOrDefault();
        //                        if (endNode != null)
        //                        {
        //                            var sourceGroups = endNode.PWPointIn.ConnectionList
        //                                            .Where(c => c.ValueT is PWGroup)
        //                                            .Select(c => c.ValueT as PWGroup).ToList();
        //                            foreach (var pwGroup in sourceGroups)
        //                            {
        //                                lastDischargingNode = pwGroup.FindChildComponents<PWDischarging>(c => c is PWDischarging).FirstOrDefault();
        //                                if (lastDischargingNode != null)
        //                                    break;
        //                            }
        //                        }
        //                    }
        //                    if (lastDischargingNode == null)
        //                        lastDischargingNode = disNodes.FirstOrDefault();
        //                }

        //                FacilityReservation facilityReservation = null;
        //                if (lastDischargingNode != null)
        //                {
        //                    facilityReservation = lastDischargingNode.GetNextFreeDestination(plannedSilos, currentBatchPos, false);
        //                }
        //                else
        //                {
        //                    facilityReservation = PWDischarging.GetNextFreeDestination(this, plannedSilos, currentBatchPos, false);
        //                }

        //                if (facilityReservation != null)
        //                {
        //                    targetCache._LastTargets = new Guid[] { facilityReservation.VBiACClassID.Value };
        //                }
        //                else
        //                {
        //                    targetCache._LastTargets = plannedSilos.OrderBy(c => c.VBiACClassID.Value).Select(c => c.VBiACClassID.Value).ToArray();
        //                }
        //            }
        //            // Alle 20 Sekunden abfragen um Datenbankbelastung zu reduzieren
        //            targetCache._NextTargetQuery = DateTime.Now.AddSeconds(20);
        //        }
        //        errorCode = 0;
        //        return targetCache._LastTargets;
        //    }
        //    catch (Exception e)
        //    {
        //        errorMsg = e.Message;
        //        errorCode = -1;
        //        return null;
        //    }
        //}

        public static bool AreCachedDestinationsDifferent(Guid[] cachedTargets, Guid[] targets)
        {
            return cachedTargets == null
                    || targets == null
                    || !cachedTargets.Any()
                    || cachedTargets.Count() != targets.Count()
                    || !cachedTargets.All(targets.Contains)
                    || !targets.All(cachedTargets.Contains);
        }
        #endregion

        #endregion
    }
}
