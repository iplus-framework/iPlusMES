using gip.core.datamodel;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'TandTv3MDBookingDirection'}de{'TandTv3MDBookingDirection'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "")]
    public partial class TandTv3MDBookingDirection
    {
        [NotMapped]
        public const string ClassName = "TandTv3MDBookingDirection";
    }
}
