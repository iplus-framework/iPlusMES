using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.processapplication
{
    public class DischargingItemManager
    {
        public List<DischargingItem> LoadDischargingItemList(Guid intermediateChildPosID, ManualPreparationSourceInfoTypeEnum sourceInfoType)
        {
            List<DischargingItem> items = new List<DischargingItem>();
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                ProdOrderPartslistPos intermedatePartPos = databaseApp.ProdOrderPartslistPos.FirstOrDefault(c => c.ProdOrderPartslistPosID == intermediateChildPosID);
                switch (sourceInfoType)
                {
                    case ManualPreparationSourceInfoTypeEnum.FacilityID:
                        items = LoadDischargingItemListForFacility(intermedatePartPos);
                        break;
                    case ManualPreparationSourceInfoTypeEnum.FacilityChargeID:
                        items = LoadDischargingItemListForFacilityCharge(intermedatePartPos);
                        break;
                }

                var reservations = intermedatePartPos
                   .FacilityBooking_ProdOrderPartslistPos
                   .Where(c => c.InwardQuantity == PWBinSelection.BinSelectionReservationQuantity)
                   .Select(c => new { c.FacilityBookingNo, c.InwardFacilityChargeID })
                   .ToList();

                // Expect only one source relation per batch
                // Can cumulate outward FB per many Relations
                // By creating new outward booking 
                ProdOrderPartslistPosRelation nextMixureRelation =
                    intermedatePartPos
                    .ProdOrderPartslistPos1_ParentProdOrderPartslistPos
                    .ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                    .Where(c => c.TargetProdOrderPartslistPos.ProdOrderBatchID == intermedatePartPos.ProdOrderBatchID)
                    .FirstOrDefault();

                foreach (var item in items)
                {
                    if (sourceInfoType == ManualPreparationSourceInfoTypeEnum.FacilityID && item.InwardFacilityNo != null)
                    {
                        double inwardQuantity = intermedatePartPos.FacilityBooking_ProdOrderPartslistPos.Where(c => c.InwardFacility?.FacilityNo == item.InwardFacilityNo)
                                                                                                        .Sum(x => x.InwardQuantity);
                        item.InwardBookingQuantityUOM = inwardQuantity;
                    }
                    else
                    {
                        if (reservations.Any(c => c.InwardFacilityChargeID == item.InwardFacilityChargeID))
                            item.InwardBookingQuantityUOM += PWBinSelection.BinSelectionReservationQuantity;
                    }

                    #region Outward part setup
                    item.ProdorderPartslistPosRelationID = nextMixureRelation.ProdOrderPartslistPosRelationID;
                    Guid? outwardFacilityID = null;
                    Guid? outwardFacilityChargeID = null;
                    if (sourceInfoType == ManualPreparationSourceInfoTypeEnum.FacilityID)
                        outwardFacilityID = item.ItemID;
                    if (sourceInfoType == ManualPreparationSourceInfoTypeEnum.FacilityChargeID)
                        outwardFacilityChargeID = item.ItemID;
                    FacilityBooking outwardBooking =
                        nextMixureRelation
                        .FacilityBooking_ProdOrderPartslistPosRelation
                        .Where(c =>
                            (outwardFacilityID != null && c.OutwardFacilityID == outwardFacilityID) ||
                            (outwardFacilityChargeID != null && c.OutwardFacilityChargeID == outwardFacilityChargeID))
                        .FirstOrDefault();

                    if (outwardBooking != null)
                    {
                        item.OutwardBookingNo = outwardBooking.FacilityBookingNo;
                        item.OutwardBookingQuantityUOM = outwardBooking.OutwardQuantity;
                    }
                    #endregion
                }
            }
            return items;
        }
        private List<DischargingItem> LoadDischargingItemListForFacility(ProdOrderPartslistPos intermedatePartPos)
        {
            return
                  intermedatePartPos
                  .FacilityBooking_ProdOrderPartslistPos
                  .Where(c => c.Comment != null && c.Comment.Contains(PWBinSelection.BinSelectionReservationComment))
                  .Select(c => new DischargingItem()
                  {
                      ItemID = c.InwardFacilityID ?? Guid.Empty,

                      LotNo = c.FacilityBookingCharge_FacilityBooking.Select(x => x.InwardFacilityCharge.FacilityLot?.LotNo).DefaultIfEmpty().FirstOrDefault(),
                      InwardFacilityNo = c.InwardFacility.FacilityNo,
                      InwardFacilityName = c.InwardFacility.FacilityName,

                      InwardBookingNo = c.FacilityBookingNo,
                      InwardBookingQuantityUOM = c.InwardQuantity,
                      ProdorderPartslistPosID = (c.ProdOrderPartslistPosID ?? Guid.Empty),
                      InwardBookingTime = c.InsertDate, 
                      InwardFacilityChargeID = c.InwardFacilityChargeID
                  }).ToList();
        }

        private List<DischargingItem> LoadDischargingItemListForFacilityCharge(ProdOrderPartslistPos intermedatePartPos)
        {
            return
                  intermedatePartPos
                  .FacilityBooking_ProdOrderPartslistPos
                  .Where(c => c.Comment != null && c.Comment.Contains(PWBinSelection.BinSelectionReservationComment))
                  .Select(c => new DischargingItem()
                  {
                      ItemID = c.FacilityBookingCharge_FacilityBooking.Select(x => x.InwardFacilityCharge.FacilityChargeID).DefaultIfEmpty().FirstOrDefault(),

                      LotNo = c.FacilityBookingCharge_FacilityBooking.Select(x => x.InwardFacilityCharge.FacilityLot.LotNo).DefaultIfEmpty().FirstOrDefault(),
                      InwardFacilityNo = c.FacilityBookingCharge_FacilityBooking.Select(x => x.InwardFacilityCharge.FacilityLot.LotNo).DefaultIfEmpty().FirstOrDefault(),
                      InwardFacilityName = c.FacilityBookingCharge_FacilityBooking.Select(x => x.InwardFacilityCharge.FacilityChargeID.ToString()).DefaultIfEmpty().FirstOrDefault(),
                      
                      InwardBookingNo = c.FacilityBookingNo,
                      InwardBookingQuantityUOM = c.InwardQuantity,
                      ProdorderPartslistPosID = (c.ProdOrderPartslistPosID ?? Guid.Empty),
                      InwardBookingTime = c.InsertDate,
                      InwardFacilityChargeID = c.InwardFacilityChargeID
                  }).ToList();
        }


    }
}
