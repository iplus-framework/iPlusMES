using gip.mes.datamodel;

namespace gip.mes.facility.TandTv3
{
    public class DoForward_Facility : DoBackward_Facility
    {

        #region ctor's
        public DoForward_Facility(DatabaseApp databaseApp, TandTResult result, Facility item) : base(databaseApp, result, item)
        {
            TrackingDirection = MDTrackingDirectionEnum.Forward;
        }
        #endregion
	}
}