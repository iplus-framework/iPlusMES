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

namespace gip.bso.manufacturing
{
    /// <summary>
    /// Class BSOProcessControl
    /// </summary>
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Processcontrol'}de{'Prozesssteuerung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + gip.core.datamodel.ACProgram.ClassName)]
    public class BSOProcessControlVB : BSOProcessControl
    {
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
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
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

        #region Workflows
        /// <summary>
        /// The _ selected AC task
        /// </summary>
        ACClassTaskWrapperVB _SelectedACTaskVB;
        /// <summary>
        /// Gets or sets the selected AC task.
        /// </summary>
        /// <value>The selected AC task.</value>
        [ACPropertySelected(500, "Workflow-Live-VB")]
        public ACClassTaskWrapperVB SelectedACTaskVB
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
                        CurrentACTask = _SelectedACTaskVB.ACClassTask.FromIPlusContext<gip.core.datamodel.ACClassTask>(DatabaseApp.ContextIPlus);
                        SelectedACTask = CurrentACTask;
                    }

                    OnPropertyChanged("SelectedACTaskVB");
                }
            }
        }


        protected IEnumerable<ACClassTaskWrapperVB> _ACTaskVBList;
        /// <summary>
        /// Gets the AC task list.
        /// </summary>
        /// <value>The AC task list.</value>
        [ACPropertyList(501, "Workflow-Live-VB")]
        public IEnumerable<ACClassTaskWrapperVB> ACTaskVBList
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
        static readonly Func<DatabaseApp, Guid, IEnumerable<ACClassTaskWrapperVB>> s_cQry_TasklistByTaskID =
            CompiledQuery.Compile<DatabaseApp, Guid, IEnumerable<ACClassTaskWrapperVB>>(
                (db, rootACClassTaskID) =>
                    db.ACClassTask
                    .Include("TaskTypeACClass")
                    .Include("ContentACClassWF")
                    .Include("ACProgram")
                    .Include("ACClassTask_ParentACClassTask")
                    .Include("ProdOrderPartslistPos_ACClassTask")
                    .Include("ACProgram.ProdOrderPartslist_VBiACProgram")
                    .Include("ACProgram.ProdOrderPartslist_VBiACProgram.ProdOrder")
                    .Include("ACProgram.ProdOrderPartslist_VBiACProgram.Partslist.Material")
                    .Where(c => c.ParentACClassTaskID.HasValue && c.ParentACClassTaskID == rootACClassTaskID
                        && c.IsDynamic
                        && c.ACTaskTypeIndex == (short)Global.ACTaskTypes.WorkflowTask)
                    .OrderBy(c => c.ACProgram.ProgramNo)
                    .ThenByDescending(c => c.InsertDate)
                    .Select(c => new ACClassTaskWrapperVB()
                    {
                        ACClassTask = c,
                        ProdOrderPartslist = c.ACProgram.ACProgramLog_ACProgram
                                                    .Where(f => f.OrderLog_VBiACProgramLog != null && f.OrderLog_VBiACProgramLog.ProdOrderPartslistPosID.HasValue && f.OrderLog_VBiACProgramLog.ProdOrderPartslistPos.ProdOrderPartslist != null)
                                                    .Select(f => f.OrderLog_VBiACProgramLog.ProdOrderPartslistPos.ProdOrderPartslist)
                                                    .FirstOrDefault(),
                        ProdOrderPartslistPos = c.ProdOrderPartslistPos_ACClassTask.FirstOrDefault(),
                        ProdOrderBatch = c.ProdOrderPartslistPos_ACClassTask.FirstOrDefault() != null ? c.ProdOrderPartslistPos_ACClassTask.FirstOrDefault().ProdOrderBatch : null,
                        PickingPos = c.ACProgram.ACProgramLog_ACProgram
                                                        .Where(f => f.OrderLog_VBiACProgramLog != null && f.OrderLog_VBiACProgramLog.PickingPosID != null)
                                                        .Select(f => f.OrderLog_VBiACProgramLog.PickingPos)
                                                        .FirstOrDefault()

                    })
            );

        static readonly Func<DatabaseApp, Guid, IEnumerable<ACClassTaskWrapperVB>> s_cQry_TasklistByPWClassID =
            CompiledQuery.Compile<DatabaseApp, Guid, IEnumerable<ACClassTaskWrapperVB>>(
                (db, pwACClassID) =>
                    db.ACClassTask
                        .Include("TaskTypeACClass")
                        .Include("ContentACClassWF")
                        .Include("ACProgram")
                        .Include("ACClassTask_ParentACClassTask")
                        .Include("ProdOrderPartslistPos_ACClassTask")
                        .Include("ACProgram.ProdOrderPartslist_VBiACProgram")
                        .Include("ACProgram.ProdOrderPartslist_VBiACProgram.ProdOrder")
                        .Include("ACProgram.ProdOrderPartslist_VBiACProgram.Partslist.Material")
                        .Where(c => c.IsDynamic
                                && c.ACProgramID.HasValue
                                && c.ACTaskTypeIndex == (short)Global.ACTaskTypes.WorkflowTask
                                && c.ParentACClassTaskID.HasValue && c.ACClassTask1_ParentACClassTask.TaskTypeACClass.ACKindIndex == (short)Global.ACKinds.TACApplicationManager
                                && c.ACClassTask1_ParentACClassTask.TaskTypeACClass.ACProject.IsWorkflowEnabled
                                && c.ContentACClassWFID.HasValue && c.ContentACClassWF.PWACClass.ACClassID == pwACClassID)
                        .OrderBy(c => c.ACProgram.ProgramNo)
                        .ThenByDescending(c => c.InsertDate)
                        .Select(c => new ACClassTaskWrapperVB()
                        {
                            ACClassTask = c,
                            ProdOrderPartslist = c.ACProgram.ACProgramLog_ACProgram
                                                    .Where(f => f.OrderLog_VBiACProgramLog != null && f.OrderLog_VBiACProgramLog.ProdOrderPartslistPosID.HasValue && f.OrderLog_VBiACProgramLog.ProdOrderPartslistPos.ProdOrderPartslist != null)
                                                    .Select(f => f.OrderLog_VBiACProgramLog.ProdOrderPartslistPos.ProdOrderPartslist)
                                                    .FirstOrDefault(),
                            ProdOrderPartslistPos = c.ProdOrderPartslistPos_ACClassTask.FirstOrDefault(),
                            ProdOrderBatch = c.ProdOrderPartslistPos_ACClassTask.FirstOrDefault() != null ? c.ProdOrderPartslistPos_ACClassTask.FirstOrDefault().ProdOrderBatch : null,
                            PickingPos = c.ACProgram.ACProgramLog_ACProgram
                                                            .Where(f => f.OrderLog_VBiACProgramLog != null && f.OrderLog_VBiACProgramLog.PickingPosID != null)
                                                            .Select(f => f.OrderLog_VBiACProgramLog.PickingPos)
                                                            .FirstOrDefault()
                        })
            );

        static readonly Func<DatabaseApp, IEnumerable<ACClassTaskWrapperVB>> s_cQry_TasklistAll =
            CompiledQuery.Compile<DatabaseApp, IEnumerable<ACClassTaskWrapperVB>>(
                (db) =>
                    db.ACClassTask
                        .Include("TaskTypeACClass")
                        .Include("ContentACClassWF")
                        .Include("ACProgram")
                        .Include("ProdOrderPartslistPos_ACClassTask")
                        .Include("ACClassTask_ParentACClassTask")
                        .Include("ACProgram.ProdOrderPartslist_VBiACProgram")
                        .Include("ACProgram.ProdOrderPartslist_VBiACProgram.ProdOrder")
                        .Include("ACProgram.ProdOrderPartslist_VBiACProgram.Partslist.Material")
                        .Where(c => c.IsDynamic
                                && c.ACProgramID.HasValue
                                && c.ACTaskTypeIndex == (short)Global.ACTaskTypes.WorkflowTask
                                && c.ParentACClassTaskID.HasValue && c.ACClassTask1_ParentACClassTask.TaskTypeACClass.ACKindIndex == (short)Global.ACKinds.TACApplicationManager
                                && c.ACClassTask1_ParentACClassTask.TaskTypeACClass.ACProject.IsWorkflowEnabled)
                        .OrderBy(c => c.ACProgram.ProgramNo)
                        .ThenByDescending(c => c.InsertDate)
                        .Select(c => new ACClassTaskWrapperVB()
                        {
                            ACClassTask = c,
                            ProdOrderPartslist = c.ACProgram.ACProgramLog_ACProgram
                                                    .Where(f => f.OrderLog_VBiACProgramLog != null && f.OrderLog_VBiACProgramLog.ProdOrderPartslistPosID.HasValue && f.OrderLog_VBiACProgramLog.ProdOrderPartslistPos.ProdOrderPartslist != null)
                                                    .Select(f => f.OrderLog_VBiACProgramLog.ProdOrderPartslistPos.ProdOrderPartslist)
                                                    .FirstOrDefault(),
                            ProdOrderPartslistPos = c.ProdOrderPartslistPos_ACClassTask.FirstOrDefault(),
                            ProdOrderBatch = c.ProdOrderPartslistPos_ACClassTask.FirstOrDefault() != null ? c.ProdOrderPartslistPos_ACClassTask.FirstOrDefault().ProdOrderBatch : null,
                            PickingPos = c.ACProgram.ACProgramLog_ACProgram
                                                            .Where(f => f.OrderLog_VBiACProgramLog != null && f.OrderLog_VBiACProgramLog.PickingPosID != null)
                                                            .Select(f => f.OrderLog_VBiACProgramLog.PickingPos)
                                                            .FirstOrDefault()
                        })
            );

        protected override bool LoadACTaskList(FilterMode filterMode, bool forceUpdateTaskList)
        {
            _NeedSearch = false;
            bool taskListChanged = true;
            ACClassTaskWrapperVB[] newTaskList = null;
            if (filterMode == FilterMode.ByApplication)
            {
                if (CurrentApplicationManager == null)
                {
                    taskListChanged = _ACTaskVBList != null;
                    EmptyACTaskList();
                    return taskListChanged;
                }

                gip.mes.datamodel.ACClassTask rootTaskAppManger = this.DatabaseApp.ACClassTask.Where(c => c.TaskTypeACClassID == CurrentApplicationManager.ACClassID && !c.IsTestmode).FirstOrDefault();
                if (rootTaskAppManger == null)
                {
                    taskListChanged = _ACTaskVBList != null;
                    EmptyACTaskList();
                    return taskListChanged;
                }

                newTaskList = s_cQry_TasklistByTaskID(this.DatabaseApp, rootTaskAppManger.ACClassTaskID).ToArray();
            }
            else
            {
                if (CurrentProgramType == null)
                {
                    taskListChanged = _ACTaskVBList != null;
                    EmptyACTaskList();
                    return taskListChanged;
                }

                if (CurrentProgramType.ACObject is gip.core.datamodel.ACClass)
                {
                    gip.core.datamodel.ACClass pwACClass = CurrentProgramType.ACObject as gip.core.datamodel.ACClass;
                    newTaskList = s_cQry_TasklistByPWClassID(this.DatabaseApp, pwACClass.ACClassID).ToArray();
                }
                else
                {
                    newTaskList = s_cQry_TasklistAll(this.DatabaseApp).ToArray();
                }
            }

            if (!forceUpdateTaskList && _ACTaskVBList != null)
            {
                taskListChanged = newTaskList.Except(_ACTaskVBList, new ACClassTaskWrapperVBComparer()).Any();
                if (taskListChanged)
                    _ACTaskVBList = newTaskList;
            }
            else
                _ACTaskVBList = newTaskList;

            if (taskListChanged)
            {
                OnPropertyChanged("ACTaskVBList");
                if (_ACTaskVBList != null)
                {
                    var currentACTask = _ACTaskVBList.FirstOrDefault();
                    SelectedACTaskVB = currentACTask;
                }
            }
            return taskListChanged;
        }

        protected override void EmptyACTaskList()
        {
            _ACTaskList = null;
            _ACTaskVBList = null;
            CurrentACTask = null;
            SelectedACTaskVB = null;
            SelectedACTask = null;
            OnPropertyChanged("ACTaskList");
            OnPropertyChanged("ACTaskVBList");
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
            SelectedACTaskVB = _ACTaskVBList.FirstOrDefault(x => x.ACClassTask.ACClassTaskID == task.ACClassTaskID);
            ShowWorkflow();
        }
        #endregion  

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "DeleteWorkflowVB":
                    DeleteWorkflowVB();
                    return true;
                case "IsEnabledDeleteWorkflowVB":
                    result = IsEnabledDeleteWorkflowVB();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }

}
