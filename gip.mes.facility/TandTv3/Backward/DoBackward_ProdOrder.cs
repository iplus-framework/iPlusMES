using gip.mes.datamodel;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class DoBackward_ProdOrder : ItemTracking<ProdOrder>
    {

        #region ctor's
        public DoBackward_ProdOrder(DatabaseApp databaseApp, TandTResult result, ProdOrder item) : base(databaseApp, result, item)
        {
            ItemTypeName = "ProdOrder";
            if (!Result.Ids.Keys.Contains(item.ProdOrderID))
                Result.Ids.Add(item.ProdOrderID, ItemTypeName);
            if (!Result.ProgramNos.Contains(item.ProgramNo))
                Result.ProgramNos.Add(item.ProgramNo);
        }
        #endregion
    }
}
