using System;
using System.Collections.Generic;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'PickingPosPreview'}de{'PickingPosPreview'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    public class PickingPosPreview : OrderPosPreviewBase
    {
        [ACPropertyInfo(9999, "PickingNo", "en{'Picking-No.'}de{'Kommissions-Nr.'}")]
        public string PickingNo { get; set; }

        public List<Guid> PickingPosIDs { get; set; }

    }
}
