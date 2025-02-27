// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using gip.core.datamodel;
using System.Linq;
using System.Management.Automation;
using gip.bso.iplus;
using gip.core.autocomponent;
using gip.mes.cmdlet.Settings;

namespace gip.mes.cmdlet.DesignSync
{
    /// <summary>
    /// Save resources from ACClass: class, properties, texts, designs, relations
    /// to .gip and .xml files on hard drive
    /// </summary>
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
                VBPowerShellSettings iPlusCmdLetSettings = FactorySettings.Factory(VarioData);
                string rootFolder = iPlusCmdLetSettings.RootFolder;

                if (gip.core.datamodel.Database.Root == null)
                    ACRootFactory.Factory(iPlusCmdLetSettings.username, iPlusCmdLetSettings.password);

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
