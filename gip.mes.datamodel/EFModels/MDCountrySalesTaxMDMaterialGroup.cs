// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDCountrySalesTaxMDMaterialGroup : VBEntityObject
{

    public MDCountrySalesTaxMDMaterialGroup()
    {
    }

    private MDCountrySalesTaxMDMaterialGroup(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDCountrySalesTaxMDMaterialGroupID;
    public Guid MDCountrySalesTaxMDMaterialGroupID 
    {
        get { return _MDCountrySalesTaxMDMaterialGroupID; }
        set { SetProperty<Guid>(ref _MDCountrySalesTaxMDMaterialGroupID, value); }
    }

    Guid _MDCountrySalesTaxID;
    public Guid MDCountrySalesTaxID 
    {
        get { return _MDCountrySalesTaxID; }
        set { SetProperty<Guid>(ref _MDCountrySalesTaxID, value); }
    }

    Guid _MDMaterialGroupID;
    public Guid MDMaterialGroupID 
    {
        get { return _MDMaterialGroupID; }
        set { SetProperty<Guid>(ref _MDMaterialGroupID, value); }
    }

    decimal _SalesTax;
    public decimal SalesTax 
    {
        get { return _SalesTax; }
        set { SetProperty<decimal>(ref _SalesTax, value); }
    }

    private ICollection<InvoicePos> _InvoicePos_MDCountrySalesTaxMDMaterialGroup;
    public virtual ICollection<InvoicePos> InvoicePos_MDCountrySalesTaxMDMaterialGroup
    {
        get { return LazyLoader.Load(this, ref _InvoicePos_MDCountrySalesTaxMDMaterialGroup); }
        set { _InvoicePos_MDCountrySalesTaxMDMaterialGroup = value; }
    }

    public bool InvoicePos_MDCountrySalesTaxMDMaterialGroup_IsLoaded
    {
        get
        {
            return InvoicePos_MDCountrySalesTaxMDMaterialGroup != null;
        }
    }

    public virtual CollectionEntry InvoicePos_MDCountrySalesTaxMDMaterialGroupReference
    {
        get { return Context.Entry(this).Collection(c => c.InvoicePos_MDCountrySalesTaxMDMaterialGroup); }
    }

    private MDCountrySalesTax _MDCountrySalesTax;
    public virtual MDCountrySalesTax MDCountrySalesTax
    { 
        get { return LazyLoader.Load(this, ref _MDCountrySalesTax); } 
        set { SetProperty<MDCountrySalesTax>(ref _MDCountrySalesTax, value); }
    }

    public bool MDCountrySalesTax_IsLoaded
    {
        get
        {
            return MDCountrySalesTax != null;
        }
    }

    public virtual ReferenceEntry MDCountrySalesTaxReference 
    {
        get { return Context.Entry(this).Reference("MDCountrySalesTax"); }
    }
    
    private MDMaterialGroup _MDMaterialGroup;
    public virtual MDMaterialGroup MDMaterialGroup
    { 
        get { return LazyLoader.Load(this, ref _MDMaterialGroup); } 
        set { SetProperty<MDMaterialGroup>(ref _MDMaterialGroup, value); }
    }

    public bool MDMaterialGroup_IsLoaded
    {
        get
        {
            return MDMaterialGroup != null;
        }
    }

    public virtual ReferenceEntry MDMaterialGroupReference 
    {
        get { return Context.Entry(this).Reference("MDMaterialGroup"); }
    }
    
    private ICollection<OutOfferPos> _OutOfferPos_MDCountrySalesTaxMDMaterialGroup;
    public virtual ICollection<OutOfferPos> OutOfferPos_MDCountrySalesTaxMDMaterialGroup
    {
        get { return LazyLoader.Load(this, ref _OutOfferPos_MDCountrySalesTaxMDMaterialGroup); }
        set { _OutOfferPos_MDCountrySalesTaxMDMaterialGroup = value; }
    }

    public bool OutOfferPos_MDCountrySalesTaxMDMaterialGroup_IsLoaded
    {
        get
        {
            return OutOfferPos_MDCountrySalesTaxMDMaterialGroup != null;
        }
    }

    public virtual CollectionEntry OutOfferPos_MDCountrySalesTaxMDMaterialGroupReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOfferPos_MDCountrySalesTaxMDMaterialGroup); }
    }

    private ICollection<OutOrderPos> _OutOrderPos_MDCountrySalesTaxMDMaterialGroup;
    public virtual ICollection<OutOrderPos> OutOrderPos_MDCountrySalesTaxMDMaterialGroup
    {
        get { return LazyLoader.Load(this, ref _OutOrderPos_MDCountrySalesTaxMDMaterialGroup); }
        set { _OutOrderPos_MDCountrySalesTaxMDMaterialGroup = value; }
    }

    public bool OutOrderPos_MDCountrySalesTaxMDMaterialGroup_IsLoaded
    {
        get
        {
            return OutOrderPos_MDCountrySalesTaxMDMaterialGroup != null;
        }
    }

    public virtual CollectionEntry OutOrderPos_MDCountrySalesTaxMDMaterialGroupReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPos_MDCountrySalesTaxMDMaterialGroup); }
    }
}
