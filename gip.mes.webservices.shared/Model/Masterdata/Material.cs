﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace gip.mes.webservices
{
    [DataContract(Name = "cM")]
    public class Material
    {
        [DataMember(Name = "ID")]
        public Guid MaterialID
        {
            get; set;
        }

        [DataMember(Name = "MNo")]
        public string MaterialNo
        { 
            get;set;
        }

        [DataMember(Name = "MNa1")]
        public string MaterialName1
        {
            get; set;
        }

        [DataMember(Name = "xBU")]
        public MDUnit BaseMDUnit
        {
            get; set;
        }

        [DataMember(Name = "MSQ")]
        public double? MinStockQuantity
        {
            get; set;
        }

        [DataMember(Name = "OSQ")]
        public double? OptStockQuantity
        {
            get; set;
        }
    }
}
