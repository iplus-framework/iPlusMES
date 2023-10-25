using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    public partial class PWManualWeighing
    {

        public ACPickingManager PickingManager
        {
            get
            {
                PWMethodTransportBase pwMethodTransport = ParentPWMethod<PWMethodTransportBase>();
                return pwMethodTransport != null ? pwMethodTransport.PickingManager : null;
            }
        }

        public virtual StartNextCompResult StartManualWeighingPicking(PAProcessModule module)
        {
            Msg msg = null;

            if (!Root.Initialized)
                return StartNextCompResult.CycleWait;

            if (module == null)
            {
                // Error50274: The PAProcessModule is null.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(10)", 956, "Error50274");
                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, false);
                return StartNextCompResult.CycleWait;
            }

            PWMethodTransportBase pwMethodTransport = ParentPWMethod<PWMethodTransportBase>();
            if (pwMethodTransport == null || pwMethodTransport.CurrentPicking == null)
                return StartNextCompResult.Done;

            using (var dbIPlus = new Database())
            {
                using (var dbApp = new DatabaseApp(dbIPlus))
                {
                    Picking picking = pwMethodTransport.CurrentPicking.FromAppContext<Picking>(dbApp);
                    if (picking == null)
                        return StartNextCompResult.Done;

                    PickingPos[] openPickings = picking.PickingPos_Picking
                                                        .Where(c => c.MDDelivPosLoadStateID.HasValue
                                                                && c.TargetQuantityUOM > 0.00001
                                                                && (c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad
                                                                 || c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive))
                                                        .OrderBy(c => c.Sequence).ToArray();

                    if ((ComponentsSeqFrom > 0 || ComponentsSeqTo > 0) && openPickings != null && openPickings.Any())
                        openPickings = openPickings.Where(c => c.Sequence >= ComponentsSeqFrom && c.Sequence <= ComponentsSeqTo)
                                                   .OrderBy(c => c.Sequence)
                                                   .ToArray();

                    if (openPickings == null || !openPickings.Any())
                    {
                        return StartNextCompResult.Done;
                    }


                    using (ACMonitor.Lock(_65050_WeighingCompLock))
                    {
                        WeighingComponents = openPickings.Select(c => new WeighingComponent(c, DetermineWeighingComponentState(c.MDDelivPosLoadState.MDDelivPosLoadStateIndex))).ToList();
                    }

                    AvailableRoutes = GetAvailableStorages(module);
                }
            }

            return StartNextCompResult.NextCompStarted;
        }

        private void SMRunning_Picking()
        {
            try
            {
                PAFManualWeighing manualWeighing = CurrentExecutingFunction<PAFManualWeighing>();

                bool isWeighComponentsNull;
                using (ACMonitor.Lock(_65050_WeighingCompLock))
                {
                    isWeighComponentsNull = WeighingComponents == null;
                }

                if (isWeighComponentsNull
                    || (AutoInterDis && manualWeighing == null))
                {
                    if (ParentPWGroup.AccessedProcessModule == null)
                    {
                        SubscribeToProjectWorkCycle();
                        return;
                    }

                    StartNextCompResult result = StartNextCompResult.Done;
                    if (IsTransport)
                    {
                        if (!ParentPWGroup.IsInSkippingMode)
                            result = StartManualWeighingPicking(ParentPWGroup.AccessedProcessModule);
                    }

                    if (result == StartNextCompResult.CycleWait)
                    {
                        SubscribeToProjectWorkCycle();
                        return;
                    }
                    else if (result == StartNextCompResult.Done)
                    {
                        UnSubscribeToProjectWorkCycle();
                        // Falls durch tiefere Callstacks der Status schon weitergeschaltet worden ist, dann schalte Status nicht weiter
                        if (CurrentACState == ACStateEnum.SMRunning)
                            CurrentACState = ACStateEnum.SMCompleted;
                        return;
                    }

                    //Check if manual weighing currently active
                    if (manualWeighing != null)
                    {
                        ACValue plPosRelation = manualWeighing.CurrentACMethod.ValueT.ParameterValueList.GetACValue("PLPosRelation");
                        Guid? currentOpenMat = null;
                        if (plPosRelation != null && plPosRelation.Value != null)
                            currentOpenMat = plPosRelation.ParamAsGuid;

                        if (currentOpenMat == null)
                        {
                            SubscribeToProjectWorkCycle();
                            return;
                        }

                        CurrentOpenMaterial = currentOpenMat;

                        WeighingComponent comp = GetWeighingComponentPicking(currentOpenMat);

                        if (comp == null)
                        {
                            SubscribeToProjectWorkCycle();
                            return;
                        }

                        ACValue facilityCharge = manualWeighing.CurrentACMethod.ValueT.ParameterValueList.GetACValue("FacilityCharge");
                        if (facilityCharge != null && facilityCharge.Value != null)
                        {
                            CurrentFacilityCharge = facilityCharge.ParamAsGuid;
                            comp.SwitchState(WeighingComponentState.InWeighing);
                            SetInfo(comp, WeighingComponentInfoType.StateSelectCompAndFC_F, facilityCharge.ParamAsGuid);
                        }
                        else
                        {
                            //_ExitFromWaitForFC = false;
                            SetInfo(comp, WeighingComponentInfoType.SelectCompReturnFC_F, null);
                        }
                    }
                }

                if (manualWeighing != null)
                {
                    UnSubscribeToProjectWorkCycle();
                    return;
                }

                bool isAnyNeedToWeigh = false;
                using (ACMonitor.Lock(_65050_WeighingCompLock))
                {
                    if (WeighingComponents != null)
                    {
                        isAnyNeedToWeigh = WeighingComponents.Any(c => c.WeighState < WeighingComponentState.InWeighing);
                    }
                    else
                    {
                        Messages.LogMessage(eMsgLevel.Info, this.GetACUrl(), nameof(SMRunning), "The property WeighingComponents is null(10)");
                    }
                }

                Guid? currentOpenMaterial = CurrentOpenMaterial;

                if (!FreeSelectionMode)
                {
                    if (currentOpenMaterial == null)
                    {
                        WeighingComponent nextComp = null;
                        using (ACMonitor.Lock(_65050_WeighingCompLock))
                        {
                            nextComp = WeighingComponents.OrderBy(c => c.Sequence).FirstOrDefault(c => c.WeighState == WeighingComponentState.ReadyToWeighing);
                        }
                        if (nextComp == null)
                        {
                            bool isAllCompleted = RefreshCompStateFromDBAndCheckIsAllCompletedPicking();

                            if (isAllCompleted)
                            {
                                UnSubscribeToProjectWorkCycle();
                                CurrentACState = ACStateEnum.SMCompleted;
                                return;
                            }
                            else
                            {
                                SubscribeToProjectWorkCycle();
                                return;
                            }
                        }

                        CurrentOpenMaterial = nextComp.PickingPosition.PickingPosID;
                        bool hasQuants = TryAutoSelectFacilityChargePicking(nextComp.PickingPosition.PickingPosID);

                        StartNextCompResult funcStartResult = StartManualWeighingNextComp(ParentPWGroup.AccessedProcessModule, nextComp, hasQuants); //Auto Comp && Auto Lot
                        if (funcStartResult == StartNextCompResult.CycleWait)
                        {
                            CurrentOpenMaterial = null;
                            SubscribeToProjectWorkCycle();
                            return;
                        }

                        Guid? currentFacilityCharge = CurrentFacilityCharge;

                        if (!currentFacilityCharge.HasValue)
                        {
                            //_ExitFromWaitForFC = false;
                            SetInfo(nextComp, WeighingComponentInfoType.SelectCompReturnFC_F, currentFacilityCharge);
                            //ThreadPool.QueueUserWorkItem((object state) => WaitForFacilityChargeOrFacility(CurrentOpenMaterial, WeighingComponentInfoType.State)); //Auto Comp && Man Lot
                        }
                    }
                    else
                        UnSubscribeToProjectWorkCycle();
                }
                else if ((currentOpenMaterial != _LastOpenMaterial || _LastOpenMaterial == null) && isAnyNeedToWeigh)
                {
                    WeighingComponent nextComp = GetWeighingComponentPicking(currentOpenMaterial);
                    //if (nextComp == null)
                    //{
                    //    SubscribeToProjectWorkCycle();
                    //    return;
                    //}

                    Guid? currentFacilityCharge = CurrentFacilityCharge;

                    if (currentFacilityCharge != null)
                    {
                        StartManualWeighingNextComp(ParentPWGroup.AccessedProcessModule, nextComp, null); //Man Comp && Auto Lot || ManLot
                        UnSubscribeToProjectWorkCycle();
                    }
                    else if (AutoSelectLot && currentOpenMaterial != null)
                    {
                        bool hasQuants = TryAutoSelectFacilityChargePicking(currentOpenMaterial);
                        if (currentFacilityCharge != null)
                        {
                            StartManualWeighingNextComp(ParentPWGroup.AccessedProcessModule, nextComp, hasQuants); //Man Comp && Auto Lot || ManLot
                            UnSubscribeToProjectWorkCycle();
                        }
                    }
                    else
                    {
                        StartManualWeighingNextComp(ParentPWGroup.AccessedProcessModule, nextComp, null);
                        UnSubscribeToProjectWorkCycle();
                        SetCanStartFromBSO(true);
                    }
                }
                else
                {
                    bool isAnyReadyToWeigh = false;
                    using (ACMonitor.Lock(_65050_WeighingCompLock))
                    {
                        isAnyReadyToWeigh = WeighingComponents.Any(c => c.WeighState == WeighingComponentState.ReadyToWeighing);
                    }

                    if (isAnyReadyToWeigh)
                        SubscribeToProjectWorkCycle();
                    else
                    {
                        bool isAllCompleted = RefreshCompStateFromDBAndCheckIsAllCompletedPicking();

                        if (isAllCompleted)
                        {
                            UnSubscribeToProjectWorkCycle();
                            CurrentACState = ACStateEnum.SMCompleted;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string message = "";
                if (e.InnerException != null)
                    message = String.Format("{0}, {1}", e.Message, e.InnerException.Message);
                else
                    message = e.Message;

                Msg msg = new Msg(message, this, eMsgLevel.Exception, PWClassName, "SMRunning(10)", 772);
                Messages.LogMessageMsg(msg);
            }
        }


        private bool RefreshCompStateFromDBAndCheckIsAllCompletedPicking()
        {
            bool result = true;

            PWMethodTransportBase pwMethodTransport = ParentPWMethod<PWMethodTransportBase>();
            if (pwMethodTransport == null || pwMethodTransport.CurrentPicking == null)
                return false;

            using (var dbIPlus = new Database())
            {
                using (var dbApp = new DatabaseApp(dbIPlus))
                {
                    Picking picking = pwMethodTransport.CurrentPicking.FromAppContext<Picking>(dbApp);
                    if (picking == null)
                        return false;

                    PickingPos[] openPickings = picking.PickingPos_Picking
                                                       .OrderBy(c => c.Sequence).ToArray();

                    using (ACMonitor.Lock(_65050_WeighingCompLock))
                    {
                        foreach (PickingPos pickingPos in openPickings)
                        {
                            WeighingComponent comp = WeighingComponents.FirstOrDefault(c => c.PickingPosition.PickingPosID == pickingPos.PickingPosID);
                            if (comp == null)
                                continue;

                            WeighingComponentState newState = DetermineWeighingComponentState(pickingPos.MDDelivPosLoadState.MDDelivPosLoadStateIndex, true);
                            if (newState == WeighingComponentState.ReadyToWeighing)
                            {
                                comp.SwitchState(newState);
                                comp.TargetWeight = Math.Abs(pickingPos.RemainingDosingWeight);
                                SetInfo(comp, WeighingComponentInfoType.State, null);
                            }

                            if (comp.WeighState != WeighingComponentState.WeighingCompleted && comp.WeighState != WeighingComponentState.Aborted)
                            {
                                result = false;
                            }
                        }
                    }
                }
            }
            return result;
        }

        private bool TryAutoSelectFacilityChargePicking(Guid? materialID)
        {
            using (Database db = new core.datamodel.Database())
            using (DatabaseApp dbApp = new DatabaseApp(db))
            {
                PickingPos pickingPos = dbApp.PickingPos.FirstOrDefault(c => c.PickingPosID == materialID);
                if (pickingPos != null)
                {
                    Material mat = pickingPos.Material;
                    if (mat != null)
                    {
                        Msg msg = null;
                        var availableFC = GetFacilityChargesForMaterialPicking(dbApp, pickingPos);
                        if (!AutoSelectLot)
                            return availableFC.Any();

                        if (mat.IsLotManaged)
                        {
                            if (availableFC != null && availableFC.Count() > 1 && MultipleLotsSelectionRule.HasValue)
                            {
                                if (MultipleLotsSelectionRule.Value != LotSelectionRuleEnum.None)
                                    return availableFC.Any();
                            }

                            switch (AutoSelectLotPriority)
                            {
                                case LotUsageEnum.FIFO:
                                    availableFC = availableFC.OrderBy(c => c.FillingDate.HasValue).ThenBy(c => c.FillingDate).ToArray();
                                    break;
                                case LotUsageEnum.ExpirationFirst:
                                    availableFC = availableFC.OrderBy(c => c.ExpirationDate.HasValue).ThenBy(c => c.ExpirationDate).ToArray();
                                    break;
                                case LotUsageEnum.LastUsed:
                                    availableFC = TryGetLastUsedLot(availableFC, mat.MaterialID, dbApp);
                                    break;
                                case LotUsageEnum.LIFO:
                                    availableFC = availableFC.OrderByDescending(c => c.FillingDate.HasValue).ThenByDescending(c => c.FillingDate).ToArray();
                                    break;
                            }
                        }

                        foreach (var fc in availableFC)
                        {
                            if (mat != null && fc.MaterialID != mat.MaterialID)
                            {
                                Messages.LogError(this.GetACUrl(), "Wrong quant(20)", "The quant ID: " + fc.FacilityChargeID + ", material ID: " +
                                                  mat?.MaterialID);
                            }

                            msg = SetFacilityCharge(fc.FacilityChargeID, materialID);
                            if (msg == null)
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        private IEnumerable<FacilityCharge> GetFacilityChargesForMaterialPicking(DatabaseApp dbApp, PickingPos pickingPos)
        {
            Guid[] facilities = GetAvailableFacilitiesForMaterialPicking(dbApp, pickingPos).Select(c => c.FacilityID).ToArray();

            if (ACFacilityManager == null)
                return null;

            return ACFacilityManager.ManualWeighingFacilityChargeListQuery(dbApp, facilities, pickingPos.Material.MaterialID);
        }

        public IEnumerable<Facility> GetAvailableFacilitiesForMaterialPicking(DatabaseApp dbApp, PickingPos pickingPos)
        {
            if (ParentPWGroup == null || ParentPWGroup.AccessedProcessModule == null || PickingManager == null)
            {
                throw new NullReferenceException("AccessedProcessModule is null");
            }

            facility.ACPartslistManager.QrySilosResult facilities;

            core.datamodel.ACClass accessAClass = ParentPWGroup.AccessedProcessModule.ComponentClass;
            IEnumerable<Route> routes = PickingManager.GetRoutes(pickingPos, dbApp, dbApp.ContextIPlus,
                                        accessAClass,
                                        ACPartslistManager.SearchMode.SilosWithOutwardEnabled,
                                        null,
                                        out facilities,
                                        null,
                                        null,
                                        null,
                                        false);                                                        

            if (routes == null || facilities == null || facilities.FilteredResult == null || !facilities.FilteredResult.Any())
                return new List<Facility>();

            var routeList = routes.ToList();

            List<Facility> routableFacilities = new List<Facility>();

            PAFManualWeighing manWeigh = CurrentExecutingFunction<PAFManualWeighing>();
            if (manWeigh == null)
            {
                core.datamodel.ACClassMethod refPAACClassMethod = RefACClassMethodOfContentWF;
                ACMethod acMethod = refPAACClassMethod?.TypeACSignature();
                if (acMethod != null)
                {
                    PAProcessModule module = ParentPWGroup.AccessedProcessModule;
                    manWeigh = CanStartProcessFunc(module, acMethod) as PAFManualWeighing;
                }
            }

            foreach (Route currRoute in routes)
            {
                RouteItem lastRouteItem = currRoute.Items.LastOrDefault();
                if (lastRouteItem != null && lastRouteItem.TargetProperty != null)
                {
                    // Gehe zur nächsten Komponente, weil es mehrere Dosierfunktionen gibt und der Eingangspunkt des Prozessmoduls nicht mit dem Eingangspunkt dieser Funktion übereinstimmt.
                    // => eine andere Funktion ist dafür zuständig
                    if (manWeigh != null && !manWeigh.PAPointMatIn1.ConnectionList.Where(c => ((c as PAEdge).Source as PAPoint).ACIdentifier == lastRouteItem.TargetProperty.ACIdentifier).Any())
                    {
                        routeList.Remove(currRoute);
                    }
                    else
                    {
                        RouteItem source = currRoute.GetRouteSource();
                        if (source != null)
                        {
                            facility.ACPartslistManager.QrySilosResult.FacilitySumByLots facilityToAdd = facilities.FilteredResult.FirstOrDefault(c => c.StorageBin.VBiFacilityACClassID.HasValue && c.StorageBin.VBiFacilityACClassID == source.SourceGuid);
                            if (facilityToAdd != null)
                                routableFacilities.Add(facilityToAdd.StorageBin);
                        }
                    }
                }
            }

            if (!IncludeContainerStores)
            {
                routableFacilities = routableFacilities.Where(c => c.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBin).ToList();
            }

            return routableFacilities;
        }

        private void OnDeletedTaskPicking(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject, ACMethodEventArgs eM)
        {
            if (DiffWeighing)
            {
                return;
            }
            else
            {
                try
                {
                    PAProcessModule module = sender.ParentACComponent as PAProcessModule;
                    if (module != null)
                    {
                        PAProcessFunction function = module.GetExecutingFunction<PAProcessFunction>(eM.ACRequestID);
                        if (function != null)
                        {
                            WeighingComponentState state = WeighingComponentState.WeighingCompleted;
                            Msg msg = null;
                            bool changeState = false;

                            Guid? currentOpenMaterial = CurrentOpenMaterial;
                            Guid? currentFacilityCharge = CurrentFacilityCharge;

                            if (function.CurrentACState == ACStateEnum.SMCompleted || function.CurrentACState == ACStateEnum.SMAborted ||
                               (function.CurrentACState == ACStateEnum.SMIdle && function.LastACState == ACStateEnum.SMResetting))
                            {
                                bool isComponentConsumed = false;
                                ACValue isCC = e.GetACValue("IsComponentConsumed");
                                if (isCC != null)
                                    isComponentConsumed = isCC.ParamAsBoolean;

                                ACMethod parentACMethod = e.ParentACMethod;

                                double? actWeight = e.GetDouble("ActualQuantity");
                                //double? tolerancePlus = (double)e.ParentACMethod["TolerancePlus"];
                                double? toleranceMinus = (double)parentACMethod["ToleranceMinus"];
                                double? targetQuantity = (double)parentACMethod["TargetQuantity"];

                                bool isWeighingInTol = true;

                                if (targetQuantity.HasValue && actWeight.HasValue && toleranceMinus.HasValue)
                                {
                                    double actWeightRounded = Math.Round(actWeight.Value, 5);
                                    if (actWeightRounded < Math.Round(targetQuantity.Value - toleranceMinus.Value, 5))
                                    {
                                        isWeighingInTol = false;
                                    }
                                }

                                if (actWeight > 0.000001)
                                {
                                    msg = DoManualWeighingBookingPicking(actWeight, isWeighingInTol, false, currentOpenMaterial, currentFacilityCharge, false,
                                                                         false, targetQuantity);
                                    _ScaleComp = false;
                                }
                                else if (isWeighingInTol)
                                {
                                    SetRelationStatePicking(currentOpenMaterial, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck);
                                }

                                if (isComponentConsumed)
                                {
                                    Msg msgResult = SetRelationStatePicking(currentOpenMaterial, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck);
                                    if (msgResult != null)
                                        ActivateProcessAlarmWithLog(msgResult);
                                }

                                if (CurrentACMethod?.ValueT != null)
                                {
                                    RecalcTimeInfo();
                                    FinishProgramLog(CurrentACMethod.ValueT);
                                }

                                if (_IsBinChangeActivated)
                                {
                                    Reset();
                                    RaiseOutEvent();
                                }

                                changeState = true;
                            }

                            if (msg != null || function.CurrentACState == ACStateEnum.SMAborted ||
                                (function.LastACState == ACStateEnum.SMResetting && function.CurrentACState == ACStateEnum.SMIdle && _IsAborted))
                            {
                                _IsAborted = false;

                                Msg msgResult = SetRelationStatePicking(currentOpenMaterial, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck);
                                if (msgResult != null)
                                    ActivateProcessAlarmWithLog(msgResult);

                                state = WeighingComponentState.Aborted;
                                changeState = true;
                            }

                            if (currentOpenMaterial != null && changeState)
                            {
                                WeighingComponent weighingComp = GetWeighingComponentPicking(currentOpenMaterial); //WeighingComponents.FirstOrDefault(c => c.PLPosRelation == CurrentOpenMaterial);
                                if (weighingComp != null)
                                {
                                    weighingComp.SwitchState(state);
                                    SetInfo(weighingComp, WeighingComponentInfoType.State, currentFacilityCharge);
                                }
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    Messages.LogException(this.GetACUrl(), "TaskCallback(10)", exc);
                }
                finally
                {
                    SetCanStartFromBSO(true);
                    CurrentOpenMaterial = null;
                    CurrentFacilityCharge = null;
                    SubscribeToProjectWorkCycle();
                }
            }
        }

        protected Msg SetRelationStatePicking(Guid? pickingPosID, MDDelivPosLoadState.DelivPosLoadStates targetState)
        {
            Msg msg = null;
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                if (pickingPosID != null)
                {
                    PickingPos pickingPos = dbApp.PickingPos.FirstOrDefault(c => c.PickingPosID == pickingPosID);
                    if (pickingPos != null)
                    {
                        MDDelivPosLoadState posState = dbApp.MDDelivPosLoadState.FirstOrDefault(c => c.MDDelivPosLoadStateIndex == (short)targetState);
                        if (posState != null)
                        {
                            pickingPos.MDDelivPosLoadState = posState;
                            msg = dbApp.ACSaveChanges();
                        }
                    }
                    else
                    {
                        //Error50374: ProdOrderPartslistPosRelation {0} doesn't exist in the database.
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "SetRelationState(10)", 1915, "Error50374", pickingPosID);
                    }
                }
                else
                {
                    // Error50373: Manual weighing error, the property {0} is null!
                    // Fehler bei Handverwiegung, die Eigenschaft {0} ist null!
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "SetRelationState(20)", 1921, "Error50373", "CurrentOpenMaterial");
                }
            }
            return msg;
        }

        public Msg DoManualWeighingBookingPicking(double? actualWeight, bool thisWeighingIsInTol, bool isConsumedLot, Guid? currentOpenMaterial, Guid? currentFacilityCharge,
                                           bool isForInterdischarge = false, bool scaleOtherCompAfterAbort = false, double? tQuantityFromPAF = null)
        {
            MsgWithDetails collectedMessages = new MsgWithDetails();
            Msg msg = null;

            using (var dbIPlus = new Database())
            using (var dbApp = new DatabaseApp(dbIPlus))
            {
                try
                {
                    MDDelivPosLoadState posState;
                    // Falls in Toleranz oder Dosierung abgebrochen ohne Grund, dann beende Position
                    if (thisWeighingIsInTol)
                        posState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();
                    else
                        posState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadingActive).FirstOrDefault();

                    if (posState == null)
                    {
                        // Error50265: MDProdOrderPartslistPosState for Completed-State doesn't exist
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(1)", 1702, "Error50265");
                        ActivateProcessAlarmWithLog(msg, false);
                        return msg;
                    }

                    if (currentOpenMaterial == null)
                    {
                        // Error50373: Manual weighing error, the property {0} is null!
                        // Fehler bei Handverwiegung, die Eigenschaft {0} ist null!
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(10)", 1953, "Error50373", "CurrentOpenMaterial");
                        ActivateProcessAlarmWithLog(msg, false);
                        return msg;
                    }

                    PickingPos pickingPos = dbApp.PickingPos.FirstOrDefault(c => c.PickingPosID == currentOpenMaterial);

                    if (pickingPos != null)
                    {
                        bool changePosState = false;

                        if (currentFacilityCharge == null)
                        {
                            // Error50373: Manual weighing error, the property {0} is null!
                            // Fehler bei Handverwiegung, die Eigenschaft {0} ist null!
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(20)", 1967, "Error50373", "currentFacilityCharge");
                            ActivateProcessAlarmWithLog(msg, false);
                            return msg;
                        }

                        FacilityCharge facilityCharge = null;
                        Facility facility = null;

                        if (currentFacilityCharge != null)
                        {
                            facilityCharge = dbApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == currentFacilityCharge) as FacilityCharge;
                            if (facilityCharge == null)
                            {
                                // Error50367: The quant {0} doesn't exist in the database!
                                msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(30)", 1981, "Error50367", currentFacilityCharge);
                                ActivateProcessAlarmWithLog(msg, false);
                                return msg;
                            }
                            facility = facilityCharge.Facility;
                        }
                        //else
                        //{
                        //    facility = dbApp.Facility.FirstOrDefault(c => c.FacilityID == CurrentFacility);
                        if (facility == null)
                        {
                            //Error50378: Can't get the Facility from database with FacilityID: {0}
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(31)", 1686, "Error50378", "from quant!");
                            ActivateProcessAlarmWithLog(msg, false);
                            return msg;
                        }
                        //}

                        if (facilityCharge != null && facilityCharge.MaterialID != pickingPos.Material.MaterialID)
                        {
                            msg = new Msg(this, eMsgLevel.Error, nameof(PWManualWeighing), nameof(DoManualWeighingBookingPicking) + "(32)", 2809,
                                           "The material of quant is different than weighing material. The quant ID: " + facilityCharge.FacilityChargeID + " The material ID: " +
                                           pickingPos.Material.MaterialID);

                            ActivateProcessAlarmWithLog(msg, false);

                            return msg;
                        }

                        double actualQuantity = actualWeight.HasValue ? pickingPos.Material.ConvertBaseWeightToBaseUnit(actualWeight.Value) : 0;
                        double targetQuantity = pickingPos.TargetQuantityUOM;
                        WeighingComponent comp = GetWeighingComponentPicking(pickingPos.PickingPosID);
                        if (comp != null)
                            targetQuantity = pickingPos.Material.ConvertBaseWeightToBaseUnit(comp.TargetWeight);

                        if (!isForInterdischarge && (!tQuantityFromPAF.HasValue || _IsLotChanged))
                        {
                            double calcActualQuantity = targetQuantity + pickingPos.RemainingDosingQuantityUOM;
                            if (tQuantityFromPAF.HasValue && calcActualQuantity > 0.00001)
                            {
                                calcActualQuantity = tQuantityFromPAF.Value + pickingPos.RemainingDosingQuantityUOM;
                            }
                            if (actualQuantity > calcActualQuantity)
                                actualQuantity = actualQuantity - calcActualQuantity;
                        }

                        if (actualQuantity > 0.000001)
                        {
                            FacilityPreBooking facilityPreBooking = ACFacilityManager.NewFacilityPreBooking(dbApp, pickingPos, actualQuantity);
                            ACMethodBooking bookingParam = facilityPreBooking.ACMethodBooking as ACMethodBooking;
                            bookingParam.OutwardQuantity = (double)actualQuantity;
                            bookingParam.OutwardFacility = facility;
                            bookingParam.OutwardFacilityCharge = facilityCharge;
                            bookingParam.SetCompleted = isConsumedLot;
                            if (ParentPWGroup != null && ParentPWGroup.AccessedProcessModule != null)
                                bookingParam.PropertyACUrl = ParentPWGroup.AccessedProcessModule.GetACUrl();
                            msg = dbApp.ACSaveChangesWithRetry();

                            if (msg != null)
                            {
                                collectedMessages.AddDetailMessage(msg);
                                ActivateProcessAlarmWithLog(new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(40)", 2020), false);
                                changePosState = false;
                            }
                            else if (facilityPreBooking != null)
                            {
                                bookingParam.IgnoreIsEnabled = true;
                                ACMethodEventArgs resultBooking = ACFacilityManager.BookFacilityWithRetry(ref bookingParam, dbApp);
                                if (resultBooking.ResultState == Global.ACMethodResultState.Failed || resultBooking.ResultState == Global.ACMethodResultState.Notpossible)
                                {
                                    msg = new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(60)", 2045);
                                    ActivateProcessAlarm(msg, false);
                                    changePosState = false;
                                }
                                else
                                {
                                    if (!bookingParam.ValidMessage.IsSucceded() || bookingParam.ValidMessage.HasWarnings())
                                    {
                                        collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                                        msg = new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(70)", 2053);
                                        ActivateProcessAlarmWithLog(msg, false);
                                        changePosState = false;
                                    }
                                    changePosState = true;
                                    if (bookingParam.ValidMessage.IsSucceded())
                                    {
                                        facilityPreBooking.DeleteACObject(dbApp, true);
                                        pickingPos.IncreasePickingActualUOM(bookingParam.OutwardQuantity.Value);
                                        msg = dbApp.ACSaveChangesWithRetry();
                                        if (msg != null)
                                        {
                                            collectedMessages.AddDetailMessage(msg);
                                            msg = new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(80)", 2065);
                                            ActivateProcessAlarmWithLog(msg, false);
                                        }

                                        //Messages.LogInfo(this.GetACUrl(), "", "ManualWeighingTrace - changePosState value: " + changePosState.ToString());

                                        if (changePosState)
                                        {
                                            //Messages.LogInfo(this.GetACUrl(), "", "ManualWeighingTrace - posState value: " + posState.ToString());

                                            pickingPos.MDDelivPosLoadState = posState;

                                            if (!AutoInterDis)
                                            {
                                                if (posState != null && posState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck)
                                                {
                                                    var unconfirmedBookings = pickingPos.FacilityBooking_PickingPos
                                                                                        .Where(c => c.MaterialProcessStateIndex == (short)GlobalApp.MaterialProcessState.New);

                                                    //Messages.LogInfo(this.GetACUrl(), "", "ManualWeighingTrace - Bookings count with state new:" + unconfirmedBookings?.Count().ToString());

                                                    foreach (var booking in unconfirmedBookings)
                                                    {
                                                        booking.MaterialProcessState = GlobalApp.MaterialProcessState.Processed;
                                                        //Messages.LogInfo(this.GetACUrl(), "", "ManualWeighingTrace - Booking change state");
                                                    }
                                                }
                                            }
                                        }

                                        msg = dbApp.ACSaveChangesWithRetry();
                                        if (msg != null)
                                        {
                                            collectedMessages.AddDetailMessage(msg);
                                            msg = new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(90)", 2094);
                                            ActivateProcessAlarmWithLog(msg, false);
                                        }
                                        else
                                        {
                                            pickingPos.RecalcActualQuantity();
                                            if (dbApp.IsChanged)
                                                dbApp.ACSaveChanges();
                                        }
                                    }
                                    else
                                        collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                                }
                            }
                        }
                        else
                        {
                            // Error50269 The actual quantity for posting is too small. Order {0}, Bill of material {1}, line {2}
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoManualWeihgingBooking(100)", 2112, "Error50269", pickingPos?.Picking.PickingNo,
                                          pickingPos?.Picking.PickingNo, pickingPos.Material.MaterialNo);
                            ActivateProcessAlarmWithLog(msg, false);
                        }
                    }
                }
                catch (Exception e)
                {
                    msg = new Msg(e.Message, this, eMsgLevel.Error, PWClassName, "DoManualWeighingBooking(120)", 2120);
                    ActivateProcessAlarmWithLog(msg, false);
                }
            }

            return collectedMessages.MsgDetailsCount > 0 ? collectedMessages : null;
        }

    }
}
