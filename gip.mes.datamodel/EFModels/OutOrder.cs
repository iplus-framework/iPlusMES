using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class OutOrder : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public OutOrder()
    {
    }

    private OutOrder(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _OutOrderID;
    public Guid OutOrderID 
    {
        get { return _OutOrderID; }
        set { SetProperty<Guid>(ref _OutOrderID, value); }
    }

    string _OutOrderNo;
    public string OutOrderNo 
    {
        get { return _OutOrderNo; }
        set { SetProperty<string>(ref _OutOrderNo, value); }
    }

    DateTime _OutOrderDate;
    public DateTime OutOrderDate 
    {
        get { return _OutOrderDate; }
        set { SetProperty<DateTime>(ref _OutOrderDate, value); }
    }

    Guid? _BasedOnOutOfferID;
    public Guid? BasedOnOutOfferID 
    {
        get { return _BasedOnOutOfferID; }
        set { SetProperty<Guid?>(ref _BasedOnOutOfferID, value); }
    }

    Guid _MDOutOrderTypeID;
    public Guid MDOutOrderTypeID 
    {
        get { return _MDOutOrderTypeID; }
        set { SetProperty<Guid>(ref _MDOutOrderTypeID, value); }
    }

    Guid _MDOutOrderStateID;
    public Guid MDOutOrderStateID 
    {
        get { return _MDOutOrderStateID; }
        set { SetProperty<Guid>(ref _MDOutOrderStateID, value); }
    }

    Guid? _MDTermOfPaymentID;
    public Guid? MDTermOfPaymentID 
    {
        get { return _MDTermOfPaymentID; }
        set { SetProperty<Guid?>(ref _MDTermOfPaymentID, value); }
    }

    Guid? _CPartnerCompanyID;
    public Guid? CPartnerCompanyID 
    {
        get { return _CPartnerCompanyID; }
        set { SetProperty<Guid?>(ref _CPartnerCompanyID, value); }
    }

    Guid _CustomerCompanyID;
    public Guid CustomerCompanyID 
    {
        get { return _CustomerCompanyID; }
        set { SetProperty<Guid>(ref _CustomerCompanyID, value); }
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

    string _CustOrderNo;
    public string CustOrderNo 
    {
        get { return _CustOrderNo; }
        set { SetProperty<string>(ref _CustOrderNo, value); }
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

    string _KeyOfExtSys;
    public string KeyOfExtSys 
    {
        get { return _KeyOfExtSys; }
        set { SetProperty<string>(ref _KeyOfExtSys, value); }
    }

    private OutOffer _BasedOnOutOffer;
    public virtual OutOffer BasedOnOutOffer
    { 
        get { return LazyLoader.Load(this, ref _BasedOnOutOffer); } 
        set { SetProperty<OutOffer>(ref _BasedOnOutOffer, value); }
    }

    public bool BasedOnOutOffer_IsLoaded
    {
        get
        {
            return _BasedOnOutOffer != null;
        }
    }

    public virtual ReferenceEntry BasedOnOutOfferReference 
    {
        get { return Context.Entry(this).Reference("BasedOnOutOffer"); }
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
    
    private Company _CPartnerCompany;
    public virtual Company CPartnerCompany
    { 
        get { return LazyLoader.Load(this, ref _CPartnerCompany); } 
        set { SetProperty<Company>(ref _CPartnerCompany, value); }
    }

    public bool CPartnerCompany_IsLoaded
    {
        get
        {
            return _CPartnerCompany != null;
        }
    }

    public virtual ReferenceEntry CPartnerCompanyReference 
    {
        get { return Context.Entry(this).Reference("CPartnerCompany"); }
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
    
    private ICollection<Invoice> _Invoice_OutOrder;
    public virtual ICollection<Invoice> Invoice_OutOrder
    {
        get { return LazyLoader.Load(this, ref _Invoice_OutOrder); }
        set { _Invoice_OutOrder = value; }
    }

    public bool Invoice_OutOrder_IsLoaded
    {
        get
        {
            return _Invoice_OutOrder != null;
        }
    }

    public virtual CollectionEntry Invoice_OutOrderReference
    {
        get { return Context.Entry(this).Collection(c => c.Invoice_OutOrder); }
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
            return _MDDelivType != null;
        }
    }

    public virtual ReferenceEntry MDDelivTypeReference 
    {
        get { return Context.Entry(this).Reference("MDDelivType"); }
    }
    
    private MDOutOrderState _MDOutOrderState;
    public virtual MDOutOrderState MDOutOrderState
    { 
        get { return LazyLoader.Load(this, ref _MDOutOrderState); } 
        set { SetProperty<MDOutOrderState>(ref _MDOutOrderState, value); }
    }

    public bool MDOutOrderState_IsLoaded
    {
        get
        {
            return _MDOutOrderState != null;
        }
    }

    public virtual ReferenceEntry MDOutOrderStateReference 
    {
        get { return Context.Entry(this).Reference("MDOutOrderState"); }
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
            return _MDOutOrderType != null;
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
            return _MDTermOfPayment != null;
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
            return _MDTimeRange != null;
        }
    }

    public virtual ReferenceEntry MDTimeRangeReference 
    {
        get { return Context.Entry(this).Reference("MDTimeRange"); }
    }
    
    private ICollection<OutOrderConfig> _OutOrderConfig_OutOrder;
    public virtual ICollection<OutOrderConfig> OutOrderConfig_OutOrder
    {
        get { return LazyLoader.Load(this, ref _OutOrderConfig_OutOrder); }
        set { _OutOrderConfig_OutOrder = value; }
    }

    public bool OutOrderConfig_OutOrder_IsLoaded
    {
        get
        {
            return _OutOrderConfig_OutOrder != null;
        }
    }

    public virtual CollectionEntry OutOrderConfig_OutOrderReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderConfig_OutOrder); }
    }

    private ICollection<OutOrderPos> _OutOrderPos_OutOrder;
    public virtual ICollection<OutOrderPos> OutOrderPos_OutOrder
    {
        get { return LazyLoader.Load(this, ref _OutOrderPos_OutOrder); }
        set { _OutOrderPos_OutOrder = value; }
    }

    public bool OutOrderPos_OutOrder_IsLoaded
    {
        get
        {
            return _OutOrderPos_OutOrder != null;
        }
    }

    public virtual CollectionEntry OutOrderPos_OutOrderReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPos_OutOrder); }
    }
}
