using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoBackward_ProdOrderPartslistPos : DoItem<ProdOrderPartslistPos>
    {

        #region ctor's

        public DoBackward_ProdOrderPartslistPos(DatabaseApp databaseApp, TandTv2Result result, ProdOrderPartslistPos item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion

        #region Overriden methods
        public override TandTv2StepItem Factory_StepItem(TandTv2Result result, TandTv2Step step)
        {
            TandTv2StepItem stepItem = base.Factory_StepItem(result, step);
            stepItem.PrimaryKeyID = Item.ProdOrderPartslistPosID;
            stepItem.TandT_ItemTypeEnum = TandTv2ItemTypeEnum.ProdOrderPartslistPos;
            stepItem.ProdOrderPartslistPos = Item;
            if (Item.ProdOrderBatchID != null)
                result.BatchIDs.Add(Item.ProdOrderBatchID ?? Guid.Empty);
            stepItem.InsertDate = Item.InsertDate;
            return stepItem;
        }

        public override string Factory_StepItem_ACIdentifier()
        {
            return string.Format(@"({0}) #{1} / #{2} | {3}",
                   Item.ProdOrderPartslist.ProdOrder.ProgramNo, Item.ProdOrderPartslist.Sequence, Item.Sequence, Item.MaterialPosType.ToString());
        }

        public override string Factory_StepItem_ACCaptionTranslation()
        {
            return string.Format(@"[{0}] {1}", Item.Material.MaterialNo, Item.Material.MaterialName1);
        }

        public override List<IDoItem> ProcessRelatedSameStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            List<IDoItem> relatedSameStep = new List<IDoItem>();
            relatedSameStep.Add(new DoBackward_ProdOrderPartslist(databaseApp, result, Item.ProdOrderPartslist, jobFilter));

            IEnumerable<ACClass> foundedRelatedMachines = FindRelatedMachine(databaseApp);
            if (foundedRelatedMachines != null && foundedRelatedMachines.Any())
                relatedSameStep.AddRange(
                    foundedRelatedMachines
                    .Select(c =>
                        new DoBackward_ACClass(databaseApp, result, c, jobFilter)
                    ).ToList());
            return relatedSameStep;
        }

        public override List<IDoItem> SearchRelatedNextStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            List<IDoItem> related = new List<IDoItem>();
            switch (Item.MaterialPosType)
            {
                case GlobalApp.MaterialPosTypes.InwardIntern:
                    related.AddRange(ProcessRelated_InwardIntern(databaseApp, result, Item, jobFilter));
                    break;
                case GlobalApp.MaterialPosTypes.InwardPartIntern:
                    related.AddRange(ProcessRelated_InwardPartIntern(databaseApp, result, Item, jobFilter));
                    break;
                case GlobalApp.MaterialPosTypes.OutwardRoot:
                    related.AddRange(ProcessRelated_OutwardRoot(databaseApp, result, Item, jobFilter));
                    break;
            }
            return related;
        }

        #endregion

        #region Helper methods
        public IEnumerable<ACClass> FindRelatedMachine(DatabaseApp databaseApp)
        {
            List<Guid> acClassIDs = new List<Guid>();
            List<Guid> acProgramLogID = databaseApp
                .OrderLog.Where(c => c.ProdOrderPartslistPosID == Item.ProdOrderPartslistPosID)
                .Select(c => c.VBiACProgramLogID).ToList();
            if (acProgramLogID.Any())
            {
                using (iplusContext.Database database = new iplusContext.Database())
                {
                    acClassIDs =
                    database
                       .ACProgramLog
                       .Where(c => acProgramLogID.Contains(c.ACProgramLogID))
                       .SelectMany(c => c.ACProgramLog_ParentACProgramLog)
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

        private IEnumerable<IDoItem> ProcessRelated_OutwardRoot(DatabaseApp databaseApp, TandTv2Result result, ProdOrderPartslistPos item, TandTv2Job jobFilter)
        {
            List<IDoItem> related = new List<IDoItem>();
            if (/* jobFilter.UseSourceProdOrderPartslist && */ item.SourceProdOrderPartslistID != null)
            {
                ProdOrderPartslistPos finalMixure = TandTv2Query.s_cQry_GetFinalMixure(databaseApp, item.SourceProdOrderPartslistID ?? Guid.Empty);
                if (finalMixure != null)
                    related.Add(new DoBackward_ProdOrderPartslistPos(databaseApp, result, finalMixure, jobFilter));
            }
            return related;
        }

        private IEnumerable<IDoItem> ProcessRelated_InwardIntern(DatabaseApp databaseApp, TandTv2Result result, ProdOrderPartslistPos item, TandTv2Job jobFilter)
        {
            List<IDoItem> related = new List<IDoItem>();
            if (Item.ProdOrderPartslistPos_ParentProdOrderPartslistPos.Any())
            {
                var children = item.ProdOrderPartslistPos_ParentProdOrderPartslistPos.ToList();
                bool childAlreadyAdded = result.IDs.Intersect(children.Select(c => c.ProdOrderPartslistPosID)).Any();
                foreach (ProdOrderPartslistPos childItem in children)
                {
                    if (result.BatchIDs.Contains(childItem.ProdOrderBatchID ?? Guid.Empty))
                    {
                        bool useLotCheck = UseLotCheck(childItem);
                        if (useLotCheck)
                        {
                            if (childItem.FacilityLotID != null && result.StepLots.Select(lt => lt.LotNo).Contains(childItem.FacilityLot.LotNo))
                            {
                                related.Add(new DoBackward_ProdOrderPartslistPos(databaseApp, result, childItem, jobFilter));
                            }
                        }
                        else if (!useLotCheck && !childAlreadyAdded)
                            related.Add(new DoBackward_ProdOrderPartslistPos(databaseApp, result, childItem, jobFilter));
                    }
                }
            }
            return related;
        }

        private IEnumerable<IDoItem> ProcessRelated_InwardPartIntern(DatabaseApp databaseApp, TandTv2Result result, ProdOrderPartslistPos item, TandTv2Job jobFilter)
        {
            List<IDoItem> related = new List<IDoItem>();

            bool useLotCheck = UseLotCheck(item);
            Console.WriteLine(@"T&Tv2 UseLotCheck POS:{0} -> Check:{1}", item.ToString(), useLotCheck);

            List<ProdOrderPartslistPosRelation> relations = new List<ProdOrderPartslistPosRelation>();

            if (!useLotCheck)
            {
                relations = item.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.ToList();
            }
            else
            {
                relations =
                item.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos
                    .SelectMany(c => c.FacilityBookingCharge_ProdOrderPartslistPosRelation)
                    .Where(c =>
                        c.OutwardFacilityChargeID != null &&
                        c.OutwardFacilityChargeID != null &&
                        c.OutwardFacilityCharge.FacilityLotID != null &&
                        result.StepLots.Select(lt => lt.LotNo).Contains(c.OutwardFacilityCharge.FacilityLot.LotNo)
                     )
                    .Where(c => TandTv2Filters.filterFBCFromToForMaterial(c, jobFilter.MaterialIDs, jobFilter.FilterDateFrom, jobFilter.FilterDateTo))
                    .Select(c => c.ProdOrderPartslistPosRelation).ToList();
            }

            if (relations.Any())
                related
                    .AddRange(
                        relations
                        .Select(c => new DoBackward_ProdOrderPartslistPosRelation(databaseApp, result, c, jobFilter)).ToList());

            List<FacilityBookingCharge> facilityBookingCharges = GetFacilityBookingCharges(result, jobFilter, useLotCheck);
            if (facilityBookingCharges.Any())
                related.AddRange(facilityBookingCharges.Select(c=> new DoBackward_FacilityBookingCharge(databaseApp, result, c, jobFilter) as IDoItem));

            return related;
        }

        public virtual List<FacilityBookingCharge> GetFacilityBookingCharges(TandTv2Result result, TandTv2Job jobFilter, bool useLotCheck)
        {
            IEnumerable<FacilityBookingCharge> fbcQuery = null;
            if (Item.ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos.Any())
            {
                fbcQuery =
                    Item
                    .ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos
                    .SelectMany(c => c.FacilityBooking_ProdOrderPartslistPosFacilityLot)
                    .SelectMany(c => c.FacilityBookingCharge_FacilityBooking)
                    .Where(c =>
                        ((c.InwardFacilityChargeID != null &&
                        c.InwardFacilityCharge.FacilityLotID != null)
                        ||
                        (c.OutwardFacilityChargeID != null &&
                        c.OutwardFacilityCharge.FacilityLotID != null))
                        &&
                        (!useLotCheck ||
                            (
                                (c.InwardFacilityChargeID != null && result.StepLots.Select(x=>x.LotNo).Contains(c.InwardFacilityCharge.FacilityLot.LotNo))
                                ||
                                (c.OutwardFacilityChargeID != null && result.StepLots.Select(x => x.LotNo).Contains(c.OutwardFacilityCharge.FacilityLot.LotNo))
                            )
                        )
                    );
                
            }
            else
            {
                fbcQuery =
                    Item
                    .FacilityBookingCharge_ProdOrderPartslistPos
                    .Where(c =>
                            c.InwardFacilityChargeID != null &&
                            c.InwardFacilityCharge.FacilityLotID != null &&
                            (!useLotCheck || result.StepLots.Select(x => x.LotNo).Contains(c.InwardFacilityCharge.FacilityLot.LotNo))
                    );
            }
            fbcQuery = fbcQuery.Where(c => TandTv2Filters.filterFBCFromToForMaterial(c, jobFilter.MaterialIDs, jobFilter.FilterDateFrom, jobFilter.FilterDateTo));
            return fbcQuery.OrderBy(c => c.FacilityBookingChargeNo).ToList();
        }


        /// <summary>
        /// Define if Lot list
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool UseLotCheck(ProdOrderPartslistPos pos)
        {
            bool useLotCheck = false;
            if (pos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern)
                useLotCheck = pos.ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos.Count() >= 1;
            if (pos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern)
                useLotCheck =
                    pos
                    .ProdOrderPartslistPos_ParentProdOrderPartslistPos
                    .Select(c =>
                    new
                    {
                        ID = c.ProdOrderPartslistPosID,
                        Count = c.ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos.Any() ? c.ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos.Count() : 0
                    }
                    ).Any(c => c.Count >= 1);
            return useLotCheck;
        }

        #endregion

    }
}
