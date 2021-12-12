using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace gip.mes.webservices
{
    public enum FacilityTypesEnum : short
    {
        StorageLocation = 1000, // Lagerort
        StorageBin = 1100, // Lagerplatz
        StorageBinContainer = 1110, // Behältnis: Tank/Silo/Zelle/Waagenbehältnis
        PreparationBin = 1120,
        Vehicle = 2000, // Fahrzeug
        VehicleContainer = 2100, // Fahrzeug: Stellplatz/Kammer/Öffnung
    }


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
