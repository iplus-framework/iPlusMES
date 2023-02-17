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

        #region Methods

        public static IACObjectEntity GetTransportEntityFromACMethod(DatabaseApp dbApp, ACMethod acMethod)
        {
            Guid pickingID = GetPickingIDFromACMethod(acMethod);
            Guid deliveryNotePosID = Guid.Empty;
            Guid facilityBookingID = Guid.Empty;
            if (pickingID == Guid.Empty)
                deliveryNotePosID = GetDeliveryNotePosIDFromACMethod(acMethod);
            if (pickingID == Guid.Empty && deliveryNotePosID == Guid.Empty)
                facilityBookingID = GetFacilityBookingIDFromACMethod(acMethod);
            if (pickingID != Guid.Empty)
                return dbApp.Picking.Where(c => c.PickingID == pickingID).FirstOrDefault();
            else if (deliveryNotePosID != Guid.Empty)
                return dbApp.DeliveryNotePos.Where(c => c.DeliveryNotePosID == deliveryNotePosID).FirstOrDefault();
            else if (facilityBookingID != Guid.Empty)
                return dbApp.FacilityBooking.Where(c => c.FacilityBookingID == facilityBookingID).FirstOrDefault();
            return null;
        }

        public static Guid GetDeliveryNotePosIDFromACMethod(ACMethod acMethod)
        {
            Guid deliveryNotePosID = Guid.Empty;
            ACValue acValue = acMethod.ParameterValueList.GetACValue(DeliveryNotePos.ClassName);
            if (acValue != null && acValue.Value != null && acValue.Value is Guid)
                deliveryNotePosID = (Guid)acValue.Value;
            return deliveryNotePosID;
        }


        public virtual FacilityReservation GetNextFreeDestination(IList<FacilityReservation> plannedSilos, DeliveryNotePos dnPos, double targetQuantity)
        {
            if (plannedSilos == null || !plannedSilos.Any())
                return null;

            // 1. Prüfe zuerst ob das geplante Silo, das zur Zeit als aktive Befüllung gesetzt worden ist, befüllt werden kann
            Facility destinationSilo = null;
            foreach (FacilityReservation plannedSilo in plannedSilos.Where(c => c.ReservationState == GlobalApp.ReservationState.Active))
            {
                if (CheckPlannedDestinationSilo(plannedSilo, dnPos, targetQuantity))
                    return plannedSilo;
            }

            // 2. Die aktiven Silos können nicht mehr befüllt werden => suche ein anderes das neu geplant ist
            if (destinationSilo == null)
            {
                foreach (FacilityReservation plannedSilo in plannedSilos.Where(c => c.ReservationState == GlobalApp.ReservationState.New))
                {
                    if (CheckPlannedDestinationSilo(plannedSilo, dnPos, targetQuantity))
                    {
                        plannedSilo.ReservationState = GlobalApp.ReservationState.Active;
                        return plannedSilo;
                    }
                }
            }

            // 3. Die neu geplanten Silo's können auch nicht mehr befüllt werden => suche ob ein altes reaktivert werden kann
            //if (destinationSilo == null
            //    && !plannedSilos.Where(c => c.ReservationState == GlobalApp.ReservationState.New).Any())
            //{
            //    foreach (FacilityReservation plannedSilo in plannedSilos.Where(c => c.ReservationState == GlobalApp.ReservationState.Finished))
            //    {
            //        if (CheckPlannedDestinationSilo(plannedSilo, dnPos, targetQuantity))
            //        {
            //            plannedSilo.ReservationState = GlobalApp.ReservationState.Active;
            //            return plannedSilo;
            //        }
            //    }
            //}
            return null;
        }

        public static FacilityReservation GetNextFreeDestination(ACComponent invoker, IList<FacilityReservation> plannedSilos, DeliveryNotePos pPos, double targetQuantity)
        {
            if (plannedSilos == null || !plannedSilos.Any())
                return null;
            foreach (FacilityReservation plannedSilo in plannedSilos.Where(c => c.ReservationState == GlobalApp.ReservationState.Active))
            {
                if (CheckPlannedDestinationSilo(invoker, plannedSilo, pPos, targetQuantity))
                    return plannedSilo;
            }
            foreach (FacilityReservation plannedSilo in plannedSilos.Where(c => c.ReservationState == GlobalApp.ReservationState.New))
            {
                if (CheckPlannedDestinationSilo(invoker, plannedSilo, pPos, targetQuantity))
                {
                    plannedSilo.ReservationState = GlobalApp.ReservationState.Active;
                    return plannedSilo;
                }
            }
            foreach (FacilityReservation plannedSilo in plannedSilos.Where(c => c.ReservationState == GlobalApp.ReservationState.Finished))
            {
                if (CheckPlannedDestinationSilo(invoker, plannedSilo, pPos, targetQuantity))
                {
                    plannedSilo.ReservationState = GlobalApp.ReservationState.Active;
                    return plannedSilo;
                }
            }
            return null;
        }


        protected bool CheckPlannedDestinationSilo(FacilityReservation plannedSilo, DeliveryNotePos dnPos, double targetQuantity)
        {
            if (plannedSilo != null
                && plannedSilo.Facility != null
                && plannedSilo.Facility.InwardEnabled
                && (!plannedSilo.Facility.MaterialID.HasValue
                    || ((dnPos.InOrderPos.Material.ProductionMaterialID.HasValue && plannedSilo.Facility.MaterialID == dnPos.InOrderPos.Material.ProductionMaterialID)
                        || (!dnPos.InOrderPos.Material.ProductionMaterialID.HasValue && plannedSilo.Facility.MaterialID == dnPos.InOrderPos.Material.MaterialID))
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
        }

        public static bool CheckPlannedDestinationSilo(ACComponent invoker, FacilityReservation plannedSilo, DeliveryNotePos dnPos, double targetQuantity)
        {
            //Facility destinationSilo = null;
            if (plannedSilo != null
                && plannedSilo.Facility != null
                && plannedSilo.Facility.InwardEnabled
                && (!plannedSilo.Facility.MaterialID.HasValue
                    || ((dnPos.InOrderPos.Material.ProductionMaterialID.HasValue && plannedSilo.Facility.MaterialID == dnPos.InOrderPos.Material.ProductionMaterialID)
                        || (!dnPos.InOrderPos.Material.ProductionMaterialID.HasValue && plannedSilo.Facility.MaterialID == dnPos.InOrderPos.Material.MaterialID))
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

        #region ACState

        protected virtual StartDisResult StartDischargingIntake(PAProcessModule module)
        {
            if (!IsIntake)
                return StartDisResult.CancelDischarging;
            var pwMethod = ParentPWMethod<PWMethodIntake>();
            ACMethod acMethod = pwMethod.CurrentACMethod.ValueT;
            if (acMethod == null || pwMethod == null)
                return StartDisResult.CancelDischarging;
            using (var dbIPlus = new Database())
            using (var dbApp = new DatabaseApp(dbIPlus))
            {
                Picking picking = pwMethod.CurrentPicking != null ? pwMethod.CurrentPicking.FromAppContext<Picking>(dbApp) : null;
                PickingPos pickingPos = pwMethod.CurrentPickingPos != null ? pwMethod.CurrentPickingPos.FromAppContext<PickingPos>(dbApp) : null;
                DeliveryNotePos notePos = pwMethod.CurrentDeliveryNotePos != null ? pwMethod.CurrentDeliveryNotePos.FromAppContext<DeliveryNotePos>(dbApp) : null;
                FacilityBooking facilityBooking = pwMethod.CurrentFacilityBooking != null ? pwMethod.CurrentFacilityBooking.FromAppContext<FacilityBooking>(dbApp) : null;

                if (picking == null && notePos == null && facilityBooking == null)
                {
                    IACObjectEntity entity = GetTransportEntityFromACMethod(dbApp, acMethod);
                    if (entity == null)
                    {
                        //Error50157:Entity in ACMethod is null.
                        Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingIntake(1)", 1000, "Error50157");
                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), "StartDischargingIntake(1)", msg.InnerMessage);

                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        return StartDisResult.CancelDischarging;
                    }
                    picking = entity as Picking;
                    notePos = entity as DeliveryNotePos;
                    facilityBooking = entity as FacilityBooking;
                }
                if (picking != null)
                {
                    return StartDischargingPicking(module, acMethod, dbIPlus, dbApp, picking, pickingPos);
                }
                else if (notePos != null)
                {
                    return StartDischargingInDNote(module, acMethod, dbIPlus, dbApp, notePos);
                }
                else if (facilityBooking != null)
                {
                    return StartDischargingFBooking(module, acMethod, dbIPlus, dbApp, facilityBooking);
                }
            }
            return StartDisResult.CancelDischarging;
        }

        protected virtual StartDisResult StartDischargingInDNote(PAProcessModule module, ACMethod acMethodRoot, Database db, DatabaseApp dbApp, DeliveryNotePos dnPos)
        {
            Msg msg = null;
            //string message = "";
            if (dnPos == null)
            {
                //Error50161: DeliveryNotePos is null.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingInDNote(10)", 1010, "Error50161");

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), "StartDischargingInDNote(10)", msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CancelDischarging;
            }
            if (!dnPos.InOrderPosID.HasValue)
            {
                //Error50160: DeliveryNotePos is not for intaking.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingInDNote(20)", 1020, "Error50160");

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), "StartDischargingInDNote(20)", msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CancelDischarging;
            }

            double targetQuantity = 0;

            if (CurrentDischargingDest(null) == null)
            {
                var queryPosSameMaterial = dbApp.DeliveryNotePos.Where(c => c.DeliveryNoteID == dnPos.DeliveryNoteID
                    && c.InOrderPosID.HasValue
                    && c.InOrderPos.MaterialID == dnPos.InOrderPos.MaterialID
                    && c.InOrderPos.MDDelivPosLoadStateID.HasValue
                    && (c.InOrderPos.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad
                    || c.InOrderPos.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive
                    || c.InOrderPos.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.BatchActive))
                    .OrderBy(c => c.Sequence);
                targetQuantity = queryPosSameMaterial.Sum(c => c.InOrderPos.TargetQuantityUOM);

                IList<FacilityReservation> plannedSilos = ParentPWMethod<PWMethodIntake>().ACFacilityManager.GetSelectedTargets(dnPos, module.ComponentClass);
                if (plannedSilos == null || !plannedSilos.Any())
                {
                    //Error50102 No destination defined in Deliverynote {0}, Line {1}
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingInDNote(30)", 1030, "Error50102", 
                                  dnPos.DeliveryNote.DeliveryNoteNo, dnPos.InOrderPos.Material.MaterialName1);

                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    return StartDisResult.CycleWait;
                }

                Facility destinationSilo = null;
                FacilityReservation facReservation = GetNextFreeDestination(plannedSilos, dnPos, targetQuantity);
                if (facReservation != null)
                    destinationSilo = facReservation.Facility;

                if (destinationSilo == null)
                {
                    // Error50103	No further target-container/silo/tank found, which has enough remaining space for this Deliverynote {0}, Line {1}
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingInDNote(40)", 273, "Error50103",
                                  dnPos.DeliveryNote.DeliveryNoteNo, dnPos.InOrderPos.Material.MaterialName1);

                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    return StartDisResult.CycleWait;
                }

                gip.core.datamodel.ACClass acClassSilo = destinationSilo.GetFacilityACClass(Root.Database as Database);
                if (acClassSilo == null)
                {
                    //Error50104 acClassSilo is null at Deliverynote {0}, Line{1}
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingInDNote(50)", 1040, "Error50104",
                                  dnPos.DeliveryNote.DeliveryNoteNo, dnPos.InOrderPos.Material.MaterialName1);

                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    return StartDisResult.CycleWait;
                }

                ACComponent targetSiloACComp = this.Root.ACUrlCommand(acClassSilo.GetACUrlComponent()) as ACComponent;
                if (targetSiloACComp == null)
                {
                    // Error50105 targetSiloACComp is null at DeliveryNote {0}, Line {1}
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingInDNote(60)", 1050, "Error50105",
                                    dnPos.DeliveryNote.DeliveryNoteNo, dnPos.InOrderPos.Material.MaterialName1);

                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
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

                MDReleaseState.ReleaseStates resultReleaseState = MDReleaseState.ReleaseStates.Free;
                foreach (DeliveryNotePos notePos in dnPos.DeliveryNote.DeliveryNotePos_DeliveryNote)
                {
                    LabOrder labOrder = notePos.InOrderPos.LabOrder_InOrderPos.FirstOrDefault();
                    if (labOrder != null)
                    {
                        ACLabOrderManager labOrderManger = ParentPWMethod<PWMethodIntake>().LabOrderManager as ACLabOrderManager;
                        if (labOrderManger != null)
                        {
                            msg = labOrderManger.GetMaterialReleaseState(labOrder, notePos.InOrderPos.Material, ref resultReleaseState);
                        }
                    }
                }

                // Falls Zielsilo nicht belegt
                if (!destinationSilo.MaterialID.HasValue || resultReleaseState == MDReleaseState.ReleaseStates.Locked || db.IsChanged)
                {
                    if (resultReleaseState == MDReleaseState.ReleaseStates.Locked)
                        destinationSilo.OutwardEnabled = false;
                    if (destinationSilo.MDFacilityType != null
                        && destinationSilo.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                        destinationSilo.Material = dnPos.Material;
                    db.ACSaveChanges();
                }
            }

            if (CurrentDischargingDest(null) == null)
            {
                // Error50106 CurrentDischargingDest() is null because no route couldn't be found at DeliveryNote{0}, Line {1}.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingInDNote(70)", 1060, "Error50106",
                                dnPos.DeliveryNote.DeliveryNoteNo, dnPos.InOrderPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            PAProcessModule targetModule = TargetPAModule(Root.Database as Database) as PAProcessModule;
            if (targetModule == null)
            {
                // Error50107 targetModule is null at DeliveryNote {0}, Line {1}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingInDNote(80)", 1070, "Error50107",
                                dnPos.DeliveryNote.DeliveryNoteNo, dnPos.InOrderPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
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
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingInDNote(81)", 1080, "Error50153");

                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }
            PAProcessFunction responsibleFunc = CanStartProcessFunc(module, acMethod, dbApp, dnPos, targetModule);
            if (responsibleFunc == null)
                return StartDisResult.CycleWait;

            if (!(bool)ExecuteMethod("GetConfigForACMethod", acMethod, true, dbApp, dnPos, targetModule))
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

            if (targetQuantity < 0.000001)
            {
                var queryPosSameMaterial = dbApp.DeliveryNotePos.Where(c => c.DeliveryNoteID == dnPos.DeliveryNoteID
                && c.InOrderPosID.HasValue
                && c.InOrderPos.MaterialID == dnPos.InOrderPos.MaterialID
                && c.InOrderPos.MDDelivPosLoadStateID.HasValue
                && (c.InOrderPos.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad
                || c.InOrderPos.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive
                || c.InOrderPos.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.BatchActive))
                .OrderBy(c => c.Sequence);
                targetQuantity = queryPosSameMaterial.Sum(c => c.InOrderPos.TargetQuantityUOM);
            }

            ACValue acValueTargetQ = acMethod.ParameterValueList.GetACValue("TargetQuantity");
            if (acValueTargetQ != null)
                acValueTargetQ.Value = targetQuantity;

            NoTargetWait = null;
            if (!(bool)ExecuteMethod("AfterConfigForACMethodIsSet", acMethod, true, dbApp, dnPos, targetModule))
                return StartDisResult.CycleWait;

            if (!acMethod.IsValid())
            {
                // Error50108 Dischargingtask not startable at DeliveryNote {0}, line {1}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingInDNote(90)", 1090, "Error50108",
                                dnPos.DeliveryNote.DeliveryNoteNo, dnPos.InOrderPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            RecalcTimeInfo();
            CurrentDisEntityID.ValueT = dnPos.DeliveryNotePosID;
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
                    // Error50108 Dischargingtask not startable at DeliveryNote {0}, line {2}
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischarging(100)", 1100, "Error50108",
                                dnPos.DeliveryNote.DeliveryNoteNo, dnPos.InOrderPos.Material.MaterialName1);

                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                        Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                }
                return StartDisResult.CycleWait;
            }
            UpdateCurrentACMethod();

            CheckIfAutomaticTargetChangePossible = null;
            MsgWithDetails msg2 = dbApp.ACSaveChanges();
            if (msg2 != null)
            {
                Messages.LogException(this.GetACUrl(), "StartDischarging(110)", msg2.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, new Msg(msg2.InnerMessage, this, eMsgLevel.Exception, PWClassName, "StartDischargingInDNote", 1110), true);
                return StartDisResult.CycleWait;
            }
            AcknowledgeAlarms();
            return task.State == PointProcessingState.Deleted ? StartDisResult.CancelDischarging : StartDisResult.WaitForCallback;
            //return StartDisResult.WaitForCallback;
        }

        protected virtual StartDisResult OnHandleStateCheckFullSiloIntake(PAFDischarging discharging, PAProcessModule targetContainer, PAProcessModule module)
        {
            if (!IsIntake)
                return StartDisResult.CancelDischarging;
            var pwMethod = ParentPWMethod<PWMethodIntake>();
            ACMethod acMethod = pwMethod.CurrentACMethod.ValueT;
            if (acMethod == null)
                return StartDisResult.CancelDischarging;

            using (var dbIPlus = new Database())
            using (var dbApp = new DatabaseApp(dbIPlus))
            {
                Picking picking = null;
                DeliveryNotePos notePos = null;
                if (pwMethod.CurrentPicking != null)
                {
                    picking = pwMethod.CurrentPicking.FromAppContext<Picking>(dbApp);
                    PickingPos pickingPos = pwMethod.CurrentPickingPos != null ? pwMethod.CurrentPickingPos.FromAppContext<PickingPos>(dbApp) : null;
                    if (picking != null)
                        return OnHandleStateCheckFullSiloPicking(discharging, targetContainer, module, acMethod, dbIPlus, dbApp, picking, pickingPos);
                }
                else if (pwMethod.CurrentDeliveryNotePos != null)
                {
                    notePos = pwMethod.CurrentDeliveryNotePos.FromAppContext<DeliveryNotePos>(dbApp);
                    if (notePos != null)
                        return OnHandleStateCheckFullSiloInNotePos(discharging, targetContainer, module, acMethod, dbIPlus, dbApp, notePos);
                }
            }
            return StartDisResult.WaitForCallback;
        }

        protected virtual StartDisResult OnHandleStateCheckFullSiloInNotePos(PAFDischarging discharging, PAProcessModule targetContainer, PAProcessModule module, ACMethod acMethodRoot, Database db, DatabaseApp dbApp, DeliveryNotePos dnPos)
        {
            Msg msg = null;
            PAMSilo targetSilo = targetContainer as PAMSilo;
            if (targetSilo == null)
            {
                // Error50107 targetModule is null at DeliveryNote {0}, Line {1}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloInNotePos(10)", 1120, "Error50107",
                                dnPos.DeliveryNote.DeliveryNoteNo, dnPos.InOrderPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CancelDischarging;
            }

            Route previousDischargingRoute = CurrentDischargingRoute;
            IList<FacilityReservation> plannedSilos = ParentPWMethod<PWMethodIntake>().ACFacilityManager.GetSelectedTargets(dnPos, module.ComponentClass);
            if (plannedSilos == null || !plannedSilos.Any())
            {
                CheckIfAutomaticTargetChangePossible = false;
                //Error50102 No destination defined in Deliverynote {0}, Line {1}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloInNotePos(20)", 1130, "Error50102",
                                dnPos.DeliveryNote.DeliveryNoteNo, dnPos.InOrderPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            // 1. Prüfe zuerst ob das geplante Silo, das zur Zeit als aktive Befüllung gesetzt worden ist, befüllt werden kann
            Facility destinationSilo = null;
            foreach (FacilityReservation plannedSilo in plannedSilos.Where(c => c.ReservationState == GlobalApp.ReservationState.Active))
            {
                if (plannedSilo.FacilityID == targetSilo.Facility.ValueT.ValueT.FacilityID)
                {
                    plannedSilo.ReservationState = GlobalApp.ReservationState.Finished;
                    break;
                }
            }


            destinationSilo = null;
            FacilityReservation facReservation = GetNextFreeDestination(plannedSilos, dnPos, 0);
            if (facReservation != null)
                destinationSilo = facReservation.Facility;

            if (destinationSilo == null)
            {
                CheckIfAutomaticTargetChangePossible = false;
                // Error50103	No further target-container/silo/tank found, which has enough remaining space for this Deliverynote {0}, Line {1}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloInNotePos(30)", 1140, "Error50103",
                                dnPos.DeliveryNote.DeliveryNoteNo, dnPos.InOrderPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            gip.core.datamodel.ACClass acClassSilo = destinationSilo.GetFacilityACClass(Root.Database as Database);
            if (acClassSilo == null)
            {
                CheckIfAutomaticTargetChangePossible = false;
                //Error50104 acClassSilo is null at Deliverynote {0}, Line{1}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloInNotePos(40)", 1150, "Error50104",
                                dnPos.DeliveryNote.DeliveryNoteNo, dnPos.InOrderPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            ACComponent targetSiloACComp = this.Root.ACUrlCommand(acClassSilo.GetACUrlComponent()) as ACComponent;
            if (targetSiloACComp == null)
            {
                CheckIfAutomaticTargetChangePossible = false;
                // Error50105 targetSiloACComp is null at DeliveryNote {0}, Line {1}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloInNotePos(50)", 1160, "Error50105",
                                dnPos.DeliveryNote.DeliveryNoteNo, dnPos.InOrderPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            CurrentDischargingRoute = null;
            Type typeOfSilo = typeof(PAMSilo);
            Guid thisMethodID = ContentACClassWF.ACClassMethodID;
            DetermineDischargingRoute(Root.Database as Database, module, targetSiloACComp, 0,
                                    (c, p, r) => (c.ACKind == Global.ACKinds.TPAProcessModule
                                            && (typeOfSilo.IsAssignableFrom(c.ObjectType)
                                                || !c.BasedOnACClassID.HasValue
                                                || (c.BasedOnACClassID.HasValue && c.ACClass1_BasedOnACClass.ACClassWF_RefPAACClass.Where(refc => refc.ACClassMethodID != thisMethodID).Any()))),
                                    PAMSilo.SelRuleID_Silo_Deselector, null);

            if (CurrentDischargingDest(null) == null)
            {
                // Error50106 CurrentDischargingDest() is null because no route couldn't be found at DeliveryNote{0}, Line {1}.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloInNotePos(60)", 1170, "Error50106",
                                dnPos.DeliveryNote.DeliveryNoteNo, dnPos.InOrderPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            targetSilo = TargetPAModule(Root.Database as Database) as PAMSilo;
            if (targetSilo == null)
            {
                // Error50107 targetModule is null at DeliveryNote {0}, Line {1}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloInNotePos(70)", 1180, "Error50107",
                                dnPos.DeliveryNote.DeliveryNoteNo, dnPos.InOrderPos.Material.MaterialName1);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            PAMIntake intakeBin = module as PAMIntake;
            double actualWeight = intakeBin.Scale != null ? intakeBin.Scale.ActualWeight.ValueT - intakeBin.Scale.StoredTareWeight.ValueT : 0;
            if (actualWeight < 1 && this.IsSimulationOn)
            {
                if (intakeBin.Scale != null)
                    intakeBin.Scale.ActualWeight.ValueT += 1;
                actualWeight = 1;
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


            bool isNewACMethod = false;
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
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "OnHandleStateCheckFullSiloInNotePos(71)", 1190, "Error50153");
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    return StartDisResult.CycleWait;
                }

                isNewACMethod = true;
                if (!(bool)ExecuteMethod("GetConfigForACMethod", acMethod, true, dbApp, dnPos, targetSilo))
                    return StartDisResult.CycleWait;
            }


            acMethod["Route"] = CurrentDischargingRoute != null ? CurrentDischargingRoute.Clone() as Route : null;
            ACValue acValue = acMethod.ParameterValueList.GetACValue("Destination");
            if (acValue != null)
            {
                if (acValue.ObjectType != null)
                    acValue.Value = Convert.ChangeType(targetSilo.RouteItemIDAsNum, acValue.ObjectType);
                else
                    acMethod["Destination"] = targetSilo.RouteItemIDAsNum;
            }
            if (CurrentDischargingRoute != null)
                CurrentDischargingRoute.Detach(true);

            ACValue acValueTargetQ = acMethod.ParameterValueList.GetACValue("TargetQuantity");
            if (acValueTargetQ != null)
                acValueTargetQ.Value = targetQuantity;

            if (isNewACMethod)
            {
                if (!(bool)ExecuteMethod("AfterConfigForACMethodIsSet", acMethod, true, dbApp, dnPos, targetSilo))
                    return StartDisResult.CycleWait;
            }

            // Sende neues Ziel an dies SPS
            msg = OnReSendACMethod(discharging, acMethod, dbApp);
            if (msg != null)
            {
                CurrentDischargingRoute = previousDischargingRoute;
            }
            else
            {
                if (intakeBin.Scale != null)
                    intakeBin.Scale.StoredTareWeight.ValueT = intakeBin.Scale.ActualWeight.ValueT;
                if (!destinationSilo.MaterialID.HasValue
                    && destinationSilo.MDFacilityType != null
                    && destinationSilo.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                    destinationSilo.Material = dnPos.Material;
                if (actualWeight > 0)
                {
                    DoInwardBooking(actualWeight, dbApp, previousDischargingRoute.LastOrDefault(), dnPos, null, false);
                }
                if (dbApp.IsChanged)
                {
                    MsgWithDetails msg2 = dbApp.ACSaveChanges();
                    if (msg2 != null)
                    {
                        Messages.LogException(this.GetACUrl(), "OnHandleStateCheckFullSiloInNotePos(110)", msg2.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, new Msg(msg2.InnerMessage, this, eMsgLevel.Exception, PWClassName, "OnHandleStateCheckFullSiloInNotePos", 1200), true);
                        return StartDisResult.CycleWait;
                    }
                }

                NoTargetWait = null;
                // Quittiere Alarm
                discharging.AcknowledgeAlarms();
                if (discharging.CurrentACState == ACStateEnum.SMPaused)
                    discharging.Resume();
                this.TaskSubscriptionPoint.Persist(false);
            }
            return StartDisResult.WaitForCallback;
        }

        #endregion

        #region Booking
        public virtual Msg DoInwardBooking(double actualWeight, DatabaseApp dbApp, RouteItem dischargingDest, DeliveryNotePos dnPos, ACEventArgs e, bool isDischargingEnd)
        {
            MsgWithDetails collectedMessages = new MsgWithDetails();
            Msg msg = null;
            if (dischargingDest == null)
            {
                //Error50075: No downward-route defined for module {0}
                //Error50158: ParentPWGroup is null.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoInwardBooking(1)", 1210, "Error50075",
                                ParentPWGroup != null ? ParentPWGroup.AccessedProcessModule.GetACUrl() : Root.Environment.TranslateMessage(this, "Error50158"));

                Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return msg;
            }
            else
            {
                if (!dischargingDest.IsAttached)
                    dischargingDest.AttachTo(dbApp);
                if (dnPos == null)
                    return new Msg() { Source = this.GetACUrl(), ACIdentifier = "DoInwardBooking(2)", Message = "DeliveryNotePos is null", MessageLevel = eMsgLevel.Exception };
                Facility inwardFacility = dbApp.Facility.Where(c => c.VBiFacilityACClassID == dischargingDest.Target.ACClassID).FirstOrDefault();
                if (inwardFacility == null) // Entleerung erfolgte auf ProcessModule und kein Silo
                    return null;
                // Falls keine Materialbuchung erfolgen soll
                if (dnPos.Material.MDFacilityManagementType != null && dnPos.Material.MDFacilityManagementType.FacilityManagementType == MDFacilityManagementType.FacilityManagementTypes.NoFacility)
                    return null;

                var queryPosSameMaterial = dbApp.DeliveryNotePos.Where(c => c.DeliveryNoteID == dnPos.DeliveryNoteID
                    && c.InOrderPosID.HasValue
                    && c.InOrderPos.MaterialID == dnPos.InOrderPos.MaterialID
                    && c.InOrderPos.MDDelivPosLoadStateID.HasValue
                    && (c.InOrderPos.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad
                    || c.InOrderPos.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive
                    || c.InOrderPos.MDDelivPosLoadState.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.BatchActive))
                    .OrderBy(c => c.Sequence).ToArray();

                int count = queryPosSameMaterial.Count();

                if (actualWeight <= 0.000001 && isDischargingEnd && count > 0 && this.IsSimulationOn)
                {
                    foreach (var posSameMat in queryPosSameMaterial)
                    {
                        actualWeight += (double)posSameMat.InOrderPos.DifferenceWeight;
                    }
                }
                if (actualWeight <= 0.000001 && this.IsSimulationOn)
                    actualWeight = 1;

                double restWeight = actualWeight;
                for (int i = 0; i < count; i++)
                {
                    double bookingWeight = 0;
                    //bool isRestQ = false;
                    DeliveryNotePos currentPos = queryPosSameMaterial[i];
                    // Falls noch mehr Lieferscheinpositionen vorhanden, dann buche nur Teilmenge ab
                    if (i < (count - 1))
                    {
                        double diffQuantity = currentPos.InOrderPos.DifferenceWeight;
                        if (diffQuantity < -0.0000001)
                        {
                            bookingWeight = restWeight + diffQuantity;
                            // Falls Lieferscheinrestmenge größer als dosierter istwert, dann buche gesamte Istmenge auf diese Lieferscheinposition
                            if (bookingWeight < -0.0000001)
                            {
                                bookingWeight = restWeight;
                            }
                            // Sonst buche rest von dieser Lieferscheinposition ab
                            else
                            {
                                bookingWeight = Math.Abs(diffQuantity);
                                restWeight -= bookingWeight;
                            }
                        }
                        else
                            continue;
                    }
                    else
                    {
                        //isRestQ = true;
                        bookingWeight = restWeight;
                    }

                    if (bookingWeight > 0.0000001)
                    {
                        FacilityLot facilityLot = null;
                        FacilityPreBooking hasBookings = currentPos.InOrderPos.FacilityPreBooking_InOrderPos.FirstOrDefault();
                        if (hasBookings != null)
                            facilityLot = hasBookings.InwardFacilityLot;
                        else
                        {
                            FacilityBooking firstFBooking = currentPos.InOrderPos.FacilityBooking_InOrderPos.FirstOrDefault();
                            if (firstFBooking != null)
                                facilityLot = firstFBooking.InwardFacilityLot;
                        }

                        // 1. Bereite Buchung vor
                        if (facilityLot == null && dnPos.Material.IsLotManaged)
                        {
                            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(FacilityLot), FacilityLot.NoColumnName, FacilityLot.FormatNewNo, this);
                            facilityLot = FacilityLot.NewACObject(dbApp, null, secondaryKey);
                        }

                        var facilityManager = ParentPWMethod<PWMethodIntake>().ACFacilityManager;
                        var inDeliveryNoteManager = ParentPWMethod<PWMethodIntake>().InDeliveryNoteManager;

                        FacilityPreBooking facilityPreBooking = inDeliveryNoteManager.NewFacilityPreBooking(facilityManager, dbApp, currentPos);
                        ACMethodBooking bookingParam = facilityPreBooking.ACMethodBooking as ACMethodBooking;
                        bookingParam.InwardQuantity = currentPos.Material.ConvertBaseWeightToBaseUnit(bookingWeight);
                        bookingParam.InwardFacility = inwardFacility;
                        bookingParam.InwardFacilityLot = facilityLot;

                        msg = dbApp.ACSaveChangesWithRetry();

                        // 2. Führe Buchung durch
                        if (msg != null)
                        {
                            Messages.LogError(this.GetACUrl(), "DoInwardBooking(5)", msg.InnerMessage);
                            return msg;
                        }
                        else if (facilityPreBooking != null)
                        {
                            bookingParam.IgnoreIsEnabled = true;
                            ACMethodEventArgs resultBooking = ACFacilityManager.BookFacilityWithRetry(ref bookingParam, dbApp);
                            if (resultBooking.ResultState == Global.ACMethodResultState.Failed || resultBooking.ResultState == Global.ACMethodResultState.Notpossible)
                            {
                                collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
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
                                    currentPos.InOrderPos.TopParentInOrderPos.RecalcActualQuantity();
                                }
                                else
                                {
                                    collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                                }
                                msg = dbApp.ACSaveChangesWithRetry();
                                if (msg != null)
                                {
                                    collectedMessages.AddDetailMessage(msg);
                                    OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoInwardBooking", 1220), true);
                                    Messages.LogError(this.GetACUrl(), "DoInwardBooking(10)", msg.Message);
                                }
                            }
                        }
                    }
                }

                if (isDischargingEnd)
                {
                    MDDelivPosLoadState mdStateLoaded = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();
                    MDDelivPosState mdStateDeliv = DatabaseApp.s_cQry_GetMDDelivPosState(dbApp, MDDelivPosState.DelivPosStates.Delivered).FirstOrDefault();
                    foreach (DeliveryNotePos currentPos in queryPosSameMaterial)
                    {
                        currentPos.InOrderPos.MDDelivPosLoadState = mdStateLoaded;
                        currentPos.InOrderPos.MDDelivPosState = mdStateDeliv;
                        currentPos.InOrderPos.TopParentInOrderPos.RecalcDeliveryStates();
                    }
                    msg = dbApp.ACSaveChangesWithRetry();
                    if (msg != null)
                    {
                        collectedMessages.AddDetailMessage(msg);
                        OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoInwardBooking", 1220), true);
                        Messages.LogError(this.GetACUrl(), "DoInwardBooking(11)", msg.InnerMessage);
                    }
                }
            }
            return collectedMessages.MsgDetailsCount > 0 ? collectedMessages : null;
        }
        #endregion

        #endregion
    }
}
