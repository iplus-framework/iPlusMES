using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class PlanningMRCons : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public PlanningMRCons()
    {
    }

    private PlanningMRCons(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _PlanningMRConsID;
    public Guid PlanningMRConsID 
    {
        get { return _PlanningMRConsID; }
        set { SetProperty<Guid>(ref _PlanningMRConsID, value); }
    }

    Guid _PlanningMRID;
    public Guid PlanningMRID 
    {
        get { return _PlanningMRID; }
        set { SetProperty<Guid>(ref _PlanningMRID, value); }
    }

    Guid? _MaterialID;
    public Guid? MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid?>(ref _MaterialID, value); }
    }

    DateTime _ConsumptionDate;
    public DateTime ConsumptionDate 
    {
        get { return _ConsumptionDate; }
        set { SetProperty<DateTime>(ref _ConsumptionDate, value); }
    }

    double _EstimatedQuantityUOM;
    public double EstimatedQuantityUOM 
    {
        get { return _EstimatedQuantityUOM; }
        set { SetProperty<double>(ref _EstimatedQuantityUOM, value); }
    }

    double _ReqCorrectionQuantityUOM;
    public double ReqCorrectionQuantityUOM 
    {
        get { return _ReqCorrectionQuantityUOM; }
        set { SetProperty<double>(ref _ReqCorrectionQuantityUOM, value); }
    }

    double _RequiredQuantityUOM;
    public double RequiredQuantityUOM 
    {
        get { return _RequiredQuantityUOM; }
        set { SetProperty<double>(ref _RequiredQuantityUOM, value); }
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
        get { return LazyLoader.Load(this, ref _Material); } 
        set { SetProperty<Material>(ref _Material, value); }
    }

    public bool Material_IsLoaded
    {
        get
        {
            return _Material != null;
        }
    }

    public virtual ReferenceEntry MaterialReference 
    {
        get { return Context.Entry(this).Reference("Material"); }
    }
    
    private PlanningMR _PlanningMR;
    public virtual PlanningMR PlanningMR
    { 
        get { return LazyLoader.Load(this, ref _PlanningMR); } 
        set { SetProperty<PlanningMR>(ref _PlanningMR, value); }
    }

    public bool PlanningMR_IsLoaded
    {
        get
        {
            return _PlanningMR != null;
        }
    }

    public virtual ReferenceEntry PlanningMRReference 
    {
        get { return Context.Entry(this).Reference("PlanningMR"); }
    }
    }
