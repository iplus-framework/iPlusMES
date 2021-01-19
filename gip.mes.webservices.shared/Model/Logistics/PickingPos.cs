using System;
using System.Runtime.Serialization;

namespace gip.mes.webservices
{
    [DataContract(Name = "cPP")]
    public class PickingPos
    {
        [DataMember(Name = "ID")]
        public Guid PickingPosID
        {
            get; set;
        }

        [DataMember(Name = "LN")]
        public string LineNumber
        {
            get; set;
        }

        [DataMember(Name = "xM")]
        public Material Material
        {
            get; set;
        }

        [DataMember(Name = "xFF")]
        public Facility FromFacility
        {
            get; set;
        }

        [DataMember(Name = "xTF")]
        public Facility ToFacility
        {
            get; set;
        }

        [DataMember(Name = "xMDU")]
        public MDUnit MDUnit
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

        [DataMember(Name = "CM")]
        public string Comment
        {
            get; set;
        }
    }
}
