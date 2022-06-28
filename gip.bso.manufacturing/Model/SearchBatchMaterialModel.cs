using gip.mes.datamodel;
using System;
using System.Collections.Generic;

namespace gip.bso.manufacturing
{
    public class SearchBatchMaterialModel
    {
        public Guid ProdOrderBatchPlanID { get; set; }
        public Guid SourcePosID { get; set; }
        public Guid MaterialID { get; set; }
        public double TargetQuantityUOM { get; set; }

        public Guid? MDSchedulingGroupID { get; set; }
    }
}
