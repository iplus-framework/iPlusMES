using System;
using System.Collections.Generic;
using gip.mes.datamodel;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoBackward_Facility : DoItem<Facility>
    {

        #region ctor's

        public DoBackward_Facility(DatabaseApp dbApp, TandTv2Result result, Facility item, TandTv2Job jobFilter) : base(dbApp, result, item, jobFilter)
        {

        }

        #endregion

        #region methods

        public override TandTv2StepItem Factory_StepItem(TandTv2Result result, TandTv2Step step)
        {
            TandTv2StepItem stepItem = base.Factory_StepItem(result, step);
            stepItem.PrimaryKeyID = Item.FacilityID;
            stepItem.TandT_ItemTypeEnum = TandTv2ItemTypeEnum.Facility;
            stepItem.Facility = Item;
            stepItem.InsertDate = Item.InsertDate;
            return stepItem;
        }

        public override string Factory_StepItem_ACIdentifier()
        {
            return Item.FacilityNo;
        }

        public override string Factory_StepItem_ACCaptionTranslation()
        {
            return string.Format(@"en{{'{0}'}}", Item.FacilityName);
        }

        public override List<IDoItem> ProcessRelatedSameStep(DatabaseApp dbApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            if (Item.VBiFacilityACClassID == null) return null;
            return new List<IDoItem>() {new DoBackward_ACClass(dbApp, result, Item.VBiFacilityACClass, jobFilter) };
        }

        #endregion


    }
}
