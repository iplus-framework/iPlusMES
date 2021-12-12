using gip.core.datamodel;
using System.Runtime.Serialization;

namespace gip.mes.datamodel
{
    //[DataContract]
    //[ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Posting behaviour'}de{'Buchungsverhalten'}", Global.ACKinds.TACEnum, QRYConfig = "gip.mes.datamodel.ACValueListPostingBehaviourEnum")]
    public enum PostingBehaviourEnum : short
    {
        /// <summary>
        /// Do nothing
        /// </summary>
        DoNothing = 0,

        /// <summary>
        /// Blocks the new added quants after a relocation posting
        /// This is used for pickig orders when material is relocated to another store and the receiver has to acknowledg the receipt of goods.
        /// Otherwise the production can't use this material until the material is blocked.
        /// </summary>
        BlockOnRelocation = 1,

        /// <summary>
        /// Sets the stock to zero after a relocation posting. 
        /// This is used for scenarios where a other System is master of a storage location
        /// </summary>
        ZeroStockOnRelocation = 2
    }

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
