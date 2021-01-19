using System.Collections.Generic;

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
