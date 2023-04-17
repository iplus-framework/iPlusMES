using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class DemandOrder : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public DemandOrder()
    {
    }

    private DemandOrder(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _DemandOrderID;
    public Guid DemandOrderID 
    {
        get { return _DemandOrderID; }
        set { SetProperty<Guid>(ref _DemandOrderID, value); }
    }

    string _DemandOrderNo;
    public string DemandOrderNo 
    {
        get { return _DemandOrderNo; }
        set { SetProperty<string>(ref _DemandOrderNo, value); }
    }

    string _DemandOrderName;
    public string DemandOrderName 
    {
        get { return _DemandOrderName; }
        set { SetProperty<string>(ref _DemandOrderName, value); }
    }

    Guid _MDDemandOrderStateID;
    public Guid MDDemandOrderStateID 
    {
        get { return _MDDemandOrderStateID; }
        set { SetProperty<Guid>(ref _MDDemandOrderStateID, value); }
    }

    DateTime? _DemandOrderDate;
    public DateTime? DemandOrderDate 
    {
        get { return _DemandOrderDate; }
        set { SetProperty<DateTime?>(ref _DemandOrderDate, value); }
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

    private ICollection<DemandOrderPo> _DemandOrderPo_DemandOrder;
    public virtual ICollection<DemandOrderPo> DemandOrderPo_DemandOrder
    {
        get => LazyLoader.Load(this, ref _DemandOrderPo_DemandOrder);
        set => _DemandOrderPo_DemandOrder = value;
    }

    public bool DemandOrderPo_DemandOrder_IsLoaded
    {
        get
        {
            return DemandOrderPo_DemandOrder != null;
        }
    }

    public virtual CollectionEntry DemandOrderPo_DemandOrderReference
    {
        get { return Context.Entry(this).Collection(c => c.DemandOrderPo_DemandOrder); }
    }

    private ICollection<DemandProdOrder> _DemandProdOrder_DemandOrder;
    public virtual ICollection<DemandProdOrder> DemandProdOrder_DemandOrder
    {
        get => LazyLoader.Load(this, ref _DemandProdOrder_DemandOrder);
        set => _DemandProdOrder_DemandOrder = value;
    }

    public bool DemandProdOrder_DemandOrder_IsLoaded
    {
        get
        {
            return DemandProdOrder_DemandOrder != null;
        }
    }

    public virtual CollectionEntry DemandProdOrder_DemandOrderReference
    {
        get { return Context.Entry(this).Collection(c => c.DemandProdOrder_DemandOrder); }
    }

    private MDDemandOrderState _MDDemandOrderState;
    public virtual MDDemandOrderState MDDemandOrderState
    { 
        get => LazyLoader.Load(this, ref _MDDemandOrderState);
        set => _MDDemandOrderState = value;
    }

    public bool MDDemandOrderState_IsLoaded
    {
        get
        {
            return MDDemandOrderState != null;
        }
    }

    public virtual ReferenceEntry MDDemandOrderStateReference 
    {
        get { return Context.Entry(this).Reference("MDDemandOrderState"); }
    }
    }
