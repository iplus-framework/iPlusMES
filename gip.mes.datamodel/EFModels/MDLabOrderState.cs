// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDLabOrderState : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDLabOrderState()
    {
    }

    private MDLabOrderState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDLabOrderStateID;
    public Guid MDLabOrderStateID 
    {
        get { return _MDLabOrderStateID; }
        set { SetProperty<Guid>(ref _MDLabOrderStateID, value); }
    }

    short _MDLabOrderStateIndex;
    public short MDLabOrderStateIndex 
    {
        get { return _MDLabOrderStateIndex; }
        set { SetProperty<short>(ref _MDLabOrderStateIndex, value); }
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

    private ICollection<LabOrder> _LabOrder_MDLabOrderState;
    public virtual ICollection<LabOrder> LabOrder_MDLabOrderState
    {
        get { return LazyLoader.Load(this, ref _LabOrder_MDLabOrderState); }
        set { _LabOrder_MDLabOrderState = value; }
    }

    public bool LabOrder_MDLabOrderState_IsLoaded
    {
        get
        {
            return LabOrder_MDLabOrderState != null;
        }
    }

    public virtual CollectionEntry LabOrder_MDLabOrderStateReference
    {
        get { return Context.Entry(this).Collection(c => c.LabOrder_MDLabOrderState); }
    }
}
