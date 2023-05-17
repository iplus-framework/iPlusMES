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
using gip.core.datamodel;

namespace gip.mes.datamodel
{
#if NETFRAMEWORK
    [ACSerializeableInfo]
#endif
    [DataContract]
    public class BarcodeSequenceBase
    {

        #region State

        [DataMember]
        public short StateIndex
        {
            get; set;
        }

        [IgnoreDataMember]
        public ActionState State
        {
            get
            {
                return (ActionState) StateIndex;
            }
            set
            {
                StateIndex = (short) value;
            }
        }

        #endregion

        #region BarcodeIssuer

        [DataMember]
        public short BarcodeIssuerIndex
        {
            get; set;
        }

        [IgnoreDataMember]
        public BarcodeIssuerEnum BarcodeIssuer
        {
            get
            {
                return (BarcodeIssuerEnum) BarcodeIssuerIndex;
            }
            set
            {
                BarcodeIssuerIndex = (short) value;
            }
        }

        #endregion

        [DataMember]
        public core.datamodel.Msg Message
        {
            get; set;
        }

        [DataMember]
        public short QuestionSequence
        {
            get; set;
        }

        public enum ActionState : short
        {
            ScanAgain = 0,
            Cancelled = 1,
            Completed = 2,
            Question = 10,
            Selection = 11,
            FastSelection = 12,
            SelectionScanAgain = 13
        }

        public enum BarcodeIssuerEnum : short
        {
            Production = 0,
            Picking = 10,
            Inventory = 20,
            Facility = 30
        }
    }
}