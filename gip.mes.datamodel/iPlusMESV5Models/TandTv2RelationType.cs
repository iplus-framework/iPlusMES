using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv2RelationType : VBEntityObject 
{

    public TandTv2RelationType()
    {
    }

    private TandTv2RelationType(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    string _TandTv2RelationTypeID;
    public string TandTv2RelationTypeID 
    {
        get { return _TandTv2RelationTypeID; }
        set { SetProperty<string>(ref _TandTv2RelationTypeID, value); }
    }

    private ICollection<TandTv2StepItemRelation> _TandTv2StepItemRelation_TandTv2RelationType;
    public virtual ICollection<TandTv2StepItemRelation> TandTv2StepItemRelation_TandTv2RelationType
    {
        get => LazyLoader.Load(this, ref _TandTv2StepItemRelation_TandTv2RelationType);
        set => _TandTv2StepItemRelation_TandTv2RelationType = value;
    }

    public bool TandTv2StepItemRelation_TandTv2RelationType_IsLoaded
    {
        get
        {
            return TandTv2StepItemRelation_TandTv2RelationType != null;
        }
    }

    public virtual CollectionEntry TandTv2StepItemRelation_TandTv2RelationTypeReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv2StepItemRelation_TandTv2RelationType); }
    }
}
