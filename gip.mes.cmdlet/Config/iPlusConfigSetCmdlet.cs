using gip.mes.cmdlet.Settings;
using System.Management.Automation;

namespace gip.mes.cmdlet.Config
{
    [Cmdlet(VerbsCommon.Set, CmdLetSettings.iPlusConfigCmdlet_Name)]

    public class iPlusConfigSetCmdlet : Cmdlet
    {
        protected override void ProcessRecord()
        {
            SessionState ss = new SessionState();
            VBPowerShellSettings iPlusCmdLetSettings = FactorySettings.Factory(ss.Path.CurrentFileSystemLocation.Path);
            WriteObject(iPlusCmdLetSettings);
        }
    }
}
