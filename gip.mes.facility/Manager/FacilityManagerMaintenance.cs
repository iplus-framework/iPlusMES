using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using gip.mes.datamodel;
using System.Data.SqlClient;

namespace gip.mes.facility
{
    public partial class FacilityManager
    {
        #region Matching
        protected virtual Global.ACMethodResultState DoMatching(ACMethodBooking BP)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            if (BP == null)
                return Global.ACMethodResultState.Notpossible;
            FacilityBooking FB = NewFacilityBooking(BP);
            InitProgressTotalRange(BP, 1);

            switch (BP.BookingType)
            {
                case GlobalApp.FacilityBookingType.MatchingFacilityChargeQuantities:
                case GlobalApp.FacilityBookingType.MatchingFacilityChargeQuantitiesAll:
                    bookingResult = DoMatchingFacilityChargeQuantites(BP, FB);
                    break;

                case GlobalApp.FacilityBookingType.MatchingMaterialStock:
                case GlobalApp.FacilityBookingType.MatchingMaterialStockAll:
                    bookingResult = DoMatchingMaterialStock(BP, FB);
                    break;

                case GlobalApp.FacilityBookingType.MatchingCompanyMaterialStock:
                case GlobalApp.FacilityBookingType.MatchingCompanyMaterialStockAll:
                    bookingResult = DoMatchingCompanyMaterialStock(BP, FB);
                    break;

                case GlobalApp.FacilityBookingType.MatchingFacilityStock:
                case GlobalApp.FacilityBookingType.MatchingFacilityStockAll:
                    bookingResult = DoMatchingFacilityStock(BP, FB);
                    break;

                case GlobalApp.FacilityBookingType.MatchingFacilityLotStock:
                case GlobalApp.FacilityBookingType.MatchingFacilityLotStockAll:
                    bookingResult = DoMatchingFacilityLotStock(BP, FB);
                    break;

                case GlobalApp.FacilityBookingType.MatchingPartslistStock:
                case GlobalApp.FacilityBookingType.MatchingPartslistStockAll:
                    bookingResult = DoMatchingPartslistStock(BP, FB);
                    break;

                case GlobalApp.FacilityBookingType.MatchingStockAll:
                    InitProgressTotalRange(BP, 5);

                    bookingResult = DoMatchingFacilityChargeQuantites(BP, FB);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;

                    bookingResult = DoMatchingMaterialStock(BP, FB);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;

                    bookingResult = DoMatchingCompanyMaterialStock(BP, FB);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;

                    bookingResult = DoMatchingFacilityStock(BP, FB);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;

                    bookingResult = DoMatchingFacilityLotStock(BP, FB);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;

                    bookingResult = DoMatchingPartslistStock(BP, FB);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;
                    break;
            }

            return bookingResult;
        }

        #region MatchingFacilityChargeQuantites
        protected virtual Global.ACMethodResultState DoMatchingFacilityChargeQuantites(ACMethodBooking BP, FacilityBooking FB)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;

            try
            {
                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Error00044"));
                if (BP.ParamsAdjusted.InwardFacilityCharge != null)
                {
                    bookingResult = DoMatchingFacilityChargeQuantites(BP, FB, BP.ParamsAdjusted.InwardFacilityCharge);
                }
                else
                {
                    var query = from c in BP.DatabaseApp.FacilityCharge where c.NotAvailable == false select c;
                    if (query != null)
                    {
                        InitProgressTotalRange(BP, query.Count());
                        // BP.VBProgress.Start();
                        // BP.VBProgress.AddTask("DoMatchingFacilityChargeQuantites", 0, query.Count());
                    }
                    
                    foreach (var facilityCharge in query)
                    {
                        Global.ACMethodResultState bookingResult2 = DoMatchingFacilityChargeQuantites(BP, FB, facilityCharge);
                        if (bookingResult2 < bookingResult)
                            bookingResult = bookingResult2;

                        ReportProgressSub(BP);
                    }
                }
                ReportProgressTotal(BP);
            }
            catch (Exception e)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.TransactionError, e.Message);
                bookingResult = Global.ACMethodResultState.Failed;
            }
            return bookingResult;
        }

        protected virtual Global.ACMethodResultState DoMatchingFacilityChargeQuantites(ACMethodBooking BP, FacilityBooking FB, FacilityCharge facilityCharge)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            if (facilityCharge == null)
                return bookingResult;
            if (facilityCharge.Material == null)
                return bookingResult;
            if (facilityCharge.Material.BaseMDUnit == null)
                return bookingResult;

            Double convVal = facilityCharge.Material.ConvertQuantity(facilityCharge.StockQuantity, facilityCharge.MDUnit, facilityCharge.Material.BaseMDUnit);
            if (Math.Abs(convVal - facilityCharge.StockQuantityUOM) > Double.Epsilon)
                facilityCharge.StockQuantityUOM = convVal;

            // TODO Damir: Umrechnungsfunktionen checken
            //if (Math.Abs(convVal - facilityCharge.StockWeightInMU) > Double.Epsilon)
            //    facilityCharge.StockWeightInMU = convVal;

            //convVal = facilityCharge.Material.QuantityToWeight(facilityCharge.StockQuantity, facilityCharge.MDUnit);
            //if (Math.Abs(convVal - facilityCharge.StockWeight) > Double.Epsilon)
            //    facilityCharge.StockWeight = convVal;

            return bookingResult;
        }
        #endregion

        #region MatchingMaterialStock
        protected virtual Global.ACMethodResultState DoMatchingMaterialStock(ACMethodBooking BP, FacilityBooking FB)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;

            try
            {
                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Error00047"));
                if (BP.ParamsAdjusted.InwardMaterial != null)
                {
                    bookingResult = DoMatchingMaterialStock(BP, FB, BP.ParamsAdjusted.InwardMaterial);
                }
                else
                {
                    var query = from c in BP.DatabaseApp.Material select c;
                    if (query != null)
                    {

                    }
                        InitProgressSubRange(BP, query.Count());
                    foreach (var material in query)
                    {
                        Global.ACMethodResultState bookingResult2 = DoMatchingMaterialStock(BP, FB, material);
                        if (bookingResult2 < bookingResult)
                            bookingResult = bookingResult2;

                        ReportProgressSub(BP);
                    }
                }
                ReportProgressTotal(BP);
            }
            catch (Exception e)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.TransactionError, e.Message);
                bookingResult = Global.ACMethodResultState.Failed;
            }

            return bookingResult;
        }

        protected virtual Global.ACMethodResultState DoMatchingMaterialStock(ACMethodBooking BP, FacilityBooking FB, Material material)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            if (material == null || material.MDFacilityManagementType == null || material.MDFacilityManagementType.FacilityManagementType == MDFacilityManagementType.FacilityManagementTypes.NoFacility)
                return bookingResult;
            var query = material.FacilityCharge_Material.Where(c => c.NotAvailable == false);
            if (material.CurrentMaterialStock == null && !query.Any() && !CreateStockOnMaterialMatching)
                return bookingResult;

            MaterialStock materialStock = material.GetMaterialStock_InsertIfNotExists(BP.DatabaseApp);
            Double sumVal = query.Sum(o => o.StockQuantityUOM);
            if (Math.Abs(sumVal - materialStock.StockQuantity) > Double.Epsilon)
            {
                Messages.LogInfo(this.GetACUrl(), "DoMatchingMaterialStock", String.Format("MaterialStock at {0}/{1} different. Old: {2}, New: {3}, Diff: {4}.", material.MaterialNo, material.MaterialName1, materialStock.StockQuantity, sumVal, sumVal - materialStock.StockQuantity));
                materialStock.StockQuantity = sumVal;
            }
            else if (Math.Abs(sumVal) <= Double.Epsilon)
                materialStock.StockQuantity = 0;
            sumVal = material.FacilityCharge_Material.Where(c => c.NotAvailable == false).Sum(o => o.StockQuantityUOMAmb);
            if (Math.Abs(sumVal - materialStock.StockQuantityAmb) > Double.Epsilon)
                materialStock.StockQuantityAmb = sumVal;
            else if (Math.Abs(sumVal) <= Double.Epsilon)
                materialStock.StockQuantityAmb = 0;
            return bookingResult;
        }
        #endregion

        #region MatchingCompanyMaterialStock
        protected virtual Global.ACMethodResultState DoMatchingCompanyMaterialStock(ACMethodBooking BP, FacilityBooking FB)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;

            try
            {
                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Error00047"));
                if (BP.ParamsAdjusted.InwardCompanyMaterial != null)
                {
                    bookingResult = DoMatchingCompanyMaterialStock(BP, FB, BP.ParamsAdjusted.InwardCompanyMaterial);
                }
                else
                {
                    var query = from c in BP.DatabaseApp.CompanyMaterial where c.CMTypeIndex == (short)GlobalApp.CompanyMaterialTypes.MaterialMapping select c;
                    if (query != null)
                        InitProgressSubRange(BP, query.Count());
                    foreach (var material in query)
                    {
                        Global.ACMethodResultState bookingResult2 = DoMatchingCompanyMaterialStock(BP, FB, material);
                        if (bookingResult2 < bookingResult)
                            bookingResult = bookingResult2;

                        ReportProgressSub(BP);
                    }
                }
                ReportProgressTotal(BP);
            }
            catch (Exception e)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.TransactionError, e.Message);
                bookingResult = Global.ACMethodResultState.Failed;
            }

            return bookingResult;
        }

        protected virtual Global.ACMethodResultState DoMatchingCompanyMaterialStock(ACMethodBooking BP, FacilityBooking FB, CompanyMaterial material)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            if (material == null || material.CMMype != GlobalApp.CompanyMaterialTypes.MaterialMapping)
                return bookingResult;

            var query = material.FacilityCharge_CompanyMaterial.Where(c => c.NotAvailable == false);
            var query2 = material.FacilityCharge_CPartnerCompanyMaterial.Where(c => c.NotAvailable == false);
            if (material.CurrentCompanyMaterialStock == null 
                && !query.Any() 
                && !query2.Any() 
                && !material.FacilityBookingCharge_InwardCPartnerCompMat.Any() 
                && !material.FacilityBookingCharge_OutwardCPartnerCompMat.Any()
                && !CreateStockOnCompanyMaterialMatching)
                return bookingResult;

            CompanyMaterialStock materialStock = material.GetCompanyMaterialStock_InsertIfNotExists(BP.DatabaseApp);
            if (query.Any())
            {
                Double sumVal = query.Sum(o => o.StockQuantityUOM);
                if (Math.Abs(sumVal - materialStock.StockQuantity) > Double.Epsilon)
                    materialStock.StockQuantity = sumVal;
                else if (Math.Abs(sumVal) <= Double.Epsilon)
                    materialStock.StockQuantity = 0;
                sumVal = query.Sum(o => o.StockQuantityUOMAmb);
                if (Math.Abs(sumVal - materialStock.StockQuantityAmb) > Double.Epsilon)
                    materialStock.StockQuantityAmb = sumVal;
                else if (Math.Abs(sumVal) <= Double.Epsilon)
                    materialStock.StockQuantityAmb = 0;
            }
            else if (query2.Any())
            {
                Double sumVal = query2.Sum(o => o.StockQuantityUOM);
                if (Math.Abs(sumVal - materialStock.StockQuantity) > Double.Epsilon)
                    materialStock.StockQuantity = sumVal;
                else if (Math.Abs(sumVal) <= Double.Epsilon)
                    materialStock.StockQuantity = 0;
                sumVal = query2.Sum(o => o.StockQuantityUOMAmb);
                if (Math.Abs(sumVal - materialStock.StockQuantityAmb) > Double.Epsilon)
                    materialStock.StockQuantityAmb = sumVal;
                else if (Math.Abs(sumVal) <= Double.Epsilon)
                    materialStock.StockQuantityAmb = 0;
            }
            else if (material.FacilityBookingCharge_InwardCPartnerCompMat.Any() || material.FacilityBookingCharge_OutwardCPartnerCompMat.Any())
            {
                Double sumVal1 = material.FacilityBookingCharge_InwardCPartnerCompMat.Sum(o => o.InwardQuantityUOM);
                Double sumVal2 = material.FacilityBookingCharge_OutwardCPartnerCompMat.Sum(o => o.OutwardQuantityUOM);
                Double sumVal = sumVal1 - sumVal2;
                if (Math.Abs(sumVal - materialStock.StockQuantity) > Double.Epsilon)
                    materialStock.StockQuantity = sumVal;
                else if (Math.Abs(sumVal) <= Double.Epsilon)
                    materialStock.StockQuantity = 0;
                sumVal1 = material.FacilityBookingCharge_InwardCPartnerCompMat.Sum(o => o.InwardQuantityUOMAmb);
                sumVal2 = material.FacilityBookingCharge_OutwardCPartnerCompMat.Sum(o => o.OutwardQuantityUOMAmb);
                sumVal = sumVal1 - sumVal2;
                if (Math.Abs(sumVal - materialStock.StockQuantityAmb) > Double.Epsilon)
                    materialStock.StockQuantityAmb = sumVal;
                else if (Math.Abs(sumVal) <= Double.Epsilon)
                    materialStock.StockQuantityAmb = 0;
            }
            else
            {
                materialStock.StockQuantity = 0;
                materialStock.StockQuantityAmb = 0;
            }
            return bookingResult;
        }
        #endregion

        #region MatchingFacilityStock
        protected virtual Global.ACMethodResultState DoMatchingFacilityStock(ACMethodBooking BP, FacilityBooking FB)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;

            try
            {
                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Error00045"));
                if (BP.ParamsAdjusted.InwardFacility != null)
                {
                    bookingResult = DoMatchingFacilityStock(BP, FB, BP.ParamsAdjusted.InwardFacility);
                }
                else
                {
                    var query = from c in BP.DatabaseApp.Facility where c.MDFacilityType.MDFacilityTypeIndex == (short)MDFacilityType.FacilityTypes.StorageBinContainer select c;
                    if (query != null)
                        InitProgressSubRange(BP, query.Count());
                    foreach (var facility in query)
                    {
                        Global.ACMethodResultState bookingResult2 = DoMatchingFacilityStock(BP, FB, facility);
                        if (bookingResult2 < bookingResult)
                            bookingResult = bookingResult2;

                        ReportProgressSub(BP);
                    }
                }
                ReportProgressTotal(BP);
            }
            catch (Exception e)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.TransactionError, e.Message);
                bookingResult = Global.ACMethodResultState.Failed;
            }

            return bookingResult;
        }

        protected virtual Global.ACMethodResultState DoMatchingFacilityStock(ACMethodBooking BP, FacilityBooking FB, Facility facility)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            if (facility == null || facility.MDFacilityType == null || facility.MDFacilityType.FacilityType != MDFacilityType.FacilityTypes.StorageBinContainer)
                return bookingResult;
            var query = facility.FacilityCharge_Facility.Where(c => c.NotAvailable == false);
            if (facility.CurrentFacilityStock == null && !query.Any() && !CreateStockOnFacilityMatching)
                return bookingResult;

            FacilityStock facilityStock = facility.GetFacilityStock_InsertIfNotExists(BP.DatabaseApp);

            Double sumVal = query.Sum(o => o.StockQuantityUOM);
            if (facility.MDUnit != null && facility.Material != null)
            {
                try
                {
                    sumVal = facility.Material.ConvertQuantity(sumVal, facility.Material.BaseMDUnit, facility.MDUnit);
                }
                catch (Exception e)
                {
                    BP.AddBookingMessage(ACMethodBooking.eResultCodes.TransactionError, e.Message);
                }
            }

            if (Math.Abs(sumVal - facilityStock.StockQuantity) > Double.Epsilon)
            {
                Messages.LogInfo(this.GetACUrl(), "DoMatchingFacilityStock", String.Format("FacilityStock at {0}/{1} different. Old: {2}, New: {3}, Diff: {4}.", facility.FacilityNo, facility.FacilityName, facilityStock.StockQuantity, sumVal, sumVal - facilityStock.StockQuantity));
                facilityStock.StockQuantity = sumVal;
            }
            else if (Math.Abs(sumVal) <= Double.Epsilon)
                facilityStock.StockQuantity = 0;

            sumVal = query.Sum(o => o.StockQuantityUOMAmb);
            if (facility.MDUnit != null && facility.Material != null)
            {
                try
                {
                    sumVal = facility.Material.ConvertQuantity(sumVal, facility.Material.BaseMDUnit, facility.MDUnit);
                }
                catch (Exception e)
                {
                    BP.AddBookingMessage(ACMethodBooking.eResultCodes.TransactionError, e.Message);
                }
            }

            if (Math.Abs(sumVal - facilityStock.StockQuantity) > Double.Epsilon)
                facilityStock.StockQuantityAmb = sumVal;
            else if (Math.Abs(sumVal) <= Double.Epsilon)
                facilityStock.StockQuantityAmb = 0;
            return bookingResult;
        }
        #endregion

        #region MatchingFacilityLotStock
        protected virtual Global.ACMethodResultState DoMatchingFacilityLotStock(ACMethodBooking BP, FacilityBooking FB)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;

            try
            {
                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Error00046"));
                if (BP.ParamsAdjusted.InwardFacilityLot != null)
                {
                    bookingResult = DoMatchingFacilityLotStock(BP, FB, BP.ParamsAdjusted.InwardFacilityLot);
                }
                else
                {
                    var query = from c in BP.DatabaseApp.FacilityLot select c;
                    if (query != null)
                        InitProgressSubRange(BP, query.Count());
                    foreach (var facilityLot in query)
                    {
                        Global.ACMethodResultState bookingResult2 = DoMatchingFacilityLotStock(BP, FB, facilityLot);
                        if (bookingResult2 < bookingResult)
                            bookingResult = bookingResult2;

                        ReportProgressSub(BP);
                    }
                }
                ReportProgressTotal(BP);
            }
            catch (Exception e)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.TransactionError, e.Message);
                bookingResult = Global.ACMethodResultState.Failed;
            }
            return bookingResult;
        }

        protected virtual Global.ACMethodResultState DoMatchingFacilityLotStock(ACMethodBooking BP, FacilityBooking FB, FacilityLot facilityLot)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            if (facilityLot == null)
                return bookingResult;

            var query = facilityLot.FacilityCharge_FacilityLot.Where(c => c.NotAvailable == false);
            if (facilityLot.CurrentFacilityLotStock == null && !query.Any() && !CreateStockOnFacilityLotMatching)
                return bookingResult;

            FacilityLotStock facilityLotStock = facilityLot.GetFacilityLotStock_InsertIfNotExists(BP.DatabaseApp);

            Double sumVal = query.Sum(o => o.StockQuantityUOM);
            if (Math.Abs(sumVal - facilityLotStock.StockQuantity) > Double.Epsilon)
                facilityLotStock.StockQuantity = sumVal;
            else if (Math.Abs(sumVal) <= Double.Epsilon)
                facilityLotStock.StockQuantity = 0;

            sumVal = query.Sum(o => o.StockQuantityUOMAmb);
            if (Math.Abs(sumVal - facilityLotStock.StockQuantity) > Double.Epsilon)
                facilityLotStock.StockQuantityAmb = sumVal;
            else if (Math.Abs(sumVal) <= Double.Epsilon)
                facilityLotStock.StockQuantityAmb = 0;
            return bookingResult;
        }
        #endregion

        #region MatchingPartslistStock
        protected virtual Global.ACMethodResultState DoMatchingPartslistStock(ACMethodBooking BP, FacilityBooking FB)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;

            try
            {
                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Error00048"));
                if (BP.ParamsAdjusted.InwardPartslist != null)
                {
                    bookingResult = DoMatchingPartslistStock(BP, FB, BP.ParamsAdjusted.InwardPartslist);
                }
                else
                {
                    var query = BP.DatabaseApp.Partslist.Where(c => c.DeleteDate == null);
                    if (query != null)
                        InitProgressSubRange(BP, query.Count());
                    foreach (var Partslist in query)
                    {
                        Global.ACMethodResultState bookingResult2 = DoMatchingPartslistStock(BP, FB, Partslist);
                        if (bookingResult2 < bookingResult)
                            bookingResult = bookingResult2;

                        ReportProgressSub(BP);
                    }
                }
                ReportProgressTotal(BP);
            }
            catch (Exception e)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.TransactionError, e.Message);
                bookingResult = Global.ACMethodResultState.Failed;
            }

            return bookingResult;
        }

        protected virtual Global.ACMethodResultState DoMatchingPartslistStock(ACMethodBooking BP, FacilityBooking FB, Partslist partslist)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            if (partslist == null)
                return bookingResult;

            var query = partslist.FacilityCharge_Partslist.Where(c => c.NotAvailable == false);
            if (partslist.CurrentPartslistStock == null && !query.Any() && !CreateStockOnPartslistMatching)
                return bookingResult;

            PartslistStock PartslistStock = partslist.GetPartslistStock_InsertIfNotExists(BP.DatabaseApp);

            Double sumVal = query.Sum(o => o.StockQuantityUOM);
            if (Math.Abs(sumVal - PartslistStock.StockQuantity) > Double.Epsilon)
                PartslistStock.StockQuantity = sumVal;
            else if (Math.Abs(sumVal) <= Double.Epsilon)
                PartslistStock.StockQuantity = 0;
            return bookingResult;
        }
        #endregion
        #endregion

        #region Closing
        protected virtual Global.ACMethodResultState DoClosing(ACMethodBooking BP)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            if (BP == null)
                return Global.ACMethodResultState.Notpossible;
            FacilityBooking FB = NewFacilityBooking(BP);
            InitProgressTotalRange(BP, 1);

            switch (BP.BookingType)
            {
                case GlobalApp.FacilityBookingType.ClosingDay:
                    bookingResult = DoClosingDay(BP, FB);
                    break;

                case GlobalApp.FacilityBookingType.ClosingWeek:
                    bookingResult = DoClosingWeek(BP, FB);
                    break;

                case GlobalApp.FacilityBookingType.ClosingMonth:
                    bookingResult = DoClosingMonth(BP, FB);
                    break;

                case GlobalApp.FacilityBookingType.ClosingYear:
                    bookingResult = DoClosingYear(BP, FB);
                    break;
            }

            _CurrentHistory = null;
            _CurrentHistoryBookings = null;
            return bookingResult;
        }

        #region Closing Day
        protected History _CurrentHistory = null;
        protected List<FacilityBooking> _CurrentHistoryBookings = null;
        protected virtual Global.ACMethodResultState DoClosingDay(ACMethodBooking BP, FacilityBooking FB)
        {
            bool ok = true;
            int dayClosingDays = DayClosingDays;
            dayClosingDays++;

            int periodeMax = DayClosingPeriodDayrange;
            int periode = DayClosingLastPeriodDayrange;
            periode++;
            if (periode <= 0)
                periode = 1;
            //else if (periode > periodeMax)
            //    periode = 1;

            InitProgressTotalRange(BP, 6);
            
            DateTime date = DateTime.Now;
            try
            {
                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00016", "facility history"));
                History history = DeleteAndAddHistory(BP, GlobalApp.TimePeriods.Day, periode);
                ReportProgressTotal(BP);


                FB.History = history;
                _CurrentHistoryBookings = BP.DatabaseApp.FacilityBooking.Where(c => c.History == null).ToList();

                IEnumerable<FacilityLotStock> facilityLotStockList = BP.DatabaseApp.FacilityBooking.Where(c => c.History == null)
                   .SelectMany(x => x.FacilityBookingCharge_FacilityBooking).Select(x => x.InwardFacilityLotID != null ? x.InwardFacilityLotID : x.OutwardFacilityLotID)
                   .Distinct()
                   .Join(BP.DatabaseApp.FacilityLotStock, key => key, fc => fc.FacilityLotID, (key, fc) => new { Fc = fc }).Select(x => x.Fc);

                _CurrentHistoryBookings.ForEach(c => c.History = history);


                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00015", "Materials"));
                foreach (MaterialStock materialStock in BP.DatabaseApp.MaterialStock)
                {
                    if (ClosingDayOnMaterial(BP, materialStock, history, dayClosingDays) != Global.ACMethodResultState.Succeeded)
                        ok = false;
                }
                ReportProgressTotal(BP);

                
                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00015", "CompanyMaterials"));
                foreach (CompanyMaterialStock materialStock in BP.DatabaseApp.CompanyMaterialStock)
                {
                    if (ClosingDayOnCompanyMaterial(BP, materialStock, history, dayClosingDays) != Global.ACMethodResultState.Succeeded)
                        ok = false;
                }
                ReportProgressTotal(BP);

                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00015", "Facilities"));
                foreach (FacilityStock facilityStock in BP.DatabaseApp.FacilityStock)
                {
                    if (ClosingDayOnFacility(BP, facilityStock, history, dayClosingDays) != Global.ACMethodResultState.Succeeded)
                        ok = false;
                }
                ReportProgressTotal(BP);
               
                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00015", "Lots"));
                foreach (FacilityLotStock facilityLotStock in facilityLotStockList)
                {
                    if (ClosingDayOnFacilityLot(BP, facilityLotStock, history, dayClosingDays) != Global.ACMethodResultState.Succeeded)
                        ok = false;
                }
                ReportProgressTotal(BP);


                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00015", "partslists"));
                foreach (PartslistStock partsListStock in BP.DatabaseApp.PartslistStock)
                {
                    if (ClosingDayOnPartslist(BP, partsListStock, history, dayClosingDays) != Global.ACMethodResultState.Succeeded)
                        ok = false;
                }
                ReportProgressTotal(BP);

                ReportProgressTotalText(BP, "Saving...");
                if (ok)
                {
                    OnClosingDayBeforeSave(BP, FB, history);

                    // SaveChanges wird vorher gemacht, damit der DayClosingDays Zähler abgespeichert werden kann
                    Msg result = BP.DatabaseApp.ACSaveChanges();
                    if (result == null)
                    {
                        DayClosingDays = dayClosingDays;
                        DayClosingLastDayClosing = history.BalanceDate;
                        DayClosingLastPeriodDayrange = periode;
                        BP.DatabaseApp.ACSaveChanges();
                    }
                    else
                    {
                        BP.AddBookingMessage(ACMethodBooking.eResultCodes.TransactionError, (result as MsgWithDetails).InnerMessage);
                        return Global.ACMethodResultState.Failed;
                    }
                }
                else
                {
                    return Global.ACMethodResultState.Failed;
                }
            }
            catch (Exception e)
            {
                string tmpMessage = "";
                Exception tmpException = e;
                while(tmpException != null)
                {
                    tmpMessage += tmpException.Message;
                    tmpMessage += Environment.NewLine;
                    tmpException = tmpException.InnerException;
                }
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.TransactionError, tmpMessage);
                return Global.ACMethodResultState.Failed;
            }
            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual void OnClosingDayBeforeSave(ACMethodBooking BP, FacilityBooking FB, History history)
        {
        }

        protected virtual bool CreateHistoryOnClosingDayOnMaterial(ACMethodBooking BP, MaterialStock materialStock, History history, int dayClosingDays)
        {
            return (GenerateHistoryWhenZero && materialStock.Material.IsActive)// && Math.Abs(materialStock.StockQuantity - 0) <= Double.Epsilon)
                || (Math.Abs(materialStock.DayOutward - 0) > Double.Epsilon)
                || (Math.Abs(materialStock.DayInward - 0) > Double.Epsilon)
                || (Math.Abs(materialStock.DayAdjustment - 0) > Double.Epsilon)
                || (Math.Abs(materialStock.DayTargetOutward - 0) > Double.Epsilon)
                || (Math.Abs(materialStock.DayTargetInward - 0) > Double.Epsilon);
        }

        protected virtual Global.ACMethodResultState ClosingDayOnMaterial(ACMethodBooking BP, MaterialStock materialStock, History history, int dayClosingDays)
        {
            MaterialHistory materialHistory = null;
            if (CreateHistoryOnClosingDayOnMaterial(BP, materialStock,history,dayClosingDays))
            {
                materialHistory = MaterialHistory.NewACObject(BP.DatabaseApp, history);
                materialHistory.Material = materialStock.Material;
                materialHistory.StockQuantity = materialStock.StockQuantity;
                materialHistory.Outward = materialStock.DayOutward;
                materialHistory.Inward = materialStock.DayInward;
                materialHistory.Adjustment = materialStock.DayAdjustment;
                materialHistory.TargetOutward = materialStock.DayTargetOutward;
                materialHistory.TargetInward = materialStock.DayTargetInward;
                materialHistory.LastStockQuantity = materialStock.DayLastStock;

                materialHistory.StockQuantityAmb = materialStock.StockQuantityAmb;
                materialHistory.OutwardAmb = materialStock.DayOutwardAmb;
                materialHistory.InwardAmb = materialStock.DayInwardAmb;
                materialHistory.AdjustmentAmb = materialStock.DayAdjustmentAmb;
                materialHistory.TargetOutwardAmb = materialStock.DayTargetOutwardAmb;
                materialHistory.TargetInwardAmb = materialStock.DayTargetInwardAmb;
                materialHistory.LastStockQuantityAmb = materialStock.DayLastStockAmb;

                materialHistory.OptStockQuantity = materialStock.Material.OptStockQuantity;
                materialHistory.MinStockQuantity = materialStock.Material.MinStockQuantity;
            }

            ClosingDayOnMaterialReset(BP, materialStock, history, dayClosingDays, materialHistory);
            materialStock.MonthActStock = materialStock.MonthActStock + materialStock.StockQuantity;
            materialStock.MonthAverageStock = materialStock.MonthActStock / dayClosingDays;

            materialStock.DayLastOutward = materialStock.DayOutward;
            materialStock.DayLastStock = materialStock.StockQuantity;
            materialStock.DayOutward = 0;
            materialStock.DayInward = 0;
            materialStock.DayTargetOutward = 0;
            materialStock.DayTargetInward = 0;
            materialStock.DayAdjustment = 0;

            materialStock.MonthActStockAmb = materialStock.MonthActStockAmb + materialStock.StockQuantityAmb;
            materialStock.MonthAverageStockAmb = materialStock.MonthActStockAmb / dayClosingDays;

            materialStock.DayLastOutwardAmb = materialStock.DayOutwardAmb;
            materialStock.DayLastStockAmb = materialStock.StockQuantityAmb;
            materialStock.DayOutwardAmb = 0;
            materialStock.DayInwardAmb = 0;
            materialStock.DayTargetOutwardAmb = 0;
            materialStock.DayTargetInwardAmb = 0;
            materialStock.DayAdjustmentAmb = 0;
            materialStock.DayBalanceDate = history.BalanceDate;

            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual void ClosingDayOnMaterialReset(ACMethodBooking BP, MaterialStock materialStock, History history, int dayClosingDays, MaterialHistory materialHistory)
        {
        }

        protected virtual bool CreateHistoryOnClosingDayOnCompanyMaterial(ACMethodBooking BP, CompanyMaterialStock companyMaterialStock, History history, int dayClosingDays)
        {
            return (GenerateHistoryWhenZero && companyMaterialStock.CompanyMaterial.Material != null && companyMaterialStock.CompanyMaterial.Material.IsActive)// && Math.Abs(companyMaterialStock.StockQuantity - 0) <= Double.Epsilon)
                || (Math.Abs(companyMaterialStock.DayOutward - 0) > Double.Epsilon)
                || (Math.Abs(companyMaterialStock.DayInward - 0) > Double.Epsilon)
                || (Math.Abs(companyMaterialStock.DayAdjustment - 0) > Double.Epsilon)
                || (Math.Abs(companyMaterialStock.DayTargetOutward - 0) > Double.Epsilon)
                || (Math.Abs(companyMaterialStock.DayTargetInward - 0) > Double.Epsilon);
        }

        protected virtual Global.ACMethodResultState ClosingDayOnCompanyMaterial(ACMethodBooking BP, CompanyMaterialStock companyMaterialStock, History history, int dayClosingDays)
        {
            CompanyMaterialHistory companyMaterialHistory = null;
            if (CreateHistoryOnClosingDayOnCompanyMaterial(BP, companyMaterialStock, history, dayClosingDays))
            {
                companyMaterialHistory = CompanyMaterialHistory.NewACObject(BP.DatabaseApp, history);
                companyMaterialHistory.CompanyMaterial = companyMaterialStock.CompanyMaterial;
                companyMaterialHistory.StockQuantity = companyMaterialStock.StockQuantity;
                companyMaterialHistory.LastStockQuantity = companyMaterialStock.DayLastStock;
                companyMaterialHistory.Outward = companyMaterialStock.DayOutward;
                companyMaterialHistory.Inward = companyMaterialStock.DayInward;
                companyMaterialHistory.Adjustment = companyMaterialStock.DayAdjustment;
                companyMaterialHistory.TargetOutward = companyMaterialStock.DayTargetOutward;
                companyMaterialHistory.TargetInward = companyMaterialStock.DayTargetInward;
                if (companyMaterialHistory.LentQuantity > 0)
                {
                    MaterialHistory materialHistory = history.MaterialHistory_History.Where(c => c.EntityState == System.Data.EntityState.Added && c.MaterialID == companyMaterialHistory.CompanyMaterial.MaterialID).FirstOrDefault();
                    if (materialHistory != null)
                        materialHistory.LentQuantity += companyMaterialHistory.LentQuantity;
                }

                companyMaterialHistory.StockQuantityAmb = companyMaterialStock.StockQuantityAmb;
                companyMaterialHistory.LastStockQuantityAmb = companyMaterialStock.DayLastStockAmb;
                companyMaterialHistory.OutwardAmb = companyMaterialStock.DayOutwardAmb;
                companyMaterialHistory.InwardAmb = companyMaterialStock.DayInwardAmb;
                companyMaterialHistory.AdjustmentAmb = companyMaterialStock.DayAdjustmentAmb;
                companyMaterialHistory.TargetOutwardAmb = companyMaterialStock.DayTargetOutwardAmb;
                companyMaterialHistory.TargetInwardAmb = companyMaterialStock.DayTargetInwardAmb;

                companyMaterialHistory.OptStockQuantity = companyMaterialHistory.CompanyMaterial.OptStockQuantity;
                companyMaterialHistory.MinStockQuantity = companyMaterialHistory.CompanyMaterial.MinStockQuantity;
            }

            ClosingDayOnCompanyMaterialReset(BP, companyMaterialStock, history, dayClosingDays, companyMaterialHistory);
            companyMaterialStock.MonthActStock = companyMaterialStock.MonthActStock + companyMaterialStock.StockQuantity;
            companyMaterialStock.MonthAverageStock = companyMaterialStock.MonthActStock / dayClosingDays;

            companyMaterialStock.DayLastOutward = companyMaterialStock.DayOutward;
            companyMaterialStock.DayLastStock = companyMaterialStock.StockQuantity;
            companyMaterialStock.DayOutward = 0;
            companyMaterialStock.DayInward = 0;
            companyMaterialStock.DayTargetOutward = 0;
            companyMaterialStock.DayTargetInward = 0;
            companyMaterialStock.DayAdjustment = 0;

            companyMaterialStock.MonthActStockAmb = companyMaterialStock.MonthActStockAmb + companyMaterialStock.StockQuantityAmb;
            companyMaterialStock.MonthAverageStockAmb = companyMaterialStock.MonthActStockAmb / dayClosingDays;

            companyMaterialStock.DayLastOutwardAmb = companyMaterialStock.DayOutwardAmb;
            companyMaterialStock.DayLastStockAmb = companyMaterialStock.StockQuantityAmb;
            companyMaterialStock.DayOutwardAmb = 0;
            companyMaterialStock.DayInwardAmb = 0;
            companyMaterialStock.DayTargetOutwardAmb = 0;
            companyMaterialStock.DayTargetInwardAmb = 0;
            companyMaterialStock.DayAdjustmentAmb = 0;

            companyMaterialStock.DayBalanceDate = history.BalanceDate;

            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual void ClosingDayOnCompanyMaterialReset(ACMethodBooking BP, CompanyMaterialStock companyMaterialStock, History history, int dayClosingDays, CompanyMaterialHistory companyMaterialHistory)
        {
        }

        protected virtual bool CreateHistoryOnClosingDayOnFacility(ACMethodBooking BP, FacilityStock facilityStock, History history, int dayClosingDays)
        {
            return (GenerateHistoryWhenZero && facilityStock.Facility.Material != null && facilityStock.Facility.Material.IsActive) // && Math.Abs(facilityStock.StockQuantity - 0) <= Double.Epsilon)
                || (Math.Abs(facilityStock.DayOutward - 0) > Double.Epsilon)
                || (Math.Abs(facilityStock.DayInward - 0) > Double.Epsilon)
                || (Math.Abs(facilityStock.DayAdjustment - 0) > Double.Epsilon)
                || (Math.Abs(facilityStock.DayTargetOutward - 0) > Double.Epsilon)
                || (Math.Abs(facilityStock.DayTargetInward - 0) > Double.Epsilon);
        }

        protected virtual Global.ACMethodResultState ClosingDayOnFacility(ACMethodBooking BP, FacilityStock facilityStock, History history, int dayClosingDays)
        {
            if (facilityStock.Facility.MDFacilityType.FacilityType != MDFacilityType.FacilityTypes.StorageBinContainer)
            {
                return Global.ACMethodResultState.Succeeded;
            }
            FacilityHistory facilityHistory = null;
            if (CreateHistoryOnClosingDayOnFacility(BP, facilityStock, history, dayClosingDays))
            {
                facilityHistory = FacilityHistory.NewACObject(BP.DatabaseApp, history);
                facilityHistory.Facility = facilityStock.Facility;
                facilityHistory.StockQuantity = facilityStock.StockQuantity;
                facilityHistory.Outward = facilityStock.DayOutward;
                facilityHistory.Inward = facilityStock.DayInward;
                facilityHistory.Adjustment = facilityStock.DayAdjustment;
                facilityHistory.TargetOutward = facilityStock.DayTargetOutward;
                facilityHistory.TargetInward = facilityStock.DayTargetInward;
                facilityHistory.LastStockQuantity = facilityStock.DayLastStock;

                facilityHistory.StockQuantityAmb = facilityStock.StockQuantityAmb;
                facilityHistory.OutwardAmb = facilityStock.DayOutwardAmb;
                facilityHistory.InwardAmb = facilityStock.DayInwardAmb;
                facilityHistory.AdjustmentAmb = facilityStock.DayAdjustmentAmb;
                facilityHistory.TargetOutwardAmb = facilityStock.DayTargetOutwardAmb;
                facilityHistory.TargetInwardAmb = facilityStock.DayTargetInwardAmb;
                facilityHistory.LastStockQuantityAmb = facilityStock.DayLastStockAmb;

                facilityHistory.OptStockQuantity = facilityHistory.Facility.OptStockQuantity;
                facilityHistory.MinStockQuantity = facilityHistory.Facility.MinStockQuantity;
            }
            ClosingDayOnFacilityReset(BP, facilityStock, history, dayClosingDays, facilityHistory);

            facilityStock.MonthActStock = facilityStock.MonthActStock + facilityStock.StockQuantity;
            facilityStock.MonthAverageStock = facilityStock.MonthActStock / dayClosingDays;
            facilityStock.DayLastOutward = facilityStock.DayOutward;
            facilityStock.DayLastStock = facilityStock.StockQuantity;
            facilityStock.DayOutward = 0;
            facilityStock.DayInward = 0;
            facilityStock.DayTargetOutward = 0;
            facilityStock.DayTargetInward = 0;
            facilityStock.DayAdjustment = 0;

            facilityStock.MonthActStockAmb = facilityStock.MonthActStockAmb + facilityStock.StockQuantityAmb;
            facilityStock.MonthAverageStockAmb = facilityStock.MonthActStockAmb / dayClosingDays;
            facilityStock.DayLastOutwardAmb = facilityStock.DayOutwardAmb;
            facilityStock.DayLastStockAmb = facilityStock.StockQuantityAmb;
            facilityStock.DayOutwardAmb = 0;
            facilityStock.DayInwardAmb = 0;
            facilityStock.DayTargetOutwardAmb = 0;
            facilityStock.DayTargetInwardAmb = 0;
            facilityStock.DayAdjustmentAmb = 0;
            facilityStock.DayBalanceDate = history.BalanceDate;

            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual void ClosingDayOnFacilityReset(ACMethodBooking BP, FacilityStock facilityStock, History history, int dayClosingDays, FacilityHistory facilityHistory)
        {
        }


        protected virtual Global.ACMethodResultState ClosingDayOnFacilityLot(ACMethodBooking BP, FacilityLotStock facilityLotStock, History history, int dayClosingDays)
        {
            facilityLotStock.MonthActStock = facilityLotStock.MonthActStock + facilityLotStock.StockQuantity;
            facilityLotStock.MonthAverageStock = facilityLotStock.MonthActStock / dayClosingDays;
            facilityLotStock.DayLastOutward = facilityLotStock.DayOutward;
            facilityLotStock.DayLastStock = facilityLotStock.StockQuantity;
            facilityLotStock.DayOutward = 0;
            facilityLotStock.DayInward = 0;
            facilityLotStock.DayTargetOutward = 0;
            facilityLotStock.DayTargetInward = 0;
            facilityLotStock.DayAdjustment = 0;

            facilityLotStock.MonthActStockAmb = facilityLotStock.MonthActStockAmb + facilityLotStock.StockQuantityAmb;
            facilityLotStock.MonthAverageStockAmb = facilityLotStock.MonthActStockAmb / dayClosingDays;
            facilityLotStock.DayLastOutwardAmb = facilityLotStock.DayOutwardAmb;
            facilityLotStock.DayLastStockAmb = facilityLotStock.StockQuantityAmb;
            facilityLotStock.DayOutwardAmb = 0;
            facilityLotStock.DayInwardAmb = 0;
            facilityLotStock.DayTargetOutwardAmb = 0;
            facilityLotStock.DayTargetInwardAmb = 0;
            facilityLotStock.DayAdjustmentAmb = 0;
            facilityLotStock.DayBalanceDate = history.BalanceDate;

            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual Global.ACMethodResultState ClosingDayOnPartslist(ACMethodBooking BP, PartslistStock PartslistStock, History history, int dayClosingDays)
        {
            PartslistStock.MonthActStock = PartslistStock.MonthActStock + PartslistStock.StockQuantity;
            PartslistStock.MonthAverageStock = PartslistStock.MonthActStock / dayClosingDays;
            PartslistStock.DayLastOutward = PartslistStock.DayOutward;
            PartslistStock.DayLastStock = PartslistStock.StockQuantity;
            PartslistStock.DayOutward = 0;
            PartslistStock.DayInward = 0;
            PartslistStock.DayTargetOutward = 0;
            PartslistStock.DayTargetInward = 0;
            PartslistStock.DayAdjustment = 0;
            PartslistStock.DayBalanceDate = history.BalanceDate;

            return Global.ACMethodResultState.Succeeded;
        }

        #endregion

        #region Closing Week
        protected virtual Global.ACMethodResultState DoClosingWeek(ACMethodBooking BP, FacilityBooking FB)
        {
            bool ok = true;

            int periodeMax = WeekClosingPeriodWeekrange;
            int periode = WeekClosingLastPeriodWeekrange;
            periode++;
            if (periode <= 0)
                periode = 1;
            //else if (periode > periodeMax)
            //    periode = 1;

            try
            {
                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00016", "facility history"));
                History history = DeleteAndAddHistory(BP, GlobalApp.TimePeriods.Week, periode);
                ReportProgressTotal(BP);

                FB.History = history;

                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00015", Material.ClassName));
                foreach (var materialStock in BP.DatabaseApp.MaterialStock)
                {
                    if (ClosingWeekOnMaterial(BP, materialStock, history) != Global.ACMethodResultState.Succeeded)
                        ok = false;
                }
                ReportProgressTotal(BP);

                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00015", CompanyMaterial.ClassName));
                foreach (var materialStock in BP.DatabaseApp.CompanyMaterialStock)
                {
                    if (ClosingWeekOnCompanyMaterial(BP, materialStock, history) != Global.ACMethodResultState.Succeeded)
                        ok = false;
                }
                ReportProgressTotal(BP);

                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00015", "Facilities"));
                foreach (var facilityStock in BP.DatabaseApp.FacilityStock)
                {
                    if (ClosingWeekOnFacility(BP, facilityStock, history) != Global.ACMethodResultState.Succeeded)
                        ok = false;
                }
                ReportProgressTotal(BP);

                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00015", "Lots"));
                foreach (var facilityLotStock in BP.DatabaseApp.FacilityLotStock)
                {
                    if (ClosingWeekOnFacilityLot(BP, facilityLotStock, history) != Global.ACMethodResultState.Succeeded)
                        ok = false;
                }
                ReportProgressTotal(BP);

                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00015", "PartslistStock"));
                foreach (var RartsListStock in BP.DatabaseApp.PartslistStock)
                {
                    if (ClosingWeekOnPartslist(BP, RartsListStock, history) != Global.ACMethodResultState.Succeeded)
                        ok = false;
                }
                ReportProgressTotal(BP);

                if (ok)
                {
                    OnClosingWeekBeforeSave(BP, FB, history);

                    Msg result = BP.DatabaseApp.ACSaveChanges();
                    if (result == null)
                    {
                        WeekClosingLastPeriodWeekrange = periode;
                        WeekClosingLastWeekClosing = history.BalanceDate;
                        BP.DatabaseApp.ACSaveChanges();
                    }
                    else
                    {
                        BP.AddBookingMessage(ACMethodBooking.eResultCodes.TransactionError, (result as MsgWithDetails).InnerMessage);
                        return Global.ACMethodResultState.Failed;
                    }
                }
                else
                {
                    return Global.ACMethodResultState.Failed;
                }
            }
            catch (Exception e)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.TransactionError, e.Message);
                return Global.ACMethodResultState.Failed;
            }
            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual void OnClosingWeekBeforeSave(ACMethodBooking BP, FacilityBooking FB, History history)
        {
        }

        protected virtual bool CreateHistoryOnClosingWeekOnMaterial(ACMethodBooking BP, MaterialStock materialStock, History history)
        {
            return (GenerateHistoryWhenZero && materialStock.Material.IsActive)// && Math.Abs(materialStock.StockQuantity - 0) <= Double.Epsilon)
                || (Math.Abs(materialStock.WeekOutward - 0) > Double.Epsilon)
                || (Math.Abs(materialStock.WeekInward - 0) > Double.Epsilon)
                || (Math.Abs(materialStock.WeekAdjustment - 0) > Double.Epsilon)
                || (Math.Abs(materialStock.WeekTargetOutward - 0) > Double.Epsilon)
                || (Math.Abs(materialStock.WeekTargetInward - 0) > Double.Epsilon);
        }

        protected virtual Global.ACMethodResultState ClosingWeekOnMaterial(ACMethodBooking BP, MaterialStock materialStock, History history)
        {
            MaterialHistory materialHistory = null;
            if (CreateHistoryOnClosingWeekOnMaterial(BP, materialStock,history))
            {
                materialHistory = MaterialHistory.NewACObject(BP.DatabaseApp, history);

                materialHistory.Material = materialStock.Material;
                materialHistory.StockQuantity = materialStock.StockQuantity;
                materialHistory.Outward = materialStock.WeekOutward;
                materialHistory.Inward = materialStock.WeekInward;
                materialHistory.Adjustment = materialStock.WeekAdjustment;
                materialHistory.TargetOutward = materialStock.WeekTargetOutward;
                materialHistory.TargetInward = materialStock.WeekTargetInward;

                materialHistory.StockQuantityAmb = materialStock.StockQuantityAmb;
                materialHistory.OutwardAmb = materialStock.WeekOutwardAmb;
                materialHistory.InwardAmb = materialStock.WeekInwardAmb;
                materialHistory.AdjustmentAmb = materialStock.WeekAdjustmentAmb;
                materialHistory.TargetOutwardAmb = materialStock.WeekTargetOutwardAmb;
                materialHistory.TargetInwardAmb = materialStock.WeekTargetInwardAmb;

                materialHistory.OptStockQuantity = materialStock.Material.OptStockQuantity;
                materialHistory.MinStockQuantity = materialStock.Material.MinStockQuantity;
            }
            ClosingWeekOnMaterialReset(BP, materialStock, history, materialHistory);

            materialStock.WeekOutward = 0;
            materialStock.WeekInward = 0;
            materialStock.WeekOutward = 0;
            materialStock.WeekTargetOutward = 0;
            materialStock.WeekTargetInward = 0;
            materialStock.WeekAdjustment = 0;

            materialStock.WeekOutwardAmb = 0;
            materialStock.WeekInwardAmb = 0;
            materialStock.WeekOutwardAmb = 0;
            materialStock.WeekTargetOutwardAmb = 0;
            materialStock.WeekTargetInwardAmb = 0;
            materialStock.WeekAdjustmentAmb = 0;
            materialStock.WeekBalanceDate = history.BalanceDate;

            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual void ClosingWeekOnMaterialReset(ACMethodBooking BP, MaterialStock materialStock, History history, MaterialHistory materialHistory)
        {
        }

        protected virtual bool CreateHistoryOnClosingWeekOnCompanyMaterial(ACMethodBooking BP, CompanyMaterialStock companyMaterialStock, History history)
        {
            return (GenerateHistoryWhenZero && companyMaterialStock.CompanyMaterial.Material != null && companyMaterialStock.CompanyMaterial.Material.IsActive)// && Math.Abs(companyMaterialStock.StockQuantity - 0) <= Double.Epsilon)
                || (Math.Abs(companyMaterialStock.WeekOutward - 0) > Double.Epsilon)
                || (Math.Abs(companyMaterialStock.WeekInward - 0) > Double.Epsilon)
                || (Math.Abs(companyMaterialStock.WeekAdjustment - 0) > Double.Epsilon)
                || (Math.Abs(companyMaterialStock.WeekTargetOutward - 0) > Double.Epsilon)
                || (Math.Abs(companyMaterialStock.WeekTargetInward - 0) > Double.Epsilon);
        }

        protected virtual Global.ACMethodResultState ClosingWeekOnCompanyMaterial(ACMethodBooking BP, CompanyMaterialStock companyMaterialStock, History history)
        {
            CompanyMaterialHistory companyMaterialHistory = null;
            if (CreateHistoryOnClosingWeekOnCompanyMaterial(BP, companyMaterialStock, history))
            {
                companyMaterialHistory = CompanyMaterialHistory.NewACObject(BP.DatabaseApp, history);

                companyMaterialHistory.CompanyMaterial = companyMaterialStock.CompanyMaterial;
                companyMaterialHistory.StockQuantity = companyMaterialStock.StockQuantity;
                companyMaterialHistory.Outward = companyMaterialStock.WeekOutward;
                companyMaterialHistory.Inward = companyMaterialStock.WeekInward;
                companyMaterialHistory.Adjustment = companyMaterialStock.WeekAdjustment;
                companyMaterialHistory.TargetOutward = companyMaterialStock.WeekTargetOutward;
                companyMaterialHistory.TargetInward = companyMaterialStock.WeekTargetInward;

                companyMaterialHistory.StockQuantityAmb = companyMaterialStock.StockQuantityAmb;
                companyMaterialHistory.OutwardAmb = companyMaterialStock.WeekOutwardAmb;
                companyMaterialHistory.InwardAmb = companyMaterialStock.WeekInwardAmb;
                companyMaterialHistory.AdjustmentAmb = companyMaterialStock.WeekAdjustmentAmb;
                companyMaterialHistory.TargetOutwardAmb = companyMaterialStock.WeekTargetOutwardAmb;
                companyMaterialHistory.TargetInwardAmb = companyMaterialStock.WeekTargetInwardAmb;
                
                companyMaterialHistory.OptStockQuantity = companyMaterialHistory.CompanyMaterial.OptStockQuantity;
                companyMaterialHistory.MinStockQuantity = companyMaterialHistory.CompanyMaterial.MinStockQuantity;
            }
            ClosingWeekOnCompanyMaterialReset(companyMaterialStock, history, companyMaterialHistory);

            companyMaterialStock.WeekOutward = 0;
            companyMaterialStock.WeekInward = 0;
            companyMaterialStock.WeekOutward = 0;
            companyMaterialStock.WeekTargetOutward = 0;
            companyMaterialStock.WeekTargetInward = 0;
            companyMaterialStock.WeekAdjustment = 0;

            companyMaterialStock.WeekOutwardAmb = 0;
            companyMaterialStock.WeekInwardAmb = 0;
            companyMaterialStock.WeekOutwardAmb = 0;
            companyMaterialStock.WeekTargetOutwardAmb = 0;
            companyMaterialStock.WeekTargetInwardAmb = 0;
            companyMaterialStock.WeekAdjustmentAmb = 0;

            companyMaterialStock.WeekBalanceDate = history.BalanceDate;

            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual void ClosingWeekOnCompanyMaterialReset(CompanyMaterialStock companyMaterialStock, History history, CompanyMaterialHistory companyMaterialHistory)
        {
        }

        protected virtual bool CreateHistoryOnClosingWeekOnFacility(ACMethodBooking BP, FacilityStock facilityStock, History history)
        {
            return (GenerateHistoryWhenZero && facilityStock.Facility.Material != null && facilityStock.Facility.Material.IsActive) // && Math.Abs(facilityStock.StockQuantity - 0) <= Double.Epsilon)
                || (Math.Abs(facilityStock.WeekOutward - 0) > Double.Epsilon)
                || (Math.Abs(facilityStock.WeekInward - 0) > Double.Epsilon)
                || (Math.Abs(facilityStock.WeekAdjustment - 0) > Double.Epsilon)
                || (Math.Abs(facilityStock.WeekTargetOutward - 0) > Double.Epsilon)
                || (Math.Abs(facilityStock.WeekTargetInward - 0) > Double.Epsilon);
        }

        protected virtual Global.ACMethodResultState ClosingWeekOnFacility(ACMethodBooking BP, FacilityStock facilityStock, History history)
        {
            FacilityHistory facilityHistory = null;
            if (CreateHistoryOnClosingWeekOnFacility(BP, facilityStock,history))
            {
                facilityHistory = FacilityHistory.NewACObject(BP.DatabaseApp, history);

                facilityHistory.Facility = facilityStock.Facility;
                facilityHistory.StockQuantity = facilityStock.StockQuantity;
                facilityHistory.Outward = facilityStock.WeekOutward;
                facilityHistory.Inward = facilityStock.WeekInward;
                facilityHistory.Adjustment = facilityStock.WeekAdjustment;
                facilityHistory.TargetOutward = facilityStock.WeekTargetOutward;
                facilityHistory.TargetInward = facilityStock.WeekTargetInward;

                facilityHistory.StockQuantityAmb = facilityStock.StockQuantityAmb;
                facilityHistory.OutwardAmb = facilityStock.WeekOutwardAmb;
                facilityHistory.InwardAmb = facilityStock.WeekInwardAmb;
                facilityHistory.AdjustmentAmb = facilityStock.WeekAdjustmentAmb;
                facilityHistory.TargetOutwardAmb = facilityStock.WeekTargetOutwardAmb;
                facilityHistory.TargetInwardAmb = facilityStock.WeekTargetInwardAmb;

                facilityHistory.OptStockQuantity = facilityHistory.Facility.OptStockQuantity;
                facilityHistory.MinStockQuantity = facilityHistory.Facility.MinStockQuantity;
            }
            ClosingWeekOnFacilityReset(BP, facilityStock, history, facilityHistory);

            facilityStock.WeekOutward = 0;
            facilityStock.WeekInward = 0;
            facilityStock.WeekTargetOutward = 0;
            facilityStock.WeekTargetInward = 0;
            facilityStock.WeekAdjustment = 0;

            facilityStock.WeekOutwardAmb = 0;
            facilityStock.WeekInwardAmb = 0;
            facilityStock.WeekTargetOutwardAmb = 0;
            facilityStock.WeekTargetInwardAmb = 0;
            facilityStock.WeekAdjustmentAmb = 0;

            facilityStock.WeekBalanceDate = history.BalanceDate;

            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual void ClosingWeekOnFacilityReset(ACMethodBooking BP, FacilityStock facilityStock, History history, FacilityHistory facilityHistory)
        {
        }

        protected virtual Global.ACMethodResultState ClosingWeekOnFacilityLot(ACMethodBooking BP, FacilityLotStock facilityLotStock, History history)
        {
            facilityLotStock.WeekOutward = 0;
            facilityLotStock.WeekInward = 0;
            facilityLotStock.WeekTargetOutward = 0;
            facilityLotStock.WeekTargetInward = 0;
            facilityLotStock.WeekAdjustment = 0;

            facilityLotStock.WeekOutwardAmb = 0;
            facilityLotStock.WeekInwardAmb = 0;
            facilityLotStock.WeekTargetOutwardAmb = 0;
            facilityLotStock.WeekTargetInwardAmb = 0;
            facilityLotStock.WeekAdjustmentAmb = 0;

            facilityLotStock.WeekBalanceDate = history.BalanceDate;

            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual Global.ACMethodResultState ClosingWeekOnPartslist(ACMethodBooking BP, PartslistStock PartslistStock, History history)
        {
            PartslistStock.WeekOutward = 0;
            PartslistStock.WeekInward = 0;
            PartslistStock.WeekTargetOutward = 0;
            PartslistStock.WeekTargetInward = 0;
            PartslistStock.WeekAdjustment = 0;
            PartslistStock.WeekBalanceDate = history.BalanceDate;

            return Global.ACMethodResultState.Succeeded;
        }

        #endregion

        #region Closing Month
        protected virtual Global.ACMethodResultState DoClosingMonth(ACMethodBooking BP, FacilityBooking FB)
        {
            bool ok = true;

            int periodeMax = MonthClosingPeriodMonthrange;
            int periode = MonthClosingLastPeriodMonthrange;
            periode++;
            if (periode <= 0)
                periode = 1;
            //else if (periode > periodeMax)
            //    periode = 1;

            try
            {
                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00016", "facility history"));
                History history = DeleteAndAddHistory(BP, GlobalApp.TimePeriods.Month, periode);
                ReportProgressTotal(BP);

                FB.History = history;

                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00015", "Materials"));
                foreach (var materialStock in BP.DatabaseApp.MaterialStock)
                {
                    if (ClosingMonthOnMaterial(BP, materialStock, history) != Global.ACMethodResultState.Succeeded)
                        ok = false;
                }
                ReportProgressTotal(BP);

                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00015", "CompanyMaterials"));
                foreach (var materialStock in BP.DatabaseApp.CompanyMaterialStock)
                {
                    if (ClosingMonthOnCompanyMaterial(BP, materialStock, history) != Global.ACMethodResultState.Succeeded)
                        ok = false;
                }
                ReportProgressTotal(BP);

                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00015", "Facilities"));
                foreach (var facilityStock in BP.DatabaseApp.FacilityStock)
                {
                    if (ClosingMonthOnFacility(BP, facilityStock, history) != Global.ACMethodResultState.Succeeded)
                        ok = false;
                }
                ReportProgressTotal(BP);

                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00015", "Lots"));

                //var queryFacilityLotStock = BP.DatabaseApp.FacilityLotStock.Where(c => c.MonthBalanceDate == null);
                //foreach (var facilityLotStock in queryFacilityLotStock)
                //{
                //    if (ClosingMonthOnFacilityLot(BP, facilityLotStock, history) != Global.ACMethodResultState.Succeeded)
                //        ok = false;
                //}

                SqlParameter balanceDateParam = new SqlParameter("balanceDate", history.BalanceDate);
                BP.DatabaseApp.ExecuteStoreCommand(
                    @"update facilityLotStock set
			            facilityLotStock.MonthActStock = 0,
                        facilityLotStock.MonthLastOutward = facilityLotStock.MonthOutward,
                        facilityLotStock.MonthLastStock = facilityLotStock.StockQuantity,
                        facilityLotStock.MonthOutward = 0,
                        facilityLotStock.MonthInward = 0,
                        facilityLotStock.MonthTargetOutward = 0,
                        facilityLotStock.MonthTargetInward = 0,
                        facilityLotStock.MonthAdjustment = 0,

                        facilityLotStock.MonthActStockAmb = 0,
                        facilityLotStock.MonthLastOutwardAmb = facilityLotStock.MonthOutwardAmb,
                        facilityLotStock.MonthLastStockAmb = facilityLotStock.StockQuantityAmb,
                        facilityLotStock.MonthOutwardAmb = 0,
                        facilityLotStock.MonthInwardAmb = 0,
                        facilityLotStock.MonthTargetOutwardAmb = 0,
                        facilityLotStock.MonthTargetInwardAmb = 0,
                        facilityLotStock.MonthAdjustmentAmb = 0,
                        facilityLotStock.MonthBalanceDate = @balanceDate
                    from FacilityLotStock facilityLotStock", balanceDateParam);

                ReportProgressTotal(BP);

                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00015", "partslists"));
                foreach (var RartsListStock in BP.DatabaseApp.PartslistStock)
                {
                    if (ClosingMonthOnPartslist(BP, RartsListStock, history) != Global.ACMethodResultState.Succeeded)
                        ok = false;
                }
                ReportProgressTotal(BP);

                MonthClosingLastPeriodMonthrange = periode;
                if (ok)
                {
                    OnClosingMonthBeforeSave(BP, FB, history);

                    Msg result = BP.DatabaseApp.ACSaveChanges();
                    if (result == null)
                    {
                        MonthClosingLastMonthClosing = history.BalanceDate;
                        DayClosingDays = 0;
                        BP.DatabaseApp.ACSaveChanges();
                    }
                    else
                    {
                        BP.AddBookingMessage(ACMethodBooking.eResultCodes.TransactionError, (result as MsgWithDetails).InnerMessage);
                        return Global.ACMethodResultState.Failed;
                    }
                }
                else
                {
                    return Global.ACMethodResultState.Failed;
                }
            }
            catch (Exception e)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.TransactionError, e.Message);
                return Global.ACMethodResultState.Failed;
            }
            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual void OnClosingMonthBeforeSave(ACMethodBooking BP, FacilityBooking FB, History history)
        {
        }

        protected virtual bool CreateHistoryOnClosingMonthOnMaterial(ACMethodBooking BP, MaterialStock materialStock, History history)
        {
            return (GenerateHistoryWhenZero && materialStock.Material.IsActive)// && Math.Abs(materialStock.StockQuantity - 0) <= Double.Epsilon)
                || (Math.Abs(materialStock.MonthOutward - 0) > Double.Epsilon)
                || (Math.Abs(materialStock.MonthInward - 0) > Double.Epsilon)
                || (Math.Abs(materialStock.MonthAdjustment - 0) > Double.Epsilon)
                || (Math.Abs(materialStock.MonthTargetOutward - 0) > Double.Epsilon)
                || (Math.Abs(materialStock.MonthTargetInward - 0) > Double.Epsilon);
        }

        protected virtual Global.ACMethodResultState ClosingMonthOnMaterial(ACMethodBooking BP, MaterialStock materialStock, History history)
        {
            MaterialHistory materialHistory = null;
            if (CreateHistoryOnClosingMonthOnMaterial(BP, materialStock, history))
            {
                materialHistory = MaterialHistory.NewACObject(BP.DatabaseApp, history);

                materialHistory.Material = materialStock.Material;
                materialHistory.StockQuantity = materialStock.StockQuantity;
                materialHistory.Outward = materialStock.MonthOutward;
                materialHistory.Inward = materialStock.MonthInward;
                materialHistory.Adjustment = materialStock.MonthAdjustment;
                materialHistory.TargetOutward = materialStock.MonthTargetOutward;
                materialHistory.TargetInward = materialStock.MonthTargetInward;
                materialHistory.LastStockQuantity = materialStock.MonthLastStock;

                materialHistory.StockQuantityAmb = materialStock.StockQuantityAmb;
                materialHistory.OutwardAmb = materialStock.MonthOutwardAmb;
                materialHistory.InwardAmb = materialStock.MonthInwardAmb;
                materialHistory.AdjustmentAmb = materialStock.MonthAdjustmentAmb;
                materialHistory.TargetOutwardAmb = materialStock.MonthTargetOutwardAmb;
                materialHistory.TargetInwardAmb = materialStock.MonthTargetInwardAmb;
                materialHistory.LastStockQuantityAmb = materialStock.MonthLastStockAmb;

                materialHistory.OptStockQuantity = materialStock.Material.OptStockQuantity;
                materialHistory.MinStockQuantity = materialStock.Material.MinStockQuantity;
            }

            ClosingMonthOnMaterialReset(BP, materialStock, history, materialHistory);

            // TODO: Tabellen für Planung (DemandTurnover bisher UmschlagReichw) bestücken

            materialStock.MonthActStock = 0;
            materialStock.MonthLastOutward = materialStock.MonthOutward;
            materialStock.MonthLastStock = materialStock.StockQuantity;
            materialStock.MonthOutward = 0;
            materialStock.MonthInward = 0;
            materialStock.MonthTargetOutward = 0;
            materialStock.MonthTargetInward = 0;
            materialStock.MonthAdjustment = 0;

            materialStock.MonthActStockAmb = 0;
            materialStock.MonthLastOutwardAmb = materialStock.MonthOutwardAmb;
            materialStock.MonthLastStockAmb = materialStock.StockQuantityAmb;
            materialStock.MonthOutwardAmb = 0;
            materialStock.MonthInwardAmb = 0;
            materialStock.MonthTargetOutwardAmb = 0;
            materialStock.MonthTargetInwardAmb = 0;
            materialStock.MonthAdjustmentAmb = 0;
            materialStock.MonthBalanceDate = history.BalanceDate;

            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual void ClosingMonthOnMaterialReset(ACMethodBooking BP, MaterialStock materialStock, History history, MaterialHistory materialHistory)
        {
        }

        protected virtual bool CreateHistoryOnClosingMonthOnCompanyMaterial(ACMethodBooking BP, CompanyMaterialStock companyMaterialStock, History history)
        {
            return (GenerateHistoryWhenZero && companyMaterialStock.CompanyMaterial.Material != null && companyMaterialStock.CompanyMaterial.Material.IsActive)// && Math.Abs(companyMaterialStock.StockQuantity - 0) <= Double.Epsilon)
                || (Math.Abs(companyMaterialStock.MonthOutward - 0) > Double.Epsilon)
                || (Math.Abs(companyMaterialStock.MonthInward - 0) > Double.Epsilon)
                || (Math.Abs(companyMaterialStock.MonthAdjustment - 0) > Double.Epsilon)
                || (Math.Abs(companyMaterialStock.MonthTargetOutward - 0) > Double.Epsilon)
                || (Math.Abs(companyMaterialStock.MonthTargetInward - 0) > Double.Epsilon);
        }
        
        protected virtual Global.ACMethodResultState ClosingMonthOnCompanyMaterial(ACMethodBooking BP, CompanyMaterialStock companyMaterialStock, History history)
        {
            CompanyMaterialHistory companyMaterialHistory = null;
            if (CreateHistoryOnClosingMonthOnCompanyMaterial(BP, companyMaterialStock, history))
            {
                companyMaterialHistory = CompanyMaterialHistory.NewACObject(BP.DatabaseApp, history);

                companyMaterialHistory.CompanyMaterial = companyMaterialStock.CompanyMaterial;
                companyMaterialHistory.StockQuantity = companyMaterialStock.StockQuantity;
                companyMaterialHistory.Outward = companyMaterialStock.MonthOutward;
                companyMaterialHistory.Inward = companyMaterialStock.MonthInward;
                companyMaterialHistory.Adjustment = companyMaterialStock.MonthAdjustment;
                companyMaterialHistory.TargetOutward = companyMaterialStock.MonthTargetOutward;
                companyMaterialHistory.TargetInward = companyMaterialStock.MonthTargetInward;
                companyMaterialHistory.LastStockQuantity = companyMaterialStock.MonthLastStock;

                companyMaterialHistory.StockQuantityAmb = companyMaterialStock.StockQuantityAmb;
                companyMaterialHistory.OutwardAmb = companyMaterialStock.MonthOutwardAmb;
                companyMaterialHistory.InwardAmb = companyMaterialStock.MonthInwardAmb;
                companyMaterialHistory.AdjustmentAmb = companyMaterialStock.MonthAdjustmentAmb;
                companyMaterialHistory.TargetOutwardAmb = companyMaterialStock.MonthTargetOutwardAmb;
                companyMaterialHistory.TargetInwardAmb = companyMaterialStock.MonthTargetInwardAmb;
                companyMaterialHistory.LastStockQuantityAmb = companyMaterialStock.MonthLastStockAmb;

                companyMaterialHistory.OptStockQuantity = companyMaterialHistory.CompanyMaterial.OptStockQuantity;
                companyMaterialHistory.MinStockQuantity = companyMaterialHistory.CompanyMaterial.MinStockQuantity;
            }
            ClosingMonthOnCompanyMaterialReset(BP, companyMaterialStock, history, companyMaterialHistory);

            companyMaterialStock.MonthActStock = 0;
            companyMaterialStock.MonthLastOutward = companyMaterialStock.MonthOutward;
            companyMaterialStock.MonthLastStock = companyMaterialStock.StockQuantity;
            companyMaterialStock.MonthOutward = 0;
            companyMaterialStock.MonthInward = 0;
            companyMaterialStock.MonthTargetOutward = 0;
            companyMaterialStock.MonthTargetInward = 0;
            companyMaterialStock.MonthAdjustment = 0;

            companyMaterialStock.MonthActStockAmb = 0;
            companyMaterialStock.MonthLastOutwardAmb = companyMaterialStock.MonthOutwardAmb;
            companyMaterialStock.MonthLastStockAmb = companyMaterialStock.StockQuantityAmb;
            companyMaterialStock.MonthOutwardAmb = 0;
            companyMaterialStock.MonthInwardAmb = 0;
            companyMaterialStock.MonthTargetOutwardAmb = 0;
            companyMaterialStock.MonthTargetInwardAmb = 0;
            companyMaterialStock.MonthAdjustmentAmb = 0;
            companyMaterialStock.MonthBalanceDate = history.BalanceDate;

            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual void ClosingMonthOnCompanyMaterialReset(ACMethodBooking BP, CompanyMaterialStock companyMaterialStock, History history, CompanyMaterialHistory companyMaterialHistory)
        {
        }

        protected virtual bool CreateHistoryOnClosingMonthOnFacility(ACMethodBooking BP, FacilityStock facilityStock, History history)
        {
            return (GenerateHistoryWhenZero && facilityStock.Facility.Material != null && facilityStock.Facility.Material.IsActive) // && Math.Abs(facilityStock.StockQuantity - 0) <= Double.Epsilon)
                || (Math.Abs(facilityStock.MonthOutward - 0) > Double.Epsilon)
                || (Math.Abs(facilityStock.MonthInward - 0) > Double.Epsilon)
                || (Math.Abs(facilityStock.MonthAdjustment - 0) > Double.Epsilon)
                || (Math.Abs(facilityStock.MonthTargetOutward - 0) > Double.Epsilon)
                || (Math.Abs(facilityStock.MonthTargetInward - 0) > Double.Epsilon);
        }
        
        protected virtual Global.ACMethodResultState ClosingMonthOnFacility(ACMethodBooking BP, FacilityStock facilityStock, History history)
        {
            FacilityHistory facilityHistory = null;
            if (CreateHistoryOnClosingMonthOnFacility(BP, facilityStock, history))
            {
                facilityHistory = FacilityHistory.NewACObject(BP.DatabaseApp, history);

                facilityHistory.Facility = facilityStock.Facility;
                facilityHistory.StockQuantity = facilityStock.StockQuantity;
                facilityHistory.Outward = facilityStock.MonthOutward;
                facilityHistory.Inward = facilityStock.MonthInward;
                facilityHistory.Adjustment = facilityStock.MonthAdjustment;
                facilityHistory.TargetOutward = facilityStock.MonthTargetOutward;
                facilityHistory.TargetInward = facilityStock.MonthTargetInward;
                facilityHistory.LastStockQuantity = facilityStock.MonthLastStock;

                facilityHistory.StockQuantityAmb = facilityStock.StockQuantityAmb;
                facilityHistory.OutwardAmb = facilityStock.MonthOutwardAmb;
                facilityHistory.InwardAmb = facilityStock.MonthInwardAmb;
                facilityHistory.AdjustmentAmb = facilityStock.MonthAdjustmentAmb;
                facilityHistory.TargetOutwardAmb = facilityStock.MonthTargetOutwardAmb;
                facilityHistory.TargetInwardAmb = facilityStock.MonthTargetInwardAmb;
                facilityHistory.LastStockQuantityAmb = facilityStock.MonthLastStockAmb;

                facilityHistory.OptStockQuantity = facilityHistory.Facility.OptStockQuantity;
                facilityHistory.MinStockQuantity = facilityHistory.Facility.MinStockQuantity;
            }

            ClosingMonthOnFacilityReset(BP, facilityStock, history, facilityHistory);

            facilityStock.MonthActStock = 0;
            facilityStock.MonthLastOutward = facilityStock.MonthOutward;
            facilityStock.MonthLastStock = facilityStock.StockQuantity;
            facilityStock.MonthOutward = 0;
            facilityStock.MonthInward = 0;
            facilityStock.MonthTargetOutward = 0;
            facilityStock.MonthTargetInward = 0;
            facilityStock.MonthAdjustment = 0;

            facilityStock.MonthActStockAmb = 0;
            facilityStock.MonthLastOutwardAmb = facilityStock.MonthOutwardAmb;
            facilityStock.MonthLastStockAmb = facilityStock.StockQuantityAmb;
            facilityStock.MonthOutwardAmb = 0;
            facilityStock.MonthInwardAmb = 0;
            facilityStock.MonthTargetOutwardAmb = 0;
            facilityStock.MonthTargetInwardAmb = 0;
            facilityStock.MonthAdjustmentAmb = 0;
            facilityStock.MonthBalanceDate = history.BalanceDate;

            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual void ClosingMonthOnFacilityReset(ACMethodBooking BP, FacilityStock facilityStock, History history, FacilityHistory facilityHistory)
        {
        }

        protected virtual Global.ACMethodResultState ClosingMonthOnFacilityLot(ACMethodBooking BP, FacilityLotStock facilityLotStock, History history)
        {
            facilityLotStock.MonthActStock = 0;
            facilityLotStock.MonthLastOutward = facilityLotStock.MonthOutward;
            facilityLotStock.MonthLastStock = facilityLotStock.StockQuantity;
            facilityLotStock.MonthOutward = 0;
            facilityLotStock.MonthInward = 0;
            facilityLotStock.MonthTargetOutward = 0;
            facilityLotStock.MonthTargetInward = 0;
            facilityLotStock.MonthAdjustment = 0;

            facilityLotStock.MonthActStockAmb = 0;
            facilityLotStock.MonthLastOutwardAmb = facilityLotStock.MonthOutwardAmb;
            facilityLotStock.MonthLastStockAmb = facilityLotStock.StockQuantityAmb;
            facilityLotStock.MonthOutwardAmb = 0;
            facilityLotStock.MonthInwardAmb = 0;
            facilityLotStock.MonthTargetOutwardAmb = 0;
            facilityLotStock.MonthTargetInwardAmb = 0;
            facilityLotStock.MonthAdjustmentAmb = 0;
            facilityLotStock.MonthBalanceDate = history.BalanceDate;

            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual Global.ACMethodResultState ClosingMonthOnPartslist(ACMethodBooking BP, PartslistStock PartslistStock, History history)
        {
            PartslistStock.MonthActStock = 0;
            PartslistStock.MonthLastOutward = PartslistStock.MonthOutward;
            PartslistStock.MonthLastStock = PartslistStock.MonthLastStock;
            PartslistStock.MonthOutward = 0;
            PartslistStock.MonthInward = 0;
            PartslistStock.MonthTargetOutward = 0;
            PartslistStock.MonthTargetInward = 0;
            PartslistStock.MonthAdjustment = 0;

            PartslistStock.MonthBalanceDate = history.BalanceDate;

            return Global.ACMethodResultState.Succeeded;
        }

        #endregion

        #region Closing Year
        protected virtual Global.ACMethodResultState DoClosingYear(ACMethodBooking BP, FacilityBooking FB)
        {
            bool ok = true;

            int periodeMax = YearClosingPeriodYearrange;
            int periode = YearClosingLastPeriodYearrange;
            periode++;
            if (periode <= 0)
                periode = 1;
            //else if (periode > periodeMax)
            //    periode = 1;

            try
            {
                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00016", "facility history"));
                History history = DeleteAndAddHistory(BP, GlobalApp.TimePeriods.Year, periode);
                ReportProgressTotal(BP);

                FB.History = history;

                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00015", "Materials"));
                foreach (var materialStock in BP.DatabaseApp.MaterialStock)
                {
                    if (ClosingYearOnMaterial(BP, materialStock, history) != Global.ACMethodResultState.Succeeded)
                        ok = false;
                }
                ReportProgressTotal(BP);

                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00015", "CompanyMaterials"));
                foreach (var materialStock in BP.DatabaseApp.CompanyMaterialStock)
                {
                    if (ClosingYearOnCompanyMaterial(BP, materialStock, history) != Global.ACMethodResultState.Succeeded)
                        ok = false;
                }
                ReportProgressTotal(BP);

                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00015", "Facilities"));
                foreach (var facilityStock in BP.DatabaseApp.FacilityStock)
                {
                    if (ClosingYearOnFacility(BP, facilityStock, history) != Global.ACMethodResultState.Succeeded)
                        ok = false;
                }
                ReportProgressTotal(BP);

                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00015", "Lots"));
                foreach (var facilityLotStock in BP.DatabaseApp.FacilityLotStock)
                {
                    if (ClosingYearOnFacilityLot(BP, facilityLotStock, history) != Global.ACMethodResultState.Succeeded)
                        ok = false;
                }
                ReportProgressTotal(BP);

                ReportProgressTotalText(BP, Root.Environment.TranslateMessage(this, "Info00015", "partslists"));
                foreach (var RartsListStock in BP.DatabaseApp.PartslistStock)
                {
                    if (ClosingYearOnPartslist(BP, RartsListStock, history) != Global.ACMethodResultState.Succeeded)
                        ok = false;
                }
                ReportProgressTotal(BP);

                if (ok)
                {
                    OnClosingYearBeforeSave(BP, FB, history);

                    Msg result = BP.DatabaseApp.ACSaveChanges();
                    if (result == null)
                    {
                        YearClosingLastPeriodYearrange = periode;
                        YearClosingLastYearClosing = history.BalanceDate;
                        BP.DatabaseApp.ACSaveChanges();
                    }
                    else
                    {
                        BP.AddBookingMessage(ACMethodBooking.eResultCodes.TransactionError, (result as MsgWithDetails).InnerMessage);
                        return Global.ACMethodResultState.Failed;
                    }
                }
                else
                {
                    return Global.ACMethodResultState.Failed;
                }
            }
            catch (Exception e)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.TransactionError, e.Message);
                return Global.ACMethodResultState.Failed;
            }
            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual void OnClosingYearBeforeSave(ACMethodBooking BP, FacilityBooking FB, History history)
        {
        }

        protected virtual bool CreateHistoryOnClosingYearOnMaterial(ACMethodBooking BP, MaterialStock materialStock, History history)
        {
            return (GenerateHistoryWhenZero && materialStock.Material.IsActive)// && Math.Abs(materialStock.StockQuantity - 0) <= Double.Epsilon)
                || (Math.Abs(materialStock.YearOutward - 0) > Double.Epsilon)
                || (Math.Abs(materialStock.YearInward - 0) > Double.Epsilon)
                || (Math.Abs(materialStock.YearAdjustment - 0) > Double.Epsilon)
                || (Math.Abs(materialStock.YearTargetOutward - 0) > Double.Epsilon)
                || (Math.Abs(materialStock.YearTargetInward - 0) > Double.Epsilon);
        }
        
        protected virtual Global.ACMethodResultState ClosingYearOnMaterial(ACMethodBooking BP, MaterialStock materialStock, History history)
        {
            MaterialHistory materialHistory = null;
            if (CreateHistoryOnClosingYearOnMaterial(BP, materialStock, history))
            {
                materialHistory = MaterialHistory.NewACObject(BP.DatabaseApp, history);

                materialHistory.Material = materialStock.Material;
                materialHistory.StockQuantity = materialStock.StockQuantity;
                materialHistory.Outward = materialStock.YearOutward;
                materialHistory.Inward = materialStock.YearInward;
                materialHistory.Adjustment = materialStock.YearAdjustment;
                materialHistory.TargetOutward = materialStock.YearTargetOutward;
                materialHistory.TargetInward = materialStock.YearTargetInward;

                materialHistory.StockQuantityAmb = materialStock.StockQuantityAmb;
                materialHistory.OutwardAmb = materialStock.YearOutwardAmb;
                materialHistory.InwardAmb = materialStock.YearInwardAmb;
                materialHistory.AdjustmentAmb = materialStock.YearAdjustmentAmb;
                materialHistory.TargetOutwardAmb = materialStock.YearTargetOutwardAmb;
                materialHistory.TargetInwardAmb = materialStock.YearTargetInwardAmb;

                materialHistory.OptStockQuantity = materialStock.Material.OptStockQuantity;
                materialHistory.MinStockQuantity = materialStock.Material.MinStockQuantity;
            }
            ClosingYearOnMaterialReset(BP, materialStock, history, materialHistory);

            materialStock.YearOutward = 0;
            materialStock.YearInward = 0;
            materialStock.YearTargetOutward = 0;
            materialStock.YearTargetInward = 0;
            materialStock.YearAdjustment = 0;

            materialStock.YearOutwardAmb = 0;
            materialStock.YearInwardAmb = 0;
            materialStock.YearTargetOutwardAmb = 0;
            materialStock.YearTargetInwardAmb = 0;
            materialStock.YearAdjustmentAmb = 0;
            materialStock.YearBalanceDate = history.BalanceDate;

            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual void ClosingYearOnMaterialReset(ACMethodBooking BP, MaterialStock materialStock, History history, MaterialHistory materialHistory)
        {
        }

        protected virtual bool CreateHistoryOnClosingYearOnCompanyMaterial(ACMethodBooking BP, CompanyMaterialStock companyMaterialStock, History history)
        {
            return (GenerateHistoryWhenZero && companyMaterialStock.CompanyMaterial.Material != null && companyMaterialStock.CompanyMaterial.Material.IsActive)// && Math.Abs(companyMaterialStock.StockQuantity - 0) <= Double.Epsilon)
                || (Math.Abs(companyMaterialStock.YearOutward - 0) > Double.Epsilon)
                || (Math.Abs(companyMaterialStock.YearInward - 0) > Double.Epsilon)
                || (Math.Abs(companyMaterialStock.YearAdjustment - 0) > Double.Epsilon)
                || (Math.Abs(companyMaterialStock.YearTargetOutward - 0) > Double.Epsilon)
                || (Math.Abs(companyMaterialStock.YearTargetInward - 0) > Double.Epsilon);
        }
        
        protected virtual Global.ACMethodResultState ClosingYearOnCompanyMaterial(ACMethodBooking BP, CompanyMaterialStock companyMaterialStock, History history)
        {
            CompanyMaterialHistory companyMaterialHistory = null;
            if (CreateHistoryOnClosingYearOnCompanyMaterial(BP, companyMaterialStock, history))
            {
                companyMaterialHistory = CompanyMaterialHistory.NewACObject(BP.DatabaseApp, history);

                companyMaterialHistory.CompanyMaterial = companyMaterialStock.CompanyMaterial;
                companyMaterialHistory.StockQuantity = companyMaterialStock.StockQuantity;
                companyMaterialHistory.Outward = companyMaterialStock.YearOutward;
                companyMaterialHistory.Inward = companyMaterialStock.YearInward;
                companyMaterialHistory.Adjustment = companyMaterialStock.YearAdjustment;
                companyMaterialHistory.TargetOutward = companyMaterialStock.YearTargetOutward;
                companyMaterialHistory.TargetInward = companyMaterialStock.YearTargetInward;

                companyMaterialHistory.StockQuantityAmb = companyMaterialStock.StockQuantityAmb;
                companyMaterialHistory.OutwardAmb = companyMaterialStock.YearOutwardAmb;
                companyMaterialHistory.InwardAmb = companyMaterialStock.YearInwardAmb;
                companyMaterialHistory.AdjustmentAmb = companyMaterialStock.YearAdjustmentAmb;
                companyMaterialHistory.TargetOutwardAmb = companyMaterialStock.YearTargetOutwardAmb;
                companyMaterialHistory.TargetInwardAmb = companyMaterialStock.YearTargetInwardAmb;

                companyMaterialHistory.OptStockQuantity = companyMaterialHistory.CompanyMaterial.OptStockQuantity;
                companyMaterialHistory.MinStockQuantity = companyMaterialHistory.CompanyMaterial.MinStockQuantity;
            }
            ClosingYearOnCompanyMaterialReset(BP, companyMaterialStock, history, companyMaterialHistory);

            companyMaterialStock.YearOutward = 0;
            companyMaterialStock.YearInward = 0;
            companyMaterialStock.YearTargetOutward = 0;
            companyMaterialStock.YearTargetInward = 0;
            companyMaterialStock.YearAdjustment = 0;

            companyMaterialStock.YearOutwardAmb = 0;
            companyMaterialStock.YearInwardAmb = 0;
            companyMaterialStock.YearTargetOutwardAmb = 0;
            companyMaterialStock.YearTargetInwardAmb = 0;
            companyMaterialStock.YearAdjustmentAmb = 0;

            companyMaterialStock.YearBalanceDate = history.BalanceDate;

            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual void ClosingYearOnCompanyMaterialReset(ACMethodBooking BP, CompanyMaterialStock companyMaterialStock, History history, CompanyMaterialHistory companyMaterialHistory)
        {
        }

        protected virtual bool CreateHistoryOnClosingYearOnFacility(ACMethodBooking BP, FacilityStock facilityStock, History history)
        {
            return (GenerateHistoryWhenZero && facilityStock.Facility.Material != null && facilityStock.Facility.Material.IsActive) // && Math.Abs(facilityStock.StockQuantity - 0) <= Double.Epsilon)
                || (Math.Abs(facilityStock.YearOutward - 0) > Double.Epsilon)
                || (Math.Abs(facilityStock.YearInward - 0) > Double.Epsilon)
                || (Math.Abs(facilityStock.YearAdjustment - 0) > Double.Epsilon)
                || (Math.Abs(facilityStock.YearTargetOutward - 0) > Double.Epsilon)
                || (Math.Abs(facilityStock.YearTargetInward - 0) > Double.Epsilon);
        }
        
        protected virtual Global.ACMethodResultState ClosingYearOnFacility(ACMethodBooking BP, FacilityStock facilityStock, History history)
        {
            FacilityHistory facilityHistory = null;
            if (CreateHistoryOnClosingYearOnFacility(BP, facilityStock, history))
            {
                facilityHistory = FacilityHistory.NewACObject(BP.DatabaseApp, history);
                facilityHistory.Facility = facilityStock.Facility;
                facilityHistory.StockQuantity = facilityStock.StockQuantity;
                facilityHistory.Outward = facilityStock.YearOutward;
                facilityHistory.Inward = facilityStock.YearInward;
                facilityHistory.Adjustment = facilityStock.YearAdjustment;
                facilityHistory.TargetOutward = facilityStock.YearTargetOutward;
                facilityHistory.TargetInward = facilityStock.YearTargetInward;

                facilityHistory.StockQuantityAmb = facilityStock.StockQuantityAmb;
                facilityHistory.OutwardAmb = facilityStock.YearOutwardAmb;
                facilityHistory.InwardAmb = facilityStock.YearInwardAmb;
                facilityHistory.AdjustmentAmb = facilityStock.YearAdjustmentAmb;
                facilityHistory.TargetOutwardAmb = facilityStock.YearTargetOutwardAmb;
                facilityHistory.TargetInwardAmb = facilityStock.YearTargetInwardAmb;

                facilityHistory.OptStockQuantity = facilityHistory.Facility.OptStockQuantity;
                facilityHistory.MinStockQuantity = facilityHistory.Facility.MinStockQuantity;
            }

            ClosingYearhOnFacilityReset(BP, facilityStock, history, facilityHistory);

            facilityStock.YearOutward = 0;
            facilityStock.YearInward = 0;
            facilityStock.YearTargetOutward = 0;
            facilityStock.YearTargetInward = 0;
            facilityStock.YearAdjustment = 0;

            facilityStock.YearOutwardAmb = 0;
            facilityStock.YearInwardAmb = 0;
            facilityStock.YearTargetOutwardAmb = 0;
            facilityStock.YearTargetInwardAmb = 0;
            facilityStock.YearAdjustmentAmb = 0;

            facilityStock.YearBalanceDate = history.BalanceDate;

            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual void ClosingYearhOnFacilityReset(ACMethodBooking BP, FacilityStock facilityStock, History history, FacilityHistory facilityHistory)
        {
        }

        protected virtual Global.ACMethodResultState ClosingYearOnFacilityLot(ACMethodBooking BP, FacilityLotStock facilityLotStock, History history)
        {
            facilityLotStock.YearOutward = 0;
            facilityLotStock.YearInward = 0;
            facilityLotStock.YearTargetOutward = 0;
            facilityLotStock.YearTargetInward = 0;
            facilityLotStock.YearAdjustment = 0;
            facilityLotStock.YearOutwardAmb = 0;
            facilityLotStock.YearInwardAmb = 0;
            facilityLotStock.YearTargetOutwardAmb = 0;
            facilityLotStock.YearTargetInwardAmb = 0;
            facilityLotStock.YearAdjustmentAmb = 0;
            facilityLotStock.YearBalanceDate = history.BalanceDate;

            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual Global.ACMethodResultState ClosingYearOnPartslist(ACMethodBooking BP, PartslistStock PartslistStock, History history)
        {
            PartslistStock.YearOutward = 0;
            PartslistStock.YearInward = 0;
            PartslistStock.YearTargetOutward = 0;
            PartslistStock.YearTargetInward = 0;
            PartslistStock.YearAdjustment = 0;
            PartslistStock.YearBalanceDate = history.BalanceDate;

            return Global.ACMethodResultState.Succeeded;
        }

        #endregion

        #region Closing Helpermethods
        protected virtual History DeleteAndAddHistory(ACMethodBooking BP, GlobalApp.TimePeriods timePeriod, int periode)
        {
            /*var query = from c in DatabaseApp.FacilityHistory
                        where c.MDTimePeriod.MDTimePeriodIndex == (int)timePeriod && c.PeriodNo == periode
                        select c;
            
            foreach (var facilityHistory in query)
            {
                facilityHistory.DeleteACObject(DatabaseApp, false);
            }*/

            foreach (var history in BP.DatabaseApp.History.Where(w => w.TimePeriodIndex == (int)timePeriod && w.PeriodNo == periode).ToList())
            {
                history.DeleteACObject(BP.DatabaseApp, false);
            }

            _CurrentHistory = History.NewACObject(BP.DatabaseApp, null);
            _CurrentHistory.BalanceDate = DateTime.Now;
            _CurrentHistory.TimePeriod = timePeriod;
            _CurrentHistory.PeriodNo = periode;
            BP.DatabaseApp.History.AddObject(_CurrentHistory);

            return _CurrentHistory;
        }
        #endregion
        #endregion

        #region Inventory
        protected virtual Global.ACMethodResultState DoInventory(ACMethodBooking BP)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            if (BP == null)
                return Global.ACMethodResultState.Notpossible;
            FacilityBooking FB = NewFacilityBooking(BP);
            InitProgressTotalRange(BP, 1);

            /*switch (BP.BookingType.BookingTypeIndex)
            {
                case (short)Global.FacitlityBookingType.InventoryNew:
                    bookingResult = DoClosingDay(BP, FB);
                    break;

                case (short)Global.FacitlityBookingType.InventoryStockCorrection:
                    bookingResult = DoClosingWeek(BP, FB);
                    break;

                case (short)Global.FacitlityBookingType.InventoryClose:
                    bookingResult = DoClosingMonth(BP, FB);
                    break;
            }*/

            return bookingResult;
        }

        #endregion

        #region Progress
        #region Progress Sub
        private void InitProgressSubRange(ACMethodBooking BP, int range)
        {
            // TODO: @aagincic rewrite InitProgressSubRange

            //if (BP.VBProgress == null)
            //    return;
            //BP.VBProgress.ProgressInfo.SubProgressRangeFrom = 0;
            //BP.VBProgress.ProgressInfo.SubProgressRangeTo = range;
        }

        private void ReportProgressSubText(ACMethodBooking BP, string message)
        {
            // TODO: @aagincic rewrite ReportProgressSubText

            //if (BP.VBProgress == null)
            //    return;
            //BP.VBProgress.ProgressInfo.SubProgressText = message;
        }

        private void ReportProgressSub(ACMethodBooking BP)
        {
            // TODO: @aagincic rewrite ReportProgressSub
            //if (BP.VBProgress == null)
            //    return;
            //// Falls Abbruch von Benutzer
            //if (BP.VBProgress.CancellationPending == true)
            //{
            //    BP.VBProgress.EventArgs.Cancel = true;
            //    throw new Exception(Root.Environment.TranslateMessage(this, "Error00043"));
            //}
            //BP.VBProgress.ProgressInfo.SubProgressCurrent++;
            //BP.VBProgress.ReportProgress();
            //Thread.Sleep(100);
        }
        #endregion

        #region Progress Total
        private void InitProgressTotalRange(ACMethodBooking BP, int range)
        {
            // TODO: @aagincic rewrite InitProgressTotalRange

            //if (BP.VBProgress == null)
            //    return;
            //BP.VBProgress.ProgressInfo.TotalProgressRangeFrom = 0;
            //BP.VBProgress.ProgressInfo.TotalProgressRangeTo = range;
        }

        private void ReportProgressTotalText(ACMethodBooking BP, string message)
        {
            // TODO: @aagincic rewrite ReportProgressTotalText

            //if (BP.VBProgress == null)
            //    return;
            //BP.VBProgress.ProgressInfo.TotalProgressText = message;
        }

        private void ReportProgressTotal(ACMethodBooking BP)
        {
            // TODO: @aagincic rewrite ReportProgressTotal

            //if (BP.VBProgress == null)
            //    return;
            //// Falls Abbruch von Benutzer
            //if (BP.VBProgress.CancellationPending == true)
            //{
            //    BP.VBProgress.EventArgs.Cancel = true;
            //    throw new Exception(Root.Environment.TranslateMessage(this, "Error00043"));
            //}
            //BP.VBProgress.ProgressInfo.TotalProgressCurrent++;
            //BP.VBProgress.ProgressInfo.SubProgressCurrent = BP.VBProgress.ProgressInfo.SubProgressRangeFrom;
            //BP.VBProgress.ReportProgress();
        }
        #endregion
        #endregion

    }
}
