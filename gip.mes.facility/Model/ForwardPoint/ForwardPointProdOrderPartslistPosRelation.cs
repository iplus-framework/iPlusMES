using gip.core.datamodel;
using gip.mes.datamodel;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility
{
    /// <summary>
    /// Forward Tracing and Tracing process on ProdOrderPartslistPosRelation item
    /// </summary>
    public class ForwardPointProdOrderPartslistPosRelation : TandTPoint
    {
        /// <summary>
        /// construct specific inheritor of TandTPoint
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="parentPoint">calling element</param>
        /// <param name="rel">ProdOrderPartslistPosRelation as tracked item</param>
        /// <param name="filter">Filter for enable or disable item in tracking tree for display</param>
        public ForwardPointProdOrderPartslistPosRelation(DatabaseApp dbApp, TandTResult rs, TandTPoint parentPoint, ProdOrderPartslistPosRelation rel, TandTFilter filter)
            : base(dbApp, rs, parentPoint, rel, filter)
        {

        }

        /// <summary>
        /// Forwards to related charges and register target ProdOrderPartslistPos item
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="item">current processed item</param>
        public override void ProcessRelated(DatabaseApp dbApp, TandTResult rs, IACObjectEntity item)
        {
            ProdOrderPartslistPosRelation rel = item as ProdOrderPartslistPosRelation;
            foreach (FacilityBookingCharge fbc in rel.FacilityBookingCharge_ProdOrderPartslistPosRelation)
                new ForwardPointFacilityBookingCharge(dbApp, rs, this, fbc, Filter);
            new ForwardPointProdOrderPartslistPos(dbApp, rs, this, rel.TargetProdOrderPartslistPos, Filter);
        }
    }
}
