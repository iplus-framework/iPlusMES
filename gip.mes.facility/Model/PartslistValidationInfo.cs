// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System.Collections.Generic;

namespace gip.mes.facility
{
    public class PartslistValidationInfo
    {
        public List<MapPosToWFConn> MapPosToWFConnections { get; internal set; } = new List<MapPosToWFConn>();
        public bool IsSucceded { get; internal set; }
    }
}
