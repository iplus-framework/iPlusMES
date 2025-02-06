using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Inventory position states'}de{'Bestandspositionszustände'}", Global.ACKinds.TACEnum, QRYConfig = "gip.mes.datamodel.ACValueListFacilityInventoryPosStateEnum")]
    public enum FacilityInventoryPosStateEnum : short
    {
        New = 1,
        InProgress = 2,
        //Paused = 3,
        Finished = 4,
        Posted = 5
    }

    [ACClassInfo(Const.PackName_VarioFacility, "en{'Inventory position states'}de{'Bestandspositionszustände'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListFacilityInventoryPosStateEnum : ACValueItemList
    {
        public ACValueListFacilityInventoryPosStateEnum() : base(nameof(FacilityInventoryPosStateEnum))
        {
            AddEntry(FacilityInventoryPosStateEnum.New, "en{'Not counted'}de{'Ungezählt'}");
            AddEntry(FacilityInventoryPosStateEnum.InProgress, "en{'In Counting'}de{'In Zählung'}");
            // AddEntry(FacilityInventoryPosStateEnum.Paused, "en{'Paused'}de{'Pause'}");
            AddEntry(FacilityInventoryPosStateEnum.Finished, "en{'Counted'}de{'Gezählt'}");
            AddEntry(FacilityInventoryPosStateEnum.Posted, "en{'Posted'}de{'Gebucht'}");
        }
    }
}
