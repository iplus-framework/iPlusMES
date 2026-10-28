using gip.core.datamodel;
using gip.mes.cmdlet.DesignSync.Settings;
using gip.mes.cmdlet.Settings;
using System;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace gip.mes.cmdlet.ControlScript
{
    /// <summary>
    /// Generate iPlus resources (Designs, Texts and Messages) for selected ACClasses and save them to zip file
    /// </summary>
    [Cmdlet("Export", IPlusResourceSettings.iPlusResource)]
    public class iPlusResourceExportZipCmdlet : Cmdlet
    {
        public string VarioData { get; set; } = VBPowerShellSettings.VarioDataDefault;

        #region Parameters -> Mandatory

        /// <summary>
        /// Username - will be part of .zip file name
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string UserName { get; set; }

        /// <summary>
        /// pro
        /// </summary>

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string ProjectName { get; set; }

        /// <summary>
        /// List of class names for export
        /// </summary>

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string[] ClassNames { get; set; }

        #endregion

        #region Parameters -> Not Mandatory

        /// <summary>
        /// Location for .zip export (if not use in settings default defined)
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string SavePath { get; set; } = VBPowerShellSettings.VarioDataDefault;

        /// <summary>
        /// Exporting ACClass definition (ACClass.gip file)
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClass { get; private set; }

        /// <summary>
        /// Exporting ACClassProperty definition (ACClassProperty.gip file)
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClassProperty { get; private set; }

        /// <summary>
        /// Exporting ACClassMehtod definition (ACClassMehtod.gip file)
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClassMethod { get; private set; }

        /// <summary>
        /// Exporting ACClassPropertyRelation definition (ACClassPropertyRelation.gip file)
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClassPropertyRelation { get; private set; }

        /// <summary>
        /// Exporting ACClassConfig definition (ACClassConfig.gip file)
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClassConfig { get; private set; }

        /// <summary>
        /// Exporting ACClassMessage (ACClassMessage.gip file)
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClassMessage { get; private set; } = true;


        /// <summary>
        /// Exporting ACClassText (ACClassText.gip file)
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClassText { get; private set; } = true;

        /// <summary>
        /// Exporting ACClassDesign (ACClassDesign.gip file)
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsExportACClassDesign { get; private set; } = true;

        /// <summary>
        /// Define time of last update of any exported resource
        /// only resurces changed after this time will be exported
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public DateTime? ExportFromTime { get; set; }


        /// <summary>
        /// only resources changed this date will be exported
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public SwitchParameter FromToday
        {
            get; set;
        }

        #endregion

        protected override void ProcessRecord()
        {
            VBPowerShellSettings iPlusCmdLetSettings = FactorySettings.Factory(VarioData);

            if (gip.core.datamodel.Database.Root == null)
                ACRootFactory.Factory(iPlusCmdLetSettings.username, iPlusCmdLetSettings.password);

            using (Database database = new Database())
            {
                ACProject CurrentExportACProject = database.ACProject.FirstOrDefault(c => c.ACProjectName == ProjectName);
                ACQueryDefinition qryACProject = gip.core.datamodel.Database.Root.Queries.CreateQuery(database as IACComponent, Const.QueryPrefix + ACProject.ClassName, null);
                ACQueryDefinition qryACClass = qryACProject.ACUrlCommand(Const.QueryPrefix + ACClass.ClassName) as ACQueryDefinition;
                ACEntitySerializer aCEntitySerializer = new ACEntitySerializer();
                ACProject aCProject = database.ACProject.FirstOrDefault(c => c.ACProjectName == ProjectName);
                ACClassInfoRecursiveResult result = ACClassInfoRecursiveResult.Factory(database, ProjectName, ClassNames);
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
