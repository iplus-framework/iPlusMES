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
        Elapsing = 0, // blue
        NotifyTimeReaching = 1, // yellow
        TimeReached = 2, // green
        TimeExceeded = 3 // red | if maxduration have value
    }
}
