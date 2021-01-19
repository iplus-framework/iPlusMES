using gip.core.datamodel;
using System;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'StepItem'}de{'StepItem'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "")]
    [ACPropertyEntity(1, TandTv2Step.ItemName, "en{'Step'}de{'Step'}", Const.ContextDatabase + "\\" + TandTv2Step.ClassName, "", true)]
    [ACPropertyEntity(2, TandTv2ItemType.ItemName, "en{'ItemType'}de{'ItemType'}", Const.ContextDatabase + "\\" + TandTv2ItemType.ClassName, "", true)]
    [ACPropertyEntity(3, TandTv2Operation.ItemName, "en{'Operation'}de{'Operation'}", Const.ContextDatabase + "\\" + TandTv2Operation.ClassName, "", true)]
    [ACPropertyEntity(4, "SubStepNo", "en{'Key'}de{'Schlüssel'}", "", "", true)]
    [ACPropertyEntity(5, "PrimaryKeyID", "en{'Title'}de{'Title'}", "", "", true)]
    [ACPropertyEntity(7, Const.ACIdentifierPrefix, "en{'ID'}de{'ID'}", "", "", true)]
    [ACPropertyEntity(8, "ACCaptionTranslation", "en{'ACCaptionTranslation'}de{'ACCaptionTranslation'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]

    [ACPropertyEntity(999, core.datamodel.ACClass.ClassName, "en{'ACClass'}de{'ACClass'}", Const.ContextDatabase + "\\" + core.datamodel.ACClass.ClassName, "", true)]
    [ACPropertyEntity(999, ProdOrder.ClassName, "en{'ProdOrder'}de{'ProdOrder'}", Const.ContextDatabase + "\\" + ProdOrder.ClassName, "", true)]
    [ACPropertyEntity(999, ProdOrderPartslist.ClassName, "en{'ProdOrderPartslist'}de{'ProdOrderPartslist'}", Const.ContextDatabase + "\\" + ProdOrderPartslist.ClassName, "", true)]
    [ACPropertyEntity(999, ProdOrderPartslistPos.ClassName, "en{'ProdOrderPartslistPos'}de{'ProdOrderPartslistPos'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPos.ClassName, "", true)]
    [ACPropertyEntity(999, ProdOrderPartslistPosRelation.ClassName, "en{'ProdOrderPartslistPosRelation'}de{'ProdOrderPartslistPosRelation'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPosRelation.ClassName, "", true)]
    [ACPropertyEntity(999, FacilityBooking.ClassName, "en{'FacilityBooking'}de{'FacilityBooking'}", Const.ContextDatabase + "\\" + FacilityBooking.ClassName, "", true)]
    [ACPropertyEntity(999, FacilityBookingCharge.ClassName, "en{'FacilityBookingCharge'}de{'FacilityBookingCharge'}", Const.ContextDatabase + "\\" + FacilityBookingCharge.ClassName, "", true)]
    [ACPropertyEntity(999, FacilityCharge.ClassName, "en{'FacilityCharge'}de{'FacilityCharge'}", Const.ContextDatabase + "\\" + FacilityCharge.ClassName, "", true)]
    [ACPropertyEntity(999, FacilityLot.ClassName, "en{'FacilityLot'}de{'FacilityLot'}", Const.ContextDatabase + "\\" + FacilityLot.ClassName, "", true)]
    [ACPropertyEntity(999, DeliveryNote.ClassName, "en{'DeliveryNote'}de{'DeliveryNote'}", Const.ContextDatabase + "\\" + DeliveryNote.ClassName, "", true)]
    [ACPropertyEntity(999, DeliveryNotePos.ClassName, "en{'DeliveryNotePos'}de{'DeliveryNotePos'}", Const.ContextDatabase + "\\" + DeliveryNotePos.ClassName, "", true)]
    [ACPropertyEntity(999, InOrder.ClassName, "en{'InOrder'}de{'InOrder'}", Const.ContextDatabase + "\\" + InOrder.ClassName, "", true)]
    [ACPropertyEntity(999, InOrderPos.ClassName, "en{'InOrderPos'}de{'InOrderPos'}", Const.ContextDatabase + "\\" + InOrderPos.ClassName, "", true)]
    [ACPropertyEntity(999, OutOrder.ClassName, "en{'OutOrder'}de{'OutOrder'}", Const.ContextDatabase + "\\" + OutOrder.ClassName, "", true)]
    [ACPropertyEntity(999, OutOrderPos.ClassName, "en{'OutOrderPos'}de{'OutOrderPos'}", Const.ContextDatabase + "\\" + OutOrderPos.ClassName, "", true)]
    [ACPropertyEntity(999, Facility.ClassName, "en{'Facility'}de{'Facility'}", Const.ContextDatabase + "\\" + Facility.ClassName, "", true)]
    [ACPropertyEntity(999, FacilityPreBooking.ClassName, "en{'FacilityPreBooking'}de{'FacilityPreBooking'}", Const.ContextDatabase + "\\" + FacilityPreBooking.ClassName, "", true)]
    public partial class TandTv2StepItem
    {
        #region constants
        public const string ClassName = "TandTv2StepItem";
        public const string ItemName = "TandTv2StepItem";

        #endregion

        #region Enum Properties for external lookups

        public TandTv2ItemTypeEnum TandT_ItemTypeEnum
        {
            get
            {
                return (TandTv2ItemTypeEnum)Enum.Parse(typeof(TandTv2ItemTypeEnum), TandTv2ItemTypeID);
            }
            set
            {
                TandTv2ItemTypeID = Enum.GetName(typeof(TandTv2ItemTypeEnum), value);
            }
        }

        public TandTv2OperationEnum TandT_OperationEnum
        {
            get
            {
                return (TandTv2OperationEnum)Enum.Parse(typeof(TandTv2OperationEnum), TandTv2OperationID);
            }
            set
            {
                TandTv2OperationID = Enum.GetName(typeof(TandTv2OperationEnum), value);
            }
        }

        #endregion

        #region IACObject

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(999, "ACCaption", "en{'Description'}de{'Bezeichnung'}")]
        public override string ACCaption
        {
            get
            {
                return Translator.GetTranslation(ACIdentifier, ACCaptionTranslation);
            }
            set
            {
                ACCaptionTranslation = Translator.SetTranslation(ACCaptionTranslation, value);
            }
        }

        #endregion


        #region Overrides

        public override string ToString()
        {
            return string.Format(@"[{0}] | ACIdentifier:{1} | ACCaption:{2})", TandTv2ItemTypeID, ACIdentifier, ACCaption);
        }

        #endregion
    }
}
