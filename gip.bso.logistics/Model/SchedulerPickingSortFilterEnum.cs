using gip.core.datamodel;

namespace gip.bso.logistics
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'SchedulerPickingSortFilterEnum'}de{'SchedulerPickingSortFilterEnum'}", Global.ACKinds.TACEnum, QRYConfig = "gip.bso.logistics.ACValueListSchedulerPickingSortFilterEnum")]
    public enum SchedulerPickingSortFilterEnum
    {
        StartTime,
        Material,
        ProgramNo
    }

    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Sort order'}de{'Sortierreihenfolge'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListSchedulerPickingSortFilterEnum : ACValueItemList
    {
        public ACValueListSchedulerPickingSortFilterEnum() : base("BatchOrderFilter")
        {
            AddEntry(SchedulerPickingSortFilterEnum.Material, "en{'By material'}de{'Nach material'}");
            AddEntry(SchedulerPickingSortFilterEnum.StartTime, "en{'By Start Time'}de{'Nach Startzeit'}");
            AddEntry(SchedulerPickingSortFilterEnum.ProgramNo, "en{'By PickingNo'}de{'Nach Kommissioniernummer'}");
        }
    }
}
