using gip.core.datamodel;
using gip.mes.cmdlet.Settings;
using System;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace gip.mes.cmdlet.ControlScript
{
    [Cmdlet(VerbsCommon.New, "ControlSyncScript")]
    public class ControlSyncScriptNewCmdlet : Cmdlet
    {
        public string VarioData { get; set; } = VBPowerShellSettings.VarioDataDefault;

        #region Basic input parameters

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string UserName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string ProjectName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string RootFolder { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string[] ClassNames { get; set; }

        #endregion

        #region Filters

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClass { get; private set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClassProperty { get; private set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClassMethod { get; private set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClassPropertyRelation { get; private set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClassConfig { get; private set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClassMessage { get; private set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClassText { get; private set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClassDesign { get; private set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool UseExportFromTime { get; private set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public DateTime ExportFromTime { get; set; }

        #endregion

        protected override void ProcessRecord()
        {
            VBPowerShellSettings designSettings = FactorySettings.Factory(VarioData);

            if (gip.core.datamodel.Database.Root == null)
                ACRootFactory.Factory(designSettings.username, designSettings.password);

            using (Database database = new Database())
            {
                ACProject CurrentExportACProject = database.ACProject.FirstOrDefault(c => c.ACProjectName == ProjectName);
                ACQueryDefinition qryACProject = gip.core.datamodel.Database.Root.Queries.CreateQuery(database as IACComponent, Const.QueryPrefix + ACProject.ClassName, null);
                ACQueryDefinition qryACClass = qryACProject.ACUrlCommand(Const.QueryPrefix + ACClass.ClassName) as ACQueryDefinition;
                ACEntitySerializer aCEntitySerializer = new ACEntitySerializer();
                ACProject aCProject = database.ACProject.FirstOrDefault(c => c.ACProjectName == ProjectName);
                ACClassInfoRecursiveResult result = ACClassInfoRecursiveResult.Factory(database, ProjectName, ClassNames);
                ACClassInfoRecursive rootItem = result.Projects.First();
                ACClassInfoRecursive rootClassItem = rootItem.ItemsT.FirstOrDefault() as ACClassInfoRecursive;
                ExportCommand exportCommand = new ExportCommand();
                LoadExportCommandConfig(exportCommand);
                string subExportFolderName = exportCommand.DoFolder(null, null, aCEntitySerializer, qryACProject, qryACClass, aCProject, rootClassItem, RootFolder, 0, 0, UserName);
                exportCommand.DoPackage(RootFolder, subExportFolderName);
                string exportedFile = Path.Combine(RootFolder, subExportFolderName + ".zip");
                WriteObject("Exported file:");
                WriteObject(exportedFile);
            }
        }

        public void LoadExportCommandConfig(ExportCommand exportCommand)
        {
            exportCommand.IsExportACClass = IsExportACClass;
            exportCommand.IsExportACClassProperty = IsExportACClassProperty;
            exportCommand.IsExportACClassMethod = IsExportACClassMethod;
            exportCommand.IsExportACClassPropertyRelation = IsExportACClassPropertyRelation;
            exportCommand.IsExportACClassConfig = IsExportACClassConfig;
            exportCommand.IsExportACClassMessage = IsExportACClassMessage;
            exportCommand.IsExportACClassText = IsExportACClassText;
            exportCommand.IsExportACClassDesign = IsExportACClassDesign;
            exportCommand.UseExportFromTime = UseExportFromTime;
            exportCommand.ExportFromTime = ExportFromTime;
        }
    }
}
