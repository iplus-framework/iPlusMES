namespace gip.mes.cmdlet.Settings
{
    public class VBPowerShellSettings
    {

        public static string VarioDataDefault = @"C:\VarioData";
        public static string SettingsFile = @"VBPowerShellSettings.json";

        public string VarioData { get; set; }

        public string RootFolder { get; set; }
        public string ConnectionStringFileName { get; set; }

        public string DLLBinFolder { get; set; }

        public string FullConnectionStringConfigurationPaht { get; set; }

        public string username { get; set; }
        public string password { get; set; }
    }
}
