using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACProgramLogTask : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public ACProgramLogTask()
    {
    }

    private ACProgramLogTask(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ACProgramLogTaskID;
    public Guid ACProgramLogTaskID 
    {
        get { return _ACProgramLogTaskID; }
        set { SetProperty<Guid>(ref _ACProgramLogTaskID, value); }
    }

    Guid _ACProgramLogID;
    public Guid ACProgramLogID 
    {
        get { return _ACProgramLogID; }
        set { SetForeignKeyProperty<Guid>(ref _ACProgramLogID, value, "ACProgramLog", _ACProgramLog, ACProgramLog != null ? ACProgramLog.ACProgramLogID : default(Guid)); }
    }

    string _ACClassMethodXAML;
    public string ACClassMethodXAML 
    {
        get { return _ACClassMethodXAML; }
        set { SetProperty<string>(ref _ACClassMethodXAML, value); }
    }

    int _LoopNo;
    public int LoopNo 
    {
        get { return _LoopNo; }
        set { SetProperty<int>(ref _LoopNo, value); }
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

    private ACProgramLog _ACProgramLog;
    public virtual ACProgramLog ACProgramLog
    { 
        get { return LazyLoader.Load(this, ref _ACProgramLog); } 
        set { SetProperty<ACProgramLog>(ref _ACProgramLog, value); }
    }

    public bool ACProgramLog_IsLoaded
    {
        get
        {
            return _ACProgramLog != null;
        }
    }

    public virtual ReferenceEntry ACProgramLogReference 
    {
        get { return Context.Entry(this).Reference("ACProgramLog"); }
    }
    }
