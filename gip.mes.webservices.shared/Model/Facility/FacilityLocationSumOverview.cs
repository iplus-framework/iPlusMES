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
    [DataContract(Name = "cFLSO")]
    public class FacilityLocationSumOverview
    {
        [DataMember(Name = "ixMS")]
        public IEnumerable<FacilityChargeSumMaterialHelper> MaterialSum { get; set; }

        [DataMember(Name = "ixFOS")]
        public IEnumerable<FacilityChargeSumLotHelper> FacilityLotSum { get; set; }

        [DataMember(Name = "ixFC")]
        public IEnumerable<FacilityCharge> FacilityCharges { get; set; }
    }
}