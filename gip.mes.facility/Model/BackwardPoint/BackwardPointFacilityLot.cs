// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    /// <summary>
    /// Backward Tracing and Tracing process on FacilityLot item
    /// 
    /// </summary>
    public class BackwardPointFacilityLot : TandTPoint
    {
        /// <summary>
        /// construct specific inheritor of TandTPoint
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="parentPoint">calling element</param>
        /// <param name="fl">FacilityLot tracked item</param>
        /// <param name="filter">Filter for enable or disable item in tracking tree for display</param>
        public BackwardPointFacilityLot(DatabaseApp dbApp, TandTResult rs, TandTPoint parentPoint, FacilityLot fl, TandTFilter filter)
            : base(dbApp, rs, parentPoint, fl, filter)
        {

        }

        /// <summary>
        /// Update lot list in result with new lots
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="item">current processed item</param>
        public override void ProcessRelated(DatabaseApp dbApp, TandTResult rs, IACObjectEntity item)
        {
            FacilityLot facilityLot = item as FacilityLot;
            if (!rs.Lots.Contains(facilityLot.LotNo))
                rs.Lots.Add(facilityLot.LotNo);
        }
    }
}
