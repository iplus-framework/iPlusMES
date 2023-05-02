using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class CalendarShiftPerson : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public CalendarShiftPerson()
    {
    }

    private CalendarShiftPerson(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _CalendarShiftPersonID;
    public Guid CalendarShiftPersonID 
    {
        get { return _CalendarShiftPersonID; }
        set { SetProperty<Guid>(ref _CalendarShiftPersonID, value); }
    }

    Guid _CalendarShiftID;
    public Guid CalendarShiftID 
    {
        get { return _CalendarShiftID; }
        set { SetProperty<Guid>(ref _CalendarShiftID, value); }
    }

    Guid _CompanyPersonID;
    public Guid CompanyPersonID 
    {
        get { return _CompanyPersonID; }
        set { SetProperty<Guid>(ref _CompanyPersonID, value); }
    }

    int? _Percentage;
    public int? Percentage 
    {
        get { return _Percentage; }
        set { SetProperty<int?>(ref _Percentage, value); }
    }

    TimeSpan? _TimeFrom;
    public TimeSpan? TimeFrom 
    {
        get { return _TimeFrom; }
        set { SetProperty<TimeSpan?>(ref _TimeFrom, value); }
    }

    TimeSpan? _TimeTo;
    public TimeSpan? TimeTo 
    {
        get { return _TimeTo; }
        set { SetProperty<TimeSpan?>(ref _TimeTo, value); }
    }

    short? _ShiftStateIndex;
    public short? ShiftStateIndex 
    {
        get { return _ShiftStateIndex; }
        set { SetProperty<short?>(ref _ShiftStateIndex, value); }
    }

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
    }

    DateTime _UpdateDate;
    public DateTime UpdateDate 
    {
        get { return _UpdateDate; }
        set { SetProperty<DateTime>(ref _UpdateDate, value); }
    }

    string _UpdateName;
    public string UpdateName 
    {
        get { return _UpdateName; }
        set { SetProperty<string>(ref _UpdateName, value); }
    }

    DateTime _InsertDate;
    public DateTime InsertDate 
    {
        get { return _InsertDate; }
        set { SetProperty<DateTime>(ref _InsertDate, value); }
    }

    string _InsertName;
    public string InsertName 
    {
        get { return _InsertName; }
        set { SetProperty<string>(ref _InsertName, value); }
    }

    private CalendarShift _CalendarShift;
    public virtual CalendarShift CalendarShift
    { 
        get => LazyLoader.Load(this, ref _CalendarShift);
        set => _CalendarShift = value;
    }

    public bool CalendarShift_IsLoaded
    {
        get
        {
            return CalendarShift != null;
        }
    }

    public virtual ReferenceEntry CalendarShiftReference 
    {
        get { return Context.Entry(this).Reference("CalendarShift"); }
    }
    
    private CompanyPerson _CompanyPerson;
    public virtual CompanyPerson CompanyPerson
    { 
        get => LazyLoader.Load(this, ref _CompanyPerson);
        set => _CompanyPerson = value;
    }

    public bool CompanyPerson_IsLoaded
    {
        get
        {
            return CompanyPerson != null;
        }
    }

    public virtual ReferenceEntry CompanyPersonReference 
    {
        get { return Context.Entry(this).Reference("CompanyPerson"); }
    }
    }
