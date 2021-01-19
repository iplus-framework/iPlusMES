using gip.mes.datamodel;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class DoBackward_DeliveryNote : ItemTracking<DeliveryNote>
    {
        #region ctor's
        public DoBackward_DeliveryNote(DatabaseApp databaseApp, TandTResult result, DeliveryNote item) : base(databaseApp, result, item)
        {
            ItemTypeName = "DeliveryNote";
            if (!Result.Ids.Keys.Contains(item.DeliveryNoteID))
                Result.Ids.Add(item.DeliveryNoteID, ItemTypeName);
        }
        #endregion
    }
}
