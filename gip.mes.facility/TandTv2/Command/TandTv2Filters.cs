using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility
{
    public static class TandTv2Filters
    {
        public static Func<FacilityBookingCharge, DateTime?, DateTime?, bool> filterFBCFromTo = (facilityBookingCharge, filterDateFrom, filterDateTo) =>
         {
             return (filterDateFrom == null || facilityBookingCharge.InsertDate >= filterDateFrom) &&
                     (filterDateTo == null || facilityBookingCharge.InsertDate < filterDateTo);
         };

        public static Func<FacilityBookingCharge, List<Guid>, DateTime?, DateTime?, bool> filterFBCFromToForMaterial = (fbc, materialIDs, filterDateFrom, filterDateTo) =>
         {
             bool checkByMaterial = 
                materialIDs != null && 
                materialIDs.Any() && 
                (filterFBCFromTo != null || filterDateTo != null) &&
                fbc.OutwardMaterialID != null
                && materialIDs.Contains(fbc.OutwardMaterialID ?? Guid.Empty);
             if (checkByMaterial)
                 return (
                         filterDateFrom == null || fbc.InsertDate >= filterDateFrom) &&
                         (filterDateTo == null || fbc.InsertDate < filterDateTo);
             else
                 return true;
         };
       
    }
}
