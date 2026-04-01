using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using gip.core.datamodel;

namespace gip.mes.facility
{
#if NETFRAMEWORK
    [ACClassInfo(Const.PackName_VarioFacility, "en{'FacilityChargeMobileValidationEnum'}de{'FacilityChargeMobileValidationEnum'}", Global.ACKinds.TACEnum)]
#endif
    public enum FacilityChargeMobileValidationEnum : short
    {
        None = 0,
        FIFO = 10,
        FIFOForced = 20,
        LIFO = 30,
        LIFOForced = 40,
        ExpirationFirst = 50,
        ExpirationFirstForced = 60
    }
}
