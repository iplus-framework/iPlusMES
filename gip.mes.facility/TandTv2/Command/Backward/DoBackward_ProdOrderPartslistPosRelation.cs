using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoBackward_ProdOrderPartslistPosRelation : DoItem<ProdOrderPartslistPosRelation>
    {

        #region ctor's

        public DoBackward_ProdOrderPartslistPosRelation(DatabaseApp databaseApp, TandTv2Result result, ProdOrderPartslistPosRelation item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion

        #region Overrided methods

        public override TandTv2StepItem Factory_StepItem(TandTv2Result result, TandTv2Step step)
        {
            TandTv2StepItem stepItem = base.Factory_StepItem(result, step);
            stepItem.PrimaryKeyID = Item.ProdOrderPartslistPosRelationID;
            stepItem.TandT_ItemTypeEnum = TandTv2ItemTypeEnum.ProdOrderPartslistPosRelation;
            stepItem.ProdOrderPartslistPosRelation = Item;
            stepItem.InsertDate = Item.SourceProdOrderPartslistPos.InsertDate;
            return stepItem;
        }

        public override string Factory_StepItem_ACIdentifier()
        {
            return string.Format(@"({0}) #{1} / #{2} => {3}", Item.SourceProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                Item.SourceProdOrderPartslistPos.ProdOrderPartslist.Sequence, Item.SourceProdOrderPartslistPos.Sequence, Item.TargetProdOrderPartslistPos.Sequence);
        }

        public override string Factory_StepItem_ACCaptionTranslation()
        {
            return string.Format(@"[{0}] {1} => [{2}] {3}", Item.SourceProdOrderPartslistPos.Material.MaterialNo, Item.SourceProdOrderPartslistPos.Material.MaterialName1,
                Item.TargetProdOrderPartslistPos.Material.MaterialNo, Item.TargetProdOrderPartslistPos.Material.MaterialName1);
        }

        public override List<IDoItem> ProcessRelatedSameStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            IEnumerable<ACClass> foundedRelatedMachines = FindRelatedMachine(databaseApp);
            if (foundedRelatedMachines == null || !foundedRelatedMachines.Any()) return null;
            return
                foundedRelatedMachines
                .Select(c =>
                            new DoBackward_ACClass(databaseApp, result, c, jobFilter) as IDoItem).ToList();
        }

        public override List<IDoItem> SearchRelatedNextStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            List<IDoItem> related = new List<IDoItem>();

            related.Add(new DoBackward_ProdOrderPartslistPos(databaseApp, result, Item.SourceProdOrderPartslistPos, jobFilter));

            List<IDoItem> relatedFacilityBookingCharges = ProcessRelatedFacilityBookingCharge(databaseApp, result, Item, jobFilter);
            if (relatedFacilityBookingCharges != null && relatedFacilityBookingCharges.Any())
                related.AddRange(relatedFacilityBookingCharges);

            // Note: examine all relations in step they can play togehter as rewerk
            // ProdOrderPartslistPos targetPosItem = Item.TargetProdOrderPartslistPos;
            //bool useLotCheck = DoForward_ProdOrderPartslistPos.UseLotCheck(targetPosItem);
            //if (!useLotCheck)
            //{
            List<ProdOrderPartslistPosRelation> otherRelations =
                Item.TargetProdOrderPartslistPos
                .ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos
                .ToList();
            if (otherRelations.Any())
            {
                related.AddRange(otherRelations.Select(c => new DoBackward_ProdOrderPartslistPosRelation(databaseApp, result, c, jobFilter) { AlternativeRelation = true }).ToList());
            }
            //}

            return related;
        }

        #endregion

        #region helper methods

        public IEnumerable<ACClass> FindRelatedMachine(DatabaseApp databaseApp)
        {
            List<Guid> acClassIDs = new List<Guid>();
            List<Guid> acProgramLogID = databaseApp
                .OrderLog.Where(c => c.ProdOrderPartslistPosRelationID == Item.ProdOrderPartslistPosRelationID)
                .Select(c => c.VBiACProgramLogID).ToList();
            if (acProgramLogID.Any())
            {
                using (iplusContext.Database database = new iplusContext.Database())
                {
                    acClassIDs =
                    database
                       .ACProgramLog
                       .Where(c => acProgramLogID.Contains(c.ACProgramLogID))
                       .Select(c => c.ACProgramLog1_ParentACProgramLog)
                       .Where(c => c.RefACClassID != null)
                       .Select(c => c.RefACClassID ?? Guid.Empty)
                        .ToList();
                }
            }
            if (acClassIDs.Any())
            {
                acClassIDs.RemoveAll(c => c == Guid.Empty);
                if (acClassIDs.Any())
                {
                    return
                        databaseApp
                        .ACClass
                        .Where(c => acClassIDs.Contains(c.ACClassID));
                }
            }
            return null;
        }

        #endregion

        #region private methods


        private List<IDoItem> ProcessRelatedFacilityBookingCharge(DatabaseApp databaseApp, TandTv2Result result, ProdOrderPartslistPosRelation item, TandTv2Job jobFilter)
        {
            bool useLotCheck = UseLotCheck(Item);

            Console.WriteLine(@"T&Tv2 UseLotCheck REL:{0} -> Check:{1}", item.ToString(), useLotCheck);

            IEnumerable<FacilityBookingCharge> fbcQuery = item.FacilityBookingCharge_ProdOrderPartslistPosRelation;

            fbcQuery = fbcQuery.Where(c =>
                    c.OutwardFacilityChargeID != null &&
                    c.OutwardFacilityCharge.FacilityLotID != null &&
                    (!useLotCheck || result.StepLots.Select(lt => lt.LotNo).Contains(c.OutwardFacilityCharge.FacilityLot.LotNo)));

            fbcQuery = fbcQuery.Where(c => TandTv2Filters.filterFBCFromToForMaterial(c, jobFilter.MaterialIDs, jobFilter.FilterDateFrom, jobFilter.FilterDateTo));
            if (fbcQuery.Any())
                return fbcQuery.ToList().Select(c => new DoBackward_FacilityBookingCharge(databaseApp, result, c, jobFilter) as IDoItem).ToList();

            else return null;
        }


        public bool UseLotCheck(ProdOrderPartslistPosRelation relation)
        {
            var queryLots =
               relation
               .FacilityBookingCharge_ProdOrderPartslistPosRelation
               .Where(c => c.OutwardMaterialID == relation.SourceProdOrderPartslistPos.MaterialID && c.OutwardFacilityChargeID != null && c.OutwardFacilityCharge.FacilityLotID != null)
               .Select(c => c.OutwardFacilityCharge.FacilityLot.LotNo)
               .Distinct();

            return
                !AlternativeRelation &&
                (DoForward_ProdOrderPartslistPos.UseLotCheck(relation.TargetProdOrderPartslistPos) ||
                queryLots.Count() > 1);
        }
        #endregion


        public bool AlternativeRelation { get; set; }
        private class TestState
        {
            public bool HaveLots { get; set; }
            public bool FinalMixureFound { get; set; }
            public bool IsFinalMixureWithLots { get; set; }
            public int MaterialPosType { get; set; }

            public override string ToString()
            {
                return string.Format(@"{0} | {1} | {2} | {3}", HaveLots, MaterialPosType, FinalMixureFound, IsFinalMixureWithLots);
            }
        }
    }


}
