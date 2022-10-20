// ***********************************************************************
// Assembly         : gip.core.datamodel
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="IPackagingPos.cs" company="gip mbh, Oftersheim, Germany">
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
    /// HandlingUnit: Interface für Verpackungsposition
    /// </summary>
    public interface IPackagingPos
    {
        /// <summary>
        /// Gets the material.
        /// </summary>
        /// <value>The material.</value>
        Material Material { get; }
        /// <summary>
        /// Gets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        Double Quantity { get; }
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
        /// Gets the using rule attribute.
        /// </summary>
        /// <value>The using rule attribute.</value>
        String UsingRuleAttribute { get; }
        /// <summary>
        /// Gets the is product.
        /// </summary>
        /// <value>The is product.</value>
        Boolean IsProduct { get; }
    }
}
