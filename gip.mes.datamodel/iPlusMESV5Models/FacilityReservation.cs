using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class FacilityReservation : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public FacilityReservation()
    {
    }

    private FacilityReservation(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _FacilityReservationID;
    public Guid FacilityReservationID 
    {
        get { return _FacilityReservationID; }
        set { SetProperty<Guid>(ref _FacilityReservationID, value); }
    }

    string _FacilityReservationNo;
    public string FacilityReservationNo 
    {
        get { return _FacilityReservationNo; }
        set { SetProperty<string>(ref _FacilityReservationNo, value); }
    }

    Guid? _MaterialID;
    public Guid? MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid?>(ref _MaterialID, value); }
    }

    Guid? _FacilityLotID;
    public Guid? FacilityLotID 
    {
        get { return _FacilityLotID; }
        set { SetProperty<Guid?>(ref _FacilityLotID, value); }
    }

    Guid? _FacilityChargeID;
    public Guid? FacilityChargeID 
    {
        get { return _FacilityChargeID; }
        set { SetProperty<Guid?>(ref _FacilityChargeID, value); }
    }

    Guid? _FacilityID;
    public Guid? FacilityID 
    {
        get { return _FacilityID; }
        set { SetProperty<Guid?>(ref _FacilityID, value); }
    }

    Guid? _InOrderPosID;
    public Guid? InOrderPosID 
    {
        get { return _InOrderPosID; }
        set { SetProperty<Guid?>(ref _InOrderPosID, value); }
    }

    Guid? _OutOrderPosID;
    public Guid? OutOrderPosID 
    {
        get { return _OutOrderPosID; }
        set { SetProperty<Guid?>(ref _OutOrderPosID, value); }
    }

    Guid? _ProdOrderPartslistPosID;
    public Guid? ProdOrderPartslistPosID 
    {
        get { return _ProdOrderPartslistPosID; }
        set { SetProperty<Guid?>(ref _ProdOrderPartslistPosID, value); }
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

    Guid? _ParentFacilityReservationID;
    public Guid? ParentFacilityReservationID 
    {
        get { return _ParentFacilityReservationID; }
        set { SetProperty<Guid?>(ref _ParentFacilityReservationID, value); }
    }

    Guid? _ProdOrderBatchPlanID;
    public Guid? ProdOrderBatchPlanID 
    {
        get { return _ProdOrderBatchPlanID; }
        set { SetProperty<Guid?>(ref _ProdOrderBatchPlanID, value); }
    }

    Guid? _VBiACClassID;
    public Guid? VBiACClassID 
    {
        get { return _VBiACClassID; }
        set { SetProperty<Guid?>(ref _VBiACClassID, value); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    short _ReservationStateIndex;
    public short ReservationStateIndex 
    {
        get { return _ReservationStateIndex; }
        set { SetProperty<short>(ref _ReservationStateIndex, value); }
    }

    Guid? _ProdOrderPartslistPosRelationID;
    public Guid? ProdOrderPartslistPosRelationID 
    {
        get { return _ProdOrderPartslistPosRelationID; }
        set { SetProperty<Guid?>(ref _ProdOrderPartslistPosRelationID, value); }
    }

    private Facility _Facility;
    public virtual Facility Facility
    { 
        get => LazyLoader.Load(this, ref _Facility);
        set => _Facility = value;
    }

    public bool Facility_IsLoaded
    {
        get
        {
            return Facility != null;
        }
    }

    public virtual ReferenceEntry FacilityReference 
    {
        get { return Context.Entry(this).Reference("Facility"); }
    }
    
    private FacilityCharge _FacilityCharge;
    public virtual FacilityCharge FacilityCharge
    { 
        get => LazyLoader.Load(this, ref _FacilityCharge);
        set => _FacilityCharge = value;
    }

    public bool FacilityCharge_IsLoaded
    {
        get
        {
            return FacilityCharge != null;
        }
    }

    public virtual ReferenceEntry FacilityChargeReference 
    {
        get { return Context.Entry(this).Reference("FacilityCharge"); }
    }
    
    private FacilityLot _FacilityLot;
    public virtual FacilityLot FacilityLot
    { 
        get => LazyLoader.Load(this, ref _FacilityLot);
        set => _FacilityLot = value;
    }

    public bool FacilityLot_IsLoaded
    {
        get
        {
            return FacilityLot != null;
        }
    }

    public virtual ReferenceEntry FacilityLotReference 
    {
        get { return Context.Entry(this).Reference("FacilityLot"); }
    }
    
    private InOrderPos _InOrderPos;
    public virtual InOrderPos InOrderPos
    { 
        get => LazyLoader.Load(this, ref _InOrderPos);
        set => _InOrderPos = value;
    }

    public bool InOrderPos_IsLoaded
    {
        get
        {
            return InOrderPos != null;
        }
    }

    public virtual ReferenceEntry InOrderPosReference 
    {
        get { return Context.Entry(this).Reference("InOrderPos"); }
    }
    
    private ICollection<FacilityReservation> _FacilityReservation_ParentFacilityReservation;
    public virtual ICollection<FacilityReservation> FacilityReservation_ParentFacilityReservation
    {
        get => LazyLoader.Load(this, ref _FacilityReservation_ParentFacilityReservation);
        set => _FacilityReservation_ParentFacilityReservation = value;
    }

    public bool FacilityReservation_ParentFacilityReservation_IsLoaded
    {
        get
        {
            return FacilityReservation_ParentFacilityReservation != null;
        }
    }

    public virtual CollectionEntry FacilityReservation_ParentFacilityReservationReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityReservation_ParentFacilityReservation); }
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
    
    private OutOrderPos _OutOrderPos;
    public virtual OutOrderPos OutOrderPos
    { 
        get => LazyLoader.Load(this, ref _OutOrderPos);
        set => _OutOrderPos = value;
    }

    public bool OutOrderPos_IsLoaded
    {
        get
        {
            return OutOrderPos != null;
        }
    }

    public virtual ReferenceEntry OutOrderPosReference 
    {
        get { return Context.Entry(this).Reference("OutOrderPos"); }
    }
    
    private FacilityReservation _FacilityReservation1_ParentFacilityReservation;
    public virtual FacilityReservation FacilityReservation1_ParentFacilityReservation
    { 
        get => LazyLoader.Load(this, ref _FacilityReservation1_ParentFacilityReservation);
        set => _FacilityReservation1_ParentFacilityReservation = value;
    }

    public bool FacilityReservation1_ParentFacilityReservation_IsLoaded
    {
        get
        {
            return FacilityReservation1_ParentFacilityReservation != null;
        }
    }

    public virtual ReferenceEntry FacilityReservation1_ParentFacilityReservationReference 
    {
        get { return Context.Entry(this).Reference("FacilityReservation1_ParentFacilityReservation"); }
    }
    
    private ProdOrderBatchPlan _ProdOrderBatchPlan;
    public virtual ProdOrderBatchPlan ProdOrderBatchPlan
    { 
        get => LazyLoader.Load(this, ref _ProdOrderBatchPlan);
        set => _ProdOrderBatchPlan = value;
    }

    public bool ProdOrderBatchPlan_IsLoaded
    {
        get
        {
            return ProdOrderBatchPlan != null;
        }
    }

    public virtual ReferenceEntry ProdOrderBatchPlanReference 
    {
        get { return Context.Entry(this).Reference("ProdOrderBatchPlan"); }
    }
    
    private ProdOrderPartslistPos _ProdOrderPartslistPos;
    public virtual ProdOrderPartslistPos ProdOrderPartslistPos
    { 
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPos);
        set => _ProdOrderPartslistPos = value;
    }

    public bool ProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPos != null;
        }
    }

    public virtual ReferenceEntry ProdOrderPartslistPosReference 
    {
        get { return Context.Entry(this).Reference("ProdOrderPartslistPos"); }
    }
    
    private ProdOrderPartslistPosRelation _ProdOrderPartslistPosRelation;
    public virtual ProdOrderPartslistPosRelation ProdOrderPartslistPosRelation
    { 
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPosRelation);
        set => _ProdOrderPartslistPosRelation = value;
    }

    public bool ProdOrderPartslistPosRelation_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPosRelation != null;
        }
    }

    public virtual ReferenceEntry ProdOrderPartslistPosRelationReference 
    {
        get { return Context.Entry(this).Reference("ProdOrderPartslistPosRelation"); }
    }
    
    private ACClass _VBiACClass;
    public virtual ACClass VBiACClass
    { 
        get => LazyLoader.Load(this, ref _VBiACClass);
        set => _VBiACClass = value;
    }

    public bool VBiACClass_IsLoaded
    {
        get
        {
            return VBiACClass != null;
        }
    }

    public virtual ReferenceEntry VBiACClassReference 
    {
        get { return Context.Entry(this).Reference("VBiACClass"); }
    }
    }
