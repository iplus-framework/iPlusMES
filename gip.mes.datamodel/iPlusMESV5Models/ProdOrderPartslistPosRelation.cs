using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ProdOrderPartslistPosRelation : VBEntityObject, ISequence, ITargetQuantity
{

    public ProdOrderPartslistPosRelation()
    {
    }

    private ProdOrderPartslistPosRelation(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ProdOrderPartslistPosRelationID;
    public Guid ProdOrderPartslistPosRelationID 
    {
        get { return _ProdOrderPartslistPosRelationID; }
        set { SetProperty<Guid>(ref _ProdOrderPartslistPosRelationID, value); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    Guid _TargetProdOrderPartslistPosID;
    public Guid TargetProdOrderPartslistPosID 
    {
        get { return _TargetProdOrderPartslistPosID; }
        set { SetProperty<Guid>(ref _TargetProdOrderPartslistPosID, value); }
    }

    Guid _SourceProdOrderPartslistPosID;
    public Guid SourceProdOrderPartslistPosID 
    {
        get { return _SourceProdOrderPartslistPosID; }
        set { SetProperty<Guid>(ref _SourceProdOrderPartslistPosID, value); }
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

    Guid? _ParentProdOrderPartslistPosRelationID;
    public Guid? ParentProdOrderPartslistPosRelationID 
    {
        get { return _ParentProdOrderPartslistPosRelationID; }
        set { SetProperty<Guid?>(ref _ParentProdOrderPartslistPosRelationID, value); }
    }

    Guid? _ProdOrderBatchID;
    public Guid? ProdOrderBatchID 
    {
        get { return _ProdOrderBatchID; }
        set { SetProperty<Guid?>(ref _ProdOrderBatchID, value); }
    }

    Guid _MDToleranceStateID;
    public Guid MDToleranceStateID 
    {
        get { return _MDToleranceStateID; }
        set { SetProperty<Guid>(ref _MDToleranceStateID, value); }
    }

    Guid _MDProdOrderPartslistPosStateID;
    public Guid MDProdOrderPartslistPosStateID 
    {
        get { return _MDProdOrderPartslistPosStateID; }
        set { SetProperty<Guid>(ref _MDProdOrderPartslistPosStateID, value); }
    }

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
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

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_ProdOrderPartslistPosRelation;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_ProdOrderPartslistPosRelation
    {
        get => LazyLoader.Load(this, ref _FacilityBookingCharge_ProdOrderPartslistPosRelation);
        set => _FacilityBookingCharge_ProdOrderPartslistPosRelation = value;
    }

    public bool FacilityBookingCharge_ProdOrderPartslistPosRelation_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_ProdOrderPartslistPosRelation != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_ProdOrderPartslistPosRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_ProdOrderPartslistPosRelation); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_ProdOrderPartslistPosRelation;
    public virtual ICollection<FacilityBooking> FacilityBooking_ProdOrderPartslistPosRelation
    {
        get => LazyLoader.Load(this, ref _FacilityBooking_ProdOrderPartslistPosRelation);
        set => _FacilityBooking_ProdOrderPartslistPosRelation = value;
    }

    public bool FacilityBooking_ProdOrderPartslistPosRelation_IsLoaded
    {
        get
        {
            return FacilityBooking_ProdOrderPartslistPosRelation != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_ProdOrderPartslistPosRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_ProdOrderPartslistPosRelation); }
    }

    private ICollection<FacilityPreBooking> _FacilityPreBooking_ProdOrderPartslistPosRelation;
    public virtual ICollection<FacilityPreBooking> FacilityPreBooking_ProdOrderPartslistPosRelation
    {
        get => LazyLoader.Load(this, ref _FacilityPreBooking_ProdOrderPartslistPosRelation);
        set => _FacilityPreBooking_ProdOrderPartslistPosRelation = value;
    }

    public bool FacilityPreBooking_ProdOrderPartslistPosRelation_IsLoaded
    {
        get
        {
            return FacilityPreBooking_ProdOrderPartslistPosRelation != null;
        }
    }

    public virtual CollectionEntry FacilityPreBooking_ProdOrderPartslistPosRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityPreBooking_ProdOrderPartslistPosRelation); }
    }

    private ICollection<FacilityReservation> _FacilityReservation_ProdOrderPartslistPosRelation;
    public virtual ICollection<FacilityReservation> FacilityReservation_ProdOrderPartslistPosRelation
    {
        get => LazyLoader.Load(this, ref _FacilityReservation_ProdOrderPartslistPosRelation);
        set => _FacilityReservation_ProdOrderPartslistPosRelation = value;
    }

    public bool FacilityReservation_ProdOrderPartslistPosRelation_IsLoaded
    {
        get
        {
            return FacilityReservation_ProdOrderPartslistPosRelation != null;
        }
    }

    public virtual CollectionEntry FacilityReservation_ProdOrderPartslistPosRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityReservation_ProdOrderPartslistPosRelation); }
    }

    private ICollection<ProdOrderPartslistPosRelation> _ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation;
    public virtual ICollection<ProdOrderPartslistPosRelation> ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation);
        set => _ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation = value;
    }

    public bool ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation); }
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
    
    private ICollection<OrderLog> _OrderLog_ProdOrderPartslistPosRelation;
    public virtual ICollection<OrderLog> OrderLog_ProdOrderPartslistPosRelation
    {
        get => LazyLoader.Load(this, ref _OrderLog_ProdOrderPartslistPosRelation);
        set => _OrderLog_ProdOrderPartslistPosRelation = value;
    }

    public bool OrderLog_ProdOrderPartslistPosRelation_IsLoaded
    {
        get
        {
            return OrderLog_ProdOrderPartslistPosRelation != null;
        }
    }

    public virtual CollectionEntry OrderLog_ProdOrderPartslistPosRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.OrderLog_ProdOrderPartslistPosRelation); }
    }

    private ProdOrderPartslistPosRelation _ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation;
    public virtual ProdOrderPartslistPosRelation ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation
    { 
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation);
        set => _ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation = value;
    }

    public bool ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation != null;
        }
    }

    public virtual ReferenceEntry ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelationReference 
    {
        get { return Context.Entry(this).Reference("ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation"); }
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
    
    private ProdOrderPartslistPos _SourceProdOrderPartslistPos;
    public virtual ProdOrderPartslistPos SourceProdOrderPartslistPos
    { 
        get => LazyLoader.Load(this, ref _SourceProdOrderPartslistPos);
        set => _SourceProdOrderPartslistPos = value;
    }

    public bool SourceProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return SourceProdOrderPartslistPos != null;
        }
    }

    public virtual ReferenceEntry SourceProdOrderPartslistPosReference 
    {
        get { return Context.Entry(this).Reference("SourceProdOrderPartslistPos"); }
    }
    

    private ICollection<TandTv3MixPointProdOrderPartslistPosRelation> _TandTv3MixPointProdOrderPartslistPosRelation_ProdOrderPartslistPosRelation;
    public virtual ICollection<TandTv3MixPointProdOrderPartslistPosRelation> TandTv3MixPointProdOrderPartslistPosRelation_ProdOrderPartslistPosRelation
    {
        get => LazyLoader.Load(this, ref _TandTv3MixPointProdOrderPartslistPosRelation_ProdOrderPartslistPosRelation);
        set => _TandTv3MixPointProdOrderPartslistPosRelation_ProdOrderPartslistPosRelation = value;
    }

    public bool TandTv3MixPointProdOrderPartslistPosRelation_ProdOrderPartslistPosRelation_IsLoaded
    {
        get
        {
            return TandTv3MixPointProdOrderPartslistPosRelation_ProdOrderPartslistPosRelation != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointProdOrderPartslistPosRelation_ProdOrderPartslistPosRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointProdOrderPartslistPosRelation_ProdOrderPartslistPosRelation); }
    }

    private ProdOrderPartslistPos _TargetProdOrderPartslistPos;
    public virtual ProdOrderPartslistPos TargetProdOrderPartslistPos
    { 
        get => LazyLoader.Load(this, ref _TargetProdOrderPartslistPos);
        set => _TargetProdOrderPartslistPos = value;
    }

    public bool TargetProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return TargetProdOrderPartslistPos != null;
        }
    }

    public virtual ReferenceEntry TargetProdOrderPartslistPosReference 
    {
        get { return Context.Entry(this).Reference("TargetProdOrderPartslistPos"); }
    }
    }
