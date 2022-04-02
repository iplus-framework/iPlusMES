using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using System.Diagnostics;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Register work task'}de{'Erfassung Arbeitsaufgabe'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public abstract class PWWorkTaskScanBase : PWNodeProcessMethod
    {
        public const string PWClassName = "PWWorkTaskScanBase";

        #region c´tors
        static PWWorkTaskScanBase()
        {
            RegisterExecuteHandler(typeof(PWWorkTaskScanBase), HandleExecuteACMethod_PWWorkTaskScanBase);
        }

        public PWWorkTaskScanBase(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }
        #endregion

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PWWorkTaskScanBase(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWNodeProcessMethod(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        public T ParentPWMethod<T>() where T : PWMethodVBBase
        {
            if (ParentRootWFNode == null)
                return null;
            return ParentRootWFNode as T;
        }

        public bool IsProduction
        {
            get
            {
                return ParentPWMethod<PWMethodProduction>() != null;
            }
        }

        public bool IsTargetFunction(PAFWorkTaskScanBase pafTSC)
        {
            core.datamodel.ACClass pafClass = null;
            using (ACMonitor.Lock(this.ContextLockForACClassWF))
            {
                pafClass = ContentACClassWF?.RefPAACClassMethod?.AttachedFromACClass;
            }
            if (pafClass == null || pafClass.ObjectType == null || pafTSC.ComponentClass.ObjectType == null)
                return false;
            return pafClass.ObjectType.IsAssignableFrom(pafTSC.ComponentClass.ObjectType);
        }

        public void GetAssignedIntermediate(out Guid intermediatePosID, out Guid intermediateChildPosID)
        {
            intermediatePosID = Guid.Empty;
            intermediateChildPosID = Guid.Empty;
            PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
            if (pwMethodProduction == null)
                return;

            using (var dbIPlus = new Database())
            {
                using (var dbApp = new DatabaseApp(dbIPlus))
                {
                    ProdOrderPartslistPos intermediateChildPos;
                    MaterialWFConnection matWFConnection;
                    ProdOrderBatch batch;
                    ProdOrderBatchPlan batchPlan;
                    ProdOrderPartslistPos intermediatePos;
                    ProdOrderPartslistPos endBatchPos;
                    bool posFound = PWDosing.GetRelatedProdOrderPosForWFNode(this, dbIPlus, dbApp, pwMethodProduction, out intermediateChildPos, out intermediatePos,
                        out endBatchPos, out matWFConnection, out batch, out batchPlan);
                    if (posFound)
                    {
                        intermediatePosID = intermediatePos.ProdOrderPartslistPosID;
                        intermediateChildPosID = intermediateChildPos.ProdOrderPartslistPosID;
                    }
                }
            }
        }

        public virtual Msg OnGetMessageAfterOccupyingProcessModule(PAFWorkTaskScanBase invoker)
        {
            return null;
        }

        public bool OccupyWithPModuleOnScan(PAProcessModule processModule, PAFWorkTaskScanBase invoker)
        {
            bool occupied = ParentPWGroup.OccupyWithPModuleOnScan(processModule);
            if (occupied)
            {
                var acMethod = this.CurrentACMethod.ValueT;
                //OnNewProgramLogAddedToQueue
                core.datamodel.ACProgramLog currentProgramLog = GetCurrentProgramLog(true);

                if (currentProgramLog != null)
                {
                    core.datamodel.ACProgram acProgram = null;
                    ACClassTaskQueue.TaskQueue.ProcessAction(() =>
                    {
                        acProgram = currentProgramLog.ACProgram;
                    });

                    core.datamodel.ACProgramLog subProgramLog = null;
                    Guid componentClassID = invoker.ComponentClass.ACClassID;
                    ACClassTaskQueue.TaskQueue.ProcessAction(() =>
                    {
                        subProgramLog = core.datamodel.ACProgramLog.NewACObject(ACClassTaskQueue.TaskQueue.Context, acProgram);
                        subProgramLog.ACProgramLog1_ParentACProgramLog = currentProgramLog;
                        subProgramLog.ACProgram = acProgram;
                        subProgramLog.ACUrl = invoker.GetACUrl();
                        subProgramLog.ACClassID = componentClassID;
                        if (acMethod != null)
                            subProgramLog.XMLConfig = ACConvert.ObjectToXML(acMethod, true);
                        else
                            subProgramLog.XMLConfig = "";
                        subProgramLog.StartDate = DateTime.Now;
                    }
                    );

                    //ACClassTaskQueue.TaskQueue.ProgramCache.AddProgramLog(subProgramLog);

                    // Eintrag in Queue, Speicherung kann verzögert erfolgen.
                    ACClassTaskQueue.TaskQueue.Add(() =>
                    {
                        acProgram.ACProgramLog_ACProgram.Add(subProgramLog);
                        if (currentProgramLog != null)
                            currentProgramLog.ACProgramLog_ParentACProgramLog.Add(subProgramLog);
                        OnNewProgramLogAddedToQueue(acMethod, subProgramLog);
                    }
                    );
                }
            }
            return occupied;
        }

        public bool ReleaseProcessModuleOnScan(PAFWorkTaskScanBase invoker)
        {
            if (invoker == null || ParentPWGroup == null)
                return false;
            string invokerACUrl = invoker.GetACUrl();
            core.datamodel.ACProgramLog currentProgramLog = GetCurrentProgramLog(true);

            if (currentProgramLog != null)
            {
                ACClassTaskQueue.TaskQueue.Add(() =>
                {
                    core.datamodel.ACProgramLog subProgramLog = ACClassTaskQueue.TaskQueue.Context.ACProgramLog.Where(c => c.ParentACProgramLogID == currentProgramLog.ACProgramLogID && c.ACUrl == invokerACUrl).OrderByDescending(c => c.InsertDate).FirstOrDefault();
                    if (subProgramLog != null)
                    {
                        subProgramLog.EndDate = DateTime.Now;
                        if (subProgramLog.StartDate.HasValue)
                            subProgramLog.Duration = subProgramLog.EndDate.Value - subProgramLog.StartDate.Value;
                        subProgramLog.UpdateDate = DateTime.Now;
                        //subProgramLog.StartDatePlan = GetValidDateTime(PlannedTimes.StartTime);
                        //subProgramLog.DurationPlan = PlannedTimes.Duration;
                        //subProgramLog.EndDatePlan = GetValidDateTime(PlannedTimes.EndTime);
                    }
                });
            }

            PAProcessModule processModule = invoker.ParentACComponent as PAProcessModule;
            if (ParentPWGroup.TrySemaphore.ConnectionListCount > 1)
                return ParentPWGroup.ReleaseProcessModule(processModule);

            if (this.CurrentACState != ACStateEnum.SMIdle)
                this.CurrentACState = ACStateEnum.SMCompleted;
            return true;
        }

        public virtual Msg OnGetMessageOnReleasingProcessModule(PAFWorkTaskScanBase invoker)
        {
            return null;
        }

        public virtual Msg ChangeReceivedParams(PAFWorkTaskScanBase invoker, ACMethod acMethod)
        {
            if (acMethod == null)
                return null;
            ACMethod thisACMethod = ExecutingACMethod;
            if (thisACMethod != null)
            {
                thisACMethod.ResultValueList.CopyValues(acMethod.ResultValueList, true);
                if (CurrentACMethod.ValueT != thisACMethod)
                    UpdateCurrentACMethod();
                return SaveChangedACMethod();
            }
            return null;
        }


        protected override bool RunWithoutInvokingFunction
        {
            get
            {
                return true;
            }
        }

        public override void SMStarting()
        {
            base.SMStarting();
        }
    }
}
