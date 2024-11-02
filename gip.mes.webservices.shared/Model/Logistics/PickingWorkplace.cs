// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [DataContract(Name = "cPW")]
    public class PickingWorkplace : Picking
    {
        public PickingWorkplace() : base()
        {
        }

        [DataMember(Name = "WPID")]
        public Guid WorkplaceID
        {
            get;
            set;
        }

    }
}
