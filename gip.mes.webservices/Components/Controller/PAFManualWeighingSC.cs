using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.processapplication;
using gip.mes.datamodel;

namespace gip.mes.webservices
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Scan-Controller for PAFManualWeighing'}de{'Scan-Controller für PAFManualWeighing'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.Required, false, false)]
    public class PAFManualWeighingSC : PAScannedCompContrBase
    {
        #region c'tors
        new public const string ClassName = "PAFManualWeighingSC";

        public PAFManualWeighingSC(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        #endregion


        #region Properties
        protected override Type OnGetControlledType()
        {
            return typeof(PAFManualWeighing);
        }
        #endregion 


        #region Methods
        public override void HandleBarcodeSequence(ACComponent component, BarcodeSequence sequence)
        {
            BarcodeEntity entityFacility, entityCharge;
            ParentDecoder.GetFacilityEntitiesFromSequence(sequence, out entityFacility, out entityCharge);
            Guid facilityID, facilityChargeID;
            ParentDecoder.GetGuidFromFacilityEntities(entityFacility, entityCharge, out facilityID, out facilityChargeID);

            BarcodeSequenceBase result = component.ACUrlCommand(ACUrlHelper.Delimiter_InvokeMethod + PAFWorkTaskScanBase.MN_OnScanEvent,
                new BarcodeSequenceBase() { State = sequence.State, Message = sequence.Message, QuestionSequence = sequence.QuestionSequence },
                sequence.PreviousLotConsumed,
                facilityID, facilityChargeID, sequence.Sequence.Count,
                sequence.State == BarcodeSequenceBase.ActionState.Question ? (short?)sequence.Sequence.LastOrDefault().MsgResult : null) as BarcodeSequenceBase;
            if (result != null)
            {
                if (result.Message != null && result.Message.MessageLevel == eMsgLevel.Question)
                    sequence.AddQuestion(result.Message);
                else
                {
                    sequence.State = result.State;
                    sequence.Message = result.Message;
                }
            }
            else
            {
                // Error50352: No response from process function!
                sequence.Message = new Msg(this, eMsgLevel.Error, ClassName, "HandleBarcodeSequence", 10, "Error50352");
                sequence.State = BarcodeSequence.ActionState.Cancelled;
            }
        }

        //protected virtual void OnHandleManualWeighingTask(ACComponent resolvedComponent, BarcodeSequence sequence, BarcodeEntity entityFacility, BarcodeEntity entityCharge)
        //{

        //if (sequence.Sequence.Count == 1)
        //{
        //    sequence.Message = new Msg("OK: Bitte Charge scannen!", this, eMsgLevel.Info, ClassName, "OnHandleNextBarcodeSequence", 42);
        //    sequence.State = BarcodeSequence.ActionState.ScanAgain;
        //}
        //else
        //{
        //    if (entityCharge == null && entityFacility == null)
        //    {
        //        sequence.Message = new Msg(eMsgLevel.Error, "Nicht unterstütze Befehlsfolge");
        //        sequence.State = BarcodeSequence.ActionState.Cancelled;
        //        return;
        //    }

        //    if (sequence.State == BarcodeSequence.ActionState.Question)
        //    {
        //        BarcodeEntity questionResult = sequence.Sequence.LastOrDefault();
        //        OnHandleManualWeighingTaskQuestion(resolvedComponent, sequence, entityCharge, questionResult);
        //    }
        //    else
        //        OnHandleManualWeighingTaskQuestion(resolvedComponent, sequence, entityCharge, null);
        //}
        //}

        //protected virtual void OnHandleManualWeighingTaskQuestion(ACComponent resolvedComponent, BarcodeSequence sequence, BarcodeEntity entityCharge, BarcodeEntity question)
        //{
        //    bool forceSetFacilityCharge = false;

        //    if (question != null && question.MsgResult == Global.MsgResult.Yes)
        //    {
        //        forceSetFacilityCharge = true;
        //    }
        //    else if (question != null)
        //    {
        //        sequence.Message = new Msg(eMsgLevel.Info, "Task cancelled!");
        //        sequence.State = BarcodeSequence.ActionState.Cancelled;
        //        return;
        //    }

        //    Msg msg = resolvedComponent.ExecuteMethod("LotChange", entityCharge.FacilityCharge.FacilityChargeID, sequence.PreviousLotConsumed, forceSetFacilityCharge) as Msg;
        //    if (msg != null)
        //    {
        //        if (msg.MessageLevel == eMsgLevel.Question)
        //        {
        //            // TODO: Interpretation Queation
        //            sequence.AddQuestion(msg);
        //        }
        //        else
        //        {
        //            sequence.Message = msg;
        //            sequence.State = BarcodeSequence.ActionState.Cancelled;
        //        }
        //    }
        //    else
        //    {
        //        //sequence.Message = new Msg("Charge gewechselt", this, eMsgLevel.Info, ClassName, "OnHandleNextBarcodeSequence", 42);
        //        sequence.Message = new Msg(eMsgLevel.Info, "Charge gewechselt");
        //        sequence.State = BarcodeSequence.ActionState.Completed;
        //    }
        //}

        #endregion
    }

}
