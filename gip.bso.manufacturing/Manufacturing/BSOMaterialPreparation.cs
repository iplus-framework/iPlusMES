using gip.core.datamodel;
using VD = gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Linq;
using System.Collections.Generic;
using gip.mes.datamodel;
using gip.mes.autocomponent;
using gip.core.layoutengine;
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

            MediaController = ACMediaController.GetServiceInstance(this);

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

            CleanUp();

            MediaController = null;

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

        public ACMediaController MediaController { get;set;}

        #endregion

        #region Properties

        // PreparedMaterial (PreparedMaterial)

        #region PreparedMaterial
        private PreparedMaterial _SelectedPreparedMaterial;
        /// <summary>
        /// Selected property for PreparedMaterial
        /// </summary>
        /// <value>The selected PreparedMaterial</value>
        [ACPropertySelected(500, "PreparedMaterial", "en{'TODO: PreparedMaterial'}de{'TODO: PreparedMaterial'}")]
        public PreparedMaterial SelectedPreparedMaterial
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

        private List<PreparedMaterial> _PreparedMaterialList;
        /// <summary>
        /// List property for PreparedMaterial
        /// </summary>
        /// <value>The PreparedMaterial list</value>
        [ACPropertyList(501, "PreparedMaterial")]
        public List<PreparedMaterial> PreparedMaterialList
        {
            get
            {
                if (_PreparedMaterialList == null)
                    _PreparedMaterialList = new List<PreparedMaterial>();
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

        [ACMethodInteraction("", "en{'Show picking'}de{'Kommission öffnen'}", 901, true, nameof(SelectedPicking))]
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

        #endregion

        #region BackgroundWorker -> DoMehtods


        #region BackgroundWorker -> DoMehtods -> SearchStockMaterial

        public List<PreparedMaterial> DoSearchStockMaterial(List<ProdOrderBatchPlan> selectedBatchPlans)
        {
            List<SearchBatchMaterialModel> searchModel = new List<SearchBatchMaterialModel>();

            List<PreparedMaterial> preparedMaterials = new List<PreparedMaterial>();
            if (selectedBatchPlans.Any())
            {
                searchModel = GetSearchBatchMaterialModels(selectedBatchPlans);
                preparedMaterials = GetPreparedMaterials(searchModel);
            }

            return preparedMaterials;
        }


        private List<SearchBatchMaterialModel> GetSearchBatchMaterialModels(List<ProdOrderBatchPlan> batchPlans)
        {
            List<SearchBatchMaterialModel> searchResult = new List<SearchBatchMaterialModel>();
            foreach (var batchPlan in batchPlans)
            {
                GetPositionsForBatchMaterialModel(searchResult, batchPlan, batchPlan.ProdOrderPartslistPos, batchPlan.ProdOrderPartslistPos.TargetQuantityUOM);
            }
            return searchResult;
        }

        private void GetPositionsForBatchMaterialModel(List<SearchBatchMaterialModel> searchResult, ProdOrderBatchPlan batchPlan, ProdOrderPartslistPos prodOrderPartslistPos, double posTargetQuantityUOM)
        {
            foreach (ProdOrderPartslistPosRelation prodOrderPartslistPosRelation in prodOrderPartslistPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
            {
                if (prodOrderPartslistPosRelation.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.OutwardRoot)
                {
                    SearchBatchMaterialModel searchBatchMaterialModel = GetRelationForBatchMaterialModel(batchPlan, prodOrderPartslistPosRelation, posTargetQuantityUOM);
                    searchResult.Add(searchBatchMaterialModel);
                }
                else
                {
                    double factor = prodOrderPartslistPosRelation.TargetQuantityUOM / posTargetQuantityUOM;
                    double subPosTargetQuantity = posTargetQuantityUOM * factor;
                    GetPositionsForBatchMaterialModel(searchResult, batchPlan, prodOrderPartslistPosRelation.SourceProdOrderPartslistPos, subPosTargetQuantity);
                }
            }
        }

        private SearchBatchMaterialModel GetRelationForBatchMaterialModel(ProdOrderBatchPlan batchPlan, ProdOrderPartslistPosRelation prodOrderPartslistPosRelation, double posTargetQuantityUOM)
        {
            SearchBatchMaterialModel searchBatchMaterialModel = new SearchBatchMaterialModel();
            searchBatchMaterialModel.MaterialID = prodOrderPartslistPosRelation.SourceProdOrderPartslistPos.Material.MaterialID;
            searchBatchMaterialModel.ProdOrderBatchPlanID = batchPlan.ProdOrderBatchPlanID;
            searchBatchMaterialModel.SourcePosID = prodOrderPartslistPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslistPosID;
            searchBatchMaterialModel.TargetQuantityUOM = prodOrderPartslistPosRelation.SourceProdOrderPartslistPos.TargetQuantityUOM * (batchPlan.TotalSize / batchPlan.ProdOrderPartslist.TargetQuantity);

            if (batchPlan.VBiACClassWF != null && batchPlan.VBiACClassWF.MDSchedulingGroupWF_VBiACClassWF.Any())
            {
                searchBatchMaterialModel.MDSchedulingGroupID = batchPlan.VBiACClassWF.MDSchedulingGroupWF_VBiACClassWF.Select(c => c.MDSchedulingGroupID).FirstOrDefault();
            }
            //if (posTargetQuantityUOM  > Double.Epsilon)
            //{
            //    double factor = prodOrderPartslistPosRelation.TargetQuantityUOM / posTargetQuantityUOM;
            //    searchFacilityModel.TargetQuantityUOM += batchPlan.BatchSize * batchPlan.BatchTargetCount * factor;
            //}
            return searchBatchMaterialModel;
        }

        #endregion

        #endregion

        #region Loading Methods

        public void LoadMaterialPlanFromPos(List<PreparedMaterial> preparedMaterials)
        {
            CleanUp();

            _PreparedMaterialList = preparedMaterials;
            OnPropertyChanged(nameof(PreparedMaterialList));
            SelectedPreparedMaterial = _PreparedMaterialList.FirstOrDefault();
        }

        public List<PreparedMaterial> GetPreparedMaterials(List<SearchBatchMaterialModel> researchedFacilities)
        {
            List<PreparedMaterial> preparedMaterials = new List<PreparedMaterial>();
            var queryResearchedFacilities = researchedFacilities.GroupBy(c => c.MaterialID);
            Guid[] materialIDs = queryResearchedFacilities.Select(c => c.Key).ToArray();
            List<VD.Material> materials = DatabaseApp.Material.Where(c => materialIDs.Contains(c.MaterialID)).ToList();
            int nr = 0;
            foreach (var item in queryResearchedFacilities)
            {
                Guid materialID = item.Key;
                Material material = materials.FirstOrDefault(c => c.MaterialID == materialID);
                MediaController.LoadIImageInfo(material);
                Guid[] posIDs = item.Select(c => c.SourcePosID).Distinct().ToArray();
                nr++;
                PreparedMaterial preparedMaterial = new PreparedMaterial() { Sn = nr, PickingRelationType = PickingRelationTypeEnum.ProductionLine };
                preparedMaterial.Material = material;
                preparedMaterial.MaterialNo = material.MaterialNo;
                preparedMaterial.MaterialName = material.MaterialName1;

                preparedMaterial.DefaultThumbImage = material.DefaultThumbImage;
                preparedMaterial.TargetQuantityUOM = item.Sum(x => x.TargetQuantityUOM);
                preparedMaterial.RelatedIDs = item.Select(c => c.SourcePosID).Distinct().ToArray();

                double availableQuantity =
                    DatabaseApp
                    .FacilityCharge
                    .Where(c =>
                        !c.NotAvailable
                        && c.MaterialID == materialID
                       )
                    .Select(c => c.StockQuantityUOM)
                    .DefaultIfEmpty()
                    .Sum(c => c);
                preparedMaterial.AvailableQuantityUOM = availableQuantity;

                double pickingPosQuantityUOM =
                    DatabaseApp
                    .Picking
                    .Where(c => c.PickingStateIndex < (short)PickingStateEnum.Finished)
                    .SelectMany(c => c.PickingPos_Picking)
                    .Where(c => c.PickingMaterialID == materialID && c.PickingPosProdOrderPartslistPos_PickingPos.Any(x => posIDs.Contains(x.ProdorderPartslistPosID)))
                    .Select(c => c.PickingQuantityUOM ?? 0)
                    .DefaultIfEmpty()
                    .Sum(c => c);

                preparedMaterial.PickingPosQuantityUOM = pickingPosQuantityUOM;

                preparedMaterial.MissingQuantityUOM = preparedMaterial.TargetQuantityUOM - preparedMaterial.PickingPosQuantityUOM;

                preparedMaterial.MDSchedulingGroupIDs =
                    item
                    .Where(c => c.MDSchedulingGroupID != null)
                    .Select(c => c.MDSchedulingGroupID ?? Guid.Empty)
                    .Distinct()
                    .ToArray();

                preparedMaterials.Add(preparedMaterial);
            }
            return preparedMaterials;
        }

        private void LoadPreparedMaterial(PreparedMaterial preparedMaterial)
        {
            _TargetStorageBinList = LoadTargetStorageBins(preparedMaterial);
            if (_TargetStorageBinList != null && _TargetStorageBinList.Any())
            {
                SelectedTargetStorageBin = _TargetStorageBinList.Where(c => c.MDPickingType != null).FirstOrDefault();
                if (SelectedTargetStorageBin == null)
                {
                    SelectedTargetStorageBin = _TargetStorageBinList.FirstOrDefault();
                }
            }

            _SourceStorageBinList = LoadSourceStorageBins(preparedMaterial);
            if (_SourceStorageBinList != null && _SourceStorageBinList.Any())
            {
                SelectedSourceStorageBin = _SourceStorageBinList.OrderByDescending(c => c.SumTotal).FirstOrDefault();
            }

            OnPropertyChanged(nameof(SourceStorageBinList));
            OnPropertyChanged(nameof(TargetStorageBinList));
        }

        private List<FacilityChargeSumFacilityHelper> LoadSourceStorageBins(PreparedMaterial preparedMaterial)
        {
            FacilityCharge[] facilityCharges =
                FacilityManager
                .s_cQry_MatOverviewFacilityCharge(this.DatabaseApp, preparedMaterial.Material.MaterialID, false)
                .Where(c => c.Facility != null && c.Facility.FacilityNo != ProdMatStorage)
                .ToArray();
            List<FacilityChargeSumFacilityHelper> list = ACFacilityManager.GetFacilityChargeSumFacilityHelperList(facilityCharges, new FacilityQueryFilter()).ToList();

            string[] targetsNotShownAsSource = TargetStorageBinList.Where(c => c.MDPickingType != null).Select(c => c.FacilityNo).ToArray();
            list = list.Where(c => !targetsNotShownAsSource.Contains(c.FacilityNo)).ToList();

            foreach (FacilityChargeSumFacilityHelper item in list)
            {
                item.NewPlannedStock = DatabaseApp
                        .PickingPos
                        .Where(c =>
                                c.PickingMaterialID == preparedMaterial.Material.MaterialID
                                && (c.Picking.PickingStateIndex < (short)PickingStateEnum.Finished)
                                && c.FromFacility.FacilityNo == item.FacilityNo
                               )
                        .Select(c => c.PickingQuantityUOM ?? 0)
                        .DefaultIfEmpty()
                        .Sum();
            }
            return list;
        }

        private List<PlanningTargetStockPreview> LoadTargetStorageBins(PreparedMaterial preparedMaterial)
        {
            List<PlanningTargetStockPreview> list = new List<PlanningTargetStockPreview>();

            List<FacilityMDSchedulingGroup> schGroupFacility =
                DatabaseApp
                .MDSchedulingGroup
                .Where(c => preparedMaterial.MDSchedulingGroupIDs.Contains(c.MDSchedulingGroupID))
                .SelectMany(c => c.FacilityMDSchedulingGroup_MDSchedulingGroup)
                .ToList();

            foreach (FacilityMDSchedulingGroup schGroup in schGroupFacility)
            {
                PlanningTargetStockPreview item = list.FirstOrDefault(c => c.FacilityNo == schGroup.Facility.FacilityNo);
                if (item == null)
                {
                    item = new PlanningTargetStockPreview();
                    item.Facility = schGroup.Facility;
                    item.FacilityNo = schGroup.Facility.FacilityNo;
                    item.FacilityName = schGroup.Facility.FacilityName;
                    item.MDPickingType = schGroup.MDPickingType;
                    if (preparedMaterial.Material.FacilityMaterial_Material.Any())
                    {
                        item.OptStockQuantity =
                            preparedMaterial
                            .Material
                            .FacilityMaterial_Material
                            .Where(c => c.FacilityID == schGroup.Facility.FacilityID)
                            .Select(c => c.OptStockQuantity)
                            .FirstOrDefault();
                    }
                    list.Add(item);
                }
            }

            var testConistentQuery =
                schGroupFacility
                .Where(c => c.MDPickingTypeID != null)
                .Select(c => new { c.Facility.FacilityNo, c.Facility.FacilityName, SchedulingGroup_MDKey = c.MDSchedulingGroup.MDKey, c.MDPickingType.MDKey })
                .GroupBy(c => new { c.FacilityNo, c.FacilityName, c.SchedulingGroup_MDKey });

            if (testConistentQuery.Any(c => c.Count() > 1))
            {
                string pickingTypes = string.Join(",", testConistentQuery.Select(c => c));
                Messages.Warning(this, "Warning50054", false, pickingTypes);
            }
            else
            {
                foreach (PlanningTargetStockPreview item in list)
                {
                    item.ActualStockQuantity =
                        DatabaseApp
                        .FacilityCharge
                        .Where(c => c.MaterialID == preparedMaterial.Material.MaterialID && !c.NotAvailable && c.Facility.FacilityNo == item.FacilityNo)
                        .Select(c => c.StockQuantity)
                        .DefaultIfEmpty()
                        .Sum();

                    item.OrderedQuantity =
                        DatabaseApp
                        .PickingPos
                        .Where(c =>
                                c.PickingMaterialID == preparedMaterial.Material.MaterialID
                                && (c.Picking.PickingStateIndex < (short)PickingStateEnum.Finished)
                               )
                        .AsEnumerable()
                        .Select(c => c.TargetQuantityUOM - c.ActualQuantityUOM)
                        .DefaultIfEmpty()
                        .Sum();
                }
            }

            foreach (var item in list)
            {
                if (item.MDPickingType != null)
                {
                    item.NewPlannedStockQuantity = 0;
                    if (preparedMaterial.TargetQuantityUOM > item.ActualStockQuantity)
                        item.NewPlannedStockQuantity = preparedMaterial.TargetQuantityUOM - item.ActualStockQuantity;
                    if (item.OptStockQuantity != null && (item.OptStockQuantity ?? 0) > item.ActualStockQuantity)
                        item.NewPlannedStockQuantity += (item.OptStockQuantity ?? 0) - item.ActualStockQuantity;
                    if (item.OptStockQuantity != null)
                    {
                        if (item.NewPlannedStockQuantity > Const_RangeStockQuantityTolerance)
                        {
                            item.IsInRange = -1;
                        }
                        else if (item.NewPlannedStockQuantity == 0)
                        {
                            item.IsInRange = 1;
                        }
                    }
                }
            }

            return list;
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
