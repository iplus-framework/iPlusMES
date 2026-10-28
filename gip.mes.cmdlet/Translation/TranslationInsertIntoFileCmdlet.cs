using gip.core.datamodel;
using gip.mes.cmdlet.Settings;
using gip.mes.cmdlet.Translation.Model;
using gip.mes.cmdlet.Translation.Service;
using gip.mes.cmdlet.Translation.Settings;
using System.Management.Automation;

namespace gip.mes.cmdlet.Translation
{
    /// <summary>
    /// Creates a new translation item and inserts it into a source file at the given line.
    /// </summary>
    [Cmdlet("InsertIntoFile", TranslationSettings.iPlusTranslation)]
    public class TranslationInsertIntoFileCmdlet : Cmdlet
    {

        #region Configuration
        public string VarioData { get; set; } = VBPowerShellSettings.VarioDataDefault;

        #endregion

        #region Mandatory parameters

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string ProjectName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string ClassACIdentifier { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string FileName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public int Line { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public MessageTypeEnum MessageType { get; set; }

        #endregion

        #region Optional Parameters
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string EnText { get; set; } = "-";

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string DeText { get; set; } = "-";

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string UpdateName { get; set; } = "SUP";

        #endregion

        #region Process
        protected override void ProcessRecord()
        {

            VBPowerShellSettings iPlusCmdLetSettings = FactorySettings.Factory(VarioData);
            TranslationService translationService = new TranslationService(iPlusCmdLetSettings);

            (bool success, string identifier, string message)  = translationService.NewTranslation(ProjectName, ClassACIdentifier, null, null, MessageType, EnText, DeText, UpdateName);
            if(!success)
            {
                WriteObject(string.Format(@"Failed to generate new translation item."));
                WriteObject(string.Format(@"Message: {0}.", message));
            }
            else
            {
                eMsgLevel msgLevel =eMsgLevel.Error;
                switch(MessageType)
                {
                    case MessageTypeEnum.Error:
                        msgLevel = eMsgLevel.Error;
                        break;
                    case MessageTypeEnum.Warning:
                        msgLevel = eMsgLevel.Warning;
                        break;
                    case MessageTypeEnum.Info:
                        msgLevel = eMsgLevel.Info;
                        break;
                    default:
                        msgLevel = eMsgLevel.Default;
                        break;
                }
                bool successAddedToFile = translationService.InsertTranslationIntoFile(identifier, ClassACIdentifier, FileName,Line, msgLevel, EnText, DeText);
                if(!successAddedToFile)
                {
                    WriteObject(string.Format(@"Failed to add new translation item to file."));
                }
                else
                {
                    WriteObject(string.Format(@"Successfully added new translation item to file."));
                }
            }
        }
        #endregion

    }
}
