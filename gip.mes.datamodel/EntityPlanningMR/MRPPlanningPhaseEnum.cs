﻿using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'MRP planning phases'}de{'MRP-Planungsphasen'}", Global.ACKinds.TACEnum, QRYConfig = "gip.mes.datamodel.ACValueListMRPPlanningPhaseEnum")]
    public enum MRPPlanningPhaseEnum : short
    {
        PlanDefinition = 0,
        MaterialSelection = 1,
        ConsumptionBased = 2,
        RequirementBased = 3,
        FulfillmentProduction = 4,
        FulfillmentInOrder = 5,
        Finished = 6
    }

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'MRP planning phases'}de{'MRP-Planungsphasen'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListMRPPlanningPhaseEnum : ACValueItemList
    {
        public ACValueListMRPPlanningPhaseEnum() : base(nameof(PlanningMR.PlanningMRPhaseIndex))
        {
            AddEntry((short)MRPPlanningPhaseEnum.PlanDefinition, "en{'Define planning MR'}de{'Planung MR definieren'}");
            AddEntry((short)MRPPlanningPhaseEnum.MaterialSelection, "en{'Material selection'}de{'Materialauswahl'}");
            AddEntry((short)MRPPlanningPhaseEnum.ConsumptionBased, "en{'Calculate consumption'}de{'Verbrauch berechnen'}");
            AddEntry((short)MRPPlanningPhaseEnum.RequirementBased, "en{'Calculate requirements'}de{'Bedarf berechnen'}");
            AddEntry((short)MRPPlanningPhaseEnum.FulfillmentProduction, "en{'Fulfillment from production'}de{'Erfüllung ab Produktion'}");
            AddEntry((short)MRPPlanningPhaseEnum.FulfillmentInOrder, "en{'Fulfillment from purchase'}de{'Erfüllung ab Kauf'}");
            AddEntry((short)MRPPlanningPhaseEnum.Finished, "en{'Finished'}de{'Fertig'}");
        }
    }
}
