using gip.core.datamodel;
using gip.mes.datamodel;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class DoBackward_InOrderPos : ItemTracking<InOrderPos>
    {
        #region ctor's
        public DoBackward_InOrderPos(DatabaseApp databaseApp, TandTResult result, InOrderPos item)
      : base(databaseApp, result, item)
        {
            ItemTypeName = "InOrderPos";
            if (!Result.Ids.Keys.Contains(item.InOrderPosID))
                Result.Ids.Add(item.InOrderPosID, ItemTypeName);
        }
        #endregion

        #region IItemTracking
        public override List<IACObjectEntity> GetSameStepItems()
        {
            List<IACObjectEntity> sameStepItems = new List<IACObjectEntity>();
            sameStepItems.Add(Item.InOrder);
            InOrderPos childInOrderPos = Item.InOrderPos_ParentInOrderPos.FirstOrDefault();
            if (childInOrderPos != null)
            {
                sameStepItems.AddRange(childInOrderPos.DeliveryNotePos_InOrderPos);
                InOrderPos ccInorderPos = childInOrderPos.InOrderPos_ParentInOrderPos.FirstOrDefault();
                if (ccInorderPos != null)
                    sameStepItems.AddRange(ccInorderPos.FacilityBookingCharge_InOrderPos);
            }
           
            return sameStepItems;
        }

        public override void AssignItemToMixPoint(List<IACObjectEntity> sameStepItems)
        {
            TandTv3Point mixPoint = Result.AddMixPoint(Step, Item);
            mixPoint.DeliveryNotePositions.AddRange(Item.DeliveryNotePos_InOrderPos);
            if (Item.LabOrder_InOrderPos.Any())
            {
                DatabaseApp db_test = Item.GetObjectContext<DatabaseApp>();
                mixPoint.ExistLabOrder = true;
                mixPoint.ItemsWithLabOrder.Add(MixPointLabOrder.Factory(db_test, Item));
            }
        }

        #endregion
    }
}
