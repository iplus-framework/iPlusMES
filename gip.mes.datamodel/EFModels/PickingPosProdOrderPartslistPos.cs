using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class PickingPosProdOrderPartslistPos : VBEntityObject
{

    public PickingPosProdOrderPartslistPos()
    {
    }

    private PickingPosProdOrderPartslistPos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _PickingPosProdOrderPartslistPosID;
    public Guid PickingPosProdOrderPartslistPosID 
    {
        get { return _PickingPosProdOrderPartslistPosID; }
        set { SetProperty<Guid>(ref _PickingPosProdOrderPartslistPosID, value); }
    }

    Guid _PickingPosID;
    public Guid PickingPosID 
    {
        get { return _PickingPosID; }
        set { SetForeignKeyProperty<Guid>(ref _PickingPosID, value, "PickingPos", _PickingPos, PickingPos != null ? PickingPos.PickingPosID : default(Guid)); }
    }

    Guid _ProdorderPartslistPosID;
    public Guid ProdorderPartslistPosID 
    {
        get { return _ProdorderPartslistPosID; }
        set { SetForeignKeyProperty<Guid>(ref _ProdorderPartslistPosID, value, "ProdorderPartslistPos", _ProdorderPartslistPos, ProdorderPartslistPos != null ? ProdorderPartslistPos.ProdOrderPartslistPosID : default(Guid)); }
    }

    private PickingPos _PickingPos;
    public virtual PickingPos PickingPos
    { 
        get { return LazyLoader.Load(this, ref _PickingPos); } 
        set { SetProperty<PickingPos>(ref _PickingPos, value); }
    }

    public bool PickingPos_IsLoaded
    {
        get
        {
            return _PickingPos != null;
        }
    }

    public virtual ReferenceEntry PickingPosReference 
    {
        get { return Context.Entry(this).Reference("PickingPos"); }
    }
    
    private ProdOrderPartslistPos _ProdorderPartslistPos;
    public virtual ProdOrderPartslistPos ProdorderPartslistPos
    { 
        get { return LazyLoader.Load(this, ref _ProdorderPartslistPos); } 
        set { SetProperty<ProdOrderPartslistPos>(ref _ProdorderPartslistPos, value); }
    }

    public bool ProdorderPartslistPos_IsLoaded
    {
        get
        {
            return _ProdorderPartslistPos != null;
        }
    }

    public virtual ReferenceEntry ProdorderPartslistPosReference 
    {
        get { return Context.Entry(this).Reference("ProdorderPartslistPos"); }
    }
    }
