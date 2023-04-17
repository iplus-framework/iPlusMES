using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ProdOrderPartslist : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public ProdOrderPartslist()
    {
    }

    private ProdOrderPartslist(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _ProdOrderPartslistID;
    public Guid ProdOrderPartslistID 
    {
        get { return _ProdOrderPartslistID; }
        set { SetProperty<Guid>(ref _ProdOrderPartslistID, value); }
    }

    Guid _ProdOrderID;
    public Guid ProdOrderID 
    {
        get { return _ProdOrderID; }
        set { SetProperty<Guid>(ref _ProdOrderID, value); }
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

    bool _IsEnabled;
    public bool IsEnabled 
    {
        get { return _IsEnabled; }
        set { SetProperty<bool>(ref _IsEnabled, value); }
    }

    string _LossComment;
    public string LossComment 
    {
        get { return _LossComment; }
        set { SetProperty<string>(ref _LossComment, value); }
    }

    DateTime? _ProdUserEndDate;
    public DateTime? ProdUserEndDate 
    {
        get { return _ProdUserEndDate; }
        set { SetProperty<DateTime?>(ref _ProdUserEndDate, value); }
    }

    string _ProdUserEndName;
    public string ProdUserEndName 
    {
        get { return _ProdUserEndName; }
        set { SetProperty<string>(ref _ProdUserEndName, value); }
    }

    DateTime? _DepartmentUserDate;
    public DateTime? DepartmentUserDate 
    {
        get { return _DepartmentUserDate; }
        set { SetProperty<DateTime?>(ref _DepartmentUserDate, value); }
    }

    string _DepartmentUserName;
    public string DepartmentUserName 
    {
        get { return _DepartmentUserName; }
        set { SetProperty<string>(ref _DepartmentUserName, value); }
    }

    DateTime? _StartDate;
    public DateTime? StartDate 
    {
        get { return _StartDate; }
        set { SetProperty<DateTime?>(ref _StartDate, value); }
    }

    DateTime? _EndDate;
    public DateTime? EndDate 
    {
        get { return _EndDate; }
        set { SetProperty<DateTime?>(ref _EndDate, value); }
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

    Guid? _PartslistID;
    public Guid? PartslistID 
    {
        get { return _PartslistID; }
        set { SetProperty<Guid?>(ref _PartslistID, value); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    Guid _MDProdOrderStateID;
    public Guid MDProdOrderStateID 
    {
        get { return _MDProdOrderStateID; }
        set { SetProperty<Guid>(ref _MDProdOrderStateID, value); }
    }

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
    }

    Guid? _VBiACProgramID;
    public Guid? VBiACProgramID 
    {
        get { return _VBiACProgramID; }
        set { SetProperty<Guid?>(ref _VBiACProgramID, value); }
    }

    string _ExternProdOrderNo;
    public string ExternProdOrderNo 
    {
        get { return _ExternProdOrderNo; }
        set { SetProperty<string>(ref _ExternProdOrderNo, value); }
    }

    DateTime _LastFormulaChange;
    public DateTime LastFormulaChange 
    {
        get { return _LastFormulaChange; }
        set { SetProperty<DateTime>(ref _LastFormulaChange, value); }
    }

    double _ActualQuantityScrapUOM;
    public double ActualQuantityScrapUOM 
    {
        get { return _ActualQuantityScrapUOM; }
        set { SetProperty<double>(ref _ActualQuantityScrapUOM, value); }
    }

    double? _InputQForActualOutputPer;
    public double? InputQForActualOutputPer 
    {
        get { return _InputQForActualOutputPer; }
        set { SetProperty<double?>(ref _InputQForActualOutputPer, value); }
    }

    double? _InputQForGoodActualOutputPer;
    public double? InputQForGoodActualOutputPer 
    {
        get { return _InputQForGoodActualOutputPer; }
        set { SetProperty<double?>(ref _InputQForGoodActualOutputPer, value); }
    }

    double? _InputQForScrapActualOutputPer;
    public double? InputQForScrapActualOutputPer 
    {
        get { return _InputQForScrapActualOutputPer; }
        set { SetProperty<double?>(ref _InputQForScrapActualOutputPer, value); }
    }

    double? _InputQForFinalActualOutputPer;
    public double? InputQForFinalActualOutputPer 
    {
        get { return _InputQForFinalActualOutputPer; }
        set { SetProperty<double?>(ref _InputQForFinalActualOutputPer, value); }
    }

    double? _InputQForFinalGoodActualOutputPer;
    public double? InputQForFinalGoodActualOutputPer 
    {
        get { return _InputQForFinalGoodActualOutputPer; }
        set { SetProperty<double?>(ref _InputQForFinalGoodActualOutputPer, value); }
    }

    double? _InputQForFinalScrapActualOutputPer;
    public double? InputQForFinalScrapActualOutputPer 
    {
        get { return _InputQForFinalScrapActualOutputPer; }
        set { SetProperty<double?>(ref _InputQForFinalScrapActualOutputPer, value); }
    }

    private MDProdOrderState _MDProdOrderState;
    public virtual MDProdOrderState MDProdOrderState
    { 
        get => LazyLoader.Load(this, ref _MDProdOrderState);
        set => _MDProdOrderState = value;
    }

    public bool MDProdOrderState_IsLoaded
    {
        get
        {
            return MDProdOrderState != null;
        }
    }

    public virtual ReferenceEntry MDProdOrderStateReference 
    {
        get { return Context.Entry(this).Reference("MDProdOrderState"); }
    }
    
    private Partslist _Partslist;
    public virtual Partslist Partslist
    { 
        get => LazyLoader.Load(this, ref _Partslist);
        set => _Partslist = value;
    }

    public bool Partslist_IsLoaded
    {
        get
        {
            return Partslist != null;
        }
    }

    public virtual ReferenceEntry PartslistReference 
    {
        get { return Context.Entry(this).Reference("Partslist"); }
    }
    
    private ICollection<PlanningMRProposal> _PlanningMRProposal_ProdOrderPartslist;
    public virtual ICollection<PlanningMRProposal> PlanningMRProposal_ProdOrderPartslist
    {
        get => LazyLoader.Load(this, ref _PlanningMRProposal_ProdOrderPartslist);
        set => _PlanningMRProposal_ProdOrderPartslist = value;
    }

    public bool PlanningMRProposal_ProdOrderPartslist_IsLoaded
    {
        get
        {
            return PlanningMRProposal_ProdOrderPartslist != null;
        }
    }

    public virtual CollectionEntry PlanningMRProposal_ProdOrderPartslistReference
    {
        get { return Context.Entry(this).Collection(c => c.PlanningMRProposal_ProdOrderPartslist); }
    }

    private ProdOrder _ProdOrder;
    public virtual ProdOrder ProdOrder
    { 
        get => LazyLoader.Load(this, ref _ProdOrder);
        set => _ProdOrder = value;
    }

    public bool ProdOrder_IsLoaded
    {
        get
        {
            return ProdOrder != null;
        }
    }

    public virtual ReferenceEntry ProdOrderReference 
    {
        get { return Context.Entry(this).Reference("ProdOrder"); }
    }
    
    private ICollection<ProdOrderBatchPlan> _ProdOrderBatchPlan_ProdOrderPartslist;
    public virtual ICollection<ProdOrderBatchPlan> ProdOrderBatchPlan_ProdOrderPartslist
    {
        get => LazyLoader.Load(this, ref _ProdOrderBatchPlan_ProdOrderPartslist);
        set => _ProdOrderBatchPlan_ProdOrderPartslist = value;
    }

    public bool ProdOrderBatchPlan_ProdOrderPartslist_IsLoaded
    {
        get
        {
            return ProdOrderBatchPlan_ProdOrderPartslist != null;
        }
    }

    public virtual CollectionEntry ProdOrderBatchPlan_ProdOrderPartslistReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderBatchPlan_ProdOrderPartslist); }
    }

    private ICollection<ProdOrderBatch> _ProdOrderBatch_ProdOrderPartslist;
    public virtual ICollection<ProdOrderBatch> ProdOrderBatch_ProdOrderPartslist
    {
        get => LazyLoader.Load(this, ref _ProdOrderBatch_ProdOrderPartslist);
        set => _ProdOrderBatch_ProdOrderPartslist = value;
    }

    public bool ProdOrderBatch_ProdOrderPartslist_IsLoaded
    {
        get
        {
            return ProdOrderBatch_ProdOrderPartslist != null;
        }
    }

    public virtual CollectionEntry ProdOrderBatch_ProdOrderPartslistReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderBatch_ProdOrderPartslist); }
    }

    private ICollection<ProdOrderPartslistConfig> _ProdOrderPartslistConfig_ProdOrderPartslist;
    public virtual ICollection<ProdOrderPartslistConfig> ProdOrderPartslistConfig_ProdOrderPartslist
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslistConfig_ProdOrderPartslist);
        set => _ProdOrderPartslistConfig_ProdOrderPartslist = value;
    }

    public bool ProdOrderPartslistConfig_ProdOrderPartslist_IsLoaded
    {
        get
        {
            return ProdOrderPartslistConfig_ProdOrderPartslist != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistConfig_ProdOrderPartslistReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistConfig_ProdOrderPartslist); }
    }

    private ICollection<ProdOrderPartslistPos> _ProdOrderPartslistPos_ProdOrderPartslist;
    public virtual ICollection<ProdOrderPartslistPos> ProdOrderPartslistPos_ProdOrderPartslist
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPos_ProdOrderPartslist);
        set => _ProdOrderPartslistPos_ProdOrderPartslist = value;
    }

    public bool ProdOrderPartslistPos_ProdOrderPartslist_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPos_ProdOrderPartslist != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistPos_ProdOrderPartslistReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistPos_ProdOrderPartslist); }
    }

    private ICollection<ProdOrderPartslistPos> _ProdOrderPartslistPos_SourceProdOrderPartslist;
    public virtual ICollection<ProdOrderPartslistPos> ProdOrderPartslistPos_SourceProdOrderPartslist
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPos_SourceProdOrderPartslist);
        set => _ProdOrderPartslistPos_SourceProdOrderPartslist = value;
    }

    public bool ProdOrderPartslistPos_SourceProdOrderPartslist_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPos_SourceProdOrderPartslist != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistPos_SourceProdOrderPartslistReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistPos_SourceProdOrderPartslist); }
    }

    private ICollection<TandTv2StepItem> _TandTv2StepItem_ProdOrderPartslist;
    public virtual ICollection<TandTv2StepItem> TandTv2StepItem_ProdOrderPartslist
    {
        get => LazyLoader.Load(this, ref _TandTv2StepItem_ProdOrderPartslist);
        set => _TandTv2StepItem_ProdOrderPartslist = value;
    }

    public bool TandTv2StepItem_ProdOrderPartslist_IsLoaded
    {
        get
        {
            return TandTv2StepItem_ProdOrderPartslist != null;
        }
    }

    public virtual CollectionEntry TandTv2StepItem_ProdOrderPartslistReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv2StepItem_ProdOrderPartslist); }
    }

    private ACProgram _VBiACProgram;
    public virtual ACProgram VBiACProgram
    { 
        get => LazyLoader.Load(this, ref _VBiACProgram);
        set => _VBiACProgram = value;
    }

    public bool VBiACProgram_IsLoaded
    {
        get
        {
            return VBiACProgram != null;
        }
    }

    public virtual ReferenceEntry VBiACProgramReference 
    {
        get { return Context.Entry(this).Reference("VBiACProgram"); }
    }
    }
