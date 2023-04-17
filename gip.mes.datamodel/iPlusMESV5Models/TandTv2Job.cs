using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv2Job : VBEntityObject 
{

    public TandTv2Job()
    {
    }

    private TandTv2Job(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _TandTv2JobID;
    public Guid TandTv2JobID 
    {
        get { return _TandTv2JobID; }
        set { SetProperty<Guid>(ref _TandTv2JobID, value); }
    }

    string _TandTv2TrackingStyleID;
    public string TandTv2TrackingStyleID 
    {
        get { return _TandTv2TrackingStyleID; }
        set { SetProperty<string>(ref _TandTv2TrackingStyleID, value); }
    }

    string _TandTv2ItemTypeID;
    public string TandTv2ItemTypeID 
    {
        get { return _TandTv2ItemTypeID; }
        set { SetProperty<string>(ref _TandTv2ItemTypeID, value); }
    }

    string _JobNo;
    public string JobNo 
    {
        get { return _JobNo; }
        set { SetProperty<string>(ref _JobNo, value); }
    }

    DateTime? _FilterDateFrom;
    public DateTime? FilterDateFrom 
    {
        get { return _FilterDateFrom; }
        set { SetProperty<DateTime?>(ref _FilterDateFrom, value); }
    }

    DateTime? _FilterDateTo;
    public DateTime? FilterDateTo 
    {
        get { return _FilterDateTo; }
        set { SetProperty<DateTime?>(ref _FilterDateTo, value); }
    }

    DateTime _StartTime;
    public DateTime StartTime 
    {
        get { return _StartTime; }
        set { SetProperty<DateTime>(ref _StartTime, value); }
    }

    DateTime? _EndTime;
    public DateTime? EndTime 
    {
        get { return _EndTime; }
        set { SetProperty<DateTime?>(ref _EndTime, value); }
    }

    string _ItemSystemNo;
    public string ItemSystemNo 
    {
        get { return _ItemSystemNo; }
        set { SetProperty<string>(ref _ItemSystemNo, value); }
    }

    Guid _PrimaryKeyID;
    public Guid PrimaryKeyID 
    {
        get { return _PrimaryKeyID; }
        set { SetProperty<Guid>(ref _PrimaryKeyID, value); }
    }

    string _InsertName;
    public string InsertName 
    {
        get { return _InsertName; }
        set { SetProperty<string>(ref _InsertName, value); }
    }

    private TandTv2ItemType _TandTv2ItemType;
    public virtual TandTv2ItemType TandTv2ItemType
    { 
        get => LazyLoader.Load(this, ref _TandTv2ItemType);
        set => _TandTv2ItemType = value;
    }

    public bool TandTv2ItemType_IsLoaded
    {
        get
        {
            return TandTv2ItemType != null;
        }
    }

    public virtual ReferenceEntry TandTv2ItemTypeReference 
    {
        get { return Context.Entry(this).Reference("TandTv2ItemType"); }
    }
    
    private ICollection<TandTv2JobMaterial> _TandTv2JobMaterial_TandTv2Job;
    public virtual ICollection<TandTv2JobMaterial> TandTv2JobMaterial_TandTv2Job
    {
        get => LazyLoader.Load(this, ref _TandTv2JobMaterial_TandTv2Job);
        set => _TandTv2JobMaterial_TandTv2Job = value;
    }

    public bool TandTv2JobMaterial_TandTv2Job_IsLoaded
    {
        get
        {
            return TandTv2JobMaterial_TandTv2Job != null;
        }
    }

    public virtual CollectionEntry TandTv2JobMaterial_TandTv2JobReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv2JobMaterial_TandTv2Job); }
    }

    private ICollection<TandTv2Step> _TandTv2Step_TandTv2Job;
    public virtual ICollection<TandTv2Step> TandTv2Step_TandTv2Job
    {
        get => LazyLoader.Load(this, ref _TandTv2Step_TandTv2Job);
        set => _TandTv2Step_TandTv2Job = value;
    }

    public bool TandTv2Step_TandTv2Job_IsLoaded
    {
        get
        {
            return TandTv2Step_TandTv2Job != null;
        }
    }

    public virtual CollectionEntry TandTv2Step_TandTv2JobReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv2Step_TandTv2Job); }
    }

    private TandTv2TrackingStyle _TandTv2TrackingStyle;
    public virtual TandTv2TrackingStyle TandTv2TrackingStyle
    { 
        get => LazyLoader.Load(this, ref _TandTv2TrackingStyle);
        set => _TandTv2TrackingStyle = value;
    }

    public bool TandTv2TrackingStyle_IsLoaded
    {
        get
        {
            return TandTv2TrackingStyle != null;
        }
    }

    public virtual ReferenceEntry TandTv2TrackingStyleReference 
    {
        get { return Context.Entry(this).Reference("TandTv2TrackingStyle"); }
    }
    }
