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
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using vd = gip.mes.datamodel;
using System.Xml;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Batch scheduler'}de{'Batch Zeitplaner'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + vd.ProdOrderBatchPlan.ClassName)]
    public class BSOBatchPlanScheduler : ACBSOvb
    {
        #region const
        public const string BGWorkerMehtod_DoBackwardScheduling = @"DoBackwardScheduling";
        public const string BGWorkerMehtod_DoForwardScheduling = @"DoForwardScheduling";
        public const string BGWorkerMehtod_DoCalculateAll = @"DoCalculateAll";
        public const string BGWorkerMehtod_DoGenerateBatchPlans = @"DoGenerateBatchPlans";
        public const string BGWorkerMehtod_DoMergeOrders = @"DoMergeOrders";
        public const string BGWorkerMehtod_DoSearchStockMaterial = @"DoSearchStockMaterial";
        public const int Const_MaxFilterDaySpan = 10;
        public const int Const_MaxResultSize = 500;
        public const string test = "test";
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

        private ACPropertyConfigValue<int> _AutoRemoveMDSGroupFrom;
        [ACPropertyConfig("en{'Auto delete dependant BOMs if Scheduling-Group-Nr from'}de{'Auto. Löschen von abhängigen Stücklisten wenn Scheduling-Group-Nr von'}")]
        public int AutoRemoveMDSGroupFrom
        {
            get
            {
                return _AutoRemoveMDSGroupFrom.ValueT;
            }
            set
            {
                _AutoRemoveMDSGroupFrom.ValueT = value;
            }
        }

        private ACPropertyConfigValue<int> _AutoRemoveMDSGroupTo;
        [ACPropertyConfig("en{'Auto delete dependant BOMs if Scheduling-Group-Nr to'}de{'Auto. Löschen von abhängigen Stücklisten wenn Scheduling-Group-Nr bis'}")]
        public int AutoRemoveMDSGroupTo
        {
            get
            {
                return _AutoRemoveMDSGroupTo.ValueT;
            }
            set
            {
                _AutoRemoveMDSGroupTo.ValueT = value;
            }
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
                OnPropertyChanged(nameof(CreatedBatchState));
            }
        }


        private TimeSpan? GetExpectedBatchEndTime(WizardSchedulerPartslist wizardSchedulerPartslist)
        {
            vd.ACClassWF vbACClassWF = wizardSchedulerPartslist.WFNodeMES;
            if (vbACClassWF == null)
                return null;
            Partslist partslist = wizardSchedulerPartslist.Partslist;  //DatabaseApp.Partslist.FirstOrDefault(c => c.PartslistNo == wizardSchedulerPartslist.PartslistNo);
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
            _BSOBatchPlanSchedulerRules = new ACPropertyConfigValue<string>(this, nameof(BSOBatchPlanSchedulerRules), "");
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

            if (FilterProdPartslistOrderList != null)
                SelectedFilterProdPartslistOrder = FilterProdPartslistOrderList.Where(c => (BatchPlanProdOrderSortFilterEnum)c.Value == BatchPlanProdOrderSortFilterEnum.StartTime).FirstOrDefault();

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

            _CreatedBatchState = new ACPropertyConfigValue<vd.GlobalApp.BatchPlanState>(this, nameof(CreatedBatchState), vd.GlobalApp.BatchPlanState.Created);
            _AutoRemoveMDSGroupFrom = new ACPropertyConfigValue<int>(this, nameof(AutoRemoveMDSGroupFrom), 0);
            _ = _AutoRemoveMDSGroupFrom.ValueT;
            _AutoRemoveMDSGroupTo = new ACPropertyConfigValue<int>(this, nameof(AutoRemoveMDSGroupTo), 0);
            _ = _AutoRemoveMDSGroupTo.ValueT;

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

            _TempRules = null;
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
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoSearchStockMaterial);
            ShowDialog(this, DesignNameProgressBar);
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(WizardCancel):
                    WizardCancel();
                    return true;
                case nameof(ShowBatchPlansOnTimeline):
                    ShowBatchPlansOnTimeline();
                    return true;
                case nameof(ConfigureBSO):
                    ConfigureBSO();
                    return true;
                case nameof(IsEnabledConfigureBSO):
                    result = IsEnabledConfigureBSO();
                    return true;
                case nameof(AddRule):
                    AddRule();
                    return true;
                case nameof(IsEnabledAddRule):
                    result = IsEnabledAddRule();
                    return true;
                case nameof(RemoveRule):
                    RemoveRule();
                    return true;
                case nameof(IsEnabledRemoveRule):
                    result = IsEnabledRemoveRule();
                    return true;
                case nameof(ApplyRulesAndClose):
                    ApplyRulesAndClose();
                    return true;
                case nameof(IsEnabledApplyRulesAndClose):
                    result = IsEnabledApplyRulesAndClose();
                    return true;
                case nameof(InitialBuildLines):
                    InitialBuildLines();
                    return true;
                case nameof(IsEnabledInitialBuildLines):
                    result = IsEnabledInitialBuildLines();
                    return true;
                case nameof(ChangeMode):
                    ChangeMode();
                    return true;
                case nameof(IsEnabledChangeMode):
                    result = IsEnabledChangeMode();
                    return true;
                case nameof(MoveToOtherLine):
                    MoveToOtherLine();
                    return true;
                case nameof(IsEnabledMoveToOtherLine):
                    result = IsEnabledMoveToOtherLine();
                    return true;
                case nameof(ResetFilterStartTime):
                    ResetFilterStartTime();
                    return true;
                case nameof(IsEnabledResetFilterStartTime):
                    result = IsEnabledResetFilterStartTime();
                    return true;
                case nameof(ResetFilterEndTime):
                    ResetFilterEndTime();
                    return true;
                case nameof(IsEnabledResetFilterEndTime):
                    result = IsEnabledResetFilterEndTime();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
                case nameof(IsEnabledSearch):
                    result = IsEnabledSearch();
                    return true;
                case nameof(New):
                    New();
                    return true;
                case nameof(IsEnabledNew):
                    result = IsEnabledNew();
                    return true;
                case nameof(DeleteBatch):
                    DeleteBatch();
                    return true;
                case nameof(IsEnabledDeleteBatch):
                    result = IsEnabledDeleteBatch();
                    return true;
                case nameof(BatchPlanEdit):
                    BatchPlanEdit();
                    return true;
                case nameof(IsEnabledBatchPlanEdit):
                    result = IsEnabledBatchPlanEdit();
                    return true;
                case nameof(ItemDrag):
                    ItemDrag((System.Collections.Generic.Dictionary<System.Int32, System.String>)acParameter[0]);
                    return true;
                case nameof(IsEnabledItemDrag):
                    result = IsEnabledItemDrag();
                    return true;
                case nameof(NavigateToProdOrder):
                    NavigateToProdOrder();
                    return true;
                case nameof(IsEnabledNavigateToProdOrder):
                    result = IsEnabledNavigateToProdOrder();
                    return true;
                case nameof(MoveSelectedBatchUp):
                    MoveSelectedBatchUp();
                    return true;
                case nameof(IsEnabledMoveSelectedBatchUp):
                    result = IsEnabledMoveSelectedBatchUp();
                    return true;
                case nameof(MoveSelectedBatchDown):
                    MoveSelectedBatchDown();
                    return true;
                case nameof(IsEnabledMoveSelectedBatchDown):
                    result = IsEnabledMoveSelectedBatchDown();
                    return true;
                case nameof(ShowComponents):
                    ShowComponents();
                    return true;
                case nameof(IsEnabledShowComponents):
                    result = IsEnabledShowComponents();
                    return true;
                case nameof(ShowParslist):
                    ShowParslist();
                    return true;
                case nameof(IsEnabledShowParslist):
                    result = IsEnabledShowParslist();
                    return true;
                case nameof(ShowPartslistOK):
                    ShowPartslistOK();
                    return true;
                case nameof(IsEnabledShowPartslistOK):
                    result = IsEnabledShowPartslistOK();
                    return true;
                case nameof(BackwardScheduling):
                    BackwardScheduling();
                    return true;
                case nameof(IsEnabledBackwardScheduling):
                    result = IsEnabledBackwardScheduling();
                    return true;
                case nameof(BackwardSchedulingOk):
                    BackwardSchedulingOk();
                    return true;
                case nameof(IsEnabledBackwardSchedulingOk):
                    result = IsEnabledBackwardSchedulingOk();
                    return true;
                case nameof(ForwardScheduling):
                    ForwardScheduling();
                    return true;
                case nameof(IsEnabledForwardScheduling):
                    result = IsEnabledForwardScheduling();
                    return true;
                case nameof(ForwardSchedulingOk):
                    ForwardSchedulingOk();
                    return true;
                case nameof(IsEnabledForwardSchedulingOk):
                    result = IsEnabledForwardSchedulingOk();
                    return true;
                case nameof(SchedulingCancel):
                    SchedulingCancel();
                    return true;
                case nameof(IsEnabledScheduling):
                    result = IsEnabledScheduling();
                    return true;
                case nameof(SchedulingCalculateAll):
                    SchedulingCalculateAll();
                    return true;
                case nameof(SetBatchStateReadyToStart):
                    SetBatchStateReadyToStart();
                    return true;
                case nameof(IsEnabledSetBatchStateReadyToStart):
                    result = IsEnabledSetBatchStateReadyToStart();
                    return true;
                case nameof(SetBatchStateCreated):
                    SetBatchStateCreated();
                    return true;
                case nameof(IsEnabledSetBatchStateCreated):
                    result = IsEnabledSetBatchStateCreated();
                    return true;
                case nameof(SetBatchStateCancelled):
                    SetBatchStateCancelled();
                    return true;
                case nameof(IsEnabledSetBatchStateCancelled):
                    result = IsEnabledSetBatchStateCancelled();
                    return true;
                case nameof(SearchOrders):
                    SearchOrders();
                    return true;
                case nameof(IsEnabledSearchOrders):
                    result = IsEnabledSearchOrders();
                    return true;
                case nameof(ResetFilterOrderStartTime):
                    ResetFilterOrderStartTime();
                    return true;
                case nameof(IsEnabledResetFilterOrderStartTime):
                    result = IsEnabledResetFilterOrderStartTime();
                    return true;
                case nameof(ResetFilterOrderEndTime):
                    ResetFilterOrderEndTime();
                    return true;
                case nameof(IsEnabledResetFilterOrderEndTime):
                    result = IsEnabledResetFilterOrderEndTime();
                    return true;
                case nameof(NavigateToProdOrder2):
                    NavigateToProdOrder2();
                    return true;
                case nameof(IsEnabledNavigateToProdOrder2):
                    result = IsEnabledNavigateToProdOrder2();
                    return true;
                case nameof(AddBatchPlan):
                    AddBatchPlan();
                    return true;
                case nameof(IsEnabledAddBatchPlan):
                    result = IsEnabledAddBatchPlan();
                    return true;
                case nameof(RemoveSelectedProdorderPartslist):
                    RemoveSelectedProdorderPartslist();
                    return true;
                case nameof(IsEnabledRemoveSelectedProdorderPartslist):
                    result = IsEnabledRemoveSelectedProdorderPartslist();
                    return true;
                case nameof(WizardBackward):
                    WizardBackward();
                    return true;
                case nameof(IsEnabledWizardBackward):
                    result = IsEnabledWizardBackward();
                    return true;
                case nameof(WizardForward):
                    WizardForward();
                    return true;
                case nameof(WizardForwardSelectLinie):
                    WizardForwardSelectLinie((System.Object)acParameter[0]);
                    return true;
                case nameof(ChangeBatchPlan):
                    ChangeBatchPlan((gip.mes.datamodel.ProdOrderBatchPlan)acParameter[0]);
                    return true;
                case nameof(GenerateBatchPlans):
                    GenerateBatchPlans();
                    return true;
                case nameof(IsEnabledGenerateBatchPlans):
                    result = IsEnabledGenerateBatchPlans();
                    return true;
                case nameof(MergeOrders):
                    MergeOrders();
                    return true;
                case nameof(IsEnabledMergeOrders):
                    result = IsEnabledMergeOrders();
                    return true;
                case nameof(IsEnabledWizardForward):
                    result = IsEnabledWizardForward();
                    return true;
                case nameof(RecalculateBatchSuggestion):
                    RecalculateBatchSuggestion();
                    return true;
                case nameof(IsEnabledRecalculateBatchSuggestion):
                    result = IsEnabledRecalculateBatchSuggestion();
                    return true;
                case nameof(AddSuggestion):
                    AddSuggestion();
                    return true;
                case nameof(IsEnabledAddSuggestion):
                    result = IsEnabledAddSuggestion();
                    return true;
                case nameof(RemoveSuggestion):
                    RemoveSuggestion();
                    return true;
                case nameof(IsEnabledRemoveSuggestion):
                    result = IsEnabledRemoveSuggestion();
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
                case nameof(Load):
                    Load(acParameter.Count() == 1 ? (System.Boolean)acParameter[0] : false);
                    return true;
                case nameof(IsEnabledLoad):
                    result = IsEnabledLoad();
                    return true;
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
            LoadScheduleListForPWNodes(selectedLine, RefreshBatchListByRecieveChange);
        }

        #endregion

        private void ChildBSO_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedPartslist")
            {
                OnPropertyChanged(nameof(WizardSchedulerPartslist.UnitConvertList));
                if (BSOPartslistExplorer_Child.Value.SelectedPartslist != null
                    && BSOPartslistExplorer_Child.Value.SelectedPartslist.Material != null)
                {
                    // Always Base-UOM:
                    Partslist selectedPartslist = BSOPartslistExplorer_Child.Value.SelectedPartslist;
                    if (SelectedScheduleForPWNode != null)
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

        /// <summary>
        /// Flag is network batch list refresh is executed
        /// </summary>
        public bool RefreshBatchListByRecieveChange = true;

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
                    OnPropertyChanged(nameof(SelectedScheduleForPWNode));
                    if (_SelectedScheduleForPWNode != null)
                        SelectedFilterBatchPlanStartMode = FilterBatchPlanStartModeList.Where(c => (BatchPlanStartModeEnum)c.Value == _SelectedScheduleForPWNode.StartMode).FirstOrDefault();
                    else
                        SelectedFilterBatchPlanStartMode = FilterBatchPlanStartModeList.Where(c => (BatchPlanStartModeEnum)c.Value == BatchPlanStartModeEnum.Off).FirstOrDefault();
                    LoadProdOrderBatchPlanList();

                    OnPropertyChanged(nameof(TargetScheduleForPWNodeList));
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

        bool _SchedulingGroupValidated = false;
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
                    if (!_SchedulingGroupValidated && _ScheduleForPWNodeList.Where(c => c.MDSchedulingGroup == null).Any())
                        Messages.Error(this, "A Scheduling-Group was removed. Invoke Reset on Scheduler");
                    _SchedulingGroupValidated = true;

                    _RulesForCurrentUser = GetRulesForCurrentUser();

                    if (_RulesForCurrentUser != null && _RulesForCurrentUser.Any() && !Root.Environment.User.IsSuperuser)
                    {
                        return _ScheduleForPWNodeList.Where(c => c.MDSchedulingGroup != null && _RulesForCurrentUser.Any(r => r.RuleParamID == c.MDSchedulingGroupID))
                                                     .OrderBy(c => c.MDSchedulingGroup.SortIndex)
                                                     .ThenBy(c => c.MDSchedulingGroup.MDSchedulingGroupName);
                    }

                    return _ScheduleForPWNodeList
                        .Where(c => c.MDSchedulingGroup != null)
                        .OrderBy(c => c.MDSchedulingGroup.SortIndex)
                        .ThenBy(c => c.MDSchedulingGroup.MDSchedulingGroupName);
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
                OnPropertyChanged(nameof(SelectedFilterBatchPlanStartMode));
            }
        }

        private ACValueItemList _FilterBatchPlanStartModeList;
        [ACPropertyList(999, "FilterBatchPlanStartMode")]
        public ACValueItemList FilterBatchPlanStartModeList
        {
            get
            {
                if (_FilterBatchPlanStartModeList == null)
                    _FilterBatchPlanStartModeList = DatabaseApp.BatchPlanStartModeEnumList as ACValueItemList;
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
                    OnPropertyChanged(nameof(SelectedTargetScheduleForPWNode));
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

        /// <summary>
        /// Source Property: 
        /// </summary>
        private bool _FilterBatchSelectAll;
        [ACPropertyInfo(999, "FilterBatchSelectAll", "en{'Select all'}de{'Alles auswählen'}")]
        public bool FilterBatchSelectAll
        {
            get
            {
                return _FilterBatchSelectAll;
            }
            set
            {
                if (_FilterBatchSelectAll != value)
                {
                    _FilterBatchSelectAll = value;
                    OnPropertyChanged(nameof(FilterBatchSelectAll));

                    foreach (var item in ProdOrderBatchPlanList)
                        item.IsSelected = value;

                    OnPropertyChanged(nameof(ProdOrderBatchPlanList));
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilterBatchProgramNo;
        [ACPropertyInfo(999, "FilterBatchProgramNo", ConstApp.ProdOrderProgramNo)]
        public string FilterBatchProgramNo
        {
            get
            {
                return _FilterBatchProgramNo;
            }
            set
            {
                if (_FilterBatchProgramNo != value)
                {
                    _FilterBatchProgramNo = value;
                    OnPropertyChanged(nameof(FilterBatchProgramNo));
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilterBatchMaterialNo;
        [ACPropertyInfo(999, "FilterBatchMaterialNo", ConstApp.Material)]
        public string FilterBatchMaterialNo
        {
            get
            {
                return _FilterBatchMaterialNo;
            }
            set
            {
                if (_FilterBatchMaterialNo != value)
                {
                    _FilterBatchMaterialNo = value;
                    OnPropertyChanged(nameof(FilterBatchMaterialNo));
                }
            }
        }

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
                    OnPropertyChanged(nameof(FilterStartTime));
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
                    OnPropertyChanged(nameof(FilterEndTime));
                }
            }
        }

        #region Properties -> (Tab)BatchPlanScheduler -> Filter (Search) -> FilterConnectedLine [PAScheduleForPWNode]


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
                    OnPropertyChanged(nameof(SelectedFilterConnectedLine));
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

        #region Properties -> (Tab)BatchPlanScheduler -> Filter (Search) -> FilterBatchPlanGroup [MDBatchPlanGroup]


        private MDBatchPlanGroup _SelectedFilterBatchPlanGroup;
        /// <summary>
        /// Selected property for PAScheduleForPWNode
        /// </summary>
        /// <value>The selected FilterConnectedLine</value>
        [ACPropertySelected(9999, "FilterBatchPlanGroup", "en{'Batchplan group'}de{'Batchplan Gruppe'}")]
        public MDBatchPlanGroup SelectedFilterBatchPlanGroup
        {
            get
            {
                return _SelectedFilterBatchPlanGroup;
            }
            set
            {
                if (_SelectedFilterBatchPlanGroup != value)
                {
                    _SelectedFilterBatchPlanGroup = value;
                    OnPropertyChanged(nameof(SelectedFilterBatchPlanGroup));
                }
            }
        }


        private List<MDBatchPlanGroup> _FilterBatchPlanGroupList;
        /// <summary>
        /// List property for PAScheduleForPWNode
        /// </summary>
        /// <value>The FilterConnectedLine list</value>
        [ACPropertyList(9999, "FilterBatchPlanGroup", "en{'Batchplan group'}de{'Batchplan Gruppe'}")]
        public List<MDBatchPlanGroup> FilterBatchPlanGroupList
        {
            get
            {
                if (_FilterBatchPlanGroupList == null)
                    _FilterBatchPlanGroupList = LoadFilterBatchPlanGroupList();
                return _FilterBatchPlanGroupList;
            }
        }

        private List<MDBatchPlanGroup> LoadFilterBatchPlanGroupList()
        {
            return DatabaseApp.MDBatchPlanGroup.OrderBy(c => c.SortIndex).ToList();
        }


        #endregion

        /// <summary>
        /// Source Property: 
        /// </summary>
        private bool _FilterOnlyOnThisLine;
        [ACPropertySelected(999, "FilterOnlyOnThisLine", "en{'Batch on this line'}de{'Charge auf dieser Linie'}")]
        public bool FilterOnlyOnThisLine
        {
            get
            {
                return _FilterOnlyOnThisLine;
            }
            set
            {
                if (_FilterOnlyOnThisLine != value)
                {
                    _FilterOnlyOnThisLine = value;
                    OnPropertyChanged(nameof(FilterOnlyOnThisLine));
                }
            }
        }

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
                OnPropertyChanged(nameof(SelectedProdOrderBatchPlan));
            }
        }

        private void _SelectedProdOrderBatchPlan_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PartialTargetCount"
                && SelectedScheduleForPWNode != null
                && SelectedScheduleForPWNode.StartMode == BatchPlanStartModeEnum.SemiAutomatic
                && !_IsRefreshingBatchPlan)
            {
                if (SelectedProdOrderBatchPlan.PartialTargetCount.HasValue && SelectedProdOrderBatchPlan.PartialTargetCount > 0)
                {
                    if (SelectedProdOrderBatchPlan.PlanState <= GlobalApp.BatchPlanState.Created
                        || SelectedProdOrderBatchPlan.PlanState >= GlobalApp.BatchPlanState.Paused)
                        SetReadyToStart(new ProdOrderBatchPlan[] { SelectedProdOrderBatchPlan });
                }
                else if (SelectedProdOrderBatchPlan.PartialTargetCount.HasValue && SelectedProdOrderBatchPlan.PartialTargetCount <= 0)
                {
                    SelectedProdOrderBatchPlan.PartialTargetCount = null;
                    if (SelectedProdOrderBatchPlan.PlanState == GlobalApp.BatchPlanState.ReadyToStart)
                    {
                        SelectedProdOrderBatchPlan.PlanState = GlobalApp.BatchPlanState.Paused;
                        Save();
                    }
                }
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
                OnPropertyChanged(nameof(ProdOrderBatchPlanList));
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
                        FilterPlanningMR?.PlanningMRID,
                        SelectedFilterBatchPlanGroup?.MDBatchPlanGroupID,
                        FilterBatchProgramNo,
                        FilterBatchMaterialNo);

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
                       FilterPlanningMR?.PlanningMRID,
                       SelectedFilterBatchPlanGroup?.MDBatchPlanGroupID,
                       FilterBatchProgramNo,
                       FilterBatchMaterialNo)
                   .Select(c => c.ProdOrderPartslist.ProdOrderID);
                    prodOrderBatchPlans = new ObservableCollection<ProdOrderBatchPlan>(prodOrderBatchPlans.Where(c => includedProductionOrders.Contains(c.ProdOrderPartslist.ProdOrderID)));
                }
            }
            catch (Exception ex)
            {
                this.Messages.LogException(this.GetACUrl(), "GetProdOrderBatchPlanList(10)", ex);
                this.Messages.LogException(this.GetACUrl(), "GetProdOrderBatchPlanList(10)", ex.StackTrace);
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
                OnPropertyChanged(nameof(ScheduledStartDate));
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
                OnPropertyChanged(nameof(ScheduledEndDate));
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
                    OnPropertyChanged(nameof(FilterPlanningMR));
                }
            }
        }

        #endregion

        #endregion

        #region Properties -> (Tab)ProdOrder

        #region Properties -> (Tab)ProdOrder -> Filter

        /// <summary>
        /// Select all
        /// </summary>
        private bool _FilterOrderSelectAll;
        [ACPropertyInfo(999, "FilterOrderSelectAll", "en{'Select all'}de{'Alles auswählen'}")]
        public bool FilterOrderSelectAll
        {
            get
            {
                return _FilterOrderSelectAll;
            }
            set
            {
                if (_FilterOrderSelectAll != value)
                {
                    _FilterOrderSelectAll = value;
                    OnPropertyChanged(nameof(FilterOrderSelectAll));

                    foreach (var item in ProdOrderPartslistList)
                        item.IsSelected = value;

                    OnPropertyChanged(nameof(ProdOrderPartslistList));
                }
            }
        }

        private DateTime? _FilterOrderStartTime;
        /// <summary>
        /// filter from date
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
                    OnPropertyChanged(nameof(FilterOrderStartTime));
                    OnPropertyChanged(nameof(FilterOrderEndTime));
                }
            }
        }

        private DateTime? _FilterOrderEndTime;
        /// <summary>
        /// Filter to date
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
                    OnPropertyChanged(nameof(FilterOrderStartTime));
                    OnPropertyChanged(nameof(FilterOrderEndTime));
                }
            }
        }

        private bool? _FilterOrderIsCompleted = false;
        /// <summary>
        /// Filter for finshed orders
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
                    OnPropertyChanged(nameof(FilterOrderIsCompleted));
                    OnPropertyChanged(nameof(FilterOrderStartTime));
                    OnPropertyChanged(nameof(FilterOrderEndTime));
                }
            }
        }

        private string _FilterDepartmentUserName;
        /// <summary>
        /// Filter departement
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "FilterDepartmentUserName", "en{'Department'}de{'Abteilung'}")]
        public string FilterDepartmentUserName
        {
            get
            {
                return _FilterDepartmentUserName;
            }
            set
            {
                if (_FilterDepartmentUserName != value)
                {
                    _FilterDepartmentUserName = value;
                    OnPropertyChanged(nameof(FilterDepartmentUserName));
                }
            }
        }

        /// <summary>
        /// Filter Order numer
        /// </summary>
        private string _FilterOrderProgramNo;
        [ACPropertyInfo(999, "FilterOrderProgramNo", ConstApp.ProdOrderProgramNo)]
        public string FilterOrderProgramNo
        {
            get
            {
                return _FilterOrderProgramNo;
            }
            set
            {
                if (_FilterOrderProgramNo != value)
                {
                    _FilterOrderProgramNo = value;
                    OnPropertyChanged(nameof(FilterOrderProgramNo));
                }
            }
        }

        /// <summary>
        /// Filter Material number
        /// </summary>
        private string _FilterOrderMaterialNo;
        [ACPropertyInfo(999, "FilterOrderMaterialNo", ConstApp.Material)]
        public string FilterOrderMaterialNo
        {
            get
            {
                return _FilterOrderMaterialNo;
            }
            set
            {
                if (_FilterOrderMaterialNo != value)
                {
                    _FilterOrderMaterialNo = value;
                    OnPropertyChanged(nameof(FilterOrderMaterialNo));
                }
            }
        }


        #region Properties -> (Tab)ProdOrder -> Filter -> FilterProdPartslistOrder


        public BatchPlanProdOrderSortFilterEnum? FilterProdPartslistOrder
        {
            get
            {
                if (SelectedFilterProdPartslistOrder == null)
                    return null;
                return (BatchPlanProdOrderSortFilterEnum)SelectedFilterProdPartslistOrder.Value;
            }
        }


        private ACValueItem _SelectedFilterProdPartslistOrder;
        /// <summary>
        /// Selected property for ACValueItem
        /// </summary>
        /// <value>The selected FilterProdPartslistOrder</value>
        [ACPropertySelected(305, "FilterProdPartslistOrder", "en{'Sort order'}de{'Sortierreihenfolge'}")]
        public ACValueItem SelectedFilterProdPartslistOrder
        {
            get
            {
                return _SelectedFilterProdPartslistOrder;
            }
            set
            {
                if (_SelectedFilterProdPartslistOrder != value)
                {
                    _SelectedFilterProdPartslistOrder = value;
                    OnPropertyChanged(nameof(SelectedFilterProdPartslistOrder));
                }
            }
        }

        private List<ACValueItem> _FilterProdPartslistOrderList;
        /// <summary>
        /// List property for ACValueItem
        /// </summary>
        /// <value>The FilterPickingState list</value>
        [ACPropertyList(306, "FilterProdPartslistOrder")]
        public List<ACValueItem> FilterProdPartslistOrderList
        {
            get
            {
                if (_FilterProdPartslistOrderList == null)
                    _FilterProdPartslistOrderList = LoadFilterProdPartslistOrderList();
                return _FilterProdPartslistOrderList;
            }
        }

        public ACValueItemList LoadFilterProdPartslistOrderList()
        {
            ACValueItemList list = null;
            gip.core.datamodel.ACClass enumClass = Database.ContextIPlus.GetACType(typeof(BatchPlanProdOrderSortFilterEnum));
            if (enumClass != null && enumClass.ACValueListForEnum != null)
                list = enumClass.ACValueListForEnum;
            else
                list = new ACValueListBatchPlanProdOrderSortFilterEnum();
            return list;
        }

        #endregion

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
                OnPropertyChanged(nameof(SelectedProdOrderPartslist));
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
                OnPropertyChanged(nameof(ProdOrderPartslistList));
            }
        }

        public virtual IEnumerable<ProdOrderPartslistPlanWrapper> GetProdOrderPartslistList()
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
                    (short?)maxProdOrderState,
                    FilterOnlyOnThisLine,
                    FilterDepartmentUserName,
                    FilterOrderProgramNo,
                    FilterOrderMaterialNo) as ObjectQuery<ProdOrderPartslistPlanWrapper>;

            batchQuery.MergeOption = MergeOption.OverwriteChanges;
            IOrderedQueryable<ProdOrderPartslistPlanWrapper> query = null;
            if (FilterProdPartslistOrder != null)
                switch (FilterProdPartslistOrder)
                {
                    case BatchPlanProdOrderSortFilterEnum.Material:
                        query = batchQuery.OrderBy(c => c.ProdOrderPartslist.Partslist.Material.MaterialNo).ThenBy(c => c.ProdOrderPartslist.StartDate);
                        break;
                    case BatchPlanProdOrderSortFilterEnum.StartTime:
                        query = batchQuery.OrderBy(c => c.ProdOrderPartslist.StartDate);
                        break;
                    case BatchPlanProdOrderSortFilterEnum.ProgramNo:
                        query = batchQuery.OrderBy(c => c.ProdOrderPartslist.ProdOrder.ProgramNo);
                        break;
                }
            return query.Take(Const_MaxResultSize).ToList();
        }

        protected static readonly Func<DatabaseApp, Guid, Guid?, DateTime?, DateTime?, short?, short?, bool, string, string, string, IQueryable<ProdOrderPartslistPlanWrapper>> s_cQry_ProdOrderPartslistForPWNode =
        CompiledQuery.Compile<DatabaseApp, Guid, Guid?, DateTime?, DateTime?, short?, short?, bool, string, string, string, IQueryable<ProdOrderPartslistPlanWrapper>>(
            (ctx, mdSchedulingGroupID, planningMRID, filterStartTime, filterEndTime, minProdOrderState, maxProdOrderState, filterOnlyOnThisLine, departmentUserName, programNo, materialNo) =>
                ctx
                .ProdOrderPartslist
                .Include("MDProdOrderState")
                .Include("ProdOrder")
                .Include("Partslist")
                .Include("Partslist.Material")
                .Include("Partslist.Material.BaseMDUnit")
                //.Include("Partslist.Material.MaterialUnit_Material")
                //.Include("Partslist.Material.MaterialUnit_Material.ToMDUnit")
                .Where(c =>
                        (minProdOrderState == null || (c.MDProdOrderState.MDProdOrderStateIndex >= minProdOrderState && c.ProdOrder.MDProdOrderState.MDProdOrderStateIndex >= minProdOrderState))
                        && (programNo == null || (c.ProdOrder.ProgramNo.Contains(programNo)))
                        && (
                                string.IsNullOrEmpty(materialNo)
                                || (c.Partslist.Material.MaterialNo.Contains(materialNo) || c.Partslist.Material.MaterialName1.Contains(materialNo))
                            )
                        && (maxProdOrderState == null || (c.MDProdOrderState.MDProdOrderStateIndex <= maxProdOrderState && c.ProdOrder.MDProdOrderState.MDProdOrderStateIndex <= maxProdOrderState))
                        && (
                                (!planningMRID.HasValue && !c.PlanningMRProposal_ProdOrderPartslist.Any())
                                || (planningMRID.HasValue && c.PlanningMRProposal_ProdOrderPartslist.Any(x => x.PlanningMRID == planningMRID))
                        )
                        && (
                            filterStartTime == null
                            ||
                            c.StartDate >= filterStartTime
                        )
                        && (
                            filterEndTime == null
                            ||
                            c.StartDate < filterEndTime
                        )

                        &&
                        (
                            (!filterOnlyOnThisLine && c
                            .Partslist
                            .PartslistACClassMethod_Partslist
                            .Where(d => d
                                        .MaterialWFACClassMethod
                                        .ACClassMethod.ACClassWF_ACClassMethod
                                        .SelectMany(x => x.MDSchedulingGroupWF_VBiACClassWF)
                                        .Where(x => x.MDSchedulingGroupID == mdSchedulingGroupID)
                                        .Any()
                            ).Any())
                        ||
                            (filterOnlyOnThisLine &&
                            c
                            .ProdOrderBatchPlan_ProdOrderPartslist
                            .Where(d => d
                                    .VBiACClassWF
                                    .MDSchedulingGroupWF_VBiACClassWF
                                    .Where(x => x.MDSchedulingGroupID == mdSchedulingGroupID)
                                    .Any()
                            ).Any())
                        )
                    && (departmentUserName == null || c.DepartmentUserName.Contains(departmentUserName))
                )
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
                OnPropertyChanged(nameof(CurrentMsg));
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
            OnPropertyChanged(nameof(MsgList));
        }

        public void ClearMessages()
        {
            MsgList.Clear();
            OnPropertyChanged(nameof(MsgList));
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
                    OnPropertyChanged(nameof(WizardPhase));
                    OnPropertyChanged(nameof(WizardPhaseTitle));
                    //OnPropertyChanged(nameof(WizardPhaseSubTitle));
                    //OnPropertyChanged(nameof(WizardDesign));
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
                return wizardPhaseTitleList.FirstOrDefault(c => ((short)c.Value) == (short)WizardPhase)?.ACCaption;
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

        /// <summary>
        /// Source Property: 
        /// </summary>
        private bool _IsWizard;
        [ACPropertySelected(999, "IsWizard", "en{'TODO:IsWizard'}de{'TODO:IsWizard'}")]
        public bool IsWizard
        {
            get
            {
                return _IsWizard;
            }
            set
            {
                if (_IsWizard != value)
                {
                    _IsWizard = value;
                    OnPropertyChanged(nameof(IsWizard));
                }
            }
        }

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
                    OnPropertyChanged(nameof(WizardPhaseErrorMessage));
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
            OnPropertyChanged(nameof(SelectedWizardSchedulerPartslist));
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
                if (e.PropertyName == nameof(WizardSchedulerPartslist.SelectedMDSchedulingGroup))
                {
                    item.LoadConfiguration();
                }
                else if (e.PropertyName == nameof(WizardSchedulerPartslist.NewTargetQuantityUOM))
                {
                    if (item.NewTargetQuantityUOM > 0 && item.ProdOrderPartslistPos != null)
                    {
                        Msg changeMsg = ProdOrderManager.ProdOrderPartslistChangeTargetQuantity(DatabaseApp, item.ProdOrderPartslistPos.ProdOrderPartslist, item.NewTargetQuantityUOM);
                        if (changeMsg != null)
                            SendMessage(changeMsg);
                    }
                }
                else if (e.PropertyName == nameof(WizardSchedulerPartslist.NewSyncTargetQuantityUOM))
                {
                    if (item.NewSyncTargetQuantityUOM != null)
                    {
                        // make recalc
                        double factor = item.NewSyncTargetQuantityUOM.Value / item.NewTargetQuantityUOM;
                        item.ChangeNewTargetQuantityUOM(item.NewSyncTargetQuantityUOM.Value, false);

                        foreach (WizardSchedulerPartslist otherItem in AllWizardSchedulerPartslistList)
                        {
                            if (!item.IsEqualPartslist(otherItem))
                            {
                                otherItem.ChangeNewTargetQuantityUOM(otherItem.NewTargetQuantityUOM * factor, false);
                                otherItem._NewSyncTargetQuantityUOM = null;
                            }
                        }

                        // setup value to null
                        item._NewSyncTargetQuantityUOM = null;
                        item._NewSyncTargetQuantity = null;

                        foreach (WizardSchedulerPartslist wizardItem in AllWizardSchedulerPartslistList)
                        {
                            if (wizardItem.ProdOrderPartslistPos != null)
                            {
                                Msg changeMsg = ProdOrderManager.ProdOrderPartslistChangeTargetQuantity(DatabaseApp, wizardItem.ProdOrderPartslistPos.ProdOrderPartslist, wizardItem.NewTargetQuantityUOM);
                                if (changeMsg != null)
                                    SendMessage(changeMsg);
                            }
                        }
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
                OnPropertyChanged(nameof(WizardSchedulerPartslistList));
            }
        }

        public void Item_PropertyChanged_Common()
        {

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
                return AllWizardSchedulerPartslistList
                    .Where(c => !c.IsSolved)
                    .OrderBy(c => c.Sn)
                    .ToList();
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
                    OnPropertyChanged(nameof(CurrentPartListExpand));
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

        [ACPropertySelected(516, "FilterBatchplanSuggestionMode", "en{'Calculation formula'}de{'Berechnungsformel'}")]
        public ACValueItem SelectedFilterBatchplanSuggestionMode
        {
            get
            {
                ACValueItem itemValue = null;
                if (SelectedWizardSchedulerPartslist != null
                    && FilterBatchplanSuggestionModeList != null
                    && SelectedWizardSchedulerPartslist.BatchSuggestionMode.HasValue)
                {
                    itemValue = FilterBatchplanSuggestionModeList.Where(c => (BatchSuggestionCommandModeEnum)c.Value == SelectedWizardSchedulerPartslist.BatchSuggestionMode.Value).FirstOrDefault();
                }
                return itemValue;
            }
            set
            {
                BatchSuggestionCommandModeEnum? itemValue = null;
                if (value != null)
                {
                    itemValue = (BatchSuggestionCommandModeEnum)value.Value;
                }
                if (SelectedWizardSchedulerPartslist != null && SelectedWizardSchedulerPartslist.BatchSuggestionMode != itemValue)
                {
                    SelectedWizardSchedulerPartslist.BatchSuggestionMode = itemValue;
                }
                OnPropertyChanged(nameof(SelectedFilterBatchplanSuggestionMode));
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
                    var acClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(BatchSuggestionCommandModeEnum));
                    if (acClass != null)
                        _FilterBatchplanSuggestionModeList = acClass.ACValueListForEnum;
                }
                return _FilterBatchplanSuggestionModeList;
            }
        }

        [ACMethodInfo("RecalculateBatchSuggestion", "en{'Calculate'}de{'Berechnung'}", 999)]
        public void RecalculateBatchSuggestion()
        {
            if (!IsEnabledRecalculateBatchSuggestion())
                return;
            SelectedWizardSchedulerPartslist.LoadNewBatchSuggestion();
        }

        public bool IsEnabledRecalculateBatchSuggestion()
        {
            return
                       SelectedWizardSchedulerPartslist != null
                    && SelectedWizardSchedulerPartslist.BatchSuggestionMode != null
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
                SelectedWizardSchedulerPartslist.NewTargetQuantityUOM > 0 ? SelectedWizardSchedulerPartslist.NewTargetQuantityUOM : SelectedWizardSchedulerPartslist.TargetQuantityUOM, null, true);
            SelectedWizardSchedulerPartslist.BatchPlanSuggestion.AddItem(newItem);
            SelectedWizardSchedulerPartslist.BatchPlanSuggestion.SelectedItems = newItem;
            if (SelectedWizardSchedulerPartslist.OffsetToEndTime != null)
                SelectedWizardSchedulerPartslist.LoadSuggestionItemExpectedBatchEndTime();

            var tmpItem = SelectedWizardSchedulerPartslist;
            _SelectedWizardSchedulerPartslist = null;
            SelectedWizardSchedulerPartslist = tmpItem;
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
                OnPropertyChanged(nameof(BatchPlanTimelineItemsRoot));
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
                OnPropertyChanged(nameof(SelectedBatchPlanTimelineItemRoot));
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
                OnPropertyChanged(nameof(BatchPlanTimelineItems));
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
                OnPropertyChanged(nameof(SelectedBatchPlanTimelineItem));
            }
        }

        #endregion

        #region Properties -> MDBatchPlanGroup


        #region MDBatchPlanGroup
        private MDBatchPlanGroup _SelectedMDBatchPlanGroup;
        /// <summary>
        /// Selected property for MDBatchPlanGroup
        /// </summary>
        /// <value>The selected MDBatchPlanGroup</value>
        [ACPropertySelected(9999, "PropertyGroupName", "en{'TODO: MDBatchPlanGroup'}de{'TODO: MDBatchPlanGroup'}")]
        public MDBatchPlanGroup SelectedMDBatchPlanGroup
        {
            get
            {
                return _SelectedMDBatchPlanGroup;
            }
            set
            {
                if (_SelectedMDBatchPlanGroup != value)
                {
                    _SelectedMDBatchPlanGroup = value;
                    OnPropertyChanged(nameof(SelectedMDBatchPlanGroup));
                }
            }
        }


        private List<MDBatchPlanGroup> _MDBatchPlanGroupList;
        /// <summary>
        /// List property for MDBatchPlanGroup
        /// </summary>
        /// <value>The MDBatchPlanGroup list</value>
        [ACPropertyList(9999, "PropertyGroupName")]
        public List<MDBatchPlanGroup> MDBatchPlanGroupList
        {
            get
            {
                if (_MDBatchPlanGroupList == null)
                    _MDBatchPlanGroupList = LoadMDBatchPlanGroupList();
                return _MDBatchPlanGroupList;
            }
        }

        private List<MDBatchPlanGroup> LoadMDBatchPlanGroupList()
        {
            return DatabaseApp.MDBatchPlanGroup.OrderBy(c => c.SortIndex).ToList();
        }
        #endregion


        #endregion

        #region Properties => Configuration Rules

        private ACPropertyConfigValue<string> _BSOBatchPlanSchedulerRules;
        [ACPropertyConfig("en{'Work center rules'}de{'Work center rules'}")]
        public string BSOBatchPlanSchedulerRules
        {
            get => _BSOBatchPlanSchedulerRules.ValueT;
            set
            {
                _BSOBatchPlanSchedulerRules.ValueT = value;
                OnPropertyChanged();
            }
        }

        private List<UserRuleItem> _TempRules;

        private List<UserRuleItem> _RulesForCurrentUser;

        private core.datamodel.VBUser _SelectedVBUser;
        [ACPropertySelected(604, "VBUser", "en{'User'}de{'Benutzer'}")]
        public core.datamodel.VBUser SelectedVBUser
        {
            get => _SelectedVBUser;
            set
            {
                _SelectedVBUser = value;
                OnPropertyChanged(nameof(SelectedVBUser));
            }
        }

        private core.datamodel.VBUser[] _VBUserList;
        [ACPropertyList(605, "VBUser")]
        public IEnumerable<core.datamodel.VBUser> VBUserList
        {
            get
            {
                if (_VBUserList == null)
                {
                    using (ACMonitor.Lock(DatabaseApp.ContextIPlus.QueryLock_1X000))
                    {
                        _VBUserList = DatabaseApp.ContextIPlus.VBUser.OrderBy(c => c.VBUserName).ToArray();
                    }
                }
                return _VBUserList;
            }
        }

        private UserRuleItem _SelectedAssignedUserRule;
        [ACPropertySelected(606, "UserRelatedRules", "en{'Assigned user rule'}de{'Assigned user rule'}")]
        public UserRuleItem SelectedAssignedUserRule
        {
            get => _SelectedAssignedUserRule;
            set
            {


                _SelectedAssignedUserRule = value;
                OnPropertyChanged();
            }
        }

        private IEnumerable<UserRuleItem> _AssignedUserRules;
        [ACPropertyList(607, "UserRelatedRules")]
        public IEnumerable<UserRuleItem> AssignedUserRules
        {
            get => _AssignedUserRules;
            set
            {
                _AssignedUserRules = value;
                OnPropertyChanged();
            }
        }

        private MDSchedulingGroup _SelectedAvailableSchedulingGroup;
        [ACPropertySelected(608, "AvailableRules", "en{'Available scheduling groups'}de{'Available scheduling groups'}")]
        public MDSchedulingGroup SelectedAvailableSchedulingGroup
        {
            get => _SelectedAvailableSchedulingGroup;
            set
            {
                _SelectedAvailableSchedulingGroup = value;
                OnPropertyChanged();
            }
        }

        private List<MDSchedulingGroup> _AvailableSchedulingGroupsList;
        [ACPropertyList(609, "AvailableRules")]
        public List<MDSchedulingGroup> AvailableSchedulingGroupsList
        {
            get => _AvailableSchedulingGroupsList;
            set
            {
                _AvailableSchedulingGroupsList = value;
                OnPropertyChanged();
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
                OnPropertyChanged(nameof(ScheduleForPWNodeList));
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

        [ACMethodCommand("ChangeMode", "en{'Change Mode'}de{'Mode ändern'}", 501, true)]
        public void ChangeMode()
        {
            if (!IsEnabledChangeMode())
                return;
            PAScheduleForPWNode updateNode = new PAScheduleForPWNode();
            updateNode.CopyFrom(SelectedScheduleForPWNode, true);
            updateNode.StartMode = (vd.BatchPlanStartModeEnum)SelectedFilterBatchPlanStartMode.Value;
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
            if (SelectedProdOrderBatchPlan.PlanMode == BatchPlanMode.UseBatchCount)
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
                        List<vd.ProdOrderBatchPlan> changedBatchPlans;
                        if (moveBatchCount == SelectedProdOrderBatchPlan.BatchTargetCount)
                        {
                            if (!MoveBatchToOtherProdLine(SelectedProdOrderBatchPlan, SelectedTargetScheduleForPWNode))
                                return;
                            isMove = true;
                            changedBatchPlans = new List<ProdOrderBatchPlan>();
                            changedBatchPlans.Add(SelectedProdOrderBatchPlan);
                        }
                        else
                        {
                            int remainingBatchCount = SelectedProdOrderBatchPlan.BatchTargetCount - moveBatchCount;
                            ProdOrderBatchPlan oldBatchPlan = SelectedProdOrderBatchPlan;
                            SelectedProdOrderBatchPlan.BatchTargetCount = remainingBatchCount;
                            CreateNewBatchWithCount(SelectedProdOrderBatchPlan, moveBatchCount, SelectedTargetScheduleForPWNode, out changedBatchPlans);
                            if (remainingBatchCount <= 0)
                            {
                                oldBatchPlan.IsSelected = true;
                                SetBatchStateCancelled();
                            }
                        }
                        if (changedBatchPlans != null && changedBatchPlans.Any())
                        {
                            ProdOrderBatchPlan changedPlan = changedBatchPlans.FirstOrDefault();
                            ChangeBatchPlanForSchedule(changedPlan, SelectedTargetScheduleForPWNode);
                            if (this.WizardSchedulerPartslistList != null)
                            {
                                WizardSchedulerPartslist wizardLine = this.WizardSchedulerPartslistList.Where(c => c.Partslist == changedPlan.ProdOrderPartslist.Partslist).FirstOrDefault();
                                if (wizardLine != null)
                                    WizardForwardSelectLinie(wizardLine);
                            }
                        }
                    }
                }
            }
            else if (SelectedProdOrderBatchPlan.PlanMode == BatchPlanMode.UseTotalSize)
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
                        List<vd.ProdOrderBatchPlan> changedBatchPlans;
                        if (moveQuantity == SelectedProdOrderBatchPlan.BatchSize)
                        {
                            if (!MoveBatchToOtherProdLine(SelectedProdOrderBatchPlan, SelectedTargetScheduleForPWNode))
                                return;
                            isMove = true;
                            changedBatchPlans = new List<ProdOrderBatchPlan>();
                            changedBatchPlans.Add(SelectedProdOrderBatchPlan);
                        }
                        else
                        {
                            double remainingBatchSize = SelectedProdOrderBatchPlan.BatchSize - moveQuantity;
                            ProdOrderBatchPlan oldBatchPlan = SelectedProdOrderBatchPlan;
                            SelectedProdOrderBatchPlan.BatchSize = remainingBatchSize;
                            CreateNewBatchWithSize(SelectedProdOrderBatchPlan, moveQuantity, SelectedTargetScheduleForPWNode, out changedBatchPlans);
                            if (remainingBatchSize <= 0.000001)
                            {
                                oldBatchPlan.IsSelected = true;
                                SetBatchStateCancelled();
                            }
                        }
                        if (changedBatchPlans != null && changedBatchPlans.Any())
                        {
                            ProdOrderBatchPlan changedPlan = changedBatchPlans.FirstOrDefault();
                            ChangeBatchPlanForSchedule(changedPlan, SelectedTargetScheduleForPWNode);
                            if (this.WizardSchedulerPartslistList != null)
                            {
                                WizardSchedulerPartslist wizardLine = this.WizardSchedulerPartslistList.Where(c => c.Partslist == changedPlan.ProdOrderPartslist.Partslist).FirstOrDefault();
                                if (wizardLine != null)
                                    WizardForwardSelectLinie(wizardLine);
                            }
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
                    OnPropertyChanged(nameof(ProdOrderBatchPlanList));
                }
            }
        }


        public bool IsEnabledMoveToOtherLine()
        {
            return SelectedProdOrderBatchPlan != null
                && SelectedProdOrderBatchPlan.ProdOrderPartslist != null
                && SelectedProdOrderBatchPlan.PlanState < GlobalApp.BatchPlanState.Completed
                && (
                       (SelectedProdOrderBatchPlan.PlanMode == BatchPlanMode.UseBatchCount
                        && SelectedProdOrderBatchPlan.BatchActualCount < SelectedProdOrderBatchPlan.BatchTargetCount)
                    || (SelectedProdOrderBatchPlan.PlanMode == BatchPlanMode.UseTotalSize
                        && SelectedProdOrderBatchPlan.RemainingQuantity > 0.00001)
                    )
                && SelectedTargetScheduleForPWNode != null
                && SelectedTargetScheduleForPWNode.MDSchedulingGroup != null;
        }

        #region Methods -> Explorer -> MoveToOtherLine -> Helper methods


        private bool MoveBatchToOtherProdLine(ProdOrderBatchPlan prodOrderBatchPlan, PAScheduleForPWNode selectedTargetScheduleForPWNode)
        {
            if (prodOrderBatchPlan.VBiACClassWF == null)
                return false;
            vd.ACClassWF tempACClassWFItem = selectedTargetScheduleForPWNode.MDSchedulingGroup.MDSchedulingGroupWF_MDSchedulingGroup
                                                .Where(c => c.VBiACClassWF.ACClassMethodID == prodOrderBatchPlan.VBiACClassWF.ACClassMethodID)
                                                .Select(c => c.VBiACClassWF)
                                                .FirstOrDefault();
            if (tempACClassWFItem == null)
                return false;
            prodOrderBatchPlan.VBiACClassWF = tempACClassWFItem;

            List<SchedulingMaxBPOrder> maxSchedulerOrders = ProdOrderManager.GetMaxScheduledOrder(DatabaseApp, FilterPlanningMR?.PlanningMRNo);
            int scheduledOrderNo =
                maxSchedulerOrders
                .Where(c => c.MDSchedulingGroup.MDSchedulingGroupID == selectedTargetScheduleForPWNode.MDSchedulingGroupID)
                .SelectMany(c => c.WFs)
                .Where(c => c.ACClassWF.ACClassWFID == tempACClassWFItem.ACClassWFID)
                .Select(c => c.MaxScheduledOrder)
                .DefaultIfEmpty()
                .Max();
            scheduledOrderNo++;
            prodOrderBatchPlan.ScheduledOrder = scheduledOrderNo;
            foreach (var reservation in prodOrderBatchPlan.FacilityReservation_ProdOrderBatchPlan.ToArray())
            {
                prodOrderBatchPlan.FacilityReservation_ProdOrderBatchPlan.Remove(reservation);
            }
            return true;
        }

        private void CreateNewBatchWithCount(ProdOrderBatchPlan prodOrderBatchPlan, int moveBatchCount, PAScheduleForPWNode selectedTargetScheduleForPWNode, out List<vd.ProdOrderBatchPlan> generatedBatchPlans)
        {
            double totalSize = moveBatchCount * prodOrderBatchPlan.BatchSize;
            int sn = 1;
            WizardSchedulerPartslist wizardSchedulerPartslist = new WizardSchedulerPartslist(
                DatabaseApp, ProdOrderManager, LocalBSOBatchPlan.VarioConfigManager,
                prodOrderBatchPlan.ProdOrderPartslist.Partslist, totalSize, sn, new List<MDSchedulingGroup>() { selectedTargetScheduleForPWNode.MDSchedulingGroup });
            wizardSchedulerPartslist.MDProdOrderState = prodOrderBatchPlan.ProdOrderPartslist.MDProdOrderState;
            wizardSchedulerPartslist.ProdOrderPartslistPos = prodOrderBatchPlan.ProdOrderPartslistPos;
            if (wizardSchedulerPartslist.SelectedMDSchedulingGroup != null)
                wizardSchedulerPartslist.LoadConfiguration();
            wizardSchedulerPartslist.BatchPlanSuggestion =
               new BatchPlanSuggestion(wizardSchedulerPartslist)
               {
                   RestQuantityToleranceUOM = totalSize * (ProdOrderManager.TolRemainingCallQ / 100),
                   RestNotUsedQuantityUOM = 0,
                   ItemsList = new BindingList<BatchPlanSuggestionItem>()
                   {
                        new BatchPlanSuggestionItem(wizardSchedulerPartslist,1, prodOrderBatchPlan.BatchSize, moveBatchCount, totalSize, null, true)
                   }
               };
            string programNo = prodOrderBatchPlan.ProdOrderPartslist.ProdOrder.ProgramNo;
            FactoryBatchPlans(wizardSchedulerPartslist, ref programNo, out generatedBatchPlans);
        }

        private void CreateNewBatchWithSize(ProdOrderBatchPlan prodOrderBatchPlan, double moveQuantity, PAScheduleForPWNode selectedTargetScheduleForPWNode, out List<vd.ProdOrderBatchPlan> generatedBatchPlans)
        {
            int sn = 1;
            WizardSchedulerPartslist wizardSchedulerPartslist = new WizardSchedulerPartslist(
                DatabaseApp, ProdOrderManager, LocalBSOBatchPlan.VarioConfigManager,
                prodOrderBatchPlan.ProdOrderPartslist.Partslist, moveQuantity, sn, new List<MDSchedulingGroup>() { selectedTargetScheduleForPWNode.MDSchedulingGroup });
            if (wizardSchedulerPartslist.SelectedMDSchedulingGroup != null)
                wizardSchedulerPartslist.LoadConfiguration();

            wizardSchedulerPartslist.BatchPlanSuggestion =
                new BatchPlanSuggestion(wizardSchedulerPartslist)
                {
                    RestQuantityToleranceUOM = (ProdOrderManager.TolRemainingCallQ / 100) * moveQuantity,
                    RestNotUsedQuantityUOM = 0,
                    ItemsList = new BindingList<BatchPlanSuggestionItem>()
                    {
                        new BatchPlanSuggestionItem(wizardSchedulerPartslist, 1, moveQuantity, 1, moveQuantity, null, true)
                    }
                };
            string programNo = "";
            FactoryBatchPlans(wizardSchedulerPartslist, ref programNo, out generatedBatchPlans);
        }

        private void MoveBatchSortOrderCorrect(IEnumerable<ProdOrderBatchPlan> prodOrderBatchPlans)
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
                WizardSchedulerPartslist tmpWizardPl =
                    new WizardSchedulerPartslist(DatabaseApp, ProdOrderManager, LocalBSOBatchPlan.VarioConfigManager,
                        partslist, moveQuantity, 1, new List<MDSchedulingGroup>() { SelectedTargetScheduleForPWNode.MDSchedulingGroup });
                if (tmpWizardPl.SelectedMDSchedulingGroup != null)
                    tmpWizardPl.LoadConfiguration();
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
            OnPropertyChanged(nameof(ScheduleForPWNodeList));
            LoadProdOrderBatchPlanList();
        }

        public bool IsEnabledSearch()
        {
            return SelectedScheduleForPWNode != null
                   && (SelectedFilterConnectedLine != null
                        || (FilterStartTime != null
                            && FilterEndTime != null
                            && (FilterEndTime.Value - FilterStartTime.Value).TotalDays > 0
                            && (FilterEndTime.Value - FilterStartTime.Value).TotalDays <= Const_MaxFilterDaySpan
                           )
                       );
        }

        [ACMethodCommand("New", "en{'New'}de{'Neu'}", (short)MISort.New, true)]
        public void New()
        {
            if (!IsEnabledNew())
                return;
            ClearMessages();
            ClearAndResetChildBSOLists(SelectedScheduleForPWNode.MDSchedulingGroup);
            WizardPhase = NewScheduledBatchWizardPhaseEnum.SelectMaterial;
            IsWizard = true;
            WizardForwardAction(WizardPhase);
            OnPropertyChanged(nameof(WizardPhase));
            OnPropertyChanged(nameof(CurrentLayout));
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

        [ACMethodInfo("DeleteBatch", "en{'Delete'}de{'Löschen'}", 503, true)]
        public void DeleteBatch()
        {
            ProdOrderBatchPlan batchPlan = SelectedProdOrderBatchPlan;
            MsgWithDetails msg = batchPlan.DeleteACObject(DatabaseApp, false);
            if (msg == null || msg.IsSucceded())
            {
                if (_ProdOrderBatchPlanList != null)
                    _ProdOrderBatchPlanList.Remove(batchPlan);
                OnPropertyChanged(nameof(ProdOrderBatchPlanList));
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

        [ACMethodInfo("BatchPlanEdit", "en{'Edit'}de{'Bearbeiten'}", 503, true)]
        public void BatchPlanEdit()
        {
            if (!IsEnabledBatchPlanEdit())
                return;
            ChangeBatchPlan(SelectedProdOrderBatchPlan);
        }

        public bool IsEnabledBatchPlanEdit()
        {
            return SelectedProdOrderBatchPlan != null
                && SelectedProdOrderBatchPlan.PlanState == GlobalApp.BatchPlanState.Created
                && SelectedProdOrderBatchPlan.BatchActualCount <= 0;
        }

        #endregion

        #region Methods -> (Tab)BatchPlanScheduler -> Interaction

        [ACMethodInfo("ItemDrag", "en{'Cancel'}de{'Abbrechen'}", 506, true)]
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
            OnPropertyChanged(nameof(ProdOrderBatchPlanList));
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

        [ACMethodInteraction("MoveSelectedBatchUp", "en{'Up'}de{'Oben'}", 602, false, "SelectedProdOrderBatchPlan")]
        public void MoveSelectedBatchUp()
        {
            if (!IsEnabledMoveSelectedBatchUp())
                return;

            ProdOrderBatchPlan[] plans = ProdOrderBatchPlanList.ToArray();
            ScheduledOrderManager<ProdOrderBatchPlan>.MoveUp(plans);
            ProdOrderBatchPlanList = new ObservableCollection<ProdOrderBatchPlan>(plans.OrderBy(c => c.ScheduledOrder));
        }

        public bool IsEnabledMoveSelectedBatchUp()
        {
            return
                ProdOrderBatchPlanList != null
                && ProdOrderBatchPlanList.Any(c => c.IsSelected);
        }

        [ACMethodInteraction("MoveSelectedBatchDown", "en{'Down'}de{'Unten'}", 603, false, "SelectedProdOrderBatchPlan")]
        public void MoveSelectedBatchDown()
        {
            if (!IsEnabledMoveSelectedBatchDown())
                return;

            ProdOrderBatchPlan[] plans = ProdOrderBatchPlanList.ToArray();
            ScheduledOrderManager<ProdOrderBatchPlan>.MoveDown(plans);
            ProdOrderBatchPlanList = new ObservableCollection<ProdOrderBatchPlan>(plans.OrderBy(c => c.ScheduledOrder));
        }

        public bool IsEnabledMoveSelectedBatchDown()
        {
            return IsEnabledMoveSelectedBatchUp();
        }

        [ACMethodInteraction("ShowComponents", "en{'Show components'}de{'Komponenten anzeigen'}", 503, false, "SelectedProdOrderBatchPlan")]
        public void ShowComponents()
        {
            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(
                new PAOrderInfoEntry()
                {
                    EntityID = SelectedProdOrderBatchPlan.ProdOrderBatchPlanID,
                    EntityName = ProdOrderBatchPlan.ClassName
                });
                service.ShowDialogComponents(this, info);
            }
        }

        public bool IsEnabledShowComponents()
        {
            return SelectedProdOrderBatchPlan != null;
        }

        [ACMethodInteraction("ShowParslist", "en{'Show recipe'}de{'Rezept Anzeigen'}", 605, true, "SelectedProdOrderBatchPlan", Global.ACKinds.MSMethodPrePost)]
        public void ShowParslist()
        {
            double treeQuantityRatio = SelectedProdOrderBatchPlan.ProdOrderPartslist.TargetQuantity / SelectedProdOrderBatchPlan.ProdOrderPartslist.Partslist.TargetQuantityUOM;
            rootPartslistExpand = new PartslistExpand(SelectedProdOrderBatchPlan.ProdOrderPartslist.Partslist, treeQuantityRatio);
            rootPartslistExpand.IsChecked = true;
            rootPartslistExpand.LoadTree();

            if (rootPartslistExpand.Children == null || rootPartslistExpand.Children.Count == 0)
            {
                string partslistNo = rootPartslistExpand.PartslistNo;
                StartPartslistBSO(partslistNo);
            }
            else
            {
                _PartListExpandList = new List<PartslistExpand>() { rootPartslistExpand };
                CurrentPartListExpand = rootPartslistExpand;
                ShowDialog(this, "ProdOrderPartslistExpandDlg");
            }
        }


        public bool IsEnabledShowParslist()
        {
            return SelectedProdOrderBatchPlan != null;
        }

        [ACMethodInfo("ShowPartslistOK", "en{'Ok'}de{'Ok'}", 999)]
        public void ShowPartslistOK()
        {
            CloseTopDialog();
            string partslistNo = CurrentPartListExpand.PartslistNo;
            StartPartslistBSO(partslistNo);
        }

        public bool IsEnabledShowPartslistOK()
        {
            return CurrentPartListExpand != null;
        }

        private void StartPartslistBSO(string partslistNo)
        {
            ACMethod acMethod = Root.ACType.ACType.ACUrlACTypeSignature(Const.BusinessobjectsACUrl + ACUrlHelper.Delimiter_Start + Const.BSOiPlusStudio);
            acMethod.ParameterValueList["AutoFilter"] = partslistNo;
            var acClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(Partslist));
            if (acClass != null && acClass.ManagingBSO != null)
                this.Root().RootPageWPF.StartBusinessobject(Const.BusinessobjectsACUrl + ACUrlHelper.Delimiter_Start + acClass.ManagingBSO.ACIdentifier, acMethod.ParameterValueList);
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

        [ACMethodCommand("BackwardSchedulingOk", "en{'Ok'}de{'Ok'}", 507, true)]
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

        [ACMethodCommand("ForwardScheduling", "en{'Forward scheduling'}de{'Vorwärtsterminierung'}", 508, true)]
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

        [ACMethodCommand("SetBatchStateReadyToStart", "en{'Switch to Readiness'}de{'Startbereit setzen'}", (short)MISort.Start, true)]
        public void SetBatchStateReadyToStart()
        {
            if (!IsEnabledSetBatchStateReadyToStart())
                return;
            ProdOrderBatchPlan[] selectedBatches = ProdOrderBatchPlanList.Where(c => c.IsSelected
                                                                                    && (c.PlanState <= GlobalApp.BatchPlanState.Created
                                                                                        || c.PlanState >= GlobalApp.BatchPlanState.Paused))
                                                                         .ToArray();
            SetReadyToStart(selectedBatches);
        }

        public bool IsEnabledSetBatchStateReadyToStart()
        {
            return ProdOrderBatchPlanList != null
                && ProdOrderBatchPlanList.Any(c => c.IsSelected
                                                && (c.PlanState <= GlobalApp.BatchPlanState.Created
                                                    || c.PlanState >= GlobalApp.BatchPlanState.Paused));
        }


        private void SetReadyToStart(ProdOrderBatchPlan[] batchPlans)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();
            foreach (ProdOrderBatchPlan batchPlan in batchPlans)
            {
                bool isBatchReadyToStart = batchPlan.FacilityReservation_ProdOrderBatchPlan.Any();
                if (isBatchReadyToStart)
                    batchPlan.PlanState = vd.GlobalApp.BatchPlanState.ReadyToStart;
                else
                {
                    // Error50559
                    // Unable to start batch plan #{0} {1} {2} {3}x{4}! Destination not selected!
                    //  9   New 8401    NADJEV SIR ZA BUREK 1   0            
                    Msg msg = new Msg(this, eMsgLevel.Error, "BSOBatchPlanScheduler", "SetBatchStateReadyToStart()", 3184, "Error50559",
                        batchPlan.ScheduledOrder, batchPlan.ProdOrderPartslistPos.Material.MaterialNo, batchPlan.ProdOrderPartslistPos.Material.MaterialName1,
                        batchPlan.BatchTargetCount, batchPlan.BatchSize);
                    msgWithDetails.AddDetailMessage(msg);
                }
            }
            if (msgWithDetails.MsgDetails.Any())
                Messages.Msg(msgWithDetails);
            Save();
            OnPropertyChanged(nameof(ProdOrderBatchPlanList));
        }

        [ACMethodCommand("SetBatchStateCreated", "en{'Reset Readiness'}de{'Startbereitschaft rücksetzen'}", 508, true)]
        public void SetBatchStateCreated()
        {
            if (!IsEnabledSetBatchStateCreated()) return;
            List<ProdOrderBatchPlan> selectedBatches = ProdOrderBatchPlanList.Where(c => c.IsSelected).ToList();
            foreach (ProdOrderBatchPlan batchPlan in selectedBatches)
            {
                if (batchPlan.PlanState == GlobalApp.BatchPlanState.ReadyToStart
                    || batchPlan.PlanState == GlobalApp.BatchPlanState.AutoStart)
                {
                    batchPlan.PlanState = GlobalApp.BatchPlanState.Created;
                    if (batchPlan.PartialTargetCount.HasValue && batchPlan.PartialTargetCount.Value > 0)
                        batchPlan.PartialTargetCount = null;
                }
            }
            Save();
            OnPropertyChanged(nameof(ProdOrderBatchPlanList));
        }

        public bool IsEnabledSetBatchStateCreated()
        {
            return ProdOrderBatchPlanList != null && ProdOrderBatchPlanList.Any(c => c.IsSelected && (c.PlanState == GlobalApp.BatchPlanState.ReadyToStart || c.PlanState == GlobalApp.BatchPlanState.AutoStart));
        }

        public short[] NotAllowedStatesForBatchCancel
        {
            get
            {
                return new short[]
                {
                    (short)GlobalApp.BatchPlanState.ReadyToStart,
                    (short)GlobalApp.BatchPlanState.AutoStart,
                    (short)GlobalApp.BatchPlanState.InProcess
                };
            }
        }


        [ACMethodCommand("SetBatchStateCancelled", "en{'Deactivate and remove'}de{'Deaktivieren und Entfernen'}", (short)MISort.Start, true)]
        public void SetBatchStateCancelled()
        {
            ClearMessages();
            if (!IsEnabledSetBatchStateCancelled())
                return;
            try
            {
                List<Guid> groupsForRefresh = new List<Guid>();
                List<ProdOrderBatchPlan> selectedBatches = ProdOrderBatchPlanList.Where(c => c.IsSelected).ToList();
                if (Messages.Question(this, "Question50084", Global.MsgResult.Yes) == Global.MsgResult.Yes)
                {
                    List<ProdOrderBatchPlan> notSelected =
                    ProdOrderBatchPlanList
                    .Where(c => !c.IsSelected)
                    .OrderBy(c => c.ScheduledOrder ?? 0)
                    .ThenBy(c => c.InsertDate)
                    .ToList();
                    foreach (ProdOrderBatchPlan batchPlan in selectedBatches)
                    {
                        batchPlan.AutoRefresh();
                        batchPlan.FacilityReservation_ProdOrderBatchPlan.AutoRefresh();
                    }
                    bool autoDeleteDependingBatchPlans = AutoRemoveMDSGroupFrom > 0
                                                        && AutoRemoveMDSGroupTo > 0
                                                        && SelectedScheduleForPWNode != null
                                                        && SelectedScheduleForPWNode.MDSchedulingGroup != null
                                                        && SelectedScheduleForPWNode.MDSchedulingGroup.MDSchedulingGroupIndex >= AutoRemoveMDSGroupFrom
                                                        && SelectedScheduleForPWNode.MDSchedulingGroup.MDSchedulingGroupIndex <= AutoRemoveMDSGroupTo;

                    List<MaintainOrderInfo> prodOrders = new List<MaintainOrderInfo>();
                    //partslists.Select(c => new MaintainOrderInfo() { PO = c.ProdOrder }).Distinct().ToList();
                    foreach (ProdOrderBatchPlan batchPlan in selectedBatches)
                    {
                        if (NotAllowedStatesForBatchCancel.Contains(batchPlan.PlanStateIndex))
                            return;

                        var parentPOList = batchPlan.ProdOrderPartslist;
                        MaintainOrderInfo mOrderInfo = prodOrders.Where(c => c.PO == parentPOList.ProdOrder).FirstOrDefault();
                        if (mOrderInfo == null)
                        {
                            mOrderInfo = new MaintainOrderInfo();
                            mOrderInfo.PO = parentPOList.ProdOrder;
                            prodOrders.Add(mOrderInfo);
                        }

                        if (batchPlan.PlanState >= vd.GlobalApp.BatchPlanState.Paused
                            || batchPlan.ProdOrderBatch_ProdOrderBatchPlan.Any())
                        {
                            batchPlan.PlanState = GlobalApp.BatchPlanState.Cancelled;
                            if (autoDeleteDependingBatchPlans)
                                mOrderInfo.DeactivateAll = true;
                        }
                        else if (batchPlan.PlanState == GlobalApp.BatchPlanState.Created)
                        {
                            foreach (var reservation in batchPlan.FacilityReservation_ProdOrderBatchPlan.ToArray())
                            {
                                batchPlan.FacilityReservation_ProdOrderBatchPlan.Remove(reservation);
                                reservation.DeleteACObject(this.DatabaseApp, true);
                            }
                            Guid mdSchedulingGroupID = batchPlan.VBiACClassWF.MDSchedulingGroupWF_VBiACClassWF.Select(x => x.MDSchedulingGroupID).FirstOrDefault();
                            if (!groupsForRefresh.Contains(mdSchedulingGroupID))
                                groupsForRefresh.Add(mdSchedulingGroupID);

                            batchPlan.DeleteACObject(this.DatabaseApp, true);
                            parentPOList.ProdOrderBatchPlan_ProdOrderPartslist.Remove(batchPlan);
                            if (autoDeleteDependingBatchPlans && !parentPOList.ProdOrderBatchPlan_ProdOrderPartslist.Any())
                                mOrderInfo.RemoveAll = true;
                        }
                        else
                        {
                            if (autoDeleteDependingBatchPlans)
                                mOrderInfo.DeactivateAll = true;
                        }
                    }

                    MDProdOrderState mDProdOrderStateCompleted = DatabaseApp.s_cQry_GetMDProdOrderState(DatabaseApp, MDProdOrderState.ProdOrderStates.ProdFinished).FirstOrDefault();
                    MDProdOrderState mDProdOrderStateCancelled = DatabaseApp.s_cQry_GetMDProdOrderState(DatabaseApp, MDProdOrderState.ProdOrderStates.Cancelled).FirstOrDefault();

                    MaintainProdOrderState(prodOrders, mDProdOrderStateCancelled, mDProdOrderStateCompleted);

                    MoveBatchSortOrderCorrect(notSelected);

                    MsgWithDetails saveMsg = saveMsg = DatabaseApp.ACSaveChanges();
                    if (saveMsg != null)
                        SendMessage(saveMsg);

                    LoadProdOrderBatchPlanList();
                    OnPropertyChanged(nameof(ProdOrderBatchPlanList));
                    OnPropertyChanged(nameof(ProdOrderPartslistList));
                    if (!IsBSOTemplateScheduleParent)
                    {
                        if (groupsForRefresh.Any())
                        {
                            foreach (var mdSchedulingGroupID in groupsForRefresh)
                            {
                                RefreshServerState(mdSchedulingGroupID);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = "";
                Exception tmpEx = ex;
                while (tmpEx != null)
                {
                    msg += tmpEx.Message;
                    tmpEx = tmpEx.InnerException;
                }

                Msg errMsg = new Msg() { MessageLevel = eMsgLevel.Error, ACIdentifier = nameof(SetBatchStateCancelled), Message = msg };
                SendMessage(errMsg);
            }
        }

        public bool IsEnabledSetBatchStateCancelled()
        {
            return
                ProdOrderBatchPlanList != null
                && ProdOrderBatchPlanList.Any(c => c.IsSelected)
                && !ProdOrderBatchPlanList.Where(c => c.IsSelected).Any(c =>
                      NotAllowedStatesForBatchCancel.Contains(c.PlanStateIndex)
                );
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


        [ACMethodInteraction("NavigateToProdOrder2", "en{'Show Order'}de{'Auftrag anzeigen'}", 502, false, "SelectedProdOrderPartslist")]
        public void NavigateToProdOrder2()
        {
            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(
                new PAOrderInfoEntry()
                {
                    EntityID = SelectedProdOrderPartslist.ProdOrderPartslist.ProdOrderPartslistID,
                    EntityName = ProdOrderPartslist.ClassName
                });
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToProdOrder2()
        {
            return SelectedProdOrderPartslist != null && SelectedProdOrderPartslist.ProdOrderPartslist != null;
        }


        #endregion

        #region Methods -> (Tab)ProdOrder -> Manipulate Batch Plan

        [ACMethodCommand("New", "en{'Add Batchplan'}de{'Batchplan Hinzufügen'}", 504)]
        public void AddBatchPlan()
        {
            if (!IsEnabledAddBatchPlan())
                return;

            try
            {
                ProdOrderPartslist prodOrderPartslist = SelectedProdOrderPartslist.ProdOrderPartslist;
                if (prodOrderPartslist.ProdOrder.ProdOrderPartslist_ProdOrder.Count() > 1)
                {
                    Guid[] connections =
                        prodOrderPartslist
                        .ProdOrder
                        .ProdOrderPartslist_ProdOrder
                        .SelectMany(c => c.ProdOrderPartslistPos_ProdOrderPartslist)
                        .Select(c => c.SourceProdOrderPartslistID)
                        .Where(c => c != null)
                        .Select(c => c ?? Guid.Empty)
                        .ToArray();

                    prodOrderPartslist =
                        prodOrderPartslist
                        .ProdOrder
                        .ProdOrderPartslist_ProdOrder
                        .Where(c => !connections.Contains(c.ProdOrderPartslistID))
                        .OrderByDescending(c => c.Sequence)
                        .FirstOrDefault();
                }

                if (prodOrderPartslist != null)
                {
                    var schedulingGroups = PartslistMDSchedulerGroupConnections.FirstOrDefault(c => c.PartslistID == prodOrderPartslist.Partslist.PartslistID).SchedulingGroups;
                    MDSchedulingGroup schedulingGroup = schedulingGroups.FirstOrDefault(c => c.MDSchedulingGroupID == SelectedScheduleForPWNode.MDSchedulingGroupID);
                    if (schedulingGroup == null)
                        schedulingGroup = schedulingGroups.FirstOrDefault();

                    WizardDefineDefaultPartslist(schedulingGroup, prodOrderPartslist.Partslist, prodOrderPartslist.TargetQuantity, prodOrderPartslist);


                    //double targetQuantity = SelectedProdOrderPartslist.UnPlannedQuantityUOM;
                    //if (targetQuantity < double.Epsilon)
                    //    targetQuantity = SelectedProdOrderPartslist.TargetQuantityUOM;

                    IsWizard = true;
                    bool isForExpand =
                        prodOrderPartslist.ProdOrder.ProdOrderPartslist_ProdOrder.Count() == 1
                        && !prodOrderPartslist.ProdOrderBatch_ProdOrderPartslist.Any();
                    WizardPhase = isForExpand ? NewScheduledBatchWizardPhaseEnum.BOMExplosion : NewScheduledBatchWizardPhaseEnum.PartslistForDefinition;
                    bool success = WizardForwardAction(WizardPhase);

                }
            }
            catch (Exception ex)
            {
                IsWizard = false;
                throw ex;
            }
            OnPropertyChanged(nameof(CurrentLayout));
        }

        public bool IsEnabledAddBatchPlan()
        {
            return IsEnabledForBatchPlan(SelectedProdOrderPartslist);
        }

        private bool IsEnabledForBatchPlan(ProdOrderPartslistPlanWrapper plWrapper)
        {
            return
                 !IsWizard
                && SelectedScheduleForPWNode != null
                && plWrapper != null
                && plWrapper.ProdOrderPartslist != null
                && plWrapper.ProdOrderPartslist.MDProdOrderState != null
                && plWrapper.ProdOrderPartslist.MDProdOrderState.ProdOrderState < MDProdOrderState.ProdOrderStates.ProdFinished
                && plWrapper.ProdOrderPartslist.ProdOrder != null
                && plWrapper.ProdOrderPartslist.ProdOrder.MDProdOrderState != null
                && plWrapper.ProdOrderPartslist.ProdOrder.MDProdOrderState.ProdOrderState < MDProdOrderState.ProdOrderStates.ProdFinished;
            // && plWrapper.UnPlannedQuantityUOM > Double.Epsilon;
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

            bool autoDeleteDependingBatchPlans = AutoRemoveMDSGroupFrom > 0
                                                && AutoRemoveMDSGroupTo > 0
                                                && SelectedScheduleForPWNode != null
                                                && SelectedScheduleForPWNode.MDSchedulingGroup != null
                                                && SelectedScheduleForPWNode.MDSchedulingGroup.MDSchedulingGroupIndex >= AutoRemoveMDSGroupFrom
                                                && SelectedScheduleForPWNode.MDSchedulingGroup.MDSchedulingGroupIndex <= AutoRemoveMDSGroupTo;

            List<MaintainOrderInfo> prodOrders = new List<MaintainOrderInfo>();
            //= plForRemove.Select(c => new MaintainOrderInfo() { PO = c.ProdOrder }).Distinct().ToList();

            foreach (ProdOrderPartslist partslist in plForRemove)
            {
                MaintainOrderInfo mOrderInfo = prodOrders.Where(c => c.PO == partslist.ProdOrder).FirstOrDefault();
                if (mOrderInfo == null)
                {
                    mOrderInfo = new MaintainOrderInfo();
                    mOrderInfo.PO = partslist.ProdOrder;
                    mOrderInfo.RemoveAll = autoDeleteDependingBatchPlans;
                    prodOrders.Add(mOrderInfo);
                }
                partslist.AutoRefresh();
                bool hasAnyActivePlans = partslist.ProdOrderBatchPlan_ProdOrderPartslist.Where(c => c.PlanStateIndex > (short)GlobalApp.BatchPlanState.Created).Any();
                if (autoDeleteDependingBatchPlans)
                {
                    if (hasAnyActivePlans)
                        mOrderInfo.DeactivateAll = true;
                    else
                        mOrderInfo.RemoveAll = true;
                }

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
                    RemovePartslist(partslist, mDProdOrderStateCancelled, mDProdOrderStateCompleted);
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
                {
                    foreach (var mdSchedulingGroupID in groupsForRefresh)
                    {
                        RefreshServerState(mdSchedulingGroupID);
                    }
                }
            }
        }

        public class MaintainOrderInfo
        {
            public ProdOrder PO { get; set; }
            public bool RemoveAll { get; set; }
            public bool DeactivateAll { get; set; }
        }

        protected void MaintainProdOrderState(List<MaintainOrderInfo> prodOrders, MDProdOrderState mDProdOrderStateCancelled, MDProdOrderState mDProdOrderStateCompleted)
        {
            foreach (MaintainOrderInfo poInfo in prodOrders)
            {
                poInfo.PO.AutoRefresh();
                ProdOrderPartslist[] allProdPartslists = poInfo.PO.ProdOrderPartslist_ProdOrder.ToArray();
                bool canRemoveAll = true;
                if (poInfo.RemoveAll || poInfo.DeactivateAll)
                {
                    foreach (var batchPlan2Remove in poInfo.PO.ProdOrderPartslist_ProdOrder.SelectMany(c => c.ProdOrderBatchPlan_ProdOrderPartslist).ToArray())
                    {
                        var parentPOList = batchPlan2Remove.ProdOrderPartslist;
                        if (batchPlan2Remove.PlanState == GlobalApp.BatchPlanState.Created)
                        {
                            foreach (var reservation in batchPlan2Remove.FacilityReservation_ProdOrderBatchPlan.ToArray())
                            {
                                reservation.DeleteACObject(this.DatabaseApp, true);
                                batchPlan2Remove.FacilityReservation_ProdOrderBatchPlan.Remove(reservation);
                            }
                            batchPlan2Remove.DeleteACObject(this.DatabaseApp, true);
                            parentPOList.ProdOrderBatchPlan_ProdOrderPartslist.Remove(batchPlan2Remove);
                        }
                        else
                        {
                            if (poInfo.DeactivateAll)
                            {
                                if (batchPlan2Remove.PlanState < GlobalApp.BatchPlanState.ReadyToStart || batchPlan2Remove.PlanState > GlobalApp.BatchPlanState.InProcess)
                                    batchPlan2Remove.PlanState = GlobalApp.BatchPlanState.Cancelled;
                            }
                            canRemoveAll = false;
                        }
                    }
                }
                else
                    canRemoveAll = !poInfo.PO.ProdOrderPartslist_ProdOrder.SelectMany(c => c.ProdOrderBatchPlan_ProdOrderPartslist).Any();
                if (canRemoveAll && string.IsNullOrEmpty(poInfo.PO.KeyOfExtSys))
                {
                    foreach (ProdOrderPartslist pl in allProdPartslists)
                    {
                        RemovePartslist(pl, mDProdOrderStateCancelled, mDProdOrderStateCompleted);
                    }
                }
                else if (!IsBSOTemplateScheduleParent || poInfo.DeactivateAll)
                {
                    if ((poInfo.PO.ProdOrderPartslist_ProdOrder.SelectMany(c => c.ProdOrderBatchPlan_ProdOrderPartslist).Any()
                            && !poInfo.PO.ProdOrderPartslist_ProdOrder.SelectMany(c => c.ProdOrderBatchPlan_ProdOrderPartslist).Any(c => c.PlanStateIndex < (short)GlobalApp.BatchPlanState.Completed))
                        || poInfo.DeactivateAll)
                    {
                        foreach (ProdOrderPartslist pl in allProdPartslists)
                            SetProdorderPartslistState(mDProdOrderStateCancelled, mDProdOrderStateCompleted, pl);
                    }
                    if (!poInfo.PO.ProdOrderPartslist_ProdOrder.Any(c => c.MDProdOrderState.MDProdOrderStateIndex < (short)(short)GlobalApp.BatchPlanState.Completed))
                    {
                        poInfo.PO.MDProdOrderState = mDProdOrderStateCompleted;
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
                    !(SelectedWizardSchedulerPartslist != null && SelectedWizardSchedulerPartslist.ProdOrderPartslistPos != null)
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
                    OnPropertyChanged(nameof(WizardSchedulerPartslistList));
                    if (WizardSchedulerPartslistList != null)
                        SelectedWizardSchedulerPartslist = WizardSchedulerPartslistList.FirstOrDefault();
                }
                else
                {
                    rootPartslistExpand = null;
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
                    && wizardSchedulerPartslist.WFNodeMES != null
                    && (
                            !wizardSchedulerPartslist.IsSolved
                            ||
                                (
                                    (SelectedWizardSchedulerPartslist != null && SelectedWizardSchedulerPartslist.ProdOrderPartslistPos != null)
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
            if (batchPlan == null)
            {
                Messages.LogError(this.GetACUrl(), "ChangeBatchPlan(10)", "batchPlan is null");
                return;
            }
            ChangeBatchPlanForSchedule(batchPlan, SelectedScheduleForPWNode);
        }

        protected void ChangeBatchPlanForSchedule(ProdOrderBatchPlan batchPlan, PAScheduleForPWNode forSchedule)
        {
            bool notValidBatchForChange =
                       batchPlan == null
                    || forSchedule == null
                    || IsWizard
                    //|| batchPlan.ProdOrderBatch_ProdOrderBatchPlan.Any()
                    || batchPlan.ProdOrderPartslist.MDProdOrderState.MDProdOrderStateIndex >= (short)MDProdOrderState.ProdOrderStates.ProdFinished
                    || batchPlan.ProdOrderPartslist.ProdOrder.MDProdOrderState.MDProdOrderStateIndex >= (short)MDProdOrderState.ProdOrderStates.ProdFinished;
            if (notValidBatchForChange)
                return;
            ClearMessages();
            try
            {
                ClearMessages();
                WizardDefineDefaultPartslist(forSchedule.MDSchedulingGroup, batchPlan.ProdOrderPartslist.Partslist, batchPlan.ProdOrderPartslist.TargetQuantity, batchPlan.ProdOrderPartslist);
                WizardPhase = NewScheduledBatchWizardPhaseEnum.PartslistForDefinition;
                IsWizard = true;
                WizardForwardAction(WizardPhase);

                vd.ACClassWF vbACClassWF = batchPlan.VBiACClassWF;
                if (vbACClassWF == null)
                    vbACClassWF = forSchedule.MDSchedulingGroup.MDSchedulingGroupWF_MDSchedulingGroup.Select(c => c.VBiACClassWF).FirstOrDefault();
                SetBSOBatchPlan_BatchParents(vbACClassWF, batchPlan.ProdOrderPartslist);
                LoadGeneratedBatchInCurrentLine(batchPlan, batchPlan.ProdOrderPartslist.TargetQuantity);
            }
            catch (Exception ex)
            {
                IsWizard = false;
                Exception tmp = ex;
                string errorMessage = "";
                while (tmp != null)
                {
                    errorMessage += tmp.Message;
                    tmp = tmp.InnerException;
                }
                Msg msg = new Msg(eMsgLevel.Error, errorMessage);
                SendMessage(msg);
            }

            OnPropertyChanged(nameof(CurrentLayout));
        }

        /// <summary>
        /// Source Property: GenerateBatchPlans
        /// </summary>
        [ACMethodInfo("GenerateBatchPlans", "en{'Generate batch plans'}de{'Batchplan generieren'}", 999, true)]
        public void GenerateBatchPlans()
        {
            if (!IsEnabledGenerateBatchPlans())
                return;
            if (Messages.Question(this, "Question50086", Global.MsgResult.No) == Global.MsgResult.Yes)
            {
                BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoGenerateBatchPlans);
                ShowDialog(this, DesignNameProgressBar);
            }
        }

        public bool IsEnabledGenerateBatchPlans()
        {
            return
                ProdOrderPartslistList != null
                && ProdOrderPartslistList.Any(c => c.IsSelected)
                && ProdOrderPartslistList.Where(c => c.IsSelected && IsEnabledForBatchPlan(c)).Any();
        }

        /// <summary>
        /// Source Property: GenerateBatchPlans
        /// </summary>
        [ACMethodInfo("MergeOrders", "en{'Merge prodorders'}de{'Auftrag zusammenführen'}", 999, true)]
        public void MergeOrders()
        {
            if (!IsEnabledMergeOrders())
                return;
            if (Messages.Question(this, "Question50086", Global.MsgResult.No) == Global.MsgResult.Yes)
            {
                BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoMergeOrders);
                ShowDialog(this, DesignNameProgressBar);
            }
        }

        public bool IsEnabledMergeOrders()
        {
            return
                ProdOrderPartslistList != null
                && ProdOrderPartslistList.Any(c => c.IsSelected)
                && ProdOrderPartslistList.Where(c => c.IsSelected && IsEnabledForBatchPlan(c)).Any();
        }

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
                    isEnabled = (!SelectedWizardSchedulerPartslist.IsSolved || SelectedWizardSchedulerPartslist.ProdOrderPartslistPos != null) && SelectedWizardSchedulerPartslist.SelectedMDSchedulingGroup != null;
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
                        isEnabled =
                            BSOBatchPlanChild.Value.SelectedBatchPlanForIntermediate != null
                            && SelectedWizardSchedulerPartslist != null
                            && SelectedWizardSchedulerPartslist.NewTargetQuantityUOM > Double.Epsilon
                            && IsThereBatchWithoutTarget();
                    break;
            }
            return isEnabled;
        }

        public bool IsThereBatchWithoutTarget()
        {
            return SelectedWizardSchedulerPartslist != null
                && SelectedWizardSchedulerPartslist.ProdOrderPartslistPos != null
                && !SelectedWizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderBatchPlan_ProdOrderPartslistPos.Any(c => !c.FacilityReservation_ProdOrderBatchPlan.Any());
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
                        BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value.FilterIsNotDeleted = true;
                        BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value.FilterIsConnectedWithEnabledPartslist = true;
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
                        if (selectedPartslist.MDUnitID.HasValue && selectedPartslist.MDUnitID != selectedPartslist.Material.BaseMDUnitID)
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
                        OnPropertyChanged(nameof(PartListExpandList));
                        success = true;
                        WizardSolvedTasks.Add(NewScheduledBatchWizardPhaseEnum.BOMExplosion);
                        break;
                    case NewScheduledBatchWizardPhaseEnum.PartslistForDefinition:
                        if (WizardSolvedTasks.Contains(NewScheduledBatchWizardPhaseEnum.PartslistForDefinition))
                            return true;
                        if (DefaultWizardSchedulerPartslist.ProgramNo != null)
                            LoadExistingWizardSchedulerPartslistList(DefaultWizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder);
                        if (rootPartslistExpand != null)
                            LoadBOMExplosionItems(DefaultWizardSchedulerPartslist.ProdOrderPartslistPos?.ProdOrderPartslist.ProdOrder);

                        foreach (var item in AllWizardSchedulerPartslistList)
                        {
                            if (item.SelectedMDSchedulingGroup != null)
                                item.LoadConfiguration();
                        }
                        success = SelectedWizardSchedulerPartslist != null;
                        var tmp = _SelectedWizardSchedulerPartslist;
                        OnPropertyChanged(nameof(WizardSchedulerPartslistList));
                        _SelectedWizardSchedulerPartslist = tmp;
                        WizardSolvedTasks.Add(NewScheduledBatchWizardPhaseEnum.PartslistForDefinition);
                        break;
                    case NewScheduledBatchWizardPhaseEnum.DefineBatch:
                        SelectedWizardSchedulerPartslist.LoadBatchSuggestion();
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
                        OnPropertyChanged(nameof(SelectedFilterBatchplanSuggestionMode));
                        break;
                    case NewScheduledBatchWizardPhaseEnum.DefineTargets:
                        success = SelectedWizardSchedulerPartslist != null && SelectedWizardSchedulerPartslist.NewTargetQuantityUOM > Double.Epsilon && SelectedWizardSchedulerPartslist.BatchPlanSuggestion != null;
                        if (success)
                        {
                            // Update prod PL TargetQuantityUOM if is changed in Define batch wizard phase
                            if (SelectedWizardSchedulerPartslist.ProdOrderPartslistPos != null
                                && Math.Abs(SelectedWizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderPartslist.TargetQuantity - SelectedWizardSchedulerPartslist.TargetQuantityUOM) > 0.1)
                            {
                                Msg changeMsg = ProdOrderManager.ProdOrderPartslistChangeTargetQuantity(DatabaseApp, SelectedWizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderPartslist, SelectedWizardSchedulerPartslist.TargetQuantityUOM);
                                if (changeMsg != null)
                                    SendMessage(changeMsg);
                            }

                            string programNo = SelectedWizardSchedulerPartslist.ProgramNo;

                            if (SelectedWizardSchedulerPartslist.ProdOrderPartslistPos != null)
                                success = UpdateBatchPlans(SelectedWizardSchedulerPartslist);
                            else
                            {
                                List<vd.ProdOrderBatchPlan> generatedBatchPlans;
                                success = FactoryBatchPlans(SelectedWizardSchedulerPartslist, ref programNo, out generatedBatchPlans);
                            }
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

        #region Methods -> Wizard -> Helper 

        private void ClearAndResetChildBSOLists(MDSchedulingGroup mdSchedulingGroup)
        {
            BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value.AccessPrimary.NavList.Clear();
            BSOPartslistExplorer_Child.Value.AccessPrimary.NavList.Clear();
            BSOPartslistExplorer_Child.Value.FilterMDSchedulingGroup = mdSchedulingGroup;
            BSOPartslistExplorer_Child.Value.FilterIsEnabled = true;
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
            ConnectSourceProdOrderPartslist();
            bool success = ACSaveChanges();
            if (success)
            {
                IsWizard = false;
                OnPropertyChanged(nameof(CurrentLayout));
                OnPostSave();
                Search();
            }
            return success;
        }

        private void ConnectSourceProdOrderPartslist()
        {
            ProdOrder prodorder = DefaultWizardSchedulerPartslist?.ProdOrderPartslistPos?.ProdOrderPartslist?.ProdOrder;
            if (prodorder != null)
            {
                ProdOrderManager.ConnectSourceProdOrderPartslist(prodorder);
                ProdOrderManager.CorrectSortOrder(prodorder);
            }
        }

        private void LoadBOMExplosionItems(ProdOrder prodOrder)
        {
            List<ExpandResult> treeResult = rootPartslistExpand.BuildTreeList();
            treeResult =
                treeResult
                .Where(x =>
                    x.Item.IsChecked
                    && x.Item.IsEnabled)
                .OrderByDescending(x => x.TreeVersion)
                .ToList();

            int sn = 0;

            foreach (ExpandResult expand in treeResult)
            {
                WizardSchedulerPartslist wizardSchedulerPartslist =
                    AllWizardSchedulerPartslistList
                    .Where(c =>
                                c.ProdOrderPartslistPos != null
                                && c.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo == expand.Item.PartslistNo)
                    .FirstOrDefault();

                sn++;

                if (wizardSchedulerPartslist == null)
                {
                    ProdOrderPartslist prodOrderPartslist = null;
                    if (prodOrder != null)
                        prodOrderPartslist = prodOrder.ProdOrderPartslist_ProdOrder.FirstOrDefault(c => c.Partslist.PartslistNo == expand.Item.PartslistNo);

                    List<MDSchedulingGroup> schedulingGroups = ProdOrderManager.GetSchedulingGroups(DatabaseApp, PWNodeProcessWorkflowVB.PWClassName, expand.Item.PartslistForPosition, PartslistMDSchedulerGroupConnections);
                    if (prodOrderPartslist != null)
                        wizardSchedulerPartslist = new WizardSchedulerPartslist(
                        DatabaseApp, ProdOrderManager, LocalBSOBatchPlan.VarioConfigManager,
                        expand.Item.PartslistForPosition, expand.Item.TargetQuantityUOM, sn, schedulingGroups, prodOrderPartslist);
                    else
                        wizardSchedulerPartslist = new WizardSchedulerPartslist(
                            DatabaseApp, ProdOrderManager, LocalBSOBatchPlan.VarioConfigManager,
                            expand.Item.PartslistForPosition, expand.Item.TargetQuantityUOM, sn, schedulingGroups);

                    AddWizardSchedulerPartslistList(wizardSchedulerPartslist, sn);
                }

                wizardSchedulerPartslist.Sn = sn;
                if (prodOrder != null && string.IsNullOrEmpty(wizardSchedulerPartslist.ProgramNo))
                    wizardSchedulerPartslist.ProgramNo = prodOrder.ProgramNo;
            }
            if (AllWizardSchedulerPartslistList.Any())
            {
                DefaultWizardSchedulerPartslist.Sn = AllWizardSchedulerPartslistList.Count();
            }
        }

        private void LoadExistingWizardSchedulerPartslistList(ProdOrder prodOrder)
        {
            AllWizardSchedulerPartslistList = new List<WizardSchedulerPartslist>();
            List<ProdOrderPartslist> partslists = prodOrder.ProdOrderPartslist_ProdOrder.OrderBy(c => c.Sequence).ToList();
            foreach (ProdOrderPartslist prodOrderPartslist in partslists)
            {
                bool isThere = AllWizardSchedulerPartslistList.Where(c => c.ProdOrderPartslistPos != null && c.ProdOrderPartslistPos.ProdOrderPartslistID == prodOrderPartslist.ProdOrderPartslistID).Any();
                if (!isThere)
                {
                    List<MDSchedulingGroup> schedulingGroups = ProdOrderManager.GetSchedulingGroups(DatabaseApp, PWNodeProcessWorkflowVB.PWClassName, prodOrderPartslist.Partslist, PartslistMDSchedulerGroupConnections);
                    WizardSchedulerPartslist item = new WizardSchedulerPartslist(
                        DatabaseApp, ProdOrderManager, LocalBSOBatchPlan.VarioConfigManager,
                        prodOrderPartslist.Partslist, prodOrderPartslist.TargetQuantity, prodOrderPartslist.Sequence, schedulingGroups,
                        prodOrderPartslist);

                    AddWizardSchedulerPartslistList(item);
                    if (SelectedProdOrderBatchPlan != null && SelectedProdOrderBatchPlan.ProdOrderPartslistID == prodOrderPartslist.ProdOrderPartslistID)
                        SelectedWizardSchedulerPartslist = item;
                }
            }
        }

        private Msg CheckProductionUnits()
        {
            Msg msg = null;
            if (SelectedWizardSchedulerPartslist.ProductionUnitsUOM.HasValue && SelectedWizardSchedulerPartslist.ProductionUnitsUOM.Value > 0)
            {
                double rest = SelectedWizardSchedulerPartslist.TargetQuantityUOM % SelectedWizardSchedulerPartslist.ProductionUnitsUOM.Value;
                if (rest > 0)
                {
                    msg = new Msg(this, eMsgLevel.Error, GetACUrl(), "CheckProductionUnits", 3119, "Error50440", SelectedWizardSchedulerPartslist.TargetQuantityUOM, SelectedWizardSchedulerPartslist.ProductionUnitsUOM);
                }
            }
            return msg;
        }


        public void WizardDefineDefaultPartslist(MDSchedulingGroup schedulingGroup, Partslist partslist, double targetQuantity, ProdOrderPartslist prodOrderPartslist = null)
        {
            List<MDSchedulingGroup> schedulingGroups = ProdOrderManager.GetSchedulingGroups(DatabaseApp, PWNodeProcessWorkflowVB.PWClassName, partslist, PartslistMDSchedulerGroupConnections);
            if (prodOrderPartslist != null)
                DefaultWizardSchedulerPartslist = new WizardSchedulerPartslist(
                    DatabaseApp, ProdOrderManager, LocalBSOBatchPlan.VarioConfigManager,
                    partslist, targetQuantity, 1, schedulingGroups, prodOrderPartslist);
            else
                DefaultWizardSchedulerPartslist = new WizardSchedulerPartslist(
                   DatabaseApp, ProdOrderManager, LocalBSOBatchPlan.VarioConfigManager,
                   partslist, targetQuantity, 1, schedulingGroups);
            DefaultWizardSchedulerPartslist.SelectedMDSchedulingGroup = schedulingGroup;
            AllWizardSchedulerPartslistList.Clear();
            AddWizardSchedulerPartslistList(DefaultWizardSchedulerPartslist);
            SelectedWizardSchedulerPartslist = DefaultWizardSchedulerPartslist;
            if (SelectedWizardSchedulerPartslist != null)
            {
                SelectedWizardSchedulerPartslist.LoadConfiguration();
            }
        }

        [ACMethodInfo("Wizard", "en{'Close'}de{'Schließen'}", 511)]
        public void WizardCancel()
        {
            IsWizard = false;
            ACUndoChanges();

            ConnectSourceProdOrderPartslist();
            ACSaveChanges();

            WizardClean();
            OnPropertyChanged(nameof(CurrentLayout));
            LoadProdOrderBatchPlanList();
            OnPropertyChanged(nameof(SelectedProdOrderBatchPlan));
        }

        public void WizardClean()
        {
            BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value.SearchWord = "";
            BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value.SelectedMaterial = null;
            BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value.MaterialList.Clear();

            BSOPartslistExplorer_Child.Value.SearchWord = "";
            BSOPartslistExplorer_Child.Value.SelectedPartslist = null;
            BSOPartslistExplorer_Child.Value.FilterIsEnabled = true;
            BSOPartslistExplorer_Child.Value.AccessPrimary.NavList.Clear();

            DefaultWizardSchedulerPartslist = null;
            SelectedWizardSchedulerPartslist = null;
            AllWizardSchedulerPartslistList.Clear();


            LocalBSOBatchPlan.BatchPlanForIntermediateList.Clear();
            LocalBSOBatchPlan.SelectedBatchPlanForIntermediate = null;

            _WizardPhase = NewScheduledBatchWizardPhaseEnum.SelectMaterial;

            WizardSolvedTasks.Clear();
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
                    FilterPlanningMR?.PlanningMRID,
                    SelectedFilterBatchPlanGroup?.MDBatchPlanGroupID,
                    FilterBatchProgramNo,
                    FilterBatchMaterialNo);

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
                            && SelectedScheduleForPWNode.StartMode == BatchPlanStartModeEnum.SemiAutomatic
                            && SelectedProdOrderBatchPlan != null
                            && SelectedProdOrderBatchPlan.PlanMode == BatchPlanMode.UseBatchCount
                            && SelectedProdOrderBatchPlan.BatchTargetCount > SelectedProdOrderBatchPlan.BatchActualCount)
                            result = Global.ControlModes.Enabled;
                        else
                            result = Global.ControlModes.Disabled;
                        break;
                    case "SelectedProdOrderBatchPlan\\ScheduledStartDate":
                    case "SelectedProdOrderBatchPlan\\ScheduledEndDate":
                        if (SelectedProdOrderBatchPlan != null
                            && (SelectedProdOrderBatchPlan.PlanState <= GlobalApp.BatchPlanState.ReadyToStart || SelectedProdOrderBatchPlan.PlanState == GlobalApp.BatchPlanState.Paused)
                            && (SelectedProdOrderBatchPlan.PlanMode != BatchPlanMode.UseBatchCount
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
            try
            {
                ProdOrderBatchPlanList = GetProdOrderBatchPlanList(SelectedScheduleForPWNode?.MDSchedulingGroupID);
                ProdOrderPartslistList = GetProdOrderPartslistList();
            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), nameof(LoadProdOrderBatchPlanList), e);
                Messages.LogException(this.GetACUrl(), nameof(LoadProdOrderBatchPlanList), e.StackTrace);
            }
        }

        private void LoadScheduleListForPWNodes(string schedulingGroupMDKey, bool reloadBatchPlanList = true)
        {
            PAScheduleForPWNodeList newScheduleForWFNodeList = null;
            if (_SchedulesForPWNodesProp != null && _SchedulesForPWNodesProp.ValueT != null)
                newScheduleForWFNodeList = _SchedulesForPWNodesProp.ValueT;
            else
                newScheduleForWFNodeList = PABatchPlanScheduler.CreateScheduleListForPWNodes(this, DatabaseApp, null);
            //int removedCount = newScheduleForWFNodeList.RemoveAll(x => x.MDSchedulingGroupID == Guid.Empty);
            UpdateScheduleForPWNodeList(newScheduleForWFNodeList, reloadBatchPlanList);
            if (!string.IsNullOrEmpty(schedulingGroupMDKey))
                SelectedScheduleForPWNode = ScheduleForPWNodeList.FirstOrDefault(c => c.MDSchedulingGroup.MDKey == schedulingGroupMDKey);
        }

        private void UpdateScheduleForPWNodeList(PAScheduleForPWNodeList newScheduleForWFNodeList, bool reloadBatchPlanList = true)
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
                    OnPropertyChanged(nameof(SelectedScheduleForPWNode));
                }
                else
                {
                    var selected = SelectedScheduleForPWNode;
                    OnPropertyChanged(nameof(ScheduleForPWNodeList));
                    SelectedScheduleForPWNode = selected;
                    if (SelectedScheduleForPWNode != null)
                        SelectedFilterBatchPlanStartMode = FilterBatchPlanStartModeList.Where(c => (BatchPlanStartModeEnum)c.Value == _SelectedScheduleForPWNode.StartMode).FirstOrDefault();
                }
            }

            if ((diffResult.HasFlag(PAScheduleForPWNodeList.DiffResult.RefreshCounterChanged)
                    || diffResult.HasFlag(PAScheduleForPWNodeList.DiffResult.ValueChangesInList))
                && reloadBatchPlanList)
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

        private bool FactoryBatchPlans(WizardSchedulerPartslist wizardSchedulerPartslist, ref string programNo, out List<vd.ProdOrderBatchPlan> generatedBatchPlans)
        {
            bool success = ProdOrderManager.FactoryBatchPlans(DatabaseApp, FilterPlanningMR, CreatedBatchState, wizardSchedulerPartslist, DefaultWizardSchedulerPartslist, ref programNo, out generatedBatchPlans);
            if (success)
            {
                SetBSOBatchPlan_BatchParents(wizardSchedulerPartslist.WFNodeMES, wizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderPartslist);
                ProdOrderBatchPlan firstBatchPlan = wizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderBatchPlan_ProdOrderPartslistPos.FirstOrDefault();
                LoadGeneratedBatchInCurrentLine(firstBatchPlan, wizardSchedulerPartslist.NewTargetQuantityUOM);
            }
            return success;
        }

        private bool UpdateBatchPlans(WizardSchedulerPartslist wizardSchedulerPartslist)
        {
            List<ProdOrderBatchPlan> otherBatchPlans = GetProdOrderBatchPlanList(wizardSchedulerPartslist.SelectedMDSchedulingGroup.MDSchedulingGroupID).ToList();

            bool success = ProdOrderManager.UpdateBatchPlans(DatabaseApp, wizardSchedulerPartslist, otherBatchPlans);

            SetBSOBatchPlan_BatchParents(wizardSchedulerPartslist.WFNodeMES, wizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderPartslist);

            ProdOrderBatchPlan[] tmp = wizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrderBatchPlan_ProdOrderPartslist.OrderByDescending(c => c.ScheduledOrder).ToArray();
            foreach (ProdOrderBatchPlan bp in tmp)
                LoadGeneratedBatchInCurrentLine(bp, SelectedWizardSchedulerPartslist.NewTargetQuantityUOM);
            if (
                !AllWizardSchedulerPartslistList.Any(c => c.ProdOrderPartslistPos != null && c.PartslistNo != wizardSchedulerPartslist.PartslistNo)
                || AllWizardSchedulerPartslistList.Where(c => !c.IsSolved).Count() == 1)
                wizardSchedulerPartslist.IsSolved = success;
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
                LocalBSOBatchPlan.MandatoryConfigStores =
                    ProdOrderManager
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

            LocalBSOBatchPlan.OnPropertyChanged(nameof(BSOBatchPlan.IsVisibleInCurrentContext));
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

        private void RemovePartslist(ProdOrderPartslist partslist, MDProdOrderState mDProdOrderStateCancelled, MDProdOrderState mDProdOrderStateCompleted)
        {
            ProdOrder prodOrder = partslist.ProdOrder;
            if (!partslist.ProdOrderBatchPlan_ProdOrderPartslist.Any())
            {
                bool hasAnyPostings =
                                    DatabaseApp.FacilityBooking.Where(c => (c.ProdOrderPartslistPosID.HasValue && c.ProdOrderPartslistPos.ProdOrderPartslistID == partslist.ProdOrderPartslistID)
                                                || (c.ProdOrderPartslistPosRelationID.HasValue && c.ProdOrderPartslistPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslistPosID == partslist.ProdOrderPartslistID))
                                    .Any();

                bool hasOrderLog =
                        partslist.ProdOrderPartslistPos_ProdOrderPartslist.SelectMany(c => c.OrderLog_ProdOrderPartslistPos).Any()
                        || partslist.ProdOrderPartslistPos_ProdOrderPartslist.SelectMany(c => c.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos).SelectMany(c => c.OrderLog_ProdOrderPartslistPosRelation).Any();

                if (hasAnyPostings || hasOrderLog || !string.IsNullOrEmpty(partslist.ProdOrder.KeyOfExtSys))
                    partslist.MDProdOrderState = mDProdOrderStateCancelled;
                else
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
        }

        #endregion

        #endregion

        #region Methods => Configuration

        [ACMethodInteraction("", "en{'Configure scheduler rules'}de{'Konfigurieren Regeln für den Zeitplaner'}", 601, true)]
        public void ConfigureBSO()
        {
            if (_TempRules == null)
                _TempRules = GetStoredRules();
            AssignedUserRules = _TempRules.OrderBy(c => c.RuleParamCaption);

            if (AvailableSchedulingGroupsList == null)
                AvailableSchedulingGroupsList = DatabaseApp.MDSchedulingGroup.ToArray().OrderBy(c => c.ACCaption).ToList();

            ShowDialog(this, "ConfigurationDialog");
        }

        public bool IsEnabledConfigureBSO()
        {
            return true;//Root.Environment.User.IsSuperuser;
        }

        [ACMethodInfo("", "en{'Grant permission'}de{'Berechtigung erteilen'}", 602)]
        public void AddRule()
        {
            UserRuleItem existingRule = AssignedUserRules.FirstOrDefault(c => c.VBUserID == SelectedVBUser.VBUserID
                                                                           && c.RuleParamID == SelectedAvailableSchedulingGroup.MDSchedulingGroupID);

            if (existingRule != null)
                return;

            UserRuleItem rule = new UserRuleItem()
            {
                VBUserID = SelectedVBUser.VBUserID,
                VBUserName = SelectedVBUser.VBUserName,
                RuleParamID = SelectedAvailableSchedulingGroup.MDSchedulingGroupID,
                RuleParamCaption = SelectedAvailableSchedulingGroup.ACCaption
            };

            _TempRules.Add(rule);
            AssignedUserRules = _TempRules.OrderBy(c => c.RuleParamCaption).ToList();
        }

        public bool IsEnabledAddRule()
        {
            return SelectedVBUser != null && SelectedAvailableSchedulingGroup != null;
        }

        [ACMethodInfo("", "en{'Remove permission'}de{'Berechtigung entfernen'}", 603)]
        public void RemoveRule()
        {
            UserRuleItem rule = _TempRules.FirstOrDefault(c => c == SelectedAssignedUserRule);
            if (rule != null)
            {
                _TempRules.Remove(rule);
                AssignedUserRules = _TempRules.OrderBy(c => c.RuleParamCaption).ToList();
                SelectedAssignedUserRule = null;
            }
        }

        public bool IsEnabledRemoveRule()
        {
            return SelectedAssignedUserRule != null;
        }

        [ACMethodInfo("", "en{'Apply rules and close'}de{'Regeln anwenden und schließen'}", 604)]
        public void ApplyRulesAndClose()
        {
            string xml = "";
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            using (XmlTextWriter xmlWriter = new XmlTextWriter(sw))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(List<UserRuleItem>));
                serializer.WriteObject(xmlWriter, _TempRules);
                xml = sw.ToString();
            }
            BSOBatchPlanSchedulerRules = xml;
            var msg = DatabaseApp.ACSaveChanges();

            if (msg != null)
                Messages.Msg(msg);

            _TempRules = GetStoredRules();

            CloseTopDialog();
        }

        public bool IsEnabledApplyRulesAndClose()
        {
            return true;
        }

        private List<UserRuleItem> GetRulesForCurrentUser()
        {
            if (_TempRules == null)
                _TempRules = GetStoredRules();
            return _TempRules.Where(c => c.VBUserName == Root.Environment.User.VBUserName).ToList();
        }

        private List<UserRuleItem> GetStoredRules()
        {
            if (string.IsNullOrEmpty(BSOBatchPlanSchedulerRules))
                return new List<UserRuleItem>();

            try
            {
                using (StringReader ms = new StringReader(BSOBatchPlanSchedulerRules))
                using (XmlTextReader xmlReader = new XmlTextReader(ms))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(List<UserRuleItem>));
                    List<UserRuleItem> result = serializer.ReadObject(xmlReader) as List<UserRuleItem>;
                    if (result == null)
                        return new List<UserRuleItem>();

                    using (Database db = new core.datamodel.Database())
                    {
                        foreach (UserRuleItem item in result)
                        {
                            core.datamodel.VBUser vbUser = db.VBUser.FirstOrDefault(c => c.VBUserID == item.VBUserID);
                            if (vbUser == null)
                                continue;

                            item.VBUserName = vbUser.VBUserName;

                            MDSchedulingGroup group = DatabaseApp.MDSchedulingGroup.FirstOrDefault(c => c.MDSchedulingGroupID == item.RuleParamID);
                            if (group == null)
                                continue;

                            item.RuleParamCaption = group.ACCaption;
                        }
                    }

                    return result;
                }
            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), nameof(GetStoredRules), e);
                return new List<UserRuleItem>();
            }
        }

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
                case BGWorkerMehtod_DoGenerateBatchPlans:
                    List<ProdOrderPartslist> plForBatchGenerate = ProdOrderPartslistList.Where(c => c.IsSelected).Select(c => c.ProdOrderPartslist).ToList();
                    e.Result = ProdOrderManager.GenerateBatchPlans(DatabaseApp, LocalBSOBatchPlan.VarioConfigManager, LocalBSOBatchPlan.RoutingService, PWNodeProcessWorkflowVB.PWClassName, plForBatchGenerate);
                    break;
                case BGWorkerMehtod_DoMergeOrders:
                    List<ProdOrderPartslist> plForMerge = ProdOrderPartslistList.Where(c => c.IsSelected).Select(c => c.ProdOrderPartslist).ToList();
                    e.Result = ProdOrderManager.MergeOrders(DatabaseApp, plForMerge);
                    break;
                case BGWorkerMehtod_DoSearchStockMaterial:
                    List<PreparedMaterial> preparedMaterials = DoSearchStockMaterial();
                    e.Result = preparedMaterials;
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
            ClearMessages();
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
                if (command == BGWorkerMehtod_DoSearchStockMaterial)
                {
                    List<PreparedMaterial> preparedMaterials = e.Result as List<PreparedMaterial>;
                    DoSearchStockMaterialFinish(preparedMaterials);
                }
                else
                {
                    MsgWithDetails resultMsg = null;
                    if (e.Result != null)
                        resultMsg = (MsgWithDetails)e.Result;

                    if (resultMsg != null)
                    {
                        if (resultMsg is MsgWithDetails)
                        {
                            MsgWithDetails msgWithDetails = resultMsg as MsgWithDetails;
                            if (msgWithDetails.MsgDetails.Any())
                            {
                                foreach (Msg detailMsg in msgWithDetails.MsgDetails)
                                    SendMessage(detailMsg);

                                // Warning50049
                                // 
                                Msg msg = new Msg(this, eMsgLevel.Error, GetACUrl(), command + "()", 4489, "Warning50049");
                                Messages.Msg(msg);
                            }
                        }
                    }

                    if (resultMsg == null || resultMsg.IsSucceded())
                    {
                        switch (command)
                        {
                            case BGWorkerMehtod_DoGenerateBatchPlans:
                                LoadProdOrderBatchPlanList();
                                break;
                        }
                    }
                }
            }
        }

        #endregion

        #region BackgroundWorker -> DoMehtods


        #region BackgroundWorker -> DoMehtods -> SearchStockMaterial

        private List<PreparedMaterial> DoSearchStockMaterial()
        {
            List<SearchBatchMaterialModel> searchModel = new List<SearchBatchMaterialModel>();
            List<ProdOrderBatchPlan> selectedBatchPlans = new List<ProdOrderBatchPlan>();
            if (ScheduleForPWNodeList != null
                && ScheduleForPWNodeList.Where(c => c.IsSelected).Any())
            {
                PAScheduleForPWNode[] selectedLines = ScheduleForPWNodeList.Where(c => c.IsSelected).ToArray();
                foreach (PAScheduleForPWNode selectedLine in selectedLines)
                {
                    List<ProdOrderBatchPlan> lineItems = GetProdOrderBatchPlanList(selectedLine.MDSchedulingGroupID).ToList();
                    selectedBatchPlans.AddRange(lineItems);
                }
            }
            else if (ProdOrderBatchPlanList != null && ProdOrderBatchPlanList.Where(c => c.IsSelected).Any())
            {
                selectedBatchPlans = ProdOrderBatchPlanList.Where(c => c.IsSelected).ToList();
            }

            List<PreparedMaterial> preparedMaterials = new List<PreparedMaterial>();
            if (selectedBatchPlans.Any())
            {
                searchModel = GetSearchBatchMaterialModels(selectedBatchPlans);
                preparedMaterials = BSOMaterialPreparationChild.Value.GetPreparedMaterials(searchModel);
            }

            return preparedMaterials;
        }

        private void DoSearchStockMaterialFinish(List<PreparedMaterial> preparedMaterials)
        {
            BSOMaterialPreparationChild.Value.LoadMaterialPlanFromPos(preparedMaterials);
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

        #endregion

        #endregion

        #region Transformation - WFsLines

        /// <summary>
        /// InitialBuildLines
        /// </summary>
        /// <exception cref="Exception"></exception>
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
                OnPropertyChanged(nameof(ScheduleForPWNodeList));
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

        #region Test
        public void MyTestGit()
        {
            bool testbool = false;        
        }
        #endregion
    }


    /// <summary>
    /// State in Wizard
    /// </summary>
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