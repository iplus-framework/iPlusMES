using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace gip.mes.datamodel
{
    /// <summary>
    /// Class WeighingNet
    /// </summary>
    [DataContract]
    public class WeighingNet
    {
        /// <summary>
        /// Gets the is finished.
        /// </summary>
        /// <value>The is finished.</value>
        public bool IsFinished
        {
            get
            {
                if (StoredNetWeighing != null)
                    return true;
                else if (EndWeighingGross != null && StartWeighingGross != null)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public double Value
        {
            get
            {
                if (!IsFinished)
                    return 0;
                return StoredNetWeighing != null ? StoredNetWeighing.Value :
                EndWeighingGross.Value - StartWeighingGross.Value;
            }
        }

        /// <summary>
        /// Gets the value invers.
        /// </summary>
        /// <value>The value invers.</value>
        public double ValueInvers
        {
            get
            {
                return Value * -1;
            }
        }

        /// <summary>
        /// Gets the invers actual net weight.
        /// </summary>
        /// <param name="currentGrossWeight">The current gross weight.</param>
        /// <returns>System.Double.</returns>
        public double GetInversActualNetWeight(double currentGrossWeight)
        {
            return GetActualNetWeight(currentGrossWeight) * -1;
        }

        /// <summary>
        /// Gets the actual net weight.
        /// </summary>
        /// <param name="currentGrossWeight">The current gross weight.</param>
        /// <returns>System.Double.</returns>
        public double GetActualNetWeight(double currentGrossWeight)
        {
            return currentGrossWeight - StartWeighingGross.Value;
        }

        /// <summary>
        /// Gets or sets the stored net weighing.
        /// </summary>
        /// <value>The stored net weighing.</value>
        [DataMember]
        public WeighingValue StoredNetWeighing { get; set; }

        /// <summary>
        /// Gets the first gross weighings.
        /// </summary>
        /// <value>The first gross weighings.</value>
        [DataMember]
        public List<WeighingValue> FirstGrossWeighings { get; private set; }

        /// <summary>
        /// Gets the start weighing gross.
        /// </summary>
        /// <value>The start weighing gross.</value>
        public WeighingValue StartWeighingGross
        {
            get
            {
                return FirstGrossWeighings.Last();
            }
        }

        /// <summary>
        /// Gets or sets the end weighing gross.
        /// </summary>
        /// <value>The end weighing gross.</value>
        [DataMember]
        public WeighingValue EndWeighingGross { get; set; }
    }
}
