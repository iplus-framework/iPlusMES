// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MachineMaterialRelView : VBEntityObject, ISequence
{

    public MachineMaterialRelView()
    {
    }

    private MachineMaterialRelView(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    string _ProgramNo;
    public string ProgramNo 
    {
        get { return _ProgramNo; }
        set { SetProperty<string>(ref _ProgramNo, value); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
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
