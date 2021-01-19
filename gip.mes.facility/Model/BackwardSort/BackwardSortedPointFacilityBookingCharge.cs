using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{
    public class BackwardSortedPointFacilityBookingCharge : TandTPointSorted
    {

        public BackwardSortedPointFacilityBookingCharge(DatabaseApp dbApp, TandTResult result, TandTResult sortedResult, TandTPoint parentPoint, TandTPoint originalPoint, TandTSortingShemaEnum sortingShema, TandTFilter filter)
            : base(dbApp, result, sortedResult, parentPoint, originalPoint, sortingShema, filter)
        {

        }

        public override void ProcessSortedRelated(DatabaseApp dbApp, TandTResult result, TandTResult sortedResult, TandTPoint originalPoint, TandTSortingShemaEnum sortingShema)
        {
            FacilityBookingCharge fbc = originalPoint.Item as FacilityBookingCharge;
            if (fbc.OutwardFacilityChargeID != null)
            {
                // we search ProdOrder
            }
            else if (fbc.ProdOrderPartslistPosID != null)
            {
                // test
            }
            else if (fbc.InwardFacilityChargeID != null)
            {
                // test tt 
            }
        }
    }
}
