using gip.mes.cmdlet.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.cmdlet.Config
{
    [Cmdlet(VerbsCommon.Set, CmdLetSettings.iPlusConfigCmdlet_Name)]

    public class iPlusConfigSetCmdlet : Cmdlet
    {
        protected override void ProcessRecord()
        {
            VBPowerShellSettings iPlusCmdLetSettings = FactorySettings.Factory(".\\");
        }
    }
}
