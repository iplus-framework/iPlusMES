using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDFacilityInventoryState : VBEntityObject , IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDFacilityInventoryState()
    {
    }

    private MDFacilityInventoryState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _MDFacilityInventoryStateID;
    public Guid MDFacilityInventoryStateID 
    {
        get { return _MDFacilityInventoryStateID; }
        set { SetProperty<Guid>(ref _MDFacilityInventoryStateID, value); }
    }

    short _MDFacilityInventoryStateIndex;
    public short MDFacilityInventoryStateIndex 
    {
        get { return _MDFacilityInventoryStateIndex; }
        set { SetProperty<short>(ref _MDFacilityInventoryStateIndex, value); }
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

    private ICollection<FacilityInventory> _FacilityInventory_MDFacilityInventoryState;
    public virtual ICollection<FacilityInventory> FacilityInventory_MDFacilityInventoryState
    {
        get => LazyLoader.Load(this, ref _FacilityInventory_MDFacilityInventoryState);
        set => _FacilityInventory_MDFacilityInventoryState = value;
    }

    public bool FacilityInventory_MDFacilityInventoryState_IsLoaded
    {
        get
        {
            return FacilityInventory_MDFacilityInventoryState != null;
        }
    }

    public virtual CollectionEntry FacilityInventory_MDFacilityInventoryStateReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityInventory_MDFacilityInventoryState); }
    }
}
