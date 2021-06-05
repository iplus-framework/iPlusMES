using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoBackward_InOrderPos : DoItem<InOrderPos>
    {

        #region ctor's

        public DoBackward_InOrderPos(DatabaseApp databaseApp, TandTv2Result result, InOrderPos item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion

        #region methods
        public override TandTv2StepItem Factory_StepItem(TandTv2Result result, TandTv2Step step)
        {
            TandTv2StepItem stepItem = base.Factory_StepItem(result, step);
            stepItem.PrimaryKeyID = Item.InOrderID;
            stepItem.TandT_ItemTypeEnum = TandTv2ItemTypeEnum.InOrderPos;
            stepItem.InOrderPos = Item;
            stepItem.InsertDate = Item.InsertDate;
            return stepItem;
        }

        public override string Factory_StepItem_ACIdentifier()
        {
            return string.Format(@"#{0} ({1})", Item.Sequence, Item.InOrder.InOrderNo);
        }

        public override string Factory_StepItem_ACCaptionTranslation()
        {
            return Item.ACCaption;
        }

        public override List<IDoItem> ProcessRelatedSameStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            List<IDoItem> related = new List<IDoItem>();
            related.Add(new DoBackward_InOrder(databaseApp, result, Item.InOrder, jobFilter));
            var deliveryNotePositions = Item
                .DeliveryNotePos_InOrderPos
                .Select(c => new DoBackward_DeliveryNotePos(databaseApp, result, c, jobFilter));
            related.AddRange(deliveryNotePositions);
            return related;
        }

        #endregion
    }
}
