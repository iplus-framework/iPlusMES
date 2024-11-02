// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class CalendarHoliday : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public CalendarHoliday()
    {
    }

    private CalendarHoliday(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _CalendarHolidayID;
    public Guid CalendarHolidayID 
    {
        get { return _CalendarHolidayID; }
        set { SetProperty<Guid>(ref _CalendarHolidayID, value); }
    }

    Guid _CalendarID;
    public Guid CalendarID 
    {
        get { return _CalendarID; }
        set { SetProperty<Guid>(ref _CalendarID, value); }
    }

    string _HolidayName;
    public string HolidayName 
    {
        get { return _HolidayName; }
        set { SetProperty<string>(ref _HolidayName, value); }
    }

    Guid _MDCountryID;
    public Guid MDCountryID 
    {
        get { return _MDCountryID; }
        set { SetProperty<Guid>(ref _MDCountryID, value); }
    }

    Guid? _MDCountryLandID;
    public Guid? MDCountryLandID 
    {
        get { return _MDCountryLandID; }
        set { SetProperty<Guid?>(ref _MDCountryLandID, value); }
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
    
    private MDCountry _MDCountry;
    public virtual MDCountry MDCountry
    { 
        get { return LazyLoader.Load(this, ref _MDCountry); } 
        set { SetProperty<MDCountry>(ref _MDCountry, value); }
    }

    public bool MDCountry_IsLoaded
    {
        get
        {
            return MDCountry != null;
        }
    }

    public virtual ReferenceEntry MDCountryReference 
    {
        get { return Context.Entry(this).Reference("MDCountry"); }
    }
    
    private MDCountryLand _MDCountryLand;
    public virtual MDCountryLand MDCountryLand
    { 
        get { return LazyLoader.Load(this, ref _MDCountryLand); } 
        set { SetProperty<MDCountryLand>(ref _MDCountryLand, value); }
    }

    public bool MDCountryLand_IsLoaded
    {
        get
        {
            return MDCountryLand != null;
        }
    }

    public virtual ReferenceEntry MDCountryLandReference 
    {
        get { return Context.Entry(this).Reference("MDCountryLand"); }
    }
    }
