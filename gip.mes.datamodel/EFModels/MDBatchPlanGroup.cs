// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDBatchPlanGroup : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDBatchPlanGroup()
    {
    }

    private MDBatchPlanGroup(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDBatchPlanGroupID;
    public Guid MDBatchPlanGroupID 
    {
        get { return _MDBatchPlanGroupID; }
        set { SetProperty<Guid>(ref _MDBatchPlanGroupID, value); }
    }

    short _MDBatchPlanGroupIndex;
    public short MDBatchPlanGroupIndex 
    {
        get { return _MDBatchPlanGroupIndex; }
        set { SetProperty<short>(ref _MDBatchPlanGroupIndex, value); }
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

    string _MDKey;
    public string MDKey 
    {
        get { return _MDKey; }
        set { SetProperty<string>(ref _MDKey, value); }
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

    private ICollection<ProdOrderBatchPlan> _ProdOrderBatchPlan_MDBatchPlanGroup;
    public virtual ICollection<ProdOrderBatchPlan> ProdOrderBatchPlan_MDBatchPlanGroup
    {
        get { return LazyLoader.Load(this, ref _ProdOrderBatchPlan_MDBatchPlanGroup); }
        set { _ProdOrderBatchPlan_MDBatchPlanGroup = value; }
    }

    public bool ProdOrderBatchPlan_MDBatchPlanGroup_IsLoaded
    {
        get
        {
            return ProdOrderBatchPlan_MDBatchPlanGroup != null;
        }
    }

    public virtual CollectionEntry ProdOrderBatchPlan_MDBatchPlanGroupReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderBatchPlan_MDBatchPlanGroup); }
    }
}
