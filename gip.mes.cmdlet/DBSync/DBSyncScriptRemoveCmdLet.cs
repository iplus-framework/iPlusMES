using gip.core.datamodel;
using gip.core.dbsyncer.Command;
using gip.core.dbsyncer.helper;
using gip.core.dbsyncer.model;
using gip.mes.cmdlet.Settings;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Management.Automation;

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
            DbSyncerInfoContext dbSyncerInfoContext = new DbSyncerInfoContext() { DbSyncerInfoContextID = DbSyncerInfoContextID };
            FileInfo fi = new FileInfo(Path.Combine(rootFolder, FileName));
            ScriptFileInfo scriptFileInfo = new ScriptFileInfo(dbSyncerInfoContext, fi, rootFolder);
            using (DbContext db = new DbContext(connectionString))
            {
                DbSyncerInfoCommand.DeleteScriptFile(db, scriptFileInfo);
                WriteObject("Script removed:");
                WriteObject(string.Format("{0}\\{1}", DbSyncerInfoContextID, FileName));
                WriteObject("Now last script:");
                var result = db.Database.SqlQuery<DbSyncerInfo>("select top 1 * from [dbo].[@DbSyncerInfo] order by [ScriptDate] desc");
                WriteObject(result.FirstOrDefault());
            }
        }
    }
}
