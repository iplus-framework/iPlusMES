using gip.core.datamodel;
using gip.mes.datamodel;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class DoBackward_DeliveryNotePos : ItemTracking<DeliveryNotePos>
    {

        #region ctor's
        public DoBackward_DeliveryNotePos(DatabaseApp databaseApp, TandTResult result, DeliveryNotePos item) : base(databaseApp, result, item)
        {
            ItemTypeName = "DeliveryNotePos";
            if (!Result.Ids.Keys.Contains(item.DeliveryNotePosID))
                Result.Ids.Add(item.DeliveryNotePosID, ItemTypeName);
            Result.DeliveryNotePoses.Add(Item);
        }
        #endregion

        #region IItemTracking
        public override List<IACObjectEntity> GetSameStepItems()
        {
            List<IACObjectEntity> sameStepItems = new List<IACObjectEntity>();
            sameStepItems.Add(Item.DeliveryNote);
            if (Item.InOrderPosID != null)
                sameStepItems.Add(Item.InOrderPos);
            if(Item.OutOrderPosID != null)
                sameStepItems.Add(Item.OutOrderPos);
            return sameStepItems;
        }
        #endregion
    }
}
