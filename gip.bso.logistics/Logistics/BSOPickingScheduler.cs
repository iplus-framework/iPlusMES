using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using VD = gip.mes.datamodel;
using System.Data;
using gip.core.media;

namespace gip.bso.logistics
{
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Picking scheduler'}de{'Kommission Zeitplaner'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + VD.Picking.ClassName)]
    public class BSOPickingScheduler : BSOWorkflowSchedulerBase
    {
        #region const
        public const string BGWorkerMethod_DoBackwardScheduling = @"DoBackwardScheduling";
        public const string BGWorkerMethod_DoForwardScheduling = @"DoForwardScheduling";
        public const string BGWorkerMethod_DoCalculateAll = @"DoCalculateAll";
        #endregion

        #region Configuration

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

        private ACPropertyConfigValue<bool> _ValidateBeforeStart;
        [ACPropertyConfig("en{'Validate batch plan on start'}de{'Validierung des Batchplans bei Start'}")]
        public bool ValidateBeforeStart
        {
            get
            {
                return _ValidateBeforeStart.ValueT;
            }
            set
            {
                _ValidateBeforeStart.ValueT = value;
            }
        }

        protected override string LoadPABatchPlanSchedulerURL()
        {
            string acUrl = @"\Planning\PickingScheduler";
            using (ACMonitor.Lock(DatabaseApp.ContextIPlus.QueryLock_1X000))
            {
                core.datamodel.ACClass paClass = DatabaseApp.ContextIPlus.ACClass.FirstOrDefault(c => c.ACIdentifier == nameof(PAPickingScheduler) && !c.ACProject.IsProduction);
                while (paClass != null)
                {
                    acUrl = paClass.ACURLComponentCached;
                    paClass = paClass.ACClass_BasedOnACClass.Where(c => c.ACProject.IsProduction).FirstOrDefault();
                }
            }
            return acUrl;
        }


        protected ACPropertyConfigValue<PickingStateEnum> _FilterPickingStateLess;
        [ACPropertyConfig("en{'Filter state less equal than'}de{'Filter Status klein gleich'}")]
        public virtual PickingStateEnum FilterPickingStateLess
        {
            get
            {
                return _FilterPickingStateLess.ValueT;
            }
            set
            {
                _FilterPickingStateLess.ValueT = value;
            }
        }


        protected ACPropertyConfigValue<PickingStateEnum> _FilterPickingStateGreater;
        [ACPropertyConfig("en{'Filter state greater'}de{'Filter Status größer'}")]
        public virtual PickingStateEnum FilterPickingStateGreater
        {
            get
            {
                return _FilterPickingStateGreater.ValueT;
            }
            set
            {
                _FilterPickingStateGreater.ValueT = value;
            }
        }

        #endregion

        #region c´tors

        public BSOPickingScheduler(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _ShowImages = new ACPropertyConfigValue<bool>(this, nameof(ShowImages), false);
            _ValidateBeforeStart = new ACPropertyConfigValue<bool>(this, nameof(ValidateBeforeStart), false);
            _FilterPickingStateLess = new ACPropertyConfigValue<PickingStateEnum>(this, nameof(FilterPickingStateLess), PickingStateEnum.InProcess);
            _FilterPickingStateGreater = new ACPropertyConfigValue<PickingStateEnum>(this, nameof(FilterPickingStateGreater), PickingStateEnum.WaitOnManualClosing);

            _RMISubscr = new ACPointAsyncRMISubscr(this, "RMISubscr", 1);
        }

        #region c´tors -> ACInit

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            _PickingManager = ACPickingManager.ACRefToServiceInstance(this);
            if (_PickingManager == null)
                throw new Exception("PickingManager not configured");

            _SchedulingForecastManager = SchedulingForecastManager.ACRefToServiceInstance(this);
            if (_SchedulingForecastManager == null)
                throw new Exception("SchedulingForecastManager not configured");

            MediaController = ACMediaController.GetServiceInstance(this);

            //if (FilterTabPickingOrderList != null)
            //    SelectedFilterTabPickingOrder = FilterTabPickingOrderList.Where(c => (SchedulerPickingSortFilterEnum)c.Value == SchedulerPickingSortFilterEnum.StartTime).FirstOrDefault();

            if (!base.ACInit(startChildMode))
                return false;

            _ = ShowImages;
            _ = ValidateBeforeStart;

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_PickingManager != null)
                ACPickingManager.DetachACRefFromServiceInstance(this, _PickingManager);
            _PickingManager = null;

            if (_SchedulingForecastManager != null)
                SchedulingForecastManager.DetachACRefFromServiceInstance(this, _SchedulingForecastManager);
            _SchedulingForecastManager = null;

            MediaController = null;
            SelectedPicking = null;

            return base.ACDeInit(deleteACClassTask);
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(ShowPickingsOnTimeline):
                    ShowPickingsOnTimeline();
                    return true;
                case nameof(MoveToOtherLine):
                    MoveToOtherLine();
                    return true;
                case nameof(IsEnabledMoveToOtherLine):
                    result = IsEnabledMoveToOtherLine();
                    return true;
                case nameof(NavigateToPicking):
                    NavigateToPicking();
                    return true;
                case nameof(IsEnabledNavigateToPicking):
                    result = IsEnabledNavigateToPicking();
                    return true;
                case nameof(NavigateToVoucher):
                    NavigateToVoucher();
                    return true;
                case nameof(IsEnabledNavigateToVoucher):
                    result = IsEnabledNavigateToVoucher();
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
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        private void ChildBSO_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
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

        public ACMediaController MediaController { get; set; }

        #endregion

        #region Properties

        #region Properties -> Explorer

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
                PickingStateEnum lessEqualState = PickingStateEnum.InProcess;
                
                if (
                        SelectedScheduleForPWNode == null 
                        || PickingList == null 
                        || !PickingList.Any(c => c.IsSelected && c.PickingState <= lessEqualState)
                   )
                    return null;
                
                Guid[] matchedSchedunlingGroups =
                    PickingList
                    .Where(c => c.IsSelected && c.PickingState <= lessEqualState)
                    .Select(c=>c.ACClassMethod)
                    .SelectMany(c=>c.ACClassWF_ACClassMethod)
                    .SelectMany(c=>c.MDSchedulingGroupWF_VBiACClassWF)
                    .Select(c=>c.MDSchedulingGroupID)
                    .Distinct()
                    .ToArray();
                
                return
                    ScheduleForPWNodeList
                    .Where(c =>
                                c.MDSchedulingGroupID != SelectedScheduleForPWNode.MDSchedulingGroupID
                                && matchedSchedunlingGroups.Contains(c.MDSchedulingGroupID)
                           )
                    .ToList();
            }
        }

        #endregion

        #endregion

        #region Properties -> (Tab)BatchPlanScheduler

        #region Properties -> (Tab)BatchPlanScheduler -> Filter (Search)

        /// <summary>
        /// Source Property: 
        /// </summary>
        private bool _FilterPickingSelectAll;
        [ACPropertyInfo(999, "FilterPickingSelectAll", ConstApp.SelectAll)]
        public bool FilterPickingSelectAll
        {
            get
            {
                return _FilterPickingSelectAll;
            }
            set
            {
                if (_FilterPickingSelectAll != value)
                {
                    _FilterPickingSelectAll = value;
                    OnPropertyChanged(nameof(FilterPickingSelectAll));

                    foreach (var item in PickingList)
                        item.IsSelected = value;

                    OnPropertyChanged(nameof(PickingList));
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilterPickingProgramNo;
        [ACPropertyInfo(999, "FilterPickingProgramNo", ConstApp.PickingNo)]
        public string FilterPickingProgramNo
        {
            get
            {
                return _FilterPickingProgramNo;
            }
            set
            {
                if (_FilterPickingProgramNo != value)
                {
                    _FilterPickingProgramNo = value;
                    OnPropertyChanged(nameof(FilterPickingProgramNo));
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilterPickingMaterialNo;
        [ACPropertyInfo(999, "FilterPickingMaterialNo", ConstApp.Material)]
        public string FilterPickingMaterialNo
        {
            get
            {
                return _FilterPickingMaterialNo;
            }
            set
            {
                if (_FilterPickingMaterialNo != value)
                {
                    _FilterPickingMaterialNo = value;
                    OnPropertyChanged(nameof(FilterPickingMaterialNo));
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private bool _FilterOnlyOnThisLine;
        [ACPropertySelected(999, "FilterOnlyOnThisLine", "en{'Pickings on this line'}de{'Kommissionierungen auf dieser Linie'}")]
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

        #region Properties -> (Tab)BatchPlanScheduler -> Picking

        private PickingPlanWrapper _SelectedPicking;
        [ACPropertySelected(505, "Picking")]
        public PickingPlanWrapper SelectedPicking
        {
            get => _SelectedPicking;
            set
            {
                bool changed = false;
                if (_SelectedPicking != value)
                {
                    changed = true;
                    if (_SelectedPicking != null)
                        _SelectedPicking.PropertyChanged -= _SelectedPicking_PropertyChanged;
                }
                _SelectedPicking = value;
                if (changed && _SelectedPicking != null)
                    _SelectedPicking.PropertyChanged += _SelectedPicking_PropertyChanged;
                OnPropertyChanged(nameof(SelectedPicking));
            }
        }

        private void _SelectedPicking_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PickingPlanWrapper.IsSelected))
            {
                OnPropertyChanged(nameof(TargetScheduleForPWNodeList));
            }
        }

        private ObservableCollection<PickingPlanWrapper> _PickingList;
        [ACPropertyList(506, "Picking")]
        public ObservableCollection<PickingPlanWrapper> PickingList
        {
            get
            {
                if (_PickingList == null)
                    _PickingList = new ObservableCollection<PickingPlanWrapper>();
                return _PickingList;
            }
            protected set
            {
                _PickingList = value;
                OnPropertyChanged(nameof(PickingList));
            }
        }

        //private bool _IsRefreshingBatchPlan = false;
        private ObservableCollection<PickingPlanWrapper> GetPickingList(Guid? mdSchedulingGroupID)
        {
            if (!mdSchedulingGroupID.HasValue)
                return new ObservableCollection<PickingPlanWrapper>();

            PickingStateEnum lessEqualState = FilterPickingStateLess;
            VD.PickingStateEnum greaterEqualState = FilterPickingStateGreater;
            ObservableCollection<PickingPlanWrapper> scheduledPickings = null;
            try
            {
                //_IsRefreshingBatchPlan = true;
                scheduledPickings = new ObservableCollection<PickingPlanWrapper>(
                    PickingManager
                    .GetScheduledPickings(
                        DatabaseApp,
                        mdSchedulingGroupID,
                        greaterEqualState,
                        lessEqualState,
                        FilterStartTime,
                        FilterEndTime,
                        FilterPickingProgramNo,
                        FilterPickingMaterialNo)
                    .AsEnumerable()
                    .Select(c => CreateNewPickingPlanWrapper(c)));
            }
            catch (Exception ex)
            {
                this.Messages.LogException(this.GetACUrl(), "GetPickingList(10)", ex);
                this.Messages.LogException(this.GetACUrl(), "GetPickingList(10)", ex.StackTrace);
            }
            finally
            {
                //_IsRefreshingBatchPlan = false;
            }
            if (scheduledPickings != null)
            {
                if (ShowImages)
                {
                    foreach (var picking in scheduledPickings)
                    {
                        if (picking.Material != null)
                            MediaController.LoadIImageInfo(picking.Material);
                    }
                }
            }
            return scheduledPickings;
        }

        protected virtual PickingPlanWrapper CreateNewPickingPlanWrapper(Picking picking)
        {
            return new PickingPlanWrapper(picking);
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

        #endregion

        #region Properties -> Tab Picking without Workflow

        //#region Properties -> Tab Picking -> Filter

        ///// <summary>
        ///// Select all
        ///// </summary>
        //private bool _FilterTabPickingSelectAll;
        //[ACPropertyInfo(999, "FilterTabPickingSelectAll", "en{'Select all'}de{'Alles auswählen'}")]
        //public bool FilterTabPickingSelectAll
        //{
        //    get
        //    {
        //        return _FilterTabPickingSelectAll;
        //    }
        //    set
        //    {
        //        if (_FilterTabPickingSelectAll != value)
        //        {
        //            _FilterTabPickingSelectAll = value;
        //            OnPropertyChanged(nameof(FilterTabPickingSelectAll));

        //            foreach (var item in ProdOrderPartslistList)
        //                item.IsSelected = value;

        //            OnPropertyChanged(nameof(ProdOrderPartslistList));
        //        }
        //    }
        //}

        //private DateTime? _FilterTabPickingStartTime;
        ///// <summary>
        ///// filter from date
        ///// </summary>
        ///// <value>The selected </value>
        //[ACPropertyInfo(999, "FilterTabPickingStartTime", "en{'From'}de{'Von'}")]
        //public DateTime? FilterTabPickingStartTime
        //{
        //    get
        //    {
        //        return _FilterTabPickingStartTime;
        //    }
        //    set
        //    {
        //        if (_FilterTabPickingStartTime != value)
        //        {
        //            _FilterTabPickingStartTime = value;
        //            OnPropertyChanged(nameof(FilterTabPickingStartTime));
        //            OnPropertyChanged(nameof(FilterTabPickingEndTime));
        //        }
        //    }
        //}

        //private DateTime? _FilterTabPickingEndTime;
        ///// <summary>
        ///// Filter to date
        ///// </summary>
        ///// <value>The selected </value>
        //[ACPropertyInfo(999, "FilterTabPickingEndTime", "en{'To'}de{'Bis'}")]
        //public DateTime? FilterTabPickingEndTime
        //{
        //    get
        //    {
        //        return _FilterTabPickingEndTime;
        //    }
        //    set
        //    {
        //        if (_FilterTabPickingEndTime != value)
        //        {
        //            _FilterTabPickingEndTime = value;
        //            OnPropertyChanged(nameof(FilterTabPickingStartTime));
        //            OnPropertyChanged(nameof(FilterTabPickingEndTime));
        //        }
        //    }
        //}

        //private bool? _FilterTabPickingIsCompleted = false;
        ///// <summary>
        ///// Filter for finshed orders
        ///// </summary>
        ///// <value>The selected </value>
        //[ACPropertyInfo(999, "FilterTabPickingIsCompleted", "en{'Completed'}de{'Erledigt'}")]
        //public bool? FilterTabPickingIsCompleted
        //{
        //    get
        //    {
        //        return _FilterTabPickingIsCompleted;
        //    }
        //    set
        //    {
        //        if (_FilterTabPickingIsCompleted != value)
        //        {
        //            _FilterTabPickingIsCompleted = value;
        //            OnPropertyChanged(nameof(FilterTabPickingIsCompleted));
        //            OnPropertyChanged(nameof(FilterTabPickingStartTime));
        //            OnPropertyChanged(nameof(FilterTabPickingEndTime));
        //        }
        //    }
        //}

        //private string _FilterDepartmentUserName;
        ///// <summary>
        ///// Filter departement
        ///// </summary>
        ///// <value>The selected </value>
        //[ACPropertyInfo(999, "FilterDepartmentUserName", "en{'Department'}de{'Abteilung'}")]
        //public string FilterDepartmentUserName
        //{
        //    get
        //    {
        //        return _FilterDepartmentUserName;
        //    }
        //    set
        //    {
        //        if (_FilterDepartmentUserName != value)
        //        {
        //            _FilterDepartmentUserName = value;
        //            OnPropertyChanged(nameof(FilterDepartmentUserName));
        //        }
        //    }
        //}

        ///// <summary>
        ///// Filter Order numer
        ///// </summary>
        //private string _FilterTabPickingPickingNo;
        //[ACPropertyInfo(999, "FilterTabPickingPickingNo", ConstApp.ProdOrderProgramNo)]
        //public string FilterTabPickingPickingNo
        //{
        //    get
        //    {
        //        return _FilterTabPickingPickingNo;
        //    }
        //    set
        //    {
        //        if (_FilterTabPickingPickingNo != value)
        //        {
        //            _FilterTabPickingPickingNo = value;
        //            OnPropertyChanged(nameof(FilterTabPickingPickingNo));
        //        }
        //    }
        //}

        ///// <summary>
        ///// Filter Material number
        ///// </summary>
        //private string _FilterTabPickingMaterialNo;
        //[ACPropertyInfo(999, "FilterTabPickingMaterialNo", ConstApp.Material)]
        //public string FilterTabPickingMaterialNo
        //{
        //    get
        //    {
        //        return _FilterTabPickingMaterialNo;
        //    }
        //    set
        //    {
        //        if (_FilterTabPickingMaterialNo != value)
        //        {
        //            _FilterTabPickingMaterialNo = value;
        //            OnPropertyChanged(nameof(FilterTabPickingMaterialNo));
        //        }
        //    }
        //}


        //#region Properties -> Filter Tab PickingOrder


        //public SchedulerPickingSortFilterEnum? FilterTabPickingOrder
        //{
        //    get
        //    {
        //        if (SelectedFilterTabPickingOrder == null)
        //            return null;
        //        return (SchedulerPickingSortFilterEnum)SelectedFilterTabPickingOrder.Value;
        //    }
        //}


        //private ACValueItem _SelectedFilterTabPickingOrder;
        ///// <summary>
        ///// Selected property for ACValueItem
        ///// </summary>
        ///// <value>The selected FilterTabPickingOrder</value>
        //[ACPropertySelected(305, "FilterTabPickingOrder", "en{'Sort order'}de{'Sortierreihenfolge'}")]
        //public ACValueItem SelectedFilterTabPickingOrder
        //{
        //    get
        //    {
        //        return _SelectedFilterTabPickingOrder;
        //    }
        //    set
        //    {
        //        if (_SelectedFilterTabPickingOrder != value)
        //        {
        //            _SelectedFilterTabPickingOrder = value;
        //            OnPropertyChanged(nameof(SelectedFilterTabPickingOrder));
        //        }
        //    }
        //}

        //private List<ACValueItem> _FilterTabPickingOrderList;
        ///// <summary>
        ///// List property for ACValueItem
        ///// </summary>
        ///// <value>The FilterPickingState list</value>
        //[ACPropertyList(306, "FilterTabPickingOrder")]
        //public List<ACValueItem> FilterTabPickingOrderList
        //{
        //    get
        //    {
        //        if (_FilterTabPickingOrderList == null)
        //            _FilterTabPickingOrderList = LoadFilterTabPickingOrderList();
        //        return _FilterTabPickingOrderList;
        //    }
        //}

        //public ACValueItemList LoadFilterTabPickingOrderList()
        //{
        //    ACValueItemList list = null;
        //    gip.core.datamodel.ACClass enumClass = Database.ContextIPlus.GetACType(typeof(SchedulerPickingSortFilterEnum));
        //    if (enumClass != null && enumClass.ACValueListForEnum != null)
        //        list = enumClass.ACValueListForEnum;
        //    else
        //        list = new ACValueListSchedulerPickingSortFilterEnum();
        //    return list;
        //}

        //#endregion

        //#endregion

        #region Tab PickingOrder without Workflow

        //private ProdOrderPartslistPlanWrapper _SelectedProdOrderPartslist;
        //[ACPropertySelected(507, "ProdOrderPartslist")]
        //public ProdOrderPartslistPlanWrapper SelectedProdOrderPartslist
        //{
        //    get => _SelectedProdOrderPartslist;
        //    set
        //    {
        //        _SelectedProdOrderPartslist = value;
        //        OnPropertyChanged(nameof(SelectedProdOrderPartslist));
        //    }
        //}

        //private IEnumerable<ProdOrderPartslistPlanWrapper> _ProdOrderPartslistList;
        //[ACPropertyList(507, "ProdOrderPartslist")]
        //public IEnumerable<ProdOrderPartslistPlanWrapper> ProdOrderPartslistList
        //{
        //    get
        //    {
        //        if (_ProdOrderPartslistList == null)
        //            _ProdOrderPartslistList = new List<ProdOrderPartslistPlanWrapper>();
        //        return _ProdOrderPartslistList;
        //    }
        //    set
        //    {
        //        _ProdOrderPartslistList = value;
        //        OnPropertyChanged(nameof(ProdOrderPartslistList));
        //    }
        //}

        //public virtual IEnumerable<ProdOrderPartslistPlanWrapper> GetProdOrderPartslistList()
        //{
        //    if (SelectedScheduleForPWNode == null)
        //        return new List<ProdOrderPartslistPlanWrapper>();

        //    MDProdOrderState.ProdOrderStates? minProdOrderState = null;
        //    MDProdOrderState.ProdOrderStates? maxProdOrderState = null;
        //    if (FilterTabPickingIsCompleted != null)
        //    {
        //        if (FilterTabPickingIsCompleted.Value)
        //            minProdOrderState = MDProdOrderState.ProdOrderStates.ProdFinished;
        //        else
        //            maxProdOrderState = MDProdOrderState.ProdOrderStates.InProduction;
        //    }

        //    ObjectQuery<ProdOrderPartslistPlanWrapper> batchQuery =
        //        s_cQry_ProdOrderPartslistForPWNode(
        //            DatabaseApp,
        //            SelectedScheduleForPWNode.MDSchedulingGroupID,
        //            FilterPlanningMR?.PlanningMRID,
        //            FilterTabPickingStartTime,
        //            FilterTabPickingEndTime,
        //            (short?)minProdOrderState,
        //            (short?)maxProdOrderState,
        //            FilterOnlyOnThisLine,
        //            FilterDepartmentUserName,
        //            FilterTabPickingPickingNo,
        //            FilterTabPickingMaterialNo) as ObjectQuery<ProdOrderPartslistPlanWrapper>;

        //    batchQuery.MergeOption = MergeOption.OverwriteChanges;
        //    IOrderedQueryable<ProdOrderPartslistPlanWrapper> query = batchQuery;
        //    if (FilterTabPickingOrder != null)
        //    {
        //        switch (FilterTabPickingOrder)
        //        {
        //            case SchedulerPickingSortFilterEnum.Material:
        //                query = batchQuery.OrderBy(c => c.ProdOrderPartslist.Partslist.Material.MaterialNo).ThenBy(c => c.ProdOrderPartslist.StartDate);
        //                break;
        //            case SchedulerPickingSortFilterEnum.StartTime:
        //                query = batchQuery.OrderBy(c => c.ProdOrderPartslist.StartDate);
        //                break;
        //            case SchedulerPickingSortFilterEnum.ProgramNo:
        //                query = batchQuery.OrderBy(c => c.ProdOrderPartslist.ProdOrder.ProgramNo);
        //                break;
        //        }
        //    }
        //    return query.Take(Const_MaxResultSize).ToList();
        //}

        //protected static readonly Func<DatabaseApp, Guid, Guid?, DateTime?, DateTime?, short?, short?, bool, string, string, string, IQueryable<ProdOrderPartslistPlanWrapper>> s_cQry_ProdOrderPartslistForPWNode =
        //CompiledQuery.Compile<DatabaseApp, Guid, Guid?, DateTime?, DateTime?, short?, short?, bool, string, string, string, IQueryable<ProdOrderPartslistPlanWrapper>>(
        //    (ctx, mdSchedulingGroupID, planningMRID, filterStartTime, filterEndTime, minProdOrderState, maxProdOrderState, filterOnlyOnThisLine, departmentUserName, programNo, materialNo) =>
        //        ctx
        //        .ProdOrderPartslist
        //        .Include("MDProdOrderState")
        //        .Include("ProdOrder")
        //        .Include("Partslist")
        //        .Include("Partslist.Material")
        //        .Include("Partslist.Material.BaseMDUnit")
        //        //.Include("Partslist.Material.MaterialUnit_Material")
        //        //.Include("Partslist.Material.MaterialUnit_Material.ToMDUnit")
        //        .Where(c =>
        //                (minProdOrderState == null || (c.MDProdOrderState.MDProdOrderStateIndex >= minProdOrderState && c.ProdOrder.MDProdOrderState.MDProdOrderStateIndex >= minProdOrderState))
        //                && (programNo == null || (c.ProdOrder.ProgramNo.Contains(programNo)))
        //                && (
        //                        string.IsNullOrEmpty(materialNo)
        //                        || (c.Partslist.Material.MaterialNo.Contains(materialNo) || c.Partslist.Material.MaterialName1.Contains(materialNo))
        //                    )
        //                && (maxProdOrderState == null || (c.MDProdOrderState.MDProdOrderStateIndex <= maxProdOrderState && c.ProdOrder.MDProdOrderState.MDProdOrderStateIndex <= maxProdOrderState))
        //                && (
        //                        (!planningMRID.HasValue && !c.PlanningMRProposal_ProdOrderPartslist.Any())
        //                        || (planningMRID.HasValue && c.PlanningMRProposal_ProdOrderPartslist.Any(x => x.PlanningMRID == planningMRID))
        //                )
        //                && (
        //                    filterStartTime == null
        //                    ||
        //                    c.StartDate >= filterStartTime
        //                )
        //                && (
        //                    filterEndTime == null
        //                    ||
        //                    c.StartDate < filterEndTime
        //                )

        //                &&
        //                (
        //                    (!filterOnlyOnThisLine && c
        //                    .Partslist
        //                    .PartslistACClassMethod_Partslist
        //                    .Where(d => d
        //                                .MaterialWFACClassMethod
        //                                .ACClassMethod.ACClassWF_ACClassMethod
        //                                .SelectMany(x => x.MDSchedulingGroupWF_VBiACClassWF)
        //                                .Where(x => x.MDSchedulingGroupID == mdSchedulingGroupID)
        //                                .Any()
        //                    ).Any())
        //                ||
        //                    (filterOnlyOnThisLine &&
        //                    c
        //                    .Picking_ProdOrderPartslist
        //                    .Where(d => d
        //                            .VBiACClassWF
        //                            .MDSchedulingGroupWF_VBiACClassWF
        //                            .Where(x => x.MDSchedulingGroupID == mdSchedulingGroupID)
        //                            .Any()
        //                    ).Any())
        //                )
        //            && (departmentUserName == null || c.DepartmentUserName.Contains(departmentUserName))
        //        )
        //        .Select(c => new ProdOrderPartslistPlanWrapper()
        //        {
        //            ProdOrderPartslist = c,
        //            PlannedQuantityUOM = c.Picking_ProdOrderPartslist.Any() ? c.Picking_ProdOrderPartslist.Sum(d => d.TotalSize) : 0.0
        //        })
        //);


        #endregion

        #endregion

        #region Properties -> BatchPlanTimelinePresenter

        private IEnumerable<PickingTimelineItem> _PickingTimelineItemsRoot;
        /// <summary>
        /// Gets or sets the ACPropertyLogsRoot. Represents the list for treeListView control.
        /// </summary>
        [ACPropertyList(519, "PickingTimelineItemsRoot")]
        public IEnumerable<PickingTimelineItem> PickingTimelineItemsRoot
        {
            get
            {
                return _PickingTimelineItemsRoot;
            }
            set
            {
                _PickingTimelineItemsRoot = value;
                OnPropertyChanged(nameof(PickingTimelineItemsRoot));
            }
        }

        private PickingTimelineItem _SelectedPickingTimelineItemRoot;
        /// <summary>
        /// Gets or sets the selected PropertyLog root. (Selected in treeListView control.)
        /// </summary>
        [ACPropertyCurrent(520, "PickingTimelineItemsRoot")]
        public PickingTimelineItem SelectedPickingTimelineItemRoot
        {
            get
            {
                return _SelectedPickingTimelineItemRoot;
            }
            set
            {
                _SelectedPickingTimelineItemRoot = value;
                OnPropertyChanged(nameof(SelectedPickingTimelineItemRoot));
            }
        }

        private ObservableCollection<PickingTimelineItem> _PickingTimelineItems;
        /// <summary>
        /// Gets or sets the ACPropertyLogs. Represents the collection for timeline view control. Contains all timeline items(ACPropertyLogModel).
        /// </summary>
        [ACPropertyList(521, "PickingTimelineItems")]
        public ObservableCollection<PickingTimelineItem> PickingTimelineItems
        {
            get
            {

                return _PickingTimelineItems;
            }
            set
            {
                _PickingTimelineItems = value;
                OnPropertyChanged(nameof(PickingTimelineItems));
            }
        }

        private PickingTimelineItem _SelectedPickingTimelineItem;
        /// <summary>
        /// Gets or sets the seelcted PropertyLog. (Selected in the timeline view control)
        /// </summary>
        [ACPropertyCurrent(522, "PickingTimelineItems")]
        public PickingTimelineItem SelectedPickingTimelineItem
        {
            get
            {
                return _SelectedPickingTimelineItem;
            }
            set
            {
                _SelectedPickingTimelineItem = value;
                OnPropertyChanged(nameof(SelectedPickingTimelineItem));
            }
        }

        #endregion

        #region Properties => RMI and Routing

        ACPointAsyncRMISubscr _RMISubscr;
        [ACPropertyAsyncMethodPointSubscr(9999, false, 0, "RMICallback")]
        public ACPointAsyncRMISubscr RMISubscr
        {
            get
            {
                return _RMISubscr;
            }
        }

        private string _CalculateRouteResult;
        [ACPropertyInfo(9999,"","")]
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

        #region Methods -> Explorer

        #region Methods -> Explorer -> MoveToOtherLine

        [ACMethodInteraction("MoveToOtherLine", "en{'Move'}de{'Verlagern'}", (short)MISort.MovedData, false, "TargetScheduleForPWNode")]
        public void MoveToOtherLine()
        {
            if (!IsEnabledMoveToOtherLine())
                return;
            ClearMessages();
            bool isSaveValid = false;
            bool saveBeforeMove = LocalSaveChanges();
            PickingPlanWrapper[] selectedPickingPlans = new PickingPlanWrapper[] { };
            if (saveBeforeMove)
            {
                PickingPlanWrapper[] notSelectedPickingPlans = PickingList.Where(c => !c.IsSelected).ToArray();
                ObservableCollection<PickingPlanWrapper> itemsOnTargetLineP = GetPickingList(SelectedTargetScheduleForPWNode.MDSchedulingGroupID);
                List<PickingPlanWrapper> itemsOnTargetLine = itemsOnTargetLineP.ToArray().ToList();
                selectedPickingPlans = PickingList.Where(c => c.IsSelected).ToArray();
                foreach (PickingPlanWrapper selectedPickingPlan in selectedPickingPlans)
                {
                    VD.ACClassWF matchWf =
                        selectedPickingPlan
                        .Picking
                        .ACClassMethod
                        .ACClassWF_ACClassMethod
                        .SelectMany(c=>c.MDSchedulingGroupWF_VBiACClassWF)
                        .Where(c => c.MDSchedulingGroupID == SelectedTargetScheduleForPWNode.MDSchedulingGroupID)
                        .Select(c=>c.VBiACClassWF)
                        .FirstOrDefault();

                    if(matchWf != null)
                    {
                        selectedPickingPlan.Picking.VBiACClassWF = matchWf;
                    }
                }

                // Correct sort order on original place
                MoveBatchSortOrderCorrect(notSelectedPickingPlans);

                itemsOnTargetLine.AddRange(selectedPickingPlans);
                MoveBatchSortOrderCorrect(itemsOnTargetLine);

                isSaveValid = LocalSaveChanges();
            }
            else
            {
                DatabaseApp.ACUndoChanges();
            }
            if (isSaveValid)
            {
                RefreshServerState(SelectedScheduleForPWNode.MDSchedulingGroupID);
                RefreshServerState(SelectedTargetScheduleForPWNode.MDSchedulingGroupID);
                PickingList = GetPickingList(SelectedScheduleForPWNode.MDSchedulingGroupID);
                SelectedPicking = PickingList.FirstOrDefault();
                OnPropertyChanged(nameof(PickingList));
                foreach (PickingPlanWrapper selectedPickingPlan in selectedPickingPlans)
                {
                    selectedPickingPlan.IsSelected = false;
                }
            }
        }

        public bool IsEnabledMoveToOtherLine()
        {
            PickingStateEnum lessEqualState = PickingStateEnum.InProcess;
            return
                PickingList != null
                && PickingList.Where(c => c.PickingState <= lessEqualState && c.IsSelected).Any()
                && SelectedTargetScheduleForPWNode != null
                && SelectedTargetScheduleForPWNode.MDSchedulingGroup != null;
        }

        #region Methods -> Explorer -> MoveToOtherLine -> Helper methods

        private void MoveBatchSortOrderCorrect()
        {
            List<PickingPlanWrapper> notSelected =
                   PickingList
                   .Where(c => !c.IsSelected && c.EntityState != EntityState.Deleted && c.EntityState != EntityState.Detached)
                   .OrderBy(c => c.ScheduledOrder ?? 0)
                   .ThenBy(c => c.InsertDate)
                   .ToList();
            MoveBatchSortOrderCorrect(notSelected);
        }

        private void MoveBatchSortOrderCorrect(IEnumerable<PickingPlanWrapper> prodOrderBatchPlans)
        {
            int nr = 0;
            foreach (PickingPlanWrapper item in prodOrderBatchPlans)
            {
                nr++;
                item.ScheduledOrder = nr;
            }
        }
        #endregion

        #endregion

        #endregion

        #region Methods -> (Tab)BatchPlanScheduler

        #region Methods -> (Tab)BatchPlanScheduler -> New, Delete, Search

        public override bool IsEnabledSearch()
        {
            return SelectedScheduleForPWNode != null
                    && FilterStartTime != null
                    && FilterEndTime != null
                    && (FilterEndTime.Value - FilterStartTime.Value).TotalDays > 0
                    && (FilterEndTime.Value - FilterStartTime.Value).TotalDays <= Const_MaxFilterDaySpan;
        }

        #endregion

        #region Methods -> (Tab)BatchPlanScheduler -> Interaction

        public override void ItemDrag(Dictionary<int, string> newOrder)
        {
            if (!IsEnabledItemDrag())
                return;
            Dictionary<int, Guid> revisitedNewOrder = newOrder.ToDictionary(key => key.Key, val => new Guid(val.Value));
            var batchPlanList = PickingList.ToList();
            foreach (var item in revisitedNewOrder)
            {
                PickingPlanWrapper prodOrderBatchPlan = batchPlanList.FirstOrDefault(c => c.PickingID == item.Value);
                if (prodOrderBatchPlan != null && prodOrderBatchPlan.ScheduledOrder != item.Key)
                    prodOrderBatchPlan.ScheduledOrder = item.Key;
            }
            OnPropertyChanged(nameof(PickingList));
        }


        [ACMethodInteraction("Picking", "en{'Show Order'}de{'Auftrag anzeigen'}", 502, false, "SelectedPicking")]
        public void NavigateToPicking()
        {
            if (!IsEnabledNavigateToPicking())
                return;
            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(
                new PAOrderInfoEntry()
                {
                    EntityID = SelectedPicking.PickingID,
                    EntityName = Picking.ClassName
                });
                service.ShowDialogOrder(this, info);
                Load(true);
            }
        }

        public bool IsEnabledNavigateToPicking()
        {
            return SelectedPicking != null;
        }

        [ACMethodInteraction("Picking", "en{'Show Vistor voucher'}de{'Besucherbeleg anzeigen'}", 503, false, "SelectedPicking")]
        public void NavigateToVoucher()
        {
            if (!IsEnabledNavigateToVoucher())
                return;
            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(
                new PAOrderInfoEntry()
                {
                    EntityID = SelectedPicking.VisitorVoucher.VisitorVoucherID,
                    EntityName = VisitorVoucher.ClassName
                });
                service.ShowDialogOrder(this, info);
                Load(true);
            }
        }

        public bool IsEnabledNavigateToVoucher()
        {
            return SelectedPicking != null && SelectedPicking.VisitorVoucher != null;
        }

        [ACMethodInteraction("MoveSelectedBatchUp", "en{'Up'}de{'Oben'}", 602, false, "SelectedPicking")]
        public void MoveSelectedBatchUp()
        {
            if (!IsEnabledMoveSelectedBatchUp())
                return;

            PickingPlanWrapper[] plans = PickingList.ToArray();
            ScheduledOrderManager<PickingPlanWrapper>.MoveUp(plans);
            PickingList = new ObservableCollection<PickingPlanWrapper>(plans.OrderBy(c => c.ScheduledOrder));
        }

        public bool IsEnabledMoveSelectedBatchUp()
        {
            return
                PickingList != null
                && PickingList.Any(c => c.IsSelected);
        }

        [ACMethodInteraction("MoveSelectedBatchDown", "en{'Down'}de{'Unten'}", 603, false, "SelectedPicking")]
        public void MoveSelectedBatchDown()
        {
            if (!IsEnabledMoveSelectedBatchDown())
                return;

            PickingPlanWrapper[] plans = PickingList.ToArray();
            ScheduledOrderManager<PickingPlanWrapper>.MoveDown(plans);
            PickingList = new ObservableCollection<PickingPlanWrapper>(plans.OrderBy(c => c.ScheduledOrder));
        }

        public bool IsEnabledMoveSelectedBatchDown()
        {
            return IsEnabledMoveSelectedBatchUp();
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

        [ACMethodInfo("ForwardSchedulingOk", Const.Ok, 509)]
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

        [ACMethodInfo("SchedulingCancel", Const.Cancel, 510)]
        public void SchedulingCancel()
        {
            CloseTopDialog();
            ScheduledStartDate = null;
            ScheduledEndDate = null;
        }

        public bool IsEnabledScheduling()
        {
            return !BackgroundWorker.IsBusy && SelectedScheduleForPWNode != null && PickingList != null && PickingList.Any();
        }

        [ACMethodInfo("SchedulingCalculateAll", "en{'Calculate All'}de{'Calculate All'}", 511)]
        public void SchedulingCalculateAll()
        {
            if (BackgroundWorker.IsBusy) return;
            BackgroundWorker.RunWorkerAsync(BGWorkerMethod_DoCalculateAll);
            ShowDialog(this, DesignNameProgressBar);
        }

        #endregion

        #region Methods -> (Tab)BatchPlanScheduler -> BatchState

        [ACMethodCommand("SetBatchStateReadyToStart", "en{'Switch to Readiness'}de{'Startbereit setzen'}", (short)MISort.Start, true)]
        public void SetBatchStateReadyToStart()
        {
            if (!IsEnabledSetBatchStateReadyToStart())
                return;
            PickingPlanWrapper[] selectedBatches = PickingList.Where(c => c.IsSelected && c.PickingState < PickingStateEnum.WFReadyToStart)
                                                    .ToArray();
            SetReadyToStart(selectedBatches);
        }

        public bool IsEnabledSetBatchStateReadyToStart()
        {
            return PickingList != null
                && PickingList.Any(c => c.IsSelected && c.PickingState <= PickingStateEnum.InProcess);
        }

        private void SetReadyToStart(PickingPlanWrapper[] batchPlans)
        {
            bool? setReadyToLoad = null;
            using (var dbIPlus = new Database())
            {
                MDDelivPosLoadState loadState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(this.DatabaseApp, MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad).FirstOrDefault();
                MsgWithDetails msgWithDetails = new MsgWithDetails();
                foreach (PickingPlanWrapper picking in batchPlans)
                {
                    if (picking.PickingState > PickingStateEnum.InProcess)
                        continue;

                    Msg msg = null;
                    foreach (PickingPlanPosWrapper pickingPos in picking.Lines)
                    {
                        if (pickingPos.PickingPos.MDDelivPosLoadState.DelivPosLoadState == MDDelivPosLoadState.DelivPosLoadStates.NewCreated)
                        {
                            if (!setReadyToLoad.HasValue)
                            {
                                //Question50104: There are lines that are not set ready to start. Would you like to set all of them to ready to start?
                                Global.MsgResult msgResult = Messages.Question(this, "Question50104", Global.MsgResult.Yes);
                                setReadyToLoad = msgResult == Global.MsgResult.Yes;
                            }
                            if (setReadyToLoad.Value)
                                pickingPos.PickingPos.MDDelivPosLoadState = loadState;
                        }
                    }

                    if (!picking.Lines.Where(c => c.PickingPos.MDDelivPosLoadState.DelivPosLoadState <= MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad).Any())
                    {
                        // Info50093:The picking order {0} at position {1} no longer has any lines that can be started. Check whether the picking order can be completed or put a line in a ready-to-start state.
                        msg = new Msg(this, eMsgLevel.Info, "BSOPickingScheduler", "SetBatchStateReadyToStart()", 1000, "Info50093", picking.PickingNo, picking.ScheduledOrder);
                        msgWithDetails.AddDetailMessage(msg);
                        continue;
                    }

                    List<IACConfigStore> listOfSelectedStores = new List<IACConfigStore>() { picking.Picking };
                    msg = this.PickingManager.ValidateStart(this.DatabaseApp, dbIPlus, picking.Picking,
                                            listOfSelectedStores,
                                            PARole.ValidationBehaviour.Strict, picking.Picking.IplusVBiACClassWF, true);
                    if (msg != null)
                    {
                        if (!msg.IsSucceded())
                        {
                            // Error50609: The picking order {0} at position {1} is not startable
                            if (String.IsNullOrEmpty(msg.Message))
                                msg.Message = Root.Environment.TranslateMessage(this, "Error50609", picking.PickingNo, picking.ScheduledOrder);
                            msgWithDetails.AddDetailMessage(msg);
                            continue;
                        }
                        else if (msg.HasWarnings())
                        {
                            // Warning50069 The picking order {0} at position {1} has warnings. Would you still like to start the job?
                            if (String.IsNullOrEmpty(msg.Message))
                                msg.Message = Root.Environment.TranslateMessage(this, "Warning50069", picking.PickingNo, picking.ScheduledOrder);
                            var userResult = Messages.Msg(msg, Global.MsgResult.No, eMsgButton.YesNo);
                            if (userResult == Global.MsgResult.No || userResult == Global.MsgResult.Cancel)
                                continue;
                        }
                    }

                    picking.PickingState = VD.PickingStateEnum.WFReadyToStart;
                }

                if (msgWithDetails.MsgDetails.Any())
                {
                    Messages.Msg(msgWithDetails);
                }
                Save();
            }
            OnPropertyChanged(nameof(PickingList));
        }


        [ACMethodCommand("SetBatchStateCreated", "en{'Reset Readiness'}de{'Startbereitschaft rücksetzen'}", 508, true)]
        public void SetBatchStateCreated()
        {
            if (!IsEnabledSetBatchStateCreated())
                return;
            List<PickingPlanWrapper> selectedBatches = PickingList.Where(c => c.IsSelected).ToList();
            foreach (PickingPlanWrapper batchPlan in selectedBatches)
            {
                if (batchPlan.PickingState == PickingStateEnum.WFReadyToStart)
                {
                    batchPlan.PickingState = PickingStateEnum.InProcess;
                }
            }
            Save();
            OnPropertyChanged(nameof(PickingList));
        }

        public bool IsEnabledSetBatchStateCreated()
        {
            return PickingList != null && PickingList.Any(c => c.IsSelected && c.PickingState == PickingStateEnum.WFReadyToStart);
        }

        //public short[] NotAllowedStatesForBatchCancel
        //{
        //    get
        //    {
        //        return new short[]
        //        {
        //            (short)PickingStateEnum.ReadyToStart,
        //            (short)PickingStateEnum.AutoStart,
        //            (short)PickingStateEnum.InProcess
        //        };
        //    }
        //}

        //private void DoSetBatchStateCancelled(bool autoDeleteDependingBatchPlans, List<Picking> selectedBatches, ref List<Guid> groupsForRefresh)
        //{

        //    foreach (Picking batchPlan in selectedBatches)
        //    {
        //        batchPlan.AutoRefresh();
        //        batchPlan.FacilityReservation_Picking.AutoRefresh();
        //    }

        //    List<MaintainOrderInfo> maintainOrderInfos = new List<MaintainOrderInfo>();

        //    foreach (Picking batchPlan in selectedBatches)
        //    {
        //        if (NotAllowedStatesForBatchCancel.Contains(batchPlan.PlanStateIndex))
        //            return;

        //        var prodOrderPartslist = batchPlan.ProdOrderPartslist;
        //        MaintainOrderInfo maintainOrderInfo = maintainOrderInfos.Where(c => c.PO == prodOrderPartslist.ProdOrder).FirstOrDefault();
        //        if (maintainOrderInfo == null)
        //        {
        //            maintainOrderInfo = new MaintainOrderInfo();
        //            maintainOrderInfo.PO = prodOrderPartslist.ProdOrder;
        //            maintainOrderInfos.Add(maintainOrderInfo);
        //        }

        //        if (batchPlan.PlanState >= VD.PickingStateEnum.Paused
        //            || batchPlan.ProdOrderBatch_Picking.Any())
        //        {
        //            batchPlan.PlanState = PickingStateEnum.Cancelled;
        //            if (autoDeleteDependingBatchPlans)
        //                maintainOrderInfo.DeactivateAll = true;
        //            else
        //            {
        //                var mDProdOrderStateCancelled = DatabaseApp.s_cQry_GetMDProdOrderState(DatabaseApp, MDProdOrderState.ProdOrderStates.Cancelled).FirstOrDefault();
        //                if (prodOrderPartslist.MDProdOrderState.ProdOrderState != mDProdOrderStateCancelled.ProdOrderState)
        //                    SetProdorderPartslistState(mDProdOrderStateCancelled, null, prodOrderPartslist);
        //            }
        //        }
        //        else if (batchPlan.PlanState == PickingStateEnum.Created)
        //        {
        //            foreach (var reservation in batchPlan.FacilityReservation_Picking.ToArray())
        //            {
        //                batchPlan.FacilityReservation_Picking.Remove(reservation);
        //                reservation.DeleteACObject(this.DatabaseApp, true);
        //            }
        //            Guid mdSchedulingGroupID = batchPlan.VBiACClassWF.MDSchedulingGroupWF_VBiACClassWF.Select(x => x.MDSchedulingGroupID).FirstOrDefault();
        //            if (!groupsForRefresh.Contains(mdSchedulingGroupID))
        //                groupsForRefresh.Add(mdSchedulingGroupID);

        //            batchPlan.DeleteACObject(this.DatabaseApp, true);
        //            prodOrderPartslist.Picking_ProdOrderPartslist.Remove(batchPlan);
        //            if (autoDeleteDependingBatchPlans && !prodOrderPartslist.Picking_ProdOrderPartslist.Any())
        //                maintainOrderInfo.RemoveAll = true;
        //        }
        //        else
        //        {
        //            if (autoDeleteDependingBatchPlans)
        //                maintainOrderInfo.DeactivateAll = true;
        //        }
        //    }

        //    MaintainProdOrderState(maintainOrderInfos);

        //    MoveBatchSortOrderCorrect();

        //    LocalSaveChanges();

        //}

        //private void DoRefreshLinesAfterBatchDelete(List<Guid> groupsForRefresh)
        //{
        //    RefreshScheduleForSelectedNode();
        //    OnPropertyChanged(nameof(PickingList));
        //    OnPropertyChanged(nameof(ProdOrderPartslistList));
        //    if (!IsBSOTemplateScheduleParent)
        //    {
        //        if (groupsForRefresh.Any())
        //        {
        //            foreach (var mdSchedulingGroupID in groupsForRefresh)
        //            {
        //                RefreshServerState(mdSchedulingGroupID);
        //            }
        //        }
        //    }
        //}


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
        //            List<Picking> selectedBatches = PickingList.Where(c => c.IsSelected).ToList();
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
        //        PickingList != null
        //        && PickingList.Any(c => c.IsSelected)
        //        && !PickingList.Where(c => c.IsSelected).Any(c =>
        //              NotAllowedStatesForBatchCancel.Contains(c.PlanStateIndex)
        //        );
        //}

        /// <summary>
        /// Method for new behavior deleting batch plans
        /// for test
        /// </summary>
        //[ACMethodCommand("SetBatchStateCancelled", "en{'Deactivate and remove'}de{'Deaktivieren und Entfernen'}", (short)MISort.Start, true)]
        //public void SetBatchStateCancelled()
        //{
        //    ClearMessages();
        //    if (!IsEnabledSetBatchStateCancelled())
        //        return;
        //    if (Messages.Question(this, "Question50084", Global.MsgResult.Yes) == Global.MsgResult.Yes)
        //    {
        //        try
        //        {
        //            List<Guid> groupsForRefresh = new List<Guid>();

        //            List<Picking> selectedBatches = PickingList.Where(c => c.IsSelected).ToList();
        //            Dictionary<ProdOrderPartslistPos, double> positions =
        //                selectedBatches
        //                .ToDictionary(key => key.ProdOrderPartslistPos, val => val.BatchTargetCount * val.BatchSize);

        //            bool isSetBatchStateCancelledForTreeDelete = IsSetBatchStateCancelledForTreeDelete();
        //            if (isSetBatchStateCancelledForTreeDelete)
        //            {
        //                Global.MsgResult answer = Messages.YesNoCancel(this, "Question50093");
        //                if (answer == Global.MsgResult.Yes)
        //                {
        //                    // Silent delete batch plans for current ProdOrder
        //                    OnSetBatchStateCanceling(positions);
        //                    DoSetBatchStateCancelled(true, selectedBatches, ref groupsForRefresh);
        //                    DoRefreshLinesAfterBatchDelete(groupsForRefresh);
        //                }
        //                else if (answer == Global.MsgResult.No)
        //                {
        //                    // Wizard delete batch plans for current ProdOrder
        //                    if (SelectedPicking != null)
        //                    {
        //                        IsWizard = true;
        //                        WizardPhase = NewScheduledBatchWizardPhaseEnum.DeleteBatchPlan;
        //                        WizardForwardAction(wizardPhase: NewScheduledBatchWizardPhaseEnum.DeleteBatchPlan);
        //                        OnPropertyChanged(nameof(CurrentLayout));
        //                        OnPropertyChanged(nameof(WizardSchedulerPartslistList));
        //                    }
        //                }
        //                else
        //                {
        //                    // do nothing
        //                }
        //            }
        //            else
        //            {
        //                // delete batches as usuall (on current linie)
        //                bool autoDeleteDependingBatchPlans = AutoRemoveMDSGroupFrom > 0
        //                                                && AutoRemoveMDSGroupTo > 0
        //                                                && SelectedScheduleForPWNode != null
        //                                                && SelectedScheduleForPWNode.MDSchedulingGroup != null
        //                                                && SelectedScheduleForPWNode.MDSchedulingGroup.MDSchedulingGroupIndex >= AutoRemoveMDSGroupFrom
        //                                                && SelectedScheduleForPWNode.MDSchedulingGroup.MDSchedulingGroupIndex <= AutoRemoveMDSGroupTo;
        //                OnSetBatchStateCanceling(positions);
        //                DoSetBatchStateCancelled(autoDeleteDependingBatchPlans, selectedBatches, ref groupsForRefresh);
        //                DoRefreshLinesAfterBatchDelete(groupsForRefresh);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            string msg = "";
        //            Exception tmpEx = ex;
        //            while (tmpEx != null)
        //            {
        //                msg += tmpEx.Message;
        //                tmpEx = tmpEx.InnerException;
        //            }

        //            Msg errMsg = new Msg() { MessageLevel = eMsgLevel.Error, ACIdentifier = nameof(SetBatchStateCancelled), Message = msg };
        //            SendMessage(errMsg);
        //        }

        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="positions">
        /// Dictionary with pos canceled batch plans + batch plan quantity
        /// </param>
        public virtual void OnSetBatchStateCanceling(Dictionary<ProdOrderPartslistPos, double> positions)
        {

        }

        //private bool IsSetBatchStateCancelledForTreeDelete()
        //{
        //    return
        //        SelectedPicking
        //        .ProdOrderPartslist
        //        .ProdOrder
        //        .ProdOrderPartslist_ProdOrder
        //        .SelectMany(c => c.Picking_ProdOrderPartslist)
        //        .AsEnumerable()
        //        .Where(c =>
        //                    !c.IsSelected
        //                    && c.PickingID != SelectedPicking.PickingID
        //                    && !NotAllowedStatesForBatchCancel.Contains(c.PlanStateIndex)
        //              )
        //        .Any();
        //}

        //public bool IsEnabledSetBatchStateCancelled()
        //{
        //    return
        //        PickingList != null
        //        && PickingList.Any(c => c.IsSelected)
        //        && !PickingList.Where(c => c.IsSelected).Any(c =>
        //              NotAllowedStatesForBatchCancel.Contains(c.PlanStateIndex)
        //        );
        //}

        #endregion

        #endregion

        #region Methods -> (Tab)ProdOrder

        #region Methods -> (Tab)ProdOrder -> Filter

        ///// <summary>
        ///// Source Property: SearchOrders
        ///// </summary>
        //[ACMethodInfo("SearchOrders", "en{'Search'}de{'Suchen'}", 999)]
        //public void SearchOrders()
        //{
        //    if (!IsEnabledSearchOrders())
        //        return;
        //    ProdOrderPartslistList = GetProdOrderPartslistList();
        //}

        //public bool IsEnabledSearchOrders()
        //{
        //    return
        //        (FilterTabPickingStartTime == null && FilterTabPickingEndTime == null && !(FilterTabPickingIsCompleted ?? true))
        //        ||
        //        (
        //            FilterTabPickingStartTime != null
        //            && FilterTabPickingEndTime != null
        //            && (FilterTabPickingEndTime.Value - FilterTabPickingStartTime.Value).TotalDays > 0
        //        );
        //}


        //[ACMethodInteraction("NavigateToProdOrder2", "en{'Show Order'}de{'Auftrag anzeigen'}", 502, false, "SelectedProdOrderPartslist")]
        //public void NavigateToProdOrder2()
        //{
        //    PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
        //    if (service != null)
        //    {
        //        PAOrderInfo info = new PAOrderInfo();
        //        info.Entities.Add(
        //        new PAOrderInfoEntry()
        //        {
        //            EntityID = SelectedProdOrderPartslist.ProdOrderPartslist.ProdOrderPartslistID,
        //            EntityName = ProdOrderPartslist.ClassName
        //        });
        //        service.ShowDialogOrder(this, info);
        //    }
        //}

        //public bool IsEnabledNavigateToProdOrder2()
        //{
        //    return SelectedProdOrderPartslist != null && SelectedProdOrderPartslist.ProdOrderPartslist != null;
        //}


        #endregion

        #region Methods -> (Tab)ProdOrder -> Manipulate Batch Plan

        //public class MaintainOrderInfo
        //{
        //    public ProdOrder PO { get; set; }
        //    public bool RemoveAll { get; set; }
        //    public bool DeactivateAll { get; set; }
        //}

        //protected void MaintainProdOrderState(List<MaintainOrderInfo> prodOrders)
        //{
        //    MDProdOrderState mDProdOrderStateCompleted = DatabaseApp.s_cQry_GetMDProdOrderState(DatabaseApp, MDProdOrderState.ProdOrderStates.ProdFinished).FirstOrDefault();
        //    MDProdOrderState mDProdOrderStateCancelled = DatabaseApp.s_cQry_GetMDProdOrderState(DatabaseApp, MDProdOrderState.ProdOrderStates.Cancelled).FirstOrDefault();

        //    foreach (MaintainOrderInfo poInfo in prodOrders)
        //    {

        //        poInfo.PO.AutoRefresh();
        //        ProdOrderPartslist[] allProdPartslists = poInfo.PO.ProdOrderPartslist_ProdOrder.ToArray();
        //        bool canRemoveAll = true;

        //        if (poInfo.RemoveAll || poInfo.DeactivateAll)
        //        {
        //            foreach (Picking batchPlan2Remove in poInfo.PO.ProdOrderPartslist_ProdOrder.SelectMany(c => c.Picking_ProdOrderPartslist).ToArray())
        //            {
        //                var parentPOList = batchPlan2Remove.ProdOrderPartslist;
        //                if (batchPlan2Remove.PlanState == PickingStateEnum.Created)
        //                {
        //                    foreach (var reservation in batchPlan2Remove.FacilityReservation_Picking.ToArray())
        //                    {
        //                        reservation.DeleteACObject(this.DatabaseApp, true);
        //                        batchPlan2Remove.FacilityReservation_Picking.Remove(reservation);
        //                    }
        //                    batchPlan2Remove.DeleteACObject(this.DatabaseApp, true);
        //                    parentPOList.Picking_ProdOrderPartslist.Remove(batchPlan2Remove);
        //                }
        //                else
        //                {
        //                    if (poInfo.DeactivateAll)
        //                    {
        //                        if (batchPlan2Remove.PlanState < PickingStateEnum.ReadyToStart || batchPlan2Remove.PlanState > PickingStateEnum.InProcess)
        //                            batchPlan2Remove.PlanState = PickingStateEnum.Cancelled;
        //                    }
        //                    canRemoveAll = false;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            canRemoveAll = !poInfo.PO.ProdOrderPartslist_ProdOrder.SelectMany(c => c.Picking_ProdOrderPartslist).Any();
        //        }


        //        if (canRemoveAll && string.IsNullOrEmpty(poInfo.PO.KeyOfExtSys))
        //        {
        //            foreach (ProdOrderPartslist pl in allProdPartslists)
        //            {
        //                RemovePartslist(pl, mDProdOrderStateCancelled, mDProdOrderStateCompleted);
        //            }
        //        }
        //        else if (!IsBSOTemplateScheduleParent || poInfo.DeactivateAll)
        //        {
        //            if (
        //                    (
        //                        poInfo.PO.ProdOrderPartslist_ProdOrder.SelectMany(c => c.Picking_ProdOrderPartslist).Any()
        //                        && !poInfo.PO.ProdOrderPartslist_ProdOrder.SelectMany(c => c.Picking_ProdOrderPartslist).Any(c => c.PlanStateIndex < (short)PickingStateEnum.Completed)
        //                    )
        //                    || poInfo.DeactivateAll
        //                )
        //            {
        //                foreach (ProdOrderPartslist pl in allProdPartslists)
        //                    SetProdorderPartslistState(mDProdOrderStateCancelled, mDProdOrderStateCompleted, pl);
        //            }
        //            if (!poInfo.PO.ProdOrderPartslist_ProdOrder.Any(c => c.MDProdOrderState.MDProdOrderStateIndex < (short)(short)PickingStateEnum.Completed))
        //            {
        //                poInfo.PO.MDProdOrderState = mDProdOrderStateCompleted;
        //            }
        //        }
        //    }
        //}

        //public bool IsEnabledRemoveSelectedProdorderPartslist()
        //{
        //    return ProdOrderPartslistList != null && ProdOrderPartslistList.Any(c => c.IsSelected);
        //}

        #endregion

        #endregion

        #region Methods -> BatchPlanTimelinePresenter

        [ACMethodInfo("", "en{'Show'}de{'Anzeigen'}", 520)]
        public void ShowPickingsOnTimeline()
        {
            //List<PickingTimelineItem> treeViewItems = new List<PickingTimelineItem>();
            //List<PickingTimelineItem> timelineItems = new List<PickingTimelineItem>();

            //VD.PickingStateEnum startState = PickingStateEnum.Created;
            //VD.PickingStateEnum endState = PickingStateEnum.Paused;
            //MDProdOrderState.ProdOrderStates? prodOrderState = null;
            //ObservableCollection<VD.Picking> prodOrderBatchPlans =
            //    PickingManager
            //    .GetProductionLinieBatchPlans(
            //        DatabaseApp,
            //        null,
            //        startState,
            //        endState,
            //        FilterStartTime,
            //        FilterEndTime,
            //        prodOrderState,
            //        FilterPlanningMR?.PlanningMRID,
            //        SelectedFilterPickingPlanGroup?.MDBatchPlanGroupID,
            //        FilterPickingProgramNo,
            //        FilterPickingMaterialNo);

            //int displayOrder = 0;

            //foreach (PAScheduleForPWNode schedule in ScheduleForPWNodeList)
            //{
            //    PickingTimelineItem treeViewItem = new PickingTimelineItem();
            //    treeViewItem._ACCaption = schedule.ACCaption;
            //    treeViewItem.DisplayOrder = displayOrder;
            //    treeViewItem.TimelineItemType = PickingTimelineItem.PickingTimelineItemType.ContainerItem;

            //    //TODO: change start and end date, add more details to timelineitem (order etc.)
            //    Guid[] acClassWFIds =
            //        DatabaseApp
            //        .MDSchedulingGroup
            //        .Where(c => c.MDSchedulingGroupID == schedule.MDSchedulingGroupID)
            //        .SelectMany(c => c.MDSchedulingGroupWF_MDSchedulingGroup)
            //        .Select(c => c.VBiACClassWFID)
            //        .Distinct()
            //        .ToArray();


            //    List<PickingTimelineItem> batchPlanTimeline = new List<PickingTimelineItem>();
            //    List<Picking> batchPlans = prodOrderBatchPlans.Where(c => acClassWFIds.Contains(c.VBiACClassWFID ?? Guid.Empty)).ToList();
            //    foreach (Picking batchPlan in batchPlans)
            //    {
            //        PickingTimelineItem item = new PickingTimelineItem();
            //        item._ACCaption = string.Format(@"[{0}] {1} ({2} {3})",
            //            batchPlan.ProdOrderPartslist.ProdOrder.ProgramNo,
            //            batchPlan.ProdOrderPartslist.Partslist.Material.MaterialName1,
            //            batchPlan.TotalSize,
            //            batchPlan.ProdOrderPartslist.Partslist.MDUnitID.HasValue ? batchPlan.ProdOrderPartslist.Partslist.MDUnit.MDUnitName : batchPlan.ProdOrderPartslist.Partslist.Material.BaseMDUnit.MDUnitName);
            //        if (batchPlan.ScheduledStartDate != null)
            //            item.StartDate = batchPlan.ScheduledStartDate;
            //        else
            //            item.StartDate = batchPlan.PlannedStartDate;
            //        if (batchPlan.ScheduledEndDate != null)
            //            item.EndDate = batchPlan.ScheduledEndDate;
            //        else
            //            item.EndDate = item.StartDate.Value.AddMinutes(10);
            //        item.DisplayOrder = displayOrder;
            //        item.ParentACObject = treeViewItem;
            //        item.TimelineItemType = PickingTimelineItem.PickingTimelineItemType.TimelineItem;
            //        item.ProgramNo = batchPlan.ProdOrderPartslist.ProdOrder.ProgramNo;
            //        item.TargetQuantityUOM = batchPlan.TotalSize;
            //        batchPlanTimeline.Add(item);
            //    }

            //    if (!batchPlanTimeline.Any())
            //        continue;

            //    treeViewItem.StartDate = batchPlanTimeline.Min(c => c.StartDate);
            //    treeViewItem.EndDate = batchPlanTimeline.Max(c => c.EndDate);

            //    timelineItems.Add(treeViewItem);
            //    timelineItems.AddRange(batchPlanTimeline);
            //    treeViewItems.Add(treeViewItem);

            //    displayOrder++;
            //}

            //PickingTimelineItemsRoot = treeViewItems;
            //PickingTimelineItems = new ObservableCollection<PickingTimelineItem>(timelineItems);
        }

        #endregion

        #region Methods -> Overrides
        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            var result = base.OnGetControlModes(vbControl);

            if (vbControl != null && !string.IsNullOrEmpty(vbControl.VBContent))
            {
                //switch (vbControl.VBContent)
                //{
                //case "SelectedPicking\\ScheduledStartDate":
                //case "SelectedPicking\\ScheduledEndDate":
                //    if (SelectedPicking != null
                //        && (SelectedPicking.PlanState <= PickingStateEnum.ReadyToStart || SelectedPicking.PlanState == PickingStateEnum.Paused)
                //        && (SelectedPicking.PlanMode != BatchPlanMode.UseBatchCount
                //           || SelectedPicking.BatchTargetCount > SelectedPicking.BatchActualCount))
                //        result = Global.ControlModes.Enabled;
                //    else
                //        result = Global.ControlModes.Disabled;
                //    break;
                //case nameof(FilterTabPickingStartTime):
                //    result = Global.ControlModes.Enabled;
                //    bool filterOrderStartTimeIsEnabled =
                //       !(FilterTabPickingIsCompleted ?? true)
                //       || (FilterTabPickingStartTime != null && FilterTabPickingEndTime != null);
                //    if (!filterOrderStartTimeIsEnabled)
                //        result = Global.ControlModes.EnabledWrong;
                //    break;
                //case nameof(FilterTabPickingEndTime):
                //    result = Global.ControlModes.Enabled;
                //    bool filterOrderEndTimeIsEnabled =
                //         !(FilterTabPickingIsCompleted ?? true)
                //       || (FilterTabPickingStartTime != null && FilterTabPickingEndTime != null);
                //    if (!filterOrderEndTimeIsEnabled)
                //        result = Global.ControlModes.EnabledWrong;
                //    break;
                //}
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
                return SelectedPicking?.PickingID;
            }
        }


        protected override void RefreshScheduleForSelectedNode(Guid? selectedEntityID = null)
        {
            try
            {
                using (ACMonitor.Lock(DatabaseApp.QueryLock_1X000))
                {
                    PickingList = GetPickingList(SelectedScheduleForPWNode?.MDSchedulingGroupID);
                    if (selectedEntityID != null)
                        SelectedPicking = PickingList.FirstOrDefault(c => c.PickingID == selectedEntityID);
                    else
                        SelectedPicking = PickingList.FirstOrDefault();
                    //ProdOrderPartslistList = GetProdOrderPartslistList();
                }
            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), nameof(RefreshScheduleForSelectedNode), e);
                Messages.LogException(this.GetACUrl(), nameof(RefreshScheduleForSelectedNode), e.StackTrace);
            }
        }

        #endregion

        #endregion

        #region Methods => Routing

        [ACMethodInfo("", "en{'Route check over orders'}de{'Routenprüfung über Aufträge'}", 9999, true)]
        public void RunPossibleRoutesCheck()
        {
            CalculateRouteResult = null;
            CurrentProgressInfo.ProgressInfoIsIndeterminate = true;
            bool invoked = InvokeCalculateRoutesAsync();
            if (!invoked)
            {
                Messages.Info(this, "The calculation is in progress, please wait and try again!");
                return;
            }
            ShowDialog(this, "CalculatedRouteDialog");
        }

        public bool IsEnabledPossibleRoutesCheck()
        {
            return PickingList != null && PickingList.Any();
        }

        [ACMethodInfo("Function", "en{'RMICallback'}de{'RMICallback'}", 9999)]
        public void RMICallback(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject)
        {
            // The callback-method can be called
            if (e != null)
            {
                IACTask taskEntry = wrapObject as IACTask;
                ACPointAsyncRMIWrap<ACComponent> taskEntryMoreConcrete = wrapObject as ACPointAsyncRMIWrap<ACComponent>;
                ACMethodEventArgs eM = e as ACMethodEventArgs;
                if (taskEntry.State == PointProcessingState.Deleted)
                {
                    // Compare RequestID to identify your asynchronus invocation
                    if (taskEntry.RequestID == myTestRequestID)
                    {
                        OnCalculateRoutesCallback();
                    }
                }
                if (taskEntryMoreConcrete.Result.ResultState == Global.ACMethodResultState.Succeeded)
                {
                    bool wasMyAsynchronousRequest = false;
                    if (myRequestEntryA != null && myRequestEntryA.CompareTo(taskEntryMoreConcrete) == 0)
                        wasMyAsynchronousRequest = true;
                    System.Diagnostics.Trace.WriteLine(wasMyAsynchronousRequest.ToString());
                }
            }
        }

        Guid myTestRequestID;
        ACPointAsyncRMISubscrWrap<ACComponent> myRequestEntryA;
        public bool InvokeCalculateRoutesAsync()
        {
            // 1. Invoke ACUrlACTypeSignature for getting a default-ACMethod-Instance
            ACMethod acMethod = PAWorkflowScheduler.ACUrlACTypeSignature("!RunRouteCalculation", gip.core.datamodel.Database.GlobalDatabase);

            // 2. Fill out all important parameters
            acMethod.ParameterValueList.GetACValue("RouteCalculation").Value = true;


            myRequestEntryA = RMISubscr.InvokeAsyncMethod(PAWorkflowScheduler, "RMIPoint", acMethod, RMICallback);
            if (myRequestEntryA != null)
                myTestRequestID = myRequestEntryA.RequestID;
            return myRequestEntryA != null;
        }

        public void OnCalculateRoutesCallback()
        {
            var pickings = PickingManager.GetScheduledPickings(DatabaseApp, PickingStateEnum.WaitOnManualClosing, PickingStateEnum.InProcess, null, null, null, null).ToArray();

            List<FacilityReservation> reservations = new List<FacilityReservation>();

            foreach (Picking picking in pickings)
            {
                if (PickingList.Where(c => c.PickingID == picking.PickingID).Any())
                    continue;

                foreach (PickingPos pPos in picking.PickingPos_Picking)
                {
                    reservations.AddRange(pPos.FacilityReservation_PickingPos);
                }
            }

            var myReservations = SelectedPicking.Picking.PickingPos_Picking.SelectMany(c => c.FacilityReservation_PickingPos).ToArray();

            ACProdOrderManager prodOrderManager = ACProdOrderManager.GetServiceInstance(this);
            var prodOrderBatchPlans  = prodOrderManager.GetProductionLinieBatchPlansWithPWNode(DatabaseApp, GlobalApp.BatchPlanState.Created, GlobalApp.BatchPlanState.Paused,
                                                                                                     null, null, null, null, null, null, null);

            reservations.AddRange(prodOrderBatchPlans.SelectMany(c => c.FacilityReservation_ProdOrderBatchPlan.Where(x => x.FacilityID.HasValue)));

            List<FacilityReservation> result = new List<FacilityReservation>();

            foreach (FacilityReservation reservation in myReservations)
            {
                if (reservation.CalculatedRoute != null)
                {
                    string[] splitedRoute = reservation.CalculatedRoute.Split(new char[] { ',' });

                    foreach (string routeHash in splitedRoute)
                    {
                        IEnumerable<FacilityReservation> items = reservations.Where(c => c.CalculatedRoute != null && c.CalculatedRoute.Contains(routeHash));
                        if (items.Any())
                            result.AddRange(items);
                    }
                }
            }

            if (result.Any())
            {
                List<string> reservationsWithSameRoute = result.Where(c => c.PickingPos != null).Select(c => c.PickingPos.Picking.PickingNo).Distinct().ToList();
                IEnumerable<string> prodOrderWithSameRoute = result.Where(c => c.ProdOrderBatchPlan != null).Select(c => c.ProdOrderBatchPlan.ProdOrderPartslist.ProdOrder.ProgramNo).Distinct();
                if (prodOrderWithSameRoute != null && prodOrderWithSameRoute.Any())
                    reservationsWithSameRoute.AddRange(prodOrderWithSameRoute);

                if (reservationsWithSameRoute != null && reservationsWithSameRoute.Any())
                {
                    CalculateRouteResult = "The following orders order may use same module: " + string.Join(", ", reservationsWithSameRoute);
                }
                else
                {
                    CalculateRouteResult = "There no order which will use equipment from this order!";
                }
            }
            else
                CalculateRouteResult = "There no order which will use equipment from this order!"; ;

            CurrentProgressInfo.ProgressInfoIsIndeterminate = false;
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
            }
        }

        #endregion
    }

}