using gip.core.datamodel;

namespace gip.bso.manufacturing
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'TimeFilterTypeEnum'}de{'TimeFilterTypeEnum'}", Global.ACKinds.TACEnum, QRYConfig = "gip.bso.manufacturing.ACValueListTimeFilterTypeEnum")]

    public enum TimeFilterTypeEnum
    {
        ProdOrderStartTime,
        BookingTime
    }

    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Sort order'}de{'Sortierreihenfolge'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListTimeFilterTypeEnum : ACValueItemList
    {
        public ACValueListTimeFilterTypeEnum() : base("TimeFilterType")
        {
            AddEntry(TimeFilterTypeEnum.ProdOrderStartTime, "en{'ProdOrder start time'}de{'Auftrag Startzeit'}");
            AddEntry(TimeFilterTypeEnum.BookingTime, "en{'Booking time'}de{'Buchungzeit'}");
        }
    }
}
