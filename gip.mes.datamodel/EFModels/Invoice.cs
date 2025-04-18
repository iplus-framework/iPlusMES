﻿using System;
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
        set { SetProperty<Guid>(ref _MDInvoiceTypeID, value); }
    }

    Guid _MDInvoiceStateID;
    public Guid MDInvoiceStateID 
    {
        get { return _MDInvoiceStateID; }
        set { SetProperty<Guid>(ref _MDInvoiceStateID, value); }
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
        set { SetProperty<Guid>(ref _CustomerCompanyID, value); }
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
        set { SetProperty<Guid?>(ref _DeliveryCompanyAddressID, value); }
    }

    Guid? _BillingCompanyAddressID;
    public Guid? BillingCompanyAddressID 
    {
        get { return _BillingCompanyAddressID; }
        set { SetProperty<Guid?>(ref _BillingCompanyAddressID, value); }
    }

    Guid? _IssuerCompanyAddressID;
    public Guid? IssuerCompanyAddressID 
    {
        get { return _IssuerCompanyAddressID; }
        set { SetProperty<Guid?>(ref _IssuerCompanyAddressID, value); }
    }

    Guid? _IssuerCompanyPersonID;
    public Guid? IssuerCompanyPersonID 
    {
        get { return _IssuerCompanyPersonID; }
        set { SetProperty<Guid?>(ref _IssuerCompanyPersonID, value); }
    }

    Guid? _OutOrderID;
    public Guid? OutOrderID 
    {
        get { return _OutOrderID; }
        set { SetProperty<Guid?>(ref _OutOrderID, value); }
    }

    Guid? _MDTermOfPaymentID;
    public Guid? MDTermOfPaymentID 
    {
        get { return _MDTermOfPaymentID; }
        set { SetProperty<Guid?>(ref _MDTermOfPaymentID, value); }
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
        set { SetProperty<Guid>(ref _MDCurrencyID, value); }
    }

    Guid? _MDCurrencyExchangeID;
    public Guid? MDCurrencyExchangeID 
    {
        get { return _MDCurrencyExchangeID; }
        set { SetProperty<Guid?>(ref _MDCurrencyExchangeID, value); }
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
    
    private ICollection<InvoicePos> _InvoicePos_Invoice;
    public virtual ICollection<InvoicePos> InvoicePos_Invoice
    {
        get { return LazyLoader.Load(this, ref _InvoicePos_Invoice); }
        set { _InvoicePos_Invoice = value; }
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
    }
