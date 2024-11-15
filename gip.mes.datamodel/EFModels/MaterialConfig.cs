using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MaterialConfig : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public MaterialConfig()
    {
    }

    private MaterialConfig(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MaterialConfigID;
    public Guid MaterialConfigID 
    {
        get { return _MaterialConfigID; }
        set { SetProperty<Guid>(ref _MaterialConfigID, value); }
    }

    Guid? _MaterialID;
    public Guid? MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid?>(ref _MaterialID, value); }
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

    Guid? _ParentMaterialConfigID;
    public Guid? ParentMaterialConfigID 
    {
        get { return _ParentMaterialConfigID; }
        set { SetProperty<Guid?>(ref _ParentMaterialConfigID, value); }
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

    Guid? _VBiACClassWFID;
    public Guid? VBiACClassWFID 
    {
        get { return _VBiACClassWFID; }
        set { SetProperty<Guid?>(ref _VBiACClassWFID, value); }
    }

    private ICollection<MaterialConfig> _MaterialConfig_ParentMaterialConfig;
    public virtual ICollection<MaterialConfig> MaterialConfig_ParentMaterialConfig
    {
        get { return LazyLoader.Load(this, ref _MaterialConfig_ParentMaterialConfig); }
        set { _MaterialConfig_ParentMaterialConfig = value; }
    }

    public bool MaterialConfig_ParentMaterialConfig_IsLoaded
    {
        get
        {
            return _MaterialConfig_ParentMaterialConfig != null;
        }
    }

    public virtual CollectionEntry MaterialConfig_ParentMaterialConfigReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialConfig_ParentMaterialConfig); }
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
    
    private MaterialConfig _MaterialConfig1_ParentMaterialConfig;
    public virtual MaterialConfig MaterialConfig1_ParentMaterialConfig
    { 
        get { return LazyLoader.Load(this, ref _MaterialConfig1_ParentMaterialConfig); } 
        set { SetProperty<MaterialConfig>(ref _MaterialConfig1_ParentMaterialConfig, value); }
    }

    public bool MaterialConfig1_ParentMaterialConfig_IsLoaded
    {
        get
        {
            return _MaterialConfig1_ParentMaterialConfig != null;
        }
    }

    public virtual ReferenceEntry MaterialConfig1_ParentMaterialConfigReference 
    {
        get { return Context.Entry(this).Reference("MaterialConfig1_ParentMaterialConfig"); }
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
            return _VBiACClass != null;
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
            return _VBiACClassPropertyRelation != null;
        }
    }

    public virtual ReferenceEntry VBiACClassPropertyRelationReference 
    {
        get { return Context.Entry(this).Reference("VBiACClassPropertyRelation"); }
    }
    
    private ACClassWF _VBiACClassWF;
    public virtual ACClassWF VBiACClassWF
    { 
        get { return LazyLoader.Load(this, ref _VBiACClassWF); } 
        set { SetProperty<ACClassWF>(ref _VBiACClassWF, value); }
    }

    public bool VBiACClassWF_IsLoaded
    {
        get
        {
            return _VBiACClassWF != null;
        }
    }

    public virtual ReferenceEntry VBiACClassWFReference 
    {
        get { return Context.Entry(this).Reference("VBiACClassWF"); }
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
            return _VBiValueTypeACClass != null;
        }
    }

    public virtual ReferenceEntry VBiValueTypeACClassReference 
    {
        get { return Context.Entry(this).Reference("VBiValueTypeACClass"); }
    }
    }
