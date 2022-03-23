using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.mes.manager;
using System;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PAShowDlgManagerVB'}de{'PAShowDlgManagerVB'}", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public class PAShowDlgManagerVB : PAShowDlgManagerVBBase
    {
        #region c´tors
        public PAShowDlgManagerVB(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public const string ClassNameVB = "PAShowDlgManagerVB";

        #endregion

        #region Public Methods

        #region OrderInfo methods
        public override string BuildAndSetOrderInfo(PAProcessModule pm)
        {
            if (pm == null)
                return null;
            if (   pm.Semaphore == null 
                || pm.Semaphore.ConnectionListCount <= 0)
            {
                IACContainerTNet<ACRef<ProdOrderPartslistPos>> currentBatchPos = null;
                PAMHopperscale hopperScale = pm as PAMHopperscale;
                if (hopperScale != null)
                    currentBatchPos = hopperScale.CurrentBatchPos;
                else
                {
                    PAMIntermediatebin bin = pm as PAMIntermediatebin;
                    if (bin != null)
                        currentBatchPos = bin.CurrentBatchPos;
                }
                if (currentBatchPos != null && currentBatchPos.ValueT != null)
                {
                    using (var dbApp = new DatabaseApp())
                    {
                        var poPos = s_cQry_POPosInfo(dbApp, currentBatchPos.ValueT.ValueT.ProdOrderPartslistPosID).FirstOrDefault();
                        if (poPos != null)
                        {
                            pm.OrderInfo.ValueT = BuildOrderString(poPos, pm);
                        }
                    }
                }
                else
                    pm.OrderInfo.ValueT = null;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (var entry in pm.Semaphore.ConnectionList)
                {
                    PWBase pwNode = entry.ValueT as PWBase;
                    if (pwNode == null)
                    {
                        //Error50170: Workflownode {0} is null.
                        Msg msg = new Msg(this, eMsgLevel.Error, ClassNameVB, "BuildAndSetOrderInfo(1)", 1010, "Error50170", "acUrl");

                        pm.OrderInfo.ValueT = msg.Message;
                        if (pm.IsAlarmActive(pm.OrderInfo, msg.Message) == null)
                            Messages.LogError(pm.GetACUrl(), "BuildAndSetOrderInfo(1)", msg.Message);
                        pm.OnNewAlarmOccurred(pm.OrderInfo, msg, true);

                        continue;
                    }
                    string orderInfo = BuildOrderInfo(pwNode);
                    if (!String.IsNullOrEmpty(orderInfo))
                        sb.AppendLine(orderInfo);
                    else
                    {
                        //Error50171: Order of {0} not found.
                        Msg msg = new Msg(this, eMsgLevel.Error, ClassNameVB, "BuildAndSetOrderInfo(1)", 1020, "Error50171", pwNode.GetACUrl());

                        pm.OrderInfo.ValueT = msg.Message;
                        if (pm.IsAlarmActive(pm.OrderInfo, msg.Message) == null)
                            Messages.LogError(pm.GetACUrl(), "BuildAndSetOrderInfo(1)", msg.Message);
                        pm.OnNewAlarmOccurred(pm.OrderInfo, msg, true);
                        continue;
                    }
                }

                pm.OrderInfo.ValueT = sb.ToString();
            }
            return pm.OrderInfo.ValueT;
        }

        public override string BuildOrderInfo(PWBase pw)
        {
            if (pw == null)
                return null;
            PAOrderInfo paOrderInfo = pw.GetPAOrderInfo();
            if (paOrderInfo == null)
                return null;
            var entry = paOrderInfo.Entities.Where(c => c.EntityName == ProdOrderBatch.ClassName).FirstOrDefault();
            if (entry != null)
            {
                using (var dbApp = new DatabaseApp())
                {
                    var prodOrderBatch = s_cQry_BatchInfo(dbApp, entry.EntityID).FirstOrDefault();
                    if (prodOrderBatch != null)
                        return BuildOrderString(prodOrderBatch, pw);
                }
            }
            entry = paOrderInfo.Entities.Where(c => c.EntityName == Picking.ClassName).FirstOrDefault();
            if (entry != null)
            {
                using (var dbApp = new DatabaseApp())
                {
                    var picking = s_cQry_PickingInfo(dbApp, entry.EntityID).FirstOrDefault();
                    if (picking != null)
                        return BuildOrderString(picking, pw);
                }
            }
            entry = paOrderInfo.Entities.Where(c => c.EntityName == DeliveryNotePos.ClassName).FirstOrDefault();
            if (entry != null)
            {
                using (var dbApp = new DatabaseApp())
                {
                    var dnPos = s_cQry_DeliveryNotePosInfo(dbApp, entry.EntityID).FirstOrDefault();
                    if (dnPos != null)
                        return BuildOrderString(dnPos, pw);
                }
            }
            entry = paOrderInfo.Entities.Where(c => c.EntityName == FacilityBooking.ClassName).FirstOrDefault();
            if (entry != null)
            {
                using (var dbApp = new DatabaseApp())
                {
                    var fb = s_cQry_FacilityBookingInfo(dbApp, entry.EntityID).FirstOrDefault();
                    if (fb != null)
                        return BuildOrderString(fb, pw);
                }
            }

            return null;
        }

        public virtual string BuildOrderString(ProdOrderPartslistPos posPos, PAProcessModule pm)
        {
            if (posPos == null)
                return "";
            return String.Format("O:{0},B:{2}\nP:{1}",
                posPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                posPos.ProdOrderPartslist.Partslist.Material.MaterialName1,
                posPos.ProdOrderBatch != null ? posPos.ProdOrderBatch.BatchSeqNo : 0);
        }

        public virtual string BuildOrderString(ProdOrderBatch prodOrderBatch, PWBase pw)
        {
            if (prodOrderBatch == null)
                return "";
            return String.Format("O:{0},B:{2}\nP:{1}",
                prodOrderBatch.ProdOrderPartslist.ProdOrder.ProgramNo,
                prodOrderBatch.ProdOrderPartslist.Partslist.Material.MaterialName1,
                prodOrderBatch.BatchSeqNo);
        }

        public virtual string BuildOrderString(Picking picking, PWBase pw)
        {
            if (picking == null)
                return "";
            PickingPos pickingPos = picking.PickingPos_Picking.FirstOrDefault();
            if (pickingPos != null && pickingPos.Material != null && pickingPos.ToFacility != null)
            {
                return String.Format("O:{0},Z:{2}\nP:{1}",
                    picking.PickingNo,
                    pickingPos.Material.MaterialName1,
                    pickingPos.ToFacility.FacilityName);
            }
            else
            {
                return String.Format("O:{0}",
                    picking.PickingNo);
            }
        }

        public virtual string BuildOrderString(DeliveryNotePos dnPos, PWBase pw)
        {
            if (dnPos == null)
                return "";
            return String.Format("O:{0}\nP:{1}",
                dnPos.DeliveryNote.DeliveryNoteNo,
                dnPos.Material.MaterialName1);
        }

        public virtual string BuildOrderString(FacilityBooking fb, PWBase pw)
        {
            if (fb == null)
                return "";
            Material material = fb.OutwardMaterial != null ? fb.OutwardMaterial : fb.InwardMaterial;
            if (material == null)
            {
                if (fb.OutwardFacility != null)
                    material = fb.OutwardFacility.Material;
                else if (fb.InwardFacility != null)
                    material = fb.InwardFacility.Material;
            }
            if (material == null && fb.OutwardFacilityCharge != null)
                material = fb.OutwardFacilityCharge.Material;
            if (material == null && fb.InwardFacilityCharge != null)
                material = fb.InwardFacilityCharge.Material;

            Facility outwardFacility = fb.OutwardFacility;
            if (outwardFacility == null && fb.OutwardFacilityCharge != null)
                outwardFacility = fb.OutwardFacilityCharge.Facility;
            Facility inwardFacility = fb.InwardFacility;
            if (inwardFacility == null && fb.InwardFacilityCharge != null)
                inwardFacility = fb.InwardFacilityCharge.Facility;

            return String.Format("O:{0},P:{1}\nO:{2}->I:{3}",
                fb.FacilityBookingNo,
                material != null ? material.MaterialName1 : "<?>",
                outwardFacility != null ? outwardFacility.FacilityNo : "<?>",
                inwardFacility != null ? inwardFacility.FacilityNo : "<?>");
        }

        public string BuildOrderString(ProdOrderPartslistPos batchPos, bool withShortBatchNo, bool useNewContext, PAProcessModule pm = null)
        {
            if (batchPos == null)
                return "";
            if (useNewContext)
            {
                using (var dbApp = new DatabaseApp())
                {
                    ProdOrderPartslistPos batchPos2 = batchPos.FromAppContext<ProdOrderPartslistPos>(dbApp);
                    if (batchPos2 != null)
                    {
                        return BuildOrderStringLocal(batchPos2, withShortBatchNo, useNewContext, pm);
                    }
                }
            }
            return BuildOrderStringLocal(batchPos, withShortBatchNo, useNewContext, pm);
        }

        private string BuildOrderStringLocal(ProdOrderPartslistPos batchPos, bool withShortBatchNo, bool useNewContext, PAProcessModule pm = null)
        {
            ProdOrderPartslist prodOrderPartsList = batchPos.ProdOrderPartslist;
            ProdOrderBatch prodOrderBatch = batchPos.ProdOrderBatch;
            if (prodOrderPartsList == null)
                return "";

            if (prodOrderBatch == null)
            {
                if (withShortBatchNo)
                    return String.Format("O:{0}\nP:{1}", prodOrderPartsList.ProdOrder.ProgramNo, prodOrderPartsList.Partslist.Material.MaterialName1);
                else
                {
                    var query = prodOrderPartsList.ProdOrderBatchPlan_ProdOrderPartslist.Where(c => c.PlanStateIndex == (short)GlobalApp.BatchPlanState.InProcess);
                    if (!query.Any())
                        return String.Format("O:{0}\nP:{1}", prodOrderPartsList.ProdOrder.ProgramNo, prodOrderPartsList.Partslist.Material.MaterialName1);
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        //foreach (ProdOrderBatchPlan plan in query)
                        //{
                        //    if (pm != null)
                        //    {
                        //        var selmods = plan.GetSelectedModules();
                        //        if (selmods == null)
                        //            continue;
                        //        if (!selmods.Where(c => c.VBiACClassID == pm.ACType.ACTypeID).Any())
                        //            continue;
                        //    }
                        //    if (plan.PlanMode == BatchPlanMode.UseBatchCount)
                        //        sb.AppendFormat("({0}/{1})", plan.BatchActualCount, plan.BatchTargetCount);
                        //    else if (plan.PlanMode == BatchPlanMode.UseTotalSize)
                        //        sb.AppendFormat("({0:0.}/{1:0.})", plan.ActualQuantity, plan.TotalSize);
                        //}
                        return String.Format("O:{0}-{2}\nP:{1}", prodOrderPartsList.ProdOrder.ProgramNo, prodOrderPartsList.Partslist.Material.MaterialName1, sb.ToString());
                    }
                }
            }
            else
            {
                if (withShortBatchNo)
                {
                    return "TODO";
                    //return String.Format("A:{0}/Batch:{1}({2})\nProdukt:{3}", prodOrderPartsList.ProdOrder.ProgramNo, prodOrderBatch.BatchSeqNo, prodOrderBatch.GetShortBatchNo(), prodOrderPartsList.Partslist.Material.MaterialName1);
                }
                else
                {
                    String strPlanInfo = "";
                    if (prodOrderBatch.ProdOrderBatchPlan != null)
                    {
                        prodOrderBatch.ProdOrderBatchPlan.AutoRefresh();
                        if (prodOrderBatch.ProdOrderBatchPlan.PlanMode == BatchPlanMode.UseBatchCount)
                        {
                            strPlanInfo = String.Format("({0}/{1})", prodOrderBatch.ProdOrderBatchPlan.BatchActualCount, prodOrderBatch.ProdOrderBatchPlan.BatchTargetCount);
                        }
                        else if (prodOrderBatch.ProdOrderBatchPlan.PlanMode == BatchPlanMode.UseTotalSize)
                        {
                            strPlanInfo = String.Format("({0:0.}/{1:0.})", prodOrderBatch.ProdOrderBatchPlan.ActualQuantity, prodOrderBatch.ProdOrderBatchPlan.TotalSize);
                        }
                    }
                    return String.Format("O:{0} / B:{1} {2}\nP:{3}", prodOrderPartsList.ProdOrder.ProgramNo, prodOrderBatch.BatchSeqNo, strPlanInfo, prodOrderPartsList.Partslist.Material.MaterialName1);
                }
            }
        }

        public override void ShowDialogOrder(IACComponent caller, PAOrderInfo orderInfo = null)
        {
            if (orderInfo == null)
            {
                PWNodeProxy pwNode = caller as PWNodeProxy;
                if (pwNode != null)
                {
                    orderInfo = pwNode.ACUrlCommand("!GetPAOrderInfo") as PAOrderInfo;
                }
                else
                {
                    PWBaseExecutable baseExe = caller as PWBaseExecutable;
                    if (baseExe != null)
                        orderInfo = baseExe.GetPAOrderInfo();
                }
            }

            base.ShowDialogOrder(caller, orderInfo);
        }

        public override bool IsEnabledShowDialogOrder(IACComponent caller)
        {
            bool result =  base.IsEnabledShowDialogOrder(caller);
            if (result)
                return result;

            PWNodeProxy pwNode = caller as PWNodeProxy;
            if (pwNode != null)
            {
                var orderInfo = pwNode.ACUrlCommand("!GetPAOrderInfo") as PAOrderInfo;
                if (orderInfo != null)
                    return true;
            }
            else
            {
                PWBaseExecutable baseExe = caller as PWBaseExecutable;
                if (baseExe != null)
                {
                    var orderInfo = baseExe.GetPAOrderInfo();
                    if (orderInfo != null)
                        return true;
                }
            }

            return false;
        }


        #endregion

        #endregion

    }
}
