using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    /// <summary>
    /// Forward Tracing and Tracing process on ProdOrder item
    /// </summary>
    public class ForwardPointProdOrder : TandTPoint
    {
        /// <summary>
        /// construct specific inheritor of TandTPoint
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="parentPoint">calling element</param>
        /// <param name="prodOrder">ProdOrder as tracking item</param>
        /// <param name="filter">Filter for enable or disable item in tracking tree for display</param>
        public ForwardPointProdOrder(DatabaseApp dbApp, TandTResult rs, TandTPoint parentPoint, ProdOrder prodOrder, TandTFilter filter)
            : base(dbApp, rs, parentPoint, prodOrder, filter)
        {

        }

        /// <summary>
        /// Only register ProdOrder item
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="item">current processed item</param>
        public override void ProcessRelated(DatabaseApp dbApp, TandTResult rs, IACObjectEntity item)
        {

        }
    }
}
