using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv2TempPos : VBEntityObject 
{

    public TandTv2TempPos()
    {
    }

    private TandTv2TempPos(ILazyLoader lazyLoader)
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

    Guid? _ProdOrderPartslistPosID;
    public Guid? ProdOrderPartslistPosID 
    {
        get { return _ProdOrderPartslistPosID; }
        set { SetProperty<Guid?>(ref _ProdOrderPartslistPosID, value); }
    }

    int? _MaterialPosTypeIndex;
    public int? MaterialPosTypeIndex 
    {
        get { return _MaterialPosTypeIndex; }
        set { SetProperty<int?>(ref _MaterialPosTypeIndex, value); }
    }

    Guid? _SourceProdOrderPartslistPosRelationID;
    public Guid? SourceProdOrderPartslistPosRelationID 
    {
        get { return _SourceProdOrderPartslistPosRelationID; }
        set { SetProperty<Guid?>(ref _SourceProdOrderPartslistPosRelationID, value); }
    }

    Guid? _SourceProdOrderPartslistPosID;
    public Guid? SourceProdOrderPartslistPosID 
    {
        get { return _SourceProdOrderPartslistPosID; }
        set { SetProperty<Guid?>(ref _SourceProdOrderPartslistPosID, value); }
    }

    private TandTv2Step _TandTv2Step;
    public virtual TandTv2Step TandTv2Step
    { 
        get => LazyLoader.Load(this, ref _TandTv2Step);
        set => _TandTv2Step = value;
    }

    public bool TandTv2Step_IsLoaded
    {
        get
        {
            return TandTv2Step != null;
        }
    }

    public virtual ReferenceEntry TandTv2StepReference 
    {
        get { return Context.Entry(this).Reference("TandTv2Step"); }
    }
    }
