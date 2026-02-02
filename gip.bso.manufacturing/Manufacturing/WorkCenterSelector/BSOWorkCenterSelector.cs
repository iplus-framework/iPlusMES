// Copyright (c) 2024, gipSoft d.o.o.
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
using System.Threading.Tasks;
using gip.mes.datamodel;
using System.Threading;
using gip.mes.processapplication;
using Microsoft.EntityFrameworkCore;
using gip.bso.iplus;
using System.Timers;

namespace gip.bso.manufacturing
{
    /// <summary>
    /// Business service object for managing work center selection in manufacturing environments.
    /// On startup searches for a possible work center, specifically a PAProcessModule which contains function for work in the BSO like(PAFManualWeighing, PAFSampleWeighing,...).
    /// Possible work centers are filtered with user specific access rules if they are defined. The currently working center represents the property SelectedWorkCenterItem.
    /// The property CurrentWorkCenterItem is used for loading all related data for currently selected work center item. 
    /// Except for installed functions under a process module there also other entites which needs to be loaded:
    /// the currently active workflow, bill of material with quantites and function monitor.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Work center'}de{'Arbeitsplatz'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true,
                 Description = @"Business service object for managing work center selection in manufacturing environments.
                                 On startup searches for a possible work center, specifically a PAProcessModule which contains function for work in the BSO like(PAFManualWeighing, PAFSampleWeighing,...).
                                 Possible work centers are filtered with user specific access rules if they are defined. The currently working center represents the property SelectedWorkCenterItem.
                                 The property CurrentWorkCenterItem is used for loading all related data for currently selected work center item. 
                                 Except for installed functions under a process module there also other entites which needs to be loaded:
                                 the currently active workflow, bill of material with quantites and function monitor.")]
    [ACQueryInfo(Const.PackName_VarioManufacturing, Const.QueryPrefix + nameof(BSOWorkCenterSelector), "en{'Work center'}de{'Arbeitsplatz'}", typeof(WorkCenterItem), nameof(BSOWorkCenterSelector), "", "")]
    public class BSOWorkCenterSelector : ACBSOvbNav
    {
        #region c'tors

        public BSOWorkCenterSelector(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _CN_BSOWCSNavigation = new ACPropertyConfigValue<string>(this, "CN_BSOWCSNavigation", "");
        }

        /// <summary>
        /// Initializes the BSOWorkCenterSelector by calling the base initialization, setting up the main synchronization context,
        /// configuring property values for work center rules and last selected item settings, starting the application queue worker thread,
        /// and initializing a timer for periodic updates.
        /// </summary>
        /// <param name="startChildMode">The start mode for child components.</param>
        /// <returns>True if initialization succeeds, otherwise false.</returns>
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

        /// <summary>
        /// Performs post-initialization of the BSOWorkCenterSelector by building work center items from available process modules,
        /// selecting the initial work center item (optionally restoring the last selected item based on configuration),
        /// setting up the WCF client manager for remote communication, and calling the base post-initialization.
        /// This method is called after ACInit to complete the component setup.
        /// </summary>
        /// <returns>True if post-initialization succeeds, otherwise false.</returns>
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

        /// <summary>
        /// Deinitializes the BSOWorkCenterSelector instance and releases all resources, unsubscribes from events, stops background threads, and cleans up child components.
        /// This method should be called when the business service object is no longer needed to ensure proper cleanup and avoid memory leaks.
        /// </summary>
        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
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

            return await base.ACDeInit(deleteACClassTask);
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

        /// <summary>
        /// Gets the navigation access object for the work center selector.
        /// Returns the primary navigation access, which manages navigation and selection of WorkCenterItem entities.
        /// </summary>
        public override IAccessNav AccessNav 
        { 
            get 
            { 
                return AccessPrimary; 
            } 
        }

        ACAccessNav<WorkCenterItem> _AccessPrimary;

        /// <summary>
        /// Provides primary navigation access for WorkCenterItem entities in the work center selector.
        /// Initializes the ACAccessNav instance using the class type and query definition, enabling navigation and selection of work center items.
        /// </summary>
        [ACPropertyAccessPrimary(9999, "WorkCenterItem",
                                 Description = @"Provides primary navigation access for WorkCenterItem entities in the work center selector.
                                                 Initializes the ACAccessNav instance using the class type and query definition, enabling navigation and selection of work center items.")]
        public virtual ACAccessNav<WorkCenterItem> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(this, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<WorkCenterItem>("WorkCenterItem", this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the currently selected work center item.
        /// This property represents the work center item that is highlighted or chosen by the user in the navigation list.
        /// Setting this property updates the current work center item and notifies property changes.
        /// </summary>
        [ACPropertySelected(601, "WorkCenterItem",
                            Description = @"Gets or sets the currently selected work center item.
                                            This property represents the work center item that is highlighted or chosen by the user in the navigation list.
                                            Setting this property updates the current work center item and notifies property changes.")]
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

        /// <summary>
        /// Gets or sets the current work center item that is being actively used or displayed.
        /// This property represents the work center item that is currently loaded and active for operations.
        /// When set, it handles the complete lifecycle of switching between work center items including:
        /// - Deinitializing the function monitor for the previous item
        /// - Calling deselection handlers on the previous item
        /// - Unloading the task presenter if the workflow context changes
        /// - Calling selection handlers and updating the layout for the new item
        /// - Saving the selection to configuration for persistence across sessions
        /// Setting this property triggers a cascade of updates throughout the work center system.
        /// </summary>
        [ACPropertyCurrent(602, "WorkCenterItem",
                           Description = @"Gets or sets the current work center item that is being actively used or displayed.
                                           This property represents the work center item that is currently loaded and active for operations.
                                           When set, it handles the complete lifecycle of switching between work center items including:
                                           - Deinitializing the function monitor for the previous item
                                           - Calling deselection handlers on the previous item
                                           - Unloading the task presenter if the workflow context changes
                                           - Calling selection handlers and updating the layout for the new item
                                           - Saving the selection to configuration for persistence across sessions
                                           Setting this property triggers a cascade of updates throughout the work center system.")]
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

        /// <summary>
        /// Gets the collection of available work center items that can be selected and operated on.
        /// This property provides access to all work center items that have been built during initialization,
        /// filtered by user access rules if configured. Each work center item represents a process module
        /// with its associated functions and capabilities. The items are ordered by sort index and caption.
        /// </summary>
        [ACPropertyList(603, "WorkCenterItem",
                        Description = @"Gets the collection of available work center items that can be selected and operated on.
                                        This property provides access to all work center items that have been built during initialization,
                                        filtered by user access rules if configured. Each work center item represents a process module
                                        with its associated functions and capabilities. The items are ordered by sort index and caption.")]
        public IEnumerable<WorkCenterItem> WorkCenterItems
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        private ACComponent _CurrentProcessModule;
        /// <summary>
        /// Gets or sets the current process module associated with the work center.
        /// This property represents the active ACComponent that serves as the process module for the selected work center.
        /// When set, it updates the PAProcessModuleACCaption and determines if the current user is configured for this module.
        /// If the process module is null, both the caption and user configuration status are reset to null/false.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the current VBContent identifier representing the active tab or view in the work center selector UI.
        /// Used to track which content (e.g., Workflow, BOM, Function Monitor) is currently displayed.
        /// </summary>
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
        /// <summary>
        /// Gets or sets the currently selected VBUser for work center rule configuration.
        /// This property is used to assign, remove, or modify permissions for users in the work center selector.
        /// Changing this property updates the UI and triggers property change notifications.
        /// </summary>
        [ACPropertySelected(604, "VBUser", "en{'User'}de{'Benutzer'}",
                            Description = @"Gets or sets the currently selected VBUser for work center rule configuration.
                                            This property is used to assign, remove, or modify permissions for users in the work center selector.
                                            Changing this property updates the UI and triggers property change notifications.")]
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
        /// <summary>
        /// Gets the list of VBUser entities from the database, ordered by user name.
        /// The list is loaded and cached on first access, and subsequent accesses return the cached list.
        /// </summary>
        [ACPropertyList(605, "VBUser",
                        Description = @"Gets the list of VBUser entities from the database, ordered by user name.
                                        The list is loaded and cached on first access, and subsequent accesses return the cached list.")]
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
        /// <summary>
        /// Gets or sets the currently selected assigned process module rule for the work center.
        /// This property is used to highlight or choose a specific WorkCenterRule (permission assignment)
        /// in the configuration dialog, allowing the user to manage which process module is assigned to which user.
        /// Changing this property updates the UI and triggers property change notifications.
        /// </summary>
        [ACPropertySelected(606, "ProcessModuleRules", "en{'Assigned process module'}de{'Assigned process module'}",
                            Description = @"Gets or sets the currently selected assigned process module rule for the work center.
                                            This property is used to highlight or choose a specific WorkCenterRule (permission assignment)
                                            in the configuration dialog, allowing the user to manage which process module is assigned to which user.
                                            Changing this property updates the UI and triggers property change notifications.")]
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
        /// <summary>
        /// Gets or sets the list of assigned process module rules for the work center.
        /// This property provides access to the collection of WorkCenterRule objects that define
        /// which process modules are assigned to which users for the current configuration.
        /// Used in the configuration dialog to manage user permissions for work centers.
        /// </summary>
        [ACPropertyList(607, "ProcessModuleRules",
                        Description = @"Gets or sets the list of assigned process module rules for the work center.
                                        This property provides access to the collection of WorkCenterRule objects that define
                                        which process modules are assigned to which users for the current configuration.
                                        Used in the configuration dialog to manage user permissions for work centers.")]
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
        /// <summary>
        /// Gets or sets the currently selected available process module (work center) for rule assignment.
        /// This property is used in the configuration dialog to select a work center from the list of available modules
        /// when granting permissions to users. Changing this property updates the UI and triggers property change notifications.
        /// </summary>
        [ACPropertySelected(608, "AvailableRules", "en{'Available work centers'}de{'Verfügbare Arbeitsplätze'}",
                            Description = @"Gets or sets the currently selected available process module (work center) for rule assignment.
                                            This property is used in the configuration dialog to select a work center from the list of available modules
                                            when granting permissions to users. Changing this property updates the UI and triggers property change notifications.")]
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
        /// <summary>
        /// Gets or sets the list of available process modules (work centers) for rule assignment.
        /// This property is used in the configuration dialog to select a work center from the list of available modules
        /// when granting permissions to users. Changing this property updates the UI and triggers property change notifications.
        /// </summary>
        [ACPropertyList(609, "AvailableRules",
                        Description = @"Gets or sets the list of available process modules (work centers) for rule assignment.
                                        This property is used in the configuration dialog to select a work center from the list of available modules
                                        when granting permissions to users. Changing this property updates the UI and triggers property change notifications.")]
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
        /// <summary>
        /// Gets or sets the old username to be replaced in work center rules.
        /// Used in the configuration dialog for replacing user assignments.
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Old username'}de{'Alter Benutzername'}",
                        Description = @"Gets or sets the old username to be replaced in work center rules.
                                        Used in the configuration dialog for replacing user assignments.")]
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
        /// <summary>
        /// Gets or sets the XML-serialized work center rules configuration.
        /// This property stores the permission assignments for users and process modules
        /// as an XML string, which is loaded and saved to persist user-specific access rules.
        /// </summary>
        [ACPropertyConfig("en{'Work center rules'}de{'Work center rules'}",
                          Description = @"Gets or sets the XML-serialized work center rules configuration.
                                          This property stores the permission assignments for users and process modules
                                          as an XML string, which is loaded and saved to persist user-specific access rules.")]
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
        /// <summary>
        /// Gets or sets a value indicating whether the last selected work center item should be restored when the selector is opened.
        /// If true, the selector will automatically select the previously used work center for the current user.
        /// </summary>
        [ACPropertyConfig("en{'Work center rules'}de{'Work center rules'}",
                          Description = @"Gets or sets a value indicating whether the last selected work center item should be restored when the selector is opened.
                                          If true, the selector will automatically select the previously used work center for the current user.")]
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
        /// <summary>
        /// Gets the workflow presenter task for the current work center selector.
        /// Lazily initializes and returns a VBPresenterTask instance for displaying and managing workflows.
        /// If the presenter cannot be created and the user lacks rights, an error message is shown.
        /// </summary>
        public VBPresenterTask TaskPresenter
        {
            get
            {
                if (_TaskPresenter == null)
                {
                    _TaskPresenter = this.ACUrlCommand("VBPresenterTask(CurrentDesign)") as VBPresenterTask;
                    if (_TaskPresenter == null && !_PresenterRightsChecked && this.Root.InitState == ACInitState.Initialized)
                        Messages.ErrorAsync(this, "This user has no rights for viewing workflows. Assign rights for VBPresenterTask in the group management!", true);
                    _PresenterRightsChecked = true;
                }
                return _TaskPresenter;
            }
        }

        #endregion

        #region Properties => Partslist

        private InputComponentItem _SelectedInputComponent;
        /// <summary>
        /// Gets or sets the currently selected input component item in the work center selector.
        /// This property represents the input component (e.g., material or part) that is highlighted or chosen by the user
        /// from the list of input components associated with the current batch or picking operation.
        /// Changing this property updates the UI and triggers property change notifications.
        /// </summary>
        [ACPropertySelected(620, "InputComponent",
                            Description = @"Gets or sets the currently selected input component item in the work center selector.
                                            This property represents the input component (e.g., material or part) that is highlighted or chosen by the user
                                            from the list of input components associated with the current batch or picking operation.
                                            Changing this property updates the UI and triggers property change notifications.")]
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
        /// <summary>
        /// Gets or sets the list of input component items in the work center selector.
        /// This property represents the collection of input components (e.g., materials or parts)
        /// associated with the current batch or picking operation. The list is updated when the batch
        /// or picking context changes and is used to display and manage input components in the UI.
        /// </summary>
        [ACPropertyList(621, "InputComponent",
                        Description = @"Gets or sets the list of input component items in the work center selector.
                                        This property represents the collection of input components (e.g., materials or parts)
                                        associated with the current batch or picking operation. The list is updated when the batch
                                        or picking context changes and is used to display and manage input components in the UI.")]
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
        /// <summary>
        /// Gets or sets the caption of the current process module (work center).
        /// This property is used to display the name of the active module in the UI.
        /// The value is updated whenever the CurrentProcessModule property changes.
        /// </summary>
        [ACPropertyInfo(603, "", "en{'Module'}de{'Modul'}",
                        Description = @"Gets or sets the caption of the current process module (work center).
                                        This property is used to display the name of the active module in the UI.
                                        The value is updated whenever the CurrentProcessModule property changes.")]
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
        /// <summary>
        /// Gets or sets the currently selected production batch in the work center selector.
        /// When set, this property updates the internal batch reference and reloads the associated bill of materials (partslist).
        /// </summary>
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
        /// <summary>
        /// Gets or sets the currently selected picking order in the work center selector.
        /// When set, this property updates the internal picking reference and reloads the associated bill of materials (partslist).
        /// </summary>
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
        /// <summary>
        /// Gets or sets the end batch position for the current production order.
        /// This property represents the final or last relevant position in the bill of materials (BOM) for the batch.
        /// When set, it updates related properties such as the production order number, batch sequence number, and material name.
        /// If the value is null, all related properties are reset to null.
        /// </summary>
        [ACPropertyInfo(604,
                        Description = @"Gets or sets the end batch position for the current production order.
                                        This property represents the final or last relevant position in the bill of materials (BOM) for the batch.
                                        When set, it updates related properties such as the production order number, batch sequence number, and material name.
                                        If the value is null, all related properties are reset to null.")]
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
        /// <summary>
        /// Gets or sets the production order number for the current batch or picking operation.
        /// This property is used to display or update the order number associated with the selected production batch or picking order
        /// in the work center selector. Changing this property triggers property change notifications.
        /// </summary>
        [ACPropertyInfo(605, "", "en{'Order Number'}de{'Auftragsnummer'}",
                        Description = @"Gets or sets the production order number for the current batch or picking operation.
                                        This property is used to display or update the order number associated with the selected production batch or picking order
                                        in the work center selector. Changing this property triggers property change notifications.")]
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
        /// <summary>
        /// Gets or sets the batch sequence number for the current production batch.
        /// This property represents the sequence number of the batch in the production order.
        /// Changing this property updates the UI and triggers property change notifications.
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Batch'}de{'Batch'}",
                        Description = @"Gets or sets the batch sequence number for the current production batch.
                                        This property represents the sequence number of the batch in the production order.
                                        Changing this property updates the UI and triggers property change notifications.")]
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
        /// <summary>
        /// Gets or sets the material name for the end batch position in the work center selector.
        /// This property displays the name and number of the material associated with the final batch position
        /// in the bill of materials (BOM) for the current production order or picking operation.
        /// The value is updated when the EndBatchPos property changes.
        /// </summary>
        [ACPropertyInfo(607, "", "en{'Material'}de{'Material'}",
                        Description = @"Gets or sets the material name for the end batch position in the work center selector.
                                        This property displays the name and number of the material associated with the final batch position
                                        in the bill of materials (BOM) for the current production order or picking operation.
                                        The value is updated when the EndBatchPos property changes.")]
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
        /// <summary>
        /// Gets or sets the currently selected process module monitor in the work center selector.
        /// This property represents the process module (ACComponent) that is highlighted or chosen for monitoring
        /// in the function monitor tab. Changing this property updates the UI and triggers property change notifications.
        /// </summary>
        [ACPropertySelected(650, "ModuleMonitor",
                            Description = @"Gets or sets the currently selected process module monitor in the work center selector.
                                            This property represents the process module (ACComponent) that is highlighted or chosen for monitoring
                                            in the function monitor tab. Changing this property updates the UI and triggers property change notifications.")]
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
        /// <summary>
        /// Gets or sets the list of process module monitors for the work center selector.
        /// This property provides access to the collection of ACComponent references that are currently monitored
        /// in the function monitor tab. Changing this property updates the UI and triggers property change notifications.
        /// </summary>
        [ACPropertyList(650, "ModuleMonitor",
                        Description = @"Gets or sets the list of process module monitors for the work center selector.
                                        This property provides access to the collection of ACComponent references that are currently monitored
                                        in the function monitor tab. Changing this property updates the UI and triggers property change notifications.")]
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
        /// <summary>
        /// Gets the selection manager for the work center selector.
        /// Returns the VBBSOSelectionManager instance if available, otherwise null.
        /// Used to manage selection of functions and modules within the work center.
        /// </summary>
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
        /// <summary>
        /// Gets or sets the collection of available function commands for the selected function in the work center selector.
        /// This property provides access to the list of ACClassMethod objects representing interactive commands
        /// that can be executed for the currently selected function, such as workflow or process actions.
        /// The list is updated when the selection changes and is used to display available commands in the UI.
        /// </summary>
        [ACPropertyInfo(651,
                        Description = @"Gets or sets the collection of available function commands for the selected function in the work center selector.
                                        This property provides access to the list of ACClassMethod objects representing interactive commands
                                        that can be executed for the currently selected function, such as workflow or process actions.
                                        The list is updated when the selection changes and is used to display available commands in the UI.")]
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
        /// <summary>
        /// Gets or sets the currently selected function in the work center selector.
        /// This property represents the function (IACObject) that is highlighted or chosen by the user
        /// in the function monitor or workflow tab. Changing this property updates the UI and triggers property change notifications.
        /// </summary>
        [ACPropertyInfo(652,
                        Description = @"Gets or sets the currently selected function in the work center selector.
                                        This property represents the function (IACObject) that is highlighted or chosen by the user
                                        in the function monitor or workflow tab. Changing this property updates the UI and triggers property change notifications.")]
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
        /// <summary>
        /// Gets or sets a value indicating whether there is an active alarm in the physical model for the current work center.
        /// This property is updated when the alarm state changes and notifies property changes to update the UI.
        /// </summary>
        [ACPropertyInfo(653,
                        Description = @"Gets or sets a value indicating whether there is an active alarm in the physical model for the current work center.
                                        This property is updated when the alarm state changes and notifies property changes to update the UI.")]
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
        /// <summary>
        /// Gets or sets the currently selected extra discharge target (ACClass) for the work center.
        /// This property is used to select a target for emptying or discharge operations in the work center selector UI.
        /// Changing this property updates the UI and triggers property change notifications.
        /// </summary>
        [ACPropertySelected(850, "ExtraDis",
                            Description = @"Gets or sets the currently selected extra discharge target (ACClass) for the work center.
                                            This property is used to select a target for emptying or discharge operations in the work center selector UI.
                                            Changing this property updates the UI and triggers property change notifications.")]
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
        /// <summary>
        /// Gets or sets the list of extra discharge targets (ACClass) for the work center.
        /// This property is used to select possible targets for emptying or discharge operations in the work center selector UI.
        /// Changing this property updates the UI and triggers property change notifications.
        /// </summary>
        [ACPropertyList(850, "ExtraDis",
                        Description = @"Gets or sets the list of extra discharge targets (ACClass) for the work center.
                                        This property is used to select possible targets for emptying or discharge operations in the work center selector UI.
                                        Changing this property updates the UI and triggers property change notifications.")]
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
        /// <summary>
        /// Gets or sets the current layout identifier for the work center selector UI.
        /// This property is used to track and update the active layout or view displayed to the user.
        /// Changing this property triggers property change notifications to update the interface.
        /// </summary>
        [ACPropertyInfo(610,
                        Description = @"Gets or sets the current layout identifier for the work center selector UI.
                                        This property is used to track and update the active layout or view displayed to the user.
                                        Changing this property triggers property change notifications to update the interface.")]
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

        /// <summary>
        /// Gets the collection of ACClass types used for user acknowledgements in process workflows.
        /// This includes the base types PWNodeUserAck and PWNodeDecisionMsg, along with all their derived classes.
        /// The list is initialized on first access and cached for subsequent calls.
        /// </summary>
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
        /// <summary>
        /// Gets the routing service component associated with the current work center selector.
        /// Returns the ACComponent instance referenced by the internal _RoutingService property,
        /// or null if no routing service is available.
        /// </summary>
        public ACComponent RoutingService
        {
            get
            {
                if (_RoutingService == null)
                    return null;
                return _RoutingService.ValueT;
            }
        }

        /// <summary>
        /// Indicates whether the routing service is available and connected.
        /// Returns true if the RoutingService property is not null and its connection state is not disconnected.
        /// </summary>
        public bool IsRoutingServiceAvailable
        {
            get
            {
                return RoutingService != null && RoutingService.ConnectionState != ACObjectConnectionState.DisConnected;
            }
        }

        #endregion

        private System.Timers.Timer _timer;
        private DateTime _CurrentTime;
        /// <summary>
        /// Gets or sets the current time for the work center selector.
        /// This property is updated every second by an internal timer and can be used for time-based UI updates or operations.
        /// </summary>
        [ACPropertySelected(9999, "CurrentTime", "en{'CurrentTime'}de{'CurrentTime'}",
                            Description = @"Gets or sets the current time for the work center selector.
                                            This property is updated every second by an internal timer and can be used for time-based UI updates or operations.")]
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
        /// <summary>
        /// Gets or sets a value indicating whether any received alarm is present for the current work center.
        /// This property is updated when the alarm state changes and notifies property changes to update the UI.
        /// </summary>
        [ACPropertyInfo(9999,
                        Description = @"Gets or sets a value indicating whether any received alarm is present for the current work center.
                                        This property is updated when the alarm state changes and notifies property changes to update the UI.")]
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
        /// <summary>
        /// Gets or sets the currently selected received alarm message for the work center.
        /// This property represents the alarm (Msg) that is highlighted or chosen by the user
        /// in the received alarms dialog. Changing this property updates the UI and triggers property change notifications.
        /// </summary>
        [ACPropertySelected(9999, "ReceivedAlarm", "en{'Alarms'}de{'Alarms'}",
                            Description = @"Gets or sets the currently selected received alarm message for the work center.
                                            This property represents the alarm (Msg) that is highlighted or chosen by the user
                                            in the received alarms dialog. Changing this property updates the UI and triggers property change notifications.")]
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
        /// <summary>
        /// Gets or sets the list of received alarm messages for the work center.
        /// This property represents the collection of Msg objects that have been received and are displayed in the alarms dialog.
        /// Changing this property updates the UI and triggers property change notifications.
        /// </summary>
        [ACPropertyList(9999, "ReceivedAlarm",
                        Description = @"Gets or sets the list of received alarm messages for the work center.
                                        This property represents the collection of Msg objects that have been received and are displayed in the alarms dialog.
                                        Changing this property updates the UI and triggers property change notifications.")]
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
        /// <summary>
        /// Gets or sets the navigation class name for the BSOWorkCenterSelector used in navigation scenarios.
        /// This property is stored in the configuration and allows dynamic navigation to the work center selector.
        /// </summary>
        [ACPropertyConfig("en{'Classname BSOWorkCenterSelector for navigate'}de{'Klassenname BSOWorkCenterSelector for navigate'}",
                          Description = @"Gets or sets the navigation class name for the BSOWorkCenterSelector used in navigation scenarios.
                                          This property is stored in the configuration and allows dynamic navigation to the work center selector.")]
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
        /// <summary>
        /// Gets the navigation class name for the BSOWorkCenterSelector used in navigation scenarios.
        /// This static property retrieves the class name from the configuration table (ACClassConfig) for the BSOWorkCenterSelector type.
        /// If no configuration is found, it defaults to the type name "BSOWorkCenterSelector".
        /// Used for dynamic navigation and instantiation of the work center selector business object.
        /// </summary>
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

        /// <summary>
        /// Opens the configuration dialog for work center permissions.
        /// Loads the current permission rules and available process modules (work centers),
        /// then displays the configuration UI to allow assignment or removal of user permissions.
        /// </summary>
        [ACMethodInteraction("", "en{'Configure work center'}de{'Arbeitsplatz konfigurieren'}", 601, true,
                             Description = @"Opens the configuration dialog for work center permissions.
                                             Loads the current permission rules and available process modules (work centers),
                                             then displays the configuration UI to allow assignment or removal of user permissions.")]
        public void ConfigureBSO()
        {
            if (_TempRules == null)
                _TempRules = GetStoredRules();
            AssignedProcessModulesList = _TempRules;

            var availablePAFs = s_cQry_GetRelevantPAProcessFunctions(DatabaseApp.ContextIPlus, "PAProcessFunction", Const.KeyACUrl_BusinessobjectList).ToArray();
            AvailableProcessModulesList = availablePAFs.Select(c => c.ACClass1_ParentACClass).Distinct().Select(x => new ACValueItem(x.ACUrlComponent, x.ACUrlComponent, null)).OrderBy(c => c.ACCaption).ToList();

            ShowDialog(this, "ConfigurationDialog");
        }

        /// <summary>
        /// Determines whether the configuration dialog for work center permissions can be opened.
        /// Currently returns true, allowing all users to configure work centers.
        /// Previously restricted to superusers only (commented out logic: Root.Environment.User.IsSuperuser).
        /// </summary>
        /// <returns>True if configuration is enabled, otherwise false.</returns>
        public bool IsEnabledConfigureBSO()
        {
            return true;//Root.Environment.User.IsSuperuser;
        }

        /// <summary>
        /// Grants permission to a user for accessing a specific work center (process module).
        /// Creates a new WorkCenterRule that associates the selected user with the selected process module.
        /// If the user already has permission for the selected work center, the method returns without making changes.
        /// The new rule is added to the temporary rules collection and updates the assigned process modules list in the UI.
        /// </summary>
        [ACMethodInfo("", "en{'Grant permission'}de{'Berechtigung erteilen'}", 602, true,
                      Description = @"Grants permission to a user for accessing a specific work center (process module).
                                      Creates a new WorkCenterRule that associates the selected user with the selected process module.
                                      If the user already has permission for the selected work center, the method returns without making changes.
                                      The new rule is added to the temporary rules collection and updates the assigned process modules list in the UI.")]
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
                //    Messages.InfoAsync(this, "Info50039", false, SelectedVBUser.VBUserName, existingRule.ProcessModuleACUrl);
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

        /// <summary>
        /// Determines whether the AddRule command can be executed.
        /// Returns true if both a VBUser and an available process module are selected,
        /// which are required to create a new work center permission rule.
        /// </summary>
        /// <returns>True if both SelectedVBUser and SelectedAvailableProcessModule are not null, otherwise false.</returns>
        public bool IsEnabledAddRule()
        {
            return SelectedVBUser != null && SelectedAvailableProcessModule != null;
        }

        /// <summary>
        /// Removes permission for a user from accessing a specific work center (process module).
        /// Deletes the selected WorkCenterRule that associates a user with a process module.
        /// The rule is removed from both the temporary rules collection and the assigned process modules list in the UI.
        /// After removal, the selection is cleared to prevent operations on a deleted object.
        /// </summary>
        [ACMethodInfo("", "en{'Remove permission'}de{'Berechtigung entfernen'}", 603, true,
                      Description = @"Removes permission for a user from accessing a specific work center (process module).
                                      Deletes the selected WorkCenterRule that associates a user with a process module.
                                      The rule is removed from both the temporary rules collection and the assigned process modules list in the UI.
                                      After removal, the selection is cleared to prevent operations on a deleted object.")]
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

        /// <summary>
        /// Determines whether the RemoveRule command can be executed.
        /// Returns true if a process module rule is currently selected for removal,
        /// which is required to perform the remove operation for work center permissions.
        /// </summary>
        /// <returns>True if SelectedAssignedProcessModule is not null, otherwise false.</returns>
        public bool IsEnabledRemoveRule()
        {
            return SelectedAssignedProcessModule != null;
        }

        /// <summary>
        /// Replaces usernames in work center permission rules.
        /// Opens a dialog that allows the user to replace all occurrences of a specific username
        /// with a new username in the work center access rules configuration.
        /// This method is useful when users are renamed or when consolidating user permissions.
        /// </summary>
        [ACMethodInfo("", "en{'Replace username'}de{'Benutzernamen ersetzen'}", 603, true,
                      Description = @"Replaces usernames in work center permission rules.
                                      Opens a dialog that allows the user to replace all occurrences of a specific username
                                      with a new username in the work center access rules configuration.
                                      This method is useful when users are renamed or when consolidating user permissions.")]
        public void ReplaceUsernameRule()
        {
            ShowDialog(this, "ConfigurationDialogReplace");
        }

        /// <summary>
        /// Confirms the replacement of usernames in work center permission rules.
        /// This method processes the replacement operation by updating all occurrences of the old username
        /// with the new username in the temporary rules collection, then closes the replacement dialog.
        /// The actual replacement logic should be implemented to iterate through the rules and update
        /// the VBUserName property where it matches the username to be replaced.
        /// </summary>
        [ACMethodInfo("", "en{'Replace'}de{'Ersetzen'}", 603, true,
                      Description = @"Confirms the replacement of usernames in work center permission rules.
                                      This method processes the replacement operation by updating all occurrences of the old username
                                      with the new username in the temporary rules collection, then closes the replacement dialog.
                                      The actual replacement logic should be implemented to iterate through the rules and update
                                      the VBUserName property where it matches the username to be replaced.")]
        public void ReplaceUsernameRuleOk()
        {

        }

        /// <summary>
        /// Opens a dialog to copy work center permission rules from one user to another.
        /// Loads the current rules and prepares the UI for selecting source and target users.
        /// The actual copy logic should be implemented in the confirmation method.
        /// </summary>
        [ACMethodInfo("", "en{'Copy user rules'}de{'Benutzerregeln kopieren'}", 603, true,
                      Description = @"Opens a dialog to copy work center permission rules from one user to another.
                                      Loads the current rules and prepares the UI for selecting source and target users.
                                      The actual copy logic should be implemented in the confirmation method.")]
        public void CopyUsernameRules()
        {

        }

        /// <summary>
        /// Opens the dialog to copy work center permission rules from one user to another.
        /// This method is intended to initiate the user interface for copying rules, allowing the selection
        /// of source and target users and confirming the copy operation. The actual logic for copying rules
        /// should be implemented in the corresponding confirmation method.
        /// </summary>
        [ACMethodInfo("", "en{'Copy'}de{'Kopieren'}", 603, true,
                      Description = @"Opens the dialog to copy work center permission rules from one user to another.
                                      This method is intended to initiate the user interface for copying rules, allowing the selection
                                      of source and target users and confirming the copy operation. The actual logic for copying rules
                                      should be implemented in the corresponding confirmation method.")]
        public void CopyUsernameOk()
        {
            ShowDialog(this, "ConfigurationDialogCopy");
        }

        /// <summary>
        /// Serializes the current temporary work center rules (_TempRules) to XML,
        /// saves them to the BSOWorkCenterSelectorRules property, commits changes to the database,
        /// displays any resulting messages, and closes the configuration dialog.
        /// </summary>
        [ACMethodInfo("", "en{'Apply rules and close'}de{'Regeln anwenden und schließen'}", 604, true,
                      Description = @"Serializes the current temporary work center rules (_TempRules) to XML,
                                      saves them to the BSOWorkCenterSelectorRules property, commits changes to the database,
                                      displays any resulting messages, and closes the configuration dialog.")]
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
                Messages.MsgAsync(msg);

            CloseTopDialog();
        }

        /// <summary>
        /// Determines whether the ApplyRulesAndClose command can be executed.
        /// Currently always returns true, allowing all users to apply work center permission rules and close the configuration dialog.
        /// This method validates if the user has permission to save the temporary work center rules to the configuration
        /// and close the rules management interface.
        /// </summary>
        /// <returns>True if the apply and close operation is enabled, otherwise false.</returns>
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
        /// <summary>
        /// Saves any pending changes in the work center selector to the database.
        /// This method delegates to the base OnSave() implementation to persist configuration changes,
        /// work center rules, and other modifications made through the selector interface.
        /// Should be called after making changes to work center assignments, rules, or configurations
        /// to ensure data persistence across sessions.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost,
                         Description = @"Saves any pending changes in the work center selector to the database.
                                         This method delegates to the base OnSave() implementation to persist configuration changes,
                                         work center rules, and other modifications made through the selector interface.
                                         Should be called after making changes to work center assignments, rules, or configurations
                                         to ensure data persistence across sessions.")]
        public void Save()
        {
            OnSave();
        }
  
        ///<summary>
        /// Determines whether the Save command can be executed.
        /// Returns true if there are pending changes in the work center selector that can be saved to the database.
        /// This method delegates to the base OnIsEnabledSave() implementation to check for modified entities
        /// and validate the current state of the business object.
        /// </summary>
        /// <returns>True if save operation is enabled and there are changes to persist, otherwise false.</returns>
        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        /// <summary>
        /// Reverts any unsaved changes made in the work center selector and restores the previous state.
        /// This method delegates to the base OnUndoSave() implementation to discard pending modifications
        /// to work center configurations, rules, and other settings without persisting them to the database.
        /// Should be called when the user wants to cancel changes and return to the last saved state.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost,
                         Description = @"Reverts any unsaved changes made in the work center selector and restores the previous state.
                                         This method delegates to the base OnUndoSave() implementation to discard pending modifications
                                         to work center configurations, rules, and other settings without persisting them to the database.
                                         Should be called when the user wants to cancel changes and return to the last saved state.")]
        public void UndoSave()
        {
            OnUndoSave();
        }

        /// <summary>
        /// Determines whether the UndoSave command can be executed.
        /// Returns true if there are pending changes in the work center selector that can be reverted to their previous state.
        /// This method delegates to the base OnIsEnabledUndoSave() implementation to check for modified entities
        /// and validate if an undo operation is possible in the current context.
        /// </summary>
        /// <returns>True if undo save operation is enabled and there are changes to revert, otherwise false.</returns>
        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }
        #endregion

        #region Public Methods
        private void _ClientManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WCFClientManager.ConnectionQuality))
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
                    Messages.ErrorAsync(this, "Can not get mapped PWGroup! ACUrl: " + pwGroupACUrl);
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

        /// <summary>
        /// Builds the collection of work center items from available process modules and their associated functions.
        /// This method initializes the WorkCenterItems collection by:
        /// 1. Loading user-specific access rules for work center permissions
        /// 2. Querying all relevant PAProcessFunction classes from the database
        /// 3. Filtering functions based on user permissions (unless user is superuser)
        /// 4. Creating WorkCenterItem instances for each unique process module
        /// 5. Adding WorkCenterItemFunction instances for each function within a module
        /// 6. Ordering the final collection by sort index and caption
        /// The method respects user access control rules defined in BSOWorkCenterSelectorRules configuration.
        /// Only process modules marked as automatic start type and containing valid business object configurations are included.
        /// </summary>
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

        /// <summary>
        /// Creates a new WorkCenterItem instance for the specified process module and work center selector.
        /// This virtual method can be overridden by derived classes to create custom WorkCenterItem implementations
        /// that provide specialized functionality or behavior for specific work center types.
        /// </summary>
        /// <param name="processModule">The ACComponent representing the process module (work center) to associate with the item.</param>
        /// <param name="workCenterSelector">The BSOWorkCenterSelector instance that manages this work center item.</param>
        /// <returns>A new WorkCenterItem instance initialized with the provided process module and selector.</returns>
        public virtual WorkCenterItem CreateWorkCenterItem(ACComponent processModule, BSOWorkCenterSelector workCenterSelector)
        {
            return new WorkCenterItem(processModule, workCenterSelector);
        }

        /// <summary>
        /// Selects the initial work center item for the work center selector.
        /// This method determines which work center should be selected when the selector is initialized:
        /// 1. By default, selects the first available work center item from the WorkCenterItems collection
        /// 2. If OnOpenSetLastSelectedWorkCenterItem is enabled, attempts to restore the previously selected work center
        /// 3. Searches for a matching work center by comparing the stored ACUrl with available work centers
        /// 4. Falls back to the first work center if the previously selected one is not found
        /// The selected work center is then set as the SelectedWorkCenterItem, which triggers the selection workflow.
        /// </summary>
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

        /// <summary>
        /// Allows derived classes to modify or extend the list of business object configurations (BSOs) 
        /// associated with a specific process automation function (PAF) when creating work center items.
        /// This virtual method is called during the work center item building process and provides an extension point
        /// for customizing which BSOs are available for a particular function within a work center.
        /// Override this method to add additional BSOs, filter existing ones, or modify the configuration
        /// based on the PAF class type and work center item context.
        /// </summary>
        /// <param name="pafACClass">The ACClass representing the process automation function (PAF) for which BSOs are being configured.</param>
        /// <param name="bsos">The current array of ACComposition objects representing the business object configurations associated with the PAF.</param>
        /// <param name="workCenterItem">The WorkCenterItem instance that will contain the function with these BSOs.</param>
        /// <returns>An array of ACComposition objects representing the final list of BSOs to be associated with the function. 
        /// The default implementation returns the input array unchanged.</returns>
        public virtual ACComposition[] OnAddFunctionBSOs(core.datamodel.ACClass pafACClass, ACComposition[] bsos, WorkCenterItem workCenterItem)
        {
            return bsos;
        }

        /// <summary>
        /// Handles action events from WPF controls, particularly tab activation events in the work center selector.
        /// This method manages the switching between different tabs and their associated content, including:
        /// - Child business objects (BSOWorkCenterChild instances)
        /// - Built-in tabs like Workflow, BOM (Bill of Materials), and Function Monitor
        /// When a tab is activated, it deactivates the current child BSO, updates the current process module if changed,
        /// activates the new child BSO or loads the appropriate content for built-in tabs, and registers for order info changes.
        /// </summary>
        /// <param name="actionArgs">Contains information about the interaction event, including the target tab content and action type</param>
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

        /// <summary>
        /// Shows or refreshes the workflow visualization for the current work center item.
        /// This method retrieves the active workflow instance from the current process module and loads it into the TaskPresenter.
        /// It first checks for accessed workflow instances using the SemaphoreAccessedFrom method on the process module.
        /// If no workflow is found or accessible, the TaskPresenter is unloaded and the method returns.
        /// If a valid workflow instance is found, the method unloads any existing workflow from the TaskPresenter
        /// and loads the parent root workflow node of the found workflow instance for display.
        /// This allows users to view and monitor the currently executing workflow associated with the selected work center.
        /// </summary>
        [ACMethodInfo("", "en{'Show/Refresh workflow'}de{'Workflow anzeigen/aktualisieren'}", 610, true,
                      Description = @"Shows or refreshes the workflow visualization for the current work center item.
                                      This method retrieves the active workflow instance from the current process module and loads it into the TaskPresenter.
                                      It first checks for accessed workflow instances using the SemaphoreAccessedFrom method on the process module.
                                      If no workflow is found or accessible, the TaskPresenter is unloaded and the method returns.
                                      If a valid workflow instance is found, the method unloads any existing workflow from the TaskPresenter
                                      and loads the parent root workflow node of the found workflow instance for display.
                                      This allows users to view and monitor the currently executing workflow associated with the selected work center.")]
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

        /// <summary>
        /// Determines whether the ShowWorkflow command can be executed.
        /// Returns true if a work center item is currently selected and it has an associated process module,
        /// which are required to display or refresh the workflow visualization for the selected work center.
        /// </summary>
        /// <returns>True if ShowWorkflow operation is enabled, otherwise false.</returns>
        public bool IsEnabledShowWorkflow()
        {
            return CurrentWorkCenterItem != null && CurrentWorkCenterItem.ProcessModule != null;
        }

        /// <summary>
        /// Loads the bill of material (parts list) for the current batch or picking operation.
        /// If a batch is selected, retrieves and filters the relevant input components from the batch's parts list relations.
        /// If a picking order is selected, retrieves the input components from the picking positions.
        /// The resulting list of input components is assigned to the InputComponentList property for display and further processing.
        /// </summary>
        [ACMethodInfo("", "en{'Load Bill of material'}de{'Load Bill of material'}", 620, true,
                      Description = @"Loads the bill of material (parts list) for the current batch or picking operation.
                                      If a batch is selected, retrieves and filters the relevant input components from the batch's parts list relations.
                                      If a picking order is selected, retrieves the input components from the picking positions.
                                      The resulting list of input components is assigned to the InputComponentList property for display and further processing.")]
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

        /// <summary>
        /// Selects and displays possible extra discharge targets for the current process workflow group.
        /// This method:
        /// - Acquires the current PWGroup in a thread-safe manner.
        /// - Initializes the routing service if not already available.
        /// - Builds routing parameters to find valid storage targets (e.g., silos, parkingspaces, intermediate bins).
        /// - Uses the routing service to find successors (possible discharge targets) for the current process module.
        /// - If targets are found, sets them to the ExtraDisTargets property and opens the selection dialog.
        /// - Handles errors if the routing service or targets are unavailable.
        /// </summary>
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
                    Messages.ErrorAsync(this, "Error50430");
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
                Messages.ErrorAsync(this, "Error50431");
                return;
            }

            ExtraDisTargets = rResult.Routes.SelectMany(c => c.GetRouteTargets()).Select(x => x.Target);

            ShowDialog(this, "ExtraDisTargetDialog");
        }

        /// <summary>
        /// Switches the current process workflow group to emptying mode using the selected extra discharge target.
        /// If a target is selected and the current workflow group is available, sets the extra discharge target
        /// and closes the dialog. This is typically used to initiate an emptying operation for the work center.
        /// </summary>
        [ACMethodInfo("", "en{'Switch to emptying'}de{'Leerfahren'}", 9999, true,
                      Description = @"Switches the current process workflow group to emptying mode using the selected extra discharge target.
                                      If a target is selected and the current workflow group is available, sets the extra discharge target
                                      and closes the dialog. This is typically used to initiate an emptying operation for the work center.")]
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

        /// <summary>
        /// Determines whether the SwitchPWGroupToEmptyingMode command can be executed.
        /// Returns true if an extra discharge target is currently selected, which is required
        /// to initiate the emptying operation for the process workflow group.
        /// </summary>
        /// <returns>True if SelectedExtraDisTarget is not null, otherwise false.</returns>
        public virtual bool IsEnabledSwitchPWGroupToEmptyingMode()
        {
            return SelectedExtraDisTarget != null;
        }

        /// <summary>
        /// Aborts all current operations in the process workflow group and switches to emptying mode using the selected extra discharge target.
        /// If no extra discharge target is selected or the current workflow group is unavailable, the method returns without action.
        /// Executes the abort and emptying operation, then closes the dialog.
        /// </summary>
        [ACMethodInfo("", "en{'Abort all and switch to emptying'}de{'Alles abbrechen und leerfahren'}", 9999, true,
                      Description = @"Aborts all current operations in the process workflow group and switches to emptying mode using the selected extra discharge target.
                                      If no extra discharge target is selected or the current workflow group is unavailable, the method returns without action.
                                      Executes the abort and emptying operation, then closes the dialog.")]
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

        /// <summary>
        /// Determines whether the AbortAllAndSwitchPWGroupToEmptyingMode command can be executed.
        /// Returns true if an extra discharge target is currently selected, which is required
        /// to initiate the abort and emptying operation for the process workflow group.
        /// </summary>
        /// <returns>True if SelectedExtraDisTarget is not null, otherwise false.</returns>
        public virtual bool IsEnabledAbortAllAndSwitchPWGroupToEmptyingMode()
        {
            return SelectedExtraDisTarget != null;
        }

        /// <summary>
        /// Opens the dialog displaying all received alarms for the current process module (work center).
        /// Retrieves the list of attached alarms using the PAProcessModule.GetAttachedAlarms method,
        /// assigns it to the ReceivedAlarmsList property, and shows the "ReceivedAlarmsDialog" UI.
        /// </summary>
        [ACMethodInfo("","",9999,
                      Description = @"Opens the dialog displaying all received alarms for the current process module (work center).
                                      Retrieves the list of attached alarms using the PAProcessModule.GetAttachedAlarms method,
                                      assigns it to the ReceivedAlarmsList property, and shows the ""ReceivedAlarmsDialog"" UI.")]
        public void ShowReceivedAlarmsDialog()
        {
            var processModule = CurrentProcessModule;

            if (processModule == null)
                return;

            MsgList receivedAlarms = processModule.ExecuteMethod(nameof(PAProcessModule.GetAttachedAlarms)) as MsgList;

            ReceivedAlarmsList = receivedAlarms;

            ShowDialog(this, "ReceivedAlarmsDialog");
        }

        /// <summary>
        /// Acknowledges the currently selected alarm for the active process module (work center).
        /// Executes the acknowledgment method on the process module using the selected alarm's message ID,
        /// removes the acknowledged alarm from the ReceivedAlarmsList, and updates the alarm list property.
        /// </summary>
        [ACMethodInfo("", "en{'Acknowledge alarm'}de{'Alarm quittieren'}", 9999,
                      Description = @"Acknowledges the currently selected alarm for the active process module (work center).
                                      Executes the acknowledgment method on the process module using the selected alarm's message ID,
                                      removes the acknowledged alarm from the ReceivedAlarmsList, and updates the alarm list property.")]
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

        /// <summary>
        /// Determines whether the AckAllReceivedAlarms command can be executed.
        /// Returns true if there are any received alarms present for the current process module (work center).
        /// This method validates if there are alarms that can be acknowledged in bulk using the acknowledge all operation.
        /// </summary>
        /// <returns>True if ReceivedAlarmsList is not null and contains alarm messages, otherwise false.</returns>
        public bool IsEnabledAckReceivedAlarm()
        {
            return SelectedReceivedAlarm != null;
        }

        /// <summary>
        /// Acknowledges all received alarms for the current process module (work center).
        /// Executes the method to acknowledge all attached alarms on the process module and clears the ReceivedAlarmsList.
        /// </summary>
        [ACMethodInfo("", "en{'Acknowledge all alarms'}de{'Alle Alarme quittieren'}", 9999,
                      Description = @"Acknowledges all received alarms for the current process module (work center).
                                      Executes the method to acknowledge all attached alarms on the process module and clears the ReceivedAlarmsList.")]
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

        /// <summary>
        /// Gets the configuration object for the last selected work center item for the current user.
        /// This method retrieves the user-specific configuration that stores which work center was last selected,
        /// allowing the system to restore the user's previous work center selection when the selector is opened again.
        /// The configuration is stored using the user's ACUrl as the key to ensure user-specific persistence.
        /// </summary>
        /// <returns>An IACConfig object containing the last selected work center configuration, or null if no configuration exists for the current user.</returns>
        public IACConfig GetLastSelectedWorkCenterItemConfig()
        {
            if (ACType == null)
                return null;

            var configs = ACType.GetConfigByKeyACUrl(Root.Environment.User.GetACUrl());
            IACConfig config = configs.FirstOrDefault();
            return config;
        }

        /// <summary>
        /// Gets the ACUrl of the last selected work center item for the current user from the configuration.
        /// This method retrieves the user-specific configuration value that stores which work center was previously selected,
        /// allowing the system to restore the user's last work center selection when the selector is reopened.
        /// Returns an empty string if no configuration exists or the configuration value is null.
        /// </summary>
        /// <returns>The ACUrl of the last selected work center item as a string, or an empty string if not found.</returns>
        public string GetLastSelectedWorkCenterItem()
        {
            IACConfig config = GetLastSelectedWorkCenterItemConfig();
            return config != null ?
                (config.Value != null ? config.Value.ToString() : "") : "";
        }

        /// <summary>
        /// Sets the configuration value that stores the last selected work center item for the current user.
        /// This method creates or updates a user-specific configuration entry that persists which work center
        /// was last selected, allowing the system to restore the user's previous selection when the selector
        /// is reopened. The configuration is stored using the user's ACUrl as the key to ensure user-specific persistence.
        /// If the work center item is null, any existing configuration entry is deleted from the database.
        /// Changes are automatically saved to the database and any resulting messages are displayed to the user.
        /// </summary>
        /// <param name="item">The WorkCenterItem to save as the last selected item, or null to remove the configuration entry.</param>

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
                Messages.MsgAsync(msg);
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
                        Messages.ErrorAsync(this, "Can not find a component with ACUrl: " + acUrl);
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

        /// <summary>
        /// Navigates to the specified work center in the work center selector.
        /// This static method finds or creates a BSOWorkCenterSelector instance and selects the work center
        /// associated with the provided ACComponent. If no selector exists, a new one is started.
        /// The method also focuses the selector in the UI if it was already running.
        /// </summary>
        [ACMethodAttached("", "en{'Navigate to work center'}de{'Navigiere zur Arbeitsplatz'}", 550, typeof(PAProcessModule), true, "", false, Global.ContextMenuCategory.ProdPlanLog,
                          Description = @"Navigates to the specified work center in the work center selector.
                                          This static method finds or creates a BSOWorkCenterSelector instance and selects the work center
                                          associated with the provided ACComponent. If no selector exists, a new one is started.
                                          The method also focuses the selector in the UI if it was already running.")]
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

        /// <summary>
        /// Determines whether navigation to a work center is enabled for the specified ACComponent.
        /// This method checks if the provided component represents a valid work center module
        /// by verifying if its ACUrl is contained in the static WorkCenterModules collection.
        /// The WorkCenterModules collection contains ACUrl components of all process modules
        /// that have relevant PAProcessFunction classes with business object configurations.
        /// </summary>
        /// <param name="acComponent">The ACComponent to check for work center navigation capability. 
        /// If null, the method returns false.</param>
        /// <returns>True if the component is a valid work center module that can be navigated to, 
        /// otherwise false. Returns false if the acComponent parameter is null.</returns>
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
