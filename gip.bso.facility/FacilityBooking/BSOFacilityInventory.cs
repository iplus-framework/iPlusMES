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
using System.Text;
using System.Threading.Tasks;
using static gip.core.datamodel.Global;

namespace gip.bso.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Warehouse Inventory'}de{'Lager Inventur'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + FacilityInventory.ClassName)]
    [ACQueryInfo(Const.PackName_VarioFacility, Const.QueryPrefix + "FacilityInventory", "en{'FacilityInventory'}de{'FacilityInventory'}", typeof(FacilityInventory), FacilityInventory.ClassName, "FacilityInventoryNo", "FacilityInventoryNo")]

    public class BSOFacilityInventory : ACBSOvbNav
    {
        #region contants
        public const string BGWorkerMehtod_DoNewInventory = @"DoNewInventory";
        public const string BGWorkerMehtod_DoInventoryClosing = @"DoInventoryClosing";

        #endregion

        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityVehicle"/> class.
        /// </summary>
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

            AccessFilterInventoryPosFacility.NavSearch();
            AccessFilterInventoryPosMaterial.NavSearch();
            SelectedFilterInventoryPosFacility = null;
            SelectedFilterInventoryPosMaterial = null;

            LoadInitialFilterInventoryDates();
            IsEnabledInventoryPosEdit = false;
            Search();
            return true;
        }

        private void LoadInitialFilterInventoryDates()
        {
            FilterInventoryEndDate = DateTime.Now.Date;
            FilterInventoryStartDate = FilterInventoryEndDate.AddYears(-1);
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

        #region Properties

        #region Properties -> Filter

        #region Properties -> Filter-> Inventory

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
                    OnPropertyChanged("FilterInventoryStartDate");
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
                    OnPropertyChanged("FilterInventoryEndDate");
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
                    OnPropertyChanged("SelectedFilterInventoryState");
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


        #endregion

        #region Properties -> Filter -> InventoryPos

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
                    _AccessFilterInventoryPosFacility = navACQueryDefinition.NewAccessNav<Facility>("Facility", this);
                    _AccessFilterInventoryPosFacility.AutoSaveOnNavigation = false;
                }
                return _AccessFilterInventoryPosFacility;
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
                AccessFilterInventoryPosFacility.Selected = value;
                OnPropertyChanged("SelectedFilterInventoryPosFacility");
                SearchPos();
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


        #region FilterInventoryPosMaterial

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
                OnPropertyChanged("SelectedFilterInventoryPosMaterial");
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
                    OnPropertyChanged("FilterInventoryPosLotNo");
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
                    OnPropertyChanged("_FilterInventoryPosNotAvailable");
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
                    OnPropertyChanged("FilterInventoryPosZeroQuantity");
                    SearchPos();
                }
            }
        }

        #region Properties -> Filter-> FilterInventoryPosState (MDFacilityInventoryPosState)

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
                    OnPropertyChanged("SelectedFilterInventoryPosState");
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


        private bool _FilterInventoryPosFBConnected = true;
        [ACPropertyInfo(105, "FilterInventoryPosFBConnected", "en{'Connect Position with Booking'}de{'Verbinde Pos mit Buchung'}")]
        public bool FilterInventoryPosFBConnected
        {
            get
            {
                return _FilterInventoryPosFBConnected;
            }
            set
            {
                if (_FilterInventoryPosFBConnected != value)
                {
                    _FilterInventoryPosFBConnected = value;
                    OnPropertyChanged("FilterInventoryPosFBConnected");
                    SetFacilityBookingList();
                }
            }
        }

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
                    OnPropertyChanged("IsEnabledInventoryEdit");
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
                OnPropertyChanged("IsEnabledInventoryPosEdit");
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
                OnPropertyChanged("InventoryDisabledModes");
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
                    OnPropertyChanged("InventoryPosDisabledModes");
                }
            }
        }
        #endregion

        #endregion

        #region Properties -> InputCode


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
                    OnPropertyChanged("InputCode");
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
                    OnPropertyChanged("NewFaciltiyInventoryNo");
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
                    OnPropertyChanged("NewFaciltiyInventoryName");
                }
            }
        }

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
                        //if (navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(NavigationqueryDefaultFilter))
                        //{
                        //    //SetMaterialIDFilter();
                        //}
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

                ACSortItem facilityInventoryNo = new ACSortItem("FacilityInventoryNo", SortDirections.ascending, true);
                acSortItems.Add(facilityInventoryNo);

                return acSortItems;
            }
        }

        public List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {

                // TODO: @aagincic write FilterInventory part
                List<ACFilterItem> aCFilterItems = new List<ACFilterItem>();

                // MDFacilityInventoryStateID
                ACFilterItem stateFilter = new ACFilterItem(Global.FilterTypes.filter, "MDFacilityInventoryStateID", Global.LogicalOperators.isNull, Global.Operators.and, null, true);
                aCFilterItems.Add(stateFilter);

                // FilterInventoryStartDate
                ACFilterItem fromDate = new ACFilterItem(Global.FilterTypes.filter, "InsertDate", Global.LogicalOperators.isNull, Global.Operators.and, null, true);
                aCFilterItems.Add(fromDate);

                // FilterInventoryEndDate

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
                        IsEnabledInventoryEdit = _SelectedFacilityInventory.MDFacilityInventoryState.MDFacilityInventoryStateIndex == (short)MDFacilityInventoryState.FacilityInventoryStates.InProgress;
                    else
                        IsEnabledInventoryEdit = false;

                    OnPropertyChanged("SelectedFacilityInventory");

                    SetFacilityInventoryPosList();
                }
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
                    IsEnabledInventoryPosEdit = IsInventoryPosEnabledEdit();
                    OnPropertyChanged("SelectedFacilityInventoryPos");

                    if (SelectedFacilityInventoryPos != null)
                    {
                        SelectedFacilityInventoryPos.PropertyChanged -= SelectedFacilityInventoryPos_PropertyChanged;
                        SelectedFacilityInventoryPos.PropertyChanged += SelectedFacilityInventoryPos_PropertyChanged;
                    }
                    if (FilterInventoryPosFBConnected)
                        SetFacilityBookingList();
                }
            }
        }

        private bool IsInventoryPosEnabledEdit()
        {
            return
                IsEnabledInventoryEdit
                && SelectedFacilityInventoryPos != null
                && SelectedFacilityInventoryPos.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex == (short)MDFacilityInventoryPosState.FacilityInventoryPosStates.InProgress;
        }

        private void SelectedFacilityInventoryPos_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MDFacilityInventoryPosStateID")
            {
                IsEnabledInventoryPosEdit = IsInventoryPosEnabledEdit();
                OnPropertyChanged("SelectedFacilityInventoryPos");
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
                    && (SelectedFilterInventoryPosFacility == null || c.FacilityCharge.Facility.FacilityNo == SelectedFilterInventoryPosFacility.FacilityNo)
                    && ((FilterInventoryPosLotNo ?? "") == "" || (c.FacilityCharge.FacilityLot != null && c.FacilityCharge.FacilityLot.LotNo == FilterInventoryPosLotNo))
                    && (SelectedFilterInventoryPosMaterial == null || c.FacilityCharge.Material.MaterialNo == SelectedFilterInventoryPosMaterial.MaterialNo)
                    && (SelectedFilterInventoryPosState == null || c.MDFacilityInventoryPosState.MDKey == SelectedFilterInventoryPosState.MDKey)
                    && (FilterInventoryPosNotAvailable == null || c.NotAvailable == (FilterInventoryPosNotAvailable ?? false))
                    && (FilterInventoryPosZeroQuantity == null ||
                        // both true or both false - ! exclusive OR ^
                        !((c.StockQuantity == 0) ^ (FilterInventoryPosZeroQuantity ?? false)))
                 )
                .OrderBy(c => c.Sequence)
                .ToList();
        }

        public void SetFacilityInventoryPosList()
        {
            _FacilityInventoryPosList = LoadFacilityInventoryPosList();
            OnPropertyChanged("FacilityInventoryPosList");
            if (_FacilityInventoryPosList != null)
                SelectedFacilityInventoryPos = _FacilityInventoryPosList.FirstOrDefault();
            else
                SelectedFacilityInventoryPos = null;
        }

        #endregion

        #region Property ->  FacilityBooking
        private FacilityBooking _SelectedFacilityBooking;
        /// <summary>
        /// Selected property for FacilityBooking
        /// </summary>
        /// <value>The selected FacilityBooking</value>
        [ACPropertySelected(300, "FacilityBooking", "en{'TODO: FacilityBooking'}de{'TODO: FacilityBooking'}")]
        public FacilityBooking SelectedFacilityBooking
        {
            get
            {
                return _SelectedFacilityBooking;
            }
            set
            {
                if (_SelectedFacilityBooking != value)
                {
                    _SelectedFacilityBooking = value;
                    OnPropertyChanged("SelectedFacilityBooking");

                    _FacilityBookingChargeList = LoadFacilityBookingChargeList();
                    OnPropertyChanged("FacilityBookingChargeList");
                    if (_FacilityBookingChargeList != null)
                        SelectedFacilityBookingCharge = _FacilityBookingChargeList.FirstOrDefault();
                    else
                        SelectedFacilityBookingCharge = null;
                }
            }
        }

        private List<FacilityBooking> _FacilityBookingList;
        /// <summary>
        /// List property for FacilityBooking
        /// </summary>
        /// <value>The FacilityBooking list</value>
        [ACPropertyList(301, "FacilityBooking")]
        public List<FacilityBooking> FacilityBookingList
        {
            get
            {
                return _FacilityBookingList;
            }
        }

        private List<FacilityBooking> GetFacilityBookingList()
        {
            List<FacilityBooking> bookings = null;

            if (FilterInventoryPosFBConnected)
            {
                if (SelectedFacilityInventoryPos != null)
                {
                    SelectedFacilityInventoryPos.FacilityBooking_FacilityInventoryPos.AutoLoad();
                    bookings = SelectedFacilityInventoryPos.FacilityBooking_FacilityInventoryPos.OrderBy(c => c.FacilityBookingNo).ToList();
                }
            }
            else
            {
                if (FacilityInventoryPosList != null && FacilityInventoryPosList.Any())
                {
                    foreach (var item in FacilityInventoryPosList)
                        item.FacilityBooking_FacilityInventoryPos.AutoLoad();
                    bookings = FacilityInventoryPosList.SelectMany(c => c.FacilityBooking_FacilityInventoryPos).OrderBy(c => c.FacilityBookingNo).ToList();
                }
            }
            return bookings;
        }

        public void SetFacilityBookingList()
        {
            _FacilityBookingList = GetFacilityBookingList();
            OnPropertyChanged("FacilityBookingList");
            if (_FacilityBookingList != null)
                SelectedFacilityBooking = _FacilityBookingList.FirstOrDefault();
            else
                SelectedFacilityBooking = null;
        }
        #endregion

        #region Property ->  FacilityBookingCharge
        private FacilityBookingCharge _SelectedFacilityBookingCharge;
        /// <summary>
        /// Selected property for FacilityBookingCharge
        /// </summary>
        /// <value>The selected FacilityBookingCharge</value>
        [ACPropertySelected(400, "FacilityBookingCharge", "en{'TODO: FacilityBookingCharge'}de{'TODO: FacilityBookingCharge'}")]
        public FacilityBookingCharge SelectedFacilityBookingCharge
        {
            get
            {
                return _SelectedFacilityBookingCharge;
            }
            set
            {
                if (_SelectedFacilityBookingCharge != value)
                {
                    _SelectedFacilityBookingCharge = value;
                    OnPropertyChanged("SelectedFacilityBookingCharge");
                }
            }
        }


        private List<FacilityBookingCharge> _FacilityBookingChargeList;
        /// <summary>
        /// List property for FacilityBookingCharge
        /// </summary>
        /// <value>The FacilityBookingCharge list</value>
        [ACPropertyList(401, "FacilityBookingCharge")]
        public List<FacilityBookingCharge> FacilityBookingChargeList
        {
            get
            {
                if (_FacilityBookingChargeList == null)
                    _FacilityBookingChargeList = LoadFacilityBookingChargeList();
                return _FacilityBookingChargeList;
            }
        }

        private List<FacilityBookingCharge> LoadFacilityBookingChargeList()
        {
            if (SelectedFacilityBooking == null) return null;
            return SelectedFacilityBooking.FacilityBookingCharge_FacilityBooking.ToList();
        }
        #endregion

        #endregion

        #region Methods

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
            OnPropertyChanged("FacilityInventoryList");
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
                OnPropertyChanged("SelectedFacilityInventory");
                SetFacilityInventoryPosList();
            }
        }

        public bool IsEnabledLoad()
        {
            return SelectedFacilityInventory != null;
        }

        /// Searches this instance.
        /// </summary>
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

        /// Searches this instance.
        /// </summary>
        [ACMethodInfo(FacilityInventoryPos.ClassName, "en{'Reset'}de{'Zurücksetzen'}", 501)]
        public void ClearSearchPos()
        {
            if (!IsEnabledClearSearchPos())
                return;
            _InputCode = "";
            OnPropertyChanged("InputCode");
            SelectedFilterInventoryPosFacility = null;
            FilterInventoryPosLotNo = null;
            SelectedFilterInventoryPosMaterial = null;
            SelectedFilterInventoryPosState = null;
            FilterInventoryPosNotAvailable = null;
            FilterInventoryPosZeroQuantity = null;
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
            CloseTopDialog();
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoNewInventory);
            ShowDialog(this, DesignNameProgressBar);
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

        #endregion

        #region Methods -> ACvbBSO -> Delete

        [ACMethodInteraction("Delete", "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "SelectedFacilityInventory", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!IsEnabledDelete())
                return;
        }

        public bool IsEnabledDelete()
        {
            return SelectedFacilityInventory != null && SelectedFacilityInventory.MDFacilityInventoryState.FacilityInventoryState == MDFacilityInventoryState.FacilityInventoryStates.New;
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

        [ACMethodInfo("StartInventory", "en{'Start'}de{'Starten'}", 101)]
        public void StartInventory()
        {
            if (!IsEnabledStartInventory())
                return;
            MDFacilityInventoryState inProgressState = DatabaseApp.MDFacilityInventoryState.FirstOrDefault(c => c.MDFacilityInventoryStateIndex == (short)MDFacilityInventoryState.FacilityInventoryStates.InProgress);
            SelectedFacilityInventory.MDFacilityInventoryState = inProgressState;
            DatabaseApp.ACSaveChanges();
            OnPropertyChanged("SelectedFacilityInventory");
            OnPropertyChanged("SelectedFacilityInventoryPos");
            IsEnabledInventoryEdit = true;
            IsEnabledInventoryPosEdit = IsInventoryPosEnabledEdit();
        }

        public bool IsEnabledStartInventory()
        {
            return
                SelectedFacilityInventory != null &&
                SelectedFacilityInventory.MDFacilityInventoryState.MDFacilityInventoryStateIndex == (short)MDFacilityInventoryState.FacilityInventoryStates.New;
        }


        [ACMethodInfo("ClosingInventory", "en{'Closing'}de{'Schließen'}", 100)]
        public void ClosingInventory()
        {
            if (!IsEnabledClosingInventory())
                return;
            bool existAnyUnclosedPosition =
                SelectedFacilityInventory.FacilityInventoryPos_FacilityInventory.Any(c => c.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex != (short)MDFacilityInventoryPosState.FacilityInventoryPosStates.Finished);
            if (existAnyUnclosedPosition)
            {
                SelectedFilterInventoryPosState = FilterInventoryPosStateList.FirstOrDefault(c => c.MDFacilityInventoryPosStateIndex != (short)MDFacilityInventoryPosState.FacilityInventoryPosStates.Finished);
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
                SelectedFacilityInventory != null &&
                SelectedFacilityInventory.MDFacilityInventoryState.MDFacilityInventoryStateIndex == (short)MDFacilityInventoryState.FacilityInventoryStates.InProgress;
        }

        [ACMethodInfo("CloseAllPositions", "en{'Close all lines'}de{'Schließe alle Linien'}", 100)]
        public void CloseAllPositions()
        {
            // Question50055.
            var questionResult = Root.Messages.Question(this, "Question50055");
            if (questionResult == MsgResult.Yes)
            {
                MDFacilityInventoryPosState finishedState = DatabaseApp.MDFacilityInventoryPosState.FirstOrDefault(c => c.MDFacilityInventoryPosStateIndex == (short)MDFacilityInventoryPosState.FacilityInventoryPosStates.Finished);
                foreach (FacilityInventoryPos item in FacilityInventoryPosList)
                {
                    item.MDFacilityInventoryPosState = finishedState;
                }
                ACSaveChanges();
                OnPropertyChanged("FacilityInventoryPosList");
            }
        }

        public bool IsEnabledCloseAllPositions()
        {
            return SelectedFacilityInventory != null &&
                SelectedFacilityInventory.MDFacilityInventoryState.MDFacilityInventoryStateIndex == (short)MDFacilityInventoryState.FacilityInventoryStates.InProgress;
        }

        #endregion

        #region Methods -> Inventory Lifecycle -> InventoryPos
        // StartInventory

        [ACMethodInfo("StartInventoryPos", "en{'Start'}de{'Starten'}", 111)]
        public void StartInventoryPos()
        {
            if (!IsEnabledStartInventoryPos())
                return;
            SetInventoryPosState(SelectedFacilityInventoryPos, MDFacilityInventoryPosState.FacilityInventoryPosStates.InProgress);
            OnPropertyChanged("SelectedFacilityInventoryPos");
        }

        public bool IsEnabledStartInventoryPos()
        {
            return
                SelectedFacilityInventory != null
                && SelectedFacilityInventory.MDFacilityInventoryState.MDFacilityInventoryStateIndex == (short)MDFacilityInventoryState.FacilityInventoryStates.InProgress
                && SelectedFacilityInventoryPos != null
                && SelectedFacilityInventoryPos.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex == (short)MDFacilityInventoryPosState.FacilityInventoryPosStates.New;
        }


        [ACMethodInfo("ClosingInventoryPos", "en{'Closing'}de{'Schließen'}", 110)]
        public void ClosingInventoryPos()
        {
            if (!IsEnabledClosingInventoryPos())
                return;
            SetInventoryPosState(SelectedFacilityInventoryPos, MDFacilityInventoryPosState.FacilityInventoryPosStates.Finished);
            OnPropertyChanged("SelectedFacilityInventoryPos");
        }

        public bool IsEnabledClosingInventoryPos()
        {
            return
                SelectedFacilityInventoryPos != null &&
                SelectedFacilityInventoryPos.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex == (short)MDFacilityInventoryState.FacilityInventoryStates.InProgress;
        }

        private void SetInventoryPosState(FacilityInventoryPos pos, MDFacilityInventoryPosState.FacilityInventoryPosStates posState)
        {
            MDFacilityInventoryPosState inventoryState = DatabaseApp.MDFacilityInventoryPosState.FirstOrDefault(c => c.MDFacilityInventoryPosStateIndex == (short)posState);
            pos.MDFacilityInventoryPosState = inventoryState;
            DatabaseApp.ACSaveChanges();
        }

        #endregion


        #endregion

        #region Methods -> ACvBSO -> Save
        /// </summary>
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

        #endregion

        #region Mehtods -> Override

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
                    e.Result = ACFacilityManager.InventoryGenerate(NewFaciltiyInventoryNo, NewFaciltiyInventoryName, DoNewInventoryProgressCallback);
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
                    case BGWorkerMehtod_DoInventoryClosing:
                        MsgWithDetails closingMessage = e.Result as MsgWithDetails;
                        if (closingMessage.IsSucceded())
                        {
                            SelectedFacilityInventory.AutoRefresh();
                            OnPropertyChanged("SelectedFacilityInventory");
                            OnPropertyChanged("FacilityInventoryList");
                            SetFacilityBookingList();
                        }
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

        #endregion

        #endregion

        #region Feak Data

        public List<FacilityInventory> FeakInventories()
        {
            return null;
        }

        public List<FacilityInventoryPos> FeakInventoryPoses()
        {
            return null;
        }

        #endregion

        #region Messages

        #region Messages -> IMsgObserver

        public void SendMessage(Msg msg)
        {
            MsgList.Add(msg);
            OnPropertyChanged("MsgList");
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
        }

        #endregion

        #endregion

    }
}
