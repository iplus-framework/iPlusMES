using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Change bucket'}de{'Eimer wechseln'}", Global.ACKinds.TPWNodeStatic, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWBinChangeLoop : PWNodeDecisionFunc
    {

        public const string PWClassName = "PWBinChangeLoop";

        #region c´tors
        static PWBinChangeLoop()
        {
            RegisterExecuteHandler(typeof(PWBinChangeLoop), HandleExecuteACMethod_PWBinChangeLoop);
        }

        public PWBinChangeLoop(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties => Managers

        public ACProdOrderManager ProdOrderManager
        {
            get
            {
                PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                return pwMethodProduction != null ? pwMethodProduction.ProdOrderManager : null;
            }
        }

        protected FacilityManager ACFacilityManager
        {
            get
            {
                if (ParentPWMethodVBBase == null)
                    return null;
                return ParentPWMethodVBBase.ACFacilityManager as FacilityManager;
            }
        }

        #endregion

        #region Properties

        #region Properties -> Structure
        public T ParentPWMethod<T>() where T : PWMethodVBBase
        {
            if (ParentRootWFNode == null)
                return null;
            return ParentRootWFNode as T;
        }

        public PWMethodVBBase ParentPWMethodVBBase
        {
            get
            {
                return ParentRootWFNode as PWMethodVBBase;
            }
        }

        public bool IsProduction
        {
            get
            {
                return ParentPWMethod<PWMethodProduction>() != null;
            }
        }
        #endregion

        #endregion

        #region Methods

        #region Methods => ACState

        public override void SMStarting()
        {
            PAProcessModule module = ParentPWGroup.AccessedProcessModule;
            PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
            Guid? intermediateChildPosID = GetIntermediateChildPosID(ContentACClassWF, pwMethodProduction);
            PWBinSelection pwBinSelection = this.FindPredecessors<PWBinSelection>(true, c => c is PWBinSelection, c => c is PWBinSelection, 0).FirstOrDefault();
            if (intermediateChildPosID == null)
            {
                string errorMsg = "Missing batch!";
                OnNewAlarmOccurred(ProcessAlarm, errorMsg);
                Messages.LogError(this.GetACUrl(), "SMStarting()", errorMsg);
                Reset();
                return;
            }
            if (pwBinSelection == null)
            {
                string errorMsg = "PWBinSelection not detected! Must be in same group!";
                OnNewAlarmOccurred(ProcessAlarm, errorMsg);
                Messages.LogError(this.GetACUrl(), "SMStarting()", errorMsg);
                Reset();
                return;
            }

            PositionStatusCheck positionStatusCheck = IsAllRelationsCompleted(intermediateChildPosID.Value);
            if (positionStatusCheck.IsAnyCanceledPosition || positionStatusCheck.AllPositionsCompleted)
            {
                MsgWithDetails msgUpdateBooking = pwBinSelection.CompleteReservationBookingWithRestQ(intermediateChildPosID.Value);
                if (!msgUpdateBooking.IsSucceded())
                    ActivateProcessAlarmWithLog(msgUpdateBooking, false);
                RaiseOutEventAndComplete();
            }
            else
                RaiseElseEventAndComplete();
        }

        #endregion

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PWBinChangeLoop(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWNodeDecisionFunc(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        #endregion

        #region Methods -> Helper

        private Guid? GetIntermediateChildPosID(core.datamodel.ACClassWF acClassWF, PWMethodProduction pwMethodProduction)
        {
            Guid? intermediateChildPosID = null;
            using (var dbIPlus = new Database())
            {
                using (var dbApp = new DatabaseApp(dbIPlus))
                {
                    ProdOrderPartslistPos endBatchPos = pwMethodProduction.CurrentProdOrderPartslistPos.FromAppContext<ProdOrderPartslistPos>(dbApp);
                    if (pwMethodProduction.CurrentProdOrderBatch == null)
                        return null;

                    var contentACClassWFVB = ContentACClassWF.FromAppContext<gip.mes.datamodel.ACClassWF>(dbApp);
                    ProdOrderBatch batch = pwMethodProduction.CurrentProdOrderBatch.FromAppContext<ProdOrderBatch>(dbApp);
                    ProdOrderBatchPlan batchPlan = batch.ProdOrderBatchPlan;
                    MaterialWFConnection matWFConnection = null;
                    if (batchPlan != null && batchPlan.MaterialWFACClassMethodID.HasValue)
                    {
                        matWFConnection = dbApp.MaterialWFConnection
                                                .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == batchPlan.MaterialWFACClassMethodID.Value
                                                        && c.ACClassWFID == contentACClassWFVB.ACClassWFID)
                                                .FirstOrDefault();
                    }
                    else
                    {
                        PartslistACClassMethod plMethod = endBatchPos.ProdOrderPartslist.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
                        if (plMethod != null)
                        {
                            matWFConnection = dbApp.MaterialWFConnection
                                                    .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == plMethod.MaterialWFACClassMethodID
                                                            && c.ACClassWFID == contentACClassWFVB.ACClassWFID)
                                                    .FirstOrDefault();
                        }
                        else
                        {
                            matWFConnection = contentACClassWFVB.MaterialWFConnection_ACClassWF
                                .Where(c => c.MaterialWFACClassMethod.MaterialWFID == endBatchPos.ProdOrderPartslist.Partslist.MaterialWFID
                                            && c.MaterialWFACClassMethod.PartslistACClassMethod_MaterialWFACClassMethod.Where(d => d.PartslistID == endBatchPos.ProdOrderPartslist.PartslistID).Any())
                                .FirstOrDefault();
                        }
                    }

                    if (matWFConnection == null)
                    {
                        return null;
                    }

                    // Find intermediate position which is assigned to this Dosing-Workflownode
                    var currentProdOrderPartslist = endBatchPos.ProdOrderPartslist.FromAppContext<ProdOrderPartslist>(dbApp);
                    ProdOrderPartslistPos intermediatePosition = currentProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                        .Where(c => c.MaterialID.HasValue && c.MaterialID == matWFConnection.MaterialID
                            && c.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern
                            && !c.ParentProdOrderPartslistPosID.HasValue).FirstOrDefault();
                    if (intermediatePosition == null)
                    {
                        return null;
                    }

                    // Lock, if a parallel Dosing also creates a child Position for this intermediate Position

                    using (ACMonitor.Lock(pwMethodProduction._62000_PWGroupLockObj))
                    {
                        // Find intermediate child position, which is assigned to this Batch
                        var intermediateChildPos = intermediatePosition.ProdOrderPartslistPos_ParentProdOrderPartslistPos
                            .Where(c => c.ProdOrderBatchID.HasValue
                                        && c.ProdOrderBatchID.Value == pwMethodProduction.CurrentProdOrderBatch.ProdOrderBatchID)
                            .FirstOrDefault();
                        intermediateChildPosID = intermediateChildPos.ProdOrderPartslistPosID;
                    }
                }
            }
            return intermediateChildPosID;
        }

        private PositionStatusCheck IsAllRelationsCompleted(Guid intermediateChildPosID)
        {
            PositionStatusCheck result = new PositionStatusCheck();
            using (var dbIPlus = new Database())
            {
                using (var dbApp = new DatabaseApp(dbIPlus))
                {
                    ProdOrderPartslistPos intermediateChildPos = dbApp.ProdOrderPartslistPos.FirstOrDefault(c => c.ProdOrderPartslistPosID == intermediateChildPosID);
                    result.IsAnyCanceledPosition = intermediateChildPos != null &&
                            intermediateChildPos
                            .ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Where(c => c.TargetQuantityUOM > 0.00000001)
                            .Any(c =>
                                        c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled ||
                                        c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Blocked
                                );
                    if (!result.IsAnyCanceledPosition)
                        result.AllPositionsCompleted = intermediateChildPos != null &&
                            !intermediateChildPos
                            .ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Where(c => c.TargetQuantityUOM > 0.00000001)
                            .Any(c =>
                                        c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Created ||
                                        c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.PartialCompleted
                                );
                }
            }
            return result;
        }

        

        public void ActivateProcessAlarmWithLog(Msg msg, bool autoAck = true)
        {
            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                Messages.LogMessageMsg(msg);
            ActivateProcessAlarm(msg, autoAck);
        }

        public void ActivateProcessAlarm(Msg msg, bool autoAck = true)
        {
            OnNewAlarmOccurred(ProcessAlarm, msg, autoAck);
            ProcessAlarm.ValueT = PANotifyState.AlarmOrFault;
        }

        #endregion

        #endregion

    }


    class PositionStatusCheck
    {
        public bool AllPositionsCompleted { get; set; }
        public bool IsAnyCanceledPosition { get; set; }
    }
}
