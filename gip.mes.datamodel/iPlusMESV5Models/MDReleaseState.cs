using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDReleaseState : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDReleaseState()
    {
    }

    private MDReleaseState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDReleaseStateID;
    public Guid MDReleaseStateID 
    {
        get { return _MDReleaseStateID; }
        set { SetProperty<Guid>(ref _MDReleaseStateID, value); }
    }

    string _MDNameTrans;
    public string MDNameTrans 
    {
        get { return _MDNameTrans; }
        set { SetProperty<string>(ref _MDNameTrans, value); }
    }

    short _MDReleaseStateIndex;
    public short MDReleaseStateIndex 
    {
        get { return _MDReleaseStateIndex; }
        set { SetProperty<short>(ref _MDReleaseStateIndex, value); }
    }

    short _SortIndex;
    public short SortIndex 
    {
        get { return _SortIndex; }
        set { SetProperty<short>(ref _SortIndex, value); }
    }

    bool _IsDefault;
    public bool IsDefault 
    {
        get { return _IsDefault; }
        set { SetProperty<bool>(ref _IsDefault, value); }
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

    string _MDKey;
    public string MDKey 
    {
        get { return _MDKey; }
        set { SetProperty<string>(ref _MDKey, value); }
    }

    private ObservableHashSet<CompanyMaterialStock> _CompanyMaterialStock_MDReleaseState;
    public virtual ObservableHashSet<CompanyMaterialStock> CompanyMaterialStock_MDReleaseState
    {
        get => LazyLoader.Load(this, ref _CompanyMaterialStock_MDReleaseState);
        set => _CompanyMaterialStock_MDReleaseState = value;
    }

    public bool CompanyMaterialStock_MDReleaseState_IsLoaded
    {
        get
        {
            return CompanyMaterialStock_MDReleaseState != null;
        }
    }

    public virtual CollectionEntry CompanyMaterialStock_MDReleaseStateReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyMaterialStock_MDReleaseState); }
    }

    private ObservableHashSet<FacilityBookingCharge> _FacilityBookingCharge_MDReleaseState;
    public virtual ObservableHashSet<FacilityBookingCharge> FacilityBookingCharge_MDReleaseState
    {
        get => LazyLoader.Load(this, ref _FacilityBookingCharge_MDReleaseState);
        set => _FacilityBookingCharge_MDReleaseState = value;
    }

    public bool FacilityBookingCharge_MDReleaseState_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_MDReleaseState != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_MDReleaseStateReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_MDReleaseState); }
    }

    private ObservableHashSet<FacilityBooking> _FacilityBooking_MDReleaseState;
    public virtual ObservableHashSet<FacilityBooking> FacilityBooking_MDReleaseState
    {
        get => LazyLoader.Load(this, ref _FacilityBooking_MDReleaseState);
        set => _FacilityBooking_MDReleaseState = value;
    }

    public bool FacilityBooking_MDReleaseState_IsLoaded
    {
        get
        {
            return FacilityBooking_MDReleaseState != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_MDReleaseStateReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_MDReleaseState); }
    }

    private ObservableHashSet<FacilityCharge> _FacilityCharge_MDReleaseState;
    public virtual ObservableHashSet<FacilityCharge> FacilityCharge_MDReleaseState
    {
        get => LazyLoader.Load(this, ref _FacilityCharge_MDReleaseState);
        set => _FacilityCharge_MDReleaseState = value;
    }

    public bool FacilityCharge_MDReleaseState_IsLoaded
    {
        get
        {
            return FacilityCharge_MDReleaseState != null;
        }
    }

    public virtual CollectionEntry FacilityCharge_MDReleaseStateReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityCharge_MDReleaseState); }
    }

    private ObservableHashSet<FacilityLotStock> _FacilityLotStock_MDReleaseState;
    public virtual ObservableHashSet<FacilityLotStock> FacilityLotStock_MDReleaseState
    {
        get => LazyLoader.Load(this, ref _FacilityLotStock_MDReleaseState);
        set => _FacilityLotStock_MDReleaseState = value;
    }

    public bool FacilityLotStock_MDReleaseState_IsLoaded
    {
        get
        {
            return FacilityLotStock_MDReleaseState != null;
        }
    }

    public virtual CollectionEntry FacilityLotStock_MDReleaseStateReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityLotStock_MDReleaseState); }
    }

    private ObservableHashSet<FacilityLot> _FacilityLot_MDReleaseState;
    public virtual ObservableHashSet<FacilityLot> FacilityLot_MDReleaseState
    {
        get => LazyLoader.Load(this, ref _FacilityLot_MDReleaseState);
        set => _FacilityLot_MDReleaseState = value;
    }

    public bool FacilityLot_MDReleaseState_IsLoaded
    {
        get
        {
            return FacilityLot_MDReleaseState != null;
        }
    }

    public virtual CollectionEntry FacilityLot_MDReleaseStateReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityLot_MDReleaseState); }
    }

    private ObservableHashSet<FacilityStock> _FacilityStock_MDReleaseState;
    public virtual ObservableHashSet<FacilityStock> FacilityStock_MDReleaseState
    {
        get => LazyLoader.Load(this, ref _FacilityStock_MDReleaseState);
        set => _FacilityStock_MDReleaseState = value;
    }

    public bool FacilityStock_MDReleaseState_IsLoaded
    {
        get
        {
            return FacilityStock_MDReleaseState != null;
        }
    }

    public virtual CollectionEntry FacilityStock_MDReleaseStateReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityStock_MDReleaseState); }
    }

    private ObservableHashSet<MaterialStock> _MaterialStock_MDReleaseState;
    public virtual ObservableHashSet<MaterialStock> MaterialStock_MDReleaseState
    {
        get => LazyLoader.Load(this, ref _MaterialStock_MDReleaseState);
        set => _MaterialStock_MDReleaseState = value;
    }

    public bool MaterialStock_MDReleaseState_IsLoaded
    {
        get
        {
            return MaterialStock_MDReleaseState != null;
        }
    }

    public virtual CollectionEntry MaterialStock_MDReleaseStateReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialStock_MDReleaseState); }
    }

    private ObservableHashSet<PartslistStock> _PartslistStock_MDReleaseState;
    public virtual ObservableHashSet<PartslistStock> PartslistStock_MDReleaseState
    {
        get => LazyLoader.Load(this, ref _PartslistStock_MDReleaseState);
        set => _PartslistStock_MDReleaseState = value;
    }

    public bool PartslistStock_MDReleaseState_IsLoaded
    {
        get
        {
            return PartslistStock_MDReleaseState != null;
        }
    }

    public virtual CollectionEntry PartslistStock_MDReleaseStateReference
    {
        get { return Context.Entry(this).Collection(c => c.PartslistStock_MDReleaseState); }
    }
}
