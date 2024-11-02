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
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Tasks on hold'}de{'Aufgaben in der Warteschleife'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, SortIndex = 500)]
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

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties

        private ACRef<IACComponent> _PAFWorkTaskScan;

        private WorkTaskOnHoldItem _SelectedWorkTaskOnHold;
        [ACPropertySelected(700, "WorkTask")]
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
        [ACPropertyList(700, "WorkTask")]
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
        [ACPropertySelected(701, "WorkTaskSort", "en{'Sort'}de{'Sort'}")]
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
        [ACPropertyList(701, "WorkTaskSort")]
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
                Messages.Error(this, "The PAFWorkTaskScan can not be found!", true, null);
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

        [ACMethodInfo("", "en{'Start work task'}de{'Arbeitsaufgabe starten'}", 710, true)]
        public void StartWorkTask()
        {
            if (_PAFWorkTaskScan == null || SelectedWorkTaskOnHold == null)
                return;

            IACComponent paf = _PAFWorkTaskScan.ValueT;
            Msg result = paf.ExecuteMethod(nameof(PAFWorkTaskScanBase.OccupyReleaseProcessModule), SelectedWorkTaskOnHold.WFACUrl, SelectedWorkTaskOnHold.ForRelease) as Msg;
            if (result != null && result.MessageLevel > eMsgLevel.Info)
            {
                Messages.Msg(result);
            }

            RefreshWorkTasks();
        }

        public bool IsEnabledStartWorkTask()
        {
            return SelectedWorkTaskOnHold != null && !SelectedWorkTaskOnHold.ForRelease;
        }

        [ACMethodInfo("", "en{'Complete work task'}de{'Arbeitsaufgabe abschließen'}", 711, true)]
        public void CompleteWorkTask()
        {
            if (_PAFWorkTaskScan == null)
                return;

            IACComponent paf = _PAFWorkTaskScan.ValueT;
            Msg result = paf.ExecuteMethod(nameof(PAFWorkTaskScanBase.OccupyReleaseProcessModule), SelectedWorkTaskOnHold.WFACUrl, SelectedWorkTaskOnHold.ForRelease) as Msg;
            if (result != null && result.MessageLevel > eMsgLevel.Info)
            {
                Messages.Msg(result);
            }

            RefreshWorkTasks();
        }

        public bool IsEnabledCompleteWorkTask()
        {
            return SelectedWorkTaskOnHold != null && SelectedWorkTaskOnHold.ForRelease;
        }

        [ACMethodInfo("", "en{'Refresh work tasks'}de{'Arbeitsaufgaben auffrischen'}", 711, true)]
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
                    result = dbApp.ProdOrderPartslistPos
                                  .Include(i => i.ProdOrderPartslist)
                                  .Include(i => i.ProdOrderPartslist.ProdOrder)
                                  .Include(i => i.ProdOrderPartslist.Partslist)
                                  .Where(c => temp.Any(x => x == c.ProdOrderPartslistPosID))
                                  .OrderBy(c => c.ProdOrderPartslist.ProdOrder.ProgramNo)
                                  .ThenBy(c => c.ProdOrderPartslist.Partslist.PartslistNo)
                                  .ThenBy(c => c.Sequence)
                                  .ToArray()
                                  .Select(x => new Tuple<ProdOrderPartslistPos, PAProdOrderPartslistWFInfo>(x, prodOrderWFInfoList
                                                                                                               .OrderInfos.FirstOrDefault(c => x.ProdOrderPartslistPosID == c.POPPosId)))
                                  .Select(x => new WorkTaskOnHoldItem()
                                  {
                                      POProgramNo = x.Item1.ProdOrderPartslist.ProdOrder.ProgramNo,
                                      StartDate = x.Item2.WFMethodStartDate,
                                      Sequence = x.Item1.Sequence.ToString(),
                                      PartslistNo = x.Item1.ProdOrderPartslist.Partslist.PartslistNo,
                                      PartslistName = x.Item1.ProdOrderPartslist.Partslist.PartslistName,
                                      IntermediateMaterial = x.Item1.MaterialName,
                                      WFACUrl = x.Item2.ACUrlWF,
                                      ForRelease = x.Item2.ForRelease
                                  })
                                  .ToList();
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
