using gip.core.datamodel;
using System;
using System.Runtime.Serialization;

namespace gip.mes.webservices
{
    [DataContract(Name = "cMDFIS")]
    public class MDFacilityInventoryState
    {
        [DataMember(Name = "ID")]
        public Guid MDFacilityInventoryStateID
        {
            get; set;
        }

        [DataMember(Name = "MDNT")]
        public string MDNameTrans
        {
            get; set;
        }

        [IgnoreDataMember]
        public string MDFacilityInventoryStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
        }

        [DataMember(Name = "SI")]
        public int SortIndex
        {
            get; set;
        }

        [DataMember(Name = "IDF")]
        public bool IsDefault
        {
            get; set;
        }

        [DataMember(Name = "MDFII")]
        public int MDFacilityInventoryStateIndex
        {
            get; set;
        }

        [IgnoreDataMember]
        public FacilityInventoryStates FacilityInventoryState
        {
            get
            {
                return (FacilityInventoryStates)MDFacilityInventoryStateIndex;
            }
            set
            {
                MDFacilityInventoryStateIndex = (Int16)value;
            }
        }

        public enum FacilityInventoryStates : short
        {
            New = 1,
            InProgress = 2,
            Finished = 3,
        }
    }
}
