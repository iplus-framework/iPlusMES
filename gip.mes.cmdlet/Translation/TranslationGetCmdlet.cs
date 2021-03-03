using gip.core.datamodel;
using gip.mes.cmdlet.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace gip.mes.cmdlet.Translation
{
    [Cmdlet(VerbsCommon.Get, CmdLetSettings.TranslationCmdlet_Name)]

    public class TranslationGetCmdlet : Cmdlet
    {
        #region Configuration
        public string VarioData { get; set; } = VBPowerShellSettings.VarioDataDefault;

        #endregion

        #region Parameters
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string ClassACIdentifer { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string ACIdentifer { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string SearchText { get; set; }
        #endregion

        protected override void ProcessRecord()
        {
            VBPowerShellSettings designSettings = FactorySettings.Factory(VarioData);

            List<TranslationPresentation> presentations = new List<TranslationPresentation>();

            bool searchParamsEmpty = string.IsNullOrEmpty(ClassACIdentifer) && string.IsNullOrEmpty(ACIdentifer) && string.IsNullOrEmpty(SearchText);

            if (!searchParamsEmpty)
            {
                using (Database database = new Database())
                {

                    // ACClass
                    ItemTypeEnum itemType = ItemTypeEnum.ACClass;
                    List<ACClass> classes =
                        database
                        .ACClass
                        .Where(c =>
                            (ClassACIdentifer == null || c.ACIdentifier.Contains(ClassACIdentifer))
                            &&
                            (ACIdentifer == null || c.ACIdentifier.Contains(ACIdentifer))
                            &&
                            (SearchText == null || c.ACCaptionTranslation.Contains(SearchText))
                         )
                        .ToList();
                    presentations.AddRange(classes.Select(c => new TranslationPresentation()
                    {
                        ItemType = itemType,
                        ClassACIdentifier = c.ACIdentifier,
                        ACIdentifier = c.ACIdentifier,
                        ACCaptionTranslation = c.ACCaptionTranslation,
                        UpdateDate = c.UpdateDate,
                        UpdateName = c.UpdateName
                    }));

                    // ACClassDesign
                    itemType = ItemTypeEnum.ACClassDesign;
                    List<ACClassDesign> designs =
                        database
                        .ACClassDesign
                        .Where(c =>
                            (ClassACIdentifer == null || c.ACClass.ACIdentifier.Contains(ClassACIdentifer))
                            &&
                            (ACIdentifer == null || c.ACIdentifier.Contains(ACIdentifer))
                            &&
                            (SearchText == null || c.ACCaptionTranslation.Contains(SearchText))
                         )
                        .ToList();
                    presentations.AddRange(designs.Select(c => new TranslationPresentation()
                    {
                        ItemType = itemType,
                        ClassACIdentifier = c.ACClass.ACIdentifier,
                        ACIdentifier = c.ACIdentifier,
                        ACCaptionTranslation = c.ACCaptionTranslation,
                        UpdateDate = c.UpdateDate,
                        UpdateName = c.UpdateName
                    }));

                    // ACClassMessage
                    itemType = ItemTypeEnum.ACClassMessage;
                    List<ACClassMessage> messages =
                        database
                        .ACClassMessage
                        .Where(c =>
                            (ClassACIdentifer == null || c.ACClass.ACIdentifier.Contains(ClassACIdentifer))
                            &&
                            (ACIdentifer == null || c.ACIdentifier.Contains(ACIdentifer))
                            &&
                            (SearchText == null || c.ACCaptionTranslation.Contains(SearchText))
                         )
                        .ToList();
                    presentations.AddRange(messages.Select(c => new TranslationPresentation()
                    {
                        ItemType = itemType,
                        ClassACIdentifier = c.ACClass.ACIdentifier,
                        ACIdentifier = c.ACIdentifier,
                        ACCaptionTranslation = c.ACCaptionTranslation,
                        UpdateDate = c.UpdateDate,
                        UpdateName = c.UpdateName
                    }));

                    // ACClassProperty
                    itemType = ItemTypeEnum.ACClassProperty;
                    List<ACClassProperty> properties =
                        database
                        .ACClassProperty
                        .Where(c =>
                             (ClassACIdentifer == null || c.ACClass.ACIdentifier.Contains(ClassACIdentifer))
                            &&
                            (ACIdentifer == null || c.ACIdentifier.Contains(ACIdentifer))
                            &&
                            (SearchText == null || c.ACCaptionTranslation.Contains(SearchText))
                         )
                        .ToList();
                    presentations.AddRange(properties.Select(c => new TranslationPresentation()
                    {
                        ItemType = itemType,
                        ClassACIdentifier = c.ACClass.ACIdentifier,
                        ACIdentifier = c.ACIdentifier,
                        ACCaptionTranslation = c.ACCaptionTranslation,
                        UpdateDate = c.UpdateDate,
                        UpdateName = c.UpdateName
                    }));

                    // ACClassText
                    itemType = ItemTypeEnum.ACClassMessage;
                    List<ACClassText> texts =
                        database
                        .ACClassText
                        .Where(c =>
                            (ClassACIdentifer == null || c.ACClass.ACIdentifier.Contains(ClassACIdentifer))
                            &&
                            (ACIdentifer == null || c.ACIdentifier.Contains(ACIdentifer))
                            &&
                            (SearchText == null || c.ACCaptionTranslation.Contains(SearchText))
                         )
                        .ToList();
                    presentations.AddRange(texts.Select(c => new TranslationPresentation()
                    {
                        ItemType = itemType,
                        ClassACIdentifier = c.ACClass.ACIdentifier,
                        ACIdentifier = c.ACIdentifier,
                        ACCaptionTranslation = c.ACCaptionTranslation,
                        UpdateDate = c.UpdateDate,
                        UpdateName = c.UpdateName
                    }));

                }
            }

            WriteObject(presentations);
        }
    }
}
