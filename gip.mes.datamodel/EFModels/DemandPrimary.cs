using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class DemandPrimary : VBEntityObject, IInsertInfo, IUpdateInfo, ITargetQuantity
{

    public DemandPrimary()
    {
    }

    private DemandPrimary(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _DemandPrimaryID;
    public Guid DemandPrimaryID 
    {
        get { return _DemandPrimaryID; }
        set { SetProperty<Guid>(ref _DemandPrimaryID, value); }
    }

    string _DemandPrimaryNo;
    public string DemandPrimaryNo 
    {
        get { return _DemandPrimaryNo; }
        set { SetProperty<string>(ref _DemandPrimaryNo, value); }
    }

    Guid _MaterialID;
    public Guid MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid>(ref _MaterialID, value); }
    }

    Guid _CalendarID;
    public Guid CalendarID 
    {
        get { return _CalendarID; }
        set { SetProperty<Guid>(ref _CalendarID, value); }
    }

    double _TargetQuantity;
    public double TargetQuantity 
    {
        get { return _TargetQuantity; }
        set { SetProperty<double>(ref _TargetQuantity, value); }
    }

    double _TargetWeight;
    public double TargetWeight 
    {
        get { return _TargetWeight; }
        set { SetProperty<double>(ref _TargetWeight, value); }
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

    private Calendar _Calendar;
    public virtual Calendar Calendar
    { 
        get { return LazyLoader.Load(this, ref _Calendar); } 
        set { SetProperty<Calendar>(ref _Calendar, value); }
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
    
    private Material _Material;
    public virtual Material Material
    { 
        get { return LazyLoader.Load(this, ref _Material); } 
        set { SetProperty<Material>(ref _Material, value); }
    }

    public bool Material_IsLoaded
    {
        get
        {
            return Material != null;
        }
    }

    public virtual ReferenceEntry MaterialReference 
    {
        get { return Context.Entry(this).Reference("Material"); }
    }
    }
