// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    public class FacilityChargeList : List<FacilityCharge>
    {
        public FacilityChargeList(IEnumerable<FacilityCharge> list, ACMethodBooking BP) 
            : base(list)
        {
            if (BP.AutoRefresh)
            {
                this.ForEach(c => c.AutoRefresh(BP.DatabaseApp));
            }
        }
    }
}