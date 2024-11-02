// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [DataContract(Name = "cPOPL")]
    public class Partslist : EntityBase
    {
        [DataMember(Name = "ID")]
        public Guid PartlistID
        {
            get;set;
        }

        [DataMember(Name = "MT")]
        public Material Material
        {
            get;set;
        }

        [DataMember(Name = "PNo")]
        public string PartslistNo
        {
            get;set;
        }

        [DataMember(Name = "PN")]
        public string PartslistName
        {
            get;set;
        }

        [DataMember(Name = "PV")]
        public string PartslistVersion
        {
            get;set;
        }
    }
}
