using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    public class DoForward_ProdOrderPartslistPos : DoBackward_ProdOrderPartslistPos
    {
        #region ctor's

        public DoForward_ProdOrderPartslistPos(DatabaseApp databaseApp, TandTv2Result result, ProdOrderPartslistPos item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion

        #region method overrides
        public override List<IDoItem> ProcessRelatedSameStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            List<IDoItem> relatedSameStep = new List<IDoItem>();
            relatedSameStep.Add(new DoForward_ProdOrderPartslist(databaseApp, result, Item.ProdOrderPartslist, jobFilter));

            IEnumerable<ACClass> foundedRelatedMachines = FindRelatedMachine(databaseApp);
            if (foundedRelatedMachines != null && foundedRelatedMachines.Any())
                relatedSameStep.AddRange(
                    foundedRelatedMachines
                    .Select(c => new DoForward_ACClass(databaseApp, result, c, jobFilter))
                    .ToList());
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

        #region private methods
        private IEnumerable<IDoItem> ProcessRelated_OutwardRoot(DatabaseApp databaseApp, TandTv2Result result, ProdOrderPartslistPos item, TandTv2Job jobFilter)
        {
            List<IDoItem> related = new List<IDoItem>();
            IEnumerable<FacilityBookingCharge> fbcQuery = Item
                .ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                .SelectMany(c => c.FacilityBookingCharge_ProdOrderPartslistPosRelation);

            fbcQuery = fbcQuery
                .Where(c =>
                    c.OutwardFacilityChargeID != null &&
                    c.OutwardFacilityCharge.FacilityLotID != null &&
                    result.StepLots.Select(lt => lt.LotNo).Contains(c.OutwardFacilityCharge.FacilityLot.LotNo));

            fbcQuery = fbcQuery.Where(c => TandTv2Filters.filterFBCFromToForMaterial(c, jobFilter.MaterialIDs, jobFilter.FilterDateFrom, jobFilter.FilterDateTo));

            foreach (FacilityBookingCharge fbc in fbcQuery)
                related.Add(new DoForward_ProdOrderPartslistPosRelation(databaseApp, result, fbc.ProdOrderPartslistPosRelation, jobFilter));

            return related;
        }

        private IEnumerable<IDoItem> ProcessRelated_InwardIntern(DatabaseApp databaseApp, TandTv2Result result, ProdOrderPartslistPos item, TandTv2Job jobFilter)
        {
            List<IDoItem> related = new List<IDoItem>();
            if (item.IsFinalMixure)
            {
                List<ProdOrderPartslistPos> mixureSpenders =
                    databaseApp
                    .ProdOrderPartslistPos
                    .Where(c => c.SourceProdOrderPartslistID == item.ProdOrderPartslistID && c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot)
                    .ToList();
                if (mixureSpenders != null && mixureSpenders.Any())
                    foreach (var spender in mixureSpenders)
                        related.Add(new DoForward_ProdOrderPartslistPos(databaseApp, result, spender, jobFilter));
            }
            else
            {
                List<ProdOrderPartslistPosRelation> relations =
                     Item.
                     ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                     .ToList()
                     .Where
                     (c =>
                        !UseLotCheck(c.SourceProdOrderPartslistPos)
                        ||
                        c.FacilityBookingCharge_ProdOrderPartslistPosRelation
                        .Where(x=>x.OutwardFacilityChargeID != null &&
                        x.OutwardFacilityCharge.FacilityLotID != null &&
                        result.StepLots.Select(lt=>lt.LotNo).Contains(x.OutwardFacilityCharge.FacilityLot.LotNo)).Any()
                     )
                     .ToList();
            }
            return related;
        }

        private IEnumerable<IDoItem> ProcessRelated_InwardPartIntern(DatabaseApp databaseApp, TandTv2Result result, ProdOrderPartslistPos item, TandTv2Job jobFilter)
        {
            List<IDoItem> related = new List<IDoItem>();

            List<FacilityBookingCharge> facilityBookingCharges = GetFacilityBookingCharges(result, jobFilter, UseLotCheck(Item));
            if(facilityBookingCharges.Any())
                related.AddRange(facilityBookingCharges.Select(c => new DoForward_FacilityBookingCharge(databaseApp, result, c, jobFilter) as IDoItem));

            related.Add(new DoForward_ProdOrderPartslistPos(databaseApp, result, Item.ProdOrderPartslistPos1_ParentProdOrderPartslistPos, jobFilter));
            return related;
        }

        public override List<FacilityBookingCharge> GetFacilityBookingCharges(TandTv2Result result, TandTv2Job jobFilter, bool useLotCheck)
        {
            IEnumerable<FacilityBookingCharge> fbcQuery = Item.FacilityBookingCharge_ProdOrderPartslistPos;
            fbcQuery = fbcQuery.Where(c =>
                   c.InwardFacilityChargeID != null &&
                   c.InwardFacilityCharge.FacilityLotID != null &&
                   (!useLotCheck || result.StepLots.Select(lt => lt.LotNo).Contains(c.InwardFacilityCharge.FacilityLot.LotNo)));

            //IEnumerable< FacilityBookingCharge> fbcQuery = TandTv2Query.s_cQry_GetPosCharge(databaseApp, pos.ProdOrderPartslistID, useLotCheck, result.IDs, result.StepLots.Select(lt => lt.LotNo).ToList());
            fbcQuery = fbcQuery.Where(c => TandTv2Filters.filterFBCFromToForMaterial(c, jobFilter.MaterialIDs, jobFilter.FilterDateFrom, jobFilter.FilterDateTo));
            return fbcQuery.ToList();
        }

        #endregion

    }
}
