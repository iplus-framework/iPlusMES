using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACProgramLogPropertyLog : VBEntityObject
{

    public ACProgramLogPropertyLog()
    {
    }

    private ACProgramLogPropertyLog(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ACProgramLogPropertyLogID;
    public Guid ACProgramLogPropertyLogID 
    {
        get { return _ACProgramLogPropertyLogID; }
        set { SetProperty<Guid>(ref _ACProgramLogPropertyLogID, value); }
    }

    Guid? _ACProgramLogID;
    public Guid? ACProgramLogID 
    {
        get { return _ACProgramLogID; }
        set { SetProperty<Guid?>(ref _ACProgramLogID, value); }
    }

    Guid _ACPropertyLogID;
    public Guid ACPropertyLogID 
    {
        get { return _ACPropertyLogID; }
        set { SetProperty<Guid>(ref _ACPropertyLogID, value); }
    }

    private ACPropertyLog _ACPropertyLog;
    public virtual ACPropertyLog ACPropertyLog
    { 
        get { return LazyLoader.Load(this, ref _ACPropertyLog); } 
        set { SetProperty<ACPropertyLog>(ref _ACPropertyLog, value); }
    }

    public bool ACPropertyLog_IsLoaded
    {
        get
        {
            return _ACPropertyLog != null;
        }
    }

    public virtual ReferenceEntry ACPropertyLogReference 
    {
        get { return Context.Entry(this).Reference("ACPropertyLog"); }
    }
    }
