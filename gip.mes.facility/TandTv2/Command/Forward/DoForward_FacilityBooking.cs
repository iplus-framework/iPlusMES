using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoForward_FacilityBooking : DoBackward_FacilityBooking
    {
        #region ctor's 

        public DoForward_FacilityBooking(DatabaseApp databaseApp, TandTv2Result result, FacilityBooking item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion

    }
}
