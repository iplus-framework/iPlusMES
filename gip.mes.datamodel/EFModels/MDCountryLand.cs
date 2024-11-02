// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDCountryLand : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDCountryLand()
    {
    }

    private MDCountryLand(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDCountryLandID;
    public Guid MDCountryLandID 
    {
        get { return _MDCountryLandID; }
        set { SetProperty<Guid>(ref _MDCountryLandID, value); }
    }

    Guid _MDCountryID;
    public Guid MDCountryID 
    {
        get { return _MDCountryID; }
        set { SetProperty<Guid>(ref _MDCountryID, value); }
    }

    string _MDNameTrans;
    public string MDNameTrans 
    {
        get { return _MDNameTrans; }
        set { SetProperty<string>(ref _MDNameTrans, value); }
    }

    string _MDCountryLandCode;
    public string MDCountryLandCode 
    {
        get { return _MDCountryLandCode; }
        set { SetProperty<string>(ref _MDCountryLandCode, value); }
    }

    bool _IsDefault;
    public bool IsDefault 
    {
        get { return _IsDefault; }
        set { SetProperty<bool>(ref _IsDefault, value); }
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

    string _MDKey;
    public string MDKey 
    {
        get { return _MDKey; }
        set { SetProperty<string>(ref _MDKey, value); }
    }

    private ICollection<CalendarHoliday> _CalendarHoliday_MDCountryLand;
    public virtual ICollection<CalendarHoliday> CalendarHoliday_MDCountryLand
    {
        get { return LazyLoader.Load(this, ref _CalendarHoliday_MDCountryLand); }
        set { _CalendarHoliday_MDCountryLand = value; }
    }

    public bool CalendarHoliday_MDCountryLand_IsLoaded
    {
        get
        {
            return CalendarHoliday_MDCountryLand != null;
        }
    }

    public virtual CollectionEntry CalendarHoliday_MDCountryLandReference
    {
        get { return Context.Entry(this).Collection(c => c.CalendarHoliday_MDCountryLand); }
    }

    private ICollection<CompanyAddress> _CompanyAddress_MDCountryLand;
    public virtual ICollection<CompanyAddress> CompanyAddress_MDCountryLand
    {
        get { return LazyLoader.Load(this, ref _CompanyAddress_MDCountryLand); }
        set { _CompanyAddress_MDCountryLand = value; }
    }

    public bool CompanyAddress_MDCountryLand_IsLoaded
    {
        get
        {
            return CompanyAddress_MDCountryLand != null;
        }
    }

    public virtual CollectionEntry CompanyAddress_MDCountryLandReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyAddress_MDCountryLand); }
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
    }
