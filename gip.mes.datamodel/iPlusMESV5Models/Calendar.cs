using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class Calendar : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public Calendar()
    {
    }

    private Calendar(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _CalendarID;
    public Guid CalendarID 
    {
        get { return _CalendarID; }
        set { SetProperty<Guid>(ref _CalendarID, value); }
    }

    DateTime _CalendarDate;
    public DateTime CalendarDate 
    {
        get { return _CalendarDate; }
        set { SetProperty<DateTime>(ref _CalendarDate, value); }
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

    private ICollection<CalendarHoliday> _CalendarHoliday_Calendar;
    public virtual ICollection<CalendarHoliday> CalendarHoliday_Calendar
    {
        get { return LazyLoader.Load(this, ref _CalendarHoliday_Calendar); }
        set { _CalendarHoliday_Calendar = value; }
    }

    public bool CalendarHoliday_Calendar_IsLoaded
    {
        get
        {
            return CalendarHoliday_Calendar != null;
        }
    }

    public virtual CollectionEntry CalendarHoliday_CalendarReference
    {
        get { return Context.Entry(this).Collection(c => c.CalendarHoliday_Calendar); }
    }

    private ICollection<CalendarShift> _CalendarShift_Calendar;
    public virtual ICollection<CalendarShift> CalendarShift_Calendar
    {
        get { return LazyLoader.Load(this, ref _CalendarShift_Calendar); }
        set { _CalendarShift_Calendar = value; }
    }

    public bool CalendarShift_Calendar_IsLoaded
    {
        get
        {
            return CalendarShift_Calendar != null;
        }
    }

    public virtual CollectionEntry CalendarShift_CalendarReference
    {
        get { return Context.Entry(this).Collection(c => c.CalendarShift_Calendar); }
    }

    private ICollection<DemandPrimary> _DemandPrimary_Calendar;
    public virtual ICollection<DemandPrimary> DemandPrimary_Calendar
    {
        get { return LazyLoader.Load(this, ref _DemandPrimary_Calendar); }
        set { _DemandPrimary_Calendar = value; }
    }

    public bool DemandPrimary_Calendar_IsLoaded
    {
        get
        {
            return DemandPrimary_Calendar != null;
        }
    }

    public virtual CollectionEntry DemandPrimary_CalendarReference
    {
        get { return Context.Entry(this).Collection(c => c.DemandPrimary_Calendar); }
    }
}
