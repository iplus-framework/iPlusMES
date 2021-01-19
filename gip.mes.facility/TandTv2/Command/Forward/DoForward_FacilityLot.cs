using gip.mes.datamodel;
using System;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoForward_FacilityLot : DoBackward_FacilityLot
    {

        #region ctor's

        public DoForward_FacilityLot(DatabaseApp databaseApp, TandTv2Result result, FacilityLot item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion

        #region overriden methods

        #endregion

    }
}
