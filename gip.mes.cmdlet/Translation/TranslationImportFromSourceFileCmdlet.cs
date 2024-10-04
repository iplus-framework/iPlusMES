using gip.core.datamodel;
using gip.mes.cmdlet.Settings;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace gip.mes.cmdlet.Translation
{
    [Cmdlet(VerbsCommon.Set, CmdLetSettings.TranslationFromSourceFileCmdlet_Name)]
    public class TranslationImportFromSourceFileCmdlet : Cmdlet
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
                        TranslationFromFileService translationServiceFromFile = new TranslationFromFileService();
                        MsgWithDetails msg = translationServiceFromFile.UpdateTranslations(database, project, FileName, UpdateName);
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
