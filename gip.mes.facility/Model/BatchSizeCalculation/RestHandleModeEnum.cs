// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿
namespace gip.mes.facility
{
    /// <summary>
    ///  Define calculation model by calculating 
    ///  with rest of quantity in batch generation
    /// </summary>
    public enum RestHandleModeEnum
    {
        ToFirstBatch,
        ToLastBatch,
        DevideToAllBatches,
        DoNothing
    }
}