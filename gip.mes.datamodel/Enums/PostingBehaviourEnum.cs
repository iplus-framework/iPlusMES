using gip.core.datamodel;
using System.Runtime.Serialization;

namespace gip.mes.datamodel
{


    [ACClassInfo(Const.PackName_VarioFacility, "en{'Posting behaviour'}de{'Buchungsverhalten'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListPostingBehaviourEnum : ACValueItemList
    {
        public ACValueListPostingBehaviourEnum() : base("PostingBehaviourEnum")
        {
            AddEntry((short)PostingBehaviourEnum.DoNothing, "en{'No Action'}de{'Keine Aktion'}");
            AddEntry((short)PostingBehaviourEnum.BlockOnRelocation, "en{'Blocks new quants after relocation'}de{'Sperrt neue Quanten nach der Umlagerung'}");
            AddEntry((short)PostingBehaviourEnum.ZeroStockOnRelocation, "en{'Set the stock to zero after relocation'}de{'Setzt quanten auf Nullbestand nach einer Umlagerung'}");
        }
    }
}
