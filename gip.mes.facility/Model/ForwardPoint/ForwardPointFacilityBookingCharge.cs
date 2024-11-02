// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility
{
    /// <summary>
    /// Forward Tracing and Tracing process on FacilityBookingCharge item
    /// </summary>
    public class ForwardPointFacilityBookingCharge : TandTPoint
    {
        /// <summary>
        /// construct specific inheritor of TandTPoint
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="parentPoint">calling element</param>
        /// <param name="fbc">FacilityBookingCharge as tracking item</param>
        /// <param name="filter">Filter for enable or disable item in tracking tree for display</param>
        public ForwardPointFacilityBookingCharge(DatabaseApp dbApp, TandTResult rs, TandTPoint parentPoint, FacilityBookingCharge fbc, TandTFilter filter)
            : base(dbApp, rs, parentPoint, fbc, filter)
        {

        }

        /// <summary>
        /// Forward tracking of facility booking charge
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="item">current processed item</param>
        public override void ProcessRelated(DatabaseApp dbApp, TandTResult rs, IACObjectEntity item)
        {
            FacilityBookingCharge fbc = item as FacilityBookingCharge;
            List<FacilityBookingCharge> fbcs = item as List<FacilityBookingCharge>;
            new ForwardPointFacilityBooking(dbApp, rs, this, fbc.FacilityBooking, Filter);

            // Register FacilityCharge
            FacilityCharge facilityCharge = null;
            if (fbc.OutwardFacilityChargeID != null)
                facilityCharge = fbc.OutwardFacilityCharge;
            if (fbc.InwardFacilityChargeID != null)
                facilityCharge = fbc.InwardFacilityCharge;

            if (facilityCharge != null)
                new ForwardPointFacilityCharge(dbApp, rs, this, facilityCharge, Filter);

            if (fbc.ProdOrderPartslistPosID != null)
                new ForwardPointProdOrderPartslistPos(dbApp, rs, this, fbc.ProdOrderPartslistPos, Filter);
            if (fbc.ProdOrderPartslistPosRelationID != null)
                new ForwardPointProdOrderPartslistPosRelation(dbApp, rs, this, fbc.ProdOrderPartslistPosRelation, Filter);
            if (fbc.OutOrderPosID != null)
                new ForwardPointOutOrderPos(dbApp, rs, this, fbc.OutOrderPos, Filter);

            bool forwardSearch =
                fbc.InOrderPosID != null ||
                (fbc.ProdOrderPartslistPosID != null &&
                   fbc.ProdOrderPartslistPos.IsFinalMixure &&
                   dbApp.ProdOrderPartslistPos.Any(c => c.SourceProdOrderPartslistID == fbc.ProdOrderPartslistPos.ProdOrderPartslistPosID)
              );
            if (forwardSearch && fbc.InwardFacilityChargeID != null)
            {
                fbcs = dbApp.FacilityBookingCharge.Where(c =>
                    c.OutwardFacilityChargeID != null &&
                    c.OutwardFacilityChargeID == fbc.InwardFacilityChargeID
                    ).ToList();
                foreach (FacilityBookingCharge tmpFbc in fbcs)
                    new ForwardPointFacilityBookingCharge(dbApp, rs, this, tmpFbc, Filter);
            }
        }
    }
}
