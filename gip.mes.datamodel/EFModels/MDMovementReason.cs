using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDMovementReason : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDMovementReason()
    {
    }

    private MDMovementReason(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDMovementReasonID;
    public Guid MDMovementReasonID 
    {
        get { return _MDMovementReasonID; }
        set { SetProperty<Guid>(ref _MDMovementReasonID, value); }
    }

    short _MDMovementReasonIndex;
    public short MDMovementReasonIndex 
    {
        get { return _MDMovementReasonIndex; }
        set { SetProperty<short>(ref _MDMovementReasonIndex, value); }
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

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_MDMovementReason;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_MDMovementReason
    {
        get { return LazyLoader.Load(this, ref _FacilityBookingCharge_MDMovementReason); }
        set { _FacilityBookingCharge_MDMovementReason = value; }
    }

    public bool FacilityBookingCharge_MDMovementReason_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_MDMovementReason != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_MDMovementReasonReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_MDMovementReason); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_MDMovementReason;
    public virtual ICollection<FacilityBooking> FacilityBooking_MDMovementReason
    {
        get { return LazyLoader.Load(this, ref _FacilityBooking_MDMovementReason); }
        set { _FacilityBooking_MDMovementReason = value; }
    }

    public bool FacilityBooking_MDMovementReason_IsLoaded
    {
        get
        {
            return FacilityBooking_MDMovementReason != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_MDMovementReasonReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_MDMovementReason); }
    }
}
