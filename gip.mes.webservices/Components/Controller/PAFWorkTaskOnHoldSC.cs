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
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Scan-Controller for PAFWorkTaskOnHold'}de{'Scan-Controller für PAFWorkTaskOnHold'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.Required, false, false)]
    public class PAFWorkTaskOnHoldSC : PAFWorkTaskScanBaseSC
    {
        public PAFWorkTaskOnHoldSC(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        protected override Type OnGetControlledType()
        {
            return typeof(PAFWorkTaskOnHold);
        }

        public override void HandleBarcodeSequence(ACComponent component, BarcodeSequence sequence)
        {
            if (sequence.Sequence.Count > 1)
            {
                ProdOrderBatch batch = sequence.Sequence.Where(c => c.POBatch != null).FirstOrDefault()?.POBatch;
                core.webservices.ACClass pafWorkTaskOnHold = sequence.Sequence.Where(c => c.ACClass != null).FirstOrDefault()?.ACClass;
                if (batch != null && pafWorkTaskOnHold != null)
                {
                    WorkTaskScanResult prodOrderWFInfoList = component.ExecuteMethod(nameof(PAFWorkTaskScanBase.GetOrderInfos)) as WorkTaskScanResult;
                    if (prodOrderWFInfoList != null && prodOrderWFInfoList.OrderInfos != null && prodOrderWFInfoList.OrderInfos.Any())
                    {
                        using (DatabaseApp dbApp = new DatabaseApp())
                        {
                            Guid[] temp = prodOrderWFInfoList.OrderInfos.Select(t => t.POPPosId).ToArray();

                            var result = dbApp.ProdOrderPartslistPos
                                          .Where(c => temp.Any(x => x == c.ProdOrderPartslistPosID)
                                                   && c.ProdOrderBatchID == batch.ProdOrderBatchID)
                                          .OrderBy(c => c.Sequence)
                                          .ToArray()
                                          .Select(x => new Tuple<mes.datamodel.ProdOrderPartslistPos, PAProdOrderPartslistWFInfo>(x, prodOrderWFInfoList
                                                                                                                       .OrderInfos.FirstOrDefault(c => x.ProdOrderPartslistPosID == c.POPPosId))).FirstOrDefault();
                            if (result != null)
                            {
                                Global.MsgResult? questionResult = sequence.Sequence.FirstOrDefault(c => c.MsgResult != null)?.MsgResult;

                                if (questionResult == null)
                                {
                                    //Question50118: Are you sure that you want to activate the order?
                                    Msg msg = new Msg(this, eMsgLevel.Question, nameof(PAFWorkTaskOnHoldSC), nameof(HandleBarcodeSequence), 55, "Question50119");
                                    msg.MessageButton = eMsgButton.YesNo;
                                    sequence.AddQuestion(msg);
                                    return;
                                }

                                if (questionResult == Global.MsgResult.Yes)
                                {
                                    Msg msgResult = component.ExecuteMethod(nameof(PAFWorkTaskScanBase.OccupyReleaseProcessModule), result.Item2.ACUrlWF, result.Item2.ForRelease) as Msg;
                                    if (msgResult != null && msgResult.MessageLevel > eMsgLevel.Info)
                                        sequence.Message = msgResult;
                                    else
                                    {
                                        sequence.Message = new Msg(eMsgLevel.Info, "OK");
                                        sequence.State = BarcodeSequenceBase.ActionState.Completed;
                                    }
                                }
                                else
                                {
                                    sequence.Message = new Msg(eMsgLevel.Info, "OK");
                                    sequence.State = BarcodeSequenceBase.ActionState.Completed;
                                }

                                return;
                            }

                        }
                    }
                }
            }

            base.HandleBarcodeSequence(component, sequence);
            if (sequence.State == BarcodeSequenceBase.ActionState.Selection)
                sequence.State = BarcodeSequenceBase.ActionState.SelectionScanAgain;
        }
    }
}
