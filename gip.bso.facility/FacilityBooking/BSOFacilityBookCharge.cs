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
using System.Text;

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
                    OnPropertyChanged("FilterMaterial");
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
                    OnPropertyChanged("FilterFacility");
                }
            }
        }


        public const string _CLotNoProperty = FacilityLot.ClassName + "\\LotNo";
        [ACPropertyInfo(715, "Filter", "en{'Lot'}de{'Los'}")]
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
                    OnPropertyChanged("FilterLot");
                }
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
                OnPropertyChanged("CurrentBookParam");
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
                OnPropertyChanged("CurrentBookParamInwardMovement");
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
                OnPropertyChanged("CurrentBookParamOutwardMovement");
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
                OnPropertyChanged("CurrentBookParamRelocation");
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
                OnPropertyChanged("CurrentBookParamReleaseAndLock");
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
                OnPropertyChanged("CurrentBookParamNotAvailable");
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
                OnPropertyChanged("CurrentBookParamReassignMat");
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
                OnPropertyChanged("CurrentBookParamSplit");
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
                        if (navACQueryDefinition.TakeCount == 0)
                            navACQueryDefinition.TakeCount = ACQueryDefinition.C_DefaultTakeCount;
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

                    new ACFilterItem(Global.FilterTypes.filter, _CLotNoProperty, Global.LogicalOperators.contains, Global.Operators.and, "", true, true)
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
                .Include(c => c.Material);
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
                OnPropertyChanged("SelectedFacilityCharge");
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
                    if (CurrentFacilityCharge != null)
                        CurrentFacilityCharge.PropertyChanged -= CurrentFacilityCharge_PropertyChanged;

                    AccessPrimary.CurrentNavObject = value;

                    if (value != null)
                    {
                        value.PropertyChanged -= CurrentFacilityCharge_PropertyChanged;
                        value.PropertyChanged += CurrentFacilityCharge_PropertyChanged;
                    }

                    OnPropertyChanged_CurrentFacilityCharge();
                    OnPropertyChanged(nameof(CurrentFacilityCharge));
                    OnPropertyChanged(nameof(ContractualPartnerList));
                    OnPropertyChanged(nameof(ContractualPartnerList));
                    OnPropertyChanged(nameof(StorageUnitTestList));
                    RefreshFilterFacilityLotAccess();
                    ClearBookingData();
                }
            }
        }

        public virtual void OnPropertyChanged_CurrentFacilityCharge()
        {
        }

        void CurrentFacilityCharge_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MaterialID")
            {
                OnPropertyChanged("StorageUnitTestList");
                OnPropertyChanged("ContractualPartnerList");
                RefreshFilterFacilityLotAccess();
                OnPropertyChanged("CurrentFacilityCharge");
            }
            if (e.PropertyName == "FacilityLotID")
            {
                OnPropertyChanged("CurrentFacilityCharge");
            }
            if (e.PropertyName == "FacilityID")
            {
                OnPropertyChanged("CurrentFacilityCharge");
            }
        }

        /// <summary>
        /// Gets or sets the facility charge list.
        /// </summary>
        /// <value>The facility charge list.</value>
        [ACPropertyList(710, FacilityCharge.ClassName)]
        public IEnumerable<FacilityCharge> FacilityChargeList
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.NavList;
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
        [ACPropertyAccess(791, "FacilityChargeLotList")]
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
                    //RefreshFilterFacilityLotAccess();
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
                OnPropertyChanged("FacilityChargeLotList");
                return true;
            }
            return base.ExecuteNavSearch(acAccess);
        }


        /// <summary>
        /// Gets the facility lot list.
        /// </summary>
        /// <value>The facility lot list.</value>
        [ACPropertyList(711, "FacilityChargeLotList")]
        public IEnumerable<FacilityLot> FacilityChargeLotList
        {
            get
            {
                if (AccessFacilityLot == null)
                    return null;
                return AccessFacilityLot.NavList;
            }
        }

        public virtual void RefreshFilterFacilityLotAccess()
        {
            if (_AccessFacilityLot != null)
                _AccessFacilityLot.NavSearch();
            OnPropertyChanged("FacilityChargeLotList");
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
                OnPropertyChanged("BSOMsg");
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

                //Error50552 The quants with a lot managed material must have a lot!
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
            return SelectedFacilityCharge != null;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        [ACMethodInteraction(FacilityCharge.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentFacilityCharge", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            if (CurrentFacilityCharge == null)
                return;
            if (AccessPrimary == null)
                return;

            FacilityManager manager = ACFacilityManager as FacilityManager;
            Global.ACMethodResultState result = manager.DeleteFacilityCharge(CurrentBookParamNotAvailable, CurrentFacilityCharge, false, this.DatabaseApp);
            if (result <= Global.ACMethodResultState.Succeeded)
            {
                //(manager as ACMethod).ValidMessage;
                // BSOMsg.AddDetailMessage((manager as ACMethod).ValidMessage);
            }

            ClearBookingData();
            AccessPrimary.NavigateFirst();
            PostExecute("Delete");
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
            AccessPrimary.NavSearch(DatabaseApp, MergeOption.OverwriteChanges);
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

            if (_BookParamSplitClone == null)
                _BookParamSplitClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_Split_FacilityCharge, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamSplitClone.Clone() as ACMethodBooking;
            if (CurrentFacilityCharge != null)
            {
                clone.OutwardFacilityCharge = CurrentFacilityCharge;
            }
            CurrentBookParamSplit = clone;

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
                    if (CurrentFacilityCharge == null || CurrentFacilityCharge.EntityState != System.Data.EntityState.Added)
                        return Global.ControlModes.Disabled;
                    break;
                case "CurrentFacilityCharge\\FacilityLot":
                    if (IsFacilityChargeWithFacilityBooking)
                    {
                        return Global.ControlModes.Disabled;
                    }
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
            if (!PreExecute("InwardFacilityChargeMovement")) return;
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
                ClearBookingData();
            PostExecute("InwardFacilityChargeMovement");
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
            if (!PreExecute("OutwardFacilityChargeMovement")) return;
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
                ClearBookingData();
            PostExecute("OutwardFacilityChargeMovement");
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
                        msgDetails = ACPickingManager.ValidateStart(this.DatabaseApp, this.DatabaseApp.ContextIPlus, picking, null, PARole.ValidationBehaviour.Strict);
                        if (msgDetails != null && msgDetails.MsgDetailsCount > 0)
                        {
                            Messages.Msg(msgDetails);
                            ClearBookingData();
                            return;
                        }
                        StartWorkflow(acClassMethod, picking);
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
                ClearBookingData();
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
                OnPropertyChanged("CurrentBookParamRelocation");
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
            if (!PreExecute("LockFacilityCharge")) return;
            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.Locked);
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
                OnPropertyChanged("CurrentFacilityCharge");
            }
            PostExecute("LockFacilityCharge");
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
            if (!PreExecute("LockFacilityChargeAbsolute")) return;
            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.AbsLocked);
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
                OnPropertyChanged("CurrentFacilityCharge");
            }
            PostExecute("LockFacilityChargeAbsolute");
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
            if (!PreExecute("ReleaseFacilityCharge")) return;
            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.Free);
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
                OnPropertyChanged("CurrentFacilityCharge");
            }
            PostExecute("ReleaseFacilityCharge");
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
            if (!PreExecute("ReleaseFacilityChargeAbsolute")) return;
            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.AbsFree);
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
                OnPropertyChanged("CurrentFacilityCharge");
            }
            PostExecute("ReleaseFacilityChargeAbsolute");
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
            if (!PreExecute("NotAvailableFacilityCharge"))
                return;
            BookNotAvailableFacilityCharge(false);
            PostExecute("NotAvailableFacilityCharge");
        }

        public bool BookNotAvailableFacilityCharge(bool withRefresh)
        {
            CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);
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
                    OnPropertyChanged("FacilityChargeList");
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

        /// <summary>
        /// Availables the facility charge.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Quant available'}de{'Quant verfügbar'}", 710, true, Global.ACKinds.MSMethodPrePost)]
        public void AvailableFacilityCharge()
        {
            if (!PreExecute("AvailableFacilityCharge")) return;
            CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.ResetIfNotAvailable);
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
                ClearBookingData();
            PostExecute("AvailableFacilityCharge");
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
        #endregion

        #region Materialneuzuordnung (Reassignment)
        /// <summary>
        /// Facilities the relocation.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Reassign Material'}de{'Material neu zuordnen'}", 711, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void FacilityReassign()
        {
            if (!PreExecute("FacilityReassign")) return;
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
            PostExecute("FacilityReassign");
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

        #region Materialneuzuordnung (Reassignment)
        /// <summary>
        /// Facilities the relocation.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Split Quant'}de{'Quant Splitten'}", 712, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void SplitQuant()
        {
            if (!PreExecute("SplitQuant")) return;
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
            PostExecute("SplitQuant");
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
        [ACMethodCommand(FacilityCharge.ClassName, "en{'New Split No.'}de{'Neue Splitnr.'}", 713, true)]
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
                CurrentFacilityCharge.Material.FacilityLot_Material.Add(lot);
                //OnPropertyChanged("CurrentFacilityCharge");
                //RefreshFilterFacilityLotAccess();
                _AccessFacilityLot.NavList.Add(lot);
                CurrentFacilityCharge.FacilityLot = lot;
                OnPropertyChanged("FacilityChargeLotList");
                OnPropertyChanged("CurrentFacilityCharge");
            }
            if (childBSO != null)
                childBSO.Stop();
        }

        public bool IsEnabledFacilityChargeLotGenerateDlg()
        {
            return CurrentFacilityCharge != null && CurrentFacilityCharge.Material != null && !IsFacilityChargeWithFacilityBooking;
        }


        public bool IsFacilityChargeWithFacilityBooking
        {
            get
            {
                if (CurrentFacilityCharge == null) return false;
                return
                    CurrentFacilityCharge.FacilityBooking_InwardFacilityCharge.Any() ||
                    CurrentFacilityCharge.FacilityBooking_OutwardFacilityCharge.Any() ||
                    CurrentFacilityCharge.FacilityBookingCharge_InwardFacilityCharge.Any() ||
                    CurrentFacilityCharge.FacilityBookingCharge_OutwardFacilityCharge.Any();
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
                if (CurrentBookParamRelocation.OutwardFacilityCharge != null
                    && CurrentBookParamRelocation.OutwardFacilityCharge.Facility != null
                    && CurrentBookParamRelocation.InwardFacility != null
                    && CurrentBookParamRelocation.InwardFacility.VBiFacilityACClassID.HasValue)
                {
                    return true;
                }
                return false;
            }
        }
        #endregion


        #endregion

        #region Eventhandling
        public override void OnPropertyChanged([CallerMemberName] string name = "")
        {
            base.OnPropertyChanged(name);
            if (name == "ShowNotAvailable"
                || name == "FilterMaterial"
                || name == "FilterFacility"
                || name == "FilterLot")
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
                case "New":
                    New();
                    return true;
                case "IsEnabledNew":
                    result = IsEnabledNew();
                    return true;
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
                case "Delete":
                    Delete();
                    return true;
                case "IsEnabledDelete":
                    result = IsEnabledDelete();
                    return true;
                case "Search":
                    Search();
                    return true;
                case "ClearBookingData":
                    ClearBookingData();
                    return true;
                case "IsEnabledClearBookingData":
                    result = IsEnabledClearBookingData();
                    return true;
                case "InwardFacilityChargeMovement":
                    InwardFacilityChargeMovement();
                    return true;
                case "IsEnabledInwardFacilityChargeMovement":
                    result = IsEnabledInwardFacilityChargeMovement();
                    return true;
                case "OutwardFacilityChargeMovement":
                    OutwardFacilityChargeMovement();
                    return true;
                case "IsEnabledOutwardFacilityChargeMovement":
                    result = IsEnabledOutwardFacilityChargeMovement();
                    return true;
                case "FacilityChargeRelocation":
                    FacilityChargeRelocation();
                    return true;
                case "IsEnabledFacilityChargeRelocation":
                    result = IsEnabledFacilityChargeRelocation();
                    return true;
                case "LockFacilityCharge":
                    LockFacilityCharge();
                    return true;
                case "IsEnabledLockFacilityCharge":
                    result = IsEnabledLockFacilityCharge();
                    return true;
                case "LockFacilityChargeAbsolute":
                    LockFacilityChargeAbsolute();
                    return true;
                case "IsEnabledLockFacilityChargeAbsolute":
                    result = IsEnabledLockFacilityChargeAbsolute();
                    return true;
                case "ReleaseFacilityCharge":
                    ReleaseFacilityCharge();
                    return true;
                case "IsEnabledReleaseFacilityCharge":
                    result = IsEnabledReleaseFacilityCharge();
                    return true;
                case "ReleaseFacilityChargeAbsolute":
                    ReleaseFacilityChargeAbsolute();
                    return true;
                case "IsEnabledReleaseFacilityChargeAbsolute":
                    result = IsEnabledReleaseFacilityChargeAbsolute();
                    return true;
                case "NotAvailableFacilityCharge":
                    NotAvailableFacilityCharge();
                    return true;
                case "IsEnabledNotAvailableFacilityCharge":
                    result = IsEnabledNotAvailableFacilityCharge();
                    return true;
                case "AvailableFacilityCharge":
                    AvailableFacilityCharge();
                    return true;
                case "IsEnabledAvailableFacilityCharge":
                    result = IsEnabledAvailableFacilityCharge();
                    return true;
                case "FacilityReassign":
                    FacilityReassign();
                    return true;
                case "IsEnabledFacilityReassign":
                    result = IsEnabledFacilityReassign();
                    return true;
                case "SplitQuant":
                    SplitQuant();
                    return true;
                case "IsEnabledSplitQuant":
                    result = IsEnabledSplitQuant();
                    return true;
                case "NewChargeNo":
                    NewChargeNo();
                    return true;
                case "IsEnabledNewChargeNo":
                    result = IsEnabledNewChargeNo();
                    return true;
                case "NewSplitChargeNo":
                    NewSplitChargeNo();
                    return true;
                case "IsEnabledNewSplitChargeNo":
                    result = IsEnabledNewSplitChargeNo();
                    return true;
                case "FacilityChargeLotGenerateDlg":
                    FacilityChargeLotGenerateDlg();
                    return true;
                case "IsEnabledFacilityChargeLotGenerateDlg":
                    result = IsEnabledFacilityChargeLotGenerateDlg();
                    return true;
                case "OnActivate":
                    OnActivate((String)acParameter[0]);
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
    }

}
