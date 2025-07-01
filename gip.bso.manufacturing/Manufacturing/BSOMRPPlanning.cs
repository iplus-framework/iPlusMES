// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, ConstApp.PlanningMR, Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + PlanningMR.ClassName)]
    public class BSOMRPPlanning : ACBSOvbNav
    {
        #region const

        public const string BGWorkerMethod_RunMRP = @"RunMRP";
        public const string BGWorkerMethod_ActivatePlanning = @"ActivatePlanning";
        public const string BGWorkerMethod_DeletePlanning = @"DeletePlanning";
        public const string CONST_WizardPlan = @"WizardPlan";

        #endregion

        #region c'tors

        public BSOMRPPlanning(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            _ProdOrderManager = ACProdOrderManager.ACRefToServiceInstance(this);
            if (_ProdOrderManager == null)
                throw new Exception("ProdOrderManager not configured");

            _PlanningMRManager = MRPPlanningManager.ACRefToServiceInstance(this);
            if (_ProdOrderManager == null)
                throw new Exception("PlanningMRManager not configured");

            CurrentLayoutEnum = MRPPlanningLayoutEnum.Preview;

            Search();

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_ProdOrderManager != null)
            {
                ACProdOrderManager.DetachACRefFromServiceInstance(this, _ProdOrderManager);
            }
            _ProdOrderManager = null;

            if (_PlanningMRManager != null)
            {
                MRPPlanningManager.DetachACRefFromServiceInstance(this, _PlanningMRManager);
            }
            _PlanningMRManager = null;
            _AccessPrimary.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;

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

        protected ACRef<MRPPlanningManager> _PlanningMRManager = null;
        public MRPPlanningManager PlanningMRManager
        {
            get
            {
                if (_PlanningMRManager == null)
                    return null;
                return _PlanningMRManager.ValueT;
            }
        }


        #endregion

        #region Properties

        #region Properties -> Layouts

        private MRPPlanningLayoutEnum _CurrentLayoutEnum;
        public MRPPlanningLayoutEnum CurrentLayoutEnum
        {
            get
            {
                return _CurrentLayoutEnum;
            }
            set
            {
                _CurrentLayoutEnum = value;
                CurrentLayout = GetLayout(value.ToString());
            }
        }


        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _CurrentLayout;
        [ACPropertyInfo(999, nameof(CurrentLayout), "en{'TODO:CurrentLayout'}de{'TODO:CurrentLayout'}")]
        public string CurrentLayout
        {
            get
            {
                return _CurrentLayout;
            }
            set
            {
                if (_CurrentLayout != value)
                {
                    _CurrentLayout = value;
                    OnPropertyChanged();
                }
            }
        }

        private MRPPlanningPhaseEnum _WizardLayoutEnum;
        public MRPPlanningPhaseEnum WizardLayoutEnum
        {
            get
            {
                return _WizardLayoutEnum;
            }
            set
            {
                _WizardLayoutEnum = value;
                WizardLayout = GetLayout(CONST_WizardPlan + value.ToString());
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _WizardLayout;
        [ACPropertyInfo(999, nameof(WizardLayout), "en{'TODO:WizardLayout'}de{'TODO:WizardLayout'}")]
        public string WizardLayout
        {
            get
            {
                return _WizardLayout;
            }
            set
            {
                if (_WizardLayout != value)
                {
                    _WizardLayout = value;
                    OnPropertyChanged();
                }
            }
        }

        #region Properties -> Layouts->PlanningPhase
        public const string PlanningPhase = "PlanningPhase";
        private ACValueItem _SelectedPlanningPhase;
        /// <summary>
        /// Selected property for ACValueItem
        /// </summary>
        /// <value>The selected PlanningPhase</value>
        [ACPropertySelected(9999, nameof(PlanningPhase), "en{'TODO: PlanningPhase'}de{'TODO: PlanningPhase'}")]
        public ACValueItem SelectedPlanningPhase
        {
            get
            {
                return _SelectedPlanningPhase;
            }
            set
            {
                if (_SelectedPlanningPhase != value)
                {
                    _SelectedPlanningPhase = value;
                    OnPropertyChanged();
                }
            }
        }


        private List<ACValueItem> _PlanningPhaseList;
        /// <summary>
        /// List property for ACValueItem
        /// </summary>
        /// <value>The PlanningPhase list</value>
        [ACPropertyList(9999, nameof(PlanningPhase))]
        public List<ACValueItem> PlanningPhaseList
        {
            get
            {
                if (_PlanningPhaseList == null)
                {
                    _PlanningPhaseList = DatabaseApp.MRPPlanningPhaseList.ToList();
                }
                return _PlanningPhaseList;
            }
        }

        #endregion


        #endregion

        #region Properties -> ACAccessNav (PlanningMR)
        public override IAccessNav AccessNav { get { return AccessPrimary; } }

        ACAccessNav<PlanningMR> _AccessPrimary;
        [ACPropertyAccessPrimary(9999, PlanningMR.ClassName)]
        public ACAccessNav<PlanningMR> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceColumnsIfDifferent(NavigationqueryDefaultFilter, NavigationqueryDefaultSort);
                    }
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<PlanningMR>(PlanningMR.ClassName, this);
                    _AccessPrimary.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;

                }
                return _AccessPrimary;
            }
        }

        private IQueryable<PlanningMR> _AccessPrimary_NavSearchExecuting(IQueryable<PlanningMR> result)
        {
            return result;
        }

        protected virtual List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {
                List<ACFilterItem> aCFilterItems = new List<ACFilterItem>();

                ACFilterItem filterNotTemplate = new ACFilterItem(Global.FilterTypes.filter, nameof(PlanningMR.Template), Global.LogicalOperators.equal, Global.Operators.and, "False", true, true);
                aCFilterItems.Add(filterNotTemplate);

                ACFilterItem filterPlanningMRNo = new ACFilterItem(Global.FilterTypes.filter, nameof(PlanningMR.PlanningMRNo), Global.LogicalOperators.contains, Global.Operators.or, null, true, true);
                aCFilterItems.Add(filterPlanningMRNo);

                return aCFilterItems;
            }
        }

        protected virtual List<ACSortItem> NavigationqueryDefaultSort
        {
            get
            {
                return new List<ACSortItem>()
                {
                    new ACSortItem(nameof(PlanningMR.PlanningMRNo), Global.SortDirections.descending, true)
                };
            }
        }

        [ACPropertySelected(9999, PlanningMR.ClassName)]
        public PlanningMR SelectedPlanningMR
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                if (AccessPrimary.Selected != value)
                {
                    AccessPrimary.Selected = value;
                    OnPropertyChanged("SelectedPlanningMR");
                    LoadPlanningDetails();
                }
            }
        }

        [ACPropertyCurrent(9999, PlanningMR.ClassName)]
        public PlanningMR CurrentPlanningMR
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                if (AccessPrimary.Current != value)
                {
                    AccessPrimary.Current = value;
                    OnPropertyChanged("CurrentPlanningMR");
                }
            }
        }


        [ACPropertyList(9999, PlanningMR.ClassName)]
        public IEnumerable<PlanningMR> PlanningMRList
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.NavList;
            }
        }

        #endregion

        #region Properties -> Filter

        private DateTime _PlanningHorizonFrom = DateTime.Today;
        [ACPropertySelected(998, "PlanningHorizonFrom", "en{'Planning Horizon From'}de{'Planungshorizont Von'}")]
        public DateTime PlanningHorizonFrom
        {
            get { return _PlanningHorizonFrom; }
            set
            {
                if (_PlanningHorizonFrom != value)
                {
                    _PlanningHorizonFrom = value;
                    OnPropertyChanged("PlanningHorizonFrom");
                }
            }
        }

        private DateTime _PlanningHorizonTo = DateTime.Today.AddDays(90);
        [ACPropertySelected(997, "PlanningHorizonTo", "en{'Planning Horizon To'}de{'Planungshorizont Bis'}")]
        public DateTime PlanningHorizonTo
        {
            get { return _PlanningHorizonTo; }
            set
            {
                if (_PlanningHorizonTo != value)
                {
                    _PlanningHorizonTo = value;
                    OnPropertyChanged("PlanningHorizonTo");
                }
            }
        }

        private bool _IncludeConsumptionForecast = true;
        [ACPropertySelected(996, "IncludeConsumptionForecast", "en{'Include Consumption Forecast'}de{'Verbrauchsprognose einbeziehen'}")]
        public bool IncludeConsumptionForecast
        {
            get { return _IncludeConsumptionForecast; }
            set
            {
                if (_IncludeConsumptionForecast != value)
                {
                    _IncludeConsumptionForecast = value;
                    OnPropertyChanged("IncludeConsumptionForecast");
                }
            }
        }

        #endregion

        #region Properties -> PlanningPositions (PlanningMRPos)

        private IEnumerable<PlanningMRPos> _PlanningPositions;
        [ACPropertyList(995, "PlanningPositions")]
        public IEnumerable<PlanningMRPos> PlanningPositions
        {
            get { return _PlanningPositions; }
            set
            {
                _PlanningPositions = value;
                OnPropertyChanged("PlanningPositions");
            }
        }

        private IEnumerable<PlanningMRProposal> _PlanningProposals;
        [ACPropertyList(994, "PlanningProposals")]
        public IEnumerable<PlanningMRProposal> PlanningProposals
        {
            get { return _PlanningProposals; }
            set
            {
                _PlanningProposals = value;
                OnPropertyChanged("PlanningProposals");
            }
        }

        #endregion


        #endregion

        #region BSO->ACMethod

        #region BSO->ACMethod->Standard

        [ACMethodCommand(PlanningMR.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        [ACMethodCommand(PlanningMR.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
        }

        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        [ACMethodInteraction(PlanningMR.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedPlanningMR", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<PlanningMR>(requery, () => SelectedPlanningMR, () => CurrentPlanningMR, c => CurrentPlanningMR = c,
                        DatabaseApp
                        .PlanningMR
                        .Where(c => c.PlanningMRID == SelectedPlanningMR.PlanningMRID));
            PostExecute("Load");
        }

        public bool IsEnabledLoad()
        {
            return SelectedPlanningMR != null;
        }

        [ACMethodCommand(PlanningMR.ClassName, ConstApp.Search, (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(PlanningMRList));
        }

        #endregion

        #region BSO->ACMethod->PlanningMR new / delete

        [ACMethodInteraction(PlanningMR.ClassName, Const.New, (short)MISort.New, true, nameof(SelectedPlanningMR), Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(PlanningMR), PlanningMR.NoColumnName, PlanningMR.FormatNewNo, this);
            PlanningMR planningMR = PlanningMR.NewACObject(DatabaseApp, null, secondaryKey);
            planningMR.PlanningName = $"MRP Planning {DateTime.Now:yyyy-MM-dd HH:mm}";
            planningMR.RangeFrom = PlanningHorizonFrom;
            planningMR.RangeTo = PlanningHorizonTo;

            AccessPrimary.NavList.Add(planningMR);
            DatabaseApp.PlanningMR.AddObject(planningMR);
            CurrentPlanningMR = planningMR;
            OnPropertyChanged(nameof(PlanningMRList));
        }

        public bool IsEnabledNew()
        {
            return true;
        }

        [ACMethodInteraction(PlanningMR.ClassName, ConstApp.Delete, (short)MISort.Delete, true, nameof(CurrentPlanningMR), Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (AccessPrimary == null || CurrentPlanningMR == null)
                return;

            Global.MsgResult result = Messages.Question(this, "Question50097", Global.MsgResult.Yes, false, CurrentPlanningMR.PlanningMRNo);
            if (result == Global.MsgResult.Yes)
            {
                BackgroundWorker.RunWorkerAsync(BGWorkerMethod_DeletePlanning);
                ShowDialog(this, DesignNameProgressBar);
            }
        }

        public bool IsEnabledDelete()
        {
            return CurrentPlanningMR != null;
        }

        #endregion

        #region BSO->ACMethod->Wizard


        /// <summary>
        /// Source Property: Edit
        /// </summary>
        [ACMethodInfo(nameof(Edit), "en{'Edit'}de{'Bearbeiten'}", 999)]
        public void Edit()
        {
            if (!IsEnabledEdit())
                return;

            BackgroundWorker.RunWorkerAsync(nameof(DoLoadMRPData));
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledEdit()
        {
            return
                SelectedPlanningMR != null
                && SelectedPlanningMR.PlanningMRPhaseIndex < (short)MRPPlanningPhaseEnum.Finished
                && CurrentLayoutEnum == MRPPlanningLayoutEnum.Preview;
        }

        /// <summary>
        /// Source Property: Edit
        /// </summary>
        [ACMethodInfo("Edit", Const.Cancel, 999)]
        public void CancelEdit()
        {
            if (!IsEnabledCancelEdit())
                return;


            DatabaseApp.ACUndoChanges();
            LoadPreview();
            CurrentLayoutEnum = MRPPlanningLayoutEnum.Preview;
        }

        private void LoadPreview()
        {

        }

        public bool IsEnabledCancelEdit()
        {
            return CurrentLayout != MRPPlanningLayoutEnum.Preview.ToString();
        }


        /// <summary>
        /// Source Property: WizardForward
        /// </summary>
        [ACMethodInfo(nameof(WizardForward), ConstApp.Forward, 999)]
        public void WizardForward()
        {
            if (!IsEnabledWizardForward())
                return;

            BackgroundWorker.RunWorkerAsync(nameof(DoPlanningForward));
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledWizardForward()
        {
            return
                CurrentPlanningMR != null
                && CurrentPlanningMR.PlanningMRPhaseIndex < (short)MRPPlanningPhaseEnum.Finished
                && CurrentLayoutEnum != MRPPlanningLayoutEnum.Preview;
        }

        /// <summary>
        /// Source Property: WizardForward
        /// </summary>
        [ACMethodInfo(nameof(WizardBackward), ConstApp.Backward, 999)]
        public void WizardBackward()
        {
            if (!IsEnabledWizardForward())
                return;

            BackgroundWorker.RunWorkerAsync(nameof(DoPlanningBackward));
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledWizardBackward()
        {
            return
                CurrentPlanningMR != null
                && CurrentPlanningMR.PlanningMRPhaseIndex > (short)MRPPlanningPhaseEnum.PlanDefinition
                && CurrentLayoutEnum != MRPPlanningLayoutEnum.Preview;
        }


        #endregion


        #region MRP Specific Operations
        [ACMethodInfo("RunMRPCalculation", "en{'Run MRP Calculation'}de{'MRP Berechnung starten'}", 100)]
        public void RunMRPCalculation()
        {
            if (!IsEnabledRunMRPCalculation())
                return;

            // Create new planning if none selected
            if (CurrentPlanningMR == null)
            {
                New();
            }

            // Update planning horizon
            CurrentPlanningMR.RangeFrom = PlanningHorizonFrom;
            CurrentPlanningMR.RangeTo = PlanningHorizonTo;

            BackgroundWorker.RunWorkerAsync(BGWorkerMethod_RunMRP);
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledRunMRPCalculation()
        {
            return !BackgroundWorker.IsBusy && PlanningHorizonFrom < PlanningHorizonTo;
        }

        [ACMethodInfo("ActivatePlanning", "en{'Activate Planning'}de{'Planung aktivieren'}", 101)]
        public void ActivatePlanning()
        {
            if (!IsEnabledActivatePlanning())
                return;

            Global.MsgResult result = Messages.Question(this, "Question50098", Global.MsgResult.Yes, false, CurrentPlanningMR.PlanningMRNo);
            if (result == Global.MsgResult.Yes)
            {
                BackgroundWorker.RunWorkerAsync(BGWorkerMethod_ActivatePlanning);
                ShowDialog(this, DesignNameProgressBar);
            }
        }

        public bool IsEnabledActivatePlanning()
        {
            return !BackgroundWorker.IsBusy;
            //CurrentPlanningMR != null &&
            //CurrentPlanningMR.PlanningMRProposal_PlanningMR.Any();
        }

        [ACMethodInfo("RefreshPlanningData", "en{'Refresh Planning Data'}de{'Planungsdaten aktualisieren'}", 102)]
        public void RefreshPlanningData()
        {
            LoadPlanningDetails();
        }

        public bool IsEnabledRefreshPlanningData()
        {
            return CurrentPlanningMR != null;
        }
        #endregion

        #endregion

        #region Private Methods
        private void LoadPlanningDetails()
        {
            if (CurrentPlanningMR == null)
            {
                PlanningPositions = null;
                PlanningProposals = null;
                return;
            }

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                var planningMR = dbApp.PlanningMR
                    .Where(p => p.PlanningMRID == CurrentPlanningMR.PlanningMRID)
                    .FirstOrDefault();

                if (planningMR != null)
                {
                    //PlanningPositions = planningMR.PlanningMRPos_PlanningMR.OrderBy(p => p.ExpectedBookingDate).ToList();
                    //PlanningProposals = planningMR.PlanningMRProposal_PlanningMR.ToList();
                    //ConsumptionForecasts = planningMR.PlanningMRCons_PlanningMR.OrderBy(c => c.ConsumptionDate).ToList();
                }
            }
        }
        #endregion

        #region BackgroundWorker
        public override void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            base.BgWorkerDoWork(sender, e);
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = e.Argument.ToString();

            worker.ProgressInfo.OnlyTotalProgress = true;
            worker.ProgressInfo.AddSubTask(command, 0, 9);
            string message = Translator.GetTranslation("en{'Running {0}...'}de{'{0} läuft...'}");
            worker.ProgressInfo.ReportProgress(command, 0, string.Format(message, command));

            switch (command)
            {
                case nameof(DoLoadMRPData):
                    e.Result = new MRPResult();
                    DoLoadMRPData();
                    break;
                case nameof(DoPlanningForward):
                    e.Result = new MRPResult();
                    DoPlanningForward();
                    break;
                case nameof(DoPlanningBackward):
                    e.Result = new MRPResult();
                    DoPlanningBackward();
                    break;

                case BGWorkerMethod_RunMRP:
                    e.Result = ExecuteMRPCalculation(worker);
                    break;
                case BGWorkerMethod_ActivatePlanning:
                    e.Result = ActivatePlanningExecution(worker);
                    break;
                case BGWorkerMethod_DeletePlanning:
                    e.Result = DeletePlanningExecution();
                    break;
            }
        }

        public override void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            base.BgWorkerCompleted(sender, e);
            CloseWindow(this, DesignNameProgressBar);

            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = worker.EventArgs.Argument.ToString();

            if (e.Cancelled)
            {
                //MRPCalculationStatus = $"Operation {command} canceled by user!";
                Messages.Info(this, "Info50075", false, command);
            }
            else if (e.Error != null)
            {
                //MRPCalculationStatus = $"Error in {command}: {e.Error.Message}";
                Messages.Error(this, "Error50076", false, command, e.Error.Message);
            }
            else if (e.Result != null)
            {
                MRPResult result = e.Result as MRPResult;
                if (result != null)
                {
                    if (result.SaveMessage != null && !result.SaveMessage.IsSucceded())
                    {
                        Messages.Msg(result.SaveMessage);
                        //MRPCalculationStatus = "MRP Calculation failed - see messages for details";
                    }
                    else
                    {
                        switch (command)
                        {
                            case nameof(DoLoadMRPData):
                                // Do distribute data
                                SetSelectedPlanningPhase((MRPPlanningPhaseEnum)CurrentPlanningMR.PlanningMRPhaseIndex);
                                CurrentLayoutEnum = MRPPlanningLayoutEnum.Wizard;
                                WizardLayoutEnum = (MRPPlanningPhaseEnum)CurrentPlanningMR.PlanningMRPhaseIndex;
                                ;
                                break;
                            case nameof(DoPlanningForward):
                            case nameof(DoPlanningBackward):
                                SetSelectedPlanningPhase((MRPPlanningPhaseEnum)CurrentPlanningMR.PlanningMRPhaseIndex);
                                if (CurrentPlanningMR.PlanningMRPhaseIndex == (short)MRPPlanningPhaseEnum.Finished)
                                {
                                    CurrentLayoutEnum = MRPPlanningLayoutEnum.Preview;
                                }
                                else
                                {
                                    CurrentLayoutEnum = MRPPlanningLayoutEnum.Wizard;
                                    WizardLayoutEnum = (MRPPlanningPhaseEnum)CurrentPlanningMR.PlanningMRPhaseIndex;
                                }
                                OnPropertyChanged(CurrentLayout);
                                break;



                            case BGWorkerMethod_RunMRP:
                                //MRPCalculationStatus = $"MRP Calculation completed successfully. Created {result.CreatedProposals} proposals.";
                                Messages.Info(this, "Info50077", false, result.CreatedProposals);
                                LoadPlanningDetails();
                                break;
                            case BGWorkerMethod_ActivatePlanning:
                                //MRPCalculationStatus = $"Planning activated successfully. Created {result.CreatedOrders} orders.";
                                Messages.Info(this, "Info50078", false, result.CreatedOrders);
                                break;
                            case BGWorkerMethod_DeletePlanning:
                                //MRPCalculationStatus = "Planning deleted successfully.";
                                Messages.Info(this, "Info50079", false);
                                Search();
                                break;
                        }
                    }
                }
            }
        }

        #region BackgroundWorker Methods

        private void DoLoadMRPData()
        {

        }

        private void DoPlanningForward()
        {
            PlanningMRManager.PlanningForward(DatabaseApp, CurrentPlanningMR);
        }

        public void DoPlanningBackward()
        {
            PlanningMRManager.PlanningBackward(DatabaseApp, CurrentPlanningMR);
        }

        private MRPResult ExecuteMRPCalculation(ACBackgroundWorker worker)
        {
            MRPResult result = new MRPResult();

            try
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    worker.ProgressInfo.ReportProgress("RunMRP", 1, "Phase 1: Clearing existing planning data...");

                    // Phase 1: Clear existing planning data
                    ClearExistingPlanningData(dbApp);

                    worker.ProgressInfo.ReportProgress("RunMRP", 2, "Phase 2: Analyzing current demands...");

                    // Phase 2: Calculate current positions (demands and supplies)
                    CalculateCurrentPositions(dbApp, worker);

                    worker.ProgressInfo.ReportProgress("RunMRP", 4, "Phase 3: Performing BOM explosion...");

                    // Phase 3: BOM explosion and planned order creation
                    PerformBOMExplosion(dbApp, worker);

                    worker.ProgressInfo.ReportProgress("RunMRP", 7, "Phase 4: Consolidating requirements...");

                    // Phase 4: Consolidate material requirements
                    ConsolidateMaterialRequirements(dbApp, worker);

                    worker.ProgressInfo.ReportProgress("RunMRP", 8, "Phase 5: Saving results...");

                    // Save changes
                    MsgWithDetails saveMsg = dbApp.ACSaveChanges();
                    if (saveMsg != null)
                    {
                        result.SaveMessage = saveMsg;
                    }
                    else
                    {
                        // result.CreatedProposals = CurrentPlanningMR.PlanningMRProposal_PlanningMR.Count();
                    }

                    worker.ProgressInfo.ReportProgress("RunMRP", 9, "MRP Calculation completed!");
                }
            }
            catch (Exception ex)
            {
                result.SaveMessage = new MsgWithDetails() { };
                result.SaveMessage.AddDetailMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = ex.Message });
            }

            return result;
        }

        private void ClearExistingPlanningData(DatabaseApp dbApp)
        {
            //var existingPositions = dbApp.PlanningMRPos.Where(p => /*p.PlanningMRID == CurrentPlanningMR.PlanningMRID*/).ToList();
            //var existingProposals = dbApp.PlanningMRProposal.Where(p => p.PlanningMRID == CurrentPlanningMR.PlanningMRID).ToList();
            //var existingConsumption = dbApp.PlanningMRCons.Where(p => p.PlanningMRID == CurrentPlanningMR.PlanningMRID).ToList();

            //foreach (var pos in existingPositions)
            //    dbApp.PlanningMRPos.DeleteObject(pos);

            //foreach (var prop in existingProposals)
            //    dbApp.PlanningMRProposal.DeleteObject(prop);

            //foreach (var cons in existingConsumption)
            //    dbApp.PlanningMRCons.DeleteObject(cons);
        }

        private void CalculateCurrentPositions(DatabaseApp dbApp, ACBackgroundWorker worker)
        {
            // Get all open sales orders within planning horizon
            var openOutOrders = dbApp.OutOrderPos
                .Where(o => o.TargetDeliveryDate >= PlanningHorizonFrom &&
                           o.TargetDeliveryDate <= PlanningHorizonTo &&
                           o.MDOutOrderPosState.MDKey == "InProgress")
                .ToList();

            foreach (var outOrderPos in openOutOrders)
            {
                var planningPos = PlanningMRPos.NewACObject(dbApp, CurrentPlanningMR);
                //planningPos.MaterialID = outOrderPos.MaterialID;
                planningPos.OutOrderPosID = outOrderPos.OutOrderPosID;
                planningPos.StoreQuantityUOM = -outOrderPos.TargetQuantityUOM; // Negative = demand
                // planningPos.ExpectedBookingDate = outOrderPos.TargetDeliveryDate ?? DateTime.Now;

                //CurrentPlanningMR.PlanningMRPos_PlanningMR.Add(planningPos);
            }

            // Get all active production orders within planning horizon
            var activeProdOrders = dbApp.ProdOrderPartslist
                //.Where(p => p.TargetDeliveryDate >= PlanningHorizonFrom &&
                //           p.TargetDeliveryDate <= PlanningHorizonTo &&
                //           p.MDProdOrderState.MDKey == "InProgress")
                .ToList();

            foreach (var prodOrderPartslist in activeProdOrders)
            {
                // Add as supply (positive quantity)
                var supplyPos = PlanningMRPos.NewACObject(dbApp, CurrentPlanningMR);
                //supplyPos.MaterialID = prodOrderPartslist.Partslist.MaterialID;
                supplyPos.ProdOrderPartslistID = prodOrderPartslist.ProdOrderPartslistID;
                // supplyPos.StoreQuantityUOM = prodOrderPartslist.TargetQuantityUOM; // Positive = supply
                // supplyPos.ExpectedBookingDate = prodOrderPartslist.TargetDeliveryDate ?? DateTime.Now;

                //CurrentPlanningMR.PlanningMRPos_PlanningMR.Add(supplyPos);

                // Add component demands (negative quantities)
                foreach (var bomPos in prodOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                    .Where(p => p.MaterialPosTypeIndex == (int)GlobalApp.MaterialPosTypes.InwardRoot))
                {
                    var demandPos = PlanningMRPos.NewACObject(dbApp, CurrentPlanningMR);
                    //demandPos.MaterialID = bomPos.MaterialID;
                    demandPos.ProdOrderPartslistPosID = bomPos.ProdOrderPartslistPosID;
                    demandPos.StoreQuantityUOM = -bomPos.TargetQuantityUOM; // Negative = demand
                    // demandPos.ExpectedBookingDate = (prodOrderPartslist.TargetDeliveryDate ?? DateTime.Now).AddDays(-2); // Lead time

                    //CurrentPlanningMR.PlanningMRPos_PlanningMR.Add(demandPos);
                }
            }

            // Add consumption forecasts for consumption-controlled materials
            if (IncludeConsumptionForecast)
            {
                AddConsumptionForecasts(dbApp);
            }
        }

        private void AddConsumptionForecasts(DatabaseApp dbApp)
        {
            // Get materials with consumption-based MRP procedure
            var consumptionMaterials = dbApp.Material
                //.Where(m => m.MRPProcedure == (int)MRPProcedureEnum.ConsumptionBased)
                .ToList();

            foreach (var material in consumptionMaterials)
            {
                // Calculate consumption forecast based on historical data
                var historicalConsumption = CalculateHistoricalConsumption(dbApp, material);

                if (historicalConsumption > 0)
                {
                    // Create consumption forecast entries for each week in planning horizon
                    DateTime currentDate = PlanningHorizonFrom;
                    while (currentDate <= PlanningHorizonTo)
                    {
                        var consumption = PlanningMRCons.NewACObject(dbApp, CurrentPlanningMR);
                        consumption.MaterialID = material.MaterialID;
                        consumption.ConsumptionDate = currentDate;
                        consumption.EstimatedQuantityUOM = historicalConsumption / 12; // Weekly consumption
                        consumption.RequiredQuantityUOM = consumption.EstimatedQuantityUOM;

                        CurrentPlanningMR.PlanningMRCons_PlanningMR.Add(consumption);

                        // Add corresponding demand position
                        var demandPos = PlanningMRPos.NewACObject(dbApp, CurrentPlanningMR);
                        //demandPos.MaterialID = material.MaterialID;
                        // demandPos.PlanningMRConsumptionID = consumption.PlanningMRConsID;
                        demandPos.StoreQuantityUOM = -consumption.RequiredQuantityUOM;
                        // demandPos.ExpectedBookingDate = currentDate;

                        //CurrentPlanningMR.PlanningMRPos_PlanningMR.Add(demandPos);

                        currentDate = currentDate.AddDays(7); // Weekly periods
                    }
                }
            }
        }

        private double CalculateHistoricalConsumption(DatabaseApp dbApp, Material material)
        {
            // Calculate average monthly consumption based on last 6 months
            DateTime sixMonthsAgo = DateTime.Now.AddMonths(-6);

            var historicalBookings = dbApp.FacilityBooking
                .Where(b => b.OutwardMaterialID == material.MaterialID &&
                           b.InsertDate >= sixMonthsAgo)
                .Sum(b => b.OutwardTargetQuantity);

            return historicalBookings / 6; // Monthly average
        }

        private void PerformBOMExplosion(DatabaseApp dbApp, ACBackgroundWorker worker)
        {
            //// Get all uncovered demand positions
            var uncoveredDemands = CurrentPlanningMR.PlanningMRCons_PlanningMR.SelectMany(c => c.PlanningMRPos_PlanningMRCons)
                //.Where(p => p.StoreQuantityUOM < 0 && p.ReqPlanningMRProposalID == null)
                .GroupBy(p => p.PlanningMRCons.MaterialID)
                .Select(g => new { MaterialID = g.Key, TotalDemand = g.Sum(p => p.StoreQuantityUOM) })
                .Where(g => g.TotalDemand < 0)
                .ToList();

            foreach (var demand in uncoveredDemands)
            {
                var material = dbApp.Material.FirstOrDefault(m => m.MaterialID == demand.MaterialID);
                if (material == null) continue;

                // Determine if this should be produced or purchased
                var activePartslist = dbApp.Partslist
                    .Where(p => p.MaterialID == material.MaterialID && p.DeleteDate == null)
                    .OrderByDescending(p => p.PartslistVersion)
                    .FirstOrDefault();

                if (activePartslist != null && material.ProductionMaterialID == null)
                {
                    // Create production proposal
                    CreateProductionProposal(dbApp, material, activePartslist, Math.Abs(demand.TotalDemand));
                }
                else
                {
                    // Create purchase proposal
                    CreatePurchaseProposal(dbApp, material, Math.Abs(demand.TotalDemand));
                }
            }
        }

        private void CreateProductionProposal(DatabaseApp dbApp, Material material, Partslist partslist, double quantity)
        {
            // Create production order proposal
            string prodOrderNo = Root.NoManager.GetNewNo(Database, typeof(ProdOrder), ProdOrder.NoColumnName, ProdOrder.FormatNewNo, this);
            var prodOrder = ProdOrder.NewACObject(dbApp, null, prodOrderNo);

            var prodOrderPartslist = ProdOrderPartslist.NewACObject(dbApp, prodOrder);
            prodOrderPartslist.PartslistID = partslist.PartslistID;
            //prodOrderPartslist.TargetQuantityUOM = quantity;
            //prodOrderPartslist.TargetDeliveryDate = DateTime.Now.AddDays(7); // Default lead time

            prodOrder.ProdOrderPartslist_ProdOrder.Add(prodOrderPartslist);

            var proposal = PlanningMRProposal.NewACObject(dbApp, CurrentPlanningMR);
            proposal.ProdOrder = prodOrder;
            proposal.ProdOrderPartslist = prodOrderPartslist;

            CurrentPlanningMR.PlanningMRProposal_PlanningMR.Add(proposal);

            // Update demand positions to reference this proposal
            var demandPositions = CurrentPlanningMR.PlanningMRCons_PlanningMR.SelectMany(c => c.PlanningMRPos_PlanningMRCons)
                .Where(p => p.PlanningMRCons.MaterialID == material.MaterialID && p.StoreQuantityUOM < 0)
                .ToList();

            //foreach (var pos in demandPositions)
            //{
            //    pos.ReqPlanningMRProposalID = proposal.PlanningMRProposalID;
            //}

            // Explode BOM components
            //foreach (var bomPos in partslist.PartslistPos_Partslist.Where(p => p.MaterialPosTypeIndex == (int)GlobalApp.MaterialPosTypes.InwardRoot))
            //{
            //    var componentDemand = PlanningMRPos.NewACObject(dbApp, CurrentPlanningMR);
            //    componentDemand.MaterialID = bomPos.MaterialID;
            //    componentDemand.StoreQuantityUOM = -(bomPos.TargetQuantityUOM * quantity); // Negative = demand
            //    // componentDemand.ExpectedBookingDate = prodOrderPartslist.TargetDeliveryDate.Value.AddDays(-2);

            //    CurrentPlanningMR.PlanningMRPos_PlanningMR.Add(componentDemand);
            //}
        }

        private void CreatePurchaseProposal(DatabaseApp dbApp, Material material, double quantity)
        {
            // Create purchase order proposal
            string inOrderNo = Root.NoManager.GetNewNo(Database, typeof(InOrder), InOrder.NoColumnName, InOrder.FormatNewNo, this);
            var inOrder = InOrder.NewACObject(dbApp, null, inOrderNo);
            inOrder.InOrderDate = DateTime.Now;
            inOrder.TargetDeliveryDate = DateTime.Now.AddDays(14); // Default purchase lead time

            var proposal = PlanningMRProposal.NewACObject(dbApp, CurrentPlanningMR);
            proposal.InOrder = inOrder;

            CurrentPlanningMR.PlanningMRProposal_PlanningMR.Add(proposal);

            // Update demand positions to reference this proposal
            var demandPositions = CurrentPlanningMR.PlanningMRCons_PlanningMR.SelectMany(c => c.PlanningMRPos_PlanningMRCons)
                .Where(p => p.PlanningMRCons.MaterialID == material.MaterialID && p.StoreQuantityUOM < 0)
                .ToList();

            foreach (var pos in demandPositions)
            {
                //pos.ReqPlanningMRProposalID = proposal.PlanningMRProposalID;
            }
        }

        private void ConsolidateMaterialRequirements(DatabaseApp dbApp, ACBackgroundWorker worker)
        {
            // Group uncovered demands by material and consolidate into single proposals
            var materialGroups = CurrentPlanningMR.PlanningMRCons_PlanningMR.SelectMany(c => c.PlanningMRPos_PlanningMRCons)
                .Where(p => p.StoreQuantityUOM < 0)
                .GroupBy(p => new { p.PlanningMRCons.MaterialID, RequestDate = DateTime.Now /*p.ExpectedBookingDate.Date*/ })
                .Where(g => g.Sum(p => p.StoreQuantityUOM) < 0)
                .ToList();

            foreach (var group in materialGroups)
            {
                var material = dbApp.Material.FirstOrDefault(m => m.MaterialID == group.Key.MaterialID);
                if (material == null) continue;

                double totalQuantity = Math.Abs(group.Sum(p => p.StoreQuantityUOM));

                // Check if we should consolidate multiple small demands into one larger proposal
                var existingProposals = CurrentPlanningMR.PlanningMRProposal_PlanningMR
                    .Where(p => (p.ProdOrderPartslist != null && p.ProdOrderPartslist.Partslist.MaterialID == material.MaterialID) ||
                               (p.InOrder != null)) // For purchases, we'd need to check InOrderPos
                    .ToList();

                // Consolidation logic would go here
                // For now, we'll create individual proposals as done in PerformBOMExplosion
            }
        }

        private MRPResult ActivatePlanningExecution(ACBackgroundWorker worker)
        {
            MRPResult result = new MRPResult();

            try
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    var planningMR = dbApp.PlanningMR
                        .Where(p => p.PlanningMRID == CurrentPlanningMR.PlanningMRID)
                        .FirstOrDefault();

                    if (planningMR == null)
                    {
                        result.SaveMessage = new MsgWithDetails();
                        result.SaveMessage.AddDetailMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = "Planning not found" });
                        return result;
                    }

                    worker.ProgressInfo.ReportProgress("ActivatePlanning", 1, "Creating production orders...");

                    int createdOrders = 0;

                    // Activate production order proposals
                    foreach (var proposal in planningMR.PlanningMRProposal_PlanningMR.Where(p => p.ProdOrder != null))
                    {
                        var prodOrder = proposal.ProdOrder;

                        // Change state from planned to active
                        var inProgressState = dbApp.MDProdOrderState.FirstOrDefault(s => s.MDKey == "InProgress");
                        if (inProgressState != null)
                        {
                            prodOrder.MDProdOrderStateID = inProgressState.MDProdOrderStateID;

                            foreach (var partslist in prodOrder.ProdOrderPartslist_ProdOrder)
                            {
                                partslist.MDProdOrderStateID = inProgressState.MDProdOrderStateID;
                            }
                        }

                        createdOrders++;
                    }

                    // Activate purchase order proposals
                    foreach (var proposal in planningMR.PlanningMRProposal_PlanningMR.Where(p => p.InOrder != null))
                    {
                        var inOrder = proposal.InOrder;

                        // Change state from planned to active
                        var inProgressState = dbApp.MDInOrderState.FirstOrDefault(s => s.MDKey == "InProgress");
                        if (inProgressState != null)
                        {
                            inOrder.MDInOrderStateID = inProgressState.MDInOrderStateID;
                        }

                        createdOrders++;
                    }

                    worker.ProgressInfo.ReportProgress("ActivatePlanning", 8, "Saving activated orders...");

                    MsgWithDetails saveMsg = dbApp.ACSaveChanges();
                    if (saveMsg != null)
                    {
                        result.SaveMessage = saveMsg;
                    }
                    else
                    {
                        result.CreatedOrders = createdOrders;
                    }
                }
            }
            catch (Exception ex)
            {
                result.SaveMessage = new MsgWithDetails();
                result.SaveMessage.AddDetailMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = ex.Message });
            }

            return result;
        }

        private MRPResult DeletePlanningExecution()
        {
            MRPResult result = new MRPResult();

            try
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    var planningMR = dbApp.PlanningMR
                        .Where(p => p.PlanningMRID == CurrentPlanningMR.PlanningMRID)
                        .FirstOrDefault();

                    if (planningMR != null)
                    {
                        // Delete related proposals and their orders
                        foreach (var proposal in planningMR.PlanningMRProposal_PlanningMR.ToList())
                        {
                            if (proposal.ProdOrder != null)
                            {
                                proposal.ProdOrder.DeleteACObject(dbApp, false);
                            }
                            if (proposal.InOrder != null)
                            {
                                proposal.InOrder.DeleteACObject(dbApp, false);
                            }
                            proposal.DeleteACObject(dbApp, false);
                        }

                        // Delete planning positions
                        //foreach (var pos in planningMR.PlanningMRPos_PlanningMR.ToList())
                        //{
                        //    pos.DeleteACObject(dbApp, false);
                        //}

                        // Delete consumption forecasts
                        foreach (var cons in planningMR.PlanningMRCons_PlanningMR.ToList())
                        {
                            cons.DeleteACObject(dbApp, false);
                        }

                        // Delete planning itself
                        planningMR.DeleteACObject(dbApp, false);

                        result.SaveMessage = dbApp.ACSaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                result.SaveMessage = new MsgWithDetails();
                result.SaveMessage.AddDetailMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = ex.Message });
            }

            return result;
        }
        #endregion

        #region Helper Methods

        private void SetSelectedPlanningPhase(MRPPlanningPhaseEnum planningMRPhase)
        {
            SelectedPlanningPhase = PlanningPhaseList.Where(c => (short)c.Value == (short)planningMRPhase).FirstOrDefault();
        }

        private string GetLayout(string designName)
        {
            gip.core.datamodel.ACClassDesign acClassDesign = ACType.GetDesign(this, Global.ACUsages.DULayout, Global.ACKinds.DSDesignLayout, designName);
            string layoutXAML = "<vb:VBDockPanel></vb:VBDockPanel>";
            if (acClassDesign != null)
                layoutXAML = acClassDesign.XMLDesign;
            return layoutXAML;
        }

        #endregion

        #endregion
    }

    #region Helper Classes
    public class MRPResult
    {
        public MsgWithDetails SaveMessage { get; set; }
        public int CreatedProposals { get; set; }
        public int CreatedOrders { get; set; }
    }


    #endregion
}