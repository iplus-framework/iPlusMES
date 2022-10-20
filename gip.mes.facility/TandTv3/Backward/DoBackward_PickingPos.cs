using gip.core.datamodel;
using gip.mes.datamodel;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class DoBackward_PickingPos : ItemTracking<PickingPos>
    {
        #region ctor's
        public DoBackward_PickingPos(DatabaseApp databaseApp, TandTResult result, PickingPos item)
      : base(databaseApp, result, item)
        {
            ItemTypeName = "PickingPos";
            if (!Result.Ids.Keys.Contains(item.PickingPosID))
                Result.Ids.Add(item.PickingPosID, ItemTypeName);
        }
        #endregion

        #region IItemTracking
        public override List<IACObjectEntity> GetSameStepItems()
        {
            List<IACObjectEntity> sameStepItems = new List<IACObjectEntity>();
            sameStepItems.Add(Item.Picking);
            
            if(Item.InOrderPos != null)
                sameStepItems.Add(Item.InOrderPos.TopParentInOrderPos);

            if(Item.OutOrderPos != null)
                sameStepItems.Add(Item.OutOrderPos.TopParentOutOrderPos);

            if(Item.FacilityBookingCharge_PickingPos.Any())
                sameStepItems.AddRange(Item.FacilityBookingCharge_PickingPos.ToArray());

            return sameStepItems;
        }

        public override void AssignItemToMixPoint(List<IACObjectEntity> sameStepItems)
        {
            if(Item.InOrderPos == null && Item.OutOrderPos == null)
            {
                TandTv3Point mixPoint = Result.AddMixPoint(Step, Item);
            }
        }

        #endregion
    }
}
