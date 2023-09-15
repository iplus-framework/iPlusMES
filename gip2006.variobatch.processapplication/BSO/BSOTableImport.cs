// ***********************************************************************
// Assembly         : gip.bso.iplus
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 11-07-2012
// ***********************************************************************
// <copyright file="BSOTableImport.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.mes.autocomponent;
using System.Threading;
using System.ComponentModel;
using System.IO;
using gip.core.processapplication;
using System.Data;
using gip.core.media;

namespace gip2006.variobatch.processapplication
{
    /// <summary>
    /// Class BSOTableImport
    /// </summary>
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'Import tables vb Classic'}de{'Import Tabellen aus vb Classic'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, false, true)]
    public class BSOTableImport : ACBSOvb
    {
        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOTableImport"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOTableImport(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            this._BgList = null;
            this._BSOMsg = null;
            this._CurrentImportDataTable = null;
            this._CurrentImportFile = null;
            this._CurrentImportFileTxt = null;
            this._CurrentImportFolder = null;
            this._CurrentImportItem = null;
            this._CurrentImportItemRoot = null;
            this._CurrentInvokingACCommand = null;
            this._Importer = null;
            bool done = base.ACDeInit(deleteACClassTask);
            return done;
        }


        /// <summary>
        /// ACs the init.
        /// </summary>
        /// <param name="startChildMode">The start child mode.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;


            CurrentImportFolder = Root.Environment.Datapath;

            return true;
        }
        #endregion

        #region BSO->ACProperty

        #region ChildImporter
        GIPConv2006TableImport _Importer = null;
        public GIPConv2006TableImport Importer
        {
            get
            {
                if (_Importer == null)
                    _Importer = FindChildComponents<GIPConv2006TableImport>(c => c is GIPConv2006TableImport, null, 1).FirstOrDefault();
                return _Importer;
            }
        }
        #endregion

        #region Messages/Progress

        private bool _PreviewFile = false;
        [ACPropertyInfo(9999, "", "en{'Preview'}de{'Datei-Vorschau'}")]
        public bool PreviewFile
        {
            get
            {
                return _PreviewFile;
            }
            set
            {
                _PreviewFile = value;
                OnPropertyChanged("PreviewFile");
            }
        }


        private MsgWithDetails _BSOMsg = new MsgWithDetails();
        [ACPropertyInfo(9999, "Message")]
        public MsgWithDetails BSOMsg
        {
            get
            {
                return _BSOMsg;
            }
            set
            {
                _BSOMsg = value;
                OnPropertyChanged("BSOMsg");
            }
        }

        private List<Msg> _BgList = new List<Msg>();
        #endregion

        #region Import
        /// <summary>
        /// The _ current import folder
        /// </summary>
        string _CurrentImportFolder;
        /// <summary>
        /// Gets or sets the current import folder.
        /// </summary>
        /// <value>The current import folder.</value>
        [ACPropertyCurrent(9999, "ImportFolder", "en{'ImportFolder'}de{'Importordner'}")]
        public string CurrentImportFolder
        {
            get
            {
                return _CurrentImportFolder;
            }
            set
            {
                _CurrentImportFolder = value;
                try
                {
                    CurrentImportItemRoot = GetImportItemRoot(_CurrentImportFolder);
                }
                catch(Exception e)
                {
                    string msg = e.Message;
                    if (e.InnerException != null && e.InnerException.Message != null)
                        msg += " Inner:" + e.InnerException.Message;

                    Messages.LogException("BSOTableImport", "CurrentImportFolder", msg);
                }
                OnPropertyChanged("CurrentImportFolder");
            }
        }

        /// <summary>
        /// RootVerzeichnis des Projektes, das zu Importieren ist. Wird als Source im VBTreeView verwendet
        /// </summary>
        ACFSItem _CurrentImportItemRoot;
        /// <summary>
        /// Gets or sets the current import project item root.
        /// </summary>
        /// <value>The current import project item root.</value>
        [ACPropertyCurrent(9999, "ImportItemRoot")]
        public ACFSItem CurrentImportItemRoot
        {
            get
            {
                return _CurrentImportItemRoot;
            }
            set
            {
                _CurrentImportItemRoot = value;
                OnPropertyChanged("CurrentImportItemRoot");
            }

        }

        /// <summary>
        /// Aktuell ausgewähltes Verzeichnis im VBTreeView
        /// </summary>
        ACFSItem _CurrentImportItem = null;
        /// <summary>   
        /// Gets or sets the current import project item.
        /// </summary>
        /// <value>The current import project item.</value>
        [ACPropertyCurrent(9999, "ImportItem")]
        public ACFSItem CurrentImportItem
        {
            get
            {
                return _CurrentImportItem;
            }
            set
            {
                if (_CurrentImportItem != value)
                {
                    _CurrentImportItem = value;
                    OnPropertyChanged("CurrentImportItem");
                    OnPropertyChanged("ImportFileList");
                    CurrentImportFile = value;
                }
            }
        }

        /// <summary>
        /// The _ current import file
        /// </summary>
        ACFSItem _CurrentImportFile;
        /// <summary>
        /// Gets or sets the current import file.
        /// </summary>
        /// <value>The current import file.</value>
        [ACPropertyCurrent(9999, "ImportFile")]
        public ACFSItem CurrentImportFile
        {
            get
            {
                return _CurrentImportFile;
            }
            set
            {
                if (_CurrentImportFile != value)
                {
                    _CurrentImportFile = value;

                    if (value == null || !PreviewFile)
                    {
                        CurrentImportFileTxt = null;
                    }
                    else if (PreviewFile)
                    {
                        string txt = File.ReadAllText(_CurrentImportFile.Path, Encoding.GetEncoding(1252));
                        CurrentImportFileTxt = txt;
                    }

                    OnPropertyChanged("CurrentImportFile");
                }
            }
        }

        /// <summary>
        /// Gets the import file list.
        /// </summary>
        /// <value>The import file list.</value>
        [ACPropertyList(9999, "ImportFile")]
        public IEnumerable<ACFSItem> ImportFileList
        {
            get
            {
                if (CurrentImportItem == null || !CurrentImportItem.Items.Any())
                    return null;
                return CurrentImportItem.Items.Select(x => x as ACFSItem).Where(x => x.ResourceType == ResourceTypeEnum.IACObject).OrderBy(x => x.ACCaption);
            }
        }

        /// <summary>
        /// The _ current import file XML
        /// </summary>
        string _CurrentImportFileTxt;
        /// <summary>
        /// Gets or sets the current import file XML.
        /// </summary>
        /// <value>The current import file XML.</value>
        [ACPropertyCurrent(9999, "", "en{'Filecontent'}de{'Dateiinhalt'}")]
        public string CurrentImportFileTxt
        {
            get
            {
                return _CurrentImportFileTxt;
            }
            set
            {
                _CurrentImportFileTxt = value;
                OnPropertyChanged("CurrentImportFileTxt");
                OnPropertyChanged("CurrentImportDataTable");
            }
        }

        private string CorrectedCSVImportTxt
        {
            get
            {
                return GIPConv2006TableImport.CorrectCVSString(CurrentImportFileTxt);
            }
        }

        DataTable _CurrentImportDataTable;
        [ACPropertyInfo(9999)]
        public DataTable CurrentImportDataTable
        {
            get
            {
                String txt = CorrectedCSVImportTxt;
                if (String.IsNullOrEmpty(txt))
                {
                    _CurrentImportDataTable = null;
                }
                else
                {
                    using (CSVReader reader = new CSVReader(txt))
                    {
                        _CurrentImportDataTable = reader.CreateDataTable(true);
                    }
                }
                return _CurrentImportDataTable;
            }
        }

        #endregion

        #endregion

        #region BSO->ACMethod

        #region Import
        /// <summary>
        /// Imports this instance.
        /// </summary>
        [ACMethodInfo("Import", "en{'Import'}de{'Import'}", 9999, false, false, true, Global.ACKinds.MSMethodPrePost)]
        public void Import()
        {
            if (BackgroundWorker.IsBusy)
                return;
            if (!PreExecute("Import"))
                return;
            OnlyCompare = false;
            BSOMsg.ClearMsgDetails();
            _BgList = new List<Msg>();


            // TODO: @aagincic: Rewrite progress - Import()
            //BackgroundWorker.ProgressInfo.TotalProgressRangeFrom = 0;
            //BackgroundWorker.ProgressInfo.TotalProgressRangeTo = CurrentImportItemRoot.GetCountRecursive();
            //BackgroundWorker.ProgressInfo.TotalProgressCurrent = 0;

            BackgroundWorker.RunWorkerAsync("Import");
            ShowDialog(this, DesignNameProgressBar);
        }

        /// <summary>
        /// Determines whether [is enabled import].
        /// </summary>
        /// <returns><c>true</c> if [is enabled import]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledImport()
        {
            return CurrentImportItemRoot != null && !BackgroundWorker.IsBusy && Importer != null;
        }

        /// <summary>
        /// Imports the file.
        /// </summary>
        [ACMethodInfo("Import", "en{'Import File'}de{'Import Datei'}", 9999, false, false, true)]
        public void ImportFile()
        {
            if (BackgroundWorker.IsBusy)
                return;
            if (!PreExecute("ImportFile"))
                return;
            OnlyCompare = false;
            BSOMsg.ClearMsgDetails();
            _BgList = new List<Msg>();


            // TODO: @aagincic: Rewrite progress - ImportFile()
            //BackgroundWorker.ProgressInfo.TotalProgressRangeFrom = 0;
            //BackgroundWorker.ProgressInfo.TotalProgressRangeTo = CurrentImportFile.GetCountRecursive();
            //BackgroundWorker.ProgressInfo.TotalProgressCurrent = 0;

            BackgroundWorker.RunWorkerAsync("ImportOne");
            ShowDialog(this, DesignNameProgressBar);
        }

        /// <summary>
        /// Determines whether [is enabled import file].
        /// </summary>
        /// <returns><c>true</c> if [is enabled import file]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledImportFile()
        {
            return !String.IsNullOrEmpty(CurrentImportFileTxt) && CurrentImportFile != null && !BackgroundWorker.IsBusy && Importer != null;
        }

        /// <summary>
        /// Imports the folder.
        /// </summary>
        [ACMethodInfo("Import", "en{'...'}de{'...'}", 9999, false, false, true)]
        public void ImportFolder()
        {
            ACMediaController mediaController = ACMediaController.GetServiceInstance(this);
            string folderPath = mediaController.OpenFileDialog(true, CurrentImportFolder, true);
            if (!string.IsNullOrEmpty(folderPath))
            {
                CurrentImportFolder = folderPath;
            }
        }
        #endregion

        #endregion

        #region Import/Export

        #region BackgroundWorker

        /// <summary>
        /// 1. Dieser Eventhandler wird aufgerufen, wenn Hintergrundjob starten soll
        /// Dies wird ausgelöst durch den Aufruf der Methode RunWorkerAsync()
        /// Methode läuft im Hintergrundthread
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        public override void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            base.BgWorkerDoWork(sender, e);
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = e.Argument.ToString();
            switch (command)
            {
                case "Import":
                    DoImport();
                    break;
                case "ImportOne":
                    DoImportOne();
                    break;
            }
        }

        /// <summary>
        /// 2. Dieser Eventhandler wird aufgerufen, wenn Hintergrundjob erledigt ist
        /// Methode läuft im Benutzerthread
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        public override void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Falls Exception geworfen wurde
            if (e.Error != null)
            {
            }
            // Falls Hintergrundprozess vorzeitig vom Benutzer abgebrochen worden ist
            else if (e.Cancelled)
            {
            }
            // Sonst erfolgreicher Durchlauf
            else
            {
            }
            CloseTopDialog();
            // Lösche Balken

            // TODO: @aagincic: Rewrite progress - BgWorkerCompleted();
            //CurrentProgressInfo.TotalProgressCurrent = 0;
            //CurrentProgressInfo.SubProgressCurrent = 0;

            foreach (Msg msg in _BgList)
            {
                Messages.Msg(msg);
                MsgWithDetails details = msg as MsgWithDetails;
                if (details != null)
                {
                    foreach (Msg subMsg in details.MsgDetails)
                    {
                        BSOMsg.AddDetailMessage(subMsg);
                    }
                }
                else
                    BSOMsg.AddDetailMessage(msg);
            }
            //Messages.GlobalMsg.AddDetailMessage(result);
        }

        #endregion

        #region Import
        /// <summary>
        /// Gets or sets a value indicating whether [only compare].
        /// </summary>
        /// <value><c>true</c> if [only compare]; otherwise, <c>false</c>.</value>
        public bool OnlyCompare { get; set; }

        /// <summary>
        /// Does the import.
        /// </summary>
        public void DoImport()
        {
            if (!ImportRekursiv(CurrentImportItemRoot, OnlyCompare))
            {
                return;
            }
        }

        public void DoImportOne()
        {
            // TODO: @aagincic: Rewrite progress - DoImportOne();

            //BackgroundWorker.ProgressInfo.TotalProgressCurrent++;
            //BackgroundWorker.ProgressInfo.TotalProgressText = CurrentImportFile.ACUrlFS;
            //BackgroundWorker.ReportProgress();

            //Msg result = Importer.ImportString(CurrentImportFileTxt, Path.GetFileName(CurrentImportFile.Path), DatabaseApp, GIPConv2006TableImport.AutoSaveMode.SaveAfterEveryRow, BackgroundWorker);
            Msg result = Importer.ImportDataTable(CurrentImportDataTable, Path.GetFileName(CurrentImportFile.Path), DatabaseApp, GIPConv2006TableImport.AutoSaveMode.SaveAfterEveryRow, BackgroundWorker);
            if (result != null)
            {
                _BgList.Add(result);
            }
        }

        /// <summary>
        /// Imports the rekursiv.
        /// </summary>
        /// <param name="fsItem">The fs item.</param>
        /// <param name="onlyCompare">if set to <c>true</c> [only compare].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool ImportRekursiv(ACFSItem fsItem, bool onlyCompare)
        {
            // TODO: @aagincic - Rewrite progress - ImportRekursiv();
            //BackgroundWorker.ProgressInfo.TotalProgressCurrent++;
            //BackgroundWorker.ProgressInfo.TotalProgressText = fsItem.ACUrlFS;
            //BackgroundWorker.ReportProgress();

            if (fsItem.IsChecked && fsItem.ResourceType != ResourceTypeEnum.IACObject )
            {
                if (BackgroundWorker.CancellationPending)
                {
                    return false;
                }

                Msg result = Importer.ImportFile(fsItem.Path, DatabaseApp, GIPConv2006TableImport.AutoSaveMode.SaveAfterEveryRow, BackgroundWorker);
                if (result != null)
                {
                    _BgList.Add(result);
                }
                Thread.Sleep(100);
                string fileName = fsItem.ACUrlFS.Replace("\\Resources\\", "");
                string xml = File.ReadAllText(fileName);
            }


            foreach (var item in fsItem.Items)
            {
                if (!(item is ACFSItem))
                    continue;
                ACFSItem folder = item as ACFSItem;
                if (!ImportRekursiv(folder, onlyCompare))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the import project item root.
        /// </summary>
        /// <param name="importFolderProject">The import folder project.</param>
        /// <returns>ACFSItem.</returns>
        public ACFSItem GetImportItemRoot(string importFolderProject)
        {
            Resources rs = new Resources();
            return rs.Dir(Database, new ACFSItemContainer(Database), importFolderProject, true);
        }
        #endregion

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case"Import":
                    Import();
                    return true;
                case"IsEnabledImport":
                    result = IsEnabledImport();
                    return true;
                case"ImportFile":
                    ImportFile();
                    return true;
                case"IsEnabledImportFile":
                    result = IsEnabledImportFile();
                    return true;
                case"ImportFolder":
                    ImportFolder();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }
}
