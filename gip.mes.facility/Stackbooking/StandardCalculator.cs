using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Defaultcalculator'}de{'Standardcalculator'}", Global.ACKinds.TACObject, Global.ACStorableTypes.NotStorable, false, false)]
    public class StandardCalculator : ACStackCalculatorBase
    {
        public new const string ClassName = "StandardCalculator";

        #region c´tors
        public StandardCalculator(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _SortOrderForRetroFIFO = new ACPropertyConfigValue<short>(this, "SortOrderForRetroFIFO", 0);
        }
        #endregion

        #region Configuration
        private ACPropertyConfigValue<short> _SortOrderForRetroFIFO;
        [ACPropertyConfig("en{'Sort order for retrograde posting'}de{'Sortierreihenfolge für Retrograde Buchung'}", DefaultValue = (int)0)]
        public short SortOrderForRetroFIFO
        {
            get
            {
                return _SortOrderForRetroFIFO.ValueT;
            }
            set
            {
                _SortOrderForRetroFIFO.ValueT = value;
            }
        }

        public SortOrderFIFOEnum SortOrder
        {
            get
            {
                switch (SortOrderForRetroFIFO)
                {
                    case 1:
                        return SortOrderFIFOEnum.ByFirstFillingDate;
                    case 2:
                        return SortOrderFIFOEnum.ByExpirationDate;
                    default:
                        return SortOrderFIFOEnum.ByFacilityChargeSortNo;
                }
            }
        }
        #endregion

        #region CalculateInOut
        /// <summary>
        /// If isOutwardBooking (isInwardBooking is false) and passed booking-values are negative: Stock should be increased
        /// If isInwardBooking and passed booking-values are negative: Stock should be decreased
        /// If Relocation, then this method will be called first on Source-Facility with paramater "isOutwardBooking"
        ///                afterwards, method CalculateRelocation() will be called from the framework
        /// The passed List of facilityCharges is different if Facility is a Cell/Silo or a normal Storage area
        ///     if Cell/Silo: All facilityCharges in the cell are included in the list
        ///     if normal Storage area: the list is reduced depending on the parameters FacilityLot or Material are set
        /// The method returns a integer-casted value of FacilityBookingManager.BookingResult 
        /// The result of the calculation can be retrieved from the framework over the StackItemList of property StackItemListInOut
        /// </summary>
        public override Global.ACMethodResultState CalculateInOut(bool isInwardBooking, // false: Outward, true: Inward
                                 bool shiftBookingReverse, // false: normal, true: reverse booking
                                 bool negativeStockAllowed,
                                 Double quantityUOM, MDUnit mdUnitUOM,
                                 IEnumerable<FacilityCharge> facilityCharges,
                                 ACMethodBooking BP,
                                 out StackItemList stackItemListInOut,
                                 out MsgBooking msgBookingResult,
                                 bool isRetrogradePosting = false)
        {
            stackItemListInOut = new StackItemList();
            msgBookingResult = new MsgBooking();
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            StackItemList stackItemList = new StackItemList();
            if ((mdUnitUOM == null)
                || (facilityCharges == null)
                )
            {
                msgBookingResult.AddBookingMessage(MsgBooking.eResultCodes.RequiredParamsNotSet, "Not all passed parameters for StandardCalulator are set.");
                bookingResult = Global.ACMethodResultState.Notpossible;
                return bookingResult;
            }

            // Falls Nullbestandsbuchung
            if ((BP.ParamsAdjusted.MDZeroStockState != null)
                && ((BP.ParamsAdjusted.MDZeroStockState.ZeroStockState == MDZeroStockState.ZeroStockStates.BookToZeroStock)
                    || (BP.ParamsAdjusted.MDZeroStockState.ZeroStockState == MDZeroStockState.ZeroStockStates.SetNotAvailable)))
            {
                return SetZeroStock(isInwardBooking, shiftBookingReverse, negativeStockAllowed, quantityUOM, mdUnitUOM, facilityCharges, BP, stackItemListInOut, msgBookingResult);
            }

            if ((BP.ParamsAdjusted.MDZeroStockState != null)
                && ((BP.ParamsAdjusted.MDZeroStockState.ZeroStockState == MDZeroStockState.ZeroStockStates.ResetIfNotAvailableFacility)
                    || (BP.ParamsAdjusted.MDZeroStockState.ZeroStockState == MDZeroStockState.ZeroStockStates.RestoreQuantityIfNotAvailable)))
            {
                bool restoreLastQuantity = BP.ParamsAdjusted.MDZeroStockState.ZeroStockState == MDZeroStockState.ZeroStockStates.RestoreQuantityIfNotAvailable;
                return ResetZeroStock(isInwardBooking, shiftBookingReverse, negativeStockAllowed, restoreLastQuantity, quantityUOM, mdUnitUOM, facilityCharges, BP, stackItemListInOut, msgBookingResult);
            }

            // Falls Sperrung/Freigabe
            else if (BP.ParamsAdjusted.MDReleaseState != null)
            {
                return ResetReleaseState(isInwardBooking, shiftBookingReverse, negativeStockAllowed, quantityUOM, mdUnitUOM, facilityCharges, BP, stackItemListInOut, msgBookingResult);
            }
            // Sonst normale Buchung
            else
            {
                return InternCalculateInOut(isInwardBooking, shiftBookingReverse, negativeStockAllowed, quantityUOM, mdUnitUOM, facilityCharges, BP, stackItemListInOut, msgBookingResult, isRetrogradePosting);
            }
        }

        private Global.ACMethodResultState SetZeroStock(bool isInwardBooking, // false: Outward, true: Inward
                                bool shiftBookingReverse, // false: normal, true: reverse booking
                                bool negativeStockAllowed,
                                Double quantityUOM, MDUnit mdUnitUOM,
                                IEnumerable<FacilityCharge> facilityCharges,
                                ACMethodBooking BP,
                                StackItemList stackItemListInOut,
                                MsgBooking msgBookingResult)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            StackItemList localStackItemListRelocation = new StackItemList();
            StackItemList localStackItemListReOrganize = new StackItemList();

            int nCountElements = facilityCharges.Count();
            if (nCountElements <= 0)
            {
                return bookingResult;
            }

            foreach (var facilityCharge in facilityCharges)
            {
                bookingResult = AddQuantityToStackItemList(ItemListId.InOut, facilityCharge, facilityCharge.StockQuantity == 0 ? 0 : facilityCharge.StockQuantity * (-1), false, isInwardBooking, stackItemListInOut, localStackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                    return bookingResult;
            }
            return bookingResult;

        }


        private Global.ACMethodResultState ResetZeroStock(bool isInwardBooking, // false: Outward, true: Inward
                        bool shiftBookingReverse, // false: normal, true: reverse booking
                        bool negativeStockAllowed,
                        bool restoreLastQuantity,
                        Double quantityUOM, MDUnit mdUnitUOM,
                        IEnumerable<FacilityCharge> facilityCharges,
                        ACMethodBooking BP,
                        StackItemList stackItemListInOut,
                        MsgBooking msgBookingResult)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            StackItemList localStackItemListRelocation = new StackItemList();
            StackItemList localStackItemListReOrganize = new StackItemList();

            int nCountElements = facilityCharges.Count();
            if (nCountElements <= 0)
            {
                return bookingResult;
            }

            foreach (var facilityCharge in facilityCharges)
            {
                double quantity = 0;
                if (restoreLastQuantity && facilityCharge.NotAvailable)
                {
                    FacilityBookingCharge fbc = facilityCharge.FacilityBookingCharge_InwardFacilityCharge
                                                              .Where(c => c.FacilityBookingTypeIndex == (short)GlobalApp.FacilityBookingType.ZeroStock_Facility_BulkMaterial)
                                                              .OrderByDescending(c => c.InsertDate)
                                                              .FirstOrDefault();

                    quantity = fbc.InwardQuantity * (-1);
                }

                bookingResult = AddQuantityToStackItemList(ItemListId.InOut, facilityCharge, quantity, false, isInwardBooking, stackItemListInOut, localStackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                    return bookingResult;
            }
            return bookingResult;

        }

        private Global.ACMethodResultState ResetReleaseState(bool isInwardBooking, // false: Outward, true: Inward
                                bool shiftBookingReverse, // false: normal, true: reverse booking
                                bool negativeStockAllowed,
                                Double quantityUOM, MDUnit mdUnitUOM,
                                IEnumerable<FacilityCharge> facilityCharges,
                                ACMethodBooking BP,
                                StackItemList stackItemListInOut,
                                MsgBooking msgBookingResult)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            StackItemList localStackItemListRelocation = new StackItemList();
            StackItemList localStackItemListReOrganize = new StackItemList();

            int nCountElements = facilityCharges.Count();
            if (nCountElements <= 0)
            {
                return bookingResult;
            }

            foreach (var facilityCharge in facilityCharges)
            {
                bookingResult = AddQuantityToStackItemList(ItemListId.InOut, facilityCharge, 0, false, isInwardBooking, stackItemListInOut, localStackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                    return bookingResult;
            }
            return bookingResult;
        }

        private Global.ACMethodResultState InternCalculateInOut(bool isInwardBooking, // false: Outward, true: Inward
                                bool shiftBookingReverse, // false: normal, true: reverse booking
                                bool negativeStockAllowed,
                                Double quantityUOM, MDUnit mdUnitUOM,
                                IEnumerable<FacilityCharge> facilityCharges,
                                ACMethodBooking BP,
                                StackItemList stackItemListInOut,
                                MsgBooking msgBookingResult,
                                bool isRetrogradePosting)
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

            IEnumerable<FacilityCharge> sortedFacilityCharges = null;
            // Umsortieren der Liste
            if (shiftBookingReverse)
            {
                if (isInwardBooking)
                    sortedFacilityCharges = facilityCharges.OrderBy(c => c.FacilityChargeSortNo);
                else
                    sortedFacilityCharges = facilityCharges.OrderByDescending(c => c.FacilityChargeSortNo);
            }
            else
            {
                if (isInwardBooking)
                    sortedFacilityCharges = facilityCharges.OrderByDescending(c => c.FacilityChargeSortNo);
                else
                {
                    if (!isRetrogradePosting || SortOrder == SortOrderFIFOEnum.ByFacilityChargeSortNo)
                        sortedFacilityCharges = facilityCharges.OrderBy(c => c.FacilityChargeSortNo);
                    else if (SortOrder == SortOrderFIFOEnum.ByExpirationDate)
                    {
                        List<FacilityCharge> sortWithExpirationDate = facilityCharges.Where(c => c.FacilityLot != null && c.FacilityLot.ExpirationDate.HasValue).OrderBy(c => c.FacilityLot.ExpirationDate).ToList();
                        sortWithExpirationDate.AddRange(facilityCharges.Where(c => c.FacilityLot == null || !c.FacilityLot.ExpirationDate.HasValue).OrderBy(c => c.FacilityChargeSortNo));
                        sortedFacilityCharges = sortWithExpirationDate;
                    }
                    else // if (SortOrder == SortOrderFIFOEnum.ByFirstFillingDate)
                    {
                        List<FacilityCharge> sortWithExpirationDate = facilityCharges.Where(c => c.FacilityLot != null && c.FacilityLot.FillingDate.HasValue).OrderBy(c => c.FacilityLot.FillingDate).ToList();
                        sortWithExpirationDate.AddRange(facilityCharges.Where(c => c.FacilityLot == null || !c.FacilityLot.FillingDate.HasValue).OrderBy(c => c.FacilityChargeSortNo));
                        sortedFacilityCharges = sortWithExpirationDate;
                    }
                }
            }

            // StockIncrease: Inward and positive, Outward and negative
            // StockDecrease: Inward and negative, Outward and positive
            bool bStockIncrease = false;
            Double quantityAbsoluteUOM = quantityUOM;
            if (quantityUOM > 0)
            {
                if (isInwardBooking)
                    bStockIncrease = true;
                else
                    bStockIncrease = false;
            }
            else
            {
                quantityAbsoluteUOM = Math.Abs(quantityUOM);
                if (isInwardBooking)
                    bStockIncrease = false;
                else
                    bStockIncrease = true;
            }

            // Summen der bisher gebuchten Chargen, gemessen in übergebener quantityUnit und weightUnit
            Double quantityAbsoluteRestUOM = quantityAbsoluteUOM;
            int forEachCounter = 0;

            foreach (FacilityCharge facilityCharge in sortedFacilityCharges)
            {
                forEachCounter++;
                Double facilityChargeQuantityUOM = facilityCharge.StockQuantityUOM;
                // Konvertiere Menge von FaciliyCharge in übergebenen Einheiten
                //try
                //{
                //    facilityChargeQuantityUOM = facilityCharge.Material.ConvertQuantity(facilityCharge.StockQuantity, facilityCharge.MDUnit, mdUnitUOM);
                //}
                //catch (Exception e)
                //{
                //    MsgBookingResult.AddBookingMessage(MsgBooking.eResultCodes.QuantityConversionError, e.Message);
                //    _StackItemListInOut = new StackItemList();
                //    bookingResult = Global.ACMethodResultState.Notpossible;
                //    return bookingResult;
                //}

                Double newFacilityQuantityUOM = 0;

                // PSUEDO-CODE:
                // Falls rechnerischer Zugang
                // Ermittle neuen Restbestand von Charge
                // Falls Restbestand negativ und negativer bestand nicht erlaubt
                // Falls noch weitere Elemente vorhanden
                // Buche facility Charge auf 0
                // Ermittle zu buchende Restmenge
                // gehe zum nächsten Element
                // Falls keine weiteren Elemente vorhanden
                // Buche facility Charge auf 0
                // Rest ist 0
                // Ende Buchung
                // Falls negativer Bestand erlaubt oder Restbestand positiv
                // Buche Restmenge auf Charge
                // Rest ist 0
                // Ende Buchung
                // Falls rechnerischer Abgang (!bStockIncrease)
                // Ist Charge negativ
                // Ist es die letzte Charge?
                // Falls kein negativer Bestand erlaubt
                // Buche gegen 0
                // Ende
                // Sonst negativer Bestand erlaubt
                // Buche Restmenge ab -> charge negativer
                // Ende
                // Sonst weitere Chargen vorhanden
                // buche gegen 0
                // Addiere Restmenge
                // Gehe zur nächsten Charge
                // Ist Charge positiv
                // Ist Charge größer als Restmenge
                // Buche Restmenge ab
                // Ende
                // Sonst Charge kleiner als Restmenge
                // Ist es die letzte Charge ?
                // Falls kein negativer Bestand erlaubt
                // Buche gegen 0
                // Ende
                // Sonst negativer Bestand erlaubt
                // Buche Restmenge ab -> charge negativ
                // Ende
                // Sonst weitere Chargen vorhanden
                // Buche gegen 0
                // Subtrahiere Restmenge
                // Gehe zur nächsten Charge

                // Falls rechnerischer Zugang
                if (bStockIncrease)
                {
                    // Ermittle neuen Restbestand von Charge
                    newFacilityQuantityUOM = facilityChargeQuantityUOM + quantityAbsoluteRestUOM;
                    // Falls Restbestand negativ und negativer bestand nicht erlaubt
                    if ((newFacilityQuantityUOM < 0) && (!negativeStockAllowed))
                    {
                        // Falls noch weitere Elemente vorhanden
                        if (forEachCounter < nCountElements)
                        {
                            // Buche facility Charge auf 0
                            bookingResult = AddQuantityToStackItemList(ItemListId.InOut, facilityCharge, (facilityChargeQuantityUOM * (-1)), mdUnitUOM, false, isInwardBooking, stackItemListInOut, localStackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                            if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                                return bookingResult;

                            // Ermittle zu buchende Restmenge
                            quantityAbsoluteRestUOM += facilityChargeQuantityUOM;

                            // gehe zum nächsten Element
                            continue;
                        }
                        // Falls keine weiteren Elemente vorhanden
                        else
                        {
                            // Buche facility Charge auf 0
                            bookingResult = AddQuantityToStackItemList(ItemListId.InOut, facilityCharge, (facilityChargeQuantityUOM * (-1)), mdUnitUOM, false, isInwardBooking, stackItemListInOut, localStackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                            if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                                return bookingResult;

                            // Rest ist 0
                            quantityAbsoluteRestUOM = 0;

                            // Ende Buchung
                            break;
                        }
                    }
                    // Falls negativer Bestand erlaubt oder Menge positiv
                    else
                    {
                        // Buche Restmenge auf Charge
                        bookingResult = AddQuantityToStackItemList(ItemListId.InOut, facilityCharge, quantityAbsoluteRestUOM, mdUnitUOM, false, isInwardBooking, stackItemListInOut, localStackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                        if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                            return bookingResult;

                        // Rest ist 0
                        quantityAbsoluteRestUOM = 0;

                        // Ende Buchung
                        break;
                    }
                }
                // Falls rechnerischer Abgang (!bStockIncrease)
                else
                {
                    // Ist Charge negativ
                    if (facilityChargeQuantityUOM < 0)
                    {
                        // Ist es die letzte Charge?
                        if (forEachCounter >= nCountElements)
                        {
                            // Falls kein negativer Bestand erlaubt
                            if (!negativeStockAllowed)
                            {
                                // Buche gegen 0
                                bookingResult = AddQuantityToStackItemList(ItemListId.InOut, facilityCharge, (facilityChargeQuantityUOM * (-1)), mdUnitUOM, false, isInwardBooking, stackItemListInOut, localStackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                                if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                                    return bookingResult;

                                // Rest ist 0
                                quantityAbsoluteRestUOM = 0;

                                // Ende Buchung
                                break;
                            }
                            // Sonst negativer Bestand erlaubt
                            else
                            {
                                // Buche Restmenge ab -> charge negativer
                                bookingResult = AddQuantityToStackItemList(ItemListId.InOut, facilityCharge, (quantityAbsoluteRestUOM * (-1)), mdUnitUOM, false, isInwardBooking, stackItemListInOut, localStackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                                if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                                    return bookingResult;

                                // Rest ist 0
                                quantityAbsoluteRestUOM = 0;

                                // Ende Buchung
                                break;
                            }
                        }
                        // Sonst weitere Chargen vorhanden
                        else
                        {
                            // buche gegen 0
                            bookingResult = AddQuantityToStackItemList(ItemListId.InOut, facilityCharge, (facilityChargeQuantityUOM * (-1)), mdUnitUOM, false, isInwardBooking, stackItemListInOut, localStackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                            if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                                return bookingResult;

                            // Subtrahiere negativen Bestand auf Restmenge (= Addiere auf Restmenge)
                            quantityAbsoluteRestUOM -= facilityChargeQuantityUOM;

                            // Gehe zur nächsten Charge
                            continue;
                        }
                    }
                    // Ist Charge positiv
                    else
                    {
                        // Ist Charge größer als Restmenge
                        if (facilityChargeQuantityUOM >= quantityAbsoluteRestUOM)
                        {
                            // Buche Restmenge ab
                            bookingResult = AddQuantityToStackItemList(ItemListId.InOut, facilityCharge, (quantityAbsoluteRestUOM * (-1)), mdUnitUOM, false, isInwardBooking, stackItemListInOut, localStackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                            if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                                return bookingResult;

                            // Rest ist 0
                            quantityAbsoluteRestUOM = 0;

                            // Ende Buchung
                            break;
                        }
                        // Sonst Charge kleiner als Restmenge
                        else
                        {
                            // Ist es die letzte Charge ?
                            if (forEachCounter >= nCountElements)
                            {
                                // Falls kein negativer Bestand erlaubt
                                if (!negativeStockAllowed)
                                {
                                    // Buche gegen 0
                                    bookingResult = AddQuantityToStackItemList(ItemListId.InOut, facilityCharge, (facilityChargeQuantityUOM * (-1)), mdUnitUOM, false, isInwardBooking, stackItemListInOut, localStackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                                        return bookingResult;

                                    // Rest ist 0
                                    quantityAbsoluteRestUOM = 0;

                                    // Ende Buchung
                                    break;
                                }
                                // Sonst negativer Bestand erlaubt
                                else
                                {
                                    // Buche Restmenge ab -> charge negativ
                                    bookingResult = AddQuantityToStackItemList(ItemListId.InOut, facilityCharge, (quantityAbsoluteRestUOM * (-1)), mdUnitUOM, false, isInwardBooking, stackItemListInOut, localStackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                                        return bookingResult;

                                    // Rest ist 0
                                    quantityAbsoluteRestUOM = 0;

                                    // Ende Buchung
                                    break;
                                }
                            }
                            // Sonst weitere Chargen vorhanden
                            else
                            {
                                // Buche gegen 0
                                bookingResult = AddQuantityToStackItemList(ItemListId.InOut, facilityCharge, (facilityChargeQuantityUOM * (-1)), mdUnitUOM, false, isInwardBooking, stackItemListInOut, localStackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                                if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                                    return bookingResult;

                                // Subtrahiere Restmenge
                                quantityAbsoluteRestUOM -= facilityChargeQuantityUOM;

                                // Gehe zur nächsten Charge   
                                continue;
                            }
                        }
                    }
                }
            }
            return bookingResult;
        }
        
        #endregion


        #region Relocation
        /// <summary>
        /// Only if Relocation:
        /// facilityChargesSource is IStackItemList of Facility-Charges which was polpulated with call Calculate() before
        /// facilityChargesTarget is List of Facility-Charges on the Destination-Facility
        /// The List is different if Facility is a Cell/Silo or a normal Storage area
        ///     if Cell/Silo: All facilityCharges in the cell are included in the list
        ///     if normal Storage area: the list is reduced depending on the parameters FacilityLot or Material are set
        /// The method returns a integer-casted value of FacilityBookingManager.BookingResult 
        /// The result of the calculation can be retrieved from the framework over the StackItemList of property StackItemListRelocation
        /// </summary>
        public override Global.ACMethodResultState CalculateRelocation(bool shiftBookingReverse, // false: normal, true: reverse booking
                                bool negativeStockAllowed,
                                StackItemList facilityChargesSource,
                                IEnumerable<FacilityCharge> facilityChargesTarget,
                                ACMethodBooking BP,
                                out StackItemList stackItemListRelocation,
                                out MsgBooking msgBookingResult)
        {
            bool isInwardBooking = false;
            StackItemList localStackItemListInOut = new StackItemList();
            stackItemListRelocation = new StackItemList();
            StackItemList localStackItemListReOrganize = new StackItemList();

            msgBookingResult = new MsgBooking();

            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            if ((facilityChargesSource == null) || (facilityChargesTarget == null))
                return Global.ACMethodResultState.Notpossible;

            // Zugangsbuchung von normalem Schichtenmodell betrachtet nur die physikalisch oberste Schicht
            // Falls diese gleich der ersten Charge ist auf der facilityChargesSource-Liste,
            // dann aktualisiere Schicht ansonsten Neuanlage auf dem Silo

            IOrderedEnumerable<FacilityCharge> sortedFacilityChargesTarget = null;
            FacilityCharge facilityChargeOnTop = null;
            if (facilityChargesTarget.Any())
            {
                // Umsortieren der Liste
                if (shiftBookingReverse)
                    sortedFacilityChargesTarget = facilityChargesTarget.OrderBy(c => c.FacilityChargeSortNo);
                else
                    sortedFacilityChargesTarget = facilityChargesTarget.OrderByDescending(c => c.FacilityChargeSortNo);
                facilityChargeOnTop = sortedFacilityChargesTarget.First();
            }

            int forEachCounter = 0;
            foreach (var stackItem in facilityChargesSource)
            {
                forEachCounter++;
                if (forEachCounter == 1)
                {
                    ItemCompare compareRes = FacilityChargesAreDifferent(stackItem.FacilityCharge, facilityChargeOnTop);
                    if ((compareRes == ItemCompare.Equal) || (compareRes == ItemCompare.Anonymous))
                    {
                        // Mache aus anonymer Charge eine bekannte Charge
                        if (compareRes == ItemCompare.Anonymous)
                            facilityChargeOnTop.FacilityLot = stackItem.FacilityCharge.FacilityLot;

                        bookingResult = AddQuantityToStackItemList(ItemListId.Relocation, facilityChargeOnTop, stackItem.Quantity * (-1), false, isInwardBooking, localStackItemListInOut, stackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                        if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                            return bookingResult;

                        //// Buchung auf vorhandene Charge
                        //Double facilityChargeQuantity = 0; // in übergebenen quantityUnit's
                        //// Konvertiere Menge von FaciliyCharge in übergebenen Einheiten
                        //try
                        //{
                        //    facilityChargeQuantity = facilityChargeOnTop.Material.ConvertQuantity(stackItem.Quantity, stackItem.FacilityCharge.MDUnit, facilityChargeOnTop.MDUnit);

                        //    bookingResult = AddQuantityToStackItemList(ItemListId.Relocation, facilityChargeOnTop, (facilityChargeQuantity * (-1)), facilityChargeOnTop.MDUnit, false);
                        //    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        //        return bookingResult;
                        //}
                        //catch (Exception e)
                        //{
                        //    MsgBookingResult.AddBookingMessage(MsgBooking.eResultCodes.QuantityConversionError, e.Message);
                        //    _StackItemListInOut = new StackItemList();
                        //    bookingResult = Global.ACMethodResultState.Notpossible;
                        //    return bookingResult;
                        //}
                        continue;
                    }
                    else
                    {
                        // Lege neue Schicht an
                        //bookingResult = AddQuantityToStackItemList(ItemListId.Relocation, stackItem.FacilityCharge, (stackItem.Quantity * (-1)), stackItem.FacilityCharge.MDUnit, true);
                        bookingResult = AddQuantityToStackItemList(ItemListId.Relocation, stackItem.FacilityCharge, stackItem.Quantity * (-1), true, isInwardBooking, localStackItemListInOut, stackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                        if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                            return bookingResult;
                    }
                }
                else
                {
                    // Lege neue Schicht an
                    //bookingResult = AddQuantityToStackItemList(ItemListId.Relocation, stackItem.FacilityCharge, (stackItem.Quantity * (-1)), stackItem.FacilityCharge.MDUnit, true);
                    bookingResult = AddQuantityToStackItemList(ItemListId.Relocation, stackItem.FacilityCharge, stackItem.Quantity * (-1), true, isInwardBooking, localStackItemListInOut, stackItemListRelocation, localStackItemListReOrganize, msgBookingResult);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;
                }

            }
            return bookingResult;
        }

        protected enum ItemCompare
        {
            Equal = 0,
            Different = 1,
            SplitNo = 2, // Unterscheidet sich bezüglich der Split-Nummmer
            Anonymous = 3, // Anonyme Charge
        }

        protected ItemCompare FacilityChargesAreDifferent(FacilityCharge facilityChargeSource, FacilityCharge facilityChargeDest)
        {
            if ((facilityChargeSource == null) || (facilityChargeDest == null))
                return ItemCompare.Different;
            if (facilityChargeSource.Material.MaterialID != facilityChargeDest.Material.MaterialID)
                return ItemCompare.Different;
            if (facilityChargeSource.FacilityLot != null)
            {
                if (facilityChargeDest.FacilityLot != null)
                {
                    if (facilityChargeSource.FacilityLot.FacilityLotID != facilityChargeDest.FacilityLot.FacilityLotID)
                        return ItemCompare.Different;
                }
                else
                    return ItemCompare.Anonymous;
            }
            else
            {
                if (facilityChargeDest.FacilityLot != null)
                    return ItemCompare.Different;
            }
            if (facilityChargeSource.SplitNo != facilityChargeDest.SplitNo)
                return ItemCompare.SplitNo;

            return ItemCompare.Equal;
        }



        #region ReOrganize
        /// <summary>
        /// If framework has inserted a new FacilityCharge on a Facility, afterwards the framework will call
        /// this method. This method Reorganizes the FacilityCharges.
        /// The method returns a integer-casted value of FacilityBookingManager.BookingResult
        /// The result of the calculation can be retrieved from the framework over the StackItemList of property StackItemListReOrganize
        /// </summary>
        public override Global.ACMethodResultState ReOrganize(IEnumerable<FacilityCharge> facilityCharges, out StackItemList stackItemListReOrganize, out MsgBooking msgBookingResult)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            msgBookingResult = new MsgBooking();

            // Empty Result-List
            stackItemListReOrganize = new StackItemList();

            return bookingResult;
        }

        #endregion

        #region private methods
        protected enum ItemListId
        {
            InOut = 1,
            Relocation = 2,
            Reorganize = 3,
        }

        // Adds Quantity in BausUnit of Measure
        protected Global.ACMethodResultState AddQuantityToStackItemList(ItemListId whichItemList,
                                 FacilityCharge facilityCharge,
                                 Double quantityUOM,
                                 MDUnit mdUnitUOM,
                                 bool facilityChargeIsNew,
                                 bool isInwardBooking,
                                 StackItemList localStackItemListInOut,
                                 StackItemList localStackItemListRelocation,
                                 StackItemList localStackItemListReOrganize,
                                 MsgBooking msgBookingResult)
        {
            Double facilityChargeQuantity = 0; // in quantityUnit's von FacilityCharge
            try
            {
                facilityChargeQuantity = facilityCharge.Material.ConvertQuantity(quantityUOM, mdUnitUOM, facilityCharge.MDUnit);
                /// Falls Abgangsbuchung, dann subtrahiert FacilityBookingManager die Werte, daher muss invertiert werden
                /// Beispiel: Chargenbestand war negativ und sollte auf null gebucht werden 
                ///    -> Quantity ist positiv 
                ///    -> Wegen Outward-Buchung muss menge invertiert werden
                ///    Wenn Outwardbuchung durchgeführt wird, dann wird gerechnet z.B. (-30 Kg Chargenbestand) - (-30 kg Buchungsmenge) = 0 kg
                if (!isInwardBooking)
                {
                    facilityChargeQuantity *= (-1);
                    quantityUOM *= (-1);
                }
                if (whichItemList == ItemListId.InOut)
                    localStackItemListInOut.Add(new StackItem(facilityCharge, facilityChargeQuantity, quantityUOM, facilityChargeIsNew));
                else if (whichItemList == ItemListId.Relocation)
                    localStackItemListRelocation.Add(new StackItem(facilityCharge, facilityChargeQuantity, quantityUOM, facilityChargeIsNew));
                else // (whichItemList == ItemListId.Reorganize)
                    localStackItemListReOrganize.Add(new StackItem(facilityCharge, facilityChargeQuantity, quantityUOM, facilityChargeIsNew));
            }
            catch (Exception e)
            {
                msgBookingResult.AddBookingMessage(MsgBooking.eResultCodes.QuantityConversionError, e.Message);
                //if (whichItemList == ItemListId.InOut)
                //    _StackItemListInOut = new StackItemList();
                //else if (whichItemList == ItemListId.Relocation)
                //    _StackItemListRelocation = new StackItemList();
                //else // (whichItemList == ItemListId.Reorganize)
                //    _StackItemListReOrganize = new StackItemList();
                return Global.ACMethodResultState.Notpossible;
            }

            return Global.ACMethodResultState.Succeeded;
        }

        // Adds Quantity in Facility Qunatity
        protected Global.ACMethodResultState AddQuantityToStackItemList(ItemListId whichItemList,
                                 FacilityCharge facilityCharge,
                                 Double facilityChargeQuantity,
                                 bool facilityChargeIsNew,
                                 bool isInwardBooking,
                                 StackItemList localStackItemListInOut,
                                 StackItemList localStackItemListRelocation,
                                 StackItemList localStackItemListReOrganize,
                                 MsgBooking msgBookingResult)
        {
            Double quantityUOM = 0; // in quantityUnit's von FacilityCharge
            try
            {
                quantityUOM = facilityCharge.Material.ConvertToBaseQuantity(facilityChargeQuantity, facilityCharge.MDUnit);
                /// Falls Abgangsbuchung, dann subtrahiert FacilityBookingManager die Werte, daher muss invertiert werden
                /// Beispiel: Chargenbestand war negativ und sollte auf null gebucht werden 
                ///    -> Quantity ist positiv 
                ///    -> Wegen Outward-Buchung muss menge invertiert werden
                ///    Wenn Outwardbuchung durchgeführt wird, dann wird gerechnet z.B. (-30 Kg Chargenbestand) - (-30 kg Buchungsmenge) = 0 kg
                if (!isInwardBooking)
                {
                    facilityChargeQuantity *= (-1);
                    quantityUOM *= (-1);
                }
                if (whichItemList == ItemListId.InOut)
                    localStackItemListInOut.Add(new StackItem(facilityCharge, facilityChargeQuantity, quantityUOM, facilityChargeIsNew));
                else if (whichItemList == ItemListId.Relocation)
                    localStackItemListRelocation.Add(new StackItem(facilityCharge, facilityChargeQuantity, quantityUOM, facilityChargeIsNew));
                else // (whichItemList == ItemListId.Reorganize)
                    localStackItemListReOrganize.Add(new StackItem(facilityCharge, facilityChargeQuantity, quantityUOM, facilityChargeIsNew));
            }
            catch (Exception e)
            {
                msgBookingResult.AddBookingMessage(MsgBooking.eResultCodes.QuantityConversionError, e.Message);
                //if (whichItemList == ItemListId.InOut)
                //    _StackItemListInOut = new StackItemList();
                //else if (whichItemList == ItemListId.Relocation)
                //    _StackItemListRelocation = new StackItemList();
                //else // (whichItemList == ItemListId.Reorganize)
                //    _StackItemListReOrganize = new StackItemList();
                return Global.ACMethodResultState.Notpossible;
            }

            return Global.ACMethodResultState.Succeeded;
        }
        #endregion

        #endregion
    }
}
