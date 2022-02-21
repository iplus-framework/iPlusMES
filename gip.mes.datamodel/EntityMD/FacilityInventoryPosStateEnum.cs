using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Inventory position states'}de{'Bestandspositionszustände'}", Global.ACKinds.TACEnum, QRYConfig = "gip.mes.datamodel.ACValueListFacilityInventoryPosStateEnum")]
    public enum FacilityInventoryPosStateEnum : short
    {
        New = 1,
        InProgress = 2,
        Paused = 3,
        Finished = 4,
        Posted = 5
    }

    [ACClassInfo(Const.PackName_VarioFacility, "en{'Inventory position states'}de{'Bestandspositionszustände'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListFacilityInventoryPosStateEnum : ACValueItemList
    {
        public ACValueListFacilityInventoryPosStateEnum() : base("FacilityInventoryPosStateEnum")
        {
            AddEntry((short)FacilityInventoryPosStateEnum.New, "en{'New'}de{'Neu'}");
            AddEntry((short)FacilityInventoryPosStateEnum.InProgress, "en{'In Process'}de{'In Bearbeitung'}");
            AddEntry((short)FacilityInventoryPosStateEnum.Paused, "en{'Paused'}de{'Pause'}");
            AddEntry((short)FacilityInventoryPosStateEnum.Finished, "en{'Finished'}de{'Beendet'}");
            AddEntry((short)FacilityInventoryPosStateEnum.Posted, "en{'Posted'}de{'Gesendet'}");
        }
    }
}
