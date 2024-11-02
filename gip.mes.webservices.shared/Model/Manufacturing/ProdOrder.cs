// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [DataContract(Name = "cPO")]
    public class ProdOrder : EntityBase
    {
        [DataMember(Name = "ID")]
        public Guid ProdOrderID
        {
            get;set;
        }

        [DataMember(Name = "PNo")]
        public string ProgramNo
        {
            get;set;
        }
    }
}
