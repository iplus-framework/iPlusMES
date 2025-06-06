﻿// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.manager;
using gip.mes.autocomponent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using gip.mes.datamodel;
using System.Threading;
using gip.mes.processapplication;
using Microsoft.EntityFrameworkCore;
using gip.bso.iplus;
using System.Timers;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Work center'}de{'Arbeitsplatz'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    [ACQueryInfo(Const.PackName_VarioManufacturing, Const.QueryPrefix + nameof(BSOWorkCenterSelector), "en{'Work center'}de{'Arbeitsplatz'}", typeof(WorkCenterItem), nameof(BSOWorkCenterSelector), "", "")]
    public class BSOWorkCenterSelector : ACBSOvbNav
    {
        #region c'tors

        public BSOWorkCenterSelector(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _CN_BSOWCSNavigation = new ACPropertyConfigValue<string>(this, "CN_BSOWCSNavigation", "");
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool result = base.ACInit(startChildMode);
            if (!result)
                return result;

            _MainSyncContext = SynchronizationContext.Current;
            _BSOWorkCenterSelectorRules = new ACPropertyConfigValue<string>(this, nameof(BSOWorkCenterSelectorRules), "");
            _OnOpenSetLastSelectedWorkCenterItem = new ACPropertyConfigValue<bool>(this, nameof(OnOpenSetLastSelectedWorkCenterItem), true);

            using (ACMonitor.Lock(_20015_LockValue))
            {
                _ApplicationQueue = new ACDelegateQueue(this.GetACUrl() + ";AppQueue");
                _ApplicationQueue.StartWorkerThread();
            }

            _timer = new System.Timers.Timer();
            _timer.Interval = 1000;
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Tick);
            _timer.Start();

            return result;
        }

        public override bool ACPostInit()
        {
            BuildWorkCenterItems();

            SelectWorkCenterItem();

            Communications wcfManager = ACRoot.SRoot.GetChildComponent("Communications") as Communications;
            if (wcfManager != null && wcfManager.WCFClientManager != null)
            {
                _ClientManager = wcfManager.WCFClientManager;
                _ClientManager.PropertyChanged += _ClientManager_PropertyChanged;
            }

            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_ClientManager != null)
            {
                _ClientManager.PropertyChanged -= _ClientManager_PropertyChanged;
                _ClientManager = null;
            }

            if (TaskPresenter != null)
            {
                TaskPresenter.Unload();
            }
            _TaskPresenter = null;

            if (ProcessModuleOrderInfo != null)
                ProcessModuleOrderInfo.PropertyChanged -= ProcessModuleOrderInfo_PropertyChanged;

            ProcessModuleOrderInfo = null;

            using (ACMonitor.Lock(_70050_MembersLock))
            {
                if (_AlarmsInPhysicalModel != null)
                {
                    _AlarmsInPhysicalModel.PropertyChanged -= _AlarmsInPhysicalModel_PropertyChanged;
                    _AlarmsInPhysicalModel = null;
                }

                if (_CurrentPWGroup != null)
                {
                    _CurrentPWGroup.Detach();
                    _CurrentPWGroup = null;
                }
            }

            foreach (WorkCenterItem item in WorkCenterItems)
            {
                item.DeInit();
            }

            foreach (BSOWorkCenterChild childBSO in FindChildComponents<BSOWorkCenterChild>(1))
            {
                childBSO.DeActivate();
            }

            DeinitFunctionMonitor();

            if (_ApplicationQueue != null)
            {
                _ApplicationQueue.StopWorkerThread(true);
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _ApplicationQueue = null;
                }
            }

            if (_RoutingService != null)
            {
                _RoutingService.Detach();
                _RoutingService = null;
            }

            _MainSyncContext = null;

            if (_timer != null)
            {
                _timer.Elapsed -= _timer_Tick;
                _timer.Stop();
                _timer = null;
            }

            return base.ACDeInit(deleteACClassTask);
        }

        public const string FunctionMonitorTabVBContent = "FuncMonitor";

        #endregion

        #region Properties

        private ACMonitorObject _70050_MembersLock = new ACMonitorObject(70050);

        private ACDelegateQueue _ApplicationQueue;
        public ACDelegateQueue ApplicationQueue
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _ApplicationQueue;
                }
            }
        }

        private WCFClientManager _ClientManager;
        private SynchronizationContext _MainSyncContext;

        private bool _IsCurrentUserConfigured = false;
        public bool IsCurrentUserConfigured => _IsCurrentUserConfigured;

        #region Properties => AccessNav

        public override IAccessNav AccessNav { get { return AccessPrimary; } }

        ACAccessNav<WorkCenterItem> _AccessPrimary;

        [ACPropertyAccessPrimary(9999, "WorkCenterItem")]
        public virtual ACAccessNav<WorkCenterItem> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(this, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<WorkCenterItem>("WorkCenterItem", this);

                    //_AccessPrimary.ToNavList(GetRelatedProcessModules(Db));

                    //if (!_IsInManualWeighingAndAdditionMode)
                    //{
                    //    SelectedPWManualWeighing = _AccessPrimary.NavList.FirstOrDefault();
                    //    CurrentPWManualWeighing = SelectedPWManualWeighing;
                    //}
                }
                return _AccessPrimary;
            }
        }

        [ACPropertySelected(601, "WorkCenterItem")]
        public WorkCenterItem SelectedWorkCenterItem
        {
            get
            {
                return AccessPrimary?.Selected;
            }
            set
            {
                if (AccessPrimary == null || AccessPrimary.Selected == value)
                    return;

                CurrentWorkCenterItem = value;
                AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedWorkCenterItem");
            }
        }

        [ACPropertyCurrent(602, "WorkCenterItem")]
        public WorkCenterItem CurrentWorkCenterItem
        {
            get
            {
                return AccessPrimary?.Current;
            }
            set
            {
                if (AccessPrimary == null || AccessPrimary.Current == value)
                    return;

                DeinitFunctionMonitor();

                if (AccessPrimary.Current != null)
                    AccessPrimary.Current.OnItemDeselected();

                bool changedWFContext = AccessPrimary.Current != value;
                AccessPrimary.Current = value;

                if (changedWFContext)
                    TaskPresenter?.Unload();

                if (value != null)
                {
                    AccessPrimary.Current.OnItemSelected(this);
                    CurrentLayout = null;
                    CurrentLayout = AccessPrimary.Current.ItemLayout;
                }

                OnPropertyChanged("CurrentWorkCenterItem");


                string lastSelected = GetLastSelectedWorkCenterItem();
                string moduleACUrl = "";
                if (value != null && value.ProcessModule != null)
                    moduleACUrl = value.ProcessModule.GetACUrl();
                if (lastSelected != moduleACUrl)
                    SetSelectedWorkCenterItemConfig(value);

                //ApplicationQueue?.Add(() => InitFunctionMonitor());
            }
        }

        [ACPropertyList(603, "WorkCenterItem")]
        public IEnumerable<WorkCenterItem> WorkCenterItems
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        private ACComponent _CurrentProcessModule;
        public ACComponent CurrentProcessModule
        {
            get => _CurrentProcessModule;
            set
            {
                if (_CurrentProcessModule != value)
                {
                    _CurrentProcessModule = value;
                    if (_CurrentProcessModule != null)
                    {
                        PAProcessModuleACCaption = _CurrentProcessModule.ACCaption;
                        _IsCurrentUserConfigured = _RulesForCurrentUser != null
                                                   && _RulesForCurrentUser.Any(c => c.ProcessModuleACUrl == _CurrentProcessModule.ACUrl);
                    }
                    else
                    {
                        PAProcessModuleACCaption = null;
                        _IsCurrentUserConfigured = false;
                    }
                }
            }
        }

        private ACRef<IACComponentPWNode> _CurrentPWGroup;

        //public IACComponentPWNode CurrentPWGroup
        //{
        //    get => _CurrentPWGroup?.ValueT;
        //}

        public string CurrentVBContent
        {
            get;
            set;
        }

        #endregion

        #region Properties => Configuration (Rules)

        private List<WorkCenterRule> _TempRules;

        private List<WorkCenterRule> _RulesForCurrentUser;

        private core.datamodel.VBUser _SelectedVBUser;
        [ACPropertySelected(604, "VBUser", "en{'User'}de{'Benutzer'}")]
        public core.datamodel.VBUser SelectedVBUser
        {
            get => _SelectedVBUser;
            set
            {
                _SelectedVBUser = value;
                OnPropertyChanged("SelectedVBUser");
            }
        }

        private IEnumerable<core.datamodel.VBUser> _VBUserList;
        [ACPropertyList(605, "VBUser")]
        public IEnumerable<core.datamodel.VBUser> VBUserList
        {
            get
            {
                if (_VBUserList == null)
                {
                    using(ACMonitor.Lock(DatabaseApp.ContextIPlus.QueryLock_1X000))
                        _VBUserList = DatabaseApp.ContextIPlus.VBUser.OrderBy(c => c.VBUserName).ToArray();
                }
                return _VBUserList;
            }
        }

        private WorkCenterRule _SelectedAssignedProcessModule;
        [ACPropertySelected(606, "ProcessModuleRules", "en{'Assigned process module'}de{'Assigned process module'}")]
        public WorkCenterRule SelectedAssignedProcessModule
        {
            get => _SelectedAssignedProcessModule;
            set
            {


                _SelectedAssignedProcessModule = value;
                OnPropertyChanged("SelectedAssignedProcessModule");
            }
        }

        private IEnumerable<WorkCenterRule> _AssignedProcessModulesList;
        [ACPropertyList(607, "ProcessModuleRules")]
        public IEnumerable<WorkCenterRule> AssignedProcessModulesList
        {
            get => _AssignedProcessModulesList;
            set
            {
                _AssignedProcessModulesList = value;
                OnPropertyChanged("AssignedProcessModulesList");
            }
        }

        private ACValueItem _SelectedAvailableProcessModule;
        [ACPropertySelected(608, "AvailableRules", "en{'Available work centers'}de{'Verfügbare Arbeitsplätze'}")]
        public ACValueItem SelectedAvailableProcessModule
        {
            get => _SelectedAvailableProcessModule;
            set
            {
                _SelectedAvailableProcessModule = value;
                OnPropertyChanged("SelectedAvailableProcessModule");
            }
        }

        private List<ACValueItem> _AvailableProcessModulesList;
        [ACPropertyList(609, "AvailableRules")]
        public List<ACValueItem> AvailableProcessModulesList
        {
            get => _AvailableProcessModulesList;
            set
            {
                _AvailableProcessModulesList = value;
                OnPropertyChanged("AvailableProcessModulesList");
            }
        }

        private string _UsernameToReplace;
        [ACPropertyInfo(9999, "", "en{'Old username'}de{'Alter Benutzername'}")]
        public string UsernameToReplace
        {
            get => _UsernameToReplace;
            set
            {
                _UsernameToReplace = value;
                OnPropertyChanged();
            }
        }

        private ACPropertyConfigValue<string> _BSOWorkCenterSelectorRules;
        [ACPropertyConfig("en{'Work center rules'}de{'Work center rules'}")]
        public string BSOWorkCenterSelectorRules
        {
            get => _BSOWorkCenterSelectorRules.ValueT;
            set
            {
                _BSOWorkCenterSelectorRules.ValueT = value;
                OnPropertyChanged("BSOWorkCenterSelectorRules");
            }
        }

        private ACPropertyConfigValue<bool> _OnOpenSetLastSelectedWorkCenterItem;
        [ACPropertyConfig("en{'Work center rules'}de{'Work center rules'}")]
        public bool OnOpenSetLastSelectedWorkCenterItem
        {
            get => _OnOpenSetLastSelectedWorkCenterItem.ValueT;
            set
            {
                _OnOpenSetLastSelectedWorkCenterItem.ValueT = value;
                OnPropertyChanged("BSOWorkCenterSelectorRules");
            }
        }

        #endregion

        #region Properties => Workflow

        private VBPresenterTask _TaskPresenter;
        bool _PresenterRightsChecked = false;
        public VBPresenterTask TaskPresenter
        {
            get
            {
                if (_TaskPresenter == null)
                {
                    _TaskPresenter = this.ACUrlCommand("VBPresenterTask(CurrentDesign)") as VBPresenterTask;
                    if (_TaskPresenter == null && !_PresenterRightsChecked && this.Root.InitState == ACInitState.Initialized)
                        Messages.Error(this, "This user has no rights for viewing workflows. Assign rights for VBPresenterTask in the group management!", true);
                    _PresenterRightsChecked = true;
                }
                return _TaskPresenter;
            }
        }

        #endregion

        #region Properties => Partslist

        private InputComponentItem _SelectedInputComponent;
        [ACPropertySelected(620, "InputComponent")]
        public InputComponentItem SelectedInputComponent
        {
            get => _SelectedInputComponent;
            set
            {
                _SelectedInputComponent = value;
                OnPropertyChanged("SelectedInputComponent");
            }
        }

        private List<InputComponentItem> _InputComponentList;
        [ACPropertyList(621, "InputComponent")]
        public List<InputComponentItem> InputComponentList
        {
            get => _InputComponentList;
            set
            {
                _InputComponentList = value;
                OnPropertyChanged("InputComponentList");
            }
        }

        private string _PAProcessModuleACCaption;

        [ACPropertyInfo(603, "", "en{'Module'}de{'Modul'}")]
        public string PAProcessModuleACCaption
        {
            get => _PAProcessModuleACCaption;
            set
            {
                _PAProcessModuleACCaption = value;
                OnPropertyChanged("PAProcessModuleACCaption");
            }
        }

        #endregion

        #region Properties => OrderInfo

        public IACContainerTNet<string> ProcessModuleOrderInfo;

        private ProdOrderBatch _CurrentBatch;
        public ProdOrderBatch CurrentBatch
        {
            get => _CurrentBatch;
            set
            {
                _CurrentBatch = value;
                LoadPartslist();
            }
        }

        private Picking _CurrentPicking;
        public Picking CurrentPicking
        {
            get => _CurrentPicking;
            set
            {
                _CurrentPicking = value;
                LoadPartslist();
            }
        }

        private ProdOrderPartslistPos _EndBatchPos;
        [ACPropertyInfo(604)]
        public ProdOrderPartslistPos EndBatchPos
        {
            get => _EndBatchPos;
            set
            {
                _EndBatchPos = value;
                if (_EndBatchPos == null)
                {
                    ProdOrderProgramNo = null;
                    BatchSeqNo = null;
                    EBPMaterialName = null;
                }
                else
                {
                    ProdOrderProgramNo = _EndBatchPos.ProdOrderPartslist.ProdOrder.ProgramNo;
                    BatchSeqNo = _EndBatchPos.ProdOrderBatch?.BatchSeqNo;
                    EBPMaterialName = _EndBatchPos.ProdOrderPartslist?.Partslist?.Material?.MaterialNo + " " + _EndBatchPos.ProdOrderPartslist?.Partslist?.Material?.MaterialName1;
                }
                OnPropertyChanged("EndBatchPos");
            }
        }

        private string _ProdOrderProgramNo;
        [ACPropertyInfo(605, "", "en{'Order Number'}de{'Auftragsnummer'}")]
        public string ProdOrderProgramNo
        {
            get => _ProdOrderProgramNo;
            set
            {
                _ProdOrderProgramNo = value;
                OnPropertyChanged("ProdOrderProgramNo");
            }
        }

        private int? _BatchSeqNo;
        [ACPropertyInfo(999, "", "en{'Batch'}de{'Batch'}")]
        public int? BatchSeqNo
        {
            get => _BatchSeqNo;
            set
            {
                _BatchSeqNo = value;
                OnPropertyChanged("BatchSeqNo");
            }
        }

        private string _EBPMaterialName;
        [ACPropertyInfo(607, "", "en{'Material'}de{'Material'}")]
        public string EBPMaterialName
        {
            get => _EBPMaterialName;
            set
            {
                _EBPMaterialName = value;
                OnPropertyChanged("EBPMaterialName");
            }
        }

        #endregion

        #region Properties => FunctionMonitor

        private IACContainerTNet<List<ACChildInstanceInfo>> _AccessedProcessModulesProp;

        private IACContainerTNet<bool> _AlarmsInPhysicalModel;

        private bool _IsSelectionManagerInitialized = false;

        private ACComponent _SelectedProcessModuleMonitor;
        [ACPropertySelected(650, "ModuleMonitor")]
        public ACComponent SelectedProcessModuleMonitor
        {
            get => _SelectedProcessModuleMonitor;
            set
            {
                _SelectedProcessModuleMonitor = value;
                OnPropertyChanged("SelectedProcessModuleMonitor");
            }
        }

        private IEnumerable<ACRef<ACComponent>> _ProcessModuleMonitorsList;
        [ACPropertyList(650, "ModuleMonitor")]
        public IEnumerable<ACRef<ACComponent>> ProcessModuleMonitorsList
        {
            get => _ProcessModuleMonitorsList;
            set
            {
                _ProcessModuleMonitorsList = value;
                OnPropertyChanged("ProcessModuleMonitorsList");
            }
        }

        private ACRef<VBBSOSelectionManager> _SelectionManager;
        public VBBSOSelectionManager SelectionManager
        {
            get
            {
                if (_SelectionManager != null)
                    return _SelectionManager.ValueT;
                return null;
            }
        }

        private IEnumerable<core.datamodel.ACClassMethod> _FunctionCommands;
        [ACPropertyInfo(651)]
        public IEnumerable<core.datamodel.ACClassMethod> FunctionCommands
        {
            get => _FunctionCommands;
            set
            {
                _FunctionCommands = value;
                OnPropertyChanged("FunctionCommands");
            }
        }

        private IACObject _SelectedFunction;
        [ACPropertyInfo(652)]
        public IACObject SelectedFunction
        {
            get => _SelectedFunction;
            set
            {
                _SelectedFunction = value;
                OnPropertyChanged("SelectedFunction");
            }
        }

        private bool _AlarmInPhysicalModel;
        [ACPropertyInfo(653)]
        public bool AlarmInPhysicalModel
        {
            get => _AlarmInPhysicalModel;
            set
            {
                _AlarmInPhysicalModel = value;
                OnPropertyChanged("AlarmInPhysicalModel");
            }
        }

        private Type _PAProcFuncType = typeof(PAProcessFunction);
        private Type _PAAlarmBaseType = typeof(PAClassAlarmingBase);

        #endregion

        public core.datamodel.ACClass _SelectedExtraDisTarget;
        [ACPropertySelected(850, "ExtraDis")]
        public core.datamodel.ACClass SelectedExtraDisTarget
        {
            get => _SelectedExtraDisTarget;
            set
            {
                _SelectedExtraDisTarget = value;
                OnPropertyChanged("SelectedExtraDisTarget");
            }
        }

        private IEnumerable<core.datamodel.ACClass> _ExtraDisTargets;
        [ACPropertyList(850, "ExtraDis")]
        public IEnumerable<core.datamodel.ACClass> ExtraDisTargets
        {
            get => _ExtraDisTargets;
            set
            {
                _ExtraDisTargets = value;
                OnPropertyChanged("PumpTargets");
            }
        }

        BSOWorkCenterChild CurrentChildBSO
        {
            get;
            set;
        }

        private string _CurrentLayout;
        [ACPropertyInfo(610)]
        public string CurrentLayout
        {
            get => _CurrentLayout;
            set
            {
                _CurrentLayout = value;
                OnPropertyChanged("CurrentLayout");
            }
        }

        protected List<core.datamodel.ACClass> _PWUserAckClasses = null;
        public virtual IEnumerable<core.datamodel.ACClass> PWUserAckClasses
        {
            get
            {
                if (_PWUserAckClasses == null)
                {
                    List<core.datamodel.ACClass> pwUserAckClasses = new List<core.datamodel.ACClass>();
                    var acClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(PWNodeUserAck));
                    if (acClass != null)
                    {
                        pwUserAckClasses.Add(acClass);
                        var derivedClasses = acClass.DerivedClasses;
                        if (derivedClasses.Any())
                            pwUserAckClasses.AddRange(derivedClasses);
                    }

                    acClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(PWNodeDecisionMsg));
                    if (acClass != null)
                    {
                        pwUserAckClasses.Add(acClass);
                        var derivedClasses = acClass.DerivedClasses;
                        if (derivedClasses.Any())
                            pwUserAckClasses.AddRange(derivedClasses);
                    }

                    _PWUserAckClasses = pwUserAckClasses;
                }
                return _PWUserAckClasses;
            }
        }

        #region Routing service

        protected ACRef<ACComponent> _RoutingService = null;
        public ACComponent RoutingService
        {
            get
            {
                if (_RoutingService == null)
                    return null;
                return _RoutingService.ValueT;
            }
        }

        public bool IsRoutingServiceAvailable
        {
            get
            {
                return RoutingService != null && RoutingService.ConnectionState != ACObjectConnectionState.DisConnected;
            }
        }

        #endregion


        private System.Timers.Timer _timer;
        /// <summary>
        /// Source Property: 
        /// </summary>
        private DateTime _CurrentTime;
        [ACPropertySelected(999, "CurrentTime", "en{'CurrentTime'}de{'CurrentTime'}")]
        public DateTime CurrentTime
        {
            get
            {
                return _CurrentTime;
            }
            set
            {
                if (_CurrentTime != value)
                {
                    _CurrentTime = value;
                    OnPropertyChanged("CurrentTime");
                }
            }
        }

        #region Properties => ReceivedAlarms

        public IACContainerTNet<bool> HasReceivedAlarms;

        private bool _HasAnyReceivedAlarm;
        [ACPropertyInfo(9999)]
        public bool HasAnyReceivedAlarm
        {
            get => _HasAnyReceivedAlarm;
            set
            {
                _HasAnyReceivedAlarm = value;
                OnPropertyChanged();
            }
        }

        private Msg _SelectedReceivedAlarm;
        [ACPropertySelected(9999, "ReceivedAlarm", "en{'Alarms'}de{'Alarms'}")]
        public Msg SelectedReceivedAlarm
        {
            get => _SelectedReceivedAlarm;
            set
            {
                _SelectedReceivedAlarm = value;
                OnPropertyChanged();
            }
        }

        private MsgList _ReceivedAlarmsList;
        [ACPropertyList(9999, "ReceivedAlarm")]
        public MsgList ReceivedAlarmsList
        {
            get => _ReceivedAlarmsList;
            set
            {
                _ReceivedAlarmsList = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Properties => Navigate

        private static List<string> _WorkCenterModules;
        private static List<string> WorkCenterModules
        {
            get
            {
                if (_WorkCenterModules == null)
                {
                    using (Database db = new core.datamodel.Database())
                    {
                        _WorkCenterModules = s_cQry_GetRelevantPAProcessFunctions(db, nameof(PAProcessFunction), Const.KeyACUrl_BusinessobjectList).ToArray().Select(c => c.ACClass1_ParentACClass.ACUrlComponent).ToList();
                    }
                }

                return _WorkCenterModules;
            }
        }

        private ACPropertyConfigValue<string> _CN_BSOWCSNavigation;
        [ACPropertyConfig("en{'Classname BSOWorkCenterSelector for navigate'}de{'Klassenname BSOWorkCenterSelector for navigate'}")]
        public string CN_BSOWCSNavigation
        {
            get
            {
                return _CN_BSOWCSNavigation.ValueT;
            }
            set 
            { 
                _CN_BSOWCSNavigation.ValueT = value; 
            }
        }

        private static string _BSOWCSNavigation;
        public static string BSOWCSNavigation
        {
            get
            {
                if (_BSOWCSNavigation == null)
                {
                    using (Database db = new core.datamodel.Database())
                    {
                        gip.core.datamodel.ACClass classOfBso = typeof(BSOWorkCenterSelector).GetACType() as gip.core.datamodel.ACClass;
                        if (classOfBso != null)
                        {
                            core.datamodel.ACClassConfig config = db.ACClassConfig.Where(c => c.ACClassID == classOfBso.ACClassID && c.LocalConfigACUrl == nameof(CN_BSOWCSNavigation)).FirstOrDefault();
                            if (config != null)
                            {
                                _BSOWCSNavigation = config.Value as string;
                            }
                        }
                    }

                    if (_BSOWCSNavigation == null)
                        _BSOWCSNavigation = nameof(BSOWorkCenterSelector);
                }
                return _BSOWCSNavigation;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Methods => Configuration

        [ACMethodInteraction("", "en{'Configure work center'}de{'Arbeitsplatz konfigurieren'}", 601, true)]
        public void ConfigureBSO()
        {
            if (_TempRules == null)
                _TempRules = GetStoredRules();
            AssignedProcessModulesList = _TempRules;

            var availablePAFs = s_cQry_GetRelevantPAProcessFunctions(DatabaseApp.ContextIPlus, "PAProcessFunction", Const.KeyACUrl_BusinessobjectList).ToArray();
            AvailableProcessModulesList = availablePAFs.Select(c => c.ACClass1_ParentACClass).Distinct().Select(x => new ACValueItem(x.ACUrlComponent, x.ACUrlComponent, null)).OrderBy(c => c.ACCaption).ToList();

            ShowDialog(this, "ConfigurationDialog");
        }

        public bool IsEnabledConfigureBSO()
        {
            return true;//Root.Environment.User.IsSuperuser;
        }

        [ACMethodInfo("", "en{'Grant permission'}de{'Berechtigung erteilen'}", 602, true)]
        public void AddRule()
        {
            //Tuple<string, string> ruleValue = SelectedAvailableProcessModule.Value as Tuple<string, string>;

            WorkCenterRule existingRule = AssignedProcessModulesList.FirstOrDefault(c => c.VBUserName == SelectedVBUser.VBUserName
                                                                                                            && c.ProcessModuleACUrl == SelectedAvailableProcessModule.Value as string);
            if (existingRule != null)
            {
                //if (existingRule.ProcessModuleACUrl == SelectedAvailableProcessModule.Value as string)
                //{
                //    //Info50039:User {0} is already assigned to {1} function. You must first unassign user, then you can assign it again."
                //    Messages.Info(this, "Info50039", false, SelectedVBUser.VBUserName, existingRule.ProcessModuleACUrl);
                //    return;
                //}
                return;
            }

            WorkCenterRule rule = new WorkCenterRule()
            {
                ProcessModuleACUrl = SelectedAvailableProcessModule.Value as string,
                VBUserName = SelectedVBUser.VBUserName,
                //ProcessModulePAFName = ruleValue.Item2
            };
            _TempRules.Add(rule);
            
            //var tempRules = AssignedProcessModulesList.ToList();
            //tempRules.Add(rule);
            AssignedProcessModulesList = _TempRules.ToList();
        }

        public bool IsEnabledAddRule()
        {
            return SelectedVBUser != null && SelectedAvailableProcessModule != null;
        }

        [ACMethodInfo("", "en{'Remove permission'}de{'Berechtigung entfernen'}", 603, true)]
        public void RemoveRule()
        {
            WorkCenterRule rule = _TempRules.FirstOrDefault(c => c == SelectedAssignedProcessModule);
            if (rule != null)
            {
                _TempRules.Remove(rule);
                var tempRules = AssignedProcessModulesList.ToList();
                tempRules.Remove(SelectedAssignedProcessModule);
                AssignedProcessModulesList = tempRules.ToList();
                SelectedAssignedProcessModule = null;
            }
        }

        public bool IsEnabledRemoveRule()
        {
            return SelectedAssignedProcessModule != null;
        }

        [ACMethodInfo("", "en{'Replace username'}de{'Benutzernamen ersetzen'}", 603, true)]
        public void ReplaceUsernameRule()
        {
            ShowDialog(this, "ConfigurationDialogReplace");
        }

        [ACMethodInfo("", "en{'Replace'}de{'Ersetzen'}", 603, true)]
        public void ReplaceUsernameRuleOk()
        {

        }

        [ACMethodInfo("", "en{'Copy user rules'}de{'Benutzerregeln kopieren'}", 603, true)]
        public void CopyUsernameRules()
        {

        }

        [ACMethodInfo("", "en{'Copy'}de{'Kopieren'}", 603, true)]
        public void CopyUsernameOk()
        {
            ShowDialog(this, "ConfigurationDialogCopy");
        }

        [ACMethodInfo("", "en{'Apply rules and close'}de{'Regeln anwenden und schließen'}", 604, true)]
        public void ApplyRulesAndClose()
        {
            string xml = "";
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            using (XmlTextWriter xmlWriter = new XmlTextWriter(sw))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(List<WorkCenterRule>));
                serializer.WriteObject(xmlWriter, _TempRules);
                xml = sw.ToString();
            }
            BSOWorkCenterSelectorRules = xml;
            var msg = DatabaseApp.ACSaveChanges();

            if (msg != null)
                Messages.Msg(msg);

            CloseTopDialog();
        }

        public bool IsEnabledApplyRulesAndClose()
        {
            return true;
        }

        private List<WorkCenterRule> GetRulesByCurrentUser()
        {
            if (_TempRules == null)
                _TempRules = GetStoredRules();
            return _TempRules.Where(c => c.VBUserName == Root.Environment.User.VBUserName).ToList();
        }

        private List<WorkCenterRule> GetStoredRules()
        {
            if (string.IsNullOrEmpty(BSOWorkCenterSelectorRules))
                return new List<WorkCenterRule>();

            using (StringReader ms = new StringReader(BSOWorkCenterSelectorRules))
            using (XmlTextReader xmlReader = new XmlTextReader(ms))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(List<WorkCenterRule>));
                List<WorkCenterRule> result = serializer.ReadObject(xmlReader) as List<WorkCenterRule>;
                if (result == null)
                    return new List<WorkCenterRule>();
                return result;
            }
        }

        #endregion

        #region Save and Undo
        [ACMethodCommand(Facility.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
        }

        /// <summary>
        /// Determines whether [is enabled save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
        }

        /// <summary>
        /// Determines whether [is enabled undo save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled undo save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }
        #endregion

        #region Public Methods
        private void _ClientManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ConnectionQuality")
            {
                if (_ClientManager.ConnectionQuality == ConnectionQuality.Good)
                {
                    ApplicationQueue.Add(() =>
                    {
                        _MainSyncContext?.Send((object state) =>
                        {
                            if (CurrentChildBSO != null)
                            {
                                CurrentChildBSO.DeActivate();
                                CurrentChildBSO.Activate(CurrentProcessModule);
                            }
                        }, new object());
                    });
                }
            }
        }

        private void RegisterOnOrderInfoPropChanged(ACComponent processModule)
        {
            if (ProcessModuleOrderInfo != null)
                ProcessModuleOrderInfo.PropertyChanged -= ProcessModuleOrderInfo_PropertyChanged;

            if (HasReceivedAlarms != null)
                HasReceivedAlarms.PropertyChanged -= HasReceivedAlarms_PropertyChanged;

            ProcessModuleOrderInfo = null;

            ProcessModuleOrderInfo = processModule.GetPropertyNet(nameof(PAProcessModule.OrderInfo)) as IACContainerTNet<string>;
            if (ProcessModuleOrderInfo == null)
            {
                Messages.LogMessage(eMsgLevel.Error, this.GetACUrl(), nameof(BSOWorkCenterSelector), "ProcessModuleOrderInfo property is null!");
                return;
            }

            HasReceivedAlarms = processModule.GetPropertyNet(nameof(PAProcessModule.HasAttachedAlarm)) as IACContainerTNet<bool>;
            if (HasReceivedAlarms != null)
            {
                HasReceivedAlarms.PropertyChanged += HasReceivedAlarms_PropertyChanged;
                HasAnyReceivedAlarm = HasReceivedAlarms.ValueT;
            }

            ProcessModuleOrderInfo.PropertyChanged += ProcessModuleOrderInfo_PropertyChanged;
            string orderInfo = ProcessModuleOrderInfo.ValueT;
            HandleOrderInfoPropChanged(orderInfo, processModule);
        }

        private void HasReceivedAlarms_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                IACContainerTNet<bool> senderProp = sender as IACContainerTNet<bool>;
                if (senderProp != null)
                {
                    HasAnyReceivedAlarm = senderProp.ValueT;
                }
            }
        }

        private void ProcessModuleOrderInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                IACContainerTNet<string> senderProp = sender as IACContainerTNet<string>;
                if (senderProp != null)
                {
                    string orderInfo = senderProp.ValueT;
                    ACComponent currentProcessModule = senderProp.ParentACComponent as ACComponent;
                    ApplicationQueue?.Add(() => HandleOrderInfoPropChanged(orderInfo, currentProcessModule));
                }
            }
        }

        private void HandleOrderInfoPropChanged(string orderInfo, ACComponent currentProcessModule)
        {
            if (string.IsNullOrEmpty(orderInfo))
            {
                CurrentBatch = null;
                EndBatchPos = null;
                CurrentPicking = null;

                DeinitFunctionMonitor();

                using (ACMonitor.Lock(_70050_MembersLock))
                {
                    if (_CurrentPWGroup != null)
                    {
                        _CurrentPWGroup.Detach();
                        _CurrentPWGroup = null;
                    }

                    if (_AlarmsInPhysicalModel != null)
                    {
                        _AlarmsInPhysicalModel.PropertyChanged -= _AlarmsInPhysicalModel_PropertyChanged;
                        _AlarmsInPhysicalModel = null;
                    }
                }

                AlarmInPhysicalModel = false;
            }
            else
            {
                string[] accessArr = (string[])currentProcessModule?.ExecuteMethod(nameof(PAProcessModule.SemaphoreAccessedFrom));
                if (accessArr == null || !accessArr.Any())
                {
                    CurrentBatch = null;
                    EndBatchPos = null;
                    CurrentPicking = null;
                    return;
                }

                string pwGroupACUrl = accessArr[0];
                IACComponentPWNode pwGroup = Root.ACUrlCommand(pwGroupACUrl) as IACComponentPWNode;

                if (pwGroup == null)
                {
                    Thread.Sleep(100);
                    pwGroup = Root.ACUrlCommand(pwGroupACUrl) as IACComponentPWNode;
                }

                if (pwGroup == null)
                {
                    Messages.Error(this, "Can not get mapped PWGroup! ACUrl: " + pwGroupACUrl);
                    return;
                }

                using (ACMonitor.Lock(_70050_MembersLock))
                {
                    _CurrentPWGroup = new ACRef<IACComponentPWNode>(pwGroup, this);
                }

                PAOrderInfo currentOrderInfo = pwGroup?.ExecuteMethod(nameof(PWGroup.GetPAOrderInfo)) as PAOrderInfo;
                if (currentOrderInfo == null)
                {
                    Thread.Sleep(200);
                    currentOrderInfo = Root.ACUrlCommand(pwGroupACUrl + "!" + nameof(PWGroup.GetPAOrderInfo)) as PAOrderInfo;
                }
                if (currentOrderInfo != null)
                {
                    var rootPW = pwGroup?.ParentRootWFNode;
                    if (rootPW == null)
                    {
                        //error
                        return;
                    }

                    var alarmsInPhysicalModel = rootPW.GetPropertyNet(nameof(PWProcessFunction.AlarmsInPhysicalModel)) as IACContainerTNet<bool>;
                    if (alarmsInPhysicalModel == null)
                    {
                        //TODO:error
                        return;
                    }

                    AlarmInPhysicalModel = alarmsInPhysicalModel.ValueT;

                    using (ACMonitor.Lock(_70050_MembersLock))
                    {
                        _AlarmsInPhysicalModel = alarmsInPhysicalModel;
                        _AlarmsInPhysicalModel.PropertyChanged += _AlarmsInPhysicalModel_PropertyChanged;
                    }

                    ApplicationQueue?.Add(() => InitFunctionMonitor());

                    PAOrderInfoEntry entry = currentOrderInfo.Entities.FirstOrDefault(c => c.EntityName == ProdOrderBatch.ClassName);
                    if (entry != null)
                    {
                        CurrentPicking = null;

                        using (DatabaseApp dbApp = new DatabaseApp())
                        {
                            var pb = dbApp.ProdOrderBatch.Include(c => c.ProdOrderPartslistPos_ProdOrderBatch)
                                                               .FirstOrDefault(c => c.ProdOrderBatchID == entry.EntityID);
                            if (pb != null)
                            {
                                CurrentBatch = pb;
                                EndBatchPos = pb.ProdOrderPartslistPos_ProdOrderBatch.FirstOrDefault(c => c.IsFinalMixureBatch);

                                if (EndBatchPos == null)
                                    EndBatchPos = pb.ProdOrderPartslistPos_ProdOrderBatch.OrderByDescending(c => c.Sequence).FirstOrDefault();
                            }
                        }
                    }
                    else
                    {
                        EndBatchPos = null;
                        entry = currentOrderInfo.Entities.FirstOrDefault(c => c.EntityName == Picking.ClassName);
                        if (entry != null)
                        {
                            CurrentBatch = null;

                            using (DatabaseApp dbApp = new DatabaseApp())
                            {
                                var picking = dbApp.Picking.Include(c => c.PickingPos_Picking).FirstOrDefault(c => c.PickingID == entry.EntityID);
                                if (picking != null)
                                {
                                    ProdOrderProgramNo = picking.ACCaption;
                                    CurrentPicking = picking;
                                }
                            }
                        }
                    }
                }
            }
        }

        public virtual void BuildWorkCenterItems()
        {
            _RulesForCurrentUser = GetRulesByCurrentUser();
            var relevantPAFs = s_cQry_GetRelevantPAProcessFunctions(DatabaseApp.ContextIPlus, nameof(PAProcessFunction), Const.KeyACUrl_BusinessobjectList)
                                    .ToArray().OrderBy(c => c.AssemblyACClassInfo != null ? c.AssemblyACClassInfo.SortIndex : 9999).ThenBy(c => c.ACCaption);

            if (_RulesForCurrentUser != null && _RulesForCurrentUser.Any() && !Root.Environment.User.IsSuperuser)
            {
                relevantPAFs = relevantPAFs.Where(c => _RulesForCurrentUser.Any(x => x.ProcessModuleACUrl == c.ACClass1_ParentACClass.ACUrlComponent)).ToArray()
                                           .OrderBy(c => c.AssemblyACClassInfo != null ? c.AssemblyACClassInfo.SortIndex : 9999).ThenBy(c => c.ACCaption);
            }

            if (!relevantPAFs.Any())
                return;

            Type processModuleType = typeof(PAProcessModule);

            List<WorkCenterItem> workCenterItems = new List<WorkCenterItem>();

            foreach (core.datamodel.ACClass paf in relevantPAFs)
            {
                core.datamodel.ACClass processModule = paf.ACClass1_ParentACClass;
                if (processModule == null || !processModuleType.IsAssignableFrom(processModule.ObjectType) || processModule.ACStartTypeIndex != (short)Global.ACStartTypes.Automatic)
                    continue;

                core.datamodel.ACClass configPAF = null;

                if (paf.ConfigurationEntries.Any(x => x.LocalConfigACUrl == Const.LocalConfigACUrl_BusinessobjectList))
                {
                    configPAF = paf;
                }
                else
                {
                    var basePAF = paf.ClassHierarchy.FirstOrDefault(c => c.ACClass1_ParentACClass == null && c.ConfigurationEntries.Any(x => x.LocalConfigACUrl == Const.LocalConfigACUrl_BusinessobjectList));
                    if (basePAF == null)
                        continue;

                    configPAF = basePAF;
                }

                var BSOs = configPAF.ConfigurationEntries.Where(c => c.LocalConfigACUrl == Const.LocalConfigACUrl_BusinessobjectList).Select(x => x.Value as ACComposition).ToArray();
                if (!BSOs.Any())
                    continue;


                WorkCenterItem workCenterItem = workCenterItems.FirstOrDefault(c => c.ProcessModule.ACUrl == processModule.ACUrlComponent);
                if (workCenterItem == null)
                {
                    ACComponent procModule = ACUrlCommand(processModule.ACUrlComponent) as ACComponent;
                    if (procModule == null)
                        continue;

                    workCenterItem = CreateWorkCenterItem(procModule, this);
                    workCenterItems.Add(workCenterItem);
                }
                if (workCenterItem.ProcessModule == null)
                    continue;

                BSOs = OnAddFunctionBSOs(paf, BSOs, workCenterItem);

                WorkCenterItemFunction func = new WorkCenterItemFunction(workCenterItem.ProcessModule, paf.ACIdentifier, this, BSOs);
                workCenterItem.AddItemFunction(func);
            }

            AccessPrimary.ToNavList(workCenterItems.OrderBy(c => c.SortIndex).ThenBy(c => c.ACCaption));
        }

        public virtual WorkCenterItem CreateWorkCenterItem(ACComponent processModule, BSOWorkCenterSelector workCenterSelector)
        {
            return new WorkCenterItem(processModule, workCenterSelector);
        }

        public virtual void SelectWorkCenterItem()
        {
            WorkCenterItem selectedItem = WorkCenterItems.FirstOrDefault();

            if (OnOpenSetLastSelectedWorkCenterItem)
            {
                string workCenterItem = GetLastSelectedWorkCenterItem();
                if (!string.IsNullOrEmpty(workCenterItem))
                {
                    WorkCenterItem lastItem = WorkCenterItems.FirstOrDefault(c => c.ProcessModule != null && c.ProcessModule.GetACUrl() == workCenterItem);
                    if (lastItem != null)
                    {
                        selectedItem = lastItem;
                    }
                }
            }

            SelectedWorkCenterItem = selectedItem;
        }

        public virtual ACComposition[] OnAddFunctionBSOs(core.datamodel.ACClass pafACClass, ACComposition[] bsos, WorkCenterItem workCenterItem)
        {
            return bsos;
        }

        public override void ACAction(ACActionArgs actionArgs)
        {
            base.ACAction(actionArgs);

            if (actionArgs == null || actionArgs.DropObject == null || string.IsNullOrEmpty(actionArgs.DropObject.VBContent))
                return;

            var childBSO = this.ACComponentChilds.FirstOrDefault(c => c.ACIdentifier == actionArgs.DropObject.VBContent) as BSOWorkCenterChild;
            if (childBSO == null && (actionArgs.DropObject.VBContent != "Workflow" && actionArgs.DropObject.VBContent != "BOM" && actionArgs.DropObject.VBContent != "FuncMonitor"))
                return;

            if (CurrentVBContent == actionArgs.DropObject.VBContent && CurrentChildBSO == childBSO && CurrentProcessModule == CurrentWorkCenterItem.ProcessModule)
                return;

            if (actionArgs.ElementAction == Global.ElementActionType.TabItemActivated)
            {
                if (CurrentChildBSO != null)
                    CurrentChildBSO.DeActivate();

                //DeinitFunctionMonitor();

                CurrentChildBSO = childBSO;

                bool processModuleChanged = false;
                if (CurrentProcessModule != CurrentWorkCenterItem.ProcessModule)
                {
                    CurrentProcessModule = CurrentWorkCenterItem.ProcessModule;
                    processModuleChanged = true;
                }

                if (childBSO != null)
                    childBSO.Activate(CurrentProcessModule);

                else if (actionArgs.DropObject.VBContent == "Workflow")
                {
                    ShowWorkflow();
                }
                else if (actionArgs.DropObject.VBContent == "BOM")
                {
                    LoadPartslist();
                }
                //else if (actionArgs.DropObject.VBContent == FunctionMonitorTabVBContent)
                //{
                //    InitFunctionMonitor();
                //}

                CurrentVBContent = actionArgs.DropObject.VBContent;

                if (processModuleChanged)
                    RegisterOnOrderInfoPropChanged(CurrentProcessModule);
            }
        }

        [ACMethodInfo("", "en{'Show/Refresh workflow'}de{'Workflow anzeigen/aktualisieren'}", 610, true)]
        public void ShowWorkflow()
        {
            string[] accessArr = (string[])CurrentWorkCenterItem?.ProcessModule?.ExecuteMethod(nameof(PAProcessModule.SemaphoreAccessedFrom));
            if (accessArr == null || !accessArr.Any())
            {
                if (TaskPresenter != null)
                    TaskPresenter.Unload();
                return;
            }

            string wf = accessArr[0];
            if (!string.IsNullOrEmpty(wf))
            {
                var wfInstance = Root.ACUrlCommand(wf) as IACComponentPWNode;
                if (TaskPresenter != null && wfInstance != null)
                //&& wfInstance.ParentRootWFNode != TaskPresenter.SelectedRootWFNode)
                {
                    TaskPresenter.Unload();
                    TaskPresenter.Load(wfInstance.ParentRootWFNode);
                }
            }
        }

        public bool IsEnabledShowWorkflow()
        {
            return CurrentWorkCenterItem != null && CurrentWorkCenterItem.ProcessModule != null;
        }

        [ACMethodInfo("", "en{'Load Bill of material'}de{'Load Bill of material'}", 620, true)]
        public void LoadPartslist()
        {
            if (_CurrentBatch != null)
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    var currentBatch = _CurrentBatch.FromAppContext<ProdOrderBatch>(dbApp);

                    var inputList = currentBatch.ProdOrderPartslistPosRelation_ProdOrderBatch
                                                        .Where(c => c.SourceProdOrderPartslistPos != null
                                                                && c.TopParentPartslistPosRelation != null
                                                                && c.TargetProdOrderPartslistPos != null
                                                                && c.TargetProdOrderPartslistPos.TopParentPartslistPos != null
                                                                && c.MDProdOrderPartslistPosState != null
                                                                && c.SourceProdOrderPartslistPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot
                                                                && c.TargetQuantityUOM > 0.00001)
                                                        .OrderBy(p => p.TargetProdOrderPartslistPos.TopParentPartslistPos.Sequence)
                                                        .ThenBy(s => s.TopParentPartslistPosRelation.Sequence);

                    List<InputComponentItem> inputComponentsList = new List<InputComponentItem>();
                    foreach (var relation in inputList)
                    {
                        InputComponentItem compItem = new InputComponentItem(relation);
                        OnInputComponentCreated(compItem, relation, dbApp);
                        inputComponentsList.Add(compItem);
                    }

                    InputComponentList = inputComponentsList;
                }
            }
            else if (_CurrentPicking != null)
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    var currentPicking = _CurrentPicking.FromAppContext<Picking>(dbApp);

                    var inputList = currentPicking.PickingPos_Picking.OrderBy(c => c.Sequence);

                    List<InputComponentItem> inputComponentsList = new List<InputComponentItem>();
                    foreach (var picking in inputList)
                    {
                        InputComponentItem compItem = new InputComponentItem(picking);
                        //OnInputComponentCreated(compItem, relation, DatabaseApp);
                        inputComponentsList.Add(compItem);
                    }

                    InputComponentList = inputComponentsList;
                }
            }
            else
            {
                InputComponentList = null;
            }
        }

        protected virtual void OnInputComponentCreated(InputComponentItem item, ProdOrderPartslistPosRelation relation, DatabaseApp dbApp)
        {

        }

        /// <summary>
        /// With this method you can show additional tabs on the interface according a selected process module.
        /// </summary>
        /// <param name="item">Represents the work center item (process module)</param>
        /// <param name="dynamicContent">Represents the dynamic content on the user interface.</param>
        public virtual void OnWorkcenterItemSelected(WorkCenterItem item, ref string dynamicContent)
        {

        }

        public virtual void SelectExtraDisTargetOnPWGroup()
        {
            IACComponentPWNode currentPWGroup = null;
            using (ACMonitor.Lock(_70050_MembersLock))
            {
                currentPWGroup = _CurrentPWGroup?.ValueT;
            }

            if (currentPWGroup == null)
                return;

            if (_RoutingService == null)
            {
                _RoutingService = ACRoutingService.ACRefToServiceInstance(this);
                if (_RoutingService == null)
                {
                    //Error50430: The routing service is unavailable.
                    Messages.Error(this, "Error50430");
                    return;
                }
            }

            ACComponent currentProcessModule = CurrentProcessModule;

            ACRoutingParameters routingParameters = new ACRoutingParameters()
            {
                RoutingService = this.RoutingService,
                Database = DatabaseApp.ContextIPlus,
                AttachRouteItemsToContext = true,
                SelectionRuleID = PAMSilo.SelRuleID_Storage,
                Direction = RouteDirections.Forwards,
                DBSelector = (c, p, r) => typeof(PAMParkingspace).IsAssignableFrom(c.ObjectFullType) || typeof(PAMSilo).IsAssignableFrom(c.ObjectFullType)
                                                                                                     || typeof(PAMIntermediatebin).IsAssignableFrom(c.ObjectFullType),
                DBDeSelector = (c, p, r) => typeof(PAProcessModule).IsAssignableFrom(c.ObjectFullType),
                MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                IncludeReserved = true,
                IncludeAllocated = true
            };

            RoutingResult rResult = ACRoutingService.FindSuccessors(currentProcessModule.ComponentClass, routingParameters);

            if (rResult == null || rResult.Routes == null)
            {
                //Error50431: Can not find any target storage for this station.
                Messages.Error(this, "Error50431");
                return;
            }

            ExtraDisTargets = rResult.Routes.SelectMany(c => c.GetRouteTargets()).Select(x => x.Target);

            ShowDialog(this, "ExtraDisTargetDialog");
        }

        [ACMethodInfo("", "en{'Switch to emptying'}de{'Leerfahren'}", 9999, true)]
        public virtual void SwitchPWGroupToEmptyingMode()
        {
            if (SelectedExtraDisTarget == null)
                return;

            IACComponentPWNode currentPWGroup = null;
            using (ACMonitor.Lock(_70050_MembersLock))
            {
                currentPWGroup = _CurrentPWGroup?.ValueT;
            }

            if (currentPWGroup == null)
                return;

            _CurrentPWGroup.ValueT.ExecuteMethod(nameof(PWGroupVB.SetExtraDisTarget), SelectedExtraDisTarget.ACUrlComponent);

            CloseTopDialog();
        }

        public virtual bool IsEnabledSwitchPWGroupToEmptyingMode()
        {
            return SelectedExtraDisTarget != null;
        }

        [ACMethodInfo("", "en{'Abort all and switch to emptying'}de{'Alles abbrechen und leerfahren'}", 9999, true)]
        public virtual void AbortAllAndSwitchPWGroupToEmptyingMode()
        {
            if (SelectedExtraDisTarget == null)
                return;

            IACComponentPWNode currentPWGroup = null;
            using (ACMonitor.Lock(_70050_MembersLock))
            {
                currentPWGroup = _CurrentPWGroup?.ValueT;
            }

            if (currentPWGroup == null)
                return;

            _CurrentPWGroup.ValueT.ExecuteMethod(nameof(PWGroupVB.AbortAllAndSetExtraDisTarget), SelectedExtraDisTarget.ACUrlComponent);

            CloseTopDialog();
        }

        public virtual bool IsEnabledAbortAllAndSwitchPWGroupToEmptyingMode()
        {
            return SelectedExtraDisTarget != null;
        }

        [ACMethodInfo("","",9999)]
        public void ShowReceivedAlarmsDialog()
        {
            var processModule = CurrentProcessModule;

            if (processModule == null)
                return;

            MsgList receivedAlarms = processModule.ExecuteMethod(nameof(PAProcessModule.GetAttachedAlarms)) as MsgList;

            ReceivedAlarmsList = receivedAlarms;

            ShowDialog(this, "ReceivedAlarmsDialog");
        }

        [ACMethodInfo("", "en{'Acknowledge alarm'}de{'Alarm quittieren'}", 9999)]
        public void AckReceivedAlarm()
        {
            var processModule = CurrentProcessModule;

            if (processModule == null)
                return;

            processModule.ExecuteMethod(nameof(PAProcessModule.AckAttachedAlarm), SelectedReceivedAlarm.MsgId);

            ReceivedAlarmsList.Remove(SelectedReceivedAlarm);
            var alarmList = new MsgList();
            alarmList.AddRange(ReceivedAlarmsList);
            ReceivedAlarmsList = alarmList;
        }

        public bool IsEnabledAckReceivedAlarm()
        {
            return SelectedReceivedAlarm != null;
        }

        [ACMethodInfo("", "en{'Acknowledge all alarms'}de{'Alle Alarme quittieren'}", 9999)]
        public void AckAllReceivedAlarms()
        {
            var processModule = CurrentProcessModule;

            if (processModule == null)
                return;

            processModule.ExecuteMethod(nameof(PAProcessModule.AckAllAttachedAlarms));

            ReceivedAlarmsList = null;
        }

        #endregion

        #region Methods => LastSelectedWorkCenterItem

        public IACConfig GetLastSelectedWorkCenterItemConfig()
        {
            if (ACType == null)
                return null;

            var configs = ACType.GetConfigByKeyACUrl(Root.Environment.User.GetACUrl());
            IACConfig config = configs.FirstOrDefault();
            return config;
        }

        public string GetLastSelectedWorkCenterItem()
        {
            IACConfig config = GetLastSelectedWorkCenterItemConfig();
            return config != null ?
                (config.Value != null ? config.Value.ToString() : "") : "";
        }

        public void SetSelectedWorkCenterItemConfig(WorkCenterItem item)
        {
            if (ACType == null)
                return;

            IACConfig config = GetLastSelectedWorkCenterItemConfig();
            if (config == null && item != null)
            {
                config = ACType.ValueTypeACClass.NewACConfig(null, ACType.ValueTypeACClass.Database.GetACType(typeof(string)));
                config.KeyACUrl = Root.Environment.User.GetACUrl();
            }
            if (item != null && item.ProcessModule != null)
                config.Value = item.ProcessModule.GetACUrl();
            else if (config != null)
                (config as VBEntityObject).DeleteACObject(Database, false);
            Msg msg = DatabaseApp.ContextIPlus.ACSaveChanges();
            if (msg != null)
                Messages.Msg(msg);
        }

        #endregion

        #region Methods => FunctionMonitor

        private void InitFunctionMonitor()
        {
            if (_AccessedProcessModulesProp != null)
            {
                Messages.LogError(this.GetACUrl(), nameof(InitFunctionMonitor) + "(5)", "The _AccessedProcessModulesProp is not null!");
                return;
            }

            IACComponentPWNode pwGroup = null;
            using (ACMonitor.Lock(_70050_MembersLock))
            {
                pwGroup = _CurrentPWGroup?.ValueT;
            }

            var rootPW = pwGroup?.ParentRootWFNode;
            if (rootPW == null)
            {
                Messages.LogError(this.GetACUrl(), nameof(InitFunctionMonitor) + "(10)", "The rootPW is null!");
                return;
            }

            _AccessedProcessModulesProp = rootPW.GetPropertyNet(nameof(PWProcessFunction.AccessedProcessModules)) as IACContainerTNet<List<ACChildInstanceInfo>>;
            if (_AccessedProcessModulesProp == null)
            {
                Messages.LogError(this.GetACUrl(), nameof(InitFunctionMonitor) + "(20)", "The _AccessedProcessModulesProp is null!");
                return;
            }

            InitSelectionManger(Const.SelectionManagerCDesign_ClassName);

            HandleAccessedPMsChanged(_AccessedProcessModulesProp.ValueT);

            _AccessedProcessModulesProp.PropertyChanged += _AccessedProcessModulesProp_PropertyChanged;
        }

        private void DeinitFunctionMonitor()
        {
            if (_AccessedProcessModulesProp != null)
            {
                _AccessedProcessModulesProp.PropertyChanged -= _AccessedProcessModulesProp_PropertyChanged;
                _AccessedProcessModulesProp = null;
            }

            if (SelectionManager != null)
            {
                SelectionManager.PropertyChanged -= SelectionManager_PropertyChanged;
                _SelectionManager.Detach();
                _SelectionManager = null;
            }

            _IsSelectionManagerInitialized = false;

            ProcessModuleMonitorsList = null;
            SelectedProcessModuleMonitor = null;
            FunctionCommands = null;
        }

        private void _AccessedProcessModulesProp_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                IACContainerTNet<List<ACChildInstanceInfo>> senderProp = sender as IACContainerTNet<List<ACChildInstanceInfo>>;
                if (senderProp != null)
                {
                    var modules = senderProp.ValueT?.ToList();
                    ApplicationQueue.Add(() => HandleAccessedPMsChanged(modules));
                }
            }
        }

        private void HandleAccessedPMsChanged(List<ACChildInstanceInfo> activeModules)
        {
            List<ACRef<ACComponent>> result = new List<ACRef<ACComponent>>();

            if (activeModules == null)
                return;

            foreach (ACChildInstanceInfo instaceInfo in activeModules)
            {
                string acUrl = instaceInfo.ACUrlParent + "\\" + instaceInfo.ACIdentifier;

                ACRef<ACComponent> moduleRef = ProcessModuleMonitorsList?.FirstOrDefault(c => c.ValueT.ACUrl == acUrl);
                if (moduleRef == null)
                {
                    ACComponent module = Root.ACUrlCommand(acUrl) as ACComponent;
                    if (module == null)
                    {
                        Messages.Error(this, "Can not find a component with ACUrl: " + acUrl);
                        continue;
                    }

                    moduleRef = new ACRef<ACComponent>(module, this);
                }

                result.Add(moduleRef);
            }

            if (ProcessModuleMonitorsList != null)
            {
                var forDetach = ProcessModuleMonitorsList.Except(result);

                foreach (ACRef<ACComponent> item in forDetach)
                {
                    item.Detach();
                }
            }

            ProcessModuleMonitorsList = result;
            if (!ProcessModuleMonitorsList.Any())
            {
                SelectedFunction = null;
                FunctionCommands = null;
            }
        }

        private void _AlarmsInPhysicalModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                IACContainerTNet<bool> senderProp = sender as IACContainerTNet<bool>;
                if (senderProp != null)
                    AlarmInPhysicalModel = senderProp.ValueT;
            }
        }

        private void InitSelectionManger(string acIdentifier)
        {
            if (_IsSelectionManagerInitialized || SelectionManager != null)
                return;

            var childComp = GetChildComponent(acIdentifier) as VBBSOSelectionManager;
            if (childComp == null)
                childComp = StartComponent(acIdentifier, this, null) as VBBSOSelectionManager;

            if (childComp == null)
                return;

            _SelectionManager = new ACRef<VBBSOSelectionManager>(childComp, this);

            SelectionManager.PropertyChanged += SelectionManager_PropertyChanged;

            _IsSelectionManagerInitialized = true;
        }

        private void SelectionManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectionManager.SelectedACObject))
            {
                FunctionCommands = null;
                var acClass = SelectionManager?.SelectedACObject?.ACType as core.datamodel.ACClass;
                if (acClass != null && acClass.ACKind == Global.ACKinds.TPAProcessFunction)
                {
                    acClass = acClass.FromIPlusContext<core.datamodel.ACClass>(DatabaseApp.ContextIPlus);
                    if (acClass != null)
                    {
                        SelectedFunction = SelectionManager.SelectedACObject;
                        if (SelectedFunction == null)
                            return;

                        FunctionCommands = acClass.GetMethods().Where(c => c.IsInteraction && FilterFunctionCommands(c.ACIdentifier)
                                                                                           && (!c.IsRightmanagement || acClass.RightManager.GetControlMode(c) == Global.ControlModes.Enabled)
                                                                                           && _PAAlarmBaseType.IsAssignableFrom(c.ACClass.ObjectType))
                                                               .OrderBy(x => x.SortIndex).ThenBy(p => p.ACCaption).ToArray();

                        return;
                    }
                }

                SelectedFunction = null;
                FunctionCommands = null;
            }
        }

        protected virtual bool FilterFunctionCommands(string acIdentifier)
        {
            return acIdentifier != "ShowMaintenanceOrder" && acIdentifier != "ShowMaintenanceOrderHistory" && acIdentifier != "GenerateNewMaintenanceOrder";
        }

        #endregion

        private void _timer_Tick(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now;
        }

        #region Methods => Navigation

        [ACMethodAttached("", "en{'Navigate to work center'}de{'Navigiere zur Arbeitsplatz'}", 550, typeof(PAProcessModule), true, "", false, Global.ContextMenuCategory.ProdPlanLog)]
        public static void NavigateToWorkCenter(IACComponent acComponent)
        {
            if (acComponent == null)
                return;

            string acUrl = acComponent.ACUrl;
            IRoot root = acComponent.Root;

            if (root == null)
                return;

            BSOWorkCenterSelector selector = root.Businessobjects.FindChildComponents<BSOWorkCenterSelector>(c => c is BSOWorkCenterSelector, null, 1).FirstOrDefault();
            bool startNewBSO = selector == null;
            if (startNewBSO)
            {
                string bsoNav = BSOWCSNavigation;
                if (string.IsNullOrEmpty(bsoNav))
                    bsoNav = nameof(BSOWorkCenterSelector);

                root.RootPageWPF.StartBusinessobject(Const.BusinessobjectsACUrl + ACUrlHelper.Delimiter_Start + bsoNav, null);
                selector = root.Businessobjects.FindChildComponents<BSOWorkCenterSelector>(c => c is BSOWorkCenterSelector, null, 1).FirstOrDefault();
            }

            if (selector == null)
                return;

            selector.SelectWorkCenterItem(acUrl);

            if (!startNewBSO)
                selector.Root.RootPageWPF.FocusBSO(selector);
        }

        internal void SelectWorkCenterItem(string acUrl)
        {
            var selectedItem = WorkCenterItems.FirstOrDefault(c => c.ProcessModule.ACUrl == acUrl);
            if (selectedItem != null)
                SelectedWorkCenterItem = selectedItem;
        }

        public static bool IsEnabledNavigateToWorkCenter(IACComponent acComponent)
        {
            if (acComponent == null)
                return false;

            return WorkCenterModules.Contains(acComponent.ACUrl);
        }

        #endregion

        #endregion

        #region Execute helper handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;

            switch (acMethodName)
            {
                case nameof(ConfigureBSO):
                    ConfigureBSO();
                    return true;
                case nameof(IsEnabledConfigureBSO):
                    result = IsEnabledConfigureBSO();
                    return true;
                case nameof(AddRule):
                    AddRule();
                    return true;
                case nameof(IsEnabledAddRule):
                    result = IsEnabledAddRule();
                    return true;
                case nameof(RemoveRule):
                    RemoveRule();
                    return true;
                case nameof(IsEnabledRemoveRule):
                    result = IsEnabledRemoveRule();
                    return true;
                case nameof(ApplyRulesAndClose):
                    ApplyRulesAndClose();
                    return true;
                case nameof(IsEnabledApplyRulesAndClose):
                    result = IsEnabledApplyRulesAndClose();
                    return true;
                case nameof(ShowWorkflow):
                    ShowWorkflow();
                    return true;
                case nameof(IsEnabledShowWorkflow):
                    result = IsEnabledShowWorkflow();
                    return true;
                case nameof(SwitchPWGroupToEmptyingMode):
                    SwitchPWGroupToEmptyingMode();
                    return true;
                case nameof(Save):
                    Save();
                    return true;
                case nameof(IsEnabledSave):
                    result = IsEnabledSave();
                    return true;
                case nameof(UndoSave):
                    UndoSave();
                    return true;
                case nameof(IsEnabledUndoSave):
                    result = IsEnabledUndoSave();
                    return true;
                case nameof(IsEnabledSwitchPWGroupToEmptyingMode):
                    result = IsEnabledSwitchPWGroupToEmptyingMode();
                    return true;
            }

            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

        #region Precompiled Queries

        /// <summary>
        /// Get ACClass by based on base ACClass ACIdentifier. Max depth is 5 levels.
        /// </summary>
        public static readonly Func<Database, string, string, IEnumerable<gip.core.datamodel.ACClass>> s_cQry_GetRelevantPAProcessFunctions =
        EF.CompileQuery<Database, string, string, IEnumerable<gip.core.datamodel.ACClass>>(
            (ctx, pafACIdentifier, configKeyACUrl) => ctx.ACClass.Where(c => (c.BasedOnACClassID.HasValue
                                                            && (c.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier // 1. Ableitungsstufe
                                                                || (c.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                        && (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier // 2. Ableitungsstufe
                                                                            || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                        && (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier // 3. Ableitungsstufe
                                                                                            || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                                && c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier) // 4. Ableitungsstufe
                                                                                                    || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                                        && c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier) // 5. Ableitungsstufe
                                                                                                            || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                                            && c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier)
                                                                                            )
                                                                                )
                                                                            )
                                                                    )
                                                                )
                                                            )
                                                            && (c.BasedOnACClassID.HasValue
                                                            && (c.ACClass1_BasedOnACClass.ACClassConfig_ACClass.Any(cf => cf.KeyACUrl == configKeyACUrl) // 1. Ableitungsstufe
                                                                || (c.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                        && (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClassConfig_ACClass.Any(cf => cf.KeyACUrl == configKeyACUrl) // 2. Ableitungsstufe
                                                                            || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                        && (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClassConfig_ACClass.Any(cf => cf.KeyACUrl == configKeyACUrl) // 3. Ableitungsstufe
                                                                                            || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                                && c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClassConfig_ACClass.Any(cf => cf.KeyACUrl == configKeyACUrl) // 4. Ableitungsstufe
                                                                                                    || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                                        && c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClassConfig_ACClass.Any(cf => cf.KeyACUrl == configKeyACUrl) // 5. Ableitungsstufe
                                                                                            )
                                                                                )
                                                                            )
                                                                    )
                                                                )
                                                            )

            ) && c.ACProject != null && c.ACProject.ACProjectTypeIndex == (short)Global.ACProjectTypes.Application && c.ACStartTypeIndex == (short)Global.ACStartTypes.Automatic))
        );

        #endregion
    }
}
