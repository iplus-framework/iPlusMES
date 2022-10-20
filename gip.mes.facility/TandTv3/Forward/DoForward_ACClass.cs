using gip.mes.datamodel;

namespace gip.mes.facility.TandTv3
{
    public class DoForward_ACClass : DoBackward_ACClass
    {

        #region ctor's

        public DoForward_ACClass(DatabaseApp databaseApp, TandTResult result, gip.mes.datamodel.ACClass item)
       : base(databaseApp, result, item)
        {
            TrackingDirection = MDTrackingDirectionEnum.Forward;
        }

        #endregion
	}
}