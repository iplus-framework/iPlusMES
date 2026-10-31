// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
// ***********************************************************************
// Assembly         : gip.bso.facility
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOFacilityChargeOverview.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.bso.masterdata;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.facility
{
    /// <summary>
    /// Businessobject or App for the stock overview of quants (FacilityCharge). A quant is the smallest stock unit: one material in one storage bin belonging to one lot/batch.
    /// This BSO replaces the former stock overview masks (article/charge totals per storage location and storage bin). It is a read-only overview: quants are created, changed and consumed exclusively by stock bookings (FacilityBookingManager). Therefore the BSO has no New() or Delete() method.
    /// To search records set one or more filter properties (FilterMaterial, FilterFacility, FilterLot, FilterExternLotNo, FilterExternLotNo2, FilterExpirationDate, ShowNotAvailable) and call Search(). Every change of a filter property automatically executes Search() again.
    /// The database result is copied to the FacilityChargeList property.
    /// Then call the NavigateFirst() method to set CurrentFacilityCharge with the first record in the list.
    /// CurrentFacilityCharge is used to display the currently selected quant; SelectedFacilityCharge holds the row selected in the list.
    /// The Load method updates the CurrentFacilityCharge object with fresh database data if another user has made changes in the background.
    /// If properties of CurrentFacilityCharge were changed, the Save() method writes the changes to the database and UndoSave() discards them.
    /// The expiration date filter can be set with the methods SetFilterExpirationDateToday(), SetFilterExpirationDateNextDays() (period taken from the ExpirationDateDayPeriod configuration, default 30 days) and ResetFilterExpirationDate().
    /// The stock of the current quant can be aggregated with FacilityChargeSumLocationHelperList (grouped by storage location), FacilityChargeSumFacilityHelperList (grouped by storage bin) and FacilityChargeSumMaterialHelperList (grouped by material).
    /// The booking history of the current quant is loaded with RefreshMovements() into FacilityBookingOverviewList and FacilityBookingChargeOverviewList for the period SearchFrom/SearchTo, optionally filtered by FilterFBType (posting type).
    /// With the IsIncludeDisabled configuration set to true, quants flagged as NotAvailable are included in the search (default: only available quants).
    /// Navigation from the selected quant: NavigateToOrder() (Show Order), NavigateToFacilityCharge() (Manage Quant), NavigateToFacilityLotOverview() (Show Lot Stock and History), NavigateToFacilityLot() (Manage Lot/Batch), NavigateToMaterialOverview() (Show Material Stock and History), NavigateToMaterial() (Manage Material), NavigateToFacilityOverview() (Show Bin Stock and History) and NavigateToFacility() (Manage Stock of Bin, only for storage bin containers).
    /// Other applications can open this BSO for a specific quant with ShowDialogOrderInfo(PAOrderInfo); the list is then prefiltered and the quant preselected.
    /// Child BSOs: BSOTandTFastView_Child (tracking and tracing fast view) and BSOFacilityReservationOverview_Child (picking and production order reservations).
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityChargeOverview&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Quant Overview'}de{'Quantübersicht'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + FacilityCharge.ClassName,
        Description = @"Businessobject or App for the stock overview of quants (FacilityCharge). A quant is the smallest stock unit: one material in one storage bin belonging to one lot/batch.
        This BSO replaces the former stock overview masks (article/charge totals per storage location and storage bin). It is a read-only overview: quants are created, changed and consumed exclusively by stock bookings (FacilityBookingManager). Therefore the BSO has no New() or Delete() method.
        To search records set one or more filter properties (FilterMaterial, FilterFacility, FilterLot, FilterExternLotNo, FilterExternLotNo2, FilterExpirationDate, ShowNotAvailable) and call Search(). Every change of a filter property automatically executes Search() again.
        The database result is copied to the FacilityChargeList property.
        Then call the NavigateFirst() method to set CurrentFacilityCharge with the first record in the list.
        CurrentFacilityCharge is used to display the currently selected quant; SelectedFacilityCharge holds the row selected in the list.
        The Load method updates the CurrentFacilityCharge object with fresh database data if another user has made changes in the background.
        If properties of CurrentFacilityCharge were changed, the Save() method writes the changes to the database and UndoSave() discards them.
        The expiration date filter can be set with the methods SetFilterExpirationDateToday(), SetFilterExpirationDateNextDays() (period taken from the ExpirationDateDayPeriod configuration, default 30 days) and ResetFilterExpirationDate().
        The stock of the current quant can be aggregated with FacilityChargeSumLocationHelperList (grouped by storage location), FacilityChargeSumFacilityHelperList (grouped by storage bin) and FacilityChargeSumMaterialHelperList (grouped by material).
        The booking history of the current quant is loaded with RefreshMovements() into FacilityBookingOverviewList and FacilityBookingChargeOverviewList for the period SearchFrom/SearchTo, optionally filtered by FilterFBType (posting type).
        With the IsIncludeDisabled configuration set to true, quants flagged as NotAvailable are included in the search (default: only available quants).
        Navigation from the selected quant: NavigateToOrder() (Show Order), NavigateToFacilityCharge() (Manage Quant), NavigateToFacilityLotOverview() (Show Lot Stock and History), NavigateToFacilityLot() (Manage Lot/Batch), NavigateToMaterialOverview() (Show Material Stock and History), NavigateToMaterial() (Manage Material), NavigateToFacilityOverview() (Show Bin Stock and History) and NavigateToFacility() (Manage Stock of Bin, only for storage bin containers).
        Other applications can open this BSO for a specific quant with ShowDialogOrderInfo(PAOrderInfo); the list is then prefiltered and the quant preselected.
        Child BSOs: BSOTandTFastView_Child (tracking and tracing fast view) and BSOFacilityReservationOverview_Child (picking and production order reservations).
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityChargeOverview&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOFacilityChargeOverview : BSOFacilityOverviewBase
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityChargeOverview"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityChargeOverview(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _ExpirationDateDayPeriod = new ACPropertyConfigValue<int>(this, "ExpirationDateDayPeriod", 30);
            _IsIncludeDisabled = new ACPropertyConfigValue<bool>(this, nameof(IsIncludeDisabled), false);
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
            _ = BSOTandTFastView_Child;
            Search();

            _ = ExpirationDateDayPeriod;
            _ = IsIncludeDisabled;

            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            this._CurrentFacilityChargeSumFacilityHelper = null;
            this._CurrentFacilityChargeSumMaterialHelper = null;
            this._SelectedFacilityChargeSumFacilityHelper = null;
            this._SelectedFacilityChargeSumLocationHelper = null;
            this._SelectedFacilityChargeSumMaterialHelper = null;
            var b = await base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                await _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }

        #endregion

        #region Configuration

        private ACPropertyConfigValue<int> _ExpirationDateDayPeriod;
        [ACPropertyConfig("en{'Expiration date filter period'}de{'Ablaufdatum Filterperiode'}", Description = "Configurable period in days for the 'Next [0] days' expiration date filter (default 30).")]
        public int ExpirationDateDayPeriod
        {
            get
            {
                return _ExpirationDateDayPeriod.ValueT;
            }
            set
            {
                if (_ExpirationDateDayPeriod.ValueT != value)
                {
                    _ExpirationDateDayPeriod.ValueT = value;
                    OnPropertyChanged(nameof(ExpirationDateDayPeriod));
                }
            }
        }

        private ACPropertyConfigValue<bool> _IsIncludeDisabled;
        [ACPropertyConfig("en{'Disable include'}de{'Include deaktivieren'}", Description = "If true, quants flagged as NotAvailable are included in the search; by default only available quants are shown.")]
        public bool IsIncludeDisabled
        {
            get
            {
                return _IsIncludeDisabled.ValueT;
            }
            set
            {
                if (_IsIncludeDisabled.ValueT != value)
                {
                    _IsIncludeDisabled.ValueT = value;
                    OnPropertyChanged();
                }
            }
        }


        #endregion

        #region Child (Local BSOs)

        ACChildItem<BSOTandTFastView> _BSOTandTFastView_Child;
        [ACPropertyInfo(590, Description = "Embedded child BSO for the tracking and tracing fast view of the current quant.")]
        [ACChildInfo("BSOTandTFastView_Child", typeof(BSOTandTFastView))]
        public ACChildItem<BSOTandTFastView> BSOTandTFastView_Child
        {
            get
            {
                if (_BSOTandTFastView_Child == null)
                    _BSOTandTFastView_Child = new ACChildItem<BSOTandTFastView>(this, "BSOTandTFastView_Child");
                return _BSOTandTFastView_Child;
            }
        }

        ACChildItem<BSOFacilityReservationOverview> _BSOFacilityReservationOverview_Child;
        [ACPropertyInfo(600, Description = "Embedded child BSO that shows picking and production order reservations for the current quant.")]
        [ACChildInfo(nameof(BSOFacilityReservationOverview_Child), typeof(BSOFacilityReservationOverview))]
        public ACChildItem<BSOFacilityReservationOverview> BSOFacilityReservationOverview_Child
        {
            get
            {
                if (_BSOFacilityReservationOverview_Child == null)
                    _BSOFacilityReservationOverview_Child = new ACChildItem<BSOFacilityReservationOverview>(this, nameof(BSOFacilityReservationOverview_Child));
                return _BSOFacilityReservationOverview_Child;
            }
        }

        #endregion

        #region BSO->ACProperty

        #region Filters
        public override bool? ShowNotAvailable
        {
            get
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CNotAvailableProperty);
                if (String.IsNullOrEmpty(tmp))
                    return null;
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<bool>(_CNotAvailableProperty);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CNotAvailableProperty);
                if (!value.HasValue)
                {
                    if (String.IsNullOrEmpty(tmp))
                        return;
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CNotAvailableProperty, "");
                    OnPropertyChanged("ShowNotAvailable");
                    return;
                }
                if (String.IsNullOrEmpty(tmp)
                    || AccessPrimary.NavACQueryDefinition.GetSearchValue<bool>(_CNotAvailableProperty) != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<bool>(_CNotAvailableProperty, value.Value);
                    OnPropertyChanged("ShowNotAvailable");
                    return;
                }
            }
        }


        public const string _CMaterialNoProperty = Material.ClassName + "\\MaterialNo";
        public const string _CMaterialNameProperty = Material.ClassName + "\\MaterialName1";
        [ACPropertyInfo(804, "Filter", "en{'Material'}de{'Material'}", Description = "Filter for the material number or material name of the quant; triggers Search() on change.")]
        public string FilterMaterial
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CMaterialNoProperty);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CMaterialNoProperty);
                if (tmp != value)
                {
                    _AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CMaterialNoProperty, value);
                    _AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CMaterialNameProperty, value);
                    OnPropertyChanged("FilterMaterial");
                }
            }
        }

        public const string _CFacilityNoProperty = Facility.ClassName + "\\FacilityNo";
        public const string _CFacilityNameProperty = Facility.ClassName + "\\FacilityName";
        [ACPropertyInfo(805, "Filter", "en{'Storage'}de{'Lagerplatz'}", Description = "Filter for the storage bin number or name; triggers Search() on change.")]
        public string FilterFacility
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CFacilityNoProperty);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CFacilityNoProperty);
                if (tmp != value)
                {
                    _AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CFacilityNoProperty, value);
                    _AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CFacilityNameProperty, value);
                    OnPropertyChanged("FilterFacility");
                }
            }
        }


        public const string _CLotNoProperty = FacilityLot.ClassName + "\\LotNo";
        [ACPropertyInfo(806, "Filter", ConstApp.LotNo, Description = "Filter for the lot number of the quant; triggers Search() on change.")]
        public string FilterLot
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CLotNoProperty);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CLotNoProperty);
                if (tmp != value)
                {
                    _AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CLotNoProperty, value);
                    OnPropertyChanged(nameof(FilterLot));
                }
            }
        }

        public const string _CExternLotNoProperty = FacilityLot.ClassName + "\\" + nameof(FacilityLot.ExternLotNo);
        [ACPropertyInfo(816, "Filter", ConstApp.ExternLotNo, Description = "Filter for the first external lot number; triggers Search() on change.")]
        public string FilterExternLotNo
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CExternLotNoProperty);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CExternLotNoProperty);
                if (tmp != value)
                {
                    _AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CExternLotNoProperty, value);
                    OnPropertyChanged(nameof(FilterExternLotNo));
                }
            }
        }

        public const string _CExternLotNo2Property = FacilityLot.ClassName + "\\" + nameof(FacilityLot.ExternLotNo2);
        [ACPropertyInfo(817, "Filter", ConstApp.ExternLotNo2, Description = "Filter for the second external lot number; triggers Search() on change.")]
        public string FilterExternLotNo2
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CExternLotNo2Property);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CExternLotNo2Property);
                if (tmp != value)
                {
                    _AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CExternLotNo2Property, value);
                    OnPropertyChanged(nameof(FilterExternLotNo2));
                }
            }
        }

        [ACPropertyInfo(717, "Filter", "en{'Expiration date less then'}de{'Mindesthaltbarkeitsdatum dann'}", Description = "Filters quants whose expiration date is less than or equal to this date; triggers Search() on change.")]
        public DateTime? FilterExpirationDate
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<DateTime?>(nameof(FacilityCharge.ExpirationDate));
            }
            set
            {
                DateTime? tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<DateTime?>(nameof(FacilityCharge.ExpirationDate));
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<DateTime?>(nameof(FacilityCharge.ExpirationDate), value);
                    OnPropertyChanged(nameof(FilterExpirationDate));
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _SetFilterExpirationDateNextDaysCaption;
        [ACPropertyInfo(999, "SetFilterExpirationDateNextDaysCaption", "en{'TODO:FilterExpirationDateDayPeriod'}de{'TODO:FilterExpirationDateDayPeriod'}", Description = "Caption of the 'Next [0] days' button with [0] replaced by the ExpirationDateDayPeriod configuration value.")]
        public string SetFilterExpirationDateNextDaysCaption
        {
            get
            {
                if (_SetFilterExpirationDateNextDaysCaption == null)
                {

                    gip.core.datamodel.ACClassMethod method = this.ACType.ValueTypeACClass.Methods.Where(c => c.ACIdentifier == nameof(SetFilterExpirationDateNextDays)).FirstOrDefault();
                    if (method != null)
                    {
                        _SetFilterExpirationDateNextDaysCaption = method.ACCaption;
                        _SetFilterExpirationDateNextDaysCaption = _SetFilterExpirationDateNextDaysCaption.Replace("[0]", ExpirationDateDayPeriod.ToString());
                    }
                }
                return _SetFilterExpirationDateNextDaysCaption;
            }
        }

        #endregion

        #region Primary
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<FacilityCharge> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(890, FacilityCharge.ClassName, Description = "Primary navigation access to FacilityCharge; executes the search query and holds the result list, selection and current record.")]
        public ACAccessNav<FacilityCharge> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(NavigationqueryDefaultFilter, true, true);
                    }
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<FacilityCharge>(FacilityCharge.ClassName, this);
                    _AccessPrimary.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;
                }
                return _AccessPrimary;
            }
        }

        private IQueryable<FacilityCharge> _AccessPrimary_NavSearchExecuting(IQueryable<FacilityCharge> result)
        {
            IQueryable<FacilityCharge> query = result as IQueryable<FacilityCharge>;
            if (query != null && !IsIncludeDisabled)
            {
                query =
                query.Include(c => c.Facility)
                .Include(c => c.FacilityLot)
                .Include(c => c.Material)
                .Include(c => c.MDUnit)
                .Include(c => c.MDReleaseState)
                .Include(c => c.CompanyMaterial)
                .Include(c => c.CPartnerCompanyMaterial)
                .Include("Material.MaterialUnit_Material.ToMDUnit")
                //if (ShowNotAvailable.HasValue)
                //{
                //    query = query.Where(c => c.NotAvailable == ShowNotAvailable.Value) as IQueryable<FacilityCharge>;
                //}
                ;
            }
            return query;
        }

        private List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, _CNotAvailableProperty, Global.LogicalOperators.equal, Global.Operators.and, "false", true),

                    new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, _CMaterialNoProperty, Global.LogicalOperators.contains, Global.Operators.or, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, _CMaterialNameProperty, Global.LogicalOperators.contains, Global.Operators.or, "", true),
                    new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true),

                    new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, _CFacilityNoProperty, Global.LogicalOperators.contains, Global.Operators.or, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, _CFacilityNameProperty, Global.LogicalOperators.contains, Global.Operators.or, "", true),
                    new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true),

                    new ACFilterItem(Global.FilterTypes.filter, _CLotNoProperty, Global.LogicalOperators.contains, Global.Operators.and, "", true, true),
                    new ACFilterItem(Global.FilterTypes.filter, _CExternLotNoProperty, Global.LogicalOperators.contains, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, _CExternLotNo2Property, Global.LogicalOperators.contains, Global.Operators.and, "", true),

                    new ACFilterItem(Global.FilterTypes.filter, nameof(FacilityCharge.ExpirationDate), Global.LogicalOperators.lessThanOrEqual, Global.Operators.and, null, true)

                };
            }
        }


        /// <summary>
        /// Gets or sets the selected facility charge.
        /// </summary>
        /// <value>The selected facility charge.</value>
        [ACPropertySelected(801, FacilityCharge.ClassName, Description = "The quant row currently selected in the list.")]
        public FacilityCharge SelectedFacilityCharge
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                AccessPrimary.Selected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the current facility charge.
        /// </summary>
        /// <value>The current facility charge.</value>
        [ACPropertyCurrent(802, FacilityCharge.ClassName, Description = "The currently displayed quant. Read its properties to show details; changes are written with Save(). Setting it refreshes the sum helper lists, the reservation child BSO and the tracking fast view.")]
        public FacilityCharge CurrentFacilityCharge
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                AccessPrimary.Current = value;

                if (BSOTandTFastView_Child != null && BSOTandTFastView_Child.Value != null && BSOTandTFastView_Child.Value.FilterFacilityCharge != value)
                    BSOTandTFastView_Child.Value.SetFaciltiyCharge(value);

                OnPropertyChanged("CurrentFacilityCharge");
                CleanMovements();

                if(BSOFacilityReservationOverview_Child != null && BSOFacilityReservationOverview_Child.Value != null)
                {
                    BSOFacilityReservationOverview_Child.Value.LoadReservation(AccessPrimary.Current);
                }
                
                OnPropertyChanged("FacilityChargeSumLocationHelperList");
                OnPropertyChanged("FacilityChargeSumFacilityHelperList");
                OnPropertyChanged("FacilityChargeSumMaterialHelperList");
            }
        }

        private List<FacilityCharge> _FacilityChargeList;
        /// <summary>
        /// Gets the facility charge list.
        /// </summary>
        /// <value>The facility charge list.</value>
        [ACPropertyList(803, FacilityCharge.ClassName, Description = "Result list of the last search; all quants matching the current filters.")]
        public List<FacilityCharge> FacilityChargeList
        {
            get
            {
                return _FacilityChargeList;
            }
            set
            {
                _FacilityChargeList = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region SumHelper
        /// <summary>
        /// Gets or sets the current facility charge sum location helper.
        /// </summary>
        /// <value>The current facility charge sum location helper.</value>
        [ACPropertyCurrent(807, "FacilityChargeSumLocationHelper", Description = "Current aggregation row of the quant stock grouped by storage location.")]
        public FacilityChargeSumLocationHelper CurrentFacilityChargeSumLocationHelper
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the facility charge sum location helper list.
        /// </summary>
        /// <value>The facility charge sum location helper list.</value>
        [ACPropertyList(808, "FacilityChargeSumLocationHelper", Description = "Stock of the current quant aggregated by storage location.")]
        public IEnumerable<FacilityChargeSumLocationHelper> FacilityChargeSumLocationHelperList
        {
            get
            {
                if (CurrentFacilityCharge == null)
                    return null;
                return ACFacilityManager.GetFacilityChargeSumLocationHelperList(FacilityChargeList, new FacilityQueryFilter() { FacilityChargeID = CurrentFacilityCharge.FacilityChargeID });
            }
        }
        /// <summary>
        /// The _ selected facility charge sum location helper
        /// </summary>
        FacilityChargeSumLocationHelper _SelectedFacilityChargeSumLocationHelper;
        /// <summary>
        /// Gets or sets the selected facility charge sum location helper.
        /// </summary>
        /// <value>The selected facility charge sum location helper.</value>
        [ACPropertySelected(809, "FacilityChargeSumLocationHelper", Description = "Selected row in the storage location aggregation list.")]
        public FacilityChargeSumLocationHelper SelectedFacilityChargeSumLocationHelper
        {
            get
            {
                return _SelectedFacilityChargeSumLocationHelper;
            }
            set
            {
                _SelectedFacilityChargeSumLocationHelper = value;
                OnPropertyChanged("SelectedFacilityChargeSumLocationHelper");
            }
        }

        /// <summary>
        /// The _ current facility charge sum facility helper
        /// </summary>
        FacilityChargeSumFacilityHelper _CurrentFacilityChargeSumFacilityHelper;
        /// <summary>
        /// Gets or sets the current facility charge sum facility helper.
        /// </summary>
        /// <value>The current facility charge sum facility helper.</value>
        [ACPropertyCurrent(810, "FacilityChargeSumFacilityHelper", Description = "Current aggregation row of the quant stock grouped by storage bin.")]
        public FacilityChargeSumFacilityHelper CurrentFacilityChargeSumFacilityHelper
        {
            get
            {
                return _CurrentFacilityChargeSumFacilityHelper;
            }
            set
            {
                _CurrentFacilityChargeSumFacilityHelper = value;
                OnPropertyChanged("CurrentFacilityChargeSumFacilityHelper");
            }
        }

        /// <summary>
        /// Gets the facility charge sum facility helper list.
        /// </summary>
        /// <value>The facility charge sum facility helper list.</value>
        [ACPropertyList(811, "FacilityChargeSumFacilityHelper", Description = "Stock of the current quant aggregated by storage bin.")]
        public IEnumerable<FacilityChargeSumFacilityHelper> FacilityChargeSumFacilityHelperList
        {
            get
            {
                if (CurrentFacilityCharge == null)
                    return null;
                return ACFacilityManager.GetFacilityChargeSumFacilityHelperList(FacilityChargeList, new FacilityQueryFilter() { FacilityChargeID = CurrentFacilityCharge.FacilityChargeID });
            }
        }
        /// <summary>
        /// The _ selected facility charge sum facility helper
        /// </summary>
        FacilityChargeSumFacilityHelper _SelectedFacilityChargeSumFacilityHelper;
        /// <summary>
        /// Gets or sets the selected facility charge sum facility helper.
        /// </summary>
        /// <value>The selected facility charge sum facility helper.</value>
        [ACPropertySelected(812, "FacilityChargeSumFacilityHelper", Description = "Selected row in the storage bin aggregation list.")]
        public FacilityChargeSumFacilityHelper SelectedFacilityChargeSumFacilityHelper
        {
            get
            {
                return _SelectedFacilityChargeSumFacilityHelper;
            }
            set
            {
                _SelectedFacilityChargeSumFacilityHelper = value;
                OnPropertyChanged("SelectedFacilityChargeSumFacilityHelper");
            }
        }

        /// <summary>
        /// The _ current facility charge sum material helper
        /// </summary>
        FacilityChargeSumMaterialHelper _CurrentFacilityChargeSumMaterialHelper;
        /// <summary>
        /// Gets or sets the current facility charge sum material helper.
        /// </summary>
        /// <value>The current facility charge sum material helper.</value>
        [ACPropertyCurrent(813, "FacilityChargeSumMaterialHelper", Description = "Current aggregation row of the quant stock grouped by material.")]
        public FacilityChargeSumMaterialHelper CurrentFacilityChargeSumMaterialHelper
        {
            get
            {
                return _CurrentFacilityChargeSumMaterialHelper;
            }
            set
            {
                _CurrentFacilityChargeSumMaterialHelper = value;
                OnPropertyChanged("CurrentFacilityChargeSumMaterialHelper");
            }
        }

        /// <summary>
        /// Gets the facility charge sum material helper list.
        /// </summary>
        /// <value>The facility charge sum material helper list.</value>
        [ACPropertyList(814, "FacilityChargeSumMaterialHelper", Description = "Stock of the current quant aggregated by material.")]
        public IEnumerable<FacilityChargeSumMaterialHelper> FacilityChargeSumMaterialHelperList
        {
            get
            {
                if (CurrentFacilityCharge == null)
                    return null;
                return ACFacilityManager.GetFacilityChargeSumMaterialHelperList(FacilityChargeList, new FacilityQueryFilter() { FacilityChargeID = CurrentFacilityCharge.FacilityChargeID });
            }
        }
        /// <summary>
        /// The _ selected facility charge sum material helper
        /// </summary>
        FacilityChargeSumMaterialHelper _SelectedFacilityChargeSumMaterialHelper;
        /// <summary>
        /// Gets or sets the selected facility charge sum material helper.
        /// </summary>
        /// <value>The selected facility charge sum material helper.</value>
        [ACPropertySelected(815, "FacilityChargeSumMaterialHelper", Description = "Selected row in the material aggregation list.")]
        public FacilityChargeSumMaterialHelper SelectedFacilityChargeSumMaterialHelper
        {
            get
            {
                return _SelectedFacilityChargeSumMaterialHelper;
            }
            set
            {
                _SelectedFacilityChargeSumMaterialHelper = value;
                OnPropertyChanged("SelectedFacilityChargeSumMaterialHelper");
            }
        }
        #endregion

        #endregion

        #region BSO->ACMethod

        #region BSO->ACMethod->Save&Search

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = "Saves changes made on CurrentFacilityCharge to the database.")]
        public async Task Save()
        {
            await OnSave();
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
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = "Discards unsaved changes on CurrentFacilityCharge and restores the database state.")]
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

        /// <summary>
        /// Loads this instance.
        /// </summary>
        [ACMethodInteraction(FacilityCharge.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedFacilityCharge", Global.ACKinds.MSMethodPrePost, Description = "Reloads the current quant from the database; with requery=true cached values are reset first. Use if another user changed the quant in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            bool isNewDialog = CurrentFacilityCharge == null;
            if (requery)
                CurrentFacilityCharge?.ResetCachedValues();
            LoadEntity<FacilityCharge>(requery, () => SelectedFacilityCharge, () => CurrentFacilityCharge, c => CurrentFacilityCharge = c,
                        DatabaseApp.FacilityCharge
                        .Where(c => c.FacilityChargeID == SelectedFacilityCharge.FacilityChargeID));
            if (isNewDialog)
                CurrentFacilityCharge?.ResetCachedValues();
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return true;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = "Executes the search with the current filters and fills FacilityChargeList.")]
        public void Search()
        {
            if (AccessPrimary == null)
                return;

            _FacilityChargeList = null;
            if (AccessPrimary != null)
            {
                AccessPrimary.NavSearch(DatabaseApp, MergeOption.OverwriteChanges);
                _FacilityChargeList = AccessPrimary.NavList.ToList();

                // Lazy-Loading is sometimes faster:
                if (IsIncludeDisabled && _FacilityChargeList != null)
                {
                    foreach (var charge in _FacilityChargeList)
                    {
                        _ = charge.Facility;
                        _ = charge.FacilityLot;
                        _ = charge.Material.MaterialUnit_Material;
                        _ = charge.MDUnit;
                        _ = charge.MDReleaseState;
                        _ = charge.CompanyMaterial;
                        _ = charge.CPartnerCompanyMaterial;
                    }
                }
            }

            OnPropertyChanged(nameof(FacilityChargeList));
        }

        private bool _SearchFacilityChargeListInProgress;
        [ACMethodInteraction("Deposit", "en{'Filter'}de{'Filtern'}", (short)MISort.Search, true, "FacilityChargeList", Global.ACKinds.MSMethod, Description = "Re-executes the list search and refreshes FacilityChargeList; disabled while a search is already running.")]
        public virtual void SearchFacilityChargeList()
        {
            _SearchFacilityChargeListInProgress = true;
            AccessNav.NavSearch(MergeOption.OverwriteChanges);
            OnPropertyChanged("FacilityChargeList");
            _SearchFacilityChargeListInProgress = false;
        }

        public bool IsEnabledSearchFacilityChargeList()
        {
            return !_SearchFacilityChargeListInProgress;
        }

        public override void OnPropertyChanged([CallerMemberName] string name = "")
        {
            base.OnPropertyChanged(name);
            if (name == nameof(ShowNotAvailable)
                || name == nameof(FilterMaterial)
                || name == nameof(FilterFacility)
                || name == nameof(FilterLot)
                || name == nameof(FilterExternLotNo)
                || name == nameof(FilterExternLotNo2)
                || name == nameof(FilterExpirationDate))
            {
                Search();
            }
        }

        public override Msg FilterByOrderInfo(PAOrderInfo paOrderInfo)
        {
            if (paOrderInfo != null)
            {
                PAOrderInfoEntry entry = paOrderInfo.Entities.Where(c => c.EntityName == FacilityCharge.ClassName).FirstOrDefault();
                CurrentFacilityCharge =
                    DatabaseApp
                    .FacilityCharge
                    .Include(c => c.Facility)
                    .Include(c => c.FacilityLot)
                    .Include(c => c.Material)
                    .FirstOrDefault(c => c.FacilityChargeID == entry.EntityID);
                return null;
            }
            return new Msg() { MessageLevel = eMsgLevel.Error, Message = "" };
        }

        #endregion

        #region BSO->ACMethod Filter ExpirationDate

        /// <summary>
        /// Source Property: SetFilterExpirationDateToday
        /// </summary>
        [ACMethodInfo("SetFilterExpirationDate", "en{'Today'}de{'Heute'}", 999, Description = "Sets the expiration date filter to today's date.")]
        public void SetFilterExpirationDateToday()
        {
            if (!IsEnabledSetFilterExpirationDateToday())
                return;
            FilterExpirationDate = DateTime.Now.Date;
        }

        public bool IsEnabledSetFilterExpirationDateToday()
        {
            return true;
        }

        /// <summary>
        /// Source Property: SetFilterExpirationDateNextDays
        /// </summary>
        [ACMethodInfo("SetFilterExpirationDate", "en{'Next [0] days'}de{'Nächste [0] Tage'}", 999, Description = "Sets the expiration date filter to today plus the ExpirationDateDayPeriod configuration value in days.")]
        public void SetFilterExpirationDateNextDays()
        {
            if (!IsEnabledSetFilterExpirationDateNextDays())
                return;
            FilterExpirationDate = DateTime.Now.Date.AddDays(ExpirationDateDayPeriod);
        }

        public bool IsEnabledSetFilterExpirationDateNextDays()
        {
            return true;
        }

        /// <summary>
        /// Source Property: ResetFilterExpirationDate
        /// </summary>
        [ACMethodInfo("SetFilterExpirationDate", "en{'Reset'}de{'Umsetzen'}", 999, Description = "Clears the expiration date filter.")]
        public void ResetFilterExpirationDate()
        {
            if (!IsEnabledResetFilterExpirationDate())
                return;
            FilterExpirationDate = null;
        }

        public bool IsEnabledResetFilterExpirationDate()
        {
            return true;
        }

        #endregion

        #region Dialog Navigate

        [ACMethodInteraction("", ConstApp.ShowProdOrder, 781, true, nameof(SelectedFacilityCharge), Description = "Opens the production order dialog for the production order position that produced this quant (enabled if the quant has a final position).")]
        public async Task NavigateToOrder()
        {
            if (!IsEnabledNavigateToOrder())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(ProdOrderPartslistPos), SelectedFacilityCharge.FinalPositionFromFbc.ProdOrderPartslistPosID));
                await service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToOrder()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.FinalPositionFromFbc != null)
                return true;
            return false;
        }

        [ACMethodInteraction(nameof(NavigateToFacilityCharge), "en{'Manage Quant'}de{'Quant verwalten'}", 782, true, nameof(SelectedFacilityCharge), Description = "Opens the quant management dialog (BSOFacility) for the selected quant.")]
        public async Task NavigateToFacilityCharge()
        {
            if (!IsEnabledNavigateToFacilityCharge())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo() { DialogSelectInfo = 0 };
                info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityCharge), SelectedFacilityCharge.FacilityChargeID));
                await service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacilityCharge()
        {
            return SelectedFacilityCharge != null;
        }


        [ACMethodInteraction("", "en{'Show Lot Stock and History'}de{'Zeige Losbestand und Historie'}", 783, true, nameof(SelectedFacilityCharge), Description = "Opens the lot stock and history overview (BSOFacilityLotOverview) for the lot of the selected quant.")]
        public async Task NavigateToFacilityLotOverview()
        {
            if (!IsEnabledNavigateToFacilityLotOverview())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo() { DialogSelectInfo = 1 };
                info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityLot), SelectedFacilityCharge.FacilityLotID ?? Guid.Empty));
                await service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacilityLotOverview()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.FacilityLot != null)
                return true;
            return false;
        }

        [ACMethodInteraction("", "en{'Manage Lot/Batch'}de{'Verwalte Los/Charge'}", 784, true, nameof(SelectedFacilityCharge), Description = "Opens the lot management dialog (BSOFacilityLot) for the lot of the selected quant.")]
        public async Task NavigateToFacilityLot()
        {
            if (!IsEnabledNavigateToFacilityLot())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityLot), SelectedFacilityCharge.FacilityLotID ?? Guid.Empty));
                await service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacilityLot()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.FacilityLot != null)
                return true;
            return false;
        }


        [ACMethodInteraction("", "en{'Show Material Stock and History'}de{'Zeige Materialbestand und Historie'}", 785, true, nameof(SelectedFacilityCharge), Description = "Opens the material stock and history overview for the material of the selected quant.")]
        public async Task NavigateToMaterialOverview()
        {
            if (!IsEnabledNavigateToMaterialOverview())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo() { DialogSelectInfo = 1 };
                info.Entities.Add(new PAOrderInfoEntry(nameof(Material), SelectedFacilityCharge.MaterialID));
                await service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToMaterialOverview()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.Material != null)
                return true;
            return false;
        }

        [ACMethodInteraction("", "en{'Manage Material'}de{'Verwalte Material'}", 786, true, nameof(SelectedFacilityCharge), Description = "Opens the material master data dialog for the material of the selected quant.")]
        public async Task NavigateToMaterial()
        {
            if (!IsEnabledNavigateToMaterial())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(Material), SelectedFacilityCharge.MaterialID));
                await service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToMaterial()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.Material != null)
                return true;
            return false;
        }


        [ACMethodInteraction("", "en{'Show Bin Stock and History'}de{'Zeige Behälterbestand und Historie'}", 787, true, nameof(SelectedFacilityCharge), Description = "Opens the storage bin stock and history overview for the storage bin of the selected quant.")]
        public async Task NavigateToFacilityOverview()
        {
            if (!IsEnabledNavigateToFacilityOverview())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo() { DialogSelectInfo = 1 };
                info.Entities.Add(new PAOrderInfoEntry(nameof(Facility), SelectedFacilityCharge.FacilityID));
                await service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacilityOverview()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.Facility != null)
                return true;
            return false;
        }

        [ACMethodInteraction("", "en{'Manage Stock of Bin'}de{'Verwalte Behälterbestand'}", 788, true, nameof(SelectedFacilityCharge), Description = "Opens the storage bin management dialog for the storage bin of the selected quant (only enabled if the bin is a storage bin container).")]
        public async Task NavigateToFacility()
        {
            if (!IsEnabledNavigateToFacility())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(Facility), SelectedFacilityCharge.FacilityID));
                await service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacility()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.Facility != null && SelectedFacilityCharge.Facility.MDFacilityType != null && SelectedFacilityCharge.Facility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                return true;
            return false;
        }
        #endregion

        #region Show Dialog
        [ACMethodInfo("Dialog", "en{'Dialog lot overview'}de{'Dialog Losübersicht'}", (short)MISort.QueryPrintDlg + 1, Description = "Opens this BSO as a dialog for a specific quant passed in the PAOrderInfo: prefilters the list and preselects the quant. Used by other applications to jump to a quant.")]
        public virtual async Task ShowDialogOrderInfo(PAOrderInfo paOrderInfo)
        {
            if (AccessPrimary == null || paOrderInfo == null)
                return;
            DialogOrderInfoPrepareFilter(paOrderInfo);

            Search();

            DialogOrderInfoPreSelectCharge(paOrderInfo);

            await ShowDialogAsync(this, "ShowDlgOrderInfo");
            await this.ParentACComponent.StopComponent(this);
        }

        public virtual void DialogOrderInfoPrepareFilter(PAOrderInfo paOrderInfo)
        {
            PAOrderInfoEntry materialEntry = paOrderInfo.Entities.Where(c => c.EntityName == nameof(Material)).FirstOrDefault();
            PAOrderInfoEntry facilityLotEntry = paOrderInfo.Entities.Where(c => c.EntityName == nameof(FacilityLot)).FirstOrDefault();

            if (materialEntry != null)
            {
                Material material = DatabaseApp.Material.Where(c => c.MaterialID == materialEntry.EntityID).FirstOrDefault();
                AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CMaterialNoProperty, material.MaterialNo);
            }

            if (facilityLotEntry != null)
            {
                FacilityLot facilityLot = DatabaseApp.FacilityLot.Where(c => c.FacilityLotID == facilityLotEntry.EntityID).FirstOrDefault();
                AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CLotNoProperty, facilityLot.LotNo);
            }
        }

        public virtual void DialogOrderInfoPreSelectCharge(PAOrderInfo paOrderInfo)
        {
            PAOrderInfoEntry facilityChargeEntry = paOrderInfo.Entities.Where(c => c.EntityName == nameof(FacilityCharge)).FirstOrDefault();

            if (facilityChargeEntry != null)
            {
                FacilityCharge facilityCharge = DatabaseApp.FacilityCharge.Where(c => c.FacilityChargeID == facilityChargeEntry.EntityID).FirstOrDefault();
                if (FacilityChargeList == null)
                {
                    FacilityChargeList = new List<FacilityCharge>();
                }

                if (!FacilityChargeList.Contains(facilityCharge))
                {
                    FacilityChargeList.Add(facilityCharge);
                }

                SelectedFacilityCharge = facilityCharge;
                CurrentFacilityCharge = facilityCharge;

            }
        }
        #endregion

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(Save):
                    result = Save();
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
                case nameof(Load):
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
                case nameof(IsEnabledLoad):
                    result = IsEnabledLoad();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
                case nameof(SearchFacilityChargeList):
                    SearchFacilityChargeList();
                    return true;
                case nameof(IsEnabledSearchFacilityChargeList):
                    result = IsEnabledSearchFacilityChargeList();
                    return true;
                case nameof(SetFilterExpirationDateToday):
                    SetFilterExpirationDateToday();
                    return true;
                case nameof(IsEnabledSetFilterExpirationDateNextDays):
                    result = IsEnabledSetFilterExpirationDateNextDays();
                    return true;
                case nameof(ResetFilterExpirationDate):
                    ResetFilterExpirationDate();
                    return true;
                case nameof(IsEnabledResetFilterExpirationDate):
                    result = IsEnabledResetFilterExpirationDate();
                    return true;
                case nameof(NavigateToFacilityLotOverview):
                    result = NavigateToFacilityLotOverview();
                    return true;
                case nameof(IsEnabledNavigateToFacilityLotOverview):
                    result = IsEnabledNavigateToFacilityLotOverview();
                    return true;
                case nameof(NavigateToMaterialOverview):
                    result = NavigateToMaterialOverview();
                    return true;
                case nameof(IsEnabledNavigateToMaterialOverview):
                    result = IsEnabledNavigateToMaterialOverview();
                    return true;
                case nameof(NavigateToFacilityOverview):
                    result = NavigateToFacilityOverview();
                    return true;
                case nameof(IsEnabledNavigateToFacilityOverview):
                    result = IsEnabledNavigateToFacilityOverview();
                    return true;
                case nameof(NavigateToFacilityCharge):
                    result = NavigateToFacilityCharge();
                    return true;
                case nameof(IsEnabledNavigateToFacilityCharge):
                    result = IsEnabledNavigateToFacilityCharge();
                    return true;
                case nameof(ShowDialogOrderInfo):
                    result = ShowDialogOrderInfo((gip.core.autocomponent.PAOrderInfo)acParameter[0]);
                    return true;
                case nameof(NavigateToFacility):
                    result = NavigateToFacility();
                    return true;
                case nameof(IsEnabledNavigateToFacility):
                    result = IsEnabledNavigateToFacility();
                    return true;
                case nameof(NavigateToMaterial):
                    result = NavigateToMaterial();
                    return true;
                case nameof(IsEnabledNavigateToMaterial):
                    result = IsEnabledNavigateToMaterial();
                    return true;
                case nameof(NavigateToFacilityLot):
                    result = NavigateToFacilityLot();
                    return true;
                case nameof(IsEnabledNavigateToFacilityLot):
                    result = IsEnabledNavigateToFacilityLot();
                    return true;
                case nameof(NavigateToOrder):
                    result = NavigateToOrder();
                    return true;
                case nameof(IsEnabledNavigateToOrder):
                    result = IsEnabledNavigateToOrder();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(RefreshMovements):
                    return new[] { "CurrentFacilityCharge" };
                case nameof(UndoSave):
                case nameof(IsEnabledUndoSave):
                    return new[] { "CurrentFacilityCharge", "SelectedFacilityCharge", "IsDirty" };
                case nameof(Load):
                case nameof(IsEnabledLoad):
                    return new[] { "CurrentFacilityCharge", "SelectedFacilityCharge" };
                case nameof(Search):
                    return new[] { "CurrentFacilityCharge", "SelectedFacilityCharge" };
                case nameof(SearchFacilityChargeList):
                case nameof(IsEnabledSearchFacilityChargeList):
                    return new[] { "CurrentFacilityCharge", "SelectedFacilityCharge" };
                case nameof(SetFilterExpirationDateToday):
                case nameof(IsEnabledSetFilterExpirationDateNextDays):
                    return new[] { "CurrentFacilityCharge", "SelectedFacilityCharge" };
                case nameof(ResetFilterExpirationDate):
                case nameof(IsEnabledResetFilterExpirationDate):
                    return new[] { "CurrentFacilityCharge", "SelectedFacilityCharge" };
                case nameof(NavigateToFacilityLotOverview):
                case nameof(IsEnabledNavigateToFacilityLotOverview):
                    return new[] { "CurrentFacilityCharge", "SelectedFacilityCharge" };
                case nameof(NavigateToMaterialOverview):
                case nameof(IsEnabledNavigateToMaterialOverview):
                    return new[] { "CurrentFacilityCharge", "SelectedFacilityCharge" };
                case nameof(NavigateToFacilityOverview):
                case nameof(IsEnabledNavigateToFacilityOverview):
                    return new[] { "CurrentFacilityCharge", "SelectedFacilityCharge" };
                case nameof(NavigateToFacilityCharge):
                case nameof(IsEnabledNavigateToFacilityCharge):
                    return new[] { "CurrentFacilityCharge", "SelectedFacilityCharge" };
                case nameof(ShowDialogOrderInfo):
                    return new[] { "CurrentFacilityCharge", "SelectedFacilityCharge" };
                case nameof(NavigateToFacility):
                case nameof(IsEnabledNavigateToFacility):
                    return new[] { "CurrentFacilityCharge", "SelectedFacilityCharge" };
                case nameof(NavigateToMaterial):
                case nameof(IsEnabledNavigateToMaterial):
                    return new[] { "CurrentFacilityCharge", "SelectedFacilityCharge" };
                case nameof(NavigateToFacilityLot):
                case nameof(IsEnabledNavigateToFacilityLot):
                    return new[] { "CurrentFacilityCharge", "SelectedFacilityCharge" };
                case nameof(NavigateToOrder):
                case nameof(IsEnabledNavigateToOrder):
                    return new[] { "CurrentFacilityCharge", "SelectedFacilityCharge" };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion

        #region FacilityBooking(Charge)Overview methods -> Executive methods overrides

        public override PAOrderInfo GetOrderInfo()
        {
            PAOrderInfo pAOrderInfo = new PAOrderInfo();
            if (SelectedFacilityCharge != null)
                pAOrderInfo.Add(FacilityCharge.ClassName, SelectedFacilityCharge.FacilityChargeID);
            return pAOrderInfo;
        }

        public override bool IsEnabledRefreshMovements()
        {
            return base.IsEnabledRefreshMovements() && CurrentFacilityCharge != null;
        }

        public override FacilityQueryFilter GetFacilityBookingFilter()
        {
            FacilityQueryFilter filter = base.GetFacilityBookingFilter();
            if (CurrentFacilityCharge != null)
                filter.FacilityChargeID = CurrentFacilityCharge.FacilityChargeID;
            return filter;
        }

        #endregion

    }
}
