using System;
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

    private ICollection<InvoicePos> _InvoicePo_MDCountrySalesTaxMDMaterialGroup;
    public virtual ICollection<InvoicePos> InvoicePo_MDCountrySalesTaxMDMaterialGroup
    {
        get => LazyLoader.Load(this, ref _InvoicePo_MDCountrySalesTaxMDMaterialGroup);
        set => _InvoicePo_MDCountrySalesTaxMDMaterialGroup = value;
    }

    public bool InvoicePo_MDCountrySalesTaxMDMaterialGroup_IsLoaded
    {
        get
        {
            return InvoicePo_MDCountrySalesTaxMDMaterialGroup != null;
        }
    }

    public virtual CollectionEntry InvoicePo_MDCountrySalesTaxMDMaterialGroupReference
    {
        get { return Context.Entry(this).Collection(c => c.InvoicePo_MDCountrySalesTaxMDMaterialGroup); }
    }

    private MDCountrySalesTax _MDCountrySalesTax;
    public virtual MDCountrySalesTax MDCountrySalesTax
    { 
        get => LazyLoader.Load(this, ref _MDCountrySalesTax);
        set => _MDCountrySalesTax = value;
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
        get => LazyLoader.Load(this, ref _MDMaterialGroup);
        set => _MDMaterialGroup = value;
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
    
    private ICollection<OutOfferPos> _OutOfferPo_MDCountrySalesTaxMDMaterialGroup;
    public virtual ICollection<OutOfferPos> OutOfferPo_MDCountrySalesTaxMDMaterialGroup
    {
        get => LazyLoader.Load(this, ref _OutOfferPo_MDCountrySalesTaxMDMaterialGroup);
        set => _OutOfferPo_MDCountrySalesTaxMDMaterialGroup = value;
    }

    public bool OutOfferPo_MDCountrySalesTaxMDMaterialGroup_IsLoaded
    {
        get
        {
            return OutOfferPo_MDCountrySalesTaxMDMaterialGroup != null;
        }
    }

    public virtual CollectionEntry OutOfferPo_MDCountrySalesTaxMDMaterialGroupReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOfferPo_MDCountrySalesTaxMDMaterialGroup); }
    }

    private ICollection<OutOrderPos> _OutOrderPo_MDCountrySalesTaxMDMaterialGroup;
    public virtual ICollection<OutOrderPos> OutOrderPo_MDCountrySalesTaxMDMaterialGroup
    {
        get => LazyLoader.Load(this, ref _OutOrderPo_MDCountrySalesTaxMDMaterialGroup);
        set => _OutOrderPo_MDCountrySalesTaxMDMaterialGroup = value;
    }

    public bool OutOrderPo_MDCountrySalesTaxMDMaterialGroup_IsLoaded
    {
        get
        {
            return OutOrderPo_MDCountrySalesTaxMDMaterialGroup != null;
        }
    }

    public virtual CollectionEntry OutOrderPo_MDCountrySalesTaxMDMaterialGroupReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPo_MDCountrySalesTaxMDMaterialGroup); }
    }
}
