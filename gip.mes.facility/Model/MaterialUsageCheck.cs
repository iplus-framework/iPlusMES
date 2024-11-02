// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.mes.datamodel;

namespace gip.mes.facility
{
    public class MaterialUsageCheck
    {
        public Material Material { get; set; }
        public double OutwardQuantityUOM { get; set; }
        public double InwardQuantityUOM { get; set; }

        public bool IsQuantityValid
        {
            get
            {
                return OutwardQuantityUOM >= InwardQuantityUOM;
            }
        }
    }
}
