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
#if NETFRAMEWORK
using gip.mes.datamodel;
using gip.core.datamodel;
#endif
using System.Runtime.Serialization;

namespace gip.mes.facility
{
    /// <summary>
    /// Sum per Storage bin
    /// </summary>
    [DataContract(Name = "cFCSFH")]
#if NETFRAMEWORK
    [ACClassInfo(Const.PackName_VarioFacility, "en{'FacilityChargeSumFacilityHelper'}de{'FacilityChargeSumFacilityHelper'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + "FacilityChargeSumFacilityHelper", "en{'FacilityChargeSumFacilityHelper'}de{'FacilityChargeSumFacilityHelper'}", typeof(FacilityChargeSumFacilityHelper), "FacilityChargeSumFacilityHelper", "Facility\\FacilityNo", "Facility\\FacilityNo")]
#endif
    public class FacilityChargeSumFacilityHelper
    {
        /// <summary>
        /// Gets or sets the facility charge sum facility helper ID.
        /// </summary>
        /// <value>The facility charge sum facility helper ID.</value>
#if NETFRAMEWORK
        [ACPropertyInfo(9999)]
#endif
        [DataMember(Name = "ID")]
        public Guid FacilityChargeSumFacilityHelperID { get; set; }

        /// <summary>
        /// Gets or sets the facility.
        /// </summary>
        /// <value>The facility.</value>
#if NETFRAMEWORK
        [ACPropertyInfo(1, "", ConstApp.Facility)]
        public Facility Facility { get; set; }
#endif

#if NETFRAMEWORK
        [ACPropertyInfo(2, "", ConstApp.Number)]
#elif NETSTANDARD
        private string _FacilityNo;
#endif
        [DataMember(Name = "FNo")]
        public string FacilityNo
        {
            get
            {
#if NETFRAMEWORK
                return Facility.FacilityNo;
#elif NETSTANDARD
                return _FacilityNo;
#endif
            }
            set
            {
#if NETSTANDARD
                _FacilityNo = value;
#endif
            }
        }

#if NETFRAMEWORK
        [ACPropertyInfo(3, "", ConstApp.Name)]
#elif NETSTANDARD
        private string _FacilityName;
#endif
        [DataMember(Name = "FN")]
        public string FacilityName
        {
            get
            {
#if NETFRAMEWORK
                return Facility.FacilityName;
#elif NETSTANDARD
                return _FacilityName;
#endif
            }
            set
            {
#if NETSTANDARD
                _FacilityName = value;
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


#if NETFRAMEWORK
        [ACPropertyInfo(8, "", ConstApp.NewPlannedStock)]
#endif
        [DataMember(Name = "NPS")]
        public double NewPlannedStock { get; set; }

        /// <summary>
        /// Gets the key AC identifier.
        /// </summary>
        /// <value>The key AC identifier.</value>
        static public string KeyACIdentifier
        {
            get
            {
                return "Facility\\FacilityNo";
            }
        }
    }
}