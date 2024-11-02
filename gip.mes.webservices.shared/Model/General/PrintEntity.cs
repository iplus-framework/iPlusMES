// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace gip.mes.webservices
{

    [DataContract(Name = "prT")]
    public class PrintEntity
    {
        [DataMember]
        public List<BarcodeEntity> Sequence
        {
            get; set;
        }
        [DataMember]
        public int CopyCount
        {
            get; set;
        }

        [DataMember]
        public int MaxPrintJobsInSpooler
        {
            get; set;
        }
    }
}
