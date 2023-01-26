using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{

    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'OperationitemTimeStatusEnum'}de{'OperationitemTimeStatusEnum'}", Global.ACKinds.TACEnum)]
    public enum OperationitemTimeStatusEnum : short
    {
        Elapsing = 0,
        TimeReaching = 1,
        TimeExceeded = 2
    }
}
