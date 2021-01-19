using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [DataContract(Name = "cPOB")]
    public class ProdOrderBatch : EntityBase
    {
        [DataMember(Name = "ID")]
        public Guid ProdOrderBatchID
        {
            get; set;
        }

        [DataMember(Name = "POBNo")]
        public string ProdOrderBatchNo
        {
            get; set;
        }

        [DataMember(Name = "BSNo")]
        public int BatchSeqNo
        {
            get; set;
        }
    }
}
