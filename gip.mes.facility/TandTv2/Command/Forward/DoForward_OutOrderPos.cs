using gip.mes.datamodel;
using System.Collections.Generic;
using System.Linq;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoForward_OutOrderPos : DoBackward_OutOrderPos
    {
        #region ctor's

        public DoForward_OutOrderPos(DatabaseApp databaseApp, TandTv2Result result, OutOrderPos item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion

        #region overrides

        public override List<IDoItem> ProcessRelatedSameStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            List<IDoItem> related = new List<IDoItem>();
            related.Add(new DoForward_OutOrder(databaseApp, result, Item.OutOrder, jobFilter));
            var deliveryNotePositions = Item
                .DeliveryNotePos_OutOrderPos
                .Select(c => new DoForward_DeliveryNotePos(databaseApp, result, c, jobFilter));
            related.AddRange(deliveryNotePositions);
            return related;
        }

        #endregion
    }
}
