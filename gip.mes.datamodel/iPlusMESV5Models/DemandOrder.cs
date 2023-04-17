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

    private ICollection<DemandOrderPos> _DemandOrderPos_DemandOrder;
    public virtual ICollection<DemandOrderPos> DemandOrderPos_DemandOrder
    {
        get => LazyLoader.Load(this, ref _DemandOrderPos_DemandOrder);
        set => _DemandOrderPos_DemandOrder = value;
    }

    public bool DemandOrderPos_DemandOrder_IsLoaded
    {
        get
        {
            return DemandOrderPos_DemandOrder != null;
        }
    }

    public virtual CollectionEntry DemandOrderPos_DemandOrderReference
    {
        get { return Context.Entry(this).Collection(c => c.DemandOrderPos_DemandOrder); }
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
