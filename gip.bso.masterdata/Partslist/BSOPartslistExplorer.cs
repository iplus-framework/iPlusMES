using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Windows.Input;
using static gip.core.datamodel.Global;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Bill of Materials'}de{'Stückliste'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Partslist.ClassName)]
    [ACQueryInfo(Const.PackName_VarioMaterial, Const.QueryPrefix + "PartsParamCopy", "en{'Copy Bill of Materials param'}de{'Kopiere Stücklistenparameter'}", typeof(Partslist), Partslist.ClassName, "PartslistName,IsEnabled", "PartslistNo")]
    public class BSOPartslistExplorer : ACBSOvbNav
    {

        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOPartslist"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOPartslistExplorer(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        /// <summary>
        /// ACs the init.
        /// </summary>
        /// <param name="startChildMode">The start child mode.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode)) return false;

            if (BSOMaterialExplorer_Child != null && BSOMaterialExplorer_Child.Value != null)
                BSOMaterialExplorer_Child.Value.PropertyChanged += BSOMaterialExplorer_Child_PropertyChanged;

            _VisitedPartslists = new List<Partslist>();
            _ChangedPartslists = new List<Partslist>();

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {

            PropagatedPartslist = null;
            CurrentPartslist = null;
            SelectedPartslist = null;
            _AccessPrimary = null;

            if (BSOMaterialExplorer_Child != null && BSOMaterialExplorer_Child.Value != null)
                BSOMaterialExplorer_Child.Value.PropertyChanged -= BSOMaterialExplorer_Child_PropertyChanged;

            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }

            _VisitedPartslists = null;
            _ChangedPartslists = null;

            return b;
        }

        private void BSOMaterialExplorer_Child_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedMaterial")
            {
                SetMaterialIDFilter();
            }
        }

        #endregion

        #region ChildBSO

        ACChildItem<BSOMaterialExplorer> _BSOMaterialExplorer_Child;
        [ACPropertyInfo(9999)]
        [ACChildInfo("BSOMaterialExplorer_Child", typeof(BSOMaterialExplorer))]
        public ACChildItem<BSOMaterialExplorer> BSOMaterialExplorer_Child
        {
            get
            {
                if (_BSOMaterialExplorer_Child == null)
                    _BSOMaterialExplorer_Child = new ACChildItem<BSOMaterialExplorer>(this, "BSOMaterialExplorer_Child");
                return _BSOMaterialExplorer_Child;
            }
        }

        #endregion

        #region Properties

        #region Properties -> Filters

        public bool? filterIsEnabled;
        [ACPropertyInfo(9999, "Filter", "en{'Enabled'}de{'Freigegeben'}")]
        public bool? FilterIsEnabled
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<bool?>("IsEnabled");
            }
            set
            {
                bool? tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<bool?>("IsEnabled");
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<bool?>("IsEnabled", value);
                    OnPropertyChanged("FilterIsEnabled");
                }
            }
        }

        private MDSchedulingGroup _FilterMDSchedulingGroup;
        [ACPropertyInfo(9999, "Filter", "en{'Method'}de{'Method'}")]
        public MDSchedulingGroup FilterMDSchedulingGroup
        {
            get
            {
                return _FilterMDSchedulingGroup;
            }
            set

            {
                if (value != _FilterMDSchedulingGroup)
                {
                    _FilterMDSchedulingGroup = value;
                    OnPropertyChanged("FilterMDSchedulingGroup");
                }
            }
        }

        [ACPropertyInfo(5, "", "en{'Filter'}de{'Filter'}")]
        public string SearchWord
        {
            get
            {
                if (_AccessPrimary == null || _AccessPrimary.NavACQueryDefinition == null) return null;
                return _AccessPrimary.NavACQueryDefinition.SearchWord;
            }
            set
            {
                if (_AccessPrimary != null && _AccessPrimary.NavACQueryDefinition != null)
                {
                    if (_AccessPrimary.NavACQueryDefinition.SearchWord != value)
                    {
                        _AccessPrimary.NavACQueryDefinition.SearchWord = value;
                        OnPropertyChanged("SearchWord");
                        if (string.IsNullOrEmpty(value))
                        {
                            ClearSearch();
                            Search();
                        }
                        else
                            Search();
                    }
                }
            }
        }

        #endregion

        #region Properties -> Partslist

        #region Properties -> Partslist -> AccessNav

        public override IAccessNav AccessNav { get { return AccessPrimary; } }

        ACAccessNav<Partslist> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, Partslist.ClassName)]
        public ACAccessNav<Partslist> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceSortColumnsIfDifferent(NavigationqueryDefaultSort);
                        if (navACQueryDefinition.TakeCount == 0)
                            navACQueryDefinition.TakeCount = ACQueryDefinition.C_DefaultTakeCount;
                        if (navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(NavigationqueryDefaultFilter))
                        {
                            SetMaterialIDFilter();
                        }
                        navACQueryDefinition.ACFilterColumns.ListChanged += ACFilterColumns_ListChanged;
                        _AccessPrimary = navACQueryDefinition.NewAccessNav<Partslist>(Partslist.ClassName, this);
                        _AccessPrimary.NavSearchExecuting += AccessPrimaryNavSearchExecuting;
                    }
                }
                return _AccessPrimary;
            }
        }


        Guid? _FilterMaterialID;
        public void SetMaterialIDFilter()
        {
            _FilterMaterialID = null;
            if (BSOMaterialExplorer_Child.Value.SelectedMaterial != null)
            {
                _FilterMaterialID = BSOMaterialExplorer_Child.Value.SelectedMaterial.MaterialID;
            }
            if (_AccessPrimary != null && _AccessPrimary.NavACQueryDefinition != null)
            {
                _AccessPrimary.NavSearch();
                OnPropertyChanged("PartslistList");
            }
        }


        private void ACFilterColumns_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged || e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted)
            {
                BindingList<ACFilterItem> items = sender as BindingList<ACFilterItem>;
                if (items != null)
                {
                    if (e.ListChangedType == ListChangedType.ItemAdded
                            || (e.ListChangedType == ListChangedType.ItemChanged && e.PropertyDescriptor != null && e.PropertyDescriptor.Name == "SearchWord"))
                    {
                        if (e.NewIndex >= 0 && items.Count() > e.NewIndex)
                        {
                            ACFilterItem item = items[e.NewIndex];
                            OnPropertyChangedFromFilter(item.PropertyName);
                        }
                    }
                    else if (e.ListChangedType == ListChangedType.ItemDeleted)
                    {
                        string[] allPropertes = new string[] { "PartslistNo", "PartslistName", "IsEnabled" };
                        string[] missingProperties = allPropertes.Where(c => !items.Select(x => x.PropertyName).Contains(c)).ToArray();
                        foreach (var missingProperty in missingProperties)
                            OnPropertyChangedFromFilter(missingProperty);
                    }
                }
            }
        }

        private List<ACSortItem> NavigationqueryDefaultSort
        {
            get
            {
                List<ACSortItem> acSortItems = new List<ACSortItem>();

                ACSortItem partslistNo = new ACSortItem("PartslistNo", SortDirections.ascending, true);
                acSortItems.Add(partslistNo);

                ACSortItem version = new ACSortItem("PartslistVersion", SortDirections.ascending, true);
                acSortItems.Add(version);

                return acSortItems;
            }
        }

        private void OnPropertyChangedFromFilter(string filterPropertyName)
        {
            switch (filterPropertyName)
            {
                case "IsEnabled":
                    OnPropertyChanged("FilterIsEnabled");
                    break;
            }
        }

        public List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {
                List<ACFilterItem> aCFilterItems = new List<ACFilterItem>();

                ACFilterItem isDeletedFilter = new ACFilterItem(Global.FilterTypes.filter, "DeleteDate", Global.LogicalOperators.isNull, Global.Operators.and, null, true);
                aCFilterItems.Add(isDeletedFilter);

                ACFilterItem phOpen = new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true);
                aCFilterItems.Add(phOpen);

                ACFilterItem partslistNoFilter = new ACFilterItem(FilterTypes.filter, "PartslistNo", LogicalOperators.contains, Operators.or, "", true, true);
                aCFilterItems.Add(partslistNoFilter);

                ACFilterItem filterPartslistName = new ACFilterItem(FilterTypes.filter, "PartslistName", LogicalOperators.contains, Operators.or, "", true, true);
                aCFilterItems.Add(filterPartslistName);

                ACFilterItem phClose = new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true);
                aCFilterItems.Add(phClose);

                ACFilterItem filterIsEnabled = new ACFilterItem(FilterTypes.filter, "IsEnabled", LogicalOperators.contains, Operators.and, "", true);
                aCFilterItems.Add(filterIsEnabled);

                ACFilterItem filterMaterial = new ACFilterItem(FilterTypes.filter, "Material\\MaterialNo", LogicalOperators.contains, Operators.and, "", true);
                aCFilterItems.Add(filterMaterial);

                return aCFilterItems;
            }
        }

        public virtual IQueryable<Partslist> AccessPrimaryNavSearchExecuting(IQueryable<Partslist> result)
        {
            ObjectQuery<Partslist> query = result as ObjectQuery<Partslist>;
            if (query != null)
            {
                query.Include(c => c.Material)
                .Include(c => c.Material.BaseMDUnit)
                .Include(c => c.MaterialWF)
                .Include(c => c.Material.MDUnitList)
                .Include(c => c.PartslistPos_Partslist)
                .Include(c => c.MDUnit);
            }

            Guid? mdSchedulingGroup = null;
            if (FilterMDSchedulingGroup != null)
                mdSchedulingGroup = FilterMDSchedulingGroup.MDSchedulingGroupID;

            result = result
                .Where(x =>
                (
                    _FilterMaterialID == null
                    || x.MaterialID == (_FilterMaterialID ?? Guid.Empty)
                )
                &&
                (
                    mdSchedulingGroup == null
                    || x
                        .MaterialWF
                        .MaterialWFACClassMethod_MaterialWF
                        .SelectMany(c => c.MaterialWFConnection_MaterialWFACClassMethod)
                        .SelectMany(c => c.ACClassWF.MDSchedulingGroupWF_VBiACClassWF)
                        .Select(c => c.MDSchedulingGroupID)
                        .Contains(mdSchedulingGroup ?? Guid.Empty)
                )
             );
            return result;
        }

        #endregion

        #region Properties -> Partslist -> Select, (Current,) List

        public Partslist PropagatedPartslist;

        /// <summary>
        /// Gets the partslist list.
        /// </summary>
        /// <value>The partslist list.</value>
        [ACPropertyList(9999, Partslist.ClassName)]
        public IEnumerable<Partslist> PartslistList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the current partslist.
        /// </summary>
        /// <value>The current partslist.</value>
        [ACPropertyCurrent(9999, Partslist.ClassName)]
        public Partslist CurrentPartslist
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
                    if (CurrentPartslist != null)
                        CurrentPartslist.PropertyChanged -= CurrentPartslist_PropertyChanged;
                    if (value != null)
                        value.PropertyChanged += CurrentPartslist_PropertyChanged;
                    PropagateParslistSelection(value);
                    OnPropertyChanged("CurrentPartslist");
                    OnPropertyChanged("MDUnitList");
                    OnPropertyChanged("CurrentMDUnit");
                    OnPropertyChanged("ConfigurationTransferList");
                    LoadBOM(value);
                    if (value != null)
                        if (!_VisitedPartslists.Any(c => c.PartslistID == value.PartslistID))
                            _VisitedPartslists.Add(value);
                }
            }
        }

        public virtual void CurrentPartslist_PropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Gets or sets the selected partslist.
        /// </summary>
        /// <value>The selected partslist.</value>
        [ACPropertySelected(9999, Partslist.ClassName)]
        public Partslist SelectedPartslist
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary.Selected != value)
                {
                    if (AccessPrimary == null) return;
                    if (AccessPrimary.Selected != value)
                    {
                        if (AccessPrimary.Selected != null)
                            AccessPrimary.Selected.PropertyChanged -= SelectedPartslist_PropertyChanged;
                        AccessPrimary.Selected = value;
                        if (AccessPrimary.Selected != null)
                            AccessPrimary.Selected.PropertyChanged += SelectedPartslist_PropertyChanged;

                        PropagateParslistSelection(value);
                        OnPropertyChanged("SelectedPartslist");
                    }
                }
            }
        }

        public virtual void SelectedPartslist_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        #endregion

        #endregion

        #region Properties -> Tracking Changes
        // Public property while relation BSOPartslistExplorer and BSOPartslist
        private List<Partslist> _VisitedPartslists;
        public List<Partslist> VisitedPartslists
        {
            get
            {
                return _VisitedPartslists;
            }
        }

        private List<Partslist> _ChangedPartslists;
        public List<Partslist> ChangedPartslists
        {
            get
            {
                return _ChangedPartslists;
            }
            set
            {
                _ChangedPartslists = value;
            }
        }

        #endregion

        #region Properties -> BOM

        #region Properties -> BOM -> Tree

        private PartslistExpand RootProdOrderPartListExpand;
        private List<ExpandResult> ExpandResult;
        private PartslistExpand _CurrentProdOrderPartListExpand;
        /// <summary>
        /// 
        /// </summary>
        [ACPropertyCurrent(606, "ProdOrderPartListExpand")]
        public PartslistExpand CurrentProdOrderPartListExpand
        {
            get
            {
                return _CurrentProdOrderPartListExpand;
            }
            set
            {
                if (_CurrentProdOrderPartListExpand != value)
                {
                    _CurrentProdOrderPartListExpand = value;
                    _ProdOrderPartListExpandList = null;
                }
            }
        }

        private List<PartslistExpand> _ProdOrderPartListExpandList;
        [ACPropertyList(607, "ProdOrderPartListExpand")]
        public List<PartslistExpand> ProdOrderPartListExpandList
        {
            get
            {
                if (_ProdOrderPartListExpandList == null)
                    _ProdOrderPartListExpandList = new List<PartslistExpand>();
                if (CurrentProdOrderPartListExpand != null)
                    _ProdOrderPartListExpandList.Add(CurrentProdOrderPartListExpand);
                return _ProdOrderPartListExpandList;
            }
        }

        #endregion

        #region Properties -> BOM components

        #region BOMComponent
        private PartslistPos _SelectedBOMComponent;
        /// <summary>
        /// Selected property for PartslistPos
        /// </summary>
        /// <value>The selected BOMComponent</value>
        [ACPropertySelected(9999, "BOMComponent", "en{'TODO: BOMComponent'}de{'TODO: BOMComponent'}")]
        public PartslistPos SelectedBOMComponent
        {
            get
            {
                return _SelectedBOMComponent;
            }
            set
            {
                if (_SelectedBOMComponent != value)
                {
                    _SelectedBOMComponent = value;
                    OnPropertyChanged(nameof(SelectedBOMComponent));
                }
            }
        }


        private List<PartslistPos> _BOMComponentList;
        /// <summary>
        /// List property for PartslistPos
        /// </summary>
        /// <value>The BOMComponent list</value>
        [ACPropertyList(9999, "BOMComponent")]
        public List<PartslistPos> BOMComponentList
        {
            get
            {
                return _BOMComponentList;
            }
        }

        #endregion


        #endregion

        #endregion

        #endregion

        #region Partslist -> Methods

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(Partslist.ClassName, "en{'Search material'}de{'Stückliste Suche'}", (short)MISort.Search)]
        public void Search(Partslist selectedPartslist = null)
        {
            if (!PreExecute("Search")) return;
            if (AccessPrimary == null) return;
            AccessPrimary.NavSearch(DatabaseApp);

            if (selectedPartslist != null)
            {
                AccessPrimary.Selected = null;
                PropagatedPartslist = null;
                SelectedPartslist = selectedPartslist;
            }
            if (AccessPrimary != null && AccessPrimary.Selected != null)
            {
                AccessPrimary.Selected.PropertyChanged -= SelectedPartslist_PropertyChanged;
                AccessPrimary.Selected.PropertyChanged += SelectedPartslist_PropertyChanged;
            }
            OnPropertyChanged("PartslistList");
            PostExecute("Search");
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(Partslist.ClassName, "en{'Remove'}de{'Entfernen'}", (short)MISort.Search)]
        public void ClearSearch()
        {
            if (!PreExecute("ClearSearch")) return;
            if (AccessPrimary == null) return;
            AccessPrimary.NavList.Clear();
            SelectedPartslist = null;
            CurrentPartslist = null;
            OnPropertyChanged("PartslistList");
            PostExecute("ClearSearch");
        }

        /// <summary>
        /// Helper 
        /// </summary>
        public virtual void PropagateParslistSelection(Partslist partsList)
        {

        }

        [ACMethodInfo("", "en{'Key event'}de{'Tastatur Ereignis'}", 9999, false)]
        public void OnKeyEvent(KeyEventArgs e)
        {
            IVBContent control = e.Source as IVBContent;
            if (control != null && control.VBContent == "SearchWord")
            {
                if (e.Key == Key.Enter)
                {
                    Search();
                }
            }
        }

        #region Method -> BOM


        /// <summary>
        /// Source Property: ShowBOM
        /// </summary>
        public void LoadBOM(Partslist partslist)
        {
            _BOMComponentList = null;
            RootProdOrderPartListExpand = null;
            _CurrentProdOrderPartListExpand = null;
            _ProdOrderPartListExpandList = null;

            ExpandResult = null;

            if (partslist != null)
            {
                RootProdOrderPartListExpand = new PartslistExpand(partslist, 1, 1);
                RootProdOrderPartListExpand.LoadTree();
                RootProdOrderPartListExpand.IsEnabled = true;
                RootProdOrderPartListExpand.IsChecked = false;


                _CurrentProdOrderPartListExpand = RootProdOrderPartListExpand;

                ExpandResult = new List<ExpandResult>();
                RootProdOrderPartListExpand.BuildTreeList(ExpandResult);
                foreach (var item in ExpandResult)
                {
                    item.Item.PropertyChanged += PartslistExpand_PropertyChanged;
                }

                _BOMComponentList = LoadBOMComponentList();
            }

            OnPropertyChanged(nameof(BOMComponentList));
            OnPropertyChanged(nameof(RootProdOrderPartListExpand));
            OnPropertyChanged(nameof(CurrentProdOrderPartListExpand));
            OnPropertyChanged(nameof(ProdOrderPartListExpandList));
        }

        private void PartslistExpand_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PartslistExpand.IsChecked))
            {
                _BOMComponentList = LoadBOMComponentList();
                OnPropertyChanged(nameof(BOMComponentList));
            }
        }

        private List<PartslistPos> LoadBOMComponentList()
        {
            List<PartslistPos> positions = new List<PartslistPos>();

            if (ExpandResult != null)
            {
                ExpandResult[] items = ExpandResult.OrderByDescending(c => c.TreeVersion).Where(c => c.Item.IsChecked).ToArray();
                foreach (ExpandResult item in items)
                {
                    Partslist pl = item.Item.Item as Partslist;
                    PartslistPos[] components = pl.PartslistPos_Partslist.Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot).OrderBy(c => c.Sequence).ToArray();
                    List<PartslistPos> clonedComponents = new List<PartslistPos>();
                    foreach (PartslistPos component in components)
                    {
                        PartslistPos clonedComponent = component.Clone(true) as PartslistPos;
                        clonedComponent.TargetQuantityUOM *= item.Item.TreeQuantityRatio;
                        clonedComponent.PartslistPosID = Guid.NewGuid();
                        DatabaseApp.ObjectStateManager.ChangeObjectState(clonedComponent, System.Data.EntityState.Unchanged);
                        clonedComponents.Add(clonedComponent);
                    }
                    positions.AddRange(clonedComponents);
                }
            }

            return positions;
        }

        #endregion

        #endregion
    }
}
