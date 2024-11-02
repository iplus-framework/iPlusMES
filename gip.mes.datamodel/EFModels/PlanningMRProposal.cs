// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class PlanningMRProposal : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public PlanningMRProposal()
    {
    }

    private PlanningMRProposal(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _PlanningMRProposalID;
    public Guid PlanningMRProposalID 
    {
        get { return _PlanningMRProposalID; }
        set { SetProperty<Guid>(ref _PlanningMRProposalID, value); }
    }

    Guid _PlanningMRID;
    public Guid PlanningMRID 
    {
        get { return _PlanningMRID; }
        set { SetProperty<Guid>(ref _PlanningMRID, value); }
    }

    Guid? _InOrderID;
    public Guid? InOrderID 
    {
        get { return _InOrderID; }
        set { SetProperty<Guid?>(ref _InOrderID, value); }
    }

    Guid? _ProdOrderID;
    public Guid? ProdOrderID 
    {
        get { return _ProdOrderID; }
        set { SetProperty<Guid?>(ref _ProdOrderID, value); }
    }

    Guid? _ProdOrderPartslistID;
    public Guid? ProdOrderPartslistID 
    {
        get { return _ProdOrderPartslistID; }
        set { SetProperty<Guid?>(ref _ProdOrderPartslistID, value); }
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

    private InOrder _InOrder;
    public virtual InOrder InOrder
    { 
        get { return LazyLoader.Load(this, ref _InOrder); } 
        set { SetProperty<InOrder>(ref _InOrder, value); }
    }

    public bool InOrder_IsLoaded
    {
        get
        {
            return InOrder != null;
        }
    }

    public virtual ReferenceEntry InOrderReference 
    {
        get { return Context.Entry(this).Reference("InOrder"); }
    }
    
    private PlanningMR _PlanningMR;
    public virtual PlanningMR PlanningMR
    { 
        get { return LazyLoader.Load(this, ref _PlanningMR); } 
        set { SetProperty<PlanningMR>(ref _PlanningMR, value); }
    }

    public bool PlanningMR_IsLoaded
    {
        get
        {
            return PlanningMR != null;
        }
    }

    public virtual ReferenceEntry PlanningMRReference 
    {
        get { return Context.Entry(this).Reference("PlanningMR"); }
    }
    
    private ProdOrder _ProdOrder;
    public virtual ProdOrder ProdOrder
    { 
        get { return LazyLoader.Load(this, ref _ProdOrder); } 
        set { SetProperty<ProdOrder>(ref _ProdOrder, value); }
    }

    public bool ProdOrder_IsLoaded
    {
        get
        {
            return ProdOrder != null;
        }
    }

    public virtual ReferenceEntry ProdOrderReference 
    {
        get { return Context.Entry(this).Reference("ProdOrder"); }
    }
    
    private ProdOrderPartslist _ProdOrderPartslist;
    public virtual ProdOrderPartslist ProdOrderPartslist
    { 
        get { return LazyLoader.Load(this, ref _ProdOrderPartslist); } 
        set { SetProperty<ProdOrderPartslist>(ref _ProdOrderPartslist, value); }
    }

    public bool ProdOrderPartslist_IsLoaded
    {
        get
        {
            return ProdOrderPartslist != null;
        }
    }

    public virtual ReferenceEntry ProdOrderPartslistReference 
    {
        get { return Context.Entry(this).Reference("ProdOrderPartslist"); }
    }
    }
