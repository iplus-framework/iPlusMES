using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MaterialWFConnection : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public MaterialWFConnection()
    {
    }

    private MaterialWFConnection(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _MaterialWFConnectionID;
    public Guid MaterialWFConnectionID 
    {
        get { return _MaterialWFConnectionID; }
        set { SetProperty<Guid>(ref _MaterialWFConnectionID, value); }
    }

    Guid _MaterialID;
    public Guid MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid>(ref _MaterialID, value); }
    }

    Guid _ACClassWFID;
    public Guid ACClassWFID 
    {
        get { return _ACClassWFID; }
        set { SetProperty<Guid>(ref _ACClassWFID, value); }
    }

    Guid _MaterialWFACClassMethodID;
    public Guid MaterialWFACClassMethodID 
    {
        get { return _MaterialWFACClassMethodID; }
        set { SetProperty<Guid>(ref _MaterialWFACClassMethodID, value); }
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

    private ACClassWF _ACClassWF;
    public virtual ACClassWF ACClassWF
    { 
        get => LazyLoader.Load(this, ref _ACClassWF);
        set => _ACClassWF = value;
    }

    public bool ACClassWF_IsLoaded
    {
        get
        {
            return ACClassWF != null;
        }
    }

    public virtual ReferenceEntry ACClassWFReference 
    {
        get { return Context.Entry(this).Reference("ACClassWF"); }
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
    
    private MaterialWFACClassMethod _MaterialWFACClassMethod;
    public virtual MaterialWFACClassMethod MaterialWFACClassMethod
    { 
        get => LazyLoader.Load(this, ref _MaterialWFACClassMethod);
        set => _MaterialWFACClassMethod = value;
    }

    public bool MaterialWFACClassMethod_IsLoaded
    {
        get
        {
            return MaterialWFACClassMethod != null;
        }
    }

    public virtual ReferenceEntry MaterialWFACClassMethodReference 
    {
        get { return Context.Entry(this).Reference("MaterialWFACClassMethod"); }
    }
    }
