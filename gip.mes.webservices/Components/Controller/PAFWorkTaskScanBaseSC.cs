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
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Scan-Controller for PAFWorkTaskScanBase'}de{'Scan-Controller für PAFWorkTaskScanBase'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.Required, false, false)]
    public class PAFWorkTaskScanBaseSC : PAScannedCompContrBase
    {
        #region c'tors
        new public const string ClassName = "PAFWorkTaskScanBaseSC";

        public PAFWorkTaskScanBaseSC(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        #endregion


        #region Properties
        protected override Type OnGetControlledType()
        {
            return typeof(PAFWorkTaskScanBase);
        }
        #endregion 


        #region Methods
        public override bool CanHandleBarcodeSequence(ACComponent component, BarcodeSequence sequence)
        {
            return true;
            //return base.CanHandleBarcodeSequence(component, sequence);
        }

        public override void HandleBarcodeSequence(ACComponent component, BarcodeSequence sequence)
        {
            BarcodeEntity entityFacility, entityCharge;
            ParentDecoder.GetFacilityEntitiesFromSequence(sequence, out entityFacility, out entityCharge);
            Guid facilityID, facilityChargeID;
            ParentDecoder.GetGuidFromFacilityEntities(entityFacility, entityCharge, out facilityID, out facilityChargeID);

            WorkTaskScanResult result = component.ACUrlCommand("!OnScanEvent",
                new BarcodeSequenceBase() { State = sequence.State, Message = sequence.Message, QuestionSequence = sequence.QuestionSequence },
                sequence.State == BarcodeSequenceBase.ActionState.Selection ? ConvertWFInfoToPA(sequence.Sequence.LastOrDefault().SelectedOrderWF) : null,
                facilityChargeID, 
                sequence.Sequence.Count,
                sequence.State == BarcodeSequenceBase.ActionState.Question ? (short?)sequence.Sequence.LastOrDefault().MsgResult : null) as WorkTaskScanResult;
            if (result != null)
            {
                if (result.Result.State == BarcodeSequenceBase.ActionState.Selection)
                {
                    BarcodeEntity barcodeEntity = new BarcodeEntity();
                    List<ProdOrderPartslistWFInfo> orderInfoList = new List<ProdOrderPartslistWFInfo>();
                    FillOrderWFInfo(result.OrderInfos, orderInfoList);
                    barcodeEntity.OrderWFInfos = orderInfoList.ToArray();
                    sequence.Message = result.Result.Message;
                    sequence.State = result.Result.State;
                    sequence.Sequence.Add(barcodeEntity);
                }
                else if (result.Result.Message.MessageLevel == eMsgLevel.Question)
                    sequence.AddQuestion(result.Result.Message);
                else
                {
                    sequence.State = result.Result.State;
                    sequence.Message = result.Result.Message;
                }
            }
            else
            {
                // Error50352: No response from process function!
                sequence.Message = new Msg(this, eMsgLevel.Error, ClassName, "HandleBarcodeSequence", 10, "Error50352");
                sequence.State = BarcodeSequence.ActionState.Cancelled;
            }
        }

        private PAProdOrderPartslistWFInfo ConvertWFInfoToPA(ProdOrderPartslistWFInfo wfInfo)
        {
            if (wfInfo == null)
                return null;
            return new PAProdOrderPartslistWFInfo()
            {
                POPId = wfInfo.ProdOrderPartslist != null ? wfInfo.ProdOrderPartslist.ProdOrderPartslistID : Guid.Empty,
                IntermPOPPosId = wfInfo.Intermediate != null ? wfInfo.Intermediate.ProdOrderPartslistPosID : Guid.Empty,
                IntermChildPOPPosId = wfInfo.IntermediateBatch != null ? wfInfo.IntermediateBatch.ProdOrderPartslistPosID : Guid.Empty,
                ACUrlWF = wfInfo.ACUrlWF,
                ForRelease = wfInfo.ForRelease
            };
        }

        private void FillOrderWFInfo(PAProdOrderPartslistWFInfo[] paOrderWFInfos, List<ProdOrderPartslistWFInfo> orderWFList2Fill)
        {
            if (paOrderWFInfos == null || !paOrderWFInfos.Any())
                return;
            VBWebService vbWebService = new VBWebService();
            try
            {
                using (var dbApp = new DatabaseApp())
                {
                    foreach (PAProdOrderPartslistWFInfo orderWFInfo in paOrderWFInfos)
                    {
                        if (orderWFInfo.POPId == Guid.Empty)
                            continue;
                        ProdOrderPartslistWFInfo pwInfo = new ProdOrderPartslistWFInfo() { ForRelease = orderWFInfo.ForRelease };
                        if (orderWFInfo.WFMethod != null)
                        {
                            pwInfo.WFMethod = orderWFInfo.WFMethod.Clone() as ACMethod;
                            pwInfo.WFMethod.FullSerialization = true;
                        }

                        pwInfo.ProdOrderPartslist = vbWebService.ConvertToWSProdOrderPartslists(VBWebService.s_cQry_GetProdOrderPartslist(dbApp, orderWFInfo.POPId)).FirstOrDefault();
                        if (orderWFInfo.IntermPOPPosId != Guid.Empty)
                            pwInfo.Intermediate = vbWebService.ConvertToWSProdOrderPLIntermediates(VBWebService.s_cQry_GetProdOrderPLIntermediates(dbApp, null, orderWFInfo.IntermPOPPosId)).FirstOrDefault();
                        if (orderWFInfo.IntermChildPOPPosId != Guid.Empty)
                            pwInfo.IntermediateBatch = vbWebService.ConvertToWSProdOrderIntermBatches(VBWebService.s_cQry_GetProdOrderIntermBatches(dbApp, null, orderWFInfo.IntermChildPOPPosId)).FirstOrDefault();
                        orderWFList2Fill.Add(pwInfo);
                    }
                }
            }
            catch (Exception e)
            {
                this.Messages.LogException(this.GetACUrl(), "FillOrderWFInfo()", e);
            }
        }

        #endregion
    }

}
