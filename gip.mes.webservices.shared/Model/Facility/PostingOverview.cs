// ***********************************************************************
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