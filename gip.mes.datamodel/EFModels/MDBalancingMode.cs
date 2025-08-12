using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDBalancingMode : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDBalancingMode()
    {
    }

    private MDBalancingMode(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDBalancingModeID;
    public Guid MDBalancingModeID 
    {
        get { return _MDBalancingModeID; }
        set { SetProperty<Guid>(ref _MDBalancingModeID, value); }
    }

    short _MDBalancingModeIndex;
    public short MDBalancingModeIndex 
    {
        get { return _MDBalancingModeIndex; }
        set { SetProperty<short>(ref _MDBalancingModeIndex, value); }
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

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_MDBalancingMode;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_MDBalancingMode
    {
        get { return LazyLoader.Load(this, ref _FacilityBookingCharge_MDBalancingMode); }
        set { SetProperty<ICollection<FacilityBookingCharge>>(ref _FacilityBookingCharge_MDBalancingMode, value); }
    }

    public bool FacilityBookingCharge_MDBalancingMode_IsLoaded
    {
        get
        {
            return _FacilityBookingCharge_MDBalancingMode != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_MDBalancingModeReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_MDBalancingMode); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_MDBalancingMode;
    public virtual ICollection<FacilityBooking> FacilityBooking_MDBalancingMode
    {
        get { return LazyLoader.Load(this, ref _FacilityBooking_MDBalancingMode); }
        set { SetProperty<ICollection<FacilityBooking>>(ref _FacilityBooking_MDBalancingMode, value); }
    }

    public bool FacilityBooking_MDBalancingMode_IsLoaded
    {
        get
        {
            return _FacilityBooking_MDBalancingMode != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_MDBalancingModeReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_MDBalancingMode); }
    }
}
