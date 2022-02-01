using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [DataContract(Name = "cFCAI")]
    public class FacilityChargeActivationItem : FacilityCharge
    {
        public const string FacilityChargeActivationKeyACUrl = "FCActivationItem";

        public FacilityChargeActivationItem() : base()
        {

        }

        [DataMember(Name = "wID")]
        public Guid WorkplaceID
        {
            get;
            set;
        }

    }
}
