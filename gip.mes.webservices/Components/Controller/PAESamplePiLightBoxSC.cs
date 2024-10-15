using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.webservices
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Scan-Controller for PAESamplePiLightBoxSC'}de{'Scan-Controller für PAESamplePiLightBoxSC'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.Required, false, false)]
    public class PAESamplePiLightBoxSC : PAScannedCompContrBase
    {
        public PAESamplePiLightBoxSC(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override void HandleBarcodeSequence(ACComponent component, BarcodeSequence sequence)
        {
            if (sequence == null)
                return;

            if (sequence.QuestionSequence == 0)
            {
                Msg question = new Msg("Do you want restart weighing cycle on the sample pi box?", component, eMsgLevel.Question, "", "", 26, eMsgButton.YesNo);
                sequence.AddQuestion(question);
                sequence.QuestionSequence = 1;
            }
            else
            {
                BarcodeEntity entity = sequence.Sequence?.LastOrDefault();

                if (entity != null && entity.MsgResult.HasValue && entity.MsgResult == Global.MsgResult.Yes)
                {
                    PAESamplePiLightBox lightBox = component as PAESamplePiLightBox;
                    if (lightBox == null)
                        sequence.Message = new Msg(eMsgLevel.Error, "The scanned code is not from Sample Pi Light box!");

                    ApplicationManager.ApplicationQueue.Add(() => lightBox.ResetWeighingCycle());

                    sequence.Message = new Msg(eMsgLevel.Info, "Restart weighing cycle is initialized.");
                    sequence.State = datamodel.BarcodeSequenceBase.ActionState.Completed;
                }
                else
                {
                    sequence.Message = new Msg(eMsgLevel.Info, "Restart weighing cycle is cancelled.");
                    sequence.State = datamodel.BarcodeSequenceBase.ActionState.Cancelled;
                }
                
            }
        }

        protected override Type OnGetControlledType()
        {
            return typeof(PAESamplePiLightBox);
        }
    }
}
