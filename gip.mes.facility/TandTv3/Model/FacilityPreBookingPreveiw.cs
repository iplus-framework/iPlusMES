using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility.TandTv3
{
    public class FacilityPreBookingPreveiw : FacilityBookingPreviewBase
    {
        public string FacilityPreBookingNo { get; set; }
        public FacilityPreBooking FacilityPreBooking { get; set; }

        public override string ToString()
        {
            return string.Format(@"{0} [{1}][{2}]]", FacilityPreBookingNo, LotNo, FacilityNo);
        }
    }
}
