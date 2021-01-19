using System.Collections.Generic;
using gip.mes.datamodel;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoBackward_DeliveryNotePos : DoItem<DeliveryNotePos>
    {

        #region ctor's

        public DoBackward_DeliveryNotePos(DatabaseApp dbApp, TandTv2Result result, DeliveryNotePos item, TandTv2Job jobFilter) : base(dbApp, result, item, jobFilter)
        {

        }

        #endregion

        #region methods

        public override TandTv2StepItem Factory_StepItem(TandTv2Result result, TandTv2Step step)
        {
            TandTv2StepItem stepItem = base.Factory_StepItem(result, step);
            stepItem.PrimaryKeyID = Item.DeliveryNotePosID;
            stepItem.TandT_ItemTypeEnum = TandTv2ItemTypeEnum.DeliveryNotePos;
            stepItem.DeliveryNotePos = Item;
            stepItem.InsertDate = Item.InsertDate;
            return stepItem;
        }

        public override string Factory_StepItem_ACIdentifier()
        {
            return string.Format(@"#{0} ({1})", Item.Sequence, Item.DeliveryNote.DeliveryNoteNo);
        }

        public override string Factory_StepItem_ACCaptionTranslation()
        {
            return string.Format(@"#{0} [{1}] {2})", Item.Sequence, Item.Material.MaterialNo, Item.Material.MaterialName1);
        }

        public override List<IDoItem> ProcessRelatedSameStep(DatabaseApp dbApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            return new List<IDoItem>() { new DoBackward_DeliveryNote(dbApp, result, Item.DeliveryNote, jobFilter) };
        }

        #endregion
    }
}
