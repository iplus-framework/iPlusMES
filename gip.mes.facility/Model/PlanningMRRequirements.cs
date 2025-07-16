using gip.mes.datamodel;
using System.Collections.Generic;

namespace gip.mes.facility
{
    public class PlanningMRRequirements
    {
        public List<OutOrderPos> OutOrderPosList { get; set; }
        public List<ProdOrderPartslistPos> Components { get; set; }
    }
}
