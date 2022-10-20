using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility.TandTv3
{
    public class DoForward_FacilityCharge : DoBackward_FacilityCharge
    {

        #region ctor's

        public DoForward_FacilityCharge(DatabaseApp databaseApp, TandTResult result, FacilityCharge item) : base(databaseApp, result, item)
        {
            TrackingDirection = MDTrackingDirectionEnum.Forward;
        }

        #endregion

    }
}