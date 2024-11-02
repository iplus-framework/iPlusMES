// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace gip.mes.webservices
{
    [DataContract(Name = "cBPm")]
    public class ACMethodBooking : EntityBase
    {
        [DataMember(Name = "ID")]
        public Guid BookingParamID
        {
            get; set;
        }

        #region Common Paaramters
        string _VirtualMethodName;
        [DataMember(Name = "VMN")]
        public string VirtualMethodName
        {
            get { return _VirtualMethodName; }
            set { SetProperty<string>(ref _VirtualMethodName, value); }
        }

        bool _ShiftBookingReverse;
        [DataMember(Name = "SFBR")]
        public bool ShiftBookingReverse
        {
            get { return _ShiftBookingReverse; }
            set { SetProperty<bool>(ref _ShiftBookingReverse, value); }
        }

        short _BookingTypeIndex;
        [DataMember(Name = "BTpI")]
        public short BookingTypeIndex
        {
            get { return _BookingTypeIndex; }
            set { SetProperty<short>(ref _BookingTypeIndex, value); OnPropertyChanged("BookingType"); }
        }

        [IgnoreDataMember]
        public GlobalApp.FacilityBookingType BookingType
        {
            get { return (GlobalApp.FacilityBookingType) BookingTypeIndex; }
            set { BookingTypeIndex = (short) value; }
        }

        bool? _DontAllowNegativeStock;
        [DataMember(Name = "DANS")]
        public bool? DontAllowNegativeStock
        {
            get { return _DontAllowNegativeStock; }
            set { SetProperty<bool?>(ref _DontAllowNegativeStock, value); }
        }

        bool? _IgnoreManagement;
        [DataMember(Name = "IgMn")]
        public bool? IgnoreManagement
        {
            get { return _IgnoreManagement; }
            set { SetProperty<bool?>(ref _IgnoreManagement, value); }
        }

        bool? _QuantityIsAbsolute;
        [DataMember(Name = "QIA")]
        public bool? QuantityIsAbsolute
        {
            get { return _QuantityIsAbsolute; }
            set { SetProperty<bool?>(ref _QuantityIsAbsolute, value); }
        }

        short? _BalancingModeIndex;
        [DataMember(Name = "BMI")]
        public short? BalancingModeIndex
        {
            get { return _BalancingModeIndex; }
            set { SetProperty<short?>(ref _BalancingModeIndex, value); }
        }

        short? _ZeroStockStateIndex;
        [DataMember(Name = "ZSTI")]
        public short? ZeroStockStateIndex
        {
            get { return _ZeroStockStateIndex; }
            set { SetProperty<short?>(ref _ZeroStockStateIndex, value); }
        }

        short? _ReleaseStateIndex;
        [DataMember(Name = "RLI")]
        public short? ReleaseStateIndex
        {
            get { return _ReleaseStateIndex; }
            set { SetProperty<short?>(ref _ReleaseStateIndex, value); }
        }

        bool _IgnoreIsEnabled;
        [DataMember(Name = "IgEn")]
        public bool IgnoreIsEnabled
        {
            get { return _IgnoreIsEnabled; }
            set { SetProperty<bool>(ref _IgnoreIsEnabled, value); }
        }

        bool _SetCompleted;
        [DataMember(Name = "StCp")]
        public bool SetCompleted
        {
            get { return _SetCompleted; }
            set { SetProperty<bool>(ref _SetCompleted, value); }
        }

        short? _ReservationModeIndex;
        [DataMember(Name = "ReMI")]
        public short? ReservationModeIndex
        {
            get { return _ReservationModeIndex; }
            set { SetProperty<short?>(ref _ReservationModeIndex, value); }
        }

        short? _MovementReasonIndex;
        [DataMember(Name = "MoRI")]
        public short? MovementReasonIndex
        {
            get { return _MovementReasonIndex; }
            set { SetProperty<short?>(ref _MovementReasonIndex, value); }
        }

        private Guid? _MovementReasonID;
        [DataMember(Name = "MoRID")]
        public Guid? MovementReasonID
        {
            get => _MovementReasonID;
            set => SetProperty(ref _MovementReasonID, value);
        }

        #endregion

        #region Inward

        Guid? _InwardMaterialID;
        [DataMember(Name = "IMID")]
        public Guid? InwardMaterialID
        {
            get { return _InwardMaterialID; }
            set { SetProperty<Guid?>(ref _InwardMaterialID, value); }
        }

        Guid? _InwardFacilityID;
        [DataMember(Name = "IFID")]
        public Guid? InwardFacilityID
        {
            get { return _InwardFacilityID; }
            set { SetProperty<Guid?>(ref _InwardFacilityID, value); }
        }

        Guid? _InwardFacilityLotID;
        [DataMember(Name = "IFLTID")]
        public Guid? InwardFacilityLotID
        {
            get { return _InwardFacilityLotID; }
            set { SetProperty<Guid?>(ref _InwardFacilityLotID, value); }
        }

        Guid? _InwardFacilityChargeID;
        [DataMember(Name = "IFCID")]
        public Guid? InwardFacilityChargeID
        {
            get { return _InwardFacilityChargeID; }
            set { SetProperty<Guid?>(ref _InwardFacilityChargeID, value); }
        }

        Guid? _InwardFacilityLocationID;
        [DataMember(Name = "IFLOID")]
        public Guid? InwardFacilityLocationID
        {
            get { return _InwardFacilityLocationID; }
            set { SetProperty<Guid?>(ref _InwardFacilityLocationID, value); }
        }

        Int32? _InwardSplitNo;
        [DataMember(Name = "ISNo")]
        public Int32? InwardSplitNo
        {
            get { return _InwardSplitNo; }
            set { SetProperty<Int32?>(ref _InwardSplitNo, value); }
        }

        Guid? _InwardPartslistID;
        [DataMember(Name = "IPLID")]
        public Guid? InwardPartslistID
        {
            get { return _InwardPartslistID; }
            set { SetProperty<Guid?>(ref _InwardPartslistID, value); }
        }

        Guid? _InwardCompanyMaterialID;
        [DataMember(Name = "ICMID")]
        public Guid? InwardCompanyMaterialID
        {
            get { return _InwardCompanyMaterialID; }
            set { SetProperty<Guid?>(ref _InwardCompanyMaterialID, value); }
        }

        Double? _InwardQuantity;
        [DataMember(Name = "IQ")]
        public Double? InwardQuantity
        {
            get { return _InwardQuantity; }
            set { SetProperty<Double?>(ref _InwardQuantity, value); }
        }

        Double? _InwardTargetQuantity;
        [DataMember(Name = "ITQ")]
        public Double? InwardTargetQuantity
        {
            get { return _InwardTargetQuantity; }
            set { SetProperty<Double?>(ref _InwardTargetQuantity, value); }
        }

        private int? _InwardAutoSplitQuant;
        [DataMember(Name = "IASQ")]
        public int? InwardAutoSplitQuant
        {
            get => _InwardAutoSplitQuant;
            set => SetProperty<int?>(ref _InwardAutoSplitQuant, value);
        }

        #endregion


        #region Outward

        Guid? _OutwardMaterialID;
        [DataMember(Name = "OMID")]
        public Guid? OutwardMaterialID
        {
            get { return _OutwardMaterialID; }
            set { SetProperty<Guid?>(ref _OutwardMaterialID, value); }
        }

        Guid? _OutwardFacilityID;
        [DataMember(Name = "OFID")]
        public Guid? OutwardFacilityID
        {
            get { return _OutwardFacilityID; }
            set { SetProperty<Guid?>(ref _OutwardFacilityID, value); }
        }

        Guid? _OutwardFacilityLotID;
        [DataMember(Name = "OFLTID")]
        public Guid? OutwardFacilityLotID
        {
            get { return _OutwardFacilityLotID; }
            set { SetProperty<Guid?>(ref _OutwardFacilityLotID, value); }
        }

        Guid? _OutwardFacilityChargeID;
        [DataMember(Name = "OFCID")]
        public Guid? OutwardFacilityChargeID
        {
            get { return _OutwardFacilityChargeID; }
            set { SetProperty<Guid?>(ref _OutwardFacilityChargeID, value); }
        }

        Guid? _OutwardFacilityLocationID;
        [DataMember(Name = "OFLOID")]
        public Guid? OutwardFacilityLocationID
        {
            get { return _OutwardFacilityLocationID; }
            set { SetProperty<Guid?>(ref _OutwardFacilityLocationID, value); }
        }

        Int32? _OutwardSplitNo;
        [DataMember(Name = "OSNo")]
        public Int32? OutwardSplitNo
        {
            get { return _OutwardSplitNo; }
            set { SetProperty<Int32?>(ref _OutwardSplitNo, value); }
        }

        Guid? _OutwardPartslistID;
        [DataMember(Name = "OPLID")]
        public Guid? OutwardPartslistID
        {
            get { return _OutwardPartslistID; }
            set { SetProperty<Guid?>(ref _OutwardPartslistID, value); }
        }

        Guid? _OutwardCompanyMaterialID;
        [DataMember(Name = "OCMID")]
        public Guid? OutwardCompanyMaterialID
        {
            get { return _OutwardCompanyMaterialID; }
            set { SetProperty<Guid?>(ref _OutwardCompanyMaterialID, value); }
        }

        Double? _OutwardQuantity;
        [DataMember(Name = "OQ")]
        public Double? OutwardQuantity
        {
            get { return _OutwardQuantity; }
            set { SetProperty<Double?>(ref _OutwardQuantity, value); }
        }

        Double? _OutwardTargetQuantity;
        [DataMember(Name = "OTQ")]
        public Double? OutwardTargetQuantity
        {
            get { return _OutwardTargetQuantity; }
            set { SetProperty<Double?>(ref _OutwardTargetQuantity, value); }
        }
        #endregion

        #region Order-Related

        Guid? _MDUnitID;
        [DataMember(Name = "MDUID")]
        public Guid? MDUnitID
        {
            get { return _MDUnitID; }
            set { SetProperty<Guid?>(ref _MDUnitID, value); }
        }

        Guid? _InOrderPosID;
        [DataMember(Name = "IOPID")]
        public Guid? InOrderPosID
        {
            get { return _InOrderPosID; }
            set { SetProperty<Guid?>(ref _InOrderPosID, value); }
        }

        Guid? _OutOrderPosID;
        [DataMember(Name = "OOPID")]
        public Guid? OutOrderPosID
        {
            get { return _OutOrderPosID; }
            set { SetProperty<Guid?>(ref _OutOrderPosID, value); }
        }

        Guid? _PartslistPosID;
        [DataMember(Name = "PoPPID")]
        public Guid? PartslistPosID
        {
            get { return _PartslistPosID; }
            set { SetProperty<Guid?>(ref _PartslistPosID, value); }
        }

        Guid? _PartslistPosRelationID;
        [DataMember(Name = "PoPRRID")]
        public Guid? PartslistPosRelationID
        {
            get { return _PartslistPosRelationID; }
            set { SetProperty<Guid?>(ref _PartslistPosRelationID, value); }
        }

        Guid? _ProdOrderPartslistPosFacilityLotID;
        [DataMember(Name = "PoPPFLID")]
        public Guid? ProdOrderPartslistPosFacilityLotID
        {
            get { return _ProdOrderPartslistPosFacilityLotID; }
            set { SetProperty<Guid?>(ref _ProdOrderPartslistPosFacilityLotID, value); }
        }

        Guid? _PickingPosID;
        [DataMember(Name = "PiPoID")]
        public Guid? PickingPosID
        {
            get { return _PickingPosID; }
            set { SetProperty<Guid?>(ref _PickingPosID, value); }
        }
        #endregion

        #region Misc-Data

        DateTime? _StorageDate;
        [DataMember(Name = "StoDT")]
        public DateTime? StorageDate
        {
            get { return _StorageDate; }
            set { SetProperty<DateTime?>(ref _StorageDate, value); }
        }

        DateTime? _ProductionDate;
        [DataMember(Name = "ProDT")]
        public DateTime? ProductionDate
        {
            get { return _ProductionDate; }
            set { SetProperty<DateTime?>(ref _ProductionDate, value); }
        }

        DateTime? _ExpirationDate;
        [DataMember(Name = "ExpDT")]
        public DateTime? ExpirationDate
        {
            get { return _ExpirationDate; }
            set { SetProperty<DateTime?>(ref _ExpirationDate, value); }
        }

        int? _MinimumDurability;
        [DataMember(Name = "MinDu")]
        public int? MinimumDurability
        {
            get { return _MinimumDurability; }
            set { SetProperty<int?>(ref _MinimumDurability, value); }
        }

        string _Comment;
        [DataMember(Name = "CMT")]
        public string Comment
        {
            get { return _Comment; }
            set { SetProperty<string>(ref _Comment, value); }
        }

        string _RecipeOrFactoryInfo;
        [DataMember(Name = "RoFI")]
        public string RecipeOrFactoryInfo
        {
            get { return _RecipeOrFactoryInfo; }
            set { SetProperty<string>(ref _RecipeOrFactoryInfo, value); }
        }

        string _PropertyACUrl;
        [DataMember(Name = "PACurl")]
        public string PropertyACUrl
        {
            get { return _PropertyACUrl; }
            set { SetProperty<string>(ref _PropertyACUrl, value); }
        }

        string _ExternLotNo;
        [DataMember(Name = "ExtLNo")]
        public string ExternLotNo
        {
            get { return _ExternLotNo; }
            set { SetProperty<string>(ref _ExternLotNo, value); }
        }
        #endregion

    }
}
