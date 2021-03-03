using System;
using gip.core.datamodel;
using System.Linq;
using System.Management.Automation;
using gip.bso.iplus;
using gip.core.autocomponent;
using gip.mes.cmdlet.Settings;

namespace gip.mes.cmdlet.DesignSync
{
    [Cmdlet(VerbsCommon.Get, CmdLetSettings.iPlusResourceCmdlet_Name)]
    public class iPlusResourceGetCmdlet : Cmdlet
    {

        #region Mandatory parameters
       
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string ProjectName { get; set; }

        #endregion

        #region Optional Params

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string[] ClassNames { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string VarioData { get; set; } = VBPowerShellSettings.VarioDataDefault;

        [Parameter(
        Mandatory = false,
        ParameterSetName = "ExportOptions",
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
        public SwitchParameter OmitExportACClassDesign { get; set; } = false;


        [Parameter(
        Mandatory = false,
        ParameterSetName = "ExportOptions",
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
        public SwitchParameter OmitExportACClassText { get; set; } = false;

        [Parameter(
        Mandatory = false,
        ParameterSetName = "ExportOptions",
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
        public SwitchParameter OmitExportACClassMessage { get; set; } = false;

        #endregion

        protected override void ProcessRecord()
        {
            try
            {
                VBPowerShellSettings designSettings = FactorySettings.Factory(VarioData);
                string rootFolder = designSettings.RootFolder;

                if (gip.core.datamodel.Database.Root == null)
                    ACRootFactory.Factory(designSettings.username, designSettings.password);

                ExportCommandExtended exportCommand = new ExportCommandExtended();
                exportCommand.IsExportACClassDesign = !OmitExportACClassDesign;
                exportCommand.IsExportACClassText = !OmitExportACClassText;
                exportCommand.IsExportACClassMessage = !OmitExportACClassMessage;

                using (Database database = new Database())
                {
                    ACProjectManager aCProjectManager = new ACProjectManager(database, ACRoot.SRoot);
                    ACClassExporter exporter = new ACClassExporter(rootFolder, aCProjectManager, database, exportCommand);
                    exporter.OnImportMessage += Exporter_OnImportMessage;
                    exporter.ExportCommand.ExportProgressEvent += ExportCommand_ExportProgressEvent;
                    ACProject acRootProject = database.ACProject.FirstOrDefault(c => c.ACProjectName == ProjectName);
                    exporter.Export(acRootProject, ClassNames);
                }
            }
            catch(Exception ec)
            {
                WriteObject(ec);
            }
        }

        private void ExportCommand_ExportProgressEvent(int currentItem, string progressMessage)
        {
            WriteObject(progressMessage);
        }

        private void Exporter_OnImportMessage(string msg)
        {
            WriteObject(msg);
        }
    }
}
