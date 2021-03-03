using gip.core.autocomponent;
using gip.core.ControlScriptSync;
using gip.core.datamodel;
using gip.core.dbsyncer.helper;
using gip.mes.cmdlet.Settings;
using System.Management.Automation;

namespace gip.mes.cmdlet.ControlScript
{
    [Cmdlet(VerbsCommon.Set, CmdLetSettings.ControlSyncScriptCmdlet_Name)]
    public class ControlSyncScriptSetCmdlet : Cmdlet, IMsgObserver
    {
        #region Config
        public string VarioData { get; set; } = VBPowerShellSettings.VarioDataDefault;

        
        #endregion

        protected override void ProcessRecord()
        {
            VBPowerShellSettings designSettings = FactorySettings.Factory(VarioData);
            string connectionString = DbSyncerSettings.GetDefaultConnectionString(CommandLineHelper.ConfigCurrentDir);
            if (gip.core.datamodel.Database.Root == null)
                ACRootFactory.Factory(designSettings.username, designSettings.password);
            ControlSync controlSync = new ControlSync();
            ACRoot.SRoot.PrepareQueriesAndResoruces();
            controlSync.OnMessage += controlSync_OnMessage;
            bool importSuccess = false;
            IResources rootResources = new Resources();
            rootResources.MsgObserver = this;
            using (ACMonitor.Lock(ACRoot.SRoot.Database.QueryLock_1X000))
            {
                importSuccess = controlSync.Sync(ACRoot.SRoot, Database.GlobalDatabase, designSettings.TrunkFolder, connectionString);
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
