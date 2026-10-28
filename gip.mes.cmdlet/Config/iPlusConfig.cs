using gip.mes.cmdlet.Settings;
using System.Management.Automation;

namespace gip.mes.cmdlet.Config
{
    /// <summary>
    /// Loads the connection string settings into the cmdlet session.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, nameof(iPlusConfig))]

    public class iPlusConfig : Cmdlet
    {
        protected override void ProcessRecord()
        {
            SessionState ss = new SessionState();
            VBPowerShellSettings iPlusCmdLetSettings = FactorySettings.Factory(ss.Path.CurrentFileSystemLocation.Path);
            WriteObject(iPlusCmdLetSettings);
        }
    }
}
