using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System.Threading;
using System.Reflection.Emit;
using System.Net;

namespace gip.mes.processapplication
{
    public partial class PWDischarging
    {
        #region Methods

        #region ACState

        protected virtual StartDisResult StartDischargingProd(PAProcessModule module)
        {
            Msg msg = null;

            // Falls Produktionsauftrag
            if (IsProduction)
            {
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
                // Else desitnation is a silo
                else if (LastTargets != null && NoTargetLongWait.HasValue && DateTime.Now < NoTargetLongWait.Value)
                {
                    short destError;
                    string errorMsg;
                    Guid[] targets = ParentPWMethod<PWMethodProduction>().GetCachedDestinations(false/*addedModules.Any()*/, out destError, out errorMsg);
                    if (!PWMethodTransportBase.AreCachedDestinationsDifferent(LastTargets, targets))
                        return StartDisResult.FastCycleWait;
                }
                NoTargetLongWait = null;

                var pwMethod = ParentPWMethod<PWMethodProduction>();
                if (pwMethod.CurrentProdOrderPartslistPos == null 
                    || pwMethod.CurrentProdOrderBatch == null 
                    || !pwMethod.CurrentProdOrderBatch.ProdOrderBatchPlanID.HasValue)
                {
                    // Error50067:Batchplan not found
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingProd(1)", 1000, "Error50067");

                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        Messages.LogError(this.GetACUrl(), "StartDischarging(1)", msg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    return StartDisResult.CycleWait;
                }

                using (var dbIPlus = new Database())
                using (var dbApp = new DatabaseApp(dbIPlus))
                {
                    //ProdOrderBatch batch = pwMethod.CurrentProdOrderBatch.FromAppContext<ProdOrderBatch>(dbApp);
                    ProdOrderPartslistPos currentBatchPos = pwMethod.CurrentProdOrderPartslistPos.FromAppContext<ProdOrderPartslistPos>(dbApp);
                    var batchPlan = dbApp.ProdOrderBatchPlan.Where(c => c.ProdOrderBatchPlanID == pwMethod.CurrentProdOrderBatch.ProdOrderBatchPlanID.Value).FirstOrDefault();
                    if (batchPlan == null || !batchPlan.VBiACClassWFID.HasValue)
                    {
                        // Error50068: Reference from Batchplan to Workflownode is null
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischarging(2)", 1010, "Error50068",
                                        batchPlan == null ? "" : batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                batchPlan == null ? "" : batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                batchPlan == null ? "" : batchPlan.ProdOrderPartslistPos.BookingMaterial.MaterialName1);

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), "StartDischarging(2)", msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        return StartDisResult.CycleWait;
                    }

                    MaterialWFConnection connectionToDischarging = null;
                    bool isLastDisNode = IsLastDisNode(dbApp, batchPlan, out connectionToDischarging);
                    ACComponent targetSiloACComp = null;
                    // Falls dies der letzte Entleerschritt ist, dann erfolgt Entleerung ins ein Silo
                    if (connectionToDischarging != null)
                    {
                        FacilityReservation nextDestination = null;
                        if (CurrentDischargingDest(dbIPlus, false) == null)
                        {
                            PWGroupVB pwGroup = ParentPWGroup as PWGroupVB;
                            // Prüfe zuerst ob in Leerfahrmodus und prüfe ob es sich um ein endgültiges Ziel handelt
                            if (pwGroup != null && pwGroup.IsInEmptyingMode && !KeepPlannedDestOnEmptying)
                            {
                                var subResult = ResolveExtraDestOnEmptyingMode(ref targetSiloACComp, ref msg, true);
                                if (subResult == StartDisResult.CycleWait)
                                    return subResult;
                                // Falls anderes Sonderziel angegeben prüfe zuert die Route
                                if (targetSiloACComp != null)
                                {
                                    Type typeOfSilo = typeof(PAMSilo);
                                    Guid thisMethodID = ContentACClassWF.ACClassMethodID;
                                    DetermineDischargingRoute(Root.Database as Database, module, targetSiloACComp, 0,
                                                            (c, p, r) => (c.ACKind == Global.ACKinds.TPAProcessModule
                                                                    && (typeOfSilo.IsAssignableFrom(c.ObjectType)
                                                                        || !c.BasedOnACClassID.HasValue
                                                                        || (c.BasedOnACClassID.HasValue && c.ACClass1_BasedOnACClass.ACClassWF_RefPAACClass.Where(refc => refc.ACClassMethodID != thisMethodID).Any()))),
                                                            PAProcessModule.SelRuleID_ProcessModule_Deselector, null);
                                    // Falls Route nicht existiert, dann fahre in Standardziel
                                    if (CurrentDischargingDest(dbIPlus, false) == null)
                                        targetSiloACComp = null;
                                }
                            }
                            // Kein Leerfahrmodus oder keine Route gefunden
                            if (targetSiloACComp == null || CurrentDischargingDest(dbIPlus, false) == null)
                            {
                                IList<FacilityReservation> plannedSilos = batchPlan.FacilityReservation_ProdOrderBatchPlan.OrderBy(c => c.Sequence).ToArray();
                                if (plannedSilos == null || !plannedSilos.Any())
                                {
                                    // Error50083: No destinations defined for Order {0}, Bill of material {1}, Line {2}
                                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischarging(2)", 1020, "Error50083",
                                                    batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                            batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                            batchPlan.ProdOrderPartslistPos.BookingMaterial.MaterialName1);

                                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                        Messages.LogError(this.GetACUrl(), "StartDischarging(2)", msg.InnerMessage);
                                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                    return StartDisResult.CycleWait;
                                }

                                nextDestination = GetNextFreeDestination(plannedSilos, currentBatchPos, true);
                                if (nextDestination == null)
                                {
                                    // Error50069: No further target-container/silo/tank found, which has enough remaining space for this batch, at Order {0}, Bill of material {1}, Line {2}
                                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischarging(3)", 1030, "Error50069",
                                                    batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                            batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                            batchPlan.ProdOrderPartslistPos.BookingMaterial.MaterialName1);

                                    if (OnCheckFullSiloNoSiloFound(null, null, msg))
                                    {
                                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                            Messages.LogError(this.GetACUrl(), "StartDischarging(3)", msg.InnerMessage);
                                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                    }

                                    var lastTargets = plannedSilos.OrderBy(c => c.VBiACClassID.Value).Select(c => c.VBiACClassID.Value).ToArray();
                                    LastTargets = lastTargets;
                                    if (lastTargets != null && lastTargets.Any())
                                    {
                                        // TODO: Falls sich im relevanten PAMSilo der Bestand verändert, dann Langes Warten verkürzen => Subscribe to Event on PAMSilo
                                        short destError;
                                        string errorMsg;
                                        Guid[] targets = ParentPWMethod<PWMethodProduction>().GetCachedDestinations(false/*addedModules.Any()*/, out destError, out errorMsg);
                                        if (!PWMethodTransportBase.AreCachedDestinationsDifferent(lastTargets, targets))
                                        {
                                            NoTargetLongWait = DateTime.Now + TimeSpan.FromSeconds(20);
                                            return StartDisResult.FastCycleWait;
                                        }
                                    }
                                    return StartDisResult.CycleWait;
                                }

                                gip.core.datamodel.ACClass acClassSilo = nextDestination.GetFacilityACClass(Root.Database as Database);
                                if (acClassSilo == null)
                                {
                                    // Error50070: acClassSilo is null at Order {0}, Bill of material {1}, Line {2}
                                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischarging(4)", 1040, "Error50070",
                                                    batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                                    batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                                    batchPlan.ProdOrderPartslistPos.BookingMaterial.MaterialName1);

                                    if (OnCheckFullSiloNoSiloFound(null, null, msg))
                                    {
                                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                            Messages.LogError(this.GetACUrl(), "StartDischarging(4)", msg.InnerMessage);
                                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                    }
                                    return StartDisResult.CycleWait;
                                }

                                targetSiloACComp = this.Root.ACUrlCommand(acClassSilo.GetACUrlComponent()) as ACComponent;
                                if (targetSiloACComp == null)
                                {
                                    // Error50071: targetSiloACComp is null at Order {0}, Bill of material {1}, Line {2}
                                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischarging(5)", 1050, "Error50071",
                                                    batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                            batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                            batchPlan.ProdOrderPartslistPos.BookingMaterial.MaterialName1);

                                    if (OnCheckFullSiloNoSiloFound(null, null, msg))
                                    {
                                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                            Messages.LogError(this.GetACUrl(), "StartDischarging(5)", msg.InnerMessage);
                                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                    }
                                    return StartDisResult.CycleWait;
                                }
                                Type typeOfSilo = typeof(PAMSilo);
                                Guid thisMethodID = ContentACClassWF.ACClassMethodID;
                                DetermineDischargingRoute(Root.Database as Database, module, targetSiloACComp, 0,
                                                        (c, p, r) => (c.ACKind == Global.ACKinds.TPAProcessModule
                                                                && (typeOfSilo.IsAssignableFrom(c.ObjectType)
                                                                    || !c.BasedOnACClassID.HasValue
                                                                    || (c.BasedOnACClassID.HasValue && c.ACClass1_BasedOnACClass.ACClassWF_RefPAACClass.Where(refc => refc.ACClassMethodID != thisMethodID).Any()))),
                                                        PAMSilo.SelRuleID_Silo_Deselector, null);

                                nextDestination.ReservationState = GlobalApp.ReservationState.Active;
                                // Falls Zielsilo nicht belegt
                                if (nextDestination.Facility != null
                                    && nextDestination.Facility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                                {
                                    if (!nextDestination.Facility.MaterialID.HasValue)
                                        nextDestination.Facility.Material = batchPlan.ProdOrderPartslistPos.BookingMaterial?.Material1_ProductionMaterial != null ? batchPlan.ProdOrderPartslistPos.BookingMaterial.Material1_ProductionMaterial : batchPlan.ProdOrderPartslistPos.BookingMaterial;
                                    if (!batchPlan.ProdOrderPartslistPos.IsFinalMixure
                                        && (!nextDestination.Facility.PartslistID.HasValue
                                            || nextDestination.Facility.PartslistID != batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.PartslistID))
                                        nextDestination.Facility.Partslist = batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist;
                                }
                                if (dbApp.IsChanged)
                                    dbApp.ACSaveChanges();
                            }
                        }

                        if (CurrentDischargingDest(dbIPlus, false) == null)
                        {
                            // Error50072: CurrentDischargingDest() is null because no route couldn't be found at Order {0}, Bill of material {1}, Line {2}.
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischarging(6)", 1060, "Error50072",
                                            batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                    batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                    batchPlan.ProdOrderPartslistPos.BookingMaterial.MaterialName1);

                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                Messages.LogError(this.GetACUrl(), "StartDischarging(6)", msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                            return StartDisResult.CycleWait;
                        }

                        PAProcessModule targetModule = TargetPAModule(Root.Database as Database) as PAProcessModule;
                        if (targetModule == null)
                        {
                            // Error50073: targetSilo is null at Order {0}, Bill of material {1}, Line {2}.
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischarging(7)", 1070, "Error50073",
                                            batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                    batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                    batchPlan.ProdOrderPartslistPos.BookingMaterial.MaterialName1);

                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                Messages.LogError(this.GetACUrl(), "StartDischarging(7)", msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                            return StartDisResult.CycleWait;
                        }

                        core.datamodel.ACClassMethod refPAACClassMethod = RefACClassMethodOfContentWF;
                        if (refPAACClassMethod == null)
                            return StartDisResult.CancelDischarging;

                        ACMethod acMethod = refPAACClassMethod.TypeACSignature();
                        if (acMethod == null)
                        {
                            //Error50153: acMethod is null.
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischarging(7c)", 1080, "Error50153");
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                            return StartDisResult.CycleWait;
                        }
                        PAProcessFunction responsibleFunc = CanStartProcessFunc(module, acMethod, dbApp, batchPlan, currentBatchPos, targetModule);
                        if (responsibleFunc == null)
                            return StartDisResult.CycleWait;

                        if (!(bool)ExecuteMethod(nameof(GetConfigForACMethod), acMethod, true, dbApp, batchPlan, currentBatchPos, targetModule))
                            return StartDisResult.CycleWait;

                        acMethod["Route"] = CurrentDischargingRoute != null ? CurrentDischargingRoute.Clone() as Route : null;
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

                        if (  (ParentPWMethod<PWMethodProduction>() != null
                                && (   ((ACSubStateEnum)ParentPWMethod<PWMethodProduction>().CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrder)
                                    || ((ACSubStateEnum)ParentPWMethod<PWMethodProduction>().CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode))
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
                        else if (ParentPWMethod<PWMethodProduction>() != null)
                        {
                            acValue = acMethod.ParameterValueList.GetACValue(PWMethodVBBase.IsLastBatchParamName);
                            if (acValue != null && acValue.ParamAsInt16 <= (Int16)0)
                                acValue.Value = (Int16)ParentPWMethod<PWMethodProduction>().IsLastBatch;
                        }
                        if (acValue != null)
                            OnSetLastBatchParam(acValue, acMethod, targetModule, dbApp, batchPlan, currentBatchPos);

                        acValue = acMethod.ParameterValueList.GetACValue("InterDischarging");
                        if (   acValue != null
                            && ParentPWGroup != null
                            && (((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMInterDischarging)
                                || ((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMDisThenNextComp)))
                        {
                            acValue.Value = (Int16)1;
                        }

                        NoTargetWait = null;
                        if (!(bool)ExecuteMethod(nameof(AfterConfigForACMethodIsSet), acMethod, true, dbApp, batchPlan, currentBatchPos, targetModule))
                            return StartDisResult.CycleWait;

                        if (!acMethod.IsValid())
                        {
                            // Error50074: Dischargingtask not startable Order {0}, Bill of material {1}, line {2}
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischarging(7b)", 1090, "Error50074",
                                            batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                    batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                    batchPlan.ProdOrderPartslistPos.BookingMaterial.MaterialName1);

                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                Messages.LogError(this.GetACUrl(), "StartDischarging(7b)", msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                            return StartDisResult.CycleWait;
                        }

                        RecalcTimeInfo();
                        if (CreateNewProgramLog(acMethod) <= CreateNewProgramLogResult.ErrorNoProgramFound)
                            return StartDisResult.CycleWait;
                        _ExecutingACMethod = acMethod;

                        module.TaskInvocationPoint.ClearMyInvocations(this);
                        _CurrentMethodEventArgs = null;
                        IACPointEntry task = module.TaskInvocationPoint.AddTask(acMethod, this);
                        if (!IsTaskStarted(task))
                        {
                            ACMethodEventArgs eM = _CurrentMethodEventArgs;
                            if (eM == null || eM.ResultState != Global.ACMethodResultState.FailedAndRepeat)
                            {
                                // Error50074: Dischargingtask not startable Order {0}, Bill of material {1}, line {2}
                                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischarging(8)", 1100, "Error50074",
                                                batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                        batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                        batchPlan.ProdOrderPartslistPos.BookingMaterial.MaterialName1);

                                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                    Messages.LogError(this.GetACUrl(), "StartDischarging(8)", msg.InnerMessage);
                                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                            }
                            return StartDisResult.CycleWait;
                        }
                        UpdateCurrentACMethod();
                        RememberWeightOnRunDischarging(true);

                        CheckIfAutomaticTargetChangePossible = null;
                        MsgWithDetails msg2 = dbApp.ACSaveChanges();
                        if (msg2 != null)
                        {
                            Messages.LogException(this.GetACUrl(), "StartDischarging(9)", msg2.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, new Msg(msg2.InnerMessage, this, eMsgLevel.Exception, PWClassName, "StartDischargingProd", 1110), true);
                            return StartDisResult.CycleWait;
                        }
                        AcknowledgeAlarms();
                        ExecuteMethod(nameof(OnACMethodSended), acMethod, true, dbApp, batchPlan, currentBatchPos, targetModule, responsibleFunc);
                        OnSwitchedToNextSilo(dbApp, currentBatchPos, responsibleFunc as PAFDischarging, targetModule, null, targetSiloACComp as PAMSilo, null, nextDestination);
                        return task.State == PointProcessingState.Deleted ? StartDisResult.CancelDischarging : StartDisResult.WaitForCallback;
                        //return StartDisResult.WaitForCallback;
                    }
                    // Sonst ist dieser Entleerschritt eine Entleerung in einen Zwischenbehälter/Processmodul
                    else
                    {
                        if (ParentPWGroup == null)
                        {
                            Messages.LogError(this.GetACUrl(), "StartDischargingProd(9a)", "ParentPWGroup == null");
                            return StartDisResult.CycleWait;
                        }
                        var acClassModule = ParentPWGroup.AccessedProcessModule.ComponentClass.FromIPlusContext<gip.core.datamodel.ACClass>(dbIPlus);

                        IEnumerable<Route> routes = null;

                        PWGroupVB pwGroup = ParentPWGroup as PWGroupVB;
                        targetSiloACComp = null;
                        // Prüfe zuerst ob in Leerfahrmodus und prüfe ob es sich um ein Ziel handelt, das von hier aus direkt angesteuert werden kann oder ob es über die nachfolgenden PWGruppen geschleuet werden muss:
                        if (pwGroup != null && pwGroup.IsInEmptyingMode && !KeepPlannedDestOnEmptying)
                        {
                            var subResult = ResolveExtraDestOnEmptyingMode(ref targetSiloACComp, ref msg, true);
                            if (subResult == StartDisResult.CycleWait)
                                return subResult;
                            // Prüfe ob es bei der Sonderentleerung von diesem Punkt aus ein DIREKTES Entleerziel gibt, sonst entleere in die gemappte Gruppe 
                            if (targetSiloACComp != null)
                            {
                                Guid targetACClassID = targetSiloACComp.ComponentClass.ACClassID;
                                RoutingResult rResult = ACRoutingService.FindSuccessors(RoutingService, dbIPlus, RoutingService != null && RoutingService.IsProxy,
                                                    ParentPWGroup.AccessedProcessModule, PWGroupVB.SelRuleID_IsDestDirectSucessor, RouteDirections.Forwards, new object[] { targetACClassID },
                                                    (c, p, r) => c.ACClassID == targetACClassID,
                                                    (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule,
                                                    0, true, true, false, false);
                                routes = rResult.Routes;
                            }
                        }
                        // Falls keine Sonderentleerung bzw. keine direkte Route zum Sonderentleerziel gefunden, suche Wege zu den nachfolgend möglichen Prozessmodulen
                        if (routes == null || !routes.Any())
                        {
                            targetSiloACComp = null;
                            RoutingResult rResult = ACRoutingService.FindSuccessors(RoutingService, dbIPlus, RoutingService != null && RoutingService.IsProxy,
                                                ParentPWGroup.AccessedProcessModule, PAProcessModule.SelRuleID_ProcessModule, RouteDirections.Forwards, new object[] { },
                                                (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule,
                                                null,
                                                0, true, true, false, false);
                            routes = rResult.Routes;
                        }
                        if (routes != null && routes.Any())
                        {
                            // Falls keine Sonderentleerung sondern regulärer weitertransport zu einem Prozessmodul
                            if (targetSiloACComp == null)
                            {
                                cacheModuleDestinations = routes.Select(c => c.LastOrDefault().Target.ACClassID).ToArray();
                                CacheModuleDestinations = cacheModuleDestinations;
                                ACComponent cachedDest = null;
                                var subResult = CheckCachedModuleDestinations(ref cachedDest, ref msg);
                                if (subResult == StartDisResult.CycleWait)
                                    return cacheModuleDestinations != null ? StartDisResult.FastCycleWait : StartDisResult.CycleWait;
                                dischargeToModule = cachedDest as PAProcessModule;
                            }
                            // Sonst Sonderentleerung und es wurde ein direktes Entleerziel gefunden
                            else
                                dischargeToModule = targetSiloACComp as PAProcessModule;

                            // Falls ein Ziel ermittelt werden konnte
                            if (dischargeToModule != null)
                            {
                                core.datamodel.ACClassMethod refPAACClassMethod = RefACClassMethodOfContentWF;
                                if (refPAACClassMethod == null)
                                    return StartDisResult.CancelDischarging;

                                ACMethod acMethod = refPAACClassMethod.TypeACSignature();
                                if (acMethod == null)
                                {
                                    //Error50153: acMethod is null.
                                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingProd(9b)", 1120, "Error50153");
                                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                    return StartDisResult.CycleWait;
                                }
                                PAProcessFunction responsibleFunc = CanStartProcessFunc(module, acMethod, dbApp, batchPlan, currentBatchPos, dischargeToModule);
                                if (responsibleFunc == null)
                                    return StartDisResult.CycleWait;

                                Guid toModuleClassID = dischargeToModule.ComponentClass.ACClassID;
                                var route = routes.Where(c => c.LastOrDefault().Target.ACClassID == toModuleClassID).FirstOrDefault();
                                if (route != null)
                                    route.Detach(true);
                                CurrentDischargingRoute = route;

                                if (!(bool)ExecuteMethod(nameof(GetConfigForACMethod), acMethod, true, dbApp, batchPlan, currentBatchPos, dischargeToModule))
                                    return StartDisResult.CycleWait;

                                acMethod["Route"] = CurrentDischargingRoute != null ? CurrentDischargingRoute.Clone() as Route : null;
                                ACValue acValue = acMethod.ParameterValueList.GetACValue("Destination");
                                if (acValue != null)
                                {
                                    if (acValue.ObjectType != null)
                                        acValue.Value = Convert.ChangeType(dischargeToModule.RouteItemIDAsNum, acValue.ObjectType);
                                    else
                                        acMethod["Destination"] = dischargeToModule.RouteItemIDAsNum;
                                }
                                if (CurrentDischargingRoute != null)
                                    CurrentDischargingRoute.Detach(true);

                                if (    (ParentPWMethod<PWMethodProduction>() != null
                                        && (   ((ACSubStateEnum)ParentPWMethod<PWMethodProduction>().CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrder)
                                            || ((ACSubStateEnum)ParentPWMethod<PWMethodProduction>().CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode))
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
                                else if (ParentPWMethod<PWMethodProduction>() != null)
                                {
                                    acValue = acMethod.ParameterValueList.GetACValue(PWMethodVBBase.IsLastBatchParamName);
                                    if (acValue != null && acValue.ParamAsInt16 <= (Int16)0)
                                        acValue.Value = (Int16)ParentPWMethod<PWMethodProduction>().IsLastBatch;
                                }
                                if (acValue != null)
                                    OnSetLastBatchParam(acValue, acMethod, dischargeToModule, dbApp, batchPlan, currentBatchPos);

                                acValue = acMethod.ParameterValueList.GetACValue("InterDischarging");
                                if (acValue != null
                                    && ParentPWGroup != null
                                    && (((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMInterDischarging)
                                        || ((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMDisThenNextComp)))
                                {
                                    acValue.Value = (Int16)1;
                                }

                                PWNodeCheckWeighing checkWeighing = this.FindSuccessors<PWNodeCheckWeighing>(false, c => c is PWNodeCheckWeighing, null, 1).FirstOrDefault();
                                if (checkWeighing != null)
                                {
                                    double? targetWeight = 0;
                                    try { targetWeight = checkWeighing.CalcTargetWeight(); }
                                    catch(Exception ec)
                                    {
                                        Messages.LogException("PWDischarging_Prod", "StartDischargingProd", ec);
                                    }
                                    if (!targetWeight.HasValue)
                                        targetWeight = 0;
                                    acValue = acMethod.ParameterValueList.GetACValue("TargetQuantity");
                                    if (acValue != null)
                                        acValue.Value = (double)targetWeight;
                                }
                                if (!(bool)ExecuteMethod(nameof(AfterConfigForACMethodIsSet), acMethod, true, dbApp, batchPlan, currentBatchPos, dischargeToModule))
                                    return StartDisResult.CycleWait;


                                if (!acMethod.IsValid())
                                {
                                    // Error50074: Dischargingtask not startable Order {0}, Bill of material {1}, line {2}
                                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischarging(19)", 1130, "Error50074",
                                                    batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                                    batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                                    acMethod.ValidMessage.Message);

                                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                        Messages.LogError(this.GetACUrl(), "StartDischarging(19)", msg.Message);
                                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                    return StartDisResult.CycleWait;
                                }

                                NoTargetWait = null;
                                RecalcTimeInfo();
                                if (CreateNewProgramLog(acMethod) <= CreateNewProgramLogResult.ErrorNoProgramFound)
                                    return StartDisResult.CycleWait;
                                _ExecutingACMethod = acMethod;

                                module.TaskInvocationPoint.ClearMyInvocations(this);
                                _CurrentMethodEventArgs = null;
                                IACPointEntry task = module.TaskInvocationPoint.AddTask(acMethod, this);
                                if (!IsTaskStarted(task))
                                {
                                    ACMethodEventArgs eM = _CurrentMethodEventArgs;
                                    if (eM == null || eM.ResultState != Global.ACMethodResultState.FailedAndRepeat)
                                    {
                                        // Error50074: Dischargingtask not startable Order {0}, Bill of material {1}, line {2}
                                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingProd(20)", 1140, "Error50074",
                                                    batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                            batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                            batchPlan.ProdOrderPartslistPos.BookingMaterial.MaterialName1);

                                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                            Messages.LogError(this.GetACUrl(), "StartDischarging(20)", msg.InnerMessage);
                                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                    }
                                    return StartDisResult.CycleWait;
                                }
                                UpdateCurrentACMethod();

                                AcknowledgeAlarms();
                                ExecuteMethod(nameof(OnACMethodSended), acMethod, true, dbApp, batchPlan, currentBatchPos, dischargeToModule, responsibleFunc);
                                return task.State == PointProcessingState.Deleted ? StartDisResult.CancelDischarging : StartDisResult.WaitForCallback;
                                //return StartDisResult.WaitForCallback;
                            }
                            else
                            {
                                return StartDisResult.CycleWait;
                            }
                        }
                        else
                        {
                            CacheModuleDestinations = null;
                            if (this.InitState == ACInitState.Initialized && Root.InitState == ACInitState.Initialized)
                            {
                                // Error50075: No downward-route defined for module {0}
                                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingProd(30)", 1150, "Error50075", ParentPWGroup.AccessedProcessModule.GetACUrl());

                                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                    Messages.LogError(this.GetACUrl(), "StartDischargingProd(30)", msg.InnerMessage);
                                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                            }
                            return StartDisResult.CycleWait;
                        }
                    }
                }
            }
            return StartDisResult.CancelDischarging;
        }

        protected virtual StartDisResult CheckDischargingToExtraTarget(ref ACComponent dischargeToModule, ref Msg msg, bool isFinalDischarging)
        {
            var pwGroup = ParentPWGroup;
            var rootPW = RootPW;
            if (pwGroup == null || rootPW == null)
            {
                if (this.InitState == ACInitState.Initialized)
                    Messages.LogError(this.GetACUrl(), "CheckDischargingToExtraTarget()", "ParentPWGroup is null");
                return StartDisResult.CycleWait;
            }

            PAProcessModuleVB thisAccessedPM = pwGroup.AccessedProcessModule as PAProcessModuleVB;
            if (thisAccessedPM != null)
            {
                if (!String.IsNullOrEmpty(thisAccessedPM.ACUrlExtraDisDest))
                {
                    dischargeToModule = ACUrlCommand(thisAccessedPM.ACUrlExtraDisDest) as ACComponent;
                }
            }
            if (dischargeToModule == null && pwGroup is PWGroupVB)
            {
                string acUrlExtraDisDest;
                if ((pwGroup as PWGroupVB).IsExtraDisTargetEntered(out acUrlExtraDisDest))
                {
                    dischargeToModule = ACUrlCommand(acUrlExtraDisDest) as ACComponent;
                }
            }
            if (dischargeToModule == null)
            {
                // Error50092: Kein Sonderentleerziel definiert.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "CheckDischargingToExtraTarget(10)", 1160, "Error50092");

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }
            // Falls Entleerung in ein Nachfolgemodul und nicht endgültiges Siloziel dann prüfe ob eine Route zum Sonderentleerziel existiert:
            if (!isFinalDischarging && !CacheModuleDestinations.Contains(dischargeToModule.ComponentClass.ACClassID))
            {
                // Error50093: Keine Route zum Sonderentleerziel gefunden.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "CheckDischargingToExtraTarget(20)", 1170, "Error50093");

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }
            return StartDisResult.WaitForCallback;
        }

        /// <summary>
        /// Prüfe ob der Benutzer ein Sonderentleerziel angegeben hat für den Leerfahrmodus.
        /// Falls er noch keine Eingabe gemacht hat gibt die Methode StartDisResult.CycleWait zurück
        /// Falls er etwas eingegeben hat und er möchte es in das geplante Ziel entleeren, dann ist dischargeToModule == null und die Methode gibt StartDisResult.WaitForCallback zurück
        /// Hat der Benutzer ein gültiges Ziel eingegeben, dann wird dischargeToModule gesetzt
        /// </summary>
        /// <param name="dischargeToModule"></param>
        /// <param name="msg"></param>
        /// <param name="isFinalDischarging"></param>
        /// <returns></returns>
        protected virtual StartDisResult ResolveExtraDestOnEmptyingMode(ref ACComponent dischargeToModule, ref Msg msg, bool isFinalDischarging)
        {
            var pwGroup = ParentPWGroup as PWGroupVB;
            var rootPW = RootPW as PWMethodVBBase;
            if (pwGroup == null || rootPW == null)
            {
                if (this.InitState == ACInitState.Initialized)
                    Messages.LogError(this.GetACUrl(), "CheckDischargingOnEmptyingMode()", "ParentPWGroup is null");
                return StartDisResult.CycleWait;
            }

            string acUrlExtraDisDest = null;
            // Benutzer hat noch gar kein Ziel definiert wo es hingehen soll
            if (!pwGroup.IsExtraDisTargetEntered(out acUrlExtraDisDest))
            {
                //Error50163: No special emptying destination entered.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "ResolveExtraDestOnEmptyingMode(0)", 1180, "Error50163");

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            dischargeToModule = pwGroup.ExtraDisTargetComp;

            if (dischargeToModule == null)
            {
                //Error50164:The entered special destination is invalid.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "ResolveExtraDestOnEmptyingMode(20)", 1190, "Error50164");

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.Message, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
                //return StartDisResult.WaitForCallback;
            }
            // Falls Entleerung in ein Nachfolgemodul und nicht endgültiges Siloziel dann prüfe ob eine Route zum Sonderentleerziel existiert:
            if (!isFinalDischarging && CacheModuleDestinations.Contains(dischargeToModule.ComponentClass.ACClassID))
            {
                // Error50093: Keine Route zum Sonderentleerziel gefunden.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "ResolveExtraDestOnEmptyingMode(30)", 1200, "Error50093");

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }
            return StartDisResult.WaitForCallback;
        }


        public virtual StartDisResult CheckCachedModuleDestinations(ref ACComponent dischargeToModule, ref Msg msg)
        {
            PWGroupVB pwGroup = ParentPWGroup as PWGroupVB;
            if (pwGroup == null)
            {
                if (this.InitState == ACInitState.Initialized)
                    Messages.LogError(this.GetACUrl(), "CheckCachedModuleDestinations()", "ParentPWGroup is null");
                return StartDisResult.CycleWait;
            }

            // Falls Entleerung in Sonderentleerziel
            if (((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMDisThenNextComp))
            {
                var subResult = CheckDischargingToExtraTarget(ref dischargeToModule, ref msg, false);
                if (subResult == StartDisResult.CycleWait)
                    return subResult;
            }
            // Sonst Entleerung in ein vom Steuerrezept gemapptes Prozessmodul
            else
            {
                var cacheModuleDestinations = CacheModuleDestinations;
                PWGroupVB cacheDestinationGroup = ParentPWMethod<PWMethodVBBase>()
                                    .FindChildComponents<PWGroupVB>(c => c is PWGroupVB
                                            && GetDestinationPMModule(c as PWGroupVB) != null
                                            && cacheModuleDestinations.Contains(GetDestinationPMModule(c as PWGroupVB).ComponentClass.ACClassID)).FirstOrDefault();
                if (cacheDestinationGroup != null)
                    dischargeToModule = GetDestinationPMModule(cacheDestinationGroup);
                if (dischargeToModule == null)
                    return StartDisResult.CycleWait;
            }
            return StartDisResult.WaitForCallback;
        }

        public virtual PAProcessModule GetDestinationPMModule(PWGroupVB fordestinationGroup)
        {
            return fordestinationGroup.AccessedProcessModule;
        }

        protected virtual StartDisResult OnHandleStateCheckFullSiloProd(PAFDischarging discharging, PAProcessModule targetContainer)
        {
            Msg msg = null;
            // Falls Produktionsauftrag
            if (IsProduction)
            {
                var pwMethod = ParentPWMethod<PWMethodProduction>();
                if (    pwMethod.CurrentProdOrderPartslistPos == null
                     || pwMethod.CurrentProdOrderBatch == null
                     || !pwMethod.CurrentProdOrderBatch.ProdOrderBatchPlanID.HasValue)
                {
                    // Error50067:Batchplan not found
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSilo(1)", 1210, "Error50067");

                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        Messages.LogError(this.GetACUrl(), "OnHandleStateCheckFullSilo(1)", msg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    return StartDisResult.CycleWait;
                }

                using (var dbIPlus = new Database())
                using (var dbApp = new DatabaseApp(dbIPlus))
                {
                    ProdOrderPartslistPos currentBatchPos = pwMethod.CurrentProdOrderPartslistPos.FromAppContext<ProdOrderPartslistPos>(dbApp);
                    var batchPlan = dbApp.ProdOrderBatchPlan.Where(c => c.ProdOrderBatchPlanID == pwMethod.CurrentProdOrderBatch.ProdOrderBatchPlanID.Value).FirstOrDefault();
                    if (batchPlan == null || !batchPlan.VBiACClassWFID.HasValue)
                    {
                        // Error50068: Reference from Batchplan to Workflownode is null
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSilo(2)", 1220, "Error50068",
                                        batchPlan == null ? "" : batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                batchPlan == null ? "" : batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                batchPlan == null ? "" : batchPlan.ProdOrderPartslistPos.BookingMaterial.MaterialName1);

                        if (OnCheckFullSiloNoSiloFound(discharging, targetContainer, msg))
                        {
                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                Messages.LogError(this.GetACUrl(), "OnHandleStateCheckFullSilo(2)", msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        }
                        return StartDisResult.CycleWait;
                    }

                    if (CurrentDischargingDest(dbIPlus, false) == null)
                    {
                        // Error50072: CurrentDischargingDest() is null because no route couldn't be found at Order {0}, Bill of material {1}, Line {2}.
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSilo(3)", 1230, "Error50072",
                                        batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                batchPlan.ProdOrderPartslistPos.BookingMaterial.MaterialName1);

                        if (OnCheckFullSiloNoSiloFound(discharging, targetContainer, msg))
                        {
                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                Messages.LogError(this.GetACUrl(), "OnHandleStateCheckFullSilo(3)", msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        }
                        return StartDisResult.CycleWait;
                    }
                    targetContainer = TargetPAModule(null);
                    if (targetContainer == null)
                    {
                        // Error50073: targetSilo is null at Order {0}, Bill of material {1}, Line {2}.
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSilo(4)", 1240, "Error50073",
                                        batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                batchPlan.ProdOrderPartslistPos.BookingMaterial.MaterialName1);

                        if (OnCheckFullSiloNoSiloFound(discharging, targetContainer, msg))
                        {
                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                Messages.LogError(this.GetACUrl(), "OnHandleStateCheckFullSilo(4)", msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        }
                        return StartDisResult.CycleWait;
                    }

                    Route previousDischargingRoute = CurrentDischargingRoute;
                    IList<FacilityReservation> plannedSilos = batchPlan.FacilityReservation_ProdOrderBatchPlan.OrderBy(c => c.Sequence).ToArray();

                    if (plannedSilos == null || !plannedSilos.Any())
                    {
                        CheckIfAutomaticTargetChangePossible = false;
                        // Error50083: No destinations defined for Order {0}, Bill of material {1}, Line {2}
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSilo(5)", 1250, "Error50083",
                                        batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                batchPlan.ProdOrderPartslistPos.BookingMaterial.MaterialName1);

                        if (OnCheckFullSiloNoSiloFound(discharging, targetContainer, msg))
                        {
                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                Messages.LogError(this.GetACUrl(), "OnHandleStateCheckFullSilo(5)", msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        }
                        return StartDisResult.CycleWait;
                    }

                    // 1. Suche position von zur Zeit zu befüllendem Silo
                    FacilityReservation fullSiloReservation = null;

                    PAMSilo fullSilo = targetContainer as PAMSilo;
                    foreach (var activedSiloInPlan in plannedSilos.Where(c => c.ReservationState == GlobalApp.ReservationState.Active))
                    {
                        if (fullSilo != null && activedSiloInPlan.FacilityID == fullSilo.Facility.ValueT.ValueT.FacilityID)
                        {
                            fullSiloReservation = activedSiloInPlan;
                        }
                        // Fehler: Ein anderes Ziel wurde fälschlicherweise auch auf aktiv gesetzt:
                        else
                        {
                            activedSiloInPlan.ReservationState = GlobalApp.ReservationState.Finished;
                        }
                    }

                    if (fullSiloReservation == null)
                    {
                        CheckIfAutomaticTargetChangePossible = false;
                        // Error50084: Current active destination in batch planning {3} is not in sync with current discharging destination {4} at Order {0}, Bill of material {1}, Line {2}
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSilo(6)", 1260, "Error50084",
                                        batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                batchPlan.ProdOrderPartslistPos.BookingMaterial.MaterialName1,
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
                    else
                    {
                        fullSiloReservation.ReservationState = GlobalApp.ReservationState.Finished;
                    }

                    //batchPos = batchPlan.ProdOrderPartslistPos;
                    FacilityReservation plannedSilo = GetNextFreeDestination(plannedSilos, currentBatchPos, true, fullSiloReservation, discharging);
                    if (plannedSilo == null)
                    {
                        CheckIfAutomaticTargetChangePossible = false;
                        // Error50069: No further target-container/silo/tank found, which has enough remaining space for this batch, at Order {0}, Bill of material {1}, Line {2}
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSilo(7)", 1270, "Error50069",
                                        batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                batchPlan.ProdOrderPartslistPos.BookingMaterial.MaterialName1);

                        if (OnCheckFullSiloNoSiloFound(discharging, targetContainer, msg))
                        {
                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                Messages.LogError(this.GetACUrl(), "OnHandleStateCheckFullSilo(7)", msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        }
                        return StartDisResult.CycleWait;
                    }

                    gip.core.datamodel.ACClass acClassSilo = plannedSilo.Facility.GetFacilityACClass(Root.Database as Database);
                    if (acClassSilo == null)
                    {
                        // Error50070: acClassSilo is null at Order {0}, Bill of material {1}, Line {2}
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSilo(8)", 1280, "Error50070",
                                        batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                batchPlan.ProdOrderPartslistPos.BookingMaterial.MaterialName1);

                        if (OnCheckFullSiloNoSiloFound(discharging, targetContainer, msg))
                        {
                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                Messages.LogError(this.GetACUrl(), "OnHandleStateCheckFullSilo(8)", msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        }
                        return StartDisResult.CycleWait;
                    }

                    ACComponent targetSiloACComp = this.Root.ACUrlCommand(acClassSilo.GetACUrlComponent()) as ACComponent;
                    if (targetSiloACComp == null)
                    {
                        // Error50071: targetSiloACComp is null at Order {0}, Bill of material {1}, Line {2}
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSilo(9)", 1290, "Error50071",
                                        batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                batchPlan.ProdOrderPartslistPos.BookingMaterial.MaterialName1);

                        if (OnCheckFullSiloNoSiloFound(discharging, targetContainer, msg))
                        {
                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                Messages.LogError(this.GetACUrl(), "OnHandleStateCheckFullSilo(9)", msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        }
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
                        return StartDisResult.CycleWait;
                    }

                    CurrentDischargingRoute = null;
                    Type typeOfSilo = typeof(PAMSilo);
                    Guid thisMethodID = ContentACClassWF.ACClassMethodID;
                    DetermineDischargingRoute(Root.Database as Database, discharging.ParentACComponent as ACComponent, targetSiloACComp, 0,
                                            (c, p, r) => (c.ACKind == Global.ACKinds.TPAProcessModule
                                                    && (typeOfSilo.IsAssignableFrom(c.ObjectType)
                                                        || !c.BasedOnACClassID.HasValue
                                                        || (c.BasedOnACClassID.HasValue && c.ACClass1_BasedOnACClass.ACClassWF_RefPAACClass.Where(refc => refc.ACClassMethodID != thisMethodID).Any()))),
                                            PAMSilo.SelRuleID_Silo_Deselector, null);

                    if (CurrentDischargingDest(dbIPlus, false) == null)
                    {
                        CheckIfAutomaticTargetChangePossible = false;
                        // Error50072: CurrentDischargingDest() is null because no route couldn't be found at Order {0}, Bill of material {1}, Line {2}.
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSilo(10)", 1300, "Error50072",
                                        batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                batchPlan.ProdOrderPartslistPos.BookingMaterial.MaterialName1);

                        if (OnCheckFullSiloNoSiloFound(discharging, targetContainer, msg))
                        {
                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                Messages.LogError(this.GetACUrl(), "OnHandleStateCheckFullSilo(10)", msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        }
                        return StartDisResult.CycleWait;
                    }

                    targetContainer = TargetPAModule(null) as PAMSilo;
                    if (targetContainer == null)
                    {
                        // Error50073: targetSilo is null at Order {0}, Bill of material {1}, Line {2}.
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSilo(11)", 1310, "Error50073",
                                        batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo,
                                batchPlan.ProdOrderPartslistPos.BookingMaterial.MaterialName1);

                        if (OnCheckFullSiloNoSiloFound(discharging, targetContainer, msg))
                        {
                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                Messages.LogError(this.GetACUrl(), "OnHandleStateCheckFullSilo(11)", msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        }
                        return StartDisResult.CycleWait;
                    }

                    double? disChargedWeight = GetDischargedWeight(true);

                    bool isNewACMethod = false;
                    CheckIfAutomaticTargetChangePossible = null;
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
                    if (acMethod == null)
                    {
                        core.datamodel.ACClassMethod refPAACClassMethod = RefACClassMethodOfContentWF;
                        if (refPAACClassMethod == null)
                            return StartDisResult.CancelDischarging;
                        acMethod = refPAACClassMethod.TypeACSignature();
                        if (acMethod == null)
                        {
                            //Error50153: acMethod is null.
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSilo(11a)", 1320, "Error50153");
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                            return StartDisResult.CycleWait;
                        }

                        isNewACMethod = true;
                        if (!(bool)ExecuteMethod(nameof(GetConfigForACMethod), acMethod, true, dbApp, batchPlan, currentBatchPos, targetContainer))
                            return StartDisResult.CycleWait;
                    }
                    acMethod["Route"] = CurrentDischargingRoute != null ? CurrentDischargingRoute.Clone() as Route : null;
                    ACValue acValue = acMethod.ParameterValueList.GetACValue("Destination");
                    if (acValue != null)
                    {
                        if (acValue.ObjectType != null)
                            acValue.Value = Convert.ChangeType(targetContainer.RouteItemIDAsNum, acValue.ObjectType);
                        else
                            acMethod["Destination"] = targetContainer.RouteItemIDAsNum;
                    }

                    if ((ParentPWMethod<PWMethodProduction>() != null
                            && (((ACSubStateEnum)ParentPWMethod<PWMethodProduction>().CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrder)
                                || ((ACSubStateEnum)ParentPWMethod<PWMethodProduction>().CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode))
                            )
                        || (ParentPWGroup != null
                            && (((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrder)
                                || ((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode))
                            )
                        )
                    {
                        acValue = acMethod.ParameterValueList.GetACValue(PWMethodVBBase.IsLastBatchParamName);
                        if (acValue != null)
                            acValue.Value = (Int16)1;
                    }
                    else if (ParentPWMethod<PWMethodProduction>() != null)
                    {
                        acValue = acMethod.ParameterValueList.GetACValue(PWMethodVBBase.IsLastBatchParamName);
                        if (acValue != null && acValue.ParamAsInt16 <= (Int16)0)
                            acValue.Value = (Int16)ParentPWMethod<PWMethodProduction>().IsLastBatch;
                    }
                    if (acValue != null)
                        OnSetLastBatchParam(acValue, acMethod, targetContainer, dbApp, batchPlan, currentBatchPos);

                    if (CurrentDischargingRoute != null)
                        CurrentDischargingRoute.Detach(true);

                    acValue = acMethod.ParameterValueList.GetACValue("InterDischarging");
                    if (acValue != null
                        && ParentPWGroup != null
                        && (((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMInterDischarging)
                            || ((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMDisThenNextComp)))
                    {
                        acValue.Value = (Int16)1;
                    }

                    if (isNewACMethod)
                    {
                        if (!(bool)ExecuteMethod(nameof(AfterConfigForACMethodIsSet), acMethod, true, dbApp, batchPlan, currentBatchPos, targetContainer))
                            return StartDisResult.CycleWait;
                    }

                    OnSwitchingToNextSilo(dbApp, currentBatchPos, discharging, targetContainer, fullSilo, targetSiloACComp as PAMSilo, fullSiloReservation, plannedSilo);
                    // Sende neues Ziel an dies SPS
                    msg = OnReSendACMethod(discharging, acMethod, dbApp);
                    if (msg != null)
                    {
                        CurrentDischargingRoute = previousDischargingRoute;
                    }
                    else
                    {
                        if (disChargedWeight.HasValue)
                        {
                            DoInwardBooking(disChargedWeight.Value, dbApp, previousDischargingRoute.LastOrDefault(), fullSiloReservation.Facility, currentBatchPos, null, false);
                            RememberWeightOnRunDischarging(false);
                        }

                        plannedSilo.ReservationState = GlobalApp.ReservationState.Active;
                        // Falls Zielsilo nicht belegt
                        if (plannedSilo.Facility != null 
                            && plannedSilo.Facility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                        {
                            if (!plannedSilo.Facility.MaterialID.HasValue)
                                plannedSilo.Facility.Material = currentBatchPos.BookingMaterial?.Material1_ProductionMaterial != null ? currentBatchPos.BookingMaterial.Material1_ProductionMaterial : currentBatchPos.BookingMaterial;
                            if (!currentBatchPos.IsFinalMixureBatch
                                &&  ( !plannedSilo.Facility.PartslistID.HasValue
                                    || plannedSilo.Facility.PartslistID != currentBatchPos.ProdOrderPartslist.PartslistID))
                                plannedSilo.Facility.Partslist = currentBatchPos.ProdOrderPartslist.Partslist;
                        }
                        if (dbApp.IsChanged)
                            dbApp.ACSaveChanges();

                        NoTargetWait = null;
                        this.TaskSubscriptionPoint.Persist(false);
                        ExecuteMethod(nameof(OnACMethodSended), acMethod, false, dbApp, batchPlan, currentBatchPos, targetContainer, discharging);
                        if (discharging.CurrentACState == ACStateEnum.SMPaused)
                            discharging.Resume();
                        OnSwitchedToNextSilo(dbApp, currentBatchPos, discharging, targetContainer, fullSilo, targetSiloACComp as PAMSilo, fullSiloReservation, plannedSilo);
                    }
                }
            }
            return StartDisResult.WaitForCallback;
        }

        /// <summary>
        /// Method for informing derivations, that a new/alternative destination couldn't be found.
        /// The method sets the discharging-function to the pause-state.
        /// If the method returns false, then the passed message will not be added to the alarm list.
        /// </summary>
        /// <param name="discharging">Null, if called from StartDischargingProd()</param>
        /// <param name="targetContainer">Null, if called from StartDischargingProd()</param>
        /// <param name="msg"></param>
        /// <returns>true, if passed Message should be added to the alarm list</returns>
        protected virtual bool OnCheckFullSiloNoSiloFound(PAFDischarging discharging, PAProcessModule targetContainer, Msg msg)
        {
            if (discharging != null)
                discharging.Pause();
            return true;
        }

        protected virtual Msg OnPrepareSwitchToNextSilo(PAFDischarging discharging, PAProcessModule currentDestination, ACComponent nextDestination)
        {
            if (nextDestination != null)
            {
                PAMSilo silo = nextDestination as PAMSilo;
                if (silo != null && silo.IsSiloFullFromSensorStates)
                {
                    // Error50206: Switching to Destination {0} not possible because Full-Sensors are active.
                    return new Msg(this, eMsgLevel.Error, PWClassName, "OnPrepareSwitchToNextSilo(9)", 1290, "Error50206", silo.ACIdentifier);
                }
            }
            return null;
        }

        protected virtual void OnSwitchedToNextSilo(DatabaseApp dbApp, ProdOrderPartslistPos currentBatchPos, PAFDischarging discharging, PAProcessModule targetContainer, 
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

        protected virtual void OnSwitchingToNextSilo(DatabaseApp dbApp, ProdOrderPartslistPos currentBatchPos, PAFDischarging discharging, PAProcessModule targetContainer,
                                            PAMSilo fullSilo, PAMSilo nextSilo,
                                            FacilityReservation fullSiloReservation, FacilityReservation nextSiloReservation)
        {
        }

        #endregion

        #region Booking

        /// <summary>
        /// If false is returned, then use actualQuantity, if true returned then CalcProducedBatchWeight() must be used
        /// </summary>
        /// <param name="eM"></param>
        /// <param name="taskEntry"></param>
        /// <param name="acMethod"></param>
        /// <param name="dbApp"></param>
        /// <param name="dbIPlus"></param>
        /// <param name="currentBatchPos"></param>
        /// <param name="actualWeight"></param>
        /// <returns></returns>
        protected virtual void OnTaskCallbackCheckQuantity(ACMethodEventArgs eM, IACTask taskEntry, ACMethod acMethod, DatabaseApp dbApp, Database dbIPlus, ProdOrderPartslistPos currentBatchPos, ref double actualWeight)
        {
            // Wenn kein Istwert von der Funktion zurückgekommen, dann berechne Zugangsmenge über die Summe der dosierten Mengen
            // Minus der bereits zugebuchten Menge (falls zyklische Zugagnsbuchungen im Hintergrund erfolgten)
            if (    actualWeight <= 0.000001
                && (   eM == null
                    || eM.ResultState < Global.ACMethodResultState.Failed))
            {
                try
                {
                    ACProdOrderManager prodOrderManager = ACProdOrderManager.GetServiceInstance(this);
                    if (prodOrderManager != null)
                    {
                        double calculatedBatchWeight = 0;
                        if (prodOrderManager.CalcProducedBatchWeight(dbApp, currentBatchPos, LossCorrectionFactor, out calculatedBatchWeight) == null)
                        {
                            double diff = calculatedBatchWeight - currentBatchPos.ActualWeight;
                            if (diff > 0.00001)
                                actualWeight = diff;
                        }
                    }
                }
                catch(Exception ex) 
                {
                    Messages.LogException(this.GetACUrl(), "OnTaskCallbackCheckQuantity(20)", ex);
                    //ProcessAlarm.ValueT = PANotifyState.AlarmOrFault;
                    //OnNewAlarmOccurred(ProcessAlarm, new Msg(ex.Message, this, eMsgLevel.Error, PWClassName, "OnTaskCallbackCheckQuantity", 1030), true);
                    //discharging.FunctionError.ValueT = PANotifyState.AlarmOrFault;
                    //discharging.OnNewAlarmOccurred(discharging.FunctionError, new Msg(ex.Message, discharging, eMsgLevel.Error, nameof(PAFDischarging), "TaskCallback", 1020), true);
                    //exceptionHandled = true;
                }
            }

            if ((this.IsSimulationOn/* || simulationWeight == 1*/)
                && actualWeight <= 0.000001
                && currentBatchPos != null)
            {
                actualWeight = currentBatchPos.TargetWeight;
            }
            // Entleerschritt liefert keine Menge
            else if (actualWeight <= 0.000001)
            {
                actualWeight = -0.001;
            }
        }

        public virtual bool CanExecutePosting(ACMethodBooking bookingParam, ProdOrderPartslistPos currentBatchPos)
        {
            return true;
        }

        public virtual Msg DoInwardBooking(double actualWeight, DatabaseApp dbApp, RouteItem dischargingDest, Facility inwardFacility, ProdOrderPartslistPos currentBatchPos, ACEventArgs e, bool isDischargingEnd, bool blockQuant = false)
        {
            MsgWithDetails collectedMessages = new MsgWithDetails();
            Msg msg = null;
            if (dischargingDest == null && inwardFacility == null)
            {
                // Error50075: No downward-route defined for module {0}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoInwardBooking(1)", 1330, "Error50075",
                                ParentPWGroup != null ? ParentPWGroup.AccessedProcessModule.GetACUrl() : Root.Environment.TranslateMessage(this, "Error50158"));

                Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return msg;
            }
            else
            {
                if (dischargingDest != null && !dischargingDest.IsAttached)
                    dischargingDest.AttachTo(dbApp);
                if (currentBatchPos == null)
                    return new Msg() { Source = this.GetACUrl(), ACIdentifier = "DoInwardBooking(2)", Message = "CurrentBatchPos is null", MessageLevel = eMsgLevel.Exception };
                if (inwardFacility == null)
                    inwardFacility = dbApp.Facility.Where(c => c.VBiFacilityACClassID == dischargingDest.Target.ACClassID).FirstOrDefault();
                if (inwardFacility == null) // Entleerung erfolgte auf ProcessModule und kein Silo
                    return null;
                // Falls keine Materialbuchung erfolgen soll
                if (currentBatchPos.BookingMaterial.MDFacilityManagementType != null && currentBatchPos.BookingMaterial.MDFacilityManagementType.FacilityManagementType == MDFacilityManagementType.FacilityManagementTypes.NoFacility)
                    return null;

                MDProdOrderPartslistPosState posState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed).FirstOrDefault();
                if (posState == null)
                {
                    // Error50078: MDProdOrderPartslistPosState for Completed-State doesn't exist
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoInwardBooking(4)", 1340, "Error50078");

                    Messages.LogError(this.GetACUrl(), "DoInwardBooking(4)", msg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    return msg;
                }
                if (blockQuant)
                {
                    posState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Blocked).FirstOrDefault();
                    if (posState == null)
                    {
                        // Error50079: MDProdOrderPartslistPosState for Blocked-State doesn't exist
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoInwardBooking(5)", 1350, "Error50079");

                        Messages.LogError(this.GetACUrl(), "DoInwardBooking(5)", msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        return msg;
                    }
                }

                if (isDischargingEnd)
                    currentBatchPos.MDProdOrderPartslistPosState = posState;

                ProdOrderPartslistPos rootPartslistPos = currentBatchPos.TopParentPartslistPos;
                if (rootPartslistPos != null && isDischargingEnd)
                {
                    if (rootPartslistPos.MDProdOrderPartslistPosState.ProdOrderPartslistPosState == MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed)
                    {
                        rootPartslistPos.ProdOrderPartslist.MDProdOrderState = DatabaseApp.s_cQry_GetMDProdOrderState(dbApp, MDProdOrderState.ProdOrderStates.ProdFinished).FirstOrDefault();
                        dbApp.ACSaveChanges();
                    }
                }

                // Falls dosiert
                if (actualWeight > 0.00001 && IsProduction)
                {
                    // 1. Bereite Buchung vor
                    FacilityLot facilityLot = null;
                    if (currentBatchPos.BookingMaterial.IsLotManaged)
                    {
                        facilityLot = currentBatchPos.FacilityLot;
                        if (facilityLot == null)
                        {
                            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(FacilityLot), FacilityLot.NoColumnName, FacilityLot.FormatNewNo, this);
                            facilityLot = FacilityLot.NewACObject(dbApp, null, secondaryKey);
                            currentBatchPos.FacilityLot = facilityLot;
                        }
                        facilityLot.UpdateExpirationInfo(currentBatchPos.BookingMaterial);
                    }

                    FacilityPreBooking facilityPreBooking = ParentPWMethod<PWMethodProduction>().ProdOrderManager.NewInwardFacilityPreBooking(ParentPWMethod<PWMethodProduction>().ACFacilityManager, dbApp, currentBatchPos);
                    ACMethodBooking bookingParam = facilityPreBooking.ACMethodBooking as ACMethodBooking;
                    try
                    {
                        bookingParam.InwardQuantity = currentBatchPos.BookingMaterial.ConvertBaseWeightToBaseUnit(actualWeight);
                    }
                    catch (ArgumentException) 
                    {
                        if (currentBatchPos.BookingMaterial.BaseMDUnit.SIDimension != GlobalApp.SIDimensions.Mass)
                            bookingParam.InwardQuantity = actualWeight;
                    }
                    bookingParam.InwardFacility = inwardFacility;
                    bookingParam.InwardFacilityLot = facilityLot;
                    bool isFinalMixture = currentBatchPos.IsFinalMixureBatch;
                    if (!isFinalMixture)
                        bookingParam.InwardPartslist = currentBatchPos.ProdOrderPartslist.Partslist;
                    if (ParentPWGroup != null && ParentPWGroup.AccessedProcessModule != null)
                        bookingParam.PropertyACUrl = ParentPWGroup.AccessedProcessModule.GetACUrl();
                    if (PostingBehaviour != PostingBehaviourEnum.NotSet)
                        bookingParam.PostingBehaviour = PostingBehaviour;
                    else if (isFinalMixture && currentBatchPos.ProdOrderPartslist.ProdOrderPartslistPos_SourceProdOrderPartslist.Any())
                        bookingParam.PostingBehaviour = PostingBehaviourEnum.DoNothing;
                    OnPrepareInwardBooking(actualWeight, dbApp, dischargingDest, currentBatchPos, e, isDischargingEnd, blockQuant, facilityPreBooking, bookingParam);
                    msg = dbApp.ACSaveChangesWithRetry();

                    // 2. Führe Buchung durch
                    if (msg != null)
                    {
                        collectedMessages.AddDetailMessage(msg);
                        OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoInwardBooking", 1360), true);
                        Messages.LogError(this.GetACUrl(), "DoInwardBooking(6)", msg.InnerMessage);
                        return msg;
                    }
                    else if (facilityPreBooking != null)
                    {
                        bookingParam.IgnoreIsEnabled = true;
                        ACMethodEventArgs resultBooking = null;
                        bool canExecutePosting = CanExecutePosting(bookingParam, currentBatchPos);
                        if (canExecutePosting)
                            resultBooking = ACFacilityManager.BookFacilityWithRetry(ref bookingParam, dbApp);
                        if (resultBooking != null && (resultBooking.ResultState == Global.ACMethodResultState.Failed || resultBooking.ResultState == Global.ACMethodResultState.Notpossible))
                        {
                            collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                            OnNewAlarmOccurred(ProcessAlarm, new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoInwardBooking", 1380), true);
                        }
                        else
                        {
                            if (resultBooking != null && (!bookingParam.ValidMessage.IsSucceded() || bookingParam.ValidMessage.HasWarnings()))
                            {
                                OnNewAlarmOccurred(ProcessAlarm, new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoInwardBooking", 1370), true);
                                Messages.LogError(this.GetACUrl(), "DoInwardBooking(7)", bookingParam.ValidMessage.InnerMessage);
                            }

                            if (  (resultBooking == null && !canExecutePosting)
                                || bookingParam.ValidMessage.IsSucceded())
                            {
                                if (canExecutePosting)
                                {
                                    facilityPreBooking.DeleteACObject(dbApp, true);
                                    currentBatchPos.IncreaseActualQuantityUOM(bookingParam.InwardQuantity.Value);
                                    OnDoInwardBookingSucceeded(actualWeight, dbApp, dischargingDest, currentBatchPos, e, isDischargingEnd, blockQuant, facilityPreBooking, bookingParam);

                                    //batchPos.RecalcActualQuantity();
                                    //batchPos.TopParentPartslistPos.RecalcActualQuantity();
                                    // Aktualisiere Abrufemenge mit Istmenge, weil sonst ein erneuter Start nicht mehr möglich ist:
                                    //batchPos.TopParentPartslistPos.CalledUpQuantity = batchPos.TopParentPartslistPos.ActualQuantity;
                                }
                            }
                            else
                            {
                                collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                            }
                        }
                        msg = dbApp.ACSaveChangesWithRetry();
                        if (msg != null)
                        {
                            collectedMessages.AddDetailMessage(msg);
                            OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoInwardBooking", 1390), true);
                            Messages.LogError(this.GetACUrl(), "DoInwardBooking(9)", msg.InnerMessage);
                            return msg;
                        }
                        else
                        {
                            currentBatchPos.RecalcActualQuantityFast();
                            if (dbApp.IsChanged)
                                dbApp.ACSaveChanges();
                        }
                    }
                }
                // Sonst 
                else
                {
                    msg = dbApp.ACSaveChangesWithRetry();
                    if (msg != null)
                    {
                        collectedMessages.AddDetailMessage(msg);
                        OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoInwardBooking", 1400), true);
                        Messages.LogError(this.GetACUrl(), "DoInwardBooking(10)", msg.InnerMessage);
                    }
                }
            }
            return collectedMessages.MsgDetailsCount > 0 ? collectedMessages : null;
        }

        protected virtual void OnPrepareInwardBooking(double actualWeight, DatabaseApp dbApp, RouteItem dischargingDest,
            ProdOrderPartslistPos currentBatchPos, ACEventArgs e, bool isDischargingEnd, bool blockQuant,
            FacilityPreBooking facilityPreBooking, ACMethodBooking bookingParam)
        {
        }

        protected virtual void OnDoInwardBookingSucceeded(double actualWeight, DatabaseApp dbApp, RouteItem dischargingDest, 
            ProdOrderPartslistPos currentBatchPos, ACEventArgs e, bool isDischargingEnd, bool blockQuant,
            FacilityPreBooking facilityPreBooking, ACMethodBooking bookingParam)
        {
        }
        #endregion

        #region Misc
        public virtual bool IsLastDisNode(DatabaseApp dbApp, ProdOrderBatchPlan batchPlan, out MaterialWFConnection connectionToDischarging)
        {
            if (batchPlan == null || dbApp == null)
            {
                connectionToDischarging = null;
                return false;
            }
            if (batchPlan.MaterialWFACClassMethodID.HasValue)
            {
                connectionToDischarging = dbApp.MaterialWFConnection
                                        .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == batchPlan.MaterialWFACClassMethodID.Value
                                                && c.MaterialID == batchPlan.ProdOrderPartslistPos.MaterialID
                                                && c.ACClassWFID == ContentACClassWF.ACClassWFID)
                                        .FirstOrDefault();
            }
            else
            {
                PartslistACClassMethod plMethod = batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
                if (plMethod != null)
                {
                    connectionToDischarging = dbApp.MaterialWFConnection
                                            .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == plMethod.MaterialWFACClassMethodID
                                                    && c.MaterialID == batchPlan.ProdOrderPartslistPos.MaterialID
                                                    && c.ACClassWFID == ContentACClassWF.ACClassWFID)
                                            .FirstOrDefault();
                }
                else
                {
                    connectionToDischarging = dbApp.MaterialWFConnection
                                            .Where(c => c.MaterialWFACClassMethod.MaterialWFID == batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.MaterialWFID
                                                    && c.MaterialID == batchPlan.ProdOrderPartslistPos.MaterialID
                                                    && c.ACClassWFID == ContentACClassWF.ACClassWFID)
                                            .FirstOrDefault();
                }
            }
            return connectionToDischarging != null;
        }

        public virtual bool GetMaterialWFConnection(DatabaseApp dbApp, ProdOrderBatchPlan batchPlan, string materialNo, out MaterialWFConnection connectionToDischarging)
        {
            if (batchPlan == null || dbApp == null || String.IsNullOrEmpty(materialNo))
            {
                connectionToDischarging = null;
                return false;
            }
            if (batchPlan.MaterialWFACClassMethodID.HasValue)
            {
                connectionToDischarging = dbApp.MaterialWFConnection
                                        .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == batchPlan.MaterialWFACClassMethodID.Value
                                                && c.Material.MaterialNo == materialNo
                                                && c.ACClassWFID == ContentACClassWF.ACClassWFID)
                                        .FirstOrDefault();
            }
            else
            {
                PartslistACClassMethod plMethod = batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
                if (plMethod != null)
                {
                    connectionToDischarging = dbApp.MaterialWFConnection
                                            .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == plMethod.MaterialWFACClassMethodID
                                                    && c.Material.MaterialNo == materialNo
                                                    && c.ACClassWFID == ContentACClassWF.ACClassWFID)
                                            .FirstOrDefault();
                }
                else
                {
                    connectionToDischarging = dbApp.MaterialWFConnection
                                            .Where(c => c.MaterialWFACClassMethod.MaterialWFID == batchPlan.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.MaterialWFID
                                                    && c.Material.MaterialNo == materialNo
                                                    && c.ACClassWFID == ContentACClassWF.ACClassWFID)
                                            .FirstOrDefault();
                }
            }
            return connectionToDischarging != null;
        }


        protected virtual void OnSetLastBatchParam(ACValue lastBatchParam, ACMethod acMethod, PAProcessModule targetModule, DatabaseApp dbApp, ProdOrderBatchPlan batchPlan, ProdOrderPartslistPos currentBatchPos)
        {
        }

        protected virtual Msg OnReSendACMethod(PAProcessFunction function, ACMethod acMethod, DatabaseApp dbApp)
        {
            return function.ReSendACMethod(acMethod);
        }
        #endregion


        #endregion
    }
}
