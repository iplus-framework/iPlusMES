// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;

namespace gip.mes.facility
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioFacility, "en{'SchedulingForecastModeEnum'}de{'SchedulingForecastModeEnum'}", Global.ACKinds.TACEnum)]
    public enum SchedulingForecastModeEnum
    {
        LinearAverage,
        AverageWithoutDeviateValues
    }
}
