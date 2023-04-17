using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDCountrySalesTax : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public MDCountrySalesTax()
    {
    }

    private MDCountrySalesTax(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _MDCountrySalesTaxID;
    public Guid MDCountrySalesTaxID 
    {
        get { return _MDCountrySalesTaxID; }
        set { SetProperty<Guid>(ref _MDCountrySalesTaxID, value); }
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

    decimal _SalesTax;
    public decimal SalesTax 
    {
        get { return _SalesTax; }
        set { SetProperty<decimal>(ref _SalesTax, value); }
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

    private ICollection<InOrderPos> _InOrderPo_MDCountrySalesTax;
    public virtual ICollection<InOrderPos> InOrderPo_MDCountrySalesTax
    {
        get => LazyLoader.Load(this, ref _InOrderPo_MDCountrySalesTax);
        set => _InOrderPo_MDCountrySalesTax = value;
    }

    public bool InOrderPo_MDCountrySalesTax_IsLoaded
    {
        get
        {
            return InOrderPo_MDCountrySalesTax != null;
        }
    }

    public virtual CollectionEntry InOrderPo_MDCountrySalesTaxReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderPo_MDCountrySalesTax); }
    }

    private ICollection<InRequestPos> _InRequestPo_MDCountrySalesTax;
    public virtual ICollection<InRequestPos> InRequestPo_MDCountrySalesTax
    {
        get => LazyLoader.Load(this, ref _InRequestPo_MDCountrySalesTax);
        set => _InRequestPo_MDCountrySalesTax = value;
    }

    public bool InRequestPo_MDCountrySalesTax_IsLoaded
    {
        get
        {
            return InRequestPo_MDCountrySalesTax != null;
        }
    }

    public virtual CollectionEntry InRequestPo_MDCountrySalesTaxReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequestPo_MDCountrySalesTax); }
    }

    private ICollection<InvoicePos> _InvoicePo_MDCountrySalesTax;
    public virtual ICollection<InvoicePos> InvoicePo_MDCountrySalesTax
    {
        get => LazyLoader.Load(this, ref _InvoicePo_MDCountrySalesTax);
        set => _InvoicePo_MDCountrySalesTax = value;
    }

    public bool InvoicePo_MDCountrySalesTax_IsLoaded
    {
        get
        {
            return InvoicePo_MDCountrySalesTax != null;
        }
    }

    public virtual CollectionEntry InvoicePo_MDCountrySalesTaxReference
    {
        get { return Context.Entry(this).Collection(c => c.InvoicePo_MDCountrySalesTax); }
    }

    private MDCountry _MDCountry;
    public virtual MDCountry MDCountry
    { 
        get => LazyLoader.Load(this, ref _MDCountry);
        set => _MDCountry = value;
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
    
    private ICollection<MDCountrySalesTaxMDMaterialGroup> _MDCountrySalesTaxMDMaterialGroup_MDCountrySalesTax;
    public virtual ICollection<MDCountrySalesTaxMDMaterialGroup> MDCountrySalesTaxMDMaterialGroup_MDCountrySalesTax
    {
        get => LazyLoader.Load(this, ref _MDCountrySalesTaxMDMaterialGroup_MDCountrySalesTax);
        set => _MDCountrySalesTaxMDMaterialGroup_MDCountrySalesTax = value;
    }

    public bool MDCountrySalesTaxMDMaterialGroup_MDCountrySalesTax_IsLoaded
    {
        get
        {
            return MDCountrySalesTaxMDMaterialGroup_MDCountrySalesTax != null;
        }
    }

    public virtual CollectionEntry MDCountrySalesTaxMDMaterialGroup_MDCountrySalesTaxReference
    {
        get { return Context.Entry(this).Collection(c => c.MDCountrySalesTaxMDMaterialGroup_MDCountrySalesTax); }
    }

    private ICollection<MDCountrySalesTaxMaterial> _MDCountrySalesTaxMaterial_MDCountrySalesTax;
    public virtual ICollection<MDCountrySalesTaxMaterial> MDCountrySalesTaxMaterial_MDCountrySalesTax
    {
        get => LazyLoader.Load(this, ref _MDCountrySalesTaxMaterial_MDCountrySalesTax);
        set => _MDCountrySalesTaxMaterial_MDCountrySalesTax = value;
    }

    public bool MDCountrySalesTaxMaterial_MDCountrySalesTax_IsLoaded
    {
        get
        {
            return MDCountrySalesTaxMaterial_MDCountrySalesTax != null;
        }
    }

    public virtual CollectionEntry MDCountrySalesTaxMaterial_MDCountrySalesTaxReference
    {
        get { return Context.Entry(this).Collection(c => c.MDCountrySalesTaxMaterial_MDCountrySalesTax); }
    }

    private ICollection<OutOfferPos> _OutOfferPo_MDCountrySalesTax;
    public virtual ICollection<OutOfferPos> OutOfferPo_MDCountrySalesTax
    {
        get => LazyLoader.Load(this, ref _OutOfferPo_MDCountrySalesTax);
        set => _OutOfferPo_MDCountrySalesTax = value;
    }

    public bool OutOfferPo_MDCountrySalesTax_IsLoaded
    {
        get
        {
            return OutOfferPo_MDCountrySalesTax != null;
        }
    }

    public virtual CollectionEntry OutOfferPo_MDCountrySalesTaxReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOfferPo_MDCountrySalesTax); }
    }

    private ICollection<OutOrderPos> _OutOrderPo_MDCountrySalesTax;
    public virtual ICollection<OutOrderPos> OutOrderPo_MDCountrySalesTax
    {
        get => LazyLoader.Load(this, ref _OutOrderPo_MDCountrySalesTax);
        set => _OutOrderPo_MDCountrySalesTax = value;
    }

    public bool OutOrderPo_MDCountrySalesTax_IsLoaded
    {
        get
        {
            return OutOrderPo_MDCountrySalesTax != null;
        }
    }

    public virtual CollectionEntry OutOrderPo_MDCountrySalesTaxReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPo_MDCountrySalesTax); }
    }
}
