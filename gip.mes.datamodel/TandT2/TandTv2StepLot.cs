using gip.core.datamodel;

/*
TandT_StepLot
	StepLotID
	StepID
	LotNo
	FacilityLotID
*/

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'StepLot'}de{'StepLot'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "")]
    [ACPropertyEntity(1, TandTv2Step.ItemName, "en{'Step'}de{'Step'}", Const.ContextDatabase + "\\" + TandTv2Step.ClassName, "", true)]
    [ACPropertyEntity(2, "LotNo", "en{'LotNo'}de{'LotNo'}", "", "", true)]
    public partial class TandTv2StepLot
    {
        public const string ClassName = "TandTv2StepLot";
        public const string ItemName = "TandTv2StepLot";

        #region overrides
        public override string ToString()
        {
            return string.Format(@"StepLot: {0}", LotNo);
        }
        #endregion
    }
}
