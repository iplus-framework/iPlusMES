using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDDelivPosState : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDDelivPosState()
    {
    }

    private MDDelivPosState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDDelivPosStateID;
    public Guid MDDelivPosStateID 
    {
        get { return _MDDelivPosStateID; }
        set { SetProperty<Guid>(ref _MDDelivPosStateID, value); }
    }

    short _MDDelivPosStateIndex;
    public short MDDelivPosStateIndex 
    {
        get { return _MDDelivPosStateIndex; }
        set { SetProperty<short>(ref _MDDelivPosStateIndex, value); }
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

    private ICollection<InOrderPos> _InOrderPos_MDDelivPosState;
    public virtual ICollection<InOrderPos> InOrderPos_MDDelivPosState
    {
        get => LazyLoader.Load(this, ref _InOrderPos_MDDelivPosState);
        set => _InOrderPos_MDDelivPosState = value;
    }

    public bool InOrderPos_MDDelivPosState_IsLoaded
    {
        get
        {
            return InOrderPos_MDDelivPosState != null;
        }
    }

    public virtual CollectionEntry InOrderPos_MDDelivPosStateReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderPos_MDDelivPosState); }
    }

    private ICollection<OutOrderPos> _OutOrderPos_MDDelivPosState;
    public virtual ICollection<OutOrderPos> OutOrderPos_MDDelivPosState
    {
        get => LazyLoader.Load(this, ref _OutOrderPos_MDDelivPosState);
        set => _OutOrderPos_MDDelivPosState = value;
    }

    public bool OutOrderPos_MDDelivPosState_IsLoaded
    {
        get
        {
            return OutOrderPos_MDDelivPosState != null;
        }
    }

    public virtual CollectionEntry OutOrderPos_MDDelivPosStateReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPos_MDDelivPosState); }
    }
}
