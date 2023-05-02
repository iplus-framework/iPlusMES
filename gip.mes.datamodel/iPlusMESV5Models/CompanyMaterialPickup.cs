using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class CompanyMaterialPickup : VBEntityObject
{

    public CompanyMaterialPickup()
    {
    }

    private CompanyMaterialPickup(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _CompanyMaterialPickupID;
    public Guid CompanyMaterialPickupID 
    {
        get { return _CompanyMaterialPickupID; }
        set { SetProperty<Guid>(ref _CompanyMaterialPickupID, value); }
    }

    Guid _CompanyMaterialID;
    public Guid CompanyMaterialID 
    {
        get { return _CompanyMaterialID; }
        set { SetProperty<Guid>(ref _CompanyMaterialID, value); }
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
    }
