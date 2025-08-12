using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class OrderLog : VBEntityObject
{

    public OrderLog()
    {
    }

    private OrderLog(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _VBiACProgramLogID;
    public Guid VBiACProgramLogID 
    {
        get { return _VBiACProgramLogID; }
        set { SetForeignKeyProperty<Guid>(ref _VBiACProgramLogID, value, "VBiACProgramLog", _VBiACProgramLog, VBiACProgramLog != null ? VBiACProgramLog.ACProgramLogID : default(Guid)); }
    }

    Guid? _ProdOrderPartslistPosID;
    public Guid? ProdOrderPartslistPosID 
    {
        get { return _ProdOrderPartslistPosID; }
        set { SetForeignKeyProperty<Guid?>(ref _ProdOrderPartslistPosID, value, "ProdOrderPartslistPos", _ProdOrderPartslistPos, ProdOrderPartslistPos != null ? ProdOrderPartslistPos.ProdOrderPartslistPosID : default(Guid?)); }
    }

    Guid? _ProdOrderPartslistPosRelationID;
    public Guid? ProdOrderPartslistPosRelationID 
    {
        get { return _ProdOrderPartslistPosRelationID; }
        set { SetForeignKeyProperty<Guid?>(ref _ProdOrderPartslistPosRelationID, value, "ProdOrderPartslistPosRelation", _ProdOrderPartslistPosRelation, ProdOrderPartslistPosRelation != null ? ProdOrderPartslistPosRelation.ProdOrderPartslistPosRelationID : default(Guid?)); }
    }

    Guid? _DeliveryNotePosID;
    public Guid? DeliveryNotePosID 
    {
        get { return _DeliveryNotePosID; }
        set { SetForeignKeyProperty<Guid?>(ref _DeliveryNotePosID, value, "DeliveryNotePos", _DeliveryNotePos, DeliveryNotePos != null ? DeliveryNotePos.DeliveryNotePosID : default(Guid?)); }
    }

    Guid? _PickingPosID;
    public Guid? PickingPosID 
    {
        get { return _PickingPosID; }
        set { SetForeignKeyProperty<Guid?>(ref _PickingPosID, value, "PickingPos", _PickingPos, PickingPos != null ? PickingPos.PickingPosID : default(Guid?)); }
    }

    Guid? _FacilityBookingID;
    public Guid? FacilityBookingID 
    {
        get { return _FacilityBookingID; }
        set { SetForeignKeyProperty<Guid?>(ref _FacilityBookingID, value, "FacilityBooking", _FacilityBooking, FacilityBooking != null ? FacilityBooking.FacilityBookingID : default(Guid?)); }
    }

    private DeliveryNotePos _DeliveryNotePos;
    public virtual DeliveryNotePos DeliveryNotePos
    { 
        get { return LazyLoader.Load(this, ref _DeliveryNotePos); } 
        set { SetProperty<DeliveryNotePos>(ref _DeliveryNotePos, value); }
    }

    public bool DeliveryNotePos_IsLoaded
    {
        get
        {
            return _DeliveryNotePos != null;
        }
    }

    public virtual ReferenceEntry DeliveryNotePosReference 
    {
        get { return Context.Entry(this).Reference("DeliveryNotePos"); }
    }
    
    private FacilityBooking _FacilityBooking;
    public virtual FacilityBooking FacilityBooking
    { 
        get { return LazyLoader.Load(this, ref _FacilityBooking); } 
        set { SetProperty<FacilityBooking>(ref _FacilityBooking, value); }
    }

    public bool FacilityBooking_IsLoaded
    {
        get
        {
            return _FacilityBooking != null;
        }
    }

    public virtual ReferenceEntry FacilityBookingReference 
    {
        get { return Context.Entry(this).Reference("FacilityBooking"); }
    }
    
    private PickingPos _PickingPos;
    public virtual PickingPos PickingPos
    { 
        get { return LazyLoader.Load(this, ref _PickingPos); } 
        set { SetProperty<PickingPos>(ref _PickingPos, value); }
    }

    public bool PickingPos_IsLoaded
    {
        get
        {
            return _PickingPos != null;
        }
    }

    public virtual ReferenceEntry PickingPosReference 
    {
        get { return Context.Entry(this).Reference("PickingPos"); }
    }
    
    private ProdOrderPartslistPos _ProdOrderPartslistPos;
    public virtual ProdOrderPartslistPos ProdOrderPartslistPos
    { 
        get { return LazyLoader.Load(this, ref _ProdOrderPartslistPos); } 
        set { SetProperty<ProdOrderPartslistPos>(ref _ProdOrderPartslistPos, value); }
    }

    public bool ProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return _ProdOrderPartslistPos != null;
        }
    }

    public virtual ReferenceEntry ProdOrderPartslistPosReference 
    {
        get { return Context.Entry(this).Reference("ProdOrderPartslistPos"); }
    }
    
    private ProdOrderPartslistPosRelation _ProdOrderPartslistPosRelation;
    public virtual ProdOrderPartslistPosRelation ProdOrderPartslistPosRelation
    { 
        get { return LazyLoader.Load(this, ref _ProdOrderPartslistPosRelation); } 
        set { SetProperty<ProdOrderPartslistPosRelation>(ref _ProdOrderPartslistPosRelation, value); }
    }

    public bool ProdOrderPartslistPosRelation_IsLoaded
    {
        get
        {
            return _ProdOrderPartslistPosRelation != null;
        }
    }

    public virtual ReferenceEntry ProdOrderPartslistPosRelationReference 
    {
        get { return Context.Entry(this).Reference("ProdOrderPartslistPosRelation"); }
    }
    
    private ACProgramLog _VBiACProgramLog;
    public virtual ACProgramLog VBiACProgramLog
    { 
        get { return LazyLoader.Load(this, ref _VBiACProgramLog); } 
        set { SetProperty<ACProgramLog>(ref _VBiACProgramLog, value); }
    }

    public bool VBiACProgramLog_IsLoaded
    {
        get
        {
            return _VBiACProgramLog != null;
        }
    }

    public virtual ReferenceEntry VBiACProgramLogReference 
    {
        get { return Context.Entry(this).Reference("VBiACProgramLog"); }
    }
    }
