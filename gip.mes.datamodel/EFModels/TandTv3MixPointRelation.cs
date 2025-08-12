using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv3MixPointRelation : VBEntityObject
{

    public TandTv3MixPointRelation()
    {
    }

    private TandTv3MixPointRelation(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _TandTv3MixPointRelationID;
    public Guid TandTv3MixPointRelationID 
    {
        get { return _TandTv3MixPointRelationID; }
        set { SetProperty<Guid>(ref _TandTv3MixPointRelationID, value); }
    }

    Guid _SourceTandTv3MixPointID;
    public Guid SourceTandTv3MixPointID 
    {
        get { return _SourceTandTv3MixPointID; }
        set { SetForeignKeyProperty<Guid>(ref _SourceTandTv3MixPointID, value, "SourceTandTv3MixPoint", _SourceTandTv3MixPoint, SourceTandTv3MixPoint != null ? SourceTandTv3MixPoint.TandTv3MixPointID : default(Guid)); }
    }

    Guid _TargetTandTv3MixPointID;
    public Guid TargetTandTv3MixPointID 
    {
        get { return _TargetTandTv3MixPointID; }
        set { SetForeignKeyProperty<Guid>(ref _TargetTandTv3MixPointID, value, "TargetTandTv3MixPoint", _TargetTandTv3MixPoint, TargetTandTv3MixPoint != null ? TargetTandTv3MixPoint.TandTv3MixPointID : default(Guid)); }
    }

    private TandTv3MixPoint _SourceTandTv3MixPoint;
    public virtual TandTv3MixPoint SourceTandTv3MixPoint
    { 
        get { return LazyLoader.Load(this, ref _SourceTandTv3MixPoint); } 
        set { SetProperty<TandTv3MixPoint>(ref _SourceTandTv3MixPoint, value); }
    }

    public bool SourceTandTv3MixPoint_IsLoaded
    {
        get
        {
            return _SourceTandTv3MixPoint != null;
        }
    }

    public virtual ReferenceEntry SourceTandTv3MixPointReference 
    {
        get { return Context.Entry(this).Reference("SourceTandTv3MixPoint"); }
    }
    
    private TandTv3MixPoint _TargetTandTv3MixPoint;
    public virtual TandTv3MixPoint TargetTandTv3MixPoint
    { 
        get { return LazyLoader.Load(this, ref _TargetTandTv3MixPoint); } 
        set { SetProperty<TandTv3MixPoint>(ref _TargetTandTv3MixPoint, value); }
    }

    public bool TargetTandTv3MixPoint_IsLoaded
    {
        get
        {
            return _TargetTandTv3MixPoint != null;
        }
    }

    public virtual ReferenceEntry TargetTandTv3MixPointReference 
    {
        get { return Context.Entry(this).Reference("TargetTandTv3MixPoint"); }
    }
    }
