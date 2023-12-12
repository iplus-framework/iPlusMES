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
using System.Data.Objects;

namespace gip.bso.logistics
{

    /// <summary>
    /// Unter-BSO für VBBSOControlDialog
    /// Wird verwendet für PWBase (Workflowwelt) und Ableitungen
    /// 
    /// </summary>
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Destination planning Picking'}de{'Zielplanung Picking'}", Global.ACKinds.TACBSOGlobal, Global.ACStorableTypes.NotStorable, false, true)]
    public class BSOPickingTargetSelector : VBBSOModulesSelector
    {

        #region c´tors

        public BSOPickingTargetSelector(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            _PickingManager = ACPickingManager.ACRefToServiceInstance(this);
            if (_PickingManager == null)
                throw new Exception("ProdOrderManager not configured");

            _VarioConfigManager = ConfigManagerIPlus.ACRefToServiceInstance(this);
            if (_VarioConfigManager == null)
                throw new Exception("VarioConfigManager not configured");

            CurrentPWInfo = ParentACComponent.ACUrlCommand("CurrentSelection") as IACComponentPWNode;

            if (SelectionDialog != null)
                SelectionDialog.PropertyChanged += SelectionDialog_PropertyChanged;
            if (PickingBSO != null)
                PickingBSO.PropertyChanged += ProdOrderBSO_PropertyChanged;
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (SelectionDialog != null)
                SelectionDialog.PropertyChanged -= SelectionDialog_PropertyChanged;
            if (PickingBSO != null)
                PickingBSO.PropertyChanged -= ProdOrderBSO_PropertyChanged;
            this._CurrentACClassWF = null;
            this._CurrentPWInfo = null;

            if (_PickingManager != null)
                ACPickingManager.DetachACRefFromServiceInstance(this, _PickingManager);
            _PickingManager = null;

            if (_VarioConfigManager != null)
                ConfigManagerIPlus.DetachACRefFromServiceInstance(this, _VarioConfigManager);
            _VarioConfigManager = null;

            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Managers

        protected ACRef<ACPickingManager> _PickingManager = null;
        public ACPickingManager PickingManager
        {
            get
            {
                if (_PickingManager == null)
                    return null;
                return _PickingManager.ValueT;
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
                    if (DatabaseApp.IsChanged && PickingBSO != null)
                    {
                        PickingBSO.Save();
                    }
                    LoadBatchPlan();
                    //SelectedWFProductionLine = GetSchedulingGroup(value);
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
        //private MDSchedulingGroup _SelectedWFProductionLine;
        ///// <summary>
        ///// Selected property for MDSchedulingGroup
        ///// </summary>
        ///// <value>The selected WFProductionLine</value>
        //[ACPropertySelected(9999, "WFProductionLine", "en{'Production Line'}de{'Production Line'}")]
        //public MDSchedulingGroup SelectedWFProductionLine
        //{
        //    get
        //    {
        //        return _SelectedWFProductionLine;
        //    }
        //    set
        //    {
        //        if (_SelectedWFProductionLine != value)
        //        {
        //            _SelectedWFProductionLine = value;
        //            OnPropertyChanged("SelectedWFProductionLine");
        //        }
        //    }
        //}


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

        //private List<MDSchedulingGroup> _WFProductionLineList;
        ///// <summary>
        ///// List property for MDSchedulingGroup
        ///// </summary>
        ///// <value>The WFProductionLine list</value>
        //[ACPropertyList(9999, "WFProductionLine")]
        //public List<MDSchedulingGroup> WFProductionLineList
        //{
        //    get
        //    {
        //        if (_WFProductionLineList == null)
        //            _WFProductionLineList = LoadWFProductionLineList();
        //        return _WFProductionLineList;
        //    }
        //}

        //private List<MDSchedulingGroup> LoadWFProductionLineList()
        //{
        //    return DatabaseApp.MDSchedulingGroup.OrderBy(c => c.MDKey).ToList();
        //}


        //public MDSchedulingGroup GetSchedulingGroup(core.datamodel.ACClassWF aclassWF)
        //{
        //    Type typeOfSelectedNode = aclassWF?.PWACClass?.ObjectType;
        //    if (typeOfSelectedNode == null)
        //        return null;
        //    if (!TypeOfPWNodeProcessWorkflow.IsAssignableFrom(typeOfSelectedNode))
        //        return null;

        //    return DatabaseApp.MDSchedulingGroupWF.Where(c => c.VBiACClassWFID == aclassWF.ACClassWFID).Select(c => c.MDSchedulingGroup).FirstOrDefault();
        //}
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

        public BSOPicking PickingBSO
        {
            get
            {
                return FindParentComponent<BSOPicking>();
            }
        }

        //public BSOPickingTargetSelectorScheduler BSOPickingTargetSelectorSchedulerBSO
        //{
        //    get
        //    {
        //        return FindParentComponent<BSOPickingTargetSelectorScheduler>();
        //    }
        //}

        //public ProdOrderPartslist ExternProdOrderPartslist { get; set; }


        public Picking CurrentPicking
        {
            get
            {
                var bso = PickingBSO;
                if (bso != null)
                {
                    return bso.SelectedPicking;
                }
                //else if (ExternProdOrderPartslist != null)
                //    return ExternProdOrderPartslist;
                else if (_CurrentPWInfo != null)
                {
                    PWNodeProcessWorkflowVB pwNodeReal = _CurrentPWInfo as PWNodeProcessWorkflowVB;
                    if (pwNodeReal != null && pwNodeReal.CurrentPicking != null)
                    {
                        return pwNodeReal.CurrentPicking.FromAppContext<Picking>(this.DatabaseApp);
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

        public PickingPos CurrentPickingPos
        {
            get
            {
                var bso = PickingBSO;
                if (bso != null)
                    return bso.SelectedPickingPos;
                return null;
            }
        }

        public override DatabaseApp DatabaseApp
        {
            get
            {
                var prodOrderBSO = PickingBSO;
                if (prodOrderBSO != null)
                    return prodOrderBSO.DatabaseApp;
                //var BSOPickingTargetSelectorSchedulerBSO = BSOPickingTargetSelectorSchedulerBSO;
                //if (BSOPickingTargetSelectorSchedulerBSO != null)
                //    return BSOPickingTargetSelectorSchedulerBSO.DatabaseApp;
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
        //private ProdOrderBatchPlan _SelectedBatchPlanForIntermediate;
        ///// <summary>
        ///// Selected property for ProdOrderBatchPlan
        ///// </summary>
        ///// <value>The selected BatchPlanForIntermediate</value>
        //[ACPropertySelected(9999, "BatchPlanForIntermediate", "en{'Batchplan'}de{'Batchplan'}")]
        //public ProdOrderBatchPlan SelectedBatchPlanForIntermediate
        //{
        //    get
        //    {
        //        return _SelectedBatchPlanForIntermediate;
        //    }
        //    set
        //    {
        //        bool changed = false;
        //        if (_SelectedBatchPlanForIntermediate != value)
        //        {
        //            if (_SelectedBatchPlanForIntermediate != null)
        //                _SelectedBatchPlanForIntermediate.PropertyChanged -= _SelectedBatchPlanForIntermediate_PropertyChanged;
        //            changed = true;
        //        }
        //        _SelectedBatchPlanForIntermediate = value;
        //        if (changed)
        //        {
        //            if (_SelectedBatchPlanForIntermediate != null)
        //                _SelectedBatchPlanForIntermediate.PropertyChanged += _SelectedBatchPlanForIntermediate_PropertyChanged;
        //            RefreshTargets();
        //            OnPropertyChanged("SumTotalSize");
        //            OnPropertyChanged("SumDiffSize");
        //            OnPropertyChanged("SumDiffBatchCount");
        //            OnPropertyChanged("SumDiffBatchCountAct");
        //        }
        //        OnPropertyChanged("SelectedBatchPlanForIntermediate");
        //    }
        //}

        //protected bool _UpdatingControlModeBatchPlan = false;
        //void _SelectedBatchPlanForIntermediate_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (_UpdatingControlModeBatchPlan || this.InitState != ACInitState.Initialized)
        //        return;

        //    try
        //    {
        //        if (e.PropertyName == "PlanModeIndex")
        //        {
        //            _UpdatingControlModeBatchPlan = true;
        //            SelectedBatchPlanForIntermediate.OnEntityPropertyChanged("BatchNoFrom");
        //            SelectedBatchPlanForIntermediate.OnEntityPropertyChanged("BatchNoTo");
        //            SelectedBatchPlanForIntermediate.OnEntityPropertyChanged("BatchTargetCount");
        //            SelectedBatchPlanForIntermediate.OnEntityPropertyChanged("BatchSize");
        //            SelectedBatchPlanForIntermediate.OnEntityPropertyChanged("TotalSize");
        //            SelectedBatchPlanForIntermediate.OnEntityPropertyChanged("YieldTotalSizeExpected");
        //        }
        //        else if (e.PropertyName == "BatchNoTo")
        //        {
        //            _UpdatingControlModeBatchPlan = true;
        //            SelectedBatchPlanForIntermediate.OnEntityPropertyChanged("BatchNoFrom");
        //            OnPropertyChanged("SumTotalSize");
        //            OnPropertyChanged("SumDiffSize");
        //            OnPropertyChanged("SumDiffBatchCount");
        //        }
        //        else if (e.PropertyName == "BatchNoFrom")
        //        {
        //            _UpdatingControlModeBatchPlan = true;
        //            SelectedBatchPlanForIntermediate.OnEntityPropertyChanged("BatchNoTo");
        //            OnPropertyChanged("SumTotalSize");
        //            OnPropertyChanged("SumDiffSize");
        //            OnPropertyChanged("SumDiffBatchCount");
        //        }
        //        else if (e.PropertyName == "BatchSize")
        //        {
        //            _UpdatingControlModeBatchPlan = true;
        //            SelectedBatchPlanForIntermediate.OnEntityPropertyChanged("BatchSize");
        //            OnPropertyChanged("SumTotalSize");
        //            OnPropertyChanged("SumDiffSize");
        //            OnPropertyChanged("SumDiffBatchCount");
        //            OnPropertyChanged("SumDiffBatchCountAct");
        //        }
        //        else if (e.PropertyName == "PlanState" || e.PropertyName == "PlanStateIndex")
        //        {
        //            if (SelectedBatchPlanForIntermediate.PlanState == GlobalApp.BatchPlanState.AutoStart)
        //            {
        //                SelectedBatchPlanForIntermediate.PlannedStartDate = DateTimeUtils.NowDST;
        //                if (!_IsStartingBatchPlan)
        //                {
        //                    // Eine manuelle Statusänderung auf <Autostart> hat möglicherweise keine Auswirkung auf die Prozesssteuerung. Bitte setzen Sie den Status wieder zurück auf <Neu angelegt> und drücken Sie die <Start Batch>-Taste!
        //                    Msg msgForAll = new Msg
        //                    {
        //                        Source = GetACUrl(),
        //                        MessageLevel = eMsgLevel.Info,
        //                        ACIdentifier = "SelectedBatchPlanForIntermediate(0)",
        //                        Message = Root.Environment.TranslateMessage(this, "Info50023")
        //                    };
        //                    Messages.Msg(msgForAll, Global.MsgResult.OK, eMsgButton.OK);

        //                }
        //            }
        //        }
        //        else if (e.PropertyName == "TotalSize" || e.PropertyName == "BatchTargetCount")
        //        {
        //            OnPropertyChanged("SumTotalSize");
        //            OnPropertyChanged("SumDiffSize");
        //            OnPropertyChanged("SumDiffBatchCount");
        //        }
        //    }
        //    finally
        //    {
        //        _UpdatingControlModeBatchPlan = false;
        //    }
        //}


        //private ObservableCollection<ProdOrderBatchPlan> _BatchPlanForIntermediateList;
        ///// <summary>
        ///// List property for ProdOrderBatchPlan
        ///// </summary>
        ///// <value>The BatchPlanForIntermediate list</value>
        //[ACPropertyList(9999, "BatchPlanForIntermediate")]
        //public ObservableCollection<ProdOrderBatchPlan> BatchPlanForIntermediateList
        //{
        //    get
        //    {
        //        if (_BatchPlanForIntermediateList == null)
        //            _BatchPlanForIntermediateList = new ObservableCollection<ProdOrderBatchPlan>();
        //        return _BatchPlanForIntermediateList;
        //    }
        //    set
        //    {
        //        _BatchPlanForIntermediateList = value;
        //        OnPropertyChanged("BatchPlanForIntermediateList");
        //    }
        //}

        //private ProdOrderPartslistPos _SelectedIntermediate;
        //[ACPropertySelected(9999, "Intermediate")]
        //public ProdOrderPartslistPos SelectedIntermediate
        //{
        //    get
        //    {
        //        return _SelectedIntermediate;
        //    }
        //    set
        //    {
        //        if (_SelectedIntermediate != value)
        //        {
        //            _SelectedIntermediate = value;
        //            OnPropertyChanged("SelectedIntermediate");
        //        }
        //    }
        //}

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
                if (CurrentPicking != null) return CurrentPicking;
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
                return PickingBSO != null && CurrentConfigStore != null && CurrentConfigACUrl != null;
            }
        }


        #endregion

        #region Methods

        public void LoadBatchPlan()
        {
            var prodOrderBSO = PickingBSO;
            try
            {

                if (_CurrentACClassWF == null || _CurrentACClassWF.PWACClass == null || !typeof(PWNodeProcessWorkflowVB).IsAssignableFrom(_CurrentACClassWF.PWACClass.ObjectType))
                {
                    return;
                }

                var currentPicking = CurrentPicking;
                if (currentPicking == null)
                {
                    // TODO: Show Message
                    return;
                }

                var currentPickingPos = CurrentPickingPos;
                if (currentPickingPos == null)
                    return;

                VBCurrentACClassWF = _CurrentACClassWF.FromAppContext<gip.mes.datamodel.ACClassWF>(DatabaseApp);

                MaterialWFACClassMethod materialWFACClassMethod = null;
                gip.core.datamodel.ACClassWF acClassWFDischarging = this.PickingManager.GetACClassWFDischarging(DatabaseApp, currentPicking, VBCurrentACClassWF, currentPickingPos, out materialWFACClassMethod);
                if (acClassWFDischarging == null || materialWFACClassMethod == null)
                    return;

                MaterialWFConnection matWFConnection = this.PickingManager.GetMaterialWFConnection(VBCurrentACClassWF, materialWFACClassMethod.MaterialWFID);
                if (matWFConnection == null)
                {
                    // TODO: Show Message
                    return;
                }

                _MandatoryConfigStores = PickingManager.GetCurrentConfigStores(_CurrentACClassWF, VBCurrentACClassWF, materialWFACClassMethod.MaterialWFID, currentPicking);

                //SelectedIntermediate = poManager.GetIntermediate(currentPicking, matWFConnection);
                //if (SelectedIntermediate == null)
                //{
                //    SelectedBatchPlanForIntermediate = null;
                //    BatchPlanForIntermediateList = null;
                //    // TODO: Show Message
                //    //OnNewAlarmOccurred(ProcessAlarm, "ProdOrderPartslistPos not found for intermediate Material", true);
                //    return;
                //}

                //if (ExternProdOrderPartslist == null)
                //    LoadBatchPlanForIntermediateList(true);

                RefreshTargets();

            }
            catch (Exception e)
            {
                string message = e.Message;
                if (e.InnerException != null)
                    message += e.InnerException.Message;
                Messages.Exception(this, message, true);
            }
        }

        //public void LoadBatchPlanForIntermediateList(bool autoLoad)
        //{
        //    if (autoLoad)
        //        SelectedIntermediate.ProdOrderBatchPlan_ProdOrderPartslistPos.AutoLoad(DatabaseApp);
        //    BatchPlanForIntermediateList = new ObservableCollection<ProdOrderBatchPlan>(SelectedIntermediate.ProdOrderBatchPlan_ProdOrderPartslistPos
        //        .Where(c => c.VBiACClassWFID.HasValue && c.VBiACClassWFID == VBCurrentACClassWF.ACClassWFID)
        //        .OrderBy(c => c.Sequence)
        //        .ThenBy(c => c.InsertDate)
        //        .ToArray());
        //    if (!BatchPlanForIntermediateList.Any())
        //    {
        //        SelectedBatchPlanForIntermediate = null;
        //        // TODO: Show MEssage Error00123: No batchplan found for this intermediate material
        //        return;
        //    }

        //    SelectedBatchPlanForIntermediate = _BatchPlanForIntermediateList.FirstOrDefault();
        //}

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
            Picking currentPicking = CurrentPicking; // != null ? CurrentPicking : ExternProdOrderPartslist;
            PickingPos pickingPos = CurrentPickingPos;
            if (currentPicking != null && PickingManager != null)
            {
               result = PickingManager.GetTargets(DatabaseApp, VarioConfigManager, RoutingService, VBCurrentACClassWF,
               currentPicking, pickingPos, CurrentConfigACUrl,               
               ShowCellsInRoute, ShowSelectedCells, ShowEnabledCells, ShowSameMaterialCells, PreselectFirstFacilityReservation);
            }
            TargetsList = result;
            SelectedTarget = TargetsList.FirstOrDefault();
        }

        protected virtual bool OnFilterTarget(RouteItem routeItem)
        {
            return false;
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
                case nameof(StartWorkflow):
                    result = Global.ControlModes.Enabled;
                    //if (ExternProdOrderPartslist != null)
                    //    result = Global.ControlModes.Hidden;
                    break;

            }

            return result;
        }
        #endregion

        // Static, if more instances active
        //private static bool _IsStartingBatchPlan = false;
        [ACMethodCommand("", "en{'Start Workflow'}de{'Starte Workflow'}", (short)MISort.Start)]
        public void StartWorkflow()
        {
            if (!IsEnabledStartWorkflow()) 
                return;
            //_IsStartingBatchPlan = true;
            try
            {
                if (!StartBatchPlanValidation())
                    return;

                gip.core.datamodel.ACClassMethod acClassMethod = CurrentACClassMethod.FromIPlusContext<gip.core.datamodel.ACClassMethod>(this.Database.ContextIPlus);
                if (acClassMethod == null)
                    return;
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

                var acProgramIDs = this.DatabaseApp.OrderLog.Where(c => c.PickingPosID.HasValue
                                                    && c.PickingPos.PickingID == CurrentPicking.PickingID)
                                         .Select(c => c.VBiACProgramLog.ACProgramID)
                                         .Distinct()
                                         .ToArray();

                if (acProgramIDs != null && acProgramIDs.Any())
                {
                    ChildInstanceInfoSearchParam searchParam = new ChildInstanceInfoSearchParam() { OnlyWorkflows = true, ACProgramIDs = acProgramIDs };
                    var childInstanceInfos = pAppManager.GetChildInstanceInfo(1, searchParam);
                    if (childInstanceInfos != null && childInstanceInfos.Any())
                    {
                        //var childInstanceInfo = childInstanceInfos.FirstOrDefault();
                        //string acUrlComand = String.Format("{0}\\{1}!{2}", childInstanceInfo.ACUrlParent, childInstanceInfo.ACIdentifier, PWMethodTransportBase.ReloadBPAndResumeACIdentifier);
                        //pAppManager.ACUrlCommand(acUrlComand);
                        return;
                    }
                }

                ACMethod acMethod = pAppManager.NewACMethod(acClassMethod.ACIdentifier);
                if (acMethod == null)
                    return;

                string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(gip.core.datamodel.ACProgram), gip.core.datamodel.ACProgram.NoColumnName, gip.core.datamodel.ACProgram.FormatNewNo, this);
                gip.core.datamodel.ACProgram program = gip.core.datamodel.ACProgram.NewACObject(this.Database.ContextIPlus, null, secondaryKey);
                program.ProgramACClassMethod = acClassMethod;
                program.WorkflowTypeACClass = acClassMethod.WorkflowTypeACClass;
                this.Database.ContextIPlus.ACProgram.AddObject(program);
                //CurrentProdOrderPartslist.VBiACProgramID = program.ACProgramID;
                if (ACSaveChanges())
                {
                    ACValue paramProgram = acMethod.ParameterValueList.GetACValue(gip.core.datamodel.ACProgram.ClassName);
                    if (paramProgram == null)
                        acMethod.ParameterValueList.Add(new ACValue(gip.core.datamodel.ACProgram.ClassName, typeof(Guid), program.ACProgramID));
                    else
                        paramProgram.Value = program.ACProgramID;

                    if (this.CurrentPicking != null)
                    {
                        ACValue acValuePPos = acMethod.ParameterValueList.GetACValue(Picking.ClassName);
                        if (acValuePPos == null)
                            acMethod.ParameterValueList.Add(new ACValue(Picking.ClassName, typeof(Guid), CurrentPicking.PickingID));
                        else
                            acValuePPos.Value = CurrentPicking.PickingID;
                    }

                    gip.core.datamodel.ACClassWF allowedWFNode = CurrentACClassWF; // SelectedPWNodeProcessWorkflow;
                    if (allowedWFNode != null)
                    {
                        ACValue paramACClassWF = acMethod.ParameterValueList.GetACValue(gip.core.datamodel.ACClassWF.ClassName);
                        if (paramACClassWF == null)
                            acMethod.ParameterValueList.Add(new ACValue(gip.core.datamodel.ACClassWF.ClassName, typeof(Guid), allowedWFNode.ACClassWFID));
                        else
                            paramACClassWF.Value = allowedWFNode.ACClassWFID;
                    }

                    pAppManager.ExecuteMethod(acClassMethod.ACIdentifier, acMethod);

                    //IACPointAsyncRMI rmiInvocationPoint = pAppManager.GetPoint(Const.TaskInvocationPoint) as IACPointAsyncRMI;
                    //if (rmiInvocationPoint != null)
                    //rmiInvocationPoint.AddTask(acMethod, this);

                }

            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), "StartBatchPlan", e.Message);
                if (e.InnerException != null)
                    Messages.LogException(this.GetACUrl(), "StartBatchPlan", e.InnerException.Message);
            }
            finally
            {
                //_IsStartingBatchPlan = false;
            }
        }

        public bool IsEnabledStartWorkflow()
        {
            if (this.CurrentPicking == null
                || CurrentACClassMethod == null
                || this.CurrentPicking.PickingState >= PickingStateEnum.InProcess
                || PickingBSO == null
                || PickingBSO.PickingPosList == null
                || !PickingBSO.PickingPosList.Any()
                || !PickingBSO.PickingPosList.Where(c => c.MDDelivPosLoadState.DelivPosLoadState == MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad).Any()
                || PickingManager == null
                || CurrentACClassWF == null)
                return false;
            return true;
        }

        public bool StartBatchPlanValidation()
        {
            bool success = true;
            ACSaveChanges();

            MsgWithDetails validationMsg = StartBatchValidation(DatabaseApp, CurrentPicking, MandatoryConfigStores);
            if (!validationMsg.IsSucceded())
            {
                if (CorrectInputData(validationMsg))
                    validationMsg = StartBatchValidation(DatabaseApp, CurrentPicking, MandatoryConfigStores);
            }
            if (!validationMsg.IsSucceded())
                success = false;
            return success;
        }

        public MsgWithDetails StartBatchValidation(DatabaseApp databaseApp, Picking picking, List<IACConfigStore> configStores)
        {
            MsgWithDetails msg = null;
            if (PickingManager == null)
                return new MsgWithDetails();


            using (var dbIPlus = new Database())
            {
                msg = this.PickingManager.ValidateStart(databaseApp, dbIPlus, CurrentPicking,
                                                            configStores,
                                                            PARole.ValidationBehaviour.Strict);

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

            }

            return msg;
        }

        #region Dialog select App-Manager
        public VBDialogResult DialogResult { get; set; }

        [ACMethodCommand("Dialog", "en{'OK'}de{'OK'}", (short)MISort.Okay)]
        public void DialogOK()
        {
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.OK;
            var selected = SelectedAppManager;
            CloseTopDialog();
            SelectedAppManager = selected;
        }

        [ACMethodCommand("Dialog", "en{'Cancel'}de{'Abbrechen'}", (short)MISort.Cancel)]
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
            Picking picking = CurrentPicking;// != null ? CurrentPicking : ExternProdOrderPartslist;
            if (VBCurrentACClassWF == null
                || picking == null)
            {
                return null;
            }
            MaterialWFACClassMethod materialWFACClassMethod = null;
            return PickingManager.GetACClassWFDischarging(DatabaseApp, picking, VBCurrentACClassWF, CurrentPickingPos, out materialWFACClassMethod);
        }


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
            var sources = ACRoutingService.MemFindSuccessors(RoutingService, Database.ContextIPlus, targetCompACUrl, PAProcessModule.SelRuleID_ProcessModule, RouteDirections.Backwards, 1,
                                                             true, true);
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

            routeSelector.ShowAvailableRoutes(sources.Routes.Select(c => c.FirstOrDefault().Source), new core.datamodel.ACClass[] { SelectedTarget.Module });

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
            routeSelector.EditRoutes(SelectedTarget.CurrentRoute, false, true, true);

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

        #endregion

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(StartWorkflow):
                    StartWorkflow();
                    return true;
                case nameof(IsEnabledStartWorkflow):
                    result = IsEnabledStartWorkflow();
                    return true;
                case nameof(DialogOK):
                    DialogOK();
                    return true;
                case nameof(DialogCancel):
                    DialogCancel();
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
