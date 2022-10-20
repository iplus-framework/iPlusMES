using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility.TandTv3
{
    public class DoBackward_ProdOrderPartslist : ItemTracking<ProdOrderPartslist>
    {
        #region ctor's

        public DoBackward_ProdOrderPartslist(DatabaseApp databaseApp, TandTResult result, ProdOrderPartslist item) : base(databaseApp, result, item)
        {
            ItemTypeName = "ProdOrderPartslist";
            if (!Result.Ids.Keys.Contains(item.ProdOrderPartslistID))
                Result.Ids.Add(item.ProdOrderPartslistID, ItemTypeName);
        }

        #endregion

        #region IItemTracking

        public override List<IACObjectEntity> GetSameStepItems()
        {
            List<IACObjectEntity> sameStepItems = new List<IACObjectEntity>();
            sameStepItems.Add(Item.ProdOrder);
            return sameStepItems;
        }

        #endregion

    }
}
