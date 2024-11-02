// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDOutOrderPosState : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDOutOrderPosState()
    {
    }

    private MDOutOrderPosState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDOutOrderPosStateID;
    public Guid MDOutOrderPosStateID 
    {
        get { return _MDOutOrderPosStateID; }
        set { SetProperty<Guid>(ref _MDOutOrderPosStateID, value); }
    }

    short _MDOutOrderPosStateIndex;
    public short MDOutOrderPosStateIndex 
    {
        get { return _MDOutOrderPosStateIndex; }
        set { SetProperty<short>(ref _MDOutOrderPosStateIndex, value); }
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

    private ICollection<OutOrderPos> _OutOrderPos_MDOutOrderPosState;
    public virtual ICollection<OutOrderPos> OutOrderPos_MDOutOrderPosState
    {
        get { return LazyLoader.Load(this, ref _OutOrderPos_MDOutOrderPosState); }
        set { _OutOrderPos_MDOutOrderPosState = value; }
    }

    public bool OutOrderPos_MDOutOrderPosState_IsLoaded
    {
        get
        {
            return OutOrderPos_MDOutOrderPosState != null;
        }
    }

    public virtual CollectionEntry OutOrderPos_MDOutOrderPosStateReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPos_MDOutOrderPosState); }
    }
}
