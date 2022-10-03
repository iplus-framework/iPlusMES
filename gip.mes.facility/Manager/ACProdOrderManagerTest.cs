using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'ProdOrderManagerTest'}de{'ProdOrderManagerTest'}", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]

    public class ACProdOrderManagerTest : ACProdOrderManager
    {

        #region c´tors

        public ACProdOrderManagerTest(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        #endregion

        #region Overrides

        public MsgWithDetails RecalcIntermediateItem(ProdOrderPartslistPos inwardPos, bool updateMixureRelations, MDUnit startMDUnit)
        {
            MsgWithDetails msgWithDetails = null;
            try
            {
                List<ProdOrderPartslistPos> inputMixures =
                   inwardPos
                   .ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos
                   .Where(c => c.SourceProdOrderPartslistPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern)
                   .Select(c => c.SourceProdOrderPartslistPos)
                   .ToList();
                foreach (ProdOrderPartslistPos inputMixure in inputMixures)
                {
                    RecalcIntermediateItem(inputMixure, updateMixureRelations, startMDUnit);
                }

                MDUnit inwardPosMDUnit = inwardPos.MDUnit != null ? inwardPos.MDUnit : inwardPos.Material.BaseMDUnit;

                if (inwardPos.Material.ExcludeFromSumCalc)
                {
                    inwardPos.TargetQuantityUOM = 0;
                }
                else
                {
                    // fix child relations
                    double newTargetQuantityUOM = 0;
                    if (inwardPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Any())
                    {
                        ProdOrderPartslistPosRelation[] targetRelations = inwardPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.ToArray();
                        foreach(ProdOrderPartslistPosRelation targetRelation in targetRelations)
                        {
                            ProdOrderPartslistPos sourcePos = targetRelation.SourceProdOrderPartslistPos;
                            MDUnit sourcePosMDUnit = sourcePos.MDUnit != null ?  sourcePos.MDUnit : sourcePos.Material.BaseMDUnit;
                            if(sourcePosMDUnit.MDUnitID == inwardPosMDUnit.MDUnitID)
                            {
                                newTargetQuantityUOM += targetRelation.TargetQuantityUOM;
                            }
                            else
                            {
                                if(sourcePos.Material.IsConvertableToUnit(sourcePosMDUnit, inwardPosMDUnit))
                                {
                                    newTargetQuantityUOM += sourcePos.Material.ConvertQuantity(targetRelation.TargetQuantityUOM, sourcePosMDUnit, inwardPosMDUnit);
                                }
                            }
                        }
                    }
                    else
                    {
                        newTargetQuantityUOM = 0;
                    }

                    var queryMixureConsumers =
                        inwardPos
                        .ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                        .Where(c => c.TargetProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern);

                    double diffQuantity = newTargetQuantityUOM - inwardPos.TargetQuantityUOM;
                    inwardPos.TargetQuantityUOM = newTargetQuantityUOM;
                    int sourceRelationCount = queryMixureConsumers.Count();
                    double ratioInwardPosQuantityGrowth = 0;
                    if (!FacilityConst.IsDoubleZeroForPosting(inwardPos.TargetQuantityUOM))
                        ratioInwardPosQuantityGrowth = diffQuantity / inwardPos.TargetQuantityUOM;

                    //mixure distrubutes it's quantity to target mixures
                    if (sourceRelationCount > 0)
                    {
                        foreach (var rel in queryMixureConsumers)
                        {
                            // redistrubute complete quantity (updateMixureRelations or previous quanitity = 0
                            if (updateMixureRelations || ratioInwardPosQuantityGrowth == 0 /*&& rel.SourceProdOrderPartslistPos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern*/)
                            {
                                rel.TargetQuantityUOM = rel.SourceProdOrderPartslistPos.TargetQuantityUOM * (1 / sourceRelationCount);
                            }
                            else
                            {
                                rel.TargetQuantityUOM = rel.TargetQuantityUOM + ratioInwardPosQuantityGrowth * rel.TargetQuantityUOM;
                            }
                        }
                    }
                }
            }
            catch (Exception ec)
            {
                msgWithDetails = new MsgWithDetails() { MessageLevel = eMsgLevel.Error, Message = ec.Message };
            }

            return msgWithDetails;
        }


        public MsgWithDetails IsRecalcIntermediateSumPossible(ProdOrderPartslistPos inwardPos)
        {
            return IsRecalcIntermediateSumPossible(inwardPos, inwardPos.MDUnit);
        }

        public MsgWithDetails IsRecalcIntermediateSumPossible(ProdOrderPartslistPos inwardPos, MDUnit startMDUnit)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();
            Msg msgCheckComponents = IsInputComponentsCompatibile(inwardPos);
            Msg msgCheckMixures = IsInputMixuresCompatibile(inwardPos, startMDUnit);

            if (msgCheckComponents != null)
            {
                msgWithDetails.AddDetailMessage(msgCheckComponents);
            }

            if (msgCheckMixures != null)
            {
                msgWithDetails.AddDetailMessage(msgCheckMixures);
            }
            return msgWithDetails;
        }


        /// <summary>
        /// Pass component unit to own intermediate unit
        /// </summary>
        /// <param name="inwardPos"></param>
        /// <param name="startMDUnit"></param>
        /// <returns></returns>
        private Msg IsInputComponentsCompatibile(ProdOrderPartslistPos inwardPos)
        {
            Msg msg = null;
            ProdOrderPartslistPosRelation[] relations =
                inwardPos
                .ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos
                .Where(c => c.SourceProdOrderPartslistPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot)
                .ToArray();

            // OutwardRoot[Pos] MDUnit != InwardIntern[Pos] MDUnit
            MDUnit posUnit = inwardPos.MDUnit != null ? inwardPos.MDUnit : inwardPos.Material.BaseMDUnit;
            MDUnit[] inputUnits = relations.Select(c => c.SourceProdOrderPartslistPos.MDUnit != null ? c.SourceProdOrderPartslistPos.MDUnit : c.SourceProdOrderPartslistPos.Material.BaseMDUnit).ToArray();
            if (inputUnits.Any(c => c.MDUnitID != posUnit.MDUnitID))
            {
                msg = new Msg(this, eMsgLevel.Warning, nameof(ACProdOrderManager), nameof(IsInputComponentsCompatibile), 145, "Question50091");
            }

            // Recursive search to other input mixures
            if (msg == null)
            {
                ProdOrderPartslistPosRelation[] mixRelations =
                   inwardPos
                   .ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos
                   .Where(c => c.SourceProdOrderPartslistPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern)
                   .ToArray();

                foreach (ProdOrderPartslistPosRelation inputMix in mixRelations)
                {
                    msg = IsInputComponentsCompatibile(inputMix.SourceProdOrderPartslistPos);
                    if (msg != null)
                        break;
                }
            }
            return msg;
        }

        private Msg IsInputMixuresCompatibile(ProdOrderPartslistPos inwardPos, MDUnit startMDUnit)
        {
            Msg msg = null;
            ProdOrderPartslistPosRelation[] mixRelations =
                    inwardPos
                    .ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos
                    .Where(c => c.SourceProdOrderPartslistPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern)
                    .ToArray();

            foreach (ProdOrderPartslistPosRelation inputMix in mixRelations)
            {
                if (inputMix.SourceProdOrderPartslistPos.MDUnitID != startMDUnit.MDUnitID)
                {
                    msg = new Msg(this, eMsgLevel.Warning, nameof(ACProdOrderManager), nameof(IsInputComponentsCompatibile), 145, "Question50092");
                }
                if (msg == null)
                {
                    msg = IsInputMixuresCompatibile(inputMix.SourceProdOrderPartslistPos, startMDUnit);
                }
            }
            return msg;
        }

        #endregion
    }
}
