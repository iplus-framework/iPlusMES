using gip.core.autocomponent;
using gip.core.ControlScriptSync;
using gip.core.datamodel;
using gip.core.reporthandler;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace gip.bso.test
{
    [ACClassInfo(Const.PackName_VarioDevelopment, "en{'BSOTest'}de{'BSOTest'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Material.ClassName)]
    public partial class BSOTest : ACBSOvb
    {
        #region constants
        public const string BGWorkerMehtod_DoTestWork = @"BGWorkerMehtod_DoTestWork";
        public const string BGWorkerMehtod_DoTestWithSubTaskNotPlanned = @"BGWorkerMehtod_DoTestWithSubTaskNotPlanned";
        public const string BGWorkerMehtod_DoTestWithSubTaskPlanned = @"BGWorkerMehtod_DoTestWithSubTaskPlanned";
        public const string BGWorkerMehtod_DoControlSync = @"BGWorkerMehtod_DoControlSync";
        #endregion

        #region c´tors

        public BSOTest(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
        }


        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            _PickingManager = ACPickingManager.ACRefToServiceInstance(this);
            if (_PickingManager == null)
                throw new Exception("PickingManager not configured");

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            bool baseReturn = base.ACDeInit(deleteACClassTask);

            if(_ACFacilityManager != null)
            {
                FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            }

            _ACFacilityManager = null;

            if (_PickingManager != null)
            {
                ACPickingManager.DetachACRefFromServiceInstance(this, _PickingManager);
            }
            _PickingManager = null;

            return baseReturn;
        }

        #endregion

        #region Managers

        protected ACRef<ACPickingManager> _PickingManager = null;
        protected ACPickingManager PickingManager
        {
            get
            {
                if (_PickingManager == null)
                    return null;
                return _PickingManager.ValueT;
            }
        }

        protected ACRef<ACComponent> _ACFacilityManager = null;
        public FacilityManager ACFacilityManager
        {
            get
            {
                if (_ACFacilityManager == null)
                    return null;
                return _ACFacilityManager.ValueT as FacilityManager;
            }
        }

        #endregion

        #region Properties

        private string _TestInput;
        [ACPropertyInfo(999, "TestInput", "en{'Test input'}de{'Test input'}")]
        public string TestInput
        {
            get
            {
                return _TestInput;
            }
            set
            {
                if (_TestInput != value)
                {
                    _TestInput = value;
                    OnPropertyChanged(TestInput);
                }
            }
        }

        private string _TestInput1;
        [ACPropertyInfo(999, "TestInput1", "en{'Test input1'}de{'Test input1'}")]
        public string TestInput1
        {
            get
            {
                return _TestInput1;
            }
            set
            {
                if (_TestInput1 != value)
                {
                    _TestInput1 = value;
                    OnPropertyChanged(TestInput1);
                }
            }
        }

        private string _TestInput2;
        [ACPropertyInfo(999, "TestInput2", "en{'Test input2'}de{'Test input2'}")]
        public string TestInput2
        {
            get
            {
                return _TestInput2;
            }
            set
            {
                if (_TestInput2 != value)
                {
                    _TestInput2 = value;
                    OnPropertyChanged(TestInput2);
                }
            }
        }

        #region Messages -> Properties

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

        private ObservableCollection<Msg> msgList;
        /// <summary>
        /// Gets the MSG list.
        /// </summary>
        /// <value>The MSG list.</value>
        [ACPropertyList(9999, "Message", "en{'Messagelist'}de{'Meldungsliste'}")]
        public ObservableCollection<Msg> MsgList
        {
            get
            {
                if (msgList == null)
                    msgList = new ObservableCollection<Msg>();
                return msgList;
            }
            set
            {
                msgList = value;
            }
        }

        private BatchSuggestionCommandModeEnum _FilterSuggestionMode;
        [ACPropertyInfo(999, "FilterSuggestionMode", "en{'Suggestion mode'}de{'Suggestion mode'}")]
        public BatchSuggestionCommandModeEnum FilterSuggestionMode
        {
            get
            {
                return _FilterSuggestionMode;
            }
            set
            {
                if (_FilterSuggestionMode != value)
                {
                    _FilterSuggestionMode = value;
                    OnPropertyChanged("FilterSuggestionMode");
                }
            }
        }

        #endregion
        #endregion

        #region Methods

        #region Methods -> BackgroudndWorker test
        [ACMethodInfo("RunTestWork", "en{'Test work'}de{'Test work'}", 9999, false, false, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void RunTestWork()
        {
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoTestWork);
            ShowDialog(this, DesignNameProgressBar);
        }

        [ACMethodInfo("RunTestWorkWithSubTask", "en{'Work with subtask (not planned)'}de{'Work with subtask (not planned)'}", 9999, false, false, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void RunTestWorkWithSubTaskNotPlanned()
        {
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoTestWithSubTaskNotPlanned);
            ShowDialog(this, DesignNameProgressBar);
        }

        [ACMethodInfo("RunTestWorkWithSubTask", "en{'Work with subtask (planned)'}de{'Work with subtask (planned)'}", 9999, false, false, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void RunTestWorkWithSubTaskPlanned()
        {
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoTestWithSubTaskPlanned);
            ShowDialog(this, DesignNameProgressBar);
        }

        [ACMethodInfo("ControlSync", "en{'ControlSync'}de{'ControlSync'}", 9999, false, false, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void ControlSync()
        {
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoControlSync);
            ShowDialog(this, DesignNameProgressBar);
        }

        #endregion

        #region Methods -> Test Methods

        [ACMethodInfo("TestMethod", "en{'TestMethod'}de{'TestMethod'}", 9999, false, false, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void TestMethod()
        {
            if (!IsEnabledTestMethod())
                return;

        }

        public bool IsEnabledTestMethod()
        {
            return true;
        }

        [ACMethodInfo("TestMethod1", "en{'ShuffleMaterial'}de{'ShuffleMaterial'}", 9999, false, false, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void TestMethod1()
        {
            if (!IsEnabledTestMethod1())
                return;
            ShuffleMaterial(false);
        }

        public bool IsEnabledTestMethod1()
        {
            return true;
        }

        [ACMethodInfo("TestMethod2", "en{'UndoShuffleMaterial'}de{'UndoShuffleMaterial'}", 9999, false, false, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void TestMethod2()
        {
            if (!IsEnabledTestMethod2())
                return;
            ShuffleMaterial(true);
        }

        public bool IsEnabledTestMethod2()
        {
            return true;
        }

        public virtual void ShuffleMaterial(bool revert)
        {
            foreach (var material in DatabaseApp.Material.AsEnumerable())
            {
                Random r = new Random();
                if (!String.IsNullOrEmpty(material.MaterialName1))
                {
                    string random = material.MaterialName3;
                    if (!revert)
                    {
                        random = new string(material.MaterialName1.ToCharArray().OrderBy(s => (r.Next(2) % 2) == 0).ToArray());
                        material.MaterialName3 = material.MaterialName1;
                    }
                    material.MaterialName1 = random;
                    foreach (var pl in DatabaseApp.Partslist.Where(c => c.MaterialID == material.MaterialID).AsEnumerable())
                    {
                        pl.PartslistName = random;
                    }
                }
            }
            DatabaseApp.SaveChanges();
        }

        [ACMethodCommand("", "en{'Count XAML-Lines'}de{'Count XAML-Lines'}", 999)]
        public void CountXAMLLines()
        {
            using (var db = new Database())
            {
                int count = 0;
                foreach (core.datamodel.ACClassDesign d in db.ACClassDesign)
                {
                    Regex RE = new Regex("\n", RegexOptions.Multiline);
                    MatchCollection theMatches = RE.Matches(d.XMLDesign);
                    count += theMatches.Count;
                }
                this.Messages.Msg(new Msg(eMsgLevel.Info, count.ToString()));
            }
        }

        #region Methods -> Tests

        public void TestACPrintManagerPrint(Guid ID)
        {
            ACComponent printManager = ACPrintManager.GetServiceInstance(this);
            if (printManager != null && printManager.ConnectionState == ACObjectConnectionState.Connected)
            {
                PAOrderInfo pAOrderInfo = new PAOrderInfo();
                pAOrderInfo.Add(FacilityCharge.ClassName, ID);
                printManager.ACUrlCommand("!Print", pAOrderInfo, 1);
            }
        }

        #endregion

        #endregion

        #endregion

        #region BackgroundWorker

        #region BackgroundWorker 

        /// <summary>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        public override void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            base.BgWorkerDoWork(sender, e);
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = e.Argument.ToString();
            MsgList = new ObservableCollection<Msg>();
            switch (command)
            {
                case BGWorkerMehtod_DoTestWork:
                    worker.ProgressInfo.OnlyTotalProgress = true;
                    worker.ProgressInfo.AddSubTask(BGWorkerMehtod_DoTestWork, 0, 9);
                    worker.ProgressInfo.ReportProgress(BGWorkerMehtod_DoTestWork, 0, string.Format("Running {0}...", command));
                    e.Result = DoTestWork(worker, e);
                    break;
                case BGWorkerMehtod_DoTestWithSubTaskNotPlanned:
                    worker.ProgressInfo.OnlyTotalProgress = false;
                    worker.ProgressInfo.ReportProgress(BGWorkerMehtod_DoTestWork, 0, string.Format("Running {0}...", command));
                    e.Result = DoTestWithSubTaskNotPlanned(worker, e);
                    break;
                case BGWorkerMehtod_DoTestWithSubTaskPlanned:
                    worker.ProgressInfo.OnlyTotalProgress = false;
                    worker.ProgressInfo.ReportProgress(BGWorkerMehtod_DoTestWork, 0, string.Format("Running {0}...", command));
                    e.Result = DoTestWithSubTaskPlanned(worker, e);
                    break;
                case BGWorkerMehtod_DoControlSync:
                    worker.ProgressInfo.OnlyTotalProgress = false;
                    worker.ProgressInfo.ReportProgress(BGWorkerMehtod_DoTestWork, 0, string.Format("Running {0}...", command));
                    DoControlSync();
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
                    case BGWorkerMehtod_DoTestWork:
                        //string[] messages1 = e.Result as string[];
                        //DoTestWorkFinish(messages1);
                        break;
                    case BGWorkerMehtod_DoTestWithSubTaskNotPlanned:
                        string[] messages2 = e.Result as string[];
                        DoTestWithSubTaskFinish(messages2);
                        break;
                    case BGWorkerMehtod_DoTestWithSubTaskPlanned:
                        string[] messages3 = e.Result as string[];
                        DoTestWithSubTaskFinish(messages3);
                        break;
                }
            }
        }
        #endregion

        #region BackgroundWorker -> BGWorker mehtods -> Methods for call
        public string[] DoTestWork(ACBackgroundWorker worker, DoWorkEventArgs e)
        {
            List<string> messages = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    return messages.ToArray();
                }
                messages.Add(string.Format(@"Step {0}...", i + 1));
                worker.ProgressInfo.ReportProgress(BGWorkerMehtod_DoTestWork, i, string.Format("Step {0} / {1} completed... ", i + 1, 10));
                Thread.Sleep(1000 * 5);
            }
            return messages.ToArray();
        }

        public string[] DoTestWithSubTaskNotPlanned(ACBackgroundWorker worker, DoWorkEventArgs e)
        {
            List<string> messages = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                messages.Add(string.Format(@"Step {0}...", i + 1));
                string subTaskName = string.Format(@"Task {0}", i + 1);
                worker.ProgressInfo.AddSubTask(subTaskName, 0, 9);
                for (int j = 0; j < 10; j++)
                {
                    if (worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        e.Result = messages.ToArray();
                        return messages.ToArray();
                    }
                    Thread.Sleep(1000 * 5 / 10);
                    worker.ProgressInfo.ReportProgress(subTaskName, j, string.Format("Step {0} / {1} completed... ", j + 1, 10));
                    OnPropertyChanged("CurrentProgressInfo\\SelectedSubTask");
                    worker.ProgressInfo.TotalProgress.ProgressText = string.Format(@"Total progress {0} / {1} ...", i + 1, 10);
                }
            }
            return messages.ToArray();
        }

        public string[] DoTestWithSubTaskPlanned(ACBackgroundWorker worker, DoWorkEventArgs e)
        {
            List<string> messages = new List<string>();
            string subTaskName = "";
            for (int i = 0; i < 10; i++)
            {
                messages.Add(string.Format(@"Step {0}...", i + 1));
                subTaskName = string.Format(@"Task {0}", i + 1);
                worker.ProgressInfo.AddSubTask(subTaskName, 0, 9);
            }

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        return messages.ToArray();
                    }
                    subTaskName = string.Format(@"Task {0}", i + 1);
                    Thread.Sleep(1000 * 5 / 10);
                    worker.ProgressInfo.ReportProgress(subTaskName, j, string.Format("Step {0} / {1} completed... ", j + 1, 10));
                    worker.ProgressInfo.TotalProgress.ProgressText = string.Format(@"Total progress {0} / {1} ...", i + 1, 10);
                }
            }
            return messages.ToArray();
        }

        public void DoControlSync()
        {
            ControlSync controlSync = new ControlSync();
            // pre-perapring Resources and Query for root - this resources is used for importing
            ACRoot.SRoot.PrepareQueriesAndResoruces();
            controlSync.OnMessage += controlSync_OnMessage;
            bool importSuccess = false;
            IResources rootResources = new Resources();
            importSuccess = controlSync.Sync(ACRoot.SRoot, Database);
        }

        void controlSync_OnMessage(SyncMessage msg)
        {
            string source = "ControlSync";
            if (!string.IsNullOrEmpty(msg.Source))
                source = msg.Source;
            if (msg.MessageLevel == MessageLevel.Error)
                Root.Messages.LogError(source, "Sync", msg.Message);
            else if (msg.MessageLevel == MessageLevel.Warning)
                Root.Messages.LogWarning(source, "Sync", msg.Message);

            gip.core.autocomponent.Messages.ConsoleMsg("ControlSync", msg.Message);
        }

        #endregion

        #region  BackgroundWorker -> BGWorker methods -> Callback methods (Finish / Completed)

        public void DoTestWorkFinish(string[] messages)
        {
            foreach (string message in messages)
                SendMessage(new Msg() { MessageLevel = eMsgLevel.Info, Message = message });
        }

        public void DoTestWithSubTaskFinish(string[] messages)
        {
            foreach (string message in messages)
                SendMessage(new Msg() { MessageLevel = eMsgLevel.Info, Message = message });
        }

        #endregion


        public void SendMessage(Msg msg)
        {
            MsgList.Add(msg);
            OnPropertyChanged("MsgList");
        }
        #endregion

        #region Mario igra

        #region Mario igra -> Methods

        /// <summary>
        /// Suggestion : use this while mail is stored on right place
        /// </summary>
        public void TestSavingUserData_EmailInCompanyAddress()
        {
            string email = "";
            string firstName = "";
            string lastName = "";
            string companyName = "";
            int rating = 0;

            Company company = null;
            CompanyAddress companyAddress = DatabaseApp.CompanyAddress.FirstOrDefault(c => c.EMail == email);
            // Company setup
            if (companyAddress == null)
            {
                company = DatabaseApp.Company.Where(c => c.CompanyName == companyName).FirstOrDefault();
                if (company == null)
                {
                    string secondaryKeyCompany = Root.NoManager.GetNewNo(Database, typeof(Company), Company.NoColumnName, Company.FormatNewNo, this);
                    company = Company.NewACObject(DatabaseApp, null, secondaryKeyCompany);
                    company.CompanyName = companyName;
                    // Setup not nullable fields varchar
                    company.BillingAccountNo = "";
                    company.ShippingAccountNo = "";
                    company.NoteInternal = "";
                    company.NoteExternal = "";
                    company.VATNumber = "";
                    companyAddress = company.CompanyAddress_Company.FirstOrDefault();
                    CompanyAddressPopulateValues(company, companyAddress);
                }

                // za drugog čovjeka iz iste firme s drugim mailom
                if (companyAddress == null)
                {
                    companyAddress = CompanyAddress.NewACObject(DatabaseApp, company);
                    CompanyAddressPopulateValues(company, companyAddress);
                }


                // CompanyAddress

                companyAddress.EMail = email;

            }

            CompanyPerson companyPerson = company.CompanyPerson_Company.Where(c => c.Name3 == email).FirstOrDefault();

            if (companyPerson == null)
            {
                // CompanyPerson setup
                string secondaryKeyCompanyPerson = Root.NoManager.GetNewNo(Database, typeof(CompanyPerson), CompanyPerson.NoColumnName, CompanyPerson.FormatNewNo, this);
                companyPerson = CompanyPerson.NewACObject(DatabaseApp, company, secondaryKeyCompanyPerson);
                company.CompanyPerson_Company.Add(companyPerson);
                companyPerson.Name3 = email;

                // Setup not nullable fields varchar
                companyPerson.Street = "";
                companyPerson.City = "";
                companyPerson.Postcode = "";
            }


            // values always updated
            companyPerson.Name1 = firstName + " " + lastName;

            companyAddress.RouteNo = rating;

            DatabaseApp.Company.AddObject(company);
            MsgWithDetails msgWithDetails = DatabaseApp.ACSaveChanges();
            bool saveSuccess = msgWithDetails.IsSucceded();
        }

        private static void CompanyAddressPopulateValues(Company company, CompanyAddress companyAddress)
        {
            companyAddress.Name1 = company.CompanyName;
            companyAddress.Name2 = "";
            //companyAddress.Name3 = "";
            companyAddress.Street = "";
            companyAddress.City = "";
            companyAddress.Postcode = "";
            companyAddress.PostOfficeBox = "";
            companyAddress.Phone = "";
            companyAddress.Fax = "";
            companyAddress.Mobile = "";
            companyAddress.EMail = "";
        }


        /// <summary>
        /// Without CompanyAddress record
        /// All stored in CompanyPerson
        /// simple solution
        /// </summary>
        public void TestSavingUserData_EmailInCompanyPerson()
        {
            string email = "";
            string firstName = "";
            string lastName = "";
            string companyName = "";
            int rating = 0;

            Company company = null;
            CompanyPerson companyPerson = company.CompanyPerson_Company.Where(c => c.Name3 == email).FirstOrDefault();
            // Company setup
            if (companyPerson == null)
            {
                company = DatabaseApp.Company.Where(c => c.CompanyName == companyName).FirstOrDefault();
                if (company == null)
                {
                    string secondaryKeyCompany = Root.NoManager.GetNewNo(Database, typeof(Company), Company.NoColumnName, Company.FormatNewNo, this);
                    company = Company.NewACObject(DatabaseApp, null, secondaryKeyCompany);
                    company.CompanyName = companyName;
                    // Setup not nullable fields varchar
                    company.BillingAccountNo = "";
                    company.ShippingAccountNo = "";
                    company.NoteInternal = "";
                    company.NoteExternal = "";
                    company.VATNumber = "";
                }

                string secondaryKeyCompanyPerson = Root.NoManager.GetNewNo(Database, typeof(CompanyPerson), CompanyPerson.NoColumnName, CompanyPerson.FormatNewNo, this);
                companyPerson = CompanyPerson.NewACObject(DatabaseApp, company, secondaryKeyCompanyPerson);
                company.CompanyPerson_Company.Add(companyPerson);
                companyPerson.Name3 = email;

            }


            // values always updated
            companyPerson.Name1 = firstName + " " + lastName;

            companyPerson.PostOfficeBox = rating.ToString();

            DatabaseApp.Company.AddObject(company);
            MsgWithDetails msgWithDetails = DatabaseApp.ACSaveChanges();
            bool saveSuccess = msgWithDetails.IsSucceded();
        }

        public void TestUsingUserData()
        {
            List<ScoreBoard> list =
                DatabaseApp
                .CompanyAddress
                .Where(c => c.GEO_x != null)
                .Select(c => new ScoreBoard()
                {
                    Name = c.Company.CompanyPerson_Company
                    .Where(x => x.Name3 == c.EMail)
                    .Select(x => x.Name1)
                    .FirstOrDefault(),
                    Score = c.GEO_x.Value
                })
                .OrderByDescending(c => c.Score)
                .ToList();
        }

        #endregion

        #region Mario igra -> ScoreBoard

        private ScoreBoard _SelectedScoreBoard;
        /// <summary>
        /// Selected property for ScoreBoard
        /// </summary>
        /// <value>The selected ScoreBoard</value>
        [ACPropertySelected(9999, "PropertyGroupName", "en{'TODO: ScoreBoard'}de{'TODO: ScoreBoard'}")]
        public ScoreBoard SelectedScoreBoard
        {
            get
            {
                return _SelectedScoreBoard;
            }
            set
            {
                if (_SelectedScoreBoard != value)
                {
                    _SelectedScoreBoard = value;
                    OnPropertyChanged(nameof(SelectedScoreBoard));
                }
            }
        }

        private List<ScoreBoard> _ScoreBoardList;
        /// <summary>
        /// List property for ScoreBoard
        /// </summary>
        /// <value>The ScoreBoard list</value>
        [ACPropertyList(9999, "PropertyGroupName")]
        public List<ScoreBoard> ScoreBoardList
        {
            get
            {
                if (_ScoreBoardList == null)
                    _ScoreBoardList = LoadScoreBoardList();
                return _ScoreBoardList;
            }
        }
        private List<ScoreBoard> LoadScoreBoardList()
        {
            return
              DatabaseApp
              .CompanyAddress
              .Where(c => c.GEO_x != null)
              .Select(c => new ScoreBoard()
              {
                  Name = c.Company.CompanyPerson_Company
                  .Where(x => x.Name3 == c.EMail)
                  .Select(x => x.Name1)
                  .FirstOrDefault(),
                  Score = c.GEO_x.Value
              })
              .OrderByDescending(c => c.Score)
              .ToList();

        }
        #endregion

        #endregion

      
    }

    public class ScoreBoard
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }
}
