using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class FacilityMaterial : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public FacilityMaterial()
    {
    }

    private FacilityMaterial(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _FacilityMaterialID;
    public Guid FacilityMaterialID 
    {
        get { return _FacilityMaterialID; }
        set { SetProperty<Guid>(ref _FacilityMaterialID, value); }
    }

    Guid _FacilityID;
    public Guid FacilityID 
    {
        get { return _FacilityID; }
        set { SetForeignKeyProperty<Guid>(ref _FacilityID, value, "Facility", _Facility, Facility != null ? Facility.FacilityID : default(Guid)); }
    }

    Guid _MaterialID;
    public Guid MaterialID 
    {
        get { return _MaterialID; }
        set { SetForeignKeyProperty<Guid>(ref _MaterialID, value, "Material", _Material, Material != null ? Material.MaterialID : default(Guid)); }
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

    double? _MaxStockQuantity;
    public double? MaxStockQuantity 
    {
        get { return _MaxStockQuantity; }
        set { SetProperty<double?>(ref _MaxStockQuantity, value); }
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

    double? _Throughput;
    public double? Throughput 
    {
        get { return _Throughput; }
        set { SetProperty<double?>(ref _Throughput, value); }
    }

    double? _ThroughputMax;
    public double? ThroughputMax 
    {
        get { return _ThroughputMax; }
        set { SetProperty<double?>(ref _ThroughputMax, value); }
    }

    double? _ThroughputMin;
    public double? ThroughputMin 
    {
        get { return _ThroughputMin; }
        set { SetProperty<double?>(ref _ThroughputMin, value); }
    }

    short _ThroughputAuto;
    public short ThroughputAuto 
    {
        get { return _ThroughputAuto; }
        set { SetProperty<short>(ref _ThroughputAuto, value); }
    }

    private Facility _Facility;
    public virtual Facility Facility
    { 
        get { return LazyLoader.Load(this, ref _Facility); } 
        set { SetProperty<Facility>(ref _Facility, value); }
    }

    public bool Facility_IsLoaded
    {
        get
        {
            return _Facility != null;
        }
    }

    public virtual ReferenceEntry FacilityReference 
    {
        get { return Context.Entry(this).Reference("Facility"); }
    }
    
    private ICollection<FacilityMaterialOEE> _FacilityMaterialOEE_FacilityMaterial;
    public virtual ICollection<FacilityMaterialOEE> FacilityMaterialOEE_FacilityMaterial
    {
        get { return LazyLoader.Load(this, ref _FacilityMaterialOEE_FacilityMaterial); }
        set { SetProperty<ICollection<FacilityMaterialOEE>>(ref _FacilityMaterialOEE_FacilityMaterial, value); }
    }

    public bool FacilityMaterialOEE_FacilityMaterial_IsLoaded
    {
        get
        {
            return _FacilityMaterialOEE_FacilityMaterial != null;
        }
    }

    public virtual CollectionEntry FacilityMaterialOEE_FacilityMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityMaterialOEE_FacilityMaterial); }
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
    }
