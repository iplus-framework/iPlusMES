// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACClassRouteUsageGroup : VBEntityObject
{

    public ACClassRouteUsageGroup()
    {
    }

    private ACClassRouteUsageGroup(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ACClassRouteUsageGroupID;
    public Guid ACClassRouteUsageGroupID 
    {
        get { return _ACClassRouteUsageGroupID; }
        set { SetProperty<Guid>(ref _ACClassRouteUsageGroupID, value); }
    }

    Guid _ACClassRouteUsageID;
    public Guid ACClassRouteUsageID 
    {
        get { return _ACClassRouteUsageID; }
        set { SetProperty<Guid>(ref _ACClassRouteUsageID, value); }
    }

    Guid _GroupID;
    public Guid GroupID 
    {
        get { return _GroupID; }
        set { SetProperty<Guid>(ref _GroupID, value); }
    }

    int _UseFactor;
    public int UseFactor 
    {
        get { return _UseFactor; }
        set { SetProperty<int>(ref _UseFactor, value); }
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
