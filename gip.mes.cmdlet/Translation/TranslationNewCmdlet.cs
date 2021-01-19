using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.cmdlet.Settings;
using System;
using System.Data;
using System.Linq;
using System.Management.Automation;

namespace gip.mes.cmdlet.Translation
{
    [Cmdlet(VerbsCommon.New, "VariobatchTranslation")]
    public class TranslationNewCmdlet : Cmdlet
    {
        #region Configuration
        public string VarioData { get; set; } = VBPowerShellSettings.VarioDataDefault;

        #endregion

        #region Mandatory parameters

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string ProjectName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string ClassACIdentifier { get; set; }

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

        #endregion

        #region Process

        protected override void ProcessRecord()
        {
            VBPowerShellSettings designSettings = FactorySettings.Factory(VarioData);
            string identifier = ACIdentifier;
            string aCCaptionTranslation = "";
            using (Database database = new Database())
            {
                ACClass aCClass = database.ACClass.Where(c => c.ACProject.ACProjectName == ProjectName && c.ACIdentifier == ClassACIdentifier).FirstOrDefault();
                if (aCClass != null)
                {
                    aCCaptionTranslation = string.Format(@"en{{'{0}'}}de{{'{1}'}}", EnText, DeText);
                    DateTime nowTime = DateTime.Now;
                    if (MessageType == MessageTypeEnum.None)
                    {
                        ACClassText acClassText = ACClassText.NewACObject(database, aCClass);
                        acClassText.ACIdentifier = identifier;
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
