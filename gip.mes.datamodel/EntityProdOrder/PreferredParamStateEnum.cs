using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'PreferredParamStateEnum'}de{'PreferredParamStateEnum'}", Global.ACKinds.TACEnum, QRYConfig = "gip.mes.datamodel.ACValueListPreferredParamStateEnum")]

    public enum PreferredParamStateEnum : short
    {
        ParamsNotRequired = 0,
        ParamsRequiredNotDefined = 1,
        ParamsRequiredDefined = 2
    }

    [ACClassInfo(Const.PackName_VarioLogistics, "en{'ACValueListPreferredParamStateEnum'}de{'ACValueListPreferredParamStateEnum'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListPreferredParamStateEnum : ACValueItemList
    {
        public ACValueListPreferredParamStateEnum() : base("WizardSetupState")
        {
            AddEntry(PreferredParamStateEnum.ParamsNotRequired, "en{'Params not required'}de{'Parameter nicht erforderlich'}");
            AddEntry(PreferredParamStateEnum.ParamsRequiredNotDefined, "en{'Params required but not defined'}de{'Parameter erforderlich, aber nicht definiert'}");
            AddEntry(PreferredParamStateEnum.ParamsRequiredDefined, "en{'Params required and defined'}de{'Erforderliche und definierte Parameter'}");
        }
    }
}
