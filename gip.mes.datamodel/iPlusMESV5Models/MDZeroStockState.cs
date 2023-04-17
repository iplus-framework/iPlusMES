using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDZeroStockState : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public MDZeroStockState()
    {
    }

    private MDZeroStockState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _MDZeroStockStateID;
    public Guid MDZeroStockStateID 
    {
        get { return _MDZeroStockStateID; }
        set { SetProperty<Guid>(ref _MDZeroStockStateID, value); }
    }

    short _MDZeroStockStateIndex;
    public short MDZeroStockStateIndex 
    {
        get { return _MDZeroStockStateIndex; }
        set { SetProperty<short>(ref _MDZeroStockStateIndex, value); }
    }

    string _MDNameTrans;
    public string MDNameTrans 
    {
        get { return _MDNameTrans; }
        set { SetProperty<string>(ref _MDNameTrans, value); }
    }

    short _SortIndex;
    public short SortIndex 
    {
        get { return _SortIndex; }
        set { SetProperty<short>(ref _SortIndex, value); }
    }

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
    }

    bool _IsDefault;
    public bool IsDefault 
    {
        get { return _IsDefault; }
        set { SetProperty<bool>(ref _IsDefault, value); }
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

    string _MDKey;
    public string MDKey 
    {
        get { return _MDKey; }
        set { SetProperty<string>(ref _MDKey, value); }
    }

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_MDZeroStockState;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_MDZeroStockState
    {
        get => LazyLoader.Load(this, ref _FacilityBookingCharge_MDZeroStockState);
        set => _FacilityBookingCharge_MDZeroStockState = value;
    }

    public bool FacilityBookingCharge_MDZeroStockState_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_MDZeroStockState != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_MDZeroStockStateReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_MDZeroStockState); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_MDZeroStockState;
    public virtual ICollection<FacilityBooking> FacilityBooking_MDZeroStockState
    {
        get => LazyLoader.Load(this, ref _FacilityBooking_MDZeroStockState);
        set => _FacilityBooking_MDZeroStockState = value;
    }

    public bool FacilityBooking_MDZeroStockState_IsLoaded
    {
        get
        {
            return FacilityBooking_MDZeroStockState != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_MDZeroStockStateReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_MDZeroStockState); }
    }
}
