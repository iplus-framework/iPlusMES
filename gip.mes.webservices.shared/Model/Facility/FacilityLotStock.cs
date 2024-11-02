// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace gip.mes.webservices
{
    [DataContract(Name = "cFLS")]
    public class FacilityLotStock
    {
        [DataMember(Name = "ID")]
        public Guid FacilityLotStockID
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
