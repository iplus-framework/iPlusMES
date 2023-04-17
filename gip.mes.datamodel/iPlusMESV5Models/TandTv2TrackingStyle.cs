using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv2TrackingStyle : VBEntityObject 
{

    public TandTv2TrackingStyle()
    {
    }

    private TandTv2TrackingStyle(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    string _TandTv2TrackingStyleID;
    public string TandTv2TrackingStyleID 
    {
        get { return _TandTv2TrackingStyleID; }
        set { SetProperty<string>(ref _TandTv2TrackingStyleID, value); }
    }

    private ICollection<TandTv2Job> _TandTv2Job_TandTv2TrackingStyle;
    public virtual ICollection<TandTv2Job> TandTv2Job_TandTv2TrackingStyle
    {
        get => LazyLoader.Load(this, ref _TandTv2Job_TandTv2TrackingStyle);
        set => _TandTv2Job_TandTv2TrackingStyle = value;
    }

    public bool TandTv2Job_TandTv2TrackingStyle_IsLoaded
    {
        get
        {
            return TandTv2Job_TandTv2TrackingStyle != null;
        }
    }

    public virtual CollectionEntry TandTv2Job_TandTv2TrackingStyleReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv2Job_TandTv2TrackingStyle); }
    }
}
