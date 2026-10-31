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
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.facility
{
    /// <summary>
    /// Businessobject or App for the stock overview of materials (MaterialStock).
    /// This app replaces the legacy stock overview masks (total material stocks of articles, charges and material charges).
    /// ALL stock bookings are always executed only via the FacilityBookingManager. This BSO is a read-only overview; it does not create, change or delete stock.
    /// To search for records, enter the search string in the SearchWord property. The search filters on the material number and the material name.
    /// The database result is copied to the MaterialStockList property.
    /// Then call the NavigateFirst() method to set CurrentMaterialStock with the first record in the list.
    /// CurrentMaterialStock is used to display the currently selected material with its stock.
    /// The MaterialGroupFilter property can be assigned a material group; call the Filter() method afterwards to restrict the list to this material group.
    /// The current material is passed to the embedded child BSO BSOMaterialDetails_Child (properties CurrentMaterialID and CurrentMaterialStock), which shows the details of the material.
    /// The FacilityBookingOverviewList and FacilityBookingChargeOverviewList properties (base class BSOFacilityOverviewBase) contain the inward/outward movements of the material in the period SearchFrom to SearchTo; call RefreshMovements() to reload them.
    /// The ShowLotDlg() method opens the lot selection dialog for a material number and returns the selected lot in the DialogResult property; ShowLotDlgOk() and ShowLotDlgCancel() close the dialog.
    /// The Save(), UndoSave() and Load() methods follow the common BSO pattern; the overview itself provides no New() or Delete() method.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityMaterialOverview&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Material Overview'}de{'Materialübersicht'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MaterialStock.ClassName,
        Description = @"Businessobject or App for the stock overview of materials (MaterialStock).
        This app replaces the legacy stock overview masks (total material stocks of articles, charges and material charges).
        ALL stock bookings are always executed only via the FacilityBookingManager. This BSO is a read-only overview; it does not create, change or delete stock.
        To search for records, enter the search string in the SearchWord property. The search filters on the material number and the material name.
        The database result is copied to the MaterialStockList property.
        Then call the NavigateFirst() method to set CurrentMaterialStock with the first record in the list.
        CurrentMaterialStock is used to display the currently selected material with its stock.
        The MaterialGroupFilter property can be assigned a material group; call the Filter() method afterwards to restrict the list to this material group.
        The current material is passed to the embedded child BSO BSOMaterialDetails_Child (properties CurrentMaterialID and CurrentMaterialStock), which shows the details of the material.
        The FacilityBookingOverviewList and FacilityBookingChargeOverviewList properties (base class BSOFacilityOverviewBase) contain the inward/outward movements of the material in the period SearchFrom to SearchTo; call RefreshMovements() to reload them.
        The ShowLotDlg() method opens the lot selection dialog for a material number and returns the selected lot in the DialogResult property; ShowLotDlgOk() and ShowLotDlgCancel() close the dialog.
        The Save(), UndoSave() and Load() methods follow the common BSO pattern; the overview itself provides no New() or Delete() method.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityMaterialOverview&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
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


        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            this._MaterialGroupFilter = null;

            if (_AccessPrimary != null)
                _AccessPrimary.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;

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
            BSOFacilityMaterialOverview clone = base.Clone() as BSOFacilityMaterialOverview;
            clone.FilterFBType = FilterFBType;
            clone.MaterialGroupFilter = MaterialGroupFilter;
            return clone;
        }

        #endregion

        #region ChildBSO

        ACChildItem<BSOMaterialDetails> _BSOMaterialDetails_Child_Child;
        [ACPropertyInfo(600, "", "en{'Material details'}de{'Materialdetails'}",
            Description = "Embedded child BSO (Material Details) that shows the details of the selected material.")]
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
        [ACPropertyAccessPrimary(890, "MaterialStock", "en{'Material stock'}de{'Materialbestand'}",
            Description = "Navigation access to the material stocks; source of MaterialStockList, SelectedMaterialStock and CurrentMaterialStock.")]
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
        [ACPropertySelected(801, "MaterialStock", "en{'Material stock'}de{'Materialbestand'}",
            Description = "Material stock selected in the list.")]
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
        [ACPropertyCurrent(802, "MaterialStock", "en{'Material stock'}de{'Materialbestand'}",
            Description = "Currently displayed material stock; it is passed to the child BSO BSOMaterialDetails_Child.")]
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
        [ACPropertyList(803, "MaterialStock", "en{'Material stock'}de{'Materialbestand'}",
            Description = "Result list of the search.")]
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
        [ACPropertySelected(804, "MaterialGroup", "en{'Material Group Filter'}de{'Materialgruppenfilter'}",
            Description = "Material group the list is filtered by (applied with the Filter method).")]
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
        [ACPropertyList(805, "MaterialGroup", "en{'Material Group Filter'}de{'Materialgruppenfilter'}",
            Description = "All material groups offered for filtering.")]
        public IEnumerable<MDMaterialGroup> MaterialGroupFilterList
        {
            get
            {
                return DatabaseApp.MDMaterialGroup.ToList();
            }
        }

        #endregion

        #endregion

        #region BSO->ACMethod

        #region BSO->ACMethod->Save&Search

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand("MaterialStock", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost,
            Description = "Saves the changes of the current material stock to the database.")]
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
        [ACMethodCommand("MaterialStock", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost,
            Description = "Discards the unsaved changes of the current material stock.")]
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
        [ACMethodInteraction("MaterialStock", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedMaterialStock",
            Description = "Reloads the selected material stock from the database.")]
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
        [ACMethodCommand("MaterialStock", "en{'Search'}de{'Suchen'}", (short)MISort.Search,
            Description = "Re-executes the search and rebuilds the MaterialStockList.")]
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
            IQueryable<MaterialStock> query = result as IQueryable<MaterialStock>;
            if (query != null)
            {
                query.Include(c => c.Material)
                     .Include(c => c.Material.BaseMDUnit)
                     .Include("Material.MaterialUnit_Material.ToMDUnit");
            }
            return result;
        }

        [ACMethodInteraction("Filter", "en{'Filter'}de{'Filtern'}", (short)MISort.Search, false, "Filter",
            Description = "Restricts the list to the selected material group (MaterialGroupFilter); removes the restriction if no group is selected.")]
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

        [ACMethodInfo(nameof(ShowLotDlg), "en{'Lot'}de{'Los'}", (short)MISort.QueryPrintDlg,
            Description = "Opens the lot selection dialog for the given material number and returns the selected lot in DialogResult.")]
        public async Task<VBDialogResult> ShowLotDlg(string materialNo, string[] lotNos)
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

            await ShowDialogAsync(this, "LotDlg");
            await this.ParentACComponent.StopComponent(this);
            return DialogResult;
        }

        [ACMethodCommand(nameof(ShowLotDlg), Const.Ok, (short)MISort.Okay,
            Description = "Confirms the lot dialog and returns the selected lot.")]
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

        [ACMethodCommand(nameof(ShowLotDlg), Const.Cancel, (short)MISort.Cancel,
            Description = "Cancels the lot dialog without a selection.")]
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

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(Load):
                case nameof(Search):
                case nameof(Filter):
                    return new[] { "CurrentFacilityCharge", "SelectedFacilityCharge" };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion

    }
}
