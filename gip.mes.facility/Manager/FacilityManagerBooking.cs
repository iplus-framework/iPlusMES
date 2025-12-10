// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.facility
{
    public partial class FacilityManager
    {
        /// <summary>
        /// Buchungsmethoden auf elementare Entitäten: Facility, FacilityCharge, Material, FacilityLot
        /// </summary>
        #region Bookingmethods on Basic Entities

        #region Book Entities over FacilityBookingCharge
        /// <summary>
        /// Abgangsbuchung auf  FacilityBookingCharge
        /// </summary>
        private Global.ACMethodResultState BookFacilityBookingChargeOut(ACMethodBooking BP, FacilityBookingCharge FBC, List<FacilityCharge> facilityCharges)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            bookingResult = BookFacilityChargeOut(BP, FBC, facilityCharges);
            if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                return bookingResult;

            bookingResult = BookMaterialOut(BP, FBC, facilityCharges);
            if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                return bookingResult;

            bookingResult = BookFacilityOut(BP, FBC, facilityCharges);
            if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                return bookingResult;

            bookingResult = BookFacilityLotOut(BP, FBC, facilityCharges);
            if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                return bookingResult;

            bookingResult = BookCompanyMaterialOut(BP, FBC, facilityCharges);
            if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                return bookingResult;

            bookingResult = BookCPartnerCompMatOut(BP, FBC, facilityCharges);
            if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                return bookingResult;

            bookingResult = PerformAnterogradePosting(BP, FBC, facilityCharges);
            if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                return bookingResult;

            return bookingResult;
        }

        /// <summary>
        /// Zugangsbuchung auf  FacilityBookingCharge
        /// </summary>
        private Global.ACMethodResultState BookFacilityBookingChargeIn(ACMethodBooking BP, FacilityBookingCharge FBC, List<FacilityCharge> facilityCharges)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            bookingResult = BookFacilityChargeIn(BP, FBC, facilityCharges);
            if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                return bookingResult;

            bookingResult = BookMaterialIn(BP, FBC, facilityCharges);
            if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                return bookingResult;

            bookingResult = BookFacilityIn(BP, FBC, facilityCharges);
            if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                return bookingResult;

            bookingResult = BookFacilityLotIn(BP, FBC, facilityCharges);
            if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                return bookingResult;

            bookingResult = BookCompanyMaterialIn(BP, FBC, facilityCharges);
            if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                return bookingResult;

            bookingResult = BookCPartnerCompMatIn(BP, FBC, facilityCharges);
            if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                return bookingResult;

            bookingResult = PerformRetrogradePosting(BP, FBC, facilityCharges);
            if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                return bookingResult;

            return bookingResult;
        }
        #endregion

        public enum BookingEventOnField
        {
            QuantityIn,
            QuantityOut,
            Inward,
            TargetInward,
            Outward,
            TargetOutward
        }

        public enum BookingEventOnEntity
        {
            Material,
            CompanyMaterial,
            CPartner,
            Facility,
            FacilityLot,
            Partslist,
            FacilityCharge
        }

        protected virtual void OnBookFacilityBookingCharge(ACMethodBooking BP, FacilityBookingCharge FBC, BookingEventOnEntity entity, BookingEventOnField field, List<FacilityCharge> facilityCharges = null)
        {
        }

        #region Book Material
        /// <summary>
        /// Einbuchung To-Felder in Tabelle Material 
        /// </summary>
        Global.ACMethodResultState BookMaterialIn(ACMethodBooking BP, FacilityBookingCharge FBC, List<FacilityCharge> facilityCharges)
        {
            //AddBookingMessage("BookMaterialIn");
            if (FBC.FacilityBooking == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", FacilityBooking.ClassName, "FacilityBookingCharge"));
                return Global.ACMethodResultState.Notpossible;
            }
            if (FBC.InwardMaterialStock == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", "InwardMaterialStock", FacilityBooking.ClassName));
                return Global.ACMethodResultState.Notpossible;
            }
            else if (BP.AutoRefresh)
                FBC.InwardMaterialStock.AutoRefresh();

            //if (Math.Abs(FBC.InwardQuantityUOM - 0) > Double.Epsilon)
            //    FBC.InwardMaterialStock.StockWeight += FBC.InwardWeightInMU;
            if (Math.Abs(FBC.InwardQuantityUOM - 0) > Double.Epsilon)
            {
                FBC.InwardMaterialStock.StockQuantity += FBC.InwardQuantityUOM;
                FBC.InwardMaterialStock.StockQuantityAmb += FBC.InwardQuantityUOMAmb;
                if (FBC.IsAdjustmentBooking)
                {
                    FBC.InwardMaterialStock.DayAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardMaterialStock.DayAdjustmentAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardMaterialStock.WeekAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardMaterialStock.WeekAdjustmentAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardMaterialStock.MonthAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardMaterialStock.MonthAdjustmentAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardMaterialStock.YearAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardMaterialStock.YearAdjustmentAmb += FBC.InwardQuantityUOMAmb;
                }
                OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.Material, BookingEventOnField.QuantityIn, facilityCharges);

                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOff)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.InwardMaterialStock.DayInward += FBC.InwardQuantityUOM;
                    FBC.InwardMaterialStock.MonthInward += FBC.InwardQuantityUOM;
                    FBC.InwardMaterialStock.WeekInward += FBC.InwardQuantityUOM;
                    FBC.InwardMaterialStock.YearInward += FBC.InwardQuantityUOM;
                    FBC.InwardMaterialStock.DayInwardAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardMaterialStock.MonthInwardAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardMaterialStock.WeekInwardAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardMaterialStock.YearInwardAmb += FBC.InwardQuantityUOMAmb;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.Material, BookingEventOnField.Inward, facilityCharges);
                }
            }
            if (Math.Abs(FBC.InwardTargetQuantityUOM - 0) > Double.Epsilon)
            {
                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOff)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.InwardMaterialStock.DayTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardMaterialStock.MonthTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardMaterialStock.WeekTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardMaterialStock.YearTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardMaterialStock.DayTargetInwardAmb += FBC.InwardTargetQuantityUOMAmb;
                    FBC.InwardMaterialStock.MonthTargetInwardAmb += FBC.InwardTargetQuantityUOMAmb;
                    FBC.InwardMaterialStock.WeekTargetInwardAmb += FBC.InwardTargetQuantityUOMAmb;
                    FBC.InwardMaterialStock.YearTargetInwardAmb += FBC.InwardTargetQuantityUOMAmb;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.Material, BookingEventOnField.TargetInward, facilityCharges);
                }
            }

            if (BP.ParamsAdjusted.InwardMaterial != null)
                ReleaseStateMaterial(FBC.InwardMaterialStock, BP.ParamsAdjusted.MDReleaseState);

            return Global.ACMethodResultState.Succeeded;
        }

        /// <summary>
        /// Ausbuchung From-Felder in Tabelle Material 
        /// </summary>
        Global.ACMethodResultState BookMaterialOut(ACMethodBooking BP, FacilityBookingCharge FBC, List<FacilityCharge> facilityCharges)
        {
            //AddBookingMessage("BookMaterialOut");
            if (FBC.FacilityBooking == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", FacilityBooking.ClassName, "FacilityBookingCharge"));
                return Global.ACMethodResultState.Notpossible;
            }
            if (FBC.OutwardMaterialStock == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", "OutwardMaterialStock", FacilityBooking.ClassName));
                return Global.ACMethodResultState.Notpossible;
            }
            else if (BP.AutoRefresh)
                FBC.OutwardMaterialStock.AutoRefresh();

            //if (Math.Abs(FBC.OutwardWeightInMU - 0) > Double.Epsilon)
            //    FBC.OutwardMaterialStock.StockWeight -= FBC.OutwardWeightInMU;
            if (Math.Abs(FBC.OutwardQuantityUOM - 0) > Double.Epsilon)
            {
                FBC.OutwardMaterialStock.StockQuantity -= FBC.OutwardQuantityUOM;
                FBC.OutwardMaterialStock.StockQuantityAmb -= FBC.OutwardQuantityUOMAmb;
                if (FBC.IsAdjustmentBooking)
                {
                    FBC.OutwardMaterialStock.DayAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardMaterialStock.DayAdjustmentAmb -= FBC.InwardQuantityUOMAmb;
                    FBC.OutwardMaterialStock.WeekAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardMaterialStock.WeekAdjustmentAmb -= FBC.InwardQuantityUOMAmb;
                    FBC.OutwardMaterialStock.MonthAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardMaterialStock.MonthAdjustmentAmb -= FBC.InwardQuantityUOMAmb;
                    FBC.OutwardMaterialStock.YearAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardMaterialStock.YearAdjustmentAmb -= FBC.InwardQuantityUOMAmb;
                }

                OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.Material, BookingEventOnField.QuantityOut, facilityCharges);

                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOff_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.OutwardMaterialStock.DayOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardMaterialStock.MonthOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardMaterialStock.WeekOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardMaterialStock.YearOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardMaterialStock.DayOutwardAmb += FBC.OutwardQuantityUOMAmb;
                    FBC.OutwardMaterialStock.MonthOutwardAmb += FBC.OutwardQuantityUOMAmb;
                    FBC.OutwardMaterialStock.WeekOutwardAmb += FBC.OutwardQuantityUOMAmb;
                    FBC.OutwardMaterialStock.YearOutwardAmb += FBC.OutwardQuantityUOMAmb;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.Material, BookingEventOnField.Outward, facilityCharges);
                }
            }
            if (Math.Abs(FBC.OutwardTargetQuantityUOM - 0) > Double.Epsilon)
            {
                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOff_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.OutwardMaterialStock.DayTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardMaterialStock.MonthTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardMaterialStock.WeekTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardMaterialStock.YearTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardMaterialStock.DayTargetOutwardAmb += FBC.OutwardTargetQuantityUOMAmb;
                    FBC.OutwardMaterialStock.MonthTargetOutwardAmb += FBC.OutwardTargetQuantityUOMAmb;
                    FBC.OutwardMaterialStock.WeekTargetOutwardAmb += FBC.OutwardTargetQuantityUOMAmb;
                    FBC.OutwardMaterialStock.YearTargetOutwardAmb += FBC.OutwardTargetQuantityUOMAmb;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.Material, BookingEventOnField.TargetOutward, facilityCharges);
                }
            }
            
            if (BP.ParamsAdjusted.OutwardMaterial != null)
                ReleaseStateMaterial(FBC.InwardMaterialStock, BP.ParamsAdjusted.MDReleaseState);

            return Global.ACMethodResultState.Succeeded;
        }
        #endregion


        #region Book CompanyMaterial
        /// <summary>
        /// Einbuchung To-Felder in Tabelle Material 
        /// </summary>
        Global.ACMethodResultState BookCompanyMaterialIn(ACMethodBooking BP, FacilityBookingCharge FBC, List<FacilityCharge> facilityCharges)
        {
            if (FBC.FacilityBooking == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", FacilityBooking.ClassName, "FacilityBookingCharge"));
                return Global.ACMethodResultState.Notpossible;
            }

            if (FBC.InwardCompanyMaterial == null)
                return Global.ACMethodResultState.Succeeded;

            if (FBC.InwardCompanyMaterialStock == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", "InwardCompanyMaterialStock", FacilityBooking.ClassName));
                return Global.ACMethodResultState.Notpossible;
            }
            else if (BP.AutoRefresh)
                FBC.InwardCompanyMaterialStock.AutoRefresh();

            if (Math.Abs(FBC.InwardQuantityUOM - 0) > Double.Epsilon)
            {
                FBC.InwardCompanyMaterialStock.StockQuantity += FBC.InwardQuantityUOM;
                FBC.InwardCompanyMaterialStock.StockQuantityAmb += FBC.InwardQuantityUOMAmb;
                if (FBC.IsAdjustmentBooking)
                {
                    FBC.InwardCompanyMaterialStock.DayAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardCompanyMaterialStock.DayAdjustmentAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardCompanyMaterialStock.WeekAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardCompanyMaterialStock.WeekAdjustmentAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardCompanyMaterialStock.MonthAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardCompanyMaterialStock.MonthAdjustmentAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardCompanyMaterialStock.YearAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardCompanyMaterialStock.YearAdjustmentAmb += FBC.InwardQuantityUOMAmb;
                }
                OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.CompanyMaterial, BookingEventOnField.QuantityIn, facilityCharges);

                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOff)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.InwardCompanyMaterialStock.DayInward += FBC.InwardQuantityUOM;
                    FBC.InwardCompanyMaterialStock.MonthInward += FBC.InwardQuantityUOM;
                    FBC.InwardCompanyMaterialStock.WeekInward += FBC.InwardQuantityUOM;
                    FBC.InwardCompanyMaterialStock.YearInward += FBC.InwardQuantityUOM;
                    FBC.InwardCompanyMaterialStock.DayInwardAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardCompanyMaterialStock.MonthInwardAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardCompanyMaterialStock.WeekInwardAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardCompanyMaterialStock.YearInwardAmb += FBC.InwardQuantityUOMAmb;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.CompanyMaterial, BookingEventOnField.Inward, facilityCharges);
                }
            }
            if (Math.Abs(FBC.InwardTargetQuantityUOM - 0) > Double.Epsilon)
            {
                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOff)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.InwardCompanyMaterialStock.DayTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardCompanyMaterialStock.MonthTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardCompanyMaterialStock.WeekTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardCompanyMaterialStock.YearTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardCompanyMaterialStock.DayTargetInwardAmb += FBC.InwardTargetQuantityUOMAmb;
                    FBC.InwardCompanyMaterialStock.MonthTargetInwardAmb += FBC.InwardTargetQuantityUOMAmb;
                    FBC.InwardCompanyMaterialStock.WeekTargetInwardAmb += FBC.InwardTargetQuantityUOMAmb;
                    FBC.InwardCompanyMaterialStock.YearTargetInwardAmb += FBC.InwardTargetQuantityUOMAmb;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.CompanyMaterial, BookingEventOnField.TargetInward, facilityCharges);
                }
            }

            return Global.ACMethodResultState.Succeeded;
        }

        /// <summary>
        /// Ausbuchung From-Felder in Tabelle Material 
        /// </summary>
        Global.ACMethodResultState BookCompanyMaterialOut(ACMethodBooking BP, FacilityBookingCharge FBC, List<FacilityCharge> facilityCharges)
        {
            if (FBC.FacilityBooking == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", FacilityBooking.ClassName, "FacilityBookingCharge"));
                return Global.ACMethodResultState.Notpossible;
            }

            if (FBC.OutwardCompanyMaterial == null)
                return Global.ACMethodResultState.Succeeded;

            if (FBC.OutwardCompanyMaterialStock == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", "OutwardCompanyMaterialStock", FacilityBooking.ClassName));
                return Global.ACMethodResultState.Notpossible;
            }
            else if (BP.AutoRefresh)
                FBC.OutwardCompanyMaterialStock.AutoRefresh();

            if (Math.Abs(FBC.OutwardQuantityUOM - 0) > Double.Epsilon)
            {
                FBC.OutwardCompanyMaterialStock.StockQuantity -= FBC.OutwardQuantityUOM;
                FBC.OutwardCompanyMaterialStock.StockQuantityAmb -= FBC.OutwardQuantityUOMAmb;
                if (FBC.IsAdjustmentBooking)
                {
                    FBC.OutwardCompanyMaterialStock.DayAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardCompanyMaterialStock.DayAdjustmentAmb -= FBC.InwardQuantityUOMAmb;
                    FBC.OutwardCompanyMaterialStock.WeekAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardCompanyMaterialStock.WeekAdjustmentAmb -= FBC.InwardQuantityUOMAmb;
                    FBC.OutwardCompanyMaterialStock.MonthAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardCompanyMaterialStock.MonthAdjustmentAmb -= FBC.InwardQuantityUOMAmb;
                    FBC.OutwardCompanyMaterialStock.YearAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardCompanyMaterialStock.YearAdjustmentAmb -= FBC.InwardQuantityUOMAmb;
                }
                OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.CompanyMaterial, BookingEventOnField.QuantityOut, facilityCharges);

                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOff_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.OutwardCompanyMaterialStock.DayOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardCompanyMaterialStock.MonthOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardCompanyMaterialStock.WeekOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardCompanyMaterialStock.YearOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardCompanyMaterialStock.DayOutwardAmb += FBC.OutwardQuantityUOMAmb;
                    FBC.OutwardCompanyMaterialStock.MonthOutwardAmb += FBC.OutwardQuantityUOMAmb;
                    FBC.OutwardCompanyMaterialStock.WeekOutwardAmb += FBC.OutwardQuantityUOMAmb;
                    FBC.OutwardCompanyMaterialStock.YearOutwardAmb += FBC.OutwardQuantityUOMAmb;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.CompanyMaterial, BookingEventOnField.Outward, facilityCharges);
                }
            }
            if (Math.Abs(FBC.OutwardTargetQuantityUOM - 0) > Double.Epsilon)
            {
                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOff_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.OutwardCompanyMaterialStock.DayTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardCompanyMaterialStock.MonthTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardCompanyMaterialStock.WeekTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardCompanyMaterialStock.YearTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardCompanyMaterialStock.DayTargetOutwardAmb += FBC.OutwardTargetQuantityUOMAmb;
                    FBC.OutwardCompanyMaterialStock.MonthTargetOutwardAmb += FBC.OutwardTargetQuantityUOMAmb;
                    FBC.OutwardCompanyMaterialStock.WeekTargetOutwardAmb += FBC.OutwardTargetQuantityUOMAmb;
                    FBC.OutwardCompanyMaterialStock.YearTargetOutwardAmb += FBC.OutwardTargetQuantityUOMAmb;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.CompanyMaterial, BookingEventOnField.TargetOutward, facilityCharges);
                }
            }

            return Global.ACMethodResultState.Succeeded;
        }
        #endregion


        #region Book Contractual partner CompanyMaterial
        /// <summary>
        /// Einbuchung To-Felder in Tabelle Material 
        /// </summary>
        Global.ACMethodResultState BookCPartnerCompMatIn(ACMethodBooking BP, FacilityBookingCharge FBC, List<FacilityCharge> facilityCharges)
        {
            if (BP.ParamsAdjusted.CPartnerCompanyMaterial == null)
                return Global.ACMethodResultState.Succeeded;

            //AddBookingMessage("BookMaterialIn");
            if (FBC.FacilityBooking == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", FacilityBooking.ClassName, "FacilityBookingCharge"));
                return Global.ACMethodResultState.Notpossible;
            }
            if (FBC.InwardCPartnerCompMatStock == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", "InwardCPartnerCompMatStock", FacilityBooking.ClassName));
                return Global.ACMethodResultState.Notpossible;
            }
            else if (BP.AutoRefresh)
                FBC.InwardCPartnerCompMatStock.AutoRefresh();

            //if (Math.Abs(FBC.InwardQuantityUOM - 0) > Double.Epsilon)
            //    FBC.InwardMaterialStock.StockWeight += FBC.InwardWeightInMU;
            if (Math.Abs(FBC.InwardQuantityUOM - 0) > Double.Epsilon)
            {
                FBC.InwardCPartnerCompMatStock.StockQuantity += FBC.InwardQuantityUOM;
                FBC.InwardCPartnerCompMatStock.StockQuantityAmb += FBC.InwardQuantityUOMAmb;
                if (FBC.IsAdjustmentBooking)
                {
                    FBC.InwardCPartnerCompMatStock.DayAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardCPartnerCompMatStock.DayAdjustmentAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardCPartnerCompMatStock.WeekAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardCPartnerCompMatStock.WeekAdjustmentAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardCPartnerCompMatStock.MonthAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardCPartnerCompMatStock.MonthAdjustmentAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardCPartnerCompMatStock.YearAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardCPartnerCompMatStock.YearAdjustmentAmb += FBC.InwardQuantityUOMAmb;
                }
                OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.CPartner, BookingEventOnField.QuantityIn, facilityCharges);

                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOff)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.InwardCPartnerCompMatStock.DayInward += FBC.InwardQuantityUOM;
                    FBC.InwardCPartnerCompMatStock.MonthInward += FBC.InwardQuantityUOM;
                    FBC.InwardCPartnerCompMatStock.WeekInward += FBC.InwardQuantityUOM;
                    FBC.InwardCPartnerCompMatStock.YearInward += FBC.InwardQuantityUOM;
                    FBC.InwardCPartnerCompMatStock.DayInwardAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardCPartnerCompMatStock.MonthInwardAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardCPartnerCompMatStock.WeekInwardAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardCPartnerCompMatStock.YearInwardAmb += FBC.InwardQuantityUOMAmb;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.CPartner, BookingEventOnField.Inward, facilityCharges);
                }
            }
            if (Math.Abs(FBC.InwardTargetQuantityUOM - 0) > Double.Epsilon)
            {
                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOff)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.InwardCPartnerCompMatStock.DayTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardCPartnerCompMatStock.MonthTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardCPartnerCompMatStock.WeekTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardCPartnerCompMatStock.YearTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardCPartnerCompMatStock.DayTargetInwardAmb += FBC.InwardTargetQuantityUOMAmb;
                    FBC.InwardCPartnerCompMatStock.MonthTargetInwardAmb += FBC.InwardTargetQuantityUOMAmb;
                    FBC.InwardCPartnerCompMatStock.WeekTargetInwardAmb += FBC.InwardTargetQuantityUOMAmb;
                    FBC.InwardCPartnerCompMatStock.YearTargetInwardAmb += FBC.InwardTargetQuantityUOMAmb;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.CPartner, BookingEventOnField.TargetInward, facilityCharges);
                }
            }

            //if (BP.ParamsAdjusted.InwardMaterial != null)
            //    ReleaseStateMaterial(FBC.InwardMaterialStock, BP.ParamsAdjusted.MDReleaseState);

            return Global.ACMethodResultState.Succeeded;
        }

        /// <summary>
        /// Ausbuchung From-Felder in Tabelle Material 
        /// </summary>
        Global.ACMethodResultState BookCPartnerCompMatOut(ACMethodBooking BP, FacilityBookingCharge FBC, List<FacilityCharge> facilityCharges)
        {
            if (BP.ParamsAdjusted.CPartnerCompanyMaterial == null)
                return Global.ACMethodResultState.Succeeded;
            //AddBookingMessage("BookMaterialOut");
            if (FBC.FacilityBooking == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", FacilityBooking.ClassName, "FacilityBookingCharge"));
                return Global.ACMethodResultState.Notpossible;
            }
            if (FBC.OutwardCPartnerCompMatStock == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", "OutwardCPartnerCompMatStock", FacilityBooking.ClassName));
                return Global.ACMethodResultState.Notpossible;
            }
            else if (BP.AutoRefresh)
                FBC.OutwardCPartnerCompMatStock.AutoRefresh();

            //if (Math.Abs(FBC.OutwardWeightInMU - 0) > Double.Epsilon)
            //    FBC.OutwardCPartnerCompMatStock.StockWeight -= FBC.OutwardWeightInMU;
            if (Math.Abs(FBC.OutwardQuantityUOM - 0) > Double.Epsilon)
            {
                FBC.OutwardCPartnerCompMatStock.StockQuantity -= FBC.OutwardQuantityUOM;
                FBC.OutwardCPartnerCompMatStock.StockQuantityAmb -= FBC.OutwardQuantityUOMAmb;
                if (FBC.IsAdjustmentBooking)
                {
                    FBC.OutwardCPartnerCompMatStock.DayAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardCPartnerCompMatStock.DayAdjustmentAmb -= FBC.InwardQuantityUOMAmb;
                    FBC.OutwardCPartnerCompMatStock.WeekAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardCPartnerCompMatStock.WeekAdjustmentAmb -= FBC.InwardQuantityUOMAmb;
                    FBC.OutwardCPartnerCompMatStock.MonthAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardCPartnerCompMatStock.MonthAdjustmentAmb -= FBC.InwardQuantityUOMAmb;
                    FBC.OutwardCPartnerCompMatStock.YearAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardCPartnerCompMatStock.YearAdjustmentAmb -= FBC.InwardQuantityUOMAmb;
                }
                OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.CPartner, BookingEventOnField.QuantityOut, facilityCharges);

                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOff_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.OutwardCPartnerCompMatStock.DayOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardCPartnerCompMatStock.MonthOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardCPartnerCompMatStock.WeekOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardCPartnerCompMatStock.YearOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardCPartnerCompMatStock.DayOutwardAmb += FBC.OutwardQuantityUOMAmb;
                    FBC.OutwardCPartnerCompMatStock.MonthOutwardAmb += FBC.OutwardQuantityUOMAmb;
                    FBC.OutwardCPartnerCompMatStock.WeekOutwardAmb += FBC.OutwardQuantityUOMAmb;
                    FBC.OutwardCPartnerCompMatStock.YearOutwardAmb += FBC.OutwardQuantityUOMAmb;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.CPartner, BookingEventOnField.Outward, facilityCharges);
                }
            }
            if (Math.Abs(FBC.OutwardTargetQuantityUOM - 0) > Double.Epsilon)
            {
                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOff_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.OutwardCPartnerCompMatStock.DayTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardCPartnerCompMatStock.MonthTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardCPartnerCompMatStock.WeekTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardCPartnerCompMatStock.YearTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardCPartnerCompMatStock.DayTargetOutwardAmb += FBC.OutwardTargetQuantityUOMAmb;
                    FBC.OutwardCPartnerCompMatStock.MonthTargetOutwardAmb += FBC.OutwardTargetQuantityUOMAmb;
                    FBC.OutwardCPartnerCompMatStock.WeekTargetOutwardAmb += FBC.OutwardTargetQuantityUOMAmb;
                    FBC.OutwardCPartnerCompMatStock.YearTargetOutwardAmb += FBC.OutwardTargetQuantityUOMAmb;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.CPartner, BookingEventOnField.TargetOutward, facilityCharges);
                }
            }

            return Global.ACMethodResultState.Succeeded;
        }
        #endregion


        #region Book Facility
        /// <summary>
        /// Einbuchung To-Felder in Tabelle Facility 
        /// </summary>
        Global.ACMethodResultState BookFacilityIn(ACMethodBooking BP, FacilityBookingCharge FBC, List<FacilityCharge> facilityCharges)
        {
            if (FBC.FacilityBooking == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", FacilityBooking.ClassName, "FacilityBookingCharge"));
                return Global.ACMethodResultState.Notpossible;
            }
            if (FBC.InwardFacilityStock == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", "InwardFacilityStock", FacilityBooking.ClassName));
                return Global.ACMethodResultState.Notpossible;
            }
            else if (BP.AutoRefresh)
                FBC.InwardFacilityStock.AutoRefresh();

            //if (Math.Abs(FBC.InwardWeightInMU - 0) > Double.Epsilon)
            //    FBC.InwardFacilityStock.StockWeight += FBC.InwardWeightInMU;
            if (Math.Abs(FBC.InwardQuantityUOM - 0) > Double.Epsilon)
            {
                FBC.InwardFacilityStock.StockQuantity += FBC.InwardQuantityUOM;
                FBC.InwardFacilityStock.StockQuantityAmb += FBC.InwardQuantityUOMAmb;
                if (FBC.IsAdjustmentBooking)
                {
                    FBC.InwardFacilityStock.DayAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardFacilityStock.DayAdjustmentAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardFacilityStock.WeekAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardFacilityStock.WeekAdjustmentAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardFacilityStock.MonthAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardFacilityStock.MonthAdjustmentAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardFacilityStock.YearAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardFacilityStock.YearAdjustmentAmb += FBC.InwardQuantityUOMAmb;
                }
                OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.Facility, BookingEventOnField.QuantityIn, facilityCharges);

                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOff)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.InwardFacilityStock.DayInward += FBC.InwardQuantityUOM;
                    FBC.InwardFacilityStock.MonthInward += FBC.InwardQuantityUOM;
                    FBC.InwardFacilityStock.WeekInward += FBC.InwardQuantityUOM;
                    FBC.InwardFacilityStock.YearInward += FBC.InwardQuantityUOM;
                    FBC.InwardFacilityStock.DayInwardAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardFacilityStock.MonthInwardAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardFacilityStock.WeekInwardAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardFacilityStock.YearInwardAmb += FBC.InwardQuantityUOMAmb;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.Facility, BookingEventOnField.Inward, facilityCharges);
                }
            }
            if (Math.Abs(FBC.InwardTargetQuantityUOM - 0) > Double.Epsilon)
            {
                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOff)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.InwardFacilityStock.DayTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardFacilityStock.MonthTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardFacilityStock.WeekTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardFacilityStock.YearTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardFacilityStock.DayTargetInwardAmb += FBC.InwardTargetQuantityUOMAmb;
                    FBC.InwardFacilityStock.MonthTargetInwardAmb += FBC.InwardTargetQuantityUOMAmb;
                    FBC.InwardFacilityStock.WeekTargetInwardAmb += FBC.InwardTargetQuantityUOMAmb;
                    FBC.InwardFacilityStock.YearTargetInwardAmb += FBC.InwardTargetQuantityUOMAmb;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.Facility, BookingEventOnField.TargetInward, facilityCharges);
                }
            }

            Facility facilityBin = BP.ParamsAdjusted.InwardFacility;
            if (facilityBin == null)
            {
                if (FBC.InwardFacility != null)
                    facilityBin = FBC.InwardFacility;
                else if (FBC.InwardFacilityCharge != null)
                    facilityBin = FBC.InwardFacilityCharge.Facility;
                else if (facilityCharges != null && facilityCharges.Any())
                    facilityBin = facilityCharges.FirstOrDefault().Facility;
            }
            if (facilityBin != null)
            {
                if (BP.ParamsAdjusted.InwardFacility != null)
                {
                    ReleaseStateFacility(FBC.InwardFacilityStock, BP.ParamsAdjusted.MDReleaseState);
                    SetMaterialAssignmentOnFacility(BP, facilityBin, FBC.InwardMaterial, FBC.InwardFacilityCharge.Partslist);
                }

                // Merke dass auf Silo gebucht worden ist, weil evtl. eine Reorganistation danach stattfinden muss
                if (facilityBin.MDFacilityType != null && facilityBin.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                {
                    if (!BP.InwardCellsWithLotManagedBookings.Contains(facilityBin))
                        BP.InwardCellsWithLotManagedBookings.Add(facilityBin);
                }
            }

            return Global.ACMethodResultState.Succeeded;
        }

        /// <summary>
        /// Ausbuchung From-Felder in Tabelle Facility 
        /// </summary>
        Global.ACMethodResultState BookFacilityOut(ACMethodBooking BP, FacilityBookingCharge FBC, List<FacilityCharge> facilityCharges)
        {
            //AddBookingMessage("BookFacilityOut");
            if (FBC.FacilityBooking == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", FacilityBooking.ClassName, "FacilityBookingCharge"));
                return Global.ACMethodResultState.Notpossible;
            }
            if (FBC.OutwardFacilityStock == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", "OutwardFacilityStock", FacilityBooking.ClassName));
                return Global.ACMethodResultState.Notpossible;
            }
            else if (BP.AutoRefresh)
                FBC.OutwardFacilityStock.AutoRefresh();

            if (Math.Abs(FBC.OutwardQuantityUOM - 0) > Double.Epsilon)
            {
                FBC.OutwardFacilityStock.StockQuantity -= FBC.OutwardQuantityUOM;
                FBC.OutwardFacilityStock.StockQuantityAmb -= FBC.OutwardQuantityUOMAmb;
                if (FBC.IsAdjustmentBooking)
                {
                    FBC.OutwardFacilityStock.DayAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardFacilityStock.DayAdjustmentAmb -= FBC.InwardQuantityUOMAmb;
                    FBC.OutwardFacilityStock.WeekAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardFacilityStock.WeekAdjustmentAmb -= FBC.InwardQuantityUOMAmb;
                    FBC.OutwardFacilityStock.MonthAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardFacilityStock.MonthAdjustmentAmb -= FBC.InwardQuantityUOMAmb;
                    FBC.OutwardFacilityStock.YearAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardFacilityStock.YearAdjustmentAmb -= FBC.InwardQuantityUOMAmb;
                }
                OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.Facility, BookingEventOnField.QuantityOut, facilityCharges);

                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOff_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.OutwardFacilityStock.DayOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardFacilityStock.MonthOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardFacilityStock.WeekOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardFacilityStock.YearOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardFacilityStock.DayOutwardAmb += FBC.OutwardQuantityUOMAmb;
                    FBC.OutwardFacilityStock.MonthOutwardAmb += FBC.OutwardQuantityUOMAmb;
                    FBC.OutwardFacilityStock.WeekOutwardAmb += FBC.OutwardQuantityUOMAmb;
                    FBC.OutwardFacilityStock.YearOutwardAmb += FBC.OutwardQuantityUOMAmb;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.Facility, BookingEventOnField.Outward, facilityCharges);
                }
            }
            if (Math.Abs(FBC.OutwardTargetQuantityUOM - 0) > Double.Epsilon)
            {
                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOff_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.OutwardFacilityStock.DayTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardFacilityStock.MonthTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardFacilityStock.WeekTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardFacilityStock.YearTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardFacilityStock.DayTargetOutwardAmb += FBC.OutwardTargetQuantityUOMAmb;
                    FBC.OutwardFacilityStock.MonthTargetOutwardAmb += FBC.OutwardTargetQuantityUOMAmb;
                    FBC.OutwardFacilityStock.WeekTargetOutwardAmb += FBC.OutwardTargetQuantityUOMAmb;
                    FBC.OutwardFacilityStock.YearTargetOutwardAmb += FBC.OutwardTargetQuantityUOMAmb;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.Facility, BookingEventOnField.TargetOutward, facilityCharges);
                }
            }


            Facility facilityBin = BP.ParamsAdjusted.OutwardFacility;
            if (facilityBin == null)
            {
                if (FBC.OutwardFacility != null)
                    facilityBin = FBC.OutwardFacility;
                else if (FBC.OutwardFacilityCharge != null)
                    facilityBin = FBC.OutwardFacilityCharge.Facility;
                else if (facilityCharges != null && facilityCharges.Any())
                    facilityBin = facilityCharges.FirstOrDefault().Facility;
            }

            if (facilityBin != null)
            {
                if (BP.ParamsAdjusted.OutwardFacility != null)
                    ReleaseStateFacility(FBC.OutwardFacilityStock, BP.ParamsAdjusted.MDReleaseState);

                // Merke dass auf Silo gebucht worden ist, weil evtl. eine Reorganistation danach stattfinden muss
                if (facilityBin.MDFacilityType != null && facilityBin.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                {
                    if (!BP.OutwardCellsWithLotManagedBookings.Contains(facilityBin))
                        BP.OutwardCellsWithLotManagedBookings.Add(facilityBin);
                }
            }

            return Global.ACMethodResultState.Succeeded;
        }
        #endregion


        #region Book FacilityLot
        /// <summary>
        /// Einbuchung To-Felder in Tabelle FacilityLot 
        /// </summary>
        Global.ACMethodResultState BookFacilityLotIn(ACMethodBooking BP, FacilityBookingCharge FBC, List<FacilityCharge> facilityCharges)
        {
            if (FBC.FacilityBooking == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", FacilityBooking.ClassName, "FacilityBookingCharge"));
                return Global.ACMethodResultState.Notpossible;
            }
            if (FBC.InwardFacilityLot == null)
                return Global.ACMethodResultState.Succeeded;

            if (FBC.InwardFacilityLotStock == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", "InwardFacilityLotStock", FacilityBooking.ClassName));
                return Global.ACMethodResultState.Notpossible;
            }
            else if (BP.AutoRefresh)
                FBC.InwardFacilityLotStock.AutoRefresh();

            if (Math.Abs(FBC.InwardQuantityUOM - 0) > Double.Epsilon)
            {
                FBC.InwardFacilityLotStock.StockQuantity += FBC.InwardQuantityUOM;
                FBC.InwardFacilityLotStock.StockQuantityAmb += FBC.InwardQuantityUOMAmb;
                if (FBC.IsAdjustmentBooking)
                {
                    FBC.InwardFacilityLotStock.DayAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardFacilityLotStock.DayAdjustmentAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardFacilityLotStock.WeekAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardFacilityLotStock.WeekAdjustmentAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardFacilityLotStock.MonthAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardFacilityLotStock.MonthAdjustmentAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardFacilityLotStock.YearAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardFacilityLotStock.YearAdjustmentAmb += FBC.InwardQuantityUOMAmb;
                }
                OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.FacilityLot, BookingEventOnField.QuantityIn, facilityCharges);

                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOff)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.InwardFacilityLotStock.DayInward += FBC.InwardQuantityUOM;
                    FBC.InwardFacilityLotStock.MonthInward += FBC.InwardQuantityUOM;
                    FBC.InwardFacilityLotStock.WeekInward += FBC.InwardQuantityUOM;
                    FBC.InwardFacilityLotStock.YearInward += FBC.InwardQuantityUOM;
                    FBC.InwardFacilityLotStock.DayInwardAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardFacilityLotStock.MonthInwardAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardFacilityLotStock.WeekInwardAmb += FBC.InwardQuantityUOMAmb;
                    FBC.InwardFacilityLotStock.YearInwardAmb += FBC.InwardQuantityUOMAmb;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.FacilityLot, BookingEventOnField.Inward, facilityCharges);
                }
            }
            if (Math.Abs(FBC.InwardTargetQuantityUOM - 0) > Double.Epsilon)
            {
                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOff)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.InwardFacilityLotStock.DayTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardFacilityLotStock.MonthTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardFacilityLotStock.WeekTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardFacilityLotStock.YearTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardFacilityLotStock.DayTargetInwardAmb += FBC.InwardTargetQuantityUOMAmb;
                    FBC.InwardFacilityLotStock.MonthTargetInwardAmb += FBC.InwardTargetQuantityUOMAmb;
                    FBC.InwardFacilityLotStock.WeekTargetInwardAmb += FBC.InwardTargetQuantityUOMAmb;
                    FBC.InwardFacilityLotStock.YearTargetInwardAmb += FBC.InwardTargetQuantityUOMAmb;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.FacilityLot, BookingEventOnField.TargetInward, facilityCharges);
                }
            }

            if (BP.ParamsAdjusted.InwardFacilityLot != null)
                ReleaseStateFacilityLot(FBC.InwardFacilityLot, BP.ParamsAdjusted.MDReleaseState);

            return Global.ACMethodResultState.Succeeded;
        }

        /// <summary>
        /// Ausbuchung From-Felder in Tabelle FacilityLot 
        /// </summary>
        Global.ACMethodResultState BookFacilityLotOut(ACMethodBooking BP, FacilityBookingCharge FBC, List<FacilityCharge> facilityCharges)
        {
            //AddBookingMessage("BookFacilityLotOut");
            if (FBC.FacilityBooking == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", FacilityBooking.ClassName, "FacilityBookingCharge"));
                return Global.ACMethodResultState.Notpossible;
            }
            if (FBC.OutwardFacilityLot == null)
                return Global.ACMethodResultState.Succeeded;
            if (FBC.OutwardFacilityLotStock == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", "OutwardFacilityLotStock", FacilityBooking.ClassName));
                return Global.ACMethodResultState.Notpossible;
            }
            else if (BP.AutoRefresh)
                FBC.OutwardFacilityLotStock.AutoRefresh();

            //if (Math.Abs(FBC.OutwardWeightInMU - 0) > Double.Epsilon)
            //    FBC.OutwardFacilityLotStock.StockWeight -= FBC.OutwardWeightInMU;
            if (Math.Abs(FBC.OutwardQuantityUOM - 0) > Double.Epsilon)
            {
                FBC.OutwardFacilityLotStock.StockQuantity -= FBC.OutwardQuantityUOM;
                FBC.OutwardFacilityLotStock.StockQuantityAmb -= FBC.OutwardQuantityUOMAmb;
                if (FBC.IsAdjustmentBooking)
                {
                    FBC.OutwardFacilityLotStock.DayAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardFacilityLotStock.DayAdjustmentAmb -= FBC.InwardQuantityUOMAmb;
                    FBC.OutwardFacilityLotStock.WeekAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardFacilityLotStock.WeekAdjustmentAmb -= FBC.InwardQuantityUOMAmb;
                    FBC.OutwardFacilityLotStock.MonthAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardFacilityLotStock.MonthAdjustmentAmb -= FBC.InwardQuantityUOMAmb;
                    FBC.OutwardFacilityLotStock.YearAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardFacilityLotStock.YearAdjustmentAmb -= FBC.InwardQuantityUOMAmb;
                }
                OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.FacilityLot, BookingEventOnField.QuantityOut, facilityCharges);

                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOff_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.OutwardFacilityLotStock.DayOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardFacilityLotStock.MonthOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardFacilityLotStock.WeekOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardFacilityLotStock.YearOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardFacilityLotStock.DayOutwardAmb += FBC.OutwardQuantityUOMAmb;
                    FBC.OutwardFacilityLotStock.MonthOutwardAmb += FBC.OutwardQuantityUOMAmb;
                    FBC.OutwardFacilityLotStock.WeekOutwardAmb += FBC.OutwardQuantityUOMAmb;
                    FBC.OutwardFacilityLotStock.YearOutwardAmb += FBC.OutwardQuantityUOMAmb;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.FacilityLot, BookingEventOnField.Outward, facilityCharges);
                }
            }
            if (Math.Abs(FBC.OutwardTargetQuantityUOM - 0) > Double.Epsilon)
            {
                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOff_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.OutwardFacilityLotStock.DayTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardFacilityLotStock.MonthTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardFacilityLotStock.WeekTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardFacilityLotStock.YearTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardFacilityLotStock.DayTargetOutwardAmb += FBC.OutwardTargetQuantityUOMAmb;
                    FBC.OutwardFacilityLotStock.MonthTargetOutwardAmb += FBC.OutwardTargetQuantityUOMAmb;
                    FBC.OutwardFacilityLotStock.WeekTargetOutwardAmb += FBC.OutwardTargetQuantityUOMAmb;
                    FBC.OutwardFacilityLotStock.YearTargetOutwardAmb += FBC.OutwardTargetQuantityUOMAmb;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.FacilityLot, BookingEventOnField.TargetOutward, facilityCharges);
                }
            }

            if (BP.ParamsAdjusted.OutwardFacilityLot != null)
                ReleaseStateFacilityLot(FBC.OutwardFacilityLot, BP.ParamsAdjusted.MDReleaseState);

            return Global.ACMethodResultState.Succeeded;
        }
        #endregion


        #region Book Partslist
        /// <summary>
        /// Einbuchung To-Felder in Tabelle Partslist 
        /// </summary>
        Global.ACMethodResultState BookPartslistIn(ACMethodBooking BP, FacilityBookingCharge FBC)
        {
            if (FBC.FacilityBooking == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", FacilityBooking.ClassName, "FacilityBookingCharge"));
                return Global.ACMethodResultState.Notpossible;
            }
            if (FBC.InwardPartslist == null)
                return Global.ACMethodResultState.Succeeded;

            if (FBC.InwardPartslistStock == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", "InwardPartslistStock", FacilityBooking.ClassName));
                return Global.ACMethodResultState.Notpossible;
            }
            else if (BP.AutoRefresh)
                FBC.InwardPartslistStock.AutoRefresh();

            if (Math.Abs(FBC.InwardQuantityUOM - 0) > Double.Epsilon)
            {
                FBC.InwardPartslistStock.StockQuantity += FBC.InwardQuantityUOM;
                if (FBC.IsAdjustmentBooking)
                {
                    FBC.InwardPartslistStock.DayAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardPartslistStock.WeekAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardPartslistStock.MonthAdjustment += FBC.InwardQuantityUOM;
                    FBC.InwardPartslistStock.YearAdjustment += FBC.InwardQuantityUOM;
                }

                OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.Partslist, BookingEventOnField.QuantityIn);

                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOff)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.InwardPartslistStock.DayInward += FBC.InwardQuantityUOM;
                    FBC.InwardPartslistStock.MonthInward += FBC.InwardQuantityUOM;
                    FBC.InwardPartslistStock.WeekInward += FBC.InwardQuantityUOM;
                    FBC.InwardPartslistStock.YearInward += FBC.InwardQuantityUOM;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.Partslist, BookingEventOnField.Inward);
                }
            }
            if (Math.Abs(FBC.InwardTargetQuantityUOM - 0) > Double.Epsilon)
            {
                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOff)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.InwardPartslistStock.DayTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardPartslistStock.MonthTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardPartslistStock.WeekTargetInward += FBC.InwardTargetQuantityUOM;
                    FBC.InwardPartslistStock.YearTargetInward += FBC.InwardTargetQuantityUOM;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.Partslist, BookingEventOnField.TargetInward);
                }
            }

            return Global.ACMethodResultState.Succeeded;
        }

        /// <summary>
        /// Ausbuchung From-Felder in Tabelle Partslist 
        /// </summary>
        Global.ACMethodResultState BookPartslistOut(ACMethodBooking BP, FacilityBookingCharge FBC)
        {
            if (FBC.FacilityBooking == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", FacilityBooking.ClassName, "FacilityBookingCharge"));
                return Global.ACMethodResultState.Notpossible;
            }
            if (FBC.OutwardPartslist == null)
                return Global.ACMethodResultState.Succeeded;
            if (FBC.OutwardPartslistStock == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.EntityPropertyNotSet, Root.Environment.TranslateMessage(this, "Error00011", "OutwardPartslistStock", FacilityBooking.ClassName));
                return Global.ACMethodResultState.Notpossible;
            }
            else if (BP.AutoRefresh)
                FBC.OutwardPartslistStock.AutoRefresh();

            if (Math.Abs(FBC.OutwardQuantityUOM - 0) > Double.Epsilon)
            {
                FBC.OutwardPartslistStock.StockQuantity -= FBC.OutwardQuantityUOM;
                if (FBC.IsAdjustmentBooking)
                {
                    FBC.OutwardPartslistStock.DayAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardPartslistStock.WeekAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardPartslistStock.MonthAdjustment -= FBC.InwardQuantityUOM;
                    FBC.OutwardPartslistStock.YearAdjustment -= FBC.InwardQuantityUOM;
                }
                OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.Partslist, BookingEventOnField.QuantityOut);

                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOff_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.OutwardPartslistStock.DayOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardPartslistStock.MonthOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardPartslistStock.WeekOutward += FBC.OutwardQuantityUOM;
                    FBC.OutwardPartslistStock.YearOutward += FBC.OutwardQuantityUOM;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.Partslist, BookingEventOnField.Outward);
                }
            }
            if (Math.Abs(FBC.OutwardTargetQuantityUOM - 0) > Double.Epsilon)
            {
                if ((BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOff_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes.InwardOn_OutwardOn)
                    || (BP.ParamsAdjusted.MDBalancingMode.BalancingMode == MDBalancingMode.BalancingModes._null))
                {
                    FBC.OutwardPartslistStock.DayTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardPartslistStock.MonthTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardPartslistStock.WeekTargetOutward += FBC.OutwardTargetQuantityUOM;
                    FBC.OutwardPartslistStock.YearTargetOutward += FBC.OutwardTargetQuantityUOM;
                    OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.Partslist, BookingEventOnField.TargetOutward);
                }
            }

            return Global.ACMethodResultState.Succeeded;
        }
        #endregion


        #region Book FacilityCharge
        /// <summary>
        /// Einbuchung To-Felder in Tabelle FacilityCharge 
        /// </summary>
        Global.ACMethodResultState BookFacilityChargeIn(ACMethodBooking BP, FacilityBookingCharge FBC, List<FacilityCharge> facilityCharges)
        {
            // Ohne Charge funzt es nicht, darf aber nicht vorkommen, weil zuvor durch Checkalgorithmus vin Bookingparamter überprüft
            if (FBC.InwardFacilityCharge == null)
                return Global.ACMethodResultState.Failed;
            else if (BP.AutoRefresh)
                FBC.InwardFacilityCharge.AutoRefresh();

            // If relocation posting and target should be set to blocked state for new quants, set quant to blocked
            if (   (   FBC.InwardFacilityCharge.EntityState == EntityState.Added
                    || Math.Abs(FBC.InwardFacilityCharge.StockQuantityUOM - 0) < Double.Epsilon)
                && BP.ParamsAdjusted.MDReleaseState == null
                && BP.ParamsAdjusted.PostingBehaviour == PostingBehaviourEnum.BlockOnRelocation)
            {
                BP.ParamsAdjusted.MDReleaseState = MDReleaseState.DefaultMDReleaseState(BP.DatabaseApp, MDReleaseState.ReleaseStates.Locked);
            }

            if (Math.Abs(FBC.InwardQuantityUOM - 0) > Double.Epsilon)
            {
                FBC.InwardFacilityCharge.StockQuantityUOM += FBC.InwardQuantityUOM;
                FBC.InwardFacilityCharge.StockQuantityUOMAmb += FBC.InwardQuantityUOMAmb;
                OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.FacilityCharge, BookingEventOnField.QuantityIn, facilityCharges);
                double floatVal = 0;
                if (ConvertQuantityInFacilityChargeUnit(BP, FBC.InwardFacilityCharge.StockQuantityUOM, FBC.InwardMaterial.BaseMDUnit, FBC.InwardFacilityCharge, ref floatVal) == Global.ACMethodResultState.Failed)
                    return Global.ACMethodResultState.Failed;
                if (FBC.InwardFacilityCharge.MDUnit.Rounding >= 0)
                {
                    floatVal = FBC.InwardFacilityCharge.MDUnit.GetRoundedValue(floatVal);
                    FBC.InwardFacilityCharge.StockQuantity = floatVal;
                }
                else
                    FBC.InwardFacilityCharge.StockQuantity = floatVal;
                if (FBC.StorageDate.HasValue)
                    FBC.InwardFacilityCharge.FillingDate = FBC.StorageDate.Value;
            }

            ManageFacilityChargeNotAvailableState(BP, FBC, FBC.InwardFacilityCharge);
            ReleaseStateFacilityCharge(FBC.InwardFacilityCharge, BP.ParamsAdjusted.MDReleaseState);
            return Global.ACMethodResultState.Succeeded;
        }

        /// <summary>
        /// Ausbuchung From-Felder in Tabelle FacilityCharge
        /// </summary>
        Global.ACMethodResultState BookFacilityChargeOut(ACMethodBooking BP, FacilityBookingCharge FBC, List<FacilityCharge> facilityCharges)
        {
            // Ohne Charge funzt es nicht, darf aber nicht vorkommen, weil zuvor durch Checkalgorithmus vin Bookingparamter überprüft
            if (FBC.OutwardFacilityCharge == null)
                return Global.ACMethodResultState.Failed;
            else if (BP.AutoRefresh)
                FBC.OutwardFacilityCharge.AutoRefresh();

            if (Math.Abs(FBC.OutwardQuantityUOM - 0) > Double.Epsilon)
            {
                FBC.OutwardFacilityCharge.StockQuantityUOM -= FBC.OutwardQuantityUOM;
                FBC.OutwardFacilityCharge.StockQuantityUOMAmb -= FBC.OutwardQuantityUOMAmb;
                OnBookFacilityBookingCharge(BP, FBC, BookingEventOnEntity.FacilityCharge, BookingEventOnField.QuantityOut, facilityCharges);
                double floatVal = 0;
                if (ConvertQuantityInFacilityChargeUnit(BP, FBC.OutwardFacilityCharge.StockQuantityUOM, FBC.OutwardMaterial.BaseMDUnit, FBC.OutwardFacilityCharge, ref floatVal) == Global.ACMethodResultState.Failed)
                    return Global.ACMethodResultState.Failed;
                if (FBC.OutwardFacilityCharge.MDUnit.Rounding >= 0)
                    FBC.OutwardFacilityCharge.StockQuantity = FBC.OutwardFacilityCharge.MDUnit.GetRoundedValue(floatVal);
                else
                    FBC.OutwardFacilityCharge.StockQuantity = floatVal;
                if (FBC.StorageDate.HasValue)
                    FBC.InwardFacilityCharge.FillingDate = FBC.StorageDate.Value;
            }

            ManageFacilityChargeNotAvailableState(BP, FBC, FBC.OutwardFacilityCharge);
            ReleaseStateFacilityCharge(FBC.OutwardFacilityCharge, BP.ParamsAdjusted.MDReleaseState);
            return Global.ACMethodResultState.Succeeded;
        }
        #endregion

        #region Book PickingPos

        #region Book PickingPos -> Main Methods

        public FacilityPreBooking NewFacilityPreBooking(DatabaseApp dbApp, PickingPos pickingPos, Facility facilityDest = null, double? actualQuantityUOM = null, PostingTypeEnum postingType = PostingTypeEnum.NotDefined)
        {
            if (postingType == PostingTypeEnum.NotDefined)
                postingType = DeterminePostingType(dbApp, pickingPos, pickingPos.Picking);
            IACObject businessEntity = pickingPos;
            if (pickingPos.InOrderPos != null)
            {
                businessEntity = pickingPos.InOrderPos;
                pickingPos.InOrderPos.AutoRefresh(dbApp);
            }
            else if (pickingPos.OutOrderPos != null)
            {
                businessEntity = pickingPos.OutOrderPos;
                pickingPos.OutOrderPos.AutoRefresh(dbApp);
            }

            ACMethodBooking acMethodClone = null;
            FacilityPreBooking facilityPreBooking = null;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(FacilityPreBooking), FacilityPreBooking.NoColumnName, FacilityPreBooking.FormatNewNo, this);
            if (postingType == PostingTypeEnum.Inward)
            {
                if (businessEntity is PickingPos)
                    acMethodClone = BookParamPickingInwardMovementClone(dbApp);
                else
                    acMethodClone = BookParamInOrderPosInwardMovementClone(dbApp);
                facilityPreBooking = FacilityPreBooking.NewACObject(dbApp, businessEntity, secondaryKey);
            }
            else if (postingType == PostingTypeEnum.Outward)
            {
                if (businessEntity is PickingPos)
                    acMethodClone = BookParamPickingOutwardMovementClone(dbApp);
                else
                    acMethodClone = BookParamOutOrderPosOutwardMovementClone(dbApp);
                facilityPreBooking = FacilityPreBooking.NewACObject(dbApp, businessEntity, secondaryKey);
            }
            else
            {
                acMethodClone = BookParamRelocationMovementClone(dbApp);
                facilityPreBooking = FacilityPreBooking.NewACObject(dbApp, businessEntity, secondaryKey);
            }
            if (facilityPreBooking == null)
                return null;

            ACMethodBooking acMethodBooking = acMethodClone.Clone() as ACMethodBooking;

            if (postingType == PostingTypeEnum.Inward)
            {
                //acMethodBooking.InwardQuantity = deliveryNotePos.InOrderPos.TargetQuantityUOM;
                double quantityUOM = 0;
                if (pickingPos.InOrderPos != null)
                {
                    quantityUOM = actualQuantityUOM.HasValue ? actualQuantityUOM.Value : pickingPos.InOrderPos.TargetQuantityUOM - pickingPos.InOrderPos.PreBookingInwardQuantityUOM() - pickingPos.InOrderPos.ActualQuantityUOM;
                    if (pickingPos.InOrderPos.MDUnit != null)
                    {
                        acMethodBooking.InwardQuantity = pickingPos.InOrderPos.Material.ConvertQuantity(quantityUOM, pickingPos.InOrderPos.Material.BaseMDUnit, pickingPos.InOrderPos.MDUnit);
                        acMethodBooking.MDUnit = pickingPos.InOrderPos.MDUnit;
                    }
                    else
                    {
                        acMethodBooking.InwardQuantity = quantityUOM;
                    }
                    acMethodBooking.InwardMaterial = pickingPos.InOrderPos.Material;
                    acMethodBooking.InOrderPos = pickingPos.InOrderPos;
                    if (pickingPos.InOrderPos.InOrder.CPartnerCompany != null)
                        acMethodBooking.CPartnerCompany = pickingPos.InOrderPos.InOrder.CPartnerCompany;
                }
                else if (pickingPos.OutOrderPos != null)
                {
                    quantityUOM = actualQuantityUOM.HasValue ? actualQuantityUOM.Value : pickingPos.OutOrderPos.TargetQuantityUOM - pickingPos.OutOrderPos.PreBookingOutwardQuantityUOM() - pickingPos.OutOrderPos.ActualQuantityUOM;
                    if (pickingPos.OutOrderPos.MDUnit != null)
                    {
                        acMethodBooking.InwardQuantity = pickingPos.OutOrderPos.Material.ConvertQuantity(quantityUOM, pickingPos.OutOrderPos.Material.BaseMDUnit, pickingPos.OutOrderPos.MDUnit);
                        acMethodBooking.MDUnit = pickingPos.OutOrderPos.MDUnit;
                    }
                    else
                    {
                        acMethodBooking.InwardQuantity = quantityUOM;
                    }
                    acMethodBooking.InwardMaterial = pickingPos.OutOrderPos.Material;
                    acMethodBooking.OutOrderPos = pickingPos.OutOrderPos;
                    if (pickingPos.OutOrderPos.OutOrder.CPartnerCompany != null)
                        acMethodBooking.CPartnerCompany = pickingPos.OutOrderPos.OutOrder.CPartnerCompany;
                }
                else
                {
                    if (actualQuantityUOM.HasValue)
                        quantityUOM = actualQuantityUOM.Value;
                    else
                    {
                        //if (pickingPos.TargetQuantity >= 0.0001)
                        quantityUOM = pickingPos.DiffQuantityUOM * -1;
                        //else
                        //    quantityUOM = pickingPos.DiffQuantityUOM;
                    }
                    acMethodBooking.InwardQuantity = quantityUOM;
                    acMethodBooking.InwardFacility = facilityDest != null ? facilityDest : pickingPos.ToFacility;
                    acMethodBooking.PickingPos = pickingPos;
                }

            }
            else if (postingType == PostingTypeEnum.Outward)
            {
                double quantityUOM = 0;
                if (pickingPos.InOrderPos != null)
                {
                    quantityUOM = actualQuantityUOM.HasValue ? actualQuantityUOM.Value : pickingPos.InOrderPos.TargetQuantityUOM - pickingPos.InOrderPos.PreBookingInwardQuantityUOM() - pickingPos.InOrderPos.ActualQuantityUOM;
                    if (pickingPos.InOrderPos.MDUnit != null)
                    {
                        acMethodBooking.OutwardQuantity = pickingPos.InOrderPos.Material.ConvertQuantity(quantityUOM, pickingPos.InOrderPos.Material.BaseMDUnit, pickingPos.InOrderPos.MDUnit);
                        acMethodBooking.MDUnit = pickingPos.InOrderPos.MDUnit;
                    }
                    else
                    {
                        acMethodBooking.OutwardQuantity = quantityUOM;
                    }
                    acMethodBooking.OutwardMaterial = pickingPos.InOrderPos.Material;
                    acMethodBooking.InOrderPos = pickingPos.InOrderPos;
                    if (pickingPos.InOrderPos.InOrder.CPartnerCompany != null)
                        acMethodBooking.CPartnerCompany = pickingPos.InOrderPos.InOrder.CPartnerCompany;
                }
                else if (pickingPos.OutOrderPos != null)
                {
                    quantityUOM = actualQuantityUOM.HasValue ? actualQuantityUOM.Value : pickingPos.OutOrderPos.TargetQuantityUOM - pickingPos.OutOrderPos.PreBookingOutwardQuantityUOM() - pickingPos.OutOrderPos.ActualQuantityUOM;
                    if (pickingPos.OutOrderPos.MDUnit != null)
                    {
                        acMethodBooking.OutwardQuantity = pickingPos.OutOrderPos.Material.ConvertQuantity(quantityUOM, pickingPos.OutOrderPos.Material.BaseMDUnit, pickingPos.OutOrderPos.MDUnit);
                        acMethodBooking.MDUnit = pickingPos.OutOrderPos.MDUnit;
                    }
                    else
                    {
                        acMethodBooking.OutwardQuantity = quantityUOM;
                    }
                    acMethodBooking.OutwardMaterial = pickingPos.OutOrderPos.Material;
                    acMethodBooking.OutOrderPos = pickingPos.OutOrderPos;
                    if (pickingPos.OutOrderPos.OutOrder.CPartnerCompany != null)
                        acMethodBooking.CPartnerCompany = pickingPos.OutOrderPos.OutOrder.CPartnerCompany;
                }
                else
                {
                    if (actualQuantityUOM.HasValue)
                        quantityUOM = actualQuantityUOM.Value;
                    else
                    {
                        //if (pickingPos.TargetQuantity >= 0.0001)
                        quantityUOM = pickingPos.DiffQuantityUOM * -1;
                        //else
                        //    quantityUOM = pickingPos.DiffQuantityUOM;
                    }
                    acMethodBooking.OutwardQuantity = quantityUOM;
                    acMethodBooking.OutwardFacility = pickingPos.FromFacility;
                    if (pickingPos.FromFacility != null && pickingPos.FromFacility.Material != null)
                        acMethodBooking.MDUnit = pickingPos.FromFacility.Material.BaseMDUnit;
                    acMethodBooking.PickingPos = pickingPos;
                }
            }
            else //if (pickingPos.InOrderPos == null && pickingPos.OutOrderPos == null)
            {
                double quantityUOM = 0;
                if (pickingPos.InOrderPos != null)
                {
                    quantityUOM = actualQuantityUOM.HasValue ? actualQuantityUOM.Value : pickingPos.InOrderPos.TargetQuantityUOM - pickingPos.InOrderPos.PreBookingInwardQuantityUOM() - pickingPos.InOrderPos.ActualQuantityUOM;
                    if (pickingPos.InOrderPos.MDUnit != null)
                    {
                        acMethodBooking.InwardQuantity = pickingPos.InOrderPos.Material.ConvertQuantity(quantityUOM, pickingPos.InOrderPos.Material.BaseMDUnit, pickingPos.InOrderPos.MDUnit);
                        acMethodBooking.OutwardQuantity = acMethodBooking.InwardQuantity;
                        acMethodBooking.MDUnit = pickingPos.InOrderPos.MDUnit;
                    }
                    else
                    {
                        acMethodBooking.InwardQuantity = quantityUOM;
                        acMethodBooking.OutwardQuantity = quantityUOM;
                    }
                    acMethodBooking.InwardFacility = facilityDest != null ? facilityDest : pickingPos.ToFacility;
                    acMethodBooking.OutwardFacility = pickingPos.FromFacility;
                    acMethodBooking.InwardMaterial = pickingPos.InOrderPos.Material;
                    acMethodBooking.InOrderPos = pickingPos.InOrderPos;
                    if (pickingPos.InOrderPos.InOrder.CPartnerCompany != null)
                        acMethodBooking.CPartnerCompany = pickingPos.InOrderPos.InOrder.CPartnerCompany;
                }
                else if (pickingPos.OutOrderPos != null)
                {
                    quantityUOM = actualQuantityUOM.HasValue ? actualQuantityUOM.Value : pickingPos.OutOrderPos.TargetQuantityUOM - pickingPos.OutOrderPos.PreBookingOutwardQuantityUOM() - pickingPos.OutOrderPos.ActualQuantityUOM;
                    if (pickingPos.OutOrderPos.MDUnit != null)
                    {
                        acMethodBooking.OutwardQuantity = pickingPos.OutOrderPos.Material.ConvertQuantity(quantityUOM, pickingPos.OutOrderPos.Material.BaseMDUnit, pickingPos.OutOrderPos.MDUnit);
                        acMethodBooking.InwardQuantity = acMethodBooking.OutwardQuantity;
                        acMethodBooking.MDUnit = pickingPos.OutOrderPos.MDUnit;
                    }
                    else
                    {
                        acMethodBooking.OutwardQuantity = quantityUOM;
                        acMethodBooking.InwardQuantity = quantityUOM;
                    }
                    acMethodBooking.InwardFacility = facilityDest != null ? facilityDest : pickingPos.ToFacility;
                    acMethodBooking.OutwardFacility = pickingPos.FromFacility;
                    acMethodBooking.OutwardMaterial = pickingPos.OutOrderPos.Material;
                    acMethodBooking.OutOrderPos = pickingPos.OutOrderPos;
                    if (pickingPos.OutOrderPos.OutOrder.CPartnerCompany != null)
                        acMethodBooking.CPartnerCompany = pickingPos.OutOrderPos.OutOrder.CPartnerCompany;
                }
                else
                {
                    if (actualQuantityUOM.HasValue)
                        quantityUOM = actualQuantityUOM.Value;
                    else
                    {
                        //if (pickingPos.TargetQuantity >= 0.0001)
                        quantityUOM = pickingPos.DiffQuantityUOM * -1;
                        //else
                        //    quantityUOM = pickingPos.DiffQuantityUOM;
                    }
                    acMethodBooking.InwardQuantity = quantityUOM;
                    acMethodBooking.InwardFacility = facilityDest != null ? facilityDest : pickingPos.ToFacility;
                    acMethodBooking.OutwardQuantity = quantityUOM;
                    acMethodBooking.OutwardFacility = pickingPos.FromFacility;
                    if (pickingPos.FromFacility != null && pickingPos.FromFacility.Material != null)
                        acMethodBooking.MDUnit = pickingPos.FromFacility.Material.BaseMDUnit;
                    acMethodBooking.PickingPos = pickingPos;
                }
            }

            // TODO: Restliche Parameter von acMethodBooking ausfüllen
            facilityPreBooking.ACMethodBooking = acMethodBooking;
            return facilityPreBooking;
        }

        public List<FacilityPreBooking> CancelFacilityPreBooking(DatabaseApp dbApp, PickingPos pickingPos, PostingTypeEnum postingType = PostingTypeEnum.NotDefined)
        {
            if (postingType == PostingTypeEnum.NotDefined)
                postingType = DeterminePostingType(dbApp, pickingPos, pickingPos.Picking);

            List<FacilityPreBooking> bookings = new List<FacilityPreBooking>();
            ACMethodBooking acMethodClone = null;
            FacilityPreBooking facilityPreBooking = null;
            if (pickingPos.InOrderPos != null)
            {
                if (pickingPos.InOrderPos.MDInOrderPosState.InOrderPosState == MDInOrderPosState.InOrderPosStates.Cancelled)
                    return null;
                if (pickingPos.InOrderPos.EntityState != EntityState.Added)
                {
                    pickingPos.InOrderPos.FacilityBooking_InOrderPos.AutoLoad(pickingPos.InOrderPos.FacilityBooking_InOrderPosReference, pickingPos);
                    pickingPos.InOrderPos.FacilityPreBooking_InOrderPos.AutoLoad(pickingPos.InOrderPos.FacilityPreBooking_InOrderPosReference, pickingPos);
                }
                else
                    return null;
                if (pickingPos.InOrderPos.FacilityPreBooking_InOrderPos.Any())
                    return null;
                foreach (FacilityBooking previousBooking in pickingPos.InOrderPos.FacilityBooking_InOrderPos)
                {
                    // Wenn einmal Storniert, dann kann nicht mehr storniert werden. Der Fall dürfte normalerweise nicht auftreten, 
                    // da der Positionsstatus auch MDOutOrderPosState.OutOrderPosStates.Cancelled sein müsste
                    if (previousBooking.FacilityBookingType == GlobalApp.FacilityBookingType.InOrderPosCancel)
                        return null;
                }
                foreach (FacilityBooking previousBooking in pickingPos.InOrderPos.FacilityBooking_InOrderPos)
                {
                    if (previousBooking.FacilityBookingType != GlobalApp.FacilityBookingType.InOrderPosInwardMovement)
                        continue;
                    acMethodClone = BookParamInCancelClone(dbApp);
                    string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(FacilityPreBooking), FacilityPreBooking.NoColumnName, FacilityPreBooking.FormatNewNo, this);
                    facilityPreBooking = FacilityPreBooking.NewACObject(dbApp, pickingPos.InOrderPos, secondaryKey);
                    ACMethodBooking acMethodBooking = acMethodClone.Clone() as ACMethodBooking;
                    acMethodBooking.InwardQuantity = previousBooking.InwardQuantity * -1;
                    acMethodBooking.InwardQuantityAmb = previousBooking.InwardQuantityAmb * -1;
                    if (previousBooking.MDUnit != null)
                        acMethodBooking.MDUnit = previousBooking.MDUnit;
                    acMethodBooking.InwardFacility = previousBooking.InwardFacility;
                    if (previousBooking.InwardFacilityLot != null)
                        acMethodBooking.InwardFacilityLot = previousBooking.InwardFacilityLot;
                    if (previousBooking.InwardFacilityCharge != null)
                        acMethodBooking.InwardFacilityCharge = previousBooking.InwardFacilityCharge;
                    if (previousBooking.InwardMaterial != null)
                        acMethodBooking.InwardMaterial = previousBooking.InwardMaterial;
                    else
                        acMethodBooking.InwardMaterial = pickingPos.InOrderPos.Material;
                    if (postingType == PostingTypeEnum.Relocation
                        && Math.Abs(previousBooking.OutwardQuantity - 0) > Double.Epsilon)
                    {
                        acMethodBooking.OutwardQuantity = previousBooking.OutwardQuantity * -1;
                        acMethodBooking.OutwardQuantityAmb = previousBooking.OutwardQuantityAmb * -1;
                        if (previousBooking.MDUnit != null)
                            acMethodBooking.MDUnit = previousBooking.MDUnit;
                        acMethodBooking.OutwardFacility = previousBooking.OutwardFacility;
                        if (previousBooking.OutwardFacilityLot != null)
                            acMethodBooking.OutwardFacilityLot = previousBooking.OutwardFacilityLot;
                        if (previousBooking.OutwardFacilityCharge != null)
                            acMethodBooking.OutwardFacilityCharge = previousBooking.OutwardFacilityCharge;
                        if (previousBooking.OutwardMaterial != null)
                            acMethodBooking.OutwardMaterial = previousBooking.OutwardMaterial;
                        else
                            acMethodBooking.OutwardMaterial = pickingPos.OutOrderPos.Material;
                    }
                    acMethodBooking.InOrderPos = pickingPos.InOrderPos;
                    if (previousBooking.CPartnerCompany != null)
                        acMethodBooking.CPartnerCompany = previousBooking.CPartnerCompany;
                    facilityPreBooking.ACMethodBooking = acMethodBooking;
                    bookings.Add(facilityPreBooking);
                }
            }
            else if (pickingPos.OutOrderPos != null)
            {
                if (pickingPos.OutOrderPos.MDOutOrderPosState.OutOrderPosState == MDOutOrderPosState.OutOrderPosStates.Cancelled)
                    return null;
                if (pickingPos.OutOrderPos.EntityState != EntityState.Added)
                {
                    pickingPos.OutOrderPos.FacilityBooking_OutOrderPos.AutoLoad(pickingPos.OutOrderPos.FacilityBooking_OutOrderPosReference, pickingPos);
                    pickingPos.OutOrderPos.FacilityPreBooking_OutOrderPos.AutoLoad(pickingPos.OutOrderPos.FacilityPreBooking_OutOrderPosReference, pickingPos);
                }
                else
                    return null;
                if (pickingPos.OutOrderPos.FacilityPreBooking_OutOrderPos.Any())
                    return null;
                foreach (FacilityBooking previousBooking in pickingPos.OutOrderPos.FacilityBooking_OutOrderPos)
                {
                    // Wenn einmal Storniert, dann kann nicht mehr storniert werden. Der Fall dürfte normalerweise nicht auftreten, 
                    // da der Positionsstatus auch MDOutOrderPosState.OutOrderPosStates.Cancelled sein müsste
                    if (previousBooking.FacilityBookingType == GlobalApp.FacilityBookingType.OutOrderPosCancel)
                        return null;
                }
                foreach (FacilityBooking previousBooking in pickingPos.OutOrderPos.FacilityBooking_OutOrderPos)
                {
                    if (previousBooking.FacilityBookingType != GlobalApp.FacilityBookingType.OutOrderPosOutwardMovement)
                        continue;
                    acMethodClone = BookParamOutCancelClone(dbApp);
                    string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(FacilityPreBooking), FacilityPreBooking.NoColumnName, FacilityPreBooking.FormatNewNo, this);
                    facilityPreBooking = FacilityPreBooking.NewACObject(dbApp, pickingPos.OutOrderPos, secondaryKey);
                    ACMethodBooking acMethodBooking = acMethodClone.Clone() as ACMethodBooking;
                    acMethodBooking.OutwardQuantity = previousBooking.OutwardQuantity * -1;
                    acMethodBooking.OutwardQuantityAmb = previousBooking.OutwardQuantityAmb * -1;
                    if (previousBooking.MDUnit != null)
                        acMethodBooking.MDUnit = previousBooking.MDUnit;
                    acMethodBooking.OutwardFacility = previousBooking.OutwardFacility;
                    if (previousBooking.OutwardFacilityLot != null)
                        acMethodBooking.OutwardFacilityLot = previousBooking.OutwardFacilityLot;
                    if (previousBooking.OutwardFacilityCharge != null)
                        acMethodBooking.OutwardFacilityCharge = previousBooking.OutwardFacilityCharge;
                    if (previousBooking.OutwardMaterial != null)
                        acMethodBooking.OutwardMaterial = previousBooking.OutwardMaterial;
                    else
                        acMethodBooking.OutwardMaterial = pickingPos.OutOrderPos.Material;

                    if (postingType == PostingTypeEnum.Relocation
                        && Math.Abs(previousBooking.InwardQuantity - 0) > Double.Epsilon)
                    {
                        acMethodBooking.InwardQuantity = previousBooking.InwardQuantity * -1;
                        acMethodBooking.InwardQuantityAmb = previousBooking.InwardQuantityAmb * -1;
                        if (previousBooking.MDUnit != null)
                            acMethodBooking.MDUnit = previousBooking.MDUnit;
                        acMethodBooking.InwardFacility = previousBooking.InwardFacility;
                        if (previousBooking.InwardFacilityLot != null)
                            acMethodBooking.InwardFacilityLot = previousBooking.InwardFacilityLot;
                        if (previousBooking.InwardFacilityCharge != null)
                            acMethodBooking.InwardFacilityCharge = previousBooking.InwardFacilityCharge;
                        if (previousBooking.InwardMaterial != null)
                            acMethodBooking.InwardMaterial = previousBooking.InwardMaterial;
                        else
                            acMethodBooking.InwardMaterial = pickingPos.InOrderPos.Material;
                    }

                    acMethodBooking.OutOrderPos = pickingPos.OutOrderPos;
                    if (previousBooking.CPartnerCompany != null)
                        acMethodBooking.CPartnerCompany = previousBooking.CPartnerCompany;
                    facilityPreBooking.ACMethodBooking = acMethodBooking;
                    bookings.Add(facilityPreBooking);
                }
            }
            else
            {
                if (pickingPos.MDDelivPosLoadState.DelivPosLoadState == MDDelivPosLoadState.DelivPosLoadStates.NewCreated)
                    return null;
                if (pickingPos.EntityState != EntityState.Added)
                {
                    pickingPos.FacilityBooking_PickingPos.AutoLoad(pickingPos.FacilityBooking_PickingPosReference, pickingPos);
                    pickingPos.FacilityPreBooking_PickingPos.AutoLoad(pickingPos.FacilityPreBooking_PickingPosReference, pickingPos);
                }
                else
                    return null;
                if (pickingPos.FacilityBooking_PickingPos.Any())
                    return null;
                //foreach (FacilityBooking previousBooking in pickingPos.FacilityBooking_PickingPos)
                //{
                //    // Wenn einmal Storniert, dann kann nicht mehr storniert werden. Der Fall dürfte normalerweise nicht auftreten, 
                //    // da der Positionsstatus auch MDOutOrderPosState.OutOrderPosStates.Cancelled sein müsste
                //    if (previousBooking.FacilityBookingType == GlobalApp.FacilityBookingType.Relocation_FacilityCharge)
                //        return null;
                //}
                foreach (FacilityBooking previousBooking in pickingPos.FacilityBooking_PickingPos)
                {
                    //if (previousBooking.FacilityBookingType != GlobalApp.FacilityBookingType.OutOrderPosOutwardMovement)
                    //    continue;
                    acMethodClone = BookParamRelocationMovementClone(dbApp);
                    string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(FacilityPreBooking), FacilityPreBooking.NoColumnName, FacilityPreBooking.FormatNewNo, this);
                    facilityPreBooking = FacilityPreBooking.NewACObject(dbApp, pickingPos.OutOrderPos, secondaryKey);
                    ACMethodBooking acMethodBooking = acMethodClone.Clone() as ACMethodBooking;

                    if (Math.Abs(previousBooking.OutwardQuantity - 0) > Double.Epsilon)
                    {
                        acMethodBooking.OutwardQuantity = previousBooking.OutwardQuantity * -1;
                        acMethodBooking.OutwardQuantityAmb = previousBooking.OutwardQuantityAmb * -1;
                        if (previousBooking.MDUnit != null)
                            acMethodBooking.MDUnit = previousBooking.MDUnit;
                        acMethodBooking.OutwardFacility = previousBooking.OutwardFacility;
                        if (previousBooking.OutwardFacilityLot != null)
                            acMethodBooking.OutwardFacilityLot = previousBooking.OutwardFacilityLot;
                        if (previousBooking.OutwardFacilityCharge != null)
                            acMethodBooking.OutwardFacilityCharge = previousBooking.OutwardFacilityCharge;
                        if (previousBooking.OutwardMaterial != null)
                            acMethodBooking.OutwardMaterial = previousBooking.OutwardMaterial;
                        else
                            acMethodBooking.OutwardMaterial = pickingPos.OutOrderPos.Material;
                    }

                    if (Math.Abs(previousBooking.InwardQuantity - 0) > Double.Epsilon)
                    {
                        acMethodBooking.InwardQuantity = previousBooking.InwardQuantity * -1;
                        acMethodBooking.InwardQuantityAmb = previousBooking.InwardQuantityAmb * -1;
                        if (previousBooking.MDUnit != null)
                            acMethodBooking.MDUnit = previousBooking.MDUnit;
                        acMethodBooking.InwardFacility = previousBooking.InwardFacility;
                        if (previousBooking.InwardFacilityLot != null)
                            acMethodBooking.InwardFacilityLot = previousBooking.InwardFacilityLot;
                        if (previousBooking.InwardFacilityCharge != null)
                            acMethodBooking.InwardFacilityCharge = previousBooking.InwardFacilityCharge;
                        if (previousBooking.InwardMaterial != null)
                            acMethodBooking.InwardMaterial = previousBooking.InwardMaterial;
                        else
                            acMethodBooking.InwardMaterial = pickingPos.InOrderPos.Material;
                    }

                    acMethodBooking.PickingPos = pickingPos;
                    //if (previousBooking.CPartnerCompany != null)
                    //    acMethodBooking.CPartnerCompany = previousBooking.CPartnerCompany;
                    facilityPreBooking.ACMethodBooking = acMethodBooking;
                    bookings.Add(facilityPreBooking);
                }
            }
            return bookings;
        }

        public List<FacilityPreBooking> CancelFacilityPreBooking(DatabaseApp dbApp, Picking picking)
        {
            if (picking == null)
                return null;
            if (picking.EntityState != EntityState.Added)
                picking.PickingPos_Picking.AutoLoad(picking.PickingPos_PickingReference, picking);
            List<FacilityPreBooking> result = null;
            foreach (PickingPos pickingPos in picking.PickingPos_Picking)
            {
                var subResult = CancelFacilityPreBooking(dbApp, pickingPos);
                if (subResult != null)
                {
                    if (result == null)
                        result = subResult;
                    else
                        result.AddRange(subResult);
                }
            }
            return result;
        }

        public virtual void RecalcAfterPosting(DatabaseApp dbApp, PickingPos pickingPos, double postedQuantityUOM, bool isCancellation, bool autoSetState = false, PostingTypeEnum postingType = PostingTypeEnum.NotDefined)
        {
            if (postingType == PostingTypeEnum.NotDefined)
                postingType = DeterminePostingType(dbApp, pickingPos, pickingPos.Picking);
            if (pickingPos.InOrderPos != null)
            {
                pickingPos.InOrderPos.TopParentInOrderPos.RecalcActualQuantity();
                if (isCancellation)
                {
                    MDInOrderPosState state = dbApp.MDInOrderPosState.Where(c => c.MDInOrderPosStateIndex == (short)MDInOrderPosState.InOrderPosStates.Cancelled).FirstOrDefault();
                    if (state != null)
                        pickingPos.InOrderPos.MDInOrderPosState = state;
                    pickingPos.InOrderPos.TopParentInOrderPos.CalledUpQuantity -= pickingPos.InOrderPos.TargetQuantity;
                    pickingPos.InOrderPos.TargetQuantity = 0;
                    pickingPos.InOrderPos.TargetQuantityUOM = 0;
                }
                else
                {
                    if (autoSetState
                        && ((pickingPos.TargetQuantityUOM > 0.0001 && pickingPos.DiffQuantityUOM >= 0)
                            || (pickingPos.TargetQuantityUOM < -0.0001 && pickingPos.DiffQuantityUOM <= 0)))
                    {
                        MDInOrderPosState state = dbApp.MDInOrderPosState.Where(c => c.MDInOrderPosStateIndex == (short)MDInOrderPosState.InOrderPosStates.Completed).FirstOrDefault();
                        if (state != null)
                            pickingPos.InOrderPos.MDInOrderPosState = state;
                    }
                }
            }
            else if (pickingPos.OutOrderPos != null)
            {
                pickingPos.OutOrderPos.TopParentOutOrderPos.RecalcActualQuantity();
                if (isCancellation)
                {
                    MDOutOrderPosState state = dbApp.MDOutOrderPosState.Where(c => c.MDOutOrderPosStateIndex == (short)MDOutOrderPosState.OutOrderPosStates.Cancelled).FirstOrDefault();
                    if (state != null)
                        pickingPos.OutOrderPos.MDOutOrderPosState = state;
                    pickingPos.OutOrderPos.TopParentOutOrderPos.CalledUpQuantity -= pickingPos.OutOrderPos.TargetQuantity;
                    pickingPos.OutOrderPos.TargetQuantity = 0;
                    pickingPos.OutOrderPos.TargetQuantityUOM = 0;
                }
                else
                {
                    if (autoSetState
                        && ((pickingPos.TargetQuantityUOM > 0.0001 && pickingPos.DiffQuantityUOM >= 0)
                            || (pickingPos.TargetQuantityUOM < -0.0001 && pickingPos.DiffQuantityUOM <= 0)))
                    {
                        MDOutOrderPosState state = dbApp.MDOutOrderPosState.Where(c => c.MDOutOrderPosStateIndex == (short)MDOutOrderPosState.OutOrderPosStates.Completed).FirstOrDefault();
                        if (state != null)
                            pickingPos.OutOrderPos.MDOutOrderPosState = state;
                    }
                }
            }
            else
            {
                pickingPos.IncreasePickingActualUOM(postedQuantityUOM);
                pickingPos.RecalcActualQuantity();

                if (autoSetState
                    && ((pickingPos.TargetQuantityUOM > 0.0001 && pickingPos.DiffQuantityUOM >= 0)
                        || (pickingPos.TargetQuantityUOM < -0.0001 && pickingPos.DiffQuantityUOM <= 0)))
                {
                    MDDelivPosLoadState state = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();
                    if (state != null)
                        pickingPos.MDDelivPosLoadState = state;
                }
            }
            pickingPos.OnEntityPropertyChanged("ActualQuantity");
            pickingPos.OnEntityPropertyChanged("ActualQuantityUOM");
            pickingPos.OnEntityPropertyChanged("DiffQuantityUOM");
            pickingPos.OnEntityPropertyChanged("PickingDiffQuantityUOM");
        }

        public void InitBookingParamsFromTemplate(ACMethodBooking acMethodBooking, PickingPos pickingPos, FacilityPreBooking ignorePreBooking = null)
        {
            if (acMethodBooking == null
                || pickingPos == null)
                return;

            if (!(acMethodBooking.BookingType == GlobalApp.FacilityBookingType.PickingInward || acMethodBooking.BookingType == GlobalApp.FacilityBookingType.PickingInwardCancel))
                return;

            if (!InitLotFromPreBooking(acMethodBooking, pickingPos, ignorePreBooking))
            {
                InitLotFromPostings(acMethodBooking, pickingPos);
            }
        }

        public MsgWithDetails IsQuantStockConsumed(FacilityCharge fc, DatabaseApp dbApp)
        {
            if (fc != null && fc.StockQuantity <= 0.001)
            {
                //Question50079 :The quant stock is negative. Is quant spent?
                return new MsgWithDetails(this, eMsgLevel.Question, "ACPickingManager", "IsQauntStockConsumed", 1647, "Question50079", eMsgButton.YesNo);
            }

            return null;
        }
        public virtual PostingTypeEnum DeterminePostingType(DatabaseApp dbApp, PickingPos pickingPos, Picking picking)
        {
            PostingTypeEnum postingTypeEnum = PostingTypeEnum.Relocation;
            if (pickingPos.InOrderPos != null)
            {
                postingTypeEnum = PostingTypeEnum.Inward;
            }
            else if (pickingPos.OutOrderPos != null)
            {
                postingTypeEnum = PostingTypeEnum.Outward;
                if (picking.PickingType == GlobalApp.PickingType.IssueVehicle)
                    postingTypeEnum = PostingTypeEnum.Relocation;
            }
            else
            {
                postingTypeEnum = PostingTypeEnum.Relocation;
                if (picking.PickingType == GlobalApp.PickingType.Receipt
                     || picking.PickingType == GlobalApp.PickingType.ReceiptVehicle)
                    postingTypeEnum = PostingTypeEnum.Inward;
                else if (picking.PickingType == GlobalApp.PickingType.Issue)
                    //|| picking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.IssueVehicle))
                    postingTypeEnum = PostingTypeEnum.Outward;
            }
            return postingTypeEnum;
        }

        #endregion

        #region Book PickingPos -> Helper Methods (public)

        ACMethodBooking _BookParamInOrderPosInwardMovementClone;
        public ACMethodBooking BookParamInOrderPosInwardMovementClone(DatabaseApp dbApp)
        {
            if (_BookParamInOrderPosInwardMovementClone != null)
                return _BookParamInOrderPosInwardMovementClone;
            _BookParamInOrderPosInwardMovementClone = ACUrlACTypeSignature("!" + GlobalApp.FBT_InOrderPosInwardMovement.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            _BookParamInOrderPosInwardMovementClone.Database = dbApp;
            return _BookParamInOrderPosInwardMovementClone;
        }

        ACMethodBooking _BookParamOutOrderPosOutwardMovementClone;
        public ACMethodBooking BookParamOutOrderPosOutwardMovementClone(DatabaseApp dbApp)
        {
            if (_BookParamOutOrderPosOutwardMovementClone != null)
                return _BookParamOutOrderPosOutwardMovementClone;
            _BookParamOutOrderPosOutwardMovementClone = ACUrlACTypeSignature("!" + GlobalApp.FBT_OutOrderPosOutwardMovement.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            _BookParamOutOrderPosOutwardMovementClone.Database = dbApp;
            return _BookParamOutOrderPosOutwardMovementClone;
        }

        ACMethodBooking _BookParamInCancelClone;
        public ACMethodBooking BookParamInCancelClone(DatabaseApp dbApp)
        {
            if (_BookParamInCancelClone != null)
                return _BookParamInCancelClone;
            _BookParamInCancelClone = ACUrlACTypeSignature("!" + GlobalApp.FBT_InOrderPosCancel.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            _BookParamInCancelClone.Database = dbApp;
            return _BookParamInCancelClone;
        }

        ACMethodBooking _BookParamOutCancelClone;
        public ACMethodBooking BookParamOutCancelClone(DatabaseApp dbApp)
        {
            if (_BookParamOutCancelClone != null)
                return _BookParamOutCancelClone;
            _BookParamOutCancelClone = ACUrlACTypeSignature("!" + GlobalApp.FBT_OutOrderPosCancel.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            _BookParamOutCancelClone.Database = dbApp;
            return _BookParamOutCancelClone;
        }

        #endregion

        #region Book PickingPos -> Helper Methods (private)

        ACMethodBooking _BookParamPickingInwardMovementClone;
        private ACMethodBooking BookParamPickingInwardMovementClone(DatabaseApp dbApp)
        {
            if (_BookParamPickingInwardMovementClone != null)
                return _BookParamPickingInwardMovementClone;
            _BookParamPickingInwardMovementClone = ACUrlACTypeSignature("!" + GlobalApp.FBT_PickingInward.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            _BookParamPickingInwardMovementClone.Database = dbApp;
            return _BookParamPickingInwardMovementClone;
        }

        ACMethodBooking _BookParamPickingOutwardMovementClone;
        private ACMethodBooking BookParamPickingOutwardMovementClone(DatabaseApp dbApp)
        {
            if (_BookParamPickingOutwardMovementClone != null)
                return _BookParamPickingOutwardMovementClone;
            _BookParamPickingOutwardMovementClone = ACUrlACTypeSignature("!" + GlobalApp.FBT_PickingOutward.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            _BookParamPickingOutwardMovementClone.Database = dbApp;
            return _BookParamPickingOutwardMovementClone;
        }

        ACMethodBooking _BookParamRelocationMovementClone;
        private ACMethodBooking BookParamRelocationMovementClone(DatabaseApp dbApp)
        {
            if (_BookParamRelocationMovementClone != null)
                return _BookParamRelocationMovementClone;
            _BookParamRelocationMovementClone = ACUrlACTypeSignature("!" + GlobalApp.FBT_PickingRelocation.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            _BookParamRelocationMovementClone.Database = dbApp;
            return _BookParamRelocationMovementClone;
        }

        private bool InitLotFromPreBooking(ACMethodBooking acMethodBooking, PickingPos pickingPos, FacilityPreBooking ignorePreBooking = null)
        {
            if (acMethodBooking == null
                || pickingPos == null)
                return false;

            if (acMethodBooking.InwardFacilityLot != null)
                return true;

            FacilityLot existingFacilityLot = null;
            FacilityPreBooking[] existingPrebookings = new FacilityPreBooking[] { };

            if (pickingPos.InOrderPos != null)
                existingPrebookings = pickingPos.InOrderPos.FacilityPreBooking_InOrderPos.Where(c => ignorePreBooking == null || c != ignorePreBooking).ToArray();
            else if (pickingPos.OutOrderPos != null)
                existingPrebookings = pickingPos.OutOrderPos.FacilityPreBooking_OutOrderPos.Where(c => ignorePreBooking == null || c != ignorePreBooking).ToArray();
            else
                existingPrebookings = pickingPos.FacilityPreBooking_PickingPos.Where(c => ignorePreBooking == null || c != ignorePreBooking).ToArray();

            foreach (FacilityPreBooking preBook in existingPrebookings)
            {
                ACMethodBooking methodBooking = preBook.ACMethodBooking as ACMethodBooking;
                if (methodBooking == null || methodBooking.BookingType != acMethodBooking.BookingType)
                    continue;

                existingFacilityLot = methodBooking.InwardFacilityLot;
                if (existingFacilityLot != null)
                    break;
            }

            if (existingFacilityLot != null)
                acMethodBooking.InwardFacilityLot = existingFacilityLot;
            return acMethodBooking.InwardFacilityLot != null;
        }

        private bool InitLotFromPostings(ACMethodBooking acMethodBooking, PickingPos pickingPos)
        {
            if (acMethodBooking == null
                || pickingPos == null)
                return false;
            if (acMethodBooking.InwardFacilityLot != null)
                return true;
            if (pickingPos.InOrderPos != null)
                acMethodBooking.InwardFacilityLot = pickingPos.InOrderPos.FacilityBookingCharge_InOrderPos.Select(c => c.InwardFacilityLot).FirstOrDefault();
            else if (pickingPos.OutOrderPos != null)
                acMethodBooking.InwardFacilityLot = pickingPos.OutOrderPos.FacilityBookingCharge_OutOrderPos.Select(c => c.InwardFacilityLot).FirstOrDefault();
            else
                acMethodBooking.InwardFacilityLot = pickingPos.FacilityBookingCharge_PickingPos.Select(c => c.InwardFacilityLot).FirstOrDefault();
            return acMethodBooking.InwardFacilityLot != null;
        }

        #endregion

        #endregion

        #endregion

        /// <summary>
        /// Stapelbuchungen (Stackcalculator-Ergebnisliste)
        /// </summary>
        #region Stack-Bookingmethods 

        #region Ein- Auslagerungen
        private Global.ACMethodResultState DoInwardStackBooking(ACMethodBooking BP, StackItemList stackItemList, List<FacilityCharge> facilityCharges)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            if (stackItemList.Count <= 0)
                return bookingResult;

            foreach (var stackItem in stackItemList)
            {
                FacilityCharge facilityChargeToBook = null;
                // Falls Charge neu angelegt Charge von externer Assembly, initialisiere mit Daten
                if (stackItem.CloneOnDest)
                {
                    if (BP.IsAutoResetNotAvailable)
                        facilityChargeToBook = TryReactivateFacilityCharge(BP, stackItem.FacilityCharge.Material, BP.ParamsAdjusted.InwardFacility, stackItem.FacilityCharge.FacilityLot, stackItem.FacilityCharge.Partslist, BP.ParamsAdjusted.InwardSplitNo);
                    if (facilityChargeToBook == null)
                        facilityChargeToBook = FacilityCharge.NewACObject(BP.DatabaseApp, null);
                    facilityChargeToBook.CloneFrom(stackItem.FacilityCharge, false);
                    facilityChargeToBook.Facility = BP.ParamsAdjusted.InwardFacility;
                    facilityChargeToBook.NotAvailable = false;
                    UpdateExpirationInfo(BP, facilityChargeToBook);
                    facilityChargeToBook.AddToParentsList();
                    // Einlagerdatum + Eindeutige Reihenfolgennumer der Einlagerung
                    facilityChargeToBook.FillingDate = DateTime.Now;
                    if (BP.ShiftBookingReverse)
                        facilityChargeToBook.FacilityChargeSortNo = facilityChargeToBook.Facility.GetNextFCSortNoReverse(BP.DatabaseApp);
                    else
                        facilityChargeToBook.FacilityChargeSortNo = facilityChargeToBook.Facility.GetNextFCSortNo(BP.DatabaseApp);
                }
                else
                {
                    facilityChargeToBook = stackItem.FacilityCharge;
                }

                FacilityBookingCharge FBC = NewFacilityBookingCharge(BP, false);
                bookingResult = InitFacilityBookingCharge_FromFacilityCharge_Inward(BP, FBC, facilityChargeToBook,
                    false,
                    stackItem.Quantity, (stackItem.Quantity * BP.ParamsAdjusted.FactorInwardTargetQuantityToQuantity), facilityChargeToBook.MDUnit);
                if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                    return bookingResult;

                bookingResult = BookFacilityBookingChargeIn(BP, FBC, facilityCharges);
                if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                    return bookingResult;
            }
            return bookingResult;
        }

        private Global.ACMethodResultState CheckOutwardStackBooking(ACMethodBooking BP, StackItemList stackItemList)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            if (stackItemList.Count <= 0)
                return bookingResult;
            foreach (var stackItem in stackItemList)
            {
                if (stackItem.CloneOnDest)
                {
                    BP.AddBookingMessage(ACMethodBooking.eResultCodes.WrongImplementation, Root.Environment.TranslateMessage(this, "Error00050"));
                    BP.AddBookingMessage(ACMethodBooking.eResultCodes.WrongImplementation, Root.Environment.TranslateMessage(this, "Error00119"));
                    return Global.ACMethodResultState.Failed;
                }
            }
            return bookingResult;
        }

        private Global.ACMethodResultState DoOutwardStackBooking(ACMethodBooking BP, StackItemList stackItemList, List<FacilityCharge> facilityCharges)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            if (stackItemList.Count <= 0)
                return bookingResult;
            bookingResult = CheckOutwardStackBooking(BP, stackItemList);
            if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                return bookingResult;

            foreach (var stackItem in stackItemList)
            {
                FacilityCharge facilityChargeToBook = null;
                // Falls Charge neu angelegt Charge von externer Assembly, initialisiere mit Daten
                if (stackItem.CloneOnDest)
                {
                    if (BP.IsAutoResetNotAvailable)
                        facilityChargeToBook = TryReactivateFacilityCharge(BP, stackItem.FacilityCharge.Material, stackItem.FacilityCharge.Facility, stackItem.FacilityCharge.FacilityLot, stackItem.FacilityCharge.Partslist, BP.ParamsAdjusted.OutwardSplitNo);
                    if (facilityChargeToBook == null)
                        facilityChargeToBook = FacilityCharge.NewACObject(BP.DatabaseApp, null);
                    facilityChargeToBook.CloneFrom(stackItem.FacilityCharge, false);
                    facilityChargeToBook.Facility = BP.ParamsAdjusted.OutwardFacility;
                    facilityChargeToBook.NotAvailable = false;
                    UpdateExpirationInfo(BP, facilityChargeToBook);
                    facilityChargeToBook.AddToParentsList();
                    // Einlagerdatum + Eindeutige Reihenfolgennumer der Einlagerung
                    facilityChargeToBook.FillingDate = DateTime.Now;
                    if (BP.ShiftBookingReverse)
                        facilityChargeToBook.FacilityChargeSortNo = facilityChargeToBook.Facility.GetNextFCSortNoReverse(BP.DatabaseApp);
                    else
                        facilityChargeToBook.FacilityChargeSortNo = facilityChargeToBook.Facility.GetNextFCSortNo(BP.DatabaseApp);
                }
                else
                {
                    facilityChargeToBook = stackItem.FacilityCharge;
                }


                FacilityBookingCharge FBC = NewFacilityBookingCharge(BP, false);
                bookingResult = InitFacilityBookingCharge_FromFacilityCharge_Outward(BP, FBC, facilityChargeToBook,
                                   false,
                                   stackItem.Quantity, (stackItem.Quantity * BP.ParamsAdjusted.FactorInwardTargetQuantityToQuantity), facilityChargeToBook.MDUnit);
                if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                    return bookingResult;

                bookingResult = BookFacilityBookingChargeOut(BP, FBC, facilityCharges);
                if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                    return bookingResult;
            }
            return bookingResult;
        }        
        #endregion

        #region Reorganization of FacilityCharges in Silos
        private Global.ACMethodResultState ReOrganizeCells(ACMethodBooking BP)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            foreach (ACMethodBooking acBooking in BP.FacilityBookings)
            {
                // Falls es Buchungen auf Silos gab, dann müssen die Schichten im Silo eventuell Reorganisiert werden
                foreach (Facility facility in acBooking.InwardCellsWithLotManagedBookings)
                {
                    bookingResult = ReOrganizeCell(acBooking, facility, BP.StackCalculatorInward(this));
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;
                }
                foreach (Facility facility in acBooking.OutwardCellsWithLotManagedBookings)
                {
                    bookingResult = ReOrganizeCell(acBooking, facility, BP.StackCalculatorOutward(this));
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;
                }
            }
            //_facilityBookings = new List<ACMethodBooking>();
            return Global.ACMethodResultState.Succeeded;
        }

        private Global.ACMethodResultState ReOrganizeCell(ACMethodBooking BP, Facility facility, ACStackCalculatorBase stackCalculator)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            if ((facility == null) || (BP == null))
                return Global.ACMethodResultState.Notpossible;
            if (stackCalculator == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.WrongConfigurationInMaterialManagement,
                                                                    Root.Environment.TranslateMessage(this, "Error00014"));
                return Global.ACMethodResultState.Notpossible;
            }

            // Ermittle Schichten im Silo
            List<FacilityCharge> cellChargeList = s_cQry_FCList_Fac_NotAvailable(BP.DatabaseApp, facility.FacilityID, false).ToList();
            if (BP.CreatedPostings != null && BP.CreatedPostings.Any())
            {
                foreach (FacilityCharge modifiedFC in BP.CreatedPostings.Where(c => c.InwardFacilityCharge != null || c.OutwardFacilityCharge != null)
                                                                        .Select(c => c.InwardFacilityCharge != null ? c.InwardFacilityCharge : c.OutwardFacilityCharge))
                {
                    if (!modifiedFC.NotAvailable && !cellChargeList.Contains(modifiedFC))
                        cellChargeList.Add(modifiedFC);
                    else if (modifiedFC.NotAvailable && cellChargeList.Contains(modifiedFC))
                        cellChargeList.Remove(modifiedFC);
                }
            }

            if (cellChargeList.Count == 0)
            {
                if (BP.InwardFacilityCharge != null && !BP.InwardFacilityCharge.NotAvailable)
                    cellChargeList.Add(BP.InwardFacilityCharge);
                else if (BP.OutwardFacilityCharge != null && !BP.OutwardFacilityCharge.NotAvailable)
                    cellChargeList.Add(BP.OutwardFacilityCharge);
            }
            else if (cellChargeList.Count >= 1)
            {
                // If some changed InMemory, then remove them
                cellChargeList.RemoveAll(c => c.NotAvailable);
            }
            if (cellChargeList.Count >= 1)
            {
                if (cellChargeList.Count > 1)
                {
                    // Reorganisiere
                    StackItemList stackItems;
                    MsgBooking msgBookingResult;
                    stackCalculator.ReOrganize(cellChargeList, out stackItems, out msgBookingResult);
                    bookingResult = DoReOrganizeStackBooking(BP, stackItems);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                    {
                        BP.Merge(msgBookingResult);
                        return bookingResult;
                    }
                }
                if (facility.Material == null)
                {
                    FacilityCharge fc = cellChargeList.FirstOrDefault();
                    SetMaterialAssignmentOnFacility(BP, facility, fc.Material.Material1_ProductionMaterial != null ? fc.Material.Material1_ProductionMaterial : fc.Material, null);
                }
            }
            else if (facility.Material != null && !facility.ShouldLeaveMaterialOccupation)
            {
                bool isFacilityInUse = CheckIsFacilityInUse(facility);
                if (!isFacilityInUse)
                {
                    facility.Material = null;
                    facility.Partslist = null;
                }
            }
            return bookingResult;
        }

        private Global.ACMethodResultState DoReOrganizeStackBooking(ACMethodBooking BP, StackItemList stackItemList)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            if (stackItemList.Count <= 0)
                return bookingResult;
            bookingResult = CheckOutwardStackBooking(BP, stackItemList);
            if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                return bookingResult;

            MDBalancingMode.BalancingModes balancingMode = BP.ParamsAdjusted.MDBalancingMode.BalancingMode;
            BP.ParamsAdjusted.MDBalancingMode.BalancingMode = MDBalancingMode.BalancingModes.InwardOff_OutwardOff;
            foreach (var stackItem in stackItemList)
            {
                FacilityBookingCharge FBC = NewFacilityBookingCharge(BP, false);
                bookingResult = InitFacilityBookingCharge_FromFacilityCharge_Outward(BP, FBC, stackItem.FacilityCharge, false, stackItem.Quantity, stackItem.Quantity, stackItem.FacilityCharge.MDUnit);
                if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                    return bookingResult;

                bookingResult = BookFacilityBookingChargeOut(BP, FBC, null);
                if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                    return bookingResult;
            }
            BP.ParamsAdjusted.MDBalancingMode.BalancingMode = balancingMode;
            return bookingResult;
        }
        #endregion
        
        #endregion



        /// <summary>
        /// Setzen von Zuständen auf elementaren Entitäten: Freigabe, Sperren, Material, Nullbestand...
        /// </summary>
        #region Set States and Properties on Basic Entities 

        #region ReleaseState

        Global.ACMethodResultState ReleaseStateFacility(FacilityStock facilityStock, MDReleaseState releaseState)
        {
            if (releaseState == null)
                return Global.ACMethodResultState.Succeeded;
            if (facilityStock.MDReleaseState != releaseState)
            {
                OnChangeReleaseStateFacility(facilityStock, releaseState);
                return Global.ACMethodResultState.Succeeded;
            }
            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual void OnChangeReleaseStateFacility(FacilityStock facilityStock, MDReleaseState releaseState)
        {
            facilityStock.MDReleaseState = releaseState;
        }


        Global.ACMethodResultState ReleaseStateMaterial(MaterialStock materialStock, MDReleaseState releaseState)
        {
            if (releaseState == null)
                return Global.ACMethodResultState.Succeeded;
            if (materialStock.MDReleaseState != releaseState)
            {
                OnChangeReleaseStateMaterial(materialStock, releaseState);
                return Global.ACMethodResultState.Succeeded;
            }
            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual void OnChangeReleaseStateMaterial(MaterialStock materialStock, MDReleaseState releaseState)
        {
            materialStock.MDReleaseState = releaseState;
        }


        Global.ACMethodResultState ReleaseStateFacilityLot(FacilityLot facilityLot, MDReleaseState releaseState)
        {
            if (releaseState == null)
                return Global.ACMethodResultState.Succeeded;
            if (facilityLot.MDReleaseState != releaseState)
            {
                OnChangeReleaseStateFacilityLot(facilityLot, releaseState);
                return Global.ACMethodResultState.Succeeded;
            }
            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual void OnChangeReleaseStateFacilityLot(FacilityLot facilityLot, MDReleaseState releaseState)
        {
            facilityLot.MDReleaseState = releaseState;
        }


        Global.ACMethodResultState ReleaseStateFacilityCharge(FacilityCharge facilityCharge, MDReleaseState releaseState)
        {
            if (releaseState == null)
                return Global.ACMethodResultState.Succeeded;
            if (facilityCharge.MDReleaseState != releaseState)
            {
                OnChangeReleaseStateFacilityCharge(facilityCharge, releaseState);
                return Global.ACMethodResultState.Succeeded;
            }
            return Global.ACMethodResultState.Succeeded;
        }

        protected virtual void OnChangeReleaseStateFacilityCharge(FacilityCharge facilityCharge, MDReleaseState releaseState)
        {
            facilityCharge.MDReleaseState = releaseState;
        }

        #endregion

        #region NotAvailableState
        void ManageFacilityChargeNotAvailableState(ACMethodBooking BP, FacilityBookingCharge FBC, FacilityCharge facilityCharge)
        {
            if (facilityCharge == null)
                return;
            if (Math.Abs(facilityCharge.StockQuantity - 0) <= FacilityConst.C_ZeroStockCompare)
            {
                if (((BP.ParamsAdjusted.MDBookingNotAvailableMode.BookingNotAvailableMode == MDBookingNotAvailableMode.BookingNotAvailableModes.AutoSet)
                        || (BP.ParamsAdjusted.MDBookingNotAvailableMode.BookingNotAvailableMode == MDBookingNotAvailableMode.BookingNotAvailableModes.AutoSetAndReset)
                        || (BP.ParamsAdjusted.MDBookingNotAvailableMode.BookingNotAvailableMode == MDBookingNotAvailableMode.BookingNotAvailableModes._null)
                        || (BP.ParamsAdjusted.MDZeroStockState.ZeroStockState == MDZeroStockState.ZeroStockStates.SetNotAvailable))
                    && (facilityCharge.NotAvailable == false))
                {   
                    facilityCharge.NotAvailable = true;
                    FBC.SetCompleted = true;
                }
                else if ((BP.ParamsAdjusted.MDZeroStockState.ZeroStockState >= MDZeroStockState.ZeroStockStates.ResetIfNotAvailable)
                    && (facilityCharge.NotAvailable == true))
                    facilityCharge.NotAvailable = false;
            }
            else
            {
                if (((BP.ParamsAdjusted.MDBookingNotAvailableMode.BookingNotAvailableMode == MDBookingNotAvailableMode.BookingNotAvailableModes.AutoReset)
                        || (BP.ParamsAdjusted.MDBookingNotAvailableMode.BookingNotAvailableMode == MDBookingNotAvailableMode.BookingNotAvailableModes.AutoSetAndReset)
                        || (BP.ParamsAdjusted.MDZeroStockState.ZeroStockState == MDZeroStockState.ZeroStockStates.ResetIfNotAvailable))
                    && (facilityCharge.NotAvailable == true))
                {
                    facilityCharge.NotAvailable = false;
                }
                else if ((BP.ParamsAdjusted.MDZeroStockState.ZeroStockState == MDZeroStockState.ZeroStockStates.SetNotAvailable)
                    && (facilityCharge.NotAvailable == false))
                {
                    facilityCharge.NotAvailable = true;
                    FBC.SetCompleted = true;
                }
            }
        }
        #endregion

        #region MaterialAssignment on Facility

        Global.ACMethodResultState SetMaterialAssignmentOnFacility(ACMethodBooking BP, Facility facility, Material material, Partslist Partslist)
        {
            if (facility == null)
                return Global.ACMethodResultState.Succeeded;

            if (facility.MDFacilityType == null)
                return Global.ACMethodResultState.Succeeded;

            if (facility.MDFacilityType.FacilityType != FacilityTypesEnum.StorageBinContainer)
                return Global.ACMethodResultState.Succeeded;

            // Falls Belegung zurückgesetzt werden soll, dann dürfen keine Chargen auf dem Lagerplatz vorhanden sein
            if (material == null)
            {
                if (facility.Material == null)
                    return Global.ACMethodResultState.Succeeded;

                if (s_cQry_FCList_Fac_NotAvailable(BP.DatabaseApp, facility.FacilityID, false).Any())
                {
                    BP.AddBookingMessage(ACMethodBooking.eResultCodes.ProhibitedBooking,
                        Root.Environment.TranslateMessage(this, "Error00092",
                                        facility.FacilityNo, facility.FacilityName));
                    return Global.ACMethodResultState.Notpossible;
                }
                facility.Material = null;
                facility.Partslist = null;
            }
            else
            {
                // Falls nicht belegt mit Material
                if (facility.Material == null)
                {
                    // Falls Chargen vorhanden, die ein anderes Material sind, dann kann Belegung nicht geändert werden
                    // Überprüfe ob Chargen im Silo vorhanden sind, die von einem anderen Rezept sind
                    if (Partslist != null)
                    {
                        if (s_cQry_FCList_Fac_OtherPL_NotAvailable(BP.DatabaseApp, facility.FacilityID, Partslist.PartslistID, false).Any())
                        {
                            BP.AddBookingMessage(ACMethodBooking.eResultCodes.ProhibitedBooking,
                                Root.Environment.TranslateMessage(this, "Error00097",
                                                facility.FacilityNo, facility.FacilityName));
                            return Global.ACMethodResultState.Notpossible;
                        }
                    }

                    facility.Material = material.Material1_ProductionMaterial != null ? material.Material1_ProductionMaterial : material;
                    facility.Partslist = Partslist;
                }
                // Falls Belegung gleich
                else if (Material.IsMaterialEqual(facility.Material,material))
                {
                    // Überprüfe und überschreibe Rezeptzuordnung im Lagerplatz
                    if (Partslist != null)
                    {
                        // Falls Rezept vom Material in Facility unterschiedlich,
                        // dann darf Identifikation nur dann geändert werden, wenn keine FacilityChargen enthalten sind
                        if (facility.Partslist != Partslist)
                        {
                            // Falls Chargen vorhanden, dann kann Belegung nicht geändert werden
                            if (s_cQry_FCList_Fac_NotAvailable(BP.DatabaseApp, facility.FacilityID, false).Any())
                            {
                                BP.AddBookingMessage(ACMethodBooking.eResultCodes.ProhibitedBooking,
                                    Root.Environment.TranslateMessage(this, "Error00101",
                                                    facility.FacilityNo, facility.FacilityName));
                                return Global.ACMethodResultState.Notpossible;
                            }
                            facility.Partslist = Partslist;
                        }
                    }
                    else
                    {
                        if (facility.Partslist != null)
                        {
                            // Falls Chargen vorhanden, dann kann Belegung nicht geändert werden
                            if (s_cQry_FCList_Fac_NotAvailable(BP.DatabaseApp, facility.FacilityID, false).Any())
                            {
                                BP.AddBookingMessage(ACMethodBooking.eResultCodes.ProhibitedBooking,
                                    Root.Environment.TranslateMessage(this, "Error00106",
                                                    facility.FacilityNo, facility.FacilityName));
                                return Global.ACMethodResultState.Notpossible;
                            }
                            facility.Partslist = null;
                        }
                    }
                    return Global.ACMethodResultState.Succeeded;
                }
                // Sonst belegtes Material unterschiedlich
                else
                {
                    // Falls Chargen vorhanden, dann kann Belegung nicht geändert werden
                    if (s_cQry_FCList_Fac_NotAvailable(BP.DatabaseApp, facility.FacilityID, false).Any())
                    {
                        BP.AddBookingMessage(ACMethodBooking.eResultCodes.ProhibitedBooking,
                            Root.Environment.TranslateMessage(this, "Error00099",
                                            facility.FacilityNo, facility.FacilityName));
                        return Global.ACMethodResultState.Notpossible;
                    }
                    facility.Partslist = Partslist;
                    facility.Material = material.Material1_ProductionMaterial != null ? material.Material1_ProductionMaterial : material;
                }
            }

            return Global.ACMethodResultState.Succeeded;
        }
        #endregion

        #endregion

        #region FacilityBookingOverview

        public double GetFacilityBookingQuantityUOM(DatabaseApp databaseApp, PAOrderInfo paOrderInfo)
        {
            double fbCTargetQuantityUOM = 0;
            PAOrderInfoEntry pAOrderInfoEntry = paOrderInfo.Entities.Where(c => c.EntityName == nameof(FacilityBookingCharge)).FirstOrDefault();
            if (pAOrderInfoEntry != null)
            {
                FacilityBookingCharge facilityBookingCharge = databaseApp.FacilityBookingCharge.FirstOrDefault(c => c.FacilityBookingChargeID == pAOrderInfoEntry.EntityID);
                if (facilityBookingCharge != null)
                {
                    if (facilityBookingCharge.InwardTargetQuantityUOM > 0)
                    {
                        fbCTargetQuantityUOM = facilityBookingCharge.InwardTargetQuantityUOM;
                    }
                    if (facilityBookingCharge.OutwardTargetQuantityUOM > 0)
                    {
                        fbCTargetQuantityUOM = facilityBookingCharge.OutwardTargetQuantityUOM;
                    }
                }
            }
            return fbCTargetQuantityUOM;
        }

        #endregion

    }
}
