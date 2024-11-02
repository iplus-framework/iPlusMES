// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'BatchPercentageModel'}de{'BatchPercentageModel'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class BatchPercentageModel
    {
        [ACPropertyInfo(1, "Sequence", "en{'Sequence'}de{'Reihenfolge'}")]
        public int Sequence { get; set; }

        [ACPropertyInfo(2, "Percentage", "en{'Percentage'}de{'Prozentsatz'}")]
        public double Percentage { get; set; }
    }
}
