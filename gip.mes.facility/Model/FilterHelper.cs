// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility
{
    public static class FilterHelper
    {
        public static IEnumerable<T> IsInPeriod<T>(this IEnumerable<T> qvr, TandTFilter filter) where T : IInsertInfo
        {
            if (filter.StartTime != null)
                qvr = qvr.Where(x => x.InsertDate >= filter.StartTime.Value);
            if (filter.EndTime != null)
                qvr = qvr.Where(x => x.InsertDate < filter.EndTime.Value);
            return qvr;
        }

        public static bool IsInPeriod(this IInsertInfo item, TandTFilter filter)
        {
            return (filter.StartTime == null || item.InsertDate >= filter.StartTime.Value)
                && (filter.EndTime == null || item.InsertDate < filter.EndTime.Value);
        }
    }
}
