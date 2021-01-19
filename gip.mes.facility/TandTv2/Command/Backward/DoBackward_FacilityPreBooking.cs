using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoBackward_FacilityPreBooking : DoItem<FacilityPreBooking>
    {

        #region ctor's
        public DoBackward_FacilityPreBooking(DatabaseApp databaseApp, TandTv2Result result, FacilityPreBooking item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }
        #endregion


        #region overriden methods

        public override TandTv2StepItem Factory_StepItem(TandTv2Result result, TandTv2Step step)
        {
            TandTv2StepItem stepItem = base.Factory_StepItem(result, step);
            stepItem.PrimaryKeyID = Item.FacilityPreBookingID;
            stepItem.TandT_ItemTypeEnum = TandTv2ItemTypeEnum.FacilityPreBooking;
            stepItem.InsertDate = Item.InsertDate;
            stepItem.FacilityPreBooking = Item;
            return stepItem;
        }

        public override string Factory_StepItem_ACIdentifier()
        {
            return Item.FacilityPreBookingNo;
        }

        public override string Factory_StepItem_ACCaptionTranslation()
        {
            Facility facility = Item.InwardFacility != null ? Item.InwardFacility : Item.OutwardFacility;
            Material material = Item.InwardMaterial != null ? Item.InwardMaterial : Item.OutwardMaterial;
            if (facility == null || material == null) return Item.FacilityPreBookingNo;
            return string.Format(@"en{{'[{0}] ({1}) [{2}] {3}'}}de{{'[{4}] ({1}) [{2}] {3}'}}",
                Item.InwardMaterial != null ? "Inward" : "Outward",
                facility.FacilityNo,
                material.MaterialNo,
                material.MaterialName1,
                Item.InwardMaterial != null ? "Eingang" : "Ausgang"
            );
        }

        public override List<IDoItem> ProcessRelatedSameStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            List<IDoItem> doItems = new List<IDoItem>();
            if (Item.InwardFacilityCharge != null || Item.OutwardFacilityCharge != null)
            {
                FacilityCharge facilityCharge = Item.InwardFacilityCharge != null ? Item.InwardFacilityCharge : Item.OutwardFacilityCharge;
                doItems.Add(new DoBackward_FacilityCharge(databaseApp, result, facilityCharge, jobFilter));
            }
            return doItems;
        }

        public override List<IDoItem> SearchRelatedNextStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            List<IDoItem> related = new List<IDoItem>();
            if (Item.ProdOrderPartslistPosID != null)
                related.Add(new DoBackward_ProdOrderPartslistPos(databaseApp, result, Item.ProdOrderPartslistPos, jobFilter));

            if (Item.ProdOrderPartslistPosRelationID != null)
                related.Add(new DoBackward_ProdOrderPartslistPosRelation(databaseApp, result, Item.ProdOrderPartslistPosRelation, jobFilter));

            if (Item.InOrderPosID != null)
                related.Add(new DoBackward_InOrderPos(databaseApp, result, Item.InOrderPos, jobFilter));

            return related;
        }

        #endregion
    }
}
