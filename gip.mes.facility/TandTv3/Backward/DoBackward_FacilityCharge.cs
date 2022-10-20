using gip.core.datamodel;
using gip.mes.datamodel;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class DoBackward_FacilityCharge : ItemTracking<FacilityCharge>
    {
        #region ctor's

        public DoBackward_FacilityCharge(DatabaseApp databaseApp, TandTResult result, FacilityCharge item)
       : base(databaseApp, result, item)
        {
            ItemTypeName = "FacilityCharge";
            if (!Result.Ids.Keys.Contains(item.FacilityChargeID))
                Result.Ids.Add(item.FacilityChargeID, ItemTypeName);
            result.RegisterFacilityChargeID(result.CurrentStep.StepNo, item.FacilityChargeID);
        }

        #endregion

        #region IItemTracking

        public override List<IACObjectEntity> GetSameStepItems()
        {
            List<IACObjectEntity> sameStepItems = new List<IACObjectEntity>();
            if (Item.FacilityLotID != null)
                sameStepItems.Add(Item.FacilityLot);
            return sameStepItems;
        }

        #endregion
    }
}
