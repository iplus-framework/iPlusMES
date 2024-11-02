// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Runtime.Serialization;

namespace gip.mes.webservices
{
    [DataContract(Name = "fCSe")]
    public enum FacilityChargeStateEnum
    {
        NotExist,
        QuantNotAvailable,
        Available,
        InDifferentFacility,
        AlreadyFinished
    }
}
