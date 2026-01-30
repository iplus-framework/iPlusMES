using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;

namespace gip.mes.facility
{
    public class GenerateProdOrdersResult
    {
        public string PlanningMRNo { get; set; }
        public List<Guid> MDSchedulingGroupIDs { get; set; } = new List<Guid>();

        public List<ProdOrder> GeneratedProdOrders { get; set; } = new List<ProdOrder>();

        public MsgWithDetails SaveMessage { get; set; }
        public MsgWithDetails ErrorMessage { get; set; }
    }
}
