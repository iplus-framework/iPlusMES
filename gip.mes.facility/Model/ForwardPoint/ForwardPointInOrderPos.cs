// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.datamodel;
using System.Linq;

namespace gip.mes.facility
{
    /// <summary>
    /// Forward Tracing and Tracing process on InOrderPos item
    /// </summary>
    public class ForwardPointInOrderPos : TandTPoint
    {
        /// <summary>
        /// construct specific inheritor of TandTPoint
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="parentPoint">calling element</param>
        /// <param name="inPos">InOrderPos as tracking item</param>
        /// <param name="filter">Filter for enable or disable item in tracking tree for display</param>
        public ForwardPointInOrderPos(DatabaseApp dbApp, TandTResult rs, TandTPoint parentPoint, InOrderPos inPos, TandTFilter filter)
            : base(dbApp, rs, parentPoint, inPos, filter)
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
            InOrderPos inPos = item as InOrderPos;
            if (inPos.DeliveryNotePos_InOrderPos.Any())
            {
                new ForwardPointDeliveryNotePos(dbApp, rs, this, inPos.DeliveryNotePos_InOrderPos.FirstOrDefault(), Filter);
            }

            if (inPos.ParentInOrderPosID != null)
            {
                if (inPos.DeliveryNotePos_InOrderPos.Any())
                {
                    foreach (var dnp in inPos.DeliveryNotePos_InOrderPos.OrderBy(x => x.InsertDate))
                        new ForwardPointDeliveryNotePos(dbApp, rs, this, dnp, Filter);
                }

                if (inPos.FacilityBookingCharge_InOrderPos.Any())
                {
                    foreach (FacilityBookingCharge fbc in inPos.FacilityBookingCharge_InOrderPos.OrderBy(x => x.FacilityBooking.FacilityBookingNo))
                    {
                        new ForwardPointFacilityBookingCharge(dbApp, rs, this, fbc, Filter);
                    }
                }
            }
            else
            {
                foreach (InOrderPos iop_child in inPos.InOrderPos_ParentInOrderPos.OrderBy(x => x.Sequence))
                    new ForwardPointInOrderPos(dbApp, rs, this, iop_child, Filter);
            }
        }
    }
}
