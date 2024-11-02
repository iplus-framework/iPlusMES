// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    /// <summary>
    /// Backward Tracing and Tracing process on DeliveryNotePos item
    /// </summary>
    public class BackwardPointDeliveryNotePos : TandTPoint
    {
        /// <summary>
        /// construct specific inheritor of TandTPoint
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="parentPoint">calling element</param>
        /// <param name="dns">DeliveryNotePos as tracking item</param>
        /// <param name="filter">Filter for enable or disable item in tracking tree for display</param>
        public BackwardPointDeliveryNotePos(DatabaseApp dbApp, TandTResult rs, TandTPoint parentPoint, DeliveryNotePos dns, TandTFilter filter)
            : base(dbApp, rs, parentPoint, dns, filter)
        {

        }

        /// <summary>
        /// Backward tracing proces leads to related InOrderPos
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="item">current processed item</param>
        public override void ProcessRelated(DatabaseApp dbApp, TandTResult rs, IACObjectEntity item)
        {
            DeliveryNotePos dnp = item as DeliveryNotePos;
            new BackwardPointDeliveryNote(dbApp, rs, this, dnp.DeliveryNote, Filter);

            if (dnp.InOrderPosID != null)
                new BackwardPointInOrderPos(dbApp, rs, this, dnp.InOrderPos.InOrderPos1_ParentInOrderPos, Filter);
        }
    }
}
