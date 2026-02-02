// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System.Linq;
using System.Threading.Tasks;
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
    /// 1. Reservierungen-&gt;Artikel Reservierungen
    /// Neue Masken:
    /// 1. Artikel Reservierungen
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Material Reservation'}de{'Material Reservierung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
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
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("FacilityReservationData", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
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

        #endregion


    }
}