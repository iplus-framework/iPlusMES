using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class OutOrderPos : VBEntityObject , IInsertInfo, IUpdateInfo, ISequence, ITargetQuantity
{

    public OutOrderPos()
    {
    }

    private OutOrderPos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _OutOrderPosID;
    public Guid OutOrderPosID 
    {
        get { return _OutOrderPosID; }
        set { SetProperty<Guid>(ref _OutOrderPosID, value); }
    }

    Guid _OutOrderID;
    public Guid OutOrderID 
    {
        get { return _OutOrderID; }
        set { SetProperty<Guid>(ref _OutOrderID, value); }
    }

    Guid? _ParentOutOrderPosID;
    public Guid? ParentOutOrderPosID 
    {
        get { return _ParentOutOrderPosID; }
        set { SetProperty<Guid?>(ref _ParentOutOrderPosID, value); }
    }

    Guid? _MDTimeRangeID;
    public Guid? MDTimeRangeID 
    {
        get { return _MDTimeRangeID; }
        set { SetProperty<Guid?>(ref _MDTimeRangeID, value); }
    }

    Guid _MDOutOrderPosStateID;
    public Guid MDOutOrderPosStateID 
    {
        get { return _MDOutOrderPosStateID; }
        set { SetProperty<Guid>(ref _MDOutOrderPosStateID, value); }
    }

    Guid _MDDelivPosStateID;
    public Guid MDDelivPosStateID 
    {
        get { return _MDDelivPosStateID; }
        set { SetProperty<Guid>(ref _MDDelivPosStateID, value); }
    }

    Guid? _MDDelivPosLoadStateID;
    public Guid? MDDelivPosLoadStateID 
    {
        get { return _MDDelivPosLoadStateID; }
        set { SetProperty<Guid?>(ref _MDDelivPosLoadStateID, value); }
    }

    Guid? _MDTransportModeID;
    public Guid? MDTransportModeID 
    {
        get { return _MDTransportModeID; }
        set { SetProperty<Guid?>(ref _MDTransportModeID, value); }
    }

    Guid _MDToleranceStateID;
    public Guid MDToleranceStateID 
    {
        get { return _MDToleranceStateID; }
        set { SetProperty<Guid>(ref _MDToleranceStateID, value); }
    }

    Guid? _MDOutOrderPlanStateID;
    public Guid? MDOutOrderPlanStateID 
    {
        get { return _MDOutOrderPlanStateID; }
        set { SetProperty<Guid?>(ref _MDOutOrderPlanStateID, value); }
    }

    Guid? _MDTourplanPosStateID;
    public Guid? MDTourplanPosStateID 
    {
        get { return _MDTourplanPosStateID; }
        set { SetProperty<Guid?>(ref _MDTourplanPosStateID, value); }
    }

    Guid? _CompanyAddressUnloadingpointID;
    public Guid? CompanyAddressUnloadingpointID 
    {
        get { return _CompanyAddressUnloadingpointID; }
        set { SetProperty<Guid?>(ref _CompanyAddressUnloadingpointID, value); }
    }

    Guid? _PickupCompanyMaterialID;
    public Guid? PickupCompanyMaterialID 
    {
        get { return _PickupCompanyMaterialID; }
        set { SetProperty<Guid?>(ref _PickupCompanyMaterialID, value); }
    }

    string _LineNumber;
    public string LineNumber 
    {
        get { return _LineNumber; }
        set { SetProperty<string>(ref _LineNumber, value); }
    }

    short _MaterialPosTypeIndex;
    public short MaterialPosTypeIndex 
    {
        get { return _MaterialPosTypeIndex; }
        set { SetProperty<short>(ref _MaterialPosTypeIndex, value); }
    }

    Guid? _MDCountrySalesTaxID;
    public Guid? MDCountrySalesTaxID 
    {
        get { return _MDCountrySalesTaxID; }
        set { SetProperty<Guid?>(ref _MDCountrySalesTaxID, value); }
    }

    Guid? _MDCountrySalesTaxMDMaterialGroupID;
    public Guid? MDCountrySalesTaxMDMaterialGroupID 
    {
        get { return _MDCountrySalesTaxMDMaterialGroupID; }
        set { SetProperty<Guid?>(ref _MDCountrySalesTaxMDMaterialGroupID, value); }
    }

    Guid? _MDCountrySalesTaxMaterialID;
    public Guid? MDCountrySalesTaxMaterialID 
    {
        get { return _MDCountrySalesTaxMaterialID; }
        set { SetProperty<Guid?>(ref _MDCountrySalesTaxMaterialID, value); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    Guid _MaterialID;
    public Guid MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid>(ref _MaterialID, value); }
    }

    Guid? _MDUnitID;
    public Guid? MDUnitID 
    {
        get { return _MDUnitID; }
        set { SetProperty<Guid?>(ref _MDUnitID, value); }
    }

    double _TargetQuantityUOM;
    public double TargetQuantityUOM 
    {
        get { return _TargetQuantityUOM; }
        set { SetProperty<double>(ref _TargetQuantityUOM, value); }
    }

    double _TargetQuantity;
    public double TargetQuantity 
    {
        get { return _TargetQuantity; }
        set { SetProperty<double>(ref _TargetQuantity, value); }
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

    decimal _SalesTax;
    public decimal SalesTax 
    {
        get { return _SalesTax; }
        set { SetProperty<decimal>(ref _SalesTax, value); }
    }

    double _ActualQuantityUOM;
    public double ActualQuantityUOM 
    {
        get { return _ActualQuantityUOM; }
        set { SetProperty<double>(ref _ActualQuantityUOM, value); }
    }

    double _ActualQuantity;
    public double ActualQuantity 
    {
        get { return _ActualQuantity; }
        set { SetProperty<double>(ref _ActualQuantity, value); }
    }

    double _CalledUpQuantityUOM;
    public double CalledUpQuantityUOM 
    {
        get { return _CalledUpQuantityUOM; }
        set { SetProperty<double>(ref _CalledUpQuantityUOM, value); }
    }

    double _CalledUpQuantity;
    public double CalledUpQuantity 
    {
        get { return _CalledUpQuantity; }
        set { SetProperty<double>(ref _CalledUpQuantity, value); }
    }

    double _ExternQuantityUOM;
    public double ExternQuantityUOM 
    {
        get { return _ExternQuantityUOM; }
        set { SetProperty<double>(ref _ExternQuantityUOM, value); }
    }

    double _ExternQuantity;
    public double ExternQuantity 
    {
        get { return _ExternQuantity; }
        set { SetProperty<double>(ref _ExternQuantity, value); }
    }

    bool _GroupSum;
    public bool GroupSum 
    {
        get { return _GroupSum; }
        set { SetProperty<bool>(ref _GroupSum, value); }
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

    short _TargetDeliveryPriority;
    public short TargetDeliveryPriority 
    {
        get { return _TargetDeliveryPriority; }
        set { SetProperty<short>(ref _TargetDeliveryPriority, value); }
    }

    DateTime? _TargetDeliveryDateConfirmed;
    public DateTime? TargetDeliveryDateConfirmed 
    {
        get { return _TargetDeliveryDateConfirmed; }
        set { SetProperty<DateTime?>(ref _TargetDeliveryDateConfirmed, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
    }

    string _Comment2;
    public string Comment2 
    {
        get { return _Comment2; }
        set { SetProperty<string>(ref _Comment2, value); }
    }

    string _XMLDesign;
    public string XMLDesign 
    {
        get { return _XMLDesign; }
        set { SetProperty<string>(ref _XMLDesign, value); }
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

    Guid? _GroupOutOrderPosID;
    public Guid? GroupOutOrderPosID 
    {
        get { return _GroupOutOrderPosID; }
        set { SetProperty<Guid?>(ref _GroupOutOrderPosID, value); }
    }

    string _KeyOfExtSys;
    public string KeyOfExtSys 
    {
        get { return _KeyOfExtSys; }
        set { SetProperty<string>(ref _KeyOfExtSys, value); }
    }

    private CompanyAddressUnloadingpoint _CompanyAddressUnloadingpoint;
    public virtual CompanyAddressUnloadingpoint CompanyAddressUnloadingpoint
    { 
        get => LazyLoader.Load(this, ref _CompanyAddressUnloadingpoint);
        set => _CompanyAddressUnloadingpoint = value;
    }

    public bool CompanyAddressUnloadingpoint_IsLoaded
    {
        get
        {
            return CompanyAddressUnloadingpoint != null;
        }
    }

    public virtual ReferenceEntry CompanyAddressUnloadingpointReference 
    {
        get { return Context.Entry(this).Reference("CompanyAddressUnloadingpoint"); }
    }
    
    private ICollection<CompanyMaterialPickup> _CompanyMaterialPickup_OutOrderPos;
    public virtual ICollection<CompanyMaterialPickup> CompanyMaterialPickup_OutOrderPos
    {
        get => LazyLoader.Load(this, ref _CompanyMaterialPickup_OutOrderPos);
        set => _CompanyMaterialPickup_OutOrderPos = value;
    }

    public bool CompanyMaterialPickup_OutOrderPos_IsLoaded
    {
        get
        {
            return CompanyMaterialPickup_OutOrderPos != null;
        }
    }

    public virtual CollectionEntry CompanyMaterialPickup_OutOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyMaterialPickup_OutOrderPos); }
    }

    private ICollection<DeliveryNotePos> _DeliveryNotePos_OutOrderPos;
    public virtual ICollection<DeliveryNotePos> DeliveryNotePos_OutOrderPos
    {
        get => LazyLoader.Load(this, ref _DeliveryNotePos_OutOrderPos);
        set => _DeliveryNotePos_OutOrderPos = value;
    }

    public bool DeliveryNotePos_OutOrderPos_IsLoaded
    {
        get
        {
            return DeliveryNotePos_OutOrderPos != null;
        }
    }

    public virtual CollectionEntry DeliveryNotePos_OutOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.DeliveryNotePos_OutOrderPos); }
    }

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_OutOrderPos;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_OutOrderPos
    {
        get => LazyLoader.Load(this, ref _FacilityBookingCharge_OutOrderPos);
        set => _FacilityBookingCharge_OutOrderPos = value;
    }

    public bool FacilityBookingCharge_OutOrderPos_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_OutOrderPos != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_OutOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_OutOrderPos); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_OutOrderPos;
    public virtual ICollection<FacilityBooking> FacilityBooking_OutOrderPos
    {
        get => LazyLoader.Load(this, ref _FacilityBooking_OutOrderPos);
        set => _FacilityBooking_OutOrderPos = value;
    }

    public bool FacilityBooking_OutOrderPos_IsLoaded
    {
        get
        {
            return FacilityBooking_OutOrderPos != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_OutOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_OutOrderPos); }
    }

    private ICollection<FacilityPreBooking> _FacilityPreBooking_OutOrderPos;
    public virtual ICollection<FacilityPreBooking> FacilityPreBooking_OutOrderPos
    {
        get => LazyLoader.Load(this, ref _FacilityPreBooking_OutOrderPos);
        set => _FacilityPreBooking_OutOrderPos = value;
    }

    public bool FacilityPreBooking_OutOrderPos_IsLoaded
    {
        get
        {
            return FacilityPreBooking_OutOrderPos != null;
        }
    }

    public virtual CollectionEntry FacilityPreBooking_OutOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityPreBooking_OutOrderPos); }
    }

    private ICollection<FacilityReservation> _FacilityReservation_OutOrderPos;
    public virtual ICollection<FacilityReservation> FacilityReservation_OutOrderPos
    {
        get => LazyLoader.Load(this, ref _FacilityReservation_OutOrderPos);
        set => _FacilityReservation_OutOrderPos = value;
    }

    public bool FacilityReservation_OutOrderPos_IsLoaded
    {
        get
        {
            return FacilityReservation_OutOrderPos != null;
        }
    }

    public virtual CollectionEntry FacilityReservation_OutOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityReservation_OutOrderPos); }
    }

    private OutOrderPos _OutOrderPos1_GroupOutOrderPos;
    public virtual OutOrderPos OutOrderPos1_GroupOutOrderPos
    { 
        get => LazyLoader.Load(this, ref _OutOrderPos1_GroupOutOrderPos);
        set => _OutOrderPos1_GroupOutOrderPos = value;
    }

    public bool OutOrderPos1_GroupOutOrderPos_IsLoaded
    {
        get
        {
            return OutOrderPos1_GroupOutOrderPos != null;
        }
    }

    public virtual ReferenceEntry OutOrderPos1_GroupOutOrderPosReference 
    {
        get { return Context.Entry(this).Reference("OutOrderPos1_GroupOutOrderPos"); }
    }
    
    private ICollection<OutOrderPos> _OutOrderPos_GroupOutOrderPos;
    public virtual ICollection<OutOrderPos> OutOrderPos_GroupOutOrderPos
    {
        get => LazyLoader.Load(this, ref _OutOrderPos_GroupOutOrderPos);
        set => _OutOrderPos_GroupOutOrderPos = value;
    }

    public bool OutOrderPos_GroupOutOrderPos_IsLoaded
    {
        get
        {
            return OutOrderPos_GroupOutOrderPos != null;
        }
    }

    public virtual CollectionEntry OutOrderPos_GroupOutOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPos_GroupOutOrderPos); }
    }

    private ICollection<OutOrderPos> _OutOrderPos_ParentOutOrderPos;
    public virtual ICollection<OutOrderPos> OutOrderPos_ParentOutOrderPos
    {
        get => LazyLoader.Load(this, ref _OutOrderPos_ParentOutOrderPos);
        set => _OutOrderPos_ParentOutOrderPos = value;
    }

    public bool OutOrderPos_ParentOutOrderPos_IsLoaded
    {
        get
        {
            return OutOrderPos_ParentOutOrderPos != null;
        }
    }

    public virtual CollectionEntry OutOrderPos_ParentOutOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPos_ParentOutOrderPos); }
    }

    private ICollection<InvoicePos> _InvoicePos_OutOrderPos;
    public virtual ICollection<InvoicePos> InvoicePos_OutOrderPos
    {
        get => LazyLoader.Load(this, ref _InvoicePos_OutOrderPos);
        set => _InvoicePos_OutOrderPos = value;
    }

    public bool InvoicePos_OutOrderPos_IsLoaded
    {
        get
        {
            return InvoicePos_OutOrderPos != null;
        }
    }

    public virtual CollectionEntry InvoicePos_OutOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.InvoicePos_OutOrderPos); }
    }

    private ICollection<LabOrder> _LabOrder_OutOrderPos;
    public virtual ICollection<LabOrder> LabOrder_OutOrderPos
    {
        get => LazyLoader.Load(this, ref _LabOrder_OutOrderPos);
        set => _LabOrder_OutOrderPos = value;
    }

    public bool LabOrder_OutOrderPos_IsLoaded
    {
        get
        {
            return LabOrder_OutOrderPos != null;
        }
    }

    public virtual CollectionEntry LabOrder_OutOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.LabOrder_OutOrderPos); }
    }

    private MDCountrySalesTax _MDCountrySalesTax;
    public virtual MDCountrySalesTax MDCountrySalesTax
    { 
        get => LazyLoader.Load(this, ref _MDCountrySalesTax);
        set => _MDCountrySalesTax = value;
    }

    public bool MDCountrySalesTax_IsLoaded
    {
        get
        {
            return MDCountrySalesTax != null;
        }
    }

    public virtual ReferenceEntry MDCountrySalesTaxReference 
    {
        get { return Context.Entry(this).Reference("MDCountrySalesTax"); }
    }
    
    private MDCountrySalesTaxMDMaterialGroup _MDCountrySalesTaxMDMaterialGroup;
    public virtual MDCountrySalesTaxMDMaterialGroup MDCountrySalesTaxMDMaterialGroup
    { 
        get => LazyLoader.Load(this, ref _MDCountrySalesTaxMDMaterialGroup);
        set => _MDCountrySalesTaxMDMaterialGroup = value;
    }

    public bool MDCountrySalesTaxMDMaterialGroup_IsLoaded
    {
        get
        {
            return MDCountrySalesTaxMDMaterialGroup != null;
        }
    }

    public virtual ReferenceEntry MDCountrySalesTaxMDMaterialGroupReference 
    {
        get { return Context.Entry(this).Reference("MDCountrySalesTaxMDMaterialGroup"); }
    }
    
    private MDCountrySalesTaxMaterial _MDCountrySalesTaxMaterial;
    public virtual MDCountrySalesTaxMaterial MDCountrySalesTaxMaterial
    { 
        get => LazyLoader.Load(this, ref _MDCountrySalesTaxMaterial);
        set => _MDCountrySalesTaxMaterial = value;
    }

    public bool MDCountrySalesTaxMaterial_IsLoaded
    {
        get
        {
            return MDCountrySalesTaxMaterial != null;
        }
    }

    public virtual ReferenceEntry MDCountrySalesTaxMaterialReference 
    {
        get { return Context.Entry(this).Reference("MDCountrySalesTaxMaterial"); }
    }
    
    private MDDelivPosLoadState _MDDelivPosLoadState;
    public virtual MDDelivPosLoadState MDDelivPosLoadState
    { 
        get => LazyLoader.Load(this, ref _MDDelivPosLoadState);
        set => _MDDelivPosLoadState = value;
    }

    public bool MDDelivPosLoadState_IsLoaded
    {
        get
        {
            return MDDelivPosLoadState != null;
        }
    }

    public virtual ReferenceEntry MDDelivPosLoadStateReference 
    {
        get { return Context.Entry(this).Reference("MDDelivPosLoadState"); }
    }
    
    private MDDelivPosState _MDDelivPosState;
    public virtual MDDelivPosState MDDelivPosState
    { 
        get => LazyLoader.Load(this, ref _MDDelivPosState);
        set => _MDDelivPosState = value;
    }

    public bool MDDelivPosState_IsLoaded
    {
        get
        {
            return MDDelivPosState != null;
        }
    }

    public virtual ReferenceEntry MDDelivPosStateReference 
    {
        get { return Context.Entry(this).Reference("MDDelivPosState"); }
    }
    
    private MDOutOrderPlanState _MDOutOrderPlanState;
    public virtual MDOutOrderPlanState MDOutOrderPlanState
    { 
        get => LazyLoader.Load(this, ref _MDOutOrderPlanState);
        set => _MDOutOrderPlanState = value;
    }

    public bool MDOutOrderPlanState_IsLoaded
    {
        get
        {
            return MDOutOrderPlanState != null;
        }
    }

    public virtual ReferenceEntry MDOutOrderPlanStateReference 
    {
        get { return Context.Entry(this).Reference("MDOutOrderPlanState"); }
    }
    
    private MDOutOrderPosState _MDOutOrderPosState;
    public virtual MDOutOrderPosState MDOutOrderPosState
    { 
        get => LazyLoader.Load(this, ref _MDOutOrderPosState);
        set => _MDOutOrderPosState = value;
    }

    public bool MDOutOrderPosState_IsLoaded
    {
        get
        {
            return MDOutOrderPosState != null;
        }
    }

    public virtual ReferenceEntry MDOutOrderPosStateReference 
    {
        get { return Context.Entry(this).Reference("MDOutOrderPosState"); }
    }
    
    private MDTimeRange _MDTimeRange;
    public virtual MDTimeRange MDTimeRange
    { 
        get => LazyLoader.Load(this, ref _MDTimeRange);
        set => _MDTimeRange = value;
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
    
    private MDToleranceState _MDToleranceState;
    public virtual MDToleranceState MDToleranceState
    { 
        get => LazyLoader.Load(this, ref _MDToleranceState);
        set => _MDToleranceState = value;
    }

    public bool MDToleranceState_IsLoaded
    {
        get
        {
            return MDToleranceState != null;
        }
    }

    public virtual ReferenceEntry MDToleranceStateReference 
    {
        get { return Context.Entry(this).Reference("MDToleranceState"); }
    }
    
    private MDTourplanPosState _MDTourplanPosState;
    public virtual MDTourplanPosState MDTourplanPosState
    { 
        get => LazyLoader.Load(this, ref _MDTourplanPosState);
        set => _MDTourplanPosState = value;
    }

    public bool MDTourplanPosState_IsLoaded
    {
        get
        {
            return MDTourplanPosState != null;
        }
    }

    public virtual ReferenceEntry MDTourplanPosStateReference 
    {
        get { return Context.Entry(this).Reference("MDTourplanPosState"); }
    }
    
    private MDTransportMode _MDTransportMode;
    public virtual MDTransportMode MDTransportMode
    { 
        get => LazyLoader.Load(this, ref _MDTransportMode);
        set => _MDTransportMode = value;
    }

    public bool MDTransportMode_IsLoaded
    {
        get
        {
            return MDTransportMode != null;
        }
    }

    public virtual ReferenceEntry MDTransportModeReference 
    {
        get { return Context.Entry(this).Reference("MDTransportMode"); }
    }
    
    private MDUnit _MDUnit;
    public virtual MDUnit MDUnit
    { 
        get => LazyLoader.Load(this, ref _MDUnit);
        set => _MDUnit = value;
    }

    public bool MDUnit_IsLoaded
    {
        get
        {
            return MDUnit != null;
        }
    }

    public virtual ReferenceEntry MDUnitReference 
    {
        get { return Context.Entry(this).Reference("MDUnit"); }
    }
    
    private Material _Material;
    public virtual Material Material
    { 
        get => LazyLoader.Load(this, ref _Material);
        set => _Material = value;
    }

    public bool Material_IsLoaded
    {
        get
        {
            return Material != null;
        }
    }

    public virtual ReferenceEntry MaterialReference 
    {
        get { return Context.Entry(this).Reference("Material"); }
    }
    
    private OutOrder _OutOrder;
    public virtual OutOrder OutOrder
    { 
        get => LazyLoader.Load(this, ref _OutOrder);
        set => _OutOrder = value;
    }

    public bool OutOrder_IsLoaded
    {
        get
        {
            return OutOrder != null;
        }
    }

    public virtual ReferenceEntry OutOrderReference 
    {
        get { return Context.Entry(this).Reference("OutOrder"); }
    }
    
    private ICollection<OutOrderPosSplit> _OutOrderPosSplit_OutOrderPos;
    public virtual ICollection<OutOrderPosSplit> OutOrderPosSplit_OutOrderPos
    {
        get => LazyLoader.Load(this, ref _OutOrderPosSplit_OutOrderPos);
        set => _OutOrderPosSplit_OutOrderPos = value;
    }

    public bool OutOrderPosSplit_OutOrderPos_IsLoaded
    {
        get
        {
            return OutOrderPosSplit_OutOrderPos != null;
        }
    }

    public virtual CollectionEntry OutOrderPosSplit_OutOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPosSplit_OutOrderPos); }
    }

    private ICollection<OutOrderPosUtilization> _OutOrderPosUtilization_OutOrderPos;
    public virtual ICollection<OutOrderPosUtilization> OutOrderPosUtilization_OutOrderPos
    {
        get => LazyLoader.Load(this, ref _OutOrderPosUtilization_OutOrderPos);
        set => _OutOrderPosUtilization_OutOrderPos = value;
    }

    public bool OutOrderPosUtilization_OutOrderPos_IsLoaded
    {
        get
        {
            return OutOrderPosUtilization_OutOrderPos != null;
        }
    }

    public virtual CollectionEntry OutOrderPosUtilization_OutOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPosUtilization_OutOrderPos); }
    }

    private OutOrderPos _OutOrderPos1_ParentOutOrderPos;
    public virtual OutOrderPos OutOrderPos1_ParentOutOrderPos
    { 
        get => LazyLoader.Load(this, ref _OutOrderPos1_ParentOutOrderPos);
        set => _OutOrderPos1_ParentOutOrderPos = value;
    }

    public bool OutOrderPos1_ParentOutOrderPos_IsLoaded
    {
        get
        {
            return OutOrderPos1_ParentOutOrderPos != null;
        }
    }

    public virtual ReferenceEntry OutOrderPos1_ParentOutOrderPosReference 
    {
        get { return Context.Entry(this).Reference("OutOrderPos1_ParentOutOrderPos"); }
    }
    
    private ICollection<PickingPos> _PickingPos_OutOrderPos;
    public virtual ICollection<PickingPos> PickingPos_OutOrderPos
    {
        get => LazyLoader.Load(this, ref _PickingPos_OutOrderPos);
        set => _PickingPos_OutOrderPos = value;
    }

    public bool PickingPos_OutOrderPos_IsLoaded
    {
        get
        {
            return PickingPos_OutOrderPos != null;
        }
    }

    public virtual CollectionEntry PickingPos_OutOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.PickingPos_OutOrderPos); }
    }

    private CompanyMaterial _PickupCompanyMaterial;
    public virtual CompanyMaterial PickupCompanyMaterial
    { 
        get => LazyLoader.Load(this, ref _PickupCompanyMaterial);
        set => _PickupCompanyMaterial = value;
    }

    public bool PickupCompanyMaterial_IsLoaded
    {
        get
        {
            return PickupCompanyMaterial != null;
        }
    }

    public virtual ReferenceEntry PickupCompanyMaterialReference 
    {
        get { return Context.Entry(this).Reference("PickupCompanyMaterial"); }
    }
    
    private ICollection<TandTv2StepItem> _TandTv2StepItem_OutOrderPos;
    public virtual ICollection<TandTv2StepItem> TandTv2StepItem_OutOrderPos
    {
        get => LazyLoader.Load(this, ref _TandTv2StepItem_OutOrderPos);
        set => _TandTv2StepItem_OutOrderPos = value;
    }

    public bool TandTv2StepItem_OutOrderPos_IsLoaded
    {
        get
        {
            return TandTv2StepItem_OutOrderPos != null;
        }
    }

    public virtual CollectionEntry TandTv2StepItem_OutOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv2StepItem_OutOrderPos); }
    }

    private ICollection<TandTv3MixPointOutOrderPos> _TandTv3MixPointOutOrderPos_OutOrderPos;
    public virtual ICollection<TandTv3MixPointOutOrderPos> TandTv3MixPointOutOrderPos_OutOrderPos
    {
        get => LazyLoader.Load(this, ref _TandTv3MixPointOutOrderPos_OutOrderPos);
        set => _TandTv3MixPointOutOrderPos_OutOrderPos = value;
    }

    public bool TandTv3MixPointOutOrderPos_OutOrderPos_IsLoaded
    {
        get
        {
            return TandTv3MixPointOutOrderPos_OutOrderPos != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointOutOrderPos_OutOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointOutOrderPos_OutOrderPos); }
    }

    private ICollection<Weighing> _Weighing_OutOrderPos;
    public virtual ICollection<Weighing> Weighing_OutOrderPos
    {
        get => LazyLoader.Load(this, ref _Weighing_OutOrderPos);
        set => _Weighing_OutOrderPos = value;
    }

    public bool Weighing_OutOrderPos_IsLoaded
    {
        get
        {
            return Weighing_OutOrderPos != null;
        }
    }

    public virtual CollectionEntry Weighing_OutOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.Weighing_OutOrderPos); }
    }
}
