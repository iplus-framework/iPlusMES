// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{
    public class FacilityQueryFilter
    {
        public DateTime SearchFrom { get; set; }
        public DateTime SearchTo { get; set; }

        public short? FilterFBTypeValue { get; set; }

        public Guid? FacilityID { get; set; }
        public Guid? FacilityLotID { get; set; }
        public Guid? FacilityChargeID { get; set; }
        public Guid? FacilityLocationID { get; set; }

        public Guid? MaterialID { get; set; }
        public Guid? FacilityInventoryPosID { get; set; }

        public bool HasEntityFilterSet
        {
            get
            {
                return FacilityID.HasValue || FacilityLotID.HasValue || FacilityChargeID.HasValue || FacilityLocationID.HasValue || MaterialID.HasValue;
            }
        }
    }
}
