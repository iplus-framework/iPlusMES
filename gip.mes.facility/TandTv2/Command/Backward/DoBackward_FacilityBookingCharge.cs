using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    public class DoBackward_FacilityBookingCharge : DoItem<FacilityBookingCharge>
    {

        #region ctor's

        public DoBackward_FacilityBookingCharge(DatabaseApp dbApp, TandTv2Result result, FacilityBookingCharge item, TandTv2Job jobFilter) : base(dbApp, result, item, jobFilter)
        {

        }

        #endregion

        #region methods

        public override TandTv2StepItem Factory_StepItem(TandTv2Result result, TandTv2Step step)
        {
            TandTv2StepItem stepItem = base.Factory_StepItem(result, step);
            stepItem.PrimaryKeyID = Item.FacilityBookingChargeID;
            stepItem.TandT_ItemTypeEnum = TandTv2ItemTypeEnum.FacilityBookingCharge;
            stepItem.FacilityBookingCharge = Item;
            stepItem.InsertDate = Item.InsertDate;
            return stepItem;
        }

        public override string Factory_StepItem_ACIdentifier()
        {
            return Item.FacilityBookingChargeNo;
        }

        public override string Factory_StepItem_ACCaptionTranslation()
        {
            Facility facility = Item.InwardFacility != null ? Item.InwardFacility : Item.OutwardFacility;
            Material material = Item.InwardMaterial != null ? Item.InwardMaterial : Item.OutwardMaterial;
            FacilityCharge facilityCharge = null;
            string lotNo = "-";
            if (Item.InwardFacilityChargeID != null || Item.OutwardFacilityChargeID != null)
            {
                facilityCharge = Item.InwardFacilityChargeID != null ? Item.InwardFacilityCharge : Item.OutwardFacilityCharge;
                if (facilityCharge != null && facilityCharge.FacilityLotID != null)
                    lotNo = facilityCharge.FacilityLot.LotNo;
            }
            return string.Format(@"en{{'[{0}] | {1} | ({2}) | [{3}] {4}'}}de{{'[{5}] | {1} | ({2}) | [{3}] {4}'}}",
                Item.InwardMaterial != null ? "Inward" : "Outward",
                lotNo,
                facility.FacilityNo,
                material.MaterialNo,
                material.MaterialName1,
                Item.InwardMaterial != null ? "Eingang" : "Ausgang");
        }

        public override List<IDoItem> ProcessRelatedSameStep(DatabaseApp dbApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            List<IDoItem> doItems = new List<IDoItem>();
            doItems.Add(new DoBackward_FacilityBooking(dbApp, result, Item.FacilityBooking, jobFilter));
            if (Item.InwardFacilityChargeID != null || Item.OutwardFacilityChargeID != null)
            {
                FacilityCharge facilityCharge = Item.InwardFacilityChargeID != null ? Item.InwardFacilityCharge : Item.OutwardFacilityCharge;
                doItems.Add(new DoBackward_FacilityCharge(dbApp, result, facilityCharge, jobFilter));
            }
            Facility facility = Item.InwardFacilityID != null ? Item.InwardFacility : Item.OutwardFacility;
            doItems.Add(new DoBackward_Facility(dbApp, result, facility, jobFilter));
            return doItems;
        }

        public override List<IDoItem> SearchRelatedNextStep(DatabaseApp dbApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            List<IDoItem> related = new List<IDoItem>();
            if (Item.ProdOrderPartslistPosID != null)
                related.Add(new DoBackward_ProdOrderPartslistPos(dbApp, result, Item.ProdOrderPartslistPos, jobFilter));

            if (Item.ProdOrderPartslistPosRelationID != null)
                related.Add(new DoBackward_ProdOrderPartslistPosRelation(dbApp, result, Item.ProdOrderPartslistPosRelation, jobFilter));

            if (Item.InOrderPosID != null)
                related.Add(new DoBackward_InOrderPos(dbApp, result, Item.InOrderPos, jobFilter));

            var fbcQuery =
                Item
                .FacilityBooking.FacilityBookingCharge_FacilityBooking
                .Where(c => 
                    c.FacilityBookingChargeID != Item.FacilityBookingChargeID && 
                    c.ProdOrderPartslistPosRelationID != null &&
                    TandTv2Filters.filterFBCFromToForMaterial(c, jobFilter.MaterialIDs, jobFilter.FilterDateFrom, jobFilter.FilterDateTo));
            if (fbcQuery.Any())
                related.AddRange(fbcQuery.Select(c=> new DoBackward_FacilityBookingCharge(dbApp, result, c, jobFilter)).ToList());

            return related;
        }

        #endregion
    }
}
