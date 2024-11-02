// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class FacilityLotStock : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public FacilityLotStock()
    {
    }

    private FacilityLotStock(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _FacilityLotStockID;
    public Guid FacilityLotStockID 
    {
        get { return _FacilityLotStockID; }
        set { SetProperty<Guid>(ref _FacilityLotStockID, value); }
    }

    Guid _FacilityLotID;
    public Guid FacilityLotID 
    {
        get { return _FacilityLotID; }
        set { SetProperty<Guid>(ref _FacilityLotID, value); }
    }

    Guid? _MDReleaseStateID;
    public Guid? MDReleaseStateID 
    {
        get { return _MDReleaseStateID; }
        set { SetProperty<Guid?>(ref _MDReleaseStateID, value); }
    }

    double _StockQuantity;
    public double StockQuantity 
    {
        get { return _StockQuantity; }
        set { SetProperty<double>(ref _StockQuantity, value); }
    }

    double _StockWeight;
    public double StockWeight 
    {
        get { return _StockWeight; }
        set { SetProperty<double>(ref _StockWeight, value); }
    }

    double _ReservedInwardQuantity;
    public double ReservedInwardQuantity 
    {
        get { return _ReservedInwardQuantity; }
        set { SetProperty<double>(ref _ReservedInwardQuantity, value); }
    }

    double _ReservedOutwardQuantity;
    public double ReservedOutwardQuantity 
    {
        get { return _ReservedOutwardQuantity; }
        set { SetProperty<double>(ref _ReservedOutwardQuantity, value); }
    }

    double _DayOutward;
    public double DayOutward 
    {
        get { return _DayOutward; }
        set { SetProperty<double>(ref _DayOutward, value); }
    }

    double _DayLastOutward;
    public double DayLastOutward 
    {
        get { return _DayLastOutward; }
        set { SetProperty<double>(ref _DayLastOutward, value); }
    }

    double _DayLastStock;
    public double DayLastStock 
    {
        get { return _DayLastStock; }
        set { SetProperty<double>(ref _DayLastStock, value); }
    }

    double _DayInward;
    public double DayInward 
    {
        get { return _DayInward; }
        set { SetProperty<double>(ref _DayInward, value); }
    }

    double _DayAdjustment;
    public double DayAdjustment 
    {
        get { return _DayAdjustment; }
        set { SetProperty<double>(ref _DayAdjustment, value); }
    }

    DateTime? _DayBalanceDate;
    public DateTime? DayBalanceDate 
    {
        get { return _DayBalanceDate; }
        set { SetProperty<DateTime?>(ref _DayBalanceDate, value); }
    }

    double _WeekOutward;
    public double WeekOutward 
    {
        get { return _WeekOutward; }
        set { SetProperty<double>(ref _WeekOutward, value); }
    }

    double _WeekInward;
    public double WeekInward 
    {
        get { return _WeekInward; }
        set { SetProperty<double>(ref _WeekInward, value); }
    }

    double _WeekAdjustment;
    public double WeekAdjustment 
    {
        get { return _WeekAdjustment; }
        set { SetProperty<double>(ref _WeekAdjustment, value); }
    }

    DateTime? _WeekBalanceDate;
    public DateTime? WeekBalanceDate 
    {
        get { return _WeekBalanceDate; }
        set { SetProperty<DateTime?>(ref _WeekBalanceDate, value); }
    }

    double _MonthOutward;
    public double MonthOutward 
    {
        get { return _MonthOutward; }
        set { SetProperty<double>(ref _MonthOutward, value); }
    }

    double _MonthActStock;
    public double MonthActStock 
    {
        get { return _MonthActStock; }
        set { SetProperty<double>(ref _MonthActStock, value); }
    }

    double _MonthLastStock;
    public double MonthLastStock 
    {
        get { return _MonthLastStock; }
        set { SetProperty<double>(ref _MonthLastStock, value); }
    }

    double _MonthAverageStock;
    public double MonthAverageStock 
    {
        get { return _MonthAverageStock; }
        set { SetProperty<double>(ref _MonthAverageStock, value); }
    }

    double _MonthLastOutward;
    public double MonthLastOutward 
    {
        get { return _MonthLastOutward; }
        set { SetProperty<double>(ref _MonthLastOutward, value); }
    }

    double _MonthInward;
    public double MonthInward 
    {
        get { return _MonthInward; }
        set { SetProperty<double>(ref _MonthInward, value); }
    }

    double _MonthAdjustment;
    public double MonthAdjustment 
    {
        get { return _MonthAdjustment; }
        set { SetProperty<double>(ref _MonthAdjustment, value); }
    }

    DateTime? _MonthBalanceDate;
    public DateTime? MonthBalanceDate 
    {
        get { return _MonthBalanceDate; }
        set { SetProperty<DateTime?>(ref _MonthBalanceDate, value); }
    }

    double _YearOutward;
    public double YearOutward 
    {
        get { return _YearOutward; }
        set { SetProperty<double>(ref _YearOutward, value); }
    }

    double _YearInward;
    public double YearInward 
    {
        get { return _YearInward; }
        set { SetProperty<double>(ref _YearInward, value); }
    }

    double _YearAdjustment;
    public double YearAdjustment 
    {
        get { return _YearAdjustment; }
        set { SetProperty<double>(ref _YearAdjustment, value); }
    }

    DateTime? _YearBalanceDate;
    public DateTime? YearBalanceDate 
    {
        get { return _YearBalanceDate; }
        set { SetProperty<DateTime?>(ref _YearBalanceDate, value); }
    }

    double _DayTargetInward;
    public double DayTargetInward 
    {
        get { return _DayTargetInward; }
        set { SetProperty<double>(ref _DayTargetInward, value); }
    }

    double _DayTargetOutward;
    public double DayTargetOutward 
    {
        get { return _DayTargetOutward; }
        set { SetProperty<double>(ref _DayTargetOutward, value); }
    }

    double _WeekTargetInward;
    public double WeekTargetInward 
    {
        get { return _WeekTargetInward; }
        set { SetProperty<double>(ref _WeekTargetInward, value); }
    }

    double _WeekTargetOutward;
    public double WeekTargetOutward 
    {
        get { return _WeekTargetOutward; }
        set { SetProperty<double>(ref _WeekTargetOutward, value); }
    }

    double _MonthTargetInward;
    public double MonthTargetInward 
    {
        get { return _MonthTargetInward; }
        set { SetProperty<double>(ref _MonthTargetInward, value); }
    }

    double _MonthTargetOutward;
    public double MonthTargetOutward 
    {
        get { return _MonthTargetOutward; }
        set { SetProperty<double>(ref _MonthTargetOutward, value); }
    }

    double _YearTargetInward;
    public double YearTargetInward 
    {
        get { return _YearTargetInward; }
        set { SetProperty<double>(ref _YearTargetInward, value); }
    }

    double _YearTargetOutward;
    public double YearTargetOutward 
    {
        get { return _YearTargetOutward; }
        set { SetProperty<double>(ref _YearTargetOutward, value); }
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

    byte[] _RowVersion;
    public byte[] RowVersion 
    {
        get { return _RowVersion; }
        set { SetProperty<byte[]>(ref _RowVersion, value); }
    }

    double _StockQuantityAmb;
    public double StockQuantityAmb 
    {
        get { return _StockQuantityAmb; }
        set { SetProperty<double>(ref _StockQuantityAmb, value); }
    }

    double _DayOutwardAmb;
    public double DayOutwardAmb 
    {
        get { return _DayOutwardAmb; }
        set { SetProperty<double>(ref _DayOutwardAmb, value); }
    }

    double _DayLastOutwardAmb;
    public double DayLastOutwardAmb 
    {
        get { return _DayLastOutwardAmb; }
        set { SetProperty<double>(ref _DayLastOutwardAmb, value); }
    }

    double _DayLastStockAmb;
    public double DayLastStockAmb 
    {
        get { return _DayLastStockAmb; }
        set { SetProperty<double>(ref _DayLastStockAmb, value); }
    }

    double _DayInwardAmb;
    public double DayInwardAmb 
    {
        get { return _DayInwardAmb; }
        set { SetProperty<double>(ref _DayInwardAmb, value); }
    }

    double _DayAdjustmentAmb;
    public double DayAdjustmentAmb 
    {
        get { return _DayAdjustmentAmb; }
        set { SetProperty<double>(ref _DayAdjustmentAmb, value); }
    }

    double _WeekOutwardAmb;
    public double WeekOutwardAmb 
    {
        get { return _WeekOutwardAmb; }
        set { SetProperty<double>(ref _WeekOutwardAmb, value); }
    }

    double _WeekInwardAmb;
    public double WeekInwardAmb 
    {
        get { return _WeekInwardAmb; }
        set { SetProperty<double>(ref _WeekInwardAmb, value); }
    }

    double _WeekAdjustmentAmb;
    public double WeekAdjustmentAmb 
    {
        get { return _WeekAdjustmentAmb; }
        set { SetProperty<double>(ref _WeekAdjustmentAmb, value); }
    }

    double _MonthOutwardAmb;
    public double MonthOutwardAmb 
    {
        get { return _MonthOutwardAmb; }
        set { SetProperty<double>(ref _MonthOutwardAmb, value); }
    }

    double _MonthActStockAmb;
    public double MonthActStockAmb 
    {
        get { return _MonthActStockAmb; }
        set { SetProperty<double>(ref _MonthActStockAmb, value); }
    }

    double _MonthLastStockAmb;
    public double MonthLastStockAmb 
    {
        get { return _MonthLastStockAmb; }
        set { SetProperty<double>(ref _MonthLastStockAmb, value); }
    }

    double _MonthAverageStockAmb;
    public double MonthAverageStockAmb 
    {
        get { return _MonthAverageStockAmb; }
        set { SetProperty<double>(ref _MonthAverageStockAmb, value); }
    }

    double _MonthLastOutwardAmb;
    public double MonthLastOutwardAmb 
    {
        get { return _MonthLastOutwardAmb; }
        set { SetProperty<double>(ref _MonthLastOutwardAmb, value); }
    }

    double _MonthInwardAmb;
    public double MonthInwardAmb 
    {
        get { return _MonthInwardAmb; }
        set { SetProperty<double>(ref _MonthInwardAmb, value); }
    }

    double _MonthAdjustmentAmb;
    public double MonthAdjustmentAmb 
    {
        get { return _MonthAdjustmentAmb; }
        set { SetProperty<double>(ref _MonthAdjustmentAmb, value); }
    }

    double _YearOutwardAmb;
    public double YearOutwardAmb 
    {
        get { return _YearOutwardAmb; }
        set { SetProperty<double>(ref _YearOutwardAmb, value); }
    }

    double _YearInwardAmb;
    public double YearInwardAmb 
    {
        get { return _YearInwardAmb; }
        set { SetProperty<double>(ref _YearInwardAmb, value); }
    }

    double _YearAdjustmentAmb;
    public double YearAdjustmentAmb 
    {
        get { return _YearAdjustmentAmb; }
        set { SetProperty<double>(ref _YearAdjustmentAmb, value); }
    }

    double _DayTargetInwardAmb;
    public double DayTargetInwardAmb 
    {
        get { return _DayTargetInwardAmb; }
        set { SetProperty<double>(ref _DayTargetInwardAmb, value); }
    }

    double _DayTargetOutwardAmb;
    public double DayTargetOutwardAmb 
    {
        get { return _DayTargetOutwardAmb; }
        set { SetProperty<double>(ref _DayTargetOutwardAmb, value); }
    }

    double _WeekTargetInwardAmb;
    public double WeekTargetInwardAmb 
    {
        get { return _WeekTargetInwardAmb; }
        set { SetProperty<double>(ref _WeekTargetInwardAmb, value); }
    }

    double _WeekTargetOutwardAmb;
    public double WeekTargetOutwardAmb 
    {
        get { return _WeekTargetOutwardAmb; }
        set { SetProperty<double>(ref _WeekTargetOutwardAmb, value); }
    }

    double _MonthTargetInwardAmb;
    public double MonthTargetInwardAmb 
    {
        get { return _MonthTargetInwardAmb; }
        set { SetProperty<double>(ref _MonthTargetInwardAmb, value); }
    }

    double _MonthTargetOutwardAmb;
    public double MonthTargetOutwardAmb 
    {
        get { return _MonthTargetOutwardAmb; }
        set { SetProperty<double>(ref _MonthTargetOutwardAmb, value); }
    }

    double _YearTargetInwardAmb;
    public double YearTargetInwardAmb 
    {
        get { return _YearTargetInwardAmb; }
        set { SetProperty<double>(ref _YearTargetInwardAmb, value); }
    }

    double _YearTargetOutwardAmb;
    public double YearTargetOutwardAmb 
    {
        get { return _YearTargetOutwardAmb; }
        set { SetProperty<double>(ref _YearTargetOutwardAmb, value); }
    }

    private FacilityLot _FacilityLot;
    public virtual FacilityLot FacilityLot
    { 
        get { return LazyLoader.Load(this, ref _FacilityLot); } 
        set { SetProperty<FacilityLot>(ref _FacilityLot, value); }
    }

    public bool FacilityLot_IsLoaded
    {
        get
        {
            return FacilityLot != null;
        }
    }

    public virtual ReferenceEntry FacilityLotReference 
    {
        get { return Context.Entry(this).Reference("FacilityLot"); }
    }
    
    private MDReleaseState _MDReleaseState;
    public virtual MDReleaseState MDReleaseState
    { 
        get { return LazyLoader.Load(this, ref _MDReleaseState); } 
        set { SetProperty<MDReleaseState>(ref _MDReleaseState, value); }
    }

    public bool MDReleaseState_IsLoaded
    {
        get
        {
            return MDReleaseState != null;
        }
    }

    public virtual ReferenceEntry MDReleaseStateReference 
    {
        get { return Context.Entry(this).Reference("MDReleaseState"); }
    }
    }
