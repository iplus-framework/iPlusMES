// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [CollectionDataContract(Name = "cPPL")]
    public class PickingPosList : List<PickingPos>
    {
        public PickingPosList() : base ()
        {

        }

        public PickingPosList(IEnumerable<PickingPos> collection) : base (collection)
        {

        }
    }
}
