using gip.core.datamodel;
using gip.mes.cmdlet.Settings;
using gip.mes.cmdlet.Translation.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace gip.mes.cmdlet.Translation.Service
{
    public class TranslationService
    {
        #region const
        public string[] begins = new string[] { @"\sError(\d\d\d\d\d)", @"\sInfo(\d\d\d\d\d)", @"\sWarning(\d\d\d\d\d)", @"\sQuestion(\d\d\d\d\d)", @"\sException(\d\d\d\d\d)" };
        #endregion

        #region DI
        public VBPowerShellSettings IPlusCmdLetSettings { get; private set; }
        #endregion

        #region ctor's
        public TranslationService(VBPowerShellSettings iPlusCmdLetSettings)
        {
            IPlusCmdLetSettings = iPlusCmdLetSettings;
        }
        #endregion

        #region Methods

        public List<TranslationPresentation> GetTranslationPresentation(string acIdentifier, string classACIdentifer, string searchText, ItemTypeEnum[] itemTypes)
        {
            List<TranslationPresentation> presentations = new List<TranslationPresentation>();

            bool searchParamsEmpty = string.IsNullOrEmpty(classACIdentifer) && string.IsNullOrEmpty(acIdentifier) && string.IsNullOrEmpty(searchText);

            if (!searchParamsEmpty)
            {
                using (Database database = new Database())
                {

                    // ACClass
                    ItemTypeEnum itemType = ItemTypeEnum.ACClass;
                    if (itemTypes == null || itemTypes.Contains(itemType))
                    {
                        List<ACClass> classes =
                       database
                       .ACClass
                       .Where(c =>
                           (classACIdentifer == null || c.ACIdentifier.Contains(classACIdentifer))
                           &&
                           (acIdentifier == null || c.ACIdentifier.Contains(acIdentifier))
                           &&
                           (searchText == null || c.ACCaptionTranslation.Contains(searchText))
                        )
                       .ToList();
                        presentations.AddRange(classes.Select(c => new TranslationPresentation()
                        {
                            ItemType = itemType,
                            ClassACIdentifier = c.ACIdentifier,
                            ACIdentifier = c.ACIdentifier,
                            ACCaptionTranslation = c.ACCaptionTranslation,
                            UpdateDate = c.UpdateDate,
                            UpdateName = c.UpdateName
                        }));
                    }

                    // ACClassDesign
                    itemType = ItemTypeEnum.ACClassDesign;
                    if (itemTypes == null || itemTypes.Contains(itemType))
                    {
                        List<ACClassDesign> designs =
                       database
                       .ACClassDesign
                       .Where(c =>
                           (classACIdentifer == null || c.ACClass.ACIdentifier.Contains(classACIdentifer))
                           &&
                           (acIdentifier == null || c.ACIdentifier.Contains(acIdentifier))
                           &&
                           (searchText == null || c.ACCaptionTranslation.Contains(searchText))
                        )
                       .ToList();
                        presentations.AddRange(designs.Select(c => new TranslationPresentation()
                        {
                            ItemType = itemType,
                            ClassACIdentifier = c.ACClass.ACIdentifier,
                            ACIdentifier = c.ACIdentifier,
                            ACCaptionTranslation = c.ACCaptionTranslation,
                            UpdateDate = c.UpdateDate,
                            UpdateName = c.UpdateName
                        }));
                    }

                    // ACClassMessage
                    itemType = ItemTypeEnum.ACClassMessage;
                    if (itemTypes == null || itemTypes.Contains(itemType))
                    {
                        List<ACClassMessage> messages =
                       database
                       .ACClassMessage
                       .Where(c =>
                           (classACIdentifer == null || c.ACClass.ACIdentifier.Contains(classACIdentifer))
                           &&
                           (acIdentifier == null || c.ACIdentifier.Contains(acIdentifier))
                           &&
                           (searchText == null || c.ACCaptionTranslation.Contains(searchText))
                        )
                       .ToList();
                        presentations.AddRange(messages.Select(c => new TranslationPresentation()
                        {
                            ItemType = itemType,
                            ClassACIdentifier = c.ACClass.ACIdentifier,
                            ACIdentifier = c.ACIdentifier,
                            ACCaptionTranslation = c.ACCaptionTranslation,
                            UpdateDate = c.UpdateDate,
                            UpdateName = c.UpdateName
                        }));
                    }

                    // ACClassProperty
                    itemType = ItemTypeEnum.ACClassProperty;
                    if (itemTypes == null || itemTypes.Contains(itemType))
                    {
                        List<ACClassProperty> properties =
                        database
                        .ACClassProperty
                        .Where(c =>
                             (classACIdentifer == null || c.ACClass.ACIdentifier.Contains(classACIdentifer))
                            &&
                            (acIdentifier == null || c.ACIdentifier.Contains(acIdentifier))
                            &&
                            (searchText == null || c.ACCaptionTranslation.Contains(searchText))
                         )
                        .ToList();
                        presentations.AddRange(properties.Select(c => new TranslationPresentation()
                        {
                            ItemType = itemType,
                            ClassACIdentifier = c.ACClass.ACIdentifier,
                            ACIdentifier = c.ACIdentifier,
                            ACCaptionTranslation = c.ACCaptionTranslation,
                            UpdateDate = c.UpdateDate,
                            UpdateName = c.UpdateName
                        }));
                    }

                    // ACClassText
                    itemType = ItemTypeEnum.ACClassText;
                    if (itemTypes == null || itemTypes.Contains(itemType))
                    {
                        List<ACClassText> texts =
                        database
                        .ACClassText
                        .Where(c =>
                            (classACIdentifer == null || c.ACClass.ACIdentifier.Contains(classACIdentifer))
                            &&
                            (acIdentifier == null || c.ACIdentifier.Contains(acIdentifier))
                            &&
                            (searchText == null || c.ACCaptionTranslation.Contains(searchText))
                         )
                        .ToList();
                        presentations.AddRange(texts.Select(c => new TranslationPresentation()
                        {
                            ItemType = itemType,
                            ClassACIdentifier = c.ACClass.ACIdentifier,
                            ACIdentifier = c.ACIdentifier,
                            ACCaptionTranslation = c.ACCaptionTranslation,
                            UpdateDate = c.UpdateDate,
                            UpdateName = c.UpdateName
                        }));
                    }
                }
            }

            return presentations;
        }

        public (bool success, string identifier, string message) NewTranslation(string projectName, string classACIdentifier, string acIdentifier, string acURLComponentCached, MessageTypeEnum messageType, string enText, string deText, string updateName)
        {
            string aCCaptionTranslation = "";
            string identifier = "-";
            string message = "";
            bool success = false;
            using (Database database = new Database())
            {
                ACClass aCClass =
                    database
                    .ACClass
                    .Where(c =>
                                c.ACProject.ACProjectName == projectName
                                && (
                                       (classACIdentifier != null && c.ACIdentifier == classACIdentifier)
                                       || (acURLComponentCached != null && c.ACURLComponentCached == acURLComponentCached)
                                    )
                            )
                   .FirstOrDefault();
                if (aCClass != null)
                {
                    aCCaptionTranslation = string.Format(@"en{{'{0}'}}de{{'{1}'}}", enText, deText);
                    DateTime nowTime = DateTime.Now;
                    if (messageType == MessageTypeEnum.None)
                    {
                        ACClassText acClassText = ACClassText.NewACObject(database, aCClass);
                        acClassText.ACIdentifier = acIdentifier;
                        identifier = acIdentifier;
                        acClassText.ACCaptionTranslation = aCCaptionTranslation;
                        acClassText.InsertDate = nowTime;
                        acClassText.UpdateDate = nowTime;
                        acClassText.InsertName = updateName;
                        acClassText.UpdateName = updateName;
                        database.ACClassText.Add(acClassText);
                    }
                    else
                    {
                        string prefix = messageType.ToString();
                        identifier = ACClassMessage.GetUniqueNameIdentifier(database, prefix, false);
                        ACClassMessage acClassMessage = ACClassMessage.NewACObject(database, aCClass);
                        acClassMessage.ACIdentifier = identifier;
                        acClassMessage.ACCaptionTranslation = aCCaptionTranslation;
                        acClassMessage.InsertDate = nowTime;
                        acClassMessage.UpdateDate = nowTime;
                        acClassMessage.InsertName = updateName;
                        acClassMessage.UpdateName = updateName;

                        database.ACClassMessage.Add(acClassMessage);
                    }
                    MsgWithDetails saveMsg = database.ACSaveChanges();
                    if (saveMsg != null && !saveMsg.IsSucceded())
                    {
                        message = saveMsg.DetailsAsText;
                    }
                    else
                    {
                        success = true;
                        message = string.Format("New translation created: {0} - {1}!", projectName, identifier);
                    }
                }
                else
                {
                    message = string.Format("Missing class {0} - {1}!", projectName, classACIdentifier);
                }
            }

            return (success, identifier, message);
        }

        public (bool success, string message) SetTranslation(string acIdentifier, ItemTypeEnum itemType, string enText, string deText, string updateName, Guid? ID = null)
        {
            string translation = string.Format(@"en{{'{0}'}}de{{'{1}'}}", enText, deText);
            bool success = false;
            string message = "";
            using (Database database = new Database())
            {
                string successfullyUpdateMsgStr = string.Format("{0} {1} updated: {2}", itemType, acIdentifier, translation);
                string notSuccessfullyUpdateMsgStr = string.Format("{0} {1} not updated!", itemType, acIdentifier, translation);
                bool updateSuccess = false;
                switch (itemType)
                {
                    case ItemTypeEnum.ACClass:
                        ACClass aCClass = database.ACClass.FirstOrDefault(c => (ID != null && c.ACClassID == (ID ?? Guid.Empty)) || c.ACIdentifier == acIdentifier);
                        if (aCClass != null)
                        {
                            aCClass.ACCaptionTranslation = translation;
                            aCClass.UpdateName = !string.IsNullOrEmpty(updateName) ? updateName : "SUP";
                            aCClass.UpdateDate = DateTime.Now;
                            updateSuccess = true;
                        }
                        break;
                    case ItemTypeEnum.ACClassProperty:
                        ACClassProperty property = database.ACClassProperty.FirstOrDefault(c => (ID != null && c.ACClassID == (ID ?? Guid.Empty)) || c.ACIdentifier == acIdentifier);
                        if (property != null)
                        {
                            property.ACCaptionTranslation = translation;
                            property.UpdateName = !string.IsNullOrEmpty(updateName) ? updateName : "SUP";
                            property.UpdateDate = DateTime.Now;
                            updateSuccess = true;
                        }
                        break;
                    case ItemTypeEnum.ACClassText:
                        ACClassText text = database.ACClassText.FirstOrDefault(c => (ID != null && c.ACClassID == (ID ?? Guid.Empty)) || c.ACIdentifier == acIdentifier);
                        if (text != null)
                        {
                            text.ACCaptionTranslation = translation;
                            text.UpdateName = !string.IsNullOrEmpty(updateName) ? updateName : "SUP";
                            text.UpdateDate = DateTime.Now;
                            updateSuccess = true;
                        }
                        break;
                    case ItemTypeEnum.ACClassDesign:
                        ACClassDesign design = database.ACClassDesign.FirstOrDefault(c => (ID != null && c.ACClassID == (ID ?? Guid.Empty)) || c.ACIdentifier == acIdentifier);
                        if (design != null)
                        {
                            design.ACCaptionTranslation = translation;
                            design.UpdateName = !string.IsNullOrEmpty(updateName) ? updateName : "SUP";
                            design.UpdateDate = DateTime.Now;
                            updateSuccess = true;
                        }
                        break;
                    case ItemTypeEnum.ACClassMessage:
                        ACClassMessage aCClassMessage = database.ACClassMessage.FirstOrDefault(c => (ID != null && c.ACClassID == (ID ?? Guid.Empty)) || c.ACIdentifier == acIdentifier);
                        if (aCClassMessage != null)
                        {
                            aCClassMessage.ACCaptionTranslation = translation;
                            aCClassMessage.UpdateName = !string.IsNullOrEmpty(updateName) ? updateName : "SUP";
                            aCClassMessage.UpdateDate = DateTime.Now;
                            updateSuccess = true;
                        }
                        break;
                    default:
                        break;
                }
                if (updateSuccess)
                {
                    MsgWithDetails saveMsg = database.ACSaveChanges();
                    if (saveMsg != null && !saveMsg.IsSucceded())
                    {
                        message = saveMsg.DetailsAsText;
                    }
                    else
                    {
                        success = true;
                        message = successfullyUpdateMsgStr;
                    }
                }
                else
                {
                    message = notSuccessfullyUpdateMsgStr;
                }
            }

            return (success, message);
        }

        public MsgWithDetails UpdateTranslationsFromFile(Database database, ACProject project, string fileName, string updateName)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();

            List<SoruceFileTranslationPair> translationPairs = GetTranslationPairs(fileName);

            List<FileTranslationDefinition> definitions = new List<FileTranslationDefinition>();
            foreach (var translationPair in translationPairs)
            {
                (Msg msg, FileTranslationDefinition definition) = GetTranslationDefinition(translationPair);
                if (msg != null)
                {
                    msgWithDetails.AddDetailMessage(msg);
                }
                else if (definition != null)
                {
                    definitions.Add(definition);
                }
            }

            foreach (FileTranslationDefinition definition in definitions)
            {
                Msg updateMsg = UpdateDefinition(database, definition, project, updateName);
                if (updateMsg != null)
                {
                    msgWithDetails.AddDetailMessage(updateMsg);
                }
            }

            return msgWithDetails;
        }

        public bool InsertTranslationIntoFile(string acIdentifier, string classACIdentifier, string fileName, int lineNr, eMsgLevel msgLevel, string enText, string deText)
        {
            bool success = false;
            if (File.Exists(fileName))
            {
                List<string> allLines = File.ReadAllLines(fileName).ToList();
                if (lineNr > 0 && lineNr <= allLines.Count)
                {
                    string addLine1 = $"// {acIdentifier}";
                    string addLine2 = $"// {classACIdentifier}";
                    string addLine3 = $"// {enText}";
                    string addLine4 = $"// {deText}";
                    string addLine5 = $"Msg msg = new Msg(this, eMsgLevel.{msgLevel.ToString()}, nameof({classACIdentifier}), nameof(methodName), {lineNr}, \"{acIdentifier}\");";
                    allLines.Insert(lineNr - 1, addLine1);
                    allLines.Insert(lineNr, addLine2);
                    allLines.Insert(lineNr + 1, addLine3);
                    allLines.Insert(lineNr + 2, addLine4);
                    allLines.Insert(lineNr + 3, addLine5);
                    File.WriteAllLines(fileName, allLines);
                    success = true;
                }
            }
            return success;
        }

        #endregion

        #region Private methods

        private List<SoruceFileTranslationPair> GetTranslationPairs(string fileName)
        {
            List<SoruceFileTranslationPair> translationPairs = new List<SoruceFileTranslationPair>();
            List<string> tempList = null;
            int? inLines = null;
            List<string> allLines = File.ReadAllLines(fileName).ToList();
            foreach (string line in allLines)
            {
                int lineNr = allLines.IndexOf(line) + 1;
                if (LineMatch(line))
                {
                    inLines = 0;
                    tempList = new List<string>();
                }

                if (inLines != null)
                {
                    inLines++;
                    tempList.Add(line);

                    if (inLines == 4)
                    {
                        SoruceFileTranslationPair pair = new SoruceFileTranslationPair();
                        pair.LineNr = lineNr - 4;
                        pair.Data = tempList;
                        translationPairs.Add(pair);
                        inLines = null;
                        tempList = null;
                    }
                }


            }
            return translationPairs;
        }

        private bool LineMatch(string line)
        {
            bool lineMatch = false;
            foreach (string begin in begins)
            {
                lineMatch = lineMatch || Regex.IsMatch(line, begin);
            }
            return lineMatch;
        }

        private Msg UpdateDefinition(Database database, FileTranslationDefinition definition, ACProject project, string updateName)
        {
            Msg msg = null;
            ACClass aCClass = project.ACClass_ACProject.Where(c => c.ACIdentifier == definition.Owner).FirstOrDefault();
            if (aCClass == null)
            {
                msg = new Msg()
                {
                    MessageLevel = eMsgLevel.Error,
                    Message = $"Missing class :{definition.Owner}!"
                };
            }
            else
            {
                ACClassMessage message = aCClass.ACClassMessage_ACClass.Where(c => c.ACIdentifier == definition.ID).FirstOrDefault();
                if (message == null)
                {
                    msg = new Msg()
                    {
                        MessageLevel = eMsgLevel.Error,
                        Message = $"Missing translation :{definition.Owner} | {definition.ID}!"
                    };
                }
                else
                {
                    string translation = $"en{{'{definition.EnTranslation}'}}de{{'{definition.DeTranslation}'}}";
                    if (message.ACCaptionTranslation != translation)
                    {
                        message.ACCaptionTranslation = translation;
                        message.UpdateName = updateName;
                        message.UpdateDate = DateTime.Now;
                        MsgWithDetails saveMsg = database.ACSaveChanges();
                        if (saveMsg != null && saveMsg.IsSucceded())
                        {
                            msg = new Msg()
                            {
                                MessageLevel = eMsgLevel.Error,
                                Message = $"Error saving translation :{definition.Owner} | {definition.ID}! Error: {saveMsg.DetailsAsText}"
                            };
                            database.ACUndoChanges();
                        }
                        else
                        {
                            msg = new Msg()
                            {
                                MessageLevel = eMsgLevel.Info,
                                Message = $"Saved translation :{definition.Owner} | {definition.ID}! \n Translation: {message.ACCaptionTranslation}"
                            };
                        }
                    }
                    else
                    {
                        msg = new Msg()
                        {
                            MessageLevel = eMsgLevel.Info,
                            Message = $"Translation {definition.Owner} | {definition.ID} not changed!"
                        };
                    }
                }
            }
            return msg;
        }

        private (Msg msg, FileTranslationDefinition definition) GetTranslationDefinition(SoruceFileTranslationPair source)
        {
            Msg msg = null;
            FileTranslationDefinition definition = null;
            if (source.Data.Count != 4)
            {
                msg = new Msg()
                {
                    MessageLevel = eMsgLevel.Error,
                    Message = $"Bad linies count! Line Nr:{source.LineNr}"
                };
            }
            else
            {
                string ID = ParseID(source.Data[0]);
                if (ID == null)
                {
                    msg = new Msg()
                    {
                        MessageLevel = eMsgLevel.Error,
                        Message = $"Unable to parse ID! Line Nr:{source.LineNr}, Value: {source.Data[0]}"
                    };
                }
                else
                {
                    string className = GetLineContent(source.Data[1]);
                    if (className == null)
                    {
                        msg = new Msg()
                        {
                            MessageLevel = eMsgLevel.Error,
                            Message = $"Unable to parse class name! Line Nr:{source.LineNr} Value: {source.Data[1]}"
                        };
                    }
                    else
                    {
                        string enTranslation = GetLineContent(source.Data[2]);
                        if (enTranslation == null)
                        {
                            msg = new Msg()
                            {
                                MessageLevel = eMsgLevel.Error,
                                Message = $"Unable to parse en translation! Line Nr:{source.LineNr} Value: {source.Data[2]}"
                            };
                        }

                        string deTranslation = GetLineContent(source.Data[3]);
                        if (enTranslation == null)
                        {
                            msg = new Msg()
                            {
                                MessageLevel = eMsgLevel.Error,
                                Message = $"Unable to parse de translation! Line Nr:{source.LineNr} Value: {source.Data[3]}"
                            };
                        }

                        if (enTranslation != null && deTranslation != null)
                        {
                            definition = new FileTranslationDefinition()
                            {
                                ID = ID,
                                Owner = className,
                                EnTranslation = enTranslation,
                                DeTranslation = deTranslation
                            };
                        }
                    }
                }
            }

            return (msg, definition);
        }

        private string GetLineContent(string line)
        {
            string content = null;
            if (!string.IsNullOrEmpty(line))
            {
                int index = line.IndexOf("// ");
                if (index > 0 && line.Length > (index + 3))
                {
                    content = line.Substring(index + 3, line.Length - (index + 3));
                    content = content.Trim();
                }
            }
            return content;
        }

        private string ParseID(string line)
        {
            string id = null;
            if (LineMatch(line))
            {
                string content = GetLineContent(line);
                if (content != null)
                {
                    id = content;
                }
            }
            return id;
        }

        #endregion
    }
}
