using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Bin selection'}de{'Behältnis auswählen'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, PWBinSelection.PWClassName, true, BSOConfig = "BSOBinSelection")]
    public class PAFBinSelection : PAProcessFunction
    {
        #region constants

        public const string Const_InputSourceCodes = "InputSourceCodes";
        public const string Const_Break = "Break";

        #endregion

        #region Constructors

        public const string ClassName = "PAFBinSelection";

        static PAFBinSelection()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFBinSelection), ACStateConst.TMStart, CreateVirtualMethod("BinSelection", "en{'Bin selection'}de{'Behältnis auswählen'}", typeof(PWBinSelection)));
            RegisterExecuteHandler(typeof(PAFBinSelection), HandleExecuteACMethod_PAFBinSelection);
        }

        public PAFBinSelection(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            DischargingItemNoValidator = new DischargingItemNoValidator(this, ClassName);
            return base.ACInit(startChildMode);
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            DischargingItemNoValidator = null;
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties

        public DischargingItemNoValidator DischargingItemNoValidator { get; private set; }

        private Msg _InputSourceCodeValidationMessage;
        [ACPropertyInfo(999)]
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


        [ACPropertyInfo(500)]
        public PWBinSelection BinSelection
        {
            get
            {
                if (CurrentTask != null && CurrentTask.ValueT != null)
                    return CurrentTask.ValueT as PWBinSelection;
                return null;
            }
        }

        #endregion

        #region Methods

        #region Methods -> ACState
        [ACMethodAsync("Process", "en{'Start'}de{'Start'}", (short)MISort.Start, false)]
        public override ACMethodEventArgs Start(ACMethod acMethod)
        {
            return base.Start(acMethod);
        }

        public override void SMStarting()
        {
            base.SMStarting();
            InputSourceCodeValidationMessage = null;
        }

        public override void SMRunning()
        {
            UnSubscribeToProjectWorkCycle();
            bool isCodeRecieved =
                CurrentACMethod != null
                && CurrentACMethod.ValueT != null
                && CurrentACMethod.ValueT[Const_InputSourceCodes] != null;

            if (isCodeRecieved)
            {
                if (InputSourceCodeValidationMessage == null)
                {
                    Guid intermediateChildPosID = (Guid)BinSelection.IntermediateChildPosKey.EntityKeyValues[0].Value;
                    InputSourceCodeValidationMessage = DischargingItemNoValidator.ValidateInputNo(CurrentACMethod.ValueT[Const_InputSourceCodes].ToString(), intermediateChildPosID, null, BinSelection.SourceInfoType, DischargingItemNoValidatorBehaviorEnum.BINSelection_NoInwardBookings);
                }
            }

            bool inputCodeSuccess = InputSourceCodeValidationMessage != null && InputSourceCodeValidationMessage.IsSucceded();
            bool breakCodeRecieved = CurrentACMethod.ValueT[Const_Break] != null && (bool)CurrentACMethod.ValueT[Const_Break];
            if (inputCodeSuccess || breakCodeRecieved)
                if (ACStateConverter != null)
                    CurrentACState = ACStateConverter.GetNextACState(this);
                else
                    CurrentACState = PAFuncStateConvBase.GetDefaultNextACState(CurrentACState);
        }

        public override void SMCompleted()
        {
            base.SMCompleted();
            InputSourceCodeValidationMessage = null;
            CleanUpInputParams();
        }

        public override void SMAborted()
        {
            base.SMAborted();
            InputSourceCodeValidationMessage = null;
            CleanUpInputParams();
        }

        public override void SMIdle()
        {
            base.SMIdle();
            InputSourceCodeValidationMessage = null;
            CleanUpInputParams();
        }

        public override void SMResetting()
        {
            base.SMResetting();
            InputSourceCodeValidationMessage = null;
            CleanUpInputParams();
        }

        public override void SMRestarting()
        {
            base.SMRestarting();
            InputSourceCodeValidationMessage = null;
            CleanUpInputParams();
        }
        #endregion


        #region Methods -> Customer

        [ACMethodInfo("GetSelection", "en{'GetSelection'}de{'GetSelection'}", 502)]
        public object[] GetSelection()
        {
            if (BinSelection == null || BinSelection.IntermediateChildPosKey == null) return null;
            Guid intermediateChildPosID = (Guid)BinSelection.IntermediateChildPosKey.EntityKeyValues[0].Value;
            return new object[] { intermediateChildPosID, BinSelection.SourceInfoType };
        }

        [ACMethodInfo("SetCodeNo", "en{'SetCodeNo'}de{'SetCodeNo'}", 500)]
        public Msg SetCodeNo(string itemNo)
        {
            if (BinSelection == null)
            {
                // Error50359: This discharging function was not invoked by a workflow node. (Diese Entleerfunktion wurde nicht durch einen Worfkowknoten gestartet.)
                InputSourceCodeValidationMessage = new Msg(this, eMsgLevel.Error, ClassName, "SetCodeNo", 10, "Error50358");
            }
            else if (BinSelection.IntermediateChildPosKey == null)
            {
                // Error50359: Reference to a intermediate product was not assigned in the workflow node. (Referenz zu Zwischenprodukt wurde nicht im Workflowknoten zugeordnet.)
                InputSourceCodeValidationMessage = new Msg(this, eMsgLevel.Error, ClassName, "SetCodeNo", 20, "Error50359");
            }
            else
            {
                Guid intermediateChildPosID = (Guid)BinSelection.IntermediateChildPosKey.EntityKeyValues[0].Value;
                InputSourceCodeValidationMessage = DischargingItemNoValidator.ValidateInputNo(itemNo, intermediateChildPosID, null, BinSelection.SourceInfoType, DischargingItemNoValidatorBehaviorEnum.BINSelection_NoInwardBookings);
                if (InputSourceCodeValidationMessage.IsSucceded())
                {
                    CurrentACMethod.ValueT[Const_InputSourceCodes] = itemNo;
                    SubscribeToProjectWorkCycle();
                }
            }

            return InputSourceCodeValidationMessage;
        }

        [ACMethodInfo("BreakBinSelection", "en{'Break'}de{'Abbrechen'}", 501)]
        public Msg BreakBinSelection()
        {
            if (BinSelection == null)
            {
                // Error50359: This discharging function was not invoked by a workflow node. (Diese Entleerfunktion wurde nicht durch einen Worfkowknoten gestartet.)
                return new Msg(this, eMsgLevel.Error, ClassName, "BreakBinSelection", 10, "Error50358");
            }
            else if (BinSelection.IntermediateChildPosKey == null)
            {
                // Error50359: Reference to a intermediate product was not assigned in the workflow node. (Referenz zu Zwischenprodukt wurde nicht im Workflowknoten zugeordnet.)
                return new Msg(this, eMsgLevel.Error, ClassName, "BreakBinSelection", 20, "Error50359");
            }

            CurrentACMethod.ValueT[Const_Break] = true;
            SubscribeToProjectWorkCycle();
            // Info50041: The bin selection has been completed. (Die Behälterauswahl wurde beendet.)
            return new Msg(this, eMsgLevel.Info, ClassName, "BreakBinSelection", 30, "Info50041");
        }


        [ACMethodInfo("OnScanEvent", "en{'OnScanEvent'}de{'OnScanEvent'}", 503)]
        public BarcodeSequenceBase OnScanEvent(Guid facilityID, Guid facilityChargeID, int scanSequence)
        {
            BarcodeSequenceBase sequence = new BarcodeSequenceBase();
            if (scanSequence == 1)
            {
                ManualPreparationSourceInfoTypeEnum? selectionMode = BinSelection?.SourceInfoType;
                if (selectionMode.HasValue)
                {
                    if (selectionMode.Value == ManualPreparationSourceInfoTypeEnum.FacilityID)
                    {
                        // Info50048: Scan a bin or container. (Scannen Sie ein Behältnis.)
                        sequence.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnScanEvent", 10, "Info50048");
                        sequence.State = BarcodeSequenceBase.ActionState.ScanAgain;
                    }
                    else if (selectionMode.Value == ManualPreparationSourceInfoTypeEnum.FacilityChargeID)
                    {
                        // TODO: This makes no sense to scan a Barcode of a FacilityCharge. A new FacilityCharge must be generated instead
                        // and printed with a new barcode!
                        //sequence.Message = new Msg(eMsgLevel.Info, "OK: Bitte Charge scannen!");
                        sequence.Message = new Msg(eMsgLevel.Info, "TODO: Not implemented so far");
                        sequence.State = BarcodeSequenceBase.ActionState.Completed;
                    }
                }
                else
                {
                    // Info50048: Scan a bin or container. (Scannen Sie ein Behältnis.)
                    sequence.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnScanEvent", 30, "Info50048");
                    sequence.State = BarcodeSequenceBase.ActionState.ScanAgain;
                }
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
                        // Info50049:  This bin or container has been selected. (Das Behältnis wurde ausgewählt.)
                        sequence.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnScanEvent", 50, "Info50049");
                        sequence.State = BarcodeSequenceBase.ActionState.Completed;
                    }
                }
            }
            return sequence;
        }

        public void CleanUpInputParams()
        {
            if (CurrentACMethod.ValueT != null)
                CurrentACMethod.ValueT[Const_InputSourceCodes] = null;
        }
        #endregion


        #region Methods -> Handle & Create
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
                case "BreakBinSelection":
                    result = BreakBinSelection();
                    return true;
                case PAFWorkTaskScanBase.MN_OnScanEvent:
                    result = OnScanEvent((Guid)acParameter[0], (Guid)acParameter[1], (int)acParameter[2]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PAFBinSelection(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessFunction(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        #region Private


        protected static ACMethodWrapper CreateVirtualMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();

            method.ParameterValueList.Add(new ACValue(Const_InputSourceCodes, typeof(string), null, Global.ParamOption.Optional));
            paramTranslation.Add(Const_InputSourceCodes, "en{'Scanned Barcode'}de{'Gescannter Barcode'}");

            method.ParameterValueList.Add(new ACValue(Const_Break, typeof(bool), null, Global.ParamOption.Optional));
            paramTranslation.Add(Const_Break, "en{'Break'}de{'Abbrechen'}");

            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();

            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }
        #endregion
        #endregion
        
        #endregion
    }
}
