// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
// ***********************************************************************
// Assembly         : gip.bso.facility
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 11-08-2012
// ***********************************************************************
// <copyright file="BSOFacilityCorrection.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.facility
{
    /// <summary>
    /// Businessobject or App for correcting stock records (FacilityCharge), e.g. stock corrections, zero-stock postings, quantity/weight adjustments, stock matching and cost rate corrections.
    /// To search for records, enter the search string in the SearchWord property.
    /// The database result is copied to the FacilityChargeList property.
    /// Then call the NavigateFirst() method to set CurrentFacilityCharge with the first record in the list.
    /// CurrentFacilityCharge is used to display and edit the currently selected record.
    /// This BSO is booking-parameter driven: the CurrentBookingParameter property (ACMethodBooking) holds the posting parameters (InwardMaterial, InwardFacilityLocation, InwardFacility, InwardFacilityCharge, quantities, ...) which are edited in the "Quant" group of the screen.
    /// The New() method clears the booking parameters (ClearBookingData) and sets the state to "New"; it does not create a database record.
    /// Each command method (FacilityStockCorrection, MaterialZero, FacilityZero, ChangeUnit, MatchingWeightWithQuantityAll, MatchingWeightWithQuantityMaterial, MatchingStockAll, MatchingStockFacility, MatchingMaterialInwardFacilityChargeAll, MatchingStockMaterialMaterial, CorrectionCostRateAll, CorrectionCostRateMaterial) executes a facility booking with the current booking parameters via ACFacilityManager.BookFacility().
    /// Each command has a corresponding IsEnabled...() method which returns the enabled state of the current booking parameters (CurrentBookingParameter.IsEnabled()).
    /// NewChargeNo() and NewSplitChargeNo() are placeholders without implementation.
    /// The Load method updates the CurrentFacilityCharge object with fresh database data if another user has made changes in the background.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityCorrection&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Stock Correction'}de{'Bestandskorrektur'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + FacilityCharge.ClassName,
        Description = @"Businessobject or App for correcting stock records (FacilityCharge), e.g. stock corrections, zero-stock postings, quantity/weight adjustments, stock matching and cost rate corrections.
        To search for records, enter the search string in the SearchWord property.
        The database result is copied to the FacilityChargeList property.
        Then call the NavigateFirst() method to set CurrentFacilityCharge with the first record in the list.
        CurrentFacilityCharge is used to display and edit the currently selected record.
        This BSO is booking-parameter driven: the CurrentBookingParameter property (ACMethodBooking) holds the posting parameters (InwardMaterial, InwardFacilityLocation, InwardFacility, InwardFacilityCharge, quantities, ...) which are edited in the 'Quant' group of the screen.
        The New() method clears the booking parameters (ClearBookingData) and sets the state to 'New'; it does not create a database record.
        Each command method (FacilityStockCorrection, MaterialZero, FacilityZero, ChangeUnit, MatchingWeightWithQuantityAll, MatchingWeightWithQuantityMaterial, MatchingStockAll, MatchingStockFacility, MatchingMaterialInwardFacilityChargeAll, MatchingStockMaterialMaterial, CorrectionCostRateAll, CorrectionCostRateMaterial) executes a facility booking with the current booking parameters via ACFacilityManager.BookFacility().
        Each command has a corresponding IsEnabled...() method which returns the enabled state of the current booking parameters (CurrentBookingParameter.IsEnabled()).
        NewChargeNo() and NewSplitChargeNo() are placeholders without implementation.
        The Load method updates the CurrentFacilityCharge object with fresh database data if another user has made changes in the background.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityCorrection&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOFacilityCorrection : BSOFacilityBase
    {
        /// <summary>
        /// The _ booking parameter
        /// </summary>
        ACMethodBooking _BookingParameter;

        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityCorrection"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityCorrection(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
        }

        /// <summary>
        /// ACs the init.
        /// </summary>
        /// <param name="startChildMode">The start child mode.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            _BookingParameter = ACFacilityManager.ACUrlACTypeSignature("!StockCorrection", gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            this._BookingParameter = null;
            var b = await base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                await _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }

        #endregion

        #region BSO->ACProperty

        /// <summary>
        /// Gets the booking parameters (ACMethodBooking) for the stock correction posting.
        /// The parameters (material, storage bin location, storage bin, charge, quantities) are edited on the screen and executed by one of the booking command methods.
        /// </summary>
        /// <value>The current booking parameter.</value>
        [ACPropertyCurrent(701, "BookingParameter", Description =
            "Gets the booking parameters (ACMethodBooking) for the stock correction posting. The parameters (material, storage bin location, storage bin, charge, quantities) are edited on the screen and executed by one of the booking command methods (FacilityStockCorrection, MaterialZero, FacilityZero, ChangeUnit, Matching..., CorrectionCostRate...).")]
        public ACMethodBooking CurrentBookingParameter
        {
            get
            {
                return _BookingParameter;
            }
        }

        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<FacilityCharge> _AccessPrimary;
        /// <summary>
        /// Gets the primary navigation access for FacilityCharge records.
        /// The SearchWord filters the result; FacilityChargeList, SelectedFacilityCharge and CurrentFacilityCharge are served by this access.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(790, FacilityCharge.ClassName, Description =
            "Gets the primary navigation access for FacilityCharge records. The SearchWord filters the result; FacilityChargeList, SelectedFacilityCharge and CurrentFacilityCharge are served by this access.")]
        public ACAccessNav<FacilityCharge> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<FacilityCharge>(FacilityCharge.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the FacilityCharge currently selected in the result list.
        /// </summary>
        /// <value>The selected facility charge.</value>
        [ACPropertySelected(702, FacilityCharge.ClassName, Description =
            "Gets or sets the FacilityCharge currently selected in the result list.")]
        public FacilityCharge SelectedFacilityCharge
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedFacilityCharge");
            }
        }

        /// <summary>
        /// Gets or sets the current FacilityCharge record. Edit this record and call Save() to persist changes.
        /// </summary>
        /// <value>The current facility charge.</value>
        [ACPropertyCurrent(703, FacilityCharge.ClassName, Description =
            "Gets or sets the current FacilityCharge record. Edit this record and call Save() to persist changes before navigating to the next record.")]
        public FacilityCharge CurrentFacilityCharge
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                AccessPrimary.Current = value;
            }
        }

        /// <summary>
        /// Gets the list of FacilityCharge records resulting from the last search.
        /// </summary>
        /// <value>The facility charge list.</value>
        [ACPropertyList(704, FacilityCharge.ClassName, Description =
            "Gets the list of FacilityCharge records resulting from the last search (SearchWord + Search()).")]
        public IEnumerable<FacilityCharge> FacilityChargeList
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.NavList;
            }
        }
        #endregion

        #region BSO->ACMethod
        #region Allgemein
        /// <summary>
        /// Executes the search with the current SearchWord and refreshes the FacilityChargeList.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description =
            "Executes the search with the current SearchWord and refreshes the FacilityChargeList.")]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("FacilityChargeList");
        }

        /// <summary>
        /// Clears the booking parameters (ClearBookingData) and sets the BSO state to "New". Does not create a database record.
        /// </summary>
        [ACMethodCommand("BookingParameter", Const.New, (short)MISort.New, true, Description =
            "Clears the booking parameters (ClearBookingData) and sets the BSO state to 'New'. Does not create a database record; the booking parameters are filled afterwards and executed by one of the booking commands.")]
        public void New()
        {
            CurrentBookingParameter.ClearBookingData();
            ACState = Const.SMNew;

        }

        /// <summary>
        /// Determines whether [is enabled new].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNew()
        {
            return true;
        }
        #endregion

        #region Einheit ändern (ChangeStorageUnit)
        /// <summary>
        /// Executes the "Change storage unit" booking with the current booking parameters.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Change Unit'}de{'Einheit ändern'}", 701, true, Description =
            "Executes the 'Change storage unit' booking with the current booking parameters via ACFacilityManager.BookFacility(). Converts the quantity of the selected charge into another storage unit.")]
        public void ChangeUnit()
        {
            //TODO: CurrentBookingParameter.BookingType = ACFacilityManager.DefaultMDFacilityBookingType(MDFacilityBookingType.FacitlityBookingType.ChangeStorageUnit);
            ACFacilityManager.BookFacility(CurrentBookingParameter, this.DatabaseApp);
        }

        /// <summary>
        /// Determines whether [is enabled change unit].
        /// </summary>
        /// <returns><c>true</c> if [is enabled change unit]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledChangeUnit()
        {
            return CurrentBookingParameter.IsEnabled();
        }
        #endregion


        #region Buchung Korrektur (StockCorrectionGeneral, StockCorrectionAdulterated,StockCorrectionStockout,StockCorrectionZeroStockMaterial,StockCorrectionZeroStockFacility  
        /// <summary>
        /// Executes a stock correction booking with the current booking parameters.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Stock Correction'}de{'Bestandskorrektur'}", 702, true, Description =
            "Executes a stock correction booking with the current booking parameters via ACFacilityManager.BookFacility(). Corrects the stock of the selected charge/material to the entered target quantity.")]
        public void FacilityStockCorrection()
        {
            //TODO: CurrentBookingParameter.BookingType = ACFacilityManager.DefaultMDFacilityBookingType(MDFacilityBookingType.FacitlityBookingType.StockCorrection);
            ACFacilityManager.BookFacility(CurrentBookingParameter, this.DatabaseApp);
        }
        /// <summary>
        /// Determines whether [is enabled facility stock correction].
        /// </summary>
        /// <returns><c>true</c> if [is enabled facility stock correction]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledFacilityStockCorrection()
        {
            return CurrentBookingParameter.IsEnabled();
        }

        /// <summary>
        /// Books the stock of the selected material to zero.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Material Stock = 0'}de{'Materialbestand = 0'}", 703, true, Description =
            "Books the stock of the selected material to zero (booking type ZeroStock_Facility_BulkMaterial) via ACFacilityManager.BookFacility(). All charges of this material are set to zero stock.")]
        public void MaterialZero()
        {
            //TODO: CurrentBookingParameter.BookingType = ACFacilityManager.DefaultMDFacilityBookingType(MDFacilityBookingType.FacitlityBookingType.ZeroStock_Facility_BulkMaterial);
            ACFacilityManager.BookFacility(CurrentBookingParameter, this.DatabaseApp);
        }
        /// <summary>
        /// Determines whether [is enabled material zero].
        /// </summary>
        /// <returns><c>true</c> if [is enabled material zero]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledMaterialZero()
        {
            return CurrentBookingParameter.IsEnabled();
        }

        /// <summary>
        /// Books the stock of the selected storage bin charge to zero.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Storage Bin Stock = 0'}de{'Lagerplatzbestand = 0'}", 704, true, Description =
            "Books the stock of the selected storage bin charge to zero (booking type ZeroStock_FacilityCharge) via ACFacilityManager.BookFacility().")]
        public void FacilityZero()
        {
            //TODO: CurrentBookingParameter.BookingType = ACFacilityManager.DefaultMDFacilityBookingType(MDFacilityBookingType.FacitlityBookingType.ZeroStock_FacilityCharge);
            ACFacilityManager.BookFacility(CurrentBookingParameter, this.DatabaseApp);
        }
        /// <summary>
        /// Determines whether [is enabled facility zero].
        /// </summary>
        /// <returns><c>true</c> if [is enabled facility zero]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledFacilityZero()
        {
            return CurrentBookingParameter.IsEnabled();
        }
        #endregion

        #region Abgleich (MatchingWeightWithQuantityAll, MatchingWeightWithQuantityMaterial, MatchingStockAll,MatchingStockFacility, MatchingStockMaterialAll, MatchingStockMaterialMaterial)
        /// <summary>
        /// Runs the complete adjustment of quantities with weights for all charges.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Completeadjustment Quantity with Weight'}de{'Gesamtabgleich Menge mit Gewicht'}", 705, true, Description =
            "Runs the complete adjustment of quantities with weights for all charges (booking type MatchingFacilityChargeQuantitiesAll) via ACFacilityManager.BookFacility().")]
        public void MatchingWeightWithQuantityAll()
        {
            //TODO: CurrentBookingParameter.BookingType = ACFacilityManager.DefaultMDFacilityBookingType(MDFacilityBookingType.FacitlityBookingType.MatchingFacilityChargeQuantitiesAll);
            ACFacilityManager.BookFacility(CurrentBookingParameter, this.DatabaseApp);
        }

        /// <summary>
        /// Determines whether [is enabled matching weight with quantity all].
        /// </summary>
        /// <returns><c>true</c> if [is enabled matching weight with quantity all]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledMatchingWeightWithQuantityAll()
        {
            return CurrentBookingParameter.IsEnabled();
        }

        /// <summary>
        /// Runs the quantity/weight adjustment for the selected material.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Completeadjustment Quantity with Weight Material'}de{'Abgleich Menge mit Gewicht Material'}", 706, true, Description =
            "Runs the quantity/weight adjustment for the selected material (booking type MatchingFacilityChargeQuantites) via ACFacilityManager.BookFacility().")]
        public void MatchingWeightWithQuantityMaterial()
        {
            //TODO: CurrentBookingParameter.BookingType = ACFacilityManager.DefaultMDFacilityBookingType(MDFacilityBookingType.FacitlityBookingType.MatchingFacilityChargeQuantites);
            ACFacilityManager.BookFacility(CurrentBookingParameter, this.DatabaseApp);
        }

        /// <summary>
        /// Determines whether [is enabled matching weight with quantity material].
        /// </summary>
        /// <returns><c>true</c> if [is enabled matching weight with quantity material]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledMatchingWeightWithQuantityMaterial()
        {
            return CurrentBookingParameter.IsEnabled();
        }

        /// <summary>
        /// Runs the stock adjustment for all storage bins.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Adjustment Stock all Facilitylocation'}de{'Abgleich Bestände für alle Lagerplätze'}", 707, true, Description =
            "Runs the stock adjustment for all storage bins (booking type MatchingFacilityStockAll) via ACFacilityManager.BookFacility().")]
        public void MatchingStockAll()
        {
            //TODO: CurrentBookingParameter.BookingType = ACFacilityManager.DefaultMDFacilityBookingType(MDFacilityBookingType.FacitlityBookingType.MatchingFacilityStockAll);
            ACFacilityManager.BookFacility(CurrentBookingParameter, this.DatabaseApp);
        }

        /// <summary>
        /// Determines whether [is enabled matching stock all].
        /// </summary>
        /// <returns><c>true</c> if [is enabled matching stock all]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledMatchingStockAll()
        {
            return CurrentBookingParameter.IsEnabled();
        }

        /// <summary>
        /// Runs the stock adjustment for a definite storage bin.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Adjustment Stock definite Facilitylocation'}de{'Abgleich Bestände für bestimmten Lagerplatz'}", 708, true, Description =
            "Runs the stock adjustment for the definite storage bin selected in the booking parameters (booking type MatchingFacilityStock) via ACFacilityManager.BookFacility().")]
        public void MatchingStockFacility()
        {
            //TODO: CurrentBookingParameter.BookingType = ACFacilityManager.DefaultMDFacilityBookingType(MDFacilityBookingType.FacitlityBookingType.MatchingFacilityStock);
            ACFacilityManager.BookFacility(CurrentBookingParameter, this.DatabaseApp);
        }

        /// <summary>
        /// Determines whether [is enabled matching stock facility].
        /// </summary>
        /// <returns><c>true</c> if [is enabled matching stock facility]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledMatchingStockFacility()
        {
            return CurrentBookingParameter.IsEnabled();
        }


        /// <summary>
        /// Runs the stock adjustment for all materials.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Adjustment Stock all Material'}de{'Abgleich Bestände für alle Material'}", 709, true, Description =
            "Runs the stock adjustment for all materials (booking type MatchingCostRateMaterialInwardFacilityChargeAll) via ACFacilityManager.BookFacility().")]
        public void MatchingMaterialInwardFacilityChargeAll()
        {
            //TODO: CurrentBookingParameter.BookingType = ACFacilityManager.DefaultMDFacilityBookingType(MDFacilityBookingType.FacitlityBookingType.MatchingCostRateMaterialInwardFacilityChargeAll);
            ACFacilityManager.BookFacility(CurrentBookingParameter, this.DatabaseApp);
        }

        /// <summary>
        /// Determines whether [is enabled matching material inward facility charge all].
        /// </summary>
        /// <returns><c>true</c> if [is enabled matching material inward facility charge all]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledMatchingMaterialInwardFacilityChargeAll()
        {
            return CurrentBookingParameter.IsEnabled();
        }

        /// <summary>
        /// Runs the stock adjustment for a definite material.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Adjustment Stock definite Material'}de{'Abgleich Bestände für bestimmtes Material'}", 710, true, Description =
            "Runs the stock adjustment for the definite material selected in the booking parameters (booking type MatchingCostRateMaterialInwardFacilityCharge) via ACFacilityManager.BookFacility().")]
        public void MatchingStockMaterialMaterial()
        {
            //TODO: CurrentBookingParameter.BookingType = ACFacilityManager.DefaultMDFacilityBookingType(MDFacilityBookingType.FacitlityBookingType.MatchingCostRateMaterialInwardFacilityCharge);
            ACFacilityManager.BookFacility(CurrentBookingParameter, this.DatabaseApp);
        }

        /// <summary>
        /// Determines whether [is enabled matching stock material material].
        /// </summary>
        /// <returns><c>true</c> if [is enabled matching stock material material]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledMatchingStockMaterialMaterial()
        {
            return CurrentBookingParameter.IsEnabled();
        }

        #endregion

        #region Kostensatz Korrektur (CorrectionCostRateAll, CorrectionCostRateMaterial)
        /// <summary>
        /// Corrects the cost rates of all charges.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Adjustment Costrate'}de{'Korrektur Kostensatz'}", 711, true, Description =
            "Corrects the cost rates of all charges (booking type CorrectionCostRateAll) via ACFacilityManager.BookFacility().")]
        public void CorrectionCostRateAll()
        {
            //TODO: CurrentBookingParameter.BookingType = ACFacilityManager.DefaultMDFacilityBookingType(MDFacilityBookingType.FacitlityBookingType.CorrectionCostRateAll);
            ACFacilityManager.BookFacility(CurrentBookingParameter, this.DatabaseApp);
        }

        /// <summary>
        /// Determines whether [is enabled correction cost rate all].
        /// </summary>
        /// <returns><c>true</c> if [is enabled correction cost rate all]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledCorrectionCostRateAll()
        {
            return CurrentBookingParameter.IsEnabled();
        }


        /// <summary>
        /// Corrects the cost rate of a definite material.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Adjustment Costrate definite Material'}de{'Korrektur Kostensatz für bestimmtes Material'}", 712, true, Description =
            "Corrects the cost rate of the definite material selected in the booking parameters (booking type CorrectionCostRateMaterial) via ACFacilityManager.BookFacility().")]
        public void CorrectionCostRateMaterial()
        {
            //TODO: CurrentBookingParameter.BookingType = ACFacilityManager.DefaultMDFacilityBookingType(MDFacilityBookingType.FacitlityBookingType.CorrectionCostRateMaterial);
            ACFacilityManager.BookFacility(CurrentBookingParameter, this.DatabaseApp);
        }

        /// <summary>
        /// Determines whether [is enabled correction cost rate material].
        /// </summary>
        /// <returns><c>true</c> if [is enabled correction cost rate material]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledCorrectionCostRateMaterial()
        {
            return CurrentBookingParameter.IsEnabled();
        }
        #endregion


        /// <summary>
        /// News the charge no.
        /// </summary>
        public void NewChargeNo()
        {
            // TODO:
        }

        /// <summary>
        /// Determines whether [is enabled new charge no].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new charge no]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewChargeNo()
        {
            return true;
        }

        /// <summary>
        /// News the split charge no.
        /// </summary>
        public void NewSplitChargeNo()
        {
            //CurrentBookingParameter.InwardFacilityCharge.FacilityLot.LotNo = ACFacilityManager.GenerateSplitChargeNo(CurrentBookingParameter.InwardFacilityCharge);
        }

        /// <summary>
        /// Determines whether [is enabled new split charge no].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new split charge no]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewSplitChargeNo()
        {
            return CurrentBookingParameter.InwardFacilityCharge != null;
        }

        #endregion

        #region Eventhandling
        /// <summary>
        /// ACAction is called when one IACInteractiveObject (Source) wants to inform another IACInteractiveObject (Target) about an relevant interaction-event.
        /// </summary>
        /// <param name="actionArgs">Information about the type of interaction and the source</param>
        public override void ACAction(ACActionArgs actionArgs)
        {
            if (actionArgs.ElementAction == Global.ElementActionType.TabItemActivated)
            {
                OnActivate(actionArgs.DropObject.VBContent);
            }
            else
                base.ACAction(actionArgs);
        }

        /// <summary>
        /// Called when a tab item is activated; runs Pre/PostExecute for the page activation.
        /// </summary>
        /// <param name="page">The page.</param>
        [ACMethodInfo("xxx", "en{'Activate'}de{'Aktiviere'}", 9999, true, Global.ACKinds.MSMethodPrePost, Description =
            "Called by ACAction when a tab item is activated. Runs PreExecute/PostExecute for the page activation.")]
        public void OnActivate(string page)
        {
            if (!PreExecute("OnActivate"))
                return;
            PostExecute("OnActivate");

        }
        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "Search":
                    Search();
                    return true;
                case "New":
                    New();
                    return true;
                case "IsEnabledNew":
                    result = IsEnabledNew();
                    return true;
                case "ChangeUnit":
                    ChangeUnit();
                    return true;
                case "IsEnabledChangeUnit":
                    result = IsEnabledChangeUnit();
                    return true;
                case "FacilityStockCorrection":
                    FacilityStockCorrection();
                    return true;
                case "IsEnabledFacilityStockCorrection":
                    result = IsEnabledFacilityStockCorrection();
                    return true;
                case "MaterialZero":
                    MaterialZero();
                    return true;
                case "IsEnabledMaterialZero":
                    result = IsEnabledMaterialZero();
                    return true;
                case "FacilityZero":
                    FacilityZero();
                    return true;
                case "IsEnabledFacilityZero":
                    result = IsEnabledFacilityZero();
                    return true;
                case "MatchingWeightWithQuantityAll":
                    MatchingWeightWithQuantityAll();
                    return true;
                case "IsEnabledMatchingWeightWithQuantityAll":
                    result = IsEnabledMatchingWeightWithQuantityAll();
                    return true;
                case "MatchingWeightWithQuantityMaterial":
                    MatchingWeightWithQuantityMaterial();
                    return true;
                case "IsEnabledMatchingWeightWithQuantityMaterial":
                    result = IsEnabledMatchingWeightWithQuantityMaterial();
                    return true;
                case "MatchingStockAll":
                    MatchingStockAll();
                    return true;
                case "IsEnabledMatchingStockAll":
                    result = IsEnabledMatchingStockAll();
                    return true;
                case "MatchingStockFacility":
                    MatchingStockFacility();
                    return true;
                case "IsEnabledMatchingStockFacility":
                    result = IsEnabledMatchingStockFacility();
                    return true;
                case "MatchingMaterialInwardFacilityChargeAll":
                    MatchingMaterialInwardFacilityChargeAll();
                    return true;
                case "IsEnabledMatchingMaterialInwardFacilityChargeAll":
                    result = IsEnabledMatchingMaterialInwardFacilityChargeAll();
                    return true;
                case "MatchingStockMaterialMaterial":
                    MatchingStockMaterialMaterial();
                    return true;
                case "IsEnabledMatchingStockMaterialMaterial":
                    result = IsEnabledMatchingStockMaterialMaterial();
                    return true;
                case "CorrectionCostRateAll":
                    CorrectionCostRateAll();
                    return true;
                case "IsEnabledCorrectionCostRateAll":
                    result = IsEnabledCorrectionCostRateAll();
                    return true;
                case "CorrectionCostRateMaterial":
                    CorrectionCostRateMaterial();
                    return true;
                case "IsEnabledCorrectionCostRateMaterial":
                    result = IsEnabledCorrectionCostRateMaterial();
                    return true;
                case "IsEnabledNewChargeNo":
                    result = IsEnabledNewChargeNo();
                    return true;
                case "IsEnabledNewSplitChargeNo":
                    result = IsEnabledNewSplitChargeNo();
                    return true;
                case "OnActivate":
                    OnActivate((String)acParameter[0]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(FacilityStockCorrection):
                case nameof(MaterialZero):
                case nameof(FacilityZero):
                    return new[] { "CurrentBookingParameter" };
                case nameof(MatchingWeightWithQuantityAll):
                case nameof(MatchingWeightWithQuantityMaterial):
                case nameof(MatchingStockMaterialMaterial):
                case nameof(CorrectionCostRateAll):
                case nameof(CorrectionCostRateMaterial):
                    return new[] { "CurrentBookingParameter" };
                case nameof(NewChargeNo):
                    return new[] { "CurrentBookingParameter" };
                case nameof(NewSplitChargeNo):
                    return new[] { "CurrentBookingParameter" };
                case nameof(OnActivate):
                    return new[] { "CurrentBookingParameter" };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion

    }

}
