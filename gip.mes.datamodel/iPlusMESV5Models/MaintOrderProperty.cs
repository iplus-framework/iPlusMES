using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MaintOrderProperty : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public MaintOrderProperty()
    {
    }

    private MaintOrderProperty(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _MaintOrderPropertyID;
    public Guid MaintOrderPropertyID 
    {
        get { return _MaintOrderPropertyID; }
        set { SetProperty<Guid>(ref _MaintOrderPropertyID, value); }
    }

    Guid _MaintOrderID;
    public Guid MaintOrderID 
    {
        get { return _MaintOrderID; }
        set { SetProperty<Guid>(ref _MaintOrderID, value); }
    }

    Guid _MaintACClassPropertyID;
    public Guid MaintACClassPropertyID 
    {
        get { return _MaintACClassPropertyID; }
        set { SetProperty<Guid>(ref _MaintACClassPropertyID, value); }
    }

    string _SetValue;
    public string SetValue 
    {
        get { return _SetValue; }
        set { SetProperty<string>(ref _SetValue, value); }
    }

    string _ActValue;
    public string ActValue 
    {
        get { return _ActValue; }
        set { SetProperty<string>(ref _ActValue, value); }
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

    private MaintACClassProperty _MaintACClassProperty;
    public virtual MaintACClassProperty MaintACClassProperty
    { 
        get => LazyLoader.Load(this, ref _MaintACClassProperty);
        set => _MaintACClassProperty = value;
    }

    public bool MaintACClassProperty_IsLoaded
    {
        get
        {
            return MaintACClassProperty != null;
        }
    }

    public virtual ReferenceEntry MaintACClassPropertyReference 
    {
        get { return Context.Entry(this).Reference("MaintACClassProperty"); }
    }
    
    private MaintOrder _MaintOrder;
    public virtual MaintOrder MaintOrder
    { 
        get => LazyLoader.Load(this, ref _MaintOrder);
        set => _MaintOrder = value;
    }

    public bool MaintOrder_IsLoaded
    {
        get
        {
            return MaintOrder != null;
        }
    }

    public virtual ReferenceEntry MaintOrderReference 
    {
        get { return Context.Entry(this).Reference("MaintOrder"); }
    }
    }
