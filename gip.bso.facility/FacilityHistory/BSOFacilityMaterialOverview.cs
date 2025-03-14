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

        ACChildItem<BSOMaterialDetails> _BSOMaterialDetails_Child_Child;
        [ACPropertyInfo(600)]
        [ACChildInfo(nameof(BSOMaterialDetails_Child), typeof(BSOMaterialDetails))]
        public ACChildItem<BSOMaterialDetails> BSOMaterialDetails_Child
        {
            get
            {
                if (_BSOMaterialDetails_Child_Child == null)
                    _BSOMaterialDetails_Child_Child = new ACChildItem<BSOMaterialDetails>(this, nameof(BSOMaterialDetails_Child));
                return _BSOMaterialDetails_Child_Child;
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
                OnPropertyChanged();
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
                OnPropertyChanged();

                if (BSOMaterialDetails_Child != null && BSOMaterialDetails_Child.Value != null)
                {
                    if (value != null)
                    {
                        BSOMaterialDetails_Child.Value.CurrentMaterialID = value.MaterialID;
                        BSOMaterialDetails_Child.Value.CurrentMaterialStock = value;
                    }
                    else
                    {
                        BSOMaterialDetails_Child.Value.CurrentMaterialID = null;
                        BSOMaterialDetails_Child.Value.CurrentMaterialStock = null;
                    }
                }
            }
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
                query.Include(c => c.Material)
                     .Include("Material.MaterialUnit_Material.ToMDUnit");
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
            if (DialogResult != null && IsEnabledShowLotDlgOk())
            {
                DialogResult.SelectedCommand = eMsgButton.OK;
                DialogResult.ReturnValue = BSOMaterialDetails_Child.Value.SelectedFacilityChargeSumLotHelper.FacilityLot;
            }
            FilterLotNos = null;
            CloseTopDialog();
        }

        public bool IsEnabledShowLotDlgOk()
        {
            return BSOMaterialDetails_Child != null && BSOMaterialDetails_Child.Value != null && BSOMaterialDetails_Child.Value.SelectedFacilityChargeSumLotHelper != null;
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
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}
