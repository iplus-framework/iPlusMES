using gip.core.datamodel;
using VD = gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Linq;
using System.Collections.Generic;
using gip.mes.datamodel;
using gip.mes.autocomponent;
using gip.core.autocomponent;
using gip.core.media;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Material Preparation'}de{'Bereitstellung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
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

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            var b = base.ACDeInit(deleteACClassTask);

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
                    NewInOrder();
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
        #endregion

        #region Managers

        /// <summary>
        /// The _ facility manager
        /// </summary>
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

        // PreparedMaterial (PreparedMaterial)

        #region PreparedMaterial
        private MaterialPreparationModel _SelectedPreparedMaterial;
        /// <summary>
        /// Selected property for PreparedMaterial
        /// </summary>
        /// <value>The selected PreparedMaterial</value>
        [ACPropertySelected(500, "PreparedMaterial", "en{'TODO: PreparedMaterial'}de{'TODO: PreparedMaterial'}")]
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
        /// <summary>
        /// List property for PreparedMaterial
        /// </summary>
        /// <value>The PreparedMaterial list</value>
        [ACPropertyList(501, "PreparedMaterial")]
        public List<MaterialPreparationModel> PreparedMaterialList
        {
            get
            {
                if (_PreparedMaterialList == null)
                    _PreparedMaterialList = new List<MaterialPreparationModel>();
                return _PreparedMaterialList;
            }
        }


        #endregion

        // SourceStorageBin (FacilityChargeSumFacilityHelper)

        #region SourceStorageBin
        private FacilityChargeSumFacilityHelper _SelectedSourceStorageBin;
        /// <summary>
        /// Selected property for FacilityChargeSumFacilityHelper
        /// </summary>
        /// <value>The selected SourceStorageBin</value>
        [ACPropertySelected(502, "SourceStorageBin", "en{'TODO: SourceStorageBin'}de{'TODO: SourceStorageBin'}")]
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
        /// <summary>
        /// List property for FacilityChargeSumFacilityHelper
        /// </summary>
        /// <value>The SourceStorageBin list</value>
        [ACPropertyList(503, "SourceStorageBin")]
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

        // TargetStorageBin (FacilityChargeSumFacilityHelper)

        #region TargetStorageBin
        private PlanningTargetStockPreview _SelectedTargetStorageBin;
        /// <summary>
        /// Selected property for FacilityChargeSumFacilityHelper
        /// </summary>
        /// <value>The selected TargetStorageBin</value>
        [ACPropertySelected(504, "TargetStorageBin", "en{'Target store'}de{'Ziellagerplatz'}")]
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
        /// <summary>
        /// List property for FacilityChargeSumFacilityHelper
        /// </summary>
        /// <value>The TargetStorageBin list</value>
        [ACPropertyList(505, "TargetStorageBin")]
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

        // Picking

        #region Picking
        private VD.Picking _SelectedPicking;
        /// <summary>
        /// Selected property for Picking
        /// </summary>
        /// <value>The selected Picking</value>
        [ACPropertySelected(506, "Picking", "en{'TODO: Picking'}de{'TODO: Picking'}")]
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
        /// <summary>
        /// List property for Picking
        /// </summary>
        /// <value>The Picking list</value>
        [ACPropertyList(507, "Picking")]
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

        // PickingPos
        #region PickingPos
        private VD.PickingPos _SelectedPickingPos;
        /// <summary>
        /// Selected property for PickingPos
        /// </summary>
        /// <value>The selected PickingPos</value>
        [ACPropertySelected(508, "PickingPos", "en{'TODO: PickingPos'}de{'TODO: PickingPos'}")]
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
        /// <summary>
        /// List property for PickingPos
        /// </summary>
        /// <value>The PickingPos list</value>
        [ACPropertyList(509, "PickingPos")]
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
        [ACPropertyInfo(510, "PickingQuantityUOM", "en{'Picking quantity'}de{'Komissioniermenge'}")]
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
        /// <summary>
        /// Doc  ShowAllPickingLines
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(511, "ShowAllPickingLines", "en{'Show of all picking orders'}de{'Von allen Kommisionieraufträgen anzeigen'}")]
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
        [ACPropertySelected(526, "InOrder", "en{'Purchase Order'}de{'Bestellung'}")]
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
        [ACPropertyList(527, "InOrder")]
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
        [ACPropertySelected(528, "InOrderPos", "en{'Orde line'}de{'Bestellpositionen'}")]
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
        [ACPropertyList(529, "InOrderPos")]
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
        [ACPropertyInfo(511, "ShowAllInOrderLines", "en{'Show of all Purchase orders'}de{'Von allen Bestellungen anzeigen'}")]
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
        [ACPropertyInfo(530, "InOrderQuantityUOM", "en{'Purchase quantity'}de{'Bestellmenge'}")]
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
        [ACPropertySelected(506, "ProdOrderPartslist", "en{'Purchase Order'}de{'Bestellung'}")]
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
        [ACPropertyList(507, "ProdOrderPartslist")]
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
        [ACPropertyInfo(510, "ProdOrderPartslistQuantityUOM", "en{'Production order size'}de{'Auftragsgröße'}")]
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
        [ACMethodCommand("SelectedPicking", "en{'New picking order'}de{'Neuer Kommissionierauftrag'}", (short)800, true)]
        public void NewPicking()
        {
            if (!IsEnabledNewPicking()) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(Picking), Picking.NoColumnName, Picking.FormatNewNo, this);
            var picking = Picking.NewACObject(DatabaseApp, null, secondaryKey);
            picking.MDPickingType = SelectedTargetStorageBin.MDPickingType;
            DatabaseApp.Picking.AddObject(picking);
            ACSaveChanges();
            PickingList.Add(picking);
            OnPropertyChanged(nameof(PickingList));
            SelectedPicking = picking;
        }
        
        public bool IsEnabledNewPicking()
        {
            return SelectedTargetStorageBin != null && SelectedTargetStorageBin.MDPickingType != null;
        }

        [ACMethodCommand("SelectedPicking", "en{'Delete picking order'}de{'Lösche Kommissionierauftrag'}", (short)801, true)]
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
                Messages.Msg(msg);
            }
        }

        public bool IsEnabledDeletePicking()
        {
            return SelectedPicking != null;
        }

        [ACMethodCommand("SelectedPickingPos", "en{'Take over into picking order'}de{'Übernehme in Kommissionierauftrag'}", (short)802, true)]
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
                List<ProdOrderPartslistPos> relatedPositions = DatabaseApp.ProdOrderPartslistPos.Where(c => SelectedPreparedMaterial.RelatedIDs.Contains(c.ProdOrderPartslistPosID)).ToList();
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

        [ACMethodCommand("SelectedPickingPos", "en{'Delete picking line'}de{'Lösche Kommissionierposition'}", (short)803, true)]
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

        [ACMethodInteraction("", "en{'Show picking order'}de{'Kommissionierauftrag anzeigen'}", 804, true, nameof(SelectedPicking))]
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
        [ACMethodCommand("NewPicking", "en{'Calculate demands'}de{'Bedarfsliste ermitteln'}", (short)805, true)]
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
        [ACMethodCommand("SelectedProdOrderPartslist", "en{'Create production order'}de{'Produktionsauftrag erstellen'}", 810, true)]
        public void GenerateProductionOrder()
        {
            Partslist partsList = SelectedPreparedMaterial.Material.Partslist_Material.Where(c => c.IsEnabled || (c.IsInEnabledPeriod != null && c.IsInEnabledPeriod.Value)).FirstOrDefault();
            if (partsList == null)
            {
                Messages.Error(this, "Error50657");
                return;
            }

            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(ProdOrder), ProdOrder.NoColumnName, ProdOrder.FormatNewNo, this);
            ProdOrder prodOrder = ProdOrder.NewACObject(DatabaseApp, null, secondaryKey);

            ProdOrderPartslist prodOrderPartslist;
            Msg msg = ProdOrderManager.PartslistAdd(DatabaseApp, prodOrder, partsList, 1, ProdOrderPartslistQuantityUOM, out prodOrderPartslist);
            if (msg != null)
            {
                Messages.Msg(msg);
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

            Messages.Info(this, "Info50104");

        }

        public bool IsEnabledGenerateProductionOrder()
        {
            return SelectedPreparedMaterial != null && SelectedPreparedMaterial.Material != null 
                && ProdOrderPartslistQuantityUOM > double.Epsilon && SelectedPreparedMaterial.Material.Partslist_Material.Any();
        }

        [ACMethodCommand("SelectedProdOrderPartslist", "en{'Delete production order'}de{'Lösche Produktionsauftrag'}", (short)811, true)]
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
                Messages.Msg(msg);
            }
        }

        public bool IsEnabledDeleteProductionOrder()
        {
            return SelectedProdOrderPartslist != null && SelectedProdOrderPartslist.MDProdOrderState.ProdOrderState == MDProdOrderState.ProdOrderStates.NewCreated;
        }


        [ACMethodInteraction("SelectedProdOrderPartslist", "en{'Show production order'}de{'Produktionsauftrag anzeigen'}", 812, true, nameof(SelectedProdOrderPartslist))]
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
        [ACMethodCommand("SelectedInOrder", "en{'New purchase order'}de{'Neue Bestellung'}", (short)820, true)]
        public void NewInOrder()
        {
            if (!IsEnabledNewInOrder()) 
                return;
            //string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(InOrder), InOrder.NoColumnName, InOrder.FormatNewNo, this);
            //var inOrder = InOrder.NewACObject(DatabaseApp, null, secondaryKey);
            //DatabaseApp.InOrder.AddObject(inOrder);
            //ACSaveChanges();

            if (!ACSaveChanges())
                return;
            ACComponent childBSO = ACUrlCommand("?BSOInOrder_Child") as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent("BSOInOrder_Child", null, new object[] { }) as ACComponent;
            if (childBSO == null) return;
            VBDialogResult dlgResult = (VBDialogResult)childBSO.ACUrlCommand("!ShowDialogNewInOrder", SelectedPreparedMaterial.Material, InOrderQuantityUOM);
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
                childBSO.Stop();
        }

        public bool IsEnabledNewInOrder()
        {
            return SelectedPreparedMaterial != null && SelectedPreparedMaterial.Material != null && InOrderQuantityUOM > double.Epsilon;
        }

        [ACMethodCommand("SelectedInOrder", "en{'Delete purchase order'}de{'Lösche Bestellung'}", (short)821, true)]
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
                Messages.Msg(msg);
            }
        }
        
        public bool IsEnabledDeleteInOrder()
        {
            return SelectedInOrder != null && SelectedInOrder.MDInOrderState.InOrderState == MDInOrderState.InOrderStates.Created;
        }

        [ACMethodCommand("SelectedInOrderPos", "en{'Take over into purchase order'}de{'Übernehme in Bestellung'}", (short)822, true)]
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

        [ACMethodCommand("SelectedInOrderPos", "en{'Delete order lin'}de{'Lösche Bestellposition'}", (short)823, true)]
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

        [ACMethodInteraction("", "en{'Show purchase order'}de{'Bestellung anzeigen'}", 824, true, nameof(SelectedInOrder))]
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
            MaterialPreparationResult materialPreparationResult =
                ACFacilityManager.GetMaterialPreparationModel(DatabaseApp.ContextIPlus, DatabaseApp, MediaController, ConfigManager, routingParameters, selectedBatchPlans);
            return materialPreparationResult.PreparedMaterials;
        }


        //private void GetPositionsForBatchMaterialModel(List<SearchBatchMaterialModel> searchResult, ProdOrderBatchPlan batchPlan, ProdOrderPartslistPos prodOrderPartslistPos, double posTargetQuantityUOM)
        //{
        //    foreach (ProdOrderPartslistPosRelation prodOrderPartslistPosRelation in prodOrderPartslistPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
        //    {
        //        if (prodOrderPartslistPosRelation.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.OutwardRoot)
        //        {
        //            SearchBatchMaterialModel searchBatchMaterialModel = GetRelationForBatchMaterialModel(batchPlan, prodOrderPartslistPosRelation, posTargetQuantityUOM);
        //            searchResult.Add(searchBatchMaterialModel);
        //        }
        //        else
        //        {
        //            double factor = prodOrderPartslistPosRelation.TargetQuantityUOM / posTargetQuantityUOM;
        //            double subPosTargetQuantity = posTargetQuantityUOM * factor;
        //            GetPositionsForBatchMaterialModel(searchResult, batchPlan, prodOrderPartslistPosRelation.SourceProdOrderPartslistPos, subPosTargetQuantity);
        //        }
        //    }
        //}

        //private SearchBatchMaterialModel GetRelationForBatchMaterialModel(ProdOrderBatchPlan batchPlan, ProdOrderPartslistPosRelation prodOrderPartslistPosRelation, double posTargetQuantityUOM)
        //{
        //    SearchBatchMaterialModel searchBatchMaterialModel = new SearchBatchMaterialModel();
        //    searchBatchMaterialModel.MaterialID = prodOrderPartslistPosRelation.SourceProdOrderPartslistPos.Material.MaterialID;
        //    searchBatchMaterialModel.ProdOrderBatchPlanID = batchPlan.ProdOrderBatchPlanID;
        //    searchBatchMaterialModel.SourcePosID = prodOrderPartslistPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslistPosID;
        //    searchBatchMaterialModel.TargetQuantityUOM = prodOrderPartslistPosRelation.SourceProdOrderPartslistPos.TargetQuantityUOM * (batchPlan.TotalSize / batchPlan.ProdOrderPartslist.TargetQuantity);

        //    if (batchPlan.VBiACClassWF != null && batchPlan.VBiACClassWF.MDSchedulingGroupWF_VBiACClassWF.Any())
        //    {
        //        searchBatchMaterialModel.MDSchedulingGroupID = batchPlan.VBiACClassWF.MDSchedulingGroupWF_VBiACClassWF.Select(c => c.MDSchedulingGroupID).FirstOrDefault();
        //    }
        //    //if (posTargetQuantityUOM  > Double.Epsilon)
        //    //{
        //    //    double factor = prodOrderPartslistPosRelation.TargetQuantityUOM / posTargetQuantityUOM;
        //    //    searchFacilityModel.TargetQuantityUOM += batchPlan.BatchSize * batchPlan.BatchTargetCount * factor;
        //    //}
        //    return searchBatchMaterialModel;
        //}

        #endregion

        #endregion

        #region Loading Methods

        public void LoadMaterialPlanFromPos(List<MaterialPreparationModel> preparedMaterials)
        {
            CleanUp();

            _PreparedMaterialList = preparedMaterials;
            OnPropertyChanged(nameof(PreparedMaterialList));
            SelectedPreparedMaterial = _PreparedMaterialList.FirstOrDefault();
        }

        //public List<PreparedMaterial> GetPreparedMaterials(List<SearchBatchMaterialModel> researchedFacilities)
        //{
        //    List<PreparedMaterial> preparedMaterials = new List<PreparedMaterial>();
        //    var queryResearchedFacilities = researchedFacilities.GroupBy(c => c.MaterialID);
        //    Guid[] materialIDs = queryResearchedFacilities.Select(c => c.Key).ToArray();
        //    List<VD.Material> materials = DatabaseApp.Material.Where(c => materialIDs.Contains(c.MaterialID)).ToList();
        //    int nr = 0;
        //    foreach (var item in queryResearchedFacilities)
        //    {
        //        Guid materialID = item.Key;
        //        Material material = materials.FirstOrDefault(c => c.MaterialID == materialID);
        //        MediaController.LoadIImageInfo(material);
        //        Guid[] posIDs = item.Select(c => c.SourcePosID).Distinct().ToArray();
        //        nr++;
        //        PreparedMaterial preparedMaterial = new PreparedMaterial() { Sn = nr, PickingRelationType = PickingRelationTypeEnum.ProductionLine };
        //        preparedMaterial.Material = material;
        //        preparedMaterial.MaterialNo = material.MaterialNo;
        //        preparedMaterial.MaterialName = material.MaterialName1;

        //        preparedMaterial.DefaultThumbImage = material.DefaultThumbImage;
        //        preparedMaterial.TargetQuantityUOM = item.Sum(x => x.TargetQuantityUOM);
        //        preparedMaterial.RelatedIDs = item.Select(c => c.SourcePosID).Distinct().ToArray();

        //        double availableQuantity =
        //            DatabaseApp
        //            .FacilityCharge
        //            .Where(c =>
        //                !c.NotAvailable
        //                && c.MaterialID == materialID
        //               )
        //            .Select(c => c.StockQuantityUOM)
        //            .DefaultIfEmpty()
        //            .Sum(c => c);
        //        preparedMaterial.AvailableQuantityUOM = availableQuantity;

        //        double pickingPosQuantityUOM =
        //            DatabaseApp
        //            .Picking
        //            .Where(c => c.PickingStateIndex < (short)PickingStateEnum.Finished)
        //            .SelectMany(c => c.PickingPos_Picking)
        //            .Where(c => c.PickingMaterialID == materialID && c.PickingPosProdOrderPartslistPos_PickingPos.Any(x => posIDs.Contains(x.ProdorderPartslistPosID)))
        //            .Select(c => c.PickingQuantityUOM ?? 0)
        //            .DefaultIfEmpty()
        //            .Sum(c => c);

        //        double inOrderQuantityUOM = 
        //            DatabaseApp
        //            .InOrder
        //            .Where(c => c.MDInOrderState.MDInOrderStateIndex <= (short)MDInOrderState.InOrderStates.InProcess)
        //            .SelectMany(c => c.InOrderPos_InOrder)
        //            .Where(c => c.MaterialID == materialID)
        //            .Select(c => c.TargetQuantityUOM)
        //            .DefaultIfEmpty()
        //            .Sum(c => c);

        //        double prodOrderQuantityUOM =
        //            DatabaseApp
        //            .ProdOrderPartslist
        //            .Where(c => c.MDProdOrderState.MDProdOrderStateIndex <= (short)MDProdOrderState.ProdOrderStates.InProduction)
        //            .Where(c => c.Partslist.MaterialID == materialID)
        //            .Select(c => c.TargetQuantity)
        //            .DefaultIfEmpty()
        //            .Sum(c => c);

        //        preparedMaterial.PickingPosQuantityUOM = pickingPosQuantityUOM + inOrderQuantityUOM + prodOrderQuantityUOM;

        //        preparedMaterial.MissingQuantityUOM = preparedMaterial.TargetQuantityUOM - preparedMaterial.PickingPosQuantityUOM;

        //        preparedMaterial.MDSchedulingGroupIDs =
        //            item
        //            .Where(c => c.MDSchedulingGroupID != null)
        //            .Select(c => c.MDSchedulingGroupID ?? Guid.Empty)
        //            .Distinct()
        //            .ToArray();

        //        preparedMaterials.Add(preparedMaterial);
        //    }
        //    return preparedMaterials;
        //}

        private void LoadPreparedMaterial(MaterialPreparationModel preparedMaterial)
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
            LoadInOrder();
            LoadProdOrderPartslist();

            OnPropertyChanged(nameof(SourceStorageBinList));
            OnPropertyChanged(nameof(TargetStorageBinList));
        }

        //private List<FacilityChargeSumFacilityHelper> LoadSourceStorageBins(PreparedMaterial preparedMaterial)
        //{
        //    FacilityCharge[] facilityCharges =
        //        FacilityManager
        //        .s_cQry_MatOverviewFacilityCharge(this.DatabaseApp, preparedMaterial.Material.MaterialID, false)
        //        .Where(c => c.Facility != null && c.Facility.FacilityNo != ProdMatStorage)
        //        .ToArray();
        //    List<FacilityChargeSumFacilityHelper> list = ACFacilityManager.GetFacilityChargeSumFacilityHelperList(facilityCharges, new FacilityQueryFilter()).ToList();

        //    string[] targetsNotShownAsSource = TargetStorageBinList.Where(c => c.MDPickingType != null).Select(c => c.FacilityNo).ToArray();
        //    list = list.Where(c => !targetsNotShownAsSource.Contains(c.FacilityNo)).ToList();

        //    foreach (FacilityChargeSumFacilityHelper item in list)
        //    {
        //        item.NewPlannedStock = DatabaseApp
        //                .PickingPos
        //                .Where(c =>
        //                        c.PickingMaterialID == preparedMaterial.Material.MaterialID
        //                        && (c.Picking.PickingStateIndex < (short)PickingStateEnum.Finished)
        //                        && c.FromFacility.FacilityNo == item.FacilityNo
        //                       )
        //                .Select(c => c.PickingQuantityUOM ?? 0)
        //                .DefaultIfEmpty()
        //                .Sum();
        //    }
        //    return list;
        //}

        //private List<PlanningTargetStockPreview> LoadTargetStorageBins(PreparedMaterial preparedMaterial)
        //{
        //    List<PlanningTargetStockPreview> list = new List<PlanningTargetStockPreview>();

        //    List<FacilityMDSchedulingGroup> schGroupFacility =
        //        DatabaseApp
        //        .MDSchedulingGroup
        //        .Where(c => preparedMaterial.MDSchedulingGroupIDs.Contains(c.MDSchedulingGroupID))
        //        .SelectMany(c => c.FacilityMDSchedulingGroup_MDSchedulingGroup)
        //        .ToList();

        //    foreach (FacilityMDSchedulingGroup schGroup in schGroupFacility)
        //    {
        //        PlanningTargetStockPreview item = list.FirstOrDefault(c => c.FacilityNo == schGroup.Facility.FacilityNo);
        //        if (item == null)
        //        {
        //            item = new PlanningTargetStockPreview();
        //            item.Facility = schGroup.Facility;
        //            item.FacilityNo = schGroup.Facility.FacilityNo;
        //            item.FacilityName = schGroup.Facility.FacilityName;
        //            item.MDPickingType = schGroup.MDPickingType;
        //            if (preparedMaterial.Material.FacilityMaterial_Material.Any())
        //            {
        //                item.OptStockQuantity =
        //                    preparedMaterial
        //                    .Material
        //                    .FacilityMaterial_Material
        //                    .Where(c => c.FacilityID == schGroup.Facility.FacilityID)
        //                    .Select(c => c.OptStockQuantity)
        //                    .FirstOrDefault();
        //            }
        //            list.Add(item);
        //        }
        //    }

        //    var testConistentQuery =
        //        schGroupFacility
        //        .Where(c => c.MDPickingTypeID != null)
        //        .Select(c => new { c.Facility.FacilityNo, c.Facility.FacilityName, SchedulingGroup_MDKey = c.MDSchedulingGroup.MDKey, c.MDPickingType.MDKey })
        //        .GroupBy(c => new { c.FacilityNo, c.FacilityName, c.SchedulingGroup_MDKey });

        //    if (testConistentQuery.Any(c => c.Count() > 1))
        //    {
        //        string pickingTypes = string.Join(",", testConistentQuery.Select(c => c));
        //        Messages.Warning(this, "Warning50054", false, pickingTypes);
        //    }
        //    else
        //    {
        //        foreach (PlanningTargetStockPreview item in list)
        //        {
        //            item.ActualStockQuantity =
        //                DatabaseApp
        //                .FacilityCharge
        //                .Where(c => c.MaterialID == preparedMaterial.Material.MaterialID && !c.NotAvailable && c.Facility.FacilityNo == item.FacilityNo)
        //                .Select(c => c.StockQuantity)
        //                .DefaultIfEmpty()
        //                .Sum();

        //            item.OrderedQuantity =
        //                DatabaseApp
        //                .PickingPos
        //                .Where(c =>
        //                        c.PickingMaterialID == preparedMaterial.Material.MaterialID
        //                        && (c.Picking.PickingStateIndex < (short)PickingStateEnum.Finished)
        //                       )
        //                .AsEnumerable()
        //                .Select(c => c.TargetQuantityUOM - c.ActualQuantityUOM)
        //                .DefaultIfEmpty()
        //                .Sum();
        //        }
        //    }

        //    foreach (var item in list)
        //    {
        //        if (item.MDPickingType != null)
        //        {
        //            item.NewPlannedStockQuantity = 0;
        //            if (preparedMaterial.TargetQuantityUOM > item.ActualStockQuantity)
        //                item.NewPlannedStockQuantity = preparedMaterial.TargetQuantityUOM - item.ActualStockQuantity;
        //            if (item.OptStockQuantity != null && (item.OptStockQuantity ?? 0) > item.ActualStockQuantity)
        //                item.NewPlannedStockQuantity += (item.OptStockQuantity ?? 0) - item.ActualStockQuantity;
        //            if (item.OptStockQuantity != null)
        //            {
        //                if (item.NewPlannedStockQuantity > Const_RangeStockQuantityTolerance)
        //                {
        //                    item.IsInRange = -1;
        //                }
        //                else if (item.NewPlannedStockQuantity == 0)
        //                {
        //                    item.IsInRange = 1;
        //                }
        //            }
        //        }
        //    }

        //    return list;
        //}

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
                            .Where(c =>    c.MDProdOrderState.MDProdOrderStateIndex <= (short)MDProdOrderState.ProdOrderStates.InProduction
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

        #endregion

    }
}
