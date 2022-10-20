// ***********************************************************************
// Assembly         : gip.bso.facility
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="FacilityChargeSumLotHelper.cs" company="gip mbh, Oftersheim, Germany">
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
    /// Sum per Lot
    /// </summary>
    [DataContract(Name = "cFCSLH")]
#if NETFRAMEWORK
    [ACClassInfo(Const.PackName_VarioFacility, "en{'FacilityChargeSumLotHelper'}de{'FacilityChargeSumLotHelper'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + "FacilityChargeSumLotHelper", "en{'FacilityChargeSumLotHelper'}de{'FacilityChargeSumLotHelper'}", typeof(FacilityChargeSumLotHelper), "FacilityChargeSumLotHelper", "FacilityLot\\LotNo", "FacilityLot\\LotNo")]
#endif
    public class FacilityChargeSumLotHelper
    {
        /// <summary>
        /// Gets or sets the facility charge sum lot helper ID.
        /// </summary>
        /// <value>The facility charge sum lot helper ID.</value>
#if NETFRAMEWORK
        [ACPropertyInfo(9999)]
#endif
        [DataMember(Name = "ID")]
        public Guid FacilityChargeSumLotHelperID { get; set; }

        /// <summary>
        /// Gets or sets the facility lot.
        /// </summary>
        /// <value>The facility lot.</value>
#if NETFRAMEWORK
        [ACPropertyInfo(1, "", ConstApp.Lot)]
        public FacilityLot FacilityLot { get; set; }
#endif

#if NETFRAMEWORK
        [ACPropertyInfo(2, "", ConstApp.LotNo)]
#elif NETSTANDARD
        private string _FacilityLotNo;
#endif
        [DataMember(Name = "FLNo")]
        public string FacilityLotNo
        {
            get
            {
#if NETFRAMEWORK
                return FacilityLot.LotNo;
#elif NETSTANDARD
                return _FacilityLotNo;
#endif
            }
            set
            {
#if NETSTANDARD
                _FacilityLotNo = value;
#endif
            }
        }


        /// <summary>
        /// Gets or sets the sum total.
        /// </summary>
        /// <value>The sum total.</value>
#if NETFRAMEWORK
        [ACPropertyInfo(3, "", ConstApp.TotalQuantity)]
#endif
        [DataMember(Name = "ST")]
        public double SumTotal { get; set; }

        /// <summary>
        /// Gets or sets the sum blocked.
        /// </summary>
        /// <value>The sum blocked.</value>
#if NETFRAMEWORK
        [ACPropertyInfo(4, "", ConstApp.BlockedQuantity)]
#endif
        [DataMember(Name = "SB")]
        public double SumBlocked { get; set; }

        /// <summary>
        /// Gets or sets the sum blocked absolute.
        /// </summary>
        /// <value>The sum blocked absolute.</value>
#if NETFRAMEWORK
        [ACPropertyInfo(5, "", ConstApp.AbsoluteBlockedQuantity)]
#endif
        [DataMember(Name = "SBA")]
        public double SumBlockedAbsolute { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(6, "", ConstApp.FreeQuantity)]
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
                return "FacilityLot\\LotNo";
            }
        }
    }
}