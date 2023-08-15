using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ProdOrderBatchPlan : VBEntityObject, IInsertInfo, IUpdateInfo, ISequence, IScheduledOrder
{

    public ProdOrderBatchPlan()
    {
    }

    private ProdOrderBatchPlan(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ProdOrderBatchPlanID;
    public Guid ProdOrderBatchPlanID 
    {
        get { return _ProdOrderBatchPlanID; }
        set { SetProperty<Guid>(ref _ProdOrderBatchPlanID, value); }
    }

    Guid _ProdOrderPartslistID;
    public Guid ProdOrderPartslistID 
    {
        get { return _ProdOrderPartslistID; }
        set { SetProperty<Guid>(ref _ProdOrderPartslistID, value); }
    }

    Guid? _VBiACClassWFID;
    public Guid? VBiACClassWFID 
    {
        get { return _VBiACClassWFID; }
        set { SetProperty<Guid?>(ref _VBiACClassWFID, value); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    int? _BatchNoFrom;
    public int? BatchNoFrom 
    {
        get { return _BatchNoFrom; }
        set { SetProperty<int?>(ref _BatchNoFrom, value); }
    }

    int? _BatchNoTo;
    public int? BatchNoTo 
    {
        get { return _BatchNoTo; }
        set { SetProperty<int?>(ref _BatchNoTo, value); }
    }

    int _BatchTargetCount;
    public int BatchTargetCount 
    {
        get { return _BatchTargetCount; }
        set { SetProperty<int>(ref _BatchTargetCount, value); }
    }

    int _BatchActualCount;
    public int BatchActualCount 
    {
        get { return _BatchActualCount; }
        set { SetProperty<int>(ref _BatchActualCount, value); }
    }

    double _BatchSize;
    public double BatchSize 
    {
        get { return _BatchSize; }
        set { SetProperty<double>(ref _BatchSize, value); }
    }

    double _TotalSize;
    public double TotalSize 
    {
        get { return _TotalSize; }
        set { SetProperty<double>(ref _TotalSize, value); }
    }

    short _PlanModeIndex;
    public short PlanModeIndex 
    {
        get { return _PlanModeIndex; }
        set { SetProperty<short>(ref _PlanModeIndex, value); }
    }

    short _PlanStateIndex;
    public short PlanStateIndex 
    {
        get { return _PlanStateIndex; }
        set { SetProperty<short>(ref _PlanStateIndex, value); }
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

    Guid? _ProdOrderPartslistPosID;
    public Guid? ProdOrderPartslistPosID 
    {
        get { return _ProdOrderPartslistPosID; }
        set { SetProperty<Guid?>(ref _ProdOrderPartslistPosID, value); }
    }

    Guid? _MaterialWFACClassMethodID;
    public Guid? MaterialWFACClassMethodID 
    {
        get { return _MaterialWFACClassMethodID; }
        set { SetProperty<Guid?>(ref _MaterialWFACClassMethodID, value); }
    }

    bool _IsValidated;
    public bool IsValidated 
    {
        get { return _IsValidated; }
        set { SetProperty<bool>(ref _IsValidated, value); }
    }

    DateTime _PlannedStartDate;
    public DateTime PlannedStartDate 
    {
        get { return _PlannedStartDate; }
        set { SetProperty<DateTime>(ref _PlannedStartDate, value); }
    }

    int? _ScheduledOrder;
    public int? ScheduledOrder 
    {
        get { return _ScheduledOrder; }
        set { SetProperty<int?>(ref _ScheduledOrder, value); }
    }

    DateTime? _ScheduledStartDate;
    public DateTime? ScheduledStartDate 
    {
        get { return _ScheduledStartDate; }
        set { SetProperty<DateTime?>(ref _ScheduledStartDate, value); }
    }

    DateTime? _ScheduledEndDate;
    public DateTime? ScheduledEndDate 
    {
        get { return _ScheduledEndDate; }
        set { SetProperty<DateTime?>(ref _ScheduledEndDate, value); }
    }

    DateTime? _CalculatedStartDate;
    public DateTime? CalculatedStartDate 
    {
        get { return _CalculatedStartDate; }
        set { SetProperty<DateTime?>(ref _CalculatedStartDate, value); }
    }

    DateTime? _CalculatedEndDate;
    public DateTime? CalculatedEndDate 
    {
        get { return _CalculatedEndDate; }
        set { SetProperty<DateTime?>(ref _CalculatedEndDate, value); }
    }

    int? _PartialTargetCount;
    public int? PartialTargetCount 
    {
        get { return _PartialTargetCount; }
        set { SetProperty<int?>(ref _PartialTargetCount, value); }
    }

    int? _PartialActualCount;
    public int? PartialActualCount 
    {
        get { return _PartialActualCount; }
        set { SetProperty<int?>(ref _PartialActualCount, value); }
    }

    double? _StartOffsetSecAVG;
    public double? StartOffsetSecAVG 
    {
        get { return _StartOffsetSecAVG; }
        set { SetProperty<double?>(ref _StartOffsetSecAVG, value); }
    }

    double? _DurationSecAVG;
    public double? DurationSecAVG 
    {
        get { return _DurationSecAVG; }
        set { SetProperty<double?>(ref _DurationSecAVG, value); }
    }

    Guid? _MDBatchPlanGroupID;
    public Guid? MDBatchPlanGroupID 
    {
        get { return _MDBatchPlanGroupID; }
        set { SetProperty<Guid?>(ref _MDBatchPlanGroupID, value); }
    }

    private ObservableHashSet<FacilityReservation> _FacilityReservation_ProdOrderBatchPlan;
    public virtual ObservableHashSet<FacilityReservation> FacilityReservation_ProdOrderBatchPlan
    {
        get => LazyLoader.Load(this, ref _FacilityReservation_ProdOrderBatchPlan);
        set => _FacilityReservation_ProdOrderBatchPlan = value;
    }

    public bool FacilityReservation_ProdOrderBatchPlan_IsLoaded
    {
        get
        {
            return FacilityReservation_ProdOrderBatchPlan != null;
        }
    }

    public virtual CollectionEntry FacilityReservation_ProdOrderBatchPlanReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityReservation_ProdOrderBatchPlan); }
    }

    private MDBatchPlanGroup _MDBatchPlanGroup;
    public virtual MDBatchPlanGroup MDBatchPlanGroup
    { 
        get => LazyLoader.Load(this, ref _MDBatchPlanGroup);
        set => _MDBatchPlanGroup = value;
    }

    public bool MDBatchPlanGroup_IsLoaded
    {
        get
        {
            return MDBatchPlanGroup != null;
        }
    }

    public virtual ReferenceEntry MDBatchPlanGroupReference 
    {
        get { return Context.Entry(this).Reference("MDBatchPlanGroup"); }
    }
    
    private MaterialWFACClassMethod _MaterialWFACClassMethod;
    public virtual MaterialWFACClassMethod MaterialWFACClassMethod
    { 
        get => LazyLoader.Load(this, ref _MaterialWFACClassMethod);
        set => _MaterialWFACClassMethod = value;
    }

    public bool MaterialWFACClassMethod_IsLoaded
    {
        get
        {
            return MaterialWFACClassMethod != null;
        }
    }

    public virtual ReferenceEntry MaterialWFACClassMethodReference 
    {
        get { return Context.Entry(this).Reference("MaterialWFACClassMethod"); }
    }
    
    private ObservableHashSet<ProdOrderBatch> _ProdOrderBatch_ProdOrderBatchPlan;
    public virtual ObservableHashSet<ProdOrderBatch> ProdOrderBatch_ProdOrderBatchPlan
    {
        get => LazyLoader.Load(this, ref _ProdOrderBatch_ProdOrderBatchPlan);
        set => _ProdOrderBatch_ProdOrderBatchPlan = value;
    }

    public bool ProdOrderBatch_ProdOrderBatchPlan_IsLoaded
    {
        get
        {
            return ProdOrderBatch_ProdOrderBatchPlan != null;
        }
    }

    public virtual CollectionEntry ProdOrderBatch_ProdOrderBatchPlanReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderBatch_ProdOrderBatchPlan); }
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
    
    private ACClassWF _VBiACClassWF;
    public virtual ACClassWF VBiACClassWF
    { 
        get => LazyLoader.Load(this, ref _VBiACClassWF);
        set => _VBiACClassWF = value;
    }

    public bool VBiACClassWF_IsLoaded
    {
        get
        {
            return VBiACClassWF != null;
        }
    }

    public virtual ReferenceEntry VBiACClassWFReference 
    {
        get { return Context.Entry(this).Reference("VBiACClassWF"); }
    }
    }
