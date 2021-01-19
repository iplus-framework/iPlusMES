using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoForward_FacilityPreBooking : DoBackward_FacilityPreBooking
    {

        #region ctor's
        public DoForward_FacilityPreBooking(DatabaseApp databaseApp, TandTv2Result result, FacilityPreBooking item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }
        #endregion

        #region Overriden methods
        public override List<IDoItem> ProcessRelatedSameStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            List<IDoItem> doItems = new List<IDoItem>();
            if (Item.InwardFacilityCharge != null || Item.OutwardFacilityCharge != null)
            {
                FacilityCharge facilityCharge = Item.InwardFacilityCharge != null ? Item.InwardFacilityCharge : Item.OutwardFacilityCharge;
                doItems.Add(new DoForward_FacilityCharge(databaseApp, result, facilityCharge, jobFilter));
            }
            return doItems;
        }

        public override List<IDoItem> SearchRelatedNextStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            List<IDoItem> related = new List<IDoItem>();
            if (Item.ProdOrderPartslistPosID != null)
                related.Add(new DoForward_ProdOrderPartslistPos(databaseApp, result, Item.ProdOrderPartslistPos, jobFilter));

            if (Item.ProdOrderPartslistPosRelationID != null)
                related.Add(new DoForward_ProdOrderPartslistPosRelation(databaseApp, result, Item.ProdOrderPartslistPosRelation, jobFilter));

            if (Item.InOrderPosID != null)
                related.Add(new DoForward_InOrderPos(databaseApp, result, Item.InOrderPos, jobFilter));

            if (Item.OutOrderPosID != null)
                related.Add(new DoForward_OutOrderPos(databaseApp, result, Item.OutOrderPos, jobFilter));

            return related;
        }
        #endregion

    }
}
