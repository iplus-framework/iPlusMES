// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.mes.datamodel;
using System;
using System.Collections.Generic;

namespace gip.mes.facility
{
    public class PartslistMDSchedulerGroupConnection
    {
        public Guid PartslistID { get; set; }

        public List<MDSchedulingGroup> SchedulingGroups { get; set; }

    }
}
