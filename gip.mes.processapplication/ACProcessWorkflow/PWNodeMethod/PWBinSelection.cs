// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PWBinSelection'}de{'PWBinSelection'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWBinSelection : PWNodeProcessMethod
    {
        #region config
        public const string Config_LPHF = @"LPHF";
        public const string Config_LPFW = @"LPFW";
        #endregion

        #region c'tors

        public const string PWClassName = "PWBinSelection";

        public static double BinSelectionReservationQuantity = 0.1;
        public static string BinSelectionReservationComment = "[RESERVATION]";

        static PWBinSelection()
        {
            ACMethod method = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();

            method.ParameterValueList.Add(new ACValue("SourceInfoType", typeof(ManualPreparationSourceInfoTypeEnum), ManualPreparationSourceInfoTypeEnum.FacilityID, Global.ParamOption.Optional));
            paramTranslation.Add("SourceInfoType", "en{'Define source info type'}de{'Quelletyp'}");

            var wrapper = new ACMethodWrapper(method, "en{'Configuration'}de{'Konfiguration'}", typeof(PWBinSelection), paramTranslation, null);
            ACMethod.RegisterVirtualMethod(typeof(PWBinSelection), ACStateConst.SMStarting, wrapper);
        }

        public PWBinSelection(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            CurrentEndBatchPosKey = null;
            IntermediateChildPosKey = null;
            
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties

        #region Properties => Managers


        public ACProdOrderManager ProdOrderManager
        {
            get
            {
                PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                return pwMethodProduction != null ? pwMethodProduction.ProdOrderManager : null;
            }
        }


        protected FacilityManager ACFacilityManager
        {
            get
            {
                if (ParentPWMethodVBBase == null)
                    return null;
                return ParentPWMethodVBBase.ACFacilityManager as FacilityManager;
            }
        }

        #endregion

        #region Properties -> Structure
        public T ParentPWMethod<T>() where T : PWMethodVBBase
        {
            if (ParentRootWFNode == null)
                return null;
            return ParentRootWFNode as T;
        }

        public PWMethodVBBase ParentPWMethodVBBase
        {
            get
            {
                return ParentRootWFNode as PWMethodVBBase;
            }
        }
        public bool IsProduction
        {
            get
            {
                return ParentPWMethod<PWMethodProduction>() != null;
            }
        }
        #endregion

        #region Properties -> Configuration
        public ManualPreparationSourceInfoTypeEnum SourceInfoType
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("SourceInfoType");
                    if (acValue != null)
                        return (ManualPreparationSourceInfoTypeEnum)acValue.Value;
                }
                return ManualPreparationSourceInfoTypeEnum.FacilityID;
            }
        }
        #endregion

        #region Properties -> Pos

        private EntityKey _CurrentEndBatchPosKey;
        [ACPropertyInfo(999)]
        public EntityKey CurrentEndBatchPosKey
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _CurrentEndBatchPosKey;
                }
            }
            set
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _CurrentEndBatchPosKey = value;
                }
            }
        }

        private EntityKey _IntermediateChildPosKey;
        public EntityKey IntermediateChildPosKey
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _IntermediateChildPosKey;
                }
            }
            set
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _IntermediateChildPosKey = value;
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Methods => ACState

        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            var pwGroup = ParentPWGroup;
            if (!CheckParentGroupAndHandleSkipMode())
                return;

            if (!Root.Initialized)
            {
                SubscribeToProjectWorkCycle();
                return;
            }

            if (ProdOrderManager == null)
            {
                // Error50337: ProdOrderManager is null. [OK]
                Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(20)", 970, "Error50337");
                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, false);
                SubscribeToProjectWorkCycle();
                return;
            }

            PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
            // If dosing is not for production, then do nothing
            if (pwMethodProduction == null)
            {
                UnSubscribeToProjectWorkCycle();
                // Falls durch tiefere Callstacks der Status schon weitergeschaltet worden ist, dann schalte Status nicht weiter
                if (CurrentACState == ACStateEnum.SMStarting)
                    CurrentACState = ACStateEnum.SMCompleted;
                return;
            }

            core.datamodel.ACClassMethod refPAACClassMethod = RefACClassMethodOfContentWF;
            if (refPAACClassMethod == null)
                return;
            ACMethod acMethod = refPAACClassMethod.TypeACSignature();
            if (acMethod == null)
            {
                //Error50338: acMethod is null. [OK]
                Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "SMStarting(20)", 863, "Error50338", "acMethod");
                OnNewAlarmOccurred(PropNameProcessAlarm, msg, false);
                SubscribeToProjectWorkCycle();
                return;
            }


            if (!acMethod.IsValid())
            {
                // Error50339. H1 - H10 filling task not startable Order {0}, Bill of material {1}, line {2} [OK] 
                Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "SMStarting(30)", 1130, "Error50339");

                if (IsAlarmActive(PropNameProcessAlarm, msg.Message) == null)
                    Root.Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(PropNameProcessAlarm, msg, false);
                SubscribeToProjectWorkCycle();
                return;
            }

            if (pwGroup != null
                && this.ContentACClassWF != null
                && refPAACClassMethod != null)
            {
                PAProcessModule module = null;
                if (ParentPWGroup.NeedsAProcessModule && (ACOperationMode == ACOperationModes.Live || ACOperationMode == ACOperationModes.Simulation))
                    module = ParentPWGroup.AccessedProcessModule;
                // Testmode
                else
                    module = ParentPWGroup.ProcessModuleForTestmode;

                if (module == null)
                {
                    //Error50372: The workflow group has not occupied a process module.
                    // Die Workflowgruppe hat kein Prozessmodul belegt.
                    Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "SMStarting(10)", 1000, "Error50372");
                    ActivateProcessAlarmWithLog(msg, false);
                    SubscribeToProjectWorkCycle();
                    return;
                }

                PAProcessFunction responsibleFunc = CanStartProcessFunc(module, acMethod);
                if (responsibleFunc == null)
                {
                    SubscribeToProjectWorkCycle();
                    return;
                }


                PAFBinSelection binSelection = CurrentExecutingFunction<PAFBinSelection>();
                if (binSelection != null)
                {
                    SubscribeToProjectWorkCycle();
                    return;
                }

                StartNextCompResult result = StartNextCompResult.Done;
                if (IsProduction)
                    result = PreparePosition(pwMethodProduction);
                if (result == StartNextCompResult.CycleWait)
                {
                    SubscribeToProjectWorkCycle();
                    return;
                }
                else if (result == StartNextCompResult.NextCompStarted || result == StartNextCompResult.WaitForNextEvent)
                {
                    RecalcTimeInfo();
                    if (CreateNewProgramLog(acMethod, _NewAddedProgramLog == null) <= CreateNewProgramLogResult.ErrorNoProgramFound)
                        return;
                    module.TaskInvocationPoint.ClearMyInvocations(this);
                    _CurrentMethodEventArgs = null;
                    IACPointEntry task = module.TaskInvocationPoint.AddTask(acMethod, this);
                    if (!IsTaskStarted(task))
                    {
                        ACMethodEventArgs eM = _CurrentMethodEventArgs;
                        if (eM == null || eM.ResultState != Global.ACMethodResultState.FailedAndRepeat)
                        {
                            // Error50303 H1 - H10 filling task not startable Order {0}, Bill of material {1}, line {2} [OK] 
                            Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "SMStarting(40)", 1140, "Error50339");

                            if (IsAlarmActive(PropNameProcessAlarm, msg.Message) == null)
                                Root.Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                            OnNewAlarmOccurred(PropNameProcessAlarm, msg, false);
                        }
                        SubscribeToProjectWorkCycle();
                        return;
                    }
                    UpdateCurrentACMethod();
                }
                else if (result == StartNextCompResult.Done)
                {
                    UnSubscribeToProjectWorkCycle();
                    CurrentACState = ACStateEnum.SMCompleted;
                    return;
                }

                if (CurrentACState == ACStateEnum.SMStarting)
                {
                    CurrentACState = ACStateEnum.SMRunning;
                    RaiseRunningEvent();
                }
            }
        }

        public override void SMRunning()
        {
            base.SMRunning();
            if (IntermediateChildPosKey == null)
            {
                PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                if (pwMethodProduction != null)
                {
                    UnSubscribeToProjectWorkCycle();
                    StartNextCompResult result = PreparePosition(pwMethodProduction);
                    if (result == StartNextCompResult.Done)
                        CurrentACState = ACStateEnum.SMCompleted;
                }
            }
        }

        public override void SMIdle()
        {
            ClearMyConfiguration();
            using (ACMonitor.Lock(_20015_LockValue))
            {
                _NewAddedProgramLog = null;
            }
            base.SMIdle();
        }

        public override void TaskCallback(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject)
        {
            _InCallback = true;
            if (e != null)
            {
                IACTask taskEntry = wrapObject as IACTask;
                ACMethodEventArgs eM = e as ACMethodEventArgs;
                _CurrentMethodEventArgs = eM;
                PAProcessModule module = sender.ParentACComponent as PAProcessModule;
                PAProcessFunction function = module.GetExecutingFunction<PAProcessFunction>(eM.ACRequestID);
                if (taskEntry.State == PointProcessingState.Deleted && CurrentACState != ACStateEnum.SMIdle)
                {
                    if (function.CurrentACState == ACStateEnum.SMAborted || (function.LastACState == ACStateEnum.SMResetting && function.CurrentACState == ACStateEnum.SMIdle))
                    {
                        UnSubscribeToProjectWorkCycle();
                        CurrentACState = ACStateEnum.SMCompleted;
                    }
                    else
                    {
                        string id = "";
                        Msg msgBookingBinSelection = null;
                        if (function.CurrentACMethod.ValueT[PAFBinSelection.Const_InputSourceCodes] != null)
                            id = function.CurrentACMethod.ValueT[PAFBinSelection.Const_InputSourceCodes].ToString();
                        if (!string.IsNullOrEmpty(id))
                        {
                            msgBookingBinSelection = CreateReservationBooking(id);
                            if (msgBookingBinSelection != null && !msgBookingBinSelection.IsSucceded())
                                ActivateProcessAlarmWithLog(msgBookingBinSelection, false);
                        }

                        if (    msgBookingBinSelection == null 
                            ||  msgBookingBinSelection.IsSucceded()
                            ||  (function.CurrentACMethod.ValueT[PAFBinSelection.Const_Break] != null && (bool)function.CurrentACMethod.ValueT[PAFBinSelection.Const_Break])
                            )
                        {
                            UnSubscribeToProjectWorkCycle();
                            CurrentACState = ACStateEnum.SMCompleted;
                        }
                    }
                }
            }
            _InCallback = false;
        }

        protected override void OnNewProgramLogAddedToQueue(ACMethod acMethod, gip.core.datamodel.ACProgramLog currentProgramLog)
        {
            if (_NewAddedProgramLog == null)
            {
                _NewAddedProgramLog = currentProgramLog;
                //ACClassTaskQueue.TaskQueue.ObjectContext.ACChangesExecuted += ACClassTaskQueue_ChangesSaved;
            }
        }

        #endregion

        #region Methods -> Validator

        public StartNextCompResult ValidateStart(PAProcessModule module)
        {
            Msg msg = null;

            if (!Root.Initialized)
                return StartNextCompResult.CycleWait;

            if (module == null)
            {
                // Error50341: The PAProcessModule is null. [OK]
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(10)", 956, "Error50341");
                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, false);
                return StartNextCompResult.CycleWait;
            }

            PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
            // If dosing is not for production, then do nothing
            if (pwMethodProduction == null)
                return StartNextCompResult.Done;

            if (ProdOrderManager == null)
            {
                // Error50337: ProdOrderManager is null. [OK]
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(20)", 970, "Error50337");
                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, false);
                return StartNextCompResult.CycleWait;
            }
            return StartNextCompResult.NextCompStarted;
        }


        #endregion

        #region Methods -> Process

        private Msg CreateReservationBooking(string id)
        {
            Guid intermediateChildPosID = (Guid)IntermediateChildPosKey.EntityKeyValues[0].Value;
            BinSelectionModel binSelectionModel = new BinSelectionModel() { ProdorderPartslistPosID = intermediateChildPosID, RestQuantity = PWBinSelection.BinSelectionReservationQuantity, FacilityChargeID = null, Comment = PWBinSelection.BinSelectionReservationComment };
            switch (SourceInfoType)
            {
                case ManualPreparationSourceInfoTypeEnum.FacilityChargeID:
                    binSelectionModel.FacilityChargeID = new Guid(id);
                    break;
                case ManualPreparationSourceInfoTypeEnum.FacilityID:
                    binSelectionModel.FacilityID = new Guid(id);
                    break;
            }
            return DoInwardBooking(binSelectionModel);
        }

        public Msg DoInwardBooking(BinSelectionModel binSelectionModel)
        {
            Msg msg = null;
            using (DatabaseApp dbApp = new DatabaseApp())
            {

                ProdOrderPartslistPos intermediateChildPos = dbApp.ProdOrderPartslistPos.FirstOrDefault(c => c.ProdOrderPartslistPosID == (binSelectionModel.ProdorderPartslistPosID ?? Guid.Empty));
                KeyValuePair<FacilityPreBooking, ACMethodBooking> bookingParams = CreateNewPreBooking(dbApp, intermediateChildPos, binSelectionModel);
                msg = DoInwardBooking(dbApp, intermediateChildPos, binSelectionModel, bookingParams.Key, bookingParams.Value);
            }
            return msg;
        }

        private StartNextCompResult PreparePosition(PWMethodProduction pwMethodProduction)
        {
            Msg msg = null;
            using (var dbIPlus = new Database())
            {
                using (var dbApp = new DatabaseApp(dbIPlus))
                {
                    ProdOrderPartslistPos endBatchPos = pwMethodProduction.CurrentProdOrderPartslistPos.FromAppContext<ProdOrderPartslistPos>(dbApp);
                    CurrentEndBatchPosKey = endBatchPos.EntityKey;
                    if (pwMethodProduction.CurrentProdOrderBatch == null)
                    {
                        // Error50342: No batch assigned to last intermediate material of this workflow-process [OK] 
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "PreparePosition(30)", 1010, "Error50342");

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, false);
                        return StartNextCompResult.CycleWait;
                    }

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
                    {
                        // Error50343.: No relation defined between Workflownode and intermediate material in Materialworkflow [OK]
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "PreparePosition(40)", 761, "Error50343");

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, false);
                        return StartNextCompResult.CycleWait;
                    }

                    // Find intermediate position which is assigned to this Dosing-Workflownode
                    var currentProdOrderPartslist = endBatchPos.ProdOrderPartslist.FromAppContext<ProdOrderPartslist>(dbApp);
                    ProdOrderPartslistPos intermediatePosition = currentProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                        .Where(c => c.MaterialID.HasValue && c.MaterialID == matWFConnection.MaterialID
                            && c.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern
                            && !c.ParentProdOrderPartslistPosID.HasValue).FirstOrDefault();
                    if (intermediatePosition == null)
                    {
                        // Error50344: Intermediate line not found which is assigned to this ManualWeighing-Workflownode [OK]
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "PreparePosition(50)", 778, "Error50344");

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, false);
                        return StartNextCompResult.CycleWait;
                    }

                    ProdOrderPartslistPos intermediateChildPos = null;
                    // Lock, if a parallel Dosing also creates a child Position for this intermediate Position

                    using (ACMonitor.Lock(pwMethodProduction._62000_PWGroupLockObj))
                    {
                        // Find intermediate child position, which is assigned to this Batch
                        intermediateChildPos = intermediatePosition.ProdOrderPartslistPos_ParentProdOrderPartslistPos
                            .Where(c => c.ProdOrderBatchID.HasValue
                                        && c.ProdOrderBatchID.Value == pwMethodProduction.CurrentProdOrderBatch.ProdOrderBatchID)
                            .FirstOrDefault();

                        // If intermediate child position not found, generate childposition for this Batch/Intermediate
                        if (intermediateChildPos == null)
                        {
                            List<object> resultNewEntities = new List<object>();
                            msg = ProdOrderManager.BatchCreate(dbApp, intermediatePosition, batch, endBatchPos.BatchFraction, batch.BatchSeqNo, resultNewEntities); // Toleranz ist max. ein Batch mehr
                            if (msg != null)
                            {
                                Messages.LogException(this.GetACUrl(), "PreparePosition(60)", msg.InnerMessage);
                                dbApp.ACUndoChanges();
                                return StartNextCompResult.CycleWait;
                            }
                            else
                            {
                                dbApp.ACSaveChanges();
                            }
                            intermediateChildPos = resultNewEntities.Where(c => c is ProdOrderPartslistPos).FirstOrDefault() as ProdOrderPartslistPos;
                            if (intermediateChildPos != null && endBatchPos.FacilityLot != null)
                                intermediateChildPos.FacilityLot = endBatchPos.FacilityLot;
                        }
                    }
                    if (intermediateChildPos == null)
                    {
                        //Error50345:intermediateChildPos is null. [OK] 
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "PreparePosition(70)", 1040, "Error50345");
                        ActivateProcessAlarmWithLog(msg, false);
                        return StartNextCompResult.CycleWait;
                    }
                    else
                    {
                        ProdOrderPartslistPosRelation[] queryOpenMaterials = OnGetAllMaterials(dbIPlus, dbApp, intermediateChildPos);
                        if (queryOpenMaterials == null || !queryOpenMaterials.Any())
                        {
                            //Error50280: queryOpenMaterials is null or does not contain any material.
                            //msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(80)", 834, "Error50280");
                            //Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.Message);
                            //OnNewAlarmOccurred(ProcessAlarm, msg, false);
                            return StartNextCompResult.Done;
                        }

                        IntermediateChildPosKey = intermediateChildPos.EntityKey;
                    }
                }
            }
            return StartNextCompResult.NextCompStarted;
        }

        protected virtual ProdOrderPartslistPosRelation[] OnGetAllMaterials(Database dbIPlus, DatabaseApp dbApp, ProdOrderPartslistPos intermediateChildPos)
        {
            ProdOrderPartslistPosRelation[] queryOpenDosings = intermediateChildPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.ToArray()
                                .Where(c => c.MDProdOrderPartslistPosState != null && (c.SourceProdOrderPartslistPos != null && c.SourceProdOrderPartslistPos.Material != null
                                                                                    && c.SourceProdOrderPartslistPos.Material.UsageACProgram))
                                .OrderBy(c => c.Sequence)
                                .ToArray();
            return queryOpenDosings;
        }

        #endregion

        #region Methods -> Helper

        [ACMethodInfo("", "", 999)]
        public EntityKey GetIntermediateChildPos()
        {
            return IntermediateChildPosKey;
        }

        #endregion

        #region Methods -> Booking

        public virtual MsgWithDetails CompleteReservationBookingWithRestQ(Guid intermediateChildPosID)
        {
            MsgWithDetails completeMsg = new MsgWithDetails();
            List<BinSelectionModel> binSelectionModels = GetRestInwardQuantity(intermediateChildPosID);
            if (binSelectionModels.Any())
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    foreach (BinSelectionModel binSelectionModel in binSelectionModels)
                    {
                        ProdOrderPartslistPos intermediateChildPos = dbApp.ProdOrderPartslistPos.FirstOrDefault(c => c.ProdOrderPartslistPosID == (binSelectionModel.ProdorderPartslistPosID ?? Guid.Empty));
                        KeyValuePair<FacilityPreBooking, ACMethodBooking> bookingParams = CreateNewPreBooking(dbApp, intermediateChildPos, binSelectionModel);
                        Msg msg = DoInwardBooking(dbApp, intermediateChildPos, binSelectionModel, bookingParams.Key, bookingParams.Value);
                        completeMsg.AddDetailMessage(msg);
                    }
                }
            }
            return completeMsg;
        }

        public virtual List<BinSelectionModel> GetRestInwardQuantity(Guid intermediateChildPosID)
        {
            List<BinSelectionModel> binSelectionModels = new List<BinSelectionModel>();
            using (var dbIPlus = new Database())
            using (var dbApp = new DatabaseApp(dbIPlus))
            {
                ProdOrderPartslistPos intermediateChildPos = dbApp.ProdOrderPartslistPos.FirstOrDefault(c => c.ProdOrderPartslistPosID == intermediateChildPosID);

                List<FacilityBooking> reservationFbcs = intermediateChildPos
                  .FacilityBooking_ProdOrderPartslistPos
                  .Where(c => c.Comment.Contains(PWBinSelection.BinSelectionReservationComment))
                  .OrderBy(x => x.InsertDate)
                  .ToList();

                if (reservationFbcs.Any())
                {
                    double outwardSumQuantity =
                       intermediateChildPos
                       .ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos
                       .SelectMany(c => c.FacilityBookingCharge_ProdOrderPartslistPosRelation)
                       .Select(c => c.OutwardQuantityUOM)
                       .DefaultIfEmpty()
                       .Sum();

                    double inwardQuantity =
                       intermediateChildPos
                       .FacilityBookingCharge_ProdOrderPartslistPos
                       .Select(c => c.InwardQuantityUOM)
                       .DefaultIfEmpty()
                       .Sum();

                    double restQuantity = outwardSumQuantity - inwardQuantity;
                    double partRestQuantity = restQuantity / reservationFbcs.Count();

                    List<FacilityBooking> outwardFbc = intermediateChildPos
                       .ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos
                       .SelectMany(c => c.FacilityBooking_ProdOrderPartslistPosRelation)
                       .OrderByDescending(c => c.InsertDate)
                       .ToList();
                    bool manyOutwardFbcs = outwardFbc.Count() > 1 && reservationFbcs.Count() > 1;

                    foreach (FacilityBooking fb in reservationFbcs)
                    {
                        BinSelectionModel binSelectionModel = new BinSelectionModel();
                        binSelectionModel.ProdorderPartslistPosID = intermediateChildPosID;
                        binSelectionModel.FacilityID = fb.InwardFacilityID;
                        binSelectionModel.FacilityChargeID = fb.FacilityBookingCharge_FacilityBooking.Select(c=>c.InwardFacilityChargeID).DefaultIfEmpty().FirstOrDefault();
                        binSelectionModel.FacilityLotID = fb.InwardFacilityLotID;
                        if (manyOutwardFbcs)
                        {
                            FacilityBooking nextFb = reservationFbcs.Where(c => c.FacilityBookingNo != fb.FacilityBookingNo && c.InsertDate > fb.InsertDate).FirstOrDefault();
                            double outwardBookingQuantity = outwardFbc.Where(c => c.InsertDate >= fb.InsertDate && (nextFb == null || c.InsertDate < nextFb.InsertDate)).Select(c => c.OutwardQuantity).DefaultIfEmpty().Sum();
                            if (outwardBookingQuantity > 0)
                                binSelectionModel.RestQuantity = outwardBookingQuantity - PWBinSelection.BinSelectionReservationQuantity;
                            else
                                binSelectionModel.RestQuantity = partRestQuantity - PWBinSelection.BinSelectionReservationQuantity;
                        }
                        else
                            binSelectionModel.RestQuantity = partRestQuantity;
                        binSelectionModels.Add(binSelectionModel);
                    }
                }
            }
            return binSelectionModels;
        }

        public virtual Msg DoInwardBooking(DatabaseApp dbApp, ProdOrderPartslistPos intermediateChildPos, BinSelectionModel binSelectionModel, FacilityPreBooking facilityPreBooking, ACMethodBooking bookingParam)
        {
            MsgWithDetails collectedMessages = new MsgWithDetails();
            Msg msg = null;
            bool changePosState = true;
            try
            {
                msg = dbApp.ACSaveChangesWithRetry();
                if (msg != null)
                {
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                    return msg;
                }

                MDProdOrderPartslistPosState posState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed).FirstOrDefault();
                bookingParam.IgnoreIsEnabled = true;
                bookingParam.Comment = binSelectionModel.Comment;
                ACMethodEventArgs resultBooking = ACFacilityManager.BookFacilityWithRetry(ref bookingParam, dbApp) as ACMethodEventArgs;
                if (resultBooking.ResultState == Global.ACMethodResultState.Failed || resultBooking.ResultState == Global.ACMethodResultState.Notpossible)
                {
                    collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                    OnNewAlarmOccurred(PWBase.PropNameProcessAlarm, new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoInwardBooking(60)", 1016), false);
                    changePosState = false;
                }
                else
                {
                    if (!bookingParam.ValidMessage.IsSucceded() || bookingParam.ValidMessage.HasWarnings())
                    {
                        Root.Messages.LogError(GetACUrl(), "DoInwardBooking(70)", bookingParam.ValidMessage.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoInwardBooking(70)", 1024), false);
                        changePosState = false;
                    }
                    changePosState = true;
                    if (bookingParam.ValidMessage.IsSucceded())
                    {
                        facilityPreBooking.DeleteACObject(dbApp, true);
                        intermediateChildPos.IncreaseActualQuantityUOM(bookingParam.InwardQuantity.Value);
                        msg = dbApp.ACSaveChangesWithRetry();
                        if (msg != null)
                        {
                            collectedMessages.AddDetailMessage(msg);
                            Root.Messages.LogError(GetACUrl(), "DoInwardBooking(80)", msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoInwardBooking(80)", 1036), false);
                        }
                    }
                    else
                    {
                        collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                    }

                    if (changePosState)
                        intermediateChildPos.MDProdOrderPartslistPosState = posState;

                    msg = dbApp.ACSaveChangesWithRetry();
                    if (msg != null)
                    {
                        collectedMessages.AddDetailMessage(msg);
                        Root.Messages.LogError(GetACUrl(), "DoInwardBooking(90)", msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoInwardBooking(90)", 1048), false);
                    }
                    else
                    {
                        intermediateChildPos.RecalcActualQuantityFast();
                        if (dbApp.IsChanged)
                            dbApp.ACSaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                collectedMessages.AddDetailMessage(new Msg(eMsgLevel.Exception, e.Message));
                Root.Messages.LogException(GetACUrl(), "DoInwardBooking(100)", e);
            }
            return collectedMessages.MsgDetailsCount > 0 ? collectedMessages : null;
        }

        #region Methods -> Booking -> Prepare FacilityPreBooking

        protected virtual KeyValuePair<FacilityPreBooking, ACMethodBooking> CreateNewPreBooking(DatabaseApp dbApp, ProdOrderPartslistPos intermediateChildPos, BinSelectionModel binSelectionModel)
        {
            KeyValuePair<FacilityPreBooking, ACMethodBooking> bookingParams = new KeyValuePair<FacilityPreBooking, ACMethodBooking>();
            switch (SourceInfoType)
            {
                case ManualPreparationSourceInfoTypeEnum.FacilityChargeID:
                    bookingParams = CreateNewFacilityChargePreBooking(dbApp, intermediateChildPos, binSelectionModel);
                    break;
                case ManualPreparationSourceInfoTypeEnum.FacilityID:
                    bookingParams = CreateNewFacilityPreBooking(dbApp, intermediateChildPos, binSelectionModel);
                    break;
                default:
                    break;
            }
            return bookingParams;
        }

        protected virtual KeyValuePair<FacilityPreBooking, ACMethodBooking> CreateNewFacilityPreBooking(DatabaseApp dbApp, ProdOrderPartslistPos intermediateChildPos, BinSelectionModel binSelectionModel)
        {
            Facility facility = dbApp.Facility.FirstOrDefault(c => c.FacilityID == (binSelectionModel.FacilityID ?? Guid.Empty));
            FacilityCharge facilityCharge = null;
            if (binSelectionModel.FacilityChargeID == null)
            {
                facilityCharge = FacilityCharge.NewACObject(dbApp, null);
                facilityCharge.FacilityLot = intermediateChildPos.FacilityLot;
                facilityCharge.Facility = facility;
                facilityCharge.Material = intermediateChildPos.Material;
                facilityCharge.MDUnit = intermediateChildPos.Material.BaseMDUnit;
            }
            else
                facilityCharge = dbApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == (binSelectionModel.FacilityChargeID ?? Guid.Empty));
            FacilityPreBooking facilityPreBooking = ProdOrderManager.NewInwardFacilityPreBooking(ACFacilityManager, dbApp, intermediateChildPos);
            ACMethodBooking bookingParam = facilityPreBooking.ACMethodBooking as ACMethodBooking;
            bookingParam.InwardQuantity = binSelectionModel.RestQuantity;
            bookingParam.InwardFacility = facility;
            if (ParentPWGroup != null && ParentPWGroup.AccessedProcessModule != null)
                bookingParam.PropertyACUrl = ParentPWGroup.AccessedProcessModule.GetACUrl();
            SetACMethodBookingPropertyACUrl(bookingParam);
            return new KeyValuePair<FacilityPreBooking, ACMethodBooking>(facilityPreBooking, bookingParam);
        }

        protected virtual KeyValuePair<FacilityPreBooking, ACMethodBooking> CreateNewFacilityChargePreBooking(DatabaseApp dbApp, ProdOrderPartslistPos intermediateChildPos, BinSelectionModel binSelectionModel)
        {
            FacilityCharge facilityCharge = dbApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == (binSelectionModel.FacilityChargeID ?? Guid.Empty));
            if (facilityCharge == null)
            {
                facilityCharge = FacilityCharge.NewACObject(dbApp, null);
                facilityCharge.FacilityChargeID = binSelectionModel.FacilityChargeID ?? Guid.Empty;
                facilityCharge.FacilityLot = intermediateChildPos.FacilityLot;
                facilityCharge.Facility = dbApp.Facility.Where(c => c.FacilityNo == Config_LPHF).FirstOrDefault();
                facilityCharge.Material = intermediateChildPos.Material;
                facilityCharge.MDUnit = intermediateChildPos.Material.BaseMDUnit;
            }

            FacilityPreBooking facilityPreBooking = ProdOrderManager.NewInwardFacilityPreBooking(ACFacilityManager, dbApp, intermediateChildPos);
            ACMethodBooking bookingParam = facilityPreBooking.ACMethodBooking as ACMethodBooking;
            bookingParam.InwardQuantity = binSelectionModel.RestQuantity;
            bookingParam.InwardFacility = facilityCharge.Facility;
            if (ParentPWGroup != null && ParentPWGroup.AccessedProcessModule != null)
                bookingParam.PropertyACUrl = ParentPWGroup.AccessedProcessModule.GetACUrl();
            SetACMethodBookingPropertyACUrl(bookingParam);
            return new KeyValuePair<FacilityPreBooking, ACMethodBooking>(facilityPreBooking, bookingParam);
        }

        private void SetACMethodBookingPropertyACUrl(ACMethodBooking bookingParam)
        {
            string propertyACUrl = "";
            if (ParentPWGroup != null && ParentPWGroup.AccessedProcessModule != null)
                propertyACUrl = ParentPWGroup.AccessedProcessModule.GetACUrl();
            bookingParam.PropertyACUrl = propertyACUrl;
        }

        #endregion

        #endregion


        #region Reset

        public override void Reset()
        {
            CurrentEndBatchPosKey = null;
            IntermediateChildPosKey = null;

            base.Reset();
        }

        public override bool IsEnabledReset()
        {
            //if (this.TaskSubscriptionPoint.ConnectionList.Any())
            //return false;
            return base.IsEnabledReset();
        }

        public override void Recycle(IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {
            CurrentEndBatchPosKey = null;
            IntermediateChildPosKey = null;

            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }

        #endregion

        #endregion

        #region Misc private fields
        private gip.core.datamodel.ACProgramLog _NewAddedProgramLog = null;
        #endregion
    }
}
