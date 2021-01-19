using gip.mes.datamodel;
using System.Collections.Generic;
using System.Linq;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoBackward_OutOrderPos : DoItem<OutOrderPos>
    {
        #region ctor's

        public DoBackward_OutOrderPos(DatabaseApp databaseApp, TandTv2Result result, OutOrderPos item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion

        #region overrides
        public override TandTv2StepItem Factory_StepItem(TandTv2Result result, TandTv2Step step)
        {
            TandTv2StepItem stepItem = base.Factory_StepItem(result, step);
            stepItem.PrimaryKeyID = Item.OutOrderPosID;
            stepItem.TandT_ItemTypeEnum = TandTv2ItemTypeEnum.OutOrderPos;
            stepItem.OutOrderPos = Item;
            stepItem.InsertDate = Item.InsertDate;
            return stepItem;
        }

        public override string Factory_StepItem_ACIdentifier()
        {
            return string.Format(@"#{0} ({1})", Item.Sequence, Item.OutOrder.OutOrderNo);
        }

        public override string Factory_StepItem_ACCaptionTranslation()
        {
            return Item.ACCaption;
        }

        public override List<IDoItem> ProcessRelatedSameStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            List<IDoItem> related = new List<IDoItem>();
            related.Add(new DoBackward_OutOrder(databaseApp, result, Item.OutOrder, jobFilter));
            var deliveryNotePoses = Item
                .DeliveryNotePos_OutOrderPos
                .Select(c => new DoBackward_DeliveryNotePos(databaseApp, result, c, jobFilter));
            related.AddRange(deliveryNotePoses);
            return related;
        }

        #endregion
    }
}
