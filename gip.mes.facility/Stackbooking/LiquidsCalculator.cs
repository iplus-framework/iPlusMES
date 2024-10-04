using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'LiquidsCalculator'}de{'LiquidsCalculator'}", Global.ACKinds.TACObject, Global.ACStorableTypes.NotStorable, false, false)]
    public class LiquidsCalculator : StandardCalculator
    {
        public new const string ClassName = "LiquidsCalculator";

        public LiquidsCalculator(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _MaxRelevantFacilityCharges = new ACPropertyConfigValue<int>(this, "MaxRelevantFacilityCharges", 5);
        }

        private ACPropertyConfigValue<int> _MaxRelevantFacilityCharges;
        [ACPropertyConfig("en{'Max relevant facility charges in facility'}de{'Max relevant facility charges in facility'}")]
        public int MaxRelevantFacilityCharges
        {
            get => _MaxRelevantFacilityCharges.ValueT;
            set
            {
                _MaxRelevantFacilityCharges.ValueT = value;
                OnPropertyChanged("MaxRelevantFacilityCharges");
            }
        }

        public override Global.ACMethodResultState CalculateInOut(bool isInwardBooking, // false: Outward, true: Inward
                                 bool shiftBookingReverse, // false: normal, true: reverse booking
                                 bool negativeStockAllowed,
                                 Double quantityUOM, MDUnit mdUnitUOM,
                                 FacilityChargeList facilityCharges,
                                 ACMethodBooking BP,
                                 out StackItemList stackItemListInOut,
                                 out MsgBooking msgBookingResult,
                                 bool isRetrogradePosting = false)
        {
            stackItemListInOut = new StackItemList();
            msgBookingResult = new MsgBooking();
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            StackItemList stackItemList = new StackItemList();
            if ((mdUnitUOM == null) || (facilityCharges == null))
            {
                msgBookingResult.AddBookingMessage(MsgBooking.eResultCodes.RequiredParamsNotSet, "Not all passed parameters for LiquidsCalulator are set.");
                bookingResult = Global.ACMethodResultState.Notpossible;
                return bookingResult;
            }

            // Falls Nullbestandsbuchung
            if ((BP.ParamsAdjusted.MDZeroStockState != null)
                && ((BP.ParamsAdjusted.MDZeroStockState.ZeroStockState == MDZeroStockState.ZeroStockStates.BookToZeroStock)
                    || (BP.ParamsAdjusted.MDZeroStockState.ZeroStockState == MDZeroStockState.ZeroStockStates.SetNotAvailable)))
            {
                return base.CalculateInOut(isInwardBooking, shiftBookingReverse, negativeStockAllowed, quantityUOM, mdUnitUOM, facilityCharges, BP, out stackItemListInOut, out msgBookingResult);
            }
            // Falls Sperrung/Freigabe
            else if (BP.ParamsAdjusted.MDReleaseState != null)
            {
                return base.CalculateInOut(isInwardBooking, shiftBookingReverse, negativeStockAllowed, quantityUOM, mdUnitUOM, facilityCharges, BP, out stackItemListInOut, out msgBookingResult);
            }
            // Sonst normale Buchung
            else
            {
                return LiquidsCalculateInOut(isInwardBooking, shiftBookingReverse, negativeStockAllowed, quantityUOM, mdUnitUOM, facilityCharges, BP, stackItemListInOut, msgBookingResult);
            }

            //return bookingResult;
        }

        private Global.ACMethodResultState LiquidsCalculateInOut(bool isInwardBooking, // false: Outward, true: Inward
                                                            bool shiftBookingReverse, // false: normal, true: reverse booking
                                                            bool negativeStockAllowed,
                                                            Double quantityUOM, MDUnit mdUnitUOM,
                                                            FacilityChargeList facilityCharges,
                                                            ACMethodBooking BP,
                                                            StackItemList stackItemListInOut,
                                                            MsgBooking msgBookingResult)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            StackItemList localStackItemListRelocation = new StackItemList();
            StackItemList localStackItemListReOrganize = new StackItemList();

            if (Math.Abs(quantityUOM - 0) < Double.Epsilon)
                return bookingResult;

            int nCountElements = facilityCharges.Count();
            if (nCountElements <= 0)
            {
                msgBookingResult.AddBookingMessage(MsgBooking.eResultCodes.RequiredParamsNotSet, "Passed list with FacilityCharge's is empty.");
                bookingResult = Global.ACMethodResultState.Notpossible;
                return bookingResult;
            }

            bool observeQuantity = false;
            List<ACPartslistManager.QrySilosResult.ReservationInfo> reservations = null;
            Guid[] reservedLots = null;
            if (BP.PartslistPosRelation != null
                && BP.PartslistPosRelation.SourceProdOrderPartslistPos != null
                && BP.PartslistPosRelation.SourceProdOrderPartslistPos.FacilityReservation_ProdOrderPartslistPos.Any())
            {
                reservations = BP.PartslistPosRelation.SourceProdOrderPartslistPos.FacilityReservation_ProdOrderPartslistPos
                                    .Select(c => new ACPartslistManager.QrySilosResult.ReservationInfo() { FacilityLotID = c.FacilityLotID.Value, Quantity = c.ReservedQuantityUOM, ReservationStateIndex = c.ReservationStateIndex })
                                    .ToList();
                observeQuantity = reservations.Any(c => c.IsQuantityObservable);
                if (observeQuantity)
                {
                    ACPartslistManager.QrySilosResult.ReservationInfo.UpdateActualQFromResCollection(reservations,
                    BP.PartslistPosRelation.FacilityBookingCharge_ProdOrderPartslistPosRelation
                        .Where(c => c.OutwardFacilityLotID.HasValue)
                        .Select(c => new { LotID = c.OutwardFacilityLotID.Value, Q = c.OutwardQuantityUOM })
                        .GroupBy(c => c.LotID)
                        .Select(d => new ACPartslistManager.QrySilosResult.ReservationInfo() { FacilityLotID = d.Key, ActualQuantity = d.Sum(e => e.Q) })
                        .ToArray());
                }
                reservedLots = reservations.Select(c => c.FacilityLotID).ToArray();
            }
            else if (BP.PickingPos != null
                && BP.PickingPos.FacilityReservation_PickingPos.Any())
            {
                reservations = BP.PickingPos.FacilityReservation_PickingPos
                                    .Where(c => c.FacilityLotID.HasValue && !c.VBiACClassID.HasValue)
                                    .Select(c => new ACPartslistManager.QrySilosResult.ReservationInfo() { FacilityLotID = c.FacilityLotID.Value, Quantity = c.ReservedQuantityUOM, ReservationStateIndex = c.ReservationStateIndex })
                                    .ToList();
                observeQuantity = reservations.Any(c => c.IsQuantityObservable);
                if (observeQuantity)
                {
                    ACPartslistManager.QrySilosResult.ReservationInfo.UpdateActualQFromResCollection(reservations,
                    BP.PickingPos.FacilityBookingCharge_PickingPos
                        .Where(c => c.OutwardFacilityLotID.HasValue)
                        .Select(c => new { LotID = c.OutwardFacilityLotID.Value, Q = c.OutwardQuantityUOM })
                        .GroupBy(c => c.LotID)
                        .Select(d => new ACPartslistManager.QrySilosResult.ReservationInfo() { FacilityLotID = d.Key, ActualQuantity = d.Sum(e => e.Q) })
                        .ToArray());
                }
                reservedLots = reservations.Select(c => c.FacilityLotID).ToArray();
            }

            int maxRelevantFacilityCharges = 5;
            if (MaxRelevantFacilityCharges > 0)
                maxRelevantFacilityCharges = MaxRelevantFacilityCharges;

            List<FacilityCharge> sortedFacilityCharges = null;
            // Umsortieren der Liste
            if (shiftBookingReverse)
            {
                if (isInwardBooking)
                {
                    if (reservedLots != null)
                    {
                        List<FacilityCharge> preSorted = facilityCharges.Where(c => c.FacilityLotID.HasValue && reservedLots.Contains(c.FacilityLotID.Value)).OrderBy(c => c.FacilityChargeSortNo).ToList();
                        if (BP.TakeOtherLotIfReserved)
                            preSorted.AddRange(facilityCharges.Where(c => !c.FacilityLotID.HasValue || !reservedLots.Contains(c.FacilityLotID.Value)).OrderBy(c => c.FacilityChargeSortNo));
                        sortedFacilityCharges = preSorted;
                    }
                    else
                        sortedFacilityCharges = facilityCharges.OrderBy(c => c.FacilityChargeSortNo).ToList();
                }
                else
                {
                    if (reservedLots != null)
                    {
                        List<FacilityCharge> preSorted = facilityCharges.Where(c => c.FacilityLotID.HasValue && reservedLots.Contains(c.FacilityLotID.Value)).OrderByDescending(c => c.FacilityChargeSortNo).ToList();
                        if (BP.TakeOtherLotIfReserved)
                            preSorted.AddRange(facilityCharges.Where(c => !c.FacilityLotID.HasValue || !reservedLots.Contains(c.FacilityLotID.Value)).OrderByDescending(c => c.FacilityChargeSortNo));
                        sortedFacilityCharges = preSorted;
                    }
                    else
                        sortedFacilityCharges = facilityCharges.OrderByDescending(c => c.FacilityChargeSortNo).ToList();
                }
            }
            else
            {
                if (isInwardBooking)
                {
                    if (reservedLots != null)
                    {
                        List<FacilityCharge> preSorted = facilityCharges.Where(c => c.FacilityLotID.HasValue && reservedLots.Contains(c.FacilityLotID.Value)).OrderByDescending(c => c.FacilityChargeSortNo).ToList();
                        if (BP.TakeOtherLotIfReserved)
                            preSorted.AddRange(facilityCharges.Where(c => !c.FacilityLotID.HasValue || !reservedLots.Contains(c.FacilityLotID.Value)).OrderByDescending(c => c.FacilityChargeSortNo));
                        sortedFacilityCharges = preSorted;
                    }
                    else
                        sortedFacilityCharges = facilityCharges.OrderByDescending(c => c.FacilityChargeSortNo).ToList();
                }
                else
                {
                    if (reservedLots != null)
                    {
                        List<FacilityCharge> preSorted = facilityCharges.Where(c => c.FacilityLotID.HasValue && reservedLots.Contains(c.FacilityLotID.Value)).OrderBy(c => c.FacilityChargeSortNo).ToList();
                        if (BP.TakeOtherLotIfReserved)
                            preSorted.AddRange(facilityCharges.Where(c => !c.FacilityLotID.HasValue || !reservedLots.Contains(c.FacilityLotID.Value)).OrderBy(c => c.FacilityChargeSortNo));
                        sortedFacilityCharges = preSorted;
                    }
                    else
                        sortedFacilityCharges = facilityCharges.OrderBy(c => c.FacilityChargeSortNo).ToList();
                }
            }

            nCountElements = sortedFacilityCharges.Count();
            if (isInwardBooking)
            {
                double quantityPerCharge = quantityUOM / nCountElements;
                foreach (FacilityCharge fc in sortedFacilityCharges)
                {
                    AddQuantityToStackItemList(ItemListId.InOut, fc, quantityPerCharge, mdUnitUOM, false, isInwardBooking, stackItemListInOut,
                                                   localStackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                }
            }
            else
            {
                double ratioQuantity = quantityUOM;

                if (nCountElements > maxRelevantFacilityCharges || quantityUOM < 0)
                {
                    double quantityPerCharge = quantityUOM / nCountElements;

                    List<FacilityCharge> fCWithoutEnoughQ = sortedFacilityCharges.Where(c => c.StockQuantityUOM < quantityPerCharge).ToList();

                    if (fCWithoutEnoughQ.Any())
                    {
                        if (fCWithoutEnoughQ.Count() >= nCountElements && negativeStockAllowed)
                            fCWithoutEnoughQ.Remove(sortedFacilityCharges.LastOrDefault());

                        foreach (FacilityCharge facilityCharge in fCWithoutEnoughQ)
                        {
                            ratioQuantity = ratioQuantity - facilityCharge.StockQuantityUOM;
                            AddQuantityToStackItemList(ItemListId.InOut, facilityCharge, (facilityCharge.StockQuantityUOM * -1), mdUnitUOM, false, isInwardBooking, stackItemListInOut,
                                                       localStackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                            sortedFacilityCharges.Remove(facilityCharge);
                        }
                    }
                    else
                    {
                        foreach (FacilityCharge facilityCharge in sortedFacilityCharges)
                        {
                            AddQuantityToStackItemList(ItemListId.InOut, facilityCharge, (quantityPerCharge * -1), mdUnitUOM, false, isInwardBooking, stackItemListInOut,
                                                   localStackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                        }
                        return bookingResult;
                    }
                }

                double totalQuantity = sortedFacilityCharges.Sum(c => c.StockQuantityUOM);

                if (ratioQuantity > totalQuantity)
                {
                    int loopCounter = 0;

                    foreach (FacilityCharge facilityCharge in sortedFacilityCharges)
                    {
                        loopCounter++;
                        double facilityChargeQuantity = facilityCharge.StockQuantity;
                        if(loopCounter >= nCountElements && negativeStockAllowed)
                        {
                            double sumWithoutLastFC = sortedFacilityCharges.Where(c => c != facilityCharge).Sum(x => x.StockQuantity);
                            facilityChargeQuantity = ratioQuantity - sumWithoutLastFC;

                        }
                        AddQuantityToStackItemList(ItemListId.InOut, facilityCharge, (facilityChargeQuantity * -1), mdUnitUOM, false, isInwardBooking, stackItemListInOut,
                                                   localStackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                    }
                }
                else
                {
                    foreach (FacilityCharge facilityCharge in sortedFacilityCharges)
                    {
                        double facilityChargeRatioQuantity = (facilityCharge.StockQuantity / totalQuantity) * ratioQuantity;
                        AddQuantityToStackItemList(ItemListId.InOut, facilityCharge, (facilityChargeRatioQuantity * -1), mdUnitUOM, false, isInwardBooking, stackItemListInOut,
                                                   localStackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                    }
                }
            }

            return bookingResult;
        }

        //TODO: Anonymous target facility charge
        public override Global.ACMethodResultState CalculateRelocation(bool shiftBookingReverse, bool negativeStockAllowed, StackItemList facilityChargesSource, FacilityChargeList facilityChargesTarget, ACMethodBooking BP, out StackItemList stackItemListRelocation, out MsgBooking msgBookingResult)
        {
            bool isInwardBooking = false;
            StackItemList localStackItemListInOut = new StackItemList();
            stackItemListRelocation = new StackItemList();
            StackItemList localStackItemListReOrganize = new StackItemList();

            msgBookingResult = new MsgBooking();

            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            if ((facilityChargesSource == null) || (facilityChargesTarget == null))
                return Global.ACMethodResultState.Notpossible;

            foreach (var stackItem in facilityChargesSource)
            {
                FacilityCharge targetFacilityCharge = facilityChargesTarget?.FirstOrDefault(c => FacilityChargesAreDifferent(stackItem.FacilityCharge, c) == ItemCompare.Equal);
                if(targetFacilityCharge != null)
                {
                    bookingResult = AddQuantityToStackItemList(ItemListId.Relocation, targetFacilityCharge, stackItem.Quantity * (-1), false, isInwardBooking, localStackItemListInOut, stackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;
                }
                else
                {
                    bookingResult = AddQuantityToStackItemList(ItemListId.Relocation, stackItem.FacilityCharge, stackItem.Quantity * (-1), true, isInwardBooking, localStackItemListInOut, stackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;
                }
            }

            return bookingResult;
        }

        //public override Global.ACMethodResultState ReOrganize(List<FacilityCharge> facilityCharges, out StackItemList stackItemListReOrganize, out MsgBooking msgBookingResult)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
