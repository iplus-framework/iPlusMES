using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [DataContract(Name = "cMDPT")]
    public class MDPickingType
    {
        [DataMember(Name = "ID")]
        public Guid MDPickingTypeID
        {
            get; set;
        }

        [DataMember]
        public string MDKey
        {
            get;
            set;
        }

        [DataMember(Name = "MDPTT")]
        public string MDPickingTypeTrans
        {
            get; set;
        }

        [IgnoreDataMember]
        public string MDPickingTypeName
        {
            get
            {
                return Translator.GetTranslation(MDPickingTypeTrans);
            }
        }
    }
}
