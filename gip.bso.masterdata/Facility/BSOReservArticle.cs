// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.mes.facility;
using gip.mes.autocomponent;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Businessobject or App for querying material reservations (reservations of stock for a material).
    /// This BSO is read-only: it only searches and displays FacilityReservation records via the FacilityReservationManager, it does not create, change or delete reservations.
    /// Enter the filter criteria in the CurrentFacilityReservationData object (the MaterialFilter property is the material whose reservations shall be shown).
    /// Call the Search() method: it sets the search type to SearchByMaterial and copies the result to the CurrentFacReservationList property.
    /// The CurrentFacility property provides the storage bin of the result, but only if the result list contains reservations of exactly one facility.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOReservArticle&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Material Reservation'}de{'Material Reservierung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true,
        Description = @"Businessobject or App for querying material reservations (reservations of stock for a material).
        This BSO is read-only: it only searches and displays FacilityReservation records via the FacilityReservationManager, it does not create, change or delete reservations.
        Enter the filter criteria in the CurrentFacilityReservationData object (the MaterialFilter property is the material whose reservations shall be shown).
        Call the Search() method: it sets the search type to SearchByMaterial and copies the result to the CurrentFacReservationList property.
        The CurrentFacility property provides the storage bin of the result, but only if the result list contains reservations of exactly one facility.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOReservArticle&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    class BSOReservArticle : ACBSOvb
    {
        /// <summary>
        /// The _ facility reservation manager
        /// </summary>
        FacilityReservationManager _FacilityReservationManager;
        #region c´tors

        /// <summary>
        /// Konstruktor für ACComponent
        /// (Gleiche Signatur, wie beim ACGenericObject)
        /// </summary>
        /// <param name="acType">ACType anhand dessen die Methoden, Properties und Designs initialisiert werden</param>
        /// <param name="content">Inhalt
        /// Bei Model- oder BSO immer gleich ACClass
        /// Bei WF immer WorkOrderWF</param>
        /// <param name="parentACObject">Lebende ACComponent-Instanz</param>
        /// <param name="parameter">Parameter je nach Ableitungsimplementierung</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOReservArticle(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            this._CurrentFacilityReservationData = null;
            return await base.ACDeInit(deleteACClassTask);
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
        [ACPropertyCurrent(9999, "FacilityReservationData", Description = "Filter criteria object for the reservation search. Set the MaterialFilter before calling Search().")]
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
        /// <value>The material filter.</value>
        public Material MaterialFilter
        {
            get
            {
                return CurrentFacilityReservationData.MaterialFilter;
            }
        }

        /// <summary>
        /// Ergebnis: Aktueller Lagerplatz, Nur verfügbar wenn nur ein Lagerort im Listen der
        /// FacilityReservation vorkommt
        /// </summary>
        /// <value>The current facility.</value>
        [ACPropertyCurrent(9999, Facility.ClassName, Description = "The storage bin of the reservation result, but only available if the result list contains reservations of exactly one facility.")]
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
        [ACPropertyList(9999, "CurrentFacReservation", Description = "Result of the last reservation search, filled by the Search() method.")]
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
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("FacilityReservationData", "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = "Executes the reservation search by material (MaterialFilter of CurrentFacilityReservationData) and fills the CurrentFacReservationList property.")]
        public void Search()
        {
            CurrentFacilityReservationData.FacReservationSearchType = FacilityReservationData.FacReservationSearchTypes.SearchByMaterial;
            _FacilityReservationManager.GetFacilityReservation(this.DatabaseApp, CurrentFacilityReservationData);

        }
        #endregion

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(Search):
                    Search();
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
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion

    }
}