using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioFacility, "en{'TandTLotPreviewEnum'}de{'TandTLotPreviewEnum'}", Global.ACKinds.TACEnum)]
    public enum TandTLotPreviewEnum
    {
        All,
        Input,
        Production
    }
}
