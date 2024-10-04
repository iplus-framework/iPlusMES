using gip.core.datamodel;
using gip.mes.autocomponent;

namespace gip.mes.maintenance
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Maintenance Rule'}de{'Wartungsregel'}", Global.ACKinds.TACBSOGlobal, Global.ACStorableTypes.NotStorable, false, true)]
    public class BSOMaintConfig : ACBSOvb
    {
        //#region c'tors

        public BSOMaintConfig(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        //public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        //{
        //    AttachToMaintServices();
        //    return (base.ACInit(startChildMode));
        //}

        //public override bool ACPostInit()
        //{
        //    if (SelectionManager != null)
        //    {
        //        SelectionManager.PropertyChanged += SelectionManager_PropertyChanged;
        //        if ((this.SelectionManager as VBBSOSelectionManager).ShowACObjectForSelection != null)
        //        {
        //            CurrentACComponent = (this.SelectionManager as VBBSOSelectionManager).ShowACObjectForSelection;
        //        }
        //    }
        //    return base.ACPostInit();
        //}

        //public override bool ACDeInit(bool deleteACClassTask = false)
        //{
        //    if (_SelectionManager != null)
        //    {
        //        _SelectionManager.Detach();
        //        _SelectionManager.ObjectDetaching -= _SelectionManager_ObjectDetaching;
        //        _SelectionManager.ObjectAttached -= _SelectionManager_ObjectAttached;
        //        _SelectionManager = null;
        //    }

        //    if (_CurrentACComponent != null)
        //    {
        //        _CurrentACComponent.Detach();
        //        _CurrentACComponent = null;
        //    }

        //    DetachMaintServices();
        //    if (MaintRoleACClassList != null && MaintRoleACClassList.Any())
        //    {
        //        foreach (var maintRole in MaintRoleACClassList)
        //            maintRole.PropertyChanged -= roleACClass_PropertyChanged;
        //    }
        //    //if (MaintRoleList != null && MaintRoleList.Any())
        //    //{
        //    //    foreach (var maintRole in MaintRoleList)
        //    //        maintRole.PropertyChanged -= role_PropertyChanged;
        //    //}
        //    //MaintRoleList = null;
        //    MaintRoleACClassList = null;

        //    return base.ACDeInit(deleteACClassTask);
        //}


        //#endregion

        //#region Properties

        //#region Maintenance
        //private List<ACRef<ACComponent>> _MaintServices = null;
        //public List<ACRef<ACComponent>> MaintServices
        //{
        //    get
        //    {
        //        return _MaintServices;
        //    }
        //}

        //private bool _SelectedFromHierarchy = false;

        //private List<VD.VBGroup> _VBGroups;
        //private List<VD.VBGroup> VBGroups
        //{
        //    get
        //    {
        //        if (_VBGroups == null)
        //            _VBGroups = DatabaseApp.VBGroup.ToList();
        //        return _VBGroups;
        //    }
        //}

        //private List<VD.MaintACClassVBGroup> _TempMaintACClassVBGroup = new List<VD.MaintACClassVBGroup>();

        //private List<VD.MaintACClassVBGroup> _TempMaintACClassPropertyVBGroup = new List<VD.MaintACClassVBGroup>();

        //private List<VD.MaintACClass> _MaintRules;

        //private List<VD.MaintACClass> MaintRules
        //{
        //    get
        //    {
        //        if (_MaintRules == null)
        //            _MaintRules = DatabaseApp.MaintACClass.ToList();
        //        return _MaintRules;
        //    }
        //}

        //private ACClass _CurrentACClass;
        //private ACClass CurrentACClass
        //{
        //    get
        //    {
        //        return _CurrentACClass;
        //    }
        //    set
        //    {
        //        _CurrentACClass = value;
        //        SetMaintACClass(_CurrentACClass);
        //        if (!_SelectedFromHierarchy)
        //            GetHierarchyOfACClass();
        //        _SelectedFromHierarchy = false;
        //        OnPropertyChanged("CurrentACClass");
        //    }
        //}

        //#endregion

        //#region Selection
        //private ACRef<VBBSOSelectionManager> _SelectionManager;
        //public VBBSOSelectionManager SelectionManager
        //{
        //    get
        //    {
        //        if (_SelectionManager != null)
        //            return _SelectionManager.ValueT;
        //        if (ParentACComponent != null)
        //        {
        //            VBBSOSelectionManager subACComponent = ParentACComponent.GetChildComponent(SelectionManagerACName) as VBBSOSelectionManager;
        //            if (subACComponent == null)
        //            {
        //                if (ParentACComponent is VBBSOSelectionDependentDialog)
        //                {
        //                    subACComponent = (ParentACComponent as VBBSOSelectionDependentDialog).SelectionManager;
        //                }
        //                else
        //                    subACComponent = ParentACComponent.StartComponent(SelectionManagerACName, null, null) as VBBSOSelectionManager;
        //            }
        //            if (subACComponent != null)
        //            {
        //                _SelectionManager = new ACRef<VBBSOSelectionManager>(subACComponent, this);
        //                _SelectionManager.ObjectDetaching += new EventHandler(_SelectionManager_ObjectDetaching);
        //                _SelectionManager.ObjectAttached += new EventHandler(_SelectionManager_ObjectAttached);
        //            }
        //        }
        //        if (_SelectionManager == null)
        //            return null;
        //        return _SelectionManager.ValueT;
        //    }
        //}

        //void _SelectionManager_ObjectAttached(object sender, EventArgs e)
        //{
        //    if (SelectionManager != null)
        //        SelectionManager.PropertyChanged += SelectionManager_PropertyChanged;
        //}

        //void _SelectionManager_ObjectDetaching(object sender, EventArgs e)
        //{
        //    if (SelectionManager != null)
        //        SelectionManager.PropertyChanged -= SelectionManager_PropertyChanged;
        //}

        //private string SelectionManagerACName
        //{
        //    get
        //    {
        //        string acInstance = ACUrlHelper.ExtractInstanceName(this.ACIdentifier);
        //        if (String.IsNullOrEmpty(acInstance))
        //            return "VBBSOSelectionManager";
        //        else
        //            return "VBBSOSelectionManager(" + acInstance + ")";
        //    }
        //}

        //void SelectionManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "ShowACObjectForSelection")
        //    {
        //        CurrentACComponent = SelectionManager.ShowACObjectForSelection;
        //    }
        //}

        //ACRef<IACObject> _CurrentACComponent;
        ///// <summary>
        ///// Gets or sets the current ACComponent (current component for the maintenance rule configuration).
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Ruft die aktuelle ACComponent (aktuelle Komponente für die Konfiguration der Wartungsregel) ab oder setzt sie.
        ///// </summary>
        //[ACPropertyInfo(9999)]
        //public IACObject CurrentACComponent
        //{
        //    get
        //    {
        //        if (_CurrentACComponent == null)
        //            return null;
        //        return _CurrentACComponent.ValueT;
        //    }
        //    set
        //    {
        //        bool objectSwapped = true;
        //        if (_CurrentACComponent != null)
        //        {
        //            if (_CurrentACComponent != value)
        //            {
        //                _CurrentACComponent.Detach();
        //            }
        //            else
        //                objectSwapped = false;
        //        }
        //        if (value == null)
        //            _CurrentACComponent = null;
        //        else
        //            _CurrentACComponent = new ACRef<IACObject>(value, this);
        //        if (_CurrentACComponent != null)
        //        {
        //            if (objectSwapped)
        //            {
        //                OnSelectionChanged();
        //            }
        //        }
        //        else
        //        {
        //            OnSelectionChanged();
        //        }
        //        OnPropertyChanged("CurrentACComponent");
        //    }
        //}
        //#endregion

        //#endregion

        //#region Class Selection

        //protected virtual void OnSelectionChanged()
        //{
        //    ACClass selectedClass = CurrentACComponent != null ? CurrentACComponent.ACType as ACClass : null;
        //    SetACClass(selectedClass);
        //}

        //public void AttachToMaintServices()
        //{
        //    if (_MaintServices != null)
        //        return;
        //    _MaintServices = new List<ACRef<ACComponent>>();
        //    List<ACComponent> appManagers = this.Root.FindChildComponents<ACComponent>(c => c is ApplicationManagerProxy || c is ApplicationManager, null, 1);
        //    foreach (var appManager in appManagers)
        //    {
        //        var maintService = appManager.ACUrlCommand("?ACMaintService") as ACComponent;
        //        if (maintService == null)
        //        {
        //            appManager.StartComponent("ACMaintService", null, null, ACStartCompOptions.NoServerReqFromProxy | ACStartCompOptions.OnlyAutomatic);
        //            continue;
        //        }
        //        ACRef<ACComponent> refToService = new ACRef<ACComponent>(maintService, this);
        //        _MaintServices.Add(refToService);
        //    }
        //}

        //public void DetachMaintServices()
        //{
        //    if (_MaintServices == null)
        //        return;
        //    _MaintServices.ForEach(c => c.Detach());
        //    _MaintServices = null;
        //}

        ///// <summary>
        ///// Sets the ACClass and CurrentACClassInfoWithItems.
        ///// </summary>
        ///// <param name="aCClassInfoWithItems">The acClassInfoWithItems to set.</param>
        ///// <summary xml:lang="de">
        ///// 
        ///// </summary>
        ///// <param xml:lang="de" name="aCClassInfoWithItems"></param>
        //[ACMethodInfo("", "", 999)]
        //public void SetACClassInfoWithItems(ACClassInfoWithItems aCClassInfoWithItems)
        //{
        //    SetACClass(aCClassInfoWithItems.ValueT);
        //    CurrentACClassInfoWithItems = aCClassInfoWithItems;
        //}

        ///// <summary>
        ///// Sets the current ACClass and search the available properties. 
        ///// </summary>
        ///// <param name="acClass"> The acClass to set.</param>
        //[ACMethodInfo("", "", 999)]
        //public void SetACClass(ACClass acClass)
        //{
        //    CurrentACClass = acClass;
        //    SearchProperties();
        //}

        //private VD.MaintACClass _CurrentMaintACClass;
        ///// <summary>
        ///// Gets or sets the current maintenance ACClass.
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Ruft die CurrentMaintACClass ab oder setzt sie.
        ///// </summary>
        //[ACPropertyInfo(999, "", "en{'Maintenance-Class'}de{'Wartungsklasse'}")]
        //public VD.MaintACClass CurrentMaintACClass
        //{
        //    get
        //    {
        //        return _CurrentMaintACClass;
        //    }
        //    set
        //    {
        //        if (_CurrentMaintACClass != null && _CurrentMaintACClass.EntityState != System.Data.EntityState.Detached)
        //            DatabaseApp.Detach(_CurrentMaintACClass);
        //        _CurrentMaintACClass = value;
        //        //ClearPropertyNonSavedRoles();
        //        SetMaintRolesForClass();
        //        OnPropertyChanged("CurrentMaintACClass");
        //    }
        //}

        //public void SetMaintACClass(gip.core.datamodel.ACClass acClass)
        //{
        //    if (acClass == null)
        //    {
        //        CurrentMaintACClass = null;
        //        return;
        //    }
        //    VD.MaintACClass maintACClass = DatabaseApp.MaintACClass.FirstOrDefault(c => c.VBiACClassID == acClass.ACClassID);
        //    if (maintACClass != null)
        //    {
        //        if (CurrentMaintACClass != maintACClass)
        //        {
        //            CurrentMaintACClass = maintACClass;
        //            SetMaintACClassProperty(CurrentMaintACClass);
        //            IsInCreateMode = false;
        //        }
        //        //if (maintACClass.LastMaintTerm != null && maintACClass.MaintInterval != null)
        //        //    OnPropertyChanged("CurrentMaintACClass\\NextMaintTerm");
        //        //    //maintACClass.NextMaintTerm = maintACClass.LastMaintTerm + TimeSpan.FromDays(maintACClass.MaintInterval.Value);
        //    }
        //    else
        //    {
        //        CurrentMaintACClass = null;
        //        if (MaintACClassPropertyList != null)
        //            MaintACClassPropertyList.Clear();
        //        IsInCreateMode = true;
        //    }
        //}

        ///// <summary>
        ///// Generates a new maintenance rule. 
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Generiert die neue Wartungsregel. 
        ///// </summary>
        //[ACMethodInfo("", "en{'Create new Maintenance Rule'}de{'Neue Wartungsregel anlegen'}", 999)]
        //public void GenerateNewMaintenanceConfiguration()
        //{
        //    VD.MaintACClass maintACClass = VD.MaintACClass.NewACObject(DatabaseApp, null);
        //    maintACClass.VBiACClass = DatabaseApp.ACClass.FirstOrDefault(c => c.ACClassID == SelectedACClassHierarchy.ACClass.ACClassID);
        //    CurrentMaintACClass = maintACClass;
        //    ACClassWrapper hierarchyClass = ACClassHierarchyList.FirstOrDefault(c => c.ACClass.ACClassID == maintACClass.VBiACClassID);
        //    if (hierarchyClass != null)
        //    {
        //        hierarchyClass.IsConfigured = maintACClass.IsActive;
        //        ACClassHierarchyList = ACClassHierarchyList.ToList();
        //    }
        //    IsInCreateMode = false;
        //    _IsRefreshNeeded = true;
        //}

        //public bool IsEnabledGenerateNewMaintenanceConfiguration()
        //{
        //    if (CurrentACClass != null && SelectedACClassHierarchy != null)
        //        return true;
        //    return false;
        //}

        //#endregion

        //#region MaintACClassProperty

        //private MaintACClassPropertyWrapper _CurrentMaintACClassProperty;
        ///// <summary>
        ///// Gets or sets the current maintenance ACClass.
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Ruft die aktuelle Wartung der ACClassProperty ab oder setzt sie.
        ///// </summary>
        //[ACPropertyCurrent(999, "MaintACClassProperty", "en{'Maintenance Property'}de{'Wartungseigenschaft'}")]
        //public MaintACClassPropertyWrapper CurrentMaintACClassProperty
        //{
        //    get
        //    {
        //        return _CurrentMaintACClassProperty;
        //    }
        //    set
        //    {
        //        _CurrentMaintACClassProperty = value;
        //        //SetMaintRolesForProperty();
        //        OnPropertyChanged("CurrentMaintACClassProperty");
        //    }
        //}

        //private List<MaintACClassPropertyWrapper> _MaintACClassProprtyList;
        ///// <summary>
        ///// Gets or sets the list of maintenance ACClassProperty.
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Ruft die Wartungsliste ACClassProperty ab oder setzt sie.
        ///// </summary>
        //[ACPropertyList(999, "MaintACClassProperty")]
        //public List<MaintACClassPropertyWrapper> MaintACClassPropertyList
        //{
        //    get
        //    {
        //        return _MaintACClassProprtyList;
        //    }
        //    set
        //    {
        //        _MaintACClassProprtyList = value;
        //        OnPropertyChanged("MaintACClassPropertyList");
        //    }
        //}

        //public void SetMaintACClassProperty(VD.MaintACClass maintACClass)
        //{
        //    IEnumerable<VD.MaintACClassProperty> tempProperties = maintACClass.MaintACClassProperty_MaintACClass.Where(c => c.IsActive);
        //    List<MaintACClassPropertyWrapper> wrappers = new List<MaintACClassPropertyWrapper>();
        //    foreach (VD.MaintACClassProperty prop in tempProperties)
        //    {
        //        MaintACClassPropertyWrapper wrapper = new MaintACClassPropertyWrapper() { MaintACClassProperty = prop };
        //        wrapper.ACClassProperty = DatabaseApp.ContextIPlus.ACClassProperty.FirstOrDefault(c => c.ACClassPropertyID == prop.VBiACClassPropertyID);
        //        wrappers.Add(wrapper);
        //    }
        //    MaintACClassPropertyList = wrappers;
        //}

        ///// <summary>
        ///// Gets or sets the ACClassProperty filter (search keyword).
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Ruft den ACClassProperty-Filter (Suchbegriff) ab oder setzt ihn.
        ///// </summary>
        //[ACPropertyInfo(999, "", "en{'Search Keyword'}de{'Suchwort'}", DefaultValue = "Statistics")]
        //public string ACClassPropertyFilter
        //{
        //    get;
        //    set;
        //}

        //public ACValueItem _CurrentFilterMode;
        ///// <summary>
        ///// Gets or sets the current filter mode (the filter mode for the events rule configuration).
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Ruft den aktuellen Filtermodus ab oder setzt ihn (den Filtermodus für die Konfiguration der Ereignisregel).
        ///// </summary>
        //[ACPropertyCurrent(999, "FilterMode", "en{'Filter Mode'}de{'Filtermodus'}")]
        //public ACValueItem CurrentFilterMode
        //{
        //    get
        //    {
        //        if (_CurrentFilterMode == null)
        //            _CurrentFilterMode = FilterModeList.FirstOrDefault();
        //        return _CurrentFilterMode;
        //    }
        //    set
        //    {
        //        _CurrentFilterMode = value;
        //        OnPropertyChanged("CurrentFilterMode");
        //    }
        //}

        //private ACValueItemList _FilterModeList;
        ///// <summary>
        ///// Gets the list of filter modes.
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Ruft die Liste der Filtermodi ab.
        ///// </summary>
        //[ACPropertyList(999, "FilterMode")]
        //public ACValueItemList FilterModeList
        //{
        //    get
        //    {
        //        if (_FilterModeList == null)
        //        {
        //            _FilterModeList = new ACValueItemList("FilterMode");
        //            _FilterModeList.Add(new ACValueItem("en{'By Group'}de{'Nach Gruppe'}", "Group", null));
        //            _FilterModeList.Add(new ACValueItem("en{'By Property Name'}de{'Nach Eigenschaft'}", "PropertyName", null));
        //        }
        //        return _FilterModeList;
        //    }
        //}

        //private ACClassProperty _SelectedACClassProperty;
        ///// <summary>
        ///// Gets or sets the selected ACClassProperty.
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Ruft die ausgewählte ACClassProperty ab oder setzt sie.
        ///// </summary>
        //[ACPropertySelected(999, "ACClassProperty")]
        //public ACClassProperty SelectedACClassProperty
        //{
        //    get
        //    {
        //        return _SelectedACClassProperty;
        //    }
        //    set
        //    {
        //        _SelectedACClassProperty = value;
        //        OnPropertyChanged("SelectedACClassProperty");
        //    }
        //}

        //private List<ACClassProperty> _ACClassPropertyList;
        ///// <summary>
        ///// Gets or sets the list of ACClassProperties (Available properties)
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Ruft die Liste der ACClassProperties (Available properties) ab oder setzt sie.
        ///// </summary>
        //[ACPropertyList(999, "ACClassProperty")]
        //public List<ACClassProperty> ACClassPropertyList
        //{
        //    get
        //    {
        //        return _ACClassPropertyList;
        //    }
        //    set
        //    {
        //        _ACClassPropertyList = value;
        //        OnPropertyChanged("ACClassPropertyList");
        //    }
        //}

        //public List<ACClassProperty> FindACClassProperties(ACClass acClass)
        //{
        //    List<ACClassProperty> tempList = new List<ACClassProperty>();
        //    ACClass tempACClass = acClass;

        //    if (CurrentFilterMode.Value.ToString() == "Group")
        //    {
        //        while (tempACClass != null)
        //        {
        //            IEnumerable<ACClassProperty> properties = tempACClass.ACClassProperty_ACClass.Where(c => !String.IsNullOrEmpty(c.ACGroup)
        //                                                                                                && !c.IsStatic
        //                                                                                                && c.ACGroup.Equals(ACClassPropertyFilter, StringComparison.OrdinalIgnoreCase)
        //                                                                                                && (MaintACClassPropertyList == null
        //                                                                                                    || !MaintACClassPropertyList.Any(x => c.ACIdentifier == x.ACClassProperty.ACIdentifier
        //                                                                                                                                         && x.MaintACClassProperty.IsActive)));
        //            if (properties != null)
        //            {
        //                tempList.AddRange(properties);
        //            }
        //            tempACClass = tempACClass.ACClass1_BasedOnACClass;
        //        }
        //    }
        //    else if (CurrentFilterMode.Value.ToString() == "PropertyName")
        //    {
        //        while (tempACClass != null)
        //        {
        //            IEnumerable<ACClassProperty> properties = tempACClass.ACClassProperty_ACClass.Where(c => !c.IsStatic 
        //                                                                                                    && ((!String.IsNullOrEmpty(c.ACIdentifier) && c.ACIdentifier.Equals(ACClassPropertyFilter, StringComparison.OrdinalIgnoreCase))
        //                                                                                                        || (!String.IsNullOrEmpty(c.ACCaption) && c.ACCaption.Equals(ACClassPropertyFilter, StringComparison.OrdinalIgnoreCase)))
        //                                                                                                    && (MaintACClassPropertyList == null
        //                                                                                                        || !MaintACClassPropertyList.Any(x => c.ACIdentifier == x.ACClassProperty.ACIdentifier
        //                                                                                                                                                && x.MaintACClassProperty.IsActive)));
        //            if (properties != null)
        //            {
        //                tempList.AddRange(properties);
        //            }
        //            tempACClass = tempACClass.ACClass1_BasedOnACClass;
        //        }
        //    }
        //    return tempList;
        //}

        ///// <summary>
        ///// Adds the selected maintenance rule property to the assigned properties. 
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Fügt den zugeordneten Eigenschaften die Eigenschaft der ausgewählten Wartungsregel hinzu. 
        ///// </summary>
        //[ACMethodInfo("", "", 999)]
        //public void AddPropertyToMaintACClass()
        //{
        //    VD.MaintACClassProperty maintACClassProperty = CurrentMaintACClass.MaintACClassProperty_MaintACClass.FirstOrDefault(c => c.VBiACClassProperty.ACIdentifier == SelectedACClassProperty.ACIdentifier);
        //    if (maintACClassProperty == null)
        //    {
        //        maintACClassProperty = VD.MaintACClassProperty.NewACObject(DatabaseApp, CurrentMaintACClass);
        //        maintACClassProperty.VBiACClassProperty = DatabaseApp.ACClassProperty.FirstOrDefault(c => c.ACClassPropertyID == SelectedACClassProperty.ACClassPropertyID);
        //        CurrentMaintACClass.MaintACClassProperty_MaintACClass.Add(maintACClassProperty);
        //    }
        //    maintACClassProperty.IsActive = true;
        //    if (MaintACClassPropertyList == null)
        //        MaintACClassPropertyList = new List<MaintACClassPropertyWrapper>();
        //    MaintACClassPropertyList.Add(new MaintACClassPropertyWrapper() { MaintACClassProperty = maintACClassProperty, ACClassProperty = SelectedACClassProperty });
        //    MaintACClassPropertyList = MaintACClassPropertyList.ToList();
        //    ACClassPropertyList.Remove(SelectedACClassProperty);
        //    ACClassPropertyList = ACClassPropertyList.ToList();
        //}

        //public bool IsEnabledAddPropertyToMaintACClass()
        //{
        //    if (SelectedACClassProperty != null)
        //        return true;
        //    return false;
        //}

        ///// <summary>
        ///// Removes the selected MaintACClass property from the assigned properties. 
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Entfernt die ausgewählte MaintACClass-Eigenschaft aus den zugewiesenen Eigenschaften.
        ///// </summary>
        //[ACMethodInfo("", "", 999)]
        //public void RemovePropertyFromMaintACClass()
        //{
        //    CurrentMaintACClassProperty.MaintACClassProperty.IsActive = false;
        //    ACClassPropertyList.Add(CurrentMaintACClassProperty.ACClassProperty);
        //    MaintACClassPropertyList.Remove(CurrentMaintACClassProperty);
        //    MaintACClassPropertyList = MaintACClassPropertyList.ToList();
        //    ACClassPropertyList = ACClassPropertyList.ToList();
        //}

        //public bool IsEnabledRemovePropertyFromMaintACClass()
        //{
        //    if (CurrentMaintACClassProperty != null)
        //        return true;
        //    return false;
        //}

        //#endregion

        //#region MaintACClassRole

        //private VBGroupWrapper _CurrentMaintACClassRole;
        ///// <summary>
        ///// Gets or sets the current maintenance ACClassRole.
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Ruft die aktuelle Wartung der ACClassRole ab oder setzt sie.
        ///// </summary>
        //[ACPropertyCurrent(999, "MaintRoleACClass", "en{'Role of Maintainer'}de{'Rolle des Instandhalters'}")]
        //public VBGroupWrapper CurrentMaintACClassRole
        //{
        //    get
        //    {
        //        return _CurrentMaintACClassRole;
        //    }
        //    set
        //    {
        //        _CurrentMaintACClassRole = value;
        //        SetMaintACClassGroup();
        //        OnPropertyChanged("CurrentMaintACClassRole");
        //    }
        //}

        //private IEnumerable<VBGroupWrapper> _MaintRoleACClassList;
        ///// <summary>
        ///// Gets or sets the list of maintenance RoleACClass.
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Ruft die Liste der WartungsroleACClass ab oder setzt sie.
        ///// </summary>
        //[ACPropertyList(999, "MaintRoleACClass")]
        //public IEnumerable<VBGroupWrapper> MaintRoleACClassList
        //{
        //    get
        //    {
        //        return _MaintRoleACClassList;
        //    }
        //    set
        //    {
        //        _MaintRoleACClassList = value;
        //        OnPropertyChanged("MaintRoleACClassList");
        //    }
        //}

        //private VD.MaintACClassVBGroup _CurrentMaintACClassGroup;
        ///// <summary>
        ///// Gets or sets the current MaintACClassGroup (VBGroup for the maintenance roles).
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Ruft die aktuelle MaintACClassGroup (VBGroup für die Pflegerollen) ab oder setzt sie.
        ///// </summary>
        //[ACPropertyInfo(999)]
        //public VD.MaintACClassVBGroup CurrentMaintACClassGroup
        //{
        //    get
        //    {
        //        return _CurrentMaintACClassGroup;
        //    }
        //    set
        //    {
        //        _CurrentMaintACClassGroup = value;
        //        OnPropertyChanged("CurrentMaintACClassGroup");
        //    }
        //}

        //private void SetMaintRolesForClass()
        //{
        //    if (MaintRoleACClassList != null && MaintRoleACClassList.Any())
        //    {
        //        foreach (var maintRole in MaintRoleACClassList)
        //            maintRole.PropertyChanged -= roleACClass_PropertyChanged;
        //    }
        //    if (CurrentMaintACClass == null)
        //    {
        //        MaintRoleACClassList = null;
        //        return;
        //    }

        //    List<VBGroupWrapper> tempRoles = new List<VBGroupWrapper>();
        //    VBGroups.ForEach(c => tempRoles.Add(new VBGroupWrapper() { VBGroup = c }));
        //    //IEnumerable<VD.MaintACClassVBGroup> assignedRoles = DatabaseApp.MaintACClassVBGroup.Where(c => c.MaintACClassID == CurrentMaintACClass.MaintACClassID && c.IsActive);
        //    //foreach (var roleACClass in tempRoles)
        //    //{
        //    //    roleACClass.PropertyChanged += roleACClass_PropertyChanged;
        //    //    if (assignedRoles.Any(c => c.VBGroupID == roleACClass.VBGroup.VBGroupID))
        //    //        roleACClass.IsChecked = true;
        //    //    else
        //    //        roleACClass.IsChecked = false;
        //    //}
        //    MaintRoleACClassList = tempRoles;
        //}

        //private void SetMaintACClassGroup()
        //{
        //    if(CurrentMaintACClass == null || CurrentMaintACClassRole == null)
        //    {
        //        CurrentMaintACClassGroup = null;
        //        return;
        //    }

        //    VD.MaintACClassVBGroup classGroup = _TempMaintACClassVBGroup.FirstOrDefault(c => c.VBGroupID == _CurrentMaintACClassRole.VBGroup.VBGroupID
        //                                                                                  && c.MaintACClassID == CurrentMaintACClass.MaintACClassID);
        //    if (classGroup != null)
        //        CurrentMaintACClassGroup = classGroup;
        //    else if (_CurrentMaintACClassRole != null)
        //    {
        //        classGroup = DatabaseApp.MaintACClassVBGroup.FirstOrDefault(c => c.VBGroupID == _CurrentMaintACClassRole.VBGroup.VBGroupID
        //                                                                      && c.MaintACClassID == CurrentMaintACClass.MaintACClassID);
        //        CurrentMaintACClassGroup = classGroup;
        //    }
        //    else
        //        CurrentMaintACClassGroup = null;

        //}

        //void roleACClass_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    VBGroupWrapper maintRole = sender as VBGroupWrapper;
        //    if (e.PropertyName == "IsChecked" && maintRole != null)
        //    {
        //        CurrentMaintACClassRole = maintRole;
        //        VD.MaintACClassVBGroup acClassRole = DatabaseApp.MaintACClassVBGroup.FirstOrDefault(c => c.MaintACClassID == CurrentMaintACClass.MaintACClassID
        //                                                                                        && c.VBGroupID == maintRole.VBGroup.VBGroupID);

        //        VD.MaintACClassVBGroup acClassRoleTemp = _TempMaintACClassVBGroup.FirstOrDefault(c => c.MaintACClassID == CurrentMaintACClass.MaintACClassID
        //                                                                                        && c.VBGroupID == maintRole.VBGroup.VBGroupID);

        //        if (maintRole.IsChecked && acClassRole == null && acClassRoleTemp == null)
        //        {
        //            acClassRole = VD.MaintACClassVBGroup.NewACObject(DatabaseApp, null);
        //            acClassRole.MaintACClass = CurrentMaintACClass;
        //            acClassRole.VBGroup = maintRole.VBGroup;
        //            acClassRole.IsActive = true;
        //            _TempMaintACClassVBGroup.Add(acClassRole);
        //            if (CurrentMaintACClassRole == maintRole)
        //                CurrentMaintACClassGroup = acClassRole;
        //        }
        //        else if (maintRole.IsChecked && acClassRole != null && !acClassRole.IsActive)
        //        {
        //            acClassRole.IsActive = true;
        //        }
        //        else if (!maintRole.IsChecked && acClassRole != null)
        //        {
        //            acClassRole.IsActive = false;
        //        }
        //        else if (!maintRole.IsChecked && acClassRoleTemp != null)
        //        {
        //            DatabaseApp.Detach(acClassRoleTemp);
        //            _TempMaintACClassVBGroup.Remove(acClassRoleTemp);
        //        }
        //    }
        //}

        //private void ClearClassNonSavedRoles()
        //{
        //    foreach (VD.MaintACClassVBGroup role in _TempMaintACClassVBGroup)
        //        DatabaseApp.Detach(role);
        //    _TempMaintACClassVBGroup.Clear();
        //}

        //#endregion

        //#region HierarchyConfiguration

        //private ACClassWrapper _SelectedACClassHierarchy;
        ///// <summary>
        ///// Gets or sets the selected ACClassHierarchy.
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Ruft die ausgewählte ACClassHierarchie ab oder setzt sie.
        ///// </summary>
        //[ACPropertySelected(999, "ACClassHierarchy")]
        //public ACClassWrapper SelectedACClassHierarchy
        //{
        //    get
        //    {
        //        return _SelectedACClassHierarchy;
        //    }
        //    set
        //    {
        //        if (OnIsEnabledSave())
        //        {
        //            if (Messages.Question(this, "Question50030") == Global.MsgResult.Yes)
        //                ApplyChanges();
        //            else
        //                DatabaseApp.ACUndoChanges();
        //        }
        //        if (CurrentMaintACClass != null && CurrentMaintACClass.EntityState == System.Data.EntityState.Detached)
        //            _SelectedACClassHierarchy.IsConfigured = null;

        //        _SelectedACClassHierarchy = value;
        //        _SelectedFromHierarchy = true;
        //        if (_SelectedACClassHierarchy != null)
        //            SetACClass(_SelectedACClassHierarchy.ACClass);
        //        else
        //            SetACClass(null);
        //        OnPropertyChanged("SelectedACClassHierarchy");
        //    }
        //}

        //private IEnumerable<ACClassWrapper> _ACClassHierarchyList;
        ///// <summary>
        ///// Gets or sets the list of ACClass hierarchy.
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Ruft die Liste der ACClass-Hierarchie ab oder setzt sie.
        ///// </summary>
        //[ACPropertyList(999, "ACClassHierarchy")]
        //public IEnumerable<ACClassWrapper> ACClassHierarchyList
        //{
        //    get
        //    {
        //        return _ACClassHierarchyList;
        //    }
        //    set
        //    {
        //        _ACClassHierarchyList = value;
        //        OnPropertyChanged("ACClassHierarchyList");
        //    }
        //}

        //private void GetHierarchyOfACClass()
        //{
        //    if (CurrentACClass == null)
        //        return;
        //    List<ACClassWrapper> tempACClassList = new List<ACClassWrapper>();
        //    tempACClassList.Add(new ACClassWrapper() { ACClass = CurrentACClass, IsConfigured = IsConfigured(CurrentACClass) });
        //    ACClass tempACClass = CurrentACClass.ACClass1_BasedOnACClass;

        //    while (tempACClass.ACClass1_BasedOnACClass != null && tempACClass.ACClass1_BasedOnACClass.ValueTypeACClass.ObjectType != typeof(core.autocomponent.PAClassAlarmingBase))
        //    {
        //        tempACClassList.Add(new ACClassWrapper() { ACClass = tempACClass, IsConfigured = IsConfigured(tempACClass) });
        //        tempACClass = tempACClass.ACClass1_BasedOnACClass;
        //    }
        //    ACClassHierarchyList = tempACClassList;
        //    SelectedACClassHierarchy = ACClassHierarchyList.FirstOrDefault();
        //}

        //private void RefreshHierarchy()
        //{
        //    if (CurrentMaintACClass != null && SelectedACClassHierarchy != null && CurrentMaintACClass.IsActive != SelectedACClassHierarchy.IsConfigured)
        //    {
        //        SelectedACClassHierarchy.IsConfigured = CurrentMaintACClass.IsActive;
        //        SelectedACClassHierarchy.OnPropertyChanged("IsConfigured");
        //        _IsRefreshNeeded = true;
        //    }
        //}

        //private bool? IsConfigured(ACClass acClass)
        //{
        //    VD.MaintACClass maintClass = DatabaseApp.MaintACClass.FirstOrDefault(c => c.VBiACClassID == acClass.ACClassID);
        //    if (maintClass == null)
        //        return null;
        //    return maintClass.IsActive;
        //}

        //#endregion

        //#region ComponentSelector

        //private ACClassInfoWithItems.VisibilityFilters _ComponentTypeFilter;
        //[ACPropertyInfo(999)]
        //public ACClassInfoWithItems.VisibilityFilters ComponentTypeFilter
        //{
        //    get
        //    {
        //        if (_ComponentTypeFilter == null)
        //            _ComponentTypeFilter = new ACClassInfoWithItems.VisibilityFilters() { IncludeTypes = new List<Type> { typeof(PAClassAlarmingBase), typeof(ApplicationManager) } };
        //        return _ComponentTypeFilter;
        //    }
        //}

        //#endregion

        //#region Show/Hide Prop

        //private bool _IsInCreateMode = false;
        //public bool IsInCreateMode
        //{
        //    get
        //    {
        //        return _IsInCreateMode;
        //    }
        //    private set
        //    {
        //        _IsInCreateMode = value;
        //        if (_IsInCreateMode)
        //        {
        //            NewMaintenanceConfigCM = Global.ControlModes.Enabled;
        //            MaintenanceConfigDetailsCM = Global.ControlModes.Collapsed;
        //        }
        //        else
        //        {
        //            NewMaintenanceConfigCM = Global.ControlModes.Collapsed;
        //            MaintenanceConfigDetailsCM = Global.ControlModes.Enabled;
        //        }
        //        OnPropertyChanged("IsInCreateMode");
        //    }

        //}

        //private Global.ControlModes _NewMaintenanceConfigCM = Global.ControlModes.Collapsed;
        ///// <summary>
        ///// Gets or sets the control modes of new maintenance rule configuration.
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Ruft die Liste der WartungsroleACClass ab oder setzt sie.
        ///// </summary>
        //[ACPropertyInfo(999)]
        //public Global.ControlModes NewMaintenanceConfigCM
        //{
        //    get
        //    {
        //        return _NewMaintenanceConfigCM;
        //    }
        //    set
        //    {
        //        _NewMaintenanceConfigCM = value;
        //        OnPropertyChanged("NewMaintenanceConfigCM");
        //    }
        //}

        //private Global.ControlModes _MaintenanceConfigDetailsCM = Global.ControlModes.Collapsed;
        ///// <summary>
        ///// Gets or sets the control modes of maintenance configuration details.
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Ruft die Kontrollmodi der Wartungskonfigurationsdetails ab oder setzt sie.
        ///// </summary>
        //[ACPropertyInfo(999)]
        //public Global.ControlModes MaintenanceConfigDetailsCM
        //{
        //    get
        //    {
        //        return _MaintenanceConfigDetailsCM;
        //    }
        //    set
        //    {
        //        _MaintenanceConfigDetailsCM = value;
        //        OnPropertyChanged("MaintenanceConfigDetailsCM");
        //    }
        //}

        //private bool _IsTimeVisible;
        ///// <summary>
        ///// Gets or sets the visibility of the times tab.
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Ruft die Sichtbarkeit der Registerkarte Zeiten ab oder legt sie fest.
        ///// </summary>
        //[ACPropertyInfo(999)]
        //public bool IsTimeVisible
        //{
        //    get
        //    {
        //        return _IsTimeVisible;
        //    }
        //    set
        //    {
        //        _IsTimeVisible = value;
        //        OnPropertyChanged("IsTimeVisible");
        //    }
        //}

        //private bool _IsEventVisible;
        ///// <summary>
        ///// Gets or sets the visibility of the events tab.
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Ruft die Sichtbarkeit der Registerkarte Ereignisse ab oder legt sie fest.
        ///// </summary>
        //[ACPropertyInfo(999)]
        //public bool IsEventVisible
        //{
        //    get
        //    {
        //        return _IsEventVisible;
        //    }
        //    set
        //    {
        //        _IsEventVisible = value;
        //        OnPropertyChanged("IsEventVisible");
        //    }
        //}

        //#endregion

        //#region Methods

        ///// <summary>
        ///// Saves the changes. 
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Speichert die Änderungen. 
        ///// </summary>
        //[ACMethodInfo("", "en{'Apply Changes'}de{'Änderungen anwenden'}", 999, true)]
        //public void ApplyChanges()
        //{
        //    OnApplyChanges();
        //}

        //public bool IsEnabledApplyChanges()
        //{
        //    if (CurrentMaintACClass == null || CurrentMaintACClass.MDMaintMode == null || !OnIsEnabledSave())
        //        return false;
        //    return true;
        //}

        //private bool OnApplyChanges()
        //{
        //    if (CurrentMaintACClass != null && CurrentMaintACClass.MDMaintMode != null && !CurrentMaintACClass.MaintACClassVBGroup_MaintACClass.Any(c => c.IsActive))
        //    {
        //        // At least one maintenance role must be checked!
        //        Messages.Error(this, "Error50101");
        //        return false;
        //    }
        //    Msg msg = DatabaseApp.ACSaveChanges();
        //    if (msg != null)
        //    {
        //        Messages.Msg(msg);
        //        return false;
        //    }
        //    if (MaintServices != null)
        //        MaintServices.ForEach(c => c.ValueT.ACUrlCommand("!RebuildMaintCache"));

        //    _MaintRules = null;

        //    RefreshOnSave();

        //    _TempMaintACClassVBGroup.Clear();
        //    _TempMaintACClassPropertyVBGroup.Clear();

        //    return true;
        //}

        //[ACMethodCommand("MaintConfig", "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, Global.ACKinds.MSMethodPrePost)]
        //public void Delete()
        //{
        //    if (!PreExecute("Delete"))
        //        return;

        //    foreach (VD.MaintACClassProperty maintProp in CurrentMaintACClass.MaintACClassProperty_MaintACClass.ToArray())
        //    {
        //        Msg msg = maintProp.DeleteACObject(DatabaseApp, true);
        //        if(msg != null)
        //        {
        //            Messages.Msg(msg);
        //            return;
        //        }
        //    }

        //    foreach(VD.MaintACClassVBGroup maintGroup in CurrentMaintACClass.MaintACClassVBGroup_MaintACClass.ToArray())
        //    {
        //        Msg msg = maintGroup.DeleteACObject(DatabaseApp, true);
        //        if (msg != null)
        //        {
        //            Messages.Msg(msg);
        //            return;
        //        }
        //    }

        //    Msg msgMaint = CurrentMaintACClass.DeleteACObject(DatabaseApp, true);
        //    if (msgMaint != null)
        //    {
        //        Messages.Msg(msgMaint);
        //        return;
        //    }

        //    _IsRefreshNeeded = true;

        //    if (OnIsEnabledSave())
        //    {
        //        bool undo = false;
        //        if (Messages.Question(this, "Question50030") == Global.MsgResult.Yes)
        //            undo = !OnApplyChanges();
        //        else
        //            undo = true;

        //        if(undo)
        //        {
        //            DatabaseApp.ACUndoChanges();
        //            return;
        //        }
        //    }

        //    SelectedACClassHierarchy.IsConfigured = null;
        //    IsInCreateMode = true;
        //    CurrentMaintACClass = null;

        //    PostExecute("Delete");
        //}

        //public bool IsEnabledDelete()
        //{
        //    return CurrentMaintACClass != null;
        //}

        ///// <summary>
        ///// Searches the available MaintACClass properties. 
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// Sucht die verfügbaren MaintACClass-Eigenschaften. 
        ///// </summary>
        //[ACMethodInfo("", "en{'Search'}de{'Suchen'}", 999)]
        //public void SearchProperties()
        //{
        //    ACClassPropertyList = FindACClassProperties(CurrentACClass);
        //}

        ///// <summary>Called inside the GetControlModes-Method to get the Global.ControlModes from derivations.
        ///// This method should be overriden in the derivations to dynmically control the presentation mode depending on the current value which is bound via VBContent</summary>
        ///// <param name="vbControl">A WPF-Control that implements IVBContent</param>
        ///// <returns>ControlModesInfo</returns>
        //public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        //{
        //    var cm = base.OnGetControlModes(vbControl);
        //    if (vbControl != null && !string.IsNullOrEmpty(vbControl.VBContent))
        //    {
        //        switch (vbControl.VBContent)
        //        {
        //            case "CurrentMaintACClass\\MDMaintMode":
        //                if (CurrentMaintACClass != null && CurrentMaintACClass.MDMaintMode != null)
        //                {
        //                    if (CurrentMaintACClass.MDMaintMode.MDMaintModeIndex == (short)VD.MDMaintMode.MaintModes.TimeOnly)
        //                    {
        //                        IsTimeVisible = true;
        //                        IsEventVisible = false;
        //                    }
        //                    else if (CurrentMaintACClass.MDMaintMode.MDMaintModeIndex == (short)VD.MDMaintMode.MaintModes.EventOnly)
        //                    {
        //                        IsTimeVisible = false;
        //                        IsEventVisible = true;
        //                    }
        //                    else if (CurrentMaintACClass.MDMaintMode.MDMaintModeIndex == (short)VD.MDMaintMode.MaintModes.TimeAndEvent)
        //                    {
        //                        IsTimeVisible = true;
        //                        IsEventVisible = true;
        //                    }
        //                }
        //                else
        //                {
        //                    IsTimeVisible = false;
        //                    IsEventVisible = false;
        //                }
        //                break;
        //            case "CurrentMaintACClass\\IsActive":
        //                RefreshHierarchy();
        //                break;
        //            case "IsTimeVisible":
        //                if (IsTimeVisible)
        //                    cm = Global.ControlModes.Enabled;
        //                else
        //                    cm = Global.ControlModes.Hidden;
        //                break;
        //            case "IsEventVisible":
        //                if (IsEventVisible)
        //                    cm = Global.ControlModes.Enabled;
        //                else
        //                    cm = Global.ControlModes.Hidden;
        //                break;
        //            case "CurrentMaintACClassProperty\\VBiACClassProperty\\ACIdentifier":
        //                if (CurrentMaintACClassProperty == null)
        //                    cm = Global.ControlModes.Collapsed;
        //                else
        //                    cm = Global.ControlModes.Enabled;
        //                break;
        //            case "CurrentMaintACClassProperty\\IsActive":
        //                if (CurrentMaintACClassProperty == null)
        //                    cm = Global.ControlModes.Collapsed;
        //                else
        //                    cm = Global.ControlModes.Enabled;
        //                break;
        //            case "CurrentMaintACClassProperty\\MaxValue":
        //                if (CurrentMaintACClassProperty != null && CurrentMaintACClassProperty.MaintACClassProperty.VBiACClassProperty.ValueTypeACClass.ACIdentifier != Const.TNameDateTime)
        //                    cm = Global.ControlModes.Enabled;
        //                else
        //                    cm = Global.ControlModes.Collapsed;
        //                break;
        //            case "CurrentMaintACClassProperty\\MaxValueDT":
        //                if (CurrentMaintACClassProperty != null && CurrentMaintACClassProperty.MaintACClassProperty.VBiACClassProperty.ValueTypeACClass.ACIdentifier == Const.TNameDateTime)
        //                    cm = Global.ControlModes.Enabled;
        //                else
        //                    cm = Global.ControlModes.Collapsed;
        //                break;
        //            case "CurrentMaintACClassProperty\\IsWarningActive":
        //                if (CurrentMaintACClassProperty == null)
        //                    cm = Global.ControlModes.Collapsed;
        //                else
        //                    cm = Global.ControlModes.Enabled;
        //                break;
        //            case "CurrentMaintACClassProperty\\WarningValueDiff":
        //                if (CurrentMaintACClassProperty == null)
        //                    cm = Global.ControlModes.Collapsed;
        //                else
        //                    cm = Global.ControlModes.Enabled;
        //                break;
        //            case "CurrentMaintACClassProperty\\MaintACClassProperty\\IsWarningActive":
        //                if (CurrentMaintACClassProperty != null)
        //                    cm = Global.ControlModes.Enabled;
        //                else
        //                    cm = Global.ControlModes.Hidden;
        //                break;
        //        }
        //    }
        //    return cm;
        //}

        //#endregion

        //#region IconHasRule

        //private ACBSO _ComponentSelector;
        //public ACBSO ComponentSelector
        //{
        //    get
        //    {
        //        if (_ComponentSelector == null)
        //            _ComponentSelector = GetChildComponent("BSOComponentSelectorMaint") as ACBSO;
        //        return _ComponentSelector;
        //    }
        //}

        //private bool _IsRefreshNeeded = false;

        //private ACClassInfoWithItems _CurrentACClassInfoWithItems;
        //public ACClassInfoWithItems CurrentACClassInfoWithItems
        //{
        //    get
        //    {
        //        return _CurrentACClassInfoWithItems;
        //    }
        //    set
        //    {
        //        _CurrentACClassInfoWithItems = value;
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the rule on component filter.
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// 
        ///// </summary>
        //[ACPropertyInfo(999, "", "en{'Rules on Component'}de{'Komponentenregeln'}", DefaultValue = true)]
        //public bool RuleOnComponent
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// Gets or sets the inherited rule filter.
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// 
        ///// </summary>
        //[ACPropertyInfo(999, "", "en{'Inherited Rules'}de{'Vererbte Regeln'}", DefaultValue = true)]
        //public bool InheritedRule
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// Gets or sets the activated rules filter.
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// 
        ///// </summary>
        //[ACPropertyInfo(999, "", "en{'Activated Rules'}de{'Aktivierte Regeln'}", DefaultValue = true)]
        //public bool ActivatedRules
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// Gets or sets the deactivated rules filter.
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// 
        ///// </summary>
        //[ACPropertyInfo(999, "", "en{'Deactivated Rules'}de{'Deaktivierte Regeln'}")]
        //public bool DeactivatedRules
        //{
        //    get;
        //    set;
        //}

        //private void RefreshOnSave(bool forceRefresh = false)
        //{
        //    if (_IsRefreshNeeded || forceRefresh)
        //    {
        //        if (ComponentSelector != null)
        //        {
        //            ACClassInfoWithItems root = ComponentSelector.ACUrlCommand("CurrentProjectItemRoot") as ACClassInfoWithItems;
        //            if (root != null)
        //            {
        //                CheckIcons(root);
        //            }
        //        }
        //        _IsRefreshNeeded = false;
        //    }
        //}

        ///// <summary>
        ///// Checks the configured maintenance rules and marks the ACClass with appropriate icon.
        ///// </summary>
        ///// <param name="items">The items for check.</param>
        ///// <param name="recursive">The recursive parameter, define is method check icons recursive or only for top level.</param>
        ///// <summary xml:lang="de">
        ///// 
        ///// </summary>
        ///// <param xml:lang="de" name="items"></param>
        ///// <param xml:lang="de" name="recursive"></param>
        //[ACMethodInfo("", "", 999, true)]
        //public void CheckIcons(ACClassInfoWithItems items, bool recursive = true)
        //{
        //    if (MaintRules.Any())
        //    {
        //        CheckConfigRuleMarks(items, MaintRules, recursive);
        //    }
        //    else
        //    {
        //        ClearConfigRuleMarks(items);
        //    }
        //}

        //private Global.ConfigIconState CheckConfigRuleMarks(ACClassInfoWithItems items, IEnumerable<VD.MaintACClass> maintconfigs, bool recursive = true)
        //{
        //    Global.ConfigIconState iconState = Global.ConfigIconState.NoConfig;
        //    items.IconState = iconState;
        //    IEnumerable<VD.MaintACClass> maintConfigsFilter = null;

        //    if (ActivatedRules && !DeactivatedRules)
        //        maintConfigsFilter = maintconfigs.Where(c => c.IsActive);

        //    else if (!ActivatedRules && DeactivatedRules)
        //        maintConfigsFilter = maintconfigs.Where(c => !c.IsActive);

        //    else if (!ActivatedRules && !DeactivatedRules)
        //    {
        //        ClearConfigRuleMarks(items);
        //        return Global.ConfigIconState.NoConfig;
        //    }

        //    else
        //        maintConfigsFilter = maintconfigs;


        //    if (RuleOnComponent && maintConfigsFilter.Any(c => c.VBiACClassID == items.ValueT.ACClassID))
        //    {
        //        iconState = Global.ConfigIconState.Config;
        //        items.IconState = iconState;
        //    }

        //    else if (InheritedRule && !maintconfigs.Any(c => c.VBiACClassID == items.ValueT.ACClassID && (c.IsActive == ActivatedRules || !c.IsActive == DeactivatedRules)))
        //    {
        //        foreach (var config in maintConfigsFilter)
        //        {
        //            if (items.ValueT.IsDerivedClassFrom(config.ACClass))
        //            {
        //                iconState = Global.ConfigIconState.InheritedConfig;
        //                items.IconState = iconState;
        //                continue;
        //            }
        //        }
        //    }

        //    if (recursive)
        //    {
        //        foreach (ACClassInfoWithItems info in items.Items)
        //        {
        //            iconState = CheckConfigRuleMarks(info, maintconfigs);
        //            if (items.IconState == null || (Global.ConfigIconState)items.IconState == Global.ConfigIconState.NoConfig)
        //                items.IconState = iconState;
        //            else if (items.IconState != null && items.IconState is Global.ConfigIconState)
        //                iconState = (Global.ConfigIconState)items.IconState;
        //        }
        //    }
        //    return iconState;
        //}

        //private void ClearConfigRuleMarks(ACClassInfoWithItems items)
        //{
        //    items.IconState = Global.ConfigIconState.NoConfig;
        //    foreach (ACClassInfoWithItems item in items.Items)
        //    {
        //        item.IconState = Global.ConfigIconState.NoConfig;
        //        ClearConfigRuleMarks(item);
        //    }
        //}

        ///// <summary>
        ///// Refreshes the icon state of ACClasses.
        ///// </summary>
        ///// <summary xml:lang="de">
        ///// 
        ///// </summary>
        //[ACMethodInfo("", "en{'Refresh'}de{'Aktualisieren'}", 999, true)]
        //public void Refresh()
        //{
        //    RefreshOnSave(true);
        //}

        //#endregion

        //#region Execute-Helper-Handlers

        //protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        //{
        //    result = null;
        //    switch (acMethodName)
        //    {
        //        case "SetACClassInfoWithItems":
        //            SetACClassInfoWithItems((ACClassInfoWithItems)acParameter[0]);
        //            return true;
        //        case "SetACClass":
        //            SetACClass((ACClass)acParameter[0]);
        //            return true;
        //        case "GenerateNewMaintenanceConfiguration":
        //            GenerateNewMaintenanceConfiguration();
        //            return true;
        //        case "IsEnabledGenerateNewMaintenanceConfiguration":
        //            result = IsEnabledGenerateNewMaintenanceConfiguration();
        //            return true;
        //        case "AddPropertyToMaintACClass":
        //            AddPropertyToMaintACClass();
        //            return true;
        //        case "IsEnabledAddPropertyToMaintACClass":
        //            result = IsEnabledAddPropertyToMaintACClass();
        //            return true;
        //        case "RemovePropertyFromMaintACClass":
        //            RemovePropertyFromMaintACClass();
        //            return true;
        //        case "IsEnabledRemovePropertyFromMaintACClass":
        //            result = IsEnabledRemovePropertyFromMaintACClass();
        //            return true;
        //        case "ApplyChanges":
        //            ApplyChanges();
        //            return true;
        //        case "IsEnabledApplyChanges":
        //            result = IsEnabledApplyChanges();
        //            return true;
        //        case "SearchProperties":
        //            SearchProperties();
        //            return true;
        //        case "CheckIcons":
        //            CheckIcons((ACClassInfoWithItems)acParameter[0], acParameter.Count() == 2 ? (Boolean)acParameter[1] : true);
        //            return true;
        //        case "Refresh":
        //            Refresh();
        //            return true;
        //        case "Delete":
        //            Delete();
        //            return true;
        //        case "IsEnabledDelete":
        //            result = IsEnabledDelete();
        //            return true;
        //    }
        //    return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        //}

        //#endregion
    }
}

