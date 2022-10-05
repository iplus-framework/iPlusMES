using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using gip.core.reporthandler;
using gip.mes.facility;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace gip.bso.test
{
    [ACClassInfo(Const.PackName_VarioDevelopment, "en{'Material'}de{'Material'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Material.ClassName)]
    public class BSOTest : ACBSOvb
    {
        #region constants
        public const string BGWorkerMehtod_DoTestWork = @"BGWorkerMehtod_DoTestWork";
        public const string BGWorkerMehtod_DoTestWithSubTaskNotPlanned = @"BGWorkerMehtod_DoTestWithSubTaskNotPlanned";
        public const string BGWorkerMehtod_DoTestWithSubTaskPlanned = @"BGWorkerMehtod_DoTestWithSubTaskPlanned";
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
            return true;
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

        [ACMethodInfo("TestMethod", "en{'Test'}de{'Test'}", 9999, false, false, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void TestMethod()
        {
            Guid ID = Guid.Empty;
            if (!string.IsNullOrEmpty(TestInput) && Guid.TryParse(TestInput, out ID))
            {
                ACComponent printManager = ACPrintManager.GetServiceInstance(this);
                if (printManager != null && printManager.ConnectionState == ACObjectConnectionState.Connected)
                {
                    PAOrderInfo pAOrderInfo = new PAOrderInfo();
                    pAOrderInfo.Add(FacilityCharge.ClassName, ID);
                    printManager.ACUrlCommand("!Print", pAOrderInfo, 1);
                }
            }
        }
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
                        string[] messages1 = e.Result as string[];
                        DoTestWorkFinish(messages1);
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


        #region Za igru Mario

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


        #endregion

    }
}
