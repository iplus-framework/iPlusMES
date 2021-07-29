using gip.core.datamodel;
using vd = gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Linq;
using System.Collections.Generic;
using gip.mes.datamodel;
using gip.mes.autocomponent;
using gip.core.layoutengine;
using gip.core.autocomponent;

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
        #endregion

        #region DI Properties

        public MediaSettings MediaSettings { get; private set; }

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

            MediaSettings = new MediaSettings();
            Material dummyMaterial = DatabaseApp.Material.FirstOrDefault();
            MediaSettings.LoadTypeFolder(dummyMaterial);

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

            MediaSettings = null;

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
                    PickingQuantityUOM = SelectedPreparedMaterial.MissingQuantityUOM ?? 0;
                    LoadPreparedMaterial(value);
                    LoadPickingPositions();
                    OnPropertyChanged("SelectedPreparedMaterial");
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
                    _PreparedMaterialList = LoadPreparedMaterialList();
                return _PreparedMaterialList;
            }
        }

        private List<PreparedMaterial> LoadPreparedMaterialList()
        {
            return new List<PreparedMaterial>();
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
                    OnPropertyChanged("SelectedSourceStorageBin");
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
                    _SourceStorageBinList = LoadSourceStorageBinList();
                return _SourceStorageBinList;
            }
        }

        private List<FacilityChargeSumFacilityHelper> LoadSourceStorageBinList()
        {
            return new List<FacilityChargeSumFacilityHelper>();
        }

        #endregion

        // TargetStorageBin (FacilityChargeSumFacilityHelper)

        #region TargetStorageBin
        private FacilityChargeSumFacilityHelper _SelectedTargetStorageBin;
        /// <summary>
        /// Selected property for FacilityChargeSumFacilityHelper
        /// </summary>
        /// <value>The selected TargetStorageBin</value>
        [ACPropertySelected(504, "TargetStorageBin", "en{'TODO: TargetStorageBin'}de{'TODO: TargetStorageBin'}")]
        public FacilityChargeSumFacilityHelper SelectedTargetStorageBin
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
                    OnPropertyChanged("SelectedTargetStorageBin");
                }
            }
        }

        private List<FacilityChargeSumFacilityHelper> _TargetStorageBinList;
        /// <summary>
        /// List property for FacilityChargeSumFacilityHelper
        /// </summary>
        /// <value>The TargetStorageBin list</value>
        [ACPropertyList(505, "TargetStorageBin")]
        public List<FacilityChargeSumFacilityHelper> TargetStorageBinList
        {
            get
            {
                if (_TargetStorageBinList == null)
                    _TargetStorageBinList = LoadTargetStorageBinList();
                return _TargetStorageBinList;
            }
        }

        private List<FacilityChargeSumFacilityHelper> LoadTargetStorageBinList()
        {
            return new List<FacilityChargeSumFacilityHelper>();
        }
        #endregion

        // Picking

        #region Picking
        private vd.Picking _SelectedPicking;
        /// <summary>
        /// Selected property for Picking
        /// </summary>
        /// <value>The selected Picking</value>
        [ACPropertySelected(506, "Picking", "en{'TODO: Picking'}de{'TODO: Picking'}")]
        public vd.Picking SelectedPicking
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
                    OnPropertyChanged("SelectedPicking");
                }
            }
        }


        private List<vd.Picking> _PickingList;
        /// <summary>
        /// List property for Picking
        /// </summary>
        /// <value>The Picking list</value>
        [ACPropertyList(507, "Picking")]
        public List<vd.Picking> PickingList
        {
            get
            {
                if (_PickingList == null)
                    _PickingList = LoadPickingList();
                return _PickingList;
            }
        }

        private List<vd.Picking> LoadPickingList()
        {
            return new List<vd.Picking>();
        }
        #endregion

        // PickingPos
        #region PickingPos
        private vd.PickingPos _SelectedPickingPos;
        /// <summary>
        /// Selected property for PickingPos
        /// </summary>
        /// <value>The selected PickingPos</value>
        [ACPropertySelected(508, "PickingPos", "en{'TODO: PickingPos'}de{'TODO: PickingPos'}")]
        public vd.PickingPos SelectedPickingPos
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
                    OnPropertyChanged("SelectedPickingPos");
                }
            }
        }


        private List<vd.PickingPos> _PickingPosList;
        /// <summary>
        /// List property for PickingPos
        /// </summary>
        /// <value>The PickingPos list</value>
        [ACPropertyList(509, "PickingPos")]
        public List<vd.PickingPos> PickingPosList
        {
            get
            {
                if (_PickingPosList == null)
                    _PickingPosList = LoadPickingPosList();
                return _PickingPosList;
            }
        }

        private List<vd.PickingPos> LoadPickingPosList()
        {
            return new List<vd.PickingPos>();
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
                    OnPropertyChanged("PickingQuantityUOM");
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
                    OnPropertyChanged("ShowAllPickingLines");
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
            picking.PickingType = GlobalApp.PickingType.Receipt;
            DatabaseApp.Picking.AddObject(picking);
            ACSaveChanges();
            PickingList.Add(picking);
            OnPropertyChanged("PickingList");
            SelectedPicking = picking;
        }
        public bool IsEnabledNewPicking()
        {
            return true;
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
            if(msg == null || msg.IsSucceded())
            {
                SelectedPicking = PickingList.FirstOrDefault();
                OnPropertyChanged("PickingList");
            }
            else
            {
                Root.Messages.Msg(msg);
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
            vd.PickingPos pickingPos = vd.PickingPos.NewACObject(DatabaseApp, SelectedPicking);
            pickingPos.PickingQuantityUOM = PickingQuantityUOM;
            vd.Material pickingMaterial = DatabaseApp.Material.FirstOrDefault(c => c.MaterialNo == SelectedPreparedMaterial.MaterialNo);
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
            _SelectedTargetStorageBin.NewPlannedStock += pickingPos.PickingQuantityUOM ?? 0;
            _TargetStorageBinList = _TargetStorageBinList.ToList();

            OnPropertyChanged("PreparedMaterialList");
            OnPropertyChanged("SourceStorageBinList");
            OnPropertyChanged("TargetStorageBinList");

            OnPropertyChanged("PickingPosList");
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
            vd.PickingPos pickingPos = SelectedPickingPos;
            PickingPosList.Remove(pickingPos);
            SelectedPreparedMaterial.PickingPosQuantityUOM -= pickingPos.PickingQuantityUOM ?? 0;
            SelectedPreparedMaterial.MissingQuantityUOM = SelectedPreparedMaterial.TargetQuantityUOM - SelectedPreparedMaterial.PickingPosQuantityUOM;
            _PreparedMaterialList = PreparedMaterialList.ToList();
            OnPropertyChanged("PreparedMaterialList");
            var pickingPosPositions = pickingPos.PickingPosProdOrderPartslistPos_PickingPos.ToList();
            foreach (var pickingPosPos in pickingPosPositions)
                pickingPosPos.DeleteACObject(DatabaseApp, false);
            pickingPos.DeleteACObject(DatabaseApp, false);
            OnPropertyChanged("PickingPosList");
            SelectedPickingPos = PickingPosList.FirstOrDefault();
            ACSaveChanges();
            LoadPreparedMaterial(SelectedPreparedMaterial);
        }

        public bool IsEnabledDeletePickingPos()
        {
            return SelectedPickingPos != null && SelectedPreparedMaterial != null; ;
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

        #region Loading Methods

        public void LoadMaterialPlanFromPos(List<SearchBatchMaterialModel> researchedFacilities)
        {
            CleanUp();

            _PreparedMaterialList = GetPreparedMaterials(researchedFacilities);
            OnPropertyChanged("PreparedMaterialList");
            SelectedPreparedMaterial = _PreparedMaterialList.FirstOrDefault();

            LoadPickings();
            LoadPickingPositions();
        }

        public List<PreparedMaterial> GetPreparedMaterials(List<SearchBatchMaterialModel> researchedFacilities)
        {
            List<PreparedMaterial> preparedMaterials = new List<PreparedMaterial>();
            var queryResearchedFacilities = researchedFacilities.GroupBy(c => c.MaterialID);
            Guid[] materialIDs = queryResearchedFacilities.Select(c => c.Key).ToArray();
            List<vd.Material> materials = DatabaseApp.Material.Where(c => materialIDs.Contains(c.MaterialID)).ToList();
            int nr = 0;
            foreach (var item in queryResearchedFacilities)
            {
                Guid materialID = item.Key;
                Material material = materials.FirstOrDefault(c => c.MaterialID == materialID);
                MediaSettings.LoadImage(material);
                Guid[] posIDs = item.Select(c => c.SourcePosID).Distinct().ToArray();
                nr++;
                PreparedMaterial preparedMaterial = new PreparedMaterial() { Sn = nr, PickingRelationType = PickingRelationTypeEnum.ProductionLine };
                preparedMaterial.MaterialID = material.MaterialID;
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
                    .DefaultIfEmpty()
                    .Sum(c => c.StockQuantityUOM);
                preparedMaterial.AvailableQuantityUOM = availableQuantity;

                double pickingPosQuantityUOM =
                    DatabaseApp
                    .Picking
                    .Where(c => c.PickingStateIndex < (short)GlobalApp.PickingState.Finished)
                    .SelectMany(c => c.PickingPos_Picking)
                    .Where(c => c.PickingMaterialID == materialID && c.PickingPosProdOrderPartslistPos_PickingPos.Any(x=>posIDs.Contains(x.ProdorderPartslistPosID)))
                    .DefaultIfEmpty()
                    .Sum(c => c.PickingQuantityUOM ?? 0);

                preparedMaterial.PickingPosQuantityUOM = pickingPosQuantityUOM;

                preparedMaterial.MissingQuantityUOM = preparedMaterial.TargetQuantityUOM - preparedMaterial.PickingPosQuantityUOM ;

                preparedMaterials.Add(preparedMaterial);
            }
            return preparedMaterials;
        }

        private void LoadPreparedMaterial(PreparedMaterial preparedMaterial)
        {
            _SourceStorageBinList = LoadSourceStorageBins(preparedMaterial);
            _TargetStorageBinList = LoadTargetStorageBins(preparedMaterial);
            OnPropertyChanged("SourceStorageBinList");
            OnPropertyChanged("TargetStorageBinList");
        }

        private List<FacilityChargeSumFacilityHelper> LoadSourceStorageBins(PreparedMaterial preparedMaterial)
        {
            FacilityCharge[] facilityCharges =
                FacilityManager
                .s_cQry_MatOverviewFacilityCharge(this.DatabaseApp, preparedMaterial.MaterialID, false)
                .Where(c => c.Facility != null && c.Facility.FacilityNo != ProdMatStorage)
                .ToArray();
            List<FacilityChargeSumFacilityHelper> list = ACFacilityManager.GetFacilityChargeSumFacilityHelperList(facilityCharges, new FacilityQueryFilter()).ToList();
            
            foreach(FacilityChargeSumFacilityHelper item in list)
            {
                item.NewPlannedStock = DatabaseApp
                        .PickingPos
                        .Where(c =>
                                c.PickingMaterialID == preparedMaterial.MaterialID
                                && (c.Picking.PickingStateIndex < (short)GlobalApp.PickingState.Finished)
                                && c.FromFacility.FacilityNo == item.FacilityNo
                               )
                        .Select(c => c.PickingQuantityUOM ?? 0)
                        .DefaultIfEmpty()
                        .Sum();
            }
            return list;
        }

        private List<FacilityChargeSumFacilityHelper> LoadTargetStorageBins(PreparedMaterial preparedMaterial)
        {
            var query =
                DatabaseApp
                .Facility
                .Where(c => c.ParentFacilityID != null && c.VBiFacilityACClassID != null)
                .GroupJoin(DatabaseApp.FacilityCharge.Where(ch => ch.MaterialID == preparedMaterial.MaterialID), fc => fc.FacilityID, ch => ch.FacilityID, (fc, ch) => new { fc, ch })
                .GroupJoin(
                        DatabaseApp
                        .PickingPos
                        .Where(c =>
                                c.PickingMaterialID == preparedMaterial.MaterialID
                                && (c.Picking.PickingStateIndex < (short)GlobalApp.PickingState.Finished)
                               ),
                        tmp => tmp.fc.FacilityID,
                        pp => pp.ToFacilityID,
                        (tmp, pp) => new { fc = tmp.fc, ch = tmp.ch, pp })
                .Select(c =>
                            new FacilityChargeSumFacilityHelper()
                            {
                                Facility = c.fc,
                                FacilityChargeSumFacilityHelperID = Guid.NewGuid(),
                                SumTotal = c.ch.Select(fch => fch.StockQuantityUOM).DefaultIfEmpty().Sum(),
                                SumBlocked = c.ch.Where(o => o.MDReleaseStateID != null && o.MDReleaseState.MDReleaseStateIndex == (short)MDReleaseState.ReleaseStates.Locked).Select(fch => fch.StockQuantityUOM).DefaultIfEmpty().Sum(),
                                SumBlockedAbsolute = c.ch.Where(o => o.MDReleaseStateID != null && o.MDReleaseState.MDReleaseStateIndex == (short)MDReleaseState.ReleaseStates.AbsLocked).Select(fch => fch.StockQuantityUOM).DefaultIfEmpty().Sum(),
                                SumFree = c.ch.Where(o => o.MDReleaseStateID != null && o.MDReleaseState.MDReleaseStateIndex == (short)MDReleaseState.ReleaseStates.Free).Select(fch => fch.StockQuantityUOM).DefaultIfEmpty().Sum(),
                                NewPlannedStock = c.pp.Select(o => o.PickingQuantityUOM ?? 0).DefaultIfEmpty().Sum()
                            }
                );

            return query.ToList();
        }

        private void LoadPickings()
        {
            _PickingList = DatabaseApp
                .Picking
                .Where(c => c.PickingStateIndex < (short)GlobalApp.PickingState.Finished)
                .OrderByDescending(c => c.InsertDate)
                .ToList();
            _SelectedPicking = _PickingList.FirstOrDefault();
            OnPropertyChanged("SelectedPicking");
            OnPropertyChanged("PickingList");
        }

        private void LoadPickingPositions()
        {
            Guid? pickingID = null;
            Guid materialID = Guid.Empty;
            if (SelectedPreparedMaterial != null)
                materialID = SelectedPreparedMaterial.MaterialID;
            if (SelectedPicking != null)
                pickingID = SelectedPicking.PickingID;
            _PickingPosList = DatabaseApp
               .PickingPos
               .Where(c =>
                    c.Picking.PickingStateIndex < (short)GlobalApp.PickingState.Finished
                    && c.PickingMaterialID == materialID
                    && (ShowAllPickingLines || (pickingID == null || (c.PickingID == (pickingID ?? Guid.Empty))))
                )
               .OrderByDescending(c => c.InsertDate)
               .ToList();
            SelectedPickingPos = _PickingPosList.FirstOrDefault();
            OnPropertyChanged("PickingPosList");
        }

        #endregion

        #region Private 

        #endregion

        #region Mockup

        private void LoadMockupData()
        {
            _PreparedMaterialList = LoadMockup_PreparedMaterialList();
            _SourceStorageBinList = LoadMockup__SourceStorageBinList();
            _TargetStorageBinList = LoadMockup_TargetStorageBinList();
            _PickingList = LoadMockup_PickingList();
            _PickingPosList = LoadMockup_PickingPosList();
        }

        private List<PreparedMaterial> LoadMockup_PreparedMaterialList()
        {
            List<PreparedMaterial> list = new List<PreparedMaterial>();
            list.Add(
            new PreparedMaterial()
            {
                MaterialNo = "101",
                MaterialName = "Palmöl",
                TargetQuantityUOM = 12,
                AvailableQuantityUOM = 37456,
                PickingPosQuantityUOM = null,
                MissingQuantityUOM = null
            }
        );
            list.Add(
                new PreparedMaterial()
                {
                    MaterialNo = "2",
                    MaterialName = "Weizenmehl Typ 1050",
                    TargetQuantityUOM = 13.3,
                    AvailableQuantityUOM = 19940,
                    PickingPosQuantityUOM = null,
                    MissingQuantityUOM = null
                }
            );

            list.Add(
                new PreparedMaterial()
                {
                    MaterialNo = "1",
                    MaterialName = "Weizenmehl Typ 550",
                    TargetQuantityUOM = 266.65,
                    AvailableQuantityUOM = 19940,
                    PickingPosQuantityUOM = null,
                    MissingQuantityUOM = null
                }
            );
            list.Add(
                new PreparedMaterial()
                {
                    MaterialNo = "4",
                    MaterialName = "Zucker",
                    TargetQuantityUOM = 1200,
                    AvailableQuantityUOM = 0,
                    PickingPosQuantityUOM = 1200,
                    MissingQuantityUOM = 0
                }
            );
            list.Add(
                new PreparedMaterial()
                {
                    MaterialNo = "3",
                    MaterialName = "Salz",
                    TargetQuantityUOM = 200,
                    AvailableQuantityUOM = 0,
                    PickingPosQuantityUOM = 150,
                    MissingQuantityUOM = 50
                }
            );
            list.Add(
                new PreparedMaterial()
                {
                    MaterialNo = "102",
                    MaterialName = "Wasser",
                    TargetQuantityUOM = 20,
                    AvailableQuantityUOM = 9910.2,
                    PickingPosQuantityUOM = null,
                    MissingQuantityUOM = null
                }
            );
            return list;
        }

        private List<FacilityChargeSumFacilityHelper> LoadMockup__SourceStorageBinList()
        {
            List<FacilityChargeSumFacilityHelper> list = new List<FacilityChargeSumFacilityHelper>();
            list.Add(
            new FacilityChargeSumFacilityHelper()
            {
                FacilityNo = "G01R01E02",
                SumTotal = 0,
                SumFree = 1000,
                SumBlocked = 0,
                NewPlannedStock = 0
            });
            list.Add(
            new FacilityChargeSumFacilityHelper()
            {
                FacilityNo = "ProdMatStorage",
                SumTotal = 3.1,
                SumFree = 3.01,
                SumBlocked = 0,
                NewPlannedStock = 0
            });
            list.Add(
            new FacilityChargeSumFacilityHelper()
            {
                FacilityNo = "G01R01E03",
                SumTotal = 800,
                SumFree = 800,
                SumBlocked = 0,
                NewPlannedStock = 600
            });
            return list;
        }

        private List<FacilityChargeSumFacilityHelper> LoadMockup_TargetStorageBinList()
        {
            List<FacilityChargeSumFacilityHelper> list = new List<FacilityChargeSumFacilityHelper>();
            list.Add(
            new FacilityChargeSumFacilityHelper()
            {
                FacilityNo = "G01R01E02",
                SumTotal = 1000,
                SumFree = 1000,
                SumBlocked = 0,
                NewPlannedStock = 0
            });
            list.Add(
            new FacilityChargeSumFacilityHelper()
            {
                FacilityNo = "ProdMatStorage",
                SumTotal = 1003.01,
                SumFree = 3,
                SumBlocked = 0,
                NewPlannedStock = 1203.011
            });
            return list;
        }

        private List<Picking> LoadMockup_PickingList()
        {
            List<Picking> list = new List<Picking>();
            list.Add(new Picking()
            {
                PickingNo = "1006",
                DeliveryDateFrom = DateTime.Now,
                PickingState = GlobalApp.PickingState.New
            });
            return list;
        }

        private List<PickingPos> LoadMockup_PickingPosList()
        {
            List<PickingPos> list = new List<PickingPos>();
            PickingPos pickingPos = new PickingPos()
            {
                PickingQuantityUOM = 0,
                Comment = "-",
                UpdateDate = DateTime.Now
            };

            List<ProdOrderPartslistPos> tmpPositions = DatabaseApp.ProdOrderPartslistPos.Where(c => c.Material.MaterialNo == "102").Take(2).ToList();
            foreach (ProdOrderPartslistPos tmpPos in tmpPositions)
            {
                PickingPosProdOrderPartslistPos item = new PickingPosProdOrderPartslistPos();
                item.PickingPos = pickingPos;
                item.ProdorderPartslistPos = tmpPos;
                pickingPos.PickingPosProdOrderPartslistPos_PickingPos.Add(item);
            }


            list.Add(new PickingPos()
            {

                PickingQuantityUOM = 0,
                Comment = "-",
                UpdateDate = DateTime.Now
            });
            return list;
        }


        #endregion
    }
}
