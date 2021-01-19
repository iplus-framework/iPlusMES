using gip.mes.datamodel;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoBackward_OutOrder : DoItem<OutOrder>
    {
        #region ctor's

        public DoBackward_OutOrder(DatabaseApp databaseApp, TandTv2Result result, OutOrder item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion

        #region methods

        public override TandTv2StepItem Factory_StepItem(TandTv2Result result, TandTv2Step step)
        {
            TandTv2StepItem stepItem = base.Factory_StepItem(result, step);
            stepItem.PrimaryKeyID = Item.OutOrderID;
            stepItem.TandT_ItemTypeEnum = TandTv2ItemTypeEnum.OutOrder;
            stepItem.OutOrder = Item;
            stepItem.InsertDate = Item.InsertDate;
            return stepItem;
        }

        public override string Factory_StepItem_ACIdentifier()
        {
            return Item.OutOrderNo;
        }

        public override string Factory_StepItem_ACCaptionTranslation()
        {
            return string.Format(@"{0} {1} ({2})",
                Item.MDDelivType.ACCaption, Item.MDOutOrderState.ACCaption, Item.OutOrderDate.ToString("dd.MM.yyyy"));
        }

        #endregion
    }
}
