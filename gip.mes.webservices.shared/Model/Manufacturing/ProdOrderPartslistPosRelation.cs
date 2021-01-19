using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [DataContract(Name = "cPOPLPosRel")]
    public class ProdOrderPartslistPosRelation : EntityBase
    {
        [DataMember(Name = "ID")]
        public Guid ProdOrderPartslistPosRelationID
        {
            get;set;
        }


        [DataMember(Name = "SPos")]
        public ProdOrderPartslistPos SourcePos
        {
            get;set;
        }

        [DataMember(Name = "TPos")]
        public ProdOrderPartslistPos TargetPos
        {
            get;set;
        }

        [DataMember(Name = "SQ")]
        public int Sequence
        {
            get; set;
        }

        [DataMember(Name = "TQ")]
        public double TargetQuantity
        {
            get; set;
        }

        [DataMember(Name = "TQU")]
        public double TargetQuantityUOM
        {
            get; set;
        }

        [DataMember(Name = "AQ")]
        public double ActualQuantity
        {
            get; set;
        }

        [DataMember(Name = "AQU")]
        public double ActualQuantityUOM
        {
            get; set;
        }
    }
}
