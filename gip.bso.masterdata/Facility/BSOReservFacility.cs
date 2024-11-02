// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOReservFacility.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.mes.facility;
using gip.mes.autocomponent;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Version 3
    /// Folgende alte Masken sind in diesem BSO enthalten:
    /// 1. Fahrzeugwaage
    /// Neue Masken:
    /// 1. Fahrzeugwaage
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Facility Reservation'}de{'Lagerplatz Reservierung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    class BSOReservFacility : ACBSOvb
    {
        /// <summary>
        /// The _ facility reservation manager
        /// </summary>
        FacilityReservationManager _FacilityReservationManager;

        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOReservFacility"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOReservFacility(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            _FacilityReservationManager = new FacilityReservationManager(DatabaseApp, Root);

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            this._CurrentFacilityReservationData = null;
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region BSO->ACProperty
        /// <summary>
        /// Datenklassen für die Filterkriterien
        /// </summary>
        FacilityReservationData _CurrentFacilityReservationData;
        /// <summary>
        /// Gets or sets the current facility reservation data.
        /// </summary>
        /// <value>The current facility reservation data.</value>
        [ACPropertyCurrent(9999, "FacilityReservationData")]
        public FacilityReservationData CurrentFacilityReservationData
        {
            get
            {
                return _CurrentFacilityReservationData;
            }
            set
            {
                _CurrentFacilityReservationData = value;
                OnPropertyChanged("CurrentFacilityReservationData");
            }
        }

        /// <summary>
        /// Filterkriterium Lagerort
        /// </summary>
        /// <value>The facility filter.</value>
        public Facility FacilityFilter
        {
            get
            {
                return CurrentFacilityReservationData.FacilityFilter;
            }
        }

        /// <summary>
        /// Ergebnis: Aktueller Lagerplatz, Nur verfügbar wenn nur ein Lagerort im Listen der
        /// FacilityReservation vorkommt
        /// </summary>
        /// <value>The current facility.</value>
        [ACPropertyCurrent(9999, Facility.ClassName)]
        public Facility CurrentFacility
        {
            get
            {
                return _FacilityReservationManager.CurrentFacility;
            }
        }

        /// <summary>
        /// Ergebnis: Liste aller Lagerplatzreservierungen
        /// </summary>
        /// <value>The current fac reservation list.</value>
        [ACPropertyList(9999, "CurrentFacReservation")]
        public IQueryable<FacilityReservation> CurrentFacReservationList
        {
            get
            {
                return _FacilityReservationManager.CurrentFacReservationList;
            }
        }
        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodCommand("FacilityReservationData", "en{'New'}de{'Neu'}", (short)MISort.New, true, Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            //CurrentFacilityReservationData = _FacilityReservationManager.NewFacReservationData();
            ACState = Const.SMNew;
            PostExecute("New");

        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("FacilityReservationData", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            CurrentFacilityReservationData.FacReservationSearchType = FacilityReservationData.FacReservationSearchTypes.SearchByFacility;
            _FacilityReservationManager.GetFacilityReservation(this.DatabaseApp, CurrentFacilityReservationData);

        }
        #endregion

        #region ACState
        /// <summary>
        /// SMs the new.
        /// </summary>
        [ACMethodState("en{'New'}de{'Neu'}", 1010)]
        public virtual void SMNew()
        {
            if (!PreExecute(Const.SMNew))
                return;
            PostExecute(Const.SMNew);
        }
        #endregion

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(New):
                    New();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
                case nameof(SMNew):
                    SMNew();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}