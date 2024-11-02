// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    /// <summary>
    /// Intermediate selected for batch creation
    /// Contains info about depth in tree, selection status and used quantity (different for own TargetQuantity in case when is used as part of target intermediate)
    /// </summary>
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'CreateBatchCalc'}de{'CreateBatchCalc'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class PosIntermediateDepthWrap
    {

        /// <summary>
        /// Tree depth
        /// </summary>
        [ACPropertyInfo(1, "Depth", "en{'Depth'}de{'Tief'}")]
        public int Depth { get; set; }

        [ACPropertyInfo(3, "IntermediateItem", "en{'IntermediateItem'}de{'IntermediateItem'}")]
        public ProdOrderPartslistPos Intermediate { get; set; }

        [ACPropertyInfo(3, "Selected", "en{'Selected'}de{'Ausgewählt'}")]
        public bool Selected { get; set; }

        /// <summary>
        /// Quantity used in target intermediate
        /// if is last intermediate in chain - own TargetQuantity
        /// </summary>
        public double TargetQuantityUOM { get; set; }

    }
}
