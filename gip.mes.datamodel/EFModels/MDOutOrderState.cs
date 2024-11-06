using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDOutOrderState : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDOutOrderState()
    {
    }

    private MDOutOrderState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDOutOrderStateID;
    public Guid MDOutOrderStateID 
    {
        get { return _MDOutOrderStateID; }
        set { SetProperty<Guid>(ref _MDOutOrderStateID, value); }
    }

    short _MDOutOrderStateIndex;
    public short MDOutOrderStateIndex 
    {
        get { return _MDOutOrderStateIndex; }
        set { SetProperty<short>(ref _MDOutOrderStateIndex, value); }
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

    private ICollection<OutOrder> _OutOrder_MDOutOrderState;
    public virtual ICollection<OutOrder> OutOrder_MDOutOrderState
    {
        get { return LazyLoader.Load(this, ref _OutOrder_MDOutOrderState); }
        set { _OutOrder_MDOutOrderState = value; }
    }

    public bool OutOrder_MDOutOrderState_IsLoaded
    {
        get
        {
            return OutOrder_MDOutOrderState != null;
        }
    }

    public virtual CollectionEntry OutOrder_MDOutOrderStateReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrder_MDOutOrderState); }
    }
}
