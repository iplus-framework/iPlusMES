using gip.mes.datamodel;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoBackward_ProdOrder : DoItem<ProdOrder>
    {

        #region ctor's

        public DoBackward_ProdOrder(DatabaseApp databaseApp, TandTv2Result result, ProdOrder item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion

        #region methods

        public override TandTv2StepItem Factory_StepItem(TandTv2Result result, TandTv2Step step)
        {
            TandTv2StepItem stepItem = base.Factory_StepItem(result, step);
            stepItem.PrimaryKeyID = Item.ProdOrderID;
            stepItem.TandT_ItemTypeEnum = TandTv2ItemTypeEnum.ProdOrder;
            stepItem.ProdOrder = Item;
            stepItem.InsertDate = Item.InsertDate;
            return stepItem;
        }

        public override string Factory_StepItem_ACIdentifier()
        {
            return Item.ProgramNo;
        }

        public override string Factory_StepItem_ACCaptionTranslation()
        {
            return string.Format(@"[{0}] {1}", Item.ProgramNo, Item.MDProdOrderState.ACCaption);
        }

        #endregion
    }
}
