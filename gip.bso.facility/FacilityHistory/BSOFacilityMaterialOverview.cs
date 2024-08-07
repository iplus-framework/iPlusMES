// ***********************************************************************
// Assembly         : gip.bso.facility
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOFacilityMaterialOverview.cs" company="gip mbh, Oftersheim, Germany">
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
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Material Overview'}de{'Materialübersicht'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MaterialStock.ClassName)]
    public class BSOFacilityMaterialOverview : BSOFacilityOverviewBase
    {
        #region constants
        public const string filter_key_materialgroup_mdkey = "Material\\MDMaterialGroup\\MDKey";
        public const string filter_key_materialwf = "Material\\MaterialWF";
        public const string filter_key_materialno = "Material\\MaterialNo";


        //public virtual Global.LogicalOperators WFOperator 
        //{
        //    get
        //    {
        //        return Global.LogicalOperators.isNull;
        //    }
        //}
        #endregion

        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityMaterialOverview"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityMaterialOverview(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            bool search = Parameters == null;
            if (!search)
            {
                search = !Parameters.Any();
            }
            if (!search)
            {
                ACValue autoFilterParam = Parameters.Where(c => c.ACIdentifier == "AutoFilter").FirstOrDefault();
                if (autoFilterParam != null)
                {
                    search = autoFilterParam.ValueT<bool>();
                }
            }
            if (search)
            {
                Search();
            }
            return true;
        }


        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            this._CurrentFacilityCharge = null;
            this._CurrentFacilityChargeSumFacilityHelper = null;
            this._CurrentFacilityChargeSumLocationHelper = null;
            this._CurrentFacilityChargeSumLotHelper = null;
            this._SelectedFacilityCharge = null;
            this._SelectedFacilityChargeSumFacilityHelper = null;
            this._SelectedFacilityChargeSumLocationHelper = null;
            this._SelectedFacilityChargeSumLotHelper = null;
            this._MaterialGroupFilter = null;

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


        public override object Clone()
        {
            BSOFacilityMaterialOverview clone = base.Clone() as BSOFacilityMaterialOverview;
            clone.FilterFBType = FilterFBType;
            clone.MaterialGroupFilter = MaterialGroupFilter;
            return clone;
        }

        #endregion

        #region ChildBSO

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

        #region BSO->ACPropertyPrimary->MaterialStock
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<MaterialStock> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(890, "MaterialStock")]
        public ACAccessNav<MaterialStock> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(NavigationqueryDefaultFilter);
                    }
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MaterialStock>("MaterialStock", this);
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
                    new ACFilterItem(Global.FilterTypes.filter, Material.ClassName + "\\" + nameof(Material.MaterialNo), Global.LogicalOperators.contains, Global.Operators.or, null, true, true),
                    new ACFilterItem(Global.FilterTypes.filter, Material.ClassName + "\\" + nameof(Material.MaterialName1), Global.LogicalOperators.contains, Global.Operators.or, null, true, true),
                    new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                };
            }
        }

        /// <summary>
        /// Gets or sets the selected material stock.
        /// </summary>
        /// <value>The selected material stock.</value>
        [ACPropertySelected(801, "MaterialStock")]
        public MaterialStock SelectedMaterialStock
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
                OnPropertyChanged(nameof(SelectedMaterialStock));
            }
        }

        /// <summary>
        /// Gets or sets the current material stock.
        /// </summary>
        /// <value>The current material stock.</value>
        [ACPropertyCurrent(802, "MaterialStock")]
        public MaterialStock CurrentMaterialStock
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
                CleanMovements();
                OnPropertyChanged(nameof(CurrentMaterialStock));
                RefreshRelatedData();
                ClearReservation();
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

            RefreshFacilityChargeSumLotHelperList();
            RefreshFacilityChargeSumFacilityHelperList();
            RefreshFacilityChargeSumLocationHelperList();
        }

        /// <summary>
        /// Gets the material stock list.
        /// </summary>
        /// <value>The material stock list.</value>
        [ACPropertyList(803, "MaterialStock")]
        public IEnumerable<MaterialStock> MaterialStockList
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.NavList;
            }
        }

        private MDMaterialGroup _MaterialGroupFilter;
        [ACPropertySelected(804, "MaterialGroup", "en{'Material Group Filter'}de{'Materialgruppenfilter'}")]
        public MDMaterialGroup MaterialGroupFilter
        {
            get
            {
                return _MaterialGroupFilter;
            }
            set
            {
                _MaterialGroupFilter = value;
                OnPropertyChanged(nameof(MaterialGroupFilter));
            }
        }

        /// <summary>
        /// Gets the material group list.
        /// </summary>
        /// <value>The material group list</value>
        [ACPropertyList(805, "MaterialGroup", "en{'Material Group Filter'}de{'Materialgruppenfilter'}")]
        public IEnumerable<MDMaterialGroup> MaterialGroupFilterList
        {
            get
            {
                return DatabaseApp.MDMaterialGroup;
            }
        }

        #endregion

        #region BSO->ACProperty->FacilityLot

        /// <summary>
        /// The _ current facility charge sum lot helper
        /// </summary>
        FacilityChargeSumLotHelper _CurrentFacilityChargeSumLotHelper;
        /// <summary>
        /// Gets or sets the current facility charge sum lot helper.
        /// </summary>
        /// <value>The current facility charge sum lot helper.</value>
        [ACPropertyCurrent(806, "FacilityChargeSumLotHelper")]
        public FacilityChargeSumLotHelper CurrentFacilityChargeSumLotHelper
        {
            get
            {
                return _CurrentFacilityChargeSumLotHelper;
            }
            set
            {
                _CurrentFacilityChargeSumLotHelper = value;
                OnPropertyChanged(nameof(CurrentFacilityChargeSumLotHelper));
            }
        }

        /// <summary>
        /// Gets the facility charge sum lot helper list.
        /// </summary>
        /// <value>The facility charge sum lot helper list.</value>
        [ACPropertyList(807, "FacilityChargeSumLotHelper")]
        public IEnumerable<FacilityChargeSumLotHelper> FacilityChargeSumLotHelperList
        {
            get
            {
                if (CurrentMaterialStock == null)
                    return null;
                if (CurrentMaterialStock.Material == null)
                    return null;
                List<FacilityChargeSumLotHelper> items = ACFacilityManager.GetFacilityChargeSumLotHelperList(FacilityChargeList, new FacilityQueryFilter() { MaterialID = CurrentMaterialStock.MaterialID }).ToList();
                // when FilterLotNos exist - reduce already used FacilityLots
                if (FilterLotNos != null && FilterLotNos.Any())
                {
                    items = items.Where(c => !FilterLotNos.Contains(c.FacilityLot.LotNo)).ToList();
                }
                return items;
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
        [ACPropertySelected(808, "FacilityChargeSumLotHelper")]
        public FacilityChargeSumLotHelper SelectedFacilityChargeSumLotHelper
        {
            get
            {
                return _SelectedFacilityChargeSumLotHelper;
            }
            set
            {
                _SelectedFacilityChargeSumLotHelper = value;
                OnPropertyChanged(nameof(SelectedFacilityChargeSumLotHelper));
            }
        }


        private void RefreshFacilityChargeSumLotHelperList()
        {
            CurrentFacilityChargeSumLotHelper = null;
            SelectedFacilityChargeSumLotHelper = null;
            OnPropertyChanged(nameof(FacilityChargeSumLotHelperList));
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
        [ACPropertyCurrent(809, FacilityCharge.ClassName)]
        public FacilityCharge CurrentFacilityCharge
        {
            get
            {
                return _CurrentFacilityCharge;
            }
            set
            {
                _CurrentFacilityCharge = value;
                OnPropertyChanged(nameof(CurrentFacilityCharge));
            }
        }

        IEnumerable<FacilityCharge> _FacilityChargeList;
        /// <summary>
        /// Gets the facility charge list.
        /// </summary>
        /// <value>The facility charge list.</value>
        [ACPropertyList(810, FacilityCharge.ClassName)]
        public IEnumerable<FacilityCharge> FacilityChargeList
        {
            get
            {
                if (_FacilityChargeList != null)
                    return _FacilityChargeList;
                if (CurrentMaterialStock == null)
                    return null;
                if (CurrentMaterialStock.Material == null)
                    return null;
                _FacilityChargeList = FacilityManager.s_cQry_MatOverviewFacilityCharge(this.DatabaseApp, CurrentMaterialStock.MaterialID, ShowNotAvailable).ToArray();
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
        [ACPropertySelected(811, FacilityCharge.ClassName)]
        public FacilityCharge SelectedFacilityCharge
        {
            get
            {
                return _SelectedFacilityCharge;
            }
            set
            {
                _SelectedFacilityCharge = value;
                OnPropertyChanged(nameof(SelectedFacilityCharge));
            }
        }

        private void RefreshFacilityChargeList()
        {
            CurrentFacilityCharge = null;
            SelectedFacilityCharge = null;
            _FacilityChargeList = null;
            OnPropertyChanged(nameof(FacilityChargeList));
        }
        #endregion

        #region BSO->ACProperty->SumLocation
        /// <summary>
        /// SumLocation: A sum over all Charges/Batches of the actual Material grouped by Storage Location
        /// </summary>
        FacilityChargeSumLocationHelper _CurrentFacilityChargeSumLocationHelper;
        /// <summary>
        /// Gets or sets the current facility charge sum location helper.
        /// </summary>
        /// <value>The current facility charge sum location helper.</value>
        [ACPropertyCurrent(812, "FacilityChargeSumLocationHelper")]
        public FacilityChargeSumLocationHelper CurrentFacilityChargeSumLocationHelper
        {
            get
            {
                return _CurrentFacilityChargeSumLocationHelper;
            }
            set
            {
                _CurrentFacilityChargeSumLocationHelper = value;
                OnPropertyChanged(nameof(CurrentFacilityChargeSumLocationHelper));
            }
        }

        /// <summary>
        /// Gets the facility charge sum location helper list.
        /// </summary>
        /// <value>The facility charge sum location helper list.</value>
        [ACPropertyList(813, "FacilityChargeSumLocationHelper")]
        public IEnumerable<FacilityChargeSumLocationHelper> FacilityChargeSumLocationHelperList
        {
            get
            {
                if (CurrentMaterialStock == null)
                    return null;
                if (CurrentMaterialStock.Material == null)
                    return null;
                return ACFacilityManager.GetFacilityChargeSumLocationHelperList(FacilityChargeList, new FacilityQueryFilter());
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
        [ACPropertySelected(814, "FacilityChargeSumLocationHelper")]
        public FacilityChargeSumLocationHelper SelectedFacilityChargeSumLocationHelper
        {
            get
            {
                return _SelectedFacilityChargeSumLocationHelper;
            }
            set
            {
                _SelectedFacilityChargeSumLocationHelper = value;
                OnPropertyChanged(nameof(SelectedFacilityChargeSumLocationHelper));
            }
        }

        private void RefreshFacilityChargeSumLocationHelperList()
        {
            CurrentFacilityChargeSumLocationHelper = null;
            SelectedFacilityChargeSumLocationHelper = null;
            OnPropertyChanged(nameof(FacilityChargeSumLocationHelperList));
        }

        #endregion

        #region BSO->ACProperty->SumFacility
        /// <summary>
        /// SumLocation: A sum over all Charges/Batches of the actual Material grouped by Storage Area
        /// </summary>
        FacilityChargeSumFacilityHelper _CurrentFacilityChargeSumFacilityHelper;
        /// <summary>
        /// Gets or sets the current facility charge sum facility helper.
        /// </summary>
        /// <value>The current facility charge sum facility helper.</value>
        [ACPropertyCurrent(815, "FacilityChargeSumFacilityHelper")]
        public FacilityChargeSumFacilityHelper CurrentFacilityChargeSumFacilityHelper
        {
            get
            {
                return _CurrentFacilityChargeSumFacilityHelper;
            }
            set
            {
                _CurrentFacilityChargeSumFacilityHelper = value;
                OnPropertyChanged(nameof(CurrentFacilityChargeSumFacilityHelper));
            }
        }

        /// <summary>
        /// Gets the facility charge sum facility helper list.
        /// </summary>
        /// <value>The facility charge sum facility helper list.</value>
        [ACPropertyList(816, "FacilityChargeSumFacilityHelper")]
        public IEnumerable<FacilityChargeSumFacilityHelper> FacilityChargeSumFacilityHelperList
        {
            get
            {
                if (CurrentMaterialStock == null)
                    return null;
                if (CurrentMaterialStock.Material == null)
                    return null;
                return ACFacilityManager.GetFacilityChargeSumFacilityHelperList(FacilityChargeList, new FacilityQueryFilter());
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
        [ACPropertySelected(817, "FacilityChargeSumFacilityHelper")]
        public FacilityChargeSumFacilityHelper SelectedFacilityChargeSumFacilityHelper
        {
            get
            {
                return _SelectedFacilityChargeSumFacilityHelper;
            }
            set
            {
                _SelectedFacilityChargeSumFacilityHelper = value;
                OnPropertyChanged(nameof(SelectedFacilityChargeSumFacilityHelper));
            }
        }

        private void RefreshFacilityChargeSumFacilityHelperList()
        {
            CurrentFacilityChargeSumFacilityHelper = null;
            SelectedFacilityChargeSumFacilityHelper = null;
            OnPropertyChanged(nameof(FacilityChargeSumFacilityHelperList));
        }

        #endregion

        #endregion

        #region BSO->ACMethod

        #region BSO->ACMethod->Save&Search

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand("MaterialStock", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand("MaterialStock", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction("MaterialStock", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedMaterialStock")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MaterialStock>(requery, () => SelectedMaterialStock, () => CurrentMaterialStock, c => CurrentMaterialStock = c,
                        DatabaseApp.MaterialStock
                        .Where(c => c.Material.MaterialID == SelectedMaterialStock.Material.MaterialID));
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
        [ACMethodCommand("MaterialStock", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public virtual void Search()
        {
            if (AccessPrimary == null)
                return;
            RefreshFacilityMaterialAccess();
            AccessPrimary.NavSearch(DatabaseApp, MergeOption.OverwriteChanges);
            OnPropertyChanged(nameof(MaterialStockList));
        }

        IQueryable<MaterialStock> _AccessPrimary_NavSearchExecuting(IQueryable<MaterialStock> result)
        {
            ObjectQuery<MaterialStock> query = result as ObjectQuery<MaterialStock>;
            if (query != null)
            {
                query.Include(c => c.Material);
            }
            return result;
        }

        [ACMethodInteraction("Filter", "en{'Filter'}de{'Filtern'}", (short)MISort.Search, false, "Filter")]
        public void Filter()
        {
            if (AccessPrimary == null)
                return;
            if (MaterialGroupFilter != null)
            {
                ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.FirstOrDefault(c => c.PropertyName == filter_key_materialgroup_mdkey);
                if (filterItem == null)
                {
                    filterItem = new ACFilterItem(Global.FilterTypes.filter, filter_key_materialgroup_mdkey, Global.LogicalOperators.equal, Global.Operators.and, MaterialGroupFilter.MDKey.ToString(), false);
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Add(filterItem);
                }
                else
                    filterItem.SearchWord = MaterialGroupFilter.MDKey.ToString();
            }
            else
            {
                ACFilterItem filterItemExist = AccessPrimary.NavACQueryDefinition.ACFilterColumns.FirstOrDefault(c => c.PropertyName == filter_key_materialgroup_mdkey);
                if (filterItemExist != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExist);
            }
            Search();
        }

        public VBDialogResult DialogResult { get; set; }
        public string[] FilterLotNos { get; set; }

        [ACMethodInfo(nameof(ShowLotDlg), "en{'Lot'}de{'Los'}", (short)MISort.QueryPrintDlg)]
        public VBDialogResult ShowLotDlg(string materialNo, string[] lotNos)
        {
            if (DialogResult == null)
                DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.Cancel;
            FilterLotNos = lotNos;

            ACFilterItem materialNoFilter = AccessPrimary.NavACQueryDefinition.ACFilterColumns.FirstOrDefault(c => c.PropertyName == filter_key_materialno);
            if (materialNoFilter == null)
            {
                materialNoFilter = new ACFilterItem(Global.FilterTypes.filter, filter_key_materialno, Global.LogicalOperators.equal, Global.Operators.and, MaterialGroupFilter.MDKey.ToString(), false);
                AccessPrimary.NavACQueryDefinition.ACFilterColumns.Add(materialNoFilter);
            }
            materialNoFilter.SearchWord = materialNo;

            Search();

            ShowDialog(this, "LotDlg");
            this.ParentACComponent.StopComponent(this);
            return DialogResult;
        }

        [ACMethodCommand(nameof(ShowLotDlg), Const.Ok, (short)MISort.Okay)]
        public void ShowLotDlgOk()
        {
            if (DialogResult != null)
            {
                DialogResult.SelectedCommand = eMsgButton.OK;
                DialogResult.ReturnValue = SelectedFacilityChargeSumLotHelper.FacilityLot;
            }
            FilterLotNos = null;
            CloseTopDialog();
        }

        public bool IsEnabledShowLotDlgOk()
        {
            return SelectedFacilityChargeSumLotHelper != null;
        }

        [ACMethodCommand(nameof(ShowLotDlg), Const.Cancel, (short)MISort.Cancel)]
        public void ShowLotDlgCancel()
        {
            if (DialogResult != null)
            {
                DialogResult.SelectedCommand = eMsgButton.Cancel;
                DialogResult.ReturnValue = null;
            }
            FilterLotNos = null;
            CloseTopDialog();
        }

        public bool IsEnalbedShowLotDlgCancel()
        {
            return true;
        }


        public virtual void RefreshFacilityMaterialAccess()
        {
            if (AccessPrimary == null)
                return;
            ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.FirstOrDefault(c => c.PropertyName == filter_key_materialwf);
            if (filterItem != null)
            {
                AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItem);
            }
            //filterItem = new ACFilterItem(Global.FilterTypes.filter, filter_key_materialwf, WFOperator, Global.Operators.and, "", true);
            //AccessPrimary.NavACQueryDefinition.ACFilterColumns.Add(filterItem);
        }

        #endregion

        #region BSO->ACMethod->Reservation

        /// <summary>
        /// Source Property: LoadFacilityReservation
        /// </summary>
        [ACMethodInfo(nameof(LoadFacilityReservation), "en{'Load Facility Reservation'}de{'Reservierung einladen'}", 999)]
        public void LoadFacilityReservation()
        {
            if (!IsEnabledMethodName())
                return;
            if (BSOFacilityReservationOverview_Child != null && BSOFacilityReservationOverview_Child.Value != null)
            {
                BSOFacilityReservationOverview_Child.Value.LoadReservation(CurrentMaterialStock.Material, SearchFrom, SearchTo);
            }
        }

        public bool IsEnabledMethodName()
        {
            return CurrentMaterialStock != null;
        }



        private void ClearReservation()
        {
            if (BSOFacilityReservationOverview_Child != null && BSOFacilityReservationOverview_Child.Value != null)
            {
                BSOFacilityReservationOverview_Child.Value.ClearReservation();
            }
        }

        #endregion

        #region BSO->ACMethod->Navigation
        [ACMethodInteraction("", "en{'Show Order'}de{'Auftrag anzeigen'}", 780, true, nameof(SelectedFacilityCharge))]
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

        [ACMethodInteraction(nameof(NavigateToFacilityCharge), "en{'Manage Quant'}de{'Quant verwalten'}", 781, true, nameof(SelectedFacilityCharge))]
        public void NavigateToFacilityCharge()
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

                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacilityCharge()
        {
            return SelectedFacilityCharge != null;
        }

        [ACMethodInteraction("", "en{'Manage Lot/Batch'}de{'Verwalte Los/Charge'}", 781, true, nameof(SelectedFacilityCharge))]
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

        [ACMethodInteraction("", "en{'Show Lot Stock and History'}de{'Zeige Losbestand und Historie'}", 782, true, nameof(SelectedFacilityCharge))]
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

        [ACMethodInteraction("", "en{'Show Bin Stock and History'}de{'Zeige Behälterbestand und Historie'}", 783, true, nameof(SelectedFacilityCharge))]
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

        [ACMethodInteraction("", "en{'Manage Stock of Bin'}de{'Verwalte Behälterbestand'}", 784, true, nameof(SelectedFacilityCharge))]
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

        [ACMethodInteraction("", "en{'Manage Material'}de{'Verwalte Material'}", 785, true, nameof(SelectedMaterialStock))]
        public void NavigateToMaterial()
        {
            if (!IsEnabledNavigateToMaterial())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(Material), SelectedMaterialStock.MaterialID));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToMaterial()
        {
            if (SelectedMaterialStock != null && SelectedMaterialStock.Material != null)
                return true;
            return false;
        }
        #endregion

        #region BSO->ACMethod->DialogOrderInfo

        [ACMethodInfo("Dialog", "en{'Dialog'}de{'Dialog'}", (short)MISort.QueryPrintDlg + 1)]
        public void ShowDialogOrderInfo(PAOrderInfo paOrderInfo)
        {
            if (AccessPrimary == null || paOrderInfo == null)
                return;

            PAOrderInfoEntry entityInfo = paOrderInfo.Entities.Where(c => c.EntityName == nameof(Material)).FirstOrDefault();
            if (entityInfo == null)
                return;

            Material material = this.DatabaseApp.Material.Where(c => c.MaterialID == entityInfo.EntityID).FirstOrDefault();
            if (material == null)
                return;

            ShowDialogOrder(material.MaterialNo);
        }

        [ACMethodInfo("Dialog", "en{'Dialog'}de{'Dialog'}", (short)MISort.QueryPrintDlg)]
        public void ShowDialogOrder(string no)
        {
            if (AccessPrimary == null)
                return;

            ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == Material.ClassName + "\\" + nameof(Material.MaterialNo)).FirstOrDefault();
            if (filterItem == null)
            {
                filterItem = new ACFilterItem(Global.FilterTypes.filter, Material.ClassName + "\\" + nameof(Material.MaterialNo), Global.LogicalOperators.contains, Global.Operators.and, no, false);
                AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);
            }
            filterItem.SearchWord = no;

            this.Search();
            ShowDialog(this, "OrderInfoDialog");
            this.ParentACComponent.StopComponent(this);
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
                case nameof(Filter):
                    Filter();
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
                case nameof(NavigateToFacilityCharge):
                    NavigateToFacilityCharge();
                    return true;
                case nameof(IsEnabledNavigateToFacilityCharge):
                    result = IsEnabledNavigateToFacilityCharge();
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
                case nameof(ShowDialogOrderInfo):
                    ShowDialogOrderInfo((PAOrderInfo)acParameter[0]);
                    return true;
                case nameof(ShowDialogOrder):
                    ShowDialogOrder((String)acParameter[0]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

        #region FacilityBooking(Charge)Overview methods -> Executive methods overrides

        public override bool IsEnabledRefreshMovements()
        {
            return base.IsEnabledRefreshMovements() && CurrentMaterialStock != null && CurrentMaterialStock.MaterialID != null;
        }

        public override FacilityQueryFilter GetFacilityBookingFilter()
        {
            FacilityQueryFilter filter = base.GetFacilityBookingFilter();
            if (CurrentMaterialStock != null && CurrentMaterialStock.MaterialID != null)
                filter.MaterialID = CurrentMaterialStock.MaterialID;
            return filter;
        }

        public override void OnFacilityBookingSearchSum()
        {
            if (FacilityBookingOverviewList != null && CurrentMaterialStock != null)
            {
                double sum = 0.0;
                foreach (var fb in FacilityBookingOverviewList)
                {
                    if (String.IsNullOrEmpty(fb.InwardMaterialNo) || fb.InwardMaterialNo == CurrentMaterialStock.Material.MaterialNo)
                        sum += fb.InwardQuantityUOM;
                    if (String.IsNullOrEmpty(fb.OutwardMaterialNo) || fb.OutwardMaterialNo == CurrentMaterialStock.Material.MaterialNo)
                        sum -= fb.OutwardQuantityUOM;
                    fb.InOutSumUOM = sum;
                }
            }

            if (FacilityBookingChargeOverviewList != null && CurrentMaterialStock != null)
            {
                double sum = 0.0;
                foreach (var fb in FacilityBookingChargeOverviewList)
                {
                    if (fb.InwardMaterialNo == CurrentMaterialStock.Material.MaterialNo)
                        sum += fb.InwardQuantityUOM;
                    if (fb.OutwardMaterialNo == CurrentMaterialStock.Material.MaterialNo)
                        sum -= fb.OutwardQuantityUOM;
                    fb.InOutSumUOM = sum;
                }
            }
        }

        #endregion
    }
}
