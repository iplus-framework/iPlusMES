using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACChangeLog : VBEntityObject 
{

    public ACChangeLog()
    {
    }

    private ACChangeLog(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _ACChangeLogID;
    public Guid ACChangeLogID 
    {
        get { return _ACChangeLogID; }
        set { SetProperty<Guid>(ref _ACChangeLogID, value); }
    }

    Guid _ACClassID;
    public Guid ACClassID 
    {
        get { return _ACClassID; }
        set { SetProperty<Guid>(ref _ACClassID, value); }
    }

    Guid _ACClassPropertyID;
    public Guid ACClassPropertyID 
    {
        get { return _ACClassPropertyID; }
        set { SetProperty<Guid>(ref _ACClassPropertyID, value); }
    }

    Guid _EntityKey;
    public Guid EntityKey 
    {
        get { return _EntityKey; }
        set { SetProperty<Guid>(ref _EntityKey, value); }
    }

    string _XMLValue;
    public string XMLValue 
    {
        get { return _XMLValue; }
        set { SetProperty<string>(ref _XMLValue, value); }
    }

    DateTime _ChangeDate;
    public DateTime ChangeDate 
    {
        get { return _ChangeDate; }
        set { SetProperty<DateTime>(ref _ChangeDate, value); }
    }

    Guid _VBUserID;
    public Guid VBUserID 
    {
        get { return _VBUserID; }
        set { SetProperty<Guid>(ref _VBUserID, value); }
    }

    bool _Deleted;
    public bool Deleted 
    {
        get { return _Deleted; }
        set { SetProperty<bool>(ref _Deleted, value); }
    }

    private ACClass _ACClass;
    public virtual ACClass ACClass
    { 
        get => LazyLoader.Load(this, ref _ACClass);
        set => _ACClass = value;
    }

    public bool ACClass_IsLoaded
    {
        get
        {
            return ACClass != null;
        }
    }

    public virtual ReferenceEntry ACClassReference 
    {
        get { return Context.Entry(this).Reference("ACClass"); }
    }
    
    private ACClassProperty _ACClassProperty;
    public virtual ACClassProperty ACClassProperty
    { 
        get => LazyLoader.Load(this, ref _ACClassProperty);
        set => _ACClassProperty = value;
    }

    public bool ACClassProperty_IsLoaded
    {
        get
        {
            return ACClassProperty != null;
        }
    }

    public virtual ReferenceEntry ACClassPropertyReference 
    {
        get { return Context.Entry(this).Reference("ACClassProperty"); }
    }
    
    private VBUser _VBUser;
    public virtual VBUser VBUser
    { 
        get => LazyLoader.Load(this, ref _VBUser);
        set => _VBUser = value;
    }

    public bool VBUser_IsLoaded
    {
        get
        {
            return VBUser != null;
        }
    }

    public virtual ReferenceEntry VBUserReference 
    {
        get { return Context.Entry(this).Reference("VBUser"); }
    }
    }
