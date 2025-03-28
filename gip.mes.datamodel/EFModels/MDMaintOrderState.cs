﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDMaintOrderState : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDMaintOrderState()
    {
    }

    private MDMaintOrderState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDMaintOrderStateID;
    public Guid MDMaintOrderStateID 
    {
        get { return _MDMaintOrderStateID; }
        set { SetProperty<Guid>(ref _MDMaintOrderStateID, value); }
    }

    short _MDMaintOrderStateIndex;
    public short MDMaintOrderStateIndex 
    {
        get { return _MDMaintOrderStateIndex; }
        set { SetProperty<short>(ref _MDMaintOrderStateIndex, value); }
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

    bool _IsDefault;
    public bool IsDefault 
    {
        get { return _IsDefault; }
        set { SetProperty<bool>(ref _IsDefault, value); }
    }

    private ICollection<MaintOrder> _MaintOrder_MDMaintOrderState;
    public virtual ICollection<MaintOrder> MaintOrder_MDMaintOrderState
    {
        get { return LazyLoader.Load(this, ref _MaintOrder_MDMaintOrderState); }
        set { _MaintOrder_MDMaintOrderState = value; }
    }

    public bool MaintOrder_MDMaintOrderState_IsLoaded
    {
        get
        {
            return _MaintOrder_MDMaintOrderState != null;
        }
    }

    public virtual CollectionEntry MaintOrder_MDMaintOrderStateReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintOrder_MDMaintOrderState); }
    }
}
