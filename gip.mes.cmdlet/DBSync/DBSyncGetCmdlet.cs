using gip.core.datamodel;
using gip.core.dbsyncer;
using gip.core.dbsyncer.helper;
using gip.core.dbsyncer.Messages;
using gip.mes.cmdlet.Settings;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace gip.mes.cmdlet.DBSync
{
    [Cmdlet(VerbsCommon.Get, CmdLetSettings.DBSyncCmdlet_Name)]
    public class DBSyncGetCmdlet : Cmdlet
    {

        #region Config
        public string VarioData { get; set; } = VBPowerShellSettings.VarioDataDefault;

        #endregion

        protected override void ProcessRecord()
        {
            VBPowerShellSettings iPlusCmdLetSettings = FactorySettings.Factory(VarioData);
            string connectionString = DbSyncerSettings.GetDefaultConnectionString(CommandLineHelper.ConfigCurrentDir);
            string rootFolder = Path.Combine(iPlusCmdLetSettings.DLLBinFolder);

            SyncTask syncTask = new SyncTask();
            syncTask.OnStatusChange += syncTask_OnStatusChange;
            List<BaseSyncMessage> syncMsgList = syncTask.DoSync(connectionString, rootFolder);
            if (syncMsgList != null && syncMsgList.Where(x => !x.Success).Any())
            {
                string errorDBSyncMessage = "There is some errors by executing DBSync!";
                errorDBSyncMessage += System.Environment.NewLine;
                errorDBSyncMessage += System.Environment.NewLine;
                foreach (var a in syncMsgList)
                {
                    errorDBSyncMessage += a.Message;
                    errorDBSyncMessage += System.Environment.NewLine;
                }
                errorDBSyncMessage += System.Environment.NewLine;
                WriteObject(errorDBSyncMessage);
            }

        }

        private void syncTask_OnStatusChange(core.dbsyncer.Messages.BaseSyncMessage msg)
        {
            WriteObject(msg.Message);
        }
    }
}
