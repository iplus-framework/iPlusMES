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
    /// Backward Tracing and Tracing process on ProdOrderPartslistPos item
    /// </summary>
    public class BackwardPointProdOrderPartslistPos : TandTPoint
    {
        /// <summary>
        /// construct specific inheritor of TandTPoint
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="parentPoint">calling element</param>
        /// <param name="prodOr">ProdOrderPartslistPos tracked item</param>
        /// <param name="filter">Filter for enable or disable item in tracking tree for display</param>
        public BackwardPointProdOrderPartslistPos(DatabaseApp dbApp, TandTResult rs, TandTPoint parentPoint, ProdOrderPartslistPos prodOr, TandTFilter filter)
            : base(dbApp, rs, parentPoint, prodOr, filter)
        {

        }


        /// <summary>
        /// Complete logic for tracking and tracking from position 
        /// depends on type of position (MaterialPosType)
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="item">current processed item</param>
        public override void ProcessRelated(DatabaseApp dbApp, TandTResult rs, IACObjectEntity item)
        {
            ProdOrderPartslistPos pos = item as ProdOrderPartslistPos;
            List<FacilityBookingCharge> fbcs = null;
            switch (pos.MaterialPosType)
            {
                case GlobalApp.MaterialPosTypes.InwardPartIntern:
                    fbcs = pos
                        .FacilityBookingCharge_ProdOrderPartslistPos
                        .Where(c => !rs.Lots.Any() || (c.InwardFacilityLotID != null && rs.Lots.Contains(c.InwardFacilityLot.LotNo))).ToList();
                    foreach (FacilityBookingCharge fbc in fbcs)
                        new BackwardPointFacilityBookingCharge(dbApp, rs, this, fbc, Filter);
                    foreach (var rel in pos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.OrderBy(x => x.Sequence))
                        new BackwardPointProdOrderPartslistPosRelation(dbApp, rs, this, rel, Filter);
                    
                    break;
                case GlobalApp.MaterialPosTypes.InwardIntern:
                    fbcs = pos
                        .ProdOrderPartslistPos_ParentProdOrderPartslistPos
                        .SelectMany(c => c.FacilityBookingCharge_ProdOrderPartslistPos)
                        .Where(c => !rs.Lots.Any() || (c.InwardFacilityLotID != null && rs.Lots.Contains(c.InwardFacilityLot.LotNo)))
                        .ToList();
                    foreach (FacilityBookingCharge fbc in fbcs)
                        new BackwardPointFacilityBookingCharge(dbApp, rs, this, fbc, Filter);
                    if (!fbcs.Any())
                        foreach (ProdOrderPartslistPosRelation rel in pos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
                            new BackwardPointProdOrderPartslistPosRelation(dbApp, rs, this, rel, Filter);
                    break;
                case GlobalApp.MaterialPosTypes.OutwardRoot:
                    if (pos.SourceProdOrderPartslistID != null)
                    {
                        ProdOrderPartslistPos finalMixure = TrackingAndTracingBackwardCommand.GetFinalMixure(dbApp, pos.SourceProdOrderPartslistID ?? Guid.Empty);
                        if (finalMixure != null)
                        {
                            new BackwardPointProdOrderPartslist(dbApp, rs, this, finalMixure.ProdOrderPartslist, Filter);
                            new BackwardPointProdOrderPartslistPos(dbApp, rs, this, finalMixure, Filter);
                        }
                    }
                    else
                    {
                        fbcs =
                            dbApp
                            .FacilityBookingCharge
                            .Where(c =>
                                c.InOrderPosID != null &&
                                c.InwardFacilityChargeID != null &&
                                rs.Lots.Contains(c.InwardFacilityCharge.FacilityLot.LotNo))
                            .ToList();
                        foreach (FacilityBookingCharge fbc in fbcs)
                            new BackwardPointFacilityBookingCharge(dbApp, rs, this, fbc, Filter);
                    }
                    break;
            }
        }
    }
}
