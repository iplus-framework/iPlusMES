using gip.mes.datamodel;
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
