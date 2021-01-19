using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.cmdlet.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace gip.mes.cmdlet.ControlScript
{
    [Cmdlet(VerbsCommon.Clear, "ControlSyncScript")]
    public class ControlSyncScriptClearCmdlet : Cmdlet
    {
        public string VarioData { get; set; } = VBPowerShellSettings.VarioDataDefault;

        #region Parameters

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public bool IsLogDisabled { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string RootFolder { get; set; }


        private string ReportPath { get; set; }
        #endregion

        protected override void ProcessRecord()
        {
            VBPowerShellSettings designSettings = FactorySettings.Factory(VarioData);
            if (string.IsNullOrEmpty(RootFolder))
                RootFolder = designSettings.VarioData;
            using (Database database = new Database())
            {
                Translator.DefaultVBLanguageCode = "en";
                ACClassCleanManager cleanManager = new ACClassCleanManager(database, designSettings.DLLBin);
                cleanManager.CollectData();
                MsgWithDetails msg = cleanManager.RemoveAssembiles(true);
                List<RemoveClassReport> removeClassReports = cleanManager.RemoveClasses(true);
                if (!IsLogDisabled)
                {
                    RootFolder = designSettings.VarioData;
                    ACCleanManagerJsonReport aCCleanManagerJsonReport = new ACCleanManagerJsonReport(RootFolder);
                    aCCleanManagerJsonReport.WriteACClassCleanManagerJsonData(cleanManager);
                    aCCleanManagerJsonReport.WriteRemoveClassesReport(removeClassReports);
                }
            }
        }
    }
}
