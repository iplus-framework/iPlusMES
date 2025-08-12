using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDDelivPosLoadState : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDDelivPosLoadState()
    {
    }

    private MDDelivPosLoadState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDDelivPosLoadStateID;
    public Guid MDDelivPosLoadStateID 
    {
        get { return _MDDelivPosLoadStateID; }
        set { SetProperty<Guid>(ref _MDDelivPosLoadStateID, value); }
    }

    short _MDDelivPosLoadStateIndex;
    public short MDDelivPosLoadStateIndex 
    {
        get { return _MDDelivPosLoadStateIndex; }
        set { SetProperty<short>(ref _MDDelivPosLoadStateIndex, value); }
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

    private ICollection<InOrderPos> _InOrderPos_MDDelivPosLoadState;
    public virtual ICollection<InOrderPos> InOrderPos_MDDelivPosLoadState
    {
        get { return LazyLoader.Load(this, ref _InOrderPos_MDDelivPosLoadState); }
        set { SetProperty<ICollection<InOrderPos>>(ref _InOrderPos_MDDelivPosLoadState, value); }
    }

    public bool InOrderPos_MDDelivPosLoadState_IsLoaded
    {
        get
        {
            return _InOrderPos_MDDelivPosLoadState != null;
        }
    }

    public virtual CollectionEntry InOrderPos_MDDelivPosLoadStateReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderPos_MDDelivPosLoadState); }
    }

    private ICollection<OutOrderPos> _OutOrderPos_MDDelivPosLoadState;
    public virtual ICollection<OutOrderPos> OutOrderPos_MDDelivPosLoadState
    {
        get { return LazyLoader.Load(this, ref _OutOrderPos_MDDelivPosLoadState); }
        set { SetProperty<ICollection<OutOrderPos>>(ref _OutOrderPos_MDDelivPosLoadState, value); }
    }

    public bool OutOrderPos_MDDelivPosLoadState_IsLoaded
    {
        get
        {
            return _OutOrderPos_MDDelivPosLoadState != null;
        }
    }

    public virtual CollectionEntry OutOrderPos_MDDelivPosLoadStateReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPos_MDDelivPosLoadState); }
    }

    private ICollection<PickingPos> _PickingPos_MDDelivPosLoadState;
    public virtual ICollection<PickingPos> PickingPos_MDDelivPosLoadState
    {
        get { return LazyLoader.Load(this, ref _PickingPos_MDDelivPosLoadState); }
        set { SetProperty<ICollection<PickingPos>>(ref _PickingPos_MDDelivPosLoadState, value); }
    }

    public bool PickingPos_MDDelivPosLoadState_IsLoaded
    {
        get
        {
            return _PickingPos_MDDelivPosLoadState != null;
        }
    }

    public virtual CollectionEntry PickingPos_MDDelivPosLoadStateReference
    {
        get { return Context.Entry(this).Collection(c => c.PickingPos_MDDelivPosLoadState); }
    }
}
