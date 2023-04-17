using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv3MixPointProdOrderPartslistPosRelation : VBEntityObject 
{

    public TandTv3MixPointProdOrderPartslistPosRelation()
    {
    }

    private TandTv3MixPointProdOrderPartslistPosRelation(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _TandTv3MixPointProdOrderPartslistPosRelationID;
    public Guid TandTv3MixPointProdOrderPartslistPosRelationID 
    {
        get { return _TandTv3MixPointProdOrderPartslistPosRelationID; }
        set { SetProperty<Guid>(ref _TandTv3MixPointProdOrderPartslistPosRelationID, value); }
    }

    Guid _TandTv3MixPointID;
    public Guid TandTv3MixPointID 
    {
        get { return _TandTv3MixPointID; }
        set { SetProperty<Guid>(ref _TandTv3MixPointID, value); }
    }

    Guid _ProdOrderPartslistPosRelationID;
    public Guid ProdOrderPartslistPosRelationID 
    {
        get { return _ProdOrderPartslistPosRelationID; }
        set { SetProperty<Guid>(ref _ProdOrderPartslistPosRelationID, value); }
    }

    private ProdOrderPartslistPosRelation _ProdOrderPartslistPosRelation;
    public virtual ProdOrderPartslistPosRelation ProdOrderPartslistPosRelation
    { 
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPosRelation);
        set => _ProdOrderPartslistPosRelation = value;
    }

    public bool ProdOrderPartslistPosRelation_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPosRelation != null;
        }
    }

    public virtual ReferenceEntry ProdOrderPartslistPosRelationReference 
    {
        get { return Context.Entry(this).Reference("ProdOrderPartslistPosRelation"); }
    }
    
    private TandTv3MixPoint _TandTv3MixPoint;
    public virtual TandTv3MixPoint TandTv3MixPoint
    { 
        get => LazyLoader.Load(this, ref _TandTv3MixPoint);
        set => _TandTv3MixPoint = value;
    }

    public bool TandTv3MixPoint_IsLoaded
    {
        get
        {
            return TandTv3MixPoint != null;
        }
    }

    public virtual ReferenceEntry TandTv3MixPointReference 
    {
        get { return Context.Entry(this).Reference("TandTv3MixPoint"); }
    }
    }
