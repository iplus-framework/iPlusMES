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

        #region Configuration

        private ACPropertyConfigValue<int> _ExpirationDateDayPeriod;
        [ACPropertyConfig("en{'Expiration date filter period'}de{'Ablaufdatum Filterperiode'}")]
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
        [ACPropertyConfig("en{'Disable include'}de{'Include deaktivieren'}")]
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

        ACChildItem<BSOFacilityReservationOverview> _BSOFacilityReservationOverview_Child;
        [ACPropertyInfo(600)]
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

        public const string _CExternLotNoProperty = FacilityLot.ClassName + "\\" + nameof(FacilityLot.ExternLotNo);
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

        public const string _CExternLotNo2Property = FacilityLot.ClassName + "\\" + nameof(FacilityLot.ExternLotNo2);
        [ACPropertyInfo(817, "Filter", ConstApp.ExternLotNo2)]
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

        [ACPropertyInfo(717, "Filter", "en{'Expiration date less then'}de{'Mindesthaltbarkeitsdatum dann'}")]
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
        [ACPropertyInfo(999, "SetFilterExpirationDateNextDaysCaption", "en{'TODO:FilterExpirationDateDayPeriod'}de{'TODO:FilterExpirationDateDayPeriod'}")]
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
                //    query = query.Where(c => c.NotAvailable == ShowNotAvailable.Value) as ObjectQuery<FacilityCharge>;
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
                OnPropertyChanged();
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
        [ACPropertyList(803, FacilityCharge.ClassName)]
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

        #region BSO->ACMethod->Save&Search

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
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
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
        [ACMethodInfo("SetFilterExpirationDate", "en{'Today'}de{'Heute'}", 999)]
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
        [ACMethodInfo("SetFilterExpirationDate", "en{'Next [0] days'}de{'Nächste [0] Tage'}", 999)]
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
        [ACMethodInfo("SetFilterExpirationDate", "en{'Reset'}de{'Umsetzen'}", 999)]
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

        [ACMethodInteraction("", ConstApp.ShowProdOrder, 781, true, nameof(SelectedFacilityCharge))]
        public void NavigateToOrder()
        {
            if (!IsEnabledNavigateToOrder())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(ProdOrderPartslistPos), SelectedFacilityCharge.FinalPositionFromFbc.ProdOrderPartslistPosID));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToOrder()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.FinalPositionFromFbc != null)
                return true;
            return false;
        }

        [ACMethodInteraction(nameof(NavigateToFacilityCharge), "en{'Manage Quant'}de{'Quant verwalten'}", 782, true, nameof(SelectedFacilityCharge))]
        public void NavigateToFacilityCharge()
        {
            if (!IsEnabledNavigateToFacilityCharge())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo() { DialogSelectInfo = 0 };
                info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityCharge), SelectedFacilityCharge.FacilityChargeID));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacilityCharge()
        {
            return SelectedFacilityCharge != null;
        }


        [ACMethodInteraction("", "en{'Show Lot Stock and History'}de{'Zeige Losbestand und Historie'}", 783, true, nameof(SelectedFacilityCharge))]
        public void NavigateToFacilityLotOverview()
        {
            if (!IsEnabledNavigateToFacilityLotOverview())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo() { DialogSelectInfo = 1 };
                info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityLot), SelectedFacilityCharge.FacilityLotID ?? Guid.Empty));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacilityLotOverview()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.FacilityLot != null)
                return true;
            return false;
        }

        [ACMethodInteraction("", "en{'Manage Lot/Batch'}de{'Verwalte Los/Charge'}", 784, true, nameof(SelectedFacilityCharge))]
        public void NavigateToFacilityLot()
        {
            if (!IsEnabledNavigateToFacilityLot())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityLot), SelectedFacilityCharge.FacilityLotID ?? Guid.Empty));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacilityLot()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.FacilityLot != null)
                return true;
            return false;
        }


        [ACMethodInteraction("", "en{'Show Material Stock and History'}de{'Zeige Materialbestand und Historie'}", 785, true, nameof(SelectedFacilityCharge))]
        public void NavigateToMaterialOverview()
        {
            if (!IsEnabledNavigateToMaterialOverview())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo() { DialogSelectInfo = 1 };
                info.Entities.Add(new PAOrderInfoEntry(nameof(Material), SelectedFacilityCharge.MaterialID));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToMaterialOverview()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.Material != null)
                return true;
            return false;
        }

        [ACMethodInteraction("", "en{'Manage Material'}de{'Verwalte Material'}", 786, true, nameof(SelectedFacilityCharge))]
        public void NavigateToMaterial()
        {
            if (!IsEnabledNavigateToMaterial())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(Material), SelectedFacilityCharge.MaterialID));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToMaterial()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.Material != null)
                return true;
            return false;
        }


        [ACMethodInteraction("", "en{'Show Bin Stock and History'}de{'Zeige Behälterbestand und Historie'}", 787, true, nameof(SelectedFacilityCharge))]
        public void NavigateToFacilityOverview()
        {
            if (!IsEnabledNavigateToFacilityOverview())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo() { DialogSelectInfo = 1 };
                info.Entities.Add(new PAOrderInfoEntry(nameof(Facility), SelectedFacilityCharge.FacilityID));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacilityOverview()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.Facility != null)
                return true;
            return false;
        }

        [ACMethodInteraction("", "en{'Manage Stock of Bin'}de{'Verwalte Behälterbestand'}", 788, true, nameof(SelectedFacilityCharge))]
        public void NavigateToFacility()
        {
            if (!IsEnabledNavigateToFacility())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(Facility), SelectedFacilityCharge.FacilityID));
                service.ShowDialogOrder(this, info);
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
        [ACMethodInfo("Dialog", "en{'Dialog lot overview'}de{'Dialog Losübersicht'}", (short)MISort.QueryPrintDlg + 1)]
        public virtual void ShowDialogOrderInfo(PAOrderInfo paOrderInfo)
        {
            if (AccessPrimary == null || paOrderInfo == null)
                return;
            DialogOrderInfoPrepareFilter(paOrderInfo);

            Search();

            DialogOrderInfoPreSelectCharge(paOrderInfo);

            ShowDialog(this, "ShowDlgOrderInfo");
            this.ParentACComponent.StopComponent(this);
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
                    NavigateToFacilityLotOverview();
                    return true;
                case nameof(IsEnabledNavigateToFacilityLotOverview):
                    result = IsEnabledNavigateToFacilityLotOverview();
                    return true;
                case nameof(NavigateToMaterialOverview):
                    NavigateToMaterialOverview();
                    return true;
                case nameof(IsEnabledNavigateToMaterialOverview):
                    result = IsEnabledNavigateToMaterialOverview();
                    return true;
                case nameof(NavigateToFacilityOverview):
                    NavigateToFacilityOverview();
                    return true;
                case nameof(IsEnabledNavigateToFacilityOverview):
                    result = IsEnabledNavigateToFacilityOverview();
                    return true;
                case nameof(NavigateToFacilityCharge):
                    NavigateToFacilityCharge();
                    return true;
                case nameof(IsEnabledNavigateToFacilityCharge):
                    result = IsEnabledNavigateToFacilityCharge();
                    return true;
                case nameof(ShowDialogOrderInfo):
                    ShowDialogOrderInfo((gip.core.autocomponent.PAOrderInfo)acParameter[0]);
                    return true;
                case nameof(NavigateToFacility):
                    NavigateToFacility();
                    return true;
                case nameof(IsEnabledNavigateToFacility):
                    result = IsEnabledNavigateToFacility();
                    return true;
                case nameof(NavigateToMaterial):
                    NavigateToMaterial();
                    return true;
                case nameof(IsEnabledNavigateToMaterial):
                    result = IsEnabledNavigateToMaterial();
                    return true;
                case nameof(NavigateToFacilityLot):
                    NavigateToFacilityLot();
                    return true;
                case nameof(IsEnabledNavigateToFacilityLot):
                    result = IsEnabledNavigateToFacilityLot();
                    return true;
                case nameof(NavigateToOrder):
                    NavigateToOrder();
                    return true;
                case nameof(IsEnabledNavigateToOrder):
                    result = IsEnabledNavigateToOrder();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
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
