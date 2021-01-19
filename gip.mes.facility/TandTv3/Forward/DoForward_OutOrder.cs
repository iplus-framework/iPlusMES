using gip.mes.datamodel;

namespace gip.mes.facility.TandTv3
{
    public class DoForward_OutOrder : DoBackward_OutOrder
    {

        #region ctor's

        public DoForward_OutOrder(DatabaseApp databaseApp, TandTResult result, OutOrder item) : base(databaseApp, result, item)
        {
            TrackingDirection = MDTrackingDirectionEnum.Forward;
        }

        #endregion
	}
}