using gip.core.autocomponent;
using gip.core.ControlScriptSync;
using gip.core.datamodel;
using gip.core.dbsyncer.helper;
using gip.mes.cmdlet.DesignSync.Settings;
using gip.mes.cmdlet.Settings;
using System.Management.Automation;

namespace gip.mes.cmdlet.ControlScript
{
    /// <summary>
    /// Imports new .zip designs and resources into the database:
    /// runs the same import procedure for the .zip file as CTRL load.
    /// </summary>
    [Cmdlet("Import", IPlusResourceSettings.iPlusResource)]
    public class iPlusResourceImportCmdlet : Cmdlet
    {
        #region Config
        public string VarioData { get; set; } = VBPowerShellSettings.VarioDataDefault;

        
        #endregion

        protected override void ProcessRecord()
        {
            VBPowerShellSettings iPlusCmdLetSettings = FactorySettings.Factory(VarioData);
            string connectionString = DbSyncerSettings.GetDefaultConnectionString(CommandLineHelper.ConfigCurrentDir);
            if (gip.core.datamodel.Database.Root == null)
                ACRootFactory.Factory(iPlusCmdLetSettings.username, iPlusCmdLetSettings.password);
            ControlSync controlSync = new ControlSync();
            ACRoot.SRoot.PrepareQueriesAndResoruces();
            controlSync.OnMessage += controlSync_OnMessage;
            bool importSuccess = false;
            IResources rootResources = new Resources();
            using (ACMonitor.Lock(ACRoot.SRoot.Database.QueryLock_1X000))
            {
                importSuccess = controlSync.Sync(ACRoot.SRoot, Database.GlobalDatabase, iPlusCmdLetSettings.DLLBinFolder, connectionString);
            }
        }

        void controlSync_OnMessage(SyncMessage msg)
        {
            WriteObject(msg.Message);
        }

        public void SendMessage(Msg msg)
        {
            WriteObject(msg.Message);
        }
    }
}
