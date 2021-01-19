using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoForward_FacilityBookingCharge : DoBackward_FacilityBookingCharge
    {

        #region ctor's

        public DoForward_FacilityBookingCharge(DatabaseApp databaseApp, TandTv2Result result, FacilityBookingCharge item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion

        #region Overriden methods
        public override List<IDoItem> ProcessRelatedSameStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            List<IDoItem> doItems = new List<IDoItem>();
            if (Item.InwardFacilityChargeID != null || Item.OutwardFacilityChargeID != null)
            {
                FacilityCharge facilityCharge = Item.InwardFacilityCharge != null ? Item.InwardFacilityCharge : Item.OutwardFacilityCharge;
                doItems.Add(new DoForward_FacilityCharge(databaseApp, result, facilityCharge, jobFilter));
            }
            doItems.Add(new DoForward_FacilityBooking(databaseApp, result, Item.FacilityBooking, jobFilter));
            Facility facility = Item.InwardFacilityID != null ? Item.InwardFacility : Item.OutwardFacility;
            if (facility != null)
                doItems.Add(new DoForward_Facility(databaseApp, result, facility, jobFilter));
            return doItems;
        }

        public override List<IDoItem> SearchRelatedNextStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            List<IDoItem> related = new List<IDoItem>();
            if (Item.ProdOrderPartslistPosID != null)
                related.Add(new DoForward_ProdOrderPartslistPos(databaseApp, result, Item.ProdOrderPartslistPos, jobFilter));

            if (Item.ProdOrderPartslistPosRelationID != null)
                related.Add(new DoForward_ProdOrderPartslistPosRelation(databaseApp, result, Item.ProdOrderPartslistPosRelation, jobFilter));

            if (Item.OutOrderPosID != null)
                related.Add(new DoForward_OutOrderPos(databaseApp, result, Item.OutOrderPos, jobFilter));

            return related;
        }
        #endregion

    }
}
