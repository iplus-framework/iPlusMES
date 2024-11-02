// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    /// <summary>
    /// Forward Tracing and Tracing process on FacilityCharge item
    /// </summary>
    public class ForwardPointFacilityCharge : TandTPoint
    {
        /// <summary>
        /// construct specific inheritor of TandTPoint
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="parentPoint">calling element</param>
        /// <param name="fc">FacilityCharge as tracking point</param>
        /// <param name="filter">Filter for enable or disable item in tracking tree for display</param>
        public ForwardPointFacilityCharge(DatabaseApp dbApp, TandTResult rs, TandTPoint parentPoint, FacilityCharge fc, TandTFilter filter)
            : base(dbApp, rs, parentPoint, fc, filter)
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
            FacilityCharge fc = item as FacilityCharge;
            rs.AddFacilityCharge(fc);
            if (fc.FacilityLotID != null)
                new ForwardPointFacilityLot(dbApp, rs, this, fc.FacilityLot, Filter);
        }
    }
}
