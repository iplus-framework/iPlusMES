using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MaintACClass : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public MaintACClass()
    {
    }

    private MaintACClass(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MaintACClassID;
    public Guid MaintACClassID 
    {
        get { return _MaintACClassID; }
        set { SetProperty<Guid>(ref _MaintACClassID, value); }
    }

    Guid _VBiACClassID;
    public Guid VBiACClassID 
    {
        get { return _VBiACClassID; }
        set { SetProperty<Guid>(ref _VBiACClassID, value); }
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

    private ICollection<MaintACClassProperty> _MaintACClassProperty_MaintACClass;
    public virtual ICollection<MaintACClassProperty> MaintACClassProperty_MaintACClass
    {
        get => LazyLoader.Load(this, ref _MaintACClassProperty_MaintACClass);
        set => _MaintACClassProperty_MaintACClass = value;
    }

    public bool MaintACClassProperty_MaintACClass_IsLoaded
    {
        get
        {
            return MaintACClassProperty_MaintACClass != null;
        }
    }

    public virtual CollectionEntry MaintACClassProperty_MaintACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintACClassProperty_MaintACClass); }
    }

    private ICollection<MaintOrder> _MaintOrder_MaintACClass;
    public virtual ICollection<MaintOrder> MaintOrder_MaintACClass
    {
        get => LazyLoader.Load(this, ref _MaintOrder_MaintACClass);
        set => _MaintOrder_MaintACClass = value;
    }

    public bool MaintOrder_MaintACClass_IsLoaded
    {
        get
        {
            return MaintOrder_MaintACClass != null;
        }
    }

    public virtual CollectionEntry MaintOrder_MaintACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintOrder_MaintACClass); }
    }

    private ACClass _VBiACClass;
    public virtual ACClass VBiACClass
    { 
        get => LazyLoader.Load(this, ref _VBiACClass);
        set => _VBiACClass = value;
    }

    public bool VBiACClass_IsLoaded
    {
        get
        {
            return VBiACClass != null;
        }
    }

    public virtual ReferenceEntry VBiACClassReference 
    {
        get { return Context.Entry(this).Reference("VBiACClass"); }
    }
    }
