using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;

namespace gip.mes.processapplication
{
    public partial class PWDischarging
    {
        #region Methods

        public static Guid GetFacilityBookingIDFromACMethod(ACMethod acMethod)
        {
            Guid facilityBookingID = Guid.Empty;
            ACValue acValue = acMethod.ParameterValueList.GetACValue(FacilityBooking.ClassName);
            if (acValue != null && acValue.Value != null && acValue.Value is Guid)
                facilityBookingID = (Guid)acValue.Value;
            return facilityBookingID;
        }


        #region ACState

        protected virtual StartDisResult StartDischargingRelocation(PAProcessModule module)
        {
            if (!IsRelocation)
                return StartDisResult.CancelDischarging;
            var pwMethod = ParentPWMethod<PWMethodRelocation>();
            ACMethod acMethod = pwMethod.CurrentACMethod.ValueT;
            if (acMethod == null || pwMethod == null)
                return StartDisResult.CancelDischarging;
            using (var dbIPlus = new Database())
            using (var dbApp = new DatabaseApp(dbIPlus))
            {
                Picking picking = pwMethod.CurrentPicking != null ? pwMethod.CurrentPicking.FromAppContext<Picking>(dbApp) : null;
                PickingPos pickingPos = pwMethod.CurrentPickingPos != null ? pwMethod.CurrentPickingPos.FromAppContext<PickingPos>(dbApp) : null;
                FacilityBooking facilityBooking = null;
                if (picking == null)
                {
                    IACObjectEntity entity = GetTransportEntityFromACMethod(dbApp, acMethod);
                    if (entity == null)
                    {
                        //Error50157: Entity in ACMethod is null.
                        Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingRelocation(1)", 1000, "Error50157");
                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), "StartDischargingRelocation(1)", msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        return StartDisResult.CancelDischarging;
                    }
                    picking = entity as Picking;
                    facilityBooking = entity as FacilityBooking;
                }
                if (picking != null)
                {
                    return StartDischargingPicking(module, acMethod, dbIPlus, dbApp, picking, pickingPos);
                }
                else if (facilityBooking != null)
                {
                    return StartDischargingFBooking(module, acMethod, dbIPlus, dbApp, facilityBooking);
                }
            }
            return StartDisResult.CancelDischarging;
        }

        protected virtual StartDisResult StartDischargingFBooking(PAProcessModule module, ACMethod acMethodRoot, Database db, DatabaseApp dbApp, FacilityBooking facilityBooking)
        {
            Msg msg = null;
            //string message = "";
            if (facilityBooking == null)
            {
                //Error50156: FacilityBooking is null.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingFBooking(1)", 1010, "Error50156");

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), "StartDischargingFBooking(1)", msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CancelDischarging;
            }

            if (CurrentDischargingDest(db, false) == null)
            {
                if (facilityBooking.InwardFacility == null)
                {
                    // TODO: Error-Message
                    return StartDisResult.CancelDischarging;
                }
                gip.core.datamodel.ACClass targetACClass = facilityBooking.InwardFacility.FacilityACClass;
                if (targetACClass == null)
                {
                    // TODO: Error-Message
                    return StartDisResult.CancelDischarging;
                }
                string acUrlOfACClass = targetACClass.GetACUrlComponent();
                if (String.IsNullOrEmpty(acUrlOfACClass))
                {
                    // TODO: Error-Message
                    return StartDisResult.CancelDischarging;
                }
                ACComponent targetSiloACComp = ACUrlCommand(acUrlOfACClass) as ACComponent;
                if (targetSiloACComp == null)
                {
                    // TODO: Error-Message
                    return StartDisResult.CancelDischarging;
                }

                Type typeOfSilo = typeof(PAMSilo);
                Guid thisMethodID = ContentACClassWF.ACClassMethodID;
                DetermineDischargingRoute(Root.Database as Database, module, targetSiloACComp, 0,
                                        (c, p, r) => (c.ACKind == Global.ACKinds.TPAProcessModule
                                                && (typeOfSilo.IsAssignableFrom(c.ObjectType)
                                                    || !c.BasedOnACClassID.HasValue
                                                    || (c.BasedOnACClassID.HasValue && c.ACClass1_BasedOnACClass.ACClassWF_RefPAACClass.Where(refc => refc.ACClassMethodID != thisMethodID).Any()))),
                                        PAMSilo.SelRuleID_Silo_Deselector, null);

            }

            if (CurrentDischargingDest(db, false) == null)
            {
                // Error50109 CurrentDischargingDest() is null because no route couldn't be found for Relocationbooking {0}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingFBooking(10)", 1020, "Error50109", facilityBooking.FacilityBookingNo);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            PAProcessModule targetModule = TargetPAModule(Root.Database as Database) as PAProcessModule;
            if (targetModule == null)
            {
                // Error50110: targetModule is null at Relocationbooking {0}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingFBooking(11)", 1030, "Error50110", facilityBooking.FacilityBookingNo);

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
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingFBooking(12)", 1040, "Error50153");
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }
            PAProcessFunction responsibleFunc = CanStartProcessFunc(module, acMethod, dbApp, facilityBooking, targetModule);
            if (responsibleFunc == null)
                return StartDisResult.CycleWait;

            if (!(bool)ExecuteMethod(nameof(GetConfigForACMethod), acMethod, true, dbApp, facilityBooking, targetModule))
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

            NoTargetWait = null;
            if (!(bool)ExecuteMethod(nameof(AfterConfigForACMethodIsSet), acMethod, true, dbApp, facilityBooking, targetModule))
                return StartDisResult.CycleWait;

            if (!acMethod.IsValid())
            {
                // Error50111 Dischargingtask not startable for Relocationbooking {0}
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingFBooking(20)", 1050, "Error50111", facilityBooking.FacilityBookingNo);

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CycleWait;
            }

            RecalcTimeInfo();
            CurrentDisEntityID.ValueT = facilityBooking.FacilityBookingID;
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
                    // Error50111 Dischargingtask not startable for Relocationbooking {0}
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingFBooking(21)", 1060, "Error50111", facilityBooking.FacilityBookingNo);

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
                Messages.LogException(this.GetACUrl(), "StartDischargingFBooking(22)", msg2.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, new Msg(msg2.InnerMessage, this, eMsgLevel.Exception, PWClassName, "StartDischargingFBooking", 1070), true);
                return StartDisResult.CycleWait;
            }
            AcknowledgeAlarms();
            ExecuteMethod(nameof(OnACMethodSended), acMethod, true, dbApp, facilityBooking, targetModule, responsibleFunc);
            return task.State == PointProcessingState.Deleted ? StartDisResult.CancelDischarging : StartDisResult.WaitForCallback;
            //return StartDisResult.WaitForCallback;
        }

        protected virtual StartDisResult OnHandleStateCheckFullSiloRelocation(PAFDischarging discharging, PAProcessModule targetContainer, PAProcessModule module)
        {
            if (!IsRelocation)
                return StartDisResult.CancelDischarging;
            var pwMethod = ParentPWMethod<PWMethodRelocation>();
            ACMethod acMethod = pwMethod.CurrentACMethod.ValueT;
            if (acMethod == null)
                return StartDisResult.CancelDischarging;
            using (var dbIPlus = new Database())
            using (var dbApp = new DatabaseApp(dbIPlus))
            {
                Picking picking = null;
                FacilityBooking fBooking = null;
                if (pwMethod.CurrentPicking != null)
                {
                    picking = pwMethod.CurrentPicking.FromAppContext<Picking>(dbApp);
                    PickingPos pickingPos = pwMethod.CurrentPickingPos != null ? pwMethod.CurrentPickingPos.FromAppContext<PickingPos>(dbApp) : null;
                    if (picking != null)
                        return OnHandleStateCheckFullSiloPicking(discharging, targetContainer, module, acMethod, dbIPlus, dbApp, picking, pickingPos);
                }
                else if (pwMethod.CurrentFacilityBooking != null)
                {
                    fBooking = pwMethod.CurrentFacilityBooking.FromAppContext<FacilityBooking>(dbApp);
                    if (fBooking != null)
                        return OnHandleStateCheckFullSiloFBooking(discharging, targetContainer, module, acMethod, dbIPlus, dbApp, fBooking);
                }
            }
            return StartDisResult.WaitForCallback;
        }

        protected virtual StartDisResult OnHandleStateCheckFullSiloFBooking(PAFDischarging discharging, PAProcessModule targetContainer, PAProcessModule module, ACMethod acMethodRoot, Database db, DatabaseApp dbApp, FacilityBooking fBooking)
        {
            // TDOO: Implement Standard-Behaviour for Relocation
            //Msg msg = null;
            return StartDisResult.CancelDischarging;
        }


        #endregion

        #region Booking
        public virtual Msg DoInwardBooking(double actualWeight, DatabaseApp dbApp, RouteItem dischargingDest, FacilityBooking fb, ACEventArgs e, bool isDischargingEnd)
        {
            // TDOO: Implement Standard-Behaviour for Relocation
            return null;
        }
        #endregion

        #endregion
    }
}
