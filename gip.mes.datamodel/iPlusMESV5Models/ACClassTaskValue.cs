using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACClassTaskValue : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public ACClassTaskValue()
    {
    }

    private ACClassTaskValue(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _ACClassTaskValueID;
    public Guid ACClassTaskValueID 
    {
        get { return _ACClassTaskValueID; }
        set { SetProperty<Guid>(ref _ACClassTaskValueID, value); }
    }

    Guid _ACClassTaskID;
    public Guid ACClassTaskID 
    {
        get { return _ACClassTaskID; }
        set { SetProperty<Guid>(ref _ACClassTaskID, value); }
    }

    Guid _ACClassPropertyID;
    public Guid ACClassPropertyID 
    {
        get { return _ACClassPropertyID; }
        set { SetProperty<Guid>(ref _ACClassPropertyID, value); }
    }

    Guid? _VBUserID;
    public Guid? VBUserID 
    {
        get { return _VBUserID; }
        set { SetProperty<Guid?>(ref _VBUserID, value); }
    }

    string _XMLValue;
    public string XMLValue 
    {
        get { return _XMLValue; }
        set { SetProperty<string>(ref _XMLValue, value); }
    }

    string _XMLValue2;
    public string XMLValue2 
    {
        get { return _XMLValue2; }
        set { SetProperty<string>(ref _XMLValue2, value); }
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
    
    private ACClassTask _ACClassTask;
    public virtual ACClassTask ACClassTask
    { 
        get => LazyLoader.Load(this, ref _ACClassTask);
        set => _ACClassTask = value;
    }

    public bool ACClassTask_IsLoaded
    {
        get
        {
            return ACClassTask != null;
        }
    }

    public virtual ReferenceEntry ACClassTaskReference 
    {
        get { return Context.Entry(this).Reference("ACClassTask"); }
    }
    
    private ICollection<ACClassTaskValuePos> _ACClassTaskValuePo_ACClassTaskValue;
    public virtual ICollection<ACClassTaskValuePos> ACClassTaskValuePo_ACClassTaskValue
    {
        get => LazyLoader.Load(this, ref _ACClassTaskValuePo_ACClassTaskValue);
        set => _ACClassTaskValuePo_ACClassTaskValue = value;
    }

    public bool ACClassTaskValuePo_ACClassTaskValue_IsLoaded
    {
        get
        {
            return ACClassTaskValuePo_ACClassTaskValue != null;
        }
    }

    public virtual CollectionEntry ACClassTaskValuePo_ACClassTaskValueReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassTaskValuePo_ACClassTaskValue); }
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
