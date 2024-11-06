using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class OperationLog : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public OperationLog()
    {
    }

    private OperationLog(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _OperationLogID;
    public Guid OperationLogID 
    {
        get { return _OperationLogID; }
        set { SetProperty<Guid>(ref _OperationLogID, value); }
    }

    Guid? _RefACClassID;
    public Guid? RefACClassID 
    {
        get { return _RefACClassID; }
        set { SetProperty<Guid?>(ref _RefACClassID, value); }
    }

    Guid? _ACProgramLogID;
    public Guid? ACProgramLogID 
    {
        get { return _ACProgramLogID; }
        set { SetProperty<Guid?>(ref _ACProgramLogID, value); }
    }

    Guid? _FacilityChargeID;
    public Guid? FacilityChargeID 
    {
        get { return _FacilityChargeID; }
        set { SetProperty<Guid?>(ref _FacilityChargeID, value); }
    }

    DateTime _OperationTime;
    public DateTime OperationTime 
    {
        get { return _OperationTime; }
        set { SetProperty<DateTime>(ref _OperationTime, value); }
    }

    short _Operation;
    public short Operation 
    {
        get { return _Operation; }
        set { SetProperty<short>(ref _Operation, value); }
    }

    short _OperationState;
    public short OperationState 
    {
        get { return _OperationState; }
        set { SetProperty<short>(ref _OperationState, value); }
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

    string _XMLValue;
    public string XMLValue 
    {
        get { return _XMLValue; }
        set { SetProperty<string>(ref _XMLValue, value); }
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
            return ACProgramLog != null;
        }
    }

    public virtual ReferenceEntry ACProgramLogReference 
    {
        get { return Context.Entry(this).Reference("ACProgramLog"); }
    }
    
    private FacilityCharge _FacilityCharge;
    public virtual FacilityCharge FacilityCharge
    { 
        get { return LazyLoader.Load(this, ref _FacilityCharge); } 
        set { SetProperty<FacilityCharge>(ref _FacilityCharge, value); }
    }

    public bool FacilityCharge_IsLoaded
    {
        get
        {
            return FacilityCharge != null;
        }
    }

    public virtual ReferenceEntry FacilityChargeReference 
    {
        get { return Context.Entry(this).Reference("FacilityCharge"); }
    }
    
    private ACClass _RefACClass;
    public virtual ACClass RefACClass
    { 
        get { return LazyLoader.Load(this, ref _RefACClass); } 
        set { SetProperty<ACClass>(ref _RefACClass, value); }
    }

    public bool RefACClass_IsLoaded
    {
        get
        {
            return RefACClass != null;
        }
    }

    public virtual ReferenceEntry RefACClassReference 
    {
        get { return Context.Entry(this).Reference("RefACClass"); }
    }
    }
