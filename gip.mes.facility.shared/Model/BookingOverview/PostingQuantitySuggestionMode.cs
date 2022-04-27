using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.facility
{
    [DataContract]
#if NETFRAMEWORK
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Mode for posting quantity suggestion'}de{'Modus für die Buchung von Mengenvorschlägen'}", Global.ACKinds.TACEnum, QRYConfig = "gip.mes.facility.ACValueListPostingQuantitySuggestionMode")]
#endif
    public enum PostingQuantitySuggestionMode : short
    {
        [EnumMember]
        OrderQuantity = 0, 
        [EnumMember]
        None = 10,
        [EnumMember]
        ForceQuantQuantity = 20,
        ProportionallyAnotherComp = 30
    }

#if NETFRAMEWORK
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Mode for posting quantity suggestion'}de{'Modus für den Vorschlag der Buchungsmenge'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListPostingQuantitySuggestionMode : ACValueItemList
    {
        public ACValueListPostingQuantitySuggestionMode() : base(nameof(PostingQuantitySuggestionMode))
        {
            AddEntry(PostingQuantitySuggestionMode.None, "en{'None'}de{'Keine'}");
            AddEntry(PostingQuantitySuggestionMode.OrderQuantity, "en{'According order quantity'}de{'Nach Bestellmenge'}");
            AddEntry(PostingQuantitySuggestionMode.ForceQuantQuantity, "en{'Force quant quantity'}de{'Kraft-Quant-Menge'}");
        }
    }
#endif
}
