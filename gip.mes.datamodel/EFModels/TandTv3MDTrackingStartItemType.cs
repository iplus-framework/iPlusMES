using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv3MDTrackingStartItemType : VBEntityObject
{

    public TandTv3MDTrackingStartItemType()
    {
    }

    private TandTv3MDTrackingStartItemType(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    string _TandTv3MDTrackingStartItemTypeID;
    public string TandTv3MDTrackingStartItemTypeID 
    {
        get { return _TandTv3MDTrackingStartItemTypeID; }
        set { SetProperty<string>(ref _TandTv3MDTrackingStartItemTypeID, value); }
    }

    string _ACCaptionTranslation;
    public string ACCaptionTranslation 
    {
        get { return _ACCaptionTranslation; }
        set { SetProperty<string>(ref _ACCaptionTranslation, value); }
    }

    private ICollection<TandTv3FilterTracking> _TandTv3FilterTracking_TandTv3MDTrackingStartItemType;
    public virtual ICollection<TandTv3FilterTracking> TandTv3FilterTracking_TandTv3MDTrackingStartItemType
    {
        get { return LazyLoader.Load(this, ref _TandTv3FilterTracking_TandTv3MDTrackingStartItemType); }
        set { SetProperty<ICollection<TandTv3FilterTracking>>(ref _TandTv3FilterTracking_TandTv3MDTrackingStartItemType, value); }
    }

    public bool TandTv3FilterTracking_TandTv3MDTrackingStartItemType_IsLoaded
    {
        get
        {
            return _TandTv3FilterTracking_TandTv3MDTrackingStartItemType != null;
        }
    }

    public virtual CollectionEntry TandTv3FilterTracking_TandTv3MDTrackingStartItemTypeReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3FilterTracking_TandTv3MDTrackingStartItemType); }
    }
}
