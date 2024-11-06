using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class FacilityBooking : VBEntityObject
{

    public FacilityBooking()
    {
    }

    private FacilityBooking(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
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

    string _FacilityBookingNo;
    public string FacilityBookingNo 
    {
        get { return _FacilityBookingNo; }
        set { SetProperty<string>(ref _FacilityBookingNo, value); }
    }

    bool? _DontAllowNegativeStock;
    public bool? DontAllowNegativeStock 
    {
        get { return _DontAllowNegativeStock; }
        set { SetProperty<bool?>(ref _DontAllowNegativeStock, value); }
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

    string _InwardHandlingUnit;
    public string InwardHandlingUnit 
    {
        get { return _InwardHandlingUnit; }
        set { SetProperty<string>(ref _InwardHandlingUnit, value); }
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

    string _InwardXMLIdentification;
    public string InwardXMLIdentification 
    {
        get { return _InwardXMLIdentification; }
        set { SetProperty<string>(ref _InwardXMLIdentification, value); }
    }

    double _InwardQuantity;
    public double InwardQuantity 
    {
        get { return _InwardQuantity; }
        set { SetProperty<double>(ref _InwardQuantity, value); }
    }

    double _InwardTargetQuantity;
    public double InwardTargetQuantity 
    {
        get { return _InwardTargetQuantity; }
        set { SetProperty<double>(ref _InwardTargetQuantity, value); }
    }

    Guid? _OutwardMaterialID;
    public Guid? OutwardMaterialID 
    {
        get { return _OutwardMaterialID; }
        set { SetProperty<Guid?>(ref _OutwardMaterialID, value); }
    }

    string _OutwardHandlingUnit;
    public string OutwardHandlingUnit 
    {
        get { return _OutwardHandlingUnit; }
        set { SetProperty<string>(ref _OutwardHandlingUnit, value); }
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

    string _OutwardXMLIdentification;
    public string OutwardXMLIdentification 
    {
        get { return _OutwardXMLIdentification; }
        set { SetProperty<string>(ref _OutwardXMLIdentification, value); }
    }

    double _OutwardQuantity;
    public double OutwardQuantity 
    {
        get { return _OutwardQuantity; }
        set { SetProperty<double>(ref _OutwardQuantity, value); }
    }

    double _OutwardTargetQuantity;
    public double OutwardTargetQuantity 
    {
        get { return _OutwardTargetQuantity; }
        set { SetProperty<double>(ref _OutwardTargetQuantity, value); }
    }

    Guid? _MDUnitID;
    public Guid? MDUnitID 
    {
        get { return _MDUnitID; }
        set { SetProperty<Guid?>(ref _MDUnitID, value); }
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

    Guid? _CPartnerCompanyID;
    public Guid? CPartnerCompanyID 
    {
        get { return _CPartnerCompanyID; }
        set { SetProperty<Guid?>(ref _CPartnerCompanyID, value); }
    }

    Guid? _ProdOrderPartslistPosID;
    public Guid? ProdOrderPartslistPosID 
    {
        get { return _ProdOrderPartslistPosID; }
        set { SetProperty<Guid?>(ref _ProdOrderPartslistPosID, value); }
    }

    string _PropertyACUrl;
    public string PropertyACUrl 
    {
        get { return _PropertyACUrl; }
        set { SetProperty<string>(ref _PropertyACUrl, value); }
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

    Guid? _HistoryID;
    public Guid? HistoryID 
    {
        get { return _HistoryID; }
        set { SetProperty<Guid?>(ref _HistoryID, value); }
    }

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
    }

    string _BookingMessage;
    public string BookingMessage 
    {
        get { return _BookingMessage; }
        set { SetProperty<string>(ref _BookingMessage, value); }
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

    double _InwardQuantityAmb;
    public double InwardQuantityAmb 
    {
        get { return _InwardQuantityAmb; }
        set { SetProperty<double>(ref _InwardQuantityAmb, value); }
    }

    double _InwardTargetQuantityAmb;
    public double InwardTargetQuantityAmb 
    {
        get { return _InwardTargetQuantityAmb; }
        set { SetProperty<double>(ref _InwardTargetQuantityAmb, value); }
    }

    double _OutwardQuantityAmb;
    public double OutwardQuantityAmb 
    {
        get { return _OutwardQuantityAmb; }
        set { SetProperty<double>(ref _OutwardQuantityAmb, value); }
    }

    double _OutwardTargetQuantityAmb;
    public double OutwardTargetQuantityAmb 
    {
        get { return _OutwardTargetQuantityAmb; }
        set { SetProperty<double>(ref _OutwardTargetQuantityAmb, value); }
    }

    Guid? _ProdOrderPartslistPosRelationID;
    public Guid? ProdOrderPartslistPosRelationID 
    {
        get { return _ProdOrderPartslistPosRelationID; }
        set { SetProperty<Guid?>(ref _ProdOrderPartslistPosRelationID, value); }
    }

    short _MaterialProcessStateIndex;
    public short MaterialProcessStateIndex 
    {
        get { return _MaterialProcessStateIndex; }
        set { SetProperty<short>(ref _MaterialProcessStateIndex, value); }
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

    private Company _CPartnerCompany;
    public virtual Company CPartnerCompany
    { 
        get { return LazyLoader.Load(this, ref _CPartnerCompany); } 
        set { SetProperty<Company>(ref _CPartnerCompany, value); }
    }

    public bool CPartnerCompany_IsLoaded
    {
        get
        {
            return CPartnerCompany != null;
        }
    }

    public virtual ReferenceEntry CPartnerCompanyReference 
    {
        get { return Context.Entry(this).Reference("CPartnerCompany"); }
    }
    
    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_FacilityBooking;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_FacilityBooking
    {
        get { return LazyLoader.Load(this, ref _FacilityBookingCharge_FacilityBooking); }
        set { _FacilityBookingCharge_FacilityBooking = value; }
    }

    public bool FacilityBookingCharge_FacilityBooking_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_FacilityBooking != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_FacilityBookingReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_FacilityBooking); }
    }

    private FacilityInventoryPos _FacilityInventoryPos;
    public virtual FacilityInventoryPos FacilityInventoryPos
    { 
        get { return LazyLoader.Load(this, ref _FacilityInventoryPos); } 
        set { SetProperty<FacilityInventoryPos>(ref _FacilityInventoryPos, value); }
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
    
    private History _History;
    public virtual History History
    { 
        get { return LazyLoader.Load(this, ref _History); } 
        set { SetProperty<History>(ref _History, value); }
    }

    public bool History_IsLoaded
    {
        get
        {
            return History != null;
        }
    }

    public virtual ReferenceEntry HistoryReference 
    {
        get { return Context.Entry(this).Reference("History"); }
    }
    
    private InOrderPos _InOrderPos;
    public virtual InOrderPos InOrderPos
    { 
        get { return LazyLoader.Load(this, ref _InOrderPos); } 
        set { SetProperty<InOrderPos>(ref _InOrderPos, value); }
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
    
    private CompanyMaterial _InwardCompanyMaterial;
    public virtual CompanyMaterial InwardCompanyMaterial
    { 
        get { return LazyLoader.Load(this, ref _InwardCompanyMaterial); } 
        set { SetProperty<CompanyMaterial>(ref _InwardCompanyMaterial, value); }
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
        get { return LazyLoader.Load(this, ref _InwardFacility); } 
        set { SetProperty<Facility>(ref _InwardFacility, value); }
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
        get { return LazyLoader.Load(this, ref _InwardFacilityCharge); } 
        set { SetProperty<FacilityCharge>(ref _InwardFacilityCharge, value); }
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
        get { return LazyLoader.Load(this, ref _InwardFacilityLocation); } 
        set { SetProperty<Facility>(ref _InwardFacilityLocation, value); }
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
        get { return LazyLoader.Load(this, ref _InwardFacilityLot); } 
        set { SetProperty<FacilityLot>(ref _InwardFacilityLot, value); }
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
        get { return LazyLoader.Load(this, ref _InwardMaterial); } 
        set { SetProperty<Material>(ref _InwardMaterial, value); }
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
        get { return LazyLoader.Load(this, ref _InwardPartslist); } 
        set { SetProperty<Partslist>(ref _InwardPartslist, value); }
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
        get { return LazyLoader.Load(this, ref _MDBalancingMode); } 
        set { SetProperty<MDBalancingMode>(ref _MDBalancingMode, value); }
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
        get { return LazyLoader.Load(this, ref _MDBookingNotAvailableMode); } 
        set { SetProperty<MDBookingNotAvailableMode>(ref _MDBookingNotAvailableMode, value); }
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
        get { return LazyLoader.Load(this, ref _MDMovementReason); } 
        set { SetProperty<MDMovementReason>(ref _MDMovementReason, value); }
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
        get { return LazyLoader.Load(this, ref _MDReleaseState); } 
        set { SetProperty<MDReleaseState>(ref _MDReleaseState, value); }
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
        get { return LazyLoader.Load(this, ref _MDReservationMode); } 
        set { SetProperty<MDReservationMode>(ref _MDReservationMode, value); }
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
        get { return LazyLoader.Load(this, ref _MDUnit); } 
        set { SetProperty<MDUnit>(ref _MDUnit, value); }
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
        get { return LazyLoader.Load(this, ref _MDZeroStockState); } 
        set { SetProperty<MDZeroStockState>(ref _MDZeroStockState, value); }
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
    
    private ICollection<OrderLog> _OrderLog_FacilityBooking;
    public virtual ICollection<OrderLog> OrderLog_FacilityBooking
    {
        get { return LazyLoader.Load(this, ref _OrderLog_FacilityBooking); }
        set { _OrderLog_FacilityBooking = value; }
    }

    public bool OrderLog_FacilityBooking_IsLoaded
    {
        get
        {
            return OrderLog_FacilityBooking != null;
        }
    }

    public virtual CollectionEntry OrderLog_FacilityBookingReference
    {
        get { return Context.Entry(this).Collection(c => c.OrderLog_FacilityBooking); }
    }

    private OutOrderPos _OutOrderPos;
    public virtual OutOrderPos OutOrderPos
    { 
        get { return LazyLoader.Load(this, ref _OutOrderPos); } 
        set { SetProperty<OutOrderPos>(ref _OutOrderPos, value); }
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
    
    private CompanyMaterial _OutwardCompanyMaterial;
    public virtual CompanyMaterial OutwardCompanyMaterial
    { 
        get { return LazyLoader.Load(this, ref _OutwardCompanyMaterial); } 
        set { SetProperty<CompanyMaterial>(ref _OutwardCompanyMaterial, value); }
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
        get { return LazyLoader.Load(this, ref _OutwardFacility); } 
        set { SetProperty<Facility>(ref _OutwardFacility, value); }
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
        get { return LazyLoader.Load(this, ref _OutwardFacilityCharge); } 
        set { SetProperty<FacilityCharge>(ref _OutwardFacilityCharge, value); }
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
        get { return LazyLoader.Load(this, ref _OutwardFacilityLocation); } 
        set { SetProperty<Facility>(ref _OutwardFacilityLocation, value); }
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
        get { return LazyLoader.Load(this, ref _OutwardFacilityLot); } 
        set { SetProperty<FacilityLot>(ref _OutwardFacilityLot, value); }
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
        get { return LazyLoader.Load(this, ref _OutwardMaterial); } 
        set { SetProperty<Material>(ref _OutwardMaterial, value); }
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
        get { return LazyLoader.Load(this, ref _OutwardPartslist); } 
        set { SetProperty<Partslist>(ref _OutwardPartslist, value); }
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
        get { return LazyLoader.Load(this, ref _PickingPos); } 
        set { SetProperty<PickingPos>(ref _PickingPos, value); }
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
        get { return LazyLoader.Load(this, ref _ProdOrderPartslistPos); } 
        set { SetProperty<ProdOrderPartslistPos>(ref _ProdOrderPartslistPos, value); }
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
        get { return LazyLoader.Load(this, ref _ProdOrderPartslistPosFacilityLot); } 
        set { SetProperty<ProdOrderPartslistPosFacilityLot>(ref _ProdOrderPartslistPosFacilityLot, value); }
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
        get { return LazyLoader.Load(this, ref _ProdOrderPartslistPosRelation); } 
        set { SetProperty<ProdOrderPartslistPosRelation>(ref _ProdOrderPartslistPosRelation, value); }
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
    
    private ACClass _VBiStackCalculatorACClass;
    public virtual ACClass VBiStackCalculatorACClass
    { 
        get { return LazyLoader.Load(this, ref _VBiStackCalculatorACClass); } 
        set { SetProperty<ACClass>(ref _VBiStackCalculatorACClass, value); }
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
