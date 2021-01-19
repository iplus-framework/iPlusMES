using gip.mes.datamodel;

namespace gip.mes.facility.TandTv3
{
    public class DoForward_ProdOrder : DoBackward_ProdOrder
    {

        #region ctor's

        public DoForward_ProdOrder(DatabaseApp databaseApp, TandTResult result, ProdOrder item) : base(databaseApp, result, item)
        {
            TrackingDirection = MDTrackingDirectionEnum.Forward;
        }

        #endregion
	}
}