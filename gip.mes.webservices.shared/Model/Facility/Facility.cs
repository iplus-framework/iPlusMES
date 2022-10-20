using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using gip.core.datamodel;

namespace gip.mes.webservices
{
#if NETFRAMEWORK
    [ACSerializeableInfo]
#endif
    [DataContract(Name = "cF")]
    public class Facility
    {
        [DataMember(Name = "ID")]
        public Guid FacilityID
        {
            get; set;
        }

        [DataMember(Name = "FNo")]
        public string FacilityNo
        {
            get; set;
        }

        [DataMember(Name = "FN")]
        public string FacilityName
        {
            get; set;
        }

        [DataMember(Name = "xMDFT")]
        public MDFacilityType MDFacilityType
        {
            get; set;
        }

        [DataMember(Name = "PFID")]
        public Guid? ParentFacilityID
        {
            get; set;
        }

        [DataMember(Name = "PF")]
        public Facility ParentFacility
        {
            get; set;
        }

        [DataMember(Name = "SPQ")]
        public bool SkipPrintQuestion
        {
            get; set;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(FacilityName))
                return FacilityName;

            if (!string.IsNullOrEmpty(FacilityNo))
                return FacilityNo;

            return base.ToString();
        }
    }
}
