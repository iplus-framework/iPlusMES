using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MaterialGMPAdditive : VBEntityObject, IInsertInfo, IUpdateInfo, ISequence
{

    public MaterialGMPAdditive()
    {
    }

    private MaterialGMPAdditive(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MaterialGMPAdditiveID;
    public Guid MaterialGMPAdditiveID 
    {
        get { return _MaterialGMPAdditiveID; }
        set { SetProperty<Guid>(ref _MaterialGMPAdditiveID, value); }
    }

    Guid _MaterialID;
    public Guid MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid>(ref _MaterialID, value); }
    }

    Guid _MDGMPAdditiveID;
    public Guid MDGMPAdditiveID 
    {
        get { return _MDGMPAdditiveID; }
        set { SetProperty<Guid>(ref _MDGMPAdditiveID, value); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    double _Concentration;
    public double Concentration 
    {
        get { return _Concentration; }
        set { SetProperty<double>(ref _Concentration, value); }
    }

    string _InsertName;
    public string InsertName 
    {
        get { return _InsertName; }
        set { SetProperty<string>(ref _InsertName, value); }
    }

    DateTime _InsertDate;
    public DateTime InsertDate 
    {
        get { return _InsertDate; }
        set { SetProperty<DateTime>(ref _InsertDate, value); }
    }

    string _UpdateName;
    public string UpdateName 
    {
        get { return _UpdateName; }
        set { SetProperty<string>(ref _UpdateName, value); }
    }

    DateTime _UpdateDate;
    public DateTime UpdateDate 
    {
        get { return _UpdateDate; }
        set { SetProperty<DateTime>(ref _UpdateDate, value); }
    }

    private MDGMPAdditive _MDGMPAdditive;
    public virtual MDGMPAdditive MDGMPAdditive
    { 
        get { return LazyLoader.Load(this, ref _MDGMPAdditive); } 
        set { SetProperty<MDGMPAdditive>(ref _MDGMPAdditive, value); }
    }

    public bool MDGMPAdditive_IsLoaded
    {
        get
        {
            return MDGMPAdditive != null;
        }
    }

    public virtual ReferenceEntry MDGMPAdditiveReference 
    {
        get { return Context.Entry(this).Reference("MDGMPAdditive"); }
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
            return Material != null;
        }
    }

    public virtual ReferenceEntry MaterialReference 
    {
        get { return Context.Entry(this).Reference("Material"); }
    }
    }
