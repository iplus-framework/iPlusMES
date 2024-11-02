// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACClassRouteUsage : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public ACClassRouteUsage()
    {
    }

    private ACClassRouteUsage(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ACClassRouteUsageID;
    public Guid ACClassRouteUsageID 
    {
        get { return _ACClassRouteUsageID; }
        set { SetProperty<Guid>(ref _ACClassRouteUsageID, value); }
    }

    Guid _ACClassID;
    public Guid ACClassID 
    {
        get { return _ACClassID; }
        set { SetProperty<Guid>(ref _ACClassID, value); }
    }

    int _UseFactor;
    public int UseFactor 
    {
        get { return _UseFactor; }
        set { SetProperty<int>(ref _UseFactor, value); }
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

    private ICollection<ACClassRouteUsageGroup> _ACClassRouteUsageGroup_ACClassRouteUsage;
    public virtual ICollection<ACClassRouteUsageGroup> ACClassRouteUsageGroup_ACClassRouteUsage
    {
        get { return LazyLoader.Load(this, ref _ACClassRouteUsageGroup_ACClassRouteUsage); }
        set { _ACClassRouteUsageGroup_ACClassRouteUsage = value; }
    }

    public bool ACClassRouteUsageGroup_ACClassRouteUsage_IsLoaded
    {
        get
        {
            return ACClassRouteUsageGroup_ACClassRouteUsage != null;
        }
    }

    public virtual CollectionEntry ACClassRouteUsageGroup_ACClassRouteUsageReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassRouteUsageGroup_ACClassRouteUsage); }
    }

    private ICollection<ACClassRouteUsagePos> _ACClassRouteUsagePos_ACClassRouteUsage;
    public virtual ICollection<ACClassRouteUsagePos> ACClassRouteUsagePos_ACClassRouteUsage
    {
        get { return LazyLoader.Load(this, ref _ACClassRouteUsagePos_ACClassRouteUsage); }
        set { _ACClassRouteUsagePos_ACClassRouteUsage = value; }
    }

    public bool ACClassRouteUsagePos_ACClassRouteUsage_IsLoaded
    {
        get
        {
            return ACClassRouteUsagePos_ACClassRouteUsage != null;
        }
    }

    public virtual CollectionEntry ACClassRouteUsagePos_ACClassRouteUsageReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassRouteUsagePos_ACClassRouteUsage); }
    }
}
