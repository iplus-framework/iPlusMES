using System;
using System.Runtime.Serialization;

namespace gip.mes.webservices
{
    [DataContract(Name = "cFI")]
    public class FacilityInventory
    {
        [DataMember(Name = "ID")]
        public Guid FacilityInventoryID
        {
            get;set;
        }

        [DataMember(Name = "FIN")]
        public string FacilityInventoryNo
        {
            get; set;
        }

        [DataMember(Name = "FINM")]
        public string FacilityInventoryName
        {
            get; set;
        }

      

        [DataMember(Name = "MFIS")]
        public MDFacilityInventoryState MDFacilityInventoryState
        {
            get; set;
        }


        [DataMember(Name = "IN")]
        public string InsertName
        {
            get; set;
        }

        [DataMember(Name = "IND")]
        public DateTime InsertDate
        {
            get; set;
        }
    }
}
