// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using VD = gip.mes.datamodel;
using System.Collections.Generic;
using System;

namespace gip.mes.facility
{
    public class MaterialPreparationResult
    {

        public List<MaterialPreparationBatchModel> MaterialPreparationBatchModels { get; set; } = new List<MaterialPreparationBatchModel>();
        public List<MaterialPreparationWFGroup> MaterialPreparationWFGroup { get; set; } = new List<MaterialPreparationWFGroup>();

        public List<MaterialPreparationModel> PreparedMaterials { get; set; }

        public Dictionary<Guid, List<VD.Facility>> RoutingResult = new Dictionary<Guid, List<VD.Facility>>();

    }
}
