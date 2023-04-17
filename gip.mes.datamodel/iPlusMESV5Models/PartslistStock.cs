using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class PartslistStock : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public PartslistStock()
    {
    }

    private PartslistStock(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _PartslistStockID;
    public Guid PartslistStockID 
    {
        get { return _PartslistStockID; }
        set { SetProperty<Guid>(ref _PartslistStockID, value); }
    }

    Guid _PartslistID;
    public Guid PartslistID 
    {
        get { return _PartslistID; }
        set { SetProperty<Guid>(ref _PartslistID, value); }
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

    double? _DayLastOutward;
    public double? DayLastOutward 
    {
        get { return _DayLastOutward; }
        set { SetProperty<double?>(ref _DayLastOutward, value); }
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

    private MDReleaseState _MDReleaseState;
    public virtual MDReleaseState MDReleaseState
    { 
        get => LazyLoader.Load(this, ref _MDReleaseState);
        set => _MDReleaseState = value;
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
    
    private Partslist _Partslist;
    public virtual Partslist Partslist
    { 
        get => LazyLoader.Load(this, ref _Partslist);
        set => _Partslist = value;
    }

    public bool Partslist_IsLoaded
    {
        get
        {
            return Partslist != null;
        }
    }

    public virtual ReferenceEntry PartslistReference 
    {
        get { return Context.Entry(this).Reference("Partslist"); }
    }
    }
