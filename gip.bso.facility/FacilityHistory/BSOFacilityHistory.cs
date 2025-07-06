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
using Microsoft.EntityFrameworkCore;

namespace gip.bso.facility
{
    /// <summary>
    /// Folgende alte Masken sind in diesem BSO enthalten:
    /// 1. Lagerhistorie
    /// Neue Masken:
    /// 1. Lagerhistorie
    /// ALLE Lagerbuchungen erfolgen immer nur über den FacilityHistoryManager.
    /// Dieser ist auch in anderen buchenden Anwendungen zu verwenden.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Balance Sheet History'}de{'Bilanzhistorie'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + History.ClassName)]
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


        public override bool ACDeInit(bool deleteACClassTask = false)
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
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
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
        [ACPropertyAccessPrimary(690, History.ClassName)]
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
        [ACPropertySelected(601, History.ClassName)]
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
        [ACPropertyCurrent(602, History.ClassName)]
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
        [ACPropertyList(603, History.ClassName)]
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
        [ACPropertySelected(604, MaterialHistory.ClassName)]
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
        [ACPropertyCurrent(605, MaterialHistory.ClassName)]
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
        [ACPropertyList(606, MaterialHistory.ClassName)]
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
        [ACPropertySelected(607, "CompanyFilter", "en{'Filter by Company'}de{'Filtern nach Unternehmen'}")]
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
        [ACPropertyCurrent(608, "CompanyFilter")]
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
        [ACPropertyList(609, "CompanyFilter")]
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
        [ACPropertySelected(610, CompanyMaterialHistory.ClassName)]
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
        [ACPropertyCurrent(611, CompanyMaterialHistory.ClassName)]
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
        [ACPropertyList(612, CompanyMaterialHistory.ClassName)]
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
        [ACPropertySelected(613, FacilityHistory.ClassName)]
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
        [ACPropertyCurrent(614, FacilityHistory.ClassName)]
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
        [ACPropertyList(615, FacilityHistory.ClassName)]
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
        [ACPropertySelected(616, "TimePeriod", "en{'Time Period'}de{'Zeitspanne'}")]
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
        [ACPropertyCurrent(617, "TimePeriod", "en{'Period'}de{'Periode'}")]
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

        [ACPropertyList(618, "TimePeriod", "en{'Period'}de{'Periode'}")]
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
        [ACPropertyInfo(619, "", "en{'Period no.'}de{'Periodennr.'}")]
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
        [ACPropertyInfo(620, "", "en{'Balance date from'}de{'Bilanzdatum von'}")]
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
        [ACPropertyInfo(621, "", "en{'Balance date to'}de{'Bilanzdatum bis'}")]
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
        [ACMethodCommand(History.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand(History.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction(History.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentHistory", Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand(History.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
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
        [ACMethodInteraction(History.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedHistory", Global.ACKinds.MSMethodPrePost)]
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

        #endregion


    }
}
