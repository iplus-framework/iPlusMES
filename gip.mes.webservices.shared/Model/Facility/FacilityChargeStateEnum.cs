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