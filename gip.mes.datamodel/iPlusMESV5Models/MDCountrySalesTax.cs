using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDCountrySalesTax : VBEntityObject , IInsertInfo, IUpdateInfo, IMDTrans
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

    private ICollection<InOrderPos> _InOrderPos_MDCountrySalesTax;
    public virtual ICollection<InOrderPos> InOrderPos_MDCountrySalesTax
    {
        get => LazyLoader.Load(this, ref _InOrderPos_MDCountrySalesTax);
        set => _InOrderPos_MDCountrySalesTax = value;
    }

    public bool InOrderPos_MDCountrySalesTax_IsLoaded
    {
        get
        {
            return InOrderPos_MDCountrySalesTax != null;
        }
    }

    public virtual CollectionEntry InOrderPos_MDCountrySalesTaxReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderPos_MDCountrySalesTax); }
    }

    private ICollection<InRequestPos> _InRequestPos_MDCountrySalesTax;
    public virtual ICollection<InRequestPos> InRequestPos_MDCountrySalesTax
    {
        get => LazyLoader.Load(this, ref _InRequestPos_MDCountrySalesTax);
        set => _InRequestPos_MDCountrySalesTax = value;
    }

    public bool InRequestPos_MDCountrySalesTax_IsLoaded
    {
        get
        {
            return InRequestPos_MDCountrySalesTax != null;
        }
    }

    public virtual CollectionEntry InRequestPos_MDCountrySalesTaxReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequestPos_MDCountrySalesTax); }
    }

    private ICollection<InvoicePos> _InvoicePos_MDCountrySalesTax;
    public virtual ICollection<InvoicePos> InvoicePos_MDCountrySalesTax
    {
        get => LazyLoader.Load(this, ref _InvoicePos_MDCountrySalesTax);
        set => _InvoicePos_MDCountrySalesTax = value;
    }

    public bool InvoicePos_MDCountrySalesTax_IsLoaded
    {
        get
        {
            return InvoicePos_MDCountrySalesTax != null;
        }
    }

    public virtual CollectionEntry InvoicePos_MDCountrySalesTaxReference
    {
        get { return Context.Entry(this).Collection(c => c.InvoicePos_MDCountrySalesTax); }
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

    private ICollection<OutOfferPos> _OutOfferPos_MDCountrySalesTax;
    public virtual ICollection<OutOfferPos> OutOfferPos_MDCountrySalesTax
    {
        get => LazyLoader.Load(this, ref _OutOfferPos_MDCountrySalesTax);
        set => _OutOfferPos_MDCountrySalesTax = value;
    }

    public bool OutOfferPos_MDCountrySalesTax_IsLoaded
    {
        get
        {
            return OutOfferPos_MDCountrySalesTax != null;
        }
    }

    public virtual CollectionEntry OutOfferPos_MDCountrySalesTaxReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOfferPos_MDCountrySalesTax); }
    }

    private ICollection<OutOrderPos> _OutOrderPos_MDCountrySalesTax;
    public virtual ICollection<OutOrderPos> OutOrderPos_MDCountrySalesTax
    {
        get => LazyLoader.Load(this, ref _OutOrderPos_MDCountrySalesTax);
        set => _OutOrderPos_MDCountrySalesTax = value;
    }

    public bool OutOrderPos_MDCountrySalesTax_IsLoaded
    {
        get
        {
            return OutOrderPos_MDCountrySalesTax != null;
        }
    }

    public virtual CollectionEntry OutOrderPos_MDCountrySalesTaxReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPos_MDCountrySalesTax); }
    }
}
