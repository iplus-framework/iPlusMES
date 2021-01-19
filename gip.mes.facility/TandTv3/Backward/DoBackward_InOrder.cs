using gip.mes.datamodel;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class DoBackward_InOrder : ItemTracking<InOrder>
    {
        #region ctor's
        public DoBackward_InOrder(DatabaseApp databaseApp, TandTResult result, InOrder item) : base(databaseApp, result, item)
        {
            ItemTypeName = "InOrder";
            if (!Result.Ids.Keys.Contains(item.InOrderID))
                Result.Ids.Add(item.InOrderID, ItemTypeName);
        }
        #endregion
    }
}
