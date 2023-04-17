using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class Company : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public Company()
    {
    }

    private Company(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _CompanyID;
    public Guid CompanyID 
    {
        get { return _CompanyID; }
        set { SetProperty<Guid>(ref _CompanyID, value); }
    }

    Guid? _ParentCompanyID;
    public Guid? ParentCompanyID 
    {
        get { return _ParentCompanyID; }
        set { SetProperty<Guid?>(ref _ParentCompanyID, value); }
    }

    string _CompanyNo;
    public string CompanyNo 
    {
        get { return _CompanyNo; }
        set { SetProperty<string>(ref _CompanyNo, value); }
    }

    string _CompanyName;
    public string CompanyName 
    {
        get { return _CompanyName; }
        set { SetProperty<string>(ref _CompanyName, value); }
    }

    Guid? _BillingMDTermOfPaymentID;
    public Guid? BillingMDTermOfPaymentID 
    {
        get { return _BillingMDTermOfPaymentID; }
        set { SetProperty<Guid?>(ref _BillingMDTermOfPaymentID, value); }
    }

    Guid? _ShippingMDTermOfPaymentID;
    public Guid? ShippingMDTermOfPaymentID 
    {
        get { return _ShippingMDTermOfPaymentID; }
        set { SetProperty<Guid?>(ref _ShippingMDTermOfPaymentID, value); }
    }

    Guid? _VBUserID;
    public Guid? VBUserID 
    {
        get { return _VBUserID; }
        set { SetProperty<Guid?>(ref _VBUserID, value); }
    }

    bool _UseBillingAccountNo;
    public bool UseBillingAccountNo 
    {
        get { return _UseBillingAccountNo; }
        set { SetProperty<bool>(ref _UseBillingAccountNo, value); }
    }

    bool _UseShippingAccountNo;
    public bool UseShippingAccountNo 
    {
        get { return _UseShippingAccountNo; }
        set { SetProperty<bool>(ref _UseShippingAccountNo, value); }
    }

    string _BillingAccountNo;
    public string BillingAccountNo 
    {
        get { return _BillingAccountNo; }
        set { SetProperty<string>(ref _BillingAccountNo, value); }
    }

    string _ShippingAccountNo;
    public string ShippingAccountNo 
    {
        get { return _ShippingAccountNo; }
        set { SetProperty<string>(ref _ShippingAccountNo, value); }
    }

    string _NoteInternal;
    public string NoteInternal 
    {
        get { return _NoteInternal; }
        set { SetProperty<string>(ref _NoteInternal, value); }
    }

    string _NoteExternal;
    public string NoteExternal 
    {
        get { return _NoteExternal; }
        set { SetProperty<string>(ref _NoteExternal, value); }
    }

    bool _IsCustomer;
    public bool IsCustomer 
    {
        get { return _IsCustomer; }
        set { SetProperty<bool>(ref _IsCustomer, value); }
    }

    bool _IsDistributor;
    public bool IsDistributor 
    {
        get { return _IsDistributor; }
        set { SetProperty<bool>(ref _IsDistributor, value); }
    }

    bool _IsSalesLead;
    public bool IsSalesLead 
    {
        get { return _IsSalesLead; }
        set { SetProperty<bool>(ref _IsSalesLead, value); }
    }

    bool _IsDistributorLead;
    public bool IsDistributorLead 
    {
        get { return _IsDistributorLead; }
        set { SetProperty<bool>(ref _IsDistributorLead, value); }
    }

    bool _IsOwnCompany;
    public bool IsOwnCompany 
    {
        get { return _IsOwnCompany; }
        set { SetProperty<bool>(ref _IsOwnCompany, value); }
    }

    bool _IsShipper;
    public bool IsShipper 
    {
        get { return _IsShipper; }
        set { SetProperty<bool>(ref _IsShipper, value); }
    }

    Guid _MDCurrencyID;
    public Guid MDCurrencyID 
    {
        get { return _MDCurrencyID; }
        set { SetProperty<Guid>(ref _MDCurrencyID, value); }
    }

    bool _IsActive;
    public bool IsActive 
    {
        get { return _IsActive; }
        set { SetProperty<bool>(ref _IsActive, value); }
    }

    string _VATNumber;
    public string VATNumber 
    {
        get { return _VATNumber; }
        set { SetProperty<string>(ref _VATNumber, value); }
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

    bool _IsTenant;
    public bool IsTenant 
    {
        get { return _IsTenant; }
        set { SetProperty<bool>(ref _IsTenant, value); }
    }

    string _KeyOfExtSys;
    public string KeyOfExtSys 
    {
        get { return _KeyOfExtSys; }
        set { SetProperty<string>(ref _KeyOfExtSys, value); }
    }

    private MDTermOfPayment _BillingMDTermOfPayment;
    public virtual MDTermOfPayment BillingMDTermOfPayment
    { 
        get => LazyLoader.Load(this, ref _BillingMDTermOfPayment);
        set => _BillingMDTermOfPayment = value;
    }

    public bool BillingMDTermOfPayment_IsLoaded
    {
        get
        {
            return BillingMDTermOfPayment != null;
        }
    }

    public virtual ReferenceEntry BillingMDTermOfPaymentReference 
    {
        get { return Context.Entry(this).Reference("BillingMDTermOfPayment"); }
    }
    
    private ICollection<CompanyAddress> _CompanyAddress_Company;
    public virtual ICollection<CompanyAddress> CompanyAddress_Company
    {
        get => LazyLoader.Load(this, ref _CompanyAddress_Company);
        set => _CompanyAddress_Company = value;
    }

    public bool CompanyAddress_Company_IsLoaded
    {
        get
        {
            return CompanyAddress_Company != null;
        }
    }

    public virtual CollectionEntry CompanyAddress_CompanyReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyAddress_Company); }
    }

    private ICollection<CompanyMaterial> _CompanyMaterial_Company;
    public virtual ICollection<CompanyMaterial> CompanyMaterial_Company
    {
        get => LazyLoader.Load(this, ref _CompanyMaterial_Company);
        set => _CompanyMaterial_Company = value;
    }

    public bool CompanyMaterial_Company_IsLoaded
    {
        get
        {
            return CompanyMaterial_Company != null;
        }
    }

    public virtual CollectionEntry CompanyMaterial_CompanyReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyMaterial_Company); }
    }

    private ICollection<CompanyPerson> _CompanyPerson_Company;
    public virtual ICollection<CompanyPerson> CompanyPerson_Company
    {
        get => LazyLoader.Load(this, ref _CompanyPerson_Company);
        set => _CompanyPerson_Company = value;
    }

    public bool CompanyPerson_Company_IsLoaded
    {
        get
        {
            return CompanyPerson_Company != null;
        }
    }

    public virtual CollectionEntry CompanyPerson_CompanyReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyPerson_Company); }
    }

    private ICollection<Facility> _Facility_Company;
    public virtual ICollection<Facility> Facility_Company
    {
        get => LazyLoader.Load(this, ref _Facility_Company);
        set => _Facility_Company = value;
    }

    public bool Facility_Company_IsLoaded
    {
        get
        {
            return Facility_Company != null;
        }
    }

    public virtual CollectionEntry Facility_CompanyReference
    {
        get { return Context.Entry(this).Collection(c => c.Facility_Company); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_CPartnerCompany;
    public virtual ICollection<FacilityBooking> FacilityBooking_CPartnerCompany
    {
        get => LazyLoader.Load(this, ref _FacilityBooking_CPartnerCompany);
        set => _FacilityBooking_CPartnerCompany = value;
    }

    public bool FacilityBooking_CPartnerCompany_IsLoaded
    {
        get
        {
            return FacilityBooking_CPartnerCompany != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_CPartnerCompanyReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_CPartnerCompany); }
    }

    private ICollection<InOrder> _InOrder_CPartnerCompany;
    public virtual ICollection<InOrder> InOrder_CPartnerCompany
    {
        get => LazyLoader.Load(this, ref _InOrder_CPartnerCompany);
        set => _InOrder_CPartnerCompany = value;
    }

    public bool InOrder_CPartnerCompany_IsLoaded
    {
        get
        {
            return InOrder_CPartnerCompany != null;
        }
    }

    public virtual CollectionEntry InOrder_CPartnerCompanyReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrder_CPartnerCompany); }
    }

    private ICollection<InOrder> _InOrder_DistributorCompany;
    public virtual ICollection<InOrder> InOrder_DistributorCompany
    {
        get => LazyLoader.Load(this, ref _InOrder_DistributorCompany);
        set => _InOrder_DistributorCompany = value;
    }

    public bool InOrder_DistributorCompany_IsLoaded
    {
        get
        {
            return InOrder_DistributorCompany != null;
        }
    }

    public virtual CollectionEntry InOrder_DistributorCompanyReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrder_DistributorCompany); }
    }

    private ICollection<InRequest> _InRequest_DistributorCompany;
    public virtual ICollection<InRequest> InRequest_DistributorCompany
    {
        get => LazyLoader.Load(this, ref _InRequest_DistributorCompany);
        set => _InRequest_DistributorCompany = value;
    }

    public bool InRequest_DistributorCompany_IsLoaded
    {
        get
        {
            return InRequest_DistributorCompany != null;
        }
    }

    public virtual CollectionEntry InRequest_DistributorCompanyReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequest_DistributorCompany); }
    }

    private ICollection<Company> _Company_ParentCompany;
    public virtual ICollection<Company> Company_ParentCompany
    {
        get => LazyLoader.Load(this, ref _Company_ParentCompany);
        set => _Company_ParentCompany = value;
    }

    public bool Company_ParentCompany_IsLoaded
    {
        get
        {
            return Company_ParentCompany != null;
        }
    }

    public virtual CollectionEntry Company_ParentCompanyReference
    {
        get { return Context.Entry(this).Collection(c => c.Company_ParentCompany); }
    }

    private ICollection<Invoice> _Invoice_CustomerCompany;
    public virtual ICollection<Invoice> Invoice_CustomerCompany
    {
        get => LazyLoader.Load(this, ref _Invoice_CustomerCompany);
        set => _Invoice_CustomerCompany = value;
    }

    public bool Invoice_CustomerCompany_IsLoaded
    {
        get
        {
            return Invoice_CustomerCompany != null;
        }
    }

    public virtual CollectionEntry Invoice_CustomerCompanyReference
    {
        get { return Context.Entry(this).Collection(c => c.Invoice_CustomerCompany); }
    }

    private MDCurrency _MDCurrency;
    public virtual MDCurrency MDCurrency
    { 
        get => LazyLoader.Load(this, ref _MDCurrency);
        set => _MDCurrency = value;
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
    
    private ICollection<OutOffer> _OutOffer_CustomerCompany;
    public virtual ICollection<OutOffer> OutOffer_CustomerCompany
    {
        get => LazyLoader.Load(this, ref _OutOffer_CustomerCompany);
        set => _OutOffer_CustomerCompany = value;
    }

    public bool OutOffer_CustomerCompany_IsLoaded
    {
        get
        {
            return OutOffer_CustomerCompany != null;
        }
    }

    public virtual CollectionEntry OutOffer_CustomerCompanyReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOffer_CustomerCompany); }
    }

    private ICollection<OutOrder> _OutOrder_CPartnerCompany;
    public virtual ICollection<OutOrder> OutOrder_CPartnerCompany
    {
        get => LazyLoader.Load(this, ref _OutOrder_CPartnerCompany);
        set => _OutOrder_CPartnerCompany = value;
    }

    public bool OutOrder_CPartnerCompany_IsLoaded
    {
        get
        {
            return OutOrder_CPartnerCompany != null;
        }
    }

    public virtual CollectionEntry OutOrder_CPartnerCompanyReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrder_CPartnerCompany); }
    }

    private ICollection<OutOrder> _OutOrder_CustomerCompany;
    public virtual ICollection<OutOrder> OutOrder_CustomerCompany
    {
        get => LazyLoader.Load(this, ref _OutOrder_CustomerCompany);
        set => _OutOrder_CustomerCompany = value;
    }

    public bool OutOrder_CustomerCompany_IsLoaded
    {
        get
        {
            return OutOrder_CustomerCompany != null;
        }
    }

    public virtual CollectionEntry OutOrder_CustomerCompanyReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrder_CustomerCompany); }
    }

    private Company _Company1_ParentCompany;
    public virtual Company Company1_ParentCompany
    { 
        get => LazyLoader.Load(this, ref _Company1_ParentCompany);
        set => _Company1_ParentCompany = value;
    }

    public bool Company1_ParentCompany_IsLoaded
    {
        get
        {
            return Company1_ParentCompany != null;
        }
    }

    public virtual ReferenceEntry Company1_ParentCompanyReference 
    {
        get { return Context.Entry(this).Reference("Company1_ParentCompany"); }
    }
    
    private ICollection<ProdOrder> _ProdOrder_CPartnerCompany;
    public virtual ICollection<ProdOrder> ProdOrder_CPartnerCompany
    {
        get => LazyLoader.Load(this, ref _ProdOrder_CPartnerCompany);
        set => _ProdOrder_CPartnerCompany = value;
    }

    public bool ProdOrder_CPartnerCompany_IsLoaded
    {
        get
        {
            return ProdOrder_CPartnerCompany != null;
        }
    }

    public virtual CollectionEntry ProdOrder_CPartnerCompanyReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrder_CPartnerCompany); }
    }

    private ICollection<Rating> _Rating_Company;
    public virtual ICollection<Rating> Rating_Company
    {
        get => LazyLoader.Load(this, ref _Rating_Company);
        set => _Rating_Company = value;
    }

    public bool Rating_Company_IsLoaded
    {
        get
        {
            return Rating_Company != null;
        }
    }

    public virtual CollectionEntry Rating_CompanyReference
    {
        get { return Context.Entry(this).Collection(c => c.Rating_Company); }
    }

    private MDTermOfPayment _ShippingMDTermOfPayment;
    public virtual MDTermOfPayment ShippingMDTermOfPayment
    { 
        get => LazyLoader.Load(this, ref _ShippingMDTermOfPayment);
        set => _ShippingMDTermOfPayment = value;
    }

    public bool ShippingMDTermOfPayment_IsLoaded
    {
        get
        {
            return ShippingMDTermOfPayment != null;
        }
    }

    public virtual ReferenceEntry ShippingMDTermOfPaymentReference 
    {
        get { return Context.Entry(this).Reference("ShippingMDTermOfPayment"); }
    }
    
    private ICollection<TourplanPos> _TourplanPo_Company;
    public virtual ICollection<TourplanPos> TourplanPo_Company
    {
        get => LazyLoader.Load(this, ref _TourplanPo_Company);
        set => _TourplanPo_Company = value;
    }

    public bool TourplanPo_Company_IsLoaded
    {
        get
        {
            return TourplanPo_Company != null;
        }
    }

    public virtual CollectionEntry TourplanPo_CompanyReference
    {
        get { return Context.Entry(this).Collection(c => c.TourplanPo_Company); }
    }

    private ICollection<Tourplan> _Tourplan_Company;
    public virtual ICollection<Tourplan> Tourplan_Company
    {
        get => LazyLoader.Load(this, ref _Tourplan_Company);
        set => _Tourplan_Company = value;
    }

    public bool Tourplan_Company_IsLoaded
    {
        get
        {
            return Tourplan_Company != null;
        }
    }

    public virtual CollectionEntry Tourplan_CompanyReference
    {
        get { return Context.Entry(this).Collection(c => c.Tourplan_Company); }
    }

    private ICollection<UserSettings> _UserSetting_TenantCompany;
    public virtual ICollection<UserSettings> UserSetting_TenantCompany
    {
        get => LazyLoader.Load(this, ref _UserSetting_TenantCompany);
        set => _UserSetting_TenantCompany = value;
    }

    public bool UserSetting_TenantCompany_IsLoaded
    {
        get
        {
            return UserSetting_TenantCompany != null;
        }
    }

    public virtual CollectionEntry UserSetting_TenantCompanyReference
    {
        get { return Context.Entry(this).Collection(c => c.UserSetting_TenantCompany); }
    }

    private VBUser _VBUser;
    public virtual VBUser VBUser
    { 
        get => LazyLoader.Load(this, ref _VBUser);
        set => _VBUser = value;
    }

    public bool VBUser_IsLoaded
    {
        get
        {
            return VBUser != null;
        }
    }

    public virtual ReferenceEntry VBUserReference 
    {
        get { return Context.Entry(this).Reference("VBUser"); }
    }
    
    private ICollection<Visitor> _Visitor_VisitedCompany;
    public virtual ICollection<Visitor> Visitor_VisitedCompany
    {
        get => LazyLoader.Load(this, ref _Visitor_VisitedCompany);
        set => _Visitor_VisitedCompany = value;
    }

    public bool Visitor_VisitedCompany_IsLoaded
    {
        get
        {
            return Visitor_VisitedCompany != null;
        }
    }

    public virtual CollectionEntry Visitor_VisitedCompanyReference
    {
        get { return Context.Entry(this).Collection(c => c.Visitor_VisitedCompany); }
    }

    private ICollection<Visitor> _Visitor_VisitorCompany;
    public virtual ICollection<Visitor> Visitor_VisitorCompany
    {
        get => LazyLoader.Load(this, ref _Visitor_VisitorCompany);
        set => _Visitor_VisitorCompany = value;
    }

    public bool Visitor_VisitorCompany_IsLoaded
    {
        get
        {
            return Visitor_VisitorCompany != null;
        }
    }

    public virtual CollectionEntry Visitor_VisitorCompanyReference
    {
        get { return Context.Entry(this).Collection(c => c.Visitor_VisitorCompany); }
    }

    private ICollection<VisitorVoucher> _VisitorVoucher_VisitorCompany;
    public virtual ICollection<VisitorVoucher> VisitorVoucher_VisitorCompany
    {
        get => LazyLoader.Load(this, ref _VisitorVoucher_VisitorCompany);
        set => _VisitorVoucher_VisitorCompany = value;
    }

    public bool VisitorVoucher_VisitorCompany_IsLoaded
    {
        get
        {
            return VisitorVoucher_VisitorCompany != null;
        }
    }

    public virtual CollectionEntry VisitorVoucher_VisitorCompanyReference
    {
        get { return Context.Entry(this).Collection(c => c.VisitorVoucher_VisitorCompany); }
    }
}
