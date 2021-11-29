using gip.bso.masterdata;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Objects;
using System.Linq;
using System.Text;
using vd = gip.mes.datamodel;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Batch scheduler'}de{'Batch Zeitplaner'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + vd.ProdOrderBatchPlan.ClassName)]
    public class BSOBatchPlanScheduler : ACBSOvb
    {
        #region const
        public const string BGWorkerMehtod_DoBackwardScheduling = @"DoBackwardScheduling";
        public const string BGWorkerMehtod_DoForwardScheduling = @"DoForwardScheduling";
        public const string BGWorkerMehtod_DoCalculateAll = @"DoCalculateAll";
        public const int Const_MaxFilterDaySpan = 10;
        public const int Const_MaxResultSize = 500;
        #endregion

        #region Configuration

        #region Configuration -> ConfigPreselectedLine

        public IACConfig GetSelectedLineConfig()
        {
            var configs = ACType.GetConfigByKeyACUrl(Root.Environment.User.GetACUrl());
            IACConfig config = configs.FirstOrDefault();
            return config;
        }

        public string GetSelectedLine()
        {

            IACConfig config = GetSelectedLineConfig();
            return config != null ?
                (config.Value != null ? config.Value.ToString() : "") : "";
        }

        public void SetSelectedLineConfig(PAScheduleForPWNode line)
        {
            IACConfig config = GetSelectedLineConfig();
            if (config == null && line != null)
            {
                config = ACType.ValueTypeACClass.NewACConfig(null, ACType.ValueTypeACClass.Database.GetACType(typeof(string)));
                config.KeyACUrl = Root.Environment.User.GetACUrl();
            }
            if (line != null)
                config.Value = line.MDSchedulingGroup.MDKey;
            else if (config != null)
                (config as VBEntityObject).DeleteACObject(Database, false);
            Msg msg = DatabaseApp.ContextIPlus.ACSaveChanges();
        }

        #endregion

        private ACPropertyConfigValue<string> _PABatchPlanSchedulerURL;
        [ACPropertyConfig("PABatchPlanSchedulerURL")]
        public string PABatchPlanSchedulerURL
        {
            get
            {
                if (string.IsNullOrEmpty(_PABatchPlanSchedulerURL.ValueT))
                    _PABatchPlanSchedulerURL.ValueT = LoadPABatchPlanSchedulerURL();
                return _PABatchPlanSchedulerURL.ValueT;
            }
            set
            {
                _PABatchPlanSchedulerURL.ValueT = value;
            }
        }
        private string LoadPABatchPlanSchedulerURL()
        {
            string acUrl = @"\\Planung\BatchPlanScheduler";
            using (ACMonitor.Lock(DatabaseApp.ContextIPlus.QueryLock_1X000))
            {
                core.datamodel.ACClass paClass = DatabaseApp.ContextIPlus.ACClass.FirstOrDefault(c => c.ACIdentifier == PABatchPlanScheduler.ClassName && !c.ACProject.IsProduction);
                while (paClass != null)
                {
                    acUrl = paClass.ACURLComponentCached;
                    paClass = paClass.ACClass_BasedOnACClass.Where(c => c.ACProject.IsProduction).FirstOrDefault();
                }
            }
            return acUrl;
        }


        private ACPropertyConfigValue<vd.GlobalApp.BatchPlanState> _CreatedBatchState;
        [ACPropertyConfig("en{'Created batch state'}de{'Neu Batch Status'}")]
        public vd.GlobalApp.BatchPlanState CreatedBatchState
        {
            get
            {
                return _CreatedBatchState.ValueT;
            }
            set
            {
                _CreatedBatchState.ValueT = value;
                OnPropertyChanged("CreatedBatchState");
            }
        }

        private IACConfig GetConfig(string propertyName, Partslist partslist, core.datamodel.ACClassWF aCClassWF, vd.ACClassWF vbACClassWF)
        {
            int priorityLevel = 0;
            var mandatoryConfigStores = LocalBSOBatchPlan.GetCurrentConfigStores(aCClassWF, vbACClassWF, partslist.MaterialWFID, partslist, null);
            foreach (var item in mandatoryConfigStores)
                item.ClearCacheOfConfigurationEntries();
            string preValueACUrl = null; //(LocalBSOBatchPlan.CurrentPWInfo as IACConfigURL).PreValueACUrl
            string localConfigACUrl = aCClassWF.ConfigACUrl + @"\" + ACStateConst.SMStarting.ToString() + @"\" + propertyName;
            IACConfig aCConfig = LocalBSOBatchPlan.VarioConfigManager.GetConfiguration(mandatoryConfigStores, preValueACUrl, localConfigACUrl, null, out priorityLevel);
            return aCConfig;
        }

        public void LoadConfiguration(WizardSchedulerPartslist schedulerPartslist)
        {
            Partslist partslist = schedulerPartslist.Partslist;
            PartslistACClassMethod method = partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
            vd.ACClassWF vbACClassWF =
                DatabaseApp
                .MDSchedulingGroup
                .Where(c => c.MDSchedulingGroupID == schedulerPartslist.SelectedMDSchedulingGroup.MDSchedulingGroupID)
                .SelectMany(c => c.MDSchedulingGroupWF_MDSchedulingGroup)
                .Where(c => c.VBiACClassWF.ACClassMethodID == method.MaterialWFACClassMethod.ACClassMethodID)
                .Select(c => c.VBiACClassWF)
                .FirstOrDefault();
            core.datamodel.ACClassWF aCClassWF = vbACClassWF.FromIPlusContext<gip.core.datamodel.ACClassWF>(DatabaseApp.ContextIPlus);
            IACConfig batchSizeMin = GetConfig("BatchSizeMin", partslist, aCClassWF, vbACClassWF);
            IACConfig batchSizeMax = GetConfig("BatchSizeMax", partslist, aCClassWF, vbACClassWF);
            IACConfig batchSizeStandard = GetConfig("BatchSizeStandard", partslist, aCClassWF, vbACClassWF);
            IACConfig batchPlanMode = GetConfig("PlanMode", partslist, aCClassWF, vbACClassWF);
            IACConfig batchSuggestionMode = GetConfig("BatchSuggestionMode", partslist, aCClassWF, vbACClassWF);
            IACConfig durationSecAVG = GetConfig("DurationSecAVG", partslist, aCClassWF, vbACClassWF);
            IACConfig startOffsetSecAVG = GetConfig("StartOffsetSecAVG", partslist, aCClassWF, vbACClassWF);
            IACConfig offsetToEndTime = GetConfig("OffsetToEndTime", partslist, aCClassWF, vbACClassWF);

            if (batchSizeMin != null && batchSizeMin.Value != null)
                schedulerPartslist.BatchSizeMinUOM = (double)batchSizeMin.Value;

            if (batchSizeMax != null && batchSizeMax.Value != null)
                schedulerPartslist.BatchSizeMaxUOM = (double)batchSizeMax.Value;

            if (batchSizeStandard != null && batchSizeStandard.Value != null)
                schedulerPartslist.BatchSizeStandardUOM = (double)batchSizeStandard.Value;

            if (batchPlanMode != null && batchPlanMode.Value != null)
            {
                schedulerPartslist.PlanMode = (GlobalApp.BatchPlanMode)batchPlanMode.Value;
                schedulerPartslist.PlanModeName = DatabaseApp.BatchPlanModeList.FirstOrDefault(c => ((short)c.Value) == (short)schedulerPartslist.PlanMode).ACCaption;
            }

            if (batchSuggestionMode != null && batchSuggestionMode.Value != null)
                schedulerPartslist.BatchSuggestionMode = (BatchSuggestionCommandModeEnum)batchSuggestionMode.Value;

            if (durationSecAVG != null)
                schedulerPartslist.DurationSecAVG = (int)durationSecAVG.Value;

            if (startOffsetSecAVG != null)
                schedulerPartslist.StartOffsetSecAVG = (int)startOffsetSecAVG.Value;

            if (offsetToEndTime != null)
                schedulerPartslist.OffsetToEndTime = (TimeSpan)offsetToEndTime.Value;
        }

        private TimeSpan? GetExpectedBatchEndTime(WizardSchedulerPartslist wizardSchedulerPartslist)
        {
            vd.ACClassWF vbACClassWF = wizardSchedulerPartslist.SelectedMDSchedulingGroup.MDSchedulingGroupWF_MDSchedulingGroup.Select(c => c.VBiACClassWF).FirstOrDefault();
            Partslist partslist = DatabaseApp.Partslist.FirstOrDefault(c => c.PartslistNo == wizardSchedulerPartslist.PartslistNo);
            var materialWFConnection = ProdOrderManager.GetMaterialWFConnection(vbACClassWF, partslist.MaterialWFID);

            ACProdOrderManager poManager = ACProdOrderManager.GetServiceInstance(this);
            return poManager.GetCalculatedBatchPlanDuration(DatabaseApp, materialWFConnection.MaterialWFACClassMethodID, vbACClassWF.ACClassWFID);
        }

        #endregion

        #region c´tors

        public BSOBatchPlanScheduler(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _PABatchPlanSchedulerURL = new ACPropertyConfigValue<string>(this, "PABatchPlanSchedulerURL", "");
            //_ConfigPreselectedLine = new ACPropertyConfigValue<string>(this, "ConfigPreselectedLine", "");
        }

        #region c´tors -> ACInit

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            //_ConfigPreselectedLineDict = GetConfigPreselectedLineDict();

            _ProdOrderManager = ACProdOrderManager.ACRefToServiceInstance(this);
            if (_ProdOrderManager == null)
                throw new Exception("ProdOrderManager not configured");

            _PickingManager = ACPickingManager.ACRefToServiceInstance(this);
            if (_PickingManager == null)
                throw new Exception("PickingManager not configured");


            _SchedulingForecastManager = SchedulingForecastManager.ACRefToServiceInstance(this);
            if (_SchedulingForecastManager == null)
                throw new Exception("SchedulingForecastManager not configured");


            var refBatchPlanSchedulerComponent = ACUrlCommand(PABatchPlanSchedulerURL) as ACComponent;
            if (refBatchPlanSchedulerComponent != null)
                _BatchPlanScheduler = new ACRef<ACComponent>(refBatchPlanSchedulerComponent, this);

            MediaSettings = new MediaSettings();
            Material dummyMaterial = DatabaseApp.Material.FirstOrDefault();
            MediaSettings.LoadTypeFolder(dummyMaterial);

            InitBatchPlanSchedulerComponent();

            string selectedLine = GetSelectedLine();
            LoadScheduleListForPWNodes(selectedLine);

            if (BSOPartslistExplorer_Child != null && BSOPartslistExplorer_Child.Value != null && BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child != null && BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value != null)
                BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value.PropertyChanged += ChildBSO_PropertyChanged;

            if (BSOPartslistExplorer_Child != null && BSOPartslistExplorer_Child.Value != null)
                BSOPartslistExplorer_Child.Value.PropertyChanged += ChildBSO_PropertyChanged;


            if (LocalBSOBatchPlan != null)
            {
                LocalBSOBatchPlan.PropertyChanged += ChildBSO_PropertyChanged;
                LocalBSOBatchPlan.PreselectFirstFacilityReservation = true;
            }


            if (BSOMaterialPreparationChild != null && BSOMaterialPreparationChild.Value != null)
                BSOMaterialPreparationChild.Value.OnSearchStockMaterial += Value_OnSearchStockMaterial;

            _CreatedBatchState = new ACPropertyConfigValue<vd.GlobalApp.BatchPlanState>(this, "CreatedBatchState", vd.GlobalApp.BatchPlanState.Created);

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_ProdOrderManager != null)
                ACProdOrderManager.DetachACRefFromServiceInstance(this, _ProdOrderManager);
            _ProdOrderManager = null;

            if (_PickingManager != null)
                ACPickingManager.DetachACRefFromServiceInstance(this, _PickingManager);
            _PickingManager = null;

            if (_SchedulingForecastManager != null)
                SchedulingForecastManager.DetachACRefFromServiceInstance(this, _SchedulingForecastManager);
            _SchedulingForecastManager = null;

            if (_BatchPlanScheduler != null)
            {
                if (_SchedulesForPWNodesProp != null)
                    _SchedulesForPWNodesProp.PropertyChanged -= SchedulesForPWNodesProp_Changed;
                _BatchPlanScheduler.Detach();
                _BatchPlanScheduler = null;
            }

            if (BSOPartslistExplorer_Child != null && BSOPartslistExplorer_Child.Value != null && BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child != null && BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value != null)
                BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value.PropertyChanged -= ChildBSO_PropertyChanged;

            if (BSOPartslistExplorer_Child != null && BSOPartslistExplorer_Child.Value != null)
                BSOPartslistExplorer_Child.Value.PropertyChanged -= ChildBSO_PropertyChanged;

            if (BSOMaterialPreparationChild != null && BSOMaterialPreparationChild.Value != null)
                BSOMaterialPreparationChild.Value.OnSearchStockMaterial -= Value_OnSearchStockMaterial;

            if (LocalBSOBatchPlan != null)
                LocalBSOBatchPlan.PropertyChanged += ChildBSO_PropertyChanged;

            MediaSettings = null;
            SelectedProdOrderBatchPlan = null;
            IsWizard = false;

            return base.ACDeInit(deleteACClassTask);
        }

        private vd.DatabaseApp _DatabaseApp;
        public override vd.DatabaseApp DatabaseApp
        {
            get
            {
                if (_DatabaseApp != null)
                    return _DatabaseApp;
                if (ParentACComponent is Businessobjects
                    || !(ParentACComponent is ACBSOvb || ParentACComponent is ACBSOvbNav))
                {
                    _DatabaseApp = ACObjectContextManager.GetOrCreateContext<vd.DatabaseApp>(this.GetACUrl());
                    return _DatabaseApp;
                }
                else
                {
                    ACBSOvbNav parentNav = ParentACComponent as ACBSOvbNav;
                    if (parentNav != null)
                        return parentNav.DatabaseApp;
                    ACBSOvb parent = ParentACComponent as ACBSOvb;
                    if (parent != null)
                        return parent.DatabaseApp;
                    _DatabaseApp = ACObjectContextManager.GetOrCreateContext<vd.DatabaseApp>(this.GetACUrl());
                    return _DatabaseApp;
                }
            }
        }

        private void Value_OnSearchStockMaterial(object sender, EventArgs e)
        {
            if (ProdOrderBatchPlanList != null)
            {
                List<ProdOrderBatchPlan> selectedBatchPlans = ProdOrderBatchPlanList.Where(c => c.IsSelected).ToList();
                List<SearchBatchMaterialModel> searchModel = GetSearchBatchMaterialModels(selectedBatchPlans);
                BSOMaterialPreparationChild.Value.LoadMaterialPlanFromPos(searchModel);
            }
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
                    GetRelationForBatchMaterialModel(searchResult, batchPlan, prodOrderPartslistPosRelation, posTargetQuantityUOM);
                }
                else
                {
                    double factor = prodOrderPartslistPosRelation.TargetQuantityUOM / posTargetQuantityUOM;
                    double subPosTargetQuantity = posTargetQuantityUOM * factor;
                    GetPositionsForBatchMaterialModel(searchResult, batchPlan, prodOrderPartslistPosRelation.SourceProdOrderPartslistPos, subPosTargetQuantity);
                }
            }
        }

        private double GetRelationForBatchMaterialModel(List<SearchBatchMaterialModel> searchResult, ProdOrderBatchPlan batchPlan, ProdOrderPartslistPosRelation prodOrderPartslistPosRelation, double posTargetQuantityUOM)
        {
            SearchBatchMaterialModel searchFacilityModel = new SearchBatchMaterialModel();
            searchFacilityModel.MaterialID = prodOrderPartslistPosRelation.SourceProdOrderPartslistPos.Material.MaterialID;
            searchFacilityModel.ProdOrderBatchPlanID = batchPlan.ProdOrderBatchPlanID;
            searchFacilityModel.SourcePosID = prodOrderPartslistPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslistPosID;
            searchFacilityModel.TargetQuantityUOM = prodOrderPartslistPosRelation.SourceProdOrderPartslistPos.TargetQuantityUOM * (batchPlan.TotalSize / batchPlan.ProdOrderPartslist.TargetQuantity);
            searchResult.Add(searchFacilityModel);

            //if (posTargetQuantityUOM  > Double.Epsilon)
            //{
            //    double factor = prodOrderPartslistPosRelation.TargetQuantityUOM / posTargetQuantityUOM;
            //    searchFacilityModel.TargetQuantityUOM += batchPlan.BatchSize * batchPlan.BatchTargetCount * factor;
            //}
            return searchFacilityModel.TargetQuantityUOM;
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "RecalculateBatchSuggestion":
                    RecalculateBatchSuggestion();
                    return true;
                case "IsEnabledRecalculateBatchSuggestion":
                    result = IsEnabledRecalculateBatchSuggestion();
                    return true;
                case "Search":
                    Search();
                    return true;
                case "Save":
                    Save();
                    return true;
                case "UndoSave":
                    UndoSave();
                    return true;
                case "ChangeMode":
                    ChangeMode();
                    return true;
                case "IsEnabledChangeMode":
                    result = IsEnabledChangeMode();
                    return true;
                case "IsEnabledSave":
                    result = IsEnabledSave();
                    return true;
                case "New":
                    New();
                    return true;
                case "IsEnabledNew":
                    result = IsEnabledNew();
                    return true;
                case "AddBatchPlan":
                    AddBatchPlan();
                    return true;
                case "IsEnabledAddBatchPlan":
                    result = IsEnabledAddBatchPlan();
                    return true;
                case "IsEnabledFactoryBatch":
                    result = IsEnabledFactoryBatch();
                    return true;
                case "ItemDrag":
                    ItemDrag((System.Collections.Generic.Dictionary<System.Int32, System.String>)acParameter[0]);
                    return true;
                case "IsEnabledItemDrag":
                    result = IsEnabledItemDrag();
                    return true;
                case "BackwardScheduling":
                    BackwardScheduling();
                    return true;
                case "IsEnabledBackwardScheduling":
                    result = IsEnabledBackwardScheduling();
                    return true;
                case "ForwardScheduling":
                    ForwardScheduling();
                    return true;
                case "IsEnabledForwardScheduling":
                    result = IsEnabledForwardScheduling();
                    return true;
                case "SetBatchStateReadyToStart":
                    SetBatchStateReadyToStart();
                    return true;
                case "IsEnabledSetBatchStateReadyToStart":
                    result = IsEnabledSetBatchStateReadyToStart();
                    return true;
                case "SetBatchStateCreated":
                    SetBatchStateCreated();
                    return true;
                case "IsEnabledSetBatchStateCreated":
                    result = IsEnabledSetBatchStateCreated();
                    return true;
                case "SetBatchStateCancelled":
                    SetBatchStateCancelled();
                    return true;
                case "IsEnabledSetBatchStateCancelled":
                    result = IsEnabledSetBatchStateCancelled();
                    return true;
                case "WizardForward":
                    WizardForward();
                    return true;
                case "IsEnabledWizardForward":
                    result = IsEnabledWizardForward();
                    return true;
                case "WizardBackward":
                    WizardBackward();
                    return true;
                case "IsEnabledWizardBackward":
                    result = IsEnabledWizardBackward();
                    return true;
                case "WizardCancel":
                    WizardCancel();
                    return true;
                case "NavigateToProdOrder":
                    NavigateToProdOrder();
                    return true;
                case "IsEnabledNavigateToProdOrder":
                    result = IsEnabledNavigateToProdOrder();
                    return true;
                case "DeleteBatch":
                    DeleteBatch();
                    return true;
                case "IsEnabledDeleteBatch":
                    result = IsEnabledDeleteBatch();
                    return true;
                case "MoveToOtherLine":
                    MoveToOtherLine();
                    return true;
                case "IsEnabledMoveToOtherLine":
                    result = IsEnabledMoveToOtherLine();
                    return true;
                case "Load":
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
                case "IsEnabledLoad":
                    result = IsEnabledLoad();
                    return true;
                case "ChangeBatchPlan":
                    ChangeBatchPlan((ProdOrderBatchPlan)acParameter[0]);
                    return true;
                case "IsEnabledSearch":
                    result = IsEnabledSearch();
                    return true;
                case "IsEnabledAddSuggestion":
                    result = IsEnabledAddSuggestion();
                    return true;
                case "IsEnabledRemoveSuggestion":
                    result = IsEnabledRemoveSuggestion();
                    return true;
                case "WizardForwardSelectLinie":
                    WizardForwardSelectLinie(acParameter[0]);
                    return true;
                default:
                    break;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #region c´tors -> ACInit -> Scheduler Component

        private void InitBatchPlanSchedulerComponent()
        {
            var refBatchPlanSchedulerComponent = ACUrlCommand(PABatchPlanSchedulerURL) as ACComponent;
            if (refBatchPlanSchedulerComponent != null)
                _BatchPlanScheduler = new ACRef<ACComponent>(refBatchPlanSchedulerComponent, this);

            if (_BatchPlanScheduler != null)
            {
                _SchedulesForPWNodesProp = _BatchPlanScheduler.ValueT.GetPropertyNet(PABatchPlanScheduler.PN_SchedulesForPWNodes) as IACContainerTNet<PAScheduleForPWNodeList>;
                if (_SchedulesForPWNodesProp != null)
                    _SchedulesForPWNodesProp.PropertyChanged += SchedulesForPWNodesProp_Changed;
            }
        }

        private void SchedulesForPWNodesProp_Changed(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            string selectedLine = GetSelectedLine();
            LoadScheduleListForPWNodes(selectedLine);
        }

        #endregion

        private void ChildBSO_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedPartslist")
            {
                OnPropertyChanged("UnitConvertList");
                if (BSOPartslistExplorer_Child.Value.SelectedPartslist != null
                    && BSOPartslistExplorer_Child.Value.SelectedPartslist.Material != null)
                {
                    // Always Base-UOM:
                    Partslist selectedPartslist = BSOPartslistExplorer_Child.Value.SelectedPartslist;
                    WizardDefineDefaultPartslist(SelectedScheduleForPWNode.MDSchedulingGroup, selectedPartslist, 0);
                }
            }
        }

        #endregion

        #endregion

        #region Managers

        protected ACRef<ACProdOrderManager> _ProdOrderManager = null;
        public ACProdOrderManager ProdOrderManager
        {
            get
            {
                if (_ProdOrderManager == null)
                    return null;
                return _ProdOrderManager.ValueT;
            }
        }


        protected ACRef<ACComponent> _BatchPlanScheduler = null;
        public ACComponent BatchPlanScheduler
        {
            get
            {
                if (_BatchPlanScheduler == null) return null;
                return _BatchPlanScheduler.ValueT;
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

        protected ACRef<SchedulingForecastManager> _SchedulingForecastManager = null;
        public SchedulingForecastManager SchedulingForecastManager
        {
            get
            {
                if (_SchedulingForecastManager == null)
                    return null;
                return _SchedulingForecastManager.ValueT;
            }
        }
        #endregion

        #region Child (Local BSOs)

        ACChildItem<BSOPartslistExplorer> _BSOPartslistExplorer_Child;
        [ACPropertyInfo(590)]
        [ACChildInfo("BSOPartslistExplorer_Child", typeof(BSOPartslistExplorer))]
        public ACChildItem<BSOPartslistExplorer> BSOPartslistExplorer_Child
        {
            get
            {
                if (_BSOPartslistExplorer_Child == null)
                    _BSOPartslistExplorer_Child = new ACChildItem<BSOPartslistExplorer>(this, "BSOPartslistExplorer_Child");
                return _BSOPartslistExplorer_Child;
            }
        }

        ACChildItem<BSOBatchPlan> _BSOBatchPlanChild;
        [ACPropertyInfo(591)]
        [ACChildInfo("BSOBatchPlan_Child", typeof(BSOBatchPlan))]
        public ACChildItem<BSOBatchPlan> BSOBatchPlanChild
        {
            get
            {
                if (_BSOBatchPlanChild == null)
                    _BSOBatchPlanChild = new ACChildItem<BSOBatchPlan>(this, "BSOBatchPlan_Child");
                return _BSOBatchPlanChild;
            }
        }

        [ACPropertyInfo(592)]
        public virtual BSOBatchPlan LocalBSOBatchPlan
        {
            get
            {
                if (BSOBatchPlanChild == null || BSOBatchPlanChild.Value == null)
                    return null;
                else
                    return BSOBatchPlanChild.Value;
            }
        }

        ACChildItem<BSOMaterialPreparation> _BSOMaterialPreparationChild;
        [ACPropertyInfo(593)]
        [ACChildInfo("BSOMaterialPreparation_Child", typeof(BSOMaterialPreparation))]
        public ACChildItem<BSOMaterialPreparation> BSOMaterialPreparationChild
        {
            get
            {
                if (_BSOMaterialPreparationChild == null)
                    _BSOMaterialPreparationChild = new ACChildItem<BSOMaterialPreparation>(this, "BSOMaterialPreparation_Child");
                return _BSOMaterialPreparationChild;
            }
        }

        #endregion

        #region Properties

        #region Properties -> Common

        public bool IsBSOTemplateScheduleParent
        {
            get
            {
                return ParentACComponent != null && ParentACComponent is BSOTemplateSchedule;
            }
        }
        #endregion

        #region Properties -> Explorer

        #region Properties -> Explorer -> ScheduleForPWNode

        private IACContainerTNet<PAScheduleForPWNodeList> _SchedulesForPWNodesProp;

        private PAScheduleForPWNode _SelectedScheduleForPWNode;
        /// <summary>
        /// Selected property for BatchStartModeConfiguration
        /// </summary>
        /// <value>The selected BatchStartModeConfiguration</value>
        [ACPropertySelected(501, "ScheduleForPWNode", "en{'Schedule for WF-Batch-Manager'}de{'Zeitplan für WF-Batch-Manager'}")]
        public PAScheduleForPWNode SelectedScheduleForPWNode
        {
            get
            {
                return _SelectedScheduleForPWNode;
            }
            set
            {
                if (_SelectedScheduleForPWNode != value)
                {
                    _SelectedScheduleForPWNode = value;
                    OnPropertyChanged("SelectedScheduleForPWNode");
                    if (_SelectedScheduleForPWNode != null)
                        SelectedFilterBatchPlanStartMode = FilterBatchPlanStartModeList.Where(c => (GlobalApp.BatchPlanStartModeEnum)c.Value == _SelectedScheduleForPWNode.StartMode).FirstOrDefault();
                    else
                        SelectedFilterBatchPlanStartMode = FilterBatchPlanStartModeList.Where(c => (GlobalApp.BatchPlanStartModeEnum)c.Value == GlobalApp.BatchPlanStartModeEnum.Off).FirstOrDefault();
                    LoadProdOrderBatchPlanList();

                    OnPropertyChanged("TargetScheduleForPWNodeList");
                    if (TargetScheduleForPWNodeList != null)
                        SelectedTargetScheduleForPWNode = TargetScheduleForPWNodeList.FirstOrDefault();
                    else
                        SelectedTargetScheduleForPWNode = null;

                    SelectedTargetScheduleForPWNode = null;

                    string selectedLinie = GetSelectedLine();
                    string mdKey = "";
                    if (value != null)
                        mdKey = value.MDSchedulingGroup.MDKey;
                    if (selectedLinie != mdKey)
                        SetSelectedLineConfig(value);
                }
            }
        }

        /// <summary>
        /// List property for BatchStartModeConfiguration
        /// </summary>
        /// <value>The BatchStartModeConfiguration list</value>
        private PAScheduleForPWNodeList _ScheduleForPWNodeList;
        [ACPropertyList(502, "ScheduleForPWNode")]
        public IEnumerable<PAScheduleForPWNode> ScheduleForPWNodeList
        {
            get
            {
                if (_ScheduleForPWNodeList != null && _ScheduleForPWNodeList.Any())
                {
                    return _ScheduleForPWNodeList.OrderBy(c => c.MDSchedulingGroup.SortIndex).ThenBy(c => c.MDSchedulingGroup.MDSchedulingGroupName);
                }
                return _ScheduleForPWNodeList;
            }
        }

        #endregion

        #region Properties -> Explorer -> FilterBatchPlanStartMode

        private ACValueItem _SelectedFilterBatchPlanStartMode;
        [ACPropertySelected(999, "FilterBatchPlanStartMode", "en{'Active selection rule query'}de{'Aktive Selektionsregelabfrage'}")]
        public ACValueItem SelectedFilterBatchPlanStartMode
        {
            get
            {
                return _SelectedFilterBatchPlanStartMode;
            }
            set
            {
                _SelectedFilterBatchPlanStartMode = value;
                OnPropertyChanged("SelectedFilterBatchPlanStartMode");
            }
        }

        private ACValueItemList _FilterBatchPlanStartModeList;
        [ACPropertyList(999, "FilterBatchPlanStartMode")]
        public ACValueItemList FilterBatchPlanStartModeList
        {
            get
            {
                if (_FilterBatchPlanStartModeList == null)
                    _FilterBatchPlanStartModeList = vd.GlobalApp.BatchPlanStartModeEnumList;
                return _FilterBatchPlanStartModeList;
            }
        }
        #endregion

        #region Properties -> Explorer -> TargetScheduleForPWNode

        private PAScheduleForPWNode _SelectedTargetScheduleForPWNode;
        [ACPropertySelected(503, "TargetScheduleForPWNode", "en{'Move to other line'}de{'Zu anderer Linie verschieben'}")]
        public PAScheduleForPWNode SelectedTargetScheduleForPWNode
        {
            get
            {
                return _SelectedTargetScheduleForPWNode;
            }
            set
            {
                if (_SelectedTargetScheduleForPWNode != value)
                {
                    _SelectedTargetScheduleForPWNode = value;
                    OnPropertyChanged("SelectedTargetScheduleForPWNode");
                }
            }
        }

        [ACPropertyList(504, "TargetScheduleForPWNode")]
        public IEnumerable<PAScheduleForPWNode> TargetScheduleForPWNodeList
        {
            get
            {
                if (SelectedScheduleForPWNode == null) return null;
                var connections = PartslistMDSchedulerGroupConnections.Where(c => c.SchedulingGroups.Any(x => x.MDSchedulingGroupID == SelectedScheduleForPWNode.MDSchedulingGroupID));
                Guid[] mdSchedulerOtherGroups = connections.SelectMany(c => c.SchedulingGroups).Select(c => c.MDSchedulingGroupID).Where(c => c != SelectedScheduleForPWNode.MDSchedulingGroupID).Distinct().ToArray();
                return
                    ScheduleForPWNodeList.Where(c => mdSchedulerOtherGroups.Contains(c.MDSchedulingGroupID)).ToList();
            }
        }

        #endregion

        #endregion

        #region Properties -> (Tab)BatchPlanScheduler

        #region Properties -> (Tab)BatchPlanScheduler -> Filter (Search)

        private DateTime? _FilterStartTime;
        [ACPropertyInfo(525, "FilterStartTime", "en{'From'}de{'Von'}")]
        public DateTime? FilterStartTime
        {
            get
            {
                return _FilterStartTime;
            }
            set
            {
                if (_FilterStartTime != value)
                {
                    _FilterStartTime = value;
                    OnPropertyChanged("FilterStartTime");
                }
            }
        }

        private DateTime? _FilterEndTime;
        [ACPropertyInfo(526, "FilterEndTime", "en{'To'}de{'Bis'}")]
        public DateTime? FilterEndTime
        {
            get
            {
                return _FilterEndTime;
            }
            set
            {
                if (_FilterEndTime != value)
                {
                    _FilterEndTime = value;
                    OnPropertyChanged("FilterEndTime");
                }
            }
        }

        #region Properties -> (Tab)BatchPlanScheduler -> Filter (Search) -> FilterConnectedLine [PAScheduleForPWNode]


        #region FilterConnectedLine
        private PAScheduleForPWNode _SelectedFilterConnectedLine;
        /// <summary>
        /// Selected property for PAScheduleForPWNode
        /// </summary>
        /// <value>The selected FilterConnectedLine</value>
        [ACPropertySelected(9999, "FilterConnectedLine", "en{'Show only related orders from production line'}de{'Nur verbundene Aufträge aus Produktionslinie anzeigen'}")]
        public PAScheduleForPWNode SelectedFilterConnectedLine
        {
            get
            {
                return _SelectedFilterConnectedLine;
            }
            set
            {
                if (_SelectedFilterConnectedLine != value)
                {
                    _SelectedFilterConnectedLine = value;
                    OnPropertyChanged("SelectedFilterConnectedLine");
                }
            }
        }


        private List<PAScheduleForPWNode> _FilterConnectedLineList;
        /// <summary>
        /// List property for PAScheduleForPWNode
        /// </summary>
        /// <value>The FilterConnectedLine list</value>
        [ACPropertyList(9999, "FilterConnectedLine")]
        public List<PAScheduleForPWNode> FilterConnectedLineList
        {
            get
            {
                if (_FilterConnectedLineList == null)
                {
                    _FilterConnectedLineList = LoadFilterConnectedLineList();
                    if (_FilterConnectedLineList != null)
                        SelectedFilterConnectedLine = _FilterConnectedLineList.FirstOrDefault();
                }
                return _FilterConnectedLineList;
            }
        }

        private List<PAScheduleForPWNode> LoadFilterConnectedLineList()
        {
            List<PAScheduleForPWNode> list = ScheduleForPWNodeList.ToList();
            PAScheduleForPWNode emptyNode = new PAScheduleForPWNode() { MDSchedulingGroup = new MDSchedulingGroup() { MDSchedulingGroupName = "-" } };
            list.Insert(0, emptyNode);
            return list;
        }
        #endregion


        #endregion

        #endregion

        #region Properties -> (Tab)BatchPlanScheduler -> ProdOrderBatchPlan

        private vd.ProdOrderBatchPlan _SelectedProdOrderBatchPlan;
        [ACPropertySelected(505, "ProdOrderBatchPlan")]
        public vd.ProdOrderBatchPlan SelectedProdOrderBatchPlan
        {
            get => _SelectedProdOrderBatchPlan;
            set
            {
                bool changed = false;
                if (_SelectedProdOrderBatchPlan != value)
                {
                    changed = true;
                    if (_SelectedProdOrderBatchPlan != null)
                        _SelectedProdOrderBatchPlan.PropertyChanged -= _SelectedProdOrderBatchPlan_PropertyChanged;
                }
                _SelectedProdOrderBatchPlan = value;
                if (changed && _SelectedProdOrderBatchPlan != null)
                    _SelectedProdOrderBatchPlan.PropertyChanged += _SelectedProdOrderBatchPlan_PropertyChanged;
                OnPropertyChanged("SelectedProdOrderBatchPlan");
            }
        }

        private void _SelectedProdOrderBatchPlan_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PartialTargetCount"
                && SelectedScheduleForPWNode != null
                && SelectedScheduleForPWNode.StartMode == GlobalApp.BatchPlanStartModeEnum.SemiAutomatic
                && !_IsRefreshingBatchPlan)
            {
                if (SelectedProdOrderBatchPlan.PartialTargetCount.HasValue && SelectedProdOrderBatchPlan.PartialTargetCount > 0)
                {
                    if (SelectedProdOrderBatchPlan.PlanState <= GlobalApp.BatchPlanState.Created
                        || SelectedProdOrderBatchPlan.PlanState >= GlobalApp.BatchPlanState.Paused)
                        SelectedProdOrderBatchPlan.PlanState = GlobalApp.BatchPlanState.ReadyToStart;
                }
                else if (SelectedProdOrderBatchPlan.PartialTargetCount.HasValue && SelectedProdOrderBatchPlan.PartialTargetCount <= 0)
                {
                    SelectedProdOrderBatchPlan.PartialTargetCount = null;
                    if (SelectedProdOrderBatchPlan.PlanState == GlobalApp.BatchPlanState.ReadyToStart)
                        SelectedProdOrderBatchPlan.PlanState = GlobalApp.BatchPlanState.Paused;
                }
                Save();
            }
        }

        private ObservableCollection<vd.ProdOrderBatchPlan> _ProdOrderBatchPlanList;
        [ACPropertyList(506, "ProdOrderBatchPlan")]
        public ObservableCollection<vd.ProdOrderBatchPlan> ProdOrderBatchPlanList
        {
            get
            {
                if (_ProdOrderBatchPlanList == null)
                    _ProdOrderBatchPlanList = new ObservableCollection<ProdOrderBatchPlan>();
                return _ProdOrderBatchPlanList;
            }
            protected set
            {
                _ProdOrderBatchPlanList = value;
                OnPropertyChanged("ProdOrderBatchPlanList");
            }
        }

        private bool _IsRefreshingBatchPlan = false;
        private ObservableCollection<vd.ProdOrderBatchPlan> GetProdOrderBatchPlanList(Guid? mdSchedulingGroupID)
        {
            if (!mdSchedulingGroupID.HasValue)
                return new ObservableCollection<vd.ProdOrderBatchPlan>();

            vd.GlobalApp.BatchPlanState startState = GlobalApp.BatchPlanState.Created;
            vd.GlobalApp.BatchPlanState endState = GlobalApp.BatchPlanState.Paused;
            MDProdOrderState.ProdOrderStates? minProdOrderState = null;
            ObservableCollection<vd.ProdOrderBatchPlan> prodOrderBatchPlans = null;
            try
            {
                _IsRefreshingBatchPlan = true;
                prodOrderBatchPlans =
                    ProdOrderManager
                    .GetProductionLinieBatchPlans(
                        DatabaseApp,
                        mdSchedulingGroupID,
                        startState,
                        endState,
                        FilterStartTime,
                        FilterEndTime,
                        minProdOrderState,
                        FilterPlanningMR?.PlanningMRID);

                // Filter list if SelectedFilterConnectedLine is selected: Only batch they have connection via ProdOrder with other line
                if (SelectedFilterConnectedLine != null && SelectedFilterConnectedLine.MDSchedulingGroupID != Guid.Empty)
                {
                    var includedProductionOrders =
                    ProdOrderManager
                   .GetProductionLinieBatchPlans(
                       DatabaseApp,
                       SelectedFilterConnectedLine.MDSchedulingGroupID,
                       startState,
                       endState,
                       FilterStartTime,
                       FilterEndTime,
                       minProdOrderState,
                       FilterPlanningMR?.PlanningMRID)
                   .Select(c => c.ProdOrderPartslist.ProdOrderID);
                    prodOrderBatchPlans = new ObservableCollection<ProdOrderBatchPlan>(prodOrderBatchPlans.Where(c => includedProductionOrders.Contains(c.ProdOrderPartslist.ProdOrderID)));
                }
            }
            finally
            {
                _IsRefreshingBatchPlan = false;
            }
            if (prodOrderBatchPlans != null)
            {
                foreach (var batchPlan in prodOrderBatchPlans)
                {
                    Material material = batchPlan.ProdOrderPartslist.Partslist.Material;
                    MediaSettings.LoadImage(material);
                }
            }
            return prodOrderBatchPlans;
        }


        #endregion

        #region Properties -> (Tab)BatchPlanScheduler -> Scheduling

        private DateTime? _ScheduledStartDate;
        [ACPropertyInfo(518, "ScheduledStartDate", "en{'Planned Start Date'}de{'Geplante Startzeit'}")]
        public DateTime? ScheduledStartDate
        {
            get
            {
                return _ScheduledStartDate;
            }
            set
            {
                _ScheduledStartDate = value;
                OnPropertyChanged("ScheduledStartDate");
            }
        }


        private DateTime? _ScheduledEndDate;
        [ACPropertyInfo(518, "ScheduledEndDate", "en{'Planned End Date'}de{'Geplante Endezeit'}")]
        public DateTime? ScheduledEndDate
        {
            get
            {
                return _ScheduledEndDate;
            }
            set
            {
                _ScheduledEndDate = value;
                OnPropertyChanged("ScheduledEndDate");
            }
        }

        #endregion

        #region Properties -> (Tab)BatchPlanScheduler -> Other

        private List<PartslistMDSchedulerGroupConnection> _PartslistMDSchedulerGroupConnections;
        public List<PartslistMDSchedulerGroupConnection> PartslistMDSchedulerGroupConnections
        {
            get
            {
                if (_PartslistMDSchedulerGroupConnections == null)
                    _PartslistMDSchedulerGroupConnections = ProdOrderManager.GetPartslistMDSchedulerGroupConnections(DatabaseApp, PWNodeProcessWorkflowVB.PWClassName);
                return _PartslistMDSchedulerGroupConnections;
            }
        }



        public MediaSettings MediaSettings { get; private set; }


        private PlanningMR _FilterPlanningMR;
        public PlanningMR FilterPlanningMR
        {
            get
            {
                return _FilterPlanningMR;
            }
            set
            {
                if (_FilterPlanningMR != value)
                {
                    _FilterPlanningMR = value;
                    LoadProdOrderBatchPlanList();
                    OnPropertyChanged("FilterPlanningMR");
                }
            }
        }

        #endregion

        #endregion

        #region Properties -> (Tab)ProdOrder
        #region Properties -> (Tab)ProdOrder -> Filter

        private DateTime? _FilterOrderStartTime;
        /// <summary>
        /// Selected property for 
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "FilterOrderStartTime", "en{'From'}de{'Von'}")]
        public DateTime? FilterOrderStartTime
        {
            get
            {
                return _FilterOrderStartTime;
            }
            set
            {
                if (_FilterOrderStartTime != value)
                {
                    _FilterOrderStartTime = value;
                    OnPropertyChanged("FilterOrderStartTime");
                    OnPropertyChanged("FilterOrderEndTime");
                }
            }
        }

        private DateTime? _FilterOrderEndTime;
        /// <summary>
        /// Selected property for 
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "FilterOrderEndTime", "en{'To'}de{'Bis'}")]
        public DateTime? FilterOrderEndTime
        {
            get
            {
                return _FilterOrderEndTime;
            }
            set
            {
                if (_FilterOrderEndTime != value)
                {
                    _FilterOrderEndTime = value;
                    OnPropertyChanged("FilterOrderStartTime");
                    OnPropertyChanged("FilterOrderEndTime");
                }
            }
        }

        private bool? _FilterOrderIsCompleted = false;
        /// <summary>
        /// Selected property for 
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "FilterOrderIsCompleted", "en{'Completed'}de{'Erledigt'}")]
        public bool? FilterOrderIsCompleted
        {
            get
            {
                return _FilterOrderIsCompleted;
            }
            set
            {
                if (_FilterOrderIsCompleted != value)
                {
                    _FilterOrderIsCompleted = value;
                    OnPropertyChanged("FilterOrderIsCompleted");
                    OnPropertyChanged("FilterOrderStartTime");
                    OnPropertyChanged("FilterOrderEndTime");
                }
            }
        }

        #endregion

        #region Properties -> (Tab)ProdOrder -> ProdOrderPartslist

        private ProdOrderPartslistPlanWrapper _SelectedProdOrderPartslist;
        [ACPropertySelected(507, "ProdOrderPartslist")]
        public ProdOrderPartslistPlanWrapper SelectedProdOrderPartslist
        {
            get => _SelectedProdOrderPartslist;
            set
            {
                _SelectedProdOrderPartslist = value;
                OnPropertyChanged("SelectedProdOrderPartslist");
            }
        }

        private IEnumerable<ProdOrderPartslistPlanWrapper> _ProdOrderPartslistList;
        [ACPropertyList(507, "ProdOrderPartslist")]
        public IEnumerable<ProdOrderPartslistPlanWrapper> ProdOrderPartslistList
        {
            get
            {
                if (_ProdOrderPartslistList == null)
                    _ProdOrderPartslistList = new List<ProdOrderPartslistPlanWrapper>();
                return _ProdOrderPartslistList;
            }
            set
            {
                _ProdOrderPartslistList = value;
                OnPropertyChanged("ProdOrderPartslistList");
            }
        }

        public IEnumerable<ProdOrderPartslistPlanWrapper> GetProdOrderPartslistList()
        {
            if (SelectedScheduleForPWNode == null)
                return new List<ProdOrderPartslistPlanWrapper>();

            MDProdOrderState.ProdOrderStates? minProdOrderState = null;
            MDProdOrderState.ProdOrderStates? maxProdOrderState = null;
            if (FilterOrderIsCompleted != null)
            {
                if (FilterOrderIsCompleted.Value)
                    minProdOrderState = MDProdOrderState.ProdOrderStates.ProdFinished;
                else
                    maxProdOrderState = MDProdOrderState.ProdOrderStates.InProduction;
            }

            ObjectQuery<ProdOrderPartslistPlanWrapper> batchQuery =
                s_cQry_ProdOrderPartslistForPWNode(
                    DatabaseApp,
                    SelectedScheduleForPWNode.MDSchedulingGroupID,
                    FilterPlanningMR?.PlanningMRID,
                    FilterOrderStartTime,
                    FilterOrderEndTime,
                    (short?)minProdOrderState,
                    (short?)maxProdOrderState) as ObjectQuery<ProdOrderPartslistPlanWrapper>;
            batchQuery.MergeOption = MergeOption.OverwriteChanges;
            return batchQuery.Take(Const_MaxResultSize).ToList();
        }

        protected static readonly Func<DatabaseApp, Guid, Guid?, DateTime?, DateTime?, short?, short?, IQueryable<ProdOrderPartslistPlanWrapper>> s_cQry_ProdOrderPartslistForPWNode =
        CompiledQuery.Compile<DatabaseApp, Guid, Guid?, DateTime?, DateTime?, short?, short?, IQueryable<ProdOrderPartslistPlanWrapper>>(
            (ctx, mdSchedulingGroupID, planningMRID, filterStartTime, filterEndTime, minProdOrderState, maxProdOrderState) => ctx.ProdOrderPartslist
                                                        .Include("MDProdOrderState")
                                                        .Include("ProdOrder")
                                                        .Include("Partslist")
                                                        .Include("Partslist.Material")
                                                        .Include("Partslist.Material.BaseMDUnit")
                                                        .Include("Partslist.Material.MaterialUnit_Material")
                                                        .Include("Partslist.Material.MaterialUnit_Material.ToMDUnit")
                                                        .Where(c =>
                                                             (minProdOrderState == null || (c.MDProdOrderState.MDProdOrderStateIndex >= minProdOrderState && c.ProdOrder.MDProdOrderState.MDProdOrderStateIndex >= minProdOrderState))
                                                             && (maxProdOrderState == null || (c.MDProdOrderState.MDProdOrderStateIndex <= maxProdOrderState && c.ProdOrder.MDProdOrderState.MDProdOrderStateIndex <= maxProdOrderState))
                                                             && ((!planningMRID.HasValue && !c.PlanningMRProposal_ProdOrderPartslist.Any())
                                                                 || (planningMRID.HasValue && c.PlanningMRProposal_ProdOrderPartslist.Any(x => x.PlanningMRID == planningMRID))
                                                                )
                                                             && (
                                                                    filterStartTime == null
                                                                    ||
                                                                    c.InsertDate >= filterStartTime
                                                                )
                                                             && (
                                                                    filterEndTime == null
                                                                    ||
                                                                    c.InsertDate < filterEndTime
                                                                )

                                                             && c
                                                                 .Partslist
                                                                 .PartslistACClassMethod_Partslist
                                                                .Where(d => d
                                                                            .MaterialWFACClassMethod
                                                                            .ACClassMethod.ACClassWF_ACClassMethod
                                                                            .SelectMany(x => x.MDSchedulingGroupWF_VBiACClassWF)
                                                                            .Where(x => x.MDSchedulingGroupID == mdSchedulingGroupID)
                                                                            .Any()
                                                             ).Any())
                                                        .OrderByDescending(c => c.ProdOrder.ProgramNo)
                                                        .Select(c => new ProdOrderPartslistPlanWrapper()
                                                        {
                                                            ProdOrderPartslist = c,
                                                            PlannedQuantityUOM = c.ProdOrderBatchPlan_ProdOrderPartslist.Any() ? c.ProdOrderBatchPlan_ProdOrderPartslist.Sum(d => d.TotalSize) : 0.0
                                                        })
        );


        #endregion

        #endregion

        #region Properties -> Messages

        public void SendMessage(object result)
        {
            Msg msg = result as Msg;
            if (msg != null)
            {
                SendMessage(msg);
            }
        }

        /// <summary>
        /// The _ current MSG
        /// </summary>
        Msg _CurrentMsg;
        /// <summary>
        /// Gets or sets the current MSG.
        /// </summary>
        /// <value>The current MSG.</value>
        [ACPropertyCurrent(528, "Message", "en{'Message'}de{'Meldung'}")]
        public Msg CurrentMsg
        {
            get
            {
                return _CurrentMsg;
            }
            set
            {
                _CurrentMsg = value;
                OnPropertyChanged("CurrentMsg");
            }
        }

        private ObservableCollection<Msg> msgList;
        /// <summary>
        /// Gets the MSG list.
        /// </summary>
        /// <value>The MSG list.</value>
        [ACPropertyList(529, "Message", "en{'Messagelist'}de{'Meldungsliste'}")]
        public ObservableCollection<Msg> MsgList
        {
            get
            {
                if (msgList == null)
                    msgList = new ObservableCollection<Msg>();
                return msgList;
            }
        }

        public void SendMessage(Msg msg)
        {
            MsgList.Add(msg);
            OnPropertyChanged("MsgList");
        }

        public void ClearMessages()
        {
            MsgList.Clear();
            OnPropertyChanged("MsgList");
        }
        #endregion

        #region Properties -> Wizard

        #region Properties -> Wizard -> Basic

        private NewScheduledBatchWizardPhaseEnum _WizardPhase;
        [ACPropertyInfo(508, "WizardPhase", "en{'WizardPhase'}de{'WizardPhase'}")]
        public NewScheduledBatchWizardPhaseEnum WizardPhase
        {
            get
            {
                return _WizardPhase;
            }
            set
            {
                if (_WizardPhase != value)
                {
                    _WizardPhase = value;
                    OnPropertyChanged("WizardPhase");
                    OnPropertyChanged("WizardPhaseTitle");
                    OnPropertyChanged("WizardPhaseSubTitle");
                    OnPropertyChanged("WizardDesign");
                }
            }
        }

        private ACValueItemList wizardPhaseTitleList;

        [ACPropertyInfo(509, "WizardPhaseTitle", "en{'Wizard Phase'}de{'Wizard Phase'}")]

        public string WizardPhaseTitle
        {
            get
            {
                if (wizardPhaseTitleList == null)
                    wizardPhaseTitleList = LoadWizardPhaseTitleList();
                return wizardPhaseTitleList.FirstOrDefault(c => ((short)c.Value) == (short)WizardPhase).ACCaption;
            }
        }

        private ACValueItemList LoadWizardPhaseTitleList()
        {
            ACValueItemList wizardPhaseTitleList = new ACValueItemList("WizardPhaseTitleList");
            wizardPhaseTitleList.AddEntry((short)NewScheduledBatchWizardPhaseEnum.SelectMaterial, "en{'Select material'}de{'Material auswählen'}");
            wizardPhaseTitleList.AddEntry((short)NewScheduledBatchWizardPhaseEnum.SelectPartslist, "en{'Select bill of material'}de{'Stückliste auswählen'}");
            wizardPhaseTitleList.AddEntry((short)NewScheduledBatchWizardPhaseEnum.BOMExplosion, "en{'BOM-Expand'}de{'Stücklistenauflösung'}");
            wizardPhaseTitleList.AddEntry((short)NewScheduledBatchWizardPhaseEnum.PartslistForDefinition, "en{'Select & Plan Linie'}de{'Linie Auswählen und Planen'}");
            wizardPhaseTitleList.AddEntry((short)NewScheduledBatchWizardPhaseEnum.DefineBatch, "en{'Batch size suggestion'}de{'Vorschlag zur Chargengröße'}");
            wizardPhaseTitleList.AddEntry((short)NewScheduledBatchWizardPhaseEnum.DefineTargets, "en{'Batchplan'}de{'Batchplan'}");
            return wizardPhaseTitleList;
        }


        public bool IsWizard { get; set; }

        [ACPropertyInfo(515, "CurrentLayout")]
        public string CurrentLayout
        {
            get
            {
                string designName = "Batch";
                if (ParentACObject != null && ParentACObject.ACType.ACIdentifier == BSOTemplateSchedule.ClassName)
                    designName = "BatchMin";
                if (IsWizard)
                    designName = "Wizard";
                gip.core.datamodel.ACClassDesign acClassDesign = ACType.GetDesign(this, Global.ACUsages.DULayout, Global.ACKinds.DSDesignLayout, designName);
                string layoutXAML = "<vb:VBDockPanel></vb:VBDockPanel>";
                if (acClassDesign != null)
                    layoutXAML = acClassDesign.XMLDesign;
                return layoutXAML;
            }
        }

        private string _WizardPhaseErrorMessage;
        /// <summary>
        /// Selected property for 
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "WizardPhaseErrorMessage", "en{'Message'}de{'Meldung'}")]
        public string WizardPhaseErrorMessage
        {
            get
            {
                return _WizardPhaseErrorMessage;
            }
            set
            {
                if (_WizardPhaseErrorMessage != value)
                {
                    _WizardPhaseErrorMessage = value;
                    OnPropertyChanged("WizardPhaseErrorMessage");
                }
            }
        }

        #endregion

        #region Properties -> Wizard -> WizardSchedulerPartslist

        public WizardSchedulerPartslist DefaultWizardSchedulerPartslist { get; set; }

        private WizardSchedulerPartslist _SelectedWizardSchedulerPartslist;
        /// <summary>
        /// Selected property for WizardSchedulerPartslist
        /// </summary>
        /// <value>The selected WizardSchedulerPartslist</value>
        [ACPropertySelected(9999, "WizardSchedulerPartslist", "en{'TODO: WizardSchedulerPartslist'}de{'TODO: WizardSchedulerPartslist'}")]
        public WizardSchedulerPartslist SelectedWizardSchedulerPartslist
        {
            get
            {
                return _SelectedWizardSchedulerPartslist;
            }
            set
            {
                if (_SelectedWizardSchedulerPartslist != value)
                {
                    _SelectedWizardSchedulerPartslist = value;
                    OnSelectedWizardSchedulerPartslistChanged();
                }
            }
        }

        public void OnSelectedWizardSchedulerPartslistChanged()
        {
            OnPropertyChanged("SelectedWizardSchedulerPartslist");
            OnPropertyChanged("SelectedWizardSchedulerPartslist\\TargetQuantity");
            OnPropertyChanged("SelectedWizardSchedulerPartslist\\TargetQuantityUOM");
        }

        private List<WizardSchedulerPartslist> _AllWizardSchedulerPartslistList;

        protected List<WizardSchedulerPartslist> AllWizardSchedulerPartslistList
        {
            get
            {
                if (_AllWizardSchedulerPartslistList == null)
                    _AllWizardSchedulerPartslistList = new List<WizardSchedulerPartslist>();
                return _AllWizardSchedulerPartslistList;
            }
            set
            {
                _AllWizardSchedulerPartslistList = value;
            }
        }

        public void AddWizardSchedulerPartslistList(WizardSchedulerPartslist item, int? index = null)
        {
            if (index == null)
                AllWizardSchedulerPartslistList.Add(item);
            else
                AllWizardSchedulerPartslistList.Insert(index.Value, item);
            item.PropertyChanged -= Item_PropertyChanged;
            item.PropertyChanged += Item_PropertyChanged;
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ClearMessages();
            WizardSchedulerPartslist item = sender as WizardSchedulerPartslist;
            if (item != null)
                if (e.PropertyName == WizardSchedulerPartslist.Const_SelectedMDSchedulingGroup)
                {
                    LoadConfiguration(item);
                }
                else if (e.PropertyName == WizardSchedulerPartslist.Const_NewTargetQuantityUOM)
                {
                    // do nothing
                }
                else if (e.PropertyName == WizardSchedulerPartslist.Const_NewSyncTargetQuantityUOM)
                {
                    if (item.NewSyncTargetQuantityUOM != null)
                    {
                        // make recalc
                        double factor = item.NewSyncTargetQuantityUOM.Value / item.NewTargetQuantityUOM;
                        item._NewTargetQuantityUOM = item.NewSyncTargetQuantityUOM.Value;

                        foreach (WizardSchedulerPartslist otherItem in AllWizardSchedulerPartslistList)
                            if (!item.IsEqualPartslist(otherItem))
                                otherItem._NewTargetQuantityUOM = otherItem.NewTargetQuantityUOM * factor;

                        // setup value to null
                        item._NewSyncTargetQuantityUOM = null;
                        foreach (WizardSchedulerPartslist wizardItem in AllWizardSchedulerPartslistList)
                        {
                            if (wizardItem.ProdOrderPartslistPos != null)
                            {
                                Msg changeMsg = ProdOrderManager.ProdOrderPartslistChangeTargetQuantity(DatabaseApp, wizardItem.ProdOrderPartslistPos.ProdOrderPartslist, wizardItem.NewTargetQuantityUOM);
                                if (changeMsg != null)
                                    SendMessage(changeMsg);
                            }
                        }
                        if (DatabaseApp.IsChanged)
                        {
                            MsgWithDetails saveMsg = DatabaseApp.ACSaveChanges();
                            foreach (WizardSchedulerPartslist wizardItem in AllWizardSchedulerPartslistList)
                            {
                                if (!IsBSOTemplateScheduleParent && wizardItem.SelectedMDSchedulingGroup != null)
                                    RefreshServerState(wizardItem.SelectedMDSchedulingGroup.MDSchedulingGroupID);
                            }
                            if (saveMsg != null)
                                SendMessage(saveMsg);
                        }
                        OnPropertyChanged("WizardSchedulerPartslistList");
                    }
                }
        }

        /// <summary>
        /// List property for WizardSchedulerPartslist
        /// </summary>
        /// <value>The WizardSchedulerPartslist list</value>
        [ACPropertyList(9999, "WizardSchedulerPartslist")]
        public List<WizardSchedulerPartslist> WizardSchedulerPartslistList
        {
            get
            {
                return AllWizardSchedulerPartslistList.Where(c => !c.IsSolved || IsWizardExistingBatch).ToList();
            }
        }

        #endregion

        #region Properties -> Wizard -> PartListExpand

        private PartslistExpand rootPartslistExpand;
        private PartslistExpand _CurrentPartListExpand;
        /// <summary>
        /// 
        /// </summary>
        [ACPropertyCurrent(606, "PartListExpand")]
        public PartslistExpand CurrentPartListExpand
        {
            get
            {
                return _CurrentPartListExpand;
            }
            set
            {
                if (_CurrentPartListExpand != value)
                {
                    _CurrentPartListExpand = value;
                    OnPropertyChanged("CurrentPartListExpand");
                }
            }
        }

        private List<PartslistExpand> _PartListExpandList;
        [ACPropertyList(607, "PartListExpand")]
        public List<PartslistExpand> PartListExpandList
        {
            get
            {
                return _PartListExpandList;
            }
        }

        #endregion

        #region Properties -> Wizard ->   BatchPlanSuggestion

        public BatchSuggestionCommandModeEnum? FilterBatchplanSuggestionMode
        {
            get
            {
                if (SelectedFilterBatchplanSuggestionMode == null) return null;
                return (BatchSuggestionCommandModeEnum)SelectedFilterBatchplanSuggestionMode.Value;
            }
        }

        private ACValueItem _SelectedFilterBatchplanSuggestionMode;
        [ACPropertySelected(516, "FilterBatchplanSuggestionMode", "en{'Calculation formula'}de{'Berechnungsformel'}")]
        public ACValueItem SelectedFilterBatchplanSuggestionMode
        {
            get
            {
                return _SelectedFilterBatchplanSuggestionMode;
            }
            set
            {
                if (_SelectedFilterBatchplanSuggestionMode != value)
                {
                    _SelectedFilterBatchplanSuggestionMode = value;
                    OnPropertyChanged("SelectedFilterBatchplanSuggestionMode");
                }
            }
        }

        private ACValueItemList _FilterBatchplanSuggestionModeList;
        [ACPropertyList(517, "FilterBatchplanSuggestionMode")]
        public ACValueItemList FilterBatchplanSuggestionModeList
        {
            get
            {
                if (_FilterBatchplanSuggestionModeList == null)
                {
                    _FilterBatchplanSuggestionModeList = new ACValueListBatchSuggestionCommandModeEnum();
                }
                return _FilterBatchplanSuggestionModeList;
            }
        }

        [ACMethodInfo("RecalculateBatchSuggestion", "en{'Calculate'}de{'Berechnung'}", 999)]
        public void RecalculateBatchSuggestion()
        {
            if (!IsEnabledRecalculateBatchSuggestion())
                return;
            BatchSuggestionCommand cmd = new BatchSuggestionCommand(SelectedWizardSchedulerPartslist, FilterBatchplanSuggestionMode.Value, ProdOrderManager.TolRemainingCallQ);
            OnPropertyChanged("SelectedWizardSchedulerPartslist\\BatchPlanSuggestion");
            OnPropertyChanged("SelectedWizardSchedulerPartslist\\BatchPlanSuggestion\\TotalSize");
            OnPropertyChanged("SelectedWizardSchedulerPartslist\\BatchPlanSuggestion\\BatchTargetCount");
            OnPropertyChanged("SelectedWizardSchedulerPartslist\\BatchPlanSuggestion\\BatchSize");
            OnPropertyChanged("SelectedWizardSchedulerPartslist\\BatchPlanSuggestion\\ItemsList");
            OnPropertyChanged("SelectedWizardSchedulerPartslist\\BatchPlanSuggestion\\SelectedItems");
        }

        public bool IsEnabledRecalculateBatchSuggestion()
        {
            return
                    FilterBatchplanSuggestionMode != null
                    && SelectedWizardSchedulerPartslist != null
                    && SelectedWizardSchedulerPartslist.TargetQuantityUOM > Double.Epsilon
                    && SelectedWizardSchedulerPartslist.BatchPlanSuggestion != null
                    && (
                            SelectedWizardSchedulerPartslist.BatchPlanSuggestion == null
                            || SelectedWizardSchedulerPartslist.BatchPlanSuggestion.ItemsList == null
                            || !SelectedWizardSchedulerPartslist.BatchPlanSuggestion.ItemsList.Any(c => c.ProdOrderBatchPlan != null && c.ProdOrderBatchPlan.PlanStateIndex >= (short)GlobalApp.BatchPlanState.AutoStart)
                       );
        }

        [ACMethodInfo("AddSuggestion", "en{'Add'}de{'Neu'}", 999)]
        public void AddSuggestion()
        {
            if (!IsEnabledAddSuggestion())
                return;
            int nr = 0;
            if (SelectedWizardSchedulerPartslist.BatchPlanSuggestion.ItemsList.Any())
                nr = SelectedWizardSchedulerPartslist.BatchPlanSuggestion.ItemsList.Max(c => c.Nr);
            nr++;
            BatchPlanSuggestionItem newItem = new BatchPlanSuggestionItem(SelectedWizardSchedulerPartslist, nr, 0, 0, 
                SelectedWizardSchedulerPartslist.NewTargetQuantityUOM  > 0 ? SelectedWizardSchedulerPartslist.NewTargetQuantityUOM: SelectedWizardSchedulerPartslist.TargetQuantityUOM) { IsEditable = true };
            SelectedWizardSchedulerPartslist.BatchPlanSuggestion.AddItem(newItem);
            SelectedWizardSchedulerPartslist.BatchPlanSuggestion.SelectedItems = newItem;
            if (SelectedWizardSchedulerPartslist.OffsetToEndTime != null)
                SelectedWizardSchedulerPartslist.LoadSuggestionItemExpectedBatchEndTime();
            OnPropertyChanged("SelectedWizardSchedulerPartslist\\BatchPlanSuggestion\\ItemsList");
        }

        public bool IsEnabledAddSuggestion()
        {
            return
                SelectedWizardSchedulerPartslist != null
                 && SelectedWizardSchedulerPartslist.BatchPlanSuggestion != null
                 && (SelectedWizardSchedulerPartslist.BatchPlanSuggestion.ItemsList == null
                 || SelectedWizardSchedulerPartslist.ProdOrderPartslistPos == null
                 || SelectedWizardSchedulerPartslist.ProdOrderPartslistPos == null
                 || !(
                        SelectedWizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderPartslist.MDProdOrderState.ProdOrderState >= MDProdOrderState.ProdOrderStates.ProdFinished
                        || SelectedWizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.MDProdOrderState.ProdOrderState >= MDProdOrderState.ProdOrderStates.ProdFinished
                    ));
        }

        [ACMethodInfo("RemoveSuggestion", "en{'Delete'}de{'Löschen'}", 999)]
        public void RemoveSuggestion()
        {
            if (!IsEnabledRemoveSuggestion())
                return;
            SelectedWizardSchedulerPartslist.BatchPlanSuggestion.ItemsList.Remove(SelectedWizardSchedulerPartslist.BatchPlanSuggestion.SelectedItems);
            int nr = 0;
            foreach (var item in SelectedWizardSchedulerPartslist.BatchPlanSuggestion.ItemsList)
            {
                nr++;
                item.Nr = nr;
            }
            SelectedWizardSchedulerPartslist.BatchPlanSuggestion.SelectedItems = SelectedWizardSchedulerPartslist.BatchPlanSuggestion.ItemsList.FirstOrDefault();
            OnPropertyChanged("SelectedWizardSchedulerPartslist\\BatchPlanSuggestion\\ItemsList");
            OnPropertyChanged("SelectedWizardSchedulerPartslist\\BatchPlanSuggestion\\SelectedItems");
        }

        public bool IsEnabledRemoveSuggestion()
        {
            return
                SelectedWizardSchedulerPartslist != null
                && SelectedWizardSchedulerPartslist.BatchPlanSuggestion != null
                && SelectedWizardSchedulerPartslist.BatchPlanSuggestion.ItemsList != null
                && SelectedWizardSchedulerPartslist.BatchPlanSuggestion.SelectedItems != null
                && !SelectedWizardSchedulerPartslist.BatchPlanSuggestion.SelectedItems.IsInProduction
                && SelectedWizardSchedulerPartslist.BatchPlanSuggestion.SelectedItems.IsEditable;
        }


        #endregion

        #endregion

        #region Properties -> BatchPlanTimelinePresenter

        private IEnumerable<BatchPlanTimelineItem> _BatchPlanTimelineItemsRoot;
        /// <summary>
        /// Gets or sets the ACPropertyLogsRoot. Represents the list for treeListView control.
        /// </summary>
        [ACPropertyList(519, "BatchPlanTimelineItemsRoot")]
        public IEnumerable<BatchPlanTimelineItem> BatchPlanTimelineItemsRoot
        {
            get
            {
                return _BatchPlanTimelineItemsRoot;
            }
            set
            {
                _BatchPlanTimelineItemsRoot = value;
                OnPropertyChanged("BatchPlanTimelineItemsRoot");
            }
        }

        private BatchPlanTimelineItem _SelectedBatchPlanTimelineItemRoot;
        /// <summary>
        /// Gets or sets the selected PropertyLog root. (Selected in treeListView control.)
        /// </summary>
        [ACPropertyCurrent(520, "BatchPlanTimelineItemsRoot")]
        public BatchPlanTimelineItem SelectedBatchPlanTimelineItemRoot
        {
            get
            {
                return _SelectedBatchPlanTimelineItemRoot;
            }
            set
            {
                _SelectedBatchPlanTimelineItemRoot = value;
                OnPropertyChanged("SelectedBatchPlanTimelineItemRoot");
            }
        }

        private ObservableCollection<BatchPlanTimelineItem> _BatchPlanTimelineItems;
        /// <summary>
        /// Gets or sets the ACPropertyLogs. Represents the collection for timeline view control. Contains all timeline items(ACPropertyLogModel).
        /// </summary>
        [ACPropertyList(521, "BatchPlanTimelineItems")]
        public ObservableCollection<BatchPlanTimelineItem> BatchPlanTimelineItems
        {
            get
            {

                return _BatchPlanTimelineItems;
            }
            set
            {
                _BatchPlanTimelineItems = value;
                OnPropertyChanged("BatchPlanTimelineItems");
            }
        }

        private BatchPlanTimelineItem _SelectedBatchPlanTimelineItem;
        /// <summary>
        /// Gets or sets the seelcted PropertyLog. (Selected in the timeline view control)
        /// </summary>
        [ACPropertyCurrent(522, "BatchPlanTimelineItems")]
        public BatchPlanTimelineItem SelectedBatchPlanTimelineItem
        {
            get
            {
                return _SelectedBatchPlanTimelineItem;
            }
            set
            {
                _SelectedBatchPlanTimelineItem = value;
                OnPropertyChanged("SelectedBatchPlanTimelineItem");
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Methods -> ACMethod

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand("BatchPlanList", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            if (!PreExecute("Save")) return;
            OnSave();
            PostExecute("Save");
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        protected override void OnPostSave()
        {
            if (SelectedScheduleForPWNode != null && ParentACObject != null && ParentACObject.ACType.ACIdentifier != BSOTemplateSchedule.ClassName)
            {
                RefreshServerState(SelectedScheduleForPWNode);
            }
            base.OnPostSave();
        }


        public void RefreshServerState(Guid mdSchedulingGroupID)
        {
            PAScheduleForPWNode pAScheduleForPWNode = ScheduleForPWNodeList.FirstOrDefault(c => c.MDSchedulingGroupID == mdSchedulingGroupID);
            if (pAScheduleForPWNode != null)
                RefreshServerState(pAScheduleForPWNode);
        }

        public void RefreshServerState(PAScheduleForPWNode pAScheduleForPWNode)
        {
            pAScheduleForPWNode.RefreshCounter++;
            var result = BatchPlanScheduler.ExecuteMethod(PABatchPlanScheduler.MN_UpdateScheduleFromClient, new object[] { pAScheduleForPWNode });
            if (result != null)
            {
                SendMessage(result);
            }
        }


        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand("BatchPlanList", "en{'Undo'}de{'Rückgängig'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            if (!PreExecute("UndoSave")) return;
            OnUndoSave();
            PostExecute("UndoSave");
        }

        [ACMethodInteraction(Partslist.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedProdOrderBatchPlan", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (requery)
            {
                OnPropertyChanged("ScheduleForPWNodeList");
                LoadProdOrderBatchPlanList();
            }
        }

        public bool IsEnabledLoad()
        {
            return SelectedScheduleForPWNode != null;
        }
        #endregion

        #region Methods -> Explorer

        #region Methods -> Explorer -> ChangeMode

        [ACMethodInfo("ChangeMode", "en{'Change Mode'}de{'Mode ändern'}", 501)]
        public void ChangeMode()
        {
            if (!IsEnabledChangeMode())
                return;
            PAScheduleForPWNode updateNode = new PAScheduleForPWNode();
            updateNode.CopyFrom(SelectedScheduleForPWNode, true);
            updateNode.StartMode = (vd.GlobalApp.BatchPlanStartModeEnum)SelectedFilterBatchPlanStartMode.Value;
            var result = BatchPlanScheduler.ExecuteMethod(PABatchPlanScheduler.MN_UpdateScheduleFromClient, new object[] { updateNode });
            if (result != null)
            {
                SendMessage(result);
            }
        }

        public bool IsEnabledChangeMode()
        {
            return
                SelectedScheduleForPWNode != null &&
                BatchPlanScheduler != null &&
                SelectedFilterBatchPlanStartMode != null &&
                _SchedulesForPWNodesProp != null;
        }

        #endregion

        #region Methods -> Explorer -> MoveToOtherLine

        [ACMethodInteraction("MoveToOtherLine", "en{'Move'}de{'Verlagern'}", (short)MISort.MovedData, false, "TargetScheduleForPWNode")]
        public void MoveToOtherLine()
        {
            if (!IsEnabledMoveToOtherLine())
                return;
            ClearMessages();
            bool isMovingValueValid = false;
            bool isMove = false;
            if (SelectedProdOrderBatchPlan.PlanMode == GlobalApp.BatchPlanMode.UseBatchCount)
            {
                int diffBatchCount = SelectedProdOrderBatchPlan.BatchTargetCount - SelectedProdOrderBatchPlan.BatchActualCount;
                // "Question50045" Please enter the number of batches you want to move to {0}:
                string header = Root.Environment.TranslateMessage(this, "Question50045", SelectedTargetScheduleForPWNode.MDSchedulingGroup.ACCaption);
                string enteredValue = Messages.InputBox(header, diffBatchCount.ToString());
                if (!string.IsNullOrEmpty(enteredValue))
                {
                    int moveBatchCount = BatchCountValidate(enteredValue, SelectedProdOrderBatchPlan.BatchTargetCount);
                    isMovingValueValid = moveBatchCount > 0;
                    if (isMovingValueValid)
                    {
                        if (moveBatchCount == SelectedProdOrderBatchPlan.BatchTargetCount)
                        {
                            MoveBatchToOtherProdLine(SelectedProdOrderBatchPlan, SelectedTargetScheduleForPWNode);
                            isMove = true;
                        }
                        else
                        {
                            int newBatchCount = SelectedProdOrderBatchPlan.BatchTargetCount - moveBatchCount;
                            SelectedProdOrderBatchPlan.BatchTargetCount = newBatchCount;
                            CreateNewBatchWithCount(SelectedProdOrderBatchPlan, moveBatchCount, SelectedTargetScheduleForPWNode);
                        }
                    }
                }
            }
            else if (SelectedProdOrderBatchPlan.PlanMode == GlobalApp.BatchPlanMode.UseTotalSize)
            {
                // "Question50046" Please enter the quantity you want to move to {0}:

                string header = Root.Environment.TranslateMessage(this, "Question50046", SelectedTargetScheduleForPWNode.MDSchedulingGroup.ACCaption);
                string enteredValue = Messages.InputBox(header, SelectedProdOrderBatchPlan.RemainingQuantity.ToString());
                if (string.IsNullOrEmpty(enteredValue))
                {
                    double moveQuantity = BatchSizeValidate(enteredValue, SelectedProdOrderBatchPlan.BatchSize, SelectedProdOrderBatchPlan.ProdOrderPartslist.Partslist);
                    isMovingValueValid = moveQuantity > Double.Epsilon;
                    if (isMovingValueValid)
                    {
                        if (moveQuantity == SelectedProdOrderBatchPlan.BatchSize)
                        {
                            MoveBatchToOtherProdLine(SelectedProdOrderBatchPlan, SelectedTargetScheduleForPWNode);
                            isMove = true;
                        }
                        else
                        {
                            double newBatchSize = SelectedProdOrderBatchPlan.BatchSize - moveQuantity;
                            SelectedProdOrderBatchPlan.BatchSize = newBatchSize;
                            CreateNewBatchWithSize(SelectedProdOrderBatchPlan, moveQuantity, SelectedTargetScheduleForPWNode);
                        }
                    }
                }
            }
            if (isMovingValueValid)
            {
                bool isSaveValid = true;
                MsgWithDetails saveBatchPlanMoveMsg = DatabaseApp.ACSaveChanges();
                if (saveBatchPlanMoveMsg == null || saveBatchPlanMoveMsg.IsSucceded())
                {
                    // Correct the SortOrder from ProdOrderBatchPlan
                    ProdOrderBatchPlanList = GetProdOrderBatchPlanList(SelectedScheduleForPWNode?.MDSchedulingGroupID);
                    if (ProdOrderBatchPlanList != null && ProdOrderBatchPlanList.Any() && isMove)
                    {
                        MoveBatchSortOrderCorrect(ProdOrderBatchPlanList);
                        MsgWithDetails saveOrderNoMsg = DatabaseApp.ACSaveChanges();
                        if (saveOrderNoMsg != null && !saveOrderNoMsg.IsSucceded())
                        {
                            SendMessage(saveOrderNoMsg);
                            DatabaseApp.ACUndoChanges();
                        }
                    }
                }
                else
                {
                    isSaveValid = false;
                    SendMessage(saveBatchPlanMoveMsg);
                    DatabaseApp.ACUndoChanges();
                }
                if (isSaveValid)
                {
                    if (!IsBSOTemplateScheduleParent)
                    {
                        RefreshServerState(SelectedScheduleForPWNode.MDSchedulingGroupID);
                        RefreshServerState(SelectedTargetScheduleForPWNode.MDSchedulingGroupID);
                    }
                    SelectedProdOrderBatchPlan = ProdOrderBatchPlanList.FirstOrDefault();
                    OnPropertyChanged("ProdOrderBatchPlanList");
                }
            }
        }


        public bool IsEnabledMoveToOtherLine()
        {
            return SelectedProdOrderBatchPlan != null
                && SelectedProdOrderBatchPlan.ProdOrderPartslist != null
                && SelectedProdOrderBatchPlan.PlanState < GlobalApp.BatchPlanState.Completed
                && (
                       (SelectedProdOrderBatchPlan.PlanMode == GlobalApp.BatchPlanMode.UseBatchCount
                        && SelectedProdOrderBatchPlan.BatchActualCount < SelectedProdOrderBatchPlan.BatchTargetCount)
                    || (SelectedProdOrderBatchPlan.PlanMode == GlobalApp.BatchPlanMode.UseTotalSize
                        && SelectedProdOrderBatchPlan.RemainingQuantity > 0.00001)
                    )
                && SelectedTargetScheduleForPWNode != null
                && SelectedTargetScheduleForPWNode.MDSchedulingGroup != null;
        }

        #region Methods -> Explorer -> MoveToOtherLine -> Helper methods


        private void MoveBatchToOtherProdLine(ProdOrderBatchPlan prodOrderBatchPlan, PAScheduleForPWNode selectedTargetScheduleForPWNode)
        {
            var otherLinieBatches = GetProdOrderBatchPlanList(selectedTargetScheduleForPWNode.MDSchedulingGroupID);
            int scheduledOrderNo = 0;
            if (otherLinieBatches != null && otherLinieBatches.Any())
                scheduledOrderNo = otherLinieBatches.Count();
            scheduledOrderNo++;
            vd.ACClassWF tempACClassWFItem = selectedTargetScheduleForPWNode.MDSchedulingGroup.MDSchedulingGroupWF_MDSchedulingGroup.Select(c => c.VBiACClassWF).FirstOrDefault();
            core.datamodel.ACClassWF aCClassWF = tempACClassWFItem.FromIPlusContext<core.datamodel.ACClassWF>(DatabaseApp.ContextIPlus);
            prodOrderBatchPlan.VBiACClassWF = tempACClassWFItem;
            prodOrderBatchPlan.ScheduledOrder = scheduledOrderNo;
        }

        private void CreateNewBatchWithCount(ProdOrderBatchPlan prodOrderBatchPlan, int moveBatchCount, PAScheduleForPWNode selectedTargetScheduleForPWNode)
        {
            double totalSize = moveBatchCount * prodOrderBatchPlan.BatchSize;
            int sn = 1;
            WizardSchedulerPartslist wizardSchedulerPartslist = GetWizardSchedulerPartslist(prodOrderBatchPlan.ProdOrderPartslist.Partslist, totalSize, sn, new List<MDSchedulingGroup>() { selectedTargetScheduleForPWNode.MDSchedulingGroup });
            wizardSchedulerPartslist.MDProdOrderState = prodOrderBatchPlan.ProdOrderPartslist.MDProdOrderState;
            wizardSchedulerPartslist.ProdOrderPartslistPos = prodOrderBatchPlan.ProdOrderPartslistPos;
            if (wizardSchedulerPartslist.SelectedMDSchedulingGroup != null)
                LoadConfiguration(wizardSchedulerPartslist);
            wizardSchedulerPartslist.BatchPlanSuggestion =
               new BatchPlanSuggestion(wizardSchedulerPartslist)
               {
                   RestQuantityToleranceUOM = totalSize * (ProdOrderManager.TolRemainingCallQ / 100),
                   RestNotUsedQuantityUOM = 0,
                   ItemsList = new BindingList<BatchPlanSuggestionItem>()
                   {
                        new BatchPlanSuggestionItem(wizardSchedulerPartslist,1, prodOrderBatchPlan.BatchSize, moveBatchCount, totalSize){IsEditable = true}
                   }
               };
            string programNo = prodOrderBatchPlan.ProdOrderPartslist.ProdOrder.ProgramNo;
            FactoryBatchPlans(wizardSchedulerPartslist, ref programNo);
        }

        private void CreateNewBatchWithSize(ProdOrderBatchPlan prodOrderBatchPlan, double moveQuantity, PAScheduleForPWNode selectedTargetScheduleForPWNode)
        {
            int sn = 1;
            WizardSchedulerPartslist wizardSchedulerPartslist = GetWizardSchedulerPartslist(prodOrderBatchPlan.ProdOrderPartslist.Partslist, moveQuantity, sn, new List<MDSchedulingGroup>() { selectedTargetScheduleForPWNode.MDSchedulingGroup });
            if (wizardSchedulerPartslist.SelectedMDSchedulingGroup != null)
                LoadConfiguration(wizardSchedulerPartslist);

            wizardSchedulerPartslist.BatchPlanSuggestion =
                new BatchPlanSuggestion(wizardSchedulerPartslist)
                {
                    RestQuantityToleranceUOM = (ProdOrderManager.TolRemainingCallQ / 100) * moveQuantity,
                    RestNotUsedQuantityUOM = 0,
                    ItemsList = new BindingList<BatchPlanSuggestionItem>()
                    {
                        new BatchPlanSuggestionItem(wizardSchedulerPartslist, 1, moveQuantity, 1, moveQuantity){IsEditable = true }
                    }
                };
            string programNo = "";
            FactoryBatchPlans(wizardSchedulerPartslist, ref programNo);
        }

        private void MoveBatchSortOrderCorrect(ObservableCollection<ProdOrderBatchPlan> prodOrderBatchPlans)
        {
            int nr = 0;
            foreach (var item in prodOrderBatchPlans)
            {
                nr++;
                item.ScheduledOrder = nr;
            }
        }

        private int BatchCountValidate(string enteredValue, int batchTargetCount)
        {
            int moveBatchCount = 0;
            if (!Int32.TryParse(enteredValue, out moveBatchCount))
            {
                // Error50393
                Msg msg = new Msg(this, eMsgLevel.Error, GetACUrl(), "MoveToOtherLine", 1839, "Error50393");
                Root.Messages.Msg(msg);
                SendMessage(msg);
            }
            if (moveBatchCount <= 0 || moveBatchCount > batchTargetCount)
            {
                //Error50394.
                Msg msg = new Msg(this, eMsgLevel.Error, GetACUrl(), "MoveToOtherLine", 1847, "Error50394", batchTargetCount);
                Root.Messages.Msg(msg);
                SendMessage(msg);
                moveBatchCount = 0;
            }
            return moveBatchCount;
        }

        private double BatchSizeValidate(string enteredValue, double targetQuantity, Partslist partslist)
        {
            double moveQuantity = 0;
            if (!Double.TryParse(enteredValue, out moveQuantity))
            {
                // Error50395
                Msg msg = new Msg(this, eMsgLevel.Error, GetACUrl(), "MoveToOtherLine", 1885, "Error50395");
                Root.Messages.Msg(msg);
                SendMessage(msg);
            }
            if (moveQuantity <= Double.Epsilon || moveQuantity > targetQuantity)
            {
                //Error50396
                Msg msg = new Msg(this, eMsgLevel.Error, GetACUrl(), "MoveToOtherLine", 1894, "Error50396", targetQuantity);
                Root.Messages.Msg(msg);
                SendMessage(msg);
                moveQuantity = 0;
            }
            else
            {
                WizardSchedulerPartslist tmpWizardPl = GetWizardSchedulerPartslist(partslist, moveQuantity, 1, new List<MDSchedulingGroup>() { SelectedTargetScheduleForPWNode.MDSchedulingGroup });
                if (tmpWizardPl.SelectedMDSchedulingGroup != null)
                    LoadConfiguration(tmpWizardPl);
                if (tmpWizardPl.BatchSizeMin > 0)
                {
                    if (moveQuantity < tmpWizardPl.BatchSizeMin)
                    {
                        //Error50397
                        Msg msg = new Msg(this, eMsgLevel.Error, GetACUrl(), "MoveToOtherLine", 1908, "Error50397", tmpWizardPl.BatchSizeMin, moveQuantity);
                        Root.Messages.Msg(msg);
                        SendMessage(msg);
                        moveQuantity = 0;
                    }
                }
            }
            return moveQuantity;
        }

        #endregion

        #endregion

        #endregion

        #region Methods -> (Tab)BatchPlanScheduler

        #region Methods -> (Tab)BatchPlanScheduler -> ResetFilter

        /// <summary>
        /// Method ResetFilterStartTime
        /// </summary>
        [ACMethodInfo("ResetFilterStartTime", "en{'Set / Reset'}de{'Setzen / Zurücksetzen'}", 700)]
        public void ResetFilterStartTime()
        {
            if (!IsEnabledResetFilterStartTime())
                return;
            FilterStartTime = FilterStartTime == null ? (DateTime?)DateTime.Now : null;
        }

        public bool IsEnabledResetFilterStartTime()
        {
            return true;
        }

        /// <summary>
        /// Method ResetFilterEndTime
        /// </summary>
        [ACMethodInfo("ResetFilterEndTime", "en{'Set / Reset'}de{'Setzen / Zurücksetzen'}", 701)]
        public void ResetFilterEndTime()
        {
            if (!IsEnabledResetFilterEndTime())
                return;
            if (FilterEndTime != null)
                FilterEndTime = null;
            else if (FilterStartTime != null)
                FilterEndTime = FilterStartTime.Value.AddDays(1);
            else
                FilterEndTime = null;
        }

        public bool IsEnabledResetFilterEndTime()
        {
            return true;
        }


        #endregion

        #region Methods -> (Tab)BatchPlanScheduler -> New, Delete, Search
        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("Search", "en{'Search'}de{'Suche'}", (short)MISort.Search)]
        public void Search()
        {
            if (!IsEnabledSearch())
                return;
            OnPropertyChanged("ScheduleForPWNodeList");
            LoadProdOrderBatchPlanList();
        }

        public bool IsEnabledSearch()
        {
            return
                SelectedScheduleForPWNode != null
                &&
                (
                    SelectedFilterConnectedLine != null
                    ||
                    (
                        FilterStartTime != null
                        && FilterEndTime != null
                        && (FilterEndTime.Value - FilterStartTime.Value).TotalDays > 0
                        && (FilterEndTime.Value - FilterStartTime.Value).TotalDays <= Const_MaxFilterDaySpan
                    )
                 );
        }

        [ACMethodCommand("New", "en{'New'}de{'Neu'}", (short)MISort.New)]
        public void New()
        {
            if (!PreExecute("New"))
                return;
            if (!IsEnabledNew())
                return;
            ClearMessages();
            ClearAndResetChildBSOLists(SelectedScheduleForPWNode.MDSchedulingGroup);
            WizardPhase = NewScheduledBatchWizardPhaseEnum.SelectMaterial;
            WizardForwardAction(WizardPhase);
            IsWizard = true;
            OnPropertyChanged("CurrentLayout");
            PostExecute("New");
        }

        public bool IsEnabledNew()
        {
            return
                BSOPartslistExplorer_Child != null
                && BSOPartslistExplorer_Child.Value != null
                && BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child != null
                && BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value != null
                && SelectedScheduleForPWNode != null;
        }

        [ACMethodInfo("DeleteBatch", "en{'Delete'}de{'Löschen'}", 503)]
        public void DeleteBatch()
        {
            ProdOrderBatchPlan batchPlan = SelectedProdOrderBatchPlan;
            MsgWithDetails msg = batchPlan.DeleteACObject(DatabaseApp, false);
            if (msg == null || msg.IsSucceded())
            {
                if (_ProdOrderBatchPlanList != null)
                    _ProdOrderBatchPlanList.Remove(batchPlan);
                OnPropertyChanged("ProdOrderBatchPlanList");
                Save();
            }
            else
            {
                MsgList.Add(msg);
            }
        }

        public bool IsEnabledDeleteBatch()
        {
            return SelectedProdOrderBatchPlan != null
                && SelectedProdOrderBatchPlan.PlanState == GlobalApp.BatchPlanState.Created
                && SelectedProdOrderBatchPlan.BatchActualCount <= 0;
        }

        #endregion

        #region Methods -> (Tab)BatchPlanScheduler -> Interaction

        [ACMethodInfo("ItemDrag", "en{'Cancel'}de{'Abbrechen'}", 506)]
        public void ItemDrag(Dictionary<int, string> newOrder)
        {
            if (!IsEnabledItemDrag()) return;
            Dictionary<int, Guid> revisitedNewOrder = newOrder.ToDictionary(key => key.Key, val => new Guid(val.Value));
            var batchPlanList = ProdOrderBatchPlanList.ToList();
            foreach (var item in revisitedNewOrder)
            {
                vd.ProdOrderBatchPlan prodOrderBatchPlan = batchPlanList.FirstOrDefault(c => c.ProdOrderBatchPlanID == item.Value);
                if (prodOrderBatchPlan != null && prodOrderBatchPlan.ScheduledOrder != item.Key)
                {
                    prodOrderBatchPlan.ScheduledOrder = item.Key;
                    prodOrderBatchPlan.OnEntityPropertyChanged("ScheduledOrder");
                }
            }
            OnPropertyChanged("ProdOrderBatchPlanList");
        }

        public bool IsEnabledItemDrag()
        {
            return SelectedScheduleForPWNode != null;
        }

        [ACMethodInteraction("ProdOrderBatchPlan", "en{'Show Order'}de{'Auftrag anzeigen'}", 502, false, "SelectedProdOrderBatchPlan")]
        public void NavigateToProdOrder()
        {
            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(
                new PAOrderInfoEntry()
                {
                    EntityID = SelectedProdOrderBatchPlan.ProdOrderPartslistID,
                    EntityName = ProdOrderPartslist.ClassName
                });
                if (SelectedProdOrderBatchPlan.ProdOrderPartslist.PlanningMRProposal_ProdOrderPartslist.Any())
                {
                    info.Entities.Add(
                    new PAOrderInfoEntry()
                    {
                        EntityID = SelectedProdOrderBatchPlan.ProdOrderPartslist.PlanningMRProposal_ProdOrderPartslist.Select(c => c.PlanningMRID).FirstOrDefault(),
                        EntityName = PlanningMR.ClassName
                    });
                }
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToProdOrder()
        {
            return SelectedProdOrderBatchPlan != null && SelectedProdOrderBatchPlan.ProdOrderPartslist != null;
        }


        #endregion

        #region Methods -> (Tab)BatchPlanScheduler -> Scheduling

        [ACMethodInfo("BackwardScheduling", "en{'Backward scheduling'}de{'Rückwärtsterminierung'}", 506)]
        public void BackwardScheduling()
        {
            if (!IsEnabledBackwardScheduling()) return;
            ShowDialog(this, "DlgBackwardScheduling");
        }

        public bool IsEnabledBackwardScheduling()
        {
            return IsEnabledScheduling();
        }

        [ACMethodInfo("BackwardSchedulingOk", "en{'Ok'}de{'Ok'}", 507)]
        public void BackwardSchedulingOk()
        {
            if (!IsEnabledBackwardScheduling() || !IsEnabledBackwardSchedulingOk()) return;
            CloseTopDialog();
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoBackwardScheduling);
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledBackwardSchedulingOk()
        {
            return ScheduledEndDate != null && IsEnabledScheduling();
        }

        [ACMethodInfo("ForwardScheduling", "en{'Forward scheduling'}de{'Vorwärtsterminierung'}", 508)]
        public void ForwardScheduling()
        {
            if (!IsEnabledForwardScheduling()) return;
            ShowDialog(this, "DlgForwardScheduling");

        }

        public bool IsEnabledForwardScheduling()
        {
            return IsEnabledScheduling();
        }

        [ACMethodInfo("ForwardSchedulingOk", "en{'Ok'}de{'Ok'}", 509)]
        public void ForwardSchedulingOk()
        {
            if (!IsEnabledForwardScheduling() || !IsEnabledForwardSchedulingOk()) return;
            CloseTopDialog();
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoForwardScheduling);
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledForwardSchedulingOk()
        {
            return ScheduledStartDate != null && IsEnabledScheduling();
        }

        [ACMethodInfo("SchedulingCancel", "en{'Cancel'}de{'Abbrechen'}", 510)]
        public void SchedulingCancel()
        {
            CloseTopDialog();
            ScheduledStartDate = null;
            ScheduledEndDate = null;
        }

        public bool IsEnabledScheduling()
        {
            return !BackgroundWorker.IsBusy && SelectedScheduleForPWNode != null && ProdOrderBatchPlanList != null && ProdOrderBatchPlanList.Any();
        }

        [ACMethodInfo("SchedulingCalculateAll", "en{'Calculate All'}de{'Calculate All'}", 511)]
        public void SchedulingCalculateAll()
        {
            if (BackgroundWorker.IsBusy) return;
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoCalculateAll);
            ShowDialog(this, DesignNameProgressBar);
        }

        #endregion

        #region Methods -> (Tab)BatchPlanScheduler -> BatchState

        [ACMethodCommand("SetBatchStateReadyToStart", "en{'Switch to Readiness'}de{'Startbereit setzen'}", (short)MISort.Start)]
        public void SetBatchStateReadyToStart()
        {
            if (!IsEnabledSetBatchStateReadyToStart())
                return;
            List<ProdOrderBatchPlan> selectedBatches = ProdOrderBatchPlanList.Where(c => c.IsSelected).ToList();
            foreach (ProdOrderBatchPlan batchPlan in selectedBatches)
            {
                if (batchPlan.PlanState == GlobalApp.BatchPlanState.Created)
                    batchPlan.PlanState = vd.GlobalApp.BatchPlanState.ReadyToStart;
            }
            Save();
            OnPropertyChanged("ProdOrderBatchPlanList");
        }

        public bool IsEnabledSetBatchStateReadyToStart()
        {
            return ProdOrderBatchPlanList != null && ProdOrderBatchPlanList.Any(c => c.IsSelected && c.PlanState == vd.GlobalApp.BatchPlanState.Created);
        }

        [ACMethodCommand("SetBatchStateCreated", "en{'Reset Readiness'}de{'Startbereitschaft rücksetzen'}", 508)]
        public void SetBatchStateCreated()
        {
            if (!IsEnabledSetBatchStateCreated()) return;
            List<ProdOrderBatchPlan> selectedBatches = ProdOrderBatchPlanList.Where(c => c.IsSelected).ToList();
            foreach (ProdOrderBatchPlan batchPlan in selectedBatches)
            {
                if (batchPlan.PlanState == GlobalApp.BatchPlanState.ReadyToStart)
                    batchPlan.PlanState = vd.GlobalApp.BatchPlanState.Created;
            }
            Save();
            OnPropertyChanged("ProdOrderBatchPlanList");
        }

        public bool IsEnabledSetBatchStateCreated()
        {
            return ProdOrderBatchPlanList != null && ProdOrderBatchPlanList.Any(c => c.IsSelected && c.PlanState == vd.GlobalApp.BatchPlanState.ReadyToStart);
        }

        [ACMethodCommand("SetBatchStateCancelled", "en{'Deactivate and remove'}de{'Deaktivieren und Entfernen'}", (short)MISort.Start)]
        public void SetBatchStateCancelled()
        {
            ClearMessages();
            if (!IsEnabledSetBatchStateCancelled())
                return;

            List<Guid> groupsForRefresh = new List<Guid>();

            List<ProdOrderBatchPlan> selectedBatches = ProdOrderBatchPlanList.Where(c => c.IsSelected).ToList();
            foreach (ProdOrderBatchPlan batchPlan in selectedBatches)
                batchPlan.AutoRefresh();
            List<ProdOrderPartslist> partslists = selectedBatches.Select(c => c.ProdOrderPartslist).ToList();
            List<ProdOrder> prodOrders = partslists.Select(c => c.ProdOrder).Distinct().ToList();

            foreach (ProdOrderBatchPlan batchPlan in selectedBatches)
            {
                if (batchPlan.PlanState >= vd.GlobalApp.BatchPlanState.Paused
                    || batchPlan.ProdOrderBatch_ProdOrderBatchPlan.Any())
                    batchPlan.PlanState = GlobalApp.BatchPlanState.Cancelled;
                else if (batchPlan.PlanState == GlobalApp.BatchPlanState.Created)
                {
                    foreach (var reservation in batchPlan.FacilityReservation_ProdOrderBatchPlan.ToArray())
                    {
                        reservation.DeleteACObject(this.DatabaseApp, true);
                    }
                    Guid mdSchedulingGroupID = batchPlan.VBiACClassWF.MDSchedulingGroupWF_VBiACClassWF.Select(x => x.MDSchedulingGroupID).FirstOrDefault();
                    if (!groupsForRefresh.Contains(mdSchedulingGroupID))
                        groupsForRefresh.Add(mdSchedulingGroupID);
                    batchPlan.DeleteACObject(this.DatabaseApp, true);
                }
            }

            MDProdOrderState mDProdOrderStateCompleted = DatabaseApp.s_cQry_GetMDProdOrderState(DatabaseApp, MDProdOrderState.ProdOrderStates.ProdFinished).FirstOrDefault();
            MDProdOrderState mDProdOrderStateCancelled = DatabaseApp.s_cQry_GetMDProdOrderState(DatabaseApp, MDProdOrderState.ProdOrderStates.Cancelled).FirstOrDefault();

            MaintainProdOrderState(prodOrders, mDProdOrderStateCancelled, mDProdOrderStateCompleted);

            MsgWithDetails saveMsg = saveMsg = DatabaseApp.ACSaveChanges();
            if (saveMsg != null)
                SendMessage(saveMsg);

            LoadProdOrderBatchPlanList();
            if (!IsBSOTemplateScheduleParent)
            {
                if (groupsForRefresh.Any())
                    foreach (var mdSchedulingGroupID in groupsForRefresh)
                        RefreshServerState(mdSchedulingGroupID);
            }
        }

        public bool IsEnabledSetBatchStateCancelled()
        {
            return ProdOrderBatchPlanList != null
                && ProdOrderBatchPlanList.Any(c => c.IsSelected
                                                    && (c.PlanState <= vd.GlobalApp.BatchPlanState.Created
                                                        || c.PlanState >= vd.GlobalApp.BatchPlanState.Paused));
        }

        #endregion

        #endregion

        #region Methods -> (Tab)ProdOrder

        #region Methods -> (Tab)ProdOrder -> Filter

        /// <summary>
        /// Source Property: SearchOrders
        /// </summary>
        [ACMethodInfo("SearchOrders", "en{'Search'}de{'Suchen'}", 999)]
        public void SearchOrders()
        {
            if (!IsEnabledSearchOrders())
                return;
            ProdOrderPartslistList = GetProdOrderPartslistList();
        }

        public bool IsEnabledSearchOrders()
        {
            return
                (FilterOrderStartTime == null && FilterOrderEndTime == null && !(FilterOrderIsCompleted ?? true))
                ||
                (
                    FilterOrderStartTime != null
                    && FilterOrderEndTime != null
                    && (FilterOrderEndTime.Value - FilterOrderStartTime.Value).TotalDays > 0
                );
        }

        /// <summary>
        /// Source Property: ResetFilterOrderStartTime
        /// </summary>
        [ACMethodInfo("ResetFilterOrderStartTime", "en{'Set / Reset'}de{'Setzen / Zurücksetzen'}", 999)]
        public void ResetFilterOrderStartTime()
        {
            if (!IsEnabledResetFilterOrderStartTime())
                return;
            FilterOrderStartTime = FilterOrderStartTime == null ? (DateTime?)DateTime.Now : null;
        }

        public bool IsEnabledResetFilterOrderStartTime()
        {
            return true;
        }

        /// <summary>
        /// Source Property: ResetFilterOrderEndTime
        /// </summary>
        [ACMethodInfo("ResetFilterOrderEndTime", "en{'Set / Reset'}de{'Setzen / Zurücksetzen'}", 999)]
        public void ResetFilterOrderEndTime()
        {
            if (!IsEnabledResetFilterOrderEndTime())
                return;
            if (FilterOrderStartTime != null)
                FilterOrderEndTime = FilterOrderEndTime == null ? (DateTime?)FilterOrderStartTime.Value.AddDays(1) : null;
            else
                FilterOrderEndTime = null;
        }

        public bool IsEnabledResetFilterOrderEndTime()
        {
            return true;
        }

        #endregion

        #region Methods -> (Tab)ProdOrder -> Manipulate Batch Plan

        [ACMethodCommand("New", "en{'Add Batchplan'}de{'Batchplan Hinzufügen'}", 504)]
        public void AddBatchPlan()
        {
            if (!IsEnabledAddBatchPlan())
                return;
            var schedulingGroups = PartslistMDSchedulerGroupConnections.FirstOrDefault(c => c.PartslistID == SelectedProdOrderPartslist.ProdOrderPartslist.Partslist.PartslistID).SchedulingGroups;
            MDSchedulingGroup schedulingGroup = schedulingGroups.FirstOrDefault(c => c.MDSchedulingGroupID == SelectedScheduleForPWNode.MDSchedulingGroupID);
            schedulingGroup = schedulingGroups.FirstOrDefault();
            WizardDefineDefaultPartslist(schedulingGroup, SelectedProdOrderPartslist.ProdOrderPartslist.Partslist, SelectedProdOrderPartslist.UnPlannedQuantityUOM);
            DefaultWizardSchedulerPartslist.ProgramNo = SelectedProdOrderPartslist.ProdOrderPartslist.ProdOrder.ProgramNo;
            DefaultWizardSchedulerPartslist.MDProdOrderState = SelectedProdOrderPartslist.ProdOrderPartslist.MDProdOrderState;
            vd.ACClassWF tempACClassWFItem = DefaultWizardSchedulerPartslist.SelectedMDSchedulingGroup.MDSchedulingGroupWF_MDSchedulingGroup.Select(c => c.VBiACClassWF).FirstOrDefault();
            ProdOrderPartslistPos finalMix = ProdOrderManager.GetIntermediate(SelectedProdOrderPartslist.ProdOrderPartslist, tempACClassWFItem.MaterialWFConnection_ACClassWF.FirstOrDefault());
            DefaultWizardSchedulerPartslist.ProdOrderPartslistPos = finalMix;
            try
            {
                IsWizard = true;
                WizardPhase = NewScheduledBatchWizardPhaseEnum.BOMExplosion;
                bool success = WizardForwardAction(WizardPhase);
            }
            catch (Exception ex)
            {
                IsWizard = false;
                throw ex;
            }
            OnPropertyChanged("CurrentLayout");
        }

        public bool IsEnabledAddBatchPlan()
        {
            return
                !IsWizard
                && SelectedScheduleForPWNode != null
                && SelectedProdOrderPartslist != null
                && SelectedProdOrderPartslist.ProdOrderPartslist.MDProdOrderState.ProdOrderState < MDProdOrderState.ProdOrderStates.ProdFinished
                && SelectedProdOrderPartslist.ProdOrderPartslist.ProdOrder.MDProdOrderState.ProdOrderState < MDProdOrderState.ProdOrderStates.ProdFinished;
        }

        [ACMethodCommand("RemoveSelectedProdorderPartslist", "en{'Deactivate and remove'}de{'Deaktivieren und Entfernen'}", (short)MISort.Start)]
        public void RemoveSelectedProdorderPartslist()
        {
            ClearMessages();
            if (!IsEnabledRemoveSelectedProdorderPartslist())
                return;

            List<Guid> groupsForRefresh = new List<Guid>();

            MDProdOrderState mDProdOrderStateCancelled = DatabaseApp.s_cQry_GetMDProdOrderState(DatabaseApp, MDProdOrderState.ProdOrderStates.Cancelled).FirstOrDefault();
            MDProdOrderState mDProdOrderStateCompleted = DatabaseApp.s_cQry_GetMDProdOrderState(DatabaseApp, MDProdOrderState.ProdOrderStates.ProdFinished).FirstOrDefault();
            ProdOrderPartslist[] plForRemove = ProdOrderPartslistList.Where(c => c.IsSelected).Select(c => c.ProdOrderPartslist).ToArray();
            List<ProdOrder> prodOrders = plForRemove.Select(c => c.ProdOrder).Distinct().ToList();

            foreach (ProdOrderPartslist partslist in plForRemove)
            {
                partslist.AutoRefresh();
                foreach (var batchPlan2Remove in partslist.ProdOrderBatchPlan_ProdOrderPartslist.Where(c => c.PlanStateIndex == (short)GlobalApp.BatchPlanState.Created).ToArray())
                {
                    foreach (var reservation in batchPlan2Remove.FacilityReservation_ProdOrderBatchPlan.ToArray())
                    {
                        reservation.DeleteACObject(this.DatabaseApp, true);
                        batchPlan2Remove.FacilityReservation_ProdOrderBatchPlan.Remove(reservation);
                    }
                    Guid mdSchedulingGroupID = batchPlan2Remove.VBiACClassWF.MDSchedulingGroupWF_VBiACClassWF.Select(x => x.MDSchedulingGroupID).FirstOrDefault();
                    if (!groupsForRefresh.Contains(mdSchedulingGroupID))
                        groupsForRefresh.Add(mdSchedulingGroupID);
                    batchPlan2Remove.DeleteACObject(this.DatabaseApp, true);
                    partslist.ProdOrderBatchPlan_ProdOrderPartslist.Remove(batchPlan2Remove);
                }
                if (!partslist.ProdOrderBatchPlan_ProdOrderPartslist.Any())
                    RemovePartslist(partslist);
                else if (!IsBSOTemplateScheduleParent)
                {
                    if (!partslist.ProdOrderBatchPlan_ProdOrderPartslist.Any(c => c.PlanStateIndex >= (short)GlobalApp.BatchPlanState.ReadyToStart && c.PlanStateIndex <= (short)GlobalApp.BatchPlanState.InProcess))
                    {
                        SetProdorderPartslistState(mDProdOrderStateCancelled, mDProdOrderStateCompleted, partslist);
                    }
                }
            }
            MaintainProdOrderState(prodOrders, mDProdOrderStateCancelled, mDProdOrderStateCompleted);

            MsgWithDetails saveMsg = DatabaseApp.ACSaveChanges();
            if (saveMsg != null)
                SendMessage(saveMsg);
            LoadProdOrderBatchPlanList();
            if (!IsBSOTemplateScheduleParent)
            {
                if (groupsForRefresh.Any())
                    foreach (var mdSchedulingGroupID in groupsForRefresh)
                        RefreshServerState(mdSchedulingGroupID);
            }
        }

        protected void MaintainProdOrderState(List<ProdOrder> prodOrders, MDProdOrderState mDProdOrderStateCancelled, MDProdOrderState mDProdOrderStateCompleted)
        {
            foreach (ProdOrder prodOrder in prodOrders)
            {
                prodOrder.AutoRefresh();
                ProdOrderPartslist[] allProdPartslists = prodOrder.ProdOrderPartslist_ProdOrder.ToArray();
                if (!prodOrder.ProdOrderPartslist_ProdOrder.SelectMany(c => c.ProdOrderBatchPlan_ProdOrderPartslist).Any())
                {
                    foreach (ProdOrderPartslist pl in allProdPartslists)
                        RemovePartslist(pl);
                }
                else if (!IsBSOTemplateScheduleParent)
                {
                    if (
                        prodOrder.ProdOrderPartslist_ProdOrder.SelectMany(c => c.ProdOrderBatchPlan_ProdOrderPartslist).Any()
                        && !prodOrder.ProdOrderPartslist_ProdOrder.SelectMany(c => c.ProdOrderBatchPlan_ProdOrderPartslist).Any(c => c.PlanStateIndex < (short)GlobalApp.BatchPlanState.Completed))
                    {
                        foreach (ProdOrderPartslist pl in allProdPartslists)
                            SetProdorderPartslistState(mDProdOrderStateCancelled, mDProdOrderStateCompleted, pl);
                    }
                    if (!prodOrder.ProdOrderPartslist_ProdOrder.Any(c => c.MDProdOrderState.MDProdOrderStateIndex < (short)(short)GlobalApp.BatchPlanState.Completed))
                    {
                        prodOrder.MDProdOrderState = mDProdOrderStateCompleted;
                    }
                }
            }
        }

        public bool IsEnabledRemoveSelectedProdorderPartslist()
        {
            return ProdOrderPartslistList != null && ProdOrderPartslistList.Any(c => c.IsSelected);
        }

        #endregion

        #endregion

        #region Methods -> Wizard

        #region Methods -> Wizard -> Buttons

        [ACMethodInfo("Wizard", "en{'Back'}de{'Zurück'}", 510)]
        public void WizardBackward()
        {
            if (!IsEnabledWizardBackward())
                return;
            int wizardPhaseInt = (int)WizardPhase;
            wizardPhaseInt--;
            WizardPhase = (NewScheduledBatchWizardPhaseEnum)wizardPhaseInt;
        }

        public bool IsEnabledWizardBackward()
        {
            return
                WizardPhase > NewScheduledBatchWizardPhaseEnum.SelectMaterial
                &&
                (
                    !IsWizardExistingBatch
                    || WizardPhase > NewScheduledBatchWizardPhaseEnum.PartslistForDefinition
                )
                && WizardPhase < NewScheduledBatchWizardPhaseEnum.DefineTargets;
        }

        [ACMethodInfo("Wizard", "en{'Forward'}de{'Weiter'}", 509)]
        public void WizardForward()
        {
            if (!IsEnabledWizardForward())
                return;
            Msg validationMsg = IsEnabledWizardForwardValidation();
            if (validationMsg != null)
            {
                SendMessage(validationMsg);
                return;
            }
            if (IsWizard && WizardPhase < NewScheduledBatchWizardPhaseEnum.DefineTargets)
            {
                int wizardPhaseInt = (int)WizardPhase;
                wizardPhaseInt++;
                NewScheduledBatchWizardPhaseEnum wizardPhase = (NewScheduledBatchWizardPhaseEnum)wizardPhaseInt;
                WizardForwardAction(wizardPhase);
                WizardPhase = wizardPhase;
            }
            else
            {
                if (WizardSchedulerPartslistList.Any())
                {
                    if (DatabaseApp.IsChanged)
                    {
                        MsgWithDetails saveMsg = DatabaseApp.ACSaveChanges();
                        if (saveMsg != null)
                            SendMessage(saveMsg);
                    }
                    WizardPhase = NewScheduledBatchWizardPhaseEnum.PartslistForDefinition;
                    OnPropertyChanged("WizardSchedulerPartslistList");
                    if (WizardSchedulerPartslistList != null)
                        SelectedWizardSchedulerPartslist = WizardSchedulerPartslistList.FirstOrDefault();
                }
                else
                {
                    if (WizardFinish())
                        WizardClean();
                }
            }
        }

        [ACMethodInfo("WizardForwardSelectLinie", "en{'Plan'}de{'Planen'}", 9999)]
        public void WizardForwardSelectLinie(object CommandParameter)
        {
            WizardSchedulerPartslist wizardSchedulerPartslist = CommandParameter as WizardSchedulerPartslist;
            if (
                    wizardSchedulerPartslist != null
                    && (
                            !wizardSchedulerPartslist.IsSolved
                            ||
                                (
                                    IsWizardExistingBatch
                                    && wizardSchedulerPartslist.MDProdOrderState.ProdOrderState < MDProdOrderState.ProdOrderStates.ProdFinished
                                )
                        ))
            {
                SelectedWizardSchedulerPartslist = wizardSchedulerPartslist;
                WizardForward();
            }
        }

        private bool IsEnabledFactoryBatch()
        {
            return
                LocalBSOBatchPlan != null
                && DefaultWizardSchedulerPartslist != null
                && SelectedWizardSchedulerPartslist != null
                && SelectedWizardSchedulerPartslist.TargetQuantityUOM > Double.Epsilon;
        }


        [ACMethodInfo("ChangeBatchPlan", "en{'Change'}de{'Bearbeiten'}", 600)]
        public void ChangeBatchPlan(ProdOrderBatchPlan batchPlan)
        {
            bool notValidBatchForChange =
                    batchPlan == null
                    || IsWizard
                    //|| batchPlan.ProdOrderBatch_ProdOrderBatchPlan.Any()
                    || batchPlan.ProdOrderPartslist.MDProdOrderState.MDProdOrderStateIndex >= (short)MDProdOrderState.ProdOrderStates.ProdFinished
                    || batchPlan.ProdOrderPartslist.ProdOrder.MDProdOrderState.MDProdOrderStateIndex >= (short)MDProdOrderState.ProdOrderStates.ProdFinished;
            if (notValidBatchForChange)
                return;

            try
            {
                ClearMessages();
                WizardPhase = NewScheduledBatchWizardPhaseEnum.PartslistForDefinition;
                IsWizardExistingBatch = true;
                IsWizard = true;
                WizardForwardAction(WizardPhase);

                vd.ACClassWF vbACClassWF = SelectedScheduleForPWNode.MDSchedulingGroup.MDSchedulingGroupWF_MDSchedulingGroup.Select(c => c.VBiACClassWF).FirstOrDefault();
                SetBSOBatchPlan_BatchParents(vbACClassWF, batchPlan.ProdOrderPartslist);
                LoadGeneratedBatchInCurrentLine(batchPlan, batchPlan.ProdOrderPartslist.TargetQuantity);
            }
            catch (Exception ex)
            {
                IsWizardExistingBatch = false;
                IsWizard = false;
                throw ex;
            }

            OnPropertyChanged("CurrentLayout");
        }

        public bool IsWizardExistingBatch { get; set; }

        #endregion

        #region Methods -> Wizard -> Execute

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsEnabledWizardForward()
        {
            bool isEnabled = false;
            switch (WizardPhase)
            {
                case NewScheduledBatchWizardPhaseEnum.SelectMaterial:
                    isEnabled =
                        BSOPartslistExplorer_Child != null &&
                        BSOPartslistExplorer_Child.Value != null &&
                        BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child != null &&
                        BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value != null &&
                        BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value.SelectedMaterial != null &&
                        DatabaseApp.Partslist.Any(c => c.MaterialID == BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value.SelectedMaterial.MaterialID);
                    break;
                case NewScheduledBatchWizardPhaseEnum.SelectPartslist:
                    isEnabled = IsEnabledFactoryBatch();
                    break;
                case NewScheduledBatchWizardPhaseEnum.BOMExplosion:
                    isEnabled = IsEnabledFactoryBatch();
                    break;
                case NewScheduledBatchWizardPhaseEnum.PartslistForDefinition:
                    isEnabled = (!SelectedWizardSchedulerPartslist.IsSolved || IsWizardExistingBatch) && SelectedWizardSchedulerPartslist.SelectedMDSchedulingGroup != null;
                    break;
                case NewScheduledBatchWizardPhaseEnum.DefineBatch:
                    isEnabled =
                        SelectedWizardSchedulerPartslist != null
                        && SelectedWizardSchedulerPartslist.BatchPlanSuggestion != null
                        && SelectedWizardSchedulerPartslist.BatchPlanSuggestion.ItemsList != null
                        && SelectedWizardSchedulerPartslist.BatchPlanSuggestion.ItemsList.Any()
                        && SelectedWizardSchedulerPartslist.BatchPlanSuggestion.IsSuggestionValid();
                    break;
                case NewScheduledBatchWizardPhaseEnum.DefineTargets:
                    isEnabled =
                        BSOBatchPlanChild != null &&
                        BSOBatchPlanChild.Value != null;
                    if (isEnabled)
                        isEnabled = BSOBatchPlanChild.Value.SelectedBatchPlanForIntermediate != null && SelectedWizardSchedulerPartslist != null && SelectedWizardSchedulerPartslist.NewTargetQuantityUOM > Double.Epsilon;
                    break;
            }
            return isEnabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Msg IsEnabledWizardForwardValidation()
        {
            Msg msg = null;
            switch (WizardPhase)
            {
                case NewScheduledBatchWizardPhaseEnum.DefineBatch:
                    WizardPhaseErrorMessage = "";
                    bool isValidBatchExpectedEndTime =
                        SelectedWizardSchedulerPartslist != null
                        &&
                            (
                                SelectedWizardSchedulerPartslist.OffsetToEndTime == null
                                ||
                                (SelectedWizardSchedulerPartslist.BatchPlanSuggestion == null && SelectedWizardSchedulerPartslist.BatchPlanSuggestion.ItemsList == null)
                                ||
                                !SelectedWizardSchedulerPartslist.BatchPlanSuggestion.ItemsList.Any(c => c.ExpectedBatchEndTime == null || c.ExpectedBatchEndTime == (new DateTime()))
                            );
                    if (!isValidBatchExpectedEndTime)
                    {
                        msg = new Msg(this, eMsgLevel.Error, "BSOBatchPlanScheduler", "WizardForward()", 2880, "Error50472");
                        WizardPhaseErrorMessage = msg.Message;
                    }
                    break;
            }
            return msg;
        }

        private bool WizardForwardAction(NewScheduledBatchWizardPhaseEnum wizardPhase)
        {
            bool success = false;
            try
            {
                switch (wizardPhase)
                {
                    case NewScheduledBatchWizardPhaseEnum.SelectMaterial:
                        BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value.FilterMDSchedulingGroupID = SelectedScheduleForPWNode.MDSchedulingGroupID;
                        BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value.Search();
                        success = true;
                        break;
                    case NewScheduledBatchWizardPhaseEnum.SelectPartslist:
                        success = true;
                        WizardSolvedTasks.Add(NewScheduledBatchWizardPhaseEnum.SelectPartslist);
                        break;
                    case NewScheduledBatchWizardPhaseEnum.BOMExplosion:

                        if (WizardSolvedTasks.Contains(NewScheduledBatchWizardPhaseEnum.BOMExplosion))
                            return true;

                        Partslist selectedPartslist = DefaultWizardSchedulerPartslist.Partslist;
                        if (selectedPartslist.MDUnitID.HasValue)
                            DefaultWizardSchedulerPartslist.TargetQuantity = selectedPartslist.Material.ConvertQuantity(DefaultWizardSchedulerPartslist.TargetQuantityUOM, selectedPartslist.Material.BaseMDUnit, selectedPartslist.MDUnit);
                        SelectedWizardSchedulerPartslist = DefaultWizardSchedulerPartslist;
                        OnSelectedWizardSchedulerPartslistChanged();

                        double treeQuantityRatio = DefaultWizardSchedulerPartslist.TargetQuantityUOM / selectedPartslist.TargetQuantityUOM;
                        rootPartslistExpand = new PartslistExpand(selectedPartslist, treeQuantityRatio);
                        rootPartslistExpand.IsChecked = true;
                        rootPartslistExpand.LoadTree();
                        rootPartslistExpand.IsEnabled = false;
                        rootPartslistExpand.IsChecked = false;
                        _PartListExpandList = new List<PartslistExpand>();
                        _PartListExpandList.Add(rootPartslistExpand);
                        CurrentPartListExpand = rootPartslistExpand;
                        OnPropertyChanged("PartListExpandList");
                        success = true;
                        WizardSolvedTasks.Add(NewScheduledBatchWizardPhaseEnum.BOMExplosion);
                        break;
                    case NewScheduledBatchWizardPhaseEnum.PartslistForDefinition:
                        if (WizardSolvedTasks.Contains(NewScheduledBatchWizardPhaseEnum.PartslistForDefinition))
                            return true;
                        if (IsWizardExistingBatch)
                            LoadExistingWizardSchedulerPartslistList();
                        else
                            LoadNewWizardSchedulerPartslistList();
                        rootPartslistExpand = null;
                        foreach (var item in AllWizardSchedulerPartslistList)
                            if (item.SelectedMDSchedulingGroup != null)
                                LoadConfiguration(item);
                        success = SelectedWizardSchedulerPartslist != null;
                        var tmp = _SelectedWizardSchedulerPartslist;
                        OnPropertyChanged("WizardSchedulerPartslistList");
                        _SelectedWizardSchedulerPartslist = tmp;
                        WizardSolvedTasks.Add(NewScheduledBatchWizardPhaseEnum.PartslistForDefinition);
                        break;
                    case NewScheduledBatchWizardPhaseEnum.DefineBatch:
                        if (IsWizardExistingBatch)
                            SelectedWizardSchedulerPartslist.LoadExistingBatchSuggestion();
                        else
                            SelectedWizardSchedulerPartslist.LoadNewBatchSuggestion(FilterBatchplanSuggestionMode);
                        if (
                                SelectedWizardSchedulerPartslist.BatchPlanSuggestion == null
                                || SelectedWizardSchedulerPartslist.BatchPlanSuggestion.ItemsList == null
                                || !SelectedWizardSchedulerPartslist.BatchPlanSuggestion.ItemsList.Any())
                        {
                            // Error50392
                            Msg noBachSuggestionsErr = new Msg(this, eMsgLevel.Error, GetACUrl(), "WizardForward", 0, "Error50392");
                            SendMessage(noBachSuggestionsErr);
                        }
                        WizardSolvedTasks.Add(NewScheduledBatchWizardPhaseEnum.DefineBatch);
                        success = true;
                        break;
                    case NewScheduledBatchWizardPhaseEnum.DefineTargets:
                        success = SelectedWizardSchedulerPartslist != null && SelectedWizardSchedulerPartslist.NewTargetQuantityUOM > Double.Epsilon && SelectedWizardSchedulerPartslist.BatchPlanSuggestion != null;
                        if (success)
                        {
                            string programNo = SelectedWizardSchedulerPartslist.ProgramNo;

                            if (SelectedWizardSchedulerPartslist.ProdOrderPartslistPos != null)
                                success = UpdateBatchPlans(SelectedWizardSchedulerPartslist);
                            else
                                success = FactoryBatchPlans(SelectedWizardSchedulerPartslist, ref programNo);

                            MsgWithDetails saveMsg = DatabaseApp.ACSaveChanges();
                            if (saveMsg != null)
                                SendMessage(saveMsg);
                            if (success && !string.IsNullOrEmpty(programNo) && (saveMsg == null || saveMsg.IsSucceded()))
                            {
                                AllWizardSchedulerPartslistList.ForEach(x => x.ProgramNo = programNo);
                            }
                            WizardSolvedTasks.Add(NewScheduledBatchWizardPhaseEnum.DefineTargets);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Msg msg = new Msg() { MessageLevel = eMsgLevel.Exception, Source = GetACUrl(), Message = @"Error: " + ex.Message };
                Root.Messages.LogException(GetACUrl(), "WizardForward", ex);
                SendMessage(msg);
            }
            return success;
        }

        #endregion

        #region Methods -> Wizard -> Helper Methods

        private List<WizardSchedulerPartslist> LoadWizardSchedulerPartslistList(PartslistExpand currentPartListExpand)
        {
            List<ExpandResult> treeResult = currentPartListExpand.BuildTreeList();
            treeResult =
                treeResult
                .Where(x =>
                    x.Item.IsChecked
                    && x.Item.IsEnabled)
                .OrderByDescending(x => x.TreeVersion)
                .ToList();

            int sn = 0;

            List<WizardSchedulerPartslist> wizardSchedulerPartslists = new List<WizardSchedulerPartslist>();
            foreach (ExpandResult expand in treeResult)
            {
                sn++;
                List<MDSchedulingGroup> schedulingGroups = GetSchedulingGroups(expand.Item.PartslistForPosition);
                WizardSchedulerPartslist wizardSchedulerPartslist = GetWizardSchedulerPartslist(expand.Item.PartslistForPosition, expand.Item.TargetQuantityUOM, sn, schedulingGroups);
                wizardSchedulerPartslists.Add(wizardSchedulerPartslist);
            }
            return wizardSchedulerPartslists;
        }

        private WizardSchedulerPartslist GetWizardSchedulerPartslist(Partslist partslist, double targetQuantityUOM, int sn, List<MDSchedulingGroup> schedulingGroups)
        {
            WizardSchedulerPartslist item = new WizardSchedulerPartslist(DatabaseApp, ProdOrderManager);
            item.Partslist = partslist;
            item.PartslistNo = partslist.PartslistNo;
            item.PartslistName = partslist.PartslistName;
            item.Sn = sn;
            if (targetQuantityUOM > Double.Epsilon)
            {
                item.TargetQuantityUOM = targetQuantityUOM;
                if (partslist.MDUnitID.HasValue)
                    item.TargetQuantity = partslist.Material.ConvertQuantity(item.TargetQuantityUOM, partslist.Material.BaseMDUnit, partslist.MDUnit);
            }
            item.MDSchedulingGroupList = schedulingGroups;
            item.SelectedMDSchedulingGroup = item.MDSchedulingGroupList.FirstOrDefault();
            item.ProductionUnits = partslist.ProductionUnits;
            return item;
        }

        private List<MDSchedulingGroup> GetSchedulingGroups(Partslist partslist)
        {
            List<MDSchedulingGroup> schedulingGroups =
                    PartslistMDSchedulerGroupConnections
                    .Where(c =>
                    c.PartslistID == partslist.PartslistID)
                    .SelectMany(c => c.SchedulingGroups)
                    .OrderBy(c => c.SortIndex)
                    .ToList();
            IEnumerable<Tuple<int, Guid>> items =
                partslist
                .PartslistConfig_Partslist
                .Where(c => c.LocalConfigACUrl.Contains("LineOrderInPlan") && c.VBiACClassWFID != null && c.Value != null)
                .ToArray()
                .Select(c => new Tuple<int, Guid>((int)c.Value, c.VBiACClassWFID.Value))
                .OrderBy(c => c.Item1)
                .ToArray();
            if (items != null && items.Any())
            {
                List<MDSchedulingGroup> tmpSchedulingGroups = new List<MDSchedulingGroup>();
                if (schedulingGroups != null && schedulingGroups.Any())
                    foreach (Tuple<int, Guid> item in items)
                    {
                        MDSchedulingGroup mDSchedulingGroup = schedulingGroups.Where(c => c.MDSchedulingGroupWF_MDSchedulingGroup.Any(x => x.VBiACClassWFID == item.Item2)).FirstOrDefault();
                        if (mDSchedulingGroup != null)
                            tmpSchedulingGroups.Add(mDSchedulingGroup);
                    }

                tmpSchedulingGroups.AddRange(
                    schedulingGroups
                    .Where(c => !tmpSchedulingGroups.Select(x => x.MDSchedulingGroupID).Contains(c.MDSchedulingGroupID))
                    .OrderBy(c => c.SortIndex)
                );
                schedulingGroups = tmpSchedulingGroups;
            }
            return schedulingGroups;
        }

        private void ClearAndResetChildBSOLists(MDSchedulingGroup mdSchedulingGroup)
        {
            BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value.AccessPrimary.NavList.Clear();
            BSOPartslistExplorer_Child.Value.AccessPrimary.NavList.Clear();
            BSOPartslistExplorer_Child.Value.FilterMDSchedulingGroup = mdSchedulingGroup;
        }


        private List<NewScheduledBatchWizardPhaseEnum> _WizardSolvedTasks;
        public List<NewScheduledBatchWizardPhaseEnum> WizardSolvedTasks
        {
            get
            {
                if (_WizardSolvedTasks == null)
                    _WizardSolvedTasks = new List<NewScheduledBatchWizardPhaseEnum>();
                return _WizardSolvedTasks;
            }
        }

        private bool WizardFinish()
        {
            bool success = ACSaveChanges();
            if (success)
            {
                IsWizard = false;
                OnPropertyChanged("CurrentLayout");
                OnPostSave();
                Search();
            }
            return success;
        }

        private void LoadNewWizardSchedulerPartslistList()
        {
            List<WizardSchedulerPartslist> expandedItems = LoadWizardSchedulerPartslistList(rootPartslistExpand);
            if (expandedItems.Any())
            {
                int nr = 0;
                foreach (WizardSchedulerPartslist item in expandedItems)
                {
                    AddWizardSchedulerPartslistList(item, nr);
                    ++nr;
                }
                DefaultWizardSchedulerPartslist.Sn = expandedItems.Count() + 1;
            }
        }

        private void LoadExistingWizardSchedulerPartslistList()
        {
            AllWizardSchedulerPartslistList = new List<WizardSchedulerPartslist>();
            List<ProdOrderPartslist> partslists = SelectedProdOrderBatchPlan.ProdOrderPartslist.ProdOrder.ProdOrderPartslist_ProdOrder.OrderBy(c => c.Sequence).ToList();
            foreach (ProdOrderPartslist partslist in partslists)
            {
                List<MDSchedulingGroup> schedulingGroups = GetSchedulingGroups(partslist.Partslist);
                WizardSchedulerPartslist item = GetWizardSchedulerPartslist(partslist.Partslist, partslist.TargetQuantity, partslist.Sequence, schedulingGroups);
                item.ProgramNo = partslist.ProdOrder.ProgramNo;
                item.MDProdOrderState = partslist.MDProdOrderState;
                vd.ACClassWF tempACClassWFItem = item.SelectedMDSchedulingGroup.MDSchedulingGroupWF_MDSchedulingGroup.Select(c => c.VBiACClassWF).FirstOrDefault();
                ProdOrderPartslistPos finalMix = ProdOrderManager.GetIntermediate(partslist, tempACClassWFItem.MaterialWFConnection_ACClassWF.FirstOrDefault());
                item.ProdOrderPartslistPos = finalMix;
                if(finalMix.MDUnit == null)
                    item.SelectedUnitConvert = null;
                else
                    item.SelectedUnitConvert = finalMix.MDUnit;
                AddWizardSchedulerPartslistList(item);
                if (SelectedProdOrderBatchPlan.ProdOrderPartslistID == partslist.ProdOrderPartslistID)
                    SelectedWizardSchedulerPartslist = item;
            }
            DefaultWizardSchedulerPartslist = AllWizardSchedulerPartslistList.LastOrDefault();
        }

        private Msg CheckProductionUnits()
        {
            Msg msg = null;
            if (SelectedWizardSchedulerPartslist.ProductionUnits != null && SelectedWizardSchedulerPartslist.ProductionUnits.Value > 0)
            {
                double rest = SelectedWizardSchedulerPartslist.TargetQuantityUOM % SelectedWizardSchedulerPartslist.ProductionUnits.Value;
                if (rest > 0)
                {
                    msg = new Msg(this, eMsgLevel.Error, GetACUrl(), "CheckProductionUnits", 3119, "Error50440", SelectedWizardSchedulerPartslist.TargetQuantityUOM, SelectedWizardSchedulerPartslist.ProductionUnits);
                }
            }
            return msg;
        }


        public void WizardDefineDefaultPartslist(MDSchedulingGroup schedulingGroup, Partslist partslist, double targetQuantity)
        {
            List<MDSchedulingGroup> tmpSchedulingGroup = new List<MDSchedulingGroup>() { schedulingGroup };
            DefaultWizardSchedulerPartslist = GetWizardSchedulerPartslist(partslist, targetQuantity, 1, tmpSchedulingGroup);
            AllWizardSchedulerPartslistList.Clear();
            AddWizardSchedulerPartslistList(DefaultWizardSchedulerPartslist);
            SelectedWizardSchedulerPartslist = DefaultWizardSchedulerPartslist;
        }

        [ACMethodInfo("Wizard", "en{'Close'}de{'Schließen'}", 511)]
        public void WizardCancel()
        {
            IsWizard = false;
            ACUndoChanges();
            WizardClean();
            OnPropertyChanged("CurrentLayout");
            LoadProdOrderBatchPlanList();
            OnPropertyChanged("SelectedProdOrderBatchPlan");
        }

        public void WizardClean()
        {
            BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value.SearchWord = "";
            BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value.SelectedMaterial = null;
            BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value.MaterialList.Clear();

            BSOPartslistExplorer_Child.Value.SearchWord = "";
            BSOPartslistExplorer_Child.Value.SelectedPartslist = null;
            BSOPartslistExplorer_Child.Value.AccessPrimary.NavList.Clear();

            DefaultWizardSchedulerPartslist = null;
            SelectedWizardSchedulerPartslist = null;
            AllWizardSchedulerPartslistList.Clear();


            LocalBSOBatchPlan.BatchPlanForIntermediateList.Clear();
            LocalBSOBatchPlan.SelectedBatchPlanForIntermediate = null;

            _WizardPhase = NewScheduledBatchWizardPhaseEnum.SelectMaterial;

            WizardSolvedTasks.Clear();
            IsWizardExistingBatch = false;
            ClearMessages();
        }

        #endregion

        #region Methods -> BatchPlanTimelinePresenter

        [ACMethodInfo("", "en{'Show'}de{'Anzeigen'}", 520)]
        public void ShowBatchPlansOnTimeline()
        {
            List<BatchPlanTimelineItem> treeViewItems = new List<BatchPlanTimelineItem>();
            List<BatchPlanTimelineItem> timelineItems = new List<BatchPlanTimelineItem>();

            vd.GlobalApp.BatchPlanState startState = GlobalApp.BatchPlanState.Created;
            vd.GlobalApp.BatchPlanState endState = GlobalApp.BatchPlanState.Paused;
            MDProdOrderState.ProdOrderStates? prodOrderState = null;
            ObservableCollection<vd.ProdOrderBatchPlan> prodOrderBatchPlans =
                ProdOrderManager
                .GetProductionLinieBatchPlans(
                    DatabaseApp,
                    null,
                    startState,
                    endState,
                    FilterStartTime,
                    FilterEndTime,
                    prodOrderState,
                    FilterPlanningMR?.PlanningMRID);

            int displayOrder = 0;

            foreach (PAScheduleForPWNode schedule in ScheduleForPWNodeList)
            {
                BatchPlanTimelineItem treeViewItem = new BatchPlanTimelineItem();
                treeViewItem._ACCaption = schedule.ACCaption;
                treeViewItem.DisplayOrder = displayOrder;
                treeViewItem.TimelineItemType = BatchPlanTimelineItem.BatchPlanTimelineItemType.ContainerItem;

                //TODO: change start and end date, add more details to timelineitem (order etc.)
                Guid[] acClassWFIds =
                    DatabaseApp
                    .MDSchedulingGroup
                    .Where(c => c.MDSchedulingGroupID == schedule.MDSchedulingGroupID)
                    .SelectMany(c => c.MDSchedulingGroupWF_MDSchedulingGroup)
                    .Select(c => c.VBiACClassWFID)
                    .Distinct()
                    .ToArray();


                List<BatchPlanTimelineItem> batchPlanTimeline = new List<BatchPlanTimelineItem>();
                List<ProdOrderBatchPlan> batchPlans = prodOrderBatchPlans.Where(c => acClassWFIds.Contains(c.VBiACClassWFID ?? Guid.Empty)).ToList();
                foreach (ProdOrderBatchPlan batchPlan in batchPlans)
                {
                    BatchPlanTimelineItem item = new BatchPlanTimelineItem();
                    item._ACCaption = string.Format(@"[{0}] {1} ({2} {3})",
                        batchPlan.ProdOrderPartslist.ProdOrder.ProgramNo,
                        batchPlan.ProdOrderPartslist.Partslist.Material.MaterialName1,
                        batchPlan.TotalSize,
                        batchPlan.ProdOrderPartslist.Partslist.MDUnitID.HasValue ? batchPlan.ProdOrderPartslist.Partslist.MDUnit.MDUnitName : batchPlan.ProdOrderPartslist.Partslist.Material.BaseMDUnit.MDUnitName);
                    if (batchPlan.ScheduledStartDate != null)
                        item.StartDate = batchPlan.ScheduledStartDate;
                    else
                        item.StartDate = batchPlan.PlannedStartDate;
                    if (batchPlan.ScheduledEndDate != null)
                        item.EndDate = batchPlan.ScheduledEndDate;
                    else
                        item.EndDate = item.StartDate.Value.AddMinutes(10);
                    item.DisplayOrder = displayOrder;
                    item.ParentACObject = treeViewItem;
                    item.TimelineItemType = BatchPlanTimelineItem.BatchPlanTimelineItemType.TimelineItem;
                    item.ProgramNo = batchPlan.ProdOrderPartslist.ProdOrder.ProgramNo;
                    item.TargetQuantityUOM = batchPlan.TotalSize;
                    batchPlanTimeline.Add(item);
                }

                if (!batchPlanTimeline.Any())
                    continue;

                treeViewItem.StartDate = batchPlanTimeline.Min(c => c.StartDate);
                treeViewItem.EndDate = batchPlanTimeline.Max(c => c.EndDate);

                timelineItems.Add(treeViewItem);
                timelineItems.AddRange(batchPlanTimeline);
                treeViewItems.Add(treeViewItem);

                displayOrder++;
            }

            BatchPlanTimelineItemsRoot = treeViewItems;
            BatchPlanTimelineItems = new ObservableCollection<BatchPlanTimelineItem>(timelineItems);
        }

        #endregion

        #region Methods -> Overrides
        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            var result = base.OnGetControlModes(vbControl);

            if (vbControl != null && !string.IsNullOrEmpty(vbControl.VBContent))
            {
                switch (vbControl.VBContent)
                {
                    case "New":
                        if (IsEnabledNew())
                            result = Global.ControlModes.Enabled;
                        else
                            result = Global.ControlModes.Hidden;
                        break;
                    case "SelectedProdOrderBatchPlan\\PartialTargetCount":
                        //SelectedProdOrderBatchPlan.PlannedStartDate && SelectedProdOrderBatchPlan.end
                        if (SelectedScheduleForPWNode != null
                            && SelectedScheduleForPWNode.StartMode == GlobalApp.BatchPlanStartModeEnum.SemiAutomatic
                            && SelectedProdOrderBatchPlan != null
                            && SelectedProdOrderBatchPlan.PlanMode == GlobalApp.BatchPlanMode.UseBatchCount
                            && SelectedProdOrderBatchPlan.BatchTargetCount > SelectedProdOrderBatchPlan.BatchActualCount)
                            result = Global.ControlModes.Enabled;
                        else
                            result = Global.ControlModes.Disabled;
                        break;
                    case "SelectedProdOrderBatchPlan\\ScheduledStartDate":
                    case "SelectedProdOrderBatchPlan\\ScheduledEndDate":
                        if (SelectedProdOrderBatchPlan != null
                            && (SelectedProdOrderBatchPlan.PlanState <= GlobalApp.BatchPlanState.ReadyToStart || SelectedProdOrderBatchPlan.PlanState == GlobalApp.BatchPlanState.Paused)
                            && (SelectedProdOrderBatchPlan.PlanMode != GlobalApp.BatchPlanMode.UseBatchCount
                               || SelectedProdOrderBatchPlan.BatchTargetCount > SelectedProdOrderBatchPlan.BatchActualCount))
                            result = Global.ControlModes.Enabled;
                        else
                            result = Global.ControlModes.Disabled;
                        break;
                    case "FilterStartTime":
                        result = Global.ControlModes.Enabled;
                        bool filterStartTimeIsRequired =
                           FilterStartTime == null
                                    || (
                                            FilterStartTime != null
                                            && FilterEndTime != null
                                            && (FilterEndTime.Value - FilterStartTime.Value).TotalDays > Const_MaxFilterDaySpan
                                        );
                        if (filterStartTimeIsRequired)
                            result = Global.ControlModes.EnabledWrong;
                        break;
                    case "FilterEndTime":
                        result = Global.ControlModes.Enabled;
                        bool filterEndTimeIsRequired =
                            FilterEndTime == null
                                    || (
                                            FilterStartTime != null
                                            && FilterEndTime != null
                                            && (FilterEndTime.Value - FilterStartTime.Value).TotalDays > Const_MaxFilterDaySpan
                                        );
                        if (filterEndTimeIsRequired)
                            result = Global.ControlModes.EnabledWrong;
                        break;
                    case "FilterOrderStartTime":
                        result = Global.ControlModes.Enabled;
                        bool filterOrderStartTimeIsEnabled =
                           !(FilterOrderIsCompleted ?? true)
                           || (FilterOrderStartTime != null && FilterOrderEndTime != null);
                        if (!filterOrderStartTimeIsEnabled)
                            result = Global.ControlModes.EnabledWrong;
                        break;
                    case "FilterOrderEndTime":
                        result = Global.ControlModes.Enabled;
                        bool filterOrderEndTimeIsEnabled =
                             !(FilterOrderIsCompleted ?? true)
                           || (FilterOrderStartTime != null && FilterOrderEndTime != null);
                        if (!filterOrderEndTimeIsEnabled)
                            result = Global.ControlModes.EnabledWrong;
                        break;

                    case "SelectedWizardSchedulerPartslist\\BatchPlanSuggestion\\Difference":
                        result = Global.ControlModes.Enabled;
                        if (
                                SelectedWizardSchedulerPartslist != null
                                && SelectedWizardSchedulerPartslist.BatchPlanSuggestion != null
                                && SelectedWizardSchedulerPartslist.BatchPlanSuggestion.IsSuggestionValid())
                        {
                            result = Global.ControlModes.EnabledWrong;
                        }
                        break;
                }
            }
            return result;
        }
        #endregion

        #endregion

        #region Methods -> Private (Helper) Mehtods

        #region Methods -> Private (Helper) Mehtods -> Load

        private void LoadProdOrderBatchPlanList()
        {
            ProdOrderBatchPlanList = GetProdOrderBatchPlanList(SelectedScheduleForPWNode?.MDSchedulingGroupID);
            ProdOrderPartslistList = GetProdOrderPartslistList();
        }

        private void LoadScheduleListForPWNodes(string schedulingGroupMDKey)
        {
            PAScheduleForPWNodeList newScheduleForWFNodeList = null;
            if (_SchedulesForPWNodesProp != null && _SchedulesForPWNodesProp.ValueT != null)
                newScheduleForWFNodeList = _SchedulesForPWNodesProp.ValueT;
            else
                newScheduleForWFNodeList = PABatchPlanScheduler.CreateScheduleListForPWNodes(this, DatabaseApp, null);
            //int removedCount = newScheduleForWFNodeList.RemoveAll(x => x.MDSchedulingGroupID == Guid.Empty);
            UpdateScheduleForPWNodeList(newScheduleForWFNodeList);
            if (!string.IsNullOrEmpty(schedulingGroupMDKey))
                SelectedScheduleForPWNode = ScheduleForPWNodeList.FirstOrDefault(c => c.MDSchedulingGroup.MDKey == schedulingGroupMDKey);
        }

        private void UpdateScheduleForPWNodeList(PAScheduleForPWNodeList newScheduleForWFNodeList)
        {
            PAScheduleForPWNodeList.DiffResult diffResult = PAScheduleForPWNodeList.DiffResult.Equal;
            if (_ScheduleForPWNodeList == null)
            {
                _ScheduleForPWNodeList = newScheduleForWFNodeList.Clone() as PAScheduleForPWNodeList;
                diffResult = PAScheduleForPWNodeList.DiffResult.NewPWNodesDetected;
            }
            else
            {
                diffResult = _ScheduleForPWNodeList.CompareAndUpdateFrom(newScheduleForWFNodeList, this.SelectedScheduleForPWNode);
            }

            if (diffResult.HasFlag(PAScheduleForPWNodeList.DiffResult.NewPWNodesDetected))
            {
                PAScheduleForPWNode[] newNodes = _ScheduleForPWNodeList.Where(c => c.MDSchedulingGroup == null).ToArray();
                if (newNodes != null && newNodes.Any())
                {
                    List<MDSchedulingGroup> mDSchedulingGroups = DatabaseApp.MDSchedulingGroup.ToList();
                    foreach (var newNode in newNodes)
                    {
                        newNode.MDSchedulingGroup = mDSchedulingGroups.Where(c => c.MDSchedulingGroupID == newNode.MDSchedulingGroupID).FirstOrDefault();
                    }
                }
            }

            if (diffResult.HasFlag(PAScheduleForPWNodeList.DiffResult.NewPWNodesDetected)
                || diffResult.HasFlag(PAScheduleForPWNodeList.DiffResult.PWNodesRemoved)
                || diffResult.HasFlag(PAScheduleForPWNodeList.DiffResult.StartModeChanged))
            {
                if (diffResult.HasFlag(PAScheduleForPWNodeList.DiffResult.StartModeChanged)
                    && !diffResult.HasFlag(PAScheduleForPWNodeList.DiffResult.NewPWNodesDetected)
                    && !diffResult.HasFlag(PAScheduleForPWNodeList.DiffResult.PWNodesRemoved))
                {
                    OnPropertyChanged("SelectedScheduleForPWNode");
                }
                else
                {
                    var selected = SelectedScheduleForPWNode;
                    OnPropertyChanged("ScheduleForPWNodeList");
                    SelectedScheduleForPWNode = selected;
                    if (SelectedScheduleForPWNode != null)
                        SelectedFilterBatchPlanStartMode = FilterBatchPlanStartModeList.Where(c => (GlobalApp.BatchPlanStartModeEnum)c.Value == _SelectedScheduleForPWNode.StartMode).FirstOrDefault();
                }
            }

            if (diffResult.HasFlag(PAScheduleForPWNodeList.DiffResult.RefreshCounterChanged)
                || diffResult.HasFlag(PAScheduleForPWNodeList.DiffResult.ValueChangesInList))
                LoadProdOrderBatchPlanList();
        }

        #endregion

        #region Methods -> Private(Helper) Mehtods -> Factory Batch

        private void WritePosMDUnit(vd.ProdOrderBatchPlan prodOrderBatchPlan, WizardSchedulerPartslist wizardSchedulerPartslist)
        {
            if (wizardSchedulerPartslist.SelectedUnitConvert != null
                && wizardSchedulerPartslist.SelectedUnitConvert != wizardSchedulerPartslist.Partslist.Material.BaseMDUnit)
                prodOrderBatchPlan.ProdOrderPartslistPos.MDUnit = wizardSchedulerPartslist.SelectedUnitConvert;
            else
                prodOrderBatchPlan.ProdOrderPartslistPos.MDUnit = null;
        }

        private vd.ProdOrderBatchPlan FactoryBatchPlan(vd.ACClassWF vbACClassWF, vd.Partslist partslist, vd.ProdOrderPartslist prodOrderPartslist, vd.GlobalApp.BatchPlanState startMode, ref int maxScheduledOrder, DateTime? scheduledEndDate, WizardSchedulerPartslist wizardSchedulerPartslist)
        {

            vd.ProdOrderBatchPlan prodOrderBatchPlan = vd.ProdOrderBatchPlan.NewACObject(DatabaseApp, prodOrderPartslist);
            prodOrderBatchPlan.PlanState = startMode;
            ++maxScheduledOrder;
            prodOrderBatchPlan.ScheduledOrder = maxScheduledOrder;
            prodOrderBatchPlan.VBiACClassWF = vbACClassWF;


            var materialWFConnection = ProdOrderManager.GetMaterialWFConnection(vbACClassWF, prodOrderPartslist.Partslist.MaterialWFID);
            var test = materialWFConnection.MaterialWFACClassMethod;
            prodOrderBatchPlan.MaterialWFACClassMethod = partslist.PartslistACClassMethod_Partslist
                                               .Where(c => c.MaterialWFACClassMethod.ACClassMethodID == vbACClassWF.ACClassMethodID)
                                               .Select(c => c.MaterialWFACClassMethod)
                                               .FirstOrDefault();
            prodOrderBatchPlan.ProdOrderPartslistPos = ProdOrderManager.GetIntermediate(prodOrderPartslist, materialWFConnection);

            WritePosMDUnit(prodOrderBatchPlan, wizardSchedulerPartslist);

            prodOrderBatchPlan.ScheduledEndDate = scheduledEndDate;
            if (wizardSchedulerPartslist.OffsetToEndTime.HasValue)
                prodOrderBatchPlan.ScheduledStartDate = prodOrderBatchPlan.ScheduledEndDate - wizardSchedulerPartslist.OffsetToEndTime.Value;

            return prodOrderBatchPlan;
        }

        private bool FactoryBatchPlans(WizardSchedulerPartslist wizardSchedulerPartslist, ref string programNo)
        {
            bool success = false;
            vd.ProdOrderBatchPlan firstBatchPlan = null;
            vd.ProdOrderPartslist prodOrderPartslist = null;
            vd.ProdOrder prodOrder = null;
            if (string.IsNullOrEmpty(programNo))
            {
                string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(vd.ProdOrder), vd.ProdOrder.NoColumnName, vd.ProdOrder.FormatNewNo, this);
                programNo = secondaryKey;
                prodOrder = vd.ProdOrder.NewACObject(DatabaseApp, null, secondaryKey);
                ACSaveChanges();
            }
            else
            {
                string tmpProgramNo = programNo;
                prodOrder = DatabaseApp.ProdOrder.FirstOrDefault(c => c.ProgramNo == tmpProgramNo);
            }

            if (wizardSchedulerPartslist.ProdOrderPartslistPos != null)
            {
                success = true;
            }
            else
            {
                Msg msg = ProdOrderManager.PartslistAdd(DatabaseApp, prodOrder, wizardSchedulerPartslist.Partslist, wizardSchedulerPartslist.Sn, wizardSchedulerPartslist.NewTargetQuantityUOM, out prodOrderPartslist);
                success = msg == null || msg.IsSucceded();

                if (FilterPlanningMR != null && success)
                {
                    PlanningMR planningMR = DatabaseApp.PlanningMR.FirstOrDefault(c => c.PlanningMRID == FilterPlanningMR.PlanningMRID);
                    PlanningMRProposal proposal = PlanningMRProposal.NewACObject(DatabaseApp, planningMR);
                    proposal.ProdOrder = prodOrderPartslist.ProdOrder;
                    proposal.ProdOrderPartslist = prodOrderPartslist;
                    planningMR.PlanningMRProposal_PlanningMR.Add(proposal);
                }
            }

            if (success)
            {

                vd.MDProdOrderPartslistPosState mDProdOrderPartslistPosState = DatabaseApp.MDProdOrderPartslistPosState.FirstOrDefault(c => c.MDProdOrderPartslistPosStateIndex == (short)vd.MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Created);

                List<vd.ProdOrderBatchPlan> generatedBatchPlans = new List<ProdOrderBatchPlan>();
                int nr = 0;
                // @aagincic: There is selected first node - logic for setup next node ?
                vd.ACClassWF vbACClassWF = wizardSchedulerPartslist.SelectedMDSchedulingGroup.MDSchedulingGroupWF_MDSchedulingGroup.Select(c => c.VBiACClassWF).FirstOrDefault();
                List<SchedulingMaxBPOrder> maxSchedulerOrders = ProdOrderManager.GetMaxScheduledOrder(DatabaseApp, FilterPlanningMR?.PlanningMRNo);
                int maxSchedulingOrder =
                    maxSchedulerOrders
                    .Where(c => c.MDSchedulingGroup.MDSchedulingGroupID == wizardSchedulerPartslist.SelectedMDSchedulingGroup.MDSchedulingGroupID)
                    .SelectMany(c => c.WFs)
                    .Where(c => c.ACClassWF.ACClassWFID == vbACClassWF.ACClassWFID)
                    .Select(c => c.MaxScheduledOrder)
                    .DefaultIfEmpty()
                    .Max();
                int test = maxSchedulingOrder;
                foreach (var item in wizardSchedulerPartslist.BatchPlanSuggestion.ItemsList)
                {
                    nr++;
                    vd.ProdOrderBatchPlan batchPlan = FactoryBatchPlan(vbACClassWF, wizardSchedulerPartslist.Partslist, prodOrderPartslist, CreatedBatchState, ref maxSchedulingOrder, item.ExpectedBatchEndTime, wizardSchedulerPartslist);
                    batchPlan.ProdOrderPartslistPos.MDProdOrderPartslistPosState = mDProdOrderPartslistPosState;
                    WriteBatchPlanQuantities(item, batchPlan);
                    batchPlan.Sequence = nr;
                    if (firstBatchPlan == null)
                        firstBatchPlan = batchPlan;
                    generatedBatchPlans.Add(batchPlan);
                }
                wizardSchedulerPartslist.IsSolved = true;
                wizardSchedulerPartslist.ProgramNo = prodOrder.ProgramNo;

                SetBSOBatchPlan_BatchParents(vbACClassWF, prodOrderPartslist);
                LoadGeneratedBatchInCurrentLine(firstBatchPlan, wizardSchedulerPartslist.NewTargetQuantityUOM);
            }
            return success;
        }

        private void WriteBatchPlanQuantities(BatchPlanSuggestionItem suggestionItem, ProdOrderBatchPlan batchPlan)
        {
            batchPlan.BatchSize = suggestionItem.BatchSizeUOM;
            batchPlan.BatchTargetCount = suggestionItem.BatchTargetCount;
            batchPlan.TotalSize = suggestionItem.TotalBatchSize;
        }

        private bool UpdateBatchPlans(WizardSchedulerPartslist wizardSchedulerPartslist)
        {
            bool success = true;
            ProdOrderPartslist prodOrderPartslist = wizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderPartslist;
            vd.MDProdOrderPartslistPosState mDProdOrderPartslistPosState = DatabaseApp.MDProdOrderPartslistPosState.FirstOrDefault(c => c.MDProdOrderPartslistPosStateIndex == (short)vd.MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Created);
            PartslistACClassMethod method = wizardSchedulerPartslist.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
            vd.ACClassWF vbACClassWF = wizardSchedulerPartslist.SelectedMDSchedulingGroup.MDSchedulingGroupWF_MDSchedulingGroup
                .Where(c => c.VBiACClassWF.ACClassMethodID == method.MaterialWFACClassMethod.ACClassMethodID)
                .Select(c => c.VBiACClassWF)
                .FirstOrDefault();

            List<ProdOrderBatchPlan> existingBatchPlans = wizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrderBatchPlan_ProdOrderPartslist.ToList();
            Guid[] wizardBatchPlanIDs = wizardSchedulerPartslist.BatchPlanSuggestion.ItemsList.Where(c => c.ProdOrderBatchPlan != null).Select(c => c.ProdOrderBatchPlan.ProdOrderBatchPlanID).ToArray();
            List<ProdOrderBatchPlan> missingBatchPlans = existingBatchPlans.Where(c => !wizardBatchPlanIDs.Contains(c.ProdOrderBatchPlanID)).ToList();
            foreach (BatchPlanSuggestionItem suggestionItem in wizardSchedulerPartslist.BatchPlanSuggestion.ItemsList)
            {
                ProdOrderBatchPlan batchPlan = null;
                if (suggestionItem.ProdOrderBatchPlan != null)
                    batchPlan = existingBatchPlans.FirstOrDefault(c => c.ProdOrderBatchPlanID == suggestionItem.ProdOrderBatchPlan.ProdOrderBatchPlanID);
                else
                {
                    List<SchedulingMaxBPOrder> maxSchedulerOrders = ProdOrderManager.GetMaxScheduledOrder(DatabaseApp, FilterPlanningMR?.PlanningMRNo);
                    int maxSchedulingOrder =
                        maxSchedulerOrders
                        .Where(c => c.MDSchedulingGroup.MDSchedulingGroupID == wizardSchedulerPartslist.SelectedMDSchedulingGroup.MDSchedulingGroupID)
                        .SelectMany(c => c.WFs)
                        .Where(c => c.ACClassWF.ACClassWFID == vbACClassWF.ACClassWFID)
                        .Select(c => c.MaxScheduledOrder)
                        .DefaultIfEmpty()
                        .Max();
                    batchPlan = FactoryBatchPlan(vbACClassWF, prodOrderPartslist.Partslist, prodOrderPartslist, GlobalApp.BatchPlanState.Created, ref maxSchedulingOrder, suggestionItem.ExpectedBatchEndTime, wizardSchedulerPartslist);
                    prodOrderPartslist.ProdOrderBatchPlan_ProdOrderPartslist.Add(batchPlan);
                    batchPlan.ProdOrderPartslistPos.MDProdOrderPartslistPosState = mDProdOrderPartslistPosState;
                }
                WriteBatchPlanQuantities(suggestionItem, batchPlan);

                batchPlan.ScheduledEndDate = suggestionItem.ExpectedBatchEndTime;
                if (wizardSchedulerPartslist.OffsetToEndTime.HasValue)
                    batchPlan.ScheduledStartDate = batchPlan.ScheduledEndDate - wizardSchedulerPartslist.OffsetToEndTime.Value;
            }
            foreach (ProdOrderBatchPlan missingBatchPlan in missingBatchPlans)
                missingBatchPlan.DeleteACObject(DatabaseApp, false);

            ProdOrderBatchPlan firstBatchPlan = prodOrderPartslist.ProdOrderBatchPlan_ProdOrderPartslist.FirstOrDefault();

            WritePosMDUnit(firstBatchPlan, wizardSchedulerPartslist);

            SetBSOBatchPlan_BatchParents(vbACClassWF, prodOrderPartslist);
            LoadGeneratedBatchInCurrentLine(firstBatchPlan, SelectedWizardSchedulerPartslist.NewTargetQuantityUOM);
            return success;
        }

        #endregion

        #region Methods -> Private (Helper) Mehtods -> LocalBSOBatchPlan Select batch plan

        private void SetBSOBatchPlan_BatchParents(vd.ACClassWF vbACClassWF, vd.ProdOrderPartslist prodOrderPartslist)
        {
            core.datamodel.ACClassWF aCClassWF = vbACClassWF.FromIPlusContext<gip.core.datamodel.ACClassWF>(DatabaseApp.ContextIPlus);
            if (prodOrderPartslist != null)
            {
                LocalBSOBatchPlan.ExternProdOrderPartslist = prodOrderPartslist;
                LocalBSOBatchPlan.MandatoryConfigStores = LocalBSOBatchPlan
                    .GetCurrentConfigStores(
                    aCClassWF,
                    vbACClassWF,
                    LocalBSOBatchPlan.CurrentProdOrderPartslist.Partslist.MaterialWFID,
                    LocalBSOBatchPlan.CurrentProdOrderPartslist.Partslist,
                    LocalBSOBatchPlan.CurrentProdOrderPartslist
                    );
            }
            LocalBSOBatchPlan.CurrentACClassWF = aCClassWF;
            LocalBSOBatchPlan.VBCurrentACClassWF = vbACClassWF;
        }

        private void LoadGeneratedBatchInCurrentLine(vd.ProdOrderBatchPlan batchPlan, double targetQuantityUOM)
        {
            LocalBSOBatchPlan.SelectedIntermediate = batchPlan.ProdOrderPartslistPos;
            LocalBSOBatchPlan.LoadBatchPlanForIntermediateList(false);

            if (!LocalBSOBatchPlan.BatchPlanForIntermediateList.Any(c => c.ProdOrderBatchPlanID == batchPlan.ProdOrderBatchPlanID))
                LocalBSOBatchPlan.BatchPlanForIntermediateList.Add(batchPlan);

            LocalBSOBatchPlan.SelectedBatchPlanForIntermediate = batchPlan;
            if (Math.Abs(LocalBSOBatchPlan.SelectedIntermediate.TargetQuantityUOM - targetQuantityUOM) > Double.Epsilon)
                LocalBSOBatchPlan.SelectedIntermediate.TargetQuantityUOM = targetQuantityUOM;

            LocalBSOBatchPlan.OnPropertyChanged("IsVisibleInCurrentContext");
        }

        #endregion

        #region Methods -> Private -> Remove ProdORderPartslist or change state

        private bool SetProdorderPartslistState(MDProdOrderState mDProdOrderStateCancelled, MDProdOrderState mDProdOrderStateCompleted, ProdOrderPartslist partslist)
        {
            bool isSetState = false;
            if (!partslist.ProdOrderBatchPlan_ProdOrderPartslist.Any() || partslist.ProdOrderBatchPlan_ProdOrderPartslist.Where(c => c.PlanStateIndex != (short)GlobalApp.BatchPlanState.Completed).Any())
            {
                partslist.MDProdOrderState = mDProdOrderStateCancelled;
                isSetState = true;
            }
            else if (!partslist.ProdOrderBatchPlan_ProdOrderPartslist.Where(c => c.PlanStateIndex != (short)GlobalApp.BatchPlanState.Completed).Any())
            {
                partslist.MDProdOrderState = mDProdOrderStateCompleted;
                isSetState = true;
            }
            return isSetState;
        }

        private void RemovePartslist(ProdOrderPartslist partslist)
        {
            ProdOrder prodOrder = partslist.ProdOrder;
            if (!partslist.ProdOrderBatchPlan_ProdOrderPartslist.Any())
            {
                Msg msg = ProdOrderManager.PartslistRemove(DatabaseApp, prodOrder, partslist);
                if (msg != null)
                    SendMessage(msg);
                prodOrder.ProdOrderPartslist_ProdOrder.Remove(partslist);
                if (prodOrder.ProdOrderPartslist_ProdOrder != null && prodOrder.ProdOrderPartslist_ProdOrder.Any())
                {
                    var tempPlList = prodOrder.ProdOrderPartslist_ProdOrder.ToList();
                    SequenceManager<ProdOrderPartslist>.Order(ref tempPlList);
                }
                if (prodOrder.ProdOrderPartslist_ProdOrder == null || !prodOrder.ProdOrderPartslist_ProdOrder.Any())
                    prodOrder.DeleteACObject(DatabaseApp, false);
            }
        }

        #endregion

        #endregion

        #endregion

        #region BackgroundWorker

        /// <summary>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        public override void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            base.BgWorkerDoWork(sender, e);
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = e.Argument.ToString();

            worker.ProgressInfo.OnlyTotalProgress = true;
            worker.ProgressInfo.AddSubTask(command, 0, 9);
            string message = Translator.GetTranslation("en{'Running {0}...'}de{'{0} läuft...'}");
            worker.ProgressInfo.ReportProgress(command, 0, string.Format(message, command));

            string updateName = Root.Environment.User.Initials;
            switch (command)
            {
                case BGWorkerMehtod_DoBackwardScheduling:
                    e.Result =
                        SchedulingForecastManager
                        .BackwardScheduling(
                                            DatabaseApp,
                                            SelectedScheduleForPWNode.MDSchedulingGroupID,
                                            updateName,
                                            ScheduledEndDate.Value);
                    break;
                case BGWorkerMehtod_DoForwardScheduling:
                    e.Result =
                        SchedulingForecastManager
                        .ForwardScheduling(
                                            DatabaseApp,
                                            SelectedScheduleForPWNode.MDSchedulingGroupID,
                                            updateName,
                                            ScheduledStartDate.Value);
                    break;
                case BGWorkerMehtod_DoCalculateAll:
                    SchedulingForecastManager.UpdateAllBatchPlanDurations(Root.Environment.User.Initials);
                    break;
            }
        }

        public override void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            base.BgWorkerCompleted(sender, e);
            CloseWindow(this, DesignNameProgressBar);
            ClearMessages();
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
                Msg resultMsg = null;
                if (e.Result != null)
                    resultMsg = (Msg)e.Result;
                if (resultMsg == null || resultMsg.IsSucceded())
                    LoadProdOrderBatchPlanList();

                if (resultMsg != null)
                {
                    if (resultMsg is MsgWithDetails)
                    {
                        MsgWithDetails msgWithDetails = resultMsg as MsgWithDetails;
                        if (msgWithDetails.MsgDetails.Any())
                            foreach (Msg detailMsg in msgWithDetails.MsgDetails)
                                SendMessage(detailMsg);
                    }
                    else
                        SendMessage(resultMsg);
                    Root.Messages.Msg(resultMsg);
                }

            }
        }

        #endregion

        #region Transformation - WFsLines

        [ACMethodInfo("InitialBuildLines", "en{'Initial build lines'}de{'Vorbereite Linien'}", 507)]
        public void InitialBuildLines()
        {
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                var query = databaseApp
               .ACClassWF
               .Where(c =>
                   c.RefPAACClassMethodID.HasValue
                   && c.RefPAACClassID.HasValue
                   && c.RefPAACClassMethod.ACKindIndex == (short)Global.ACKinds.MSWorkflow
                   && c.RefPAACClassMethod.PWACClass != null
                   && (c.RefPAACClassMethod.PWACClass.ACIdentifier == PWNodeProcessWorkflowVB.PWClassName
                       || c.RefPAACClassMethod.PWACClass.ACClass1_BasedOnACClass.ACIdentifier == PWNodeProcessWorkflowVB.PWClassName)
                   && !string.IsNullOrEmpty(c.Comment))
               .ToArray();

                int nr = 0;
                foreach (var item in query)
                {
                    nr++;
                    MDSchedulingGroup group = MDSchedulingGroup.NewACObject(databaseApp, null);
                    string wfName = item.ACCaption;
                    if (string.IsNullOrEmpty(wfName))
                        wfName = item.Comment;
                    group.MDNameTrans = string.Format(@"en{{'{0}'}}de{{'{0}'}}", wfName);
                    group.MDKey = item.ACIdentifier + nr.ToString("00");
                    if (group.MDKey.Length > 40)
                        throw new Exception();
                    MDSchedulingGroupWF groupWf = MDSchedulingGroupWF.NewACObject(databaseApp, group);
                    groupWf.VBiACClassWF = item;
                    databaseApp.MDSchedulingGroup.AddObject(group);
                }
                databaseApp.ACSaveChanges();
                OnPropertyChanged("ScheduleForPWNodeList");
            }
        }

        private bool? _IsEnabledInitialBuildLines;
        public bool IsEnabledInitialBuildLines()
        {
            if (_IsEnabledInitialBuildLines == null)
            {
                _IsEnabledInitialBuildLines = !DatabaseApp.MDSchedulingGroup.Any();
            }
            return _IsEnabledInitialBuildLines ?? false;
        }

        #endregion

    }

    public enum NewScheduledBatchWizardPhaseEnum : short
    {
        SelectMaterial = 1,
        SelectPartslist = 2,
        BOMExplosion = 3,
        PartslistForDefinition = 4,
        DefineBatch = 5,
        DefineTargets = 6
    }
}