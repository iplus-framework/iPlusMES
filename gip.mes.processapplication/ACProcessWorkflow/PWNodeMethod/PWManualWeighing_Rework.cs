using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    //TODO: unify code for rework activation and reworkstatus
    public partial class PWManualWeighing
    {
        [ACMethodInfo("", "", 9999)]
        public Msg ActivateRework()
        {
            IEnumerable<PWManualWeighing> manualWeighingsForRework = ParentACComponent?.FindChildComponents<PWManualWeighing>(c => c is PWManualWeighing).Where(c => !string.IsNullOrEmpty(c.ReworkMaterialNo) && c.ReworkQuantityPercentage > 0);
            if (manualWeighingsForRework == null)
            {
                return new Msg(eMsgLevel.Error, "There is no any manual weighing node that is configured for rework.");
            }

            foreach (PWManualWeighing manualWeighing in manualWeighingsForRework)
            {
                manualWeighing.AdjustProdOrderForRework();
            }

            return null;
        }

        private void AdjustProdOrderForRework()
        {
            if (string.IsNullOrEmpty(ReworkMaterialNo) || ReworkQuantityPercentage <= 0)
                return;

            PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
            // If dosing is not for production, then do nothing
            if (pwMethodProduction == null)
                return;

            if (ProdOrderManager == null)
            {
                return;
            }

            using (var dbIPlus = new Database())
            {
                using (var dbApp = new DatabaseApp(dbIPlus))
                {
                    Material reworkMaterial = dbApp.Material.FirstOrDefault(c => c.MaterialNo == ReworkMaterialNo);
                    if (reworkMaterial == null)
                    {
                        //TODO: message
                        return;
                    }

                    ProdOrderPartslistPos intermediateChildPos;
                    ProdOrderPartslistPos intermediatePosition;
                    MaterialWFConnection matWFConnection;
                    ProdOrderBatch batch;
                    ProdOrderBatchPlan batchPlan;
                    ProdOrderPartslistPos endBatchPos;
                    bool posFound = PWDosing.GetRelatedProdOrderPosForWFNode(this, dbIPlus, dbApp, pwMethodProduction,
                        out intermediateChildPos, out intermediatePosition, out endBatchPos, out matWFConnection, out batch, out batchPlan);

                    if (!posFound)
                    {
                        //error
                        return;
                    }

                    if (batch == null)
                    {
                        // Error50276: No batch assigned to last intermediate material of this workflow
                        Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(30)", 1010, "Error50276");

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, false);
                        return;
                    }
                    else if (matWFConnection == null)
                    {
                        // Error50277: No relation defined between Workflownode and intermediate material in Materialworkflow
                        Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(40)", 761, "Error50277");

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, false);
                        return;
                    }
                    else if (intermediatePosition == null)
                    {
                        // Error50278: Intermediate product line not found which is assigned to this workflownode.
                        Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(50)", 778, "Error50278");

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, false);
                        return;
                    }

                    ProdOrderPartslist poPartslist = batch?.ProdOrderPartslist;
                    if (poPartslist == null)
                    {
                        //error
                        return;
                    }

                    double reworkQuantity = poPartslist.TargetQuantity * ((double)ReworkQuantityPercentage / 100);
                    var factor = batchPlan.BatchSize / poPartslist.TargetQuantity;
                    double batchQuantity = reworkQuantity * factor;

                    var components = poPartslist.ProdOrderPartslistPos_ProdOrderPartslist.Where(c => c.MaterialID.HasValue
                                                                                                  && c.MaterialPosType == GlobalApp.MaterialPosTypes.OutwardRoot);

                    ProdOrderPartslistPos reworkComponent = components.FirstOrDefault(c => c.MaterialID == reworkMaterial.MaterialID);
                    if (reworkComponent == null)
                    {
                        reworkComponent = AddProdOrderPartslistPos(dbApp, poPartslist, reworkMaterial, components, reworkQuantity);
                    }

                    if (reworkComponent == null)
                    {
                        //todo: error
                        return;
                    }

                    Material intermediateMaterial = matWFConnection.Material;
                    if (intermediateMaterial == null)
                    {
                        //todo: error
                        return;
                    }

                    AdjustBatchPosInProdOrderPartslist(dbApp, poPartslist, intermediateMaterial, reworkComponent, batch, batchQuantity, reworkQuantity);

                }
            }
        }

        private ProdOrderPartslistPos AddProdOrderPartslistPos(DatabaseApp dbApp, ProdOrderPartslist poPartslist, Material reworkMaterial,
                                                               IEnumerable<ProdOrderPartslistPos> components, double targetQuantity)
        {
            if (poPartslist == null || reworkMaterial == null || components == null)
                return null;

            ProdOrderPartslistPos pos = ProdOrderPartslistPos.NewACObject(dbApp, poPartslist);
            pos.Material = reworkMaterial;
            pos.MaterialPosTypeIndex = (short)GlobalApp.MaterialPosTypes.OutwardRoot;
            pos.Sequence = 1;
            if (components.Any())
            {
                pos.Sequence = components.Max(x => x.Sequence) + 1;
            }
            pos.MDUnit = reworkMaterial.BaseMDUnit;
            pos.TargetQuantityUOM = targetQuantity;

            dbApp.ProdOrderPartslistPos.AddObject(pos);

            return pos;
        }

        private void AdjustBatchPosInProdOrderPartslist(DatabaseApp dbApp, ProdOrderPartslist poPartslist, Material intermediateMaterial, ProdOrderPartslistPos sourcePos, ProdOrderBatch batch,
                                                        double batchQuantity, double totalQuantity)
        {
            ProdOrderPartslistPos targetPos = poPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                                                             .FirstOrDefault(c => c.MaterialID == intermediateMaterial.MaterialID
                                                                               && c.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern);

            if (targetPos == null)
            {
                targetPos = ProdOrderPartslistPos.NewACObject(dbApp, null);
                targetPos.Sequence = 1;
                targetPos.MaterialID = intermediateMaterial.MaterialID;
                targetPos.MaterialPosTypeIndex = (short)GlobalApp.MaterialPosTypes.InwardIntern;
                dbApp.ProdOrderPartslistPos.AddObject(targetPos);
            }

            ProdOrderPartslistPosRelation topRelation = sourcePos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                                                           .FirstOrDefault(c => c.TargetProdOrderPartslistPos.MaterialID == targetPos.MaterialID
                                                                             && c.TargetProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern);

            if (topRelation == null)
            {
                var existingRelations = targetPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos
                                                 .Where(c => c.TargetProdOrderPartslistPos.MaterialID == targetPos.MaterialID
                                                          && c.TargetProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern);
                int nextSeq = 1;
                if (existingRelations != null && existingRelations.Any())
                {
                    if (ComponentsSeqFrom > 0)
                        existingRelations = existingRelations.Where(c => c.Sequence >= ComponentsSeqFrom);

                    if (ComponentsSeqTo > 0)
                        existingRelations = existingRelations.Where(c => c.Sequence <= ComponentsSeqTo);

                    nextSeq = existingRelations.Max(c => c.Sequence);
                    if (nextSeq < ComponentsSeqTo)
                        nextSeq++;
                }

                topRelation = ProdOrderPartslistPosRelation.NewACObject(dbApp, null);
                topRelation.SourceProdOrderPartslistPos = sourcePos;
                topRelation.TargetProdOrderPartslistPos = targetPos;
                topRelation.Sequence = nextSeq;
                topRelation.TargetQuantityUOM = totalQuantity;

                dbApp.ProdOrderPartslistPosRelation.AddObject(topRelation);
            }

            if (topRelation.MDProdOrderPartslistPosState.ProdOrderPartslistPosState != MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.InProcess)
            {
                topRelation.MDProdOrderPartslistPosState = dbApp.MDProdOrderPartslistPosState.FirstOrDefault(c => c.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.InProcess);
            }

            ProdOrderPartslistPosRelation batchRelation = batch.ProdOrderPartslistPosRelation_ProdOrderBatch
                                                               .FirstOrDefault(c => c.ParentProdOrderPartslistPosRelationID == topRelation.ProdOrderPartslistPosRelationID);

            ProdOrderPartslistPos batchPos = batch.ProdOrderPartslistPos_ProdOrderBatch.FirstOrDefault(c => c.MaterialID == intermediateMaterial.MaterialID
                                                                                     && c.MaterialPosType == GlobalApp.MaterialPosTypes.InwardPartIntern);

            if (batchPos == null)
            {
                batchPos = ProdOrderPartslistPos.NewACObject(dbApp, targetPos);
                batchPos.Sequence = topRelation.Sequence;
                batchPos.TargetQuantityUOM = totalQuantity;
                batchPos.ProdOrderBatch = batch;
                batchPos.MDUnit = targetPos.MDUnit;
                targetPos.ProdOrderPartslistPos_ParentProdOrderPartslistPos.Add(batchPos);
            }

            //targetPos.CalledUpQuantityUOM += quantity;

            if (batchRelation == null)
            {
                batchRelation = ProdOrderPartslistPosRelation.NewACObject(dbApp, topRelation);
                batchRelation.Sequence = topRelation.Sequence;
                batchRelation.TargetProdOrderPartslistPos = batchPos;
                batchRelation.SourceProdOrderPartslistPos = topRelation.SourceProdOrderPartslistPos;
                batchRelation.ProdOrderBatch = batch;
            }

            batchRelation.TargetQuantityUOM = batchQuantity;

            var msg = dbApp.ACSaveChanges();
        }

        [ACMethodInfo("", "", 9999)]
        public ReworkInfoList GetReworkStatus()
        {
            IEnumerable<PWManualWeighing> manualWeighingsForRework = ParentACComponent?.FindChildComponents<PWManualWeighing>(c => c is PWManualWeighing).Where(c => !string.IsNullOrEmpty(c.ReworkMaterialNo) && c.ReworkQuantityPercentage > 0);
            if (manualWeighingsForRework == null)
            {
                return null;
            }

            ReworkInfoList reworkInfoList = new ReworkInfoList();

            foreach (PWManualWeighing manualWeighing in manualWeighingsForRework)
            {
                ReworkInfo reworkInfo = manualWeighing.GetReworkStatusFromNode();
                if (reworkInfo != null)
                {
                    reworkInfoList.Add(reworkInfo);
                }
            }

            return reworkInfoList;
        }

        private ReworkInfo GetReworkStatusFromNode()
        {
            if (string.IsNullOrWhiteSpace(ReworkMaterialNo) || ReworkQuantityPercentage <= 0.00001)
                return null;

            ReworkInfo result = new ReworkInfo();
            result.MaterialNo = ReworkMaterialNo;
            result.WFNodeACUrl = this.GetACUrl();
            result.WFNodeComment = this.Comment;

            Msg msg = IsReworkActivated(result);
            if (msg != null)
            {
                //TODO: alarm
                return null;
            }

            return result;
        }

        private Msg IsReworkActivated(ReworkInfo reworkInfo)
        {
            PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
            // If dosing is not for production, then do nothing
            if (pwMethodProduction == null)
            {
                reworkInfo = null;
                return null;
            }

            if (ProdOrderManager == null)
            {
                reworkInfo = null;
                return null;
            }

            using (var dbIPlus = new Database())
            {
                using (var dbApp = new DatabaseApp(dbIPlus))
                {
                    Material reworkMaterial = dbApp.Material.FirstOrDefault(c => c.MaterialNo == ReworkMaterialNo);
                    if (reworkMaterial == null)
                    {
                        //TODO: message
                        return new Msg();
                    }

                    reworkInfo.MaterialName = reworkMaterial.MaterialName1;

                    ProdOrderPartslistPos intermediateChildPos;
                    ProdOrderPartslistPos intermediatePosition;
                    MaterialWFConnection matWFConnection;
                    ProdOrderBatch batch;
                    ProdOrderBatchPlan batchPlan;
                    ProdOrderPartslistPos endBatchPos;
                    bool posFound = PWDosing.GetRelatedProdOrderPosForWFNode(this, dbIPlus, dbApp, pwMethodProduction,
                        out intermediateChildPos, out intermediatePosition, out endBatchPos, out matWFConnection, out batch, out batchPlan);

                    if (!posFound)
                    {
                        //error
                        return new Msg();
                    }

                    if (batch == null)
                    {
                        // Error50276: No batch assigned to last intermediate material of this workflow
                        Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(30)", 1010, "Error50276");

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, false);
                        return msg;
                    }
                    else if (matWFConnection == null)
                    {
                        // Error50277: No relation defined between Workflownode and intermediate material in Materialworkflow
                        Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(40)", 761, "Error50277");

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, false);
                        return msg;
                    }
                    else if (intermediatePosition == null)
                    {
                        // Error50278: Intermediate product line not found which is assigned to this workflownode.
                        Msg msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartManualWeighingProd(50)", 778, "Error50278");

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, false);
                        return msg;
                    }

                    ProdOrderPartslist poPartslist = batch?.ProdOrderPartslist;
                    if (poPartslist == null)
                    {
                        //error
                        return new Msg();
                    }

                    double reworkQuantity = poPartslist.TargetQuantity * ((double)ReworkQuantityPercentage / 100);
                    var factor = batchPlan.BatchSize / poPartslist.TargetQuantity;
                    double batchQuantity = reworkQuantity * factor;

                    reworkInfo.ReworkQuantityPerOrder = reworkQuantity;
                    reworkInfo.ReworkQuantityPerBatch = batchQuantity;

                    var components = poPartslist.ProdOrderPartslistPos_ProdOrderPartslist.Where(c => c.MaterialID.HasValue
                                                                                                  && c.MaterialPosType == GlobalApp.MaterialPosTypes.OutwardRoot);

                    ProdOrderPartslistPos reworkComponent = components.FirstOrDefault(c => c.MaterialID == reworkMaterial.MaterialID);
                    if (reworkComponent == null)
                    {
                        reworkInfo.IsReworkActivated = false;
                        return null;
                    }

                    Material intermediateMaterial = matWFConnection.Material;
                    if (intermediateMaterial == null)
                    {
                        //todo: error
                        return new Msg();
                    }

                    ProdOrderPartslistPos targetPos = poPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                                                            .FirstOrDefault(c => c.MaterialID == intermediateMaterial.MaterialID
                                                                              && c.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern);

                    if (targetPos == null)
                    {
                        reworkInfo.IsReworkActivated = false;
                        return null;
                    }

                    ProdOrderPartslistPosRelation topRelation = reworkComponent.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                                                                   .FirstOrDefault(c => c.TargetProdOrderPartslistPos.MaterialID == targetPos.MaterialID
                                                                                     && c.TargetProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern);

                    if (topRelation == null)
                    {
                        reworkInfo.IsReworkActivated = false;
                        return null;
                    }

                    if (topRelation.MDProdOrderPartslistPosState.ProdOrderPartslistPosState == MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                        || topRelation.MDProdOrderPartslistPosState.ProdOrderPartslistPosState == MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled
                        || topRelation.MDProdOrderPartslistPosState.ProdOrderPartslistPosState == MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Blocked)
                    {
                        reworkInfo.IsReworkActivated = false;
                        return null;
                    }
                }
            }

            reworkInfo.IsReworkActivated = true;
            return null;
        }

    }

    [ACSerializeableInfo]
    [DataContract]
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Rework info'}de{'Rework info'}", Global.ACKinds.TACSimpleClass)]
    public class ReworkInfo : EntityBase, IACObject
    {
        [DataMember(Name = "A")]
        public string WFNodeACUrl
        {
            get;
            set;
        }

        [ACPropertyInfo(100, "", "")]
        [DataMember(Name = "AA")]
        public string WFNodeComment
        {
            get;
            set;
        }

        [ACPropertyInfo(100, "", ConstApp.MaterialNo)]
        [DataMember(Name = "B")]
        public string MaterialNo
        {
            get;
            set;
        }

        [ACPropertyInfo(100, "", ConstApp.MaterialName1)]
        [DataMember(Name = "C")]
        public string MaterialName
        {
            get;
            set;
        }

        [ACPropertyInfo(100, "", "")]
        [DataMember(Name = "D")]
        public double ReworkQuantityPerBatch
        {
            get;
            set;
        }

        [ACPropertyInfo(100, "", "")]
        [DataMember(Name = "E")]
        public double ReworkQuantityPerOrder
        {
            get;
            set;
        }

        [ACPropertyInfo(100, "", "")]
        [DataMember(Name = "F")]
        public bool IsReworkActivated
        {
            get;
            set;
        }

        public IACObject ParentACObject => null;

        public IACType ACType => this.ReflectACType();

        public IEnumerable<IACObject> ACContentList => this.ReflectGetACContentList();

        public string ACIdentifier => this.ReflectGetACIdentifier();

        public string ACCaption => ACIdentifier;

        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            return this.ReflectACUrlBinding(acUrl, ref acTypeInfo, ref source, ref path, ref rightControlMode);
        }

        public object ACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectACUrlCommand(acUrl, acParameter);
        }

        public string GetACUrl(IACObject rootACObject = null)
        {
            return this.ReflectGetACUrl(rootACObject);
        }

        public bool IsEnabledACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectIsEnabledACUrlCommand(acUrl, acParameter);
        }
    }

    [ACSerializeableInfo]
    [CollectionDataContract]
    public class ReworkInfoList : List<ReworkInfo>
    {

    }
}
