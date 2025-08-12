using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDInOrderState : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDInOrderState()
    {
    }

    private MDInOrderState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDInOrderStateID;
    public Guid MDInOrderStateID 
    {
        get { return _MDInOrderStateID; }
        set { SetProperty<Guid>(ref _MDInOrderStateID, value); }
    }

    short _MDInOrderStateIndex;
    public short MDInOrderStateIndex 
    {
        get { return _MDInOrderStateIndex; }
        set { SetProperty<short>(ref _MDInOrderStateIndex, value); }
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

    private ICollection<InOrder> _InOrder_MDInOrderState;
    public virtual ICollection<InOrder> InOrder_MDInOrderState
    {
        get { return LazyLoader.Load(this, ref _InOrder_MDInOrderState); }
        set { SetProperty<ICollection<InOrder>>(ref _InOrder_MDInOrderState, value); }
    }

    public bool InOrder_MDInOrderState_IsLoaded
    {
        get
        {
            return _InOrder_MDInOrderState != null;
        }
    }

    public virtual CollectionEntry InOrder_MDInOrderStateReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrder_MDInOrderState); }
    }
}
