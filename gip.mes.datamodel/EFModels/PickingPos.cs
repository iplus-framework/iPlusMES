using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class PickingPos : VBEntityObject, IInsertInfo, IUpdateInfo, ISequence
{

    public PickingPos()
    {
    }

    private PickingPos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _PickingPosID;
    public Guid PickingPosID 
    {
        get { return _PickingPosID; }
        set { SetProperty<Guid>(ref _PickingPosID, value); }
    }

    Guid _PickingID;
    public Guid PickingID 
    {
        get { return _PickingID; }
        set { SetForeignKeyProperty<Guid>(ref _PickingID, value, "Picking", _Picking, Picking != null ? Picking.PickingID : default(Guid)); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    Guid? _OutOrderPosID;
    public Guid? OutOrderPosID 
    {
        get { return _OutOrderPosID; }
        set { SetForeignKeyProperty<Guid?>(ref _OutOrderPosID, value, "OutOrderPos", _OutOrderPos, OutOrderPos != null ? OutOrderPos.OutOrderPosID : default(Guid?)); }
    }

    Guid? _InOrderPosID;
    public Guid? InOrderPosID 
    {
        get { return _InOrderPosID; }
        set { SetForeignKeyProperty<Guid?>(ref _InOrderPosID, value, "InOrderPos", _InOrderPos, InOrderPos != null ? InOrderPos.InOrderPosID : default(Guid?)); }
    }

    Guid? _FromFacilityID;
    public Guid? FromFacilityID 
    {
        get { return _FromFacilityID; }
        set { SetForeignKeyProperty<Guid?>(ref _FromFacilityID, value, "FromFacility", _FromFacility, FromFacility != null ? FromFacility.FacilityID : default(Guid?)); }
    }

    Guid? _ToFacilityID;
    public Guid? ToFacilityID 
    {
        get { return _ToFacilityID; }
        set { SetForeignKeyProperty<Guid?>(ref _ToFacilityID, value, "ToFacility", _ToFacility, ToFacility != null ? ToFacility.FacilityID : default(Guid?)); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
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

    Guid? _PickingMaterialID;
    public Guid? PickingMaterialID 
    {
        get { return _PickingMaterialID; }
        set { SetForeignKeyProperty<Guid?>(ref _PickingMaterialID, value, "PickingMaterial", _PickingMaterial, PickingMaterial != null ? PickingMaterial.MaterialID : default(Guid?)); }
    }

    double? _PickingQuantityUOM;
    public double? PickingQuantityUOM 
    {
        get { return _PickingQuantityUOM; }
        set { SetProperty<double?>(ref _PickingQuantityUOM, value); }
    }

    Guid? _MDDelivPosLoadStateID;
    public Guid? MDDelivPosLoadStateID 
    {
        get { return _MDDelivPosLoadStateID; }
        set { SetForeignKeyProperty<Guid?>(ref _MDDelivPosLoadStateID, value, "MDDelivPosLoadState", _MDDelivPosLoadState, MDDelivPosLoadState != null ? MDDelivPosLoadState.MDDelivPosLoadStateID : default(Guid?)); }
    }

    double? _PickingActualUOM;
    public double? PickingActualUOM 
    {
        get { return _PickingActualUOM; }
        set { SetProperty<double?>(ref _PickingActualUOM, value); }
    }

    string _KeyOfExtSys;
    public string KeyOfExtSys 
    {
        get { return _KeyOfExtSys; }
        set { SetProperty<string>(ref _KeyOfExtSys, value); }
    }

    Guid? _ACClassTaskID;
    public Guid? ACClassTaskID 
    {
        get { return _ACClassTaskID; }
        set { SetForeignKeyProperty<Guid?>(ref _ACClassTaskID, value, "ACClassTask", _ACClassTask, ACClassTask != null ? ACClassTask.ACClassTaskID : default(Guid?)); }
    }

    Guid? _ACClassTaskID2;
    public Guid? ACClassTaskID2 
    {
        get { return _ACClassTaskID2; }
        set { SetProperty<Guid?>(ref _ACClassTaskID2, value); }
    }

    private ACClassTask _ACClassTask;
    public virtual ACClassTask ACClassTask
    { 
        get { return LazyLoader.Load(this, ref _ACClassTask); } 
        set { SetProperty<ACClassTask>(ref _ACClassTask, value); }
    }

    public bool ACClassTask_IsLoaded
    {
        get
        {
            return _ACClassTask != null;
        }
    }

    public virtual ReferenceEntry ACClassTaskReference 
    {
        get { return Context.Entry(this).Reference("ACClassTask"); }
    }
    
    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_PickingPos;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_PickingPos
    {
        get { return LazyLoader.Load(this, ref _FacilityBookingCharge_PickingPos); }
        set { SetProperty<ICollection<FacilityBookingCharge>>(ref _FacilityBookingCharge_PickingPos, value); }
    }

    public bool FacilityBookingCharge_PickingPos_IsLoaded
    {
        get
        {
            return _FacilityBookingCharge_PickingPos != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_PickingPosReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_PickingPos); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_PickingPos;
    public virtual ICollection<FacilityBooking> FacilityBooking_PickingPos
    {
        get { return LazyLoader.Load(this, ref _FacilityBooking_PickingPos); }
        set { SetProperty<ICollection<FacilityBooking>>(ref _FacilityBooking_PickingPos, value); }
    }

    public bool FacilityBooking_PickingPos_IsLoaded
    {
        get
        {
            return _FacilityBooking_PickingPos != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_PickingPosReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_PickingPos); }
    }

    private ICollection<FacilityPreBooking> _FacilityPreBooking_PickingPos;
    public virtual ICollection<FacilityPreBooking> FacilityPreBooking_PickingPos
    {
        get { return LazyLoader.Load(this, ref _FacilityPreBooking_PickingPos); }
        set { SetProperty<ICollection<FacilityPreBooking>>(ref _FacilityPreBooking_PickingPos, value); }
    }

    public bool FacilityPreBooking_PickingPos_IsLoaded
    {
        get
        {
            return _FacilityPreBooking_PickingPos != null;
        }
    }

    public virtual CollectionEntry FacilityPreBooking_PickingPosReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityPreBooking_PickingPos); }
    }

    private ICollection<FacilityReservation> _FacilityReservation_PickingPos;
    public virtual ICollection<FacilityReservation> FacilityReservation_PickingPos
    {
        get { return LazyLoader.Load(this, ref _FacilityReservation_PickingPos); }
        set { SetProperty<ICollection<FacilityReservation>>(ref _FacilityReservation_PickingPos, value); }
    }

    public bool FacilityReservation_PickingPos_IsLoaded
    {
        get
        {
            return _FacilityReservation_PickingPos != null;
        }
    }

    public virtual CollectionEntry FacilityReservation_PickingPosReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityReservation_PickingPos); }
    }

    private Facility _FromFacility;
    public virtual Facility FromFacility
    { 
        get { return LazyLoader.Load(this, ref _FromFacility); } 
        set { SetProperty<Facility>(ref _FromFacility, value); }
    }

    public bool FromFacility_IsLoaded
    {
        get
        {
            return _FromFacility != null;
        }
    }

    public virtual ReferenceEntry FromFacilityReference 
    {
        get { return Context.Entry(this).Reference("FromFacility"); }
    }
    
    private InOrderPos _InOrderPos;
    public virtual InOrderPos InOrderPos
    { 
        get { return LazyLoader.Load(this, ref _InOrderPos); } 
        set { SetProperty<InOrderPos>(ref _InOrderPos, value); }
    }

    public bool InOrderPos_IsLoaded
    {
        get
        {
            return _InOrderPos != null;
        }
    }

    public virtual ReferenceEntry InOrderPosReference 
    {
        get { return Context.Entry(this).Reference("InOrderPos"); }
    }
    
    private ICollection<LabOrder> _LabOrder_PickingPos;
    public virtual ICollection<LabOrder> LabOrder_PickingPos
    {
        get { return LazyLoader.Load(this, ref _LabOrder_PickingPos); }
        set { SetProperty<ICollection<LabOrder>>(ref _LabOrder_PickingPos, value); }
    }

    public bool LabOrder_PickingPos_IsLoaded
    {
        get
        {
            return _LabOrder_PickingPos != null;
        }
    }

    public virtual CollectionEntry LabOrder_PickingPosReference
    {
        get { return Context.Entry(this).Collection(c => c.LabOrder_PickingPos); }
    }

    private MDDelivPosLoadState _MDDelivPosLoadState;
    public virtual MDDelivPosLoadState MDDelivPosLoadState
    { 
        get { return LazyLoader.Load(this, ref _MDDelivPosLoadState); } 
        set { SetProperty<MDDelivPosLoadState>(ref _MDDelivPosLoadState, value); }
    }

    public bool MDDelivPosLoadState_IsLoaded
    {
        get
        {
            return _MDDelivPosLoadState != null;
        }
    }

    public virtual ReferenceEntry MDDelivPosLoadStateReference 
    {
        get { return Context.Entry(this).Reference("MDDelivPosLoadState"); }
    }
    
    private ICollection<OrderLog> _OrderLog_PickingPos;
    public virtual ICollection<OrderLog> OrderLog_PickingPos
    {
        get { return LazyLoader.Load(this, ref _OrderLog_PickingPos); }
        set { SetProperty<ICollection<OrderLog>>(ref _OrderLog_PickingPos, value); }
    }

    public bool OrderLog_PickingPos_IsLoaded
    {
        get
        {
            return _OrderLog_PickingPos != null;
        }
    }

    public virtual CollectionEntry OrderLog_PickingPosReference
    {
        get { return Context.Entry(this).Collection(c => c.OrderLog_PickingPos); }
    }

    private OutOrderPos _OutOrderPos;
    public virtual OutOrderPos OutOrderPos
    { 
        get { return LazyLoader.Load(this, ref _OutOrderPos); } 
        set { SetProperty<OutOrderPos>(ref _OutOrderPos, value); }
    }

    public bool OutOrderPos_IsLoaded
    {
        get
        {
            return _OutOrderPos != null;
        }
    }

    public virtual ReferenceEntry OutOrderPosReference 
    {
        get { return Context.Entry(this).Reference("OutOrderPos"); }
    }
    
    private Picking _Picking;
    public virtual Picking Picking
    { 
        get { return LazyLoader.Load(this, ref _Picking); } 
        set { SetProperty<Picking>(ref _Picking, value); }
    }

    public bool Picking_IsLoaded
    {
        get
        {
            return _Picking != null;
        }
    }

    public virtual ReferenceEntry PickingReference 
    {
        get { return Context.Entry(this).Reference("Picking"); }
    }
    
    private Material _PickingMaterial;
    public virtual Material PickingMaterial
    { 
        get { return LazyLoader.Load(this, ref _PickingMaterial); } 
        set { SetProperty<Material>(ref _PickingMaterial, value); }
    }

    public bool PickingMaterial_IsLoaded
    {
        get
        {
            return _PickingMaterial != null;
        }
    }

    public virtual ReferenceEntry PickingMaterialReference 
    {
        get { return Context.Entry(this).Reference("PickingMaterial"); }
    }
    
    private ICollection<PickingPosProdOrderPartslistPos> _PickingPosProdOrderPartslistPos_PickingPos;
    public virtual ICollection<PickingPosProdOrderPartslistPos> PickingPosProdOrderPartslistPos_PickingPos
    {
        get { return LazyLoader.Load(this, ref _PickingPosProdOrderPartslistPos_PickingPos); }
        set { SetProperty<ICollection<PickingPosProdOrderPartslistPos>>(ref _PickingPosProdOrderPartslistPos_PickingPos, value); }
    }

    public bool PickingPosProdOrderPartslistPos_PickingPos_IsLoaded
    {
        get
        {
            return _PickingPosProdOrderPartslistPos_PickingPos != null;
        }
    }

    public virtual CollectionEntry PickingPosProdOrderPartslistPos_PickingPosReference
    {
        get { return Context.Entry(this).Collection(c => c.PickingPosProdOrderPartslistPos_PickingPos); }
    }

    private ICollection<TandTv3MixPointPickingPos> _TandTv3MixPointPickingPos_PickingPos;
    public virtual ICollection<TandTv3MixPointPickingPos> TandTv3MixPointPickingPos_PickingPos
    {
        get { return LazyLoader.Load(this, ref _TandTv3MixPointPickingPos_PickingPos); }
        set { SetProperty<ICollection<TandTv3MixPointPickingPos>>(ref _TandTv3MixPointPickingPos_PickingPos, value); }
    }

    public bool TandTv3MixPointPickingPos_PickingPos_IsLoaded
    {
        get
        {
            return _TandTv3MixPointPickingPos_PickingPos != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointPickingPos_PickingPosReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointPickingPos_PickingPos); }
    }

    private Facility _ToFacility;
    public virtual Facility ToFacility
    { 
        get { return LazyLoader.Load(this, ref _ToFacility); } 
        set { SetProperty<Facility>(ref _ToFacility, value); }
    }

    public bool ToFacility_IsLoaded
    {
        get
        {
            return _ToFacility != null;
        }
    }

    public virtual ReferenceEntry ToFacilityReference 
    {
        get { return Context.Entry(this).Reference("ToFacility"); }
    }
    
    private ICollection<Weighing> _Weighing_PickingPos;
    public virtual ICollection<Weighing> Weighing_PickingPos
    {
        get { return LazyLoader.Load(this, ref _Weighing_PickingPos); }
        set { SetProperty<ICollection<Weighing>>(ref _Weighing_PickingPos, value); }
    }

    public bool Weighing_PickingPos_IsLoaded
    {
        get
        {
            return _Weighing_PickingPos != null;
        }
    }

    public virtual CollectionEntry Weighing_PickingPosReference
    {
        get { return Context.Entry(this).Collection(c => c.Weighing_PickingPos); }
    }
}
