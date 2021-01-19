using gip.core.datamodel;
using System.Collections.Generic;
using System;

namespace gip.mes.datamodel
{
    public class IACObjectEntityComparer: IComparer<IACObjectEntity>
    {
        public int Compare(IACObjectEntity a, IACObjectEntity b)
        {
            int result = 0;
            if(a.ACIdentifier != b.ACIdentifier)
            {
                StartItemTypeComparer startItemTypeComparer = new StartItemTypeComparer();
                MDTrackingStartItemTypeEnum typA = (MDTrackingStartItemTypeEnum)Enum.Parse(typeof(MDTrackingStartItemTypeEnum), a.ACType.ACIdentifier);
                MDTrackingStartItemTypeEnum typB = (MDTrackingStartItemTypeEnum)Enum.Parse(typeof(MDTrackingStartItemTypeEnum), b.ACType.ACIdentifier);
                result = startItemTypeComparer.Compare(typA, typB);
            }
            else
            {
                MDTrackingStartItemTypeEnum typBooth = (MDTrackingStartItemTypeEnum)Enum.Parse(typeof(MDTrackingStartItemTypeEnum), a.ACType.ACIdentifier);
                switch (typBooth)
                {
                    case MDTrackingStartItemTypeEnum.FacilityBookingCharge:
                        FacilityBookingCharge fbcA = a as FacilityBookingCharge;
                        FacilityBookingCharge fbcB = b as FacilityBookingCharge;
                        result = string.Compare(fbcA.FacilityBookingChargeNo, fbcB.FacilityBookingChargeNo);
                        break;
                    case MDTrackingStartItemTypeEnum.FacilityBooking:
                        FacilityBooking fbA = a as FacilityBooking;
                        FacilityBooking fbB = b as FacilityBooking;
                        result = string.Compare(fbA.FacilityBookingNo, fbB.FacilityBookingNo);
                        break;
                    case MDTrackingStartItemTypeEnum.FacilityPreBooking:
                        FacilityPreBooking fbPreA = a as FacilityPreBooking;
                        FacilityPreBooking fbPreB = b as FacilityPreBooking;
                        result = string.Compare(fbPreA.FacilityPreBookingNo, fbPreB.FacilityPreBookingNo);
                        break;
                    case MDTrackingStartItemTypeEnum.FacilityCharge:
                        FacilityCharge fcA = a as FacilityCharge;
                        FacilityCharge fcB = b as FacilityCharge;
                        result = fcA.FacilityChargeSortNo - fcB.FacilityChargeSortNo;
                        break;
                    case MDTrackingStartItemTypeEnum.FacilityLot:
                        FacilityLot flA = a as FacilityLot;
                        FacilityLot flB = b as FacilityLot;
                        result = string.Compare(flA.LotNo, flB.LotNo);
                        break;
                    case MDTrackingStartItemTypeEnum.Facility:
                        Facility facilityA = a as Facility;
                        Facility facilityB = b as Facility;
                        result = string.Compare(facilityA.FacilityNo, facilityB.FacilityNo);
                        break;
                    case MDTrackingStartItemTypeEnum.ProdOrderPartslistPos:
                        ProdOrderPartslistPos posA = a as ProdOrderPartslistPos;
                        ProdOrderPartslistPos posB = b as ProdOrderPartslistPos;
                        result = posA.Sequence - posB.Sequence;
                        break;
                    case MDTrackingStartItemTypeEnum.ProdOrderPartslistPosRelation:
                        ProdOrderPartslistPosRelation relA = a as ProdOrderPartslistPosRelation;
                        ProdOrderPartslistPosRelation relB = b as ProdOrderPartslistPosRelation;
                        result = relA.Sequence - relB.Sequence;
                        break;
                    case MDTrackingStartItemTypeEnum.ProdOrderPartslist:
                        ProdOrderPartslist plA = a as ProdOrderPartslist;
                        ProdOrderPartslist plB = b as ProdOrderPartslist;
                        result = plA.Sequence - plA.Sequence;
                        break;
                    case MDTrackingStartItemTypeEnum.ProdOrder:
                        ProdOrder prodA = a as ProdOrder;
                        ProdOrder prodB = b as ProdOrder;
                        result = string.Compare(prodA.ProgramNo, prodB.ProgramNo);
                        break;
                    case MDTrackingStartItemTypeEnum.InOrder:
                        InOrder inOrderA = a as InOrder;
                        InOrder inOrderB = b as InOrder;
                        result = string.Compare(inOrderA.InOrderNo, inOrderB.InOrderNo);
                        break;
                    case MDTrackingStartItemTypeEnum.InOrderPosPreview:
                        InOrderPos inOrderPosA = a as InOrderPos;
                        InOrderPos inOrderPosB = b as InOrderPos;
                        result = inOrderPosA.Sequence - inOrderPosB.Sequence;
                        break;
                    case MDTrackingStartItemTypeEnum.OutOrder:
                        OutOrder outOrderA = a as OutOrder;
                        OutOrder outOrderB = b as OutOrder;
                        result = string.Compare(outOrderA.OutOrderNo, outOrderB.OutOrderNo);
                        break;
                    case MDTrackingStartItemTypeEnum.OutOrderPosPreview:
                        OutOrderPos outOrderPosA = a as OutOrderPos;
                        OutOrderPos outOrderPosB = b as OutOrderPos;
                        result = outOrderPosA.Sequence - outOrderPosB.Sequence;
                        break;
                    case MDTrackingStartItemTypeEnum.DeliveryNote:
                        DeliveryNote dnA = a as DeliveryNote;
                        DeliveryNote dnB = b as DeliveryNote;
                        result = string.Compare(dnA.DeliveryNoteNo, dnB.DeliveryNoteNo);
                        break;
                    case MDTrackingStartItemTypeEnum.DeliveryNotePos:
                        break;
                    case MDTrackingStartItemTypeEnum.ACClass:
                        result = string.Compare(a.ACIdentifier, b.ACIdentifier);
                        break;
                    case MDTrackingStartItemTypeEnum.TandTv3Point:
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
    }
}
