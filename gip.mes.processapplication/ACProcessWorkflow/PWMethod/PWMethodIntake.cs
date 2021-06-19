using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;

namespace gip.mes.processapplication
{
    /// <summary>
    /// Workflow-Root für Wareneingang
    /// </summary>
    [ACClassConstructorInfo(
    new object[] 
        { 
            new object[] {gip.core.datamodel.ACProgram.ClassName, Global.ParamOption.Required, typeof(Guid)},
            new object[] {gip.core.datamodel.ACProgramLog.ClassName, Global.ParamOption.Optional, typeof(Guid)},
            new object[] {DeliveryNotePos.ClassName, Global.ParamOption.Optional, typeof(Guid)},
            new object[] {Picking.ClassName, Global.ParamOption.Optional, typeof(Guid)},
            new object[] {PickingPos.ClassName, Global.ParamOption.Optional, typeof(Guid)}
        }
    )]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Intake'}de{'Wareneingang'}", Global.ACKinds.TPWMethod, Global.ACStorableTypes.Optional, true, true, "", "ACProgram", 20)]
    public class PWMethodIntake : PWMethodTransportBase
    {
        new public const string PWClassName = "PWMethodIntake";

        #region c´tors
        static PWMethodIntake()
        {
            RegisterExecuteHandler(typeof(PWMethodIntake), HandleExecuteACMethod_PWMethodIntake);
        }

        public PWMethodIntake(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            _LabOrderManager = ACLabOrderManager.ACRefToServiceInstance(this);
            if (_LabOrderManager == null)
                throw new Exception("LabOrderManager not configured");
            _InDeliveryNoteManager = ACInDeliveryNoteManager.ACRefToServiceInstance(this);
            if (_InDeliveryNoteManager == null)
                throw new Exception("ACInDeliveryNoteManager not configured");
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            ACLabOrderManager.DetachACRefFromServiceInstance(this, _LabOrderManager);
            _LabOrderManager = null;
            ACInDeliveryNoteManager.DetachACRefFromServiceInstance(this, _InDeliveryNoteManager);
            _InDeliveryNoteManager = null;
            UnregisterFromCachedDestinationsForDN();


            using (ACMonitor.Lock(_20015_LockValue))
            {
                _CurrentDeliveryNotePos = null;
            }

            lock (_TargetCacheLock)
            {
                _DeliveryNoteTargetCache = new Dictionary<Guid, DeliveryNoteTargets>();
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
            }


            lock (_TargetCacheLock)
            {
                _DeliveryNoteTargetCache = new Dictionary<Guid, DeliveryNoteTargets>();
            }

            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }
        #endregion

        #region Properties

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

        protected ACRef<ACInDeliveryNoteManager> _InDeliveryNoteManager = null;
        public ACInDeliveryNoteManager InDeliveryNoteManager
        {
            get
            {
                if (_InDeliveryNoteManager == null)
                    return null;
                return _InDeliveryNoteManager.ValueT as ACInDeliveryNoteManager;
            }
        }



        private gip.mes.datamodel.DeliveryNotePos _CurrentDeliveryNotePos = null;
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


        #endregion

        #region Methods

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PWMethodIntake(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWMethodTransportBase(out result, acComponent, acMethodName, acClassMethod, acParameter);
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

        protected override void LoadVBEntities()
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
                if (_CurrentPicking != null || _CurrentDeliveryNotePos != null)
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
                            Guid pickingPosID = PWDischarging.GetPickingPosIDFromACMethod(acMethod);
                            if (pickingPosID != Guid.Empty)
                                currentPickingPos = currentPicking.PickingPos_Picking.Where(c => c.PickingPosID == pickingPosID).FirstOrDefault();
                            if (currentPickingPos == null)
                                currentPickingPos = currentPicking.PickingPos_Picking.Where(c => c.MDDelivPosLoadStateID.HasValue
                                                                && (c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive
                                                                    || c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad)).FirstOrDefault();
                            if (currentPickingPos == null)
                                currentPickingPos = currentPicking.PickingPos_Picking.FirstOrDefault();
                            dbApp.Detach(currentPicking);
                            if (currentPickingPos != null)
                                dbApp.Detach(currentPickingPos);
                        }

                        currentDeliveryNotePos = entity as DeliveryNotePos;
                        if (currentDeliveryNotePos != null)
                            dbApp.Detach(currentDeliveryNotePos);

                        using (ACMonitor.Lock(_20015_LockValue))
                        {
                            _CurrentPicking = currentPicking;
                            _CurrentPickingPos = currentPickingPos;
                            _CurrentDeliveryNotePos = currentDeliveryNotePos;
                        }

                    }
                }
            }
        }

        protected override void ACClassTaskQueue_ChangesSaved(object sender, ACChangesEventArgs e)
        {
            if (CurrentDeliveryNotePos != null)
            {
                ACClassTaskQueue.TaskQueue.ObjectContext.ACChangesExecuted -= ACClassTaskQueue_ChangesSaved;
                if (_NewAddedProgramLog != null)
                {
                    this.ApplicationManager.ApplicationQueue.Add(() =>
                    //ThreadPool.QueueUserWorkItem((object state) =>
                    {
                        using (DatabaseApp dbApp = new DatabaseApp())
                        {
                            OrderLog orderLog = OrderLog.NewACObject(dbApp, _NewAddedProgramLog);
                            orderLog.DeliveryNotePosID = CurrentDeliveryNotePos.DeliveryNotePosID;
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
            {
                base.ACClassTaskQueue_ChangesSaved(sender, e);
            }
        }

        private class DeliveryNoteTargets
        {
            public Guid[] _LastTargets = null;
            public DateTime? _NextTargetQuery = null;
            public List<int> _PWMethods = new List<int>();
        }

        private static Dictionary<Guid, DeliveryNoteTargets> _DeliveryNoteTargetCache = new Dictionary<Guid, DeliveryNoteTargets>();
        private static object _TargetCacheLock = new object();

        private void UnregisterFromCachedDestinationsForDN()
        {
            try
            {
                DeliveryNotePos currentDeliveryNotePos = null;

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    currentDeliveryNotePos = _CurrentDeliveryNotePos;
                }

                if (currentDeliveryNotePos != null)
                {
                    Guid notePosID = currentDeliveryNotePos.DeliveryNotePosID;
                    lock (_TargetCacheLock)
                    {
                        DeliveryNoteTargets targetCache = null;
                        if (_DeliveryNoteTargetCache.TryGetValue(notePosID, out targetCache))
                        {
                            int myHashCode = this.GetHashCode();
                            if (targetCache._PWMethods.Contains(myHashCode))
                                targetCache._PWMethods.Remove(myHashCode);
                            if (targetCache._PWMethods.Count <= 0)
                                _DeliveryNoteTargetCache.Remove(notePosID);
                        }
                    }
                }
            }
            catch (Exception ec)
            {
                Messages.LogException("PWMethodIntake", "UnregisterFromCachedDestinationsForDN", ec);
            }
        }

        public Guid[] GetCachedDestinationsForDN(bool refreshCache, out short errorCode, out string errorMsg)
        {
            errorMsg = null;
            try
            {
                if (CurrentDeliveryNotePos == null)
                {
                    errorCode = 1;
                    return null;
                }

                DeliveryNoteTargets targetCache = null;
                Guid notePosID = CurrentDeliveryNotePos.DeliveryNotePosID;
                lock (_TargetCacheLock)
                {
                    if (!_DeliveryNoteTargetCache.TryGetValue(notePosID, out targetCache))
                    {
                        targetCache = new DeliveryNoteTargets() { _NextTargetQuery = DateTime.Now.AddSeconds(20) };
                        _DeliveryNoteTargetCache.Add(notePosID, targetCache);
                    }
                    int myHashCode = this.GetHashCode();
                    if (!targetCache._PWMethods.Contains(myHashCode))
                        targetCache._PWMethods.Add(myHashCode);
                }

                if (targetCache._NextTargetQuery == null || DateTime.Now > targetCache._NextTargetQuery.Value || targetCache._LastTargets == null || refreshCache)
                {
                    using (var db = new Database())
                    using (var dbApp = new DatabaseApp())
                    {
                        DeliveryNotePos dnPos = CurrentDeliveryNotePos.FromAppContext<DeliveryNotePos>(dbApp);
                        if (dnPos == null)
                        {
                            errorCode = 1;
                            return null;
                        }

                        var queryPosSameMaterial = dbApp.DeliveryNotePos.Where(c => c.DeliveryNoteID == dnPos.DeliveryNoteID
                            && c.InOrderPosID.HasValue
                            && c.InOrderPos.MaterialID == dnPos.InOrderPos.MaterialID
                            && c.InOrderPos.MDDelivPosLoadStateID.HasValue
                            && (c.InOrderPos.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad
                            || c.InOrderPos.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive
                            || c.InOrderPos.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.BatchActive))
                            .OrderBy(c => c.Sequence);
                        double targetQuantity = queryPosSameMaterial.Sum(c => c.InOrderPos.TargetQuantityUOM);

                        IList<gip.core.datamodel.ACClass> selectedModules = ACFacilityManager.GetSelectedModulesAsACClass(dnPos, db);
                        if (selectedModules == null || !selectedModules.Any())
                        {
                            errorCode = 2;
                            return null;
                        }

                        foreach (var module in selectedModules)
                        {
                            IList<FacilityReservation> plannedSilos = ACFacilityManager.GetSelectedTargets(dnPos, module);
                            if (plannedSilos == null || !plannedSilos.Any())
                            {
                                errorCode = 2;
                                return null;
                            }

                            // TODO 10001: Find last PWDischarging-Node to invoke GetNextFreeDestination()-Method, to ensure, that virtual method is called and not Standadr-Static-Method
                            PWDischarging lastDischargingNode = null;
                            FacilityReservation facilityReservation = null;
                            if (lastDischargingNode != null)
                            {
                                facilityReservation = lastDischargingNode.GetNextFreeDestination(plannedSilos, dnPos, targetQuantity);
                            }
                            else
                            {
                                facilityReservation = PWDischarging.GetNextFreeDestination(this, plannedSilos, dnPos, targetQuantity);
                            }

                            if (facilityReservation != null)
                            {
                                targetCache._LastTargets = new Guid[] { facilityReservation.VBiACClassID.Value };
                            }
                            else
                            {
                                targetCache._LastTargets = plannedSilos.OrderBy(c => c.VBiACClassID.Value).Select(c => c.VBiACClassID.Value).ToArray();
                            }
                            break;
                        }
                    }
                    // Alle 20 Sekunden abfragen um Datenbankbelastung zu reduzieren
                    targetCache._NextTargetQuery = DateTime.Now.AddSeconds(20);
                }
                errorCode = 0;
                return targetCache._LastTargets;
            }
            catch (Exception e)
            {
                errorMsg = e.Message;
                errorCode = -1;
                return null;
            }
        }

        #endregion

        #endregion

    }
}
