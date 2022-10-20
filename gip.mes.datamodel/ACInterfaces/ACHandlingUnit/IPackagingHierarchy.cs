// ***********************************************************************
// Assembly         : gip.core.datamodel
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="IPackagingHierarchy.cs" company="gip mbh, Oftersheim, Germany">
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
    /// HandlingUnit: Interface für Verpackungshierarchie
    /// </summary>
    public interface IPackagingHierarchy
    {
        /// <summary>
        /// Gets the name of the packaging hierarchy.
        /// </summary>
        /// <value>The name of the packaging hierarchy.</value>
        String PackagingHierarchyName { get; }

        /// <summary>
        /// Gets the MD quantity unit.
        /// </summary>
        /// <value>The MD quantity unit.</value>
        MDUnit MDQuantityUnit { get; }
        /// <summary>
        /// Gets the MD weight unit.
        /// </summary>
        /// <value>The MD weight unit.</value>
        MDUnit MDWeightUnit { get; }
        /// <summary>
        /// Gets the net weight.
        /// </summary>
        /// <value>The net weight.</value>
        Single NetWeight { get; }
        /// <summary>
        /// Gets the gross weight.
        /// </summary>
        /// <value>The gross weight.</value>
        Single GrossWeight { get; }

        /// <summary>
        /// Gets the packaging factor.
        /// </summary>
        /// <value>The packaging factor.</value>
        Single PackagingFactor { get; }
        /// <summary>
        /// Gets the total packaging quantity.
        /// </summary>
        /// <value>The total packaging quantity.</value>
        Single TotalPackagingQuantity { get; }
        /// <summary>
        /// Gets the total packaging gross weight.
        /// </summary>
        /// <value>The total packaging gross weight.</value>
        Single TotalPackagingGrossWeight { get; }
        /// <summary>
        /// Gets the total packaging net weight.
        /// </summary>
        /// <value>The total packaging net weight.</value>
        Single TotalPackagingNetWeight { get; }

        /// <summary>
        /// Gets the packaging list.
        /// </summary>
        /// <value>The packaging list.</value>
        IEnumerable<IPackaging> PackagingList { get; }
    }
}
