using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class VisitorVoucher : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public VisitorVoucher()
    {
    }

    private VisitorVoucher(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _VisitorVoucherID;
    public Guid VisitorVoucherID 
    {
        get { return _VisitorVoucherID; }
        set { SetProperty<Guid>(ref _VisitorVoucherID, value); }
    }

    Guid _VisitorID;
    public Guid VisitorID 
    {
        get { return _VisitorID; }
        set { SetProperty<Guid>(ref _VisitorID, value); }
    }

    int _VisitorVoucherNo;
    public int VisitorVoucherNo 
    {
        get { return _VisitorVoucherNo; }
        set { SetProperty<int>(ref _VisitorVoucherNo, value); }
    }

    Guid _MDVisitorVoucherStateID;
    public Guid MDVisitorVoucherStateID 
    {
        get { return _MDVisitorVoucherStateID; }
        set { SetProperty<Guid>(ref _MDVisitorVoucherStateID, value); }
    }

    DateTime _CheckInDate;
    public DateTime CheckInDate 
    {
        get { return _CheckInDate; }
        set { SetProperty<DateTime>(ref _CheckInDate, value); }
    }

    DateTime? _CheckOutDate;
    public DateTime? CheckOutDate 
    {
        get { return _CheckOutDate; }
        set { SetProperty<DateTime?>(ref _CheckOutDate, value); }
    }

    double _TotalWeight;
    public double TotalWeight 
    {
        get { return _TotalWeight; }
        set { SetProperty<double>(ref _TotalWeight, value); }
    }

    double _EmptyWeight;
    public double EmptyWeight 
    {
        get { return _EmptyWeight; }
        set { SetProperty<double>(ref _EmptyWeight, value); }
    }

    double _LossWeight;
    public double LossWeight 
    {
        get { return _LossWeight; }
        set { SetProperty<double>(ref _LossWeight, value); }
    }

    double _NetWeight;
    public double NetWeight 
    {
        get { return _NetWeight; }
        set { SetProperty<double>(ref _NetWeight, value); }
    }

    Guid? _WeighingID;
    public Guid? WeighingID 
    {
        get { return _WeighingID; }
        set { SetProperty<Guid?>(ref _WeighingID, value); }
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

    Guid? _VisitorCompanyID;
    public Guid? VisitorCompanyID 
    {
        get { return _VisitorCompanyID; }
        set { SetProperty<Guid?>(ref _VisitorCompanyID, value); }
    }

    Guid? _VisitorCompanyPersonID;
    public Guid? VisitorCompanyPersonID 
    {
        get { return _VisitorCompanyPersonID; }
        set { SetProperty<Guid?>(ref _VisitorCompanyPersonID, value); }
    }

    Guid? _MDVisitorCardID;
    public Guid? MDVisitorCardID 
    {
        get { return _MDVisitorCardID; }
        set { SetProperty<Guid?>(ref _MDVisitorCardID, value); }
    }

    Guid? _VehicleFacilityID;
    public Guid? VehicleFacilityID 
    {
        get { return _VehicleFacilityID; }
        set { SetProperty<Guid?>(ref _VehicleFacilityID, value); }
    }

    Guid? _TrailerFacilityID;
    public Guid? TrailerFacilityID 
    {
        get { return _TrailerFacilityID; }
        set { SetProperty<Guid?>(ref _TrailerFacilityID, value); }
    }

    private ICollection<DeliveryNote> _DeliveryNote_VisitorVoucher;
    public virtual ICollection<DeliveryNote> DeliveryNote_VisitorVoucher
    {
        get => LazyLoader.Load(this, ref _DeliveryNote_VisitorVoucher);
        set => _DeliveryNote_VisitorVoucher = value;
    }

    public bool DeliveryNote_VisitorVoucher_IsLoaded
    {
        get
        {
            return DeliveryNote_VisitorVoucher != null;
        }
    }

    public virtual CollectionEntry DeliveryNote_VisitorVoucherReference
    {
        get { return Context.Entry(this).Collection(c => c.DeliveryNote_VisitorVoucher); }
    }

    private MDVisitorCard _MDVisitorCard;
    public virtual MDVisitorCard MDVisitorCard
    { 
        get => LazyLoader.Load(this, ref _MDVisitorCard);
        set => _MDVisitorCard = value;
    }

    public bool MDVisitorCard_IsLoaded
    {
        get
        {
            return MDVisitorCard != null;
        }
    }

    public virtual ReferenceEntry MDVisitorCardReference 
    {
        get { return Context.Entry(this).Reference("MDVisitorCard"); }
    }
    
    private MDVisitorVoucherState _MDVisitorVoucherState;
    public virtual MDVisitorVoucherState MDVisitorVoucherState
    { 
        get => LazyLoader.Load(this, ref _MDVisitorVoucherState);
        set => _MDVisitorVoucherState = value;
    }

    public bool MDVisitorVoucherState_IsLoaded
    {
        get
        {
            return MDVisitorVoucherState != null;
        }
    }

    public virtual ReferenceEntry MDVisitorVoucherStateReference 
    {
        get { return Context.Entry(this).Reference("MDVisitorVoucherState"); }
    }
    
    private ICollection<Picking> _Picking_VisitorVoucher;
    public virtual ICollection<Picking> Picking_VisitorVoucher
    {
        get => LazyLoader.Load(this, ref _Picking_VisitorVoucher);
        set => _Picking_VisitorVoucher = value;
    }

    public bool Picking_VisitorVoucher_IsLoaded
    {
        get
        {
            return Picking_VisitorVoucher != null;
        }
    }

    public virtual CollectionEntry Picking_VisitorVoucherReference
    {
        get { return Context.Entry(this).Collection(c => c.Picking_VisitorVoucher); }
    }

    private ICollection<Tourplan> _Tourplan_VisitorVoucher;
    public virtual ICollection<Tourplan> Tourplan_VisitorVoucher
    {
        get => LazyLoader.Load(this, ref _Tourplan_VisitorVoucher);
        set => _Tourplan_VisitorVoucher = value;
    }

    public bool Tourplan_VisitorVoucher_IsLoaded
    {
        get
        {
            return Tourplan_VisitorVoucher != null;
        }
    }

    public virtual CollectionEntry Tourplan_VisitorVoucherReference
    {
        get { return Context.Entry(this).Collection(c => c.Tourplan_VisitorVoucher); }
    }

    private Facility _TrailerFacility;
    public virtual Facility TrailerFacility
    { 
        get => LazyLoader.Load(this, ref _TrailerFacility);
        set => _TrailerFacility = value;
    }

    public bool TrailerFacility_IsLoaded
    {
        get
        {
            return TrailerFacility != null;
        }
    }

    public virtual ReferenceEntry TrailerFacilityReference 
    {
        get { return Context.Entry(this).Reference("TrailerFacility"); }
    }
    
    private Facility _VehicleFacility;
    public virtual Facility VehicleFacility
    { 
        get => LazyLoader.Load(this, ref _VehicleFacility);
        set => _VehicleFacility = value;
    }

    public bool VehicleFacility_IsLoaded
    {
        get
        {
            return VehicleFacility != null;
        }
    }

    public virtual ReferenceEntry VehicleFacilityReference 
    {
        get { return Context.Entry(this).Reference("VehicleFacility"); }
    }
    
    private Visitor _Visitor;
    public virtual Visitor Visitor
    { 
        get => LazyLoader.Load(this, ref _Visitor);
        set => _Visitor = value;
    }

    public bool Visitor_IsLoaded
    {
        get
        {
            return Visitor != null;
        }
    }

    public virtual ReferenceEntry VisitorReference 
    {
        get { return Context.Entry(this).Reference("Visitor"); }
    }
    
    private Company _VisitorCompany;
    public virtual Company VisitorCompany
    { 
        get => LazyLoader.Load(this, ref _VisitorCompany);
        set => _VisitorCompany = value;
    }

    public bool VisitorCompany_IsLoaded
    {
        get
        {
            return VisitorCompany != null;
        }
    }

    public virtual ReferenceEntry VisitorCompanyReference 
    {
        get { return Context.Entry(this).Reference("VisitorCompany"); }
    }
    
    private CompanyPerson _VisitorCompanyPerson;
    public virtual CompanyPerson VisitorCompanyPerson
    { 
        get => LazyLoader.Load(this, ref _VisitorCompanyPerson);
        set => _VisitorCompanyPerson = value;
    }

    public bool VisitorCompanyPerson_IsLoaded
    {
        get
        {
            return VisitorCompanyPerson != null;
        }
    }

    public virtual ReferenceEntry VisitorCompanyPersonReference 
    {
        get { return Context.Entry(this).Reference("VisitorCompanyPerson"); }
    }
    }
