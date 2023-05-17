using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    [ACSerializeableInfo]
    [DataContract]
    public class WorkTaskScanResult
    {
        public WorkTaskScanResult()
        {
            Result = new BarcodeSequenceBase();
        }

        [DataMember]
        public BarcodeSequenceBase Result
        {
            get; set;
        }

        [DataMember]
        public PAProdOrderPartslistWFInfo[] OrderInfos
        {
            get; set;
        }
    }

    [ACSerializeableInfo]
    [DataContract]
    public class PAProdOrderPartslistWFInfo
    {
        [DataMember]
        public Guid POPId
        {
            get; set;
        }

        [DataMember]
        public Guid POPPosId
        {
            get; set;
        }

        [DataMember]
        public Guid IntermPOPPosId
        {
            get; set;
        }

        [DataMember]
        public Guid IntermChildPOPPosId
        {
            get; set;
        }

        [DataMember]
        public Guid ACClassWFId
        {
            get; set;
        }

        [DataMember]
        public string ACUrlWF
        {
            get; set;
        }

        [DataMember]
        public bool ForRelease
        {
            get; set;
        }

        [DataMember]
        public bool Pause
        {
            get; set;
        }

        [DataMember(Name="WFMSD")]
        public DateTime? WFMethodStartDate
        {
            get;
            set;
        }

        [DataMember]
        public ACMethod WFMethod
        {
            get; set;
        }
    }

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Register work task'}de{'Erfassung Arbeitsaufgabe'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, PWWorkTaskScanBase.PWClassName, true)]
    public abstract class PAFWorkTaskScanBase : PAProcessFunction
    {
        #region Constructors

        public const string ClassName = nameof(PAFWorkTaskScanBase);
        public const string MN_OnScanEvent = nameof(OnScanEvent);

        static PAFWorkTaskScanBase()
        {
            RegisterExecuteHandler(typeof(PAFWorkTaskScanBase), HandleExecuteACMethod_PAFWorkTaskScanBase);
        }

        public PAFWorkTaskScanBase(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        #endregion

        #region Methods

        #region Abstract
        protected abstract bool PWWorkTaskScanSelector(IACComponent c);
        protected abstract bool PWWorkTaskScanDeSelector(IACComponent c);
        #endregion


        #region HandleExceute
        public static bool HandleExecuteACMethod_PAFWorkTaskScanBase(out object result, IACComponent acComponent, string acMethodName, core.datamodel.ACClassMethod acClassMethod, object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessFunction(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(OnScanEvent):
                    result = OnScanEvent((BarcodeSequenceBase)acParameter[0], (PAProdOrderPartslistWFInfo)acParameter[1], (Guid)acParameter[2], (int)acParameter[3], (short?)acParameter[4], acParameter[5] as ACMethod, acParameter[6] as bool?);
                    return true;
                case nameof(GetOrderInfos):
                    result = GetOrderInfos();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }
        #endregion


        #region public
        [ACMethodInfo("OnScanEvent", "en{'OnScanEvent'}de{'OnScanEvent'}", 503)]
        public virtual WorkTaskScanResult OnScanEvent(BarcodeSequenceBase sequence, PAProdOrderPartslistWFInfo selectedPOLWf, Guid facilityChargeID, int scanSequence, 
                                                      short? sQuestionResult, ACMethod acMethod, bool? malfunction)
        {
            WorkTaskScanResult scanResult = new WorkTaskScanResult();

            PAProcessModule parentPM = ParentACComponent as PAProcessModule;
            if (parentPM == null)
            {
                // Error50367: The application tree is incorrect. The parent component must be a process module! (Der Anwendungsbaum ist falsch. Die Elternkomponente muss ein Prozessmodul sein!)
                scanResult.Result.Message = new Msg(this, eMsgLevel.Error, ClassName, "OnScanEvent(10)", 10, "Error50367");
                scanResult.Result.State = BarcodeSequenceBase.ActionState.Cancelled;
                return scanResult;
            }

            if (malfunction.HasValue && this.CurrentACState != ACStateEnum.SMIdle)
            {
                if (malfunction.Value)
                {
                    Malfunction.ValueT = PANotifyState.AlarmOrFault;
                    Pause();
                    scanResult.Result.Message = new Msg();
                    scanResult.Result.State = BarcodeSequenceBase.ActionState.Completed;
                    return scanResult;
                }
                else
                {
                    Malfunction.ValueT = PANotifyState.Off;
                    AcknowledgeAlarms();
                    Resume();
                    scanResult.Result.Message = new Msg();
                    scanResult.Result.State = BarcodeSequenceBase.ActionState.Completed;
                    return scanResult;
                }
            }

            if (scanSequence == 1)
            {
                List<PAProdOrderPartslistWFInfo> ordersForOccupation = GetWaitingProdOrderPartslistWFInfo();
                List <PAProdOrderPartslistWFInfo> ordersForRelease = GetActivatedProdOrderPartslistWFInfo();
                if (ordersForOccupation.Any() && ordersForRelease.Any())
                {
                    foreach (PAProdOrderPartslistWFInfo info in ordersForRelease)
                    {
                        ordersForOccupation.RemoveAll(c => c.ACClassWFId == info.ACClassWFId && c.POPPosId == info.POPPosId);
                    }
                }
                

                int releaseableOrderCount = ordersForRelease != null ? ordersForRelease.Count() : 0;
                int occupyableOrderCount = ordersForOccupation != null ? ordersForOccupation.Count() : 0;

                List<PAProdOrderPartslistWFInfo> orderInfoList = new List<PAProdOrderPartslistWFInfo>();
                if (releaseableOrderCount > 0)
                    orderInfoList.AddRange(ordersForRelease);
                if (occupyableOrderCount > 0)
                    orderInfoList.AddRange(ordersForOccupation);
                scanResult.OrderInfos = orderInfoList.ToArray();

                if (releaseableOrderCount == 0 && occupyableOrderCount == 0)
                {
                    // Info50053: There are no waiting orders that can be registered at this machine.
                    // (Es gibt keine wartenden Aufträge die an dieser Maschine angemeldet werden können.)
                    scanResult.Result.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnScanEvent(20)", 20, "Info50053");
                    scanResult.Result.State = BarcodeSequenceBase.ActionState.Cancelled;
                }
                else if (occupyableOrderCount > 0 && releaseableOrderCount == 0)
                {
                    // Info50054: Select an order you want to REGISTER on the machine.
                    // (Wählen Sie einen Auftrag aus, den sie an der Maschine ANMELDEN wollen.)
                    scanResult.Result.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnScanEvent(30)", 30, "Info50054");
                    scanResult.Result.State = BarcodeSequenceBase.ActionState.Selection;
                }
                else if (occupyableOrderCount > 0 && (releaseableOrderCount > 0 && parentPM.OnGetSemaphoreCapacity() == 0))
                {
                    // Info50055: Wählen Sie einen Auftrag aus, den sie entweder an der Maschine ABMELDEN, einen weiteren ANMELDEN oder eine Material-BUCHUNG durchführen wollen. 
                    // Select an order you want to either DEREGISTER OR REGISTER on the machine or you want to make a MATERIAL POSTING.
                    scanResult.Result.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnScanEvent(40)", 40, "Info50055");
                    scanResult.Result.State = BarcodeSequenceBase.ActionState.Selection;
                }
                else //if (occupyableOrderCount == 0)
                {
                    // Info50056: Wählen Sie einen Auftrag aus, den sie entweder an der Maschine ABMELDEN oder eine Material-BUCHUNG durchführen wollen. 
                    // Select an order you want to either DEREGISTER on the machine or you want to make a MATERIAL POSTING.
                    scanResult.Result.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnScanEvent(50)", 50, "Info50056");
                    scanResult.Result.State = BarcodeSequenceBase.ActionState.Selection;
                }
            }
            else // if (scanSequence == 2)
            {
                if (selectedPOLWf == null)
                {
                    // Error50368: No order selected! (Kein Auftrag ausgewählt!)
                    scanResult.Result.Message = new Msg(this, eMsgLevel.Error, ClassName, "OnScanEvent(60)", 60, "Error50368");
                    scanResult.Result.State = BarcodeSequenceBase.ActionState.Cancelled;
                    return scanResult;
                }

                PAProdOrderPartslistWFInfo releaseOrderInfo = null;
                PAProdOrderPartslistWFInfo occupyOrderInfo = null;
                PWWorkTaskScanBase pwNode = null;
                if (selectedPOLWf != null)
                {
                    if (selectedPOLWf.ForRelease)
                    {
                        List<PAProdOrderPartslistWFInfo> ordersForRelease = GetActivatedProdOrderPartslistWFInfo();
                        if (ordersForRelease != null && ordersForRelease.Any())
                            releaseOrderInfo = ordersForRelease.Where(c => c.POPId == selectedPOLWf.POPId).FirstOrDefault();
                    }
                    else
                    {
                        List<PAProdOrderPartslistWFInfo> ordersForOccupation = GetWaitingProdOrderPartslistWFInfo();
                        if (ordersForOccupation != null && ordersForOccupation.Any())
                            occupyOrderInfo = ordersForOccupation.Where(c => c.POPId == selectedPOLWf.POPId).FirstOrDefault();
                    }
                }

                if (occupyOrderInfo == null && releaseOrderInfo == null)
                {
                    // Error50369: The selected order isn't active anymore! (Der ausgewählte Auftrag ist nicht mehr aktiv!)
                    scanResult.Result.Message = new Msg(this, eMsgLevel.Error, ClassName, "OnScanEvent(70)", 70, "Error50369");
                    scanResult.Result.State = BarcodeSequenceBase.ActionState.Cancelled;
                    return scanResult;
                }

                if (releaseOrderInfo != null)
                    pwNode = ACUrlCommand(releaseOrderInfo.ACUrlWF) as PWWorkTaskScanBase;
                else
                    pwNode = ACUrlCommand(occupyOrderInfo.ACUrlWF) as PWWorkTaskScanBase;
                if (pwNode == null || pwNode.ParentPWGroup == null)
                {
                    // Error50369: The selected order isn't active anymore! (Der ausgewählte Auftrag ist nicht mehr aktiv!)
                    scanResult.Result.Message = new Msg(this, eMsgLevel.Error, ClassName, "OnScanEvent(80)", 80, "Error50369");
                    scanResult.Result.State = BarcodeSequenceBase.ActionState.Cancelled;
                    return scanResult;
                }

                if (releaseOrderInfo != null)
                {
                    if (acMethod != null)
                        scanResult = OnChangingACMethodOnScan(pwNode, releaseOrderInfo, sequence, selectedPOLWf, facilityChargeID, scanSequence, sQuestionResult, acMethod);
                    else
                        scanResult = OnReleasingProcessModuleOnScan(pwNode, releaseOrderInfo, sequence, selectedPOLWf, facilityChargeID, scanSequence, sQuestionResult);
                }
                else
                {
                    scanResult = OnOccupyingProcessModuleOnScan(parentPM, pwNode, releaseOrderInfo, sequence, selectedPOLWf, facilityChargeID, scanSequence, sQuestionResult);
                }
            }
            return scanResult;
        }

        [ACMethodInfo("", "en{'GetOrderInfos'}de{'GetOrderInfos'}", 504)]
        public virtual WorkTaskScanResult GetOrderInfos()
        {
            List<PAProdOrderPartslistWFInfo> ordersForOccupation = GetWaitingProdOrderPartslistWFInfo();
            List<PAProdOrderPartslistWFInfo> ordersForRelease = GetActivatedProdOrderPartslistWFInfo();
            if (ordersForOccupation.Any() && ordersForRelease.Any())
            {
                foreach (PAProdOrderPartslistWFInfo info in ordersForRelease)
                {
                    ordersForOccupation.RemoveAll(c => c.ACClassWFId == info.ACClassWFId && c.POPPosId == info.POPPosId);
                }
            }

            int releaseableOrderCount = ordersForRelease != null ? ordersForRelease.Count() : 0;
            int occupyableOrderCount = ordersForOccupation != null ? ordersForOccupation.Count() : 0;

            List<PAProdOrderPartslistWFInfo> orderInfoList = new List<PAProdOrderPartslistWFInfo>();
            if (releaseableOrderCount > 0)
                orderInfoList.AddRange(ordersForRelease);
            if (occupyableOrderCount > 0)
                orderInfoList.AddRange(ordersForOccupation);

            return new WorkTaskScanResult() { OrderInfos = orderInfoList.ToArray() };
        }

        [ACMethodInfo("", "en{'OccupyReleaseProcessModule'}de{'OccupyReleaseProcessModule'}", 504)]
        public Msg OccupyReleaseProcessModule(string wfACUrl, bool forRelease)
        {
            PAProcessModule parentPM = ParentACComponent as PAProcessModule;
            if (parentPM == null)
            {
                // Error50367: The application tree is incorrect. The parent component must be a process module! (Der Anwendungsbaum ist falsch. Die Elternkomponente muss ein Prozessmodul sein!)
                return new Msg(this, eMsgLevel.Error, ClassName, nameof(OccupyReleaseProcessModule) + "(10)", 10, "Error50367");
            }

            PWWorkTaskScanBase pwNode  = ACUrlCommand(wfACUrl) as PWWorkTaskScanBase;

            if (pwNode == null || pwNode.ParentPWGroup == null)
            {
                // Error50369: The selected order isn't active anymore! (Der ausgewählte Auftrag ist nicht mehr aktiv!)
                return new Msg(this, eMsgLevel.Error, ClassName, nameof(OccupyReleaseProcessModule) + "(20)", 80, "Error50369");
            }

            WorkTaskScanResult taskScanResult = null;
            if (forRelease)
                taskScanResult = OnReleasingProcessModuleOnScan(pwNode, null, null, null, Guid.Empty, 0, null);
            else
                taskScanResult = OnOccupyingProcessModuleOnScan(parentPM, pwNode, null, null, null, Guid.Empty, 0, null);
            return taskScanResult.Result.Message;
        }

        #endregion


        #region Private
        protected List<PAProdOrderPartslistWFInfo> GetWaitingProdOrderPartslistWFInfo()
        {
            PAProcessModule parentPM = ParentACComponent as PAProcessModule;
            if (parentPM == null || ApplicationManager == null)
                return new List<PAProdOrderPartslistWFInfo>();
            List<PWMethodProduction> loadedWorkflows = ApplicationManager.FindChildComponents<PWMethodProduction>(c => c is PWMethodProduction, c => !(c is PWMethodProduction), 2);
            if (!loadedWorkflows.Any())
                return new List<PAProdOrderPartslistWFInfo>();
            List<PWWorkTaskScanBase> waitingNodes = new List<PWWorkTaskScanBase>();
            foreach (PWMethodProduction pwFunc in loadedWorkflows)
            {
                waitingNodes.AddRange(pwFunc.FindChildComponents<PWWorkTaskScanBase>(c => PWWorkTaskScanSelector(c), c => PWWorkTaskScanDeSelector(c))
                                            .Where(c => c.ParentPWGroup != null
                                                        && c.ParentPWGroup.CurrentACState != ACStateEnum.SMIdle
                                                        && c.ParentPWGroup.ProcessModuleList.Contains(parentPM))); // Check Routing-Rules if this PM can be occupied
            }
            if (!waitingNodes.Any())
                return new List<PAProdOrderPartslistWFInfo>();

            List<PAProdOrderPartslistWFInfo> infoList = new List<PAProdOrderPartslistWFInfo>();
            foreach (PWWorkTaskScanBase pwNode in waitingNodes)
            {
                AddInfoNodeToList(infoList,
                                    pwNode.ParentPWMethod<PWMethodProduction>(),
                                    pwNode,
                                    false);
            }
            return infoList;
        }

        protected List<PAProdOrderPartslistWFInfo> GetActivatedProdOrderPartslistWFInfo()
        {
            PAProcessModule parentPM = ParentACComponent as PAProcessModule;
            if (parentPM == null || parentPM.Semaphore.ConnectionListCount <= 0)
                return new List<PAProdOrderPartslistWFInfo>();
            List<PAProdOrderPartslistWFInfo> infoList = new List<PAProdOrderPartslistWFInfo>();
            foreach (var wrapObject in parentPM.Semaphore.ConnectionList)
            {
                PWGroupVB pwGroup = wrapObject.ValueT as PWGroupVB;
                if (pwGroup != null)
                {
                    AddInfoNodeToList(infoList,
                        pwGroup.ParentPWMethod<PWMethodProduction>(),
                        pwGroup.FindChildComponents<PWWorkTaskScanBase>(c => c is PWWorkTaskScanBase && (c as PWWorkTaskScanBase).IsTargetFunction(this)).FirstOrDefault(),
                        true);
                }
            }
            return infoList;
        }

        private void AddInfoNodeToList(List<PAProdOrderPartslistWFInfo> infoList, PWMethodProduction activeWorkflow, PWWorkTaskScanBase pwNode, bool forRelease)
        {
            if (pwNode == null || activeWorkflow == null || activeWorkflow.CurrentProdOrderPartslistPos == null)
                return;
            Guid intermediatePosID, intermediateChildPosID;
            pwNode.GetAssignedIntermediate(out intermediatePosID, out intermediateChildPosID);
            infoList.Add(OnCreateNewWFInfo(infoList, activeWorkflow, pwNode, forRelease, intermediatePosID, intermediateChildPosID)); 
        }

        protected virtual PAProdOrderPartslistWFInfo OnCreateNewWFInfo(List<PAProdOrderPartslistWFInfo> infoList, PWMethodProduction activeWorkflow, PWWorkTaskScanBase pwNode, bool forRelease, Guid intermediatePosID, Guid intermediateChildPosID)
        {
            return new PAProdOrderPartslistWFInfo()
            {
                POPId = activeWorkflow.CurrentProdOrderPartslistPos.ProdOrderPartslistID,
                POPPosId = activeWorkflow.CurrentProdOrderPartslistPos.ProdOrderPartslistPosID,
                IntermPOPPosId = intermediatePosID,
                IntermChildPOPPosId = intermediateChildPosID,
                ACClassWFId = pwNode.ContentACClassWF != null ? pwNode.ContentACClassWF.ACClassWFID : Guid.Empty,
                ACUrlWF = pwNode.GetACUrl(),
                ForRelease = forRelease,
                WFMethodStartDate = activeWorkflow.TimeInfo?.ValueT?.ActualTimes?.StartTime,
                WFMethod = pwNode.CurrentACMethod.ValueT
            };
        }

        protected virtual WorkTaskScanResult OnChangingACMethodOnScan(PWWorkTaskScanBase pwNode, PAProdOrderPartslistWFInfo releaseOrderInfo, BarcodeSequenceBase sequence, PAProdOrderPartslistWFInfo selectedPOLWf, Guid facilityChargeID, int scanSequence, short? sQuestionResult, ACMethod acMethod)
        {
            WorkTaskScanResult scanResult = new WorkTaskScanResult();
            Msg wfMsg = pwNode.ChangeReceivedParams(this, acMethod);
            if (wfMsg == null || wfMsg.MessageLevel < eMsgLevel.Failure)
            {
                // Info50057: The order has been deregistered on the machine.
                // Der Auftrag wurde an der Maschine abgemeldet.
                scanResult.Result.Message = wfMsg != null ? wfMsg : new Msg("OK", this, eMsgLevel.Info, ClassName, "OnScanEvent(80)", 80);
                scanResult.Result.State = BarcodeSequenceBase.ActionState.Completed;
            }
            else
            {
                scanResult.Result.Message = wfMsg;
                scanResult.Result.State = BarcodeSequenceBase.ActionState.Cancelled;
            }
            return scanResult;
        }


        protected virtual WorkTaskScanResult OnReleasingProcessModuleOnScan(PWWorkTaskScanBase pwNode, PAProdOrderPartslistWFInfo releaseOrderInfo, BarcodeSequenceBase sequence, PAProdOrderPartslistWFInfo selectedPOLWf, Guid facilityChargeID, int scanSequence, short? sQuestionResult)
        {
            Msg actionMsg = null;
            WorkTaskScanResult scanResult = new WorkTaskScanResult();
            Msg wfMsg = pwNode.OnGetMessageOnReleasingProcessModule(this, selectedPOLWf.Pause);
            if (wfMsg == null || wfMsg.MessageLevel < eMsgLevel.Failure)
            {
                if (pwNode.ReleaseProcessModuleOnScan(this, selectedPOLWf.Pause))
                {
                    // Info50057: The order has been deregistered on the machine.
                    // Der Auftrag wurde an der Maschine abgemeldet.
                    actionMsg = new Msg(this, eMsgLevel.Info, ClassName, "OnScanEvent(90)", 90, "Info50057");
                    scanResult.Result.State = BarcodeSequenceBase.ActionState.Completed;
                }
                else
                {
                    // Error50370: The order couldn't be deregistered on this machine.
                    // Der Auftrag konnte an der Maschine nicht abgemeldet werden!
                    actionMsg = new Msg(this, eMsgLevel.Error, ClassName, "OnScanEvent(100)", 100, "Error50370");
                    scanResult.Result.State = BarcodeSequenceBase.ActionState.Cancelled;
                }
            }
            else if (wfMsg != null && wfMsg.MessageLevel == eMsgLevel.Question)
            {
                actionMsg = wfMsg;
                scanResult.Result.State = BarcodeSequenceBase.ActionState.Question;
            }
            else
            {
                actionMsg = wfMsg;
                scanResult.Result.State = BarcodeSequenceBase.ActionState.Cancelled;
            }
            if (wfMsg != null && actionMsg != wfMsg)
                actionMsg.Message += System.Environment.NewLine + wfMsg.Message;
            scanResult.Result.Message = actionMsg;
            return scanResult;
        }

        protected virtual WorkTaskScanResult OnOccupyingProcessModuleOnScan(PAProcessModule parentPM, PWWorkTaskScanBase pwNode, PAProdOrderPartslistWFInfo releaseOrderInfo, BarcodeSequenceBase sequence, PAProdOrderPartslistWFInfo selectedPOLWf, Guid facilityChargeID, int scanSequence, short? sQuestionResult)
        {
            WorkTaskScanResult scanResult = new WorkTaskScanResult();
            Msg wfMsg = null;
            Msg actionMsg = null;
            if (pwNode.OccupyWithPModuleOnScan(parentPM, this))
            {
                wfMsg = pwNode.OnGetMessageAfterOccupyingProcessModule(this);
                // Info50058: The order has been registered on the machine.
                // Der Auftrag wurde an der Maschine aktiviert.
                if (wfMsg == null || wfMsg.MessageLevel < eMsgLevel.Failure)
                {
                    actionMsg = new Msg(this, eMsgLevel.Info, ClassName, "OnScanEvent(110)", 110, "Info50058");
                    scanResult.Result.State = BarcodeSequenceBase.ActionState.Completed;
                }
                else
                {
                    actionMsg = wfMsg;
                    scanResult.Result.State = BarcodeSequenceBase.ActionState.Cancelled;
                }
            }
            else
            {
                // Error50371: The order couldn't be registered on this machine.
                // Der Auftrag konnte an der Maschine nicht angemeldet werden!
                actionMsg = new Msg(this, eMsgLevel.Error, ClassName, "OnScanEvent(120)", 120, "Error50371");
                scanResult.Result.State = BarcodeSequenceBase.ActionState.Cancelled;
            }
            if (wfMsg != null && actionMsg != wfMsg)
                actionMsg.Message += System.Environment.NewLine + wfMsg.Message;
            scanResult.Result.Message = actionMsg;
            return scanResult;
        }

        public override void OnOrderInfoRefreshed()
        {
            HandleACStateOfWorkTask();
        }

        protected virtual void HandleACStateOfWorkTask()
        {
            if (Root == null || !Root.Initialized)
                return;
            
            PAProcessModule parentPM = ParentACComponent as PAProcessModule;
            if (parentPM == null)
            {
                if (CurrentACState != ACStateEnum.SMIdle)
                    CurrentACState = ACStateEnum.SMIdle;
                return;
            }
            if (parentPM.Semaphore.ConnectionListCount <= 0)
            {
                if (CurrentACState != ACStateEnum.SMIdle)
                    CurrentACState = ACStateEnum.SMIdle;
                return;
            }
            else
            {
                if (CurrentACState == ACStateEnum.SMIdle)
                    CurrentACState = ACStateEnum.SMRunning;
            }
        }

        public override void SMRunning()
        {
            UnSubscribeToProjectWorkCycle();
            //base.SMRunning();
        }
        #endregion

        #endregion
    }
}
