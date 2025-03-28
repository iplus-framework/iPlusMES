﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDLabOrderPosState : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDLabOrderPosState()
    {
    }

    private MDLabOrderPosState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDLabOrderPosStateID;
    public Guid MDLabOrderPosStateID 
    {
        get { return _MDLabOrderPosStateID; }
        set { SetProperty<Guid>(ref _MDLabOrderPosStateID, value); }
    }

    short _MDLabOrderPosStateIndex;
    public short MDLabOrderPosStateIndex 
    {
        get { return _MDLabOrderPosStateIndex; }
        set { SetProperty<short>(ref _MDLabOrderPosStateIndex, value); }
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

    private ICollection<LabOrderPos> _LabOrderPos_MDLabOrderPosState;
    public virtual ICollection<LabOrderPos> LabOrderPos_MDLabOrderPosState
    {
        get { return LazyLoader.Load(this, ref _LabOrderPos_MDLabOrderPosState); }
        set { _LabOrderPos_MDLabOrderPosState = value; }
    }

    public bool LabOrderPos_MDLabOrderPosState_IsLoaded
    {
        get
        {
            return _LabOrderPos_MDLabOrderPosState != null;
        }
    }

    public virtual CollectionEntry LabOrderPos_MDLabOrderPosStateReference
    {
        get { return Context.Entry(this).Collection(c => c.LabOrderPos_MDLabOrderPosState); }
    }
}
