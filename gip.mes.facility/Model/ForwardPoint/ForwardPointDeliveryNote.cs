﻿using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    /// <summary>
    /// Forward Tracing and Tracing process on DeliveryNote item
    /// </summary>
    public class ForwardPointDeliveryNote : TandTPoint
    {
        /// <summary>
        /// construct specific inheritor of TandTPoint
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="parentPoint">calling element</param>
        /// <param name="dn">DeliveryNote as tracking item</param>
        /// <param name="filter">Filter for enable or disable item in tracking tree for display</param>
        public ForwardPointDeliveryNote(DatabaseApp dbApp, TandTResult rs, TandTPoint parentPoint, DeliveryNote dn, TandTFilter filter)
            : base(dbApp, rs, parentPoint, dn, filter)
        {

        }

        /// <summary>
        /// Register only delivery note
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="item">current processed item</param>
        public override void ProcessRelated(DatabaseApp dbApp, TandTResult rs, IACObjectEntity item)
        {

        }
    }
}