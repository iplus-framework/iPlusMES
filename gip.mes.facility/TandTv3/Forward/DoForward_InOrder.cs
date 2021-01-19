using gip.mes.datamodel;

namespace gip.mes.facility.TandTv3
{
    public class DoForward_InOrder : DoBackward_InOrder
    {

        #region ctor's

        public DoForward_InOrder(DatabaseApp databaseApp, TandTResult result, InOrder item) : base(databaseApp, result, item)
        {
            TrackingDirection = MDTrackingDirectionEnum.Forward;
        }

        #endregion
	}
}