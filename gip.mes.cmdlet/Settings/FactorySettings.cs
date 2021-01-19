using gip.core.datamodel;
using System.Configuration;
using System.IO;
using System.Text;

namespace gip.mes.cmdlet.Settings
{
    public static class FactorySettings
    {
        public static VBPowerShellSettings Factory(string rootFolder)
        {
            Translator.VBLanguageCode = "en";
            string path = Path.Combine(rootFolder, VBPowerShellSettings.SettingsFile);
            string content = File.ReadAllText(path);
            VBPowerShellSettings settings = Newtonsoft.Json.JsonConvert.DeserializeObject<VBPowerShellSettings>(content);
            settings.FullConnectionStringConfigurationPaht = LoadConnections(settings, rootFolder);
            WarmapConfig(settings);
            return settings;
        }

        private static string LoadConnections(VBPowerShellSettings settings, string rootFolder)
        {
            string connectionFileNameSource = settings.ConnectionStringFileName;
            string connectionFileNameTarget = Path.GetFileNameWithoutExtension(connectionFileNameSource) + @"Temp" + Path.GetExtension(connectionFileNameSource);

            string fullPathSource = Path.Combine(rootFolder, connectionFileNameSource);
            string fullPathTarget = Path.Combine(rootFolder, connectionFileNameTarget);

            string template = @"<configuration>{0}</configuration>";
            File.WriteAllText(fullPathTarget, string.Format(template, File.ReadAllText(fullPathSource)), Encoding.UTF8);

            return fullPathTarget;

        }

        public static void WarmapConfig(VBPowerShellSettings settings)
        {
            ExeConfigurationFileMap externConfigMap = new ExeConfigurationFileMap();
            externConfigMap.ExeConfigFilename = Path.Combine(settings.FullConnectionStringConfigurationPaht);

            var externalConfig = ConfigurationManager.OpenMappedExeConfiguration(externConfigMap, ConfigurationUserLevel.None);

            CommandLineHelper.ConfigCurrentDir = externalConfig;
        }
    }
}
