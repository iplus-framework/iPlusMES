using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv3MixPointOutOrderPos : VBEntityObject
{

    public TandTv3MixPointOutOrderPos()
    {
    }

    private TandTv3MixPointOutOrderPos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _TandTv3MixPointOutOrderPosID;
    public Guid TandTv3MixPointOutOrderPosID 
    {
        get { return _TandTv3MixPointOutOrderPosID; }
        set { SetProperty<Guid>(ref _TandTv3MixPointOutOrderPosID, value); }
    }

    Guid _TandTv3MixPointID;
    public Guid TandTv3MixPointID 
    {
        get { return _TandTv3MixPointID; }
        set { SetForeignKeyProperty<Guid>(ref _TandTv3MixPointID, value, "TandTv3MixPoint", _TandTv3MixPoint, TandTv3MixPoint != null ? TandTv3MixPoint.TandTv3MixPointID : default(Guid)); }
    }

    Guid _OutOrderPosID;
    public Guid OutOrderPosID 
    {
        get { return _OutOrderPosID; }
        set { SetForeignKeyProperty<Guid>(ref _OutOrderPosID, value, "OutOrderPos", _OutOrderPos, OutOrderPos != null ? OutOrderPos.OutOrderPosID : default(Guid)); }
    }

    private OutOrderPos _OutOrderPos;
    public virtual OutOrderPos OutOrderPos
    { 
        get { return LazyLoader.Load(this, ref _OutOrderPos); } 
        set { SetProperty<OutOrderPos>(ref _OutOrderPos, value); }
    }

    public bool OutOrderPos_IsLoaded
    {
        get
        {
            return _OutOrderPos != null;
        }
    }

    public virtual ReferenceEntry OutOrderPosReference 
    {
        get { return Context.Entry(this).Reference("OutOrderPos"); }
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
