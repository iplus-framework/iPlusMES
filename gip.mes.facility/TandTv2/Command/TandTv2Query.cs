using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;

namespace gip.mes.facility
{
    public static class TandTv2Query
    {
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

        public static Func<DatabaseApp, Guid, bool, List<Guid>, List<string>, IEnumerable<FacilityBookingCharge>> s_cQry_GetPosCharge =
           CompiledQuery.Compile<DatabaseApp, Guid, bool, List<Guid>, List<string>, IEnumerable<FacilityBookingCharge>>(
           (db, posID, useLotCheck, IDs, lotNos) =>
                   db.FacilityBookingCharge.Where(c =>
                   c.ProdOrderPartslistPosID == posID &&
                   !IDs.Contains(c.FacilityBookingChargeID) &&
                   c.InwardFacilityChargeID != null &&
                   c.InwardFacilityCharge.FacilityLotID != null &&
                   (!useLotCheck || lotNos.Contains(c.InwardFacilityCharge.FacilityLot.LotNo)))
       );


       // public static Func<DatabaseApp, Guid, bool, List<Guid>, List<string>, IEnumerable<ProdOrderPartslistPosRelation>> s_cQry_GetPosRelationsBackward =
       //    CompiledQuery.Compile<DatabaseApp, Guid, bool, List<Guid>, List<string>, IEnumerable<ProdOrderPartslistPosRelation>>(
       //    (db, posID, useLotCheck, IDs, lotNos) =>
       //            db.ProdOrderPartslistPosRelation.Where(c =>
       //            c.TargetProdOrderPartslistPosID == posID && 
       //            IDs.Contains(c.ProdOrderPartslistPosRelationID) &&
       //            (!useLotCheck || )
                   
       //            )
       //);
    }
}
