using gip.core.datamodel;
using gip.mes.datamodel;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class DoBackward_OutOrderPos : ItemTracking<OutOrderPos>
    {
        #region ctor's
        public DoBackward_OutOrderPos(DatabaseApp databaseApp, TandTResult result, OutOrderPos item) : base(databaseApp, result, item)
        {
            ItemTypeName = "OutOrderPos";
            if (!Result.Ids.Keys.Contains(item.OutOrderPosID))
                Result.Ids.Add(item.OutOrderPosID, ItemTypeName);
        }
        #endregion

        #region IItemTracking
        public override List<IACObjectEntity> GetSameStepItems()
        {
            List<IACObjectEntity> sameStepItems = new List<IACObjectEntity>();
            sameStepItems.Add(Item.OutOrder);

            sameStepItems.AddRange(Item.DeliveryNotePos_OutOrderPos);
            return sameStepItems;
        }

        public override void AssignItemToMixPoint(List<IACObjectEntity> sameStepItems)
        {
            TandTv3Point mixPoint = Result.AddMixPoint(Step, Item);
            mixPoint.DeliveryNotePositions.AddRange(
                Item
                .OutOrder
                .OutOrderPos_OutOrder
                .SelectMany(c => c.DeliveryNotePos_OutOrderPos)
                .ToArray()
                );

            mixPoint.PickingPositions.AddRange(
              Item
              .OutOrder
              .OutOrderPos_OutOrder
              .SelectMany(c => c.PickingPos_OutOrderPos)
              .ToList());

        }
        #endregion
    }
}
