using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACClassTask : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public ACClassTask()
    {
    }

    private ACClassTask(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ACClassTaskID;
    public Guid ACClassTaskID 
    {
        get { return _ACClassTaskID; }
        set { SetProperty<Guid>(ref _ACClassTaskID, value); }
    }

    Guid? _ParentACClassTaskID;
    public Guid? ParentACClassTaskID 
    {
        get { return _ParentACClassTaskID; }
        set { SetProperty<Guid?>(ref _ParentACClassTaskID, value); }
    }

    Guid _TaskTypeACClassID;
    public Guid TaskTypeACClassID 
    {
        get { return _TaskTypeACClassID; }
        set { SetProperty<Guid>(ref _TaskTypeACClassID, value); }
    }

    Guid? _ContentACClassWFID;
    public Guid? ContentACClassWFID 
    {
        get { return _ContentACClassWFID; }
        set { SetProperty<Guid?>(ref _ContentACClassWFID, value); }
    }

    Guid? _ACProgramID;
    public Guid? ACProgramID 
    {
        get { return _ACProgramID; }
        set { SetProperty<Guid?>(ref _ACProgramID, value); }
    }

    string _ACIdentifier;
    public override string ACIdentifier 
    {
        get { return _ACIdentifier; }
        set { SetProperty<string>(ref _ACIdentifier, value); }
    }

    short _ACTaskTypeIndex;
    public short ACTaskTypeIndex 
    {
        get { return _ACTaskTypeIndex; }
        set { SetProperty<short>(ref _ACTaskTypeIndex, value); }
    }

    bool _IsDynamic;
    public bool IsDynamic 
    {
        get { return _IsDynamic; }
        set { SetProperty<bool>(ref _IsDynamic, value); }
    }

    bool _IsTestmode;
    public bool IsTestmode 
    {
        get { return _IsTestmode; }
        set { SetProperty<bool>(ref _IsTestmode, value); }
    }

    string _XMLACMethod;
    public string XMLACMethod 
    {
        get { return _XMLACMethod; }
        set { SetProperty<string>(ref _XMLACMethod, value); }
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

    private ObservableHashSet<ACClassTaskValue> _ACClassTaskValue_ACClassTask;
    public virtual ObservableHashSet<ACClassTaskValue> ACClassTaskValue_ACClassTask
    {
        get => LazyLoader.Load(this, ref _ACClassTaskValue_ACClassTask);
        set => _ACClassTaskValue_ACClassTask = value;
    }

    public bool ACClassTaskValue_ACClassTask_IsLoaded
    {
        get
        {
            return ACClassTaskValue_ACClassTask != null;
        }
    }

    public virtual CollectionEntry ACClassTaskValue_ACClassTaskReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassTaskValue_ACClassTask); }
    }

    private ACProgram _ACProgram;
    public virtual ACProgram ACProgram
    { 
        get => LazyLoader.Load(this, ref _ACProgram);
        set => _ACProgram = value;
    }

    public bool ACProgram_IsLoaded
    {
        get
        {
            return ACProgram != null;
        }
    }

    public virtual ReferenceEntry ACProgramReference 
    {
        get { return Context.Entry(this).Reference("ACProgram"); }
    }
    
    private ACClassWF _ContentACClassWF;
    public virtual ACClassWF ContentACClassWF
    { 
        get => LazyLoader.Load(this, ref _ContentACClassWF);
        set => _ContentACClassWF = value;
    }

    public bool ContentACClassWF_IsLoaded
    {
        get
        {
            return ContentACClassWF != null;
        }
    }

    public virtual ReferenceEntry ContentACClassWFReference 
    {
        get { return Context.Entry(this).Reference("ContentACClassWF"); }
    }
    
    private ObservableHashSet<ACClassTask> _ACClassTask_ParentACClassTask;
    public virtual ObservableHashSet<ACClassTask> ACClassTask_ParentACClassTask
    {
        get => LazyLoader.Load(this, ref _ACClassTask_ParentACClassTask);
        set => _ACClassTask_ParentACClassTask = value;
    }

    public bool ACClassTask_ParentACClassTask_IsLoaded
    {
        get
        {
            return ACClassTask_ParentACClassTask != null;
        }
    }

    public virtual CollectionEntry ACClassTask_ParentACClassTaskReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassTask_ParentACClassTask); }
    }

    private ACClassTask _ACClassTask1_ParentACClassTask;
    public virtual ACClassTask ACClassTask1_ParentACClassTask
    { 
        get => LazyLoader.Load(this, ref _ACClassTask1_ParentACClassTask);
        set => _ACClassTask1_ParentACClassTask = value;
    }

    public bool ACClassTask1_ParentACClassTask_IsLoaded
    {
        get
        {
            return ACClassTask1_ParentACClassTask != null;
        }
    }

    public virtual ReferenceEntry ACClassTask1_ParentACClassTaskReference 
    {
        get { return Context.Entry(this).Reference("ACClassTask1_ParentACClassTask"); }
    }
    
    private ObservableHashSet<PickingPos> _PickingPos_ACClassTask;
    public virtual ObservableHashSet<PickingPos> PickingPos_ACClassTask
    {
        get => LazyLoader.Load(this, ref _PickingPos_ACClassTask);
        set => _PickingPos_ACClassTask = value;
    }

    public bool PickingPos_ACClassTask_IsLoaded
    {
        get
        {
            return PickingPos_ACClassTask != null;
        }
    }

    public virtual CollectionEntry PickingPos_ACClassTaskReference
    {
        get { return Context.Entry(this).Collection(c => c.PickingPos_ACClassTask); }
    }

    private ObservableHashSet<ProdOrderPartslistPos> _ProdOrderPartslistPos_ACClassTask;
    public virtual ObservableHashSet<ProdOrderPartslistPos> ProdOrderPartslistPos_ACClassTask
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPos_ACClassTask);
        set => _ProdOrderPartslistPos_ACClassTask = value;
    }

    public bool ProdOrderPartslistPos_ACClassTask_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPos_ACClassTask != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistPos_ACClassTaskReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistPos_ACClassTask); }
    }

    private ACClass _TaskTypeACClass;
    public virtual ACClass TaskTypeACClass
    { 
        get => LazyLoader.Load(this, ref _TaskTypeACClass);
        set => _TaskTypeACClass = value;
    }

    public bool TaskTypeACClass_IsLoaded
    {
        get
        {
            return TaskTypeACClass != null;
        }
    }

    public virtual ReferenceEntry TaskTypeACClassReference 
    {
        get { return Context.Entry(this).Reference("TaskTypeACClass"); }
    }
    }
