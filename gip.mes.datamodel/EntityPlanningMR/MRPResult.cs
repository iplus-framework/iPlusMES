using gip.core.datamodel;
using System.Collections.Generic;

namespace gip.mes.datamodel
{
    public class MRPResult
    {

        #region ctor's
        public MRPResult()
        {
            SaveMessage = new MsgWithDetails();
        }
        #endregion
        
        public MsgWithDetails SaveMessage { get; set; }
        public int CreatedProposals { get; set; }
        public int CreatedOrders { get; set; }

        public PlanningMR PlanningMR { get; set; }
        public List<ConsumptionModel> PlanningPosition { get; set; }
        public List<ConsumptionModel> ConsumptionPlanningPosition { get; set; }
        public List<ConsumptionModel> RequirementPlanningPosition { get; set; }
       
    }
}
