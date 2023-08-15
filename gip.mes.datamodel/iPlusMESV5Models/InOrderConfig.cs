using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class InOrderConfig : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public InOrderConfig()
    {
    }

    private InOrderConfig(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _InOrderConfigID;
    public Guid InOrderConfigID 
    {
        get { return _InOrderConfigID; }
        set { SetProperty<Guid>(ref _InOrderConfigID, value); }
    }

    Guid _InOrderID;
    public Guid InOrderID 
    {
        get { return _InOrderID; }
        set { SetProperty<Guid>(ref _InOrderID, value); }
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

    Guid? _ParentInOrderConfigID;
    public Guid? ParentInOrderConfigID 
    {
        get { return _ParentInOrderConfigID; }
        set { SetProperty<Guid?>(ref _ParentInOrderConfigID, value); }
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

    private InOrder _InOrder;
    public virtual InOrder InOrder
    { 
        get => LazyLoader.Load(this, ref _InOrder);
        set => _InOrder = value;
    }

    public bool InOrder_IsLoaded
    {
        get
        {
            return InOrder != null;
        }
    }

    public virtual ReferenceEntry InOrderReference 
    {
        get { return Context.Entry(this).Reference("InOrder"); }
    }
    
    private ICollection<InOrderConfig> _InOrderConfig_ParentInOrderConfig;
    public virtual ICollection<InOrderConfig> InOrderConfig_ParentInOrderConfig
    {
        get => LazyLoader.Load(this, ref _InOrderConfig_ParentInOrderConfig);
        set => _InOrderConfig_ParentInOrderConfig = value;
    }

    public bool InOrderConfig_ParentInOrderConfig_IsLoaded
    {
        get
        {
            return InOrderConfig_ParentInOrderConfig != null;
        }
    }

    public virtual CollectionEntry InOrderConfig_ParentInOrderConfigReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderConfig_ParentInOrderConfig); }
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
    
    private InOrderConfig _InOrderConfig1_ParentInOrderConfig;
    public virtual InOrderConfig InOrderConfig1_ParentInOrderConfig
    { 
        get => LazyLoader.Load(this, ref _InOrderConfig1_ParentInOrderConfig);
        set => _InOrderConfig1_ParentInOrderConfig = value;
    }

    public bool InOrderConfig1_ParentInOrderConfig_IsLoaded
    {
        get
        {
            return InOrderConfig1_ParentInOrderConfig != null;
        }
    }

    public virtual ReferenceEntry InOrderConfig1_ParentInOrderConfigReference 
    {
        get { return Context.Entry(this).Reference("InOrderConfig1_ParentInOrderConfig"); }
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
