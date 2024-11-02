// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;

namespace gip.mes.facility
{
    public class RemotePickingInfo
    {
        public long RowNr { get; set; }

        public string PickingNo { get; set; }
        public DateTime DeliveryDateFrom { get; set; }
        public DateTime InsertDate { get; set; }
        public string InsertName { get; set; }
        public DateTime? PosStartUpdate { get; set; }
        public DateTime? PosEndUpdate { get; set; }
        public DateTime? FBStartUpdate { get; set; }
        public DateTime? FBEndUpdate { get; set; }

        public bool? IsSuccessfullyClosed { get; set; }
    }
}
