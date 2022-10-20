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
        public FacilityChargeList(IEnumerable<FacilityCharge> list) 
            : base(list)
        {
        }
    }
}