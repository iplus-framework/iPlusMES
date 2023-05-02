using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class FacilityPreBooking : VBEntityObject
{

    public FacilityPreBooking()
    {
    }

    private FacilityPreBooking(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _FacilityPreBookingID;
    public Guid FacilityPreBookingID 
    {
        get { return _FacilityPreBookingID; }
        set { SetProperty<Guid>(ref _FacilityPreBookingID, value); }
    }

    string _FacilityPreBookingNo;
    public string FacilityPreBookingNo 
    {
        get { return _FacilityPreBookingNo; }
        set { SetProperty<string>(ref _FacilityPreBookingNo, value); }
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

    string _ACMethodBookingXML;
    public string ACMethodBookingXML 
    {
        get { return _ACMethodBookingXML; }
        set { SetProperty<string>(ref _ACMethodBookingXML, value); }
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

    Guid? _PickingPosID;
    public Guid? PickingPosID 
    {
        get { return _PickingPosID; }
        set { SetProperty<Guid?>(ref _PickingPosID, value); }
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
    
    private PickingPos _PickingPos;
    public virtual PickingPos PickingPos
    { 
        get => LazyLoader.Load(this, ref _PickingPos);
        set => _PickingPos = value;
    }

    public bool PickingPos_IsLoaded
    {
        get
        {
            return PickingPos != null;
        }
    }

    public virtual ReferenceEntry PickingPosReference 
    {
        get { return Context.Entry(this).Reference("PickingPos"); }
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
    

    private ICollection<TandTv3MixPointFacilityPreBooking> _TandTv3MixPointFacilityPreBooking_FacilityPreBooking;
    public virtual ICollection<TandTv3MixPointFacilityPreBooking> TandTv3MixPointFacilityPreBooking_FacilityPreBooking
    {
        get => LazyLoader.Load(this, ref _TandTv3MixPointFacilityPreBooking_FacilityPreBooking);
        set => _TandTv3MixPointFacilityPreBooking_FacilityPreBooking = value;
    }

    public bool TandTv3MixPointFacilityPreBooking_FacilityPreBooking_IsLoaded
    {
        get
        {
            return TandTv3MixPointFacilityPreBooking_FacilityPreBooking != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointFacilityPreBooking_FacilityPreBookingReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointFacilityPreBooking_FacilityPreBooking); }
    }
}
