using gip.mes.datamodel;

namespace gip.mes.facility.TandTv3
{
    public class DoForward_OutOrderPos : DoBackward_OutOrderPos
    {

        #region ctor's

        public DoForward_OutOrderPos(DatabaseApp databaseApp, TandTResult result, OutOrderPos item) : base(databaseApp, result, item)
        {
            TrackingDirection = MDTrackingDirectionEnum.Forward;
        }

        #endregion
	}
}