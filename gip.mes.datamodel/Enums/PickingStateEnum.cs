using gip.core.datamodel;

namespace gip.mes.datamodel
{
#if NETFRAMEWORK
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Picking state'}de{'Kommissionsstatus'}", Global.ACKinds.TACEnum, QRYConfig = "gip.mes.datamodel.ACValueListPickingStateEnum")]
#else
    [DataContract]
#endif

    public enum PickingStateEnum : short
    {
        New = 0,
        InProcess = 1,
        Finished = 2,
        Cancelled = 3,
        WaitOnManualClosing = 4,
        WFReadyToStart = 5,
        WFActive = 6,
    }


    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Picking state'}de{'Zustand der Kommissionierung'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListPickingStateEnum : ACValueItemList
    {
        public ACValueListPickingStateEnum() : base("PickingState")
        {
            AddEntry(PickingStateEnum.New, "en{'New'}de{'Neu'}");
            AddEntry(PickingStateEnum.InProcess, "en{'In process'}de{'In Bearbeitung'}");
            AddEntry(PickingStateEnum.WFReadyToStart, "en{'Workflow ready to start'}de{'Workflow Startbereit'}");
            AddEntry(PickingStateEnum.WFActive, "en{'Workflow Active'}de{'Workflow Aktiv'}");
            AddEntry(PickingStateEnum.Finished, "en{'Finished'}de{'Fertiggestellt'}");
            AddEntry(PickingStateEnum.Cancelled, "en{'Cancelled'}de{'Storniert'}");
            AddEntry(PickingStateEnum.WaitOnManualClosing, "en{'Evaluating and completing'}de{'Bewerten und Abschließen'}");
        }
    }
}
