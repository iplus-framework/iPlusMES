using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    /// <summary>
    /// Backward Tracing and Tracing process on FacilityPreBooking item
    /// </summary>
    public class BackwardPointFacilityPreBooking : TandTPoint
    {
        /// <summary>
        /// construct specific inheritor of TandTPoint
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="parentPoint">calling element</param>
        /// <param name="fpb">FacilityPreBooking tracked item</param>
        /// <param name="filter">Filter for enable or disable item in tracking tree for display</param>
        public BackwardPointFacilityPreBooking(DatabaseApp dbApp, TandTResult rs, TandTPoint parentPoint, FacilityPreBooking fpb, TandTFilter filter)
            : base(dbApp, rs, parentPoint, fpb, filter)
        {

        }

        /// <summary>
        /// Forward tracking and tracing to 
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="item">current processed item</param>
        public override void ProcessRelated(DatabaseApp dbApp, TandTResult rs, IACObjectEntity item)
        {
            FacilityPreBooking fpb = item as FacilityPreBooking;
            if (fpb.ProdOrderPartslistPosID != null)
                new BackwardPointProdOrderPartslistPos(dbApp, rs, this, fpb.ProdOrderPartslistPos, Filter);
            if (fpb.ProdOrderPartslistPosRelationID != null)
                new BackwardPointProdOrderPartslistPosRelation(dbApp, rs, this, fpb.ProdOrderPartslistPosRelation, Filter);
            if (fpb.InOrderPosID != null)
                new BackwardPointInOrderPos(dbApp, rs, this, fpb.InOrderPos, Filter);
        }
    }
}
