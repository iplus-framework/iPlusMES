using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility
{
    /// <summary>
    /// Forward Tracing and Tracing process on ProdOrderPartslistPos item
    /// </summary>
    public class ForwardPointProdOrderPartslistPos : TandTPoint
    {
        /// <summary>
        /// construct specific inheritor of TandTPoint
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="parentPoint">calling element</param>
        /// <param name="pos">ProdOrderPartslistPos as tracked item</param>
        /// <param name="filter">Filter for enable or disable item in tracking tree for display</param>
        public ForwardPointProdOrderPartslistPos(DatabaseApp dbApp, TandTResult rs, TandTPoint parentPoint, ProdOrderPartslistPos pos, TandTFilter filter)
            : base(dbApp, rs, parentPoint, pos, filter)
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
            ProdOrderPartslistPos pos = item as ProdOrderPartslistPos;
            List<FacilityBookingCharge> fbcs = null;
            switch (pos.MaterialPosType)
            {
                case GlobalApp.MaterialPosTypes.InwardPartIntern:
                    fbcs = pos
                        .FacilityBookingCharge_ProdOrderPartslistPos
                        //.Where(c => !rs.Lots.Any() || (c.InwardFacilityLotID != null && rs.Lots.Contains(c.InwardFacilityLot.LotNo)))
                        .ToList();
                    foreach (FacilityBookingCharge fbc in fbcs)
                        new ForwardPointFacilityBookingCharge(dbApp, rs, this, fbc, Filter);
                    foreach (var rel in pos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.OrderBy(x => x.Sequence))
                        new ForwardPointProdOrderPartslistPosRelation(dbApp, rs, this, rel, Filter);
                    if (pos.IsFinalMixureBatch)
                        new ForwardPointProdOrderPartslistPos(dbApp, rs, this, pos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos, Filter);
                    break;
                case GlobalApp.MaterialPosTypes.InwardIntern:
                    fbcs = pos
                       .ProdOrderPartslistPos_ParentProdOrderPartslistPos
                       .SelectMany(c => c.FacilityBookingCharge_ProdOrderPartslistPos)
                       .Where(c => !rs.Lots.Any() || (c.InwardFacilityLotID != null && rs.Lots.Contains(c.InwardFacilityLot.LotNo)))
                       .ToList();
                    foreach (FacilityBookingCharge fbc in fbcs)
                        new ForwardPointFacilityBookingCharge(dbApp, rs, this, fbc, Filter);
                    //if (!fbcs.Any())
                    //var rels = pos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                    //    .Where(c => c.FacilityBookingCharge_ProdOrderPartslistPosRelation.Any(x => x.OutwardFacilityChargeID != null && rs.Lots.Contains(x.OutwardFacilityCharge.FacilityLot.LotNo)));
                    //foreach (ProdOrderPartslistPosRelation rel in rels)
                    //    new ForwardPointProdOrderPartslistPosRelation(db, rs, this, rel, Filter);
                    if (!fbcs.Any())
                        fbcs =
                          dbApp.FacilityBookingCharge
                          .Where(c => c.OutwardFacilityLotID != null && rs.Lots.Contains(c.OutwardFacilityLot.LotNo))
                          .ToList();
                    foreach (FacilityBookingCharge fbc in fbcs)
                        new ForwardPointFacilityBookingCharge(dbApp, rs, this, fbc, Filter);
                    //List<Guid> idsa = rs.FacilityChargeList.Select(c => c.FacilityChargeID).ToList();
                    //List<ProdOrderPartslistPos> batches =
                    //    pos
                    //    .ProdOrderPartslistPos_ParentProdOrderPartslistPos
                    //    .Where(c =>
                    //            c.FacilityBooking_ProdOrderPartslistPos
                    //            .Any(x => x.InwardFacilityChargeID != null && idsa.Contains(x.InwardFacilityChargeID ?? Guid.Empty)))
                    //            .ToList();
                    //if (!batches.Any())
                    //{
                    //    batches =
                    //        pos
                    //        .ProdOrderPartslistPos_ParentProdOrderPartslistPos
                    //        .Where(c =>
                    //            c.FacilityBooking_ProdOrderPartslistPos
                    //            .Any(x => x.InwardFacilityLotID != null && rs.Lots.Contains(x.InwardFacilityLot.LotNo)))
                    //            .ToList();
                    //}

                    //if (batches.Any())
                    //{
                    //    foreach (ProdOrderPartslistPos batch in batches)
                    //        new ForwardPointProdOrderPartslistPos(db, rs, this, batch, Filter);
                    //}
                    //else
                    //{
                    //    fbcs =
                    //        pos
                    //        .FacilityBookingCharge_ProdOrderPartslistPos
                    //        .Where(c => !rs.Lots.Any() || rs.Lots.Contains(c.FacilityBooking.InwardFacilityCharge.FacilityLot.LotNo))
                    //        .OrderBy(x => x.FacilityBooking.FacilityBookingNo)
                    //        .ToList();
                    //    foreach (FacilityBookingCharge fbc in fbcs)
                    //        new ForwardPointFacilityBookingCharge(db, rs, this, fbc, Filter);
                    //    if (pos.ProdOrderPartslistPos_ParentProdOrderPartslistPos.Any())
                    //        foreach (ProdOrderPartslistPos batch in pos.ProdOrderPartslistPos_ParentProdOrderPartslistPos)
                    //            new ForwardPointProdOrderPartslistPos(db, rs, this, batch, Filter);
                    //}
                    //foreach (ProdOrderPartslistPosRelation rel in pos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos)
                    //    new ForwardPointProdOrderPartslistPosRelation(db, rs, this, rel, Filter);
                    break;
                case GlobalApp.MaterialPosTypes.OutwardRoot:
                    break;
            }

            if (pos.IsFinalMixure)
            {
                new ForwardPointProdOrderPartslist(dbApp, rs, this, pos.ProdOrderPartslist, Filter);
                // Search for next prodorder and forward lots
                fbcs =
                    dbApp
                    .ProdOrderPartslistPos
                    .Where(c => c.SourceProdOrderPartslistID == pos.ProdOrderPartslistID)
                    .SelectMany(c => c.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos)
                    .SelectMany(c => c.FacilityBookingCharge_ProdOrderPartslistPosRelation)
                    .Where(c => c.OutwardFacilityChargeID != null && rs.Lots.Contains(c.OutwardFacilityCharge.FacilityLot.LotNo))
                    .ToList();
                foreach (FacilityBookingCharge fbc in fbcs)
                    new ForwardPointFacilityBookingCharge(dbApp, rs, this, fbc, Filter);
                //fbcs =
                //    db
                //    .FacilityBookingCharge
                //    .Where(c => c.OutwardFacilityChargeID != null && rs.Lots.Contains(c.OutwardFacilityCharge.FacilityLot.LotNo))
                //    .ToList();
                //foreach (FacilityBookingCharge fbc in fbcs)
                //    new ForwardPointFacilityBookingCharge(db, rs, this, fbc, Filter);
            }
        }
    }
}
