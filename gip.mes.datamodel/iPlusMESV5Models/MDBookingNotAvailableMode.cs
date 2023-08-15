using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDBookingNotAvailableMode : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDBookingNotAvailableMode()
    {
    }

    private MDBookingNotAvailableMode(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDBookingNotAvailableModeID;
    public Guid MDBookingNotAvailableModeID 
    {
        get { return _MDBookingNotAvailableModeID; }
        set { SetProperty<Guid>(ref _MDBookingNotAvailableModeID, value); }
    }

    short _MDBookingNotAvailableModeIndex;
    public short MDBookingNotAvailableModeIndex 
    {
        get { return _MDBookingNotAvailableModeIndex; }
        set { SetProperty<short>(ref _MDBookingNotAvailableModeIndex, value); }
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

    private ObservableHashSet<FacilityBookingCharge> _FacilityBookingCharge_MDBookingNotAvailableMode;
    public virtual ObservableHashSet<FacilityBookingCharge> FacilityBookingCharge_MDBookingNotAvailableMode
    {
        get => LazyLoader.Load(this, ref _FacilityBookingCharge_MDBookingNotAvailableMode);
        set => _FacilityBookingCharge_MDBookingNotAvailableMode = value;
    }

    public bool FacilityBookingCharge_MDBookingNotAvailableMode_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_MDBookingNotAvailableMode != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_MDBookingNotAvailableModeReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_MDBookingNotAvailableMode); }
    }

    private ObservableHashSet<FacilityBooking> _FacilityBooking_MDBookingNotAvailableMode;
    public virtual ObservableHashSet<FacilityBooking> FacilityBooking_MDBookingNotAvailableMode
    {
        get => LazyLoader.Load(this, ref _FacilityBooking_MDBookingNotAvailableMode);
        set => _FacilityBooking_MDBookingNotAvailableMode = value;
    }

    public bool FacilityBooking_MDBookingNotAvailableMode_IsLoaded
    {
        get
        {
            return FacilityBooking_MDBookingNotAvailableMode != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_MDBookingNotAvailableModeReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_MDBookingNotAvailableMode); }
    }
}
