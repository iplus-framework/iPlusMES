using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioFacility, "en{'TandTStartPointEnum'}de{'TandTStartPointEnum'}", Global.ACKinds.TACEnum)]
    public enum TandTStartPointEnum
    {
        FacilityBooking,
        FacilityPreBooking,
        InOrderPos,
        OutOrderPos,
        DeliveryNotePos
    }
}
