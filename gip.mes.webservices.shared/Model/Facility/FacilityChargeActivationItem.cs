using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [DataContract(Name = "cFCAI")]
    public class FacilityChargeParamItem : FacilityCharge
    {
        public const string FacilityChargeActivationKeyACUrl = "FCActivationItem";

        public FacilityChargeParamItem() : base()
        {

        }

        [DataMember(Name = "wID")]
        public Guid ParamID
        {
            get;
            set;
        }

    }
}
