// ***********************************************************************
// Assembly         : gip.core.datamodel
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 01-19-2013
// ***********************************************************************
// <copyright file="WeighingTotal.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace gip.mes.datamodel
{
    /// <summary>
    /// Class WeighingTotal
    /// </summary>
    [DataContract]
    public class WeighingTotal
    {
        /// <summary>
        /// News the net weighing.
        /// </summary>
        /// <param name="netWeight">The net weight.</param>
        public void NewNetWeighing(double netWeight) { }
        
        /// <summary>
        /// Starts the new gross weighing.
        /// </summary>
        /// <param name="grossWeight">The gross weight.</param>
        public void StartNewGrossWeighing(double grossWeight) { }
        
        /// <summary>
        /// Ends the current gross weighing.
        /// </summary>
        /// <param name="grossWeight">The gross weight.</param>
        public void EndCurrentGrossWeighing(double grossWeight) { }
        
        /// <summary>
        /// Intermediates the gross weighing.
        /// </summary>
        /// <param name="grossWeight">The gross weight.</param>
        public void IntermediateGrossWeighing(double grossWeight)
        {
            EndCurrentGrossWeighing(grossWeight);
            StartNewGrossWeighing(grossWeight);
        
        }
        /// <summary>
        /// Gets the actual net weight.
        /// </summary>
        /// <param name="currentGrossWeight">The current gross weight.</param>
        /// <returns>System.Double.</returns>
        public double GetActualNetWeight(double currentGrossWeight)
        {
            var query = NetWeighings.Where(c => c.IsFinished == false);
            if (!query.Any())
                return Value;
            if (IsNegativeWeighing)
            {
                return query.First().GetInversActualNetWeight(currentGrossWeight) + Value;
            }
            else
            {
                return query.First().GetActualNetWeight(currentGrossWeight) + Value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is negative weighing.
        /// </summary>
        /// <value><c>true</c> if this instance is negative weighing; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool IsNegativeWeighing { get; set; }
        
        /// <summary>
        /// Gets the start time.
        /// </summary>
        /// <value>The start time.</value>
        [DataMember]
        public DateTime StartTime { get; private set; }
        
        /// <summary>
        /// Gets the end time.
        /// </summary>
        /// <value>The end time.</value>
        [DataMember]
        public DateTime EndTime { get; private set; }
        
        /// <summary>
        /// The net weighings
        /// </summary>
        [DataMember]
        public List<WeighingNet> NetWeighings;
        
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public double Value
        {
            get
            {
                if (IsNegativeWeighing)
                    return NetWeighings.Where(c => c.IsFinished == true).Sum(c =>
                    c.ValueInvers);
                else
                    return NetWeighings.Where(c => c.IsFinished == true).Sum(c => c.Value);
            }
        }
    }
}
