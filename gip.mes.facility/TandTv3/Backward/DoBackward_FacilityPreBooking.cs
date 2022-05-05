using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class DoBackward_FacilityPreBooking : ItemTracking<FacilityPreBooking>
    {
        #region ctor's
        public DoBackward_FacilityPreBooking(DatabaseApp databaseApp, TandTResult result, FacilityPreBooking item) : base(databaseApp, result, item)
        {
            ItemTypeName = "FacilityPreBooking";
            if (!Result.Ids.Keys.Contains(item.FacilityPreBookingID))
                Result.Ids.Add(item.FacilityPreBookingID, ItemTypeName);
        }
        #endregion

        #region IItemTracking

        public override List<IACObjectEntity> GetSameStepItems()
        {
            List<IACObjectEntity> sameStepItems = new List<IACObjectEntity>();

            if (Item.InwardFacilityCharge != null)
                sameStepItems.Add(Item.InwardFacilityCharge);

            if (Item.OutwardFacilityCharge != null)
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
            FacilityCharge fc = null;
            if (Item.OutwardFacilityCharge != null)
                fc = Item.OutwardFacilityCharge;
            if (fc != null && Item.OutwardMaterial != null)
            {
                var nextFbcs =
                    fc
                    .FacilityLot
                    .FacilityBookingCharge_InwardFacilityLot
                    .Where(c => c.InwardMaterialID == Item.OutwardMaterial.MaterialID)
                    .Where(c => TandTv3Query.s_cQry_FBCInwardQuery(c, Result.Filter, Item.OutwardMaterial?.MaterialID, Item.OutwardFacility?.FacilityID))
                    .OrderBy(c => c.FacilityBookingChargeNo)
                    .ToList();

                nextStepItems.AddRange(nextFbcs);
            }

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
                    FacilityPreBookingPreveiw inwardPreview = new FacilityPreBookingPreveiw()
                    {
                        FacilityPreBookingNo = Item.FacilityPreBookingNo,
                        FacilityNo = (Item.InwardFacility != null) ? Item.InwardFacility.FacilityNo : "",
                        InsertDate = Item.InsertDate,
                        FacilityPreBooking = Item
                    };
                    if (Item.InwardFacilityCharge != null && Item.InwardFacilityCharge.FacilityLotID != null)
                    {
                        inwardPreview.LotNo = Item.InwardFacilityCharge.FacilityLot.LotNo;
                        inwardFacilityLot = Item.InwardFacilityCharge.FacilityLot;
                    }

                    TandTv3Point mixPoint = Result.AddMixPoint(Step, pos, inwardFacilityLot);

                    if (!mixPoint.InwardBookings.Select(c => c.FacilityBookingChargeNo).Contains(Item.FacilityPreBookingNo))
                    {

                        mixPoint.InwardPreBookings.Add(inwardPreview);
                    }

                }
            }

            if (Item.ProdOrderPartslistPosRelationID != null)
            {
                ProdOrderPartslistPos pos = Item.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos;
                if (pos.MaterialPosType == GlobalApp.MaterialPosTypes.InwardPartIntern)
                {

                    FacilityLot inwardFacilityLot = null;
                    FacilityPreBookingPreveiw outwardPrevew = new FacilityPreBookingPreveiw()
                    {
                        FacilityPreBookingNo = Item.FacilityPreBookingNo,
                        FacilityNo = (Item.OutwardFacility != null) ? Item.OutwardFacility.FacilityNo : "",
                        InsertDate = Item.InsertDate,
                        FacilityPreBooking = Item
                    };
                    if (Item.OutwardFacilityCharge != null && Item.OutwardFacilityCharge.FacilityLotID != null)
                    {
                        outwardPrevew.LotNo = Item.OutwardFacilityCharge.FacilityLot.LotNo;
                        inwardFacilityLot = Item.OutwardFacilityCharge.FacilityLot;
                    }

                    TandTv3Point mixPoint = Result.GetMixPoint(Step, pos);
                    if (mixPoint == null)
                        mixPoint = Result.AddMixPoint(Step, pos, inwardFacilityLot);

                    if (!mixPoint.OutwardBookings.Select(c => c.FacilityBookingChargeNo).Contains(Item.FacilityPreBookingNo))
                        mixPoint.OutwardPreBookings.Add(outwardPrevew);

                    if (Item.OutwardFacilityCharge != null && Item.OutwardFacilityCharge.FacilityLotID != null)
                        mixPoint.AddOutwardLotQuantity(Item);

                }
            }

            if (Item.InOrderPosID != null)
            {
                TandTv3Point mixPoint = Result.AddMixPoint(Step, Item.InOrderPos);

                if (!mixPoint.InwardBookings.Select(c => c.FacilityBookingChargeNo).Contains(Item.FacilityPreBookingNo))
                {
                    FacilityPreBookingPreveiw inwardPreview = new FacilityPreBookingPreveiw()
                    {
                        FacilityPreBookingNo = Item.FacilityPreBookingNo,
                        FacilityNo = Item.InwardFacility.FacilityNo,
                        InsertDate = Item.InsertDate,
                        FacilityPreBooking = Item
                    };
                    if (Item.InwardFacilityCharge != null && Item.InwardFacilityCharge.FacilityLotID != null)
                        inwardPreview.LotNo = Item.InwardFacilityCharge.FacilityLot.LotNo;
                    mixPoint.InwardPreBookings.Add(inwardPreview);
                }
            }
        }

        #endregion

    }
}
