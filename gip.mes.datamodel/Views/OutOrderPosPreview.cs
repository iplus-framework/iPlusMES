using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioPurchase, "en{'OutOrderPosPreview'}de{'OutOrderPosPreview'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    public class OutOrderPosPreview : OrderPosPreviewBase
    {
        [ACPropertyInfo(9999, "OutOrderNo", "en{'Sales Order No.'}de{'Auftragsnummer'}")]
        public string OutOrderNo { get; set; }
    }
}
