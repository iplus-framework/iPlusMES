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
// <copyright file="ACFacility.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.autocomponent;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    public static class HelperIFacilityManager
    {
        public const string C_DefaultServiceACIdentifier = "FacilityManager";

        public static IFacilityManager GetServiceInstance(ACComponent requester = null)
        {
            if (requester == null)
                requester = ACRoot.SRoot;
            return PARole.GetServiceInstance<IFacilityManager>(requester, C_DefaultServiceACIdentifier, PARole.CreationBehaviour.OnlyLocal);
        }
    }
}
