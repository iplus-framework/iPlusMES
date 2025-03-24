using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.processapplication;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static gip.core.communication.ISOonTCP.PLC;

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
        public short MaterialWFConnectionMode
        {
            get; set;
        }

        [DataMember]
        public IEnumerable<Guid>IntermediateChildPOPosIDs
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

        [DataMember]
        public PAUserTimeInfo UserTimeInfo
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public int MinIntermediateSequence
        {
            get;
            set;
        }
    }

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Register work task'}de{'Erfassung Arbeitsaufgabe'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, PWWorkTaskScanBase.PWClassName, true)]
    public abstract class PAFWorkTaskScanBase : PAProcessFunction
    {
        #region Constructors

        public const string ClassName = nameof(PAFWorkTaskScanBase);
        public const string MN_OnScanEvent = nameof(OnScanEvent);
        public const string OEEReasonPrefix = "OEEReason";

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
            result = null;
            switch(acMethodName)
            {
                case nameof(MachineMalfunction):
                    MachineMalfunction(acComponent);
                    return true;
            }

            return HandleExecuteACMethod_PAProcessFunction(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(OnScanEvent):
                    result = OnScanEvent((BarcodeSequenceBase)acParameter[0], (PAProdOrderPartslistWFInfo)acParameter[1], (Guid)acParameter[2], (int)acParameter[3], (short?)acParameter[4], (PAProdOrderPartslistWFInfo)acParameter[5], acParameter[6] as bool?, acParameter[7] as Guid?);
                    return true;
                case nameof(GetOrderInfos):
                    result = GetOrderInfos();
                    return true;
                case nameof(MalfunctionOnOff):
                    MalfunctionOnOff(acParameter[0] as Guid?);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }
        #endregion


        #region public
        [ACMethodInfo("OnScanEvent", "en{'OnScanEvent'}de{'OnScanEvent'}", 503)]
        public virtual WorkTaskScanResult OnScanEvent(BarcodeSequenceBase sequence, PAProdOrderPartslistWFInfo selectedPOLWf, Guid facilityChargeID, int scanSequence, 
                                                      short? sQuestionResult, PAProdOrderPartslistWFInfo lastInfo, bool? malfunction, Guid? oeeReason)
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
                IPAOEEProvider oeeProvider = parentPM as IPAOEEProvider;

                if (malfunction.Value)
                {
                    if (oeeProvider != null)
                        oeeProvider.OEEReason = oeeReason;

                    Malfunction.ValueT = PANotifyState.AlarmOrFault;
                    Pause();
                    scanResult.Result.Message = new Msg();
                    scanResult.Result.State = BarcodeSequenceBase.ActionState.Completed;
                    return scanResult;
                }
                else
                {
                    if (oeeProvider != null)
                        oeeProvider.OEEReason = null;

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
                    if (lastInfo != null && lastInfo.WFMethod != null)
                    {
                        scanResult = OnChangingACMethodOnScan(pwNode, releaseOrderInfo, sequence, selectedPOLWf, facilityChargeID, scanSequence, sQuestionResult, lastInfo.WFMethod);
                        if (scanResult != null && scanResult.Result.State == BarcodeSequenceBase.ActionState.Completed)
                            scanResult = OnChangingProgramLogTime(pwNode, releaseOrderInfo, sequence, selectedPOLWf, facilityChargeID, scanSequence, sQuestionResult, lastInfo.UserTimeInfo);
                    }
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

        [ACMethodInfo("", "en{'Malfunction on/off'}de{'Störung ein/aus'}", 505)]
        public void MalfunctionOnOff(Guid? malfunctionReason = null)
        {
            if (this.CurrentACState != ACStateEnum.SMIdle)
            {
                if (Malfunction.ValueT == PANotifyState.Off)
                {
                    if (malfunctionReason.HasValue)
                    {
                        IPAOEEProvider oeeProvider = ParentACComponent as IPAOEEProvider;
                        if (oeeProvider != null)
                            oeeProvider.OEEReason = malfunctionReason;
                    }

                    Malfunction.ValueT = PANotifyState.AlarmOrFault;
                    Pause();
                }
                else
                {
                    IPAOEEProvider oeeProvider = ParentACComponent as IPAOEEProvider;
                    if (oeeProvider != null)
                        oeeProvider.OEEReason = null;

                    Malfunction.ValueT = PANotifyState.Off;
                    AcknowledgeAlarms();
                    Resume();
                }
            }
        }
        

        [ACMethodInteractionClient("", "en{'Malfunction on/off'}de{'Störung allgemein ein/aus'}", 9999,true)]
        public static void MachineMalfunction(IACComponent acComponent)
        {
            ACComponent accomp = acComponent as ACComponent;
            if (accomp == null)
                return;

            Guid? msgID = null;
            bool inPause = false;

            var prop = accomp.GetProperty(nameof(ACState));
            if (prop != null)
            {
                ACStateEnum? stateEnum = prop.Value as ACStateEnum?;
                if (stateEnum.HasValue && stateEnum.Value == ACStateEnum.SMPaused)
                {
                    inPause = true;
                }
            }

            if (!inPause)
            {
                PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(acComponent.Root as ACComponent);

                if (service != null)
                {
                    core.datamodel.ACClass compClass = accomp.ComponentClass;

                    if (compClass != null)
                    {
                        string acCaption = "OEE reason";
                        string buttonACCaption = "Ok";
                        string header = "Malfunction";

                        var oeeReason = compClass.GetText("OEEReason");
                        if (oeeReason != null)
                            acCaption = oeeReason.ACCaption;

                        var oeeReasonButton = compClass.GetText("OEEReasonButton");
                        if (oeeReasonButton != null)
                            buttonACCaption = oeeReasonButton.ACCaption;

                        var oeeReasonHeader = compClass.GetText("OEEReasonHeader");
                        if (oeeReasonHeader != null)
                            header = oeeReasonHeader.ACCaption;

                        var messages = compClass.Messages.Where(c => c.ACIdentifier.StartsWith(OEEReasonPrefix)).ToList();

                        core.datamodel.ACClassMessage msg = service.ShowACClassMessageDialog(acComponent, messages, acCaption, buttonACCaption, header) as core.datamodel.ACClassMessage;

                        if (msg != null)
                            msgID = msg.ACClassMessageID;
                    }
                }
            }

            accomp.ExecuteMethod(nameof(MalfunctionOnOff), msgID);
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

            List<PAProdOrderPartslistWFInfo> resultList = new List<PAProdOrderPartslistWFInfo>();

            foreach (PAProdOrderPartslistWFInfo info in infoList.OrderBy(c => c.MinIntermediateSequence))
            {
                if (resultList.Any(x => x.POPPosId == info.POPPosId))
                    continue;

                resultList.Add(info);
            }

            return resultList;
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
            short connMode = 0;
            IEnumerable<Guid> intermediateChildPosIDs;
            int minIntermediateSequence = 0;

            pwNode.GetAssignedIntermediate(out intermediatePosID, out intermediateChildPosID, out connMode, out intermediateChildPosIDs, out minIntermediateSequence);
            infoList.Add(OnCreateNewWFInfo(infoList, activeWorkflow, pwNode, forRelease, intermediatePosID, intermediateChildPosID, connMode, intermediateChildPosIDs, minIntermediateSequence)); 
        }

        protected virtual PAProdOrderPartslistWFInfo OnCreateNewWFInfo(List<PAProdOrderPartslistWFInfo> infoList, PWMethodProduction activeWorkflow, PWWorkTaskScanBase pwNode, bool forRelease, 
                                                                       Guid intermediatePosID, Guid intermediateChildPosID, short materialWFConnectionMode, IEnumerable<Guid> intermediateChildPosIDs, int minIntermediateSequence)
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
                WFMethod = pwNode.CurrentACMethod.ValueT,
                UserTimeInfo = GetUserTimeInfo(pwNode),
                MaterialWFConnectionMode = materialWFConnectionMode,
                IntermediateChildPOPosIDs = intermediateChildPosIDs,
                MinIntermediateSequence = minIntermediateSequence
            };
        }

        protected virtual PAUserTimeInfo GetUserTimeInfo(PWWorkTaskScanBase pwNode)
        {
            if (pwNode == null || pwNode.CurrentProgramLog == null)
                return null;

            core.datamodel.ACProgramLog programLog = pwNode.CurrentProgramLog.ACProgramLog_ParentACProgramLog.Where(c => c.ACUrl == this.ACUrl).FirstOrDefault();
            if (programLog != null)
            {
                return new PAUserTimeInfo() { StartDate = programLog.StartDateDST, EndDate = programLog.EndDateDST };
            }
            else
            {
                return new PAUserTimeInfo() { StartDate = pwNode.CurrentProgramLog.StartDateDST, EndDate = pwNode.CurrentProgramLog.EndDateDST };
            }
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

        protected virtual WorkTaskScanResult OnChangingProgramLogTime(PWWorkTaskScanBase pwNode, PAProdOrderPartslistWFInfo releaseOrderInfo, BarcodeSequenceBase sequence, PAProdOrderPartslistWFInfo selectedPOLWf, Guid facilityChargeID, int scanSequence, short? sQuestionResult, PAUserTimeInfo userTime)
        {
            WorkTaskScanResult scanResult = new WorkTaskScanResult();

            core.datamodel.ACProgramLog programLogPWNode = pwNode.CurrentProgramLog;
            core.datamodel.ACProgramLog programLogPAF = programLogPWNode.ACProgramLog_ParentACProgramLog.Where(c => c.ACUrl == this.ACUrl).FirstOrDefault();
            PWGroup pwGroup = pwNode.ParentPWGroup;
            core.datamodel.ACProgramLog programLogGroup = pwGroup?.CurrentProgramLog;

            PAProcessModule processModule = FindParentComponent<PAProcessModule>(c => c is PAProcessModule);
            Guid[] programLogs = null;
            if (processModule != null)
            {
                programLogs = processModule.CurrentProgramLogs.Select(c => c.ACProgramLogID).ToArray();
            }

            if (userTime != null && programLogs != null)
            {
                if (userTime.UserStartDate.HasValue && userTime.StartDate != userTime.UserStartDate)
                {
                    if (programLogGroup != null)
                    {
                        using (Database db = new core.datamodel.Database())
                        {
                            string inOperationEnum = AvailabilityState.InOperation.ToString();
                            string idleEnum = AvailabilityState.Idle.ToString();
                            string standbyEnum = AvailabilityState.Standby.ToString();

                            var propertyLogs = db.ACProgramLogPropertyLog.Include(c => c.ACPropertyLog.ACClassProperty)
                                                                         .Include(c => c.ACPropertyLog.ACClass)
                                                                         .GroupJoin(db.ACProgramLog,
                                                                                    propLog => propLog.ACProgramLogID,
                                                                                    programLog => programLog.ACProgramLogID,
                                                                                    (propLog, programLog) => new { propLog, programLog })
                                                                         .Where(c => c.programLog.Any(x => programLogs.Contains(x.ACProgramLogID)))
                                                                         .AsEnumerable()
                                                                         .GroupBy(c => c.propLog.ACPropertyLog)
                                                                         .ToArray()
                                                                         .OrderBy(c => c.Key.EventTime);

                            var propLogStandby = propertyLogs.Where(c => c.Key.Value == standbyEnum).FirstOrDefault();
                            var propLogOperation = propertyLogs.Where(c => c.Key.Value == inOperationEnum).FirstOrDefault();

                            TimeSpan diff = TimeSpan.Zero;
                            
                            if (propLogStandby != null && propLogOperation != null)
                                diff = propLogStandby.Key.EventTime - propLogOperation.Key.EventTime;

                            if (propLogOperation != null)
                            {
                                DateTime? minDate = propLogOperation.SelectMany(c => c.programLog).Where(c => c.ACProgramLogID != programLogGroup.ACProgramLogID).Min(c => c.StartDate);

                                if (minDate.HasValue && minDate.Value > propLogOperation.Key.EventTime)
                                {
                                    propLogOperation.Key.EventTime = minDate.Value;
                                    if (propLogStandby != null)
                                        propLogStandby.Key.EventTime = minDate.Value.Add(diff);
                                }
                                else
                                {
                                    DateTime? lastEventIdle = db.ACPropertyLog.Where(c => c.ACClassID == propLogOperation.Key.ACClassID
                                                                                       && c.ACClassPropertyID == propLogOperation.Key.ACClassPropertyID
                                                                                       && c.Value == idleEnum
                                                                                       && c.EventTime < propLogOperation.Key.EventTime)
                                                                              .Max(c => c.EventTime);

                                    if (lastEventIdle.HasValue && lastEventIdle.Value > userTime.UserStartDate.Value)
                                    {
                                        //Error50704: The previous activity on the machine was: {0}. The start cannot be earlier than that time!
                                        scanResult.Result.Message = new Msg(this, eMsgLevel.Error, nameof(PAFWorkTaskScanBase), nameof(OnChangingProgramLogTime), 597, "Error50704", lastEventIdle.Value.ToString("dd.M. HH:mm"));
                                        scanResult.Result.State = BarcodeSequenceBase.ActionState.Cancelled;
                                        return scanResult;
                                    }

                                    var itemsForNextEvent = db.ACPropertyLog.Where(c => c.ACClassID == propLogOperation.Key.ACClassID
                                                                                     && c.ACClassPropertyID == propLogOperation.Key.ACClassPropertyID
                                                                                     && c.Value != inOperationEnum
                                                                                     && c.EventTime > propLogOperation.Key.EventTime)
                                                                            .ToArray();

                                    if (itemsForNextEvent != null && itemsForNextEvent.Any())
                                    {
                                        DateTime? nextEvent = itemsForNextEvent.Min(c => c.EventTime);

                                        if (nextEvent.HasValue && nextEvent.Value < userTime.UserStartDate.Value)
                                        {
                                            // The next activity on the machine was: {0}. The start cannot be later than that time!
                                            scanResult.Result.Message = new Msg(this, eMsgLevel.Error, nameof(PAFWorkTaskScanBase), nameof(OnChangingProgramLogTime), 597, "Error50705", nextEvent.Value.ToString("dd.M. HH:mm"));
                                            scanResult.Result.State = BarcodeSequenceBase.ActionState.Cancelled;
                                            return scanResult;
                                        }
                                    }

                                    propLogOperation.Key.EventTime = userTime.UserStartDate.Value;
                                    if (propLogStandby != null)
                                        propLogStandby.Key.EventTime = userTime.UserStartDate.Value.Add(diff);
                                }
                            }

                            db.ACSaveChanges();
                        }

                        if (TimeInfo.ValueT != null)
                        {
                            //DateTime? endDate = userTime.UserEndDate;
                            //if (!endDate.HasValue)
                            //    endDate = DateTime.Now;

                            //TimeSpan duration = endDate.Value - userTime.UserStartDate.Value;
                            //pwGroup.TimeInfo.ValueT.ActualTimes.Duration = duration;
                            pwGroup.TimeInfo.ValueT.ActualTimes.ResetEnd();
                            pwGroup.TimeInfo.ValueT.ActualTimes.StartTime = userTime.UserStartDate.Value;
                        }
                    }

                    if (programLogPAF != null)
                        programLogPAF.StartDate = userTime.UserStartDate;
                }

                if (userTime.UserEndDate.HasValue)
                {
                    if (programLogGroup != null)
                    {
                        using (Database db = new core.datamodel.Database())
                        {
                            string inOperationEnum = AvailabilityState.InOperation.ToString();
                            string idleEnum = AvailabilityState.Idle.ToString();
                            string standbyEnum = AvailabilityState.Standby.ToString();

                            var propertyLog = db.ACProgramLogPropertyLog.Include(c => c.ACPropertyLog.ACClassProperty)
                                                                         .Include(c => c.ACPropertyLog.ACClass)
                                                                         .GroupJoin(db.ACProgramLog,
                                                                                    propLog => propLog.ACProgramLogID,
                                                                                    programLog => programLog.ACProgramLogID,
                                                                                    (propLog, programLog) => new { propLog, programLog })
                                                                         .Where(c => c.programLog.Any(x => programLogs.Contains(x.ACProgramLogID)))
                                                                         .OrderByDescending(c => c.propLog.ACPropertyLog.EventTime)
                                                                         .FirstOrDefault();

                            if (propertyLog != null)
                            {
                                if (userTime.UserEndDate.Value < propertyLog.propLog.ACPropertyLog.EventTime)
                                {
                                    //Error50706: The previous activity on the machine was: {0}. The end cannot be earlier than that time!
                                    scanResult.Result.Message = new Msg(this, eMsgLevel.Error, nameof(PAFWorkTaskScanBase), nameof(OnChangingProgramLogTime), 665, "Error50706", propertyLog.propLog.ACPropertyLog.EventTime.ToString("dd.M. HH:mm"));
                                    scanResult.Result.State = BarcodeSequenceBase.ActionState.Cancelled;
                                    return scanResult;
                                }
                            }
                        }

                        if (TimeInfo.ValueT != null)
                        {
                            DateTime? startDate = userTime.UserStartDate;
                            if (!startDate.HasValue)
                                startDate = userTime.StartDate;
                            if (!startDate.HasValue)
                                startDate = pwGroup.TimeInfo.ValueT.ActualTimes.StartTimeValue;
                            if (!startDate.HasValue)
                                startDate = DateTime.Now;

                            TimeSpan duration = userTime.UserEndDate.Value - startDate.Value;
                            pwGroup.TimeInfo.ValueT.ActualTimes.Duration = duration;
                            pwGroup.TimeInfo.ValueT.ActualTimes.EndTime = userTime.UserEndDate.Value;
                        }
                    }

                    if (programLogPAF != null)
                        programLogPAF.EndDate = userTime.UserEndDate;
                }
            }

            Msg wfMsg = null;
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
