// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDCurrencyExchange : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public MDCurrencyExchange()
    {
    }

    private MDCurrencyExchange(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDCurrencyExchangeID;
    public Guid MDCurrencyExchangeID 
    {
        get { return _MDCurrencyExchangeID; }
        set { SetProperty<Guid>(ref _MDCurrencyExchangeID, value); }
    }

    Guid _MDCurrencyID;
    public Guid MDCurrencyID 
    {
        get { return _MDCurrencyID; }
        set { SetProperty<Guid>(ref _MDCurrencyID, value); }
    }

    Guid _ToMDCurrencyID;
    public Guid ToMDCurrencyID 
    {
        get { return _ToMDCurrencyID; }
        set { SetProperty<Guid>(ref _ToMDCurrencyID, value); }
    }

    double _ExchangeRate;
    public double ExchangeRate 
    {
        get { return _ExchangeRate; }
        set { SetProperty<double>(ref _ExchangeRate, value); }
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

    string _ExchangeNo;
    public string ExchangeNo 
    {
        get { return _ExchangeNo; }
        set { SetProperty<string>(ref _ExchangeNo, value); }
    }

    private ICollection<Invoice> _Invoice_MDCurrencyExchange;
    public virtual ICollection<Invoice> Invoice_MDCurrencyExchange
    {
        get { return LazyLoader.Load(this, ref _Invoice_MDCurrencyExchange); }
        set { _Invoice_MDCurrencyExchange = value; }
    }

    public bool Invoice_MDCurrencyExchange_IsLoaded
    {
        get
        {
            return Invoice_MDCurrencyExchange != null;
        }
    }

    public virtual CollectionEntry Invoice_MDCurrencyExchangeReference
    {
        get { return Context.Entry(this).Collection(c => c.Invoice_MDCurrencyExchange); }
    }

    private MDCurrency _MDCurrency;
    public virtual MDCurrency MDCurrency
    { 
        get { return LazyLoader.Load(this, ref _MDCurrency); } 
        set { SetProperty<MDCurrency>(ref _MDCurrency, value); }
    }

    public bool MDCurrency_IsLoaded
    {
        get
        {
            return MDCurrency != null;
        }
    }

    public virtual ReferenceEntry MDCurrencyReference 
    {
        get { return Context.Entry(this).Reference("MDCurrency"); }
    }
    
    private MDCurrency _ToMDCurrency;
    public virtual MDCurrency ToMDCurrency
    { 
        get { return LazyLoader.Load(this, ref _ToMDCurrency); } 
        set { SetProperty<MDCurrency>(ref _ToMDCurrency, value); }
    }

    public bool ToMDCurrency_IsLoaded
    {
        get
        {
            return ToMDCurrency != null;
        }
    }

    public virtual ReferenceEntry ToMDCurrencyReference 
    {
        get { return Context.Entry(this).Reference("ToMDCurrency"); }
    }
    }
