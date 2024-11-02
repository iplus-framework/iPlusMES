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
// <copyright file="FacilityChargeSumMaterialHelper.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
#if NETFRAMEWORK
using gip.core.datamodel;
using gip.mes.datamodel;
#endif
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace gip.mes.facility
{
    /// <summary>
    /// Sum per Material
    /// </summary>
    [DataContract(Name = "cFCSMH")]
#if NETFRAMEWORK
    [ACClassInfo(Const.PackName_VarioFacility, "en{'FacilityChargeSumMaterialHelper'}de{'FacilityChargeSumMaterialHelper'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + "FacilityChargeSumMaterialHelper", "en{'FacilityChargeSumMaterialHelper'}de{'FacilityChargeSumMaterialHelper'}", typeof(FacilityChargeSumMaterialHelper), "FacilityChargeSumMaterialHelper", "Material\\MaterialNo", "Material\\MaterialNo")]
#endif
    public class FacilityChargeSumMaterialHelper
    {
        /// <summary>
        /// Gets or sets the facility charge sum material helper ID.
        /// </summary>
        /// <value>The facility charge sum material helper ID.</value>
#if NETFRAMEWORK
        [ACPropertyInfo(9999)]
#endif
        [DataMember(Name = "ID")]
        public Guid FacilityChargeSumMaterialHelperID { get; set; }

        /// <summary>
        /// Gets or sets the material.
        /// </summary>
        /// <value>The material.</value>
#if NETFRAMEWORK
        [ACPropertyInfo(1, "", ConstApp.Material)]
        public Material Material { get; set; }
#endif

#if NETFRAMEWORK
        [ACPropertyInfo(2, "", ConstApp.MaterialNo)]
#elif NETSTANDARD
        private string _MaterialNo;
#endif
        [DataMember(Name = "MNo")]
        public string MaterialNo
        {
            get
            {
#if NETFRAMEWORK
                return Material.MaterialNo;
#elif NETSTANDARD
                return _MaterialNo;
#endif
            }
            set
            {
#if NETSTANDARD
                _MaterialNo = value;
#endif
            }
        }

#if NETFRAMEWORK
        [ACPropertyInfo(3, "", ConstApp.MaterialName1)]
#elif NETSTANDARD
        private string _MaterialName;
#endif
        [DataMember(Name = "MNa")]
        public string MaterialName
        {
            get
            {
#if NETFRAMEWORK
                return Material.MaterialName1;
#elif NETSTANDARD
                return _MaterialName;
#endif
            }
            set
            {
#if NETSTANDARD
                _MaterialName = value;
#endif
            }
        }


        /// <summary>
        /// Gets or sets the sum total.
        /// </summary>
        /// <value>The sum total.</value>
#if NETFRAMEWORK
        [ACPropertyInfo(4, "", ConstApp.TotalQuantity)]
#endif
        [DataMember(Name = "ST")]
        public double SumTotal { get; set; }

        /// <summary>
        /// Gets or sets the sum blocked.
        /// </summary>
        /// <value>The sum blocked.</value>
#if NETFRAMEWORK
        [ACPropertyInfo(5, "", ConstApp.BlockedQuantity)]
#endif
        [DataMember(Name = "SB")]
        public double SumBlocked { get; set; }

        /// <summary>
        /// Gets or sets the sum blocked absolute.
        /// </summary>
        /// <value>The sum blocked absolute.</value>
#if NETFRAMEWORK
        [ACPropertyInfo(6, "", ConstApp.AbsoluteBlockedQuantity)]
#endif
        [DataMember(Name = "SBA")]
        public double SumBlockedAbsolute { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(7, "", ConstApp.FreeQuantity)]
#endif
        [DataMember(Name = "SF")]
        public double SumFree { get; set; }

        /// <summary>
        /// Gets the key AC identifier.
        /// </summary>
        /// <value>The key AC identifier.</value>
        static public string KeyACIdentifier
        {
            get
            {
                return "Material\\MaterialNo";
            }
        }
    }
}