using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.Reflection;
using static gip.core.datamodel.Global;
using gip.mes.facility;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Business Service Object for managing laboratory orders in the system.
    /// This class provides functionality for creating, managing, and processing laboratory orders
    /// that can be associated with various order types including inbound orders, outbound orders, 
    /// production orders, facility lots, and picking positions.
    /// To search records enter the search string in the SearchWord property.
    /// The database result is copied to the LabOrderList property.
    /// Then call NavigateFirst() method to set CurrentLabOrder with the first record in the list.
    /// CurrentLabOrder is used to display and edit the currently selected record.
    /// Property changes should always be made to CurrentLabOrder and when all field values ​​have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
    /// Labotory order usually created automatically by workflow node or another module (eg. PAProcessModule).
    /// Also the laboratory order can be created manually by BSOLabOrder from laboratory order template.
    /// The laboratory order template is the template which contains data about laboratory order like a material and positions with measurement tags.
    /// The New() method creates a new record and assigns the new entity object to the CurrentLabOrder property.
    /// Then opens dialog LabOrderDialog to select for which enitity type is laboratory order (InOrderPos, OutOrderPos, ProdOrderPartslistPos, PickingPos or FacilityLot)
    /// After entity type is selected then needs to be selected concrete entity for selected entity type. 
    /// The laboratory order template now needs to be selected for the current laborder. The property DialogTemplateList provides all available templeates for selected material(Material property of the LabOrder).
    /// One template from the DialogTemplateList must be selected. Then the Sample taken date(SampleTakingDate) and Test date(TestDate) must be selected.
    /// On the end creating process we can select the laboratory order status, usually that is New.
    /// To create a new laboratory order from template the method DialogCreatePos() should be called.
    /// Then a newly created laboratory order was assigned to the property CurrentLabOrder.
    /// Laboratory order test parameters and results are presented in the positions list (LabOrderPosList).
    /// The property CurrentLabOrderPos is used to display and the edit currently selected labotory order position.
    /// Usually laboratory order positions are taken over from the template, but there is possiblity to add or remove position manually.
    /// To add a new position call the method NewLabOrderPos().
    /// After a new position was added, the properties as MDLabTag, ValueMinMin, ValueMin, ValueMax, ValueMaxMax, ReferenceValue needs to be setted.
    /// To remove position, select the target position and then call the method DeleteLabOrderPos().
    /// The property ActualValue in the LabOrderPos is used for test value.
    /// After all changes needs to be called Save() method to save changes to the database.
    /// Laboratory order status marks the state of the currently selected lab order.
    /// Laboratory orders also providing filtering capabilities for efficient order searching.
    /// The BSO automatically calculates statistic over laboratory tags.
    /// This BSO integrates with the manufacturing execution system (MES) to provide
    /// quality control and laboratory testing capabilities throughout the production process.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Lab Order'}de{'Laborauftrag'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + LabOrder.ClassName,
        Description = @"Business Service Object for managing laboratory orders in the system.
                        This class provides functionality for creating, managing, and processing laboratory orders
                        that can be associated with various order types including inbound orders, outbound orders, 
                        production orders, facility lots, and picking positions.
                        To search records enter the search string in the SearchWord property.
                        The database result is copied to the LabOrderList property.
                        Then call NavigateFirst() method to set CurrentLabOrder with the first record in the list.
                        CurrentLabOrder is used to display and edit the currently selected record.
                        Property changes should always be made to CurrentLabOrder and when all field values ​​have been changed, the Save() method should be called to save the changes in the database before navigating to the 
                        next record or creating a new record.
                        Labotory order usually created automatically by workflow node or another module (eg. PAProcessModule).
                        Also the laboratory order can be created manually by BSOLabOrder from laboratory order template.
                        The laboratory order template is the template which contains data about laboratory order like a material and positions with measurement tags.
                        The New() method creates a new record and assigns the new entity object to the CurrentLabOrder property.
                        Then opens dialog LabOrderDialog to select for which enitity type is laboratory order (InOrderPos, OutOrderPos, ProdOrderPartslistPos, PickingPos or FacilityLot)
                        After entity type is selected then needs to be selected concrete entity for selected entity type. 
                        The laboratory order template now needs to be selected for the current laborder. The property DialogTemplateList provides all available templeates for selected material(Material property of the LabOrder).
                        One template from the DialogTemplateList must be selected. Then the Sample taken date(SampleTakingDate) and Test date(TestDate) must be selected.
                        On the end creating process we can select the laboratory order status, usually that is New.
                        To create a new laboratory order from template the method DialogCreatePos() should be called.
                        Then a newly created laboratory order was assigned to the property CurrentLabOrder.
                        Laboratory order test parameters and results are presented in the positions list (LabOrderPosList).
                        The property CurrentLabOrderPos is used to display and the edit currently selected labotory order position.
                        Usually laboratory order positions are taken over from the template, but there is possiblity to add or remove position manually.
                        To add a new position call the method NewLabOrderPos().
                        After a new position was added, the properties as MDLabTag, ValueMinMin, ValueMin, ValueMax, ValueMaxMax, ReferenceValue needs to be setted.
                        To remove position, select the target position and then call the method DeleteLabOrderPos().
                        The property ActualValue in the LabOrderPos is used for test value.
                        Laboratory order status marks the state of the currently selected lab order.
                        Laboratory orders also providing filtering capabilities for efficient order searching.
                        The BSO automatically calculates statistic over laboratory tags.
                        This BSO integrates with the manufacturing execution system (MES) to provide
                        quality control and laboratory testing capabilities throughout the production process.")]
    [ACQueryInfo(Const.PackName_VarioMaterial, Const.QueryPrefix + LabOrder.ClassName, "en{'Lab Order'}de{'Laborauftrag'}", typeof(LabOrder), LabOrder.ClassName, "LabOrderTypeIndex", "LabOrderNo")]
    public class BSOLabOrder : BSOLabOrderBase
    {

        #region const

        public const string Const_FilterMaterialGroup = @"Material\MDMaterialGroup\MDKey";
        #endregion

        #region c'tors

        /// <summary>
        /// Initializes a new instance of the BSOLabOrder class.
        /// This constructor creates a business service object for managing laboratory orders in the manufacturing execution system.
        /// The BSO provides functionality for creating, managing, and processing laboratory orders that can be associated 
        /// with various order types including inbound orders, outbound orders, production orders, facility lots, and picking positions.
        /// </summary>
        /// <param name="acType">The ACClass type definition that contains metadata for this BSO instance.</param>
        /// <param name="content">The content object associated with this BSO instance.</param>
        /// <param name="parentACObject">The parent AC object in the composition hierarchy.</param>
        /// <param name="parameter">Optional parameter list for initialization values.</param>
        /// <param name="acIdentifier">Optional identifier for this BSO instance. Defaults to empty string if not specified.</param>
        public BSOLabOrder(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        /// <summary>
        /// Initializes the business service object for laboratory order management.
        /// This method is called during the startup phase to set up the BSO and its dependencies.
        /// It ensures proper initialization of the base class and validates that all required
        /// components are correctly configured for laboratory order operations.
        /// </summary>
        /// <param name="startChildMode">The startup mode that determines how child components should be initialized</param>
        /// <returns>Returns true if initialization was successful; otherwise false</returns>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        /// <summary>
        /// Performs cleanup and deinitialization of the BSOLabOrder instance.
        /// This method unsubscribes from property change events, clears internal references,
        /// and ensures proper disposal of resources before the BSO is destroyed.
        /// </summary>
        /// <param name="deleteACClassTask">Indicates whether the associated ACClassTask should be deleted during deinitialization</param>
        /// <returns>Returns true if deinitialization was successful; otherwise false</returns>
        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_LastLabOrder != null)
                _LastLabOrder.PropertyChanged -= CurrentLabOrder_PropertyChanged;
            _LastLabOrder = null;
            this._DialogSelectedTemplate = null;
            this._LabOrderMaterialState = null;
            this._LabOrderTemplate = null;
            var b = base.ACDeInit(deleteACClassTask);
            return b;
        }
        #endregion

        #region Filters

        private bool _IsConnectedWithDeliveryNote;
        /// <summary>
        /// Gets or sets a value indicating whether the laboratory orders should be filtered to show only those connected with delivery notes.
        /// When set to true, the filter will only display laboratory orders that have associated InOrderPos entries with corresponding delivery note positions.
        /// This property provides filtering capability to help users focus on laboratory orders related to goods receipt processes.
        /// </summary>
        [ACPropertyInfo(750, "FilterConnectedWithDN", "en{'Has Delivery Note'}de{'Verbunden mit Eingangslieferschein'}", 
                        Description = @"Gets or sets a value indicating whether the laboratory orders should be filtered to show only those connected with delivery notes.
                                        When set to true, the filter will only display laboratory orders that have associated InOrderPos entries with corresponding delivery note positions.
                                        This property provides filtering capability to help users focus on laboratory orders related to goods receipt processes.")]
        public bool IsConnectedWithDeliveryNote
        {
            get
            {
                return _IsConnectedWithDeliveryNote;
            }
            set
            {
                if (_IsConnectedWithDeliveryNote != value)
                {
                    _IsConnectedWithDeliveryNote = value;
                    OnIsConnectedWithDeliveryNoteChanged(value);
                    OnPropertyChanged(nameof(IsConnectedWithDeliveryNote));
                }
            }
        }

        /// <summary>
        /// Handles the property change event when the IsConnectedWithDeliveryNote filter value changes.
        /// This virtual method can be overridden in derived classes to implement custom behavior
        /// when the delivery note connection filter is modified.
        /// </summary>
        /// <param name="value">The new value indicating whether to filter laboratory orders connected with delivery notes</param>
        public virtual void OnIsConnectedWithDeliveryNoteChanged(bool value)
        {
            // do nothing
        }

        /// <summary>
        /// Gets or sets the start date filter for sample taking date range.
        /// This property provides filtering capability to search for laboratory orders based on when samples were taken.
        /// When set, it filters laboratory orders to show only those with sample taking dates greater than or equal to this value.
        /// The filter uses the first item in the SampleTakingDate filter array from the navigation query definition.
        /// </summary>
        [ACPropertyInfo(717, "FilterSampleTakingDateFrom", "en{'From'}de{'Von'}",
                        Description = @"Gets or sets the start date filter for sample taking date range.
                                        This property provides filtering capability to search for laboratory orders based on when samples were taken.
                                        When set, it filters laboratory orders to show only those with sample taking dates greater than or equal to this value.
                                        The filter uses the first item in the SampleTakingDate filter array from the navigation query definition.")]
        public DateTime? FilterSampleTakingDateFrom
        {
            get
            {
                ACFilterItem[] items = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == nameof(LabOrder.SampleTakingDate)).ToArray();
                if (items.Length != 2)
                    return null;

                DateTime dateTime = new DateTime();
                if (DateTime.TryParse(items[0].SearchWord, out dateTime))
                    return dateTime;
                else
                    return null;
            }
            set
            {
                ACFilterItem[] items = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == nameof(LabOrder.SampleTakingDate)).ToArray();
                if (items.Length == 2)
                {
                    items[0].SetSearchValue<DateTime?>(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the end date filter for sample taking date range.
        /// This property provides filtering capability to search for laboratory orders based on when samples were taken.
        /// When set, it filters laboratory orders to show only those with sample taking dates less than this value.
        /// The filter uses the second item in the SampleTakingDate filter array from the navigation query definition.
        /// </summary>
        [ACPropertyInfo(718, "FilterSampleTakingDateTo", "en{'To'}de{'Bis'}",
                        Description = @"Gets or sets the end date filter for sample taking date range.
                                        This property provides filtering capability to search for laboratory orders based on when samples were taken.
                                        When set, it filters laboratory orders to show only those with sample taking dates less than this value.
                                        The filter uses the second item in the SampleTakingDate filter array from the navigation query definition.")]
        public DateTime? FilterSampleTakingDateTo
        {
            get
            {
                ACFilterItem[] items = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == nameof(LabOrder.SampleTakingDate)).ToArray();
                if (items.Length != 2)
                    return null;

                DateTime dateTime = new DateTime();
                if (DateTime.TryParse(items[1].SearchWord, out dateTime))
                    return dateTime;
                else
                    return null;
            }
            set
            {
                DateTime? tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<DateTime?>(nameof(FacilityCharge.ExpirationDate));
                ACFilterItem[] items = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == nameof(LabOrder.SampleTakingDate)).ToArray();
                if (items.Length == 2)
                {
                    items[1].SetSearchValue<DateTime?>(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the filter value for order numbers across multiple order types.
        /// This property provides a unified search capability that filters laboratory orders by order numbers
        /// from facility lots, inbound orders, outbound orders, and production orders simultaneously.
        /// When set, it applies the same search value to all order number filter fields and triggers
        /// a property change notification to update the user interface.
        /// </summary>
        [ACPropertyInfo(755, "Filter", ConstApp.OrderNo,
                        Description = @"Gets or sets the filter value for order numbers across multiple order types.
                                        This property provides a unified search capability that filters laboratory orders by order numbers
                                        from facility lots, inbound orders, outbound orders, and production orders simultaneously.
                                        When set, it applies the same search value to all order number filter fields and triggers
                                        a property change notification to update the user interface.")]
        public string FilterOrderNo
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(FilterProgramNoName);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(FilterProgramNoName);
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(FilterFacilityLotNoName, value);
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(FilterInOrderNoName, value);
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(FilterOutOrderNoName, value);
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(FilterProgramNoName, value);


                    OnPropertyChanged(nameof(FilterOrderNo));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the sample taking date filter represents a wide range.
        /// Returns true if the date range between FilterSampleTakingDateFrom and FilterSampleTakingDateTo
        /// is greater than 3 days, which is used to determine query performance optimizations.
        /// </summary>
        public bool IsFilterSampleTakingDateWideRange
        {
            get
            {
                bool isWideRange = false;
                if (FilterSampleTakingDateFrom != null && FilterSampleTakingDateTo != null)
                {
                    isWideRange = Math.Abs((FilterSampleTakingDateTo.Value - FilterSampleTakingDateFrom.Value).TotalDays) > 3;
                }
                return isWideRange;
            }
        }


        #region Filters -> FilterMaterialGroup

        private MDMaterialGroup _SelectedFilterMaterialGroup;
        /// <summary>
        /// Gets or sets the selected material group filter for filtering laboratory orders.
        /// This property allows users to filter laboratory orders based on the material group 
        /// of the associated materials. When set, it updates the corresponding filter item
        /// in the navigation query definition to apply the material group filter.
        /// </summary>
        [ACPropertySelected(751, "FilterMaterialGroup", "en{'Material Group'}de{'Materialgruppe'}",
                            Description = @"Gets or sets the selected material group filter for filtering laboratory orders.
                                            This property allows users to filter laboratory orders based on the material group 
                                            of the associated materials. When set, it updates the corresponding filter item
                                            in the navigation query definition to apply the material group filter.")]
        public MDMaterialGroup SelectedFilterMaterialGroup
        {
            get
            {
                return _SelectedFilterMaterialGroup;
            }
            set
            {
                _SelectedFilterMaterialGroup = value;
                ACFilterItem filterItemMaterialGroup = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(x => x.PropertyName == Const_FilterMaterialGroup).FirstOrDefault();
                if (filterItemMaterialGroup != null)
                {
                    if (value == null)
                        filterItemMaterialGroup.SearchWord = null;
                    else
                        filterItemMaterialGroup.SearchWord = value.MDKey;
                }
                OnPropertyChanged(nameof(SelectedFilterMaterialGroup));
            }
        }

        private IEnumerable<MDMaterialGroup> _FilterMaterialGroupList;
        /// <summary>
        /// Gets the list of all material groups available for filtering laboratory orders.
        /// This property provides a collection of MDMaterialGroup entities ordered by their MDKey
        /// for use in the material group filter dropdown. The list is cached after first access
        /// and includes all material groups from the database.
        /// </summary>
        [ACPropertyList(752, "FilterMaterialGroup",
                        Description = @"Gets the list of all material groups available for filtering laboratory orders.
                                        This property provides a collection of MDMaterialGroup entities ordered by their MDKey
                                        for use in the material group filter dropdown. The list is cached after first access
                                        and includes all material groups from the database.")]
        public IEnumerable<MDMaterialGroup> FilterMaterialGroupList
        {
            get
            {
                if (_FilterMaterialGroupList == null)
                {
                    _FilterMaterialGroupList = DatabaseApp.MDMaterialGroup.OrderBy(x => x.MDKey);
                }
                return _FilterMaterialGroupList;
            }
        }

        #endregion

        #region Filter -> FilterDistributorCompany (Delivery company)

        #region FilterShipperCompany
        private Company _SelectedFilterDistributorCompany;
        /// <summary>
        /// Gets or sets the selected distributor company filter for filtering laboratory orders.
        /// This property allows users to filter laboratory orders based on the distributor/supplier company
        /// associated with the materials or order positions. When set, it enables filtering to show only
        /// laboratory orders related to materials or orders from the selected distributor company.
        /// </summary>
        [ACPropertySelected(753, "FilterDistributorCompany", "en{'Distributor'}de{'Lieferant'}",
                            Description = @"Gets or sets the selected distributor company filter for filtering laboratory orders.
                                            This property allows users to filter laboratory orders based on the distributor/supplier company
                                            associated with the materials or order positions. When set, it enables filtering to show only
                                            laboratory orders related to materials or orders from the selected distributor company.")]
        public Company SelectedFilterDistributorCompany
        {
            get
            {
                return _SelectedFilterDistributorCompany;
            }
            set
            {
                if (_SelectedFilterDistributorCompany != value)
                {
                    _SelectedFilterDistributorCompany = value;
                    OnPropertyChanged(nameof(SelectedFilterDistributorCompany));
                }
            }
        }

        private List<Company> _FilterDistributorCompanyList;
        /// <summary>
        /// Gets the list of all distributor companies available for filtering laboratory orders.
        /// This property provides a collection of Company entities that are marked as distributors,
        /// ordered by company name for use in the distributor company filter dropdown.
        /// The list is cached after first access and includes all active distributor companies from the database.
        /// </summary>
        [ACPropertyList(754, "FilterDistributorCompany",
                        Description = @"Gets the list of all distributor companies available for filtering laboratory orders.
                                        This property provides a collection of Company entities that are marked as distributors,
                                        ordered by company name for use in the distributor company filter dropdown.
                                        The list is cached after first access and includes all active distributor companies from the database.")]
        public List<Company> FilterDistributorCompanyList
        {
            get
            {
                if (_FilterDistributorCompanyList == null)
                    LoadFilterDistributorCompanyList();
                return _FilterDistributorCompanyList;
            }
        }

        private void LoadFilterDistributorCompanyList()
        {
            // @aagincic NOTE: IsActive is not edited in system so there is not this condition applyed jet.
            _FilterDistributorCompanyList =
                DatabaseApp.Company.Where(x => x.IsDistributor /* && x.IsActive*/).OrderBy(x => x.CompanyName).ToList();
        }
        #endregion


        #endregion

        #endregion

        #region BSO -> ACProperties

        #region BSO -> ACProperties -> Filter Item Names

        /// <summary>
        /// Gets the property name path for filtering laboratory orders by facility lot number.
        /// This property returns the navigation path to the FacilityLot.LotNo field used in filter queries.
        /// The path follows the pattern: "LabOrder.FacilityLot\FacilityLot.LotNo" for LINQ navigation.
        /// </summary>
        public string FilterFacilityLotNoName
        {
            get
            {

                return $"{nameof(LabOrder.FacilityLot)}\\{nameof(FacilityLot.LotNo)}";
            }
        }

        /// <summary>
        /// Gets the property name path for filtering laboratory orders by inbound order number.
        /// This property returns the navigation path to the InOrder.InOrderNo field used in filter queries.
        /// The path follows the pattern: "LabOrder.InOrderPos\InOrderPos.InOrder\InOrder.InOrderNo" for LINQ navigation.
        /// </summary>
        public string FilterInOrderNoName
        {
            get
            {
                return $"{nameof(LabOrder.InOrderPos)}\\{nameof(InOrderPos.InOrder)}\\{nameof(InOrder.InOrderNo)}";
            }
        }

        /// <summary>
        /// Gets the property name path for filtering laboratory orders by outbound order number.
        /// This property returns the navigation path to the OutOrder.OutOrderNo field used in filter queries.
        /// The path follows the pattern: "LabOrder.OutOrderPos\OutOrderPos.OutOrder\OutOrder.OutOrderNo" for LINQ navigation.
        /// </summary>
        public string FilterOutOrderNoName
        {
            get
            {
                return $"{nameof(LabOrder.OutOrderPos)}\\{nameof(OutOrderPos.OutOrder)}\\{nameof(OutOrder.OutOrderNo)}";
            }
        }

        /// <summary>
        /// Gets the property name path for filtering laboratory orders by production order program number.
        /// This property returns the navigation path to the ProdOrder.ProgramNo field used in filter queries.
        /// The path follows the pattern: "LabOrder.ProdOrderPartslistPos\ProdOrderPartslistPos.ProdOrderPartslist\ProdOrderPartslist.ProdOrder\ProdOrder.ProgramNo" for LINQ navigation.
        /// </summary>
        public string FilterProgramNoName
        {
            get
            {
                return $"{nameof(LabOrder.ProdOrderPartslistPos)}\\{nameof(ProdOrderPartslistPos.ProdOrderPartslist)}\\{nameof(ProdOrderPartslist.ProdOrder)}\\{nameof(ProdOrder.ProgramNo)}";
            }
        }

        #endregion

        #region BSO -> ACProperties -> AccessPrimary

        /// <summary>
        /// Gets the primary navigation access interface for laboratory order operations.
        /// This property provides access to the primary data navigation functionality,
        /// enabling search, filtering, and navigation through laboratory order records.
        /// It delegates to AccessPrimary which manages the underlying data access and query operations.
        /// </summary>
        public override IAccessNav AccessNav { get { return AccessPrimary; } }

        /// <summary>
        /// Executes custom filtering logic during laboratory order search navigation.
        /// This method extends the base search functionality by applying additional filters
        /// based on the IsConnectedWithDeliveryNote property to show only laboratory orders
        /// that are associated with delivery note positions when that filter is enabled.
        /// </summary>
        /// <param name="result">The base query result from the parent class that will be further filtered</param>
        /// <returns>The filtered IQueryable of LabOrder entities with applied custom search criteria</returns>
        public override IQueryable<LabOrder> LabOrder_AccessPrimary_NavSearchExecuting(IQueryable<LabOrder> result)
        {
            result = base.LabOrder_AccessPrimary_NavSearchExecuting(result);
            if (IsConnectedWithDeliveryNote)
            {
                result = result.Where(x => x.InOrderPosID != null && x.InOrderPos.DeliveryNotePos_InOrderPos.Any());
            }
            return result;
        }

        /// <summary>
        /// Gets the default filter configuration for the navigation query used in laboratory order searches.
        /// This property defines the standard filter items that are automatically applied when searching laboratory orders,
        /// including filters for lab order type, order number, material group, sample taking date range, and related order numbers.
        /// The filter configuration supports searching across multiple order types (facility lots, inbound orders, outbound orders, production orders)
        /// using OR logic grouping within parentheses to provide comprehensive search capabilities.
        /// </summary>
        public override List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {
                List<ACFilterItem> aCFilterItems = new List<ACFilterItem>();

                ACFilterItem phLabOrderTypeIndex = new ACFilterItem(Global.FilterTypes.filter, nameof(LabOrder.LabOrderTypeIndex), Global.LogicalOperators.equal, Global.Operators.and, ((short)FilterLabOrderType).ToString(), true);
                aCFilterItems.Add(phLabOrderTypeIndex);

                ACFilterItem phLabOrderNo = new ACFilterItem(FilterTypes.filter, nameof(LabOrder.LabOrderNo), LogicalOperators.contains, Operators.and, null, true, true);
                aCFilterItems.Add(phLabOrderNo);

                ACFilterItem phMaterialGroup = new ACFilterItem(Global.FilterTypes.filter, Const_FilterMaterialGroup, Global.LogicalOperators.equal, Global.Operators.and, null, true);
                aCFilterItems.Add(phMaterialGroup);

                ACFilterItem fromSampleTakingDate = new ACFilterItem(Global.FilterTypes.filter, nameof(LabOrder.SampleTakingDate), Global.LogicalOperators.greaterThanOrEqual, Global.Operators.and, null, true);
                aCFilterItems.Add(fromSampleTakingDate);

                ACFilterItem toSampleTakingDate = new ACFilterItem(Global.FilterTypes.filter, nameof(LabOrder.SampleTakingDate), Global.LogicalOperators.lessThan, Global.Operators.and, null, true);
                aCFilterItems.Add(toSampleTakingDate);

                List<ACFilterItem> orderFilterItems = new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, FilterFacilityLotNoName, Global.LogicalOperators.contains, Global.Operators.or, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, FilterInOrderNoName, Global.LogicalOperators.contains, Global.Operators.or, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, FilterOutOrderNoName, Global.LogicalOperators.contains, Global.Operators.or, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, FilterProgramNoName, Global.LogicalOperators.contains, Global.Operators.or, null, true),
                    new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                };

                aCFilterItems.AddRange(orderFilterItems);

                return aCFilterItems;
            }
        }

        #endregion

        LabOrder _LastLabOrder = null;

        /// <summary>
        /// Gets or sets the currently selected laboratory order. This property manages the current laboratory order
        /// being viewed or edited in the user interface. When set, it updates property change event handlers
        /// and manages the dialog template list based on the entity state.
        /// </summary>
        [ACPropertyCurrent(701, "LabOrder",
                           Description = @"Gets or sets the currently selected laboratory order. This property manages the current laboratory order
                                           being viewed or edited in the user interface. When set, it updates property change event handlers
                                           and manages the dialog template list based on the entity state.")]
        public override LabOrder CurrentLabOrder
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (base.CurrentLabOrder != value)
                {
                    if (_LastLabOrder != null)
                        _LastLabOrder.PropertyChanged -= CurrentLabOrder_PropertyChanged;
                    if (value == null || value.EntityState != System.Data.EntityState.Added)
                        _DialogTemplateList = null;
                    SetCurrentSelected(value);
                    _LastLabOrder = value;
                    if (_LastLabOrder != null)
                    {
                        _LastLabOrder.PropertyChanged += CurrentLabOrder_PropertyChanged;
                    }
                }
            }
        }

        protected virtual void CurrentLabOrder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MaterialID")
            {
                if (CurrentLabOrder == null || CurrentLabOrder.EntityState != System.Data.EntityState.Added)
                {
                    _DialogTemplateList = null;
                    OnPropertyChanged(nameof(DialogTemplateList));
                }
            }
            else if (e.PropertyName == "InOrderPosID")
            {
                if (CurrentLabOrder != null && CurrentLabOrder.InOrderPos != null)
                    CurrentLabOrder.Material = CurrentLabOrder.InOrderPos.Material;
                OnPropertyChanged(nameof(CurrentLabOrder));
            }
            else if (e.PropertyName == "OutOrderPosID")
            {
                if (CurrentLabOrder != null && CurrentLabOrder.OutOrderPos != null)
                    CurrentLabOrder.Material = CurrentLabOrder.OutOrderPos.Material;
                OnPropertyChanged(nameof(CurrentLabOrder));
            }
            else if (e.PropertyName == "ProdOrderPartslistPosID")
            {
                if (CurrentLabOrder != null && CurrentLabOrder.ProdOrderPartslistPos != null)
                {
                    // Last mixure represent a finall product - used templates for a this finall product
                    CurrentLabOrder.Material = CurrentLabOrder.ProdOrderPartslistPos.BookingMaterial;
                }
                OnPropertyChanged(nameof(CurrentLabOrder));
            }
            else if (e.PropertyName == "FacilityLotID")
            {
                if (CurrentLabOrder != null && CurrentLabOrder.FacilityLot != null)
                    CurrentLabOrder.Material = CurrentLabOrder.FacilityLot.Material;
                OnPropertyChanged(nameof(CurrentLabOrder));
            }
            else if (e.PropertyName == "PickingPosID")
            {
                if (CurrentLabOrder != null && CurrentLabOrder.PickingPos != null)
                    CurrentLabOrder.Material = CurrentLabOrder.PickingPos.Material;
                OnPropertyChanged(nameof(CurrentLabOrder));
            }
        }

        protected override void CurrentLabOrderPos_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.CurrentLabOrderPos_PropertyChanged(sender, e);
        }

        List<LabOrder> _DialogTemplateList = null;
        /// <summary>
        /// Gets the list of available laboratory order templates that can be used to create new laboratory orders.
        /// This property provides a filtered collection of LabOrder template entities based on the material
        /// associated with the current laboratory order. Templates are filtered to match either the direct
        /// material ID or the production material ID if available. The list is cached after first access
        /// and automatically refreshed when the current lab order's material changes.
        /// </summary>
        [ACPropertyList(710, "Template", "en{'Template'}de{'Vorlage'}", "", false,
                        Description = @"Gets the list of available laboratory order templates that can be used to create new laboratory orders.
                                        This property provides a filtered collection of LabOrder template entities based on the material
                                        associated with the current laboratory order. Templates are filtered to match either the direct
                                        material ID or the production material ID if available. The list is cached after first access
                                        and automatically refreshed when the current lab order's material changes.")]
        public List<LabOrder> DialogTemplateList
        {
            get
            {
                if (_DialogTemplateList == null && CurrentLabOrder != null && CurrentLabOrder.Material != null)
                {
                    Material material = null;
                    material = CurrentLabOrder.Material;
                    if (material != null)
                        _DialogTemplateList = LabOrderManager.ReturnLabOrderTemplateList(DatabaseApp).Where(c => c.MaterialID == material.MaterialID || (material.ProductionMaterialID.HasValue && c.MaterialID == material.ProductionMaterialID.Value)).ToList();
                }
                return _DialogTemplateList;
            }
        }

        LabOrder _DialogSelectedTemplate;

        /// <summary>
        /// Gets or sets the currently selected laboratory order template from the dialog template list.
        /// This property is used in the new laboratory order dialog to allow users to select which template
        /// should be used as the basis for creating a new laboratory order. The selected template will be
        /// used to copy laboratory order positions and their associated test parameters when creating
        /// the new laboratory order through the DialogCreatePos method.
        /// </summary>
        [ACPropertySelected(711, "Template", "en{'Template'}de{'Vorlage'}", "", false,
                            Description = @"Gets or sets the currently selected laboratory order template from the dialog template list.
                                            This property is used in the new laboratory order dialog to allow users to select which template
                                            should be used as the basis for creating a new laboratory order. The selected template will be
                                            used to copy laboratory order positions and their associated test parameters when creating
                                            the new laboratory order through the DialogCreatePos method.")]
        public LabOrder DialogSelectedTemplate
        {
            get
            {
                return _DialogSelectedTemplate;
            }
            set
            {
                if (_DialogSelectedTemplate != value)
                    _DialogSelectedTemplate = value;
            }
        }

        /// <summary>
        /// Gets the list of available material states for laboratory orders.
        /// This property provides a collection of ACValueItem objects representing different position types
        /// that can be associated with laboratory orders, such as InOrderPos, OutOrderPos, PartslistPos, 
        /// LotCharge, and PickingPos. The list is used in the laboratory order creation dialog to allow
        /// users to select the appropriate material state or position type for the new laboratory order.
        /// </summary>
        [ACPropertyInfo(712, "Dialog", "en{'From Position Type'}de{'Aus Positionsart'}", "LabOrderMaterialStateList", false,
                        Description = @"Gets the list of available material states for laboratory orders.
                                        This property provides a collection of ACValueItem objects representing different position types
                                        that can be associated with laboratory orders, such as InOrderPos, OutOrderPos, PartslistPos, 
                                        LotCharge, and PickingPos. The list is used in the laboratory order creation dialog to allow
                                        users to select the appropriate material state or position type for the new laboratory order.")]
        public ACValueItemList LabOrderMaterialStateList
        {
            get
            {
                return GlobalApp.LabOrderMaterialStateList;
            }
        }

        ACValueItem _LabOrderMaterialState = GlobalApp.LabOrderMaterialStateList.First();
        /// <summary>
        /// Gets or sets the selected material state item that determines the position type for laboratory order creation.
        /// This property controls which type of order position (InOrderPos, OutOrderPos, PartslistPos, LotCharge, or PickingPos) 
        /// the laboratory order should be associated with. When changed, it resets the current laboratory order's material 
        /// and clears the selected template to ensure consistency in the laboratory order creation dialog.
        /// </summary>
        [ACPropertyInfo(713, "Dialog", "en{'From Position Type'}de{'Aus Positionsart'}", "LabOrderMaterialState", false,
                        Description = @"Gets or sets the selected material state item that determines the position type for laboratory order creation.
                                        This property controls which type of order position (InOrderPos, OutOrderPos, PartslistPos, LotCharge, or PickingPos) 
                                        the laboratory order should be associated with. When changed, it resets the current laboratory order's material 
                                        and clears the selected template to ensure consistency in the laboratory order creation dialog.")]
        public ACValueItem LabOrderMaterialState
        {
            get
            {
                return _LabOrderMaterialState;
            }
            set
            {
                if (_LabOrderMaterialState != value)
                {
                    _LabOrderMaterialState = value;
                    CurrentLabOrder.Material = null;
                    _DialogSelectedTemplate = null;
                    OnPropertyChanged(nameof(CurrentLabOrder));
                    OnPropertyChanged(nameof(DialogSelectedTemplate));
                    _DialogTemplateList = null;
                    OnPropertyChanged(nameof(DialogTemplateList));
                }
            }
        }

        LabOrder _LabOrderTemplate;
        /// <summary>
        /// Gets the template number of the laboratory order template that the current laboratory order is based on.
        /// Returns the LabOrderNo as a string if a template exists, otherwise returns an empty string.
        /// This property is used to display the template identification number for laboratory orders created from templates.
        /// </summary>
        [ACPropertyInfo(714, "LabOrder", "en{'Template No.'}de{'Vorlage Nr.'}", "LabOrderTemplateNo", false,
                        Description = @"Gets the template number of the laboratory order template that the current laboratory order is based on.
                                        Returns the LabOrderNo as a string if a template exists, otherwise returns an empty string.
                                        This property is used to display the template identification number for laboratory orders created from templates.")]
        public string LabOrderTemplateNo
        {
            get
            {
                if (_LabOrderTemplate != null)
                    return _LabOrderTemplate.LabOrderNo.ToString();
                return "";
            }
        }

        /// <summary>
        /// Gets the template name of the laboratory order template that the current laboratory order is based on.
        /// Returns the TemplateName as a string if a template exists, otherwise returns an empty string.
        /// This property is used to display the template name for laboratory orders created from templates.
        /// </summary>
        [ACPropertyInfo(715, "LabOrder", "en{'Template Name'}de{'Vorlagebezeichnung'}", "LabOrderTemplateName", false,
                        Description = @"Gets the template name of the laboratory order template that the current laboratory order is based on.
                                        Returns the TemplateName as a string if a template exists, otherwise returns an empty string.
                                        This property is used to display the template name for laboratory orders created from templates.")]
        public string LabOrderTemplateName
        {
            get
            {
                if (_LabOrderTemplate != null)
                {
                    if (!string.IsNullOrEmpty(_LabOrderTemplate.TemplateName))
                    {
                        return _LabOrderTemplate.TemplateName;
                    }
                    else
                        return "";
                }
                else
                    return "";
            }
        }

        /// <summary>
        /// Gets a value indicating whether this laboratory order BSO is running as a parent component in the Businessobjects context.
        /// Returns true if the ParentACComponent is of type Businessobjects, which means this BSO is operating 
        /// at the root level of the business object hierarchy rather than as a child dialog or sub-component.
        /// This property is used to determine the operational context and control the behavior of laboratory order management features.
        /// </summary>
        [ACPropertyInfo(716, "LabOrder", "en{'IsLabOrderParent'}de{'IsLabOrderParent'}", "LabOrderTemplateName", false,
                        Description = @"Gets a value indicating whether this laboratory order BSO is running as a parent component in the Businessobjects context.
                                        Returns true if the ParentACComponent is of type Businessobjects, which means this BSO is operating 
                                        at the root level of the business object hierarchy rather than as a child dialog or sub-component.
                                        This property is used to determine the operational context and control the behavior of laboratory order management features.")]
        public bool IsLabOrderParent
        {
            get
            {
                return ParentACComponent is Businessobjects;
            }
        }

        /// <summary>
        /// Gets the list of inbound order positions that are connected with delivery notes and available for laboratory order assignment.
        /// This property provides a filtered collection of InOrderPos entities that have associated delivery note positions,
        /// indicating they represent materials that have been received and can be subject to laboratory testing.
        /// The list is used in laboratory order dialogs to allow users to select which received materials
        /// should be associated with new laboratory orders for quality control testing.
        /// </summary>
        [ACPropertyList(717, "LabOrderInOrderPos", "en{'InOrderPos'}de{'InOrderPos'}",
                        Description = @"Gets the list of inbound order positions that are connected with delivery notes and available for laboratory order assignment.
                                        This property provides a filtered collection of InOrderPos entities that have associated delivery note positions,
                                        indicating they represent materials that have been received and can be subject to laboratory testing.
                                        The list is used in laboratory order dialogs to allow users to select which received materials
                                        should be associated with new laboratory orders for quality control testing.")]
        public List<InOrderPos> LabOrderInOrderPosList
        {
            get
            {
                return DatabaseApp.InOrderPos.Where(c => c.DeliveryNotePos_InOrderPos.Any()).ToList();
            }
        }

        /// <summary>
        /// Gets the list of outbound order positions that are connected with delivery notes and available for laboratory order assignment.
        /// This property provides a filtered collection of OutOrderPos entities that have associated delivery note positions,
        /// indicating they represent materials that have been shipped and can be subject to laboratory testing for quality control.
        /// The list is used in laboratory order dialogs to allow users to select which shipped materials
        /// should be associated with new laboratory orders for quality assurance testing.
        /// </summary>
        [ACPropertyList(718, "LabOrderOutOrderPos", "en{'OutOrderPos'}de{'OutOrderPos'}",
                        Description = @"Gets the list of outbound order positions that are connected with delivery notes and available for laboratory order assignment.
                                        This property provides a filtered collection of OutOrderPos entities that have associated delivery note positions,
                                        indicating they represent materials that have been shipped and can be subject to laboratory testing for quality control.
                                        The list is used in laboratory order dialogs to allow users to select which shipped materials
                                        should be associated with new laboratory orders for quality assurance testing.")]
        public List<OutOrderPos> LabOrderOutOrderPosList
        {
            get
            {
                return DatabaseApp.OutOrderPos.Where(c => c.DeliveryNotePos_OutOrderPos.Any()).ToList();
            }
        }
        #endregion

        #region BSO -> ACMethods

        bool _IsMaterialStateEnabled = true;
        bool _IsLabOrderPosViewDialog = false;
        bool _IsNewLabOrderDialogOpen = false;
        public VBDialogResult DialogResult;

        /// <summary>
        /// Determines the control modes (Enabled, Disabled, Hidden, etc.) for UI controls based on the current state of the laboratory order dialog.
        /// This method handles visibility and interaction rules for different order position types (InOrderPos, OutOrderPos, ProdOrderPartslistPos, FacilityLot)
        /// during new laboratory order creation and existing laboratory order viewing. It manages control states based on material state selection,
        /// dialog open status, and existing entity associations to ensure proper user interface behavior and data integrity.
        /// </summary>
        /// <param name="vbControl">The VB control whose mode needs to be determined</param>
        /// <returns>The appropriate control mode for the specified control</returns>
        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            if (vbControl == null)
                return base.OnGetControlModes(vbControl);
            Global.ControlModes result = base.OnGetControlModes(vbControl);

            if (result < Global.ControlModes.Enabled)
                return result;
            if (_IsNewLabOrderDialogOpen && !string.IsNullOrEmpty(vbControl.VBContent))
            {
                if (vbControl.VBContent.StartsWith("CurrentLabOrder\\InOrderPos"))
                    if (!_IsMaterialStateEnabled && CurrentLabOrder.InOrderPos != null)
                        result = Global.ControlModes.Disabled;
                    else if (_LabOrderMaterialState != null && _LabOrderMaterialState.Value.ToString() == InOrderPos.ClassName)
                        result = Global.ControlModes.Enabled;
                    else
                    {
                        CurrentLabOrder.InOrderPos = null;
                        result = Global.ControlModes.Hidden;
                    }

                else if (vbControl.VBContent.StartsWith("CurrentLabOrder\\OutOrderPos"))
                    if (!_IsMaterialStateEnabled && CurrentLabOrder.OutOrderPos != null)
                        result = Global.ControlModes.Disabled;
                    else if (_LabOrderMaterialState != null && _LabOrderMaterialState.Value.ToString() == OutOrderPos.ClassName)
                        result = Global.ControlModes.Enabled;
                    else
                    {
                        CurrentLabOrder.OutOrderPos = null;
                        result = Global.ControlModes.Hidden;
                    }

                else if (vbControl.VBContent.StartsWith("CurrentLabOrder\\ProdOrderPartslistPos"))
                    if (!_IsMaterialStateEnabled && CurrentLabOrder.ProdOrderPartslistPos != null)
                        result = Global.ControlModes.Disabled;
                    else if (_LabOrderMaterialState != null && _LabOrderMaterialState.Value.ToString() == "PartslistPos")
                        result = Global.ControlModes.Enabled;
                    else
                    {
                        CurrentLabOrder.ProdOrderPartslistPos = null;
                        result = Global.ControlModes.Hidden;
                    }

                else if (vbControl.VBContent.StartsWith("CurrentLabOrder\\FacilityLot"))
                    if (!_IsMaterialStateEnabled && CurrentLabOrder.FacilityLot != null)
                        result = Global.ControlModes.Disabled;
                    else if (_LabOrderMaterialState != null && _LabOrderMaterialState.Value.ToString() == "LotCharge")
                        result = Global.ControlModes.Enabled;
                    else
                    {
                        CurrentLabOrder.FacilityLot = null;
                        result = Global.ControlModes.Hidden;
                    }
            }
            else if (!string.IsNullOrEmpty(vbControl.VBContent) && CurrentLabOrder != null && !_IsNewLabOrderDialogOpen)
            {
                if (vbControl.VBContent.StartsWith("CurrentLabOrder\\InOrderPos"))
                    if (CurrentLabOrder.InOrderPos != null)
                        result = Global.ControlModes.Disabled;
                    else
                        result = Global.ControlModes.Hidden;

                else if (vbControl.VBContent.StartsWith("CurrentLabOrder\\OutOrderPos"))
                    if (CurrentLabOrder.OutOrderPos != null)
                        result = Global.ControlModes.Disabled;
                    else
                        result = Global.ControlModes.Hidden;

                else if (vbControl.VBContent.StartsWith("CurrentLabOrder\\ProdOrderPartslistPos"))
                    if (CurrentLabOrder.ProdOrderPartslistPos != null)
                        result = Global.ControlModes.Disabled;
                    else
                        result = Global.ControlModes.Hidden;

                else if (vbControl.VBContent.StartsWith("CurrentLabOrder\\FacilityLot"))
                    if (CurrentLabOrder.FacilityLot != null)
                        result = Global.ControlModes.Disabled;
                    else
                        result = Global.ControlModes.Hidden;
            }
            else if (vbControl.VBContent != null && CurrentLabOrder == null && !_IsNewLabOrderDialogOpen)
            {
                if (vbControl.VBContent.StartsWith("CurrentLabOrder\\InOrderPos") || vbControl.VBContent.StartsWith("CurrentLabOrder\\OutOrderPos") ||
                    vbControl.VBContent.StartsWith("CurrentLabOrder\\ProdOrderPartslistPos") || vbControl.VBContent.StartsWith("CurrentLabOrder\\FacilityLot"))
                    result = Global.ControlModes.Hidden;
            }

            if (!string.IsNullOrEmpty(vbControl.VBContent) && vbControl.VBContent.StartsWith("DialogSelectedTemplate"))
            {
                result = Global.ControlModes.EnabledRequired;
            }

            if (!_IsMaterialStateEnabled && !string.IsNullOrEmpty(vbControl.VBContent) && vbControl.VBContent.StartsWith("LabOrderMaterialState"))
                result = Global.ControlModes.Disabled;

            if (isCurrentPosInChange && vbControl.VBContent == @"CurrentLabOrderPos\ActualValue")
            {
                // @aagincic: i don't know exist bether VB way to doing this (focusing ActualValue by selection new pos param)
                MethodInfo mth = vbControl.GetType().GetMethod("Focus");
                if (mth != null)
                    mth.Invoke(vbControl, null);
                isCurrentPosInChange = false;
            }

            return result;
        }

        private void SetPosToNull()
        {
            CurrentLabOrder.InOrderPos = null;
            CurrentLabOrder.OutOrderPos = null;
            CurrentLabOrder.ProdOrderPartslistPos = null;
            CurrentLabOrder.FacilityLot = null;
        }

        /// <summary>
        /// Sets the specified laboratory order as the currently selected record and updates related cached data.
        /// This method calls the base implementation to perform the selection operation and then refreshes
        /// the laboratory order position item average list and template information if the selection changed.
        /// The method ensures that UI-bound properties and cached collections are properly synchronized
        /// when the current laboratory order selection changes.
        /// </summary>
        /// <param name="value">The LabOrder entity to set as the current selection. Can be null to clear the selection.</param>
        /// <returns>Returns true if the current selection was changed; otherwise false. Returns false if AccessPrimary is null.</returns>
        public override bool SetCurrentSelected(LabOrder value)
        {
            if (AccessPrimary == null)
                return false;
            bool isChanged = base.SetCurrentSelected(value);
            if (isChanged)
            {
                _LabOrderPosItemAVGList = null;
                OnPropertyChanged(nameof(LabOrderPosItemAVGList));
                ChangeLabOrderTemplateNoName();
            }
            return isChanged;
        }

        /// <summary>
        /// Creates and displays a dialog for creating a new laboratory order with optional pre-populated entity associations.
        /// This method initializes a new laboratory order and automatically associates it with the provided order position entities
        /// (InOrderPos, OutOrderPos, ProdOrderPartslistPos, FacilityLot, or PickingPos). If no entities are provided, the user
        /// can manually select the entity type and specific entity. The method retrieves or creates default laboratory templates
        /// based on the associated material and automatically proceeds with template application if only one template is available.
        /// If multiple templates exist, it displays a selection dialog for the user to choose the appropriate template.
        /// </summary>
        /// <param name="inOrderPos">The inbound delivery note position to associate with the laboratory order. Used for incoming material testing.</param>
        /// <param name="outOrderPos">The outbound delivery note position to associate with the laboratory order. Used for outgoing material testing.</param>
        /// <param name="prodOrderPartslistPos">The production order parts list position to associate with the laboratory order. Used for production material testing.</param>
        /// <param name="facilityLot">The facility lot/charge to associate with the laboratory order. Used for lot-based material testing.</param>
        /// <param name="pickingPos">The picking position to associate with the laboratory order. Used for picked material testing.</param>
        /// <returns>A VBDialogResult indicating the outcome of the dialog operation. Returns OK if the laboratory order was successfully created, or Cancel if the operation was cancelled or failed.</returns>
        [ACMethodInfo("Dialog", "en{'New Lab Order'}de{'Neuer Laborauftrag'}", 701,
                      Description = @"Creates and displays a dialog for creating a new laboratory order with optional pre-populated entity associations.
                                      This method initializes a new laboratory order and automatically associates it with the provided order position entities
                                      (InOrderPos, OutOrderPos, ProdOrderPartslistPos, FacilityLot, or PickingPos). If no entities are provided, the user
                                      can manually select the entity type and specific entity. The method retrieves or creates default laboratory templates
                                      based on the associated material and automatically proceeds with template application if only one template is available.
                                      If multiple templates exist, it displays a selection dialog for the user to choose the appropriate template.")]
        public VBDialogResult NewLabOrderDialog(DeliveryNotePos inOrderPos, DeliveryNotePos outOrderPos, ProdOrderPartslistPos prodOrderPartslistPos, FacilityLot facilityLot, PickingPos pickingPos)
        {
            if (DialogResult == null)
                DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.Cancel;
            bool hasOrderEntities = !(inOrderPos == null && outOrderPos == null && prodOrderPartslistPos == null && facilityLot == null && pickingPos == null);
            if (!hasOrderEntities)
                _IsMaterialStateEnabled = true;
            else
                FilterDialog(inOrderPos != null ? inOrderPos.InOrderPos : null, outOrderPos != null ? outOrderPos.OutOrderPos : null, prodOrderPartslistPos, facilityLot, pickingPos,  null, null);

            List<LabOrder> labOrderTemplates = null;
            if (hasOrderEntities)
            {
                Msg msg = LabOrderManager.GetOrCreateDefaultLabTemplate(DatabaseApp, inOrderPos, outOrderPos, prodOrderPartslistPos, facilityLot, pickingPos, out labOrderTemplates);
                if (msg != null)
                {
                    Messages.Msg(msg, MsgResult.OK);
                    return new VBDialogResult() { SelectedCommand = eMsgButton.Cancel };
                }
                _DialogTemplateList = labOrderTemplates;
            }

            base.New();
            CurrentLabOrder.MDLabOrderState = DatabaseApp.MDLabOrderState.FirstOrDefault(c => c.IsDefault);
            if (inOrderPos != null)
            {
                _IsMaterialStateEnabled = false;
                _LabOrderMaterialState = LabOrderMaterialStateList.FirstOrDefault(c => c.Value.ToString() == nameof(GlobalApp.LabOrderMaterialState.InOrderPos));
                CurrentLabOrder.InOrderPosID = inOrderPos.InOrderPosID;
            }

            if (outOrderPos != null)
            {
                _IsMaterialStateEnabled = false;
                _LabOrderMaterialState = LabOrderMaterialStateList.FirstOrDefault(c => c.Value.ToString() == nameof(GlobalApp.LabOrderMaterialState.OutOrderPos));
                CurrentLabOrder.OutOrderPosID = outOrderPos.OutOrderPosID;
            }

            if (prodOrderPartslistPos != null)
            {
                _IsMaterialStateEnabled = false;
                _LabOrderMaterialState = LabOrderMaterialStateList.FirstOrDefault(c => c.Value.ToString() == nameof(GlobalApp.LabOrderMaterialState.PartslistPos));
                CurrentLabOrder.ProdOrderPartslistPos = prodOrderPartslistPos;
            }

            if (facilityLot != null)
            {
                _IsMaterialStateEnabled = false;
                _LabOrderMaterialState = LabOrderMaterialStateList.FirstOrDefault(c => c.Value.ToString() == nameof(GlobalApp.LabOrderMaterialState.LotCharge));
                CurrentLabOrder.FacilityLot = facilityLot;
            }

            if (pickingPos != null)
            {
                _IsMaterialStateEnabled = false;
                _LabOrderMaterialState = LabOrderMaterialStateList.FirstOrDefault(c => c.Value.ToString() == nameof(GlobalApp.LabOrderMaterialState.PickingPos));
                CurrentLabOrder.PickingPos = pickingPos;
            }

            _IsNewLabOrderDialogOpen = true;
            //do not change DialogTemplateList.Count() == 1 to DialogTemplateList.Any() if exist more than one template user must select template manually
            if (DialogTemplateList != null && DialogTemplateList.Count() == 1)
            {
                _DialogSelectedTemplate = DialogTemplateList.FirstOrDefault();
                DialogCreatePos();
            }
            else
            {
                ShowDialog(this, "LabOrderDialog");
                if (DialogResult.SelectedCommand != eMsgButton.OK)
                {
                    base.UndoSave();
                    _IsNewLabOrderDialogOpen = false;
                    Search();
                }
            }
            return DialogResult;
        }

        /// <summary>
        /// Creates laboratory order positions from the selected template and finalizes the laboratory order creation process.
        /// This method validates that a material and template are selected, then copies template positions to the current
        /// laboratory order. After successful creation, it closes the dialog, loads the position list, and either displays
        /// a view dialog for child components or saves changes for parent components. The method handles different order
        /// position types (InOrderPos, OutOrderPos, ProdOrderPartslistPos, FacilityLot, PickingPos) and manages the
        /// laboratory order workflow state accordingly.
        /// </summary>
        [ACMethodInteraction("Dialog", "en{'Create'}de{'Generieren'}", 702, true, "DialogCreatePos", Global.ACKinds.MSMethodPrePost,
                             Description = @"Creates laboratory order positions from the selected template and finalizes the laboratory order creation process.
                                             This method validates that a material and template are selected, then copies template positions to the current
                                             laboratory order. After successful creation, it closes the dialog, loads the position list, and either displays
                                             a view dialog for child components or saves changes for parent components. The method handles different order
                                             position types (InOrderPos, OutOrderPos, ProdOrderPartslistPos, FacilityLot, PickingPos) and manages the
                                             laboratory order workflow state accordingly.")]
        public void DialogCreatePos()
        {
            if (CurrentLabOrder.Material == null)
            {
                Messages.Warning(this, "Warning50002");
                return;
            }
            if (_DialogSelectedTemplate == null && !_IsMaterialStateEnabled)
            {
                Messages.Warning(this, "Warning50003");
                return;
            }
            DialogResult.SelectedCommand = eMsgButton.OK;
            LabOrderManager.CopyLabOrderTemplatePos(DatabaseApp, CurrentLabOrder, _DialogSelectedTemplate);
            OnLaborderCopied(DatabaseApp, CurrentLabOrder, _DialogSelectedTemplate);
            _IsNewLabOrderDialogOpen = false;
            CloseTopDialog();
            LoadLabOrderPosList(CurrentLabOrder);
            _IsLabOrderPosViewDialog = false;
            if (!IsLabOrderParent)
            {
                if (CurrentLabOrder.InOrderPos != null)
                    ShowLabOrderViewDialog(CurrentLabOrder.InOrderPos, null, null, null, null, null, CurrentLabOrder.EntityState != System.Data.EntityState.Added, null);
                else if (CurrentLabOrder.OutOrderPos != null)
                    ShowLabOrderViewDialog(null, CurrentLabOrder.OutOrderPos, null, null, null, null, CurrentLabOrder.EntityState != System.Data.EntityState.Added, null);
                else if (CurrentLabOrder.ProdOrderPartslistPos != null)
                    ShowLabOrderViewDialog(null, null, CurrentLabOrder.ProdOrderPartslistPos, null, null, null, CurrentLabOrder.EntityState != System.Data.EntityState.Added, null);
                else if (CurrentLabOrder.PickingPos != null)
                    ShowLabOrderViewDialog(null, null, null, null, CurrentLabOrder.PickingPos, null, CurrentLabOrder.EntityState != System.Data.EntityState.Added, null);
                else if (CurrentLabOrder.FacilityLot != null)
                    ShowLabOrderViewDialog(null, null, null, CurrentLabOrder.FacilityLot, null, null, CurrentLabOrder.EntityState != System.Data.EntityState.Added, null);
                else
                    ShowLabOrderViewDialog(null, null, null, null, null, null, CurrentLabOrder.EntityState != System.Data.EntityState.Added, null);
            }
            else
            {
                Save();
                base.OnPropertyChanged(nameof(LabOrderPosList));
                ChangeLabOrderTemplateNoName();
            }
        }

        /// <summary>
        /// Called after a laboratory order has been successfully copied from a template.
        /// This virtual method provides an extension point for derived classes to perform additional
        /// operations or customizations after the laboratory order positions and data have been
        /// copied from the selected template to the current laboratory order.
        /// Override this method in derived classes to implement custom post-copy logic.
        /// </summary>
        /// <param name="dbApp">The database application context used for data operations</param>
        /// <param name="current">The newly created laboratory order that received the copied data</param>
        /// <param name="template">The template laboratory order that served as the source for copying</param>
        protected virtual void OnLaborderCopied(DatabaseApp dbApp, LabOrder current, LabOrder template)
        {
        }

        /// <summary>
        /// Cancels the laboratory order creation dialog and closes the top dialog.
        /// This method is called when the user cancels the new laboratory order creation process,
        /// closing the dialog without creating or saving any laboratory order data.
        /// </summary>
        [ACMethodInteraction("Dialog", Const.Cancel, 703, true, "DialogCancelPos", Global.ACKinds.MSMethodPrePost,
                             Description = @"Cancels the laboratory order creation dialog and closes the top dialog.
                                             This method is called when the user cancels the new laboratory order creation process,
                                             closing the dialog without creating or saving any laboratory order data.")]
        public void DialogCancelPos()
        {
            CloseTopDialog();
        }

        /// <summary>
        /// Displays a laboratory order view dialog for examining and editing laboratory orders associated with various order position types.
        /// This method provides a unified interface for viewing laboratory orders linked to inbound orders, outbound orders, 
        /// production orders, facility lots, picking positions, or specific laboratory orders. When the filter parameter is true,
        /// it applies appropriate filters to show only laboratory orders related to the provided entities. The method opens
        /// a modal dialog, saves any changes made, and properly handles cleanup including stopping the current component
        /// and setting dialog results for workflow integration.
        /// </summary>
        [ACMethodInteraction("Dialog", "en{'View Lab Order'}de{'Laborauftrag anzeigen'}", 704, true, "ShowLabOrderViewDialog", Global.ACKinds.MSMethodPrePost,
                             Description = @"Displays a laboratory order view dialog for examining and editing laboratory orders associated with various order position types.
                                             This method provides a unified interface for viewing laboratory orders linked to inbound orders, outbound orders, 
                                             production orders, facility lots, picking positions, or specific laboratory orders. When the filter parameter is true,
                                             it applies appropriate filters to show only laboratory orders related to the provided entities. The method opens
                                             a modal dialog, saves any changes made, and properly handles cleanup including stopping the current component
                                             and setting dialog results for workflow integration.")]
        public void ShowLabOrderViewDialog(
            InOrderPos inOrderPos,
            OutOrderPos outOrderPos,
            ProdOrderPartslistPos prodOrderPartslistPos,
            FacilityLot facilityLot,
            PickingPos pickingPos,
            LabOrder labOrder,
            bool filter,
            PAOrderInfo orderInfo)
        {
            if (filter)
                FilterDialog(inOrderPos, outOrderPos, prodOrderPartslistPos, facilityLot, pickingPos, labOrder, orderInfo);

            //if (inOrderPos == null && outOrderPos == null && prodOrderPartslistPos == null && facilityLot == null && pickingPos == null)
            //{
            //    ACComponent childBSO = ACUrlCommand("?LabOrderDialog") as ACComponent;
            //    if (childBSO == null)
            //        childBSO = StartComponent("LabOrderDialog", null, new object[] { }) as ACComponent;
            //}
            ShowDialog(this, "LabOrderViewDialog");
            Save();
            CloseTopDialog();
            if (orderInfo != null)
                orderInfo.DialogResult = this.DialogResult;
            this.ParentACComponent.StopComponent(this);
        }

        /// <summary>
        /// Configures dialog filters for laboratory orders based on the provided entity parameters.
        /// This method dynamically applies filter criteria to the navigation query definition to show only laboratory orders 
        /// associated with the specified entity type (InOrderPos, OutOrderPos, ProdOrderPartslistPos, FacilityLot, PickingPos, or LabOrder).
        /// When an entity is provided, it removes conflicting filter items for other entity types and either creates or updates 
        /// the appropriate filter item for the target entity. If orderInfo is provided without specific entities, it attempts 
        /// to resolve entities using LabOrderManager before applying filters. After configuring filters, it triggers a search 
        /// to refresh the laboratory order list and sets the view dialog flag to indicate filtered results.
        /// </summary>
        /// <param name="inOrderPos">The inbound order position to filter laboratory orders by. When provided, shows only lab orders associated with this purchase order position.</param>
        /// <param name="outOrderPos">The outbound order position to filter laboratory orders by. When provided, shows only lab orders associated with this sales order position.</param>
        /// <param name="prodOrderPartslistPos">The production order parts list position to filter laboratory orders by. When provided, shows only lab orders associated with this production component.</param>
        /// <param name="facilityLot">The facility lot to filter laboratory orders by. When provided, shows only lab orders associated with this lot/charge.</param>
        /// <param name="pickingPos">The picking position to filter laboratory orders by. When provided, shows only lab orders associated with this picking line.</param>
        /// <param name="labOrder">The specific laboratory order to filter by. When provided, shows only this particular lab order.</param>
        /// <param name="orderInfo">Order information container that can be used to resolve entities when specific entity parameters are null. Contains workflow context for entity resolution.</param>
        protected void FilterDialog(
            InOrderPos inOrderPos,
            OutOrderPos outOrderPos,
            ProdOrderPartslistPos prodOrderPartslistPos,
            FacilityLot facilityLot,
            PickingPos pickingPos,
            LabOrder labOrder,
            PAOrderInfo orderInfo)
        {
            if (AccessPrimary == null)
                return;
            FacilityBooking fBooking = null;
            DeliveryNotePos dnPos = null;
            if (orderInfo != null && inOrderPos == null && outOrderPos == null && prodOrderPartslistPos == null && facilityLot == null && labOrder == null)
            {
                if (!LabOrderManager.ResolveEntities(DatabaseApp, orderInfo, out prodOrderPartslistPos, out dnPos, out pickingPos, out fBooking, out inOrderPos, out outOrderPos, out labOrder))
                    return;
            }

            if (inOrderPos != null)
            {
                ACFilterItem filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "OutOrderPos\\OutOrderPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "ProdOrderPartslistPos\\ProdOrderPartslistPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "LabOrderID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "PickingPos\\PickingPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                _IsLabOrderPosViewDialog = true;
                ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "InOrderPos\\InOrderPosID").FirstOrDefault();
                if (filterItem == null)
                {
                    filterItem = new ACFilterItem(Global.FilterTypes.filter, "InOrderPos\\InOrderPosID", Global.LogicalOperators.equal, Global.Operators.and, inOrderPos.InOrderPosID.ToString(), false);
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);

                }
                else
                    filterItem.SearchWord = inOrderPos.InOrderPosID.ToString();
                this.Search();
            }
            else if (outOrderPos != null)
            {
                ACFilterItem filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "InOrderPos\\InOrderPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "ProdOrderPartslistPos\\ProdOrderPartslistPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "LabOrderID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "PickingPos\\PickingPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);

                _IsLabOrderPosViewDialog = true;
                ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "OutOrderPos\\OutOrderPosID").FirstOrDefault();
                if (filterItem == null)
                {
                    filterItem = new ACFilterItem(Global.FilterTypes.filter, "OutOrderPos\\OutOrderPosID", Global.LogicalOperators.equal, Global.Operators.and, outOrderPos.OutOrderPosID.ToString(), false);
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);
                }
                else
                    filterItem.SearchWord = outOrderPos.OutOrderPosID.ToString();
                this.Search();
            }
            else if (prodOrderPartslistPos != null)
            {
                ACFilterItem filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "InOrderPos\\InOrderPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "OutOrderPos\\OutOrderPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "LabOrderID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "PickingPos\\PickingPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                _IsLabOrderPosViewDialog = true;
                ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "ProdOrderPartslistPos\\ProdOrderPartslistPosID").FirstOrDefault();
                if (filterItem == null)
                {
                    filterItem = new ACFilterItem(Global.FilterTypes.filter, "ProdOrderPartslistPos\\ProdOrderPartslistPosID", Global.LogicalOperators.equal, Global.Operators.and, prodOrderPartslistPos.ProdOrderPartslistPosID.ToString(), false);
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);
                }
                else
                    filterItem.SearchWord = prodOrderPartslistPos.ProdOrderPartslistPosID.ToString();
                this.Search();
            }
            else if (pickingPos != null)
            {
                ACFilterItem filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "InOrderPos\\InOrderPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "OutOrderPos\\OutOrderPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "ProdOrderPartslistPos\\ProdOrderPartslistPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits); filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "LabOrderID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);

                _IsLabOrderPosViewDialog = true;
                ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "PickingPos\\PickingPosID").FirstOrDefault();
                if (filterItem == null)
                {
                    filterItem = new ACFilterItem(Global.FilterTypes.filter, "PickingPos\\PickingPosID", Global.LogicalOperators.equal, Global.Operators.and, pickingPos.PickingPosID.ToString(), false);
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);
                }
                else
                    filterItem.SearchWord = prodOrderPartslistPos.ProdOrderPartslistPosID.ToString();
                this.Search();
            }
            else if (labOrder != null)
            {
                ACFilterItem filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "InOrderPos\\InOrderPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "OutOrderPos\\OutOrderPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "ProdOrderPartslistPos\\ProdOrderPartslistPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "PickingPos\\PickingPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);

                _IsLabOrderPosViewDialog = true;
                ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "LabOrderID").FirstOrDefault();
                if (filterItem == null)
                {
                    filterItem = new ACFilterItem(Global.FilterTypes.filter, "LabOrderID", Global.LogicalOperators.equal, Global.Operators.and, labOrder.LabOrderID.ToString(), false);
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);
                }
                else
                    filterItem.SearchWord = labOrder.LabOrderID.ToString();
                this.Search();
            }
            else
            {
                ACFilterItem filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "InOrderPos\\InOrderPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "OutOrderPos\\OutOrderPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "ProdOrderPartslistPos\\ProdOrderPartslistPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "LabOrderID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "PickingPos\\PickingPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
            }
        }

        /// <summary>
        /// Closes the laboratory order view dialog and performs necessary cleanup operations.
        /// This method saves any pending changes, closes the top dialog, refreshes the laboratory order
        /// positions list to reflect any modifications, and updates the template information display.
        /// It is typically called when the user finishes viewing or editing laboratory orders in the
        /// modal view dialog and wants to return to the main laboratory order management interface.
        /// </summary>
        [ACMethodInteraction("Dialog", "en{'Close'}de{'Schließen'}", 705, true, "CloseLabOrderViewDialog", Global.ACKinds.MSMethodPrePost,
                             Description = @"Closes the laboratory order view dialog and performs necessary cleanup operations.
                                             This method saves any pending changes, closes the top dialog, refreshes the laboratory order
                                             positions list to reflect any modifications, and updates the template information display.
                                             It is typically called when the user finishes viewing or editing laboratory orders in the
                                             modal view dialog and wants to return to the main laboratory order management interface.")]
        public void CloseLabOrderViewDialog()
        {
            Save();
            CloseTopDialog();
            base.OnPropertyChanged(nameof(LabOrderPosList));
            ChangeLabOrderTemplateNoName();
        }

        /// <summary>
        /// Determines whether a new laboratory order position can be created.
        /// This method evaluates if the user interface should enable the "New Lab Order Position" functionality
        /// based on the current state of the laboratory order and any active dialog modes.
        /// Returns true if a new laboratory order position can be added; otherwise false.
        /// </summary>
        /// <returns>True if new laboratory order positions can be created; otherwise false</returns>
        public override bool IsEnabledNewLabOrderPos()
        {
            //if (_IsLabOrderPosViewDialog)
            //    return false;
            return base.IsEnabledNewLabOrderPos();
        }

        /// <summary>
        /// Determines whether the delete laboratory order position functionality should be enabled.
        /// This method evaluates if the user interface should allow deletion of laboratory order positions
        /// based on the current state of the laboratory order view dialog and other system conditions.
        /// When the laboratory order position view dialog is active, deletion is disabled to prevent
        /// modifications during view-only operations.
        /// </summary>
        /// <returns>True if laboratory order positions can be deleted; otherwise false</returns>
        public override bool IsEnabledDeleteLabOrderPos()
        {
            if (_IsLabOrderPosViewDialog)
                return false;
            return base.IsEnabledDeleteLabOrderPos();
        }

        /// <summary>
        /// Updates the laboratory order template information and refreshes the related display properties.
        /// This method retrieves the template laboratory order that the current laboratory order is based on
        /// and updates the template number and name properties for display purposes. If no current laboratory
        /// order exists, the template information is cleared. This method is typically called when the
        /// current laboratory order selection changes to ensure the template information is synchronized
        /// with the user interface.
        /// </summary>
        public void ChangeLabOrderTemplateNoName()
        {
            if (CurrentLabOrder != null)
                _LabOrderTemplate = DatabaseApp.LabOrder.FirstOrDefault(c => c.LabOrderID == CurrentLabOrder.BasedOnTemplateID && c.LabOrderTypeIndex == (short)GlobalApp.LabOrderType.Template);
            else
                _LabOrderTemplate = null;
            OnPropertyChanged(nameof(LabOrderTemplateNo));
            OnPropertyChanged(nameof(LabOrderTemplateName));
        }

        /// <summary>
        /// Creates a new laboratory order by opening the laboratory order creation dialog.
        /// This method overrides the base New() method to provide specialized laboratory order creation functionality.
        /// Instead of creating a basic new record, it launches the NewLabOrderDialog to allow users to select
        /// the order position type and configure laboratory order parameters before creation.
        /// The dialog ensures proper association with order entities and template selection for comprehensive
        /// laboratory order setup with appropriate test parameters and measurement tags.
        /// </summary>
        public override void New()
        {
            NewLabOrderDialog(null, null, null, null, null);
        }

        /// <summary>
        /// Executes a search operation for laboratory orders with optimized query performance based on date range filtering.
        /// This method overrides the base Search functionality to implement dynamic result limiting based on the sample taking date filter.
        /// When the sample taking date range is considered wide (more than 3 days), it limits results to 500 records to maintain performance.
        /// For narrower date ranges, it limits results to 50 records for faster response times.
        /// After executing the search, it clears and refreshes the laboratory order position average statistics cache.
        /// </summary>
        [ACMethodCommand("LabOrder", "en{'Search'}de{'Suchen'}", (short)MISort.Search,
                         Description = @"Executes a search operation for laboratory orders with optimized query performance based on date range filtering.
                                         This method overrides the base Search functionality to implement dynamic result limiting based on the sample taking date filter.
                                         When the sample taking date range is considered wide (more than 3 days), it limits results to 500 records to maintain performance.
                                         For narrower date ranges, it limits results to 50 records for faster response times.
                                         After executing the search, it clears and refreshes the laboratory order position average statistics cache.")]
        public override void Search()
        {
            if (IsFilterSampleTakingDateWideRange)
                AccessPrimary.NavACQueryDefinition.TakeCount = 500;
            else
                AccessPrimary.NavACQueryDefinition.TakeCount = 50;
            base.Search();

            _LabOrderPosAVGList = null;
            OnPropertyChanged(nameof(LabOrderPosAVGList));
        }
        #endregion

        #region LabOrderPosItemAVG

        private LabOrderPos _SelectedLabOrderPosItemAVG;
        /// <summary>
        /// Gets or sets the currently selected laboratory order position item for average value calculations.
        /// This property represents the selected item from the LabOrderPosItemAVGList which contains 
        /// aggregated average values of laboratory test parameters grouped by lab tag for the current laboratory order.
        /// It is used to display and interact with individual averaged test parameter results in the user interface.
        /// </summary>
        [ACPropertySelected(720, "LabOrderPosItemAVG", "en{'TODO: PropertyName'}de{'TODO: PropertyName'}",
                            Description = @"Gets or sets the currently selected laboratory order position item for average value calculations.
                                            This property represents the selected item from the LabOrderPosItemAVGList which contains 
                                            aggregated average values of laboratory test parameters grouped by lab tag for the current laboratory order.
                                            It is used to display and interact with individual averaged test parameter results in the user interface.")]
        public LabOrderPos SelectedLabOrderPosItemAVG
        {
            get
            {
                return _SelectedLabOrderPosItemAVG;
            }
            set
            {
                if (_SelectedLabOrderPosItemAVG != value)
                {
                    _SelectedLabOrderPosItemAVG = value;
                    OnPropertyChanged(nameof(SelectedLabOrderPosItemAVG));
                }
            }
        }

        private IEnumerable<LabOrderPos> _LabOrderPosItemAVGList;
        /// <summary>
        /// Gets the list of laboratory order positions with averaged values grouped by laboratory tag for the current laboratory order.
        /// This property provides aggregated average values of test parameters from all positions in the current laboratory order,
        /// grouped by MDLabTag. Each item in the list represents a synthetic LabOrderPos with calculated averages for
        /// ReferenceValue, ActualValue, ValueMax, ValueMaxMax, ValueMin, and ValueMinMin properties.
        /// The list is cached after first access and automatically refreshed when the current laboratory order changes.
        /// </summary>
        [ACPropertyList(721, "LabOrderPosItemAVG", 
                        Description = @"Gets the list of laboratory order positions with averaged values grouped by laboratory tag for the current laboratory order.
                                        This property provides aggregated average values of test parameters from all positions in the current laboratory order,
                                        grouped by MDLabTag. Each item in the list represents a synthetic LabOrderPos with calculated averages for
                                        ReferenceValue, ActualValue, ValueMax, ValueMaxMax, ValueMin, and ValueMinMin properties.
                                        The list is cached after first access and automatically refreshed when the current laboratory order changes.")]
        public IEnumerable<LabOrderPos> LabOrderPosItemAVGList
        {
            get
            {
                if (_LabOrderPosItemAVGList == null)
                {
                    LoadLabOrderPosItemAVG();
                }
                return _LabOrderPosItemAVGList;
            }
        }

        private void LoadLabOrderPosItemAVG()
        {

            if (CurrentLabOrder != null && CurrentLabOrder.LabOrderPos_LabOrder.Any())
            {
                _LabOrderPosItemAVGList =
                    CurrentLabOrder
                    .LabOrderPos_LabOrder
                    .GroupBy(x => new { MDKey = x.MDLabTag.MDKey, MDNameTrans = x.MDLabTag.MDNameTrans })
                    .OrderBy(x => x.Key.MDKey)
                    .Select(x => new LabOrderPos
                    {
                        LabOrderPosID = Guid.NewGuid(),
                        MDLabTag = new MDLabTag() { MDKey = x.Key.MDKey, MDNameTrans = x.Key.MDNameTrans },
                        ReferenceValue = x.Average(p => p.ReferenceValue ?? 0),
                        ActualValue = x.Average(p => p.ActualValue ?? 0),
                        ValueMax = x.Average(p => p.ValueMax ?? 0),
                        ValueMaxMax = x.Average(p => p.ValueMaxMax ?? 0),
                        ValueMin = x.Average(p => p.ValueMin ?? 0),
                        ValueMinMin = x.Average(p => p.ValueMinMin ?? 0)
                    });
                SelectedLabOrderPosItemAVG = _LabOrderPosItemAVGList.FirstOrDefault();
            }
            else
            {
                SelectedLabOrderPosItemAVG = null;
            }
        }

        #endregion

        #region LabOrderPosAVG

        private LabOrderPos _SelectedLabOrderPosAVG;
        /// <summary>
        /// Gets or sets the currently selected laboratory order position for aggregate statistical analysis.
        /// This property represents the selected item from the LabOrderPosAVGList which contains 
        /// calculated average values of all laboratory test parameters across all laboratory orders
        /// in the current search result set, grouped by laboratory tag (MDLabTag). It is used to
        /// display and interact with statistical summaries of test parameter results across multiple
        /// laboratory orders for trend analysis and quality control oversight.
        /// </summary>
        [ACPropertySelected(722, "LabOrderPosAVG", "en{'TODO: PropertyName'}de{'TODO: PropertyName'}",
                            Description = @"Gets or sets the currently selected laboratory order position for aggregate statistical analysis.
                                            This property represents the selected item from the LabOrderPosAVGList which contains 
                                            calculated average values of all laboratory test parameters across all laboratory orders
                                            in the current search result set, grouped by laboratory tag (MDLabTag). It is used to
                                            display and interact with statistical summaries of test parameter results across multiple
                                            laboratory orders for trend analysis and quality control oversight.")]
        public LabOrderPos SelectedLabOrderPosAVG
        {
            get
            {
                return _SelectedLabOrderPosAVG;
            }
            set
            {
                if (_SelectedLabOrderPosAVG != value)
                {
                    _SelectedLabOrderPosAVG = value;
                    OnPropertyChanged(nameof(SelectedLabOrderPosAVG));
                }
            }
        }

        private IEnumerable<LabOrderPos> _LabOrderPosAVGList;
        /// <summary>
        /// Gets the list of laboratory order positions with averaged values across all laboratory orders in the current search result set.
        /// This property provides aggregated statistical data by grouping all laboratory order positions from all laboratory orders
        /// in the current LabOrderList by their laboratory tag (MDLabTag). For each laboratory tag, it calculates the average values
        /// of ReferenceValue, ActualValue, ValueMax, ValueMaxMax, ValueMin, and ValueMinMin across all positions.
        /// The list is used for statistical analysis and quality control oversight to identify trends and patterns
        /// in laboratory test results across multiple laboratory orders. The list is cached after first access
        /// and automatically refreshed when the laboratory order search results change.
        /// </summary>
        [ACPropertyList(723, "LabOrderPosAVG", 
                        Description = @"Gets the list of laboratory order positions with averaged values across all laboratory orders in the current search result set.
                                        This property provides aggregated statistical data by grouping all laboratory order positions from all laboratory orders
                                        in the current LabOrderList by their laboratory tag (MDLabTag). For each laboratory tag, it calculates the average values
                                        of ReferenceValue, ActualValue, ValueMax, ValueMaxMax, ValueMin, and ValueMinMin across all positions.
                                        The list is used for statistical analysis and quality control oversight to identify trends and patterns
                                        in laboratory test results across multiple laboratory orders. The list is cached after first access
                                        and automatically refreshed when the laboratory order search results change.")]
        public IEnumerable<LabOrderPos> LabOrderPosAVGList
        {
            get
            {
                if (_LabOrderPosAVGList == null)
                {
                    LoadLabOrderPosAVGList();
                }
                return _LabOrderPosAVGList;
            }
        }

        private void LoadLabOrderPosAVGList()
        {

            if (LabOrderList != null && LabOrderList.Any())
            {
                _LabOrderPosAVGList =
                    LabOrderList
                    .SelectMany(x => x.LabOrderPos_LabOrder)
                    .GroupBy(x => new { MDKey = x.MDLabTag.MDKey, MDNameTrans = x.MDLabTag.MDNameTrans })
                    .OrderBy(x => x.Key.MDKey)
                    .Select(x => new LabOrderPos
                    {
                        LabOrderPosID = Guid.NewGuid(),
                        MDLabTag = new MDLabTag() { MDKey = x.Key.MDKey, MDNameTrans = x.Key.MDNameTrans },
                        ReferenceValue = x.Average(p => p.ReferenceValue ?? 0),
                        ActualValue = x.Average(p => p.ActualValue ?? 0),
                        ValueMax = x.Average(p => p.ValueMax ?? 0),
                        ValueMaxMax = x.Average(p => p.ValueMaxMax ?? 0),
                        ValueMin = x.Average(p => p.ValueMin ?? 0),
                        ValueMinMin = x.Average(p => p.ValueMinMin ?? 0)
                    });
                SelectedLabOrderPosAVG = _LabOrderPosAVGList.FirstOrDefault();
            }
            else
            {
                SelectedLabOrderPosAVG = null;
            }
        }

        #endregion

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(NewLabOrderDialog):
                    result = NewLabOrderDialog((gip.mes.datamodel.DeliveryNotePos)acParameter[0], (gip.mes.datamodel.DeliveryNotePos)acParameter[1], (gip.mes.datamodel.ProdOrderPartslistPos)acParameter[2], (gip.mes.datamodel.FacilityLot)acParameter[3], (gip.mes.datamodel.PickingPos)acParameter[4]);
                    return true;
                case nameof(DialogCreatePos):
                    DialogCreatePos();
                    return true;
                case nameof(DialogCancelPos):
                    DialogCancelPos();
                    return true;
                case nameof(ShowLabOrderViewDialog):
                    ShowLabOrderViewDialog((gip.mes.datamodel.InOrderPos)acParameter[0], (gip.mes.datamodel.OutOrderPos)acParameter[1], (gip.mes.datamodel.ProdOrderPartslistPos)acParameter[2], (gip.mes.datamodel.FacilityLot)acParameter[3], (gip.mes.datamodel.PickingPos)acParameter[4], (gip.mes.datamodel.LabOrder)acParameter[5], (System.Boolean)acParameter[6], (gip.core.autocomponent.PAOrderInfo)acParameter[7]);
                    return true;
                case nameof(CloseLabOrderViewDialog):
                    CloseLabOrderViewDialog();
                    return true;
                case nameof(IsEnabledNewLabOrderPos):
                    result = IsEnabledNewLabOrderPos();
                    return true;
                case nameof(IsEnabledDeleteLabOrderPos):
                    result = IsEnabledDeleteLabOrderPos();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}