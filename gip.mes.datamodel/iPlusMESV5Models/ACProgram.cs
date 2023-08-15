using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACProgram : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public ACProgram()
    {
    }

    private ACProgram(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ACProgramID;
    public Guid ACProgramID 
    {
        get { return _ACProgramID; }
        set { SetProperty<Guid>(ref _ACProgramID, value); }
    }

    Guid _WorkflowTypeACClassID;
    public Guid WorkflowTypeACClassID 
    {
        get { return _WorkflowTypeACClassID; }
        set { SetProperty<Guid>(ref _WorkflowTypeACClassID, value); }
    }

    Guid? _ProgramACClassMethodID;
    public Guid? ProgramACClassMethodID 
    {
        get { return _ProgramACClassMethodID; }
        set { SetProperty<Guid?>(ref _ProgramACClassMethodID, value); }
    }

    string _ProgramNo;
    public string ProgramNo 
    {
        get { return _ProgramNo; }
        set { SetProperty<string>(ref _ProgramNo, value); }
    }

    string _ProgramName;
    public string ProgramName 
    {
        get { return _ProgramName; }
        set { SetProperty<string>(ref _ProgramName, value); }
    }

    short _ACProgramTypeIndex;
    public short ACProgramTypeIndex 
    {
        get { return _ACProgramTypeIndex; }
        set { SetProperty<short>(ref _ACProgramTypeIndex, value); }
    }

    DateTime _PlannedStartDate;
    public DateTime PlannedStartDate 
    {
        get { return _PlannedStartDate; }
        set { SetProperty<DateTime>(ref _PlannedStartDate, value); }
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

    private ObservableHashSet<ACClassTask> _ACClassTask_ACProgram;
    public virtual ObservableHashSet<ACClassTask> ACClassTask_ACProgram
    {
        get => LazyLoader.Load(this, ref _ACClassTask_ACProgram);
        set => _ACClassTask_ACProgram = value;
    }

    public bool ACClassTask_ACProgram_IsLoaded
    {
        get
        {
            return ACClassTask_ACProgram != null;
        }
    }

    public virtual CollectionEntry ACClassTask_ACProgramReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassTask_ACProgram); }
    }

    private ObservableHashSet<ACProgramConfig> _ACProgramConfig_ACProgram;
    public virtual ObservableHashSet<ACProgramConfig> ACProgramConfig_ACProgram
    {
        get => LazyLoader.Load(this, ref _ACProgramConfig_ACProgram);
        set => _ACProgramConfig_ACProgram = value;
    }

    public bool ACProgramConfig_ACProgram_IsLoaded
    {
        get
        {
            return ACProgramConfig_ACProgram != null;
        }
    }

    public virtual CollectionEntry ACProgramConfig_ACProgramReference
    {
        get { return Context.Entry(this).Collection(c => c.ACProgramConfig_ACProgram); }
    }

    private ObservableHashSet<ACProgramLog> _ACProgramLog_ACProgram;
    public virtual ObservableHashSet<ACProgramLog> ACProgramLog_ACProgram
    {
        get => LazyLoader.Load(this, ref _ACProgramLog_ACProgram);
        set => _ACProgramLog_ACProgram = value;
    }

    public bool ACProgramLog_ACProgram_IsLoaded
    {
        get
        {
            return ACProgramLog_ACProgram != null;
        }
    }

    public virtual CollectionEntry ACProgramLog_ACProgramReference
    {
        get { return Context.Entry(this).Collection(c => c.ACProgramLog_ACProgram); }
    }

    private ObservableHashSet<DemandOrderPos> _DemandOrderPos_ACProgram;
    public virtual ObservableHashSet<DemandOrderPos> DemandOrderPos_ACProgram
    {
        get => LazyLoader.Load(this, ref _DemandOrderPos_ACProgram);
        set => _DemandOrderPos_ACProgram = value;
    }

    public bool DemandOrderPos_ACProgram_IsLoaded
    {
        get
        {
            return DemandOrderPos_ACProgram != null;
        }
    }

    public virtual CollectionEntry DemandOrderPos_ACProgramReference
    {
        get { return Context.Entry(this).Collection(c => c.DemandOrderPos_ACProgram); }
    }

    private ObservableHashSet<ProdOrderPartslist> _ProdOrderPartslist_VBiACProgram;
    public virtual ObservableHashSet<ProdOrderPartslist> ProdOrderPartslist_VBiACProgram
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslist_VBiACProgram);
        set => _ProdOrderPartslist_VBiACProgram = value;
    }

    public bool ProdOrderPartslist_VBiACProgram_IsLoaded
    {
        get
        {
            return ProdOrderPartslist_VBiACProgram != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslist_VBiACProgramReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslist_VBiACProgram); }
    }

    private ACClassMethod _ProgramACClassMethod;
    public virtual ACClassMethod ProgramACClassMethod
    { 
        get => LazyLoader.Load(this, ref _ProgramACClassMethod);
        set => _ProgramACClassMethod = value;
    }

    public bool ProgramACClassMethod_IsLoaded
    {
        get
        {
            return ProgramACClassMethod != null;
        }
    }

    public virtual ReferenceEntry ProgramACClassMethodReference 
    {
        get { return Context.Entry(this).Reference("ProgramACClassMethod"); }
    }
    
    private ACClass _WorkflowTypeACClass;
    public virtual ACClass WorkflowTypeACClass
    { 
        get => LazyLoader.Load(this, ref _WorkflowTypeACClass);
        set => _WorkflowTypeACClass = value;
    }

    public bool WorkflowTypeACClass_IsLoaded
    {
        get
        {
            return WorkflowTypeACClass != null;
        }
    }

    public virtual ReferenceEntry WorkflowTypeACClassReference 
    {
        get { return Context.Entry(this).Reference("WorkflowTypeACClass"); }
    }
    }
