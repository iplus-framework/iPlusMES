// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDCurrency : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDCurrency()
    {
    }

    private MDCurrency(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDCurrencyID;
    public Guid MDCurrencyID 
    {
        get { return _MDCurrencyID; }
        set { SetProperty<Guid>(ref _MDCurrencyID, value); }
    }

    string _MDCurrencyShortname;
    public string MDCurrencyShortname 
    {
        get { return _MDCurrencyShortname; }
        set { SetProperty<string>(ref _MDCurrencyShortname, value); }
    }

    string _MDNameTrans;
    public string MDNameTrans 
    {
        get { return _MDNameTrans; }
        set { SetProperty<string>(ref _MDNameTrans, value); }
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

    string _MDKey;
    public string MDKey 
    {
        get { return _MDKey; }
        set { SetProperty<string>(ref _MDKey, value); }
    }

    private ICollection<Company> _Company_MDCurrency;
    public virtual ICollection<Company> Company_MDCurrency
    {
        get { return LazyLoader.Load(this, ref _Company_MDCurrency); }
        set { _Company_MDCurrency = value; }
    }

    public bool Company_MDCurrency_IsLoaded
    {
        get
        {
            return Company_MDCurrency != null;
        }
    }

    public virtual CollectionEntry Company_MDCurrencyReference
    {
        get { return Context.Entry(this).Collection(c => c.Company_MDCurrency); }
    }

    private ICollection<InOrder> _InOrder_MDCurrency;
    public virtual ICollection<InOrder> InOrder_MDCurrency
    {
        get { return LazyLoader.Load(this, ref _InOrder_MDCurrency); }
        set { _InOrder_MDCurrency = value; }
    }

    public bool InOrder_MDCurrency_IsLoaded
    {
        get
        {
            return InOrder_MDCurrency != null;
        }
    }

    public virtual CollectionEntry InOrder_MDCurrencyReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrder_MDCurrency); }
    }

    private ICollection<Invoice> _Invoice_MDCurrency;
    public virtual ICollection<Invoice> Invoice_MDCurrency
    {
        get { return LazyLoader.Load(this, ref _Invoice_MDCurrency); }
        set { _Invoice_MDCurrency = value; }
    }

    public bool Invoice_MDCurrency_IsLoaded
    {
        get
        {
            return Invoice_MDCurrency != null;
        }
    }

    public virtual CollectionEntry Invoice_MDCurrencyReference
    {
        get { return Context.Entry(this).Collection(c => c.Invoice_MDCurrency); }
    }

    private ICollection<MDCountry> _MDCountry_MDCurrency;
    public virtual ICollection<MDCountry> MDCountry_MDCurrency
    {
        get { return LazyLoader.Load(this, ref _MDCountry_MDCurrency); }
        set { _MDCountry_MDCurrency = value; }
    }

    public bool MDCountry_MDCurrency_IsLoaded
    {
        get
        {
            return MDCountry_MDCurrency != null;
        }
    }

    public virtual CollectionEntry MDCountry_MDCurrencyReference
    {
        get { return Context.Entry(this).Collection(c => c.MDCountry_MDCurrency); }
    }

    private ICollection<MDCurrencyExchange> _MDCurrencyExchange_MDCurrency;
    public virtual ICollection<MDCurrencyExchange> MDCurrencyExchange_MDCurrency
    {
        get { return LazyLoader.Load(this, ref _MDCurrencyExchange_MDCurrency); }
        set { _MDCurrencyExchange_MDCurrency = value; }
    }

    public bool MDCurrencyExchange_MDCurrency_IsLoaded
    {
        get
        {
            return MDCurrencyExchange_MDCurrency != null;
        }
    }

    public virtual CollectionEntry MDCurrencyExchange_MDCurrencyReference
    {
        get { return Context.Entry(this).Collection(c => c.MDCurrencyExchange_MDCurrency); }
    }

    private ICollection<MDCurrencyExchange> _MDCurrencyExchange_ToMDCurrency;
    public virtual ICollection<MDCurrencyExchange> MDCurrencyExchange_ToMDCurrency
    {
        get { return LazyLoader.Load(this, ref _MDCurrencyExchange_ToMDCurrency); }
        set { _MDCurrencyExchange_ToMDCurrency = value; }
    }

    public bool MDCurrencyExchange_ToMDCurrency_IsLoaded
    {
        get
        {
            return MDCurrencyExchange_ToMDCurrency != null;
        }
    }

    public virtual CollectionEntry MDCurrencyExchange_ToMDCurrencyReference
    {
        get { return Context.Entry(this).Collection(c => c.MDCurrencyExchange_ToMDCurrency); }
    }

    private ICollection<OutOffer> _OutOffer_MDCurrency;
    public virtual ICollection<OutOffer> OutOffer_MDCurrency
    {
        get { return LazyLoader.Load(this, ref _OutOffer_MDCurrency); }
        set { _OutOffer_MDCurrency = value; }
    }

    public bool OutOffer_MDCurrency_IsLoaded
    {
        get
        {
            return OutOffer_MDCurrency != null;
        }
    }

    public virtual CollectionEntry OutOffer_MDCurrencyReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOffer_MDCurrency); }
    }

    private ICollection<OutOrder> _OutOrder_MDCurrency;
    public virtual ICollection<OutOrder> OutOrder_MDCurrency
    {
        get { return LazyLoader.Load(this, ref _OutOrder_MDCurrency); }
        set { _OutOrder_MDCurrency = value; }
    }

    public bool OutOrder_MDCurrency_IsLoaded
    {
        get
        {
            return OutOrder_MDCurrency != null;
        }
    }

    public virtual CollectionEntry OutOrder_MDCurrencyReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrder_MDCurrency); }
    }

    private ICollection<PriceList> _PriceList_MDCurrency;
    public virtual ICollection<PriceList> PriceList_MDCurrency
    {
        get { return LazyLoader.Load(this, ref _PriceList_MDCurrency); }
        set { _PriceList_MDCurrency = value; }
    }

    public bool PriceList_MDCurrency_IsLoaded
    {
        get
        {
            return PriceList_MDCurrency != null;
        }
    }

    public virtual CollectionEntry PriceList_MDCurrencyReference
    {
        get { return Context.Entry(this).Collection(c => c.PriceList_MDCurrency); }
    }
}
