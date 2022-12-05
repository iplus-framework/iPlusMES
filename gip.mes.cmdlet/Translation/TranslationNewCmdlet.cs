using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.cmdlet.Settings;
using System;
using System.Data;
using System.Linq;
using System.Management.Automation;

namespace gip.mes.cmdlet.Translation
{
    /// <summary>
    /// Add new translation item into ACClassText or ACClassMessage
    /// </summary>
    [Cmdlet(VerbsCommon.New, CmdLetSettings.TranslationCmdlet_Name)]
    public class TranslationNewCmdlet : Cmdlet
    {
        #region Configuration
        public string VarioData { get; set; } = VBPowerShellSettings.VarioDataDefault;

        #endregion

        #region Mandatory parameters

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string ProjectName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public MessageTypeEnum MessageType { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string EnText { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string DeText { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string UpdateName { get; set; }

        #endregion

        #region Optional Parameters

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string ACIdentifier { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string ClassACIdentifier { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string ACURLComponentCached { get; set; }


        #endregion

        #region Process

        protected override void ProcessRecord()
        {
            if (string.IsNullOrEmpty(ClassACIdentifier) && string.IsNullOrEmpty(ACURLComponentCached))
            {
                WriteObject(string.Format("Missing object {0} - {1}", ClassACIdentifier, ACURLComponentCached));
                return;
            }
            string identifier = "";
            VBPowerShellSettings iPlusCmdLetSettings = FactorySettings.Factory(VarioData);

            string aCCaptionTranslation = "";
            using (Database database = new Database())
            {
                ACClass aCClass =
                    database
                    .ACClass
                    .Where(c =>
                                c.ACProject.ACProjectName == ProjectName
                                && (
                                       (ClassACIdentifier != null && c.ACIdentifier == ClassACIdentifier)
                                       || (ACURLComponentCached != null && c.ACURLComponentCached == ACURLComponentCached)
                                    )
                            )
                   .FirstOrDefault();
                if (aCClass != null)
                {
                    aCCaptionTranslation = string.Format(@"en{{'{0}'}}de{{'{1}'}}", EnText, DeText);
                    DateTime nowTime = DateTime.Now;
                    if (MessageType == MessageTypeEnum.None)
                    {
                        ACClassText acClassText = ACClassText.NewACObject(database, aCClass);
                        acClassText.ACIdentifier = ACIdentifier;
                        acClassText.ACCaptionTranslation = aCCaptionTranslation;

                        acClassText.InsertDate = nowTime;
                        acClassText.UpdateDate = nowTime;
                        acClassText.InsertName = UpdateName;
                        acClassText.UpdateName = UpdateName;

                        database.ACClassText.AddObject(acClassText);
                    }
                    else
                    {
                        string prefix = MessageType.ToString();
                        identifier = ACClassMessage.GetUniqueNameIdentifier(database, prefix, false);
                        ACClassMessage acClassMessage = ACClassMessage.NewACObject(database, aCClass);
                        acClassMessage.ACIdentifier = identifier;
                        acClassMessage.ACCaptionTranslation = aCCaptionTranslation;

                        acClassMessage.InsertDate = nowTime;
                        acClassMessage.UpdateDate = nowTime;
                        acClassMessage.InsertName = UpdateName;
                        acClassMessage.UpdateName = UpdateName;

                        database.ACClassMessage.AddObject(acClassMessage);
                    }
                    MsgWithDetails saveMsg = database.ACSaveChanges();
                    if (saveMsg != null && !saveMsg.IsSucceded())
                        identifier = saveMsg.InnerMessage;
                }
                else
                    identifier = string.Format("Missing class {0} - {1}!", ProjectName, ClassACIdentifier);
            }
            WriteObject(string.Format(@"Generated: {0}.", identifier));
        }

        #endregion
    }
}
