using gip.core.datamodel;
using gip.mes.cmdlet.Settings;
using gip.mes.cmdlet.Translation.Model;
using gip.mes.cmdlet.Translation.Service;
using gip.mes.cmdlet.Translation.Settings;
using System;
using System.Linq;
using System.Management.Automation;

namespace gip.mes.cmdlet.Translation
{
    /// <summary>
    /// Sets the translation value in the ACCaptionTranslation field of the tables that have it.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, TranslationSettings.iPlusTranslation)]

    public class TranslationSetCmdlet : Cmdlet
    {

        #region Configuration
        public string VarioData { get; set; } = VBPowerShellSettings.VarioDataDefault;

        #endregion

        #region Parameters

        #region Parameters -> Mandatory


        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public ItemTypeEnum ItemType { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string ACIdentifer { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string EnText { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string DeText { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string UpdateName { get; set; }

        #endregion

        #region Parameters -> Optional

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public Guid? ID { get; set; }

        #endregion

        #endregion

        #region Process

        protected override void ProcessRecord()
        {
            bool isTranslationEntered =
                !string.IsNullOrEmpty(ACIdentifer) &&
                !string.IsNullOrEmpty(EnText) &&
                !string.IsNullOrEmpty(DeText);

            if (isTranslationEntered)
            {
                VBPowerShellSettings iPlusCmdLetSettings = FactorySettings.Factory(VarioData);
                TranslationService translationService = new TranslationService(iPlusCmdLetSettings);
                (bool success, string message) = translationService.SetTranslation(ACIdentifer, ItemType, EnText, DeText, UpdateName, ID);
                if (success)
                {
                    WriteObject(message);
                }
                else
                {
                    WriteError(new ErrorRecord(new Exception(message), "TranslationError", ErrorCategory.InvalidOperation, this));
                }
            }
        }

        #endregion
    }
}
