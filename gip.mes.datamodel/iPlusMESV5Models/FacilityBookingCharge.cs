using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class FacilityBookingCharge : VBEntityObject 
{

    public FacilityBookingCharge()
    {
    }

    private FacilityBookingCharge(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _FacilityBookingChargeID;
    public Guid FacilityBookingChargeID 
    {
        get { return _FacilityBookingChargeID; }
        set { SetProperty<Guid>(ref _FacilityBookingChargeID, value); }
    }

    Guid _FacilityBookingID;
    public Guid FacilityBookingID 
    {
        get { return _FacilityBookingID; }
        set { SetProperty<Guid>(ref _FacilityBookingID, value); }
    }

    short _FacilityBookingTypeIndex;
    public short FacilityBookingTypeIndex 
    {
        get { return _FacilityBookingTypeIndex; }
        set { SetProperty<short>(ref _FacilityBookingTypeIndex, value); }
    }

    bool _BookingSucceeded;
    public bool BookingSucceeded 
    {
        get { return _BookingSucceeded; }
        set { SetProperty<bool>(ref _BookingSucceeded, value); }
    }

    string _FacilityBookingChargeNo;
    public string FacilityBookingChargeNo 
    {
        get { return _FacilityBookingChargeNo; }
        set { SetProperty<string>(ref _FacilityBookingChargeNo, value); }
    }

    bool? _DontAllowNegativeStock;
    public bool? DontAllowNegativeStock 
    {
        get { return _DontAllowNegativeStock; }
        set { SetProperty<bool?>(ref _DontAllowNegativeStock, value); }
    }

    Guid? _MDZeroStockStateID;
    public Guid? MDZeroStockStateID 
    {
        get { return _MDZeroStockStateID; }
        set { SetProperty<Guid?>(ref _MDZeroStockStateID, value); }
    }

    Guid? _MDBookingNotAvailableModeID;
    public Guid? MDBookingNotAvailableModeID 
    {
        get { return _MDBookingNotAvailableModeID; }
        set { SetProperty<Guid?>(ref _MDBookingNotAvailableModeID, value); }
    }

    Guid? _MDBalancingModeID;
    public Guid? MDBalancingModeID 
    {
        get { return _MDBalancingModeID; }
        set { SetProperty<Guid?>(ref _MDBalancingModeID, value); }
    }

    Guid? _MDReservationModeID;
    public Guid? MDReservationModeID 
    {
        get { return _MDReservationModeID; }
        set { SetProperty<Guid?>(ref _MDReservationModeID, value); }
    }

    Guid? _VBiStackCalculatorACClassID;
    public Guid? VBiStackCalculatorACClassID 
    {
        get { return _VBiStackCalculatorACClassID; }
        set { SetProperty<Guid?>(ref _VBiStackCalculatorACClassID, value); }
    }

    bool? _IgnoreManagement;
    public bool? IgnoreManagement 
    {
        get { return _IgnoreManagement; }
        set { SetProperty<bool?>(ref _IgnoreManagement, value); }
    }

    bool? _QuantityIsAbsolute;
    public bool? QuantityIsAbsolute 
    {
        get { return _QuantityIsAbsolute; }
        set { SetProperty<bool?>(ref _QuantityIsAbsolute, value); }
    }

    bool _ShiftBookingReverse;
    public bool ShiftBookingReverse 
    {
        get { return _ShiftBookingReverse; }
        set { SetProperty<bool>(ref _ShiftBookingReverse, value); }
    }

    Guid? _MDReleaseStateID;
    public Guid? MDReleaseStateID 
    {
        get { return _MDReleaseStateID; }
        set { SetProperty<Guid?>(ref _MDReleaseStateID, value); }
    }

    bool _SetCompleted;
    public bool SetCompleted 
    {
        get { return _SetCompleted; }
        set { SetProperty<bool>(ref _SetCompleted, value); }
    }

    bool _NoInwardOutwardBalancing;
    public bool NoInwardOutwardBalancing 
    {
        get { return _NoInwardOutwardBalancing; }
        set { SetProperty<bool>(ref _NoInwardOutwardBalancing, value); }
    }

    Guid? _MDMovementReasonID;
    public Guid? MDMovementReasonID 
    {
        get { return _MDMovementReasonID; }
        set { SetProperty<Guid?>(ref _MDMovementReasonID, value); }
    }

    Guid? _InwardMaterialID;
    public Guid? InwardMaterialID 
    {
        get { return _InwardMaterialID; }
        set { SetProperty<Guid?>(ref _InwardMaterialID, value); }
    }

    Guid? _InwardFacilityID;
    public Guid? InwardFacilityID 
    {
        get { return _InwardFacilityID; }
        set { SetProperty<Guid?>(ref _InwardFacilityID, value); }
    }

    Guid? _InwardFacilityLotID;
    public Guid? InwardFacilityLotID 
    {
        get { return _InwardFacilityLotID; }
        set { SetProperty<Guid?>(ref _InwardFacilityLotID, value); }
    }

    Guid? _InwardFacilityChargeID;
    public Guid? InwardFacilityChargeID 
    {
        get { return _InwardFacilityChargeID; }
        set { SetProperty<Guid?>(ref _InwardFacilityChargeID, value); }
    }

    Guid? _InwardFacilityLocationID;
    public Guid? InwardFacilityLocationID 
    {
        get { return _InwardFacilityLocationID; }
        set { SetProperty<Guid?>(ref _InwardFacilityLocationID, value); }
    }

    Guid? _InwardPartslistID;
    public Guid? InwardPartslistID 
    {
        get { return _InwardPartslistID; }
        set { SetProperty<Guid?>(ref _InwardPartslistID, value); }
    }

    Guid? _InwardCompanyMaterialID;
    public Guid? InwardCompanyMaterialID 
    {
        get { return _InwardCompanyMaterialID; }
        set { SetProperty<Guid?>(ref _InwardCompanyMaterialID, value); }
    }

    Guid? _InwardCPartnerCompMatID;
    public Guid? InwardCPartnerCompMatID 
    {
        get { return _InwardCPartnerCompMatID; }
        set { SetProperty<Guid?>(ref _InwardCPartnerCompMatID, value); }
    }

    double _InwardQuantity;
    public double InwardQuantity 
    {
        get { return _InwardQuantity; }
        set { SetProperty<double>(ref _InwardQuantity, value); }
    }

    double _InwardQuantityUOM;
    public double InwardQuantityUOM 
    {
        get { return _InwardQuantityUOM; }
        set { SetProperty<double>(ref _InwardQuantityUOM, value); }
    }

    double _InwardTargetQuantity;
    public double InwardTargetQuantity 
    {
        get { return _InwardTargetQuantity; }
        set { SetProperty<double>(ref _InwardTargetQuantity, value); }
    }

    double _InwardTargetQuantityUOM;
    public double InwardTargetQuantityUOM 
    {
        get { return _InwardTargetQuantityUOM; }
        set { SetProperty<double>(ref _InwardTargetQuantityUOM, value); }
    }

    Guid? _OutwardMaterialID;
    public Guid? OutwardMaterialID 
    {
        get { return _OutwardMaterialID; }
        set { SetProperty<Guid?>(ref _OutwardMaterialID, value); }
    }

    Guid? _OutwardFacilityID;
    public Guid? OutwardFacilityID 
    {
        get { return _OutwardFacilityID; }
        set { SetProperty<Guid?>(ref _OutwardFacilityID, value); }
    }

    Guid? _OutwardFacilityLotID;
    public Guid? OutwardFacilityLotID 
    {
        get { return _OutwardFacilityLotID; }
        set { SetProperty<Guid?>(ref _OutwardFacilityLotID, value); }
    }

    Guid? _OutwardFacilityChargeID;
    public Guid? OutwardFacilityChargeID 
    {
        get { return _OutwardFacilityChargeID; }
        set { SetProperty<Guid?>(ref _OutwardFacilityChargeID, value); }
    }

    Guid? _OutwardFacilityLocationID;
    public Guid? OutwardFacilityLocationID 
    {
        get { return _OutwardFacilityLocationID; }
        set { SetProperty<Guid?>(ref _OutwardFacilityLocationID, value); }
    }

    Guid? _OutwardPartslistID;
    public Guid? OutwardPartslistID 
    {
        get { return _OutwardPartslistID; }
        set { SetProperty<Guid?>(ref _OutwardPartslistID, value); }
    }

    Guid? _OutwardCompanyMaterialID;
    public Guid? OutwardCompanyMaterialID 
    {
        get { return _OutwardCompanyMaterialID; }
        set { SetProperty<Guid?>(ref _OutwardCompanyMaterialID, value); }
    }

    Guid? _OutwardCPartnerCompMatID;
    public Guid? OutwardCPartnerCompMatID 
    {
        get { return _OutwardCPartnerCompMatID; }
        set { SetProperty<Guid?>(ref _OutwardCPartnerCompMatID, value); }
    }

    double _OutwardQuantity;
    public double OutwardQuantity 
    {
        get { return _OutwardQuantity; }
        set { SetProperty<double>(ref _OutwardQuantity, value); }
    }

    double _OutwardQuantityUOM;
    public double OutwardQuantityUOM 
    {
        get { return _OutwardQuantityUOM; }
        set { SetProperty<double>(ref _OutwardQuantityUOM, value); }
    }

    double _OutwardTargetQuantity;
    public double OutwardTargetQuantity 
    {
        get { return _OutwardTargetQuantity; }
        set { SetProperty<double>(ref _OutwardTargetQuantity, value); }
    }

    double _OutwardTargetQuantityUOM;
    public double OutwardTargetQuantityUOM 
    {
        get { return _OutwardTargetQuantityUOM; }
        set { SetProperty<double>(ref _OutwardTargetQuantityUOM, value); }
    }

    Guid _MDUnitID;
    public Guid MDUnitID 
    {
        get { return _MDUnitID; }
        set { SetProperty<Guid>(ref _MDUnitID, value); }
    }

    Guid? _InOrderPosID;
    public Guid? InOrderPosID 
    {
        get { return _InOrderPosID; }
        set { SetProperty<Guid?>(ref _InOrderPosID, value); }
    }

    Guid? _OutOrderPosID;
    public Guid? OutOrderPosID 
    {
        get { return _OutOrderPosID; }
        set { SetProperty<Guid?>(ref _OutOrderPosID, value); }
    }

    Guid? _ProdOrderPartslistPosID;
    public Guid? ProdOrderPartslistPosID 
    {
        get { return _ProdOrderPartslistPosID; }
        set { SetProperty<Guid?>(ref _ProdOrderPartslistPosID, value); }
    }

    DateTime? _StorageDate;
    public DateTime? StorageDate 
    {
        get { return _StorageDate; }
        set { SetProperty<DateTime?>(ref _StorageDate, value); }
    }

    short _StorageLife;
    public short StorageLife 
    {
        get { return _StorageLife; }
        set { SetProperty<short>(ref _StorageLife, value); }
    }

    DateTime? _ProductionDate;
    public DateTime? ProductionDate 
    {
        get { return _ProductionDate; }
        set { SetProperty<DateTime?>(ref _ProductionDate, value); }
    }

    DateTime? _ExpirationDate;
    public DateTime? ExpirationDate 
    {
        get { return _ExpirationDate; }
        set { SetProperty<DateTime?>(ref _ExpirationDate, value); }
    }

    int? _MinimumDurability;
    public int? MinimumDurability 
    {
        get { return _MinimumDurability; }
        set { SetProperty<int?>(ref _MinimumDurability, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
    }

    string _RecipeOrFactoryInfo;
    public string RecipeOrFactoryInfo 
    {
        get { return _RecipeOrFactoryInfo; }
        set { SetProperty<string>(ref _RecipeOrFactoryInfo, value); }
    }

    string _BookingMessage;
    public string BookingMessage 
    {
        get { return _BookingMessage; }
        set { SetProperty<string>(ref _BookingMessage, value); }
    }

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
    }

    string _InsertName;
    public string InsertName 
    {
        get { return _InsertName; }
        set { SetProperty<string>(ref _InsertName, value); }
    }

    DateTime _InsertDate;
    public DateTime InsertDate 
    {
        get { return _InsertDate; }
        set { SetProperty<DateTime>(ref _InsertDate, value); }
    }

    double _InwardQuantityUOMAmb;
    public double InwardQuantityUOMAmb 
    {
        get { return _InwardQuantityUOMAmb; }
        set { SetProperty<double>(ref _InwardQuantityUOMAmb, value); }
    }

    double _InwardTargetQuantityUOMAmb;
    public double InwardTargetQuantityUOMAmb 
    {
        get { return _InwardTargetQuantityUOMAmb; }
        set { SetProperty<double>(ref _InwardTargetQuantityUOMAmb, value); }
    }

    double _OutwardQuantityUOMAmb;
    public double OutwardQuantityUOMAmb 
    {
        get { return _OutwardQuantityUOMAmb; }
        set { SetProperty<double>(ref _OutwardQuantityUOMAmb, value); }
    }

    double _OutwardTargetQuantityUOMAmb;
    public double OutwardTargetQuantityUOMAmb 
    {
        get { return _OutwardTargetQuantityUOMAmb; }
        set { SetProperty<double>(ref _OutwardTargetQuantityUOMAmb, value); }
    }

    Guid? _ProdOrderPartslistPosRelationID;
    public Guid? ProdOrderPartslistPosRelationID 
    {
        get { return _ProdOrderPartslistPosRelationID; }
        set { SetProperty<Guid?>(ref _ProdOrderPartslistPosRelationID, value); }
    }

    Guid? _PickingPosID;
    public Guid? PickingPosID 
    {
        get { return _PickingPosID; }
        set { SetProperty<Guid?>(ref _PickingPosID, value); }
    }

    Guid? _ProdOrderPartslistPosFacilityLotID;
    public Guid? ProdOrderPartslistPosFacilityLotID 
    {
        get { return _ProdOrderPartslistPosFacilityLotID; }
        set { SetProperty<Guid?>(ref _ProdOrderPartslistPosFacilityLotID, value); }
    }

    Guid? _FacilityInventoryPosID;
    public Guid? FacilityInventoryPosID 
    {
        get { return _FacilityInventoryPosID; }
        set { SetProperty<Guid?>(ref _FacilityInventoryPosID, value); }
    }

    private FacilityBooking _FacilityBooking;
    public virtual FacilityBooking FacilityBooking
    { 
        get => LazyLoader.Load(this, ref _FacilityBooking);
        set => _FacilityBooking = value;
    }

    public bool FacilityBooking_IsLoaded
    {
        get
        {
            return FacilityBooking != null;
        }
    }

    public virtual ReferenceEntry FacilityBookingReference 
    {
        get { return Context.Entry(this).Reference("FacilityBooking"); }
    }
    
    private FacilityInventoryPos _FacilityInventoryPos;
    public virtual FacilityInventoryPos FacilityInventoryPos
    { 
        get => LazyLoader.Load(this, ref _FacilityInventoryPos);
        set => _FacilityInventoryPos = value;
    }

    public bool FacilityInventoryPos_IsLoaded
    {
        get
        {
            return FacilityInventoryPos != null;
        }
    }

    public virtual ReferenceEntry FacilityInventoryPosReference 
    {
        get { return Context.Entry(this).Reference("FacilityInventoryPos"); }
    }
    
    private InOrderPos _InOrderPos;
    public virtual InOrderPos InOrderPos
    { 
        get => LazyLoader.Load(this, ref _InOrderPos);
        set => _InOrderPos = value;
    }

    public bool InOrderPos_IsLoaded
    {
        get
        {
            return InOrderPos != null;
        }
    }

    public virtual ReferenceEntry InOrderPosReference 
    {
        get { return Context.Entry(this).Reference("InOrderPos"); }
    }
    
    private CompanyMaterial _InwardCPartnerCompMat;
    public virtual CompanyMaterial InwardCPartnerCompMat
    { 
        get => LazyLoader.Load(this, ref _InwardCPartnerCompMat);
        set => _InwardCPartnerCompMat = value;
    }

    public bool InwardCPartnerCompMat_IsLoaded
    {
        get
        {
            return InwardCPartnerCompMat != null;
        }
    }

    public virtual ReferenceEntry InwardCPartnerCompMatReference 
    {
        get { return Context.Entry(this).Reference("InwardCPartnerCompMat"); }
    }
    
    private CompanyMaterial _InwardCompanyMaterial;
    public virtual CompanyMaterial InwardCompanyMaterial
    { 
        get => LazyLoader.Load(this, ref _InwardCompanyMaterial);
        set => _InwardCompanyMaterial = value;
    }

    public bool InwardCompanyMaterial_IsLoaded
    {
        get
        {
            return InwardCompanyMaterial != null;
        }
    }

    public virtual ReferenceEntry InwardCompanyMaterialReference 
    {
        get { return Context.Entry(this).Reference("InwardCompanyMaterial"); }
    }
    
    private Facility _InwardFacility;
    public virtual Facility InwardFacility
    { 
        get => LazyLoader.Load(this, ref _InwardFacility);
        set => _InwardFacility = value;
    }

    public bool InwardFacility_IsLoaded
    {
        get
        {
            return InwardFacility != null;
        }
    }

    public virtual ReferenceEntry InwardFacilityReference 
    {
        get { return Context.Entry(this).Reference("InwardFacility"); }
    }
    
    private FacilityCharge _InwardFacilityCharge;
    public virtual FacilityCharge InwardFacilityCharge
    { 
        get => LazyLoader.Load(this, ref _InwardFacilityCharge);
        set => _InwardFacilityCharge = value;
    }

    public bool InwardFacilityCharge_IsLoaded
    {
        get
        {
            return InwardFacilityCharge != null;
        }
    }

    public virtual ReferenceEntry InwardFacilityChargeReference 
    {
        get { return Context.Entry(this).Reference("InwardFacilityCharge"); }
    }
    
    private Facility _InwardFacilityLocation;
    public virtual Facility InwardFacilityLocation
    { 
        get => LazyLoader.Load(this, ref _InwardFacilityLocation);
        set => _InwardFacilityLocation = value;
    }

    public bool InwardFacilityLocation_IsLoaded
    {
        get
        {
            return InwardFacilityLocation != null;
        }
    }

    public virtual ReferenceEntry InwardFacilityLocationReference 
    {
        get { return Context.Entry(this).Reference("InwardFacilityLocation"); }
    }
    
    private FacilityLot _InwardFacilityLot;
    public virtual FacilityLot InwardFacilityLot
    { 
        get => LazyLoader.Load(this, ref _InwardFacilityLot);
        set => _InwardFacilityLot = value;
    }

    public bool InwardFacilityLot_IsLoaded
    {
        get
        {
            return InwardFacilityLot != null;
        }
    }

    public virtual ReferenceEntry InwardFacilityLotReference 
    {
        get { return Context.Entry(this).Reference("InwardFacilityLot"); }
    }
    
    private Material _InwardMaterial;
    public virtual Material InwardMaterial
    { 
        get => LazyLoader.Load(this, ref _InwardMaterial);
        set => _InwardMaterial = value;
    }

    public bool InwardMaterial_IsLoaded
    {
        get
        {
            return InwardMaterial != null;
        }
    }

    public virtual ReferenceEntry InwardMaterialReference 
    {
        get { return Context.Entry(this).Reference("InwardMaterial"); }
    }
    
    private Partslist _InwardPartslist;
    public virtual Partslist InwardPartslist
    { 
        get => LazyLoader.Load(this, ref _InwardPartslist);
        set => _InwardPartslist = value;
    }

    public bool InwardPartslist_IsLoaded
    {
        get
        {
            return InwardPartslist != null;
        }
    }

    public virtual ReferenceEntry InwardPartslistReference 
    {
        get { return Context.Entry(this).Reference("InwardPartslist"); }
    }
    
    private MDBalancingMode _MDBalancingMode;
    public virtual MDBalancingMode MDBalancingMode
    { 
        get => LazyLoader.Load(this, ref _MDBalancingMode);
        set => _MDBalancingMode = value;
    }

    public bool MDBalancingMode_IsLoaded
    {
        get
        {
            return MDBalancingMode != null;
        }
    }

    public virtual ReferenceEntry MDBalancingModeReference 
    {
        get { return Context.Entry(this).Reference("MDBalancingMode"); }
    }
    
    private MDBookingNotAvailableMode _MDBookingNotAvailableMode;
    public virtual MDBookingNotAvailableMode MDBookingNotAvailableMode
    { 
        get => LazyLoader.Load(this, ref _MDBookingNotAvailableMode);
        set => _MDBookingNotAvailableMode = value;
    }

    public bool MDBookingNotAvailableMode_IsLoaded
    {
        get
        {
            return MDBookingNotAvailableMode != null;
        }
    }

    public virtual ReferenceEntry MDBookingNotAvailableModeReference 
    {
        get { return Context.Entry(this).Reference("MDBookingNotAvailableMode"); }
    }
    
    private MDMovementReason _MDMovementReason;
    public virtual MDMovementReason MDMovementReason
    { 
        get => LazyLoader.Load(this, ref _MDMovementReason);
        set => _MDMovementReason = value;
    }

    public bool MDMovementReason_IsLoaded
    {
        get
        {
            return MDMovementReason != null;
        }
    }

    public virtual ReferenceEntry MDMovementReasonReference 
    {
        get { return Context.Entry(this).Reference("MDMovementReason"); }
    }
    
    private MDReleaseState _MDReleaseState;
    public virtual MDReleaseState MDReleaseState
    { 
        get => LazyLoader.Load(this, ref _MDReleaseState);
        set => _MDReleaseState = value;
    }

    public bool MDReleaseState_IsLoaded
    {
        get
        {
            return MDReleaseState != null;
        }
    }

    public virtual ReferenceEntry MDReleaseStateReference 
    {
        get { return Context.Entry(this).Reference("MDReleaseState"); }
    }
    
    private MDReservationMode _MDReservationMode;
    public virtual MDReservationMode MDReservationMode
    { 
        get => LazyLoader.Load(this, ref _MDReservationMode);
        set => _MDReservationMode = value;
    }

    public bool MDReservationMode_IsLoaded
    {
        get
        {
            return MDReservationMode != null;
        }
    }

    public virtual ReferenceEntry MDReservationModeReference 
    {
        get { return Context.Entry(this).Reference("MDReservationMode"); }
    }
    
    private MDUnit _MDUnit;
    public virtual MDUnit MDUnit
    { 
        get => LazyLoader.Load(this, ref _MDUnit);
        set => _MDUnit = value;
    }

    public bool MDUnit_IsLoaded
    {
        get
        {
            return MDUnit != null;
        }
    }

    public virtual ReferenceEntry MDUnitReference 
    {
        get { return Context.Entry(this).Reference("MDUnit"); }
    }
    
    private MDZeroStockState _MDZeroStockState;
    public virtual MDZeroStockState MDZeroStockState
    { 
        get => LazyLoader.Load(this, ref _MDZeroStockState);
        set => _MDZeroStockState = value;
    }

    public bool MDZeroStockState_IsLoaded
    {
        get
        {
            return MDZeroStockState != null;
        }
    }

    public virtual ReferenceEntry MDZeroStockStateReference 
    {
        get { return Context.Entry(this).Reference("MDZeroStockState"); }
    }
    
    private OutOrderPos _OutOrderPos;
    public virtual OutOrderPos OutOrderPos
    { 
        get => LazyLoader.Load(this, ref _OutOrderPos);
        set => _OutOrderPos = value;
    }

    public bool OutOrderPos_IsLoaded
    {
        get
        {
            return OutOrderPos != null;
        }
    }

    public virtual ReferenceEntry OutOrderPosReference 
    {
        get { return Context.Entry(this).Reference("OutOrderPos"); }
    }
    
    private CompanyMaterial _OutwardCPartnerCompMat;
    public virtual CompanyMaterial OutwardCPartnerCompMat
    { 
        get => LazyLoader.Load(this, ref _OutwardCPartnerCompMat);
        set => _OutwardCPartnerCompMat = value;
    }

    public bool OutwardCPartnerCompMat_IsLoaded
    {
        get
        {
            return OutwardCPartnerCompMat != null;
        }
    }

    public virtual ReferenceEntry OutwardCPartnerCompMatReference 
    {
        get { return Context.Entry(this).Reference("OutwardCPartnerCompMat"); }
    }
    
    private CompanyMaterial _OutwardCompanyMaterial;
    public virtual CompanyMaterial OutwardCompanyMaterial
    { 
        get => LazyLoader.Load(this, ref _OutwardCompanyMaterial);
        set => _OutwardCompanyMaterial = value;
    }

    public bool OutwardCompanyMaterial_IsLoaded
    {
        get
        {
            return OutwardCompanyMaterial != null;
        }
    }

    public virtual ReferenceEntry OutwardCompanyMaterialReference 
    {
        get { return Context.Entry(this).Reference("OutwardCompanyMaterial"); }
    }
    
    private Facility _OutwardFacility;
    public virtual Facility OutwardFacility
    { 
        get => LazyLoader.Load(this, ref _OutwardFacility);
        set => _OutwardFacility = value;
    }

    public bool OutwardFacility_IsLoaded
    {
        get
        {
            return OutwardFacility != null;
        }
    }

    public virtual ReferenceEntry OutwardFacilityReference 
    {
        get { return Context.Entry(this).Reference("OutwardFacility"); }
    }
    
    private FacilityCharge _OutwardFacilityCharge;
    public virtual FacilityCharge OutwardFacilityCharge
    { 
        get => LazyLoader.Load(this, ref _OutwardFacilityCharge);
        set => _OutwardFacilityCharge = value;
    }

    public bool OutwardFacilityCharge_IsLoaded
    {
        get
        {
            return OutwardFacilityCharge != null;
        }
    }

    public virtual ReferenceEntry OutwardFacilityChargeReference 
    {
        get { return Context.Entry(this).Reference("OutwardFacilityCharge"); }
    }
    
    private Facility _OutwardFacilityLocation;
    public virtual Facility OutwardFacilityLocation
    { 
        get => LazyLoader.Load(this, ref _OutwardFacilityLocation);
        set => _OutwardFacilityLocation = value;
    }

    public bool OutwardFacilityLocation_IsLoaded
    {
        get
        {
            return OutwardFacilityLocation != null;
        }
    }

    public virtual ReferenceEntry OutwardFacilityLocationReference 
    {
        get { return Context.Entry(this).Reference("OutwardFacilityLocation"); }
    }
    
    private FacilityLot _OutwardFacilityLot;
    public virtual FacilityLot OutwardFacilityLot
    { 
        get => LazyLoader.Load(this, ref _OutwardFacilityLot);
        set => _OutwardFacilityLot = value;
    }

    public bool OutwardFacilityLot_IsLoaded
    {
        get
        {
            return OutwardFacilityLot != null;
        }
    }

    public virtual ReferenceEntry OutwardFacilityLotReference 
    {
        get { return Context.Entry(this).Reference("OutwardFacilityLot"); }
    }
    
    private Material _OutwardMaterial;
    public virtual Material OutwardMaterial
    { 
        get => LazyLoader.Load(this, ref _OutwardMaterial);
        set => _OutwardMaterial = value;
    }

    public bool OutwardMaterial_IsLoaded
    {
        get
        {
            return OutwardMaterial != null;
        }
    }

    public virtual ReferenceEntry OutwardMaterialReference 
    {
        get { return Context.Entry(this).Reference("OutwardMaterial"); }
    }
    
    private Partslist _OutwardPartslist;
    public virtual Partslist OutwardPartslist
    { 
        get => LazyLoader.Load(this, ref _OutwardPartslist);
        set => _OutwardPartslist = value;
    }

    public bool OutwardPartslist_IsLoaded
    {
        get
        {
            return OutwardPartslist != null;
        }
    }

    public virtual ReferenceEntry OutwardPartslistReference 
    {
        get { return Context.Entry(this).Reference("OutwardPartslist"); }
    }
    
    private PickingPos _PickingPos;
    public virtual PickingPos PickingPos
    { 
        get => LazyLoader.Load(this, ref _PickingPos);
        set => _PickingPos = value;
    }

    public bool PickingPos_IsLoaded
    {
        get
        {
            return PickingPos != null;
        }
    }

    public virtual ReferenceEntry PickingPosReference 
    {
        get { return Context.Entry(this).Reference("PickingPos"); }
    }
    
    private ProdOrderPartslistPos _ProdOrderPartslistPos;
    public virtual ProdOrderPartslistPos ProdOrderPartslistPos
    { 
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPos);
        set => _ProdOrderPartslistPos = value;
    }

    public bool ProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPos != null;
        }
    }

    public virtual ReferenceEntry ProdOrderPartslistPosReference 
    {
        get { return Context.Entry(this).Reference("ProdOrderPartslistPos"); }
    }
    
    private ProdOrderPartslistPosFacilityLot _ProdOrderPartslistPosFacilityLot;
    public virtual ProdOrderPartslistPosFacilityLot ProdOrderPartslistPosFacilityLot
    { 
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPosFacilityLot);
        set => _ProdOrderPartslistPosFacilityLot = value;
    }

    public bool ProdOrderPartslistPosFacilityLot_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPosFacilityLot != null;
        }
    }

    public virtual ReferenceEntry ProdOrderPartslistPosFacilityLotReference 
    {
        get { return Context.Entry(this).Reference("ProdOrderPartslistPosFacilityLot"); }
    }
    
    private ProdOrderPartslistPosRelation _ProdOrderPartslistPosRelation;
    public virtual ProdOrderPartslistPosRelation ProdOrderPartslistPosRelation
    { 
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPosRelation);
        set => _ProdOrderPartslistPosRelation = value;
    }

    public bool ProdOrderPartslistPosRelation_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPosRelation != null;
        }
    }

    public virtual ReferenceEntry ProdOrderPartslistPosRelationReference 
    {
        get { return Context.Entry(this).Reference("ProdOrderPartslistPosRelation"); }
    }
    
    private ICollection<TandTv2StepItem> _TandTv2StepItem_FacilityBookingCharge;
    public virtual ICollection<TandTv2StepItem> TandTv2StepItem_FacilityBookingCharge
    {
        get => LazyLoader.Load(this, ref _TandTv2StepItem_FacilityBookingCharge);
        set => _TandTv2StepItem_FacilityBookingCharge = value;
    }

    public bool TandTv2StepItem_FacilityBookingCharge_IsLoaded
    {
        get
        {
            return TandTv2StepItem_FacilityBookingCharge != null;
        }
    }

    public virtual CollectionEntry TandTv2StepItem_FacilityBookingChargeReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv2StepItem_FacilityBookingCharge); }
    }

    private ICollection<TandTv3MixPointFacilityBookingCharge> _TandTv3MixPointFacilityBookingCharge_FacilityBookingCharge;
    public virtual ICollection<TandTv3MixPointFacilityBookingCharge> TandTv3MixPointFacilityBookingCharge_FacilityBookingCharge
    {
        get => LazyLoader.Load(this, ref _TandTv3MixPointFacilityBookingCharge_FacilityBookingCharge);
        set => _TandTv3MixPointFacilityBookingCharge_FacilityBookingCharge = value;
    }

    public bool TandTv3MixPointFacilityBookingCharge_FacilityBookingCharge_IsLoaded
    {
        get
        {
            return TandTv3MixPointFacilityBookingCharge_FacilityBookingCharge != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointFacilityBookingCharge_FacilityBookingChargeReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointFacilityBookingCharge_FacilityBookingCharge); }
    }

    private ACClass _VBiStackCalculatorACClass;
    public virtual ACClass VBiStackCalculatorACClass
    { 
        get => LazyLoader.Load(this, ref _VBiStackCalculatorACClass);
        set => _VBiStackCalculatorACClass = value;
    }

    public bool VBiStackCalculatorACClass_IsLoaded
    {
        get
        {
            return VBiStackCalculatorACClass != null;
        }
    }

    public virtual ReferenceEntry VBiStackCalculatorACClassReference 
    {
        get { return Context.Entry(this).Reference("VBiStackCalculatorACClass"); }
    }
    }
