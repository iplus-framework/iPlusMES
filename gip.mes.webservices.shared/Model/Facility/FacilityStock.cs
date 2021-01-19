using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace gip.mes.webservices
{
    [DataContract(Name = "cFS")]
    public class FacilityStock
    {
        [DataMember(Name = "ID")]
        public Guid FacilityStockID
        {
            get; set;
        }

        [DataMember(Name = "xMDRS")]
        public MDReleaseState MDReleaseState
        {
            get; set;
        }

        [DataMember(Name = "SQ")]
        public double StockQuantity
        {
            get; set;
        }

        [DataMember(Name = "DI")]
        public double DayInward
        {
            get; set;
        }

        [DataMember(Name = "DO")]
        public double DayOutward
        {
            get; set;
        }

        [DataMember(Name = "MI")]
        public double MonthInward
        {
            get; set;
        }

        [DataMember(Name = "MO")]
        public double MonthOutward
        {
            get; set;
        }
    }
}
