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

            if (Item?.OutwardFacilityCharge?.FacilityLot?.LotNo == "ZGFL10123338")
            {
                //System.Diagnostics.Debugger.Break();
            }
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
                Guid? materialID = null;
                if (!Result.Filter.IsDisableReworkTracking)
                    materialID = Item.OutwardMaterialID;

                bool isOrderTrackingActive = Result.IsOrderTrackingActive();

                Guid? outwardFacilityID = Item.OutwardFacilityID;
                if (Result.TandTv3Command != null && !Result.TandTv3Command.FilterFaciltiyAtSearchInwardCharges)
                {
                    outwardFacilityID = null;
                }

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
                    .Select(c=> new {c.Key, FCS = c.Select(x=> new {x.InwardFacility?.FacilityNo, x.InwardFacility.FacilityID })})
                    .ToArray();


                if (Result.Filter.OrderDepth != null && Item.ProdOrderPartslistPosRelationID != null)
                {
                    foreach (FacilityBookingCharge fbc in nextFbcs)
                    {
                        Result.AddOrderConnection(fbc, Item);
                    }
                }

                if (isOrderTrackingActive && nextFbcs.Any())
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

        #endregion
    }
}
