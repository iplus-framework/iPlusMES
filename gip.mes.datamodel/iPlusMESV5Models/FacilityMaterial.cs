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
        set { SetProperty<Guid>(ref _FacilityID, value); }
    }

    Guid _MaterialID;
    public Guid MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid>(ref _MaterialID, value); }
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
    }
