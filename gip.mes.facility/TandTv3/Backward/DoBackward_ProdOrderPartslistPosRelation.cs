using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class DoBackward_ProdOrderPartslistPosRelation : ItemTracking<ProdOrderPartslistPosRelation>
    {

        #region ctor's


        public DoBackward_ProdOrderPartslistPosRelation(DatabaseApp databaseApp, TandTResult result, ProdOrderPartslistPosRelation item) : base(databaseApp, result, item)
        {
            ItemTypeName = "ProdOrderPartslistPosRelation";
            if (!Result.Ids.Keys.Contains(item.ProdOrderPartslistPosRelationID))
                Result.Ids.Add(item.ProdOrderPartslistPosRelationID, ItemTypeName);
            if (Item.TargetProdOrderPartslistPos.ProdOrderBatchID != null && !Result.BatchIDs.Contains(Item.TargetProdOrderPartslistPos.ProdOrderBatchID ?? Guid.Empty))
                Result.BatchIDs.Add(Item.TargetProdOrderPartslistPos.ProdOrderBatchID ?? Guid.Empty);
            IsUseLotCheck = UseLotCheck(Item);
        }

        #endregion

        public bool IsUseLotCheck { get; set; }

        #region IItemTracking

        #region IItemtracking -> SameStepItem

        public override List<IACObjectEntity> GetSameStepItems()
        {
            List<IACObjectEntity> sameStepItems = new List<IACObjectEntity>();
            List<FacilityBookingCharge> fbc =
           Item
           .FacilityBookingCharge_ProdOrderPartslistPosRelation
           .Where(c => TandTv3Query.s_cQry_FBCOutwardQuery(c, Result.Filter))
           .OrderBy(c => c.FacilityBookingChargeNo)
           .ToList();
            if(IsUseLotCheck)
            {
                fbc = fbc.Where(c => Result.Lots.Contains(c.OutwardFacilityCharge.FacilityLot.LotNo)).ToList();
            }
            sameStepItems.AddRange(fbc);
            return sameStepItems;
        }

        #endregion

        #region IItemTracking -> NextStepItems

        public override List<IACObjectEntity> GetNextStepItems()
        {
            List<IACObjectEntity> nextStepItems = new List<IACObjectEntity>();
            nextStepItems.Add(Item.SourceProdOrderPartslistPos);
            return nextStepItems;
        }

        #endregion

        public override void AssignItemToMixPoint(List<IACObjectEntity> sameStepItems)
        {
            ProdOrderPartslistPos pos = Item.TargetProdOrderPartslistPos;
            if (pos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardPartIntern)
            {
                TandTv3Point mixPoint = Result.GetMixPoint(Step, pos);
                if (!mixPoint.Relations.Select(c => c.ProdOrderPartslistPosRelationID).Contains(Item.ProdOrderPartslistPosRelationID))
                    mixPoint.Relations.Add(Item);
            }
        }

        public bool AlternativeRelation { get; set; }

        #endregion

        #region helper methods

        public virtual bool UseLotCheck(ProdOrderPartslistPosRelation relation)
        {
            bool useLotCheck = Item.FacilityBooking_ProdOrderPartslistPosRelation.Any(c => c.ProdOrderPartslistPosFacilityLotID != null);
            Console.WriteLine(@"T&Tv3 UseLotCheck REL:{0} -> Check:{1}", relation.ToString(), useLotCheck);
            return useLotCheck;
        }

        #endregion

        #region Overrides
        public override string ToString()
        {
            return string.Format(@"{0}| IsUseLotCheck: {1}", base.ToString(), IsUseLotCheck);
        }
        #endregion
    }
}
