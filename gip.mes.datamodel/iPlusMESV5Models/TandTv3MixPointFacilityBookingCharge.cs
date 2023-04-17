using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv3MixPointFacilityBookingCharge : VBEntityObject 
{

    public TandTv3MixPointFacilityBookingCharge()
    {
    }

    private TandTv3MixPointFacilityBookingCharge(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _TandTv3MixPointFacilityBookingChargeID;
    public Guid TandTv3MixPointFacilityBookingChargeID 
    {
        get { return _TandTv3MixPointFacilityBookingChargeID; }
        set { SetProperty<Guid>(ref _TandTv3MixPointFacilityBookingChargeID, value); }
    }

    Guid _TandTv3MixPointID;
    public Guid TandTv3MixPointID 
    {
        get { return _TandTv3MixPointID; }
        set { SetProperty<Guid>(ref _TandTv3MixPointID, value); }
    }

    Guid _FacilityBookingChargeID;
    public Guid FacilityBookingChargeID 
    {
        get { return _FacilityBookingChargeID; }
        set { SetProperty<Guid>(ref _FacilityBookingChargeID, value); }
    }

    private FacilityBookingCharge _FacilityBookingCharge;
    public virtual FacilityBookingCharge FacilityBookingCharge
    { 
        get => LazyLoader.Load(this, ref _FacilityBookingCharge);
        set => _FacilityBookingCharge = value;
    }

    public bool FacilityBookingCharge_IsLoaded
    {
        get
        {
            return FacilityBookingCharge != null;
        }
    }

    public virtual ReferenceEntry FacilityBookingChargeReference 
    {
        get { return Context.Entry(this).Reference("FacilityBookingCharge"); }
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
