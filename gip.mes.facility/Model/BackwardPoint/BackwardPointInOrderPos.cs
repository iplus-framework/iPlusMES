// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.datamodel;
using System.Linq;

namespace gip.mes.facility
{
    /// <summary>
    /// Backward Tracing and Tracing process on InOrderPos item
    /// </summary>
    public class BackwardPointInOrderPos : TandTPoint
    {
        /// <summary>
        /// construct specific inheritor of TandTPoint
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="parentPoint">calling element</param>
        /// <param name="iop">InOrderPos tracked item</param>
        /// <param name="filter">Filter for enable or disable item in tracking tree for display</param>
        public BackwardPointInOrderPos(DatabaseApp dbApp, TandTResult rs, TandTPoint parentPoint, InOrderPos iop, TandTFilter filter)
            : base(dbApp, rs, parentPoint, iop, filter)
        {

        }

        /// <summary>
        /// Tracked forwarded to InOrder and to same InOrderPos on other
        /// place of hierarchy
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="item">current processed item</param>
        public override void ProcessRelated(DatabaseApp dbApp, TandTResult rs, IACObjectEntity item)
        {
            InOrderPos ioPos = item as InOrderPos;
            new BackwardPointInOrder(dbApp, rs, this, ioPos.InOrder, Filter);


            if (ioPos.ParentInOrderPosID != null)
            {
                if (ioPos.DeliveryNotePos_InOrderPos.Any())
                {
                    foreach (var dnp in ioPos.DeliveryNotePos_InOrderPos.OrderBy(x => x.InsertDate))
                        new BackwardPointDeliveryNotePos(dbApp, rs, this, dnp, Filter);
                }

                if (ioPos.FacilityBookingCharge_InOrderPos.Any())
                {
                    foreach (FacilityBookingCharge fbc in ioPos.FacilityBookingCharge_InOrderPos.OrderBy(x => x.FacilityBooking.FacilityBookingNo))
                    {
                        new BackwardPointFacilityBookingCharge(dbApp, rs, this, fbc, Filter);
                    }
                }
            }
            else
            {
                foreach (InOrderPos iop_child in ioPos.InOrderPos_ParentInOrderPos.OrderBy(x => x.Sequence))
                    new BackwardPointInOrderPos(dbApp, rs, this, iop_child, Filter);
            }
        }
    }
}
