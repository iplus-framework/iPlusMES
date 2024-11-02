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
    /// Backward Tracing and Tracing process on ProdOrderPartslistPosRelation item
    /// </summary>
    public class BackwardPointProdOrderPartslistPosRelation : TandTPoint
    {
        /// <summary>
        /// construct specific inheritor of TandTPoint
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="parentPoint">calling element</param>
        /// <param name="rel">ProdOrderPartslistPosRelation tracked item</param>
        /// <param name="filter">Filter for enable or disable item in tracking tree for display</param>
        public BackwardPointProdOrderPartslistPosRelation(DatabaseApp dbApp, TandTResult rs, TandTPoint parentPoint, ProdOrderPartslistPosRelation rel, TandTFilter filter)
            : base(dbApp, rs, parentPoint, rel, filter)
        {

        }

        /// <summary>
        /// Forward tracking to relation charges and source position
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="item">current processed item</param>
        public override void ProcessRelated(DatabaseApp dbApp, TandTResult rs, IACObjectEntity item)
        {
            ProdOrderPartslistPosRelation rel = item as ProdOrderPartslistPosRelation;

            List<FacilityBookingCharge> fbcs = new List<FacilityBookingCharge>();
            //if (rel.TargetProdOrderPartslistPos.ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos.Any())
            fbcs = rel
                        .FacilityBookingCharge_ProdOrderPartslistPosRelation
                        .Where(c => !rs.Lots.Any()
                            || (c.OutwardFacilityLotID != null && rs.Lots.Contains(c.OutwardFacilityLot.LotNo))).ToList();
            if (!fbcs.Any() && !rel.TargetProdOrderPartslistPos.ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos.Any())
                fbcs = rel.FacilityBookingCharge_ProdOrderPartslistPosRelation.ToList();
            else
            {
                // Add charges from same booking
                fbcs = fbcs.Select(c => c.FacilityBooking).SelectMany(c => c.FacilityBookingCharge_FacilityBooking).ToList();
            }
            foreach (FacilityBookingCharge fbc in fbcs)
                new BackwardPointFacilityBookingCharge(dbApp, rs, this, fbc, Filter);
            new BackwardPointProdOrderPartslistPos(dbApp, rs, this, rel.SourceProdOrderPartslistPos, Filter);

            if (rel.SourceProdOrderPartslistPos.TakeMatFromOtherOrder)
            {
                List<Guid> partslistIDs = rel.SourceProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProdOrderPartslist_ProdOrder.Select(c => c.ProdOrderPartslistID).ToList();
                fbcs = dbApp
                    .FacilityBookingCharge
                    .Where(c =>
                        c.ProdOrderPartslistPosID != null &&
                        !partslistIDs.Contains(c.ProdOrderPartslistPos.ProdOrderPartslistID) &&
                        c.InwardFacilityLotID != null &&
                        rs.Lots.Contains(c.InwardFacilityLot.LotNo))
                    .ToList();
                foreach (FacilityBookingCharge fbc in fbcs)
                    new BackwardPointFacilityBookingCharge(dbApp, rs, this, fbc, Filter);
            }
        }
    }
}
