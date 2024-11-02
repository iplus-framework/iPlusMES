// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility
{
    public class TrackingAndTracingBackwardCommand
    {
        // Function 01: find final mix in previus partslist used for a current tracking position
        public static ProdOrderPartslistPos GetFinalMixure(DatabaseApp ctx, Guid sourceProdOrderPartslistID)
        {
            return ctx.ProdOrderPartslistPos.Where(p => 
                p.ProdOrderPartslistID == sourceProdOrderPartslistID && 
                p.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern &&
                !p.Material.MaterialWFRelation_SourceMaterial.Where(c => c.SourceMaterialID != c.TargetMaterialID).Any() && 
                !p.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Any()
            ).FirstOrDefault(); ;
        }

        // Function 002: Get FBC connected with lot list
        public static List<FacilityBookingCharge> GetMixureFbcConnectedWithLotNo(ProdOrderPartslistPos finalMixurePos, List<string> lotNoList)
        {
            return
                    finalMixurePos.FacilityBookingCharge_ProdOrderPartslistPos
                        .Where(p => p.InwardFacilityChargeID != null)
                        .Where(p => lotNoList.Contains(p.InwardFacilityCharge.FacilityLot.LotNo))
                        .Select(p => p)
                    .Union(
                        finalMixurePos.ProdOrderPartslistPos_ParentProdOrderPartslistPos
                        .SelectMany(p => p.FacilityBookingCharge_ProdOrderPartslistPos)
                        .Where(p => p.InwardFacilityChargeID != null)
                        .Where(p => p.InwardFacilityCharge.FacilityLotID !=null && lotNoList.Contains(p.InwardFacilityCharge.FacilityLot.LotNo))
                        .Select(p => p))
                        .OrderBy(x=>x.FacilityBookingChargeNo)
                        .ToList();
        }

        // Function 003: Find a related batches to selected charges
        public static List<ProdOrderPartslistPos> GetFbcRelatedPositions(DatabaseApp ctx, List<FacilityBookingCharge> fbcList)
        {
            return fbcList
                    .Select(p => p.ProdOrderPartslistPosID)
                    .Distinct()
                    .Join(ctx.ProdOrderPartslistPos, id => id.Value, pos => pos.ProdOrderPartslistPosID, (id, pos) => new { pos = pos })
                    .Select(p => p.pos).ToList();
        }
    }
}
