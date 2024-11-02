// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿// ***********************************************************************
// Assembly         : gip.core.datamodel
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="HUPosCharge.cs" company="gip mbh, Oftersheim, Germany">
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
    /// Class HUPosCharge
    /// </summary>
    [DataContract]
    public class HUPosCharge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HUPosCharge"/> class.
        /// </summary>
        /// <param name="chargeNo">The charge no.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="weight">The weight.</param>
        public HUPosCharge(string chargeNo, Double quantity, Double weight)
        {
            ChargeNo = chargeNo;
            Quantity = quantity;
            Weight = weight;
        }

        /// <summary>
        /// Chargennummer
        /// </summary>
        /// <value>The charge no.</value>
        [DataMember]
        public string ChargeNo { get; set; }

        /// <summary>
        /// Mengenangabe des Materials
        /// </summary>
        /// <value>The quantity.</value>
        [DataMember]
        public Double Quantity { get; set; }

        /// <summary>
        /// Gewichtangabe des Materials
        /// </summary>
        /// <value>The weight.</value>
        [DataMember]
        public Double Weight { get; set; }
    }
}
