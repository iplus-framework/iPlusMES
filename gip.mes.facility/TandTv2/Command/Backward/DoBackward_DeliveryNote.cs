using gip.mes.datamodel;
using iplusContext = gip.core.datamodel;


namespace gip.mes.facility
{
    public class DoBackward_DeliveryNote : DoItem<DeliveryNote>
    {

        #region ctor's

        public DoBackward_DeliveryNote(DatabaseApp dbApp, TandTv2Result result, DeliveryNote item, TandTv2Job jobFilter) : base(dbApp, result, item, jobFilter)
        {

        }

        #endregion

        #region methods

        public override TandTv2StepItem Factory_StepItem(TandTv2Result result, TandTv2Step step)
        {
            TandTv2StepItem stepItem = base.Factory_StepItem(result, step);
            stepItem.PrimaryKeyID = Item.DeliveryNoteID;
            stepItem.TandT_ItemTypeEnum = TandTv2ItemTypeEnum.DeliveryNote;
            stepItem.DeliveryNote = Item;
            stepItem.InsertDate = Item.InsertDate;
            return stepItem;
        }

        public override string Factory_StepItem_ACIdentifier()
        {
            return Item.DeliveryNoteNo;
        }

        public override string Factory_StepItem_ACCaptionTranslation()
        {
            return string.Format(@"en{{'Delivery Address: {0}, Shipper Address: {1}'}}de{{'Lieferadresse: {0}, Speditionsadresse: {1}'}}",
                Item.DeliveryCompanyAddress.Name1, Item.ShipperCompanyAddress.Name1);
        }

        #endregion
    }
}
