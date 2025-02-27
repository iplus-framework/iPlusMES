// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using gip.core.datamodel;


namespace gip.mes.facility
{
    public class MaterialPreparationWFGroup
    {

        public Guid MaterialWFID { get; set; } = Guid.NewGuid();
        public List<ACClassWF> ACClassWFs { get; set; } = new List<ACClassWF>();

        public string IntermediateMaterialNo { get; set; }
        public List<MaterialPreparationAllowedInstances> AllowedInstances { get; set; } = new List<MaterialPreparationAllowedInstances>();

        public List<string> OutwardMaterials { get; set; } = new List<string>();

    }
}
