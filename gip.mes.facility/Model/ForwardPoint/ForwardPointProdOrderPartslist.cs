// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    /// <summary>
    /// Forward Tracing and Tracing process on ProdOrderPartslist item
    /// </summary>
    public class ForwardPointProdOrderPartslist : TandTPoint
    {
        /// <summary>
        /// construct specific inheritor of TandTPoint
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="parentPoint">calling element</param>
        /// <param name="pl">ProdOrderPartslist as a tracking item</param>
        /// <param name="filter">Filter for enable or disable item in tracking tree for display</param>
        public ForwardPointProdOrderPartslist(DatabaseApp dbApp, TandTResult rs, TandTPoint parentPoint, ProdOrderPartslist pl, TandTFilter filter)
            : base(dbApp, rs, parentPoint, pl, filter)
        {

        }

        /// <summary>
        /// forwards to ProdOrder item
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="item">current processed item</param>
        public override void ProcessRelated(DatabaseApp dbApp, TandTResult rs, IACObjectEntity item)
        {
            ProdOrderPartslist pl = item as ProdOrderPartslist;
            new ForwardPointProdOrder(dbApp, rs, this, pl.ProdOrder, Filter);
        }
    }
}
