﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv3MixPointFacilityPreBooking : VBEntityObject
{

    public TandTv3MixPointFacilityPreBooking()
    {
    }

    private TandTv3MixPointFacilityPreBooking(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _TandTv3MixPointFacilityPreBookingID;
    public Guid TandTv3MixPointFacilityPreBookingID 
    {
        get { return _TandTv3MixPointFacilityPreBookingID; }
        set { SetProperty<Guid>(ref _TandTv3MixPointFacilityPreBookingID, value); }
    }

    Guid _TandTv3MixPointID;
    public Guid TandTv3MixPointID 
    {
        get { return _TandTv3MixPointID; }
        set { SetProperty<Guid>(ref _TandTv3MixPointID, value); }
    }

    Guid _FacilityPreBookingID;
    public Guid FacilityPreBookingID 
    {
        get { return _FacilityPreBookingID; }
        set { SetProperty<Guid>(ref _FacilityPreBookingID, value); }
    }

    private FacilityPreBooking _FacilityPreBooking;
    public virtual FacilityPreBooking FacilityPreBooking
    { 
        get => LazyLoader.Load(this, ref _FacilityPreBooking);
        set => _FacilityPreBooking = value;
    }

    public bool FacilityPreBooking_IsLoaded
    {
        get
        {
            return FacilityPreBooking != null;
        }
    }

    public virtual ReferenceEntry FacilityPreBookingReference 
    {
        get { return Context.Entry(this).Reference("FacilityPreBooking"); }
    }
    
    private TandTv3MixPoint _TandTv3MixPoint;
    public virtual TandTv3MixPoint TandTv3MixPoint
    { 
        get => LazyLoader.Load(this, ref _TandTv3MixPoint);
        set => _TandTv3MixPoint = value;
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