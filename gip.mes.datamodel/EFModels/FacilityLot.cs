using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class FacilityLot : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public FacilityLot()
    {
    }

    private FacilityLot(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _FacilityLotID;
    public Guid FacilityLotID 
    {
        get { return _FacilityLotID; }
        set { SetProperty<Guid>(ref _FacilityLotID, value); }
    }

    string _LotNo;
    public string LotNo 
    {
        get { return _LotNo; }
        set { SetProperty<string>(ref _LotNo, value); }
    }

    Guid? _MaterialID;
    public Guid? MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid?>(ref _MaterialID, value); }
    }

    Guid? _MDReleaseStateID;
    public Guid? MDReleaseStateID 
    {
        get { return _MDReleaseStateID; }
        set { SetProperty<Guid?>(ref _MDReleaseStateID, value); }
    }

    DateTime? _FillingDate;
    public DateTime? FillingDate 
    {
        get { return _FillingDate; }
        set { SetProperty<DateTime?>(ref _FillingDate, value); }
    }

    short _StorageLife;
    public short StorageLife 
    {
        get { return _StorageLife; }
        set { SetProperty<short>(ref _StorageLife, value); }
    }

    DateTime? _ProductionDate;
    public DateTime? ProductionDate 
    {
        get { return _ProductionDate; }
        set { SetProperty<DateTime?>(ref _ProductionDate, value); }
    }

    DateTime? _ExpirationDate;
    public DateTime? ExpirationDate 
    {
        get { return _ExpirationDate; }
        set { SetProperty<DateTime?>(ref _ExpirationDate, value); }
    }

    string _ExternLotNo;
    public string ExternLotNo 
    {
        get { return _ExternLotNo; }
        set { SetProperty<string>(ref _ExternLotNo, value); }
    }

    string _ExternLotNo2;
    public string ExternLotNo2 
    {
        get { return _ExternLotNo2; }
        set { SetProperty<string>(ref _ExternLotNo2, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
    }

    bool _Lock;
    public bool Lock 
    {
        get { return _Lock; }
        set { SetProperty<bool>(ref _Lock, value); }
    }

    bool _IsEnabled;
    public bool IsEnabled 
    {
        get { return _IsEnabled; }
        set { SetProperty<bool>(ref _IsEnabled, value); }
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

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_InwardFacilityLot;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_InwardFacilityLot
    {
        get { return LazyLoader.Load(this, ref _FacilityBookingCharge_InwardFacilityLot); }
        set { _FacilityBookingCharge_InwardFacilityLot = value; }
    }

    public bool FacilityBookingCharge_InwardFacilityLot_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_InwardFacilityLot != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_InwardFacilityLotReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_InwardFacilityLot); }
    }

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_OutwardFacilityLot;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_OutwardFacilityLot
    {
        get { return LazyLoader.Load(this, ref _FacilityBookingCharge_OutwardFacilityLot); }
        set { _FacilityBookingCharge_OutwardFacilityLot = value; }
    }

    public bool FacilityBookingCharge_OutwardFacilityLot_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_OutwardFacilityLot != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_OutwardFacilityLotReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_OutwardFacilityLot); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_InwardFacilityLot;
    public virtual ICollection<FacilityBooking> FacilityBooking_InwardFacilityLot
    {
        get { return LazyLoader.Load(this, ref _FacilityBooking_InwardFacilityLot); }
        set { _FacilityBooking_InwardFacilityLot = value; }
    }

    public bool FacilityBooking_InwardFacilityLot_IsLoaded
    {
        get
        {
            return FacilityBooking_InwardFacilityLot != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_InwardFacilityLotReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_InwardFacilityLot); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_OutwardFacilityLot;
    public virtual ICollection<FacilityBooking> FacilityBooking_OutwardFacilityLot
    {
        get { return LazyLoader.Load(this, ref _FacilityBooking_OutwardFacilityLot); }
        set { _FacilityBooking_OutwardFacilityLot = value; }
    }

    public bool FacilityBooking_OutwardFacilityLot_IsLoaded
    {
        get
        {
            return FacilityBooking_OutwardFacilityLot != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_OutwardFacilityLotReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_OutwardFacilityLot); }
    }

    private ICollection<FacilityCharge> _FacilityCharge_FacilityLot;
    public virtual ICollection<FacilityCharge> FacilityCharge_FacilityLot
    {
        get { return LazyLoader.Load(this, ref _FacilityCharge_FacilityLot); }
        set { _FacilityCharge_FacilityLot = value; }
    }

    public bool FacilityCharge_FacilityLot_IsLoaded
    {
        get
        {
            return FacilityCharge_FacilityLot != null;
        }
    }

    public virtual CollectionEntry FacilityCharge_FacilityLotReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityCharge_FacilityLot); }
    }

    private ICollection<FacilityLotStock> _FacilityLotStock_FacilityLot;
    public virtual ICollection<FacilityLotStock> FacilityLotStock_FacilityLot
    {
        get { return LazyLoader.Load(this, ref _FacilityLotStock_FacilityLot); }
        set { _FacilityLotStock_FacilityLot = value; }
    }

    public bool FacilityLotStock_FacilityLot_IsLoaded
    {
        get
        {
            return FacilityLotStock_FacilityLot != null;
        }
    }

    public virtual CollectionEntry FacilityLotStock_FacilityLotReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityLotStock_FacilityLot); }
    }

    private ICollection<FacilityReservation> _FacilityReservation_FacilityLot;
    public virtual ICollection<FacilityReservation> FacilityReservation_FacilityLot
    {
        get { return LazyLoader.Load(this, ref _FacilityReservation_FacilityLot); }
        set { _FacilityReservation_FacilityLot = value; }
    }

    public bool FacilityReservation_FacilityLot_IsLoaded
    {
        get
        {
            return FacilityReservation_FacilityLot != null;
        }
    }

    public virtual CollectionEntry FacilityReservation_FacilityLotReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityReservation_FacilityLot); }
    }

    private ICollection<LabOrder> _LabOrder_FacilityLot;
    public virtual ICollection<LabOrder> LabOrder_FacilityLot
    {
        get { return LazyLoader.Load(this, ref _LabOrder_FacilityLot); }
        set { _LabOrder_FacilityLot = value; }
    }

    public bool LabOrder_FacilityLot_IsLoaded
    {
        get
        {
            return LabOrder_FacilityLot != null;
        }
    }

    public virtual CollectionEntry LabOrder_FacilityLotReference
    {
        get { return Context.Entry(this).Collection(c => c.LabOrder_FacilityLot); }
    }

    private MDReleaseState _MDReleaseState;
    public virtual MDReleaseState MDReleaseState
    { 
        get { return LazyLoader.Load(this, ref _MDReleaseState); } 
        set { SetProperty<MDReleaseState>(ref _MDReleaseState, value); }
    }

    public bool MDReleaseState_IsLoaded
    {
        get
        {
            return MDReleaseState != null;
        }
    }

    public virtual ReferenceEntry MDReleaseStateReference 
    {
        get { return Context.Entry(this).Reference("MDReleaseState"); }
    }
    
    private Material _Material;
    public virtual Material Material
    { 
        get { return LazyLoader.Load(this, ref _Material); } 
        set { SetProperty<Material>(ref _Material, value); }
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
    
    private ICollection<ProdOrderPartslistPos> _ProdOrderPartslistPos_FacilityLot;
    public virtual ICollection<ProdOrderPartslistPos> ProdOrderPartslistPos_FacilityLot
    {
        get { return LazyLoader.Load(this, ref _ProdOrderPartslistPos_FacilityLot); }
        set { _ProdOrderPartslistPos_FacilityLot = value; }
    }

    public bool ProdOrderPartslistPos_FacilityLot_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPos_FacilityLot != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistPos_FacilityLotReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistPos_FacilityLot); }
    }

    private ICollection<ProdOrderPartslistPosFacilityLot> _ProdOrderPartslistPosFacilityLot_FacilityLot;
    public virtual ICollection<ProdOrderPartslistPosFacilityLot> ProdOrderPartslistPosFacilityLot_FacilityLot
    {
        get { return LazyLoader.Load(this, ref _ProdOrderPartslistPosFacilityLot_FacilityLot); }
        set { _ProdOrderPartslistPosFacilityLot_FacilityLot = value; }
    }

    public bool ProdOrderPartslistPosFacilityLot_FacilityLot_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPosFacilityLot_FacilityLot != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistPosFacilityLot_FacilityLotReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistPosFacilityLot_FacilityLot); }
    }

    private ICollection<TandTv3MixPoint> _TandTv3MixPoint_InwardLot;
    public virtual ICollection<TandTv3MixPoint> TandTv3MixPoint_InwardLot
    {
        get { return LazyLoader.Load(this, ref _TandTv3MixPoint_InwardLot); }
        set { _TandTv3MixPoint_InwardLot = value; }
    }

    public bool TandTv3MixPoint_InwardLot_IsLoaded
    {
        get
        {
            return TandTv3MixPoint_InwardLot != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPoint_InwardLotReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPoint_InwardLot); }
    }

    private ICollection<TandTv3MixPointFacilityLot> _TandTv3MixPointFacilityLot_FacilityLot;
    public virtual ICollection<TandTv3MixPointFacilityLot> TandTv3MixPointFacilityLot_FacilityLot
    {
        get { return LazyLoader.Load(this, ref _TandTv3MixPointFacilityLot_FacilityLot); }
        set { _TandTv3MixPointFacilityLot_FacilityLot = value; }
    }

    public bool TandTv3MixPointFacilityLot_FacilityLot_IsLoaded
    {
        get
        {
            return TandTv3MixPointFacilityLot_FacilityLot != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointFacilityLot_FacilityLotReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointFacilityLot_FacilityLot); }
    }
}
