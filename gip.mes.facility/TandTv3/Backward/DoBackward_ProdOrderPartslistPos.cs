using gip.mes.datamodel;
using Facility = gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;

namespace gip.mes.facility.TandTv3
{
    public class DoBackward_ProdOrderPartslistPos : ItemTracking<ProdOrderPartslistPos>
    {
        #region ctor's

        public DoBackward_ProdOrderPartslistPos(DatabaseApp databaseApp, TandTResult result, ProdOrderPartslistPos item) : base(databaseApp, result, item)
        {
            ItemTypeName = "ProdOrderPartslistPos";
            if (!Result.Ids.Keys.Contains(item.ProdOrderPartslistPosID))
                Result.Ids.Add(item.ProdOrderPartslistPosID, ItemTypeName);
            if (Item.ProdOrderBatchID != null)
                if (!result.BatchIDs.Contains(Item.ProdOrderBatchID ?? Guid.Empty))
                    Result.BatchIDs.Add(Item.ProdOrderBatchID ?? Guid.Empty);

            IsUseLotCheck = UseLotCheck(Item);
        }

        #endregion

        #region Properties

        public bool IsUseLotCheck { get; set; }

        public bool IsMaterialWFNoForFilterLotByTime
        {
            get
            {
                return 
                    Result.Filter.MaterialWFNoForFilterLotByTime == (Item.ProdOrderPartslist.Partslist?.MaterialWF?.MaterialWFNo);
            }
        }

        #endregion

        #region IItemTracking

        public override List<IACObjectEntity> GetSameStepItems()
        {
            List<IACObjectEntity> sameStepItems = new List<IACObjectEntity>();
            sameStepItems.Add(Item.ProdOrderPartslist);
            switch (Item.MaterialPosType)
            {
                case GlobalApp.MaterialPosTypes.InwardIntern:
                    sameStepItems.AddRange(OperateSameStepItems_InwardIntern());
                    break;
                case GlobalApp.MaterialPosTypes.InwardPartIntern:
                    sameStepItems.AddRange(OperateSameStepItems_InwardPartIntern());
                    break;
                case GlobalApp.MaterialPosTypes.OutwardRoot:
                    sameStepItems.AddRange(OperateSameStepItems_OutwardRoot());
                    break;
            }

            if (Item.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern && !IsMaterialWFNoForFilterLotByTime)
            {
                List<FacilityBookingCharge> facilityBookingCharges = GetFacilityBookingCharges(IsUseLotCheck);
                sameStepItems.AddRange(facilityBookingCharges);
            }

            return sameStepItems;
        }

        public override List<IACObjectEntity> GetNextStepItems()
        {
            List<IACObjectEntity> nextStepItems = new List<IACObjectEntity>();
            switch (Item.MaterialPosType)
            {
                case GlobalApp.MaterialPosTypes.InwardIntern:
                    nextStepItems.AddRange(OperateNextStepItems_InwardIntern());
                    break;
                case GlobalApp.MaterialPosTypes.InwardPartIntern:
                    nextStepItems.AddRange(OperateNextStepItems_InwardPartIntern());
                    break;
                case GlobalApp.MaterialPosTypes.OutwardRoot:
                    nextStepItems.AddRange(OperateNextStepItems_OutwardRoot());
                    break;
            }
            return nextStepItems;
        }

        public override void AssignItemToMixPoint(List<IACObjectEntity> sameStepItems)
        {
            if (Item.MaterialPosType == GlobalApp.MaterialPosTypes.InwardPartIntern)
            {
                TandTv3Point mixPoint = Result.GetMixPoint(Step, Item);
                if (Item.LabOrder_ProdOrderPartslistPos.Any())
                {
                    DatabaseApp db_test = Item.GetObjectContext<DatabaseApp>();
                    mixPoint.ExistLabOrder = true;
                    mixPoint.ItemsWithLabOrder.Add(MixPointLabOrder.Factory(db_test, Item));
                }
            }
        }

        #endregion

        #region POS Operations

        #region POS Operations -> SameStep
        public virtual List<IACObjectEntity> OperateSameStepItems_InwardIntern()
        {
            List<IACObjectEntity> sameStepItems = new List<IACObjectEntity>();
            #region T&Tv3 Case Without Bookings
            var childPositions =
                Item
                .ProdOrderPartslistPos_ParentProdOrderPartslistPos
                .Where(c => c.ProdOrderBatchID != null && Result.BatchIDs.Contains(c.ProdOrderBatchID ?? Guid.Empty));
            if (childPositions.Any())
            {
                bool withoutFB = !childPositions.SelectMany(c => c.FacilityBookingCharge_ProdOrderPartslistPos).Any();
                if (withoutFB)
                {
                    List<ProdOrderPartslistPos> childInBatch = childPositions.ToList();
                    if (childInBatch.Any())
                    {
                        ProdOrderPartslistPos firstItem = childInBatch.FirstOrDefault();
                        FacilityLot emptyLot = new FacilityLot() { LotNo = TandTv3Command.EmptyLotName };
                        TandTv3Point mixPoint = Result.AddMixPoint(Step, firstItem, emptyLot);
                    }
                }
                sameStepItems.AddRange(childPositions);
            }
            else
            {
                List<ProdOrderPartslistPos> childInBatch = Item.ProdOrderPartslistPos_ParentProdOrderPartslistPos.Where(c=>c.FacilityLotID!= null && Result.Lots.Contains(c.FacilityLot.LotNo)).ToList();
                sameStepItems.AddRange(childInBatch);
            }
            #endregion
            return sameStepItems;
        }

        public virtual List<IACObjectEntity> OperateSameStepItems_InwardPartIntern()
        {
            List<IACObjectEntity> sameStepItems = new List<IACObjectEntity>();
            List<ProdOrderPartslistPosRelation> relations = Item.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.OrderBy(c => c.Sequence).ToList();
            sameStepItems.AddRange(relations);
            return sameStepItems;
        }

        public virtual List<IACObjectEntity> OperateSameStepItems_OutwardRoot()
        {
            List<IACObjectEntity> sameStepItems = new List<IACObjectEntity>();
            return sameStepItems;
        }
        #endregion

        #region POS Operations -> NextStep
        public virtual List<IACObjectEntity> OperateNextStepItems_InwardIntern()
        {
            return new List<IACObjectEntity>();
        }

        public virtual List<IACObjectEntity> OperateNextStepItems_InwardPartIntern()
        {
            return new List<IACObjectEntity>();
        }

        public virtual List<IACObjectEntity> OperateNextStepItems_OutwardRoot()
        {
            List<IACObjectEntity> nextStepItems = new List<IACObjectEntity>();
            if (/* jobFilter.UseSourceProdOrderPartslist && */ Item.SourceProdOrderPartslistID != null)
            {
                ProdOrderPartslistPos finalMixure = TandTv3Query.s_cQry_GetFinalMixure(DatabaseApp, Item.SourceProdOrderPartslistID ?? Guid.Empty);
                if (finalMixure != null)
                    nextStepItems.Add(finalMixure);
            }
            return nextStepItems;
        }
        #endregion

        #region PosOperations -> Common
        public virtual bool UseLotCheck(ProdOrderPartslistPos pos)
        {
            bool useLotCheck = false;
            if (pos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern)
                useLotCheck = pos.ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos.Any();
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
            Console.WriteLine(@"T&Tv3 UseLotCheck POS:{0} -> Check:{1}", pos.ToString(), useLotCheck);
            return useLotCheck;
        }

        public virtual List<FacilityBookingCharge> GetFacilityBookingCharges(bool useLotCheck)
        {
            List<FacilityBookingCharge> fbcs = null;
            if (useLotCheck)
            {

                Guid[] fcLotIDs =
                    Item
                        .ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos
                        .Where(c => Result.Lots.Contains(c.FacilityLot.LotNo))
                        .Select(c => c.ProdOrderPartslistPosFacilityLotID)
                        .ToArray();


                // Inward bookings
                bool isOrderTrackingActive = Result.IsOrderTrackingActive();

                fbcs = Item
                        .ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos
                        .Where(c => fcLotIDs.Contains(c.ProdOrderPartslistPosFacilityLotID))
                        .SelectMany(c => c.FacilityBooking_ProdOrderPartslistPosFacilityLot)
                        .SelectMany(c => c.FacilityBookingCharge_FacilityBooking)
                        .Where(c => TandTv3Query.s_cQry_FBCInwardQuery(c, Result.Filter, null, null, isOrderTrackingActive))
                        .ToList();

                // Outward bookings
                var outwardBookings = GetOutwardBookingsWhenIsUseLotCheck(fcLotIDs);

                fbcs.AddRange(outwardBookings);
            }
            else
            {
                bool isOrderTrackingActive = Result.IsOrderTrackingActive();

                fbcs = Item
                        .FacilityBookingCharge_ProdOrderPartslistPos
                        .Where(c => TandTv3Query.s_cQry_FBCInwardQuery(c, Result.Filter, null, null, isOrderTrackingActive))
                        .ToList();
            }
            return fbcs;
        }

        public virtual List<FacilityBookingCharge> GetOutwardBookingsWhenIsUseLotCheck(Guid[] fcLotIDs)
        {
            return Item
                    .ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos
                    .Where(c => fcLotIDs.Contains(c.ProdOrderPartslistPosFacilityLotID))
                    .SelectMany(c => c.FacilityBooking_ProdOrderPartslistPosFacilityLot)
                    .SelectMany(c => c.FacilityBookingCharge_FacilityBooking)
                    .Where(c => TandTv3Query.s_cQry_FBCOutwardQuery(c, Result.Filter))
                    .ToList();
        }

        #endregion

        #endregion

        #region Overrides
        public override string ToString()
        {
            return string.Format(@"{0}| IsUseLotCheck: {1}", base.ToString(), IsUseLotCheck);
        }
        #endregion

    }
}
