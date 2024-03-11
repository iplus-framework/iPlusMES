using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class DoForward_FacilityBookingCharge : DoBackward_FacilityBookingCharge
    {

        #region ctor's
        public DoForward_FacilityBookingCharge(DatabaseApp databaseApp, TandTResult result, FacilityBookingCharge item) : base(databaseApp, result, item)
        {
            TrackingDirection = MDTrackingDirectionEnum.Forward;
        }
        #endregion

        #region IItemTracking

        public override List<IACObjectEntity> GetNextStepItems()
        {
            List<IACObjectEntity> nextStepItems = new List<IACObjectEntity>();

            FacilityCharge fc = null;
            if (Item.InwardFacilityChargeID != null)
                fc = Item.InwardFacilityCharge;
            if (fc != null && Item.InwardMaterialID != null)
            {
                //var nextFbcIds =
                //    fc
                //    .FacilityLot
                //    .FacilityBookingCharge_OutwardFacilityLot
                //    .Where(c => c.OutwardMaterialID == Item.InwardMaterialID)
                //    .OrderBy(c => c.FacilityBookingChargeNo)
                //    .Select(c => new { c.FacilityBookingChargeID, c.ProdOrderPartslistPosID, c.ProdOrderPartslistPosRelationID, c.InOrderPosID })
                //    .ToList();

                //Guid[] fbcIds = nextFbcIds.Select(c => c.FacilityBookingChargeID).ToArray();
                //List<FacilityBookingCharge> nextFbc =
                //    DatabaseApp
                //    .FacilityBookingCharge
                //    .Include(c => c.InwardFacilityCharge)
                //    .Include(c => c.InwardFacilityCharge.FacilityLot)
                //    .Include(c => c.ProdOrderPartslistPosRelation)
                //    .Include(c => c.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos)
                //    .Include(c => c.ProdOrderPartslistPos)
                //    .Include(c => c.ProdOrderPartslistPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos)
                //    //.Include(c => c.OutwardFacilityCharge)
                //    //.Include(c => c.OutwardFacilityCharge.FacilityLot)
                //    .Where(c => fbcIds.Contains(c.FacilityBookingChargeID)).ToList();


                List<FacilityBookingCharge> nextFbcs = new List<FacilityBookingCharge>();
                bool isOrderTrackingActive = Result.IsOrderTrackingActive();

                nextFbcs =
                fc
                .FacilityLot
                .FacilityBookingCharge_OutwardFacilityLot
                .Where(c =>
                            // c.OutwardMaterialID == Item.InwardMaterialID
                             // c.OutwardFacilityID == Item.InwardFacilityID
                             (isOrderTrackingActive || c.ProdOrderPartslistPosRelationID == null))
                .OrderBy(c => c.FacilityBookingChargeNo)
                .ToList();

                if (Result.Filter.OrderDepth != null && Item.ProdOrderPartslistPosID != null)
                {
                    foreach (FacilityBookingCharge fbc in nextFbcs)
                    {
                        Result.AddOrderConnection(Item, fbc);
                    }
                }

                nextStepItems.AddRange(nextFbcs);
            }

            // MaterialWFNoForFilterLotByTime used - fetch time matched inward items
            if (
                    !string.IsNullOrEmpty(Result.Filter.MaterialWFNoForFilterLotByTime)
                    && Item.OutwardMaterialID != null
                    && Item.OutwardFacilityChargeID != null
                    && Item.ProdOrderPartslistPosRelationID != null
                    && IsMaterialWFNoForFilterLotByTime
                    )
            {
                List<IACObjectEntity> inwardItems = GetInwardItemsForThisOutwardFilterByTime(Item);
                nextStepItems.AddRange(inwardItems);
            }

            if (Item.FacilityBookingTypeIndex == (short)GlobalApp.FacilityBookingType.PickingRelocation)
            {
                List<FacilityBookingCharge> nextFbcs = new List<FacilityBookingCharge>();
                bool isOrderTrackingActive = Result.IsOrderTrackingActive();

                FacilityBookingCharge relocatedCharge
                    = Item
                      .PickingPos
                      .FacilityBookingCharge_PickingPos
                      .Where(c => c.InwardFacilityCharge != null)
                      .FirstOrDefault();

                if (relocatedCharge != null)
                {
                    nextFbcs =
                        relocatedCharge
                        .InwardFacilityCharge
                        .FacilityBookingCharge_OutwardFacilityCharge
                           .Where(c =>
                                c.OutwardMaterialID == relocatedCharge.InwardMaterialID
                                && c.OutwardFacilityID == relocatedCharge.InwardFacilityID
                                && (isOrderTrackingActive || c.ProdOrderPartslistPosRelationID == null))
                        .OrderBy(c => c.FacilityBookingChargeNo)
                        .ToList();

                    if (Result.Filter.OrderDepth != null && Item.ProdOrderPartslistPosID != null)
                    {
                        foreach (FacilityBookingCharge fbc in nextFbcs)
                        {
                            Result.AddOrderConnection(Item, fbc);
                        }
                    }
                    nextStepItems.AddRange(nextFbcs);
                }
            }

            // TODO: @aagincic: define bookings important for tracking -> (one lot transformed to another etc)
            //if (Item.FacilityInventoryPos != null)
            //{
            //    var inwardCharges = Item.InwardFacilityCharge.FacilityBookingCharge_InwardFacilityCharge.Where(c => c.FacilityBookingChargeNo != Item.FacilityBookingChargeNo);
            //    var outwardCharges = Item.InwardFacilityCharge.FacilityBookingCharge_OutwardFacilityCharge.Where(c => c.FacilityBookingChargeNo != Item.FacilityBookingChargeNo);
            //    var nextFbcs = inwardCharges.Union(outwardCharges);
            //    nextStepItems.AddRange(nextFbcs);
            //}

            return nextStepItems;
        }

        private List<IACObjectEntity> GetInwardItemsForThisOutwardFilterByTime(FacilityBookingCharge fbc)
        {
            List<IACObjectEntity> nextStepItems = new List<IACObjectEntity>();

            FacilityBookingCharge firstFBCWithSameFC =
                fbc
                .ProdOrderPartslistPosRelation
                .FacilityBookingCharge_ProdOrderPartslistPosRelation
                .Where(c => 
                            c.FacilityBookingChargeID != fbc.FacilityBookingChargeID 
                            && c.InsertDate < fbc.InsertDate
                            && c.OutwardFacilityChargeID == fbc.OutwardFacilityChargeID
                      )
                .OrderBy(c => c.InsertDate)
                .FirstOrDefault();

            FacilityBookingCharge lastFBCWithSameFC =
                fbc
                .ProdOrderPartslistPosRelation
                .FacilityBookingCharge_ProdOrderPartslistPosRelation
                .Where(c => 
                            c.FacilityBookingChargeID != fbc.FacilityBookingChargeID 
                            && c.InsertDate > fbc.InsertDate
                            && c.OutwardFacilityChargeID == fbc.OutwardFacilityChargeID
                      )
                .OrderByDescending(c => c.InsertDate)
                .FirstOrDefault();

            (DateTime startTime, DateTime endTime) = GetFBCTimeFrame(fbc, firstFBCWithSameFC, lastFBCWithSameFC);

            FacilityBookingCharge[] outwardBookings =
                fbc
                .ProdOrderPartslistPosRelation
                .TargetProdOrderPartslistPos
                .FacilityBookingCharge_ProdOrderPartslistPos
                .Where(c => c.InsertDate >= startTime && c.InsertDate <= endTime)
                .ToArray();

            nextStepItems.AddRange(outwardBookings);

            return nextStepItems;
        }

        #endregion

    }
}