using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv2JobMaterial : VBEntityObject 
{

    public TandTv2JobMaterial()
    {
    }

    private TandTv2JobMaterial(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _TandTv2JobMaterialID;
    public Guid TandTv2JobMaterialID 
    {
        get { return _TandTv2JobMaterialID; }
        set { SetProperty<Guid>(ref _TandTv2JobMaterialID, value); }
    }

    Guid _TandTv2JobID;
    public Guid TandTv2JobID 
    {
        get { return _TandTv2JobID; }
        set { SetProperty<Guid>(ref _TandTv2JobID, value); }
    }

    Guid _MaterialID;
    public Guid MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid>(ref _MaterialID, value); }
    }

    private Material _Material;
    public virtual Material Material
    { 
        get => LazyLoader.Load(this, ref _Material);
        set => _Material = value;
    }

    public bool Material_IsLoaded
    {
        get
        {
            return Material != null;
        }
    }

    public virtual ReferenceEntry MaterialReference 
    {
        get { return Context.Entry(this).Reference("Material"); }
    }
    
    private TandTv2Job _TandTv2Job;
    public virtual TandTv2Job TandTv2Job
    { 
        get => LazyLoader.Load(this, ref _TandTv2Job);
        set => _TandTv2Job = value;
    }

    public bool TandTv2Job_IsLoaded
    {
        get
        {
            return TandTv2Job != null;
        }
    }

    public virtual ReferenceEntry TandTv2JobReference 
    {
        get { return Context.Entry(this).Reference("TandTv2Job"); }
    }
    }
