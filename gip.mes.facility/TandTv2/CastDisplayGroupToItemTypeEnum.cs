using gip.mes.datamodel;
using System.Collections.Generic;

namespace gip.mes.facility
{
    public static class CastDisplayGroupToItemTypeEnum
    {

        public static  List<TandTv2ItemTypeEnum> Cast(DisplayGroupEnum displayGroupEnum)
        {
            //FacilityPreBooking
            List<TandTv2ItemTypeEnum> result = new List<TandTv2ItemTypeEnum>();
            switch (displayGroupEnum)
            {
                case DisplayGroupEnum.Storage:
                    result.Add(TandTv2ItemTypeEnum.Facility);
                    break;
                case DisplayGroupEnum.Quants:
                    result.Add(TandTv2ItemTypeEnum.FacilityCharge);
                    break;
                case DisplayGroupEnum.Lots:
                    result.Add(TandTv2ItemTypeEnum.FacilityLot);
                    break;
                case DisplayGroupEnum.Bookings:
                    result.Add(TandTv2ItemTypeEnum.FacilityBooking);
                    result.Add(TandTv2ItemTypeEnum.FacilityBookingCharge);
                    result.Add(TandTv2ItemTypeEnum.DeliveryNote);
                    result.Add(TandTv2ItemTypeEnum.DeliveryNotePos);
                    result.Add(TandTv2ItemTypeEnum.InOrder);
                    result.Add(TandTv2ItemTypeEnum.InOrderPos);
                    result.Add(TandTv2ItemTypeEnum.OutOrder);
                    result.Add(TandTv2ItemTypeEnum.OutOrderPos);
                    break;
                case DisplayGroupEnum.Orders:
                    result.Add(TandTv2ItemTypeEnum.ProdOrder);
                    result.Add(TandTv2ItemTypeEnum.ProdOrderPartslist);
                    result.Add(TandTv2ItemTypeEnum.ProdOrderPartslistPos);
                    result.Add(TandTv2ItemTypeEnum.ProdOrderPartslistPosRelation);
                    break;
                case DisplayGroupEnum.Machines:
                    result.Add(TandTv2ItemTypeEnum.ACClass);
                    break;
                default:
                    break;
            }
            return result;
        }

    }
}
