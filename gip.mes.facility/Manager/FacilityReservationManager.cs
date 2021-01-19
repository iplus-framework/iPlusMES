using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    public class FacilityReservationManager : ACManagerBase
    { 
        IQueryable<FacilityReservation> _CurrentFacilityReservationList;
        Facility _CurrentFacility;

        #region c´tors
        public FacilityReservationManager(IACEntityObjectContext db, IRoot root)
            : base(db, root)
        {
        }
        #endregion

        public DatabaseApp Database
        {
            get
            {
                // Für "normale" Anwendungen
                return this.ObjectContext as DatabaseApp;
            }
        }

        #region LoadedFacilityReservation
        public IQueryable<FacilityReservation> CurrentFacReservationList
        {
            get
            {
                return _CurrentFacilityReservationList;
            }
        }
        
        public Facility CurrentFacility
        {
            get
            {
                return _CurrentFacility;
            }
        }
        #endregion

        #region Manager->Search->FacilityReservation
        public IQueryable<FacilityReservation> GetFacilityReservation(FacilityReservationData facReservationData)
        {
            switch (facReservationData.FacReservationSearchType)
            {
                case FacilityReservationData.FacReservationSearchTypes.SearchByFacility:
                    _CurrentFacilityReservationList = from c in Database.FacilityReservation
                                                      where c.Facility == facReservationData.FacilityFilter
                                                      orderby c.InsertDate
                                                      select c;
                    break;
                case FacilityReservationData.FacReservationSearchTypes.SearchByFacilityLocation:
                    _CurrentFacilityReservationList = from c in Database.FacilityReservation
                                                      where c.Facility.Facility1_ParentFacility == facReservationData.FacilityLocationFilter
                                                      orderby c.InsertDate
                                                      select c;
                    break;
                case FacilityReservationData.FacReservationSearchTypes.SearchByMaterial:
                    _CurrentFacilityReservationList = from c in Database.FacilityReservation
                                                      where c.Material == facReservationData.MaterialFilter
                                                      orderby c.InsertDate
                                                      select c;
                    break;
            }

            // Wenn nur ein Lagerplatz betroffen ist, dann werden auch die Lagerplatzdaten geladen
            int count = _CurrentFacilityReservationList.Select(o => o.Facility).Distinct().Count();

            if (count == 1)
            {
                _CurrentFacility = _CurrentFacilityReservationList.Select(o => o.Facility).Distinct().First();
            }
            else
            {
                _CurrentFacility = null;
            }

            return _CurrentFacilityReservationList;
        }
        #endregion
    }
}
