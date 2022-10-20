using gip.mes.datamodel;
using System;

namespace gip.mes.facility.TandTv3
{
    public class FacilityBookingPreveiw : FacilityBookingPreviewBase
    {
        public string FacilityBookingNo { get; set; }
        public string FacilityBookingChargeNo { get; set; }
        

        public FacilityBookingCharge FacilityBookingCharge { get; set; }

        public Guid? FacilityChargeID { get; set; }
        public Guid? FacilityBookingChargeID { get; set; }

        public override string ToString()
        {
            return string.Format(@"{0} [{1}][{2}][{3}]", FacilityBookingNo, FacilityBookingChargeNo, LotNo, FacilityNo);
        }
    }
}
