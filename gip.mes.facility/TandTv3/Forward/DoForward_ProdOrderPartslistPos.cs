using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class DoForward_ProdOrderPartslistPos : DoBackward_ProdOrderPartslistPos
    {

        #region ctor's

        public DoForward_ProdOrderPartslistPos(DatabaseApp databaseApp, TandTResult result, ProdOrderPartslistPos item) : base(databaseApp, result, item)
        {
            TrackingDirection = MDTrackingDirectionEnum.Forward;
        }

        #endregion

        #region POS Operations

        #region POS Operations -> SameStep

        // section one
        public override List<IACObjectEntity> OperateSameStepItems_InwardIntern()
        {
            List<IACObjectEntity> sameStepItems = OperateSameStepItems_OutwardRoot();
            if(Item.IsFinalMixure)
            {

            }
            return sameStepItems;
        }

        public override List<IACObjectEntity> OperateSameStepItems_InwardPartIntern()
        {
           
            return new List<IACObjectEntity>();
        }

        public override List<IACObjectEntity> OperateSameStepItems_OutwardRoot()
        {
            List<IACObjectEntity> sameStepItems = new List<IACObjectEntity>();
            List<ProdOrderPartslistPosRelation> relations = new List<ProdOrderPartslistPosRelation>();
            relations =
                Item.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                    .SelectMany(c => c.FacilityBookingCharge_ProdOrderPartslistPosRelation)
                    .Where(c =>
                        c.OutwardFacilityChargeID != null &&
                        c.OutwardFacilityCharge.FacilityLotID != null &&
                        Result.Lots.Contains(c.OutwardFacilityCharge.FacilityLot.LotNo)
                     )
                    .Select(c => c.ProdOrderPartslistPosRelation).ToList();
            sameStepItems.AddRange(relations);
            return sameStepItems;
        }

        #endregion

        #region POS Operations -> NextStep

        public override List<IACObjectEntity> OperateNextStepItems_InwardPartIntern()
        {
            List<IACObjectEntity> nextStepItems = new List<IACObjectEntity>();
            List<FacilityBookingCharge> fbcs = Item.FacilityBookingCharge_ProdOrderPartslistPos.ToList();
            if (fbcs.Any())
            {
                nextStepItems.AddRange(fbcs);
            }
            else
            {
                List<ProdOrderPartslistPosRelation> relations =
                    Item.ProdOrderPartslistPos1_ParentProdOrderPartslistPos
                    .ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                    .Where(c => c.TargetProdOrderPartslistPos.ProdOrderBatchID != null &&
                    Result.BatchIDs.Contains(c.ProdOrderBatchID ?? Guid.Empty))
                    .ToList();
                nextStepItems.AddRange(relations);
            }
            return nextStepItems;
        }

        public override List<IACObjectEntity> OperateNextStepItems_InwardIntern()
        {
            List<IACObjectEntity> nextStepItems = new List<IACObjectEntity>();
            if (Item.IsFinalMixure)
            {
                string materialNo = Item.BookingMaterial.MaterialNo;

                List<ProdOrderPartslistPos> outwardRootPositionsUsingMixure
                    = Item
                    .ProdOrderPartslist
                    .ProdOrder
                    .ProdOrderPartslist_ProdOrder
                    .Where(c => c.Sequence > Item.ProdOrderPartslist.Sequence)
                    .SelectMany(c => c.ProdOrderPartslistPos_ProdOrderPartslist)
                    .Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot && c.SourceProdOrderPartslistID == Item.ProdOrderPartslistID)
                    .ToList();
                nextStepItems.AddRange(outwardRootPositionsUsingMixure);
            }
            return nextStepItems;
        }

        public override List<IACObjectEntity> OperateNextStepItems_OutwardRoot()
        {
            List<IACObjectEntity> nextStepItems = new List<IACObjectEntity>();
            return nextStepItems;
        }

        #endregion

        #endregion
    }
}