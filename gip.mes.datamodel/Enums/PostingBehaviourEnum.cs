﻿using gip.core.datamodel;
using System.Runtime.Serialization;

namespace gip.mes.datamodel
{


    [ACClassInfo(Const.PackName_VarioFacility, "en{'Posting behaviour'}de{'Buchungsverhalten'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListPostingBehaviourEnum : ACValueItemList
    {
        public ACValueListPostingBehaviourEnum() : base("PostingBehaviourEnum")
        {
            AddEntry(PostingBehaviourEnum.DoNothing, "en{'No Action'}de{'Keine Aktion'}");
            AddEntry(PostingBehaviourEnum.BlockOnRelocation, "en{'Blocks new quants after relocation'}de{'Sperrt neue Quanten nach der Umlagerung'}");
            AddEntry(PostingBehaviourEnum.ZeroStockOnRelocation, "en{'Set the stock to zero after relocation'}de{'Setzt quanten auf Nullbestand nach einer Umlagerung'}");
            AddEntry(PostingBehaviourEnum.ZeroStockOnProduction, "en{'Set the stock to zero after production'}de{'Setzt quanten auf Nullbestand nach einer Produktion'}");
            AddEntry(PostingBehaviourEnum.ZeroStockAlways, "en{'Set the stock to zero always'}de{'Setzt quanten immer auf Nullbestand'}");
            AddEntry(PostingBehaviourEnum.ZeroStockOnProductionIfNoBOMUsage, "en{'Set the stock to zero after production if no usage in further BOMs'}de{'Nullbestand nach einer Produktion falls keine weitere Stücklistenverwendung'}");
        }
    }
}
