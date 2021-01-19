using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    /// <summary>
    /// Diese partielle Klasse beenhaltet Methoden zum Geneireren und Manipulieren
    /// von Bewegungssätzen FacilityBooking und FacilityBookingCharge
    /// </summary>
    public partial class FacilityManager
    {
        /// <summary>
        /// Globaler Buchungsdatensatz
        /// </summary>
        #region FacilityBooking
        /// <summary>
        /// Generiere Buchungsdatensatz. Der Buchungsdatensatz enthält die Original-Werte bei Buchungsaufruf.
        /// </summary>
        public virtual FacilityBooking NewFacilityBooking(ACMethodBooking BP)
        {
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(FacilityBooking), FacilityBooking.NoColumnName, FacilityBooking.FormatNewNo, this);
            FacilityBooking FB = FacilityBooking.NewACObject(BP.DatabaseApp, null, secondaryKey);
            FB.FacilityBookingType = BP.ParamsAdjusted.BookingType;

            // Configuration paramters
            FB.MDBookingNotAvailableMode = BP.MDBookingNotAvailableMode;
            FB.DontAllowNegativeStock = BP.DontAllowNegativeStock;
            FB.IgnoreManagement = BP.IgnoreManagement;
            FB.QuantityIsAbsolute = BP.QuantityIsAbsolute;
            FB.MDBalancingMode = BP.MDBalancingMode;
            FB.MDMovementReason = BP.MDMovementReason;
            // Booking parameters
            FB.ShiftBookingReverse = BP.ShiftBookingReverse;
            FB.MDZeroStockState = BP.MDZeroStockState;
            FB.MDReleaseState = BP.MDReleaseState;
            FB.MDReservationMode = BP.MDReservationMode;
            FB.SetCompleted = BP.SetCompleted;

            FB.InwardMaterial = BP.InwardMaterial;
            if (BP.ParamsAdjusted.InwardMaterial != null)
                FB.InwardMaterialStock = BP.ParamsAdjusted.InwardMaterial.GetMaterialStock_InsertIfNotExists(BP.DatabaseApp);
            FB.InwardFacility = BP.InwardFacility;
            if (FB.InwardFacility != null)
                FB.InwardFacilityStock = FB.InwardFacility.GetFacilityStock_InsertIfNotExists(BP.DatabaseApp);
            FB.InwardFacilityLocation = BP.InwardFacilityLocation;
            FB.InwardFacilityLot = BP.InwardFacilityLot;
            if (FB.InwardFacilityLot != null)
                FB.InwardFacilityLotStock = FB.InwardFacilityLot.GetFacilityLotStock_InsertIfNotExists(BP.DatabaseApp);
            if (FB.InwardPartslist != null)
                FB.InwardPartslistStock = FB.InwardPartslist.GetPartslistStock_InsertIfNotExists(BP.DatabaseApp);
            if (FB.InwardCompanyMaterial != null)
                FB.InwardCompanyMaterialStock = FB.InwardCompanyMaterial.GetCompanyMaterialStock_InsertIfNotExists(BP.DatabaseApp);
            FB.InwardFacilityCharge = BP.InwardFacilityCharge;
            //FB.InwardFacilityLocation = BP.InwardFacilityLocation;

            FB.OutwardMaterial = BP.OutwardMaterial;
            if (BP.ParamsAdjusted.OutwardMaterial != null)
                FB.OutwardMaterialStock = BP.ParamsAdjusted.OutwardMaterial.GetMaterialStock_InsertIfNotExists(BP.DatabaseApp);
            FB.OutwardFacility = BP.OutwardFacility;
            if (FB.OutwardFacility != null)
                FB.OutwardFacilityStock = FB.OutwardFacility.GetFacilityStock_InsertIfNotExists(BP.DatabaseApp);
            FB.OutwardFacilityLocation = BP.OutwardFacilityLocation;
            FB.OutwardFacilityLot = BP.OutwardFacilityLot;
            if (FB.OutwardFacilityLot != null)
                FB.OutwardFacilityLotStock = FB.OutwardFacilityLot.GetFacilityLotStock_InsertIfNotExists(BP.DatabaseApp);
            if (FB.OutwardPartslist != null)
                FB.OutwardPartslistStock = FB.OutwardPartslist.GetPartslistStock_InsertIfNotExists(BP.DatabaseApp);
            if (FB.OutwardCompanyMaterial != null)
                FB.OutwardCompanyMaterialStock = FB.OutwardCompanyMaterial.GetCompanyMaterialStock_InsertIfNotExists(BP.DatabaseApp);
            FB.OutwardFacilityCharge = BP.OutwardFacilityCharge;
            //FB.OutwardFacilityLocation = BP.OutwardFacilityLocation;

            FB.InwardQuantity = BP.InwardQuantity.HasValue ? BP.InwardQuantity.Value : 0;
            FB.InwardTargetQuantity = BP.InwardTargetQuantity.HasValue ? BP.InwardTargetQuantity.Value : 0;

            FB.OutwardQuantity = BP.OutwardQuantity.HasValue ? BP.OutwardQuantity.Value : 0;
            FB.OutwardTargetQuantity = BP.OutwardTargetQuantity.HasValue ? BP.OutwardTargetQuantity.Value : 0;

            FB.InwardQuantityAmb = BP.InwardQuantityAmb.HasValue ? BP.InwardQuantityAmb.Value : 0;
            FB.InwardTargetQuantityAmb = BP.InwardTargetQuantityAmb.HasValue ? BP.InwardTargetQuantityAmb.Value : 0;

            FB.OutwardQuantityAmb = BP.OutwardQuantityAmb.HasValue ? BP.OutwardQuantityAmb.Value : 0;
            FB.OutwardTargetQuantityAmb = BP.OutwardTargetQuantityAmb.HasValue ? BP.OutwardTargetQuantityAmb.Value : 0;

            FB.MDUnit = BP.MDUnit;

            //FB.WorkOrder = BP.WorkOrder;
            //FB.WorkOrderPos  = BP.WorkOrderPos;
            //FB.InDeliveryNotePosLot = BP.InDeliveryNotePosLot;
            FB.InOrderPos = BP.InOrderPos;
            //FB.OutDeliveryNotePosLot = BP.OutDeliveryNotePosLot;
            FB.OutOrderPos = BP.OutOrderPos;
            FB.ProdOrderPartslistPos = BP.PartslistPos;
            FB.ProdOrderPartslistPosRelation = BP.PartslistPosRelation;
            FB.FacilityInventoryPos = BP.FacilityInventoryPos;
            FB.ProdOrderPartslistPosFacilityLot = BP.ProdOrderPartslistPosFacilityLot;
            FB.PickingPos = BP.PickingPos;
            FB.CPartnerCompany = BP.CPartnerCompany;
            FB.StorageDate = BP.StorageDate;
            FB.ProductionDate = BP.ProductionDate;
            FB.ExpirationDate = BP.ExpirationDate;
            FB.MinimumDurability = BP.MinimumDurability;
            FB.Comment = BP.Comment;
            FB.RecipeOrFactoryInfo = BP.RecipeOrFactoryInfo;
            FB.PropertyACUrl = BP.PropertyACUrl;

            BP.DatabaseApp.FacilityBooking.AddObject(FB);

            BP.FacilityBooking = FB;
            BP.EmptyCellsWithLotManagedBookings();
            BP.FacilityBookings.Add(BP);
            return FB;
        }
        #endregion


        /// <summary>
        /// Methoden auf FacilityBookingCharge 
        /// Buchungsdatensätze für FacilityCharge (Detaillierte Lagerbewegung)
        /// </summary>
        #region FacilityBookingCharge
        
        /// <summary>
        /// Generiere Buchungsdatensatz für FacilityCharge (Detaillierte Lagerbewegung)
        /// </summary>
        public FacilityBookingCharge NewFacilityBookingCharge(ACMethodBooking BP, bool copyEntityValues)
        {
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(FacilityBookingCharge), FacilityBookingCharge.NoColumnName, FacilityBookingCharge.FormatNewNo, this);
            FacilityBookingCharge FBC = FacilityBookingCharge.NewACObject(BP.DatabaseApp, BP.FacilityBooking, secondaryKey);
            FBC.FacilityBooking = BP.FacilityBooking;

            //FBC.BookingTypeACIdentifier = BP.ParamsAdjusted.ACIdentifier;
            FBC.FacilityBookingType = BP.ParamsAdjusted.BookingType;
            FBC.MDBookingNotAvailableMode = BP.ParamsAdjusted.MDBookingNotAvailableMode;
            FBC.DontAllowNegativeStock = BP.ParamsAdjusted.DontAllowNegativeStock;
            FBC.IgnoreManagement = BP.ParamsAdjusted.IgnoreManagement;
            FBC.QuantityIsAbsolute = BP.ParamsAdjusted.QuantityIsAbsolute;
            FBC.MDBalancingMode = BP.ParamsAdjusted.MDBalancingMode;
            // Booking parameters
            FBC.ShiftBookingReverse = BP.ParamsAdjusted.ShiftBookingReverse;
            FBC.MDZeroStockState = BP.ParamsAdjusted.MDZeroStockState;
            FBC.MDReleaseState = BP.ParamsAdjusted.MDReleaseState;
            FBC.MDReservationMode = BP.ParamsAdjusted.MDReservationMode;
            FBC.MDMovementReason = BP.ParamsAdjusted.MDMovementReason;
            FBC.FacilityInventoryPos = BP.ParamsAdjusted.FacilityInventoryPos;

            if (copyEntityValues)
            {
                FBC.InwardMaterial = BP.ParamsAdjusted.InwardMaterial;
                FBC.InwardFacility = BP.ParamsAdjusted.InwardFacility;
                FBC.InwardFacilityLocation = BP.ParamsAdjusted.InwardFacilityLocation;
                FBC.InwardFacilityLot = BP.ParamsAdjusted.InwardFacilityLot;
                FBC.InwardPartslist = BP.ParamsAdjusted.InwardPartslist;
                FBC.InwardFacilityCharge = BP.ParamsAdjusted.InwardFacilityCharge;
                FBC.InwardCompanyMaterial = BP.ParamsAdjusted.InwardCompanyMaterial;
                if (FBC.InwardMaterial != null)
                    FBC.InwardCPartnerCompMat = BP.ParamsAdjusted.CPartnerCompanyMaterial;
                //FBC.InwardFacilityLocation = BP.ParamsAdjusted.InwardFacilityLocation;

                FBC.OutwardMaterial = BP.ParamsAdjusted.OutwardMaterial;
                FBC.OutwardFacility = BP.ParamsAdjusted.OutwardFacility;
                FBC.OutwardFacilityLocation = BP.ParamsAdjusted.OutwardFacilityLocation;
                FBC.OutwardFacilityLot = BP.ParamsAdjusted.OutwardFacilityLot;
                FBC.OutwardPartslist = BP.ParamsAdjusted.OutwardPartslist;
                FBC.OutwardFacilityCharge = BP.ParamsAdjusted.OutwardFacilityCharge;
                FBC.OutwardCompanyMaterial = BP.ParamsAdjusted.OutwardCompanyMaterial;
                if (FBC.OutwardMaterial != null)
                    FBC.OutwardCPartnerCompMat = BP.ParamsAdjusted.CPartnerCompanyMaterial;
                //FBC.OutwardFacilityLocation = BP.ParamsAdjusted.OutwardFacilityLocation;

                FBC.InwardQuantity = BP.ParamsAdjusted.InwardQuantity.HasValue ? BP.ParamsAdjusted.InwardQuantity.Value : 0;
                FBC.InwardTargetQuantity = BP.ParamsAdjusted.InwardTargetQuantity.HasValue ? BP.ParamsAdjusted.InwardTargetQuantity.Value : 0;

                FBC.OutwardQuantity = BP.ParamsAdjusted.OutwardQuantity.HasValue ? BP.ParamsAdjusted.OutwardQuantity.Value : 0;
                FBC.OutwardTargetQuantity = BP.ParamsAdjusted.OutwardTargetQuantity.HasValue ? BP.ParamsAdjusted.OutwardTargetQuantity.Value : 0;

                FBC.MDUnit = BP.ParamsAdjusted.MDUnit;

                //FBC.WorkOrder = BP.Adjusted.WorkOrder;
                //FBC.WorkOrderPos  = BP.Adjusted.WorkOrderPos;
                //FBC.InDeliveryNotePosLot = BP.ParamsAdjusted.InDeliveryNotePosLot;
                FBC.InOrderPos = BP.ParamsAdjusted.InOrderPos;
                //FBC.OutDeliveryNotePosLot = BP.ParamsAdjusted.OutDeliveryNotePosLot;
                FBC.OutOrderPos = BP.ParamsAdjusted.OutOrderPos;
                FBC.ProdOrderPartslistPos = BP.ParamsAdjusted.PartslistPos;
                FBC.ProdOrderPartslistPosRelation = BP.ParamsAdjusted.PartslistPosRelation;
                FBC.ProdOrderPartslistPosFacilityLot = BP.ParamsAdjusted.ProdOrderPartslistPosFacilityLot;
                FBC.PickingPos = BP.ParamsAdjusted.PickingPos;
                FBC.StorageDate = BP.ParamsAdjusted.StorageDate;
                FBC.ProductionDate = BP.ParamsAdjusted.ProductionDate;
                FBC.ExpirationDate = BP.ParamsAdjusted.ExpirationDate;
                FBC.MinimumDurability = BP.ParamsAdjusted.MinimumDurability;
                FBC.Comment = BP.ParamsAdjusted.Comment;
                FBC.RecipeOrFactoryInfo = BP.ParamsAdjusted.RecipeOrFactoryInfo;
            }

            return FBC;
        }


        #region Inward FacilityBookingCharge

        /// <summary>
        /// Initialisiere Zugangs-Buchungsdatensatz mit Informationen aus FacilityCharge
        /// </summary>
        public Global.ACMethodResultState InitFacilityBookingCharge_FromBookingParameter_Inward(ACMethodBooking BP, FacilityBookingCharge FBC)
        {
            return InitFacilityBookingCharge_FromBookingParameter_Inward(BP, FBC, BP.ParamsAdjusted.InwardFacilityCharge);
        }


        /// <summary>
        /// Initialisiere Zugangs-Buchungsdatensatz mit Informationen aus FacilityCharge
        /// </summary>
        public Global.ACMethodResultState InitFacilityBookingCharge_FromBookingParameter_Inward(ACMethodBooking BP, FacilityBookingCharge FBC, FacilityCharge InwardFacilityCharge)
        {
            if ((FBC == null) || (InwardFacilityCharge == null))
                return Global.ACMethodResultState.Notpossible;

            return InitFacilityBookingCharge_FromFacilityCharge_Inward(BP, FBC, InwardFacilityCharge,
                BP.ParamsAdjusted.QuantityIsAbsolute,
                BP.ParamsAdjusted.InwardQuantity, BP.ParamsAdjusted.InwardTargetQuantity, BP.ParamsAdjusted.MDUnit);
        }

        
        /// <summary>
        /// Initialisiere Zugangs-Buchungsdatensatz mit Informationen aus FacilityCharge
        /// </summary>
        public Global.ACMethodResultState InitFacilityBookingCharge_FromFacilityCharge_Inward(ACMethodBooking BP, FacilityBookingCharge FBC, FacilityCharge InwardFacilityCharge,
            Nullable<Boolean> QuantityIsAbsolute,
            Nullable<Double> InwardQuantity, Nullable<Double> InwardTargetQuantity, MDUnit mdQuantityUnit)
        {
            if ((InwardFacilityCharge == null) || (FBC == null))
                return Global.ACMethodResultState.Notpossible;

            FBC.InwardFacilityCharge = InwardFacilityCharge;
            FBC.MDUnit = InwardFacilityCharge.MDUnit;
            FBC.InwardMaterial = InwardFacilityCharge.Material;
            FBC.InwardFacility = BP.ParamsAdjusted.InwardFacility;
            if (FBC.InwardFacility == null)
                FBC.InwardFacility = InwardFacilityCharge.Facility;
            FBC.InwardFacilityLot = BP.ParamsAdjusted.InwardFacilityLot;
            if (FBC.InwardFacilityLot == null)
                FBC.InwardFacilityLot = InwardFacilityCharge.FacilityLot;
            FBC.InwardPartslist = BP.ParamsAdjusted.InwardPartslist;
            if (FBC.InwardPartslist == null)
                FBC.InwardPartslist = InwardFacilityCharge.Partslist;
            FBC.InwardCompanyMaterial = BP.ParamsAdjusted.InwardCompanyMaterial;
            if (FBC.InwardCompanyMaterial == null)
                FBC.InwardCompanyMaterial = InwardFacilityCharge.CompanyMaterial;
            FBC.InwardCPartnerCompMat = BP.ParamsAdjusted.CPartnerCompanyMaterial;
            if (FBC.InwardCPartnerCompMat == null && InwardFacilityCharge.CPartnerCompanyMaterial != null)
                FBC.InwardCPartnerCompMat = InwardFacilityCharge.CPartnerCompanyMaterial;
            FBC.InOrderPos = BP.ParamsAdjusted.InOrderPos;
            FBC.ProdOrderPartslistPos = BP.ParamsAdjusted.PartslistPos;
            FBC.ProdOrderPartslistPosFacilityLot = BP.ParamsAdjusted.ProdOrderPartslistPosFacilityLot;

            //FBC.InwardFacilityLocation = BP.ParamsAdjusted.InwardFacilityLocation;
            //if (FBC.InwardFacilityLocation == null)
            //{
            //    if (FBC.InwardFacility != null)
            //        FBC.InwardFacilityLocation = FBC.InwardFacility.FacilityLocation;
            //}

            if (FBC.FacilityBooking != null)
            {
                if (FBC.FacilityBooking.InwardMaterial != null)
                {
                    if (FBC.FacilityBooking.InwardMaterialStock != null)
                        FBC.InwardMaterialStock = FBC.FacilityBooking.InwardMaterialStock;
                    else
                        FBC.InwardMaterialStock = FBC.FacilityBooking.InwardMaterial.GetMaterialStock_InsertIfNotExists(BP.DatabaseApp);
                }
                else if (FBC.InwardMaterial != null)
                    FBC.InwardMaterialStock = FBC.InwardMaterial.GetMaterialStock_InsertIfNotExists(BP.DatabaseApp);

                if (FBC.FacilityBooking.InwardFacility != null)
                {
                    if (FBC.FacilityBooking.InwardFacilityStock != null)
                        FBC.InwardFacilityStock = FBC.FacilityBooking.InwardFacilityStock;
                    else
                        FBC.InwardFacilityStock = FBC.FacilityBooking.InwardFacility.GetFacilityStock_InsertIfNotExists(BP.DatabaseApp);
                }
                else if (FBC.InwardFacility != null)
                    FBC.InwardFacilityStock = FBC.InwardFacility.GetFacilityStock_InsertIfNotExists(BP.DatabaseApp);

                if (FBC.FacilityBooking.InwardFacilityLot != null)
                {
                    if (FBC.FacilityBooking.InwardFacilityLotStock != null)
                        FBC.InwardFacilityLotStock = FBC.FacilityBooking.InwardFacilityLotStock;
                    else
                        FBC.InwardFacilityLotStock = FBC.FacilityBooking.InwardFacilityLot.GetFacilityLotStock_InsertIfNotExists(BP.DatabaseApp);
                }
                else if (FBC.InwardFacilityLot != null)
                    FBC.InwardFacilityLotStock = FBC.InwardFacilityLot.GetFacilityLotStock_InsertIfNotExists(BP.DatabaseApp);

                if (FBC.FacilityBooking.InwardPartslist != null)
                {
                    if (FBC.FacilityBooking.InwardPartslistStock != null)
                        FBC.InwardPartslistStock = FBC.FacilityBooking.InwardPartslistStock;
                    else
                        FBC.InwardPartslistStock = FBC.FacilityBooking.InwardPartslist.GetPartslistStock_InsertIfNotExists(BP.DatabaseApp);
                }
                else if (FBC.InwardPartslist != null)
                    FBC.InwardPartslistStock = FBC.InwardPartslist.GetPartslistStock_InsertIfNotExists(BP.DatabaseApp);

                if (FBC.FacilityBooking.InwardCompanyMaterial != null)
                {
                    if (FBC.FacilityBooking.InwardCompanyMaterialStock != null)
                        FBC.InwardCompanyMaterialStock = FBC.FacilityBooking.InwardCompanyMaterialStock;
                    else
                        FBC.InwardCompanyMaterialStock = FBC.FacilityBooking.InwardCompanyMaterial.GetCompanyMaterialStock_InsertIfNotExists(BP.DatabaseApp);
                }
                else if (FBC.InwardCompanyMaterial != null)
                    FBC.InwardCompanyMaterialStock = FBC.InwardCompanyMaterial.GetCompanyMaterialStock_InsertIfNotExists(BP.DatabaseApp);

                if (BP.ParamsAdjusted.CPartnerCompanyMaterial != null)
                {
                    if (FBC.FacilityBooking.InwardCPartnerCompMatStock != null)
                        FBC.InwardCPartnerCompMatStock = FBC.FacilityBooking.InwardCPartnerCompMatStock;
                    else
                        FBC.InwardCPartnerCompMatStock = BP.ParamsAdjusted.CPartnerCompanyMaterial.GetCompanyMaterialStock_InsertIfNotExists(BP.DatabaseApp);
                }
                else if (FBC.InwardCPartnerCompMat != null)
                    FBC.InwardCPartnerCompMatStock = FBC.InwardCPartnerCompMat.GetCompanyMaterialStock_InsertIfNotExists(BP.DatabaseApp);
            }

            try
            {
                if (BP.ParamsAdjusted.MDZeroStockState != null && BP.ParamsAdjusted.MDZeroStockState.ZeroStockState != MDZeroStockState.ZeroStockStates.Off)
                {
                    Double floatVal = 0;
                    if ((BP.ParamsAdjusted.MDZeroStockState.ZeroStockState == MDZeroStockState.ZeroStockStates.BookToZeroStock) || (BP.ParamsAdjusted.MDZeroStockState.ZeroStockState == MDZeroStockState.ZeroStockStates.SetNotAvailable))
                    {
                        if (Math.Abs(InwardFacilityCharge.StockQuantityUOM - 0) <= Double.Epsilon)
                        {
                            FBC.InwardQuantityUOM = 0;
                            FBC.InwardQuantityUOMAmb = 0;
                        }
                        else
                        {
                            FBC.InwardQuantityUOM = InwardFacilityCharge.StockQuantityUOM * (-1);
                            FBC.InwardQuantityUOMAmb = InwardFacilityCharge.StockQuantityUOMAmb * (-1);
                        }

                        floatVal = 0;
                        if (ConvertQuantityUOMToFacilityChargeUnit(BP, FBC.InwardQuantityUOM, InwardFacilityCharge, ref floatVal) == Global.ACMethodResultState.Failed)
                            return Global.ACMethodResultState.Failed;
                        if (InwardFacilityCharge.MDUnit.Rounding >= 0)
                        {
                            floatVal = Math.Round(floatVal, InwardFacilityCharge.MDUnit.Rounding);
                            FBC.InwardQuantity = floatVal;
                        }
                        else
                            FBC.InwardQuantity = floatVal;

                        FBC.InwardTargetQuantity = floatVal;
                        FBC.InwardTargetQuantityUOM = FBC.InwardQuantityUOM;

                        FBC.InwardTargetQuantityUOMAmb = FBC.InwardQuantityUOMAmb * BP.FactorInwardTargetAmbient;

                        //if (Math.Abs(InwardFacilityCharge.StockWeight - 0) <= Double.Epsilon)
                        //    FBC.InwardWeight = 0;
                        //else
                        //    FBC.InwardWeight = InwardFacilityCharge.StockWeight * (-1);

                        //FBC.InwardTargetWeight = FBC.InwardWeight;
                        //FBC.InwardTargetWeightInMU = FBC.InwardWeightInMU;
                    }
                    else // (BP.Adjusted.ZeroStock == Global.ZeroStockState.ResetIfNotAvailable)
                    {
                        FBC.InwardQuantity = 0;
                        FBC.InwardQuantityUOM = 0;
                        FBC.InwardTargetQuantity = 0;
                        FBC.InwardTargetQuantityUOM = 0;
                        FBC.InwardQuantityUOMAmb = 0;
                        FBC.InwardTargetQuantityUOMAmb = 0;
                    }
                }
                else if (QuantityIsAbsolute == true)
                {
                    Double floatVal = 0;
                    if (InwardQuantity.HasValue)
                    {
                        floatVal = 0;
                        if (ConvertQuantityToMaterialBaseUnit(BP, InwardQuantity, mdQuantityUnit, FBC.InwardMaterial, ref floatVal) == Global.ACMethodResultState.Failed)
                            return Global.ACMethodResultState.Failed;
                        FBC.InwardQuantityUOM = floatVal - FBC.InwardFacilityCharge.StockQuantityUOM;
                        FBC.InwardQuantityUOMAmb = (floatVal * BP.FactorInwardAmbient) - FBC.InwardFacilityCharge.StockQuantityUOMAmb;

                        floatVal = 0;
                        if (ConvertQuantityInFacilityChargeUnit(BP, FBC.InwardQuantityUOM, FBC.InwardMaterial.BaseMDUnit, FBC.InwardFacilityCharge, ref floatVal) == Global.ACMethodResultState.Failed)
                            return Global.ACMethodResultState.Failed;
                        if (InwardFacilityCharge.MDUnit.Rounding >= 0)
                            FBC.InwardQuantity = Math.Round(floatVal, InwardFacilityCharge.MDUnit.Rounding);
                        else
                            FBC.InwardQuantity = floatVal;
                    }

                    if (InwardTargetQuantity.HasValue)
                    {
                        floatVal = 0;
                        if (ConvertQuantityToMaterialBaseUnit(BP, InwardTargetQuantity, mdQuantityUnit, FBC.InwardMaterial, ref floatVal) == Global.ACMethodResultState.Failed)
                            return Global.ACMethodResultState.Failed;
                        FBC.InwardTargetQuantityUOM = floatVal;
                        FBC.InwardTargetQuantityUOMAmb = floatVal * BP.FactorInwardTargetAmbient;

                        floatVal = 0;
                        if (ConvertQuantityInFacilityChargeUnit(BP, FBC.InwardTargetQuantityUOM, FBC.InwardMaterial.BaseMDUnit, FBC.InwardFacilityCharge, ref floatVal) == Global.ACMethodResultState.Failed)
                            return Global.ACMethodResultState.Failed;
                        if (InwardFacilityCharge.MDUnit.Rounding >= 0)
                            FBC.InwardTargetQuantity = Math.Round(floatVal, InwardFacilityCharge.MDUnit.Rounding);
                        else
                            FBC.InwardTargetQuantity = floatVal;
                    }
                }
                else
                {
                    Double floatVal = 0;
                    if (InwardQuantity.HasValue)
                    {
                        floatVal = 0;
                        if (ConvertQuantityToMaterialBaseUnit(BP, InwardQuantity, mdQuantityUnit, FBC.InwardMaterial, ref floatVal) == Global.ACMethodResultState.Failed)
                            return Global.ACMethodResultState.Failed;
                        FBC.InwardQuantityUOM = floatVal;
                        FBC.InwardQuantityUOMAmb = floatVal * BP.FactorInwardAmbient;

                        floatVal = 0;
                        if (ConvertQuantityInFacilityChargeUnit(BP, FBC.InwardQuantityUOM, FBC.InwardMaterial.BaseMDUnit, FBC.InwardFacilityCharge, ref floatVal) == Global.ACMethodResultState.Failed)
                            return Global.ACMethodResultState.Failed;
                        if (InwardFacilityCharge.MDUnit.Rounding >= 0)
                            FBC.InwardQuantity = Math.Round(floatVal, InwardFacilityCharge.MDUnit.Rounding);
                        else
                            FBC.InwardQuantity = floatVal;
                    }

                    if (InwardTargetQuantity.HasValue)
                    {
                        floatVal = 0;
                        if (ConvertQuantityToMaterialBaseUnit(BP, InwardTargetQuantity, mdQuantityUnit, FBC.InwardMaterial, ref floatVal) == Global.ACMethodResultState.Failed)
                            return Global.ACMethodResultState.Failed;
                        FBC.InwardTargetQuantityUOM = floatVal;
                        FBC.InwardTargetQuantityUOMAmb = floatVal * BP.FactorInwardTargetAmbient;

                        floatVal = 0;
                        if (ConvertQuantityInFacilityChargeUnit(BP, FBC.InwardTargetQuantityUOM, FBC.InwardMaterial.BaseMDUnit, FBC.InwardFacilityCharge, ref floatVal) == Global.ACMethodResultState.Failed)
                            return Global.ACMethodResultState.Failed;
                        if (InwardFacilityCharge.MDUnit.Rounding >= 0)
                            FBC.InwardTargetQuantity = Math.Round(floatVal, InwardFacilityCharge.MDUnit.Rounding);
                        else
                            FBC.InwardTargetQuantity = floatVal;
                    }
                }
            }
            catch (Exception e)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.QuantityConversionError, e.Message);
                return Global.ACMethodResultState.Failed;
            }

            return Global.ACMethodResultState.Succeeded;
        }

        #endregion


        #region Outward FacilityBookingCharge
        
        /// <summary>
        /// Initialisiere Abgangs-Buchungsdatensatz mit Informationen aus FacilityCharge
        /// </summary>
        public Global.ACMethodResultState InitFacilityBookingCharge_FromBookingParameter_Outward(ACMethodBooking BP, FacilityBookingCharge FBC)
        {
            return InitFacilityBookingCharge_FromBookingParameter_Outward(BP, FBC, BP.ParamsAdjusted.OutwardFacilityCharge);
        }

        /// <summary>
        /// Initialisiere Abgangs-Buchungsdatensatz mit Informationen aus FacilityCharge
        /// </summary>
        public Global.ACMethodResultState InitFacilityBookingCharge_FromBookingParameter_Outward(ACMethodBooking BP, FacilityBookingCharge FBC, FacilityCharge OutwardFacilityCharge)
        {
            if ((FBC == null) || (OutwardFacilityCharge == null))
                return Global.ACMethodResultState.Notpossible;

            return InitFacilityBookingCharge_FromFacilityCharge_Outward(BP, FBC, OutwardFacilityCharge,
                BP.ParamsAdjusted.QuantityIsAbsolute,
                BP.ParamsAdjusted.OutwardQuantity, BP.ParamsAdjusted.OutwardTargetQuantity, BP.ParamsAdjusted.MDUnit);
        }

        
        /// <summary>
        /// Initialisiere Abgangs-Buchungsdatensatz mit Informationen aus FacilityCharge
        /// </summary>
        public Global.ACMethodResultState InitFacilityBookingCharge_FromFacilityCharge_Outward(ACMethodBooking BP, FacilityBookingCharge FBC, FacilityCharge OutwardFacilityCharge,
            Nullable<Boolean> QuantityIsAbsolute,
            Nullable<Double> OutwardQuantity, Nullable<Double> OutwardTargetQuantity, MDUnit mdQuantityUnit)
        {
            if ((OutwardFacilityCharge == null) || (FBC == null))
                return Global.ACMethodResultState.Notpossible;

            FBC.OutwardFacilityCharge = OutwardFacilityCharge;
            FBC.MDUnit = OutwardFacilityCharge.MDUnit;
            FBC.OutwardMaterial = OutwardFacilityCharge.Material;
            FBC.OutwardFacility = BP.ParamsAdjusted.OutwardFacility;
            if (FBC.OutwardFacility == null)
                FBC.OutwardFacility = FBC.OutwardFacilityCharge.Facility;
            FBC.OutwardFacilityLot = BP.ParamsAdjusted.OutwardFacilityLot;
            if (FBC.OutwardFacilityLot == null)
                FBC.OutwardFacilityLot = FBC.OutwardFacilityCharge.FacilityLot;
            FBC.OutwardPartslist = BP.ParamsAdjusted.OutwardPartslist;
            if (FBC.OutwardPartslist == null)
                FBC.OutwardPartslist = FBC.OutwardFacilityCharge.Partslist;
            //FBC.OutwardFacilityLocation = BP.ParamsAdjusted.OutwardFacilityLocation;
            //if (FBC.OutwardFacilityLocation == null)
            //{
            //    if (FBC.OutwardFacility != null)
            //        FBC.OutwardFacilityLocation = FBC.OutwardFacility.FacilityLocation;
            //}
            FBC.OutwardCompanyMaterial = BP.ParamsAdjusted.OutwardCompanyMaterial;
            if (FBC.OutwardCompanyMaterial == null)
                FBC.OutwardCompanyMaterial = OutwardFacilityCharge.CompanyMaterial;
            FBC.OutwardCPartnerCompMat = BP.ParamsAdjusted.CPartnerCompanyMaterial;
            if (FBC.OutwardCPartnerCompMat == null && OutwardFacilityCharge.CPartnerCompanyMaterial != null)
                FBC.OutwardCPartnerCompMat = OutwardFacilityCharge.CPartnerCompanyMaterial;
            FBC.OutOrderPos = BP.ParamsAdjusted.OutOrderPos;
            FBC.ProdOrderPartslistPosRelation = BP.ParamsAdjusted.PartslistPosRelation;
            FBC.FacilityInventoryPos = BP.ParamsAdjusted.FacilityInventoryPos;
            FBC.PickingPos = BP.ParamsAdjusted.PickingPos;

            if (FBC.FacilityBooking != null)
            {
                if (FBC.FacilityBooking.OutwardMaterial != null)
                {
                    if (FBC.FacilityBooking.OutwardMaterialStock != null)
                        FBC.OutwardMaterialStock = FBC.FacilityBooking.OutwardMaterialStock;
                    else
                        FBC.OutwardMaterialStock = FBC.FacilityBooking.OutwardMaterial.GetMaterialStock_InsertIfNotExists(BP.DatabaseApp);
                }
                else if (FBC.OutwardMaterial != null)
                    FBC.OutwardMaterialStock = FBC.OutwardMaterial.GetMaterialStock_InsertIfNotExists(BP.DatabaseApp);

                if (FBC.FacilityBooking.OutwardFacility != null)
                {
                    if (FBC.FacilityBooking.OutwardFacilityStock != null)
                        FBC.OutwardFacilityStock = FBC.FacilityBooking.OutwardFacilityStock;
                    else
                        FBC.OutwardFacilityStock = FBC.FacilityBooking.OutwardFacility.GetFacilityStock_InsertIfNotExists(BP.DatabaseApp);
                }
                else if (FBC.OutwardFacility != null)
                    FBC.OutwardFacilityStock = FBC.OutwardFacility.GetFacilityStock_InsertIfNotExists(BP.DatabaseApp);

                if (FBC.FacilityBooking.OutwardFacilityLot != null)
                {
                    if (FBC.FacilityBooking.OutwardFacilityLotStock != null)
                        FBC.OutwardFacilityLotStock = FBC.FacilityBooking.OutwardFacilityLotStock;
                    else
                        FBC.OutwardFacilityLotStock = FBC.FacilityBooking.OutwardFacilityLot.GetFacilityLotStock_InsertIfNotExists(BP.DatabaseApp);
                }
                else if (FBC.OutwardFacilityLot != null)
                    FBC.OutwardFacilityLotStock = FBC.OutwardFacilityLot.GetFacilityLotStock_InsertIfNotExists(BP.DatabaseApp);

                if (FBC.FacilityBooking.OutwardPartslist != null)
                {
                    if (FBC.FacilityBooking.OutwardPartslistStock != null)
                        FBC.OutwardPartslistStock = FBC.FacilityBooking.OutwardPartslistStock;
                    else
                        FBC.OutwardPartslistStock = FBC.FacilityBooking.OutwardPartslist.GetPartslistStock_InsertIfNotExists(BP.DatabaseApp);
                }
                else if (FBC.OutwardPartslist != null)
                    FBC.OutwardPartslistStock = FBC.OutwardPartslist.GetPartslistStock_InsertIfNotExists(BP.DatabaseApp);

                if (FBC.FacilityBooking.OutwardCompanyMaterial != null)
                {
                    if (FBC.FacilityBooking.OutwardCompanyMaterialStock != null)
                        FBC.OutwardCompanyMaterialStock = FBC.FacilityBooking.OutwardCompanyMaterialStock;
                    else
                        FBC.OutwardCompanyMaterialStock = FBC.FacilityBooking.OutwardCompanyMaterial.GetCompanyMaterialStock_InsertIfNotExists(BP.DatabaseApp);
                }
                else if (FBC.OutwardCompanyMaterial != null)
                    FBC.OutwardCompanyMaterialStock = FBC.OutwardCompanyMaterial.GetCompanyMaterialStock_InsertIfNotExists(BP.DatabaseApp);

                if (BP.ParamsAdjusted.CPartnerCompanyMaterial != null)
                {
                    if (FBC.FacilityBooking.OutwardCPartnerCompMatStock != null)
                        FBC.OutwardCPartnerCompMatStock = FBC.FacilityBooking.OutwardCPartnerCompMatStock;
                    else
                        FBC.OutwardCPartnerCompMatStock = BP.ParamsAdjusted.CPartnerCompanyMaterial.GetCompanyMaterialStock_InsertIfNotExists(BP.DatabaseApp);
                }
                else if (FBC.OutwardCPartnerCompMat != null)
                    FBC.OutwardCPartnerCompMatStock = FBC.OutwardCPartnerCompMat.GetCompanyMaterialStock_InsertIfNotExists(BP.DatabaseApp);

            }

            try
            {
                if (BP.ParamsAdjusted.MDZeroStockState.ZeroStockState != MDZeroStockState.ZeroStockStates.Off)
                {
                    BP.AddBookingMessage(ACMethodBooking.eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(this, "Error00118"));
                    return Global.ACMethodResultState.Notpossible;
                }
                else if (QuantityIsAbsolute == true)
                {
                    Double floatVal = 0;
                    if (OutwardQuantity.HasValue)
                    {
                        floatVal = 0;
                        if (ConvertQuantityToMaterialBaseUnit(BP, OutwardQuantity, mdQuantityUnit, FBC.OutwardMaterial, ref floatVal) == Global.ACMethodResultState.Failed)
                            return Global.ACMethodResultState.Failed;
                        FBC.OutwardQuantityUOM = floatVal - FBC.InwardFacilityCharge.StockQuantityUOM;
                        FBC.OutwardQuantityUOMAmb = (floatVal * BP.FactorOutwardAmbient) - FBC.InwardFacilityCharge.StockQuantityUOMAmb;

                        floatVal = 0;
                        if (ConvertQuantityInFacilityChargeUnit(BP, FBC.OutwardQuantityUOM, FBC.OutwardMaterial.BaseMDUnit, FBC.OutwardFacilityCharge, ref floatVal) == Global.ACMethodResultState.Failed)
                            return Global.ACMethodResultState.Failed;
                        if (OutwardFacilityCharge.MDUnit.Rounding >= 0)
                            FBC.OutwardQuantity = Math.Round(floatVal, OutwardFacilityCharge.MDUnit.Rounding);
                        else
                            FBC.OutwardQuantity = floatVal;
                    }

                    if (OutwardTargetQuantity.HasValue)
                    {
                        floatVal = 0;
                        if (ConvertQuantityToMaterialBaseUnit(BP, OutwardTargetQuantity, mdQuantityUnit, FBC.OutwardMaterial, ref floatVal) == Global.ACMethodResultState.Failed)
                            return Global.ACMethodResultState.Failed;
                        FBC.OutwardTargetQuantityUOM = floatVal;
                        FBC.OutwardTargetQuantityUOMAmb = floatVal * BP.FactorOutwardTargetAmbient;

                        floatVal = 0;
                        if (ConvertQuantityInFacilityChargeUnit(BP, FBC.OutwardTargetQuantityUOM, FBC.OutwardMaterial.BaseMDUnit, FBC.OutwardFacilityCharge, ref floatVal) == Global.ACMethodResultState.Failed)
                            return Global.ACMethodResultState.Failed;
                        if (OutwardFacilityCharge.MDUnit.Rounding >= 0)
                            FBC.OutwardTargetQuantity = Math.Round(floatVal, OutwardFacilityCharge.MDUnit.Rounding);
                        else
                            FBC.OutwardTargetQuantity = floatVal;

                    }
                }
                else
                {
                    Double floatVal = 0;
                    if (OutwardQuantity.HasValue)
                    {
                        floatVal = 0;
                        if (ConvertQuantityToMaterialBaseUnit(BP, OutwardQuantity, mdQuantityUnit, FBC.OutwardMaterial, ref floatVal) == Global.ACMethodResultState.Failed)
                            return Global.ACMethodResultState.Failed;
                        FBC.OutwardQuantityUOM = floatVal;
                        FBC.OutwardQuantityUOMAmb = floatVal * BP.FactorOutwardAmbient;

                        floatVal = 0;
                        if (ConvertQuantityInFacilityChargeUnit(BP, FBC.OutwardQuantityUOM, FBC.OutwardMaterial.BaseMDUnit, FBC.OutwardFacilityCharge, ref floatVal) == Global.ACMethodResultState.Failed)
                            return Global.ACMethodResultState.Failed;
                        if (OutwardFacilityCharge.MDUnit.Rounding >= 0)
                            FBC.OutwardQuantity = Math.Round(floatVal, OutwardFacilityCharge.MDUnit.Rounding);
                        else
                            FBC.OutwardQuantity = floatVal;
                    }

                    if (OutwardTargetQuantity.HasValue)
                    {
                        floatVal = 0;
                        if (ConvertQuantityToMaterialBaseUnit(BP, OutwardTargetQuantity, mdQuantityUnit, FBC.OutwardMaterial, ref floatVal) == Global.ACMethodResultState.Failed)
                            return Global.ACMethodResultState.Failed;
                        FBC.OutwardTargetQuantityUOM = floatVal;
                        FBC.OutwardTargetQuantityUOMAmb = floatVal * BP.FactorOutwardTargetAmbient;

                        floatVal = 0;
                        if (ConvertQuantityInFacilityChargeUnit(BP, FBC.OutwardTargetQuantityUOM, FBC.OutwardMaterial.BaseMDUnit, FBC.OutwardFacilityCharge, ref floatVal) == Global.ACMethodResultState.Failed)
                            return Global.ACMethodResultState.Failed;
                        if (OutwardFacilityCharge.MDUnit.Rounding >= 0)
                            FBC.OutwardTargetQuantity = Math.Round(floatVal, OutwardFacilityCharge.MDUnit.Rounding);
                        else
                            FBC.OutwardTargetQuantity = floatVal;
                    }
                }
            }
            catch (Exception e)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.QuantityConversionError, e.Message);
                return Global.ACMethodResultState.Failed;
            }

            return Global.ACMethodResultState.Succeeded;
        }
        
        #endregion
        #endregion
    }
}
