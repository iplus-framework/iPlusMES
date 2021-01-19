using gip.mes.datamodel;

namespace gip.mes.facility.TandTv3
{
    public class DoBackward_OutOrder : ItemTracking<OutOrder>
    {
        #region ctor's
        public DoBackward_OutOrder(DatabaseApp databaseApp, TandTResult result, OutOrder item) : base(databaseApp, result, item)
        {
            ItemTypeName = "OutOrder";
        }
        #endregion
    }
}
