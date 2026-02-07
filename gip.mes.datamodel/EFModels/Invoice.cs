using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class Invoice : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public Invoice()
    {
    }

    private Invoice(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _InvoiceID;
    public Guid InvoiceID 
    {
        get { return _InvoiceID; }
        set { SetProperty<Guid>(ref _InvoiceID, value); }
    }

    Guid _MDInvoiceTypeID;
    public Guid MDInvoiceTypeID 
    {
        get { return _MDInvoiceTypeID; }
        set { SetForeignKeyProperty<Guid>(ref _MDInvoiceTypeID, value, "MDInvoiceType", _MDInvoiceType, MDInvoiceType != null ? MDInvoiceType.MDInvoiceTypeID : default(Guid)); }
    }

    Guid _MDInvoiceStateID;
    public Guid MDInvoiceStateID 
    {
        get { return _MDInvoiceStateID; }
        set { SetForeignKeyProperty<Guid>(ref _MDInvoiceStateID, value, "MDInvoiceState", _MDInvoiceState, MDInvoiceState != null ? MDInvoiceState.MDInvoiceStateID : default(Guid)); }
    }

    string _InvoiceNo;
    public string InvoiceNo 
    {
        get { return _InvoiceNo; }
        set { SetProperty<string>(ref _InvoiceNo, value); }
    }

    DateTime _InvoiceDate;
    public DateTime InvoiceDate 
    {
        get { return _InvoiceDate; }
        set { SetProperty<DateTime>(ref _InvoiceDate, value); }
    }

    Guid _CustomerCompanyID;
    public Guid CustomerCompanyID 
    {
        get { return _CustomerCompanyID; }
        set { SetForeignKeyProperty<Guid>(ref _CustomerCompanyID, value, "CustomerCompany", _CustomerCompany, CustomerCompany != null ? CustomerCompany.CompanyID : default(Guid)); }
    }

    string _CustRequestNo;
    public string CustRequestNo 
    {
        get { return _CustRequestNo; }
        set { SetProperty<string>(ref _CustRequestNo, value); }
    }

    Guid? _DeliveryCompanyAddressID;
    public Guid? DeliveryCompanyAddressID 
    {
        get { return _DeliveryCompanyAddressID; }
        set { SetForeignKeyProperty<Guid?>(ref _DeliveryCompanyAddressID, value, "DeliveryCompanyAddress", _DeliveryCompanyAddress, DeliveryCompanyAddress != null ? DeliveryCompanyAddress.CompanyAddressID : default(Guid?)); }
    }

    Guid? _BillingCompanyAddressID;
    public Guid? BillingCompanyAddressID 
    {
        get { return _BillingCompanyAddressID; }
        set { SetForeignKeyProperty<Guid?>(ref _BillingCompanyAddressID, value, "BillingCompanyAddress", _BillingCompanyAddress, BillingCompanyAddress != null ? BillingCompanyAddress.CompanyAddressID : default(Guid?)); }
    }

    Guid? _IssuerCompanyAddressID;
    public Guid? IssuerCompanyAddressID 
    {
        get { return _IssuerCompanyAddressID; }
        set { SetForeignKeyProperty<Guid?>(ref _IssuerCompanyAddressID, value, "IssuerCompanyAddress", _IssuerCompanyAddress, IssuerCompanyAddress != null ? IssuerCompanyAddress.CompanyAddressID : default(Guid?)); }
    }

    Guid? _IssuerCompanyPersonID;
    public Guid? IssuerCompanyPersonID 
    {
        get { return _IssuerCompanyPersonID; }
        set { SetForeignKeyProperty<Guid?>(ref _IssuerCompanyPersonID, value, "IssuerCompanyPerson", _IssuerCompanyPerson, IssuerCompanyPerson != null ? IssuerCompanyPerson.CompanyPersonID : default(Guid?)); }
    }

    Guid? _OutOrderID;
    public Guid? OutOrderID 
    {
        get { return _OutOrderID; }
        set { SetForeignKeyProperty<Guid?>(ref _OutOrderID, value, "OutOrder", _OutOrder, OutOrder != null ? OutOrder.OutOrderID : default(Guid?)); }
    }

    Guid? _MDTermOfPaymentID;
    public Guid? MDTermOfPaymentID 
    {
        get { return _MDTermOfPaymentID; }
        set { SetForeignKeyProperty<Guid?>(ref _MDTermOfPaymentID, value, "MDTermOfPayment", _MDTermOfPayment, MDTermOfPayment != null ? MDTermOfPayment.MDTermOfPaymentID : default(Guid?)); }
    }

    decimal _PriceNet;
    public decimal PriceNet 
    {
        get { return _PriceNet; }
        set { SetProperty<decimal>(ref _PriceNet, value); }
    }

    decimal _PriceGross;
    public decimal PriceGross 
    {
        get { return _PriceGross; }
        set { SetProperty<decimal>(ref _PriceGross, value); }
    }

    float _SalesTax;
    public float SalesTax 
    {
        get { return _SalesTax; }
        set { SetProperty<float>(ref _SalesTax, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
    }

    string _XMLDesignStart;
    public string XMLDesignStart 
    {
        get { return _XMLDesignStart; }
        set { SetProperty<string>(ref _XMLDesignStart, value); }
    }

    string _XMLDesignEnd;
    public string XMLDesignEnd 
    {
        get { return _XMLDesignEnd; }
        set { SetProperty<string>(ref _XMLDesignEnd, value); }
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

    Guid _MDCurrencyID;
    public Guid MDCurrencyID 
    {
        get { return _MDCurrencyID; }
        set { SetForeignKeyProperty<Guid>(ref _MDCurrencyID, value, "MDCurrency", _MDCurrency, MDCurrency != null ? MDCurrency.MDCurrencyID : default(Guid)); }
    }

    Guid? _MDCurrencyExchangeID;
    public Guid? MDCurrencyExchangeID 
    {
        get { return _MDCurrencyExchangeID; }
        set { SetForeignKeyProperty<Guid?>(ref _MDCurrencyExchangeID, value, "MDCurrencyExchange", _MDCurrencyExchange, MDCurrencyExchange != null ? MDCurrencyExchange.MDCurrencyExchangeID : default(Guid?)); }
    }

    Guid? _ReferenceInvoiceID;
    public Guid? ReferenceInvoiceID 
    {
        get { return _ReferenceInvoiceID; }
        set { SetForeignKeyProperty<Guid?>(ref _ReferenceInvoiceID, value, "Invoice1_ReferenceInvoice", _Invoice1_ReferenceInvoice, Invoice1_ReferenceInvoice != null ? Invoice1_ReferenceInvoice.InvoiceID : default(Guid?)); }
    }

    int? _EInvoiceType;
    public int? EInvoiceType 
    {
        get { return _EInvoiceType; }
        set { SetProperty<int?>(ref _EInvoiceType, value); }
    }

    string _EInvoiceBusinessProcessType;
    public string EInvoiceBusinessProcessType 
    {
        get { return _EInvoiceBusinessProcessType; }
        set { SetProperty<string>(ref _EInvoiceBusinessProcessType, value); }
    }

    private CompanyAddress _BillingCompanyAddress;
    public virtual CompanyAddress BillingCompanyAddress
    { 
        get { return LazyLoader.Load(this, ref _BillingCompanyAddress); } 
        set { SetProperty<CompanyAddress>(ref _BillingCompanyAddress, value); }
    }

    public bool BillingCompanyAddress_IsLoaded
    {
        get
        {
            return _BillingCompanyAddress != null;
        }
    }

    public virtual ReferenceEntry BillingCompanyAddressReference 
    {
        get { return Context.Entry(this).Reference("BillingCompanyAddress"); }
    }
    
    private Company _CustomerCompany;
    public virtual Company CustomerCompany
    { 
        get { return LazyLoader.Load(this, ref _CustomerCompany); } 
        set { SetProperty<Company>(ref _CustomerCompany, value); }
    }

    public bool CustomerCompany_IsLoaded
    {
        get
        {
            return _CustomerCompany != null;
        }
    }

    public virtual ReferenceEntry CustomerCompanyReference 
    {
        get { return Context.Entry(this).Reference("CustomerCompany"); }
    }
    
    private CompanyAddress _DeliveryCompanyAddress;
    public virtual CompanyAddress DeliveryCompanyAddress
    { 
        get { return LazyLoader.Load(this, ref _DeliveryCompanyAddress); } 
        set { SetProperty<CompanyAddress>(ref _DeliveryCompanyAddress, value); }
    }

    public bool DeliveryCompanyAddress_IsLoaded
    {
        get
        {
            return _DeliveryCompanyAddress != null;
        }
    }

    public virtual ReferenceEntry DeliveryCompanyAddressReference 
    {
        get { return Context.Entry(this).Reference("DeliveryCompanyAddress"); }
    }
    
    private ICollection<Invoice> _Invoice_ReferenceInvoice;
    public virtual ICollection<Invoice> Invoice_ReferenceInvoice
    {
        get { return LazyLoader.Load(this, ref _Invoice_ReferenceInvoice); }
        set { SetProperty<ICollection<Invoice>>(ref _Invoice_ReferenceInvoice, value); }
    }

    public bool Invoice_ReferenceInvoice_IsLoaded
    {
        get
        {
            return _Invoice_ReferenceInvoice != null;
        }
    }

    public virtual CollectionEntry Invoice_ReferenceInvoiceReference
    {
        get { return Context.Entry(this).Collection(c => c.Invoice_ReferenceInvoice); }
    }

    private ICollection<InvoicePos> _InvoicePos_Invoice;
    public virtual ICollection<InvoicePos> InvoicePos_Invoice
    {
        get { return LazyLoader.Load(this, ref _InvoicePos_Invoice); }
        set { SetProperty<ICollection<InvoicePos>>(ref _InvoicePos_Invoice, value); }
    }

    public bool InvoicePos_Invoice_IsLoaded
    {
        get
        {
            return _InvoicePos_Invoice != null;
        }
    }

    public virtual CollectionEntry InvoicePos_InvoiceReference
    {
        get { return Context.Entry(this).Collection(c => c.InvoicePos_Invoice); }
    }

    private CompanyAddress _IssuerCompanyAddress;
    public virtual CompanyAddress IssuerCompanyAddress
    { 
        get { return LazyLoader.Load(this, ref _IssuerCompanyAddress); } 
        set { SetProperty<CompanyAddress>(ref _IssuerCompanyAddress, value); }
    }

    public bool IssuerCompanyAddress_IsLoaded
    {
        get
        {
            return _IssuerCompanyAddress != null;
        }
    }

    public virtual ReferenceEntry IssuerCompanyAddressReference 
    {
        get { return Context.Entry(this).Reference("IssuerCompanyAddress"); }
    }
    
    private CompanyPerson _IssuerCompanyPerson;
    public virtual CompanyPerson IssuerCompanyPerson
    { 
        get { return LazyLoader.Load(this, ref _IssuerCompanyPerson); } 
        set { SetProperty<CompanyPerson>(ref _IssuerCompanyPerson, value); }
    }

    public bool IssuerCompanyPerson_IsLoaded
    {
        get
        {
            return _IssuerCompanyPerson != null;
        }
    }

    public virtual ReferenceEntry IssuerCompanyPersonReference 
    {
        get { return Context.Entry(this).Reference("IssuerCompanyPerson"); }
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
            return _MDCurrency != null;
        }
    }

    public virtual ReferenceEntry MDCurrencyReference 
    {
        get { return Context.Entry(this).Reference("MDCurrency"); }
    }
    
    private MDCurrencyExchange _MDCurrencyExchange;
    public virtual MDCurrencyExchange MDCurrencyExchange
    { 
        get { return LazyLoader.Load(this, ref _MDCurrencyExchange); } 
        set { SetProperty<MDCurrencyExchange>(ref _MDCurrencyExchange, value); }
    }

    public bool MDCurrencyExchange_IsLoaded
    {
        get
        {
            return _MDCurrencyExchange != null;
        }
    }

    public virtual ReferenceEntry MDCurrencyExchangeReference 
    {
        get { return Context.Entry(this).Reference("MDCurrencyExchange"); }
    }
    
    private MDInvoiceState _MDInvoiceState;
    public virtual MDInvoiceState MDInvoiceState
    { 
        get { return LazyLoader.Load(this, ref _MDInvoiceState); } 
        set { SetProperty<MDInvoiceState>(ref _MDInvoiceState, value); }
    }

    public bool MDInvoiceState_IsLoaded
    {
        get
        {
            return _MDInvoiceState != null;
        }
    }

    public virtual ReferenceEntry MDInvoiceStateReference 
    {
        get { return Context.Entry(this).Reference("MDInvoiceState"); }
    }
    
    private MDInvoiceType _MDInvoiceType;
    public virtual MDInvoiceType MDInvoiceType
    { 
        get { return LazyLoader.Load(this, ref _MDInvoiceType); } 
        set { SetProperty<MDInvoiceType>(ref _MDInvoiceType, value); }
    }

    public bool MDInvoiceType_IsLoaded
    {
        get
        {
            return _MDInvoiceType != null;
        }
    }

    public virtual ReferenceEntry MDInvoiceTypeReference 
    {
        get { return Context.Entry(this).Reference("MDInvoiceType"); }
    }
    
    private MDTermOfPayment _MDTermOfPayment;
    public virtual MDTermOfPayment MDTermOfPayment
    { 
        get { return LazyLoader.Load(this, ref _MDTermOfPayment); } 
        set { SetProperty<MDTermOfPayment>(ref _MDTermOfPayment, value); }
    }

    public bool MDTermOfPayment_IsLoaded
    {
        get
        {
            return _MDTermOfPayment != null;
        }
    }

    public virtual ReferenceEntry MDTermOfPaymentReference 
    {
        get { return Context.Entry(this).Reference("MDTermOfPayment"); }
    }
    
    private OutOrder _OutOrder;
    public virtual OutOrder OutOrder
    { 
        get { return LazyLoader.Load(this, ref _OutOrder); } 
        set { SetProperty<OutOrder>(ref _OutOrder, value); }
    }

    public bool OutOrder_IsLoaded
    {
        get
        {
            return _OutOrder != null;
        }
    }

    public virtual ReferenceEntry OutOrderReference 
    {
        get { return Context.Entry(this).Reference("OutOrder"); }
    }
    
    private Invoice _Invoice1_ReferenceInvoice;
    public virtual Invoice Invoice1_ReferenceInvoice
    { 
        get { return LazyLoader.Load(this, ref _Invoice1_ReferenceInvoice); } 
        set { SetProperty<Invoice>(ref _Invoice1_ReferenceInvoice, value); }
    }

    public bool Invoice1_ReferenceInvoice_IsLoaded
    {
        get
        {
            return _Invoice1_ReferenceInvoice != null;
        }
    }

    public virtual ReferenceEntry Invoice1_ReferenceInvoiceReference 
    {
        get { return Context.Entry(this).Reference("Invoice1_ReferenceInvoice"); }
    }
    }
