using gip.core.datamodel;
using gip.mes.cmdlet.Settings;
using System;
using System.Linq;
using System.Management.Automation;

namespace gip.mes.cmdlet.Translation
{
    [Cmdlet(VerbsCommon.Set, CmdLetSettings.TranslationCmdlet_Name)]

    public class TranslationSetCmdlet : Cmdlet
    {

        #region Configuration
        public string VarioData { get; set; } = VBPowerShellSettings.VarioDataDefault;

        #endregion

        #region Parameters

        #region Parameters -> Mandatory
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public ItemTypeEnum ItemType { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string ACIdentifer { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string EnText { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string DeText { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string UpdateName { get; set; }

        #endregion

        #region Parameters -> Optional

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public Guid? ID { get; set; }

        #endregion

        #endregion

        protected override void ProcessRecord()
        {
            VBPowerShellSettings designSettings = FactorySettings.Factory(VarioData);
            bool isTranslationEntered =
                !string.IsNullOrEmpty(ACIdentifer) &&
                !string.IsNullOrEmpty(EnText) &&
                !string.IsNullOrEmpty(DeText);

            if (isTranslationEntered)
            {
                string translation = string.Format(@"en{{'{0}'}}de{{'{1}'}}", EnText, DeText);
                using (Database database = new Database())
                {
                    string updateMessage = string.Format("{0} {1} updated: {2}", ItemType, ACIdentifer, translation);
                    switch (ItemType)
                    {
                        case ItemTypeEnum.ACClass:
                            ACClass aCClass = 
                                database.ACClass.FirstOrDefault(c => (ID != null && c.ACClassID == (ID ?? Guid.Empty)) ||
                            c.ACIdentifier == ACIdentifer);
                            if(aCClass != null)
                            {
                                aCClass.ACCaptionTranslation = translation;
                                aCClass.UpdateName = !string.IsNullOrEmpty(UpdateName) ? UpdateName : "SUP";
                                aCClass.UpdateDate = DateTime.Now;
                                WriteObject(updateMessage);
                            }
                            break;
                        case ItemTypeEnum.ACClassProperty:
                            ACClassProperty property =
                               database.ACClassProperty.FirstOrDefault(c => (ID != null && c.ACClassID == (ID ?? Guid.Empty)) ||
                           c.ACIdentifier == ACIdentifer);
                            if (property != null)
                            {
                                property.ACCaptionTranslation = translation;
                                property.UpdateName = !string.IsNullOrEmpty(UpdateName) ? UpdateName : "SUP";
                                property.UpdateDate = DateTime.Now;
                                WriteObject(updateMessage);
                            }
                            break;
                        case ItemTypeEnum.ACClassText:
                            ACClassText text =
                               database.ACClassText.FirstOrDefault(c => (ID != null && c.ACClassID == (ID ?? Guid.Empty)) ||
                           c.ACIdentifier == ACIdentifer);
                            if (text != null)
                            {
                                text.ACCaptionTranslation = translation;
                                text.UpdateName = !string.IsNullOrEmpty(UpdateName) ? UpdateName : "SUP";
                                text.UpdateDate = DateTime.Now;
                                WriteObject(updateMessage);
                            }
                            break;
                        case ItemTypeEnum.ACClassDesign:
                            ACClassDesign design =
                               database.ACClassDesign.FirstOrDefault(c => (ID != null && c.ACClassID == (ID ?? Guid.Empty)) ||
                           c.ACIdentifier == ACIdentifer);
                            if (design != null)
                            {
                                design.ACCaptionTranslation = translation;
                                design.UpdateName = !string.IsNullOrEmpty(UpdateName) ? UpdateName : "SUP";
                                design.UpdateDate = DateTime.Now;
                                WriteObject(updateMessage);
                            }
                            break;
                        case ItemTypeEnum.ACClassMessage:
                            ACClassMessage message =
                               database.ACClassMessage.FirstOrDefault(c => (ID != null && c.ACClassID == (ID ?? Guid.Empty)) ||
                           c.ACIdentifier == ACIdentifer);
                            if (message != null)
                            {
                                message.ACCaptionTranslation = translation;
                                message.UpdateName = !string.IsNullOrEmpty(UpdateName) ? UpdateName : "SUP";
                                message.UpdateDate = DateTime.Now;
                                WriteObject(updateMessage);
                            }
                            break;
                        default:
                            break;
                    }
                    database.ACSaveChanges();
                }
            }
        }
    }
}
