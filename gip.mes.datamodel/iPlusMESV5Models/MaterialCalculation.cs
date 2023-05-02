using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MaterialCalculation : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public MaterialCalculation()
    {
    }

    private MaterialCalculation(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MaterialCalculationID;
    public Guid MaterialCalculationID 
    {
        get { return _MaterialCalculationID; }
        set { SetProperty<Guid>(ref _MaterialCalculationID, value); }
    }

    Guid _MaterialID;
    public Guid MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid>(ref _MaterialID, value); }
    }

    string _MaterialCalculationNo;
    public string MaterialCalculationNo 
    {
        get { return _MaterialCalculationNo; }
        set { SetProperty<string>(ref _MaterialCalculationNo, value); }
    }

    double _ProductionQuantity;
    public double ProductionQuantity 
    {
        get { return _ProductionQuantity; }
        set { SetProperty<double>(ref _ProductionQuantity, value); }
    }

    DateTime _ValidFromDate;
    public DateTime ValidFromDate 
    {
        get { return _ValidFromDate; }
        set { SetProperty<DateTime>(ref _ValidFromDate, value); }
    }

    DateTime _ValidToDate;
    public DateTime ValidToDate 
    {
        get { return _ValidToDate; }
        set { SetProperty<DateTime>(ref _ValidToDate, value); }
    }

    decimal _CostReQuantity;
    public decimal CostReQuantity 
    {
        get { return _CostReQuantity; }
        set { SetProperty<decimal>(ref _CostReQuantity, value); }
    }

    decimal _CostMat;
    public decimal CostMat 
    {
        get { return _CostMat; }
        set { SetProperty<decimal>(ref _CostMat, value); }
    }

    decimal _CostVar;
    public decimal CostVar 
    {
        get { return _CostVar; }
        set { SetProperty<decimal>(ref _CostVar, value); }
    }

    decimal _CostFix;
    public decimal CostFix 
    {
        get { return _CostFix; }
        set { SetProperty<decimal>(ref _CostFix, value); }
    }

    decimal _CostPack;
    public decimal CostPack 
    {
        get { return _CostPack; }
        set { SetProperty<decimal>(ref _CostPack, value); }
    }

    decimal _CostGeneral;
    public decimal CostGeneral 
    {
        get { return _CostGeneral; }
        set { SetProperty<decimal>(ref _CostGeneral, value); }
    }

    decimal _CostLoss;
    public decimal CostLoss 
    {
        get { return _CostLoss; }
        set { SetProperty<decimal>(ref _CostLoss, value); }
    }

    decimal _CostHandlingVar;
    public decimal CostHandlingVar 
    {
        get { return _CostHandlingVar; }
        set { SetProperty<decimal>(ref _CostHandlingVar, value); }
    }

    decimal _CostHandlingFix;
    public decimal CostHandlingFix 
    {
        get { return _CostHandlingFix; }
        set { SetProperty<decimal>(ref _CostHandlingFix, value); }
    }

    DateTime _CalculationDate;
    public DateTime CalculationDate 
    {
        get { return _CalculationDate; }
        set { SetProperty<DateTime>(ref _CalculationDate, value); }
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

    private Material _Material;
    public virtual Material Material
    { 
        get => LazyLoader.Load(this, ref _Material);
        set => _Material = value;
    }

    public bool Material_IsLoaded
    {
        get
        {
            return Material != null;
        }
    }

    public virtual ReferenceEntry MaterialReference 
    {
        get { return Context.Entry(this).Reference("Material"); }
    }
    }
