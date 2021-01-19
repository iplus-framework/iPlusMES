using System.Collections.Generic;
using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility.TandTv3
{
    public class DoForward_FacilityBooking : DoBackward_FacilityBooking
    {

        #region ctor's
        public DoForward_FacilityBooking(DatabaseApp databaseApp, TandTResult result, FacilityBooking item) : base(databaseApp, result, item)
        {
            TrackingDirection = MDTrackingDirectionEnum.Forward;
        }


        public override List<IACObjectEntity> GetSameStepItems()
        {
            if (Result.Lots.Count == 0)
                return base.GetSameStepItems();
            return new List<IACObjectEntity>();
        }
        #endregion

    }
}