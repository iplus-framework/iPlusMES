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

namespace gip.bso.facility
{
    /// <summary>
    /// FacilityLot manages Lots from the logistical point of view.
    /// Facility Lots contains information about Suppliers and quality relevant data from production
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Lot Management'}de{'Losverwaltung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + FacilityLot.ClassName)]
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

        [ACPropertyInfo(999, nameof(FilterExternLotNo2), ConstApp.ExternLotNo2)]
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
        [ACPropertyAccessPrimary(690, FacilityLot.ClassName)]
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
        [ACPropertyCurrent(601, nameof(FacilityLot), ConstApp.LotNo)]
        public FacilityLot CurrentFacilityLot
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
            }
        }

        /// <summary>
        /// Gets or sets the selected facility lot.
        /// </summary>
        /// <value>The selected facility lot.</value>
        [ACPropertySelected(602, nameof(FacilityLot), ConstApp.LotNo)]
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
        [ACPropertyList(603, nameof(FacilityLot), ConstApp.LotNo)]
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
        [ACPropertyCurrent(604, "FacilityLotExpirationDateHelper")]
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
        [ACPropertyList(605, "FacilityLotExpirationDateHelper")]
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
        [ACPropertySelected(606, "FacilityLotExpirationDateHelper")]
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
        [ACMethodInteraction(FacilityLot.ClassName, Const.New, (short)MISort.New, true, "SelectedFacilityLot", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(FacilityLot), FacilityLot.NoColumnName, FacilityLot.FormatNewNo, this);
            CurrentFacilityLot = FacilityLot.NewACObject(DatabaseApp, null, secondaryKey);
            DatabaseApp.FacilityLot.Add(CurrentFacilityLot);
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
        [ACMethodCommand(FacilityLot.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public async void Save()
        {
            if (await OnSave())
                Search();
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
        [ACMethodInteraction(FacilityLot.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedFacilityLot", Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction(FacilityLot.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentFacilityLot", Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand(FacilityLot.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
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


        [ACMethodInfo("Dialog", "en{'New Lot'}de{'Neues Los'}", (short)MISort.QueryPrintDlg)]
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

        [ACMethodCommand("Dialog", Const.Ok, (short)MISort.Okay)]
        public void DialogOK()
        {
            if (DialogResult != null)
            {
                DialogResult.SelectedCommand = eMsgButton.OK;
                DialogResult.ReturnValue = CurrentFacilityLot;
            }
            CloseTopDialog();
        }

        [ACMethodCommand("Dialog", Const.Cancel, (short)MISort.Cancel)]
        public void DialogCancel()
        {
            CloseTopDialog();
        }

        #region Show order dialog

        [ACMethodInfo("Dialog", "en{'Dialog Lot'}de{'Dialog Los'}", (short)MISort.QueryPrintDlg)]
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

        [ACMethodInfo("Dialog", "en{'Dialog Lot'}de{'Dialog Los'}", (short)MISort.QueryPrintDlg + 1)]
        public void ShowDialogOrderInfo(PAOrderInfo paOrderInfo)
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

            ShowDialogOrder(facilityLot.LotNo);
            paOrderInfo.DialogResult = this.DialogResult;
        }
        #endregion

        #region Navigation
        [ACMethodInteraction("", "en{'Show Lot Stock and History'}de{'Zeige Losbestand und Historie'}", 781, true, nameof(SelectedFacilityLot))]
        public void NavigateToFacilityLotOverview()
        {
            if (!IsEnabledNavigateToFacilityLotOverview())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo() { DialogSelectInfo = 1 };
                info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityLot), SelectedFacilityLot.FacilityLotID));
                service.ShowDialogOrder(this, info);
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
        [ACMethodInfo(FacilityLot.ClassName, "en{'Activate'}de{'Aktiviere'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
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
                    ShowDialogOrder((String)acParameter[0]);
                    return true;
                case nameof(ShowDialogOrderInfo):
                    ShowDialogOrderInfo((PAOrderInfo)acParameter[0]);
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
                    NavigateToFacilityLotOverview();
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

    }
}
