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
        get { return LazyLoader.Load(this, ref _Company); } 
        set { SetProperty<Company>(ref _Company, value); }
    }

    public bool Company_IsLoaded
    {
        get
        {
            return _Company != null;
        }
    }

    public virtual ReferenceEntry CompanyReference 
    {
        get { return Context.Entry(this).Reference("Company"); }
    }
    
    private ICollection<CompanyMaterialHistory> _CompanyMaterialHistory_CompanyMaterial;
    public virtual ICollection<CompanyMaterialHistory> CompanyMaterialHistory_CompanyMaterial
    {
        get { return LazyLoader.Load(this, ref _CompanyMaterialHistory_CompanyMaterial); }
        set { _CompanyMaterialHistory_CompanyMaterial = value; }
    }

    public bool CompanyMaterialHistory_CompanyMaterial_IsLoaded
    {
        get
        {
            return _CompanyMaterialHistory_CompanyMaterial != null;
        }
    }

    public virtual CollectionEntry CompanyMaterialHistory_CompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyMaterialHistory_CompanyMaterial); }
    }

    private ICollection<CompanyMaterialPickup> _CompanyMaterialPickup_CompanyMaterial;
    public virtual ICollection<CompanyMaterialPickup> CompanyMaterialPickup_CompanyMaterial
    {
        get { return LazyLoader.Load(this, ref _CompanyMaterialPickup_CompanyMaterial); }
        set { _CompanyMaterialPickup_CompanyMaterial = value; }
    }

    public bool CompanyMaterialPickup_CompanyMaterial_IsLoaded
    {
        get
        {
            return _CompanyMaterialPickup_CompanyMaterial != null;
        }
    }

    public virtual CollectionEntry CompanyMaterialPickup_CompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyMaterialPickup_CompanyMaterial); }
    }

    private ICollection<CompanyMaterialStock> _CompanyMaterialStock_CompanyMaterial;
    public virtual ICollection<CompanyMaterialStock> CompanyMaterialStock_CompanyMaterial
    {
        get { return LazyLoader.Load(this, ref _CompanyMaterialStock_CompanyMaterial); }
        set { _CompanyMaterialStock_CompanyMaterial = value; }
    }

    public bool CompanyMaterialStock_CompanyMaterial_IsLoaded
    {
        get
        {
            return _CompanyMaterialStock_CompanyMaterial != null;
        }
    }

    public virtual CollectionEntry CompanyMaterialStock_CompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyMaterialStock_CompanyMaterial); }
    }

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_InwardCPartnerCompMat;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_InwardCPartnerCompMat
    {
        get { return LazyLoader.Load(this, ref _FacilityBookingCharge_InwardCPartnerCompMat); }
        set { _FacilityBookingCharge_InwardCPartnerCompMat = value; }
    }

    public bool FacilityBookingCharge_InwardCPartnerCompMat_IsLoaded
    {
        get
        {
            return _FacilityBookingCharge_InwardCPartnerCompMat != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_InwardCPartnerCompMatReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_InwardCPartnerCompMat); }
    }

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_InwardCompanyMaterial;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_InwardCompanyMaterial
    {
        get { return LazyLoader.Load(this, ref _FacilityBookingCharge_InwardCompanyMaterial); }
        set { _FacilityBookingCharge_InwardCompanyMaterial = value; }
    }

    public bool FacilityBookingCharge_InwardCompanyMaterial_IsLoaded
    {
        get
        {
            return _FacilityBookingCharge_InwardCompanyMaterial != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_InwardCompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_InwardCompanyMaterial); }
    }

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_OutwardCPartnerCompMat;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_OutwardCPartnerCompMat
    {
        get { return LazyLoader.Load(this, ref _FacilityBookingCharge_OutwardCPartnerCompMat); }
        set { _FacilityBookingCharge_OutwardCPartnerCompMat = value; }
    }

    public bool FacilityBookingCharge_OutwardCPartnerCompMat_IsLoaded
    {
        get
        {
            return _FacilityBookingCharge_OutwardCPartnerCompMat != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_OutwardCPartnerCompMatReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_OutwardCPartnerCompMat); }
    }

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_OutwardCompanyMaterial;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_OutwardCompanyMaterial
    {
        get { return LazyLoader.Load(this, ref _FacilityBookingCharge_OutwardCompanyMaterial); }
        set { _FacilityBookingCharge_OutwardCompanyMaterial = value; }
    }

    public bool FacilityBookingCharge_OutwardCompanyMaterial_IsLoaded
    {
        get
        {
            return _FacilityBookingCharge_OutwardCompanyMaterial != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_OutwardCompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_OutwardCompanyMaterial); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_InwardCompanyMaterial;
    public virtual ICollection<FacilityBooking> FacilityBooking_InwardCompanyMaterial
    {
        get { return LazyLoader.Load(this, ref _FacilityBooking_InwardCompanyMaterial); }
        set { _FacilityBooking_InwardCompanyMaterial = value; }
    }

    public bool FacilityBooking_InwardCompanyMaterial_IsLoaded
    {
        get
        {
            return _FacilityBooking_InwardCompanyMaterial != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_InwardCompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_InwardCompanyMaterial); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_OutwardCompanyMaterial;
    public virtual ICollection<FacilityBooking> FacilityBooking_OutwardCompanyMaterial
    {
        get { return LazyLoader.Load(this, ref _FacilityBooking_OutwardCompanyMaterial); }
        set { _FacilityBooking_OutwardCompanyMaterial = value; }
    }

    public bool FacilityBooking_OutwardCompanyMaterial_IsLoaded
    {
        get
        {
            return _FacilityBooking_OutwardCompanyMaterial != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_OutwardCompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_OutwardCompanyMaterial); }
    }

    private ICollection<FacilityCharge> _FacilityCharge_CPartnerCompanyMaterial;
    public virtual ICollection<FacilityCharge> FacilityCharge_CPartnerCompanyMaterial
    {
        get { return LazyLoader.Load(this, ref _FacilityCharge_CPartnerCompanyMaterial); }
        set { _FacilityCharge_CPartnerCompanyMaterial = value; }
    }

    public bool FacilityCharge_CPartnerCompanyMaterial_IsLoaded
    {
        get
        {
            return _FacilityCharge_CPartnerCompanyMaterial != null;
        }
    }

    public virtual CollectionEntry FacilityCharge_CPartnerCompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityCharge_CPartnerCompanyMaterial); }
    }

    private ICollection<FacilityCharge> _FacilityCharge_CompanyMaterial;
    public virtual ICollection<FacilityCharge> FacilityCharge_CompanyMaterial
    {
        get { return LazyLoader.Load(this, ref _FacilityCharge_CompanyMaterial); }
        set { _FacilityCharge_CompanyMaterial = value; }
    }

    public bool FacilityCharge_CompanyMaterial_IsLoaded
    {
        get
        {
            return _FacilityCharge_CompanyMaterial != null;
        }
    }

    public virtual CollectionEntry FacilityCharge_CompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityCharge_CompanyMaterial); }
    }

    private ICollection<InOrderPos> _InOrderPos_PickupCompanyMaterial;
    public virtual ICollection<InOrderPos> InOrderPos_PickupCompanyMaterial
    {
        get { return LazyLoader.Load(this, ref _InOrderPos_PickupCompanyMaterial); }
        set { _InOrderPos_PickupCompanyMaterial = value; }
    }

    public bool InOrderPos_PickupCompanyMaterial_IsLoaded
    {
        get
        {
            return _InOrderPos_PickupCompanyMaterial != null;
        }
    }

    public virtual CollectionEntry InOrderPos_PickupCompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderPos_PickupCompanyMaterial); }
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
            return _MDUnit != null;
        }
    }

    public virtual ReferenceEntry MDUnitReference 
    {
        get { return Context.Entry(this).Reference("MDUnit"); }
    }
    
    private Material _Material;
    public virtual Material Material
    { 
        get { return LazyLoader.Load(this, ref _Material); } 
        set { SetProperty<Material>(ref _Material, value); }
    }

    public bool Material_IsLoaded
    {
        get
        {
            return _Material != null;
        }
    }

    public virtual ReferenceEntry MaterialReference 
    {
        get { return Context.Entry(this).Reference("Material"); }
    }
    
    private ICollection<OutOrderPos> _OutOrderPos_PickupCompanyMaterial;
    public virtual ICollection<OutOrderPos> OutOrderPos_PickupCompanyMaterial
    {
        get { return LazyLoader.Load(this, ref _OutOrderPos_PickupCompanyMaterial); }
        set { _OutOrderPos_PickupCompanyMaterial = value; }
    }

    public bool OutOrderPos_PickupCompanyMaterial_IsLoaded
    {
        get
        {
            return _OutOrderPos_PickupCompanyMaterial != null;
        }
    }

    public virtual CollectionEntry OutOrderPos_PickupCompanyMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPos_PickupCompanyMaterial); }
    }
}
