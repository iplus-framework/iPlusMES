// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static gip.core.datamodel.Global;

namespace gip.bso.manufacturing
{
    /// <summary>
    /// Businessobject or App for managing template schedules (PlanningMR with Template=true) and generating production orders from them.
    /// A template schedule contains proposals (PlanningMRProposal) that reference production orders; from these, real production orders and batch plans are generated for a given date.
    /// To search for records, call Search(); the result is copied to the PlanningMRList property.
    /// Then call NavigateFirst() to set CurrentPlanningMR with the first record; SelectedPlanningMR holds the selection in the list.
    /// Changing the selection filters the embedded batch plan scheduler (ChildBSOBatchPlanScheduler, child BSO BSOBatchPlanScheduler_Child) via its FilterPlanningMR property.
    /// The New() method creates a new template schedule (PlanningMR with Template=true), assigns a new number and sets it as CurrentPlanningMR.
    /// Fill PlanningMRNo, PlanningName, RangeFrom, RangeTo and Comment on CurrentPlanningMR and call Save() to persist it.
    /// Delete() asks for confirmation (Question50097) and deletes the template schedule together with its generated production orders, partslists and batch plans in a background worker.
    /// TemplateClone() duplicates the selected template schedule with a new number in a background worker and selects the clone.
    /// GeneratePlan() opens the DlgGeneratePlan dialog with BatchPlanTermin preset to the current time. GeneratePlanOK() starts the generation: first outdated partslists of the referenced production orders are updated after confirmation (Question50066), then the production orders and batch plans are generated in a background worker (GenerateProdOrders). GeneratePlanCancel() closes the dialog without generating.
    /// CheckIsPartslistChanged() checks whether the partslists (BOMs) of the referenced production orders have changed since scheduling (Partslist.LastFormulaChange) and offers to update the scheduled template orders (RefreshScheduledTemplateOrders).
    /// Enabled rules: GeneratePlan/GeneratePlanOK require a selected template schedule with proposals; Delete requires CurrentPlanningMR; TemplateClone requires SelectedPlanningMR.
    /// Messages and progress are shown via the embedded batch plan scheduler child BSO and the progress dialog.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOTemplateSchedule&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioManufacturing, ConstApp.PlanningMR, Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + PlanningMR.ClassName,
        Description = @"Businessobject or App for managing template schedules (PlanningMR with Template=true) and generating production orders from them.
        A template schedule contains proposals (PlanningMRProposal) that reference production orders; from these, real production orders and batch plans are generated for a given date.
        To search for records, call Search(); the result is copied to the PlanningMRList property.
        Then call NavigateFirst() to set CurrentPlanningMR with the first record; SelectedPlanningMR holds the selection in the list.
        Changing the selection filters the embedded batch plan scheduler (ChildBSOBatchPlanScheduler, child BSO BSOBatchPlanScheduler_Child) via its FilterPlanningMR property.
        The New() method creates a new template schedule (PlanningMR with Template=true), assigns a new number and sets it as CurrentPlanningMR.
        Fill PlanningMRNo, PlanningName, RangeFrom, RangeTo and Comment on CurrentPlanningMR and call Save() to persist it.
        Delete() asks for confirmation (Question50097) and deletes the template schedule together with its generated production orders, partslists and batch plans in a background worker.
        TemplateClone() duplicates the selected template schedule with a new number in a background worker and selects the clone.
        GeneratePlan() opens the DlgGeneratePlan dialog with BatchPlanTermin preset to the current time. GeneratePlanOK() starts the generation: first outdated partslists of the referenced production orders are updated after confirmation (Question50066), then the production orders and batch plans are generated in a background worker (GenerateProdOrders). GeneratePlanCancel() closes the dialog without generating.
        CheckIsPartslistChanged() checks whether the partslists (BOMs) of the referenced production orders have changed since scheduling (Partslist.LastFormulaChange) and offers to update the scheduled template orders (RefreshScheduledTemplateOrders).
        Enabled rules: GeneratePlan/GeneratePlanOK require a selected template schedule with proposals; Delete requires CurrentPlanningMR; TemplateClone requires SelectedPlanningMR.
        Messages and progress are shown via the embedded batch plan scheduler child BSO and the progress dialog.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOTemplateSchedule&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOTemplateSchedule : ACBSOvbNav
    {
        #region const
        public const string ClassName = @"BSOTemplateSchedule";
        public const string Const_BSOBatchPlanScheduler_Child = @"BSOBatchPlanScheduler_Child";
        public const string BGWorkerMehtod_GeneratePlan = @"GeneratePlan";
        public const string BGWorkerMehtod_DeletePlan = @"DeletePlan";
        public const string BGWorkerMehtod_ClonePlan = @"ClonePlan";
        #endregion

        #region c´tors

        public BSOTemplateSchedule(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            ChildBSOBatchPlanScheduler.RefreshScheduleListOnReceivedChange = false;
            Search();
            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            if (_ProdOrderManager != null)
                ACProdOrderManager.DetachACRefFromServiceInstance(this, _ProdOrderManager);
            _ProdOrderManager = null;

            return await base.ACDeInit(deleteACClassTask);
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

        #endregion

        #region Child (Local BSOs)

        ACChildItem<BSOBatchPlanScheduler> _BSOBatchPlanScheduler_Child;
        [ACPropertyInfo(590, Description = @"Embedded child BSO BSOBatchPlanScheduler that displays and edits the batch plans of the selected template schedule. Its FilterPlanningMR is set to the currently selected template schedule.")]
        [ACChildInfo(Const_BSOBatchPlanScheduler_Child, typeof(BSOBatchPlanScheduler))]
        public ACChildItem<BSOBatchPlanScheduler> BSOBatchPlanScheduler_Child
        {
            get
            {
                if (_BSOBatchPlanScheduler_Child == null)
                    _BSOBatchPlanScheduler_Child = new ACChildItem<BSOBatchPlanScheduler>(this, Const_BSOBatchPlanScheduler_Child);
                return _BSOBatchPlanScheduler_Child;
            }
        }


        public virtual BSOBatchPlanScheduler ChildBSOBatchPlanScheduler
        {
            get
            {
                if (BSOBatchPlanScheduler_Child == null)
                    return null;
                return BSOBatchPlanScheduler_Child.Value;
            }
        }

        #endregion

        #region BSO->ACProperty
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<PlanningMR> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, PlanningMR.ClassName, Description = @"Navigation access to the PlanningMR entities (template schedules).")]
        public ACAccessNav<PlanningMR> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<PlanningMR>(PlanningMR.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the selected balancing mode.
        /// </summary>
        /// <value>The selected balancing mode.</value>
        [ACPropertySelected(9999, PlanningMR.ClassName, Description = @"The template schedule currently selected in the list. Changing it filters the embedded batch plan scheduler (FilterPlanningMR).")]
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
                    NotifiyChangedCurrentPlanningMR(AccessPrimary.Selected);
                    OnPropertyChanged("SelectedPlanningMR");
                }
            }
        }

        /// <summary>
        /// Gets or sets the current balancing mode.
        /// </summary>
        /// <value>The current balancing mode.</value>
        [ACPropertyCurrent(9999, PlanningMR.ClassName, Description = @"The template schedule currently displayed and edited. All changes must be made on this object, followed by Save().")]
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
                    NotifiyChangedCurrentPlanningMR(AccessPrimary.Current);
                    OnPropertyChanged("CurrentPlanningMR");
                }
            }
        }

        /// <summary>
        /// Gets the balancing mode list.
        /// </summary>
        /// <value>The balancing mode list.</value>
        [ACPropertyList(9999, PlanningMR.ClassName, Description = @"The list of template schedules found by the last search.")]
        public IEnumerable<PlanningMR> PlanningMRList
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.NavList;
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private DateTime _BatchPlanTermin;
        [ACPropertySelected(999, "BatchPlanTermin", "en{'Batch Plan Termin'}de{'Batch Plan Termin'}", Description = @"Target date for which the production orders and batch plans are generated by GeneratePlanOK. Preset to the current time when the DlgGeneratePlan dialog opens.")]
        public DateTime BatchPlanTermin
        {
            get
            {
                return _BatchPlanTermin;
            }
            set
            {
                if (_BatchPlanTermin != value)
                {
                    _BatchPlanTermin = value;
                    OnPropertyChanged("BatchPlanTermin");
                }
            }
        }

        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(PlanningMR.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = @"Saves the changes made on CurrentPlanningMR to the database.")]
        public async Task Save()
        {
            await OnSave();
        }

        /// <summary>
        /// Determines whether [is enabled save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand(PlanningMR.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = @"Discards unsaved changes of the current template schedule.")]
        public void UndoSave()
        {
            OnUndoSave();
        }

        /// <summary>
        /// Determines whether [is enabled undo save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled undo save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        [ACMethodInteraction(PlanningMR.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedPlanningMR", Global.ACKinds.MSMethodPrePost, Description = @"Reloads the selected template schedule from the database to pick up changes made by other users in the background. Enabled only if a template schedule is selected.")]
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

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedPlanningMR != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(PlanningMR.ClassName, Const.New, (short)MISort.New, true, "SelectedPlanningMR", Global.ACKinds.MSMethodPrePost, Description = @"Creates a new template schedule (PlanningMR with Template=true), assigns a new number and sets it as CurrentPlanningMR. Fill the header fields and call Save().")]
        public void New()
        {
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(PlanningMR), PlanningMR.NoColumnName, PlanningMR.FormatNewNo, this);
            PlanningMR planningMR = PlanningMR.NewACObject(DatabaseApp, null, secondaryKey);
            planningMR.Template = true;
            AccessPrimary.NavList.Add(planningMR);
            DatabaseApp.PlanningMR.Add(planningMR);
            CurrentPlanningMR = planningMR;
            OnPropertyChanged("PlanningMRList");
        }

        /// <summary>
        /// Determines whether [is enabled new].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNew()
        {
            return true;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        [ACMethodInteraction(PlanningMR.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentPlanningMR", Global.ACKinds.MSMethodPrePost, Description = @"Deletes the current template schedule after confirmation (Question50097), together with its generated production orders, partslists and batch plans. Runs in a background worker; enabled only if a template schedule is selected.")]
        public async void Delete()
        {
            if (AccessPrimary == null)
                return;
            // Question50097

            Global.MsgResult result = await Messages.QuestionAsync(this, "Question50097", Global.MsgResult.Yes, false, SelectedPlanningMR.PlanningMRNo);
            if (result == Global.MsgResult.Yes)
            {
                BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DeletePlan);
                await ShowDialogAsync(this, DesignNameProgressBar);
            }
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentPlanningMR != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(PlanningMR.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = @"Searches template schedules and copies the result to PlanningMRList.")]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("PlanningMRList");
        }

        public void NotifiyChangedCurrentPlanningMR(PlanningMR planningMR)
        {
            if (ChildBSOBatchPlanScheduler != null)
                ChildBSOBatchPlanScheduler.FilterPlanningMR = planningMR ?? new PlanningMR();
        }


        /// <summary>
        /// Method GeneratePlan
        /// </summary>
        [ACMethodInfo("GeneratePlan", "en{'Add to schedule'}de{'Übernehme in Terminplan'}", 100, Description = @"Opens the DlgGeneratePlan dialog with BatchPlanTermin preset to the current time, to generate production orders and batch plans from the selected template schedule. Enabled only if the selected template schedule has proposals.")]
        public async Task GeneratePlan()
        {
            if (!IsEnabledGeneratePlan())
                return;
            BatchPlanTermin = DateTime.Now;
            await ShowDialogAsync(this, "DlgGeneratePlan");
        }

        public bool IsEnabledGeneratePlan()
        {
            return SelectedPlanningMR != null
                && SelectedPlanningMR.PlanningMRProposal_PlanningMR.Any();
        }

        /// <summary>
        /// Method GeneratePlan
        /// </summary>
        [ACMethodInfo("GeneratePlan", Const.Ok, 100, Description = @"Starts the generation from the DlgGeneratePlan dialog: outdated partslists of the referenced production orders are updated after confirmation (Question50066), then the production orders and batch plans are generated in a background worker for BatchPlanTermin. Closes the dialog.")]
        public async void GeneratePlanOK()
        {
            if (!IsEnabledGeneratePlanOk())
                return;
            CloseTopDialog();

            bool success = true;
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                List<ProdOrderPartslist> prodOrderPartslistsChanged =
                databaseApp
                .PlanningMR
                .Where(c => c.PlanningMRID == SelectedPlanningMR.PlanningMRID)
                .SelectMany(c => c.PlanningMRProposal_PlanningMR)
                .Where(c => c.ProdOrder != null)
                .Select(c => c.ProdOrder)
                .GroupBy(c => c.ProdOrderID)
                .Select(c => c.FirstOrDefault())
                .SelectMany(c => c.ProdOrderPartslist_ProdOrder)
                .Where(c => c.Partslist.LastFormulaChange > c.LastFormulaChange)
                .ToList();

                if (prodOrderPartslistsChanged.Any())
                {
                    bool isUpdate = await UpdateChangedPartslist(databaseApp, prodOrderPartslistsChanged);
                    if (isUpdate)
                    {
                        foreach (ProdOrderPartslist pl in prodOrderPartslistsChanged)
                            pl.LastFormulaChange = DateTime.Now;
                        MsgWithDetails saveMsg = databaseApp.ACSaveChanges();
                        if (saveMsg != null && !saveMsg.IsSucceded())
                        {
                            success = false;
                            Messages.LogMessageMsg(saveMsg);
                            await Messages.MsgAsync(saveMsg);
                        }
                    }
                }
            }

            if (success)
            {
                BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_GeneratePlan);
                await ShowDialogAsync(this, DesignNameProgressBar);
            }
        }

        public bool IsEnabledGeneratePlanOk()
        {
            return
                !BackgroundWorker.IsBusy
                && SelectedPlanningMR != null
                && SelectedPlanningMR.PlanningMRProposal_PlanningMR.Any();
        }

        /// <summary>
        /// Method GeneratePlan
        /// </summary>
        [ACMethodInfo("GeneratePlan", Const.Cancel, 100, Description = @"Closes the DlgGeneratePlan dialog without generating production orders.")]
        public void GeneratePlanCancel()
        {
            CloseTopDialog();
        }



        [ACMethodInfo("CheckIsPartslistChanged", "en{'Update template schedule orders'}de{'Aktualisiere Vorlageplan Produktionsaufträge'}", 101, Description = @"Checks whether the partslists (BOMs) of the production orders referenced by the selected template schedule have changed since scheduling and offers to update the scheduled template orders (RefreshScheduledTemplateOrders) after confirmation (Question50066). Enabled only if such changes exist.")]
        public async Task CheckIsPartslistChanged()
        {
            if (!IsEnabledCheckIsPartslistChanged())
                return;
            List<ProdOrderPartslist> prodOrderPartslistsChanged =
                SelectedPlanningMR
                .PlanningMRProposal_PlanningMR
                .Select(c => c.ProdOrderPartslist)
                .Where(c => c.Partslist.LastFormulaChange > c.LastFormulaChange)
                .Distinct()
                .ToList();

            if (prodOrderPartslistsChanged.Any())
            {
                await UpdateChangedPartslist(DatabaseApp, prodOrderPartslistsChanged);
            }
        }

        public bool IsEnabledCheckIsPartslistChanged()
        {
            return
                SelectedPlanningMR != null
                && SelectedPlanningMR
                .PlanningMRProposal_PlanningMR
                .Select(c => c.ProdOrderPartslist)
                .Where(c => c.Partslist.LastFormulaChange > c.LastFormulaChange)
                .Any();
        }

        public async Task<bool> UpdateChangedPartslist(DatabaseApp databaseApp, List<ProdOrderPartslist> prodOrderPartslistsChanged)
        {
            bool isUpdate = false;
            string changedPlartslistNo = string.Join(",", prodOrderPartslistsChanged.Select(c => c.Partslist.PartslistNo).Distinct().OrderBy(c => c));
            MsgResult msgResult = await Root.Messages.QuestionAsync(this, "Question50066", MsgResult.Yes, false, changedPlartslistNo);
            if (msgResult == MsgResult.Yes)
            {
                isUpdate = true;
                foreach (ProdOrderPartslist prodOrderPartslistChanged in prodOrderPartslistsChanged)
                {
                    ProdOrderManager.RefreshScheduledTemplateOrders(databaseApp, prodOrderPartslistChanged.Partslist, prodOrderPartslistChanged);
                }
            }
            return isUpdate;
        }


        [ACMethodInteraction("TemplateClone", "en{'Clone'}de{'Duplizieren'}", (short)MISort.New, true, nameof(SelectedPlanningMR), Global.ACKinds.MSMethodPrePost, Description = @"Duplicates the selected template schedule with a new number in a background worker and selects the clone. Enabled only if a template schedule is selected.")]
        public async Task TemplateClone()
        {
            if (!IsEnabledTemplateClone())
                return;
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_ClonePlan);
            await ShowDialogAsync(this, DesignNameProgressBar);
        }

        public bool IsEnabledTemplateClone()
        {
            return SelectedPlanningMR != null;
        }

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

            switch (command)
            {
                case BGWorkerMehtod_GeneratePlan:
                    using(DatabaseApp databaseApp = new DatabaseApp())
                    {
                        e.Result = GenerateProdOrders(databaseApp, BatchPlanTermin);
                    }
                    break;
                case BGWorkerMehtod_ClonePlan:
                    e.Result = ClonePlan();
                    break;
                case BGWorkerMehtod_DeletePlan:
                    e.Result = DeletePlan(SelectedPlanningMR.PlanningMRNo);
                    break;
            }
        }


        public override void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            base.BgWorkerCompleted(sender, e);
            CloseWindow(this, DesignNameProgressBar);
            ChildBSOBatchPlanScheduler.ClearMessages();
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = worker.EventArgs.Argument.ToString();

            if (e.Cancelled)
            {
                ChildBSOBatchPlanScheduler.SendMessage(new Msg() { MessageLevel = eMsgLevel.Info, Message = string.Format(@"Operation {0} canceled by user!", command) });
            }
            if (e.Error != null)
            {
                ChildBSOBatchPlanScheduler.SendMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = string.Format(@"Error by doing {0}! Message:{1}", command, e.Error.Message) });
            }
            else if (e.Result != null)
            {
                GenerateProdOrdersResult result = e.Result as GenerateProdOrdersResult;
                if (result != null)
                {
                    if (result.SaveMessage != null)
                    {
                        Messages.MsgAsync(result.SaveMessage);
                    }
                    else
                    {
                        if (command == BGWorkerMehtod_DeletePlan)
                        {
                            Search();
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(result.PlanningMRNo))
                            {
                                Messages.InfoAsync(this, "Info50090", false, result.PlanningMRNo);
                                DatabaseApp.PlanningMR.AutoMergeOption(DatabaseApp);
                                PlanningMR planningMR = DatabaseApp.PlanningMR.FirstOrDefault(c => c.PlanningMRNo == result.PlanningMRNo);
                                if (planningMR != null)
                                {
                                    AccessPrimary.NavList.Insert(0, planningMR);
                                    SelectedPlanningMR = planningMR;
                                    CurrentPlanningMR = planningMR;
                                }
                            }
                            else
                            {
                                Messages.InfoAsync(this, "Info50074", false, string.Join(",", result.GeneratedProdOrders.Select(c => c.ProgramNo)));
                                if (result.MDSchedulingGroupIDs.Any())
                                {
                                    foreach (Guid mdSchedulingGroupID in result.MDSchedulingGroupIDs)
                                    {
                                        ChildBSOBatchPlanScheduler.RefreshServerState(mdSchedulingGroupID);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        #region BackgroundWorker -> Mehtods

        private GenerateProdOrdersResult ClonePlan()
        {
            GenerateProdOrdersResult result = null;
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(PlanningMR), PlanningMR.NoColumnName, PlanningMR.FormatNewNo, this);
                PlanningMR planningMR = PlanningMR.NewACObject(DatabaseApp, null, secondaryKey);
                planningMR.Template = true;
                databaseApp.PlanningMR.Add(planningMR);
                result = GenerateProdOrders(databaseApp, null, planningMR);
            }
            return result;
        }

        public virtual GenerateProdOrdersResult GenerateProdOrders(DatabaseApp databaseApp, DateTime? batchPlanTermin, PlanningMR planningMR = null)
        {
            Guid[] filterProdOrderBatchPlanIds = new Guid[] { };
            if (ChildBSOBatchPlanScheduler.ProdOrderBatchPlanList.Any(c => c.IsSelected))
            {
                // Filter is based on ProdOrderPartslist - ProdOrderBatchPlan is selected
                // is ok select only one batch plan per ProdOrderPartslist
                filterProdOrderBatchPlanIds =
                    ChildBSOBatchPlanScheduler
                    .ProdOrderBatchPlanList
                    .Where(c => c.IsSelected)
                    .Select(c => c.ProdOrderBatchPlanID)
                    .Distinct()
                    .ToArray();
            }

            Guid[] prodOrderIDs = SelectedPlanningMR.PlanningMRProposal_PlanningMR.Where(c => c.ProdOrder != null).Select(c => c.ProdOrderID ?? Guid.Empty).Distinct().ToArray();

            return ProdOrderManager.GenerateProdOrderFromTemplate(databaseApp, prodOrderIDs, batchPlanTermin, filterProdOrderBatchPlanIds, planningMR);
        }

        private GenerateProdOrdersResult DeletePlan(string planningMRNo)
        {
            GenerateProdOrdersResult result = new GenerateProdOrdersResult();
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                PlanningMR planningMR = databaseApp.PlanningMR.Where(c => c.PlanningMRNo == planningMRNo).FirstOrDefault();
                if (planningMR != null)
                {
                    ProdOrder[] prodOrders =
                        planningMR
                        .PlanningMRProposal_PlanningMR
                        .Where(c => c.ProdOrder != null)
                        .Select(c => c.ProdOrder)
                        .GroupBy(c => c.ProdOrderID)
                        .Select(c => c.FirstOrDefault())
                        .ToArray();
                    bool success = true;

                    foreach (ProdOrder prodOrder in prodOrders)
                    {
                        ProdOrderPartslist[] pls = prodOrder.ProdOrderPartslist_ProdOrder.ToArray();
                        foreach (ProdOrderPartslist pl in pls)
                        {
                            Msg plRemoveMsg = ProdOrderManager.PartslistRemove(databaseApp, prodOrder, pl);
                            if (plRemoveMsg != null && !plRemoveMsg.IsSucceded())
                            {
                                success = false;
                                result.SaveMessage = new MsgWithDetails() { };
                                result.SaveMessage.AddDetailMessage(plRemoveMsg);
                                break;
                            }
                        }

                        MsgWithDetails poRemoveMsg = prodOrder.DeleteACObject(databaseApp, false);
                        if (poRemoveMsg != null && !poRemoveMsg.IsSucceded())
                        {
                            result.SaveMessage = poRemoveMsg;
                            success = false;
                        }
                    }

                    if (success)
                    {
                        MsgWithDetails mrRemoveMsg = planningMR.DeleteACObject(databaseApp, false);
                        if (mrRemoveMsg != null && !mrRemoveMsg.IsSucceded())
                        {
                            success = false;
                            result.SaveMessage = mrRemoveMsg;
                        }
                        if (success)
                        {
                            result.SaveMessage = databaseApp.ACSaveChanges();
                        }
                    }
                }
            }

            return result;
        }

        #endregion

        #endregion

    }
}
