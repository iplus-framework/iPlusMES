using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class CalendarShift : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public CalendarShift()
    {
    }

    private CalendarShift(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _CalendarShiftID;
    public Guid CalendarShiftID 
    {
        get { return _CalendarShiftID; }
        set { SetProperty<Guid>(ref _CalendarShiftID, value); }
    }

    Guid _CalendarID;
    public Guid CalendarID 
    {
        get { return _CalendarID; }
        set { SetProperty<Guid>(ref _CalendarID, value); }
    }

    Guid _VBiACProjectID;
    public Guid VBiACProjectID 
    {
        get { return _VBiACProjectID; }
        set { SetProperty<Guid>(ref _VBiACProjectID, value); }
    }

    Guid _MDTimeRangeID;
    public Guid MDTimeRangeID 
    {
        get { return _MDTimeRangeID; }
        set { SetProperty<Guid>(ref _MDTimeRangeID, value); }
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

    private Calendar _Calendar;
    public virtual Calendar Calendar
    { 
        get => LazyLoader.Load(this, ref _Calendar);
        set => _Calendar = value;
    }

    public bool Calendar_IsLoaded
    {
        get
        {
            return Calendar != null;
        }
    }

    public virtual ReferenceEntry CalendarReference 
    {
        get { return Context.Entry(this).Reference("Calendar"); }
    }
    
    private ICollection<CalendarShiftPerson> _CalendarShiftPerson_CalendarShift;
    public virtual ICollection<CalendarShiftPerson> CalendarShiftPerson_CalendarShift
    {
        get => LazyLoader.Load(this, ref _CalendarShiftPerson_CalendarShift);
        set => _CalendarShiftPerson_CalendarShift = value;
    }

    public bool CalendarShiftPerson_CalendarShift_IsLoaded
    {
        get
        {
            return CalendarShiftPerson_CalendarShift != null;
        }
    }

    public virtual CollectionEntry CalendarShiftPerson_CalendarShiftReference
    {
        get { return Context.Entry(this).Collection(c => c.CalendarShiftPerson_CalendarShift); }
    }

    private MDTimeRange _MDTimeRange;
    public virtual MDTimeRange MDTimeRange
    { 
        get => LazyLoader.Load(this, ref _MDTimeRange);
        set => _MDTimeRange = value;
    }

    public bool MDTimeRange_IsLoaded
    {
        get
        {
            return MDTimeRange != null;
        }
    }

    public virtual ReferenceEntry MDTimeRangeReference 
    {
        get { return Context.Entry(this).Reference("MDTimeRange"); }
    }
    
    private ACProject _VBiACProject;
    public virtual ACProject VBiACProject
    { 
        get => LazyLoader.Load(this, ref _VBiACProject);
        set => _VBiACProject = value;
    }

    public bool VBiACProject_IsLoaded
    {
        get
        {
            return VBiACProject != null;
        }
    }

    public virtual ReferenceEntry VBiACProjectReference 
    {
        get { return Context.Entry(this).Reference("VBiACProject"); }
    }
    }
