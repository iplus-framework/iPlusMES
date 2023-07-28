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

        protected override bool LoadACTaskList(FilterMode filterMode, bool forceUpdateTaskList)
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

                // newTaskList = s_cQry_TasklistByTaskID(this.DatabaseApp, rootTaskAppManger.ACClassTaskID, FilterOrderNo, FilterMaterialNo).ToArray();
                newTaskList = GetACClassTaskModels(DatabaseApp, rootTaskAppManger.ACClassTaskID, null, FilterMaterialNo, FilterOrderNo);
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

            return taskListChanged;
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

        private ACClassTaskModel[] GetACClassTaskModels(DatabaseApp databaseApp, Guid? rootACClassTaskID, Guid? pwACClassID, string materialNo, string orderNo)
        {
            List<ACClassTaskModel> result = new List<ACClassTaskModel>();

            List<gip.mes.datamodel.ACClassTask> tasks = GetTasks(databaseApp, rootACClassTaskID, pwACClassID, materialNo, orderNo);
            Guid[] aCProgramIDs = tasks.Select(c => c.ACProgramID ?? Guid.Empty).ToArray();


            foreach (gip.mes.datamodel.ACClassTask task in tasks)
            {
                ACClassTaskModel model = new ACClassTaskModel();

                model.ACClassTaskID = task.ACClassTaskID;

                ProdOrderPartslistPos prodOrderPartslistPos =
                    task
                    .ACProgram
                    .ACClassTask_ACProgram
                    .SelectMany(c => c.ProdOrderPartslistPos_ACClassTask)
                    .Where(c => c.ParentProdOrderPartslistPosID != null)
                    .FirstOrDefault();

                PickingPos pickingPos =
                     task
                    .ACProgram
                    .ACClassTask_ACProgram
                    .SelectMany(c => c.PickingPos_ACClassTask)
                    .FirstOrDefault();

                model.ProgramNo = "";
                if (prodOrderPartslistPos != null)
                {
                    model.ProgramNo = prodOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo;
                }
                if (pickingPos != null)
                {
                    model.ProgramNo = pickingPos.Picking.PickingNo;
                }

                model.BatchNo = "";
                if (prodOrderPartslistPos != null)
                {
                    model.BatchNo = prodOrderPartslistPos.ProdOrderBatch.BatchSeqNo.ToString();
                }
                if (pickingPos != null)
                {
                    if (pickingPos.FromFacility != null)
                    {
                        model.BatchNo = pickingPos.FromFacility?.FacilityName;
                    }
                    else if (pickingPos.ToFacility != null)
                    {
                        model.BatchNo = pickingPos.ToFacility?.FacilityName;
                    }
                }

                model.Material = "";
                if (prodOrderPartslistPos != null)
                {
                    model.Material = prodOrderPartslistPos.ProdOrderPartslist.Partslist.Material.MaterialName1;
                }
                if (pickingPos != null)
                {
                    model.Material = pickingPos.Material.MaterialName1;
                }

                model.InsertDate = task.InsertDate;
                model.ACProgramNo = task.ACProgram?.ProgramNo;
                model.ACIdentifier = task.ACIdentifier;

                result.Add(model);
            }



            return result.ToArray();
        }

        private List<gip.mes.datamodel.ACClassTask> GetTasks(DatabaseApp databaseApp, Guid? rootACClassTaskID, Guid? pwACClassID, string materialNo, string orderNo)
        {
            return
                databaseApp
                .ACClassTask

                .Include(c => c.TaskTypeACClass)
                .Include(c => c.ContentACClassWF)
                .Include(c => c.ContentACClassWF.PWACClass)

                .Include(c => c.ACProgram)

                .Include(c => c.ProdOrderPartslistPos_ACClassTask)
                .Include("ProdOrderPartslistPos_ACClassTask.ProdOrderPartslist.ProdOrder")
                .Include("ProdOrderPartslistPos_ACClassTask.ProdOrderPartslist.Partslist.Material")

                .Include(c => c.PickingPos_ACClassTask)
                .Include("PickingPos_ACClassTask.Picking")
                .Include("PickingPos_ACClassTask.PickingMaterial")

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
                            string.IsNullOrEmpty(orderNo)
                            ||
                                c.ProdOrderPartslistPos_ACClassTask
                                .Select(x => x.ProdOrderPartslist.ProdOrder)
                                .Where(x => x.ProgramNo.Contains(orderNo))
                                .Any()
                            ||
                            c.ACProgram
                            .ACProgramLog_ACProgram
                            .Select(x => x.OrderLog_VBiACProgramLog)
                            .Where(x => x.PickingPos.Picking.PickingNo.Contains(orderNo))
                            .Any()
                    )
                    && (
                            string.IsNullOrEmpty(materialNo)
                            ||
                                c.ProdOrderPartslistPos_ACClassTask
                                .Select(x => x.ProdOrderPartslist.Partslist.Material)
                                .Where(x => x.MaterialNo.Contains(materialNo) || x.MaterialName1.Contains(materialNo))
                                .Any()
                            ||
                            c.ACProgram
                            .ACProgramLog_ACProgram
                            .Select(x => x.OrderLog_VBiACProgramLog)
                            .Where(x => x.PickingPos.PickingMaterial.MaterialNo.Contains(materialNo) || x.PickingPos.PickingMaterial.MaterialName1.Contains(materialNo))
                            .Any()
                    )
                )
                .OrderBy(c => c.ACProgram.ProgramNo)
                .ThenByDescending(c => c.InsertDate)
                .ToList();

        }

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


    }

}
