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
// <copyright file="BSOFacilityChangeUnit.cs" company="gip mbh, Oftersheim, Germany">
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
    /// Businessobject or App for changing the storage unit (unit of measure) of existing stock charges (FacilityCharge).
    /// To search for records, enter the search string in the SearchWord property.
    /// The database result is copied to the MaterialList property.
    /// Then call the NavigateFirst() method to set CurrentMaterial with the first record in the list.
    /// CurrentMaterial is used to display and edit the currently selected record.
    /// This BSO is booking-parameter driven: the CurrentBookingParameter property (ACMethodBooking) holds the posting parameters (e.g. InwardMaterial) which are edited in the "Material" group of the screen.
    /// The New() method clears the booking parameters (ClearBookingData) and sets the state to "New"; it is called automatically on ACInit and does not create a database record.
    /// The ChangeUnit() method executes the "Change storage unit" booking with the current booking parameters via ACFacilityManager.BookFacility().
    /// The IsEnabledChangeUnit() method returns the enabled state of the current booking parameters (CurrentBookingParameter.IsEnabled()).
    /// The Load method updates the CurrentMaterial object with fresh database data if another user has made changes in the background.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityChangeUnit&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Change Facilityunit'}de{'Lagereinheiten ändern'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Material.ClassName,
        Description = @"Businessobject or App for changing the storage unit (unit of measure) of existing stock charges (FacilityCharge).
        To search for records, enter the search string in the SearchWord property.
        The database result is copied to the MaterialList property.
        Then call the NavigateFirst() method to set CurrentMaterial with the first record in the list.
        CurrentMaterial is used to display and edit the currently selected record.
        This BSO is booking-parameter driven: the CurrentBookingParameter property (ACMethodBooking) holds the posting parameters (e.g. InwardMaterial) which are edited in the 'Material' group of the screen.
        The New() method clears the booking parameters (ClearBookingData) and sets the state to 'New'; it is called automatically on ACInit and does not create a database record.
        The ChangeUnit() method executes the 'Change storage unit' booking with the current booking parameters via ACFacilityManager.BookFacility().
        The IsEnabledChangeUnit() method returns the enabled state of the current booking parameters (CurrentBookingParameter.IsEnabled()).
        The Load method updates the CurrentMaterial object with fresh database data if another user has made changes in the background.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityChangeUnit&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOFacilityChangeUnit : BSOFacilityBase 
    {
        /// <summary>
        /// The _ booking parameter
        /// </summary>
        ACMethodBooking _BookingParameter;

        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityChangeUnit"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityChangeUnit(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
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
            _BookingParameter = ACFacilityManager.ACUrlACTypeSignature("!ChangeStorageUnit", gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 

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

        #region BSO->ACProperty

        /// <summary>
        /// Gets the booking parameters (ACMethodBooking) for the "Change storage unit" booking.
        /// The parameters (e.g. the material) are edited on the screen and executed by the ChangeUnit() method.
        /// </summary>
        /// <value>The current booking parameter.</value>
        [ACPropertyCurrent(701, "BookingParameter", Description =
            "Gets the booking parameters (ACMethodBooking) for the 'Change storage unit' booking. The parameters (e.g. the material) are edited on the screen and executed by the ChangeUnit() method.")]
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
        ACAccessNav<Material> _AccessPrimary;
        /// <summary>
        /// Gets the primary navigation access for Material records.
        /// The SearchWord filters the result; MaterialList, SelectedMaterial and CurrentMaterial are served by this access.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(790, Material.ClassName, Description =
            "Gets the primary navigation access for Material records. The SearchWord filters the result; MaterialList, SelectedMaterial and CurrentMaterial are served by this access.")]
        public ACAccessNav<Material> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<Material>(Material.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the Material currently selected in the result list.
        /// </summary>
        /// <value>The selected material.</value>
        [ACPropertySelected(702, Material.ClassName, Description =
            "Gets or sets the Material currently selected in the result list.")]
        public Material SelectedMaterial
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
                OnPropertyChanged("SelectedMaterial");
            }
        }

        /// <summary>
        /// Gets or sets the current Material record. Edit this record and call Save() to persist changes.
        /// </summary>
        /// <value>The current material.</value>
        [ACPropertyCurrent(703, Material.ClassName, Description =
            "Gets or sets the current Material record. Edit this record and call Save() to persist changes before navigating to the next record.")]
        public Material CurrentMaterial
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
        /// Gets the list of Material records resulting from the last search.
        /// </summary>
        /// <value>The material list.</value>
        [ACPropertyList(704, Material.ClassName, Description =
            "Gets the list of Material records resulting from the last search (SearchWord + Search()).")]
        public IEnumerable<Material> MaterialList
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
        /// Executes the search with the current SearchWord and refreshes the MaterialList.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description =
            "Executes the search with the current SearchWord and refreshes the MaterialList.")]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("MaterialList");
        }

        /// <summary>
        /// Clears the booking parameters (ClearBookingData) and sets the BSO state to "New". Does not create a database record.
        /// </summary>
        [ACMethodCommand("BookingParameter", Const.New, (short)MISort.New, true, Global.ACKinds.MSMethodPrePost, Description =
            "Clears the booking parameters (ClearBookingData) and sets the BSO state to 'New'. Does not create a database record; the booking parameters are filled afterwards and executed by the ChangeUnit() command.")]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentBookingParameter.ClearBookingData();
            ACState = Const.SMNew;
            PostExecute("New");
           
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
        [ACMethodCommand("BookingParameter", "en{'Change Unit'}de{'Einheit ändern'}", 701, true, Global.ACKinds.MSMethodPrePost, Description =
            "Executes the 'Change storage unit' booking with the current booking parameters via ACFacilityManager.BookFacility(). Converts the quantity of the selected charge into another storage unit.")]
        public void ChangeUnit()
        {
            if (!PreExecute("ChangeUnit")) return;
            //TODO: CurrentBookingParameter.BookingType = ACFacilityManager.DefaultMDFacilityBookingType(MDFacilityBookingType.FacitlityBookingType.ChangeStorageUnit);
            ACFacilityManager.BookFacility(CurrentBookingParameter, this.DatabaseApp);
            PostExecute("ChangeUnit");
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
                case"Search":
                    Search();
                    return true;
                case"New":
                    New();
                    return true;
                case"IsEnabledNew":
                    result = IsEnabledNew();
                    return true;
                case"ChangeUnit":
                    ChangeUnit();
                    return true;
                case"IsEnabledChangeUnit":
                    result = IsEnabledChangeUnit();
                    return true;
                case"OnActivate":
                    OnActivate((String)acParameter[0]);
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(New):
                case "Save":
                case "Load":
                case "Delete":
                case nameof(Search):
                    return new[] { "CurrentFacilityChangeUnit", "FilterMaterial" };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion

    }

}
