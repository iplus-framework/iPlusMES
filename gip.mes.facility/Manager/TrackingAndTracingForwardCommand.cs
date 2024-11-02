// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{
    public class TrackingAndTracingForwardCommand
    {
        public static List<FacilityBookingCharge> GetMixureFbcOutwardConnectedWithLotNo(ProdOrderPartslistPos mixure, List<string> lotNoList)
        {
            return
                    mixure
                        .ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                        .SelectMany(p => p.FacilityBookingCharge_ProdOrderPartslistPosRelation)
                        .Where(p => p.OutwardFacilityChargeID != null)
                        .Where(p => lotNoList.Contains(p.OutwardFacilityCharge.FacilityLot.LotNo))
                        .Select(p => p)
                    .Union(
                        mixure
                        .ProdOrderPartslistPos_ParentProdOrderPartslistPos
                        .SelectMany(p => p.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos)
                        .SelectMany(p => p.FacilityBookingCharge_ProdOrderPartslistPosRelation)
                        .Where(p => p.OutwardFacilityChargeID != null)
                        .Where(p => lotNoList.Contains(p.OutwardFacilityCharge.FacilityLot.LotNo))
                        .Select(p => p)
                        ).ToList();
        }
    }
}
