using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class CompanyAddress : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public CompanyAddress()
    {
    }

    private CompanyAddress(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _CompanyAddressID;
    public Guid CompanyAddressID 
    {
        get { return _CompanyAddressID; }
        set { SetProperty<Guid>(ref _CompanyAddressID, value); }
    }

    Guid _CompanyID;
    public Guid CompanyID 
    {
        get { return _CompanyID; }
        set { SetProperty<Guid>(ref _CompanyID, value); }
    }

    bool _IsHouseCompanyAddress;
    public bool IsHouseCompanyAddress 
    {
        get { return _IsHouseCompanyAddress; }
        set { SetProperty<bool>(ref _IsHouseCompanyAddress, value); }
    }

    bool _IsBillingCompanyAddress;
    public bool IsBillingCompanyAddress 
    {
        get { return _IsBillingCompanyAddress; }
        set { SetProperty<bool>(ref _IsBillingCompanyAddress, value); }
    }

    bool _IsDeliveryCompanyAddress;
    public bool IsDeliveryCompanyAddress 
    {
        get { return _IsDeliveryCompanyAddress; }
        set { SetProperty<bool>(ref _IsDeliveryCompanyAddress, value); }
    }

    bool _IsFactory;
    public bool IsFactory 
    {
        get { return _IsFactory; }
        set { SetProperty<bool>(ref _IsFactory, value); }
    }

    string _InvoiceIssuerNo;
    public string InvoiceIssuerNo 
    {
        get { return _InvoiceIssuerNo; }
        set { SetProperty<string>(ref _InvoiceIssuerNo, value); }
    }

    string _Name1;
    public string Name1 
    {
        get { return _Name1; }
        set { SetProperty<string>(ref _Name1, value); }
    }

    string _Name2;
    public string Name2 
    {
        get { return _Name2; }
        set { SetProperty<string>(ref _Name2, value); }
    }

    string _Name3;
    public string Name3 
    {
        get { return _Name3; }
        set { SetProperty<string>(ref _Name3, value); }
    }

    string _Street;
    public string Street 
    {
        get { return _Street; }
        set { SetProperty<string>(ref _Street, value); }
    }

    string _City;
    public string City 
    {
        get { return _City; }
        set { SetProperty<string>(ref _City, value); }
    }

    string _Postcode;
    public string Postcode 
    {
        get { return _Postcode; }
        set { SetProperty<string>(ref _Postcode, value); }
    }

    string _PostOfficeBox;
    public string PostOfficeBox 
    {
        get { return _PostOfficeBox; }
        set { SetProperty<string>(ref _PostOfficeBox, value); }
    }

    string _Phone;
    public string Phone 
    {
        get { return _Phone; }
        set { SetProperty<string>(ref _Phone, value); }
    }

    string _Fax;
    public string Fax 
    {
        get { return _Fax; }
        set { SetProperty<string>(ref _Fax, value); }
    }

    string _Mobile;
    public string Mobile 
    {
        get { return _Mobile; }
        set { SetProperty<string>(ref _Mobile, value); }
    }

    string _EMail;
    public string EMail 
    {
        get { return _EMail; }
        set { SetProperty<string>(ref _EMail, value); }
    }

    Guid? _MDCountryID;
    public Guid? MDCountryID 
    {
        get { return _MDCountryID; }
        set { SetProperty<Guid?>(ref _MDCountryID, value); }
    }

    Guid? _MDCountryLandID;
    public Guid? MDCountryLandID 
    {
        get { return _MDCountryLandID; }
        set { SetProperty<Guid?>(ref _MDCountryLandID, value); }
    }

    Guid _MDDelivTypeID;
    public Guid MDDelivTypeID 
    {
        get { return _MDDelivTypeID; }
        set { SetProperty<Guid>(ref _MDDelivTypeID, value); }
    }

    int? _RouteNo;
    public int? RouteNo 
    {
        get { return _RouteNo; }
        set { SetProperty<int?>(ref _RouteNo, value); }
    }

    int? _GEO_x;
    public int? GEO_x 
    {
        get { return _GEO_x; }
        set { SetProperty<int?>(ref _GEO_x, value); }
    }

    int? _GEO_y;
    public int? GEO_y 
    {
        get { return _GEO_y; }
        set { SetProperty<int?>(ref _GEO_y, value); }
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

    private Company _Company;
    public virtual Company Company
    { 
        get => LazyLoader.Load(this, ref _Company);
        set => _Company = value;
    }

    public bool Company_IsLoaded
    {
        get
        {
            return Company != null;
        }
    }

    public virtual ReferenceEntry CompanyReference 
    {
        get { return Context.Entry(this).Reference("Company"); }
    }
    
    private ICollection<CompanyAddressDepartment> _CompanyAddressDepartment_CompanyAddress;
    public virtual ICollection<CompanyAddressDepartment> CompanyAddressDepartment_CompanyAddress
    {
        get => LazyLoader.Load(this, ref _CompanyAddressDepartment_CompanyAddress);
        set => _CompanyAddressDepartment_CompanyAddress = value;
    }

    public bool CompanyAddressDepartment_CompanyAddress_IsLoaded
    {
        get
        {
            return CompanyAddressDepartment_CompanyAddress != null;
        }
    }

    public virtual CollectionEntry CompanyAddressDepartment_CompanyAddressReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyAddressDepartment_CompanyAddress); }
    }

    private ICollection<CompanyAddressUnloadingpoint> _CompanyAddressUnloadingpoint_CompanyAddress;
    public virtual ICollection<CompanyAddressUnloadingpoint> CompanyAddressUnloadingpoint_CompanyAddress
    {
        get => LazyLoader.Load(this, ref _CompanyAddressUnloadingpoint_CompanyAddress);
        set => _CompanyAddressUnloadingpoint_CompanyAddress = value;
    }

    public bool CompanyAddressUnloadingpoint_CompanyAddress_IsLoaded
    {
        get
        {
            return CompanyAddressUnloadingpoint_CompanyAddress != null;
        }
    }

    public virtual CollectionEntry CompanyAddressUnloadingpoint_CompanyAddressReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyAddressUnloadingpoint_CompanyAddress); }
    }

    private ICollection<DeliveryNote> _DeliveryNote_Delivery2CompanyAddress;
    public virtual ICollection<DeliveryNote> DeliveryNote_Delivery2CompanyAddress
    {
        get => LazyLoader.Load(this, ref _DeliveryNote_Delivery2CompanyAddress);
        set => _DeliveryNote_Delivery2CompanyAddress = value;
    }

    public bool DeliveryNote_Delivery2CompanyAddress_IsLoaded
    {
        get
        {
            return DeliveryNote_Delivery2CompanyAddress != null;
        }
    }

    public virtual CollectionEntry DeliveryNote_Delivery2CompanyAddressReference
    {
        get { return Context.Entry(this).Collection(c => c.DeliveryNote_Delivery2CompanyAddress); }
    }

    private ICollection<DeliveryNote> _DeliveryNote_DeliveryCompanyAddress;
    public virtual ICollection<DeliveryNote> DeliveryNote_DeliveryCompanyAddress
    {
        get => LazyLoader.Load(this, ref _DeliveryNote_DeliveryCompanyAddress);
        set => _DeliveryNote_DeliveryCompanyAddress = value;
    }

    public bool DeliveryNote_DeliveryCompanyAddress_IsLoaded
    {
        get
        {
            return DeliveryNote_DeliveryCompanyAddress != null;
        }
    }

    public virtual CollectionEntry DeliveryNote_DeliveryCompanyAddressReference
    {
        get { return Context.Entry(this).Collection(c => c.DeliveryNote_DeliveryCompanyAddress); }
    }

    private ICollection<DeliveryNote> _DeliveryNote_ShipperCompanyAddress;
    public virtual ICollection<DeliveryNote> DeliveryNote_ShipperCompanyAddress
    {
        get => LazyLoader.Load(this, ref _DeliveryNote_ShipperCompanyAddress);
        set => _DeliveryNote_ShipperCompanyAddress = value;
    }

    public bool DeliveryNote_ShipperCompanyAddress_IsLoaded
    {
        get
        {
            return DeliveryNote_ShipperCompanyAddress != null;
        }
    }

    public virtual CollectionEntry DeliveryNote_ShipperCompanyAddressReference
    {
        get { return Context.Entry(this).Collection(c => c.DeliveryNote_ShipperCompanyAddress); }
    }

    private ICollection<InOrder> _InOrder_BillingCompanyAddress;
    public virtual ICollection<InOrder> InOrder_BillingCompanyAddress
    {
        get => LazyLoader.Load(this, ref _InOrder_BillingCompanyAddress);
        set => _InOrder_BillingCompanyAddress = value;
    }

    public bool InOrder_BillingCompanyAddress_IsLoaded
    {
        get
        {
            return InOrder_BillingCompanyAddress != null;
        }
    }

    public virtual CollectionEntry InOrder_BillingCompanyAddressReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrder_BillingCompanyAddress); }
    }

    private ICollection<InOrder> _InOrder_DeliveryCompanyAddress;
    public virtual ICollection<InOrder> InOrder_DeliveryCompanyAddress
    {
        get => LazyLoader.Load(this, ref _InOrder_DeliveryCompanyAddress);
        set => _InOrder_DeliveryCompanyAddress = value;
    }

    public bool InOrder_DeliveryCompanyAddress_IsLoaded
    {
        get
        {
            return InOrder_DeliveryCompanyAddress != null;
        }
    }

    public virtual CollectionEntry InOrder_DeliveryCompanyAddressReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrder_DeliveryCompanyAddress); }
    }

    private ICollection<InRequest> _InRequest_BillingCompanyAddress;
    public virtual ICollection<InRequest> InRequest_BillingCompanyAddress
    {
        get => LazyLoader.Load(this, ref _InRequest_BillingCompanyAddress);
        set => _InRequest_BillingCompanyAddress = value;
    }

    public bool InRequest_BillingCompanyAddress_IsLoaded
    {
        get
        {
            return InRequest_BillingCompanyAddress != null;
        }
    }

    public virtual CollectionEntry InRequest_BillingCompanyAddressReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequest_BillingCompanyAddress); }
    }

    private ICollection<InRequest> _InRequest_DeliveryCompanyAddress;
    public virtual ICollection<InRequest> InRequest_DeliveryCompanyAddress
    {
        get => LazyLoader.Load(this, ref _InRequest_DeliveryCompanyAddress);
        set => _InRequest_DeliveryCompanyAddress = value;
    }

    public bool InRequest_DeliveryCompanyAddress_IsLoaded
    {
        get
        {
            return InRequest_DeliveryCompanyAddress != null;
        }
    }

    public virtual CollectionEntry InRequest_DeliveryCompanyAddressReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequest_DeliveryCompanyAddress); }
    }

    private ICollection<Invoice> _Invoice_BillingCompanyAddress;
    public virtual ICollection<Invoice> Invoice_BillingCompanyAddress
    {
        get => LazyLoader.Load(this, ref _Invoice_BillingCompanyAddress);
        set => _Invoice_BillingCompanyAddress = value;
    }

    public bool Invoice_BillingCompanyAddress_IsLoaded
    {
        get
        {
            return Invoice_BillingCompanyAddress != null;
        }
    }

    public virtual CollectionEntry Invoice_BillingCompanyAddressReference
    {
        get { return Context.Entry(this).Collection(c => c.Invoice_BillingCompanyAddress); }
    }

    private ICollection<Invoice> _Invoice_DeliveryCompanyAddress;
    public virtual ICollection<Invoice> Invoice_DeliveryCompanyAddress
    {
        get => LazyLoader.Load(this, ref _Invoice_DeliveryCompanyAddress);
        set => _Invoice_DeliveryCompanyAddress = value;
    }

    public bool Invoice_DeliveryCompanyAddress_IsLoaded
    {
        get
        {
            return Invoice_DeliveryCompanyAddress != null;
        }
    }

    public virtual CollectionEntry Invoice_DeliveryCompanyAddressReference
    {
        get { return Context.Entry(this).Collection(c => c.Invoice_DeliveryCompanyAddress); }
    }

    private ICollection<Invoice> _Invoice_IssuerCompanyAddress;
    public virtual ICollection<Invoice> Invoice_IssuerCompanyAddress
    {
        get => LazyLoader.Load(this, ref _Invoice_IssuerCompanyAddress);
        set => _Invoice_IssuerCompanyAddress = value;
    }

    public bool Invoice_IssuerCompanyAddress_IsLoaded
    {
        get
        {
            return Invoice_IssuerCompanyAddress != null;
        }
    }

    public virtual CollectionEntry Invoice_IssuerCompanyAddressReference
    {
        get { return Context.Entry(this).Collection(c => c.Invoice_IssuerCompanyAddress); }
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
    
    private MDCountryLand _MDCountryLand;
    public virtual MDCountryLand MDCountryLand
    { 
        get => LazyLoader.Load(this, ref _MDCountryLand);
        set => _MDCountryLand = value;
    }

    public bool MDCountryLand_IsLoaded
    {
        get
        {
            return MDCountryLand != null;
        }
    }

    public virtual ReferenceEntry MDCountryLandReference 
    {
        get { return Context.Entry(this).Reference("MDCountryLand"); }
    }
    
    private MDDelivType _MDDelivType;
    public virtual MDDelivType MDDelivType
    { 
        get => LazyLoader.Load(this, ref _MDDelivType);
        set => _MDDelivType = value;
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
    
    private ICollection<OutOffer> _OutOffer_BillingCompanyAddress;
    public virtual ICollection<OutOffer> OutOffer_BillingCompanyAddress
    {
        get => LazyLoader.Load(this, ref _OutOffer_BillingCompanyAddress);
        set => _OutOffer_BillingCompanyAddress = value;
    }

    public bool OutOffer_BillingCompanyAddress_IsLoaded
    {
        get
        {
            return OutOffer_BillingCompanyAddress != null;
        }
    }

    public virtual CollectionEntry OutOffer_BillingCompanyAddressReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOffer_BillingCompanyAddress); }
    }

    private ICollection<OutOffer> _OutOffer_DeliveryCompanyAddress;
    public virtual ICollection<OutOffer> OutOffer_DeliveryCompanyAddress
    {
        get => LazyLoader.Load(this, ref _OutOffer_DeliveryCompanyAddress);
        set => _OutOffer_DeliveryCompanyAddress = value;
    }

    public bool OutOffer_DeliveryCompanyAddress_IsLoaded
    {
        get
        {
            return OutOffer_DeliveryCompanyAddress != null;
        }
    }

    public virtual CollectionEntry OutOffer_DeliveryCompanyAddressReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOffer_DeliveryCompanyAddress); }
    }

    private ICollection<OutOffer> _OutOffer_IssuerCompanyAddress;
    public virtual ICollection<OutOffer> OutOffer_IssuerCompanyAddress
    {
        get => LazyLoader.Load(this, ref _OutOffer_IssuerCompanyAddress);
        set => _OutOffer_IssuerCompanyAddress = value;
    }

    public bool OutOffer_IssuerCompanyAddress_IsLoaded
    {
        get
        {
            return OutOffer_IssuerCompanyAddress != null;
        }
    }

    public virtual CollectionEntry OutOffer_IssuerCompanyAddressReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOffer_IssuerCompanyAddress); }
    }

    private ICollection<OutOrder> _OutOrder_BillingCompanyAddress;
    public virtual ICollection<OutOrder> OutOrder_BillingCompanyAddress
    {
        get => LazyLoader.Load(this, ref _OutOrder_BillingCompanyAddress);
        set => _OutOrder_BillingCompanyAddress = value;
    }

    public bool OutOrder_BillingCompanyAddress_IsLoaded
    {
        get
        {
            return OutOrder_BillingCompanyAddress != null;
        }
    }

    public virtual CollectionEntry OutOrder_BillingCompanyAddressReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrder_BillingCompanyAddress); }
    }

    private ICollection<OutOrder> _OutOrder_DeliveryCompanyAddress;
    public virtual ICollection<OutOrder> OutOrder_DeliveryCompanyAddress
    {
        get => LazyLoader.Load(this, ref _OutOrder_DeliveryCompanyAddress);
        set => _OutOrder_DeliveryCompanyAddress = value;
    }

    public bool OutOrder_DeliveryCompanyAddress_IsLoaded
    {
        get
        {
            return OutOrder_DeliveryCompanyAddress != null;
        }
    }

    public virtual CollectionEntry OutOrder_DeliveryCompanyAddressReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrder_DeliveryCompanyAddress); }
    }

    private ICollection<OutOrder> _OutOrder_IssuerCompanyAddress;
    public virtual ICollection<OutOrder> OutOrder_IssuerCompanyAddress
    {
        get => LazyLoader.Load(this, ref _OutOrder_IssuerCompanyAddress);
        set => _OutOrder_IssuerCompanyAddress = value;
    }

    public bool OutOrder_IssuerCompanyAddress_IsLoaded
    {
        get
        {
            return OutOrder_IssuerCompanyAddress != null;
        }
    }

    public virtual CollectionEntry OutOrder_IssuerCompanyAddressReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrder_IssuerCompanyAddress); }
    }

    private ICollection<Picking> _Picking_DeliveryCompanyAddress;
    public virtual ICollection<Picking> Picking_DeliveryCompanyAddress
    {
        get => LazyLoader.Load(this, ref _Picking_DeliveryCompanyAddress);
        set => _Picking_DeliveryCompanyAddress = value;
    }

    public bool Picking_DeliveryCompanyAddress_IsLoaded
    {
        get
        {
            return Picking_DeliveryCompanyAddress != null;
        }
    }

    public virtual CollectionEntry Picking_DeliveryCompanyAddressReference
    {
        get { return Context.Entry(this).Collection(c => c.Picking_DeliveryCompanyAddress); }
    }

    private ICollection<TourplanPos> _TourplanPos_CompanyAddress;
    public virtual ICollection<TourplanPos> TourplanPos_CompanyAddress
    {
        get => LazyLoader.Load(this, ref _TourplanPos_CompanyAddress);
        set => _TourplanPos_CompanyAddress = value;
    }

    public bool TourplanPos_CompanyAddress_IsLoaded
    {
        get
        {
            return TourplanPos_CompanyAddress != null;
        }
    }

    public virtual CollectionEntry TourplanPos_CompanyAddressReference
    {
        get { return Context.Entry(this).Collection(c => c.TourplanPos_CompanyAddress); }
    }

    private ICollection<UserSettings> _UserSetting_InvoiceCompanyAddress;
    public virtual ICollection<UserSettings> UserSetting_InvoiceCompanyAddress
    {
        get => LazyLoader.Load(this, ref _UserSetting_InvoiceCompanyAddress);
        set => _UserSetting_InvoiceCompanyAddress = value;
    }

    public bool UserSetting_InvoiceCompanyAddress_IsLoaded
    {
        get
        {
            return UserSetting_InvoiceCompanyAddress != null;
        }
    }

    public virtual CollectionEntry UserSetting_InvoiceCompanyAddressReference
    {
        get { return Context.Entry(this).Collection(c => c.UserSetting_InvoiceCompanyAddress); }
    }
}
