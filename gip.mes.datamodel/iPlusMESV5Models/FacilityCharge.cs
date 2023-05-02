using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class FacilityCharge : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public FacilityCharge()
    {
    }

    private FacilityCharge(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _FacilityChargeID;
    public Guid FacilityChargeID 
    {
        get { return _FacilityChargeID; }
        set { SetProperty<Guid>(ref _FacilityChargeID, value); }
    }

    Guid? _FacilityLotID;
    public Guid? FacilityLotID 
    {
        get { return _FacilityLotID; }
        set { SetProperty<Guid?>(ref _FacilityLotID, value); }
    }

    int _FacilityChargeSortNo;
    public int FacilityChargeSortNo 
    {
        get { return _FacilityChargeSortNo; }
        set { SetProperty<int>(ref _FacilityChargeSortNo, value); }
    }

    int _SplitNo;
    public int SplitNo 
    {
        get { return _SplitNo; }
        set { SetProperty<int>(ref _SplitNo, value); }
    }

    bool _NotAvailable;
    public bool NotAvailable 
    {
        get { return _NotAvailable; }
        set { SetProperty<bool>(ref _NotAvailable, value); }
    }

    Guid _FacilityID;
    public Guid FacilityID 
    {
        get { return _FacilityID; }
        set { SetProperty<Guid>(ref _FacilityID, value); }
    }

    Guid _MaterialID;
    public Guid MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid>(ref _MaterialID, value); }
    }

    Guid? _PartslistID;
    public Guid? PartslistID 
    {
        get { return _PartslistID; }
        set { SetProperty<Guid?>(ref _PartslistID, value); }
    }

    Guid _MDUnitID;
    public Guid MDUnitID 
    {
        get { return _MDUnitID; }
        set { SetProperty<Guid>(ref _MDUnitID, value); }
    }

    Guid? _CompanyMaterialID;
    public Guid? CompanyMaterialID 
    {
        get { return _CompanyMaterialID; }
        set { SetProperty<Guid?>(ref _CompanyMaterialID, value); }
    }

    string _HandlingUnit;
    public string HandlingUnit 
    {
        get { return _HandlingUnit; }
        set { SetProperty<string>(ref _HandlingUnit, value); }
    }

    Guid? _MDReleaseStateID;
    public Guid? MDReleaseStateID 
    {
        get { return _MDReleaseStateID; }
        set { SetProperty<Guid?>(ref _MDReleaseStateID, value); }
    }

    double _StockQuantity;
    public double StockQuantity 
    {
        get { return _StockQuantity; }
        set { SetProperty<double>(ref _StockQuantity, value); }
    }

    double _StockQuantityUOM;
    public double StockQuantityUOM 
    {
        get { return _StockQuantityUOM; }
        set { SetProperty<double>(ref _StockQuantityUOM, value); }
    }

    double _ReservedOutwardQuantity;
    public double ReservedOutwardQuantity 
    {
        get { return _ReservedOutwardQuantity; }
        set { SetProperty<double>(ref _ReservedOutwardQuantity, value); }
    }

    double _ReservedInwardQuantity;
    public double ReservedInwardQuantity 
    {
        get { return _ReservedInwardQuantity; }
        set { SetProperty<double>(ref _ReservedInwardQuantity, value); }
    }

    DateTime? _FillingDate;
    public DateTime? FillingDate 
    {
        get { return _FillingDate; }
        set { SetProperty<DateTime?>(ref _FillingDate, value); }
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

    decimal _CostReQuantity;
    public decimal CostReQuantity 
    {
        get { return _CostReQuantity; }
        set { SetProperty<decimal>(ref _CostReQuantity, value); }
    }

    decimal _CostMat;
    public decimal CostMat 
    {
        get { return _CostMat; }
        set { SetProperty<decimal>(ref _CostMat, value); }
    }

    decimal _CostVar;
    public decimal CostVar 
    {
        get { return _CostVar; }
        set { SetProperty<decimal>(ref _CostVar, value); }
    }

    decimal _CostFix;
    public decimal CostFix 
    {
        get { return _CostFix; }
        set { SetProperty<decimal>(ref _CostFix, value); }
    }

    decimal _CostPack;
    public decimal CostPack 
    {
        get { return _CostPack; }
        set { SetProperty<decimal>(ref _CostPack, value); }
    }

    decimal _CostGeneral;
    public decimal CostGeneral 
    {
        get { return _CostGeneral; }
        set { SetProperty<decimal>(ref _CostGeneral, value); }
    }

    decimal _CostLoss;
    public decimal CostLoss 
    {
        get { return _CostLoss; }
        set { SetProperty<decimal>(ref _CostLoss, value); }
    }

    decimal _CostHandlingVar;
    public decimal CostHandlingVar 
    {
        get { return _CostHandlingVar; }
        set { SetProperty<decimal>(ref _CostHandlingVar, value); }
    }

    decimal _CostHandlingFix;
    public decimal CostHandlingFix 
    {
        get { return _CostHandlingFix; }
        set { SetProperty<decimal>(ref _CostHandlingFix, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
    }

    bool _Lock;
    public bool Lock 
    {
        get { return _Lock; }
        set { SetProperty<bool>(ref _Lock, value); }
    }

    bool _IsEnabled;
    public bool IsEnabled 
    {
        get { return _IsEnabled; }
        set { SetProperty<bool>(ref _IsEnabled, value); }
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

    string _UpdateName;
    public string UpdateName 
    {
        get { return _UpdateName; }
        set { SetProperty<string>(ref _UpdateName, value); }
    }

    DateTime _UpdateDate;
    public DateTime UpdateDate 
    {
        get { return _UpdateDate; }
        set { SetProperty<DateTime>(ref _UpdateDate, value); }
    }

    byte[] _RowVersion;
    public byte[] RowVersion 
    {
        get { return _RowVersion; }
        set { SetProperty<byte[]>(ref _RowVersion, value); }
    }

    Guid? _CPartnerCompanyMaterialID;
    public Guid? CPartnerCompanyMaterialID 
    {
        get { return _CPartnerCompanyMaterialID; }
        set { SetProperty<Guid?>(ref _CPartnerCompanyMaterialID, value); }
    }

    double _StockQuantityUOMAmb;
    public double StockQuantityUOMAmb 
    {
        get { return _StockQuantityUOMAmb; }
        set { SetProperty<double>(ref _StockQuantityUOMAmb, value); }
    }

    private CompanyMaterial _CPartnerCompanyMaterial;
    public virtual CompanyMaterial CPartnerCompanyMaterial
    { 
        get => LazyLoader.Load(this, ref _CPartnerCompanyMaterial);
        set => _CPartnerCompanyMaterial = value;
    }

    public bool CPartnerCompanyMaterial_IsLoaded
    {
        get
        {
            return CPartnerCompanyMaterial != null;
        }
    }

    public virtual ReferenceEntry CPartnerCompanyMaterialReference 
    {
        get { return Context.Entry(this).Reference("CPartnerCompanyMaterial"); }
    }
    
    private CompanyMaterial _CompanyMaterial;
    public virtual CompanyMaterial CompanyMaterial
    { 
        get => LazyLoader.Load(this, ref _CompanyMaterial);
        set => _CompanyMaterial = value;
    }

    public bool CompanyMaterial_IsLoaded
    {
        get
        {
            return CompanyMaterial != null;
        }
    }

    public virtual ReferenceEntry CompanyMaterialReference 
    {
        get { return Context.Entry(this).Reference("CompanyMaterial"); }
    }
    
    private Facility _Facility;
    public virtual Facility Facility
    { 
        get => LazyLoader.Load(this, ref _Facility);
        set => _Facility = value;
    }

    public bool Facility_IsLoaded
    {
        get
        {
            return Facility != null;
        }
    }

    public virtual ReferenceEntry FacilityReference 
    {
        get { return Context.Entry(this).Reference("Facility"); }
    }
    
    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_InwardFacilityCharge;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_InwardFacilityCharge
    {
        get => LazyLoader.Load(this, ref _FacilityBookingCharge_InwardFacilityCharge);
        set => _FacilityBookingCharge_InwardFacilityCharge = value;
    }

    public bool FacilityBookingCharge_InwardFacilityCharge_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_InwardFacilityCharge != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_InwardFacilityChargeReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_InwardFacilityCharge); }
    }

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_OutwardFacilityCharge;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_OutwardFacilityCharge
    {
        get => LazyLoader.Load(this, ref _FacilityBookingCharge_OutwardFacilityCharge);
        set => _FacilityBookingCharge_OutwardFacilityCharge = value;
    }

    public bool FacilityBookingCharge_OutwardFacilityCharge_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_OutwardFacilityCharge != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_OutwardFacilityChargeReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_OutwardFacilityCharge); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_InwardFacilityCharge;
    public virtual ICollection<FacilityBooking> FacilityBooking_InwardFacilityCharge
    {
        get => LazyLoader.Load(this, ref _FacilityBooking_InwardFacilityCharge);
        set => _FacilityBooking_InwardFacilityCharge = value;
    }

    public bool FacilityBooking_InwardFacilityCharge_IsLoaded
    {
        get
        {
            return FacilityBooking_InwardFacilityCharge != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_InwardFacilityChargeReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_InwardFacilityCharge); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_OutwardFacilityCharge;
    public virtual ICollection<FacilityBooking> FacilityBooking_OutwardFacilityCharge
    {
        get => LazyLoader.Load(this, ref _FacilityBooking_OutwardFacilityCharge);
        set => _FacilityBooking_OutwardFacilityCharge = value;
    }

    public bool FacilityBooking_OutwardFacilityCharge_IsLoaded
    {
        get
        {
            return FacilityBooking_OutwardFacilityCharge != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_OutwardFacilityChargeReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_OutwardFacilityCharge); }
    }

    private ICollection<FacilityInventoryPos> _FacilityInventoryPos_FacilityCharge;
    public virtual ICollection<FacilityInventoryPos> FacilityInventoryPos_FacilityCharge
    {
        get => LazyLoader.Load(this, ref _FacilityInventoryPos_FacilityCharge);
        set => _FacilityInventoryPos_FacilityCharge = value;
    }

    public bool FacilityInventoryPos_FacilityCharge_IsLoaded
    {
        get
        {
            return FacilityInventoryPos_FacilityCharge != null;
        }
    }

    public virtual CollectionEntry FacilityInventoryPos_FacilityChargeReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityInventoryPos_FacilityCharge); }
    }

    private FacilityLot _FacilityLot;
    public virtual FacilityLot FacilityLot
    { 
        get => LazyLoader.Load(this, ref _FacilityLot);
        set => _FacilityLot = value;
    }

    public bool FacilityLot_IsLoaded
    {
        get
        {
            return FacilityLot != null;
        }
    }

    public virtual ReferenceEntry FacilityLotReference 
    {
        get { return Context.Entry(this).Reference("FacilityLot"); }
    }
    
    private ICollection<FacilityReservation> _FacilityReservation_FacilityCharge;
    public virtual ICollection<FacilityReservation> FacilityReservation_FacilityCharge
    {
        get => LazyLoader.Load(this, ref _FacilityReservation_FacilityCharge);
        set => _FacilityReservation_FacilityCharge = value;
    }

    public bool FacilityReservation_FacilityCharge_IsLoaded
    {
        get
        {
            return FacilityReservation_FacilityCharge != null;
        }
    }

    public virtual CollectionEntry FacilityReservation_FacilityChargeReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityReservation_FacilityCharge); }
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
    
    private Material _Material;
    public virtual Material Material
    { 
        get => LazyLoader.Load(this, ref _Material);
        set => _Material = value;
    }

    public bool Material_IsLoaded
    {
        get
        {
            return Material != null;
        }
    }

    public virtual ReferenceEntry MaterialReference 
    {
        get { return Context.Entry(this).Reference("Material"); }
    }
    
    private ICollection<OperationLog> _OperationLog_FacilityCharge;
    public virtual ICollection<OperationLog> OperationLog_FacilityCharge
    {
        get => LazyLoader.Load(this, ref _OperationLog_FacilityCharge);
        set => _OperationLog_FacilityCharge = value;
    }

    public bool OperationLog_FacilityCharge_IsLoaded
    {
        get
        {
            return OperationLog_FacilityCharge != null;
        }
    }

    public virtual CollectionEntry OperationLog_FacilityChargeReference
    {
        get { return Context.Entry(this).Collection(c => c.OperationLog_FacilityCharge); }
    }

    private Partslist _Partslist;
    public virtual Partslist Partslist
    { 
        get => LazyLoader.Load(this, ref _Partslist);
        set => _Partslist = value;
    }

    public bool Partslist_IsLoaded
    {
        get
        {
            return Partslist != null;
        }
    }

    public virtual ReferenceEntry PartslistReference 
    {
        get { return Context.Entry(this).Reference("Partslist"); }
    }
    
}
