using gip.core.datamodel;
using gip.mes.facility;
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

        [DataMember]
        public string Comment
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

        [DataMember(Name = "xMWFCM")]
        public short MaterialWFConnectionMode
        {
            get; set;
        }


        [DataMember(Name = "xPOPPIBS")]
        public IEnumerable<Guid> IntermediateBatchIDs
        {
            get; set;
        }

        [DataMember]
        public string ACUrlWF
        {
            get; set;
        }

        [DataMember]
        public POPartslistInfoState InfoState
        {
            get; set;
        }

        [DataMember]
        public ACMethod WFMethod
        {
            get; set;
        }

        [DataMember(Name = "PQSM")]
        public PostingQuantitySuggestionMode PostingQSuggestionMode
        {
            get;
            set;
        }

        [DataMember(Name = "PQSM2")]
        public PostingQuantitySuggestionMode PostingQSuggestionMode2
        {
            get;
            set;
        }
    }

    [DataContract(Name = "POPLInfoState")]
    public enum POPartslistInfoState : short 
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        Release = 10,
        [EnumMember]
        Pause = 20
    }
}
