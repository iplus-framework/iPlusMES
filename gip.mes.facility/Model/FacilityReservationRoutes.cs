// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
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
