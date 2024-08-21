using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
