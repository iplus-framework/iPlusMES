// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    /// <summary>
    /// Forward Tracing and Tracing process on FacilityPreBooking item
    /// </summary>
    public class ForwardPointFacilityPreBooking : TandTPoint
    {
        /// <summary>
        /// construct specific inheritor of TandTPoint
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="parentPoint">calling element</param>
        /// <param name="parentPoint"></param>
        /// <param name="fpb">FacilityPreBooking as tracked item</param>
        /// <param name="filter">Filter for enable or disable item in tracking tree for display</param>
        public ForwardPointFacilityPreBooking(DatabaseApp dbApp, TandTResult rs, TandTPoint parentPoint, FacilityPreBooking fpb, TandTFilter filter)
            : base(dbApp, rs, parentPoint, fpb, filter)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="item">current processed item</param>
        public override void ProcessRelated(DatabaseApp dbApp, TandTResult rs, IACObjectEntity item)
        {
            FacilityPreBooking fpb = item as FacilityPreBooking;
            if (fpb.ProdOrderPartslistPosID != null)
                new ForwardPointProdOrderPartslistPos(dbApp, rs, this, fpb.ProdOrderPartslistPos, Filter);
            if (fpb.ProdOrderPartslistPosRelationID != null)
                new ForwardPointProdOrderPartslistPosRelation(dbApp, rs, this, fpb.ProdOrderPartslistPosRelation, Filter);
            if (fpb.OutOrderPosID != null)
                new ForwardPointOutOrderPos(dbApp, rs, this, fpb.OutOrderPos, Filter);
        }
    }
}
