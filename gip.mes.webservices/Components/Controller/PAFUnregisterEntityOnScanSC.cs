using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.webservices
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Scan-Controller for PAFUnregisterEntityOnScanSC'}de{'Scan-Controller für PAFUnregisterEntityOnScanSC'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.Required, false, false)]
    public class PAFUnregisterEntityOnScanSC : PAScannedCompContrBase
    {
        public PAFUnregisterEntityOnScanSC(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool CanHandleBarcodeSequence(ACComponent component, BarcodeSequence sequence)
        {
            return true;
        }

        public override void HandleBarcodeSequence(ACComponent component, BarcodeSequence sequence)
        {
            BarcodeEntity entityFacility, entityCharge;
            ParentDecoder.GetFacilityEntitiesFromSequence(sequence, out entityFacility, out entityCharge);
            Guid facilityID, facilityChargeID;
            ParentDecoder.GetGuidFromFacilityEntities(entityFacility, entityCharge, out facilityID, out facilityChargeID);

            BarcodeSequenceBase result = component.ExecuteMethod(nameof(PAFUnregisterEntityOnScan.OnScanEvent),
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

        protected override Type OnGetControlledType()
        {
            return typeof(PAFUnregisterEntityOnScan);
        }
    }
}
