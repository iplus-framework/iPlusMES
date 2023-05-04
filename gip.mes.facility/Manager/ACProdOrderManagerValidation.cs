using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace gip.mes.facility
{
    public partial class ACProdOrderManager
    {
        #region Methods

        #region Public

        #region Public -> Partslist validation
        public MsgWithDetails ValidateProdOrderPartslist(DatabaseApp dbApp, ACPartslistManager partsListManager, ProdOrderPartslist poList)
        {
            MsgWithDetails detailMessages = new MsgWithDetails();
            foreach (ProdOrderPartslistPos pos in poList.ProdOrderPartslistPos_ProdOrderPartslist.Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot))
            {
                pos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.AutoRefresh(pos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPosReference, pos);
                pos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.AutoRefresh(pos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPosReference, pos);
                if (!pos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Any())
                {
                    // Stücklistenposition {0} {1} {2} ist keinem Zwischenmaterial zugeordnet.
                    detailMessages.AddDetailMessage(new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Warning,
                        ACIdentifier = "ValidateStart(1)",
                        Message = Root.Environment.TranslateMessage(partsListManager, "Warning50013", pos.Sequence, pos.Material.MaterialNo, pos.MaterialName)
                    });
                }
            }
            return detailMessages;
        }

        #endregion

        #region Public ->Batch

        #region Public -> Batch -> Validation
        public MsgWithDetails ValidateBatchPlan(DatabaseApp dbApp, ProdOrderBatchPlan prodOrderBatchPlan)
        {
            MsgWithDetails detailMessages = new MsgWithDetails();
            bool hasBatchPlansWithoutDestination = !prodOrderBatchPlan.FacilityReservation_ProdOrderBatchPlan.Any();
            if (hasBatchPlansWithoutDestination)
            {
                //Bei einem oder mehreren Batchplänen wurde kein Ziel eingetragen. Wollen Sie dennoch starten?
                detailMessages.AddDetailMessage(new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Warning,
                    ACIdentifier = "ValidateBatchPlan(10)",
                    Message = Root.Environment.TranslateMessage(this, "Warning50008")
                });
            }

            if (!prodOrderBatchPlan.IsValidated)
            {

            }
            return detailMessages;
        }

        public virtual MsgWithDetails ValidateStart(DatabaseApp dbApp, Database dbiPlus,
            ACComponent invoker,
            ProdOrderPartslist poList, List<IACConfigStore> configStores,
            PARole.ValidationBehaviour validationBehaviour,
            ACPartslistManager partsListManager, ACMatReqManager matReqManager)
        {
            MsgWithDetails detailMessages = new MsgWithDetails();
            if (!poList.ProdOrderBatchPlan_ProdOrderPartslist.Any())
                return detailMessages;

            if (!poList.Partslist.IsEnabled)
            {
                // Error50298: Bill of Material is not released.
                detailMessages.AddDetailMessage(new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "ValidateStart(10)",
                    Message = Root.Environment.TranslateMessage(this, "Error50298")
                });
            }

            if (   (poList.Partslist.EnabledFrom.HasValue && DateTime.Now < poList.Partslist.EnabledFrom.Value)
                || (poList.Partslist.EnabledTo.HasValue && DateTime.Now > poList.Partslist.EnabledTo.Value))
            {
                // Error50299: The validity period of the BOM has been exceeded.
                detailMessages.AddDetailMessage(new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "ValidateStart(20)",
                    Message = Root.Environment.TranslateMessage(this, "Error50299")
                });
            }

            bool hasBatchPlansWithoutDestination = poList.ProdOrderBatchPlan_ProdOrderPartslist.Where(c => !c.FacilityReservation_ProdOrderBatchPlan.Any()).Any();
            if (hasBatchPlansWithoutDestination)
            {
                //Bei einem oder mehreren Batchplänen wurde kein Ziel eingetragen. Wollen Sie dennoch starten?
                detailMessages.AddDetailMessage(new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Warning,
                    ACIdentifier = "ValidateStart(30)",
                    Message = Root.Environment.TranslateMessage(this, "Warning50008")
                });
            }

            foreach (ProdOrderPartslistPos pos in poList.ProdOrderPartslistPos_ProdOrderPartslist.Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot))
            {
                pos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.AutoRefresh(pos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPosReference, pos);
                pos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.AutoRefresh(pos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPosReference, pos);
                if (!pos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Any())
                {
                    // Stücklistenposition {0} {1} {2} ist keinem Zwischenmaterial zugeordnet.
                    detailMessages.AddDetailMessage(new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Warning,
                        ACIdentifier = "ValidateStart(40)",
                        Message = Root.Environment.TranslateMessage(partsListManager, "Warning50013", pos.Sequence, pos.Material != null ? pos.Material.MaterialNo : "", pos.MaterialName)
                    });
                }
            }

            bool hasNotValidatedPos = poList.ProdOrderBatchPlan_ProdOrderPartslist.Where(c => !c.IsValidated).Any();

            if (hasNotValidatedPos)
            {
                try
                {
                    partsListManager.CheckResourcesAndRoutingEvent += PartsListManager_CheckResourcesAndRoutingEvent;
                    partsListManager.CheckResourcesAndRouting(dbApp, dbiPlus, poList, configStores, validationBehaviour, detailMessages);
                }
                catch (Exception ec)
                {
                    Msg errMsg = new Msg() { MessageLevel = eMsgLevel.Error, Message = ec.Message };
                    detailMessages.AddDetailMessage(errMsg);
                }
                finally
                {
                    partsListManager.CheckResourcesAndRoutingEvent -= PartsListManager_CheckResourcesAndRoutingEvent;
                }
            }

            if (matReqManager != null && IsActiveMatReqCheck)
                CheckMaterialReq(dbApp, poList, matReqManager, ref detailMessages);

            return detailMessages;
        }

        protected virtual void PartsListManager_CheckResourcesAndRoutingEvent(object sender, CheckResourcesAndRoutingEventArgs e)
        {
        }

        public int SetBatchPlanValidated(DatabaseApp dbApp, ProdOrderPartslist poList)
        {
            int changedEntries = 0;
            var notValidatedEntries = poList.ProdOrderBatchPlan_ProdOrderPartslist.Where(c => !c.IsValidated);
            foreach (var notValidatedPos in notValidatedEntries)
            {
                notValidatedPos.IsValidated = true;
                changedEntries++;
            }
            return changedEntries;
        }

        public static bool IsEnabledStartBatchPlan(ProdOrderBatchPlan prodOrderBatchPlan, ProdOrderPartslist prodOrderPartslist, ProdOrderPartslistPos intermediate)
        {

            bool isBatchNotValid = true;
            if(prodOrderBatchPlan != null && prodOrderPartslist != null && intermediate != null && prodOrderBatchPlan != null)
            {
                bool prodOrderorPartslistStarted =
                    prodOrderPartslist.ProdOrder.MDProdOrderState.ProdOrderState >= MDProdOrderState.ProdOrderStates.ProdFinished
                    || prodOrderPartslist.MDProdOrderState.ProdOrderState >= MDProdOrderState.ProdOrderStates.ProdFinished;

                bool batchCompletedOrInStartOrder = 
                    (
                        intermediate.MDProdOrderPartslistPosState != null
                        && (
                                intermediate.MDProdOrderPartslistPosState.ProdOrderPartslistPosState >= MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                                && intermediate.MDProdOrderPartslistPosState.ProdOrderPartslistPosState < MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.AutoStart
                            )
                    )
                    || prodOrderBatchPlan.PlanState >= GlobalApp.BatchPlanState.AutoStart;


                bool batchPlanInvalidOrWithoutReservation = !prodOrderBatchPlan.FacilityReservation_ProdOrderBatchPlan.Any() || !prodOrderBatchPlan.IsValid;

                isBatchNotValid = prodOrderorPartslistStarted || batchCompletedOrInStartOrder || batchPlanInvalidOrWithoutReservation;
            }
            return !isBatchNotValid;
        }

        #endregion

        #endregion
        #endregion

        private void CheckMaterialReq(DatabaseApp dbApp, ProdOrderPartslist poList, ACMatReqManager matReqManager, ref MsgWithDetails msg)
        {
            if (matReqManager == null)
                return;

            var result = matReqManager.CheckMaterialsRequirement(dbApp, poList.ProdOrderBatchPlan_ProdOrderPartslist);

            foreach (var item in result)
                msg.AddDetailMessage(item);
        }

        #endregion

    }
}
