// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using VD = gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using gip.mes.datamodel;
using gip.mes.autocomponent;
using gip.core.autocomponent;
using gip.core.media;

namespace gip.bso.manufacturing
{
    /// <summary>
    /// Businessobject or App for the material preparation (supply) overview of a production area.
    /// It calculates which materials have to be supplied to which storage bins and lets the user
    /// cover the missing quantities by picking orders, purchase orders or production orders.
    /// This BSO has no SearchWord/NavigateFirst pattern of its own: the demand list is calculated
    /// by the SearchStockMaterial() method (button "Calculate demands"), which raises the
    /// OnSearchStockMaterial event. The hosting application fills the PreparedMaterialList property,
    /// either by calling DoSearchStockMaterial(selectedBatchPlans) (computes the demand list via the
    /// FacilityManager routing) and then LoadMaterialPreparationResult(preparedMaterials), or by
    /// assigning its own list.
    /// Select a material in SelectedPreparedMaterial. The BSO then loads the possible source storage
    /// bins (SourceStorageBinList), the destination storage bins (TargetStorageBinList), the open
    /// picking orders (PickingList), the open purchase orders (InOrderList) and the production
    /// orders (ProdOrderPartslistList) of this material.
    /// To cover a missing quantity select a destination storage bin in SelectedTargetStorageBin
    /// (the picking type and the PickingQuantityUOM are preset from it) and a source storage bin in
    /// SelectedSourceStorageBin. Then either:
    /// - create a picking order with NewPicking() and take the quantity over into it with
    ///   NewPickingPos() (DeletePickingPos() removes the selected picking line again,
    ///   DeletePicking() deletes the whole picking order),
    /// - create a purchase order with NewInOrder() and take the quantity over into it with
    ///   NewInOrderPos() (DeleteInOrderPos() removes the selected order line again,
    ///   DeleteInOrder() deletes the whole purchase order as long as it is still in state
    ///   "Created"), or
    /// - create a production order with GenerateProductionOrder() (DeleteProductionOrder() removes
    ///   it again as long as it is still in state "NewCreated").
    /// ShowPicking(), ShowInOrder() and ShowProductionOrder() open the dialog of the selected order.
    /// The SelectedFilterTargetStorageBin property (FilterTargetStorageBinList) filters the demand
    /// list by destination storage bin; ShowAllPickingLines/ShowAllInOrderLines show the lines of
    /// all orders instead of only the lines of the selected order.
    /// This BSO saves directly to the database inside its methods (ACSaveChanges); there is no
    /// separate Save()/Delete()/Load() workflow as in the master data BSOs.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOMaterialPreparation&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Material Preparation'}de{'Bereitstellung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true,
        Description = @"Businessobject or App for the material preparation (supply) overview of a production area.
        It calculates which materials have to be supplied to which storage bins and lets the user cover the missing quantities by picking orders, purchase orders or production orders.
        This BSO has no SearchWord/NavigateFirst pattern of its own: the demand list is calculated by the SearchStockMaterial() method (button ""Calculate demands""), which raises the OnSearchStockMaterial event.
        The hosting application fills the PreparedMaterialList property, either by calling DoSearchStockMaterial(selectedBatchPlans) (computes the demand list via the FacilityManager routing) and then LoadMaterialPreparationResult(preparedMaterials), or by assigning its own list.
        Select a material in SelectedPreparedMaterial. The BSO then loads the possible source storage bins (SourceStorageBinList), the destination storage bins (TargetStorageBinList), the open picking orders (PickingList), the open purchase orders (InOrderList) and the production orders (ProdOrderPartslistList) of this material.
        To cover a missing quantity select a destination storage bin in SelectedTargetStorageBin (the picking type and the PickingQuantityUOM are preset from it) and a source storage bin in SelectedSourceStorageBin. Then either:
        - create a picking order with NewPicking() and take the quantity over into it with NewPickingPos() (DeletePickingPos() removes the selected picking line again, DeletePicking() deletes the whole picking order),
        - create a purchase order with NewInOrder() and take the quantity over into it with NewInOrderPos() (DeleteInOrderPos() removes the selected order line again, DeleteInOrder() deletes the whole purchase order as long as it is still in state ""Created""), or
        - create a production order with GenerateProductionOrder() (DeleteProductionOrder() removes it again as long as it is still in state ""NewCreated"").
        ShowPicking(), ShowInOrder() and ShowProductionOrder() open the dialog of the selected order.
        The SelectedFilterTargetStorageBin property (FilterTargetStorageBinList) filters the demand list by destination storage bin; ShowAllPickingLines/ShowAllInOrderLines show the lines of all orders instead of only the lines of the selected order.
        This BSO saves directly to the database inside its methods (ACSaveChanges); there is no separate Save()/Delete()/Load() workflow as in the master data BSOs.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOMaterialPreparation&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOMaterialPreparation : ACBSOvb
    {
        #region Events

        public event EventHandler OnSearchStockMaterial;

        #endregion

        #region const

        public const string ProdMatStorage = @"ProdMatStorage";
        public double Const_RangeStockQuantityTolerance = 0.1;
        #endregion

        #region ctor's

        public BSOMaterialPreparation(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }


        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            _PickingManager = ACPickingManager.ACRefToServiceInstance(this);
            if (_PickingManager == null)
                throw new Exception("PickingManager not configured");

            _ProdOrderManager = ACProdOrderManager.ACRefToServiceInstance(this);
            if (_ProdOrderManager == null)
                throw new Exception("ProdOrderManager not configured");

            MediaController = ACMediaController.GetServiceInstance(this);

            _ConfigManager = ConfigManagerIPlus.ACRefToServiceInstance(this);
            if (_ConfigManager == null)
                throw new Exception("ConfigManager not configured");

            _RoutingService = ACRoutingService.ACRefToServiceInstance(this);
            if (_RoutingService == null)
                throw new Exception("ACRoutingService not configured");

            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            var b = await base.ACDeInit(deleteACClassTask);

            if (_ACFacilityManager != null)
                FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;

            if (_PickingManager != null)
                ACPickingManager.DetachACRefFromServiceInstance(this, _PickingManager);
            _PickingManager = null;

            if (_ProdOrderManager != null)
                ACProdOrderManager.DetachACRefFromServiceInstance(this, _ProdOrderManager);
            _ProdOrderManager = null;

            CleanUp();

            MediaController = null;

            if (_RoutingService != null)
                ACRoutingService.DetachACRefFromServiceInstance(this, _RoutingService);
            _RoutingService = null;

            if (_ConfigManager != null)
                ConfigManagerIPlus.DetachACRefFromServiceInstance(this, _ConfigManager);
            _ConfigManager = null;

            return b;
        }

        #endregion

        #region Overrides

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(NewPicking):
                    NewPicking();
                    return true;
                case nameof(IsEnabledNewPicking):
                    result = IsEnabledNewPicking();
                    return true;
                case nameof(DeletePicking):
                    DeletePicking();
                    return true;
                case nameof(IsEnabledDeletePicking):
                    result = IsEnabledDeletePicking();
                    return true;
                case nameof(NewPickingPos):
                    NewPickingPos();
                    return true;
                case nameof(DeletePickingPos):
                    DeletePickingPos();
                    return true;
                case nameof(IsEnabledDeletePickingPos):
                    result = IsEnabledDeletePickingPos();
                    return true;
                case nameof(SearchStockMaterial):
                    SearchStockMaterial();
                    return true;
                case nameof(IsEnabledSearchStockMaterial):
                    result = IsEnabledSearchStockMaterial();
                    return true;
                case nameof(IsEnabledNewPickingPos):
                    result = IsEnabledNewPickingPos();
                    return true;
                case nameof(ShowPicking):
                    ShowPicking();
                    return true;
                case nameof(IsEnabledShowPicking):
                    result = IsEnabledShowPicking();
                    return true;
                case nameof(GenerateProductionOrder):
                    GenerateProductionOrder();
                    return true;
                case nameof(IsEnabledGenerateProductionOrder):
                    result = IsEnabledGenerateProductionOrder();
                    return true;
                case nameof(DeleteProductionOrder):
                    DeleteProductionOrder();
                    return true;
                case nameof(IsEnabledDeleteProductionOrder):
                    result = IsEnabledDeleteProductionOrder();
                    return true;
                case nameof(ShowProductionOrder):
                    ShowProductionOrder();
                    return true;
                case nameof(IsEnabledProductionOrder):
                    result = IsEnabledProductionOrder();
                    return true;
                case nameof(NewInOrder):
                    result = NewInOrder();
                    return true;
                case nameof(IsEnabledNewInOrder):
                    result = IsEnabledNewInOrder();
                    return true;
                case nameof(DeleteInOrder):
                    DeleteInOrder();
                    return true;
                case nameof(IsEnabledDeleteInOrder):
                    result = IsEnabledDeleteInOrder();
                    return true;
                case nameof(NewInOrderPos):
                    NewInOrderPos();
                    return true;
                case nameof(IsEnabledNewInOrderPos):
                    result = IsEnabledNewInOrderPos();
                    return true;
                case nameof(DeleteInOrderPos):
                    DeleteInOrderPos();
                    return true;
                case nameof(IsEnabledDeleteInOrderPos):
                    result = IsEnabledDeleteInOrderPos();
                    return true;
                case nameof(ShowInOrder):
                    ShowInOrder();
                    return true;
                case nameof(IsEnabledShowInOrder):
                    result = IsEnabledShowInOrder();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                #region Picking
                case nameof(NewPicking):
                case nameof(IsEnabledNewPicking):
                    return new string[] { nameof(SelectedTargetStorageBin) };
                case nameof(DeletePicking):
                case nameof(IsEnabledDeletePicking):
                    return new string[] { nameof(SelectedPicking) };
                case nameof(NewPickingPos):
                case nameof(IsEnabledNewPickingPos):
                    return new string[] { nameof(SelectedPicking), nameof(SelectedPreparedMaterial), nameof(SelectedSourceStorageBin) };
                case nameof(DeletePickingPos):
                case nameof(IsEnabledDeletePickingPos):
                    return new string[] { nameof(SelectedPickingPos), nameof(SelectedPreparedMaterial) };
                case nameof(ShowPicking):
                case nameof(IsEnabledShowPicking):
                    return new string[] { nameof(SelectedPicking) };
                #endregion

                #region Prepared Material -> Production Order
                case nameof(GenerateProductionOrder):
                case nameof(IsEnabledGenerateProductionOrder):
                    return new string[] { nameof(SelectedPreparedMaterial) };
                case nameof(DeleteProductionOrder):
                case nameof(IsEnabledDeleteProductionOrder):
                    return new string[] { nameof(SelectedProdOrderPartslist) };
                case nameof(ShowProductionOrder):
                case nameof(IsEnabledProductionOrder):
                    return new string[] { nameof(SelectedProdOrderPartslist) };
                #endregion

                #region Prepared Material -> InOrder
                case nameof(NewInOrder):
                case nameof(IsEnabledNewInOrder):
                    return new string[] { nameof(SelectedPreparedMaterial) };
                case nameof(DeleteInOrder):
                case nameof(IsEnabledDeleteInOrder):
                    return new string[] { nameof(SelectedInOrder) };
                case nameof(NewInOrderPos):
                case nameof(IsEnabledNewInOrderPos):
                    return new string[] { nameof(SelectedInOrder), nameof(SelectedPreparedMaterial) };
                case nameof(DeleteInOrderPos):
                case nameof(IsEnabledDeleteInOrderPos):
                    return new string[] { nameof(SelectedInOrderPos), nameof(SelectedPreparedMaterial) };
                case nameof(ShowInOrder):
                case nameof(IsEnabledShowInOrder):
                    return new string[] { nameof(SelectedInOrder) };
                #endregion

                #region Stock Material
                case nameof(SearchStockMaterial):
                case nameof(IsEnabledSearchStockMaterial):
                    return new string[] { nameof(SelectedSourceStorageBin) };
                #endregion
            }
            return base.GetPropsToObserveForIsEnabled(acMethodName);
        }
        #endregion

        #region Managers

        protected ACRef<ACComponent> _ACFacilityManager = null;
        protected FacilityManager ACFacilityManager
        {
            get
            {
                if (_ACFacilityManager == null)
                    return null;
                return _ACFacilityManager.ValueT as FacilityManager;
            }
        }

        protected ACRef<ACPickingManager> _PickingManager = null;
        protected ACPickingManager PickingManager
        {
            get
            {
                if (_PickingManager == null)
                    return null;
                return _PickingManager.ValueT;
            }
        }

        protected ACRef<ACProdOrderManager> _ProdOrderManager = null;
        protected ACProdOrderManager ProdOrderManager
        {
            get
            {
                if (_ProdOrderManager == null)
                    return null;
                return _ProdOrderManager.ValueT;
            }
        }

        public ACMediaController MediaController { get; set; }

        protected ACRef<ACComponent> _RoutingService = null;
        public ACComponent RoutingService
        {
            get
            {
                if (_RoutingService == null)
                    return null;
                return _RoutingService.ValueT;
            }
        }

        protected ACRef<ConfigManagerIPlus> _ConfigManager = null;
        public ConfigManagerIPlus ConfigManager
        {
            get
            {
                if (_ConfigManager == null)
                    return null;
                return _ConfigManager.ValueT;
            }
        }

        #endregion

        #region Properties

        #region FilterTargetStorageBin
        public const string FilterTargetStorageBin = "FilterTargetStorageBin";

        private ACValueItem _SelectedFilterTargetStorageBin;
        /// <summary>
        /// Selected property for ACValueItem
        /// </summary>
        /// <value>The selected FilterProdPartslistOrder</value>
        [ACPropertySelected(305, nameof(FilterTargetStorageBin), "en{'Target store'}de{'Ziellagerplatz'}", Description = "Gets or sets the destination storage bin the demand list is filtered by. Selecting an entry filters PreparedMaterialList to the materials of this storage bin.")]
        public ACValueItem SelectedFilterTargetStorageBin
        {
            get
            {
                return _SelectedFilterTargetStorageBin;
            }
            set
            {
                if (_SelectedFilterTargetStorageBin != value)
                {
                    _SelectedFilterTargetStorageBin = value;
                    OnPropertyChanged(nameof(PreparedMaterialList));
                    OnPropertyChanged();
                }
            }
        }


        private List<ACValueItem> _FilterTargetStorageBinList;
        /// <summary>
        /// List property for ACValueItem
        /// </summary>
        /// <value>The FilterPickingState list</value>
        [ACPropertyList(306, nameof(FilterTargetStorageBin), "en{'Target store'}de{'Ziellagerplatz'}", Description = "Gets the list of destination storage bins that can be used to filter the demand list. The first entry (\"All storage locations\") shows all materials.")]
        public List<ACValueItem> FilterTargetStorageBinList
        {
            get
            {
                if (_FilterTargetStorageBinList == null)
                    _FilterTargetStorageBinList = new List<ACValueItem>();
                return _FilterTargetStorageBinList;
            }
        }


        #endregion

        #region PreparedMaterial
        public const string PreparedMaterial = "PreparedMaterial";
        private MaterialPreparationModel _SelectedPreparedMaterial;
        [ACPropertySelected(500, nameof(PreparedMaterial), "en{'Material to supply'}de{'Bereitzustellendes Material'}", Description = "Gets or sets the currently selected material of the demand list. Selecting a material loads its source/destination storage bins, picking orders, purchase orders and production orders.")]
        public MaterialPreparationModel SelectedPreparedMaterial
        {
            get
            {
                return _SelectedPreparedMaterial;
            }
            set
            {
                if (_SelectedPreparedMaterial != value)
                {
                    _SelectedPreparedMaterial = value;
                    LoadPreparedMaterial(value);
                    OnPropertyChanged(nameof(SelectedPreparedMaterial));
                }
            }
        }

        private List<MaterialPreparationModel> _PreparedMaterialList;
        [ACPropertyList(501, nameof(PreparedMaterial), "en{'Materials to supply'}de{'Bereitzustellende Materialien'}", Description = "Gets the demand list of materials that have to be supplied, with target quantity, available quantity, already planned quantity (PickingPosQuantityUOM) and missing quantity. Filtered by SelectedFilterTargetStorageBin.")]
        public List<MaterialPreparationModel> PreparedMaterialList
        {
            get
            {
                if (_PreparedMaterialList == null)
                {
                    _PreparedMaterialList = new List<MaterialPreparationModel>();
                }
                var list = _PreparedMaterialList;
                if (SelectedFilterTargetStorageBin != null && SelectedFilterTargetStorageBin.Value != null)
                {
                    list =
                        list
                        .Where(c =>
                                    c.FacilityScheduligGroups
                                    .Select(x => x.Facility.FacilityNo)
                                    .Contains(SelectedFilterTargetStorageBin.Value.ToString())
                        )
                        .ToList();
                }
                return list;
            }
        }

        #endregion

        #region SourceStorageBin
        private FacilityChargeSumFacilityHelper _SelectedSourceStorageBin;
        [ACPropertySelected(502, "SourceStorageBin", "en{'Source storage bin'}de{'Quell-Lagerplatz'}", Description = "Gets or sets the source storage bin from which the material is taken (e.g. for a picking line). Preselected with the bin holding the largest total stock.")]
        public FacilityChargeSumFacilityHelper SelectedSourceStorageBin
        {
            get
            {
                return _SelectedSourceStorageBin;
            }
            set
            {
                if (_SelectedSourceStorageBin != value)
                {
                    _SelectedSourceStorageBin = value;
                    OnPropertyChanged(nameof(SelectedSourceStorageBin));
                }
            }
        }

        private List<FacilityChargeSumFacilityHelper> _SourceStorageBinList;
        [ACPropertyList(503, "SourceStorageBin", "en{'Source storage bins'}de{'Quell-Lagerplätze'}", Description = "Gets the list of possible source storage bins of the selected material with their total, free, blocked and newly planned stock quantities.")]
        public List<FacilityChargeSumFacilityHelper> SourceStorageBinList
        {
            get
            {
                if (_SourceStorageBinList == null)
                    _SourceStorageBinList = new List<FacilityChargeSumFacilityHelper>();
                return _SourceStorageBinList;
            }
        }

        #endregion

        #region TargetStorageBin
        public const string TargetStorageBin = "TargetStorageBin";
        private PlanningTargetStockPreview _SelectedTargetStorageBin;
        [ACPropertySelected(504, nameof(TargetStorageBin), "en{'Target store'}de{'Ziellagerplatz'}", Description = "Gets or sets the destination storage bin to which the material shall be supplied. Selecting a bin presets the picking type and the PickingQuantityUOM (new planned stock quantity) and reloads the picking orders of this picking type.")]
        public PlanningTargetStockPreview SelectedTargetStorageBin
        {
            get
            {
                return _SelectedTargetStorageBin;
            }
            set
            {
                if (_SelectedTargetStorageBin != value)
                {
                    _SelectedTargetStorageBin = value;
                    OnPropertyChanged(nameof(SelectedTargetStorageBin));

                    MDPickingType mDPickingType = null;
                    if (value != null)
                        mDPickingType = value.MDPickingType;
                    LoadPickings(mDPickingType);
                    if (mDPickingType != null)
                    {
                        PickingQuantityUOM = value.NewPlannedStockQuantity;
                    }
                }
            }
        }

        private List<PlanningTargetStockPreview> _TargetStorageBinList;
        [ACPropertyList(505, nameof(TargetStorageBin), "en{'Destination storage bins'}de{'Ziel-Lagerplätze'}", Description = "Gets the list of destination storage bins of the selected material with actual, optimal, ordered and newly planned stock quantity and the picking type used for the supply.")]
        public List<PlanningTargetStockPreview> TargetStorageBinList
        {
            get
            {
                if (_TargetStorageBinList == null)
                    _TargetStorageBinList = new List<PlanningTargetStockPreview>();
                return _TargetStorageBinList;
            }
        }

        #endregion

        #region Picking
        private VD.Picking _SelectedPicking;
        [ACPropertySelected(506, "Picking", "en{'Picking order'}de{'Kommissionierauftrag'}", Description = "Gets or sets the currently selected picking order. Selecting a picking order reloads its picking lines (PickingPosList).")]
        public VD.Picking SelectedPicking
        {
            get
            {
                return _SelectedPicking;
            }
            set
            {
                if (_SelectedPicking != value)
                {
                    _SelectedPicking = value;
                    LoadPickingPositions();
                    OnPropertyChanged(nameof(SelectedPicking));
                }
            }
        }


        private List<VD.Picking> _PickingList;
        [ACPropertyList(507, "Picking", "en{'Picking orders'}de{'Kommissionieraufträge'}", Description = "Gets the open picking orders of the picking type of the selected destination storage bin, sorted by insertion date descending.")]
        public List<VD.Picking> PickingList
        {
            get
            {
                if (_PickingList == null)
                    _PickingList = new List<Picking>();
                return _PickingList;
            }
        }
        #endregion

        #region PickingPos
        private VD.PickingPos _SelectedPickingPos;
        [ACPropertySelected(508, nameof(PickingPos), "en{'Picking line'}de{'Kommissionierposition'}", Description = "Gets or sets the currently selected picking line of the selected picking order.")]
        public VD.PickingPos SelectedPickingPos
        {
            get
            {
                return _SelectedPickingPos;
            }
            set
            {
                if (_SelectedPickingPos != value)
                {
                    _SelectedPickingPos = value;
                    OnPropertyChanged(nameof(SelectedPickingPos));
                }
            }
        }


        private List<VD.PickingPos> _PickingPosList;
        [ACPropertyList(509, nameof(PickingPos), "en{'Picking lines'}de{'Kommissionierpositionen'}", Description = "Gets the picking lines of the selected material. Depending on ShowAllPickingLines, only the lines of the selected picking order or the lines of all open picking orders are shown.")]
        public List<VD.PickingPos> PickingPosList
        {
            get
            {
                if (_PickingPosList == null)
                    _PickingPosList = new List<PickingPos>();
                return _PickingPosList;
            }
        }

        private double _PickingQuantityUOM;
        [ACPropertyInfo(510, nameof(PickingQuantityUOM), "en{'Picking quantity'}de{'Komissioniermenge'}", Description = "Gets or sets the quantity that is taken over into the picking order by NewPickingPos(). Preset with the new planned stock quantity of the selected destination storage bin.")]
        public double PickingQuantityUOM
        {
            get
            {
                return _PickingQuantityUOM;
            }
            set
            {
                if (_PickingQuantityUOM != value)
                {
                    _PickingQuantityUOM = value;
                    OnPropertyChanged(nameof(PickingQuantityUOM));
                }
            }
        }

        private bool _ShowAllPickingLines;
        [ACPropertyInfo(511, nameof(ShowAllPickingLines), "en{'Show of all picking orders'}de{'Von allen Kommisionieraufträgen anzeigen'}", Description = "If true, the picking lines of all open picking orders of the material are shown instead of only the lines of the selected picking order.")]
        public bool ShowAllPickingLines
        {
            get
            {
                return _ShowAllPickingLines;
            }
            set
            {
                if (_ShowAllPickingLines != value)
                {
                    _ShowAllPickingLines = value;
                    LoadPickingPositions();
                    OnPropertyChanged(nameof(ShowAllPickingLines));
                }
            }
        }
        #endregion

        #region InOrder
        private VD.InOrder _SelectedInOrder;
        [ACPropertySelected(526, nameof(InOrder), "en{'Purchase Order'}de{'Bestellung'}", Description = "Gets or sets the currently selected purchase order. Selecting a purchase order reloads its order lines (InOrderPosList).")]
        public VD.InOrder SelectedInOrder
        {
            get
            {
                return _SelectedInOrder;
            }
            set
            {
                if (_SelectedInOrder != value)
                {
                    _SelectedInOrder = value;
                    LoadInOrderPositions();
                    OnPropertyChanged(nameof(SelectedInOrder));
                }
            }
        }


        private List<VD.InOrder> _InOrderList;
        [ACPropertyList(527, nameof(InOrder), "en{'Purchase Orders'}de{'Bestellungen'}", Description = "Gets the open purchase orders (state up to \"InProcess\") that contain a line with the selected material, sorted by insertion date descending.")]
        public List<VD.InOrder> InOrderList
        {
            get
            {
                if (_InOrderList == null)
                    _InOrderList = new List<InOrder>();
                return _InOrderList;
            }
        }

        private VD.InOrderPos _SelectedInOrderPos;
        [ACPropertySelected(528, nameof(InOrderPos), "en{'Purchase order line'}de{'Bestellposition'}", Description = "Gets or sets the currently selected purchase order line of the selected purchase order.")]
        public VD.InOrderPos SelectedInOrderPos
        {
            get
            {
                return _SelectedInOrderPos;
            }
            set
            {
                if (_SelectedInOrderPos != value)
                {
                    _SelectedInOrderPos = value;
                    OnPropertyChanged(nameof(SelectedInOrderPos));
                }
            }
        }


        private List<VD.InOrderPos> _InOrderPosList;
        [ACPropertyList(529, nameof(InOrderPos), "en{'Purchase order lines'}de{'Bestellpositionen'}", Description = "Gets the order lines of the selected purchase order. Depending on ShowAllInOrderLines, all lines of the purchase order or only the lines of the selected material are shown.")]
        public List<VD.InOrderPos> InOrderPosList
        {
            get
            {
                if (_InOrderPosList == null)
                    _InOrderPosList = new List<InOrderPos>();
                return _InOrderPosList;
            }
        }

        private bool _ShowAllInOrderLines;
        [ACPropertyInfo(511, nameof(ShowAllInOrderLines), "en{'Show of all Purchase orders'}de{'Von allen Bestellungen anzeigen'}", Description = "If true, all order lines of the selected purchase order are shown instead of only the lines of the selected material.")]
        public bool ShowAllInOrderLines
        {
            get
            {
                return _ShowAllInOrderLines;
            }
            set
            {
                if (_ShowAllInOrderLines != value)
                {
                    _ShowAllInOrderLines = value;
                    LoadInOrderPositions();
                    OnPropertyChanged(nameof(ShowAllInOrderLines));
                }
            }
        }

        private double _InOrderQuantityUOM;
        [ACPropertyInfo(530, nameof(InOrderQuantityUOM), "en{'Purchase quantity'}de{'Bestellmenge'}", Description = "Gets or sets the quantity that is taken over into the purchase order by NewInOrderPos(). Preset with the missing quantity of the selected material.")]
        public double InOrderQuantityUOM
        {
            get
            {
                return _InOrderQuantityUOM;
            }
            set
            {
                if (_InOrderQuantityUOM != value)
                {
                    _InOrderQuantityUOM = value;
                    OnPropertyChanged(nameof(InOrderQuantityUOM));
                }
            }
        }
        #endregion


        #region Production Order
        private VD.ProdOrderPartslist _SelectedProdOrderPartslist;
        [ACPropertySelected(506, nameof(ProdOrderPartslist), "en{'Production order'}de{'Produktionsauftrag'}", Description = "Gets or sets the currently selected production order (order partslist) of the selected material.")]
        public VD.ProdOrderPartslist SelectedProdOrderPartslist
        {
            get
            {
                return _SelectedProdOrderPartslist;
            }
            set
            {
                if (_SelectedProdOrderPartslist != value)
                {
                    _SelectedProdOrderPartslist = value;
                    OnPropertyChanged(nameof(SelectedProdOrderPartslist));
                }
            }
        }


        private List<VD.ProdOrderPartslist> _ProdOrderPartslistList;
        [ACPropertyList(507, nameof(ProdOrderPartslist), "en{'Production orders'}de{'Produktionsaufträge'}", Description = "Gets the production orders (state up to \"InProduction\") of the selected material, sorted by insertion date descending.")]
        public List<VD.ProdOrderPartslist> ProdOrderPartslistList
        {
            get
            {
                if (_ProdOrderPartslistList == null)
                    _ProdOrderPartslistList = new List<ProdOrderPartslist>();
                return _ProdOrderPartslistList;
            }
        }

        private double _ProdOrderPartslistQuantityUOM;
        [ACPropertyInfo(510, nameof(ProdOrderPartslistQuantityUOM), "en{'Production order size'}de{'Auftragsgröße'}", Description = "Gets or sets the order size with which the production order is created by GenerateProductionOrder(). Preset with the missing quantity of the selected material.")]
        public double ProdOrderPartslistQuantityUOM
        {
            get
            {
                return _ProdOrderPartslistQuantityUOM;
            }
            set
            {
                if (_ProdOrderPartslistQuantityUOM != value)
                {
                    _ProdOrderPartslistQuantityUOM = value;
                    OnPropertyChanged(nameof(ProdOrderPartslistQuantityUOM));
                }
            }
        }

        #endregion

        #endregion

        #region Methods 

        #region Method -> Picking
        [ACMethodCommand(nameof(NewPicking), "en{'New picking order'}de{'Neuer Kommissionierauftrag'}", (short)800, true, Description = "Creates a new picking order of the picking type of the selected destination storage bin, saves it, adds it to PickingList and selects it. Enabled only if a destination storage bin with picking type is selected.")]
        public void NewPicking()
        {
            if (!IsEnabledNewPicking()) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(Picking), Picking.NoColumnName, Picking.FormatNewNo, this);
            var picking = Picking.NewACObject(DatabaseApp, null, secondaryKey);
            picking.MDPickingType = SelectedTargetStorageBin.MDPickingType;
            DatabaseApp.Picking.Add(picking);
            ACSaveChanges();
            PickingList.Add(picking);
            OnPropertyChanged(nameof(PickingList));
            SelectedPicking = picking;
        }

        public bool IsEnabledNewPicking()
        {
            return SelectedTargetStorageBin != null && SelectedTargetStorageBin.MDPickingType != null;
        }

        [ACMethodCommand(nameof(DeletePicking), "en{'Delete picking order'}de{'Lösche Kommissionierauftrag'}", (short)801, true, Description = "Unassigns all picking lines of the selected picking order and removes it from PickingList. Enabled only if a picking order is selected.")]
        public void DeletePicking()
        {
            if (!IsEnabledDeletePicking()) return;
            Picking picking = SelectedPicking;
            PickingList.Remove(SelectedPicking);
            Msg msg = PickingManager.UnassignAllPickingPos(picking, DatabaseApp, true);
            if (msg == null || msg.IsSucceded())
            {
                SelectedPicking = PickingList.FirstOrDefault();
                OnPropertyChanged(nameof(PickingList));
            }
            else
            {
                Messages.MsgAsync(msg);
            }
        }

        public bool IsEnabledDeletePicking()
        {
            return SelectedPicking != null;
        }

        [ACMethodCommand(nameof(NewPickingPos), "en{'Take over into picking order'}de{'Übernehme in Kommissionierauftrag'}", (short)802, true, Description = "Takes the PickingQuantityUOM over into the selected picking order: creates a picking line with the selected material, source and destination storage bin (for production line relations also the link to the related production order positions), updates the planned/missing quantities of the demand list and the storage bins.")]
        public void NewPickingPos()
        {
            VD.PickingPos pickingPos = VD.PickingPos.NewACObject(DatabaseApp, SelectedPicking);
            pickingPos.PickingQuantityUOM = PickingQuantityUOM;
            VD.Material pickingMaterial = DatabaseApp.Material.FirstOrDefault(c => c.MaterialNo == SelectedPreparedMaterial.MaterialNo);
            pickingPos.PickingMaterial = pickingMaterial;
            pickingPos.FromFacility = SelectedSourceStorageBin.Facility;
            pickingPos.ToFacility = SelectedTargetStorageBin.Facility;
            if (SelectedPreparedMaterial.PickingRelationType == PickingRelationTypeEnum.ProductionLine)
            {
                List<ProdOrderPartslistPos> relatedPositions = DatabaseApp.ProdOrderPartslistPos.Where(c => SelectedPreparedMaterial.RelatedOutwardPosIDs.Contains(c.ProdOrderPartslistPosID)).ToList();
                foreach (ProdOrderPartslistPos relatedPos in relatedPositions)
                {
                    PickingPosProdOrderPartslistPos connection = PickingPosProdOrderPartslistPos.NewACObject(DatabaseApp, pickingPos, relatedPos);
                    pickingPos.PickingPosProdOrderPartslistPos_PickingPos.Add(connection);
                }
            }
            PickingPosList.Add(pickingPos);

            // Update SelectedPreparedMaterial
            SelectedPreparedMaterial.PickingPosQuantityUOM += pickingPos.PickingQuantityUOM;
            SelectedPreparedMaterial.MissingQuantityUOM = SelectedPreparedMaterial.TargetQuantityUOM - SelectedPreparedMaterial.PickingPosQuantityUOM;
            _PreparedMaterialList = _PreparedMaterialList.ToList();

            // Update SelectedTargetStorageBin
            _SelectedSourceStorageBin.NewPlannedStock += pickingPos.PickingQuantityUOM ?? 0;
            _SourceStorageBinList = _SourceStorageBinList.ToList();
            _TargetStorageBinList = _TargetStorageBinList.ToList();

            OnPropertyChanged(nameof(PreparedMaterialList));
            OnPropertyChanged(nameof(SourceStorageBinList));
            OnPropertyChanged(nameof(TargetStorageBinList));

            OnPropertyChanged(nameof(PickingPosList));
        }

        public bool IsEnabledNewPickingPos()
        {
            return
                SelectedPicking != null
                && PickingQuantityUOM > 0
                && SelectedPreparedMaterial != null
                && SelectedSourceStorageBin != null
                && SelectedTargetStorageBin != null;
        }

        [ACMethodCommand(nameof(DeletePickingPos), "en{'Delete picking line'}de{'Lösche Kommissionierposition'}", (short)803, true, Description = "Deletes the selected picking line, removes its links to production order positions, updates the planned/missing quantities of the demand list and saves the changes.")]
        public void DeletePickingPos()
        {
            if (!IsEnabledDeletePickingPos()) return;
            VD.PickingPos pickingPos = SelectedPickingPos;
            PickingPosList.Remove(pickingPos);
            SelectedPreparedMaterial.PickingPosQuantityUOM -= pickingPos.PickingQuantityUOM ?? 0;
            SelectedPreparedMaterial.MissingQuantityUOM = SelectedPreparedMaterial.TargetQuantityUOM - SelectedPreparedMaterial.PickingPosQuantityUOM;
            _PreparedMaterialList = PreparedMaterialList.ToList();
            OnPropertyChanged(nameof(PreparedMaterialList));
            var pickingPosPositions = pickingPos.PickingPosProdOrderPartslistPos_PickingPos.ToList();
            foreach (var pickingPosPos in pickingPosPositions)
                pickingPosPos.DeleteACObject(DatabaseApp, false);
            pickingPos.DeleteACObject(DatabaseApp, false);
            OnPropertyChanged(nameof(PickingPosList));
            SelectedPickingPos = PickingPosList.FirstOrDefault();
            ACSaveChanges();
        }

        public bool IsEnabledDeletePickingPos()
        {
            return SelectedPickingPos != null && SelectedPreparedMaterial != null; ;
        }

        [ACMethodInteraction(nameof(ShowPicking), "en{'Show picking order'}de{'Kommissionierauftrag anzeigen'}", 804, true, nameof(SelectedPicking), Description = "Opens the order dialog of the selected picking order.")]
        public void ShowPicking()
        {
            if (!IsEnabledShowPicking())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(Picking), SelectedPicking.PickingID));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledShowPicking()
        {
            return SelectedPicking != null;
        }

        #endregion

        #region Methods -> CleanUp

        public void CleanUp()
        {
            _SelectedPreparedMaterial = null;
            _PreparedMaterialList = null;

            _FilterTargetStorageBinList = null;
            _SelectedFilterTargetStorageBin = null;

            _SelectedSourceStorageBin = null;
            _SourceStorageBinList = null;

            _SelectedTargetStorageBin = null;
            _TargetStorageBinList = null;

            _SelectedPicking = null;
            _PickingList = null;

            _SelectedPickingPos = null;
            _PickingPosList = null;

            _SelectedInOrder = null;
            _InOrderList = null;

            _SelectedInOrderPos = null;
            _InOrderPosList = null;

            _SelectedProdOrderPartslist = null;
            _ProdOrderPartslistList = null;
        }

        #endregion

        #region Search
        [ACMethodCommand(nameof(SearchStockMaterial), "en{'Calculate demands'}de{'Bedarfsliste ermitteln'}", (short)805, true, Description = "Raises the OnSearchStockMaterial event so that the hosting application calculates the demand list (e.g. via DoSearchStockMaterial and LoadMaterialPreparationResult). Enabled only if a listener is attached to the event.")]
        public void SearchStockMaterial()
        {
            if (!IsEnabledSearchStockMaterial())
                return;
            OnSearchStockMaterial(this, new EventArgs());
        }

        public bool IsEnabledSearchStockMaterial()
        {
            return OnSearchStockMaterial != null;
        }
        #endregion

        #region Production-Order
        [ACMethodCommand(nameof(GenerateProductionOrder), "en{'Create production order'}de{'Produktionsauftrag erstellen'}", 810, true, Description = "Creates a new production order with the ProdOrderPartslistQuantityUOM as order size, using the first enabled BOM of the selected material. Updates the planned/missing quantities of the demand list and selects the new production order. Shows an error if the material has no enabled BOM.")]
        public void GenerateProductionOrder()
        {
            Partslist partsList = SelectedPreparedMaterial.Material.Partslist_Material.Where(c => c.IsEnabled || (c.IsInEnabledPeriod != null && c.IsInEnabledPeriod.Value)).FirstOrDefault();
            if (partsList == null)
            {
                Messages.ErrorAsync(this, "Error50657");
                return;
            }

            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(ProdOrder), ProdOrder.NoColumnName, ProdOrder.FormatNewNo, this);
            ProdOrder prodOrder = ProdOrder.NewACObject(DatabaseApp, null, secondaryKey);

            ProdOrderPartslist prodOrderPartslist;
            Msg msg = ProdOrderManager.PartslistAdd(DatabaseApp, prodOrder, partsList, 1, ProdOrderPartslistQuantityUOM, out prodOrderPartslist);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }
            ACSaveChanges();
            ProdOrderPartslistList.Add(prodOrderPartslist);

            // Update SelectedPreparedMaterial
            SelectedPreparedMaterial.PickingPosQuantityUOM += prodOrderPartslist.TargetQuantity;
            SelectedPreparedMaterial.MissingQuantityUOM = ProdOrderPartslistQuantityUOM - SelectedPreparedMaterial.PickingPosQuantityUOM;
            _PreparedMaterialList = _PreparedMaterialList.ToList();

            // Update SelectedTargetStorageBin
            _SourceStorageBinList = _SourceStorageBinList.ToList();
            _TargetStorageBinList = _TargetStorageBinList.ToList();

            OnPropertyChanged(nameof(PreparedMaterialList));
            OnPropertyChanged(nameof(SourceStorageBinList));
            OnPropertyChanged(nameof(TargetStorageBinList));

            OnPropertyChanged(nameof(ProdOrderPartslistList));
            SelectedProdOrderPartslist = prodOrderPartslist;

            Messages.InfoAsync(this, "Info50104");

        }

        public bool IsEnabledGenerateProductionOrder()
        {
            return SelectedPreparedMaterial != null && SelectedPreparedMaterial.Material != null
                && ProdOrderPartslistQuantityUOM > double.Epsilon && SelectedPreparedMaterial.Material.Partslist_Material.Any();
        }

        [ACMethodCommand(nameof(DeleteProductionOrder), "en{'Delete production order'}de{'Lösche Produktionsauftrag'}", (short)811, true, Description = "Removes the selected production order from its ProdOrder and updates the planned/missing quantities of the demand list. Enabled only while the production order is in state \"NewCreated\".")]
        public void DeleteProductionOrder()
        {
            if (!IsEnabledDeleteProductionOrder())
                return;
            ProdOrderPartslist prodOrderPartslist = SelectedProdOrderPartslist;
            Msg msg = ProdOrderManager.PartslistRemove(DatabaseApp, prodOrderPartslist.ProdOrder, prodOrderPartslist);
            if (msg == null || msg.IsSucceded())
            {
                if (SelectedPreparedMaterial != null)
                {
                    SelectedPreparedMaterial.PickingPosQuantityUOM -= prodOrderPartslist.TargetQuantity;
                    SelectedPreparedMaterial.MissingQuantityUOM = SelectedPreparedMaterial.TargetQuantityUOM - SelectedPreparedMaterial.PickingPosQuantityUOM;
                }
                ProdOrderPartslistList.Remove(prodOrderPartslist);
                SelectedProdOrderPartslist = ProdOrderPartslistList.FirstOrDefault();
                OnPropertyChanged(nameof(SelectedProdOrderPartslist));
            }
            else
            {
                Messages.MsgAsync(msg);
            }
        }

        public bool IsEnabledDeleteProductionOrder()
        {
            return SelectedProdOrderPartslist != null && SelectedProdOrderPartslist.MDProdOrderState.ProdOrderState == MDProdOrderState.ProdOrderStates.NewCreated;
        }


        [ACMethodInteraction(nameof(ShowProductionOrder), "en{'Show production order'}de{'Produktionsauftrag anzeigen'}", 812, true, nameof(SelectedProdOrderPartslist), Description = "Opens the order dialog of the selected production order.")]
        public void ShowProductionOrder()
        {
            if (!IsEnabledProductionOrder())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(ProdOrderPartslist), SelectedProdOrderPartslist.ProdOrderPartslistID));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledProductionOrder()
        {
            return SelectedProdOrderPartslist != null;
        }
        #endregion

        #region InOrder
        [ACMethodCommand(nameof(NewInOrder), "en{'New purchase order'}de{'Neue Bestellung'}", (short)820, true, Description = "Opens the dialog of the child BSO BSOInOrder_Child to create a new purchase order for the selected material with the InOrderQuantityUOM as quantity, and adds the created purchase order to InOrderList.")]
        public async Task NewInOrder()
        {
            if (!IsEnabledNewInOrder())
                return;

            if (!ACSaveChanges())
                return;
            ACComponent childBSO = ACUrlCommand("?BSOInOrder_Child") as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent("BSOInOrder_Child", null, new object[] { }) as ACComponent;
            if (childBSO == null) return;
            var dlgResultAsync = childBSO.ACUrlCommand("!ShowDialogNewInOrder", SelectedPreparedMaterial.Material, InOrderQuantityUOM) as Task<VBDialogResult>;
            VBDialogResult dlgResult = await dlgResultAsync;
            if (dlgResult != null && dlgResult.SelectedCommand == eMsgButton.OK)
            {
                InOrder inOrder = dlgResult.ReturnValue as InOrder;
                if (inOrder != null)
                {
                    InOrderList.Add(inOrder);
                    OnPropertyChanged(nameof(InOrderList));
                    SelectedInOrder = inOrder;
                }
            }
            if (childBSO != null)
                await childBSO.Stop();
        }

        public bool IsEnabledNewInOrder()
        {
            return SelectedPreparedMaterial != null && SelectedPreparedMaterial.Material != null && InOrderQuantityUOM > double.Epsilon;
        }

        [ACMethodCommand(nameof(DeleteInOrder), "en{'Delete purchase order'}de{'Lösche Bestellung'}", (short)821, true, Description = "Deletes the selected purchase order from the database and removes it from InOrderList. Enabled only while the purchase order is in state \"Created\".")]
        public void DeleteInOrder()
        {
            if (!IsEnabledDeleteInOrder())
                return;
            InOrder inOrder = SelectedInOrder;
            InOrderList.Remove(SelectedInOrder);
            Msg msg = inOrder.DeleteACObject(DatabaseApp, true);
            if (msg == null || msg.IsSucceded())
            {
                SelectedInOrder = InOrderList.FirstOrDefault();
                OnPropertyChanged(nameof(InOrderList));
            }
            else
            {
                Messages.MsgAsync(msg);
            }
        }

        public bool IsEnabledDeleteInOrder()
        {
            return SelectedInOrder != null && SelectedInOrder.MDInOrderState.InOrderState == MDInOrderState.InOrderStates.Created;
        }

        [ACMethodCommand(nameof(NewInOrderPos), "en{'Take over into purchase order'}de{'Übernehme in Bestellung'}", (short)822, true, Description = "Takes the InOrderQuantityUOM over into the selected purchase order: creates an order line with the selected material, updates the planned/missing quantities of the demand list and refreshes the lists.")]
        public void NewInOrderPos()
        {
            VD.InOrderPos inOrderPos = VD.InOrderPos.NewACObject(DatabaseApp, SelectedInOrder);
            inOrderPos.TargetQuantity = InOrderQuantityUOM;
            VD.Material material = DatabaseApp.Material.FirstOrDefault(c => c.MaterialNo == SelectedPreparedMaterial.MaterialNo);
            inOrderPos.Material = material;
            InOrderPosList.Add(inOrderPos);

            // Update SelectedPreparedMaterial
            SelectedPreparedMaterial.PickingPosQuantityUOM += inOrderPos.TargetQuantityUOM;
            SelectedPreparedMaterial.MissingQuantityUOM = SelectedPreparedMaterial.TargetQuantityUOM - SelectedPreparedMaterial.PickingPosQuantityUOM;
            _PreparedMaterialList = _PreparedMaterialList.ToList();

            // Update SelectedTargetStorageBin
            _SourceStorageBinList = _SourceStorageBinList.ToList();
            _TargetStorageBinList = _TargetStorageBinList.ToList();

            OnPropertyChanged(nameof(PreparedMaterialList));
            OnPropertyChanged(nameof(SourceStorageBinList));
            OnPropertyChanged(nameof(TargetStorageBinList));

            OnPropertyChanged(nameof(InOrderPosList));
        }

        public bool IsEnabledNewInOrderPos()
        {
            return
                SelectedInOrder != null
                && InOrderQuantityUOM > 0
                && SelectedPreparedMaterial != null
                && SelectedPreparedMaterial.Material != null;
        }

        [ACMethodCommand(nameof(DeleteInOrderPos), "en{'Delete order line'}de{'Lösche Bestellposition'}", (short)823, true, Description = "Deletes the selected purchase order line, updates the planned/missing quantities of the demand list and saves the changes.")]
        public void DeleteInOrderPos()
        {
            if (!IsEnabledDeleteInOrderPos()) return;
            VD.InOrderPos inOrderPos = SelectedInOrderPos;
            InOrderPosList.Remove(inOrderPos);
            if (SelectedPreparedMaterial != null)
            {
                SelectedPreparedMaterial.PickingPosQuantityUOM -= inOrderPos.TargetQuantityUOM;
                SelectedPreparedMaterial.MissingQuantityUOM = SelectedPreparedMaterial.TargetQuantityUOM - SelectedPreparedMaterial.PickingPosQuantityUOM;
            }
            _PreparedMaterialList = PreparedMaterialList.ToList();
            OnPropertyChanged(nameof(PreparedMaterialList));
            inOrderPos.DeleteACObject(DatabaseApp, false);
            OnPropertyChanged(nameof(InOrderPosList));
            SelectedInOrderPos = InOrderPosList.FirstOrDefault();
            ACSaveChanges();
        }

        public bool IsEnabledDeleteInOrderPos()
        {
            return SelectedInOrderPos != null && SelectedPreparedMaterial != null;
        }

        [ACMethodInteraction(nameof(ShowInOrder), "en{'Show purchase order'}de{'Bestellung anzeigen'}", 824, true, nameof(SelectedInOrder), Description = "Opens the order dialog of the selected purchase order.")]
        public void ShowInOrder()
        {
            if (!IsEnabledShowInOrder())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(InOrder), SelectedInOrder.InOrderID));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledShowInOrder()
        {
            return SelectedInOrder != null;
        }

        #endregion

        #endregion

        #region BackgroundWorker -> DoMehtods


        #region BackgroundWorker -> DoMehtods -> SearchStockMaterial

        public List<MaterialPreparationModel> DoSearchStockMaterial(List<ProdOrderBatchPlan> selectedBatchPlans)
        {
            ACRoutingParameters routingParameters = new ACRoutingParameters()
            {
                RoutingService = RoutingService,
                Database = DatabaseApp.ContextIPlus,
                AttachRouteItemsToContext = true,
                SelectionRuleID = "Storage",
                Direction = RouteDirections.Backwards,
                DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule,
                MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                IncludeReserved = true,
                IncludeAllocated = true,
                ResultMode = RouteResultMode.ShortRoute
            };
            MaterialPreparationResult materialPreparationResult = ACFacilityManager.GetMaterialPreparationModel1(DatabaseApp.ContextIPlus, DatabaseApp, MediaController, ConfigManager, routingParameters, selectedBatchPlans);

#if DEBUG
            var materialOnrouteFacilities =
                materialPreparationResult
                .PreparedMaterials
                .Select(c =>
                    new
                    {
                        c.MaterialNo,
                        c.MaterialName,
                        c.OnRouteFacilityNos
                    }
                )
                .ToArray();

            var allowedMachineFacilities =
                materialPreparationResult
                .AllowedInstances
                .Select(c =>
                    new
                    {
                        c.Machine.ACIdentifier,
                        ConnectedFacilities = c.ConnectedFacilities.Select(x => x.FacilityNo).ToArray()
                    }
                )
                .ToArray();
#endif

            return materialPreparationResult.PreparedMaterials;
        }

        #endregion

        #endregion

        #region Loading Methods

        public void LoadMaterialPreparationResult(List<MaterialPreparationModel> preparedMaterials)
        {
            CleanUp();
            _PreparedMaterialList = preparedMaterials;
            OnPropertyChanged(nameof(PreparedMaterialList));
            SelectedPreparedMaterial = _PreparedMaterialList.FirstOrDefault();

            _FilterTargetStorageBinList = LoadFilterTargetStorageBinList(preparedMaterials);
            OnPropertyChanged(nameof(FilterTargetStorageBinList));
        }

        private void LoadPreparedMaterial(MaterialPreparationModel preparedMaterial)
        {
            if (preparedMaterial == null)
            {
                _TargetStorageBinList = new List<PlanningTargetStockPreview>();
            }
            else
            {
                _TargetStorageBinList = ACFacilityManager.LoadTargetStorageBins(DatabaseApp, preparedMaterial);
                if (_TargetStorageBinList != null && _TargetStorageBinList.Any())
                {
                    SelectedTargetStorageBin = _TargetStorageBinList.Where(c => c.MDPickingType != null).FirstOrDefault();
                    if (SelectedTargetStorageBin == null)
                    {
                        SelectedTargetStorageBin = _TargetStorageBinList.FirstOrDefault();
                    }
                }

                string[] targetsNotShownAsSource = TargetStorageBinList.Where(c => c.MDPickingType != null).Select(c => c.FacilityNo).ToArray();
                _SourceStorageBinList = ACFacilityManager.LoadSourceStorageBins(DatabaseApp, preparedMaterial, targetsNotShownAsSource);
                if (_SourceStorageBinList != null && _SourceStorageBinList.Any())
                {
                    SelectedSourceStorageBin = _SourceStorageBinList.OrderByDescending(c => c.SumTotal).FirstOrDefault();
                }

                InOrderQuantityUOM = preparedMaterial.MissingQuantityUOM.HasValue ? preparedMaterial.MissingQuantityUOM.Value : preparedMaterial.TargetQuantityUOM;
                ProdOrderPartslistQuantityUOM = preparedMaterial.MissingQuantityUOM.HasValue ? preparedMaterial.MissingQuantityUOM.Value : preparedMaterial.TargetQuantityUOM;
            }
            LoadInOrder();
            LoadProdOrderPartslist();

            OnPropertyChanged(nameof(SourceStorageBinList));
            OnPropertyChanged(nameof(TargetStorageBinList));
        }

        #region Pickings
        private void LoadPickings(MDPickingType mdPickingType)
        {
            SelectedPicking = null;
            _PickingList = null;
            if (mdPickingType != null)
            {
                _PickingList = DatabaseApp
                               .Picking
                               .Where(c => c.PickingStateIndex < (short)PickingStateEnum.Finished && c.MDPickingTypeID == mdPickingType.MDPickingTypeID)
                               .OrderByDescending(c => c.InsertDate)
                               .ToList();
                _SelectedPicking = _PickingList.FirstOrDefault();
            }
            OnPropertyChanged(nameof(SelectedPicking));
            OnPropertyChanged(nameof(PickingList));
            LoadPickingPositions();
        }

        private void LoadPickingPositions()
        {
            SelectedPickingPos = null;
            _PickingPosList = null;
            Guid? pickingID = null;
            Guid materialID = Guid.Empty;
            if (SelectedPreparedMaterial != null)
                materialID = SelectedPreparedMaterial.Material.MaterialID;
            if (SelectedPicking != null)
                pickingID = SelectedPicking.PickingID;
            _PickingPosList = DatabaseApp
               .PickingPos
               .Where(c =>
                    c.Picking.PickingStateIndex < (short)PickingStateEnum.Finished
                    && c.PickingMaterialID == materialID
                    && (ShowAllPickingLines || (c.PickingID == (pickingID ?? Guid.Empty)))
                )
               .OrderByDescending(c => c.InsertDate)
               .ToList();
            SelectedPickingPos = _PickingPosList.FirstOrDefault();
            OnPropertyChanged(nameof(PickingPosList));
        }
        #endregion

        #region InOrder
        private void LoadInOrder()
        {
            SelectedInOrder = null;
            _InOrderList = null;
            if (SelectedPreparedMaterial != null)
            {
                Guid materialID = SelectedPreparedMaterial.Material.MaterialID;
                _InOrderList = DatabaseApp
                                .InOrderPos.Where(c => c.InOrder.MDInOrderState.MDInOrderStateIndex <= (short)MDInOrderState.InOrderStates.InProcess
                                                    && c.MaterialID == materialID)
                                .Select(c => c.InOrder)
                                .OrderByDescending(c => c.InsertDate)
                                .ToList();
            }
            else
            {
                _InOrderList = new List<InOrder>();
            }
            _SelectedInOrder = _InOrderList.FirstOrDefault();
            OnPropertyChanged(nameof(SelectedInOrder));
            OnPropertyChanged(nameof(InOrderList));
            LoadInOrderPositions();
        }

        private void LoadInOrderPositions()
        {
            SelectedInOrderPos = null;
            _InOrderPosList = null;
            if (SelectedInOrder != null)
            {
                Guid inOrderID = SelectedInOrder.InOrderID;
                if (SelectedPreparedMaterial != null)
                {
                    Guid materialID = SelectedPreparedMaterial.Material.MaterialID;
                    _InOrderPosList = DatabaseApp
                       .InOrderPos
                       .Where(c => c.InOrderID == inOrderID
                                && (ShowAllInOrderLines || c.MaterialID == materialID)
                        )
                       .OrderByDescending(c => c.Sequence)
                       .ToList();
                }
                else
                {
                    _InOrderPosList = DatabaseApp
                       .InOrderPos
                       .Where(c => c.InOrderID == inOrderID)
                       .OrderByDescending(c => c.Sequence)
                       .ToList();

                }
            }
            else
            {
                _InOrderPosList = new List<InOrderPos>();
            }
            SelectedInOrderPos = _InOrderPosList.FirstOrDefault();
            OnPropertyChanged(nameof(InOrderPosList));
        }
        #endregion


        #region ProdOrderPartslist
        private void LoadProdOrderPartslist()
        {
            SelectedProdOrderPartslist = null;
            _ProdOrderPartslistList = null;
            if (SelectedPreparedMaterial != null)
            {
                Guid materialID = SelectedPreparedMaterial.Material.MaterialID;
                _ProdOrderPartslistList = DatabaseApp
                            .ProdOrderPartslist
                            .Where(c => c.MDProdOrderState.MDProdOrderStateIndex <= (short)MDProdOrderState.ProdOrderStates.InProduction
                                        && c.Partslist.MaterialID == materialID)
                            .OrderByDescending(c => c.InsertDate)
                            .ToList();
            }
            else
            {
                _ProdOrderPartslistList = new List<ProdOrderPartslist>();
            }
            _SelectedProdOrderPartslist = _ProdOrderPartslistList.FirstOrDefault();
            OnPropertyChanged(nameof(SelectedProdOrderPartslist));
            OnPropertyChanged(nameof(ProdOrderPartslistList));
        }

        #endregion

        #endregion

        #region Private 
        private List<ACValueItem> LoadFilterTargetStorageBinList(List<MaterialPreparationModel> preparedMaterials)
        {
            List<ACValueItem> list = new List<ACValueItem>();

            ACValueItem allItems = new ACValueItem("en{'All storage locations'}de{'Alle Lagerplätze'}", null, null);
            list.Add(allItems);

            List<string> facilityNos = new List<string>();
            foreach (MaterialPreparationModel preparedMaterial in preparedMaterials)
            {
                foreach (var item in preparedMaterial.FacilityScheduligGroups)
                {
                    if (!facilityNos.Contains(item.Facility.FacilityNo))
                    {
                        facilityNos.Add(item.Facility.FacilityNo);
                    }
                }
            }
            facilityNos.Sort();

            foreach (string facilityNo in facilityNos)
            {
                ACValueItem previewItem = new ACValueItem($"en{{'{facilityNo}'}}de{{'{facilityNo}'}}", facilityNo, null);
                list.Add(previewItem);
            }

            return list;
        }
        #endregion


    }
}
