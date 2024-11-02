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
using gip.core.datamodel;

namespace gip.mes.webservices
{
    [DataContract(Name = "cMSO")]
    public class MaterialSumOverview : EntityBase
    {
        [DataMember(Name = "xMS")]
        public MaterialStock MaterialStock { get; set; }

        [DataMember(Name = "ixFS")]
        public IEnumerable<FacilityChargeSumFacilityHelper> FacilitySum { get; set; }

        [DataMember(Name = "ixFLS")]
        public IEnumerable<FacilityChargeSumLocationHelper> FacilityLocationSum { get; set; }

        [DataMember(Name = "ixFOS")]
        public IEnumerable<FacilityChargeSumLotHelper> FacilityLotSum { get; set; }

        [DataMember(Name = "ixFC")]
        public IEnumerable<FacilityCharge> FacilityCharges { get; set; }

        public void OnFacilityChargesChanged()
        {
            OnPropertyChanged("FacilityCharges");
        }
    }
}