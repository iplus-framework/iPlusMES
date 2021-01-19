using System.Collections.Generic;
using gip.mes.datamodel;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoBackward_ProdOrderPartslist : DoItem<ProdOrderPartslist>
    {
        #region ctor's

        public DoBackward_ProdOrderPartslist(DatabaseApp databaseApp, TandTv2Result result, ProdOrderPartslist item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion

        #region methods 

        public override TandTv2StepItem Factory_StepItem(TandTv2Result result, TandTv2Step step)
        {
            TandTv2StepItem stepItem = base.Factory_StepItem(result, step);
            stepItem.PrimaryKeyID = Item.ProdOrderPartslistID;
            stepItem.TandT_ItemTypeEnum = TandTv2ItemTypeEnum.ProdOrderPartslist;
            stepItem.ProdOrderPartslist = Item;
            stepItem.InsertDate = Item.InsertDate;
            return stepItem;
        }

        public override string Factory_StepItem_ACIdentifier()
        {
            return string.Format(@"#{0} ({1})", Item.Sequence, Item.ProdOrder.ProgramNo);
        }

        public override string Factory_StepItem_ACCaptionTranslation()
        {
            return string.Format(@"#{0} [{1}] {2}", Item.Sequence, Item.Partslist.Material.MaterialNo, Item.Partslist.Material.MaterialName1);
        }

        public override List<IDoItem> ProcessRelatedSameStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            return new List<IDoItem>() { new DoBackward_ProdOrder(databaseApp, result, Item.ProdOrder, jobFilter) };
        }

        #endregion
    }
}
