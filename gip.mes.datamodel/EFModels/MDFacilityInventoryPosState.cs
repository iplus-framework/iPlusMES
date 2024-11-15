using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDFacilityInventoryPosState : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDFacilityInventoryPosState()
    {
    }

    private MDFacilityInventoryPosState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDFacilityInventoryPosStateID;
    public Guid MDFacilityInventoryPosStateID 
    {
        get { return _MDFacilityInventoryPosStateID; }
        set { SetProperty<Guid>(ref _MDFacilityInventoryPosStateID, value); }
    }

    short _MDFacilityInventoryPosStateIndex;
    public short MDFacilityInventoryPosStateIndex 
    {
        get { return _MDFacilityInventoryPosStateIndex; }
        set { SetProperty<short>(ref _MDFacilityInventoryPosStateIndex, value); }
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

    private ICollection<FacilityInventoryPos> _FacilityInventoryPos_MDFacilityInventoryPosState;
    public virtual ICollection<FacilityInventoryPos> FacilityInventoryPos_MDFacilityInventoryPosState
    {
        get { return LazyLoader.Load(this, ref _FacilityInventoryPos_MDFacilityInventoryPosState); }
        set { _FacilityInventoryPos_MDFacilityInventoryPosState = value; }
    }

    public bool FacilityInventoryPos_MDFacilityInventoryPosState_IsLoaded
    {
        get
        {
            return _FacilityInventoryPos_MDFacilityInventoryPosState != null;
        }
    }

    public virtual CollectionEntry FacilityInventoryPos_MDFacilityInventoryPosStateReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityInventoryPos_MDFacilityInventoryPosState); }
    }
}
