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
        protected virtual StartDisResult StartDischargingLoading(PAProcessModule module)
        {
            if (!IsLoading)
                return StartDisResult.CancelDischarging;
            var pwMethod = ParentPWMethod<PWMethodLoading>();
            ACMethod acMethod = pwMethod.CurrentACMethod.ValueT;
            if (acMethod == null || pwMethod == null)
                return StartDisResult.CancelDischarging;
            using (var dbIPlus = new Database())
            using (var dbApp = new DatabaseApp(dbIPlus))
            {
                Picking picking = pwMethod.CurrentPicking != null ? pwMethod.CurrentPicking.FromAppContext<Picking>(dbApp) : null;
                PickingPos pickingPos = pwMethod.CurrentPickingPos != null ? pwMethod.CurrentPickingPos.FromAppContext<PickingPos>(dbApp) : null;
                DeliveryNotePos notePos = null;
                if (picking == null)
                {
                    IACObjectEntity entity = GetTransportEntityFromACMethod(dbApp, acMethod);
                    if (entity == null)
                    {
                        //Error50157:Entity in ACMethod is null.
                        Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingLoading(1)", 1000, "Error50157");

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), "StartDischargingLoading(1)", msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        return StartDisResult.CancelDischarging;
                    }
                    picking = entity as Picking;
                    notePos = entity as DeliveryNotePos;
                }
                if (picking != null)
                {
                    return StartDischargingPicking(module, acMethod, dbIPlus, dbApp, picking, pickingPos);
                }
                else if (notePos != null)
                {
                    return StartDischargingOutDNote(module, acMethod, dbIPlus, dbApp, notePos);
                }
            }
            return StartDisResult.CancelDischarging;
        }

        protected virtual StartDisResult StartDischargingOutDNote(PAProcessModule module, ACMethod acMethodRoot, Database db, DatabaseApp dbApp, DeliveryNotePos dnPos)
        {
            Msg msg = null;
            //string message = "";
            if (dnPos == null)
            {
                //Error50161:DeliveryNotePos is null.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingOutDNote(1)", 1010, "Error50161");
                
                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), "StartDischargingOutDNote(1)", msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CancelDischarging;
            }
            if (!dnPos.InOrderPosID.HasValue)
            {
                //Error50166:DeliveryNotePos is not for loading.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartDischargingOutDNote(2)", 1020, "Error50166");

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), "StartDischargingOutDNote(2)", msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartDisResult.CancelDischarging;
            }

            return StartDisResult.CancelDischarging;
        }


        protected virtual StartDisResult OnHandleStateCheckFullSiloLoading(PAFDischarging discharging, PAProcessModule targetContainer, PAProcessModule module)
        {
            if (!IsLoading)
                return StartDisResult.CancelDischarging;
            var pwMethod = ParentPWMethod<PWMethodLoading>();
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
                        return OnHandleStateCheckFullSiloOutNotePos(discharging, targetContainer, module, acMethod, dbIPlus, dbApp, notePos);
                }
            }
            return StartDisResult.WaitForCallback;
        }

        protected virtual StartDisResult OnHandleStateCheckFullSiloOutNotePos(PAFDischarging discharging, PAProcessModule targetContainer, PAProcessModule module, ACMethod acMethodRoot, Database db, DatabaseApp dbApp, DeliveryNotePos notePos)
        {
            // TODO: Implement Standard-Behaviour for DeliveryNotePos
            //Msg msg = null;
            return StartDisResult.CycleWait;
        }

        #region Booking
        public virtual Msg DoOutwardBooking(double actualQuantity, DatabaseApp dbApp, RouteItem dischargingDest, DeliveryNotePos dnPos, ACEventArgs e, bool isDischargingEnd)
        {
            // TODO: Implement Standard-Behaviour for DeliveryNotePos
            return null;
        }
        #endregion

        #endregion
    }
}
