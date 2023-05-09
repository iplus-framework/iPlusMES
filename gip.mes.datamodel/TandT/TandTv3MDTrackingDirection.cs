using gip.core.datamodel;
using System.ComponentModel.DataAnnotations.Schema;

/*
 
TandTv3_TrackingStyle
	TrackingStyleID
*/


namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'TandTv3MDTrackingDirection'}de{'TandTv3MDTrackingDirection'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "")]

    public partial class TandTv3MDTrackingDirection
    {
        [NotMapped]
        public const string ClassName = "TandTv3_MDTrackingDirection";
    }
}
