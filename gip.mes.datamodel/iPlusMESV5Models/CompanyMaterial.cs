using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class CompanyMaterial : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public CompanyMaterial()
    {
    }

    private CompanyMaterial(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _CompanyMaterialID;
    public Guid CompanyMaterialID 
    {
        get { return _CompanyMaterialID; }
        set { SetProperty<Guid>(ref _CompanyMaterialID, value); }
    }

    Guid _CompanyID;
    public Guid CompanyID 
    {
        get { return _CompanyID; }
        set { SetProperty<Guid>(ref _CompanyID, value); }
    }

    Guid _MaterialID;
    public Guid MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid>(ref _MaterialID, value); }
    }

    Guid _MDUnitID;
    public Guid MDUnitID 
    {
        get { return _MDUnitID; }
        set { SetProperty<Guid>(ref _MDUnitID, value); }
    }

    string _CompanyMaterialNo;
    public string CompanyMaterialNo 
    {
        get { return _CompanyMaterialNo; }
        set { SetProperty<string>(ref _CompanyMaterialNo, value); }
    }

    string _CompanyMaterialName;
    public string CompanyMaterialName 
    {
        get { return _CompanyMaterialName; }
        set { SetProperty<string>(ref _CompanyMaterialName, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
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

    DateTime? _ValidFromDate;
    public DateTime? ValidFromDate 
    {
        get { return _ValidFromDate; }
        set { SetProperty<DateTime?>(ref _ValidFromDate, value); }
    }

    DateTime? _ValidToDate;
    public DateTime? ValidToDate 
    {
        get { return _ValidToDate; }
        set { SetProperty<DateTime?>(ref _ValidToDate, value); }
    }

    bool _Blocked;
    public bool Blocked 
    {
        get { return _Blocked; }
        set { SetProperty<bool>(ref _Blocked, value); }
    }

    short _CMTypeIndex;
    public short CMTypeIndex 
    {
        get { return _CMTypeIndex; }
        set { SetProperty<short>(ref _CMTypeIndex, value); }
    }

    double? _MinStockQuantity;
    public double? MinStockQuantity 
    {
        get { return _MinStockQuantity; }
        set { SetProperty<double?>(ref _MinStockQuantity, value); }
    }

    double? _OptStockQuantity;
    public double? OptStockQuantity 
    {
        get { return _OptStockQuantity; }
        set { SetProperty<double?>(ref _OptStockQuantity, value); }
    }

    bool _Tax;
    public bool Tax 
    {
        get { return _Tax; }
        set { SetProperty<bool>(ref _Tax, value); }
    }

    private Company _Company;
    public virtual Company Company
    { 
        get => LazyLoader.Load(this, ref _Company);
        set => _Company = value;
    }

    public bool Company_IsLoaded
    {
        get
        {
            return Company != null;
        }
    }

    public virtual ReferenceEntry CompanyReference 
    {
        get { return Context.Entry(this).Reference("Company"); }
    }
    
    private ObservableHashSet<CompanyMaterialHistory> _CompanyMaterialHistory_CompanyMaterial;
    public virtual ObservableHashSet<CompanyMaterialHistory> CompanyMaterialHistory_CompanyMaterial
    {
        get => LazyLoader.Load(this, ref _CompanyMaterialHistory_CompanyMaterial);
        set => _CompanyMaterialHistory_CompanyMaterial = value;
    }

    public bool CompanyMaterialHistory_CompanyMaterial_IsLoaded
    {
        get
        {
            return CompanyMaterialHistory_CompanyMaterial != null;
        }
    }

    public virtual CollectionEntry CompanyMaterialHistory_CompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyMaterialHistory_CompanyMaterial); }
    }

    private ObservableHashSet<CompanyMaterialPickup> _CompanyMaterialPickup_CompanyMaterial;
    public virtual ObservableHashSet<CompanyMaterialPickup> CompanyMaterialPickup_CompanyMaterial
    {
        get => LazyLoader.Load(this, ref _CompanyMaterialPickup_CompanyMaterial);
        set => _CompanyMaterialPickup_CompanyMaterial = value;
    }

    public bool CompanyMaterialPickup_CompanyMaterial_IsLoaded
    {
        get
        {
            return CompanyMaterialPickup_CompanyMaterial != null;
        }
    }

    public virtual CollectionEntry CompanyMaterialPickup_CompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyMaterialPickup_CompanyMaterial); }
    }

    private ObservableHashSet<CompanyMaterialStock> _CompanyMaterialStock_CompanyMaterial;
    public virtual ObservableHashSet<CompanyMaterialStock> CompanyMaterialStock_CompanyMaterial
    {
        get => LazyLoader.Load(this, ref _CompanyMaterialStock_CompanyMaterial);
        set => _CompanyMaterialStock_CompanyMaterial = value;
    }

    public bool CompanyMaterialStock_CompanyMaterial_IsLoaded
    {
        get
        {
            return CompanyMaterialStock_CompanyMaterial != null;
        }
    }

    public virtual CollectionEntry CompanyMaterialStock_CompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyMaterialStock_CompanyMaterial); }
    }

    private ObservableHashSet<FacilityBookingCharge> _FacilityBookingCharge_InwardCPartnerCompMat;
    public virtual ObservableHashSet<FacilityBookingCharge> FacilityBookingCharge_InwardCPartnerCompMat
    {
        get => LazyLoader.Load(this, ref _FacilityBookingCharge_InwardCPartnerCompMat);
        set => _FacilityBookingCharge_InwardCPartnerCompMat = value;
    }

    public bool FacilityBookingCharge_InwardCPartnerCompMat_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_InwardCPartnerCompMat != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_InwardCPartnerCompMatReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_InwardCPartnerCompMat); }
    }

    private ObservableHashSet<FacilityBookingCharge> _FacilityBookingCharge_InwardCompanyMaterial;
    public virtual ObservableHashSet<FacilityBookingCharge> FacilityBookingCharge_InwardCompanyMaterial
    {
        get => LazyLoader.Load(this, ref _FacilityBookingCharge_InwardCompanyMaterial);
        set => _FacilityBookingCharge_InwardCompanyMaterial = value;
    }

    public bool FacilityBookingCharge_InwardCompanyMaterial_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_InwardCompanyMaterial != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_InwardCompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_InwardCompanyMaterial); }
    }

    private ObservableHashSet<FacilityBookingCharge> _FacilityBookingCharge_OutwardCPartnerCompMat;
    public virtual ObservableHashSet<FacilityBookingCharge> FacilityBookingCharge_OutwardCPartnerCompMat
    {
        get => LazyLoader.Load(this, ref _FacilityBookingCharge_OutwardCPartnerCompMat);
        set => _FacilityBookingCharge_OutwardCPartnerCompMat = value;
    }

    public bool FacilityBookingCharge_OutwardCPartnerCompMat_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_OutwardCPartnerCompMat != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_OutwardCPartnerCompMatReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_OutwardCPartnerCompMat); }
    }

    private ObservableHashSet<FacilityBookingCharge> _FacilityBookingCharge_OutwardCompanyMaterial;
    public virtual ObservableHashSet<FacilityBookingCharge> FacilityBookingCharge_OutwardCompanyMaterial
    {
        get => LazyLoader.Load(this, ref _FacilityBookingCharge_OutwardCompanyMaterial);
        set => _FacilityBookingCharge_OutwardCompanyMaterial = value;
    }

    public bool FacilityBookingCharge_OutwardCompanyMaterial_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_OutwardCompanyMaterial != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_OutwardCompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_OutwardCompanyMaterial); }
    }

    private ObservableHashSet<FacilityBooking> _FacilityBooking_InwardCompanyMaterial;
    public virtual ObservableHashSet<FacilityBooking> FacilityBooking_InwardCompanyMaterial
    {
        get => LazyLoader.Load(this, ref _FacilityBooking_InwardCompanyMaterial);
        set => _FacilityBooking_InwardCompanyMaterial = value;
    }

    public bool FacilityBooking_InwardCompanyMaterial_IsLoaded
    {
        get
        {
            return FacilityBooking_InwardCompanyMaterial != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_InwardCompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_InwardCompanyMaterial); }
    }

    private ObservableHashSet<FacilityBooking> _FacilityBooking_OutwardCompanyMaterial;
    public virtual ObservableHashSet<FacilityBooking> FacilityBooking_OutwardCompanyMaterial
    {
        get => LazyLoader.Load(this, ref _FacilityBooking_OutwardCompanyMaterial);
        set => _FacilityBooking_OutwardCompanyMaterial = value;
    }

    public bool FacilityBooking_OutwardCompanyMaterial_IsLoaded
    {
        get
        {
            return FacilityBooking_OutwardCompanyMaterial != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_OutwardCompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_OutwardCompanyMaterial); }
    }

    private ObservableHashSet<FacilityCharge> _FacilityCharge_CPartnerCompanyMaterial;
    public virtual ObservableHashSet<FacilityCharge> FacilityCharge_CPartnerCompanyMaterial
    {
        get => LazyLoader.Load(this, ref _FacilityCharge_CPartnerCompanyMaterial);
        set => _FacilityCharge_CPartnerCompanyMaterial = value;
    }

    public bool FacilityCharge_CPartnerCompanyMaterial_IsLoaded
    {
        get
        {
            return FacilityCharge_CPartnerCompanyMaterial != null;
        }
    }

    public virtual CollectionEntry FacilityCharge_CPartnerCompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityCharge_CPartnerCompanyMaterial); }
    }

    private ObservableHashSet<FacilityCharge> _FacilityCharge_CompanyMaterial;
    public virtual ObservableHashSet<FacilityCharge> FacilityCharge_CompanyMaterial
    {
        get => LazyLoader.Load(this, ref _FacilityCharge_CompanyMaterial);
        set => _FacilityCharge_CompanyMaterial = value;
    }

    public bool FacilityCharge_CompanyMaterial_IsLoaded
    {
        get
        {
            return FacilityCharge_CompanyMaterial != null;
        }
    }

    public virtual CollectionEntry FacilityCharge_CompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityCharge_CompanyMaterial); }
    }

    private ObservableHashSet<InOrderPos> _InOrderPos_PickupCompanyMaterial;
    public virtual ObservableHashSet<InOrderPos> InOrderPos_PickupCompanyMaterial
    {
        get => LazyLoader.Load(this, ref _InOrderPos_PickupCompanyMaterial);
        set => _InOrderPos_PickupCompanyMaterial = value;
    }

    public bool InOrderPos_PickupCompanyMaterial_IsLoaded
    {
        get
        {
            return InOrderPos_PickupCompanyMaterial != null;
        }
    }

    public virtual CollectionEntry InOrderPos_PickupCompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderPos_PickupCompanyMaterial); }
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
    
    private ObservableHashSet<OutOrderPos> _OutOrderPos_PickupCompanyMaterial;
    public virtual ObservableHashSet<OutOrderPos> OutOrderPos_PickupCompanyMaterial
    {
        get => LazyLoader.Load(this, ref _OutOrderPos_PickupCompanyMaterial);
        set => _OutOrderPos_PickupCompanyMaterial = value;
    }

    public bool OutOrderPos_PickupCompanyMaterial_IsLoaded
    {
        get
        {
            return OutOrderPos_PickupCompanyMaterial != null;
        }
    }

    public virtual CollectionEntry OutOrderPos_PickupCompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPos_PickupCompanyMaterial); }
    }
}
