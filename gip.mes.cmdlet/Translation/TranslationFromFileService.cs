using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace gip.mes.cmdlet.Translation
{
    public class TranslationFromFileService
    {
        #region const
        public string[] begins = new string[] { @"\sError(\d\d\d\d\d)", @"\sInfo(\d\d\d\d\d)", @"\sWarning(\d\d\d\d\d)" };
        #endregion

        public TranslationFromFileService() { }


        public MsgWithDetails UpdateTranslations(Database database, ACProject project, string fileName, string updateName)
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
    }
}
