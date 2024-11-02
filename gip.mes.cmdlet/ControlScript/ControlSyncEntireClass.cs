// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.cmdlet.Settings;
using System;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace gip.mes.cmdlet.ControlScript
{
    [Cmdlet(VerbsCommon.Push, CmdLetSettings.ControlSyncScriptCmdlet_Name)]
    public class ControlSyncEntireClass : Cmdlet
    {
        public string VarioData { get; set; } = VBPowerShellSettings.VarioDataDefault;

        #region Parameters -> Mandatory

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string UserName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string ProjectName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string[] ClassNames { get; set; }

        #endregion

        #region Parameters -> Not mandatory
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public DateTime? ExportFromTime { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string SavePath { get; set; } = VBPowerShellSettings.VarioDataDefault;
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
                ACClassInfoRecursive rootClassItem = rootItem.ItemsT.FirstOrDefault() as ACClassInfoRecursive;
                ExportCommand exportCommand = new ExportCommand();

                exportCommand.IsExportACClass = false;
                exportCommand.IsExportACClassProperty = false;
                exportCommand.IsExportACClassMethod = false;
                exportCommand.IsExportACClassPropertyRelation = false;
                exportCommand.IsExportACClassConfig = false;
                exportCommand.IsExportACClassMessage = true;
                exportCommand.IsExportACClassText = true;
                exportCommand.IsExportACClassDesign = true;

                if (ExportFromTime != null)
                {
                    exportCommand.UseExportFromTime = true;
                    exportCommand.ExportFromTime = ExportFromTime.Value;
                }

                string subExportFolderName = exportCommand.DoFolder(null, null, aCEntitySerializer, qryACProject, qryACClass, aCProject, rootClassItem, SavePath, 0, 0, UserName);
                exportCommand.DoPackage(SavePath, subExportFolderName);
                string exportedFile = Path.Combine(SavePath, subExportFolderName + ".zip");
                WriteObject("Exported file:");
                WriteObject(exportedFile);
            }
        }
    }
}
