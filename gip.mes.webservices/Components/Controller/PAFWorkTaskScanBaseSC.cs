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

            WorkTaskScanResult result = component.ExecuteMethod(nameof(PAFWorkTaskScanBase.OnScanEvent),
                new BarcodeSequenceBase() { State = sequence.State, Message = sequence.Message, QuestionSequence = sequence.QuestionSequence },
                sequence.State >= BarcodeSequenceBase.ActionState.Question ? ConvertWFInfoToPA(sequence.Sequence.LastOrDefault().SelectedOrderWF) : null,
                facilityChargeID, 
                sequence.Sequence.Count,
                sequence.State == BarcodeSequenceBase.ActionState.Question ? (short?)sequence.Sequence.LastOrDefault().MsgResult : null,
                sequence.Sequence.LastOrDefault().WFMethod,
                (sequence.Sequence.FirstOrDefault(c => c.ACClass != null) != null ? sequence.Sequence.FirstOrDefault(c => c.ACClass != null).MachineMalfunction : null)) as WorkTaskScanResult;
            if (result != null)
            {
                if (result.Result.State == BarcodeSequenceBase.ActionState.Selection
                    || result.Result.State == BarcodeSequenceBase.ActionState.FastSelection)
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

                PAProcessModuleVB paProcessModule = component.FindParentComponent<PAProcessModuleVB>(c => c is PAProcessModuleVB);
                if (paProcessModule != null)
                {
                    BarcodeEntity barcodeEntity = sequence.Sequence.FirstOrDefault(c => c.ACClass != null);
                    if (barcodeEntity != null)
                        barcodeEntity.MachineAvailability = paProcessModule.AvailabilityState.ValueT;
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
                ForRelease = wfInfo.InfoState > POPartslistInfoState.None,
                Pause = wfInfo.InfoState == POPartslistInfoState.Pause,
            };
        }

        private void FillOrderWFInfo(PAProdOrderPartslistWFInfo[] paOrderWFInfos, List<ProdOrderPartslistWFInfo> orderWFList2Fill)
        {
            if (paOrderWFInfos == null || !paOrderWFInfos.Any())
                return;
            VBWebService vbWebService = null;

            PAJsonServiceHostVB jsonHost = FindParentComponent<PAJsonServiceHostVB>(c => c is PAJsonServiceHostVB);
            if (jsonHost != null)
                vbWebService = jsonHost.GetWebServiceInstance() as VBWebService;
                
            if (vbWebService == null)
                vbWebService = new VBWebService();

            try
            {
                using (var dbApp = new DatabaseApp())
                {
                    var poPLIDs = paOrderWFInfos.Select(c => c.POPId);
                    var intermPoPLPosIDs = paOrderWFInfos.Select(c => c.IntermPOPPosId);
                    var intermChildPoPLPosIDs = paOrderWFInfos.Select(c => c.IntermChildPOPPosId);

                    var intermChildPLPos = dbApp.ProdOrderPartslistPos.Include("Material")
                                                       .Include("ProdOrderBatch")
                                                       .Include("ProdOrderPartslist")
                                                       .Include("ProdOrderPartslist.ProdOrder")
                                                       .Include("ProdOrderPartslist.Partslist.Material")
                                                       .Include("ProdOrderPartslist.Partslist.Material.BaseMDUnit")
                                                       .Include("MDUnit")
                                                       .Include("ProdOrderPartslistPos1_ParentProdOrderPartslistPos")
                                                       .Include("ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos")
                                                       .Include("ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.SourceProdOrderPartslistPos")
                                                       .Include("ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.SourceProdOrderPartslistPos.Material")
                                                       .Include("Material.MaterialWFRelation_SourceMaterial")
                                                       .Where(c => (        c.ProdOrderBatch != null
                                                                        && (c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex < (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                                                                            || c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex > (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled))
                                                                        && intermChildPoPLPosIDs.Contains(c.ProdOrderPartslistPosID)
                                                       )
                                                       .OrderBy(x => x.Sequence)
                                                       .ThenBy(c => c.Material != null ? c.Material.MaterialNo : "").ToArray();

                    var poPartslist = intermChildPLPos.Select(c => c.ProdOrderPartslist).ToList();
                    var missingPOPartslist = poPLIDs.Except(poPartslist.Select(c => c.ProdOrderPartslistID));
                    if (missingPOPartslist != null && missingPOPartslist.Any())
                    {
                        var missingPOPL = dbApp.ProdOrderPartslist.Include("ProdOrder")
                                                                   .Include("Partslist")
                                                                   .Include("Partslist.Material")
                                                                   .Include("Partslist.Material.BaseMDUnit")
                                                                   .Where(c => missingPOPartslist.Contains(c.ProdOrderPartslistID))
                                                                   .OrderByDescending(x => x.UpdateDate).ToArray();
                        poPartslist.AddRange(missingPOPL);
                    }


                    var intermPOPLPos = intermChildPLPos.Select(c => c.TopParentPartslistPos).ToList();
                    var missingintermPOPLPos = intermPoPLPosIDs.Except(intermPOPLPos.Select(c => c.ProdOrderPartslistPosID));
                    if (missingintermPOPLPos != null && missingintermPOPLPos.Any())
                    {
                        var missingPOPLPos = dbApp.ProdOrderPartslistPos.Include("Material")
                                                                   .Include("Material.BaseMDUnit")
                                                                   .Include("ProdOrderPartslist")
                                                                   .Include("ProdOrderPartslist.ProdOrder")
                                                                   .Include("ProdOrderPartslist.Partslist")
                                                                   .Include("ProdOrderPartslist.Partslist.Material")
                                                                   .Include("ProdOrderPartslist.Partslist.Material.BaseMDUnit")
                                                                   .Include("ProdOrderPartslistPos1_ParentProdOrderPartslistPos")
                                                                   .Include("Material.MaterialWFRelation_SourceMaterial")
                                                                   .Where(c => (missingintermPOPLPos.Contains(c.ProdOrderPartslistPosID)))
                                                                   .OrderBy(x => x.Sequence)
                                                                   .ThenBy(c => c.Material != null ? c.Material.MaterialNo : "").ToArray();

                        intermPOPLPos.AddRange(missingPOPLPos);
                    }

                    foreach (PAProdOrderPartslistWFInfo orderWFInfo in paOrderWFInfos)
                    {
                        if (orderWFInfo.POPId == Guid.Empty)
                            continue;
                        POPartslistInfoState infoState = POPartslistInfoState.None;
                        if (orderWFInfo.ForRelease)
                        {
                            infoState = POPartslistInfoState.Release;
                        }

                        ProdOrderPartslistWFInfo pwInfo = new ProdOrderPartslistWFInfo() { InfoState = infoState, ACUrlWF = orderWFInfo.ACUrlWF };
                        if (orderWFInfo.WFMethod != null)
                        {
                            pwInfo.WFMethod = orderWFInfo.WFMethod.Clone() as ACMethod;
                            pwInfo.WFMethod.FullSerialization = true;

                            var resultToRemoveList = pwInfo.WFMethod.ResultValueList.Where(c => !c.IsPrimitiveType).ToArray();
                            foreach (var resultToRemove in resultToRemoveList)
                            {
                                pwInfo.WFMethod.ResultValueList.Remove(resultToRemove);
                            }

                            var paramToRemoveList = pwInfo.WFMethod.ParameterValueList.Where(c => !c.IsPrimitiveType).ToArray();
                            foreach (var paramToRemove in paramToRemoveList)
                            {
                                pwInfo.WFMethod.ParameterValueList.Remove(paramToRemove);
                            }
                        }

                        //pwInfo.PostingQSuggestionMode = orderWFInfo.PostingQSuggestionMode;
                        //pwInfo.PostingQSuggestionMode2 = orderWFInfo.PostingQSuggestionMode2;

                        pwInfo.ProdOrderPartslist = vbWebService.ConvertToWSProdOrderPartslists(poPartslist.Where(c => c.ProdOrderPartslistID == orderWFInfo.POPId).AsQueryable()).FirstOrDefault();
                        if (orderWFInfo.IntermPOPPosId != Guid.Empty)
                            pwInfo.Intermediate = vbWebService.ConvertToWSProdOrderPLIntermediates(intermPOPLPos.Where(c => c.ProdOrderPartslistPosID == orderWFInfo.IntermPOPPosId).AsQueryable()).FirstOrDefault();
                        if (orderWFInfo.IntermChildPOPPosId != Guid.Empty)
                            pwInfo.IntermediateBatch = vbWebService.ConvertToWSProdOrderIntermBatches(intermChildPLPos.Where(c => c.ProdOrderPartslistPosID == orderWFInfo.IntermChildPOPPosId).AsQueryable()).FirstOrDefault();
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
