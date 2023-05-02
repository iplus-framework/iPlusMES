using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv3MDBookingDirection : VBEntityObject
{

    public TandTv3MDBookingDirection()
    {
    }

    private TandTv3MDBookingDirection(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    string _TandTv3MDBookingDirectionID;
    public string TandTv3MDBookingDirectionID 
    {
        get { return _TandTv3MDBookingDirectionID; }
        set { SetProperty<string>(ref _TandTv3MDBookingDirectionID, value); }
    }

    private ICollection<TandTv3MixPointFacility> _TandTv3MixPointFacility_TandTv3MDBookingDirection;
    public virtual ICollection<TandTv3MixPointFacility> TandTv3MixPointFacility_TandTv3MDBookingDirection
    {
        get => LazyLoader.Load(this, ref _TandTv3MixPointFacility_TandTv3MDBookingDirection);
        set => _TandTv3MixPointFacility_TandTv3MDBookingDirection = value;
    }

    public bool TandTv3MixPointFacility_TandTv3MDBookingDirection_IsLoaded
    {
        get
        {
            return TandTv3MixPointFacility_TandTv3MDBookingDirection != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointFacility_TandTv3MDBookingDirectionReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointFacility_TandTv3MDBookingDirection); }
    }

    private ICollection<TandTv3MixPointFacilityLot> _TandTv3MixPointFacilityLot_TandTv3MDBookingDirection;
    public virtual ICollection<TandTv3MixPointFacilityLot> TandTv3MixPointFacilityLot_TandTv3MDBookingDirection
    {
        get => LazyLoader.Load(this, ref _TandTv3MixPointFacilityLot_TandTv3MDBookingDirection);
        set => _TandTv3MixPointFacilityLot_TandTv3MDBookingDirection = value;
    }

    public bool TandTv3MixPointFacilityLot_TandTv3MDBookingDirection_IsLoaded
    {
        get
        {
            return TandTv3MixPointFacilityLot_TandTv3MDBookingDirection != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointFacilityLot_TandTv3MDBookingDirectionReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointFacilityLot_TandTv3MDBookingDirection); }
    }
}
