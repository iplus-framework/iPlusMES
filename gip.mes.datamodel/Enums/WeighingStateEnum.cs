using gip.core.datamodel;

namespace gip.mes.datamodel
{
#if NETFRAMEWORK
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Weighing state'}de{'Wiegestatus'}", Global.ACKinds.TACEnum, QRYConfig = "gip.mes.datamodel.ACValueListWeighingStateEnum")]
#else
    [DataContract]
#endif

    public enum WeighingStateEnum : short
    {
        New = 0,
        InProcess = 1,
        Finished = 2,
        Cancelled = 3,
    }


    [ACClassInfo(Const.PackName_VarioLogistics, ConstApp.WeighingState, Global.ACKinds.TACEnumACValueList)]
    public class ACValueListWeighingStateEnum : ACValueItemList
    {
        public ACValueListWeighingStateEnum() : base("WeighingState")
        {
            AddEntry(WeighingStateEnum.New, Const.New);
            AddEntry(WeighingStateEnum.InProcess, "en{'In process'}de{'In Bearbeitung'}");
            AddEntry(WeighingStateEnum.Finished, "en{'Finished'}de{'Fertiggestellt'}");
            AddEntry(WeighingStateEnum.Cancelled, Const.Canceled);
        }
    }
}
