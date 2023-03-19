using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System.Threading;

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

            if (!pickingPos.ToFacilityID.HasValue)
            {
                // Error50085: No destination defined in commissioning line at commssion {0}!
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingPicking(20)", 1020, "Error50085", picking.PickingNo);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            if (!pickingPos.ToFacility.VBiFacilityACClassID.HasValue)
            {
                // Error50086: Foreign Key to ACComponent for Facility {0} not defined!
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingPicking(30)", 1030, "Error50086", pickingPos.ToFacility.FacilityNo);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            if (pickingPos.ToFacility.Material != null && pickingPos.Material != pickingPos.ToFacility.Material)
            {
                // Error50087: Material {0} on Silo {1} doesn't match Material {2} at Commssioningorder {3}!
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingPicking(40)", 1040, "Error50087",
                              pickingPos.ToFacility.Material.MaterialName1, pickingPos.ToFacility.FacilityNo, pickingPos.ToFacility.Material.MaterialName1, picking.PickingNo);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            gip.core.datamodel.ACClass acClassSilo = pickingPos.ToFacility.GetFacilityACClass(db);
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
            PAProcessModule targetModule = null;
            bool isLastDischarging = true;
            // Falls Workflow mehrere Gruppe hat, prüfe zuerst ob vom akteullen Prozessmodul direkt in das Ziel enteleert werden kann oder über ein weiteres Prozessmodul geschleust werden muss
            DetermineDischargingRoute(Root.Database as Database, module, targetSiloACComp, 0,
                                    (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule,
                                    PAProcessModule.SelRuleID_ProcessModule_Deselector, null);
            // Falls kein direkter Weg gefunden, prüfe über welche gemappte PWGroup weiter transportiert werden kann 
            if (CurrentDischargingDest(null) == null)
            {
                isLastDischarging = false;

                RoutingResult rResult = ACRoutingService.FindSuccessors(RoutingService, db, RoutingService != null && RoutingService.IsProxy,
                                    ParentPWGroup.AccessedProcessModule, PAProcessModule.SelRuleID_ProcessModule, RouteDirections.Forwards, new object[] { },
                                    (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule,
                                    null,
                                    0, true, true, false, false);
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

            if (CurrentDischargingDest(null) == null)
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
                && pickingPos.ToFacility.Material == null 
                && pickingPos.ToFacility.MDFacilityType != null
                && pickingPos.ToFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
            {
                pickingPos.ToFacility.Material = pickingPos.Material;
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

            NoTargetWait = null;
            if (!(bool)ExecuteMethod("AfterConfigForACMethodIsSet", acMethod, true, dbApp, pickingPos, targetModule))
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
            return task.State == PointProcessingState.Deleted ? StartDisResult.CancelDischarging : StartDisResult.WaitForCallback;
            //return StartDisResult.WaitForCallback;
        }


        protected virtual StartDisResult OnHandleStateCheckFullSiloPicking(PAFDischarging discharging, PAProcessModule targetContainer, PAProcessModule module, ACMethod acMethodRoot, Database db, DatabaseApp dbApp, Picking picking, PickingPos pickingPosPrev)
        {
            Msg msg = null;
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

            PickingPos pickingPos = null;
            if (pickingPosPrev == null)
            {
                if (!picking.PickingPos_Picking.Where(c => c.InOrderPosID.HasValue || c.OutOrderPosID.HasValue || c.PickingPosProdOrderPartslistPos_PickingPos.Any()).Any())
                {
                    var posList = picking.PickingPos_Picking.ToArray();
                    pickingPosPrev = posList
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
            if (pickingPosPrev != null)
                pickingPosPrev.MDDelivPosLoadState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();

            if (!picking.PickingPos_Picking.Where(c => c.InOrderPosID.HasValue || c.OutOrderPosID.HasValue || c.PickingPosProdOrderPartslistPos_PickingPos.Any()).Any())
            {
                var posList = picking.PickingPos_Picking.ToArray();
                pickingPos = posList
                    .Where(c => !c.MDDelivPosLoadStateID.HasValue || c.MDDelivPosLoadState.DelivPosLoadState == MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad)
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

            if (pickingPos == null)
            {
                // Error50088: No dischargable line at commissioning order {0} found.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloPicking(10)", 1240, "Error50088", picking.PickingNo);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);

                return StartDisResult.CycleWait;
            }

            if (!pickingPos.ToFacilityID.HasValue)
            {
                // Error50085: No destination defined in commissioning line at commssion {0}!
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloPicking(20)", 1250, "Error50085", picking.PickingNo);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            if (!pickingPos.ToFacility.VBiFacilityACClassID.HasValue)
            {
                // Error50086: Foreign Key to ACComponent for Facility {0} not defined!
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloPicking(30)", 1260, "Error50086", 
                                pickingPos.ToFacility.FacilityNo, pickingPos.ToFacility.FacilityNo);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            if (pickingPos.ToFacility.Material != null && pickingPos.Material != pickingPos.ToFacility.Material)
            {
                // Error50087: Material {0} on Silo {1} doesn't match Material {2} at Commssioningorder {3}!
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloPicking(40)", 1270, "Error50087",
                              pickingPos.ToFacility.Material.MaterialName1, pickingPos.ToFacility.FacilityNo, pickingPos.ToFacility.Material.MaterialName1, picking.PickingNo);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            Route previousDischargingRoute = CurrentDischargingRoute;

            gip.core.datamodel.ACClass acClassSilo = pickingPos.ToFacility.GetFacilityACClass(db);
            if (acClassSilo == null)
            {
                // Error50070: acClassSilo is null at Order {0}, Bill of material {1}, Line {2}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloPicking(50)", 1280, "Error50070",
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
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloPicking(60)", 1290, "Error50071",
                                picking.PickingNo, pickingPos.LineNumber, pickingPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            Type typeOfSilo = typeof(PAMSilo);
            Guid thisMethodID = ContentACClassWF.ACClassMethodID;
            PAProcessModule targetModule = null;
            bool isLastDischarging = true;
            // Falls Workflow mehrere Gruppe hat, prüfe zuerst ob vom akteullen Prozessmodul direkt in das Ziel enteleert werden kann oder über ein weiteres Prozessmodul geschleust werden muss
            DetermineDischargingRoute(Root.Database as Database, module, targetSiloACComp, 0,
                                        (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule,
                                        PAProcessModule.SelRuleID_ProcessModule_Deselector, null);
            // Falls kein direkter Weg gefunden, prüfe über welche gemappte PWGroup weiter transportiert werden kann 
            if (CurrentDischargingDest(null) != null)
            {
                isLastDischarging = false;
                RoutingResult rResult = ACRoutingService.FindSuccessors(RoutingService, db, RoutingService != null && RoutingService.IsProxy,
                                    ParentPWGroup.AccessedProcessModule, PAProcessModule.SelRuleID_ProcessModule, RouteDirections.Forwards, new object[] { },
                                    (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule,
                                    null,
                                    0, true, true, false, false);
                if (rResult.Routes != null && rResult.Routes.Any())
                {
                    // Falls keine Sonderentleerung sondern regulärer weitertransport zu einem Prozessmodul
                    var cacheModuleDestinations = rResult.Routes.Select(c => c.LastOrDefault().Target.ACClassID).ToArray();
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

            if (CurrentDischargingDest(null) == null)
            {
                // Error50072: CurrentDischargingDest() is null because no route couldn't be found at Order {0}, Bill of material {1}, Line {2}.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloPicking(70)", 528, "Error50072",
                                picking.PickingNo, pickingPos.LineNumber, pickingPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
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
                return StartDisResult.CycleWait;
            }

            if (isLastDischarging 
                && pickingPos.ToFacility.Material == null
                && pickingPos.ToFacility.MDFacilityType != null
                && pickingPos.ToFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
            {
                pickingPos.ToFacility.Material = pickingPos.Material;
                dbApp.ACSaveChanges();
            }


            core.datamodel.ACClassMethod refPAACClassMethod = RefACClassMethodOfContentWF;
            if (refPAACClassMethod == null)
                return StartDisResult.CancelDischarging;

            ACMethod acMethod = refPAACClassMethod.TypeACSignature();
            if (acMethod == null)
            {
                //Error50153: acMethod is null.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloPicking(81)", 1320, "Error50153");

                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            if (!(bool)ExecuteMethod(nameof(GetConfigForACMethod), acMethod, true, dbApp, pickingPos, targetModule))
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

            if (!(bool)ExecuteMethod("AfterConfigForACMethodIsSet", acMethod, true, dbApp, pickingPos, targetModule))
                return StartDisResult.CycleWait;

            if (CurrentDischargingRoute != null)
                CurrentDischargingRoute.Detach(true);
            // Sende neues Ziel an dies SPS
            msg = OnReSendACMethod(discharging, acMethod, dbApp);
            if (msg != null)
            {
                CurrentDischargingRoute = previousDischargingRoute;
            }
            else
            {
                if (pickingPos.MDDelivPosLoadState == null || pickingPos.MDDelivPosLoadState.DelivPosLoadState == MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad)
                    pickingPos.MDDelivPosLoadState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadingActive).FirstOrDefault();
                // Falls Zielsilo nicht belegt
                if (db.IsChanged)
                {
                    if (isLastDischarging 
                        && pickingPos.ToFacility.Material == null
                        && pickingPos.ToFacility.MDFacilityType != null
                        && pickingPos.ToFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                        pickingPos.ToFacility.Material = pickingPos.Material;
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
            }
            return StartDisResult.WaitForCallback;
        }

        #endregion

        #region Booking
        public virtual Msg DoInwardBooking(double actualWeight, DatabaseApp dbApp, RouteItem dischargingDest, Picking picking, PickingPos pickingPos, ACEventArgs e, bool isDischargingEnd)
        {
            MsgWithDetails collectedMessages = new MsgWithDetails();
            Msg msg = null;
            PWMethodTransportBase pwMethodTransport = ParentPWMethod<PWMethodTransportBase>();
            if (actualWeight > 0.000001 
                && pickingPos != null
                && pwMethodTransport != null && !pwMethodTransport.FindChildComponents<PWDosing>(c => c is PWDosing).Any() 
                && ACFacilityManager != null && PickingManager != null
                && !NoPostingOnRelocation)
            {
                if (pickingPos.Material == null)
                {
                    pickingPos.MDDelivPosLoadState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();
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
                    // 1. Bereite Buchung vor
                    FacilityPreBooking facilityPreBooking = ACFacilityManager.NewFacilityPreBooking(dbApp, pickingPos, pickingPos.Material.ConvertBaseWeightToBaseUnit(actualWeight));
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
                        ACMethodEventArgs resultBooking = ACFacilityManager.BookFacilityWithRetry(ref bookingParam, dbApp);
                        if (resultBooking.ResultState == Global.ACMethodResultState.Failed || resultBooking.ResultState == Global.ACMethodResultState.Notpossible)
                        {
                            OnNewAlarmOccurred(ProcessAlarm, new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoInwardBooking", 1350), true);
                        }
                        else
                        {
                            if (!bookingParam.ValidMessage.IsSucceded() || bookingParam.ValidMessage.HasWarnings())
                            {
                                Messages.LogError(this.GetACUrl(), "DoInwardBooking(6)", bookingParam.ValidMessage.InnerMessage);
                                OnNewAlarmOccurred(ProcessAlarm, new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoInwardBooking", 1340), true);
                            }
                            if (bookingParam.ValidMessage.IsSucceded())
                            {
                                facilityPreBooking.DeleteACObject(dbApp, true);
                                ACFacilityManager.RecalcAfterPosting(dbApp, pickingPos, bookingParam.OutwardQuantity.Value, false);
                                //pickingPos.RecalcAfterPosting(dbApp, bookingParam.OutwardQuantity.Value, false);
                                //pickingPos.IncreasePickingActualUOM(bookingParam.OutwardQuantity.Value);
                                //dosingPosRelation.TopParentPartslistPosRelation.RecalcActualQuantity();
                                //dosingPosRelation.SourceProdOrderPartslistPos.TopParentPartslistPos.RecalcActualQuantity();
                            }
                            else
                            {
                                collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                            }

                            if (pickingPos.RemainingDosingQuantityUOM >= -1)
                                pickingPos.MDDelivPosLoadState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();

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
