// ***********************************************************************
// Assembly         : gip.bso.facility
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 11-08-2012
// ***********************************************************************
// <copyright file="BSOFacilityBookCharge.cs" company="gip mbh, Oftersheim, Germany">
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
using System.ComponentModel;
using System.Data.Objects;
using System.Linq;
using System.Runtime.CompilerServices;
using static gip.core.datamodel.Global;

namespace gip.bso.facility
{
    /// <summary>
    /// BSOFacilityBookCharge dient zur Einlagerung, Umlagerung und Ausbuchung von Quanten
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Quant Management'}de{'Quantenverwaltung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + FacilityCharge.ClassName)]
    [ACQueryInfo(Const.PackName_VarioFacility, Const.QueryPrefix + FacilityLot.ClassName, "en{'Quant Management'}de{'Quantenverwaltung'}", typeof(FacilityLot), FacilityLot.ClassName, "LotNo", "LotNo")]
    public class BSOFacilityBookCharge : BSOFacilityBase
    {

        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityBookCharge"/> class.
        /// </summary>
        /// <param name="typeACClass">The type AC class.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityBookCharge(gip.core.datamodel.ACClass typeACClass, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(typeACClass, content, parentACObject, parameter)
        {
            _ExpirationDateDayPeriod = new ACPropertyConfigValue<int>(this, "ExpirationDateDayPeriod", 30);
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
            bool skipSearchOnStart = ParameterValueT<bool>(Const.SkipSearchOnStart);
            if (!skipSearchOnStart)
                Search();

            _ = ExpirationDateDayPeriod;

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {

            if (_AccessPrimary != null)
                _AccessPrimary.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;
            this._BookParamInwardMovement = null;
            this._BookParamInwardMovementClone = null;
            this._BookParamNotAvailable = null;
            this._BookParamNotAvailableClone = null;
            this._BookParamOutwardMovement = null;
            this._BookParamOutwardMovementClone = null;
            this._BookParamReassignMat = null;
            this._BookParamReassignMatClone = null;
            this._BookParamReassignLot = null;
            this._BookParamReassignLotClone = null;
            this._BookParamReleaseAndLock = null;
            this._BookParamReleaseAndLockClone = null;
            this._BookParamRelocation = null;
            this._BookParamRelocationClone = null;
            this._BookParamSplit = null;
            this._BookParamSplitClone = null;
            this._AccessFacilityLot = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessFacilityLot != null)
            {
                _AccessFacilityLot.ACDeInit(false);
                _AccessFacilityLot = null;
            }
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
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region BSO->ACProperty

        #region ChildBSO

        ACChildItem<BSOFacilityExplorer> _BSOFacilityExplorer_Child;
        [ACPropertyInfo(600)]
        [ACChildInfo("BSOFacilityExplorer_Child", typeof(BSOFacilityExplorer))]
        public ACChildItem<BSOFacilityExplorer> BSOFacilityExplorer_Child
        {
            get
            {
                if (_BSOFacilityExplorer_Child == null)
                    _BSOFacilityExplorer_Child = new ACChildItem<BSOFacilityExplorer>(this, "BSOFacilityExplorer_Child");
                return _BSOFacilityExplorer_Child;
            }
        }

        #endregion

        #region Filters
        public const string _CNotAvailableProperty = "NotAvailable";
        [ACPropertyInfo(712, "Filter", "en{'Show not available'}de{'Nicht verfügbare anzeigen'}")]
        public virtual bool? ShowNotAvailable
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
                    OnPropertyChanged();
                    return;
                }
                if (String.IsNullOrEmpty(tmp)
                    || AccessPrimary.NavACQueryDefinition.GetSearchValue<bool>(_CNotAvailableProperty) != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<bool>(_CNotAvailableProperty, value.Value);
                    OnPropertyChanged();
                    return;
                }
            }
        }


        public const string _CMaterialNoProperty = Material.ClassName + "\\MaterialNo";
        public const string _CMaterialNameProperty = Material.ClassName + "\\MaterialName1";
        [ACPropertyInfo(713, "Filter", "en{'Material'}de{'Material'}")]
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
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CMaterialNoProperty, value);
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CMaterialNameProperty, value);
                    OnPropertyChanged();
                }
            }
        }

        public const string _CFacilityNoProperty = Facility.ClassName + "\\FacilityNo";
        public const string _CFacilityNameProperty = Facility.ClassName + "\\FacilityName";
        [ACPropertyInfo(714, "Filter", "en{'Storage'}de{'Lagerplatz'}")]
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
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CFacilityNoProperty, value);
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CFacilityNameProperty, value);
                    OnPropertyChanged();
                }
            }
        }


        public const string _CLotNoProperty = FacilityLot.ClassName + "\\LotNo";
        [ACPropertyInfo(715, "Filter", ConstApp.LotNo)]
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
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CLotNoProperty, value);
                    OnPropertyChanged();
                }
            }
        }

        public const string _CExternLotNoProperty = FacilityLot.ClassName + "\\" + nameof(FacilityLot.ExternLotNo);
        [ACPropertyInfo(715, "Filter", ConstApp.ExternLotNo)]
        public string FilterExternLot
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
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CExternLotNoProperty, value);
                    OnPropertyChanged();
                }
            }
        }

        public const string _CExternLotNo2Property = FacilityLot.ClassName + "\\" + nameof(FacilityLot.ExternLotNo2);
        [ACPropertyInfo(715, "Filter", ConstApp.ExternLotNo2)]
        public string FilterExternLot2
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
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CExternLotNo2Property, value);
                    OnPropertyChanged();
                }
            }
        }

        [ACPropertyInfo(716, "Filter", "en{'Stock quantity less than'}de{'Lagermenge weniger als'}")]
        public double StockQuantityLessThan
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<double>(nameof(FacilityCharge.StockQuantityUOM));
            }
            set
            {
                double tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<double>(nameof(FacilityCharge.StockQuantityUOM));
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<double>(nameof(FacilityCharge.StockQuantityUOM), value);
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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

        #region BSO->ACProperty->BookingParam
        /// <summary>
        /// Gets the current book param.
        /// </summary>
        /// <value>The current book param.</value>
        [ACPropertyCurrent(701, "BookParam")]
        public ACMethodBooking CurrentBookParam
        {
            get
            {
                return _ActBookingParam;
            }
            protected set
            {
                _ActBookingParam = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current book param inward movement.
        /// </summary>
        /// <value>The current book param inward movement.</value>
        [ACPropertyCurrent(702, "BookParamInwardMovement")]
        public ACMethodBooking CurrentBookParamInwardMovement
        {
            get
            {
                return _BookParamInwardMovement;
            }
            protected set
            {
                _BookParamInwardMovement = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current book param outward movement.
        /// </summary>
        /// <value>The current book param outward movement.</value>
        [ACPropertyCurrent(703, "BookParamOutwardMovement")]
        public ACMethodBooking CurrentBookParamOutwardMovement
        {
            get
            {
                return _BookParamOutwardMovement;
            }
            protected set
            {
                _BookParamOutwardMovement = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current book param relocation.
        /// </summary>
        /// <value>The current book param relocation.</value>
        [ACPropertyCurrent(704, "BookParamRelocation")]
        public ACMethodBooking CurrentBookParamRelocation
        {
            get
            {
                return _BookParamRelocation;
            }
            protected set
            {
                _BookParamRelocation = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current book param release and lock.
        /// </summary>
        /// <value>The current book param release and lock.</value>
        [ACPropertyCurrent(705, "BookParamReleaseAndLock")]
        public ACMethodBooking CurrentBookParamReleaseAndLock
        {
            get
            {
                return _BookParamReleaseAndLock;
            }
            protected set
            {
                _BookParamReleaseAndLock = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current book param not available.
        /// </summary>
        /// <value>The current book param not available.</value>
        [ACPropertyCurrent(706, "BookParamNotAvailable")]
        public ACMethodBooking CurrentBookParamNotAvailable
        {
            get
            {
                return _BookParamNotAvailable;
            }
            protected set
            {
                _BookParamNotAvailable = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current book param matching.
        /// </summary>
        /// <value>The current book param matching.</value>
        [ACPropertyCurrent(707, "CurrentBookParamReassignMat")]
        public ACMethodBooking CurrentBookParamReassignMat
        {
            get
            {
                return _BookParamReassignMat;
            }
            protected set
            {
                _BookParamReassignMat = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current book param matching.
        /// </summary>
        /// <value>The current book param matching.</value>
        [ACPropertyCurrent(707, "CurrentBookParamSplit")]
        public ACMethodBooking CurrentBookParamSplit
        {
            get
            {
                return _BookParamSplit;
            }
            protected set
            {
                _BookParamSplit = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Gets the current book param matching.
        /// </summary>
        /// <value>The current book param matching.</value>
        [ACPropertyCurrent(708, "CurrentBookParamReassignLot")]
        public ACMethodBooking CurrentBookParamReassignLot
        {
            get
            {
                return _BookParamReassignLot;
            }
            protected set
            {
                _BookParamReassignLot = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region BSO->ACProperty->FacilityCharge
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<FacilityCharge> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(790, FacilityCharge.ClassName)]
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

        protected virtual List<ACFilterItem> NavigationqueryDefaultFilter
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
                    new ACFilterItem(Global.FilterTypes.filter, nameof(FacilityCharge.StockQuantityUOM), Global.LogicalOperators.lessThan, Global.Operators.and, "", true),

                    new ACFilterItem(Global.FilterTypes.filter, nameof(FacilityCharge.ExpirationDate), Global.LogicalOperators.lessThanOrEqual, Global.Operators.and, null, true)
                };
            }
        }

        protected virtual IQueryable<FacilityCharge> _AccessPrimary_NavSearchExecuting(IQueryable<FacilityCharge> result)
        {
            ObjectQuery<FacilityCharge> query = result as ObjectQuery<FacilityCharge>;
            query =
                query
                .Include(c => c.Facility)
                .Include(c => c.FacilityLot)
                .Include(c => c.Material)
                .Include(c => c.MDUnit)
                .Include(c => c.MDReleaseState)
                .Include(c => c.CompanyMaterial)
                .Include(c => c.CPartnerCompanyMaterial)
                //.Include(c => c.FacilityBooking_InwardFacilityCharge)
                //.Include(c => c.FacilityBookingCharge_InwardFacilityCharge)
                ;
            return query;
        }


        /// <summary>
        /// Gets or sets the selected facility charge.
        /// </summary>
        /// <value>The selected facility charge.</value>
        [ACPropertySelected(708, FacilityCharge.ClassName, "en{'Facilitycharge'}de{'Chargenplatz'}")]
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
        [ACPropertyCurrent(709, FacilityCharge.ClassName, "en{'Facilitycharge'}de{'Chargenplatz'}")]
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
                if (AccessPrimary.CurrentNavObject != value)
                {

                    RefreshFilterFacilityLotAccess();

                    if (value != null && value.FacilityLot != null)
                    {
                        if (!AccessFacilityLot.NavList.Any(c => c.FacilityLotID == value.FacilityLotID))
                        {
                            AccessFacilityLot.NavList.Add(value.FacilityLot);
                            OnPropertyChanged(nameof(FacilityLotList));
                        }
                        if (AccessPrimary.Current != null)
                        {
                            AccessPrimary.Current.PropertyChanged -= CurrentFacilityCharge_PropertyChanged;
                        }
                    }

                    AccessPrimary.CurrentNavObject = value;

                    if (AccessPrimary.Current != null)
                    {
                        AccessPrimary.Current.PropertyChanged += CurrentFacilityCharge_PropertyChanged;
                    }

                    OnPropertyChanged_CurrentFacilityCharge();
                    OnPropertyChanged(nameof(CurrentFacilityCharge));
                    OnPropertyChanged(nameof(ContractualPartnerList));
                    OnPropertyChanged(nameof(StorageUnitTestList));
                    OnPropertyChanged(nameof(CurrentFacilityLot));
                    CorrectCurrentFacilityChargeFacilityList();
                    ClearBookingData();
                    OnPropertyChanged();
                }
            }
        }

        private void CorrectCurrentFacilityChargeFacilityList()
        {
            AccessQuantFacilityFilter.NavSearch();

            if (CurrentFacilityCharge != null && CurrentFacilityCharge.Facility != null && !AccessQuantFacilityFilter.NavList.Contains(CurrentFacilityCharge.Facility))
            {
                AccessQuantFacilityFilter.NavList.Add(CurrentFacilityCharge.Facility);

            }
            if (CurrentFacilityCharge != null)
            {
                SelectedQuantFacilityFilter = CurrentFacilityCharge.Facility;
            }
            else
            {
                SelectedQuantFacilityFilter = null;
            }
            OnPropertyChanged(nameof(QuantFacilityFilterList));
        }

        public virtual void CurrentFacilityCharge_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FacilityCharge.MaterialID))
            {
                OnPropertyChanged(nameof(StorageUnitTestList));
                OnPropertyChanged(nameof(ContractualPartnerList));
                if (CurrentFacilityCharge.MDUnit == null)
                {
                    CurrentFacilityCharge.MDUnit = CurrentFacilityCharge.Material.BaseMDUnit;
                }
            }

            if (new string[] { nameof(FacilityCharge.MaterialID), nameof(FacilityCharge.FacilityID), nameof(FacilityCharge.FacilityLotID) }.Contains(e.PropertyName))
            {
                OnPropertyChanged(nameof(CurrentFacilityCharge));
            }
        }

        public virtual void OnPropertyChanged_CurrentFacilityCharge()
        {
        }

        private List<FacilityCharge> _FacilityChargeList;
        /// <summary>
        /// Gets or sets the facility charge list.
        /// </summary>
        /// <value>The facility charge list.</value>
        [ACPropertyList(710, FacilityCharge.ClassName)]
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


        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccessNav<FacilityLot> _AccessFacilityLot;
        /// <summary>
        /// Gets the access delivery note pos.
        /// </summary>
        /// <value>The access delivery note pos.</value>
        [ACPropertyAccess(791, nameof(FacilityLot))]
        public ACAccessNav<FacilityLot> AccessFacilityLot
        {
            get
            {
                if (_AccessFacilityLot == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + FacilityLot.ClassName, ACType.ACIdentifier);
                    _AccessFacilityLot = navACQueryDefinition.NewAccessNav<FacilityLot>(Const.QueryPrefix + FacilityLot.ClassName, this);
                    _AccessFacilityLot.AutoSaveOnNavigation = false;
                    _AccessFacilityLot.NavSearch();
                }
                return _AccessFacilityLot;
            }
        }

        /// <summary>
        /// Its invoked from a WPF-Itemscontrol that wants to refresh its CollectionView because the user has changed the LINQ-Expressiontree in the ACQueryDefinition-Property of IAccess. 
        /// The BSO should execute the query on the database first, to get the new results for refreshing the CollectionView of the control.
        /// If the bso don't want to handle this request or manipulate the ACQueryDefinition it returns false. The WPF-control invokes then the IAccess.NavSearch()-Method itself.  
        /// </summary>
        /// <param name="acAccess">Reference to IAccess that contains the changed query (Property NavACQueryDefinition)</param>
        /// <returns>True if the bso has handled this request and queried the database context. Otherwise it returns false.</returns>
        public override bool ExecuteNavSearch(IAccess acAccess)
        {
            if (acAccess == _AccessFacilityLot)
            {
                _AccessFacilityLot.NavSearch();
                OnPropertyChanged(nameof(FacilityLotList));
                return true;
            }
            return base.ExecuteNavSearch(acAccess);
        }


        /// <summary>
        /// Gets the facility lot list.
        /// </summary>
        /// <value>The facility lot list.</value>
        [ACPropertyList(200, nameof(FacilityLot), ConstApp.LotNo)]
        public IEnumerable<FacilityLot> FacilityLotList
        {
            get
            {
                if (AccessFacilityLot == null)
                    return null;
                return AccessFacilityLot.NavList;
            }
        }

        [ACPropertyCurrent(201, nameof(FacilityLot), ConstApp.LotNo)]
        public FacilityLot CurrentFacilityLot
        {
            get
            {
                if (CurrentFacilityCharge == null)
                    return null;
                return CurrentFacilityCharge.FacilityLot;
            }
            set
            {
                if (CurrentFacilityCharge != null && CurrentFacilityCharge.FacilityLot != value)
                {
                    CurrentFacilityCharge.FacilityLot = value;
                    OnPropertyChanged();
                }
            }
        }

        public virtual void RefreshFilterFacilityLotAccess()
        {
            if (_AccessFacilityLot != null)
                _AccessFacilityLot.NavSearch();
            OnPropertyChanged(nameof(FacilityLotList));
        }

        #endregion

        #region Units
        [ACPropertyList(792, "StorageUnit")]
        public IEnumerable<MDUnit> StorageUnitTestList
        {
            get
            {
                if (CurrentFacilityCharge == null)
                    return null;
                if (CurrentFacilityCharge.Material == null)
                    return null;
                return CurrentFacilityCharge.Material.MaterialUnitList;
            }
        }

        [ACPropertyList(793, "")]
        public IEnumerable<Company> ContractualPartnerList
        {
            get
            {
                if (CurrentFacilityCharge == null)
                    return null;
                if (CurrentFacilityCharge.Material == null)
                    return null;
                return CurrentFacilityCharge.Material.CompanyMaterial_Material.Select(c => c.Company);
            }
        }

        #endregion

        #region Message

        private MsgWithDetails _BSOMsg = new MsgWithDetails();
        [ACPropertyInfo(794, "Message")]
        public MsgWithDetails BSOMsg
        {
            get
            {
                return _BSOMsg;
            }
            set
            {
                _BSOMsg = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Material reassignment

        private Material _SelectedRassignmentMaterial;
        [ACPropertySelected(9999, "ReassignmentMaterial", "en{'Suggested material'}de{'Empfohlenes Material'}")]
        public Material SelectedRassignmentMaterial
        {
            get => _SelectedRassignmentMaterial;
            set
            {
                _SelectedRassignmentMaterial = value;
                CurrentBookParamReassignMat.InwardMaterial = _SelectedRassignmentMaterial;
                OnPropertyChanged();
            }
        }

        private IEnumerable<Material> _ReassignemntMaterials;
        [ACPropertyList(9999, "ReassignmentMaterial")]
        public IEnumerable<Material> ReassignemntMaterials
        {
            get
            {
                if (_ReassignemntMaterials == null && CurrentFacilityCharge != null)
                {
                    _ReassignemntMaterials = ACFacilityManager.GetSuggestedReassignmentMaterials(DatabaseApp, CurrentFacilityCharge.Material);
                }
                return _ReassignemntMaterials;
            }
            set
            {
                _ReassignemntMaterials = value;
            }
        }

        #endregion

        #region AccessNav -> QuantFacilityFilter 

        public const string QuantFacilityFilter = "QuantFacilityFilter";

        ACAccess<Facility> _AccessQuantFacilityFilter;
        [ACPropertyAccess(9999, nameof(QuantFacilityFilter))]
        public ACAccess<Facility> AccessQuantFacilityFilter
        {
            get
            {
                if (_AccessQuantFacilityFilter == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + Facility.ClassName, ACType.ACIdentifier);

                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceSortColumnsIfDifferent(QuantFacilityFilterDefaultSort);
                        navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(QuantFacilityFilterDefaultFilter);

                        foreach (ACFilterItem aCFilterItem in navACQueryDefinition.ACFilterColumns)
                        {
                            aCFilterItem.PropertyChanged += QuantFacilityFilterDefaultSort_PropertyChanged;
                        }
                    }

                    _AccessQuantFacilityFilter = navACQueryDefinition.NewAccessNav<Facility>(nameof(QuantFacilityFilter), this);
                }
                return _AccessQuantFacilityFilter;
            }
        }

        public virtual List<ACFilterItem> QuantFacilityFilterDefaultFilter
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, "FacilityNo", Global.LogicalOperators.equal, Global.Operators.and, "", true, true),
                    new ACFilterItem(Global.FilterTypes.filter, "FacilityName", Global.LogicalOperators.contains, Global.Operators.and, "", true, true),
                    new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.or, ((short)FacilityTypesEnum.StorageBin).ToString(), true),
                    new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.or, ((short)FacilityTypesEnum.StorageBinContainer).ToString(), true),
                    new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.or, ((short)FacilityTypesEnum.PreparationBin).ToString(), true),
                    new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                };
            }
        }

        public virtual List<ACSortItem> QuantFacilityFilterDefaultSort
        {
            get
            {
                List<ACSortItem> acSortItems = new List<ACSortItem>();

                ACSortItem acSortPickingNo = new ACSortItem("FacilityNo", SortDirections.ascending, true);
                acSortItems.Add(acSortPickingNo);

                return acSortItems;
            }
        }

        public virtual void QuantFacilityFilterDefaultSort_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ACFilterItem aCFilterItem = sender as ACFilterItem;

        }

        [ACPropertyInfo(9999, nameof(QuantFacilityFilter))]
        public IEnumerable<Facility> QuantFacilityFilterList
        {
            get
            {
                return AccessQuantFacilityFilter.NavList;
            }
        }

        private Facility _SelectedQuantFacilityFilter;
        [ACPropertySelected(9999, nameof(QuantFacilityFilter), ConstApp.FacilityNo)]
        public Facility SelectedQuantFacilityFilter
        {
            get
            {
                return _SelectedQuantFacilityFilter;
            }
            set
            {
                if (_SelectedQuantFacilityFilter != value)
                {
                    _SelectedQuantFacilityFilter = value;
                    if (
                        CurrentFacilityCharge != null
                        &&
                            (
                               (CurrentFacilityCharge.Facility == null && value != null)
                               || (CurrentFacilityCharge.Facility != null && CurrentFacilityCharge.Facility != value)
                            )
                        )
                    {
                        CurrentFacilityCharge.Facility = value;
                    }
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #endregion

        #region BSO->ACMethod
        #region Allgemein
        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(FacilityCharge.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedFecilityCharge", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentFacilityCharge = FacilityCharge.NewACObject(DatabaseApp, null);
            DatabaseApp.FacilityCharge.AddObject(CurrentFacilityCharge);
            ACState = Const.SMNew;
            PostExecute("New");

        }

        /// <summary>
        /// Determines whether [is enabled new].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNew()
        {
            return true;
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
        }

        protected override Msg OnPreSave()
        {
            Msg msg = base.OnPreSave();
            if (msg != null)
                return msg;

            if (CurrentFacilityCharge != null && CurrentFacilityCharge.Material != null && CurrentFacilityCharge.Material.IsLotManaged
                                              && CurrentFacilityCharge.FacilityLot == null)
            {
                if (CurrentFacilityCharge.EntityState != System.Data.EntityState.Added)
                    return msg;

                //Error50552: Lot managed quants must have a lot assigned!!
                return new Msg(this, eMsgLevel.Error, nameof(BSOFacilityBookCharge), nameof(OnPreSave), 693, "Error50552");
            }

            return msg;
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
            return SelectedFacilityCharge != null;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        [ACMethodInteraction(FacilityCharge.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentFacilityCharge", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            return;

            //if (!PreExecute("Delete")) return;
            //if (CurrentFacilityCharge == null)
            //    return;
            //if (AccessPrimary == null)
            //    return;

            //FacilityManager manager = ACFacilityManager as FacilityManager;
            //Global.ACMethodResultState result = manager.DeleteFacilityCharge(CurrentBookParamNotAvailable, CurrentFacilityCharge, false, this.DatabaseApp);
            //if (result <= Global.ACMethodResultState.Succeeded)
            //{
            //    //(manager as ACMethod).ValidMessage;
            //    // BSOMsg.AddDetailMessage((manager as ACMethod).ValidMessage);
            //}

            //ClearBookingData();
            //AccessPrimary.NavigateFirst();
            //PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return SelectedFacilityCharge != null;
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
            }

            OnPropertyChanged(nameof(FacilityChargeList));
        }

        private bool _SearchFacilityChargeListInProgress;
        [ACMethodInteraction("Deposit", "en{'Filter'}de{'Filtern'}", (short)MISort.Search, true, "FacilityChargeList", Global.ACKinds.MSMethod)]
        public virtual void SearchFacilityChargeList()
        {
            _SearchFacilityChargeListInProgress = true;
            AccessNav.NavSearch(MergeOption.OverwriteChanges);
            OnPropertyChanged(nameof(FacilityChargeList));
            _SearchFacilityChargeListInProgress = false;
        }


        public bool IsEnabledSearchFacilityChargeList()
        {
            return !_SearchFacilityChargeListInProgress;
        }


        /// <summary>
        /// Clears the booking data.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'New Booking Record'}de{'Neuer Buchungssatz'}", 701, true)]
        public virtual void ClearBookingData()
        {
            if (_BookParamInwardMovementClone == null)
                _BookParamInwardMovementClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_InwardMovement_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            ACMethodBooking clone = _BookParamInwardMovementClone.Clone() as ACMethodBooking;
            if (CurrentFacilityCharge != null)
            {
                clone.MDUnit = CurrentFacilityCharge.MDUnit;
                clone.InwardFacilityCharge = CurrentFacilityCharge;
            }
            CurrentBookParamInwardMovement = clone;

            if (_BookParamOutwardMovementClone == null)
                _BookParamOutwardMovementClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_OutwardMovement_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamOutwardMovementClone.Clone() as ACMethodBooking;
            if (CurrentFacilityCharge != null)
            {
                clone.MDUnit = CurrentFacilityCharge.MDUnit;
                clone.OutwardFacilityCharge = CurrentFacilityCharge;
            }
            CurrentBookParamOutwardMovement = clone;

            if (_BookParamRelocationClone == null)
                _BookParamRelocationClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_Relocation_FacilityCharge_Facility.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamRelocationClone.Clone() as ACMethodBooking;
            if (CurrentFacilityCharge != null)
            {
                clone.MDUnit = CurrentFacilityCharge.MDUnit;
                clone.OutwardFacilityCharge = CurrentFacilityCharge;
            }
            CurrentBookParamRelocation = clone;

            if (_BookParamReleaseAndLockClone == null)
                _BookParamReleaseAndLockClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ReleaseState_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamReleaseAndLockClone.Clone() as ACMethodBooking;
            if (CurrentFacilityCharge != null)
            {
                clone.InwardFacilityCharge = CurrentFacilityCharge;
            }
            CurrentBookParamReleaseAndLock = clone;

            if (_BookParamNotAvailableClone == null)
                _BookParamNotAvailableClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ZeroStock_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamNotAvailableClone.Clone() as ACMethodBooking;
            if (CurrentFacilityCharge != null)
            {
                clone.InwardFacilityCharge = CurrentFacilityCharge;
            }
            CurrentBookParamNotAvailable = clone;

            if (_BookParamReassignMatClone == null)
                _BookParamReassignMatClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_Reassign_FacilityCharge, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamReassignMatClone.Clone() as ACMethodBooking;
            if (CurrentFacilityCharge != null)
            {
                clone.OutwardFacilityCharge = CurrentFacilityCharge;
            }
            CurrentBookParamReassignMat = clone;
            SelectedRassignmentMaterial = null;
            ReassignemntMaterials = null;

            if (_BookParamSplitClone == null)
                _BookParamSplitClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_Split_FacilityCharge, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamSplitClone.Clone() as ACMethodBooking;
            if (CurrentFacilityCharge != null)
            {
                clone.OutwardFacilityCharge = CurrentFacilityCharge;
            }
            CurrentBookParamSplit = clone;


            if (_BookParamReassignLotClone == null)
                _BookParamReassignLotClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_Reassign_FacilityChargeLot, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamReassignLotClone.Clone() as ACMethodBooking;
            if (CurrentFacilityCharge != null)
            {
                clone.OutwardFacilityCharge = CurrentFacilityCharge;
            }
            CurrentBookParamReassignLot = clone;

        }

        /// <summary>
        /// Determines whether [is enabled clear booking data].
        /// </summary>
        /// <returns><c>true</c> if [is enabled clear booking data]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledClearBookingData()
        {
            return true;
        }
        #endregion

        #region ControlMode
        /// <summary>Called inside the GetControlModes-Method to get the Global.ControlModes from derivations.
        /// This method should be overriden in the derivations to dynmically control the presentation mode depending on the current value which is bound via VBContent</summary>
        /// <param name="vbControl">A WPF-Control that implements IVBContent</param>
        /// <returns>ControlModesInfo</returns>
        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            if (vbControl == null)
                return base.OnGetControlModes(vbControl);

            Global.ControlModes result = base.OnGetControlModes(vbControl);
            if (result < Global.ControlModes.Enabled)
                return result;
            switch (vbControl.VBContent)
            {
                case "CurrentFacilityCharge\\MDUnit":
                case "CurrentFacilityCharge\\Material":
                case "CurrentFacilityCharge\\Facility":
                case "CurrentFacilityCharge\\SplitNo":
                case nameof(SelectedQuantFacilityFilter):
                    if (CurrentFacilityCharge == null || CurrentFacilityCharge.EntityState != System.Data.EntityState.Added)
                        return Global.ControlModes.Disabled;
                    break;
                case "CurrentFacilityCharge\\FacilityLot":
                    if (HasFCHadAnyStock)
                        return Global.ControlModes.Disabled;
                    break;
                case nameof(CurrentFacilityLot):
                    if (HasFCHadAnyStock)
                        return Global.ControlModes.Disabled;
                    break;

            }

            if (!String.IsNullOrEmpty(vbControl.VBContent) && vbControl.VBContent.StartsWith("CurrentBookParam"))
            {
                int pos = vbControl.VBContent.IndexOf('\\');
                if (pos > 0)
                {
                    string methodId = vbControl.VBContent.Substring(0, pos);
                    string acValueId = vbControl.VBContent.Substring(pos + 1);
                    if (!String.IsNullOrEmpty(methodId) && !String.IsNullOrEmpty(acValueId))
                    {
                        ACMethodBooking acMethod = ACUrlCommand(methodId) as ACMethodBooking;
                        if (acMethod != null)
                        {
                            Global.ControlModesInfo subResult = acMethod.OnGetControlModes(vbControl, acValueId);
                            if (subResult.Mode != result)
                            {
                                result = subResult.Mode;
                            }
                        }
                    }
                }
            }

            return result;
        }
        #endregion

        #region Buchung Zugang (InwardMovementUnscheduled)
        /// <summary>
        /// Inwards the facility charge movement.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Post Inward Movement'}de{'Buche Lagerzugang'}", 702, true, Global.ACKinds.MSMethodPrePost)]
        public void InwardFacilityChargeMovement()
        {
            if (!PreExecute(nameof(InwardFacilityChargeMovement)))
                return;

            CurrentBookParamInwardMovement.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamInwardMovement, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamInwardMovement.ValidMessage.IsSucceded() || CurrentBookParamInwardMovement.ValidMessage.HasWarnings())
                Messages.Msg(CurrentBookParamInwardMovement.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
            }

            PostExecute(nameof(InwardFacilityChargeMovement));
        }
        /// <summary>
        /// Determines whether [is enabled inward facility charge movement].
        /// </summary>
        /// <returns><c>true</c> if [is enabled inward facility charge movement]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledInwardFacilityChargeMovement()
        {
            if (CurrentBookParamInwardMovement == null)
                return false;
            bool bRetVal = CurrentBookParamInwardMovement.IsEnabled();
            UpdateBSOMsg();
            return bRetVal;
        }
        #endregion

        #region Buchung Abgang(OutwardMovementUnscheduled)
        /// <summary>
        /// Outwards the facility charge movement.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Post Outward Movement'}de{'Buche Lagerabgang'}", 703, true, Global.ACKinds.MSMethodPrePost)]
        public void OutwardFacilityChargeMovement()
        {
            if (!PreExecute(nameof(OutwardFacilityChargeMovement)))
                return;

            CurrentBookParamOutwardMovement.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamOutwardMovement, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamOutwardMovement.ValidMessage.IsSucceded() || CurrentBookParamOutwardMovement.ValidMessage.HasWarnings())
                Messages.Msg(CurrentBookParamOutwardMovement.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
            }

            PostExecute(nameof(OutwardFacilityChargeMovement));
        }
        /// <summary>
        /// Determines whether [is enabled outward facility charge movement].
        /// </summary>
        /// <returns><c>true</c> if [is enabled outward facility charge movement]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledOutwardFacilityChargeMovement()
        {
            bool bRetVal = CurrentBookParamOutwardMovement.IsEnabled();
            UpdateBSOMsg();
            return bRetVal;
        }
        #endregion

        #region Umlagerung (Relocation)
        /// <summary>
        /// Facilities the charge relocation.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Post Stock Transfer'}de{'Buche Umlagerung'}", 704, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void FacilityChargeRelocation()
        {
            if (!PreExecute("FacilityChargeRelocation")) return;

            if (IsPhysicalTransportPossible)
            {
                Global.MsgResult userQuestionAutomatic = Global.MsgResult.No;
                // Question50035: Do you want to run this relocation/transport on the plant in automatic mode?
                userQuestionAutomatic = Messages.YesNoCancel(this, "Question50035");
                if (userQuestionAutomatic == Global.MsgResult.Yes)
                {
                    gip.core.datamodel.ACClassMethod acClassMethod = null;
                    bool wfRunsBatches = false;
                    if (!PrepareStartWorkflow(CurrentBookParamRelocation, out acClassMethod, out wfRunsBatches))
                    {
                        ClearBookingData();
                        return;
                    }

                    ACMethodBooking booking = CurrentBookParamRelocation;
                    // If Workflow doesn't contain a PWNodeProcessWorkflow => Start relocation directly
                    if (!wfRunsBatches)
                    {
                        if (booking.InwardQuantity.HasValue)
                            booking.InwardQuantity = 0.000001;
                        if (booking.OutwardQuantity.HasValue)
                            booking.OutwardQuantity = 0.000001;
                        BookRelocation();
                        if (booking.FacilityBooking != null)
                        {
                            StartWorkflow(acClassMethod, booking.FacilityBooking);
                        }
                    }
                    // Workflow contains a PWNodeProcessWorkflow => It runs batches => create Picking
                    else
                    {
                        if (ACPickingManager == null)
                        {
                            ClearBookingData();
                            return;
                        }
                        Picking picking = null;
                        MsgWithDetails msgDetails = ACPickingManager.CreateNewPicking(booking, acClassMethod, this.DatabaseApp, this.DatabaseApp.ContextIPlus, true, out picking);
                        if (msgDetails != null && msgDetails.MsgDetailsCount > 0)
                        {
                            Messages.Msg(msgDetails);
                            ClearBookingData();
                            ACUndoChanges();
                            return;
                        }
                        if (picking == null)
                        {
                            UndoSave();
                            ClearBookingData();
                            return;
                        }
                        Save();

                        msgDetails = ACPickingManager.ValidateStart(this.DatabaseApp, this.DatabaseApp.ContextIPlus, picking, null, PARole.ValidationBehaviour.Strict, null, true);
                        if (!msgDetails.IsSucceded())
                        {
                            if (String.IsNullOrEmpty(msgDetails.Message))
                            {
                                // Der Auftrag kann nicht gestartet werden weil:
                                msgDetails.Message = Root.Environment.TranslateMessage(this, "Error50643");
                            }
                            Messages.Msg(msgDetails, Global.MsgResult.OK, eMsgButton.OK);
                            ClearBookingData();
                            return;
                        }
                        else if (msgDetails.HasWarnings())
                        {
                            if (String.IsNullOrEmpty(msgDetails.Message))
                            {
                                //Möchten Sie den Auftrag wirklich starten? Es gibt nämlich folgende Probleme:
                                msgDetails.Message = Root.Environment.TranslateMessage(this, "Question50108");
                            }
                            var userResult = Messages.Msg(msgDetails, Global.MsgResult.No, eMsgButton.YesNo);
                            if (userResult == Global.MsgResult.No || userResult == Global.MsgResult.Cancel)
                                return;
                        }

                        Global.MsgResult openPicking = Global.MsgResult.No;
                        if (OpenPickingBeforeStart)
                        {
                            // Question50035: Do you want to open the picking order before starting the workflow?
                            openPicking = Messages.Question(this, "Question50106");
                        }

                        bool startWorkflow = true;
                        if (openPicking == Global.MsgResult.Yes)
                        {
                            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
                            if (service != null)
                            {
                                PAOrderInfo info = new PAOrderInfo();
                                info.Entities.Add(
                                new PAOrderInfoEntry()
                                {
                                    EntityID = picking.PickingID,
                                    EntityName = Picking.ClassName
                                });
                                service.ShowDialogOrder(this, info);
                                if (info.DialogResult != null && info.DialogResult.SelectedCommand == eMsgButton.OK)
                                    startWorkflow = picking.PickingState != PickingStateEnum.WFActive;
                            }
                        }
                        if (startWorkflow)
                        {
                            StartWorkflow(acClassMethod, picking);
                        }
                    }
                }
                else if (userQuestionAutomatic == Global.MsgResult.No)
                    BookRelocation();
            }
            else
                BookRelocation();

            PostExecute("FacilityChargeRelocation");
        }
        /// <summary>
        /// Determines whether [is enabled facility charge relocation].
        /// </summary>
        /// <returns><c>true</c> if [is enabled facility charge relocation]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledFacilityChargeRelocation()
        {
            CurrentBookParamRelocation.InwardQuantity = CurrentBookParamRelocation.OutwardQuantity;
            bool bRetVal = CurrentBookParamRelocation.IsEnabled();
            UpdateBSOMsg();
            return bRetVal;
        }

        protected override bool BookRelocation()
        {
            if (CurrentBookParamRelocation.FacilityBooking != null)
            {
                if (CurrentBookParamRelocation.FacilityBooking.OutwardMaterial != CurrentBookParamRelocation.OutwardMaterial)
                    CurrentBookParamRelocation.FacilityBooking.OutwardMaterial = CurrentBookParamRelocation.OutwardMaterial;

                if (CurrentBookParamRelocation.FacilityBooking.OutwardFacility != CurrentBookParamRelocation.OutwardFacility)
                    CurrentBookParamRelocation.FacilityBooking.OutwardFacility = CurrentBookParamRelocation.OutwardFacility;

                if (CurrentBookParamRelocation.FacilityBooking.OutwardFacilityLot != CurrentBookParamRelocation.OutwardFacilityLot)
                    CurrentBookParamRelocation.FacilityBooking.OutwardFacilityLot = CurrentBookParamRelocation.OutwardFacilityLot;

                if (CurrentBookParamRelocation.FacilityBooking.InwardMaterial != CurrentBookParamRelocation.InwardMaterial)
                    CurrentBookParamRelocation.FacilityBooking.InwardMaterial = CurrentBookParamRelocation.InwardMaterial;

                if (CurrentBookParamRelocation.FacilityBooking.InwardFacility != CurrentBookParamRelocation.InwardFacility)
                    CurrentBookParamRelocation.FacilityBooking.InwardFacility = CurrentBookParamRelocation.InwardFacility;

                if (CurrentBookParamRelocation.FacilityBooking.InwardFacilityLot != CurrentBookParamRelocation.InwardFacilityLot)
                    CurrentBookParamRelocation.FacilityBooking.InwardFacilityLot = CurrentBookParamRelocation.InwardFacilityLot;
            }

            CurrentBookParamRelocation.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamRelocation, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamRelocation.ValidMessage.IsSucceded() || CurrentBookParamRelocation.ValidMessage.HasWarnings())
            {
                Messages.Msg(CurrentBookParamRelocation.ValidMessage);
                return false;
            }
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
                return false;
            }
            else
            {
                ClearBookingData();
            }

            return true;
        }

        /// <summary>
        /// Source Property: ShowDlgInwardFacility
        /// </summary>
        [ACMethodInfo("ShowDlgToFacility", "en{'Choose facility'}de{'Lager auswählen'}", 999)]
        public void ShowDlgFacilityRelocation()
        {
            if (!IsEnabledShowDlgFacilityRelocation())
                return;

            VBDialogResult dlgResult = BSOFacilityExplorer_Child.Value.ShowDialog(CurrentBookParamRelocation.InwardFacility);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;
                CurrentBookParamRelocation.InwardFacility = facility;
                OnPropertyChanged(nameof(CurrentBookParamRelocation));
            }
        }

        public bool IsEnabledShowDlgFacilityRelocation()
        {
            return CurrentBookParamRelocation != null;
        }

        #endregion

        #region Freigabestatus (ReleaseStateFacility, ReleaseStateMaterial)
        /// <summary>
        /// Locks the facility charge.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Lock'}de{'Sperren'}", 705, true, Global.ACKinds.MSMethodPrePost)]
        public void LockFacilityCharge()
        {
            if (!PreExecute(nameof(LockFacilityCharge)))
                return;

            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.Locked);

            CurrentBookParamReleaseAndLock.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamReleaseAndLock, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamReleaseAndLock.ValidMessage.IsSucceded() || CurrentBookParamReleaseAndLock.ValidMessage.HasWarnings())
                Messages.Msg(CurrentBookParamReleaseAndLock.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                OnPropertyChanged(nameof(CurrentFacilityCharge));
            }
            PostExecute(nameof(LockFacilityCharge));
        }
        /// <summary>
        /// Determines whether [is enabled lock facility charge].
        /// </summary>
        /// <returns><c>true</c> if [is enabled lock facility charge]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLockFacilityCharge()
        {
            if (CurrentFacilityCharge == null)
                return false;
            if (CurrentFacilityCharge.MDReleaseState != null)
            {
                if (CurrentFacilityCharge.MDReleaseState.MDReleaseStateIndex == (int)MDReleaseState.ReleaseStates.Locked)
                    return false;
            }

            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.Locked);
            bool bRetVal = CurrentBookParamReleaseAndLock.IsEnabled();
            CurrentBookParamReleaseAndLock.MDReleaseState = null;
            UpdateBSOMsg();
            return bRetVal;
        }

        /// <summary>
        /// Locks the facility charge absolute.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Lock absolute'}de{'Absolut sperren'}", 706, true, Global.ACKinds.MSMethodPrePost)]
        public void LockFacilityChargeAbsolute()
        {
            if (!PreExecute(nameof(LockFacilityChargeAbsolute)))
                return;


            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.AbsLocked);
            CurrentBookParamReleaseAndLock.AutoRefresh = true;

            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamReleaseAndLock, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamReleaseAndLock.ValidMessage.IsSucceded() || CurrentBookParamReleaseAndLock.ValidMessage.HasWarnings())
                Messages.Msg(CurrentBookParamReleaseAndLock.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                OnPropertyChanged(nameof(CurrentFacilityCharge));
            }

            PostExecute(nameof(LockFacilityChargeAbsolute));
        }

        /// <summary>
        /// Determines whether [is enabled lock facility charge absolute].
        /// </summary>
        /// <returns><c>true</c> if [is enabled lock facility charge absolute]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLockFacilityChargeAbsolute()
        {
            if (CurrentFacilityCharge == null)
                return false;
            if (CurrentFacilityCharge.MDReleaseState != null)
            {
                if (CurrentFacilityCharge.MDReleaseState.MDReleaseStateIndex == (int)MDReleaseState.ReleaseStates.AbsLocked)
                    return false;
            }

            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.AbsLocked);
            bool isEnabled = CurrentBookParamReleaseAndLock.IsEnabled();
            CurrentBookParamReleaseAndLock.MDReleaseState = null;
            UpdateBSOMsg();
            return isEnabled;
        }

        /// <summary>
        /// Releases the facility charge.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Unlock'}de{'Freigeben'}", 707, true, Global.ACKinds.MSMethodPrePost)]
        public void ReleaseFacilityCharge()
        {
            if (!PreExecute(nameof(ReleaseFacilityCharge)))
                return;


            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.Free);
            CurrentBookParamReleaseAndLock.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamReleaseAndLock, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamReleaseAndLock.ValidMessage.IsSucceded() || CurrentBookParamReleaseAndLock.ValidMessage.HasWarnings())
                Messages.Msg(CurrentBookParamReleaseAndLock.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                OnPropertyChanged(nameof(CurrentFacilityCharge));
            }

            PostExecute(nameof(ReleaseFacilityCharge));
        }

        /// <summary>
        /// Determines whether [is enabled release facility charge].
        /// </summary>
        /// <returns><c>true</c> if [is enabled release facility charge]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledReleaseFacilityCharge()
        {
            if (CurrentFacilityCharge == null)
                return false;
            if (CurrentFacilityCharge.MDReleaseState != null)
            {
                if (CurrentFacilityCharge.MDReleaseState.MDReleaseStateIndex == (int)MDReleaseState.ReleaseStates.Free)
                    return false;
            }

            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.Free);
            bool isEnabled = CurrentBookParamReleaseAndLock.IsEnabled();
            CurrentBookParamReleaseAndLock.MDReleaseState = null;
            UpdateBSOMsg();
            return isEnabled;
        }

        /// <summary>
        /// Releases the facility charge absolute.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Unlock absolute'}de{'Absolut freigeben'}", 708, true, Global.ACKinds.MSMethodPrePost)]
        public void ReleaseFacilityChargeAbsolute()
        {
            if (!PreExecute(nameof(ReleaseFacilityChargeAbsolute)))
                return;

            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.AbsFree);
            CurrentBookParamReleaseAndLock.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamReleaseAndLock, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamReleaseAndLock.ValidMessage.IsSucceded() || CurrentBookParamReleaseAndLock.ValidMessage.HasWarnings())
                Messages.Msg(CurrentBookParamReleaseAndLock.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                OnPropertyChanged(nameof(CurrentFacilityCharge));
            }

            PostExecute(nameof(ReleaseFacilityChargeAbsolute));
        }
        /// <summary>
        /// Determines whether [is enabled release facility charge absolute].
        /// </summary>
        /// <returns><c>true</c> if [is enabled release facility charge absolute]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledReleaseFacilityChargeAbsolute()
        {
            if (CurrentFacilityCharge == null)
                return false;
            if (CurrentFacilityCharge.MDReleaseState != null)
            {
                if (CurrentFacilityCharge.MDReleaseState.MDReleaseStateIndex == (int)MDReleaseState.ReleaseStates.AbsFree)
                    return false;
            }

            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.AbsFree);
            bool isEnabled = CurrentBookParamReleaseAndLock.IsEnabled();
            CurrentBookParamReleaseAndLock.MDReleaseState = null;
            UpdateBSOMsg();
            return isEnabled;
        }
        #endregion

        #region Nicht verfügbar (NotAvailableCharge, AvailableCharge, NotAvailableMaterial, AvailableMaterial)
        /// <summary>
        /// Nots the available facility charge.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Quant not available'}de{'Quant nicht verfügbar'}", 709, true, Global.ACKinds.MSMethodPrePost)]
        public void NotAvailableFacilityCharge()
        {
            if (!PreExecute(nameof(NotAvailableFacilityCharge)))
                return;
            BookNotAvailableFacilityCharge(false);
            PostExecute(nameof(NotAvailableFacilityCharge));
        }

        public bool BookNotAvailableFacilityCharge(bool withRefresh)
        {
            CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);
            CurrentBookParamNotAvailable.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamNotAvailable, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamNotAvailable.ValidMessage.IsSucceded() || CurrentBookParamNotAvailable.ValidMessage.HasWarnings())
                Messages.Msg(CurrentBookParamNotAvailable.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                if (withRefresh)
                {
                    AccessNav.NavSearch();
                    OnPropertyChanged(nameof(FacilityChargeList));
                }
                return true;
            }
            return false;
        }


        /// <summary>
        /// Determines whether [is enabled not available facility charge].
        /// </summary>
        /// <returns><c>true</c> if [is enabled not available facility charge]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNotAvailableFacilityCharge()
        {
            if (CurrentFacilityCharge == null)
                return false;
            bool isEnabled = !CurrentFacilityCharge.NotAvailable;
            if (isEnabled)
            {
                CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);
                isEnabled = CurrentBookParamNotAvailable.IsEnabled();
                CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.Off);
                UpdateBSOMsg();
            }
            return isEnabled;
        }

        [ACMethodCommand(FacilityCharge.ClassName, "en{'All Quants not available'}de{'Alle Quanten nicht verfügbar'}", 711, true, Global.ACKinds.MSMethod)]
        public void NotAvailableFacilityChargeAll()
        {
            //Question50087: Are you sure that you want to change the zero stock status for all the quants listed in the Explorer?
            Global.MsgResult qResult = Messages.Question(this, "Question50087", Global.MsgResult.No);
            if (qResult != Global.MsgResult.Yes)
                return;
            if (FacilityChargeList == null)
                return;

            FacilityCharge[] facilityCharges = FacilityChargeList.ToArray();

            ACMethodBooking[] bookingMethods = GetMethodBookingsForNotAvailable(facilityCharges);
            Dictionary<ACMethodBooking, ACMethodEventArgs> results = ACFacilityManager.BookFacilities(bookingMethods, this.DatabaseApp);

            MsgWithDetails msgWithDetails = GetBookingMessages(results);

            if (msgWithDetails.MessageLevel >= eMsgLevel.Warning)
            {
                Messages.Msg(msgWithDetails);
            }

            Search();
        }

        public bool IsEnabledNotAvailableFacilityChargeAll()
        {
            return CurrentFacilityCharge != null && FacilityChargeList != null && FacilityChargeList.Any();
        }

        /// <summary>
        /// Availables the facility charge.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Quant available'}de{'Quant verfügbar'}", 710, true, Global.ACKinds.MSMethodPrePost)]
        public void AvailableFacilityCharge()
        {
            if (!PreExecute("AvailableFacilityCharge")) return;
            BookAvailableFacilityCharge(false);
            PostExecute("AvailableFacilityCharge");
        }

        public bool BookAvailableFacilityCharge(bool withRefresh)
        {
            FacilityBookingCharge fbc = CurrentFacilityCharge?.FacilityBookingCharge_InwardFacilityCharge
                                                              .Where(c => c.FacilityBookingTypeIndex == (short)GlobalApp.FacilityBookingType.ZeroStock_Facility_BulkMaterial)
                                                              .OrderByDescending(c => c.InsertDate)
                                                              .FirstOrDefault();

            //Question50098: Do you want to restore a last stock?
            if (fbc != null && Messages.Question(this, "Question50098") == Global.MsgResult.Yes)
            {
                CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.RestoreQuantityIfNotAvailable);
            }
            else
            {
                CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.ResetIfNotAvailable);
            }

            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamNotAvailable, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamNotAvailable.ValidMessage.IsSucceded() || CurrentBookParamNotAvailable.ValidMessage.HasWarnings())
                Messages.Msg(CurrentBookParamNotAvailable.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                if (withRefresh)
                {
                    AccessNav.NavSearch();
                    OnPropertyChanged(nameof(FacilityChargeList));
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether [is enabled available facility charge].
        /// </summary>
        /// <returns><c>true</c> if [is enabled available facility charge]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAvailableFacilityCharge()
        {
            if (CurrentFacilityCharge == null)
                return false;
            bool isEnabled = CurrentFacilityCharge.NotAvailable;
            if (isEnabled)
            {
                CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.ResetIfNotAvailable);
                isEnabled = CurrentBookParamNotAvailable.IsEnabled();
                CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.Off);
                UpdateBSOMsg();
            }
            return isEnabled;
        }


        [ACMethodCommand(FacilityCharge.ClassName, "en{'All Quants available'}de{'Alle Quanten verfügbar'}", 712, true, Global.ACKinds.MSMethod)]
        public void AvailableFacilityChargeAll()
        {
            //Question50087: Are you sure that you want to change the zero stock status for all the quants listed in the Explorer?
            Global.MsgResult qResult = Messages.Question(this, "Question50087", Global.MsgResult.No);
            if (qResult != Global.MsgResult.Yes)
                return;
            if (FacilityChargeList == null)
                return;

            //Question50098: Do you want to restore a last stock?
            bool restoreLastStock = Messages.Question(this, "Question50098") == Global.MsgResult.Yes;
            FacilityCharge[] facilityCharges = FacilityChargeList.ToArray();

            ACMethodBooking[] bookingMethods = GetMethodBookingsForAvailable(facilityCharges, restoreLastStock);
            Dictionary<ACMethodBooking, ACMethodEventArgs> results = ACFacilityManager.BookFacilities(bookingMethods, this.DatabaseApp);

            MsgWithDetails msgWithDetails = GetBookingMessages(results);

            if (msgWithDetails.MessageLevel >= eMsgLevel.Warning)
            {
                Messages.Msg(msgWithDetails);
            }

            Search();
        }

        public bool IsEnabledAvailableFacilityChargeAll()
        {
            return CurrentFacilityCharge != null && FacilityChargeList != null && FacilityChargeList.Any();
        }

        #endregion

        #region Materialneuzuordnung (Reassignment)
        /// <summary>
        /// Facilities the relocation.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Reassign Material'}de{'Material neu zuordnen'}", 811, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void FacilityReassign()
        {
            if (!PreExecute(nameof(FacilityReassign)))
                return;

            CurrentBookParamReassignMat.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamReassignMat, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamReassignMat.ValidMessage.IsSucceded() || CurrentBookParamReassignMat.ValidMessage.HasWarnings())
                Messages.Msg(CurrentBookParamReassignMat.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
            }

            PostExecute(nameof(FacilityReassign));
        }

        /// <summary>
        /// Determines whether [is enabled facility relocation].
        /// </summary>
        /// <returns><c>true</c> if [is enabled facility relocation]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledFacilityReassign()
        {
            bool bRetVal = CurrentBookParamReassignMat.IsEnabled();
            UpdateBSOMsg();
            return bRetVal;
        }
        #endregion

        #region Lostneuzuordnung (Reassignment Lot)
        /// <summary>
        /// Facilities the relocation.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Reassign Lot'}de{'Los neu zuordnen'}", 811, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void FacilityLotReassign()
        {
            if (!PreExecute(nameof(FacilityLotReassign)))
                return;

            CurrentBookParamReassignLot.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamReassignLot, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamReassignLot.ValidMessage.IsSucceded() || CurrentBookParamReassignLot.ValidMessage.HasWarnings())
                Messages.Msg(CurrentBookParamReassignLot.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
            }

            PostExecute(nameof(FacilityLotReassign));
        }

        /// <summary>
        /// Determines whether [is enabled facility relocation].
        /// </summary>
        /// <returns><c>true</c> if [is enabled facility relocation]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledFacilityLotReassign()
        {
            bool bRetVal = CurrentBookParamReassignLot.IsEnabled();
            UpdateBSOMsg();
            return bRetVal;
        }
        #endregion

        #region SplitQuant
        /// <summary>
        /// Facilities the relocation.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Split Quant'}de{'Quant Splitten'}", 712, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void SplitQuant()
        {
            if (!PreExecute(nameof(SplitQuant)))
                return;


            CurrentBookParamSplit.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamSplit, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamSplit.ValidMessage.IsSucceded() || CurrentBookParamSplit.ValidMessage.HasWarnings())
                Messages.Msg(CurrentBookParamSplit.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
            }

            PostExecute(nameof(SplitQuant));
        }

        /// <summary>
        /// Determines whether [is enabled facility relocation].
        /// </summary>
        /// <returns><c>true</c> if [is enabled facility relocation]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledSplitQuant()
        {
            bool bRetVal = CurrentBookParamSplit.IsEnabled();
            UpdateBSOMsg();
            return bRetVal;
        }
        #endregion

        #region Lot-Handling
        /// <summary>
        /// Updates the BSO MSG.
        /// </summary>
        protected void UpdateBSOMsg()
        {
            // TODO: Bei BSOFacilityBookCharge die Methode UpdateBSOMsg implementieren
            if (CurrentBookParam == null)
                return;
            if (!CurrentBookParam.ValidMessage.IsSucceded() || CurrentBookParam.ValidMessage.HasWarnings())
            {
                if (!BSOMsg.IsEqual(CurrentBookParam.ValidMessage))
                {
                    BSOMsg.UpdateFrom(CurrentBookParam.ValidMessage);
                }
            }
            else
            {
                if (BSOMsg.MsgDetailsCount > 0)
                    BSOMsg.ClearMsgDetails();
            }
        }

        /// <summary>
        /// News the charge no.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'New Charge No.'}de{'Neue Chargennr.'}", 712, true)]
        public void NewChargeNo()
        {
            // TODO:
        }

        /// <summary>
        /// Determines whether [is enabled new charge no].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new charge no]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewChargeNo()
        {
            return true;
        }

        /// <summary>
        /// News the split charge no.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'New Split No.'}de{'Neue Splitnr.'}", 740, true)]
        public void NewSplitChargeNo()
        {
            //CurrentBookParamReleaseAndLock.InwardFacilityCharge.FacilityLot.LotNo = FacilityManager.GenerateSplitChargeNo(CurrentBookParamReleaseAndLock.InwardFacilityCharge);
        }

        /// <summary>
        /// Determines whether [is enabled new split charge no].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new split charge no]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewSplitChargeNo()
        {
            return CurrentBookParamReleaseAndLock.InwardFacilityCharge != null;
        }


        [ACMethodCommand(FacilityCharge.ClassName, "en{'New Lot'}de{'Neues Los'}", 714, true, Global.ACKinds.MSMethodPrePost)]
        public void FacilityChargeLotGenerateDlg()
        {
            if (!IsEnabledFacilityChargeLotGenerateDlg())
                return;
            if (CurrentFacilityCharge.EntityState != System.Data.EntityState.Added && !ACSaveChanges())
                return;
            ACComponent childBSO = ACUrlCommand("?" + ConstApp.BSOFacilityLot_ChildName) as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent(ConstApp.BSOFacilityLot_ChildName, null, new object[] { }) as ACComponent;
            if (childBSO == null) return;
            VBDialogResult dlgResult = (VBDialogResult)childBSO.ACUrlCommand("!" + ConstApp.BSOFacilityLot_Dialog_ShowDialogNewLot, "", CurrentFacilityCharge.Material);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                FacilityLot lot = dlgResult.ReturnValue as FacilityLot;
                _AccessFacilityLot.NavList.Add(lot);
                OnPropertyChanged(nameof(FacilityLotList));
                CurrentFacilityLot = lot;
            }
            if (childBSO != null)
                childBSO.Stop();
        }

        public bool IsEnabledFacilityChargeLotGenerateDlg()
        {
            return CurrentFacilityCharge != null && CurrentFacilityCharge.Material != null && !HasFCHadAnyStock;
        }


        public bool HasFCHadAnyStock
        {
            get
            {
                return CurrentFacilityCharge == null || Math.Abs(CurrentFacilityCharge.StockQuantityUOM) >= double.Epsilon || CurrentFacilityCharge.NotAvailable;
            }
        }
        #endregion

        #region Automatic/Workflow
        public override bool IsPhysicalTransportPossible
        {
            get
            {
                if (CurrentBookParamRelocation == null)
                    return false;
                return CurrentBookParamRelocation.OutwardFacilityCharge != null
                        && CurrentBookParamRelocation.OutwardFacilityCharge.Facility != null
                        && CurrentBookParamRelocation.InwardFacility != null
                        && CurrentBookParamRelocation.InwardFacility.VBiFacilityACClassID.HasValue
                        && HasRightsForPhysicalTransport;
            }
        }
        #endregion

        #region Dialog Navigate

        [ACMethodInteraction("", "en{'Show Order'}de{'Auftrag anzeigen'}", 781, true, nameof(SelectedFacilityCharge))]
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


        [ACMethodInteraction(nameof(NavigateToFacilityChargeHistory), "en{'Show stock history of quant'}de{'Bestandshistorie des Quants anzeigen'}", 782, true, nameof(SelectedFacilityCharge))]
        public void NavigateToFacilityChargeHistory()
        {
            if (!IsEnabledNavigateToFacilityChargeHistory())
            {
                return;
            }

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo() { DialogSelectInfo = 1 };
                info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityCharge), SelectedFacilityCharge.FacilityChargeID));
                info.Entities.Add(new PAOrderInfoEntry(nameof(Facility), SelectedFacilityCharge.FacilityID));
                info.Entities.Add(new PAOrderInfoEntry(nameof(Material), SelectedFacilityCharge.MaterialID));

                if (SelectedFacilityCharge.FacilityLotID != null)
                {
                    info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityLot), SelectedFacilityCharge.FacilityLotID ?? Guid.Empty));
                }

                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacilityChargeHistory()
        {
            return SelectedFacilityCharge != null;
        }


        [ACMethodInteraction("", "en{'Show Material Stock and History'}de{'Zeige Materialbestand und Historie'}", 783, true, nameof(SelectedFacilityCharge))]
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


        [ACMethodInteraction("", "en{'Manage Material'}de{'Verwalte Material'}", 784, true, nameof(SelectedFacilityCharge))]
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


        [ACMethodInteraction("", "en{'Show Lot Stock and History'}de{'Zeige Losbestand und Historie'}", 785, true, nameof(SelectedFacilityCharge))]
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


        [ACMethodInteraction("", "en{'Manage Lot/Batch'}de{'Verwalte Los/Charge'}", 786, true, nameof(SelectedFacilityCharge))]
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
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.Facility != null && SelectedFacilityCharge.Facility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
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
        [ACMethodInfo("SetFilterExpirationDate", "en{'Reset'}de{'Zurücksetzen'}", 999)]
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

        #region Method -> ShowDialogOrderInfo

        [ACMethodInfo("Dialog", "en{'Dialog lot overview'}de{'Dialog Losübersicht'}", (short)MISort.QueryPrintDlg + 1)]
        public virtual void ShowDialogOrderInfo(PAOrderInfo paOrderInfo)
        {
            if (AccessPrimary == null || paOrderInfo == null)
                return;
            DialogOrderInfoPrepareFilter(paOrderInfo);

            Search();

            DialogOrderInfoPreSelectCharge(paOrderInfo);

            ShowDialog(this, "ShowDialogOrderInfoDlg");
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

        #region Eventhandling
        public override void OnPropertyChanged([CallerMemberName] string name = "")
        {
            base.OnPropertyChanged(name);
            if (name == nameof(ShowNotAvailable)
                || name == nameof(FilterMaterial)
                || name == nameof(FilterFacility)
                || name == nameof(FilterLot)
                || name == nameof(FilterExternLot)
                || name == nameof(FilterExternLot2)
                || name == nameof(FilterExpirationDate))
            {
                Search();
            }
        }

        /// <summary>
        /// ACAction is called when one IACInteractiveObject (Source) wants to inform another IACInteractiveObject (Target) about an relevant interaction-event.
        /// </summary>
        /// <param name="actionArgs">Information about the type of interaction and the source</param>
        public override void ACAction(ACActionArgs actionArgs)
        {
            if (actionArgs.ElementAction == Global.ElementActionType.TabItemActivated)
            {
                OnActivate(actionArgs.DropObject.VBContent);
            }
            else
                base.ACAction(actionArgs);
        }

        /// <summary>
        /// Called when [activate].
        /// </summary>
        /// <param name="page">The page.</param>
        [ACMethodInfo(FacilityCharge.ClassName, "en{'Activate'}de{'Aktiviere'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void OnActivate(string page)
        {
            if (!PreExecute("OnActivate"))
                return;
            if (page == "ActivateInwardMovement" || page == "InwardBookingTab" || page == "*InwardBookingTab")
            {
                CurrentBookParam = CurrentBookParamInwardMovement;
            }
            else if (page == "ActivateOutwardMovement" || page == "OutwardBookingTab" || page == "*OutwardBookingTab")
            {
                CurrentBookParam = CurrentBookParamOutwardMovement;
            }
            else if (page == "ActivateRelocation" || page == "RelocationTab" || page == "*RelocationTab")
            {
                CurrentBookParam = CurrentBookParamRelocation;
            }
            else if (page == "ActivateReleaseAndLock" || page == "ReleaseAndLockTab" || page == "*ReleaseAndLockTab")
            {
                CurrentBookParam = CurrentBookParamReleaseAndLock;
            }
            else if (page == "ActivateNotAvailable" || page == "AvailabilityTab" || page == "*AvailabilityTab")
            {
                CurrentBookParam = CurrentBookParamNotAvailable;
            }
            else if (page == "ActivateReassignMat" || page == "ReassignTab" || page == "*ReassignTab")
            {
                CurrentBookParam = CurrentBookParamReassignMat;
            }
            else if (page == "ActivateSplitQuant" || page == "SplitQuantTab" || page == "*SplitQuantTab")
            {
                CurrentBookParam = CurrentBookParamSplit;
            }
            else if (page == "ActivateReassignLot" || page == "ReassignLotTab" || page == "*ReassignLotTab")
            {
                CurrentBookParam = CurrentBookParamReassignLot;
            }
            PostExecute("OnActivate");

        }

        /// <summary>
        /// BSOs the facility book charge_ on value changed.
        /// </summary>
        /// <param name="dataContent">Content of the data.</param>
        void BSOFacilityBookCharge_OnValueChanged(string dataContent)
        {
            switch (dataContent)
            {
                case "CurrentFacilityCharge.Material":
                    if ((CurrentFacilityCharge.Material != null) && DatabaseApp.IsChanged)
                    {
                        if (CurrentFacilityCharge.Material.BaseMDUnit != null)
                            CurrentFacilityCharge.MDUnit = CurrentFacilityCharge.Material.BaseMDUnit;
                        else
                            CurrentFacilityCharge.MDUnit = null;
                    }
                    break;
            }
        }

        #endregion

        #region private members

        /// <summary>
        /// The _ book param inward movement
        /// </summary>
        ACMethodBooking _BookParamInwardMovement;
        ACMethodBooking _BookParamInwardMovementClone;
        /// <summary>
        /// The _ book param outward movement
        /// </summary>
        ACMethodBooking _BookParamOutwardMovement;
        ACMethodBooking _BookParamOutwardMovementClone;
        /// <summary>
        /// The _ book param relocation
        /// </summary>
        ACMethodBooking _BookParamRelocation;
        ACMethodBooking _BookParamRelocationClone;
        /// <summary>
        /// The _ book param release and lock
        /// </summary>
        ACMethodBooking _BookParamReleaseAndLock;
        ACMethodBooking _BookParamReleaseAndLockClone;
        /// <summary>
        /// The _ book param not available
        /// </summary>
        ACMethodBooking _BookParamNotAvailable;
        ACMethodBooking _BookParamNotAvailableClone;

        ACMethodBooking _BookParamReassignMat;
        ACMethodBooking _BookParamReassignMatClone;

        ACMethodBooking _BookParamSplit;
        ACMethodBooking _BookParamSplitClone;

        ACMethodBooking _BookParamReassignLot;
        ACMethodBooking _BookParamReassignLotClone;

        /// <summary>
        /// The _ act booking param
        /// </summary>
        ACMethodBooking _ActBookingParam;

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(New):
                    New();
                    return true;
                case nameof(IsEnabledNew):
                    result = IsEnabledNew();
                    return true;
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
                case nameof(Delete):
                    Delete();
                    return true;
                case nameof(IsEnabledDelete):
                    result = IsEnabledDelete();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
                case nameof(ClearBookingData):
                    ClearBookingData();
                    return true;
                case nameof(IsEnabledClearBookingData):
                    result = IsEnabledClearBookingData();
                    return true;
                case nameof(InwardFacilityChargeMovement):
                    InwardFacilityChargeMovement();
                    return true;
                case nameof(IsEnabledInwardFacilityChargeMovement):
                    result = IsEnabledInwardFacilityChargeMovement();
                    return true;
                case nameof(OutwardFacilityChargeMovement):
                    OutwardFacilityChargeMovement();
                    return true;
                case nameof(IsEnabledOutwardFacilityChargeMovement):
                    result = IsEnabledOutwardFacilityChargeMovement();
                    return true;
                case nameof(FacilityChargeRelocation):
                    FacilityChargeRelocation();
                    return true;
                case nameof(IsEnabledFacilityChargeRelocation):
                    result = IsEnabledFacilityChargeRelocation();
                    return true;
                case nameof(LockFacilityCharge):
                    LockFacilityCharge();
                    return true;
                case nameof(IsEnabledLockFacilityCharge):
                    result = IsEnabledLockFacilityCharge();
                    return true;
                case nameof(LockFacilityChargeAbsolute):
                    LockFacilityChargeAbsolute();
                    return true;
                case nameof(IsEnabledLockFacilityChargeAbsolute):
                    result = IsEnabledLockFacilityChargeAbsolute();
                    return true;
                case nameof(ReleaseFacilityCharge):
                    ReleaseFacilityCharge();
                    return true;
                case nameof(IsEnabledReleaseFacilityCharge):
                    result = IsEnabledReleaseFacilityCharge();
                    return true;
                case nameof(ReleaseFacilityChargeAbsolute):
                    ReleaseFacilityChargeAbsolute();
                    return true;
                case nameof(IsEnabledReleaseFacilityChargeAbsolute):
                    result = IsEnabledReleaseFacilityChargeAbsolute();
                    return true;
                case nameof(NotAvailableFacilityCharge):
                    NotAvailableFacilityCharge();
                    return true;
                case nameof(IsEnabledNotAvailableFacilityCharge):
                    result = IsEnabledNotAvailableFacilityCharge();
                    return true;
                case nameof(AvailableFacilityCharge):
                    AvailableFacilityCharge();
                    return true;
                case nameof(IsEnabledAvailableFacilityCharge):
                    result = IsEnabledAvailableFacilityCharge();
                    return true;
                case nameof(FacilityReassign):
                    FacilityReassign();
                    return true;
                case nameof(IsEnabledFacilityReassign):
                    result = IsEnabledFacilityReassign();
                    return true;
                case nameof(FacilityLotReassign):
                    FacilityLotReassign();
                    return true;
                case nameof(IsEnabledFacilityLotReassign):
                    result = IsEnabledFacilityLotReassign();
                    return true;
                case nameof(SplitQuant):
                    SplitQuant();
                    return true;
                case nameof(IsEnabledSplitQuant):
                    result = IsEnabledSplitQuant();
                    return true;
                case nameof(NewChargeNo):
                    NewChargeNo();
                    return true;
                case nameof(IsEnabledNewChargeNo):
                    result = IsEnabledNewChargeNo();
                    return true;
                case nameof(NewSplitChargeNo):
                    NewSplitChargeNo();
                    return true;
                case nameof(IsEnabledNewSplitChargeNo):
                    result = IsEnabledNewSplitChargeNo();
                    return true;
                case nameof(FacilityChargeLotGenerateDlg):
                    FacilityChargeLotGenerateDlg();
                    return true;
                case nameof(IsEnabledFacilityChargeLotGenerateDlg):
                    result = IsEnabledFacilityChargeLotGenerateDlg();
                    return true;
                case nameof(OnActivate):
                    OnActivate((String)acParameter[0]);
                    return true;
                case nameof(SearchFacilityChargeList):
                    SearchFacilityChargeList();
                    return true;
                case nameof(IsEnabledSearchFacilityChargeList):
                    result = IsEnabledSearchFacilityChargeList();
                    return true;
                case nameof(NotAvailableFacilityChargeAll):
                    NotAvailableFacilityChargeAll();
                    return true;
                case nameof(IsEnabledNotAvailableFacilityChargeAll):
                    result = IsEnabledNotAvailableFacilityChargeAll();
                    return true;
                case nameof(AvailableFacilityChargeAll):
                    AvailableFacilityChargeAll();
                    return true;
                case nameof(IsEnabledAvailableFacilityChargeAll):
                    result = IsEnabledAvailableFacilityChargeAll();
                    return true;
                case nameof(NavigateToOrder):
                    NavigateToOrder();
                    return true;
                case nameof(IsEnabledNavigateToOrder):
                    result = IsEnabledNavigateToOrder();
                    return true;
                case nameof(NavigateToFacilityLot):
                    NavigateToFacilityLot();
                    return true;
                case nameof(IsEnabledNavigateToFacilityLot):
                    result = IsEnabledNavigateToFacilityLot();
                    return true;
                case nameof(NavigateToFacilityLotOverview):
                    NavigateToFacilityLotOverview();
                    return true;
                case nameof(IsEnabledNavigateToFacilityLotOverview):
                    result = IsEnabledNavigateToFacilityLotOverview();
                    return true;
                case nameof(NavigateToFacilityChargeHistory):
                    NavigateToFacilityChargeHistory();
                    return true;
                case nameof(IsEnabledNavigateToFacilityChargeHistory):
                    result = IsEnabledNavigateToFacilityChargeHistory();
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
                case nameof(NavigateToFacilityOverview):
                    NavigateToFacilityOverview();
                    return true;
                case nameof(IsEnabledNavigateToFacilityOverview):
                    result = IsEnabledNavigateToFacilityOverview();
                    return true;
                case nameof(NavigateToMaterial):
                    NavigateToMaterial();
                    return true;
                case nameof(IsEnabledNavigateToMaterial):
                    result = IsEnabledNavigateToMaterial();
                    return true;
                case nameof(NavigateToMaterialOverview):
                    NavigateToMaterialOverview();
                    return true;
                case nameof(IsEnabledNavigateToMaterialOverview):
                    result = IsEnabledNavigateToMaterialOverview();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

        #region IACPrintPrepare

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

        protected override IEnumerable<core.datamodel.ACClassDesign> OnGetDefaultPrintDesigns(string printerName, PAOrderInfo paOrderInfo = null)
        {
            return base.OnGetDefaultPrintDesigns(printerName, paOrderInfo);
        }
        #endregion

        #region Overrides

        public override PAOrderInfo GetOrderInfo()
        {
            PAOrderInfo pAOrderInfo = new PAOrderInfo();
            if (SelectedFacilityCharge != null)
                pAOrderInfo.Add(FacilityCharge.ClassName, SelectedFacilityCharge.FacilityChargeID);
            return pAOrderInfo;
        }

        public override object Clone()
        {
            BSOFacilityBookCharge fc = base.Clone() as BSOFacilityBookCharge;
            if (fc != null)
            {
                fc.CurrentFacilityCharge = this.CurrentFacilityCharge;
            }
            return fc;
        }

        //public override bool IsPoolable
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}
        #endregion

        #region All FacilityCharge availability

        #region All FacilityCharge availability -> Not available

        private ACMethodBooking[] GetMethodBookingsForNotAvailable(FacilityCharge[] facilityCharges)
        {
            List<ACMethodBooking> result = new List<ACMethodBooking>();
            foreach (FacilityCharge facilityCharge in facilityCharges)
            {
                if (!facilityCharge.NotAvailable)
                {
                    ACMethodBooking aCMethodBooking = GetMethodBookingForNotAvailable(facilityCharge);
                    result.Add(aCMethodBooking);
                }
            }
            return result.ToArray();
        }

        private ACMethodBooking GetMethodBookingForNotAvailable(FacilityCharge facilityCharge)
        {
            ACMethodBooking bookParamNotAvailable = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ZeroStock_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            bookParamNotAvailable.InwardFacilityCharge = facilityCharge;
            bookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);
            bookParamNotAvailable.AutoRefresh = true;
            return bookParamNotAvailable;
        }

        #endregion
        #region All FacilityCharge availability -> Available

        private ACMethodBooking[] GetMethodBookingsForAvailable(FacilityCharge[] facilityCharges, bool restoreLastStock)
        {
            List<ACMethodBooking> result = new List<ACMethodBooking>();
            foreach (FacilityCharge facilityCharge in facilityCharges)
            {
                if (facilityCharge.NotAvailable)
                {
                    ACMethodBooking aCMethodBooking = GetMethodBookingForAvailable(facilityCharge, restoreLastStock);
                    result.Add(aCMethodBooking);
                }
            }
            return result.ToArray();
        }

        private ACMethodBooking GetMethodBookingForAvailable(FacilityCharge facilityCharge, bool restoreLastStock)
        {
            FacilityBookingCharge fbc =
                facilityCharge?.FacilityBookingCharge_InwardFacilityCharge
                                .Where(c => c.FacilityBookingTypeIndex == (short)GlobalApp.FacilityBookingType.ZeroStock_Facility_BulkMaterial)
                                .OrderByDescending(c => c.InsertDate)
                                .FirstOrDefault();

            ACMethodBooking bookParamNotAvailable = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ZeroStock_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            bookParamNotAvailable.InwardFacilityCharge = facilityCharge;
            bookParamNotAvailable.AutoRefresh = true;

            if (fbc != null && restoreLastStock)
            {
                bookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.RestoreQuantityIfNotAvailable);
            }
            else
            {
                bookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.ResetIfNotAvailable);
            }

            return bookParamNotAvailable;
        }


        #endregion

        #region All FacilityCharge availability -> Messages

        private MsgWithDetails GetBookingMessages(Dictionary<ACMethodBooking, ACMethodEventArgs> results)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();
            foreach (KeyValuePair<ACMethodBooking, ACMethodEventArgs> item in results)
            {
                MsgWithDetails msg = GetBookingMessage(item.Key, item.Value);
                if (msg != null)
                {
                    msgWithDetails.AddDetailMessage(msg);
                }
            }
            return msgWithDetails;
        }

        private MsgWithDetails GetBookingMessage(ACMethodBooking aCMethodBooking, ACMethodEventArgs result)
        {
            MsgWithDetails msgWithDetails = null;
            if (!aCMethodBooking.ValidMessage.IsSucceded() || aCMethodBooking.ValidMessage.HasWarnings())
            {
                msgWithDetails = aCMethodBooking.ValidMessage;
            }
            else if (result != null && (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible))
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                {
                    result.ValidMessage.Message = result.ResultState.ToString();
                }
                msgWithDetails = result.ValidMessage;
            }

            return msgWithDetails;
        }

        #endregion

        #endregion
    }

}
