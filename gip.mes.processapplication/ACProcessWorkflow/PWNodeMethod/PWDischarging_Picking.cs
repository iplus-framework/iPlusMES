// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System.Threading;
using System.Net;
using gip.core.processapplication;
using static gip.core.communication.ISOonTCP.PLC;

namespace gip.mes.processapplication
{
    public partial class PWDischarging
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
        #endregion

        #region Methods

        public static Guid GetPickingIDFromACMethod(ACMethod acMethod)
        {
            Guid pickingID = Guid.Empty;
            ACValue acValue = acMethod.ParameterValueList.GetACValue(Picking.ClassName);
            if (acValue != null && acValue.Value != null && acValue.Value is Guid)
                pickingID = (Guid)acValue.Value;
            return pickingID;
        }

        public static Guid GetPickingPosIDFromACMethod(ACMethod acMethod)
        {
            Guid pickingPosID = Guid.Empty;
            ACValue acValue = acMethod.ParameterValueList.GetACValue(PickingPos.ClassName);
            if (acValue != null && acValue.Value != null && acValue.Value is Guid)
                pickingPosID = (Guid)acValue.Value;
            return pickingPosID;
        }

        public virtual FacilityReservation GetNextFreeDestination(IList<FacilityReservation> plannedSilos, PickingPos dnPos, double targetQuantity, bool changeReservationStateIfFull = false, FacilityReservation ignoreFullSilo = null, PAFDischarging discharging = null)
        {
            if (plannedSilos == null || !plannedSilos.Any())
                return null;

            // 1. Prüfe zuerst ob das geplante Silo, das zur Zeit als aktive Befüllung gesetzt worden ist, befüllt werden kann
            Facility destinationSilo = null;
            foreach (FacilityReservation plannedSilo in plannedSilos.Where(c => c.ReservationState == GlobalApp.ReservationState.Active))
            {
                if (CheckPlannedDestinationSilo(plannedSilo, dnPos, targetQuantity, changeReservationStateIfFull, ignoreFullSilo))
                    return plannedSilo;
            }

            // 2. Die aktiven Silos können nicht mehr befüllt werden => suche ein anderes das neu geplant ist
            if (destinationSilo == null)
            {
                foreach (FacilityReservation plannedSilo in plannedSilos.Where(c => c.ReservationState == GlobalApp.ReservationState.New))
                {
                    if (CheckPlannedDestinationSilo(plannedSilo, dnPos, targetQuantity, changeReservationStateIfFull, ignoreFullSilo))
                    {
                        plannedSilo.ReservationState = GlobalApp.ReservationState.Active;
                        return plannedSilo;
                    }
                }
            }

            return null;
        }

        public static FacilityReservation GetNextFreeDestination(ACComponent invoker, IList<FacilityReservation> plannedSilos, PickingPos pPos, double targetQuantity, bool changeReservationStateIfFull = false, FacilityReservation ignoreFullSilo = null)
        {
            if (plannedSilos == null || !plannedSilos.Any())
                return null;
            foreach (FacilityReservation plannedSilo in plannedSilos.Where(c => c.ReservationState == GlobalApp.ReservationState.Active))
            {
                if (CheckPlannedDestinationSilo(invoker, plannedSilo, pPos, targetQuantity, changeReservationStateIfFull, ignoreFullSilo))
                    return plannedSilo;
            }
            foreach (FacilityReservation plannedSilo in plannedSilos.Where(c => c.ReservationState == GlobalApp.ReservationState.New))
            {
                if (CheckPlannedDestinationSilo(invoker, plannedSilo, pPos, targetQuantity, changeReservationStateIfFull, ignoreFullSilo))
                {
                    //plannedSilo.ReservationState = GlobalApp.ReservationState.Active;
                    return plannedSilo;
                }
            }
            //foreach (FacilityReservation plannedSilo in plannedSilos.Where(c => c.ReservationState == GlobalApp.ReservationState.Finished))
            //{
            //    if (CheckPlannedDestinationSilo(invoker, plannedSilo, pPos, targetQuantity, changeReservationStateIfFull, ignoreFullSilo))
            //    {
            //        //plannedSilo.ReservationState = GlobalApp.ReservationState.New;
            //        return plannedSilo;
            //    }
            //}
            return null;
        }


        protected virtual bool CheckPlannedDestinationSilo(FacilityReservation plannedSilo, PickingPos dnPos, double targetQuantity, bool changeReservationStateIfFull = false, FacilityReservation ignoreFullSilo = null)
        {
            if (plannedSilo == null || (ignoreFullSilo != null && ignoreFullSilo == plannedSilo))
                return false;

            if (plannedSilo != null
                && plannedSilo.Facility != null
                && plannedSilo.Facility.InwardEnabled
                && (   plannedSilo.Facility.Material == null
                    || dnPos.Material.IsMaterialEqual(plannedSilo.Facility.Material)
                    // ((dnPos.Material.ProductionMaterialID.HasValue && plannedSilo.Facility.MaterialID == dnPos.Material.ProductionMaterialID)
                    //    || (!dnPos.Material.ProductionMaterialID.HasValue && plannedSilo.Facility.MaterialID == dnPos.Material.MaterialID))
                    )
               )
            {
                // Prüfe ob rechnerisch die Charge reinpassen würde
                //if (plannedSilo.Facility.CurrentFacilityStock != null
                //    && (plannedSilo.Facility.MaxWeightCapacity > 1)
                //    && (targetQuantity + plannedSilo.Facility.CurrentFacilityStock.StockQuantity > plannedSilo.Facility.MaxWeightCapacity))
                //{
                //    Messages.LogDebug(this.GetACUrl(), "AldiRoasterCheckScale.CheckPlannedDestinationSilo(1)", String.Format("Silo {0} würde rechnerisch überfüllt werden", plannedSilo.Facility.FacilityNo));
                //    plannedSilo.ReservationState = GlobalApp.ReservationState.Finished;
                //}
                //else
                //destinationSilo = plannedSilo.Facility;
                return true;
            }
            return false;
        }

        public static bool CheckPlannedDestinationSilo(ACComponent invoker, FacilityReservation plannedSilo, PickingPos dnPos, double targetQuantity, bool changeReservationStateIfFull = false, FacilityReservation ignoreFullSilo = null)
        {
            if (plannedSilo == null || (ignoreFullSilo != null && ignoreFullSilo == plannedSilo))
                return false;
            //Facility destinationSilo = null;
            if (plannedSilo != null
                && plannedSilo.Facility != null
                && plannedSilo.Facility.InwardEnabled
                && (plannedSilo.Facility.Material == null
                    || dnPos.Material.IsMaterialEqual(plannedSilo.Facility.Material)
                    //|| ((dnPos.Material.ProductionMaterialID.HasValue && plannedSilo.Facility.MaterialID == dnPos.Material.ProductionMaterialID)
                    //    || (!dnPos.Material.ProductionMaterialID.HasValue && plannedSilo.Facility.MaterialID == dnPos.Material.MaterialID))
                    ))
            {
                // Prüfe ob rechnerisch die Charge reinpassen würde
                //if (plannedSilo.Facility.CurrentFacilityStock != null
                //    && (plannedSilo.Facility.MaxWeightCapacity > 1)
                //    && (targetQuantity + plannedSilo.Facility.CurrentFacilityStock.StockQuantity > plannedSilo.Facility.MaxWeightCapacity))
                //{
                //    Messages.LogDebug(this.GetACUrl(), "AldiRoasterCheckScale.CheckPlannedDestinationSilo(1)", String.Format("Silo {0} würde rechnerisch überfüllt werden", plannedSilo.Facility.FacilityNo));
                //    plannedSilo.ReservationState = GlobalApp.ReservationState.Finished;
                //}
                //else
                //destinationSilo = plannedSilo.Facility;
                return true;
            }
            return false;
            //return destinationSilo;
        }


        protected virtual StartDisResult StartDischargingPicking(PAProcessModule module, ACMethod acMethodRoot, Database db, DatabaseApp dbApp, Picking picking, PickingPos pickingPos)
        {
            Msg msg = null;
            string message = "";
            PWMethodTransportBase pwMethodTransport = ParentPWMethod<PWMethodTransportBase>();
            if (picking == null || pwMethodTransport == null)
            {
                //Error50152: Picking is null
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingPicking(1)", 1000, "Error50152");
                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), "StartDischargingPicking(1)", msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CancelDischarging;
            }

            PAProcessModule dischargeToModule = null;
            var cacheModuleDestinations = CacheModuleDestinations;
            // If destination is a processmodule and the possible destinations are cached, then check
            if (cacheModuleDestinations != null && cacheModuleDestinations.Any())
            {
                ACComponent cachedDest = null;
                var subResult = CheckCachedModuleDestinations(ref cachedDest, ref msg);
                if (subResult == StartDisResult.CycleWait)
                    return StartDisResult.FastCycleWait;
                dischargeToModule = cachedDest as PAProcessModule;
            }
            //// Else destination is a silo
            //else if (_LastTargets != null && _NoTargetLongWait.HasValue && DateTime.Now < _NoTargetLongWait.Value)
            //{
            //    short destError;
            //    string errorMsg;
            //    Guid[] targets = pwMethodTransport.GetCachedDestinations(false/*addedModules.Any()*/, out destError, out errorMsg);
            //    if (!PWMethodTransportBase.AreCachedDestinationsDifferent(_LastTargets, targets))
            //        return StartDisResult.FastCycleWait;
            //}
            NoTargetLongWait = null;


            if (pickingPos == null)
            {
                if (!picking.PickingPos_Picking.Where(c => c.InOrderPosID.HasValue || c.OutOrderPosID.HasValue || c.PickingPosProdOrderPartslistPos_PickingPos.Any()).Any())
                {
                    pickingPos = picking.PickingPos_Picking
                        .Where(c => !c.MDDelivPosLoadStateID.HasValue || c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad)
                        .OrderBy(c => c.Sequence)
                        .FirstOrDefault();
                    if (pickingPos == null)
                    {
                        pickingPos = picking.PickingPos_Picking
                        .Where(c => c.MDDelivPosLoadStateID.HasValue && c.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive)
                        .OrderBy(c => c.Sequence)
                        .FirstOrDefault();
                    }
                    if (pickingPos == null)
                        pickingPos = picking.PickingPos_Picking.FirstOrDefault();
                }
                else
                {
                    message = "TODO: Orderhandling with Picking is not implemented";
                    if (IsAlarmActive(ProcessAlarm, message) == null)
                        Messages.LogError(this.GetACUrl(), "StartDischargingPicking(2)", message);
                    OnNewAlarmOccurred(ProcessAlarm, message, true);
                    return StartDisResult.CancelDischarging;
                }
            }

            if (pickingPos == null)
            {
                // Error50088: No dischargable line at commissioning order {0} found.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingPicking(10)", 1010, "Error50088", picking.PickingNo);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);

                return StartDisResult.CycleWait;
            }

            Facility destinationSilo = null;
            FacilityReservation facReservation = null;
            IList<FacilityReservation> plannedSilos = ParentPWMethod<PWMethodTransportBase>().ACFacilityManager.GetSelectedTargets(pickingPos);
            if (plannedSilos != null && plannedSilos.Any())
            {
                facReservation = GetNextFreeDestination(plannedSilos, pickingPos, pickingPos.TargetQuantityUOM);
                if (facReservation != null)
                    destinationSilo = facReservation.Facility;
            }
            if (destinationSilo == null)
            {
                destinationSilo = pickingPos.ToFacility;
                if (destinationSilo != null)
                {
                    foreach (PickingPos altPickingPos in picking.PickingPos_Picking)
                    {
                        if (altPickingPos != pickingPos && altPickingPos.Material == pickingPos.Material && altPickingPos.ToFacility == destinationSilo)
                        {
                            plannedSilos = ParentPWMethod<PWMethodTransportBase>().ACFacilityManager.GetSelectedTargets(altPickingPos);
                            if (plannedSilos != null && plannedSilos.Any())
                            {
                                facReservation = GetNextFreeDestination(plannedSilos, altPickingPos, altPickingPos.TargetQuantityUOM);
                                if (facReservation != null && facReservation.Facility != null)
                                    destinationSilo = facReservation.Facility;
                            }
                        }
                    }
                }
            }

            if (destinationSilo == null)
            {
                // Error50085: No destination defined in commissioning line at commssion {0}!
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingPicking(20)", 1020, "Error50085", picking.PickingNo);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            if (!destinationSilo.VBiFacilityACClassID.HasValue)
            {
                // Error50086: Foreign Key to ACComponent for Facility {0} not defined!
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingPicking(30)", 1030, "Error50086", destinationSilo.FacilityNo);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            if (    destinationSilo.Material != null 
                && !(    pickingPos.Material == destinationSilo.Material
                      || pickingPos.Material.ProductionMaterialID.HasValue && pickingPos.Material.ProductionMaterialID == destinationSilo.Material.MaterialID))
            {
                // Error50087: Material {0} on Silo {1} doesn't match Material {2} at Commssioningorder {3}!
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingPicking(40)", 1040, "Error50087",
                              destinationSilo.Material.MaterialName1, destinationSilo.FacilityNo, destinationSilo.Material.MaterialName1, picking.PickingNo);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            gip.core.datamodel.ACClass acClassSilo = destinationSilo.GetFacilityACClass(db);
            if (acClassSilo == null)
            {
                // Error50070: acClassSilo is null at Order {0}, Bill of material {1}, Line {2}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingPicking(50)", 1050, "Error50070", 
                                picking.PickingNo, pickingPos.LineNumber, pickingPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            ACComponent targetSiloACComp = this.Root.ACUrlCommand(acClassSilo.GetACUrlComponent()) as ACComponent;
            if (targetSiloACComp == null)
            {
                // Error50071: targetSiloACComp is null at Order {0}, Bill of material {1}, Line {2}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingPicking(60)", 1060, "Error50071",
                                picking.PickingNo, pickingPos.LineNumber, pickingPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            Type typeOfSilo = typeof(PAMSilo);
            Guid thisMethodID = ContentACClassWF.ACClassMethodID;
            Route predefinedRoute = facReservation?.PredefinedRoute;
                if (predefinedRoute != null)
                    predefinedRoute = predefinedRoute.Clone() as Route;
            ACMethod exParallelMethod = null;
            if (predefinedRoute == null && KeepSameRoute)
                exParallelMethod = FindParallelDischargingIfRoute();

            PAProcessModule targetModule = null;
            bool isLastDischarging = true;
            // Falls Workflow mehrere Gruppe hat, prüfe zuerst ob vom akteullen Prozessmodul direkt in das Ziel enteleert werden kann oder über ein weiteres Prozessmodul geschleust werden muss
            DetermineDischargingRoute(Root.Database as Database, module, targetSiloACComp, 0,
                                    (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule,
                                    PAProcessModule.SelRuleID_ProcessModule_Deselector, null, predefinedRoute, exParallelMethod);
            // Falls kein direkter Weg gefunden, prüfe über welche gemappte PWGroup weiter transportiert werden kann 
            if (CurrentDischargingDest(db, false) == null)
            {
                isLastDischarging = false;

                ACRoutingParameters routingParameters = new ACRoutingParameters()
                {
                    RoutingService = this.RoutingService,
                    Database = db,
                    AttachRouteItemsToContext = RoutingService != null && RoutingService.IsProxy,
                    SelectionRuleID = PAProcessModule.SelRuleID_ProcessModule,
                    Direction = RouteDirections.Forwards,
                    DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule,
                    MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                    IncludeReserved = true,
                    IncludeAllocated = true
                };

                RoutingResult rResult = ACRoutingService.FindSuccessors(ParentPWGroup.AccessedProcessModule.GetACUrl(), routingParameters);
                if (rResult.Routes != null && rResult.Routes.Any())
                {
                    // Falls keine Sonderentleerung sondern regulärer weitertransport zu einem Prozessmodul
                    cacheModuleDestinations = rResult.Routes.Select(c => c.LastOrDefault().Target.ACClassID).ToArray();
                    CacheModuleDestinations = cacheModuleDestinations;
                    ACComponent cachedDest = null;
                    var subResult = CheckCachedModuleDestinations(ref cachedDest, ref msg);
                    if (subResult == StartDisResult.CycleWait)
                        return cacheModuleDestinations != null ? StartDisResult.FastCycleWait : StartDisResult.CycleWait;
                    targetModule = cachedDest as PAProcessModule;
                    Guid acClassIdCachedDest = targetModule.ComponentClass.ACClassID;
                    var route = rResult.Routes.Where(c => c.LastOrDefault().Target.ACClassID == acClassIdCachedDest).FirstOrDefault();
                    if (route != null)
                        route.Detach();
                    CurrentDischargingRoute = route;
                }
            }

            if (CurrentDischargingDest(db, false) == null)
            {
                // Error50072: CurrentDischargingDest() is null because no route couldn't be found at Order {0}, Bill of material {1}, Line {2}.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingPicking(70)", 1070, "Error50072",
                                picking.PickingNo, pickingPos.LineNumber, pickingPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            if (targetModule == null)
                targetModule = TargetPAModule(Root.Database as Database) as PAProcessModule;
            if (targetModule == null)
            {
                // Error50073: targetSilo is null at Order {0}, Bill of material {1}, Line {2}.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingPicking(80)", 1080, "Error50073",
                                picking.PickingNo, pickingPos.LineNumber, pickingPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            if (isLastDischarging 
                && destinationSilo.Material == null 
                && destinationSilo.MDFacilityType != null
                && destinationSilo.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
            {
                destinationSilo.Material = pickingPos.Material.Material1_ProductionMaterial != null ? pickingPos.Material.Material1_ProductionMaterial : pickingPos.Material;
                dbApp.ACSaveChanges();
            }

            core.datamodel.ACClassMethod refPAACClassMethod = RefACClassMethodOfContentWF;
            if (refPAACClassMethod == null)
                return StartDisResult.CancelDischarging;

            ACMethod acMethod = refPAACClassMethod.TypeACSignature();
            if (acMethod == null)
            {
                //Error50153: acMethod is null.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingPicking(81)", 1090, "Error50153");
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            PAProcessFunction responsibleFunc = CanStartProcessFunc(module, acMethod, dbApp, pickingPos, targetModule);
            if (responsibleFunc == null)
                return StartDisResult.CycleWait;

            if (!(bool)ExecuteMethod(nameof(GetConfigForACMethod), acMethod, true, dbApp, pickingPos, targetModule))
                return StartDisResult.CycleWait;

            if (!ValidateAndSetRouteForParam(acMethod))
                return StartDisResult.CycleWait;

            ACValue acValue = acMethod.ParameterValueList.GetACValue("Destination");
            if (acValue != null)
            {
                if (acValue.ObjectType != null)
                    acValue.Value = Convert.ChangeType(targetModule.RouteItemIDAsNum, acValue.ObjectType);
                else
                    acMethod["Destination"] = targetModule.RouteItemIDAsNum;
            }
            if (CurrentDischargingRoute != null)
                CurrentDischargingRoute.Detach(true);

            if ((ParentPWMethod<PWMethodTransportBase>() != null
                    && (   ((ACSubStateEnum)ParentPWMethod<PWMethodTransportBase>().CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrder)
                        || ((ACSubStateEnum)ParentPWMethod<PWMethodTransportBase>().CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode))
                 )
                || (ParentPWGroup != null
                    && (   ((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrder)
                        || ((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode))
                   )
                )
            {
                acValue = acMethod.ParameterValueList.GetACValue(PWMethodVBBase.IsLastBatchParamName);
                if (acValue != null)
                    acValue.Value = (Int16)1;
            }
            else if (ParentPWMethod<PWMethodTransportBase>() != null)
            {
                acValue = acMethod.ParameterValueList.GetACValue(PWMethodVBBase.IsLastBatchParamName);
                if (acValue != null && acValue.ParamAsInt16 <= (Int16)0)
                    acValue.Value = (Int16)ParentPWMethod<PWMethodTransportBase>().IsLastBatch;
            }

            acValue = acMethod.ParameterValueList.GetACValue("InterDischarging");
            if (acValue != null
                && ParentPWGroup != null
                && (((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMInterDischarging)
                    || ((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMDisThenNextComp)))
            {
                acValue.Value = (Int16)1;
            }

            acValue = acMethod.ParameterValueList.GetACValue("Source");
            if (acValue != null
                && acValue.ParamAsInt16 == (Int16)0
                && pickingPos.FromFacility != null
                && pickingPos.FromFacility.VBiFacilityACClassID.HasValue)
            {
                var acClass = pickingPos.FromFacility.FacilityACClass;
                if (acClass != null)
                {
                    IRouteItemIDProvider routeItemIDProvider = ACUrlCommand(acClass.ACUrlComponent) as IRouteItemIDProvider;
                    if (routeItemIDProvider != null)
                    {
                        Int32 itemId = routeItemIDProvider.RouteItemIDAsNum;
                        if (itemId < Int16.MaxValue && itemId > Int16.MinValue)
                            acValue.Value = (Int16)routeItemIDProvider.RouteItemIDAsNum;
                    }
                }
            }

            ACValue acValueTargetQ = acMethod.ParameterValueList.GetACValue("TargetQuantity");
            if (acValueTargetQ != null && acValueTargetQ.ParamAsDouble < 0.000001)
                acValueTargetQ.Value = pickingPos.RemainingDosingQuantityUOM > 0 ? 0.001 : pickingPos.RemainingDosingQuantityUOM * -1;

            gip.core.processapplication.PAEScaleTotalizing totalizingScale = TotalizingScaleIfSWT;
            if (IsAutomaticContinousWeighing && totalizingScale != null)
            {
                acValue = acMethod.ParameterValueList.GetACValue("ScaleBatchWeight");
                if (acValue != null)
                    acValue.Value = totalizingScale.SWTTipWeight;
            }

            NoTargetWait = null;
            if (!(bool)ExecuteMethod(nameof(AfterConfigForACMethodIsSet), acMethod, true, dbApp, pickingPos, targetModule))
                return StartDisResult.CycleWait;

            if (!acMethod.IsValid())
            {
                // Error50074: Dischargingtask not startable Order {0}, Bill of material {1}, line {2}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingPicking(90)", 1200, "Error50074",
                                picking.PickingNo, pickingPos.LineNumber, pickingPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            RecalcTimeInfo();
            CurrentDisEntityID.ValueT = pickingPos.PickingPosID;
            if (CreateNewProgramLog(acMethod) <= CreateNewProgramLogResult.ErrorNoProgramFound)
                return StartDisResult.CycleWait;
            _ExecutingACMethod = acMethod;

            module.TaskInvocationPoint.ClearMyInvocations(this);
            _CurrentMethodEventArgs = null;
            IACPointEntry task = module.TaskInvocationPoint.AddTask(acMethod, this);
            if (!IsTaskStarted(task))
            {
                CurrentDisEntityID.ValueT = Guid.Empty;
                ACMethodEventArgs eM = _CurrentMethodEventArgs;
                if (eM == null || eM.ResultState != Global.ACMethodResultState.FailedAndRepeat)
                {
                    // Error50074: Dischargingtask not startable Order {0}, Bill of material {1}, line {2}
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingPicking(100)", 1210, "Error50074",
                                picking.PickingNo, pickingPos.LineNumber, pickingPos.Material != null ? pickingPos.Material.MaterialName1 : "");

                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    return StartDisResult.CycleWait;
                }
            }
            UpdateCurrentACMethod();
            RememberWeightOnRunDischarging(true);

            if (pickingPos.MDDelivPosLoadState == null || pickingPos.MDDelivPosLoadState.DelivPosLoadState == MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad)
                pickingPos.MDDelivPosLoadState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadingActive).FirstOrDefault();
            CheckIfAutomaticTargetChangePossible = null;
            MsgWithDetails msg2 = dbApp.ACSaveChanges();
            if (msg2 != null)
            {
                Messages.LogException(this.GetACUrl(), "StartDischargingPicking(110)", msg2.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, new Msg(msg2.InnerMessage, this, eMsgLevel.Exception, PWClassName, "StartDischargingPicking", 1220), true);
                return StartDisResult.CycleWait;
            }
            AcknowledgeAlarms();
            ExecuteMethod(nameof(OnACMethodSended), acMethod, true, dbApp, pickingPos, targetModule, responsibleFunc);
            return task.State == PointProcessingState.Deleted ? StartDisResult.CancelDischarging : StartDisResult.WaitForCallback;
            //return StartDisResult.WaitForCallback;
        }


        protected virtual StartDisResult OnHandleStateCheckFullSiloPicking(PAFDischarging discharging, PAProcessModule targetContainer, PAProcessModule module, ACMethod acMethodRoot, Database db, DatabaseApp dbApp, Picking picking, PickingPos pickingPosPrev)
        {
            Msg msg = null;
            PickingPos pickingPos = pickingPosPrev;
            PAMSilo targetSilo = targetContainer as PAMSilo;
            if (targetSilo == null)
            {
                // Error50107 targetModule is null at DeliveryNote {0}, Line {1}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloPicking(10)", 1120, "Error50107",
                                pickingPos != null ? pickingPos.Picking.PickingNo : "", pickingPos != null ? pickingPos.Material.MaterialName1 : "");

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CancelDischarging;
            }

            Route previousDischargingRoute = CurrentDischargingRoute;

            PWMethodTransportBase pwMethodTransport = ParentPWMethod<PWMethodTransportBase>();
            if (picking == null || pwMethodTransport == null)
            {
                //Error50152: Picking is null.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloPicking(1)", 1230, "Error50152");

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), "OnHandleStateCheckFullSiloPicking(1)", msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CancelDischarging;
            }

            if (pickingPos == null)
            {
                if (!picking.PickingPos_Picking.Where(c => c.InOrderPosID.HasValue || c.OutOrderPosID.HasValue || c.PickingPosProdOrderPartslistPos_PickingPos.Any()).Any())
                {
                    var posList = picking.PickingPos_Picking.ToArray();
                    pickingPos = posList
                        .Where(c => c.MDDelivPosLoadStateID.HasValue && c.MDDelivPosLoadState.DelivPosLoadState == MDDelivPosLoadState.DelivPosLoadStates.LoadingActive)
                        .OrderBy(c => c.Sequence)
                        .FirstOrDefault();
                }
                else
                {
                    string message = "TODO: Orderhandling with Picking is not implemented";
                    if (IsAlarmActive(ProcessAlarm, message) == null)
                        Messages.LogError(this.GetACUrl(), "OnHandleStateCheckFullSiloPicking(2)", message);
                    OnNewAlarmOccurred(ProcessAlarm, message, true);
                    return StartDisResult.CancelDischarging;
                }
            }

            if (pickingPos == null)
            {
                string message = "Current line for Picking not found";
                if (IsAlarmActive(ProcessAlarm, message) == null)
                    Messages.LogError(this.GetACUrl(), "OnHandleStateCheckFullSiloPicking(3)", message);
                OnNewAlarmOccurred(ProcessAlarm, message, true);
                return StartDisResult.CancelDischarging;
            }

            Facility destinationSilo = null;
            IList<FacilityReservation> plannedSilos = ParentPWMethod<PWMethodTransportBase>().ACFacilityManager.GetSelectedTargets(pickingPos);

            if (plannedSilos == null || !plannedSilos.Any())
            {
                foreach (PickingPos altPickingPos in picking.PickingPos_Picking)
                {
                    if (altPickingPos != pickingPos && altPickingPos.Material == pickingPos.Material && pickingPos.ToFacility == destinationSilo)
                    {
                        plannedSilos = ParentPWMethod<PWMethodTransportBase>().ACFacilityManager.GetSelectedTargets(altPickingPos);
                        if (plannedSilos != null && plannedSilos.Any())
                            break;
                    }
                }
            }

            if (plannedSilos == null || !plannedSilos.Any())
            {
                CheckIfAutomaticTargetChangePossible = false;
                //Error50102 No destination defined in Deliverynote {0}, Line {1}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloPicking(20)", 1130, "Error50102",
                                pickingPos.Picking.PickingNo, pickingPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            FacilityReservation fullSiloReservation = null;

            PAMSilo fullSilo = targetContainer as PAMSilo;
            // 1. Prüfe zuerst ob das geplante Silo, das zur Zeit als aktive Befüllung gesetzt worden ist, befüllt werden kann
            foreach (FacilityReservation plannedSilo in plannedSilos.Where(c => c.ReservationState == GlobalApp.ReservationState.Active))
            {
                if (plannedSilo.FacilityID == targetSilo.Facility.ValueT.ValueT.FacilityID)
                {
                    fullSiloReservation = plannedSilo;
                    break;
                }
                else
                {
                    plannedSilo.ReservationState = GlobalApp.ReservationState.Finished;
                }
            }

            if (fullSiloReservation == null)
            {
                CheckIfAutomaticTargetChangePossible = false;
                // Error50084: Current active destination in batch planning {3} is not in sync with current discharging destination {4} at Order {0}, Bill of material {1}, Line {2}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSilo(6)", 1260, "Error50084",
                                pickingPos.Picking.PickingNo,
                        pickingPos.Material.MaterialNo,
                        pickingPos.Material.MaterialName1,
                        fullSiloReservation != null ? fullSiloReservation.Facility.FacilityNo : "",
                        targetContainer.GetACUrl());

                if (OnCheckFullSiloNoSiloFound(discharging, targetContainer, msg))
                {
                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        Messages.LogError(this.GetACUrl(), "OnHandleStateCheckFullSilo(6)", msg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                }
                return StartDisResult.CycleWait;
            }
            //else
            //{
            //    fullSiloReservation.ReservationState = GlobalApp.ReservationState.Finished;
            //}

            FacilityReservation nextPlannedSiloReservation = GetNextFreeDestination(plannedSilos, pickingPos, 0, true, fullSiloReservation, discharging);
            if (nextPlannedSiloReservation != null)
                destinationSilo = nextPlannedSiloReservation.Facility;

            // TOD:
            //if (pickingPosPrev != null)
            //    pickingPosPrev.MDDelivPosLoadState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();


            if (destinationSilo == null)
            {
                CheckIfAutomaticTargetChangePossible = false;
                // Error50085: No destination defined in commissioning line at commssion {0}!
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloPicking(20)", 1020, "Error50085", picking.PickingNo);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            if (!destinationSilo.VBiFacilityACClassID.HasValue)
            {
                CheckIfAutomaticTargetChangePossible = false;
                // Error50086: Foreign Key to ACComponent for Facility {0} not defined!
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingPicking(30)", 1030, "Error50086", destinationSilo.FacilityNo);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            if (destinationSilo.Material != null
                && !(pickingPos.Material == destinationSilo.Material
                      || pickingPos.Material.ProductionMaterialID.HasValue && pickingPos.Material.ProductionMaterialID == destinationSilo.Material.MaterialID))
            {
                CheckIfAutomaticTargetChangePossible = false;
                // Error50087: Material {0} on Silo {1} doesn't match Material {2} at Commssioningorder {3}!
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingPicking(40)", 1040, "Error50087",
                              destinationSilo.Material.MaterialName1, destinationSilo.FacilityNo, destinationSilo.Material.MaterialName1, picking.PickingNo);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            gip.core.datamodel.ACClass acClassSilo = destinationSilo.GetFacilityACClass(db);
            if (acClassSilo == null)
            {
                CheckIfAutomaticTargetChangePossible = false;
                // Error50070: acClassSilo is null at Order {0}, Bill of material {1}, Line {2}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingPicking(50)", 1050, "Error50070",
                                picking.PickingNo, pickingPos.LineNumber, pickingPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            ACComponent targetSiloACComp = this.Root.ACUrlCommand(acClassSilo.GetACUrlComponent()) as ACComponent;
            if (targetSiloACComp == null)
            {
                CheckIfAutomaticTargetChangePossible = false;
                // Error50071: targetSiloACComp is null at Order {0}, Bill of material {1}, Line {2}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingPicking(60)", 1060, "Error50071",
                                picking.PickingNo, pickingPos.LineNumber, pickingPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            msg = OnPrepareSwitchToNextSilo(discharging, targetContainer, targetSiloACComp);
            if (msg != null)
            {
                if (OnCheckFullSiloNoSiloFound(discharging, targetContainer, msg))
                {
                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        Messages.LogError(this.GetACUrl(), "OnHandleStateCheckFullSilo.OnPrepareSwitchToNextSilo(1)", msg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                }
                CheckIfAutomaticTargetChangePossible = false;
                return StartDisResult.CycleWait;
            }

            Type typeOfSilo = typeof(PAMSilo);
            Guid thisMethodID = ContentACClassWF.ACClassMethodID;

            Route predefinedRoute = null;
            predefinedRoute = nextPlannedSiloReservation?.PredefinedRoute;
            if (predefinedRoute != null)
                predefinedRoute = predefinedRoute.Clone() as Route;

            PAProcessModule targetModule = null;
            bool isLastDischarging = true;

            ACMethod acMethod = null;
            ACPointAsyncRMISubscrWrap<ACComponent> taskEntry = null;
            using (ACMonitor.Lock(TaskSubscriptionPoint.LockLocalStorage_20033))
            {
                taskEntry = this.TaskSubscriptionPoint.ConnectionList.FirstOrDefault();
            }
            if (taskEntry != null)
            {
                var tmpACMethod = taskEntry.ACMethodDescriptor as ACMethod;
                if (tmpACMethod != null)
                    acMethod = tmpACMethod.Clone() as ACMethod;
            }

            CurrentDischargingRoute = null;
            // Falls Workflow mehrere Gruppe hat, prüfe zuerst ob vom akteullen Prozessmodul direkt in das Ziel enteleert werden kann oder über ein weiteres Prozessmodul geschleust werden muss
            FindDisRouteResult findResult = DetermineDischargingRoute(Root.Database as Database, module, targetSiloACComp, 0,
                                        (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule,
                                        PAProcessModule.SelRuleID_ProcessModule_Deselector, null, predefinedRoute, acMethod);
            // Falls kein direkter Weg gefunden, prüfe über welche gemappte PWGroup weiter transportiert werden kann 
            if ((findResult == FindDisRouteResult.NotFound || CurrentDischargingDest(db, false) == null)
                && RootPW.FindChildComponents<PWGroup>(c => c is PWGroup && c != this.ParentPWGroup && (!(c is PWGroupVB) || !(c as PWGroupVB).OccupationByScan)).Any())
            {
                isLastDischarging = false;

                ACRoutingParameters routingParameters = new ACRoutingParameters()
                {
                    RoutingService = this.RoutingService,
                    Database = db,
                    AttachRouteItemsToContext = RoutingService != null && RoutingService.IsProxy,
                    SelectionRuleID = PAProcessModule.SelRuleID_ProcessModule,
                    Direction = RouteDirections.Forwards,
                    DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule,
                    MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                    IncludeReserved = true,
                    IncludeAllocated = true
                };


                RoutingResult rResult = ACRoutingService.FindSuccessors(ParentPWGroup.AccessedProcessModule.GetACUrl(), routingParameters);
                if (rResult.Routes != null && rResult.Routes.Any())
                {
                    // Falls keine Sonderentleerung sondern regulärer weitertransport zu einem Prozessmodul
                    var cacheModuleDestinations = rResult.Routes.Select(c => c.LastOrDefault().Target.ACClassID).ToArray();
                    CacheModuleDestinations = cacheModuleDestinations;
                    ACComponent cachedDest = null;
                    var subResult = CheckCachedModuleDestinations(ref cachedDest, ref msg);
                    if (subResult == StartDisResult.CycleWait)
                    {
                        CurrentDischargingRoute = previousDischargingRoute;
                        return cacheModuleDestinations != null ? StartDisResult.FastCycleWait : StartDisResult.CycleWait;
                    }
                    targetModule = cachedDest as PAProcessModule;
                    Guid acClassIdCachedDest = targetModule.ComponentClass.ACClassID;
                    var route = rResult.Routes.Where(c => c.LastOrDefault().Target.ACClassID == acClassIdCachedDest).FirstOrDefault();
                    if (route != null)
                        route.Detach();
                    CurrentDischargingRoute = route;
                }
            }

            if (findResult == FindDisRouteResult.OccupiedFromOtherOrderWarning && _CriticalRouteFoundAcknowledged == 0)
            {
                _CriticalRouteFoundAcknowledged = 1;
                // Warning50072: The found route is occupied by another order  at Order {0}, Bill of material {1}, Line {2}!
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloPicking(70)", 528, "Warning50072",
                                picking.PickingNo, pickingPos.LineNumber, pickingPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                if (previousDischargingRoute != null)
                    CurrentDischargingRoute = previousDischargingRoute;
                return StartDisResult.CycleWait;
            }

            if (findResult == FindDisRouteResult.NotFound
                || findResult == FindDisRouteResult.OccupiedFromOtherOrderBlock
                || CurrentDischargingDest(db, false) == null)
            {
                // Error50072: CurrentDischargingDest() is null because no route couldn't be found at Order {0}, Bill of material {1}, Line {2}.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloPicking(70)", 528, "Error50072",
                                picking.PickingNo, pickingPos.LineNumber, pickingPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                if (previousDischargingRoute != null)
                    CurrentDischargingRoute = previousDischargingRoute;
                return StartDisResult.CycleWait;
            }

            if (targetModule == null)
                targetModule = TargetPAModule(Root.Database as Database);
            if (targetModule == null)
            {
                // Error50073: targetSilo is null at Order {0}, Bill of material {1}, Line {2}.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloPicking(80)", 1310, "Error50073",
                                picking.PickingNo, pickingPos.LineNumber, pickingPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                if (previousDischargingRoute != null)
                    CurrentDischargingRoute = previousDischargingRoute;
                CheckIfAutomaticTargetChangePossible = false;
                return StartDisResult.CycleWait;
            }

            if (isLastDischarging 
                && pickingPos.ToFacility.Material == null
                && pickingPos.ToFacility.MDFacilityType != null
                && pickingPos.ToFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
            {
                pickingPos.ToFacility.Material = pickingPos.Material.Material1_ProductionMaterial != null ? pickingPos.Material.Material1_ProductionMaterial : pickingPos.Material;
                dbApp.ACSaveChanges();
            }

            double? disChargedWeight = GetDischargedWeight(true, null, null, acMethod);

            core.datamodel.ACClassMethod refPAACClassMethod = RefACClassMethodOfContentWF;
            if (refPAACClassMethod == null)
            {
                if (previousDischargingRoute != null)
                    CurrentDischargingRoute = previousDischargingRoute;
                return StartDisResult.CancelDischarging;
            }

            bool isNewACMethod = false;
            if (acMethod == null)
            {
                acMethod = refPAACClassMethod.TypeACSignature();
                if (acMethod == null)
                {
                    //Error50153: acMethod is null.
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloPicking(81)", 1320, "Error50153");

                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    if (previousDischargingRoute != null)
                        CurrentDischargingRoute = previousDischargingRoute;
                    return StartDisResult.CycleWait;
                }

                isNewACMethod = true;
                if (!(bool)ExecuteMethod(nameof(GetConfigForACMethod), acMethod, true, dbApp, pickingPos, targetModule))
                {
                    if (previousDischargingRoute != null)
                        CurrentDischargingRoute = previousDischargingRoute;
                    return StartDisResult.CycleWait;
                }
            }

            if (!ValidateAndSetRouteForParam(acMethod))
            {
                if (previousDischargingRoute != null)
                    CurrentDischargingRoute = previousDischargingRoute;
                // Warning50072: The found route is occupied by another order  at Order {0}, Bill of material {1}, Line {2}!
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloPicking(70)", 528, "Warning50072",
                                picking.PickingNo, pickingPos.LineNumber, pickingPos.Material.MaterialName1);
                if (ApplicationManager.RoutingTrySearchAgainIfOnlyWarning)
                    _CriticalRouteFoundAcknowledged = 1;

                return StartDisResult.CycleWait;
            }
            _CriticalRouteFoundAcknowledged = 0;

            ACValue acValue = acMethod.ParameterValueList.GetACValue("Destination");
            if (acValue != null)
            {
                if (acValue.ObjectType != null)
                    acValue.Value = Convert.ChangeType(targetModule.RouteItemIDAsNum, acValue.ObjectType);
                else
                    acMethod["Destination"] = targetModule.RouteItemIDAsNum;
            }

            if ((ParentPWMethod<PWMethodTransportBase>() != null
                    && (   ((ACSubStateEnum)ParentPWMethod<PWMethodTransportBase>().CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrder)
                        || ((ACSubStateEnum)ParentPWMethod<PWMethodTransportBase>().CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode))
                 )
                || (ParentPWGroup != null
                    && (   ((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrder)
                        || ((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode))
                   )
                )
            {
                acValue = acMethod.ParameterValueList.GetACValue(PWMethodVBBase.IsLastBatchParamName);
                if (acValue != null)
                    acValue.Value = (Int16)1;
            }
            else if (ParentPWMethod<PWMethodTransportBase>() != null)
            {
                acValue = acMethod.ParameterValueList.GetACValue(PWMethodVBBase.IsLastBatchParamName);
                if (acValue != null && acValue.ParamAsInt16 <= (Int16)0)
                    acValue.Value = (Int16)ParentPWMethod<PWMethodTransportBase>().IsLastBatch;
            }

            acValue = acMethod.ParameterValueList.GetACValue("InterDischarging");
            if (acValue != null
                && ParentPWGroup != null
                && (((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMInterDischarging)
                    || ((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMDisThenNextComp)))
            {
                acValue.Value = (Int16)1;
            }

            acValue = acMethod.ParameterValueList.GetACValue("Source");
            if (acValue != null 
                && acValue.ParamAsInt16 == (Int16)0 
                && pickingPos.FromFacility != null 
                && pickingPos.FromFacility.VBiFacilityACClassID.HasValue)
            {
                var acClass = pickingPos.FromFacility.FacilityACClass;
                if (acClass != null)
                {
                    IRouteItemIDProvider routeItemIDProvider = ACUrlCommand(acClass.ACUrlComponent) as IRouteItemIDProvider;
                    if (routeItemIDProvider != null)
                    {
                        Int32 itemId = routeItemIDProvider.RouteItemIDAsNum;
                        if (itemId < Int16.MaxValue && itemId > Int16.MinValue)
                            acValue.Value = (Int16)routeItemIDProvider.RouteItemIDAsNum;
                    }
                }
            }

            gip.core.processapplication.PAEScaleTotalizing totalizingScale = TotalizingScaleIfSWT;
            if (IsAutomaticContinousWeighing && totalizingScale != null)
            {
                acValue = acMethod.ParameterValueList.GetACValue("ScaleBatchWeight");
                if (acValue != null)
                    acValue.Value = totalizingScale.SWTTipWeight;
            }

            if (isNewACMethod)
            {
                if (!(bool)ExecuteMethod(nameof(AfterConfigForACMethodIsSet), acMethod, true, dbApp, pickingPos, targetModule))
                {
                    if (previousDischargingRoute != null)
                    {
                        CurrentDischargingRoute = previousDischargingRoute;
                        acMethod[nameof(Route)] = previousDischargingRoute; // Revert route, because Parameter was already set in ValidateAndSetRouteForParam
                    }
                    return StartDisResult.CycleWait;
                }
            }

            if (!acMethod.IsValid())
            {
                // Error50074: Dischargingtask not startable Order {0}, Bill of material {1}, line {2}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingPicking(90)", 1200, "Error50074",
                                picking.PickingNo, pickingPos.LineNumber, pickingPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                if (previousDischargingRoute != null)
                {
                    CurrentDischargingRoute = previousDischargingRoute;
                    acMethod[nameof(Route)] = previousDischargingRoute; // Revert route, because Parameter was already set in ValidateAndSetRouteForParam
                }
                return StartDisResult.CycleWait;
            }

            OnSwitchingToNextSilo(dbApp, pickingPos, discharging, targetContainer, fullSilo, targetSiloACComp as PAMSilo, fullSiloReservation, nextPlannedSiloReservation);

            if (CurrentDischargingRoute != null)
                CurrentDischargingRoute.Detach(true);
            
            // Sende neues Ziel an dies SPS
            msg = OnReSendACMethod(discharging, acMethod, dbApp);
            if (msg != null)
            {
                CurrentDischargingRoute = previousDischargingRoute;
                acMethod[nameof(Route)] = previousDischargingRoute; // Revert route, because Parameter was already set in ValidateAndSetRouteForParam
            }
            else
            {
                CheckIfAutomaticTargetChangePossible = true;
                if (fullSiloReservation != null)
                    fullSiloReservation.ReservationState = GlobalApp.ReservationState.Finished;
                if (nextPlannedSiloReservation != null)
                    nextPlannedSiloReservation.ReservationState = GlobalApp.ReservationState.Active;

                if (disChargedWeight.HasValue)
                {
                    //double actualWeight, DatabaseApp dbApp, RouteItem dischargingDest, Picking picking, PickingPos pickingPos, ACEventArgs e, bool isDischargingEnd
                    DoInwardBooking(disChargedWeight.Value, dbApp, previousDischargingRoute.LastOrDefault(), fullSiloReservation?.Facility, picking, pickingPos, null, false);
                    RememberWeightOnRunDischarging(false);
                }

                if (pickingPos.MDDelivPosLoadState == null || pickingPos.MDDelivPosLoadState.DelivPosLoadState == MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad)
                    pickingPos.MDDelivPosLoadState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadingActive).FirstOrDefault();
                // Falls Zielsilo nicht belegt
                if (dbApp.IsChanged)
                {
                    if (isLastDischarging 
                        && pickingPos.ToFacility.Material == null
                        && pickingPos.ToFacility.MDFacilityType != null
                        && pickingPos.ToFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                        pickingPos.ToFacility.Material = pickingPos.Material.Material1_ProductionMaterial != null ? pickingPos.Material.Material1_ProductionMaterial : pickingPos.Material;
                    MsgWithDetails msg2 = dbApp.ACSaveChanges();
                    if (msg2 != null)
                    {
                        Messages.LogException(this.GetACUrl(), "OnHandleStateCheckFullSiloPicking(100)", msg2.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, new Msg(msg2.InnerMessage, this, eMsgLevel.Exception, PWClassName, "OnHandleStateCheckFullSiloPicking", 1330), true);
                    }
                }

                NoTargetWait = null;
                // Quittiere Alarm
                discharging.AcknowledgeAlarms();
                this.TaskSubscriptionPoint.Persist(false);
                ExecuteMethod(nameof(OnACMethodSended), acMethod, false, dbApp, pickingPos, targetModule, discharging);
                if (discharging.CurrentACState == ACStateEnum.SMPaused)
                    discharging.Resume();
                OnSwitchedToNextSilo(dbApp, pickingPos, discharging as PAFDischarging, targetModule, null, targetSiloACComp as PAMSilo, null, nextPlannedSiloReservation);

            }
            return StartDisResult.WaitForCallback;
        }

        protected virtual void OnSwitchedToNextSilo(DatabaseApp dbApp, PickingPos pickingPos, PAFDischarging discharging, PAProcessModule targetContainer,
                                                    PAMSilo fullSilo, PAMSilo nextSilo,
                                                    FacilityReservation fullSiloReservation, FacilityReservation nextSiloReservation)
        {
            // Quittiere Alarm und setze fort falls pausiert
            if (discharging != null)
            {
                discharging.AcknowledgeAlarms();
                if (discharging.CurrentACState == ACStateEnum.SMPaused)
                    discharging.Resume();
            }
        }

        protected virtual void OnSwitchingToNextSilo(DatabaseApp dbApp, PickingPos pickingPos, PAFDischarging discharging, PAProcessModule targetContainer,
                                            PAMSilo fullSilo, PAMSilo nextSilo,
                                            FacilityReservation fullSiloReservation, FacilityReservation nextSiloReservation)
        {
        }
        #endregion

        #region Booking
        public virtual bool CanExecutePosting(double actualWeight, DatabaseApp dbApp, RouteItem dischargingDest, Picking picking, PickingPos pickingPos, ACEventArgs e, bool isDischargingEnd, ACMethodBooking bookingParam)
        {
            if (bookingParam == null)
            {
                // If there are any PWDosings od PWManualWeighings, then Relocationbooking is already done
                PWMethodTransportBase pwMethodTransport = ParentPWMethod<PWMethodTransportBase>();
                return   pwMethodTransport != null
                     && !pwMethodTransport.FindChildComponents<IPWNodeReceiveMaterial>(c => c is IPWNodeReceiveMaterial).Any();
            }
            return true;
        }

        public virtual Msg DoInwardBooking(double actualWeight, DatabaseApp dbApp, RouteItem dischargingDest, Facility facilityDest, Picking picking, PickingPos pickingPos, ACEventArgs e, bool isDischargingEnd)
        {
            MsgWithDetails collectedMessages = new MsgWithDetails();
            Msg msg = null;
            if (   actualWeight > 0.000001 
                && pickingPos != null
                && ACFacilityManager != null 
                && PickingManager != null
                && !NoPostingOnRelocation
                && CanExecutePosting(actualWeight, dbApp, dischargingDest, picking, pickingPos, e, isDischargingEnd, null))
            {
                if (pickingPos.Material == null)
                {
                    pickingPos.MDDelivPosLoadState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();
                    pickingPos.ACClassTaskID2 = null;
                    msg = dbApp.ACSaveChangesWithRetry();
                    if (msg != null)
                    {
                        collectedMessages.AddDetailMessage(msg);
                        Messages.LogError(this.GetACUrl(), "DoInwardBooking(0)", msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoInwardBooking", 100), true);
                    }
                }
                else 
                {
                    //Alibi-No
                    Weighing weighing = InsertNewWeighingIfAlibi(dbApp, actualWeight, e);
                    if (weighing != null)
                    {
                        weighing.PickingPos = pickingPos;
                        pickingPos.Weighing_PickingPos.Add(weighing);
                    }

                    // 1. Bereite Buchung vor
                    FacilityPreBooking facilityPreBooking = ACFacilityManager.NewFacilityPreBooking(dbApp, pickingPos, facilityDest, pickingPos.Material.ConvertBaseWeightToBaseUnit(actualWeight));
                    ACMethodBooking bookingParam = facilityPreBooking.ACMethodBooking as ACMethodBooking;
                    if (ParentPWGroup != null && ParentPWGroup.AccessedProcessModule != null)
                        bookingParam.PropertyACUrl = ParentPWGroup.AccessedProcessModule.GetACUrl();
                    if (PostingBehaviour != PostingBehaviourEnum.NotSet)
                        bookingParam.PostingBehaviour = PostingBehaviour;

                    OnInwardBookingPre(facilityPreBooking, collectedMessages, dbApp, dischargingDest, picking, pickingPos, e, isDischargingEnd);

                    msg = dbApp.ACSaveChangesWithRetry();

                    // 2. Führe Buchung durch
                    if (msg != null)
                    {
                        collectedMessages.AddDetailMessage(msg);
                        Messages.LogError(this.GetACUrl(), "DoInwardBooking(5)", msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoInwardBooking", 100), true);
                    }
                    else if (facilityPreBooking != null)
                    {
                        bookingParam.IgnoreIsEnabled = true;
                        ACMethodEventArgs resultBooking = null;
                        bool canExecutePosting = CanExecutePosting(actualWeight, dbApp, dischargingDest, picking, pickingPos, e, isDischargingEnd, bookingParam);
                        if (canExecutePosting)
                            resultBooking = ACFacilityManager.BookFacilityWithRetry(ref bookingParam, dbApp);
                        if (resultBooking != null && (resultBooking.ResultState == Global.ACMethodResultState.Failed || resultBooking.ResultState == Global.ACMethodResultState.Notpossible))
                        {
                            OnNewAlarmOccurred(ProcessAlarm, new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoInwardBooking", 1350), true);
                        }
                        else
                        {
                            if (resultBooking != null && (!bookingParam.ValidMessage.IsSucceded() || bookingParam.ValidMessage.HasWarnings()))
                            {
                                Messages.LogError(this.GetACUrl(), "DoInwardBooking(6)", bookingParam.ValidMessage.InnerMessage);
                                OnNewAlarmOccurred(ProcessAlarm, new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoInwardBooking", 1340), true);
                            }
                            if (   (resultBooking == null && !canExecutePosting)
                                 || bookingParam.ValidMessage.IsSucceded())
                            {
                                if (canExecutePosting)
                                {
                                    facilityPreBooking.DeleteACObject(dbApp, true);
                                    ACFacilityManager.RecalcAfterPosting(dbApp, pickingPos, bookingParam.OutwardQuantity.Value, false);
                                    //pickingPos.RecalcAfterPosting(dbApp, bookingParam.OutwardQuantity.Value, false);
                                    //pickingPos.IncreasePickingActualUOM(bookingParam.OutwardQuantity.Value);
                                    //dosingPosRelation.TopParentPartslistPosRelation.RecalcActualQuantity();
                                    //dosingPosRelation.SourceProdOrderPartslistPos.TopParentPartslistPos.RecalcActualQuantity();
                                }
                            }
                            else
                            {
                                collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                            }

                            if (pickingPos.RemainingDosingQuantityUOM >= -1)
                            {
                                pickingPos.MDDelivPosLoadState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();
                                pickingPos.ACClassTaskID2 = null;
                            }

                            msg = dbApp.ACSaveChangesWithRetry();
                            if (msg != null)
                            {
                                collectedMessages.AddDetailMessage(msg);
                                Messages.LogError(this.GetACUrl(), "DoInwardBooking(8)", msg.InnerMessage);
                                OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoInwardBooking", 1360), true);
                            }
                            else
                            {
                                //pickingPos.RecalcActualQuantityFast();
                                if (dbApp.IsChanged)
                                    dbApp.ACSaveChanges();
                            }
                        }
                    }
                }
            }

            return collectedMessages.MsgDetailsCount > 0 ? collectedMessages : null;
        }

        public virtual Msg DoOutwardBooking(double actualQuantity, DatabaseApp dbApp, RouteItem dischargingDest, Picking picking, PickingPos pickingPos, ACEventArgs e, bool isDischargingEnd)
        {
            return null;
        }

        protected virtual void OnInwardBookingPre(FacilityPreBooking facilityPreBooking, MsgWithDetails collectedMessages, DatabaseApp dbApp, RouteItem dischargingDest, Picking picking, PickingPos pickingPos, ACEventArgs e, bool isDischargingEnd)
        {
        }

        #endregion

    }
}
