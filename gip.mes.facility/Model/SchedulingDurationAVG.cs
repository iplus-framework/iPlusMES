// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿namespace gip.mes.facility
{
    public class SchedulingDurationAVG
    {
        public double? StartOffsetSecAVG { get; set; }
        public double? DurationSecAVG { get; set; }

        public override string ToString()
        {
            return string.Format(@"Duration | Offset: [{0}|{1}]", DurationSecAVG, StartOffsetSecAVG);
        }
    }
}
