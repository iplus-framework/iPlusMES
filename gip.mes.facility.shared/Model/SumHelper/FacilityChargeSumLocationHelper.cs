// ***********************************************************************
// Assembly         : gip.bso.facility
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="FacilityChargeSumLocationHelper.cs" company="gip mbh, Oftersheim, Germany">
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
    /// Sum per Storage Location
    /// </summary>
    [DataContract(Name = "cFCSLH")]
#if NETFRAMEWORK
    [ACClassInfo(Const.PackName_VarioFacility, "en{'FacilityChargeSumLocationHelper'}de{'FacilityChargeSumLocationHelper'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + "FacilityChargeSumLocationHelper", "en{'FacilityChargeSumLocationHelper'}de{'FacilityChargeSumLocationHelper'}", typeof(FacilityChargeSumLocationHelper), "FacilityChargeSumLocationHelper", "FacilityLocation\\FacilityNo", "FacilityLocation\\FacilityNo")]
#endif
    public class FacilityChargeSumLocationHelper
    {
        /// <summary>
        /// Gets or sets the facility charge sum location helper ID.
        /// </summary>
        /// <value>The facility charge sum location helper ID.</value>
#if NETFRAMEWORK
        [ACPropertyInfo(9999)]
#endif
        [DataMember(Name = "ID")]
        public Guid FacilityChargeSumLocationHelperID { get; set; }
        
        /// <summary>
        /// Gets or sets the facility location.
        /// </summary>
        /// <value>The facility location.</value>
#if NETFRAMEWORK
        [ACPropertyInfo(1, "", "en{'Facility location'}de{'Lagerort'}")]
        public Facility FacilityLocation { get; set; }
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
                return FacilityLocation.FacilityNo;
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
        [DataMember(Name = "FNa")]
        public string FacilityName
        {
            get
            {
#if NETFRAMEWORK
                return FacilityLocation.FacilityName;
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

        public string FacilityLocationNo { get; set; }

        public string FacilityLocationName { get; set; }

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
                return "FacilityLocation\\FacilityNo";
            }
        }
    }
}