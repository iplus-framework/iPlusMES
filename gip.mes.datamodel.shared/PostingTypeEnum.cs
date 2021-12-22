using gip.core.datamodel;
using System.Runtime.Serialization;

namespace gip.mes.datamodel
{
#if NETFRAMEWORK
    //[ACSerializeableInfo]
#else
        [DataContract]
#endif
    public enum PostingTypeEnum
    {
        NotDefined,
        Inward,
        Outward,
        Relocation
    }
}
