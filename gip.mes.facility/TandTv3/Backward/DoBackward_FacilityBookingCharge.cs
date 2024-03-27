using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class DoBackward_FacilityBookingCharge : ItemTracking<FacilityBookingCharge>
    {
        #region ctor's

        public DoBackward_FacilityBookingCharge(DatabaseApp databaseApp, TandTResult result, FacilityBookingCharge item) : base(databaseApp, result, item)
        {
            ItemTypeName = "FacilityBookingCharge";
            if (!Result.Ids.Keys.Contains(item.FacilityBookingChargeID))
                Result.Ids.Add(item.FacilityBookingChargeID, ItemTypeName);
        }

        #endregion

        #region Properties
        public bool IsMaterialWFNoForFilterLotByTime
        {
            get
            {
                bool result = false;
                ProdOrderPartslistPos pos = null;

                if (Item.ProdOrderPartslistPosRelationID != null)
                {
                    pos = Item.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos;
                }
                else if(Item.ProdOrderPartslistPosID != null)
                {
                    pos = Item.ProdOrderPartslistPos;
                }

                if (pos != null)
                {
                    result = Result.Filter.MaterialWFNoForFilterLotByTime == pos.ProdOrderPartslist.Partslist?.MaterialWF?.MaterialWFNo;
                }

                return result;
                    
            }
        }
        #endregion

        #region IItemTracking

        public override List<IACObjectEntity> GetSameStepItems()
        {
            List<IACObjectEntity> sameStepItems = new List<IACObjectEntity>();

            sameStepItems.Add(Item.FacilityBooking);

            if (Item.InwardFacilityChargeID != null)
                sameStepItems.Add(Item.InwardFacilityCharge);

            if (Item.OutwardFacilityChargeID != null)
                sameStepItems.Add(Item.OutwardFacilityCharge);

            if (Item.ProdOrderPartslistPosID != null)
                sameStepItems.Add(Item.ProdOrderPartslistPos);

            if (Item.ProdOrderPartslistPosRelationID != null)
                sameStepItems.Add(Item.ProdOrderPartslistPosRelation);

            if (Item.InOrderPosID != null)
                sameStepItems.Add(Item.InOrderPos.TopParentInOrderPos);

            if (Item.OutOrderPosID != null)
                sameStepItems.Add(Item.OutOrderPos.TopParentOutOrderPos);

            return sameStepItems;
        }

        public override List<IACObjectEntity> GetNextStepItems()
        {
            List<IACObjectEntity> nextStepItems = new List<IACObjectEntity>();

            bool isFilteredMaterialForInwardSearch = false;
            if (Item.OutwardMaterial != null)
            {
                isFilteredMaterialForInwardSearch = Result.Filter.MaterialNOsForStopTracking.Contains(Item.OutwardMaterial.MaterialNo);
            }

            if (isFilteredMaterialForInwardSearch)
                return nextStepItems;

            FacilityCharge fc = null;
            if (Item.OutwardFacilityChargeID != null)
                fc = Item.OutwardFacilityCharge;
            if (fc != null && fc.FacilityLot != null && Item.OutwardMaterialID != null)
            {
                List<IACObjectEntity> inwardItems = GetInwardElementsForThisOutward(fc);
                nextStepItems.AddRange(inwardItems);
            }


            // MaterialWFNoForFilterLotByTime used - fetch time matched outward items
            if (
                    !string.IsNullOrEmpty(Result.Filter.MaterialWFNoForFilterLotByTime)
                    && Item.InwardMaterialID != null
                    && Item.InwardFacilityChargeID != null
                    && Item.ProdOrderPartslistPos != null
                    && IsMaterialWFNoForFilterLotByTime
                    )
            {
                List<IACObjectEntity> inwardItems = GetInwardItemsForThisOutwardFilterByTime(Item);
                nextStepItems.AddRange(inwardItems);
            }

            return nextStepItems;
        }

        private List<IACObjectEntity> GetInwardItemsForThisOutwardFilterByTime(FacilityBookingCharge fbc)
        {
            List<IACObjectEntity> nextStepItems = new List<IACObjectEntity>();

            FacilityBookingCharge firstFBCWithSameFC =
                fbc
                .ProdOrderPartslistPos
                .FacilityBookingCharge_ProdOrderPartslistPos
                .Where(c => 
                            c.FacilityBookingChargeID != fbc.FacilityBookingChargeID 
                            && c.InsertDate < fbc.InsertDate 
                            && c.InwardFacilityChargeID == fbc.InwardFacilityChargeID
                       )
                .OrderBy(c => c.InsertDate)
                .FirstOrDefault();

            FacilityBookingCharge lastFBCWithSameFC =
                fbc
                .ProdOrderPartslistPos
                .FacilityBookingCharge_ProdOrderPartslistPos
                .Where(c => 
                            c.FacilityBookingChargeID != fbc.FacilityBookingChargeID 
                            && c.InsertDate > fbc.InsertDate
                            && c.InwardFacilityChargeID == fbc.InwardFacilityChargeID
                        )
                .OrderByDescending(c => c.InsertDate)
                .FirstOrDefault();

            (DateTime startTime, DateTime endTime) = GetFBCTimeFrame(fbc, firstFBCWithSameFC, lastFBCWithSameFC);

            FacilityBookingCharge[] outwardBookings =
                fbc
                .ProdOrderPartslistPos
                .ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos
                .SelectMany(c => c.FacilityBookingCharge_ProdOrderPartslistPosRelation)
                .Where(c => c.InsertDate >= startTime && c.InsertDate <= endTime)
                .ToArray();

            nextStepItems.AddRange(outwardBookings);

            return nextStepItems;
        }

        private List<IACObjectEntity> GetInwardElementsForThisOutward(FacilityCharge fc)
        {
            List<IACObjectEntity> nextStepItems = new List<IACObjectEntity>();
            Guid? materialID = null;
            if (!Result.Filter.IsDisableReworkTracking)
                materialID = Item.OutwardMaterialID;

            bool isOrderTrackingActive = Result.IsOrderTrackingActive();

            Guid? outwardFacilityID = Item.OutwardFacilityID;
            if (Result.TandTv3Command != null && !Result.TandTv3Command.FilterFaciltiyAtSearchInwardCharges)
            {
                outwardFacilityID = null;
            }

            // for outward charges search inward source (delivery note, prod orders)
            var nextFbcs =
                fc
                .FacilityLot
                .FacilityBookingCharge_InwardFacilityLot
                .Where(c => TandTv3Query.s_cQry_FBCInwardQuery(c, Result.Filter, materialID, outwardFacilityID, isOrderTrackingActive))
                .OrderBy(c => c.FacilityBookingChargeNo)
                .ToList();

            var bookingTypes = fc
                .FacilityLot
                .FacilityBookingCharge_InwardFacilityLot
                .GroupBy(c => c.FacilityBookingTypeIndex)
                .Select(c => new { c.Key, FCS = c.Select(x => new { x.InwardFacility?.FacilityNo, x.InwardFacility.FacilityID }) })
                .ToArray();

            // populate order depth
            if (Result.Filter.OrderDepth != null && Item.ProdOrderPartslistPosRelationID != null)
            {
                foreach (FacilityBookingCharge fbc in nextFbcs)
                {
                    Result.AddOrderConnection(fbc, Item);
                }
            }

            if (isOrderTrackingActive && nextFbcs.Any())
                nextStepItems.AddRange(nextFbcs);

            return nextStepItems;
        }

        public override void AssignItemToMixPoint(List<IACObjectEntity> sameStepItems)
        {
            if (Item.ProdOrderPartslistPosID != null)
            {
                ProdOrderPartslistPos pos = Item.ProdOrderPartslistPos;
                if (pos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardPartIntern)
                {
                    FacilityLot inwardFacilityLot = null;
                    if (Item.InwardFacilityChargeID != null && Item.InwardFacilityCharge.FacilityLotID != null)
                    {
                        inwardFacilityLot = Item.InwardFacilityCharge.FacilityLot;
                    }

                    TandTv3Point mixPoint = Result.AddMixPoint(Step, pos, inwardFacilityLot);

                    if (mixPoint.AddInwardBooking(Item))
                        mixPoint.AddInwardLotQuantity(Item);

                }
            }

            if (Item.ProdOrderPartslistPosRelationID != null)
            {
                ProdOrderPartslistPos pos = Item.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos;
                if (pos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardPartIntern)
                {
                    FacilityLot inwardFacilityLot = null;

                    TandTv3Point mixPoint = Result.GetMixPoint(Step, pos);
                    if (mixPoint == null)
                        mixPoint = Result.AddMixPoint(Step, pos, inwardFacilityLot);

                    if (mixPoint.AddOutwardBooking(Item))
                        mixPoint.AddOutwardLotQuantity(Item);

                }
            }

            if (Item.InOrderPosID != null)
            {
                TandTv3Point mixPoint = Result.AddMixPoint(Step, Item.InOrderPos.TopParentInOrderPos);

                if (mixPoint.AddInwardBooking(Item))
                    mixPoint.AddInwardLotQuantity(Item);
            }
        }

        public (DateTime startTime, DateTime endTime) GetFBCTimeFrame(FacilityBookingCharge fbc, FacilityBookingCharge firstFBCWithSameFC, FacilityBookingCharge lastFBCWithSameFC)
        {
            DateTime startTime = DateTime.MinValue;
            DateTime endTime = DateTime.MinValue;

            if (firstFBCWithSameFC != null)
            {
                startTime = firstFBCWithSameFC.InsertDate.AddMinutes(-1);
            }
            else
            {
                startTime = fbc.InsertDate.AddMinutes(-1);
            }

            if (lastFBCWithSameFC != null)
            {
                endTime = lastFBCWithSameFC.InsertDate.AddMinutes(1);
            }
            else
            {
                endTime = fbc.InsertDate.AddMinutes(1);
            }

            return (startTime, endTime);
        }

        #endregion
    }
}
