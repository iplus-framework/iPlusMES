// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv3MixPointFacility : VBEntityObject
{

    public TandTv3MixPointFacility()
    {
    }

    private TandTv3MixPointFacility(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _TandTv3MixPointFacilityID;
    public Guid TandTv3MixPointFacilityID 
    {
        get { return _TandTv3MixPointFacilityID; }
        set { SetProperty<Guid>(ref _TandTv3MixPointFacilityID, value); }
    }

    Guid _TandTv3MixPointID;
    public Guid TandTv3MixPointID 
    {
        get { return _TandTv3MixPointID; }
        set { SetProperty<Guid>(ref _TandTv3MixPointID, value); }
    }

    string _TandTv3MDBookingDirectionID;
    public string TandTv3MDBookingDirectionID 
    {
        get { return _TandTv3MDBookingDirectionID; }
        set { SetProperty<string>(ref _TandTv3MDBookingDirectionID, value); }
    }

    Guid _FacilityID;
    public Guid FacilityID 
    {
        get { return _FacilityID; }
        set { SetProperty<Guid>(ref _FacilityID, value); }
    }

    private Facility _Facility;
    public virtual Facility Facility
    { 
        get { return LazyLoader.Load(this, ref _Facility); } 
        set { SetProperty<Facility>(ref _Facility, value); }
    }

    public bool Facility_IsLoaded
    {
        get
        {
            return Facility != null;
        }
    }

    public virtual ReferenceEntry FacilityReference 
    {
        get { return Context.Entry(this).Reference("Facility"); }
    }
    
    private TandTv3MDBookingDirection _TandTv3MDBookingDirection;
    public virtual TandTv3MDBookingDirection TandTv3MDBookingDirection
    { 
        get { return LazyLoader.Load(this, ref _TandTv3MDBookingDirection); } 
        set { SetProperty<TandTv3MDBookingDirection>(ref _TandTv3MDBookingDirection, value); }
    }

    public bool TandTv3MDBookingDirection_IsLoaded
    {
        get
        {
            return TandTv3MDBookingDirection != null;
        }
    }

    public virtual ReferenceEntry TandTv3MDBookingDirectionReference 
    {
        get { return Context.Entry(this).Reference("TandTv3MDBookingDirection"); }
    }
    
    private TandTv3MixPoint _TandTv3MixPoint;
    public virtual TandTv3MixPoint TandTv3MixPoint
    { 
        get { return LazyLoader.Load(this, ref _TandTv3MixPoint); } 
        set { SetProperty<TandTv3MixPoint>(ref _TandTv3MixPoint, value); }
    }

    public bool TandTv3MixPoint_IsLoaded
    {
        get
        {
            return TandTv3MixPoint != null;
        }
    }

    public virtual ReferenceEntry TandTv3MixPointReference 
    {
        get { return Context.Entry(this).Reference("TandTv3MixPoint"); }
    }
    }
