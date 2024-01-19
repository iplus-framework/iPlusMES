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
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.Data.Objects;
using gip.mes.facility;
using System.Runtime.CompilerServices;

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
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Lot Overview'}de{'Losübersicht'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + FacilityLot.ClassName)]
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

        public override bool ACDeInit(bool deleteACClassTask = false)
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

        #region Properties

        #region Properties -> Filter

        /// <summary>
        /// 
        /// </summary>
        [ACPropertyInfo(999, nameof(FilterLotNo), ConstApp.LotNo)]
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
        [ACPropertyInfo(999, nameof(FilterExternLotNo), ConstApp.ExternLotNo)]
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

        // Material\MaterialNo
        /// <summary>
        /// 
        /// </summary>
        [ACPropertyInfo(999, nameof(FilterMaterialNo), ConstApp.MaterialNo)]
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
        [ACPropertyAccessPrimary(890, FacilityLot.ClassName)]
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
                        if (navACQueryDefinition.TakeCount == 0)
                            navACQueryDefinition.TakeCount = ACQueryDefinition.C_DefaultTakeCount;
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
                    new ACFilterItem(Global.FilterTypes.filter, nameof(FacilityLot.LotNo), Global.LogicalOperators.contains, Global.Operators.and, null, true, true),
                    new ACFilterItem(Global.FilterTypes.filter, nameof(FacilityLot.ExternLotNo), Global.LogicalOperators.contains, Global.Operators.and, null, true, true),
                    new ACFilterItem(Global.FilterTypes.filter, $"{nameof(Material)}\\{nameof(Material.MaterialNo)}", Global.LogicalOperators.contains, Global.Operators.and, null, true, true)
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
        [ACPropertySelected(801, FacilityLot.ClassName)]
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
        [ACPropertyCurrent(802, FacilityLot.ClassName)]
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
            LoadReservation(CurrentFacilityLot);
        }

        private List<FacilityLot> _FacilityLotList;
        /// <summary>
        /// Gets the facility lot list.
        /// </summary>
        /// <value>The facility lot list.</value>
        [ACPropertyList(803, FacilityLot.ClassName)]
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
        [ACPropertyCurrent(804, FacilityCharge.ClassName)]
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
        [ACPropertySelected(805, FacilityCharge.ClassName)]
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
        [ACPropertyList(806, FacilityCharge.ClassName)]
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
        [ACPropertyCurrent(807, "FacilityChargeSumLocationHelper")]
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
        [ACPropertyList(808, "FacilityChargeSumLocationHelper")]
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

        private void RefreshFacilityChargeSumMaterialHelperList()
        {
            CurrentFacilityChargeSumMaterialHelper = null;
            SelectedFacilityChargeSumMaterialHelper = null;
            OnPropertyChanged("FacilityChargeSumMaterialHelperList");
        }
        #endregion

        #region BSO->ACProperty->PickingReservation

        private FacilityReservationModel _SelectedPickingReservation;
        /// <summary>
        /// Selected property for FacilityReservation
        /// </summary>
        /// <value>The selected PickingReservation</value>
        [ACPropertySelected(9999, "PickingReservation", "en{'TODO: PickingReservation'}de{'TODO: PickingReservation'}")]
        public FacilityReservationModel SelectedPickingReservation
        {
            get
            {
                return _SelectedPickingReservation;
            }
            set
            {
                if (_SelectedPickingReservation != value)
                {
                    _SelectedPickingReservation = value;
                    OnPropertyChanged(nameof(SelectedPickingReservation));
                }
            }
        }


        private List<FacilityReservationModel> _PickingReservationList;
        /// <summary>
        /// List property for FacilityReservation
        /// </summary>
        /// <value>The PickingReservation list</value>
        [ACPropertyList(9999, "PickingReservation")]
        public List<FacilityReservationModel> PickingReservationList
        {
            get
            {
                return _PickingReservationList;
            }
        }

        #endregion

        #region BSO->ACProperty->ProdOrderReservation

        private FacilityReservationModel _SelectedProdOrderReservation;
        /// <summary>
        /// Selected property for FacilityReservation
        /// </summary>
        /// <value>The selected ProdOrderReservation</value>
        [ACPropertySelected(9999, "ProdOrderReservation", "en{'TODO: ProdOrderReservation'}de{'TODO: ProdOrderReservation'}")]
        public FacilityReservationModel SelectedProdOrderReservation
        {
            get
            {
                return _SelectedProdOrderReservation;
            }
            set
            {
                if (_SelectedProdOrderReservation != value)
                {
                    _SelectedProdOrderReservation = value;
                    OnPropertyChanged(nameof(SelectedProdOrderReservation));
                }
            }
        }


        private List<FacilityReservationModel> _ProdOrderReservationList;
        /// <summary>
        /// List property for FacilityReservation
        /// </summary>
        /// <value>The ProdOrderReservation list</value>
        [ACPropertyList(9999, "ProdOrderReservation")]
        public List<FacilityReservationModel> ProdOrderReservationList
        {
            get
            {
                return _ProdOrderReservationList;
            }
        }

        #endregion

        #endregion

        #region BSO->ACMethod

        #region BSO->ACMethod->Save&Search

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(FacilityLot.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand(FacilityLot.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction(FacilityLot.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedFacilityLot")]
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
        [ACMethodCommand(FacilityLot.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
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
            ObjectQuery<FacilityLot> query = result as ObjectQuery<FacilityLot>;
            if (query != null)
            {
                query.Include("FacilityLotStock_FacilityLot");
                if (ShowNotAvailable.HasValue)
                {
                    query = query.Where(c => c.FacilityCharge_FacilityLot.Any(x => x.NotAvailable == ShowNotAvailable.Value)) as ObjectQuery<FacilityLot>;
                }
            }
            return query;
        }

        #endregion

        #region BSO->ACMethod->Reservation

        private void LoadReservation(FacilityLot facilityLot)
        {
            _PickingReservationList = null;
            _ProdOrderReservationList = null;

            if (facilityLot != null)
            {
                FacilityReservation[] reservations = facilityLot.FacilityReservation_FacilityLot.ToArray();

                List<FacilityReservation> pickingReservations = reservations.Where(c => c.PickingPos != null).OrderBy(c => c.InsertDate).ToList();
                if (pickingReservations.Any())
                {
                    _PickingReservationList = new List<FacilityReservationModel>();
                    foreach (FacilityReservation reservation in pickingReservations)
                    {
                        FacilityReservationModel facilityReservationModel = ACFacilityManager.GetFacilityReservationModel(reservation);
                        _PickingReservationList.Add(facilityReservationModel);
                    }
                }

                List<FacilityReservation> prodOrderReservations = reservations.Where(c => c.ProdOrderPartslistPos != null).OrderBy(c => c.InsertDate).ToList();
                if (prodOrderReservations.Any())
                {
                    _ProdOrderReservationList = new List<FacilityReservationModel>();
                    foreach (FacilityReservation reservation in prodOrderReservations)
                    {
                        FacilityReservationModel facilityReservationModel = ACFacilityManager.GetFacilityReservationModel(reservation);
                        _ProdOrderReservationList.Add(facilityReservationModel);
                    }
                }
            }

            OnPropertyChanged(nameof(PickingReservationList));
            OnPropertyChanged(nameof(ProdOrderReservationList));
        }

        #endregion

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
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
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

        #endregion

    }
}
