using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv3MixPointProdOrderPartslistPos : VBEntityObject
{

    public TandTv3MixPointProdOrderPartslistPos()
    {
    }

    private TandTv3MixPointProdOrderPartslistPos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _TandTv3MixPointProdOrderPartslistPosID;
    public Guid TandTv3MixPointProdOrderPartslistPosID 
    {
        get { return _TandTv3MixPointProdOrderPartslistPosID; }
        set { SetProperty<Guid>(ref _TandTv3MixPointProdOrderPartslistPosID, value); }
    }

    Guid _TandTv3MixPointID;
    public Guid TandTv3MixPointID 
    {
        get { return _TandTv3MixPointID; }
        set { SetForeignKeyProperty<Guid>(ref _TandTv3MixPointID, value, "TandTv3MixPoint", _TandTv3MixPoint, TandTv3MixPoint != null ? TandTv3MixPoint.TandTv3MixPointID : default(Guid)); }
    }

    Guid _ProdOrderPartslistPosID;
    public Guid ProdOrderPartslistPosID 
    {
        get { return _ProdOrderPartslistPosID; }
        set { SetForeignKeyProperty<Guid>(ref _ProdOrderPartslistPosID, value, "ProdOrderPartslistPos", _ProdOrderPartslistPos, ProdOrderPartslistPos != null ? ProdOrderPartslistPos.ProdOrderPartslistPosID : default(Guid)); }
    }

    private ProdOrderPartslistPos _ProdOrderPartslistPos;
    public virtual ProdOrderPartslistPos ProdOrderPartslistPos
    { 
        get { return LazyLoader.Load(this, ref _ProdOrderPartslistPos); } 
        set { SetProperty<ProdOrderPartslistPos>(ref _ProdOrderPartslistPos, value); }
    }

    public bool ProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return _ProdOrderPartslistPos != null;
        }
    }

    public virtual ReferenceEntry ProdOrderPartslistPosReference 
    {
        get { return Context.Entry(this).Reference("ProdOrderPartslistPos"); }
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
