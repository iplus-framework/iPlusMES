using gip.core.datamodel;
using gip.mes.cmdlet.Settings;
using gip.mes.cmdlet.Translation.Model;
using gip.mes.cmdlet.Translation.Service;
using gip.mes.cmdlet.Translation.Settings;
using System.Collections.Generic;
using System.Management.Automation;

namespace gip.mes.cmdlet.Translation
{
    /// <summary>
    /// Searches for translated resources stored in the ACCaptionTranslation field of tables such as
    /// ACClass, ACClassProperty, etc.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, TranslationSettings.iPlusTranslation)]

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

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public ItemTypeEnum[] ItemTypes { get; set; }

        #endregion

        #region Process

        protected override void ProcessRecord()
        {
            VBPowerShellSettings iPlusCmdLetSettings = FactorySettings.Factory(VarioData);
            TranslationService translationService = new TranslationService(iPlusCmdLetSettings);
            List<TranslationPresentation> presentations = translationService.GetTranslationPresentation(ACIdentifer,ClassACIdentifer, SearchText, ItemTypes);
            WriteObject(presentations);
        }

        #endregion
    }
}
