using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class Facility : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public Facility()
    {
    }

    private Facility(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _FacilityID;
    public Guid FacilityID 
    {
        get { return _FacilityID; }
        set { SetProperty<Guid>(ref _FacilityID, value); }
    }

    Guid? _ParentFacilityID;
    public Guid? ParentFacilityID 
    {
        get { return _ParentFacilityID; }
        set { SetForeignKeyProperty<Guid?>(ref _ParentFacilityID, value, "Facility1_ParentFacility", _Facility1_ParentFacility, Facility1_ParentFacility != null ? Facility1_ParentFacility.FacilityID : default(Guid?)); }
    }

    Guid? _VBiFacilityACClassID;
    public Guid? VBiFacilityACClassID 
    {
        get { return _VBiFacilityACClassID; }
        set { SetForeignKeyProperty<Guid?>(ref _VBiFacilityACClassID, value, "VBiFacilityACClass", _VBiFacilityACClass, VBiFacilityACClass != null ? VBiFacilityACClass.ACClassID : default(Guid?)); }
    }

    string _FacilityNo;
    public string FacilityNo 
    {
        get { return _FacilityNo; }
        set { SetProperty<string>(ref _FacilityNo, value); }
    }

    string _FacilityName;
    public string FacilityName 
    {
        get { return _FacilityName; }
        set { SetProperty<string>(ref _FacilityName, value); }
    }

    Guid _MDFacilityTypeID;
    public Guid MDFacilityTypeID 
    {
        get { return _MDFacilityTypeID; }
        set { SetForeignKeyProperty<Guid>(ref _MDFacilityTypeID, value, "MDFacilityType", _MDFacilityType, MDFacilityType != null ? MDFacilityType.MDFacilityTypeID : default(Guid)); }
    }

    Guid? _MaterialID;
    public Guid? MaterialID 
    {
        get { return _MaterialID; }
        set { SetForeignKeyProperty<Guid?>(ref _MaterialID, value, "Material", _Material, Material != null ? Material.MaterialID : default(Guid?)); }
    }

    Guid? _MDUnitID;
    public Guid? MDUnitID 
    {
        get { return _MDUnitID; }
        set { SetForeignKeyProperty<Guid?>(ref _MDUnitID, value, "MDUnit", _MDUnit, MDUnit != null ? MDUnit.MDUnitID : default(Guid?)); }
    }

    Guid? _VBiStackCalculatorACClassID;
    public Guid? VBiStackCalculatorACClassID 
    {
        get { return _VBiStackCalculatorACClassID; }
        set { SetForeignKeyProperty<Guid?>(ref _VBiStackCalculatorACClassID, value, "VBiStackCalculatorACClass", _VBiStackCalculatorACClass, VBiStackCalculatorACClass != null ? VBiStackCalculatorACClass.ACClassID : default(Guid?)); }
    }

    bool _InwardEnabled;
    public bool InwardEnabled 
    {
        get { return _InwardEnabled; }
        set { SetProperty<bool>(ref _InwardEnabled, value); }
    }

    bool _OutwardEnabled;
    public bool OutwardEnabled 
    {
        get { return _OutwardEnabled; }
        set { SetProperty<bool>(ref _OutwardEnabled, value); }
    }

    int _LastFCSortNo;
    public int LastFCSortNo 
    {
        get { return _LastFCSortNo; }
        set { SetProperty<int>(ref _LastFCSortNo, value); }
    }

    int _LastFCSortNoReverse;
    public int LastFCSortNoReverse 
    {
        get { return _LastFCSortNoReverse; }
        set { SetProperty<int>(ref _LastFCSortNoReverse, value); }
    }

    Guid? _PartslistID;
    public Guid? PartslistID 
    {
        get { return _PartslistID; }
        set { SetForeignKeyProperty<Guid?>(ref _PartslistID, value, "Partslist", _Partslist, Partslist != null ? Partslist.PartslistID : default(Guid?)); }
    }

    Guid? _LockedFacilityID;
    public Guid? LockedFacilityID 
    {
        get { return _LockedFacilityID; }
        set { SetForeignKeyProperty<Guid?>(ref _LockedFacilityID, value, "Facility1_LockedFacility", _Facility1_LockedFacility, Facility1_LockedFacility != null ? Facility1_LockedFacility.FacilityID : default(Guid?)); }
    }

    Guid? _OutgoingFacilityID;
    public Guid? OutgoingFacilityID 
    {
        get { return _OutgoingFacilityID; }
        set { SetForeignKeyProperty<Guid?>(ref _OutgoingFacilityID, value, "Facility1_OutgoingFacility", _Facility1_OutgoingFacility, Facility1_OutgoingFacility != null ? Facility1_OutgoingFacility.FacilityID : default(Guid?)); }
    }

    Guid? _IncomingFacilityID;
    public Guid? IncomingFacilityID 
    {
        get { return _IncomingFacilityID; }
        set { SetForeignKeyProperty<Guid?>(ref _IncomingFacilityID, value, "Facility1_IncomingFacility", _Facility1_IncomingFacility, Facility1_IncomingFacility != null ? Facility1_IncomingFacility.FacilityID : default(Guid?)); }
    }

    double _ReservedQuantity;
    public double ReservedQuantity 
    {
        get { return _ReservedQuantity; }
        set { SetProperty<double>(ref _ReservedQuantity, value); }
    }

    double _OrderedQuantity;
    public double OrderedQuantity 
    {
        get { return _OrderedQuantity; }
        set { SetProperty<double>(ref _OrderedQuantity, value); }
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

    Guid? _CompanyID;
    public Guid? CompanyID 
    {
        get { return _CompanyID; }
        set { SetForeignKeyProperty<Guid?>(ref _CompanyID, value, "Company", _Company, Company != null ? Company.CompanyID : default(Guid?)); }
    }

    Guid? _CompanyPersonID;
    public Guid? CompanyPersonID 
    {
        get { return _CompanyPersonID; }
        set { SetForeignKeyProperty<Guid?>(ref _CompanyPersonID, value, "CompanyPerson", _CompanyPerson, CompanyPerson != null ? CompanyPerson.CompanyPersonID : default(Guid?)); }
    }

    double _Tara;
    public double Tara 
    {
        get { return _Tara; }
        set { SetProperty<double>(ref _Tara, value); }
    }

    double _MaxWeightCapacity;
    public double MaxWeightCapacity 
    {
        get { return _MaxWeightCapacity; }
        set { SetProperty<double>(ref _MaxWeightCapacity, value); }
    }

    double _MaxVolumeCapacity;
    public double MaxVolumeCapacity 
    {
        get { return _MaxVolumeCapacity; }
        set { SetProperty<double>(ref _MaxVolumeCapacity, value); }
    }

    string _Drivername;
    public string Drivername 
    {
        get { return _Drivername; }
        set { SetProperty<string>(ref _Drivername, value); }
    }

    float _Tolerance;
    public float Tolerance 
    {
        get { return _Tolerance; }
        set { SetProperty<float>(ref _Tolerance, value); }
    }

    int _HighLidNo;
    public int HighLidNo 
    {
        get { return _HighLidNo; }
        set { SetProperty<int>(ref _HighLidNo, value); }
    }

    int _FittingsDistanceFront;
    public int FittingsDistanceFront 
    {
        get { return _FittingsDistanceFront; }
        set { SetProperty<int>(ref _FittingsDistanceFront, value); }
    }

    int _FittingsDistanceBehind;
    public int FittingsDistanceBehind 
    {
        get { return _FittingsDistanceBehind; }
        set { SetProperty<int>(ref _FittingsDistanceBehind, value); }
    }

    double _DistanceFront;
    public double DistanceFront 
    {
        get { return _DistanceFront; }
        set { SetProperty<double>(ref _DistanceFront, value); }
    }

    double _DistanceBehind;
    public double DistanceBehind 
    {
        get { return _DistanceBehind; }
        set { SetProperty<double>(ref _DistanceBehind, value); }
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

    Guid? _MDFacilityVehicleTypeID;
    public Guid? MDFacilityVehicleTypeID 
    {
        get { return _MDFacilityVehicleTypeID; }
        set { SetForeignKeyProperty<Guid?>(ref _MDFacilityVehicleTypeID, value, "MDFacilityVehicleType", _MDFacilityVehicleType, MDFacilityVehicleType != null ? MDFacilityVehicleType.MDFacilityVehicleTypeID : default(Guid?)); }
    }

    double _Density;
    public double Density 
    {
        get { return _Density; }
        set { SetProperty<double>(ref _Density, value); }
    }

    double _DensityAmb;
    public double DensityAmb 
    {
        get { return _DensityAmb; }
        set { SetProperty<double>(ref _DensityAmb, value); }
    }

    double? _MinStockQuantity;
    public double? MinStockQuantity 
    {
        get { return _MinStockQuantity; }
        set { SetProperty<double?>(ref _MinStockQuantity, value); }
    }

    double? _OptStockQuantity;
    public double? OptStockQuantity 
    {
        get { return _OptStockQuantity; }
        set { SetProperty<double?>(ref _OptStockQuantity, value); }
    }

    bool _OrderPostingOnEmptying;
    public bool OrderPostingOnEmptying 
    {
        get { return _OrderPostingOnEmptying; }
        set { SetProperty<bool>(ref _OrderPostingOnEmptying, value); }
    }

    bool _DisabledForMobile;
    public bool DisabledForMobile 
    {
        get { return _DisabledForMobile; }
        set { SetProperty<bool>(ref _DisabledForMobile, value); }
    }

    string _KeyOfExtSys;
    public string KeyOfExtSys 
    {
        get { return _KeyOfExtSys; }
        set { SetProperty<string>(ref _KeyOfExtSys, value); }
    }

    short _PostingBehaviourIndex;
    public short PostingBehaviourIndex 
    {
        get { return _PostingBehaviourIndex; }
        set { SetProperty<short>(ref _PostingBehaviourIndex, value); }
    }

    bool _SkipPrintQuestion;
    public bool SkipPrintQuestion 
    {
        get { return _SkipPrintQuestion; }
        set { SetProperty<bool>(ref _SkipPrintQuestion, value); }
    }

    short _ClassCode;
    public short ClassCode 
    {
        get { return _ClassCode; }
        set { SetProperty<short>(ref _ClassCode, value); }
    }

    bool? _LeaveMaterialOccupation;
    public bool? LeaveMaterialOccupation 
    {
        get { return _LeaveMaterialOccupation; }
        set { SetProperty<bool?>(ref _LeaveMaterialOccupation, value); }
    }

    Guid? _VBiACClassMethodID;
    public Guid? VBiACClassMethodID 
    {
        get { return _VBiACClassMethodID; }
        set { SetForeignKeyProperty<Guid?>(ref _VBiACClassMethodID, value, "VBiACClassMethod", _VBiACClassMethod, VBiACClassMethod != null ? VBiACClassMethod.ACClassMethodID : default(Guid?)); }
    }

    private Company _Company;
    public virtual Company Company
    { 
        get { return LazyLoader.Load(this, ref _Company); } 
        set { SetProperty<Company>(ref _Company, value); }
    }

    public bool Company_IsLoaded
    {
        get
        {
            return _Company != null;
        }
    }

    public virtual ReferenceEntry CompanyReference 
    {
        get { return Context.Entry(this).Reference("Company"); }
    }
    
    private CompanyPerson _CompanyPerson;
    public virtual CompanyPerson CompanyPerson
    { 
        get { return LazyLoader.Load(this, ref _CompanyPerson); } 
        set { SetProperty<CompanyPerson>(ref _CompanyPerson, value); }
    }

    public bool CompanyPerson_IsLoaded
    {
        get
        {
            return _CompanyPerson != null;
        }
    }

    public virtual ReferenceEntry CompanyPersonReference 
    {
        get { return Context.Entry(this).Reference("CompanyPerson"); }
    }
    
    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_InwardFacility;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_InwardFacility
    {
        get { return LazyLoader.Load(this, ref _FacilityBookingCharge_InwardFacility); }
        set { SetProperty<ICollection<FacilityBookingCharge>>(ref _FacilityBookingCharge_InwardFacility, value); }
    }

    public bool FacilityBookingCharge_InwardFacility_IsLoaded
    {
        get
        {
            return _FacilityBookingCharge_InwardFacility != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_InwardFacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_InwardFacility); }
    }

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_InwardFacilityLocation;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_InwardFacilityLocation
    {
        get { return LazyLoader.Load(this, ref _FacilityBookingCharge_InwardFacilityLocation); }
        set { SetProperty<ICollection<FacilityBookingCharge>>(ref _FacilityBookingCharge_InwardFacilityLocation, value); }
    }

    public bool FacilityBookingCharge_InwardFacilityLocation_IsLoaded
    {
        get
        {
            return _FacilityBookingCharge_InwardFacilityLocation != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_InwardFacilityLocationReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_InwardFacilityLocation); }
    }

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_OutwardFacility;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_OutwardFacility
    {
        get { return LazyLoader.Load(this, ref _FacilityBookingCharge_OutwardFacility); }
        set { SetProperty<ICollection<FacilityBookingCharge>>(ref _FacilityBookingCharge_OutwardFacility, value); }
    }

    public bool FacilityBookingCharge_OutwardFacility_IsLoaded
    {
        get
        {
            return _FacilityBookingCharge_OutwardFacility != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_OutwardFacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_OutwardFacility); }
    }

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_OutwardFacilityLocation;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_OutwardFacilityLocation
    {
        get { return LazyLoader.Load(this, ref _FacilityBookingCharge_OutwardFacilityLocation); }
        set { SetProperty<ICollection<FacilityBookingCharge>>(ref _FacilityBookingCharge_OutwardFacilityLocation, value); }
    }

    public bool FacilityBookingCharge_OutwardFacilityLocation_IsLoaded
    {
        get
        {
            return _FacilityBookingCharge_OutwardFacilityLocation != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_OutwardFacilityLocationReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_OutwardFacilityLocation); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_InwardFacility;
    public virtual ICollection<FacilityBooking> FacilityBooking_InwardFacility
    {
        get { return LazyLoader.Load(this, ref _FacilityBooking_InwardFacility); }
        set { SetProperty<ICollection<FacilityBooking>>(ref _FacilityBooking_InwardFacility, value); }
    }

    public bool FacilityBooking_InwardFacility_IsLoaded
    {
        get
        {
            return _FacilityBooking_InwardFacility != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_InwardFacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_InwardFacility); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_InwardFacilityLocation;
    public virtual ICollection<FacilityBooking> FacilityBooking_InwardFacilityLocation
    {
        get { return LazyLoader.Load(this, ref _FacilityBooking_InwardFacilityLocation); }
        set { SetProperty<ICollection<FacilityBooking>>(ref _FacilityBooking_InwardFacilityLocation, value); }
    }

    public bool FacilityBooking_InwardFacilityLocation_IsLoaded
    {
        get
        {
            return _FacilityBooking_InwardFacilityLocation != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_InwardFacilityLocationReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_InwardFacilityLocation); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_OutwardFacility;
    public virtual ICollection<FacilityBooking> FacilityBooking_OutwardFacility
    {
        get { return LazyLoader.Load(this, ref _FacilityBooking_OutwardFacility); }
        set { SetProperty<ICollection<FacilityBooking>>(ref _FacilityBooking_OutwardFacility, value); }
    }

    public bool FacilityBooking_OutwardFacility_IsLoaded
    {
        get
        {
            return _FacilityBooking_OutwardFacility != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_OutwardFacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_OutwardFacility); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_OutwardFacilityLocation;
    public virtual ICollection<FacilityBooking> FacilityBooking_OutwardFacilityLocation
    {
        get { return LazyLoader.Load(this, ref _FacilityBooking_OutwardFacilityLocation); }
        set { SetProperty<ICollection<FacilityBooking>>(ref _FacilityBooking_OutwardFacilityLocation, value); }
    }

    public bool FacilityBooking_OutwardFacilityLocation_IsLoaded
    {
        get
        {
            return _FacilityBooking_OutwardFacilityLocation != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_OutwardFacilityLocationReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_OutwardFacilityLocation); }
    }

    private ICollection<FacilityCharge> _FacilityCharge_Facility;
    public virtual ICollection<FacilityCharge> FacilityCharge_Facility
    {
        get { return LazyLoader.Load(this, ref _FacilityCharge_Facility); }
        set { SetProperty<ICollection<FacilityCharge>>(ref _FacilityCharge_Facility, value); }
    }

    public bool FacilityCharge_Facility_IsLoaded
    {
        get
        {
            return _FacilityCharge_Facility != null;
        }
    }

    public virtual CollectionEntry FacilityCharge_FacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityCharge_Facility); }
    }

    private ICollection<FacilityHistory> _FacilityHistory_Facility;
    public virtual ICollection<FacilityHistory> FacilityHistory_Facility
    {
        get { return LazyLoader.Load(this, ref _FacilityHistory_Facility); }
        set { SetProperty<ICollection<FacilityHistory>>(ref _FacilityHistory_Facility, value); }
    }

    public bool FacilityHistory_Facility_IsLoaded
    {
        get
        {
            return _FacilityHistory_Facility != null;
        }
    }

    public virtual CollectionEntry FacilityHistory_FacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityHistory_Facility); }
    }

    private ICollection<FacilityInventory> _FacilityInventory_Facility;
    public virtual ICollection<FacilityInventory> FacilityInventory_Facility
    {
        get { return LazyLoader.Load(this, ref _FacilityInventory_Facility); }
        set { SetProperty<ICollection<FacilityInventory>>(ref _FacilityInventory_Facility, value); }
    }

    public bool FacilityInventory_Facility_IsLoaded
    {
        get
        {
            return _FacilityInventory_Facility != null;
        }
    }

    public virtual CollectionEntry FacilityInventory_FacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityInventory_Facility); }
    }

    private ICollection<FacilityMDSchedulingGroup> _FacilityMDSchedulingGroup_Facility;
    public virtual ICollection<FacilityMDSchedulingGroup> FacilityMDSchedulingGroup_Facility
    {
        get { return LazyLoader.Load(this, ref _FacilityMDSchedulingGroup_Facility); }
        set { SetProperty<ICollection<FacilityMDSchedulingGroup>>(ref _FacilityMDSchedulingGroup_Facility, value); }
    }

    public bool FacilityMDSchedulingGroup_Facility_IsLoaded
    {
        get
        {
            return _FacilityMDSchedulingGroup_Facility != null;
        }
    }

    public virtual CollectionEntry FacilityMDSchedulingGroup_FacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityMDSchedulingGroup_Facility); }
    }

    private ICollection<FacilityMaterial> _FacilityMaterial_Facility;
    public virtual ICollection<FacilityMaterial> FacilityMaterial_Facility
    {
        get { return LazyLoader.Load(this, ref _FacilityMaterial_Facility); }
        set { SetProperty<ICollection<FacilityMaterial>>(ref _FacilityMaterial_Facility, value); }
    }

    public bool FacilityMaterial_Facility_IsLoaded
    {
        get
        {
            return _FacilityMaterial_Facility != null;
        }
    }

    public virtual CollectionEntry FacilityMaterial_FacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityMaterial_Facility); }
    }

    private ICollection<FacilityReservation> _FacilityReservation_Facility;
    public virtual ICollection<FacilityReservation> FacilityReservation_Facility
    {
        get { return LazyLoader.Load(this, ref _FacilityReservation_Facility); }
        set { SetProperty<ICollection<FacilityReservation>>(ref _FacilityReservation_Facility, value); }
    }

    public bool FacilityReservation_Facility_IsLoaded
    {
        get
        {
            return _FacilityReservation_Facility != null;
        }
    }

    public virtual CollectionEntry FacilityReservation_FacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityReservation_Facility); }
    }

    private ICollection<FacilityStock> _FacilityStock_Facility;
    public virtual ICollection<FacilityStock> FacilityStock_Facility
    {
        get { return LazyLoader.Load(this, ref _FacilityStock_Facility); }
        set { SetProperty<ICollection<FacilityStock>>(ref _FacilityStock_Facility, value); }
    }

    public bool FacilityStock_Facility_IsLoaded
    {
        get
        {
            return _FacilityStock_Facility != null;
        }
    }

    public virtual CollectionEntry FacilityStock_FacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityStock_Facility); }
    }

    private Facility _Facility1_IncomingFacility;
    public virtual Facility Facility1_IncomingFacility
    { 
        get { return LazyLoader.Load(this, ref _Facility1_IncomingFacility); } 
        set { SetProperty<Facility>(ref _Facility1_IncomingFacility, value); }
    }

    public bool Facility1_IncomingFacility_IsLoaded
    {
        get
        {
            return _Facility1_IncomingFacility != null;
        }
    }

    public virtual ReferenceEntry Facility1_IncomingFacilityReference 
    {
        get { return Context.Entry(this).Reference("Facility1_IncomingFacility"); }
    }
    
    private ICollection<Facility> _Facility_IncomingFacility;
    public virtual ICollection<Facility> Facility_IncomingFacility
    {
        get { return LazyLoader.Load(this, ref _Facility_IncomingFacility); }
        set { SetProperty<ICollection<Facility>>(ref _Facility_IncomingFacility, value); }
    }

    public bool Facility_IncomingFacility_IsLoaded
    {
        get
        {
            return _Facility_IncomingFacility != null;
        }
    }

    public virtual CollectionEntry Facility_IncomingFacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.Facility_IncomingFacility); }
    }

    private ICollection<Facility> _Facility_LockedFacility;
    public virtual ICollection<Facility> Facility_LockedFacility
    {
        get { return LazyLoader.Load(this, ref _Facility_LockedFacility); }
        set { SetProperty<ICollection<Facility>>(ref _Facility_LockedFacility, value); }
    }

    public bool Facility_LockedFacility_IsLoaded
    {
        get
        {
            return _Facility_LockedFacility != null;
        }
    }

    public virtual CollectionEntry Facility_LockedFacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.Facility_LockedFacility); }
    }

    private ICollection<Facility> _Facility_OutgoingFacility;
    public virtual ICollection<Facility> Facility_OutgoingFacility
    {
        get { return LazyLoader.Load(this, ref _Facility_OutgoingFacility); }
        set { SetProperty<ICollection<Facility>>(ref _Facility_OutgoingFacility, value); }
    }

    public bool Facility_OutgoingFacility_IsLoaded
    {
        get
        {
            return _Facility_OutgoingFacility != null;
        }
    }

    public virtual CollectionEntry Facility_OutgoingFacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.Facility_OutgoingFacility); }
    }

    private ICollection<Facility> _Facility_ParentFacility;
    public virtual ICollection<Facility> Facility_ParentFacility
    {
        get { return LazyLoader.Load(this, ref _Facility_ParentFacility); }
        set { SetProperty<ICollection<Facility>>(ref _Facility_ParentFacility, value); }
    }

    public bool Facility_ParentFacility_IsLoaded
    {
        get
        {
            return _Facility_ParentFacility != null;
        }
    }

    public virtual CollectionEntry Facility_ParentFacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.Facility_ParentFacility); }
    }

    private Facility _Facility1_LockedFacility;
    public virtual Facility Facility1_LockedFacility
    { 
        get { return LazyLoader.Load(this, ref _Facility1_LockedFacility); } 
        set { SetProperty<Facility>(ref _Facility1_LockedFacility, value); }
    }

    public bool Facility1_LockedFacility_IsLoaded
    {
        get
        {
            return _Facility1_LockedFacility != null;
        }
    }

    public virtual ReferenceEntry Facility1_LockedFacilityReference 
    {
        get { return Context.Entry(this).Reference("Facility1_LockedFacility"); }
    }
    
    private MDFacilityType _MDFacilityType;
    public virtual MDFacilityType MDFacilityType
    { 
        get { return LazyLoader.Load(this, ref _MDFacilityType); } 
        set { SetProperty<MDFacilityType>(ref _MDFacilityType, value); }
    }

    public bool MDFacilityType_IsLoaded
    {
        get
        {
            return _MDFacilityType != null;
        }
    }

    public virtual ReferenceEntry MDFacilityTypeReference 
    {
        get { return Context.Entry(this).Reference("MDFacilityType"); }
    }
    
    private MDFacilityVehicleType _MDFacilityVehicleType;
    public virtual MDFacilityVehicleType MDFacilityVehicleType
    { 
        get { return LazyLoader.Load(this, ref _MDFacilityVehicleType); } 
        set { SetProperty<MDFacilityVehicleType>(ref _MDFacilityVehicleType, value); }
    }

    public bool MDFacilityVehicleType_IsLoaded
    {
        get
        {
            return _MDFacilityVehicleType != null;
        }
    }

    public virtual ReferenceEntry MDFacilityVehicleTypeReference 
    {
        get { return Context.Entry(this).Reference("MDFacilityVehicleType"); }
    }
    
    private MDUnit _MDUnit;
    public virtual MDUnit MDUnit
    { 
        get { return LazyLoader.Load(this, ref _MDUnit); } 
        set { SetProperty<MDUnit>(ref _MDUnit, value); }
    }

    public bool MDUnit_IsLoaded
    {
        get
        {
            return _MDUnit != null;
        }
    }

    public virtual ReferenceEntry MDUnitReference 
    {
        get { return Context.Entry(this).Reference("MDUnit"); }
    }
    
    private ICollection<MaintOrder> _MaintOrder_Facility;
    public virtual ICollection<MaintOrder> MaintOrder_Facility
    {
        get { return LazyLoader.Load(this, ref _MaintOrder_Facility); }
        set { SetProperty<ICollection<MaintOrder>>(ref _MaintOrder_Facility, value); }
    }

    public bool MaintOrder_Facility_IsLoaded
    {
        get
        {
            return _MaintOrder_Facility != null;
        }
    }

    public virtual CollectionEntry MaintOrder_FacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintOrder_Facility); }
    }

    private Material _Material;
    public virtual Material Material
    { 
        get { return LazyLoader.Load(this, ref _Material); } 
        set { SetProperty<Material>(ref _Material, value); }
    }

    public bool Material_IsLoaded
    {
        get
        {
            return _Material != null;
        }
    }

    public virtual ReferenceEntry MaterialReference 
    {
        get { return Context.Entry(this).Reference("Material"); }
    }
    
    private ICollection<Material> _Material_InFacility;
    public virtual ICollection<Material> Material_InFacility
    {
        get { return LazyLoader.Load(this, ref _Material_InFacility); }
        set { SetProperty<ICollection<Material>>(ref _Material_InFacility, value); }
    }

    public bool Material_InFacility_IsLoaded
    {
        get
        {
            return _Material_InFacility != null;
        }
    }

    public virtual CollectionEntry Material_InFacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.Material_InFacility); }
    }

    private ICollection<Material> _Material_OutFacility;
    public virtual ICollection<Material> Material_OutFacility
    {
        get { return LazyLoader.Load(this, ref _Material_OutFacility); }
        set { SetProperty<ICollection<Material>>(ref _Material_OutFacility, value); }
    }

    public bool Material_OutFacility_IsLoaded
    {
        get
        {
            return _Material_OutFacility != null;
        }
    }

    public virtual CollectionEntry Material_OutFacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.Material_OutFacility); }
    }

    private Facility _Facility1_OutgoingFacility;
    public virtual Facility Facility1_OutgoingFacility
    { 
        get { return LazyLoader.Load(this, ref _Facility1_OutgoingFacility); } 
        set { SetProperty<Facility>(ref _Facility1_OutgoingFacility, value); }
    }

    public bool Facility1_OutgoingFacility_IsLoaded
    {
        get
        {
            return _Facility1_OutgoingFacility != null;
        }
    }

    public virtual ReferenceEntry Facility1_OutgoingFacilityReference 
    {
        get { return Context.Entry(this).Reference("Facility1_OutgoingFacility"); }
    }
    
    private Facility _Facility1_ParentFacility;
    public virtual Facility Facility1_ParentFacility
    { 
        get { return LazyLoader.Load(this, ref _Facility1_ParentFacility); } 
        set { SetProperty<Facility>(ref _Facility1_ParentFacility, value); }
    }

    public bool Facility1_ParentFacility_IsLoaded
    {
        get
        {
            return _Facility1_ParentFacility != null;
        }
    }

    public virtual ReferenceEntry Facility1_ParentFacilityReference 
    {
        get { return Context.Entry(this).Reference("Facility1_ParentFacility"); }
    }
    
    private Partslist _Partslist;
    public virtual Partslist Partslist
    { 
        get { return LazyLoader.Load(this, ref _Partslist); } 
        set { SetProperty<Partslist>(ref _Partslist, value); }
    }

    public bool Partslist_IsLoaded
    {
        get
        {
            return _Partslist != null;
        }
    }

    public virtual ReferenceEntry PartslistReference 
    {
        get { return Context.Entry(this).Reference("Partslist"); }
    }
    
    private ICollection<PickingPos> _PickingPos_FromFacility;
    public virtual ICollection<PickingPos> PickingPos_FromFacility
    {
        get { return LazyLoader.Load(this, ref _PickingPos_FromFacility); }
        set { SetProperty<ICollection<PickingPos>>(ref _PickingPos_FromFacility, value); }
    }

    public bool PickingPos_FromFacility_IsLoaded
    {
        get
        {
            return _PickingPos_FromFacility != null;
        }
    }

    public virtual CollectionEntry PickingPos_FromFacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.PickingPos_FromFacility); }
    }

    private ICollection<PickingPos> _PickingPos_ToFacility;
    public virtual ICollection<PickingPos> PickingPos_ToFacility
    {
        get { return LazyLoader.Load(this, ref _PickingPos_ToFacility); }
        set { SetProperty<ICollection<PickingPos>>(ref _PickingPos_ToFacility, value); }
    }

    public bool PickingPos_ToFacility_IsLoaded
    {
        get
        {
            return _PickingPos_ToFacility != null;
        }
    }

    public virtual CollectionEntry PickingPos_ToFacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.PickingPos_ToFacility); }
    }

    private ICollection<TandTv3MixPointFacility> _TandTv3MixPointFacility_Facility;
    public virtual ICollection<TandTv3MixPointFacility> TandTv3MixPointFacility_Facility
    {
        get { return LazyLoader.Load(this, ref _TandTv3MixPointFacility_Facility); }
        set { SetProperty<ICollection<TandTv3MixPointFacility>>(ref _TandTv3MixPointFacility_Facility, value); }
    }

    public bool TandTv3MixPointFacility_Facility_IsLoaded
    {
        get
        {
            return _TandTv3MixPointFacility_Facility != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointFacility_FacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointFacility_Facility); }
    }

    private ICollection<Tourplan> _Tourplan_TrailerFacility;
    public virtual ICollection<Tourplan> Tourplan_TrailerFacility
    {
        get { return LazyLoader.Load(this, ref _Tourplan_TrailerFacility); }
        set { SetProperty<ICollection<Tourplan>>(ref _Tourplan_TrailerFacility, value); }
    }

    public bool Tourplan_TrailerFacility_IsLoaded
    {
        get
        {
            return _Tourplan_TrailerFacility != null;
        }
    }

    public virtual CollectionEntry Tourplan_TrailerFacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.Tourplan_TrailerFacility); }
    }

    private ICollection<Tourplan> _Tourplan_VehicleFacility;
    public virtual ICollection<Tourplan> Tourplan_VehicleFacility
    {
        get { return LazyLoader.Load(this, ref _Tourplan_VehicleFacility); }
        set { SetProperty<ICollection<Tourplan>>(ref _Tourplan_VehicleFacility, value); }
    }

    public bool Tourplan_VehicleFacility_IsLoaded
    {
        get
        {
            return _Tourplan_VehicleFacility != null;
        }
    }

    public virtual CollectionEntry Tourplan_VehicleFacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.Tourplan_VehicleFacility); }
    }

    private ACClassMethod _VBiACClassMethod;
    public virtual ACClassMethod VBiACClassMethod
    { 
        get { return LazyLoader.Load(this, ref _VBiACClassMethod); } 
        set { SetProperty<ACClassMethod>(ref _VBiACClassMethod, value); }
    }

    public bool VBiACClassMethod_IsLoaded
    {
        get
        {
            return _VBiACClassMethod != null;
        }
    }

    public virtual ReferenceEntry VBiACClassMethodReference 
    {
        get { return Context.Entry(this).Reference("VBiACClassMethod"); }
    }
    
    private ACClass _VBiFacilityACClass;
    public virtual ACClass VBiFacilityACClass
    { 
        get { return LazyLoader.Load(this, ref _VBiFacilityACClass); } 
        set { SetProperty<ACClass>(ref _VBiFacilityACClass, value); }
    }

    public bool VBiFacilityACClass_IsLoaded
    {
        get
        {
            return _VBiFacilityACClass != null;
        }
    }

    public virtual ReferenceEntry VBiFacilityACClassReference 
    {
        get { return Context.Entry(this).Reference("VBiFacilityACClass"); }
    }
    
    private ACClass _VBiStackCalculatorACClass;
    public virtual ACClass VBiStackCalculatorACClass
    { 
        get { return LazyLoader.Load(this, ref _VBiStackCalculatorACClass); } 
        set { SetProperty<ACClass>(ref _VBiStackCalculatorACClass, value); }
    }

    public bool VBiStackCalculatorACClass_IsLoaded
    {
        get
        {
            return _VBiStackCalculatorACClass != null;
        }
    }

    public virtual ReferenceEntry VBiStackCalculatorACClassReference 
    {
        get { return Context.Entry(this).Reference("VBiStackCalculatorACClass"); }
    }
    
    private ICollection<Visitor> _Visitor_TrailerFacility;
    public virtual ICollection<Visitor> Visitor_TrailerFacility
    {
        get { return LazyLoader.Load(this, ref _Visitor_TrailerFacility); }
        set { SetProperty<ICollection<Visitor>>(ref _Visitor_TrailerFacility, value); }
    }

    public bool Visitor_TrailerFacility_IsLoaded
    {
        get
        {
            return _Visitor_TrailerFacility != null;
        }
    }

    public virtual CollectionEntry Visitor_TrailerFacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.Visitor_TrailerFacility); }
    }

    private ICollection<Visitor> _Visitor_VehicleFacility;
    public virtual ICollection<Visitor> Visitor_VehicleFacility
    {
        get { return LazyLoader.Load(this, ref _Visitor_VehicleFacility); }
        set { SetProperty<ICollection<Visitor>>(ref _Visitor_VehicleFacility, value); }
    }

    public bool Visitor_VehicleFacility_IsLoaded
    {
        get
        {
            return _Visitor_VehicleFacility != null;
        }
    }

    public virtual CollectionEntry Visitor_VehicleFacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.Visitor_VehicleFacility); }
    }

    private ICollection<VisitorVoucher> _VisitorVoucher_TrailerFacility;
    public virtual ICollection<VisitorVoucher> VisitorVoucher_TrailerFacility
    {
        get { return LazyLoader.Load(this, ref _VisitorVoucher_TrailerFacility); }
        set { SetProperty<ICollection<VisitorVoucher>>(ref _VisitorVoucher_TrailerFacility, value); }
    }

    public bool VisitorVoucher_TrailerFacility_IsLoaded
    {
        get
        {
            return _VisitorVoucher_TrailerFacility != null;
        }
    }

    public virtual CollectionEntry VisitorVoucher_TrailerFacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.VisitorVoucher_TrailerFacility); }
    }

    private ICollection<VisitorVoucher> _VisitorVoucher_VehicleFacility;
    public virtual ICollection<VisitorVoucher> VisitorVoucher_VehicleFacility
    {
        get { return LazyLoader.Load(this, ref _VisitorVoucher_VehicleFacility); }
        set { SetProperty<ICollection<VisitorVoucher>>(ref _VisitorVoucher_VehicleFacility, value); }
    }

    public bool VisitorVoucher_VehicleFacility_IsLoaded
    {
        get
        {
            return _VisitorVoucher_VehicleFacility != null;
        }
    }

    public virtual CollectionEntry VisitorVoucher_VehicleFacilityReference
    {
        get { return Context.Entry(this).Collection(c => c.VisitorVoucher_VehicleFacility); }
    }
}
