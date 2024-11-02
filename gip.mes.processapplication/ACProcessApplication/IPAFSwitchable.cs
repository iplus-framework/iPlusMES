// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    /// <summary>
    /// Represents interface for a process functions that are switchable and contains 'SwitchOff' parameter in the parameter value list.
    /// </summary>
    public interface IPAFSwitchable : IACComponentProcessFunction
    {
    }
}
