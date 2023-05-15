using gip.core.datamodel;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioPurchase, "en{'Purchase Order Number'}de{'Bestellnummer'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [NotMapped]
    public class InOrderPosPreview : OrderPosPreviewBase
    {
        [ACPropertyInfo(9999, "InOrderNo", "en{'Sales Order No.'}de{'Auftragsnummer'}")]
        [NotMapped]
        public string InOrderNo { get; set; }
    }
}
