using System;
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
        set { SetForeignKeyProperty<Guid>(ref _TandTv3MixPointID, value, "TandTv3MixPoint", _TandTv3MixPoint, TandTv3MixPoint != null ? TandTv3MixPoint.TandTv3MixPointID : default(Guid)); }
    }

    string _TandTv3MDBookingDirectionID;
    public string TandTv3MDBookingDirectionID 
    {
        get { return _TandTv3MDBookingDirectionID; }
        set { SetForeignKeyProperty<string>(ref _TandTv3MDBookingDirectionID, value, "TandTv3MDBookingDirection", _TandTv3MDBookingDirection, TandTv3MDBookingDirection != null ? TandTv3MDBookingDirection.TandTv3MDBookingDirectionID : default(string)); }
    }

    Guid _FacilityID;
    public Guid FacilityID 
    {
        get { return _FacilityID; }
        set { SetForeignKeyProperty<Guid>(ref _FacilityID, value, "Facility", _Facility, Facility != null ? Facility.FacilityID : default(Guid)); }
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
            return _Facility != null;
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
            return _TandTv3MDBookingDirection != null;
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
            return _TandTv3MixPoint != null;
        }
    }

    public virtual ReferenceEntry TandTv3MixPointReference 
    {
        get { return Context.Entry(this).Reference("TandTv3MixPoint"); }
    }
    }
