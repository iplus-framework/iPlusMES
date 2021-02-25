using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using System.Xml;

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
            ACMethod method;
            method = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("IgnoreFIFO", typeof(bool), false, Global.ParamOption.Required));
            paramTranslation.Add("IgnoreFIFO", "en{'Ignore FIFO for Processmodule-Mapping'}de{'Ignoriere FIFO-Prinzip für Prozessmodul-Belegung'}");
            method.ParameterValueList.Add(new ACValue("RoutingCheck", typeof(bool), false, Global.ParamOption.Required));
            paramTranslation.Add("RoutingCheck", "en{'Only routeable modules from predecessor'}de{'Nur erreichbare Module vom Vorgänger'}");
            method.ParameterValueList.Add(new ACValue("WithoutPM", typeof(bool), false, Global.ParamOption.Required));
            paramTranslation.Add("WithoutPM", "en{'Ignore Processmodule-Mapping'}de{'Ohne Prozessmodul-Belegung'}");
            method.ParameterValueList.Add(new ACValue("OccupationByScan", typeof(bool), false, Global.ParamOption.Required));
            paramTranslation.Add("OccupationByScan", "en{'Processmodule-Mapping manually by user'}de{'Prozessmodulbelegung manuell vom Anwender'}");
            method.ParameterValueList.Add(new ACValue("Priority", typeof(ushort), 0, Global.ParamOption.Required));
            paramTranslation.Add("Priority", "en{'Priorization'}de{'Priorisierung'}");
            method.ParameterValueList.Add(new ACValue("SkipIfNoComp", typeof(bool), false, Global.ParamOption.Required));
            paramTranslation.Add("SkipIfNoComp", "en{'Skip if no one child has material to process'}de{'Skip if no one child has material to process'}");
            method.ParameterValueList.Add(new ACValue("MaxBatchWeight", typeof(double), false, Global.ParamOption.Optional));
            paramTranslation.Add("MaxBatchWeight", "en{'Max. batch weight [kg]'}de{'Maximales Batchgewicht [kg]'}");
            var wrapper = new ACMethodWrapper(method, "en{'Configuration'}de{'Konfiguration'}", typeof(PWGroupVB), paramTranslation, null);
            ACMethod.RegisterVirtualMethod(typeof(PWGroupVB), ACStateConst.SMStarting, wrapper);

            RegisterExecuteHandler(typeof(PWGroupVB), HandleExecuteACMethod_PWGroupVB);

            ACRoutingService.RegisterSelectionQuery(SelRuleID_ReachableDest, 
                (c, p) =>      c.Component.ValueT is PAProcessModule
                            && (p[0] as Guid[]).Contains(c.Component.ValueT.ComponentClass.ACClassID),
                (c, p) =>   !(p[1] as Guid[]).Contains(c.Component.ValueT.ComponentClass.ACClassID)
                            && (c.Component.ValueT is PAMSilo || c.Component.ValueT is PAMParkingspace));

            ACRoutingService.RegisterSelectionQuery(SelRuleID_ReachableSource,
                (c, p) => (c.Component.ValueT is PAMSilo || c.Component.ValueT is PAMParkingspace)
                            && (Guid)p[0] == c.Component.ValueT.ComponentClass.ACClassID,
                (c, p) => c.Component.ValueT is PAMSilo || c.Component.ValueT is PAMParkingspace);

            ACRoutingService.RegisterSelectionQuery(SelRuleID_IsDestDirectSucessor,
                (c, p) => (c.Component.ValueT is PAMSilo || c.Component.ValueT is PAMParkingspace)
                            && (Guid)p[0] == c.Component.ValueT.ComponentClass.ACClassID,
                (c, p) => c.Component.ValueT is PAProcessModule);
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
        /// <summary>
        /// Rule Cache
        /// </summary>
        private Guid[] _LastTargets = null;
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

        /// <summary>
        /// Cache with Process-Modules which can be potentially mapped/accessed from this PWGroup beacuse they are routable to the planned destination
        /// </summary>
        private SafeList<PAProcessModule> _LastCalculatedRouteablePMList = null;
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
                try
                {
                    if (modulesInAutomaticMode == null
                        || !modulesInAutomaticMode.Any()
                        // Falls Gruppe keine Materialweiterleitungsschritte besitzt, dann keine Überpüfung der Routen:
                        || !this.FindChildComponents<IPWNodeDeliverMaterial>(c => c is IPWNodeDeliverMaterial).Any())
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
                        && ParentPWMethod<PWMethodProduction>().CurrentProdOrderBatch != null
                        && ParentPWMethod<PWMethodProduction>().CurrentProdOrderBatch.ProdOrderBatchPlanID.HasValue)
                    {
                        short destError = 0;
                        string errorMsg;
                        Guid[] targets = ParentPWMethod<PWMethodProduction>().GetCachedDestinations(false/*addedModules.Any()*/, out destError, out errorMsg);
                        if (targets == null || !targets.Any())
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

                        if (targets != null && targets.Any())
                        {
                            // Performance-Optimization:
                            // If targets changed, then recalculate routebale PM List
                            if (PWMethodTransportBase.AreCachedDestinationsDifferent(LastTargets, targets))
                            {
                                LastCalculatedRouteablePMList = new SafeList<PAProcessModule>();
                            }
                            if (LastCalculatedRouteablePMList == null)
                                LastCalculatedRouteablePMList = new SafeList<PAProcessModule>();
                            LastTargets = null;

                            Type typeOfSilo = typeof(PAMSilo);
                            Guid thisMethodID = ContentACClassWF.ACClassMethodID;
                            using (var db = new Database())
                            {
                                foreach (var module in modulesInAutomaticMode)
                                {
                                    if (!LastCalculatedRouteablePMList.Contains(module))
                                    {
                                        Guid[] allPossibleModulesinThisWF = RootPW.AllPossibleModules.Select(c => c.ComponentClass.ACClassID).ToArray();
                                        RoutingResult rResult = ACRoutingService.FindSuccessors(RoutingService, db, false,
                                            module, SelRuleID_ReachableDest, RouteDirections.Forwards, new object[] { targets, allPossibleModulesinThisWF },
                                            (c, p, r) => targets.Contains(c.ACClassID),
                                                            (c, p, r) => (c.ACKind == Global.ACKinds.TPAProcessModule
                                                                        && (typeOfSilo.IsAssignableFrom(c.ObjectType)
                                                                            || !c.BasedOnACClassID.HasValue
                                                                            || (c.BasedOnACClassID.HasValue && !c.ACClass1_BasedOnACClass.ACClassWF_RefPAACClass.Where(refc => refc.ACClassMethodID == thisMethodID).Any()))),
                                                            0, true, true, false, false);
                                        if (rResult.Routes != null && rResult.Routes.Any())
                                            LastCalculatedRouteablePMList.Add(module);
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
                            LastCalculatedRouteablePMList = new SafeList<PAProcessModule>();
                        _LastTargets = targets;
                        return LastCalculatedRouteablePMList.ToList();
                    }
                    else if (IsIntake)
                    {
                        var pwMethod = ParentPWMethod<PWMethodIntake>();
                        if (pwMethod != null && (pwMethod.CurrentPicking != null || pwMethod.CurrentDeliveryNotePos != null))
                        {
                            using (var db = new Database())
                            using (var dbApp = new DatabaseApp(db))
                            {
                                Picking picking = null;
                                PickingPos pickingPos = null;
                                DeliveryNotePos notePos = null;
                                if (pwMethod.CurrentPicking != null)
                                    picking = pwMethod.CurrentPicking.FromAppContext<Picking>(dbApp);
                                if (pwMethod.CurrentPickingPos != null)
                                    pickingPos = pwMethod.CurrentPickingPos.FromAppContext<PickingPos>(dbApp);
                                if (pwMethod.CurrentDeliveryNotePos != null)
                                    notePos = pwMethod.CurrentDeliveryNotePos.FromAppContext<DeliveryNotePos>(dbApp);

                                if (picking != null)
                                {
                                    var pmListForPicking = HandleModuleListForPicking(dbApp, db, picking, pickingPos, modulesInAutomaticMode);
                                    if (pmListForPicking != null)
                                        return pmListForPicking;
                                }
                                else if (notePos != null)
                                {
                                    short destError = 0;
                                    string errorMsg;
                                    Guid[] targets = pwMethod.GetCachedDestinationsForDN(false/*addedModules.Any()*/, out destError, out errorMsg);
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
                                        return new List<PAProcessModule>();
                                    }

                                    if (targets != null && targets.Any())
                                    {
                                        // Performance-Optimization:
                                        // If targets changed, then recalculate routebale PM List
                                        if (PWMethodTransportBase.AreCachedDestinationsDifferent(LastTargets, targets))
                                        {
                                            LastCalculatedRouteablePMList = new SafeList<PAProcessModule>();
                                        }
                                        if (LastCalculatedRouteablePMList == null)
                                            LastCalculatedRouteablePMList = new SafeList<PAProcessModule>();
                                        LastTargets = null;

                                        Type typeOfSilo = typeof(PAMSilo);
                                        Guid thisMethodID = ContentACClassWF.ACClassMethodID;
                                        IList<gip.core.datamodel.ACClass> selectedModules = pwMethod.ACFacilityManager.GetSelectedModulesAsACClass(notePos, db);

                                        foreach (var module in modulesInAutomaticMode)
                                        {
                                            if (!LastCalculatedRouteablePMList.Contains(module)
                                                && selectedModules.Where(c => c.ACClassID == module.ComponentClass.ACClassID).Any())
                                            {
                                                Guid[] allPossibleModulesinThisWF = RootPW.AllPossibleModules.Select(c => c.ComponentClass.ACClassID).ToArray();
                                                RoutingResult rResult = ACRoutingService.FindSuccessors(RoutingService, db, false,
                                                    module, SelRuleID_ReachableDest, RouteDirections.Forwards, new object[] { targets, allPossibleModulesinThisWF },
                                                                    (c, p, r) => targets.Contains(c.ACClassID),
                                                                    (c, p, r) => (c.ACKind == Global.ACKinds.TPAProcessModule
                                                                                && (typeOfSilo.IsAssignableFrom(c.ObjectType)
                                                                                    || !c.BasedOnACClassID.HasValue
                                                                                    || (c.BasedOnACClassID.HasValue && !c.ACClass1_BasedOnACClass.ACClassWF_RefPAACClass.Where(refc => refc.ACClassMethodID == thisMethodID).Any()))),
                                                                    0, true, true, false, false);
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
                                    }
                                    else
                                        LastCalculatedRouteablePMList = new SafeList<PAProcessModule>();
                                    LastTargets = targets;
                                    return LastCalculatedRouteablePMList.ToList();
                                }
                            }
                        }
                    }
                    else if (IsTransport)
                    {
                        var pwMethod = ParentPWMethod<PWMethodTransportBase>();
                        if (pwMethod != null)
                        {
                            using (var db = new Database())
                            using (var dbApp = new DatabaseApp(db))
                            {
                                Picking picking = null;
                                PickingPos pickingPos = null;
                                FacilityBooking fBooking = null;
                                if (pwMethod.CurrentPicking != null)
                                    picking = pwMethod.CurrentPicking.FromAppContext<Picking>(dbApp);
                                if (pwMethod.CurrentPickingPos != null)
                                    pickingPos = pwMethod.CurrentPickingPos.FromAppContext<PickingPos>(dbApp);
                                if (IsRelocation && ParentPWMethod<PWMethodRelocation>().CurrentFacilityBooking != null)
                                    fBooking = ParentPWMethod<PWMethodRelocation>().CurrentFacilityBooking.FromAppContext<FacilityBooking>(dbApp);
                                if (picking != null)
                                {
                                    var pmListForPicking = HandleModuleListForPicking(dbApp, db, picking, pickingPos, modulesInAutomaticMode);
                                    if (pmListForPicking != null)
                                        return pmListForPicking;
                                }
                                else if (fBooking != null)
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
                                }
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

        protected virtual List<PAProcessModule> HandleModuleListForPicking(DatabaseApp dbApp, Database db, Picking picking, PickingPos pickingPos, List<PAProcessModule> modulesInAutomaticMode)
        {
            if (pickingPos == null)
            {
                if (!picking.PickingPos_Picking.Where(c => c.InOrderPosID.HasValue || c.OutOrderPosID.HasValue || c.PickingPosProdOrderPartslistPos_PickingPos.Any()).Any())
                {
                    pickingPos = picking.PickingPos_Picking
                        .Where(c => c.MDDelivPosLoadStateID.HasValue && c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive)
                        .OrderBy(c => c.Sequence)
                        .FirstOrDefault();
                    if (pickingPos == null)
                    {
                        pickingPos = picking.PickingPos_Picking
                            .Where(c => !c.MDDelivPosLoadStateID.HasValue)
                            .OrderBy(c => c.Sequence)
                            .FirstOrDefault();
                    }
                    else if (pickingPos == null)
                    {
                        pickingPos = picking.PickingPos_Picking.FirstOrDefault();
                    }
                }
            }
            if (pickingPos != null && pickingPos.ToFacility != null && pickingPos.ToFacility.VBiFacilityACClassID.HasValue)
            {
                bool hasGroupDosings = this.FindChildComponents<PWDosing>(c => c is PWDosing).Any();
                Guid[] targets = new Guid[] { pickingPos.ToFacility.VBiFacilityACClassID.Value };
                var lastTargest = LastTargets;
                if (lastTargest == null
                    || !lastTargest.Any()
                    || lastTargest.Count() != targets.Count()
                    || !lastTargest.All(targets.Contains)
                    || !targets.All(lastTargest.Contains))
                {
                    LastCalculatedRouteablePMList = new SafeList<PAProcessModule>();
                }
                if (LastCalculatedRouteablePMList == null)
                    LastCalculatedRouteablePMList = new SafeList<PAProcessModule>();
                LastTargets = null;


                Type typeOfSilo = typeof(PAMSilo);
                Guid thisMethodID = ContentACClassWF.ACClassMethodID;
                foreach (var module in modulesInAutomaticMode)
                {
                    if (!LastCalculatedRouteablePMList.Contains(module))
                    {
                        Guid[] allPossibleModulesinThisWF = RootPW.AllPossibleModules.Select(c => c.ComponentClass.ACClassID).ToArray();
                        RoutingResult rResult = ACRoutingService.FindSuccessors(RoutingService, db, false,
                            module, SelRuleID_ReachableDest, RouteDirections.Forwards, new object[] { targets, allPossibleModulesinThisWF },
                                            (c, p, r) => c.ACClassID == pickingPos.ToFacility.VBiFacilityACClassID.Value,
                                            (c, p, r) => (c.ACKind == Global.ACKinds.TPAProcessModule
                                                        && (typeOfSilo.IsAssignableFrom(c.ObjectType)
                                                            || !c.BasedOnACClassID.HasValue
                                                            || (c.BasedOnACClassID.HasValue && !c.ACClass1_BasedOnACClass.ACClassWF_RefPAACClass.Where(refc => refc.ACClassMethodID == thisMethodID).Any()))),
                                            0,true, true, false, false);
                        if (rResult.Routes != null && rResult.Routes.Any())
                        {
                            if (hasGroupDosings && pickingPos.FromFacility != null && pickingPos.FromFacility.VBiFacilityACClassID.HasValue)
                            {
                                rResult = ACRoutingService.FindSuccessors(RoutingService, db, false,
                                                            module, SelRuleID_ReachableSource, RouteDirections.Backwards, new object[] { pickingPos.FromFacility.VBiFacilityACClassID.Value },
                                                            (c, p, r) => c.ACClassID == pickingPos.FromFacility.VBiFacilityACClassID.Value,
                                                            (c, p, r) => (c.ACKind == Global.ACKinds.TPAProcessModule && (typeOfSilo.IsAssignableFrom(c.ObjectType))), 
                                                            0, true, true, false, false);
                            }
                            if (rResult.Routes != null && rResult.Routes.Any())
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

                LastTargets = targets;
                return LastCalculatedRouteablePMList.ToList();
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
        #endregion

        #endregion


        #region Methods

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PWGroupVB(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWGroup(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion


        #region Public

        #region State
        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            ExtraDisTargetDest = null;
            ExtraDisTargetComp = null;

            if (SkipIfNoComp)
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
            _LastCalculatedRouteablePMList = null;
            _SkipGroup = null;
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
        #endregion


        #region Order-Infor
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
                if (picking != null)
                    info.Add(Picking.ClassName, picking.PickingID);
                if (notePos != null)
                    info.Add(DeliveryNotePos.ClassName, notePos.DeliveryNotePosID);
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

        #endregion


        #region Planning and Testing
        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList)
        {
            base.DumpPropertyList(doc, xmlACPropertyList);

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
