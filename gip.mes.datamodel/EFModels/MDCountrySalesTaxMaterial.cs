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
        set { SetForeignKeyProperty<Guid>(ref _MDCountrySalesTaxID, value, "MDCountrySalesTax", _MDCountrySalesTax, MDCountrySalesTax != null ? MDCountrySalesTax.MDCountrySalesTaxID : default(Guid)); }
    }

    Guid _MaterialID;
    public Guid MaterialID 
    {
        get { return _MaterialID; }
        set { SetForeignKeyProperty<Guid>(ref _MaterialID, value, "Material", _Material, Material != null ? Material.MaterialID : default(Guid)); }
    }

    decimal _SalesTax;
    public decimal SalesTax 
    {
        get { return _SalesTax; }
        set { SetProperty<decimal>(ref _SalesTax, value); }
    }

    private ICollection<InvoicePos> _InvoicePos_MDCountrySalesTaxMaterial;
    public virtual ICollection<InvoicePos> InvoicePos_MDCountrySalesTaxMaterial
    {
        get { return LazyLoader.Load(this, ref _InvoicePos_MDCountrySalesTaxMaterial); }
        set { SetProperty<ICollection<InvoicePos>>(ref _InvoicePos_MDCountrySalesTaxMaterial, value); }
    }

    public bool InvoicePos_MDCountrySalesTaxMaterial_IsLoaded
    {
        get
        {
            return _InvoicePos_MDCountrySalesTaxMaterial != null;
        }
    }

    public virtual CollectionEntry InvoicePos_MDCountrySalesTaxMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.InvoicePos_MDCountrySalesTaxMaterial); }
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
            return _MDCountrySalesTax != null;
        }
    }

    public virtual ReferenceEntry MDCountrySalesTaxReference 
    {
        get { return Context.Entry(this).Reference("MDCountrySalesTax"); }
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
            return _Material != null;
        }
    }

    public virtual ReferenceEntry MaterialReference 
    {
        get { return Context.Entry(this).Reference("Material"); }
    }
    
    private ICollection<OutOfferPos> _OutOfferPos_MDCountrySalesTaxMaterial;
    public virtual ICollection<OutOfferPos> OutOfferPos_MDCountrySalesTaxMaterial
    {
        get { return LazyLoader.Load(this, ref _OutOfferPos_MDCountrySalesTaxMaterial); }
        set { SetProperty<ICollection<OutOfferPos>>(ref _OutOfferPos_MDCountrySalesTaxMaterial, value); }
    }

    public bool OutOfferPos_MDCountrySalesTaxMaterial_IsLoaded
    {
        get
        {
            return _OutOfferPos_MDCountrySalesTaxMaterial != null;
        }
    }

    public virtual CollectionEntry OutOfferPos_MDCountrySalesTaxMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOfferPos_MDCountrySalesTaxMaterial); }
    }

    private ICollection<OutOrderPos> _OutOrderPos_MDCountrySalesTaxMaterial;
    public virtual ICollection<OutOrderPos> OutOrderPos_MDCountrySalesTaxMaterial
    {
        get { return LazyLoader.Load(this, ref _OutOrderPos_MDCountrySalesTaxMaterial); }
        set { SetProperty<ICollection<OutOrderPos>>(ref _OutOrderPos_MDCountrySalesTaxMaterial, value); }
    }

    public bool OutOrderPos_MDCountrySalesTaxMaterial_IsLoaded
    {
        get
        {
            return _OutOrderPos_MDCountrySalesTaxMaterial != null;
        }
    }

    public virtual CollectionEntry OutOrderPos_MDCountrySalesTaxMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPos_MDCountrySalesTaxMaterial); }
    }
}
