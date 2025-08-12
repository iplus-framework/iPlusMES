using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv3MixPointInOrderPos : VBEntityObject
{

    public TandTv3MixPointInOrderPos()
    {
    }

    private TandTv3MixPointInOrderPos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _TandTv3MixPointInOrderPosID;
    public Guid TandTv3MixPointInOrderPosID 
    {
        get { return _TandTv3MixPointInOrderPosID; }
        set { SetProperty<Guid>(ref _TandTv3MixPointInOrderPosID, value); }
    }

    Guid _TandTv3MixPointID;
    public Guid TandTv3MixPointID 
    {
        get { return _TandTv3MixPointID; }
        set { SetForeignKeyProperty<Guid>(ref _TandTv3MixPointID, value, "TandTv3MixPoint", _TandTv3MixPoint, TandTv3MixPoint != null ? TandTv3MixPoint.TandTv3MixPointID : default(Guid)); }
    }

    Guid _InOrderPosID;
    public Guid InOrderPosID 
    {
        get { return _InOrderPosID; }
        set { SetForeignKeyProperty<Guid>(ref _InOrderPosID, value, "InOrderPos", _InOrderPos, InOrderPos != null ? InOrderPos.InOrderPosID : default(Guid)); }
    }

    private InOrderPos _InOrderPos;
    public virtual InOrderPos InOrderPos
    { 
        get { return LazyLoader.Load(this, ref _InOrderPos); } 
        set { SetProperty<InOrderPos>(ref _InOrderPos, value); }
    }

    public bool InOrderPos_IsLoaded
    {
        get
        {
            return _InOrderPos != null;
        }
    }

    public virtual ReferenceEntry InOrderPosReference 
    {
        get { return Context.Entry(this).Reference("InOrderPos"); }
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
