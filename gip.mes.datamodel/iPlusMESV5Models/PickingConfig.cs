using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class PickingConfig : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public PickingConfig()
    {
    }

    private PickingConfig(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _PickingConfigID;
    public Guid PickingConfigID 
    {
        get { return _PickingConfigID; }
        set { SetProperty<Guid>(ref _PickingConfigID, value); }
    }

    Guid? _ParentPickingConfigID;
    public Guid? ParentPickingConfigID 
    {
        get { return _ParentPickingConfigID; }
        set { SetProperty<Guid?>(ref _ParentPickingConfigID, value); }
    }

    Guid _PickingID;
    public Guid PickingID 
    {
        get { return _PickingID; }
        set { SetProperty<Guid>(ref _PickingID, value); }
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

    Guid? _VBiACClassWFID;
    public Guid? VBiACClassWFID 
    {
        get { return _VBiACClassWFID; }
        set { SetProperty<Guid?>(ref _VBiACClassWFID, value); }
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

    private ObservableHashSet<PickingConfig> _PickingConfig_ParentPickingConfig;
    public virtual ObservableHashSet<PickingConfig> PickingConfig_ParentPickingConfig
    {
        get => LazyLoader.Load(this, ref _PickingConfig_ParentPickingConfig);
        set => _PickingConfig_ParentPickingConfig = value;
    }

    public bool PickingConfig_ParentPickingConfig_IsLoaded
    {
        get
        {
            return PickingConfig_ParentPickingConfig != null;
        }
    }

    public virtual CollectionEntry PickingConfig_ParentPickingConfigReference
    {
        get { return Context.Entry(this).Collection(c => c.PickingConfig_ParentPickingConfig); }
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
    
    private PickingConfig _PickingConfig1_ParentPickingConfig;
    public virtual PickingConfig PickingConfig1_ParentPickingConfig
    { 
        get => LazyLoader.Load(this, ref _PickingConfig1_ParentPickingConfig);
        set => _PickingConfig1_ParentPickingConfig = value;
    }

    public bool PickingConfig1_ParentPickingConfig_IsLoaded
    {
        get
        {
            return PickingConfig1_ParentPickingConfig != null;
        }
    }

    public virtual ReferenceEntry PickingConfig1_ParentPickingConfigReference 
    {
        get { return Context.Entry(this).Reference("PickingConfig1_ParentPickingConfig"); }
    }
    
    private Picking _Picking;
    public virtual Picking Picking
    { 
        get => LazyLoader.Load(this, ref _Picking);
        set => _Picking = value;
    }

    public bool Picking_IsLoaded
    {
        get
        {
            return Picking != null;
        }
    }

    public virtual ReferenceEntry PickingReference 
    {
        get { return Context.Entry(this).Reference("Picking"); }
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
    
    private ACClassWF _VBiACClassWF;
    public virtual ACClassWF VBiACClassWF
    { 
        get => LazyLoader.Load(this, ref _VBiACClassWF);
        set => _VBiACClassWF = value;
    }

    public bool VBiACClassWF_IsLoaded
    {
        get
        {
            return VBiACClassWF != null;
        }
    }

    public virtual ReferenceEntry VBiACClassWFReference 
    {
        get { return Context.Entry(this).Reference("VBiACClassWF"); }
    }
    
    private ACClass _VBiValueTypeACClass;
    public virtual ACClass VBiValueTypeACClass
    { 
        get => LazyLoader.Load(this, ref _VBiValueTypeACClass);
        set => _VBiValueTypeACClass = value;
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
