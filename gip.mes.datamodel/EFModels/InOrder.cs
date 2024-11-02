// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class InOrder : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public InOrder()
    {
    }

    private InOrder(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _InOrderID;
    public Guid InOrderID 
    {
        get { return _InOrderID; }
        set { SetProperty<Guid>(ref _InOrderID, value); }
    }

    string _InOrderNo;
    public string InOrderNo 
    {
        get { return _InOrderNo; }
        set { SetProperty<string>(ref _InOrderNo, value); }
    }

    DateTime _InOrderDate;
    public DateTime InOrderDate 
    {
        get { return _InOrderDate; }
        set { SetProperty<DateTime>(ref _InOrderDate, value); }
    }

    Guid _MDInOrderTypeID;
    public Guid MDInOrderTypeID 
    {
        get { return _MDInOrderTypeID; }
        set { SetProperty<Guid>(ref _MDInOrderTypeID, value); }
    }

    Guid _MDInOrderStateID;
    public Guid MDInOrderStateID 
    {
        get { return _MDInOrderStateID; }
        set { SetProperty<Guid>(ref _MDInOrderStateID, value); }
    }

    Guid _DistributorCompanyID;
    public Guid DistributorCompanyID 
    {
        get { return _DistributorCompanyID; }
        set { SetProperty<Guid>(ref _DistributorCompanyID, value); }
    }

    string _DistributorOrderNo;
    public string DistributorOrderNo 
    {
        get { return _DistributorOrderNo; }
        set { SetProperty<string>(ref _DistributorOrderNo, value); }
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

    Guid? _MDTermOfPaymentID;
    public Guid? MDTermOfPaymentID 
    {
        get { return _MDTermOfPaymentID; }
        set { SetProperty<Guid?>(ref _MDTermOfPaymentID, value); }
    }

    Guid? _BasedOnInRequestID;
    public Guid? BasedOnInRequestID 
    {
        get { return _BasedOnInRequestID; }
        set { SetProperty<Guid?>(ref _BasedOnInRequestID, value); }
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

    Guid? _CPartnerCompanyID;
    public Guid? CPartnerCompanyID 
    {
        get { return _CPartnerCompanyID; }
        set { SetProperty<Guid?>(ref _CPartnerCompanyID, value); }
    }

    Guid? _IssuerCompanyPersonID;
    public Guid? IssuerCompanyPersonID 
    {
        get { return _IssuerCompanyPersonID; }
        set { SetProperty<Guid?>(ref _IssuerCompanyPersonID, value); }
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

    private InRequest _BasedOnInRequest;
    public virtual InRequest BasedOnInRequest
    { 
        get { return LazyLoader.Load(this, ref _BasedOnInRequest); } 
        set { SetProperty<InRequest>(ref _BasedOnInRequest, value); }
    }

    public bool BasedOnInRequest_IsLoaded
    {
        get
        {
            return BasedOnInRequest != null;
        }
    }

    public virtual ReferenceEntry BasedOnInRequestReference 
    {
        get { return Context.Entry(this).Reference("BasedOnInRequest"); }
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
            return CPartnerCompany != null;
        }
    }

    public virtual ReferenceEntry CPartnerCompanyReference 
    {
        get { return Context.Entry(this).Reference("CPartnerCompany"); }
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
            return DistributorCompany != null;
        }
    }

    public virtual ReferenceEntry DistributorCompanyReference 
    {
        get { return Context.Entry(this).Reference("DistributorCompany"); }
    }
    
    private ICollection<InOrderConfig> _InOrderConfig_InOrder;
    public virtual ICollection<InOrderConfig> InOrderConfig_InOrder
    {
        get { return LazyLoader.Load(this, ref _InOrderConfig_InOrder); }
        set { _InOrderConfig_InOrder = value; }
    }

    public bool InOrderConfig_InOrder_IsLoaded
    {
        get
        {
            return InOrderConfig_InOrder != null;
        }
    }

    public virtual CollectionEntry InOrderConfig_InOrderReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderConfig_InOrder); }
    }

    private ICollection<InOrderPos> _InOrderPos_InOrder;
    public virtual ICollection<InOrderPos> InOrderPos_InOrder
    {
        get { return LazyLoader.Load(this, ref _InOrderPos_InOrder); }
        set { _InOrderPos_InOrder = value; }
    }

    public bool InOrderPos_InOrder_IsLoaded
    {
        get
        {
            return InOrderPos_InOrder != null;
        }
    }

    public virtual CollectionEntry InOrderPos_InOrderReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderPos_InOrder); }
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
    
    private MDInOrderState _MDInOrderState;
    public virtual MDInOrderState MDInOrderState
    { 
        get { return LazyLoader.Load(this, ref _MDInOrderState); } 
        set { SetProperty<MDInOrderState>(ref _MDInOrderState, value); }
    }

    public bool MDInOrderState_IsLoaded
    {
        get
        {
            return MDInOrderState != null;
        }
    }

    public virtual ReferenceEntry MDInOrderStateReference 
    {
        get { return Context.Entry(this).Reference("MDInOrderState"); }
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
            return MDInOrderType != null;
        }
    }

    public virtual ReferenceEntry MDInOrderTypeReference 
    {
        get { return Context.Entry(this).Reference("MDInOrderType"); }
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
    
    private ICollection<PlanningMRProposal> _PlanningMRProposal_InOrder;
    public virtual ICollection<PlanningMRProposal> PlanningMRProposal_InOrder
    {
        get { return LazyLoader.Load(this, ref _PlanningMRProposal_InOrder); }
        set { _PlanningMRProposal_InOrder = value; }
    }

    public bool PlanningMRProposal_InOrder_IsLoaded
    {
        get
        {
            return PlanningMRProposal_InOrder != null;
        }
    }

    public virtual CollectionEntry PlanningMRProposal_InOrderReference
    {
        get { return Context.Entry(this).Collection(c => c.PlanningMRProposal_InOrder); }
    }
}
