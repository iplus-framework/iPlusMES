using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class DemandProdOrder : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public DemandProdOrder()
    {
    }

    private DemandProdOrder(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _DemandProdOrderID;
    public Guid DemandProdOrderID 
    {
        get { return _DemandProdOrderID; }
        set { SetProperty<Guid>(ref _DemandProdOrderID, value); }
    }

    Guid _DemandOrderID;
    public Guid DemandOrderID 
    {
        get { return _DemandOrderID; }
        set { SetProperty<Guid>(ref _DemandOrderID, value); }
    }

    string _ProgramNo;
    public string ProgramNo 
    {
        get { return _ProgramNo; }
        set { SetProperty<string>(ref _ProgramNo, value); }
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

    private DemandOrder _DemandOrder;
    public virtual DemandOrder DemandOrder
    { 
        get => LazyLoader.Load(this, ref _DemandOrder);
        set => _DemandOrder = value;
    }

    public bool DemandOrder_IsLoaded
    {
        get
        {
            return DemandOrder != null;
        }
    }

    public virtual ReferenceEntry DemandOrderReference 
    {
        get { return Context.Entry(this).Reference("DemandOrder"); }
    }
    }
