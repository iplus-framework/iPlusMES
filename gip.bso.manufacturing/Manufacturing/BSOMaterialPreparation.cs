// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
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

            _VarioConfigManager = ConfigManagerIPlus.ACRefToServiceInstance(this);
            if (_VarioConfigManager == null)
                throw new Exception("VarioConfigManager not configured");

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

            _RoutingService = ACRoutingService.ACRefToServiceInstance(this);

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            var b = base.ACDeInit(deleteACClassTask);

            ACRoutingService.DetachACRefFromServiceInstance(this, _RoutingService);
            _RoutingService = null;

            if (_ACFacilityManager != null)
                FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;

            if (_PickingManager != null)
                ACPickingManager.DetachACRefFromServiceInstance(this, _PickingManager);
            _PickingManager = null;

            CleanUp();

            MediaController = null;

            ACRoutingService.DetachACRefFromServiceInstance(this, _RoutingService);
            _RoutingService = null;

            return b;
        }

        #endregion

        #region Overrides

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "NewPicking":
                    NewPicking();
                    return true;
                case "IsEnabledNewPicking":
                    result = IsEnabledNewPicking();
                    return true;
                case "DeletePicking":
                    DeletePicking();
                    return true;
                case "IsEnabledDeletePicking":
                    result = IsEnabledDeletePicking();
                    return true;
                case "NewPickingPos":
                    NewPickingPos();
                    return true;
                case "DeletePickingPos":
                    DeletePickingPos();
                    return true;
                case "IsEnabledDeletePickingPos":
                    result = IsEnabledDeletePickingPos();
                    return true;
                case "SearchStockMaterial":
                    SearchStockMaterial();
                    return true;
                case "IsEnabledSearchStockMaterial":
                    result = IsEnabledSearchStockMaterial();
                    return true;
                case "IsEnabledNewPickingPos":
                    result = IsEnabledNewPickingPos();
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

        public ACMediaController MediaController { get;set;}

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

        protected ACRef<ConfigManagerIPlus> _VarioConfigManager = null;
        public ConfigManagerIPlus VarioConfigManager
        {
            get
            {
                if (_VarioConfigManager == null)
                    return null;
                return _VarioConfigManager.ValueT;
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
        [ACPropertySelected(504, "TargetStorageBin", "en{'TODO: TargetStorageBin'}de{'TODO: TargetStorageBin'}")]
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

        #endregion

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

        #region Methods 

        #region Method -> Picking
        /// <summary>
        /// Method NewPicking
        /// </summary>78--
        [ACMethodInfo("NewPicking", "en{'New'}de{'Neu'}", (short)MISort.New, false, false, true)]
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

        /// <summary>
        /// Method DeletePicking
        /// </summary>
        [ACMethodInfo("DeletePicking", "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, false, false, true)]
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

        /// <summary>
        /// Method NewPickingPos
        /// </summary>
        [ACMethodInfo("NewPickingPos", "en{'Take'}de{'Übernehmen'}", (short)MISort.New, false, false, true)]
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

        /// <summary>
        /// Method DeletePickingPos
        /// </summary>
        [ACMethodInfo("DeletePickingPos", "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, false, false, true)]
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

        [ACMethodInteraction("", "en{'Show picking'}de{'Kommission öffnen'}", 781, true, nameof(SelectedPicking))]
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
        }

        #endregion

        [ACMethodInfo("NewPicking", "en{'Calculate demands'}de{'Bedarfsliste ermitteln'}", (short)MISort.Search)]
        public void SearchStockMaterial()
        {
            if (!IsEnabledSearchStockMaterial()) return;
            OnSearchStockMaterial(this, new EventArgs());
        }

        public bool IsEnabledSearchStockMaterial()
        {
            return OnSearchStockMaterial != null;
        }

        [ACMethodInfo("", "en{'Create production order'}de{'Produktionsauftrag erstellen'}", 9999, true)]
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
            Msg msg = ProdOrderManager.PartslistAdd(DatabaseApp, prodOrder, partsList, 1, SelectedPreparedMaterial.TargetQuantityUOM, out prodOrderPartslist);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            ACSaveChanges();

            Messages.Info(this, "Info50104");

        }

        public bool IsEnabledGenerateProductionOrder()
        {
            return SelectedPreparedMaterial != null && SelectedPreparedMaterial.Material != null && SelectedPreparedMaterial.Material.Partslist_Material.Any();
        }

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
                ACFacilityManager.GetMaterialPreparationModel(DatabaseApp.ContextIPlus, DatabaseApp, MediaController, VarioConfigManager, routingParameters, selectedBatchPlans);
            return materialPreparationResult.PreparedMaterials;
        }


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

            OnPropertyChanged(nameof(SourceStorageBinList));
            OnPropertyChanged(nameof(TargetStorageBinList));
        }


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

        #region Private 

        #endregion

    }
}
