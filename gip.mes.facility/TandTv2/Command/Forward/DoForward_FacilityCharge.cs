using System.Collections.Generic;
using gip.mes.datamodel;
using System.Linq;
using iplusContext = gip.core.datamodel;
using System;

namespace gip.mes.facility
{
    public class DoForward_FacilityCharge : DoBackward_FacilityCharge
    {

        #region ctor's

        public DoForward_FacilityCharge(DatabaseApp databaseApp, TandTv2Result result, FacilityCharge item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {
        }

        #endregion

        #region overriden methods
        public override List<IDoItem> ProcessRelatedSameStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            if (Item.FacilityLotID == null) return null;
            return new List<IDoItem>(){ new DoForward_FacilityLot(databaseApp, result, Item.FacilityLot, jobFilter) };
        }


        public override List<IDoItem> SearchRelatedNextStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            List<IDoItem> related = new List<IDoItem>();
            IEnumerable<FacilityBookingCharge> fbcQuery = Item.FacilityBookingCharge_OutwardFacilityCharge;
            fbcQuery = fbcQuery.Where(c => c.OutwardFacilityLotID != null);
            fbcQuery = fbcQuery.Where(c => TandTv2Filters.filterFBCFromToForMaterial(c, jobFilter.MaterialIDs, jobFilter.FilterDateFrom, jobFilter.FilterDateTo));
            List<FacilityBookingCharge> facilityBookingCharges = fbcQuery.ToList();
            related.AddRange(facilityBookingCharges.Select(c => new DoForward_FacilityBookingCharge(databaseApp, result, c, jobFilter)).ToList());
            return related;
        }
        #endregion

    }
}
