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
                    ProdOrderPartslistPos endBatchPos = pwMethodProduction.CurrentProdOrderPartslistPos.FromAppContext<ProdOrderPartslistPos>(dbApp);
                    if (pwMethodProduction.CurrentProdOrderBatch == null)
                        return;
                    var contentACClassWFVB = ContentACClassWF.FromAppContext<gip.mes.datamodel.ACClassWF>(dbApp);
                    ProdOrderBatch batch = pwMethodProduction.CurrentProdOrderBatch.FromAppContext<ProdOrderBatch>(dbApp);
                    ProdOrderBatchPlan batchPlan = batch.ProdOrderBatchPlan;
                    MaterialWFConnection matWFConnection = null;
                    if (batchPlan != null && batchPlan.MaterialWFACClassMethodID.HasValue)
                    {
                        matWFConnection = dbApp.MaterialWFConnection
                                                .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == batchPlan.MaterialWFACClassMethodID.Value
                                                        && c.ACClassWFID == contentACClassWFVB.ACClassWFID)
                                                .FirstOrDefault();
                    }
                    else
                    {
                        PartslistACClassMethod plMethod = endBatchPos.ProdOrderPartslist.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
                        if (plMethod != null)
                        {
                            matWFConnection = dbApp.MaterialWFConnection
                                                    .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == plMethod.MaterialWFACClassMethodID
                                                            && c.ACClassWFID == contentACClassWFVB.ACClassWFID)
                                                    .FirstOrDefault();
                        }
                        else
                        {
                            matWFConnection = contentACClassWFVB.MaterialWFConnection_ACClassWF
                                .Where(c => c.MaterialWFACClassMethod.MaterialWFID == endBatchPos.ProdOrderPartslist.Partslist.MaterialWFID
                                            && c.MaterialWFACClassMethod.PartslistACClassMethod_MaterialWFACClassMethod.Where(d => d.PartslistID == endBatchPos.ProdOrderPartslist.PartslistID).Any())
                                .FirstOrDefault();
                        }
                    }

                    if (matWFConnection == null)
                        return;

                    // Find intermediate position which is assigned to this Dosing-Workflownode
                    var currentProdOrderPartslist = endBatchPos.ProdOrderPartslist.FromAppContext<ProdOrderPartslist>(dbApp);
                    ProdOrderPartslistPos intermediatePosition = currentProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                        .Where(c => c.MaterialID.HasValue && c.MaterialID == matWFConnection.MaterialID
                            && c.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern
                            && !c.ParentProdOrderPartslistPosID.HasValue).FirstOrDefault();
                    if (intermediatePosition == null)
                        return;
                    intermediatePosID = intermediatePosition.ProdOrderPartslistPosID;

                    ProdOrderPartslistPos intermediateChildPos = null;
                    // Lock, if a parallel Dosing also creates a child Position for this intermediate Position
                    using (ACMonitor.Lock(pwMethodProduction._62000_PWGroupLockObj))
                    {
                        // Find intermediate child position, which is assigned to this Batch
                        intermediateChildPos = intermediatePosition.ProdOrderPartslistPos_ParentProdOrderPartslistPos
                            .Where(c => c.ProdOrderBatchID.HasValue
                                        && c.ProdOrderBatchID.Value == pwMethodProduction.CurrentProdOrderBatch.ProdOrderBatchID)
                            .FirstOrDefault();

                        if (intermediateChildPos == null)
                            return;
                        intermediateChildPosID = intermediateChildPos.ProdOrderPartslistPosID;
                    }
                }
            }
        }

        public bool ReleaseProcessModuleOnScan(PAFWorkTaskScanBase invoker)
        {
            if (invoker == null || ParentPWGroup == null)
                return false;
            PAProcessModule processModule = invoker.ParentACComponent as PAProcessModule;
            if (ParentPWGroup.TrySemaphore.ConnectionListCount > 1)
                return ParentPWGroup.ReleaseProcessModule(processModule);

            if (this.CurrentACState != ACStateEnum.SMIdle)
                this.CurrentACState = ACStateEnum.SMCompleted;
            return true;
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
