using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace gip.mes.webservices
{
    [DataContract(Name = "cFL")]
    public class SearchFacilityCharge
    {
        [DataMember(Name = "St")]
        public List<FacilityChargeStateEnum> States
        {
            get; set;
        }

        [DataMember(Name = "fPos")]
        public FacilityInventoryPos FacilityInventoryPos
        {
            get; set;
        }

        [DataMember(Name = "diffFNo")]
        public string DifferentFacilityNo
        {
            get; set;
        }
    }
}
