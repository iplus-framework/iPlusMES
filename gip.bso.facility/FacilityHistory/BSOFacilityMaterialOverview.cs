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
        public const string filter_key_materialwf = "Material\\MaterialWF";


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
            Search();
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
                        if (navACQueryDefinition.TakeCount == 0)
                            navACQueryDefinition.TakeCount = ACQueryDefinition.C_DefaultTakeCount;
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
                    new ACFilterItem(Global.FilterTypes.filter, Material.ClassName + "\\MaterialNo", Global.LogicalOperators.contains, Global.Operators.or, null, true, true),
                    new ACFilterItem(Global.FilterTypes.filter, Material.ClassName + "\\MaterialName1", Global.LogicalOperators.contains, Global.Operators.or, null, true, true),
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
                OnPropertyChanged("SelectedMaterialStock");
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
                OnPropertyChanged("CurrentMaterialStock");
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
                OnPropertyChanged("MaterialGroupFilter");
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
                OnPropertyChanged("CurrentFacilityChargeSumLotHelper");
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
                return ACFacilityManager.GetFacilityChargeSumLotHelperList(FacilityChargeList, new FacilityQueryFilter() { MaterialID = CurrentMaterialStock.MaterialID });
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
                OnPropertyChanged("CurrentFacilityCharge");
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
                OnPropertyChanged("CurrentFacilityChargeSumLocationHelper");
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
                OnPropertyChanged("CurrentFacilityChargeSumFacilityHelper");
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

        #endregion

        #region BSO->ACMethod
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
            OnPropertyChanged("MaterialStockList");
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
                ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.FirstOrDefault(c => c.PropertyName == "Material\\MDMaterialGroup\\MDKey");
                if (filterItem == null)
                {
                    filterItem = new ACFilterItem(Global.FilterTypes.filter, "Material\\MDMaterialGroup\\MDKey", Global.LogicalOperators.equal, Global.Operators.and, MaterialGroupFilter.MDKey.ToString(), false);
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Add(filterItem);
                }
                else
                    filterItem.SearchWord = MaterialGroupFilter.MDKey.ToString();
            }
            else
            {
                ACFilterItem filterItemExist = AccessPrimary.NavACQueryDefinition.ACFilterColumns.FirstOrDefault(c => c.PropertyName == "Material\\MDMaterialGroup\\MDKey");
                if (filterItemExist != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExist);
            }
            Search();
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
                case "Filter":
                    Filter();
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