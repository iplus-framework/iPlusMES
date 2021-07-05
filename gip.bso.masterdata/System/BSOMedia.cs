using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Media.Imaging;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioDevelopment, "en{'Update info'}de{'Update Info'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + "Msg")]
    public class BSOMedia : ACBSOvb
    {
        #region Events

        public event EventHandler OnDefaultImageDelete;

        #endregion

        #region const


        #endregion

        #region Configuration

        private ACPropertyConfigValue<string> _MediaRootFolder;
        [ACPropertyConfig("MediaRootFolder")]
        public string MediaRootFolder
        {
            get
            {
                return _MediaRootFolder.ValueT;
            }
            set
            {
                _MediaRootFolder.ValueT = value;
            }
        }

        #endregion

        #region Settings

        public const string BGWorkerMehtod_DeleteFile = @"DeleteFile";

        public MediaSettings MediaSettings { get; set; }

        public MediaController MediaController { get; private set; }

        #endregion

        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOProdOrderGeneric"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOMedia(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _MediaRootFolder = new ACPropertyConfigValue<string>(this, @"MediaRootFolder", @"C:\VarioData\Media");
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

            MediaSettings = new MediaSettings();

            #region TestLoad
            string testMaterialNo = @"001";
            Material material = DatabaseApp.Material.FirstOrDefault(c => c.MaterialNo == testMaterialNo);
            LoadMedia(material);
            #endregion

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            _MediaRootFolder = null;
            MediaSettings = null;
            return base.ACDeInit(deleteACClassTask);
        }

        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            if (vbControl == null)
                return base.OnGetControlModes(vbControl);

            Global.ControlModes result = base.OnGetControlModes(vbControl);
            if (result < Global.ControlModes.Enabled)
                return result;


            return result;
        }

        #endregion

        #region Properties

        #region Properties -> MediaSets

        public MediaSet ImageMediaSet { get; private set; }
        public MediaSet DocumentMediaSet { get; private set; }
        public MediaSet AudioMediaSet { get; private set; }
        public MediaSet VideoMediaSet { get; private set; }

        #endregion

        #region Properties -> Image

        private MediaItemPresentation _SelectedImage;
        /// <summary>
        /// Selected property for string
        /// </summary>
        /// <value>The selected Image</value>
        [ACPropertySelected(9999, "Image", "en{'TODO: Image'}de{'TODO: Image'}")]
        public MediaItemPresentation SelectedImage
        {
            get
            {
                return _SelectedImage;
            }
            set
            {
                if (_SelectedImage != value)
                {
                    _SelectedImage = value;
                    OnPropertyChanged("SelectedImage");
                }
            }
        }

        private List<MediaItemPresentation> _ImageList;
        /// <summary>
        /// List property for string
        /// </summary>
        /// <value>The Image list</value>
        [ACPropertyList(9999, "Image")]
        public List<MediaItemPresentation> ImageList
        {
            get
            {
                if (_ImageList == null)
                    _ImageList = new List<MediaItemPresentation>();
                return _ImageList;
            }
            set
            {
                _ImageList = value;
                OnPropertyChanged("ImageList");
            }
        }

        #endregion

        #region Properties -> Document

        private MediaItemPresentation _SelectedDocument;
        /// <summary>
        /// Selected property for string
        /// </summary>
        /// <value>The selected Document</value>
        [ACPropertySelected(9999, "Document", "en{'TODO: Document'}de{'TODO: Document'}")]
        public MediaItemPresentation SelectedDocument
        {
            get
            {
                return _SelectedDocument;
            }
            set
            {
                if (_SelectedDocument != value)
                {
                    _SelectedDocument = value;
                    OnPropertyChanged("SelectedDocument");
                }
            }
        }

        private List<MediaItemPresentation> _DocumentList;
        /// <summary>
        /// List property for string
        /// </summary>
        /// <value>The Document list</value>
        [ACPropertyList(9999, "Document")]
        public List<MediaItemPresentation> DocumentList
        {
            get
            {
                if (_DocumentList == null)
                    _DocumentList = new List<MediaItemPresentation>();
                return _DocumentList;
            }
            set
            {
                _DocumentList = value;
                OnPropertyChanged("DocumentList");
            }
        }

        #endregion

        #region Properties -> Audio

        private MediaItemPresentation _SelectedAudio;
        /// <summary>
        /// Selected property for string
        /// </summary>
        /// <value>The selected Audio</value>
        [ACPropertySelected(9999, "Audio", "en{'TODO: Audio'}de{'TODO: Audio'}")]
        public MediaItemPresentation SelectedAudio
        {
            get
            {
                return _SelectedAudio;
            }
            set
            {
                if (_SelectedAudio != value)
                {
                    _SelectedAudio = value;
                    OnPropertyChanged("SelectedAudio");
                }
            }
        }


        private List<MediaItemPresentation> _AudioList;
        /// <summary>
        /// List property for string
        /// </summary>
        /// <value>The Audio list</value>
        [ACPropertyList(9999, "Audio")]
        public List<MediaItemPresentation> AudioList
        {
            get
            {
                if (_AudioList == null)
                    _AudioList = new List<MediaItemPresentation>();
                return _AudioList;
            }
            set
            {
                _AudioList = value;
                OnPropertyChanged("AudioList");
            }
        }

        #endregion

        #region Properties -> Video

        private MediaItemPresentation _SelectedVideo;
        /// <summary>
        /// Selected property for string
        /// </summary>
        /// <value>The selected Video</value>
        [ACPropertySelected(9999, "Video", "en{'TODO: Video'}de{'TODO: Video'}")]
        public MediaItemPresentation SelectedVideo
        {
            get
            {
                return _SelectedVideo;
            }
            set
            {
                if (_SelectedVideo != value)
                {
                    _SelectedVideo = value;
                    OnPropertyChanged("SelectedVideo");
                }
            }
        }


        private List<MediaItemPresentation> _VideoList;
        /// <summary>
        /// List property for string
        /// </summary>
        /// <value>The Video list</value>
        [ACPropertyList(9999, "Video")]
        public List<MediaItemPresentation> VideoList
        {
            get
            {
                if (_VideoList == null)
                    _VideoList = new List<MediaItemPresentation>();
                return _VideoList;
            }
            set
            {
                _VideoList = value;
                OnPropertyChanged("VideoList");
            }
        }

        #endregion

        #region Properties -> SelectedTab


        private int _ActiveTabIndex;
        /// <summary>
        /// Doc  ActiveTabIndex
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(79, "ActiveTabIndex", "en{'ActiveTabIndex'}de{'ActiveTabIndex'}")]
        public int ActiveTabIndex
        {
            get
            {
                return _ActiveTabIndex;
            }
            set
            {
                if (_ActiveTabIndex != value)
                {
                    _ActiveTabIndex = value;
                    ActiveTab = (MediaItemTypeEnum)_ActiveTabIndex;
                    OnPropertyChanged("ActiveTabIndex");
                }
            }
        }


        private MediaItemTypeEnum _ActiveTab = MediaItemTypeEnum.Image;
        [ACPropertyInfo(80, "ActiveTab", "en{'ActiveTab'}de{'ActiveTab'}")]
        public MediaItemTypeEnum ActiveTab
        {
            get
            {
                return _ActiveTab;
            }
            set
            {
                if (_ActiveTab != value)
                {
                    _ActiveTab = value;
                    OnPropertyChanged("ActiveTab");
                    OnPropertyChanged("SelectedMediaItemPresentation");
                }
            }
        }

        [ACPropertyInfo(81, "SelectedMediaItemPresentation", "en{'SelectedMediaItemPresentation'}de{'SelectedMediaItemPresentation'}")]
        public MediaItemPresentation SelectedMediaItemPresentation
        {
            get
            {
                MediaItemPresentation item = null;
                switch (ActiveTab)
                {
                    case MediaItemTypeEnum.Image:
                        item = SelectedImage;
                        break;
                    case MediaItemTypeEnum.Document:
                        item = SelectedDocument;
                        break;
                    case MediaItemTypeEnum.Audio:
                        item = SelectedAudio;
                        break;
                    case MediaItemTypeEnum.Video:
                        item = SelectedVideo;
                        break;
                }
                return item;
            }
            set
            {
                switch (ActiveTab)
                {
                    case MediaItemTypeEnum.Image:
                        SelectedImage = value;
                        break;
                    case MediaItemTypeEnum.Document:
                        SelectedDocument = value;
                        break;
                    case MediaItemTypeEnum.Audio:
                        SelectedAudio = value;
                        break;
                    case MediaItemTypeEnum.Video:
                        SelectedVideo = value;
                        break;
                }
                OnPropertyChanged("SelectedMediaItemPresentation");
                SelectedMediaItemPresentation_OnPropertyChanged();
            }
        }

        public void SelectedMediaItemPresentation_OnPropertyChanged()
        {
            switch (ActiveTab)
            {
                case MediaItemTypeEnum.Image:
                    OnPropertyChanged("SelectedImage");
                    break;
                case MediaItemTypeEnum.Document:
                    OnPropertyChanged("SelectedDocument");
                    break;
                case MediaItemTypeEnum.Audio:
                    OnPropertyChanged("SelectedAudio");
                    break;
                case MediaItemTypeEnum.Video:
                    OnPropertyChanged("SelectedVideo");
                    break;
            }
        }

        [ACPropertyInfo(82, "MediaItemPresentationList", "en{'MediaItemPresentationList'}de{'MediaItemPresentationList'}")]
        public List<MediaItemPresentation> MediaItemPresentationList
        {
            get
            {
                List<MediaItemPresentation> list = null;
                switch (ActiveTab)
                {
                    case MediaItemTypeEnum.Image:
                        list = ImageList;
                        break;
                    case MediaItemTypeEnum.Document:
                        list = DocumentList;
                        break;
                    case MediaItemTypeEnum.Audio:
                        list = AudioList;
                        break;
                    case MediaItemTypeEnum.Video:
                        list = VideoList;
                        break;
                }
                return list;
            }
            set
            {
                switch (ActiveTab)
                {
                    case MediaItemTypeEnum.Image:
                        _ImageList = value;
                        break;
                    case MediaItemTypeEnum.Document:
                        _DocumentList = value;
                        break;
                    case MediaItemTypeEnum.Audio:
                        _AudioList = value;
                        break;
                    case MediaItemTypeEnum.Video:
                        _VideoList = value;
                        break;
                }
                MediaItemPresentationList_OnPropertyChanged();
            }
        }

        public void MediaItemPresentationList_OnPropertyChanged()
        {
            switch (ActiveTab)
            {
                case MediaItemTypeEnum.Image:
                    OnPropertyChanged("ImageList");
                    break;
                case MediaItemTypeEnum.Document:
                    OnPropertyChanged("DocumentList");
                    break;
                case MediaItemTypeEnum.Audio:
                    OnPropertyChanged("AudioList");
                    break;
                case MediaItemTypeEnum.Video:
                    OnPropertyChanged("VideoList");
                    break;
            }
            OnPropertyChanged("MediaItemPresentationList");
        }

        #endregion

        #endregion

        #region BackgroundWorker

        #region BackgroundWorker -> BGMethod
        public List<string> DeleteFilesNames { get; set; }

        /// <summary>
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
                case BGWorkerMehtod_DeleteFile:
                    e.Result = DoDeleteFile(worker, e, DeleteFilesNames);
                    break;
            }
        }

        public override void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            base.BgWorkerCompleted(sender, e);
            CloseWindow(this, DesignNameProgressBar);
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = worker.EventArgs.Argument.ToString();

            if (e.Cancelled)
            {
                SendMessage(new Msg() { MessageLevel = eMsgLevel.Info, Message = string.Format(@"Operation {0} canceled by user!", command) });
            }
            else if (e.Error != null)
            {
                SendMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = string.Format(@"Error by doing {0}! Message:{1}", command, e.Error.Message) });
            }
            else
            {
                switch (command)
                {
                    case BGWorkerMehtod_DeleteFile:
                        bool success = (bool)e.Result;
                        DeleteFilesNames = new List<string>();
                        break;
                }
            }
        }
        #endregion

        #region BackgroundWorker -> BGWorker mehtods -> Methods for call

        public bool DoDeleteFile(ACBackgroundWorker worker, DoWorkEventArgs e, List<string> fileNames)
        {
            bool success = true;
            worker.ProgressInfo.OnlyTotalProgress = true;
            worker.ProgressInfo.AddSubTask(BGWorkerMehtod_DeleteFile, 0, fileNames.Count);
            worker.ProgressInfo.TotalProgress.ProgressText = "Start deleting files...";
            int nr = 0;
            foreach (string file in fileNames)
            {
                nr++;
                worker.ProgressInfo.ReportProgress(BGWorkerMehtod_DeleteFile, nr, string.Format(@"Deleting {0} / {1} ...", nr, fileNames.Count));
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    return false;
                }

                bool isDeleted = false;
                int cntTry = 0;
                while (!isDeleted && cntTry < 3)
                {
                    cntTry++;
                    try
                    {
                        Thread.Sleep(100 * 5);
                        if (File.Exists(file))
                            File.Delete(file);
                        isDeleted = true;
                    }
                    catch (Exception ec)
                    {
                        Messages.LogException(this.GetACUrl(), "DoDeleteFile", ec);
                    }
                }
                success = success && isDeleted;
            }
            return success;
        }

        #endregion

        #endregion

        #region Methods

        private IACObject currentACObject;

        public void LoadMedia(IACObject aCObject)
        {
            if (currentACObject == aCObject || !Directory.Exists(MediaSettings.MediaRootFolder)) return;
            currentACObject = aCObject;
            MediaController = new MediaController(MediaSettings, aCObject);
            ImageMediaSet = MediaController.Items[MediaItemTypeEnum.Image];
            DocumentMediaSet = MediaController.Items[MediaItemTypeEnum.Document];
            AudioMediaSet = MediaController.Items[MediaItemTypeEnum.Audio];
            VideoMediaSet = MediaController.Items[MediaItemTypeEnum.Video];

            _ImageList = ImageMediaSet.GetFiles(1);
            if (_ImageList != null && _ImageList.Any())
                SelectedImage = _ImageList.FirstOrDefault();

            _DocumentList = DocumentMediaSet.GetFiles(1);
            if (_DocumentList != null && _DocumentList.Any())
                SelectedDocument = _DocumentList.FirstOrDefault();

            _AudioList = AudioMediaSet.GetFiles(1);
            if (_AudioList != null && _AudioList.Any())
                SelectedAudio = _AudioList.FirstOrDefault();

            _VideoList = VideoMediaSet.GetFiles(1);
            if (_VideoList != null && _VideoList.Any())
                SelectedVideo = _VideoList.FirstOrDefault();

            OnPropertyChanged("ImageList");
            OnPropertyChanged("DocumentList");
            OnPropertyChanged("AudioList");
            OnPropertyChanged("VideoList");
        }

        #region Methods -> ACMethods

        /// <summary>
        /// Exports the folder.
        /// </summary>
        [ACMethodInfo("SetFilePath", "en{'...'}de{'...'}", 9999, false, false, true)]
        public void SetFilePath()
        {
            if (!IsEnabledSetFilePath())
                return;
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = false;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    if (!string.IsNullOrEmpty(dialog.FileName) && File.Exists(dialog.FileName))
                    {
                        SelectedMediaItemPresentation.EditFilePath = dialog.FileName;
                        SelectedMediaItemPresentation_OnPropertyChanged();
                    }
                }
            }
        }

        public bool IsEnabledSetFilePath()
        {
            return SelectedMediaItemPresentation != null;
        }


        /// <summary>
        /// Exports the folder.
        /// </summary>
        [ACMethodInfo("SetFileThumbPath", "en{'...'}de{'...'}", 9999, false, false, true)]
        public void SetFileThumbPath()
        {
            if (!IsEnabledSetFileThumbPath())
                return;
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = false;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    SelectedMediaItemPresentation.EditThumbPath = dialog.FileName;
                    SelectedMediaItemPresentation_OnPropertyChanged();
                }
            }
        }

        public bool IsEnabledSetFileThumbPath()
        {
            return IsEnabledSetFilePath();
        }

        /// <summary>
        /// Method UploadFile
        /// </summary>
        [ACMethodInfo("UploadFile", "en{'Upload file'}de{'Datei hochladen'}", 9999, false, false, true)]
        public void UploadFile()
        {
            if (!IsEnabledUploadFile())
                return;
            try
            {
                SelectedMediaItemPresentation = MediaController.Upload(SelectedMediaItemPresentation);
                SelectedMediaItemPresentation_OnPropertyChanged();
            }
            catch (Exception ec)
            {
                Msg msg = new Msg() { MessageLevel = eMsgLevel.Error, Message = ec.Message };
                SendMessage(msg);
            }

        }
        public bool IsEnabledUploadFile()
        {
            return SelectedMediaItemPresentation != null
                && !string.IsNullOrEmpty(SelectedMediaItemPresentation.EditFilePath)
                && File.Exists(SelectedMediaItemPresentation.EditFilePath)
                && (!SelectedMediaItemPresentation.IsDefault ||
                    (

                        SelectedMediaItemPresentation.IsGenerateThumb ||
                        (!string.IsNullOrEmpty(SelectedMediaItemPresentation.EditThumbPath) && File.Exists(SelectedMediaItemPresentation.EditFilePath))
                     )
                 );
        }


        #endregion

        #region Methods -> ACMethods -> Common

        /// <summary>
        /// Method DownloadImage
        /// </summary>
        [ACMethodInfo("DownloadImage", "en{'Download'}de{'Herunterladen'}", 9999, false, false, true)]
        public void DownloadItem()
        {
            if (!IsEnabledDownloadItem())
                return;
            DownloadFile(SelectedMediaItemPresentation.FilePath);
        }

        public bool IsEnabledDownloadItem()
        {
            return IsEnabledOpenItem();
        }

        /// <summary>
        /// Method DownloadDocument
        /// </summary>
        [ACMethodInfo("OpenItem", "en{'Open'}de{'Öffnen'}", 9999, false, false, true)]
        public void OpenItem()
        {
            if (!IsEnabledOpenItem())
                return;
            System.Diagnostics.Process.Start(SelectedMediaItemPresentation.FilePath);
        }

        public bool IsEnabledOpenItem()
        {
            return
               SelectedMediaItemPresentation != null
               && !string.IsNullOrEmpty(SelectedMediaItemPresentation.FilePath);
        }

        /// <summary>
        /// Method DownloadDocument
        /// </summary>
        [ACMethodInfo("ShowInFolder", "en{'Show in folder'}de{'Im Ordner anzeigen'}", 9999, false, false, true)]
        public void ShowInFolder()
        {
            if (!IsEnabledOpenItem())
                return;
            System.Diagnostics.Process.Start(Path.GetDirectoryName(SelectedMediaItemPresentation.FilePath));
        }

        public bool IsEnabledShowInFolder()
        {
            return IsEnabledOpenItem();
        }

        [ACMethodInfo("Add", "en{'Add'}de{'Neu'}", 9999, false, false, true)]
        public void Add()
        {
            MediaItemPresentation item = new MediaItemPresentation();
            LoadMediaPresentationDefaults(item, ActiveTab == MediaItemTypeEnum.Image);
            item.LoadImage(ActiveTab == MediaItemTypeEnum.Image);
            MediaItemPresentationList.Add(item);
            MediaItemPresentationList_OnPropertyChanged();
            SelectedMediaItemPresentation = item;
        }


        /// <summary>
        /// Method DeleteImage
        /// </summary>
        [ACMethodInfo("DeleteImage", "en{'Delete'}de{'Lösche'}", 9999, false, false, true)]
        public void Delete()
        {
            if (!IsEnabledDelete())
                return;
            DeleteFilesNames = new List<string>();
            MediaItemPresentation item = SelectedMediaItemPresentation;
            SelectedMediaItemPresentation = null;

            MediaItemPresentationList.Remove(item);
            MediaItemPresentationList = MediaItemPresentationList.ToList();

            if (!string.IsNullOrEmpty(item.FilePath))
            {
                item.Image = null;
                item.ImageThumb = null;
                DeleteFilesNames.Add(item.FilePath);
                string fileName = Path.GetFileNameWithoutExtension(item.FilePath);
                if (!string.IsNullOrEmpty(item.ThumbPath) && item.ThumbPath.Contains(fileName))
                    DeleteFilesNames.Add(item.ThumbPath);

                BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DeleteFile);
                ShowDialog(this, DesignNameProgressBar);
            }
        }

        public bool IsEnabledDelete()
        {
            return IsEnabledOpenItem();
        }

        #endregion

        #region Helper methods
        private MediaItemTypeEnum? GetUpladedFileType(string extension)
        {
            KeyValuePair<MediaItemTypeEnum, MediaSet>? searchItem = null;
            foreach (var tmp in MediaController.Items)
            {
                if (tmp.Value.MediaTypeSettings.Extensions.Contains(extension))
                    searchItem = tmp;
            }
            if (searchItem != null)
                return searchItem.Value.Key;
            return null;
        }
        private void DownloadFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (var dialog = new CommonOpenFileDialog())
                {
                    dialog.IsFolderPicker = false;
                    dialog.DefaultFileName = Path.GetFileName(filePath);
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        File.Copy(filePath, dialog.FileName);
                    }
                }
            }
        }

        private void LoadMediaPresentationDefaults(MediaItemPresentation item, bool isImage)
        {
            item.ThumbPath = MediaController.GetEmptyThumbImagePath();
            if (isImage)
                item.FilePath = MediaController.GetEmptyImagePath();
        }

        #endregion
        #endregion

        #region Messages

        public void SendMessage(Msg msg)
        {
            MsgList.Add(msg);
        }


        /// <summary>
        /// The _ current MSG
        /// </summary>
        Msg _CurrentMsg;
        /// <summary>
        /// Gets or sets the current MSG.
        /// </summary>
        /// <value>The current MSG.</value>
        [ACPropertyCurrent(9999, "Message", "en{'Message'}de{'Meldung'}")]
        public Msg CurrentMsg
        {
            get
            {
                return _CurrentMsg;
            }
            set
            {
                _CurrentMsg = value;
                OnPropertyChanged("CurrentMsg");
            }
        }

        private ObservableCollection<Msg> _MsgList;
        /// <summary>
        /// Gets the MSG list.
        /// </summary>
        /// <value>The MSG list.</value>
        [ACPropertyList(9999, "Message", "en{'Messagelist'}de{'Meldungsliste'}")]
        public ObservableCollection<Msg> MsgList
        {
            get
            {
                if (_MsgList == null)
                    _MsgList = new ObservableCollection<Msg>();
                return _MsgList;
            }
        }

        #endregion
    }

}
