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
using gip.mes.datamodel; using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.facility;
using gip.mes.autocomponent;

namespace gip.bso.facility
{
    /// <summary>
    /// Class BSOFacilityMaintenance
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Facilitymaintenance'}de{'Lagerwartungsfunktionen'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + FacilityCharge.ClassName)]
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

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            this._BookingParameter = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
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
        /// Called when [activate].
        /// </summary>
        /// <param name="page">The page.</param>
        [ACMethodInfo("xxx", "en{'Activate'}de{'Aktiviere'}", 791, true, Global.ACKinds.MSMethodPrePost)]
        public void OnActivate(string page)
        {
            if (!PreExecute("OnActivate"))
                return;
            PostExecute("OnActivate");

        }

        #region BSO->ACProperty

        /// <summary>
        /// Gets the current booking parameter.
        /// </summary>
        /// <value>The current booking parameter.</value>
        [ACPropertyCurrent(701, "BookingParameter")]
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
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(790, FacilityCharge.ClassName)]
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
        /// Gets or sets the selected facility charge.
        /// </summary>
        /// <value>The selected facility charge.</value>
        [ACPropertySelected(702, FacilityCharge.ClassName)]
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
        /// Gets or sets the current facility charge.
        /// </summary>
        /// <value>The current facility charge.</value>
        [ACPropertyCurrent(703, FacilityCharge.ClassName)]
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
        /// Gets the facility charge list.
        /// </summary>
        /// <value>The facility charge list.</value>
        [ACPropertyList(704, FacilityCharge.ClassName)]
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
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("FacilityChargeList");
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'New'}de{'Neu'}", (short)MISort.New, true)]
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
        /// Matchings the weight with quantity all.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Completeadjustment Quantity with Weight'}de{'Gesamtabgleich Menge mit Gewicht'}", 701, true)]
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
        /// Matchings the weight with quantity material.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Adjustment Quantity with Weight'}de{'Abgleich Menge mit Gewicht'}", 702, true)]
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
        /// Matchings the stock all.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Adjustment all Facilitylocation'}de{'Abgleich alle Lagerplätze'}", 703, true)]
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
        /// Matchings the stock facility.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Adjustment definite Facilitylocation'}de{'Abgleich bestimmter Lagerplatz'}", 704, true)]
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
        /// Matchings the material inward facility charge all.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Adjustment Stock'}de{'Abgleich Bestände'}", 705, true)]
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
        /// Matchings the stock material material.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Adjustment Stock definite Material'}de{'Abgleich Bestände bestimmtes Material'}", 706, true)]
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

        #endregion


    }

}
