using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv3Step : VBEntityObject
{

    public TandTv3Step()
    {
    }

    private TandTv3Step(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _TandTv3StepID;
    public Guid TandTv3StepID 
    {
        get { return _TandTv3StepID; }
        set { SetProperty<Guid>(ref _TandTv3StepID, value); }
    }

    Guid _TandTv3FilterTrackingID;
    public Guid TandTv3FilterTrackingID 
    {
        get { return _TandTv3FilterTrackingID; }
        set { SetForeignKeyProperty<Guid>(ref _TandTv3FilterTrackingID, value, "TandTv3FilterTracking", _TandTv3FilterTracking, TandTv3FilterTracking != null ? TandTv3FilterTracking.TandTv3FilterTrackingID : default(Guid)); }
    }

    int _StepNo;
    public int StepNo 
    {
        get { return _StepNo; }
        set { SetProperty<int>(ref _StepNo, value); }
    }

    string _StepName;
    public string StepName 
    {
        get { return _StepName; }
        set { SetProperty<string>(ref _StepName, value); }
    }

    private TandTv3FilterTracking _TandTv3FilterTracking;
    public virtual TandTv3FilterTracking TandTv3FilterTracking
    { 
        get { return LazyLoader.Load(this, ref _TandTv3FilterTracking); } 
        set { SetProperty<TandTv3FilterTracking>(ref _TandTv3FilterTracking, value); }
    }

    public bool TandTv3FilterTracking_IsLoaded
    {
        get
        {
            return _TandTv3FilterTracking != null;
        }
    }

    public virtual ReferenceEntry TandTv3FilterTrackingReference 
    {
        get { return Context.Entry(this).Reference("TandTv3FilterTracking"); }
    }
    
    private ICollection<TandTv3MixPoint> _TandTv3MixPoint_TandTv3Step;
    public virtual ICollection<TandTv3MixPoint> TandTv3MixPoint_TandTv3Step
    {
        get { return LazyLoader.Load(this, ref _TandTv3MixPoint_TandTv3Step); }
        set { SetProperty<ICollection<TandTv3MixPoint>>(ref _TandTv3MixPoint_TandTv3Step, value); }
    }

    public bool TandTv3MixPoint_TandTv3Step_IsLoaded
    {
        get
        {
            return _TandTv3MixPoint_TandTv3Step != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPoint_TandTv3StepReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPoint_TandTv3Step); }
    }
}
