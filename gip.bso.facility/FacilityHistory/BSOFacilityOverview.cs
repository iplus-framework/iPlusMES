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
// <copyright file="BSOFacilityOverview.cs" company="gip mbh, Oftersheim, Germany">
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
using System.ComponentModel;
using gip.mes.facility;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace gip.bso.facility
{
    /// <summary>
    /// Businessobject or App for the stock overview of storage bins (Facility of type "Storage bin/container").
    /// This app replaces the legacy stock overview masks (total stocks, storage location stocks and storage bin stocks of materials, charges and material charges).
    /// ALL stock bookings are always executed only via the FacilityBookingManager. This BSO is a read-only overview; it does not create, change or delete stock.
    /// To search for records, enter the search string in the SearchWord property.
    /// The database result is copied to the FacilityList property. Only storage bins (FacilityTypesEnum.StorageBinContainer) are returned.
    /// Then call the NavigateFirst() method to set CurrentFacility with the first record in the list.
    /// CurrentFacility is used to display the currently selected storage bin with its stock (CurrentFacilityStock).
    /// The list can be restricted to the storage bins of one storage location by assigning the SelectedStorageLocation property; assigning it re-executes the search.
    /// The FacilityChargeList property contains the quants stored in the current storage bin.
    /// The FacilityChargeSumMaterialHelperList and FacilityChargeSumLotHelperList properties contain the stock totals per material and per lot.
    /// The ShowNotAvailable property (base class BSOFacilityOverviewBase) controls whether not available quants are included; changing it refreshes the lists.
    /// The FacilityBookingOverviewList and FacilityBookingChargeOverviewList properties (base class BSOFacilityOverviewBase) contain the inward/outward movements of the current storage bin in the period SearchFrom to SearchTo.
    /// Call the RefreshMovements() method to reload the movements (executed in the background); the running sum per row is written to the InOutSumUOM property.
    /// The ShowDialogFacility() method opens this overview as a dialog for a given storage bin number, optionally limited to a period; DialogOK() and DialogCancel() close the dialog.
    /// From a selected quant (SelectedFacilityCharge) the following dialogs can be opened: NavigateToOrder() shows the production order, NavigateToFacilityCharge() opens the quant management, NavigateToFacility() opens the stock of the bin, NavigateToFacilityLotOverview() shows lot stock and history, NavigateToFacilityLot() opens the lot/batch management and NavigateToMaterialOverview() shows material stock and history.
    /// The Save(), UndoSave() and Load() methods follow the common BSO pattern; the overview itself provides no New() or Delete() method.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityOverview&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Storage Bin Overview'}de{'Lagerplatz Übersicht'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Facility.ClassName,
        Description = @"Businessobject or App for the stock overview of storage bins (Facility of type ""Storage bin/container"").
        This app replaces the legacy stock overview masks (total stocks, storage location stocks and storage bin stocks of materials, charges and material charges).
        ALL stock bookings are always executed only via the FacilityBookingManager. This BSO is a read-only overview; it does not create, change or delete stock.
        To search for records, enter the search string in the SearchWord property.
        The database result is copied to the FacilityList property. Only storage bins (FacilityTypesEnum.StorageBinContainer) are returned.
        Then call the NavigateFirst() method to set CurrentFacility with the first record in the list.
        CurrentFacility is used to display the currently selected storage bin with its stock (CurrentFacilityStock).
        The list can be restricted to the storage bins of one storage location by assigning the SelectedStorageLocation property; assigning it re-executes the search.
        The FacilityChargeList property contains the quants stored in the current storage bin.
        The FacilityChargeSumMaterialHelperList and FacilityChargeSumLotHelperList properties contain the stock totals per material and per lot.
        The ShowNotAvailable property (base class BSOFacilityOverviewBase) controls whether not available quants are included; changing it refreshes the lists.
        The FacilityBookingOverviewList and FacilityBookingChargeOverviewList properties (base class BSOFacilityOverviewBase) contain the inward/outward movements of the current storage bin in the period SearchFrom to SearchTo.
        Call the RefreshMovements() method to reload the movements (executed in the background); the running sum per row is written to the InOutSumUOM property.
        The ShowDialogFacility() method opens this overview as a dialog for a given storage bin number, optionally limited to a period; DialogOK() and DialogCancel() close the dialog.
        From a selected quant (SelectedFacilityCharge) the following dialogs can be opened: NavigateToOrder() shows the production order, NavigateToFacilityCharge() opens the quant management, NavigateToFacility() opens the stock of the bin, NavigateToFacilityLotOverview() shows lot stock and history, NavigateToFacilityLot() opens the lot/batch management and NavigateToMaterialOverview() shows material stock and history.
        The Save(), UndoSave() and Load() methods follow the common BSO pattern; the overview itself provides no New() or Delete() method.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityOverview&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOFacilityOverview : BSOFacilityOverviewBase
    {
        #region Constants

        public const string StorageBinFilterPropertyName = "Facility1_ParentFacility\\FacilityNo";

        #endregion

        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityOverview"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityOverview(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            this._SelectedStorageLocation = null;
            this._StorageLocationList = null;

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
        [ACPropertyAccessPrimary(890, Facility.ClassName, "en{'Storage bin'}de{'Lagerplatz'}",
            Description = "Navigation access to the storage bins; source of FacilityList, SelectedFacility and CurrentFacility.")]
        public ACAccessNav<Facility> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(NavigationqueryDefaultFilter);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<Facility>(Facility.ClassName, this);
                    _AccessPrimary.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;
                }
                return _AccessPrimary;
            }
        }

        protected virtual List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, "FacilityNo", Global.LogicalOperators.contains, Global.Operators.or, null, true, true),
                    new ACFilterItem(Global.FilterTypes.filter, "FacilityName", Global.LogicalOperators.contains, Global.Operators.or, null, true, true),
                    new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, ((short)FacilityTypesEnum.StorageBinContainer).ToString(), true),
                    new ACFilterItem(Global.FilterTypes.filter, StorageBinFilterPropertyName, Global.LogicalOperators.equal, Global.Operators.and, "", true)
                };
            }
        }

        /// <summary>
        /// Gets or sets the selected facility.
        /// </summary>
        /// <value>The selected facility.</value>
        [ACPropertySelected(801, Facility.ClassName, "en{'Storage bin'}de{'Lagerplatz'}",
            Description = "Storage bin selected in the list.")]
        public Facility SelectedFacility
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
                OnPropertyChanged("SelectedFacility");
            }
        }

        /// <summary>
        /// Gets or sets the current facility.
        /// </summary>
        /// <value>The current facility.</value>
        [ACPropertyCurrent(802, Facility.ClassName, "en{'Storage bin'}de{'Lagerplatz'}",
            Description = "Currently displayed storage bin with its stock (CurrentFacilityStock).")]
        public Facility CurrentFacility
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
                OnPropertyChanged("CurrentFacility");
                OnPropertyChanged("CurrentFacility\\CurrentFacilityStock");
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
        /// Gets the facility list.
        /// </summary>
        /// <value>The facility list.</value>
        [ACPropertyList(803, Facility.ClassName, "en{'Storage bin'}de{'Lagerplatz'}",
            Description = "Result list of the search; contains only storage bins (FacilityTypesEnum.StorageBinContainer).")]
        public IEnumerable<Facility> FacilityList
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.NavList;
            }
        }
        #endregion

        #region StorageLocation Filter

        private Facility _SelectedStorageLocation;
        /// <summary>
        /// Selected property for Facility
        /// </summary>
        /// <value>The selected StorageLocation</value>
        [ACPropertySelected(804, "StorageLocation", "en{'Storage location'}de{'Lagerort'}",
            Description = "Restricts the bin list to the storage bins of this storage location; assigning it re-executes the search.")]
        public Facility SelectedStorageLocation
        {
            get
            {
                return _SelectedStorageLocation;
            }
            set
            {
                if (_SelectedStorageLocation != value)
                {
                    _SelectedStorageLocation = value;
                    if (value != null)
                        AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(StorageBinFilterPropertyName, value.FacilityNo);
                    else
                        AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(StorageBinFilterPropertyName, "");
                    OnPropertyChanged("SelectedStorageLocation");
                }
            }
        }

        private IEnumerable<Facility> _StorageLocationList;
        /// <summary>
        /// List property for Facility
        /// </summary>
        /// <value>The StorageLocation list</value>
        [ACPropertyList(805, "StorageLocation", "en{'Storage location'}de{'Lagerort'}",
            Description = "All storage locations offered for filtering the bin list.")]
        public IEnumerable<Facility> StorageLocationList
        {
            get
            {
                if (_StorageLocationList == null)
                    _StorageLocationList = DatabaseApp.Facility.Where(x => x.ParentFacilityID == null && x.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageLocation).OrderBy(x => x.FacilityNo).ToList();
                return _StorageLocationList;
            }
        }

        //public ACFilterItem StorageLocationFilter
        //{
        //    get
        //    {
        //        if (AccessPrimary == null)
        //            return null;
        //        ACFilterItem item = AccessPrimary.NavACQueryDefinition.ACFilterColumns.FirstOrDefault(x => x.PropertyName == StorageBinFilterPropertyName);
        //        if (item == null)
        //        {
        //            item = new ACFilterItem(Global.FilterTypes.filter, StorageBinFilterPropertyName, Global.LogicalOperators.equal, Global.Operators.and, "", true);
        //            AccessPrimary.NavACQueryDefinition.ACFilterColumns.Add(item);
        //        }
        //        return item;
        //    }
        //}

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
        [ACPropertyCurrent(806, FacilityCharge.ClassName, "en{'Quant'}de{'Quant'}",
            Description = "Currently displayed quant of the storage bin.")]
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
        [ACPropertyList(807, FacilityCharge.ClassName, "en{'Quant'}de{'Quant'}",
            Description = "Quants stored in the current storage bin.")]
        public IEnumerable<FacilityCharge> FacilityChargeList
        {
            get
            {
                if (_FacilityChargeList != null)
                    return _FacilityChargeList;
                if (CurrentFacility == null)
                    return null;
                _FacilityChargeList = FacilityManager.s_cQry_FacilityOverviewFacilityCharge(this.DatabaseApp, CurrentFacility.FacilityID, ShowNotAvailable).ToArray();
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
        [ACPropertySelected(808, FacilityCharge.ClassName, "en{'Quant'}de{'Quant'}",
            Description = "Quant selected in the stock list; source for the navigation methods.")]
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

        #region Sum Material
        /// <summary>
        /// The _ current facility charge sum material helper
        /// </summary>
        FacilityChargeSumMaterialHelper _CurrentFacilityChargeSumMaterialHelper;
        /// <summary>
        /// Gets or sets the current facility charge sum material helper.
        /// </summary>
        /// <value>The current facility charge sum material helper.</value>
        [ACPropertyCurrent(809, "FacilityChargeSumMaterialHelper", "en{'Totals per material'}de{'Summen pro Material'}",
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
        [ACPropertyList(810, "FacilityChargeSumMaterialHelper", "en{'Totals per material'}de{'Summen pro Material'}",
            Description = "Stock totals of the current storage bin grouped by material.")]
        public IEnumerable<FacilityChargeSumMaterialHelper> FacilityChargeSumMaterialHelperList
        {
            get
            {
                if (CurrentFacility == null)
                    return null;

                return ACFacilityManager.GetFacilityChargeSumMaterialHelperList(FacilityChargeList, new FacilityQueryFilter() { FacilityID = CurrentFacility.FacilityID });
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
        [ACPropertySelected(811, "FacilityChargeSumMaterialHelper", "en{'Totals per material'}de{'Summen pro Material'}",
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

        #endregion


        #region Sum Lot
        /// <summary>
        /// The _ current facility charge sum lot helper
        /// </summary>
        FacilityChargeSumLotHelper _CurrentFacilityChargeSumLotHelper;
        /// <summary>
        /// Gets or sets the current facility charge sum lot helper.
        /// </summary>
        /// <value>The current facility charge sum lot helper.</value>
        [ACPropertyCurrent(812, "FacilityChargeSumLotHelper", "en{'Totals per lot'}de{'Summen pro Los'}",
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
        [ACPropertyList(813, "FacilityChargeSumLotHelper", "en{'Totals per lot'}de{'Summen pro Los'}",
            Description = "Stock totals of the current storage bin grouped by lot.")]
        public IEnumerable<FacilityChargeSumLotHelper> FacilityChargeSumLotHelperList
        {
            get
            {
                if (CurrentFacility == null)
                    return null;
                return ACFacilityManager.GetFacilityChargeSumLotHelperList(FacilityChargeList, new FacilityQueryFilter() { FacilityID = CurrentFacility.FacilityID });
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
        [ACPropertySelected(814, "FacilityChargeSumLotHelper", "en{'Totals per lot'}de{'Summen pro Los'}",
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

        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost,
            Description = "Saves the changes of the current storage bin to the database.")]
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
        [ACMethodCommand(Facility.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost,
            Description = "Discards the unsaved changes of the current storage bin.")]
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
        [ACMethodInteraction(Facility.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedFacility",
            Description = "Reloads the selected storage bin with its stock from the database (e.g. after another user changed it in the background).")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<Facility>(requery, () => SelectedFacility, () => CurrentFacility, c => CurrentFacility = c,
                        DatabaseApp.Facility
                        .Include(c => c.FacilityStock_Facility)
                        .Include(c => c.Material)
                        .Include(c => c.MDFacilityType)
                        .Where(c => c.FacilityID == SelectedFacility.FacilityID));
            if (CurrentFacility != null && CurrentFacility.CurrentFacilityStock != null)
            {
                CurrentFacility.FacilityStock_Facility.AutoLoad(CurrentFacility.FacilityStock_FacilityReference, CurrentFacility);
            }

            PostExecute("Load");
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search,
            Description = "Re-executes the search and rebuilds the FacilityList; only storage bins are returned.")]
        public virtual void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("FacilityList");
        }

        IQueryable<Facility> _AccessPrimary_NavSearchExecuting(IQueryable<Facility> result)
        {
            IQueryable<Facility> query = result as IQueryable<Facility>;
            if (query == null) return null;

            var testQuery = query
                .Include(p => p.FacilityStock_Facility)
                .Include(p => p.Material)
                .Join(DatabaseApp.FacilityStock, f => f.FacilityID, s => s.FacilityID, (f, s) => new { f = f, s = s })
                .OrderBy(p => p.f.FacilityNo)
                .ToList();

            List<Facility> testResult = new List<Facility>();
            testQuery.ForEach(c =>
            {
                c.f.CurrentFacilityStock = c.s;
                testResult.Add(c.f);
            });

            return testResult.AsQueryable();
        }

        #endregion

        #region Dialog
        public bool DialogResult
        {
            get;
            set;
        }

        [ACMethodInfo("Dialog", "en{'Dialog Facility'}de{'Dialog Zelle'}", (short)MISort.QueryPrintDlg,
            Description = "Opens this overview as a dialog for the given storage bin number, optionally limited to the period searchFrom/searchTo.")]
        public async Task ShowDialogFacility(string facilityNo, DateTime? searchFrom = null, DateTime? searchTo = null)
        {
            if (AccessPrimary == null)
                return;
            //AccessPrimary.NavACQueryDefinition.SearchWord = facilityNo;
            ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "FacilityNo").FirstOrDefault();
            if (filterItem == null)
            {
                filterItem = new ACFilterItem(Global.FilterTypes.filter, "FacilityNo", Global.LogicalOperators.contains, Global.Operators.and, facilityNo, false);
                AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);
            }
            else
                filterItem.SearchWord = facilityNo;
            if (searchFrom != null)
                SearchFrom = searchFrom.Value;
            if (searchTo != null)
                SearchTo = searchTo.Value;
            this.Search();
            await ShowDialogAsync(this, "OverviewDialog");
            await this.ParentACComponent.StopComponent(this);
        }

        [ACMethodCommand("Dialog", Const.Ok, (short)MISort.Okay,
            Description = "Closes the dialog with OK (DialogResult = true).")]
        public void DialogOK()
        {
            DialogResult = true;
            CloseTopDialog();
        }

        [ACMethodCommand("Dialog", Const.Cancel, (short)MISort.Cancel,
            Description = "Closes the dialog with Cancel (DialogResult = false).")]
        public void DialogCancel()
        {
            DialogResult = false;
            CloseTopDialog();
        }

        [ACMethodInfo("Dialog", "en{'Dialog lot overview'}de{'Dialog Losübersicht'}", (short)MISort.QueryPrintDlg + 1,
            Description = "Opens this overview as a dialog for the facility of the given PAOrderInfo entry.")]
        public virtual async Task ShowDialogOrderInfo(PAOrderInfo paOrderInfo)
        {
            if (AccessPrimary == null || paOrderInfo == null)
                return;

            PAOrderInfoEntry entityInfo = paOrderInfo.Entities.Where(c => c.EntityName == nameof(Facility)).FirstOrDefault();
            if (entityInfo == null)
                return;

            Facility facility = this.DatabaseApp.Facility.Where(c => c.FacilityID == entityInfo.EntityID).FirstOrDefault();
            if (facility == null)
                return;

            await ShowDialogFacility(facility.FacilityNo);
        }
        #endregion


        #region Dialog Navigate
        [ACMethodInteraction("", ConstApp.ShowProdOrder, 780, true, nameof(SelectedFacilityCharge),
            Description = "Opens the production order dialog for the production order position that produced the selected quant (enabled if the quant has a final position).")]
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
        [ACMethodInteraction(nameof(NavigateToFacilityCharge), "en{'Manage Quant'}de{'Quant verwalten'}", 781, true, nameof(SelectedFacilityCharge),
            Description = "Opens the quant management dialog for the selected quant.")]
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


        [ACMethodInteraction("", "en{'Manage Stock of Bin'}de{'Verwalte Behälterbestand'}", 782, true, nameof(SelectedFacility),
            Description = "Opens the stock management of the selected storage bin (enabled for bins of type StorageBinContainer).")]
        public async Task NavigateToFacility()
        {
            if (!IsEnabledNavigateToFacility())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(Facility), SelectedFacility.FacilityID));
                await service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacility()
        {
            if (SelectedFacility != null && SelectedFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                return true;
            return false;
        }

        [ACMethodInteraction("", "en{'Show Lot Stock and History'}de{'Zeige Losbestand und Historie'}", 783, true, nameof(SelectedFacilityCharge),
            Description = "Shows the stock and history of the lot of the selected quant.")]
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

        [ACMethodInteraction("", "en{'Manage Lot/Batch'}de{'Verwalte Los/Charge'}", 784, true, nameof(SelectedFacilityCharge),
            Description = "Opens the lot/batch management for the lot of the selected quant.")]
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

        [ACMethodInteraction("", "en{'Show Material Stock and History'}de{'Zeige Materialbestand und Historie'}", 785, true, nameof(SelectedFacilityCharge),
            Description = "Shows the stock and history of the material of the selected quant.")]
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
                case nameof(Search):
                    Search();
                    return true;
                case nameof(ShowDialogFacility):
                    if (acParameter.Count() == 1)
                        result = ShowDialogFacility(acParameter[0] as string);
                    else
                        result = ShowDialogFacility(acParameter[0] as string, acParameter[1] as DateTime?, acParameter[2] as DateTime?);
                    return true;
                case nameof(DialogOK):
                    DialogOK();
                    return true;
                case nameof(DialogCancel):
                    DialogCancel();
                    return true;
                case nameof(NavigateToFacilityLot):
                    result = NavigateToFacilityLot();
                    return true;
                case nameof(IsEnabledNavigateToFacilityLot):
                    result = IsEnabledNavigateToFacilityLot();
                    return true;
                case nameof(NavigateToFacilityLotOverview):
                    result = NavigateToFacilityLotOverview();
                    return true;
                case nameof(IsEnabledNavigateToFacilityLotOverview):
                    result = IsEnabledNavigateToFacilityLotOverview();
                    return true;
                case nameof(NavigateToFacilityCharge):
                    result = NavigateToFacilityCharge();
                    return true;
                case nameof(IsEnabledNavigateToFacilityCharge):
                    result = IsEnabledNavigateToFacilityCharge();
                    return true;
                case nameof(NavigateToFacility):
                    result = NavigateToFacility();
                    return true;
                case nameof(IsEnabledNavigateToFacility):
                    result = IsEnabledNavigateToFacility();
                    return true;
                case nameof(ShowDialogOrderInfo):
                    result = ShowDialogOrderInfo((gip.core.autocomponent.PAOrderInfo)acParameter[0]);
                    return true;
                case nameof(NavigateToMaterialOverview):
                    result = NavigateToMaterialOverview();
                    return true;
                case nameof(IsEnabledNavigateToMaterialOverview):
                    result = IsEnabledNavigateToMaterialOverview();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(RefreshMovements):
                    return new[] { nameof(CurrentFacility) };
                case nameof(Load):
                    return new[] { nameof(CurrentFacilityCharge), nameof(SelectedFacilityCharge) };
                case nameof(Search):
                    return new[] { nameof(CurrentFacilityCharge), nameof(SelectedFacilityCharge) };
                case nameof(ShowDialogFacility):
                    return new[] { nameof(CurrentFacilityCharge), nameof(SelectedFacilityCharge) };
                case nameof(DialogOK):
                case nameof(DialogCancel):
                    return new [] { nameof(InitState) };
                case nameof(NavigateToFacilityLot):
                    return new[] { nameof(CurrentFacilityCharge), nameof(SelectedFacilityCharge) };
                case nameof(NavigateToFacilityLotOverview):
                    return new[] { nameof(CurrentFacilityCharge), nameof(SelectedFacilityCharge) };
                case nameof(NavigateToFacilityCharge):
                    return new[] { nameof(CurrentFacilityCharge), nameof(SelectedFacilityCharge) };
                case nameof(NavigateToFacility):
                    return new[] { nameof(CurrentFacilityCharge), nameof(SelectedFacilityCharge) };
                case nameof(ShowDialogOrderInfo):
                    return new[] { nameof(CurrentFacilityCharge), nameof(SelectedFacilityCharge) };
                case nameof(NavigateToMaterialOverview):
                    return new[] { nameof(CurrentFacilityCharge), nameof(SelectedFacilityCharge) };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion

        #region FacilityBooking(Charge)Overview methods -> Executive methods overrides

        public override bool IsEnabledRefreshMovements()
        {
            return base.IsEnabledRefreshMovements() && CurrentFacility != null;
        }

        public override FacilityQueryFilter GetFacilityBookingFilter()
        {
            FacilityQueryFilter filter=   base.GetFacilityBookingFilter();
            if (CurrentFacility != null)
                filter.FacilityID = CurrentFacility.FacilityID;
            return filter;
        }

        public override void OnFacilityBookingSearchSum()
        {
            if (FacilityBookingOverviewList != null && CurrentFacility != null)
            {
                double sum = 0.0;
                foreach (var fb in FacilityBookingOverviewList)
                {
                    if (String.IsNullOrEmpty(fb.InwardFacilityNo) || fb.InwardFacilityNo == CurrentFacility.FacilityNo)
                        sum += fb.InwardQuantityUOM;
                    if (String.IsNullOrEmpty(fb.OutwardFacilityNo) || fb.OutwardFacilityNo == CurrentFacility.FacilityNo)
                        sum -= fb.OutwardQuantityUOM;
                    fb.InOutSumUOM = sum;
                }
            }

            if (FacilityBookingChargeOverviewList != null && CurrentFacility != null)
            {
                double sum = 0.0;
                foreach (var fb in FacilityBookingChargeOverviewList)
                {
                    if (fb.InwardFacilityNo == CurrentFacility.FacilityNo)
                        sum += fb.InwardQuantityUOM;
                    if (fb.OutwardFacilityNo == CurrentFacility.FacilityNo)
                        sum -= fb.OutwardQuantityUOM;
                    fb.InOutSumUOM = sum;
                }
            }
        }
        #endregion

    }
}


