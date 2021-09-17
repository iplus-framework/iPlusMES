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
            _PrintManager = ACPrintManager.ACRefToServiceInstance(this);
            if (_PrintManager == null)
                throw new Exception("ACPrintManager not configured");

            CurrentFacilityRoot = FacilityTree.LoadFacilityTree(DatabaseApp);
            CurrentFacility = CurrentFacilityRoot;

            return base.ACInit(startChildMode);
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


        #region Machine
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
                    _MachineList = LoadMachineList();
                return _MachineList;
            }
        }

        private List<ACComponent> LoadMachineList()
        {
            return new List<ACComponent>();
        }
        #endregion

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
                        SelectedESCPosPrinter = null;
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

        #region Properties -> ESCPos printers

        private PrinterInfo _SelectedESCPosPrinter;
        /// <summary>
        /// Selected property for PrinterInfo
        /// </summary>
        /// <value>The selected ESCPosPrinter</value>
        [ACPropertySelected(9999, "ESCPosPrinter", "en{'TODO: ESCPosPrinter'}de{'TODO: ESCPosPrinter'}")]
        public PrinterInfo SelectedESCPosPrinter
        {
            get
            {
                return _SelectedESCPosPrinter;
            }
            set
            {
                if (_SelectedESCPosPrinter != value)
                {
                    _SelectedESCPosPrinter = value;
                    if (value != null)
                    {
                        SelectedWindowsPrinter = null;
                        PrinterName = value.PrinterACUrl;
                    }
                    OnPropertyChanged("SelectedESCPosPrinter");
                }
            }
        }


        private List<PrinterInfo> _ESCPosPrinterList;
        /// <summary>
        /// List property for PrinterInfo
        /// </summary>
        /// <value>The ESCPosPrinter list</value>
        [ACPropertyList(9999, "ESCPosPrinter")]
        public List<PrinterInfo> ESCPosPrinterList
        {
            get
            {
                if (_ESCPosPrinterList == null)
                    _ESCPosPrinterList = LoadESCPosPrinterList();
                return _ESCPosPrinterList;
            }
        }

        private List<PrinterInfo> LoadESCPosPrinterList()
        {
            return new List<PrinterInfo>();
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
            if (isWindowsPrinter)
            {
                _WindowsPrinterList = LoadWindowsPrinterList();
                OnPropertyChanged("WindowsPrinterList");
            }
            else
            {
                _ESCPosPrinterList = LoadESCPosPrinterList();
                OnPropertyChanged("ESCPosPrinterList");
            }
            SelectedConfiguredPrinter = ConfiguredPrinterList.FirstOrDefault();
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
                WindowsPrinterList.Remove(SelectedWindowsPrinter);
                OnPropertyChanged("ConfiguredPrinterList");
                OnPropertyChanged("WindowsPrinterList");

                SelectedWindowsPrinter = WindowsPrinterList.FirstOrDefault();
            }
            else if (SelectedESCPosPrinter != null)
            {
                if (CurrentFacility != null)
                    SelectedESCPosPrinter.FacilityNo = (CurrentFacility.Value as Facility).FacilityNo;
                else if (SelectedMachine != null)
                    SelectedESCPosPrinter.MachineACUrl = SelectedMachine.ACUrl;

                ConfiguredPrinterList.Add(SelectedESCPosPrinter);
                ESCPosPrinterList.Remove(SelectedESCPosPrinter);
                OnPropertyChanged("ConfiguredPrinterList");
                OnPropertyChanged("ESCPosPrinterList");

                SelectedESCPosPrinter = ESCPosPrinterList.FirstOrDefault();
            }
        }

        public bool IsEnabledAddPrinter()
        {
            return
                ((CurrentFacility != null && CurrentFacility.Value != null) || SelectedMachine != null)
                && (SelectedWindowsPrinter != null || SelectedESCPosPrinter != null)
                && !ConfiguredPrinterList.Any(c =>
                    (
                        (SelectedWindowsPrinter != null && c.PrinterName == SelectedWindowsPrinter.PrinterName)
                        || (SelectedESCPosPrinter != null && c.PrinterACUrl == SelectedESCPosPrinter.PrinterACUrl)
                    )
                    && ((SelectedMachine != null && SelectedMachine.ACUrl == c.MachineACUrl) || (CurrentFacility != null && c.FacilityNo == (CurrentFacility.Value as Facility).FacilityNo))
                );
        }

        #endregion

    }
}
