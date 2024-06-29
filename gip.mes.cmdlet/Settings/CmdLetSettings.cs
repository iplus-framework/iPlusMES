namespace gip.mes.cmdlet.Settings
{
    public class CmdLetSettings
    {
        /// <summary>
        /// Query or set translations: ACClassText or ACClassMessage
        /// </summary>
        public const string TranslationCmdlet_Name = @"iPlusTranslation";

        /// <summary>
        /// Used to 
        /// 1. Export designs to external folder (XAMLEditing)
        /// 2. Import there changed designs (translations to) into database
        /// </summary>
        public const string iPlusResourceCmdlet_Name = @"iPlusResource";

        /// <summary>
        /// run not executed db scripts on project database
        /// </summary>
        public const string DBSyncCmdlet_Name = @"iPlusDBSync";

        /// <summary>
        /// generate design (and resources) zip file
        /// </summary>
        public const string ControlSyncScriptCmdlet_Name = @"ControlSyncScript";

        /// <summary>
        /// in current directory place default iplus powershell settings file
        /// </summary>
        public const string iPlusConfigCmdlet_Name = @"iPlusConfig";
    }
}
