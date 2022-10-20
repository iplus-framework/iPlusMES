// ***********************************************************************
// Assembly         : gip.bso.facility
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 11-08-2012
// ***********************************************************************
// <copyright file="BSOFacilityBookOutOrderPos.cs" company="gip mbh, Oftersheim, Germany">
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
    /// BSOFacilityBookOutOrderPos dient zur Einlagerung, Umlagerung und Ausbuchung von Chargen
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Outorderfacility'}de{'Auftragsbestandführung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + OutOrderPos.ClassName)]
    public class BSOFacilityBookOutOrderPos : BSOFacilityBase 
    {
        /// <summary>
        /// The _ booking parameter
        /// </summary>
        ACMethodBooking _BookingParameter;

        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityBookOutOrderPos"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityBookOutOrderPos(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
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
            _BookingParameter = ACFacilityManager.ACUrlACTypeSignature("!OutOrderPosOutwardMovement", gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 

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
        ACAccessNav<OutOrderPos> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(790, OutOrderPos.ClassName)]
        public ACAccessNav<OutOrderPos> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<OutOrderPos>(OutOrderPos.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the selected out order pos.
        /// </summary>
        /// <value>The selected out order pos.</value>
        [ACPropertySelected(702, OutOrderPos.ClassName)]
        public OutOrderPos SelectedOutOrderPos
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
                OnPropertyChanged("SelectedOutOrderPos");
            }
        }

        /// <summary>
        /// Gets or sets the current out order pos.
        /// </summary>
        /// <value>The current out order pos.</value>
        [ACPropertyCurrent(703, OutOrderPos.ClassName)]
        public OutOrderPos CurrentOutOrderPos
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
        /// Gets the out order pos list.
        /// </summary>
        /// <value>The out order pos list.</value>
        [ACPropertyList(704, OutOrderPos.ClassName)]
        public IEnumerable<OutOrderPos> OutOrderPosList
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
            OnPropertyChanged("OutOrderPosList");
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'New'}de{'Neu'}", (short)MISort.New, true, Global.ACKinds.MSMethodPrePost)]
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

        #region Buchung Abgang(OutOrderPosOutwardMovement)
        /// <summary>
        /// Outs the order pos outward movement.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Outward'}de{'Lagerabgang'}", 701, true, Global.ACKinds.MSMethodPrePost)]
        public void OutOrderPosOutwardMovement()
        {
            if (!PreExecute("OutOrderPosOutwardMovement")) return;
            //TODO: CurrentBookingParameter.BookingType = _FacilityManager.DefaultMDFacilityBookingType(MDFacilityBookingType.FacitlityBookingType.OutOrderPosOutwardMovement);
            ACFacilityManager.BookFacility(CurrentBookingParameter, this.DatabaseApp);
            PostExecute("OutOrderPosOutwardMovement");
        }
        /// <summary>
        /// Determines whether [is enabled out order pos outward movement].
        /// </summary>
        /// <returns><c>true</c> if [is enabled out order pos outward movement]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledOutOrderPosOutwardMovement()
        {
            return CurrentBookingParameter.IsEnabled();
        }
        #endregion

        #region Aktivierung Auftragsposition (OutOrderPosActivate)
        /// <summary>
        /// Outs the order pos activate.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Activate'}de{'Aktivierung'}", 702, true, Global.ACKinds.MSMethodPrePost)]
        public void OutOrderPosActivate()
        {
            if (!PreExecute("OutOrderPosActivate")) return;
            //TODO: CurrentBookingParameter.BookingType = _FacilityManager.DefaultMDFacilityBookingType(MDFacilityBookingType.FacitlityBookingType.OutOrderPosActivate);
            ACFacilityManager.BookFacility(CurrentBookingParameter, this.DatabaseApp);
            PostExecute("OutOrderPosActivate");
        }

        /// <summary>
        /// Determines whether [is enabled out order pos activate].
        /// </summary>
        /// <returns><c>true</c> if [is enabled out order pos activate]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledOutOrderPosActivate()
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
        [ACMethodInfo("xxx", "en{'Activate'}de{'Aktiviere'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
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
                case"OutOrderPosOutwardMovement":
                    OutOrderPosOutwardMovement();
                    return true;
                case"IsEnabledOutOrderPosOutwardMovement":
                    result = IsEnabledOutOrderPosOutwardMovement();
                    return true;
                case"OutOrderPosActivate":
                    OutOrderPosActivate();
                    return true;
                case"IsEnabledOutOrderPosActivate":
                    result = IsEnabledOutOrderPosActivate();
                    return true;
                case"OnActivate":
                    OnActivate((String)acParameter[0]);
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }

}
