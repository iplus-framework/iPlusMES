using gip.core.datamodel;
using System.Runtime.Serialization;

namespace gip.mes.datamodel
{
#if NETFRAMEWORK
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.ESMDBatchPlanStartMode, Global.ACKinds.TACEnum, QRYConfig = "gip.mes.datamodel.ACValueListBatchPlanStartModeEnum")]
#else
       [DataContract]
#endif
    public enum BatchPlanStartModeEnum : short
    {
        Off = 0,
        AutoSequential = 1,
        AutoTime = 2,
        AutoTimeAndSequential = 3,
        SemiAutomatic = 4
    }

#if NETFRAMEWORK
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.ESMDBatchPlanStartMode, Global.ACKinds.TACEnumACValueList)]
    public class ACValueListBatchPlanStartModeEnum : ACValueItemList
    {
        public ACValueListBatchPlanStartModeEnum() : base("BatchPlanStartModeEnum")
        {
            AddEntry(BatchPlanStartModeEnum.Off, "en{'Off'}de{'Aus'}");
            AddEntry(BatchPlanStartModeEnum.AutoSequential, "en{'Sequential'}de{'Sequenziell'}");
            AddEntry(BatchPlanStartModeEnum.AutoTime, "en{'Scheduling'}de{'Nach Zeitplan'}");
            AddEntry(BatchPlanStartModeEnum.AutoTimeAndSequential, "en{'Scheduling and sequential'}de{'Nach Zeitplan und Sequenziell'}");
            AddEntry(BatchPlanStartModeEnum.SemiAutomatic, "en{'Partial quantity'}de{'Nach Teilmenge'}");
        }
    }
#endif
}
