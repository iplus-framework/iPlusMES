﻿#if NETFRAMEWORK
using gip.core.datamodel;
using gip.mes.datamodel;
#endif
using System;
using System.Runtime.Serialization;

namespace gip.mes.facility
{

#if NETFRAMEWORK
    [ACClassInfo(Const.PackName_VarioFacility, "en{'FacilityBookingChargeOverview'}de{'FacilityBookingChargeOverview'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
#endif
    [DataContract(Name = "cFBCO")]
    public class FacilityBookingChargeOverview : BookingOverviewBase
    {
#if NETFRAMEWORK
        [ACPropertyInfo(9999, "FacilityBookingChargeNo", "en{'Posting No.'}de{'Buchungsnr.'}")]
#endif
        [DataMember(Name = "FBCNo")]
        public string FacilityBookingChargeNo { get; set; }

        [DataMember(Name = "IFCID")]
        public Guid? InwardFacilityChargeID { get; set; }

        [DataMember(Name = "OFCID")]
        public Guid? OutwardFacilityChargeID { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "InwardFacilityChargeExternLotNo", ConstApp.ExternLotNo)]
#endif
        [DataMember(Name = "IFCELNo")]
        public string InwardFacilityChargeExternLotNo { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "InwardFacilityChargeFillingDate", ConstApp.FillingDate)]
#endif
        [DataMember(Name = "IFCFDt")]
        public DateTime? InwardFacilityChargeFillingDate { get; set; }

        [DataMember(Name = "IFPB")]
        public short InwardFacilityPostingBehaviour
        {
            get;
            set;
        }

    }
}