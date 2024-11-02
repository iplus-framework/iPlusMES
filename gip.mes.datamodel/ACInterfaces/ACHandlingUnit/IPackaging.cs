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
// <copyright file="IPackaging.cs" company="gip mbh, Oftersheim, Germany">
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
    /*
     * MaterialUnit
     * MaterialUnitPackaging        MDPackaging
     * MaterialUnitPackagingPos     MDPackagingPos
     * 
     * 
     * 
    */

    /// <summary>
    /// HandlingUnit: Interface für die  Verpackung
    /// </summary>
    public interface IPackaging
    {
        /// <summary>
        /// Gets the material unit packaging level.
        /// </summary>
        /// <value>The material unit packaging level.</value>
        Int32 MaterialUnitPackagingLevel { get; }
        /// <summary>
        /// Gets the name of the MD packaging.
        /// </summary>
        /// <value>The name of the MD packaging.</value>
        String MDPackagingName { get; }
        /// <summary>
        /// Gets the product quantity.
        /// </summary>
        /// <value>The product quantity.</value>
        Single ProductQuantity { get; }
        /// <summary>
        /// Gets the packaging MD quantity unit.
        /// </summary>
        /// <value>The packaging MD quantity unit.</value>
        MDUnit PackagingMDQuantityUnit { get; }

        /// <summary>
        /// Gets the packaging pos list.
        /// </summary>
        /// <value>The packaging pos list.</value>
        IEnumerable<IPackagingPos> PackagingPosList { get; }
    }

}
