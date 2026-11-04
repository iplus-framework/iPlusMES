using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.cmdlet.DesignSync.Settings;
using gip.mes.cmdlet.Settings;
using System.Collections.Generic;
using System.Management.Automation;

namespace gip.mes.cmdlet.ControlScript
{

    /// <summary>
    /// Calls ACClassCleanManager to delete unused ACClasses and ACClass members (methods, properties, etc.).
    /// </summary>
    [Cmdlet(VerbsCommon.Clear, IPlusResourceSettings.iPlusResource)]
    public class iPlusResourceCleanUpCmdlet : Cmdlet
    {
        public string VarioData { get; set; } = VBPowerShellSettings.VarioDataDefault;

        #region Parameters
        /// <summary>
        /// Disables clean-up logging.
        /// </summary>

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsLogDisabled { get; set; }

        /// <summary>
        /// Path for saving the clean-up log.
        /// </summary>

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
