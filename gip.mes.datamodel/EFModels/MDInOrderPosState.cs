using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDInOrderPosState : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDInOrderPosState()
    {
    }

    private MDInOrderPosState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDInOrderPosStateID;
    public Guid MDInOrderPosStateID 
    {
        get { return _MDInOrderPosStateID; }
        set { SetProperty<Guid>(ref _MDInOrderPosStateID, value); }
    }

    short _MDInOrderPosStateIndex;
    public short MDInOrderPosStateIndex 
    {
        get { return _MDInOrderPosStateIndex; }
        set { SetProperty<short>(ref _MDInOrderPosStateIndex, value); }
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

    private ICollection<InOrderPos> _InOrderPos_MDInOrderPosState;
    public virtual ICollection<InOrderPos> InOrderPos_MDInOrderPosState
    {
        get { return LazyLoader.Load(this, ref _InOrderPos_MDInOrderPosState); }
        set { _InOrderPos_MDInOrderPosState = value; }
    }

    public bool InOrderPos_MDInOrderPosState_IsLoaded
    {
        get
        {
            return _InOrderPos_MDInOrderPosState != null;
        }
    }

    public virtual CollectionEntry InOrderPos_MDInOrderPosStateReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderPos_MDInOrderPosState); }
    }
}
