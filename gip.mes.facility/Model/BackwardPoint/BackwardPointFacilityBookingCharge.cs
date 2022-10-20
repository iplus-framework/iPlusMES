using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    /// <summary>
    /// Backward Tracing and Tracing process on FacilityBookingCharge item
    /// </summary>
    public class BackwardPointFacilityBookingCharge : TandTPoint
    {
        /// <summary>
        /// construct specific inheritor of TandTPoint
        /// </summary>
        /// <param name="db">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="parentPoint">calling element</param>
        /// <param name="fbc">FacilityBookingCharge tracked item</param>
        /// <param name="filter">Filter for enable or disable item in tracking tree for display</param>
        public BackwardPointFacilityBookingCharge(DatabaseApp dbApp, TandTResult rs, TandTPoint parentPoint, FacilityBookingCharge fbc, TandTFilter filter)
            : base(dbApp, rs, parentPoint, fbc, filter)
        {

        }

        /// <summary>
        /// Backward tracking process leads to ProdOrderPartslistPos, ProdOrderPartslistPosRelation and InOrderPos
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="item">current processed item</param>
        public override void ProcessRelated(DatabaseApp dbApp, TandTResult rs, IACObjectEntity item)
        {
            FacilityBookingCharge fbc = item as FacilityBookingCharge;
            new BackwardPointFacilityBooking(dbApp, rs, this, fbc.FacilityBooking, Filter);

            // Register FacilityCharge
            FacilityCharge facilityCharge = null;
            if (fbc.OutwardFacilityChargeID != null)
                facilityCharge = fbc.OutwardFacilityCharge;
            if (fbc.InwardFacilityChargeID != null)
                facilityCharge = fbc.InwardFacilityCharge;
            if (facilityCharge != null)
                new BackwardPointFacilityCharge(dbApp, rs, this, facilityCharge, Filter);

            if (fbc.ProdOrderPartslistPosID != null)
                new BackwardPointProdOrderPartslistPos(dbApp, rs, this, fbc.ProdOrderPartslistPos, Filter);
            if (fbc.ProdOrderPartslistPosRelationID != null)
                new BackwardPointProdOrderPartslistPosRelation(dbApp, rs, this, fbc.ProdOrderPartslistPosRelation, Filter);
            if (fbc.InOrderPosID != null)
                new BackwardPointInOrderPos(dbApp, rs, this, fbc.InOrderPos, Filter);
        }
    }
}
