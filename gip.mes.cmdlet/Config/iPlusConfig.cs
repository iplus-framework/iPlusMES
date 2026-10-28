using gip.mes.cmdlet.Settings;
using System.Management.Automation;

namespace gip.mes.cmdlet.Config
{
    /// <summary>
    /// This cmdlet is used to load connection string into cmdlet
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
