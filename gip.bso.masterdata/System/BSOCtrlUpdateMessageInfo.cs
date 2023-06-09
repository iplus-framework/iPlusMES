using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace gip.bso.masterdata
{
    /// <summary>
    /// BSO used for showing log presented by CTRL load
    /// After load and Masterpage opening this log is not more present
    /// Now is possible to show this log and extract it to Clipboard or store to local .txt file
    /// </summary>
    [ACClassInfo(Const.PackName_VarioDevelopment, "en{'Update info'}de{'Update Info'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + "Msg")]
    [ACClassConstructorInfo(
        new object[] 
        { 
            new object[] {"DataList", Global.ParamOption.Optional, typeof(List<Msg>) }
        }
    )]
    public class BSOCtrlUpdateMessageInfo : ACBSOvbNav
    {

        #region c´tors
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOCtrlUpdateMessageInfo(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            if (parameter != null && parameter["DataList"] != null)
            {
                MsgList = parameter["DataList"] as List<Msg>;
            }
            //else
            //{
            //    Msg testMsg1 = new Msg();
            //    testMsg1.Message = "Ovo je test1";
            //    testMsg1.MessageLevel = eMsgLevel.Info;
            //    testMsg1.TimeStampOccurred = DateTime.Now;
            //    testMsg1.ACIdentifier = "BSOTest";

            //    Msg testMsg2 = new Msg();
            //    testMsg2.Message = "Ovo je test2";
            //    testMsg2.MessageLevel = eMsgLevel.Info;
            //    testMsg2.TimeStampOccurred = DateTime.Now;
            //    testMsg2.ACIdentifier = "BSOTest";

            //    MsgList.Add(testMsg1);
            //    MsgList.Add(testMsg2);
            //}
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
            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            this._ExportFolder = null;
            this._msgList = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }
        #endregion

        #region Msg -> Access Nav


        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<Msg> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(690, "Msg")]
        public ACAccessNav<Msg> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<Msg>("Msg", this);
                }
                return _AccessPrimary;
            }
        }

        #endregion

        #region Msg -> Select, (Current,) List

        /// <summary>
        /// Gets or sets the selected production order.
        /// </summary>
        /// <value>The selected production order.</value>
        [ACPropertySelected(9999, "Msg")]
        public Msg SelectedMsg
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary.Selected != value)
                {
                    if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                    CurrentMsg = value;
                    OnPropertyChanged("SelectedMsg");
                }
            }
        }

        /// <summary>
        /// Gets or sets the current production order.
        /// </summary>
        /// <value>The current production order.</value>
        [ACPropertyCurrent(9999, "Msg")]
        public Msg CurrentMsg
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary.Current != value)
                {
                    if (AccessPrimary == null) return; AccessPrimary.Current = value;
                    if (SelectedMsg != value)
                    {
                        SelectedMsg = value;
                    }
                    OnPropertyChanged("CurrentMsg");
                }
            }
        }

        private List<Msg> _msgList;
        /// <summary>
        /// Gets the production order list.
        /// </summary>
        /// <value>The production order list.</value>
        [ACPropertyList(9999, "Msg")]
        public List<Msg> MsgList
        {
            get
            {
                if (_msgList == null)
                    _msgList = new List<Msg>();
                return _msgList;
            }
            set
            {
                _msgList = value;
            }
        }

        /// <summary>
        /// List of strings - Msg to string in single row
        /// </summary>
        public List<string> DataStringList
        {
            get
            {
                List<string> returnList = new List<string>();
                foreach(var item in MsgList)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(item.MessageLevel);
                    sb.Append("\t");
                    sb.Append(item.TimeStampOccurred);
                    sb.Append("\t");
                    sb.Append((item.ACIdentifier ?? "").Trim());
                    sb.Append("\t");
                    sb.Append((item.Message ?? "").Trim());
                    sb.Append("\t");
                    sb.Append((item.Source ?? "").Trim());
                    returnList.Add(sb.ToString());
                }
                return returnList;
            }
        }

        private string _ExportFolder = @"D:\VarioData";

        [ACPropertyInfo(999, "Msg", "en{'Export folder'}de{'Export-Ordner'}")]
        public string ExportFolder
        {
            get
            {
                return _ExportFolder;
            }
            set
            {
                _ExportFolder = value;
            }
        }
        #endregion

        #region Msg -> Methods

        /// <summary>
        /// 
        /// </summary>
        [ACMethodCommand("Msg", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        private void Search()
        {
            if(MsgList.Any())
            {
                SelectedMsg = MsgList.First();
            }
            else
            {
                SelectedMsg = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [ACMethodCommand("Msg", "en{'Copy all to clipboard'}de{'Kopieren to Zwischenablage'}", (short)MISort.Load, false, Global.ACKinds.MSMethodPrePost)]
        public void CopyAllToClipboard()
        {
            if (!PreExecute("CopyAllToClipboard")) return;
            Clipboard.SetText(string.Join(System.Environment.NewLine, DataStringList));
            PostExecute("CopyAllToClipboard");
        }

        /// <summary>
        /// 
        /// </summary>
        [ACMethodCommand("Msg", "en{'Save to file'}de{'Speichern to Datei'}", (short)MISort.Load, false, Global.ACKinds.MSMethodPrePost)]
        public void SaveAllToTxtFile()
        {
            if (!PreExecute("SaveAllToTxtFile")) return;
            if(Directory.Exists(ExportFolder))
            {
                string exportString = string.Join(System.Environment.NewLine, DataStringList);
                string exportFile = ExportFolder.TrimEnd('\\') + @"\" + "CtrlLoadMessage.txt";
                File.WriteAllText(exportFile, exportString);
            }
            PostExecute("SaveAllToTxtFile");
        }


        #region Msg -> Methods -> IsEnabled

        public bool IsEnabledCopyAllToClipboard()
        {
            return MsgList.Any();
        }

        public bool IsEnabledSaveAllToTxtFile()
        {
            return IsEnabledCopyAllToClipboard() ;
        }

        #endregion

        #endregion

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(CopyAllToClipboard):
                    CopyAllToClipboard();
                    return true;
                case nameof(SaveAllToTxtFile):
                    SaveAllToTxtFile();
                    return true;
                case nameof(IsEnabledCopyAllToClipboard):
                    result = IsEnabledCopyAllToClipboard();
                    return true;
                case nameof(IsEnabledSaveAllToTxtFile):
                    result = IsEnabledSaveAllToTxtFile();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}