using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACClassMethodConfig : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public ACClassMethodConfig()
    {
    }

    private ACClassMethodConfig(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _ACClassMethodConfigID;
    public Guid ACClassMethodConfigID 
    {
        get { return _ACClassMethodConfigID; }
        set { SetProperty<Guid>(ref _ACClassMethodConfigID, value); }
    }

    Guid? _ParentACClassMethodConfigID;
    public Guid? ParentACClassMethodConfigID 
    {
        get { return _ParentACClassMethodConfigID; }
        set { SetProperty<Guid?>(ref _ParentACClassMethodConfigID, value); }
    }

    Guid _ACClassMethodID;
    public Guid ACClassMethodID 
    {
        get { return _ACClassMethodID; }
        set { SetProperty<Guid>(ref _ACClassMethodID, value); }
    }

    Guid? _ACClassWFID;
    public Guid? ACClassWFID 
    {
        get { return _ACClassWFID; }
        set { SetProperty<Guid?>(ref _ACClassWFID, value); }
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

    private ACClassMethod _ACClassMethod;
    public virtual ACClassMethod ACClassMethod
    { 
        get => LazyLoader.Load(this, ref _ACClassMethod);
        set => _ACClassMethod = value;
    }

    public bool ACClassMethod_IsLoaded
    {
        get
        {
            return ACClassMethod != null;
        }
    }

    public virtual ReferenceEntry ACClassMethodReference 
    {
        get { return Context.Entry(this).Reference("ACClassMethod"); }
    }
    
    private ACClassWF _ACClassWF;
    public virtual ACClassWF ACClassWF
    { 
        get => LazyLoader.Load(this, ref _ACClassWF);
        set => _ACClassWF = value;
    }

    public bool ACClassWF_IsLoaded
    {
        get
        {
            return ACClassWF != null;
        }
    }

    public virtual ReferenceEntry ACClassWFReference 
    {
        get { return Context.Entry(this).Reference("ACClassWF"); }
    }
    
    private ICollection<ACClassMethodConfig> _ACClassMethodConfig_ParentACClassMethodConfig;
    public virtual ICollection<ACClassMethodConfig> ACClassMethodConfig_ParentACClassMethodConfig
    {
        get => LazyLoader.Load(this, ref _ACClassMethodConfig_ParentACClassMethodConfig);
        set => _ACClassMethodConfig_ParentACClassMethodConfig = value;
    }

    public bool ACClassMethodConfig_ParentACClassMethodConfig_IsLoaded
    {
        get
        {
            return ACClassMethodConfig_ParentACClassMethodConfig != null;
        }
    }

    public virtual CollectionEntry ACClassMethodConfig_ParentACClassMethodConfigReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassMethodConfig_ParentACClassMethodConfig); }
    }

    private ACClassMethodConfig _ACClassMethodConfig1_ParentACClassMethodConfig;
    public virtual ACClassMethodConfig ACClassMethodConfig1_ParentACClassMethodConfig
    { 
        get => LazyLoader.Load(this, ref _ACClassMethodConfig1_ParentACClassMethodConfig);
        set => _ACClassMethodConfig1_ParentACClassMethodConfig = value;
    }

    public bool ACClassMethodConfig1_ParentACClassMethodConfig_IsLoaded
    {
        get
        {
            return ACClassMethodConfig1_ParentACClassMethodConfig != null;
        }
    }

    public virtual ReferenceEntry ACClassMethodConfig1_ParentACClassMethodConfigReference 
    {
        get { return Context.Entry(this).Reference("ACClassMethodConfig1_ParentACClassMethodConfig"); }
    }
    
    private ACClass _VBiACClass;
    public virtual ACClass VBiACClass
    { 
        get => LazyLoader.Load(this, ref _VBiACClass);
        set => _VBiACClass = value;
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
        get => LazyLoader.Load(this, ref _VBiACClassPropertyRelation);
        set => _VBiACClassPropertyRelation = value;
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
    
    private ACClass _ValueTypeACClass;
    public virtual ACClass ValueTypeACClass
    { 
        get => LazyLoader.Load(this, ref _ValueTypeACClass);
        set => _ValueTypeACClass = value;
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
