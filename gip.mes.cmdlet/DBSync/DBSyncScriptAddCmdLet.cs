using gip.core.datamodel;
using gip.core.dbsyncer.Command;
using gip.core.dbsyncer.helper;
using gip.core.dbsyncer.model;
using gip.mes.cmdlet.Settings;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Runtime.CompilerServices;

namespace gip.mes.cmdlet.DBSync
{

    /// <summary>
    /// Executes only one DBSync file,
    /// the same as CTRL load for a single SQL file.
    /// </summary>
    [Cmdlet(VerbsCommon.Add, DbSyncConst.DBSync)]
    public class DBSyncScriptAddCmdlet : Cmdlet
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
            ScriptFileInfo scriptFileInfo = new ScriptFileInfo(dbSyncerInfoContext, fi, rootFolder);
            using (Database db = new Database(connectionString))
            {
                DbSyncerInfoCommand.Update(db, scriptFileInfo);
                WriteObject("Script added:");
                WriteObject(string.Format("{0}\\{1}", DbSyncerInfoContextID, FileName));
                WriteObject("Now last script:");
                var result = db.Database.SqlQuery<gip.mes.cmdlet.DBSync.Model.DbSyncerInfo>(FormattableStringFactory.Create("select top 1 * from [dbo].[@DbSyncerInfo] order by [ScriptDate] desc"));
                WriteObject(result.FirstOrDefault());
            }
        }
    }
}
