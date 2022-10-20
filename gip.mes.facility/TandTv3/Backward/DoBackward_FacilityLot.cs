using gip.mes.datamodel;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class DoBackward_FacilityLot : ItemTracking<FacilityLot>
    {
        #region ctor's

        public DoBackward_FacilityLot(DatabaseApp databaseApp, TandTResult result, FacilityLot item) : base(databaseApp, result, item)
        {
            ItemTypeName = "FacilityLot";
            if (!Result.Ids.Keys.Contains(item.FacilityLotID))
                Result.Ids.Add(item.FacilityLotID, ItemTypeName);
            result.Lots.Add(item.LotNo);
        }

        #endregion
    }
}
