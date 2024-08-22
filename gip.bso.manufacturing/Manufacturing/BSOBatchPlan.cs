using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.core.autocomponent;
using gip.core.manager;
using System.Collections.ObjectModel;
using gip.mes.facility;
using gip.mes.processapplication;

namespace gip.bso.manufacturing
{

    /// <summary>
    /// Unter-BSO für VBBSOControlDialog
    /// Wird verwendet für PWBase (Workflowwelt) und Ableitungen
    /// 
    /// </summary>
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Batch planning'}de{'Batchplanung'}", Global.ACKinds.TACBSOGlobal, Global.ACStorableTypes.NotStorable, false, true)]
    public class BSOBatchPlan : VBBSOModulesSelector
    {
        #region c´tors

        public BSOBatchPlan(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _RMISubscr = new ACPointAsyncRMISubscr(this, "RMISubscr", 1);
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            _ProdOrderManager = ACProdOrderManager.ACRefToServiceInstance(this);
            if (_ProdOrderManager == null)
                throw new Exception("ProdOrderManager not configured");

            _VarioConfigManager = ConfigManagerIPlus.ACRefToServiceInstance(this);
            if (_VarioConfigManager == null)
                throw new Exception("VarioConfigManager not configured");

            CurrentPWInfo = ParentACComponent.ACUrlCommand("CurrentSelection") as IACComponentPWNode;

            if (SelectionDialog != null)
                SelectionDialog.PropertyChanged += SelectionDialog_PropertyChanged;
            if (ProdOrderBSO != null)
                ProdOrderBSO.PropertyChanged += ProdOrderBSO_PropertyChanged;
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (SelectionDialog != null)
                SelectionDialog.PropertyChanged -= SelectionDialog_PropertyChanged;
            if (ProdOrderBSO != null)
                ProdOrderBSO.PropertyChanged -= ProdOrderBSO_PropertyChanged;
            this._CurrentACClassWF = null;
            this._CurrentPWInfo = null;

            if (_ProdOrderManager != null)
                ACProdOrderManager.DetachACRefFromServiceInstance(this, _ProdOrderManager);
            _ProdOrderManager = null;

            if (_VarioConfigManager != null)
                ConfigManagerIPlus.DetachACRefFromServiceInstance(this, _VarioConfigManager);
            _VarioConfigManager = null;

            return base.ACDeInit(deleteACClassTask);
        }

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

        public bool PreselectFirstFacilityReservation { get; set; }
        public virtual string VBBSORuleSelectionName
        {
            get
            {
                return @"VBBSORuleSelection";
            }
        }

        #region 1. PWNode/ACClassWF

        //public override void OnSelectionChanged()
        //{
        //    CurrentPWInfo = CurrentSelection as IACConfigProvider;
        //}

        IACComponentPWNode _CurrentPWInfo;
        [ACPropertyInfo(9999)]
        public IACComponentPWNode CurrentPWInfo
        {
            get
            {
                return _CurrentPWInfo;
            }
            set
            {
                _CurrentPWInfo = value;
                if (value == null)
                {
                    CurrentACClassWF = null;
                }
                else
                {
                    CurrentACClassWF = value.Content as gip.core.datamodel.ACClassWF;
                }
            }
        }


        gip.core.datamodel.ACClassWF _CurrentACClassWF;
        [ACPropertyInfo(9999)]
        public gip.core.datamodel.ACClassWF CurrentACClassWF
        {
            get
            {
                return _CurrentACClassWF;
            }
            set
            {
                bool objectSwapped = true;
                if (_CurrentACClassWF != null && _CurrentACClassWF == value)
                    objectSwapped = false;

                _CurrentACClassWF = value;
                if (objectSwapped)
                {
                    if (DatabaseApp.IsChanged && ProdOrderBSO != null)
                    {
                        ProdOrderBSO.Save();
                    }
                    LoadBatchPlan();
                    SelectedWFProductionLine = GetSchedulingGroup(value);
                }

                OnPropertyChanged("CurrentACClassWF");

                //if (objectSwapped)
                //    OnPropertyChanged("CurrentLayout");
            }
        }

        #endregion

        #region Production Lines

        // WFProductionLine MDSchedulerGroup

        #region WFProductionLine
        private MDSchedulingGroup _SelectedWFProductionLine;
        /// <summary>
        /// Selected property for MDSchedulingGroup
        /// </summary>
        /// <value>The selected WFProductionLine</value>
        [ACPropertySelected(9999, "WFProductionLine", "en{'Production Line'}de{'Production Line'}")]
        public MDSchedulingGroup SelectedWFProductionLine
        {
            get
            {
                return _SelectedWFProductionLine;
            }
            set
            {
                if (_SelectedWFProductionLine != value)
                {
                    _SelectedWFProductionLine = value;
                    OnPropertyChanged("SelectedWFProductionLine");
                }
            }
        }


        private Type _TypeOfPWNodeProcessWorkflow;
        protected Type TypeOfPWNodeProcessWorkflow
        {
            get
            {
                if (_TypeOfPWNodeProcessWorkflow == null)
                    _TypeOfPWNodeProcessWorkflow = typeof(PWNodeProcessWorkflowVB);
                return _TypeOfPWNodeProcessWorkflow;
            }
        }

        private List<MDSchedulingGroup> _WFProductionLineList;
        /// <summary>
        /// List property for MDSchedulingGroup
        /// </summary>
        /// <value>The WFProductionLine list</value>
        [ACPropertyList(9999, "WFProductionLine")]
        public List<MDSchedulingGroup> WFProductionLineList
        {
            get
            {
                if (_WFProductionLineList == null)
                    _WFProductionLineList = LoadWFProductionLineList();
                return _WFProductionLineList;
            }
        }

        private List<MDSchedulingGroup> LoadWFProductionLineList()
        {
            return DatabaseApp.MDSchedulingGroup.OrderBy(c => c.MDKey).ToList();
        }


        public MDSchedulingGroup GetSchedulingGroup(core.datamodel.ACClassWF aclassWF)
        {
            Type typeOfSelectedNode = aclassWF?.PWACClass?.ObjectType;
            if (typeOfSelectedNode == null)
                return null;
            if (!TypeOfPWNodeProcessWorkflow.IsAssignableFrom(typeOfSelectedNode))
                return null;

            return DatabaseApp.MDSchedulingGroupWF.Where(c => c.VBiACClassWFID == aclassWF.ACClassWFID).Select(c => c.MDSchedulingGroup).FirstOrDefault();
        }
        #endregion


        #endregion

        #region BSO's
        public VBBSOSelectionDependentDialog SelectionDialog
        {
            get
            {
                return FindParentComponent<VBBSOSelectionDependentDialog>();
            }
        }

        public VBPresenterMethod PresenterBSO
        {
            get
            {
                return FindParentComponent<VBPresenterMethod>();
            }
        }

        public BSOProdOrder ProdOrderBSO
        {
            get
            {
                return FindParentComponent<BSOProdOrder>();
            }
        }

        public BSOBatchPlanScheduler BSOBatchPlanSchedulerBSO
        {
            get
            {
                return FindParentComponent<BSOBatchPlanScheduler>();
            }
        }

        /// <summary>
        /// Partslist generated without BSOProdOrder
        /// </summary>
        public ProdOrderPartslist ExternProdOrderPartslist { get; set; }


        public ProdOrderPartslist CurrentProdOrderPartslist
        {
            get
            {
                var prodOrderBSO = ProdOrderBSO;
                if (prodOrderBSO != null)
                {
                    return prodOrderBSO.SelectedProdOrderPartslist;
                }
                else if (ExternProdOrderPartslist != null)
                    return ExternProdOrderPartslist;
                else if (_CurrentPWInfo != null)
                {
                    PWNodeProcessWorkflowVB pwNodeReal = _CurrentPWInfo as PWNodeProcessWorkflowVB;
                    if (pwNodeReal != null)
                    {
                        return pwNodeReal.CurrentProdOrderPartslist.FromAppContext<ProdOrderPartslist>(this.DatabaseApp);
                    }
                    else
                    {
                        PWNodeProxy pwNodeProxy = _CurrentPWInfo as PWNodeProxy;
                        if (pwNodeProxy != null)
                        {
                            // TODO: Hole Wert vom Server und speichere in lokaler Variable ab
                        }
                    }
                }
                return null;
            }
        }

        public override DatabaseApp DatabaseApp
        {
            get
            {
                var prodOrderBSO = ProdOrderBSO;
                if (prodOrderBSO != null)
                    return prodOrderBSO.DatabaseApp;
                var bSOBatchPlanSchedulerBSO = BSOBatchPlanSchedulerBSO;
                if (bSOBatchPlanSchedulerBSO != null)
                    return bSOBatchPlanSchedulerBSO.DatabaseApp;
                return base.DatabaseApp;
            }
        }

        public gip.core.datamodel.ACClassMethod CurrentACClassMethod
        {
            get
            {
                var presenter = PresenterBSO;
                if (presenter == null)
                    return null;
                return presenter.SelectedWFContext as gip.core.datamodel.ACClassMethod;
            }
        }
        #endregion

        #region BatchPlanForIntermediate
        private ProdOrderBatchPlan _SelectedBatchPlanForIntermediate;
        /// <summary>
        /// Selected property for ProdOrderBatchPlan
        /// </summary>
        /// <value>The selected BatchPlanForIntermediate</value>
        [ACPropertySelected(9999, "BatchPlanForIntermediate", "en{'Batchplan'}de{'Batchplan'}")]
        public ProdOrderBatchPlan SelectedBatchPlanForIntermediate
        {
            get
            {
                return _SelectedBatchPlanForIntermediate;
            }
            set
            {
                bool changed = false;
                if (_SelectedBatchPlanForIntermediate != value)
                {
                    if (_SelectedBatchPlanForIntermediate != null)
                        _SelectedBatchPlanForIntermediate.PropertyChanged -= _SelectedBatchPlanForIntermediate_PropertyChanged;
                    changed = true;
                }
                _SelectedBatchPlanForIntermediate = value;
                if (changed)
                {
                    if (_SelectedBatchPlanForIntermediate != null)
                        _SelectedBatchPlanForIntermediate.PropertyChanged += _SelectedBatchPlanForIntermediate_PropertyChanged;
                    RefreshTargets();
                    OnPropertyChanged("SumTotalSize");
                    OnPropertyChanged("SumDiffSize");
                    OnPropertyChanged("SumDiffBatchCount");
                    OnPropertyChanged("SumDiffBatchCountAct");
                }
                OnPropertyChanged("SelectedBatchPlanForIntermediate");
            }
        }

        protected bool _UpdatingControlModeBatchPlan = false;
        void _SelectedBatchPlanForIntermediate_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_UpdatingControlModeBatchPlan || this.InitState != ACInitState.Initialized)
                return;

            try
            {
                if (e.PropertyName == "PlanModeIndex")
                {
                    _UpdatingControlModeBatchPlan = true;
                    SelectedBatchPlanForIntermediate.OnEntityPropertyChanged("BatchNoFrom");
                    SelectedBatchPlanForIntermediate.OnEntityPropertyChanged("BatchNoTo");
                    SelectedBatchPlanForIntermediate.OnEntityPropertyChanged("BatchTargetCount");
                    SelectedBatchPlanForIntermediate.OnEntityPropertyChanged("BatchSize");
                    SelectedBatchPlanForIntermediate.OnEntityPropertyChanged("TotalSize");
                    SelectedBatchPlanForIntermediate.OnEntityPropertyChanged("YieldTotalSizeExpected");
                }
                else if (e.PropertyName == "BatchNoTo")
                {
                    _UpdatingControlModeBatchPlan = true;
                    SelectedBatchPlanForIntermediate.OnEntityPropertyChanged("BatchNoFrom");
                    OnPropertyChanged("SumTotalSize");
                    OnPropertyChanged("SumDiffSize");
                    OnPropertyChanged("SumDiffBatchCount");
                }
                else if (e.PropertyName == "BatchNoFrom")
                {
                    _UpdatingControlModeBatchPlan = true;
                    SelectedBatchPlanForIntermediate.OnEntityPropertyChanged("BatchNoTo");
                    OnPropertyChanged("SumTotalSize");
                    OnPropertyChanged("SumDiffSize");
                    OnPropertyChanged("SumDiffBatchCount");
                }
                else if (e.PropertyName == "BatchSize")
                {
                    _UpdatingControlModeBatchPlan = true;
                    SelectedBatchPlanForIntermediate.OnEntityPropertyChanged("BatchSize");
                    OnPropertyChanged("SumTotalSize");
                    OnPropertyChanged("SumDiffSize");
                    OnPropertyChanged("SumDiffBatchCount");
                    OnPropertyChanged("SumDiffBatchCountAct");
                }
                else if (e.PropertyName == "PlanState" || e.PropertyName == "PlanStateIndex")
                {
                    if (SelectedBatchPlanForIntermediate.PlanState == GlobalApp.BatchPlanState.AutoStart)
                    {
                        SelectedBatchPlanForIntermediate.PlannedStartDate = DateTimeUtils.NowDST;
                        if (!_IsStartingBatchPlan)
                        {
                            // Eine manuelle Statusänderung auf <Autostart> hat möglicherweise keine Auswirkung auf die Prozesssteuerung. Bitte setzen Sie den Status wieder zurück auf <Neu angelegt> und drücken Sie die <Start Batch>-Taste!
                            Msg msgForAll = new Msg
                            {
                                Source = GetACUrl(),
                                MessageLevel = eMsgLevel.Info,
                                ACIdentifier = "SelectedBatchPlanForIntermediate(0)",
                                Message = Root.Environment.TranslateMessage(this, "Info50023")
                            };
                            Messages.Msg(msgForAll, Global.MsgResult.OK, eMsgButton.OK);

                        }
                    }
                }
                else if (e.PropertyName == "TotalSize" || e.PropertyName == "BatchTargetCount")
                {
                    OnPropertyChanged("SumTotalSize");
                    OnPropertyChanged("SumDiffSize");
                    OnPropertyChanged("SumDiffBatchCount");
                }
            }
            finally
            {
                _UpdatingControlModeBatchPlan = false;
            }
        }


        private ObservableCollection<ProdOrderBatchPlan> _BatchPlanForIntermediateList;
        /// <summary>
        /// List property for ProdOrderBatchPlan
        /// </summary>
        /// <value>The BatchPlanForIntermediate list</value>
        [ACPropertyList(9999, "BatchPlanForIntermediate")]
        public ObservableCollection<ProdOrderBatchPlan> BatchPlanForIntermediateList
        {
            get
            {
                if (_BatchPlanForIntermediateList == null)
                    _BatchPlanForIntermediateList = new ObservableCollection<ProdOrderBatchPlan>();
                return _BatchPlanForIntermediateList;
            }
            set
            {
                _BatchPlanForIntermediateList = value;
                OnPropertyChanged("BatchPlanForIntermediateList");
            }
        }

        private ProdOrderPartslistPos _SelectedIntermediate;
        [ACPropertySelected(9999, "Intermediate")]
        public ProdOrderPartslistPos SelectedIntermediate
        {
            get
            {
                return _SelectedIntermediate;
            }
            set
            {
                if (_SelectedIntermediate != value)
                {
                    _SelectedIntermediate = value;
                    OnPropertyChanged("SelectedIntermediate");
                }
            }
        }

        private gip.mes.datamodel.ACClassWF _VBCurrentACClassWF;
        [ACPropertyInfo(9999)]
        public gip.mes.datamodel.ACClassWF VBCurrentACClassWF
        {
            get
            {
                return _VBCurrentACClassWF;
            }
            set
            {
                if (_VBCurrentACClassWF != value)
                {
                    _VBCurrentACClassWF = value;
                    OnPropertyChanged("VBCurrentACClassWF");
                }
            }
        }

        private List<IACConfigStore> _MandatoryConfigStores = null;
        public List<IACConfigStore> MandatoryConfigStores
        {
            get
            {
                return _MandatoryConfigStores;
            }
            set
            {
                _MandatoryConfigStores = value;
            }
        }


        public IACConfigStore CurrentConfigStore
        {
            get
            {
                if (CurrentPWInfo != null) return (CurrentPWInfo as IACConfigStoreSelection).CurrentConfigStore;
                if (CurrentProdOrderPartslist != null) return CurrentProdOrderPartslist;
                return null;
            }
        }

        public string CurrentConfigACUrl
        {
            get
            {
                if (CurrentPWInfo != null)
                    return CurrentPWInfo.ConfigACUrl;
                if (CurrentACClassWF != null)
                    return CurrentACClassWF.ConfigACUrl;
                return null;
            }
        }

        #endregion

        public bool IsVisibleInCurrentContext
        {
            get
            {
                return (ProdOrderBSO != null || ExternProdOrderPartslist != null) && CurrentConfigStore != null && CurrentConfigACUrl != null;
            }
        }


        [ACPropertyInfo(100, "", "en{'Planned total'}de{'Geplante Gesamtmenge'}")]
        public double SumTotalSize
        {
            get
            {
                return SelectedIntermediate == null ? 0.0 : SelectedIntermediate.ProdOrderBatchPlan_ProdOrderPartslistPos.Sum(c => c.TotalSize);
            }
        }

        [ACPropertyInfo(101, "", "en{'Missing quantity of plan'}de{'Fehlende Planmenge'}")]
        public double SumDiffSize
        {
            get
            {
                return SelectedIntermediate == null ? 0.0 : SumTotalSize - SelectedIntermediate.TargetQuantityUOM;
            }
        }

        [ACPropertyInfo(102, "", "en{'Missing target batch count'}de{'Fehlende Soll-Batchanzahl'}")]
        public double SumDiffBatchCount
        {
            get
            {
                return SelectedBatchPlanForIntermediate == null || SelectedBatchPlanForIntermediate.BatchSize < 0.000001 ? 0.0 : SumDiffSize / SelectedBatchPlanForIntermediate.BatchSize;
            }
        }

        [ACPropertyInfo(103, "", "en{'Missing actual batch count'}de{'Fehlende Ist-Batchanzahl'}")]
        public double SumDiffBatchCountAct
        {
            get
            {
                return SelectedBatchPlanForIntermediate == null || SelectedBatchPlanForIntermediate.BatchSize < 0.000001 ? 0.0 : SelectedIntermediate.DifferenceQuantityUOM / SelectedBatchPlanForIntermediate.BatchSize;
            }
        }

        #region Properties => RMI and CalculatedRoutes

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

        public void LoadBatchPlan()
        {
            var prodOrderBSO = ProdOrderBSO;
            try
            {

                if (_CurrentACClassWF == null || _CurrentACClassWF.PWACClass == null || !typeof(PWNodeProcessWorkflowVB).IsAssignableFrom(_CurrentACClassWF.PWACClass.ObjectType))
                {
                    SelectedBatchPlanForIntermediate = null;
                    BatchPlanForIntermediateList = null;
                    SelectedIntermediate = null;
                    return;
                }

                var currentProdOrderPartslist = CurrentProdOrderPartslist;
                if (currentProdOrderPartslist == null)
                {
                    SelectedBatchPlanForIntermediate = null;
                    BatchPlanForIntermediateList = null;
                    SelectedIntermediate = null;
                    // TODO: Show Message
                    return;
                }

                VBCurrentACClassWF = _CurrentACClassWF.FromAppContext<gip.mes.datamodel.ACClassWF>(DatabaseApp);
                MaterialWFConnection matWFConnection = ProdOrderManager.GetMaterialWFConnection(VBCurrentACClassWF, currentProdOrderPartslist.Partslist.MaterialWFID);
                if (matWFConnection == null)
                {
                    SelectedBatchPlanForIntermediate = null;
                    BatchPlanForIntermediateList = null;
                    SelectedIntermediate = null;
                    // TODO: Show Message
                    return;
                }

                _MandatoryConfigStores = ProdOrderManager.GetCurrentConfigStores(_CurrentACClassWF, VBCurrentACClassWF, currentProdOrderPartslist.Partslist.MaterialWFID, currentProdOrderPartslist.Partslist, currentProdOrderPartslist);

                SelectedIntermediate = ProdOrderManager.GetIntermediate(currentProdOrderPartslist, matWFConnection);
                if (SelectedIntermediate == null)
                {
                    SelectedBatchPlanForIntermediate = null;
                    BatchPlanForIntermediateList = null;
                    // TODO: Show Message
                    //OnNewAlarmOccurred(ProcessAlarm, "ProdOrderPartslistPos not found for intermediate Material", true);
                    return;
                }

                if (ExternProdOrderPartslist == null)
                    LoadBatchPlanForIntermediateList(true);

            }
            catch (Exception e)
            {
                string message = e.Message;
                if (e.InnerException != null)
                    message += e.InnerException.Message;
                Messages.Exception(this, message, true);
            }
        }

        public void LoadBatchPlanForIntermediateList(bool autoLoad)
        {
            if (autoLoad)
                SelectedIntermediate.ProdOrderBatchPlan_ProdOrderPartslistPos.AutoLoad(DatabaseApp);
            BatchPlanForIntermediateList = new ObservableCollection<ProdOrderBatchPlan>(SelectedIntermediate.ProdOrderBatchPlan_ProdOrderPartslistPos
                .Where(c => c.VBiACClassWFID.HasValue && c.VBiACClassWFID == VBCurrentACClassWF.ACClassWFID)
                .OrderBy(c => c.Sequence)
                .ThenBy(c => c.InsertDate)
                .ToArray());
            if (!BatchPlanForIntermediateList.Any())
            {
                SelectedBatchPlanForIntermediate = null;
                // TODO: Show MEssage Error00123: No batchplan found for this intermediate material
                return;
            }

            SelectedBatchPlanForIntermediate = _BatchPlanForIntermediateList.FirstOrDefault();
        }

        public virtual bool CorrectInputData(MsgWithDetails msg)
        {
            return false;
        }

        #region Overrides

        protected override void ParentACComponent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        public override bool IsModuleSelDisabled
        {
            get { return false; }
        }

        override protected void RefreshModules()
        {

        }

        override protected void RefreshTargets()
        {
            BindingList<POPartslistPosReservation> result = new BindingList<POPartslistPosReservation>();
            ProdOrderPartslist currentProdOrderPartslist = CurrentProdOrderPartslist != null ? CurrentProdOrderPartslist : ExternProdOrderPartslist;
            if (currentProdOrderPartslist != null && ProdOrderManager != null)
            {
                result = ProdOrderManager.GetTargets(DatabaseApp, VarioConfigManager, RoutingService, VBCurrentACClassWF, currentProdOrderPartslist,
               SelectedIntermediate, SelectedBatchPlanForIntermediate, CurrentConfigACUrl,
               ShowCellsInRoute, ShowSelectedCells, ShowEnabledCells, ShowSameMaterialCells, PreselectFirstFacilityReservation);
            }

            if (currentProdOrderPartslist != null && SelectedBatchPlanForIntermediate != null && VBCurrentACClassWF != null && result != null && result.Any())
            {
                FacilityReservation[] relatedReservations =
                        DatabaseApp
                        .FacilityReservation
                        .Where(c =>
                            c.VBiACClassID != null
                            && c.ProdOrderBatchPlan != null
                            && c.ProdOrderBatchPlan.VBiACClassWFID != null
                            && c.ProdOrderBatchPlan.ProdOrderPartslist.ProdOrderID != currentProdOrderPartslist.ProdOrderID
                            && c.ProdOrderBatchPlan.ProdOrderPartslist.MDProdOrderState.MDProdOrderStateIndex < (short)MDProdOrderState.ProdOrderStates.InProduction
                         )
                        .ToArray();

                if (relatedReservations.Any())
                {
                    foreach (POPartslistPosReservation item in result)
                    {
                        var relatedProdOrderNos = relatedReservations
                            .Where(c => c.VBiACClassID == item.Module?.ACClassID)
                            .Select(c => c.ProdOrderBatchPlan.ProdOrderPartslist.ProdOrder.ProgramNo)
                            .Distinct()
                            .OrderBy(c => c);

                        item.ConnectedOrders = string.Join(",", relatedProdOrderNos);
                    }
                }
            }

            TargetsList = result;
            SelectedTarget = TargetsList.FirstOrDefault();
        }

        protected virtual bool OnFilterTarget(RouteItem routeItem)
        {
            return false;
        }

        protected override void _Targets_ListChanged(object sender, ListChangedEventArgs e)
        {
            //if (SelectedTarget != null && ProdOrderManager != null)
            //{
            //    ProdOrderManager.ValidateChangedPosReservation(this.DatabaseApp, SelectedTarget);
            //}
        }

        protected override void _SelectedTarget_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base._SelectedTarget_PropertyChanged(sender, e);
            if (SelectedTarget != null && ProdOrderManager != null && e != null && e.PropertyName == nameof(POPartslistPosReservation.SelectedReservation))
            {
                ProdOrderManager.ValidateChangedPosReservation(this, this.DatabaseApp, SelectedTarget, sender, e);
            }
        }

        /// <summary>Called inside the GetControlModes-Method to get the Global.ControlModes from derivations.
        /// This method should be overriden in the derivations to dynmically control the presentation mode depending on the current value which is bound via VBContent</summary>
        /// <param name="vbControl">A WPF-Control that implements IVBContent</param>
        /// <returns>ControlModesInfo</returns>
        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            if (vbControl == null)
                return base.OnGetControlModes(vbControl);

            Global.ControlModes result = base.OnGetControlModes(vbControl);
            if (result < Global.ControlModes.Enabled)
                return result;
            switch (vbControl.VBContent)
            {
                case "SelectedBatchPlanForIntermediate\\BatchNoFrom":
                case "SelectedBatchPlanForIntermediate\\BatchNoTo":
                    {
                        if (SelectedBatchPlanForIntermediate == null || SelectedBatchPlanForIntermediate.PlanMode != BatchPlanMode.UseFromTo)
                            result = Global.ControlModes.Disabled;
                        else if (vbControl.VBContent == "SelectedBatchPlanForIntermediate\\BatchNoFrom")
                        {
                            if (!SelectedBatchPlanForIntermediate.BatchNoFrom.HasValue)
                                result = Global.ControlModes.EnabledRequired;
                            else if (SelectedBatchPlanForIntermediate.BatchNoTo.HasValue && SelectedBatchPlanForIntermediate.BatchNoTo < SelectedBatchPlanForIntermediate.BatchNoFrom)
                                result = Global.ControlModes.EnabledWrong;
                        }
                        else if (vbControl.VBContent == "SelectedBatchPlanForIntermediate\\BatchNoTo")
                        {
                            if (!SelectedBatchPlanForIntermediate.BatchNoTo.HasValue)
                                result = Global.ControlModes.EnabledRequired;
                            else if (SelectedBatchPlanForIntermediate.BatchNoFrom.HasValue && SelectedBatchPlanForIntermediate.BatchNoTo < SelectedBatchPlanForIntermediate.BatchNoFrom)
                                result = Global.ControlModes.EnabledWrong;
                        }
                        break;
                    }
                case "SelectedBatchPlanForIntermediate\\BatchTargetCount":
                    {
                        if (SelectedBatchPlanForIntermediate == null || SelectedBatchPlanForIntermediate.PlanMode != BatchPlanMode.UseBatchCount)
                            result = Global.ControlModes.Disabled;
                        else if (SelectedBatchPlanForIntermediate.BatchTargetCount <= 0)
                            result = Global.ControlModes.EnabledWrong;
                        break;
                    }
                case "SelectedBatchPlanForIntermediate\\BatchSize":
                    {
                        if (SelectedBatchPlanForIntermediate == null || SelectedBatchPlanForIntermediate.PlanMode == BatchPlanMode.UseTotalSize)
                            result = Global.ControlModes.Disabled;
                        else
                        {
                            if (SelectedBatchPlanForIntermediate.BatchSize <= 0.000001)
                            {
                                result = Global.ControlModes.EnabledWrong;
                            }
                        }
                        break;
                    }
                case "SelectedBatchPlanForIntermediate\\TotalSize":
                    {
                        if (SelectedBatchPlanForIntermediate == null || SelectedBatchPlanForIntermediate.PlanMode != BatchPlanMode.UseTotalSize)
                            result = Global.ControlModes.Disabled;
                        else if (SelectedBatchPlanForIntermediate.TotalSize <= 0.000001)
                            result = Global.ControlModes.EnabledWrong;
                        break;
                    }
                case "SelectedProdOrderPartslist\\TargetQuantity":
                case "SelectedProdOrderPartslistPos\\TargetQuantity":
                case "SelectedProdOrderPartslist\\DifferenceQuantity":
                case "SelectedProdOrderPartslistPos\\TargetQuantityUOM":
                case "SelectedOutwardPartslistPos\\TargetQuantity":
                case "SelectedOutwardPartslistPos\\TargetQuantityUOM":
                case "SelectedIntermediate\\TargetQuantity":
                case "SelectedIntermediate\\TargetQuantityUOM":
                case "SelectedProdOrderIntermediateBatch\\TargetQuantity":
                case "SelectedProdOrderIntermediateBatch\\TargetQuantityUOM":
                case "SelectedProdOrderPartslist\\ActualQuantity":
                case "SelectedProdOrderPartslistPos\\ActualQuantity":
                case "SelectedProdOrderPartslistPos\\ActualQuantityUOM":
                case "SelectedOutwardPartslistPos\\ActualQuantity":
                case "SelectedOutwardPartslistPos\\ActualQuantityUOM":
                case "SelectedIntermediate\\ActualQuantity":
                case "SelectedIntermediate\\ActualQuantityUOM":
                case "SelectedProdOrderIntermediateBatch\\ActualQuantity":
                case "SelectedProdOrderIntermediateBatch\\ActualQuantityUOM":
                    {
                        //SelectedProdOrderPartslist.ActualQuantity
                        if (!Root.Environment.User.IsSuperuser)
                            result = Global.ControlModes.Disabled;
                        break;
                    }
                case "SelectedBatchPlanForIntermediate\\YieldPerc":
                case "SelectedBatchPlanForIntermediate\\YieldBatchSize":
                case "SelectedBatchPlanForIntermediate\\YieldTotalSize":
                    {
                        break;
                    }
                case "SelectedBatchPlanForIntermediate\\YieldTotalSizeExpected":
                    {
                        if (SelectedBatchPlanForIntermediate == null || SelectedBatchPlanForIntermediate.PlanMode != BatchPlanMode.UseBatchCount)
                            result = Global.ControlModes.Disabled;
                        else if (SelectedBatchPlanForIntermediate.YieldTotalSizeExpected <= 0)
                            result = Global.ControlModes.EnabledWrong;
                        break;
                    }
                case "StartBatchPlan":
                    result = Global.ControlModes.Enabled;
                    if (ExternProdOrderPartslist != null)
                        result = Global.ControlModes.Hidden;
                    break;

            }

            return result;
        }
        #endregion

        // Static, if more instances active
        private static bool _IsStartingBatchPlan = false;
        [ACMethodCommand("", "en{'Start Batch'}de{'Start Batch'}", (short)MISort.Start)]
        public void StartBatchPlan()
        {
            if (!IsEnabledStartBatchPlan()) return;
            _IsStartingBatchPlan = true;
            try
            {

                if (!StartBatchPlanValidation())
                    return;

                gip.core.datamodel.ACClassMethod acClassMethod = CurrentACClassMethod;
                gip.core.datamodel.ACProject project = acClassMethod.ACClass.ACProject as gip.core.datamodel.ACProject;

                AppManagersList = this.Root.FindChildComponents(project.RootClass, 1);
                if (AppManagersList.Count > 1)
                {
                    DialogResult = null;
                    ShowDialog(this, "SelectAppManager");
                    if (DialogResult == null || DialogResult.SelectedCommand != eMsgButton.OK)
                        return;
                }
                else
                    SelectedAppManager = AppManagersList.FirstOrDefault();

                ACComponent pAppManager = SelectedAppManager as ACComponent;
                if (pAppManager == null)
                    return;
                if (pAppManager.IsProxy && pAppManager.ConnectionState == ACObjectConnectionState.DisConnected)
                {
                    // TODO: Message
                    return;
                }

                ACProdOrderManager poManager = ACProdOrderManager.GetServiceInstance(this);
                poManager.ActivateProdOrderStatesIntoProduction(DatabaseApp, SelectedBatchPlanForIntermediate, CurrentProdOrderPartslist, SelectedIntermediate, true);

                OnPropertyChanged("CurrentProdOrderPartslist");
                OnPropertyChanged("SelectedIntermediate");
                OnPropertyChanged("SelectedBatchPlanForIntermediate");
                bool succ = ACSaveChanges();
                if (!succ)
                    return;
                poManager.StartBatchPlan(DatabaseApp, pAppManager, CurrentProdOrderPartslist, SelectedIntermediate, acClassMethod);
            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), "StartBatchPlan", e.Message);
                if (e.InnerException != null)
                    Messages.LogException(this.GetACUrl(), "StartBatchPlan", e.InnerException.Message);
            }
            finally
            {
                _IsStartingBatchPlan = false;
            }
        }

        public bool IsEnabledStartBatchPlan()
        {
            return
                SelectedBatchPlanForIntermediate != null &&
                CurrentProdOrderPartslist != null &&
                SelectedIntermediate != null &&
                CurrentACClassMethod != null &&
                SelectedBatchPlanForIntermediate.PlanState < GlobalApp.BatchPlanState.AutoStart &&
                ACProdOrderManager.IsEnabledStartBatchPlan(SelectedBatchPlanForIntermediate, CurrentProdOrderPartslist, SelectedIntermediate);
        }

        public bool StartBatchPlanValidation()
        {
            bool success = true;
            ACSaveChanges();

            MsgWithDetails validationMsg = StartBatchValidation(DatabaseApp, CurrentProdOrderPartslist, MandatoryConfigStores);
            if (!validationMsg.IsSucceded())
            {
                if (CorrectInputData(validationMsg))
                    validationMsg = StartBatchValidation(DatabaseApp, CurrentProdOrderPartslist, MandatoryConfigStores);
            }
            if (!validationMsg.IsSucceded())
                success = false;
            return success;
        }

        public MsgWithDetails StartBatchValidation(DatabaseApp databaseApp, ProdOrderPartslist prodOrderPartslist, List<IACConfigStore> configStores)
        {
            MsgWithDetails msg = null;
            ACPartslistManager plManager = ACPartslistManager.GetServiceInstance(this);
            ACMatReqManager matReqManager = ACMatReqManager.GetServiceInstance(this);
            ACProdOrderManager poManager = ProdOrderManager;

            if (plManager != null)
            {
                using (var dbIPlus = new Database())
                {
                    msg = poManager.ValidateStart(databaseApp, dbIPlus, this, prodOrderPartslist,
                                                                configStores,
                                                                PARole.ValidationBehaviour.Strict,
                                                                plManager, matReqManager);

                    if (msg != null)
                    {
                        if (!msg.IsSucceded())
                        {
                            if (String.IsNullOrEmpty(msg.Message))
                            {
                                // Der Auftrag kann nicht gestartet werden weil:
                                msg.Message = Root.Environment.TranslateMessage(this, "Question50027");
                            }
                            Messages.Msg(msg, Global.MsgResult.OK, eMsgButton.OK);
                        }
                        else if (msg.HasWarnings())
                        {
                            if (String.IsNullOrEmpty(msg.Message))
                            {
                                //Möchten Sie den Auftrag wirklich starten? Es gibt nämlich folgende Probleme:
                                msg.Message = Root.Environment.TranslateMessage(this, "Question50028");
                            }
                            var userResult = Messages.Msg(msg, Global.MsgResult.No, eMsgButton.YesNo);
                            if (userResult == Global.MsgResult.No || userResult == Global.MsgResult.Cancel)
                                msg.AddDetailMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = Root.Environment.TranslateMessage(this, "txtWarningsAreNotAllowed") });
                        }
                    }

                    if (msg.IsSucceded())
                    {
                        if (poManager.SetBatchPlanValidated(databaseApp, prodOrderPartslist) > 0)
                            ACSaveChanges();
                    }
                }
            }

            return msg;
        }

        #region Dialog select App-Manager
        public VBDialogResult DialogResult { get; set; }

        [ACMethodCommand("Dialog", Const.Ok, (short)MISort.Okay)]
        public void DialogOK()
        {
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.OK;
            var selected = SelectedAppManager;
            CloseTopDialog();
            SelectedAppManager = selected;
        }

        [ACMethodCommand("Dialog", Const.Cancel, (short)MISort.Cancel)]
        public void DialogCancel()
        {
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.Cancel;
            var selected = SelectedAppManager;
            CloseTopDialog();
            SelectedAppManager = selected;
        }
        #endregion

        public gip.core.datamodel.ACClassWF GetACClassWFDischarging()
        {
            ProdOrderPartslist currentProdOrderPartslist = CurrentProdOrderPartslist != null ? CurrentProdOrderPartslist : ExternProdOrderPartslist;
            if (VBCurrentACClassWF == null
                || SelectedIntermediate == null
                || currentProdOrderPartslist == null
                || SelectedBatchPlanForIntermediate == null
                || currentProdOrderPartslist.Partslist.MaterialWF == null)
            {
                return null;
            }
            return ProdOrderManager.GetACClassWFDischarging(DatabaseApp, currentProdOrderPartslist, VBCurrentACClassWF, SelectedIntermediate);
        }

        #region Sub-Methods for BatchPlanForIntermediate
        /// <summary>
        /// New BatchPlanForIntermediate
        /// </summary>
        [ACMethodInteraction("BatchPlanForIntermediate", "en{'New Batchplan entry'}de{'Neuer Batchplan-Eintrag'}", (short)MISort.New, true, "CurrentBatchPlanForIntermediate", Global.ACKinds.MSMethodPrePost)]
        public virtual void NewBatchPlanForIntermediate()
        {
            if (!IsEnabledNewBatchPlanForIntermediate())
                return;
            if (!PreExecute("NewBatchPlanForIntermediate"))
                return;
            ProdOrderBatchPlan batchPlan = ProdOrderBatchPlan.NewACObject(DatabaseApp, CurrentProdOrderPartslist);
            batchPlan.ProdOrderPartslistPos = SelectedIntermediate;
            batchPlan.VBiACClassWF = VBCurrentACClassWF;
            batchPlan.Sequence = SelectedIntermediate.ProdOrderBatchPlan_ProdOrderPartslistPos.Count;
            SelectedIntermediate.ProdOrderBatchPlan_ProdOrderPartslistPos.Add(batchPlan);
            if (_BatchPlanForIntermediateList != null)
                _BatchPlanForIntermediateList.Add(batchPlan);
            SelectedBatchPlanForIntermediate = batchPlan;
            batchPlan.MaterialWFACClassMethod = CurrentProdOrderPartslist.Partslist.PartslistACClassMethod_Partslist
                                               .Where(c => c.MaterialWFACClassMethod.ACClassMethodID == CurrentACClassMethod.ACClassMethodID)
                                               .Select(c => c.MaterialWFACClassMethod)
                                               .FirstOrDefault();
            if (VarioConfigManager != null && ProdOrderManager != null && CurrentProdOrderPartslist.Partslist != null && VBCurrentACClassWF != null)
            {
                gip.core.datamodel.ACClassWF wfNode = VBCurrentACClassWF.FromIPlusContext<gip.core.datamodel.ACClassWF>(DatabaseApp.ContextIPlus);
                PartslistConfigExtract partslistConfigExtract = new PartslistConfigExtract(VarioConfigManager, ProdOrderManager, CurrentProdOrderPartslist.Partslist, wfNode, VBCurrentACClassWF);
                if (partslistConfigExtract != null)
                {
                    IACConfig batchPlanMode = partslistConfigExtract.GetConfig(ProdOrderBatchPlan.C_PlanMode);
                    if (batchPlanMode != null && batchPlanMode.Value != null)
                    {
                        batchPlan.PlanMode = (BatchPlanMode)batchPlanMode.Value;
                    }
                }
            }


            OnPropertyChanged("BatchPlanForIntermediateList");

            PostExecute("NewBatchPlanForIntermediate");
        }

        /// <summary>
        /// Is enabled new BatchPlanForIntermediate
        /// </summary>
        /// <returns><c>true</c> if [is enabled new BatchPlanForIntermediate]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewBatchPlanForIntermediate()
        {
            if (CurrentProdOrderPartslist == null || SelectedIntermediate == null)
                return false;
            return true;
        }

        /// <summary>
        /// Deletes BatchPlanForIntermediate
        /// </summary>
        [ACMethodInteraction("BatchPlanForIntermediate", "en{'Delete Batchplan entry'}de{'Batchplan löschen'}", (short)MISort.Delete, true, "CurrentBatchPlanForIntermediate", Global.ACKinds.MSMethodPrePost)]
        public void DeleteBatchPlanForIntermediate()
        {
            if (!IsEnabledDeleteBatchPlanForIntermediate())
                return;
            if (!PreExecute("DeleteBatchPlanForIntermediate"))
                return;
            Msg msg = SelectedBatchPlanForIntermediate.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            if (_BatchPlanForIntermediateList != null)
                _BatchPlanForIntermediateList.Remove(SelectedBatchPlanForIntermediate);
            PostExecute("DeleteBatchPlanForIntermediate");
            OnPropertyChanged("BatchPlanForIntermediateList");
        }

        /// <summary>
        /// Is deletion of BatchPlanForIntermediate allowed
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete BatchPlanForIntermediate]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteBatchPlanForIntermediate()
        {
            return SelectedIntermediate != null && SelectedBatchPlanForIntermediate != null;
        }

        [ACMethodInfo("GetCalculatedDates", "en{'Calculate production time'}de{'Produktionsdauer berechnen'}", 200)]
        public void GetCalculatedDates()
        {
            TimeSpan? batchPlanDuration = GetConfigBatchPlanDuration(SelectedBatchPlanForIntermediate);
            if (batchPlanDuration == null)
                batchPlanDuration = GetCalculatedBatchPlanDuration(SelectedBatchPlanForIntermediate);
            if (batchPlanDuration != null)
            {
                if (SelectedBatchPlanForIntermediate.ScheduledStartDate != null)
                    SelectedBatchPlanForIntermediate.ScheduledEndDate = SelectedBatchPlanForIntermediate.ScheduledStartDate.Value.Add(batchPlanDuration.Value);
                else if (SelectedBatchPlanForIntermediate.ScheduledEndDate != null)
                    SelectedBatchPlanForIntermediate.ScheduledStartDate = SelectedBatchPlanForIntermediate.ScheduledEndDate.Value.Add(-batchPlanDuration.Value);
            }
            SelectedBatchPlanForIntermediate.OnEntityPropertyChanged("ScheduledStartDate");
            SelectedBatchPlanForIntermediate.OnEntityPropertyChanged("ScheduledEndDate");
        }

        #region BatchPlan calculated dates

        private TimeSpan? GetConfigBatchPlanDuration(ProdOrderBatchPlan batchPlan)
        {
            return null;
        }

        private TimeSpan? GetCalculatedBatchPlanDuration(ProdOrderBatchPlan batchPlan)
        {
            ACProdOrderManager poManager = ACProdOrderManager.GetServiceInstance(this);
            return poManager.GetCalculatedBatchPlanDuration(DatabaseApp, batchPlan.MaterialWFACClassMethodID ?? Guid.Empty, batchPlan.VBiACClassWFID ?? Guid.Empty);
        }

        #endregion

        public bool IsEnabledGetCalculatedDates()
        {
            return SelectedBatchPlanForIntermediate != null &&
                (SelectedBatchPlanForIntermediate.ScheduledStartDate != null || SelectedBatchPlanForIntermediate.ScheduledEndDate != null);
        }

        #endregion

        #region Event-Callbacks
        void SelectionDialog_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var selectionDialog = SelectionDialog;
            if (selectionDialog != null)
            {
                IACObject currentSelection1 = selectionDialog.CurrentSelection;
                CurrentPWInfo = currentSelection1 as IACComponentPWNode;
            }
        }

        void ProdOrderBSO_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        public override void TaskCallback(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject)
        {
            if (e != null)
            {
                IACTask taskEntry = wrapObject as IACTask;
                if (taskEntry.State == PointProcessingState.Deleted && taskEntry.InProcess)
                {
                    //_WorkflowTestCallbacks++;
                }
                else if (taskEntry.State == PointProcessingState.Accepted && taskEntry.InProcess)
                {
                    gip.core.autocomponent.ACRoot.Dispatcher.Add(delegate
                    {
                        //ACPointAsyncRMIWrap<ACComponent> rmiWrap = wrapObject as ACPointAsyncRMIWrap<ACComponent>;
                        //if (rmiWrap.ExecutingInstance != null && rmiWrap.ExecutingInstance.IsObjLoaded)
                        //{
                        //    if (CurrentPWRootLive == null)
                        //    {
                        //        if (rmiWrap.ExecutingInstance.ValueT.IsProxy)
                        //        {
                        //            ACComponentProxy proxyInstance = rmiWrap.ExecutingInstance.ValueT as ACComponentProxy;
                        //            if (proxyInstance != null)
                        //                proxyInstance.ReloadChildsOverServerInstanceInfo(true);
                        //            CurrentPWRootLive = new ACRef<ACComponent>(proxyInstance, this);
                        //        }
                        //        else
                        //        {
                        //            CurrentPWRootLive = new ACRef<ACComponent>(rmiWrap.ExecutingInstance.ValueT as ACComponent, this);
                        //        }
                        //    }
                        //}
                    });
                }
            }
        }
        #endregion

        #region Routing

        [ACMethodInfo("", "en{'Set route'}de{'Route definieren'}", 999, true)]
        public void SetRoute()
        {
            IACVBBSORouteSelector routeSelector = ParentACComponent.ACUrlCommand("VBBSORouteSelector_Child") as IACVBBSORouteSelector;
            if (routeSelector == null)
            {
                Messages.Error(this, "Route selector is not installed");
                return;
            }
            if (!IsRoutingServiceAvailable)
            {
                Messages.Error(this, "Routing-Service is currently not available");
                return;
            }

            string targetCompACUrl = SelectedTarget.Module.ACUrlComponent;

            ACRoutingParameters routingParameters = new ACRoutingParameters()
            {
                RoutingService = this.RoutingService,
                Database = this.Database.ContextIPlus,
                SelectionRuleID = PAProcessModule.SelRuleID_ProcessModule,
                Direction = RouteDirections.Backwards,
                MaxRouteAlternativesInLoop = 1,
                IncludeReserved = true,
                IncludeAllocated = true
            };

            var sources = ACRoutingService.MemFindSuccessors(targetCompACUrl, routingParameters);
            if (sources == null)
            {
                Messages.Info(this, string.Format("Successors are not found for the component with ACUrl {0}!", targetCompACUrl));
                return;
            }

            if (sources != null && sources.Message != null)
            {
                Messages.Msg(sources.Message);
                return;
            }

            List<core.datamodel.ACClass> possibleSources = sources.Routes.Select(c => c.FirstOrDefault().Source).ToList();
            core.datamodel.ACClass start = null;

            if (CurrentACClassWF != null)
                start = ConfigManagerIPlus.FilterByAllowedInstances(CurrentACClassWF, MandatoryConfigStores, VarioConfigManager, possibleSources).FirstOrDefault();

            routeSelector.ShowAvailableRoutes(possibleSources, new core.datamodel.ACClass[] { SelectedTarget.Module }, null, null, true, start);

            if (routeSelector.RouteResult != null)
            {
                Route route = Route.MergeRoutes(routeSelector.RouteResult);
                SelectedTarget.CurrentRoute = route;
            }
            else
            {

            }
        }

        public bool IsEnabledSetRoute()
        {
            return SelectedTarget != null && SelectedTarget.SelectedReservation != null;
        }

        [ACMethodInfo("", "en{'Open route'}de{'Route anzeigen'}", 999, true)]
        public void OpenRoute()
        {
            IACVBBSORouteSelector routeSelector = ParentACComponent.ACUrlCommand("VBBSORouteSelector_Child") as IACVBBSORouteSelector;
            if (routeSelector == null)
            {
                Messages.Error(this, "Route selector is not installed");
                return;
            }

            //set flag to true if route is read only
            routeSelector.EditRoutesWithAttach(SelectedTarget.CurrentRoute, false, true, true);

            if (routeSelector.RouteResult != null)
            {
                Route route = Route.MergeRoutes(routeSelector.RouteResult);
                if (route != SelectedTarget.CurrentRoute)
                    SelectedTarget.CurrentRoute = route;
            }
        }

        public bool IsEnabledOpenRoute()
        {
            return SelectedTarget != null && SelectedTarget.CurrentRoute != null;
        }

        [ACMethodInfo("", "en{'Route check over orders'}de{'Routenprüfung über Aufträge'}", 9999, true)]
        public void RunPossibleRoutesCheck()
        {
            CalculateRouteResult = null;
            CurrentProgressInfo.ProgressInfoIsIndeterminate = true;

            InvokeCalculateRoutesAsync();

            ShowDialog(this, "CalculatedRouteDialog");
        }

        public bool IsEnabledPossibleRoutesCheck()
        {
            return SelectedBatchPlanForIntermediate != null;
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
            ACComponent paWorkflowScheduler = BSOBatchPlanSchedulerBSO?.PAWorkflowScheduler;
            if (paWorkflowScheduler == null)
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

                paWorkflowScheduler = Root.ACUrlCommand(acUrl) as ACComponent;
            }

            if (paWorkflowScheduler == null)
            {
                Messages.Msg(new Msg(eMsgLevel.Error, "Workflow scheduler is not installed or you have not rights"));
                return false;
            }

            // 1. Invoke ACUrlACTypeSignature for getting a default-ACMethod-Instance
            ACMethod acMethod = paWorkflowScheduler.ACUrlACTypeSignature("!RunRouteCalculation", gip.core.datamodel.Database.GlobalDatabase);

            // 2. Fill out all important parameters
            acMethod.ParameterValueList.GetACValue("RouteCalculation").Value = true;


            myRequestEntryA = RMISubscr.InvokeAsyncMethod(paWorkflowScheduler, "RMIPoint", acMethod, RMICallback);
            if (myRequestEntryA != null)
                myTestRequestID = myRequestEntryA.RequestID;
            return myRequestEntryA != null;
        }

        public void OnCalculateRoutesCallback()
        {
            var batchPlans = ProdOrderManager.GetProductionLinieBatchPlansWithPWNode(DatabaseApp, GlobalApp.BatchPlanState.Created, GlobalApp.BatchPlanState.Paused, null, null, null, null, null, null, null);

            List<FacilityReservation> reservations = new List<FacilityReservation>();

            foreach (ProdOrderBatchPlan batchPlan in batchPlans)
            {
                if ((BSOBatchPlanSchedulerBSO != null && BSOBatchPlanSchedulerBSO.ProdOrderBatchPlanList.Any(c => c.ProdOrderBatchPlanID == batchPlan.ProdOrderBatchPlanID))
                    || SelectedBatchPlanForIntermediate.ProdOrderBatchPlanID == batchPlan.ProdOrderBatchPlanID)
                    continue;

                reservations.AddRange(batchPlan.FacilityReservation_ProdOrderBatchPlan);
            }

            var myReservations = SelectedBatchPlanForIntermediate.FacilityReservation_ProdOrderBatchPlan.ToArray();

            List<FacilityReservation> result = new List<FacilityReservation>();

            foreach (FacilityReservation reservation in myReservations)
            {
                if (reservation.CalculatedRoute != null)
                {
                    string[] splitedRoute = reservation.CalculatedRoute.Split(new char[] { ',' });

                    foreach (string routeHash in splitedRoute)
                    {
                        var items = reservations.Where(c => c.CalculatedRoute != null && c.CalculatedRoute.Contains(routeHash));
                        if (items.Any())
                            result.AddRange(items);
                    }
                }
            }

            string calculateRouteResult;

            if (result.Any())
            {
                List<string> reservationsWithSameRoute = result.Where(c => c.ProdOrderBatchPlan != null).Select(c => c.ProdOrderBatchPlan.ProdOrderPartslist.ProdOrder.ProgramNo).Distinct().ToList();
                IEnumerable<string> pickingsWithSameRoute = result.Where(c => c.PickingPos != null).Select(c => c.PickingPos.Picking.PickingNo).Distinct();
                if (pickingsWithSameRoute != null && pickingsWithSameRoute.Any())
                    reservationsWithSameRoute.AddRange(pickingsWithSameRoute);

                if (reservationsWithSameRoute != null && reservationsWithSameRoute.Any())
                {
                    calculateRouteResult = "The following orders order may use same module: " + string.Join(", ", reservationsWithSameRoute);
                }
                else
                {
                    calculateRouteResult = "There no order which will use equipment from this order!";
                }
            }
            else
                calculateRouteResult = "There no order which will use equipment from this order!"; ;

            if (BSOBatchPlanSchedulerBSO != null)
            {
                BSOBatchPlanSchedulerBSO.CalculateRouteResult = calculateRouteResult;
                BSOBatchPlanSchedulerBSO.CurrentProgressInfo.ProgressInfoIsIndeterminate = false;
            }
            else
            {
                CalculateRouteResult = calculateRouteResult;
                CurrentProgressInfo.ProgressInfoIsIndeterminate = false;
            }
        }

        #endregion

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(StartBatchPlan):
                    StartBatchPlan();
                    return true;
                case nameof(IsEnabledStartBatchPlan):
                    result = IsEnabledStartBatchPlan();
                    return true;
                case nameof(DialogOK):
                    DialogOK();
                    return true;
                case nameof(DialogCancel):
                    DialogCancel();
                    return true;
                case nameof(NewBatchPlanForIntermediate):
                    NewBatchPlanForIntermediate();
                    return true;
                case nameof(IsEnabledNewBatchPlanForIntermediate):
                    result = IsEnabledNewBatchPlanForIntermediate();
                    return true;
                case nameof(DeleteBatchPlanForIntermediate):
                    DeleteBatchPlanForIntermediate();
                    return true;
                case nameof(OpenRoute):
                    OpenRoute();
                    return true;
                case nameof(IsEnabledOpenRoute):
                    result = IsEnabledOpenRoute();
                    return true;
                case nameof(SetRoute):
                    SetRoute();
                    return true;
                case nameof(IsEnabledSetRoute):
                    result = IsEnabledSetRoute();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }
}
