// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
// ***********************************************************************
// Assembly         : gip.bso.facility
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 11-08-2012
// ***********************************************************************
// <copyright file="BSOFacilityLot.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.mes.facility;
using System.Runtime.InteropServices;
using static gip.core.datamodel.Global;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace gip.bso.facility
{
    /// <summary>
    /// Businessobject or App for managing facility lots (FacilityLot) from the logistical point of view.
    /// A facility lot contains information about suppliers and quality relevant data from production (production date, filling date, expiration date, storage life).
    /// To search for lots enter the lot number in the FilterLotNo property, the external lot number in FilterExternLotNo or FilterExternLotNo2 and/or the material number in FilterMaterialNo.
    /// The search is executed automatically when one of these filter properties changes. The database result is copied to the FacilityLotList property, sorted by lot number descending.
    /// Then call the NavigateFirst() method to set CurrentFacilityLot with the first record in the list. SelectedFacilityLot is bound to the result grid of the explorer.
    /// Property changes should always be made to CurrentFacilityLot and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
    /// The New() method creates a new lot, generates a new lot number and assigns the new entity object to the CurrentFacilityLot property. Fill in the required fields (lot number, material) before saving.
    /// Use the Delete() method to delete the current lot provided there are no foreign key relationships from other tables (e.g. facility charges).
    /// The Load() method updates the CurrentFacilityLot object with fresh database data if another user has made changes in the background; it also refreshes the expiration date helper.
    /// The expiration date logic is handled by the CurrentFacilityLotExpirationDateHelper: entering the storage life (days) recalculates the expiration date from the production date, and changing the production date or expiration date recalculates the dependent values.
    /// The UndoSave() method discards unsaved changes on the CurrentFacilityLot.
    /// With the NavigateToFacilityLotOverview() method the lot stock and history overview can be opened for the SelectedFacilityLot.
    /// The ShowDialogNewLot() method creates a new lot (with optional lot number and material) and opens it in the FacilityLotDialog; the dialog is confirmed with DialogOK() or closed with DialogCancel() (on cancel the new lot is deleted again).
    /// The ShowDialogOrder() method opens the lot dialog filtered to a given lot number; ShowDialogOrderInfo() opens it for a lot referenced in a PAOrderInfo.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityLot&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Lot Management'}de{'Losverwaltung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + FacilityLot.ClassName,
        Description = @"Businessobject or App for managing facility lots (FacilityLot) from the logistical point of view.
        A facility lot contains information about suppliers and quality relevant data from production (production date, filling date, expiration date, storage life).
        To search for lots enter the lot number in the FilterLotNo property, the external lot number in FilterExternLotNo or FilterExternLotNo2 and/or the material number in FilterMaterialNo.
        The search is executed automatically when one of these filter properties changes. The database result is copied to the FacilityLotList property, sorted by lot number descending.
        Then call the NavigateFirst() method to set CurrentFacilityLot with the first record in the list. SelectedFacilityLot is bound to the result grid of the explorer.
        Property changes should always be made to CurrentFacilityLot and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
        The New() method creates a new lot, generates a new lot number and assigns the new entity object to the CurrentFacilityLot property. Fill in the required fields (lot number, material) before saving.
        Use the Delete() method to delete the current lot provided there are no foreign key relationships from other tables (e.g. facility charges).
        The Load() method updates the CurrentFacilityLot object with fresh database data if another user has made changes in the background; it also refreshes the expiration date helper.
        The expiration date logic is handled by the CurrentFacilityLotExpirationDateHelper: entering the storage life (days) recalculates the expiration date from the production date, and changing the production date or expiration date recalculates the dependent values.
        The UndoSave() method discards unsaved changes on the CurrentFacilityLot.
        With the NavigateToFacilityLotOverview() method the lot stock and history overview can be opened for the SelectedFacilityLot.
        The ShowDialogNewLot() method creates a new lot (with optional lot number and material) and opens it in the FacilityLotDialog; the dialog is confirmed with DialogOK() or closed with DialogCancel() (on cancel the new lot is deleted again).
        The ShowDialogOrder() method opens the lot dialog filtered to a given lot number; ShowDialogOrderInfo() opens it for a lot referenced in a PAOrderInfo.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityLot&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOFacilityLot : ACBSOvbNav
    {
        /// <summary>
        /// The _ exp date helper
        /// </summary>
        private FacilityLotExpirationDateHelper _ExpDateHelper;

        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityLot"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityLot(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            _ExpDateHelper = new FacilityLotExpirationDateHelper(false);

            Search();
            return true;
        }


        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            this._ExpDateHelper = null;
            var b = await base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                await _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }

        /// <summary>When the database context has changed, a dialog is opened that asks the user whether they want to save the changes. If yes then the OnSave()-Method will be invoked to inform all BSO's which uses the same database-context. If not then ACUndoChanges() will be invoked. If cancelled then nothing will happen.</summary>
        /// <returns>Fals, if user has cancelled saving or undoing.</returns>
        public override async Task<bool> ACSaveOrUndoChanges()
        {
            if (DialogResult != null
                && ACIdentifier.StartsWith(ConstApp.BSOFacilityLot_ChildName))
                return true;
            return await base.ACSaveOrUndoChanges();
        }

        #endregion

        #region Properties

        #region Properties -> Filter

        /// <summary>
        /// 
        /// </summary>
        [ACPropertyInfo(999, nameof(FilterLotNo), ConstApp.LotNo, Description = "Filter for the internal lot number (contains). Changing this filter automatically executes the search.")]
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
        [ACPropertyInfo(999, nameof(FilterExternLotNo), ConstApp.ExternLotNo, Description = "Filter for the first external lot number (contains). Changing this filter automatically executes the search.")]
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
        [ACPropertyInfo(999, nameof(FilterMaterialNo), ConstApp.MaterialNo, Description = "Filter for the material number of the lot (contains). Changing this filter automatically executes the search.")]
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

        [ACPropertyInfo(999, nameof(FilterExternLotNo2), ConstApp.ExternLotNo2, Description = "Filter for the second external lot number (contains). Changing this filter automatically executes the search.")]
        public string FilterExternLotNo2
        {
            get
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(nameof(FacilityLot.ExternLotNo2));
                return string.IsNullOrEmpty(tmp) ? null : tmp;
            }
            set
            {
                AccessPrimary.NavACQueryDefinition.SetSearchValue(nameof(FacilityLot.ExternLotNo2), Global.LogicalOperators.contains, value ?? "");
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region BSO->ACProperty
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<FacilityLot> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(690, FacilityLot.ClassName, Description = "Primary navigation access to the FacilityLot records; source of the search list and of SelectedFacilityLot/CurrentFacilityLot.")]
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
                    }

                    if (navACQueryDefinition != null)
                    {
                        _AccessPrimary = navACQueryDefinition.NewAccessNav<FacilityLot>(FacilityLot.ClassName, this);
                    }
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
                    new ACFilterItem(Global.FilterTypes.filter, nameof(FacilityLot.ExternLotNo), Global.LogicalOperators.contains, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, nameof(FacilityLot.ExternLotNo2), Global.LogicalOperators.contains, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, $"{nameof(Material)}\\{nameof(Material.MaterialNo)}", Global.LogicalOperators.contains, Global.Operators.and, null, true)
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
        /// Gets or sets the current facility lot.
        /// </summary>
        /// <value>The current facility lot.</value>
        [ACPropertyCurrent(601, nameof(FacilityLot), ConstApp.LotNo, Description = "The current facility lot. All property changes must be made on this object, followed by Save(). Set by the navigation methods (e.g. NavigateFirst()) from the FacilityLotList.")]
        public virtual FacilityLot CurrentFacilityLot
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
                if(AccessPrimary.Current != null)
                {
                    AccessPrimary.Current.PropertyChanged-= CurrentFacilityLot_PropertyChanged;
                }
                AccessPrimary.Current = value;
                if (AccessPrimary.Current != null)
                {
                    AccessPrimary.Current.PropertyChanged += CurrentFacilityLot_PropertyChanged;
                }
                OnPropertyChanged();
            }
        }

        public virtual void CurrentFacilityLot_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Comment: can be solved general for refreshing virtual properties on this way
            // but must be performance issues evaulated
            //if (e.PropertyName == nameof(FacilityLot.XMLConfig))
            //{
            //    OnPropertyChanged(nameof(SelectedFacilityLot));
            //}
        }

        /// <summary>
        /// Gets or sets the selected facility lot.
        /// </summary>
        /// <value>The selected facility lot.</value>
        [ACPropertySelected(602, nameof(FacilityLot), ConstApp.LotNo, Description = "The facility lot currently selected in the result grid of the explorer.")]
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
                OnPropertyChanged();
            }
        }

        private List<FacilityLot> _FacilityLotList;
        /// <summary>
        /// Gets the facility lot list.
        /// </summary>
        /// <value>The facility lot list.</value>
        [ACPropertyList(603, nameof(FacilityLot), ConstApp.LotNo, Description = "The result list of the search, sorted by lot number descending; filled by Search() with the lots matching the filter properties.")]
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


        /// <summary>
        /// Gets the current facility lot expiration date helper.
        /// </summary>
        /// <value>The current facility lot expiration date helper.</value>
        [ACPropertyCurrent(604, "FacilityLotExpirationDateHelper", Description = "Helper that calculates the expiration date from production date and storage life (days). Changing one of these values on CurrentFacilityLot recalculates the dependent values.")]
        public FacilityLotExpirationDateHelper CurrentFacilityLotExpirationDateHelper
        {
            get
            {
                return _ExpDateHelper;
            }
            //set;
        }

        /// <summary>
        /// Gets the facility lot expiration date helper list.
        /// </summary>
        /// <value>The facility lot expiration date helper list.</value>
        [ACPropertyList(605, "FacilityLotExpirationDateHelper", Description = "List containing the single CurrentFacilityLotExpirationDateHelper instance (for binding purposes).")]
        public IEnumerable<FacilityLotExpirationDateHelper> FacilityLotExpirationDateHelperList
        {
            get
            {
                List<FacilityLotExpirationDateHelper> dummyList = new List<FacilityLotExpirationDateHelper>();
                dummyList.Add(_ExpDateHelper);
                return dummyList;
            }
        }

        /// <summary>
        /// Gets the selected facility lot expiration date helper.
        /// </summary>
        /// <value>The selected facility lot expiration date helper.</value>
        [ACPropertySelected(606, "FacilityLotExpirationDateHelper", Description = "The selected expiration date helper; always the single CurrentFacilityLotExpirationDateHelper instance.")]
        public FacilityLotExpirationDateHelper SelectedFacilityLotExpirationDateHelper
        {
            get
            {
                return _ExpDateHelper;
            }
            //set;
        }

        public VBDialogResult DialogResult { get; set; }

        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(FacilityLot.ClassName, Const.New, (short)MISort.New, true, "SelectedFacilityLot", Global.ACKinds.MSMethodPrePost, Description = "Creates a new facility lot, generates a new lot number and assigns the new entity object to the CurrentFacilityLot property. Fill in the required fields and call Save() afterwards.")]
        public void New()
        {
            if (!PreExecute("New")) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(FacilityLot), FacilityLot.NoColumnName, FacilityLot.FormatNewNo, this);
            CurrentFacilityLot = FacilityLot.NewACObject(DatabaseApp, null, secondaryKey);
            DatabaseApp.FacilityLot.Add(CurrentFacilityLot);
            AccessPrimary.NavList.Insert(0, CurrentFacilityLot);
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
        [ACMethodCommand(FacilityLot.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = "Saves all changes made on the CurrentFacilityLot to the database.")]
        public async void Save()
        {
            await OnSave();

            //if (await OnSave())
            //    Search();
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
        [ACMethodCommand(FacilityLot.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = "Discards unsaved changes on the CurrentFacilityLot and restores the database state.")]
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
        [ACMethodInteraction(FacilityLot.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedFacilityLot", Global.ACKinds.MSMethodPrePost, Description = "Reloads the SelectedFacilityLot from the database to pick up changes made by other users in the background, and refreshes the expiration date helper (production date, expiration date, storage life).")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<FacilityLot>(requery, () => SelectedFacilityLot, () => CurrentFacilityLot, c => CurrentFacilityLot = c,
                        DatabaseApp.FacilityLot
                        .Where(c => c.FacilityLotID == SelectedFacilityLot.FacilityLotID));
            if (CurrentFacilityLot != null)
            {
                _ExpDateHelper.ResetProperties();
                if (CurrentFacilityLot.ProductionDate != null)
                    _ExpDateHelper.ProductionDate = (DateTime)CurrentFacilityLot.ProductionDate;
                if (CurrentFacilityLot.ExpirationDate != null)
                    _ExpDateHelper.ExpirationDate = (DateTime)CurrentFacilityLot.ExpirationDate;
                _ExpDateHelper.SetStorageLifeInUnits(FacilityLotExpirationDateHelper.TimeSpanUnit.days, CurrentFacilityLot.StorageLife);
            }
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
        /// Deletes this instance.
        /// </summary>
        [ACMethodInteraction(FacilityLot.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentFacilityLot", Global.ACKinds.MSMethodPrePost, Description = "Deletes the CurrentFacilityLot from the database, provided there are no foreign key relationships from other tables (e.g. facility charges). Removes it from the list and selects the first remaining lot.")]
        public void Delete()
        {
            if (!PreExecute("Delete"))
                return;
            if (AccessPrimary == null)
                return;
            Msg msg = CurrentFacilityLot.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }
            AccessPrimary.NavList.Remove(CurrentFacilityLot);
            SelectedFacilityLot = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
            OnPropertyChanged(nameof(FacilityLotList));
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return true;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(FacilityLot.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = "Searches facility lots with the current filter properties and copies the result to the FacilityLotList. Is executed automatically when a filter property changes.")]
        public void Search()
        {
            // If BSO should be opened als a Dialog for gerenating a new lot, then don't run Nav-Search
            if (ACIdentifier.StartsWith(ConstApp.BSOFacilityLot_ChildName) && InitState != ACInitState.Initialized)
                return;

            _FacilityLotList = null;
            if (AccessPrimary != null)
            {
                AccessPrimary.NavSearch(DatabaseApp, MergeOption.OverwriteChanges);
                _FacilityLotList = AccessPrimary.NavList.ToList();
            }

            OnPropertyChanged(nameof(FacilityLotList));
        }


        [ACMethodInfo("Dialog", "en{'New Lot'}de{'Neues Los'}", (short)MISort.QueryPrintDlg, Description = "Creates a new lot (optionally with a preset lot number and material), presets the dates with the current date and opens it in the FacilityLotDialog. If the dialog is not confirmed with OK, the new lot is deleted again.")]
        public async Task<VBDialogResult> ShowDialogNewLot(string lotNo = "", Material material = null)
        {
            if (DialogResult == null)
                DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.Cancel;
            New();
            CurrentFacilityLot.ProductionDate = DateTime.Now;
            CurrentFacilityLot.ExpirationDate = DateTime.Now;
            CurrentFacilityLot.FillingDate = DateTime.Now;
            if (!String.IsNullOrEmpty(lotNo))
                CurrentFacilityLot.LotNo = lotNo;
            if (material != null)
                CurrentFacilityLot.Material = material;
            await ShowDialogAsync(this, "FacilityLotDialog");
            if (DialogResult.SelectedCommand != eMsgButton.OK)
            {
                Delete();
                DialogCancel();
            }
            return DialogResult;
        }

        [ACMethodCommand("Dialog", Const.Ok, (short)MISort.Okay, Description = "Confirms the FacilityLotDialog: returns the CurrentFacilityLot as dialog result and closes the dialog.")]
        public void DialogOK()
        {
            if (DialogResult != null)
            {
                DialogResult.SelectedCommand = eMsgButton.OK;
                DialogResult.ReturnValue = CurrentFacilityLot;
            }
            CloseTopDialog();
        }

        [ACMethodCommand("Dialog", Const.Cancel, (short)MISort.Cancel, Description = "Cancels the FacilityLotDialog without returning a lot.")]
        public void DialogCancel()
        {
            CloseTopDialog();
        }

        #region Show order dialog

        [ACMethodInfo("Dialog", "en{'Dialog Lot'}de{'Dialog Los'}", (short)MISort.QueryPrintDlg, Description = "Opens the FacilityLotDialog filtered to the given lot number and stops this BSO instance after the dialog is closed.")]
        public async Task ShowDialogOrder(string lotNo)
        {
            if (AccessPrimary == null)
                return;
            //AccessPrimary.NavACQueryDefinition.SearchWord = facilityNo;
            ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "LotNo").FirstOrDefault();
            if (filterItem == null)
            {
                filterItem = new ACFilterItem(Global.FilterTypes.filter, "LotNo", Global.LogicalOperators.contains, Global.Operators.and, lotNo, false);
                AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);
            }
            else
                filterItem.SearchWord = lotNo;

            this.Search();
            await ShowDialogAsync(this, "FacilityLotDialog");
            await this.ParentACComponent.StopComponent(this);
        }

        [ACMethodInfo("Dialog", "en{'Dialog Lot'}de{'Dialog Los'}", (short)MISort.QueryPrintDlg + 1, Description = "Opens the lot dialog for the FacilityLot referenced in the given PAOrderInfo and writes the dialog result back to the PAOrderInfo.")]
        public async Task ShowDialogOrderInfo(PAOrderInfo paOrderInfo)
        {
            if (AccessPrimary == null || paOrderInfo == null)
                return;

            FacilityLot facilityLot = null;
            foreach (var entry in paOrderInfo.Entities)
            {
                if (entry.EntityName == FacilityLot.ClassName)
                {
                    facilityLot = this.DatabaseApp.FacilityLot
                        .Where(c => c.FacilityLotID == entry.EntityID)
                        .FirstOrDefault();
                    break;
                }
            }

            if (facilityLot == null)
                return;

            await ShowDialogOrder(facilityLot.LotNo);
            paOrderInfo.DialogResult = this.DialogResult;
        }
        #endregion

        #region Navigation
        [ACMethodInteraction("", "en{'Show Lot Stock and History'}de{'Zeige Losbestand und Historie'}", 781, true, nameof(SelectedFacilityLot), Description = "Opens the lot stock and history overview for the SelectedFacilityLot. Enabled only if a lot is selected.")]
        public async Task NavigateToFacilityLotOverview()
        {
            if (!IsEnabledNavigateToFacilityLotOverview())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo() { DialogSelectInfo = 1 };
                info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityLot), SelectedFacilityLot.FacilityLotID));
                await service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacilityLotOverview()
        {
            if (SelectedFacilityLot != null)
                return true;
            return false;
        }
        #endregion

        #endregion

        #region Eventhandling
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
        [ACMethodInfo(FacilityLot.ClassName, "en{'Activate'}de{'Aktiviere'}", 9999, true, Global.ACKinds.MSMethodPrePost, Description = "Internal event handler that is called when a tab item of the design is activated; contains no additional logic.")]
        public void OnActivate(string page)
        {
            if (!PreExecute("OnActivate"))
                return;
            PostExecute("OnActivate");

        }

        /// <summary>
        /// BSOs the facility lot_ on value changed.
        /// </summary>
        /// <param name="dataContent">Content of the data.</param>
        void BSOFacilityLot_OnValueChanged(string dataContent)
        {
            switch (dataContent)
            {
                case "CurrentFacilityLot.StorageLife":
                    if (_ExpDateHelper.OnStorageLifeChanged(FacilityLotExpirationDateHelper.TimeSpanUnit.days, CurrentFacilityLot.StorageLife) == FacilityLotExpirationDateHelper.ChangeResult.ValueSetDepValChanged)
                    {
                        if (CurrentFacilityLot != null)
                        {
                            CurrentFacilityLot.ProductionDate = _ExpDateHelper.ProductionDate;
                            CurrentFacilityLot.ExpirationDate = _ExpDateHelper.ExpirationDate;
                        }
                    }
                    break;
                case "CurrentFacilityLot.ProductionDate":
                    if (CurrentFacilityLot.ProductionDate != null)
                    {
                        if (_ExpDateHelper.OnProductionDateChanged((DateTime)CurrentFacilityLot.ProductionDate) == FacilityLotExpirationDateHelper.ChangeResult.ValueSetDepValChanged)
                        {
                            CurrentFacilityLot.ExpirationDate = _ExpDateHelper.ExpirationDate;
                            CurrentFacilityLot.StorageLife = (short)_ExpDateHelper.StorageLife.Days;
                        }
                    }
                    break;
                case "CurrentFacilityLot.ExpirationDate":
                    if (CurrentFacilityLot.ExpirationDate != null)
                    {
                        if (_ExpDateHelper.OnExpirationDateChanged((DateTime)CurrentFacilityLot.ExpirationDate) == FacilityLotExpirationDateHelper.ChangeResult.ValueSetDepValChanged)
                        {
                            CurrentFacilityLot.ProductionDate = _ExpDateHelper.ProductionDate;
                            CurrentFacilityLot.StorageLife = (short)_ExpDateHelper.StorageLife.Days;
                        }
                    }
                    break;
            }

        }

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
                case nameof(ShowDialogNewLot):
                    result = ShowDialogNewLot(acParameter.Count() == 1 ? (string)acParameter[0] : "", acParameter.Count() == 2 ? (Material)acParameter[1] : null);
                    return true;
                case nameof(ShowDialogOrder):
                    result = ShowDialogOrder((String)acParameter[0]);
                    return true;
                case nameof(ShowDialogOrderInfo):
                    result = ShowDialogOrderInfo((PAOrderInfo)acParameter[0]);
                    return true;
                case nameof(DialogOK):
                    DialogOK();
                    return true;
                case nameof(DialogCancel):
                    DialogCancel();
                    return true;
                case nameof(OnActivate):
                    OnActivate((String)acParameter[0]);
                    return true;
                case nameof(NavigateToFacilityLotOverview):
                    result = NavigateToFacilityLotOverview();
                    return true;
                case nameof(IsEnabledNavigateToFacilityLotOverview):
                    result = IsEnabledNavigateToFacilityLotOverview();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override void OnPropertyChanged([CallerMemberName] string name = "")
        {
            base.OnPropertyChanged(name);
            if (name == nameof(FilterMaterialNo)
                || name == nameof(FilterLotNo)
                || name == nameof(FilterExternLotNo)
                || name == nameof(FilterExternLotNo2))
            {
                Search();
            }
        }

        #endregion

        #region GetPropsToObserveForIsEnabled
        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(New):
                case nameof(IsEnabledNew):
                case nameof(Save):
                case nameof(IsEnabledSave):
                case nameof(UndoSave):
                case nameof(IsEnabledUndoSave):
                case nameof(Load):
                case nameof(IsEnabledLoad):
                case nameof(Delete):
                case nameof(IsEnabledDelete):
                case nameof(Search):
                    return new[] { "CurrentFacilityLot", "FilterMaterialNo", "FilterLotNo", "FilterExternLotNo", "FilterExternLotNo2" };
                case nameof(ShowDialogNewLot):
                case nameof(ShowDialogOrder):
                case nameof(ShowDialogOrderInfo):
                    return new[] { "CurrentFacilityLot" };
                case nameof(DialogOK):
                case nameof(DialogCancel):
                    return Array.Empty<string>();
                case nameof(OnActivate):
                    return new[] { "CurrentFacilityLot" };
                case nameof(NavigateToFacilityLotOverview):
                case nameof(IsEnabledNavigateToFacilityLotOverview):
                    return new[] { "SelectedFacilityLot" };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }
        #endregion

    }
}
