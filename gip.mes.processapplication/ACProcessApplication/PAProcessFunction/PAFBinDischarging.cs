using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.core.processapplication;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Bin Discharging'}de{'Gebinde entleeren'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, PWBinDischarging.PWClassName, true, BSOConfig = "BSOBinDischarging", SortIndex = 300)]
    public class PAFBinDischarging : PAProcessFunction, IPAFuncScaleConfig
    {
        #region constants

        public const string Const_InputSourceCodes = "InputSourceCodes";

        #endregion

        #region Constructors

        public const string ClassName = "PAFBinDischarging";

        static PAFBinDischarging()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFBinDischarging), ACStateConst.TMStart, CreateVirtualMethod("BinDischarging", "en{'Bin Discharging'}de{'Gebinde entleeren'}", typeof(PWBinDischarging)));
            RegisterExecuteHandler(typeof(PAFBinDischarging), HandleExecuteACMethod_PAFBinDischarging);
        }

        public PAFBinDischarging(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _FuncScaleConfig = new ACPropertyConfigValue<string>(this, PAScaleMappingHelper<IACComponent>.FuncScaleConfigName, "");
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            DischargingItemManager = new DischargingItemManager(Root, this, ClassName, null, null, null);
            DischargingItemNoValidator = new DischargingItemNoValidator(this, ClassName);
            bool result = base.ACInit(startChildMode);
            _ = FuncScaleConfig;
            return result;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            DischargingItemManager = null;
            DischargingItemNoValidator = null;

            using (ACMonitor.Lock(_20015_LockValue))
            {
                if (_ScaleMappingHelper != null)
                {
                    _ScaleMappingHelper.DetachAndRemove();
                    _ScaleMappingHelper = null;
                }
            }

            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties

        public DischargingItemManager DischargingItemManager { get; private set; }

        public DischargingItemNoValidator DischargingItemNoValidator { get; private set; }

        private Msg _InputSourceCodeValidationMessage;
        [ACPropertyInfo(500)]
        public Msg InputSourceCodeValidationMessage
        {
            get
            {
                return _InputSourceCodeValidationMessage;
            }
            set
            {
                if (_InputSourceCodeValidationMessage != value)
                {
                    _InputSourceCodeValidationMessage = value;
                    OnPropertyChanged("InputSourceCodeValidationMessage");
                }
            }
        }

        [ACPropertyInfo(501)]
        public PWBinDischarging BinDischarging
        {
            get
            {
                if (CurrentTask != null && CurrentTask.ValueT != null)
                    return CurrentTask.ValueT as PWBinDischarging;
                return null;
            }
        }

        [ACPropertyBindingSource(IsPersistable = true)]
        public IACContainerTNet<bool> WaitForConfirmDischarge
        {
            get;
            set;
        }

        [ACPropertyBindingSource(IsPersistable = true)]
        public IACContainerTNet<double> ScaleValueBeforeDischarging
        {
            get;
            set;
        }

        private bool _DischargeConfirmed = false;

        private DischargingItem CurrentDischargingItem = null;

        #endregion

        #region Methods

        #region Methods -> ACState

        [ACMethodAsync("Process", "en{'Start'}de{'Start'}", (short)MISort.Start, false)]
        public override ACMethodEventArgs Start(ACMethod acMethod)
        {
            return base.Start(acMethod);
        }

        public override void SMRunning()
        {
            if (!Root.Initialized)
                SubscribeToProjectWorkCycle();

            UnSubscribeToProjectWorkCycle();
            bool isCodeRecieved =
                CurrentACMethod != null
                && CurrentACMethod.ValueT != null
                && CurrentACMethod.ValueT[Const_InputSourceCodes] != null;

            if (WaitForConfirmDischarge.ValueT)
            {
                if (_DischargeConfirmed)
                {
                    CurrentACState = ACStateEnum.SMCompleted;
                    using (ACMonitor.Lock(_20015_LockValue))
                    {
                        WaitForConfirmDischarge.ValueT = false;
                        _DischargeConfirmed = false;
                    }
                    return;
                }
            }

            if (isCodeRecieved)
            {
                // Call validation from inputs from workflow params
                if (InputSourceCodeValidationMessage == null)
                {
                    string itemNo = CurrentACMethod.ValueT[Const_InputSourceCodes].ToString();
                    Guid itemNoGuid = Guid.Empty;
                    if (Guid.TryParse(itemNo, out itemNoGuid))
                    {
                        if (BinDischarging.IntermediateChildPosKey == null)
                        {
                            SubscribeToProjectWorkCycle();
                            return;
                        }

                        Guid intermediateChildPosID = (Guid)BinDischarging.IntermediateChildPosKey.EntityKeyValues[0].Value;
                        List<DischargingItem> dischargingItems = DischargingItemManager.LoadDischargingItemList(intermediateChildPosID, BinDischarging.SourceInfoType);
                        CurrentDischargingItem = dischargingItems.FirstOrDefault(c => c.ItemID == itemNoGuid);
                        if(CurrentDischargingItem != null)
                            InputSourceCodeValidationMessage = DischargingItemNoValidator.ValidateInputNo(itemNo, intermediateChildPosID, CurrentDischargingItem.ProdorderPartslistPosRelationID, BinDischarging.SourceInfoType, DischargingItemNoValidatorBehaviorEnum.BINDischarging_NoOutwardBookings);
                        else
                        {
                            //TODO: error
                        }
                    }
                }

                if (InputSourceCodeValidationMessage.IsSucceded())
                {
                    if(BinDischarging.CheckDischargedQuantity && CurrentScaleForWeighing != null)
                        ScaleValueBeforeDischarging.ValueT = CurrentScaleForWeighing.ActualValue.ValueT;

                    WaitForConfirmDischarge.ValueT = true;
                    UnSubscribeToProjectWorkCycle();
                }
            }
        }

        public override void SMIdle()
        {
            using (ACMonitor.Lock(_20015_LockValue))
            {
                WaitForConfirmDischarge.ValueT = false;
                ScaleValueBeforeDischarging.ValueT = 0;
                _DischargeConfirmed = false;
            }

            base.SMIdle();
        }


        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "SetCodeNo":
                    result = SetCodeNo(acParameter[0] as string);
                    return true;
                case "GetSelection":
                    result = GetSelection();
                    return true;
                case PAFWorkTaskScanBase.MN_OnScanEvent:
                    result = OnScanEvent((Guid)acParameter[0], (Guid)acParameter[1], (int)acParameter[2]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PAFBinDischarging(out object result, IACComponent acComponent, string acMethodName, core.datamodel.ACClassMethod acClassMethod, object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessFunction(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        #endregion

        #region Methods -> Customer
        [ACMethodInfo("SetCodeNo", "en{'SetCodeNo'}de{'SetCodeNo'}", 501)]
        public Msg SetCodeNo(string itemNo)
        {
            Msg msg = null;

            if (BinDischarging == null)
            {
                // Error50303: This discharging function was not invoked by a workflow node. (Diese Entleerfunktion wurde nicht durch einen Worfkowknoten gestartet.)
                msg = new Msg(this, eMsgLevel.Error, ClassName, "SetCodeNo", 10, "Error50302");
            }
            else if (BinDischarging.IntermediateChildPosKey == null)
            {
                // Error50303: Reference to a intermediate product was not assigned in the workflow node. (Referenz zu Zwischenprodukt wurde nicht im Workflowknoten zugeordnet.)
                msg = new Msg(this, eMsgLevel.Error, ClassName, "SetCodeNo", 20, "Error50303");
            }
            else
            {
                Guid intermediateChildPosID = (Guid)BinDischarging.IntermediateChildPosKey.EntityKeyValues[0].Value;
                List<DischargingItem> dischargingItems = DischargingItemManager.LoadDischargingItemList(intermediateChildPosID, BinDischarging.SourceInfoType);
                Guid itemIDVal = new Guid();
                Guid.TryParse(itemNo, out itemIDVal);
                CurrentDischargingItem = dischargingItems.FirstOrDefault(c => c.ItemID == itemIDVal);
                if (itemIDVal != Guid.Empty && CurrentDischargingItem != null)
                {
                    msg = DischargingItemNoValidator.ValidateInputNo(itemNo, intermediateChildPosID, CurrentDischargingItem.ProdorderPartslistPosRelationID, BinDischarging.SourceInfoType, DischargingItemNoValidatorBehaviorEnum.BINDischarging_NoOutwardBookings);
                    InputSourceCodeValidationMessage = msg;
                    if (msg.IsSucceded())
                    {
                        if (BinDischarging.CheckDischargedQuantity && CurrentScaleForWeighing != null)
                            ScaleValueBeforeDischarging.ValueT = CurrentScaleForWeighing.ActualValue.ValueT;
                            
                        CurrentACMethod.ValueT[Const_InputSourceCodes] = itemNo;
                        SendChangedACMethod();
                        SubscribeToProjectWorkCycle();
                    }
                }
                else
                {
                    // Error50304: Previous Inwardbooking on Facility or quant {0} not found!
                    msg = new Msg(this, eMsgLevel.Error, ClassName, "SetCodeNo", 90, "Error50304");
                }
            }

            return msg;
        }

        [ACMethodInfo("GetSelection", "en{'GetSelection'}de{'GetSelection'}", 502)]
        public object[] GetSelection()
        {
            if (BinDischarging == null || BinDischarging.IntermediateChildPosKey == null) return null;
            Guid intermediateChildPosID = (Guid)BinDischarging.IntermediateChildPosKey.EntityKeyValues[0].Value;
            return new object[] { intermediateChildPosID, BinDischarging.SourceInfoType };
        }

        [ACMethodInfo("OnScanEvent", "en{'OnScanEvent'}de{'OnScanEvent'}", 503)]
        public BarcodeSequenceBase OnScanEvent(Guid facilityID, Guid facilityChargeID, int scanSequence)
        {
            BarcodeSequenceBase sequence = new BarcodeSequenceBase();
            if (scanSequence == 1)
            {
                ManualPreparationSourceInfoTypeEnum? selectionMode = BinDischarging?.SourceInfoType;
                if (selectionMode.HasValue)
                {
                    if (selectionMode.Value == ManualPreparationSourceInfoTypeEnum.FacilityID)
                    {
                        // Info50043: Scan a bin or container. (Scannen Sie ein Behältnis.)
                        sequence.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnScanEvent", 10, "Info50043");
                    }
                    else if (selectionMode.Value == ManualPreparationSourceInfoTypeEnum.FacilityChargeID)
                    {
                        // Info50044: Please scan the Identifier of the mixture that you have prepared (ID of a quant). (Scannen Sie das Kennzeichen des Gemenges das Sie vorbereitet haben (ID des Quants).)
                        sequence.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnScanEvent", 20, "Info50044");
                    }
                }
                else
                {
                    // Info50045: Scan the Identifier of the mixture that you have prepared or the identifier of a bin or container. (Scannen Sie das Kennzeichen eines Gemenges oder Behältnisses das Sie vorbereitet haben.)
                    sequence.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnScanEvent", 30, "Info50045");
                }
                sequence.State = BarcodeSequenceBase.ActionState.ScanAgain;
            }
            else
            {
                if (facilityChargeID == Guid.Empty && facilityID == Guid.Empty)
                {
                    // Error50354: Unsupported command sequence!  (Nicht unterstützte Befehlsfolge!)
                    sequence.Message = new Msg(this, eMsgLevel.Error, ClassName, "OnScanEvent", 40, "Error50354");
                    sequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                }
                else
                {
                    string id = "";
                    if (facilityID != Guid.Empty)
                        id = facilityID.ToString();
                    else if (facilityChargeID != Guid.Empty)
                        id = facilityChargeID.ToString();

                    Msg msg = SetCodeNo(id) as Msg;
                    if (msg != null && msg.MessageLevel != eMsgLevel.Info)
                    {
                        sequence.Message = msg;
                        sequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                    }
                    else if (msg != null && msg.MessageLevel == eMsgLevel.Info)
                    {
                        sequence.Message = msg;
                        sequence.State = BarcodeSequenceBase.ActionState.Completed;
                    }
                    else
                    {
                        // Info50046: Discharge the prepared mixture or bin. (Entleeren Sie das Behältnis bzw. das vorbereitete Gemenge.)
                        sequence.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnScanEvent", 50, "Info50046");
                        sequence.State = BarcodeSequenceBase.ActionState.Completed;
                    }
                }
            }
            return sequence;
        }

        [ACMethodInfo("", "", 504)]
        public Msg ConfirmDischarge(bool skipCheck = false)
        {
            if (!WaitForConfirmDischarge.ValueT)
            {
                // Info50047: First, scan the prepared mixture or bin. (Scannen Sie zuerst das Behältnis bzw. das vorbereitete Gemenge.)
                return new Msg(this, eMsgLevel.Info, ClassName, "OnScanEvent", 60, "Info50047");
            }

            if (!skipCheck)
            {
                Msg msg = ValidateDischargedQuantity();
                if (msg != null)
                    return msg;
            }

            using (ACMonitor.Lock(_20015_LockValue))
            {
                _DischargeConfirmed = true;
            }

            SubscribeToProjectWorkCycle();

            return null;
        } 

        public Msg ValidateDischargedQuantity()
        {
            if (ApplicationManager.IsSimulationOn)
                return null;

            if (BinDischarging == null)
            {
                // Error50303: This discharging function was not invoked by a workflow node.
                return new Msg(this, eMsgLevel.Error, ClassName, "ValidateDischargedQuantity(10)", 10, "Error50302");
            }

            if (!BinDischarging.CheckDischargedQuantity)
                return null;

            if (CurrentDischargingItem == null)
            {
                string itemNo = CurrentACMethod.ValueT[Const_InputSourceCodes].ToString();
                Guid itemNoGuid = Guid.Empty;
                if (Guid.TryParse(itemNo, out itemNoGuid))
                {
                    Guid intermediateChildPosID = (Guid)BinDischarging.IntermediateChildPosKey.EntityKeyValues[0].Value;
                    List<DischargingItem> dischargingItems = DischargingItemManager.LoadDischargingItemList(intermediateChildPosID, BinDischarging.SourceInfoType);
                    CurrentDischargingItem = dischargingItems.FirstOrDefault(c => c.ItemID == itemNoGuid);
                }

                if (CurrentDischargingItem == null)
                {
                    // Error50304: Previous Inwardbooking on Facility or quant {0} not found!
                    return new Msg(this, eMsgLevel.Error, ClassName, "ValidateDischargedQuantity(20)", 20, "Error50304");
                }
            }

            if (CurrentScaleForWeighing == null)
            {
                //Error50356: No scale could be found for check weighing!! (Es konnte keine Waage zur Gewichtsprüfung gefunden werden!)
                return new Msg(this, eMsgLevel.Error, ClassName, "ValidateDischargedQuantity(30)", 30, "Error50356");
            }

            double targetQuantity = CurrentDischargingItem.InwardBookingQuantityUOM;
            double scaleWeightAfterDischarge = CurrentScaleForWeighing.ActualValue.ValueT;
            double dischargedQuantity = scaleWeightAfterDischarge - ScaleValueBeforeDischarging.ValueT;

            double toleranceMinus = BinDischarging.ToleranceMinus >= 0 ? BinDischarging.ToleranceMinus : (Math.Abs(BinDischarging.ToleranceMinus) / 100) * targetQuantity;
            double tolerancePlus = BinDischarging.TolerancePlus >= 0 ? BinDischarging.TolerancePlus : (Math.Abs(BinDischarging.TolerancePlus) / 100) * targetQuantity;

            if (dischargedQuantity >= (targetQuantity - toleranceMinus) && (targetQuantity + tolerancePlus) >= dischargedQuantity)
                return null;
            else
            {
                double targetScaleWeight = ScaleValueBeforeDischarging.ValueT + targetQuantity;
                double diff = scaleWeightAfterDischarge - targetScaleWeight;

                // Error50357:
                // Gewichtsprüfungsalarm: Die Waage hat ein Gewicht von {0} kg gemessen. Es müssten aber {1} kg angekommen sein. {2} kg Differenz sind zu hoch (Min-Tol.: {3}, Max-Tol.: {4}).
                // Weighing-Check-Alarm: The scale measures a total weight of {0} kg. But {1} kg material must appear in it. The difference of {2} kg is too high (Min-Tol.: {3}, Max-Tol.: {4}).
                return new Msg(this, eMsgLevel.Error, ClassName, "ValidateDischargedQuantity(40)", 40, "Error50357", Math.Round(scaleWeightAfterDischarge,2), Math.Round(targetScaleWeight, 2), Math.Round(diff, 2),
                                                                                                                                 Math.Round(toleranceMinus, 2), Math.Round(tolerancePlus, 2));
            }
        }

        #endregion

        #endregion

        #region Config
        protected ACPropertyConfigValue<string> _FuncScaleConfig;
        [ACPropertyConfig("en{'Assigned Scales'}de{'Zugeordnete Waagen'}")]
        public string FuncScaleConfig
        {
            get
            {
                return _FuncScaleConfig.ValueT;
            }
        }

        public PAEScaleBase CurrentScaleForWeighing
        {
            get
            {
                if (ScaleMappingHelper != null && ScaleMappingHelper.AssignedScales.Any())
                {
                    return ScaleMappingHelper.AssignedScales.FirstOrDefault();
                }
                else
                {
                    IPAMContScale scale = ParentACComponent as IPAMContScale;
                    return scale?.Scale;
                }
            }
        }

        private PAScaleMappingHelper<PAEScaleBase> _ScaleMappingHelper;
        public PAScaleMappingHelper<PAEScaleBase> ScaleMappingHelper
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_ScaleMappingHelper == null)
                        _ScaleMappingHelper = new PAScaleMappingHelper<PAEScaleBase>(this, this);
                }
                return _ScaleMappingHelper;
            }
        }
        #endregion

        #region Private

        protected static ACMethodWrapper CreateVirtualMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();

            method.ParameterValueList.Add(new ACValue(Const_InputSourceCodes, typeof(string), null, Global.ParamOption.Optional));
            paramTranslation.Add(Const_InputSourceCodes, "en{'Scanned Barcode'}de{'Gescannter Barcode'}");

            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();

            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }

        #endregion
    }
}
