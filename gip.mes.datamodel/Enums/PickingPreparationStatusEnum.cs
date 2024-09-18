using gip.core.datamodel;

namespace gip.mes.datamodel
{
#if NETFRAMEWORK
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, ConstApp.PickingPreparationStatus, Global.ACKinds.TACEnum, QRYConfig = "gip.mes.datamodel.ACValueListPickingPreparationStatusEnum")]
#else
    [DataContract]
#endif
    public enum PickingPreparationStatusEnum : short
    {
        None = 0,
        Partial = 1,
        Full = 2
    }

    [ACClassInfo(Const.PackName_VarioLogistics, ConstApp.PickingPreparationStatus, Global.ACKinds.TACEnumACValueList)]
    public class ACValueListPickingPreparationStatusEnum : ACValueItemList
    {
        public ACValueListPickingPreparationStatusEnum() : base("PreparationStatus")
        {
            AddEntry(PickingPreparationStatusEnum.None, "en{'Not provided'}de{'Nicht bereitgestellt'}");
            AddEntry(PickingPreparationStatusEnum.Partial, "en{'Partial provided'}de{'Teilmenge bereitgestellt'}");
            AddEntry(PickingPreparationStatusEnum.Full, "en{'Fully provided'}de{'Vollständig bereitgestellt'}");
        }
    }
}
