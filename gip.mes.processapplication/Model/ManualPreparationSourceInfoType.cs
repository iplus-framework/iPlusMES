using gip.core.datamodel;
using System.Runtime.Serialization;

namespace gip.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ManualPreparationSourceInfoType'}de{'ManualPreparationSourceInfoType'}", Global.ACKinds.TACEnum, Global.ACStorableTypes.NotStorable, true, false)]
    [DataContract]
    public enum ManualPreparationSourceInfoTypeEnum : short
    {
        [EnumMember(Value = "FacilityID")]
        FacilityID = 0,
        [EnumMember(Value = "FacilityChargeID")]
        FacilityChargeID = 10
    }
}
