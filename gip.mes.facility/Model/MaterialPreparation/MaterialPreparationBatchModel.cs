// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.mes.datamodel;
using System;
using System.Collections.Generic;

namespace gip.mes.facility
{
    public class MaterialPreparationBatchModel
    {
        public Guid ProdOrderBatchPlanID { get; set; }
        public ProdOrderPartslistPos SourceProdOrderPartslistPos { get; set; }
        public string MaterialNo { get; set; }
        public double TargetQuantityUOM { get; set; }

        public Guid? MDSchedulingGroupID { get; set; }

        public Guid[] FacilitiesOnRouteIds { get; set; }

        public List<string> TargetFacilityNos {  get; set; } = new List<string>();
    }
}
