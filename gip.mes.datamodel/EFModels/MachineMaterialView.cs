﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MachineMaterialView : VBEntityObject
{

    public MachineMaterialView()
    {
    }

    private MachineMaterialView(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    long? _Nr;
    public long? Nr 
    {
        get { return _Nr; }
        set { SetProperty<long?>(ref _Nr, value); }
    }

    string _ProgramNo;
    public string ProgramNo 
    {
        get { return _ProgramNo; }
        set { SetProperty<string>(ref _ProgramNo, value); }
    }

    string _MaterialNo;
    public string MaterialNo 
    {
        get { return _MaterialNo; }
        set { SetProperty<string>(ref _MaterialNo, value); }
    }

    string _MaterialName1;
    public string MaterialName1 
    {
        get { return _MaterialName1; }
        set { SetProperty<string>(ref _MaterialName1, value); }
    }

    Guid _ACClassID;
    public Guid ACClassID 
    {
        get { return _ACClassID; }
        set { SetProperty<Guid>(ref _ACClassID, value); }
    }

    string _MachineName;
    public string MachineName 
    {
        get { return _MachineName; }
        set { SetProperty<string>(ref _MachineName, value); }
    }

    Guid _BasedOnACClassID;
    public Guid BasedOnACClassID 
    {
        get { return _BasedOnACClassID; }
        set { SetProperty<Guid>(ref _BasedOnACClassID, value); }
    }

    string _BasedOnMachineName;
    public string BasedOnMachineName 
    {
        get { return _BasedOnMachineName; }
        set { SetProperty<string>(ref _BasedOnMachineName, value); }
    }

    double? _InwardTargetQuantityUOM;
    public double? InwardTargetQuantityUOM 
    {
        get { return _InwardTargetQuantityUOM; }
        set { SetProperty<double?>(ref _InwardTargetQuantityUOM, value); }
    }

    double? _InwardActualQuantityUOM;
    public double? InwardActualQuantityUOM 
    {
        get { return _InwardActualQuantityUOM; }
        set { SetProperty<double?>(ref _InwardActualQuantityUOM, value); }
    }

    double? _OutwardTargetQuantityUOM;
    public double? OutwardTargetQuantityUOM 
    {
        get { return _OutwardTargetQuantityUOM; }
        set { SetProperty<double?>(ref _OutwardTargetQuantityUOM, value); }
    }

    double? _OutwardActualQuantityUOM;
    public double? OutwardActualQuantityUOM 
    {
        get { return _OutwardActualQuantityUOM; }
        set { SetProperty<double?>(ref _OutwardActualQuantityUOM, value); }
    }
}
