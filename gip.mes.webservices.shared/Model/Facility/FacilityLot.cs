// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace gip.mes.webservices
{
    [DataContract(Name = "cFL")]
    public class FacilityLot
    {
        [DataMember(Name = "ID")]
        public Guid FacilityLotID
        {
            get; set;
        }

        [DataMember(Name = "LN")]
        public string LotNo
        {
            get; set;
        }

        [DataMember(Name = "FD")]
        public DateTime? FillingDate
        {
            get; set;
        }

        [DataMember(Name = "SL")]
        public short StorageLife
        {
            get; set;
        }

        [DataMember(Name = "PD")]
        public DateTime? ProductionDate
        {
            get; set;
        }

        [DataMember(Name = "ED")]
        public DateTime? ExpirationDate
        {
            get; set;
        }

        [DataMember(Name = "ELN")]
        public string ExternLotNo
        {
            get;
            set;
        }

        [DataMember(Name = "CM")]
        public string Comment
        {
            get; set;
        }

    }
}
