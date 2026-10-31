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
// <copyright file="BSOFacilityLocationOverview.cs" company="gip mbh, Oftersheim, Germany">
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
using Microsoft.EntityFrameworkCore;

namespace gip.bso.facility
{
    /// <summary>
    /// Businessobject or App for the stock overview of storage locations (Facility of type "Storage location").
    /// This app replaces the legacy stock overview masks (storage location stocks of materials, charges and material charges).
    /// ALL stock bookings are always executed only via the FacilityBookingManager. This BSO is a read-only overview; it does not create, change or delete stock.
    /// To search for records, enter the search string in the SearchWord property.
    /// The database result is copied to the FacilityLocationList property. Only storage locations (FacilityTypesEnum.StorageLocation) are returned.
    /// Then call the NavigateFirst() method to set CurrentFacilityLocation with the first record in the list.
    /// CurrentFacilityLocation is used to display the currently selected storage location.
    /// The FacilityChargeList property contains the quants stored in the current storage location (across all its storage bins).
    /// The FacilityChargeSumMaterialHelperList and FacilityChargeSumLotHelperList properties contain the stock totals per material and per lot.
    /// The ShowNotAvailable property (base class BSOFacilityOverviewBase) controls whether not available quants are included; changing it refreshes the lists.
    /// The FacilityBookingOverviewList and FacilityBookingChargeOverviewList properties (base class BSOFacilityOverviewBase) contain the inward/outward movements of the current storage location in the period SearchFrom to SearchTo.
    /// Call the RefreshMovements() method to reload the movements (executed in the background); the running sum per row is written to the InOutSumUOM property.
    /// The Save(), UndoSave() and Load() methods follow the common BSO pattern; the overview itself provides no New() or Delete() method.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityLocationOverview&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Storage Location Overview'}de{'Lagerort Übersicht'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Facility.ClassName,
        Description = @"Businessobject or App for the stock overview of storage locations (Facility of type ""Storage location"").
        This app replaces the legacy stock overview masks (storage location stocks of materials, charges and material charges).
        ALL stock bookings are always executed only via the FacilityBookingManager. This BSO is a read-only overview; it does not create, change or delete stock.
        To search for records, enter the search string in the SearchWord property.
        The database result is copied to the FacilityLocationList property. Only storage locations (FacilityTypesEnum.StorageLocation) are returned.
        Then call the NavigateFirst() method to set CurrentFacilityLocation with the first record in the list.
        CurrentFacilityLocation is used to display the currently selected storage location.
        The FacilityChargeList property contains the quants stored in the current storage location (across all its storage bins).
        The FacilityChargeSumMaterialHelperList and FacilityChargeSumLotHelperList properties contain the stock totals per material and per lot.
        The ShowNotAvailable property (base class BSOFacilityOverviewBase) controls whether not available quants are included; changing it refreshes the lists.
        The FacilityBookingOverviewList and FacilityBookingChargeOverviewList properties (base class BSOFacilityOverviewBase) contain the inward/outward movements of the current storage location in the period SearchFrom to SearchTo.
        Call the RefreshMovements() method to reload the movements (executed in the background); the running sum per row is written to the InOutSumUOM property.
        The Save(), UndoSave() and Load() methods follow the common BSO pattern; the overview itself provides no New() or Delete() method.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityLocationOverview&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOFacilityLocationOverview : BSOFacilityOverviewBase
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityLocationOverview"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityLocationOverview(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            Search();
            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            this._CurrentFacilityCharge = null;
            this._CurrentFacilityChargeSumLotHelper = null;
            this._CurrentFacilityChargeSumMaterialHelper = null;
            this._SelectedFacilityCharge = null;
            this._SelectedFacilityChargeSumLotHelper = null;
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

        #region BSO->ACProperty

        #region Primary
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<Facility> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(890, "FacilityLocation", "en{'Storage location'}de{'Lagerort'}",
            Description = "Navigation access to the storage locations; source of FacilityLocationList, SelectedFacilityLocation and CurrentFacilityLocation.")]
        public ACAccessNav<Facility> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(NavigationqueryDefaultFilter);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<Facility>("FacilityLocation", this);
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
                    new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, "FacilityNo", Global.LogicalOperators.contains, Global.Operators.or, null, true, true),
                    new ACFilterItem(Global.FilterTypes.filter, "FacilityName", Global.LogicalOperators.contains, Global.Operators.or, null, true, true),
                    new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, ((short)FacilityTypesEnum.StorageLocation).ToString(), true),
                };
            }
        }


        /// <summary>
        /// Gets or sets the selected facility location.
        /// </summary>
        /// <value>The selected facility location.</value>
        [ACPropertySelected(801, "FacilityLocation", "en{'Storage location'}de{'Lagerort'}",
            Description = "Storage location selected in the list.")]
        public Facility SelectedFacilityLocation
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
                OnPropertyChanged("SelectedFacilityLocation");
            }
        }

        /// <summary>
        /// Gets or sets the current facility location.
        /// </summary>
        /// <value>The current facility location.</value>
        [ACPropertyCurrent(802, "FacilityLocation", "en{'Storage location'}de{'Lagerort'}",
            Description = "Currently displayed storage location; changing it refreshes the stock and movement lists.")]
        public Facility CurrentFacilityLocation
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
                OnPropertyChanged("CurrentFacilityLocation");
                CleanMovements();
                RefreshRelatedData();
            }
        }

        public override void OnPropertyChanged([CallerMemberName] string name = "")
        {
            base.OnPropertyChanged(name);
            if (name == "ShowNotAvailable")
            {
                RefreshRelatedData();
            }
        }

        public void RefreshRelatedData()
        {
            RefreshFacilityChargeList();
            RefreshFacilityChargeSumMaterialHelperList();
            RefreshFacilityChargeSumLotHelperList();
        }

        /// <summary>
        /// Gets the facility location list.
        /// </summary>
        /// <value>The facility location list.</value>
        [ACPropertyList(803, "FacilityLocation", "en{'Storage location'}de{'Lagerort'}",
            Description = "Result list of the search; contains only storage locations (FacilityTypesEnum.StorageLocation).")]
        public IEnumerable<Facility> FacilityLocationList
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.NavList;
            }
        }
        #endregion

        #region FacilityCharge


        /// <summary>
        /// The _ current facility charge
        /// </summary>
        FacilityCharge _CurrentFacilityCharge;
        /// <summary>
        /// Gets or sets the current facility charge.
        /// </summary>
        /// <value>The current facility charge.</value>
        [ACPropertyCurrent(804, FacilityCharge.ClassName, "en{'Quant'}de{'Quant'}",
            Description = "Currently displayed quant of the storage location.")]
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

        IEnumerable<FacilityCharge> _FacilityChargeList;
        /// <summary>
        /// Gets the facility charge list.
        /// </summary>
        /// <value>The facility charge list.</value>
        [ACPropertyList(805, FacilityCharge.ClassName, "en{'Quant'}de{'Quant'}",
            Description = "Quants stored in the current storage location (across all its storage bins).")]
        public IEnumerable<FacilityCharge> FacilityChargeList
        {
            get
            {
                if (_FacilityChargeList != null)
                    return _FacilityChargeList;
                if (CurrentFacilityLocation == null)
                    return null;
                _FacilityChargeList = FacilityManager.s_cQry_LocationOverviewFacilityCharge(this.DatabaseApp, CurrentFacilityLocation.FacilityID, ShowNotAvailable).ToArray();
                return _FacilityChargeList;
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
        [ACPropertySelected(806, FacilityCharge.ClassName, "en{'Quant'}de{'Quant'}",
            Description = "Quant selected in the stock list.")]
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

        private void RefreshFacilityChargeList()
        {
            CurrentFacilityCharge = null;
            SelectedFacilityCharge = null;
            _FacilityChargeList = null;
            OnPropertyChanged("FacilityChargeList");
        }

        #endregion

        #region SumHelper
        /// <summary>
        /// The _ current facility charge sum material helper
        /// </summary>
        FacilityChargeSumMaterialHelper _CurrentFacilityChargeSumMaterialHelper;
        /// <summary>
        /// Gets or sets the current facility charge sum material helper.
        /// </summary>
        /// <value>The current facility charge sum material helper.</value>
        [ACPropertyCurrent(807, "FacilityChargeSumMaterialHelper", "en{'Totals per material'}de{'Summen pro Material'}",
            Description = "Currently displayed totals row per material.")]
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
        [ACPropertyList(808, "FacilityChargeSumMaterialHelper", "en{'Totals per material'}de{'Summen pro Material'}",
            Description = "Stock totals of the current storage location grouped by material.")]
        public IEnumerable<FacilityChargeSumMaterialHelper> FacilityChargeSumMaterialHelperList
        {
            get
            {
                if (CurrentFacilityLocation == null)
                    return null;
                return ACFacilityManager.GetFacilityChargeSumMaterialHelperList(FacilityChargeList, new FacilityQueryFilter() { FacilityLocationID = CurrentFacilityLocation.FacilityID });
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
        [ACPropertySelected(809, "FacilityChargeSumMaterialHelper", "en{'Totals per material'}de{'Summen pro Material'}",
            Description = "Totals-per-material row selected in the list.")]
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


        /// <summary>
        /// The _ current facility charge sum lot helper
        /// </summary>
        FacilityChargeSumLotHelper _CurrentFacilityChargeSumLotHelper;
        /// <summary>
        /// Gets or sets the current facility charge sum lot helper.
        /// </summary>
        /// <value>The current facility charge sum lot helper.</value>
        [ACPropertyCurrent(810, "FacilityChargeSumLotHelper", "en{'Totals per lot'}de{'Summen pro Los'}",
            Description = "Currently displayed totals row per lot.")]
        public FacilityChargeSumLotHelper CurrentFacilityChargeSumLotHelper
        {
            get
            {
                return _CurrentFacilityChargeSumLotHelper;
            }
            set
            {
                _CurrentFacilityChargeSumLotHelper = value;
                OnPropertyChanged("CurrentFacilityChargeSumLotHelper");
            }
        }

        /// <summary>
        /// Gets the facility charge sum lot helper list.
        /// </summary>
        /// <value>The facility charge sum lot helper list.</value>
        [ACPropertyList(811, "FacilityChargeSumLotHelper", "en{'Totals per lot'}de{'Summen pro Los'}",
            Description = "Stock totals of the current storage location grouped by lot.")]
        public IEnumerable<FacilityChargeSumLotHelper> FacilityChargeSumLotHelperList
        {
            get
            {
                if (CurrentFacilityLocation == null)
                    return null;
                return ACFacilityManager.GetFacilityChargeSumLotHelperList(FacilityChargeList, new FacilityQueryFilter() { FacilityLocationID = CurrentFacilityLocation.FacilityID });
            }
        }
        /// <summary>
        /// The _ selected facility charge sum lot helper
        /// </summary>
        FacilityChargeSumLotHelper _SelectedFacilityChargeSumLotHelper;
        /// <summary>
        /// Gets or sets the selected facility charge sum lot helper.
        /// </summary>
        /// <value>The selected facility charge sum lot helper.</value>
        [ACPropertySelected(812, "FacilityChargeSumLotHelper", "en{'Totals per lot'}de{'Summen pro Los'}",
            Description = "Totals-per-lot row selected in the list.")]
        public FacilityChargeSumLotHelper SelectedFacilityChargeSumLotHelper
        {
            get
            {
                return _SelectedFacilityChargeSumLotHelper;
            }
            set
            {
                _SelectedFacilityChargeSumLotHelper = value;
                OnPropertyChanged("SelectedFacilityChargeSumLotHelper");
            }
        }

        private void RefreshFacilityChargeSumLotHelperList()
        {
            CurrentFacilityChargeSumLotHelper = null;
            SelectedFacilityChargeSumLotHelper = null;
            OnPropertyChanged("FacilityChargeSumLotHelperList");
        }

        #endregion
        
        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost,
            Description = "Saves the changes of the current storage location to the database.")]
        public void Save()
        {
            OnSave();
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
        [ACMethodCommand(Facility.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost,
            Description = "Discards the unsaved changes of the current storage location.")]
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
        [ACMethodInteraction(Facility.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedFacilityLocation",
            Description = "Reloads the selected storage location with its stock from the database.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<Facility>(requery, () => SelectedFacilityLocation, () => CurrentFacilityLocation, c => CurrentFacilityLocation = c,
                        DatabaseApp.Facility
                        .Include("FacilityStock_Facility")
                        .Where(c => c.ParentFacilityID != null && c.FacilityID == SelectedFacilityLocation.FacilityID));
            if (CurrentFacilityLocation != null && CurrentFacilityLocation.CurrentFacilityStock != null)
                CurrentFacilityLocation.CurrentFacilityStock.AutoRefresh(DatabaseApp);
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
        [ACMethodCommand(Facility.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search,
            Description = "Re-executes the search and rebuilds the FacilityLocationList; only storage locations are returned.")]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("FacilityLocationList");
        }

        IQueryable<Facility> _AccessPrimary_NavSearchExecuting(IQueryable<Facility> result)
        {
            IQueryable<Facility> query = result as IQueryable<Facility>;
            if (query != null)
            {
                query.Include("FacilityStock_Facility");
            }
            return result;
        }

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
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
                case nameof(Load):
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
                case nameof(IsEnabledLoad):
                    result = IsEnabledLoad();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(RefreshMovements):
                    return new[] { nameof(CurrentFacilityLocation) };
                case nameof(Search):
                case nameof(Load):
                    return new[] { nameof(CurrentFacilityLocation), nameof(SelectedFacilityLocation) };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion

        #region FacilityBooking(Charge)Overview methods -> Executive methods overrides

        public override bool IsEnabledRefreshMovements()
        {
            return base.IsEnabledRefreshMovements() && CurrentFacilityLocation != null;
        }

        public override FacilityQueryFilter GetFacilityBookingFilter()
        {
            FacilityQueryFilter filter = base.GetFacilityBookingFilter();
            if (CurrentFacilityLocation != null)
                filter.FacilityLocationID = CurrentFacilityLocation.FacilityID;
            return filter;
        }
        #endregion
    }
}
