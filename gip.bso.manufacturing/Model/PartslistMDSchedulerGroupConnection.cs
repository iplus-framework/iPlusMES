using gip.mes.datamodel;
using System;
using System.Collections.Generic;

namespace gip.bso.manufacturing
{
    public class PartslistMDSchedulerGroupConnection
    {
        public Guid PartslistID { get; set; }

        public List<MDSchedulingGroup> SchedulingGroups { get; set; }
    }
}
