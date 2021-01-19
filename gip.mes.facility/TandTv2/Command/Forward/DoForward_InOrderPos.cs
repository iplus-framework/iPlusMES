using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoForward_InOrderPos : DoBackward_InOrderPos
    {

        #region ctor's

        public DoForward_InOrderPos(DatabaseApp databaseApp, TandTv2Result result, InOrderPos item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion

        #region Mehtods override
        public override List<IDoItem> ProcessRelatedSameStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            List<IDoItem> related = new List<IDoItem>();

            // InOrder
            related.Add(new DoForward_InOrder(databaseApp, result, Item.InOrder, jobFilter));

            // DeliveryNotePos
            var deliveryNotePoses = Item
                .DeliveryNotePos_InOrderPos
                .Select(c => new DoForward_DeliveryNotePos(databaseApp, result, c, jobFilter));

            // FacilityBookingCharge
            List<FacilityBookingCharge> fbc =
                Item
                .FacilityBookingCharge_InOrderPos
                .ToList();
            if (fbc.Any())
                related.AddRange(fbc.Select(c => new DoForward_FacilityBookingCharge(databaseApp, result, c, jobFilter)).ToList());

            related.AddRange(deliveryNotePoses);

            return related;
        }
        #endregion

    }
}
