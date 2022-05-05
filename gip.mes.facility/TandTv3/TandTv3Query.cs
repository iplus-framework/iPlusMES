using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public static class TandTv3Query
    {

        #region Production Queries

        public static Func<DatabaseApp, Guid, ProdOrderPartslistPos> s_cQry_GetFinalMixure =
           CompiledQuery.Compile<DatabaseApp, Guid, ProdOrderPartslistPos>(
           (db, sourceProdOrderPartslistID) =>
                   db.ProdOrderPartslistPos.Where(p =>
                    p.ProdOrderPartslistID == sourceProdOrderPartslistID &&
                    p.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern &&
                    !p.Material.MaterialWFRelation_SourceMaterial.Where(c => c.SourceMaterialID != c.TargetMaterialID).Any() &&
                    !p.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Any()
            ).FirstOrDefault()
       );

        #endregion

        #region Booking Queries


        public static Func<DatabaseApp, Guid, bool, List<Guid>, List<string>, IEnumerable<FacilityBookingCharge>> s_cQry_GetPosCharge =
           CompiledQuery.Compile<DatabaseApp, Guid, bool, List<Guid>, List<string>, IEnumerable<FacilityBookingCharge>>(
           (db, posID, useLotCheck, IDs, lotNos) =>
                   db.FacilityBookingCharge.Where(c =>
                        c.ProdOrderPartslistPosID == posID 
                   &&   !IDs.Contains(c.FacilityBookingChargeID) 
                   &&   c.InwardFacilityChargeID != null 
                   &&   c.InwardFacilityCharge.FacilityLotID != null &&
                   (!useLotCheck || lotNos.Contains(c.InwardFacilityCharge.FacilityLot.LotNo)))
       );


        public static readonly Func<FacilityBookingCharge, TandTv3FilterTracking, Guid?, Guid?, bool> s_cQry_FBCInwardQuery = (c, filter, materialID, facilityID) =>
                            c.InwardFacilityChargeID != null 
                            && c.InwardFacilityCharge.FacilityLotID != null
                            && (materialID == null || c.InwardMaterialID == materialID)
                            && (facilityID == null || c.InwardFacilityID == facilityID);

        public static readonly Func<FacilityBookingCharge, TandTv3FilterTracking, bool> s_cQry_FBCOutwardQuery = (c, filter) =>
                               c.OutwardMaterialID != null 
                            && c.OutwardFacilityChargeID != null 
                            && c.OutwardFacilityCharge.FacilityLotID != null 
                            &&
                            (
                                (
                                    filter.MaterialIDs == null 
                                    || !filter.MaterialIDs.Any() 
                                    || !filter.MaterialIDs.Contains(c.OutwardMaterialID ?? Guid.Empty) 
                                    || (filter.FilterDateFrom == null && filter.FilterDateTo == null)
                                ) ||
                                (
                                    filter.MaterialIDs.Contains(c.OutwardMaterialID ?? Guid.Empty) 
                                    && (filter.FilterDateFrom == null || c.InsertDate >= filter.FilterDateFrom) 
                                    && (filter.FilterDateTo == null || c.InsertDate < filter.FilterDateTo)
                                )
                            );
        #endregion


    }
}
