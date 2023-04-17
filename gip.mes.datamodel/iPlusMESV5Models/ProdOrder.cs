using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ProdOrder : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public ProdOrder()
    {
    }

    private ProdOrder(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _ProdOrderID;
    public Guid ProdOrderID 
    {
        get { return _ProdOrderID; }
        set { SetProperty<Guid>(ref _ProdOrderID, value); }
    }

    string _ProgramNo;
    public string ProgramNo 
    {
        get { return _ProgramNo; }
        set { SetProperty<string>(ref _ProgramNo, value); }
    }

    Guid _MDProdOrderStateID;
    public Guid MDProdOrderStateID 
    {
        get { return _MDProdOrderStateID; }
        set { SetProperty<Guid>(ref _MDProdOrderStateID, value); }
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

    Guid? _CPartnerCompanyID;
    public Guid? CPartnerCompanyID 
    {
        get { return _CPartnerCompanyID; }
        set { SetProperty<Guid?>(ref _CPartnerCompanyID, value); }
    }

    string _KeyOfExtSys;
    public string KeyOfExtSys 
    {
        get { return _KeyOfExtSys; }
        set { SetProperty<string>(ref _KeyOfExtSys, value); }
    }

    private Company _CPartnerCompany;
    public virtual Company CPartnerCompany
    { 
        get => LazyLoader.Load(this, ref _CPartnerCompany);
        set => _CPartnerCompany = value;
    }

    public bool CPartnerCompany_IsLoaded
    {
        get
        {
            return CPartnerCompany != null;
        }
    }

    public virtual ReferenceEntry CPartnerCompanyReference 
    {
        get { return Context.Entry(this).Reference("CPartnerCompany"); }
    }
    
    private MDProdOrderState _MDProdOrderState;
    public virtual MDProdOrderState MDProdOrderState
    { 
        get => LazyLoader.Load(this, ref _MDProdOrderState);
        set => _MDProdOrderState = value;
    }

    public bool MDProdOrderState_IsLoaded
    {
        get
        {
            return MDProdOrderState != null;
        }
    }

    public virtual ReferenceEntry MDProdOrderStateReference 
    {
        get { return Context.Entry(this).Reference("MDProdOrderState"); }
    }
    
    private ICollection<PlanningMRProposal> _PlanningMRProposal_ProdOrder;
    public virtual ICollection<PlanningMRProposal> PlanningMRProposal_ProdOrder
    {
        get => LazyLoader.Load(this, ref _PlanningMRProposal_ProdOrder);
        set => _PlanningMRProposal_ProdOrder = value;
    }

    public bool PlanningMRProposal_ProdOrder_IsLoaded
    {
        get
        {
            return PlanningMRProposal_ProdOrder != null;
        }
    }

    public virtual CollectionEntry PlanningMRProposal_ProdOrderReference
    {
        get { return Context.Entry(this).Collection(c => c.PlanningMRProposal_ProdOrder); }
    }

    private ICollection<ProdOrderPartslist> _ProdOrderPartslist_ProdOrder;
    public virtual ICollection<ProdOrderPartslist> ProdOrderPartslist_ProdOrder
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslist_ProdOrder);
        set => _ProdOrderPartslist_ProdOrder = value;
    }

    public bool ProdOrderPartslist_ProdOrder_IsLoaded
    {
        get
        {
            return ProdOrderPartslist_ProdOrder != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslist_ProdOrderReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslist_ProdOrder); }
    }

    private ICollection<TandTv2StepItem> _TandTv2StepItem_ProdOrder;
    public virtual ICollection<TandTv2StepItem> TandTv2StepItem_ProdOrder
    {
        get => LazyLoader.Load(this, ref _TandTv2StepItem_ProdOrder);
        set => _TandTv2StepItem_ProdOrder = value;
    }

    public bool TandTv2StepItem_ProdOrder_IsLoaded
    {
        get
        {
            return TandTv2StepItem_ProdOrder != null;
        }
    }

    public virtual CollectionEntry TandTv2StepItem_ProdOrderReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv2StepItem_ProdOrder); }
    }
}
