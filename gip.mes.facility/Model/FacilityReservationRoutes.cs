using gip.core.autocomponent;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    public class FacilityReservationRoutes
    {
        public FacilityReservation Reservation
        {
            get;
            set;
        }

        public RoutingResult Routes
        {
            get;
            set;
        }
    }
}
