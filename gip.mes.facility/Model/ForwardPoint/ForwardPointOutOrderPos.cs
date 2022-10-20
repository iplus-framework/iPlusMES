using gip.core.datamodel;
using gip.mes.datamodel;
using System.Linq;

namespace gip.mes.facility
{
    /// <summary>
    /// Forward Tracing and Tracing process on OutOrderPos item
    /// </summary>
    public class ForwardPointOutOrderPos : TandTPoint
    {
        /// <summary>
        /// construct specific inheritor of TandTPoint
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="parentPoint">calling element</param>
        /// <param name="ooPos">OutOrderPos as tracking item</param>
        /// <param name="filter">Filter for enable or disable item in tracking tree for display</param>
        public ForwardPointOutOrderPos(DatabaseApp dbApp, TandTResult rs, TandTPoint parentPoint, OutOrderPos ooPos, TandTFilter filter)
            : base(dbApp, rs, parentPoint, ooPos, filter)
        {

        }

        /// <summary>
        /// Only register OutOrder
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="item">current processed item</param>
        public override void ProcessRelated(DatabaseApp dbApp, TandTResult rs, IACObjectEntity item)
        {
            OutOrderPos ops = item as OutOrderPos;
            new ForwardPointOutOrder(dbApp, rs, this, ops.OutOrder, Filter);
            if (ops.FacilityBookingCharge_OutOrderPos.Any())
                foreach (FacilityBookingCharge fbc in ops.FacilityBookingCharge_OutOrderPos)
                    new ForwardPointFacilityBookingCharge(dbApp, rs, this, fbc, Filter);
        }
    }
}
