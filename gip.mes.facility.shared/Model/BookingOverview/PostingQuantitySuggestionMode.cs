// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
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
        [EnumMember]
        ProportionallyAnotherComp = 30
    }

#if NETFRAMEWORK
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Mode for posting quantity suggestion'}de{'Modus für den Vorschlag der Buchungsmenge'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListPostingQuantitySuggestionMode : ACValueItemList
    {
        public ACValueListPostingQuantitySuggestionMode() : base(nameof(PostingQuantitySuggestionMode))
        {
            AddEntry(PostingQuantitySuggestionMode.None, "en{'None'}de{'Keine'}");
            AddEntry(PostingQuantitySuggestionMode.OrderQuantity, "en{'According order quantity'}de{'Entsprechend Auftragsgröße'}");
            AddEntry(PostingQuantitySuggestionMode.ForceQuantQuantity, "en{'Force quant quantity'}de{'Übernehme Quantmenge'}");
            AddEntry(PostingQuantitySuggestionMode.ProportionallyAnotherComp, "en{'Proportionally to main component'}de{'Proportional zur Hauptkomponente'}");
        }
    }
#endif
}
