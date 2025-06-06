﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class FacilityHistory : VBEntityObject
{

    public FacilityHistory()
    {
    }

    private FacilityHistory(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _FacilityHistoryID;
    public Guid FacilityHistoryID 
    {
        get { return _FacilityHistoryID; }
        set { SetProperty<Guid>(ref _FacilityHistoryID, value); }
    }

    Guid? _HistoryID;
    public Guid? HistoryID 
    {
        get { return _HistoryID; }
        set { SetProperty<Guid?>(ref _HistoryID, value); }
    }

    Guid? _FacilityID;
    public Guid? FacilityID 
    {
        get { return _FacilityID; }
        set { SetProperty<Guid?>(ref _FacilityID, value); }
    }

    double _StockQuantity;
    public double StockQuantity 
    {
        get { return _StockQuantity; }
        set { SetProperty<double>(ref _StockQuantity, value); }
    }

    double _Outward;
    public double Outward 
    {
        get { return _Outward; }
        set { SetProperty<double>(ref _Outward, value); }
    }

    double _Inward;
    public double Inward 
    {
        get { return _Inward; }
        set { SetProperty<double>(ref _Inward, value); }
    }

    double _Adjustment;
    public double Adjustment 
    {
        get { return _Adjustment; }
        set { SetProperty<double>(ref _Adjustment, value); }
    }

    double _TargetOutward;
    public double TargetOutward 
    {
        get { return _TargetOutward; }
        set { SetProperty<double>(ref _TargetOutward, value); }
    }

    double _TargetInward;
    public double TargetInward 
    {
        get { return _TargetInward; }
        set { SetProperty<double>(ref _TargetInward, value); }
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

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
    }

    double _LastStockQuantity;
    public double LastStockQuantity 
    {
        get { return _LastStockQuantity; }
        set { SetProperty<double>(ref _LastStockQuantity, value); }
    }

    double _StockQuantityAmb;
    public double StockQuantityAmb 
    {
        get { return _StockQuantityAmb; }
        set { SetProperty<double>(ref _StockQuantityAmb, value); }
    }

    double _OutwardAmb;
    public double OutwardAmb 
    {
        get { return _OutwardAmb; }
        set { SetProperty<double>(ref _OutwardAmb, value); }
    }

    double _InwardAmb;
    public double InwardAmb 
    {
        get { return _InwardAmb; }
        set { SetProperty<double>(ref _InwardAmb, value); }
    }

    double _AdjustmentAmb;
    public double AdjustmentAmb 
    {
        get { return _AdjustmentAmb; }
        set { SetProperty<double>(ref _AdjustmentAmb, value); }
    }

    double _TargetOutwardAmb;
    public double TargetOutwardAmb 
    {
        get { return _TargetOutwardAmb; }
        set { SetProperty<double>(ref _TargetOutwardAmb, value); }
    }

    double _TargetInwardAmb;
    public double TargetInwardAmb 
    {
        get { return _TargetInwardAmb; }
        set { SetProperty<double>(ref _TargetInwardAmb, value); }
    }

    double _LastStockQuantityAmb;
    public double LastStockQuantityAmb 
    {
        get { return _LastStockQuantityAmb; }
        set { SetProperty<double>(ref _LastStockQuantityAmb, value); }
    }

    double? _MinStockQuantity;
    public double? MinStockQuantity 
    {
        get { return _MinStockQuantity; }
        set { SetProperty<double?>(ref _MinStockQuantity, value); }
    }

    double? _OptStockQuantity;
    public double? OptStockQuantity 
    {
        get { return _OptStockQuantity; }
        set { SetProperty<double?>(ref _OptStockQuantity, value); }
    }

    double _ReservedOutwardQuantity;
    public double ReservedOutwardQuantity 
    {
        get { return _ReservedOutwardQuantity; }
        set { SetProperty<double>(ref _ReservedOutwardQuantity, value); }
    }

    double _ReservedInwardQuantity;
    public double ReservedInwardQuantity 
    {
        get { return _ReservedInwardQuantity; }
        set { SetProperty<double>(ref _ReservedInwardQuantity, value); }
    }

    private Facility _Facility;
    public virtual Facility Facility
    { 
        get { return LazyLoader.Load(this, ref _Facility); } 
        set { SetProperty<Facility>(ref _Facility, value); }
    }

    public bool Facility_IsLoaded
    {
        get
        {
            return _Facility != null;
        }
    }

    public virtual ReferenceEntry FacilityReference 
    {
        get { return Context.Entry(this).Reference("Facility"); }
    }
    
    private History _History;
    public virtual History History
    { 
        get { return LazyLoader.Load(this, ref _History); } 
        set { SetProperty<History>(ref _History, value); }
    }

    public bool History_IsLoaded
    {
        get
        {
            return _History != null;
        }
    }

    public virtual ReferenceEntry HistoryReference 
    {
        get { return Context.Entry(this).Reference("History"); }
    }
    }
