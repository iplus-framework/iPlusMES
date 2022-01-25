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
        public IQueryable<FacilityReservation> GetFacilityReservation(DatabaseApp dbApp, FacilityReservationData facReservationData)
        {
            switch (facReservationData.FacReservationSearchType)
            {
                case FacilityReservationData.FacReservationSearchTypes.SearchByFacility:
                    _CurrentFacilityReservationList = dbApp.FacilityReservation
                                                      .Where(c => c.Facility == facReservationData.FacilityFilter)
                                                      .OrderBy(c => c.InsertDate);
                    break;
                case FacilityReservationData.FacReservationSearchTypes.SearchByFacilityLocation:
                    _CurrentFacilityReservationList = dbApp.FacilityReservation
                                                      .Where(c => c.Facility.Facility1_ParentFacility == facReservationData.FacilityLocationFilter)
                                                      .OrderBy(c => c.InsertDate);
                    break;
                case FacilityReservationData.FacReservationSearchTypes.SearchByMaterial:
                    _CurrentFacilityReservationList = dbApp.FacilityReservation
                                                      .Where(c => c.Material == facReservationData.MaterialFilter)
                                                      .OrderBy(c => c.InsertDate);
                    break;
            }

            // Wenn nur ein Lagerplatz betroffen ist, dann werden auch die Lagerplatzdaten geladen
            _CurrentFacility = _CurrentFacilityReservationList.Select(o => o.Facility).Distinct().FirstOrDefault();
            return _CurrentFacilityReservationList;
        }
        #endregion
    }
}
