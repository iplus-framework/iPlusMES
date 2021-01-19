using System.Collections.Generic;
using gip.mes.datamodel;
using System.Linq;
using iplusContext = gip.core.datamodel;
using System;

namespace gip.mes.facility
{
    public class DoBackward_FacilityCharge : DoItem<FacilityCharge>
    {

        #region ctor's

        public DoBackward_FacilityCharge(DatabaseApp dbApp, TandTv2Result result, FacilityCharge item, TandTv2Job jobFilter) : base(dbApp, result, item, jobFilter)
        {
        }

        #endregion

        #region methods

        public override TandTv2StepItem Factory_StepItem(TandTv2Result result, TandTv2Step step)
        {
            TandTv2StepItem stepItem = base.Factory_StepItem(result, step);
            stepItem.PrimaryKeyID = Item.FacilityChargeID;
            stepItem.TandT_ItemTypeEnum = TandTv2ItemTypeEnum.FacilityCharge;
            stepItem.FacilityCharge = Item;
            stepItem.InsertDate = Item.InsertDate;
            return stepItem;
        }

        public override string Factory_StepItem_ACIdentifier()
        {
            return string.Format(@"#{0} {1}", Item.FacilityChargeSortNo, Item.FacilityLotID != null ? Item.FacilityLot.LotNo : " - ");
        }

        public override string Factory_StepItem_ACCaptionTranslation()
        {
            return string.Format(@"({0}) [{1}] {2}",
            Item.FacilityLotID != null ? Item.FacilityLot.LotNo : " - ", Item.Material.MaterialNo, Item.Material.MaterialName1);
        }

        public override List<IDoItem> ProcessRelatedSameStep(DatabaseApp dbApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            if (Item.FacilityLotID == null) return null;
            return new List<IDoItem>() { new DoBackward_FacilityLot(dbApp, result, Item.FacilityLot, jobFilter) };
        }

        public override List<IDoItem> SearchRelatedNextStep(DatabaseApp dbApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            IEnumerable<FacilityBookingCharge> related = Item.FacilityLot.FacilityBookingCharge_InwardFacilityLot;
            related = related.Where(c => c.InwardFacilityLotID != null);
            related = related.Where(c=> TandTv2Filters.filterFBCFromToForMaterial(c, jobFilter.MaterialIDs, jobFilter.FilterDateFrom, jobFilter.FilterDateTo));
            return related.Select(c => new DoBackward_FacilityBookingCharge(dbApp, result, c, jobFilter) as IDoItem).ToList();
        }

        #endregion
    }
}
