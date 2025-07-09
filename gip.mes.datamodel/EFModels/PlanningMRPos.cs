using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class PlanningMRPos : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public PlanningMRPos()
    {
    }

    private PlanningMRPos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _PlanningMRPosID;
    public Guid PlanningMRPosID 
    {
        get { return _PlanningMRPosID; }
        set { SetProperty<Guid>(ref _PlanningMRPosID, value); }
    }

    Guid _PlanningMRConsID;
    public Guid PlanningMRConsID 
    {
        get { return _PlanningMRConsID; }
        set { SetProperty<Guid>(ref _PlanningMRConsID, value); }
    }

    Guid? _OutOrderPosID;
    public Guid? OutOrderPosID 
    {
        get { return _OutOrderPosID; }
        set { SetProperty<Guid?>(ref _OutOrderPosID, value); }
    }

    Guid? _ProdOrderPartslistPosID;
    public Guid? ProdOrderPartslistPosID 
    {
        get { return _ProdOrderPartslistPosID; }
        set { SetProperty<Guid?>(ref _ProdOrderPartslistPosID, value); }
    }

    double _StoreQuantityUOM;
    public double StoreQuantityUOM 
    {
        get { return _StoreQuantityUOM; }
        set { SetProperty<double>(ref _StoreQuantityUOM, value); }
    }

    Guid? _PlanningMRProposalID;
    public Guid? PlanningMRProposalID 
    {
        get { return _PlanningMRProposalID; }
        set { SetProperty<Guid?>(ref _PlanningMRProposalID, value); }
    }

    Guid? _InOrderPosID;
    public Guid? InOrderPosID 
    {
        get { return _InOrderPosID; }
        set { SetProperty<Guid?>(ref _InOrderPosID, value); }
    }

    Guid? _ProdOrderPartslistID;
    public Guid? ProdOrderPartslistID 
    {
        get { return _ProdOrderPartslistID; }
        set { SetProperty<Guid?>(ref _ProdOrderPartslistID, value); }
    }

    DateTime? _ExpectedBookingDate;
    public DateTime? ExpectedBookingDate 
    {
        get { return _ExpectedBookingDate; }
        set { SetProperty<DateTime?>(ref _ExpectedBookingDate, value); }
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

    private InOrderPos _InOrderPos;
    public virtual InOrderPos InOrderPos
    { 
        get { return LazyLoader.Load(this, ref _InOrderPos); } 
        set { SetProperty<InOrderPos>(ref _InOrderPos, value); }
    }

    public bool InOrderPos_IsLoaded
    {
        get
        {
            return _InOrderPos != null;
        }
    }

    public virtual ReferenceEntry InOrderPosReference 
    {
        get { return Context.Entry(this).Reference("InOrderPos"); }
    }
    
    private OutOrderPos _OutOrderPos;
    public virtual OutOrderPos OutOrderPos
    { 
        get { return LazyLoader.Load(this, ref _OutOrderPos); } 
        set { SetProperty<OutOrderPos>(ref _OutOrderPos, value); }
    }

    public bool OutOrderPos_IsLoaded
    {
        get
        {
            return _OutOrderPos != null;
        }
    }

    public virtual ReferenceEntry OutOrderPosReference 
    {
        get { return Context.Entry(this).Reference("OutOrderPos"); }
    }
    
    private PlanningMRCons _PlanningMRCons;
    public virtual PlanningMRCons PlanningMRCons
    { 
        get { return LazyLoader.Load(this, ref _PlanningMRCons); } 
        set { SetProperty<PlanningMRCons>(ref _PlanningMRCons, value); }
    }

    public bool PlanningMRCons_IsLoaded
    {
        get
        {
            return _PlanningMRCons != null;
        }
    }

    public virtual ReferenceEntry PlanningMRConsReference 
    {
        get { return Context.Entry(this).Reference("PlanningMRCons"); }
    }
    
    private PlanningMRProposal _PlanningMRProposal;
    public virtual PlanningMRProposal PlanningMRProposal
    { 
        get { return LazyLoader.Load(this, ref _PlanningMRProposal); } 
        set { SetProperty<PlanningMRProposal>(ref _PlanningMRProposal, value); }
    }

    public bool PlanningMRProposal_IsLoaded
    {
        get
        {
            return _PlanningMRProposal != null;
        }
    }

    public virtual ReferenceEntry PlanningMRProposalReference 
    {
        get { return Context.Entry(this).Reference("PlanningMRProposal"); }
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
            return _ProdOrderPartslist != null;
        }
    }

    public virtual ReferenceEntry ProdOrderPartslistReference 
    {
        get { return Context.Entry(this).Reference("ProdOrderPartslist"); }
    }
    
    private ProdOrderPartslistPos _ProdOrderPartslistPos;
    public virtual ProdOrderPartslistPos ProdOrderPartslistPos
    { 
        get { return LazyLoader.Load(this, ref _ProdOrderPartslistPos); } 
        set { SetProperty<ProdOrderPartslistPos>(ref _ProdOrderPartslistPos, value); }
    }

    public bool ProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return _ProdOrderPartslistPos != null;
        }
    }

    public virtual ReferenceEntry ProdOrderPartslistPosReference 
    {
        get { return Context.Entry(this).Reference("ProdOrderPartslistPos"); }
    }
    }
