using gip.mes.datamodel;
using System.Collections.Generic;

namespace gip.mes.facility.TandTv3
{
    public static class CastDisplayGroupToItemTypeEnum
    {

        public static List<MDTrackingStartItemTypeEnum> Cast(Model.DisplayGroupEnum displayGroupEnum)
        {
            //FacilityPreBooking
            List<MDTrackingStartItemTypeEnum> result = new List<MDTrackingStartItemTypeEnum>();
            switch (displayGroupEnum)
            {
                case Model.DisplayGroupEnum.Storage:
                    result.Add(MDTrackingStartItemTypeEnum.Facility);
                    result.Add(MDTrackingStartItemTypeEnum.FacilityPreview);
                    break;
                case Model.DisplayGroupEnum.Quants:
                    result.Add(MDTrackingStartItemTypeEnum.FacilityCharge);
                    break;
                case Model.DisplayGroupEnum.Lots:
                    result.Add(MDTrackingStartItemTypeEnum.FacilityLot);
                    break;
                case Model.DisplayGroupEnum.Bookings:
                    result.Add(MDTrackingStartItemTypeEnum.FacilityBooking);
                    result.Add(MDTrackingStartItemTypeEnum.FacilityBookingCharge);
                    break;
                case Model.DisplayGroupEnum.Orders:
                    result.Add(MDTrackingStartItemTypeEnum.ProdOrder);
                    result.Add(MDTrackingStartItemTypeEnum.ProdOrderPartslist);
                    result.Add(MDTrackingStartItemTypeEnum.ProdOrderPartslistPos);
                    result.Add(MDTrackingStartItemTypeEnum.ProdOrderPartslistPosRelation);
                    result.Add(MDTrackingStartItemTypeEnum.TandTv3PointDN);
                    //result.Add(MDTrackingStartItemTypeEnum.DeliveryNote);
                    result.Add(MDTrackingStartItemTypeEnum.InOrderPosPreview);
                    result.Add(MDTrackingStartItemTypeEnum.OutOrderPosPreview);
                    result.Add(MDTrackingStartItemTypeEnum.PickingPosPreview);
                    break;
                case Model.DisplayGroupEnum.Machines:
                    result.Add(MDTrackingStartItemTypeEnum.ACClass);
                    break;
                case Model.DisplayGroupEnum.MixPoint:
                    result.Add(MDTrackingStartItemTypeEnum.TandTv3Point);
                    break;
                case Model.DisplayGroupEnum.MixPointGroup:
                    result.Add(MDTrackingStartItemTypeEnum.TandTv3PointPosGrouped);
                    break;
                case Model.DisplayGroupEnum.Material:
                    result.Add(MDTrackingStartItemTypeEnum.Material);
                    break;
                default:
                    break;
            }
            return result;
        }

    }
}
