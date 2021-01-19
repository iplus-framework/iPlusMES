using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [DataContract(Name = "cFIPOS")]
    public class FacilityInventoryPos
    {
        [DataMember(Name = "ID")]
        public Guid FacilityInventoryPosID
        {
            get; set;
        }

        [DataMember(Name = "FC")]
        public Guid FacilityChargeID
        {
            get; set;
        }

        [DataMember(Name = "SQ")]
        public int Sequence
        {
            get; set;
        }

        [DataMember(Name = "CM")]
        public string Comment
        {
            get; set;
        }

        [DataMember(Name = "LT")]
        public string LotNo
        {
            get; set;
        }

        [DataMember(Name = "FCNo")]
        public string FacilityNo
        {
            get; set;
        }

        [DataMember(Name = "FCN")]
        public string FacilityName
        {
            get; set;
        }

        [DataMember(Name = "MNo")]
        public string MaterialNo
        {
            get; set;
        }

        [DataMember(Name = "MN")]
        public string MaterialName
        {
            get; set;
        }

        [DataMember(Name = "cFI")]
        public FacilityInventory FacilityInventory
        {
            get; set;
        }

        [DataMember(Name = "MDFIPosS")]
        public MDFacilityInventoryPosState MDFacilityInventoryPosState
        {
            get; set;
        }


        [DataMember(Name = "StQ")]
        public double StockQuantity
        {
            get; set;
        }

        [DataMember(Name = "NStQ")]
        public double? NewStockQuantity
        {
            get; set;
        }

        [DataMember(Name = "NA")]
        public bool NotAvailable
        {
            get; set;
        }

        [DataMember(Name = "UN")]
        public string UpdateName
        {
            get; set;
        }

        [DataMember(Name = "UD")]
        public DateTime UpdateDate
        {
            get; set;
        }

    }
}
