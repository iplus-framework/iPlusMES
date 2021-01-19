using gip.core.datamodel;

/*
 TandT_TempPos
	StepID
	ProdOrderPartslistPosID
	MaterialPosTypeIndex
	SourceProdOrderPartslistPosRelationID
	SourceProdOrderPartslistPosID
*/
namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'TempPos'}de{'TempPos'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "")]
    public partial class TandTv2TempPos
    {
        public const string ClassName = "TandTv2TempPos";
        public const string ItemName = "TandTv2TempPos";
    }
}
