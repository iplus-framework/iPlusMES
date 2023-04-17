using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDDelivPosLoadState : VBEntityObject , IInsertInfo, IUpdateInfo
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

    private ICollection<InOrderPos> _InOrderPo_MDDelivPosLoadState;
    public virtual ICollection<InOrderPos> InOrderPo_MDDelivPosLoadState
    {
        get => LazyLoader.Load(this, ref _InOrderPo_MDDelivPosLoadState);
        set => _InOrderPo_MDDelivPosLoadState = value;
    }

    public bool InOrderPo_MDDelivPosLoadState_IsLoaded
    {
        get
        {
            return InOrderPo_MDDelivPosLoadState != null;
        }
    }

    public virtual CollectionEntry InOrderPo_MDDelivPosLoadStateReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderPo_MDDelivPosLoadState); }
    }

    private ICollection<OutOrderPos> _OutOrderPo_MDDelivPosLoadState;
    public virtual ICollection<OutOrderPos> OutOrderPo_MDDelivPosLoadState
    {
        get => LazyLoader.Load(this, ref _OutOrderPo_MDDelivPosLoadState);
        set => _OutOrderPo_MDDelivPosLoadState = value;
    }

    public bool OutOrderPo_MDDelivPosLoadState_IsLoaded
    {
        get
        {
            return OutOrderPo_MDDelivPosLoadState != null;
        }
    }

    public virtual CollectionEntry OutOrderPo_MDDelivPosLoadStateReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPo_MDDelivPosLoadState); }
    }

    private ICollection<PickingPos> _PickingPo_MDDelivPosLoadState;
    public virtual ICollection<PickingPos> PickingPo_MDDelivPosLoadState
    {
        get => LazyLoader.Load(this, ref _PickingPo_MDDelivPosLoadState);
        set => _PickingPo_MDDelivPosLoadState = value;
    }

    public bool PickingPo_MDDelivPosLoadState_IsLoaded
    {
        get
        {
            return PickingPo_MDDelivPosLoadState != null;
        }
    }

    public virtual CollectionEntry PickingPo_MDDelivPosLoadStateReference
    {
        get { return Context.Entry(this).Collection(c => c.PickingPo_MDDelivPosLoadState); }
    }
}
