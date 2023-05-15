using gip.core.datamodel;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioPurchase, "en{'OutOrderPosPreview'}de{'OutOrderPosPreview'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [NotMapped]
    public class OutOrderPosPreview : OrderPosPreviewBase
    {
        [ACPropertyInfo(9999, "OutOrderNo", "en{'Sales Order No.'}de{'Auftragsnummer'}")]
        [NotMapped]
        public string OutOrderNo { get; set; }
    }
}
