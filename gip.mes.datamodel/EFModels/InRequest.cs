using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class InRequest : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public InRequest()
    {
    }

    private InRequest(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _InRequestID;
    public Guid InRequestID 
    {
        get { return _InRequestID; }
        set { SetProperty<Guid>(ref _InRequestID, value); }
    }

    string _InRequestNo;
    public string InRequestNo 
    {
        get { return _InRequestNo; }
        set { SetProperty<string>(ref _InRequestNo, value); }
    }

    int _InRequestVersion;
    public int InRequestVersion 
    {
        get { return _InRequestVersion; }
        set { SetProperty<int>(ref _InRequestVersion, value); }
    }

    DateTime _InRequestDate;
    public DateTime InRequestDate 
    {
        get { return _InRequestDate; }
        set { SetProperty<DateTime>(ref _InRequestDate, value); }
    }

    Guid _MDInOrderTypeID;
    public Guid MDInOrderTypeID 
    {
        get { return _MDInOrderTypeID; }
        set { SetProperty<Guid>(ref _MDInOrderTypeID, value); }
    }

    Guid _MDInRequestStateID;
    public Guid MDInRequestStateID 
    {
        get { return _MDInRequestStateID; }
        set { SetProperty<Guid>(ref _MDInRequestStateID, value); }
    }

    Guid _DistributorCompanyID;
    public Guid DistributorCompanyID 
    {
        get { return _DistributorCompanyID; }
        set { SetProperty<Guid>(ref _DistributorCompanyID, value); }
    }

    string _DistributorOfferNo;
    public string DistributorOfferNo 
    {
        get { return _DistributorOfferNo; }
        set { SetProperty<string>(ref _DistributorOfferNo, value); }
    }

    Guid _DeliveryCompanyAddressID;
    public Guid DeliveryCompanyAddressID 
    {
        get { return _DeliveryCompanyAddressID; }
        set { SetProperty<Guid>(ref _DeliveryCompanyAddressID, value); }
    }

    Guid _BillingCompanyAddressID;
    public Guid BillingCompanyAddressID 
    {
        get { return _BillingCompanyAddressID; }
        set { SetProperty<Guid>(ref _BillingCompanyAddressID, value); }
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
    
    private Company _DistributorCompany;
    public virtual Company DistributorCompany
    { 
        get { return LazyLoader.Load(this, ref _DistributorCompany); } 
        set { SetProperty<Company>(ref _DistributorCompany, value); }
    }

    public bool DistributorCompany_IsLoaded
    {
        get
        {
            return _DistributorCompany != null;
        }
    }

    public virtual ReferenceEntry DistributorCompanyReference 
    {
        get { return Context.Entry(this).Reference("DistributorCompany"); }
    }
    
    private ICollection<InOrder> _InOrder_BasedOnInRequest;
    public virtual ICollection<InOrder> InOrder_BasedOnInRequest
    {
        get { return LazyLoader.Load(this, ref _InOrder_BasedOnInRequest); }
        set { _InOrder_BasedOnInRequest = value; }
    }

    public bool InOrder_BasedOnInRequest_IsLoaded
    {
        get
        {
            return _InOrder_BasedOnInRequest != null;
        }
    }

    public virtual CollectionEntry InOrder_BasedOnInRequestReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrder_BasedOnInRequest); }
    }

    private ICollection<InRequestConfig> _InRequestConfig_InRequest;
    public virtual ICollection<InRequestConfig> InRequestConfig_InRequest
    {
        get { return LazyLoader.Load(this, ref _InRequestConfig_InRequest); }
        set { _InRequestConfig_InRequest = value; }
    }

    public bool InRequestConfig_InRequest_IsLoaded
    {
        get
        {
            return _InRequestConfig_InRequest != null;
        }
    }

    public virtual CollectionEntry InRequestConfig_InRequestReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequestConfig_InRequest); }
    }

    private ICollection<InRequestPos> _InRequestPos_InRequest;
    public virtual ICollection<InRequestPos> InRequestPos_InRequest
    {
        get { return LazyLoader.Load(this, ref _InRequestPos_InRequest); }
        set { _InRequestPos_InRequest = value; }
    }

    public bool InRequestPos_InRequest_IsLoaded
    {
        get
        {
            return _InRequestPos_InRequest != null;
        }
    }

    public virtual CollectionEntry InRequestPos_InRequestReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequestPos_InRequest); }
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
    
    private MDInOrderType _MDInOrderType;
    public virtual MDInOrderType MDInOrderType
    { 
        get { return LazyLoader.Load(this, ref _MDInOrderType); } 
        set { SetProperty<MDInOrderType>(ref _MDInOrderType, value); }
    }

    public bool MDInOrderType_IsLoaded
    {
        get
        {
            return _MDInOrderType != null;
        }
    }

    public virtual ReferenceEntry MDInOrderTypeReference 
    {
        get { return Context.Entry(this).Reference("MDInOrderType"); }
    }
    
    private MDInRequestState _MDInRequestState;
    public virtual MDInRequestState MDInRequestState
    { 
        get { return LazyLoader.Load(this, ref _MDInRequestState); } 
        set { SetProperty<MDInRequestState>(ref _MDInRequestState, value); }
    }

    public bool MDInRequestState_IsLoaded
    {
        get
        {
            return _MDInRequestState != null;
        }
    }

    public virtual ReferenceEntry MDInRequestStateReference 
    {
        get { return Context.Entry(this).Reference("MDInRequestState"); }
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
    }
