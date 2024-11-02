// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System.Collections.Generic;

namespace gip.mes.facility
{
    /// <summary>
    /// Model prepared for batch creation in cascade
    /// Used to calculate a quantity distribution on cascade
    /// </summary>
    public class IntermediateBatchQuantityConnection
    {
        /// <summary>
        /// Initial percentage model
        /// </summary>
        public List<BatchPercentageModel> BatchPercentageDefinition { get; set; }

        /// <summary>
        /// Calculated quantity model
        /// </summary>
        public List<BatchQuantityModel> BatchQuantityDefinition { get; set; }

        /// <summary>
        /// Wrapper over intermediate
        /// </summary>
        public PosIntermediateDepthWrap IntermediateWarp { get; set; }
    }
}
