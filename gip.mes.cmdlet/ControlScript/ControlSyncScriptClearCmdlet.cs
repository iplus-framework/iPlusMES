using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.cmdlet.Settings;
using System.Collections.Generic;
using System.Management.Automation;

namespace gip.mes.cmdlet.ControlScript
{

    /// <summary>
    /// call ACClassCleanManager to delete unused ACClasses or ACClass members: Methods, Properties etc
    /// </summary>
    [Cmdlet(VerbsCommon.Clear, CmdLetSettings.ControlSyncScriptCmdlet_Name)]
    public class ControlSyncScriptClearCmdlet : Cmdlet
    {
        public string VarioData { get; set; } = VBPowerShellSettings.VarioDataDefault;

        #region Parameters

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsLogDisabled { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string SavePath { get; set; }


        #endregion

        protected override void ProcessRecord()
        {
            VBPowerShellSettings iPlusCmdLetSettings = FactorySettings.Factory(VarioData);
            if (string.IsNullOrEmpty(SavePath))
                SavePath = iPlusCmdLetSettings.VarioData;
            using (Database database = new Database())
            {
                Translator.DefaultVBLanguageCode = "en";
                ACClassCleanManager cleanManager = new ACClassCleanManager(database, iPlusCmdLetSettings.DLLBinFolder);
                cleanManager.CollectData();
                MsgWithDetails msg = cleanManager.RemoveAssembiles(true);
                List<RemoveClassReport> removeClassReports = cleanManager.RemoveClasses(true);
                if (!IsLogDisabled)
                {
                    ACCleanManagerJsonReport aCCleanManagerJsonReport = new ACCleanManagerJsonReport(SavePath);
                    aCCleanManagerJsonReport.WriteACClassCleanManagerJsonData(cleanManager);
                    aCCleanManagerJsonReport.WriteRemoveClassesReport(removeClassReports);
                }
            }
        }
    }
}
