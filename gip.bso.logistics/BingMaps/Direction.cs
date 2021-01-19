// ***********************************************************************
// Assembly         : gip.bso.logistics
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="Direction.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using Microsoft.Maps.MapControl.WPF;
using gip.mes.datamodel; using gip.core.datamodel;

namespace gip.bso.logistics
{
    /// <summary>
    /// Class Direction
    /// </summary>
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Direction'}de{'Direction'}", Global.ACKinds.TACClass)]
    public class Direction
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public Location Location { get; set; }
    }
}

