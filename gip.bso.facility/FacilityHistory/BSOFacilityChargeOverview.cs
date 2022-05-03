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
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace gip.bso.facility
{
    /// <summary>
    /// Folgende alte Masken sind in diesem BSO enthalten:
    /// 1. Bestandsübersicht von Chargen/Artikeln
    /// 1.1 Artikel Gesamtbestände
    /// 1.2 Artikel Lagerortbestände
    /// 1.3 Artikel Lagerplatzbestände
    /// 1.4 Chargen Gesamtbestände
    /// 1.5 Chargen Lagerortbestände
    /// 1.6 Chargen Lagerplatzbestände
    /// 1.7 Artikelchargen Gesamtbestände
    /// 1.8 Artikelchargen Lagerortbestände
    /// 1.9 Artikelchargen Lagerplatzbestände
    /// Neue Masken:
    /// 1. Bestandsübersicht
    /// ALLE Lagerbuchungen erfolgen immer nur über den FacilityBookingManager.
    /// Dieser ist auch in anderen buchenden Anwendungen zu verwenden.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Quant Overview'}de{'Quantübersicht'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + FacilityCharge.ClassName)]
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

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            this._CurrentFacilityChargeSumFacilityHelper = null;
            this._CurrentFacilityChargeSumMaterialHelper = null;
            this._SelectedFacilityChargeSumFacilityHelper = null;
            this._SelectedFacilityChargeSumLocationHelper = null;
            this._SelectedFacilityChargeSumMaterialHelper = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }

        #endregion

        #region Child (Local BSOs)

        ACChildItem<BSOTandTFastView> _BSOTandTFastView_Child;
        [ACPropertyInfo(590)]
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
        [ACPropertyInfo(804, "Filter", "en{'Material'}de{'Material'}")]
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
        [ACPropertyInfo(805, "Filter", "en{'Storage'}de{'Lagerplatz'}")]
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
        [ACPropertyInfo(806, "Filter", ConstApp.LotNo)]
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

        public const string _CExternLotNoProperty = FacilityLot.ClassName + "\\ExternLotNo";
        [ACPropertyInfo(816, "Filter", ConstApp.ExternLotNo)]
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
        [ACPropertyAccessPrimary(890, FacilityCharge.ClassName)]
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
                        if (navACQueryDefinition.TakeCount == 0)
                            navACQueryDefinition.TakeCount = ACQueryDefinition.C_DefaultTakeCount;
                    }
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<FacilityCharge>(FacilityCharge.ClassName, this);
                    _AccessPrimary.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;
                }
                return _AccessPrimary;
            }
        }

        private IQueryable<FacilityCharge> _AccessPrimary_NavSearchExecuting(IQueryable<FacilityCharge> result)
        {
            ObjectQuery<FacilityCharge> query = result as ObjectQuery<FacilityCharge>;
            if (query != null)
            {
                query.Include("FacilityLotStock_FacilityLot");
                //if (ShowNotAvailable.HasValue)
                //{
                //    query = query.Where(c => c.NotAvailable == ShowNotAvailable.Value) as ObjectQuery<FacilityCharge>;
                //}
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
                    new ACFilterItem(Global.FilterTypes.filter, _CExternLotNoProperty, Global.LogicalOperators.contains, Global.Operators.and, "", true)
                };
            }
        }


        /// <summary>
        /// Gets or sets the selected facility charge.
        /// </summary>
        /// <value>The selected facility charge.</value>
        [ACPropertySelected(801, FacilityCharge.ClassName)]
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
                OnPropertyChanged("SelectedFacilityCharge");
            }
        }

        /// <summary>
        /// Gets or sets the current facility charge.
        /// </summary>
        /// <value>The current facility charge.</value>
        [ACPropertyCurrent(802, FacilityCharge.ClassName)]
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
                OnPropertyChanged("FacilityChargeSumLocationHelperList");
                OnPropertyChanged("FacilityChargeSumFacilityHelperList");
                OnPropertyChanged("FacilityChargeSumMaterialHelperList");
            }
        }

        /// <summary>
        /// Gets the facility charge list.
        /// </summary>
        /// <value>The facility charge list.</value>
        [ACPropertyList(803, FacilityCharge.ClassName)]
        public IEnumerable<FacilityCharge> FacilityChargeList
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.NavList;
            }
        }

        #endregion

        #region SumHelper
        /// <summary>
        /// Gets or sets the current facility charge sum location helper.
        /// </summary>
        /// <value>The current facility charge sum location helper.</value>
        [ACPropertyCurrent(807, "FacilityChargeSumLocationHelper")]
        public FacilityChargeSumLocationHelper CurrentFacilityChargeSumLocationHelper
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the facility charge sum location helper list.
        /// </summary>
        /// <value>The facility charge sum location helper list.</value>
        [ACPropertyList(808, "FacilityChargeSumLocationHelper")]
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
        [ACPropertySelected(809, "FacilityChargeSumLocationHelper")]
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
        [ACPropertyCurrent(810, "FacilityChargeSumFacilityHelper")]
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
        [ACPropertyList(811, "FacilityChargeSumFacilityHelper")]
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
        [ACPropertySelected(812, "FacilityChargeSumFacilityHelper")]
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
        [ACPropertyCurrent(813, "FacilityChargeSumMaterialHelper")]
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
        [ACPropertyList(814, "FacilityChargeSumMaterialHelper")]
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
        [ACPropertySelected(815, "FacilityChargeSumMaterialHelper")]
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
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction(FacilityCharge.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedFacilityCharge", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<FacilityCharge>(requery, () => SelectedFacilityCharge, () => CurrentFacilityCharge, c => CurrentFacilityCharge = c,
                        DatabaseApp.FacilityCharge
                        .Where(c => c.FacilityChargeID == SelectedFacilityCharge.FacilityChargeID));
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
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("FacilityChargeList");
        }

        private bool _SearchFacilityChargeListInProgress;
        [ACMethodInteraction("Deposit", "en{'Filter'}de{'Filtern'}", (short)MISort.Search, true, "FacilityChargeList", Global.ACKinds.MSMethod)]
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
                || name == nameof(FilterExternLotNo))
            {
                Search();
            }
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
                case "Load":
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
                case "IsEnabledLoad":
                    result = IsEnabledLoad();
                    return true;
                case "Search":
                    Search();
                    return true;
                case "SearchFacilityChargeList":
                    SearchFacilityChargeList();
                    return true;
                case "IsEnabledSearchFacilityChargeList":
                    result = IsEnabledSearchFacilityChargeList();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

        #region FacilityBooking(Charge)Overview methods -> Executive methods overrides

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
