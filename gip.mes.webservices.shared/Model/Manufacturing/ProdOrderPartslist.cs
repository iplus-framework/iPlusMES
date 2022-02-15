using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [DataContract(Name = "cPOPL")]
    public class ProdOrderPartslist : EntityBase
    {
        [DataMember(Name = "ID")]
        public Guid ProdOrderPartslistID
        {
            get;set;
        }

        [DataMember(Name = "PO")]
        public ProdOrder ProdOrder
        {
            get;set;
        }

        [DataMember(Name = "PL")]
        public Partslist Partslist
        {
            get;set;
        }

        [DataMember(Name = "TQ")]
        public double TargetQuantity
        {
            get;set;
        }

        [DataMember(Name = "AQ")]
        public double ActualQuantity
        {
            get;set;
        }

        [DataMember(Name = "SD")]
        public DateTime? StartDate
        {
            get; set;
        }

        [DataMember(Name = "ED")]
        public DateTime? EndDate
        {
            get; set;
        }
    }

    [DataContract(Name = "cPOPLWF")]
    public class ProdOrderPartslistWFInfo
    {
        [DataMember(Name = "xPOPL")]
        public ProdOrderPartslist ProdOrderPartslist
        {
            get; set;
        }

        [DataMember(Name = "xPOPPI")]
        public ProdOrderPartslistPos Intermediate
        {
            get; set;
        }

        [DataMember(Name = "xPOPPIB")]
        public ProdOrderPartslistPos IntermediateBatch
        {
            get; set;
        }

        [DataMember]
        public string ACUrlWF
        {
            get; set;
        }

        [DataMember]
        public bool ForRelease
        {
            get; set;
        }

        [DataMember]
        public ACMethod WFMethod
        {
            get; set;
        }
    }
}
