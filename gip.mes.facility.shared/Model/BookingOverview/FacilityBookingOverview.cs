#if NETFRAMEWORK
using gip.core.datamodel;
#endif
using System;
using System.Runtime.Serialization;

namespace gip.mes.facility
{
#if NETFRAMEWORK
    [ACClassInfo(Const.PackName_VarioFacility, "en{'FacilityBookingOverview'}de{'FacilityBookingOverview'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
#endif
    [DataContract(Name = "cFBO")]
    public class FacilityBookingOverview : BookingOverviewBase
    {
#if NETFRAMEWORK
        [ACPropertyInfo(9999, "FacilityBookingNo", "en{'Posting No.'}de{'Buchungsnr.'}")]
#endif
        [DataMember(Name = "FBNo")]
        public string FacilityBookingNo { get; set; }

        [DataMember(Name = "FBID")]
        public Guid FacilityBookingID { get; set; }

       
    }
}
