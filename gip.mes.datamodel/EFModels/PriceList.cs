// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class PriceList : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public PriceList()
    {
    }

    private PriceList(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _PriceListID;
    public Guid PriceListID 
    {
        get { return _PriceListID; }
        set { SetProperty<Guid>(ref _PriceListID, value); }
    }

    string _PriceListNo;
    public string PriceListNo 
    {
        get { return _PriceListNo; }
        set { SetProperty<string>(ref _PriceListNo, value); }
    }

    string _PriceListNameTrans;
    public string PriceListNameTrans 
    {
        get { return _PriceListNameTrans; }
        set { SetProperty<string>(ref _PriceListNameTrans, value); }
    }

    Guid _MDCurrencyID;
    public Guid MDCurrencyID 
    {
        get { return _MDCurrencyID; }
        set { SetProperty<Guid>(ref _MDCurrencyID, value); }
    }

    DateTime _DateFrom;
    public DateTime DateFrom 
    {
        get { return _DateFrom; }
        set { SetProperty<DateTime>(ref _DateFrom, value); }
    }

    DateTime? _DateTo;
    public DateTime? DateTo 
    {
        get { return _DateTo; }
        set { SetProperty<DateTime?>(ref _DateTo, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
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
    
    private ICollection<PriceListMaterial> _PriceListMaterial_PriceList;
    public virtual ICollection<PriceListMaterial> PriceListMaterial_PriceList
    {
        get { return LazyLoader.Load(this, ref _PriceListMaterial_PriceList); }
        set { _PriceListMaterial_PriceList = value; }
    }

    public bool PriceListMaterial_PriceList_IsLoaded
    {
        get
        {
            return PriceListMaterial_PriceList != null;
        }
    }

    public virtual CollectionEntry PriceListMaterial_PriceListReference
    {
        get { return Context.Entry(this).Collection(c => c.PriceListMaterial_PriceList); }
    }
}
