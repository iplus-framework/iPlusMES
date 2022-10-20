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


    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'POPosTimeFilterTypeEnum'}de{'POPosTimeFilterTypeEnum'}", Global.ACKinds.TACEnum, QRYConfig = "gip.bso.manufacturing.ACValueListPOPosTimeFilterTypeEnum")]
    public enum POPosTimeFilterTypeEnum
    {
        ProdOrderPosStartTime,
        ProdOrderPosEndTime,
        ProdOrderPosStartEndTime
    }

    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'POPosTimeFilterTypeList'}de{'POPosTimeFilterTypeList'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListPOPosTimeFilterTypeEnum : ACValueItemList
    {
        public ACValueListPOPosTimeFilterTypeEnum() : base("POPosTimeFilterType")
        {
            AddEntry(POPosTimeFilterTypeEnum.ProdOrderPosStartTime, "en{'Start time'}de{'Startzeit'}");
            AddEntry(POPosTimeFilterTypeEnum.ProdOrderPosEndTime, "en{'End time'}de{'Endzeit'}");
            AddEntry(POPosTimeFilterTypeEnum.ProdOrderPosStartEndTime, "en{'Time period'}de{'Zeitraum'}");
        }
    }

}
