using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.facility;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioDevelopment, "en{'Update info'}de{'Update Info'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + "Msg")]
    public class BSOMedia : ACBSO
    {
        #region Events

        public event EventHandler OnDefaultImageDelete;

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
            switch (vbControl.VBContent)
            {
                case "IsGenerateThumb":
                    return IsExistingImageSelected ? Global.ControlModes.Enabled : Global.ControlModes.Disabled;
                case "ISSetAsDefaultImage":
                    return IsExistingImageSelected ? Global.ControlModes.Enabled : Global.ControlModes.Disabled;
            }

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

        #region Properties -> ACProperties

        private string _DefaultImage;

        [ACPropertyInfo(999, "DefaultImage", "en{'Default image'}de{'Standardbild'}")]
        public string DefaultImage
        {
            get
            {
                return _DefaultImage;
            }
            set
            {
                if (_DefaultImage != value)
                {
                    _DefaultImage = value;
                    OnPropertyChanged("DefaultImage");
                }
            }
        }

        private string _DefaultThumbImage;

        [ACPropertyInfo(999, "DefaultThumbImage", "en{'Default thumb image'}de{'Standard-Bildzeichen'}")]
        public string DefaultThumbImage
        {
            get
            {
                return _DefaultThumbImage;
            }
            set
            {
                if (_DefaultThumbImage != value)
                {
                    _DefaultThumbImage = value;
                    OnPropertyChanged("DefaultThumbImage");
                }
            }
        }

        private bool _IsExistingImageSelected;
        public bool IsExistingImageSelected
        {
            get
            {
                return _IsExistingImageSelected;
            }
            set
            {
                if (_IsExistingImageSelected != value)
                {
                    _IsExistingImageSelected = value;
                    if (!value)
                    {
                        _IsGenerateThumb = false;
                        _ISSetAsDefaultImage = false;
                    }
                    OnPropertyChanged("IsGenerateThumb");
                    OnPropertyChanged("ISSetAsDefaultImage");
                }
            }
        }

        private string _FilePath;
        /// <summary>
        /// Selected property for 
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "FilePath", "en{'Path'}de{'Dateipfad'}")]
        public string FilePath
        {
            get
            {
                return _FilePath;
            }
            set
            {
                if (_FilePath != value)
                {
                    IsExistingImageSelected = false;
                    _FilePath = value;
                    if (value != null && File.Exists(value))
                    {
                        MediaTypeSettingsItem mediaTypeSettingsItem = MediaSettings.MediaItemTypes.Where(c => c.MediaType == MediaItemTypeEnum.Image).FirstOrDefault();
                        if (mediaTypeSettingsItem.Extensions.Contains(Path.GetExtension(value)))
                            IsExistingImageSelected = true;
                    }
                    OnPropertyChanged("FilePath");
                }
            }
        }

        private string _FileThumbPath;
        /// <summary>
        /// Selected property for 
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "FileThumbPath", "en{'Thumb file path'}de{'(Bildzeichen) Dateipfad'}")]
        public string FileThumbPath
        {
            get
            {
                return _FileThumbPath;
            }
            set
            {
                if (_FileThumbPath != value)
                {
                    _FileThumbPath = value;
                    if (value != null && File.Exists(value))
                    {
                        MediaTypeSettingsItem mediaTypeSettingsItem = MediaSettings.MediaItemTypes.Where(c => c.MediaType == MediaItemTypeEnum.Image).FirstOrDefault();
                        if (!mediaTypeSettingsItem.Extensions.Contains(Path.GetExtension(value)))
                            _FileThumbPath = null;
                    }
                    OnPropertyChanged("FileThumbPath");
                }
            }
        }

        private bool _IsGenerateThumb;
        /// <summary>
        /// Selected property for 
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "IsGenerateThumb", "en{'Generate thumb'}de{'Generiere Bildzeichen'}")]
        public bool IsGenerateThumb
        {
            get
            {
                return _IsGenerateThumb;
            }
            set
            {
                if (_IsGenerateThumb != value)
                {
                    _IsGenerateThumb = value;
                    if (_IsGenerateThumb)
                        FileThumbPath = null;
                    OnPropertyChanged("IsGenerateThumb");
                }
            }
        }

        private bool _ISSetAsDefaultImage;
        /// <summary>
        /// Selected property for 
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "ISSetAsDefaultImage", "en{'Set as default image'}de{'Stelle as Standardbild'}")]
        public bool ISSetAsDefaultImage
        {
            get
            {
                return _ISSetAsDefaultImage;
            }
            set
            {
                if (_ISSetAsDefaultImage != value)
                {
                    _ISSetAsDefaultImage = value;
                    OnPropertyChanged("IsSetAsDefaultImage");
                }
            }
        }

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
        }

        #endregion

        #region Properties -> Video


        #region Video
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
        }

        #endregion


        #endregion

        #endregion

        #region BackgroundWorker

        #region BackgroundWorker 
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
            DefaultImage = MediaController.DefaultImage;
            DefaultThumbImage = MediaController.DefaultThumbImage;
            ImageMediaSet = MediaController.Items[MediaItemTypeEnum.Image];
            DocumentMediaSet = MediaController.Items[MediaItemTypeEnum.Document];
            AudioMediaSet = MediaController.Items[MediaItemTypeEnum.Audio];
            VideoMediaSet = MediaController.Items[MediaItemTypeEnum.Video];

            _ImageList = ImageMediaSet.GetFiles(1);
            _DocumentList = DocumentMediaSet.GetFiles(1);
            _AudioList = AudioMediaSet.GetFiles(1);
            _VideoList = VideoMediaSet.GetFiles(1);

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
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = false;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    if (!string.IsNullOrEmpty(dialog.FileName) && File.Exists(dialog.FileName))
                    {
                        FilePath = dialog.FileName;
                    }
                }
            }
        }

        /// <summary>
        /// Exports the folder.
        /// </summary>
        [ACMethodInfo("SetFileThumbPath", "en{'...'}de{'...'}", 9999, false, false, true)]
        public void SetFileThumbPath()
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = false;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    if (File.Exists(dialog.FileName))
                    {
                        FileThumbPath = dialog.FileName;
                    }
                }
            }
        }

        /// <summary>
        /// Method UploadFile
        /// </summary>
        [ACMethodInfo("UploadFile", "en{'Upload file'}de{'Datei hochladen'}", 9999, false, false, true)]
        public void UploadFile()
        {
            if (!IsEnabledUploadFile()) return;
            string extension = Path.GetExtension(FilePath);
            MediaItemTypeEnum? mediaType = GetUpladedFileType(extension);
            if (mediaType != null)
            {
                if (mediaType != MediaItemTypeEnum.Image && (IsGenerateThumb || ISSetAsDefaultImage)) return;

                if (mediaType != MediaItemTypeEnum.Image)
                {
                    MediaController.UploadFile(FilePath);
                    if (!string.IsNullOrEmpty(FileThumbPath) && File.Exists(FileThumbPath))
                    {
                        string recomendedFileName =
                            Path.GetFileNameWithoutExtension(FilePath) +
                            MediaController.MediaSettings.DefaultThumbSuffix +
                            Path.GetExtension(FileThumbPath);
                        MediaSet mediaSet = MediaController.Items.Where(c => c.Value.MediaTypeSettings.Extensions.Contains(extension)).Select(c => c.Value).FirstOrDefault();
                        MediaController.UploadFile(FileThumbPath, Path.Combine(mediaSet.ItemRootFolder, recomendedFileName));
                    }
                    switch (mediaType)
                    {
                        case MediaItemTypeEnum.Document:
                            _DocumentList = DocumentMediaSet.GetFiles(1);
                            OnPropertyChanged("DocumentList");
                            break;
                        case MediaItemTypeEnum.Audio:
                            _AudioList = AudioMediaSet.GetFiles(1);
                            OnPropertyChanged("AudioList");
                            break;
                        case MediaItemTypeEnum.Video:
                            _VideoList = VideoMediaSet.GetFiles(1);
                            OnPropertyChanged("VideoList");
                            break;
                    }
                }
                else
                {
                    MediaController.UploadImage(FilePath, FileThumbPath, IsGenerateThumb, ISSetAsDefaultImage);
                    _ImageList = ImageMediaSet.GetFiles(1);
                    OnPropertyChanged("ImageList");
                    if (ISSetAsDefaultImage)
                    {
                        MediaController.LoadDefaultImage();
                        DefaultImage = MediaController.DefaultImage;
                        DefaultThumbImage = MediaController.DefaultThumbImage;
                        OnPropertyChanged("DefaultImage");
                        OnPropertyChanged("DefaultThumbImage");
                    }
                }
            }
            ClearInput();
        }
        public bool IsEnabledUploadFile()
        {
            return !string.IsNullOrEmpty(FilePath) &&
                File.Exists(FilePath) &&
                (!ISSetAsDefaultImage ||
                    (

                        IsGenerateThumb ||
                        (!string.IsNullOrEmpty(FileThumbPath) && File.Exists(FilePath))
                     )
                 );
        }
        public void ClearInput()
        {
            _FilePath = null;
            _FileThumbPath = null;
            _IsGenerateThumb = false;
            _ISSetAsDefaultImage = false;

            OnPropertyChanged("FilePath");
            OnPropertyChanged("FileThumbPath");
            OnPropertyChanged("IsGenerateThumb");
            OnPropertyChanged("ISSetAsDefaultImage");
        }

        #endregion

        #region Methods -> ACMethods -> Delete

        /// <summary>
        /// Method DeleteDefaultImage
        /// </summary>
        [ACMethodInfo("DeleteDefaultImage", "en{'Delete default image'}de{'Lösche Standardbild'}", 9999, false, false, true)]
        public void DeleteDefaultImage()
        {
            if (!IsEnabledDeleteDefaultImage()) return;
            DeleteFilesNames = new List<string>();
            DeleteFilesNames.Add(DefaultImage);
            if (!string.IsNullOrEmpty(DefaultThumbImage))
                DeleteFilesNames.Add(DefaultThumbImage);

            DefaultImage = null;
            DefaultThumbImage = null;
            if (OnDefaultImageDelete != null)
                OnDefaultImageDelete(this, new EventArgs());

            MediaItemPresentation image = ImageList.FirstOrDefault(c => c.FilePath == DefaultImage);
            if (image != null)
            {
                ImageList.Remove(image);
                _ImageList = ImageList.ToList();
                OnPropertyChanged("ImageList");
            }

            IImageInfo imageInfo = currentACObject as IImageInfo;
            imageInfo.DefaultImage = null;
            imageInfo.DefaultThumbImage = null;

            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DeleteFile);
            ShowDialog(this, DesignNameProgressBar);
        }
        public bool IsEnabledDeleteDefaultImage()
        {
            return !string.IsNullOrEmpty(DefaultImage);
        }

        /// <summary>
        /// Method DeleteImage
        /// </summary>
        [ACMethodInfo("DeleteImage", "en{'Delete'}de{'Lösche'}", 9999, false, false, true)]
        public void DeleteImage()
        {
            if (!IsEnabledDeleteImage()) return;
            DeleteFilesNames = new List<string>();
            MediaItemPresentation image = SelectedImage;
            SelectedImage = null;
            ImageList.Remove(image);
            _ImageList = ImageList.ToList();
            OnPropertyChanged("ImageList");

            DeleteFilesNames.Add(image.FilePath);
            if (image.HaveOwnThumb)
                DeleteFilesNames.Add(image.ThumbPath);

            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DeleteFile);
            ShowDialog(this, DesignNameProgressBar);
        }



        public bool IsEnabledDeleteImage()
        {
            return SelectedImage != null && (DefaultImage == null || SelectedImage.FilePath != DefaultImage);
        }

        /// <summary>
        /// Method DeleteDocument
        /// </summary>
        [ACMethodInfo("DeleteDocument", "en{'Delete'}de{'Lösche'}", 9999, false, false, true)]
        public void DeleteDocument()
        {
            if (!IsEnabledDeleteDocument()) return;
            DeleteFilesNames = new List<string>();
            MediaItemPresentation selectedDocument = SelectedDocument;
            DocumentList.Remove(selectedDocument);
            SelectedDocument = DocumentList.FirstOrDefault();
            DocumentList = DocumentList.ToList();
            OnPropertyChanged("DocumentList");

            DeleteFilesNames.Add(selectedDocument.FilePath);
            if (selectedDocument.HaveOwnThumb)
                DeleteFilesNames.Add(selectedDocument.ThumbPath);

            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DeleteFile);
            ShowDialog(this, DesignNameProgressBar);

        }
        public bool IsEnabledDeleteDocument()
        {
            return SelectedDocument != null;
        }

        /// <summary>
        /// Method DeleteAudio
        /// </summary>
        [ACMethodInfo("DeleteAudio", "en{'Delete'}de{'Lösche'}", 9999, false, false, true)]
        public void DeleteAudio()
        {
            if (!IsEnabledDeleteAudio()) return;
            DeleteFilesNames = new List<string>();
            MediaItemPresentation selectedAudio = SelectedAudio;
            AudioList.Remove(selectedAudio);
            SelectedAudio = AudioList.FirstOrDefault();
            _AudioList = AudioList.ToList();
            OnPropertyChanged("AudioList");

            DeleteFilesNames.Add(selectedAudio.FilePath);
            if (selectedAudio.HaveOwnThumb)
                DeleteFilesNames.Add(selectedAudio.ThumbPath);

            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DeleteFile);
            ShowDialog(this, DesignNameProgressBar);
        }
        public bool IsEnabledDeleteAudio()
        {
            return SelectedAudio != null;
        }
        /// <summary>
        /// Method DeleteVideo
        /// </summary>
        [ACMethodInfo("DeleteVideo", "en{'Delete'}de{'Lösche'}", 9999, false, false, true)]
        public void DeleteVideo()
        {
            if (!IsEnabledDeleteAudio()) return;
            DeleteFilesNames = new List<string>();
            MediaItemPresentation selectedVideo = SelectedVideo;
            VideoList.Remove(selectedVideo);
            SelectedVideo = AudioList.FirstOrDefault();
            _VideoList = VideoList.ToList();
            OnPropertyChanged("VideoList");

            DeleteFilesNames.Add(selectedVideo.FilePath);
            if (selectedVideo.HaveOwnThumb)
                DeleteFilesNames.Add(selectedVideo.ThumbPath);

            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DeleteFile);
            ShowDialog(this, DesignNameProgressBar);
        }
        public bool IsEnabledDeleteVideo()
        {
            return SelectedVideo != null;
        }

        #endregion

        #region Methods -> ACMethods -> Download

        /// <summary>
        /// Method DownloadImage
        /// </summary>
        [ACMethodInfo("DownloadImage", "en{'DownloadImage'}de{'DownloadImage'}", 9999, false, false, true)]
        public void DownloadImage()
        {
            if (!IsEnabledDownloadImage()) return;
            DownloadFile(SelectedImage.FilePath);
        }

        public bool IsEnabledDownloadImage()
        {
            return SelectedImage != null;
        }


        /// <summary>
        /// Method DownloadDocument
        /// </summary>
        [ACMethodInfo("DownloadDocument", "en{'DownloadDocument'}de{'DownloadDocument'}", 9999, false, false, true)]
        public void DownloadDocument()
        {
            if (!IsEnabledDownloadDocument()) return;
            DownloadFile(SelectedDocument.FilePath);
        }

        public bool IsEnabledDownloadDocument()
        {
            return SelectedDocument != null;
        }

        #endregion

        #region Helper methods
        public MediaItemTypeEnum? GetUpladedFileType(string extension)
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
        public void DownloadFile(string filePath)
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
