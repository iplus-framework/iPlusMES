using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Inventory states'}de{'Bestandszustände'}", Global.ACKinds.TACEnum, QRYConfig = "gip.mes.datamodel.ACValueListFacilityInventoryStateEnum")]
    public enum FacilityInventoryStateEnum : short
    {
        New = 1,
        InProgress = 2,
        Finished = 3,
        Posted = 4
    }

    [ACClassInfo(Const.PackName_VarioFacility, "en{'Inventory states'}de{'Bestandszustände'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListFacilityInventoryStateEnum : ACValueItemList
    {
        public ACValueListFacilityInventoryStateEnum() : base("FacilityInventoryStateEnum")
        {
            AddEntry((short)FacilityInventoryStateEnum.New, "en{'New'}de{'Neu'}");
            AddEntry((short)FacilityInventoryStateEnum.InProgress, "en{'In Process'}de{'In Bearbeitung'}");
            AddEntry((short)FacilityInventoryStateEnum.Finished, "en{'Finished'}de{'Beendet'}");
            AddEntry((short)FacilityInventoryStateEnum.Posted, "en{'Posted'}de{'Gesendet'}");
        }
    }
}
