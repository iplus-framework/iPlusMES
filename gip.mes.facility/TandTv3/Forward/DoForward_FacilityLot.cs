using gip.mes.datamodel;

namespace gip.mes.facility.TandTv3
{
    public class DoForward_FacilityLot : DoBackward_FacilityLot
    {

        #region ctor's

        public DoForward_FacilityLot(DatabaseApp databaseApp, TandTResult result, FacilityLot item) : base(databaseApp, result, item)
        {
            TrackingDirection = MDTrackingDirectionEnum.Forward;
        }

        #endregion
	}
}