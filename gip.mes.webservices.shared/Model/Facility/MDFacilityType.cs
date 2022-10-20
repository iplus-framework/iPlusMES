using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using gip.mes.datamodel;

namespace gip.mes.webservices
{
    [DataContract(Name = "cMDFT")]
    public class MDFacilityType
    {
        [DataMember(Name = "ID")]
        public Guid MDFacilityTypeID
        {
            get; set;
        }

        [DataMember(Name = "MDNT")]
        public string MDNameTrans
        { 
            get;set;
        }

        [IgnoreDataMember]
        public string MDFacilityTypeName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
        }

        [DataMember(Name = "MDTi")]
        public short MDFacilityTypeIndex
        {
            get; set;
        }

        [IgnoreDataMember]
        public FacilityTypesEnum FacilityType
        {
            get
            {
                return (FacilityTypesEnum)MDFacilityTypeIndex;
            }
            set
            {
                MDFacilityTypeIndex = (Int16)value;
            }
        }
    }
}
