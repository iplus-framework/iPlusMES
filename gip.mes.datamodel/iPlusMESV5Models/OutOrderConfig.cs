using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class OutOrderConfig : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public OutOrderConfig()
    {
    }

    private OutOrderConfig(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _OutOrderConfigID;
    public Guid OutOrderConfigID 
    {
        get { return _OutOrderConfigID; }
        set { SetProperty<Guid>(ref _OutOrderConfigID, value); }
    }

    Guid _OutOrderID;
    public Guid OutOrderID 
    {
        get { return _OutOrderID; }
        set { SetProperty<Guid>(ref _OutOrderID, value); }
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

    Guid? _ParentOutOrderConfigID;
    public Guid? ParentOutOrderConfigID 
    {
        get { return _ParentOutOrderConfigID; }
        set { SetProperty<Guid?>(ref _ParentOutOrderConfigID, value); }
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

    private ObservableHashSet<OutOrderConfig> _OutOrderConfig_ParentOutOrderConfig;
    public virtual ObservableHashSet<OutOrderConfig> OutOrderConfig_ParentOutOrderConfig
    {
        get => LazyLoader.Load(this, ref _OutOrderConfig_ParentOutOrderConfig);
        set => _OutOrderConfig_ParentOutOrderConfig = value;
    }

    public bool OutOrderConfig_ParentOutOrderConfig_IsLoaded
    {
        get
        {
            return OutOrderConfig_ParentOutOrderConfig != null;
        }
    }

    public virtual CollectionEntry OutOrderConfig_ParentOutOrderConfigReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderConfig_ParentOutOrderConfig); }
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
    
    private OutOrder _OutOrder;
    public virtual OutOrder OutOrder
    { 
        get => LazyLoader.Load(this, ref _OutOrder);
        set => _OutOrder = value;
    }

    public bool OutOrder_IsLoaded
    {
        get
        {
            return OutOrder != null;
        }
    }

    public virtual ReferenceEntry OutOrderReference 
    {
        get { return Context.Entry(this).Reference("OutOrder"); }
    }
    
    private OutOrderConfig _OutOrderConfig1_ParentOutOrderConfig;
    public virtual OutOrderConfig OutOrderConfig1_ParentOutOrderConfig
    { 
        get => LazyLoader.Load(this, ref _OutOrderConfig1_ParentOutOrderConfig);
        set => _OutOrderConfig1_ParentOutOrderConfig = value;
    }

    public bool OutOrderConfig1_ParentOutOrderConfig_IsLoaded
    {
        get
        {
            return OutOrderConfig1_ParentOutOrderConfig != null;
        }
    }

    public virtual ReferenceEntry OutOrderConfig1_ParentOutOrderConfigReference 
    {
        get { return Context.Entry(this).Reference("OutOrderConfig1_ParentOutOrderConfig"); }
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
