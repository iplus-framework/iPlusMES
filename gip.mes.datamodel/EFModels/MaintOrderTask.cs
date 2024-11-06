using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MaintOrderTask : VBEntityObject, IInsertInfo, IUpdateInfo, ISequence
{

    public MaintOrderTask()
    {
    }

    private MaintOrderTask(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MaintOrderTaskID;
    public Guid MaintOrderTaskID 
    {
        get { return _MaintOrderTaskID; }
        set { SetProperty<Guid>(ref _MaintOrderTaskID, value); }
    }

    Guid _MaintOrderID;
    public Guid MaintOrderID 
    {
        get { return _MaintOrderID; }
        set { SetProperty<Guid>(ref _MaintOrderID, value); }
    }

    Guid _MDMaintTaskStateID;
    public Guid MDMaintTaskStateID 
    {
        get { return _MDMaintTaskStateID; }
        set { SetProperty<Guid>(ref _MDMaintTaskStateID, value); }
    }

    string _TaskDescription;
    public string TaskDescription 
    {
        get { return _TaskDescription; }
        set { SetProperty<string>(ref _TaskDescription, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
    }

    DateTime? _PlannedStartDate;
    public DateTime? PlannedStartDate 
    {
        get { return _PlannedStartDate; }
        set { SetProperty<DateTime?>(ref _PlannedStartDate, value); }
    }

    int? _PlannedDuration;
    public int? PlannedDuration 
    {
        get { return _PlannedDuration; }
        set { SetProperty<int?>(ref _PlannedDuration, value); }
    }

    DateTime? _StartDate;
    public DateTime? StartDate 
    {
        get { return _StartDate; }
        set { SetProperty<DateTime?>(ref _StartDate, value); }
    }

    DateTime? _EndDate;
    public DateTime? EndDate 
    {
        get { return _EndDate; }
        set { SetProperty<DateTime?>(ref _EndDate, value); }
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

    string _TaskName;
    public string TaskName 
    {
        get { return _TaskName; }
        set { SetProperty<string>(ref _TaskName, value); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    private MDMaintTaskState _MDMaintTaskState;
    public virtual MDMaintTaskState MDMaintTaskState
    { 
        get { return LazyLoader.Load(this, ref _MDMaintTaskState); } 
        set { SetProperty<MDMaintTaskState>(ref _MDMaintTaskState, value); }
    }

    public bool MDMaintTaskState_IsLoaded
    {
        get
        {
            return MDMaintTaskState != null;
        }
    }

    public virtual ReferenceEntry MDMaintTaskStateReference 
    {
        get { return Context.Entry(this).Reference("MDMaintTaskState"); }
    }
    
    private MaintOrder _MaintOrder;
    public virtual MaintOrder MaintOrder
    { 
        get { return LazyLoader.Load(this, ref _MaintOrder); } 
        set { SetProperty<MaintOrder>(ref _MaintOrder, value); }
    }

    public bool MaintOrder_IsLoaded
    {
        get
        {
            return MaintOrder != null;
        }
    }

    public virtual ReferenceEntry MaintOrderReference 
    {
        get { return Context.Entry(this).Reference("MaintOrder"); }
    }
    }
