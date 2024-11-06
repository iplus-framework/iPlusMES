using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACProgramLog : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public ACProgramLog()
    {
    }

    private ACProgramLog(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ACProgramLogID;
    public Guid ACProgramLogID 
    {
        get { return _ACProgramLogID; }
        set { SetProperty<Guid>(ref _ACProgramLogID, value); }
    }

    Guid _ACProgramID;
    public Guid ACProgramID 
    {
        get { return _ACProgramID; }
        set { SetProperty<Guid>(ref _ACProgramID, value); }
    }

    Guid? _ParentACProgramLogID;
    public Guid? ParentACProgramLogID 
    {
        get { return _ParentACProgramLogID; }
        set { SetProperty<Guid?>(ref _ParentACProgramLogID, value); }
    }

    string _ACUrl;
    public string ACUrl 
    {
        get { return _ACUrl; }
        set { SetProperty<string>(ref _ACUrl, value); }
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

    double _DurationSec;
    public double DurationSec 
    {
        get { return _DurationSec; }
        set { SetProperty<double>(ref _DurationSec, value); }
    }

    DateTime _StartDatePlan;
    public DateTime StartDatePlan 
    {
        get { return _StartDatePlan; }
        set { SetProperty<DateTime>(ref _StartDatePlan, value); }
    }

    DateTime _EndDatePlan;
    public DateTime EndDatePlan 
    {
        get { return _EndDatePlan; }
        set { SetProperty<DateTime>(ref _EndDatePlan, value); }
    }

    double _DurationSecPlan;
    public double DurationSecPlan 
    {
        get { return _DurationSecPlan; }
        set { SetProperty<double>(ref _DurationSecPlan, value); }
    }

    string _Message;
    public string Message 
    {
        get { return _Message; }
        set { SetProperty<string>(ref _Message, value); }
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

    Guid? _ACClassID;
    public Guid? ACClassID 
    {
        get { return _ACClassID; }
        set { SetProperty<Guid?>(ref _ACClassID, value); }
    }

    Guid? _RefACClassID;
    public Guid? RefACClassID 
    {
        get { return _RefACClassID; }
        set { SetProperty<Guid?>(ref _RefACClassID, value); }
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
            return ACProgram != null;
        }
    }

    public virtual ReferenceEntry ACProgramReference 
    {
        get { return Context.Entry(this).Reference("ACProgram"); }
    }
    
    private ICollection<ACProgramLogTask> _ACProgramLogTask_ACProgramLog;
    public virtual ICollection<ACProgramLogTask> ACProgramLogTask_ACProgramLog
    {
        get { return LazyLoader.Load(this, ref _ACProgramLogTask_ACProgramLog); }
        set { _ACProgramLogTask_ACProgramLog = value; }
    }

    public bool ACProgramLogTask_ACProgramLog_IsLoaded
    {
        get
        {
            return ACProgramLogTask_ACProgramLog != null;
        }
    }

    public virtual CollectionEntry ACProgramLogTask_ACProgramLogReference
    {
        get { return Context.Entry(this).Collection(c => c.ACProgramLogTask_ACProgramLog); }
    }

    private ICollection<ACProgramLog> _ACProgramLog_ParentACProgramLog;
    public virtual ICollection<ACProgramLog> ACProgramLog_ParentACProgramLog
    {
        get { return LazyLoader.Load(this, ref _ACProgramLog_ParentACProgramLog); }
        set { _ACProgramLog_ParentACProgramLog = value; }
    }

    public bool ACProgramLog_ParentACProgramLog_IsLoaded
    {
        get
        {
            return ACProgramLog_ParentACProgramLog != null;
        }
    }

    public virtual CollectionEntry ACProgramLog_ParentACProgramLogReference
    {
        get { return Context.Entry(this).Collection(c => c.ACProgramLog_ParentACProgramLog); }
    }

    private ICollection<MsgAlarmLog> _MsgAlarmLog_ACProgramLog;
    public virtual ICollection<MsgAlarmLog> MsgAlarmLog_ACProgramLog
    {
        get { return LazyLoader.Load(this, ref _MsgAlarmLog_ACProgramLog); }
        set { _MsgAlarmLog_ACProgramLog = value; }
    }

    public bool MsgAlarmLog_ACProgramLog_IsLoaded
    {
        get
        {
            return MsgAlarmLog_ACProgramLog != null;
        }
    }

    public virtual CollectionEntry MsgAlarmLog_ACProgramLogReference
    {
        get { return Context.Entry(this).Collection(c => c.MsgAlarmLog_ACProgramLog); }
    }

    private ICollection<OperationLog> _OperationLog_ACProgramLog;
    public virtual ICollection<OperationLog> OperationLog_ACProgramLog
    {
        get { return LazyLoader.Load(this, ref _OperationLog_ACProgramLog); }
        set { _OperationLog_ACProgramLog = value; }
    }

    public bool OperationLog_ACProgramLog_IsLoaded
    {
        get
        {
            return OperationLog_ACProgramLog != null;
        }
    }

    public virtual CollectionEntry OperationLog_ACProgramLogReference
    {
        get { return Context.Entry(this).Collection(c => c.OperationLog_ACProgramLog); }
    }

    private OrderLog _OrderLog_VBiACProgramLog;
    public virtual OrderLog OrderLog_VBiACProgramLog
    { 
        get { return LazyLoader.Load(this, ref _OrderLog_VBiACProgramLog); } 
        set { SetProperty<OrderLog>(ref _OrderLog_VBiACProgramLog, value); }
    }

    public bool OrderLog_VBiACProgramLog_IsLoaded
    {
        get
        {
            return OrderLog_VBiACProgramLog != null;
        }
    }

    public virtual ReferenceEntry OrderLog_VBiACProgramLogReference 
    {
        get { return Context.Entry(this).Reference("OrderLog_VBiACProgramLog"); }
    }
    
    private ACProgramLog _ACProgramLog1_ParentACProgramLog;
    public virtual ACProgramLog ACProgramLog1_ParentACProgramLog
    { 
        get { return LazyLoader.Load(this, ref _ACProgramLog1_ParentACProgramLog); } 
        set { SetProperty<ACProgramLog>(ref _ACProgramLog1_ParentACProgramLog, value); }
    }

    public bool ACProgramLog1_ParentACProgramLog_IsLoaded
    {
        get
        {
            return ACProgramLog1_ParentACProgramLog != null;
        }
    }

    public virtual ReferenceEntry ACProgramLog1_ParentACProgramLogReference 
    {
        get { return Context.Entry(this).Reference("ACProgramLog1_ParentACProgramLog"); }
    }
    }
