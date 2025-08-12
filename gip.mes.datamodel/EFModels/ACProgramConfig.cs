using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACProgramConfig : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public ACProgramConfig()
    {
    }

    private ACProgramConfig(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ACProgramConfigID;
    public Guid ACProgramConfigID 
    {
        get { return _ACProgramConfigID; }
        set { SetProperty<Guid>(ref _ACProgramConfigID, value); }
    }

    Guid _ACProgramID;
    public Guid ACProgramID 
    {
        get { return _ACProgramID; }
        set { SetForeignKeyProperty<Guid>(ref _ACProgramID, value, "ACProgram", _ACProgram, ACProgram != null ? ACProgram.ACProgramID : default(Guid)); }
    }

    Guid? _ParentACProgramConfigID;
    public Guid? ParentACProgramConfigID 
    {
        get { return _ParentACProgramConfigID; }
        set { SetForeignKeyProperty<Guid?>(ref _ParentACProgramConfigID, value, "ACProgramConfig1_ParentACProgramConfig", _ACProgramConfig1_ParentACProgramConfig, ACProgramConfig1_ParentACProgramConfig != null ? ACProgramConfig1_ParentACProgramConfig.ACProgramConfigID : default(Guid?)); }
    }

    Guid? _ACClassID;
    public Guid? ACClassID 
    {
        get { return _ACClassID; }
        set { SetForeignKeyProperty<Guid?>(ref _ACClassID, value, "ACClass", _ACClass, ACClass != null ? ACClass.ACClassID : default(Guid?)); }
    }

    Guid? _ACClassPropertyRelationID;
    public Guid? ACClassPropertyRelationID 
    {
        get { return _ACClassPropertyRelationID; }
        set { SetForeignKeyProperty<Guid?>(ref _ACClassPropertyRelationID, value, "ACClassPropertyRelation", _ACClassPropertyRelation, ACClassPropertyRelation != null ? ACClassPropertyRelation.ACClassPropertyRelationID : default(Guid?)); }
    }

    Guid _ValueTypeACClassID;
    public Guid ValueTypeACClassID 
    {
        get { return _ValueTypeACClassID; }
        set { SetForeignKeyProperty<Guid>(ref _ValueTypeACClassID, value, "ValueTypeACClass", _ValueTypeACClass, ValueTypeACClass != null ? ValueTypeACClass.ACClassID : default(Guid)); }
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
            return _ACClass != null;
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
            return _ACClassPropertyRelation != null;
        }
    }

    public virtual ReferenceEntry ACClassPropertyRelationReference 
    {
        get { return Context.Entry(this).Reference("ACClassPropertyRelation"); }
    }
    
    private ACProgram _ACProgram;
    public virtual ACProgram ACProgram
    { 
        get { return LazyLoader.Load(this, ref _ACProgram); } 
        set { SetProperty<ACProgram>(ref _ACProgram, value); }
    }

    public bool ACProgram_IsLoaded
    {
        get
        {
            return _ACProgram != null;
        }
    }

    public virtual ReferenceEntry ACProgramReference 
    {
        get { return Context.Entry(this).Reference("ACProgram"); }
    }
    
    private ICollection<ACProgramConfig> _ACProgramConfig_ParentACProgramConfig;
    public virtual ICollection<ACProgramConfig> ACProgramConfig_ParentACProgramConfig
    {
        get { return LazyLoader.Load(this, ref _ACProgramConfig_ParentACProgramConfig); }
        set { SetProperty<ICollection<ACProgramConfig>>(ref _ACProgramConfig_ParentACProgramConfig, value); }
    }

    public bool ACProgramConfig_ParentACProgramConfig_IsLoaded
    {
        get
        {
            return _ACProgramConfig_ParentACProgramConfig != null;
        }
    }

    public virtual CollectionEntry ACProgramConfig_ParentACProgramConfigReference
    {
        get { return Context.Entry(this).Collection(c => c.ACProgramConfig_ParentACProgramConfig); }
    }

    private ACProgramConfig _ACProgramConfig1_ParentACProgramConfig;
    public virtual ACProgramConfig ACProgramConfig1_ParentACProgramConfig
    { 
        get { return LazyLoader.Load(this, ref _ACProgramConfig1_ParentACProgramConfig); } 
        set { SetProperty<ACProgramConfig>(ref _ACProgramConfig1_ParentACProgramConfig, value); }
    }

    public bool ACProgramConfig1_ParentACProgramConfig_IsLoaded
    {
        get
        {
            return _ACProgramConfig1_ParentACProgramConfig != null;
        }
    }

    public virtual ReferenceEntry ACProgramConfig1_ParentACProgramConfigReference 
    {
        get { return Context.Entry(this).Reference("ACProgramConfig1_ParentACProgramConfig"); }
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
            return _ValueTypeACClass != null;
        }
    }

    public virtual ReferenceEntry ValueTypeACClassReference 
    {
        get { return Context.Entry(this).Reference("ValueTypeACClass"); }
    }
    }
