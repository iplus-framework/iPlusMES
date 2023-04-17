using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MaterialWF : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public MaterialWF()
    {
    }

    private MaterialWF(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _MaterialWFID;
    public Guid MaterialWFID 
    {
        get { return _MaterialWFID; }
        set { SetProperty<Guid>(ref _MaterialWFID, value); }
    }

    string _MaterialWFNo;
    public string MaterialWFNo 
    {
        get { return _MaterialWFNo; }
        set { SetProperty<string>(ref _MaterialWFNo, value); }
    }

    string _Name;
    public string Name 
    {
        get { return _Name; }
        set { SetProperty<string>(ref _Name, value); }
    }

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
    }

    string _XMLDesign;
    public string XMLDesign 
    {
        get { return _XMLDesign; }
        set { SetProperty<string>(ref _XMLDesign, value); }
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

    private ICollection<MaterialWFACClassMethod> _MaterialWFACClassMethod_MaterialWF;
    public virtual ICollection<MaterialWFACClassMethod> MaterialWFACClassMethod_MaterialWF
    {
        get => LazyLoader.Load(this, ref _MaterialWFACClassMethod_MaterialWF);
        set => _MaterialWFACClassMethod_MaterialWF = value;
    }

    public bool MaterialWFACClassMethod_MaterialWF_IsLoaded
    {
        get
        {
            return MaterialWFACClassMethod_MaterialWF != null;
        }
    }

    public virtual CollectionEntry MaterialWFACClassMethod_MaterialWFReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialWFACClassMethod_MaterialWF); }
    }

    private ICollection<MaterialWFRelation> _MaterialWFRelation_MaterialWF;
    public virtual ICollection<MaterialWFRelation> MaterialWFRelation_MaterialWF
    {
        get => LazyLoader.Load(this, ref _MaterialWFRelation_MaterialWF);
        set => _MaterialWFRelation_MaterialWF = value;
    }

    public bool MaterialWFRelation_MaterialWF_IsLoaded
    {
        get
        {
            return MaterialWFRelation_MaterialWF != null;
        }
    }

    public virtual CollectionEntry MaterialWFRelation_MaterialWFReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialWFRelation_MaterialWF); }
    }

    private ICollection<Partslist> _Partslist_MaterialWF;
    public virtual ICollection<Partslist> Partslist_MaterialWF
    {
        get => LazyLoader.Load(this, ref _Partslist_MaterialWF);
        set => _Partslist_MaterialWF = value;
    }

    public bool Partslist_MaterialWF_IsLoaded
    {
        get
        {
            return Partslist_MaterialWF != null;
        }
    }

    public virtual CollectionEntry Partslist_MaterialWFReference
    {
        get { return Context.Entry(this).Collection(c => c.Partslist_MaterialWF); }
    }
}
