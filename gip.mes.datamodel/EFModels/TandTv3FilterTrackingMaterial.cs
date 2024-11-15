using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv3FilterTrackingMaterial : VBEntityObject
{

    public TandTv3FilterTrackingMaterial()
    {
    }

    private TandTv3FilterTrackingMaterial(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _TandTv3FilterTrackingMaterialID;
    public Guid TandTv3FilterTrackingMaterialID 
    {
        get { return _TandTv3FilterTrackingMaterialID; }
        set { SetProperty<Guid>(ref _TandTv3FilterTrackingMaterialID, value); }
    }

    Guid _TandTv3FilterTrackingID;
    public Guid TandTv3FilterTrackingID 
    {
        get { return _TandTv3FilterTrackingID; }
        set { SetProperty<Guid>(ref _TandTv3FilterTrackingID, value); }
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
        get { return LazyLoader.Load(this, ref _Material); } 
        set { SetProperty<Material>(ref _Material, value); }
    }

    public bool Material_IsLoaded
    {
        get
        {
            return _Material != null;
        }
    }

    public virtual ReferenceEntry MaterialReference 
    {
        get { return Context.Entry(this).Reference("Material"); }
    }
    
    private TandTv3FilterTracking _TandTv3FilterTracking;
    public virtual TandTv3FilterTracking TandTv3FilterTracking
    { 
        get { return LazyLoader.Load(this, ref _TandTv3FilterTracking); } 
        set { SetProperty<TandTv3FilterTracking>(ref _TandTv3FilterTracking, value); }
    }

    public bool TandTv3FilterTracking_IsLoaded
    {
        get
        {
            return _TandTv3FilterTracking != null;
        }
    }

    public virtual ReferenceEntry TandTv3FilterTrackingReference 
    {
        get { return Context.Entry(this).Reference("TandTv3FilterTracking"); }
    }
    }
