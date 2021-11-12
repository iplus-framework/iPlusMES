using gip.core.datamodel;
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

        [DataMember(Name = "MDPTT")]
        public string MDPickingTypeTrans
        {
            get; set;
        }

        public string MDUnitName
        {
            get
            {
                return Translator.GetTranslation(MDPickingTypeTrans);
            }
        }
    }
}
