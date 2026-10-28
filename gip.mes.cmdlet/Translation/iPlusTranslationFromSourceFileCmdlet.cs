using gip.core.datamodel;
using gip.mes.cmdlet.Settings;
using gip.mes.cmdlet.Translation.Service;
using gip.mes.cmdlet.Translation.Settings;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace gip.mes.cmdlet.Translation
{
    /// <summary>
    /// Updates translations in the database from a source file.
    /// </summary>
    [Cmdlet("ReadFromFile", TranslationSettings.iPlusTranslation)]
    public class iPlusTranslationFromSourceFileCmdlet : Cmdlet
    {
        #region Configuration
        public string VarioData { get; set; } = VBPowerShellSettings.VarioDataDefault;

        #endregion

        #region Mandatory parameters

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string ProjectName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string FileName { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string UpdateName { get; set; }

        #endregion

        #region Process

        protected override void ProcessRecord()
        {
            if (File.Exists(FileName))
            {
                using (Database database = new Database())
                {
                    ACProject project = database.ACProject.Where(c=>c.ACProjectName == ProjectName).FirstOrDefault();
                    if(project != null)
                    {
                        VBPowerShellSettings iPlusCmdLetSettings = FactorySettings.Factory(VarioData);
                        TranslationService translationService = new TranslationService(iPlusCmdLetSettings);
                        MsgWithDetails msg = translationService.UpdateTranslationsFromFile(database, project, FileName, UpdateName);
                        if(msg.MsgDetails.Any())
                        {
                            WriteObject(msg.DetailsAsText);
                        }
                    }
                    else
                    {
                        WriteObject(string.Format($"Missing project:{ProjectName}"));
                    }
                }
            }
            else
            {
                WriteObject(string.Format($"Missing file:{FileName}"));
            }
        }

        #endregion
    }
}
