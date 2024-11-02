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
// <copyright file="HUPosChargeList.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gip.mes.datamodel
{
    /// <summary>
    /// Class HUPosChargeList
    /// </summary>
    public class HUPosChargeList : List<HUPosCharge>
    {
        /// <summary>
        /// HUs the pos charge contains charge.
        /// </summary>
        /// <param name="chargeNo">The charge no.</param>
        /// <returns>HUPosCharge.</returns>
        public HUPosCharge HUPosChargeContainsCharge(string chargeNo)
        {
            var query = this.Where(c => c.ChargeNo == chargeNo);
            if (query.Any())
            {
                return query.First();
            }
            return null;
        }
    }
}
