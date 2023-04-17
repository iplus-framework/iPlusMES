using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDCountrySalesTaxMaterial : VBEntityObject 
{

    public MDCountrySalesTaxMaterial()
    {
    }

    private MDCountrySalesTaxMaterial(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _MDCountrySalesTaxMaterialID;
    public Guid MDCountrySalesTaxMaterialID 
    {
        get { return _MDCountrySalesTaxMaterialID; }
        set { SetProperty<Guid>(ref _MDCountrySalesTaxMaterialID, value); }
    }

    Guid _MDCountrySalesTaxID;
    public Guid MDCountrySalesTaxID 
    {
        get { return _MDCountrySalesTaxID; }
        set { SetProperty<Guid>(ref _MDCountrySalesTaxID, value); }
    }

    Guid _MaterialID;
    public Guid MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid>(ref _MaterialID, value); }
    }

    decimal _SalesTax;
    public decimal SalesTax 
    {
        get { return _SalesTax; }
        set { SetProperty<decimal>(ref _SalesTax, value); }
    }

    private ICollection<InvoicePos> _InvoicePo_MDCountrySalesTaxMaterial;
    public virtual ICollection<InvoicePos> InvoicePo_MDCountrySalesTaxMaterial
    {
        get => LazyLoader.Load(this, ref _InvoicePo_MDCountrySalesTaxMaterial);
        set => _InvoicePo_MDCountrySalesTaxMaterial = value;
    }

    public bool InvoicePo_MDCountrySalesTaxMaterial_IsLoaded
    {
        get
        {
            return InvoicePo_MDCountrySalesTaxMaterial != null;
        }
    }

    public virtual CollectionEntry InvoicePo_MDCountrySalesTaxMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.InvoicePo_MDCountrySalesTaxMaterial); }
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
    
    private Material _Material;
    public virtual Material Material
    { 
        get => LazyLoader.Load(this, ref _Material);
        set => _Material = value;
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
    
    private ICollection<OutOfferPos> _OutOfferPo_MDCountrySalesTaxMaterial;
    public virtual ICollection<OutOfferPos> OutOfferPo_MDCountrySalesTaxMaterial
    {
        get => LazyLoader.Load(this, ref _OutOfferPo_MDCountrySalesTaxMaterial);
        set => _OutOfferPo_MDCountrySalesTaxMaterial = value;
    }

    public bool OutOfferPo_MDCountrySalesTaxMaterial_IsLoaded
    {
        get
        {
            return OutOfferPo_MDCountrySalesTaxMaterial != null;
        }
    }

    public virtual CollectionEntry OutOfferPo_MDCountrySalesTaxMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOfferPo_MDCountrySalesTaxMaterial); }
    }

    private ICollection<OutOrderPos> _OutOrderPo_MDCountrySalesTaxMaterial;
    public virtual ICollection<OutOrderPos> OutOrderPo_MDCountrySalesTaxMaterial
    {
        get => LazyLoader.Load(this, ref _OutOrderPo_MDCountrySalesTaxMaterial);
        set => _OutOrderPo_MDCountrySalesTaxMaterial = value;
    }

    public bool OutOrderPo_MDCountrySalesTaxMaterial_IsLoaded
    {
        get
        {
            return OutOrderPo_MDCountrySalesTaxMaterial != null;
        }
    }

    public virtual CollectionEntry OutOrderPo_MDCountrySalesTaxMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPo_MDCountrySalesTaxMaterial); }
    }
}
