using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv3MixPoint : VBEntityObject 
{

    public TandTv3MixPoint()
    {
    }

    private TandTv3MixPoint(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _TandTv3MixPointID;
    public Guid TandTv3MixPointID 
    {
        get { return _TandTv3MixPointID; }
        set { SetProperty<Guid>(ref _TandTv3MixPointID, value); }
    }

    Guid _TandTv3StepID;
    public Guid TandTv3StepID 
    {
        get { return _TandTv3StepID; }
        set { SetProperty<Guid>(ref _TandTv3StepID, value); }
    }

    bool _IsProductionPoint;
    public bool IsProductionPoint 
    {
        get { return _IsProductionPoint; }
        set { SetProperty<bool>(ref _IsProductionPoint, value); }
    }

    bool _IsInputPoint;
    public bool IsInputPoint 
    {
        get { return _IsInputPoint; }
        set { SetProperty<bool>(ref _IsInputPoint, value); }
    }

    Guid? _InwardLotID;
    public Guid? InwardLotID 
    {
        get { return _InwardLotID; }
        set { SetProperty<Guid?>(ref _InwardLotID, value); }
    }

    Guid _InwardMaterialID;
    public Guid InwardMaterialID 
    {
        get { return _InwardMaterialID; }
        set { SetProperty<Guid>(ref _InwardMaterialID, value); }
    }

    private FacilityLot _InwardLot;
    public virtual FacilityLot InwardLot
    { 
        get => LazyLoader.Load(this, ref _InwardLot);
        set => _InwardLot = value;
    }

    public bool InwardLot_IsLoaded
    {
        get
        {
            return InwardLot != null;
        }
    }

    public virtual ReferenceEntry InwardLotReference 
    {
        get { return Context.Entry(this).Reference("InwardLot"); }
    }
    
    private Material _InwardMaterial;
    public virtual Material InwardMaterial
    { 
        get => LazyLoader.Load(this, ref _InwardMaterial);
        set => _InwardMaterial = value;
    }

    public bool InwardMaterial_IsLoaded
    {
        get
        {
            return InwardMaterial != null;
        }
    }

    public virtual ReferenceEntry InwardMaterialReference 
    {
        get { return Context.Entry(this).Reference("InwardMaterial"); }
    }
    
    private ICollection<TandTv3MixPointDeliveryNotePos> _TandTv3MixPointDeliveryNotePos_TandTv3MixPoint;
    public virtual ICollection<TandTv3MixPointDeliveryNotePos> TandTv3MixPointDeliveryNotePos_TandTv3MixPoint
    {
        get => LazyLoader.Load(this, ref _TandTv3MixPointDeliveryNotePos_TandTv3MixPoint);
        set => _TandTv3MixPointDeliveryNotePos_TandTv3MixPoint = value;
    }

    public bool TandTv3MixPointDeliveryNotePos_TandTv3MixPoint_IsLoaded
    {
        get
        {
            return TandTv3MixPointDeliveryNotePos_TandTv3MixPoint != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointDeliveryNotePos_TandTv3MixPointReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointDeliveryNotePos_TandTv3MixPoint); }
    }

    private ICollection<TandTv3MixPointFacility> _TandTv3MixPointFacility_TandTv3MixPoint;
    public virtual ICollection<TandTv3MixPointFacility> TandTv3MixPointFacility_TandTv3MixPoint
    {
        get => LazyLoader.Load(this, ref _TandTv3MixPointFacility_TandTv3MixPoint);
        set => _TandTv3MixPointFacility_TandTv3MixPoint = value;
    }

    public bool TandTv3MixPointFacility_TandTv3MixPoint_IsLoaded
    {
        get
        {
            return TandTv3MixPointFacility_TandTv3MixPoint != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointFacility_TandTv3MixPointReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointFacility_TandTv3MixPoint); }
    }

    private ICollection<TandTv3MixPointFacilityBookingCharge> _TandTv3MixPointFacilityBookingCharge_TandTv3MixPoint;
    public virtual ICollection<TandTv3MixPointFacilityBookingCharge> TandTv3MixPointFacilityBookingCharge_TandTv3MixPoint
    {
        get => LazyLoader.Load(this, ref _TandTv3MixPointFacilityBookingCharge_TandTv3MixPoint);
        set => _TandTv3MixPointFacilityBookingCharge_TandTv3MixPoint = value;
    }

    public bool TandTv3MixPointFacilityBookingCharge_TandTv3MixPoint_IsLoaded
    {
        get
        {
            return TandTv3MixPointFacilityBookingCharge_TandTv3MixPoint != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointFacilityBookingCharge_TandTv3MixPointReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointFacilityBookingCharge_TandTv3MixPoint); }
    }

    private ICollection<TandTv3MixPointFacilityLot> _TandTv3MixPointFacilityLot_TandTv3MixPoint;
    public virtual ICollection<TandTv3MixPointFacilityLot> TandTv3MixPointFacilityLot_TandTv3MixPoint
    {
        get => LazyLoader.Load(this, ref _TandTv3MixPointFacilityLot_TandTv3MixPoint);
        set => _TandTv3MixPointFacilityLot_TandTv3MixPoint = value;
    }

    public bool TandTv3MixPointFacilityLot_TandTv3MixPoint_IsLoaded
    {
        get
        {
            return TandTv3MixPointFacilityLot_TandTv3MixPoint != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointFacilityLot_TandTv3MixPointReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointFacilityLot_TandTv3MixPoint); }
    }

    private ICollection<TandTv3MixPointFacilityPreBooking> _TandTv3MixPointFacilityPreBooking_TandTv3MixPoint;
    public virtual ICollection<TandTv3MixPointFacilityPreBooking> TandTv3MixPointFacilityPreBooking_TandTv3MixPoint
    {
        get => LazyLoader.Load(this, ref _TandTv3MixPointFacilityPreBooking_TandTv3MixPoint);
        set => _TandTv3MixPointFacilityPreBooking_TandTv3MixPoint = value;
    }

    public bool TandTv3MixPointFacilityPreBooking_TandTv3MixPoint_IsLoaded
    {
        get
        {
            return TandTv3MixPointFacilityPreBooking_TandTv3MixPoint != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointFacilityPreBooking_TandTv3MixPointReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointFacilityPreBooking_TandTv3MixPoint); }
    }

    private ICollection<TandTv3MixPointInOrderPos> _TandTv3MixPointInOrderPos_TandTv3MixPoint;
    public virtual ICollection<TandTv3MixPointInOrderPos> TandTv3MixPointInOrderPos_TandTv3MixPoint
    {
        get => LazyLoader.Load(this, ref _TandTv3MixPointInOrderPos_TandTv3MixPoint);
        set => _TandTv3MixPointInOrderPos_TandTv3MixPoint = value;
    }

    public bool TandTv3MixPointInOrderPos_TandTv3MixPoint_IsLoaded
    {
        get
        {
            return TandTv3MixPointInOrderPos_TandTv3MixPoint != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointInOrderPos_TandTv3MixPointReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointInOrderPos_TandTv3MixPoint); }
    }

    private ICollection<TandTv3MixPointOutOrderPos> _TandTv3MixPointOutOrderPos_TandTv3MixPoint;
    public virtual ICollection<TandTv3MixPointOutOrderPos> TandTv3MixPointOutOrderPos_TandTv3MixPoint
    {
        get => LazyLoader.Load(this, ref _TandTv3MixPointOutOrderPos_TandTv3MixPoint);
        set => _TandTv3MixPointOutOrderPos_TandTv3MixPoint = value;
    }

    public bool TandTv3MixPointOutOrderPos_TandTv3MixPoint_IsLoaded
    {
        get
        {
            return TandTv3MixPointOutOrderPos_TandTv3MixPoint != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointOutOrderPos_TandTv3MixPointReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointOutOrderPos_TandTv3MixPoint); }
    }

    private ICollection<TandTv3MixPointPickingPos> _TandTv3MixPointPickingPos_TandTv3MixPoint;
    public virtual ICollection<TandTv3MixPointPickingPos> TandTv3MixPointPickingPos_TandTv3MixPoint
    {
        get => LazyLoader.Load(this, ref _TandTv3MixPointPickingPos_TandTv3MixPoint);
        set => _TandTv3MixPointPickingPos_TandTv3MixPoint = value;
    }

    public bool TandTv3MixPointPickingPos_TandTv3MixPoint_IsLoaded
    {
        get
        {
            return TandTv3MixPointPickingPos_TandTv3MixPoint != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointPickingPos_TandTv3MixPointReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointPickingPos_TandTv3MixPoint); }
    }

    private ICollection<TandTv3MixPointProdOrderPartslistPos> _TandTv3MixPointProdOrderPartslistPos_TandTv3MixPoint;
    public virtual ICollection<TandTv3MixPointProdOrderPartslistPos> TandTv3MixPointProdOrderPartslistPos_TandTv3MixPoint
    {
        get => LazyLoader.Load(this, ref _TandTv3MixPointProdOrderPartslistPos_TandTv3MixPoint);
        set => _TandTv3MixPointProdOrderPartslistPos_TandTv3MixPoint = value;
    }

    public bool TandTv3MixPointProdOrderPartslistPos_TandTv3MixPoint_IsLoaded
    {
        get
        {
            return TandTv3MixPointProdOrderPartslistPos_TandTv3MixPoint != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointProdOrderPartslistPos_TandTv3MixPointReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointProdOrderPartslistPos_TandTv3MixPoint); }
    }

    private ICollection<TandTv3MixPointProdOrderPartslistPosRelation> _TandTv3MixPointProdOrderPartslistPosRelation_TandTv3MixPoint;
    public virtual ICollection<TandTv3MixPointProdOrderPartslistPosRelation> TandTv3MixPointProdOrderPartslistPosRelation_TandTv3MixPoint
    {
        get => LazyLoader.Load(this, ref _TandTv3MixPointProdOrderPartslistPosRelation_TandTv3MixPoint);
        set => _TandTv3MixPointProdOrderPartslistPosRelation_TandTv3MixPoint = value;
    }

    public bool TandTv3MixPointProdOrderPartslistPosRelation_TandTv3MixPoint_IsLoaded
    {
        get
        {
            return TandTv3MixPointProdOrderPartslistPosRelation_TandTv3MixPoint != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointProdOrderPartslistPosRelation_TandTv3MixPointReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointProdOrderPartslistPosRelation_TandTv3MixPoint); }
    }

    private ICollection<TandTv3MixPointRelation> _TandTv3MixPointRelation_SourceTandTv3MixPoint;
    public virtual ICollection<TandTv3MixPointRelation> TandTv3MixPointRelation_SourceTandTv3MixPoint
    {
        get => LazyLoader.Load(this, ref _TandTv3MixPointRelation_SourceTandTv3MixPoint);
        set => _TandTv3MixPointRelation_SourceTandTv3MixPoint = value;
    }

    public bool TandTv3MixPointRelation_SourceTandTv3MixPoint_IsLoaded
    {
        get
        {
            return TandTv3MixPointRelation_SourceTandTv3MixPoint != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointRelation_SourceTandTv3MixPointReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointRelation_SourceTandTv3MixPoint); }
    }

    private ICollection<TandTv3MixPointRelation> _TandTv3MixPointRelation_TargetTandTv3MixPoint;
    public virtual ICollection<TandTv3MixPointRelation> TandTv3MixPointRelation_TargetTandTv3MixPoint
    {
        get => LazyLoader.Load(this, ref _TandTv3MixPointRelation_TargetTandTv3MixPoint);
        set => _TandTv3MixPointRelation_TargetTandTv3MixPoint = value;
    }

    public bool TandTv3MixPointRelation_TargetTandTv3MixPoint_IsLoaded
    {
        get
        {
            return TandTv3MixPointRelation_TargetTandTv3MixPoint != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointRelation_TargetTandTv3MixPointReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointRelation_TargetTandTv3MixPoint); }
    }

    private TandTv3Step _TandTv3Step;
    public virtual TandTv3Step TandTv3Step
    { 
        get => LazyLoader.Load(this, ref _TandTv3Step);
        set => _TandTv3Step = value;
    }

    public bool TandTv3Step_IsLoaded
    {
        get
        {
            return TandTv3Step != null;
        }
    }

    public virtual ReferenceEntry TandTv3StepReference 
    {
        get { return Context.Entry(this).Reference("TandTv3Step"); }
    }
    }
