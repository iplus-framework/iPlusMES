using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv2Step : VBEntityObject 
{

    public TandTv2Step()
    {
    }

    private TandTv2Step(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _TandTv2StepID;
    public Guid TandTv2StepID 
    {
        get { return _TandTv2StepID; }
        set { SetProperty<Guid>(ref _TandTv2StepID, value); }
    }

    Guid _TandTv2JobID;
    public Guid TandTv2JobID 
    {
        get { return _TandTv2JobID; }
        set { SetProperty<Guid>(ref _TandTv2JobID, value); }
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

    private TandTv2Job _TandTv2Job;
    public virtual TandTv2Job TandTv2Job
    { 
        get => LazyLoader.Load(this, ref _TandTv2Job);
        set => _TandTv2Job = value;
    }

    public bool TandTv2Job_IsLoaded
    {
        get
        {
            return TandTv2Job != null;
        }
    }

    public virtual ReferenceEntry TandTv2JobReference 
    {
        get { return Context.Entry(this).Reference("TandTv2Job"); }
    }
    
    private ICollection<TandTv2StepItem> _TandTv2StepItem_TandTv2Step;
    public virtual ICollection<TandTv2StepItem> TandTv2StepItem_TandTv2Step
    {
        get => LazyLoader.Load(this, ref _TandTv2StepItem_TandTv2Step);
        set => _TandTv2StepItem_TandTv2Step = value;
    }

    public bool TandTv2StepItem_TandTv2Step_IsLoaded
    {
        get
        {
            return TandTv2StepItem_TandTv2Step != null;
        }
    }

    public virtual CollectionEntry TandTv2StepItem_TandTv2StepReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv2StepItem_TandTv2Step); }
    }

    private ICollection<TandTv2TempPos> _TandTv2TempPos_TandTv2Step;
    public virtual ICollection<TandTv2TempPos> TandTv2TempPos_TandTv2Step
    {
        get => LazyLoader.Load(this, ref _TandTv2TempPos_TandTv2Step);
        set => _TandTv2TempPos_TandTv2Step = value;
    }

    public bool TandTv2TempPos_TandTv2Step_IsLoaded
    {
        get
        {
            return TandTv2StepLot_TandTv2Step != null;
        }
    }

    public virtual CollectionEntry TandTv2TempPos_TandTv2StepReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv2TempPos_TandTv2Step); }
    }

    private ICollection<TandTv2StepLot> _TandTv2StepLot_TandTv2Step;
    public virtual ICollection<TandTv2StepLot> TandTv2StepLot_TandTv2Step
    {
        get => LazyLoader.Load(this, ref _TandTv2StepLot_TandTv2Step);
        set => _TandTv2StepLot_TandTv2Step = value;
    }

    public bool TandTv2StepLot_TandTv2Step_IsLoaded
    {
        get
        {
            return TandTv2StepLot_TandTv2Step != null;
        }
    }

    public virtual CollectionEntry TandTv2StepLot_TandTv2StepReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv2StepLot_TandTv2Step); }
    }
}
