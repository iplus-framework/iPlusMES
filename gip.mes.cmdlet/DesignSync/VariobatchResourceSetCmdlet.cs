using System;
using gip.core.datamodel;
using System.Linq;
using System.Management.Automation;
using gip.bso.iplus;
using gip.core.autocomponent;
using gip.mes.cmdlet.Settings;

namespace gip.mes.cmdlet.DesignSync
{
    [Cmdlet(VerbsCommon.Set, "VariobatchResource")]
    public class VariobatchResourceSetCmdlet : Cmdlet
    {

        #region Mandatory Parameters

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string ProjectName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string[] ClassNames { get; set; }

        #endregion

        #region Optional Parameters

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string[] ItemNames { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string UserName { get; set; } = "SUP";

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
            VBPowerShellSettings designSettings = FactorySettings.Factory(VarioData);
            string rootFolder = designSettings.RootFolder;

            if (gip.core.datamodel.Database.Root == null)
                ACRootFactory.Factory(designSettings.username, designSettings.password);

            using (Database database = new Database())
            {
                ACProjectManager aCProjectManager = new ACProjectManager(database, ACRoot.SRoot);
                ACClassImporter importer = new ACClassImporter(rootFolder, aCProjectManager, database, UserName, !OmitExportACClassDesign, !OmitExportACClassText, !OmitExportACClassMessage);
                importer.OnImportMessage += Importer_OnImportMessage;
                ACProject variobatchProject = database.ACProject.FirstOrDefault(c => c.ACProjectName == ProjectName);
                importer.Import(variobatchProject, ClassNames, ItemNames);
            }

        }

        private void Importer_OnImportMessage(string msg)
        {
            WriteObject(msg);
        }
    }
}
