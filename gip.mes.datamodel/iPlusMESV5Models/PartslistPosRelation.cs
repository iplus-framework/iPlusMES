using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class PartslistPosRelation : VBEntityObject 
{

    public PartslistPosRelation()
    {
    }

    private PartslistPosRelation(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _PartslistPosRelationID;
    public Guid PartslistPosRelationID 
    {
        get { return _PartslistPosRelationID; }
        set { SetProperty<Guid>(ref _PartslistPosRelationID, value); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    Guid _TargetPartslistPosID;
    public Guid TargetPartslistPosID 
    {
        get { return _TargetPartslistPosID; }
        set { SetProperty<Guid>(ref _TargetPartslistPosID, value); }
    }

    Guid _SourcePartslistPosID;
    public Guid SourcePartslistPosID 
    {
        get { return _SourcePartslistPosID; }
        set { SetProperty<Guid>(ref _SourcePartslistPosID, value); }
    }

    double _TargetQuantity;
    public double TargetQuantity 
    {
        get { return _TargetQuantity; }
        set { SetProperty<double>(ref _TargetQuantity, value); }
    }

    double _TargetQuantityUOM;
    public double TargetQuantityUOM 
    {
        get { return _TargetQuantityUOM; }
        set { SetProperty<double>(ref _TargetQuantityUOM, value); }
    }

    Guid? _MaterialWFRelationID;
    public Guid? MaterialWFRelationID 
    {
        get { return _MaterialWFRelationID; }
        set { SetProperty<Guid?>(ref _MaterialWFRelationID, value); }
    }

    bool? _RetrogradeFIFO;
    public bool? RetrogradeFIFO 
    {
        get { return _RetrogradeFIFO; }
        set { SetProperty<bool?>(ref _RetrogradeFIFO, value); }
    }

    bool? _Anterograde;
    public bool? Anterograde 
    {
        get { return _Anterograde; }
        set { SetProperty<bool?>(ref _Anterograde, value); }
    }

    private MaterialWFRelation _MaterialWFRelation;
    public virtual MaterialWFRelation MaterialWFRelation
    { 
        get => LazyLoader.Load(this, ref _MaterialWFRelation);
        set => _MaterialWFRelation = value;
    }

    public bool MaterialWFRelation_IsLoaded
    {
        get
        {
            return MaterialWFRelation != null;
        }
    }

    public virtual ReferenceEntry MaterialWFRelationReference 
    {
        get { return Context.Entry(this).Reference("MaterialWFRelation"); }
    }
    
    private PartslistPos _SourcePartslistPos;
    public virtual PartslistPos SourcePartslistPos
    { 
        get => LazyLoader.Load(this, ref _SourcePartslistPos);
        set => _SourcePartslistPos = value;
    }

    public bool SourcePartslistPos_IsLoaded
    {
        get
        {
            return SourcePartslistPos != null;
        }
    }

    public virtual ReferenceEntry SourcePartslistPosReference 
    {
        get { return Context.Entry(this).Reference("SourcePartslistPos"); }
    }
    
    private PartslistPos _TargetPartslistPos;
    public virtual PartslistPos TargetPartslistPos
    { 
        get => LazyLoader.Load(this, ref _TargetPartslistPos);
        set => _TargetPartslistPos = value;
    }

    public bool TargetPartslistPos_IsLoaded
    {
        get
        {
            return TargetPartslistPos != null;
        }
    }

    public virtual ReferenceEntry TargetPartslistPosReference 
    {
        get { return Context.Entry(this).Reference("TargetPartslistPos"); }
    }
    }
