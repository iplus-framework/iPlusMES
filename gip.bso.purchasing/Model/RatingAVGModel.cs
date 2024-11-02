// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.purchasing
{

    [ACClassInfo(Const.PackName_VarioPurchase, "en{'RatingAVGModel'}de{'RatingAVGModel'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class RatingAVGModel
    {
        [ACPropertyInfo(1, "Sn", "en{'Rating'}de{'Bewertung'}")]
        public decimal Score { get; set; }

        [ACPropertyInfo(2, "Sn", "en{'Count'}de{'Anzahl'}")]
        public int Count { get; set; }
    }
}
