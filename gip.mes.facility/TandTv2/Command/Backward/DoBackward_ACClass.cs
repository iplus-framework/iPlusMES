using gip.mes.datamodel;
using System;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoBackward_ACClass : DoItem<ACClass>
    {
        #region ctor's

        public DoBackward_ACClass(DatabaseApp dbApp, TandTv2Result result, ACClass item, TandTv2Job jobFilter) : base(dbApp, result, item, jobFilter)
        {

        }

        #endregion


        #region methods

        public override TandTv2StepItem Factory_StepItem(TandTv2Result result, TandTv2Step step)
        {
            TandTv2StepItem stepItem = base.Factory_StepItem(result, step);
            stepItem.PrimaryKeyID = Item.ACClassID;
            stepItem.TandT_ItemTypeEnum = TandTv2ItemTypeEnum.ACClass;
            stepItem.ACClass = Item;
            stepItem.InsertDate = Item.InsertDate;
            return stepItem;
        }

        public override string Factory_StepItem_ACIdentifier()
        {
            return Item.ACIdentifier;
        }

        public override string Factory_StepItem_ACCaptionTranslation()
        {
            return Item.ACCaptionTranslation;
        }

        #endregion

    }
}
