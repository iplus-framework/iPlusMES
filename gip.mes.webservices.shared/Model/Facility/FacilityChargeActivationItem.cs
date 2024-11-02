// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [DataContract(Name = "cFCAI")]
    public class FacilityChargeParamItem : FacilityCharge
    {
        public const string FacilityChargeActivationKeyACUrl = "FCActivationItem";

        public FacilityChargeParamItem() : base()
        {

        }

        [DataMember(Name = "wID")]
        public Guid ParamID
        {
            get;
            set;
        }

    }
}
