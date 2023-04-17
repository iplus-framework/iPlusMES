using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv2StepItem : VBEntityObject 
{

    public TandTv2StepItem()
    {
    }

    private TandTv2StepItem(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _TandTv2StepItemID;
    public Guid TandTv2StepItemID 
    {
        get { return _TandTv2StepItemID; }
        set { SetProperty<Guid>(ref _TandTv2StepItemID, value); }
    }

    Guid _TandTv2StepID;
    public Guid TandTv2StepID 
    {
        get { return _TandTv2StepID; }
        set { SetProperty<Guid>(ref _TandTv2StepID, value); }
    }

    string _TandTv2ItemTypeID;
    public string TandTv2ItemTypeID 
    {
        get { return _TandTv2ItemTypeID; }
        set { SetProperty<string>(ref _TandTv2ItemTypeID, value); }
    }

    string _TandTv2OperationID;
    public string TandTv2OperationID 
    {
        get { return _TandTv2OperationID; }
        set { SetProperty<string>(ref _TandTv2OperationID, value); }
    }

    int _SubStepNo;
    public int SubStepNo 
    {
        get { return _SubStepNo; }
        set { SetProperty<int>(ref _SubStepNo, value); }
    }

    Guid _PrimaryKeyID;
    public Guid PrimaryKeyID 
    {
        get { return _PrimaryKeyID; }
        set { SetProperty<Guid>(ref _PrimaryKeyID, value); }
    }

    string _ACIdentifier;
    public override string ACIdentifier 
    {
        get { return _ACIdentifier; }
        set { SetProperty<string>(ref _ACIdentifier, value); }
    }

    string _ACCaptionTranslation;
    public string ACCaptionTranslation 
    {
        get { return _ACCaptionTranslation; }
        set { SetProperty<string>(ref _ACCaptionTranslation, value); }
    }

    DateTime? _InsertDate;
    public DateTime? InsertDate 
    {
        get { return _InsertDate; }
        set { SetProperty<DateTime?>(ref _InsertDate, value); }
    }

    Guid? _ACClassID;
    public Guid? ACClassID 
    {
        get { return _ACClassID; }
        set { SetProperty<Guid?>(ref _ACClassID, value); }
    }

    Guid? _ProdOrderID;
    public Guid? ProdOrderID 
    {
        get { return _ProdOrderID; }
        set { SetProperty<Guid?>(ref _ProdOrderID, value); }
    }

    Guid? _ProdOrderPartslistID;
    public Guid? ProdOrderPartslistID 
    {
        get { return _ProdOrderPartslistID; }
        set { SetProperty<Guid?>(ref _ProdOrderPartslistID, value); }
    }

    Guid? _ProdOrderPartslistPosID;
    public Guid? ProdOrderPartslistPosID 
    {
        get { return _ProdOrderPartslistPosID; }
        set { SetProperty<Guid?>(ref _ProdOrderPartslistPosID, value); }
    }

    Guid? _ProdOrderPartslistPosRelationID;
    public Guid? ProdOrderPartslistPosRelationID 
    {
        get { return _ProdOrderPartslistPosRelationID; }
        set { SetProperty<Guid?>(ref _ProdOrderPartslistPosRelationID, value); }
    }

    Guid? _FacilityBookingID;
    public Guid? FacilityBookingID 
    {
        get { return _FacilityBookingID; }
        set { SetProperty<Guid?>(ref _FacilityBookingID, value); }
    }

    Guid? _FacilityBookingChargeID;
    public Guid? FacilityBookingChargeID 
    {
        get { return _FacilityBookingChargeID; }
        set { SetProperty<Guid?>(ref _FacilityBookingChargeID, value); }
    }

    Guid? _FacilityChargeID;
    public Guid? FacilityChargeID 
    {
        get { return _FacilityChargeID; }
        set { SetProperty<Guid?>(ref _FacilityChargeID, value); }
    }

    Guid? _FacilityLotID;
    public Guid? FacilityLotID 
    {
        get { return _FacilityLotID; }
        set { SetProperty<Guid?>(ref _FacilityLotID, value); }
    }

    Guid? _DeliveryNoteID;
    public Guid? DeliveryNoteID 
    {
        get { return _DeliveryNoteID; }
        set { SetProperty<Guid?>(ref _DeliveryNoteID, value); }
    }

    Guid? _DeliveryNotePosID;
    public Guid? DeliveryNotePosID 
    {
        get { return _DeliveryNotePosID; }
        set { SetProperty<Guid?>(ref _DeliveryNotePosID, value); }
    }

    Guid? _InOrderID;
    public Guid? InOrderID 
    {
        get { return _InOrderID; }
        set { SetProperty<Guid?>(ref _InOrderID, value); }
    }

    Guid? _InOrderPosID;
    public Guid? InOrderPosID 
    {
        get { return _InOrderPosID; }
        set { SetProperty<Guid?>(ref _InOrderPosID, value); }
    }

    Guid? _OutOrderID;
    public Guid? OutOrderID 
    {
        get { return _OutOrderID; }
        set { SetProperty<Guid?>(ref _OutOrderID, value); }
    }

    Guid? _OutOrderPosID;
    public Guid? OutOrderPosID 
    {
        get { return _OutOrderPosID; }
        set { SetProperty<Guid?>(ref _OutOrderPosID, value); }
    }

    Guid? _FacilityID;
    public Guid? FacilityID 
    {
        get { return _FacilityID; }
        set { SetProperty<Guid?>(ref _FacilityID, value); }
    }

    Guid? _FacilityPreBookingID;
    public Guid? FacilityPreBookingID 
    {
        get { return _FacilityPreBookingID; }
        set { SetProperty<Guid?>(ref _FacilityPreBookingID, value); }
    }

    private ACClass _ACClass;
    public virtual ACClass ACClass
    { 
        get => LazyLoader.Load(this, ref _ACClass);
        set => _ACClass = value;
    }

    public bool ACClass_IsLoaded
    {
        get
        {
            return ACClass != null;
        }
    }

    public virtual ReferenceEntry ACClassReference 
    {
        get { return Context.Entry(this).Reference("ACClass"); }
    }
    
    private DeliveryNote _DeliveryNote;
    public virtual DeliveryNote DeliveryNote
    { 
        get => LazyLoader.Load(this, ref _DeliveryNote);
        set => _DeliveryNote = value;
    }

    public bool DeliveryNote_IsLoaded
    {
        get
        {
            return DeliveryNote != null;
        }
    }

    public virtual ReferenceEntry DeliveryNoteReference 
    {
        get { return Context.Entry(this).Reference("DeliveryNote"); }
    }
    
    private DeliveryNotePos _DeliveryNotePos;
    public virtual DeliveryNotePos DeliveryNotePos
    { 
        get => LazyLoader.Load(this, ref _DeliveryNotePos);
        set => _DeliveryNotePos = value;
    }

    public bool DeliveryNotePos_IsLoaded
    {
        get
        {
            return DeliveryNotePos != null;
        }
    }

    public virtual ReferenceEntry DeliveryNotePosReference 
    {
        get { return Context.Entry(this).Reference("DeliveryNotePos"); }
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
    
    private FacilityBooking _FacilityBooking;
    public virtual FacilityBooking FacilityBooking
    { 
        get => LazyLoader.Load(this, ref _FacilityBooking);
        set => _FacilityBooking = value;
    }

    public bool FacilityBooking_IsLoaded
    {
        get
        {
            return FacilityBooking != null;
        }
    }

    public virtual ReferenceEntry FacilityBookingReference 
    {
        get { return Context.Entry(this).Reference("FacilityBooking"); }
    }
    
    private FacilityBookingCharge _FacilityBookingCharge;
    public virtual FacilityBookingCharge FacilityBookingCharge
    { 
        get => LazyLoader.Load(this, ref _FacilityBookingCharge);
        set => _FacilityBookingCharge = value;
    }

    public bool FacilityBookingCharge_IsLoaded
    {
        get
        {
            return FacilityBookingCharge != null;
        }
    }

    public virtual ReferenceEntry FacilityBookingChargeReference 
    {
        get { return Context.Entry(this).Reference("FacilityBookingCharge"); }
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
    
    private FacilityPreBooking _FacilityPreBooking;
    public virtual FacilityPreBooking FacilityPreBooking
    { 
        get => LazyLoader.Load(this, ref _FacilityPreBooking);
        set => _FacilityPreBooking = value;
    }

    public bool FacilityPreBooking_IsLoaded
    {
        get
        {
            return FacilityPreBooking != null;
        }
    }

    public virtual ReferenceEntry FacilityPreBookingReference 
    {
        get { return Context.Entry(this).Reference("FacilityPreBooking"); }
    }
    
    private InOrder _InOrder;
    public virtual InOrder InOrder
    { 
        get => LazyLoader.Load(this, ref _InOrder);
        set => _InOrder = value;
    }

    public bool InOrder_IsLoaded
    {
        get
        {
            return InOrder != null;
        }
    }

    public virtual ReferenceEntry InOrderReference 
    {
        get { return Context.Entry(this).Reference("InOrder"); }
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
    
    private TandTv2ItemType _TandTv2ItemType;
    public virtual TandTv2ItemType TandTv2ItemType
    { 
        get => LazyLoader.Load(this, ref _TandTv2ItemType);
        set => _TandTv2ItemType = value;
    }

    public bool TandTv2ItemType_IsLoaded
    {
        get
        {
            return TandTv2ItemType != null;
        }
    }

    public virtual ReferenceEntry TandTv2ItemTypeReference 
    {
        get { return Context.Entry(this).Reference("TandTv2ItemType"); }
    }
    
    private TandTv2Operation _TandTv2Operation;
    public virtual TandTv2Operation TandTv2Operation
    { 
        get => LazyLoader.Load(this, ref _TandTv2Operation);
        set => _TandTv2Operation = value;
    }

    public bool TandTv2Operation_IsLoaded
    {
        get
        {
            return TandTv2Operation != null;
        }
    }

    public virtual ReferenceEntry TandTv2OperationReference 
    {
        get { return Context.Entry(this).Reference("TandTv2Operation"); }
    }
    
    private TandTv2Step _TandTv2Step;
    public virtual TandTv2Step TandTv2Step
    { 
        get => LazyLoader.Load(this, ref _TandTv2Step);
        set => _TandTv2Step = value;
    }

    public bool TandTv2Step_IsLoaded
    {
        get
        {
            return TandTv2Step != null;
        }
    }

    public virtual ReferenceEntry TandTv2StepReference 
    {
        get { return Context.Entry(this).Reference("TandTv2Step"); }
    }
    
    private ICollection<TandTv2StepItemRelation> _TandTv2StepItemRelation_SourceTandTv2StepItem;
    public virtual ICollection<TandTv2StepItemRelation> TandTv2StepItemRelation_SourceTandTv2StepItem
    {
        get => LazyLoader.Load(this, ref _TandTv2StepItemRelation_SourceTandTv2StepItem);
        set => _TandTv2StepItemRelation_SourceTandTv2StepItem = value;
    }

    public bool TandTv2StepItemRelation_SourceTandTv2StepItem_IsLoaded
    {
        get
        {
            return TandTv2StepItemRelation_SourceTandTv2StepItem != null;
        }
    }

    public virtual CollectionEntry TandTv2StepItemRelation_SourceTandTv2StepItemReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv2StepItemRelation_SourceTandTv2StepItem); }
    }

    private ICollection<TandTv2StepItemRelation> _TandTv2StepItemRelation_TargetTandTv2StepItem;
    public virtual ICollection<TandTv2StepItemRelation> TandTv2StepItemRelation_TargetTandTv2StepItem
    {
        get => LazyLoader.Load(this, ref _TandTv2StepItemRelation_TargetTandTv2StepItem);
        set => _TandTv2StepItemRelation_TargetTandTv2StepItem = value;
    }

    public bool TandTv2StepItemRelation_TargetTandTv2StepItem_IsLoaded
    {
        get
        {
            return TandTv2StepItemRelation_TargetTandTv2StepItem != null;
        }
    }

    public virtual CollectionEntry TandTv2StepItemRelation_TargetTandTv2StepItemReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv2StepItemRelation_TargetTandTv2StepItem); }
    }
}
