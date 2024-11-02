// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
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
