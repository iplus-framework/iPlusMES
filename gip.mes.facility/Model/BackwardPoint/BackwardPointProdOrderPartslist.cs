// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    /// <summary>
    ///  Backward Tracing and Tracing process on ProdOrderPartslist item
    /// </summary>
    public class BackwardPointProdOrderPartslist : TandTPoint
    {
        /// <summary>
        /// construct specific inheritor of TandTPoint
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="parentPoint">calling element</param>
        /// <param name="prodOr">ProdOrderPartslist tracked item</param>
        /// <param name="filter">Filter for enable or disable item in tracking tree for display</param>
        public BackwardPointProdOrderPartslist(DatabaseApp dbApp, TandTResult rs, TandTPoint parentPoint, ProdOrderPartslist prodOr, TandTFilter filter)
            : base(dbApp, rs, parentPoint, prodOr, filter)
        {

        }

        /// <summary>
        /// Only register ProdOrderPartslist and forwards to ProdOrder
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="item">current processed item</param>
        public override void ProcessRelated(DatabaseApp dbApp, TandTResult rs, IACObjectEntity item)
        {
            ProdOrderPartslist pl = item as ProdOrderPartslist;
            new BackwardPointProdOrder(dbApp, rs, this, pl.ProdOrder, Filter);
        }
    }
}
