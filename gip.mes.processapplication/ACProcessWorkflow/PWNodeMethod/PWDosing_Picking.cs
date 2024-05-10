using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using static gip.mes.datamodel.MDReservationMode;
using static gip.mes.facility.ACPartslistManager.QrySilosResult;
using gip.core.processapplication;
using static gip.core.communication.ISOonTCP.PLC;

namespace gip.mes.processapplication
{
    public partial class PWDosing
    {

        #region Properties
        public ACPickingManager PickingManager
        {
            get
            {
                PWMethodTransportBase pwMethodTransport = ParentPWMethod<PWMethodTransportBase>();
                return pwMethodTransport != null ? pwMethodTransport.PickingManager : null;
            }
        }

        private bool _RepeatDosingForPicking = false;
        public bool RepeatDosingForPicking
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _RepeatDosingForPicking;
                }
            }
            set
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _RepeatDosingForPicking = value;
                }
            }
        }

        public bool DoseAllPosFromPicking
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("DoseAllPosFromPicking");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }


        public bool HasAnyMaterialToProcessPicking
        {
            get
            {
                PWMethodTransportBase pwMethodTransport = ParentPWMethod<PWMethodTransportBase>();
                if (pwMethodTransport == null || pwMethodTransport.CurrentPicking == null)
                    return false;

                PAProcessModule module = ParentPWGroup.AccessedProcessModule != null ? ParentPWGroup.AccessedProcessModule : ParentPWGroup.FirstAvailableProcessModule;
                if (module == null && ParentPWGroup.ProcessModuleList != null) // If all occupied, then use first that is generally possible 
                    module = ParentPWGroup.ProcessModuleList.FirstOrDefault();
                if (module == null)
                    return false;


                using (var dbIPlus = new Database())
                using (var dbApp = new DatabaseApp(dbIPlus))
                {
                    Picking picking = pwMethodTransport.CurrentPicking.FromAppContext<Picking>(dbApp);
                    if (picking == null)
                        return false;
                    PickingPos pickingPosFromPWMethod = null;
                    if (pwMethodTransport.CurrentPickingPos != null && !DoseAllPosFromPicking)
                        pickingPosFromPWMethod = pwMethodTransport.CurrentPickingPos.FromAppContext<PickingPos>(dbApp);

                    PickingPos[] openPickings = dbApp.PickingPos.Include(c => c.FromFacility.FacilityReservation_Facility)
                                    //.Include(c => c.FromFacility.FacilityCharge_Facility)
                                    .Where(c => c.PickingID == picking.PickingID
                                            && c.MDDelivPosLoadStateID.HasValue
                                            && (c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad
                                                || c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive))
                                    .OrderBy(c => c.Sequence)
                                    .ToArray();

                    if ((ComponentsSeqFrom > 0 || ComponentsSeqTo > 0) && openPickings != null && openPickings.Any())
                    {
                        openPickings = openPickings.Where(c => c.Sequence >= ComponentsSeqFrom && c.Sequence <= ComponentsSeqTo)
                                                   .OrderBy(c => c.Sequence)
                                                   .ToArray();
                    }


                    if (!DoseAllPosFromPicking)
                    {
                        if (pickingPosFromPWMethod == null)
                            openPickings = openPickings.Take(1).ToArray();
                        else
                            openPickings = new PickingPos[] { pickingPosFromPWMethod };
                    }


                    foreach (PickingPos pickingPos in openPickings)
                    {
                        if (!(pickingPos.RemainingDosingWeight < (MinDosQuantity * -1)) && !double.IsNaN(pickingPos.RemainingDosingWeight))
                            continue;

                        Guid scaleACClassID = module.ComponentClass.ACClassID;
                        gip.core.datamodel.ACClass scaleACClass = module.ComponentClass;

                        RoutingResult rResult = null;
                        facility.ACPartslistManager.QrySilosResult possibleSilos = null;
                        IEnumerable<Route> routes = null;

                        if (pickingPos.FromFacility == null)
                        {
                            RouteQueryParams queryParams = new RouteQueryParams(RouteQueryPurpose.StartDosing,
                                                                                OldestSilo ? ACPartslistManager.SearchMode.OnlyEnabledOldestSilo : ACPartslistManager.SearchMode.SilosWithOutwardEnabled,
                                                                                null, null, ExcludedSilos, ReservationMode);
                            routes = GetRoutes(pickingPos, dbApp, dbIPlus, queryParams, module, out possibleSilos);
                        }
                        else
                        {
                            ACRoutingParameters rParameters = new ACRoutingParameters()
                            {
                                RoutingService = this.RoutingService,
                                Database = dbIPlus,
                                AttachRouteItemsToContext = true,
                                Direction = RouteDirections.Backwards,
                                SelectionRuleID = PAMSilo.SelRuleID_SiloDirect,
                                DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && pickingPos.FromFacility.VBiFacilityACClassID == c.ACClassID,
                                DBDeSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && c.ACClassID != scaleACClassID,
                                MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                                IncludeReserved = true,
                                IncludeAllocated = true,
                                DBRecursionLimit = 10
                            };

                            rResult = ACRoutingService.SelectRoutes(scaleACClass, pickingPos.FromFacility.FacilityACClass, rParameters);
                        }

                        Route dosingRoute = null;
                        if (pickingPos.FromFacility == null && possibleSilos != null && possibleSilos.FilteredResult != null)
                        {
                            foreach (ACPartslistManager.QrySilosResult.FacilitySumByLots prioSilo in possibleSilos.FilteredResult)
                            {
                                if (!prioSilo.StorageBin.VBiFacilityACClassID.HasValue
                                    || (pickingPos.ToFacilityID.HasValue && prioSilo.StorageBin.FacilityID == pickingPos.ToFacilityID))
                                    continue;
                                dosingRoute = routes == null ? null : routes.Where(c => c.FirstOrDefault().Source.ACClassID == prioSilo.StorageBin.VBiFacilityACClassID).FirstOrDefault();
                                if (dosingRoute != null)
                                    break;
                            }
                        }
                        else if (rResult == null || rResult.Routes == null || !rResult.Routes.Any())
                            continue;
                        else
                        {
                            //return true;
                            dosingRoute = rResult.Routes.FirstOrDefault();
                            //CurrentDosingRoute = dosingRoute;
                        }

                        if (dosingRoute == null)
                            continue;

                        RouteItem item = dosingRoute.FirstOrDefault();
                        if (item == null)
                            continue;
                        PAMSilo sourceSilo = item.SourceACComponent as PAMSilo;
                        if (sourceSilo == null)
                            continue;

                        core.datamodel.ACClassMethod refPAACClassMethod = RefACClassMethodOfContentWF;
                        if (refPAACClassMethod == null)
                            continue;

                        ACMethod acMethod = refPAACClassMethod.TypeACSignature();
                        if (acMethod == null)
                            continue;
                        PAProcessFunction responsibleFunc = GetResponsibleProcessFunc(module, acMethod);
                        if (responsibleFunc == null)
                            continue;

                        ACRoutingParameters routingParametersDB = new ACRoutingParameters()
                        {
                            Database = dbIPlus,
                            Direction = RouteDirections.Backwards,
                            DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && c.ACClassID == scaleACClassID,
                            DBIncludeInternalConnections = true,
                            AutoDetachFromDBContext = false
                        };

                        var parentModule = ACRoutingService.DbSelectRoutesFromPoint(responsibleFunc.ComponentClass, responsibleFunc.PAPointMatIn1.PropertyInfo, routingParametersDB).FirstOrDefault();

                        var sourcePoint = parentModule?.FirstOrDefault()?.SourceACPoint?.PropertyInfo;
                        var sourceClass = parentModule?.FirstOrDefault()?.Source;
                        if (sourcePoint == null || sourceClass == null)
                            continue;

                        ACRoutingParameters routingParameters = new ACRoutingParameters()
                        {
                            RoutingService = this.RoutingService,
                            Database = dbIPlus,
                            Direction = RouteDirections.Backwards,
                            SelectionRuleID = PAMSilo.SelRuleID_Silo,
                            MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                            IncludeReserved = true,
                            IncludeAllocated = true
                        };

                        RoutingResult routeResult = ACRoutingService.FindSuccessorsFromPoint(sourceClass, sourcePoint, routingParameters);

                        if (!routeResult.Routes.Any(c => c.Any(x => x.SourceACComponent == sourceSilo)))
                            continue;

                        return true;
                    }
                }

                return false;
            }
        }

        #endregion

        public virtual StartNextCompResult StartNextPickingPos(PAProcessModule module)
        {
            PWMethodTransportBase pwMethodTransport = ParentPWMethod<PWMethodTransportBase>();
            if (pwMethodTransport == null || pwMethodTransport.CurrentPicking == null)
                return StartNextCompResult.Done;
            var pwGroup = ParentPWGroup;
            // Nur einmal starten erlaubt:
            if (   !RepeatDosingForPicking
                && !DoseAllPosFromPicking
                && (this.CurrentACMethod.ValueT != null || this.IterationCount.ValueT >= 1))
                return StartNextCompResult.Done;

            Msg msg = null;
            using (var dbIPlus = new Database())
            using (var dbApp = new DatabaseApp(dbIPlus))
            {
                Picking picking = pwMethodTransport.CurrentPicking.FromAppContext<Picking>(dbApp);
                if (picking == null)
                    return StartNextCompResult.Done;
                PickingPos pickingPosFromPWMethod = null;
                if (pwMethodTransport.CurrentPickingPos != null && !DoseAllPosFromPicking)
                    pickingPosFromPWMethod = pwMethodTransport.CurrentPickingPos.FromAppContext<PickingPos>(dbApp);

                PickingPos[] openPickings = dbApp.PickingPos.Include(c => c.FromFacility.FacilityReservation_Facility)
                                //.Include(c => c.FromFacility.FacilityCharge_Facility)
                                .Where(c => c.PickingID == picking.PickingID
                                        && c.MDDelivPosLoadStateID.HasValue
                                        && (   c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad
                                            || c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive))
                                .OrderBy(c => c.Sequence)
                                .ToArray();

                //PickingPos[] openPickings = picking.PickingPos_Picking
                //                                    .Where(c => c.MDDelivPosLoadStateID.HasValue
                //                                            && (c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad
                //                                             || c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive))
                //                                    .OrderBy(c => c.Sequence).ToArray();

                if ((ComponentsSeqFrom > 0 || ComponentsSeqTo > 0) && openPickings != null && openPickings.Any())
                {
                    openPickings = openPickings.Where(c => c.Sequence >= ComponentsSeqFrom && c.Sequence <= ComponentsSeqTo)
                                               .OrderBy(c => c.Sequence)
                                               .ToArray();
                }


                if (!DoseAllPosFromPicking)
                {
                    if (pickingPosFromPWMethod == null)
                        openPickings = openPickings.Take(1).ToArray();
                    else
                        openPickings = new PickingPos[] { pickingPosFromPWMethod };
                }


                foreach (PickingPos pickingPos in openPickings)
                {
                    double targetWeight = 0;
                    if (!(pickingPos.RemainingDosingWeight < (MinDosQuantity * -1)) && !double.IsNaN(pickingPos.RemainingDosingWeight))
                    {
                        pickingPos.MDDelivPosLoadState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();
                        dbApp.ACSaveChanges();
                        continue;
                    }
                    targetWeight = pickingPos.RemainingDosingWeight * -1;
                    if (targetWeight < 0.000001)
                        targetWeight = 1;

                    IPAMContScale scale = ParentPWGroup != null ? ParentPWGroup.AccessedProcessModule as IPAMContScale : null;
                    ScaleBoundaries scaleBoundaries = null;
                    PAEScaleTotalizing totalizingScale = TotalizingScaleIfSWT;
                    if (scale != null)
                        scaleBoundaries = OnGetScaleBoundariesForDosing(scale, dbApp, null, null, null, null, null, null, null);
                    if (scaleBoundaries != null && !IsAutomaticContinousWeighing)
                    {
                        double? remainingWeight = null;
                        if (scaleBoundaries.RemainingWeightCapacity.HasValue)
                            remainingWeight = scaleBoundaries.RemainingWeightCapacity.Value;
                        else if (scaleBoundaries.MaxWeightCapacity > 0.00000001)
                            remainingWeight = scaleBoundaries.MaxWeightCapacity;
                        if (!remainingWeight.HasValue)
                        {
                            if (!MaxWeightAlarmSet)
                            {
                                MaxWeightAlarmSet = true;
                                //Error50162: MaxWeightCapacity of scale {0} is not configured.
                                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextPickingPos(1.1)", 1000, "Error50162", scale.GetACUrl());

                                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                {
                                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                }
                            }
                        }
                        else if (Math.Abs(targetWeight) > remainingWeight.Value)
                        {
                            targetWeight = remainingWeight.Value;
                            //pickingPos.MDDelivPosLoadState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();
                            //dbApp.ACSaveChanges();
                            //return StartNextCompResult.Done;
                        }
                    }

                    PADosingLastBatchEnum lastBatchMode = PADosingLastBatchEnum.None;
                    double newRemainingQ = pickingPos.RemainingDosingWeight + targetWeight;
                    if (newRemainingQ >= -0.000001 && MinDosQuantity > -0.0000001)
                    {
                        lastBatchMode = PADosingLastBatchEnum.LastBatch;
                        if (ParentPWMethodVBBase != null)
                            ParentPWMethodVBBase.IsLastBatch = lastBatchMode;
                    }

                    Guid scaleACClassID = ParentPWGroup.AccessedProcessModule.ComponentClass.ACClassID;
                    gip.core.datamodel.ACClass scaleACClass = ParentPWGroup.AccessedProcessModule.ComponentClass;

                    RoutingResult rResult = null;
                    facility.ACPartslistManager.QrySilosResult possibleSilos = null;
                    IEnumerable<Route> routes = null;

                    if (pickingPos.FromFacility == null)
                    {
                        RouteQueryParams queryParams = new RouteQueryParams(RouteQueryPurpose.StartDosing,
                                                                            OldestSilo ? ACPartslistManager.SearchMode.OnlyEnabledOldestSilo : ACPartslistManager.SearchMode.SilosWithOutwardEnabled,
                                                                            null, null, ExcludedSilos, ReservationMode);
                        routes = GetRoutes(pickingPos, dbApp, dbIPlus, queryParams, null, out possibleSilos);
                    }
                    else
                    {
                        ACRoutingParameters rParameters = new ACRoutingParameters()
                        {
                            RoutingService = this.RoutingService,
                            Database = dbIPlus,
                            AttachRouteItemsToContext = true,
                            Direction = RouteDirections.Backwards,
                            SelectionRuleID = PAMSilo.SelRuleID_SiloDirect,
                            DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && pickingPos.FromFacility.VBiFacilityACClassID == c.ACClassID,
                            DBDeSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && c.ACClassID != scaleACClassID, // Breche Suche ab sobald man bei einem Vorgänger der ein Silo oder Waage angelangt ist
                            MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                            IncludeReserved = true,
                            IncludeAllocated = true,
                            DBRecursionLimit = 10
                        };

                        rResult = ACRoutingService.SelectRoutes(scaleACClass, pickingPos.FromFacility.FacilityACClass, rParameters);
                    }

                    Route dosingRoute = null;
                    ACPartslistManager.QrySilosResult.FacilitySumByLots dosingSilo = null;

                    if (pickingPos.FromFacility == null && possibleSilos != null && possibleSilos.FilteredResult != null)
                    {
                        foreach (ACPartslistManager.QrySilosResult.FacilitySumByLots prioSilo in possibleSilos.FilteredResult)
                        {
                            if (!prioSilo.StorageBin.VBiFacilityACClassID.HasValue 
                                || (pickingPos.ToFacilityID.HasValue && prioSilo.StorageBin.FacilityID == pickingPos.ToFacilityID))
                                continue;
                            dosingRoute = routes == null ? null : routes.Where(c => c.FirstOrDefault().Source.ACClassID == prioSilo.StorageBin.VBiFacilityACClassID).FirstOrDefault();
                            if (dosingRoute != null)
                            {
                                dosingSilo = prioSilo;
                                break;
                            }
                        }
                        if (dosingRoute == null)
                        {
                            if (ComponentsSkippable)
                                continue;
                            if (NoSourceFoundForDosing.ValueT == 0)
                            {
                                NoSourceWait = DateTime.Now + TimeSpan.FromSeconds(10);
                                NoSourceFoundForDosing.ValueT = 1;

                                // Error50063: No Route found for dosing component {2} at Order {0}, bill of material{1}
                                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(8)", 1080, "Error50063",
                                                                         pickingPos.Material.MaterialNo,
                                                                         picking.PickingNo,
                                                                         "");

                                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                return StartNextCompResult.CycleWait;
                            }
                            else if (NoSourceFoundForDosing.ValueT == 2)
                            {
                                MDDelivPosLoadState posLoadState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();

                                if (posLoadState == null)
                                {
                                    // Error50062: posState ist null at Order {0}, BillofMaterial {1}, Line {2}
                                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextPickingPos(30)", 181, "Error50062",
                                                    pickingPos.Picking.PickingNo,
                                                    "-",
                                                    pickingPos.PickingMaterial.MaterialNo);

                                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                        Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                    return StartNextCompResult.CycleWait;

                                }
                                pickingPos.MDDelivPosLoadState = posLoadState;
                                dbApp.ACSaveChanges();
                                continue;
                            }
                            else if (NoSourceFoundForDosing.ValueT == 1)
                            {
                                return StartNextCompResult.CycleWait;
                            }
                        }
                        else if (NoSourceFoundForDosing.ValueT == 1)
                        {
                            NoSourceFoundForDosing.ValueT = 0;
                            AcknowledgeAlarms();
                        }

                        CurrentDosingRoute = dosingRoute;
                        NoSourceFoundForDosing.ValueT = 0;
                    }
                    else if (rResult == null || rResult.Routes == null || !rResult.Routes.Any() || (double.IsNaN(pickingPos.RemainingDosingWeight) && NoSourceFoundForDosing.ValueT == 1 || NoSourceFoundForDosing.ValueT == 2))
                    {
                        if (NoSourceFoundForDosing.ValueT == 0)
                        {
                            NoSourceWait = DateTime.Now + TimeSpan.FromSeconds(10);
                            NoSourceFoundForDosing.ValueT = 1;

                            // Error50063: No Route found for dosing component {2} at Order {0}, bill of material{1}
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(8)", 1080, "Error50063",
                                                                     pickingPos.Material.MaterialNo,
                                                                     picking.PickingNo,
                                                                     "-");

                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                            return StartNextCompResult.CycleWait;
                        }
                        else if (NoSourceFoundForDosing.ValueT == 2)
                        {
                            MDDelivPosLoadState posLoadState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();

                            if (posLoadState == null)
                            {
                                // Error50062: posState ist null at Order {0}, BillofMaterial {1}, Line {2}
                                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextPickingPos(30)", 181, "Error50062",
                                                pickingPos.Picking.PickingNo,
                                                "-",
                                                pickingPos.PickingMaterial.MaterialNo);

                                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                return StartNextCompResult.CycleWait;
                            }
                            pickingPos.MDDelivPosLoadState = posLoadState;
                            dbApp.ACSaveChanges();
                            continue;
                        }
                    }
                    else
                    {
                        dosingRoute = rResult.Routes.FirstOrDefault();

                        if (pickingPos.FromFacility != null)
                        {
                            var facilityCharges = dbApp.FacilityCharge
                                .Include("Facility.FacilityStock_Facility")
                                .Include("MDReleaseState")
                                .Include("FacilityLot.MDReleaseState")
                                .Include("Facility.MDFacilityType")
                                .Where(c => c.FacilityID == pickingPos.FromFacilityID && !c.NotAvailable).ToArray();
                            possibleSilos = new ACPartslistManager.QrySilosResult(facilityCharges);
                            possibleSilos.ApplyLotReservationFilter(pickingPos, 0);
                            possibleSilos.ApplyBlockedQuantsFilter();
                            dosingSilo = possibleSilos.FilteredResult.FirstOrDefault();
                        }

                        CurrentDosingRoute = dosingRoute;
                        NoSourceFoundForDosing.ValueT = 0;
                    }

                    // If Dosing by Reservations
                    if (dosingSilo != null 
                        && dosingSilo.StockOfReservations.HasValue 
                        && targetWeight > dosingSilo.StockOfReservations.Value 
                        && dosingSilo.StockFree.HasValue) // Has other quants, then dose exactly
                    {
                        targetWeight = dosingSilo.StockOfReservations.Value;
                    }

                    // 4. Starte Dosierung von diesem Silo aus
                    PAMSilo sourceSilo = CurrentDosingSilo(null);
                    if (sourceSilo == null)
                    {
                        // Error50064: Property sourceSilo is null at Order {0}, Bill of material {1}, Line {2}
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextPickingPos(8)", 1010, "Error50064",
                                        picking.PickingNo, pickingPos.Material.MaterialName1, pickingPos.Sequence);

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        return StartNextCompResult.CycleWait;
                    }
                    MDDelivPosLoadState posState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadingActive).FirstOrDefault();
                    if (posState == null)
                    {
                        // Error50065: MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.InProcess is null at Order {0}, Bill of material {1}, Line {2}
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextPickingPos(9)", 1020, "Error50065",
                                        picking.PickingNo, pickingPos.Material.MaterialName1, pickingPos.Sequence);

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        return StartNextCompResult.CycleWait;
                    }

                    core.datamodel.ACClassMethod refPAACClassMethod = RefACClassMethodOfContentWF;
                    if (refPAACClassMethod == null)
                        return StartNextCompResult.Done;

                    ACMethod acMethod = refPAACClassMethod.TypeACSignature();
                    if (acMethod == null)
                    {
                        //Error50154: acMethod is null.
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextPickingPos(9a)", 1030, "Error50154");

                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        return StartNextCompResult.CycleWait;
                    }
                    PAProcessFunction responsibleFunc = CanStartProcessFunc(module, acMethod, dbApp, pickingPos, sourceSilo);
                    if (responsibleFunc == null)
                    {
                        return StartNextCompResult.CycleWait;
                    }

                    ACRoutingParameters routingParametersDB = new ACRoutingParameters()
                    {
                        Database = dbIPlus,
                        Direction = RouteDirections.Backwards,
                        DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && c.ACClassID == scaleACClassID,
                        DBIncludeInternalConnections = true,
                        AutoDetachFromDBContext = false
                    };

                    var parentModule = ACRoutingService.DbSelectRoutesFromPoint(responsibleFunc.ComponentClass, responsibleFunc.PAPointMatIn1.PropertyInfo, routingParametersDB).FirstOrDefault();

                    var sourcePoint = parentModule?.FirstOrDefault()?.SourceACPoint?.PropertyInfo;
                    var sourceClass = parentModule?.FirstOrDefault()?.Source;
                    if (sourcePoint == null || sourceClass == null)
                    {
                        if (ComponentsSkippable)
                            continue;
                        else
                            return StartNextCompResult.CycleWait;
                    }

                    ACRoutingParameters routingParameters = new ACRoutingParameters()
                    {
                        RoutingService = this.RoutingService,
                        Database = dbIPlus,
                        AutoDetachFromDBContext = false,
                        SelectionRuleID = PAMSilo.SelRuleID_Silo,
                        Direction = RouteDirections.Backwards,
                        MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                        IncludeReserved = true,
                        IncludeAllocated = true
                    };

                    RoutingResult routeResult = ACRoutingService.FindSuccessorsFromPoint(sourceClass, sourcePoint, routingParameters);

                    if (!routeResult.Routes.Any(c => c.Any(x => x.SourceACComponent == sourceSilo)))
                    {
                        if (ComponentsSkippable)
                            continue;
                        else
                            return StartNextCompResult.CycleWait;
                    }

                    if (!(bool)ExecuteMethod(nameof(GetConfigForACMethod), acMethod, true, dbApp, pickingPos, sourceSilo))
                        return StartNextCompResult.CycleWait;

                    ACValue configuredQuantityValue = acMethod.ParameterValueList.GetACValue("TargetQuantity");
                    if (configuredQuantityValue != null)
                    {
                        double configuredQuantity = configuredQuantityValue.ParamAsDouble;
                        if (configuredQuantity > 0.00001)
                        {
                            if (targetWeight > configuredQuantity)
                            {
                                targetWeight = configuredQuantity;
                            }
                        }
                    }

                    if (pickingPos != null && double.IsNaN(pickingPos.RemainingDosingWeight) && double.IsNaN(targetWeight))
                    {
                        NoSourceFoundForDosing.ValueT = 1;
                        //Error50597: Dosing error on the component {0} {1}, {2};
                        string error = pickingPos.RemainingDosingWeightError;
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(9a)", 1111, "Error50597", pickingPos.Material.MaterialNo, pickingPos.Material.MaterialName1, error);
                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        return StartNextCompResult.CycleWait;
                    }

                    acMethod[PWMethodVBBase.IsLastBatchParamName] = (short)lastBatchMode;

                    acMethod["PLPosRelation"] = pickingPos.PickingPosID;
                    if (!ValidateAndSetRouteForParam(acMethod, dosingRoute))
                        return StartNextCompResult.CycleWait;
                    acMethod["Source"] = sourceSilo.RouteItemIDAsNum;
                    acMethod["TargetQuantity"] = targetWeight;
                    if (IsAutomaticContinousWeighing && totalizingScale != null)
                    {
                        var acValue = acMethod.ParameterValueList.GetACValue("SWTWeight");
                        if (acValue != null)
                            acValue.Value = totalizingScale.SWTTipWeight;
                    }
                    acMethod[Material.ClassName] = pickingPos.Material.MaterialName1;
                    if (pickingPos.Material.Density > 0.00001)
                        acMethod["Density"] = pickingPos.Material.Density;
                    if (dosingRoute != null)
                        dosingRoute.Detach(true);

                    if (!(bool)ExecuteMethod(nameof(AfterConfigForACMethodIsSet), acMethod, true, dbApp, pickingPos, sourceSilo))
                    {
                        return StartNextCompResult.CycleWait;
                    }

                    if (!acMethod.IsValid())
                    {
                        // Error50066: Dosingtask not startable Order {0}, Bill of material {1}, line {2}
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextPickingPos(10)", 1040, "Error50066",
                                        picking.PickingNo, pickingPos.Material.MaterialName1, pickingPos.Sequence);

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        return StartNextCompResult.CycleWait;
                    }

                    RecalcTimeInfo(true);
                    CurrentDosingPos.ValueT = pickingPos.PickingPosID;
                    if (CreateNewProgramLog(acMethod, _NewAddedProgramLog == null) <= CreateNewProgramLogResult.ErrorNoProgramFound)
                    {
                        return StartNextCompResult.CycleWait;
                    }
                    _ExecutingACMethod = acMethod;

                    module.TaskInvocationPoint.ClearMyInvocations(this);
                    _CurrentMethodEventArgs = null;
                    if (!IsTaskStarted(module.TaskInvocationPoint.AddTask(acMethod, this)))
                    {
                        ACMethodEventArgs eM = _CurrentMethodEventArgs;
                        if (eM == null || eM.ResultState != Global.ACMethodResultState.FailedAndRepeat)
                        {
                            // Error50066: Dosingtask not startable Order {0}, Bill of material {1}, line {2}
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextPickingPos(10a)", 1050, "Error50066",
                                        picking.PickingNo, pickingPos.Material.MaterialName1, pickingPos.Sequence);

                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        }
                        CurrentDosingPos.ValueT = Guid.Empty;
                        return StartNextCompResult.CycleWait;
                    }
                    UpdateCurrentACMethod();

                    CachedEmptySiloHandlingOption = null;
                    if (pwMethodTransport is PWMethodSingleDosing && lastBatchMode == PADosingLastBatchEnum.LastBatch && DoseAllPosFromPicking)
                    {
                        var posState2 = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();
                        if (posState2 != null)
                            posState = posState2;
                    }
                    pickingPos.MDDelivPosLoadState = posState;
                    MsgWithDetails msg2 = dbApp.ACSaveChanges();
                    if (msg2 != null)
                    {
                        Messages.LogException(this.GetACUrl(), "HandleState(5)", msg2.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, new Msg(msg2.InnerMessage, this, eMsgLevel.Exception, PWClassName, "StartNextPickingPos", 1060), true);
                        return StartNextCompResult.CycleWait;
                    }
                    ExecuteMethod(nameof(OnACMethodSended), acMethod, true, dbApp, pickingPos, sourceSilo, responsibleFunc);
                    AcknowledgeAlarms();

                    return StartNextCompResult.NextCompStarted;

                }

                return StartNextCompResult.Done;
            }
        }

        private bool ManageDosingStatePicking(ManageDosingStatesMode mode, DatabaseApp dbApp, out double sumQuantity)
        {
            sumQuantity = 0.0;
            PWMethodTransportBase pwMethodTransport = ParentPWMethod<PWMethodTransportBase>();
            if (pwMethodTransport == null || pwMethodTransport.CurrentPicking == null)
                return false;
            Picking picking = pwMethodTransport.CurrentPicking.FromAppContext<Picking>(dbApp);
            if (picking == null)
                return false;

            // Falls noch Dosierungen anstehen, dann dosiere nächste Komponente
            if (mode == ManageDosingStatesMode.ResetDosings || mode == ManageDosingStatesMode.SetDosingsCompleted)
            {
                if (ParentPWGroup == null || ParentPWGroup.AccessedProcessModule == null)
                    return false;
                string acUrl = ParentPWGroup.AccessedProcessModule.GetACUrl();

                bool changed = false;
                var resettingPosState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad).FirstOrDefault();
                if (resettingPosState != null)
                {
                    var queryDosings = picking.PickingPos_Picking.ToArray();
                    foreach (var childPos in queryDosings)
                    {
                        // Suche alle Dosierungen die auf DIESER Waage stattgefunden haben
                        IEnumerable<FacilityBooking> unconfirmedBookings = null;
                        if (childPos.OutOrderPos != null)
                            unconfirmedBookings = childPos.OutOrderPos.FacilityBooking_OutOrderPos.Where(c => c.PropertyACUrl == acUrl && c.MaterialProcessStateIndex == (short)GlobalApp.MaterialProcessState.New);
                        else
                            unconfirmedBookings = childPos.FacilityBooking_PickingPos.Where(c => c.PropertyACUrl == acUrl && c.MaterialProcessStateIndex == (short)GlobalApp.MaterialProcessState.New);
                        if (unconfirmedBookings != null && unconfirmedBookings.Any())
                        {
                            changed = true;
                            // Falls alle Komponenten entleert, setze Status auf Succeeded
                            foreach (var booking in unconfirmedBookings)
                            {
                                if (mode == ManageDosingStatesMode.SetDosingsCompleted)
                                    booking.MaterialProcessState = GlobalApp.MaterialProcessState.Processed;
                                else // (mode == ManageDosingStatesMode.ResetDosings)
                                    booking.MaterialProcessState = GlobalApp.MaterialProcessState.Discarded;
                                sumQuantity += booking.OutwardQuantity;
                            }
                            // Sonderentleerung, setze Status auf Teilerledigt
                            if (mode == ManageDosingStatesMode.ResetDosings)
                            {
                                childPos.MDDelivPosLoadState = resettingPosState;
                            }
                        }
                    }
                }
                return changed;
            }
            else
            {
                if (mode == ManageDosingStatesMode.QueryOpenDosings)
                {
                    var queryOpenDosings = picking.PickingPos_Picking.ToArray()
                                        .Where(c => c.MDDelivPosLoadState != null
                                                    && (c.MDDelivPosLoadState.DelivPosLoadState == MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad
                                                        || c.MDDelivPosLoadState.DelivPosLoadState == MDDelivPosLoadState.DelivPosLoadStates.LoadingActive))
                                        .OrderBy(c => c.Sequence);

                    // If not endless dosing, check Quantity
                    if (MinDosQuantity > -0.0000001)
                        queryOpenDosings = queryOpenDosings.Where(c => c.RemainingDosingQuantityUOM < -1.0).OrderBy(c => c.Sequence);

                    bool any = queryOpenDosings.Any();
                    if (any)
                        sumQuantity = queryOpenDosings.Sum(c => c.RemainingDosingQuantityUOM);
                    return any;
                }
                else if (mode == ManageDosingStatesMode.QueryHasAnyDosings)
                {
                    bool any = picking.PickingPos_Picking.Any();
                    if (any)
                        sumQuantity = picking.PickingPos_Picking.Sum(c => c.TargetQuantityUOM);
                    return any;
                }
                else //if (mode == ManageDosingStatesMode.QueryDosedComponents)
                {
                    if (ParentPWGroup == null)
                        return false;

                    PAProcessModule apm = ParentPWGroup.AccessedProcessModule != null ? ParentPWGroup.AccessedProcessModule : ParentPWGroup.PreviousAccessedPM;
                    if (apm == null)
                        return false;
                    string acUrl = apm.GetACUrl();

                    bool hasDosings = false;
                    var queryDosings = picking.PickingPos_Picking.ToArray();
                    foreach (var childPos in queryDosings)
                    {
                        IEnumerable<FacilityBooking> bookings = null;
                        if (childPos.OutOrderPos != null)
                            bookings = childPos.OutOrderPos.FacilityBooking_OutOrderPos.Where(c => c.PropertyACUrl == acUrl
                                                                && (c.MaterialProcessStateIndex == (short)GlobalApp.MaterialProcessState.New
                                                                    || c.MaterialProcessStateIndex == (short)GlobalApp.MaterialProcessState.Processed));
                        else
                            bookings = childPos.FacilityBooking_PickingPos.Where(c => c.PropertyACUrl == acUrl
                                                                && (c.MaterialProcessStateIndex == (short)GlobalApp.MaterialProcessState.New
                                                                   || c.MaterialProcessStateIndex == (short)GlobalApp.MaterialProcessState.Processed));

                        if (bookings.Any())
                        {
                            sumQuantity += bookings.Sum(c => c.OutwardQuantity);
                            hasDosings = true;
                        }
                    }
                    return hasDosings;
                }
            }
        }

        protected virtual EmptySiloHandlingOptions HandleAbortReasonOnEmptySiloPicking(PAMSilo silo)
        {
            if (!IsTransport)
                return EmptySiloHandlingOptions.NoSilosAvailable;

            if (CachedEmptySiloHandlingOption.HasValue)
                return CachedEmptySiloHandlingOption.Value;

            using (var dbIPlus = new Database())
            using (var dbApp = new DatabaseApp(dbIPlus))
            {
                PickingPos pickingPos = dbApp.PickingPos.FirstOrDefault(c => c.PickingPosID == CurrentDosingPos.ValueT);
                if (pickingPos != null && pickingPos.FromFacilityID == null)
                {
                    if (silo.Facility.ValueT == null || silo.Facility.ValueT.ValueT == null)
                    {
                        CachedEmptySiloHandlingOption = EmptySiloHandlingOptions.NoSilosAvailable;
                        return CachedEmptySiloHandlingOption.Value;
                    }

                    facility.ACPartslistManager.QrySilosResult possibleSilos;
                    RouteQueryParams queryParams = new RouteQueryParams(RouteQueryPurpose.HandleEmptySilo,
                        OldestSilo ? ACPartslistManager.SearchMode.OnlyEnabledOldestSilo : ACPartslistManager.SearchMode.SilosWithOutwardEnabled,
                        null, silo.Facility.ValueT.ValueT.FacilityID, ExcludedSilos, ReservationMode);
                    IEnumerable<Route> routes = GetRoutes(pickingPos, dbApp, dbIPlus, queryParams, null, out possibleSilos);
                    if (routes == null || !routes.Any())
                    {
                        if (AutoChangeScale && possibleSilos != null && possibleSilos.FilteredResult  != null && possibleSilos.FilteredResult.Any())
                        {
                            var parallelActiveDosings = RootPW.FindChildComponents<PWDosing>(c => c is PWDosing
                                    && c != this
                                    && (c as PWDosing).CurrentACState != ACStateEnum.SMIdle
                                    && (c as PWDosing).ParentPWGroup != null
                                    && (c as PWDosing).ParentPWGroup.AccessedProcessModule != null)
                                    .ToList();
                            if (parallelActiveDosings != null && parallelActiveDosings.Any())
                            {
                                foreach (var otherDosing in parallelActiveDosings)
                                {
                                    facility.ACPartslistManager.QrySilosResult alternativeSilos;
                                    RouteQueryParams queryParams2 = new RouteQueryParams(RouteQueryPurpose.HandleEmptySilo,
                                        OldestSilo ? ACPartslistManager.SearchMode.OnlyEnabledOldestSilo : ACPartslistManager.SearchMode.SilosWithOutwardEnabled,
                                        null, silo.Facility.ValueT.ValueT.FacilityID, ExcludedSilos, ReservationMode);
                                    IEnumerable<Route> alternativeRoutes = otherDosing.GetRoutes(pickingPos, dbApp, dbIPlus, queryParams2, null, out alternativeSilos);
                                    if (alternativeRoutes != null && alternativeRoutes.Any())
                                    {
                                        CachedEmptySiloHandlingOption = EmptySiloHandlingOptions.OtherSilosAvailable | EmptySiloHandlingOptions.AvailableOnOtherModule;
                                        if (queryParams2.SuggestedOptionResult > 0)
                                            CachedEmptySiloHandlingOption |= queryParams2.SuggestedOptionResult;
                                        return CachedEmptySiloHandlingOption.Value;
                                    }
                                }
                            }
                        }
                        CachedEmptySiloHandlingOption = EmptySiloHandlingOptions.NoSilosAvailable;
                        if (queryParams.SuggestedOptionResult > 0)
                            CachedEmptySiloHandlingOption |= queryParams.SuggestedOptionResult;
                        return CachedEmptySiloHandlingOption.Value;
                    }
                    else
                    {
                        CachedEmptySiloHandlingOption = EmptySiloHandlingOptions.OtherSilosAvailable | EmptySiloHandlingOptions.AvailabeOnThisModule;
                        if (queryParams.SuggestedOptionResult > 0)
                            CachedEmptySiloHandlingOption |= queryParams.SuggestedOptionResult;
                        return CachedEmptySiloHandlingOption.Value;
                    }
                }
            }

            CachedEmptySiloHandlingOption = EmptySiloHandlingOptions.NoSilosAvailable;
            return CachedEmptySiloHandlingOption.Value;
        }

        public virtual bool HasAndCanProcessAnyMaterialPicking(PAProcessModule module)
        {
            PWMethodTransportBase pwMethodTransport = ParentPWMethod<PWMethodTransportBase>();
            if (pwMethodTransport == null || pwMethodTransport.CurrentPicking == null)
                return false;

            using (var dbIPlus = new Database())
            using (var dbApp = new DatabaseApp(dbIPlus))
            {
                Picking picking = pwMethodTransport.CurrentPicking.FromAppContext<Picking>(dbApp);
                if (picking == null)
                    return false;
                PickingPos pickingPosFromPWMethod = null;
                if (pwMethodTransport.CurrentPickingPos != null && !DoseAllPosFromPicking)
                    pickingPosFromPWMethod = pwMethodTransport.CurrentPickingPos.FromAppContext<PickingPos>(dbApp);

                PickingPos[] openPickings = dbApp.PickingPos.Include(c => c.FromFacility.FacilityReservation_Facility)
                                //.Include(c => c.FromFacility.FacilityCharge_Facility)
                                .Where(c => c.PickingID == picking.PickingID
                                        && c.MDDelivPosLoadStateID.HasValue
                                        && (c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad
                                            || c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive))
                                .OrderBy(c => c.Sequence)
                                .ToArray();

                //PickingPos[] openPickings = picking.PickingPos_Picking
                //                                    .Where(c => c.MDDelivPosLoadStateID.HasValue
                //                                            && (c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad
                //                                             || c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive))
                //                                    .OrderBy(c => c.Sequence).ToArray();

                if ((ComponentsSeqFrom > 0 || ComponentsSeqTo > 0) && openPickings != null && openPickings.Any())
                {
                    openPickings = openPickings.Where(c => c.Sequence >= ComponentsSeqFrom && c.Sequence <= ComponentsSeqTo)
                                               .OrderBy(c => c.Sequence)
                                               .ToArray();
                }

                if (openPickings == null || !openPickings.Any())
                    return false;

                RoutingResult routingResult = HasAndCanProcessAnyMaterialPicking(module, dbApp, dbIPlus, openPickings.FirstOrDefault());
                return routingResult != null && routingResult.Routes != null && routingResult.Routes.Any();
            }
        }

        public virtual RoutingResult HasAndCanProcessAnyMaterialPicking(PAProcessModule module, DatabaseApp dbApp, Database db, PickingPos pickingPos)
        {
            facility.ACPartslistManager.QrySilosResult possibleSilos = null;
            RouteQueryParams queryParams = new RouteQueryParams(RouteQueryPurpose.StartDosing,
                            OldestSilo ? ACPartslistManager.SearchMode.OnlyEnabledOldestSilo : ACPartslistManager.SearchMode.SilosWithOutwardEnabled,
                            null, null, ExcludedSilos, ReservationMode);
            IEnumerable<Route> routes = GetRoutes(pickingPos, dbApp, db, queryParams, module, out possibleSilos);
            if (possibleSilos != null && possibleSilos.FoundSilos != null && possibleSilos.FoundSilos.Any())
            {
                foreach (ACPartslistManager.QrySilosResult.FacilitySumByLots prioSilo in possibleSilos.FilteredResult)
                {
                    if (!prioSilo.StorageBin.VBiFacilityACClassID.HasValue
                        || (pickingPos.ToFacilityID.HasValue && prioSilo.StorageBin.FacilityID == pickingPos.ToFacilityID))
                        continue;
                    return new RoutingResult(routes, false, null, null);
                }
            }
            return null;
        }

        public virtual Msg CanResumeDosingPicking()
        {
            PWMethodTransportBase pwMethodTransport = ParentPWMethod<PWMethodTransportBase>();
            if (pwMethodTransport == null || pwMethodTransport.CurrentPicking == null)
                return null;

            Msg msg = null;
            using (var dbIPlus = new Database())
            using (var dbApp = new DatabaseApp(dbIPlus))
            {
                Picking picking = pwMethodTransport.CurrentPicking.FromAppContext<Picking>(dbApp);
                if (picking == null)
                    return null;
                PickingPos pickingPos = null;
                if (CurrentDosingPos != null && CurrentDosingPos.ValueT != Guid.Empty)
                    pickingPos = dbApp.PickingPos.Where(c => c.PickingPosID == CurrentDosingPos.ValueT).FirstOrDefault();
                if (pickingPos == null && pwMethodTransport.CurrentPickingPos != null)
                    pickingPos = pwMethodTransport.CurrentPickingPos.FromAppContext<PickingPos>(dbApp);
                if (pickingPos == null)
                {
                    pickingPos = picking.PickingPos_Picking
                        .Where(c => c.MDDelivPosLoadStateID.HasValue
                                && (c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad
                                    || c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive))
                        .OrderBy(c => c.Sequence)
                        .FirstOrDefault();
                }
                if (pickingPos == null)
                    return null;
                RouteItem dosingSource = CurrentDosingSource(null);
                if (dosingSource == null)
                {
                    // Error50081: No Route found for booking component {2} at Order {0}, bill of material{1}
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "CanResumeDosingPicking(1)", 1210, "Error50081",
                                    pickingPos.Sequence, picking.PickingNo, pickingPos.Material.MaterialName1);
                    return msg;
                }

                Facility outwardFacility = dbApp.Facility.Where(c => c.VBiFacilityACClassID == dosingSource.SourceGuid).FirstOrDefault();
                if (outwardFacility == null)
                {
                    // Error50082: Facitlity not found for booking component {2} at Order {0}, bill of material{1}
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "CanResumeDosingPicking(2)", 1220, "Error50082",
                                    pickingPos.Sequence, picking.PickingNo, pickingPos.Material.MaterialName1);
                    return msg;
                }

                if (!outwardFacility.MaterialID.HasValue)
                {
                    // Error50262: The currently dosing Source {0} is not occupied with a material.
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "CanResumeDosingPicking(3)", 1312, "Error50262", outwardFacility.FacilityName);
                    return msg;
                }

                if (!(pickingPos.Material.MaterialID == outwardFacility.MaterialID.Value
                      || pickingPos.Material.ProductionMaterialID.HasValue && pickingPos.Material.ProductionMaterialID == outwardFacility.MaterialID.Value))
                {
                    // Error50263: The dosing Material {0} / {1} doesn't match Material {2} / {3} in Source {4}.
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "CanResumeDosingPicking(4)", 1313, "Error50263",
                                pickingPos.Material.MaterialNo, pickingPos.Material.MaterialName1,
                                outwardFacility.Material.MaterialNo, outwardFacility.Material.MaterialName1,
                                outwardFacility.FacilityName);
                    return msg;
                }
            }
            return null;
        }

        public virtual Msg DoDosingBookingPicking(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject,
                                    PADosingAbortReason dosingFuncResultState, PAFDosing dosing,
                                    string dis2SpecialDest, bool reEvaluatePosState,
                                    double? actualQuantity, double? tolerancePlus, double? toleranceMinus, double? targetQuantity,
                                    bool isEndlessDosing, bool inTol)
        {
            MsgWithDetails collectedMessages = new MsgWithDetails();
            Msg msg = null;
            PWMethodTransportBase pwMethodTransport = ParentPWMethod<PWMethodTransportBase>();
            if (!IsTransport || pwMethodTransport == null || pwMethodTransport.CurrentPicking == null)
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "DoDosingBookingPicking(1)",
                    Message = "Is not Transport"
                };
            }

            using (var dbApp = new DatabaseApp())
            {
                try
                {
                    Picking picking = pwMethodTransport.CurrentPicking.FromAppContext<Picking>(dbApp);
                    if (picking == null)
                    {
                        return new Msg
                        {
                            Source = GetACUrl(),
                            MessageLevel = eMsgLevel.Error,
                            ACIdentifier = "DoDosingBookingPicking(2)",
                            Message = "picking == null"
                        };
                    }

                    MDDelivPosLoadState posState = null;
                    //// Falls in Toleranz oder Dosierung abgebrochen ohne Grund, dann beende Position
                    //if (inTol
                    //    || dosingFuncResultState == PADosingAbortReason.CompCancelled
                    //    || dosingFuncResultState == PADosingAbortReason.EmptySourceEndBatchplan
                    //    || dosingFuncResultState == PADosingAbortReason.EndDosingThenDisThenNextComp
                    //    || dosingFuncResultState == PADosingAbortReason.EndTolErrorDosingThenDisThenNextComp
                    //    || dosingFuncResultState == PADosingAbortReason.EndDosingThenDisThenEnd
                    //    || dosingFuncResultState == PADosingAbortReason.EndTolErrorDosingThenDisThenEnd
                    //    )
                    //    posState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(db, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();
                    //else
                    //{
                    //    posState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(db, MDDelivPosLoadState.DelivPosLoadStates.LoadingActive).FirstOrDefault();
                    //}
                    //if (posState == null)
                    //{
                    //    // Error50080: MDProdOrderPartslistPosState for Completed-State doesn't exist
                    //    msg = new Msg
                    //    {
                    //        Source = GetACUrl(),
                    //        MessageLevel = eMsgLevel.Error,
                    //        ACIdentifier = "DoDosingBookingPicking(2)",
                    //        Message = Root.Environment.TranslateMessage(this, "Error50080")
                    //    };

                    //    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                    //    OnNewAlarmOccurred(ProcessAlarm, msg.Message, true);
                    //    return msg;
                    //}

                    PickingPos pickingPos = dbApp.PickingPos.Where(c => c.PickingPosID == CurrentDosingPos.ValueT).FirstOrDefault();
                    bool isSiloChangeOnEndlessDosing = (dosingFuncResultState == PADosingAbortReason.EmptySourceNextSource || dosingFuncResultState == PADosingAbortReason.MachineMalfunction) 
                                                        && isEndlessDosing 
                                                        && pickingPos != null 
                                                        && !pickingPos.FromFacilityID.HasValue;
                    if (   ((dosingFuncResultState == PADosingAbortReason.EmptySourceNextSource || dosingFuncResultState == PADosingAbortReason.MachineMalfunction) && !isSiloChangeOnEndlessDosing)
                        || dosingFuncResultState == PADosingAbortReason.EmptySourceEndBatchplan
                        || dosingFuncResultState == PADosingAbortReason.EndDosingThenDisThenEnd
                        || dosingFuncResultState == PADosingAbortReason.EndDosingThenDisThenNextComp
                        || dosingFuncResultState == PADosingAbortReason.EmptySourceAbortAdjustOtherAndWait)
                    {

                        if (ParentPWMethodVBBase != null)
                            ParentPWMethodVBBase.IsLastBatch = PADosingLastBatchEnum.LastBatch;
                    }

                    if (pickingPos != null)
                    {
                        bool changePosState = false;
                        //dosingPosRelation.MDProdOrderPartslistPosState = posState;

                        RouteItem dosingSource = CurrentDosingSource(null);
                        if (dosingSource == null)
                        {
                            // Error50081: No Route found for booking component {2} at Order {0}, bill of material{1}
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoDosingBookingPicking(3)", 1070, "Error50081",
                                            pickingPos.Sequence, picking.PickingNo, pickingPos.Material.MaterialName1);

                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                            return msg;
                        }

                        Facility outwardFacility = dbApp.Facility.Where(c => c.VBiFacilityACClassID == dosingSource.SourceGuid).FirstOrDefault();
                        if (outwardFacility == null)
                        {
                            // Error50082: Facitlity not found for booking component {2} at Order {0}, bill of material{1}
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoDosingBookingPicking(4)", 1080, "Error50082",
                                            pickingPos.Sequence, picking.PickingNo, pickingPos.Material.MaterialName1);

                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                            return msg;
                        }

                        changePosState = true;
                        double postingQuantity = actualQuantity.HasValue ? actualQuantity.Value : 0;
                        OnDoDosingBookingPickingGetPostingQuantity(collectedMessages, sender, e, wrapObject, dbApp, pickingPos, outwardFacility, dosingFuncResultState, ref postingQuantity);
                        // Falls dosiert
                        if (postingQuantity > 0.00001 || postingQuantity < -0.00001)
                        {
                            // 1. Bereite Buchung vor
                            FacilityPreBooking facilityPreBooking = ACFacilityManager.NewFacilityPreBooking(dbApp, pickingPos, null, postingQuantity);
                            ACMethodBooking bookingParam = facilityPreBooking.ACMethodBooking as ACMethodBooking;
                            bookingParam.OutwardQuantity = (double)postingQuantity;
                            bookingParam.OutwardFacility = outwardFacility;
                            if (outwardFacility.Material != null)
                                bookingParam.MDUnit = outwardFacility.Material.BaseMDUnit;
                            if (pickingPos.ToFacility != null)
                            {
                                var pwDisChargings = pwMethodTransport.FindChildComponents<PWDischarging>(c => c is PWDischarging).ToArray();
                                if (pwDisChargings.Any())
                                {
                                    PWDischarging nodeWithPostingBehaviour = pwDisChargings.Where(c => c.PostingBehaviour != PostingBehaviourEnum.NotSet).FirstOrDefault();
                                    if (nodeWithPostingBehaviour != null)
                                        bookingParam.PostingBehaviour = nodeWithPostingBehaviour.PostingBehaviour;
                                }
                            }

                            if (ParentPWGroup != null && ParentPWGroup.AccessedProcessModule != null)
                                bookingParam.PropertyACUrl = ParentPWGroup.AccessedProcessModule.GetACUrl();

                            msg = dbApp.ACSaveChangesWithRetry();

                            // 2. Führe Buchung durch
                            if (msg != null)
                            {
                                collectedMessages.AddDetailMessage(msg);
                                Messages.LogError(this.GetACUrl(), "DoDosingBookingPicking(5)", msg.InnerMessage);
                                OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoDosingBookingPicking", 1090), true);
                                changePosState = false;
                            }
                            else if (facilityPreBooking != null)
                            {
                                bookingParam.IgnoreIsEnabled = true;
                                ACMethodEventArgs resultBooking = ACFacilityManager.BookFacilityWithRetry(ref bookingParam, dbApp);
                                if (resultBooking.ResultState == Global.ACMethodResultState.Failed || resultBooking.ResultState == Global.ACMethodResultState.Notpossible)
                                {
                                    collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                                    OnNewAlarmOccurred(ProcessAlarm, new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoDosingBookingPicking", 1110), true);
                                    changePosState = false;
                                }
                                else
                                {
                                    if (!bookingParam.ValidMessage.IsSucceded() || bookingParam.ValidMessage.HasWarnings())
                                    {
                                        Messages.LogError(this.GetACUrl(), "DoDosingBookingPicking(6)", bookingParam.ValidMessage.InnerMessage);
                                        OnNewAlarmOccurred(ProcessAlarm, new Msg(bookingParam.ValidMessage.Message, this, eMsgLevel.Error, PWClassName, "DoDosingBookingPicking", 1100), true);
                                        changePosState = false;
                                    }
                                    if (bookingParam.ValidMessage.IsSucceded())
                                    {
                                        facilityPreBooking.DeleteACObject(dbApp, true);
                                        if (ACFacilityManager != null)
                                            ACFacilityManager.RecalcAfterPosting(dbApp, pickingPos, bookingParam.OutwardQuantity.Value, false);
                                        //pickingPos.IncreasePickingActualUOM(bookingParam.OutwardQuantity.Value);
                                        //dosingPosRelation.TopParentPartslistPosRelation.RecalcActualQuantity();
                                        //dosingPosRelation.SourceProdOrderPartslistPos.TopParentPartslistPos.RecalcActualQuantity();
                                    }
                                    else
                                    {
                                        collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                                    }

                                    if (   (!isEndlessDosing && (pickingPos.ActualQuantityUOM >= (pickingPos.TargetQuantityUOM - toleranceMinus)))
                                        || (ParentPWMethodVBBase != null && ParentPWMethodVBBase.IsLastBatch == PADosingLastBatchEnum.LastBatch))
                                    {
                                        changePosState = true;
                                        posState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();
                                    }

                                    msg = dbApp.ACSaveChangesWithRetry();
                                    if (msg != null)
                                    {
                                        collectedMessages.AddDetailMessage(msg);
                                        Messages.LogError(this.GetACUrl(), "DoDosingBookingPicking(8)", msg.InnerMessage);
                                        OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoDosingBookingPicking", 1120), true);
                                    }
                                    else
                                    {
                                        //pickingPos.RecalcActualQuantityFast();
                                        if (dbApp.IsChanged)
                                            dbApp.ACSaveChanges();
                                        // Bei Restentleerung wird in ein Sonderziel gefahren
                                        // => Es muss die selbe Menge wieder zurückgebucht werden auf ein Sonderlagerplatz
                                        if (!String.IsNullOrEmpty(dis2SpecialDest))
                                        {
                                            Facility specialDest = dbApp.Facility.Where(c => c.FacilityNo == dis2SpecialDest).FirstOrDefault();
                                            if (specialDest == null && outwardFacility.Facility1_ParentFacility != null)
                                            {
                                                specialDest = dbApp.Facility.Where(c => c.ParentFacilityID.HasValue
                                                    && c.ParentFacilityID == outwardFacility.ParentFacilityID
                                                    && c.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBin).FirstOrDefault();
                                            }
                                            if (specialDest != null && specialDest.MDFacilityType != null && specialDest.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBin)
                                            {
                                                var queryDoneOutwardBookings = dbApp.FacilityBookingCharge.Where(c => c.PickingPosID.HasValue && c.PickingPosID == pickingPos.PickingPosID).ToArray();
                                                foreach (FacilityBookingCharge fbc in queryDoneOutwardBookings)
                                                {
                                                    ReveseBookingToExtraFacility(collectedMessages, dbApp, fbc, specialDest, pickingPos);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        bool odbd = OnDosingBookingDone(collectedMessages, sender, e, wrapObject, dbApp, pickingPos, outwardFacility, dosingFuncResultState);

                        if (odbd)
                        {
                            if (       dosingFuncResultState == PADosingAbortReason.EmptySourceNextSource
                                    || dosingFuncResultState == PADosingAbortReason.EmptySourceEndBatchplan
                                    || dosingFuncResultState == PADosingAbortReason.EndDosingThenDisThenEnd
                                    || dosingFuncResultState == PADosingAbortReason.EndDosingThenDisThenNextComp
                                    || dosingFuncResultState == PADosingAbortReason.EmptySourceAbortAdjustOtherAndWait)
                            {
                                bool anyOtherFunctionActiveFromThisSilo = false;
                                // Before Silo is posted to Zero, ensure that other functions that are dosing from this same silo make their posting also
                                // Otherwise the stock not be correct
                                // Therefore only the last dosing that finishes can book this Silo to empty stock
                                if (outwardFacility.VBiFacilityACClass != null && !String.IsNullOrEmpty(outwardFacility.VBiFacilityACClass.ACURLComponentCached))
                                {
                                    PAMSilo currentSourceSilo = ACUrlCommand(outwardFacility.VBiFacilityACClass.ACURLComponentCached) as PAMSilo;
                                    if (currentSourceSilo != null)
                                    {
                                        IEnumerable<PAFDosing> dosingList = currentSourceSilo.GetActiveDosingsFromThisSilo();
                                        if (dosingList != null && dosingList.Any())
                                        {
                                            foreach (PAFDosing activeDosingFunct in dosingList)
                                            {
                                                if (activeDosingFunct != dosing
                                                    && activeDosingFunct.CurrentACState != ACStateEnum.SMIdle
                                                    && activeDosingFunct.IsTransportActive)
                                                {
                                                    if (activeDosingFunct.DosingAbortReason.ValueT == PADosingAbortReason.NotSet)
                                                    {
                                                        // This Method invokes normally an Abort or Stop, an then this function is called rekursively again!
                                                        activeDosingFunct.SetAbortReasonEmptyForced();
                                                    }
                                                }
                                            }

                                            // Check again if one of those functions are active, beacuse function didn't complete. Therfore the stock mustn't be set to zero!
                                            foreach (PAFDosing activeDosingFunct in dosingList)
                                            {
                                                if (activeDosingFunct != dosing
                                                    && activeDosingFunct.CurrentACState != ACStateEnum.SMIdle
                                                    && activeDosingFunct.IsTransportActive)
                                                {
                                                    anyOtherFunctionActiveFromThisSilo = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (!anyOtherFunctionActiveFromThisSilo)
                                {
                                    // Querytest, if antoher function has already posted silo to zero
                                    bool hasQuants = dbApp.FacilityCharge.Where(c => c.FacilityID ==  outwardFacility.FacilityID && c.NotAvailable == false).Any();

                                    //bool zeroBookSucceeded = false;
                                    if (hasQuants)
                                    {
                                        //zeroBookSucceeded = true;
                                        ACMethodBooking zeroBooking = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ZeroStock_Facility_BulkMaterial, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking;
                                        zeroBooking = zeroBooking.Clone() as ACMethodBooking;
                                        zeroBooking.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(dbApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);
                                        zeroBooking.InwardFacility = outwardFacility;
                                        if (ParentPWGroup != null && ParentPWGroup.AccessedProcessModule != null)
                                            zeroBooking.PropertyACUrl = ParentPWGroup.AccessedProcessModule.GetACUrl();
                                        //zeroBooking.OutwardFacility = outwardFacility;
                                        zeroBooking.IgnoreIsEnabled = true;
                                        ACMethodEventArgs resultBooking = ACFacilityManager.BookFacilityWithRetry(ref zeroBooking, dbApp);
                                        if (resultBooking.ResultState == Global.ACMethodResultState.Failed || resultBooking.ResultState == Global.ACMethodResultState.Notpossible)
                                        {
                                            collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                                            //zeroBookSucceeded = false;
                                            OnNewAlarmOccurred(ProcessAlarm, new Msg(zeroBooking.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoDosingBookingPicking", 1140), true);
                                        }
                                        else
                                        {
                                            if (!zeroBooking.ValidMessage.IsSucceded() || zeroBooking.ValidMessage.HasWarnings())
                                            {
                                                if (!zeroBooking.ValidMessage.IsSucceded())
                                                    collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                                                Messages.LogError(this.GetACUrl(), "DoDosingBookingPicking(9)", zeroBooking.ValidMessage.InnerMessage);
                                                OnNewAlarmOccurred(ProcessAlarm, new Msg(zeroBooking.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoDosingBookingPicking", 1130), true);
                                            }
                                            //else
                                                //zeroBookSucceeded = true;
                                        }
                                    }

                                    // Unnecessary since new Property in Facility-Table and handeled in FacilityManager
                                    //if (!hasQuants || zeroBookSucceeded)
                                    //{
                                    //    PAMSilo sourceSilo = null;
                                    //    bool disChargingActive = false;
                                    //    if (outwardFacility.FacilityACClass != null)
                                    //    {
                                    //        string url = outwardFacility.FacilityACClass.GetACUrlComponent();
                                    //        if (!String.IsNullOrEmpty(url))
                                    //        {
                                    //            sourceSilo = ACUrlCommand(url) as PAMSilo;
                                    //            if (sourceSilo != null)
                                    //            {
                                    //                IEnumerable<PAFDischarging> activeDischargings = sourceSilo.GetActiveDischargingsToThisSilo();
                                    //                disChargingActive = activeDischargings != null && activeDischargings.Any();
                                    //            }
                                    //        }
                                    //    }

                                    //    // #iP-T-24-05-08-002
                                    //    if (!disChargingActive
                                    //        && (sourceSilo == null || !sourceSilo.LeaveMaterialOccupation))
                                    //    {
                                    //        outwardFacility.Material = null; // Automatisches Löschen der Belegung?
                                    //        outwardFacility.Partslist = null;
                                    //    }
                                    //}

                                    if (ParentPWMethodVBBase != null && ParentPWMethodVBBase.IsLastBatch == PADosingLastBatchEnum.LastBatch)
                                    {
                                        changePosState = true;
                                        posState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();
                                    }

                                    msg = dbApp.ACSaveChangesWithRetry();
                                    if (msg != null)
                                    {
                                        collectedMessages.AddDetailMessage(msg);
                                        Messages.LogError(this.GetACUrl(), "DoDosingBookingPicking(10a)", msg.InnerMessage);
                                        OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoDosingBookingPicking", 1150), true);
                                    }
                                    OnBookingEmptySource(collectedMessages, sender, e, wrapObject, dbApp, pickingPos, dosingFuncResultState);
                                }
                            }
                        }

                        // Positionstate must be set at last because of conccurrency-Problems if another Scale(PWGroup) is waiting for starting this dosing in the Applicationthread
                        if (changePosState && posState != null)
                        {
                            pickingPos.MDDelivPosLoadState = posState;
                            msg = dbApp.ACSaveChanges();
                            if (msg != null)
                            {
                                Messages.LogError(this.GetACUrl(), "DoDosingBookingPicking(10b)", msg.InnerMessage);
                                OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoDosingBookingPicking", 1150), true);
                            }
                        }
                    }
                    // Sonst 
                    else
                    {
                        msg = dbApp.ACSaveChanges();
                        if (msg != null)
                        {
                            collectedMessages.AddDetailMessage(msg);
                            Messages.LogError(this.GetACUrl(), "DoDosingBookingPicking(11)", msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoDosingBookingPicking", 1170), true);
                        }
                    }
                }
                catch (Exception ec)
                {
                    string msgEc = ec.Message;
                    collectedMessages.AddDetailMessage(new Msg(eMsgLevel.Exception, msgEc));
                    Messages.LogException("PWDosing_Picking", "DoDosingBookingPicking(98)", ec);
                    msgEc = ec.StackTrace;
                    if (!String.IsNullOrEmpty(msgEc))
                        Messages.LogException("PWDosing_Picking", "DoDosingBookingPicking(99)", msgEc);
                }
                finally
                {
                    if (dbApp.IsChanged)
                    {
                        dbApp.ACSaveChanges();
                    }
                }
            }

            return collectedMessages.MsgDetailsCount > 0 ? collectedMessages : null;
        }

        public virtual void OnDoDosingBookingPickingGetPostingQuantity(MsgWithDetails collectedMessages, IACPointNetBase sender, ACEventArgs e, IACObject wrapObject,
                                        DatabaseApp dbApp, PickingPos pickingPos, Facility outwardFacility,
                                        PADosingAbortReason dosingFuncResultState, ref double postingQuantity)
        {
        }

        /// <summary>
        /// 4.2 Benachrichtigungsmethode, dass Siloabbuchung stattgefunden hat. Hier kann die Subclass noch zusätzliche stati setzten
        /// Falls Silo leer und das Silo leergebucht werden soll, dann muss true zurückgegeben werden andernfalls kann mit false die Mullbestandsbuchung in der Basisklasse ausgeschaltet werden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dbApp"></param>
        /// <param name="dosingPosRelation"></param>
        /// <returns>False if Silo ist empty and Booking to Zero is done in subclass</returns>
        public virtual bool OnDosingBookingDone(MsgWithDetails collectedMessages, IACPointNetBase sender, ACEventArgs e, IACObject wrapObject,
                                                DatabaseApp dbApp, PickingPos pickingPos, Facility outwardFacility,
                                                PADosingAbortReason dosingFuncResultState)
        {
            if (pickingPos != null)
            {
                bool inEmptyingMode = ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode)
                    || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode);

                bool dosingQuantityReached = pickingPos.RemainingDosingWeight >= -1; // No Endless Dosing
                bool isEndlessDosing = MinDosQuantity <= -0.00001;

                if (   inEmptyingMode
                    || (dosingQuantityReached && !isEndlessDosing))
                {
                    pickingPos.MDDelivPosLoadState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();
                }

                if (!inEmptyingMode)
                {
                    bool dosingLoop = this.FindSuccessors<PWDosingLoop>(true, c => c is PWDosingLoop, d => d is PWDosing).Any();

                    if (dosingLoop)
                    {
                        var pwGroup = ParentPWGroup as PWGroupVB;

                        bool isSiloChangeOnEndlessDosing = (dosingFuncResultState == PADosingAbortReason.EmptySourceNextSource || dosingFuncResultState == PADosingAbortReason.MachineMalfunction)
                                        && isEndlessDosing
                                        && pickingPos != null
                                        && !pickingPos.FromFacilityID.HasValue;

                        if (isEndlessDosing
                            && (dosingFuncResultState == PADosingAbortReason.NotSet || isSiloChangeOnEndlessDosing)
                            && pickingPos.MDDelivPosLoadState != null
                            && pickingPos.MDDelivPosLoadState.DelivPosLoadState < MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck)
                        {
                            pwGroup.CurrentACSubState = (uint)ACSubStateEnum.SMInterDischarging;
                        }
                        else if (!isEndlessDosing && !dosingQuantityReached)
                        {
                            pwGroup.CurrentACSubState = (uint)ACSubStateEnum.SMInterDischarging;
                        }
                        else
                        {
                            pwGroup.CurrentACSubState = (uint)ACSubStateEnum.SMIdle;
                            pickingPos.MDDelivPosLoadState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();
                        }
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// 4.3 Benachrichtigungsmethode für Subclass, dass aktuelles Silo auf Nullbestand gebucht worden ist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="wrapObject"></param>
        /// <param name="dbApp"></param>
        /// <param name="dosingPosRelation"></param>
        public virtual void OnBookingEmptySource(MsgWithDetails collectedMessages, IACPointNetBase sender, ACEventArgs e, IACObject wrapObject,
            DatabaseApp dbApp, PickingPos pickingPos, PADosingAbortReason dosingFuncResultState)
        {
        }


        /// <summary>
        /// Hilfsmethode um eine Entnahmebuchung von dem ursprünglichen Silo in ein Sonderziel zurckzubuchen
        /// </summary>
        /// <param name="dbApp"></param>
        /// <param name="fbc"></param>
        /// <param name="specialDest"></param>
        /// <param name="dosingPosRelation"></param>
        protected void ReveseBookingToExtraFacility(MsgWithDetails collectedMessages, DatabaseApp dbApp, FacilityBookingCharge fbc, Facility specialDest, PickingPos pickingPos)
        {
        }

    }
}
