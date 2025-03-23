using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACPropertyLog : VBEntityObject
{

    public ACPropertyLog()
    {
    }

    private ACPropertyLog(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ACPropertyLogID;
    public Guid ACPropertyLogID 
    {
        get { return _ACPropertyLogID; }
        set { SetProperty<Guid>(ref _ACPropertyLogID, value); }
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

    Guid? _ACProgramLogID;
    public Guid? ACProgramLogID 
    {
        get { return _ACProgramLogID; }
        set { SetProperty<Guid?>(ref _ACProgramLogID, value); }
    }

    DateTime _EventTime;
    public DateTime EventTime 
    {
        get { return _EventTime; }
        set { SetProperty<DateTime>(ref _EventTime, value); }
    }

    string _Value;
    public string Value 
    {
        get { return _Value; }
        set { SetProperty<string>(ref _Value, value); }
    }

    Guid? _ACClassMessageID;
    public Guid? ACClassMessageID 
    {
        get { return _ACClassMessageID; }
        set { SetProperty<Guid?>(ref _ACClassMessageID, value); }
    }

    private ACClass _ACClass;
    public virtual ACClass ACClass
    { 
        get { return LazyLoader.Load(this, ref _ACClass); } 
        set { SetProperty<ACClass>(ref _ACClass, value); }
    }

    public bool ACClass_IsLoaded
    {
        get
        {
            return _ACClass != null;
        }
    }

    public virtual ReferenceEntry ACClassReference 
    {
        get { return Context.Entry(this).Reference("ACClass"); }
    }
    
    private ACClassProperty _ACClassProperty;
    public virtual ACClassProperty ACClassProperty
    { 
        get { return LazyLoader.Load(this, ref _ACClassProperty); } 
        set { SetProperty<ACClassProperty>(ref _ACClassProperty, value); }
    }

    public bool ACClassProperty_IsLoaded
    {
        get
        {
            return _ACClassProperty != null;
        }
    }

    public virtual ReferenceEntry ACClassPropertyReference 
    {
        get { return Context.Entry(this).Reference("ACClassProperty"); }
    }
    
    private ICollection<ACProgramLogPropertyLog> _ACProgramLogPropertyLog_ACPropertyLog;
    public virtual ICollection<ACProgramLogPropertyLog> ACProgramLogPropertyLog_ACPropertyLog
    {
        get { return LazyLoader.Load(this, ref _ACProgramLogPropertyLog_ACPropertyLog); }
        set { _ACProgramLogPropertyLog_ACPropertyLog = value; }
    }

    public bool ACProgramLogPropertyLog_ACPropertyLog_IsLoaded
    {
        get
        {
            return _ACProgramLogPropertyLog_ACPropertyLog != null;
        }
    }

    public virtual CollectionEntry ACProgramLogPropertyLog_ACPropertyLogReference
    {
        get { return Context.Entry(this).Collection(c => c.ACProgramLogPropertyLog_ACPropertyLog); }
    }
}
