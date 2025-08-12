using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class FacilityInventoryPos : VBEntityObject, IInsertInfo, IUpdateInfo, ISequence
{

    public FacilityInventoryPos()
    {
    }

    private FacilityInventoryPos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _FacilityInventoryPosID;
    public Guid FacilityInventoryPosID 
    {
        get { return _FacilityInventoryPosID; }
        set { SetProperty<Guid>(ref _FacilityInventoryPosID, value); }
    }

    Guid _FacilityInventoryID;
    public Guid FacilityInventoryID 
    {
        get { return _FacilityInventoryID; }
        set { SetForeignKeyProperty<Guid>(ref _FacilityInventoryID, value, "FacilityInventory", _FacilityInventory, FacilityInventory != null ? FacilityInventory.FacilityInventoryID : default(Guid)); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    Guid _FacilityChargeID;
    public Guid FacilityChargeID 
    {
        get { return _FacilityChargeID; }
        set { SetForeignKeyProperty<Guid>(ref _FacilityChargeID, value, "FacilityCharge", _FacilityCharge, FacilityCharge != null ? FacilityCharge.FacilityChargeID : default(Guid)); }
    }

    double _StockQuantity;
    public double StockQuantity 
    {
        get { return _StockQuantity; }
        set { SetProperty<double>(ref _StockQuantity, value); }
    }

    double? _NewStockQuantity;
    public double? NewStockQuantity 
    {
        get { return _NewStockQuantity; }
        set { SetProperty<double?>(ref _NewStockQuantity, value); }
    }

    bool _NotAvailable;
    public bool NotAvailable 
    {
        get { return _NotAvailable; }
        set { SetProperty<bool>(ref _NotAvailable, value); }
    }

    Guid _MDFacilityInventoryPosStateID;
    public Guid MDFacilityInventoryPosStateID 
    {
        get { return _MDFacilityInventoryPosStateID; }
        set { SetForeignKeyProperty<Guid>(ref _MDFacilityInventoryPosStateID, value, "MDFacilityInventoryPosState", _MDFacilityInventoryPosState, MDFacilityInventoryPosState != null ? MDFacilityInventoryPosState.MDFacilityInventoryPosStateID : default(Guid)); }
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

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_FacilityInventoryPos;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_FacilityInventoryPos
    {
        get { return LazyLoader.Load(this, ref _FacilityBookingCharge_FacilityInventoryPos); }
        set { SetProperty<ICollection<FacilityBookingCharge>>(ref _FacilityBookingCharge_FacilityInventoryPos, value); }
    }

    public bool FacilityBookingCharge_FacilityInventoryPos_IsLoaded
    {
        get
        {
            return _FacilityBookingCharge_FacilityInventoryPos != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_FacilityInventoryPosReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_FacilityInventoryPos); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_FacilityInventoryPos;
    public virtual ICollection<FacilityBooking> FacilityBooking_FacilityInventoryPos
    {
        get { return LazyLoader.Load(this, ref _FacilityBooking_FacilityInventoryPos); }
        set { SetProperty<ICollection<FacilityBooking>>(ref _FacilityBooking_FacilityInventoryPos, value); }
    }

    public bool FacilityBooking_FacilityInventoryPos_IsLoaded
    {
        get
        {
            return _FacilityBooking_FacilityInventoryPos != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_FacilityInventoryPosReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_FacilityInventoryPos); }
    }

    private FacilityCharge _FacilityCharge;
    public virtual FacilityCharge FacilityCharge
    { 
        get { return LazyLoader.Load(this, ref _FacilityCharge); } 
        set { SetProperty<FacilityCharge>(ref _FacilityCharge, value); }
    }

    public bool FacilityCharge_IsLoaded
    {
        get
        {
            return _FacilityCharge != null;
        }
    }

    public virtual ReferenceEntry FacilityChargeReference 
    {
        get { return Context.Entry(this).Reference("FacilityCharge"); }
    }
    
    private FacilityInventory _FacilityInventory;
    public virtual FacilityInventory FacilityInventory
    { 
        get { return LazyLoader.Load(this, ref _FacilityInventory); } 
        set { SetProperty<FacilityInventory>(ref _FacilityInventory, value); }
    }

    public bool FacilityInventory_IsLoaded
    {
        get
        {
            return _FacilityInventory != null;
        }
    }

    public virtual ReferenceEntry FacilityInventoryReference 
    {
        get { return Context.Entry(this).Reference("FacilityInventory"); }
    }
    
    private MDFacilityInventoryPosState _MDFacilityInventoryPosState;
    public virtual MDFacilityInventoryPosState MDFacilityInventoryPosState
    { 
        get { return LazyLoader.Load(this, ref _MDFacilityInventoryPosState); } 
        set { SetProperty<MDFacilityInventoryPosState>(ref _MDFacilityInventoryPosState, value); }
    }

    public bool MDFacilityInventoryPosState_IsLoaded
    {
        get
        {
            return _MDFacilityInventoryPosState != null;
        }
    }

    public virtual ReferenceEntry MDFacilityInventoryPosStateReference 
    {
        get { return Context.Entry(this).Reference("MDFacilityInventoryPosState"); }
    }
    }
