using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class VBConfig : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public VBConfig()
    {
    }

    private VBConfig(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _VBConfigID;
    public Guid VBConfigID 
    {
        get { return _VBConfigID; }
        set { SetProperty<Guid>(ref _VBConfigID, value); }
    }

    Guid? _ParentVBConfigID;
    public Guid? ParentVBConfigID 
    {
        get { return _ParentVBConfigID; }
        set { SetProperty<Guid?>(ref _ParentVBConfigID, value); }
    }

    Guid? _ACClassID;
    public Guid? ACClassID 
    {
        get { return _ACClassID; }
        set { SetProperty<Guid?>(ref _ACClassID, value); }
    }

    Guid? _ACClassPropertyRelationID;
    public Guid? ACClassPropertyRelationID 
    {
        get { return _ACClassPropertyRelationID; }
        set { SetProperty<Guid?>(ref _ACClassPropertyRelationID, value); }
    }

    Guid _ValueTypeACClassID;
    public Guid ValueTypeACClassID 
    {
        get { return _ValueTypeACClassID; }
        set { SetProperty<Guid>(ref _ValueTypeACClassID, value); }
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

    private ACClass _ACClass;
    public virtual ACClass ACClass
    { 
        get { return LazyLoader.Load(this, ref _ACClass); } 
        set { SetProperty<ACClass>(ref _ACClass, value); }
    }

    public bool ACClass_IsLoaded
    {
        get
        {
            return ACClass != null;
        }
    }

    public virtual ReferenceEntry ACClassReference 
    {
        get { return Context.Entry(this).Reference("ACClass"); }
    }
    
    private ACClassPropertyRelation _ACClassPropertyRelation;
    public virtual ACClassPropertyRelation ACClassPropertyRelation
    { 
        get { return LazyLoader.Load(this, ref _ACClassPropertyRelation); } 
        set { SetProperty<ACClassPropertyRelation>(ref _ACClassPropertyRelation, value); }
    }

    public bool ACClassPropertyRelation_IsLoaded
    {
        get
        {
            return ACClassPropertyRelation != null;
        }
    }

    public virtual ReferenceEntry ACClassPropertyRelationReference 
    {
        get { return Context.Entry(this).Reference("ACClassPropertyRelation"); }
    }
    
    private ICollection<VBConfig> _VBConfig_ParentVBConfig;
    public virtual ICollection<VBConfig> VBConfig_ParentVBConfig
    {
        get { return LazyLoader.Load(this, ref _VBConfig_ParentVBConfig); }
        set { _VBConfig_ParentVBConfig = value; }
    }

    public bool VBConfig_ParentVBConfig_IsLoaded
    {
        get
        {
            return VBConfig_ParentVBConfig != null;
        }
    }

    public virtual CollectionEntry VBConfig_ParentVBConfigReference
    {
        get { return Context.Entry(this).Collection(c => c.VBConfig_ParentVBConfig); }
    }

    private VBConfig _VBConfig1_ParentVBConfig;
    public virtual VBConfig VBConfig1_ParentVBConfig
    { 
        get { return LazyLoader.Load(this, ref _VBConfig1_ParentVBConfig); } 
        set { SetProperty<VBConfig>(ref _VBConfig1_ParentVBConfig, value); }
    }

    public bool VBConfig1_ParentVBConfig_IsLoaded
    {
        get
        {
            return VBConfig1_ParentVBConfig != null;
        }
    }

    public virtual ReferenceEntry VBConfig1_ParentVBConfigReference 
    {
        get { return Context.Entry(this).Reference("VBConfig1_ParentVBConfig"); }
    }
    
    private ACClass _ValueTypeACClass;
    public virtual ACClass ValueTypeACClass
    { 
        get { return LazyLoader.Load(this, ref _ValueTypeACClass); } 
        set { SetProperty<ACClass>(ref _ValueTypeACClass, value); }
    }

    public bool ValueTypeACClass_IsLoaded
    {
        get
        {
            return ValueTypeACClass != null;
        }
    }

    public virtual ReferenceEntry ValueTypeACClassReference 
    {
        get { return Context.Entry(this).Reference("ValueTypeACClass"); }
    }
    }
