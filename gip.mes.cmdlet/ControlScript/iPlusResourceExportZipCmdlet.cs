using gip.core.datamodel;
using gip.mes.cmdlet.ControlScript.Model;
using gip.mes.cmdlet.DesignSync.Settings;
using gip.mes.cmdlet.Settings;
using System;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace gip.mes.cmdlet.ControlScript
{
    /// <summary>
    /// Generates iPlus resources (designs, texts and messages) for the selected ACClasses and saves them to a .zip file.
    /// </summary>
    [Cmdlet("Export", IPlusResourceSettings.iPlusResource)]
    public class iPlusResourceExportZipCmdlet : Cmdlet
    {
        public string VarioData { get; set; } = VBPowerShellSettings.VarioDataDefault;

        #region Parameters -> Mandatory

       
        /// <summary>
        /// List of class names to export.
        /// </summary>

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string[] ClassNames { get; set; }

        #endregion

        #region Parameters -> Not Mandatory

        /// <summary>
        /// Project of the ACClasses to export.
        /// Root - MES library
        /// Variolibrary - iPlus library
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public ProjectEnum Project { get; set; } = ProjectEnum.Root;

        /// <summary>
        /// Location for the .zip export (if not set, the default from the settings is used).
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string SavePath { get; set; } = VBPowerShellSettings.VarioDataDefault;

        /// <summary>
        /// Exports the ACClass definition (ACClass.gip file).
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClass { get; private set; }

        /// <summary>
        /// Exports the ACClassProperty definition (ACClassProperty.gip file).
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClassProperty { get; private set; }

        /// <summary>
        /// Exports the ACClassMethod definition (ACClassMethod.gip file).
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClassMethod { get; private set; }

        /// <summary>
        /// Exports the ACClassPropertyRelation definition (ACClassPropertyRelation.gip file).
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClassPropertyRelation { get; private set; }

        /// <summary>
        /// Exports the ACClassConfig definition (ACClassConfig.gip file).
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClassConfig { get; private set; }

        /// <summary>
        /// Exports ACClassMessage (ACClassMessage.gip file).
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClassMessage { get; private set; } = true;


        /// <summary>
        /// Exports ACClassText (ACClassText.gip file).
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClassText { get; private set; } = true;

        /// <summary>
        /// Exports ACClassDesign (ACClassDesign.gip file).
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClassDesign { get; private set; } = true;

        /// <summary>
        /// Defines the time of the last update of any exported resource;
        /// only resources changed after this time are exported.
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public DateTime? ExportFromTime { get; set; }


        /// <summary>
        /// Only resources changed on this date are exported.
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public SwitchParameter FromToday
        {
            get; set;
        }

        /// <summary>
        /// User name; becomes part of the .zip file name.
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string UserName { get; set; } = "sup";


        #endregion

        protected override void ProcessRecord()
        {
            VBPowerShellSettings iPlusCmdLetSettings = FactorySettings.Factory(VarioData);

            if (gip.core.datamodel.Database.Root == null)
                ACRootFactory.Factory(iPlusCmdLetSettings.username, iPlusCmdLetSettings.password);

            using (Database database = new Database())
            {
                string projectName = Project.ToString();
                ACProject CurrentExportACProject = database.ACProject.FirstOrDefault(c => c.ACProjectName == projectName);
                ACQueryDefinition qryACProject = gip.core.datamodel.Database.Root.Queries.CreateQuery(database as IACComponent, Const.QueryPrefix + ACProject.ClassName, null);
                ACQueryDefinition qryACClass = qryACProject.ACUrlCommand(Const.QueryPrefix + ACClass.ClassName) as ACQueryDefinition;
                ACEntitySerializer aCEntitySerializer = new ACEntitySerializer();
                ACProject aCProject = database.ACProject.FirstOrDefault(c => c.ACProjectName == projectName);
                ACClassInfoRecursiveResult result = ACClassInfoRecursiveResult.Factory(database, projectName, ClassNames);
                ACClassInfoRecursive rootItem = result.Projects.First();
                
                ExportCommand exportCommand = new ExportCommand();
                LoadExportCommandConfig(exportCommand);

                string subExportFolderName = exportCommand.GetFolderName(UserName);

                foreach(var item in rootItem.ItemsT)
                {
                    ACClassInfoRecursive rootClassItem = item as ACClassInfoRecursive;
                    exportCommand.DoFolderFromName(null, null, aCEntitySerializer, qryACProject, qryACClass, aCProject, rootClassItem, SavePath, 0, 0, subExportFolderName);
                }
                
                exportCommand.DoPackage(SavePath, subExportFolderName);
                string exportedFile = Path.Combine(SavePath, subExportFolderName + ".zip");
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
            if (ExportFromTime != null || FromToday.IsPresent)
            {
                DateTime dateTime = DateTime.Now.Date;
                if (ExportFromTime != null)
                {
                    dateTime = ExportFromTime.Value;
                }
                exportCommand.UseExportFromTime = true;
                exportCommand.ExportFromTime = dateTime;
            }
        }
    }
}
