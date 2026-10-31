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
// <copyright file="BSOFacilityHistory.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace gip.bso.facility
{
    /// <summary>
    /// Businessobject or App for displaying the balance sheet history (stock balances per period).
    /// The balance sheets are created by the system (period closing); this BSO is read-only, it cannot create new records and Delete() is disabled.
    /// To search for records, enter the search string in the SearchWord property.
    /// The database result is copied to the HistoryList property, sorted by balance date descending.
    /// Then call the NavigateFirst() method to set CurrentHistory with the first record in the list.
    /// CurrentHistory is used to display the currently selected balance sheet.
    /// The list is filtered by the time period: FilterSelectedTimePeriod (day, week, month, year, ...), FilterPeriodNo and the balance date range FilterBalanceDateFrom/FilterBalanceDateTo.
    /// Changing one of these filter properties re-executes the search automatically.
    /// The MaterialHistoryList property contains the balances per material of the current balance sheet.
    /// The CompanyMaterialHistoryList property contains the balances per company material; it can be restricted to one company with the SelectedCompanyFilter property.
    /// The FacilityHistoryList property contains the balances per storage bin of the current balance sheet.
    /// The Save() method saves changes, UndoSave() discards them and reloads, Load() reloads the current balance sheet with all detail lists if another user has made changes in the background.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityHistory&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Balance Sheet History'}de{'Bilanzhistorie'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + History.ClassName,
        Description = @"Businessobject or App for displaying the balance sheet history (stock balances per period).
        The balance sheets are created by the system (period closing); this BSO is read-only, it cannot create new records and Delete() is disabled.
        To search for records, enter the search string in the SearchWord property.
        The database result is copied to the HistoryList property, sorted by balance date descending.
        Then call the NavigateFirst() method to set CurrentHistory with the first record in the list.
        CurrentHistory is used to display the currently selected balance sheet.
        The list is filtered by the time period: FilterSelectedTimePeriod (day, week, month, year, ...), FilterPeriodNo and the balance date range FilterBalanceDateFrom/FilterBalanceDateTo.
        Changing one of these filter properties re-executes the search automatically.
        The MaterialHistoryList property contains the balances per material of the current balance sheet.
        The CompanyMaterialHistoryList property contains the balances per company material; it can be restricted to one company with the SelectedCompanyFilter property.
        The FacilityHistoryList property contains the balances per storage bin of the current balance sheet.
        The Save() method saves changes, UndoSave() discards them and reloads, Load() reloads the current balance sheet with all detail lists if another user has made changes in the background.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityHistory&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOFacilityHistory : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityHistory"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityHistory(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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

            SelectedCompanyFilter = CompanyFilterList.FirstOrDefault();
            _FilterSelectedTimePeriod = TimePeriodsList.Where(c => (GlobalApp.TimePeriods)c.Value == gip.mes.datamodel.GlobalApp.TimePeriods.Day).FirstOrDefault();
            _FilterCurrentTimePeriod = _FilterSelectedTimePeriod;
            Search();
            return true;
        }


        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;

            this._CurrentCompanyFilter = null;
            this._CurrentCompanyMaterialHistory = null;
            this._CurrentFacilityHistory = null;
            this._CurrentMaterialHistory = null;
            this._SelectedCompanyFilter = null;
            this._SelectedCompanyMaterialHistory = null;
            this._SelectedFacilityHistory = null;
            this._SelectedMaterialHistory = null;
            this._FilterBalanceDateFrom = null;
            this._FilterBalanceDateTo = null;
            this._FilterCurrentTimePeriod = null;
            this._FilterPeriodNo = null;
            this._FilterSelectedTimePeriod = null;
            var b = await base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                await _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }

        public override object Clone()
        {
            BSOFacilityHistory clone = CreateNewBSOInstanceOfThis() as BSOFacilityHistory;
            clone.SelectedHistory = this.SelectedHistory;
            clone.CurrentHistory = this.SelectedHistory;
            clone.MaterialHistoryList = MaterialHistoryList;
            return clone;
        }

        #endregion

        #region Managers
        /// <summary>
        /// The _ facility manager
        /// </summary>
        protected ACRef<ACComponent> _ACFacilityManager = null;
        protected FacilityManager ACFacilityManager
        {
            get
            {
                if (_ACFacilityManager == null)
                    return null;
                return _ACFacilityManager.ValueT as FacilityManager;
            }
        }
        #endregion

        #region events

        public event EventHandler OnSelectedHistoryChanged;
        #endregion

        #region BSO->ACProperty

        #region History
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<History> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(690, History.ClassName, "en{'Balance sheet'}de{'Bilanz'}",
            Description = "Navigation access to the balance sheets; source of HistoryList, SelectedHistory and CurrentHistory.")]
        public ACAccessNav<History> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    navACQueryDefinition.ACFilterColumns.Clear();
                    if (navACQueryDefinition != null)
                    {
                        ACSortItem sortItem = navACQueryDefinition.ACSortColumns.Where(c => c.ACIdentifier == "BalanceDate").FirstOrDefault();
                        if (sortItem != null && sortItem.IsConfiguration)
                        {
                            sortItem.SortDirection = Global.SortDirections.descending;
                        }
                    }
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<History>(History.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the selected facility history.
        /// </summary>
        /// <value>The selected facility history.</value>
        [ACPropertySelected(601, History.ClassName, "en{'Balance sheet'}de{'Bilanz'}",
            Description = "Balance sheet selected in the list.")]
        public History SelectedHistory
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
                if (OnSelectedHistoryChanged != null)
                    OnSelectedHistoryChanged(this, new EventArgs() { });
                OnPropertyChanged("SelectedHistory");
            }
        }

        /// <summary>
        /// Gets or sets the current facility history.
        /// </summary>
        /// <value>The current facility history.</value>
        [ACPropertyCurrent(602, History.ClassName, "en{'Balance sheet'}de{'Bilanz'}",
            Description = "Currently displayed balance sheet; changing it refreshes the detail lists.")]
        public History CurrentHistory
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
                if (AccessPrimary.Current != value)
                {
                    _MaterialHistoryList = null;
                    AccessPrimary.Current = value;
                    OnPropertyChanged("CurrentHistory");
                    OnPropertyChanged("MaterialHistoryList");
                    OnPropertyChanged("CompanyMaterialHistoryList");
                    OnPropertyChanged("FacilityHistoryList");
                }
            }
        }

        /// <summary>
        /// Gets or sets the facility history list.
        /// </summary>
        /// <value>The facility history list.</value>
        [ACPropertyList(603, History.ClassName, "en{'Balance sheet'}de{'Bilanz'}",
            Description = "Result list of the search, sorted by balance date descending.")]
        public IEnumerable<History> HistoryList
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.NavList;
            }
        }
        #endregion

        #region MaterialHistory
        MaterialHistory _SelectedMaterialHistory;
        /// <summary>
        /// Gets or sets the selected facility history.
        /// </summary>
        /// <value>The selected facility history.</value>
        [ACPropertySelected(604, MaterialHistory.ClassName, "en{'Material balance'}de{'Materialbilanz'}",
            Description = "Material balance row selected in the list.")]
        public MaterialHistory SelectedMaterialHistory
        {
            get
            {
                return _SelectedMaterialHistory;
            }
            set
            {
                _SelectedMaterialHistory = value;
                OnPropertyChanged("SelectedMaterialHistory");
            }
        }

        MaterialHistory _CurrentMaterialHistory;
        /// <summary>
        /// Gets or sets the current facility history.
        /// </summary>
        /// <value>The current facility history.</value>
        [ACPropertyCurrent(605, MaterialHistory.ClassName, "en{'Material balance'}de{'Materialbilanz'}",
            Description = "Currently displayed material balance row.")]
        public MaterialHistory CurrentMaterialHistory
        {
            get
            {
                return _CurrentMaterialHistory;
            }
            set
            {
                _CurrentMaterialHistory = value;
                OnPropertyChanged("CurrentMaterialHistory");
            }
        }

        /// <summary>
        /// Gets or sets the facility history list.
        /// </summary>
        /// <value>The facility history list.</value>
        private List<MaterialHistory> _MaterialHistoryList;
        [ACPropertyList(606, MaterialHistory.ClassName, "en{'Material balance'}de{'Materialbilanz'}",
            Description = "Balances per material of the current balance sheet.")]
        public virtual List<MaterialHistory> MaterialHistoryList
        {
            get
            {
                if (CurrentHistory == null)
                    return null;
                if (_MaterialHistoryList == null)
                    _MaterialHistoryList = CurrentHistory.MaterialHistory_History.OrderBy(c => c.Material.MaterialNo).ToList();
                return _MaterialHistoryList;
            }
            set
            {
                _MaterialHistoryList = value;
            }
        }
        #endregion

        #region CompanyMaterialHistory
        Company _SelectedCompanyFilter;
        /// <summary>
        /// Gets or sets the selected facility history.
        /// </summary>
        /// <value>The selected facility history.</value>
        [ACPropertySelected(607, "CompanyFilter", "en{'Filter by Company'}de{'Filtern nach Unternehmen'}",
            Description = "Company the company material balances are filtered by.")]
        public Company SelectedCompanyFilter
        {
            get
            {
                return _SelectedCompanyFilter;
            }
            set
            {
                _SelectedCompanyFilter = value;
                CurrentCompanyFilter = value;
                OnPropertyChanged("SelectedCompanyFilter");
            }
        }

        Company _CurrentCompanyFilter;
        /// <summary>
        /// Gets or sets the current facility history.
        /// </summary>
        /// <value>The current facility history.</value>
        [ACPropertyCurrent(608, "CompanyFilter", "en{'Filter by Company'}de{'Filtern nach Unternehmen'}",
            Description = "Company the company material balances are filtered by.")]
        public Company CurrentCompanyFilter
        {
            get
            {
                return _CurrentCompanyFilter;
            }
            set
            {
                _CurrentCompanyFilter = value;
                OnPropertyChanged("CurrentCompanyFilter");
                OnPropertyChanged("CompanyMaterialHistoryList");
            }
        }

        /// <summary>
        /// Gets or sets the facility history list.
        /// </summary>
        /// <value>The facility history list.</value>
        [ACPropertyList(609, "CompanyFilter", "en{'Filter by Company'}de{'Filtern nach Unternehmen'}",
            Description = "All companies offered for filtering the company material balances.")]
        public IEnumerable<Company> CompanyFilterList
        {
            get
            {
                return DatabaseApp.CPartnerCompanyList;
            }
        }


        CompanyMaterialHistory _SelectedCompanyMaterialHistory;
        /// <summary>
        /// Gets or sets the selected facility history.
        /// </summary>
        /// <value>The selected facility history.</value>
        [ACPropertySelected(610, CompanyMaterialHistory.ClassName, "en{'Company material balance'}de{'Unternehmensmaterialbilanz'}",
            Description = "Company material balance row selected in the list.")]
        public CompanyMaterialHistory SelectedCompanyMaterialHistory
        {
            get
            {
                return _SelectedCompanyMaterialHistory;
            }
            set
            {
                _SelectedCompanyMaterialHistory = value;
                OnPropertyChanged("SelectedCompanyMaterialHistory");
            }
        }

        CompanyMaterialHistory _CurrentCompanyMaterialHistory;
        /// <summary>
        /// Gets or sets the current facility history.
        /// </summary>
        /// <value>The current facility history.</value>
        [ACPropertyCurrent(611, CompanyMaterialHistory.ClassName, "en{'Company material balance'}de{'Unternehmensmaterialbilanz'}",
            Description = "Currently displayed company material balance row.")]
        public CompanyMaterialHistory CurrentCompanyMaterialHistory
        {
            get
            {
                return _CurrentCompanyMaterialHistory;
            }
            set
            {
                _CurrentCompanyMaterialHistory = value;
                OnPropertyChanged("CurrentCompanyMaterialHistory");
            }
        }

        /// <summary>
        /// Gets or sets the facility history list.
        /// </summary>
        /// <value>The facility history list.</value>
        [ACPropertyList(612, CompanyMaterialHistory.ClassName, "en{'Company material balance'}de{'Unternehmensmaterialbilanz'}",
            Description = "Balances per company material of the current balance sheet, optionally filtered by SelectedCompanyFilter.")]
        public virtual IEnumerable<CompanyMaterialHistory> CompanyMaterialHistoryList
        {
            get
            {
                if (CurrentHistory == null)
                    return null;
                if (CurrentCompanyFilter != null)
                    return CurrentHistory.CompanyMaterialHistory_History.Where(c => c.CompanyMaterial.CompanyID == CurrentCompanyFilter.CompanyID).OrderBy(c => c.CompanyMaterial.CompanyMaterialNo);
                else
                    return CurrentHistory.CompanyMaterialHistory_History.OrderBy(c => c.CompanyMaterial.CompanyMaterialNo);
            }
        }
        #endregion

        #region FacilityHistory
        FacilityHistory _SelectedFacilityHistory;
        /// <summary>
        /// Gets or sets the selected facility history.
        /// </summary>
        /// <value>The selected facility history.</value>
        [ACPropertySelected(613, FacilityHistory.ClassName, "en{'Storage bin balance'}de{'Lagerplatzbilanz'}",
            Description = "Storage bin balance row selected in the list.")]
        public FacilityHistory SelectedFacilityHistory
        {
            get
            {
                return _SelectedFacilityHistory;
            }
            set
            {
                _SelectedFacilityHistory = value;
                OnPropertyChanged("SelectedFacilityHistory");
            }
        }

        FacilityHistory _CurrentFacilityHistory;
        /// <summary>
        /// Gets or sets the current facility history.
        /// </summary>
        /// <value>The current facility history.</value>
        [ACPropertyCurrent(614, FacilityHistory.ClassName, "en{'Storage bin balance'}de{'Lagerplatzbilanz'}",
            Description = "Currently displayed storage bin balance row.")]
        public FacilityHistory CurrentFacilityHistory
        {
            get
            {
                return _CurrentFacilityHistory;
            }
            set
            {
                _CurrentFacilityHistory = value;
                OnPropertyChanged("CurrentFacilityHistory");
            }
        }

        /// <summary>
        /// Gets or sets the facility history list.
        /// </summary>
        /// <value>The facility history list.</value>
        [ACPropertyList(615, FacilityHistory.ClassName, "en{'Storage bin balance'}de{'Lagerplatzbilanz'}",
            Description = "Balances per storage bin of the current balance sheet.")]
        public IEnumerable<FacilityHistory> FacilityHistoryList
        {
            get
            {
                if (CurrentHistory == null)
                    return null;
                return CurrentHistory.FacilityHistory_History.OrderBy(c => c.Facility.FacilityNo);
            }
        }
        #endregion

        #region TimePeriod
        /// <summary>
        /// The _ selected time period
        /// </summary>
        /// GlobalApp.TimePeriods
        ACValueItem _FilterSelectedTimePeriod;
        /// <summary>
        /// Gets or sets the selected time period.
        /// </summary>
        /// <value>The selected time period.</value>
        [ACPropertySelected(616, "TimePeriod", "en{'Time Period'}de{'Zeitspanne'}",
            Description = "Selected aggregation level (day, week, month, year, ...); assigning it re-executes the search.")]
        public ACValueItem FilterSelectedTimePeriod
        {
            get
            {
                return _FilterSelectedTimePeriod;
            }
            set
            {
                _FilterSelectedTimePeriod = value;
                FilterCurrentTimePeriod = value;
                OnPropertyChanged("FilterSelectedTimePeriod");
            }

        }

        /// <summary>
        /// The _ current time period
        /// </summary>
        ACValueItem _FilterCurrentTimePeriod;
        /// <summary>
        /// Gets or sets the current time period.
        /// </summary>
        /// <value>The current time period.</value>
        [ACPropertyCurrent(617, "TimePeriod", "en{'Period'}de{'Periode'}",
            Description = "Active aggregation level of the balance sheet search.")]
        public ACValueItem FilterCurrentTimePeriod
        {
            get
            {
                return _FilterCurrentTimePeriod;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                _FilterCurrentTimePeriod = value;
                ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "TimePeriodIndex" && c.LogicalOperator == Global.LogicalOperators.equal).FirstOrDefault();
                if (filterItem == null)
                {
                    filterItem = new ACFilterItem(Global.FilterTypes.filter, "TimePeriodIndex", Global.LogicalOperators.equal, Global.Operators.and, "", true);
                    _AccessPrimary.NavACQueryDefinition.ACFilterColumns.Add(filterItem);
                }
                AccessPrimary.NavACQueryDefinition.SaveToACConfigOff = true;
                filterItem.SearchWord = ((short)_FilterCurrentTimePeriod.Value).ToString();
                AccessPrimary.NavACQueryDefinition.SaveToACConfigOff = false;
                OnPropertyChanged("FilterCurrentTimePeriod");
                Search();
            }
        }

        [ACPropertyList(618, "TimePeriod", "en{'Period'}de{'Periode'}",
            Description = "All time periods offered for filtering.")]
        public ACValueItemList TimePeriodsList
        {
            get
            {
                return GlobalApp.TimePeriodsList;
            }
        }

        /// <summary>
        /// The _ current period
        /// </summary>
        Nullable<Int32> _FilterPeriodNo;
        /// <summary>
        /// Gets or sets the current period.
        /// </summary>
        /// <value>The current period.</value>
        [ACPropertyInfo(619, "", "en{'Period no.'}de{'Periodennr.'}",
            Description = "Period number the balance sheets are filtered by; assigning it re-executes the search.")]
        public Nullable<Int32> FilterPeriodNo
        {
            get
            {
                return _FilterPeriodNo;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                _FilterPeriodNo = value;
                ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "PeriodNo" && c.LogicalOperator == Global.LogicalOperators.equal).FirstOrDefault();
                if (_FilterPeriodNo.HasValue)
                {
                    if (filterItem == null)
                    {
                        filterItem = new ACFilterItem(Global.FilterTypes.filter, "PeriodNo", Global.LogicalOperators.equal, Global.Operators.and, "", true);
                        _AccessPrimary.NavACQueryDefinition.ACFilterColumns.Add(filterItem);
                    }
                    AccessPrimary.NavACQueryDefinition.SaveToACConfigOff = true;
                    filterItem.SearchWord = _FilterPeriodNo.ToString();
                    AccessPrimary.NavACQueryDefinition.SaveToACConfigOff = false;
                }
                else
                {
                    if (filterItem != null)
                        AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItem);
                }
                OnPropertyChanged("FilterPeriodNo");
                Search();
            }
        }

        /// <summary>
        /// The _ current period
        /// </summary>
        Nullable<DateTime> _FilterBalanceDateFrom;
        /// <summary>
        /// Gets or sets the current period.
        /// </summary>
        /// <value>The current period.</value>
        [ACPropertyInfo(620, "", "en{'Balance date from'}de{'Bilanzdatum von'}",
            Description = "Start of the balance date range; assigning it re-executes the search.")]
        public Nullable<DateTime> FilterBalanceDateFrom
        {
            get
            {
                return _FilterBalanceDateFrom;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                _FilterBalanceDateFrom = value;
                ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "BalanceDate" && c.LogicalOperator == Global.LogicalOperators.greaterThanOrEqual).FirstOrDefault();
                if (_FilterBalanceDateFrom.HasValue)
                {
                    if (value.Value.Hour > 0 || value.Value.Minute > 0 || value.Value.Second > 0)
                        _FilterBalanceDateTo = new DateTime(value.Value.Year, value.Value.Month, value.Value.Day);

                    if (filterItem == null)
                    {
                        filterItem = new ACFilterItem(Global.FilterTypes.filter, "BalanceDate", Global.LogicalOperators.greaterThanOrEqual, Global.Operators.and, "", false);
                        _AccessPrimary.NavACQueryDefinition.ACFilterColumns.Add(filterItem);
                    }
                    AccessPrimary.NavACQueryDefinition.SaveToACConfigOff = true;
                    filterItem.SearchWord = _FilterBalanceDateFrom.Value.ToString("o");
                    AccessPrimary.NavACQueryDefinition.SaveToACConfigOff = false;
                }
                else
                {
                    if (filterItem != null)
                        AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItem);
                }
                OnPropertyChanged("FilterBalanceDateFrom");
                Search();
            }
        }

        /// <summary>
        /// The _ current period
        /// </summary>
        Nullable<DateTime> _FilterBalanceDateTo;
        /// <summary>
        /// Gets or sets the current period.
        /// </summary>
        /// <value>The current period.</value>
        [ACPropertyInfo(621, "", "en{'Balance date to'}de{'Bilanzdatum bis'}",
            Description = "End of the balance date range; assigning it re-executes the search.")]
        public Nullable<DateTime> FilterBalanceDateTo
        {
            get
            {
                return _FilterBalanceDateTo;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                _FilterBalanceDateTo = value;
                ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "BalanceDate" && c.LogicalOperator == Global.LogicalOperators.lessThanOrEqual).FirstOrDefault();
                if (_FilterBalanceDateTo.HasValue)
                {
                    if (value.Value.Hour == 0 && value.Value.Minute == 0 && value.Value.Second == 0)
                        _FilterBalanceDateTo = new DateTime(value.Value.Year, value.Value.Month, value.Value.Day, 23, 59, 59);
                    if (filterItem == null)
                    {
                        filterItem = new ACFilterItem(Global.FilterTypes.filter, "BalanceDate", Global.LogicalOperators.lessThanOrEqual, Global.Operators.and, "", false);
                        _AccessPrimary.NavACQueryDefinition.ACFilterColumns.Add(filterItem);
                    }
                    AccessPrimary.NavACQueryDefinition.SaveToACConfigOff = true;
                    filterItem.SearchWord = _FilterBalanceDateTo.Value.ToString("o");
                    AccessPrimary.NavACQueryDefinition.SaveToACConfigOff = false;
                }
                else
                {
                    if (filterItem != null)
                        AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItem);
                }
                OnPropertyChanged("FilterBalanceDateTo");
                Search();
            }
        }

        public gip.mes.datamodel.GlobalApp.TimePeriods FilterTimePeriodValue
        {
            get
            {
                if (FilterSelectedTimePeriod == null) return gip.mes.datamodel.GlobalApp.TimePeriods.Day;
                return (gip.mes.datamodel.GlobalApp.TimePeriods)FilterSelectedTimePeriod.Value;
            }
        }
        #endregion

        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(History.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost,
            Description = "Saves the changes of the current balance sheet to the database.")]
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

        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand(History.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost,
            Description = "Discards the unsaved changes and reloads the current balance sheet.")]
        public void UndoSave()
        {
            OnUndoSave();
            Load();
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
        /// Deletes this instance.
        /// </summary>
        [ACMethodInteraction(History.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentHistory", Global.ACKinds.MSMethodPrePost,
            Description = "Not implemented - deleting balance sheets is disabled (IsEnabledDelete() always returns false).")]
        public void Delete()
        {

        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return false;
        }


        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(History.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search,
            Description = "Re-executes the search and rebuilds the HistoryList; selects the first balance sheet.")]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            SelectedHistory = HistoryList.FirstOrDefault();
            OnPropertyChanged("HistoryList");
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        [ACMethodInteraction(History.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedHistory", Global.ACKinds.MSMethodPrePost,
            Description = "Reloads the selected balance sheet with all detail lists from the database.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<History>(requery, () => SelectedHistory, () => CurrentHistory, c => CurrentHistory = c,
                        DatabaseApp.History
                        .Include(c => c.MaterialHistory_History)
                        .Include(c => c.CompanyMaterialHistory_History)
                        .Include(c => c.FacilityHistory_History)
                        .Where(c => c.HistoryID == SelectedHistory.HistoryID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedHistory != null;
        }

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "Save":
                    Save();
                    return true;
                case "IsEnabledSave":
                    result = IsEnabledSave();
                    return true;
                case "UndoSave":
                    UndoSave();
                    return true;
                case "IsEnabledUndoSave":
                    result = IsEnabledUndoSave();
                    return true;
                case "Delete":
                    Delete();
                    return true;
                case "IsEnabledDelete":
                    result = IsEnabledDelete();
                    return true;
                case "Search":
                    Search();
                    return true;
                case "Load":
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
                case "IsEnabledLoad":
                    result = IsEnabledLoad();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(Load):
                case nameof(Delete):
                case nameof(Search):
                    return new[] { "SelectedHistory", "SelectedCompanyMaterialHistory", "SelectedFacilityHistory", "SelectedMaterialHistory" };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion


    }
}
