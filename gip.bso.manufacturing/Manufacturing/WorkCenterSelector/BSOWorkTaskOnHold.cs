// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.processapplication;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gip.bso.manufacturing
{
    /// <summary>
    /// Child business object of the BSOWorkCenterSelector for managing work tasks that are currently on hold (waiting loop).
    /// It is activated by the BSOWorkCenterSelector together with the selected work center and reads the held work tasks
    /// from the PAFWorkTaskScan component of the process module with RefreshWorkTasks().
    /// The result is copied to the WorkTaskOnHoldList property; the highlighted entry is SelectedWorkTaskOnHold.
    /// The list can be sorted by assigning one of the WorkTaskSortItems (WorkTaskOnHoldSortEnum) to SelectedWorkTaskSort.
    /// StartWorkTask() occupies/releases the process module for the selected work task so that it can be continued
    /// (enabled when the selected task is not yet marked for release). CompleteWorkTask() completes the release of the
    /// selected work task (enabled when the selected task is marked for release). Both methods delegate to the
    /// OccupyReleaseProcessModule method of the PAFWorkTaskScan component and refresh the list afterwards.
    /// ShowComponents() opens the components dialog of the batch plan (ProdOrderBatchPlan) of the selected work task.
    /// RefreshWorkTasks() re-reads the work tasks from the process module.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOWorkTaskOnHold&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Tasks on hold'}de{'Warteschleife'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, SortIndex = 500,
                 Description = @"Child business object of the BSOWorkCenterSelector for managing work tasks that are currently on hold (waiting loop).
                                 It is activated by the BSOWorkCenterSelector together with the selected work center and reads the held work tasks
                                 from the PAFWorkTaskScan component of the process module with RefreshWorkTasks().
                                 The result is copied to the WorkTaskOnHoldList property; the highlighted entry is SelectedWorkTaskOnHold.
                                 The list can be sorted by assigning one of the WorkTaskSortItems (WorkTaskOnHoldSortEnum) to SelectedWorkTaskSort.
                                 StartWorkTask() occupies/releases the process module for the selected work task so that it can be continued
                                 (enabled when the selected task is not yet marked for release). CompleteWorkTask() completes the release of the
                                 selected work task (enabled when the selected task is marked for release). Both methods delegate to the
                                 OccupyReleaseProcessModule method of the PAFWorkTaskScan component and refresh the list afterwards.
                                 ShowComponents() opens the components dialog of the batch plan (ProdOrderBatchPlan) of the selected work task.
                                 RefreshWorkTasks() re-reads the work tasks from the process module.
                                 Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOWorkTaskOnHold&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
                                 ")]
    public class BSOWorkTaskOnHold : BSOWorkCenterChild
    {
        #region c'tors

        public BSOWorkTaskOnHold(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            return base.ACInit(startChildMode);
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            return await base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties

        private ACRef<IACComponent> _PAFWorkTaskScan;

        private WorkTaskOnHoldItem _SelectedWorkTaskOnHold;
        /// <summary>
        /// Gets or sets the currently selected work task on hold.
        /// This property represents the entry highlighted in the tasks list.
        /// StartWorkTask(), CompleteWorkTask() and ShowComponents() operate on this entry.
        /// </summary>
        [ACPropertySelected(700, "WorkTask", "en{'Work task'}de{'Arbeitsaufgabe'}",
                            Description = @"Gets or sets the currently selected work task on hold.
                                            This property represents the entry highlighted in the tasks list.
                                            StartWorkTask(), CompleteWorkTask() and ShowComponents() operate on this entry.")]
        public WorkTaskOnHoldItem SelectedWorkTaskOnHold
        {
            get => _SelectedWorkTaskOnHold;
            set
            {
                _SelectedWorkTaskOnHold = value;
                OnPropertyChanged();
            }
        }

        private List<WorkTaskOnHoldItem> _WorkTaskHoldList;
        /// <summary>
        /// Gets or sets the list of work tasks that are currently on hold.
        /// The list is filled by RefreshWorkTasks() from the order infos of the PAFWorkTaskScan component
        /// and is sorted according to SelectedWorkTaskSort.
        /// </summary>
        [ACPropertyList(700, "WorkTask", "en{'Work tasks on hold'}de{'Arbeitsaufgaben in Warteschleife'}",
                        Description = @"Gets or sets the list of work tasks that are currently on hold.
                                        The list is filled by RefreshWorkTasks() from the order infos of the PAFWorkTaskScan component
                                        and is sorted according to SelectedWorkTaskSort.")]
        public List<WorkTaskOnHoldItem> WorkTaskOnHoldList
        {
            get => _WorkTaskHoldList;
            set
            {
                _WorkTaskHoldList = value;
                OnPropertyChanged();
            }
        }

        private ACValueItem _SelectedWorkTaskSort;
        /// <summary>
        /// Gets or sets the selected sort order for the work task list.
        /// Setting this property re-sorts WorkTaskOnHoldList according to the selected
        /// WorkTaskOnHoldSortEnum value (BatchSeq, BatchSeqDesc, WFStartDate, WFStartDateDesc).
        /// </summary>
        [ACPropertySelected(701, "WorkTaskSort", "en{'Sort'}de{'Sortierung'}",
                            Description = @"Gets or sets the selected sort order for the work task list.
                                            Setting this property re-sorts WorkTaskOnHoldList according to the selected
                                            WorkTaskOnHoldSortEnum value (BatchSeq, BatchSeqDesc, WFStartDate, WFStartDateDesc).")]
        public ACValueItem SelectedWorkTaskSort
        {
            get => _SelectedWorkTaskSort;
            set
            {
                if (WorkTaskOnHoldList == null)
                    return;

                _SelectedWorkTaskSort = value;
                OnPropertyChanged();
                SortWorkTasks();
            }
        }

        private ACValueItemList _WorkTaskSortItems;
        /// <summary>
        /// Gets the list of available sort orders for the work task list.
        /// The list is built from the values of the WorkTaskOnHoldSortEnum and can be
        /// assigned to SelectedWorkTaskSort to sort WorkTaskOnHoldList.
        /// </summary>
        [ACPropertyList(701, "WorkTaskSort", "en{'Sort'}de{'Sortierung'}",
                        Description = @"Gets the list of available sort orders for the work task list.
                                        The list is built from the values of the WorkTaskOnHoldSortEnum and can be
                                        assigned to SelectedWorkTaskSort to sort WorkTaskOnHoldList.")]
        public ACValueItemList WorkTaskSortItems
        {
            get
            {
                if (_WorkTaskSortItems != null)
                    return _WorkTaskSortItems;
                var acClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(WorkTaskOnHoldSortEnum));
                if (acClass != null)
                    _WorkTaskSortItems = acClass.ACValueListForEnum;
                return _WorkTaskSortItems;
            }
        }
        #endregion

        #region Methods

        public override void Activate(ACComponent selectedProcessModule)
        {
            base.Activate(selectedProcessModule);

            if (selectedProcessModule == null)
                return;

            var childComponents = selectedProcessModule.ACComponentChildsOnServer;
            var pafWorkTask = childComponents.FirstOrDefault(c => typeof(PAFWorkTaskScanBase).IsAssignableFrom(c.ComponentClass.ObjectType));
            if (pafWorkTask == null)
            {
                Messages.ErrorAsync(this, "The PAFWorkTaskScan can not be found!", true, null);
            }

            _PAFWorkTaskScan = new ACRef<IACComponent>(pafWorkTask, this);

            RefreshWorkTasks();
        }

        public override void DeActivate()
        {
            base.DeActivate();

            if (_PAFWorkTaskScan != null)
            {
                _PAFWorkTaskScan.Detach();
                _PAFWorkTaskScan = null;
            }
        }

        [ACMethodInfo("", "en{'Start work task'}de{'Arbeitsaufgabe starten'}", 710, true,
                      Description = @"Starts the selected work task on hold by occupying/releasing the process module for it
                                      via the OccupyReleaseProcessModule method of the PAFWorkTaskScan component and refreshes the list.
                                      Enabled when a work task is selected that is not yet marked for release.")]
        public void StartWorkTask()
        {
            if (_PAFWorkTaskScan == null || SelectedWorkTaskOnHold == null)
                return;

            IACComponent paf = _PAFWorkTaskScan.ValueT;
            Msg result = paf.ExecuteMethod(nameof(PAFWorkTaskScanBase.OccupyReleaseProcessModule), SelectedWorkTaskOnHold.WFACUrl, SelectedWorkTaskOnHold.ForRelease) as Msg;
            if (result != null && result.MessageLevel > eMsgLevel.Info)
            {
                Messages.MsgAsync(result);
            }

            RefreshWorkTasks();
        }

        public bool IsEnabledStartWorkTask()
        {
            return SelectedWorkTaskOnHold != null && !SelectedWorkTaskOnHold.ForRelease;
        }

        [ACMethodInfo("", "en{'Complete work task'}de{'Arbeitsaufgabe abschließen'}", 711, true,
                      Description = @"Completes the selected work task on hold by occupying/releasing the process module for it
                                      via the OccupyReleaseProcessModule method of the PAFWorkTaskScan component and refreshes the list.
                                      Enabled when a work task is selected that is marked for release.")]
        public void CompleteWorkTask()
        {
            if (_PAFWorkTaskScan == null)
                return;

            IACComponent paf = _PAFWorkTaskScan.ValueT;
            Msg result = paf.ExecuteMethod(nameof(PAFWorkTaskScanBase.OccupyReleaseProcessModule), SelectedWorkTaskOnHold.WFACUrl, SelectedWorkTaskOnHold.ForRelease) as Msg;
            if (result != null && result.MessageLevel > eMsgLevel.Info)
            {
                Messages.MsgAsync(result);
            }

            RefreshWorkTasks();
        }

        public bool IsEnabledCompleteWorkTask()
        {
            return SelectedWorkTaskOnHold != null && SelectedWorkTaskOnHold.ForRelease;
        }

        [ACMethodInfo("", "en{'Refresh work tasks'}de{'Arbeitsaufgaben auffrischen'}", 711, true,
                      Description = @"Re-reads the work tasks on hold from the GetOrderInfos method of the PAFWorkTaskScan component,
                                      rebuilds WorkTaskOnHoldList (order number, partslist, sequence, start date, intermediate and final material)
                                      and applies the selected sort order.")]
        public void RefreshWorkTasks()
        {
            if (_PAFWorkTaskScan == null)
                return;

            IACComponent pafWorkTask = _PAFWorkTaskScan.ValueT;

            WorkTaskScanResult prodOrderWFInfoList = pafWorkTask.ExecuteMethod(nameof(PAFWorkTaskScanBase.GetOrderInfos)) as WorkTaskScanResult;

            List<WorkTaskOnHoldItem> result = new List<WorkTaskOnHoldItem>();

            if (prodOrderWFInfoList != null && prodOrderWFInfoList.OrderInfos != null && prodOrderWFInfoList.OrderInfos.Any())
            {
                Guid[] temp = prodOrderWFInfoList.OrderInfos.Select(t => t.POPPosId).ToArray();

                using (DatabaseApp dbApp = new DatabaseApp())
                {


                    var query = dbApp.ProdOrderPartslistPos
                                  .Include(i => i.ProdOrderPartslist)
                                  .Include(i => i.ProdOrderPartslist.ProdOrder)
                                  .Include(i => i.ProdOrderPartslist.Partslist)
                                  .Where(c => temp.Any(x => x == c.ProdOrderPartslistPosID))
                                  .OrderBy(c => c.ProdOrderPartslist.ProdOrder.ProgramNo)
                                  .ThenBy(c => c.ProdOrderPartslist.Partslist.PartslistNo)
                                  .ThenBy(c => c.Sequence)
                                  .ToArray()
                                  .Select(x => new Tuple<ProdOrderPartslistPos, PAProdOrderPartslistWFInfo>
                                  (
                                        x,
                                        prodOrderWFInfoList.OrderInfos.FirstOrDefault(c => x.ProdOrderPartslistPosID == c.POPPosId))
                                  )
                                  .Select(x => new
                                  {
                                      Pos = x.Item1,
                                      PlWfInfo = x.Item2
                                  })
                                  .ToList();

                    foreach (var item in query)
                    {
                        WorkTaskOnHoldItem resultItem = new WorkTaskOnHoldItem();
                        resultItem.POProgramNo = item.Pos.ProdOrderPartslist.ProdOrder.ProgramNo;
                        resultItem.StartDate = item.PlWfInfo.WFMethodStartDate;
                        resultItem.Sequence = item.Pos.Sequence.ToString();
                        resultItem.PartslistNo = item.Pos.ProdOrderPartslist.Partslist.PartslistNo;
                        resultItem.PartslistName = item.Pos.ProdOrderPartslist.Partslist.PartslistName;
                        resultItem.IntermediateMaterial = item.Pos.MaterialName;
                        resultItem.WFACUrl = item.PlWfInfo.ACUrlWF;
                        if(item.Pos.ProdOrderBatch != null)
                        {
                            resultItem.ProdOrderBatchPlanID = item.Pos.ProdOrderBatch.ProdOrderBatchPlanID;
                        }
                        resultItem.ForRelease = item.PlWfInfo.ForRelease;

                        ProdOrderPartslist finalPl = item.Pos.ProdOrderPartslist.ProdOrder.ProdOrderPartslist_ProdOrder.OrderByDescending(c => c.Sequence).FirstOrDefault();
                        if (finalPl != null)
                        {
                            resultItem.FinalMaterialNo = finalPl.Partslist.Material.MaterialNo;
                            resultItem.FinalMaterialName = finalPl.Partslist.Material.MaterialName1;
                            resultItem.FinalPartslistNo = finalPl.Partslist.PartslistNo;
                            resultItem.FinalPartslistName = finalPl.Partslist.PartslistName;
                        }

                        result.Add(resultItem);
                    }
                }
            }

            WorkTaskOnHoldList = result;

            if (SelectedWorkTaskSort == null)
                SelectedWorkTaskSort = WorkTaskSortItems?.FirstOrDefault();
            else
            {
                SortWorkTasks();
            }
        }

        private void SortWorkTasks()
        {
            if (_SelectedWorkTaskSort == null)
                return;

            if ((WorkTaskOnHoldSortEnum)_SelectedWorkTaskSort.Value == WorkTaskOnHoldSortEnum.BatchSeq)
            {
                WorkTaskOnHoldList = WorkTaskOnHoldList.OrderBy(c => c.POProgramNo)
                                                       .ThenBy(c => c.PartslistNo)
                                                       .ThenBy(c => c.Sequence).ToList();
            }
            else if ((WorkTaskOnHoldSortEnum)_SelectedWorkTaskSort.Value == WorkTaskOnHoldSortEnum.BatchSeqDesc)
            {
                WorkTaskOnHoldList = WorkTaskOnHoldList.OrderBy(c => c.POProgramNo)
                                                       .ThenBy(c => c.PartslistNo)
                                                       .ThenByDescending(c => c.Sequence).ToList();
            }
            else if ((WorkTaskOnHoldSortEnum)_SelectedWorkTaskSort.Value == WorkTaskOnHoldSortEnum.WFStartDate)
            {
                WorkTaskOnHoldList = WorkTaskOnHoldList.OrderBy(c => c.StartDate).ToList();
            }
            else if ((WorkTaskOnHoldSortEnum)_SelectedWorkTaskSort.Value == WorkTaskOnHoldSortEnum.WFStartDateDesc)
            {
                WorkTaskOnHoldList = WorkTaskOnHoldList.OrderByDescending(c => c.StartDate).ToList();
            }
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;

            switch (acMethodName)
            {
                case nameof(StartWorkTask):
                    StartWorkTask();
                    return true;
                case nameof(IsEnabledStartWorkTask):
                    result = IsEnabledStartWorkTask();
                    return true;
                case nameof(CompleteWorkTask):
                    CompleteWorkTask();
                    return true;
                case nameof(IsEnabledCompleteWorkTask):
                    result = IsEnabledCompleteWorkTask();
                    return true;
                case nameof(RefreshWorkTasks):
                    RefreshWorkTasks();
                    return true;
            }

            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(StartWorkTask):
                case nameof(CompleteWorkTask):
                    return new string[] { nameof(SelectedWorkTaskOnHold) };
                case nameof(RefreshWorkTasks):
                    return new string[] { nameof(InitState) };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }


        [ACMethodInteraction("ShowComponents", "en{'Show components'}de{'Komponenten anzeigen'}", 503, false, nameof(SelectedWorkTaskOnHold),
                             Description = @"Opens the components dialog of the batch plan (ProdOrderBatchPlan) of the selected work task.
                                             Enabled when the selected work task has a batch plan.")]
        public void ShowComponents()
        {
            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(
                new PAOrderInfoEntry()
                {
                    EntityID = SelectedWorkTaskOnHold.ProdOrderBatchPlanID ?? Guid.Empty,
                    EntityName = ProdOrderBatchPlan.ClassName
                });
                service.ShowDialogComponents(this, info);
            }
        }

        public bool IsEnabledShowComponents()
        {
            return SelectedWorkTaskOnHold != null && SelectedWorkTaskOnHold.ProdOrderBatchPlanID != null;
        }

        #endregion
    }


    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Work task sort'}de{'Arbeitsaufgabe sortieren'}", Global.ACKinds.TACEnum, QRYConfig = "gip.bso.manufacturing.ACValueListWorkTaskOnHoldSortEnum")]
    public enum WorkTaskOnHoldSortEnum : short
    {
        BatchSeq = 0,
        BatchSeqDesc = 10,
        WFStartDate = 20,
        WFStartDateDesc = 30
    }


    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Work task sort'}de{'Arbeitsaufgabe sortieren'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListWorkTaskOnHoldSortEnum : ACValueItemList
    {
        public ACValueListWorkTaskOnHoldSortEnum() : base("WorkTaskSort")
        {
            AddEntry(WorkTaskOnHoldSortEnum.BatchSeq, "en{'Batch sequence'}de{'Chargenfolge'}");
            AddEntry(WorkTaskOnHoldSortEnum.BatchSeqDesc, "en{'Batch sequence desc.'}de{'Chargenfolge desc.'}");
            AddEntry(WorkTaskOnHoldSortEnum.WFStartDate, "en{'Start date'}de{'Datum des Beginns'}");
            AddEntry(WorkTaskOnHoldSortEnum.WFStartDateDesc, "en{'Start date desc.'}de{'Startdatum desc.'}");
        }
    }


}
