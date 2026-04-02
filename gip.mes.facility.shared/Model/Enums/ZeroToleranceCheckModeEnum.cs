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
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Zero Tolerance Check Mode'}de{'Nulltoleranz Prüfmodus'}", Global.ACKinds.TACEnum)]
#endif
    public enum ZeroToleranceCheckModeEnum : short
    {
        /// <summary>
        /// Direct value comparison regardless of silo tolerance value
        /// </summary>
        [EnumMember]
        Direct = 0,

        /// <summary>
        /// Absolute tolerance comparison regardless of silo tolerance value
        /// </summary>
        [EnumMember]
        AlwaysAbsolute = 1,

        /// <summary>
        /// Absolute tolerance comparison when silo tolerance <= 0, otherwise direct value comparison
        /// </summary>
        [EnumMember]
        ConditionalAbsolute = 2,
    }
}
