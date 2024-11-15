﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MaterialWFRelation : VBEntityObject, ISequence
{

    public MaterialWFRelation()
    {
    }

    private MaterialWFRelation(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MaterialWFRelationID;
    public Guid MaterialWFRelationID 
    {
        get { return _MaterialWFRelationID; }
        set { SetProperty<Guid>(ref _MaterialWFRelationID, value); }
    }

    Guid _MaterialWFID;
    public Guid MaterialWFID 
    {
        get { return _MaterialWFID; }
        set { SetProperty<Guid>(ref _MaterialWFID, value); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    Guid _TargetMaterialID;
    public Guid TargetMaterialID 
    {
        get { return _TargetMaterialID; }
        set { SetProperty<Guid>(ref _TargetMaterialID, value); }
    }

    Guid _SourceMaterialID;
    public Guid SourceMaterialID 
    {
        get { return _SourceMaterialID; }
        set { SetProperty<Guid>(ref _SourceMaterialID, value); }
    }

    private MaterialWF _MaterialWF;
    public virtual MaterialWF MaterialWF
    { 
        get { return LazyLoader.Load(this, ref _MaterialWF); } 
        set { SetProperty<MaterialWF>(ref _MaterialWF, value); }
    }

    public bool MaterialWF_IsLoaded
    {
        get
        {
            return _MaterialWF != null;
        }
    }

    public virtual ReferenceEntry MaterialWFReference 
    {
        get { return Context.Entry(this).Reference("MaterialWF"); }
    }
    
    private ICollection<PartslistPosRelation> _PartslistPosRelation_MaterialWFRelation;
    public virtual ICollection<PartslistPosRelation> PartslistPosRelation_MaterialWFRelation
    {
        get { return LazyLoader.Load(this, ref _PartslistPosRelation_MaterialWFRelation); }
        set { _PartslistPosRelation_MaterialWFRelation = value; }
    }

    public bool PartslistPosRelation_MaterialWFRelation_IsLoaded
    {
        get
        {
            return _PartslistPosRelation_MaterialWFRelation != null;
        }
    }

    public virtual CollectionEntry PartslistPosRelation_MaterialWFRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.PartslistPosRelation_MaterialWFRelation); }
    }

    private Material _SourceMaterial;
    public virtual Material SourceMaterial
    { 
        get { return LazyLoader.Load(this, ref _SourceMaterial); } 
        set { SetProperty<Material>(ref _SourceMaterial, value); }
    }

    public bool SourceMaterial_IsLoaded
    {
        get
        {
            return _SourceMaterial != null;
        }
    }

    public virtual ReferenceEntry SourceMaterialReference 
    {
        get { return Context.Entry(this).Reference("SourceMaterial"); }
    }
    
    private Material _TargetMaterial;
    public virtual Material TargetMaterial
    { 
        get { return LazyLoader.Load(this, ref _TargetMaterial); } 
        set { SetProperty<Material>(ref _TargetMaterial, value); }
    }

    public bool TargetMaterial_IsLoaded
    {
        get
        {
            return _TargetMaterial != null;
        }
    }

    public virtual ReferenceEntry TargetMaterialReference 
    {
        get { return Context.Entry(this).Reference("TargetMaterial"); }
    }
    }
