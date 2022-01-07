using System.Collections.Generic;
using System.Runtime.Serialization;

namespace gip.mes.webservices
{

    [DataContract(Name = "prT")]
    public class PrintEntity
    {
        [DataMember]
        public List<BarcodeEntity> Sequence
        {
            get; set;
        }
        [DataMember]
        public int CopyCount
        {
            get; set;
        }

        [DataMember]
        public bool SkipPrinterCheck
        {
            get; set;
        }
    }
}
