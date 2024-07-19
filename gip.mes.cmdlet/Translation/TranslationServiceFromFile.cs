using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace gip.mes.cmdlet.Translation
{
    public class TranslationServiceFromFile
    {
        #region const
        public string[] begins = new string[] { "Error", "Info", "Warning" };
        #endregion

        public TranslationServiceFromFile() { }


        public MsgWithDetails UpdateTranslations(Database database, ACProject project, string fileName, string updateName)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();

            List<List<string>> translationPairs = GetTranslationPairs(fileName);

            List< FileTranslationDefinition > definitions = new List<FileTranslationDefinition>();
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

            foreach(FileTranslationDefinition definition in definitions)
            {
                Msg updateMsg = UpdateDefinition(database, definition, project, updateName);
                if(updateMsg != null)
                {
                    msgWithDetails.AddDetailMessage(updateMsg);
                }
            }

            return msgWithDetails;
        }

        private List<List<string>> GetTranslationPairs(string fileName)
        {
            List<List<string>> translationPairs = new List<List<string>>();
            List<string> tempList = null;
            int? inLines = null;
            string[] allLines = File.ReadAllLines(fileName);
            foreach (string line in allLines)
            {
                if (
                        line.Contains($"// {begins[0]}")
                        || line.Contains($"// {begins[1]}")
                        || line.Contains($"// {begins[2]}")
                  )
                {
                    inLines = 0;
                    tempList = new List<string>();
                }

                if (inLines != null)
                {
                    inLines++;
                    tempList.Add(line);

                    if(inLines == 4)
                    {
                        translationPairs.Add(tempList);
                        inLines = null;
                        tempList = null;
                    }
                }
                
                
            }
            return translationPairs;
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
                    message.ACCaptionTranslation = $"en{{'{definition.EnTranslation}'}}de{{'{definition.DeTranslation}'}}";
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
            }
            return msg;
        }

        private (Msg msg, FileTranslationDefinition definition) GetTranslationDefinition(List<string> lines)
        {
            Msg msg = null;
            FileTranslationDefinition definition = null;
            if (lines.Count != 4)
            {
                msg = new Msg()
                {
                    MessageLevel = eMsgLevel.Error,
                    Message = $"Bad linies count!"
                };
            }
            else
            {
                string ID = ParseID(lines[0]);
                if (ID == null)
                {
                    msg = new Msg()
                    {
                        MessageLevel = eMsgLevel.Error,
                        Message = $"Unable to parse ID! Value: {lines[0]}"
                    };
                }
                else
                {
                    string className = GetLineContent(lines[1]);
                    if (className == null)
                    {
                        msg = new Msg()
                        {
                            MessageLevel = eMsgLevel.Error,
                            Message = $"Unable to parse class name! Value: {lines[1]}"
                        };
                    }
                    else
                    {
                        string enTranslation = GetLineContent(lines[2]);
                        if (enTranslation == null)
                        {
                            msg = new Msg()
                            {
                                MessageLevel = eMsgLevel.Error,
                                Message = $"Unable to parse en translation! Value: {lines[2]}"
                            };
                        }

                        string deTranslation = GetLineContent(lines[3]);
                        if (enTranslation == null)
                        {
                            msg = new Msg()
                            {
                                MessageLevel = eMsgLevel.Error,
                                Message = $"Unable to parse de translation! Value: {lines[3]}"
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
            string content = GetLineContent(line);
            if (content != null)
            {
                if (content.StartsWith(begins[0]) || content.StartsWith(begins[1]) || content.StartsWith(begins[2]))
                {
                    id = content;
                }
            }
            return id;
        }
    }
}
