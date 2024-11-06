using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TourplanConfig : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public TourplanConfig()
    {
    }

    private TourplanConfig(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _TourplanConfigID;
    public Guid TourplanConfigID 
    {
        get { return _TourplanConfigID; }
        set { SetProperty<Guid>(ref _TourplanConfigID, value); }
    }

    Guid _TourplanID;
    public Guid TourplanID 
    {
        get { return _TourplanID; }
        set { SetProperty<Guid>(ref _TourplanID, value); }
    }

    Guid? _VBiACClassID;
    public Guid? VBiACClassID 
    {
        get { return _VBiACClassID; }
        set { SetProperty<Guid?>(ref _VBiACClassID, value); }
    }

    Guid? _VBiACClassPropertyRelationID;
    public Guid? VBiACClassPropertyRelationID 
    {
        get { return _VBiACClassPropertyRelationID; }
        set { SetProperty<Guid?>(ref _VBiACClassPropertyRelationID, value); }
    }

    Guid? _MaterialID;
    public Guid? MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid?>(ref _MaterialID, value); }
    }

    Guid? _ParentTourplanConfigID;
    public Guid? ParentTourplanConfigID 
    {
        get { return _ParentTourplanConfigID; }
        set { SetProperty<Guid?>(ref _ParentTourplanConfigID, value); }
    }

    Guid _VBiValueTypeACClassID;
    public Guid VBiValueTypeACClassID 
    {
        get { return _VBiValueTypeACClassID; }
        set { SetProperty<Guid>(ref _VBiValueTypeACClassID, value); }
    }

    string _KeyACUrl;
    public string KeyACUrl 
    {
        get { return _KeyACUrl; }
        set { SetProperty<string>(ref _KeyACUrl, value); }
    }

    string _PreConfigACUrl;
    public string PreConfigACUrl 
    {
        get { return _PreConfigACUrl; }
        set { SetProperty<string>(ref _PreConfigACUrl, value); }
    }

    string _LocalConfigACUrl;
    public string LocalConfigACUrl 
    {
        get { return _LocalConfigACUrl; }
        set { SetProperty<string>(ref _LocalConfigACUrl, value); }
    }

    string _Expression;
    public string Expression 
    {
        get { return _Expression; }
        set { SetProperty<string>(ref _Expression, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
    }

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
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

    private ICollection<TourplanConfig> _TourplanConfig_ParentTourplanConfig;
    public virtual ICollection<TourplanConfig> TourplanConfig_ParentTourplanConfig
    {
        get { return LazyLoader.Load(this, ref _TourplanConfig_ParentTourplanConfig); }
        set { _TourplanConfig_ParentTourplanConfig = value; }
    }

    public bool TourplanConfig_ParentTourplanConfig_IsLoaded
    {
        get
        {
            return TourplanConfig_ParentTourplanConfig != null;
        }
    }

    public virtual CollectionEntry TourplanConfig_ParentTourplanConfigReference
    {
        get { return Context.Entry(this).Collection(c => c.TourplanConfig_ParentTourplanConfig); }
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
    
    private TourplanConfig _TourplanConfig1_ParentTourplanConfig;
    public virtual TourplanConfig TourplanConfig1_ParentTourplanConfig
    { 
        get { return LazyLoader.Load(this, ref _TourplanConfig1_ParentTourplanConfig); } 
        set { SetProperty<TourplanConfig>(ref _TourplanConfig1_ParentTourplanConfig, value); }
    }

    public bool TourplanConfig1_ParentTourplanConfig_IsLoaded
    {
        get
        {
            return TourplanConfig1_ParentTourplanConfig != null;
        }
    }

    public virtual ReferenceEntry TourplanConfig1_ParentTourplanConfigReference 
    {
        get { return Context.Entry(this).Reference("TourplanConfig1_ParentTourplanConfig"); }
    }
    
    private Tourplan _Tourplan;
    public virtual Tourplan Tourplan
    { 
        get { return LazyLoader.Load(this, ref _Tourplan); } 
        set { SetProperty<Tourplan>(ref _Tourplan, value); }
    }

    public bool Tourplan_IsLoaded
    {
        get
        {
            return Tourplan != null;
        }
    }

    public virtual ReferenceEntry TourplanReference 
    {
        get { return Context.Entry(this).Reference("Tourplan"); }
    }
    
    private ACClass _VBiACClass;
    public virtual ACClass VBiACClass
    { 
        get { return LazyLoader.Load(this, ref _VBiACClass); } 
        set { SetProperty<ACClass>(ref _VBiACClass, value); }
    }

    public bool VBiACClass_IsLoaded
    {
        get
        {
            return VBiACClass != null;
        }
    }

    public virtual ReferenceEntry VBiACClassReference 
    {
        get { return Context.Entry(this).Reference("VBiACClass"); }
    }
    
    private ACClassPropertyRelation _VBiACClassPropertyRelation;
    public virtual ACClassPropertyRelation VBiACClassPropertyRelation
    { 
        get { return LazyLoader.Load(this, ref _VBiACClassPropertyRelation); } 
        set { SetProperty<ACClassPropertyRelation>(ref _VBiACClassPropertyRelation, value); }
    }

    public bool VBiACClassPropertyRelation_IsLoaded
    {
        get
        {
            return VBiACClassPropertyRelation != null;
        }
    }

    public virtual ReferenceEntry VBiACClassPropertyRelationReference 
    {
        get { return Context.Entry(this).Reference("VBiACClassPropertyRelation"); }
    }
    
    private ACClass _VBiValueTypeACClass;
    public virtual ACClass VBiValueTypeACClass
    { 
        get { return LazyLoader.Load(this, ref _VBiValueTypeACClass); } 
        set { SetProperty<ACClass>(ref _VBiValueTypeACClass, value); }
    }

    public bool VBiValueTypeACClass_IsLoaded
    {
        get
        {
            return VBiValueTypeACClass != null;
        }
    }

    public virtual ReferenceEntry VBiValueTypeACClassReference 
    {
        get { return Context.Entry(this).Reference("VBiValueTypeACClass"); }
    }
    }
