using gip.mes.datamodel;
using System.Collections.Generic;

namespace gip.mes.facility
{
    public class SchedulingMaxBPOrder
    {
        public MDSchedulingGroup MDSchedulingGroup { get; set; }
        public List<SchedulingMaxBPOrderWF>  WFs{ get; set; }
    }
}
