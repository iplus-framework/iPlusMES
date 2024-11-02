// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Linq;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'FacilityOEEGraphItem'}de{'FacilityOEEGraphItem'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable)]
    public class FacilityOEEGraphItem : IVBChartTupleT<DateTime, double>
    {
        public DateTime ValueT1 { get; set; }

        public double ValueT2 { get; set; }

        public object Value1 { get { return ValueT1; } }
        public object Value2 { get { return ValueT2; } }
    }
}
