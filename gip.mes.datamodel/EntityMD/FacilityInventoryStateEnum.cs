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
        Posted = 4,
        Canceled = 5
    }

    [ACClassInfo(Const.PackName_VarioFacility, "en{'Inventory states'}de{'Bestandszustände'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListFacilityInventoryStateEnum : ACValueItemList
    {
        public ACValueListFacilityInventoryStateEnum() : base("FacilityInventoryStateEnum")
        {
            AddEntry(FacilityInventoryStateEnum.New, "en{'New'}de{'Neu'}");
            AddEntry(FacilityInventoryStateEnum.InProgress, "en{'In Process'}de{'In Bearbeitung'}");
            AddEntry(FacilityInventoryStateEnum.Finished, "en{'Finished'}de{'Beendet'}");
            AddEntry(FacilityInventoryStateEnum.Posted, "en{'Posted'}de{'Gesendet'}");
            AddEntry(FacilityInventoryStateEnum.Canceled, "en{'Canceled'}de{'Storniert'}");
        }

    }
}
