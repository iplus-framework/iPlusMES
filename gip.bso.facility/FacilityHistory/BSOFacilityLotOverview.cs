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
// <copyright file="BSOFacilityLotOverview.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.facility;
using System.Runtime.CompilerServices;
using gip.bso.masterdata;
using Microsoft.EntityFrameworkCore;

namespace gip.bso.facility
{
    /// <summary>
    /// Businessobject or App for the stock overview of lots/batches (FacilityLot).
    /// This is a read-only overview: lots are created by production and booking processes, therefore the BSO has no New() or Delete() method.
    /// To search records set one or more filter properties (FilterLotNo, FilterExternLotNo, FilterExternLotNo2, FilterMaterialNo, ShowNotAvailable) and call Search(). Every change of a filter property automatically executes Search() again. The result is sorted descending by lot number.
    /// The database result is copied to the FacilityLotList property.
    /// Then call the NavigateFirst() method to set CurrentFacilityLot with the first record in the list.
    /// CurrentFacilityLot is used to display the currently selected lot; SelectedFacilityLot holds the row selected in the list. Setting CurrentFacilityLot refreshes the quant list, the sum helper lists, the reservation child BSO and the tracking fast view.
    /// The quants of the current lot are provided in FacilityChargeList (SelectedFacilityCharge/CurrentFacilityCharge), aggregated in FacilityChargeSumFacilityHelperList (grouped by storage bin), FacilityChargeSumLocationHelperList (grouped by storage location) and FacilityChargeSumMaterialHelperList (grouped by material).
    /// The Load method updates the CurrentFacilityLot object with fresh database data including its stock if another user has made changes in the background.
    /// If properties of CurrentFacilityLot were changed (e.g. Lock, Comment), the Save() method writes the changes to the database and UndoSave() discards them.
    /// The booking history of the current lot is loaded with RefreshMovements() into FacilityBookingOverviewList and FacilityBookingChargeOverviewList for the period SearchFrom/SearchTo; OnFacilityBookingSearchSum() computes the running in/out sum (InOutSumUOM) for each booking row.
    /// Other applications can open this BSO for a specific lot with ShowDialogOrder(string lotNo) or ShowDialogOrderInfo(PAOrderInfo); the list is then prefiltered and the lot preselected.
    /// Navigation from the selected quant: NavigateToOrder() (Show Order), NavigateToFacilityCharge() (Manage Quant), NavigateToFacilityLot() (Manage Lot/Batch), NavigateToFacilityOverview() (Show Bin Stock and History) and NavigateToMaterialOverview() (Show Material Stock and History).
    /// Child BSOs: BSOTandTFastView_Child (tracking and tracing fast view) and BSOFacilityReservationOverview_Child (picking and production order reservations).
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityLotOverview&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Lot Overview'}de{'Losübersicht'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + FacilityLot.ClassName,
        Description = @"Businessobject or App for the stock overview of lots/batches (FacilityLot).
        This is a read-only overview: lots are created by production and booking processes, therefore the BSO has no New() or Delete() method.
        To search records set one or more filter properties (FilterLotNo, FilterExternLotNo, FilterExternLotNo2, FilterMaterialNo, ShowNotAvailable) and call Search(). Every change of a filter property automatically executes Search() again. The result is sorted descending by lot number.
        The database result is copied to the FacilityLotList property.
        Then call the NavigateFirst() method to set CurrentFacilityLot with the first record in the list.
        CurrentFacilityLot is used to display the currently selected lot; SelectedFacilityLot holds the row selected in the list. Setting CurrentFacilityLot refreshes the quant list, the sum helper lists, the reservation child BSO and the tracking fast view.
        The quants of the current lot are provided in FacilityChargeList (SelectedFacilityCharge/CurrentFacilityCharge), aggregated in FacilityChargeSumFacilityHelperList (grouped by storage bin), FacilityChargeSumLocationHelperList (grouped by storage location) and FacilityChargeSumMaterialHelperList (grouped by material).
        The Load method updates the CurrentFacilityLot object with fresh database data including its stock if another user has made changes in the background.
        If properties of CurrentFacilityLot were changed (e.g. Lock, Comment), the Save() method writes the changes to the database and UndoSave() discards them.
        The booking history of the current lot is loaded with RefreshMovements() into FacilityBookingOverviewList and FacilityBookingChargeOverviewList for the period SearchFrom/SearchTo; OnFacilityBookingSearchSum() computes the running in/out sum (InOutSumUOM) for each booking row.
        Other applications can open this BSO for a specific lot with ShowDialogOrder(string lotNo) or ShowDialogOrderInfo(PAOrderInfo); the list is then prefiltered and the lot preselected.
        Navigation from the selected quant: NavigateToOrder() (Show Order), NavigateToFacilityCharge() (Manage Quant), NavigateToFacilityLot() (Manage Lot/Batch), NavigateToFacilityOverview() (Show Bin Stock and History) and NavigateToMaterialOverview() (Show Material Stock and History).
        Child BSOs: BSOTandTFastView_Child (tracking and tracing fast view) and BSOFacilityReservationOverview_Child (picking and production order reservations).
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityLotOverview&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOFacilityLotOverview : BSOFacilityOverviewBase
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityLotOverview"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityLotOverview(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            if (!base.ACInit(startChildMode))
                return false;
            _ = BSOTandTFastView_Child;
            Search();
            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            this._CurrentFacilityCharge = null;
            this._CurrentFacilityChargeSumFacilityHelper = null;
            this._CurrentFacilityChargeSumLocationHelper = null;
            this._CurrentFacilityChargeSumMaterialHelper = null;
            this._SelectedFacilityCharge = null;
            this._SelectedFacilityChargeSumFacilityHelper = null;
            this._SelectedFacilityChargeSumLocationHelper = null;
            this._SelectedFacilityChargeSumMaterialHelper = null;

            if (_AccessPrimary != null)
                _AccessPrimary.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;

            var b = await base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                await _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }

        #endregion

        #region Child (Local BSOs)

        ACChildItem<BSOTandTFastView> _BSOTandTFastView_Child;
        [ACPropertyInfo(590, Description = "Embedded child BSO for the tracking and tracing fast view of the current lot.")]
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
        [ACPropertyInfo(600, Description = "Embedded child BSO that shows picking and production order reservations for the current lot.")]
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

        #region Properties

        #region Properties -> Filter

        /// <summary>
        /// 
        /// </summary>
        [ACPropertyInfo(999, nameof(FilterLotNo), ConstApp.LotNo, Description = "Filter for the lot number (contains); triggers Search() on change.")]
        public string FilterLotNo
        {
            get
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(nameof(FacilityLot.LotNo));
                return string.IsNullOrEmpty(tmp) ? null : tmp;
            }
            set
            {
                AccessPrimary.NavACQueryDefinition.SetSearchValue(nameof(FacilityLot.LotNo), Global.LogicalOperators.contains, value ?? "");
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [ACPropertyInfo(999, nameof(FilterExternLotNo), ConstApp.ExternLotNo, Description = "Filter for the first external lot number (contains); triggers Search() on change.")]
        public string FilterExternLotNo
        {
            get
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(nameof(FacilityLot.ExternLotNo));
                return string.IsNullOrEmpty(tmp) ? null : tmp;
            }
            set
            {
                AccessPrimary.NavACQueryDefinition.SetSearchValue(nameof(FacilityLot.ExternLotNo), Global.LogicalOperators.contains, value ?? "");
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [ACPropertyInfo(999, nameof(FilterExternLotNo2), ConstApp.ExternLotNo2, Description = "Filter for the second external lot number (contains); triggers Search() on change.")]
        public string FilterExternLotNo2
        {
            get
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(nameof(FacilityLot.ExternLotNo2));
                return string.IsNullOrEmpty(tmp) ? null : tmp;
            }
            set
            {
                AccessPrimary.NavACQueryDefinition.SetSearchValue(nameof(FacilityLot.ExternLotNo2), Global.LogicalOperators.contains, value ?? "");
                OnPropertyChanged();
            }
        }

        // Material\MaterialNo
        /// <summary>
        /// 
        /// </summary>
        [ACPropertyInfo(999, nameof(FilterMaterialNo), ConstApp.MaterialNo, Description = "Filter for the material number of the lot (contains); triggers Search() on change.")]
        public string FilterMaterialNo
        {
            get
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>($"{nameof(Material)}\\{nameof(Material.MaterialNo)}");
                return string.IsNullOrEmpty(tmp) ? null : tmp;
            }
            set
            {
                AccessPrimary.NavACQueryDefinition.SetSearchValue($"{nameof(Material)}\\{nameof(Material.MaterialNo)}", Global.LogicalOperators.contains, value ?? "");
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region BSO->ACProperty

        #region BSO->ACPropertyPrimary->FacilityLot
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<FacilityLot> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(890, FacilityLot.ClassName, Description = "Primary navigation access to FacilityLot; executes the search query and holds the result list, selection and current record.")]
        public ACAccessNav<FacilityLot> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceColumnsIfDifferent(NavigationqueryDefaultFilter, NavigationqueryDefaultSort);
                    }
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<FacilityLot>(FacilityLot.ClassName, this);
                    _AccessPrimary.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;
                }
                return _AccessPrimary;
            }
        }

        private List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, nameof(FacilityLot.LotNo), Global.LogicalOperators.contains, Global.Operators.and, null, false, true),
                    new ACFilterItem(Global.FilterTypes.filter, nameof(FacilityLot.ExternLotNo), Global.LogicalOperators.contains, Global.Operators.and, null, false),
                    new ACFilterItem(Global.FilterTypes.filter, nameof(FacilityLot.ExternLotNo2), Global.LogicalOperators.contains, Global.Operators.and, null, false),
                    new ACFilterItem(Global.FilterTypes.filter, $"{nameof(Material)}\\{nameof(Material.MaterialNo)}", Global.LogicalOperators.contains, Global.Operators.and, null, false)
                };
            }
        }

        private List<ACSortItem> NavigationqueryDefaultSort
        {
            get
            {
                return new List<ACSortItem>()
                {
                    new ACSortItem("LotNo", Global.SortDirections.descending, true)
                };
            }
        }


        /// <summary>
        /// Gets or sets the selected facility lot.
        /// </summary>
        /// <value>The selected facility lot.</value>
        [ACPropertySelected(801, nameof(FacilityLot), ConstApp.LotNo, Description = "The lot row currently selected in the list.")]
        public FacilityLot SelectedFacilityLot
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
                OnPropertyChanged("SelectedFacilityLot");
            }
        }

        /// <summary>
        /// Gets or sets the current facility lot.
        /// </summary>
        /// <value>The current facility lot.</value>
        [ACPropertyCurrent(802, nameof(FacilityLot), ConstApp.LotNo, Description = "The currently displayed lot. Setting it refreshes the quant list, the sum helper lists, the reservation child BSO and the tracking fast view.")]
        public FacilityLot CurrentFacilityLot
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Current as FacilityLot;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                AccessPrimary.Current = value;

                if (BSOTandTFastView_Child != null && BSOTandTFastView_Child.Value != null && BSOTandTFastView_Child.Value.FilterFacilityLot != value)
                    BSOTandTFastView_Child.Value.SetFaciltiyLot(value);

                OnPropertyChanged();
                CleanMovements();
                RefreshRelatedData();
            }
        }

        public override void OnPropertyChanged([CallerMemberName] string name = "")
        {
            base.OnPropertyChanged(name);

            if (name == nameof(ShowNotAvailable)
                || name == nameof(FilterMaterialNo)
                || name == nameof(FilterExternLotNo)
                || name == nameof(FilterExternLotNo2)
                || name == nameof(FilterLotNo))
            {
                Search();
                RefreshRelatedData();
            }
        }

        public void RefreshRelatedData()
        {
            RefreshFacilityChargeList();
            RefreshFacilityChargeSumMaterialHelperList();
            RefreshFacilityChargeSumFacilityHelperList();
            RefreshFacilityChargeSumLocationHelperList();
            if (BSOFacilityReservationOverview_Child != null && BSOFacilityReservationOverview_Child.Value != null)
            {
                BSOFacilityReservationOverview_Child.Value.LoadReservation(CurrentFacilityLot);
            }
        }

        private List<FacilityLot> _FacilityLotList;
        /// <summary>
        /// Gets the facility lot list.
        /// </summary>
        /// <value>The facility lot list.</value>
        [ACPropertyList(803, nameof(FacilityLot), ConstApp.LotNo, Description = "Result list of the last search; all lots matching the current filters, sorted descending by lot number.")]
        public List<FacilityLot> FacilityLotList
        {
            get
            {
                return _FacilityLotList;
            }
            set
            {
                _FacilityLotList = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region BSO->ACProperty->FacilityCharge

        /// <summary>
        /// The _ current facility charge
        /// </summary>
        FacilityCharge _CurrentFacilityCharge;
        /// <summary>
        /// Gets or sets the current facility charge.
        /// </summary>
        /// <value>The current facility charge.</value>
        [ACPropertyCurrent(804, FacilityCharge.ClassName, Description = "The quant of the current lot currently displayed in the quant list.")]
        public FacilityCharge CurrentFacilityCharge
        {
            get
            {
                return _CurrentFacilityCharge;
            }
            set
            {
                _CurrentFacilityCharge = value;
                OnPropertyChanged("CurrentFacilityCharge");
            }
        }

        /// <summary>
        /// The _ selected facility charge
        /// </summary>
        FacilityCharge _SelectedFacilityCharge;
        /// <summary>
        /// Gets or sets the selected facility charge.
        /// </summary>
        /// <value>The selected facility charge.</value>
        [ACPropertySelected(805, FacilityCharge.ClassName, Description = "The quant row currently selected in the quant list of the current lot.")]
        public FacilityCharge SelectedFacilityCharge
        {
            get
            {
                return _SelectedFacilityCharge;
            }
            set
            {
                _SelectedFacilityCharge = value;
                OnPropertyChanged("SelectedFacilityCharge");
            }
        }

        IEnumerable<FacilityCharge> _FacilityChargeList;
        /// <summary>
        /// Gets the facility charge list.
        /// </summary>
        /// <value>The facility charge list.</value>
        [ACPropertyList(806, FacilityCharge.ClassName, Description = "All quants of the current lot (lazy loaded from the database on first access).")]
        public IEnumerable<FacilityCharge> FacilityChargeList
        {
            get
            {
                if (_FacilityChargeList != null)
                    return _FacilityChargeList;
                if (CurrentFacilityLot == null)
                    return null;
                _FacilityChargeList = FacilityManager.s_cQry_LotOverviewFacilityCharge(this.DatabaseApp, CurrentFacilityLot.FacilityLotID, ShowNotAvailable).ToArray();
                return _FacilityChargeList;
            }
        }

        private void RefreshFacilityChargeList()
        {
            CurrentFacilityCharge = null;
            SelectedFacilityCharge = null;
            _FacilityChargeList = null;
            OnPropertyChanged("FacilityChargeList");
        }
        #endregion

        #region BSO->ACProperty->SumLocation
        /// <summary>
        /// SumLocation: A sum over all Charges/Batches of the actual Lot grouped by Storage Location
        /// </summary>
        FacilityChargeSumLocationHelper _CurrentFacilityChargeSumLocationHelper;
        /// <summary>
        /// Gets or sets the current facility charge sum location helper.
        /// </summary>
        /// <value>The current facility charge sum location helper.</value>
        [ACPropertyCurrent(807, "FacilityChargeSumLocationHelper", Description = "Current aggregation row of the lot stock grouped by storage location.")]
        public FacilityChargeSumLocationHelper CurrentFacilityChargeSumLocationHelper
        {
            get
            {
                return _CurrentFacilityChargeSumLocationHelper;
            }
            set
            {
                _CurrentFacilityChargeSumLocationHelper = value;
                OnPropertyChanged("CurrentFacilityChargeSumLocationHelper");
            }
        }

        /// <summary>
        /// Gets the facility charge sum location helper list.
        /// </summary>
        /// <value>The facility charge sum location helper list.</value>
        [ACPropertyList(808, "FacilityChargeSumLocationHelper", Description = "Stock of all quants of the current lot aggregated by storage location.")]
        public IEnumerable<FacilityChargeSumLocationHelper> FacilityChargeSumLocationHelperList
        {
            get
            {
                if (CurrentFacilityLot == null)
                    return null;
                return ACFacilityManager.GetFacilityChargeSumLocationHelperList(FacilityChargeList, new FacilityQueryFilter() { FacilityLotID = CurrentFacilityLot.FacilityLotID });
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

        private void RefreshFacilityChargeSumLocationHelperList()
        {
            CurrentFacilityChargeSumLocationHelper = null;
            SelectedFacilityChargeSumLocationHelper = null;
            OnPropertyChanged("FacilityChargeSumLocationHelperList");
        }

        #endregion

        #region BSO->ACProperty->SumFacility
        /// <summary>
        /// SumLocation: A sum over all Charges/Batches of the actual Lot grouped by Storage Area
        /// </summary>
        FacilityChargeSumFacilityHelper _CurrentFacilityChargeSumFacilityHelper;
        /// <summary>
        /// Gets or sets the current facility charge sum facility helper.
        /// </summary>
        /// <value>The current facility charge sum facility helper.</value>
        [ACPropertyCurrent(810, "FacilityChargeSumFacilityHelper", Description = "Current aggregation row of the lot stock grouped by storage bin.")]
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
        [ACPropertyList(811, "FacilityChargeSumFacilityHelper", Description = "Stock of all quants of the current lot aggregated by storage bin.")]
        public IEnumerable<FacilityChargeSumFacilityHelper> FacilityChargeSumFacilityHelperList
        {
            get
            {
                if (CurrentFacilityLot == null)
                    return null;

                return ACFacilityManager.GetFacilityChargeSumFacilityHelperList(FacilityChargeList, new FacilityQueryFilter() { FacilityLotID = CurrentFacilityLot.FacilityLotID });
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

        private void RefreshFacilityChargeSumFacilityHelperList()
        {
            CurrentFacilityChargeSumFacilityHelper = null;
            SelectedFacilityChargeSumFacilityHelper = null;
            OnPropertyChanged("FacilityChargeSumFacilityHelperList");
        }

        #endregion

        #region BSO->ACProperty->SumMaterial
        /// <summary>
        /// SumLocation: A sum over all Charges/Batches of the actual Lot grouped by Storage Area
        /// </summary>
        FacilityChargeSumMaterialHelper _CurrentFacilityChargeSumMaterialHelper;
        /// <summary>
        /// Gets or sets the current facility charge sum material helper.
        /// </summary>
        /// <value>The current facility charge sum material helper.</value>
        [ACPropertyCurrent(813, "FacilityChargeSumMaterialHelper", Description = "Current aggregation row of the lot stock grouped by material.")]
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
        [ACPropertyList(814, "FacilityChargeSumMaterialHelper", Description = "Stock of all quants of the current lot aggregated by material.")]
        public IEnumerable<FacilityChargeSumMaterialHelper> FacilityChargeSumMaterialHelperList
        {
            get
            {
                if (CurrentFacilityLot == null)
                    return null;

                return ACFacilityManager.GetFacilityChargeSumMaterialHelperList(FacilityChargeList, new FacilityQueryFilter() { FacilityLotID = CurrentFacilityLot.FacilityLotID });
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

        private void RefreshFacilityChargeSumMaterialHelperList()
        {
            CurrentFacilityChargeSumMaterialHelper = null;
            SelectedFacilityChargeSumMaterialHelper = null;
            OnPropertyChanged("FacilityChargeSumMaterialHelperList");
        }
        #endregion

        #endregion

        #region BSO->ACMethod

        #region BSO->ACMethod->Save&Search

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(FacilityLot.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = "Saves changes made on CurrentFacilityLot (e.g. Lock, Comment) to the database.")]
        public async Task Save()
        {
            await OnSave();
        }

        protected override void OnPostSave()
        {
            ACState = Const.SMSearch;
            PostExecute("Save");
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
        [ACMethodCommand(FacilityLot.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = "Discards unsaved changes on CurrentFacilityLot and restores the database state.")]
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
        [ACMethodInteraction(FacilityLot.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedFacilityLot", Description = "Reloads the current lot from the database including its stock; with requery=true the stock is refreshed first. Use if another user changed the lot in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<FacilityLot>(requery, () => SelectedFacilityLot, () => CurrentFacilityLot, c => CurrentFacilityLot = c,
                        DatabaseApp.FacilityLot
                        .Include("FacilityLotStock_FacilityLot")
                        .Where(c => c.FacilityLotID == SelectedFacilityLot.FacilityLotID));
            if (CurrentFacilityLot != null && CurrentFacilityLot.CurrentFacilityLotStock != null)
                CurrentFacilityLot.CurrentFacilityLotStock.AutoRefresh(DatabaseApp);
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
        [ACMethodCommand(FacilityLot.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = "Executes the search with the current filters and fills FacilityLotList.")]
        public void Search()
        {
            if (AccessPrimary == null)
                return;

            _FacilityLotList = null;
            if (AccessPrimary != null)
            {
                AccessPrimary.NavSearch(DatabaseApp, MergeOption.OverwriteChanges);
                _FacilityLotList = AccessPrimary.NavList.ToList();
            }

            OnPropertyChanged(nameof(FacilityLotList));
        }

        IQueryable<FacilityLot> _AccessPrimary_NavSearchExecuting(IQueryable<FacilityLot> result)
        {
            IQueryable<FacilityLot> query = result as IQueryable<FacilityLot>;
            if (query != null)
            {
                query.Include("FacilityLotStock_FacilityLot");
                if (ShowNotAvailable.HasValue)
                {
                    query = query.Where(c => c.FacilityCharge_FacilityLot.Any(x => x.NotAvailable == ShowNotAvailable.Value)) as IQueryable<FacilityLot>;
                }
            }
            return query;
        }

        #endregion

        #region BSO->ACMethod->DialogOrderInfo

        [ACMethodInfo("Dialog", "en{'Dialog lot overview'}de{'Dialog Losübersicht'}", (short)MISort.QueryPrintDlg + 1, Description = "Opens this BSO as a dialog for the lot passed in the PAOrderInfo (prefilters by lot number). Used by other applications to jump to a lot.")]
        public async Task ShowDialogOrderInfo(PAOrderInfo paOrderInfo)
        {
            if (AccessPrimary == null || paOrderInfo == null)
                return;

            // Falls Produktionsauftrag
            FacilityLot facilityLot = null;
            foreach (var entry in paOrderInfo.Entities)
            {
                if (entry.EntityName == FacilityLot.ClassName)
                {
                    facilityLot = this.DatabaseApp.FacilityLot
                        .Where(c => c.FacilityLotID == entry.EntityID)
                        .FirstOrDefault();
                }
            }

            if (facilityLot == null)
                return;

            await ShowDialogOrder(facilityLot.LotNo);
        }

        [ACMethodInfo("Dialog", "en{'Dialog lot overview'}de{'Dialog Losübersicht'}", (short)MISort.QueryPrintDlg, Description = "Opens this BSO as a dialog prefiltered for the given lot number and preselects the lot.")]
        public async Task ShowDialogOrder(string lotNo)
        {
            if (AccessPrimary == null)
                return;
            
            ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == nameof(FacilityLot.LotNo)).FirstOrDefault();
            if (filterItem == null)
            {
                filterItem = new ACFilterItem(Global.FilterTypes.filter, nameof(FacilityLot.LotNo), Global.LogicalOperators.contains, Global.Operators.and, lotNo, false);
                AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);
            }
            filterItem.SearchWord = lotNo;

            this.Search();
            await ShowDialogAsync(this, "DisplayOrderDialog");
            await this.ParentACComponent.StopComponent(this);
        }

        #endregion

        #region Navigation
        [ACMethodInteraction("", ConstApp.ShowProdOrder, 780, true, nameof(SelectedFacilityCharge), Description = "Opens the production order dialog for the production order position that produced the selected quant (enabled if the quant has a final position).")]
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

        [ACMethodInteraction(nameof(NavigateToFacilityCharge), "en{'Manage Quant'}de{'Quant verwalten'}", 781, true, nameof(SelectedFacilityCharge), Description = "Opens the quant management dialog (BSOFacility) for the selected quant, passing quant, storage bin, material and lot.")]
        public async Task NavigateToFacilityCharge()
        {
            if (!IsEnabledNavigateToFacilityCharge())
            {
                return;
            }

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityCharge), SelectedFacilityCharge.FacilityChargeID));
                info.Entities.Add(new PAOrderInfoEntry(nameof(Facility), SelectedFacilityCharge.FacilityID));
                info.Entities.Add(new PAOrderInfoEntry(nameof(Material), SelectedFacilityCharge.MaterialID));

                if (SelectedFacilityCharge.FacilityLotID != null)
                {
                    info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityLot), SelectedFacilityCharge.FacilityLotID ?? Guid.Empty));
                }

                await service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacilityCharge()
        {
            return SelectedFacilityCharge != null;
        }

        [ACMethodInteraction("", "en{'Manage Lot/Batch'}de{'Verwalte Los/Charge'}", 781, true, nameof(SelectedFacilityLot), Description = "Opens the lot management dialog (BSOFacilityLot) for the selected lot.")]
        public async Task NavigateToFacilityLot()
        {
            if (!IsEnabledNavigateToFacilityLot())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityLot), SelectedFacilityLot.FacilityLotID));
                await service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacilityLot()
        {
            if (SelectedFacilityLot != null)
                return true;
            return false;
        }

        [ACMethodInteraction("", "en{'Show Bin Stock and History'}de{'Zeige Behälterbestand und Historie'}", 782, true, nameof(SelectedFacilityCharge), Description = "Opens the storage bin stock and history overview for the storage bin of the selected quant.")]
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
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.Facility != null && SelectedFacilityCharge.Facility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                return true;
            return false;
        }

        [ACMethodInteraction("", "en{'Manage Stock of Bin'}de{'Verwalte Behälterbestand'}", 783, true, nameof(SelectedFacilityCharge), Description = "Opens the storage bin management dialog for the storage bin of the selected quant (only enabled if the bin is a storage bin container).")]
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

        [ACMethodInteraction("", "en{'Show Material Stock and History'}de{'Zeige Materialbestand und Historie'}", 784, true, nameof(SelectedFacilityCharge), Description = "Opens the material stock and history overview for the material of the selected quant.")]
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
                case nameof(ShowDialogOrderInfo):
                    result = ShowDialogOrderInfo((PAOrderInfo)acParameter[0]);
                    return true;
                case nameof(ShowDialogOrder):
                    result = ShowDialogOrder((String)acParameter[0]);
                    return true;
                case nameof(NavigateToFacility):
                    result = NavigateToFacility();
                    return true;
                case nameof(IsEnabledNavigateToFacility):
                    result = IsEnabledNavigateToFacility();
                    return true;
                case nameof(NavigateToFacilityCharge):
                    result = NavigateToFacilityCharge();
                    return true;
                case nameof(IsEnabledNavigateToFacilityCharge):
                    result = IsEnabledNavigateToFacilityCharge();
                    return true;
                case nameof(NavigateToFacilityOverview):
                    result = NavigateToFacilityOverview();
                    return true;
                case nameof(IsEnabledNavigateToFacilityOverview):
                    result = IsEnabledNavigateToFacilityOverview();
                    return true;
                case nameof(NavigateToMaterialOverview):
                    result = NavigateToMaterialOverview();
                    return true;
                case nameof(IsEnabledNavigateToMaterialOverview):
                    result = IsEnabledNavigateToMaterialOverview();
                    return true;
                case nameof(NavigateToFacilityLot):
                    result = NavigateToFacilityLot();
                    return true;
                case nameof(IsEnabledNavigateToFacilityLot):
                    result = IsEnabledNavigateToFacilityLot();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(RefreshMovements):
                    return new[] { nameof(CurrentFacilityLot) };
                case nameof(Load):
                    return new[] { nameof(CurrentFacilityCharge), nameof(SelectedFacilityCharge) };
                case nameof(Search):
                    return new[] { nameof(CurrentFacilityCharge), nameof(SelectedFacilityCharge) };
                case nameof(ShowDialogOrderInfo):
                    return new[] { nameof(CurrentFacilityCharge), nameof(SelectedFacilityCharge) };
                case nameof(ShowDialogOrder):
                    return new[] { nameof(CurrentFacilityCharge), nameof(SelectedFacilityCharge) };
                case nameof(NavigateToFacility):
                    return new[] { nameof(CurrentFacilityCharge), nameof(SelectedFacilityCharge) };
                case nameof(NavigateToFacilityCharge):
                    return new[] { nameof(CurrentFacilityCharge), nameof(SelectedFacilityCharge) };
                case nameof(NavigateToFacilityOverview):
                    return new[] { nameof(CurrentFacilityCharge), nameof(SelectedFacilityCharge) };
                case nameof(NavigateToMaterialOverview):
                    return new[] { nameof(CurrentFacilityCharge), nameof(SelectedFacilityCharge) };
                case nameof(NavigateToFacilityLot):
                    return new[] { nameof(CurrentFacilityCharge), nameof(SelectedFacilityCharge) };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion

        #region FacilityBooking(Charge)Overview methods -> Executive methods overrides

        public override bool IsEnabledRefreshMovements()
        {
            return base.IsEnabledRefreshMovements() && CurrentFacilityLot != null;
        }

        public override FacilityQueryFilter GetFacilityBookingFilter()
        {
            FacilityQueryFilter filter = base.GetFacilityBookingFilter();
            if (CurrentFacilityLot != null)
                filter.FacilityLotID = CurrentFacilityLot.FacilityLotID;
            return filter;
        }

        public override void OnFacilityBookingSearchSum()
        {
            if (FacilityBookingOverviewList != null && CurrentFacilityLot != null)
            {
                double sum = 0.0;
                foreach (var fb in FacilityBookingOverviewList)
                {
                    if (String.IsNullOrEmpty(fb.InwardFacilityChargeLotNo) || fb.InwardFacilityChargeLotNo == CurrentFacilityLot.LotNo)
                        sum += fb.InwardQuantityUOM;
                    if (String.IsNullOrEmpty(fb.OutwardFacilityChargeLotNo) || fb.OutwardFacilityChargeLotNo == CurrentFacilityLot.LotNo)
                        sum -= fb.OutwardQuantityUOM;
                    fb.InOutSumUOM = sum;
                }
            }

            if (FacilityBookingChargeOverviewList != null && CurrentFacilityLot != null)
            {
                double sum = 0.0;
                foreach (var fb in FacilityBookingChargeOverviewList)
                {
                    if (fb.InwardFacilityChargeLotNo == CurrentFacilityLot.LotNo)
                        sum += fb.InwardQuantityUOM;
                    if (fb.OutwardFacilityChargeLotNo == CurrentFacilityLot.LotNo)
                        sum -= fb.OutwardQuantityUOM;
                    fb.InOutSumUOM = sum;
                }
            }
        }

        public override bool IsEnabledShowFacilityLot()
        {
            return false;
        }

        #endregion

    }
}
