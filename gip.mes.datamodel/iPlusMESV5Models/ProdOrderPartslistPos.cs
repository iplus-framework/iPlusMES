using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ProdOrderPartslistPos : VBEntityObject, IInsertInfo, IUpdateInfo, ISequence, ITargetQuantity
{

    public ProdOrderPartslistPos()
    {
    }

    private ProdOrderPartslistPos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ProdOrderPartslistPosID;
    public Guid ProdOrderPartslistPosID 
    {
        get { return _ProdOrderPartslistPosID; }
        set { SetProperty<Guid>(ref _ProdOrderPartslistPosID, value); }
    }

    Guid _ProdOrderPartslistID;
    public Guid ProdOrderPartslistID 
    {
        get { return _ProdOrderPartslistID; }
        set { SetProperty<Guid>(ref _ProdOrderPartslistID, value); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    int _SequenceProduction;
    public int SequenceProduction 
    {
        get { return _SequenceProduction; }
        set { SetProperty<int>(ref _SequenceProduction, value); }
    }

    short _MaterialPosTypeIndex;
    public short MaterialPosTypeIndex 
    {
        get { return _MaterialPosTypeIndex; }
        set { SetProperty<short>(ref _MaterialPosTypeIndex, value); }
    }

    Guid? _MaterialID;
    public Guid? MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid?>(ref _MaterialID, value); }
    }

    Guid? _MDUnitID;
    public Guid? MDUnitID 
    {
        get { return _MDUnitID; }
        set { SetProperty<Guid?>(ref _MDUnitID, value); }
    }

    double _TargetQuantity;
    public double TargetQuantity 
    {
        get { return _TargetQuantity; }
        set { SetProperty<double>(ref _TargetQuantity, value); }
    }

    double _ActualQuantity;
    public double ActualQuantity 
    {
        get { return _ActualQuantity; }
        set { SetProperty<double>(ref _ActualQuantity, value); }
    }

    double _TargetQuantityUOM;
    public double TargetQuantityUOM 
    {
        get { return _TargetQuantityUOM; }
        set { SetProperty<double>(ref _TargetQuantityUOM, value); }
    }

    double _ActualQuantityUOM;
    public double ActualQuantityUOM 
    {
        get { return _ActualQuantityUOM; }
        set { SetProperty<double>(ref _ActualQuantityUOM, value); }
    }

    Guid _MDToleranceStateID;
    public Guid MDToleranceStateID 
    {
        get { return _MDToleranceStateID; }
        set { SetProperty<Guid>(ref _MDToleranceStateID, value); }
    }

    bool _IsBaseQuantityExcluded;
    public bool IsBaseQuantityExcluded 
    {
        get { return _IsBaseQuantityExcluded; }
        set { SetProperty<bool>(ref _IsBaseQuantityExcluded, value); }
    }

    Guid? _BasedOnPartslistPosID;
    public Guid? BasedOnPartslistPosID 
    {
        get { return _BasedOnPartslistPosID; }
        set { SetProperty<Guid?>(ref _BasedOnPartslistPosID, value); }
    }

    Guid? _SourceProdOrderPartslistID;
    public Guid? SourceProdOrderPartslistID 
    {
        get { return _SourceProdOrderPartslistID; }
        set { SetProperty<Guid?>(ref _SourceProdOrderPartslistID, value); }
    }

    Guid? _ParentProdOrderPartslistPosID;
    public Guid? ParentProdOrderPartslistPosID 
    {
        get { return _ParentProdOrderPartslistPosID; }
        set { SetProperty<Guid?>(ref _ParentProdOrderPartslistPosID, value); }
    }

    Guid? _AlternativeProdOrderPartslistPosID;
    public Guid? AlternativeProdOrderPartslistPosID 
    {
        get { return _AlternativeProdOrderPartslistPosID; }
        set { SetProperty<Guid?>(ref _AlternativeProdOrderPartslistPosID, value); }
    }

    Guid? _FacilityLotID;
    public Guid? FacilityLotID 
    {
        get { return _FacilityLotID; }
        set { SetProperty<Guid?>(ref _FacilityLotID, value); }
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

    Guid? _MDProdOrderPartslistPosStateID;
    public Guid? MDProdOrderPartslistPosStateID 
    {
        get { return _MDProdOrderPartslistPosStateID; }
        set { SetProperty<Guid?>(ref _MDProdOrderPartslistPosStateID, value); }
    }

    Guid? _ProdOrderBatchID;
    public Guid? ProdOrderBatchID 
    {
        get { return _ProdOrderBatchID; }
        set { SetProperty<Guid?>(ref _ProdOrderBatchID, value); }
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

    Guid? _ACClassTaskID;
    public Guid? ACClassTaskID 
    {
        get { return _ACClassTaskID; }
        set { SetProperty<Guid?>(ref _ACClassTaskID, value); }
    }

    bool _TakeMatFromOtherOrder;
    public bool TakeMatFromOtherOrder 
    {
        get { return _TakeMatFromOtherOrder; }
        set { SetProperty<bool>(ref _TakeMatFromOtherOrder, value); }
    }

    bool? _RetrogradeFIFO;
    public bool? RetrogradeFIFO 
    {
        get { return _RetrogradeFIFO; }
        set { SetProperty<bool?>(ref _RetrogradeFIFO, value); }
    }

    bool? _Anterograde;
    public bool? Anterograde 
    {
        get { return _Anterograde; }
        set { SetProperty<bool?>(ref _Anterograde, value); }
    }

    double? _InputQForActualOutput;
    public double? InputQForActualOutput 
    {
        get { return _InputQForActualOutput; }
        set { SetProperty<double?>(ref _InputQForActualOutput, value); }
    }

    double? _InputQForGoodActualOutput;
    public double? InputQForGoodActualOutput 
    {
        get { return _InputQForGoodActualOutput; }
        set { SetProperty<double?>(ref _InputQForGoodActualOutput, value); }
    }

    double? _InputQForScrapActualOutput;
    public double? InputQForScrapActualOutput 
    {
        get { return _InputQForScrapActualOutput; }
        set { SetProperty<double?>(ref _InputQForScrapActualOutput, value); }
    }

    double? _InputQForFinalActualOutput;
    public double? InputQForFinalActualOutput 
    {
        get { return _InputQForFinalActualOutput; }
        set { SetProperty<double?>(ref _InputQForFinalActualOutput, value); }
    }

    double? _InputQForFinalGoodActualOutput;
    public double? InputQForFinalGoodActualOutput 
    {
        get { return _InputQForFinalGoodActualOutput; }
        set { SetProperty<double?>(ref _InputQForFinalGoodActualOutput, value); }
    }

    double? _InputQForFinalScrapActualOutput;
    public double? InputQForFinalScrapActualOutput 
    {
        get { return _InputQForFinalScrapActualOutput; }
        set { SetProperty<double?>(ref _InputQForFinalScrapActualOutput, value); }
    }

    private ACClassTask _ACClassTask;
    public virtual ACClassTask ACClassTask
    { 
        get => LazyLoader.Load(this, ref _ACClassTask);
        set => _ACClassTask = value;
    }

    public bool ACClassTask_IsLoaded
    {
        get
        {
            return ACClassTask != null;
        }
    }

    public virtual ReferenceEntry ACClassTaskReference 
    {
        get { return Context.Entry(this).Reference("ACClassTask"); }
    }
    
    private ProdOrderPartslistPos _ProdOrderPartslistPos1_AlternativeProdOrderPartslistPos;
    public virtual ProdOrderPartslistPos ProdOrderPartslistPos1_AlternativeProdOrderPartslistPos
    { 
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPos1_AlternativeProdOrderPartslistPos);
        set => _ProdOrderPartslistPos1_AlternativeProdOrderPartslistPos = value;
    }

    public bool ProdOrderPartslistPos1_AlternativeProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPos1_AlternativeProdOrderPartslistPos != null;
        }
    }

    public virtual ReferenceEntry ProdOrderPartslistPos1_AlternativeProdOrderPartslistPosReference 
    {
        get { return Context.Entry(this).Reference("ProdOrderPartslistPos1_AlternativeProdOrderPartslistPos"); }
    }
    
    private PartslistPos _BasedOnPartslistPos;
    public virtual PartslistPos BasedOnPartslistPos
    { 
        get => LazyLoader.Load(this, ref _BasedOnPartslistPos);
        set => _BasedOnPartslistPos = value;
    }

    public bool BasedOnPartslistPos_IsLoaded
    {
        get
        {
            return BasedOnPartslistPos != null;
        }
    }

    public virtual ReferenceEntry BasedOnPartslistPosReference 
    {
        get { return Context.Entry(this).Reference("BasedOnPartslistPos"); }
    }
    
    private ObservableHashSet<FacilityBookingCharge> _FacilityBookingCharge_ProdOrderPartslistPos;
    public virtual ObservableHashSet<FacilityBookingCharge> FacilityBookingCharge_ProdOrderPartslistPos
    {
        get => LazyLoader.Load(this, ref _FacilityBookingCharge_ProdOrderPartslistPos);
        set => _FacilityBookingCharge_ProdOrderPartslistPos = value;
    }

    public bool FacilityBookingCharge_ProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_ProdOrderPartslistPos != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_ProdOrderPartslistPosReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_ProdOrderPartslistPos); }
    }

    private ObservableHashSet<FacilityBooking> _FacilityBooking_ProdOrderPartslistPos;
    public virtual ObservableHashSet<FacilityBooking> FacilityBooking_ProdOrderPartslistPos
    {
        get => LazyLoader.Load(this, ref _FacilityBooking_ProdOrderPartslistPos);
        set => _FacilityBooking_ProdOrderPartslistPos = value;
    }

    public bool FacilityBooking_ProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return FacilityBooking_ProdOrderPartslistPos != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_ProdOrderPartslistPosReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_ProdOrderPartslistPos); }
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
    
    private ObservableHashSet<FacilityPreBooking> _FacilityPreBooking_ProdOrderPartslistPos;
    public virtual ObservableHashSet<FacilityPreBooking> FacilityPreBooking_ProdOrderPartslistPos
    {
        get => LazyLoader.Load(this, ref _FacilityPreBooking_ProdOrderPartslistPos);
        set => _FacilityPreBooking_ProdOrderPartslistPos = value;
    }

    public bool FacilityPreBooking_ProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return FacilityPreBooking_ProdOrderPartslistPos != null;
        }
    }

    public virtual CollectionEntry FacilityPreBooking_ProdOrderPartslistPosReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityPreBooking_ProdOrderPartslistPos); }
    }

    private ObservableHashSet<FacilityReservation> _FacilityReservation_ProdOrderPartslistPos;
    public virtual ObservableHashSet<FacilityReservation> FacilityReservation_ProdOrderPartslistPos
    {
        get => LazyLoader.Load(this, ref _FacilityReservation_ProdOrderPartslistPos);
        set => _FacilityReservation_ProdOrderPartslistPos = value;
    }

    public bool FacilityReservation_ProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return FacilityReservation_ProdOrderPartslistPos != null;
        }
    }

    public virtual CollectionEntry FacilityReservation_ProdOrderPartslistPosReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityReservation_ProdOrderPartslistPos); }
    }

    private ObservableHashSet<ProdOrderPartslistPos> _ProdOrderPartslistPos_AlternativeProdOrderPartslistPos;
    public virtual ObservableHashSet<ProdOrderPartslistPos> ProdOrderPartslistPos_AlternativeProdOrderPartslistPos
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPos_AlternativeProdOrderPartslistPos);
        set => _ProdOrderPartslistPos_AlternativeProdOrderPartslistPos = value;
    }

    public bool ProdOrderPartslistPos_AlternativeProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPos_AlternativeProdOrderPartslistPos != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistPos_AlternativeProdOrderPartslistPosReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos); }
    }

    private ObservableHashSet<ProdOrderPartslistPos> _ProdOrderPartslistPos_ParentProdOrderPartslistPos;
    public virtual ObservableHashSet<ProdOrderPartslistPos> ProdOrderPartslistPos_ParentProdOrderPartslistPos
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPos_ParentProdOrderPartslistPos);
        set => _ProdOrderPartslistPos_ParentProdOrderPartslistPos = value;
    }

    public bool ProdOrderPartslistPos_ParentProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPos_ParentProdOrderPartslistPos != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistPos_ParentProdOrderPartslistPosReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistPos_ParentProdOrderPartslistPos); }
    }

    private ObservableHashSet<LabOrder> _LabOrder_ProdOrderPartslistPos;
    public virtual ObservableHashSet<LabOrder> LabOrder_ProdOrderPartslistPos
    {
        get => LazyLoader.Load(this, ref _LabOrder_ProdOrderPartslistPos);
        set => _LabOrder_ProdOrderPartslistPos = value;
    }

    public bool LabOrder_ProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return LabOrder_ProdOrderPartslistPos != null;
        }
    }

    public virtual CollectionEntry LabOrder_ProdOrderPartslistPosReference
    {
        get { return Context.Entry(this).Collection(c => c.LabOrder_ProdOrderPartslistPos); }
    }

    private MDProdOrderPartslistPosState _MDProdOrderPartslistPosState;
    public virtual MDProdOrderPartslistPosState MDProdOrderPartslistPosState
    { 
        get => LazyLoader.Load(this, ref _MDProdOrderPartslistPosState);
        set => _MDProdOrderPartslistPosState = value;
    }

    public bool MDProdOrderPartslistPosState_IsLoaded
    {
        get
        {
            return MDProdOrderPartslistPosState != null;
        }
    }

    public virtual ReferenceEntry MDProdOrderPartslistPosStateReference 
    {
        get { return Context.Entry(this).Reference("MDProdOrderPartslistPosState"); }
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
    
    private ObservableHashSet<OrderLog> _OrderLog_ProdOrderPartslistPos;
    public virtual ObservableHashSet<OrderLog> OrderLog_ProdOrderPartslistPos
    {
        get => LazyLoader.Load(this, ref _OrderLog_ProdOrderPartslistPos);
        set => _OrderLog_ProdOrderPartslistPos = value;
    }

    public bool OrderLog_ProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return OrderLog_ProdOrderPartslistPos != null;
        }
    }

    public virtual CollectionEntry OrderLog_ProdOrderPartslistPosReference
    {
        get { return Context.Entry(this).Collection(c => c.OrderLog_ProdOrderPartslistPos); }
    }

    private ProdOrderPartslistPos _ProdOrderPartslistPos1_ParentProdOrderPartslistPos;
    public virtual ProdOrderPartslistPos ProdOrderPartslistPos1_ParentProdOrderPartslistPos
    { 
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPos1_ParentProdOrderPartslistPos);
        set => _ProdOrderPartslistPos1_ParentProdOrderPartslistPos = value;
    }

    public bool ProdOrderPartslistPos1_ParentProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPos1_ParentProdOrderPartslistPos != null;
        }
    }

    public virtual ReferenceEntry ProdOrderPartslistPos1_ParentProdOrderPartslistPosReference 
    {
        get { return Context.Entry(this).Reference("ProdOrderPartslistPos1_ParentProdOrderPartslistPos"); }
    }
    
    private ObservableHashSet<PickingPosProdOrderPartslistPos> _PickingPosProdOrderPartslistPos_ProdorderPartslistPos;
    public virtual ObservableHashSet<PickingPosProdOrderPartslistPos> PickingPosProdOrderPartslistPos_ProdorderPartslistPos
    {
        get => LazyLoader.Load(this, ref _PickingPosProdOrderPartslistPos_ProdorderPartslistPos);
        set => _PickingPosProdOrderPartslistPos_ProdorderPartslistPos = value;
    }

    public bool PickingPosProdOrderPartslistPos_ProdorderPartslistPos_IsLoaded
    {
        get
        {
            return PickingPosProdOrderPartslistPos_ProdorderPartslistPos != null;
        }
    }

    public virtual CollectionEntry PickingPosProdOrderPartslistPos_ProdorderPartslistPosReference
    {
        get { return Context.Entry(this).Collection(c => c.PickingPosProdOrderPartslistPos_ProdorderPartslistPos); }
    }

    private ProdOrderBatch _ProdOrderBatch;
    public virtual ProdOrderBatch ProdOrderBatch
    { 
        get => LazyLoader.Load(this, ref _ProdOrderBatch);
        set => _ProdOrderBatch = value;
    }

    public bool ProdOrderBatch_IsLoaded
    {
        get
        {
            return ProdOrderBatch != null;
        }
    }

    public virtual ReferenceEntry ProdOrderBatchReference 
    {
        get { return Context.Entry(this).Reference("ProdOrderBatch"); }
    }
    
    private ObservableHashSet<ProdOrderBatchPlan> _ProdOrderBatchPlan_ProdOrderPartslistPos;
    public virtual ObservableHashSet<ProdOrderBatchPlan> ProdOrderBatchPlan_ProdOrderPartslistPos
    {
        get => LazyLoader.Load(this, ref _ProdOrderBatchPlan_ProdOrderPartslistPos);
        set => _ProdOrderBatchPlan_ProdOrderPartslistPos = value;
    }

    public bool ProdOrderBatchPlan_ProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return ProdOrderBatchPlan_ProdOrderPartslistPos != null;
        }
    }

    public virtual CollectionEntry ProdOrderBatchPlan_ProdOrderPartslistPosReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderBatchPlan_ProdOrderPartslistPos); }
    }

    private ProdOrderPartslist _ProdOrderPartslist;
    public virtual ProdOrderPartslist ProdOrderPartslist
    { 
        get => LazyLoader.Load(this, ref _ProdOrderPartslist);
        set => _ProdOrderPartslist = value;
    }

    public bool ProdOrderPartslist_IsLoaded
    {
        get
        {
            return ProdOrderPartslist != null;
        }
    }

    public virtual ReferenceEntry ProdOrderPartslistReference 
    {
        get { return Context.Entry(this).Reference("ProdOrderPartslist"); }
    }
    
    private ObservableHashSet<ProdOrderPartslistPosFacilityLot> _ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos;
    public virtual ObservableHashSet<ProdOrderPartslistPosFacilityLot> ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos);
        set => _ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos = value;
    }

    public bool ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPosReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos); }
    }

    private ObservableHashSet<ProdOrderPartslistPosRelation> _ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos;
    public virtual ObservableHashSet<ProdOrderPartslistPosRelation> ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos);
        set => _ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos = value;
    }

    public bool ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistPosRelation_SourceProdOrderPartslistPosReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos); }
    }

    private ObservableHashSet<ProdOrderPartslistPosRelation> _ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos;
    public virtual ObservableHashSet<ProdOrderPartslistPosRelation> ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos);
        set => _ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos = value;
    }

    public bool ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistPosRelation_TargetProdOrderPartslistPosReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos); }
    }

    private ObservableHashSet<ProdOrderPartslistPosSplit> _ProdOrderPartslistPosSplit_ProdOrderPartslistPos;
    public virtual ObservableHashSet<ProdOrderPartslistPosSplit> ProdOrderPartslistPosSplit_ProdOrderPartslistPos
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPosSplit_ProdOrderPartslistPos);
        set => _ProdOrderPartslistPosSplit_ProdOrderPartslistPos = value;
    }

    public bool ProdOrderPartslistPosSplit_ProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPosSplit_ProdOrderPartslistPos != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistPosSplit_ProdOrderPartslistPosReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistPosSplit_ProdOrderPartslistPos); }
    }

    private ProdOrderPartslist _SourceProdOrderPartslist;
    public virtual ProdOrderPartslist SourceProdOrderPartslist
    { 
        get => LazyLoader.Load(this, ref _SourceProdOrderPartslist);
        set => _SourceProdOrderPartslist = value;
    }

    public bool SourceProdOrderPartslist_IsLoaded
    {
        get
        {
            return SourceProdOrderPartslist != null;
        }
    }

    public virtual ReferenceEntry SourceProdOrderPartslistReference 
    {
        get { return Context.Entry(this).Reference("SourceProdOrderPartslist"); }
    }
    
    private ObservableHashSet<TandTv3MixPointProdOrderPartslistPos> _TandTv3MixPointProdOrderPartslistPos_ProdOrderPartslistPos;
    public virtual ObservableHashSet<TandTv3MixPointProdOrderPartslistPos> TandTv3MixPointProdOrderPartslistPos_ProdOrderPartslistPos
    {
        get => LazyLoader.Load(this, ref _TandTv3MixPointProdOrderPartslistPos_ProdOrderPartslistPos);
        set => _TandTv3MixPointProdOrderPartslistPos_ProdOrderPartslistPos = value;
    }

    public bool TandTv3MixPointProdOrderPartslistPos_ProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return TandTv3MixPointProdOrderPartslistPos_ProdOrderPartslistPos != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointProdOrderPartslistPos_ProdOrderPartslistPosReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointProdOrderPartslistPos_ProdOrderPartslistPos); }
    }
}
