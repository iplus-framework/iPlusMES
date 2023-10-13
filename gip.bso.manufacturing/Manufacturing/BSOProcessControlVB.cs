// ***********************************************************************
// Assembly         : gip.bso.iplus
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 11-15-2012
// ***********************************************************************
// <copyright file="BSOProcessControl.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System.Data.Objects;
using gip.bso.iplus;
using gip.mes.processapplication;
using gip.mes.facility;
using System.ComponentModel;
using System.Collections.ObjectModel;
using static gip.bso.iplus.BSOProcessControl;
using gip.core.layoutengine;

namespace gip.bso.manufacturing
{
    /// <summary>
    /// Class BSOProcessControl
    /// </summary>
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Processcontrol'}de{'Prozesssteuerung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + gip.core.datamodel.ACProgram.ClassName)]
    public class BSOProcessControlVB : BSOProcessControl
    {
        #region const
        public const string BGWorkerMehtod_DoSearchWorkflows = @"DoSearchWorkflows";
        #endregion

        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOProcessControl"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOProcessControlVB(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _UseBackGroundWorker = new ACPropertyConfigValue<bool>(this, nameof(UseBackGroundWorker), false);
        }

        /// <summary>
        /// ACs the init.
        /// </summary>
        /// <param name="startChildMode">The start child mode.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            _ = UseBackGroundWorker;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }
        #endregion

        #region Configuration

        private ACPropertyConfigValue<bool> _UseBackGroundWorker;
        [ACPropertyConfig("en{'Use Background Worker'}de{'Use Background Worker'}")]
        public bool UseBackGroundWorker
        {
            get
            {
                return _UseBackGroundWorker.ValueT;
            }
            set
            {
                _UseBackGroundWorker.ValueT = value;
            }
        }

        #endregion

        #region BSO->ACProperty

        #region Database
        private DatabaseApp _DatabaseApp = null;
        /// <summary>Returns the shared Database-Context for BSO's by calling GetAppContextForBSO()</summary>
        /// <value>Returns the shared Database-Context.</value>
        public virtual DatabaseApp DatabaseApp
        {
            get
            {
                if (_DatabaseApp == null && this.InitState != ACInitState.Destructed && this.InitState != ACInitState.Destructing && this.InitState != ACInitState.DisposedToPool && this.InitState != ACInitState.DisposingToPool)
                    _DatabaseApp = this.GetAppContextForBSO();
                return _DatabaseApp as DatabaseApp;
            }
        }

        /// <summary>
        /// Overriden: Returns the DatabaseApp-Property.
        /// </summary>
        /// <value>The context as IACEntityObjectContext.</value>
        public override IACEntityObjectContext Database
        {
            get
            {
                return DatabaseApp;
            }
        }
        #endregion

        #region Filter
        // OrderInfo1,OrderInfo2,OrderInfo3,ProgramNo,ACIdentifier,InsertDate

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilterOrderNo;
        [ACPropertySelected(999, "FilterOrderNo", "en{'ProgramNo.'}de{'AuftragNo.'}")]
        public string FilterOrderNo
        {
            get
            {
                return _FilterOrderNo;
            }
            set
            {
                if (_FilterOrderNo != value)
                {
                    _FilterOrderNo = value;
                    OnPropertyChanged(nameof(FilterOrderNo));
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilterMaterialNo;
        [ACPropertySelected(999, "FilterMaterialNo", "en{'Material'}de{'Material'}")]
        public string FilterMaterialNo
        {
            get
            {
                return _FilterMaterialNo;
            }
            set
            {
                if (_FilterMaterialNo != value)
                {
                    _FilterMaterialNo = value;
                    OnPropertyChanged(nameof(FilterMaterialNo));
                }
            }
        }

        #endregion

        #region Workflows
        /// <summary>
        /// The _ selected AC task
        /// </summary>
        ACClassTaskModel _SelectedACTaskVB;
        /// <summary>
        /// Gets or sets the selected AC task.
        /// </summary>
        /// <value>The selected AC task.</value>
        [ACPropertySelected(500, "Workflow-Live-VB")]
        public ACClassTaskModel SelectedACTaskVB
        {
            get
            {
                return _SelectedACTaskVB;
            }
            set
            {
                if (_SelectedACTaskVB != value || (value == null && SelectedACTask != null))
                {
                    _SelectedACTaskVB = value;
                    if (_SelectedACTaskVB == null)
                    {
                        CurrentACTask = null;
                        SelectedACTask = null;
                    }
                    else
                    {
                        //CurrentACTask = _SelectedACTaskVB.ACClassTask.FromIPlusContext<gip.core.datamodel.ACClassTask>(DatabaseApp.ContextIPlus);
                        CurrentACTask =
                            DatabaseApp
                            .ContextIPlus
                            .ACClassTask
                            .FirstOrDefault(c => c.ACClassTaskID == value.ACClassTaskID);
                        SelectedACTask = CurrentACTask;
                    }

                    OnPropertyChanged();
                }
            }
        }


        protected IEnumerable<ACClassTaskModel> _ACTaskVBList;
        /// <summary>
        /// Gets the AC task list.
        /// </summary>
        /// <value>The AC task list.</value>
        [ACPropertyList(501, "Workflow-Live-VB")]
        public IEnumerable<ACClassTaskModel> ACTaskVBList
        {
            get
            {
                return _ACTaskVBList;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Refresh Tasklist
        FilterMode _filterMode;
        bool _forceUpdateTaskList;
        protected override bool LoadACTaskList(FilterMode filterMode, bool forceUpdateTaskList)
        {
            if (UseBackGroundWorker)
            {
                _filterMode = filterMode;
                _forceUpdateTaskList = forceUpdateTaskList;
                EmptyACTaskList();
                BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoSearchWorkflows);
                ShowDialog(this, DesignNameProgressBar);
            }
            else
            {
                _ACTaskVBList = DoSearchWorkflows(filterMode, forceUpdateTaskList);
                OnPropertyChanged(nameof(ACTaskList));
                if (_ACTaskList != null)
                {
                    CurrentACTask = _ACTaskList.FirstOrDefault();
                    SelectedACTask = CurrentACTask;
                }
            }

            return true;
        }

        protected override void EmptyACTaskList()
        {
            _ACTaskList = null;
            _ACTaskVBList = null;
            CurrentACTask = null;
            SelectedACTaskVB = null;
            SelectedACTask = null;
            OnPropertyChanged(nameof(ACTaskList));
            OnPropertyChanged(nameof(ACTaskVBList));
        }

        protected override bool SelectACTaskAndShowWF(string taskACIdentifier)
        {
            if (ACTaskVBList == null || !ACTaskVBList.Any())
                return false;
            var task = ACTaskVBList.Where(c => c.ACIdentifier == taskACIdentifier).FirstOrDefault();
            if (task == null)
                return false;
            SelectedACTaskVB = task;
            ShowWorkflow();
            return true;
        }

        #endregion

        [ACMethodInteraction("Workflow-Live-VB", "en{'Delete inactive workflow'}de{'Inaktiven Workflow löschen'}", 501, true, "SelectedACTaskVB")]
        public void DeleteWorkflowVB()
        {
            DeleteWorkflow();
        }

        public bool IsEnabledDeleteWorkflowVB()
        {
            return IsEnabledDeleteWorkflow();
        }

        protected override void OnDeleteACClassTask(gip.core.datamodel.ACClassTask acClassTask)
        {
            var positions = DatabaseApp.ProdOrderPartslistPos.Where(c => c.ACClassTaskID.HasValue && c.ACClassTaskID == acClassTask.ACClassTaskID);
            foreach (var pos in positions)
            {
                pos.ACClassTaskID = null;
            }
        }

        #endregion

        #region ITaskPreviewCall
        public override void PreviewTask(core.datamodel.ACClass applicationManager, core.datamodel.ACClassTask task)
        {
            CurrentApplicationManager = applicationManager;
            SelectedACTaskVB = _ACTaskVBList.FirstOrDefault(x => x.ACClassTaskID == task.ACClassTaskID);
            ShowWorkflow();
        }
        #endregion


        #region provide ACClassTaskModel list


        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(DeleteWorkflowVB):
                    DeleteWorkflowVB();
                    return true;
                case nameof(IsEnabledDeleteWorkflowVB):
                    result = IsEnabledDeleteWorkflowVB();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

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
                case BGWorkerMehtod_DoSearchWorkflows:
                    using (ACMonitor.Lock(DatabaseApp.QueryLock_1X000))
                    {
                        e.Result = DoSearchWorkflows(_filterMode, _forceUpdateTaskList);
                    }
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
                if (command == BGWorkerMehtod_DoSearchWorkflows)
                {
                    _ACTaskVBList = e.Result as IEnumerable<ACClassTaskModel>;
                    OnPropertyChanged(nameof(ACTaskList));
                    if (_ACTaskList != null)
                    {
                        CurrentACTask = _ACTaskList.FirstOrDefault();
                        SelectedACTask = CurrentACTask;
                    }
                }
            }
        }

        #endregion

        #region BackgroundWorker -> DoMehtods

        private List<ACClassTaskModel> DoSearchWorkflows(FilterMode filterMode, bool forceUpdateTaskList)
        {
            using (ACMonitor.Lock(DatabaseApp.QueryLock_1X000))
            {
                // Temp set CommandTimeout for long lasting search by material
                int? commandTimeout = DatabaseApp.CommandTimeout;
                DatabaseApp.CommandTimeout = 60 * 2;

                _NeedSearch = false;
                bool taskListChanged = true;
                ACClassTaskModel[] newTaskList = null;
                if (filterMode == FilterMode.ByApplication)
                {
                    if (CurrentApplicationManager == null)
                    {
                        taskListChanged = _ACTaskVBList != null;

                    }

                    gip.mes.datamodel.ACClassTask rootTaskAppManger = this.DatabaseApp.ACClassTask.Where(c => c.TaskTypeACClassID == CurrentApplicationManager.ACClassID && !c.IsTestmode).FirstOrDefault();
                    if (rootTaskAppManger == null)
                    {
                        taskListChanged = _ACTaskVBList != null;

                    }

                    // newTaskList = s_cQry_TasklistByTaskID(this.DatabaseApp, rootTaskAppManger.ACClassTaskID, FilterOrderNo, FilterMaterialNo).ToArray();
                    newTaskList = GetACClassTaskModels(DatabaseApp, rootTaskAppManger.ACClassTaskID, null, FilterMaterialNo, FilterOrderNo);
                }
                else
                {
                    if (CurrentProgramType == null)
                    {
                        taskListChanged = _ACTaskVBList != null;

                    }

                    if (CurrentProgramType.ACObject is gip.core.datamodel.ACClass)
                    {
                        gip.core.datamodel.ACClass pwACClass = CurrentProgramType.ACObject as gip.core.datamodel.ACClass;
                        //newTaskList = s_cQry_TasklistByPWClassID(this.DatabaseApp, pwACClass.ACClassID, FilterOrderNo, FilterMaterialNo).ToArray();
                        newTaskList = GetACClassTaskModels(DatabaseApp, null, pwACClass.ACClassID, FilterOrderNo, FilterMaterialNo);
                    }
                    else
                    {
                        //newTaskList = s_cQry_TasklistAll(this.DatabaseApp).ToArray();
                        newTaskList = GetACClassTaskModels(DatabaseApp, null, null, FilterOrderNo, FilterMaterialNo);
                    }
                }

                if (!forceUpdateTaskList && _ACTaskVBList != null)
                {
                    Guid[] fetched = newTaskList.Select(c => c.ACClassTaskID).ToArray();
                    Guid[] existing = _ACTaskVBList.Select(c => c.ACClassTaskID).ToArray();
                    taskListChanged = fetched.SequenceEqual(existing);
                    if (taskListChanged)
                        _ACTaskVBList = newTaskList;
                }
                else
                    _ACTaskVBList = newTaskList;

                if (taskListChanged)
                {
                    OnPropertyChanged(nameof(ACTaskVBList));
                    if (_ACTaskVBList != null)
                    {
                        var currentACTask = _ACTaskVBList.FirstOrDefault();
                        SelectedACTaskVB = currentACTask;
                    }
                }

                DatabaseApp.CommandTimeout = commandTimeout;

                return newTaskList.ToList();
            }
        }

        public mes.datamodel.ACClassTask[] GetTasks_Classic(DatabaseApp databaseApp, Guid? rootACClassTaskID, Guid? pwACClassID, string materialNo, string orderNo)
        {
            var query = s_cQry_ACClassTask(databaseApp, rootACClassTaskID, pwACClassID, materialNo, orderNo);
            return query.ToArray();
        }

        public mes.datamodel.ACClassTask[] GetTasksSimplified(DatabaseApp databaseApp, Guid? rootACClassTaskID, Guid? pwACClassID, string materialNo, string orderNo)
        {

            return
                databaseApp
                .ACClassTask
                .Where(c =>

                    // common
                    c.IsDynamic
                    && c.ACProgramID != null
                    && c.ACTaskTypeIndex == (short)gip.core.datamodel.Global.ACTaskTypes.WorkflowTask

                    // rootACClassTaskID
                    && (rootACClassTaskID == null || (c.ParentACClassTaskID.HasValue && c.ParentACClassTaskID == rootACClassTaskID))

                    // pwACClassID
                    && (pwACClassID == null ||
                        (
                            c.ParentACClassTaskID.HasValue && c.ACClassTask1_ParentACClassTask.TaskTypeACClass.ACKindIndex == (short)Global.ACKinds.TACApplicationManager
                            && c.ACClassTask1_ParentACClassTask.TaskTypeACClass.ACProject.IsWorkflowEnabled
                            && c.ContentACClassWFID.HasValue && c.ContentACClassWF.PWACClass.ACClassID == pwACClassID
                        )
                    )
                )
                .ToArray();
        }


        private ACClassTaskModel[] GetACClassTaskModels(DatabaseApp databaseApp, Guid? rootACClassTaskID, Guid? pwACClassID, string materialNo, string orderNo)
        {
            List<ACClassTaskModel> result = new List<ACClassTaskModel>();

            mes.datamodel.ACClassTask[] tasks = GetTasksSimplified(databaseApp, rootACClassTaskID, pwACClassID, materialNo, orderNo);

            foreach (gip.mes.datamodel.ACClassTask task in tasks)
            {
                ACClassTaskModel model = new ACClassTaskModel();

                model.ACClassTaskID = task.ACClassTaskID;

                ProdOrderPartslistPos prodOrderPartslistPos =
                    task
                    .ACProgram
                    .ACClassTask_ACProgram
                    .SelectMany(c => c.ProdOrderPartslistPos_ACClassTask)
                    .Where(c => c.ParentProdOrderPartslistPosID != null && c.ACClassTaskID == task.ACClassTaskID)
                    .FirstOrDefault();
                // prodOrderPartslistPos is null at planning nodes
                if (prodOrderPartslistPos == null)
                {
                  
                    prodOrderPartslistPos =
                    task
                        .ACProgram
                        .ACClassTask_ACProgram
                        .SelectMany(c => c.ProdOrderPartslistPos_ACClassTask)
                        .Where(c => c.ParentProdOrderPartslistPosID != null)
                        .FirstOrDefault();

                    if(prodOrderPartslistPos == null)
                    {
                        prodOrderPartslistPos =
                        task
                           .ACProgram
                           .ACClassTask_ACProgram
                           .SelectMany(c => c.ProdOrderPartslistPos_ACClassTask)
                           .FirstOrDefault();
                    }
                }

                model.Material = "";
                model.BatchNo = "";
                model.ProgramNo = "";
                if (prodOrderPartslistPos != null)
                {
                    model.ProgramNo = prodOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo;
                    model.BatchNo = prodOrderPartslistPos.ProdOrderBatch?.BatchSeqNo.ToString();
                    model.Material = prodOrderPartslistPos.ProdOrderPartslist.Partslist.Material.MaterialName1;
                }
                else
                {
                    PickingPos pickingPos =
                     task
                    .ACProgram
                    .ACClassTask_ACProgram
                    .SelectMany(c => c.PickingPos_ACClassTask)
                    .FirstOrDefault();
                    if (pickingPos != null)
                    {
                        model.ProgramNo = pickingPos.Picking.PickingNo;
                        model.Material = pickingPos.Material?.MaterialName1;
                        if (pickingPos.FromFacility != null)
                            model.BatchNo = pickingPos.FromFacility.FacilityName;
                        else if (pickingPos.ToFacility != null)
                            model.BatchNo = pickingPos.ToFacility.FacilityName;
                    }
                }

                model.InsertDate = task.InsertDate;
                model.ACProgramNo = task.ACProgram?.ProgramNo;
                model.ACIdentifier = task.ACIdentifier;

                result.Add(model);
            }

            if (!string.IsNullOrEmpty(FilterMaterialNo))
            {
                result = result.Where(c => c.Material.ToLower().Contains(FilterMaterialNo.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(FilterOrderNo))
            {
                result = result.Where(c => c.ProgramNo.ToLower().Contains(FilterOrderNo.ToLower())).ToList();
            }

            return result.ToArray();
        }

        #region precompiled query

        public static readonly Func<DatabaseApp, Guid?, Guid?, string, string, IQueryable<gip.mes.datamodel.ACClassTask>> s_cQry_ACClassTask =
        CompiledQuery.Compile<DatabaseApp, Guid?, Guid?, string, string, IQueryable<gip.mes.datamodel.ACClassTask>>(
            (databaseApp, rootACClassTaskID, pwACClassID, materialNo, orderNo) =>
                databaseApp
                .ACClassTask

                .Include("TaskTypeACClass")
                .Include("ContentACClassWF")
                .Include("ContentACClassWF.PWACClass")

            #region Include ACProgram Task list

                .Include("ACClassTask1_ParentACClassTask")
                .Include("ACClassTask1_ParentACClassTask.TaskTypeACClass")
                .Include("ACProgram")
                .Include("ACProgram.ACClassTask_ACProgram")

                .Include("ACProgram.ACClassTask_ACProgram.ProdOrderPartslistPos_ACClassTask")
                .Include("ACProgram.ACClassTask_ACProgram.ProdOrderPartslistPos_ACClassTask.ProdOrderPartslist")
                .Include("ACProgram.ACClassTask_ACProgram.ProdOrderPartslistPos_ACClassTask.ProdOrderPartslist.Partslist")
                .Include("ACProgram.ACClassTask_ACProgram.ProdOrderPartslistPos_ACClassTask.ProdOrderPartslist.Partslist.Material")
                .Include("ACProgram.ACClassTask_ACProgram.ProdOrderPartslistPos_ACClassTask.ProdOrderPartslist.ProdOrder")

                .Include("ACProgram.ACClassTask_ACProgram.PickingPos_ACClassTask")
                .Include("ACProgram.ACClassTask_ACProgram.PickingPos_ACClassTask.Picking")
                .Include("ACProgram.ACClassTask_ACProgram.PickingPos_ACClassTask.PickingMaterial")

            #endregion

                .Where(c =>

                    // common
                    c.IsDynamic
                    && c.ACProgramID != null
                    && c.ACTaskTypeIndex == (short)gip.core.datamodel.Global.ACTaskTypes.WorkflowTask

                    // rootACClassTaskID
                    && (rootACClassTaskID == null || (c.ParentACClassTaskID.HasValue && c.ParentACClassTaskID == rootACClassTaskID))

                    // pwACClassID
                    && (pwACClassID == null ||
                        (
                            c.ParentACClassTaskID.HasValue && c.ACClassTask1_ParentACClassTask.TaskTypeACClass.ACKindIndex == (short)Global.ACKinds.TACApplicationManager
                            && c.ACClassTask1_ParentACClassTask.TaskTypeACClass.ACProject.IsWorkflowEnabled
                            && c.ContentACClassWFID.HasValue && c.ContentACClassWF.PWACClass.ACClassID == pwACClassID
                        )
                    )

                    // orderNo && materialNo
                    && (
                            (string.IsNullOrEmpty(orderNo) && string.IsNullOrEmpty(materialNo))
                            ||
                                c
                               .ACProgram
                                .ACClassTask_ACProgram
                                .SelectMany(x => x.ProdOrderPartslistPos_ACClassTask)
                                .Where(x =>
                                            (string.IsNullOrEmpty(orderNo) || x.ProdOrderPartslist.ProdOrder.ProgramNo.Contains(orderNo))
                                            &&
                                            (string.IsNullOrEmpty(materialNo) || (x.ProdOrderPartslist.Partslist.Material.MaterialNo.Contains(materialNo) || x.ProdOrderPartslist.Partslist.Material.MaterialName1.Contains(materialNo)))
                                )
                                .Any()
                            ||
                                c
                                 .ACProgram
                                .ACClassTask_ACProgram
                                .SelectMany(x => x.PickingPos_ACClassTask)
                                .Where(x =>
                                            (string.IsNullOrEmpty(orderNo) || x.Picking.PickingNo.Contains(orderNo))
                                            &&
                                            (string.IsNullOrEmpty(materialNo) || (x.PickingMaterial.MaterialNo.Contains(materialNo) || x.PickingMaterial.MaterialName1.Contains(materialNo)))
                                 )
                                .Any()
                    )
                )
                .OrderBy(c => c.ACProgram.ProgramNo)
                .ThenByDescending(c => c.InsertDate)
        );

        #endregion

        #endregion

    }

}
