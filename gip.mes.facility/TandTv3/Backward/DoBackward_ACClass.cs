using gip.mes.datamodel;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class DoBackward_ACClass : ItemTracking<gip.mes.datamodel.ACClass>
    {
        #region ctor's
        public DoBackward_ACClass(DatabaseApp databaseApp, TandTResult result, ACClass item)
      : base(databaseApp, result, item)
        {
            ItemTypeName = "ACClass";
            if (!Result.Ids.Keys.Contains(item.ACClassID))
                Result.Ids.Add(item.ACClassID, ItemTypeName);
        }
        #endregion
    }
}
