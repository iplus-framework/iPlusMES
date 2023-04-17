using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class Tourplan : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public Tourplan()
    {
    }

    private Tourplan(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _TourplanID;
    public Guid TourplanID 
    {
        get { return _TourplanID; }
        set { SetProperty<Guid>(ref _TourplanID, value); }
    }

    string _TourplanNo;
    public string TourplanNo 
    {
        get { return _TourplanNo; }
        set { SetProperty<string>(ref _TourplanNo, value); }
    }

    string _TourplanName;
    public string TourplanName 
    {
        get { return _TourplanName; }
        set { SetProperty<string>(ref _TourplanName, value); }
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

    Guid _CompanyID;
    public Guid CompanyID 
    {
        get { return _CompanyID; }
        set { SetProperty<Guid>(ref _CompanyID, value); }
    }

    Guid? _MDTourID;
    public Guid? MDTourID 
    {
        get { return _MDTourID; }
        set { SetProperty<Guid?>(ref _MDTourID, value); }
    }

    string _LoadingStation;
    public string LoadingStation 
    {
        get { return _LoadingStation; }
        set { SetProperty<string>(ref _LoadingStation, value); }
    }

    DateTime? _DeliveryDate;
    public DateTime? DeliveryDate 
    {
        get { return _DeliveryDate; }
        set { SetProperty<DateTime?>(ref _DeliveryDate, value); }
    }

    DateTime? _ActDeliveryDate;
    public DateTime? ActDeliveryDate 
    {
        get { return _ActDeliveryDate; }
        set { SetProperty<DateTime?>(ref _ActDeliveryDate, value); }
    }

    int _NightLoading;
    public int NightLoading 
    {
        get { return _NightLoading; }
        set { SetProperty<int>(ref _NightLoading, value); }
    }

    Guid _MDTourplanStateID;
    public Guid MDTourplanStateID 
    {
        get { return _MDTourplanStateID; }
        set { SetProperty<Guid>(ref _MDTourplanStateID, value); }
    }

    double _MaxCapacitySum;
    public double MaxCapacitySum 
    {
        get { return _MaxCapacitySum; }
        set { SetProperty<double>(ref _MaxCapacitySum, value); }
    }

    double? _MaxCapacityDiff;
    public double? MaxCapacityDiff 
    {
        get { return _MaxCapacityDiff; }
        set { SetProperty<double?>(ref _MaxCapacityDiff, value); }
    }

    double _MaxWeightSum;
    public double MaxWeightSum 
    {
        get { return _MaxWeightSum; }
        set { SetProperty<double>(ref _MaxWeightSum, value); }
    }

    double _MaxWeightDiff;
    public double MaxWeightDiff 
    {
        get { return _MaxWeightDiff; }
        set { SetProperty<double>(ref _MaxWeightDiff, value); }
    }

    double _NetWeight;
    public double NetWeight 
    {
        get { return _NetWeight; }
        set { SetProperty<double>(ref _NetWeight, value); }
    }

    string _FirstWeighingIdentityNo;
    public string FirstWeighingIdentityNo 
    {
        get { return _FirstWeighingIdentityNo; }
        set { SetProperty<string>(ref _FirstWeighingIdentityNo, value); }
    }

    double _FirstWeighing;
    public double FirstWeighing 
    {
        get { return _FirstWeighing; }
        set { SetProperty<double>(ref _FirstWeighing, value); }
    }

    string _SecondWeighingIdentityNo;
    public string SecondWeighingIdentityNo 
    {
        get { return _SecondWeighingIdentityNo; }
        set { SetProperty<string>(ref _SecondWeighingIdentityNo, value); }
    }

    double _SecondWeighing;
    public double SecondWeighing 
    {
        get { return _SecondWeighing; }
        set { SetProperty<double>(ref _SecondWeighing, value); }
    }

    string _LastWeighingIdentityNo;
    public string LastWeighingIdentityNo 
    {
        get { return _LastWeighingIdentityNo; }
        set { SetProperty<string>(ref _LastWeighingIdentityNo, value); }
    }

    double _LastWeighing;
    public double LastWeighing 
    {
        get { return _LastWeighing; }
        set { SetProperty<double>(ref _LastWeighing, value); }
    }

    Guid? _VisitorVoucherID;
    public Guid? VisitorVoucherID 
    {
        get { return _VisitorVoucherID; }
        set { SetProperty<Guid?>(ref _VisitorVoucherID, value); }
    }

    int _PeriodInt;
    public int PeriodInt 
    {
        get { return _PeriodInt; }
        set { SetProperty<int>(ref _PeriodInt, value); }
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
    
    private MDTour _MDTour;
    public virtual MDTour MDTour
    { 
        get => LazyLoader.Load(this, ref _MDTour);
        set => _MDTour = value;
    }

    public bool MDTour_IsLoaded
    {
        get
        {
            return MDTour != null;
        }
    }

    public virtual ReferenceEntry MDTourReference 
    {
        get { return Context.Entry(this).Reference("MDTour"); }
    }
    
    private MDTourplanState _MDTourplanState;
    public virtual MDTourplanState MDTourplanState
    { 
        get => LazyLoader.Load(this, ref _MDTourplanState);
        set => _MDTourplanState = value;
    }

    public bool MDTourplanState_IsLoaded
    {
        get
        {
            return MDTourplanState != null;
        }
    }

    public virtual ReferenceEntry MDTourplanStateReference 
    {
        get { return Context.Entry(this).Reference("MDTourplanState"); }
    }
    
    private ICollection<Picking> _Picking_Tourplan;
    public virtual ICollection<Picking> Picking_Tourplan
    {
        get => LazyLoader.Load(this, ref _Picking_Tourplan);
        set => _Picking_Tourplan = value;
    }

    public bool Picking_Tourplan_IsLoaded
    {
        get
        {
            return Picking_Tourplan != null;
        }
    }

    public virtual CollectionEntry Picking_TourplanReference
    {
        get { return Context.Entry(this).Collection(c => c.Picking_Tourplan); }
    }

    private ICollection<TourplanConfig> _TourplanConfig_Tourplan;
    public virtual ICollection<TourplanConfig> TourplanConfig_Tourplan
    {
        get => LazyLoader.Load(this, ref _TourplanConfig_Tourplan);
        set => _TourplanConfig_Tourplan = value;
    }

    public bool TourplanConfig_Tourplan_IsLoaded
    {
        get
        {
            return TourplanConfig_Tourplan != null;
        }
    }

    public virtual CollectionEntry TourplanConfig_TourplanReference
    {
        get { return Context.Entry(this).Collection(c => c.TourplanConfig_Tourplan); }
    }

    private ICollection<TourplanPos> _TourplanPo_Tourplan;
    public virtual ICollection<TourplanPos> TourplanPo_Tourplan
    {
        get => LazyLoader.Load(this, ref _TourplanPo_Tourplan);
        set => _TourplanPo_Tourplan = value;
    }

    public bool TourplanPo_Tourplan_IsLoaded
    {
        get
        {
            return TourplanPo_Tourplan != null;
        }
    }

    public virtual CollectionEntry TourplanPo_TourplanReference
    {
        get { return Context.Entry(this).Collection(c => c.TourplanPo_Tourplan); }
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
    
    private VisitorVoucher _VisitorVoucher;
    public virtual VisitorVoucher VisitorVoucher
    { 
        get => LazyLoader.Load(this, ref _VisitorVoucher);
        set => _VisitorVoucher = value;
    }

    public bool VisitorVoucher_IsLoaded
    {
        get
        {
            return VisitorVoucher != null;
        }
    }

    public virtual ReferenceEntry VisitorVoucherReference 
    {
        get { return Context.Entry(this).Reference("VisitorVoucher"); }
    }
    }
