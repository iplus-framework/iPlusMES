using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv3MDTrackingDirection : VBEntityObject
{

    public TandTv3MDTrackingDirection()
    {
    }

    private TandTv3MDTrackingDirection(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    string _TandTv3MDTrackingDirectionID;
    public string TandTv3MDTrackingDirectionID 
    {
        get { return _TandTv3MDTrackingDirectionID; }
        set { SetProperty<string>(ref _TandTv3MDTrackingDirectionID, value); }
    }

    private ICollection<TandTv3FilterTracking> _TandTv3FilterTracking_TandTv3MDTrackingDirection;
    public virtual ICollection<TandTv3FilterTracking> TandTv3FilterTracking_TandTv3MDTrackingDirection
    {
        get => LazyLoader.Load(this, ref _TandTv3FilterTracking_TandTv3MDTrackingDirection);
        set => _TandTv3FilterTracking_TandTv3MDTrackingDirection = value;
    }

    public bool TandTv3FilterTracking_TandTv3MDTrackingDirection_IsLoaded
    {
        get
        {
            return TandTv3FilterTracking_TandTv3MDTrackingDirection != null;
        }
    }

    public virtual CollectionEntry TandTv3FilterTracking_TandTv3MDTrackingDirectionReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3FilterTracking_TandTv3MDTrackingDirection); }
    }
}
