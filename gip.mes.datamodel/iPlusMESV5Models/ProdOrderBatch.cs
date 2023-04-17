using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ProdOrderBatch : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public ProdOrderBatch()
    {
    }

    private ProdOrderBatch(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _ProdOrderBatchID;
    public Guid ProdOrderBatchID 
    {
        get { return _ProdOrderBatchID; }
        set { SetProperty<Guid>(ref _ProdOrderBatchID, value); }
    }

    Guid _ProdOrderPartslistID;
    public Guid ProdOrderPartslistID 
    {
        get { return _ProdOrderPartslistID; }
        set { SetProperty<Guid>(ref _ProdOrderPartslistID, value); }
    }

    int _BatchSeqNo;
    public int BatchSeqNo 
    {
        get { return _BatchSeqNo; }
        set { SetProperty<int>(ref _BatchSeqNo, value); }
    }

    Guid _MDProdOrderStateID;
    public Guid MDProdOrderStateID 
    {
        get { return _MDProdOrderStateID; }
        set { SetProperty<Guid>(ref _MDProdOrderStateID, value); }
    }

    string _ProdOrderBatchNo;
    public string ProdOrderBatchNo 
    {
        get { return _ProdOrderBatchNo; }
        set { SetProperty<string>(ref _ProdOrderBatchNo, value); }
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

    Guid? _ProdOrderBatchPlanID;
    public Guid? ProdOrderBatchPlanID 
    {
        get { return _ProdOrderBatchPlanID; }
        set { SetProperty<Guid?>(ref _ProdOrderBatchPlanID, value); }
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
    
    private ICollection<ProdOrderPartslistPos> _ProdOrderPartslistPos_ProdOrderBatch;
    public virtual ICollection<ProdOrderPartslistPos> ProdOrderPartslistPos_ProdOrderBatch
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPos_ProdOrderBatch);
        set => _ProdOrderPartslistPos_ProdOrderBatch = value;
    }

    public bool ProdOrderPartslistPos_ProdOrderBatch_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPos_ProdOrderBatch != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistPos_ProdOrderBatchReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistPos_ProdOrderBatch); }
    }

    private ICollection<ProdOrderPartslistPosRelation> _ProdOrderPartslistPosRelation_ProdOrderBatch;
    public virtual ICollection<ProdOrderPartslistPosRelation> ProdOrderPartslistPosRelation_ProdOrderBatch
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPosRelation_ProdOrderBatch);
        set => _ProdOrderPartslistPosRelation_ProdOrderBatch = value;
    }

    public bool ProdOrderPartslistPosRelation_ProdOrderBatch_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPosRelation_ProdOrderBatch != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistPosRelation_ProdOrderBatchReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistPosRelation_ProdOrderBatch); }
    }
}
