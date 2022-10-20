using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [DataContract(Name = "cPW")]
    public class PickingWorkplace : Picking
    {
        public PickingWorkplace() : base()
        {
        }

        [DataMember(Name = "WPID")]
        public Guid WorkplaceID
        {
            get;
            set;
        }

    }
}
