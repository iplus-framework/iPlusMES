using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.processapplication;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Bin Discharging'}de{'Gebinde entleeren'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWBinDischarging : PWNodeProcessMethod
    {
        #region c'tors

        public const string PWClassName = "PWBinDischarging";

        static PWBinDischarging()
        {
            ACMethod method = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();

            method.ParameterValueList.Add(new ACValue("SourceInfoType", typeof(ManualPreparationSourceInfoTypeEnum), ManualPreparationSourceInfoTypeEnum.FacilityID, Global.ParamOption.Optional));
            paramTranslation.Add("SourceInfoType", "en{'Define source info type'}de{'Quelletyp'}");

            method.ParameterValueList.Add(new ACValue("CheckDischargedQuantity", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("CheckDischargedQuantity", "en{'Check discharged quantity'}de{'Entladene Menge prüfen'}");

            method.ParameterValueList.Add(new ACValue("TolerancePlus", typeof(Double), (Double)1.0, Global.ParamOption.Required));
            paramTranslation.Add("TolerancePlus", "en{'Tolerance + [+=kg/-=%]'}de{'Toleranz + [+=kg/-=%]'}");

            method.ParameterValueList.Add(new ACValue("ToleranceMinus", typeof(Double), (Double)1.0, Global.ParamOption.Required));
            paramTranslation.Add("ToleranceMinus", "en{'Tolerance - [+=kg/-=%]'}de{'Toleranz - [+=kg/-=%]'}");

            var wrapper = new ACMethodWrapper(method, "en{'Configuration'}de{'Konfiguration'}", typeof(PWBinDischarging), paramTranslation, null);
            ACMethod.RegisterVirtualMethod(typeof(PWBinDischarging), ACStateConst.SMStarting, wrapper);
        }

        public PWBinDischarging(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            _DischargingItemManager = DischargingItemManager.ACRefToServiceInstance(this);
            if (_DischargingItemManager == null)
                throw new Exception("DischargingItemManager not configured");

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            using (ACMonitor.Lock(_20015_LockValue))
            {
            }

            if (_DischargingItemManager != null)
                DischargingItemManager.DetachACRefFromServiceInstance(this, _DischargingItemManager);
            _DischargingItemManager = null;

            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties


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


        public bool CheckDischargedQuantity
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    ACValue acValue = method.ParameterValueList.GetACValue("CheckDischargedQuantity");
                    if (acValue != null)
                        return acValue.ParamAsBoolean;
                }
                return false;
            }
        }

        public double TolerancePlus
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    ACValue acValue = method.ParameterValueList.GetACValue("TolerancePlus");
                    if (acValue != null)
                        return acValue.ParamAsDouble;
                }
                return 1.0;
            }
        }

        public double ToleranceMinus
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    ACValue acValue = method.ParameterValueList.GetACValue("ToleranceMinus");
                    if (acValue != null)
                        return acValue.ParamAsDouble;
                }
                return 1.0;
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

        protected ACRef<DischargingItemManager> _DischargingItemManager = null;
        public DischargingItemManager DischargingItemManager
        {
            get
            {
                if (_DischargingItemManager == null)
                    return null;
                return _DischargingItemManager.ValueT;
            }
        }

        #endregion

        #region Properties -> Pos

        [ACPropertyInfo(999)]
        public EntityKey CurrentEndBatchPosKey { get; set; }

        public EntityKey IntermediateChildPosKey { get; set; }


        #endregion

        private PAEScaleBase _CheckScale;
        public PAEScaleBase CheckScale
        {
            get
            {
                if (_CheckScale == null)
                {
                    var processModule = ParentPWGroup?.AccessedProcessModule;
                    if (processModule != null)
                    {
                        core.datamodel.ACClassMethod refPAACClassMethod = null;
                        if (this.ContentACClassWF != null)
                        {

                            using (ACMonitor.Lock(this.ContextLockForACClassWF))
                            {
                                refPAACClassMethod = this.ContentACClassWF.RefPAACClassMethod;
                            }
                        }

                        ACMethod acMethod = refPAACClassMethod.TypeACSignature();

                        PAProcessFunction paProcessFunction = null;
                        processModule.GetACStateOfFunction(acMethod.ACIdentifier, out paProcessFunction);

                        IPAFuncScaleConfig function = paProcessFunction as IPAFuncScaleConfig;
                        if (function != null)
                            _CheckScale = function.CurrentScaleForWeighing;
                    }
                }
                return _CheckScale;
            }
        }

        [ACPropertyInfo(true, 850)]
        public double ScaleWeightOnStart
        {
            get;
            set;
        }

        #endregion

        #region Methods

        #region Methods => ACState

        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            var pwGroup = ParentPWGroup;
            if (pwGroup == null) // Is null when Service-Application is shutting down
            {
                if (this.InitState == ACInitState.Initialized)
                {
                    Messages.LogError(this.GetACUrl(), "SMStarting()", "ParentPWGroup is null");
                    return;
                }
            }

            bool isCompleted = IsCompleted(pwGroup);
            if (isCompleted)
            {
                UnSubscribeToProjectWorkCycle();
                if (CurrentACState == ACStateEnum.SMStarting)
                    CurrentACState = ACStateEnum.SMCompleted;
                return;
            }

            if (!Root.Initialized)
            {
                SubscribeToProjectWorkCycle();
                return;
            }

            if (ProdOrderManager == null)
            {
                // Error50330 - OK
                Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(20)", 970, "Error50330");
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

            if (CheckDischargedQuantity)
            {
                if (CheckScale == null)
                {
                    //TODO: alarm
                }

                ScaleWeightOnStart = CheckScale.ActualValue.ValueT;
            }

            core.datamodel.ACClassMethod refPAACClassMethod = null;
            if (this.ContentACClassWF != null)
            {

                using (ACMonitor.Lock(this.ContextLockForACClassWF))
                {
                    refPAACClassMethod = this.ContentACClassWF.RefPAACClassMethod;
                }
            }

            ACMethod acMethod = refPAACClassMethod.TypeACSignature();
            if (acMethod == null)
            {
                //Error50298: acMethod is null. - missing - ACProdOrderManager
                Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "SMStarting(20)", 863, "Error50298", "acMethod");
                OnNewAlarmOccurred(PropNameProcessAlarm, msg, false);
                SubscribeToProjectWorkCycle();
                return;
            }

            if (!acMethod.IsValid())
            {
                // Error50331 H1 - H10 filling task not startable Order {0}, Bill of material {1}, line {2} [OK] 
                Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "SMStarting(30)", 1130, "Error50331");

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

                PAFBinDischarging binDischarging = CurrentExecutingFunction<PAFBinDischarging>();
                if (binDischarging != null)
                {
                    SubscribeToProjectWorkCycle();
                    return;
                }

                StartNextCompResult result = StartNextCompResult.Done;
                if (IsProduction)
                    result = PreparePosition(module, pwMethodProduction);

                if (IntermediateChildPosKey != null)
                {
                    Guid intermediateChildPosID = (Guid)IntermediateChildPosKey.EntityKeyValues[0].Value;
                    List<DischargingItem> dischargingItems = DischargingItemManager.LoadDischargingItemList(intermediateChildPosID, SourceInfoType);
                    if (dischargingItems == null || !dischargingItems.Any(c => !c.IsDischarged))
                    {
                        UnSubscribeToProjectWorkCycle();
                        CurrentACState = ACStateEnum.SMCompleted;
                        return;
                    }
                }

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
                            Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "SMStarting(40)", 1140, "Error50303");

                            if (IsAlarmActive(PropNameProcessAlarm, msg.Message) == null)
                                Root.Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                            OnNewAlarmOccurred(PropNameProcessAlarm, msg, false);
                        }
                        SubscribeToProjectWorkCycle();
                        return;
                    }
                    UpdateCurrentACMethod();
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
            PAProcessModule module = null;
            if (ParentPWGroup.NeedsAProcessModule && (ACOperationMode == ACOperationModes.Live || ACOperationMode == ACOperationModes.Simulation))
                module = ParentPWGroup.AccessedProcessModule;
            else
                module = ParentPWGroup.ProcessModuleForTestmode;

            if (IntermediateChildPosKey == null)
            {
                if (IsProduction)
                {

                    PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                    if (module != null && pwMethodProduction != null)
                        PreparePosition(module, pwMethodProduction);
                }
            }


            Guid intermediateChildPosID = (Guid)IntermediateChildPosKey.EntityKeyValues[0].Value;
            List<DischargingItem> dischargingItems = DischargingItemManager.LoadDischargingItemList(intermediateChildPosID, SourceInfoType);
            if (dischargingItems == null || !dischargingItems.Any(c => !c.IsDischarged))
            {
                CurrentACState = ACStateEnum.SMCompleted;
                UnSubscribeToProjectWorkCycle();
            }
            else
            {

                core.datamodel.ACClassMethod refPAACClassMethod = null;
                if (this.ContentACClassWF != null)
                {
                    using (ACMonitor.Lock(this.ContextLockForACClassWF))
                    {
                        refPAACClassMethod = this.ContentACClassWF.RefPAACClassMethod;
                    }
                }

                ACMethod acMethod = refPAACClassMethod.TypeACSignature();

                PAProcessFunction responsibleFunc = CanStartProcessFunc(module, acMethod);
                if (responsibleFunc == null)
                    SubscribeToProjectWorkCycle();
                //else if (CreateNewProgramLog(acMethod, _NewAddedProgramLog == null) <= CreateNewProgramLogResult.ErrorNoProgramFound)
                //    SubscribeToProjectWorkCycle();
                else
                {
                    IACPointEntry task = module.TaskInvocationPoint.GetTaskOfACMethod(acMethod);
                    module.TaskInvocationPoint.ClearMyInvocations(this);
                    _CurrentMethodEventArgs = null;
                    task = module.TaskInvocationPoint.AddTask(acMethod, this);
                    if (!IsTaskStarted(task))
                    {
                        ACMethodEventArgs eM = _CurrentMethodEventArgs;
                        if (eM == null || eM.ResultState != Global.ACMethodResultState.FailedAndRepeat)
                        {
                            Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "SMStarting(40)", 1140, "Error50303");
                            if (IsAlarmActive(PropNameProcessAlarm, msg.Message) == null)
                                Root.Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                            OnNewAlarmOccurred(PropNameProcessAlarm, msg, false);
                        }
                        SubscribeToProjectWorkCycle();
                    }
                    else
                        UpdateCurrentACMethod();
                }
            }
        }

        public override void SMIdle()
        {
            ClearMyConfiguration();
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
                    else if (function.CurrentACState == ACStateEnum.SMCompleted)
                    {
                        if (IntermediateChildPosKey == null)
                        {
                            PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                            PreparePosition(module, pwMethodProduction);
                        }
                        Guid intermediateChildPosID = (Guid)IntermediateChildPosKey.EntityKeyValues[0].Value;
                        List<DischargingItem> dischargingItems = DischargingItemManager.LoadDischargingItemList(intermediateChildPosID, SourceInfoType);
                        string id = "";
                        Guid testGUID = Guid.Empty;
                        if (function.CurrentACMethod.ValueT[PAFBinDischarging.Const_InputSourceCodes] != null)
                            id = function.CurrentACMethod.ValueT[PAFBinDischarging.Const_InputSourceCodes].ToString();
                        if (!string.IsNullOrEmpty(id) && Guid.TryParse(id, out testGUID))
                        {
                            DischargingItem dischargingItem = dischargingItems.FirstOrDefault(c => c.ItemID == new Guid(id));
                            string propertyACUrl = "";
                            if (ParentPWGroup != null && ParentPWGroup.AccessedProcessModule != null)
                                propertyACUrl = ParentPWGroup.AccessedProcessModule.GetACUrl();
                            KeyValuePair<Msg, DischargingItem> bookingResult = DischargingItemManager.ProceeedBooking(ACFacilityManager, ProdOrderManager, SourceInfoType, id, dischargingItem, propertyACUrl);
                            function.SendChangedACMethod();

                            if (bookingResult.Key != null && !bookingResult.Key.IsSucceded())
                                ActivateProcessAlarmWithLog(bookingResult.Key, false);
                            else if (!dischargingItems.Any(c => !c.IsDischarged))
                            {
                                SubscribeToProjectWorkCycle();
                            }
                        }
                    }
                }
            }
            _InCallback = false;
        }
        #endregion

        private bool IsCheckDischargingQuantitySuccessfull()
        {
            if (!CheckDischargedQuantity)
                return true;

            if (CheckScale == null)
                return true;

            //if (ApplicationManager != null && ApplicationManager.IsSimulationOn)
            //    return true;

            if (IntermediateChildPosKey == null)
            {
                //Error50336: Intermediate Child Position is null!
                Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "IsCheckDischargingQuantitySuccessfull(10)", 565, "Error50336");
                ActivateProcessAlarmWithLog(msg);
                return false;
            }

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                ProdOrderPartslistPos intermediatePos = dbApp.GetObjectByKey(IntermediateChildPosKey) as ProdOrderPartslistPos;

                double scaleWeightAfterDischarge = CheckScale.ActualValue.ValueT;
                double dischargedQuantity = scaleWeightAfterDischarge - ScaleWeightOnStart;
                double targetQuantity = intermediatePos.TargetQuantityUOM;

                double toleranceMinus = ToleranceMinus >= 0 ? ToleranceMinus : (Math.Abs(ToleranceMinus) / 100) * targetQuantity;
                double tolerancePlus = TolerancePlus >= 0 ? TolerancePlus : (Math.Abs(TolerancePlus) / 100) * targetQuantity;

                if (dischargedQuantity >= (targetQuantity - toleranceMinus) && (targetQuantity + tolerancePlus) >= dischargedQuantity)
                    return true;
                else
                {
                    double targetScaleWeight = ScaleWeightOnStart + targetQuantity;
                    double diff = scaleWeightAfterDischarge - targetScaleWeight;

                    //Error50346 : Weighing-Check-Alarm: The scale measures a total weight of {0} kg. But {1} kg material must appear in it. The difference of {2} kg is too high (Min-Tol.: {3}, Max-Tol.: {4}).
                    Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "IsCheckDischargingQuantitySuccessfull(20)", 613, "Error50346", scaleWeightAfterDischarge, targetScaleWeight, diff,
                                                                                                                                toleranceMinus, tolerancePlus);

                    ActivateProcessAlarmWithLog(msg);
                    return false;
                }
            }
        }

        #endregion

        #region Private & Helper
        private StartNextCompResult PreparePosition(PAProcessModule module, PWMethodProduction pwMethodProduction)
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
                        // Error50276: No batch assigned to last intermediate material of this workflow-process [OK] 
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "PreparePosition(30)", 1010, "Error50333");

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
                        // Error50277: No relation defined between Workflownode and intermediate material in Materialworkflow [OK] 
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "PreparePosition(40)", 761, "Error50334");

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
                        // Error50278: Intermediate line not found which is assigned to this ManualWeighing-Workflownode [OK]
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "PreparePosition(50)", 778, "Error50335");

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
                                endBatchPos.FacilityLot = endBatchPos.FacilityLot;
                        }
                    }
                    if (intermediateChildPos == null)
                    {
                        //Error50279:intermediateChildPos is null. [OK]
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "PreparePosition(70)", 1040, "Error50336");
                        ActivateProcessAlarmWithLog(msg, false);
                        return StartNextCompResult.CycleWait;
                    }
                    else
                    {
                        IntermediateChildPosKey = intermediateChildPos.EntityKey;
                    }


                }
            }
            return StartNextCompResult.NextCompStarted;
        }

        public bool IsCompleted(PWGroup pwGroup)
        {
            return ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMBatchCancelled)
               || ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode)
               || ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMDisThenNextComp)
               || ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMInterDischarging)
               || ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode)
               || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMBatchCancelled)
               || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode)
               || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode);
        }

        #endregion

        #region Private fields
        private gip.core.datamodel.ACProgramLog _NewAddedProgramLog = null;
        #endregion

    }
}
