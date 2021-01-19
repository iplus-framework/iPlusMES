using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    /// <summary>
    /// FacilityReservationData stellt die Filterkriterien für den FacilityReservationManager
    /// zur Verfügung.
    /// 
    /// Derzeit kann nach Lagerort, Lagerplatz oder Artikel gefiltert werden
    /// 
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'FacilityReservationData'}de{'FacilityReservationData'}", Global.ACKinds.TACClass)]
    public class FacilityReservationData 
    {
        public enum FacReservationSearchTypes : short
        {
            SearchByFacility = 1,               // Nach Lagerplatz suchen
            SearchByFacilityLocation = 2,       // Nach Lagerort suchen
            SearchByMaterial = 3,               // Nach Artikel suchen
        }

        #region Datamember
        public FacReservationSearchTypes FacReservationSearchType
        {
            get;
            set;
        }

        public Facility FacilityLocationFilter
        {
            get;
            set;
        }

        public Facility FacilityFilter
        {
            get;
            set;
        }

        public Material MaterialFilter
        {
            get;
            set;
        }
        #endregion

        #region IManagerData Members
        public bool IsValid()
        {
            // TODO: 
            return true;
        }
        public MsgWithDetails ValidMessage
        {
            get
            {
                // TODO:
                return null;
            }
        }
        #endregion

        #region IManagerData Members


        public string InfoMessage
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}


