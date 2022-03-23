using gip.core.datamodel;

namespace gip.mes.datamodel
{
    /// <summary>
    /// Enum BatchPlanMode
    /// </summary>
#if NETFRAMEWORK
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, ConstApp.ESMDBatchPlanMode, Global.ACKinds.TACEnum, QRYConfig = "gip.mes.datamodel.ACValueListBatchPlanMode")]
#else
        [DataContract]
#endif

    public enum BatchPlanMode : short
    {
        UseFromTo = 0,
        UseBatchCount = 1,
        UseTotalSize = 2,
    }


#if NETFRAMEWORK
    [ACClassInfo(Const.PackName_VarioAutomation, ConstApp.ESMDBatchPlanMode, Global.ACKinds.TACEnumACValueList)]
    public class ACValueListBatchPlanMode : ACValueItemList
    {
        public ACValueListBatchPlanMode() : base("BatchPlanMode")
        {
            AddEntry((short)BatchPlanMode.UseFromTo, "en{'Use from/to values'}de{'Nach Von/Bis-Batch-Nr.'}");
            AddEntry((short)BatchPlanMode.UseBatchCount, "en{'Use target batch count'}de{'Nach Soll-Batchzahl'}");
            AddEntry((short)BatchPlanMode.UseTotalSize, "en{'Use total size'}de{'Nach Gesamtgröße'}");

        }
    }
#endif
}
