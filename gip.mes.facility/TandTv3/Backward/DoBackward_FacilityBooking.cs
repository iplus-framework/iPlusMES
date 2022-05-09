using gip.core.datamodel;
using gip.mes.datamodel;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class DoBackward_FacilityBooking : ItemTracking<FacilityBooking>
    {
        #region ctor's
        public DoBackward_FacilityBooking(DatabaseApp databaseApp, TandTResult result, FacilityBooking item) : base(databaseApp, result, item)
        {
            ItemTypeName = "FacilityBooking";
            if (!Result.Ids.Keys.Contains(item.FacilityBookingID))
                Result.Ids.Add(item.FacilityBookingID, ItemTypeName);
        }
        #endregion

        #region IItemTracking

        public override List<IACObjectEntity> GetSameStepItems()
        {
            return Item
                .FacilityBookingCharge_FacilityBooking
                .Where(c =>
                    TandTv3Query.s_cQry_FBCOutwardQuery(c, Result.Filter) ||
                    TandTv3Query.s_cQry_FBCInwardQuery(c, Result.Filter, null, null, true)
                )
                .OrderBy(c => c.FacilityBookingChargeNo)
                .ToList()
                .Select(c => (IACObjectEntity)c)
                .ToList();
        }

        #endregion
    }
}
