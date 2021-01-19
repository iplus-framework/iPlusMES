using gip.core.datamodel;
using gip.mes.datamodel;
using System.Collections.Generic;

namespace gip.mes.facility.TandTv3
{
    public class DoForward_DeliveryNotePos : DoBackward_DeliveryNotePos
    {

        #region ctor's

        public DoForward_DeliveryNotePos(DatabaseApp databaseApp, TandTResult result, DeliveryNotePos item) : base(databaseApp, result, item)
        {
            TrackingDirection = MDTrackingDirectionEnum.Forward;
        }

        #endregion

    }
}