using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{
    public class BackwardSortedPointInOrderPos : TandTPointSorted
    {

        public BackwardSortedPointInOrderPos(DatabaseApp dbApp, TandTResult result, TandTResult sortedResult, TandTPoint parentPoint, TandTPoint originalPoint, TandTSortingShemaEnum sortingShema, TandTFilter filter)
            : base(dbApp, result, sortedResult, parentPoint, originalPoint, sortingShema, filter)
        {

        }

        public override void ProcessSortedRelated(DatabaseApp dbApp, TandTResult result, TandTResult sortedResult, TandTPoint originalPoint, TandTSortingShemaEnum sortingShema)
        {
            InOrderPos ips = originalPoint.Item as InOrderPos;

            // Add delivery notes
            if (ips.DeliveryNotePos_InOrderPos.Any())
            {
                List<Guid> deliveryNoteIDs = (originalPoint.Item as InOrderPos).DeliveryNotePos_InOrderPos.Select(x => x.DeliveryNotePosID).ToList();

                if (deliveryNoteIDs.Any())
                {
                    List<TandTPoint> deliveryNotePoints = result
                        .Results
                        .Where(x => deliveryNoteIDs.Contains(x.ID))
                        .OrderBy(x => (x.Item as DeliveryNote).DeliveryDate)
                        .ToList();
                    foreach (TandTPoint dnPoint in deliveryNotePoints)
                    {
                        new BackwardSortedPointDeliveryNotePos(dbApp, result, sortedResult, this, dnPoint, sortingShema, Filter);
                    }
                }
            }

            // Add facility booking
            if (ips.FacilityBooking_InOrderPos.Any())
            {
                List<Guid> fbIds = ips.FacilityBooking_InOrderPos.Select(x => x.FacilityBookingID).ToList();
                List<TandTPoint> fbRelatedToIoP = result
                    .Results
                    .Where(x => fbIds.Contains(x.ID))
                    .OrderBy(x => (x.Item as FacilityBooking).StorageDate)
                    .ToList();
                foreach (TandTPoint fbPoint in fbRelatedToIoP)
                {
                    new BackwardSortedPointFacilityBooking(dbApp, result, sortedResult, this, fbPoint, sortingShema, Filter);
                }
            }
        }

    }
}
