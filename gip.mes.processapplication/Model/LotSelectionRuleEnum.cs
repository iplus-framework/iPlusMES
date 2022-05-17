using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Lot selection rule enum'}de{'Lot selection rule enum'}", Global.ACKinds.TACEnum, Global.ACStorableTypes.NotStorable, true, false)]
    [DataContract]
    public enum LotSelectionRuleEnum : short
    {
        [EnumMember(Value = "LSR0")]
        None = 0,
        [EnumMember(Value = "LSR10")]
        SelectAndScan = 10,
        [EnumMember(Value = "LSR20")]
        OnlySelect = 20,
        [EnumMember(Value = "LSR30")]
        OnlyScan = 30

    }
}
