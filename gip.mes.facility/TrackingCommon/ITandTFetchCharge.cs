using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;

namespace gip.mes.facility
{
    public interface ITandTFetchCharge: IACObject
    {
        List<FacilityCharge> GetFacilityChargesBackward(DatabaseApp databaseApp, FacilityBooking facilityBooking, string userInitials, Func<object, bool> breakTrackingCondition = null);
    }
}
