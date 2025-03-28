// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.core.dbsyncer.Command;
using gip.core.dbsyncer.helper;
using gip.core.dbsyncer.model;
using gip.mes.cmdlet.Settings;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace gip.mes.cmdlet.DBSync
{
    /// <summary>
    /// delete DBSyncer file from hard drive and from database
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, CmdLetSettings.DBSyncCmdlet_Name)]
    public class DBSyncScriptRemoveCmdLet : Cmdlet
    {
        #region Config
        public string VarioData { get; set; } = VBPowerShellSettings.VarioDataDefault;

        #endregion

        #region Mandatory Params

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string DbSyncerInfoContextID { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string FileName { get; set; }

        #endregion

        protected override void ProcessRecord()
        {
            VBPowerShellSettings iPlusCmdLetSettings = FactorySettings.Factory(VarioData);
            string connectionString = DbSyncerSettings.GetDefaultConnectionString(CommandLineHelper.ConfigCurrentDir);

            string rootFolder = Path.Combine(iPlusCmdLetSettings.DLLBinFolder, "DbScripts", DbSyncerInfoContextID);
            gip.core.datamodel.DbSyncerInfoContext dbSyncerInfoContext = new gip.core.datamodel.DbSyncerInfoContext() { DbSyncerInfoContextID = DbSyncerInfoContextID };
            FileInfo fi = new FileInfo(Path.Combine(rootFolder, FileName));
            if (fi.Exists)
            {
                ScriptFileInfo scriptFileInfo = new ScriptFileInfo(dbSyncerInfoContext, fi, rootFolder);
                using (Database db = new Database(connectionString))
                {
                    DbSyncerInfoCommand.DeleteScriptFile(db, scriptFileInfo);
                    WriteObject("Script removed:");
                    WriteObject(string.Format("{0}\\{1}", DbSyncerInfoContextID, FileName));
                    WriteObject("Now last script into [dbo].[@DbSyncerInfo]:");
                    var result = db.Database.SqlQuery<DbSyncerInfo>(FormattableStringFactory.Create("select top 1 * from [dbo].[@DbSyncerInfo] order by [ScriptDate] desc"));
                    WriteObject(result.FirstOrDefault());
                }
            }
            else
                WriteObject(string.Format(@"File {0} not exist!", fi.FullName));

        }
    }
}
