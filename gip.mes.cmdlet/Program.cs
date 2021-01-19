using gip.bso.iplus;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.cmdlet.DesignSync;
using gip.mes.cmdlet.Settings;
using System;
using System.IO;
using System.Linq;

namespace gip.mes.cmdlet
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.Write("Make complete export");
            VBPowerShellSettings designSettings = FactorySettings.Factory(VBPowerShellSettings.VarioDataDefault);
            string rootFolder = Path.Combine(designSettings.VarioData, "TestExport");
            if (!Directory.Exists(rootFolder))
                Directory.CreateDirectory(rootFolder);
            ACRootFactory.Factory();
            ExportCommandExtended exportCommand = new ExportCommandExtended();
            exportCommand.IsExportACClassDesign = true;
            exportCommand.IsExportACClassMessage = true;
            exportCommand.IsExportACClassText = true;
            exportCommand.IsExportACClass = true;

            using (Database database = new Database())
            {
                ACProjectManager aCProjectManager = new ACProjectManager(database, ACRoot.SRoot);
                ACClassExporter exporter = new ACClassExporter(rootFolder, aCProjectManager, database, exportCommand);
                ACProject variobatchProject = database.ACProject.FirstOrDefault(c => c.ACProjectName == Const.ACRootClassName);
                exporter.Export(variobatchProject, null);
            }
        }
    }
}
