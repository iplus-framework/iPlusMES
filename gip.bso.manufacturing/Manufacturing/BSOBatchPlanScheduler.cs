using gip.bso.masterdata;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.facility;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Objects;
using System.Linq;
using VD = gip.mes.datamodel;
using System.Data;
using gip.core.media;
using gip.mes.datamodel;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Batch scheduler'}de{'Batch Zeitplaner'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + VD.ProdOrderBatchPlan.ClassName)]
    public class BSOBatchPlanScheduler : BSOWorkflowSchedulerBase
    {
        #region const
        public const string BGWorkerMethod_DoBackwardScheduling = @"DoBackwardScheduling";
        public const string BGWorkerMethod_DoForwardScheduling = @"DoForwardScheduling";
        public const string BGWorkerMethod_DoCalculateAll = @"DoCalculateAll";
        public const string BGWorkerMehtod_DoGenerateBatchPlans = @"DoGenerateBatchPlans";
        public const string BGWorkerMehtod_DoMergeOrders = @"DoMergeOrders";
        public const string BGWorkerMehtod_DoSearchStockMaterial = @"DoSearchStockMaterial";
        #endregion

        #region Configuration

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

        private ACPropertyConfigValue<VD.GlobalApp.BatchPlanState> _CreatedBatchState;
        [ACPropertyConfig("en{'Created batch state'}de{'Neu Batch Status'}")]
        public VD.GlobalApp.BatchPlanState CreatedBatchState
        {
            get
            {
                return _CreatedBatchState.ValueT;
            }
            set
            {
                _CreatedBatchState.ValueT = value;
                OnPropertyChanged();
            }
        }


        private TimeSpan? GetExpectedBatchEndTime(WizardSchedulerPartslist wizardSchedulerPartslist)
        {
            VD.ACClassWF vbACClassWF = wizardSchedulerPartslist.WFNodeMES;
            if (vbACClassWF == null)
                return null;
            VD.Partslist partslist = wizardSchedulerPartslist.Partslist;  //DatabaseApp.Partslist.FirstOrDefault(c => c.PartslistNo == wizardSchedulerPartslist.PartslistNo);
            var materialWFConnection = ProdOrderManager.GetMaterialWFConnection(vbACClassWF, partslist.MaterialWFID);

            ACProdOrderManager poManager = ACProdOrderManager.GetServiceInstance(this);
            return poManager.GetCalculatedBatchPlanDuration(DatabaseApp, materialWFConnection.MaterialWFACClassMethodID, vbACClassWF.ACClassWFID);
        }

        private ACPropertyConfigValue<bool> _ShowImages;
        [ACPropertyConfig("en{'Show images'}de{'Bilder anzeigen'}")]
        public bool ShowImages
        {
            get
            {
                return _ShowImages.ValueT;
            }
            set
            {
                _ShowImages.ValueT = value;
            }
        }

        private ACPropertyConfigValue<bool> _ValidateBatchPlanBeforeStart;
        [ACPropertyConfig("en{'Validate batch plan on start'}de{'Validierung des Batchplans bei Start'}")]
        public bool ValidateBatchPlanBeforeStart
        {
            get
            {
                return _ValidateBatchPlanBeforeStart.ValueT;
            }
            set
            {
                _ValidateBatchPlanBeforeStart.ValueT = value;
            }
        }

        private ACPropertyConfigValue<double> _RoundingQuantity;
        [ACPropertyConfig("en{'Rounding Quantity'}de{'Rundungsmenge'}")]
        public double RoundingQuantity
        {
            get
            {
                return _RoundingQuantity.ValueT;
            }
            set
            {
                _RoundingQuantity.ValueT = value;
            }
        }

        private ACPropertyConfigValue<int> _ShowOnlyNotPlannedOrdersConfig;
        [ACPropertyConfig("en{'Rounding Quantity'}de{'Rundungsmenge'}")]
        public int ShowOnlyNotPlannedOrdersConfig
        {
            get
            {
                return _ShowOnlyNotPlannedOrdersConfig.ValueT;
            }
            set
            {
                _ShowOnlyNotPlannedOrdersConfig.ValueT = value;
            }
        }

        protected override string LoadPABatchPlanSchedulerURL()
        {
            string acUrl = @"\Planning\BatchPlanScheduler";
            using (ACMonitor.Lock(DatabaseApp.ContextIPlus.QueryLock_1X000))
            {
                core.datamodel.ACClass paClass = DatabaseApp.ContextIPlus.ACClass.FirstOrDefault(c => c.ACIdentifier == nameof(PABatchPlanScheduler) && !c.ACProject.IsProduction);
                while (paClass != null)
                {
                    acUrl = paClass.ACURLComponentCached;
                    paClass = paClass.ACClass_BasedOnACClass.Where(c => c.ACProject.IsProduction).FirstOrDefault();
                }
            }
            return acUrl;
        }


        #endregion

        #region c´tors

        public BSOBatchPlanScheduler(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _AutoRemoveMDSGroupFrom = new ACPropertyConfigValue<int>(this, nameof(AutoRemoveMDSGroupFrom), 0);
            _AutoRemoveMDSGroupTo = new ACPropertyConfigValue<int>(this, nameof(AutoRemoveMDSGroupTo), 0);
            _CreatedBatchState = new ACPropertyConfigValue<VD.GlobalApp.BatchPlanState>(this, nameof(CreatedBatchState), VD.GlobalApp.BatchPlanState.Created);
            _ShowImages = new ACPropertyConfigValue<bool>(this, nameof(ShowImages), false);
            _ValidateBatchPlanBeforeStart = new ACPropertyConfigValue<bool>(this, nameof(ValidateBatchPlanBeforeStart), false);
            _BSOBatchPlanSchedulerRules = new ACPropertyConfigValue<string>(this, nameof(BSOBatchPlanSchedulerRules), "");
            _RoundingQuantity = new ACPropertyConfigValue<double>(this, nameof(RoundingQuantity), 0);
            _ShowOnlyNotPlannedOrdersConfig = new ACPropertyConfigValue<int>(this, nameof(ShowOnlyNotPlannedOrdersConfig), -1);
        }

        #region c´tors -> ACInit

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            _ProdOrderManager = ACProdOrderManager.ACRefToServiceInstance(this);
            if (_ProdOrderManager == null)
                throw new Exception("ProdOrderManager not configured");

            _SchedulingForecastManager = SchedulingForecastManager.ACRefToServiceInstance(this);
            if (_SchedulingForecastManager == null)
                throw new Exception("SchedulingForecastManager not configured");

            MediaController = ACMediaController.GetServiceInstance(this);

            if (FilterProdPartslistOrderList != null)
                SelectedFilterProdPartslistOrder = FilterProdPartslistOrderList.Where(c => (BatchPlanProdOrderSortFilterEnum)c.Value == BatchPlanProdOrderSortFilterEnum.StartTime).FirstOrDefault();

            _PartslistMDSchedulerGroupConnections = ProdOrderManager.GetPartslistMDSchedulerGroupConnections(DatabaseApp, PWNodeProcessWorkflowVB.PWClassName);

            if (!base.ACInit(startChildMode))
                return false;

            _ = AutoRemoveMDSGroupFrom;
            _ = AutoRemoveMDSGroupTo;
            _ = CreatedBatchState;
            _ = ShowImages;
            _ = ValidateBatchPlanBeforeStart;
            _ = RoundingQuantity;
            _ = ShowOnlyNotPlannedOrdersConfig;

            _ShowOnlyNotPlannedOrders = null;
            if (ShowOnlyNotPlannedOrdersConfig == 0)
            {
                _ShowOnlyNotPlannedOrders = false;
            }
            else if (ShowOnlyNotPlannedOrdersConfig == 1)
            {
                _ShowOnlyNotPlannedOrders = true;
            }

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

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_ProdOrderManager != null)
                ACProdOrderManager.DetachACRefFromServiceInstance(this, _ProdOrderManager);
            _ProdOrderManager = null;

            if (_SchedulingForecastManager != null)
                SchedulingForecastManager.DetachACRefFromServiceInstance(this, _SchedulingForecastManager);
            _SchedulingForecastManager = null;

            if (BSOPartslistExplorer_Child != null && BSOPartslistExplorer_Child.Value != null && BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child != null && BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value != null)
                BSOPartslistExplorer_Child.Value.BSOMaterialExplorer_Child.Value.PropertyChanged -= ChildBSO_PropertyChanged;

            if (BSOPartslistExplorer_Child != null && BSOPartslistExplorer_Child.Value != null)
                BSOPartslistExplorer_Child.Value.PropertyChanged -= ChildBSO_PropertyChanged;

            if (BSOMaterialPreparationChild != null && BSOMaterialPreparationChild.Value != null)
                BSOMaterialPreparationChild.Value.OnSearchStockMaterial -= Value_OnSearchStockMaterial;

            if (LocalBSOBatchPlan != null)
                LocalBSOBatchPlan.PropertyChanged += ChildBSO_PropertyChanged;

            MediaController = null;
            SelectedProdOrderBatchPlan = null;
            IsWizard = false;

            return base.ACDeInit(deleteACClassTask);
        }

        private void Value_OnSearchStockMaterial(object sender, EventArgs e)
        {
            if (!BackgroundWorker.IsBusy)
            {
                BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoSearchStockMaterial);
                ShowDialog(this, DesignNameProgressBar);
            }
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(AddBatchPlan):
                    AddBatchPlan();
                    return true;
                case nameof(AddSuggestion):
                    AddSuggestion();
                    return true;
                case nameof(BackwardScheduling):
                    BackwardScheduling();
                    return true;
                case nameof(BackwardSchedulingOk):
                    BackwardSchedulingOk();
                    return true;
                case nameof(BatchPlanEdit):
                    BatchPlanEdit();
                    return true;
                case nameof(ChangeBatchPlan):
                    ChangeBatchPlan((gip.mes.datamodel.ProdOrderBatchPlan)acParameter[0]);
                    return true;
                case nameof(DeleteBatch):
                    DeleteBatch();
                    return true;
                case nameof(ForwardScheduling):
                    ForwardScheduling();
                    return true;
                case nameof(ForwardSchedulingOk):
                    ForwardSchedulingOk();
                    return true;
                case nameof(GenerateBatchPlans):
                    GenerateBatchPlans();
                    return true;
                case nameof(IsEnabledAddBatchPlan):
                    result = IsEnabledAddBatchPlan();
                    return true;
                case nameof(IsEnabledAddSuggestion):
                    result = IsEnabledAddSuggestion();
                    return true;
                case nameof(IsEnabledBackwardScheduling):
                    result = IsEnabledBackwardScheduling();
                    return true;
                case nameof(IsEnabledBackwardSchedulingOk):
                    result = IsEnabledBackwardSchedulingOk();
                    return true;
                case nameof(IsEnabledBatchPlanEdit):
                    result = IsEnabledBatchPlanEdit();
                    return true;
                case nameof(IsEnabledDeleteBatch):
                    result = IsEnabledDeleteBatch();
                    return true;
                case nameof(IsEnabledForwardScheduling):
                    result = IsEnabledForwardScheduling();
                    return true;
                case nameof(IsEnabledForwardSchedulingOk):
                    result = IsEnabledForwardSchedulingOk();
                    return true;
                case nameof(IsEnabledGenerateBatchPlans):
                    result = IsEnabledGenerateBatchPlans();
                    return true;
                case nameof(IsEnabledMergeOrders):
                    result = IsEnabledMergeOrders();
                    return true;
                case nameof(IsEnabledMoveSelectedBatchDown):
                    result = IsEnabledMoveSelectedBatchDown();
                    return true;
                case nameof(IsEnabledMoveSelectedBatchUp):
                    result = IsEnabledMoveSelectedBatchUp();
                    return true;
                case nameof(IsEnabledMoveToOtherLine):
                    result = IsEnabledMoveToOtherLine();
                    return true;
                case nameof(IsEnabledNavigateToProdOrder):
                    result = IsEnabledNavigateToProdOrder();
                    return true;
                case nameof(IsEnabledNavigateToProdOrder2):
                    result = IsEnabledNavigateToProdOrder2();
                    return true;
                case nameof(IsEnabledNavigateToProdOrder3):
                    result = IsEnabledNavigateToProdOrder3();
                    return true;
                case nameof(IsEnabledNew):
                    result = IsEnabledNew();
                    return true;
                case nameof(IsEnabledPossibleRoutesCheck):
                    result = IsEnabledPossibleRoutesCheck();
                    return true;
                case nameof(IsEnabledRecalculateBatchSuggestion):
                    result = IsEnabledRecalculateBatchSuggestion();
                    return true;
                case nameof(IsEnabledRemoveSelectedProdorderPartslist):
                    result = IsEnabledRemoveSelectedProdorderPartslist();
                    return true;
                case nameof(IsEnabledRemoveSuggestion):
                    result = IsEnabledRemoveSuggestion();
                    return true;
                case nameof(IsEnabledScheduling):
                    result = IsEnabledScheduling();
                    return true;
                case nameof(IsEnabledSearch):
                    result = IsEnabledSearch();
                    return true;
                case nameof(IsEnabledSearchOrders):
                    result = IsEnabledSearchOrders();
                    return true;
                case nameof(IsEnabledSetBatchStateCancelled):
                    result = IsEnabledSetBatchStateCancelled();
                    return true;
                case nameof(IsEnabledSetBatchStateCreated):
                    result = IsEnabledSetBatchStateCreated();
                    return true;
                case nameof(IsEnabledSetBatchStateReadyToStart):
                    result = IsEnabledSetBatchStateReadyToStart();
                    return true;
                case nameof(IsEnabledShowComponents):
                    result = IsEnabledShowComponents();
                    return true;
                case nameof(IsEnabledShowParslist):
                    result = IsEnabledShowParslist();
                    return true;
                case nameof(IsEnabledShowPartslistOK):
                    result = IsEnabledShowPartslistOK();
                    return true;
                case nameof(IsEnabledShowPreferredParameters):
                    result = IsEnabledShowPreferredParameters();
                    return true;
                case nameof(IsEnabledWizardBackward):
                    result = IsEnabledWizardBackward();
                    return true;
                case nameof(IsEnabledWizardForward):
                    result = IsEnabledWizardForward();
                    return true;
                case nameof(MergeOrders):
                    MergeOrders();
                    return true;
                case nameof(MoveSelectedBatchDown):
                    MoveSelectedBatchDown();
                    return true;
                case nameof(MoveSelectedBatchUp):
                    MoveSelectedBatchUp();
                    return true;
                case nameof(MoveToOtherLine):
                    MoveToOtherLine();
                    return true;
                case nameof(NavigateToProdOrder):
                    NavigateToProdOrder();
                    return true;
                case nameof(NavigateToProdOrder2):
                    NavigateToProdOrder2();
                    return true;
                case nameof(NavigateToProdOrder3):
                    NavigateToProdOrder3();
                    return true;
                case nameof(New):
                    New();
                    return true;
                case nameof(RecalculateBatchSuggestion):
                    RecalculateBatchSuggestion();
                    return true;
                case nameof(RemoveSelectedProdorderPartslist):
                    RemoveSelectedProdorderPartslist();
                    return true;
                case nameof(RemoveSuggestion):
                    RemoveSuggestion();
                    return true;
                case nameof(RunPossibleRoutesCheck):
                    RunPossibleRoutesCheck();
                    return true;
                case nameof(SchedulingCalculateAll):
                    SchedulingCalculateAll();
                    return true;
                case nameof(SchedulingCancel):
                    SchedulingCancel();
                    return true;
                case nameof(SearchOrders):
                    SearchOrders();
                    return true;
                case nameof(SearchOrdersAll):
                    SearchOrdersAll();
                    return true;
                case nameof(SetBatchStateCancelled):
                    SetBatchStateCancelled();
                    return true;
                case nameof(SetBatchStateCreated):
                    SetBatchStateCreated();
                    return true;
                case nameof(SetBatchStateReadyToStart):
                    SetBatchStateReadyToStart();
                    return true;
                case nameof(ShowBatchPlansOnTimeline):
                    ShowBatchPlansOnTimeline();
                    return true;
                case nameof(ShowComponents):
                    ShowComponents();
                    return true;
                case nameof(ShowParslist):
                    ShowParslist();
                    return true;
                case nameof(ShowPartslistOK):
                    ShowPartslistOK();
                    return true;
                case nameof(ShowPreferredParameters):
                    ShowPreferredParameters();
                    return true;
                case nameof(WizardBackward):
                    WizardBackward();
                    return true;
                case nameof(WizardCancel):
                    WizardCancel();
                    return true;
                case nameof(WizardDeletePartslist):
                    WizardDeletePartslist((System.Object)acParameter[0]);
                    return true;
                case nameof(WizardForward):
                    WizardForward();
                    return true;
                case nameof(WizardForwardSelectLinie):
                    WizardForwardSelectLinie((System.Object)acParameter[0]);
                    return true;
                case nameof(WizardSetPreferredParams):
                    WizardSetPreferredParams((System.Object)acParameter[0]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        private void ChildBSO_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BSOPartslist.SelectedPartslist))
            {
                OnPropertyChanged(nameof(WizardSchedulerPartslist.UnitConvertList));
                if (BSOPartslistExplorer_Child.Value.SelectedPartslist != null
                    && BSOPartslistExplorer_Child.Value.SelectedPartslist.Material != null)
                {
                    // Always Base-UOM:
                    VD.Partslist selectedPartslist = BSOPartslistExplorer_Child.Value.SelectedPartslist;
                    if (SelectedScheduleForPWNode != null)
                        WizardDefineDefaultPartslist(SelectedScheduleForPWNode.MDSchedulingGroup, selectedPartslist, 0);
                }
            }
        }

        protected override void OnPAScheduleForPWNodeChanged()
        {
            OnPropertyChanged(nameof(TargetScheduleForPWNodeList));
            if (TargetScheduleForPWNodeList != null)
                SelectedTargetScheduleForPWNode = TargetScheduleForPWNodeList.FirstOrDefault();
            else
                SelectedTargetScheduleForPWNode = null;

            SelectedTargetScheduleForPWNode = null;
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

        public ACMediaController MediaController { get; set; }

        #endregion

        #region Child (Local BSOs)

        ACChildItem<BSOPartslistExplorer> _BSOPartslistExplorer_Child;
        [ACPropertyInfo(590)]
        [ACChildInfo(nameof(BSOPartslistExplorer_Child), typeof(BSOPartslistExplorer))]
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
        public const string BSOBatchPlan_Child = "BSOBatchPlan_Child";
        [ACPropertyInfo(591)]
        [ACChildInfo(nameof(BSOBatchPlan_Child), typeof(BSOBatchPlan))]
        public ACChildItem<BSOBatchPlan> BSOBatchPlanChild
        {
            get
            {
                if (_BSOBatchPlanChild == null)
                    _BSOBatchPlanChild = new ACChildItem<BSOBatchPlan>(this, nameof(BSOBatchPlan_Child));
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
        public const string BSOMaterialPreparation_Child = "BSOMaterialPreparation_Child";
        [ACPropertyInfo(593)]
        [ACChildInfo(nameof(BSOMaterialPreparation_Child), typeof(BSOMaterialPreparation))]
        public ACChildItem<BSOMaterialPreparation> BSOMaterialPreparationChild
        {
            get
            {
                if (_BSOMaterialPreparationChild == null)
                    _BSOMaterialPreparationChild = new ACChildItem<BSOMaterialPreparation>(this, nameof(BSOMaterialPreparation_Child));
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

        #region Properties -> Explorer -> TargetScheduleForPWNode
        public const string TargetScheduleForPWNode = "TargetScheduleForPWNode";
        private PAScheduleForPWNode _SelectedTargetScheduleForPWNode;
        [ACPropertySelected(503, nameof(TargetScheduleForPWNode), "en{'Move to other line'}de{'Zu anderer Linie verschieben'}")]
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
                    OnPropertyChanged();
                }
            }
        }

        [ACPropertyList(504, nameof(TargetScheduleForPWNode))]
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
        [ACPropertyInfo(999, nameof(FilterBatchSelectAll), VD.ConstApp.SelectAll)]
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
                    OnPropertyChanged();

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
        [ACPropertyInfo(999, nameof(FilterBatchProgramNo), VD.ConstApp.ProdOrderProgramNo)]
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
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilterBatchMaterialNo;
        [ACPropertyInfo(999, nameof(FilterBatchMaterialNo), VD.ConstApp.Material)]
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
                    OnPropertyChanged();
                }
            }
        }

        #region Properties -> (Tab)BatchPlanScheduler -> Filter (Search) -> FilterConnectedLine [PAScheduleForPWNode]

        public const string FilterConnectedLine = "FilterConnectedLine";
        private PAScheduleForPWNode _SelectedFilterConnectedLine;
        /// <summary>
        /// Selected property for PAScheduleForPWNode
        /// </summary>
        /// <value>The selected FilterConnectedLine</value>
        [ACPropertySelected(9999, nameof(FilterConnectedLine), "en{'Show only related orders from production line'}de{'Nur verbundene Aufträge aus Produktionslinie anzeigen'}")]
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
                    OnPropertyChanged();
                }
            }
        }


        private List<PAScheduleForPWNode> _FilterConnectedLineList;
        /// <summary>
        /// List property for PAScheduleForPWNode
        /// </summary>
        /// <value>The FilterConnectedLine list</value>
        [ACPropertyList(9999, nameof(FilterConnectedLine))]
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
            PAScheduleForPWNode emptyNode = new PAScheduleForPWNode() { MDSchedulingGroup = new VD.MDSchedulingGroup() { MDSchedulingGroupName = "-" } };
            list.Insert(0, emptyNode);
            return list;
        }

        #endregion

        #region Properties -> (Tab)BatchPlanScheduler -> Filter (Search) -> FilterBatchPlanGroup [MDBatchPlanGroup]

        public const string FilterBatchPlanGroup = "FilterBatchPlanGroup";
        private VD.MDBatchPlanGroup _SelectedFilterBatchPlanGroup;
        /// <summary>
        /// Selected property for PAScheduleForPWNode
        /// </summary>
        /// <value>The selected FilterConnectedLine</value>
        [ACPropertySelected(9999, nameof(FilterBatchPlanGroup), "en{'Batchplan group'}de{'Batchplan Gruppe'}")]
        public VD.MDBatchPlanGroup SelectedFilterBatchPlanGroup
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
                    OnPropertyChanged();
                }
            }
        }


        private List<VD.MDBatchPlanGroup> _FilterBatchPlanGroupList;
        /// <summary>
        /// List property for PAScheduleForPWNode
        /// </summary>
        /// <value>The FilterConnectedLine list</value>
        [ACPropertyList(9999, nameof(FilterBatchPlanGroup), "en{'Batchplan group'}de{'Batchplan Gruppe'}")]
        public List<VD.MDBatchPlanGroup> FilterBatchPlanGroupList
        {
            get
            {
                if (_FilterBatchPlanGroupList == null)
                    _FilterBatchPlanGroupList = LoadFilterBatchPlanGroupList();
                return _FilterBatchPlanGroupList;
            }
        }

        private List<VD.MDBatchPlanGroup> LoadFilterBatchPlanGroupList()
        {
            return DatabaseApp.MDBatchPlanGroup.OrderBy(c => c.SortIndex).ToList();
        }


        #endregion
        /// <summary>
        /// Source Property: 
        /// </summary>
        private bool _FilterOnlyOnThisLine;
        [ACPropertyInfo(999, nameof(FilterOnlyOnThisLine), "en{'Production orders planned on this line'}de{'Auf dieser Linie geplante Produktionsaufträge'}")]
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
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Properties -> (Tab)BatchPlanScheduler -> ProdOrderBatchPlan
        private VD.ProdOrderBatchPlan _SelectedProdOrderBatchPlan;
        [ACPropertySelected(505, nameof(VD.ProdOrderBatchPlan))]
        public VD.ProdOrderBatchPlan SelectedProdOrderBatchPlan
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
                OnPropertyChanged();
            }
        }

        private void _SelectedProdOrderBatchPlan_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VD.ProdOrderBatchPlan.PartialTargetCount)
                && SelectedScheduleForPWNode != null
                && SelectedScheduleForPWNode.StartMode == VD.BatchPlanStartModeEnum.SemiAutomatic
                && !_IsRefreshingBatchPlan)
            {
                if (SelectedProdOrderBatchPlan.PartialTargetCount.HasValue && SelectedProdOrderBatchPlan.PartialTargetCount > 0)
                {
                    if (
                            (SelectedProdOrderBatchPlan.ProdOrderPartslist != null && SelectedProdOrderBatchPlan.ProdOrderPartslist.Partslist.IsEnabled)
                            && (SelectedProdOrderBatchPlan.PlanState <= VD.GlobalApp.BatchPlanState.Created
                            || SelectedProdOrderBatchPlan.PlanState >= VD.GlobalApp.BatchPlanState.Paused)
                        )
                        SetReadyToStart(new VD.ProdOrderBatchPlan[] { SelectedProdOrderBatchPlan });
                    else
                        Save();
                }
                else if (SelectedProdOrderBatchPlan.PartialTargetCount.HasValue && SelectedProdOrderBatchPlan.PartialTargetCount <= 0)
                {
                    SelectedProdOrderBatchPlan.PartialTargetCount = null;
                    if (SelectedProdOrderBatchPlan.PlanState == VD.GlobalApp.BatchPlanState.ReadyToStart)
                        SelectedProdOrderBatchPlan.PlanState = VD.GlobalApp.BatchPlanState.Paused;
                    Save();
                }
            }
        }

        private ObservableCollection<VD.ProdOrderBatchPlan> _ProdOrderBatchPlanList;
        [ACPropertyList(506, nameof(VD.ProdOrderBatchPlan))]
        public ObservableCollection<VD.ProdOrderBatchPlan> ProdOrderBatchPlanList
        {
            get
            {
                if (_ProdOrderBatchPlanList == null)
                    _ProdOrderBatchPlanList = new ObservableCollection<VD.ProdOrderBatchPlan>();
                return _ProdOrderBatchPlanList;
            }
            protected set
            {
                _ProdOrderBatchPlanList = value;
                OnPropertyChanged();
            }
        }

        private bool _IsRefreshingBatchPlan = false;
        private ObservableCollection<VD.ProdOrderBatchPlan> GetProdOrderBatchPlanList(Guid? mdSchedulingGroupID)
        {
            if (!mdSchedulingGroupID.HasValue)
                return new ObservableCollection<VD.ProdOrderBatchPlan>();

            VD.GlobalApp.BatchPlanState startState = VD.GlobalApp.BatchPlanState.Created;
            VD.GlobalApp.BatchPlanState endState = VD.GlobalApp.BatchPlanState.Paused;
            VD.MDProdOrderState.ProdOrderStates? minProdOrderState = null;
            VD.MDProdOrderState.ProdOrderStates? maxProdOrderState = MDProdOrderState.ProdOrderStates.InProduction;
            ObservableCollection<VD.ProdOrderBatchPlan> prodOrderBatchPlans = null;
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
                        maxProdOrderState,
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
                        maxProdOrderState,
                       FilterPlanningMR?.PlanningMRID,
                       SelectedFilterBatchPlanGroup?.MDBatchPlanGroupID,
                       FilterBatchProgramNo,
                       FilterBatchMaterialNo)
                   .Select(c => c.ProdOrderPartslist.ProdOrderID);
                    prodOrderBatchPlans = new ObservableCollection<VD.ProdOrderBatchPlan>(prodOrderBatchPlans.Where(c => includedProductionOrders.Contains(c.ProdOrderPartslist.ProdOrderID)));
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
                    if (ShowImages)
                    {
                        VD.Material material = batchPlan.ProdOrderPartslist.Partslist.Material;
                        MediaController.LoadIImageInfo(material);
                    }

                    batchPlan.ParamState = VD.PreferredParamStateEnum.ParamsNotRequired;
                    if (batchPlan.IplusVBiACClassWF.ACClassMethod.HasRequiredParams)
                    {
                        if (batchPlan.ProdOrderPartslist.ProdOrderPartslistConfig_ProdOrderPartslist.Any())
                        {
                            batchPlan.ParamState = VD.PreferredParamStateEnum.ParamsRequiredDefined;
                        }
                        else
                        {
                            batchPlan.ParamState = VD.PreferredParamStateEnum.ParamsRequiredNotDefined;
                        }
                    }
                    // For refresh actual production
                    batchPlan.ProdOrderPartslistPos.Refresh(RefreshMode.StoreWins);
                    foreach (var childPos in batchPlan.ProdOrderBatch_ProdOrderBatchPlan)
                    {
                        childPos.Refresh(RefreshMode.StoreWins);
                    }
                }
            }

            return prodOrderBatchPlans;
        }

        #endregion

        #region Properties -> (Tab)BatchPlanScheduler -> Scheduling

        private DateTime? _ScheduledStartDate;
        [ACPropertyInfo(518, nameof(ScheduledStartDate), "en{'Planned Start Date'}de{'Geplante Startzeit'}")]
        public DateTime? ScheduledStartDate
        {
            get
            {
                return _ScheduledStartDate;
            }
            set
            {
                _ScheduledStartDate = value;
                OnPropertyChanged();
            }
        }


        private DateTime? _ScheduledEndDate;
        [ACPropertyInfo(518, nameof(ScheduledEndDate), "en{'Planned End Date'}de{'Geplante Endezeit'}")]
        public DateTime? ScheduledEndDate
        {
            get
            {
                return _ScheduledEndDate;
            }
            set
            {
                _ScheduledEndDate = value;
                OnPropertyChanged();
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
                {
                    _PartslistMDSchedulerGroupConnections = new List<PartslistMDSchedulerGroupConnection>();
                }
                return _PartslistMDSchedulerGroupConnections;
            }
        }

        private VD.PlanningMR _FilterPlanningMR;
        public VD.PlanningMR FilterPlanningMR
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
                    RefreshScheduleForSelectedNode();
                    OnPropertyChanged();
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
        [ACPropertyInfo(999, nameof(FilterOrderSelectAll), "en{'Select all'}de{'Alles auswählen'}")]
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
                    OnPropertyChanged();

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
        [ACPropertyInfo(999, nameof(FilterOrderStartTime), "en{'From'}de{'Von'}")]
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
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FilterOrderEndTime));
                }
            }
        }

        private DateTime? _FilterOrderEndTime;
        /// <summary>
        /// Filter to date
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, nameof(FilterOrderEndTime), "en{'To'}de{'Bis'}")]
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
                    OnPropertyChanged();
                }
            }
        }

        private bool? _FilterOrderIsCompleted = false;
        /// <summary>
        /// Filter for finshed orders
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, nameof(FilterOrderIsCompleted), "en{'Only completed production orders'}de{'Nur erledigte Produktionsaufträge'}")]
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
                    OnPropertyChanged();
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
        [ACPropertyInfo(999, nameof(FilterDepartmentUserName), "en{'Department'}de{'Abteilung'}")]
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
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Filter Order numer
        /// </summary>
        private string _FilterOrderProgramNo;
        [ACPropertyInfo(999, nameof(FilterOrderProgramNo), VD.ConstApp.ProdOrderProgramNo)]
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
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Filter Material number
        /// </summary>
        private string _FilterOrderMaterialNo;
        [ACPropertyInfo(999, nameof(FilterOrderMaterialNo), VD.ConstApp.Material)]
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
                    OnPropertyChanged();
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
        [ACPropertySelected(305, nameof(FilterProdPartslistOrder), "en{'Sort order'}de{'Sortierreihenfolge'}")]
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
                    OnPropertyChanged();
                }
            }
        }

        private List<ACValueItem> _FilterProdPartslistOrderList;
        /// <summary>
        /// List property for ACValueItem
        /// </summary>
        /// <value>The FilterPickingState list</value>
        [ACPropertyList(306, nameof(FilterProdPartslistOrder))]
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

        /// <summary>
        /// Source Property: 
        /// </summary>
        private bool? _ShowOnlyNotPlannedOrders;
        [ACPropertyInfo(999, nameof(ShowOnlyNotPlannedOrders), "en{'Only not planned orders'}de{'Nur nicht geplante Aufträge'}", IsPersistable = true)]
        public bool? ShowOnlyNotPlannedOrders
        {
            get
            {
                return _ShowOnlyNotPlannedOrders;
            }
            set
            {
                if (_ShowOnlyNotPlannedOrders != value)
                {
                    _ShowOnlyNotPlannedOrders = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ProdOrderPartslistList));

                    // save vlaue to config (ShowOnlyNotPlannedOrdersConfig)
                    if (_ShowOnlyNotPlannedOrders == null)
                    {
                        ShowOnlyNotPlannedOrdersConfig = -1;
                    }
                    else if (!(_ShowOnlyNotPlannedOrders ?? true))
                    {
                        ShowOnlyNotPlannedOrdersConfig = 0;
                    }
                    else if ((_ShowOnlyNotPlannedOrders ?? false))
                    {
                        ShowOnlyNotPlannedOrdersConfig = 1;
                    }
                }
            }
        }

        #region Properties -> (Tab)ProdOrder -> Filter -> FiterProdOrderWorkDay
        public const string FiterProdOrderWorkDay = "FiterProdOrderWorkDay";

        private ACValueItem _CurrentFiterProdOrderWorkDay;
        [ACPropertyCurrent(999, nameof(FiterProdOrderWorkDay), "en{'Day'}de{'Tag'}")]
        public ACValueItem CurrentFiterProdOrderWorkDay
        {
            get
            {
                return _CurrentFiterProdOrderWorkDay;
            }
            set
            {
                if(_CurrentFiterProdOrderWorkDay != value)
                {
                    _CurrentFiterProdOrderWorkDay = value;
                    OnPropertyChanged();
                    SetProdOrderTimeFilterFromDay(_CurrentFiterProdOrderWorkDay);
                }
            }
        }

        [ACPropertyList(999, nameof(FiterProdOrderWorkDay))]
        public ACValueItemList FiterProdOrderWorkDayList
        {
            get
            {
                return Global.DayOfWeekList;
            }
        }

        #endregion

        #endregion

        #endregion

        #region Properties -> (Tab)ProdOrder -> ProdOrderPartslist

        private ProdOrderPartslistPlanWrapper _SelectedProdOrderPartslist;
        [ACPropertySelected(507, nameof(VD.ProdOrderPartslist))]
        public ProdOrderPartslistPlanWrapper SelectedProdOrderPartslist
        {
            get => _SelectedProdOrderPartslist;
            set
            {
                _SelectedProdOrderPartslist = value;
                OnPropertyChanged();
            }
        }

        private IEnumerable<ProdOrderPartslistPlanWrapper> _ProdOrderPartslistList;
        [ACPropertyList(507, nameof(VD.ProdOrderPartslist))]
        public IEnumerable<ProdOrderPartslistPlanWrapper> ProdOrderPartslistList
        {
            get
            {
                if (_ProdOrderPartslistList != null)
                {
                    foreach (var item in _ProdOrderPartslistList)
                    {
                        _ = item.PlanningState;
                    }
                }
                return
                    _ProdOrderPartslistList
                    .Where(c =>
                                ShowOnlyNotPlannedOrders == null
                                || ((!(ShowOnlyNotPlannedOrders ?? true)) && c.PlanningState != ProdOrderPartslistPlanWrapper.PlanningStateEnum.UnPlanned)
                                || ((ShowOnlyNotPlannedOrders ?? false) && c.PlanningState == ProdOrderPartslistPlanWrapper.PlanningStateEnum.UnPlanned)
                    )
                    .ToList();
            }
            set
            {
                _ProdOrderPartslistList = value;
                OnPropertyChanged();
            }
        }

        public virtual IEnumerable<ProdOrderPartslistPlanWrapper> GetProdOrderPartslistList()
        {
            if (SelectedScheduleForPWNode == null)
                return new List<ProdOrderPartslistPlanWrapper>();

            VD.MDProdOrderState.ProdOrderStates? minProdOrderState = null;
            VD.MDProdOrderState.ProdOrderStates? maxProdOrderState = null;
            if (FilterOrderIsCompleted != null)
            {
                if (FilterOrderIsCompleted.Value)
                    minProdOrderState = VD.MDProdOrderState.ProdOrderStates.ProdFinished;
                else
                    maxProdOrderState = VD.MDProdOrderState.ProdOrderStates.InProduction;
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
            IOrderedQueryable<ProdOrderPartslistPlanWrapper> query = batchQuery;
            if (FilterProdPartslistOrder != null)
            {
                switch (FilterProdPartslistOrder)
                {
                    case BatchPlanProdOrderSortFilterEnum.Material:
                        query = query.OrderBy(c => c.ProdOrderPartslist.Partslist.Material.MaterialNo).ThenBy(c => c.ProdOrderPartslist.StartDate);
                        break;
                    case BatchPlanProdOrderSortFilterEnum.StartTime:
                        query = query.OrderBy(c => c.ProdOrderPartslist.StartDate);
                        break;
                    case BatchPlanProdOrderSortFilterEnum.ProgramNo:
                        query = query.OrderBy(c => c.ProdOrderPartslist.ProdOrder.ProgramNo);
                        break;
                }
            }
            return query.Take(Const_MaxResultSize).ToList();
        }

        protected static readonly Func<VD.DatabaseApp, Guid, Guid?, DateTime?, DateTime?, short?, short?, bool, string, string, string, IQueryable<ProdOrderPartslistPlanWrapper>> s_cQry_ProdOrderPartslistForPWNode =
        CompiledQuery.Compile<VD.DatabaseApp, Guid, Guid?, DateTime?, DateTime?, short?, short?, bool, string, string, string, IQueryable<ProdOrderPartslistPlanWrapper>>(
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

        #region Properties -> (Tab)ProdOrder-> FinishedProdOrderBatch
        public const string FinishedProdOrderBatch = "FinishedProdOrderBatch";

        private VD.ProdOrderPartslistPos _SelectedFinishedProdOrderBatch;
        [ACPropertySelected(605, nameof(FinishedProdOrderBatch))]
        public VD.ProdOrderPartslistPos SelectedFinishedProdOrderBatch
        {
            get
            {
                return _SelectedFinishedProdOrderBatch;
            }
            set
            {
                _SelectedFinishedProdOrderBatch = value;
                OnPropertyChanged();
            }
        }


        private List<VD.ProdOrderPartslistPos> _FinishedProdOrderBatchList;
        [ACPropertyList(606, nameof(FinishedProdOrderBatch))]
        public List<VD.ProdOrderPartslistPos> FinishedProdOrderBatchList
        {
            get
            {
                if (_FinishedProdOrderBatchList == null)
                    _FinishedProdOrderBatchList = new List<VD.ProdOrderPartslistPos>();
                return _FinishedProdOrderBatchList;
            }
            protected set
            {
                _FinishedProdOrderBatchList = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Properties -> Wizard

        #region Properties -> Wizard -> Basic

        private NewScheduledBatchWizardPhaseEnum _WizardPhase;
        [ACPropertyInfo(508, nameof(WizardPhase), "en{'WizardPhase'}de{'WizardPhase'}")]
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
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(WizardPhaseTitle));
                }
            }
        }

        private ACValueItemList wizardPhaseTitleList;

        [ACPropertyInfo(509, nameof(WizardPhaseTitle), "en{'Wizard Phase'}de{'Wizard Phase'}")]

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
            wizardPhaseTitleList.AddEntry((short)NewScheduledBatchWizardPhaseEnum.DeleteBatchPlan, "en{'Delete batch plans'}de{'Batchplan löschen'}");
            return wizardPhaseTitleList;
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private bool _IsWizard;
        [ACPropertySelected(999, nameof(IsWizard), "en{'TODO:IsWizard'}de{'TODO:IsWizard'}")]
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
                    OnPropertyChanged();
                }
            }
        }

        [ACPropertyInfo(515, nameof(CurrentLayout))]
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
        [ACPropertyInfo(999, nameof(WizardPhaseErrorMessage), "en{'Message'}de{'Meldung'}")]
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
                    OnPropertyChanged();
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
        [ACPropertySelected(9999, nameof(WizardSchedulerPartslist), "en{'TODO: WizardSchedulerPartslist'}de{'TODO: WizardSchedulerPartslist'}")]
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
                LocalSaveChanges();
                foreach (WizardSchedulerPartslist wizardItem in AllWizardSchedulerPartslistList)
                {
                    if (!IsBSOTemplateScheduleParent && wizardItem.SelectedMDSchedulingGroup != null)
                        RefreshServerState(wizardItem.SelectedMDSchedulingGroup.MDSchedulingGroupID);
                }
                OnWizardListChange();
            }
        }

        public void Item_PropertyChanged_Common()
        {

        }

        /// <summary>
        /// List property for WizardSchedulerPartslist
        /// </summary>
        /// <value>The WizardSchedulerPartslist list</value>
        [ACPropertyList(9999, nameof(WizardSchedulerPartslist))]
        public List<WizardSchedulerPartslist> WizardSchedulerPartslistList
        {
            get
            {
                return AllWizardSchedulerPartslistList
                    .Where(c => !c.IsSolved)
                    .OrderByDescending(c => c.Sn)
                    .ToList();
            }
        }

        #endregion

        #region Properties -> Wizard -> PartListExpand

        private VD.PartslistExpand rootPartslistExpand;
        private VD.PartslistExpand _CurrentPartListExpand;
        /// <summary>
        /// 
        /// </summary>
        [ACPropertyCurrent(606, nameof(VD.PartslistExpand))]
        public VD.PartslistExpand CurrentPartListExpand
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
                    OnPropertyChanged();
                }
            }
        }

        private List<VD.PartslistExpand> _PartListExpandList;
        [ACPropertyList(607, nameof(VD.PartslistExpand))]
        public List<VD.PartslistExpand> PartListExpandList
        {
            get
            {
                return _PartListExpandList;
            }
        }

        #endregion

        #region Properties -> Wizard ->   BatchPlanSuggestion
        public const string FilterBatchplanSuggestionMode = "FilterBatchplanSuggestionMode";
        [ACPropertySelected(516, nameof(FilterBatchplanSuggestionMode), "en{'Calculation formula'}de{'Berechnungsformel'}")]
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
                OnPropertyChanged();
            }
        }

        private ACValueItemList _FilterBatchplanSuggestionModeList;
        [ACPropertyList(517, nameof(FilterBatchplanSuggestionMode))]
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

        [ACMethodInfo(nameof(RecalculateBatchSuggestion), "en{'Calculate'}de{'Berechnung'}", 999)]
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
                            || !SelectedWizardSchedulerPartslist.BatchPlanSuggestion.ItemsList.Any(c => c.ProdOrderBatchPlan != null && c.ProdOrderBatchPlan.PlanStateIndex >= (short)VD.GlobalApp.BatchPlanState.AutoStart)
                       );
        }

        [ACMethodInfo(nameof(AddSuggestion), "en{'Add'}de{'Neu'}", 999)]
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
                        SelectedWizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderPartslist.MDProdOrderState.ProdOrderState >= VD.MDProdOrderState.ProdOrderStates.ProdFinished
                        || SelectedWizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.MDProdOrderState.ProdOrderState >= VD.MDProdOrderState.ProdOrderStates.ProdFinished
                    ));
        }

        [ACMethodInfo(nameof(RemoveSuggestion), "en{'Delete'}de{'Löschen'}", 999)]
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
        [ACPropertyList(519, nameof(BatchPlanTimelineItemsRoot))]
        public IEnumerable<BatchPlanTimelineItem> BatchPlanTimelineItemsRoot
        {
            get
            {
                return _BatchPlanTimelineItemsRoot;
            }
            set
            {
                _BatchPlanTimelineItemsRoot = value;
                OnPropertyChanged();
            }
        }

        private BatchPlanTimelineItem _SelectedBatchPlanTimelineItemRoot;
        /// <summary>
        /// Gets or sets the selected PropertyLog root. (Selected in treeListView control.)
        /// </summary>
        [ACPropertyCurrent(520, nameof(BatchPlanTimelineItemsRoot))]
        public BatchPlanTimelineItem SelectedBatchPlanTimelineItemRoot
        {
            get
            {
                return _SelectedBatchPlanTimelineItemRoot;
            }
            set
            {
                _SelectedBatchPlanTimelineItemRoot = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<BatchPlanTimelineItem> _BatchPlanTimelineItems;
        /// <summary>
        /// Gets or sets the ACPropertyLogs. Represents the collection for timeline view control. Contains all timeline items(ACPropertyLogModel).
        /// </summary>
        [ACPropertyList(521, nameof(BatchPlanTimelineItems))]
        public ObservableCollection<BatchPlanTimelineItem> BatchPlanTimelineItems
        {
            get
            {

                return _BatchPlanTimelineItems;
            }
            set
            {
                _BatchPlanTimelineItems = value;
                OnPropertyChanged();
            }
        }

        private BatchPlanTimelineItem _SelectedBatchPlanTimelineItem;
        /// <summary>
        /// Gets or sets the seelcted PropertyLog. (Selected in the timeline view control)
        /// </summary>
        [ACPropertyCurrent(522, nameof(BatchPlanTimelineItems))]
        public BatchPlanTimelineItem SelectedBatchPlanTimelineItem
        {
            get
            {
                return _SelectedBatchPlanTimelineItem;
            }
            set
            {
                _SelectedBatchPlanTimelineItem = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Properties -> MDBatchPlanGroup


        #region MDBatchPlanGroup
        public const string PropertyGroupName = "PropertyGroupName";
        private VD.MDBatchPlanGroup _SelectedMDBatchPlanGroup;
        /// <summary>
        /// Selected property for MDBatchPlanGroup
        /// </summary>
        /// <value>The selected MDBatchPlanGroup</value>
        [ACPropertySelected(9999, nameof(PropertyGroupName), "en{'TODO: MDBatchPlanGroup'}de{'TODO: MDBatchPlanGroup'}")]
        public VD.MDBatchPlanGroup SelectedMDBatchPlanGroup
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
                    OnPropertyChanged();
                }
            }
        }


        private List<VD.MDBatchPlanGroup> _MDBatchPlanGroupList;
        /// <summary>
        /// List property for MDBatchPlanGroup
        /// </summary>
        /// <value>The MDBatchPlanGroup list</value>
        [ACPropertyList(9999, nameof(PropertyGroupName))]
        public List<VD.MDBatchPlanGroup> MDBatchPlanGroupList
        {
            get
            {
                if (_MDBatchPlanGroupList == null)
                    _MDBatchPlanGroupList = LoadMDBatchPlanGroupList();
                return _MDBatchPlanGroupList;
            }
        }

        private List<VD.MDBatchPlanGroup> LoadMDBatchPlanGroupList()
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

        protected override string OnGetWorkflowSchedulerBaseRules()
        {
            return BSOBatchPlanSchedulerRules;
        }

        #endregion

        #region Properties => CalculatedRoutes

        private string _CalculateRouteResult;
        [ACPropertyInfo(9999, "", "")]
        public string CalculateRouteResult
        {
            get => _CalculateRouteResult;
            set
            {
                _CalculateRouteResult = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Methods -> ACMethod

        protected override void HandleRefreshServerStateOnPostSave()
        {
            if (SelectedScheduleForPWNode != null && ParentACObject != null && ParentACObject.ACType.ACIdentifier != BSOTemplateSchedule.ClassName)
            {
                RefreshServerState(SelectedScheduleForPWNode);
            }
        }

        public override bool LocalSaveChanges()
        {
            Msg msg = null;
            using (ACMonitor.Lock(DatabaseApp.QueryLock_1X000))
            {
                msg = CheckForInappropriateComponentQuantityOccurrence();
            }
            bool isPowerUser = HasUserRoleOfPlanner;
            if (msg != null)
            {
                if (isPowerUser)
                {
                    Messages.Msg(msg);
                }
                else
                {
                    SendMessage(msg);
                }
                Messages.LogMessageMsg(msg);
                ProdOrderManager.OnNewAlarmOccurred(ProdOrderManager.IsProdOrderManagerAlarm, msg);
            }
            using (ACMonitor.Lock(DatabaseApp.QueryLock_1X000))
            {
                msg = DatabaseApp.ACSaveChanges();
            }
            if (msg != null)
            {
                if (isPowerUser)
                {
                    Messages.Msg(msg);
                }
                else
                {
                    SendMessage(msg);
                }
                Messages.LogMessageMsg(msg);
            }
            return msg == null || msg.IsSucceded();
        }


        public Msg CheckForInappropriateComponentQuantityOccurrence()
        {
            Msg msg = null;
            IEnumerable<ObjectStateEntry> modifiedEntities = DatabaseApp.ObjectStateManager.GetObjectStateEntries(EntityState.Modified);
            IEnumerable<ObjectStateEntry> modifiedPosEntities = modifiedEntities.Where(c => c.EntitySet.Name == nameof(VD.ProdOrderPartslistPos));
            if (modifiedPosEntities.Any())
            {
                IEnumerable<VD.ProdOrderPartslistPos> modifiedPositions = modifiedPosEntities.Select(c => c.Entity as VD.ProdOrderPartslistPos).Where(c => c.MaterialPosTypeIndex == (short)VD.GlobalApp.MaterialPosTypes.OutwardRoot);
                if (modifiedPositions.Any())
                {
                    foreach (VD.ProdOrderPartslistPos modifiedPos in modifiedPositions)
                    {
                        ObjectStateEntry objectStateEntry = DatabaseApp.ObjectStateManager.GetObjectStateEntry(modifiedPos.EntityKey);
                        if (
                                objectStateEntry != null

                                && objectStateEntry.OriginalValues != null
                                && objectStateEntry.OriginalValues.FieldCount > 0
                                && objectStateEntry.OriginalValues[nameof(VD.ProdOrderPartslistPos.TargetQuantityUOM)] != null

                                && objectStateEntry.CurrentValues != null
                                && objectStateEntry.CurrentValues.FieldCount > 0
                                && objectStateEntry.CurrentValues[nameof(VD.ProdOrderPartslistPos.TargetQuantityUOM)] != null
                            )
                        {
                            double oldValue = (double)objectStateEntry.OriginalValues[nameof(VD.ProdOrderPartslistPos.TargetQuantityUOM)];
                            double newValue = (double)objectStateEntry.CurrentValues[nameof(VD.ProdOrderPartslistPos.TargetQuantityUOM)];
                            if (VD.InappropriateComponentQuantityOccurrence.IsForAnalyse(oldValue, newValue))
                            {
                                if (VD.InappropriateComponentQuantityOccurrence.IsInappropriate(modifiedPos))
                                {
                                    msg = new Msg(this, eMsgLevel.Warning, GetACUrl(), nameof(OnPreSave), 2558, "Warning50055");
                                    //ProdOrderManager.OnNewAlarmOccurred(ProdOrderManager.IsProdOrderManagerAlarm, msg);
                                    VD.InappropriateComponentQuantityOccurrence.WriteStackTrace(modifiedPos);
                                }
                            }
                        }
                    }
                }
            }
            return msg;
        }


        #endregion

        #region Methods -> Explorer

        #region Methods -> Explorer -> MoveToOtherLine

        [ACMethodInteraction(nameof(MoveToOtherLine), "en{'Move'}de{'Verlagern'}", (short)MISort.MovedData, false, "TargetScheduleForPWNode")]
        public void MoveToOtherLine()
        {
            if (!IsEnabledMoveToOtherLine())
                return;
            ClearMessages();
            bool isMovingValueValid = false;
            bool isMove = false;
            if (SelectedProdOrderBatchPlan.PlanMode == VD.BatchPlanMode.UseBatchCount)
            {
                int diffBatchCount = SelectedProdOrderBatchPlan.BatchTargetCount - SelectedProdOrderBatchPlan.BatchActualCount;
                // "Question50045" Please enter the number of batches you want to move to {0}:
                string header = Root.Environment.TranslateMessage(this, "Question50045", SelectedTargetScheduleForPWNode.MDSchedulingGroup.ACCaption);
                string enteredValue = Messages.InputBox(header, diffBatchCount.ToString());
                if (!string.IsNullOrEmpty(enteredValue))
                {
                    int moveBatchCount = BatchCountValidate(enteredValue, diffBatchCount);
                    isMovingValueValid = moveBatchCount > 0;
                    if (isMovingValueValid)
                    {
                        List<VD.ProdOrderBatchPlan> changedBatchPlans;
                        if (moveBatchCount == SelectedProdOrderBatchPlan.BatchTargetCount)
                        {
                            if (!MoveBatchToOtherProdLine(SelectedProdOrderBatchPlan, SelectedTargetScheduleForPWNode))
                                return;
                            isMove = true;
                            changedBatchPlans = new List<VD.ProdOrderBatchPlan>();
                            changedBatchPlans.Add(SelectedProdOrderBatchPlan);
                        }
                        else
                        {
                            int remainingBatchCount = SelectedProdOrderBatchPlan.BatchTargetCount - moveBatchCount;
                            VD.ProdOrderBatchPlan oldBatchPlan = SelectedProdOrderBatchPlan;
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
                            PAScheduleForPWNode targetLinie = SelectedTargetScheduleForPWNode;
                            VD.ProdOrderBatchPlan changedPlan = changedBatchPlans.FirstOrDefault();
                            SelectedScheduleForPWNode = targetLinie;
                            ChangeBatchPlanForSchedule(changedPlan, targetLinie);
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
            else if (SelectedProdOrderBatchPlan.PlanMode == VD.BatchPlanMode.UseTotalSize)
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
                        List<VD.ProdOrderBatchPlan> changedBatchPlans;
                        if (moveQuantity == SelectedProdOrderBatchPlan.BatchSize)
                        {
                            if (!MoveBatchToOtherProdLine(SelectedProdOrderBatchPlan, SelectedTargetScheduleForPWNode))
                                return;
                            isMove = true;
                            changedBatchPlans = new List<VD.ProdOrderBatchPlan>();
                            changedBatchPlans.Add(SelectedProdOrderBatchPlan);
                        }
                        else
                        {
                            double remainingBatchSize = SelectedProdOrderBatchPlan.BatchSize - moveQuantity;
                            VD.ProdOrderBatchPlan oldBatchPlan = SelectedProdOrderBatchPlan;
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
                            VD.ProdOrderBatchPlan changedPlan = changedBatchPlans.FirstOrDefault();
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
                bool isSaveValid = LocalSaveChanges();
                if (isSaveValid)
                {
                    // Correct the SortOrder from ProdOrderBatchPlan
                    ProdOrderBatchPlanList = GetProdOrderBatchPlanList(SelectedScheduleForPWNode?.MDSchedulingGroupID);
                    if (ProdOrderBatchPlanList != null && ProdOrderBatchPlanList.Any() && isMove)
                    {
                        MoveBatchSortOrderCorrect(ProdOrderBatchPlanList);
                        isSaveValid = LocalSaveChanges();
                        if (!isSaveValid)
                        {
                            DatabaseApp.ACUndoChanges();
                        }
                    }
                }
                else
                {
                    DatabaseApp.ACUndoChanges();
                }
                if (isSaveValid)
                {
                    if (!IsBSOTemplateScheduleParent)
                    {
                        RefreshServerState(SelectedScheduleForPWNode.MDSchedulingGroupID);
                    }
                    SelectedProdOrderBatchPlan = ProdOrderBatchPlanList.FirstOrDefault();
                    OnPropertyChanged(nameof(ProdOrderBatchPlanList));
                }
            }
        }


        public bool IsEnabledMoveToOtherLine()
        {
            return
                    SelectedProdOrderBatchPlan != null
                    && SelectedProdOrderBatchPlan.ProdOrderPartslist != null
                    && SelectedProdOrderBatchPlan.PlanState < VD.GlobalApp.BatchPlanState.Completed
                    && (
                           (SelectedProdOrderBatchPlan.PlanMode == VD.BatchPlanMode.UseBatchCount && SelectedProdOrderBatchPlan.BatchActualCount < SelectedProdOrderBatchPlan.BatchTargetCount)
                            || (SelectedProdOrderBatchPlan.PlanMode == VD.BatchPlanMode.UseTotalSize && SelectedProdOrderBatchPlan.RemainingQuantity > 0.00001)
                    )
                && SelectedTargetScheduleForPWNode != null
                && SelectedTargetScheduleForPWNode.MDSchedulingGroup != null;
        }

        #region Methods -> Explorer -> MoveToOtherLine -> Helper methods


        private bool MoveBatchToOtherProdLine(VD.ProdOrderBatchPlan prodOrderBatchPlan, PAScheduleForPWNode selectedTargetScheduleForPWNode)
        {
            if (prodOrderBatchPlan.VBiACClassWF == null)
                return false;
            VD.ACClassWF tempACClassWFItem = selectedTargetScheduleForPWNode.MDSchedulingGroup.MDSchedulingGroupWF_MDSchedulingGroup
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

        private void CreateNewBatchWithCount(VD.ProdOrderBatchPlan prodOrderBatchPlan, int moveBatchCount, PAScheduleForPWNode selectedTargetScheduleForPWNode, out List<VD.ProdOrderBatchPlan> generatedBatchPlans)
        {
            double totalSize = moveBatchCount * prodOrderBatchPlan.BatchSize;
            int sn = 1;
            WizardSchedulerPartslist wizardSchedulerPartslist =
                new WizardSchedulerPartslist(
                    DatabaseApp,
                    ProdOrderManager,
                    LocalBSOBatchPlan.VarioConfigManager,
                    RoundingQuantity,
                    prodOrderBatchPlan.ProdOrderPartslist.Partslist,
                    totalSize,
                    sn,
                    SelectedFilterBatchPlanGroup,
                    new List<VD.MDSchedulingGroup>() { selectedTargetScheduleForPWNode.MDSchedulingGroup });
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

        private void CreateNewBatchWithSize(VD.ProdOrderBatchPlan prodOrderBatchPlan, double moveQuantity, PAScheduleForPWNode selectedTargetScheduleForPWNode, out List<VD.ProdOrderBatchPlan> generatedBatchPlans)
        {
            int sn = 1;
            WizardSchedulerPartslist wizardSchedulerPartslist =
                new WizardSchedulerPartslist(DatabaseApp,
                    ProdOrderManager,
                    LocalBSOBatchPlan.VarioConfigManager,
                    RoundingQuantity,
                    prodOrderBatchPlan.ProdOrderPartslist.Partslist,
                    moveQuantity,
                    sn,
                    SelectedFilterBatchPlanGroup,
                    new List<VD.MDSchedulingGroup>() { selectedTargetScheduleForPWNode.MDSchedulingGroup });
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

        private void MoveBatchSortOrderCorrect()
        {
            List<VD.ProdOrderBatchPlan> notSelected =
                   ProdOrderBatchPlanList
                   .Where(c => !c.IsSelected && c.EntityState != EntityState.Deleted && c.EntityState != EntityState.Detached)
                   .OrderBy(c => c.ScheduledOrder ?? 0)
                   .ThenBy(c => c.InsertDate)
                   .ToList();
            MoveBatchSortOrderCorrect(notSelected);
        }

        private void MoveBatchSortOrderCorrect(IEnumerable<VD.ProdOrderBatchPlan> prodOrderBatchPlans)
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
                Messages.Msg(msg);
                SendMessage(msg);
            }
            if (moveBatchCount <= 0 || moveBatchCount > batchTargetCount)
            {
                //Error50394.
                Msg msg = new Msg(this, eMsgLevel.Error, GetACUrl(), "MoveToOtherLine", 1847, "Error50394", batchTargetCount);
                Messages.Msg(msg);
                SendMessage(msg);
                moveBatchCount = 0;
            }
            return moveBatchCount;
        }

        private double BatchSizeValidate(string enteredValue, double targetQuantity, VD.Partslist partslist)
        {
            double moveQuantity = 0;
            if (!Double.TryParse(enteredValue, out moveQuantity))
            {
                // Error50395
                Msg msg = new Msg(this, eMsgLevel.Error, GetACUrl(), "MoveToOtherLine", 1885, "Error50395");
                Messages.Msg(msg);
                SendMessage(msg);
            }
            if (moveQuantity <= Double.Epsilon || moveQuantity > targetQuantity)
            {
                //Error50396
                Msg msg = new Msg(this, eMsgLevel.Error, GetACUrl(), "MoveToOtherLine", 1894, "Error50396", targetQuantity);
                Messages.Msg(msg);
                SendMessage(msg);
                moveQuantity = 0;
            }
            else
            {
                WizardSchedulerPartslist tmpWizardPl =
                    new WizardSchedulerPartslist(
                        DatabaseApp,
                        ProdOrderManager,
                        LocalBSOBatchPlan.VarioConfigManager,
                        RoundingQuantity,
                        partslist,
                        moveQuantity,
                        1,
                        SelectedFilterBatchPlanGroup,
                        new List<VD.MDSchedulingGroup>() { SelectedTargetScheduleForPWNode.MDSchedulingGroup });
                if (tmpWizardPl.SelectedMDSchedulingGroup != null)
                    tmpWizardPl.LoadConfiguration();
                if (tmpWizardPl.BatchSizeMin > 0)
                {
                    if (moveQuantity < tmpWizardPl.BatchSizeMin)
                    {
                        //Error50397
                        Msg msg = new Msg(this, eMsgLevel.Error, GetACUrl(), "MoveToOtherLine", 1908, "Error50397", tmpWizardPl.BatchSizeMin, moveQuantity);
                        Messages.Msg(msg);
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

        #region Methods -> (Tab)BatchPlanScheduler -> New, Delete, Search

        public override bool IsEnabledSearch()
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

        [ACMethodCommand("New", Const.New, (short)MISort.New, true)]
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

        [ACMethodInfo(nameof(DeleteBatch), "en{'Delete'}de{'Löschen'}", 503, true)]
        public void DeleteBatch()
        {
            VD.ProdOrderBatchPlan batchPlan = SelectedProdOrderBatchPlan;
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
                && SelectedProdOrderBatchPlan.PlanState == VD.GlobalApp.BatchPlanState.Created
                && SelectedProdOrderBatchPlan.BatchActualCount <= 0;
        }

        [ACMethodInfo(nameof(BatchPlanEdit), "en{'Edit'}de{'Bearbeiten'}", 503, true)]
        public void BatchPlanEdit()
        {
            if (!IsEnabledBatchPlanEdit())
                return;
            ChangeBatchPlan(SelectedProdOrderBatchPlan);
        }

        public bool IsEnabledBatchPlanEdit()
        {
            return SelectedProdOrderBatchPlan != null
                && SelectedProdOrderBatchPlan.PlanState == VD.GlobalApp.BatchPlanState.Created
                && SelectedProdOrderBatchPlan.BatchActualCount <= 0;
        }

        #endregion

        #region Methods -> (Tab)BatchPlanScheduler -> Interaction

        public override void ItemDrag(Dictionary<int, string> newOrder)
        {
            if (!IsEnabledItemDrag())
                return;
            Dictionary<int, Guid> revisitedNewOrder = newOrder.ToDictionary(key => key.Key, val => new Guid(val.Value));
            var batchPlanList = ProdOrderBatchPlanList.ToList();
            foreach (var item in revisitedNewOrder)
            {
                VD.ProdOrderBatchPlan prodOrderBatchPlan = batchPlanList.FirstOrDefault(c => c.ProdOrderBatchPlanID == item.Value);
                if (prodOrderBatchPlan != null && prodOrderBatchPlan.ScheduledOrder != item.Key)
                {
                    prodOrderBatchPlan.ScheduledOrder = item.Key;
                    prodOrderBatchPlan.OnEntityPropertyChanged("ScheduledOrder");
                }
            }
            OnPropertyChanged(nameof(ProdOrderBatchPlanList));
        }


        [ACMethodInteraction(nameof(NavigateToProdOrder), ConstApp.ShowProdOrder, 502, false, nameof(SelectedProdOrderBatchPlan))]
        public void NavigateToProdOrder()
        {
            if (!IsEnabledNavigateToProdOrder())
            {
                return;
            }
            ShowProdOrder(SelectedProdOrderBatchPlan.ProdOrderPartslist);
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

            VD.ProdOrderBatchPlan[] plans = ProdOrderBatchPlanList.ToArray();
            ScheduledOrderManager<VD.ProdOrderBatchPlan>.MoveUp(plans);
            ProdOrderBatchPlanList = new ObservableCollection<VD.ProdOrderBatchPlan>(plans.OrderBy(c => c.ScheduledOrder));
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

            VD.ProdOrderBatchPlan[] plans = ProdOrderBatchPlanList.ToArray();
            ScheduledOrderManager<VD.ProdOrderBatchPlan>.MoveDown(plans);
            ProdOrderBatchPlanList = new ObservableCollection<VD.ProdOrderBatchPlan>(plans.OrderBy(c => c.ScheduledOrder));
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
                    EntityName = VD.ProdOrderBatchPlan.ClassName
                });
                service.ShowDialogComponents(this, info);
            }
        }

        public bool IsEnabledShowComponents()
        {
            return SelectedProdOrderBatchPlan != null;
        }

        [ACMethodInteraction("ShowParslist", "en{'Show bill of material'}de{'Stückliste Anzeigen'}", 605, true, "SelectedProdOrderBatchPlan", Global.ACKinds.MSMethodPrePost)]
        public void ShowParslist()
        {
            double treeQuantityRatio = SelectedProdOrderBatchPlan.ProdOrderPartslist.TargetQuantity / SelectedProdOrderBatchPlan.ProdOrderPartslist.Partslist.TargetQuantityUOM;
            rootPartslistExpand = new VD.PartslistExpand(SelectedProdOrderBatchPlan.ProdOrderPartslist.Partslist, 1, treeQuantityRatio);
            rootPartslistExpand.IsChecked = true;
            rootPartslistExpand.LoadTree();

            if (rootPartslistExpand.Children == null || rootPartslistExpand.Children.Count == 0)
            {
                string partslistNo = rootPartslistExpand.PartslistNo;
                StartPartslistBSO(partslistNo);
            }
            else
            {
                _PartListExpandList = new List<VD.PartslistExpand>() { rootPartslistExpand };
                CurrentPartListExpand = rootPartslistExpand;
                ShowDialog(this, "ProdOrderPartslistExpandDlg");
            }
        }


        public bool IsEnabledShowParslist()
        {
            return SelectedProdOrderBatchPlan != null;
        }

        [ACMethodInfo(nameof(ShowPartslistOK), Const.Ok, 999)]
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
            var acClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(VD.Partslist));
            if (acClass != null && acClass.ManagingBSO != null)
                this.Root.RootPageWPF.StartBusinessobject(Const.BusinessobjectsACUrl + ACUrlHelper.Delimiter_Start + acClass.ManagingBSO.ACIdentifier, acMethod.ParameterValueList);
        }

        /// <summary>
        /// 
        /// </summary>
        [ACMethodInfo(nameof(ShowPreferredParameters), VD.ConstApp.PrefParam, 999)]
        public void ShowPreferredParameters()
        {
            if (!IsEnabledShowPreferredParameters())
                return;
            bool isParamDefined = BSOBatchPlanChild.Value.BSOPreferredParameters_Child.Value.ShowParamDialogResult(
                         SelectedProdOrderBatchPlan.IplusVBiACClassWF.ACClassWFID,
                         SelectedProdOrderBatchPlan.ProdOrderPartslist.PartslistID,
                         SelectedProdOrderBatchPlan.ProdOrderPartslist.ProdOrderPartslistID,
                         null);

            if (isParamDefined)
            {
                SelectedProdOrderBatchPlan.OnEntityPropertyChanged(nameof(VD.ProdOrderBatchPlan.PlanState));
            }
        }

        public bool IsEnabledShowPreferredParameters()
        {
            return
                BSOBatchPlanChild != null
                && BSOBatchPlanChild.Value != null
                && BSOBatchPlanChild.Value.BSOPreferredParameters_Child != null
                && BSOBatchPlanChild.Value.BSOPreferredParameters_Child.Value != null
                && SelectedProdOrderBatchPlan != null;
        }

        #endregion

        #region Methods -> (Tab)BatchPlanScheduler -> Scheduling

        [ACMethodInfo(nameof(BackwardScheduling), "en{'Backward scheduling'}de{'Rückwärtsterminierung'}", 506, true)]
        public void BackwardScheduling()
        {
            if (!IsEnabledBackwardScheduling()) return;
            ShowDialog(this, "DlgBackwardScheduling");
        }

        public bool IsEnabledBackwardScheduling()
        {
            return IsEnabledScheduling();
        }

        [ACMethodCommand("BackwardSchedulingOk", Const.Ok, 507, true)]
        public void BackwardSchedulingOk()
        {
            if (!IsEnabledBackwardScheduling() || !IsEnabledBackwardSchedulingOk()) return;
            CloseTopDialog();
            BackgroundWorker.RunWorkerAsync(BGWorkerMethod_DoBackwardScheduling);
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledBackwardSchedulingOk()
        {
            return !BackgroundWorker.IsBusy && ScheduledEndDate != null && IsEnabledScheduling();
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

        [ACMethodInfo(nameof(ForwardSchedulingOk), Const.Ok, 509)]
        public void ForwardSchedulingOk()
        {
            if (!IsEnabledForwardScheduling() || !IsEnabledForwardSchedulingOk()) return;
            CloseTopDialog();
            BackgroundWorker.RunWorkerAsync(BGWorkerMethod_DoForwardScheduling);
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledForwardSchedulingOk()
        {
            return !BackgroundWorker.IsBusy && ScheduledStartDate != null && IsEnabledScheduling();
        }

        [ACMethodInfo(nameof(SchedulingCancel), Const.Cancel, 510)]
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

        [ACMethodInfo(nameof(SchedulingCalculateAll), "en{'Calculate All'}de{'Calculate All'}", 511)]
        public void SchedulingCalculateAll()
        {
            if (BackgroundWorker.IsBusy) return;
            BackgroundWorker.RunWorkerAsync(BGWorkerMethod_DoCalculateAll);
            ShowDialog(this, DesignNameProgressBar);
        }

        /// <summary>
        /// Source Property: GenerateBatchPlans
        /// </summary>
        [ACMethodInfo(nameof(GenerateBatchPlans), "en{'Generate batch plans'}de{'Batchplan generieren'}", 999, true)]
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
                !BackgroundWorker.IsBusy
                && ProdOrderPartslistList != null
                && ProdOrderPartslistList.Any(c => c.IsSelected)
                && ProdOrderPartslistList.Where(c => c.IsSelected && IsEnabledForBatchPlan(c)).Any();
        }

        /// <summary>
        /// Source Property: GenerateBatchPlans
        /// </summary>
        [ACMethodInfo(nameof(MergeOrders), "en{'Merge prodorders'}de{'Auftrag zusammenführen'}", 999, true)]
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
                !BackgroundWorker.IsBusy
                && ProdOrderPartslistList != null
                && ProdOrderPartslistList.Any(c => c.IsSelected)
                && ProdOrderPartslistList.Where(c => c.IsSelected && IsEnabledForBatchPlan(c)).Any();
        }

        #endregion

        #region Methods -> (Tab)BatchPlanScheduler -> BatchState

        [ACMethodCommand(nameof(SetBatchStateReadyToStart), "en{'Switch to Readiness'}de{'Startbereit setzen'}", (short)MISort.Start, true)]
        public void SetBatchStateReadyToStart()
        {
            if (!IsEnabledSetBatchStateReadyToStart())
                return;
            VD.ProdOrderBatchPlan[] selectedBatches = ProdOrderBatchPlanList.Where(c => c.IsSelected
                                                                                    && (c.PlanState <= VD.GlobalApp.BatchPlanState.Created
                                                                                        || c.PlanState >= VD.GlobalApp.BatchPlanState.Paused))
                                                                         .ToArray();
            SetReadyToStart(selectedBatches);
        }

        public bool IsEnabledSetBatchStateReadyToStart()
        {
            return ProdOrderBatchPlanList != null
                && ProdOrderBatchPlanList.Any(c =>
                        c.IsSelected
                        && (c.ProdOrderPartslist != null && c.ProdOrderPartslist.Partslist.IsEnabled)
                        && (
                                c.PlanState <= VD.GlobalApp.BatchPlanState.Created
                                || c.PlanState >= VD.GlobalApp.BatchPlanState.Paused
                            )
                );
        }

        private void SetReadyToStart(VD.ProdOrderBatchPlan[] batchPlans)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();
            foreach (VD.ProdOrderBatchPlan batchPlan in batchPlans)
            {
                bool isBatchHaveFaciltiyReservation =
                    batchPlan.FacilityReservation_ProdOrderBatchPlan.Any();

                bool isPartslistEnabled =
                    (batchPlan.ProdOrderPartslist != null
                    && batchPlan.ProdOrderPartslist.Partslist.IsEnabled);

                if (!isBatchHaveFaciltiyReservation)
                {
                    // Error50559
                    // Unable to start batch plan: #{0} {1} {2} {3}x{4}! Destination not selected!
                    // Batchplan starten nicht möglich: #{0} {1} {2} {3}x{4}! Zeil nicht ausgewählt!
                    Msg msg = new Msg(this, eMsgLevel.Error, nameof(BSOBatchPlanScheduler), $"{nameof(SetBatchStateReadyToStart)}()", 2918, "Error50559",
                        batchPlan.ScheduledOrder, batchPlan.ProdOrderPartslistPos.Material.MaterialNo, batchPlan.ProdOrderPartslistPos.Material.MaterialName1,
                        batchPlan.BatchTargetCount, batchPlan.BatchSize);
                    msgWithDetails.AddDetailMessage(msg);
                }

                if (!isPartslistEnabled)
                {
                    // Error50653
                    // Unable to start batch plan #{0} {1} {2} {3}x{4}! Partslist is not enabled!
                    // Batchplan starten nicht möglich: #{0} {1} {2} {3}x{4}! Rezeptur nicht freigegeben!
                    Msg msg = new Msg(this, eMsgLevel.Error, nameof(BSOBatchPlanScheduler), $"{nameof(SetBatchStateReadyToStart)}()", 2931, "Error50653",
                        batchPlan.ScheduledOrder, batchPlan.ProdOrderPartslistPos.Material.MaterialNo, batchPlan.ProdOrderPartslistPos.Material.MaterialName1,
                        batchPlan.BatchTargetCount, batchPlan.BatchSize);
                    msgWithDetails.AddDetailMessage(msg);
                }

                // check HasRequiredParams
                bool? hasRequieredParams = BSOBatchPlanChild.Value.ValidatePreferredParams(batchPlan, msgWithDetails);

                bool isBatchReadyToStart = isBatchHaveFaciltiyReservation && isPartslistEnabled && (hasRequieredParams ?? true);

                if (ValidateBatchPlanBeforeStart)
                {
                    if (isBatchReadyToStart)
                    {
                        // check duplicate components
                        bool haveManyDuplicateComponents =
                            batchPlan
                            .ProdOrderPartslist
                            .ProdOrderPartslistPos_ProdOrderPartslist
                            .Where(c => c.MaterialPosTypeIndex == (short)VD.GlobalApp.MaterialPosTypes.OutwardRoot)
                            .GroupBy(c => c.Material.MaterialNo)
                            .Where(c => c.Count() > 1)
                            .Count() > 1;

                        if (haveManyDuplicateComponents)
                        {
                            // Warning50060
                            // Prodorder recipe [{0}] {1} have multiplied components with same material! Is this recipe correct?
                            // Auftrag Rezept [{0}] {1} hat mehrere Komponenten mit demselben Material! Ist dieses Rezept richtig?
                            Global.MsgResult msgResult = Messages.Question(this, "Warning50060", Global.MsgResult.No, false, batchPlan.ProdOrderPartslist.Partslist.PartslistNo, batchPlan.ProdOrderPartslist.Partslist.PartslistName);
                            isBatchReadyToStart = msgResult == Global.MsgResult.Yes;
                        }
                    }

                    if (isBatchReadyToStart)
                    {
                        bool notExpectedPosQuantities = NotExpectedPosQuantities(batchPlan);
                        if (notExpectedPosQuantities)
                        {
                            // Warning50061
                            // Prodorder recipe [{0}] {1} position quantities ratios have big differences from original recipe! Is this recipe correct?
                            // Prodorder-Rezept [{0}] {1} Mengenverhältnisse von Linien unterscheiden sich stark vom Originalrezept! Ist dieses Rezept richtig?
                            Global.MsgResult msgResult = Messages.Question(this, "Warning50061", Global.MsgResult.No, false, batchPlan.ProdOrderPartslist.Partslist.PartslistNo, batchPlan.ProdOrderPartslist.Partslist.PartslistName);
                            isBatchReadyToStart = msgResult == Global.MsgResult.Yes;
                        }
                    }

                    if (isBatchReadyToStart)
                    {
                        bool notExpectedComponentSum = NotExpectedComponentSum(batchPlan);
                        if (notExpectedComponentSum)
                        {
                            // Warning50062
                            // Prodorder recipe [{0}] {1} difference between component quantity sum and recipe quantity! Is this recipe correct?
                            // Prodorder Rezept [{0}] {1} Differenz zwischen Komponentenmengensumme und Rezeptmenge! Ist dieses Rezept richtig?
                            Global.MsgResult msgResult = Messages.Question(this, "Warning50062", Global.MsgResult.No, false, batchPlan.ProdOrderPartslist.Partslist.PartslistNo, batchPlan.ProdOrderPartslist.Partslist.PartslistName);
                            isBatchReadyToStart = msgResult == Global.MsgResult.Yes;
                        }
                    }
                }


                if (isBatchReadyToStart)
                {
                    batchPlan.PlanState = VD.GlobalApp.BatchPlanState.ReadyToStart;
                }
            }

            if (msgWithDetails.MsgDetails.Any())
            {
                Messages.Msg(msgWithDetails);
            }
            Save();
            OnPropertyChanged(nameof(ProdOrderBatchPlanList));
        }

        private bool NotExpectedPosQuantities(VD.ProdOrderBatchPlan batchPlan)
        {
            bool notExpectedPosQuantities = false;

            VD.Partslist pl = batchPlan.ProdOrderPartslist.Partslist;
            if (pl.TargetQuantityUOM <= double.Epsilon)
            {
                notExpectedPosQuantities = true;
            }
            else if (batchPlan.ProdOrderPartslist.TargetQuantity <= double.Epsilon)
            {
                notExpectedPosQuantities = true;
            }
            else
            {
                double plMinRatio = 0;
                double plMaxRatio = 0;
                VD.PartslistPos[] lines = pl.PartslistPos_Partslist.Where(c => c.MaterialPosTypeIndex == (short)VD.GlobalApp.MaterialPosTypes.OutwardRoot).ToArray();
                foreach (VD.PartslistPos pos in lines)
                {
                    double ratio = pos.TargetQuantityUOM / pl.TargetQuantityUOM;
                    if (ratio < plMinRatio || plMinRatio == 0)
                    {
                        plMinRatio = ratio;
                    }

                    if (ratio > plMaxRatio || plMaxRatio == 0)
                    {
                        plMaxRatio = ratio;
                    }
                }

                double prodPlMinRatio = 0;
                double prodPlMaxRatio = 0;
                VD.ProdOrderPartslistPos[] prodLines = batchPlan.ProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist.Where(c => c.MaterialPosTypeIndex == (short)VD.GlobalApp.MaterialPosTypes.OutwardRoot).ToArray();
                foreach (VD.ProdOrderPartslistPos prodLine in prodLines)
                {
                    double ratio = prodLine.TargetQuantityUOM / batchPlan.ProdOrderPartslist.TargetQuantity;
                    if (ratio < prodPlMinRatio || prodPlMinRatio == 0)
                    {
                        prodPlMinRatio = ratio;
                    }

                    if (ratio > prodPlMaxRatio || prodPlMaxRatio == 0)
                    {
                        prodPlMaxRatio = ratio;
                    }
                }

                notExpectedPosQuantities = (Math.Abs(plMinRatio - prodPlMinRatio) / prodPlMinRatio) >= 0.1;
                if (!notExpectedPosQuantities)
                {
                    notExpectedPosQuantities = (Math.Abs(plMaxRatio - prodPlMaxRatio) / prodPlMaxRatio) >= 0.1;
                }
            }

            return notExpectedPosQuantities;
        }

        private bool NotExpectedComponentSum(VD.ProdOrderBatchPlan batchPlan)
        {
            bool notExpectedComponentSum = false;
            double componentsSum =
                batchPlan
                .ProdOrderPartslist
                .ProdOrderPartslistPos_ProdOrderPartslist
                .Where(c => c.MaterialPosTypeIndex == (short)VD.GlobalApp.MaterialPosTypes.OutwardRoot && !c.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Where(x => x.TargetProdOrderPartslistPos.Material.ExcludeFromSumCalc).Any())
                .Select(c => c.TargetQuantityUOM)
                .DefaultIfEmpty()
                .Sum();

            if (batchPlan.ProdOrderPartslist.TargetQuantity <= double.Epsilon)
            {
                notExpectedComponentSum = true;
            }
            else
            {
                notExpectedComponentSum = (Math.Abs(componentsSum - batchPlan.ProdOrderPartslist.TargetQuantity) / batchPlan.ProdOrderPartslist.TargetQuantity) >= 0.1;
            }

            return notExpectedComponentSum;
        }

        [ACMethodCommand("SetBatchStateCreated", "en{'Reset Readiness'}de{'Startbereitschaft rücksetzen'}", 508, true)]
        public void SetBatchStateCreated()
        {
            if (!IsEnabledSetBatchStateCreated()) return;
            List<VD.ProdOrderBatchPlan> selectedBatches = ProdOrderBatchPlanList.Where(c => c.IsSelected).ToList();
            foreach (VD.ProdOrderBatchPlan batchPlan in selectedBatches)
            {
                if (batchPlan.PlanState == VD.GlobalApp.BatchPlanState.ReadyToStart
                    || batchPlan.PlanState == VD.GlobalApp.BatchPlanState.AutoStart)
                {
                    batchPlan.PlanState = VD.GlobalApp.BatchPlanState.Created;
                    if (batchPlan.PartialTargetCount.HasValue && batchPlan.PartialTargetCount.Value > 0)
                        batchPlan.PartialTargetCount = null;
                }
            }
            Save();
            OnPropertyChanged(nameof(ProdOrderBatchPlanList));
        }

        public bool IsEnabledSetBatchStateCreated()
        {
            return ProdOrderBatchPlanList != null && ProdOrderBatchPlanList.Any(c => c.IsSelected && (c.PlanState == VD.GlobalApp.BatchPlanState.ReadyToStart || c.PlanState == VD.GlobalApp.BatchPlanState.AutoStart));
        }

        public short[] NotAllowedStatesForBatchCancel
        {
            get
            {
                return new short[]
                {
                    (short)VD.GlobalApp.BatchPlanState.ReadyToStart,
                    (short)VD.GlobalApp.BatchPlanState.AutoStart,
                    (short)VD.GlobalApp.BatchPlanState.InProcess
                };
            }
        }

        private void DoSetBatchStateCancelled(bool autoDeleteDependingBatchPlans, List<VD.ProdOrderBatchPlan> selectedBatches, ref List<Guid> groupsForRefresh)
        {

            foreach (VD.ProdOrderBatchPlan batchPlan in selectedBatches)
            {
                batchPlan.AutoRefresh();
                batchPlan.FacilityReservation_ProdOrderBatchPlan.AutoRefresh();
            }

            List<MaintainOrderInfo> maintainOrderInfos = new List<MaintainOrderInfo>();

            foreach (VD.ProdOrderBatchPlan batchPlan in selectedBatches)
            {
                if (NotAllowedStatesForBatchCancel.Contains(batchPlan.PlanStateIndex))
                    return;

                var prodOrderPartslist = batchPlan.ProdOrderPartslist;
                MaintainOrderInfo maintainOrderInfo = maintainOrderInfos.Where(c => c.PO == prodOrderPartslist.ProdOrder).FirstOrDefault();
                if (maintainOrderInfo == null)
                {
                    maintainOrderInfo = new MaintainOrderInfo();
                    maintainOrderInfo.PO = prodOrderPartslist.ProdOrder;
                    maintainOrderInfos.Add(maintainOrderInfo);
                }

                if (batchPlan.PlanState >= VD.GlobalApp.BatchPlanState.Paused
                    || batchPlan.ProdOrderBatch_ProdOrderBatchPlan.Any())
                {
                    batchPlan.PlanState = VD.GlobalApp.BatchPlanState.Cancelled;
                    if (autoDeleteDependingBatchPlans)
                        maintainOrderInfo.DeactivateAll = true;
                    else
                    {
                        var mDProdOrderStateCancelled = VD.DatabaseApp.s_cQry_GetMDProdOrderState(DatabaseApp, VD.MDProdOrderState.ProdOrderStates.Cancelled).FirstOrDefault();
                        if (prodOrderPartslist.MDProdOrderState.ProdOrderState != mDProdOrderStateCancelled.ProdOrderState)
                            SetProdorderPartslistState(mDProdOrderStateCancelled, null, prodOrderPartslist);
                    }
                }
                else if (batchPlan.PlanState == VD.GlobalApp.BatchPlanState.Created)
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
                    prodOrderPartslist.ProdOrderBatchPlan_ProdOrderPartslist.Remove(batchPlan);
                    if (autoDeleteDependingBatchPlans && !prodOrderPartslist.ProdOrderBatchPlan_ProdOrderPartslist.Any())
                        maintainOrderInfo.RemoveAll = true;
                }
                else
                {
                    if (autoDeleteDependingBatchPlans)
                        maintainOrderInfo.DeactivateAll = true;
                }
            }

            MaintainProdOrderState(maintainOrderInfos);

            MoveBatchSortOrderCorrect();

            LocalSaveChanges();

        }

        private void DoRefreshLinesAfterBatchDelete(List<Guid> groupsForRefresh)
        {
            RefreshScheduleForSelectedNode();
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


        //[ACMethodCommand("SetBatchStateCancelled", "en{'Deactivate and remove'}de{'Deaktivieren und Entfernen'}", (short)MISort.Start, true)]
        //public void SetBatchStateCancelled()
        //{
        //    ClearMessages();
        //    if (!IsEnabledSetBatchStateCancelled())
        //        return;
        //    try
        //    {
        //        bool autoDeleteDependingBatchPlans = AutoRemoveMDSGroupFrom > 0
        //                                                && AutoRemoveMDSGroupTo > 0
        //                                                && SelectedScheduleForPWNode != null
        //                                                && SelectedScheduleForPWNode.MDSchedulingGroup != null
        //                                                && SelectedScheduleForPWNode.MDSchedulingGroup.MDSchedulingGroupIndex >= AutoRemoveMDSGroupFrom
        //                                                && SelectedScheduleForPWNode.MDSchedulingGroup.MDSchedulingGroupIndex <= AutoRemoveMDSGroupTo;

        //        if (Messages.Question(this, "Question50084", Global.MsgResult.Yes) == Global.MsgResult.Yes)
        //        {
        //            List<ProdOrderBatchPlan> selectedBatches = ProdOrderBatchPlanList.Where(c => c.IsSelected).ToList();
        //            List<Guid> groupsForRefresh = new List<Guid>();
        //            DoSetBatchStateCancelled(autoDeleteDependingBatchPlans, selectedBatches, ref groupsForRefresh);
        //            DoRefreshLinesAfterBatchDelete(groupsForRefresh);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = "";
        //        Exception tmpEx = ex;
        //        while (tmpEx != null)
        //        {
        //            msg += tmpEx.Message;
        //            tmpEx = tmpEx.InnerException;
        //        }

        //        Msg errMsg = new Msg() { MessageLevel = eMsgLevel.Error, ACIdentifier = nameof(SetBatchStateCancelled), Message = msg };
        //        SendMessage(errMsg);
        //    }
        //}

        //public bool IsEnabledSetBatchStateCancelled()
        //{
        //    return
        //        ProdOrderBatchPlanList != null
        //        && ProdOrderBatchPlanList.Any(c => c.IsSelected)
        //        && !ProdOrderBatchPlanList.Where(c => c.IsSelected).Any(c =>
        //              NotAllowedStatesForBatchCancel.Contains(c.PlanStateIndex)
        //        );
        //}

        /// <summary>
        /// Method for new behavior deleting batch plans
        /// for test
        /// </summary>
        [ACMethodCommand("SetBatchStateCancelled", "en{'Deactivate and remove'}de{'Deaktivieren und Entfernen'}", (short)MISort.Start, true)]
        public void SetBatchStateCancelled()
        {
            ClearMessages();
            if (!IsEnabledSetBatchStateCancelled())
                return;
            if (Messages.Question(this, "Question50084", Global.MsgResult.Yes) == Global.MsgResult.Yes)
            {
                try
                {
                    List<Guid> groupsForRefresh = new List<Guid>();

                    List<VD.ProdOrderBatchPlan> selectedBatches = ProdOrderBatchPlanList.Where(c => c.IsSelected).ToList();
                    Dictionary<VD.ProdOrderPartslistPos, double> positions =
                        selectedBatches
                        .ToDictionary(key => key.ProdOrderPartslistPos, val => val.BatchTargetCount * val.BatchSize);

                    bool isSetBatchStateCancelledForTreeDelete = IsSetBatchStateCancelledForTreeDelete();
                    if (isSetBatchStateCancelledForTreeDelete)
                    {
                        Global.MsgResult answer = Messages.YesNoCancel(this, "Question50093");
                        if (answer == Global.MsgResult.Yes)
                        {
                            // Silent delete batch plans for current ProdOrder
                            OnSetBatchStateCanceling(positions);
                            DoSetBatchStateCancelled(true, selectedBatches, ref groupsForRefresh);
                            DoRefreshLinesAfterBatchDelete(groupsForRefresh);
                        }
                        else if (answer == Global.MsgResult.No)
                        {
                            // Wizard delete batch plans for current ProdOrder
                            if (SelectedProdOrderBatchPlan != null)
                            {
                                IsWizard = true;
                                WizardPhase = NewScheduledBatchWizardPhaseEnum.DeleteBatchPlan;
                                WizardForwardAction(wizardPhase: NewScheduledBatchWizardPhaseEnum.DeleteBatchPlan);
                                OnPropertyChanged(nameof(CurrentLayout));
                                OnWizardListChange();
                            }
                        }
                        else
                        {
                            // do nothing
                        }
                    }
                    else
                    {
                        // delete batches as usuall (on current linie)
                        bool autoDeleteDependingBatchPlans = AutoRemoveMDSGroupFrom > 0
                                                        && AutoRemoveMDSGroupTo > 0
                                                        && SelectedScheduleForPWNode != null
                                                        && SelectedScheduleForPWNode.MDSchedulingGroup != null
                                                        && SelectedScheduleForPWNode.MDSchedulingGroup.MDSchedulingGroupIndex >= AutoRemoveMDSGroupFrom
                                                        && SelectedScheduleForPWNode.MDSchedulingGroup.MDSchedulingGroupIndex <= AutoRemoveMDSGroupTo;
                        OnSetBatchStateCanceling(positions);
                        DoSetBatchStateCancelled(autoDeleteDependingBatchPlans, selectedBatches, ref groupsForRefresh);
                        DoRefreshLinesAfterBatchDelete(groupsForRefresh);
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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="positions">
        /// Dictionary with pos canceled batch plans + batch plan quantity
        /// </param>
        public virtual void OnSetBatchStateCanceling(Dictionary<VD.ProdOrderPartslistPos, double> positions)
        {

        }

        private bool IsSetBatchStateCancelledForTreeDelete()
        {
            return
                SelectedProdOrderBatchPlan
                .ProdOrderPartslist
                .ProdOrder
                .ProdOrderPartslist_ProdOrder
                .SelectMany(c => c.ProdOrderBatchPlan_ProdOrderPartslist)
                .AsEnumerable()
                .Where(c =>
                            !c.IsSelected
                            && c.ProdOrderBatchPlanID != SelectedProdOrderBatchPlan.ProdOrderBatchPlanID
                            && !NotAllowedStatesForBatchCancel.Contains(c.PlanStateIndex)
                      )
                .Any();
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
        [ACMethodInfo(nameof(SearchOrders), "en{'Search for orders'}de{'Aufträge suchen'}", 999)]
        public void SearchOrders()
        {
            if (!IsEnabledSearchOrders())
                return;
            BackgroundWorker.RunWorkerAsync(nameof(DoSearchOrders));
            ShowDialog(this, DesignNameProgressBar);
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

        [ACMethodInfo(nameof(SearchOrdersAll), "en{'Search batches'}de{'Batche suchen'}", 999)]
        public void SearchOrdersAll()
        {
            if (!IsEnabledSearchOrdersAll())
                return;
            BackgroundWorker.RunWorkerAsync(nameof(DoSearchOrdersAll));
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledSearchOrdersAll()
        {
            return IsEnabledSearchOrders();
        }


        [ACMethodInteraction(nameof(NavigateToProdOrder2), ConstApp.ShowProdOrder, 502, false, nameof(SelectedProdOrderPartslist))]
        public void NavigateToProdOrder2()
        {
            if (!IsEnabledNavigateToProdOrder2())
            {
                return;
            }
            ShowProdOrder(SelectedProdOrderPartslist.ProdOrderPartslist);
        }

        public bool IsEnabledNavigateToProdOrder2()
        {
            return SelectedProdOrderPartslist != null && SelectedProdOrderPartslist.ProdOrderPartslist != null;
        }


        [ACMethodInteraction(nameof(NavigateToProdOrder3), ConstApp.ShowProdOrder, 908, false, nameof(SelectedFinishedProdOrderBatch))]
        public void NavigateToProdOrder3()
        {
            if (!IsEnabledNavigateToProdOrder3())
            {
                return;
            }
            ShowProdOrder(SelectedFinishedProdOrderBatch.ProdOrderPartslist);
        }

        public bool IsEnabledNavigateToProdOrder3()
        {
            return SelectedFinishedProdOrderBatch != null;
        }

        private void SetProdOrderTimeFilterFromDay(ACValueItem aCValueItem)
        {
            if(aCValueItem == null)
            {
                FilterOrderStartTime = null;
                FilterOrderEndTime = null;
            }
            else
            {
                DayOfWeek dayOfWeek = (DayOfWeek)((short)aCValueItem.Value);
                DateTime targetDay = DateTime.Now.Date;
                for(int i = 0; i < 7; i ++)
                {
                    if(targetDay.DayOfWeek == dayOfWeek)
                    {
                        break;
                    }
                    targetDay = targetDay.AddDays(1);
                }

                FilterOrderStartTime = targetDay;
                FilterOrderEndTime = targetDay.AddDays(1);
            }
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
                VD.ProdOrderPartslist prodOrderPartslist = SelectedProdOrderPartslist.ProdOrderPartslist;
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
                    VD.MDSchedulingGroup schedulingGroup = schedulingGroups.FirstOrDefault(c => c.MDSchedulingGroupID == SelectedScheduleForPWNode.MDSchedulingGroupID);
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
                && plWrapper.ProdOrderPartslist.MDProdOrderState.ProdOrderState < VD.MDProdOrderState.ProdOrderStates.ProdFinished
                && plWrapper.ProdOrderPartslist.ProdOrder != null
                && plWrapper.ProdOrderPartslist.ProdOrder.MDProdOrderState != null
                && plWrapper.ProdOrderPartslist.ProdOrder.MDProdOrderState.ProdOrderState < VD.MDProdOrderState.ProdOrderStates.ProdFinished;
            // && plWrapper.UnPlannedQuantityUOM > Double.Epsilon;
        }

        [ACMethodCommand("RemoveSelectedProdorderPartslist", "en{'Deactivate and remove'}de{'Deaktivieren und Entfernen'}", (short)MISort.Start)]
        public void RemoveSelectedProdorderPartslist()
        {
            ClearMessages();
            if (!IsEnabledRemoveSelectedProdorderPartslist())
                return;

            List<Guid> groupsForRefresh = new List<Guid>();

            VD.MDProdOrderState mDProdOrderStateCancelled = VD.DatabaseApp.s_cQry_GetMDProdOrderState(DatabaseApp, VD.MDProdOrderState.ProdOrderStates.Cancelled).FirstOrDefault();
            VD.MDProdOrderState mDProdOrderStateCompleted = VD.DatabaseApp.s_cQry_GetMDProdOrderState(DatabaseApp, VD.MDProdOrderState.ProdOrderStates.ProdFinished).FirstOrDefault();
            VD.ProdOrderPartslist[] plForRemove = ProdOrderPartslistList.Where(c => c.IsSelected).Select(c => c.ProdOrderPartslist).ToArray();

            bool autoDeleteDependingBatchPlans = AutoRemoveMDSGroupFrom > 0
                                                && AutoRemoveMDSGroupTo > 0
                                                && SelectedScheduleForPWNode != null
                                                && SelectedScheduleForPWNode.MDSchedulingGroup != null
                                                && SelectedScheduleForPWNode.MDSchedulingGroup.MDSchedulingGroupIndex >= AutoRemoveMDSGroupFrom
                                                && SelectedScheduleForPWNode.MDSchedulingGroup.MDSchedulingGroupIndex <= AutoRemoveMDSGroupTo;

            List<MaintainOrderInfo> prodOrders = new List<MaintainOrderInfo>();

            List<VD.ProdOrderPartslist> allForRemove = new List<VD.ProdOrderPartslist>();


            foreach (VD.ProdOrderPartslist partslist in plForRemove)
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
                bool hasAnyActivePlans = partslist.ProdOrderBatchPlan_ProdOrderPartslist.Where(c => c.PlanStateIndex > (short)VD.GlobalApp.BatchPlanState.Created).Any();
                if (autoDeleteDependingBatchPlans)
                {
                    if (hasAnyActivePlans)
                        mOrderInfo.DeactivateAll = true;
                    else
                        mOrderInfo.RemoveAll = true;
                }

                foreach (var batchPlan2Remove in partslist.ProdOrderBatchPlan_ProdOrderPartslist.Where(c => c.PlanStateIndex == (short)VD.GlobalApp.BatchPlanState.Created).ToArray())
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
                    if (!partslist.ProdOrderBatchPlan_ProdOrderPartslist.Any(c => c.PlanStateIndex >= (short)VD.GlobalApp.BatchPlanState.ReadyToStart && c.PlanStateIndex <= (short)VD.GlobalApp.BatchPlanState.InProcess))
                    {
                        SetProdorderPartslistState(mDProdOrderStateCancelled, mDProdOrderStateCompleted, partslist);
                    }
                }
            }

            MaintainProdOrderState(prodOrders);

            LocalSaveChanges();
            RefreshScheduleForSelectedNode();
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
            public VD.ProdOrder PO { get; set; }
            public bool RemoveAll { get; set; }
            public bool DeactivateAll { get; set; }
        }

        protected void MaintainProdOrderState(List<MaintainOrderInfo> prodOrders)
        {
            VD.MDProdOrderState mDProdOrderStateCompleted = VD.DatabaseApp.s_cQry_GetMDProdOrderState(DatabaseApp, VD.MDProdOrderState.ProdOrderStates.ProdFinished).FirstOrDefault();
            VD.MDProdOrderState mDProdOrderStateCancelled = VD.DatabaseApp.s_cQry_GetMDProdOrderState(DatabaseApp, VD.MDProdOrderState.ProdOrderStates.Cancelled).FirstOrDefault();

            foreach (MaintainOrderInfo poInfo in prodOrders)
            {

                poInfo.PO.AutoRefresh();
                VD.ProdOrderPartslist[] allProdPartslists = poInfo.PO.ProdOrderPartslist_ProdOrder.ToArray();
                bool canRemoveAll = true;

                if (poInfo.RemoveAll || poInfo.DeactivateAll)
                {
                    foreach (VD.ProdOrderBatchPlan batchPlan2Remove in poInfo.PO.ProdOrderPartslist_ProdOrder.SelectMany(c => c.ProdOrderBatchPlan_ProdOrderPartslist).ToArray())
                    {
                        var parentPOList = batchPlan2Remove.ProdOrderPartslist;
                        if (batchPlan2Remove.PlanState == VD.GlobalApp.BatchPlanState.Created)
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
                                if (batchPlan2Remove.PlanState < VD.GlobalApp.BatchPlanState.ReadyToStart || batchPlan2Remove.PlanState > VD.GlobalApp.BatchPlanState.InProcess)
                                    batchPlan2Remove.PlanState = VD.GlobalApp.BatchPlanState.Cancelled;
                            }
                            canRemoveAll = false;
                        }
                    }
                }
                else
                {
                    canRemoveAll = !poInfo.PO.ProdOrderPartslist_ProdOrder.SelectMany(c => c.ProdOrderBatchPlan_ProdOrderPartslist).Any();
                }


                if (canRemoveAll && string.IsNullOrEmpty(poInfo.PO.KeyOfExtSys))
                {
                    foreach (VD.ProdOrderPartslist pl in allProdPartslists)
                    {
                        RemovePartslist(pl, mDProdOrderStateCancelled, mDProdOrderStateCompleted);
                    }
                }
                else if (!IsBSOTemplateScheduleParent || poInfo.DeactivateAll)
                {
                    if (
                            (
                                poInfo.PO.ProdOrderPartslist_ProdOrder.SelectMany(c => c.ProdOrderBatchPlan_ProdOrderPartslist).Any()
                                && !poInfo.PO.ProdOrderPartslist_ProdOrder.SelectMany(c => c.ProdOrderBatchPlan_ProdOrderPartslist).Any(c => c.PlanStateIndex < (short)VD.GlobalApp.BatchPlanState.Completed)
                            )
                            || poInfo.DeactivateAll
                        )
                    {
                        foreach (VD.ProdOrderPartslist pl in allProdPartslists)
                            SetProdorderPartslistState(mDProdOrderStateCancelled, mDProdOrderStateCompleted, pl);
                    }
                    if (!poInfo.PO.ProdOrderPartslist_ProdOrder.Any(c => c.MDProdOrderState.MDProdOrderStateIndex < (short)(short)VD.GlobalApp.BatchPlanState.Completed))
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

        [ACMethodInfo(nameof(WizardBackward), "en{'Back'}de{'Zurück'}", 510)]
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

        [ACMethodInfo(nameof(WizardForward), "en{'Forward'}de{'Weiter'}", 509)]
        public virtual void WizardForward()
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
                SelectedWizardSchedulerPartslist.IsSolved = true;
                if (!WizardSchedulerPartslistList.Contains(SelectedWizardSchedulerPartslist) && SelectedWizardSchedulerPartslist.ProdOrderPartslistPos != null)
                {
                    WizardSchedulerPartslist tmpWizard = WizardSchedulerPartslistList.Where(c => c.ProdOrderPartslistPos != null && c.ProdOrderPartslistPos.ProdOrderPartslistPosID == SelectedWizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderPartslistPosID).FirstOrDefault();
                    if (tmpWizard != null)
                    {
                        tmpWizard.IsSolved = true;
                    }
                }
                OnWizardListChange();
                if (WizardSchedulerPartslistList.Any())
                {
                    if (DatabaseApp.IsChanged)
                    {
                        LocalSaveChanges();
                    }
                    WizardPhase = NewScheduledBatchWizardPhaseEnum.PartslistForDefinition;
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

        [ACMethodInfo(nameof(WizardForwardSelectLinie), "en{'Plan'}de{'Planen'}", 9999)]
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
                                    && wizardSchedulerPartslist.MDProdOrderState.ProdOrderState < VD.MDProdOrderState.ProdOrderStates.ProdFinished
                                )
                        ))
            {
                SelectedWizardSchedulerPartslist = wizardSchedulerPartslist;
                WizardForward();
            }
        }

        [ACMethodInfo(nameof(WizardDeletePartslist), "en{'Remove'}de{'Entfernen'}", 9999)]
        public void WizardDeletePartslist(object CommandParameter)
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
                                    && wizardSchedulerPartslist.MDProdOrderState.ProdOrderState < VD.MDProdOrderState.ProdOrderStates.ProdFinished
                                )
                        ))
            {
                List<Guid> groupsForRefresh = new List<Guid>();
                List<VD.ProdOrderBatchPlan> batchPlans = wizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrderBatchPlan_ProdOrderPartslist.ToList();

                if (batchPlans.Any(c => c.IsSelected))
                {
                    batchPlans = batchPlans.Where(c => c.IsSelected).ToList();
                }

                Dictionary<VD.ProdOrderPartslistPos, double> positions =
                        batchPlans
                        .ToDictionary(key => key.ProdOrderPartslistPos, val => val.BatchTargetCount * val.BatchSize);

                OnSetBatchStateCanceling(positions);
                DoSetBatchStateCancelled(false, batchPlans, ref groupsForRefresh);

                wizardSchedulerPartslist.IsSolved = true;
                OnWizardListChange();
            }
        }

        [ACMethodInfo(nameof(WizardSetPreferredParams), VD.ConstApp.PrefParam, 9999)]
        public void WizardSetPreferredParams(object CommandParameter)
        {
            if (CommandParameter != null)
            {
                WizardSchedulerPartslist wizardSchedulerPartslist = CommandParameter as WizardSchedulerPartslist;
                if (
                         BSOBatchPlanChild != null
                        && BSOBatchPlanChild.Value != null
                        && BSOBatchPlanChild.Value.BSOPreferredParameters_Child != null
                        && BSOBatchPlanChild.Value.BSOPreferredParameters_Child.Value != null
                        && wizardSchedulerPartslist != null
                        && wizardSchedulerPartslist.WFNodeMES != null
                        && wizardSchedulerPartslist.HasRequiredParams
                  )
                {
                    if (wizardSchedulerPartslist.ProdOrderPartslist == null)
                    {
                        string programNo = "";
                        ProdOrderManager.FactoryProdOrderPartslist(DatabaseApp, null, wizardSchedulerPartslist, DefaultWizardSchedulerPartslist?.ProdOrderPartslist, ref programNo);
                        ACSaveChanges();
                    }

                    if (wizardSchedulerPartslist.ProdOrderPartslist != null)
                    {
                        bool isParamDefined = BSOBatchPlanChild.Value.BSOPreferredParameters_Child.Value.ShowParamDialogResult(
                         wizardSchedulerPartslist.WFNodeMES.ACClassWFID,
                         wizardSchedulerPartslist.ProdOrderPartslist.PartslistID,
                         wizardSchedulerPartslist.ProdOrderPartslist.ProdOrderPartslistID,
                         null);

                        if (isParamDefined)
                        {
                            wizardSchedulerPartslist.IsRequiredParamsSolved = wizardSchedulerPartslist.ProdOrderPartslist.ProdOrderPartslistConfig_ProdOrderPartslist.Any();
                            wizardSchedulerPartslist.OnPropertyChanged(nameof(wizardSchedulerPartslist.ParamState));
                            wizardSchedulerPartslist.OnPropertyChanged(nameof(wizardSchedulerPartslist.ParamStateName));
                        }
                    }
                }
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


        [ACMethodInfo(nameof(ChangeBatchPlan), "en{'Change'}de{'Bearbeiten'}", 600, true)]
        public void ChangeBatchPlan(VD.ProdOrderBatchPlan batchPlan)
        {
            if (batchPlan == null)
            {
                Messages.LogError(this.GetACUrl(), "ChangeBatchPlan(10)", "batchPlan is null");
                return;
            }
            ChangeBatchPlanForSchedule(batchPlan, SelectedScheduleForPWNode);
        }

        protected void ChangeBatchPlanForSchedule(VD.ProdOrderBatchPlan batchPlan, PAScheduleForPWNode forSchedule)
        {
            bool notValidBatchForChange =
                       batchPlan == null
                    || forSchedule == null
                    || IsWizard
                    //|| batchPlan.ProdOrderBatch_ProdOrderBatchPlan.Any()
                    || batchPlan.ProdOrderPartslist.MDProdOrderState.MDProdOrderStateIndex >= (short)VD.MDProdOrderState.ProdOrderStates.ProdFinished
                    || batchPlan.ProdOrderPartslist.ProdOrder.MDProdOrderState.MDProdOrderStateIndex >= (short)VD.MDProdOrderState.ProdOrderStates.ProdFinished;
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

                VD.ACClassWF vbACClassWF = batchPlan.VBiACClassWF;
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

        #endregion

        #region Methods -> Wizard -> Execute

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool IsEnabledWizardForward()
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
                    isEnabled =
                        SelectedWizardSchedulerPartslist != null
                        && (!SelectedWizardSchedulerPartslist.IsSolved
                        || SelectedWizardSchedulerPartslist.ProdOrderPartslistPos != null)
                        && SelectedWizardSchedulerPartslist.SelectedMDSchedulingGroup != null;
                    // && SelectedWizardSchedulerPartslist.ParamState != WizardSetupParamStateEnum.ParamsRequiredNotDefined; - no requiered param validation in wizard
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

                        VD.Partslist selectedPartslist = DefaultWizardSchedulerPartslist.Partslist;
                        if (selectedPartslist.MDUnitID.HasValue && selectedPartslist.MDUnitID != selectedPartslist.Material.BaseMDUnitID)
                            DefaultWizardSchedulerPartslist.TargetQuantity = selectedPartslist.Material.ConvertQuantity(DefaultWizardSchedulerPartslist.TargetQuantityUOM, selectedPartslist.Material.BaseMDUnit, selectedPartslist.MDUnit);
                        SelectedWizardSchedulerPartslist = DefaultWizardSchedulerPartslist;
                        OnSelectedWizardSchedulerPartslistChanged();

                        double treeQuantityRatio = DefaultWizardSchedulerPartslist.TargetQuantityUOM / selectedPartslist.TargetQuantityUOM;
                        rootPartslistExpand = new VD.PartslistExpand(selectedPartslist, 1, treeQuantityRatio);
                        rootPartslistExpand.IsChecked = true;
                        rootPartslistExpand.LoadTree();
                        rootPartslistExpand.IsEnabled = false;
                        rootPartslistExpand.IsChecked = false;
                        _PartListExpandList = new List<VD.PartslistExpand>();
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
                        {
                            List<VD.ProdOrderPartslist> plsForDefinition =
                                DefaultWizardSchedulerPartslist
                                .ProdOrderPartslistPos
                                .ProdOrderPartslist
                                .ProdOrder
                                .ProdOrderPartslist_ProdOrder
                                .OrderByDescending(c => c.Sequence).ToList();
                            LoadExistingWizardSchedulerPartslistList(plsForDefinition);
                        }
                        if (rootPartslistExpand != null)
                        {
                            LoadBOMExplosionItems(DefaultWizardSchedulerPartslist.ProdOrderPartslistPos?.ProdOrderPartslist.ProdOrder);
                        }

                        foreach (var item in AllWizardSchedulerPartslistList)
                        {
                            if (item.SelectedMDSchedulingGroup != null)
                                item.LoadConfiguration();
                        }
                        success = SelectedWizardSchedulerPartslist != null;
                        OnWizardListChange();
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
                                List<VD.ProdOrderBatchPlan> generatedBatchPlans;
                                success = FactoryBatchPlans(SelectedWizardSchedulerPartslist, ref programNo, out generatedBatchPlans);
                            }

                            bool saveSuccess = LocalSaveChanges();
                            if (success && !string.IsNullOrEmpty(programNo) && saveSuccess)
                            {
                                AllWizardSchedulerPartslistList.ForEach(x => x.ProgramNo = programNo);
                            }
                            WizardSolvedTasks.Add(NewScheduledBatchWizardPhaseEnum.DefineTargets);
                            OnWizardListChange();
                        }
                        break;
                    case NewScheduledBatchWizardPhaseEnum.DeleteBatchPlan:
                        List<VD.ProdOrderPartslist> partslists = new List<VD.ProdOrderPartslist>();
                        VD.ProdOrder prodOrder = SelectedProdOrderBatchPlan.ProdOrderPartslist.ProdOrder;
                        foreach (VD.ProdOrderPartslist pl in prodOrder.ProdOrderPartslist_ProdOrder.OrderBy(c => c.Sequence).ToArray())
                        {
                            bool existBatchPlanForDeleteOrCancel =
                                pl
                                .ProdOrderBatchPlan_ProdOrderPartslist
                                .Any(c => c.PlanStateIndex == (short)VD.GlobalApp.BatchPlanState.Created || c.PlanStateIndex == (short)VD.GlobalApp.BatchPlanState.Paused);
                            if (existBatchPlanForDeleteOrCancel)
                            {
                                partslists.Add(pl);
                            }
                        }
                        LoadExistingWizardSchedulerPartslistList(partslists);
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

        private void OnWizardListChange()
        {
            var tmp = _SelectedWizardSchedulerPartslist;
            OnPropertyChanged(nameof(WizardSchedulerPartslistList));
            _SelectedWizardSchedulerPartslist = tmp;
        }



        #endregion

        #region Methods -> Wizard -> Helper 

        private void ClearAndResetChildBSOLists(VD.MDSchedulingGroup mdSchedulingGroup)
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
            bool success = LocalSaveChanges();
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
            VD.ProdOrder prodorder = DefaultWizardSchedulerPartslist?.ProdOrderPartslistPos?.ProdOrderPartslist?.ProdOrder;
            if (prodorder != null)
            {
                ProdOrderManager.ConnectSourceProdOrderPartslist(prodorder);
                ProdOrderManager.CorrectSortOrder(prodorder);
            }
        }

        private void LoadBOMExplosionItems(VD.ProdOrder prodOrder)
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
                VD.PartslistExpand partslistExpand = expand.Item as VD.PartslistExpand;

                WizardSchedulerPartslist wizardSchedulerPartslist =
                    AllWizardSchedulerPartslistList
                    .Where(c =>
                                c.ProdOrderPartslistPos != null
                                && c.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo == partslistExpand.PartslistNo)
                    .FirstOrDefault();

                sn++;

                if (wizardSchedulerPartslist == null)
                {
                    VD.ProdOrderPartslist prodOrderPartslist = null;
                    if (prodOrder != null)
                        prodOrderPartslist = prodOrder.ProdOrderPartslist_ProdOrder.FirstOrDefault(c => c.Partslist.PartslistNo == partslistExpand.PartslistNo);

                    List<VD.MDSchedulingGroup> schedulingGroups = ProdOrderManager.GetSchedulingGroups(DatabaseApp, PWNodeProcessWorkflowVB.PWClassName, partslistExpand.Partslist, PartslistMDSchedulerGroupConnections);
                    if (prodOrderPartslist != null)
                        wizardSchedulerPartslist =
                            new WizardSchedulerPartslist(
                                DatabaseApp,
                                ProdOrderManager,
                                LocalBSOBatchPlan.VarioConfigManager,
                                RoundingQuantity,
                                partslistExpand.Partslist,
                                partslistExpand.TargetQuantityUOM,
                                sn,
                                schedulingGroups,
                                prodOrderPartslist);
                    else
                    {
                        VD.MDSchedulingGroup schedulingGroup = DatabaseApp.MDSchedulingGroup.Where(c => c.MDSchedulingGroupID == SelectedScheduleForPWNode.MDSchedulingGroupID).FirstOrDefault();
                        wizardSchedulerPartslist =
                            new WizardSchedulerPartslist(
                                DatabaseApp,
                                ProdOrderManager,
                                LocalBSOBatchPlan.VarioConfigManager,
                                RoundingQuantity,
                                partslistExpand.Partslist,
                                partslistExpand.TargetQuantityUOM,
                                sn,
                                SelectedFilterBatchPlanGroup,
                                schedulingGroups,
                                schedulingGroup);
                    }


                    AddWizardSchedulerPartslistList(wizardSchedulerPartslist, sn);
                }

                wizardSchedulerPartslist.Sn = sn;
                if (prodOrder != null && string.IsNullOrEmpty(wizardSchedulerPartslist.ProgramNo))
                {
                    wizardSchedulerPartslist.ProgramNo = prodOrder.ProgramNo;
                }
            }
            if (AllWizardSchedulerPartslistList.Any())
            {
                DefaultWizardSchedulerPartslist.Sn = AllWizardSchedulerPartslistList.Count();
            }
        }

        private void LoadExistingWizardSchedulerPartslistList(List<VD.ProdOrderPartslist> partslists)
        {
            AllWizardSchedulerPartslistList = new List<WizardSchedulerPartslist>();
            foreach (VD.ProdOrderPartslist prodOrderPartslist in partslists)
            {
                bool isThere = AllWizardSchedulerPartslistList.Where(c => c.ProdOrderPartslistPos != null && c.ProdOrderPartslistPos.ProdOrderPartslistID == prodOrderPartslist.ProdOrderPartslistID).Any();
                if (!isThere)
                {
                    List<VD.MDSchedulingGroup> schedulingGroups = ProdOrderManager.GetSchedulingGroups(DatabaseApp, PWNodeProcessWorkflowVB.PWClassName, prodOrderPartslist.Partslist, PartslistMDSchedulerGroupConnections);
                    WizardSchedulerPartslist item =
                        new WizardSchedulerPartslist(
                            DatabaseApp,
                            ProdOrderManager,
                            LocalBSOBatchPlan.VarioConfigManager,
                            RoundingQuantity,
                            prodOrderPartslist.Partslist,
                            prodOrderPartslist.TargetQuantity,
                            prodOrderPartslist.Sequence,
                            schedulingGroups,
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


        public void WizardDefineDefaultPartslist(VD.MDSchedulingGroup schedulingGroup, VD.Partslist partslist, double targetQuantity, VD.ProdOrderPartslist prodOrderPartslist = null)
        {
            List<VD.MDSchedulingGroup> schedulingGroups = ProdOrderManager.GetSchedulingGroups(DatabaseApp, PWNodeProcessWorkflowVB.PWClassName, partslist, PartslistMDSchedulerGroupConnections);
            if (prodOrderPartslist != null)
                DefaultWizardSchedulerPartslist =
                    new WizardSchedulerPartslist(
                        DatabaseApp,
                        ProdOrderManager,
                        LocalBSOBatchPlan.VarioConfigManager,
                        RoundingQuantity,
                        partslist,
                        targetQuantity,
                        1,
                        schedulingGroups,
                        prodOrderPartslist);
            else
            {
                VD.MDSchedulingGroup selectedMDSchedulingGroup = DatabaseApp.MDSchedulingGroup.Where(c => c.MDSchedulingGroupID == SelectedScheduleForPWNode.MDSchedulingGroupID).FirstOrDefault();
                DefaultWizardSchedulerPartslist =
                                    new WizardSchedulerPartslist(
                                        DatabaseApp,
                                        ProdOrderManager,
                                        LocalBSOBatchPlan.VarioConfigManager,
                                        RoundingQuantity,
                                        partslist,
                                        targetQuantity,
                                        1,
                                        SelectedFilterBatchPlanGroup,
                                        schedulingGroups,
                                        selectedMDSchedulingGroup);

                if (schedulingGroups.Select(c => c.MDSchedulingGroupID).Contains(selectedMDSchedulingGroup.MDSchedulingGroupID))
                {
                    DefaultWizardSchedulerPartslist.SelectedMDSchedulingGroup = selectedMDSchedulingGroup;
                }
            }

            AllWizardSchedulerPartslistList.Clear();
            AddWizardSchedulerPartslistList(DefaultWizardSchedulerPartslist);
            SelectedWizardSchedulerPartslist = DefaultWizardSchedulerPartslist;
            if (SelectedWizardSchedulerPartslist != null)
            {
                SelectedWizardSchedulerPartslist.LoadConfiguration();
            }
        }

        [ACMethodInfo(nameof(WizardCancel), "en{'Close'}de{'Schließen'}", 511)]
        public void WizardCancel()
        {
            IsWizard = false;
            ACUndoChanges();

            ConnectSourceProdOrderPartslist();
            ACSaveChanges();

            Guid? selectedProdOrderBatchPlanID = null;
            if (SelectedWizardSchedulerPartslist != null && SelectedWizardSchedulerPartslist.ProdOrderPartslistPos != null)
                selectedProdOrderBatchPlanID =
                    SelectedWizardSchedulerPartslist
                    .ProdOrderPartslistPos
                    .ProdOrderBatchPlan_ProdOrderPartslistPos
                    .Select(c => c.ProdOrderBatchPlanID)
                    .DefaultIfEmpty()
                    .FirstOrDefault();
            if (selectedProdOrderBatchPlanID == null && SelectedProdOrderBatchPlan != null)
                selectedProdOrderBatchPlanID = SelectedProdOrderBatchPlan.ProdOrderBatchPlanID;

            WizardClean();
            OnPropertyChanged(nameof(CurrentLayout));
            RefreshScheduleForSelectedNode(selectedProdOrderBatchPlanID);
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

            WizardPhase = NewScheduledBatchWizardPhaseEnum.None;

            WizardSolvedTasks.Clear();
            ClearMessages();
        }

        #endregion

        #endregion

        #region Methods -> BatchPlanTimelinePresenter

        [ACMethodInfo("", "en{'Show'}de{'Anzeigen'}", 520)]
        public void ShowBatchPlansOnTimeline()
        {
            List<BatchPlanTimelineItem> treeViewItems = new List<BatchPlanTimelineItem>();
            List<BatchPlanTimelineItem> timelineItems = new List<BatchPlanTimelineItem>();

            VD.GlobalApp.BatchPlanState startState = VD.GlobalApp.BatchPlanState.Created;
            VD.GlobalApp.BatchPlanState endState = VD.GlobalApp.BatchPlanState.Paused;
            VD.MDProdOrderState.ProdOrderStates? minProdOrderState = null;
            VD.MDProdOrderState.ProdOrderStates? maxProdOrderState = MDProdOrderState.ProdOrderStates.InProduction;
            ObservableCollection<VD.ProdOrderBatchPlan> prodOrderBatchPlans =
                ProdOrderManager
                .GetProductionLinieBatchPlans(
                    DatabaseApp,
                    null,
                    startState,
                    endState,
                    FilterStartTime,
                    FilterEndTime,
                    minProdOrderState,
                    maxProdOrderState,
                    FilterPlanningMR?.PlanningMRID,
                    SelectedFilterBatchPlanGroup?.MDBatchPlanGroupID,
                    FilterBatchProgramNo,
                    FilterBatchMaterialNo);

            int displayOrder = 0;

            foreach (PAScheduleForPWNode schedule in ScheduleForPWNodeList)
            {
                BatchPlanTimelineItem treeViewItem = new BatchPlanTimelineItem();
                treeViewItem.ACCaption = schedule.ACCaption;
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
                List<VD.ProdOrderBatchPlan> batchPlans = prodOrderBatchPlans.Where(c => acClassWFIds.Contains(c.VBiACClassWFID ?? Guid.Empty)).ToList();
                foreach (VD.ProdOrderBatchPlan batchPlan in batchPlans)
                {
                    BatchPlanTimelineItem item = new BatchPlanTimelineItem();
                    item.ACCaption = string.Format(@"[{0}] {1} ({2} {3})",
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
                    case nameof(New):
                        if (IsEnabledNew())
                            result = Global.ControlModes.Enabled;
                        else
                            result = Global.ControlModes.Hidden;
                        break;
                    case "SelectedProdOrderBatchPlan\\PartialTargetCount":
                        //SelectedProdOrderBatchPlan.PlannedStartDate && SelectedProdOrderBatchPlan.end
                        if (SelectedScheduleForPWNode != null
                            && SelectedScheduleForPWNode.StartMode == VD.BatchPlanStartModeEnum.SemiAutomatic
                            && SelectedProdOrderBatchPlan != null
                            && SelectedProdOrderBatchPlan.PlanMode == VD.BatchPlanMode.UseBatchCount
                            && SelectedProdOrderBatchPlan.BatchTargetCount > SelectedProdOrderBatchPlan.BatchActualCount)
                            result = Global.ControlModes.Enabled;
                        else
                            result = Global.ControlModes.Disabled;
                        break;
                    case "SelectedProdOrderBatchPlan\\ScheduledStartDate":
                    case "SelectedProdOrderBatchPlan\\ScheduledEndDate":
                        if (SelectedProdOrderBatchPlan != null
                            && (SelectedProdOrderBatchPlan.PlanState <= VD.GlobalApp.BatchPlanState.ReadyToStart || SelectedProdOrderBatchPlan.PlanState == VD.GlobalApp.BatchPlanState.Paused)
                            && (SelectedProdOrderBatchPlan.PlanMode != VD.BatchPlanMode.UseBatchCount
                               || SelectedProdOrderBatchPlan.BatchTargetCount > SelectedProdOrderBatchPlan.BatchActualCount))
                            result = Global.ControlModes.Enabled;
                        else
                            result = Global.ControlModes.Disabled;
                        break;
                    case nameof(FilterOrderStartTime):
                        result = Global.ControlModes.Enabled;
                        bool filterOrderStartTimeIsEnabled =
                           !(FilterOrderIsCompleted ?? true)
                           || (FilterOrderStartTime != null && FilterOrderEndTime != null);
                        if (!filterOrderStartTimeIsEnabled)
                            result = Global.ControlModes.EnabledWrong;
                        break;
                    case nameof(FilterOrderEndTime):
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

        #region Methods -> Private (Helper) Mehtods

        #region Methods -> Private (Helper) Mehtods -> Load

        protected override Guid? EntityIDOfSelectedSchedule
        {
            get
            {
                return SelectedProdOrderBatchPlan?.ProdOrderBatchPlanID;
            }
        }


        protected override void RefreshScheduleForSelectedNode(Guid? selectedEntityID = null)
        {
            try
            {
                using (ACMonitor.Lock(DatabaseApp.QueryLock_1X000))
                {
                    ProdOrderBatchPlanList = GetProdOrderBatchPlanList(SelectedScheduleForPWNode?.MDSchedulingGroupID);
                    if (selectedEntityID != null)
                    {
                        SelectedProdOrderBatchPlan = ProdOrderBatchPlanList.FirstOrDefault(c => c.ProdOrderBatchPlanID == selectedEntityID);
                    }
                    else
                    {
                        SelectedProdOrderBatchPlan = ProdOrderBatchPlanList.FirstOrDefault();
                    }
                    ProdOrderPartslistList = GetProdOrderPartslistList();
                    FinishedProdOrderBatchList = null;
                    SelectedFinishedProdOrderBatch = null;
                }
            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), nameof(RefreshScheduleForSelectedNode), e);
                Messages.LogException(this.GetACUrl(), nameof(RefreshScheduleForSelectedNode), e.StackTrace);
            }
        }

        protected override void OnUpdateScheduleForPWNodeList()
        {
            if (WizardPhase == NewScheduledBatchWizardPhaseEnum.None)
                RefreshScheduleForSelectedNode(EntityIDOfSelectedSchedule);
        }



        #endregion

        #region Methods -> Private(Helper) Mehtods -> Factory Batch

        private void WritePosMDUnit(VD.ProdOrderBatchPlan prodOrderBatchPlan, WizardSchedulerPartslist wizardSchedulerPartslist)
        {
            if (wizardSchedulerPartslist.SelectedUnitConvert != null
                && wizardSchedulerPartslist.SelectedUnitConvert != wizardSchedulerPartslist.Partslist.Material.BaseMDUnit)
                prodOrderBatchPlan.ProdOrderPartslistPos.MDUnit = wizardSchedulerPartslist.SelectedUnitConvert;
            else
                prodOrderBatchPlan.ProdOrderPartslistPos.MDUnit = null;
        }

        private bool FactoryBatchPlans(WizardSchedulerPartslist wizardSchedulerPartslist, ref string programNo, out List<VD.ProdOrderBatchPlan> generatedBatchPlans)
        {
            bool success = false;
            generatedBatchPlans = new List<VD.ProdOrderBatchPlan>();
            success =
                ProdOrderManager
                .FactoryProdOrderPartslist(
                        DatabaseApp,
                        FilterPlanningMR,
                        wizardSchedulerPartslist,
                        DefaultWizardSchedulerPartslist?.ProdOrderPartslistPos?.ProdOrderPartslist,
                        ref programNo
                        );
            if (success)
            {
                success =
                ProdOrderManager.FactoryBatchPlans(
                        DatabaseApp,
                        FilterPlanningMR,
                        CreatedBatchState,
                        wizardSchedulerPartslist,
                        out generatedBatchPlans);
            }

            if (success)
            {
                wizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderPartslist.ProdUserEndDate = wizardSchedulerPartslist._ProdUserEndDate;
                SetBSOBatchPlan_BatchParents(wizardSchedulerPartslist.WFNodeMES, wizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderPartslist);
                VD.ProdOrderBatchPlan firstBatchPlan = wizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderBatchPlan_ProdOrderPartslistPos.FirstOrDefault();
                LoadGeneratedBatchInCurrentLine(firstBatchPlan, wizardSchedulerPartslist.NewTargetQuantityUOM);
            }

            return success;
        }

        private bool UpdateBatchPlans(WizardSchedulerPartslist wizardSchedulerPartslist)
        {
            List<VD.ProdOrderBatchPlan> otherBatchPlans = GetProdOrderBatchPlanList(wizardSchedulerPartslist.SelectedMDSchedulingGroup.MDSchedulingGroupID).ToList();

            (bool success, bool newBatchPlan) = ProdOrderManager.UpdateBatchPlans(DatabaseApp, wizardSchedulerPartslist, otherBatchPlans);

            SetBSOBatchPlan_BatchParents(wizardSchedulerPartslist.WFNodeMES, wizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderPartslist);

            VD.ProdOrderBatchPlan[] tmp = wizardSchedulerPartslist.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrderBatchPlan_ProdOrderPartslist.OrderByDescending(c => c.ScheduledOrder).ToArray();
            foreach (VD.ProdOrderBatchPlan bp in tmp)
            {
                LoadGeneratedBatchInCurrentLine(bp, SelectedWizardSchedulerPartslist.NewTargetQuantityUOM);
            }
            //if (
            //        !AllWizardSchedulerPartslistList.Any(c => c.ProdOrderPartslistPos != null && c.PartslistNo != wizardSchedulerPartslist.PartslistNo)
            //        || AllWizardSchedulerPartslistList.Where(c => !c.IsSolved).Count() == 1
            //        || newBatchPlan
            //    )
            //{
            //    wizardSchedulerPartslist.IsSolved = success;
            //}
            return success;
        }

        #endregion

        #region Methods -> Private (Helper) Mehtods -> LocalBSOBatchPlan Select batch plan

        private void SetBSOBatchPlan_BatchParents(VD.ACClassWF vbACClassWF, VD.ProdOrderPartslist prodOrderPartslist)
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

        private void LoadGeneratedBatchInCurrentLine(VD.ProdOrderBatchPlan batchPlan, double targetQuantityUOM)
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

        private bool SetProdorderPartslistState(VD.MDProdOrderState mDProdOrderStateCancelled, VD.MDProdOrderState mDProdOrderStateCompleted, VD.ProdOrderPartslist partslist)
        {
            bool isSetState = false;

            bool noBatchPlan = !partslist.ProdOrderBatchPlan_ProdOrderPartslist.Any();
            bool anyNotCompleted = partslist.ProdOrderBatchPlan_ProdOrderPartslist.Where(c => c.PlanStateIndex != (short)VD.GlobalApp.BatchPlanState.Completed).Any();

            if (noBatchPlan || anyNotCompleted)
            {
                partslist.MDProdOrderState = mDProdOrderStateCancelled;
                isSetState = true;
            }
            else if (!anyNotCompleted && mDProdOrderStateCompleted != null)
            {
                partslist.MDProdOrderState = mDProdOrderStateCompleted;
                isSetState = true;
            }

            return isSetState;
        }

        private void RemovePartslist(VD.ProdOrderPartslist partslist, VD.MDProdOrderState mDProdOrderStateCancelled, VD.MDProdOrderState mDProdOrderStateCompleted)
        {
            VD.ProdOrder prodOrder = partslist.ProdOrder;
            if (!partslist.ProdOrderBatchPlan_ProdOrderPartslist.Any())
            {
                bool hasAnyPostings =
                                    partslist
                                    .ProdOrderPartslistPos_ProdOrderPartslist
                                    .SelectMany(c => c.FacilityBooking_ProdOrderPartslistPos)
                                    .Any()
                                    ||
                                    partslist
                                    .ProdOrderPartslistPos_ProdOrderPartslist
                                    .SelectMany(c => c.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
                                    .SelectMany(c => c.FacilityBooking_ProdOrderPartslistPosRelation)
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
                        SequenceManager<VD.ProdOrderPartslist>.Order(ref tempPlList);
                    }
                    if (prodOrder.ProdOrderPartslist_ProdOrder == null || !prodOrder.ProdOrderPartslist_ProdOrder.Any())
                        prodOrder.DeleteACObject(DatabaseApp, false);
                }
            }
        }

        #endregion

        #endregion

        #region Methods => RouteCalculation

        [ACMethodInteraction("", "en{'Route check over orders'}de{'Routenprüfung über Aufträge'}", 9999, true)]
        public void RunPossibleRoutesCheck()
        {
            MsgList.Clear();
            CalculateRouteResult = null;
            CurrentProgressInfo.ProgressInfoIsIndeterminate = true;

            if (BSOBatchPlanChild != null && BSOBatchPlanChild.Value != null)
            {
                bool invoked = BSOBatchPlanChild.Value.InvokeCalculateRoutesAsync();
                if (!invoked)
                {
                    Messages.Info(this, "The calculation is in progress, please wait and try again!");
                    return;
                }
            }

            ShowDialog(this, "CalculatedRouteDialog");
        }

        public bool IsEnabledPossibleRoutesCheck()
        {
            return SelectedProdOrderBatchPlan != null && BSOBatchPlanChild != null;
        }

        public void SetRoutesCheckResult(IEnumerable<Msg> msgList)
        {
            foreach (Msg msg in msgList)
            {
                MsgList.Add(msg);
            }
        }

        #endregion

        #region Methods -> common (helper) private methods

        private void ShowProdOrder(ProdOrderPartslist prodOrderPartslist)
        {
            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(
                new PAOrderInfoEntry()
                {
                    EntityID = prodOrderPartslist.ProdOrderPartslistID,
                    EntityName = VD.ProdOrderPartslist.ClassName
                });
                if (prodOrderPartslist.PlanningMRProposal_ProdOrderPartslist.Any())
                {
                    info.Entities.Add(
                    new PAOrderInfoEntry()
                    {
                        EntityID = prodOrderPartslist.PlanningMRProposal_ProdOrderPartslist.Select(c => c.PlanningMRID).FirstOrDefault(),
                        EntityName = VD.PlanningMR.ClassName
                    });
                }
                service.ShowDialogOrder(this, info);
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
                case BGWorkerMethod_DoBackwardScheduling:
                    using (ACMonitor.Lock(DatabaseApp.QueryLock_1X000))
                    {
                        e.Result =
                        SchedulingForecastManager
                        .BackwardScheduling(
                                            DatabaseApp,
                                            SelectedScheduleForPWNode.MDSchedulingGroupID,
                                            updateName,
                                            ScheduledEndDate.Value);
                    }
                    break;
                case BGWorkerMethod_DoForwardScheduling:
                    using (ACMonitor.Lock(DatabaseApp.QueryLock_1X000))
                    {
                        e.Result =
                        SchedulingForecastManager
                        .ForwardScheduling(
                                            DatabaseApp,
                                            SelectedScheduleForPWNode.MDSchedulingGroupID,
                                            updateName,
                                            ScheduledStartDate.Value);
                    }
                    break;
                case BGWorkerMethod_DoCalculateAll:
                    SchedulingForecastManager.UpdateAllBatchPlanDurations(Root.Environment.User.Initials);
                    break;
                case BGWorkerMehtod_DoGenerateBatchPlans:
                    using (ACMonitor.Lock(DatabaseApp.QueryLock_1X000))
                    {
                        List<VD.ProdOrderPartslist> plForBatchGenerate = ProdOrderPartslistList.Where(c => c.IsSelected).Select(c => c.ProdOrderPartslist).ToList();
                        e.Result = ProdOrderManager.GenerateBatchPlans(DatabaseApp, LocalBSOBatchPlan.VarioConfigManager, RoundingQuantity, LocalBSOBatchPlan.RoutingService, PWNodeProcessWorkflowVB.PWClassName, plForBatchGenerate, PartslistMDSchedulerGroupConnections);
                    }
                    break;
                case BGWorkerMehtod_DoMergeOrders:
                    using (ACMonitor.Lock(DatabaseApp.QueryLock_1X000))
                    {
                        List<VD.ProdOrderPartslist> plForMerge = ProdOrderPartslistList.Where(c => c.IsSelected).Select(c => c.ProdOrderPartslist).ToList();
                        e.Result = ProdOrderManager.MergeOrders(DatabaseApp, plForMerge);
                    }
                    break;
                case BGWorkerMehtod_DoSearchStockMaterial:
                    List<MaterialPreparationModel> preparedMaterials = DoSearchStockMaterial();
                    e.Result = preparedMaterials;
                    break;
                case nameof(DoSearchOrders):
                case nameof(DoSearchOrdersAll):
                    bool searchFinishedPlans = command == nameof(DoSearchOrdersAll);
                    (IEnumerable<ProdOrderPartslistPlanWrapper> prodOrders, List<VD.ProdOrderPartslistPos> finishedPlans) = DoSearchOrders(searchFinishedPlans);
                    e.Result = new Tuple<IEnumerable<ProdOrderPartslistPlanWrapper>, List<VD.ProdOrderPartslistPos>>(prodOrders, finishedPlans);
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
                    List<MaterialPreparationModel> preparedMaterials = e.Result as List<MaterialPreparationModel>;
                    BSOMaterialPreparationChild.Value.LoadMaterialPreparationResult(preparedMaterials);
                }
                else if (command == nameof(DoSearchOrders) || command == nameof(DoSearchOrdersAll))
                {
                    Tuple<IEnumerable<ProdOrderPartslistPlanWrapper>, List<VD.ProdOrderPartslistPos>> result = (Tuple<IEnumerable<ProdOrderPartslistPlanWrapper>, List<VD.ProdOrderPartslistPos>>)e.Result;
                    ProdOrderPartslistList = result.Item1;
                    FinishedProdOrderBatchList = result.Item2;
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
                                RefreshScheduleForSelectedNode();
                                break;
                        }
                    }
                }
            }
        }

        #endregion

        #region BackgroundWorker -> DoMehtods

        public const string DoSearchOrdersAll = "DoSearchOrders";

        private (IEnumerable<ProdOrderPartslistPlanWrapper> prodOrders, List<VD.ProdOrderPartslistPos> finishedPlans) DoSearchOrders(bool searchFinishedPlans)
        {
            IEnumerable<ProdOrderPartslistPlanWrapper> prodOrders = null;
            List<VD.ProdOrderPartslistPos> finishedPlans = new List<VD.ProdOrderPartslistPos>();
            try
            {
                prodOrders = GetProdOrderPartslistList();
                //VD.ProdOrderPartslistPos posTest = null;
                //posTest.ProdOrderBatch.ProdOrderBatchPlan.MDBatchPlanGroup = null;
                //posTest.ProdOrderBatch.ProdOrderBatchPlan.IplusVBiACClassWF = null;
                Guid? mdSchedulingGroupID = SelectedScheduleForPWNode.MDSchedulingGroupID;
                VD.MDProdOrderState.ProdOrderStates? minProdOrderState = null;
                _IsRefreshingBatchPlan = true;

                int? cmdTimeout = DatabaseApp.CommandTimeout;
                DatabaseApp.CommandTimeout = 60 * 3;

                finishedPlans = new List<ProdOrderPartslistPos>();

                if (searchFinishedPlans)
                {
                    finishedPlans =
                    ProdOrderManager
                  .GetFinishedBatch(
                      DatabaseApp,
                      mdSchedulingGroupID,
                      FilterOrderStartTime,
                      FilterOrderEndTime,
                      minProdOrderState,
                      null,
                      null,
                      FilterOrderProgramNo,
                      FilterOrderMaterialNo)
                      .ToList();
                }

                DatabaseApp.CommandTimeout = cmdTimeout;

            }
            catch (Exception ex)
            {
                this.Messages.LogException(this.GetACUrl(), "DoSearchOrders(10)", ex);
                this.Messages.LogException(this.GetACUrl(), "DoSearchOrders(10)", ex.StackTrace);
            }
            finally
            {
                _IsRefreshingBatchPlan = false;
            }



            return (prodOrders, finishedPlans);
        }

        #region BackgroundWorker -> DoMehtods -> SearchStockMaterial

        private List<MaterialPreparationModel> DoSearchStockMaterial()
        {
            using (ACMonitor.Lock(DatabaseApp.QueryLock_1X000))
            {
                List<VD.ProdOrderBatchPlan> selectedBatchPlans = new List<VD.ProdOrderBatchPlan>();
                if (ScheduleForPWNodeList != null
                    && ScheduleForPWNodeList.Where(c => c.IsSelected).Any())
                {
                    PAScheduleForPWNode[] selectedLines = ScheduleForPWNodeList.Where(c => c.IsSelected).ToArray();
                    foreach (PAScheduleForPWNode selectedLine in selectedLines)
                    {
                        List<VD.ProdOrderBatchPlan> lineItems = GetProdOrderBatchPlanList(selectedLine.MDSchedulingGroupID).ToList();
                        selectedBatchPlans.AddRange(lineItems);
                    }
                }
                else if (ProdOrderBatchPlanList != null && ProdOrderBatchPlanList.Where(c => c.IsSelected).Any())
                {
                    selectedBatchPlans = ProdOrderBatchPlanList.Where(c => c.IsSelected).ToList();
                }

                return BSOMaterialPreparationChild.Value.DoSearchStockMaterial(selectedBatchPlans);
            }
        }

        #endregion

        #endregion

    }


    /// <summary>
    /// State in Wizard
    /// </summary>
    public enum NewScheduledBatchWizardPhaseEnum : short
    {
        None = 0,
        SelectMaterial = 1,
        SelectPartslist = 2,
        BOMExplosion = 3,
        PartslistForDefinition = 4,
        DefineBatch = 5,
        DefineTargets = 6,
        DeleteBatchPlan = 7
    }

}
