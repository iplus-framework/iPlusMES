﻿using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static gip.core.datamodel.Global;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, ConstApp.PlanningMR, Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + PlanningMR.ClassName)]
    public class BSOTemplateSchedule : ACBSOvbNav
    {
        #region const
        public const string ClassName = @"BSOTemplateSchedule";
        public const string Const_BSOBatchPlanScheduler_Child = @"BSOBatchPlanScheduler_Child";
        public const string BGWorkerMehtod_GeneratePlan = @"GeneratePlan";
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
            ChildBSOBatchPlanScheduler.RefreshBatchListByRecieveChange = false;
            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_ProdOrderManager != null)
                ACProdOrderManager.DetachACRefFromServiceInstance(this, _ProdOrderManager);
            _ProdOrderManager = null;

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

        #endregion

        #region Child (Local BSOs)

        ACChildItem<BSOBatchPlanScheduler> _BSOBatchPlanScheduler_Child;
        [ACPropertyInfo(590)]
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
        [ACPropertyAccessPrimary(9999, PlanningMR.ClassName)]
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
                    NotifiyChangedCurrentPlanningMR(AccessPrimary.Selected);
                    OnPropertyChanged("SelectedPlanningMR");
                }
            }
        }

        /// <summary>
        /// Gets or sets the current balancing mode.
        /// </summary>
        /// <value>The current balancing mode.</value>
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
                    NotifiyChangedCurrentPlanningMR(AccessPrimary.Current);
                    OnPropertyChanged("CurrentPlanningMR");
                }
            }
        }

        /// <summary>
        /// Gets the balancing mode list.
        /// </summary>
        /// <value>The balancing mode list.</value>
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

        /// <summary>
        /// Source Property: 
        /// </summary>
        private DateTime _BatchPlanTermin;
        [ACPropertySelected(999, "BatchPlanTermin", "en{'Batch Plan Termin'}de{'Batch Plan Termin'}")]
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
        [ACMethodCommand(PlanningMR.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
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
        [ACMethodCommand(PlanningMR.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction(PlanningMR.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedPlanningMR", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(PlanningMR), PlanningMR.NoColumnName, PlanningMR.FormatNewNo, this);
            PlanningMR planningMR = PlanningMR.NewACObject(DatabaseApp, null, secondaryKey);
            planningMR.Template = true;
            AccessPrimary.NavList.Add(planningMR);
            DatabaseApp.PlanningMR.AddObject(planningMR);
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
        [ACMethodInteraction(PlanningMR.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentPlanningMR", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (AccessPrimary == null)
                return;
            Msg msg = CurrentPlanningMR.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }


            AccessPrimary.NavList.Remove(CurrentPlanningMR);
            SelectedPlanningMR = AccessPrimary.NavList.FirstOrDefault();
            Load();
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
        [ACMethodCommand(PlanningMR.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
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
        [ACMethodInfo("GeneratePlan", "en{'Add to schedule'}de{'Übernehme in Terminplan'}", 100)]
        public void GeneratePlan()
        {
            if (!IsEnabledGeneratePlan())
                return;
            BatchPlanTermin = DateTime.Now;
            ShowDialog(this, "DlgGeneratePlan");
        }

        public bool IsEnabledGeneratePlan()
        {
            return SelectedPlanningMR != null
                && SelectedPlanningMR.PlanningMRProposal_PlanningMR.Any();
        }

        /// <summary>
        /// Method GeneratePlan
        /// </summary>
        [ACMethodInfo("GeneratePlan", "en{'Ok'}de{'Ok'}", 100)]
        public void GeneratePlanOK()
        {
            if (!IsEnabledGeneratePlanOk())
                return;
            CloseTopDialog();

            List<ProdOrderPartslist> prodOrderPartslistsChanged =
                SelectedPlanningMR
                .PlanningMRProposal_PlanningMR
                .Where(c => c.ProdOrder != null)
                .Select(c => c.ProdOrder).Distinct()
                .SelectMany(c => c.ProdOrderPartslist_ProdOrder)
                .Where(c => c.Partslist.LastFormulaChange > c.LastFormulaChange)
                .ToList();

            if (prodOrderPartslistsChanged.Any())
            {
                bool isUpdate = UpdateChangedPartslist(prodOrderPartslistsChanged);
                if (isUpdate)
                {
                    foreach (ProdOrderPartslist pl in prodOrderPartslistsChanged)
                        pl.LastFormulaChange = DateTime.Now;
                    ACSaveChanges();
                }
            }

            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_GeneratePlan);
            ShowDialog(this, DesignNameProgressBar);
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
        [ACMethodInfo("GeneratePlan", "en{'Cancel'}de{'Abbrechen'}", 100)]
        public void GeneratePlanCancel()
        {
            CloseTopDialog();
        }

        private void GenerateProdOrders()
        {
            new List<ProdOrder>();
            List<Guid> filterProdOrderBatchPlanIds = new List<Guid>();
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
                    .ToList();
            }

            List<ProdOrder> prodOrders = SelectedPlanningMR.PlanningMRProposal_PlanningMR.Where(c => c.ProdOrder != null).Select(c => c.ProdOrder).Distinct().ToList();
            if (prodOrders != null && prodOrders.Any())
                GenerateProdOrders(prodOrders, filterProdOrderBatchPlanIds.ToArray());
        }

        private void GenerateProdOrders(List<ProdOrder> prodOrders, Guid[] filterProdOrderBatchPlanIds)
        {
            List<ProdOrder> generated = new List<ProdOrder>();
            try
            {
                List<SchedulingMaxBPOrder> maxSchedulerOrders = null;
                using (DatabaseApp freshDb = new DatabaseApp())
                {
                    maxSchedulerOrders = ProdOrderManager.GetMaxScheduledOrder(DatabaseApp, null);
                }

                List<Guid> mdSchedulingGroupIDs = new List<Guid>();
                foreach (var sourceProdOrder in prodOrders)
                {

                    Guid[] prodOrderMdSchedulingGroupIDs =
                        sourceProdOrder
                        .ProdOrderPartslist_ProdOrder
                        .SelectMany(c => c.ProdOrderBatchPlan_ProdOrderPartslist)
                        .SelectMany(c => c.VBiACClassWF.MDSchedulingGroupWF_VBiACClassWF)
                        .Select(c => c.MDSchedulingGroupID)
                        .Distinct()
                        .ToArray();
                    foreach (Guid prodOrderMdSchedulingGroupID in prodOrderMdSchedulingGroupIDs)
                    {
                        if (!mdSchedulingGroupIDs.Contains(prodOrderMdSchedulingGroupID))
                            mdSchedulingGroupIDs.Add(prodOrderMdSchedulingGroupID);
                    }

                    bool generateProdorder = !filterProdOrderBatchPlanIds.Any();
                    if (!generateProdorder)
                    {
                        generateProdorder =
                            sourceProdOrder
                            .ProdOrderPartslist_ProdOrder
                            .SelectMany(c => c.ProdOrderBatchPlan_ProdOrderPartslist)
                            .Any(c => filterProdOrderBatchPlanIds.Contains(c.ProdOrderBatchPlanID));
                    }
                    if (generateProdorder)
                    {
                        ProdOrder targetProdOrder = ProdOrderManager.CloneProdOrder(DatabaseApp, sourceProdOrder, null, BatchPlanTermin, filterProdOrderBatchPlanIds, maxSchedulerOrders);
                        generated.Add(targetProdOrder);
                    }
                }

                MsgWithDetails msgWithDetails = DatabaseApp.ACSaveChanges();
                if (msgWithDetails == null)
                {
                    string[] generatedNos = generated.Select(c => c.ProgramNo).OrderBy(c => c).ToArray();
                    Root.Messages.Info(this, "Info50074", false, string.Join(",", generatedNos));
                }
                else
                    Root.Messages.Msg(msgWithDetails);

                foreach (Guid mdSchedulingGroupID in mdSchedulingGroupIDs)
                {
                    ChildBSOBatchPlanScheduler.RefreshServerState(mdSchedulingGroupID);
                }
            }
            catch (Exception ec)
            {
                Msg msg = new Msg() { MessageLevel = eMsgLevel.Error, Message = ec.Message };
                Root.Messages.Msg(msg);
            }
        }

        [ACMethodInfo("CheckIsPartslistChanged", "en{'Update template schedule orders'}de{'Aktualisiere Vorlageplan Produktionsaufträge'}", 101)]
        public void CheckIsPartslistChanged()
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
                UpdateChangedPartslist(prodOrderPartslistsChanged);
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

        public bool UpdateChangedPartslist(List<ProdOrderPartslist> prodOrderPartslistsChanged)
        {
            bool isUpdate = false;
            string changedPlartslistNo = string.Join(",", prodOrderPartslistsChanged.Select(c => c.Partslist.PartslistNo).Distinct().OrderBy(c => c));
            MsgResult msgResult = Root.Messages.Question(this, "Question50066", MsgResult.Yes, false, changedPlartslistNo);
            if (msgResult == MsgResult.Yes)
            {
                isUpdate = true;
                foreach (ProdOrderPartslist prodOrderPartslistChanged in prodOrderPartslistsChanged)
                {
                    ProdOrderManager.RefreshScheduledTemplateOrders(DatabaseApp, prodOrderPartslistChanged.Partslist, prodOrderPartslistChanged);
                }
            }
            return isUpdate;
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
                    GenerateProdOrders();
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
            else
            {
                // Root.Messages.Info(this, "Info50074");
            }
        }

        #endregion

    }
}