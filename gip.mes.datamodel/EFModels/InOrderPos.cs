using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class InOrderPos : VBEntityObject, IInsertInfo, IUpdateInfo, ISequence, ITargetQuantity
{

    public InOrderPos()
    {
    }

    private InOrderPos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _InOrderPosID;
    public Guid InOrderPosID 
    {
        get { return _InOrderPosID; }
        set { SetProperty<Guid>(ref _InOrderPosID, value); }
    }

    Guid _InOrderID;
    public Guid InOrderID 
    {
        get { return _InOrderID; }
        set { SetForeignKeyProperty<Guid>(ref _InOrderID, value, "InOrder", _InOrder, InOrder != null ? InOrder.InOrderID : default(Guid)); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    short _MaterialPosTypeIndex;
    public short MaterialPosTypeIndex 
    {
        get { return _MaterialPosTypeIndex; }
        set { SetProperty<short>(ref _MaterialPosTypeIndex, value); }
    }

    Guid? _ParentInOrderPosID;
    public Guid? ParentInOrderPosID 
    {
        get { return _ParentInOrderPosID; }
        set { SetForeignKeyProperty<Guid?>(ref _ParentInOrderPosID, value, "InOrderPos1_ParentInOrderPos", _InOrderPos1_ParentInOrderPos, InOrderPos1_ParentInOrderPos != null ? InOrderPos1_ParentInOrderPos.InOrderPosID : default(Guid?)); }
    }

    Guid _MaterialID;
    public Guid MaterialID 
    {
        get { return _MaterialID; }
        set { SetForeignKeyProperty<Guid>(ref _MaterialID, value, "Material", _Material, Material != null ? Material.MaterialID : default(Guid)); }
    }

    Guid? _MDUnitID;
    public Guid? MDUnitID 
    {
        get { return _MDUnitID; }
        set { SetForeignKeyProperty<Guid?>(ref _MDUnitID, value, "MDUnit", _MDUnit, MDUnit != null ? MDUnit.MDUnitID : default(Guid?)); }
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

    Guid? _MDTimeRangeID;
    public Guid? MDTimeRangeID 
    {
        get { return _MDTimeRangeID; }
        set { SetForeignKeyProperty<Guid?>(ref _MDTimeRangeID, value, "MDTimeRange", _MDTimeRange, MDTimeRange != null ? MDTimeRange.MDTimeRangeID : default(Guid?)); }
    }

    Guid _MDDelivPosStateID;
    public Guid MDDelivPosStateID 
    {
        get { return _MDDelivPosStateID; }
        set { SetForeignKeyProperty<Guid>(ref _MDDelivPosStateID, value, "MDDelivPosState", _MDDelivPosState, MDDelivPosState != null ? MDDelivPosState.MDDelivPosStateID : default(Guid)); }
    }

    Guid _MDInOrderPosStateID;
    public Guid MDInOrderPosStateID 
    {
        get { return _MDInOrderPosStateID; }
        set { SetForeignKeyProperty<Guid>(ref _MDInOrderPosStateID, value, "MDInOrderPosState", _MDInOrderPosState, MDInOrderPosState != null ? MDInOrderPosState.MDInOrderPosStateID : default(Guid)); }
    }

    Guid? _MDDelivPosLoadStateID;
    public Guid? MDDelivPosLoadStateID 
    {
        get { return _MDDelivPosLoadStateID; }
        set { SetForeignKeyProperty<Guid?>(ref _MDDelivPosLoadStateID, value, "MDDelivPosLoadState", _MDDelivPosLoadState, MDDelivPosLoadState != null ? MDDelivPosLoadState.MDDelivPosLoadStateID : default(Guid?)); }
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

    Guid? _MDCountrySalesTaxID;
    public Guid? MDCountrySalesTaxID 
    {
        get { return _MDCountrySalesTaxID; }
        set { SetForeignKeyProperty<Guid?>(ref _MDCountrySalesTaxID, value, "MDCountrySalesTax", _MDCountrySalesTax, MDCountrySalesTax != null ? MDCountrySalesTax.MDCountrySalesTaxID : default(Guid?)); }
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

    string _LineNumber;
    public string LineNumber 
    {
        get { return _LineNumber; }
        set { SetProperty<string>(ref _LineNumber, value); }
    }

    Guid? _PickupCompanyMaterialID;
    public Guid? PickupCompanyMaterialID 
    {
        get { return _PickupCompanyMaterialID; }
        set { SetForeignKeyProperty<Guid?>(ref _PickupCompanyMaterialID, value, "PickupCompanyMaterial", _PickupCompanyMaterial, PickupCompanyMaterial != null ? PickupCompanyMaterial.CompanyMaterialID : default(Guid?)); }
    }

    Guid? _MDTransportModeID;
    public Guid? MDTransportModeID 
    {
        get { return _MDTransportModeID; }
        set { SetForeignKeyProperty<Guid?>(ref _MDTransportModeID, value, "MDTransportMode", _MDTransportMode, MDTransportMode != null ? MDTransportMode.MDTransportModeID : default(Guid?)); }
    }

    string _KeyOfExtSys;
    public string KeyOfExtSys 
    {
        get { return _KeyOfExtSys; }
        set { SetProperty<string>(ref _KeyOfExtSys, value); }
    }

    private ICollection<CompanyMaterialPickup> _CompanyMaterialPickup_InOrderPos;
    public virtual ICollection<CompanyMaterialPickup> CompanyMaterialPickup_InOrderPos
    {
        get { return LazyLoader.Load(this, ref _CompanyMaterialPickup_InOrderPos); }
        set { SetProperty<ICollection<CompanyMaterialPickup>>(ref _CompanyMaterialPickup_InOrderPos, value); }
    }

    public bool CompanyMaterialPickup_InOrderPos_IsLoaded
    {
        get
        {
            return _CompanyMaterialPickup_InOrderPos != null;
        }
    }

    public virtual CollectionEntry CompanyMaterialPickup_InOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyMaterialPickup_InOrderPos); }
    }

    private ICollection<DeliveryNotePos> _DeliveryNotePos_InOrderPos;
    public virtual ICollection<DeliveryNotePos> DeliveryNotePos_InOrderPos
    {
        get { return LazyLoader.Load(this, ref _DeliveryNotePos_InOrderPos); }
        set { SetProperty<ICollection<DeliveryNotePos>>(ref _DeliveryNotePos_InOrderPos, value); }
    }

    public bool DeliveryNotePos_InOrderPos_IsLoaded
    {
        get
        {
            return _DeliveryNotePos_InOrderPos != null;
        }
    }

    public virtual CollectionEntry DeliveryNotePos_InOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.DeliveryNotePos_InOrderPos); }
    }

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_InOrderPos;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_InOrderPos
    {
        get { return LazyLoader.Load(this, ref _FacilityBookingCharge_InOrderPos); }
        set { SetProperty<ICollection<FacilityBookingCharge>>(ref _FacilityBookingCharge_InOrderPos, value); }
    }

    public bool FacilityBookingCharge_InOrderPos_IsLoaded
    {
        get
        {
            return _FacilityBookingCharge_InOrderPos != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_InOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_InOrderPos); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_InOrderPos;
    public virtual ICollection<FacilityBooking> FacilityBooking_InOrderPos
    {
        get { return LazyLoader.Load(this, ref _FacilityBooking_InOrderPos); }
        set { SetProperty<ICollection<FacilityBooking>>(ref _FacilityBooking_InOrderPos, value); }
    }

    public bool FacilityBooking_InOrderPos_IsLoaded
    {
        get
        {
            return _FacilityBooking_InOrderPos != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_InOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_InOrderPos); }
    }

    private ICollection<FacilityPreBooking> _FacilityPreBooking_InOrderPos;
    public virtual ICollection<FacilityPreBooking> FacilityPreBooking_InOrderPos
    {
        get { return LazyLoader.Load(this, ref _FacilityPreBooking_InOrderPos); }
        set { SetProperty<ICollection<FacilityPreBooking>>(ref _FacilityPreBooking_InOrderPos, value); }
    }

    public bool FacilityPreBooking_InOrderPos_IsLoaded
    {
        get
        {
            return _FacilityPreBooking_InOrderPos != null;
        }
    }

    public virtual CollectionEntry FacilityPreBooking_InOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityPreBooking_InOrderPos); }
    }

    private ICollection<FacilityReservation> _FacilityReservation_InOrderPos;
    public virtual ICollection<FacilityReservation> FacilityReservation_InOrderPos
    {
        get { return LazyLoader.Load(this, ref _FacilityReservation_InOrderPos); }
        set { SetProperty<ICollection<FacilityReservation>>(ref _FacilityReservation_InOrderPos, value); }
    }

    public bool FacilityReservation_InOrderPos_IsLoaded
    {
        get
        {
            return _FacilityReservation_InOrderPos != null;
        }
    }

    public virtual CollectionEntry FacilityReservation_InOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityReservation_InOrderPos); }
    }

    private InOrder _InOrder;
    public virtual InOrder InOrder
    { 
        get { return LazyLoader.Load(this, ref _InOrder); } 
        set { SetProperty<InOrder>(ref _InOrder, value); }
    }

    public bool InOrder_IsLoaded
    {
        get
        {
            return _InOrder != null;
        }
    }

    public virtual ReferenceEntry InOrderReference 
    {
        get { return Context.Entry(this).Reference("InOrder"); }
    }
    
    private ICollection<InOrderPosSplit> _InOrderPosSplit_InOrderPos;
    public virtual ICollection<InOrderPosSplit> InOrderPosSplit_InOrderPos
    {
        get { return LazyLoader.Load(this, ref _InOrderPosSplit_InOrderPos); }
        set { SetProperty<ICollection<InOrderPosSplit>>(ref _InOrderPosSplit_InOrderPos, value); }
    }

    public bool InOrderPosSplit_InOrderPos_IsLoaded
    {
        get
        {
            return _InOrderPosSplit_InOrderPos != null;
        }
    }

    public virtual CollectionEntry InOrderPosSplit_InOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderPosSplit_InOrderPos); }
    }

    private ICollection<InOrderPos> _InOrderPos_ParentInOrderPos;
    public virtual ICollection<InOrderPos> InOrderPos_ParentInOrderPos
    {
        get { return LazyLoader.Load(this, ref _InOrderPos_ParentInOrderPos); }
        set { SetProperty<ICollection<InOrderPos>>(ref _InOrderPos_ParentInOrderPos, value); }
    }

    public bool InOrderPos_ParentInOrderPos_IsLoaded
    {
        get
        {
            return _InOrderPos_ParentInOrderPos != null;
        }
    }

    public virtual CollectionEntry InOrderPos_ParentInOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderPos_ParentInOrderPos); }
    }

    private ICollection<LabOrder> _LabOrder_InOrderPos;
    public virtual ICollection<LabOrder> LabOrder_InOrderPos
    {
        get { return LazyLoader.Load(this, ref _LabOrder_InOrderPos); }
        set { SetProperty<ICollection<LabOrder>>(ref _LabOrder_InOrderPos, value); }
    }

    public bool LabOrder_InOrderPos_IsLoaded
    {
        get
        {
            return _LabOrder_InOrderPos != null;
        }
    }

    public virtual CollectionEntry LabOrder_InOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.LabOrder_InOrderPos); }
    }

    private MDCountrySalesTax _MDCountrySalesTax;
    public virtual MDCountrySalesTax MDCountrySalesTax
    { 
        get { return LazyLoader.Load(this, ref _MDCountrySalesTax); } 
        set { SetProperty<MDCountrySalesTax>(ref _MDCountrySalesTax, value); }
    }

    public bool MDCountrySalesTax_IsLoaded
    {
        get
        {
            return _MDCountrySalesTax != null;
        }
    }

    public virtual ReferenceEntry MDCountrySalesTaxReference 
    {
        get { return Context.Entry(this).Reference("MDCountrySalesTax"); }
    }
    
    private MDDelivPosLoadState _MDDelivPosLoadState;
    public virtual MDDelivPosLoadState MDDelivPosLoadState
    { 
        get { return LazyLoader.Load(this, ref _MDDelivPosLoadState); } 
        set { SetProperty<MDDelivPosLoadState>(ref _MDDelivPosLoadState, value); }
    }

    public bool MDDelivPosLoadState_IsLoaded
    {
        get
        {
            return _MDDelivPosLoadState != null;
        }
    }

    public virtual ReferenceEntry MDDelivPosLoadStateReference 
    {
        get { return Context.Entry(this).Reference("MDDelivPosLoadState"); }
    }
    
    private MDDelivPosState _MDDelivPosState;
    public virtual MDDelivPosState MDDelivPosState
    { 
        get { return LazyLoader.Load(this, ref _MDDelivPosState); } 
        set { SetProperty<MDDelivPosState>(ref _MDDelivPosState, value); }
    }

    public bool MDDelivPosState_IsLoaded
    {
        get
        {
            return _MDDelivPosState != null;
        }
    }

    public virtual ReferenceEntry MDDelivPosStateReference 
    {
        get { return Context.Entry(this).Reference("MDDelivPosState"); }
    }
    
    private MDInOrderPosState _MDInOrderPosState;
    public virtual MDInOrderPosState MDInOrderPosState
    { 
        get { return LazyLoader.Load(this, ref _MDInOrderPosState); } 
        set { SetProperty<MDInOrderPosState>(ref _MDInOrderPosState, value); }
    }

    public bool MDInOrderPosState_IsLoaded
    {
        get
        {
            return _MDInOrderPosState != null;
        }
    }

    public virtual ReferenceEntry MDInOrderPosStateReference 
    {
        get { return Context.Entry(this).Reference("MDInOrderPosState"); }
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
    
    private MDTransportMode _MDTransportMode;
    public virtual MDTransportMode MDTransportMode
    { 
        get { return LazyLoader.Load(this, ref _MDTransportMode); } 
        set { SetProperty<MDTransportMode>(ref _MDTransportMode, value); }
    }

    public bool MDTransportMode_IsLoaded
    {
        get
        {
            return _MDTransportMode != null;
        }
    }

    public virtual ReferenceEntry MDTransportModeReference 
    {
        get { return Context.Entry(this).Reference("MDTransportMode"); }
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
    
    private InOrderPos _InOrderPos1_ParentInOrderPos;
    public virtual InOrderPos InOrderPos1_ParentInOrderPos
    { 
        get { return LazyLoader.Load(this, ref _InOrderPos1_ParentInOrderPos); } 
        set { SetProperty<InOrderPos>(ref _InOrderPos1_ParentInOrderPos, value); }
    }

    public bool InOrderPos1_ParentInOrderPos_IsLoaded
    {
        get
        {
            return _InOrderPos1_ParentInOrderPos != null;
        }
    }

    public virtual ReferenceEntry InOrderPos1_ParentInOrderPosReference 
    {
        get { return Context.Entry(this).Reference("InOrderPos1_ParentInOrderPos"); }
    }
    
    private ICollection<PickingPos> _PickingPos_InOrderPos;
    public virtual ICollection<PickingPos> PickingPos_InOrderPos
    {
        get { return LazyLoader.Load(this, ref _PickingPos_InOrderPos); }
        set { SetProperty<ICollection<PickingPos>>(ref _PickingPos_InOrderPos, value); }
    }

    public bool PickingPos_InOrderPos_IsLoaded
    {
        get
        {
            return _PickingPos_InOrderPos != null;
        }
    }

    public virtual CollectionEntry PickingPos_InOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.PickingPos_InOrderPos); }
    }

    private CompanyMaterial _PickupCompanyMaterial;
    public virtual CompanyMaterial PickupCompanyMaterial
    { 
        get { return LazyLoader.Load(this, ref _PickupCompanyMaterial); } 
        set { SetProperty<CompanyMaterial>(ref _PickupCompanyMaterial, value); }
    }

    public bool PickupCompanyMaterial_IsLoaded
    {
        get
        {
            return _PickupCompanyMaterial != null;
        }
    }

    public virtual ReferenceEntry PickupCompanyMaterialReference 
    {
        get { return Context.Entry(this).Reference("PickupCompanyMaterial"); }
    }
    
    private ICollection<PlanningMRPos> _PlanningMRPos_InOrderPos;
    public virtual ICollection<PlanningMRPos> PlanningMRPos_InOrderPos
    {
        get { return LazyLoader.Load(this, ref _PlanningMRPos_InOrderPos); }
        set { SetProperty<ICollection<PlanningMRPos>>(ref _PlanningMRPos_InOrderPos, value); }
    }

    public bool PlanningMRPos_InOrderPos_IsLoaded
    {
        get
        {
            return _PlanningMRPos_InOrderPos != null;
        }
    }

    public virtual CollectionEntry PlanningMRPos_InOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.PlanningMRPos_InOrderPos); }
    }

    private ICollection<TandTv3MixPointInOrderPos> _TandTv3MixPointInOrderPos_InOrderPos;
    public virtual ICollection<TandTv3MixPointInOrderPos> TandTv3MixPointInOrderPos_InOrderPos
    {
        get { return LazyLoader.Load(this, ref _TandTv3MixPointInOrderPos_InOrderPos); }
        set { SetProperty<ICollection<TandTv3MixPointInOrderPos>>(ref _TandTv3MixPointInOrderPos_InOrderPos, value); }
    }

    public bool TandTv3MixPointInOrderPos_InOrderPos_IsLoaded
    {
        get
        {
            return _TandTv3MixPointInOrderPos_InOrderPos != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointInOrderPos_InOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointInOrderPos_InOrderPos); }
    }

    private ICollection<Weighing> _Weighing_InOrderPos;
    public virtual ICollection<Weighing> Weighing_InOrderPos
    {
        get { return LazyLoader.Load(this, ref _Weighing_InOrderPos); }
        set { SetProperty<ICollection<Weighing>>(ref _Weighing_InOrderPos, value); }
    }

    public bool Weighing_InOrderPos_IsLoaded
    {
        get
        {
            return _Weighing_InOrderPos != null;
        }
    }

    public virtual CollectionEntry Weighing_InOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.Weighing_InOrderPos); }
    }
}
