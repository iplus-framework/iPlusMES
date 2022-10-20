using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [DataContract(Name = "cMDFISPOS")]
    public class MDFacilityInventoryPosState
    {

        [DataMember(Name = "ID")]
        public Guid MDFacilityInventoryPosStateID
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
        public int MDFacilityInventoryPosStateIndex
        {
            get; set;
        }

        public FacilityInventoryPosStates FacilityInventoryPosState
        {
            get
            {
                return (FacilityInventoryPosStates)MDFacilityInventoryPosStateIndex;
            }
            set
            {
                MDFacilityInventoryPosStateIndex = (short)value;
            }
        }

        public enum FacilityInventoryPosStates : short
        {
            New = 1,
            InProgress = 2,
            Paused = 3,
            Finished = 4,
        }
    }
}
