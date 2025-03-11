using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace gip.mes.webservices
{
    [DataContract(Name = "cMDUC")]
    public class MDUnitCalc
    {
        [DataMember(Name = "MID")]
        public Guid MaterialID
        {
            get; set;
        }

        [DataMember(Name = "UID")]
        public MDUnit Unit
        {
            get; set;
        }

        [DataMember(Name = "IV")]
        public double InputValue
        {
            get; set;
        }

        [DataMember(Name = "RVT")]
        public double ResultValueInTargetUnit
        {
            get; set;
        }

        [DataMember(Name = "RVB")]
        public double ResultValueInBase
        {
            get; set;
        }

        [DataMember(Name = "BUID")]
        public MDUnit BaseUnit
        {
            get; set;
        }
    }
}
