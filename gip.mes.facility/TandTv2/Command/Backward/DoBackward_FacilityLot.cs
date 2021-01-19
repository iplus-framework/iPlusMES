using gip.mes.datamodel;
using System;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoBackward_FacilityLot : DoItem<FacilityLot>
    {

        #region ctor's

        public DoBackward_FacilityLot(DatabaseApp databaseApp, TandTv2Result result, FacilityLot item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion

        #region methods

        public override TandTv2StepItem Factory_StepItem(TandTv2Result result, TandTv2Step step)
        {
            TandTv2StepItem stepItem = base.Factory_StepItem(result, step);
            stepItem.PrimaryKeyID = Item.FacilityLotID;
            stepItem.TandT_ItemTypeEnum = TandTv2ItemTypeEnum.FacilityLot;
            stepItem.FacilityLot = Item;
            stepItem.InsertDate = Item.InsertDate;
            return stepItem;
        }

        public override string Factory_StepItem_ACIdentifier()
        {
            return Item.LotNo;
        }

        public override string Factory_StepItem_ACCaptionTranslation()
        {
            return Item.LotNo;
        }

        public override TandTv2StepLot Factory_StepLot(TandTv2Step step)
        {
            TandTv2StepLot tandT_StepLot = new TandTv2StepLot();
            tandT_StepLot.TandTv2StepLotID = Guid.NewGuid();
            tandT_StepLot.TandTv2Step = step;
            tandT_StepLot.FacilityLotID = Item.FacilityLotID;
            tandT_StepLot.LotNo = Item.LotNo;
            return tandT_StepLot;
        }

        #endregion
    }
}
