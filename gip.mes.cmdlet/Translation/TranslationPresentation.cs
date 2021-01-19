using System;

namespace gip.mes.cmdlet.Translation
{
    public class TranslationPresentation
    {
        public ItemTypeEnum ItemType { get; set; }
        public string ClassACIdentifier { get; set; }
        public string ACIdentifier { get; set; }
        public string ACCaptionTranslation { get; set; }


        public string UpdateName { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
