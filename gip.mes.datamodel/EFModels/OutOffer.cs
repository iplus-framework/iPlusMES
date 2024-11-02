// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class OutOffer : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public OutOffer()
    {
    }

    private OutOffer(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _OutOfferID;
    public Guid OutOfferID 
    {
        get { return _OutOfferID; }
        set { SetProperty<Guid>(ref _OutOfferID, value); }
    }

    string _OutOfferNo;
    public string OutOfferNo 
    {
        get { return _OutOfferNo; }
        set { SetProperty<string>(ref _OutOfferNo, value); }
    }

    int _OutOfferVersion;
    public int OutOfferVersion 
    {
        get { return _OutOfferVersion; }
        set { SetProperty<int>(ref _OutOfferVersion, value); }
    }

    DateTime? _OutOfferDate;
    public DateTime? OutOfferDate 
    {
        get { return _OutOfferDate; }
        set { SetProperty<DateTime?>(ref _OutOfferDate, value); }
    }

    Guid _MDOutOrderTypeID;
    public Guid MDOutOrderTypeID 
    {
        get { return _MDOutOrderTypeID; }
        set { SetProperty<Guid>(ref _MDOutOrderTypeID, value); }
    }

    Guid _MDOutOfferStateID;
    public Guid MDOutOfferStateID 
    {
        get { return _MDOutOfferStateID; }
        set { SetProperty<Guid>(ref _MDOutOfferStateID, value); }
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

    Guid _BillingCompanyAddressID;
    public Guid BillingCompanyAddressID 
    {
        get { return _BillingCompanyAddressID; }
        set { SetProperty<Guid>(ref _BillingCompanyAddressID, value); }
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

    DateTime _TargetDeliveryDate;
    public DateTime TargetDeliveryDate 
    {
        get { return _TargetDeliveryDate; }
        set { SetProperty<DateTime>(ref _TargetDeliveryDate, value); }
    }

    DateTime? _TargetDeliveryMaxDate;
    public DateTime? TargetDeliveryMaxDate 
    {
        get { return _TargetDeliveryMaxDate; }
        set { SetProperty<DateTime?>(ref _TargetDeliveryMaxDate, value); }
    }

    Guid? _MDTimeRangeID;
    public Guid? MDTimeRangeID 
    {
        get { return _MDTimeRangeID; }
        set { SetProperty<Guid?>(ref _MDTimeRangeID, value); }
    }

    Guid _MDDelivTypeID;
    public Guid MDDelivTypeID 
    {
        get { return _MDDelivTypeID; }
        set { SetProperty<Guid>(ref _MDDelivTypeID, value); }
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

    Guid? _MDTermOfPaymentID;
    public Guid? MDTermOfPaymentID 
    {
        get { return _MDTermOfPaymentID; }
        set { SetProperty<Guid?>(ref _MDTermOfPaymentID, value); }
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

    Guid? _MDCurrencyID;
    public Guid? MDCurrencyID 
    {
        get { return _MDCurrencyID; }
        set { SetProperty<Guid?>(ref _MDCurrencyID, value); }
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
            return BillingCompanyAddress != null;
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
            return CustomerCompany != null;
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
            return DeliveryCompanyAddress != null;
        }
    }

    public virtual ReferenceEntry DeliveryCompanyAddressReference 
    {
        get { return Context.Entry(this).Reference("DeliveryCompanyAddress"); }
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
            return IssuerCompanyAddress != null;
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
            return IssuerCompanyPerson != null;
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
            return MDCurrency != null;
        }
    }

    public virtual ReferenceEntry MDCurrencyReference 
    {
        get { return Context.Entry(this).Reference("MDCurrency"); }
    }
    
    private MDDelivType _MDDelivType;
    public virtual MDDelivType MDDelivType
    { 
        get { return LazyLoader.Load(this, ref _MDDelivType); } 
        set { SetProperty<MDDelivType>(ref _MDDelivType, value); }
    }

    public bool MDDelivType_IsLoaded
    {
        get
        {
            return MDDelivType != null;
        }
    }

    public virtual ReferenceEntry MDDelivTypeReference 
    {
        get { return Context.Entry(this).Reference("MDDelivType"); }
    }
    
    private MDOutOfferState _MDOutOfferState;
    public virtual MDOutOfferState MDOutOfferState
    { 
        get { return LazyLoader.Load(this, ref _MDOutOfferState); } 
        set { SetProperty<MDOutOfferState>(ref _MDOutOfferState, value); }
    }

    public bool MDOutOfferState_IsLoaded
    {
        get
        {
            return MDOutOfferState != null;
        }
    }

    public virtual ReferenceEntry MDOutOfferStateReference 
    {
        get { return Context.Entry(this).Reference("MDOutOfferState"); }
    }
    
    private MDOutOrderType _MDOutOrderType;
    public virtual MDOutOrderType MDOutOrderType
    { 
        get { return LazyLoader.Load(this, ref _MDOutOrderType); } 
        set { SetProperty<MDOutOrderType>(ref _MDOutOrderType, value); }
    }

    public bool MDOutOrderType_IsLoaded
    {
        get
        {
            return MDOutOrderType != null;
        }
    }

    public virtual ReferenceEntry MDOutOrderTypeReference 
    {
        get { return Context.Entry(this).Reference("MDOutOrderType"); }
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
            return MDTermOfPayment != null;
        }
    }

    public virtual ReferenceEntry MDTermOfPaymentReference 
    {
        get { return Context.Entry(this).Reference("MDTermOfPayment"); }
    }
    
    private MDTimeRange _MDTimeRange;
    public virtual MDTimeRange MDTimeRange
    { 
        get { return LazyLoader.Load(this, ref _MDTimeRange); } 
        set { SetProperty<MDTimeRange>(ref _MDTimeRange, value); }
    }

    public bool MDTimeRange_IsLoaded
    {
        get
        {
            return MDTimeRange != null;
        }
    }

    public virtual ReferenceEntry MDTimeRangeReference 
    {
        get { return Context.Entry(this).Reference("MDTimeRange"); }
    }
    
    private ICollection<OutOfferConfig> _OutOfferConfig_OutOffer;
    public virtual ICollection<OutOfferConfig> OutOfferConfig_OutOffer
    {
        get { return LazyLoader.Load(this, ref _OutOfferConfig_OutOffer); }
        set { _OutOfferConfig_OutOffer = value; }
    }

    public bool OutOfferConfig_OutOffer_IsLoaded
    {
        get
        {
            return OutOfferConfig_OutOffer != null;
        }
    }

    public virtual CollectionEntry OutOfferConfig_OutOfferReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOfferConfig_OutOffer); }
    }

    private ICollection<OutOfferPos> _OutOfferPos_OutOffer;
    public virtual ICollection<OutOfferPos> OutOfferPos_OutOffer
    {
        get { return LazyLoader.Load(this, ref _OutOfferPos_OutOffer); }
        set { _OutOfferPos_OutOffer = value; }
    }

    public bool OutOfferPos_OutOffer_IsLoaded
    {
        get
        {
            return OutOfferPos_OutOffer != null;
        }
    }

    public virtual CollectionEntry OutOfferPos_OutOfferReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOfferPos_OutOffer); }
    }

    private ICollection<OutOrder> _OutOrder_BasedOnOutOffer;
    public virtual ICollection<OutOrder> OutOrder_BasedOnOutOffer
    {
        get { return LazyLoader.Load(this, ref _OutOrder_BasedOnOutOffer); }
        set { _OutOrder_BasedOnOutOffer = value; }
    }

    public bool OutOrder_BasedOnOutOffer_IsLoaded
    {
        get
        {
            return OutOrder_BasedOnOutOffer != null;
        }
    }

    public virtual CollectionEntry OutOrder_BasedOnOutOfferReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrder_BasedOnOutOffer); }
    }
}
