using gip.mes.datamodel;

namespace gip.mes.facility.TandTv3
{
    public class DoForward_DeliveryNote : DoBackward_DeliveryNote
    {

        #region ctor's

        public DoForward_DeliveryNote(DatabaseApp databaseApp, TandTResult result, DeliveryNote item) : base(databaseApp, result, item)
        {
            TrackingDirection = MDTrackingDirectionEnum.Forward;
        }

        #endregion
	}
}