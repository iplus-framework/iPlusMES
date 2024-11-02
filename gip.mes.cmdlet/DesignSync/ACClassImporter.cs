// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.bso.iplus;
using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace gip.mes.cmdlet.DesignSync
{

    public class ACClassImporter : ACClassSyncBase
    {

        #region DI params

       

        public string UpdateName { get; set; }

        public bool IsExportACClassDesign { get; set; }
        public bool IsExportACClassText { get; set; }
        public bool IsExportACClassMessage { get; set; }


        #endregion

        public ACClassImporter(string rootFolder, ACProjectManager aCProjectManager, Database database, string updateName, bool isExportACClassDesign = true, bool isExportACClassText = true, bool isExportACClassMessage = true)
        {
            RootFolder = rootFolder;
            ProjectManager = aCProjectManager;
            Database = database;
            UpdateName = updateName;
            IsExportACClassDesign = isExportACClassDesign;
            IsExportACClassText = isExportACClassText;
            IsExportACClassMessage = isExportACClassMessage;
        }

        internal void Import(ACProject acRootProject, string[] classNames, string[] filterItemNames)
        {
            PrepareProjectTree(acRootProject, classNames);
            foreach (var acIdentifer in classNames)
            {
                SendImportMessage(string.Format("Importing class {0}...", acIdentifer));
                ImportClass(acIdentifer, filterItemNames);
            }

            MsgWithDetails saveMessage = Database.ACSaveChanges();
            if (saveMessage == null)
                SendImportMessage("Changes saved!");
            else
                SendImportMessage(saveMessage.Message);
        }

        private void ImportClass(string acIdentifer, string[] filterItemNames)
        {
            KeyValuePair<ACClass, string> rootDefine = DefineRootFolder(acIdentifer);
            string rootFolder = rootDefine.Value;
            ACClass aCClass = rootDefine.Key;
            if (IsExportACClassDesign)
                ImportClassDesign(rootFolder, aCClass, filterItemNames);
            if (IsExportACClassText)
                ImportClassText(rootFolder, aCClass, filterItemNames);
            if (IsExportACClassMessage)
                ImportClassMessage(rootFolder, aCClass, filterItemNames);
        }

        private KeyValuePair<ACClass, string> DefineRootFolder(string acIdentifer)
        {
            string rootFolder = "";
            ACClassInfoWithItems selectedItem = null;

            Action<ACClassInfoWithItems> action = delegate (ACClassInfoWithItems item)
            {
                if (item.ACIdentifier == acIdentifer && item.IsChecked)
                    selectedItem = item;
            };
            ProjectManager.CurrentProjectItemRoot.CallOnAllItems(action);

            ACClassInfoWithItems tmpItem = selectedItem;
            while (tmpItem != null)
            {
                rootFolder = tmpItem.ACIdentifier + @"\" + rootFolder;
                tmpItem = tmpItem.ParentContainerT;
            }

            rootFolder = ProjectManager.CurrentACProject.ACIdentifier + @"\" + rootFolder;

            rootFolder = Path.Combine(RootFolder, rootFolder);

            return new KeyValuePair<ACClass, string>(selectedItem.ValueT, rootFolder);
        }

        private bool FilterMatch(string file, string[] filterItemNames)
        {
            if (filterItemNames == null) return true;
            bool success = false;
            foreach (var item in filterItemNames)
            {
                if (file.Contains(item))
                {
                    success = true;
                    break;
                }
            }
            return success;
        }

        private string[] FilterMatch(string[] files, string[] filterItemNames)
        {
            List<string> matchedFiles = new List<string>();
            foreach (var file in files)
            {
                if (FilterMatch(file, filterItemNames))
                    matchedFiles.Add(file);
            }
            return matchedFiles.OrderBy(c => c).ToArray();
        }


        private void ImportClassMessage(string rootFolder, ACClass aCClass, string[] filterItemNames)
        {
            string[] gipFiles = new DirectoryInfo(rootFolder).GetFiles("ACClassMessage_*.gip").Select(c => c.Name).ToArray();
            string[] gipMatchedFiles = FilterMatch(gipFiles, filterItemNames);
            foreach (var gipFile in gipMatchedFiles)
            {
                string acIdentifier = Path.GetFileNameWithoutExtension(gipFile).Replace("ACClassMessage_", "");
                SendImportMessage(string.Format("ImportClassMessage: {0}", acIdentifier));
                XElement xDoc = XElement.Load(Path.Combine(rootFolder, gipFile));
                string acCaptionTranslation = xDoc.Element("ACCaptionTranslation").Value;
                ACClassMessage aCClassMessage = aCClass.ACClassMessage_ACClass.FirstOrDefault(c => c.ACIdentifier == acIdentifier);
                if (aCClassMessage == null)
                {
                    aCClassMessage = ACClassMessage.NewACObject(Database, aCClass);
                    aCClassMessage.ACIdentifier = acIdentifier;
                    aCClassMessage.InsertName = UpdateName;
                    aCClassMessage.InsertDate = DateTime.Now;
                }
                aCClassMessage.ACCaptionTranslation = acCaptionTranslation;
            }
        }

        private void ImportClassText(string rootFolder, ACClass aCClass, string[] filterItemNames)
        {
            string[] gipFiles = new DirectoryInfo(rootFolder).GetFiles("ACClassText_*.gip").Select(c => c.Name).ToArray();
            string[] gipMatchedFiles = FilterMatch(gipFiles, filterItemNames);
            foreach (var gipFile in gipMatchedFiles)
            {
                string acIdentifier = Path.GetFileNameWithoutExtension(gipFile).Replace("ACClassText_", "");
                SendImportMessage(string.Format("ImportClassText: {0}", acIdentifier));
                XElement xDoc = XElement.Load(Path.Combine(rootFolder, gipFile));
                string acCaptionTranslation = xDoc.Element("ACCaptionTranslation").Value;
                ACClassText aCClassText = aCClass.ACClassText_ACClass.FirstOrDefault(c => c.ACIdentifier == acIdentifier);
                if (aCClassText == null)
                {
                    aCClassText = ACClassText.NewACObject(Database, aCClass);
                    aCClassText.ACIdentifier = acIdentifier;
                    aCClassText.InsertName = UpdateName;
                    aCClassText.InsertDate = DateTime.Now;
                }
                aCClassText.ACCaptionTranslation = acCaptionTranslation;
            }
        }

        private void ImportClassDesign(string rootFolder, ACClass aCClass, string[] filterItemNames)
        {
            string[] gipFiles = new DirectoryInfo(rootFolder).GetFiles("ACClassDesign_*.gip").Select(c => c.Name).ToArray();
            string[] gipMatchedFiles = FilterMatch(gipFiles, filterItemNames);
            foreach (var gipFile in gipMatchedFiles)
            {
                string xmlFile = gipFile.Replace(".gip", ".xml");
                if(File.Exists(Path.Combine(rootFolder, xmlFile)))
                {
                    string acIdentifier = Path.GetFileNameWithoutExtension(gipFile).Replace("ACClassDesign_", "");
                    SendImportMessage(string.Format("ImportClassDesign: {0}", acIdentifier));
                    ACClassDesign aCClassDesign = aCClass.ACClassDesign_ACClass.FirstOrDefault(c => c.ACIdentifier == acIdentifier);
                    if (aCClassDesign == null)
                    {
                        string secondaryKey = ACRoot.SRoot.NoManager.GetNewNo(Database, typeof(ACClassDesign), ACClassDesign.NoColumnName, ACClassDesign.FormatNewNo, null);
                        aCClassDesign = ACClassDesign.NewACObject(Database, aCClass, secondaryKey);
                        aCClassDesign.ACIdentifier = acIdentifier;
                        aCClassDesign.InsertName = UpdateName;
                        aCClassDesign.InsertDate = DateTime.Now;
                    }
                    // 1. Load deign
                    aCClassDesign.XMLDesign = File.ReadAllText(Path.Combine(rootFolder, xmlFile));

                    // 2. Save other params - translation etc
                    XElement xDoc = XElement.Load(Path.Combine(rootFolder, gipFile));

                    // ACCaptionTranslation
                    aCClassDesign.ACCaptionTranslation = xDoc.Element("ACCaptionTranslation").Value;

                    //DesignNo
                    aCClassDesign.DesignNo = xDoc.Element("DesignNo").Value;

                    //ACKindIndex
                    aCClassDesign.ACKindIndex = SetValue<short>(xDoc, "ACKindIndex", (short)Global.ACKinds.DSDesignLayout);

                    //ACUsageIndex
                    aCClassDesign.ACUsageIndex = SetValue<short>(xDoc, "ACUsageIndex", (short)Global.ACUsages.DULayout);


                    //SortIndex
                    aCClassDesign.SortIndex = SetValue<short>(xDoc, "SortIndex", 999);

                    //IsRightmanagement
                    aCClassDesign.IsRightmanagement = SetValue<bool>(xDoc, "IsRightmanagement", false);

                    //Comment
                    aCClassDesign.Comment = xDoc.Element("Comment").Value;

                    //IsDefault
                    aCClassDesign.IsDefault = SetValue<bool>(xDoc, "IsDefault", false);

                    //IsResourceStyle
                    aCClassDesign.IsResourceStyle = SetValue<bool>(xDoc, "IsResourceStyle", false);

                    //VisualHeight
                    aCClassDesign.VisualHeight = SetValue<double>(xDoc, "VisualHeight", 0);


                    //VisualWidth
                    aCClassDesign.VisualWidth = SetValue<double>(xDoc, "VisualWidth", 0);

                    //XMLConfig
                    aCClassDesign.XMLConfig = xDoc.Element("XMLConfig").Value;

                    //BranchNo
                    aCClassDesign.BranchNo = SetValue<int>(xDoc, "BranchNo", 0);
                }
            }
        }

        public T SetValue<T>(XElement xDoc, string name, T defaultValue) where T : struct
        {
            string strValue = xDoc.Element(name).Value;
            T newValue = defaultValue;
            TypeConverter typeConverter = new TypeConverter();
            return ConversionExtensions.Convert<T>(strValue);
        }

        public void PrepareProjectTree(ACProject project, string[] acIdentifers)
        {
            LoadProjectTree(project, acIdentifers);
            ACEntitySerializer aCEntitySerializer = new ACEntitySerializer();
            ACQueryDefinition qryACProject = gip.core.datamodel.Database.Root.Queries.CreateQuery(Database as IACComponent, Const.QueryPrefix + ACProject.ClassName, null);
            ACQueryDefinition qryACClass = qryACProject.ACUrlCommand(Const.QueryPrefix + ACClass.ClassName) as ACQueryDefinition;

        }

        private void LoadProjectTree(ACProject project, string[] acIdentifers)
        {
            ACProjectManager.PresentationMode projectTreePresentationMode
                                = new ACProjectManager.PresentationMode()
                                {
                                    ShowCaptionInTree = false,
                                    DisplayGroupedTree = true
                                    //DisplayTreeAsMenu = this.CurrentMenuRootItem
                                };

            ACClassInfoWithItems.VisibilityFilters projectTreeVisibilityFilter
                  = new ACClassInfoWithItems.VisibilityFilters()
                  {
                      SearchText = null,
                      IncludeLibraryClasses = false
                  };
            core.datamodel.ACClassInfoWithItems.CheckHandler projectTreeCheckHandler = new ACClassInfoWithItems.CheckHandler()
            {
                QueryRightsFromDB = true,
                IsCheckboxVisible = true,
                CheckedSetter = null,
                CheckedGetter = null,
                CheckIsEnabledGetter = null,
            };
            ProjectManager.LoadACProject(project.ACProjectID, projectTreePresentationMode, projectTreeVisibilityFilter, projectTreeCheckHandler);
            ProjectManager.CurrentProjectItemRoot.CallOnAllItems((ACClassInfoWithItems item) => item.IsChecked = (acIdentifers == null || acIdentifers.Contains(item.ACIdentifier)));
        }

        
    }
}
