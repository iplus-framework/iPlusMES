using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'PickingPosPreview'}de{'PickingPosPreview'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [NotMapped]
    public class PickingPosPreview : OrderPosPreviewBase
    {
        [ACPropertyInfo(9999, "PickingNo", "en{'Picking-No.'}de{'Kommissions-Nr.'}")]
        [NotMapped]
        public string PickingNo { get; set; }

        [NotMapped]
        public List<Guid> PickingPosIDs { get; set; }

    }
}
