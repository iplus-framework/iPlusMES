using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using System.Xml;
using gip.mes.facility;
using static gip.mes.datamodel.MDReservationMode;
using static gip.mes.processapplication.PWDosing;
using gip.core.processapplication;
using DocumentFormat.OpenXml.ExtendedProperties;
using System.Diagnostics;

namespace gip.mes.processapplication

{
    /// <summary>
    ///   <para>
    ///   PWGroup-Implementation:
    ///   <br /> 
    /// PWGroup is on the one hand a derivative of PWBaseExecutable and on the other hand it implements the IACComponentPWGroup interface. </para>
    ///   <para>Like a PWProcessFunction, it is therefore able to have child workflow nodes. 
    ///   It contains a start and end node (PWNodeStart and PWNodeEnd) and should contain at least one node of the type PWNodeProcessMethod. 
    ///   This is because PWNodeProcessMethod-classes call PAProcessFunction's asynchronously in the physical model. 
    ///   These calls are only allowed, when the associated process module has been occupied by the PWGroup instance by setting the "PAProcessModule.Semaphore" service point with the "PWGroup.TrySemaphore" client point. 
    ///   The occupation of a process module takes place in the state "SMStarting" and will be, after all child workflow nodes have been processed and the end node has triggered, removed. 
    ///   The ACState of PWGroup will than be switched back from "SMRunning" back to the base state "SMIdle".
    /// </para>
    ///   <para>
    ///   Additional Logic in PWGroupVB:
    ///   <br /> 
    ///   It reads the MES-Tables (Productionorder, Deliverynote or Pickingorders) to determine the final destinations an removes processmodules that coudn't be used to reach them. 
    ///   Furthermore it skips the occupation of processmodules depending on the data in these MES-Tables. e.g. if there is no material to process.
    /// </para>
    /// </summary>
    /// <seealso cref="gip.core.autocomponent.PWGroup" />
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Workflow group'}de{'Workflow Gruppe'}", Global.ACKinds.TPWGroup, Global.ACStorableTypes.Optional, false, PWProcessFunction.PWClassName, true)]
    public class PWGroupVB : PWGroup
    {
        new public const string PWClassName = "PWGroupVB";
        public const string SelRuleID_ReachableDest = "PWGroupVB_RDest";
        public const string SelRuleID_ReachableSource = "PWGroupVB_RSource";
        public const string SelRuleID_IsDestDirectSucessor = "PWGroupVB_IsDDS";

        #region c´tors
        static PWGroupVB()
        {
            List<ACMethodWrapper> wrappers = ACMethod.OverrideFromBase(typeof(PWGroupVB), ACStateConst.SMStarting);
            if (wrappers != null)
            {
                foreach (ACMethodWrapper wrapper in wrappers)
                {
                    wrapper.Method.ParameterValueList.Add(new ACValue("SkipIfNoComp", typeof(bool), false, Global.ParamOption.Required));
                    wrapper.ParameterTranslation.Add("SkipIfNoComp", "en{'Skip if no material has to be processed'}de{'Überspringe wenn kein Material verarbeitet werden muss'}");
                    wrapper.Method.ParameterValueList.Add(new ACValue("MaxBatchWeight", typeof(double), false, Global.ParamOption.Optional));
                    wrapper.ParameterTranslation.Add("MaxBatchWeight", "en{'Max. batch weight [kg]'}de{'Maximales Batchgewicht [kg]'}");
                    wrapper.Method.ParameterValueList.Add(new ACValue("SkipPredCount", typeof(short), 0, Global.ParamOption.Optional));
                    wrapper.ParameterTranslation.Add("SkipPredCount", "en{'Count of dosing nodes to find (Predecessors)'}de{'Anzahl zu suchender Dosierknoten (Vorgänger)'}");
                    wrapper.Method.ParameterValueList.Add(new ACValue("DosableOnGroupCheck", typeof(bool), false, Global.ParamOption.Required));
                    wrapper.ParameterTranslation.Add("DosableOnGroupCheck", "en{'Only process modules where Materials can be processed'}de{'Nur Prozessmodule wo Material verarbeitet werden kann'}");
                    wrapper.Method.ParameterValueList.Add(new ACValue("ReserveModule", typeof(ushort), 0, Global.ParamOption.Optional));
                    wrapper.ParameterTranslation.Add("ReserveModule", "en{'Reserve occupied module for next batch (1=An;2=Ignore)'}de{'Reserviere belegtes Prozessmodul für nächsten Batch (1=An;2=Ignoriere)'}");
                }
            }
            RegisterExecuteHandler(typeof(PWGroupVB), HandleExecuteACMethod_PWGroupVB);

            ACRoutingService.RegisterSelectionQuery(SelRuleID_ReachableDest, 
                (c, p) =>      c.ComponentInstance is PAProcessModule
                            && (p[0] as Guid[]).Contains(c.ComponentInstance.ComponentClass.ACClassID),
                (c, p) =>   !(p[1] as Guid[]).Contains(c.ComponentInstance.ComponentClass.ACClassID)
                            && (c.ComponentInstance is PAMSilo || c.ComponentInstance is PAMParkingspace));

            ACRoutingService.RegisterSelectionQuery(SelRuleID_ReachableSource,
                (c, p) => (c.ComponentInstance is PAMSilo || c.ComponentInstance is PAMParkingspace)
                            && (Guid)p[0] == c.ComponentInstance.ComponentClass.ACClassID,
                (c, p) => c.ComponentInstance is PAMSilo || c.ComponentInstance is PAMParkingspace);

            ACRoutingService.RegisterSelectionQuery(SelRuleID_IsDestDirectSucessor,
                (c, p) => (c.ComponentInstance is PAMSilo || c.ComponentInstance is PAMParkingspace)
                            && (Guid)p[0] == c.ComponentInstance.ComponentClass.ACClassID,
                (c, p) => c.ComponentInstance is PAProcessModule);
        }

        public PWGroupVB(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier) 
        {
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            using (ACMonitor.Lock(_20015_LockValue))
            {
                _LastTargets = null;
                _LastCalculatedRouteablePMList = null;
                _LastModulesInAutomaticMode = null;
                _ExtraDisTargetDest = null;
                _ExtraDisTargetComp = null;
                _RoutingCheckWait = null;
            }
            return base.ACDeInit(deleteACClassTask);
        }

        public override void Recycle(IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {

            using (ACMonitor.Lock(_20015_LockValue))
            {
                _LastTargets = null;
                _LastCalculatedRouteablePMList = null;
                _LastModulesInAutomaticMode = null;
                _ExtraDisTargetDest = null;
                _ExtraDisTargetComp = null;
                _RoutingCheckWait = null;
            }
            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }
        #endregion

        #region Properties

        #region Configuration
        public double MaxBatchWeight
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("MaxBatchWeight");
                    if (acValue != null)
                    {
                        return acValue.ParamAsDouble;
                    }
                }
                return 0.0;
            }
        }

        /// <summary>
        /// Configuration:
        /// Prevents the occupation of a process module if there are no materials that could be processed by this worfklow-node.
        /// </summary>
        public bool SkipIfNoComp
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("SkipIfNoComp");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        public bool DosableOnGroupCheck
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("DosableOnGroupCheck");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Reserve occupied module for next batch
        /// </summary>
        public ushort ReserveModule
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("ReserveModule");
                    if (acValue != null)
                    {
                        string value = acValue.ParamAsString;
                        if (String.IsNullOrEmpty(value) || value.ToLower() == "false")
                            return 0;
                        else if (value.ToLower() == "true")
                            return 1;
                        return acValue.ParamAsUInt16;
                    }
                }
                return 0;
            }
        }
        #endregion


        #region MES-Properties
        public T ParentPWMethod<T>() where T : PWMethodVBBase
        {
            if (ParentRootWFNode == null)
                return null;
            return ParentRootWFNode as T;
        }

        public bool IsProduction
        {
            get
            {
                return ParentPWMethod<PWMethodProduction>() != null;
            }
        }

        public bool IsIntake
        {
            get
            {
                return ParentPWMethod<PWMethodIntake>() != null;
            }
        }

        public bool IsLoading
        {
            get
            {
                return ParentPWMethod<PWMethodLoading>() != null;
            }
        }

        public bool IsRelocation
        {
            get
            {
                return ParentPWMethod<PWMethodRelocation>() != null;
            }
        }

        public bool IsTransport
        {
            get
            {
                return ParentPWMethod<PWMethodTransportBase>() != null;
            }
        }
        #endregion


        #region Occupation of Processmodules and Routing
        private Guid[] _LastTargets = null;
        /// <summary>
        /// Cache, which destinations were chek last time, to be able to compare if user has changed the destinations in the picking/btrachplan targets
        /// If changed, then a routing check must be activated
        /// </summary>
        protected Guid[] LastTargets
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_LastTargets == null)
                        return null;
                    return _LastTargets.ToArray();
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _LastTargets = value;
                }
            }
        }

        private SafeList<PAProcessModule> _LastCalculatedRouteablePMList = null;
        /// <summary>
        /// Cache with Process-Modules which can be potentially mapped/accessed from this PWGroup beacuse they are routable to the planned destination
        /// </summary>
        protected SafeList<PAProcessModule> LastCalculatedRouteablePMList
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _LastCalculatedRouteablePMList;
                }
            }
            set
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _LastCalculatedRouteablePMList = value;
                }
            }
        }

        private List<PAProcessModule> _LastModulesInAutomaticMode = null;
        /// <summary>
        /// Cache of Processmodules, that have been checked last time an were in automatic mode. 
        /// It's needed to be able to compare if somebody has switched to manual or automatic mode an therefore an new routing check to the destinations are needed 
        /// </summary>
        protected List<PAProcessModule> LastModulesInAutomaticMode
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_LastModulesInAutomaticMode == null)
                        return null;
                    return _LastModulesInAutomaticMode.ToList();
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _LastModulesInAutomaticMode = value;
                }
            }
        }

        private DateTime? _RoutingCheckWait = null;
        public bool IsInRoutingCheckWaitNotElapsed
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _RoutingCheckWait.HasValue && _RoutingCheckWait.Value > DateTime.Now;
                }
            }
        }

        public bool IsInRoutingCheckWait
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _RoutingCheckWait.HasValue;
                }
            }
        }

        public void ResetRoutingCheckWait()
        {
            using (ACMonitor.Lock(_20015_LockValue))
            {
                _RoutingCheckWait = null;
            }
        }

        public void SetRoutingCheckWait()
        {
            using (ACMonitor.Lock(_20015_LockValue))
            {
                _RoutingCheckWait = DateTime.Now.AddSeconds(30);
            }
        }

        public override void ClearMyConfiguration()
        {
            ResetRoutingCheckWait();
            base.ClearMyConfiguration();
        }

        public override void SubscribeToProjectWorkCycle()
        {
            if (!IsSubscribedToWorkCycle)
                ResetRoutingCheckWait();
            base.SubscribeToProjectWorkCycle();
        }

        /// <summary>
        ///   <para>
        /// PWGroup-Implementation:
        /// <br /> ProcessModuleList removes all entries from PossibleModuleList that aren't in automatic mode and doesn't match the routing rules.
        /// For applying the routing rules it reads the overridable configuration stores (MandatoryConfigStores) first.
        /// Afterwards it removes all processmodules that can't be reached from preceding processmodules if the property "RoutingCheck" is set.
        /// </para>
        ///   <para> Additional Logic in PWGroupVB:
        /// <br /> Tests if there are no child nodes of that implement IPWNodeDeliverMaterial than ProcessModuleList from the base (PWGroup) is returned.
        /// Otherwise, it reads the final destinations that are stored in a BatchPlan or a DeliveryNote or a Picking-Order and removes all processmodules
        /// that couldn't be used to reach the final destination. To achieve this ACRoutingService.FindSuccessors() is called to test the routing possibilities.
        /// </para>
        /// </summary>
        /// <value>A reduced list of process modules that can be used to reach the final destination.</value>
        public override List<PAProcessModule> ProcessModuleList
        {
            get
            {
                if (ApplicationManager == null || ContentACClassWF == null || !ContentACClassWF.RefPAACClassID.HasValue)
                    return null;
                List<PAProcessModule> modulesInAutomaticMode = base.ProcessModuleList;
                if (ReserveModule != 2)
                    modulesInAutomaticMode = modulesInAutomaticMode.Where(c => c.ReservationInfo == null || String.IsNullOrEmpty(c.ReservationInfo.ValueT) || c.ReservationInfo.ValueT == this.CurrentProgramNo).ToList();
                if (ReserveModule == 1)
                    modulesInAutomaticMode = modulesInAutomaticMode.OrderByDescending(c => c.ReservationInfoSortString).ToList();

                try
                {
                    if (modulesInAutomaticMode == null
                        || !modulesInAutomaticMode.Any()
                        // Falls Gruppe keine Materialweiterleitungsschritte besitzt, dann keine Überpüfung der Routen:
                        || (!this.FindChildComponents<IPWNodeDeliverMaterial>(c => c is IPWNodeDeliverMaterial).Any() && !DosableOnGroupCheck))
                        return modulesInAutomaticMode;

                    List<PAProcessModule> removedModules = null;
                    List<PAProcessModule> addedModules = null;

                    var lastModulesInAutomaticMode = LastModulesInAutomaticMode;
                    if (lastModulesInAutomaticMode != null)
                    {
                        removedModules = lastModulesInAutomaticMode.Except(modulesInAutomaticMode).ToList();
                        addedModules = modulesInAutomaticMode.Except(lastModulesInAutomaticMode).ToList();
                    }
                    else
                    {
                        removedModules = new List<PAProcessModule>();
                        addedModules = modulesInAutomaticMode.ToList();
                    }
                    LastModulesInAutomaticMode = modulesInAutomaticMode;

                    var lastCalculatedRouteablePMList = LastCalculatedRouteablePMList;
                    // Falls Routing-Rule während der Laufzeit verändert worden ist, dann zuletzt ermittelte Prozessmodulliste erneuern
                    if (lastCalculatedRouteablePMList != null && removedModules.Any())
                        lastCalculatedRouteablePMList.RemoveAll(removedModules.Contains);

                    // Überprüfe ob über die möglichen Module ein Weg zu dem geplanten Ziel möglich ist
                    if (IsProduction
                        && ParentPWMethod<PWMethodProduction>().CurrentProdOrderPartslistPos != null
                        && ParentPWMethod<PWMethodProduction>().CurrentProdOrderBatch != null)
                    {
                        var pmListForPicking = HandleModuleListForProdOrder(modulesInAutomaticMode, removedModules, addedModules);
                        return pmListForPicking;
                    }
                    else if (IsTransport)
                    {
                        var pwMethod = ParentPWMethod<PWMethodTransportBase>();
                        if (pwMethod != null)
                        {
                            List<PAProcessModule> cachedList = null;
                            if (pwMethod.CurrentPicking != null)
                                cachedList = GetCachedModulesForPickingIfWaiting(modulesInAutomaticMode, removedModules, addedModules);
                            else if (pwMethod.CurrentDeliveryNotePos != null && IsIntake)
                            {
                                Guid[] targets;
                                bool targetsChanged = false;
                                cachedList = GetCachedModulesForDNoteIfWaiting(pwMethod as PWMethodIntake, modulesInAutomaticMode, removedModules, addedModules, out targetsChanged, out targets);
                                if (targets == null && cachedList == null)
                                    cachedList = new List<PAProcessModule>();
                            }
                            if (cachedList != null)
                                return cachedList;

                            using (var db = new Database())
                            using (var dbApp = new DatabaseApp(db))
                            {
                                Picking picking = null;
                                PickingPos pickingPos = null;
                                DeliveryNotePos notePos = null;
                                FacilityBooking fBooking = null;
                                if (pwMethod.CurrentPicking != null)
                                    picking = pwMethod.CurrentPicking.FromAppContext<Picking>(dbApp);
                                if (pwMethod.CurrentPickingPos != null)
                                    pickingPos = pwMethod.CurrentPickingPos.FromAppContext<PickingPos>(dbApp);
                                if (pwMethod.CurrentDeliveryNotePos != null)
                                    notePos = pwMethod.CurrentDeliveryNotePos.FromAppContext<DeliveryNotePos>(dbApp);
                                if (pwMethod.CurrentFacilityBooking != null)
                                    fBooking = pwMethod.CurrentFacilityBooking.FromAppContext<FacilityBooking>(dbApp);

                                List<PAProcessModule> pmList = null;
                                if (picking != null)
                                    pmList = HandleModuleListForPicking(dbApp, db, picking, pickingPos, modulesInAutomaticMode, removedModules, addedModules);
                                else if (notePos != null && IsIntake)
                                    pmList = HandleModuleListForDNote(pwMethod as PWMethodIntake, dbApp, db, notePos, modulesInAutomaticMode, removedModules, addedModules);
                                else if (fBooking != null)
                                    pmList = HandleModuleListForBooking(dbApp, db, fBooking, modulesInAutomaticMode, removedModules, addedModules);
                                if (pmList != null)
                                    return pmList;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    modulesInAutomaticMode = new List<PAProcessModule>();
                    Messages.LogException(this.GetACUrl(), "ProcessModuleList", ex);
                }
                return modulesInAutomaticMode;
            }
        }

        protected virtual List<PAProcessModule> HandleModuleListForProdOrder(List<PAProcessModule> modulesInAutomaticMode, List<PAProcessModule> removedModules, List<PAProcessModule> addedModules)
        {
            short destError = 0;
            string errorMsg = "";
            Guid[] targets = null;
            if (IsInEmptyingMode)
            {
                var extraDest = ExtraDisTargetComp;
                if (extraDest != null)
                    targets = new Guid[] { extraDest.ComponentClass.ACClassID };
            }
            if (targets == null)
                targets = ParentPWMethod<PWMethodProduction>().GetCachedDestinations(false/*addedModules.Any()*/, out destError, out errorMsg);
            if (targets == null || !targets.Any())
            {
                if (ParentPWMethod<PWMethodProduction>().CurrentProdOrderBatch.ProdOrderBatchPlanID.HasValue)
                {
                    if (destError == -1)
                    {
                        Msg msg = new Msg(errorMsg, this, eMsgLevel.Error, PWClassName, "ProcessModuleList", 1010);
                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    }
                    else if (destError == 1)
                    {
                        // Error50150: Batchplan is empty.
                        Msg msg = msg = new Msg(this, eMsgLevel.Error, PWClassName, "ProcessModuleList(2)", 1020, "Error50150");
                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    }
                    else if (destError == 2)
                    {
                        // Error00126: No route found to planned destination
                        Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "ProcessModuleList(3)", 1030, "Error00126");
                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    }
                    return new List<PAProcessModule>();
                }
                else
                    return modulesInAutomaticMode;
            }

            if (targets != null && targets.Any())
            {
                bool targetsChanged = PWMethodTransportBase.AreCachedDestinationsDifferent(LastTargets, targets);
                if (!targetsChanged
                    && LastCalculatedRouteablePMList != null
                    && (removedModules == null || !removedModules.Any())
                    && (addedModules == null || !addedModules.Any())
                    && IsInRoutingCheckWaitNotElapsed)
                    return LastCalculatedRouteablePMList.ToList();

                var pwDosings = this.FindChildComponents<IPWNodeReceiveMaterialRouteable>(c => c is IPWNodeReceiveMaterialRouteable);
                if (targetsChanged)
                    LastCalculatedRouteablePMList = new SafeList<PAProcessModule>();
                if (LastCalculatedRouteablePMList == null)
                    LastCalculatedRouteablePMList = new SafeList<PAProcessModule>();
                LastTargets = targets;
                SetRoutingCheckWait();

                Type typeOfSilo = typeof(PAMSilo);
                Guid thisMethodID = ContentACClassWF.ACClassMethodID;
                using (var db = new Database())
                {
                    foreach (var module in modulesInAutomaticMode)
                    {
                        if (!LastCalculatedRouteablePMList.Contains(module))
                        {
                            Guid[] allPossibleModulesinThisWF = RootPW.GetAllRoutableModules().Select(c => c.ComponentClass.ACClassID).ToArray();

                            ACRoutingParameters routingParameters = GetRoutingParametersForProdOrder(db, targets, allPossibleModulesinThisWF, typeOfSilo, thisMethodID);

                            RoutingResult rResult = ACRoutingService.FindSuccessors(module.GetACUrl(), routingParameters);
                            if (rResult.Routes != null && rResult.Routes.Any())
                            {
                                bool canAdd = true;
                                if (DosableOnGroupCheck)
                                {
                                    if (pwDosings != null && pwDosings.Any())
                                    {
                                        canAdd = false;
                                        foreach (IPWNodeReceiveMaterialRouteable pwDosing in pwDosings)
                                        {
                                            canAdd = pwDosing.HasAndCanProcessAnyMaterial(module);
                                            if (canAdd)
                                                break;
                                        }
                                    }
                                }

                                if (canAdd)
                                    LastCalculatedRouteablePMList.Add(module);
                            }
                        }
                    }
                }
                if (!LastCalculatedRouteablePMList.Any())
                {
                    // Error00126: No route found to planned destination
                    Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "ProcessModuleList(1)", 1040, "Error00126");
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                }
            }
            else
            {
                LastTargets = null;
                LastCalculatedRouteablePMList = new SafeList<PAProcessModule>();
            }

            return LastCalculatedRouteablePMList.ToList();
        }

        protected virtual ACRoutingParameters GetRoutingParametersForProdOrder(Database db, Guid[] targets, Guid[] allPossibleModulesinThisWF, Type typeOfSilo, Guid thisMethodID)
        {
            return  new ACRoutingParameters()
            {
                RoutingService = this.RoutingService,
                Database = db,
                SelectionRuleID = SelRuleID_ReachableDest,
                Direction = RouteDirections.Forwards,
                SelectionRuleParams = new object[] { targets, allPossibleModulesinThisWF },
                DBSelector = (c, p, r) => targets.Contains(c.ACClassID),
                DBDeSelector = (c, p, r) => (c.ACKind == Global.ACKinds.TPAProcessModule
                                            && (typeOfSilo.IsAssignableFrom(c.ObjectType)
                                                || !c.BasedOnACClassID.HasValue
                                                || (c.BasedOnACClassID.HasValue && !c.ACClass1_BasedOnACClass.ACClassWF_RefPAACClass.Where(refc => refc.ACClassMethodID == thisMethodID).Any()))),
                MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                IncludeReserved = true,
                IncludeAllocated = true
            };
        }

        protected List<PAProcessModule> GetCachedModulesForPickingIfWaiting(List<PAProcessModule> modulesInAutomaticMode, List<PAProcessModule> removedModules, List<PAProcessModule> addedModules)
        {
            Guid[] lastTargets = LastTargets;
            if (lastTargets != null
                && lastTargets.Any()
                && LastCalculatedRouteablePMList != null
                && (removedModules == null || !removedModules.Any())
                && (addedModules == null || !addedModules.Any())
                && IsInRoutingCheckWaitNotElapsed)
                return LastCalculatedRouteablePMList.ToList();
            return null;
        }

        protected virtual List<PAProcessModule> HandleModuleListForPicking(DatabaseApp dbApp, Database db, Picking picking, PickingPos pickingPos, List<PAProcessModule> modulesInAutomaticMode, List<PAProcessModule> removedModules, List<PAProcessModule> addedModules)
        {
            List<PickingPos> openPickings = new List<PickingPos>();
            List<Guid> targets = null;
            // Determine all Targets which should be reached from this module
            // If picking line is not passed when workflow was started, then find the next line which should be processed
            if (pickingPos != null && pickingPos.ToFacility != null && pickingPos.ToFacility.VBiFacilityACClassID.HasValue)
            {
                targets = new List<Guid> { pickingPos.ToFacility.VBiFacilityACClassID.Value };
            }
            else //if (pickingPos == null || pwDosing.DoseAllPosFromPicking)
            {
                targets = new List<Guid>();
                openPickings = dbApp.PickingPos.Include(c => c.FromFacility.FacilityReservation_Facility)
                                                .Include(c => c.FacilityReservation_PickingPos)
                                                .Include(c => c.PickingPosProdOrderPartslistPos_PickingPos)
                                                .Include(c => c.MDDelivPosLoadState)
                                                .Where(c => c.PickingID == picking.PickingID
                                                        && c.MDDelivPosLoadStateID.HasValue
                                                        && (c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad
                                                            || c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive))
                                                .OrderBy(c => c.Sequence)
                                                .ToList();
                foreach (PickingPos item in openPickings)
                {
                    if (item.ToFacility != null && item.ToFacility.VBiFacilityACClassID.HasValue)
                    {
                        if (!targets.Contains(item.ToFacility.VBiFacilityACClassID.Value))
                            targets.Add(item.ToFacility.VBiFacilityACClassID.Value);
                        if (pickingPos == null)
                            pickingPos = item;
                    }
                    else
                    {
                        var subTargets = item.FacilityReservation_PickingPos.Where(c => c.VBiACClassID.HasValue && c.ReservationState != GlobalApp.ReservationState.Finished).Select(c => c.VBiACClassID.Value).ToArray();
                        if (subTargets != null && subTargets.Any())
                        {
                            foreach (var subTarget in subTargets)
                            {
                                if (!targets.Contains(subTarget))
                                    targets.Add((subTarget));
                                if (pickingPos == null)
                                    pickingPos = item;
                            }
                        }
                    }
                }
            }

            if (targets == null || !targets.Any())
            {
                if (LastCalculatedRouteablePMList != null)
                    LastCalculatedRouteablePMList = null;
                LastTargets = null;
                return null;
            }

            List<PWDosing> pwDosings = this.FindChildComponents<PWDosing>(c => c is PWDosing);
            PWDosing pwDosing = pwDosings.FirstOrDefault();
            bool hasGroupDosings = pwDosings.Any();

            Guid[] lastTargets = LastTargets;
            // Has been a calculation in the past? If yes, compare if something has changed since last test. If not, then nothing has also changed accorind the routable Modules
            if (   lastTargets != null 
                && lastTargets.Any() 
                && LastCalculatedRouteablePMList != null
                && lastTargets.Count() == targets.Count()
                && lastTargets.All(targets.Contains)
                && targets.All(lastTargets.Contains)
                && (removedModules == null || !removedModules.Any())
                && (addedModules == null || !addedModules.Any())
                && !hasGroupDosings)
            {
                return LastCalculatedRouteablePMList.ToList();
            }

            LastTargets = targets.ToArray();
            if (LastCalculatedRouteablePMList == null)
                LastCalculatedRouteablePMList = new SafeList<PAProcessModule>();
            SetRoutingCheckWait();

            // If destination was defined in line, then try to find all processmodules from which the destination could be reached.
            Type typeOfSilo = typeof(PAMSilo);
            Guid thisMethodID = ContentACClassWF.ACClassMethodID;
            Guid[] allPossibleModulesinThisWF = RootPW.GetAllRoutableModules().Select(c => c.ComponentClass.ACClassID).ToArray();

            ACRoutingParameters routingParamFindDest = new ACRoutingParameters()
            {
                RoutingService = this.RoutingService,
                Database = db,
                AttachRouteItemsToContext = false,
                SelectionRuleID = SelRuleID_ReachableDest,
                Direction = RouteDirections.Forwards,
                SelectionRuleParams = new object[] { targets.ToArray(), allPossibleModulesinThisWF },
                DBSelector = (c, p, r) => c.ACClassID == pickingPos.ToFacility.VBiFacilityACClassID.Value,
                DBDeSelector = (c, p, r) => (c.ACKind == Global.ACKinds.TPAProcessModule
                                                    && (typeOfSilo.IsAssignableFrom(c.ObjectType)
                                                        || !c.BasedOnACClassID.HasValue
                                                        || (c.BasedOnACClassID.HasValue && !c.ACClass1_BasedOnACClass.ACClassWF_RefPAACClass.Where(refc => refc.ACClassMethodID == thisMethodID).Any()))),
                MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                IncludeReserved = true,
                IncludeAllocated = true
            };

            ACRoutingParameters routingParamFindSource = new ACRoutingParameters()
            {
                RoutingService = this.RoutingService,
                Database = db,
                AttachRouteItemsToContext = false,
                SelectionRuleID = SelRuleID_ReachableSource,
                Direction = RouteDirections.Backwards,
                SelectionRuleParams = new object[] { targets.ToArray(), allPossibleModulesinThisWF },
                DBSelector = (c, p, r) => c.ACClassID == pickingPos.ToFacility.VBiFacilityACClassID.Value,
                DBDeSelector = (c, p, r) => (c.ACKind == Global.ACKinds.TPAProcessModule && (typeOfSilo.IsAssignableFrom(c.ObjectType))),
                MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                IncludeReserved = true,
                IncludeAllocated = true
            };

            if (hasGroupDosings)
            {
                // If more line can be dosed, then ignore passed pickingPos and 
                if (pwDosing.DoseAllPosFromPicking && (openPickings == null || !openPickings.Any()))
                {
                    openPickings = dbApp.PickingPos.Include(c => c.FromFacility.FacilityReservation_Facility)
                                                    .Include(c => c.FacilityReservation_PickingPos)
                                                    .Include(c => c.PickingPosProdOrderPartslistPos_PickingPos)
                                                    .Include(c => c.MDDelivPosLoadState)
                                                    .Where(c => c.PickingID == picking.PickingID
                                                            && c.MDDelivPosLoadStateID.HasValue
                                                            && (c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad
                                                                || c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive))
                                                    .OrderBy(c => c.Sequence)
                                                    .ToList();
                }
                if (!openPickings.Contains(pickingPos))
                    openPickings.Add(pickingPos);
            }

            // Test modules if destination or source could be reached
            foreach (var module in modulesInAutomaticMode)
            {
                if (!LastCalculatedRouteablePMList.Contains(module))
                {
                    string moduleACUrl = module.GetACUrl();

                    // Test if destination reachable
                    RoutingResult rResult = ACRoutingService.FindSuccessors(moduleACUrl, routingParamFindDest);
                    if (rResult.Routes != null && rResult.Routes.Any())
                    {
                        // If group has dosing nodes, test if Dosing is possible
                        if (hasGroupDosings)
                        {
                            rResult = null;
                            foreach (PickingPos pickingPos2 in openPickings)
                            {
                                if (pickingPos2.FromFacility != null && pickingPos2.FromFacility.VBiFacilityACClassID.HasValue)
                                {
                                    routingParamFindSource.DBSelector = (c, p, r) => c.ACClassID == pickingPos2.FromFacility.VBiFacilityACClassID.Value;
                                    routingParamFindSource.SelectionRuleParams = new object[] { pickingPos2.FromFacility.VBiFacilityACClassID.Value };
                                    rResult = ACRoutingService.FindSuccessors(moduleACUrl, routingParamFindSource);
                                }
                                else if (pickingPos2.FromFacility == null)
                                {
                                    if (pwDosing != null)
                                        rResult = pwDosing.HasAndCanProcessAnyMaterialPicking(module, dbApp, db, pickingPos2);
                                }
                                if (rResult != null && rResult.Routes != null && rResult.Routes.Any())
                                    break;
                            }
                        }
                        if (rResult != null && rResult.Routes != null && rResult.Routes.Any())
                            LastCalculatedRouteablePMList.Add(module);
                    }
                }
            }
            if (!LastCalculatedRouteablePMList.Any())
            {
                // Error00126: No route found to planned destination
                Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "ProcessModuleList(2)", 1090, "Error00126");
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
            }

            SetRoutingCheckWait();
            return LastCalculatedRouteablePMList.ToList();

        }

        protected List<PAProcessModule> GetCachedModulesForDNoteIfWaiting(PWMethodIntake pwMethod, List<PAProcessModule> modulesInAutomaticMode, List<PAProcessModule> removedModules, List<PAProcessModule> addedModules, out bool targetsChanged, out Guid[] targets)
        {
            short destError = 0;
            string errorMsg = "";
            targets = null;
            targetsChanged = false;
            if (IsInEmptyingMode)
            {
                var extraDest = ExtraDisTargetComp;
                if (extraDest != null)
                    targets = new Guid[] { extraDest.ComponentClass.ACClassID };
            }
            if (targets == null)
                targets = pwMethod?.GetCachedDestinationsForDN(false/*addedModules.Any()*/, out destError, out errorMsg);
            if (targets == null || !targets.Any())
            {
                if (destError == -1)
                {
                    Msg msg = new Msg(errorMsg, this, eMsgLevel.Error, PWClassName, "ProcessModuleList", 1050);
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                }
                else if (destError == 1)
                {
                    // Error50150: Batchplan is empty.
                    Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "ProcessModuleList(2)", 1060, "Error50150");
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                }
                else if (destError == 2)
                {
                    // Error00126: No route found to planned destination
                    Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "ProcessModuleList(3)", 1070, "Error00126");
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                }
                return null;
            }

            // Performance-Optimization:
            // If targets changed, then recalculate routebale PM List
            targetsChanged = PWMethodTransportBase.AreCachedDestinationsDifferent(LastTargets, targets);
            if (!  targetsChanged
                && LastCalculatedRouteablePMList != null
                && (removedModules == null || !removedModules.Any())
                && (addedModules == null || !addedModules.Any()))
                return LastCalculatedRouteablePMList.ToList();
            return null;
        }

        protected virtual List<PAProcessModule> HandleModuleListForDNote(PWMethodIntake pwMethod, DatabaseApp dbApp, Database db, DeliveryNotePos notePos, List<PAProcessModule> modulesInAutomaticMode, List<PAProcessModule> removedModules, List<PAProcessModule> addedModules)
        {
            Guid[] targets;
            bool targetsChanged = false;
            List<PAProcessModule> lastRoutes = GetCachedModulesForDNoteIfWaiting(pwMethod, modulesInAutomaticMode, removedModules, addedModules, out targetsChanged, out targets);
            if (targets == null || !targets.Any())
            {
                LastCalculatedRouteablePMList = new SafeList<PAProcessModule>();
                return LastCalculatedRouteablePMList.ToList();
            }
            if (lastRoutes != null && !targetsChanged)
                return lastRoutes;

            LastCalculatedRouteablePMList = new SafeList<PAProcessModule>();
            LastTargets = targets;

            Type typeOfSilo = typeof(PAMSilo);
            Guid thisMethodID = ContentACClassWF.ACClassMethodID;
            IList<gip.core.datamodel.ACClass> selectedModules = pwMethod.ACFacilityManager.GetSelectedModulesAsACClass(notePos, db);

            Guid[] allPossibleModulesinThisWF = RootPW.GetAllRoutableModules().Select(c => c.ComponentClass.ACClassID).ToArray();

            ACRoutingParameters routingParameters = new ACRoutingParameters()
            {
                RoutingService = this.RoutingService,
                Database = db,
                AttachRouteItemsToContext = false,
                SelectionRuleID = SelRuleID_ReachableDest,
                Direction = RouteDirections.Forwards,
                SelectionRuleParams = new object[] { targets, allPossibleModulesinThisWF },
                DBSelector = (c, p, r) => targets.Contains(c.ACClassID),
                DBDeSelector = (c, p, r) => (c.ACKind == Global.ACKinds.TPAProcessModule
                                                    && (typeOfSilo.IsAssignableFrom(c.ObjectType)
                                                        || !c.BasedOnACClassID.HasValue
                                                        || (c.BasedOnACClassID.HasValue && !c.ACClass1_BasedOnACClass.ACClassWF_RefPAACClass.Where(refc => refc.ACClassMethodID == thisMethodID).Any()))),
                MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                IncludeReserved = true,
                IncludeAllocated = true
            };

            foreach (var module in modulesInAutomaticMode)
            {
                if (!LastCalculatedRouteablePMList.Contains(module)
                    && selectedModules.Where(c => c.ACClassID == module.ComponentClass.ACClassID).Any())
                {
                    RoutingResult rResult = ACRoutingService.FindSuccessors(module.GetACUrl(), routingParameters);
                    if (rResult.Routes != null && rResult.Routes.Any())
                        LastCalculatedRouteablePMList.Add(module);
                }
            }
            if (!LastCalculatedRouteablePMList.Any())
            {
                // Error00126: No route found to planned destination
                Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "ProcessModuleList(1)", 1080, "Error00126");
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
            }
            return LastCalculatedRouteablePMList.ToList();
        }

        protected virtual List<PAProcessModule> HandleModuleListForBooking(DatabaseApp dbApp, Database db, FacilityBooking fBooking, List<PAProcessModule> modulesInAutomaticMode, List<PAProcessModule> removedModules, List<PAProcessModule> addedModules)
        {
            if (!String.IsNullOrEmpty(fBooking.PropertyACUrl))
            {
                var querySource = modulesInAutomaticMode.Where(c => c.GetACUrl() == fBooking.PropertyACUrl);
                if (querySource.Any())
                    return querySource.ToList();
            }
            if (fBooking.OutwardFacility != null && fBooking.OutwardFacility.VBiFacilityACClassID.HasValue)
            {
                var querySource = modulesInAutomaticMode.Where(c => c.ComponentClass.ACClassID == fBooking.OutwardFacility.VBiFacilityACClassID.Value);
                if (querySource.Any())
                    return querySource.ToList();
            }
            if (fBooking.OutwardFacilityCharge != null && fBooking.OutwardFacilityCharge.Facility != null && fBooking.OutwardFacilityCharge.Facility.VBiFacilityACClassID.HasValue)
            {
                var querySource = modulesInAutomaticMode.Where(c => c.ComponentClass.ACClassID == fBooking.OutwardFacilityCharge.Facility.VBiFacilityACClassID.Value);
                if (querySource.Any())
                    return querySource.ToList();
            }
            return null;
        }

        #endregion


        #region Skipping
        protected string _ExtraDisTargetDest = null;
        [ACPropertyInfo(false, 9999)]
        public string ExtraDisTargetDest
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _ExtraDisTargetDest;
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _ExtraDisTargetDest = value;
                }
                ExtraDisTargetComp = null;
                OnPropertyChanged("ExtraDisTargetDest");
            }
        }

        private ACComponent _ExtraDisTargetComp = null;
        public ACComponent ExtraDisTargetComp
        {
            get
            {
                string acUrlExtraDisDest = null;
                if (!IsExtraDisTargetEntered(out acUrlExtraDisDest))
                {

                    using (ACMonitor.Lock(_20015_LockValue))
                    {
                        _ExtraDisTargetComp = null;
                        return null;
                    }
                }
                var rootPW = RootPW as PWMethodVBBase;
                if (rootPW != null)
                {
                    var targetComp = rootPW.ExtraDisTargetComp;
                    if (targetComp != null)
                        return targetComp;
                }

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_ExtraDisTargetComp != null)
                        return _ExtraDisTargetComp;
                }
                var extraDisTargetComp = PWMethodVBBase.ResolveExtraDisDest(this, acUrlExtraDisDest);

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _ExtraDisTargetComp = extraDisTargetComp;
                    return _ExtraDisTargetComp;
                }
            }
            protected set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _ExtraDisTargetComp = value;
                }
            }
        }

        public bool IsInEmptyingMode
        {
            get
            {
                var rootPW = RootPW;
                if (rootPW == null)
                    return false;
                return ((ACSubStateEnum)rootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode)
                    || ((ACSubStateEnum)rootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode)
                    || ((ACSubStateEnum)CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode)
                    || ((ACSubStateEnum)CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode)
                    || ((ACSubStateEnum)CurrentACSubState).HasFlag(ACSubStateEnum.SMDisThenNextComp);
            }
        }

        public override bool IsInSkippingMode
        {
            get
            {
                var rootPW = RootPW;
                if (rootPW == null)
                    return false;

                return ((ACSubStateEnum)CurrentACSubState).HasFlag(ACSubStateEnum.SMBatchCancelled)
                || ((ACSubStateEnum)CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode)
                || ((ACSubStateEnum)CurrentACSubState).HasFlag(ACSubStateEnum.SMDisThenNextComp)
                || ((ACSubStateEnum)CurrentACSubState).HasFlag(ACSubStateEnum.SMInterDischarging)
                || ((ACSubStateEnum)CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode)
                || RootPW.IsInSkippingMode;
            }
        }

        public override bool MustRepeatGroupAtEnd
        {
            get
            {
                var rootPW = RootPW;
                if (rootPW == null)
                    return false;
                return ((ACSubStateEnum)CurrentACSubState).HasFlag(ACSubStateEnum.SMRepeatGroup);
            }
        }

        #endregion

        #endregion


        #region Methods

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PWGroupVB(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWGroup(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;

            switch(acMethodName)
            {
                case nameof(SetExtraDisTarget):
                    SetExtraDisTarget(acParameter[0] as string);
                    return true;
                case nameof(AbortAllAndSetExtraDisTarget):
                    AbortAllAndSetExtraDisTarget(acParameter[0] as string);
                    return true;
            }

            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


        #region Public

        #region State
        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            ExtraDisTargetDest = null;
            ExtraDisTargetComp = null;

            if (!DosableOnGroupCheck && SkipIfNoComp)
            {
                if (SkipIfNoDosComp())
                {
                    CurrentACState = ACStateEnum.SMCompleted;
                    return;
                }
            }

            base.SMStarting();
        }

        public override void Reset()
        {
            base.Reset();
            LastModulesInAutomaticMode = null;
            LastTargets = null;
            LastCalculatedRouteablePMList = null;
            _SkipGroup = null;
            ResetRoutingCheckWait();
        }

        protected override bool OnHandleAvailableProcessModule(PAProcessModule processModule)
        {
            if (SkipIfNoComp && DosableOnGroupCheck && processModule == null)
            {
                CurrentACState = ACStateEnum.SMCompleted;
                return true;
            }
            return base.OnHandleAvailableProcessModule(processModule);
        }

        protected override void OnProcessModuleOccupied(PAProcessModule processModule)
        {
            base.OnProcessModuleOccupied(processModule);
            if (   processModule != null 
                && processModule.ReservationInfo != null 
                && ReserveModule == 1)
            {
                PWMethodProduction methodBase = ParentPWMethod<PWMethodProduction>();
                if (methodBase != null)
                {
                    if (methodBase.IsLastBatchRunning)
                        processModule.ReservationInfo.ValueT = null;
                    else
                        processModule.ReservationInfo.ValueT = CurrentProgramNo;
                }
            }
        }

        protected override void OnProcessModuleReleased(PAProcessModule module)
        {
            base.OnProcessModuleReleased(module);
            if (module != null && module.ReservationInfo != null)
            {
                bool canResetReservation = true;
                if (ReserveModule == 1)
                {
                    PWMethodProduction methodBase = ParentPWMethod<PWMethodProduction>();
                    if (methodBase != null && !methodBase.IsLastBatchRunning)
                        canResetReservation = false;
                }
                if (canResetReservation && !String.IsNullOrEmpty(module.ReservationInfo.ValueT))
                    module.ResetReservationInfo();
            }
        }
        #endregion


        #region Skipping
        private bool? _SkipGroup = null;
        public bool SkipIfNoDosComp()
        {
            if (_SkipGroup.HasValue)
                return _SkipGroup.Value;

            var relevantPWNodes = FindChildComponents<IPWNodeReceiveMaterial>(c => c is IPWNodeReceiveMaterial).ToArray();
            if (relevantPWNodes.Any())
                _SkipGroup = relevantPWNodes.All(c => !c.HasAnyMaterialToProcess);
            else
                _SkipGroup = false;

            return _SkipGroup.Value;
        }

        public bool HasAnyMaterialToProcess
        {
            get
            {
                var relevantPWNodes = FindChildComponents<IPWNodeReceiveMaterial>(c => c is IPWNodeReceiveMaterial).ToArray();
                if (relevantPWNodes.Any())
                    return relevantPWNodes.Any(c => c.HasAnyMaterialToProcess);
                else
                    return false;
            }
        }

        public bool IsExtraDisTargetEntered(out string acUrlExtraDisDest)
        {
            var rootPW = RootPW as PWMethodVBBase;
            acUrlExtraDisDest = "";
            if (rootPW == null)
                return false;

            // Benutzer hat noch gar kein Ziel definiert wo es hingehen soll
            if (ExtraDisTargetDest == null && rootPW.ExtraDisTargetDest == null)
                return false;

            string pwGroupExtraDisTargetDest = ExtraDisTargetDest == null ? "" : ExtraDisTargetDest.Trim();
            string rootPWExtraDisTargetDest = rootPW.ExtraDisTargetDest == null ? "" : rootPW.ExtraDisTargetDest.Trim();
            // Falls strings leer, dann hat der Benutzer angegeben dass er in die geplanten Ziele nach Batchplan transportieren will
            if (pwGroupExtraDisTargetDest == "" && rootPWExtraDisTargetDest == "")
                return true;

            acUrlExtraDisDest = !String.IsNullOrEmpty(pwGroupExtraDisTargetDest) ? pwGroupExtraDisTargetDest : rootPWExtraDisTargetDest;
            return true;
        }

        [ACMethodInfo("","",9999, true)]
        public void SetExtraDisTarget(string acUrlExtraDisDest)
        {
            if (string.IsNullOrEmpty(acUrlExtraDisDest))
                return;

            ExtraDisTargetDest = acUrlExtraDisDest;

            PWMethodVBBase root = RootPW as PWMethodVBBase;
            root?.SwitchToEmptyingMode();
        }

        [ACMethodInfo("", "", 9999, true)]
        public void AbortAllAndSetExtraDisTarget(string acUrlExtraDisDest)
        {
            SetExtraDisTarget(acUrlExtraDisDest);

            var activeNodes = RootPW?.FindChildComponents<PWBaseExecutable>(c => c is PWBaseExecutable && !(c is PWGroup))
                                     .Where(x => x.CurrentACState > ACStateEnum.SMIdle && x.IsSkippable).ToArray();

            if (activeNodes != null && activeNodes.Any())
            {
                foreach (var node in activeNodes)
                {
                    PWNodeProcessMethod nodeProcessMethod = node as PWNodeProcessMethod;
                    if (nodeProcessMethod != null)
                    {
                        PAProcessFunction activeFunction = nodeProcessMethod.GetCurrentExecutingFunction<PAProcessFunction>();
                        if (activeFunction != null && !(activeFunction is PAFDischarging))
                            activeFunction.Abort();
                    }
                    else
                    {
                        node.ResetAndComplete();
                    }
                }
            }
        }

        #endregion


        #region Order-Info
        bool _PAOrderRecursionLock = false;
        public override PAOrderInfo GetPAOrderInfo()
        {
            if (_PAOrderRecursionLock)
                return null;
            PAOrderInfo info = new PAOrderInfo();
            if (IsProduction)
            {
                var batch = ParentPWMethod<PWMethodProduction>().CurrentProdOrderBatch;
                if (batch != null)
                    info.Add(ProdOrderBatch.ClassName, batch.ProdOrderBatchID);
            }
            else if (IsIntake)
            {
                var pwMethod = ParentPWMethod<PWMethodIntake>();
                Picking picking = pwMethod.CurrentPicking;
                DeliveryNotePos notePos = pwMethod.CurrentDeliveryNotePos;
                var fBooking = pwMethod.CurrentFacilityBooking;
                if (picking != null)
                    info.Add(Picking.ClassName, picking.PickingID);
                if (notePos != null)
                    info.Add(DeliveryNotePos.ClassName, notePos.DeliveryNotePosID);
                if (fBooking != null)
                    info.Add(FacilityBooking.ClassName, fBooking.FacilityBookingID);
                if (pwMethod.CurrentPickingPos != null)
                    info.Add(PickingPos.ClassName, pwMethod.CurrentPickingPos.PickingPosID);
            }
            else if (IsRelocation)
            {
                var pwMethod = ParentPWMethod<PWMethodRelocation>();
                var fBooking = pwMethod.CurrentFacilityBooking;
                Picking picking = pwMethod.CurrentPicking;
                if (fBooking != null)
                    info.Add(FacilityBooking.ClassName, fBooking.FacilityBookingID);
                if (picking != null)
                    info.Add(Picking.ClassName, picking.PickingID);
                if (pwMethod.CurrentPickingPos != null)
                    info.Add(PickingPos.ClassName, pwMethod.CurrentPickingPos.PickingPosID);
            }
            else if (IsLoading)
            {
                var pwMethod = ParentPWMethod<PWMethodLoading>();
                Picking picking = pwMethod.CurrentPicking;
                DeliveryNotePos notePos = pwMethod.CurrentDeliveryNotePos;
                if (picking != null)
                    info.Add(Picking.ClassName, picking.PickingID);
                if (notePos != null)
                    info.Add(DeliveryNotePos.ClassName, notePos.DeliveryNotePosID);
                if (pwMethod.CurrentPickingPos != null)
                    info.Add(PickingPos.ClassName, pwMethod.CurrentPickingPos.PickingPosID);
            }

            _PAOrderRecursionLock = true;
            try
            {
                foreach (var activeChild in FindChildComponents<PWNodeProcessMethod>(c => c is PWNodeProcessMethod
                                                                                        && (c as PWNodeProcessMethod).CurrentACState != ACStateEnum.SMIdle
                                                                                        && (c as PWNodeProcessMethod).CurrentACState != ACStateEnum.SMBreakPoint))
                {
                    PAOrderInfo subInfo = activeChild.GetPAOrderInfo();
                    if (subInfo != null)
                    {
                        info.Append(subInfo);
                    }
                }
            }
            finally
            {
                _PAOrderRecursionLock = false;
            }

            return info;
        }
        #endregion


        #region Scan-Task
        public virtual Msg OnGetMessageOnReleasingProcessModule(PAFWorkTaskScanBase invoker, bool pause)
        {
            return null;
        }
        #endregion

        #endregion


        #region Planning and Testing
        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList, ref DumpStats dumpStats)
        {
            base.DumpPropertyList(doc, xmlACPropertyList, ref dumpStats);

            XmlElement xmlChild = xmlACPropertyList["LastTargets"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("LastTargets");
                if (xmlChild != null)
                {
                    var lastTargets = LastTargets;
                    if (lastTargets == null || !lastTargets.Any())
                        xmlChild.InnerText = "null";
                    else
                    {
                        int i = 0;
                        StringBuilder sb = new StringBuilder();
                        foreach (var plTime in lastTargets)
                        {
                            sb.AppendFormat("{0}:{1}|", i, plTime.ToString());
                        }
                        xmlChild.InnerText = sb.ToString();
                    }
                }
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["LastCalculatedRouteablePMList"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("LastCalculatedRouteablePMList");
                if (xmlChild != null)
                {
                    var lastPMList = LastCalculatedRouteablePMList;
                    if (lastPMList == null || !lastPMList.Any())
                        xmlChild.InnerText = "null";
                    else
                    {
                        int i = 0;
                        StringBuilder sb = new StringBuilder();
                        foreach (var plTime in lastPMList)
                        {
                            sb.AppendFormat("{0}:{1}|", i, plTime.GetACUrl());
                        }
                        xmlChild.InnerText = sb.ToString();
                    }
                }
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["ExtraDisTargetDest"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("ExtraDisTargetDest");
                if (xmlChild != null)
                    xmlChild.InnerText = ExtraDisTargetDest != null ? ExtraDisTargetDest : "null";
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["ExtraDisTargetComp"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("ExtraDisTargetComp");
                if (xmlChild != null)
                    xmlChild.InnerText = ExtraDisTargetComp != null ? ExtraDisTargetComp.GetACUrl() : "null";
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["SkipGroup"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("SkipGroup");
                if (xmlChild != null)
                    xmlChild.InnerText = _SkipGroup != null ? _SkipGroup.Value.ToString() : "null";
                xmlACPropertyList.AppendChild(xmlChild);
            }
        }
        #endregion

        #endregion
    }
}
