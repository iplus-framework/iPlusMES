﻿using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    /// <summary>
    /// Forward Tracing and Tracing process on DeliveryNotePos item
    /// </summary>
    public class ForwardPointDeliveryNotePos : TandTPoint
    {
        /// <summary>
        /// construct specific inheritor of TandTPoint
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="parentPoint">calling element</param>
        /// <param name="dns">DeliveryNotePos as tracking item</param>
        /// <param name="filter">Filter for enable or disable item in tracking tree for display</param>
        public ForwardPointDeliveryNotePos(DatabaseApp dbApp, TandTResult rs, TandTPoint parentPoint, DeliveryNotePos dns, TandTFilter filter)
            : base(dbApp, rs, parentPoint, dns, filter)
        {

        }

        /// <summary>
        /// Forward tracing to delivery note and pos and in InOrderPos hierarchy
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="item">current processed item</param>
        public override void ProcessRelated(DatabaseApp dbApp, TandTResult rs, IACObjectEntity item)
        {
            DeliveryNotePos dns = item as DeliveryNotePos;
            new ForwardPointDeliveryNote(dbApp, rs, this, dns.DeliveryNote, Filter);

            if (dns.InOrderPosID != null)
                new ForwardPointInOrderPos(dbApp, rs, this, dns.InOrderPos, Filter);
        }
    }
}