using gip.bso.masterdata;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Objects;
using System.Linq;
using static gip.core.datamodel.Global;

namespace gip.bso.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Warehouse Inventory'}de{'Lager Inventur'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + FacilityInventory.ClassName)]
    [ACQueryInfo(Const.PackName_VarioFacility, Const.QueryPrefix + "FacilityInventory", "en{'FacilityInventory'}de{'FacilityInventory'}", typeof(FacilityInventory), FacilityInventory.ClassName, "FacilityInventoryNo", "FacilityInventoryNo")]

    public class BSOFacilityInventory : ACBSOvbNav, IOnTrackingCall
    {
        #region contants
        public const string BGWorkerMehtod_DoNewInventory = @"DoNewInventory";
        public const string BGWorkerMehtod_DoGeneratePositions = @"DoGeneratePositions";
        public const string BGWorkerMehtod_DoInventoryClosing = @"DoInventoryClosing";

        #endregion

        #region c´tors
        /// <summary>Initializes a new instance of the class.</summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityInventory(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
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

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            AccessFilterInventoryPosStorageLocation.NavSearch();
            AccessFilterInventoryPosFacility.NavSearch();
            AccessFilterInventoryPosMaterial.NavSearch();
            SelectedFilterInventoryPosStorageLocation = null;
            SelectedFilterInventoryPosFacility = null;
            SelectedFilterInventoryPosMaterial = null;

            LoadInitialFilterInventoryDates();
            IsEnabledInventoryPosEdit = false;
            Search();
            return true;
        }

        private void LoadInitialFilterInventoryDates()
        {
            FilterInventoryStartDate = new DateTime(DateTime.Now.Year, 1, 1);
            FilterInventoryEndDate = FilterInventoryStartDate.AddYears(1);
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            var b = base.ACDeInit();

            if (_ACFacilityManager != null)
                FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;

            _AccessFilterInventoryPosFacility = null;
            _AccessFilterInventoryPosMaterial = null;
            return b;
        }

        #endregion

        #region Managers

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

        #region ChildBSO

        ACChildItem<BSOFacilityExplorer> _BSOFacilityExplorer_Child;
        [ACPropertyInfo(600)]
        [ACChildInfo("BSOFacilityExplorer_Child", typeof(BSOFacilityExplorer))]
        public ACChildItem<BSOFacilityExplorer> BSOFacilityExplorer_Child
        {
            get
            {
                if (_BSOFacilityExplorer_Child == null)
                    _BSOFacilityExplorer_Child = new ACChildItem<BSOFacilityExplorer>(this, "BSOFacilityExplorer_Child");
                return _BSOFacilityExplorer_Child;
            }
        }

        #endregion

        #region Properties

        #region Properties -> Filter

        #region Properties -> Filter -> Inventory

        private DateTime _FilterInventoryStartDate;
        [ACPropertyInfo(90, "FilterInventoryStartDate", "en{'From'}de{'Von'}")]
        public DateTime FilterInventoryStartDate
        {
            get
            {
                return _FilterInventoryStartDate;
            }
            set
            {
                if (_FilterInventoryStartDate != value)
                {
                    _FilterInventoryStartDate = value;
                    OnPropertyChanged(nameof(FilterInventoryStartDate));
                }
            }
        }

        private DateTime _FilterInventoryEndDate;
        [ACPropertyInfo(101, "FilterInventoryEndDate", "en{'to'}de{'bis'}")]
        public DateTime FilterInventoryEndDate
        {
            get
            {
                return _FilterInventoryEndDate;
            }
            set
            {
                if (_FilterInventoryEndDate != value)
                {
                    _FilterInventoryEndDate = value;
                    OnPropertyChanged(nameof(FilterInventoryEndDate));
                }
            }
        }

        #region Properties -> Filter-> FilterInventoryState (MDFacilityInventoryState)

        private MDFacilityInventoryState _SelectedFilterInventoryState;
        /// <summary>
        /// Selected property for MDFacilityInventoryState
        /// </summary>
        /// <value>The selected FilterInventoryState</value>
        [ACPropertySelected(9999, "FilterInventoryState", ConstApp.ESFacilityInventoryState)]
        public MDFacilityInventoryState SelectedFilterInventoryState
        {
            get
            {
                return _SelectedFilterInventoryState;
            }
            set
            {
                if (_SelectedFilterInventoryState != value)
                {
                    _SelectedFilterInventoryState = value;
                    OnPropertyChanged(nameof(SelectedFilterInventoryState));
                }
            }
        }


        private List<MDFacilityInventoryState> _FilterInventoryStateList;
        /// <summary>
        /// List property for MDFacilityInventoryState
        /// </summary>
        /// <value>The FilterInventoryState list</value>
        [ACPropertyList(9999, "FilterInventoryState")]
        public List<MDFacilityInventoryState> FilterInventoryStateList
        {
            get
            {
                if (_FilterInventoryStateList == null)
                    _FilterInventoryStateList = LoadFilterInventoryStateList();
                return _FilterInventoryStateList;
            }
        }

        private List<MDFacilityInventoryState> LoadFilterInventoryStateList()
        {
            return DatabaseApp.MDFacilityInventoryState.OrderBy(c => c.SortIndex).ToList();
        }

        #endregion


        /// <summary>
        /// Source Property: 
        /// </summary>
        private bool _GenerateInventoryPosition;
        [ACPropertySelected(999, "GenerateInventoryPosition", "en{'Generate positions'}de{'Positionen generieren'}")]
        public bool GenerateInventoryPosition
        {
            get
            {
                return _GenerateInventoryPosition;
            }
            set
            {
                if (_GenerateInventoryPosition != value)
                {
                    _GenerateInventoryPosition = value;
                    OnPropertyChanged(nameof(GenerateInventoryPosition));
                }
            }
        }


        #region Properties -> Filter -> NewInventoryFacility

        private Facility _SelectedNewInventoryFacility;
        /// <summary>
        /// Selected property for Facility
        /// </summary>
        /// <value>The selected NewInventoryFacility</value>
        [ACPropertySelected(9999, "NewInventoryFacility", ConstApp.Facility)]
        public Facility SelectedNewInventoryFacility
        {
            get
            {
                return _SelectedNewInventoryFacility;
            }
            set
            {
                if (_SelectedNewInventoryFacility != value)
                {
                    _SelectedNewInventoryFacility = value;
                    OnPropertyChanged(nameof(SelectedNewInventoryFacility));
                }
            }
        }

        private List<Facility> _NewInventoryFacilityList;
        /// <summary>
        /// List property for Facility
        /// </summary>
        /// <value>The NewInventoryFacility list</value>
        [ACPropertyList(9999, "NewInventoryFacility")]
        public List<Facility> NewInventoryFacilityList
        {
            get
            {
                if (_NewInventoryFacilityList == null)
                    _NewInventoryFacilityList = DatabaseApp.Facility.OrderBy(c => c.FacilityNo).ToList();
                return _NewInventoryFacilityList;
            }
        }

        #endregion

        #endregion

        #region Properties -> Filter -> InventoryPos

        #region Properties -> Filter -> InventoryPos -> FilterInventoryPosStorageLocation

        ACAccessNav<Facility> _AccessFilterInventoryPosStorageLocation;
        [ACPropertyAccess(400, "FilterInventoryPosStorageLocation")]
        public ACAccessNav<Facility> AccessFilterInventoryPosStorageLocation
        {
            get
            {
                if (_AccessFilterInventoryPosStorageLocation == null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "Facility", Facility.ClassName);
                    navACQueryDefinition.ACFilterColumns.Clear();
                    navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(FilterInventoryPosStorageLocationDefaultFilter);
                    navACQueryDefinition.CheckAndReplaceSortColumnsIfDifferent(FilterInventoryPosStorageLocationDefaultSort);
                    _AccessFilterInventoryPosStorageLocation = navACQueryDefinition.NewAccessNav<Facility>("Facility", this);
                    _AccessFilterInventoryPosStorageLocation.AutoSaveOnNavigation = false;
                }
                return _AccessFilterInventoryPosStorageLocation;
            }
        }


        public List<ACFilterItem> FilterInventoryPosStorageLocationDefaultFilter
        {
            get
            {

                List<ACFilterItem> aCFilterItems = new List<ACFilterItem>();

                // MDFacilityTypeIndex
                ACFilterItem facilityTypeFilter = new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, ((short)FacilityTypesEnum.StorageLocation).ToString(), true);
                aCFilterItems.Add(facilityTypeFilter);

                return aCFilterItems;
            }
        }

        private List<ACSortItem> FilterInventoryPosStorageLocationDefaultSort
        {
            get
            {
                List<ACSortItem> acSortItems = new List<ACSortItem>();

                ACSortItem facilityInventoryNo = new ACSortItem("FacilityNo", SortDirections.ascending, true);
                acSortItems.Add(facilityInventoryNo);

                return acSortItems;
            }
        }



        /// <summary>
        /// Gets or sets the selected FilterInventoryPosStorageLocation.
        /// </summary>
        /// <value>The selected FilterInventoryPosStorageLocation.</value>
        [ACPropertySelected(401, "FilterInventoryPosStorageLocation", ConstApp.Facility)]
        public Facility SelectedFilterInventoryPosStorageLocation
        {
            get
            {
                if (AccessFilterInventoryPosStorageLocation == null)
                    return null;
                return AccessFilterInventoryPosStorageLocation.Selected;
            }
            set
            {
                if (AccessFilterInventoryPosStorageLocation == null)
                    return;
                if (AccessFilterInventoryPosStorageLocation.Selected != value)
                {
                    AccessFilterInventoryPosStorageLocation.Selected = value;
                    OnPropertyChanged(nameof(SelectedFilterInventoryPosStorageLocation));

                    if (FilterParentFacilityNo != null)
                        FilterParentFacilityNo.SearchWord = value != null ? value.FacilityNo : null;
                    AccessFilterInventoryPosFacility.NavSearch();
                    OnPropertyChanged(nameof(FilterInventoryPosFacilityList));
                    SelectedFilterInventoryPosFacility = null;

                    SearchPos();
                }
            }
        }

        /// <summary>
        /// Gets the FilterInventoryPosStorageLocation list.
        /// </summary>
        /// <value>The facility list.</value>
        [ACPropertyList(502, "FilterInventoryPosStorageLocation")]
        public IEnumerable<Facility> FilterInventoryPosStorageLocationList
        {
            get
            {
                if (AccessFilterInventoryPosStorageLocation == null)
                    return null;
                return AccessFilterInventoryPosStorageLocation.NavList;
            }
        }

        #endregion

        #region Properties -> Filter -> InventoryPos -> Facility

        ACAccessNav<Facility> _AccessFilterInventoryPosFacility;
        [ACPropertyAccess(500, "FilterInventoryPosFacility")]
        public ACAccessNav<Facility> AccessFilterInventoryPosFacility
        {
            get
            {
                if (_AccessFilterInventoryPosFacility == null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "Facility", Facility.ClassName);
                    navACQueryDefinition.ACFilterColumns.Clear();
                    navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(FilterInventoryPosFacilityDefaultFilter);
                    navACQueryDefinition.CheckAndReplaceSortColumnsIfDifferent(FilterInventoryPosFacilityDefaultSort);
                    _AccessFilterInventoryPosFacility = navACQueryDefinition.NewAccessNav<Facility>("Facility", this);
                    _AccessFilterInventoryPosFacility.AutoSaveOnNavigation = false;
                }
                return _AccessFilterInventoryPosFacility;
            }
        }

        public ACFilterItem FilterParentFacilityNo
        {
            get
            {
                if (_AccessFilterInventoryPosFacility == null) return null;
                return _AccessFilterInventoryPosFacility.NavACQueryDefinition.ACFilterColumns.FirstOrDefault(c => c.PropertyName == "Facility1_ParentFacility\\FacilityNo");
            }
        }

        public List<ACFilterItem> FilterInventoryPosFacilityDefaultFilter
        {
            get
            {

                List<ACFilterItem> aCFilterItems = new List<ACFilterItem>();

                // MDFacilityTypeIndex
                ACFilterItem facilityTypeFilter = new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, ((short)FacilityTypesEnum.StorageBin).ToString(), true);
                aCFilterItems.Add(facilityTypeFilter);

                // Parent facility
                ACFilterItem parentFacilityNoFilter = new ACFilterItem(Global.FilterTypes.filter, "Facility1_ParentFacility\\FacilityNo", Global.LogicalOperators.equal, Global.Operators.and, FilterParentFacilityNo?.SearchWord, true);
                aCFilterItems.Add(parentFacilityNoFilter);

                return aCFilterItems;
            }
        }

        private List<ACSortItem> FilterInventoryPosFacilityDefaultSort
        {
            get
            {
                List<ACSortItem> acSortItems = new List<ACSortItem>();

                ACSortItem facilityInventoryNo = new ACSortItem("FacilityNo", SortDirections.ascending, true);
                acSortItems.Add(facilityInventoryNo);

                return acSortItems;
            }
        }

        /// <summary>
        /// Gets or sets the selected FilterInventoryPosFacility.
        /// </summary>
        /// <value>The selected FilterInventoryPosFacility.</value>
        [ACPropertySelected(501, "FilterInventoryPosFacility", ConstApp.Facility)]
        public Facility SelectedFilterInventoryPosFacility
        {
            get
            {
                if (AccessFilterInventoryPosFacility == null)
                    return null;
                return AccessFilterInventoryPosFacility.Selected;
            }
            set
            {
                if (AccessFilterInventoryPosFacility == null)
                    return;
                if (AccessFilterInventoryPosFacility.Selected != value)
                {
                    AccessFilterInventoryPosFacility.Selected = value;
                    OnPropertyChanged(nameof(SelectedFilterInventoryPosFacility));
                    SearchPos();
                }
            }
        }

        /// <summary>
        /// Gets the FilterInventoryPosFacility list.
        /// </summary>
        /// <value>The facility list.</value>
        [ACPropertyList(502, "FilterInventoryPosFacility")]
        public IEnumerable<Facility> FilterInventoryPosFacilityList
        {
            get
            {
                if (AccessFilterInventoryPosFacility == null)
                    return null;
                return AccessFilterInventoryPosFacility.NavList;
            }
        }

        #endregion

        #region Properties -> Filter -> InventoryPos -> Material (FilterInventoryPosMaterial)

        ACAccessNav<Material> _AccessFilterInventoryPosMaterial;
        [ACPropertyAccess(200, "FilterInventoryPosMaterial")]
        public ACAccessNav<Material> AccessFilterInventoryPosMaterial
        {
            get
            {
                if (_AccessFilterInventoryPosMaterial == null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "Material", Material.ClassName);
                    _AccessFilterInventoryPosMaterial = navACQueryDefinition.NewAccessNav<Material>("Material", this);
                    _AccessFilterInventoryPosMaterial.AutoSaveOnNavigation = false;
                }
                return _AccessFilterInventoryPosMaterial;
            }
        }

        /// <summary>
        /// Gets or sets the selected FilterInventoryPosMaterial.
        /// </summary>
        /// <value>The selected FilterInventoryPosMaterial.</value>
        [ACPropertySelected(201, "FilterInventoryPosMaterial", ConstApp.Material)]
        public Material SelectedFilterInventoryPosMaterial
        {
            get
            {
                if (AccessFilterInventoryPosMaterial == null)
                    return null;
                return AccessFilterInventoryPosMaterial.Selected;
            }
            set
            {
                if (AccessFilterInventoryPosMaterial == null)
                    return;
                AccessFilterInventoryPosMaterial.Selected = value;
                OnPropertyChanged(nameof(SelectedFilterInventoryPosMaterial));
                SearchPos();
            }
        }

        /// <summary>
        /// Gets the FilterInventoryPosMaterial list.
        /// </summary>
        /// <value>The facility list.</value>
        [ACPropertyList(202, "FilterInventoryPosMaterial")]
        public IEnumerable<Material> FilterInventoryPosMaterialList
        {
            get
            {
                if (AccessFilterInventoryPosMaterial == null)
                    return null;
                return AccessFilterInventoryPosMaterial.NavList;
            }
        }

        #endregion

        #region Properties -> Filter -> Pos (Quantity) State

        private string _FilterInventoryPosLotNo;
        [ACPropertyInfo(104, "FilterInventoryPosLotNo", ConstApp.LotNo)]
        public string FilterInventoryPosLotNo
        {
            get
            {
                return _FilterInventoryPosLotNo;
            }
            set
            {
                if (_FilterInventoryPosLotNo != value)
                {
                    _FilterInventoryPosLotNo = value;
                    OnPropertyChanged(nameof(FilterInventoryPosLotNo));
                    SearchPos();
                }
            }
        }

        private bool? _FilterInventoryPosNotAvailable;
        [ACPropertyInfo(105, "FilterInventoryPosNotAvailable", ConstApp.NotAvailable)]
        public bool? FilterInventoryPosNotAvailable
        {
            get
            {
                return _FilterInventoryPosNotAvailable;
            }
            set
            {
                if (_FilterInventoryPosNotAvailable != value)
                {
                    _FilterInventoryPosNotAvailable = value;
                    OnPropertyChanged(nameof(FilterInventoryPosNotAvailable));
                    SearchPos();
                }
            }
        }

        private bool? _FilterInventoryPosZeroQuantity;
        [ACPropertyInfo(105, "FilterInventoryPosNotAvailable", "en{'Zero Quantity'}de{'Null-Menge'}")]
        public bool? FilterInventoryPosZeroQuantity
        {
            get
            {
                return _FilterInventoryPosZeroQuantity;
            }
            set
            {
                if (_FilterInventoryPosZeroQuantity != value)
                {
                    _FilterInventoryPosZeroQuantity = value;
                    OnPropertyChanged(nameof(FilterInventoryPosZeroQuantity));
                    SearchPos();
                }
            }
        }

        private bool? _FilterOpenLines;
        [ACPropertyInfo(106, "FilterOpenLines", "en{'View open lines'}de{'Offene Zeilen anzeigen'}")]
        public bool? FilterOpenLines
        {
            get
            {
                return _FilterOpenLines;
            }
            set
            {
                if (_FilterOpenLines != value)
                {
                    _FilterOpenLines = value;
                    OnPropertyChanged(nameof(FilterOpenLines));
                    SearchPos();
                }
            }
        }

        #endregion

        #region Properties -> Filter-> State (FilterInventoryPosState) (MDFacilityInventoryPosState)

        private MDFacilityInventoryPosState _SelectedFilterInventoryPosState;
        /// <summary>
        /// Selected property for MDFacilityInventoryPosState
        /// </summary>
        /// <value>The selected FilterInventoryPosState</value>
        [ACPropertySelected(9999, "FilterInventoryPosState", ConstApp.ESFacilityInventoryPosState)]
        public MDFacilityInventoryPosState SelectedFilterInventoryPosState
        {
            get
            {
                return _SelectedFilterInventoryPosState;
            }
            set
            {
                if (_SelectedFilterInventoryPosState != value)
                {
                    _SelectedFilterInventoryPosState = value;
                    OnPropertyChanged(nameof(SelectedFilterInventoryPosState));
                    SearchPos();
                }
            }
        }


        private List<MDFacilityInventoryPosState> _FilterInventoryPosStateList;
        /// <summary>
        /// List property for MDFacilityInventoryPosState
        /// </summary>
        /// <value>The FilterInventoryPosState list</value>
        [ACPropertyList(9999, "FilterInventoryPosState")]
        public List<MDFacilityInventoryPosState> FilterInventoryPosStateList
        {
            get
            {
                if (_FilterInventoryPosStateList == null)
                    _FilterInventoryPosStateList = LoadFilterInventoryPosStateList();
                return _FilterInventoryPosStateList;
            }
        }

        private List<MDFacilityInventoryPosState> LoadFilterInventoryPosStateList()
        {
            return DatabaseApp.MDFacilityInventoryPosState.OrderBy(c => c.SortIndex).ToList();
        }

        #endregion

        private int _FilterPosPageSize = 500;
        [ACPropertyInfo(120, "FilterPosPageSize", "en{'Limit Record Count'}de{'Limit Anzahl Datensätze'}")]
        public int FilterPosPageSize
        {
            get
            {
                return _FilterPosPageSize;
            }
            set
            {
                if (_FilterPosPageSize != value)
                {
                    _FilterPosPageSize = value;
                    OnPropertyChanged(nameof(FilterPosPageSize));
                }
            }
        }

        #endregion

        #endregion

        #region Properties -> Editor

        #region Properties -> Editor -> Pos Editor Enabled

        private bool _IsEnabledInventoryEdit;
        public bool IsEnabledInventoryEdit
        {
            get
            {
                return _IsEnabledInventoryEdit;
            }
            set
            {
                if (_IsEnabledInventoryEdit != value)
                {
                    _IsEnabledInventoryEdit = value;
                    OnPropertyChanged(nameof(IsEnabledInventoryEdit));
                    InventoryDisabledModes = IsEnabledInventoryEdit ? "" : "Disabled";
                }
            }
        }

        private bool _IsEnabledInventoryPosEdit;
        public bool IsEnabledInventoryPosEdit
        {
            get
            {
                return _IsEnabledInventoryPosEdit;
            }
            set
            {
                _IsEnabledInventoryPosEdit = value;
                OnPropertyChanged(nameof(IsEnabledInventoryPosEdit));
                InventoryPosDisabledModes = IsEnabledInventoryPosEdit ? "" : "Disabled";
            }
        }

        private string _InventoryDisabledModes = "Enabled";
        public string InventoryDisabledModes
        {
            get
            {
                return _InventoryDisabledModes;
            }
            set
            {
                _InventoryDisabledModes = value;
                OnPropertyChanged(nameof(InventoryDisabledModes));
            }
        }


        private string _InventoryPosDisabledModes = "Enabled";
        public string InventoryPosDisabledModes
        {
            get
            {
                return _InventoryPosDisabledModes;
            }
            set
            {
                if (_InventoryPosDisabledModes != value)
                {
                    _InventoryPosDisabledModes = value;
                    OnPropertyChanged(nameof(InventoryPosDisabledModes));
                }
            }
        }

        #endregion

        #region Properties -> Editor -> InputCode


        private string _InputCode;
        /// <summary>
        /// Doc  InputCode
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "InputCode", "en{'Barcode'}de{'Barcode'}")]
        public string InputCode
        {
            get
            {
                return _InputCode;
            }
            set
            {
                if (_InputCode != value)
                {
                    _InputCode = value;
                    OnPropertyChanged(nameof(InputCode));
                    SearchPos();
                }
            }
        }


        private string _NewFaciltiyInventoryNo;
        /// <summary>
        /// Doc  InputCode
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "NewFaciltiyInventoryNo", "en{'Inventory Nr.'}de{'Inventurno.'}")]
        public string NewFaciltiyInventoryNo
        {
            get
            {
                return _NewFaciltiyInventoryNo;
            }
            set
            {
                if (_NewFaciltiyInventoryNo != value)
                {
                    _NewFaciltiyInventoryNo = value;
                    OnPropertyChanged(nameof(NewFaciltiyInventoryNo));
                }
            }
        }


        private string _NewFaciltiyInventoryName;
        /// <summary>
        /// Doc  InputCode
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "NewFaciltiyInventoryName", "en{'Name'}de{'Name'}")]
        public string NewFaciltiyInventoryName
        {
            get
            {
                return _NewFaciltiyInventoryName;
            }
            set
            {
                if (_NewFaciltiyInventoryName != value)
                {
                    _NewFaciltiyInventoryName = value;
                    OnPropertyChanged(nameof(NewFaciltiyInventoryName));
                }
            }
        }

        #endregion

        #endregion

        #region Properties -> AccessNav

        public override IAccessNav AccessNav { get { return AccessPrimary; } }

        ACAccessNav<FacilityInventory> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, FacilityInventory.ClassName)]
        public ACAccessNav<FacilityInventory> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    if (navACQueryDefinition != null)
                    {
                        //navACQueryDefinition.CheckAndReplaceSortColumnsIfDifferent(NavigationqueryDefaultSort);
                        if (navACQueryDefinition.TakeCount == 0)
                            navACQueryDefinition.TakeCount = ACQueryDefinition.C_DefaultTakeCount;
                        navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(NavigationqueryDefaultFilter);
                        navACQueryDefinition.CheckAndReplaceSortColumnsIfDifferent(NavigationqueryDefaultSort);
                        _AccessPrimary = navACQueryDefinition.NewAccessNav<FacilityInventory>(FacilityInventory.ClassName, this);
                        _AccessPrimary.NavSearchExecuting += AccessPrimaryNavSearchExecuting;
                    }
                }
                return _AccessPrimary;
            }
        }

        private List<ACSortItem> NavigationqueryDefaultSort
        {
            get
            {
                List<ACSortItem> acSortItems = new List<ACSortItem>();

                ACSortItem facilityInventoryNo = new ACSortItem("FacilityInventoryNo", SortDirections.descending, true);
                acSortItems.Add(facilityInventoryNo);

                return acSortItems;
            }
        }

        public List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {

                List<ACFilterItem> aCFilterItems = new List<ACFilterItem>();

                // facilityInventoryNoFilter
                ACFilterItem facilityInventoryNoFilter = new ACFilterItem(Global.FilterTypes.filter, "FacilityInventoryNo", Global.LogicalOperators.equal, Global.Operators.and, null, true, true);
                aCFilterItems.Add(facilityInventoryNoFilter);

                // MDFacilityInventoryStateID
                ACFilterItem stateFilter = new ACFilterItem(Global.FilterTypes.filter, "MDFacilityInventoryState\\MDKey", Global.LogicalOperators.equal, Global.Operators.and, null, true);
                aCFilterItems.Add(stateFilter);

                // FilterInventoryStartDate
                ACFilterItem fromDate = new ACFilterItem(Global.FilterTypes.filter, "InsertDate", Global.LogicalOperators.greaterThanOrEqual, Global.Operators.and, null, true);
                aCFilterItems.Add(fromDate);

                // FilterInventoryEndDate
                ACFilterItem toDate = new ACFilterItem(Global.FilterTypes.filter, "InsertDate", Global.LogicalOperators.lessThan, Global.Operators.and, null, true);
                aCFilterItems.Add(toDate);

                return aCFilterItems;
            }
        }

        public virtual IQueryable<FacilityInventory> AccessPrimaryNavSearchExecuting(IQueryable<FacilityInventory> result)
        {
            ObjectQuery<FacilityInventory> query = result as ObjectQuery<FacilityInventory>;
            if (query != null)
            {
                query
                .Include(c => c.MDFacilityInventoryState)
                .Include(c => c.FacilityInventoryPos_FacilityInventory)
                .Include("FacilityInventoryPos_FacilityInventory.FacilityCharge")
                .Include("FacilityInventoryPos_FacilityInventory.FacilityCharge.Material")
                .Include("FacilityInventoryPos_FacilityInventory.FacilityCharge.FacilityLot");

            }

            Guid? mdFacilityInventoryStateID = null;
            if (SelectedFilterInventoryState != null)
                mdFacilityInventoryStateID = SelectedFilterInventoryState.MDFacilityInventoryStateID;

            result = result
                .Where(x =>
                (
                    mdFacilityInventoryStateID == null
                    || x.MDFacilityInventoryStateID == mdFacilityInventoryStateID
                )
                && x.InsertDate >= FilterInventoryStartDate
                && x.InsertDate < FilterInventoryEndDate
             );
            return query;
        }


        #endregion

        #region Property -> FacilityInventory

        private FacilityInventory _SelectedFacilityInventory;
        /// <summary>
        /// Selected property for FacilityInventory
        /// </summary>
        /// <value>The selected FacilityInventory</value>
        [ACPropertySelected(9999, "FacilityInventory", "en{'TODO: FacilityInventory'}de{'TODO: FacilityInventory'}")]
        public FacilityInventory SelectedFacilityInventory
        {
            get
            {
                return _SelectedFacilityInventory;
            }
            set
            {
                if (_SelectedFacilityInventory != value)
                {
                    _SelectedFacilityInventory = value;

                    if (_SelectedFacilityInventory != null)
                        IsEnabledInventoryEdit = _SelectedFacilityInventory.MDFacilityInventoryState.MDFacilityInventoryStateIndex == (short)FacilityInventoryStateEnum.InProgress;
                    else
                        IsEnabledInventoryEdit = false;

                    OnPropertyChanged(nameof(SelectedFacilityInventory));

                    SetFacilityInventoryPosList();

                    ClearFilterInventoryFacilityBookingCharge();
                    ClearInventoryFacilityBookingCharge();

                    ClearFilterNotUsedCharge();
                    ClearNotUserdFaciltiyChargeList();

                    if (_SelectedFacilityInventory != null)
                        _SelectedFacilityInventory.PropertyChanged += _SelectedFacilityInventory_PropertyChanged;
                }
            }
        }

        private void _SelectedFacilityInventory_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FacilityInventory item = sender as FacilityInventory;
            if (item != null)
                switch (e.PropertyName)
                {
                    case nameof(FacilityInventory.MDFacilityInventoryStateID):
                        IsEnabledInventoryEdit = item.MDFacilityInventoryState.MDFacilityInventoryStateIndex == (short)FacilityInventoryStateEnum.InProgress;
                        break;
                }
        }

        private List<FacilityInventory> _FacilityInventoryList;
        /// <summary>
        /// List property for FacilityInventory
        /// </summary>
        /// <value>The FacilityInventory list</value>
        [ACPropertyList(9999, "FacilityInventory")]
        public List<FacilityInventory> FacilityInventoryList
        {
            get
            {
                if (_FacilityInventoryList == null)
                    _FacilityInventoryList = LoadFacilityInventoryList();
                return _FacilityInventoryList;
            }
        }

        private List<FacilityInventory> LoadFacilityInventoryList()
        {
            if (AccessPrimary == null || AccessPrimary.NavList == null) return null;
            return AccessPrimary.NavList.ToList();
        }

        #endregion

        #region Property -> FacilityInventoryPos

        private FacilityInventoryPos _SelectedFacilityInventoryPos;
        /// <summary>
        /// Selected property for FacilityInventoryPos
        /// </summary>
        /// <value>The selected FacilityInventoryPos</value>
        [ACPropertySelected(9999, "FacilityInventoryPos", "en{'TODO: FacilityInventoryPos'}de{'TODO: FacilityInventoryPos'}")]
        public FacilityInventoryPos SelectedFacilityInventoryPos
        {
            get
            {
                return _SelectedFacilityInventoryPos;
            }
            set
            {
                if (_SelectedFacilityInventoryPos != value)
                {
                    _SelectedFacilityInventoryPos = value;
                    if (_SelectedFacilityInventoryPos != null)
                    {
                        _SelectedFacilityInventoryPos.PropertyChanged -= _SelectedFacilityInventoryPos_PropertyChanged;
                        _SelectedFacilityInventoryPos.PropertyChanged += _SelectedFacilityInventoryPos_PropertyChanged;
                    }
                    IsEnabledInventoryPosEdit = IsInventoryPosEnabledEdit();
                    OnPropertyChanged(nameof(SelectedFacilityInventoryPos));

                    if (SelectedFacilityInventoryPos != null)
                    {
                        SelectedFacilityInventoryPos.PropertyChanged -= SelectedFacilityInventoryPos_PropertyChanged;
                        SelectedFacilityInventoryPos.PropertyChanged += SelectedFacilityInventoryPos_PropertyChanged;
                    }
                    SetInventoryPosFacilityBookingList();
                }
            }
        }

        private void _SelectedFacilityInventoryPos_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FacilityInventoryPos item = sender as FacilityInventoryPos;
            if (e.PropertyName == nameof(item.NotAvailable))
            {
                if (item.NotAvailable)
                    item.NewStockQuantity = null;
                if (item.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex == (short)FacilityInventoryPosStateEnum.New)
                {
                    MDFacilityInventoryPosState stateInProgress =
                        item.GetObjectContext<DatabaseApp>()
                        .MDFacilityInventoryPosState
                        .FirstOrDefault(c => c.MDFacilityInventoryPosStateIndex == (short)FacilityInventoryPosStateEnum.Finished);
                    item.MDFacilityInventoryPosState = stateInProgress;
                }
                OnPropertyChanged(nameof(SelectedFacilityInventoryPos));
            }
            else if (e.PropertyName == nameof(item.NewStockQuantity))
            {
                if (item.NewStockQuantity.HasValue)
                    item.NotAvailable = false;
                if (item.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex == (short)FacilityInventoryPosStateEnum.New)
                {
                    MDFacilityInventoryPosState stateInProgress =
                      item.GetObjectContext<DatabaseApp>()
                      .MDFacilityInventoryPosState
                      .FirstOrDefault(c => c.MDFacilityInventoryPosStateIndex == (short)FacilityInventoryPosStateEnum.Finished);
                    item.MDFacilityInventoryPosState = stateInProgress;
                }
                OnPropertyChanged(nameof(SelectedFacilityInventoryPos));
            }
        }

        private bool IsInventoryPosEnabledEdit()
        {
            return
                IsEnabledInventoryEdit
                && SelectedFacilityInventoryPos != null
                && SelectedFacilityInventoryPos.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex < (short)FacilityInventoryPosStateEnum.Finished;
        }

        private void SelectedFacilityInventoryPos_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MDFacilityInventoryPosStateID")
            {
                IsEnabledInventoryPosEdit = IsInventoryPosEnabledEdit();
                OnPropertyChanged(nameof(SelectedFacilityInventoryPos));
            }
        }

        private List<FacilityInventoryPos> _FacilityInventoryPosList;
        /// <summary>
        /// List property for FacilityInventoryPos
        /// </summary>
        /// <value>The FacilityInventoryPos list</value>
        [ACPropertyList(9999, "FacilityInventoryPos")]
        public List<FacilityInventoryPos> FacilityInventoryPosList
        {
            get
            {
                return _FacilityInventoryPosList;
            }
        }

        private List<FacilityInventoryPos> LoadFacilityInventoryPosList()
        {
            if (SelectedFacilityInventory == null) return null;
            SelectedFacilityInventory.FacilityInventoryPos_FacilityInventory.AutoLoad();
            return SelectedFacilityInventory
                .FacilityInventoryPos_FacilityInventory
                .Where(c =>
                    ((InputCode ?? "") == "" || c.FacilityCharge.FacilityChargeID == new Guid(InputCode))
                    && (
                            SelectedFilterInventoryPosStorageLocation == null
                            ||
                            (
                             c.FacilityCharge.Facility.Facility1_ParentFacility != null
                             && c.FacilityCharge.Facility.Facility1_ParentFacility.FacilityNo == SelectedFilterInventoryPosStorageLocation.FacilityNo
                            )
                        )
                    && (SelectedFilterInventoryPosFacility == null || c.FacilityCharge.Facility.FacilityNo == SelectedFilterInventoryPosFacility.FacilityNo)
                    && ((FilterInventoryPosLotNo ?? "") == "" || (c.FacilityCharge.FacilityLot != null && c.FacilityCharge.FacilityLot.LotNo == FilterInventoryPosLotNo))
                    && (SelectedFilterInventoryPosMaterial == null || c.FacilityCharge.Material.MaterialNo == SelectedFilterInventoryPosMaterial.MaterialNo)
                    && (SelectedFilterInventoryPosState == null || c.MDFacilityInventoryPosState.MDKey == SelectedFilterInventoryPosState.MDKey)
                    && (
                            FilterInventoryPosNotAvailable == null
                            // both true or both false - ! exclusive OR ^
                            || !(c.NotAvailable ^ (FilterInventoryPosNotAvailable ?? false))
                       )
                    && (
                            FilterInventoryPosZeroQuantity == null
                            || !((c.StockQuantity == 0) ^ (FilterInventoryPosZeroQuantity ?? false))
                       )
                     && (
                            FilterOpenLines == null
                            || (
                                    ((c.NewStockQuantity != null || c.NotAvailable) && !(FilterOpenLines ?? false))
                                    || ((c.NewStockQuantity == null && !c.NotAvailable) && (FilterOpenLines ?? false))
                               )
                       )
                 )
                .OrderBy(c => c.FacilityCharge != null && c.FacilityCharge.FacilityLot != null ? c.FacilityCharge.FacilityLot.LotNo : "")
                .Take(FilterPosPageSize)
                .ToList();
        }

        public void SetFacilityInventoryPosList()
        {
            _FacilityInventoryPosList = LoadFacilityInventoryPosList();
            OnPropertyChanged(nameof(FacilityInventoryPosList));
            if (_FacilityInventoryPosList != null)
                SelectedFacilityInventoryPos = _FacilityInventoryPosList.FirstOrDefault();
            else
                SelectedFacilityInventoryPos = null;
        }

        #endregion

        #region Property -> FilterNotUsedCharge

        private string _FilterNotUsedChargeMaterial;
        /// <summary>
        /// Doc  FilterNotUsedChargeMaterial
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "FilterNotUsedChargeMaterial", "en{'Material'}de{'Material'}")]
        public string FilterNotUsedChargeMaterial
        {
            get
            {
                return _FilterNotUsedChargeMaterial;
            }
            set
            {
                if (_FilterNotUsedChargeMaterial != value)
                {
                    _FilterNotUsedChargeMaterial = value;
                    OnPropertyChanged(nameof(FilterNotUsedChargeMaterial));
                }
            }
        }

        private string _FilterNotUsedChargeFacility;
        /// <summary>
        /// Doc  FilterNotUsedChargeFacility
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "FilterNotUsedChargeFacility", "en{'Facility'}de{'Lagerplatz'}")]
        public string FilterNotUsedChargeFacility
        {
            get
            {
                return _FilterNotUsedChargeFacility;
            }
            set
            {
                if (_FilterNotUsedChargeFacility != value)
                {
                    _FilterNotUsedChargeFacility = value;
                    OnPropertyChanged(nameof(FilterNotUsedChargeFacility));
                }
            }
        }

        private string _FilterNotUsedChargeLotNo;
        /// <summary>
        /// Doc  FilterNotUsedChargeLotNo
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "FilterNotUsedChargeLotNo", "en{'Lot'}de{'Los'}")]
        public string FilterNotUsedChargeLotNo
        {
            get
            {
                return _FilterNotUsedChargeLotNo;
            }
            set
            {
                if (_FilterNotUsedChargeLotNo != value)
                {
                    _FilterNotUsedChargeLotNo = value;
                    OnPropertyChanged(nameof(FilterNotUsedChargeLotNo));
                }
            }
        }

        private int _FilterNotUsedChargePageSize = 500;
        [ACPropertyInfo(120, "FilterPosPageSize", "en{'Limit Record Count'}de{'Limit Anzahl Datensätze'}")]
        public int FilterNotUsedChargePageSize
        {
            get
            {
                return _FilterNotUsedChargePageSize;
            }
            set
            {
                if (_FilterNotUsedChargePageSize != value)
                {
                    _FilterNotUsedChargePageSize = value;
                    OnPropertyChanged(nameof(FilterNotUsedChargePageSize));
                }
            }
        }

        #endregion

        #region Property -> NotUsedFacilityCharge

        #region NotUsedFacilityCharge
        private FacilityCharge _SelectedNotUsedFacilityCharge;
        /// <summary>
        /// Selected property for FacilityCharge
        /// </summary>
        /// <value>The selected NotUsedFacilityCharge</value>
        [ACPropertySelected(9999, "NotUsedFacilityCharge", "en{'TODO: NotUsedFacilityCharge'}de{'TODO: NotUsedFacilityCharge'}")]
        public FacilityCharge SelectedNotUsedFacilityCharge
        {
            get
            {
                return _SelectedNotUsedFacilityCharge;
            }
            set
            {
                if (_SelectedNotUsedFacilityCharge != value)
                {
                    _SelectedNotUsedFacilityCharge = value;
                    OnPropertyChanged(nameof(SelectedNotUsedFacilityCharge));
                }
            }
        }


        private List<FacilityCharge> _NotUsedFacilityChargeList;
        /// <summary>
        /// List property for FacilityCharge
        /// </summary>
        /// <value>The NotUsedFacilityCharge list</value>
        [ACPropertyList(9999, "NotUsedFacilityCharge")]
        public List<FacilityCharge> NotUsedFacilityChargeList
        {
            get
            {
                return _NotUsedFacilityChargeList;
            }
        }

        #endregion

        #endregion

        #region Property -> FilterInventoryFaciltiyBookingCharge


        private string _FilterInventoryFaciltiyBookingChargeMaterial;
        /// <summary>
        /// Doc  FilterNotUsedChargeMaterial
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "FilterInventoryFaciltiyBookingChargeMaterial", "en{'Material'}de{'Material'}")]
        public string FilterInventoryFaciltiyBookingChargeMaterial
        {
            get
            {
                return _FilterInventoryFaciltiyBookingChargeMaterial;
            }
            set
            {
                if (_FilterInventoryFaciltiyBookingChargeMaterial != value)
                {
                    _FilterInventoryFaciltiyBookingChargeMaterial = value;
                    OnPropertyChanged(nameof(FilterInventoryFaciltiyBookingChargeMaterial));
                }
            }
        }

        private string _FilterInventoryFaciltiyBookingChargeFacility;
        /// <summary>
        /// Doc  FilterNotUsedChargeFacility
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "FilterInventoryFaciltiyBookingChargeFacility", "en{'Facility'}de{'Lagerplatz'}")]
        public string FilterInventoryFaciltiyBookingChargeFacility
        {
            get
            {
                return _FilterInventoryFaciltiyBookingChargeFacility;
            }
            set
            {
                if (_FilterInventoryFaciltiyBookingChargeFacility != value)
                {
                    _FilterInventoryFaciltiyBookingChargeFacility = value;
                    OnPropertyChanged(nameof(FilterInventoryFaciltiyBookingChargeFacility));
                }
            }
        }

        private string _FilterInventoryFaciltiyBookingChargeLotNo;
        /// <summary>
        /// Doc  FilterNotUsedChargeLotNo
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "FilterInventoryFaciltiyBookingChargeLotNo", "en{'Lot'}de{'Los'}")]
        public string FilterInventoryFaciltiyBookingChargeLotNo
        {
            get
            {
                return _FilterInventoryFaciltiyBookingChargeLotNo;
            }
            set
            {
                if (_FilterInventoryFaciltiyBookingChargeLotNo != value)
                {
                    _FilterInventoryFaciltiyBookingChargeLotNo = value;
                    OnPropertyChanged(nameof(FilterInventoryFaciltiyBookingChargeLotNo));
                }
            }
        }

        private int _FilterInventoryFaciltiyBookingChargePageSize = 500;
        [ACPropertyInfo(120, "FilterInventoryFaciltiyBookingChargePageSize", "en{'Limit Record Count'}de{'Limit Anzahl Datensätze'}")]
        public int FilterInventoryFaciltiyBookingChargePageSize
        {
            get
            {
                return _FilterInventoryFaciltiyBookingChargePageSize;
            }
            set
            {
                if (_FilterInventoryFaciltiyBookingChargePageSize != value)
                {
                    _FilterInventoryFaciltiyBookingChargePageSize = value;
                    OnPropertyChanged(nameof(FilterInventoryFaciltiyBookingChargePageSize));
                }
            }
        }

        #endregion

        #region Property ->  InventoryFacilityBookingCharge
        private FacilityBookingCharge _SelectedInventoryFacilityBookingCharge;
        /// <summary>
        /// Selected property for FacilityBookingCharge
        /// </summary>
        /// <value>The selected FacilityBookingCharge</value>
        [ACPropertySelected(400, "InventoryFacilityBookingCharge", "en{'TODO: FacilityBookingCharge'}de{'TODO: FacilityBookingCharge'}")]
        public FacilityBookingCharge SelectedInventoryFacilityBookingCharge
        {
            get
            {
                return _SelectedInventoryFacilityBookingCharge;
            }
            set
            {
                if (_SelectedInventoryFacilityBookingCharge != value)
                {
                    _SelectedInventoryFacilityBookingCharge = value;
                    OnPropertyChanged(nameof(SelectedInventoryFacilityBookingCharge));
                }
            }
        }

        private List<FacilityBookingCharge> _InventoryFacilityBookingChargeList;
        /// <summary>
        /// List property for FacilityBookingCharge
        /// </summary>
        /// <value>The FacilityBookingCharge list</value>
        [ACPropertyList(401, "InventoryFacilityBookingCharge")]
        public List<FacilityBookingCharge> InventoryFacilityBookingChargeList
        {
            get
            {
                return _InventoryFacilityBookingChargeList;
            }
        }

        #endregion

        #region Property ->  InventoryPosFacilityBooking

        private FacilityBooking _SelectedInventoryPosFacilityBooking;
        /// <summary>
        /// Selected property for FacilityBooking
        /// </summary>
        /// <value>The selected FacilityBooking</value>
        [ACPropertySelected(300, "InventoryPosFacilityBooking", "en{'TODO: FacilityBooking'}de{'TODO: FacilityBooking'}")]
        public FacilityBooking SelectedInventoryPosFacilityBooking
        {
            get
            {
                return _SelectedInventoryPosFacilityBooking;
            }
            set
            {
                if (_SelectedInventoryPosFacilityBooking != value)
                {
                    _SelectedInventoryPosFacilityBooking = value;
                    OnPropertyChanged(nameof(SelectedInventoryPosFacilityBooking));

                    _InventoryPosFacilityBookingChargeList = LoadInventoryPosFacilityBookingChargeList();
                    OnPropertyChanged(nameof(InventoryPosFacilityBookingChargeList));
                    if (_InventoryPosFacilityBookingChargeList != null)
                        SelectedInventoryPosFacilityBookingCharge = _InventoryPosFacilityBookingChargeList.FirstOrDefault();
                    else
                        SelectedInventoryPosFacilityBookingCharge = null;
                }
            }
        }

        private List<FacilityBooking> _InventoryPosFacilityBookingList;
        /// <summary>
        /// List property for FacilityBooking
        /// </summary>
        /// <value>The FacilityBooking list</value>
        [ACPropertyList(301, "InventoryPosFacilityBooking")]
        public List<FacilityBooking> InventoryPosFacilityBookingList
        {
            get
            {
                return _InventoryPosFacilityBookingList;
            }
        }

        private List<FacilityBooking> GetInventoryPosFacilityBookingList()
        {
            List<FacilityBooking> bookings = null;
            if (SelectedFacilityInventoryPos != null)
            {
                SelectedFacilityInventoryPos.FacilityBooking_FacilityInventoryPos.AutoLoad();
                bookings = SelectedFacilityInventoryPos.FacilityBooking_FacilityInventoryPos.OrderBy(c => c.FacilityBookingNo).ToList();
            }
            return bookings;
        }

        public void SetInventoryPosFacilityBookingList()
        {
            _InventoryPosFacilityBookingList = GetInventoryPosFacilityBookingList();
            OnPropertyChanged(nameof(InventoryPosFacilityBookingList));
            if (_InventoryPosFacilityBookingList != null)
                SelectedInventoryPosFacilityBooking = _InventoryPosFacilityBookingList.FirstOrDefault();
            else
                SelectedInventoryPosFacilityBooking = null;
        }
        #endregion

        #region Property ->  InventoryPosFacilityBookingCharge
        private FacilityBookingCharge _SelectedInventoryPosFacilityBookingCharge;
        /// <summary>
        /// Selected property for FacilityBookingCharge
        /// </summary>
        /// <value>The selected FacilityBookingCharge</value>
        [ACPropertySelected(400, "InventoryPosFacilityBookingCharge", "en{'TODO: FacilityBookingCharge'}de{'TODO: FacilityBookingCharge'}")]
        public FacilityBookingCharge SelectedInventoryPosFacilityBookingCharge
        {
            get
            {
                return _SelectedInventoryPosFacilityBookingCharge;
            }
            set
            {
                if (_SelectedInventoryPosFacilityBookingCharge != value)
                {
                    _SelectedInventoryPosFacilityBookingCharge = value;
                    OnPropertyChanged(nameof(SelectedInventoryPosFacilityBookingCharge));
                }
            }
        }

        private List<FacilityBookingCharge> _InventoryPosFacilityBookingChargeList;
        /// <summary>
        /// List property for FacilityBookingCharge
        /// </summary>
        /// <value>The FacilityBookingCharge list</value>
        [ACPropertyList(401, "InventoryPosFacilityBookingCharge")]
        public List<FacilityBookingCharge> InventoryPosFacilityBookingChargeList
        {
            get
            {
                if (_InventoryPosFacilityBookingChargeList == null)
                    _InventoryPosFacilityBookingChargeList = LoadInventoryPosFacilityBookingChargeList();
                return _InventoryPosFacilityBookingChargeList;
            }
        }

        private List<FacilityBookingCharge> LoadInventoryPosFacilityBookingChargeList()
        {
            if (SelectedInventoryPosFacilityBooking == null) return null;
            return SelectedInventoryPosFacilityBooking.FacilityBookingCharge_FacilityBooking.OrderBy(c => c.FacilityBookingChargeNo).ToList();
        }

        #endregion

        #endregion

        #region Methods

        #region Methods -> InventoryFacilityBookingCharge

        private List<FacilityBookingCharge> LoadInventoryFacilityBookingChargeList()
        {
            if (SelectedFacilityInventory == null) return null;
            return SelectedFacilityInventory
                .FacilityInventoryPos_FacilityInventory
                .SelectMany(c => c.FacilityBookingCharge_FacilityInventoryPos)
                .Where(c =>
                    // FilterInventoryFaciltiyBookingChargeMaterial
                    (FilterInventoryFaciltiyBookingChargeMaterial == null
                        || (
                            c.InwardMaterial != null
                            && (
                                    c.InwardMaterial.MaterialNo.ToLower().Contains(FilterInventoryFaciltiyBookingChargeMaterial.ToLower())
                                    || c.InwardMaterial.MaterialName1.ToLower().Contains(FilterInventoryFaciltiyBookingChargeMaterial.ToLower())
                                )
                           )
                        || (
                            c.OutwardMaterial != null
                            && (
                                    c.OutwardMaterial.MaterialNo.ToLower().Contains(FilterInventoryFaciltiyBookingChargeMaterial.ToLower())
                                    || c.OutwardMaterial.MaterialName1.ToLower().Contains(FilterInventoryFaciltiyBookingChargeMaterial.ToLower())
                                )
                            )
                       )
                       // FilterInventoryFaciltiyBookingChargeFacility
                       && (FilterInventoryFaciltiyBookingChargeFacility == null
                            || (
                                c.InwardFacility != null
                                && (
                                        c.InwardFacility.FacilityNo.ToLower().Contains(FilterInventoryFaciltiyBookingChargeFacility.ToLower())
                                        || c.InwardFacility.FacilityName.ToLower().Contains(FilterInventoryFaciltiyBookingChargeFacility.ToLower())
                                   )
                               )
                            || (
                                c.OutwardFacility != null
                                && (
                                        c.OutwardFacility.FacilityNo.ToLower().Contains(FilterInventoryFaciltiyBookingChargeFacility.ToLower())
                                        || c.OutwardFacility.FacilityName.ToLower().Contains(FilterInventoryFaciltiyBookingChargeFacility.ToLower())
                                   )
                               )
                           )
                        // FilterInventoryFaciltiyBookingChargeLotNo
                        && (FilterInventoryFaciltiyBookingChargeLotNo == null
                        || (
                                c.InwardFacilityCharge != null && c.InwardFacilityCharge.FacilityLot != null
                                && c.InwardFacilityCharge.FacilityLot.LotNo.Contains(FilterInventoryFaciltiyBookingChargeLotNo)
                            )
                        || (
                                c.OutwardFacilityCharge != null && c.OutwardFacilityCharge.FacilityLot != null
                                && c.OutwardFacilityCharge.FacilityLot.LotNo.Contains(FilterInventoryFaciltiyBookingChargeLotNo)
                            )
                        )
                 )
                .OrderBy(c => c.FacilityBookingChargeNo)
                .Take(FilterInventoryFaciltiyBookingChargePageSize)
                .ToList();
        }

        [ACMethodInfo("SearchFacilityInventoryFacilityBookingCharge", "en{'Search'}de{'Suche'}", 321)]
        public void SearchFacilityInventoryFacilityBookingCharge()
        {
            _InventoryFacilityBookingChargeList = LoadInventoryFacilityBookingChargeList();
            OnPropertyChanged(nameof(InventoryFacilityBookingChargeList));
            if (_InventoryFacilityBookingChargeList != null)
                SelectedInventoryFacilityBookingCharge = _InventoryFacilityBookingChargeList.FirstOrDefault();
            else
                SelectedInventoryFacilityBookingCharge = null;
        }

        private void ClearInventoryFacilityBookingCharge()
        {
            _InventoryFacilityBookingChargeList = null;
            OnPropertyChanged(nameof(InventoryFacilityBookingChargeList));
        }

        private void ClearFilterInventoryFacilityBookingCharge()
        {
            FilterInventoryFaciltiyBookingChargeMaterial = null;
            FilterInventoryFaciltiyBookingChargeFacility = null;
            FilterInventoryFaciltiyBookingChargeLotNo = null;
            FilterInventoryFaciltiyBookingChargePageSize = 500;
        }

        #endregion

        #region Methods -> NotUsedFacilityCharge

        private List<FacilityCharge> LoadNotUsedFacilityChargeList()
        {
            Guid[] usedFacilityChargeIDs =
                SelectedFacilityInventory
                .FacilityInventoryPos_FacilityInventory
                .Select(c => c.FacilityChargeID)
                .ToArray();

            return
                DatabaseApp
                .FacilityCharge
                .AsEnumerable()
                .Where(c =>
                    !usedFacilityChargeIDs.Contains(c.FacilityChargeID)
                    && c.NotAvailable
                    &&
                        (
                            FilterNotUsedChargeMaterial == null
                            ||
                            (
                                c.Material.MaterialNo.ToLower().Contains(FilterNotUsedChargeMaterial.ToLower())
                                || c.Material.MaterialName1.ToLower().Contains(FilterNotUsedChargeMaterial.ToLower())
                            )
                         )
                    &&
                        (
                            FilterNotUsedChargeFacility == null
                            ||
                            (
                                c.Facility.FacilityNo.ToLower().Contains(FilterNotUsedChargeFacility.ToLower())
                                || c.Facility.FacilityName.ToLower().Contains(FilterNotUsedChargeFacility.ToLower())
                            )
                         )
                    &&
                        (
                            FilterNotUsedChargeLotNo == null
                            || c.FacilityLot.LotNo.Contains(FilterNotUsedChargeLotNo)
                         )

                ).OrderBy(c => c.FacilityLot != null ? c.FacilityLot.LotNo : "")
                .Take(FilterNotUsedChargePageSize)
                .ToList();
        }

        [ACMethodInfo("SearchNotUsedFaciltiyCharge", "en{'Search'}de{'Suche'}", 212)]
        public void SearchNotUsedFaciltiyCharge()
        {
            _NotUsedFacilityChargeList = LoadNotUsedFacilityChargeList();
            OnPropertyChanged(nameof(NotUsedFacilityChargeList));
            if (_FacilityInventoryPosList != null)
                SelectedNotUsedFacilityCharge = _NotUsedFacilityChargeList.FirstOrDefault();
            else
                SelectedNotUsedFacilityCharge = null;
        }

        private void ClearNotUserdFaciltiyChargeList()
        {
            _NotUsedFacilityChargeList = null;
            OnPropertyChanged(nameof(NotUsedFacilityChargeList));
        }

        private void ClearFilterNotUsedCharge()
        {
            FilterNotUsedChargeMaterial = null;
            FilterNotUsedChargeFacility = null;
            FilterNotUsedChargeLotNo = null;
            FilterNotUsedChargePageSize = 500;
        }

        #endregion

        #region Methods -> ACvbBSO

        #region Methods -> ACvbBSO -> Search

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(FacilityInventory.ClassName, "en{'Search'}de{'Suche'}", (short)MISort.Search)]
        public void Search()
        {
            AccessNav.NavSearch();
            _FacilityInventoryList = LoadFacilityInventoryList();
            OnPropertyChanged(nameof(FacilityInventoryList));
            if (_FacilityInventoryList != null)
                SelectedFacilityInventory = _FacilityInventoryList.FirstOrDefault();
            else
                SelectedFacilityInventory = null;
        }

        [ACMethodCommand(FacilityInventory.ClassName, "en{'Reset'}de{'Zurücksetzen'}", (short)MISort.Search)]
        public void ClearSearch()
        {
            SelectedFilterInventoryPosState = null;
            LoadInitialFilterInventoryDates();
            Search();
        }

        [ACMethodInteraction(FacilityInventory.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedFacilityInventory", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (requery)
            {
                SelectedFacilityInventory.AutoRefresh();
                OnPropertyChanged(nameof(SelectedFacilityInventory));
                SetFacilityInventoryPosList();
            }
        }

        public bool IsEnabledLoad()
        {
            return SelectedFacilityInventory != null;
        }

        [ACMethodInfo(FacilityInventoryPos.ClassName, "en{'Search'}de{'Suche'}", 205)]
        public void SearchPos()
        {
            if (!IsEnabledSearchPos())
                return;
            SetFacilityInventoryPosList();
        }

        public bool IsEnabledSearchPos()
        {
            return SelectedFacilityInventory != null;
        }

        [ACMethodInfo(FacilityInventoryPos.ClassName, "en{'Clear filter'}de{'Filter löschen'}", 501)]
        public void ClearSearchPos()
        {
            if (!IsEnabledClearSearchPos())
                return;
            _InputCode = "";
            OnPropertyChanged(nameof(InputCode));
            SelectedFilterInventoryPosFacility = null;
            FilterInventoryPosLotNo = null;
            SelectedFilterInventoryPosMaterial = null;
            SelectedFilterInventoryPosState = null;
            FilterInventoryPosNotAvailable = null;
            FilterInventoryPosZeroQuantity = null;
            FilterOpenLines = null;
            SearchPos();
        }

        public bool IsEnabledClearSearchPos()
        {
            return SelectedFacilityInventory != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(FacilityInventory.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedFacilityInventory", Global.ACKinds.MSMethodPrePost)]
        public void SelectPos()
        {
            if (!IsEnabledSelectPos())
                return;

            Guid codeID = Guid.Empty;
            if (Guid.TryParse(InputCode, out codeID))
            {
                SelectedFacilityInventoryPos = FacilityInventoryPosList.FirstOrDefault(c => c.FacilityChargeID == codeID);
            }
        }

        public bool IsEnabledSelectPos()
        {
            return SelectedFacilityInventory != null && !string.IsNullOrEmpty(InputCode);
        }

        #endregion

        #region Methods -> ACvbBSO -> New

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(FacilityInventory.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedFacilityInventory", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            NewFaciltiyInventoryName = "";
            NewFaciltiyInventoryNo = ACFacilityManager.GetNewInventoryNo();
            ShowDialog(this, "NewFacilityInventoryDlg");
        }

        [ACMethodInfo("NewDlgOk", "en{'New'}de{'Neu'}", 100)]
        public void NewDlgOk()
        {
            if (!IsEnabledNewDlgOk())
                return;
            if (SelectedNewInventoryFacility != null || Messages.Question(this, "Question50085") == MsgResult.Yes)
            {
                CloseTopDialog();
                BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoNewInventory);
                ShowDialog(this, DesignNameProgressBar);
            }
        }
        public bool IsEnabledNewDlgOk()
        {
            return !string.IsNullOrEmpty(NewFaciltiyInventoryNo) && !string.IsNullOrEmpty(NewFaciltiyInventoryName);
        }

        [ACMethodInfo("NewDlgOkCancel", "en{'Close'}de{'Schließen'}", 100)]
        public void NewDlgOkCancel()
        {
            CloseTopDialog();
        }

        [ACMethodInfo("ShowFaciltiyDialog", "en{'Choose facility'}de{'Lager auswählen'}", 999)]
        public void ShowFaciltiyDialog()
        {
            if (!IsEnabledShowFaciltiyDialog())
                return;

            VBDialogResult dlgResult = BSOFacilityExplorer_Child.Value.ShowDialog(SelectedNewInventoryFacility);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;
                SelectedNewInventoryFacility = facility;
            }
        }

        public bool IsEnabledShowFaciltiyDialog()
        {
            return true;
        }

        [ACMethodInfo("ChangeInventoryFacility", "en{'Change facility'}de{'Lager wechseln'}", 999)]
        public void ChangeInventoryFacility()
        {
            if (!IsEnabledChangeInventoryFacility())
                return;

            VBDialogResult dlgResult = BSOFacilityExplorer_Child.Value.ShowDialog(SelectedFacilityInventory.Facility);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;
                SelectedFacilityInventory.Facility = facility;
                OnPropertyChanged(nameof(SelectedFacilityInventory));
            }
        }

        public bool IsEnabledChangeInventoryFacility()
        {
            return SelectedFacilityInventory != null && !SelectedFacilityInventory.FacilityInventoryPos_FacilityInventory.Any();
        }

        /// <summary>
        /// Source Property: GeneratePositions
        /// </summary>
        [ACMethodInfo("GeneratePositions", "en{'Generate positions'}de{'Positionen generieren'}", 999)]
        public void GeneratePositions()
        {
            if (!IsEnabledGeneratePositions())
                return;

            CloseTopDialog();
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoGeneratePositions);
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledGeneratePositions()
        {
            return SelectedFacilityInventory != null && !SelectedFacilityInventory.FacilityInventoryPos_FacilityInventory.Any();
        }

        #endregion

        #region Methods -> ACvbBSO -> Delete

        [ACMethodInteraction("Delete", "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "SelectedFacilityInventory", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!IsEnabledDelete())
                return;
            var questionResult = Root.Messages.Question(this, "Question50062", MsgResult.Yes, false, SelectedFacilityInventory.FacilityInventoryNo);
            if (questionResult == MsgResult.Yes)
            {
                List<FacilityInventoryPos> positions = SelectedFacilityInventory.FacilityInventoryPos_FacilityInventory.ToList();
                foreach (var item in positions)
                    item.DeleteACObject(DatabaseApp, false);
                FacilityInventoryList.Remove(SelectedFacilityInventory);
                SelectedFacilityInventory.DeleteACObject(Database, false);
                SelectedFacilityInventory = FacilityInventoryList.FirstOrDefault();
                _FacilityInventoryList = null;
                OnPropertyChanged(nameof(FacilityInventoryList));
            }
        }

        public bool IsEnabledDelete()
        {
            return SelectedFacilityInventory != null
                && SelectedFacilityInventory.MDFacilityInventoryState != null
                && SelectedFacilityInventory.MDFacilityInventoryState.FacilityInventoryState <= FacilityInventoryStateEnum.InProgress;
        }

        [ACMethodInteraction("DeletePos", "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "SelectedFacilityInventory", Global.ACKinds.MSMethodPrePost)]
        public void DeletePos()
        {
            if (!IsEnabledDeletePos())
                return;
        }

        public bool IsEnabledDeletePos()
        {
            return SelectedFacilityInventoryPos != null;
        }

        #endregion

        #region Methods -> Inventory Lifecycle

        #region Methods -> Inventory Lifecycle -> Inventory
        // StartInventory

        [ACMethodInfo("StartInventory", "en{'1.) Start inventory'}de{'1.) Inventur beginnen'}", 101)]
        public void StartInventory()
        {
            if (!IsEnabledStartInventory())
                return;
            MDFacilityInventoryState inProgressState = DatabaseApp.MDFacilityInventoryState.FirstOrDefault(c => c.MDFacilityInventoryStateIndex == (short)FacilityInventoryStateEnum.InProgress);
            SelectedFacilityInventory.MDFacilityInventoryState = inProgressState;
            DatabaseApp.ACSaveChanges();
            OnPropertyChanged(nameof(SelectedFacilityInventory));
            OnPropertyChanged(nameof(SelectedFacilityInventoryPos));
            IsEnabledInventoryEdit = true;
            IsEnabledInventoryPosEdit = IsInventoryPosEnabledEdit();
            GeneratePositions();
        }

        public bool IsEnabledStartInventory()
        {
            return
                SelectedFacilityInventory != null
                && SelectedFacilityInventory.MDFacilityInventoryState != null
                && SelectedFacilityInventory.MDFacilityInventoryState.MDFacilityInventoryStateIndex == (short)FacilityInventoryStateEnum.New;
        }


        [ACMethodInfo("ClosingInventory", "en{'3.) Post and complete inventory'}de{'3.) Buchen und Inventur beenden'}", 100)]
        public void ClosingInventory()
        {
            if (!IsEnabledClosingInventory())
                return;
            bool existAnyUnclosedPosition =
                SelectedFacilityInventory.FacilityInventoryPos_FacilityInventory.Any(c => c.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex != (short)FacilityInventoryPosStateEnum.Finished);
            if (existAnyUnclosedPosition)
            {
                SelectedFilterInventoryPosState = FilterInventoryPosStateList.FirstOrDefault(c => c.MDFacilityInventoryPosStateIndex == (short)FacilityInventoryPosStateEnum.InProgress);
                SearchPos();
                Root.Messages.Warning(this, "Warning50038");
            }
            else
            {
                var questionResult = Root.Messages.Question(this, "Question50054");
                if (questionResult == MsgResult.Yes)
                {
                    BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoInventoryClosing);
                    ShowDialog(this, DesignNameProgressBar);
                }
            }
        }

        public bool IsEnabledClosingInventory()
        {
            return
                SelectedFacilityInventory != null
                && SelectedFacilityInventory.MDFacilityInventoryState != null
                && SelectedFacilityInventory.MDFacilityInventoryState.MDFacilityInventoryStateIndex == (short)FacilityInventoryStateEnum.InProgress;
        }

        [ACMethodInfo("CloseAllPositions", "en{'2.) Close filtered inventory lines'}de{'2.) Gefilterte Inventurpositionen abschließen'}", 100)]
        public void CloseAllPositions()
        {
            // Question50055.
            var questionResult = Root.Messages.Question(this, "Question50055");
            if (questionResult == MsgResult.Yes)
            {
                MDFacilityInventoryPosState finishedState = DatabaseApp.MDFacilityInventoryPosState.FirstOrDefault(c => c.MDFacilityInventoryPosStateIndex == (short)FacilityInventoryPosStateEnum.Finished);
                foreach (FacilityInventoryPos item in FacilityInventoryPosList)
                {
                    item.MDFacilityInventoryPosState = finishedState;
                }
                ACSaveChanges();
                OnPropertyChanged(nameof(FacilityInventoryPosList));
            }
        }

        public bool IsEnabledCloseAllPositions()
        {
            return SelectedFacilityInventory != null
                && SelectedFacilityInventory.MDFacilityInventoryState != null
                && SelectedFacilityInventory.MDFacilityInventoryState.MDFacilityInventoryStateIndex == (short)FacilityInventoryStateEnum.InProgress;
        }

        [ACMethodInfo("SendToERP", "en{'Send to ERP'}de{'An ERP senden'}", 102)]
        public virtual void SendToERP()
        {
            // do nothing

            MDFacilityInventoryState postedInventoryState = DatabaseApp.MDFacilityInventoryState.FirstOrDefault(c => c.MDFacilityInventoryStateIndex == (short)FacilityInventoryStateEnum.Posted);
            SelectedFacilityInventory.MDFacilityInventoryState = postedInventoryState;
            ACSaveChanges();
        }

        public virtual bool IsEnabledSendToERP()
        {
            return
                SelectedFacilityInventory != null
                && SelectedFacilityInventory.MDFacilityInventoryState != null
                && SelectedFacilityInventory.MDFacilityInventoryState.FacilityInventoryState == FacilityInventoryStateEnum.Finished;
        }

        #endregion

        #region Methods -> Inventory Lifecycle -> InventoryPos
        // StartInventory

        [ACMethodInfo("StartInventoryPos", "en{'Start'}de{'Starten'}", 111)]
        public void StartInventoryPos()
        {
            if (!IsEnabledStartInventoryPos())
                return;
            SetInventoryPosState(SelectedFacilityInventoryPos, FacilityInventoryPosStateEnum.InProgress);
            OnPropertyChanged(nameof(SelectedFacilityInventoryPos));
        }

        public bool IsEnabledStartInventoryPos()
        {
            return
                SelectedFacilityInventory != null
                && SelectedFacilityInventory.MDFacilityInventoryState.MDFacilityInventoryStateIndex == (short)FacilityInventoryStateEnum.InProgress
                && SelectedFacilityInventoryPos != null
                && SelectedFacilityInventoryPos.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex == (short)FacilityInventoryPosStateEnum.New;
        }


        [ACMethodInfo("ClosingInventoryPos", "en{'Close filtered inventory lines'}de{'Gefilterte Inventurpositionen abschließen'}", 110)]
        public void ClosingInventoryPos()
        {
            if (!IsEnabledClosingInventoryPos())
                return;
            SetInventoryPosState(SelectedFacilityInventoryPos, FacilityInventoryPosStateEnum.Finished);
            OnPropertyChanged(nameof(SelectedFacilityInventoryPos));
        }

        public bool IsEnabledClosingInventoryPos()
        {
            return
                SelectedFacilityInventoryPos != null &&
                SelectedFacilityInventoryPos.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex == (short)FacilityInventoryStateEnum.InProgress;
        }

        private void SetInventoryPosState(FacilityInventoryPos pos, FacilityInventoryPosStateEnum posState)
        {
            MDFacilityInventoryPosState inventoryState = DatabaseApp.MDFacilityInventoryPosState.FirstOrDefault(c => c.MDFacilityInventoryPosStateIndex == (short)posState);
            pos.MDFacilityInventoryPosState = inventoryState;
            DatabaseApp.ACSaveChanges();
        }

        [ACMethodInfo("CopyQuantityFromStock", "en{'Takeover stock'}de{'Übernehme Lagerbestand'}", 112)]
        public void CopyQuantityFromStock()
        {
            SelectedFacilityInventoryPos.NewStockQuantity =
                SelectedFacilityInventoryPos.StockQuantity;
            OnPropertyChanged("SelectedFacilityInventoryPos\\NewStockQuantity");
        }

        public bool IsEnabledCopyQuantityFromStock()
        {
            return SelectedFacilityInventoryPos != null
                && SelectedFacilityInventory.MDFacilityInventoryState != null
                && SelectedFacilityInventoryPos.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex < (short)FacilityInventoryPosStateEnum.Finished
                && !SelectedFacilityInventoryPos.NotAvailable;
        }

        #endregion


        #endregion

        #region Methods -> ACvBSO -> Save
        [ACMethodCommand("Save", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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

        #endregion

        #region Methods -> Add FacilityCharge

        [ACMethodInfo("AddFacilityCharge", "en{'Restore quant'}de{'Quanten wiederherstellen'}", 111)]
        public void AddFacilityCharge()
        {
            if (!IsEnabledAddFacilityCharge())
                return;
            if (SelectedNotUsedFacilityCharge.NotAvailable)
                SelectedNotUsedFacilityCharge.NotAvailable = false;

            if (SelectedFacilityInventory.IsFaciltiyMatch(SelectedNotUsedFacilityCharge.Facility))
            {
                FacilityInventoryPos facilityInventoryPos = FacilityInventoryPos.NewACObject(DatabaseApp, SelectedFacilityInventory);
                facilityInventoryPos.FacilityCharge = SelectedNotUsedFacilityCharge;
                DatabaseApp.FacilityInventoryPos.AddObject(facilityInventoryPos);
                FacilityInventoryPosList.Add(facilityInventoryPos);
                SelectedFacilityInventoryPos = facilityInventoryPos;
                OnPropertyChanged(nameof(FacilityInventoryPosList));

                NotUsedFacilityChargeList.Remove(SelectedNotUsedFacilityCharge);
                SelectedNotUsedFacilityCharge = NotUsedFacilityChargeList.FirstOrDefault();
                OnPropertyChanged(nameof(NotUsedFacilityChargeList));
            }
            else
            {
                Messages.Error(this, "Error50560", false, SelectedFacilityInventory.Facility?.FacilityNo, SelectedFacilityInventory.Facility?.FacilityName,
                    SelectedNotUsedFacilityCharge.Facility?.FacilityNo, SelectedNotUsedFacilityCharge.Facility?.FacilityName);
            }
        }

        public bool IsEnabledAddFacilityCharge()
        {
            return
                SelectedFacilityInventory != null
                && SelectedFacilityInventory.MDFacilityInventoryState.MDFacilityInventoryStateIndex < (short)FacilityInventoryStateEnum.Finished
                && SelectedNotUsedFacilityCharge != null;
        }

        #endregion

        #endregion

        #region Mehtods -> Override

        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            if (vbControl == null)
                return base.OnGetControlModes(vbControl);

            Global.ControlModes result = base.OnGetControlModes(vbControl);
            if (result < Global.ControlModes.Enabled)
                return result;
            bool isNotAvailable = SelectedFacilityInventoryPos != null ? SelectedFacilityInventoryPos.NotAvailable : false;
            switch (vbControl.VBContent)
            {
                case "!CopyQuantityFromStock":
                    return !isNotAvailable ? Global.ControlModes.Enabled : Global.ControlModes.Disabled;
                case "SelectedFacilityInventoryPos\\NewStockQuantity":
                    return !isNotAvailable ? Global.ControlModes.Enabled : Global.ControlModes.Disabled;
                case "SelectedFacilityInventoryPos\\MDFacilityInventoryPosState":
                    return
                        SelectedFacilityInventoryPos != null
                        && SelectedFacilityInventoryPos.MDFacilityInventoryPosState != null
                        && SelectedFacilityInventoryPos.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex <= (short)FacilityInventoryPosStateEnum.Finished

                        ? Global.ControlModes.Enabled : Global.ControlModes.Disabled;
                case "SelectedFacilityInventory\\Facility":
                    return IsEnabledChangeInventoryFacility() ? Global.ControlModes.Enabled : Global.ControlModes.Disabled;
            }

            return result;
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "Search":
                    Search();
                    return true;
                case "ClearSearch":
                    ClearSearch();
                    return true;
                case "SearchPos":
                    SearchPos();
                    return true;
                case "IsEnabledSearchPos":
                    result = IsEnabledSearchPos();
                    return true;
                case "ClearSearchPos":
                    ClearSearchPos();
                    return true;
                case "IsEnabledClearSearchPos":
                    result = IsEnabledClearSearchPos();
                    return true;
                case "Load":
                    Load(acParameter.Count() == 1 ? (System.Boolean)acParameter[0] : false);
                    return true;
                case "SelectPos":
                    SelectPos();
                    return true;
                case "IsEnabledSelectPos":
                    result = IsEnabledSelectPos();
                    return true;
                case "New":
                    New();
                    return true;
                case "NewDlgOk":
                    NewDlgOk();
                    return true;
                case "IsEnabledNewDlgOk":
                    result = IsEnabledNewDlgOk();
                    return true;
                case "NewDlgOkCancel":
                    NewDlgOkCancel();
                    return true;
                case "Delete":
                    Delete();
                    return true;
                case "IsEnabledDelete":
                    result = IsEnabledDelete();
                    return true;
                case "DeletePos":
                    DeletePos();
                    return true;
                case "IsEnabledDeletePos":
                    result = IsEnabledDeletePos();
                    return true;
                case "StartInventory":
                    StartInventory();
                    return true;
                case "IsEnabledStartInventory":
                    result = IsEnabledStartInventory();
                    return true;
                case "ClosingInventory":
                    ClosingInventory();
                    return true;
                case "IsEnabledClosingInventory":
                    result = IsEnabledClosingInventory();
                    return true;
                case "CloseAllPositions":
                    CloseAllPositions();
                    return true;
                case "IsEnabledCloseAllPositions":
                    result = IsEnabledCloseAllPositions();
                    return true;
                case "StartInventoryPos":
                    StartInventoryPos();
                    return true;
                case "IsEnabledStartInventoryPos":
                    result = IsEnabledStartInventoryPos();
                    return true;
                case "ClosingInventoryPos":
                    ClosingInventoryPos();
                    return true;
                case "IsEnabledClosingInventoryPos":
                    result = IsEnabledClosingInventoryPos();
                    return true;
                case "Save":
                    Save();
                    return true;
                case "IsEnabledSave":
                    result = IsEnabledSave();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override object Clone()
        {
            object clonedObject = base.Clone();
            BSOFacilityInventory cloned = clonedObject as BSOFacilityInventory;
            cloned.SelectedFacilityInventoryPos = SelectedFacilityInventoryPos;
            return cloned;
        }

        #endregion

        #region Tracking

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vbContent"></param>
        /// <param name="vbControl"></param>
        /// <returns></returns>
        public override ACMenuItemList GetMenu(string vbContent, string vbControl)
        {
            ACMenuItemList aCMenuItems = base.GetMenu(vbContent, vbControl);
            if (vbContent == "SelectedInventoryPosFacilityBooking" && SelectedInventoryPosFacilityBooking != null)
            {
                TrackingCommonStart trackingCommonStart = new TrackingCommonStart();
                ACMenuItemList trackingAndTracingMenuItems = trackingCommonStart.GetTrackingAndTrackingMenuItems(this, SelectedInventoryPosFacilityBooking);
                aCMenuItems.AddRange(trackingAndTracingMenuItems);
            }
            else if (vbContent == "SelectedInventoryFacilityBookingCharge" && SelectedInventoryFacilityBookingCharge != null)
            {
                TrackingCommonStart trackingCommonStart = new TrackingCommonStart();
                ACMenuItemList trackingAndTracingMenuItems = trackingCommonStart.GetTrackingAndTrackingMenuItems(this, SelectedInventoryFacilityBookingCharge.FacilityBooking);
                aCMenuItems.AddRange(trackingAndTracingMenuItems);
            }
            return aCMenuItems;
        }


        [ACMethodInfo("OnTrackingCall", "en{'OnTrackingCall'}de{'OnTrackingCall'}", 600, false)]
        public void OnTrackingCall(GlobalApp.TrackingAndTracingSearchModel direction, IACObject itemForTrack, object additionalFilter, TrackingEnginesEnum engine)
        {
            TrackingCommonStart trackingCommonStart = new TrackingCommonStart();
            trackingCommonStart.DoTracking(this, direction, itemForTrack, additionalFilter, engine);
        }

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
            worker.ProgressInfo.ProgressInfoIsIndeterminate = false;
            worker.ProgressInfo.OnlyTotalProgress = true;
            switch (command)
            {
                case BGWorkerMehtod_DoNewInventory:
                    e.Result = ACFacilityManager.InventoryGenerate(NewFaciltiyInventoryNo, NewFaciltiyInventoryName, SelectedNewInventoryFacility?.FacilityID, GenerateInventoryPosition, DoNewInventoryProgressCallback);
                    break;
                case BGWorkerMehtod_DoGeneratePositions:
                    DoGeneratePositions(SelectedFacilityInventory);
                    break;
                case BGWorkerMehtod_DoInventoryClosing:
                    e.Result = ACFacilityManager.InventoryClosing(SelectedFacilityInventory.FacilityInventoryNo, DoInventoryClosingProgressCallback);
                    break;
            }
        }

        private void DoNewInventoryProgressCallback(int current, int count)
        {
            if (current == 0)
                BackgroundWorker.ProgressInfo.AddSubTask("DoNewInventory", 0, count);
            BackgroundWorker.ProgressInfo.ReportProgress("DoNewInventory", current);
            BackgroundWorker.ProgressInfo.TotalProgress.ProgressText = string.Format(@"DoNewInventory: [{0}] | Progress: {1} / {2} steps...", NewFaciltiyInventoryNo, current, count);
        }

        public MsgWithDetails DoGeneratePositions(FacilityInventory facilityInventory)
        {
            MsgWithDetails msg = null;
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                FacilityInventory tmpInventory = databaseApp.FacilityInventory.FirstOrDefault(c => c.FacilityInventoryID == facilityInventory.FacilityInventoryID);
                ACFacilityManager.InventoryGeneratePositions(databaseApp, tmpInventory, null);
                msg = databaseApp.ACSaveChanges();
            }
            return msg;
        }

        private void DoInventoryClosingProgressCallback(int current, int count)
        {
            if (current == 0)
                BackgroundWorker.ProgressInfo.AddSubTask("DoInventoryClosing", 0, count);
            BackgroundWorker.ProgressInfo.ReportProgress("DoInventoryClosing", current);
            BackgroundWorker.ProgressInfo.TotalProgress.ProgressText = string.Format(@"DoInventoryClosing: [{0}] | Progress: {1} / {2} steps...", SelectedFacilityInventory.FacilityInventoryNo, current, count);
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
                switch (command)
                {
                    case BGWorkerMehtod_DoNewInventory:
                        MsgWithDetails creatingMessage = e.Result as MsgWithDetails;
                        if (creatingMessage == null || creatingMessage.IsSucceded())
                        {
                            Search();
                            SelectedFacilityInventory = FacilityInventoryList.FirstOrDefault(c => c.FacilityInventoryNo == NewFaciltiyInventoryNo);
                        }
                        else
                            SendMessage(creatingMessage);
                        break;
                    case BGWorkerMehtod_DoGeneratePositions:
                        MsgWithDetails msgAddPositions = e.Result as MsgWithDetails;
                        if (msgAddPositions == null || msgAddPositions.IsSucceded())
                            RefreshInventory(true);
                        else
                            SendMessage(msgAddPositions);
                        break;
                    case BGWorkerMehtod_DoInventoryClosing:
                        MsgWithDetails closingMessage = e.Result as MsgWithDetails;
                        if (closingMessage.IsSucceded())
                            RefreshInventory(false);
                        else
                        {
                            if (closingMessage.MsgDetails.Any(c => c.MessageLevel == eMsgLevel.Error && c.ACIdentifier == FacilityManager.Const_Inventory_NotAllowedClosing))
                            {
                                Msg existUnclosedPositions = new Msg(this, eMsgLevel.Warning, FacilityInventory.ClassName, "DoInventoryClosing", 1015, "Warning50038");
                                SendMessage(existUnclosedPositions);
                            }
                            else
                                SendMessage(closingMessage);
                        }
                        break;
                }
            }
        }

        private void RefreshInventory(bool refreshPos)
        {
            SelectedFacilityInventory.AutoRefresh();
            OnPropertyChanged(nameof(SelectedFacilityInventory));
            OnPropertyChanged(nameof(FacilityInventoryList));

            if (refreshPos)
            {
                SelectedFacilityInventory.FacilityInventoryPos_FacilityInventory.AutoLoad();
                SetFacilityInventoryPosList();
            }

            SetInventoryPosFacilityBookingList();
        }

        #endregion

        #endregion

        #region Messages

        #region Messages -> IMsgObserver

        public void SendMessage(Msg msg)
        {
            MsgList.Add(msg);
            OnPropertyChanged(nameof(MsgList));
        }

        #endregion

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
                OnPropertyChanged(nameof(CurrentMsg));
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
        }

        #endregion

        #endregion

    }
}
