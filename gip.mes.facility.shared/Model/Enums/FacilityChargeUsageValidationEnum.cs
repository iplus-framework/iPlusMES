using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using gip.core.datamodel;

namespace gip.mes.facility
{
    [DataContract]
#if NETFRAMEWORK
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Usage Validation of lots'}de{'Verwendungsvalidierung von Losen'}", Global.ACKinds.TACEnum)]
#endif
    public enum FacilityChargeUsageValidationEnum : short
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        FIFO = 10,
        [EnumMember]
        FIFOForced = 20,
        [EnumMember]
        LIFO = 30,
        [EnumMember]
        LIFOForced = 40,
        [EnumMember]
        ExpirationFirst = 50,
        [EnumMember]
        ExpirationFirstForced = 60
    }
}
