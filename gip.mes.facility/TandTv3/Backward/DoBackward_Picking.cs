using gip.mes.datamodel;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class DoBackward_Picking : ItemTracking<Picking>
    {
        #region ctor's
        public DoBackward_Picking(DatabaseApp databaseApp, TandTResult result, Picking item)
      : base(databaseApp, result, item)
        {
            ItemTypeName = "Picking";
            if (!Result.Ids.Keys.Contains(item.PickingID))
                Result.Ids.Add(item.PickingID, ItemTypeName);
        }
        #endregion

       
    }
}
