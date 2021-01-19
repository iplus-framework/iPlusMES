using gip.core.datamodel;
using gip.mes.datamodel;
using System.Linq;

namespace gip.mes.facility
{
    /// <summary>
    /// Forward Tracing and Tracing process on FacilityBooking item
    /// </summary>
    public class ForwardPointFacilityBooking : TandTPoint
    {
        /// <summary>
        /// construct specific inheritor of TandTPoint
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="parentPoint">calling element</param>
        /// <param name="fb">FacilityBooking as tracking item</param>
        /// <param name="filter">Filter for enable or disable item in tracking tree for display</param>
        public ForwardPointFacilityBooking(DatabaseApp dbApp, TandTResult rs, TandTPoint parentPoint, FacilityBooking fb, TandTFilter filter)
            : base(dbApp, rs, parentPoint, fb, filter)
        {

        }

        /// <summary>
        /// Only at first time process is forwarded to FacilityBookingCharge
        /// </summary>
        /// <param name="dbApp">DatabaseApp context</param>
        /// <param name="rs">tracking and tracing result</param>
        /// <param name="item">current processed item</param>
        public override void ProcessRelated(DatabaseApp dbApp, TandTResult rs, IACObjectEntity item)
        {
            FacilityBooking fb = item as FacilityBooking;
            // Only on inital call tracking for charge - FB selected 
            // in ohter cases FBC leads tracking process and FB is only for record
            if (rs.Results.Count == 1 && rs.Results[0] == this)
                foreach (FacilityBookingCharge fbc in fb.FacilityBookingCharge_FacilityBooking)
                    new ForwardPointFacilityBookingCharge(dbApp, rs, this, fbc, Filter);
        }
    }
}
