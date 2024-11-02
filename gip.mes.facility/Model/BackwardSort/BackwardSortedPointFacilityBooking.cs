// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{
    public class BackwardSortedPointFacilityBooking : TandTPointSorted
    {
        public BackwardSortedPointFacilityBooking(DatabaseApp dbApp, TandTResult result, TandTResult sortedResult, TandTPoint parentPoint, TandTPoint originalPoint, TandTSortingShemaEnum sortingShema, TandTFilter filter)
            : base(dbApp, result, sortedResult, parentPoint, originalPoint, sortingShema, filter)
        {

        }

        public override void ProcessSortedRelated(DatabaseApp dbApp, TandTResult result, TandTResult sortedResult, TandTPoint originalPoint, TandTSortingShemaEnum sortingShema)
        {
            List<Guid> fbcIds = (originalPoint.Item as FacilityBooking).FacilityBookingCharge_FacilityBooking.Select(x => x.FacilityBookingChargeID).ToList();
            List<TandTPoint> fbcPoints = result
                .Results
                .Where(x => fbcIds.Contains(x.ID))
                .OrderBy(x => (x.Item as FacilityBookingCharge).StorageDate)
                .ThenBy(x => (x.Item as FacilityBookingCharge).InsertDate)
                .ToList();
            foreach (TandTPoint fbcPoint in fbcPoints)
                new BackwardSortedPointFacilityBookingCharge(dbApp, result, sortedResult, this, fbcPoint, sortingShema, Filter);
        }
    }
}
