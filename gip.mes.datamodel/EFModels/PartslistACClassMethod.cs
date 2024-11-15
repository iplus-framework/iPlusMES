using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class PartslistACClassMethod : VBEntityObject
{

    public PartslistACClassMethod()
    {
    }

    private PartslistACClassMethod(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _PartslistACClassMethodID;
    public Guid PartslistACClassMethodID 
    {
        get { return _PartslistACClassMethodID; }
        set { SetProperty<Guid>(ref _PartslistACClassMethodID, value); }
    }

    Guid _PartslistID;
    public Guid PartslistID 
    {
        get { return _PartslistID; }
        set { SetProperty<Guid>(ref _PartslistID, value); }
    }

    Guid _MaterialWFACClassMethodID;
    public Guid MaterialWFACClassMethodID 
    {
        get { return _MaterialWFACClassMethodID; }
        set { SetProperty<Guid>(ref _MaterialWFACClassMethodID, value); }
    }

    short _UsedInPhaseIndex;
    public short UsedInPhaseIndex 
    {
        get { return _UsedInPhaseIndex; }
        set { SetProperty<short>(ref _UsedInPhaseIndex, value); }
    }

    DateTime _InsertDate;
    public DateTime InsertDate 
    {
        get { return _InsertDate; }
        set { SetProperty<DateTime>(ref _InsertDate, value); }
    }

    string _InsertName;
    public string InsertName 
    {
        get { return _InsertName; }
        set { SetProperty<string>(ref _InsertName, value); }
    }

    private MaterialWFACClassMethod _MaterialWFACClassMethod;
    public virtual MaterialWFACClassMethod MaterialWFACClassMethod
    { 
        get { return LazyLoader.Load(this, ref _MaterialWFACClassMethod); } 
        set { SetProperty<MaterialWFACClassMethod>(ref _MaterialWFACClassMethod, value); }
    }

    public bool MaterialWFACClassMethod_IsLoaded
    {
        get
        {
            return _MaterialWFACClassMethod != null;
        }
    }

    public virtual ReferenceEntry MaterialWFACClassMethodReference 
    {
        get { return Context.Entry(this).Reference("MaterialWFACClassMethod"); }
    }
    
    private Partslist _Partslist;
    public virtual Partslist Partslist
    { 
        get { return LazyLoader.Load(this, ref _Partslist); } 
        set { SetProperty<Partslist>(ref _Partslist, value); }
    }

    public bool Partslist_IsLoaded
    {
        get
        {
            return _Partslist != null;
        }
    }

    public virtual ReferenceEntry PartslistReference 
    {
        get { return Context.Entry(this).Reference("Partslist"); }
    }
    }
