using gip.core.datamodel;

namespace gip.bso.manufacturing
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'BatchPlanProdOrderSortFilterEnum'}de{'BatchPlanProdOrderSortFilterEnum'}", Global.ACKinds.TACEnum, QRYConfig = "gip.bso.manufacturing.ACValueListBatchPlanProdOrderSortFilterEnum")]

    public enum BatchPlanProdOrderSortFilterEnum
    {
        StartTime,
        Material,
        ProgramNo
    }

    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Sort order'}de{'Sortierreihenfolge'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListBatchPlanProdOrderSortFilterEnum : ACValueItemList
    {
        public ACValueListBatchPlanProdOrderSortFilterEnum() : base("BatchOrderFilter")
        {
            AddEntry(BatchPlanProdOrderSortFilterEnum.Material, "en{'By material'}de{'Nach material'}");
            AddEntry(BatchPlanProdOrderSortFilterEnum.StartTime, "en{'By Start Time'}de{'Nach Startzeit'}");
            AddEntry(BatchPlanProdOrderSortFilterEnum.ProgramNo, "en{'By ProgramNo'}de{'Nach AuftragNo.'}");
        }
    }
}
