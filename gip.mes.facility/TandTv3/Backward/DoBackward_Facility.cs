using gip.mes.datamodel;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class DoBackward_Facility : ItemTracking<Facility>
    {
        #region ctor's
        public DoBackward_Facility(DatabaseApp databaseApp, TandTResult result, Facility item) : base(databaseApp, result, item)
        {
            ItemTypeName = "Facility";
            if (!Result.Ids.Keys.Contains(item.FacilityID))
                Result.Ids.Add(item.FacilityID, ItemTypeName);
        }
        #endregion
    }
}
