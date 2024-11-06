using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACClassRouteUsagePos : VBEntityObject
{

    public ACClassRouteUsagePos()
    {
    }

    private ACClassRouteUsagePos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ACClassRouteUsagePosID;
    public Guid ACClassRouteUsagePosID 
    {
        get { return _ACClassRouteUsagePosID; }
        set { SetProperty<Guid>(ref _ACClassRouteUsagePosID, value); }
    }

    Guid _ACClassRouteUsageID;
    public Guid ACClassRouteUsageID 
    {
        get { return _ACClassRouteUsageID; }
        set { SetProperty<Guid>(ref _ACClassRouteUsageID, value); }
    }

    int _HashCode;
    public int HashCode 
    {
        get { return _HashCode; }
        set { SetProperty<int>(ref _HashCode, value); }
    }

    private ACClassRouteUsage _ACClassRouteUsage;
    public virtual ACClassRouteUsage ACClassRouteUsage
    { 
        get { return LazyLoader.Load(this, ref _ACClassRouteUsage); } 
        set { SetProperty<ACClassRouteUsage>(ref _ACClassRouteUsage, value); }
    }

    public bool ACClassRouteUsage_IsLoaded
    {
        get
        {
            return ACClassRouteUsage != null;
        }
    }

    public virtual ReferenceEntry ACClassRouteUsageReference 
    {
        get { return Context.Entry(this).Reference("ACClassRouteUsage"); }
    }
    }
