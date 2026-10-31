// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.bso.masterdata;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static gip.core.datamodel.Global;

namespace gip.bso.facility
{
    /// <summary>
    /// Business object (App) for stock-taking (physical inventory) of warehouse stock.
    /// This class extends the <see cref="ACBSOvbNav"/> and manages FacilityInventory records (inventory headers) with their inventory lines (FacilityInventoryPos).
    /// To search inventories set the FilterInventoryStartDate and FilterInventoryEndDate (by default the current year) and optionally select a state in SelectedFilterInventoryState, then call Search().
    /// The database result is copied to the FacilityInventoryList property.
    /// Then call NavigateFirst() to set CurrentFacilityInventory / SelectedFacilityInventory with the first record of the list.
    /// Property changes should always be made on SelectedFacilityInventory and the Save() method should be called before navigating to another record or creating a new one.
    /// The New() method opens the dialog for a new inventory: enter the inventory number (NewFaciltiyInventoryNo, suggested automatically) and a name (NewFaciltiyInventoryName), optionally choose a storage bin with ShowFaciltiyDialog() (result in SelectedNewInventoryFacility).
    /// With GenerateInventoryPosition it is decided whether inventory lines are generated for the existing quants; with OmitGenerateSiloQuantPosition silo quants can be excluded from generation.
    /// Confirm with NewDlgOk() (or close the dialog with NewDlgOkCancel()); the inventory is created in the background via FacilityManager.InventoryGenerate.
    /// If an inventory has no lines, GeneratePositions() opens the dialog to generate the lines afterwards; confirm with GeneratePositionsOK().
    /// The counting workflow: 1.) StartInventory() sets the inventory state to "In progress" (only possible for new inventories).
    /// 2.) Count the lines: the lines of the selected inventory are in FacilityInventoryPosList, the selected line in SelectedFacilityInventoryPos.
    /// The lines can be filtered with FilterMaterial, FilterFacility, FilterInventoryPosLotNo, SelectedFilterInventoryPosState, FilterOpenLines, FilterNewStockQuantityDifferent and the barcode InputCode; SearchPos() reapplies the filter, ClearSearchPos() clears it, PositionSelectAll selects all lines, SelectPos() selects the line matching a scanned barcode (FacilityChargeID).
    /// Enter the counted quantity in SelectedFacilityInventoryPos.NewStockQuantity or mark the quant as SelectedFacilityInventoryPos.NotAvailable; CopyQuantityFromStock() takes over the stock quantity as counted quantity.
    /// StartInventoryPos() sets the selected line to "In progress", ClosingInventoryPos() finishes it; CloseAllPositions() marks all selected lines as counted.
    /// Mass functions for the selected lines (checkbox IsSelected): CopyQuantityFromStockForSelected() takes over the stock quantity, NotAvailableForSelected() marks them as not available, InventoryPosStateMassChange() sets the state selected in SelectedPosStateMassChange.
    /// 3.) ClosingInventory() posts the inventory and completes it (FacilityManager.InventoryClosing); if unclosed lines exist the user is warned first. SendToERP() marks a finished or canceled inventory as posted.
    /// CancelInventory() cancels the inventory and reverts all bookings made by it (FacilityManager.CancelInventory).
    /// Quants that were not included in the inventory can be added afterwards: NotUsedFacilityChargeList (SearchNotUsedFaciltiyCharge()) lists unused quants, AddFacilityCharge() ("Restore quant") adds the selected one to the inventory, NewFacilityCharge() creates a new quant via dialog and adds it to the inventory.
    /// The bookings of the selected line are in InventoryPosFacilityBookingList and InventoryPosFacilityBookingChargeList, the bookings of the whole inventory in InventoryFacilityBookingChargeList (SearchFacilityInventoryFacilityBookingCharge()).
    /// ChangeInventoryFacility() changes the storage bin of the inventory (only possible as long as no lines exist).
    /// Delete() deletes the selected inventory with all its lines (only possible in state New or In progress); DeletePos() is a placeholder without functionality.
    /// Load() re-reads the selected inventory if it was changed by other users in the meantime, UndoSave() discards unsaved changes.
    /// Configuration: OmitGenerateSiloQuantPositionConfig, GenerateInventoryPositionConfig and DefaultSuggestStockQuantity control the defaults for new inventories.
    /// Visit https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityInventory&type=code
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Warehouse Inventory'}de{'Lager Inventur'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + nameof(FacilityInventory),
                 Description = @"Business object (App) for stock-taking (physical inventory) of warehouse stock.
                                This class extends the <see cref=""ACBSOvbNav""/> and manages FacilityInventory records (inventory headers) with their inventory lines (FacilityInventoryPos).
                                To search inventories set the FilterInventoryStartDate and FilterInventoryEndDate (by default the current year) and optionally select a state in SelectedFilterInventoryState, then call Search().
                                The database result is copied to the FacilityInventoryList property.
                                Then call NavigateFirst() to set CurrentFacilityInventory / SelectedFacilityInventory with the first record of the list.
                                Property changes should always be made on SelectedFacilityInventory and the Save() method should be called before navigating to another record or creating a new one.
                                The New() method opens the dialog for a new inventory: enter the inventory number (NewFaciltiyInventoryNo, suggested automatically) and a name (NewFaciltiyInventoryName), optionally choose a storage bin with ShowFaciltiyDialog() (result in SelectedNewInventoryFacility).
                                With GenerateInventoryPosition it is decided whether inventory lines are generated for the existing quants; with OmitGenerateSiloQuantPosition silo quants can be excluded from generation.
                                Confirm with NewDlgOk() (or close the dialog with NewDlgOkCancel()); the inventory is created in the background via FacilityManager.InventoryGenerate.
                                If an inventory has no lines, GeneratePositions() opens the dialog to generate the lines afterwards; confirm with GeneratePositionsOK().
                                The counting workflow: 1.) StartInventory() sets the inventory state to ""In progress"" (only possible for new inventories).
                                2.) Count the lines: the lines of the selected inventory are in FacilityInventoryPosList, the selected line in SelectedFacilityInventoryPos.
                                The lines can be filtered with FilterMaterial, FilterFacility, FilterInventoryPosLotNo, SelectedFilterInventoryPosState, FilterOpenLines, FilterNewStockQuantityDifferent and the barcode InputCode; SearchPos() reapplies the filter, ClearSearchPos() clears it, PositionSelectAll selects all lines, SelectPos() selects the line matching a scanned barcode (FacilityChargeID).
                                Enter the counted quantity in SelectedFacilityInventoryPos.NewStockQuantity or mark the quant as SelectedFacilityInventoryPos.NotAvailable; CopyQuantityFromStock() takes over the stock quantity as counted quantity.
                                StartInventoryPos() sets the selected line to ""In progress"", ClosingInventoryPos() finishes it; CloseAllPositions() marks all selected lines as counted.
                                Mass functions for the selected lines (checkbox IsSelected): CopyQuantityFromStockForSelected() takes over the stock quantity, NotAvailableForSelected() marks them as not available, InventoryPosStateMassChange() sets the state selected in SelectedPosStateMassChange.
                                3.) ClosingInventory() posts the inventory and completes it (FacilityManager.InventoryClosing); if unclosed lines exist the user is warned first. SendToERP() marks a finished or canceled inventory as posted.
                                CancelInventory() cancels the inventory and reverts all bookings made by it (FacilityManager.CancelInventory).
                                Quants that were not included in the inventory can be added afterwards: NotUsedFacilityChargeList (SearchNotUsedFaciltiyCharge()) lists unused quants, AddFacilityCharge() (""Restore quant"") adds the selected one to the inventory, NewFacilityCharge() creates a new quant via dialog and adds it to the inventory.
                                The bookings of the selected line are in InventoryPosFacilityBookingList and InventoryPosFacilityBookingChargeList, the bookings of the whole inventory in InventoryFacilityBookingChargeList (SearchFacilityInventoryFacilityBookingCharge()).
                                ChangeInventoryFacility() changes the storage bin of the inventory (only possible as long as no lines exist).
                                Delete() deletes the selected inventory with all its lines (only possible in state New or In progress); DeletePos() is a placeholder without functionality.
                                Load() re-reads the selected inventory if it was changed by other users in the meantime, UndoSave() discards unsaved changes.
                                Configuration: OmitGenerateSiloQuantPositionConfig, GenerateInventoryPositionConfig and DefaultSuggestStockQuantity control the defaults for new inventories.
                                Visit https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityInventory&type=code")]
    [ACQueryInfo(Const.PackName_VarioFacility, Const.QueryPrefix + nameof(FacilityInventory), "en{'FacilityInventory'}de{'FacilityInventory'}", typeof(FacilityInventory), nameof(FacilityInventory), nameof(FacilityInventory.FacilityInventoryNo), nameof(FacilityInventory.FacilityInventoryNo))]

    public class BSOFacilityInventory : ACBSOvbNav, IOnTrackingCall
    {

        #region c´tors
        /// <summary>Initializes a new instance of the class.</summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityInventory(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _OmitGenerateSiloQuantPositionConfig = new ACPropertyConfigValue<bool>(this, nameof(OmitGenerateSiloQuantPositionConfig), true);
            _GenerateInventoryPositionConfig = new ACPropertyConfigValue<bool>(this, nameof(GenerateInventoryPositionConfig), true);
            _DefaultSuggestStockQuantity = new ACPropertyConfigValue<bool>(this, nameof(DefaultSuggestStockQuantity), true);
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

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            _= OmitGenerateSiloQuantPositionConfig;
            _= GenerateInventoryPositionConfig;
            _= DefaultSuggestStockQuantity;

            LoadInitialFilterInventoryDates();
            IsEnabledInventoryPosEdit = false;

            _GenerateInventoryPosition = GenerateInventoryPositionConfig;
            _OmitGenerateSiloQuantPosition = OmitGenerateSiloQuantPositionConfig;
            FinishedPosState = DatabaseApp.MDFacilityInventoryPosState.Where(c => c.MDFacilityInventoryPosStateIndex == (short)FacilityInventoryPosStateEnum.Finished).FirstOrDefault();

            Search();
            return true;
        }

        private void LoadInitialFilterInventoryDates()
        {
            FilterInventoryStartDate = new DateTime(DateTime.Now.Year, 1, 1);
            FilterInventoryEndDate = FilterInventoryStartDate.AddYears(1);
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            var b = await base.ACDeInit();

            FinishedPosState = null;

            if (_ACFacilityManager != null)
                FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;

            return b;
        }

        #endregion

        #region Configuration

        protected ACPropertyConfigValue<bool> _OmitGenerateSiloQuantPositionConfig;
        [ACPropertyConfig("en{'Omit silo quants'}de{'Silo-Quants weglassen'}", Description = "Configuration: if true, silo quants are omitted when generating inventory lines.")]
        public bool OmitGenerateSiloQuantPositionConfig
        {
            get
            {
                return _OmitGenerateSiloQuantPositionConfig.ValueT;
            }
            set
            {
                _OmitGenerateSiloQuantPositionConfig.ValueT = value;
            }
        }

        protected ACPropertyConfigValue<bool> _GenerateInventoryPositionConfig;
        [ACPropertyConfig("en{'Generate positions'}de{'Positionen generieren'}", Description = "Configuration: if true, inventory lines are generated for the existing quants when creating a new inventory.")]
        public bool GenerateInventoryPositionConfig
        {
            get
            {
                return _GenerateInventoryPositionConfig.ValueT;
            }
            set
            {
                _GenerateInventoryPositionConfig.ValueT = value;
            }
        }

        protected ACPropertyConfigValue<bool> _DefaultSuggestStockQuantity;
        [ACPropertyConfig("en{'Default suggest stock quantity'}de{'Standardmäßig Vorschlag Lagerbestand'}", Description = "Configuration: if true, the current stock quantity is preassigned as counted quantity suggestion for new inventory lines.")]
        public bool DefaultSuggestStockQuantity
        {
            get
            {
                return _DefaultSuggestStockQuantity.ValueT;
            }
            set
            {
                _DefaultSuggestStockQuantity.ValueT = value;
            }
        }

        #endregion

        #region Managers

        protected ACRef<ACComponent> _ACFacilityManager = null;
        public FacilityManager ACFacilityManager
        {
            get
            {
                if (_ACFacilityManager == null)
                    return null;
                return _ACFacilityManager.ValueT as FacilityManager;
            }
        }

        #endregion

        #region ChildBSO

        ACChildItem<BSOFacilityExplorer> _BSOFacilityExplorer_Child;
        [ACPropertyInfo(600, Description = "Child BSO for selecting a storage bin (Facility) in the dialogs.")]
        [ACChildInfo(nameof(BSOFacilityExplorer_Child), typeof(BSOFacilityExplorer))]
        public ACChildItem<BSOFacilityExplorer> BSOFacilityExplorer_Child
        {
            get
            {
                if (_BSOFacilityExplorer_Child == null)
                    _BSOFacilityExplorer_Child = new ACChildItem<BSOFacilityExplorer>(this, nameof(BSOFacilityExplorer_Child));
                return _BSOFacilityExplorer_Child;
            }
        }

        #endregion

        #region Properties

        public MDFacilityInventoryPosState FinishedPosState { get; set; }

        #region Properties -> Filter

        #region Properties -> Filter -> Inventory

        private DateTime _FilterInventoryStartDate;
        [ACPropertyInfo(90, nameof(FilterInventoryStartDate), Const.From, Description = "Start of the date range (insert date) used to filter the inventory list.")]
        public DateTime FilterInventoryStartDate
        {
            get
            {
                return _FilterInventoryStartDate;
            }
            set
            {
                if (_FilterInventoryStartDate != value)
                {
                    _FilterInventoryStartDate = value;
                    OnPropertyChanged(nameof(FilterInventoryStartDate));
                }
            }
        }

        private DateTime _FilterInventoryEndDate;
        [ACPropertyInfo(101, nameof(FilterInventoryEndDate), "en{'to'}de{'bis'}", Description = "End of the date range (insert date) used to filter the inventory list.")]
        public DateTime FilterInventoryEndDate
        {
            get
            {
                return _FilterInventoryEndDate;
            }
            set
            {
                if (_FilterInventoryEndDate != value)
                {
                    _FilterInventoryEndDate = value;
                    OnPropertyChanged(nameof(FilterInventoryEndDate));
                }
            }
        }

        #region Properties -> Filter-> FilterInventoryState (MDFacilityInventoryState)
        public const string FilterInventoryState = "FilterInventoryState";
        private MDFacilityInventoryState _SelectedFilterInventoryState;
        /// <summary>
        /// Selected property for MDFacilityInventoryState
        /// </summary>
        /// <value>The selected FilterInventoryState</value>
        [ACPropertySelected(9999, nameof(FilterInventoryState), ConstApp.ESFacilityInventoryState, Description = "Gets or sets the inventory state (MDFacilityInventoryState) used to filter the inventory list.")]
        public MDFacilityInventoryState SelectedFilterInventoryState
        {
            get
            {
                return _SelectedFilterInventoryState;
            }
            set
            {
                if (_SelectedFilterInventoryState != value)
                {
                    _SelectedFilterInventoryState = value;
                    OnPropertyChanged(nameof(SelectedFilterInventoryState));
                }
            }
        }


        private List<MDFacilityInventoryState> _FilterInventoryStateList;
        /// <summary>
        /// List property for MDFacilityInventoryState
        /// </summary>
        /// <value>The FilterInventoryState list</value>
        [ACPropertyList(9999, nameof(FilterInventoryState), Description = "Gets the list of all inventory states available for the inventory filter.")]
        public List<MDFacilityInventoryState> FilterInventoryStateList
        {
            get
            {
                if (_FilterInventoryStateList == null)
                    _FilterInventoryStateList = LoadFilterInventoryStateList();
                return _FilterInventoryStateList;
            }
        }

        private List<MDFacilityInventoryState> LoadFilterInventoryStateList()
        {
            return DatabaseApp.MDFacilityInventoryState.OrderBy(c => c.SortIndex).ToList();
        }

        #endregion

        /// <summary>
        /// Source Property: 
        /// </summary>
        private bool _GenerateInventoryPosition;
        [ACPropertySelected(999, nameof(GenerateInventoryPosition), "en{'Generate positions'}de{'Positionen generieren'}", Description = "Gets or sets whether inventory lines are generated for the existing quants when creating an inventory or generating positions.")]
        public bool GenerateInventoryPosition
        {
            get
            {
                return _GenerateInventoryPosition;
            }
            set
            {
                if (_GenerateInventoryPosition != value)
                {
                    _GenerateInventoryPosition = value;
                    OnPropertyChanged(nameof(GenerateInventoryPosition));
                }
            }
        }

        /// <summary>
        /// Default not include generate inventory positions for silo quants 
        /// </summary>
        private bool _OmitGenerateSiloQuantPosition;
        [ACPropertyInfo(999, nameof(OmitGenerateSiloQuantPosition), "en{'Omit silo quants'}de{'Silo-Quants weglassen'}", Description = "Gets or sets whether silo quants are excluded when generating inventory lines.")]
        public bool OmitGenerateSiloQuantPosition
        {
            get
            {
                return _OmitGenerateSiloQuantPosition;
            }
            set
            {
                if (_OmitGenerateSiloQuantPosition != value)
                {
                    _OmitGenerateSiloQuantPosition = value;
                    OnPropertyChanged();
                }
            }
        }

        #region Properties -> Filter -> NewInventoryFacility
        public const string NewInventoryFacility = "NewInventoryFacility";
        private Facility _SelectedNewInventoryFacility;
        /// <summary>
        /// Selected property for Facility
        /// </summary>
        /// <value>The selected NewInventoryFacility</value>
        [ACPropertySelected(9999, nameof(NewInventoryFacility), ConstApp.Facility, Description = "Gets or sets the storage bin (Facility) selected in the new-inventory dialog for which the inventory is created.")]
        public Facility SelectedNewInventoryFacility
        {
            get
            {
                return _SelectedNewInventoryFacility;
            }
            set
            {
                if (_SelectedNewInventoryFacility != value)
                {
                    _SelectedNewInventoryFacility = value;
                    OnPropertyChanged(nameof(SelectedNewInventoryFacility));
                    OnPropertyChanged(nameof(SelectedFacilityInventory));
                }
            }
        }

        private List<Facility> _NewInventoryFacilityList;
        /// <summary>
        /// List property for Facility
        /// </summary>
        /// <value>The NewInventoryFacility list</value>
        [ACPropertyList(9999, nameof(NewInventoryFacility), Description = "Gets the list of all storage bins available for selection in the new-inventory dialog.")]
        public List<Facility> NewInventoryFacilityList
        {
            get
            {
                if (_NewInventoryFacilityList == null)
                    _NewInventoryFacilityList = DatabaseApp.Facility.OrderBy(c => c.FacilityNo).ToList();
                return _NewInventoryFacilityList;
            }
        }

        #endregion

        #endregion

        #region Properties -> Filter -> InventoryPos

        #region Properties -> Filter -> InventoryPos -> Material

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilterMaterial;
        [ACPropertyInfo(999, nameof(FilterMaterial), ConstApp.Material, Description = "Filter for the inventory lines: material number or material name of the quant.")]
        public string FilterMaterial
        {
            get
            {
                return _FilterMaterial;
            }
            set
            {
                if (_FilterMaterial != value)
                {
                    _FilterMaterial = value;
                    OnPropertyChanged();
                    SearchPos();
                }
            }
        }

        #endregion

        #region Properties -> Filter -> InventoryPos -> Facility

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilterFacility;
        [ACPropertyInfo(999, nameof(FilterFacility), ConstApp.Facility, Description = "Filter for the inventory lines: storage bin number or name of the quant.")]
        public string FilterFacility
        {
            get
            {
                return _FilterFacility;
            }
            set
            {
                if (_FilterFacility != value)
                {
                    _FilterFacility = value;
                    OnPropertyChanged();
                    SearchPos();
                }
            }
        }

        #endregion


        #region Properties -> Filter -> InventoryPos -> (Quantity) State

        private string _FilterInventoryPosLotNo;
        [ACPropertyInfo(104, nameof(FilterInventoryPosLotNo), ConstApp.LotNo, Description = "Filter for the inventory lines: lot number of the quant.")]
        public string FilterInventoryPosLotNo
        {
            get
            {
                return _FilterInventoryPosLotNo;
            }
            set
            {
                if (_FilterInventoryPosLotNo != value)
                {
                    _FilterInventoryPosLotNo = value;
                    OnPropertyChanged(nameof(FilterInventoryPosLotNo));
                    SearchPos();
                }
            }
        }

        private bool? _FilterInventoryPosNotAvailable;
        [ACPropertyInfo(105, nameof(FilterInventoryPosNotAvailable), ConstApp.NotAvailable, Description = "Filter for the inventory lines: shows only lines marked as not available (true) or available (false).")]
        public bool? FilterInventoryPosNotAvailable
        {
            get
            {
                return _FilterInventoryPosNotAvailable;
            }
            set
            {
                if (_FilterInventoryPosNotAvailable != value)
                {
                    _FilterInventoryPosNotAvailable = value;
                    OnPropertyChanged(nameof(FilterInventoryPosNotAvailable));
                    SearchPos();
                }
            }
        }

        private bool? _FilterInventoryPosZeroQuantity;
        [ACPropertyInfo(105, nameof(FilterInventoryPosNotAvailable), "en{'Empty quant'}de{'Leeres Quant'}", Description = "Filter for the inventory lines: shows only empty quants (true) or non-empty quants (false). Currently not evaluated by the line filter.")]
        public bool? FilterInventoryPosZeroQuantity
        {
            get
            {
                return _FilterInventoryPosZeroQuantity;
            }
            set
            {
                if (_FilterInventoryPosZeroQuantity != value)
                {
                    _FilterInventoryPosZeroQuantity = value;
                    OnPropertyChanged(nameof(FilterInventoryPosZeroQuantity));
                    SearchPos();
                }
            }
        }

        private bool? _FilterOpenLines;
        [ACPropertyInfo(106, nameof(FilterOpenLines), "en{'View open lines'}de{'Offene Zeilen anzeigen'}", Description = "Filter for the inventory lines: if true, only open (not yet counted) lines are shown; if false, only finished lines.")]
        public bool? FilterOpenLines
        {
            get
            {
                return _FilterOpenLines;
            }
            set
            {
                if (_FilterOpenLines != value)
                {
                    _FilterOpenLines = value;
                    OnPropertyChanged(nameof(FilterOpenLines));
                    SearchPos();
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private bool? _FilterNewStockQuantityDifferent;
        [ACPropertyInfo(999, nameof(FilterNewStockQuantityDifferent), "en{'Lines whose inventory has changed'}de{'Positionen deren Lagerbestand sich geändert hat'}", Description = "Filter for the inventory lines: if true, only lines whose counted quantity differs from the stock quantity are shown.")]
        public bool? FilterNewStockQuantityDifferent
        {
            get
            {
                return _FilterNewStockQuantityDifferent;
            }
            set
            {
                if (_FilterNewStockQuantityDifferent != value)
                {
                    _FilterNewStockQuantityDifferent = value;
                    OnPropertyChanged();
                    SearchPos();
                }
            }
        }


        #endregion

        #region Properties -> Filter -> InventoryPos -> State (FilterInventoryPosState) (MDFacilityInventoryPosState)
        public const string FilterInventoryPosState = "FilterInventoryPosState";
        private MDFacilityInventoryPosState _SelectedFilterInventoryPosState;
        /// <summary>
        /// Selected property for MDFacilityInventoryPosState
        /// </summary>
        /// <value>The selected FilterInventoryPosState</value>
        [ACPropertySelected(9999, nameof(FilterInventoryPosState), ConstApp.ESFacilityInventoryPosState, Description = "Gets or sets the inventory line state (MDFacilityInventoryPosState) used to filter the inventory lines.")]
        public MDFacilityInventoryPosState SelectedFilterInventoryPosState
        {
            get
            {
                return _SelectedFilterInventoryPosState;
            }
            set
            {
                if (_SelectedFilterInventoryPosState != value)
                {
                    _SelectedFilterInventoryPosState = value;
                    OnPropertyChanged(nameof(SelectedFilterInventoryPosState));
                    SearchPos();
                }
            }
        }


        private List<MDFacilityInventoryPosState> _FilterInventoryPosStateList;
        /// <summary>
        /// List property for MDFacilityInventoryPosState
        /// </summary>
        /// <value>The FilterInventoryPosState list</value>
        [ACPropertyList(9999, nameof(FilterInventoryPosState), Description = "Gets the list of all inventory line states available for the line filter.")]
        public List<MDFacilityInventoryPosState> FilterInventoryPosStateList
        {
            get
            {
                if (_FilterInventoryPosStateList == null)
                    _FilterInventoryPosStateList = LoadFilterInventoryPosStateList();
                return _FilterInventoryPosStateList;
            }
        }

        private List<MDFacilityInventoryPosState> LoadFilterInventoryPosStateList()
        {
            return DatabaseApp.MDFacilityInventoryPosState.OrderBy(c => c.MDFacilityInventoryPosStateIndex).ToList();
        }

        #endregion

        private int _FilterPosPageSize = 500;
        [ACPropertyInfo(120, nameof(FilterPosPageSize), "en{'Limit Record Count'}de{'Limit Anzahl Datensätze'}", Description = "Maximum number of inventory lines loaded per search (default 500).")]
        public int FilterPosPageSize
        {
            get
            {
                return _FilterPosPageSize;
            }
            set
            {
                if (_FilterPosPageSize != value)
                {
                    _FilterPosPageSize = value;
                    OnPropertyChanged(nameof(FilterPosPageSize));

                    SearchPos();
                }
            }
        }

        /// <summary>
        /// Select all
        /// </summary>
        private bool _PositionSelectAll;
        [ACPropertyInfo(999, nameof(PositionSelectAll), "en{'Select all'}de{'Alles auswählen'}", Description = "Selects or deselects all inventory lines (IsSelected) for the mass functions.")]
        public bool PositionSelectAll
        {
            get
            {
                return _PositionSelectAll;
            }
            set
            {
                if (_PositionSelectAll != value)
                {
                    _PositionSelectAll = value;
                    OnPropertyChanged();

                    if (FacilityInventoryPosList != null)
                    {
                        foreach (var item in FacilityInventoryPosList)
                        {
                            item.IsSelected = value;
                        }
                    }

                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #endregion

        #region Properties -> Editor

        #region Properties -> Editor -> Pos Editor Enabled

        private bool _IsEnabledInventoryEdit;
        public bool IsEnabledInventoryEdit
        {
            get
            {
                return _IsEnabledInventoryEdit;
            }
            set
            {
                if (_IsEnabledInventoryEdit != value)
                {
                    _IsEnabledInventoryEdit = value;
                    OnPropertyChanged(nameof(IsEnabledInventoryEdit));
                    InventoryDisabledModes = IsEnabledInventoryEdit ? "" : "Disabled";
                }
            }
        }

        private bool _IsEnabledInventoryPosEdit;
        public bool IsEnabledInventoryPosEdit
        {
            get
            {
                return _IsEnabledInventoryPosEdit;
            }
            set
            {
                _IsEnabledInventoryPosEdit = value;
                OnPropertyChanged(nameof(IsEnabledInventoryPosEdit));
                InventoryPosDisabledModes = IsEnabledInventoryPosEdit ? "" : "Disabled";
            }
        }

        private string _InventoryDisabledModes = "Enabled";
        public string InventoryDisabledModes
        {
            get
            {
                return _InventoryDisabledModes;
            }
            set
            {
                _InventoryDisabledModes = value;
                OnPropertyChanged(nameof(InventoryDisabledModes));
            }
        }


        private string _InventoryPosDisabledModes = "Enabled";
        public string InventoryPosDisabledModes
        {
            get
            {
                return _InventoryPosDisabledModes;
            }
            set
            {
                if (_InventoryPosDisabledModes != value)
                {
                    _InventoryPosDisabledModes = value;
                    OnPropertyChanged(nameof(InventoryPosDisabledModes));
                }
            }
        }

        #endregion

        #region Properties -> Editor -> InputCode


        private string _InputCode;
        /// <summary>
        /// Doc  InputCode
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, nameof(InputCode), "en{'Barcode'}de{'Barcode'}", Description = "Barcode input field: a scanned quant barcode (FacilityChargeID) selects the matching inventory line.")]
        public string InputCode
        {
            get
            {
                return _InputCode;
            }
            set
            {
                if (_InputCode != value)
                {
                    _InputCode = value;
                    OnPropertyChanged(nameof(InputCode));
                    SearchPos();
                }
            }
        }


        private string _NewFaciltiyInventoryNo;
        /// <summary>
        /// Doc  InputCode
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, nameof(NewFaciltiyInventoryNo), "en{'Inventory Nr.'}de{'Inventurno.'}", Description = "Inventory number entered in the new-inventory dialog (suggested automatically by New()).")]
        public string NewFaciltiyInventoryNo
        {
            get
            {
                return _NewFaciltiyInventoryNo;
            }
            set
            {
                if (_NewFaciltiyInventoryNo != value)
                {
                    _NewFaciltiyInventoryNo = value;
                    OnPropertyChanged(nameof(NewFaciltiyInventoryNo));
                    OnPropertyChanged(nameof(SelectedFacilityInventory));
                }
            }
        }


        private string _NewFaciltiyInventoryName;
        /// <summary>
        /// Doc  InputCode
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, nameof(NewFaciltiyInventoryName), "en{'Name'}de{'Name'}", Description = "Name of the new inventory entered in the new-inventory dialog.")]
        public string NewFaciltiyInventoryName
        {
            get
            {
                return _NewFaciltiyInventoryName;
            }
            set
            {
                if (_NewFaciltiyInventoryName != value)
                {
                    _NewFaciltiyInventoryName = value;
                    OnPropertyChanged(nameof(NewFaciltiyInventoryName));
                    OnPropertyChanged(nameof(SelectedFacilityInventory));
                }
            }
        }

        #endregion

        #region Properties -> Editor -> PosStateMassChange
        public const string PosStateMassChange = "PosStateMassChange";
        private MDFacilityInventoryPosState _SelectedPosStateMassChange;
        /// <summary>
        /// Selected property for MDFacilityInventoryPosState
        /// </summary>
        /// <value>The selected PosStateMassChange</value>
        [ACPropertySelected(9999, nameof(SelectedPosStateMassChange), ConstApp.ESFacilityInventoryPosState, Description = "Gets or sets the target line state used by the mass status change for selected inventory lines.")]
        public MDFacilityInventoryPosState SelectedPosStateMassChange
        {
            get
            {
                return _SelectedPosStateMassChange;
            }
            set
            {
                if (_SelectedPosStateMassChange != value)
                {
                    _SelectedPosStateMassChange = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<MDFacilityInventoryPosState> _PosStateMassChangeList;
        /// <summary>
        /// List property for MDFacilityInventoryPosState
        /// </summary>
        /// <value>The FilterInventoryPosState list</value>
        [ACPropertyList(9999, nameof(PosStateMassChangeList), Description = "Gets the list of line states (below Posted) offered for the mass status change.")]
        public List<MDFacilityInventoryPosState> PosStateMassChangeList
        {
            get
            {
                if (_PosStateMassChangeList == null)
                {
                    List<MDFacilityInventoryPosState> list = LoadFilterInventoryPosStateList();
                    list = list.Where(c => c.MDFacilityInventoryPosStateIndex < (short)FacilityInventoryPosStateEnum.Posted).ToList();
                    _PosStateMassChangeList = list;
                }
                return _PosStateMassChangeList;
            }
        }

        #endregion

        #endregion

        #region Properties -> AccessNav

        public override IAccessNav AccessNav { get { return AccessPrimary; } }

        ACAccessNav<FacilityInventory> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, nameof(FacilityInventory), Description = "Gets the primary navigation access to the FacilityInventory records (search list of inventories).")]
        public ACAccessNav<FacilityInventory> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(NavigationqueryDefaultFilter);
                        navACQueryDefinition.CheckAndReplaceSortColumnsIfDifferent(NavigationqueryDefaultSort);
                        _AccessPrimary = navACQueryDefinition.NewAccessNav<FacilityInventory>(nameof(FacilityInventory), this);
                        _AccessPrimary.NavSearchExecuting += AccessPrimaryNavSearchExecuting;
                    }
                }
                return _AccessPrimary;
            }
        }

        private List<ACSortItem> NavigationqueryDefaultSort
        {
            get
            {
                List<ACSortItem> acSortItems = new List<ACSortItem>();

                ACSortItem facilityInventoryNo = new ACSortItem("FacilityInventoryNo", SortDirections.descending, true);
                acSortItems.Add(facilityInventoryNo);

                return acSortItems;
            }
        }

        public List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {

                List<ACFilterItem> aCFilterItems = new List<ACFilterItem>();

                // facilityInventoryNoFilter
                ACFilterItem facilityInventoryNoFilter = new ACFilterItem(Global.FilterTypes.filter, "FacilityInventoryNo", Global.LogicalOperators.equal, Global.Operators.and, null, true, true);
                aCFilterItems.Add(facilityInventoryNoFilter);

                // MDFacilityInventoryStateID
                ACFilterItem stateFilter = new ACFilterItem(Global.FilterTypes.filter, "MDFacilityInventoryState\\MDKey", Global.LogicalOperators.equal, Global.Operators.and, null, true);
                aCFilterItems.Add(stateFilter);

                // FilterInventoryStartDate
                ACFilterItem fromDate = new ACFilterItem(Global.FilterTypes.filter, "InsertDate", Global.LogicalOperators.greaterThanOrEqual, Global.Operators.and, null, true);
                aCFilterItems.Add(fromDate);

                // FilterInventoryEndDate
                ACFilterItem toDate = new ACFilterItem(Global.FilterTypes.filter, "InsertDate", Global.LogicalOperators.lessThan, Global.Operators.and, null, true);
                aCFilterItems.Add(toDate);

                return aCFilterItems;
            }
        }

        public virtual IQueryable<FacilityInventory> AccessPrimaryNavSearchExecuting(IQueryable<FacilityInventory> result)
        {
            IQueryable<FacilityInventory> query = result as IQueryable<FacilityInventory>;
            if (query != null)
            {
                query
                .Include(c => c.MDFacilityInventoryState)
                .Include(c => c.FacilityInventoryPos_FacilityInventory)
                .Include("FacilityInventoryPos_FacilityInventory.FacilityCharge")
                .Include("FacilityInventoryPos_FacilityInventory.FacilityCharge.Material")
                .Include("FacilityInventoryPos_FacilityInventory.FacilityCharge.FacilityLot");

            }

            Guid? mdFacilityInventoryStateID = null;
            if (SelectedFilterInventoryState != null)
                mdFacilityInventoryStateID = SelectedFilterInventoryState.MDFacilityInventoryStateID;

            result = result
                .Where(x =>
                (
                    mdFacilityInventoryStateID == null
                    || x.MDFacilityInventoryStateID == mdFacilityInventoryStateID
                )
                && x.InsertDate >= FilterInventoryStartDate
                && x.InsertDate < FilterInventoryEndDate
             );
            return query;
        }


        #endregion

        #region Property -> FacilityInventory

        [ACPropertyCurrent(9999, nameof(FacilityInventory), "en{'Inventory'}de{'Inventur'}", Description = "Gets or sets the current inventory (FacilityInventory) shown and edited in the detail form.")]
        public FacilityInventory CurrentFacilityInventory
        {
            get
            {
                return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary.Current != value)
                {

                    AccessPrimary.Current = value;
                    OnSelectedFacilityInventoryChanged();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Selected property for FacilityInventory
        /// </summary>
        /// <value>The selected FacilityInventory</value>
        [ACPropertySelected(9999, nameof(FacilityInventory), "en{'Inventory'}de{'Inventur'}", Description = "Gets or sets the currently selected inventory (FacilityInventory) in the search list.")]
        public FacilityInventory SelectedFacilityInventory
        {
            get
            {
                return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary.Selected != value)
                {

                    AccessPrimary.Selected = value;
                    OnPropertyChanged();
                }
            }
        }

        private void _SelectedFacilityInventory_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FacilityInventory item = sender as FacilityInventory;
            if (item != null)
                switch (e.PropertyName)
                {
                    case nameof(FacilityInventory.MDFacilityInventoryStateID):
                        IsEnabledInventoryEdit = item.MDFacilityInventoryState.MDFacilityInventoryStateIndex == (short)FacilityInventoryStateEnum.InProgress;
                        break;
                }
        }

        /// <summary>
        /// List property for FacilityInventory
        /// </summary>
        /// <value>The FacilityInventory list</value>
        [ACPropertyList(9999, nameof(FacilityInventory), Description = "Gets the list of inventories (FacilityInventory) resulting from the last search.")]
        public IEnumerable<FacilityInventory> FacilityInventoryList
        {
            get
            {
                if (AccessPrimary == null || AccessPrimary.NavList == null) return null;
                return AccessPrimary.NavList;
            }
        }

        #endregion

        #region Property -> FacilityInventoryPos

        private FacilityInventoryPos _SelectedFacilityInventoryPos;
        /// <summary>
        /// Selected property for FacilityInventoryPos
        /// </summary>
        /// <value>The selected FacilityInventoryPos</value>
        [ACPropertySelected(9999, nameof(FacilityInventoryPos), "en{'Inventory Line'}de{'Inventurposition'}", Description = "Gets or sets the currently selected inventory line (FacilityInventoryPos) whose counted quantity is edited.")]
        public FacilityInventoryPos SelectedFacilityInventoryPos
        {
            get
            {
                return _SelectedFacilityInventoryPos;
            }
            set
            {
                if (_SelectedFacilityInventoryPos != value)
                {
                    _SelectedFacilityInventoryPos = value;
                    if (_SelectedFacilityInventoryPos != null)
                    {
                        _SelectedFacilityInventoryPos.PropertyChanged -= _SelectedFacilityInventoryPos_PropertyChanged;
                        _SelectedFacilityInventoryPos.PropertyChanged += _SelectedFacilityInventoryPos_PropertyChanged;
                    }
                    IsEnabledInventoryPosEdit = IsInventoryPosEnabledEdit();
                    OnPropertyChanged(nameof(SelectedFacilityInventoryPos));

                    if (SelectedFacilityInventoryPos != null)
                    {
                        SelectedFacilityInventoryPos.PropertyChanged -= SelectedFacilityInventoryPos_PropertyChanged;
                        SelectedFacilityInventoryPos.PropertyChanged += SelectedFacilityInventoryPos_PropertyChanged;
                    }
                    SetInventoryPosFacilityBookingList();
                }
            }
        }

        private void _SelectedFacilityInventoryPos_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FacilityInventoryPos item = sender as FacilityInventoryPos;
            if (e.PropertyName == nameof(item.NotAvailable))
            {
                if (item.NotAvailable)
                    item.NewStockQuantity = null;
                if (item.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex == (short)FacilityInventoryPosStateEnum.New)
                {
                    MDFacilityInventoryPosState stateInProgress =
                        item.GetObjectContext<DatabaseApp>()
                        .MDFacilityInventoryPosState
                        .FirstOrDefault(c => c.MDFacilityInventoryPosStateIndex == (short)FacilityInventoryPosStateEnum.Finished);
                    item.MDFacilityInventoryPosState = stateInProgress;
                }
                OnPropertyChanged(nameof(SelectedFacilityInventoryPos));
            }
            else if (e.PropertyName == nameof(item.NewStockQuantity))
            {
                if (item.NewStockQuantity.HasValue)
                    item.NotAvailable = false;
                if (item.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex == (short)FacilityInventoryPosStateEnum.New)
                {
                    MDFacilityInventoryPosState stateInProgress =
                      item.GetObjectContext<DatabaseApp>()
                      .MDFacilityInventoryPosState
                      .FirstOrDefault(c => c.MDFacilityInventoryPosStateIndex == (short)FacilityInventoryPosStateEnum.Finished);
                    item.MDFacilityInventoryPosState = stateInProgress;
                }
                OnPropertyChanged(nameof(SelectedFacilityInventoryPos));
            }
        }

        private bool IsInventoryPosEnabledEdit()
        {
            return
                IsEnabledInventoryEdit
                && SelectedFacilityInventoryPos != null
                && SelectedFacilityInventoryPos.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex < (short)FacilityInventoryPosStateEnum.Finished;
        }

        private void SelectedFacilityInventoryPos_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FacilityInventoryPos.MDFacilityInventoryPosStateID))
            {
                IsEnabledInventoryPosEdit = IsInventoryPosEnabledEdit();
                OnPropertyChanged(nameof(SelectedFacilityInventoryPos));
            }
        }

        private List<FacilityInventoryPos> _FacilityInventoryPosList;
        /// <summary>
        /// List property for FacilityInventoryPos
        /// </summary>
        /// <value>The FacilityInventoryPos list</value>
        [ACPropertyList(9999, nameof(FacilityInventoryPos), Description = "Gets the filtered list of inventory lines (FacilityInventoryPos) of the selected inventory.")]
        public List<FacilityInventoryPos> FacilityInventoryPosList
        {
            get
            {
                return _FacilityInventoryPosList;
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _FilteredInventoryLinesCount;
        [ACPropertyInfo(999, nameof(FilteredInventoryLinesCount), "en{'filtered'}de{'gefiltert'}", Description = "Number of inventory lines matching the current line filter.")]
        public double FilteredInventoryLinesCount
        {
            get
            {
                return _FilteredInventoryLinesCount;
            }
            set
            {
                if (_FilteredInventoryLinesCount != value)
                {
                    _FilteredInventoryLinesCount = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _InventoryLinesCount;
        [ACPropertyInfo(999, nameof(InventoryLinesCount), "en{'from'}de{'von'}", Description = "Total number of inventory lines of the selected inventory.")]
        public double InventoryLinesCount
        {
            get
            {
                return _InventoryLinesCount;
            }
            set
            {
                if (_InventoryLinesCount != value)
                {
                    _InventoryLinesCount = value;
                    OnPropertyChanged();
                }
            }
        }


        private (List<FacilityInventoryPos> inventoryPosList, double filteredInventoryLinesCount, double inventoryLinesCount) GetFacilityInventoryLines()
        {
            double filteredInventoryLinesCount = 0;
            double inventoryLinesCount = 0;
            List<FacilityInventoryPos> inventoryPosList = null;

            if (SelectedFacilityInventory != null)
            {
                SelectedFacilityInventory.FacilityInventoryPos_FacilityInventory.AutoLoad(SelectedFacilityInventory.FacilityInventoryPos_FacilityInventoryReference, SelectedFacilityInventory);
                inventoryLinesCount = SelectedFacilityInventory.FacilityInventoryPos_FacilityInventory.Count();
                var query = SelectedFacilityInventory
                    .FacilityInventoryPos_FacilityInventory
                    .Where(c =>
                        ((InputCode ?? "") == "" || c.FacilityCharge.FacilityChargeID == new Guid(InputCode))
                        && (
                                string.IsNullOrEmpty(FilterFacility)
                                ||
                                c.FacilityCharge.Facility.FacilityNo.ToLower().Contains(FilterFacility.ToLower())
                                || c.FacilityCharge.Facility.FacilityName.ToLower().Contains(FilterFacility.ToLower())
                        )
                        && (
                                string.IsNullOrEmpty(FilterMaterial)
                                || c.FacilityCharge.Material.MaterialNo.ToLower().Contains(FilterMaterial.ToLower())
                                || c.FacilityCharge.Material.MaterialName1.ToLower().Contains(FilterMaterial.ToLower())
                        )
                        && (
                                (FilterInventoryPosLotNo ?? "") == ""
                                || (
                                        c.FacilityCharge.FacilityLot != null
                                        && c.FacilityCharge.FacilityLot.LotNo == FilterInventoryPosLotNo
                                   )
                            )
                        && (SelectedFilterInventoryPosState == null || c.MDFacilityInventoryPosState.MDKey == SelectedFilterInventoryPosState.MDKey)
                        && (
                                FilterInventoryPosNotAvailable == null
                                // both true or both false - ! exclusive OR ^
                                || !(c.NotAvailable ^ (FilterInventoryPosNotAvailable ?? false))
                           )
                        //&& (
                        //        FilterInventoryPosZeroQuantity == null
                        //        || !((c.StockQuantity == 0) ^ (FilterInventoryPosZeroQuantity ?? false))
                        //   )
                        && (
                                FilterNewStockQuantityDifferent == null
                                || !((c.NewStockQuantity != null && Math.Abs(c.NewStockQuantity ?? -c.StockQuantity) > 0.1) ^ (FilterNewStockQuantityDifferent ?? false))
                            )
                         && (
                                FilterOpenLines == null
                                || (
                                        ((c.NewStockQuantity != null || c.NotAvailable || c.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex == (short)FacilityInventoryPosStateEnum.Finished) && !(FilterOpenLines ?? false))
                                        || ((c.NewStockQuantity == null && !c.NotAvailable && c.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex < (short)FacilityInventoryPosStateEnum.Finished) && (FilterOpenLines ?? false))
                                   )
                           )
                     );

                filteredInventoryLinesCount = query.Count();
                inventoryPosList =
                    query
                    .OrderBy(c => c.FacilityCharge != null && c.FacilityCharge.FacilityLot != null ? c.FacilityCharge.FacilityLot.LotNo : "")
                    .Take(FilterPosPageSize)
                    .ToList();
            }

            return (inventoryPosList, filteredInventoryLinesCount, inventoryLinesCount);
        }

        public void SetFacilityInventoryPosList()
        {
            (List<FacilityInventoryPos> inventoryPosList, double filteredInventoryLinesCount, double inventoryLinesCount) = GetFacilityInventoryLines();
            _FacilityInventoryPosList = inventoryPosList;
            FilteredInventoryLinesCount = filteredInventoryLinesCount;
            InventoryLinesCount = inventoryLinesCount;
            OnPropertyChanged(nameof(FacilityInventoryPosList));
            if (_FacilityInventoryPosList != null)
            {
                SelectedFacilityInventoryPos = _FacilityInventoryPosList.FirstOrDefault();
            }
            else
            {
                SelectedFacilityInventoryPos = null;
            }
        }

        #endregion

        #region Property -> FilterNotUsedCharge

        private string _FilterNotUsedChargeMaterial;
        /// <summary>
        /// Doc  FilterNotUsedChargeMaterial
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, nameof(FilterNotUsedChargeMaterial), "en{'Material'}de{'Material'}", Description = "Filter for the unused quants list: material number or material name.")]
        public string FilterNotUsedChargeMaterial
        {
            get
            {
                return _FilterNotUsedChargeMaterial;
            }
            set
            {
                if (_FilterNotUsedChargeMaterial != value)
                {
                    _FilterNotUsedChargeMaterial = value;
                    OnPropertyChanged(nameof(FilterNotUsedChargeMaterial));
                }
            }
        }

        private string _FilterNotUsedChargeFacility;
        /// <summary>
        /// Doc  FilterNotUsedChargeFacility
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, nameof(FilterNotUsedChargeFacility), "en{'Facility'}de{'Lagerplatz'}", Description = "Filter for the unused quants list: storage bin number or name.")]
        public string FilterNotUsedChargeFacility
        {
            get
            {
                return _FilterNotUsedChargeFacility;
            }
            set
            {
                if (_FilterNotUsedChargeFacility != value)
                {
                    _FilterNotUsedChargeFacility = value;
                    OnPropertyChanged(nameof(FilterNotUsedChargeFacility));
                }
            }
        }

        private string _FilterNotUsedChargeLotNo;
        /// <summary>
        /// Doc  FilterNotUsedChargeLotNo
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, nameof(FilterNotUsedChargeLotNo), "en{'Lot'}de{'Los'}", Description = "Filter for the unused quants list: lot number.")]
        public string FilterNotUsedChargeLotNo
        {
            get
            {
                return _FilterNotUsedChargeLotNo;
            }
            set
            {
                if (_FilterNotUsedChargeLotNo != value)
                {
                    _FilterNotUsedChargeLotNo = value;
                    OnPropertyChanged(nameof(FilterNotUsedChargeLotNo));
                }
            }
        }

        private int _FilterNotUsedChargePageSize = 500;
        [ACPropertyInfo(120, nameof(FilterPosPageSize), "en{'Limit Record Count'}de{'Limit Anzahl Datensätze'}", Description = "Maximum number of unused quants loaded per search (default 500). Note: the bound property name is FilterPosPageSize (see FilterNotUsedChargePageSize).")]
        public int FilterNotUsedChargePageSize
        {
            get
            {
                return _FilterNotUsedChargePageSize;
            }
            set
            {
                if (_FilterNotUsedChargePageSize != value)
                {
                    _FilterNotUsedChargePageSize = value;
                    OnPropertyChanged(nameof(FilterNotUsedChargePageSize));
                }
            }
        }

        #endregion

        #region Property -> NotUsedFacilityCharge
        public const string NotUsedFacilityCharge = "NotUsedFacilityCharge";
        private FacilityCharge _SelectedNotUsedFacilityCharge;
        /// <summary>
        /// Selected property for FacilityCharge
        /// </summary>
        /// <value>The selected NotUsedFacilityCharge</value>
        [ACPropertySelected(9999, nameof(NotUsedFacilityCharge), "en{'Unused Quant'}de{'Nicht verwendetes Quant'}", Description = "Gets or sets the currently selected unused quant (FacilityCharge) that can be added to the inventory.")]
        public FacilityCharge SelectedNotUsedFacilityCharge
        {
            get
            {
                return _SelectedNotUsedFacilityCharge;
            }
            set
            {
                if (_SelectedNotUsedFacilityCharge != value)
                {
                    _SelectedNotUsedFacilityCharge = value;
                    OnPropertyChanged(nameof(SelectedNotUsedFacilityCharge));
                }
            }
        }


        private List<FacilityCharge> _NotUsedFacilityChargeList;
        /// <summary>
        /// List property for FacilityCharge
        /// </summary>
        /// <value>The NotUsedFacilityCharge list</value>
        [ACPropertyList(9999, nameof(NotUsedFacilityCharge), Description = "Gets the list of quants (FacilityCharge) not yet included in the selected inventory.")]
        public List<FacilityCharge> NotUsedFacilityChargeList
        {
            get
            {
                return _NotUsedFacilityChargeList;
            }
        }


        #endregion

        #region Property -> FilterInventoryFaciltiyBookingCharge


        private string _FilterInventoryFaciltiyBookingChargeMaterial;
        /// <summary>
        /// Doc  FilterNotUsedChargeMaterial
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, nameof(FilterInventoryFaciltiyBookingChargeMaterial), "en{'Material'}de{'Material'}", Description = "Filter for the inventory bookings list: material number or material name of the inward/outward material.")]
        public string FilterInventoryFaciltiyBookingChargeMaterial
        {
            get
            {
                return _FilterInventoryFaciltiyBookingChargeMaterial;
            }
            set
            {
                if (_FilterInventoryFaciltiyBookingChargeMaterial != value)
                {
                    _FilterInventoryFaciltiyBookingChargeMaterial = value;
                    OnPropertyChanged(nameof(FilterInventoryFaciltiyBookingChargeMaterial));
                }
            }
        }

        private string _FilterInventoryFaciltiyBookingChargeFacility;
        /// <summary>
        /// Doc  FilterNotUsedChargeFacility
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, nameof(FilterInventoryFaciltiyBookingChargeFacility), "en{'Facility'}de{'Lagerplatz'}", Description = "Filter for the inventory bookings list: storage bin number or name of the inward/outward storage bin.")]
        public string FilterInventoryFaciltiyBookingChargeFacility
        {
            get
            {
                return _FilterInventoryFaciltiyBookingChargeFacility;
            }
            set
            {
                if (_FilterInventoryFaciltiyBookingChargeFacility != value)
                {
                    _FilterInventoryFaciltiyBookingChargeFacility = value;
                    OnPropertyChanged(nameof(FilterInventoryFaciltiyBookingChargeFacility));
                }
            }
        }

        private string _FilterInventoryFaciltiyBookingChargeLotNo;
        /// <summary>
        /// Doc  FilterNotUsedChargeLotNo
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, nameof(FilterInventoryFaciltiyBookingChargeLotNo), "en{'Lot'}de{'Los'}", Description = "Filter for the inventory bookings list: lot number of the inward/outward charge.")]
        public string FilterInventoryFaciltiyBookingChargeLotNo
        {
            get
            {
                return _FilterInventoryFaciltiyBookingChargeLotNo;
            }
            set
            {
                if (_FilterInventoryFaciltiyBookingChargeLotNo != value)
                {
                    _FilterInventoryFaciltiyBookingChargeLotNo = value;
                    OnPropertyChanged(nameof(FilterInventoryFaciltiyBookingChargeLotNo));
                }
            }
        }

        private int _FilterInventoryFaciltiyBookingChargePageSize = 500;
        [ACPropertyInfo(120, nameof(FilterInventoryFaciltiyBookingChargePageSize), "en{'Limit Record Count'}de{'Limit Anzahl Datensätze'}", Description = "Maximum number of inventory bookings loaded per search (default 500).")]
        public int FilterInventoryFaciltiyBookingChargePageSize
        {
            get
            {
                return _FilterInventoryFaciltiyBookingChargePageSize;
            }
            set
            {
                if (_FilterInventoryFaciltiyBookingChargePageSize != value)
                {
                    _FilterInventoryFaciltiyBookingChargePageSize = value;
                    OnPropertyChanged(nameof(FilterInventoryFaciltiyBookingChargePageSize));
                }
            }
        }

        #endregion

        #region Property ->  InventoryFacilityBookingCharge
        public const string InventoryFacilityBookingCharge = "InventoryFacilityBookingCharge";
        private FacilityBookingCharge _SelectedInventoryFacilityBookingCharge;
        /// <summary>
        /// Selected property for FacilityBookingCharge
        /// </summary>
        /// <value>The selected FacilityBookingCharge</value>
        [ACPropertySelected(400, nameof(InventoryFacilityBookingCharge), "en{'Booking'}de{'Buchung'}", Description = "Gets or sets the currently selected booking charge (FacilityBookingCharge) of the whole inventory.")]
        public FacilityBookingCharge SelectedInventoryFacilityBookingCharge
        {
            get
            {
                return _SelectedInventoryFacilityBookingCharge;
            }
            set
            {
                if (_SelectedInventoryFacilityBookingCharge != value)
                {
                    _SelectedInventoryFacilityBookingCharge = value;
                    OnPropertyChanged(nameof(SelectedInventoryFacilityBookingCharge));
                }
            }
        }

        private List<FacilityBookingCharge> _InventoryFacilityBookingChargeList;
        /// <summary>
        /// List property for FacilityBookingCharge
        /// </summary>
        /// <value>The FacilityBookingCharge list</value>
        [ACPropertyList(401, nameof(InventoryFacilityBookingCharge), Description = "Gets the list of booking charges (FacilityBookingCharge) created by the selected inventory.")]
        public List<FacilityBookingCharge> InventoryFacilityBookingChargeList
        {
            get
            {
                return _InventoryFacilityBookingChargeList;
            }
        }

        #endregion

        #region Property ->  InventoryPosFacilityBooking
        public const string InventoryPosFacilityBooking = "InventoryPosFacilityBooking";
        private FacilityBooking _SelectedInventoryPosFacilityBooking;
        /// <summary>
        /// Selected property for FacilityBooking
        /// </summary>
        /// <value>The selected FacilityBooking</value>
        [ACPropertySelected(300, nameof(InventoryPosFacilityBooking), "en{'Booking'}de{'Buchung'}", Description = "Gets or sets the currently selected booking (FacilityBooking) of the selected inventory line.")]
        public FacilityBooking SelectedInventoryPosFacilityBooking
        {
            get
            {
                return _SelectedInventoryPosFacilityBooking;
            }
            set
            {
                if (_SelectedInventoryPosFacilityBooking != value)
                {
                    _SelectedInventoryPosFacilityBooking = value;
                    OnPropertyChanged(nameof(SelectedInventoryPosFacilityBooking));

                    _InventoryPosFacilityBookingChargeList = LoadInventoryPosFacilityBookingChargeList();
                    OnPropertyChanged(nameof(InventoryPosFacilityBookingChargeList));
                    if (_InventoryPosFacilityBookingChargeList != null)
                        SelectedInventoryPosFacilityBookingCharge = _InventoryPosFacilityBookingChargeList.FirstOrDefault();
                    else
                        SelectedInventoryPosFacilityBookingCharge = null;
                }
            }
        }

        private List<FacilityBooking> _InventoryPosFacilityBookingList;
        /// <summary>
        /// List property for FacilityBooking
        /// </summary>
        /// <value>The FacilityBooking list</value>
        [ACPropertyList(301, nameof(InventoryPosFacilityBooking), Description = "Gets the list of bookings (FacilityBooking) of the selected inventory line.")]
        public List<FacilityBooking> InventoryPosFacilityBookingList
        {
            get
            {
                return _InventoryPosFacilityBookingList;
            }
        }

        private List<FacilityBooking> GetInventoryPosFacilityBookingList()
        {
            List<FacilityBooking> bookings = null;
            if (SelectedFacilityInventoryPos != null)
            {
                SelectedFacilityInventoryPos.FacilityBooking_FacilityInventoryPos.AutoLoad(SelectedFacilityInventoryPos.FacilityBooking_FacilityInventoryPosReference, SelectedFacilityInventoryPos);
                bookings = SelectedFacilityInventoryPos.FacilityBooking_FacilityInventoryPos.OrderBy(c => c.FacilityBookingNo).ToList();
            }
            return bookings;
        }

        public void SetInventoryPosFacilityBookingList()
        {
            _InventoryPosFacilityBookingList = GetInventoryPosFacilityBookingList();
            OnPropertyChanged(nameof(InventoryPosFacilityBookingList));
            if (_InventoryPosFacilityBookingList != null)
                SelectedInventoryPosFacilityBooking = _InventoryPosFacilityBookingList.FirstOrDefault();
            else
                SelectedInventoryPosFacilityBooking = null;
        }
        #endregion

        #region Property ->  InventoryPosFacilityBookingCharge
        public const string InventoryPosFacilityBookingCharge = "InventoryPosFacilityBookingCharge";
        private FacilityBookingCharge _SelectedInventoryPosFacilityBookingCharge;
        /// <summary>
        /// Selected property for FacilityBookingCharge
        /// </summary>
        /// <value>The selected FacilityBookingCharge</value>
        [ACPropertySelected(400, nameof(InventoryPosFacilityBookingCharge), "en{'Booking'}de{'Buchung'}", Description = "Gets or sets the currently selected booking charge (FacilityBookingCharge) of the selected booking of the inventory line.")]
        public FacilityBookingCharge SelectedInventoryPosFacilityBookingCharge
        {
            get
            {
                return _SelectedInventoryPosFacilityBookingCharge;
            }
            set
            {
                if (_SelectedInventoryPosFacilityBookingCharge != value)
                {
                    _SelectedInventoryPosFacilityBookingCharge = value;
                    OnPropertyChanged(nameof(SelectedInventoryPosFacilityBookingCharge));
                }
            }
        }

        private List<FacilityBookingCharge> _InventoryPosFacilityBookingChargeList;
        /// <summary>
        /// List property for FacilityBookingCharge
        /// </summary>
        /// <value>The FacilityBookingCharge list</value>
        [ACPropertyList(401, nameof(InventoryPosFacilityBookingCharge), Description = "Gets the list of booking charges (FacilityBookingCharge) of the selected booking of the inventory line.")]
        public List<FacilityBookingCharge> InventoryPosFacilityBookingChargeList
        {
            get
            {
                if (_InventoryPosFacilityBookingChargeList == null)
                    _InventoryPosFacilityBookingChargeList = LoadInventoryPosFacilityBookingChargeList();
                return _InventoryPosFacilityBookingChargeList;
            }
        }

        private List<FacilityBookingCharge> LoadInventoryPosFacilityBookingChargeList()
        {
            if (SelectedInventoryPosFacilityBooking == null) return null;
            return SelectedInventoryPosFacilityBooking.FacilityBookingCharge_FacilityBooking.OrderBy(c => c.FacilityBookingChargeNo).ToList();
        }

        #endregion

        #endregion

        #region Methods

        #region Methods -> InventoryFacilityBookingCharge

        private List<FacilityBookingCharge> LoadInventoryFacilityBookingChargeList()
        {
            if (SelectedFacilityInventory == null) return null;
            return SelectedFacilityInventory
                .FacilityInventoryPos_FacilityInventory
                .SelectMany(c => c.FacilityBookingCharge_FacilityInventoryPos)
                .Where(c =>
                    // FilterInventoryFaciltiyBookingChargeMaterial
                    (FilterInventoryFaciltiyBookingChargeMaterial == null
                        || (
                            c.InwardMaterial != null
                            && (
                                    c.InwardMaterial.MaterialNo.ToLower().Contains(FilterInventoryFaciltiyBookingChargeMaterial.ToLower())
                                    || c.InwardMaterial.MaterialName1.ToLower().Contains(FilterInventoryFaciltiyBookingChargeMaterial.ToLower())
                                )
                           )
                        || (
                            c.OutwardMaterial != null
                            && (
                                    c.OutwardMaterial.MaterialNo.ToLower().Contains(FilterInventoryFaciltiyBookingChargeMaterial.ToLower())
                                    || c.OutwardMaterial.MaterialName1.ToLower().Contains(FilterInventoryFaciltiyBookingChargeMaterial.ToLower())
                                )
                            )
                       )
                       // FilterInventoryFaciltiyBookingChargeFacility
                       && (FilterInventoryFaciltiyBookingChargeFacility == null
                            || (
                                c.InwardFacility != null
                                && (
                                        c.InwardFacility.FacilityNo.ToLower().Contains(FilterInventoryFaciltiyBookingChargeFacility.ToLower())
                                        || c.InwardFacility.FacilityName.ToLower().Contains(FilterInventoryFaciltiyBookingChargeFacility.ToLower())
                                   )
                               )
                            || (
                                c.OutwardFacility != null
                                && (
                                        c.OutwardFacility.FacilityNo.ToLower().Contains(FilterInventoryFaciltiyBookingChargeFacility.ToLower())
                                        || c.OutwardFacility.FacilityName.ToLower().Contains(FilterInventoryFaciltiyBookingChargeFacility.ToLower())
                                   )
                               )
                           )
                        // FilterInventoryFaciltiyBookingChargeLotNo
                        && (FilterInventoryFaciltiyBookingChargeLotNo == null
                        || (
                                c.InwardFacilityCharge != null && c.InwardFacilityCharge.FacilityLot != null
                                && c.InwardFacilityCharge.FacilityLot.LotNo.Contains(FilterInventoryFaciltiyBookingChargeLotNo)
                            )
                        || (
                                c.OutwardFacilityCharge != null && c.OutwardFacilityCharge.FacilityLot != null
                                && c.OutwardFacilityCharge.FacilityLot.LotNo.Contains(FilterInventoryFaciltiyBookingChargeLotNo)
                            )
                        )
                 )
                .OrderBy(c => c.FacilityBookingChargeNo)
                .Take(FilterInventoryFaciltiyBookingChargePageSize)
                .ToList();
        }

        [ACMethodInfo(nameof(SearchFacilityInventoryFacilityBookingCharge), ConstApp.Search, 100, Description = "Searches the booking charges created by the selected inventory, filtered by material, storage bin and lot.")]
        public void SearchFacilityInventoryFacilityBookingCharge()
        {
            _InventoryFacilityBookingChargeList = LoadInventoryFacilityBookingChargeList();
            OnPropertyChanged(nameof(InventoryFacilityBookingChargeList));
            if (_InventoryFacilityBookingChargeList != null)
                SelectedInventoryFacilityBookingCharge = _InventoryFacilityBookingChargeList.FirstOrDefault();
            else
                SelectedInventoryFacilityBookingCharge = null;
        }

        private void ClearInventoryFacilityBookingCharge()
        {
            _InventoryFacilityBookingChargeList = null;
            OnPropertyChanged(nameof(InventoryFacilityBookingChargeList));
        }

        private void ClearFilterInventoryFacilityBookingCharge()
        {
            FilterInventoryFaciltiyBookingChargeMaterial = null;
            FilterInventoryFaciltiyBookingChargeFacility = null;
            FilterInventoryFaciltiyBookingChargeLotNo = null;
            FilterInventoryFaciltiyBookingChargePageSize = 500;
        }

        #endregion

        #region Methods -> NotUsedFacilityCharge

        private List<FacilityCharge> LoadNotUsedFacilityChargeList()
        {
            Guid[] usedFacilityChargeIDs =
                SelectedFacilityInventory
                .FacilityInventoryPos_FacilityInventory
                .Select(c => c.FacilityChargeID)
                .ToArray();

            return
                DatabaseApp
                .FacilityCharge
                .Include(c => c.Material)
                .Include(c => c.FacilityLot)
                .Include(c => c.Facility)
                .Where(c =>
                    !usedFacilityChargeIDs.Contains(c.FacilityChargeID)
                    && c.NotAvailable
                    &&
                        (
                            FilterNotUsedChargeMaterial == null
                            ||
                            (
                                c.Material.MaterialNo.ToLower().Contains(FilterNotUsedChargeMaterial.ToLower())
                                || c.Material.MaterialName1.ToLower().Contains(FilterNotUsedChargeMaterial.ToLower())
                            )
                         )
                    &&
                        (
                            FilterNotUsedChargeFacility == null
                            ||
                            (
                                c.Facility.FacilityNo.ToLower().Contains(FilterNotUsedChargeFacility.ToLower())
                                || c.Facility.FacilityName.ToLower().Contains(FilterNotUsedChargeFacility.ToLower())
                            )
                         )
                    &&
                        (
                            FilterNotUsedChargeLotNo == null
                            || c.FacilityLot.LotNo.Contains(FilterNotUsedChargeLotNo)
                         )

                ).OrderBy(c => c.FacilityLot != null ? c.FacilityLot.LotNo : "")
                .Take(FilterNotUsedChargePageSize)
                .ToList();
        }

        [ACMethodInfo(nameof(SearchNotUsedFaciltiyCharge), ConstApp.Search, 110, Description = "Searches quants (FacilityCharge) that are not yet included in the selected inventory, filtered by material, storage bin and lot.")]
        public void SearchNotUsedFaciltiyCharge()
        {
            _NotUsedFacilityChargeList = LoadNotUsedFacilityChargeList();
            OnPropertyChanged(nameof(NotUsedFacilityChargeList));
            if (_FacilityInventoryPosList != null)
                SelectedNotUsedFacilityCharge = _NotUsedFacilityChargeList.FirstOrDefault();
            else
                SelectedNotUsedFacilityCharge = null;
        }

        private void ClearNotUserdFaciltiyChargeList()
        {
            _NotUsedFacilityChargeList = null;
            OnPropertyChanged(nameof(NotUsedFacilityChargeList));
        }

        private void ClearFilterNotUsedCharge()
        {
            FilterNotUsedChargeMaterial = null;
            FilterNotUsedChargeFacility = null;
            FilterNotUsedChargeLotNo = null;
            FilterNotUsedChargePageSize = 500;
        }

        #endregion


        #region Methods -> ACvbBSO -> Search

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(nameof(FacilityInventory), "en{'Search'}de{'Suche'}", (short)MISort.Search, Description = "Searches inventories within the filter date range (FilterInventoryStartDate/FilterInventoryEndDate) and state filter and fills FacilityInventoryList.")]
        public void Search()
        {
            AccessNav.NavSearch();
            OnPropertyChanged(nameof(FacilityInventoryList));
        }

        [ACMethodCommand(nameof(FacilityInventory), ConstApp.Reset, (short)MISort.Search, Description = "Resets the search filters (line state, date range) and searches again.")]
        public void ClearSearch()
        {
            SelectedFilterInventoryPosState = null;
            LoadInitialFilterInventoryDates();
            Search();
        }

        [ACMethodInteraction(nameof(FacilityInventory), ConstApp.Load, (short)MISort.Load, false, nameof(SelectedFacilityInventory), Global.ACKinds.MSMethodPrePost, Description = "Re-reads the selected inventory from the database (e.g. after another user changed it) and refreshes the inventory lines.")]
        public void Load(bool requery = false)
        {
            var query =
                DatabaseApp
                .FacilityInventory
                .Include(c => c.MDFacilityInventoryState)
                .Include(c => c.FacilityInventoryPos_FacilityInventory)
                .Include("FacilityInventoryPos_FacilityInventory.FacilityCharge")
                .Include("FacilityInventoryPos_FacilityInventory.FacilityCharge.Material")
                .Include("FacilityInventoryPos_FacilityInventory.FacilityCharge.FacilityLot");

            LoadEntity<FacilityInventory>(requery, () => SelectedFacilityInventory, () => CurrentFacilityInventory, c => CurrentFacilityInventory = c, query);

            if (requery)
            {
                SelectedFacilityInventory.AutoRefresh();
                OnPropertyChanged(nameof(SelectedFacilityInventory));
                SetFacilityInventoryPosList();
            }
        }

        public bool IsEnabledLoad()
        {
            return SelectedFacilityInventory != null;
        }

        [ACMethodInfo(nameof(FacilityInventoryPos), ConstApp.Search, 120, Description = "Reapplies the line filters (material, storage bin, lot, state, barcode, ...) and reloads FacilityInventoryPosList.")]
        public void SearchPos()
        {
            if (!IsEnabledSearchPos())
                return;
            SetFacilityInventoryPosList();
        }

        public bool IsEnabledSearchPos()
        {
            return SelectedFacilityInventory != null;
        }

        [ACMethodInfo(nameof(FacilityInventoryPos), "en{'Clear filter'}de{'Filter löschen'}", 130, Description = "Clears all inventory line filters and reloads the lines.")]
        public void ClearSearchPos()
        {
            if (!IsEnabledClearSearchPos())
                return;

            _InputCode = "";
            OnPropertyChanged(nameof(InputCode));

            _FilterInventoryPosLotNo = null;
            OnPropertyChanged(nameof(FilterInventoryPosLotNo));

            _SelectedFilterInventoryPosState = null;
            OnPropertyChanged(nameof(SelectedFilterInventoryPosState));

            _FilterInventoryPosNotAvailable = null;
            OnPropertyChanged(nameof(FilterInventoryPosNotAvailable));

            _FilterInventoryPosZeroQuantity = null;
            OnPropertyChanged(nameof(FilterInventoryPosZeroQuantity));

            _FilterOpenLines = null;
            OnPropertyChanged(nameof(FilterOpenLines));

            _FilterFacility = null;
            OnPropertyChanged(nameof(FilterFacility));

            _FilterMaterial = null;
            OnPropertyChanged(nameof(FilterMaterial));

            SearchPos();
        }

        public bool IsEnabledClearSearchPos()
        {
            return SelectedFacilityInventory != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(nameof(FacilityInventory), Const.New, (short)MISort.New, true, nameof(SelectedFacilityInventory), Global.ACKinds.MSMethodPrePost, Description = "Selects the inventory line matching the scanned barcode in InputCode (FacilityChargeID). Note: the attribute uses the 'New' caption for historical reasons.")]
        public void SelectPos()
        {
            if (!IsEnabledSelectPos())
                return;

            Guid codeID = Guid.Empty;
            if (Guid.TryParse(InputCode, out codeID))
            {
                SelectedFacilityInventoryPos = FacilityInventoryPosList.FirstOrDefault(c => c.FacilityChargeID == codeID);
            }
        }

        public bool IsEnabledSelectPos()
        {
            return SelectedFacilityInventory != null && !string.IsNullOrEmpty(InputCode);
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(nameof(FacilityInventory), Const.New, (short)MISort.New, true, nameof(SelectedFacilityInventory), Global.ACKinds.MSMethodPrePost, Description = "Opens the new-inventory dialog: suggests an inventory number (NewFaciltiyInventoryNo), resets the name and shows the generation options.")]
        public async Task New()
        {
            NewFaciltiyInventoryName = "";
            NewFaciltiyInventoryNo = ACFacilityManager.GetNewInventoryNo();
            _GenerateInventoryPosition = true;
            _OmitGenerateSiloQuantPosition = true;
            await ShowDialogAsync(this, "NewFacilityInventoryDlg");
        }

        [ACMethodInfo("NewDlgOk", "en{'New'}de{'Neu'}", 140, Description = "Creates the new inventory in the background (FacilityManager.InventoryGenerate) with the entered number, name, storage bin and generation options.")]
        public async Task NewDlgOk()
        {
            if (!IsEnabledNewDlgOk())
                return;
            if (SelectedNewInventoryFacility != null || await Messages.QuestionAsync(this, "Question50085") == MsgResult.Yes)
            {
                CloseTopDialog();
                BackgroundWorker.RunWorkerAsync(nameof(DoNewInventory));
                await ShowDialogAsync(this, DesignNameProgressBar);
            }
        }
        public bool IsEnabledNewDlgOk()
        {
            return !string.IsNullOrEmpty(NewFaciltiyInventoryNo) && !string.IsNullOrEmpty(NewFaciltiyInventoryName);
        }

        [ACMethodInfo(nameof(NewDlgOkCancel), "en{'Close'}de{'Schließen'}", 150, Description = "Closes the new-inventory dialog without creating an inventory.")]
        public void NewDlgOkCancel()
        {
            CloseTopDialog();
        }

        [ACMethodInfo(nameof(ShowFaciltiyDialog), "en{'Choose facility'}de{'Lager auswählen'}", 160, Description = "Opens the storage bin explorer to choose the storage bin for the new inventory (result in SelectedNewInventoryFacility).")]
        public async Task ShowFaciltiyDialog()
        {
            if (!IsEnabledShowFaciltiyDialog())
                return;

            VBDialogResult dlgResult = await BSOFacilityExplorer_Child.Value.ShowDialog(SelectedNewInventoryFacility);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;
                SelectedNewInventoryFacility = facility;
            }
        }

        public bool IsEnabledShowFaciltiyDialog()
        {
            return true;
        }

        [ACMethodInfo(nameof(ChangeInventoryFacility), "en{'Change facility'}de{'Lager wechseln'}", 170, Description = "Changes the storage bin of the selected inventory (only possible as long as the inventory has no lines).")]
        public async Task ChangeInventoryFacility()
        {
            if (!IsEnabledChangeInventoryFacility())
                return;

            VBDialogResult dlgResult = await BSOFacilityExplorer_Child.Value.ShowDialog(SelectedFacilityInventory.Facility);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;
                SelectedFacilityInventory.Facility = facility;
                OnPropertyChanged(nameof(SelectedFacilityInventory));
            }
        }

        public bool IsEnabledChangeInventoryFacility()
        {
            return SelectedFacilityInventory != null && !SelectedFacilityInventory.FacilityInventoryPos_FacilityInventory.Any();
        }

        private bool onGeneratePositionsStartInventory;

        /// <summary>
        /// Source Property: GeneratePositions
        /// </summary>
        [ACMethodInfo(nameof(GeneratePositions), "en{'Generate positions'}de{'Positionen generieren'}", 180, Description = "Opens the dialog to generate inventory lines for the existing quants (only possible if the inventory has no lines yet).")]
        public async Task GeneratePositions(bool onGetPosStartInv = false)
        {
            if (!IsEnabledGeneratePositions())
                return;
            onGeneratePositionsStartInventory = onGetPosStartInv;
            _GenerateInventoryPosition = true;
            _OmitGenerateSiloQuantPosition = true;
            CloseTopDialog();
            await ShowDialogAsync(this, "DlgGeneratePositions");
        }

        public bool IsEnabledGeneratePositions()
        {
            return
                SelectedFacilityInventory != null
                && !SelectedFacilityInventory.FacilityInventoryPos_FacilityInventory.Any();
        }

        [ACMethodInfo(nameof(GeneratePositionsOK), Const.Ok, 190, Description = "Generates the inventory lines in the background (and starts the inventory if triggered from StartInventory).")]
        public async Task GeneratePositionsOK()
        {
            if (!IsEnabledGeneratePositionsOK())
                return;

            CloseTopDialog();
            BackgroundWorker.RunWorkerAsync(nameof(DoGeneratePositions));
            await ShowDialogAsync(this, DesignNameProgressBar);
        }

        public bool IsEnabledGeneratePositionsOK()
        {
            return IsEnabledGeneratePositions();
        }

        #endregion

        #region Methods -> ACvbBSO -> Delete

        [ACMethodInteraction(nameof(Delete), ConstApp.Delete, (short)MISort.Delete, true, nameof(SelectedFacilityInventory), Global.ACKinds.MSMethodPrePost, Description = "Deletes the selected inventory with all its lines after confirmation (only possible in state New or In progress).")]
        public async void Delete()
        {
            if (!IsEnabledDelete())
                return;
            var questionResult = await Root.Messages.QuestionAsync(this, "Question50062", MsgResult.Yes, false, SelectedFacilityInventory.FacilityInventoryNo);
            if (questionResult == MsgResult.Yes)
            {
                List<FacilityInventoryPos> positions = SelectedFacilityInventory.FacilityInventoryPos_FacilityInventory.ToList();
                foreach (var item in positions)
                {
                    item.DeleteACObject(DatabaseApp, false);
                }

                FacilityInventory facilityInventory = SelectedFacilityInventory;
                AccessPrimary.NavList.Remove(facilityInventory);
                facilityInventory.DeleteACObject(Database, false);

                SelectedFacilityInventory = FacilityInventoryList.FirstOrDefault();
                OnPropertyChanged(nameof(FacilityInventoryList));
            }
        }

        public bool IsEnabledDelete()
        {
            return SelectedFacilityInventory != null
                && SelectedFacilityInventory.MDFacilityInventoryState != null
                && SelectedFacilityInventory.MDFacilityInventoryState.FacilityInventoryState <= FacilityInventoryStateEnum.InProgress;
        }

        [ACMethodInteraction(nameof(DeletePos), ConstApp.Delete, (short)MISort.Delete, true, nameof(SelectedFacilityInventory), Global.ACKinds.MSMethodPrePost, Description = "Placeholder for deleting the selected inventory line; currently without functionality (only the enabled check is implemented).")]
        public void DeletePos()
        {
            if (!IsEnabledDeletePos())
                return;
        }

        public bool IsEnabledDeletePos()
        {
            return SelectedFacilityInventoryPos != null;
        }

        #endregion

        #region Methods -> Inventory Lifecycle

        #region Methods -> Inventory Lifecycle -> Inventory
        // StartInventory

        [ACMethodInfo(nameof(StartInventory), "en{'1.) Start inventory'}de{'1.) Inventur beginnen'}", 200, Description = "Starts the counting: sets the inventory state to In progress; if no lines exist, the generate-positions dialog is shown first.")]
        public async Task StartInventory()
        {
            if (!IsEnabledStartInventory())
                return;
            if (IsEnabledGeneratePositions())
            {
                await GeneratePositions(true);
            }
            else
            {
                DoStartInventory();
            }
        }

        public bool IsEnabledStartInventory()
        {
            return
                SelectedFacilityInventory != null
                && SelectedFacilityInventory.MDFacilityInventoryState != null
                && SelectedFacilityInventory.MDFacilityInventoryState.MDFacilityInventoryStateIndex == (short)FacilityInventoryStateEnum.New;
        }


        [ACMethodInfo(nameof(ClosingInventory), "en{'3.) Post and complete inventory'}de{'3.) Buchen und Inventur beenden'}", 210, Description = "Posts the inventory and completes it (FacilityManager.InventoryClosing); warns first if unclosed lines exist.")]
        public async Task ClosingInventory()
        {
            if (!IsEnabledClosingInventory())
                return;
            bool existAnyUnclosedPosition =
                SelectedFacilityInventory
                .FacilityInventoryPos_FacilityInventory
                .Any(c => c.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex < (short)FacilityInventoryPosStateEnum.Finished);
            if (existAnyUnclosedPosition)
            {
                SelectedFilterInventoryPosState = FilterInventoryPosStateList.FirstOrDefault(c => c.MDFacilityInventoryPosStateIndex == (short)FacilityInventoryPosStateEnum.InProgress);
                SearchPos();
                await Root.Messages.WarningAsync(this, "Warning50038");
            }
            else
            {
                var questionResult = await Root.Messages.QuestionAsync(this, "Question50054");
                if (questionResult == MsgResult.Yes)
                {
                    ACSaveChanges();
                    BackgroundWorker.RunWorkerAsync(nameof(DoInventoryClosing));
                    await ShowDialogAsync(this, DesignNameProgressBar);
                }
            }
        }

        public bool IsEnabledClosingInventory()
        {
            return
                SelectedFacilityInventory != null
                && SelectedFacilityInventory.MDFacilityInventoryState != null
                && SelectedFacilityInventory.MDFacilityInventoryState.MDFacilityInventoryStateIndex == (short)FacilityInventoryStateEnum.InProgress;
        }

        [ACMethodInfo(nameof(CloseAllPositions), "en{'2.) Set selected lines as counted'}de{'2.) Ausgewählte Zeilen als Gezählt kennzeichnen'}", 220, Description = "Marks all selected inventory lines (IsSelected) that are not yet finished as counted (state Finished).")]
        public async void CloseAllPositions()
        {
            var questionResult = await Root.Messages.QuestionAsync(this, "Question50055");
            if (questionResult == MsgResult.Yes)
            {
                FacilityInventoryPos[] inventoryItems = FacilityInventoryPosList.Where(c => c.IsSelected).ToArray();
                foreach (FacilityInventoryPos item in inventoryItems)
                {
                    if (item.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex < (short)FacilityInventoryPosStateEnum.Finished)
                    {
                        item.MDFacilityInventoryPosState = FinishedPosState;
                    }
                }
                ACSaveChanges();
                OnPropertyChanged(nameof(FacilityInventoryPosList));
            }
        }

        public bool IsEnabledCloseAllPositions()
        {
            return SelectedFacilityInventory != null
                && SelectedFacilityInventory.MDFacilityInventoryState != null
                && SelectedFacilityInventory.MDFacilityInventoryState.MDFacilityInventoryStateIndex == (short)FacilityInventoryStateEnum.InProgress;
        }

        [ACMethodInfo(nameof(SendToERP), "en{'Send to ERP'}de{'An ERP senden'}", 230, Description = "Marks a finished or canceled inventory as posted (transfer to ERP).")]
        public virtual void SendToERP()
        {
            // do nothing
            if (SelectedFacilityInventory.MDFacilityInventoryState.FacilityInventoryState != FacilityInventoryStateEnum.Canceled)
            {
                MDFacilityInventoryState postedInventoryState = DatabaseApp.MDFacilityInventoryState.FirstOrDefault(c => c.MDFacilityInventoryStateIndex == (short)FacilityInventoryStateEnum.Posted);
                SelectedFacilityInventory.MDFacilityInventoryState = postedInventoryState;
                ACSaveChanges();
                OnPropertyChanged(nameof(SelectedFacilityInventory));
            }
        }

        public virtual bool IsEnabledSendToERP()
        {
            return
                SelectedFacilityInventory != null
                && SelectedFacilityInventory.MDFacilityInventoryState != null
                && (
                        SelectedFacilityInventory.MDFacilityInventoryState.FacilityInventoryState == FacilityInventoryStateEnum.Finished
                        || SelectedFacilityInventory.MDFacilityInventoryState.FacilityInventoryState == FacilityInventoryStateEnum.Canceled
                    );
        }


        [ACMethodInfo(nameof(CancelInventory), "en{'Cancel Inventory'}de{'Inventur stornieren'}", 240, Description = "Cancels the inventory after confirmation and reverts all bookings made by it (FacilityManager.CancelInventory).")]
        public virtual async void CancelInventory()
        {
            if (!IsEnabledCancelInventory())
                return;
            // Question50120
            // BSOFacilityInventory
            // Do you want to cancel inventory {0}? All already made bookings will be reverted! Quants will have state as before inventory!
            // Möchten Sie den Bestand {0} stornieren? Alle bereits getätigten Buchungen werden rückgängig gemacht! Der Bestand bleibt unverändert!
            var questionResult = await Root.Messages.QuestionAsync(this, "Question50120", MsgResult.Yes, false, SelectedFacilityInventory.FacilityInventoryNo);
            if (questionResult == MsgResult.Yes)
            {
                ACSaveChanges();
                BackgroundWorker.RunWorkerAsync(nameof(DoCancelInventory));
                await ShowDialogAsync(this, DesignNameProgressBar);
            }
        }

        public virtual bool IsEnabledCancelInventory()
        {
            return
                SelectedFacilityInventory != null
                && SelectedFacilityInventory.MDFacilityInventoryState != null
                && SelectedFacilityInventory.MDFacilityInventoryState.FacilityInventoryState >= FacilityInventoryStateEnum.Finished
                && SelectedFacilityInventory.MDFacilityInventoryState.FacilityInventoryState < FacilityInventoryStateEnum.Canceled;
        }

        #endregion

        #region Methods -> Inventory Lifecycle -> InventoryPos


        [ACMethodInfo(nameof(StartInventoryPos), "en{'Start'}de{'Starten'}", 250, Description = "Sets the selected inventory line to state In progress (only for lines in state New).")]
        public void StartInventoryPos()
        {
            if (!IsEnabledStartInventoryPos())
                return;
            SetInventoryPosState(SelectedFacilityInventoryPos, FacilityInventoryPosStateEnum.InProgress);
            OnPropertyChanged(nameof(SelectedFacilityInventoryPos));
        }

        public bool IsEnabledStartInventoryPos()
        {
            return
                SelectedFacilityInventory != null
                && SelectedFacilityInventory.MDFacilityInventoryState.MDFacilityInventoryStateIndex == (short)FacilityInventoryStateEnum.InProgress
                && SelectedFacilityInventoryPos != null
                && SelectedFacilityInventoryPos.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex == (short)FacilityInventoryPosStateEnum.New;
        }


        [ACMethodInfo(nameof(ClosingInventoryPos), "en{'Close filtered inventory lines'}de{'Gefilterte Inventurpositionen abschließen'}", 260, Description = "Sets the selected inventory line to state Finished (only for lines in state In progress).")]
        public void ClosingInventoryPos()
        {
            if (!IsEnabledClosingInventoryPos())
                return;
            SetInventoryPosState(SelectedFacilityInventoryPos, FacilityInventoryPosStateEnum.Finished);
            OnPropertyChanged(nameof(SelectedFacilityInventoryPos));
        }

        public bool IsEnabledClosingInventoryPos()
        {
            return
                SelectedFacilityInventoryPos != null &&
                SelectedFacilityInventoryPos.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex == (short)FacilityInventoryPosStateEnum.InProgress;
        }

        private void SetInventoryPosState(FacilityInventoryPos pos, FacilityInventoryPosStateEnum posState)
        {
            MDFacilityInventoryPosState inventoryState = DatabaseApp.MDFacilityInventoryPosState.FirstOrDefault(c => c.MDFacilityInventoryPosStateIndex == (short)posState);
            pos.MDFacilityInventoryPosState = inventoryState;
            DatabaseApp.ACSaveChanges();
        }

        [ACMethodInfo(nameof(CopyQuantityFromStock), "en{'Takeover stock'}de{'Übernehme Lagerbestand'}", 270, Description = "Takes over the current stock quantity of the selected line as counted quantity (NewStockQuantity).")]
        public void CopyQuantityFromStock()
        {
            SelectedFacilityInventoryPos.NewStockQuantity =
                SelectedFacilityInventoryPos.StockQuantity;
            OnPropertyChanged("SelectedFacilityInventoryPos\\NewStockQuantity");
        }

        public bool IsEnabledCopyQuantityFromStock()
        {
            return SelectedFacilityInventoryPos != null
                && SelectedFacilityInventoryPos.MDFacilityInventoryPosState != null
                && SelectedFacilityInventoryPos.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex < (short)FacilityInventoryPosStateEnum.Finished
                && !SelectedFacilityInventoryPos.NotAvailable;
        }

        #endregion

        #region Methods -> ACvbBSO -> Save
        [ACMethodCommand(nameof(Save), ConstApp.Save, (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = "Saves all changes of the selected inventory and its lines to the database.")]
        public async Task Save()
        {
            await OnSave();
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
        [ACMethodCommand(Material.ClassName, ConstApp.Undo, (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = "Discards unsaved changes of the selected inventory and reloads it.")]
        public void UndoSave()
        {
            OnUndoSave();
            Load();
        }

        /// <summary>
        /// Determines whether [is enabled undo save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled undo save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        #endregion

        #region Methods -> Add FacilityCharge

        [ACMethodInfo(nameof(AddFacilityCharge), "en{'Restore quant'}de{'Quanten wiederherstellen'}", 280, Description = "Adds the selected unused quant (NotUsedFacilityCharge) to the inventory as a new line; the quant is released if it was marked not available.")]
        public void AddFacilityCharge()
        {
            if (!IsEnabledAddFacilityCharge())
                return;
            if (SelectedNotUsedFacilityCharge.NotAvailable)
                SelectedNotUsedFacilityCharge.NotAvailable = false;

            if (SelectedFacilityInventory.IsFaciltiyMatch(SelectedNotUsedFacilityCharge.Facility))
            {
                FacilityInventoryPos facilityInventoryPos = FacilityInventoryPos.NewACObject(DatabaseApp, SelectedFacilityInventory);
                facilityInventoryPos.FacilityCharge = SelectedNotUsedFacilityCharge;
                DatabaseApp.FacilityInventoryPos.Add(facilityInventoryPos);
                FacilityInventoryPosList.Add(facilityInventoryPos);
                SelectedFacilityInventoryPos = facilityInventoryPos;
                OnPropertyChanged(nameof(FacilityInventoryPosList));

                NotUsedFacilityChargeList.Remove(SelectedNotUsedFacilityCharge);
                SelectedNotUsedFacilityCharge = NotUsedFacilityChargeList.FirstOrDefault();
                OnPropertyChanged(nameof(NotUsedFacilityChargeList));
            }
            else
            {
                Messages.ErrorAsync(this, "Error50560", false, SelectedFacilityInventory.Facility?.FacilityNo, SelectedFacilityInventory.Facility?.FacilityName,
                    SelectedNotUsedFacilityCharge.Facility?.FacilityNo, SelectedNotUsedFacilityCharge.Facility?.FacilityName);
            }
        }

        public bool IsEnabledAddFacilityCharge()
        {
            return
                SelectedFacilityInventory != null
                && SelectedFacilityInventory.MDFacilityInventoryState.MDFacilityInventoryStateIndex < (short)FacilityInventoryStateEnum.Finished
                && SelectedNotUsedFacilityCharge != null;
        }


        /// <summary>
        /// Source Property: NewFacilityCharge
        /// </summary>
        [ACMethodInfo(nameof(NewFacilityCharge), "en{'Add new quant to inventory list'}de{'Neues Quant in Inventurliste erfassen'}", 290, Description = "Opens the quant dialog to create a new quant and adds it to the inventory as a new line.")]
        public void NewFacilityCharge()
        {
            if (!IsEnabledNewFacilityCharge())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo() { DialogSelectInfo = 2 };
                info.Entities.Add(new PAOrderInfoEntry(nameof(Facility), SelectedFacilityInventory.FacilityID ?? Guid.Empty));
                info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityCharge), Guid.Empty));
                service.ShowDialogOrder(this, info);

                if (info.DialogResult != null && info.DialogResult.ReturnValue != null)
                {
                    if (info.DialogResult.ReturnValue is FacilityCharge)
                    {
                        FacilityCharge newFacilityCharge = info.DialogResult.ReturnValue as FacilityCharge;

                        FacilityInventoryPos facilityInventoryPos = FacilityInventoryPos.NewACObject(DatabaseApp, SelectedFacilityInventory);
                        facilityInventoryPos.FacilityCharge = newFacilityCharge;
                        DatabaseApp.FacilityInventoryPos.Add(facilityInventoryPos);
                        FacilityInventoryPosList.Insert(0, facilityInventoryPos);
                        SelectedFacilityInventoryPos = facilityInventoryPos;
                        OnPropertyChanged(nameof(FacilityInventoryPosList));
                    }
                }
            }
        }

        public bool IsEnabledNewFacilityCharge()
        {
            return IsEnabledClosingInventory() && SelectedFacilityInventory.Facility != null;
        }

        #endregion

        #region Methods -> Mass update


        /// <summary>
        /// Use Stock quantity as Inventory quantity
        /// </summary>
        [ACMethodInfo(nameof(CopyQuantityFromStockForSelected), "en{'Copy quantity from stock'}de{'Menge aus Lagerbestand kopieren'}", 300, Description = "Takes over the stock quantity as counted quantity for all selected lines and finishes them (after confirmation).")]
        public async void CopyQuantityFromStockForSelected()
        {
            if (!IsEnabledCopyQuantityFromStockForSelected())
                return;

            // Question50115
            // For selected lines, take over the old inventory to the counted inventory?
            // Bei ausgewählten Zeilen den alten Bestand in gezählten Bestand übernehmen?
            if (await Messages.QuestionAsync(this, "Question50115") == Global.MsgResult.Yes)
            {
                BackgroundWorker.RunWorkerAsync(nameof(DoCopyQuantityFromStockForSelected));
                await ShowDialogAsync(this, DesignNameProgressBar);
            }
        }

        public bool IsEnabledCopyQuantityFromStockForSelected()
        {
            return IsEnabledClosingInventory() && IsAnyFacilityPosItemSelected();
        }


        /// <summary>
        /// Set all selected charges as not available
        /// </summary>
        [ACMethodInfo(nameof(NotAvailableForSelected), "en{'Set not available'}de{'Auf nicht verfügbar setzen'}", 310, Description = "Marks all selected lines as not available (infinite-stock quants are kept available) and finishes them (after confirmation).")]
        public async void NotAvailableForSelected()
        {
            if (!IsEnabledNotAvailableForSelected())
                return;
            // Question50116
            // Set selected lines to unavailable? 
            // Ausgewählte Zeilen auf nicht verfügbar setzen?
            if (await Messages.QuestionAsync(this, "Question50116") == Global.MsgResult.Yes)
            {
                BackgroundWorker.RunWorkerAsync(nameof(DoNotAvailableForSelected));
                await ShowDialogAsync(this, DesignNameProgressBar);
            }
        }

        public bool IsEnabledNotAvailableForSelected()
        {
            return IsEnabledClosingInventory() && IsAnyFacilityPosItemSelected();
        }



        /// <summary>
        /// Source Property: MakePosMassChage
        /// </summary>
        [ACMethodInfo(nameof(InventoryPosStateMassChange), "en{'Change status for selected'}de{'Status für ausgewählte ändern'}", 320, Description = "Sets the state selected in SelectedPosStateMassChange for all selected lines below Posted.")]
        public void InventoryPosStateMassChange()
        {
            if (!IsEnabledInventoryPosStateMassChange())
                return;
            FacilityInventoryPos[] positions = FacilityInventoryPosList.Where(c => c.IsSelected).ToArray();
            foreach (FacilityInventoryPos item in positions)
            {
                if (item.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex < (short)FacilityInventoryPosStateEnum.Posted)
                {
                    item.MDFacilityInventoryPosState = SelectedPosStateMassChange;
                }
            }
        }

        public bool IsEnabledInventoryPosStateMassChange()
        {
            return
                SelectedPosStateMassChange != null
                && IsEnabledNotAvailableForSelected()
                && FacilityInventoryPosList.Where(c => c.IsSelected && c.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex < (short)FacilityInventoryPosStateEnum.Posted).Any(); 
        }


        #endregion

        #region Mehtods -> Override

        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            if (vbControl == null)
                return base.OnGetControlModes(vbControl);

            Global.ControlModes result = base.OnGetControlModes(vbControl);
            if (result < Global.ControlModes.Enabled)
                return result;
            bool isNotAvailable = SelectedFacilityInventoryPos != null ? SelectedFacilityInventoryPos.NotAvailable : false;
            switch (vbControl.VBContent)
            {
                case "!CopyQuantityFromStock":
                    return !isNotAvailable ? Global.ControlModes.Enabled : Global.ControlModes.Disabled;
                case "SelectedFacilityInventoryPos\\NewStockQuantity":
                    return !isNotAvailable ? Global.ControlModes.Enabled : Global.ControlModes.Disabled;
                case "SelectedFacilityInventoryPos\\MDFacilityInventoryPosState":
                    return
                        SelectedFacilityInventoryPos != null
                        && SelectedFacilityInventoryPos.MDFacilityInventoryPosState != null
                        && SelectedFacilityInventoryPos.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex <= (short)FacilityInventoryPosStateEnum.Finished

                        ? Global.ControlModes.Enabled : Global.ControlModes.Disabled;
                case "SelectedFacilityInventory\\Facility":
                    return IsEnabledChangeInventoryFacility() ? Global.ControlModes.Enabled : Global.ControlModes.Disabled;
                case "SelectedFacilityInventoryPos\\NotAvailable":
                    return SelectedFacilityInventoryPos == null || SelectedFacilityInventoryPos.IsInfiniteStock ? ControlModes.Disabled : ControlModes.Enabled;
            }

            return result;
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(AddFacilityCharge):
                    AddFacilityCharge();
                    return true;
                case nameof(ChangeInventoryFacility):
                    result = ChangeInventoryFacility();
                    return true;
                case nameof(ClearSearch):
                    ClearSearch();
                    return true;
                case nameof(ClearSearchPos):
                    ClearSearchPos();
                    return true;
                case nameof(CloseAllPositions):
                    CloseAllPositions();
                    return true;
                case nameof(ClosingInventory):
                    result = ClosingInventory();
                    return true;
                case nameof(ClosingInventoryPos):
                    ClosingInventoryPos();
                    return true;
                case nameof(CopyQuantityFromStock):
                    CopyQuantityFromStock();
                    return true;
                case nameof(CopyQuantityFromStockForSelected):
                    CopyQuantityFromStockForSelected();
                    return true;
                case nameof(Delete):
                    Delete();
                    return true;
                case nameof(DeletePos):
                    DeletePos();
                    return true;
                case nameof(GeneratePositions):
                    result = GeneratePositions(acParameter.Count() == 1 ? (System.Boolean)acParameter[0] : false);
                    return true;
                case nameof(GeneratePositionsOK):
                    result = GeneratePositionsOK();
                    return true;
                case nameof(IsEnabledAddFacilityCharge):
                    result = IsEnabledAddFacilityCharge();
                    return true;
                case nameof(IsEnabledChangeInventoryFacility):
                    result = IsEnabledChangeInventoryFacility();
                    return true;
                case nameof(IsEnabledClearSearchPos):
                    result = IsEnabledClearSearchPos();
                    return true;
                case nameof(IsEnabledCloseAllPositions):
                    result = IsEnabledCloseAllPositions();
                    return true;
                case nameof(IsEnabledClosingInventory):
                    result = IsEnabledClosingInventory();
                    return true;
                case nameof(IsEnabledClosingInventoryPos):
                    result = IsEnabledClosingInventoryPos();
                    return true;
                case nameof(IsEnabledCopyQuantityFromStock):
                    result = IsEnabledCopyQuantityFromStock();
                    return true;
                case nameof(IsEnabledCopyQuantityFromStockForSelected):
                    result = IsEnabledCopyQuantityFromStockForSelected();
                    return true;
                case nameof(IsEnabledDelete):
                    result = IsEnabledDelete();
                    return true;
                case nameof(IsEnabledDeletePos):
                    result = IsEnabledDeletePos();
                    return true;
                case nameof(IsEnabledGeneratePositions):
                    result = IsEnabledGeneratePositions();
                    return true;
                case nameof(IsEnabledGeneratePositionsOK):
                    result = IsEnabledGeneratePositionsOK();
                    return true;
                case nameof(IsEnabledLoad):
                    result = IsEnabledLoad();
                    return true;
                case nameof(IsEnabledNewDlgOk):
                    result = IsEnabledNewDlgOk();
                    return true;
                case nameof(IsEnabledNewFacilityCharge):
                    result = IsEnabledNewFacilityCharge();
                    return true;
                case nameof(IsEnabledNotAvailableForSelected):
                    result = IsEnabledNotAvailableForSelected();
                    return true;
                case nameof(IsEnabledSave):
                    result = IsEnabledSave();
                    return true;
                case nameof(IsEnabledSearchPos):
                    result = IsEnabledSearchPos();
                    return true;
                case nameof(IsEnabledSelectPos):
                    result = IsEnabledSelectPos();
                    return true;
                case nameof(IsEnabledSendToERP):
                    result = IsEnabledSendToERP();
                    return true;
                case nameof(IsEnabledShowFaciltiyDialog):
                    result = IsEnabledShowFaciltiyDialog();
                    return true;
                case nameof(IsEnabledStartInventory):
                    result = IsEnabledStartInventory();
                    return true;
                case nameof(IsEnabledStartInventoryPos):
                    result = IsEnabledStartInventoryPos();
                    return true;
                case nameof(IsEnabledUndoSave):
                    result = IsEnabledUndoSave();
                    return true;
                case nameof(Load):
                    Load(acParameter.Count() == 1 ? (System.Boolean)acParameter[0] : false);
                    return true;
                case nameof(New):
                    result = New();
                    return true;
                case nameof(NewDlgOk):
                    result = NewDlgOk();
                    return true;
                case nameof(NewDlgOkCancel):
                    NewDlgOkCancel();
                    return true;
                case nameof(NotAvailableForSelected):
                    NotAvailableForSelected();
                    return true;
                case nameof(OnTrackingCall):
                    OnTrackingCall((GlobalApp.TrackingAndTracingSearchModel)acParameter[0], (gip.core.datamodel.IACObject)acParameter[1], (System.Object)acParameter[2], (TrackingEnginesEnum)acParameter[3]);
                    return true;
                case nameof(Save):
                    result = Save();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
                case nameof(SearchFacilityInventoryFacilityBookingCharge):
                    SearchFacilityInventoryFacilityBookingCharge();
                    return true;
                case nameof(SearchNotUsedFaciltiyCharge):
                    SearchNotUsedFaciltiyCharge();
                    return true;
                case nameof(SearchPos):
                    SearchPos();
                    return true;
                case nameof(SelectPos):
                    SelectPos();
                    return true;
                case nameof(SendToERP):
                    SendToERP();
                    return true;
                case nameof(ShowFaciltiyDialog):
                    result = ShowFaciltiyDialog();
                    return true;
                case nameof(StartInventory):
                    result = StartInventory();
                    return true;
                case nameof(StartInventoryPos):
                    StartInventoryPos();
                    return true;
                case nameof(UndoSave):
                    UndoSave();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                // Core methods
                case nameof(AddFacilityCharge):
                case nameof(IsEnabledAddFacilityCharge):
                    return new[] { nameof(SelectedFacilityInventory), nameof(SelectedNotUsedFacilityCharge) };
                case nameof(ChangeInventoryFacility):
                case nameof(IsEnabledChangeInventoryFacility):
                    return new[] { nameof(SelectedFacilityInventory) };
                case nameof(CopyQuantityFromStock):
                case nameof(IsEnabledCopyQuantityFromStock):
                    return new[] { nameof(SelectedFacilityInventory), nameof(SelectedFacilityInventoryPos) };
                case nameof(CopyQuantityFromStockForSelected):
                case nameof(IsEnabledCopyQuantityFromStockForSelected):
                    return new[] { nameof(SelectedFacilityInventory), nameof(FacilityInventoryPosList) };
                case nameof(NotAvailableForSelected):
                case nameof(IsEnabledNotAvailableForSelected):
                    return new[] { nameof(SelectedFacilityInventory), nameof(FacilityInventoryPosList) };
                case nameof(CloseAllPositions):
                case nameof(IsEnabledCloseAllPositions):
                    return new[] { nameof(SelectedFacilityInventory), nameof(SelectedFacilityInventoryPos) };
                case nameof(ClosingInventory):
                case nameof(IsEnabledClosingInventory):
                    return new[] { nameof(SelectedFacilityInventory) };
                case nameof(ClosingInventoryPos):
                case nameof(IsEnabledClosingInventoryPos):
                    return new[] { nameof(SelectedFacilityInventory), nameof(SelectedFacilityInventoryPos) };
                // Search methods
                case nameof(ClearSearch):
                    return new[] { nameof(SelectedFacilityInventory), nameof(FilterInventoryPosState) };
                case nameof(ClearSearchPos):
                case nameof(IsEnabledClearSearchPos):
                    return new[] { nameof(SelectedFacilityInventory), nameof(FilterInventoryPosState) };
                case nameof(SearchPos):
                case nameof(IsEnabledSearchPos):
                    return new[] { nameof(SelectedFacilityInventory) };
                case nameof(SelectPos):
                case nameof(IsEnabledSelectPos):
                    return new[] { nameof(SelectedFacilityInventory), nameof(SelectedFacilityInventoryPos) };
                // CRUD methods
                case nameof(Delete):
                case nameof(IsEnabledDelete):
                case nameof(DeletePos):
                case nameof(IsEnabledDeletePos):
                    return new[] { nameof(SelectedFacilityInventory) };
                case nameof(Load):
                case nameof(IsEnabledLoad):
                    return new[] { nameof(SelectedFacilityInventory) };
                case nameof(New):
                    return new[] { nameof(SelectedFacilityInventory) };
                case nameof(Save):
                case nameof(IsEnabledSave):
                    return new[] { nameof(SelectedFacilityInventory) };
                case nameof(UndoSave):
                case nameof(IsEnabledUndoSave):
                    return new[] { nameof(SelectedFacilityInventory) };
                // Generate positions
                case nameof(GeneratePositions):
                case nameof(IsEnabledGeneratePositions):
                case nameof(GeneratePositionsOK):
                case nameof(IsEnabledGeneratePositionsOK):
                    return new[] { nameof(SelectedFacilityInventory) };
                // New facility charge
                case nameof(IsEnabledNewFacilityCharge):
                    return new[] { nameof(SelectedFacilityInventory) };
                case nameof(NewFacilityCharge):
                    return new[] { nameof(SelectedFacilityInventory) };
                // Dialog methods
                case nameof(ShowFaciltiyDialog):
                case nameof(IsEnabledShowFaciltiyDialog):
                    return new[] { nameof(SelectedFacilityInventory) };
                case nameof(StartInventory):
                case nameof(IsEnabledStartInventory):
                    return new[] { nameof(SelectedFacilityInventory) };
                case nameof(StartInventoryPos):
                case nameof(IsEnabledStartInventoryPos):
                    return new[] { nameof(SelectedFacilityInventory), nameof(SelectedFacilityInventoryPos) };
                // New dialog
                case nameof(NewDlgOk):
                case nameof(IsEnabledNewDlgOk):
                    return new[] { nameof(SelectedFacilityInventory) };
                case nameof(NewDlgOkCancel):
                    return Array.Empty<string>();
                // Send to ERP
                case nameof(SendToERP):
                case nameof(IsEnabledSendToERP):
                    return new[] { nameof(SelectedFacilityInventory) };
                // Search methods
                case nameof(Search):
                    return new[] { nameof(SelectedFacilityInventory) };
                case nameof(SearchFacilityInventoryFacilityBookingCharge):
                    return new[] { nameof(SelectedFacilityInventory) };
                case nameof(SearchNotUsedFaciltiyCharge):
                    return new[] { nameof(SelectedFacilityInventory) };
                // Tracking
                case nameof(OnTrackingCall):
                    return new[] { nameof(SelectedInventoryPosFacilityBooking), nameof(SelectedInventoryFacilityBookingCharge) };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        public override object Clone()
        {
            object clonedObject = base.Clone();
            BSOFacilityInventory cloned = clonedObject as BSOFacilityInventory;
            cloned.SelectedFacilityInventoryPos = SelectedFacilityInventoryPos;
            return cloned;
        }

        #endregion

        #region Tracking

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vbContent"></param>
        /// <param name="vbControl"></param>
        /// <returns></returns>
        public override ACMenuItemList GetMenu(string vbContent, string vbControl)
        {
            ACMenuItemList aCMenuItems = base.GetMenu(vbContent, vbControl);
            if (vbContent == nameof(SelectedInventoryPosFacilityBooking) && SelectedInventoryPosFacilityBooking != null)
            {
                TrackingCommonStart trackingCommonStart = new TrackingCommonStart();
                ACMenuItemList trackingAndTracingMenuItems = trackingCommonStart.GetTrackingAndTrackingMenuItems(this, SelectedInventoryPosFacilityBooking);
                aCMenuItems.AddRange(trackingAndTracingMenuItems);
            }
            else if (vbContent == nameof(SelectedInventoryFacilityBookingCharge) && SelectedInventoryFacilityBookingCharge != null)
            {
                TrackingCommonStart trackingCommonStart = new TrackingCommonStart();
                ACMenuItemList trackingAndTracingMenuItems = trackingCommonStart.GetTrackingAndTrackingMenuItems(this, SelectedInventoryFacilityBookingCharge.FacilityBooking);
                aCMenuItems.AddRange(trackingAndTracingMenuItems);
            }
            return aCMenuItems;
        }


        [ACMethodInfo(nameof(OnTrackingCall), "en{'OnTrackingCall'}de{'OnTrackingCall'}", 330, false, Description = "Opens tracking and tracing for the selected booking or booking charge.")]
        public void OnTrackingCall(GlobalApp.TrackingAndTracingSearchModel direction, IACObject itemForTrack, object additionalFilter, TrackingEnginesEnum engine)
        {
            TrackingCommonStart trackingCommonStart = new TrackingCommonStart();
            trackingCommonStart.DoTracking(this, direction, itemForTrack, additionalFilter, engine);
        }

        #endregion

        #endregion

        #region BackgroundWorker

        #region BackgroundWorker  -> BgWorkerDoWork

        /// <summary>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        public override void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            base.BgWorkerDoWork(sender, e);
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = e.Argument.ToString();
            worker.ProgressInfo.ProgressInfoIsIndeterminate = false;
            worker.ProgressInfo.OnlyTotalProgress = true;
            switch (command)
            {
                case nameof(DoNewInventory):
                    e.Result = DoNewInventory();
                    break;
                case nameof(DoGeneratePositions):
                    DoGeneratePositions(SelectedFacilityInventory);
                    if (onGeneratePositionsStartInventory)
                    {
                        DoStartInventory();
                        onGeneratePositionsStartInventory = false;
                    }
                    break;
                case nameof(DoInventoryClosing):
                    e.Result = DoInventoryClosing();
                    break;
                case nameof(DoCopyQuantityFromStockForSelected):
                    DoCopyQuantityFromStockForSelected();
                    break;
                case nameof(DoNotAvailableForSelected):
                    DoNotAvailableForSelected();
                    break;
                case nameof(DoCancelInventory):
                    e.Result = DoCancelInventory();
                    break;
            }
        }

        public override void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            base.BgWorkerCompleted(sender, e);
            CloseWindow(this, DesignNameProgressBar);
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = worker.EventArgs.Argument.ToString();

            if (e.Cancelled)
            {
                SendMessage(new Msg() { MessageLevel = eMsgLevel.Info, Message = string.Format(@"Operation {0} canceled by user!", command) });
            }
            if (e.Error != null)
            {
                SendMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = string.Format(@"Error by doing {0}! Message:{1}", command, e.Error.Message) });
            }
            else
            {
                switch (command)
                {
                    case nameof(DoNewInventory):
                        MsgWithDetails creatingMessage = e.Result as MsgWithDetails;
                        if (creatingMessage == null || creatingMessage.IsSucceded())
                        {
                            Search();
                            SelectedFacilityInventory = FacilityInventoryList.FirstOrDefault(c => c.FacilityInventoryNo == NewFaciltiyInventoryNo);
                        }
                        else
                        {
                            SendMessage(creatingMessage);
                        }
                        break;
                    case nameof(DoGeneratePositions):
                        MsgWithDetails msgAddPositions = e.Result as MsgWithDetails;
                        if (msgAddPositions == null || msgAddPositions.IsSucceded())
                        {
                            RefreshInventory(true);
                        }
                        else
                        {
                            SendMessage(msgAddPositions);
                        }
                        break;
                    case nameof(DoInventoryClosing):
                        MsgWithDetails closingMessage = e.Result as MsgWithDetails;
                        if (closingMessage.IsSucceded())
                        {
                            RefreshInventory(true);
                        }
                        else
                        {
                            if (closingMessage.MsgDetails.Any(c => c.MessageLevel == eMsgLevel.Error && c.ACIdentifier == FacilityManager.Const_Inventory_NotAllowedClosing))
                            {
                                Msg existUnclosedPositions = new Msg(this, eMsgLevel.Warning, nameof(FacilityInventory), "DoInventoryClosing", 1015, "Warning50038");
                                SendMessage(existUnclosedPositions);
                            }
                            else
                                SendMessage(closingMessage);
                        }
                        break;
                    case nameof(DoCopyQuantityFromStockForSelected):
                    case nameof(DoNotAvailableForSelected):
                        OnPropertyChanged(nameof(FacilityInventoryPosList));
                        break;
                    case nameof(DoCancelInventory):
                        MsgWithDetails cancelingMessage = e.Result as MsgWithDetails;
                        RefreshInventory(true);
                        if (!cancelingMessage.IsSucceded())
                        {
                            SendMessage(cancelingMessage);
                        }
                        break;
                }
            }
        }

        #endregion

        #region BackgroundWorker -> Do methods

        private MsgWithDetails DoNewInventory()
        {
            return ACFacilityManager.InventoryGenerate(NewFaciltiyInventoryNo, NewFaciltiyInventoryName,
                SelectedNewInventoryFacility?.FacilityID, GenerateInventoryPosition, OmitGenerateSiloQuantPosition,DefaultSuggestStockQuantity, DoNewInventoryProgressCallback);
        }

        private MsgWithDetails DoGeneratePositions(FacilityInventory facilityInventory)
        {
            MsgWithDetails msg = null;
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                FacilityInventory tmpInventory = databaseApp.FacilityInventory.FirstOrDefault(c => c.FacilityInventoryID == facilityInventory.FacilityInventoryID);
                ACFacilityManager.InventoryGeneratePositions(databaseApp, tmpInventory, OmitGenerateSiloQuantPosition, null);
                msg = databaseApp.ACSaveChanges();
            }
            return msg;
        }

        private MsgWithDetails DoInventoryClosing()
        {
            return ACFacilityManager.InventoryClosing(SelectedFacilityInventory.FacilityInventoryNo, DoInventoryClosingProgressCallback);
        }

        private void DoCopyQuantityFromStockForSelected()
        {
            FacilityInventoryPos[] selectedItems = FacilityInventoryPosList.Where(c => c.IsSelected).ToArray();
            foreach (FacilityInventoryPos item in selectedItems)
            {
                item.NewStockQuantity = item.StockQuantity;
                item.MDFacilityInventoryPosState = FinishedPosState;
                item.IsSelected = false;
            }
        }

        private void DoNotAvailableForSelected()
        {
            FacilityInventoryPos[] selectedItems = FacilityInventoryPosList.Where(c => c.IsSelected).ToArray();
            foreach (FacilityInventoryPos item in selectedItems)
            {
                item.NewStockQuantity = null;

                if (item.IsInfiniteStock)
                {
                    item.NotAvailable = false;
                }
                else
                {
                    item.NotAvailable = true;
                }

                if (item.NotAvailable)
                {
                    item.MDFacilityInventoryPosState = FinishedPosState;
                }

                item.IsSelected = false;
            }
        }

        private void DoStartInventory()
        {
            MDFacilityInventoryState inProgressState = DatabaseApp.MDFacilityInventoryState.FirstOrDefault(c => c.MDFacilityInventoryStateIndex == (short)FacilityInventoryStateEnum.InProgress);
            SelectedFacilityInventory.MDFacilityInventoryState = inProgressState;
            DatabaseApp.ACSaveChanges();
            OnPropertyChanged(nameof(SelectedFacilityInventory));
            OnPropertyChanged(nameof(SelectedFacilityInventoryPos));
            IsEnabledInventoryEdit = true;
            IsEnabledInventoryPosEdit = IsInventoryPosEnabledEdit();
            OnPropertyChanged(nameof(SelectedFacilityInventoryPos));
        }

        private MsgWithDetails DoCancelInventory()
        {
            return ACFacilityManager.CancelInventory(SelectedFacilityInventory.FacilityInventoryNo, DoInventoryClosingProgressCallback);
        }

        #endregion


        #region BackgroundWorker -> ProgressCallback && Common

        private void DoNewInventoryProgressCallback(int current, int count)
        {
            if (current == 0)
                BackgroundWorker.ProgressInfo.AddSubTask("DoNewInventory", 0, count);
            BackgroundWorker.ProgressInfo.ReportProgress("DoNewInventory", current);
            BackgroundWorker.ProgressInfo.TotalProgress.ProgressText = string.Format(@"DoNewInventory: [{0}] | Progress: {1} / {2} steps...", NewFaciltiyInventoryNo, current, count);
        }

        private void DoInventoryClosingProgressCallback(int current, int count)
        {
            if (current == 0)
                BackgroundWorker.ProgressInfo.AddSubTask("DoInventoryClosing", 0, count);
            BackgroundWorker.ProgressInfo.ReportProgress("DoInventoryClosing", current);
            BackgroundWorker.ProgressInfo.TotalProgress.ProgressText = string.Format(@"DoInventoryClosing: [{0}] | Progress: {1} / {2} steps...", SelectedFacilityInventory.FacilityInventoryNo, current, count);
        }

        private void RefreshInventory(bool refreshPos)
        {
            SelectedFacilityInventory.AutoRefresh();
            if (refreshPos)
            {
                SelectedFacilityInventory.FacilityInventoryPos_FacilityInventory.AutoLoad(SelectedFacilityInventory.FacilityInventoryPos_FacilityInventoryReference, SelectedFacilityInventory);
                SetFacilityInventoryPosList();
            }
            OnPropertyChanged(nameof(SelectedFacilityInventory));
            OnPropertyChanged(nameof(FacilityInventoryList));

            SetInventoryPosFacilityBookingList();
        }

        #endregion

        #endregion

        #region Messages

        public void SendMessage(Msg msg)
        {
            MsgList.Add(msg);
            OnPropertyChanged(nameof(MsgList));
        }


        #region Messages -> Properties
        public const string Message = "Message";
        /// <summary>
        /// The _ current MSG
        /// </summary>
        Msg _CurrentMsg;
        /// <summary>
        /// Gets or sets the current MSG.
        /// </summary>
        /// <value>The current MSG.</value>
        [ACPropertyCurrent(9999, nameof(Message), "en{'Message'}de{'Meldung'}", Description = "Gets or sets the currently selected message of the message list.")]
        public Msg CurrentMsg
        {
            get
            {
                return _CurrentMsg;
            }
            set
            {
                _CurrentMsg = value;
                OnPropertyChanged(nameof(CurrentMsg));
            }
        }

        private ObservableCollection<Msg> msgList;
        /// <summary>
        /// Gets the MSG list.
        /// </summary>
        /// <value>The MSG list.</value>
        [ACPropertyList(9999, nameof(Message), "en{'Messagelist'}de{'Meldungsliste'}", Description = "Gets the list of messages (errors, warnings, infos) of this business object.")]
        public ObservableCollection<Msg> MsgList
        {
            get
            {
                if (msgList == null)
                    msgList = new ObservableCollection<Msg>();
                return msgList;
            }
        }

        #endregion

        #endregion

        #region Common methods
        private bool IsAnyFacilityPosItemSelected()
        {
            return FacilityInventoryPosList != null && FacilityInventoryPosList.Where(c => c.IsSelected).Any();
        }

        private void OnSelectedFacilityInventoryChanged()
        {
            if (AccessPrimary.Selected != null)
            {
                IsEnabledInventoryEdit = AccessPrimary.Selected.MDFacilityInventoryState.MDFacilityInventoryStateIndex == (short)FacilityInventoryStateEnum.InProgress;
            }
            else
            {
                IsEnabledInventoryEdit = false;
            }

            OnPropertyChanged(nameof(SelectedFacilityInventory));

            SetFacilityInventoryPosList();

            ClearFilterInventoryFacilityBookingCharge();
            ClearInventoryFacilityBookingCharge();

            ClearFilterNotUsedCharge();
            ClearNotUserdFaciltiyChargeList();

            if (AccessPrimary.Selected != null)
            {
                AccessPrimary.Selected.PropertyChanged -= _SelectedFacilityInventory_PropertyChanged;
                AccessPrimary.Selected.PropertyChanged += _SelectedFacilityInventory_PropertyChanged;
            }
        }
        #endregion

        #endregion

    }
}
