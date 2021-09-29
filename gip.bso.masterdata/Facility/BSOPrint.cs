using gip.bso.iplus;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using static System.Drawing.Printing.PrinterSettings;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Storage Location'}de{'Lagerplatz'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public class BSOPrint : ACBSOvb
    {
        #region const
        public const string ClassName = @"BSOPrint";
        public const string BGWorkerMehtod_LoadMachinesAndPrinters = "LoadMachinesAndPrinters";
        #endregion

        #region c'tors

        /// <summary>
        /// Creates a new instance of the BSOPropertyLogRules.
        /// </summary>
        /// <param name="acType">The acType parameter.</param>
        /// <param name="content">The content parameter.</param>
        /// <param name="parentACObject">The parentACObject parameter.</param>
        /// <param name="parameter">The parameters in the ACValueList.</param>
        /// <param name="acIdentifier">The acIdentifier parameter.</param>
        public BSOPrint(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
            // this is test
        }

        /// <summary>
        /// Initializes this component.
        /// </summary>
        /// <param name="startChildMode">The start child mode parameter.</param>
        /// <returns>True if is initialization success, otherwise returns false.</returns>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool baseInit = base.ACInit(startChildMode);

            _PrintManager = ACPrintManager.ACRefToServiceInstance(this);
            if (_PrintManager == null)
                throw new Exception("ACPrintManager not configured");

            CurrentFacilityRoot = FacilityTree.LoadFacilityTree(DatabaseApp);
            CurrentFacility = CurrentFacilityRoot;

            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_LoadMachinesAndPrinters);
            ShowDialog(this, DesignNameProgressBar);

            return baseInit;
        }

        /// <summary>
        ///  Deinitializes this component.
        /// </summary>
        /// <param name="deleteACClassTask">The deleteACClassTask parameter.</param>
        /// <returns>True if is deinitialization success, otherwise returns false.</returns>
        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            bool done = base.ACDeInit(deleteACClassTask);
            if (_PrintManager != null)
                ACPrintManager.DetachACRefFromServiceInstance(this, _PrintManager);
            _PrintManager = null;
            return done;
        }

        #endregion

        #region Managers

        protected ACRef<ACPrintManager> _PrintManager = null;
        protected ACPrintManager PrintManager
        {
            get
            {
                if (_PrintManager == null)
                    return null;
                return _PrintManager.ValueT;
            }
        }

        #endregion

        #region Properties

        protected WorkerResult BSOPrinterWorkerResult { get; set; }

        #region Properties -> Messages

        public void SendMessage(object result)
        {
            Msg msg = result as Msg;
            if (msg != null)
            {
                SendMessage(msg);
            }
        }

        /// <summary>
        /// The _ current MSG
        /// </summary>
        Msg _CurrentMsg;
        /// <summary>
        /// Gets or sets the current MSG.
        /// </summary>
        /// <value>The current MSG.</value>
        [ACPropertyCurrent(528, "Message", "en{'Message'}de{'Meldung'}")]
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
        [ACPropertyList(529, "Message", "en{'Messagelist'}de{'Meldungsliste'}")]
        public ObservableCollection<Msg> MsgList
        {
            get
            {
                if (msgList == null)
                    msgList = new ObservableCollection<Msg>();
                return msgList;
            }
        }

        public void SendMessage(Msg msg)
        {
            MsgList.Add(msg);
            OnPropertyChanged("MsgList");
        }

        public void ClearMessages()
        {
            MsgList.Clear();
            OnPropertyChanged("MsgList");
        }
        #endregion

        #region Properties -> Project Manager

        ACProjectManager _ACProjectManager;
        public ACProjectManager ProjectManager
        {
            get
            {
                if (_ACProjectManager != null)
                    return _ACProjectManager;
                _ACProjectManager = new ACProjectManager(DatabaseApp.ContextIPlus, Root);
                return _ACProjectManager;
            }
        }

        #endregion

        #region Properties -> FacilityTree

        ACFSItem _CurrentFacilityRoot;
        ACFSItem _CurrentFacility;


        /// <summary>
        /// Gets or sets the current import project item root.
        /// </summary>
        /// <value>The current import project item root.</value>
        [ACPropertyCurrent(9999, "FacilityRoot")]
        public ACFSItem CurrentFacilityRoot
        {
            get
            {
                return _CurrentFacilityRoot;
            }
            set
            {
                _CurrentFacilityRoot = value;
                OnPropertyChanged("CurrentFacilityRoot");
            }

        }

        /// <summary>
        /// Gets or sets the current import project item.
        /// </summary>
        /// <value>The current import project item.</value>
        [ACPropertyCurrent(9999, "Facility")]
        public ACFSItem CurrentFacility
        {
            get
            {
                return _CurrentFacility;
            }
            set
            {
                if (_CurrentFacility != value)
                {
                    if (_CurrentFacility != null && _CurrentFacility.ACObject != null)
                        (_CurrentFacility.ACObject as INotifyPropertyChanged).PropertyChanged -= _CurrentFacility_PropertyChanged;
                    _CurrentFacility = value;
                    if (_CurrentFacility != null && _CurrentFacility.ACObject != null)
                        (_CurrentFacility.ACObject as INotifyPropertyChanged).PropertyChanged += _CurrentFacility_PropertyChanged;
                    if (value != null && value.Value != null)
                    {
                        SelectedMachine = null;
                        LocationName = (value.Value as Facility).FacilityNo;
                    }
                    OnPropertyChanged("CurrentFacility");
                    OnPropertyChanged("SelectedFacility");
                }
            }
        }

        private void _CurrentFacility_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FacilityNo" || e.PropertyName == "FacilityName")
            {
                ACFSItem current = CurrentFacility;
                current.ACCaption = FacilityTree.FacilityACCaption(CurrentFacility.ACObject as Facility);
                CurrentFacilityRoot = FacilityTree.GetNewRootFacilityACFSItem(Database as gip.core.datamodel.Database, CurrentFacilityRoot.Items);
                CurrentFacility = current;
            }
        }

        [ACPropertyInfo(9999, "SelectedFacility")]
        public Facility SelectedFacility
        {
            get
            {
                if (CurrentFacility != null && CurrentFacility.ACObject != null)
                    return CurrentFacility.ACObject as Facility;
                return null;
            }
        }

        #endregion

        #region Properties -> Machines

        private ACComponent _SelectedMachine;
        /// <summary>
        /// Selected property for ACComponent
        /// </summary>
        /// <value>The selected Machine</value>
        [ACPropertySelected(9999, "Machine", "en{'TODO: Machine'}de{'TODO: Machine'}")]
        public ACComponent SelectedMachine
        {
            get
            {
                return _SelectedMachine;
            }
            set
            {
                if (_SelectedMachine != value)
                {
                    _SelectedMachine = value;
                    if (value != null)
                    {
                        CurrentFacility = null;
                        LocationName = SelectedMachine.ACCaption;
                    }
                    OnPropertyChanged("SelectedMachine");
                }
            }
        }


        private List<ACComponent> _MachineList;
        /// <summary>
        /// List property for ACComponent
        /// </summary>
        /// <value>The Machine list</value>
        [ACPropertyList(9999, "Machine")]
        public List<ACComponent> MachineList
        {
            get
            {
                if (_MachineList == null)
                    _MachineList = new List<ACComponent>();
                return _MachineList;
            }
        }

        #endregion

        #region LocationName
        private string _LocationName;
        /// <summary>
        /// Selected property for 
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "LocationName", "en{'Selected location'}de{'Ausgewählter Standort'}")]
        public string LocationName
        {
            get
            {
                return _LocationName;
            }
            set
            {
                if (_LocationName != value)
                {
                    _LocationName = value;
                    OnPropertyChanged("LocationName");
                }
            }
        }

        #endregion

        #region Properties -> ConfiguredPrinter

        private PrinterInfo _SelectedConfiguredPrinter;
        /// <summary>
        /// Selected property for PrinterInfo
        /// </summary>
        /// <value>The selected ConfiguredPrinter</value>
        [ACPropertySelected(9999, "ConfiguredPrinter", "en{'TODO: ConfiguredPrinter'}de{'TODO: ConfiguredPrinter'}")]
        public PrinterInfo SelectedConfiguredPrinter
        {
            get
            {
                return _SelectedConfiguredPrinter;
            }
            set
            {
                if (_SelectedConfiguredPrinter != value)
                {
                    _SelectedConfiguredPrinter = value;
                    OnPropertyChanged("SelectedConfiguredPrinter");
                }
            }
        }

        private ObservableCollection<PrinterInfo> _ConfiguredPrinterList;
        /// <summary>
        /// List property for PrinterInfo
        /// </summary>
        /// <value>The ConfiguredPrinters list</value>
        [ACPropertyList(9999, "ConfiguredPrinter")]
        public ObservableCollection<PrinterInfo> ConfiguredPrinterList
        {
            get
            {
                if (_ConfiguredPrinterList == null)
                {
                    if (string.IsNullOrEmpty(PrintManager.ConfiguredPrinters))
                        _ConfiguredPrinterList = new ObservableCollection<PrinterInfo>();
                    else
                    {
                        PrinterInfo[] tmpList = JsonConvert.DeserializeObject<PrinterInfo[]>(PrintManager.ConfiguredPrinters);
                        _ConfiguredPrinterList = new ObservableCollection<PrinterInfo>(tmpList);
                    }
                    _ConfiguredPrinterList.CollectionChanged += _ConfiguredPrinterList_CollectionChanged;
                }
                return _ConfiguredPrinterList;
            }
        }

        private void _ConfiguredPrinterList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_ConfiguredPrinterList.Any())
                PrintManager.ConfiguredPrinters = JsonConvert.SerializeObject(_ConfiguredPrinterList.ToArray());
            else
                PrintManager.ConfiguredPrinters = null;
        }

        #endregion

        #region Properties -> Windows printers

        private PrinterInfo _SelectedWindowsPrinter;
        /// <summary>
        /// Selected property for PrinterInfo
        /// </summary>
        /// <value>The selected WindowsPrinter</value>
        [ACPropertySelected(9999, "WindowsPrinter", "en{'TODO: WindowsPrinter'}de{'TODO: WindowsPrinter'}")]
        public PrinterInfo SelectedWindowsPrinter
        {
            get
            {
                return _SelectedWindowsPrinter;
            }
            set
            {
                if (_SelectedWindowsPrinter != value)
                {
                    _SelectedWindowsPrinter = value;
                    if (value != null)
                    {
                        SelectedPrinterComponent = null;
                        PrinterName = value.PrinterName;
                    }
                    OnPropertyChanged("SelectedWindowsPrinter");
                }
            }
        }

        private List<PrinterInfo> _WindowsPrinterList;
        /// <summary>
        /// List property for PrinterInfo
        /// </summary>
        /// <value>The WindowsPrinter list</value>
        [ACPropertyList(9999, "WindowsPrinter")]
        public List<PrinterInfo> WindowsPrinterList
        {
            get
            {
                if (_WindowsPrinterList == null)
                    _WindowsPrinterList = LoadWindowsPrinterList();
                return _WindowsPrinterList;
            }
        }

        private List<PrinterInfo> LoadWindowsPrinterList()
        {
            string[] configuredPrinters = ConfiguredPrinterList.Select(c => c.PrinterName).ToArray();
            StringCollection windowsPrinters = PrinterSettings.InstalledPrinters;
            List<string> windowsPrintersList = new List<string>();
            foreach (string printer in windowsPrinters)
            {
                windowsPrintersList.Add(printer);
            }
            return
                windowsPrintersList
                .Where(c => !configuredPrinters.Contains(c))
                .Select(c => new PrinterInfo() { PrinterName = c })
                .ToList();
        }

        #endregion

        #region Properties ->  PrinterComponent

        private PrinterInfo _SelectedPrinterComponent;
        /// <summary>
        /// Selected property for PrinterInfo
        /// </summary>
        /// <value>The selected ESCPosPrinter</value>
        [ACPropertySelected(9999, "PrinterComponent", "en{'TODO: ESCPosPrinter'}de{'TODO: ESCPosPrinter'}")]
        public PrinterInfo SelectedPrinterComponent
        {
            get
            {
                return _SelectedPrinterComponent;
            }
            set
            {
                if (_SelectedPrinterComponent != value)
                {
                    _SelectedPrinterComponent = value;
                    if (value != null)
                    {
                        SelectedWindowsPrinter = null;
                        PrinterName = value.PrinterACUrl;
                    }
                    OnPropertyChanged("SelectedPrinterComponent");
                }
            }
        }


        private List<PrinterInfo> _PrinterComponentList;
        /// <summary>
        /// List property for PrinterInfo
        /// </summary>
        /// <value>The ESCPosPrinter list</value>
        [ACPropertyList(9999, "PrinterComponent")]
        public List<PrinterInfo> PrinterComponentList
        {
            get
            {
                if (_PrinterComponentList == null)
                    _PrinterComponentList = new List<PrinterInfo>();
                return _PrinterComponentList;
            }
        }

        #endregion

        #region Properties -> PrinterName


        #region PrinterName
        private string _PrinterName;
        /// <summary>
        /// Selected property for 
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "PrinterName", "en{'Selected printer'}de{'Ausgewählte Drucker'}")]
        public string PrinterName
        {
            get
            {
                return _PrinterName;
            }
            set
            {
                if (_PrinterName != value)
                {
                    _PrinterName = value;
                    OnPropertyChanged("PrinterName");
                }
            }
        }

        #endregion

        #endregion

        #endregion

        #region Methods

        [ACMethodInfo(BSOPrint.ClassName, "en{'Remove printer'}de{'Drucker entfernen'}", 9999)]

        public void RemovePrinter()
        {
            if (!IsEnabledRemovePrinter())
                return;
            bool isWindowsPrinter = !string.IsNullOrEmpty(SelectedConfiguredPrinter.PrinterName);
            ConfiguredPrinterList.Remove(SelectedConfiguredPrinter);
            OnPropertyChanged("ConfiguredPrinterList");
        }

        public bool IsEnabledRemovePrinter()
        {
            return SelectedConfiguredPrinter != null;
        }

        [ACMethodInfo(BSOPrint.ClassName, "en{'Add printer'}de{'Drucker hinzufügen'}", 9999)]
        public void AddPrinter()
        {
            if (!IsEnabledAddPrinter())
                return;
            if (SelectedWindowsPrinter != null)
            {
                if (CurrentFacility != null)
                    SelectedWindowsPrinter.FacilityNo = (CurrentFacility.Value as Facility).FacilityNo;
                else if (SelectedMachine != null)
                    SelectedWindowsPrinter.MachineACUrl = SelectedMachine.ACUrl;

                ConfiguredPrinterList.Add(SelectedWindowsPrinter);
                OnPropertyChanged("ConfiguredPrinterList");
            }
            else if (SelectedPrinterComponent != null)
            {
                if (CurrentFacility != null)
                    SelectedPrinterComponent.FacilityNo = (CurrentFacility.Value as Facility).FacilityNo;
                else if (SelectedMachine != null)
                    SelectedPrinterComponent.MachineACUrl = SelectedMachine.ACUrl;

                ConfiguredPrinterList.Add(SelectedPrinterComponent);
                OnPropertyChanged("ConfiguredPrinterList");
            }
        }

        public bool IsEnabledAddPrinter()
        {
            return
                ((CurrentFacility != null && CurrentFacility.Value != null) || SelectedMachine != null)
                && (SelectedWindowsPrinter != null || SelectedPrinterComponent != null)
                && !ConfiguredPrinterList.Any(c =>
                    (
                        (SelectedWindowsPrinter != null && c.PrinterName == SelectedWindowsPrinter.PrinterName)
                        || (SelectedPrinterComponent != null && c.PrinterACUrl == SelectedPrinterComponent.PrinterACUrl)
                    )
                    && ((SelectedMachine != null && SelectedMachine.ACUrl == c.MachineACUrl) || (CurrentFacility != null && c.FacilityNo == (CurrentFacility.Value as Facility).FacilityNo))
                );
        }

        #endregion

        #region Background worker
        /// <summary>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        public override void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            base.BgWorkerDoWork(sender, e);
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = e.Argument.ToString();

            worker.ProgressInfo.OnlyTotalProgress = true;
            worker.ProgressInfo.AddSubTask(command, 0, 9);
            string message = Translator.GetTranslation("en{'Running {0}...'}de{'{0} läuft...'}");
            worker.ProgressInfo.ReportProgress(command, 0, string.Format(message, command));

            switch (command)
            {
                case BGWorkerMehtod_LoadMachinesAndPrinters:
                    e.Result = DoLoadMachinesAndPrinters(worker, Database.ContextIPlus, ProjectManager);
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
            if (e.Error != null)
            {
                SendMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = string.Format(@"Error by doing {0}! Message:{1}", command, e.Error.Message) });
            }
            else
            {
                BSOPrinterWorkerResult = (WorkerResult)e.Result;
                _PrinterComponentList = LoadPrinterComponentList();
                _MachineList = LoadMachineList();
                OnPropertyChanged("PrinterComponentList");
                OnPropertyChanged("MachineList");
            }
        }

        private WorkerResult DoLoadMachinesAndPrinters(ACBackgroundWorker worker, Database database, ACProjectManager projectManager)
        {
            WorkerResult workerResult = new WorkerResult();
            workerResult.Printers = new List<IACComponent>();
            workerResult.Machines = new List<IACComponent>();


            List<gip.core.datamodel.ACProject> projects =
                database
                .ACProject
                .Where(c => c.ACProjectTypeIndex == (short)Global.ACProjectTypes.Application).ToList();

            foreach (gip.core.datamodel.ACProject project in projects)
            {
                worker.ProgressInfo.TotalProgress.ProgressText = string.Format(@"Loading machines and printers for project {0} ...", project.ACProjectName);
                gip.core.datamodel.ACProject acProject = projectManager.LoadACProject(project);
                ACComponent rootACComponent = Root.FindChildComponents(project.RootClass, 1).Select(c => c as ACComponent).FirstOrDefault();

                List<ACComponent> machines = rootACComponent.FindChildComponents<ACComponent>(c => c is PAClassPhysicalBase);
                List<ACComponent> printers = rootACComponent.FindChildComponents<ACComponent>(c => c is ACPrintServerBase);

                if (machines != null)
                    workerResult.Machines.AddRange(machines);
                if (printers != null)
                    workerResult.Printers.AddRange(printers);
            }
            return workerResult;
        }

        private List<PrinterInfo> LoadPrinterComponentList()
        {
            return
                BSOPrinterWorkerResult
                .Printers
                //.Where(c => !ConfiguredPrinterList.Select(x => x.PrinterACUrl).Contains(c.ACUrl))
                .Select(c =>
                new PrinterInfo()
                {
                    PrinterACUrl = c.ACUrl
                })
                .ToList();
        }

        private List<ACComponent> LoadMachineList()
        {
            return BSOPrinterWorkerResult
                .Machines
                //.Where(c => !ConfiguredPrinterList.Select(x => x.MachineACUrl).Contains(c.ACUrl))
                .Select(c => c as ACComponent)
                .ToList();
        }

        protected struct WorkerResult
        {
            public List<IACComponent> Printers { get; set; }
            public List<IACComponent> Machines { get; set; }
        }
        #endregion

    }
}
