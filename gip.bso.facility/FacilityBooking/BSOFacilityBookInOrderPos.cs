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
// <copyright file="BSOFacilityBookInOrderPos.cs" company="gip mbh, Oftersheim, Germany">
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
    /// Businessobject or App for posting goods receipts (inward movements) and activating purchase order positions (InOrderPos) in the warehouse management.
    /// This BSO does not edit database records directly. It prepares booking parameters in the CurrentBookingParameter object (an ACMethodBooking) and executes them with the facility manager.
    /// On start the New() method is called automatically, which clears all booking parameters (CurrentBookingParameter.ClearBookingData()).
    /// To post an inward movement: select the purchase order in CurrentBookingParameter.InOrder, the order position in CurrentBookingParameter.InOrderPos and the material to be stored in CurrentBookingParameter.InwardMaterial.
    /// Then enter the quantity in CurrentBookingParameter.OutwardQuantity and the weight in CurrentBookingParameter.OutwardWeight and select the target storage location in CurrentBookingParameter.OutwardFacilityLocation, the storage bin in CurrentBookingParameter.OutwardFacility and the storage charge in CurrentBookingParameter.OutwardFacilityCharge.
    /// The release state of the position can be set with CurrentBookingParameter.ReleaseState.
    /// Then call the InOrderPosInwardMovement() method to post the goods receipt. The booking is executed by ACFacilityManager.BookFacility(CurrentBookingParameter).
    /// To activate (release) a purchase order position, call the InOrderPosActivate() method on the Activation tab.
    /// To search for purchase order positions call the Search() method. The database result is copied to the InOrderPosList property.
    /// Then call the NavigateFirst() method to set CurrentInOrderPos with the first record in the list. SelectedInOrderPos is bound to the result grid of the explorer.
    /// There are no Save(), Delete() or Load() methods in this BSO, because bookings are executed immediately and are not stored as editable records.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityBookInOrderPos&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Inorderfacility'}de{'Bestellbestandsführung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + InOrderPos.ClassName,
        Description = @"Businessobject or App for posting goods receipts (inward movements) and activating purchase order positions (InOrderPos) in the warehouse management.
        This BSO does not edit database records directly. It prepares booking parameters in the CurrentBookingParameter object (an ACMethodBooking) and executes them with the facility manager.
        On start the New() method is called automatically, which clears all booking parameters (CurrentBookingParameter.ClearBookingData()).
        To post an inward movement: select the purchase order in CurrentBookingParameter.InOrder, the order position in CurrentBookingParameter.InOrderPos and the material to be stored in CurrentBookingParameter.InwardMaterial.
        Then enter the quantity in CurrentBookingParameter.OutwardQuantity and the weight in CurrentBookingParameter.OutwardWeight and select the target storage location in CurrentBookingParameter.OutwardFacilityLocation, the storage bin in CurrentBookingParameter.OutwardFacility and the storage charge in CurrentBookingParameter.OutwardFacilityCharge.
        The release state of the position can be set with CurrentBookingParameter.ReleaseState.
        Then call the InOrderPosInwardMovement() method to post the goods receipt. The booking is executed by ACFacilityManager.BookFacility(CurrentBookingParameter).
        To activate (release) a purchase order position, call the InOrderPosActivate() method on the Activation tab.
        To search for purchase order positions call the Search() method. The database result is copied to the InOrderPosList property.
        Then call the NavigateFirst() method to set CurrentInOrderPos with the first record in the list. SelectedInOrderPos is bound to the result grid of the explorer.
        There are no Save(), Delete() or Load() methods in this BSO, because bookings are executed immediately and are not stored as editable records.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityBookInOrderPos&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOFacilityBookInOrderPos : BSOFacilityBase 
    {
        /// <summary>
        /// The _ booking parameter
        /// </summary>
        ACMethodBooking _BookingParameter;

        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityBookInOrderPos"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityBookInOrderPos(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
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
            _BookingParameter = ACFacilityManager.ACUrlACTypeSignature("!InOrderPosInwardMovement", gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 

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
        /// Gets the current booking parameter.
        /// </summary>
        /// <value>The current booking parameter.</value>
        [ACPropertyCurrent(701, "BookingParameter", Description = "Booking parameters (ACMethodBooking) that are prepared in this dialog and executed with InOrderPosInwardMovement() or InOrderPosActivate(). Cleared by New().")]
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
        ACAccessNav<InOrderPos> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(790, InOrderPos.ClassName, Description = "Primary navigation access to the InOrderPos records; source of the search list and of SelectedInOrderPos/CurrentInOrderPos.")]
        public ACAccessNav<InOrderPos> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<InOrderPos>(InOrderPos.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the selected in order pos.
        /// </summary>
        /// <value>The selected in order pos.</value>
        [ACPropertySelected(702, InOrderPos.ClassName, Description = "The purchase order position currently selected in the result grid of the explorer.")]
        public InOrderPos SelectedInOrderPos
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
                OnPropertyChanged("SelectedInOrderPos");
            }
        }

        /// <summary>
        /// Gets or sets the current in order pos.
        /// </summary>
        /// <value>The current in order pos.</value>
        [ACPropertyCurrent(703, InOrderPos.ClassName, Description = "The current purchase order position; set by the navigation methods (e.g. NavigateFirst()) from the InOrderPosList.")]
        public InOrderPos CurrentInOrderPos
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
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(704, InOrderPos.ClassName, Description = "The result list of the search; filled by Search() with the purchase order positions matching the current filter.")]
        public IEnumerable<InOrderPos> InOrderPosList
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
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = "Searches purchase order positions with the current filter and copies the result to the InOrderPosList.")]
        public void Search()
        {
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("InOrderPosList");
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodCommand("BookingParameter", Const.New, (short)MISort.New, true, Global.ACKinds.MSMethodPrePost, Description = "Clears all booking parameters (CurrentBookingParameter.ClearBookingData()) and sets the state to 'New' so that a fresh booking can be entered. Called automatically on start.")]
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

        #region Buchung Zugang (InOrderPosInwardMovement)
        /// <summary>
        /// Ins the order pos inward movement.
        /// </summary>
        [ACMethodInteraction("BookingParameter", "en{'Post Inward Movement'}de{'Buche Lagerzugang'}", 701, true, "", Global.ACKinds.MSMethodPrePost, Description = "Posts the goods receipt (inward movement) for the selected purchase order position with the current booking parameters. The booking is executed by ACFacilityManager.BookFacility(CurrentBookingParameter). Enabled only if the booking parameters are complete (CurrentBookingParameter.IsEnabled()).")]
        public void InOrderPosInwardMovement()
        {
            if (!PreExecute("InOrderPosInwardMovement")) return;
            //TODO: CurrentBookingParameter.BookingType = ACFacilityManager.DefaultMDFacilityBookingType(MDFacilityBookingType.FacitlityBookingType.InOrderPosInwardMovement);
            ACFacilityManager.BookFacility(CurrentBookingParameter, this.DatabaseApp);
            PostExecute("InOrderPosInwardMovement");
        }

        /// <summary>
        /// Determines whether [is enabled in order pos inward movement].
        /// </summary>
        /// <returns><c>true</c> if [is enabled in order pos inward movement]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledInOrderPosInwardMovement()
        {
            return CurrentBookingParameter.IsEnabled();
        }
        #endregion

        #region Aktivierung Bestellposition (InOrderPosActivate)
        /// <summary>
        /// Ins the order pos activate.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Activate'}de{'Aktivierung'}", 702, true, Global.ACKinds.MSMethodPrePost, Description = "Activates (releases) the selected purchase order position with the current booking parameters via ACFacilityManager.BookFacility(CurrentBookingParameter). Enabled only if the booking parameters are complete (CurrentBookingParameter.IsEnabled()).")]
        public void InOrderPosActivate()
        {
            if (!PreExecute("InOrderPosActivate")) return;
            //TODO: CurrentBookingParameter.BookingType = ACFacilityManager.DefaultMDFacilityBookingType(MDFacilityBookingType.FacitlityBookingType.InOrderPosActivate);
            ACFacilityManager.BookFacility(CurrentBookingParameter, this.DatabaseApp);
            PostExecute("InOrderPosActivate");
        }
        /// <summary>
        /// Determines whether [is enabled in order pos activate].
        /// </summary>
        /// <returns><c>true</c> if [is enabled in order pos activate]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledInOrderPosActivate()
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
        /// Called when [activate].
        /// </summary>
        /// <param name="page">The page.</param>
        [ACMethodInfo("xxx", "en{'Activate'}de{'Aktiviere'}", 9999, true, Global.ACKinds.MSMethodPrePost, Description = "Internal event handler that is called when a tab item of the design is activated; contains no additional logic.")]
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
                case"InOrderPosInwardMovement":
                    InOrderPosInwardMovement();
                    return true;
                case"IsEnabledInOrderPosInwardMovement":
                    result = IsEnabledInOrderPosInwardMovement();
                    return true;
                case"InOrderPosActivate":
                    InOrderPosActivate();
                    return true;
                case"IsEnabledInOrderPosActivate":
                    result = IsEnabledInOrderPosActivate();
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
                case nameof(Search):
                    return new string[] { nameof(InitState) };
                case nameof(New):
                    return new string[] { nameof(ACState) };
                case nameof(InOrderPosInwardMovement):
                case nameof(InOrderPosActivate):
                    return new string[] { nameof(CurrentBookingParameter) };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion


    }

}
