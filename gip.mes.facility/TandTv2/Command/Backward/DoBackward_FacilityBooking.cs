using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoBackward_FacilityBooking : DoItem<FacilityBooking>
    {
        #region ctor's 

        public DoBackward_FacilityBooking(DatabaseApp dbApp, TandTv2Result result, FacilityBooking item, TandTv2Job jobFilter) : base(dbApp, result, item, jobFilter)
        {

        }

        #endregion


        #region methods

        public override TandTv2StepItem Factory_StepItem(TandTv2Result result, TandTv2Step step)
        {
            TandTv2StepItem stepItem = base.Factory_StepItem(result, step);
            stepItem.PrimaryKeyID = Item.FacilityBookingID;
            stepItem.TandT_ItemTypeEnum = TandTv2ItemTypeEnum.FacilityBooking;
            stepItem.FacilityBooking = Item;
            stepItem.InsertDate = Item.InsertDate;
            return stepItem;
        }

        public override string Factory_StepItem_ACIdentifier()
        {
            return Item.FacilityBookingNo; 
        }

        public override string Factory_StepItem_ACCaptionTranslation()
        {
            Facility facility = Item.InwardFacility != null ? Item.InwardFacility : Item.OutwardFacility;
            FacilityBookingCharge facilityBookingCharge = Item.FacilityBookingCharge_FacilityBooking.FirstOrDefault();
            if (facilityBookingCharge != null && facility == null)
                facility = facilityBookingCharge.InwardFacility != null ? facilityBookingCharge.InwardFacility : facilityBookingCharge.OutwardFacility;
            Material material = Item.InwardMaterial != null ? Item.InwardMaterial : Item.OutwardMaterial;
            if(facilityBookingCharge != null && material == null)
                material = facilityBookingCharge.InwardMaterial != null ? facilityBookingCharge.InwardMaterial : facilityBookingCharge.OutwardMaterial;
            if (facility == null || material == null) return Item.FacilityBookingNo;
            return string.Format(@"en{{'[{0}] ({1}) [{2}] {3}'}}de{{'[{4}] ({1}) [{2}] {3}'}}",
                Item.InwardMaterial != null ? "Inward" : "Outward",
                facility.FacilityNo,
                material.MaterialNo,
                material.MaterialName1,
                Item.InwardMaterial != null ? "Eingang" : "Ausgang"
            );
        }



        #endregion
    }
}
