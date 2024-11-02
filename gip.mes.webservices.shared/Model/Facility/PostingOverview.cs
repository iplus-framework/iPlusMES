// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿// ***********************************************************************
// Assembly         : gip.bso.facility
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="FacilityChargeSumFacilityHelper.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using gip.mes.facility;

namespace gip.mes.webservices
{
    [DataContract(Name = "cPO")]
    public class PostingOverview
    {
        [DataMember(Name = "ixP")]
        public IEnumerable<FacilityBookingOverview> Postings { get; set; }

        [DataMember(Name = "ixPC")]
        public IEnumerable<FacilityBookingChargeOverview> PostingsFBC { get; set; }
    }
}