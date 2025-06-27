using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'MRP planning phases'}de{'MRP-Planungsphasen'}", Global.ACKinds.TACEnum, QRYConfig = "gip.mes.datamodel.ACValueListPlanningMRPhaseEnum")]
    public enum PlanningMRPhaseEnum : short
    {
        PlanDefinition = 0,
        MaterialSelection = 1,
        ConsumptionBased = 2,
        RequirementBased = 3,
        Fulfillment = 4
    }

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'MRP planning phases'}de{'MRP-Planungsphasen'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListPlanningMRPhaseEnum : ACValueItemList
    {
        public ACValueListPlanningMRPhaseEnum() : base(nameof(PlanningMR.PlanningMRPhaseIndex))
        {
            AddEntry((short)PlanningMRPhaseEnum.PlanDefinition, "en{'Define planning MR'}de{'Planung MR definieren'}");
            AddEntry((short)PlanningMRPhaseEnum.MaterialSelection, "en{'Material selection'}de{'Materialauswahl'}");
            AddEntry((short)PlanningMRPhaseEnum.ConsumptionBased, "en{'Calculate consumption'}de{'Verbrauch berechnen'}");
            AddEntry((short)PlanningMRPhaseEnum.RequirementBased, "en{'Calculate requierements'}de{'Bedarf berechnen'}");
            AddEntry((short)PlanningMRPhaseEnum.Fulfillment, "en{'Fulfillment generate'}de{'Erfüllung generieren'}");
        }
    }
}
