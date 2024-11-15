using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class HistoryConfig : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public HistoryConfig()
    {
    }

    private HistoryConfig(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _HistoryConfigID;
    public Guid HistoryConfigID 
    {
        get { return _HistoryConfigID; }
        set { SetProperty<Guid>(ref _HistoryConfigID, value); }
    }

    Guid? _HistoryID;
    public Guid? HistoryID 
    {
        get { return _HistoryID; }
        set { SetProperty<Guid?>(ref _HistoryID, value); }
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

    Guid? _ParentHistoryConfigID;
    public Guid? ParentHistoryConfigID 
    {
        get { return _ParentHistoryConfigID; }
        set { SetProperty<Guid?>(ref _ParentHistoryConfigID, value); }
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

    private History _History;
    public virtual History History
    { 
        get { return LazyLoader.Load(this, ref _History); } 
        set { SetProperty<History>(ref _History, value); }
    }

    public bool History_IsLoaded
    {
        get
        {
            return _History != null;
        }
    }

    public virtual ReferenceEntry HistoryReference 
    {
        get { return Context.Entry(this).Reference("History"); }
    }
    
    private ICollection<HistoryConfig> _HistoryConfig_ParentHistoryConfig;
    public virtual ICollection<HistoryConfig> HistoryConfig_ParentHistoryConfig
    {
        get { return LazyLoader.Load(this, ref _HistoryConfig_ParentHistoryConfig); }
        set { _HistoryConfig_ParentHistoryConfig = value; }
    }

    public bool HistoryConfig_ParentHistoryConfig_IsLoaded
    {
        get
        {
            return _HistoryConfig_ParentHistoryConfig != null;
        }
    }

    public virtual CollectionEntry HistoryConfig_ParentHistoryConfigReference
    {
        get { return Context.Entry(this).Collection(c => c.HistoryConfig_ParentHistoryConfig); }
    }

    private HistoryConfig _HistoryConfig1_ParentHistoryConfig;
    public virtual HistoryConfig HistoryConfig1_ParentHistoryConfig
    { 
        get { return LazyLoader.Load(this, ref _HistoryConfig1_ParentHistoryConfig); } 
        set { SetProperty<HistoryConfig>(ref _HistoryConfig1_ParentHistoryConfig, value); }
    }

    public bool HistoryConfig1_ParentHistoryConfig_IsLoaded
    {
        get
        {
            return _HistoryConfig1_ParentHistoryConfig != null;
        }
    }

    public virtual ReferenceEntry HistoryConfig1_ParentHistoryConfigReference 
    {
        get { return Context.Entry(this).Reference("HistoryConfig1_ParentHistoryConfig"); }
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
