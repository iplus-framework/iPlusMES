using gip.mes.datamodel;

namespace gip.mes.facility.TandTv3
{
    public class DoForward_InOrderPos : DoBackward_InOrderPos
    {
        #region ctor's

        public DoForward_InOrderPos(DatabaseApp databaseApp, TandTResult result, InOrderPos item) : base(databaseApp, result, item)
        {
            TrackingDirection = MDTrackingDirectionEnum.Forward;
        }

        #endregion
	}
}