using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class FacilityMaterialOEE : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public FacilityMaterialOEE()
    {
    }

    private FacilityMaterialOEE(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _FacilityMaterialOEEID;
    public Guid FacilityMaterialOEEID 
    {
        get { return _FacilityMaterialOEEID; }
        set { SetProperty<Guid>(ref _FacilityMaterialOEEID, value); }
    }

    Guid _FacilityMaterialID;
    public Guid FacilityMaterialID 
    {
        get { return _FacilityMaterialID; }
        set { SetProperty<Guid>(ref _FacilityMaterialID, value); }
    }

    Guid? _ACProgramLogID;
    public Guid? ACProgramLogID 
    {
        get { return _ACProgramLogID; }
        set { SetProperty<Guid?>(ref _ACProgramLogID, value); }
    }

    DateTime _StartDate;
    public DateTime StartDate 
    {
        get { return _StartDate; }
        set { SetProperty<DateTime>(ref _StartDate, value); }
    }

    DateTime _EndDate;
    public DateTime EndDate 
    {
        get { return _EndDate; }
        set { SetProperty<DateTime>(ref _EndDate, value); }
    }

    int _IdleTimeSec;
    public int IdleTimeSec 
    {
        get { return _IdleTimeSec; }
        set { SetProperty<int>(ref _IdleTimeSec, value); }
    }

    int _StandByTimeSec;
    public int StandByTimeSec 
    {
        get { return _StandByTimeSec; }
        set { SetProperty<int>(ref _StandByTimeSec, value); }
    }

    int _OperationTimeSec;
    public int OperationTimeSec 
    {
        get { return _OperationTimeSec; }
        set { SetProperty<int>(ref _OperationTimeSec, value); }
    }

    int _ScheduledBreakTimeSec;
    public int ScheduledBreakTimeSec 
    {
        get { return _ScheduledBreakTimeSec; }
        set { SetProperty<int>(ref _ScheduledBreakTimeSec, value); }
    }

    int _UnscheduledBreakTimeSec;
    public int UnscheduledBreakTimeSec 
    {
        get { return _UnscheduledBreakTimeSec; }
        set { SetProperty<int>(ref _UnscheduledBreakTimeSec, value); }
    }

    int _RetoolingTimeSec;
    public int RetoolingTimeSec 
    {
        get { return _RetoolingTimeSec; }
        set { SetProperty<int>(ref _RetoolingTimeSec, value); }
    }

    int _MaintenanceTimeSec;
    public int MaintenanceTimeSec 
    {
        get { return _MaintenanceTimeSec; }
        set { SetProperty<int>(ref _MaintenanceTimeSec, value); }
    }

    double _AvailabilityOEE;
    public double AvailabilityOEE 
    {
        get { return _AvailabilityOEE; }
        set { SetProperty<double>(ref _AvailabilityOEE, value); }
    }

    double _Quantity;
    public double Quantity 
    {
        get { return _Quantity; }
        set { SetProperty<double>(ref _Quantity, value); }
    }

    double _Throughput;
    public double Throughput 
    {
        get { return _Throughput; }
        set { SetProperty<double>(ref _Throughput, value); }
    }

    double _PerformanceOEE;
    public double PerformanceOEE 
    {
        get { return _PerformanceOEE; }
        set { SetProperty<double>(ref _PerformanceOEE, value); }
    }

    double _QuantityScrap;
    public double QuantityScrap 
    {
        get { return _QuantityScrap; }
        set { SetProperty<double>(ref _QuantityScrap, value); }
    }

    double _QualityOEE;
    public double QualityOEE 
    {
        get { return _QualityOEE; }
        set { SetProperty<double>(ref _QualityOEE, value); }
    }

    double _TotalOEE;
    public double TotalOEE 
    {
        get { return _TotalOEE; }
        set { SetProperty<double>(ref _TotalOEE, value); }
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

    private FacilityMaterial _FacilityMaterial;
    public virtual FacilityMaterial FacilityMaterial
    { 
        get => LazyLoader.Load(this, ref _FacilityMaterial);
        set => _FacilityMaterial = value;
    }

    public bool FacilityMaterial_IsLoaded
    {
        get
        {
            return FacilityMaterial != null;
        }
    }

    public virtual ReferenceEntry FacilityMaterialReference 
    {
        get { return Context.Entry(this).Reference("FacilityMaterial"); }
    }
    }
