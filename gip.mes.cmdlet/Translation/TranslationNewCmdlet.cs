using gip.mes.cmdlet.Settings;
using gip.mes.cmdlet.Translation.Model;
using gip.mes.cmdlet.Translation.Service;
using gip.mes.cmdlet.Translation.Settings;
using System.Management.Automation;

namespace gip.mes.cmdlet.Translation
{
    /// <summary>
    /// Adds a new translation item into ACClassText or ACClassMessage.
    /// </summary>
    [Cmdlet(VerbsCommon.New, TranslationSettings.iPlusTranslation)]
    public class TranslationNewCmdlet : Cmdlet
    {
        #region Configuration
        public string VarioData { get; set; } = VBPowerShellSettings.VarioDataDefault;

        #endregion

        #region Mandatory parameters

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string ProjectName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public MessageTypeEnum MessageType { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string EnText { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string DeText { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string UpdateName { get; set; }

        #endregion

        #region Optional Parameters

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string ACIdentifier { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string ClassACIdentifier { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string ACURLComponentCached { get; set; }


        #endregion

        #region Process

        protected override void ProcessRecord()
        {
            if (string.IsNullOrEmpty(ClassACIdentifier) && string.IsNullOrEmpty(ACURLComponentCached))
            {
                WriteObject(string.Format("Missing object {0} - {1}", ClassACIdentifier, ACURLComponentCached));
                return;
            }
            VBPowerShellSettings iPlusCmdLetSettings = FactorySettings.Factory(VarioData);
            TranslationService translationService = new TranslationService(iPlusCmdLetSettings);
            (bool success, string identifier, string message) = translationService.NewTranslation(ProjectName, ClassACIdentifier, ACIdentifier, ACURLComponentCached, MessageType, EnText, DeText, iPlusCmdLetSettings.username);

            if (success)
            {
                WriteObject(string.Format(@"Successfully generated new translation item."));
                WriteObject(string.Format(@"Generated: {0}.", identifier));
            }
            else
            {
                WriteObject(string.Format(@"Failed to generate new translation item."));
            }
            WriteObject(string.Format(@"Message: {0}.", message));
        }

        #endregion
    }
}
