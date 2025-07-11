// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.bso.iplus;
using gip.bso.masterdata;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.media;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static gip.core.datamodel.Global;

namespace gip.mes.maintenance
{
    /// <summary>
    /// Businessobject or App for managing the Maintenance plans.
    /// Open Maintenance plans can be lisend in MaintOrderList property. 
    /// Call the NavigateFirst() method to set CurrentMaintOrder with the first record in the list. 
    /// CurrentMaintOrder is used to display and edit the currently selected record. 
    /// Property changes should always be made to CurrentMaintOrder and when all field values ​​have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record. 
    /// Fill in all required fields before saving. Use the Delete() method to delete the material provided there are no foreign key relationships from other tables. 
    /// Always call the Save() method after calling Delete() to execute the delete operation in the database.
    /// The Load method updates the CurrentMaterial object with fresh database data if another user has made changes in the background.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOMaintOrderTemplate&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(
    Const.PackName_VarioAutomation,
    "en{'Maintenance plan'}de{'Wartungsplan'}",
    Global.ACKinds.TACBSOGlobal,
    Global.ACStorableTypes.NotStorable,
    true,
    true,
    Const.QueryPrefix + MaintOrder.ClassName,
    Description = @"Businessobject or App for managing the Maintenance orders.
    Open Maintenance orders can be lisend in MaintOrderList property. 
    Call the NavigateFirst() method to set CurrentMaintOrder with the first record in the list. 
    CurrentMaintOrder is used to display and edit the currently selected record. 
    Property changes should always be made to CurrentMaintOrder and when all field values ​​have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record. 
    Fill in all required fields before saving. Use the Delete() method to delete the material provided there are no foreign key relationships from other tables. 
    Always call the Save() method after calling Delete() to execute the delete operation in the database.
    The Load method updates the CurrentMaterial object with fresh database data if another user has made changes in the background.
    Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOMaintOrderTemplate&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name."
    )]
    public class BSOMaintOrderTemplate : BSOMaintOrderBase
    {
        #region c'tors

        public BSOMaintOrderTemplate(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool result = base.ACInit(startChildMode);
            AttachToMaintServices();
            Search();
            return result;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_SelectionManager != null)
            {
                _SelectionManager.Detach();
                _SelectionManager.ObjectDetaching -= _SelectionManager_ObjectDetaching;
                _SelectionManager.ObjectAttached -= _SelectionManager_ObjectAttached;
                _SelectionManager = null;
            }

            if (_CurrentACComponent != null)
            {
                _CurrentACComponent.Detach();
                _CurrentACComponent = null;
            }

            if (BSOFacilityExplorer_Child != null && BSOFacilityExplorer_Child.Value != null)
            {
                BSOFacilityExplorer_Child.Value.PropertyChanged -= FacilityExplorer_PropertyChanged;
            }

            DetachMaintServices();

            return base.ACDeInit(deleteACClassTask);
        }

        public override bool ACPostInit()
        {
            if (SelectionManager != null)
            {
                SelectionManager.PropertyChanged += SelectionManager_PropertyChanged;
                if ((this.SelectionManager as VBBSOSelectionManager).ShowACObjectForSelection != null)
                {
                    CurrentACComponent = (this.SelectionManager as VBBSOSelectionManager).ShowACObjectForSelection;
                }
            }

            if (BSOFacilityExplorer_Child != null && BSOFacilityExplorer_Child.Value != null)
            {
                BSOFacilityExplorer_Child.Value.PropertyChanged += FacilityExplorer_PropertyChanged;
            }

            return base.ACPostInit();
        }

        public const string Const_FacilityExplorer = "BSOFacilityExplorer_Child";
        public const string Const_ComponentSelector = "BSOComponentSelector_Child";

        #endregion

        #region Properties

        private List<ACRef<ACComponent>> _MaintServices = null;
        public List<ACRef<ACComponent>> MaintServices
        {
            get
            {
                return _MaintServices;
            }
        }

        #region Properties => Facility

        private ACChildItem<BSOFacilityExplorer> _BSOFacilityExplorer_Child;
        [ACPropertyInfo(600)]
        [ACChildInfo("BSOFacilityExplorer_Child", typeof(BSOFacilityExplorer))]
        public ACChildItem<BSOFacilityExplorer> BSOFacilityExplorer_Child
        {
            get
            {
                if (_BSOFacilityExplorer_Child == null)
                    _BSOFacilityExplorer_Child = new ACChildItem<BSOFacilityExplorer>(this, Const_FacilityExplorer);
                return _BSOFacilityExplorer_Child;
            }
        }

        #endregion

        #region Properties => MaintACClass

        #region ComponentSelector

        //private ACChildItem<BSOComponentSelector> _BSOComponentSelector_Child;
        //[ACPropertyInfo(600)]
        //[ACChildInfo("BSOComponentSelector_Child", typeof(BSOComponentSelector))]
        //public ACChildItem<BSOComponentSelector> BSOComponentSelector_Child
        //{
        //    get
        //    {
        //        if (_BSOComponentSelector_Child == null)
        //            _BSOComponentSelector_Child = new ACChildItem<BSOComponentSelector>(this, Const_ComponentSelector);
        //        return _BSOComponentSelector_Child;
        //    }
        //}

        //[ACPropertyInfo(600)]
        //public BSOComponentSelector BSOCompSelector
        //{
        //    get => BSOComponentSelector_Child?.Value;
        //}

        private ACClassInfoWithItems.VisibilityFilters _ComponentTypeFilter;
        [ACPropertyInfo(999)]
        public ACClassInfoWithItems.VisibilityFilters ComponentTypeFilter
        {
            get
            {
                if (_ComponentTypeFilter == null)
                    _ComponentTypeFilter = new ACClassInfoWithItems.VisibilityFilters() { IncludeTypes = new List<Type> { typeof(PAClassAlarmingBase), typeof(ApplicationManager) } };
                return _ComponentTypeFilter;
            }
        }

        public override IAccessNav AccessNav { get { return AccessPrimary; } }

        public override List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {
                List<ACFilterItem> aCFilterItems = new List<ACFilterItem>();

                ACFilterItem basedOnFilter = new ACFilterItem(Global.FilterTypes.filter, nameof(MaintOrder.MaintOrder1_BasedOnMaintOrder), Global.LogicalOperators.equal, Global.Operators.and, null, true);
                aCFilterItems.Add(basedOnFilter);

                ACFilterItem maintOrderNoFilter = new ACFilterItem(FilterTypes.filter, nameof(MaintOrder.MaintOrderNo), LogicalOperators.contains, Operators.and, null, true, true);
                aCFilterItems.Add(maintOrderNoFilter);

                return aCFilterItems;
            }
        }

        #endregion

        #endregion

        #region Selection
        private ACRef<VBBSOSelectionManager> _SelectionManager;
        public VBBSOSelectionManager SelectionManager
        {
            get
            {
                if (_SelectionManager != null)
                    return _SelectionManager.ValueT;
                if (ParentACComponent != null)
                {
                    VBBSOSelectionManager subACComponent = ParentACComponent.GetChildComponent(SelectionManagerACName) as VBBSOSelectionManager;
                    if (subACComponent == null)
                    {
                        if (ParentACComponent is VBBSOSelectionDependentDialog)
                        {
                            subACComponent = (ParentACComponent as VBBSOSelectionDependentDialog).SelectionManager;
                        }
                        else
                            subACComponent = ParentACComponent.StartComponent(SelectionManagerACName, null, null) as VBBSOSelectionManager;
                    }
                    if (subACComponent != null)
                    {
                        _SelectionManager = new ACRef<VBBSOSelectionManager>(subACComponent, this);
                        _SelectionManager.ObjectDetaching += new EventHandler(_SelectionManager_ObjectDetaching);
                        _SelectionManager.ObjectAttached += new EventHandler(_SelectionManager_ObjectAttached);
                    }
                }
                if (_SelectionManager == null)
                    return null;
                return _SelectionManager.ValueT;
            }
        }

        void _SelectionManager_ObjectAttached(object sender, EventArgs e)
        {
            if (SelectionManager != null)
                SelectionManager.PropertyChanged += SelectionManager_PropertyChanged;
        }

        void _SelectionManager_ObjectDetaching(object sender, EventArgs e)
        {
            if (SelectionManager != null)
                SelectionManager.PropertyChanged -= SelectionManager_PropertyChanged;
        }

        private string SelectionManagerACName
        {
            get
            {
                string acInstance = ACUrlHelper.ExtractInstanceName(this.ACIdentifier);
                if (String.IsNullOrEmpty(acInstance))
                    return "VBBSOSelectionManager";
                else
                    return "VBBSOSelectionManager(" + acInstance + ")";
            }
        }

        void SelectionManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectionManager.ShowACObjectForSelection))
            {
                CurrentACComponent = SelectionManager.ShowACObjectForSelection;
            }
        }

        ACRef<IACObject> _CurrentACComponent;
        /// <summary>
        /// Gets or sets the current ACComponent (current component for the maintenance rule configuration).
        /// </summary>
        /// <summary xml:lang="de">
        /// Ruft die aktuelle ACComponent (aktuelle Komponente für die Konfiguration der Wartungsregel) ab oder setzt sie.
        /// </summary>
        [ACPropertyInfo(9999)]
        public IACObject CurrentACComponent
        {
            get
            {
                if (_CurrentACComponent == null)
                    return null;
                return _CurrentACComponent.ValueT;
            }
            set
            {
                bool objectSwapped = true;
                if (_CurrentACComponent != null)
                {
                    if (_CurrentACComponent != value)
                    {
                        _CurrentACComponent.Detach();
                    }
                    else
                        objectSwapped = false;
                }
                if (value == null)
                    _CurrentACComponent = null;
                else
                    _CurrentACComponent = new ACRef<IACObject>(value, this);
                if (_CurrentACComponent != null)
                {
                    if (objectSwapped)
                    {
                        OnSelectionChanged();
                    }
                }
                else
                {
                    OnSelectionChanged();
                }
                OnPropertyChanged();
            }
        }
        #endregion

        public override MaintOrder CurrentMaintOrder 
        { 
            get => base.CurrentMaintOrder;
            set
            {
                base.CurrentMaintOrder = value;

                if (value != null)
                {
                    MaintACClassPropertyList = value.MaintACClass?.MaintACClassProperty_MaintACClass
                                                                  .Where(c => c.IsActive)
                                                                  .Select(c => new MaintACClassPropertyWrapper() { MaintACClassProperty = c, ACClassProperty = c.ACClassProperty })
                                                                  .ToList();

                    SearchProperties();
                }
                else
                {
                    MaintACClassPropertyList = null;
                    ACClassPropertyList = null;
                }
            }
        }

        private MaintenanceWizzardStepsEnum _CurrentWizzardStep;
        [ACPropertyInfo(9999)]
        public MaintenanceWizzardStepsEnum CurrentWizzardStep
        {
            get => _CurrentWizzardStep;
            set
            {
                _CurrentWizzardStep = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the ACClassProperty filter (search keyword).
        /// </summary>
        /// <summary xml:lang="de">
        /// Ruft den ACClassProperty-Filter (Suchbegriff) ab oder setzt ihn.
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Search Keyword'}de{'Suchwort'}", DefaultValue = "Statistics")]
        public string ACClassPropertyFilter
        {
            get;
            set;
        }

        public ACValueItem _CurrentFilterMode;
        /// <summary>
        /// Gets or sets the current filter mode (the filter mode for the events rule configuration).
        /// </summary>
        /// <summary xml:lang="de">
        /// Ruft den aktuellen Filtermodus ab oder setzt ihn (den Filtermodus für die Konfiguration der Ereignisregel).
        /// </summary>
        [ACPropertyCurrent(9999, "FilterMode", "en{'Filter Mode'}de{'Filtermodus'}")]
        public ACValueItem CurrentFilterMode
        {
            get
            {
                if (_CurrentFilterMode == null)
                    _CurrentFilterMode = FilterModeList.FirstOrDefault();
                return _CurrentFilterMode;
            }
            set
            {
                _CurrentFilterMode = value;
                OnPropertyChanged("CurrentFilterMode");
            }
        }

        private ACValueItemList _FilterModeList;
        /// <summary>
        /// Gets the list of filter modes.
        /// </summary>
        /// <summary xml:lang="de">
        /// Ruft die Liste der Filtermodi ab.
        /// </summary>
        [ACPropertyList(9999, "FilterMode")]
        public ACValueItemList FilterModeList
        {
            get
            {
                if (_FilterModeList == null)
                {
                    _FilterModeList = new ACValueItemList("FilterMode");
                    _FilterModeList.Add(new ACValueItem("en{'By Group'}de{'Nach Gruppe'}", "Group", null));
                    _FilterModeList.Add(new ACValueItem("en{'By Property Name'}de{'Nach Eigenschaft'}", "PropertyName", null));
                }
                return _FilterModeList;
            }
        }

        private core.datamodel.ACClassProperty _SelectedACClassProperty;
        /// <summary>
        /// Gets or sets the selected ACClassProperty.
        /// </summary>
        /// <summary xml:lang="de">
        /// Ruft die ausgewählte ACClassProperty ab oder setzt sie.
        /// </summary>
        [ACPropertySelected(9999, "ACClassProperty")]
        public core.datamodel.ACClassProperty SelectedACClassProperty
        {
            get
            {
                return _SelectedACClassProperty;
            }
            set
            {
                _SelectedACClassProperty = value;
                OnPropertyChanged("SelectedACClassProperty");
            }
        }

        private List<core.datamodel.ACClassProperty> _ACClassPropertyList;
        /// <summary>
        /// Gets or sets the list of ACClassProperties (Available properties)
        /// </summary>
        /// <summary xml:lang="de">
        /// Ruft die Liste der ACClassProperties (Available properties) ab oder setzt sie.
        /// </summary>
        [ACPropertyList(9999, "ACClassProperty")]
        public List<core.datamodel.ACClassProperty> ACClassPropertyList
        {
            get
            {
                return _ACClassPropertyList;
            }
            set
            {
                _ACClassPropertyList = value;
                OnPropertyChanged("ACClassPropertyList");
            }
        }

        private MaintACClassPropertyWrapper _CurrentMaintACClassProperty;
        /// <summary>
        /// Gets or sets the current maintenance ACClass.
        /// </summary>
        /// <summary xml:lang="de">
        /// Ruft die aktuelle Wartung der ACClassProperty ab oder setzt sie.
        /// </summary>
        [ACPropertyCurrent(9999, "MaintACClassProperty", "en{'Maintenance Property'}de{'Wartungseigenschaft'}")]
        public MaintACClassPropertyWrapper CurrentMaintACClassProperty
        {
            get
            {
                return _CurrentMaintACClassProperty;
            }
            set
            {
                _CurrentMaintACClassProperty = value;
                OnPropertyChanged();
            }
        }

        private List<MaintACClassPropertyWrapper> _MaintACClassProprtyList;
        /// <summary>
        /// Gets or sets the list of maintenance ACClassProperty.
        /// </summary>
        /// <summary xml:lang="de">
        /// Ruft die Wartungsliste ACClassProperty ab oder setzt sie.
        /// </summary>
        [ACPropertyList(9999, "MaintACClassProperty")]
        public List<MaintACClassPropertyWrapper> MaintACClassPropertyList
        {
            get
            {
                return _MaintACClassProprtyList;
            }
            set
            {
                _MaintACClassProprtyList = value;
                OnPropertyChanged();
            }
        }

        #region Properties => Assignment

        private core.datamodel.VBUser _SelectedVBUser;
        [ACPropertySelected(604, "VBUser", "en{'User'}de{'Benutzer'}")]
        public core.datamodel.VBUser SelectedVBUser
        {
            get => _SelectedVBUser;
            set
            {
                if (value != null)
                {
                    SelectedVBGroup = null;
                    SelectedCompany = null;
                }

                _SelectedVBUser = value;
                OnPropertyChanged(nameof(SelectedVBUser));
            }
        }

        private core.datamodel.VBUser[] _VBUserList;
        [ACPropertyList(605, "VBUser")]
        public IEnumerable<core.datamodel.VBUser> VBUserList
        {
            get
            {
                if (_VBUserList == null)
                {
                    using (ACMonitor.Lock(DatabaseApp.ContextIPlus.QueryLock_1X000))
                    {
                        _VBUserList = DatabaseApp.ContextIPlus.VBUser.OrderBy(c => c.VBUserName).ToArray();
                    }
                }
                return _VBUserList;
            }
        }

        private core.datamodel.VBGroup _SelectedVBGroup;
        [ACPropertySelected(604, "VBGroup", "en{'Group'}de{'Gruppe'}")]
        public core.datamodel.VBGroup SelectedVBGroup
        {
            get => _SelectedVBGroup;
            set
            {
                if (value != null)
                {
                    SelectedVBUser = null;
                    SelectedCompany = null;
                }

                _SelectedVBGroup = value;
                OnPropertyChanged(nameof(SelectedVBUser));
            }
        }

        private core.datamodel.VBGroup[] _VBGroupList;
        [ACPropertyList(605, "VBGroup")]
        public IEnumerable<core.datamodel.VBGroup> VBGroupList
        {
            get
            {
                if (_VBGroupList == null)
                {
                    using (ACMonitor.Lock(DatabaseApp.ContextIPlus.QueryLock_1X000))
                    {
                        _VBGroupList = DatabaseApp.ContextIPlus.VBGroup.OrderBy(c => c.VBGroupName).ToArray();
                    }
                }
                return _VBGroupList;
            }
        }

        private Company _SelectedCompany;
        [ACPropertySelected(9999, "Company", "")]
        public Company SelectedCompany
        {
            get => _SelectedCompany;
            set
            {
                if (value != null)
                {
                    SelectedVBUser = null;
                    SelectedVBGroup = null;
                }

                _SelectedCompany = value;
                OnPropertyChanged();
            }

        }

        private Company[] _CompanyList;
        [ACPropertyList(605, "Company")]
        public IEnumerable<Company> CompanyList
        {
            get
            {
                if (_CompanyList == null)
                {
                    using (ACMonitor.Lock(DatabaseApp.ContextIPlus.QueryLock_1X000))
                    {
                        _CompanyList = DatabaseApp.Company.OrderBy(c => c.CompanyName).ToArray();
                    }
                }
                return _CompanyList;
            }
        }



        #endregion

        ACChildItem<BSOMedia> _BSOMedia_Child;
        [ACPropertyInfo(9999)]
        [ACChildInfo("BSOMedia_Child", typeof(BSOMedia))]
        public ACChildItem<BSOMedia> BSOMedia_Child
        {
            get
            {
                if (_BSOMedia_Child == null)
                    _BSOMedia_Child = new ACChildItem<BSOMedia>(this, "BSOMedia_Child");
                return _BSOMedia_Child;
            }
        }

        private ACBSO _ComponentSelector;
        public ACBSO ComponentSelector
        {
            get
            {
                if (_ComponentSelector == null)
                    _ComponentSelector = GetChildComponent("BSOComponentSelectorMaint") as ACBSO;
                return _ComponentSelector;
            }
        }

        #endregion

        #region Methods

        protected override IQueryable<MaintOrder> _AccessPrimary_NavSearchExecuting(IQueryable<MaintOrder> result)
        {
            return result.Where(c => !c.BasedOnMaintOrderID.HasValue);
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(nameof(MaintOrder), Const.New, (short)MISort.New, true, "SelectedMaintOrder", Global.ACKinds.MSMethodPrePost)]
        public virtual void New()
        {
            if (!PreExecute("New"))
                return;
            if (AccessPrimary == null)
                return;

            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(MaintOrder), nameof(MaintOrder.MaintOrderNo), MaintOrder.FormatNewNoTemplate, this);
            CurrentMaintOrder = MaintOrder.NewACObject(DatabaseApp, null, secondaryKey);
            DatabaseApp.MaintOrder.Add(CurrentMaintOrder);
            ACState = Const.SMNew;
            CurrentWizzardStep = MaintenanceWizzardStepsEnum.FacilityOrClass;

            ShowDialog(this, "NewTemplateDialog");

            PostExecute("New");
        }

        [ACMethodInfo("","en{'New template'}de{'Neue vorlage'}",9999)]
        public void NewTemplate()
        {
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(MaintOrder), nameof(MaintOrder.MaintOrderNo), MaintOrder.FormatNewNoTemplate, this);
            CurrentMaintOrder = MaintOrder.NewACObject(DatabaseApp, null, secondaryKey);
            DatabaseApp.MaintOrder.Add(CurrentMaintOrder);
            ACState = Const.SMNew;

            mes.datamodel.ACClass tempACClass = null;
            using (ACMonitor.Lock(Database.QueryLock_1X000))
            {
                tempACClass = (CurrentACComponent?.ACType as core.datamodel.ACClass)?.FromAppContext<mes.datamodel.ACClass>(DatabaseApp);
            }

            //CurrentMaintOrder.TempACClass = tempACClass.FromIPlusContext<core.datamodel.ACClass>(Database.ContextIPlus);
            MaintACClass maintACClass = MaintACClass.NewACObject(DatabaseApp, tempACClass);
            DatabaseApp.MaintACClass.Add(maintACClass);
            CurrentMaintOrder.MaintACClass = maintACClass;
            SearchProperties();

            CurrentWizzardStep = MaintenanceWizzardStepsEnum.TimeEvent;

            ShowDialog(this, "NewTemplateDialog");
        }

        public bool IsEnabledNewTemplate()
        {
            return CurrentACComponent != null;
        }

        [ACMethodCommand(nameof(MaintOrder), Const.Delete, (short)MISort.Delete, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void Delete()
        {
            MaintOrderTask[] tempTasksList = SelectedMaintOrder.MaintOrderTask_MaintOrder.ToArray();
            foreach (MaintOrderTask task in tempTasksList)
            {
                DatabaseApp.Remove(task);
            }

            MaintOrderAssignment[] tempAssignmentList = SelectedMaintOrder.MaintOrderAssignment_MaintOrder.ToArray();
            foreach (MaintOrderAssignment assignment in tempAssignmentList)
            {
                DatabaseApp.Remove(assignment);
            }
           

            DatabaseApp.Remove(SelectedMaintOrder);
            DatabaseApp.ACSaveChanges();
            AccessPrimary.NavList.Remove(SelectedMaintOrder);
            OnPropertyChanged(nameof(MaintOrderList));
        }

        public bool IsEnabledDelete()
        {
            return SelectedMaintOrder != null;
        }

        /// <summary>
        /// Determines whether [is enabled new].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new]; otherwise, <c>false</c>.</returns>
        public virtual bool IsEnabledNew()
        {
            return true;
        }

        protected override void OnPostSave()
        {
            base.OnPostSave();
            RefreshOnSave();
        }

        [ACMethodInfo("", "en{'Next'}de{'Weiter'}", 9999, true)]
        public void WizzardNext()
        {
            if (CurrentWizzardStep == MaintenanceWizzardStepsEnum.FacilityOrClass)
            {
                BSOFacilityExplorer facilityExpl = FindChildComponents<BSOFacilityExplorer>(c => c is BSOFacilityExplorer).OrderByDescending(c => c.ACIdentifier).FirstOrDefault();
                if (facilityExpl != null && facilityExpl.SelectedFacility != null)
                {
                    CurrentMaintOrder.Facility = facilityExpl.SelectedFacility;
                }
                else
                {
                    BSOComponentSelector compSel = FindChildComponents<BSOComponentSelector>(c => c is BSOComponentSelector).FirstOrDefault();
                    if (compSel != null && compSel.CurrentProjectItemCS != null)
                    {
                        mes.datamodel.ACClass tempACClass = compSel.CurrentProjectItemCS.ValueT.FromAppContext<mes.datamodel.ACClass>(DatabaseApp);
                        //CurrentMaintOrder.TempACClass = compSel.CurrentProjectItemCS.ValueT;
                        MaintACClass maintACClass = MaintACClass.NewACObject(DatabaseApp, tempACClass);
                        DatabaseApp.MaintACClass.Add(maintACClass);
                        CurrentMaintOrder.MaintACClass = maintACClass;
                    }    
                }

                if (CurrentMaintOrder != null && CurrentMaintOrder.MaintACClass == null && CurrentMaintOrder.Facility == null)
                    return;

                SearchProperties();

                CurrentWizzardStep = MaintenanceWizzardStepsEnum.TimeEvent;
            }
            else if (CurrentWizzardStep == MaintenanceWizzardStepsEnum.TimeEvent)
            {
                if (CurrentMaintOrder != null)
                {
                    SelectedMaintOrderTask = CurrentMaintOrder.MaintOrderTask_MaintOrder.FirstOrDefault();
                    if (SelectedMaintOrderTask == null)
                    {
                        AddNewTask();
                    }
                }

                CurrentWizzardStep = MaintenanceWizzardStepsEnum.Task;
            }

            else if (CurrentWizzardStep == MaintenanceWizzardStepsEnum.Task)
            {
                CurrentWizzardStep = MaintenanceWizzardStepsEnum.Performer;
            }
            else if (CurrentWizzardStep == MaintenanceWizzardStepsEnum.Performer)
            {
                Msg msg = DatabaseApp.ACSaveChanges();
                if (msg != null)
                {
                    Messages.Msg(msg);
                    return;
                }

                CloseTopDialog();

                AccessPrimary.NavList.Add(CurrentMaintOrder);
                OnPropertyChanged(nameof(MaintOrderList));
                RefreshOnSave();
            }
        }

        public bool IsEnabledWizzardNext()
        {
            return true;
        }

        [ACMethodInfo("", "en{'Back'}de{'Zurück'}", 9999, true)]
        public void WizzardBack()
        {
            if (CurrentWizzardStep == MaintenanceWizzardStepsEnum.TimeEvent)
            {

                CurrentWizzardStep = MaintenanceWizzardStepsEnum.FacilityOrClass;
            }
            else if (CurrentWizzardStep == MaintenanceWizzardStepsEnum.Task)
            {

                CurrentWizzardStep = MaintenanceWizzardStepsEnum.TimeEvent;
            }
            else if (CurrentWizzardStep == MaintenanceWizzardStepsEnum.Performer)
            {
                CurrentWizzardStep = MaintenanceWizzardStepsEnum.Task;
            }
        }

        public bool IsEnabledWizzardBack()
        {
            return true;
        }

        [ACMethodInfo("", "en{'Close'}de{'Schließen'}", 9999, true)]
        public void CloseWizzard()
        {
            DatabaseApp.ACUndoChanges();
            CloseTopDialog();
        }

        public List<core.datamodel.ACClassProperty> FindACClassProperties(core.datamodel.ACClass acClass)
        {
            List<core.datamodel.ACClassProperty> tempList = new List<core.datamodel.ACClassProperty>();
            core.datamodel.ACClass tempACClass = acClass;

            if (CurrentFilterMode.Value.ToString() == "Group")
            {
                while (tempACClass != null)
                {
                    IEnumerable<core.datamodel.ACClassProperty> properties = tempACClass.ACClassProperty_ACClass.Where(c => !String.IsNullOrEmpty(c.ACGroup)
                                                                                                        && !c.IsStatic
                                                                                                        && c.ACGroup.Equals(ACClassPropertyFilter, StringComparison.OrdinalIgnoreCase)
                                                                                                        && (MaintACClassPropertyList == null
                                                                                                            || !MaintACClassPropertyList.Any(x => c.ACIdentifier == x.ACClassProperty.ACIdentifier
                                                                                                                                                 && x.MaintACClassProperty.IsActive)));
                    if (properties != null)
                    {
                        tempList.AddRange(properties);
                    }
                    tempACClass = tempACClass.ACClass1_BasedOnACClass;
                }
            }
            else if (CurrentFilterMode.Value.ToString() == "PropertyName")
            {
                while (tempACClass != null)
                {
                    IEnumerable<core.datamodel.ACClassProperty> properties = tempACClass.ACClassProperty_ACClass.Where(c => !c.IsStatic
                                                                                                            && ((!String.IsNullOrEmpty(c.ACIdentifier) && c.ACIdentifier.Equals(ACClassPropertyFilter, StringComparison.OrdinalIgnoreCase))
                                                                                                                || (!String.IsNullOrEmpty(c.ACCaption) && c.ACCaption.Equals(ACClassPropertyFilter, StringComparison.OrdinalIgnoreCase)))
                                                                                                            && (MaintACClassPropertyList == null
                                                                                                                || !MaintACClassPropertyList.Any(x => c.ACIdentifier == x.ACClassProperty.ACIdentifier
                                                                                                                                                        && x.MaintACClassProperty.IsActive)));
                    if (properties != null)
                    {
                        tempList.AddRange(properties);
                    }
                    tempACClass = tempACClass.ACClass1_BasedOnACClass;
                }
            }
            return tempList;
        }

        #region Methods => AvailableProperties

        /// <summary>
        /// Searches the available MaintACClass properties. 
        /// </summary>
        /// <summary xml:lang="de">
        /// Sucht die verfügbaren MaintACClass-Eigenschaften. 
        /// </summary>
        [ACMethodInfo("", "en{'Search'}de{'Suchen'}", 999)]
        public void SearchProperties()
        {
            core.datamodel.ACClass tempACClass = CurrentMaintOrder.MaintACClass?.VBiACClass.FromIPlusContext<core.datamodel.ACClass>(Database.ContextIPlus);
            if (tempACClass != null)
                ACClassPropertyList = FindACClassProperties(tempACClass);
            else
                ACClassPropertyList = null;
        }

        public bool IsEnabledSearchProperties()
        {
            if (CurrentMaintOrder != null && CurrentMaintOrder.MaintACClass != null)
                return true;

            return false;
        }

        /// <summary>
        /// Adds the selected maintenance rule property to the assigned properties. 
        /// </summary>
        /// <summary xml:lang="de">
        /// Fügt den zugeordneten Eigenschaften die Eigenschaft der ausgewählten Wartungsregel hinzu. 
        /// </summary>
        [ACMethodInfo("", "", 999)]
        public void AddPropertyToMaintACClass()
        {
            MaintACClass maintACClass = CurrentMaintOrder?.MaintACClass;

            MaintACClassProperty maintACClassProperty = maintACClass.MaintACClassProperty_MaintACClass.FirstOrDefault(c => c.VBiACClassProperty.ACIdentifier == SelectedACClassProperty.ACIdentifier);
            if (maintACClassProperty == null)
            {
                maintACClassProperty = MaintACClassProperty.NewACObject(DatabaseApp, maintACClass);
                DatabaseApp.MaintACClassProperty.Add(maintACClassProperty);
                maintACClassProperty.VBiACClassProperty = DatabaseApp.ACClassProperty.FirstOrDefault(c => c.ACClassPropertyID == SelectedACClassProperty.ACClassPropertyID);
                maintACClass.MaintACClassProperty_MaintACClass.Add(maintACClassProperty);
            }
            maintACClassProperty.IsActive = true;
            if (MaintACClassPropertyList == null)
                MaintACClassPropertyList = new List<MaintACClassPropertyWrapper>();
            MaintACClassPropertyList.Add(new MaintACClassPropertyWrapper() { MaintACClassProperty = maintACClassProperty, ACClassProperty = SelectedACClassProperty });
            MaintACClassPropertyList = MaintACClassPropertyList.ToList();
            ACClassPropertyList.Remove(SelectedACClassProperty);
            ACClassPropertyList = ACClassPropertyList.ToList();
        }

        public bool IsEnabledAddPropertyToMaintACClass()
        {
            if (SelectedACClassProperty != null)
                return true;
            return false;
        }

        /// <summary>
        /// Removes the selected MaintACClass property from the assigned properties. 
        /// </summary>
        /// <summary xml:lang="de">
        /// Entfernt die ausgewählte MaintACClass-Eigenschaft aus den zugewiesenen Eigenschaften.
        /// </summary>
        [ACMethodInfo("", "", 999)]
        public void RemovePropertyFromMaintACClass()
        {
            CurrentMaintACClassProperty.MaintACClassProperty.IsActive = false;
            ACClassPropertyList.Add(CurrentMaintACClassProperty.ACClassProperty);
            MaintACClassPropertyList.Remove(CurrentMaintACClassProperty);
            MaintACClassPropertyList = MaintACClassPropertyList.ToList();
            ACClassPropertyList = ACClassPropertyList.ToList();
        }

        public bool IsEnabledRemovePropertyFromMaintACClass()
        {
            if (CurrentMaintACClassProperty != null)
                return true;
            return false;
        }

        #endregion

        [ACMethodInfo("", "en{'Add task'}de{'Aufgabe hinzufügen'}", 9999, true)]
        public void AddNewTask()
        {
            SelectedMaintOrderTask = MaintOrderTask.NewACObject(DatabaseApp, CurrentMaintOrder);
            this.DatabaseApp.MaintOrderTask.Add(SelectedMaintOrderTask);

            List<MaintOrderTask> tempList = CurrentMaintOrder.MaintOrderTask_MaintOrder.ToList();

            MaintOrderTaskList = tempList;
        }

        [ACMethodInfo("", "en{'Remove task'}de{'Aufgabe entfernen'}", 9999, true)]
        public void RemoveTask()
        {
            MaintOrderTaskList.Remove(SelectedMaintOrderTask);
            SelectedMaintOrderTask.DeleteACObject(DatabaseApp, true);
            MaintOrderTaskList = MaintOrderTaskList.ToList();
            SelectedMaintOrderTask = null;
        }

        #region Methods => Assignment

        [ACMethodInfo("", "en{'Add performer'}de{'Darsteller hinzufügen'}", 9999, true)]
        public void AddNewMaintAssignment()
        {
            SelectedMaintOrderAssignment = MaintOrderAssignment.NewACObject(DatabaseApp, CurrentMaintOrder);
            DatabaseApp.MaintOrderAssignment.Add(SelectedMaintOrderAssignment);
            List<MaintOrderAssignment> tempList = CurrentMaintOrder.MaintOrderAssignment_MaintOrder.ToList();

            MaintOrderAssignmentList = tempList;

            if (SelectedVBUser != null)
            {
                SelectedMaintOrderAssignment.VBUser = SelectedVBUser.FromAppContext<mes.datamodel.VBUser>(DatabaseApp);
            }
            else if (SelectedVBGroup != null)
            {
                SelectedMaintOrderAssignment.VBGroup = SelectedVBGroup.FromAppContext<mes.datamodel.VBGroup>(DatabaseApp);
            }
            else if (SelectedCompany != null)
            {
                SelectedMaintOrderAssignment.Company = SelectedCompany;
            }

            SelectedMaintOrderAssignment.IsActive = true;
        }

        public bool IsEnabledAddNewMaintAssignment()
        {
            if (SelectedVBUser == null && SelectedVBGroup == null && SelectedCompany == null)
                return false;

            return true;
        }

        [ACMethodInfo("", "en{'Remove performer'}de{'Darsteller entfernen'}", 9999, true)]
        public void RemoveMaintAssignment()
        {
            MaintOrderAssignmentList.Remove(SelectedMaintOrderAssignment);
            SelectedMaintOrderAssignment.DeleteACObject(DatabaseApp, true);
            MaintOrderAssignmentList = MaintOrderAssignmentList.ToList();

            SelectedMaintOrderAssignment = null;

        }

        public bool IsEnabledRemoveMaintAssignment()
        {
            return SelectedMaintOrderAssignment != null;
        }

        [ACMethodInfo("", "en{'Unselect'}de{'Auswahl aufheben'}", 9999, true)]
        public void UnselectPerformer()
        {
            SelectedVBUser = null;
            SelectedCompany = null;
            SelectedVBGroup = null;
        }

        #endregion

        protected virtual void OnSelectionChanged()
        {
            core.datamodel.ACClass selectedClass = CurrentACComponent != null ? CurrentACComponent.ACType as core.datamodel.ACClass : null;
            
            if (selectedClass != null)
            {
                List<MaintOrder> templates = new List<MaintOrder>();
                LoadTemplates(selectedClass, templates);

                if (templates.Any())
                {
                    AccessPrimary.ToNavList(templates);
                    OnPropertyChanged(nameof(MaintOrderList));
                    return;
                }

                var hierarchy = selectedClass.ClassHierarchy.ToArray();
                foreach(core.datamodel.ACClass acClass in hierarchy)
                {
                    LoadTemplates(acClass, templates);

                    if (templates.Any())
                    {
                        AccessPrimary.ToNavList(templates);
                        OnPropertyChanged(nameof(MaintOrderList));
                        return;
                    }

                }

                if (!templates.Any())
                {
                    AccessPrimary.ToNavList(templates);
                    OnPropertyChanged(nameof(MaintOrderList));
                    CurrentMaintOrder = null;
                }
            }
        }

        private void LoadTemplates(core.datamodel.ACClass acClass, List<MaintOrder> templates)
        {
            datamodel.ACClass acClassApp = acClass.FromAppContext<datamodel.ACClass>(DatabaseApp);
            foreach (MaintACClass maintACClass in acClassApp.MaintACClass_VBiACClass)
            {
                var templ = maintACClass.MaintOrder_MaintACClass.Where(c => c.BasedOnMaintOrderID == null);
                templates.AddRange(templ);
            }
        }

        [ACMethodInfo("", "en{'Documentation'}de{'Dokumentation'}", 9999, true)]
        public void OpenTaskDocumentation()
        {
            BSOMedia_Child.Value.LoadMedia(SelectedMaintOrderTask);
            ShowDialog(BSOMedia_Child.Value, "MediaDialog");
        }

        public bool IsEnabledOpenTaskDocumentation()
        {
            return SelectedMaintOrderTask != null;
        }

        #region Methods => MaintService

        public void AttachToMaintServices()
        {
            if (_MaintServices != null)
                return;
            _MaintServices = new List<ACRef<ACComponent>>();
            List<ACComponent> appManagers = this.Root.FindChildComponents<ACComponent>(c => c is ApplicationManagerProxy || c is ApplicationManager, null, 1);
            foreach (var appManager in appManagers)
            {
                var maintService = appManager.ACUrlCommand("?" + nameof(ACMaintService)) as ACComponent;
                if (maintService == null)
                {
                    appManager.StartComponent(nameof(ACMaintService), null, null, ACStartCompOptions.NoServerReqFromProxy | ACStartCompOptions.OnlyAutomatic);
                    continue;
                }
                ACRef<ACComponent> refToService = new ACRef<ACComponent>(maintService, this);
                _MaintServices.Add(refToService);
            }
        }

        public void DetachMaintServices()
        {
            if (_MaintServices == null)
                return;
            _MaintServices.ForEach(c => c.Detach());
            _MaintServices = null;
        }

        #endregion

        /// <summary>
        /// Checks the configured maintenance rules and marks the ACClass with appropriate icon.
        /// </summary>
        /// <param name="items">The items for check.</param>
        /// <param name="recursive">The recursive parameter, define is method check icons recursive or only for top level.</param>
        /// <summary xml:lang="de">
        /// 
        /// </summary>
        /// <param xml:lang="de" name="items"></param>
        /// <param xml:lang="de" name="recursive"></param>
        [ACMethodInfo("", "", 999, true)]
        public void CheckIcons(ACClassInfoWithItems items, bool recursive = true)
        {
            //if (MaintRules.Any())
            //{
            //    CheckConfigRuleMarks(items, MaintRules, recursive);
            //}
            //else
            //{
            //    ClearConfigRuleMarks(items);
            //}
        }

        private Global.ConfigIconState CheckConfigRuleMarks(ACClassInfoWithItems items, IEnumerable<MaintACClass> maintconfigs, bool recursive = true)
        {
            Global.ConfigIconState iconState = Global.ConfigIconState.NoConfig;
            //items.IconState = iconState;
            //IEnumerable<vd.MaintACClass> maintConfigsFilter = null;

            //if (ActivatedRules && !DeactivatedRules)
            //    maintConfigsFilter = maintconfigs.Where(c => c.IsActive);

            //else if (!ActivatedRules && DeactivatedRules)
            //    maintConfigsFilter = maintconfigs.Where(c => !c.IsActive);

            //else if (!ActivatedRules && !DeactivatedRules)
            //{
            //    ClearConfigRuleMarks(items);
            //    return Global.ConfigIconState.NoConfig;
            //}

            //else
            //    maintConfigsFilter = maintconfigs;


            //if (RuleOnComponent && maintConfigsFilter.Any(c => c.VBiACClassID == items.ValueT.ACClassID))
            //{
            //    iconState = Global.ConfigIconState.Config;
            //    items.IconState = iconState;
            //}

            //else if (InheritedRule && !maintconfigs.Any(c => c.VBiACClassID == items.ValueT.ACClassID && (c.IsActive == ActivatedRules || !c.IsActive == DeactivatedRules)))
            //{
            //    foreach (var config in maintConfigsFilter)
            //    {
            //        if (items.ValueT.IsDerivedClassFrom(config.ACClass))
            //        {
            //            iconState = Global.ConfigIconState.InheritedConfig;
            //            items.IconState = iconState;
            //            continue;
            //        }
            //    }
            //}

            //if (recursive)
            //{
            //    foreach (ACClassInfoWithItems info in items.Items)
            //    {
            //        iconState = CheckConfigRuleMarks(info, maintconfigs);
            //        if (items.IconState == null || (Global.ConfigIconState)items.IconState == Global.ConfigIconState.NoConfig)
            //            items.IconState = iconState;
            //        else if (items.IconState != null && items.IconState is Global.ConfigIconState)
            //            iconState = (Global.ConfigIconState)items.IconState;
            //    }
            //}
            return iconState;
        }

        private void ClearConfigRuleMarks(ACClassInfoWithItems items)
        {
            items.IconState = Global.ConfigIconState.NoConfig;
            foreach (ACClassInfoWithItems item in items.Items)
            {
                item.IconState = Global.ConfigIconState.NoConfig;
                ClearConfigRuleMarks(item);
            }
        }

        private void RefreshOnSave(bool forceRefresh = false)
        {
            if (MaintServices != null)
                MaintServices.ForEach(c => c.ValueT.ExecuteMethod(nameof(ACMaintService.RebuildMaintCache)));
            else
            {
                Messages.Error(this, "The maintenace services are not available!");
            }
        }

        private void FacilityExplorer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BSOFacilityExplorer.SelectedFacility))
            {
                //if (_ACQueryDefinition.ACFilterColumns.Count != 1 || _ACQueryDefinition.ACFilterColumns.FirstOrDefault().PropertyName != "MDMaintOrderState\\MDMaintOrderStateIndex"
                //    || _ACQueryDefinition.ACFilterColumns.FirstOrDefault().SearchWord != CurrentMaintOrderStateFilter.MDMaintOrderStateIndex.ToString())
                //{
                //    _ACQueryDefinition.ClearFilter(true);
                //    _ACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "MDMaintOrderState\\MDMaintOrderStateIndex", Global.LogicalOperators.equal,
                //        Global.Operators.and, CurrentMaintOrderStateFilter.MDMaintOrderStateIndex.ToString(), true));
                //}
            }
        }



        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(New):
                    New();
                    return true;
                case nameof(IsEnabledNew):
                    result = IsEnabledNew();
                    return true;
                case nameof(NewTemplate):
                    NewTemplate();
                    return true;
                case nameof(IsEnabledNewTemplate):
                    result = IsEnabledNewTemplate();
                    return true;
                case nameof(Delete):
                    Delete();
                    return true;
                case nameof(IsEnabledDelete):
                    result = IsEnabledDelete();
                    return true;
                case nameof(WizzardNext):
                    WizzardNext();
                    return true;
                case nameof(IsEnabledWizzardNext):
                    result = IsEnabledWizzardNext();
                    return true;
                case nameof(WizzardBack):
                    WizzardBack();
                    return true;
                case nameof(IsEnabledWizzardBack):
                    result = IsEnabledWizzardBack();
                    return true;
                case nameof(CloseWindow):
                    CloseWizzard();
                    return true;
                case nameof(AddNewTask):
                    AddNewTask();
                    return true;
                case nameof(RemoveTask):
                    RemoveTask();
                    return true;
                case nameof(OpenTaskDocumentation):
                    OpenTaskDocumentation();
                    return true;
                case nameof(IsEnabledOpenTaskDocumentation):
                    result = IsEnabledOpenTaskDocumentation();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }

    public enum MaintenanceWizzardStepsEnum : short
    {
        FacilityOrClass = 0,
        TimeEvent = 10,
        Task = 20,
        Performer = 30
    }
}
