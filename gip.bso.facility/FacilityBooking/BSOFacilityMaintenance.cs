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
// <copyright file="BSOFacilityMaintenance.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.mes.datamodel; using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.facility;
using gip.mes.autocomponent;

namespace gip.bso.facility
{
    /// <summary>
    /// Businessobject or App for maintenance functions on stock records (FacilityCharge), e.g. quantity/weight adjustments and stock matching.
    /// To search for records, enter the search string in the SearchWord property.
    /// The database result is copied to the FacilityChargeList property.
    /// Then call the NavigateFirst() method to set CurrentFacilityCharge with the first record in the list.
    /// CurrentFacilityCharge is used to display and edit the currently selected record.
    /// This BSO is booking-parameter driven: the CurrentBookingParameter property (ACMethodBooking) holds the posting parameters (InwardMaterial, InwardFacilityLocation, InwardFacility, InwardFacilityCharge, quantities, ...) which are edited in the "Charge" group of the screen.
    /// The New() method clears the booking parameters (ClearBookingData) and sets the state to "New"; it is called automatically on ACInit and does not create a database record.
    /// Each command method (MatchingWeightWithQuantityAll, MatchingWeightWithQuantityMaterial, MatchingStockAll, MatchingStockFacility, MatchingMaterialInwardFacilityChargeAll, MatchingStockMaterialMaterial) executes a facility booking with the current booking parameters via ACFacilityManager.BookFacility().
    /// Each command has a corresponding IsEnabled...() method which returns the enabled state of the current booking parameters (CurrentBookingParameter.IsEnabled()).
    /// The Load method updates the CurrentFacilityCharge object with fresh database data if another user has made changes in the background.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityMaintenance&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Facilitymaintenance'}de{'Lagerwartungsfunktionen'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + FacilityCharge.ClassName,
        Description = @"Businessobject or App for maintenance functions on stock records (FacilityCharge), e.g. quantity/weight adjustments and stock matching.
        To search for records, enter the search string in the SearchWord property.
        The database result is copied to the FacilityChargeList property.
        Then call the NavigateFirst() method to set CurrentFacilityCharge with the first record in the list.
        CurrentFacilityCharge is used to display and edit the currently selected record.
        This BSO is booking-parameter driven: the CurrentBookingParameter property (ACMethodBooking) holds the posting parameters (InwardMaterial, InwardFacilityLocation, InwardFacility, InwardFacilityCharge, quantities, ...) which are edited in the 'Charge' group of the screen.
        The New() method clears the booking parameters (ClearBookingData) and sets the state to 'New'; it is called automatically on ACInit and does not create a database record.
        Each command method (MatchingWeightWithQuantityAll, MatchingWeightWithQuantityMaterial, MatchingStockAll, MatchingStockFacility, MatchingMaterialInwardFacilityChargeAll, MatchingStockMaterialMaterial) executes a facility booking with the current booking parameters via ACFacilityManager.BookFacility().
        Each command has a corresponding IsEnabled...() method which returns the enabled state of the current booking parameters (CurrentBookingParameter.IsEnabled()).
        The Load method updates the CurrentFacilityCharge object with fresh database data if another user has made changes in the background.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityMaintenance&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOFacilityMaintenance : BSOFacilityBase 
    {
        /// <summary>
        /// The _ booking parameter
        /// </summary>
        ACMethodBooking _BookingParameter;

        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityMaintenance"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityMaintenance(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
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
            _BookingParameter = ACFacilityManager.ACUrlACTypeSignature("!MatchingFacilityChargeQuantitiesAll", gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            
            New();
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
        [ACMethodInfo("xxx", "en{'Activate'}de{'Aktiviere'}", 791, true, Global.ACKinds.MSMethodPrePost, Description =
            "Called by ACAction when a tab item is activated. Runs PreExecute/PostExecute for the page activation.")]
        public void OnActivate(string page)
        {
            if (!PreExecute("OnActivate"))
                return;
            PostExecute("OnActivate");

        }

        #region BSO->ACProperty

        /// <summary>
        /// Gets the booking parameters (ACMethodBooking) for the maintenance bookings.
        /// The parameters (material, storage bin location, storage bin, charge, quantities) are edited on the screen and executed by one of the matching command methods.
        /// </summary>
        /// <value>The current booking parameter.</value>
        [ACPropertyCurrent(701, "BookingParameter", Description =
            "Gets the booking parameters (ACMethodBooking) for the maintenance bookings. The parameters (material, storage bin location, storage bin, charge, quantities) are edited on the screen and executed by one of the Matching... command methods.")]
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
            "Clears the booking parameters (ClearBookingData) and sets the BSO state to 'New'. Does not create a database record; the booking parameters are filled afterwards and executed by one of the matching commands.")]
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

        #region Abgleich (MatchingWeightWithQuantityAll, MatchingWeightWithQuantityMaterial, MatchingStockAll,MatchingStockFacility, MatchingStockMaterialAll, MatchingStockMaterialMaterial)
        /// <summary>
        /// Runs the complete adjustment of quantities with weights for all charges.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Completeadjustment Quantity with Weight'}de{'Gesamtabgleich Menge mit Gewicht'}", 701, true, Description =
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
        [ACMethodCommand("BookingParameter", "en{'Adjustment Quantity with Weight'}de{'Abgleich Menge mit Gewicht'}", 702, true, Description =
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
        [ACMethodCommand("BookingParameter", "en{'Adjustment all Facilitylocation'}de{'Abgleich alle Lagerplätze'}", 703, true, Description =
            "Runs the stock adjustment for all storage bins (booking type MatchingFacilityInwardFacilityChargeAll) via ACFacilityManager.BookFacility().")]
        public void MatchingStockAll()
        {
            //TODO: CurrentBookingParameter.BookingType = ACFacilityManager.DefaultMDFacilityBookingType(MDFacilityBookingType.FacitlityBookingType.MatchingFacilityInwardFacilityChargeAll);
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
        [ACMethodCommand("BookingParameter", "en{'Adjustment definite Facilitylocation'}de{'Abgleich bestimmter Lagerplatz'}", 704, true, Description =
            "Runs the stock adjustment for the definite storage bin selected in the booking parameters (booking type MatchingFacilityInwardFacilityChargeOneFacility) via ACFacilityManager.BookFacility().")]
        public void MatchingStockFacility()
        {
            //TODO: CurrentBookingParameter.BookingType = ACFacilityManager.DefaultMDFacilityBookingType(MDFacilityBookingType.FacitlityBookingType.MatchingFacilityInwardFacilityChargeOneFacility);
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
        [ACMethodCommand("BookingParameter", "en{'Adjustment Stock'}de{'Abgleich Bestände'}", 705, true, Description =
            "Runs the stock adjustment for all materials (booking type MatchingMaterialInwardFacilityChargeAll) via ACFacilityManager.BookFacility().")]
        public void MatchingMaterialInwardFacilityChargeAll()
        {
            //TODO: CurrentBookingParameter.BookingType = ACFacilityManager.DefaultMDFacilityBookingType(MDFacilityBookingType.FacitlityBookingType.MatchingMaterialInwardFacilityChargeAll);
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
        [ACMethodCommand("BookingParameter", "en{'Adjustment Stock definite Material'}de{'Abgleich Bestände bestimmtes Material'}", 706, true, Description =
            "Runs the stock adjustment for the definite material selected in the booking parameters (booking type MatchingMaterialInwardFacilityChargeOneMaterial) via ACFacilityManager.BookFacility().")]
        public void MatchingStockMaterialMaterial()
        {
            //TODO: CurrentBookingParameter.BookingType = ACFacilityManager.DefaultMDFacilityBookingType(MDFacilityBookingType.FacitlityBookingType.MatchingMaterialInwardFacilityChargeOneMaterial);
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
        #endregion

        #region Eventhandling
        /// <summary>
        /// BSOs the facility maintenance_ on activate page.
        /// </summary>
        /// <param name="acObject">The ac object.</param>
        void BSOFacilityMaintenance_OnActivatePage(IACObject acObject)
        {

        }
        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case"OnActivate":
                    OnActivate((String)acParameter[0]);
                    return true;
                case"Search":
                    Search();
                    return true;
                case"New":
                    New();
                    return true;
                case"IsEnabledNew":
                    result = IsEnabledNew();
                    return true;
                case"MatchingWeightWithQuantityAll":
                    MatchingWeightWithQuantityAll();
                    return true;
                case"IsEnabledMatchingWeightWithQuantityAll":
                    result = IsEnabledMatchingWeightWithQuantityAll();
                    return true;
                case"MatchingWeightWithQuantityMaterial":
                    MatchingWeightWithQuantityMaterial();
                    return true;
                case"IsEnabledMatchingWeightWithQuantityMaterial":
                    result = IsEnabledMatchingWeightWithQuantityMaterial();
                    return true;
                case"MatchingStockAll":
                    MatchingStockAll();
                    return true;
                case"IsEnabledMatchingStockAll":
                    result = IsEnabledMatchingStockAll();
                    return true;
                case"MatchingStockFacility":
                    MatchingStockFacility();
                    return true;
                case"IsEnabledMatchingStockFacility":
                    result = IsEnabledMatchingStockFacility();
                    return true;
                case"MatchingMaterialInwardFacilityChargeAll":
                    MatchingMaterialInwardFacilityChargeAll();
                    return true;
                case"IsEnabledMatchingMaterialInwardFacilityChargeAll":
                    result = IsEnabledMatchingMaterialInwardFacilityChargeAll();
                    return true;
                case"MatchingStockMaterialMaterial":
                    MatchingStockMaterialMaterial();
                    return true;
                case"IsEnabledMatchingStockMaterialMaterial":
                    result = IsEnabledMatchingStockMaterialMaterial();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(OnActivate):
                case nameof(Search):
                case nameof(New):
                    return new[] { "CurrentBookingParameter" };
                case nameof(MatchingWeightWithQuantityAll):
                case nameof(MatchingWeightWithQuantityMaterial):
                case nameof(MatchingStockAll):
                case nameof(MatchingStockFacility):
                case nameof(MatchingMaterialInwardFacilityChargeAll):
                case nameof(MatchingStockMaterialMaterial):
                    return new[] { "CurrentBookingParameter" };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion


    }

}
