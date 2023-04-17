using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv2StepItemRelation : VBEntityObject 
{

    public TandTv2StepItemRelation()
    {
    }

    private TandTv2StepItemRelation(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _TandTv2StepItemRelationID;
    public Guid TandTv2StepItemRelationID 
    {
        get { return _TandTv2StepItemRelationID; }
        set { SetProperty<Guid>(ref _TandTv2StepItemRelationID, value); }
    }

    string _TandTv2RelationTypeID;
    public string TandTv2RelationTypeID 
    {
        get { return _TandTv2RelationTypeID; }
        set { SetProperty<string>(ref _TandTv2RelationTypeID, value); }
    }

    Guid _SourceTandTv2StepItemID;
    public Guid SourceTandTv2StepItemID 
    {
        get { return _SourceTandTv2StepItemID; }
        set { SetProperty<Guid>(ref _SourceTandTv2StepItemID, value); }
    }

    Guid _TargetTandTv2StepItemID;
    public Guid TargetTandTv2StepItemID 
    {
        get { return _TargetTandTv2StepItemID; }
        set { SetProperty<Guid>(ref _TargetTandTv2StepItemID, value); }
    }

    private TandTv2StepItem _SourceTandTv2StepItem;
    public virtual TandTv2StepItem SourceTandTv2StepItem
    { 
        get => LazyLoader.Load(this, ref _SourceTandTv2StepItem);
        set => _SourceTandTv2StepItem = value;
    }

    public bool SourceTandTv2StepItem_IsLoaded
    {
        get
        {
            return SourceTandTv2StepItem != null;
        }
    }

    public virtual ReferenceEntry SourceTandTv2StepItemReference 
    {
        get { return Context.Entry(this).Reference("SourceTandTv2StepItem"); }
    }
    
    private TandTv2RelationType _TandTv2RelationType;
    public virtual TandTv2RelationType TandTv2RelationType
    { 
        get => LazyLoader.Load(this, ref _TandTv2RelationType);
        set => _TandTv2RelationType = value;
    }

    public bool TandTv2RelationType_IsLoaded
    {
        get
        {
            return TandTv2RelationType != null;
        }
    }

    public virtual ReferenceEntry TandTv2RelationTypeReference 
    {
        get { return Context.Entry(this).Reference("TandTv2RelationType"); }
    }
    
    private TandTv2StepItem _TargetTandTv2StepItem;
    public virtual TandTv2StepItem TargetTandTv2StepItem
    { 
        get => LazyLoader.Load(this, ref _TargetTandTv2StepItem);
        set => _TargetTandTv2StepItem = value;
    }

    public bool TargetTandTv2StepItem_IsLoaded
    {
        get
        {
            return TargetTandTv2StepItem != null;
        }
    }

    public virtual ReferenceEntry TargetTandTv2StepItemReference 
    {
        get { return Context.Entry(this).Reference("TargetTandTv2StepItem"); }
    }
    }
